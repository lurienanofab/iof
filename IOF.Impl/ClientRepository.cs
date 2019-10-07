using IOF.Models;
using LNF.Models.Data;
using LNF.Repository;
using System.Collections.Generic;
using System.Linq;
using Data = LNF.Repository.Data;
using Ordering = LNF.Repository.Ordering;

namespace IOF.Impl
{
    public class ClientRepository : RepositoryBase, IClientRepository
    {
        public static readonly ClientPrivilege StoreManagerPrivilege = ClientPrivilege.Staff | ClientPrivilege.StoreManager | ClientPrivilege.Developer;
        public static readonly ClientPrivilege AdministratorPrivilege = ClientPrivilege.Administrator | ClientPrivilege.Developer;
        public static readonly ClientPrivilege StaffPrivilege = ClientPrivilege.Staff | ClientPrivilege.Developer;
        public static readonly ClientPrivilege PurchaserPrivilege = ClientPrivilege.FinancialAdmin | ClientPrivilege.Developer;

        public Client Single(int clientId)
        {
            var c = Require<Data.ClientInfo>(x => x.ClientID, clientId);
            return CreateClient(c);
        }

        public IEnumerable<Client> GetActiveClients()
        {
            var query = DA.Current.Query<Data.ClientInfo>().Where(x => x.ClientActive);
            return CreateClients(query);
        }

        public IEnumerable<Client> GetAllClients(int priv = 0)
        {
            IQueryable<Data.ClientInfo> query;

            ClientPrivilege p = (ClientPrivilege)priv;

            if (priv == 0)
                query = DA.Current.Query<Data.ClientInfo>();
            else
                query = DA.Current.Query<Data.ClientInfo>().Where(x => (x.Privs & p) > 0);

            return CreateClients(query);
        }

        public IEnumerable<Client> GetClientsWithVendor()
        {
            var current = DA.Current.Query<Ordering.Vendor>().Where(x => x.Active && x.ClientID > 0).Select(x => x.ClientID).Distinct().ToArray();
            var clients = DA.Current.Query<Data.ClientInfo>().Where(x => x.ClientActive && current.Contains(x.ClientID));
            return CreateClients(clients);
        }

        public Purchaser GetPurchaser(Order order)
        {
            if (order.PurchaserID.HasValue)
            {
                // Confusingly PurchaseOrder.PurchaserID is the purchaser's ClientID, not the primary key in the Purchaser table, which is also called PurchaserID.
                var purch = DA.Current.Query<Ordering.Purchaser>().FirstOrDefault(x => x.Client.ClientID == order.PurchaserID.Value);
                var client = Require<Data.ClientInfo>(x => x.ClientID, purch.Client.ClientID);
                return CreatePurchaser(purch, client);
            }

            return null;
        }

        public Purchaser GetPurchaser(Client client)
        {
            var purch = DA.Current.Query<Ordering.Purchaser>().FirstOrDefault(x => x.Client.ClientID == client.ClientID);
            var c = Require<Data.ClientInfo>(x => x.ClientID, client.ClientID);
            return CreatePurchaser(purch, c);
        }

        public Purchaser GetPurchaser(int purchaserId)
        {
            var purch = Require<Ordering.Purchaser>(x => x.PurchaserID, purchaserId);
            var c = Require<Data.ClientInfo>(x => x.ClientID, purch.Client.ClientID);
            return CreatePurchaser(purch, c);
        }

        public IEnumerable<Purchaser> GetPurchasers(bool? active = true)
        {
            var query = DA.Current.Query<Ordering.Purchaser>().Where(x => !x.Deleted && x.Active == active.GetValueOrDefault(x.Active));
            return CreatePurchasers(query);
        }

        public IEnumerable<Purchaser> GetAvailablePurchasers()
        {
            var current = DA.Current.Query<Ordering.Purchaser>().Where(x => !x.Deleted).Select(x => x.Client.ClientID).ToArray();
            var query = DA.Current.Query<Data.ClientInfo>().Where(x => x.ClientActive && !current.Contains(x.ClientID) && (x.Privs & PurchaserPrivilege) > 0);
            return CreatePurchasers(query);
        }

