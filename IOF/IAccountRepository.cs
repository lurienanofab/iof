using System.Collections.Generic;
using IOF.Models;

namespace IOF
{
    public interface IAccountRepository
    {
        Account Single(int clientId, int? accountId);

        /// <summary>
        /// Get accounts the user has added to the IOF system.
        /// </summary>
        IEnumerable<Account> GetActiveAccounts(int clientId);

        /// <summary>
        /// Get all accounts the user is currently assigned to and are available to be added to the IOF system.
        /// </summary>
        IEnumerable<Account> GetAvailableAccounts(int clientId);

        /// <summary>
        /// Get all accounts that any user has added to the IOF system.
        /// </summary>
        IEnumerable<Account> GetAllAccounts();

        string GetShortCode(int? accountId);

        Account AddOrUpdate(int clientId, int accountId);

        void Delete(int clientId, int accountId);
    }
}
