using LNF;
using LNF.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap.Attributes;

namespace IOF.Tests
{
    public abstract class TestBase
    {
        private IUnitOfWork _uow;

        [SetterProperty]
        public IContext Context { get; set; }

        [SetterProperty]
        public IPdfService PdfService { get; set; }

        [SetterProperty]
        public IEmailService EmailService { get; set; }

        [SetterProperty]
        public IItemRepository ItemRepository { get; set; }

        [SetterProperty]
        public IClientRepository ClientRepository { get; set; }

        [TestInitialize]
        public void TestSetup()
        {
            _uow = ServiceProvider.Current.DataAccess.StartUnitOfWork();
            IOC.Container.BuildUp(this);
        }

        [TestCleanup]
        public void TestComplete()
        {
            if (_uow != null)
                _uow.Dispose();
        }
    }
}
