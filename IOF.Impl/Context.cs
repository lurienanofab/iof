using IOF.Models;
using LNF.Models.Data;
using LNF.Web;
using System;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace IOF.Impl
{
    public class Context : IContext
    {
        public Client CurrentUser => CreateClient(HttpContext.Current.Request.GetCurrentUser());

        public Uri Url => HttpContext.Current.Request.Url;

        public IPrincipal User
        {
            get { return HttpContext.Current.User; }
            set { HttpContext.Current.User = value; }
        }

        public void SignIn(Client client)
        {
            var repo = new ClientRepository();
            var c = repo.Require<LNF.Repository.Data.ClientInfo>(x => x.ClientID, client.ClientID);
            var roles = c.Roles();

            var authCookie = FormsAuthentication.GetAuthCookie(c.UserName, true);
            var formsAuthTicket = FormsAuthentication.Decrypt(authCookie.Value);
            var ticket = new FormsAuthenticationTicket(formsAuthTicket.Version, formsAuthTicket.Name, formsAuthTicket.IssueDate, formsAuthTicket.Expiration, formsAuthTicket.IsPersistent, string.Join("|", roles), formsAuthTicket.CookiePath);
            authCookie.Value = FormsAuthentication.Encrypt(ticket);
            authCookie.Expires = formsAuthTicket.Expiration;
            HttpContext.Current.Response.Cookies.Add(authCookie);

            var ident = new GenericIdentity(c.UserName);
            var user = new GenericPrincipal(ident, roles);
            HttpContext.Current.User = user;
        }

        public string VirtualToAbsolute(string virtualPath)
        {
            return VirtualPathUtility.ToAbsolute(virtualPath);
        }

        public Uri VirtualToUri(string virtualPath)
        {
            var baseUri = new Uri(Url.GetLeftPart(UriPartial.Authority));
            var result = new Uri(baseUri, VirtualToAbsolute(virtualPath));
            return result;
        }

        public object GetSessionValue(string key)
        {
            return HttpContext.Current.Session[key];
        }

        public void SetSessionValue(string key, object value)
        {
            HttpContext.Current.Session[key] = value;
        }

        public void RemoveSessionValue(string key)
        {
            HttpContext.Current.Session.Remove(key);
        }
        
        private Client CreateClient(ClientItem c)
        {
            return new Client()
            {
                ClientID = c.ClientID,
                LName = c.LName,
                FName = c.FName,
                Email = c.Email,
                Phone = c.Phone
            };
        }

        public string MapPath(string path)
        {
            return HttpContext.Current.Server.MapPath(path);
        }
    }
}
