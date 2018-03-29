using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace IOF.Tests
{
    [TestClass]
    public class ItemRepositoryTests : TestBase
    {
        [TestMethod]
        public void ItemRepository_CanGetClientItems()
        {
            var items = ItemRepository.GetClientItems(1301);
            Assert.IsTrue(items.Count() > 0);
        }

        [TestMethod]
        public void ItemRepository_CanGetOrderItems()
        {
            var items = ItemRepository.GetOrderItems(123);
            Assert.IsTrue(items.Count() > 0);
        }
    }
}
