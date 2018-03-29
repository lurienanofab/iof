using IOF.Models;
using System.Collections.Generic;

namespace IOF
{
    public interface IVendorRepository
    {
        Vendor Single(int vendorId);
        IEnumerable<Vendor> GetActiveVendors(int clientId);
        IEnumerable<Vendor> GetAllVendors();
        Vendor Add(int clientId, string vendorName, string address1, string address2, string address3, string contact, string phone, string fax, string url, string email);
        Vendor Copy(int toClientId, int fromVendorId);
        void Update(int vendorId, string vendorName, string address1, string address2, string address3, string contact, string phone, string fax, string url, string email);
        void Delete(int vendorId);
    }
}
