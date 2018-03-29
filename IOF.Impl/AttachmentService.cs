using IOF.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;

namespace IOF.Impl
{
    public class AttachmentService : IAttachmentService
    {
        public IContext Context { get; }

        public AttachmentService(IContext context)
        {
            Context = context;
        }

        public IEnumerable<Attachment> GetAttachments(int poid)
        {
            if (poid == 0)
                throw new ArgumentException("The argument poid cannot be zero.", "poid");

            string path = GetPhysicalPath(poid);

            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path);

                if (files.Length > 0)
                {
                    foreach (var f in files)
                    {
                        yield return new Attachment()
                        {
                            POID = poid,
                            FilePath = f,
                            Url = GetFullUrl(poid, Path.GetFileName(f)).ToString(),
                            Uploaded = false,
                            Deleted = false
                        };
                    }
                }
            }
        }

        public Attachment SaveAttachment(int poid, string fileName, byte[] data)
        {
            if (data.Length == 0)
                throw new InvalidOperationException("Cannot save zero length file.");

            var filePath = Path.Combine(GetPhysicalPath(poid), fileName);

            var url = GetFullUrl(poid, fileName);

            var result = new Attachment()
            {
                POID = poid,
                FilePath = filePath,
                Url = url.ToString(),
                Uploaded = true,
                Deleted = false
            };

            File.WriteAllBytes(filePath, data);

            return result;
        }

        public Attachment DeleteAttachment(int poid, string fileName)
        {
            var filePath = Path.Combine(GetPhysicalPath(poid), fileName);

            var url = GetFullUrl(poid, fileName);

            var result = new Attachment()
            {
                POID = poid,
                FilePath = filePath,
                Url = url.ToString(),
                Uploaded = false,
                Deleted = false
            };

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                result.Deleted = true;
            }

            return result;
        }

        public byte[] GetBytes(int poid, string fileName)
        {
            return File.ReadAllBytes(Path.Combine(GetPhysicalPath(poid), fileName));
        }

        public IEnumerable<int> GetFolders()
        {
            string root = ConfigurationManager.AppSettings["AttachmentsDirectory"];

            if (root.StartsWith("."))
                root = Context.MapPath(root);

            var result = new List<int>();

            if (Directory.Exists(root))
            {
                string[] dirs = Directory.GetDirectories(root);

                foreach (string dirPath in dirs)
                {
                    string dirName = Path.GetFileName(dirPath);

                    if (int.TryParse(dirName, out int poid))
                    {
                        if (Directory.GetFiles(dirPath).Length > 0)
                            result.Add(poid);
                    }
                }
            }

            return result.ToArray();
        }

        public bool Exists(int poid, string fileName)
        {
            return File.Exists(Path.Combine(GetPhysicalPath(poid), fileName));
        }

        private string GetPhysicalPath(int poid)
        {
            string attachmentsDir = ConfigurationManager.AppSettings["AttachmentsDirectory"];

            if (string.IsNullOrEmpty(attachmentsDir))
                throw new InvalidOperationException("Missing required appSetting: AttachmentsDirectory");

            if (attachmentsDir.StartsWith("."))
                attachmentsDir = Context.MapPath(attachmentsDir);

            if (!Directory.Exists(attachmentsDir))
                Directory.CreateDirectory(attachmentsDir);

            string result = Path.Combine(attachmentsDir, poid.ToString());

            if (!Directory.Exists(result))
                Directory.CreateDirectory(result);

            return result;
        }

        private Uri GetFullUrl(int poid, string fileName)
        {
            string vp = ConfigurationManager.AppSettings["AttachmentsVirtualPath"]; //should contain {0} and {1} to be replaced by POID and FileName

            if (string.IsNullOrEmpty(vp))
                throw new InvalidOperationException("Missing required appSetting: AttachmentsVirtualPath");

            Uri result = Context.VirtualToUri(string.Format(vp, poid, HttpUtility.UrlEncode(fileName)));

            return result;
        }
    }
}
