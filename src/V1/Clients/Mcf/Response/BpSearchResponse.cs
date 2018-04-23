using Newtonsoft.Json;
using PSE.Customer.V1.Models;
using PSE.RestUtility.Core.Mcf;
using System;

namespace PSE.Customer.V1.Clients.Mcf.Response
{
    /// <summary>
    ///     Response for BP search to check if a customer already exists.
    /// </summary>
    /// <seealso cref="PSE.RestUtility.Core.Interfaces.IMcfResult" />
    public class BpSearchResponse
    {
        /// <summary>
        ///     Gets or sets the BP Id if there is a match found.
        /// </summary>
        [JsonProperty("BusinessPartner")]
        public string BpId { get; set; }

        [JsonProperty("BPsearchIDinfoSet")]
        public McfList<IdentifierModel> BpSearchIdInfoSet { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the threshold for a matching user has been met or exceeded.
        /// </summary>
        /// <value>This will returned as an 'X' from MCF if the threshhold for determining a positive match has been met.</value>
        [JsonProperty("Threshold")]
        public string Threshhold { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether there is a unique Business Partner.
        /// </summary>
        /// <value>This will returned as a '1' from MCF if it's unique.</value>
        [JsonProperty("Unique")]
        public string Unique { get; set; }

        /// <summary>
        ///     Gets or sets the reason.
        /// </summary>
        [JsonProperty("Reason")]
        public string Reason { get; set; }

        /// <summary>
        ///     Gets or sets the code associated with the reason.
        /// </summary>
        [JsonProperty("ReasonCode")]
        public string ReasonCode { get; set; }

        #region UnusedPropertiesFromMcf
        ///// <summary>
        /////     Gets or sets the match percentage.
        ///// </summary>
        //public double ActualPercentage { get; set; }

        ///// <summary>
        /////     Gets or sets a value indicating whether the email is a match.
        ///// </summary>
        //public bool EmailMatch { get; set; }

        ///// <summary>
        /////     Gets or sets a value indicating whether the name is a match.
        ///// </summary>
        //public bool NameMatch { get; set; }

        ///// <summary>
        /////     Gets or sets a value indicating whether the Organization Name is a match.
        ///// </summary>
        //public bool OrganizationNameMatch { get; set; }

        ///// <summary>
        /////     Gets or sets a value indicating whether the phone number is a match.
        ///// </summary>
        //public bool PhoneNumberMatch { get; set; }

        ///// <summary>
        /////     Gets or sets a value indicating whether the Tax ID is a match.
        ///// </summary>
        //public bool TaxIdMatch { get; set; }

        ///// <summary>
        /////     Gets or sets a value indicating whether the UBI is a match.
        ///// </summary>
        //public bool UbiMatch { get; set; }
        #endregion
    }
}