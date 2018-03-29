using IOF.Models;
using LNF;
using LNF.Email;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using Encryption = SimpleEncryption.Encryption;

namespace IOF.Impl
{
    public class EmailService : IEmailService
    {
        public IContext Context { get; }
        public IOrderRepository OrderRepository { get; }
        public IDetailRepository DetailRepository { get; }
        public IClientRepository ClientRepository { get; }
        public IAccountRepository AccountRepository { get; }
        public IAttachmentService AttachmentService { get; }

        public EmailService(IContext context, IOrderRepository orderRepo, IDetailRepository detailRepo, IClientRepository clientRepo, IAccountRepository accountRepo, IAttachmentService attachmentSvc)
        {
            Context = context;
            OrderRepository = orderRepo;
            DetailRepository = detailRepo;
            ClientRepository = clientRepo;
            AccountRepository = accountRepo;
            AttachmentService = attachmentSvc;
        }

        public void SendItemModifiedEmail(int podid, IEnumerable<string> changes)
        {
            var detail = DetailRepository.Single(podid);
            var order = OrderRepository.Single(detail.POID);

            StringBuilder body = new StringBuilder();

            body.AppendLine($"An item on IOF #{order.POID} was modified by <a href=\"mailto:{Context.CurrentUser.Email}\">{Context.CurrentUser.DisplayName}</a>");
            body.AppendLine($"<br><br><b>{detail.Description}:</b>");
            body.AppendLine("<ul>");
            foreach (var c in changes)
                body.AppendLine($"<li>{c}</li>");
            body.AppendLine("</ul>");

            var args = CreateArgs("IOF.Impl.EmailService.SendItemModifiedEmail", $"IOF #{order.POID} modified by purchaser", GetBody(order, body), GetClientEmail(order), GetApproverEmail(order), GetPurchaserEmail(order));

            Providers.Email.SendMessage(args);
        }

        public void SendAddAttachmentsEmail(int poid, IEnumerable<Attachment> attachments)
        {
            var order = OrderRepository.Single(poid);

            var sb = new StringBuilder();
            sb.AppendLine($"<b>The following attachments were added to your IOF on {DateTime.Now:MM/dd/yyyy hh:mm:ss tt} by {Context.CurrentUser.DisplayName}</b>");
            sb.AppendLine("<ol>");
            foreach (var att in attachments)
                sb.AppendLine($"<li><a href=\"{att.Url}\">{att.FileName}</a></li>");
            sb.AppendLine("</ol>");

            var args = CreateArgs("IOF.Impl.EmailService.SendAddAttachmentsEmail", $"IOF #{order.POID} attachment added by purchaser", GetBody(order, sb), GetClientEmail(order), GetApproverEmail(order), GetPurchaserEmail(order));

            Providers.Email.SendMessage(args);
        }

        public void SendDeleteAttachmentEmail(int poid, string attachmentFileName)
        {
            var order = OrderRepository.Single(poid);

            var sb = new StringBuilder();
            sb.AppendLine($"<b>The following attachment was deleted from your IOF on {DateTime.Now:MM/dd/yyyy hh:mm:ss tt} by {Context.CurrentUser.DisplayName}</b>");
            sb.AppendLine("<ol>");
            sb.AppendLine($"<li>{attachmentFileName}</li>");
            sb.AppendLine("</ol>");

            var args = CreateArgs("IOF.Impl.EmailService.SendDeleteAttachmentEmail", $"IOF #{poid} attachment deleted by purchaser", GetBody(order, sb), GetClientEmail(order), GetApproverEmail(order), GetPurchaserEmail(order));

            Providers.Email.SendMessage(args);
        }

        public void SendCancelOrderEmail(int poid, string notes)
        {
            var order = OrderRepository.Single(poid);

            var sb = new StringBuilder();
            sb.AppendLine($"<b>This IOF was canceled on {DateTime.Now:MM/dd/yyyy hh:mm:ss tt} by {Context.CurrentUser.DisplayName}</b>");
            sb.AppendLine($"<br><br>Purchaser Notes:<table style=\"width: 100%;\"><tr><td style=\"border: solid 1px #ccc; background-color: #eee; padding: 5px;\">{notes}</td></tr></table>");

            var args = CreateArgs("IOF.Impl.EmailService.SendCancelOrderEmail", $"IOF #{order.POID} canceled by purchaser", GetBody(order, sb), GetClientEmail(order), GetApproverEmail(order), GetPurchaserEmail(order));

            Providers.Email.SendMessage(args);
        }

