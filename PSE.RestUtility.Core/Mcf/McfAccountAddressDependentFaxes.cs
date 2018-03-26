using PSE.RestUtility.Core.Interfaces;

namespace PSE.RestUtility.Core.Mcf
{
    public class McfAccountAddressDependentFaxes : IMcfDeferred
    {
        /// <summary>
        /// Gets or sets the deferred.
        /// </summary>
        /// <value>
        /// The deferred.
        /// </value>
        public McfDeferred Deferred { get; set; }
    }
}
