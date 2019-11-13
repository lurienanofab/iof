using IOF.Models;
using LNF;
using LNF.Repository;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Ordering = LNF.Repository.Ordering;

namespace IOF.Impl
{
    public class VendorRepository : RepositoryBase, IVendorRepository
    {
        public Vendor Single(int vendorId)
        {
            var vendor = Require<Ordering.Vendor>(x => x.VendorID, vendorId);
            return CreateVendor(vendor);
        }

        public IEnumerable<Vendor> GetActiveVendors(int clientId)
        {
            var query = DA.Current.Query<Ordering.Vendor>().Where(x => x.ClientID == clientId && x.Active);
            return CreateVendors(query);
        }

        public IEnumerable<Vendor> GetAllVendors()
        {
            // returns both active and inactive
            var query = DA.Current.Query<Ordering.Vendor>();
            return CreateVendors(query);
        }

        public Vendor Copy(int toClientId, int fromVendorId)
        {
            var vend = Require<Ordering.Vendor>(x => x.VendorID, fromVendorId);

            var copy = new Ordering.Vendor()
            {
                ClientID = toClientId,
                VendorName = vend.VendorName,
                Address1 = vend.Address1,
                Address2 = vend.Address2,
                Address3 = vend.Address3,
                Contact = vend.Contact,
                Phone = vend.Phone,
                Fax = vend.Fax,
                URL = vend.URL,
                Email = vend.Email,
                Active = vend.Active
            };

            DA.Current.Insert(copy);

            return CreateVendor(copy);
        }

        public Vendor Add(int clientId, string vendorName, string address1, string address2, string address3, string contact, string phone, string fax, string url, string email)
        {
            var vend = new Ordering.Vendor()
            {
                ClientID = clientId,
                VendorName = vendorName,
                Address1 = address1,
                Address2 = address2,
                Address3 = address3,
                Contact = contact,
                Phone = phone,
                Fax = fax,
                URL = url,
                Email = email,
                Active = true
            };

            DA.Current.Insert(vend);

            if (clientId > 0 && AutoAddStoreManagerVendor())
            {
                // check for a store manager vendor with same name
                var storeManagerVendor = DA.Current.Query<Ordering.Vendor>().Where(x => x.VendorName == vendorName && x.ClientID == 0).FirstOrDefault();

                if (storeManagerVendor == null)
                {
                    // add a store manager vendor if the name was not found

                    vend = new Ordering.Vendor()
                    {
                        ClientID = 0,
                        VendorName = vendorName,
                        Address1 = address1,
                        Address2 = address2,
                        Address3 = address3,
                        Contact = contact,
                        Phone = phone,
                        Fax = fax,
                        URL = url,
                        Email = email,
                        Active = true
                    };

                    DA.Current.Insert(vend);
                }
            }

            return CreateVendor(vend);
        }

        public void Update(int vendorId, string vendorName, string address1, string address2, string address3, string contact, string phone, string fax, string url, string email)
        {
            var vend = Require<Ordering.Vendor>(x => x.VendorID, vendorId);

            vend.VendorName = vendorName;
            vend.Address1 = address1;
            vend.Address2 = address2;
            vend.Address3 = address3;
            vend.Contact = contact;
            vend.Phone = phone;
            vend.Fax = fax;
            vend.URL = url;
            vend.Email = email;
        }

        public void Delete(int vendorId)
        {
            var vend = Require<Ordering.Vendor>(x => x.VendorID, vendorId);
            vend.Active = false;
        }

        private bool AutoAddStoreManagerVendor()
        {
            var setting = ConfigurationManager.AppSettings["AutoAddStoreManagerVendor"];
            bool.TryParse(setting, out bool result);
            return result;
        }

        private Vendor CreateVendor(Ordering.Vendor vendor)
        {
            if (vendor == null) return null;

            return new Vendor()
            {
                VendorID = vendor.VendorID,
                ClientID = vendor.ClientID,
                VendorName = vendor.VendorName,
                Address1 = vendor.Address1,
                Address2 = vendor.Address2,
                Address3 = vendor.Address3,
                Contact = vendor.Contact,
                Phone = vendor.Phone,
                Fax = vendor.Fax,
                URL = vendor.URL,
                Email = vendor.Email,
                Active = vendor.Active
            };
        }

        private IEnumerable<Vendor> CreateVendors(IQueryable<Ordering.Vendor> query)
        {
            return query.Select(x => new Vendor()
            {
                VendorID = x.VendorID,
                ClientID = x.ClientID,
                VendorName = x.VendorName,
                Address1 = x.Address1,
                Address2 = x.Address2,
                Address3 = x.Address3,
                Contact = x.Contact,
                Phone = x.Phone,
                Fax = x.Fax,
                URL = x.URL,
                Email = x.Email,
                Active = x.Active
            }).ToList();
        }
    }
}
