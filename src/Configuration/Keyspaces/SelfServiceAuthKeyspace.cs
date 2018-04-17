using PSE.Cassandra.Core.Session;

namespace PSE.Customer.Configuration.Keyspaces
{
    public class SelfServiceAuthKeyspace : Keyspace
    {
        public override string Name => "selfservice_auth";
    }
}