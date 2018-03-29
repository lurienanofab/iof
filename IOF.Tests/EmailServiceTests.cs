using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IOF.Tests
{
    [TestClass]
    public class EmailServiceTests : TestBase
    {
        [TestMethod]
        public void EmailService_CanSendApproverEmail()
        {
            EmailService.SendApproverEmail(13154);
        }

        [TestMethod]
        public void EmailService_CanSendPurchaserEmail()
        {
            var filePath = PdfService.CreatePDF(13154);
            EmailService.SendPurchaserEmail(13154, filePath);
        }

        [TestMethod]
        public void EmailService_CanSendItemModifiedEmail()
        {
            int podid = 28080; // belongs to a PO that has an assigned purchaser
            string[] changes = { "test1", "test2" };
            EmailService.SendItemModifiedEmail(podid, changes);
        }

        [TestMethod]
        public void EmailService_CanSendAddAttachmentsEmail()
        {
            int poid = 13154;

            string url = Context.VirtualToUri($"~/attachments/IOF{poid}.pdf").ToString();
            Assert.AreEqual($"http://lnf-dev.eecs.umich.edu/iof/attachments/IOF{poid}.pdf", url);

            var attachment = new Models.Attachment()
            {
                POID = 13154,
                FilePath = $"c:\\test\\attachments\\IOF{poid}.pdf",
                Url = url
            };

            EmailService.SendAddAttachmentsEmail(poid, new Models.Attachment[] { attachment });
        }

        [TestMethod]
        public void EmailService_CanSendCancelOrderEmail()
        {
            EmailService.SendCancelOrderEmail(13154, "This is a test.");
        }

        [TestMethod]
        public void EmailService_CanSendDeleteAttachmentEmail()
        {
            int poid = 13154;
            EmailService.SendDeleteAttachmentEmail(poid, $"IOF{poid}.pdf");
        }

        [TestMethod]
        public void EmailService_CanSendRejectEmail()
        {
            int poid = 13154;
            EmailService.SendRejectEmail(poid, "Testing send reject email.");
        }

        [TestMethod]
        public void EmailService_CanGetApprovalProcessParameters()
        {
            Models.ApprovalProcessParameters parameters;

            parameters = EmailService.GetApprovalProcessParameters("sdy9DMV0PTzY36PJ+iHD2G4nz6hp0gd60JvVHnC22p3Yw8In+qAadH4QLt3yC8QQ");
            Assert.AreEqual(13154, parameters.POID);
            Assert.AreEqual(155, parameters.ApproverID);
            Assert.AreEqual("Approve", parameters.Action);

            parameters = EmailService.GetApprovalProcessParameters("hFqtb1T7jHB/TeP/EYpAdks6tP4SNFtuXv3Q4UWIcmobVGd3OxK8ZWS6fO8GC47u");
            Assert.AreEqual(13154, parameters.POID);
            Assert.AreEqual(155, parameters.ApproverID);
            Assert.AreEqual("Reject", parameters.Action);
        }
    }
}
