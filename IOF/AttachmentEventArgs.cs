using IOF.Models;
using System.Collections.Generic;

namespace IOF
{
    public class AttachmentEventArgs
    {
        public AttachmentEventArgs(int poid, IEnumerable<Attachment> attachments)
        {
            POID = poid;
            Attachments = attachments;
        }

        public int POID { get; private set; }
        public IEnumerable<Attachment> Attachments { get; private set; }
        public bool Uploaded { get; private set; }
    }
}
