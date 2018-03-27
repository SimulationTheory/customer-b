using PSE.RestUtility.Core.Interfaces;

namespace PSE.RestUtility.Core.Mcf
{
    public class McfAccountAddressDependentMobilePhones: IMcfDeferred
    {
        /// !!! This class is not an MCF clas and should be in the V1/Clients/Mcf/Request & Response folders
        /// <summary>
        /// Gets or sets the deferred.
        /// </summary>
        /// <value>
        /// The deferred.
        /// </value>
        public McfDeferred Deferred { get; set; }
    }
}
