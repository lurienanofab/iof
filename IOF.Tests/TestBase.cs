using LNF;
using LNF.DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IOF.Tests
{
    public abstract class TestBase
    {
        private IUnitOfWork _uow;

        [Inject] public IProvider Provider { get; set; }

        [Inject] public IContext Context { get; set; }

        [Inject] public IPdfService PdfService { get; set; }

        [Inject] public IEmailService EmailService { get; set; }

        [Inject] public IItemRepository ItemRepository { get; set; }

        [Inject] public IClientRepository ClientRepository { get; set; }

        public ISession DataSession => Provider.DataAccess.Session;

        [TestInitialize]
        public void TestSetup()
        {
            _uow = ServiceProvider.Current.DataAccess.StartUnitOfWork();
        }

        [TestCleanup]
        public void TestComplete()
        {
            if (_uow != null)
                _uow.Dispose();
        }
    }
}