        public void SendRejectEmail(int poid, string reason)
        {
            var order = OrderRepository.Single(poid);
            var client = ClientRepository.Single(order.ClientID);

            var name = client.DisplayName;
            var email = client.Email;

            var sb = new StringBuilder();
            sb.AppendLine($"Hi {name}, <br><br>");
            sb.AppendLine($"Your IOF #{order.POID} has been rejected by {Context.CurrentUser.DisplayName}.<br><br>");

            if (!string.IsNullOrEmpty(reason))
            {
                sb.Append("<hr>");
                sb.AppendLine($"<b>Approver Notes:</b><p>{reason.Replace(((char)13).ToString(), string.Empty).Replace(((char)10).ToString(), "<br>")}</p>");
            }

            var args = CreateArgs("LNF.Impl.EmailService.SendRejectEmail", $"IOF #{order.POID}: Rejected", sb.ToString(), email);

            Providers.Email.SendMessage(args);
        }

        public void SendApproverEmail(int poid)
        {
            var order = OrderRepository.Single(poid);
            var acct = AccountRepository.Single(order.ClientID, order.AccountID);

            if (acct == null)
                throw new Exception("The IOF must have an account before sending for approval.");

            var client = ClientRepository.Single(order.ClientID);
            var approver = ClientRepository.GetApprover(order);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<div style=\"margin-top: 20px;\">");
            sb.AppendLine($"<h3>Approver: {approver.DisplayName}</h3>");

            sb.AppendLine("<hr/>");

            string style1 = "text-align: right; padding: 6px 6px 6px 24px;";
            string style2 = "padding: 6px 6px 6px 12px";

            sb.AppendLine("<div style=\"padding: 10px;\">");
            sb.AppendLine("<div style=\"padding-bottom: 10px;\"><b>IOF Info</b></div>");
            sb.AppendLine("<table style=\"border-collapse: collapse;\">");
            sb.AppendLine($"<tr><td style=\"{style1}\"><b>IOF #</b></td><td style=\"{style2}\">{order.POID}</td></tr>");
            sb.AppendLine($"<tr><td style=\"{style1}\"><b>Vendor</b></td><td style=\"{style2}\">{order.VendorName}</td></tr>");
            sb.AppendLine($"<tr><td style=\"{style1}\"><b>Account</b></td><td style=\"{style2}\">{acct.AccountDisplayName}</td></tr>");
            sb.AppendLine($"<tr><td style=\"{style1}\"><b>Date Needed</b></td><td style=\"{style2}\">{order.NeededDate:M/d/yyyy}</td></tr>");
            sb.AppendLine($"<tr><td style=\"{style1}\"><b>Notes</b></td><td style=\"{style2};\">{order.Notes}</td></tr>");
            sb.AppendLine("</table>");
            sb.AppendLine("</div>");

            style1 = "border: solid 1px #444444; padding: 6px; background-color: #5078b3; color: #ffffff;";

            sb.AppendLine("<hr/>");

            sb.AppendLine("<div style=\"padding: 10px;\">");
            sb.AppendLine("<div style=\"padding-bottom: 10px;\"><b>IOF Items</b></div>");
            sb.AppendLine($"<table style=\"border-collapse: collapse; width: 100%;\">");
            sb.AppendLine("<tr>");
            sb.AppendLine($"<th style=\"{style1} width: 150px; text-align: left;\">Part #</th>");
            sb.AppendLine($"<th style=\"{style1} text-align: left;\">Description</th>");
            sb.AppendLine($"<th style=\"{style1} width: 150px; text-align: left;\">Category</th>");
            sb.AppendLine($"<th style=\"{style1} width: 60px; text-align: right;\">Qty</th>");
            sb.AppendLine($"<th style=\"{style1} width: 90px; text-align: right;\">Unit Price</th>");
            sb.AppendLine($"<th style=\"{style1} width: 90px; text-align: right;\">Ext Price</th>");
            sb.AppendLine("</tr>");

            style1 = "border: solid 1px #808080; padding: 6px;";

            double total = 0;

