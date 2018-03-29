using System.IO;

namespace IOF.Models
{
    public class Attachment
    {
        public int POID { get; set; }
        public string FilePath { get; set; }
        public string FileName => Path.GetFileName(FilePath);
        public string Url { get; set; }
        public bool Uploaded { get; set; }
        public bool Deleted { get; set; }
    }
}
