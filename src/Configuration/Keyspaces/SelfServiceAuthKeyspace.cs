using PSE.Cassandra.Core.Session;

namespace PSE.Customer.Configuration.Keyspaces
{
    public class SelfServiceAuthKeyspace : Keyspace
    {
        public override string Name => WebAPI.Core.Common.Keyspaces.SelfServiceAuth;
    }
}