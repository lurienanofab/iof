using IOF.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.xml.xmp;
using LNF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Ordering = LNF.Repository.Ordering;

namespace IOF.Impl
{
    public class PdfService : IPdfService
    {
        public IOrderRepository OrderRepository { get; }
        public IClientRepository ClientRepository { get; }
        public IAccountRepository AccountRepository { get; }
        public IVendorRepository VendorRepository { get; }
        public IItemRepository ItemRepository { get; }
        public IDetailRepository DetailRepository { get; }

        public PdfService(IOrderRepository orderRepo, IClientRepository clientRepo, IAccountRepository accountRepo, IVendorRepository vendorRepo, IItemRepository itemRepo, IDetailRepository detailRepo)
        {
            OrderRepository = orderRepo;
            ClientRepository = clientRepo;
            AccountRepository = accountRepo;
            VendorRepository = vendorRepo;
            ItemRepository = itemRepo;
            DetailRepository = detailRepo;
        }

        public string CreatePDF(int poid)
        {
            var po = OrderRepository.Single(poid);

            if (po == null)
                throw new ItemNotFoundException<Ordering.PurchaseOrder>(x => x.POID, poid);

            var basePath = ConfigurationManager.AppSettings["PdfBasePath"];

            if (string.IsNullOrEmpty(basePath))
                throw new InvalidOperationException("Missing required appSetting: PdfBasePath");

            if (!Directory.Exists(basePath))
                throw new InvalidOperationException($"Directory not found: {basePath}");

            string tempPath = Path.Combine(basePath, "temp");

            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);

            string imagePath = Path.Combine(basePath, "images");

            if (!Directory.Exists(imagePath))
                Directory.CreateDirectory(imagePath);

            string template = Path.Combine(basePath, "IOFTemplate.pdf");

            if (!File.Exists(template))
                throw new InvalidOperationException($"Missing template file: {template}");

            string templatePage2 = Path.Combine(basePath, "IOFTemplatePage2.pdf");

            if (!File.Exists(templatePage2))
                throw new InvalidOperationException($"Missing template page 2 file: {templatePage2}");

            var c = ClientRepository.Single(po.ClientID);

            string pdfName = $"IOF_{po.POID}_{c.LName}{c.FName}_{DateTime.Now:yyMmddHHmmss}.pdf";
            string pdfPath = Path.Combine(tempPath, pdfName);

            if (File.Exists(pdfPath)) File.Delete(pdfPath);

