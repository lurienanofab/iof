using IOF.Models;
using LNF;
using LNF.Data;
using LNF.Web;
using System;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace IOF.Impl
{
    public class Context : IContext
    {
        public IProvider Provider => ServiceProvider.Current;

        public HttpContextBase ContextBase => new HttpContextWrapper(HttpContext.Current);

        public Client CurrentUser => CreateClient(ContextBase.CurrentUser(Provider));

        public Uri Url => ContextBase.Request.Url;

        public IPrincipal User
        {
            get { return ContextBase.User; }
            set { ContextBase.User = value; }
        }

        public void SignIn(Client client)
        {
            var repo = new ClientRepository(Provider);
            var c = repo.Require<LNF.Impl.Repository.Data.ClientInfo>(x => x.ClientID, client.ClientID);
            var roles = c.Roles();

            var authCookie = FormsAuthentication.GetAuthCookie(c.UserName, true);
            var formsAuthTicket = FormsAuthentication.Decrypt(authCookie.Value);
            var ticket = new FormsAuthenticationTicket(formsAuthTicket.Version, formsAuthTicket.Name, formsAuthTicket.IssueDate, formsAuthTicket.Expiration, formsAuthTicket.IsPersistent, string.Join("|", roles), formsAuthTicket.CookiePath);
            authCookie.Value = FormsAuthentication.Encrypt(ticket);
            authCookie.Expires = formsAuthTicket.Expiration;
            ContextBase.Response.Cookies.Add(authCookie);

            var ident = new GenericIdentity(c.UserName);
            var user = new GenericPrincipal(ident, roles);
            ContextBase.User = user;
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
            return ContextBase.Session[key];
        }

        public void SetSessionValue(string key, object value)
        {
            ContextBase.Session[key] = value;
        }

        public void RemoveSessionValue(string key)
        {
            ContextBase.Session.Remove(key);
        }
        
        private Client CreateClient(IClient c)
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
            return ContextBase.Server.MapPath(path);
        }
    }
}
