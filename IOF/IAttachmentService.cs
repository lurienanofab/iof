using IOF.Models;
using System.Collections.Generic;

namespace IOF
{
    public interface IAttachmentService
    {
        IEnumerable<Attachment> GetAttachments(int poid);
        Attachment SaveAttachment(int poid, string fileName, byte[] data);
        Attachment DeleteAttachment(int poid, string fileName);
        byte[] GetBytes(int poid, string fileName);
        IEnumerable<int> GetFolders();
        bool Exists(int poid, string fileName);
    }
}