            using (var fs = new FileStream(pdfPath, FileMode.Create, FileAccess.Write))
            {
                PdfReader pr = new PdfReader(template);
                PdfStamper ps = new PdfStamper(pr, fs);
                AcroFields af = ps.AcroFields;

                var info = pr.Info;
                info["Title"] = $"IOF #{po.POID} : {po.VendorName} : {c.FName} {c.LName}";
                ps.MoreInfo = info;

                using (var ms = new MemoryStream())
                {
                    var xmp = new XmpWriter(ms, info);
                    ps.XmpMetadata = ms.ToArray();
                    xmp.Close();
                }

                af.SetField("poID", $"IOF # {po.POID}");
                af.SetField("RequestedBy", c.DisplayName);
                af.SetField("Email", c.Email);
                af.SetField("Phone", c.Phone);

                var approver = ClientRepository.GetApprover(po);

                if (po.IsApproved())
                {
                    string imageFile = Path.Combine(imagePath, $"{approver.LName}.tif");
                    if (File.Exists(imageFile))
                    {
                        var sig = Image.GetInstance(imageFile);
                        ps.GetOverContent(1).AddImage(sig, 50, 0, 0, 24, 231, 539);
                    }
                    af.SetField("ApprovedBy", approver.DisplayName);
                }

                // fill in shipping info
                af.SetField("Date", po.CreatedDate.ToShortDateString());
                af.SetField("DateNeeded", po.NeededDate.ToShortDateString());
                af.SetField("Advisor", approver.DisplayName);
                af.SetField("Shipping", po.ShippingMethodName);

                var acct = AccountRepository.Single(po.ClientID, po.AccountID);

                if (acct == null)
                    af.SetField("Account", string.Empty);
                else
                    af.SetField("Account", acct.ShortCode);

                if (po.Oversized) af.SetField("chkOverSized", "Yes");
                af.SetField("PurchaseTotal", po.TotalPrice.ToString("#,##0.00"));

                // fill in vendor info
                var vendor = VendorRepository.Single(po.VendorID);
                af.SetField("Vendor", vendor.VendorName);
                af.SetField("VendAddr1", vendor.Address1);
                af.SetField("VendAddr2", vendor.Address2);
                af.SetField("VendAddr3", vendor.Address3);
                af.SetField("Contact", vendor.Contact);
                af.SetField("ContactPhone", vendor.Phone);
                af.SetField("ContactFax", vendor.Fax);
                af.SetField("VendorWWW", vendor.URL);
                af.SetField("ContactEmail", vendor.Email);

                // in the 'office use only' box
                af.SetField("VendorOffice", vendor.VendorName);

                // notes
                var items = GetItems(po.POID).ToList();

                if (!string.IsNullOrEmpty(po.Notes))
                    items.Add(new PdfItem() { Description = po.Notes, IsNotes = true });

                // items
                int x = 0;
                float y = 410F; //page 1 starting position
                int pageNumber = 1;
                float width = pr.GetPageSize(pageNumber).Width - x;

                //DrawLine(ps.GetOverContent(page), 280)

                foreach (var item in items.OrderByDescending(i => i.PODID))
                {
                    var tbl = CreateTable(item, width);
                    if (pageNumber < 2)
                    {
                        if (y + tbl.TotalHeight < 280)
                        {
                            pageNumber = 2;
                            y = 580; //page 2 starting position
                            ps.InsertPage(pageNumber, pr.GetPageSize(1));
                            var p2reader = new PdfReader(templatePage2);
                            ps.ReplacePage(p2reader, 1, 2);
                            ps.Writer.FreeReader(p2reader);
                            p2reader.Close();

                            //DrawLine(ps.GetOverContent(page), 60)
                        }
                    }
                    else
                    {
                        if (y + tbl.TotalHeight < 60)
                        {
                            pageNumber += 1;
                            y = 580; //page n starting position
                            ps.InsertPage(pageNumber, pr.GetPageSize(1));
                            var p2reader = new PdfReader(templatePage2);
                            ps.ReplacePage(p2reader, 1, pageNumber);
                            ps.Writer.FreeReader(p2reader);
                            p2reader.Close();

                            //DrawLine(ps.GetOverContent(page), 60)
                        }
                    }

                    var ct = new ColumnText(ps.GetOverContent(pageNumber));
                    ct.Alignment = Element.ALIGN_LEFT;
                    ct.SetSimpleColumn(x, y, width, y + 100);
                    ct.AddElement(tbl);
                    ct.Go();
                    y -= tbl.TotalHeight;
                }

                AddPageNumberFooter(ps, pr.NumberOfPages, width);

                ps.FormFlattening = true;
                ps.Close();
            }

            return pdfPath;
        }

        private static PdfPTable CreateTable(PdfItem item, float width)
        {
            var tbl = new PdfPTable(10);
            tbl.TotalWidth = width;
            tbl.LockedWidth = true;
            tbl.HorizontalAlignment = Element.ALIGN_LEFT;

            float[] wcol =
            {
                35.0F,  //left padding
                54.0F,  //Label
                229.0F, //Part Number/Description
                28.0F,  //QTY
                30.0F,  //REC
                32.0F,  //B.O.
                40.0F,  //CAT
                68.0F,  //UNIT PRICE
                75.0F,  //ITEM TOTAL
                0.0F    //RIGHT PADDING
            };

            wcol[9] = width - (wcol[0] + wcol[1] + wcol[2] + wcol[3] + wcol[4] + wcol[5] + wcol[6] + wcol[7] + wcol[8]);

            if (wcol[9] < 0)
                throw new Exception($"Total column widths exceed page width by {Math.Abs(wcol[9])}");

            tbl.SetTotalWidth(wcol);

            if (!item.IsNotes)
            {
                // first row
                tbl.AddCell(GetCell(string.Empty, 1, Element.ALIGN_UNDEFINED, Element.ALIGN_UNDEFINED, false));
                tbl.AddCell(GetCell("Part Number:", 1, Element.ALIGN_LEFT, Element.ALIGN_TOP, false));
                tbl.AddCell(GetCell(item.PartNum, 1, Element.ALIGN_LEFT, Element.ALIGN_TOP, false));
                tbl.AddCell(GetCell(string.Empty, 7, Element.ALIGN_UNDEFINED, Element.ALIGN_UNDEFINED, false));

                // second row
                tbl.AddCell(GetCell(string.Empty, 1, Element.ALIGN_UNDEFINED, Element.ALIGN_UNDEFINED, false));
                tbl.AddCell(GetCell("Description:", 1, Element.ALIGN_LEFT, Element.ALIGN_TOP, true));
                tbl.AddCell(GetCell(item.Description, 1, Element.ALIGN_LEFT, Element.ALIGN_TOP, true));
                tbl.AddCell(GetCell(GetQtyAndUnit(item), 1, Element.ALIGN_RIGHT, Element.ALIGN_BOTTOM, true));
                tbl.AddCell(GetCell(string.Empty, 1, Element.ALIGN_UNDEFINED, Element.ALIGN_UNDEFINED, true));
                tbl.AddCell(GetCell(string.Empty, 1, Element.ALIGN_UNDEFINED, Element.ALIGN_UNDEFINED, true));
                tbl.AddCell(GetCell(item.CategoryNumber, 1, Element.ALIGN_RIGHT, Element.ALIGN_BOTTOM, true));
                tbl.AddCell(GetCell(item.UnitPrice.ToString("C"), 1, Element.ALIGN_RIGHT, Element.ALIGN_BOTTOM, true));
                tbl.AddCell(GetCell(item.ExtPrice.ToString("C"), 1, Element.ALIGN_RIGHT, Element.ALIGN_BOTTOM, true));
                tbl.AddCell(GetCell(string.Empty, 1, Element.ALIGN_UNDEFINED, Element.ALIGN_UNDEFINED, false));
            }
            else
            {
                tbl.AddCell(GetCell(string.Empty, 10, Element.ALIGN_UNDEFINED, Element.ALIGN_UNDEFINED, false));
                tbl.AddCell(GetCell(string.Empty, 10, Element.ALIGN_UNDEFINED, Element.ALIGN_UNDEFINED, false));
                tbl.AddCell(GetCell(string.Empty, 1, Element.ALIGN_UNDEFINED, Element.ALIGN_UNDEFINED, false));
                tbl.AddCell(GetCell("Notes:", 1, Element.ALIGN_LEFT, Element.ALIGN_TOP, false));
                tbl.AddCell(GetCell(item.Description, 1, Element.ALIGN_LEFT, Element.ALIGN_TOP, false));
                tbl.AddCell(GetCell(string.Empty, 7, Element.ALIGN_UNDEFINED, Element.ALIGN_UNDEFINED, false));
            }

            return tbl;
        }

