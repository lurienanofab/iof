using LNF;
using LNF.Repository;
using System;
using System.Linq.Expressions;
using Ordering = LNF.Repository.Ordering;

namespace IOF.Impl
{
    public abstract class RepositoryBase
    {
        internal T Require<T>(Expression<Func<T, int>> exp, int id) where T : IDataItem
        {
            var result = DA.Current.Single<T>(id);

            if (result == null)
                throw new ItemNotFoundException<T>(exp, id);

            return result;
        }

        internal Ordering.Approver Require(Ordering.Approver id)
        {
            var result = DA.Current.Single<Ordering.Approver>(id);

            if (result == null)
                throw new ItemNotFoundException("Approver", $"ClientID = {id.ClientID} and ApproverID = {id.ApproverID}");

            return result;
        }

        internal Ordering.PurchaseOrderAccount Require(Ordering.PurchaseOrderAccount id)
        {
            var result = DA.Current.Single<Ordering.PurchaseOrderAccount>(id);

            if (result == null)
                throw new ItemNotFoundException("PurchaseOrderAccount", $"ClientID = {id.ClientID} and AccountID = {id.AccountID}");

            return result;
        }
    }
}