        public Purchaser AddOrUpdatePurchaser(int clientId, bool active)
        {
            var client = Require<Data.ClientInfo>(x => x.ClientID, clientId);

            var purch = DA.Current.Query<Ordering.Purchaser>().FirstOrDefault(x => x.Client.ClientID == clientId);

            if (purch == null)
            {
                purch = new Ordering.Purchaser()
                {
                    Client = Require<Data.Client>(x => x.ClientID, clientId),
                    Active = active,
                    Deleted = false
                };

                DA.Current.Insert(purch);
            }
            else
            {
                purch.Active = active;
                purch.Deleted = false;
            }

            return CreatePurchaser(purch, client);
        }

        public void DeletePurchaser(int purchaserId)
        {
            var purch = Require<Ordering.Purchaser>(x => x.PurchaserID, purchaserId);
            purch.Active = false;
            purch.Deleted = true;
        }

        public bool IsPurchaser(int clientId)
        {
            return DA.Current.Query<Ordering.Purchaser>().Any(x => x.Client.ClientID == clientId && x.Active && !x.Deleted);
        }

        public Approver GetApprover(Order order)
        {
            var app = Require(new Ordering.Approver() { ClientID = order.ClientID, ApproverID = order.ApproverID });
            return CreateApprover(app);
        }

        public IEnumerable<Approver> GetActiveApprovers(int clientId)
        {
            var query = DA.Current.Query<Ordering.Approver>().Where(x => x.ClientID == clientId && x.Active);
            return CreateApprovers(query);
        }

        public IEnumerable<Approver> GetAvailableApprovers(int clientId)
        {
            var current = GetActiveApprovers(clientId).Select(x => x.ApproverID).ToArray();
            var query = DA.Current.Query<Data.ClientInfo>().Where(x => !current.Contains(x.ClientID) && (x.Privs & AdministratorPrivilege) > 0 && x.ClientActive);
            return CreateApprovers(query);
        }

        public IEnumerable<Approver> GetAllAvailableApprovers()
        {
            var query = DA.Current.Query<Data.ClientInfo>().Where(x => (x.Privs & AdministratorPrivilege) > 0 && x.ClientActive);
            return CreateApprovers(query);
        }

        public IEnumerable<Approver> GetAllApprovers()
        {
            var query = DA.Current.Query<Ordering.Approver>();
            return CreateApprovers(query);
        }

        public bool IsApprover(int clientId)
        {
            int count = DA.Current.Query<Ordering.Approver>().Count(x => x.ApproverID == clientId && x.Active);
            return count > 0;
        }

        public bool IsStaff(int clientId)
        {
            var c = Require<Data.Client>(x => x.ClientID, clientId);
            return c.HasPriv(StaffPrivilege);
        }

        public bool IsStoreManager(int clientId)
        {
            var c = Require<Data.Client>(x => x.ClientID, clientId);
            return c.HasPriv(StoreManagerPrivilege);
        }

        public bool IsAdministrator(int clientId)
        {
            var c = Require<Data.Client>(x => x.ClientID, clientId);
            return c.HasPriv(AdministratorPrivilege);
        }

        public Approver AddOrUpdateApprover(int clientId, int approverId, bool isPrimary)
        {
            var appr = new Ordering.Approver { ClientID = clientId, ApproverID = approverId, IsPrimary = false };
            var existing = DA.Current.Single<Ordering.Approver>(appr);

            if (existing == null)
                return AddApprover(appr, isPrimary);
            else
                return UpdateApprover(existing, isPrimary);
        }

        public void DeleteApprover(int clientId, int approverId)
        {
            var appr = Require(new Ordering.Approver { ClientID = clientId, ApproverID = approverId });
            appr.Active = false;
            SetPrimary(appr, false);
        }

        private Client CreateClient(Data.ClientInfo client)
        {
            if (client == null) return null;

            return new Client()
            {
                ClientID = client.ClientID,
                UserName = client.UserName,
                LName = client.LName,
                FName = client.FName,
                Email = client.Email,
                Phone = client.Phone
            };
        }

        private IEnumerable<Client> CreateClients(IQueryable<Data.ClientInfo> query)
        {
            return query.Select(x => new Client()
            {
                ClientID = x.ClientID,
                UserName = x.UserName,
                LName = x.LName,
                FName = x.FName,
                Email = x.Email,
                Phone = x.Phone
            });
        }

        private Purchaser CreatePurchaser(Ordering.Purchaser purchaser, Data.ClientInfo client)
        {
            if (purchaser == null || client == null) return null;

            return new Purchaser()
            {
                PurchaserID = purchaser.PurchaserID,
                ClientID = client.ClientID,
                UserName = client.UserName,
                LName = client.LName,
                FName = client.FName,
                Email = client.Email,
                Phone = client.Phone,
                Active = purchaser.Active,
                Deleted = purchaser.Deleted
            };
        }