        private static PdfPCell GetCell(object partNum, int v1, int aLIGN_LEFT, int aLIGN_TOP, bool v2)
        {
            throw new NotImplementedException();
        }

        private static PdfPCell GetCell(string ctext, int colspan, int halign, int valign, bool border)
        {
            // create an inner table for cell spacing (otherwise we get one long solid border)
            var tbl = new PdfPTable(1);
            ctext = string.IsNullOrEmpty(ctext) ? string.Empty : ctext;
            var c = new PdfPCell(new Phrase(ctext.Trim(), new Font(Font.FontFamily.TIMES_ROMAN, 8.0F)));
            c.HorizontalAlignment = halign;
            c.VerticalAlignment = valign;

            if (border)
            {
                c.PaddingBottom = 5.0F;
                c.Border = Rectangle.BOTTOM_BORDER;
                c.BorderWidthBottom = 1.0F;
            }
            else
            {
                c.Border = Rectangle.NO_BORDER;
            }

            tbl.AddCell(c);

            var result = new PdfPCell(tbl);
            result.Colspan = colspan;
            result.Border = Rectangle.NO_BORDER;
            result.PaddingRight = 2.0F;
            result.PaddingLeft = 2.0F;

            return result;
        }

        private static string GetQtyAndUnit(PdfItem item)
        {
            var result = item.Quantity.ToString();
            if (!string.IsNullOrEmpty(item.Unit))
                result += " " + item.Unit;
            return result;
        }

        private static void AddPageNumberFooter(PdfStamper ps, int totalPages, float pageWidth)
        {
            for (int p = 1; p <= totalPages; p++)
            {
                var cb = ps.GetUnderContent(p);
                var footerX = pageWidth - 25.0F;
                var footerY = 15.0F;
                cb.BeginText();
                cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 8.0F);
                cb.SetTextMatrix(footerX, footerY);
                cb.ShowTextAligned(Element.ALIGN_RIGHT, $"Page {p} of {totalPages}", footerX, footerY, 0.0F);
                cb.EndText();
            }
        }

        private static void DrawLine(PdfContentByte cb, int y)
        {
            cb.SetLineWidth(0.5F); //thin
            cb.SetGrayStroke(0.0F); //black
            cb.MoveTo(0, y);
            cb.LineTo(cb.PdfDocument.PageSize.Width, y);
            cb.Stroke();
        }

        private IEnumerable<PdfItem> GetItems(int poid)
        {
            var items = DetailRepository.GetOrderDetails(poid);

            var result = items.Select(x => new PdfItem()
            {
                PODID = x.PODID,
                Quantity = x.Quantity,
                Unit = x.Unit,
                UnitPrice = x.UnitPrice,
                Description = x.Description,
                PartNum = x.PartNum,
                CategoryID = x.CategoryID,
                ParentID = x.ParentID,
                CategoryNumber = x.CategoryNumber,
                CreatedDate = x.CreatedDate,
                IsNotes = false
            });

            return result;
        }
    }
}
