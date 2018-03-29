using IOF.Models;
using System.Collections.Generic;

namespace IOF
{
    public interface IEmailService
    {
        void SendItemModifiedEmail(int podid, IEnumerable<string> changes);

        void SendAddAttachmentsEmail(int poid, IEnumerable<Attachment> attachments);

        void SendDeleteAttachmentEmail(int poid, string attachmentFileName);

        void SendCancelOrderEmail(int poid, string notes);

        void SendRejectEmail(int poid, string reason);

        void SendPurchaserEmail(int poid, string attachmentFilePath);

        void SendApproverEmail(int poid);

        ApprovalProcessParameters GetApprovalProcessParameters(string encrypted);
    }
}
