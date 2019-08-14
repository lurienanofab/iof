using IOF.Models;
using System.Collections.Generic;

namespace IOF
{
    public interface IDetailRepository
    {
        Detail Single(int podid);
        IEnumerable<Detail> GetOrderDetails(int poid);
        IEnumerable<Detail> GetItemDetails(int itemId);
        Detail Add(int poid, int itemId, int catId, double qty, string unit, double unitPrice);
        void Update(int podid, int catId, double qty, string unit, double unitPrice);
        IEnumerable<string> PurchaserUpdate(int podid, string partNum, double quantity, double unitPrice, bool updateInventory);
        void Delete(int podid);
        Category GetCategory(Detail detail);
        IEnumerable<Category> GetParentCategories();
        IEnumerable<Category> GetChildCategories(int parentId);
        int AddCategory(int parentId, string categoryName, string categoryNumber);
        void ModifyCategory(int categoryId, string categoryName, string categoryNumber);
        void DeleteCategory(int categoryId);
        IEnumerable<Detail> GetInvalidCategoryItems(int poid);
    }
}
