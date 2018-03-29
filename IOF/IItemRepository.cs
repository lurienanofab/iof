using IOF.Models;
using System.Collections.Generic;

namespace IOF
{
    public interface IItemRepository
    {
        Item Single(int itemId);
        IEnumerable<Item> GetOrderItems(int poid, bool? active = true);
        IEnumerable<Item> GetVendorItems(int vendorId, bool? active = true);
        IEnumerable<Item> GetClientItems(int clientId, bool? active = true);
        IEnumerable<Item> Copy(int toVendorId, int fromVendorId);
        InventoryItem GetInventoryItem(Item item);
        IEnumerable<InventoryItem> GetInventoryItems();
        Item Add(string partNum, string description, double unitPrice, int vendorId, int? inventoryItemId);
        void Update(int itemId, string partNum, string description, double unitPrice, int? inventoryItemId);
        void Delete(int itemId);
        void Restore(int itemId);
    }
}