            foreach (var pod in DetailRepository.GetOrderDetails(order.POID))
            {
                sb.AppendLine("<tr>");
                sb.AppendLine($"<td style=\"{style1}\">{pod.PartNum}</td>");
                sb.AppendLine($"<td style=\"{style1}\">{pod.Description}</td>");
                sb.AppendLine($"<td style=\"{style1}\">{pod.CategoryNumber} - {pod.CategoryName}</td>");
                sb.AppendLine($"<td style=\"{style1} text-align: right;\">{$"{pod.Quantity} {pod.Unit}".Trim()}</td>");
                sb.AppendLine($"<td style=\"{style1} text-align: right;\">{pod.UnitPrice:C}</td>");
                sb.AppendLine($"<td style=\"{style1} text-align: right;\">{pod.ExtPrice:C}</td>");
                sb.AppendLine("</tr>");
                total += pod.ExtPrice;
            }

            style1 = "border: solid 1px #808080; padding: 6px; background-color: #ccff99;";

            sb.AppendLine("<tr>");
            sb.AppendLine($"<td style=\"{style1}\">&nbsp;</td>");
            sb.AppendLine($"<td style=\"{style1}\">&nbsp;</td>");
            sb.AppendLine($"<td style=\"{style1}\">&nbsp;</td>");
            sb.AppendLine($"<td style=\"{style1}\">&nbsp;</td>");
            sb.AppendLine($"<td style=\"{style1} text-align: right;\"><b>Total:</b></td>");
            sb.AppendLine($"<td style=\"{style1} text-align: right;\"><b>{total:C}</b></td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("</table>");
            sb.AppendLine("</div>");

            sb.AppendLine("<hr/>");

            sb.AppendLine("<div style=\"padding: 10px;\">");
            sb.AppendLine("<div style=\"padding-bottom: 10px;\"><b>Attachments</b></div>");

            var attachments = AttachmentService.GetAttachments(order.POID);

            if (attachments.Count() > 0)
            {
                sb.AppendLine("<ol>");
                foreach (var a in attachments)
                    sb.AppendLine($"<li><a href=\"{a.Url}\">{a.FileName}</a></li>");
                sb.AppendLine("</ol>");
            }
            else
            {
                sb.AppendLine("<div><i style=\"color: #808080;\">No attachments found.</i></div>");
            }

            sb.AppendLine("</div>");

            sb.AppendLine("<hr/>");

            sb.AppendLine("<div style=\"padding: 10px;\">");
            sb.AppendLine($"<div style=\"margin-bottom: 20px;\"><b><a href=\"{GetApproveUrl(order)}\">Approve this IOF</a></b></div>");
            sb.AppendLine($"<div style=\"margin-bottom: 20px;\"><b><a href=\"{GetRejectUrl(order)}\">Reject this IOF</a></b></div>");
            sb.AppendLine("</div>");

            sb.AppendLine("</div>");

            var args = CreateArgs("IOF.Impl.EmailService.SendApproverEmail", $"IOF #{order.POID}: Request By {client.DisplayName}", sb.ToString(), approver.Email);

            Providers.Email.SendMessage(args);
        }

        public void SendPurchaserEmail(int poid, string attachmentFilePath)
        {
            var po = OrderRepository.Single(poid);

            // Send Email to Purchaser, Approver and IOF Owner
            var client = ClientRepository.Single(po.ClientID);
            var approver = ClientRepository.GetApprover(po);

            double totalWarningMin = GetTotalWarningMinimum();
            int over = Convert.ToInt32(totalWarningMin / 1000);
            string subject = $"IOF #{po.POID}: Approved"
                + (po.TotalPrice > totalWarningMin ? $" (over ${over}k)" : string.Empty)
                + (po.Attention.GetValueOrDefault() ? " (URGENT)" : string.Empty);

            string body = $"IOF #{po.POID} has been approved. Please see the attachment.";

            var args = CreateArgs("IOF.Impl.EmailService.SendPurchaserEmail", subject, body, approver.Email, GetPurchasingAgentEmails(), client.Email);
            args.Bcc = GetBccEmails(new[] { "lnf-it@umich.edu" });
            args.Attachments = new[] { attachmentFilePath };

            Providers.Email.SendMessage(args);
        }

        public ApprovalProcessParameters GetApprovalProcessParameters(string encrypted)
        {
            var decrypted = Decrypt(encrypted);
            var nvc = HttpUtility.ParseQueryString(decrypted);

            var result = new ApprovalProcessParameters();
            result.POID = int.Parse(nvc["POID"] ?? throw new Exception("POID not found."));
            result.ApproverID = int.Parse(nvc["ApproverID"] ?? throw new Exception("ApproverID not found."));
            result.Action = nvc["Action"] ?? throw new Exception("Action not found.");

            return result;
        }

