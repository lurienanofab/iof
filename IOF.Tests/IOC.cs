using IOF.Impl;
using LNF;
using LNF.Data;
using LNF.DataAccess;
using LNF.Impl.Repository.Data;
using LNF.Repository;
using Moq;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;

namespace IOF.Tests
{
    public static class IOC
    {
        private static readonly IDictionary<string, object> _session;

        public static Container Container { get; }

        static IOC()
        {
            Container = new Container();

            _session = new Dictionary<string, object>();

            var mock = CreateContext("http://lnf-dev.eecs.umich.edu/iof", 1301);

            Container.Register(() => mock.Object);
            Container.Register<IAccountRepository, AccountRepository>();
            Container.Register<IClientRepository, ClientRepository>();
            Container.Register<IDetailRepository, DetailRepository>();
            Container.Register<IItemRepository, ItemRepository>();
            Container.Register<IOrderRepository, OrderRepository>();
            Container.Register<IVendorRepository, VendorRepository>();
            Container.Register<IAttachmentService, AttachmentService>();
            Container.Register<IEmailService, EmailService>();
            Container.Register<IPdfService, PdfService>();
            Container.Register<IExcelService, ExcelService>();
        }

        private static Mock<IContext> CreateContext(string url, int currentUserClientId)
        {
            var uri = new Uri(url);
            var mock = new Mock<IContext>();
            mock.Setup(x => x.Url).Returns(uri);

            mock.Setup(x => x.VirtualToAbsolute(It.IsAny<string>())).Returns<string>(x =>
            {
                return x.Replace("~", uri.AbsolutePath);
            });

            mock.Setup(x => x.VirtualToUri(It.IsAny<string>())).Returns<string>(x =>
            {
                var baseUri = new Uri(mock.Object.Url.GetLeftPart(UriPartial.Authority));
                var result = new Uri(baseUri, mock.Object.VirtualToAbsolute(x));
                return result;
            });

            IPrincipal user = null;

            IProvider provider = Container.GetInstance<IProvider>();
            ISession session = provider.DataAccess.Session;

            if (currentUserClientId > 0)
            {
                var client = session.Single<Client>(currentUserClientId);
                if (client != null)
                {
                    var ident = new GenericIdentity(client.UserName);
                    user = new GenericPrincipal(ident, client.Roles());
                }
            }

            mock.SetupProperty(x => x.User, user);

            mock.Setup(x => x.CurrentUser).Returns(() =>
            {
                var username = mock.Object.User.Identity.Name;
                var c = session.Query<ClientInfo>().First(x => x.UserName == username);

                return new Models.Client()
                {
                    ClientID = c.ClientID,
                    LName = c.LName,
                    FName = c.FName,
                    Email = c.Email,
                    Phone = c.Phone
                };
            });

            mock.Setup(x => x.GetSessionValue(It.IsAny<string>())).Returns<string>(x =>
            {
                return _session.ContainsKey(x) ? _session[x] : null;
            });

            mock.Setup(x => x.SetSessionValue(It.IsAny<string>(), It.IsAny<object>())).Callback<string, object>((k, v) =>
            {
                if (_session.ContainsKey(k))
                    _session[k] = v;
                else
                    _session.Add(k, v);
            });

            mock.Setup(x => x.RemoveSessionValue(It.IsAny<string>())).Callback<string>(k =>
            {
                if (_session.ContainsKey(k))
                    _session.Remove(k);
            });

            var baseDir = AppDomain.CurrentDomain.BaseDirectory;

            mock.Setup(x => x.MapPath(It.IsAny<string>())).Returns<string>(x =>
            {
                return Path.Combine(baseDir, x.Replace(".", string.Empty));
            });

            return mock;
        }
    }
}
