using IOF.Impl;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using Moq;
using StructureMap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;

namespace IOF.Tests
{
    public static class IOC
    {
        private static readonly IDictionary<string, object> _session;

        public static Container Container { get; }

        static IOC()
        {
            _session = new Dictionary<string, object>();

            Container = new Container(x =>
            {
                var mock = CreateContext("http://lnf-dev.eecs.umich.edu/iof", 1301);

                x.For<IContext>().Use(mock.Object);

                x.For<IAccountRepository>().Use<AccountRepository>();
                x.For<IClientRepository>().Use<ClientRepository>();
                x.For<IDetailRepository>().Use<DetailRepository>();
                x.For<IItemRepository>().Use<ItemRepository>();
                x.For<IOrderRepository>().Use<OrderRepository>();
                x.For<IVendorRepository>().Use<VendorRepository>();

                x.For<IAttachmentService>().Use<AttachmentService>();
                x.For<IEmailService>().Use<EmailService>();
                x.For<IPdfService>().Use<PdfService>();
                x.For<IExcelService>().Use<ExcelService>();
            });
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

            if (currentUserClientId > 0)
            {
                var client = DA.Current.Single<Client>(currentUserClientId);
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
                var c = ClientInfo.Find(username);

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
