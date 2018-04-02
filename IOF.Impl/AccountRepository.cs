using IOF.Models;
using LNF.Repository;
using System.Collections.Generic;
using System.Linq;
using Data = LNF.Repository.Data;
using Ordering = LNF.Repository.Ordering;

namespace IOF.Impl
{
    public class AccountRepository : RepositoryBase, IAccountRepository
    {
        public Account Single(int clientId, int? accountId)
        {
            if (!accountId.HasValue)
                return null;

            var acct = Require<Data.Account>(x => x.AccountID, accountId.Value);
            var poa = DA.Current.Single<Ordering.PurchaseOrderAccount>(new Ordering.PurchaseOrderAccount() { ClientID = clientId, AccountID = accountId.Value });
           
            int cid = 0;
            bool active = false;

            if (poa != null)
            {
                cid = poa.ClientID;
                active = poa.Active;
            }

            return new Account()
            {
                ClientID = cid,
                AccountID = acct.AccountID,
                AccountName = acct.Name,
                ShortCode = acct.ShortCode,
                Active = active
            };
        }

        public IEnumerable<Account> GetActiveAccounts(int clientId)
        {
            var query = DA.Current.Query<Ordering.PurchaseOrderAccount>().Where(x => x.ClientID == clientId && x.Active);

            var result = CreateAccounts(query).Where(x => x.Active); //need to check Active again to exclude inactive accounts (not just inactive in IOF)

            return result;
        }

        public IEnumerable<Account> GetAvailableAccounts(int clientId)
        {
            var current = GetActiveAccounts(clientId).Select(x => x.AccountID).ToArray();
            var query = DA.Current.Query<Data.ClientAccountInfo>().Where(x => x.ClientID == clientId && x.ClientAccountActive && !current.Contains(x.AccountID));
            var result = CreateAccounts(query);
            return result;
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            var query = DA.Current.Query<Ordering.PurchaseOrderAccount>();
            var result = CreateAccounts(query);
            return result;
        }

        public string GetShortCode(int? accountId)
        {
            if (accountId.HasValue)
            {
                var acct = Require<Data.Account>(x => x.AccountID, accountId.Value);
                return acct.ShortCode;
            }

            return string.Empty;
        }

        public Account AddOrUpdate(int clientId, int accountId)
        {
            var acct = new Ordering.PurchaseOrderAccount { AccountID = accountId, ClientID = clientId };

            var existing = DA.Current.Single<Ordering.PurchaseOrderAccount>(acct);

            if (existing == null)
            {
                //insert new
                acct.Active = true;
                DA.Current.Insert(acct);
                return CreateAccount(acct);
            }
            else
            {
                //update existing
                existing.Active = true;
                return CreateAccount(existing);
            }
        }

        public void Delete(int clientId, int accountId)
        {
            var acct = Require(new Ordering.PurchaseOrderAccount { AccountID = accountId, ClientID = clientId });

            if (acct != null)
                acct.Active = false;
        }

        private Account CreateAccount(Ordering.PurchaseOrderAccount poa)
        {
            var acct = Require<Data.Account>(x => x.AccountID, poa.AccountID);

            return new Account()
            {
                ClientID = poa.ClientID,
                AccountID = acct.AccountID,
                AccountName = acct.Name,
                ShortCode = acct.ShortCode,
                Active = poa.Active
            };
        }

        private IEnumerable<Account> CreateAccounts(IQueryable<Ordering.PurchaseOrderAccount> query)
        {
            var join = query.Join(DA.Current.Query<Data.Account>(),
                o => o.AccountID,
                i => i.AccountID,
                (o, i) => new { PurchaseOrderAccount = o, Account = i });

            var result = join.Select(x => new Account()
            {
                ClientID = x.PurchaseOrderAccount.ClientID,
                AccountID = x.PurchaseOrderAccount.AccountID,
                AccountName = x.Account.Name,
                ShortCode = x.Account.ShortCode,
                Active = x.PurchaseOrderAccount.Active && x.Account.Active
            }).ToList();

            return result;
        }

        private IEnumerable<Account> CreateAccounts(IQueryable<Data.ClientAccountInfo> query)
        {
            return query.Select(x => new Account()
            {
                ClientID = x.ClientID,
                AccountID = x.AccountID,
                AccountName = x.AccountName,
                ShortCode = x.ShortCode,
                Active = true
            }).ToList();
        }
    }
}
