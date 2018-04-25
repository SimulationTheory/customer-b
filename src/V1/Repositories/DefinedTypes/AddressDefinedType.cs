using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using PSE.Cassandra.Core.Session.Interfaces;

namespace PSE.Customer.V1.Repositories.DefinedTypes
{
    /// <summary>
    /// Address Defined Type Cassandra Object model
    /// </summary>
    /// <seealso cref="PSE.Cassandra.Core.Session.Interfaces.IUserDefinedType" />
    [DataContract(Name = "address")]
    public class AddressDefinedType : IUserDefinedType
    {
        /// <summary>
        /// Gets or sets the address line1.
        /// </summary>
        /// <value>
        /// The address line1.
        /// </value>
        [DataMember(Name = "line_1")]
        [Required]
        public string AddressLine1 { get; set; }

        /// <summary>
        /// Gets or sets the address line2.
        /// </summary>
        /// <value>
        /// The address line2.
        /// </value>
        [DataMember(Name = "line_2")]
        public string AddressLine2 { get; set; }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        /// <value>
        /// The city.
        /// </value>
        [DataMember]
        [Required]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        [DataMember]
        [Required]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        /// <value>
        /// The country.
        /// </value>
        [DataMember]
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        /// <value>
        /// The postal code.
        /// </value>
        [DataMember(Name = "postal_code")]
        [Required]
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the care of.
        /// </summary>
        /// <value>
        /// The care of.
        /// </value>
        [DataMember(Name = "care_of")]
        public string CareOf { get; set; } = string.Empty;
    }
}