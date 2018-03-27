using Newtonsoft.Json;

namespace PSE.RestUtility.Core.Mcf
{
    /// !!! This class is not an MCF clas and should be in the V1/Clients/Mcf/Request & Response folders
    public class McfAddressinfo
    {
        public McfMetadata1 Metadata { get; set; }
        [JsonProperty("StandardFlag")]
        public string StandardFlag { get; set; }
        [JsonProperty("City")]
        public string City { get; set; }
        [JsonProperty("District")]
        public string District { get; set; }
        [JsonProperty("PostalCode")]
        public string PostalCode { get; set; }
        [JsonProperty("POBoxPostalCode")]
        public string POBoxPostalCode { get; set; }
        [JsonProperty("POBox")]
        public string POBox { get; set; }
        [JsonProperty("Street")]
        public string Street { get; set; }
        [JsonProperty("HouseNo")]
        public string HouseNo { get; set; }
        [JsonProperty("Building")]
        public string Building { get; set; }
        [JsonProperty("Floor")]
        public string Floor { get; set; }
        [JsonProperty("RoomNo")]
        public string RoomNo { get; set; }
        [JsonProperty("CountryID")]
        public string CountryID { get; set; }
        [JsonProperty("CountryName")]
        public string CountryName { get; set; }
        [JsonProperty("Region")]
        public string Region { get; set; }
        [JsonProperty("RegionName")]
        public string RegionName { get; set; }
        [JsonProperty("TimeZone")]
        public string TimeZone { get; set; }
        [JsonProperty("TaxJurisdictionCode")]
        public string TaxJurisdictionCode { get; set; }
        [JsonProperty("LanguageID")]
        public string LanguageID { get; set; }
        [JsonProperty("ShortForm")]
        public string ShortForm { get; set; }
    }
}
