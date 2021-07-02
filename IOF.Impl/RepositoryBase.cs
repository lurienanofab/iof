using LNF;
using LNF.DataAccess;
using System;
using System.Linq.Expressions;
using Ordering = LNF.Impl.Repository.Ordering;

namespace IOF.Impl
{
    public abstract class RepositoryBase
    {
        protected IProvider Provider { get; }
        protected ISession DataSession => Provider.DataAccess.Session;

        protected RepositoryBase(IProvider provider)
        {
            Provider = provider;
        }

        internal T Require<T>(Expression<Func<T, int>> exp, int id) where T : LNF.DataAccess.IDataItem
        {
            var result = DataSession.Single<T>(id);

            if (result == null)
                throw new ItemNotFoundException<T>(exp, id);

            return result;
        }

        internal Ordering.Approver Require(Ordering.Approver id)
        {
            var result = DataSession.Single<Ordering.Approver>(id);

            if (result == null)
                throw new ItemNotFoundException("Approver", $"ClientID = {id.ClientID} and ApproverID = {id.ApproverID}");

            return result;
        }

        internal Ordering.PurchaseOrderAccount Require(Ordering.PurchaseOrderAccount id)
        {
            var result = DataSession.Single<Ordering.PurchaseOrderAccount>(id);

            if (result == null)
                throw new ItemNotFoundException("PurchaseOrderAccount", $"ClientID = {id.ClientID} and AccountID = {id.AccountID}");

            return result;
        }
    }
}
