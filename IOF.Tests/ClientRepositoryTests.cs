using LNF.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Ordering = LNF.Repository.Ordering;

namespace IOF.Tests
{
    [TestClass]
    public class ClientRepositoryTests : TestBase
    {
        [TestMethod]
        public void TestSingle()
        {
            var c = ClientRepository.Single(1301);
            Assert.IsNotNull(c);
            Assert.AreEqual("Getty, James", c.DisplayName);
        }
        
        [TestMethod]
        public void TestGetActiveClients()
        {
            var query = ClientRepository.GetActiveClients();
            Assert.IsTrue(query.Count() > 0);
        }

        [TestMethod]
        public void TestGetClientsWithVendor()
        {
            var query = ClientRepository.GetClientsWithVendor();
            Assert.IsTrue(query.Count() > 0);
        }

        [TestMethod]
        public void TestPurchaserMethods()
        {
            var query = ClientRepository.GetPurchasers();
            Assert.IsTrue(query.Count() > 0);

            var purch = ClientRepository.AddOrUpdatePurchaser(1301, true);
            Assert.AreEqual("Getty, James", purch.DisplayName);

            Assert.IsTrue(ClientRepository.IsPurchaser(purch.ClientID));

            var p = ClientRepository.GetPurchaser(1301);
            Assert.AreEqual("Getty, James", p.DisplayName);

            ClientRepository.DeletePurchaser(p.PurchaserID);

            Assert.IsFalse(ClientRepository.IsPurchaser(p.ClientID));

            //clean up
            var entity = DA.Current.Single<Ordering.Purchaser>(p.PurchaserID);
            DA.Current.Delete(entity);
        }
    }
}
