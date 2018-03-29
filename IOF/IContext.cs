using IOF.Models;
using System;
using System.Security.Principal;

namespace IOF
{
    public interface IContext
    {
        IPrincipal User { get; set; }
        Uri Url { get; }
        string VirtualToAbsolute(string virtualPath);
        Uri VirtualToUri(string virtualPath);
        void SignIn(Client client);
        Client CurrentUser { get; }
        object GetSessionValue(string key);
        void SetSessionValue(string key, object value);
        void RemoveSessionValue(string key);
        string MapPath(string path);
    }
}
