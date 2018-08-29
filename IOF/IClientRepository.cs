using IOF.Models;
using System.Collections.Generic;

namespace IOF
{
    public interface IClientRepository
    {
        /// <summary>
        /// Get one client.
        /// </summary>
        Client Single(int clientId);

        /// <summary>
        /// Get all active clients.
        /// </summary>
        IEnumerable<Client> GetActiveClients();

        /// <summary>
        /// Gets all clients. If p is greater than zero only clients with the matching privilege are selected.
        /// </summary>
        IEnumerable<Client> GetAllClients(int priv = 0);

        /// <summary>
        /// Get all clients that have at least one active vendor in the IOF system.
        /// </summary>
        IEnumerable<Client> GetClientsWithVendor();

        /// <summary>
        /// Get the purchaser for an order.
        /// </summary>
        Purchaser GetPurchaser(Order order);

        /// <summary>
        /// Get a purchaser based on a client.
        /// </summary>
        Purchaser GetPurchaser(Client client);


        /// <summary>
        /// Get a purchaser based on a purchaser id.
        /// </summary>
        Purchaser GetPurchaser(int purchaserId);

        /// <summary>
        /// Get purchasers in the IOF system.
        /// </summary>
        IEnumerable<Purchaser> GetPurchasers(bool? active = true);

        /// <summary>
        /// Get possible purchasers not already added to the IOF system.
        /// </summary>
        IEnumerable<Purchaser> GetAvailablePurchasers();

        /// <summary>
        /// Check if the client is a purchaser.
        /// </summary>
        bool IsPurchaser(int clientId);

        /// <summary>
        /// Add a new purchaser or update one if it already exists.
        /// </summary>
        Purchaser AddOrUpdatePurchaser(int clientId, bool active);

        /// <summary>
        /// Delete a purchaser (set Active = false).
        /// </summary>
        void DeletePurchaser(int purchaserId);

        /// <summary>
        /// Get the approver for an order.
        /// </summary>
        Approver GetApprover(Order order);

        /// <summary>
        /// Get approvers added to a particular user.
        /// </summary>
        IEnumerable<Approver> GetActiveApprovers(int clientId);

        /// <summary>
        /// Get all approvers previously added by a user.
        /// </summary>
        IEnumerable<Approver> GetAllApprovers();

        /// <summary>
        /// Get possible approvers not already added by a user.
        /// </summary>
        IEnumerable<Approver> GetAvailableApprovers(int clientId);

        /// <summary>
        /// Get all clients who can possibly be an approver.
        /// </summary>
        IEnumerable<Approver> GetAllAvailableApprovers();

        /// <summary>
        /// Check if the client is an approver.
        /// </summary>
        bool IsApprover(int clientId);

        bool IsStaff(int clientId);

        bool IsStoreManager(int clientId);

        bool IsAdministrator(int clientId);

        /// <summary>
        /// Add an approver or update a previously added approver.
        /// </summary>
        Approver AddOrUpdateApprover(int clientId, int approverId, bool isPrimary);

        /// <summary>
        /// Remove an approver from a user.
        /// </summary>
        void DeleteApprover(int clientId, int approverId);
    }
}