        private IEnumerable<Purchaser> CreatePurchasers(IQueryable<Ordering.Purchaser> query)
        {
            var join = query.Join(DA.Current.Query<Data.ClientInfo>(),
                o => o.Client.ClientID,
                i => i.ClientID,
                (o, i) => new { Purchaser = o, ClientInfo = i });

            var result = join.Select(x => new Purchaser()
            {
                PurchaserID = x.Purchaser.PurchaserID,
                ClientID = x.ClientInfo.ClientID,
                UserName = x.ClientInfo.UserName,
                LName = x.ClientInfo.LName,
                FName = x.ClientInfo.FName,
                Email = x.ClientInfo.Email,
                Phone = x.ClientInfo.Phone,
                Active = x.Purchaser.Active,
                Deleted = x.Purchaser.Deleted
            }).ToList();

            return result;
        }

        private IEnumerable<Purchaser> CreatePurchasers(IQueryable<Data.ClientInfo> query)
        {
            var result = query.Select(x => new Purchaser()
            {
                PurchaserID = 0,
                ClientID = x.ClientID,
                UserName = x.UserName,
                LName = x.LName,
                FName = x.FName,
                Email = x.Email,
                Phone = x.Phone,
                Active = true,
                Deleted = false
            }).ToList();

            return result;
        }

        private Approver AddApprover(Ordering.Approver appr, bool isPrimary)
        {
            if (appr == null) return null;
            appr.Active = true;
            DA.Current.Insert(appr);
            SetPrimary(appr, isPrimary);
            return CreateApprover(appr);
        }

        private Approver UpdateApprover(Ordering.Approver appr, bool isPrimary)
        {
            if (appr == null) return null;
            appr.Active = true;
            SetPrimary(appr, isPrimary);
            return CreateApprover(appr);
        }

        private void SetPrimary(Ordering.Approver appr, bool isPrimary)
        {
            if (appr.IsPrimary != isPrimary)
            {
                if (isPrimary)
                {
                    // get the current primary approver, if any
                    var currentPrimary = DA.Current.Query<Ordering.Approver>().Where(x => x.ClientID == appr.ClientID && x.ApproverID != appr.ApproverID && x.IsPrimary);

                    // set any found to false
                    foreach (var cp in currentPrimary)
                        cp.IsPrimary = false;

                    // set this approver to primary
                    appr.IsPrimary = true;
                }
                else
                {
                    appr.IsPrimary = false;
                }
            }
        }

        private Approver CreateApprover(Ordering.Approver app)
        {
            var c = Require<Data.ClientInfo>(x => x.ClientID, app.ApproverID);

            return new Approver()
            {
                ClientID = app.ClientID,
                UserName = c.UserName,
                ApproverID = app.ApproverID,
                FName = c.FName,
                LName = c.LName,
                Email = c.Email,
                Phone = c.Phone,
                IsPrimary = app.IsPrimary,
                Active = app.Active
            };
        }

        private IEnumerable<Approver> CreateApprovers(IQueryable<Ordering.Approver> query)
        {
            var join = query.Join(DA.Current.Query<Data.ClientInfo>(),
                    o => o.ApproverID,
                    i => i.ClientID,
                    (o, i) => new { Approver = o, ClientInfo = i });

            var result = join.Select(x => new Approver()
            {
                ClientID = x.Approver.ClientID,
                UserName = x.ClientInfo.UserName,
                ApproverID = x.Approver.ApproverID,
                LName = x.ClientInfo.LName,
                FName = x.ClientInfo.FName,
                Email = x.ClientInfo.Email,
                Phone = x.ClientInfo.Phone,
                IsPrimary = x.Approver.IsPrimary,
                Active = x.Approver.Active
            }).ToList();

            return result;
        }

        private IEnumerable<Approver> CreateApprovers(IQueryable<Data.ClientInfo> query)
        {
            return query.Select(x => new Approver()
            {
                ClientID = x.ClientID,
                UserName = x.UserName,
                ApproverID = x.ClientID,
                LName = x.LName,
                FName = x.FName,
                Email = x.Email,
                Phone = x.Phone,
                IsPrimary = false,
                Active = true
            }).ToList();
        }
    }
}
