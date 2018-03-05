using PSE.Cassandra.Core.Session;

namespace PSE.Customer.Configuration.Keyspaces
{
    public class MicroservicesKeyspace : Keyspace
    {
        public override string Name => "microservices";
    }
}