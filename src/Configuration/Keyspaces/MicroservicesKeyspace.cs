using PSE.Cassandra.Core.Session;

namespace PSE.Customer.Configuration.Keyspaces
{
    public class MicroservicesKeyspace : Keyspace
    {
        /// <summary>
        /// 
        /// </summary>
        public override string Name => WebAPI.Core.Common.Keyspaces.Microservices;
    }
}