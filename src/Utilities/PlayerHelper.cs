namespace ProtectHighTierPrefabs.Utilities
{
    internal class PlayerHelper
    {
        public static bool TryGetClientInfo(int entityId, out ClientInfo clientInfo)
        {
            clientInfo = ConnectionManager.Instance.Clients.ForEntityId(entityId);
            return clientInfo != null;
        }
    }
}
