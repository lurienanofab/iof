using IOF.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace IOF.Tests
{
    [TestClass]
    public class CopyDataTests : TestBase
    {
        [TestMethod]
        public void CanCopyData()
        {
            var page = new CopyData();

            var jim = 1301;
            var brian = 1229;
            var zygo = 1182;

            var result = page.CopyData(jim, brian, zygo, true);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(1, result.Items.Count());
            Assert.AreEqual("Zygo", result.Vendor.VendorName);
            Assert.IsTrue(ItemRepository.GetVendorItems(result.Vendor.VendorID).Count() == 1);
        }
    }
}