        private SendMessageArgs CreateArgs(string caller, string subject, string body, string to, params string[] cc)
        {
            var fromEmail = GetSystemEmail();

            var args = new SendMessageArgs()
            {
                Caller = caller,
                ClientID = Context.CurrentUser.ClientID,
                From = fromEmail,
                DisplayName = "LNF Ordering",
                To = new[] { GetToEmail(to) },
                Cc = GetCcEmail(cc),
                Subject = subject,
                Body = body,
                IsHtml = true
            };

            return args;
        }

        private string GetClientEmail(Order order)
        {
            var client = ClientRepository.Single(order.ClientID);
            return client.Email;
        }

        private string GetApproverEmail(Order order)
        {
            var approver = ClientRepository.GetApprover(order);
            return approver.Email;
        }

        private string GetPurchaserEmail(Order order)
        {
            var purchaser = ClientRepository.GetPurchaser(order);
            var purchaserEmail = string.Empty;

            if (purchaser != null)
                purchaserEmail = purchaser.Email;

            return purchaserEmail;
        }
        
        private string GetBody(Order po, StringBuilder sb)
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine("<table style=\"width: 100%;\">");
            result.AppendLine("<tr>");
            result.AppendLine("<td>");
            result.AppendLine($"<h2>IOF #{po.POID} [{po.VendorName}]</h2><hr>");
            result.AppendLine($"<p style=\"margin-top: 30px;\">{sb}</p>");
            result.AppendLine("</td>");
            result.AppendLine("</tr>");
            result.AppendLine("</table>");

            return result.ToString();
        }

        private double GetTotalWarningMinimum()
        {
            var setting = ConfigurationManager.AppSettings["TotalWarningMinimum"];

            if (string.IsNullOrEmpty(setting))
                throw new InvalidOperationException("Missing required appSetting: TotalWarningMinimum");

            return double.Parse(setting);
        }

        private string GetApproveUrl(Order order)
        {
            string host = Context.Url.GetLeftPart(UriPartial.Authority);
            string processUrl = host + Context.VirtualToAbsolute("~/ApprovalProcess.aspx");
            string approveUrl = processUrl + "?qs=" + HttpUtility.UrlEncode(Encrypt($"Action=Approve&POID={order.POID}&ApproverID={order.ApproverID}"));
            return approveUrl;
        }

        private string GetRejectUrl(Order order)
        {
            string host = Context.Url.GetLeftPart(UriPartial.Authority);
            string processUrl = host + Context.VirtualToAbsolute("~/ApprovalProcess.aspx");
            string rejectUrl = processUrl + "?qs=" + HttpUtility.UrlEncode(Encrypt($"Action=Reject&POID={order.POID}&ApproverID={order.ApproverID}"));
            return rejectUrl;
        }

        private string Encrypt(string text)
        {
            var sym = new Encryption.Symmetric(Encryption.Symmetric.Provider.Rijndael);
            var key = new Encryption.Data("lnfn@n0f@b");

            var encryptedData = sym.Encrypt(new Encryption.Data(text), key);
            var encrypted = encryptedData.ToBase64();

            return encrypted;
        }

        public static string Decrypt(string encrypted)
        {
            var sym = new Encryption.Symmetric(Encryption.Symmetric.Provider.Rijndael);
            var key = new Encryption.Data("lnfn@n0f@b");

            var encryptedData = new Encryption.Data();
            encryptedData.Base64 = encrypted;

            var decryptedData = sym.Decrypt(encryptedData, key);
            var result = decryptedData.ToString();

            return result;
        }

        private string GetSystemEmail()
        {
            var setting = ConfigurationManager.AppSettings["SystemEmail"];

            if (string.IsNullOrEmpty(setting))
                throw new InvalidOperationException("Missing required appSetting: SystemEmail");

            return setting;
        }

        private string GetPurchasingAgentEmails()
        {
            var setting = ConfigurationManager.AppSettings["PurchasingAgentEmails"];

            if (string.IsNullOrEmpty(setting))
                throw new InvalidOperationException("Missing required appSetting: PurchasingAgentEmails");

            return setting;
        }

        private string GetToEmail(string to)
        {
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["DebugEmail"]))
                return to;
            else
                return ConfigurationManager.AppSettings["DebugEmail"];
        }

        private string[] GetCcEmail(string[] cc)
        {
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["DebugEmail"]))
                return cc;
            else
                return new string[] { };
        }

        private string[] GetBccEmails(string[] bcc)
        {
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["DebugEmail"]))
                return bcc;
            else
                return new string[] { };
        }
    }
}
