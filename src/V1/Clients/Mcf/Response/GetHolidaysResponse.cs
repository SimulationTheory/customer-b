using Newtonsoft.Json;
using PSE.Customer.V1.Clients.Mcf.Models;
using PSE.RestUtility.Core.Interfaces;
using PSE.RestUtility.Core.Mcf;
using System.Collections.Generic;

namespace PSE.Customer.V1.Clients.Mcf.Response
{
    /// <summary>
    /// Represents base result from MCF call.
    /// </summary>
    public class GetHolidaysResponse : IMcfResult
    {
        /// <summary>
        /// Gets or sets the list of holidays.
        /// </summary>
        [JsonProperty("results")]
        public List<HolidaysResult> HolidaysResultList { get; set; }

        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>
        /// The metadata is ignored
        /// </value>
        public McfMetadata Metadata { get; set; }

        /// <summary>
        /// Gets a valid sample MCF holiday response.
        /// </summary>
        public static string GetSampleData()
            {
                return "{\r\n    \"d\": {\r\n        \"results\": [\r\n            {\r\n                \"__metadata\": {\r\n                    \"id\": \"http://ssapngtn2apv1.puget.com:8002/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/FactoryCalHolidaysSet(datetime'2018-03-01T00%3A00%3A00')\",\r\n                    \"uri\": \"http://ssapngtn2apv1.puget.com:8002/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/FactoryCalHolidaysSet(datetime'2018-03-01T00%3A00%3A00')\",\r\n                    \"type\": \"ERP_UTILITIES_UMC.FactoryCalHolidays\"\r\n                },\r\n                \"HolidayCalendar\": \"Z1\",\r\n                \"FactoryCalendar\": \"Z1\",\r\n                \"DateFrom\": \"/Date(1519862400000)/\",\r\n                \"DateTo\": \"/Date(1521417600000)/\",\r\n                \"HolidaysNav\": {\r\n                    \"results\": [\r\n                        {\r\n                            \"__metadata\": {\r\n                                \"id\": \"http://ssapngtn2apv1.puget.com:8002/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/HolidaysSet(datetime'2018-03-03T00%3A00%3A00')\",\r\n                                \"uri\": \"http://ssapngtn2apv1.puget.com:8002/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/HolidaysSet(datetime'2018-03-03T00%3A00%3A00')\",\r\n                                \"type\": \"ERP_UTILITIES_UMC.Holidays\"\r\n                            },\r\n                            \"Date\": \"/Date(1520035200000)/\"\r\n                        },\r\n                        {\r\n                            \"__metadata\": {\r\n                                \"id\": \"http://ssapngtn2apv1.puget.com:8002/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/HolidaysSet(datetime'2018-03-04T00%3A00%3A00')\",\r\n                                \"uri\": \"http://ssapngtn2apv1.puget.com:8002/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/HolidaysSet(datetime'2018-03-04T00%3A00%3A00')\",\r\n                                \"type\": \"ERP_UTILITIES_UMC.Holidays\"\r\n                            },\r\n                            \"Date\": \"/Date(1520121600000)/\"\r\n                        },\r\n                        {\r\n                            \"__metadata\": {\r\n                                \"id\": \"http://ssapngtn2apv1.puget.com:8002/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/HolidaysSet(datetime'2018-03-10T00%3A00%3A00')\",\r\n                                \"uri\": \"http://ssapngtn2apv1.puget.com:8002/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/HolidaysSet(datetime'2018-03-10T00%3A00%3A00')\",\r\n                                \"type\": \"ERP_UTILITIES_UMC.Holidays\"\r\n                            },\r\n                            \"Date\": \"/Date(1520640000000)/\"\r\n                        },\r\n                        {\r\n                            \"__metadata\": {\r\n                                \"id\": \"http://ssapngtn2apv1.puget.com:8002/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/HolidaysSet(datetime'2018-03-11T00%3A00%3A00')\",\r\n                                \"uri\": \"http://ssapngtn2apv1.puget.com:8002/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/HolidaysSet(datetime'2018-03-11T00%3A00%3A00')\",\r\n                                \"type\": \"ERP_UTILITIES_UMC.Holidays\"\r\n                            },\r\n                            \"Date\": \"/Date(1520726400000)/\"\r\n                        },\r\n                        {\r\n                            \"__metadata\": {\r\n                                \"id\": \"http://ssapngtn2apv1.puget.com:8002/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/HolidaysSet(datetime'2018-03-17T00%3A00%3A00')\",\r\n                                \"uri\": \"http://ssapngtn2apv1.puget.com:8002/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/HolidaysSet(datetime'2018-03-17T00%3A00%3A00')\",\r\n                                \"type\": \"ERP_UTILITIES_UMC.Holidays\"\r\n                            },\r\n                            \"Date\": \"/Date(1521244800000)/\"\r\n                        },\r\n                        {\r\n                            \"__metadata\": {\r\n                                \"id\": \"http://ssapngtn2apv1.puget.com:8002/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/HolidaysSet(datetime'2018-03-18T00%3A00%3A00')\",\r\n                                \"uri\": \"http://ssapngtn2apv1.puget.com:8002/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/HolidaysSet(datetime'2018-03-18T00%3A00%3A00')\",\r\n                                \"type\": \"ERP_UTILITIES_UMC.Holidays\"\r\n                            },\r\n                            \"Date\": \"/Date(1521331200000)/\"\r\n                        }\r\n                    ]\r\n                }\r\n            }\r\n        ]\r\n    }\r\n}";
            }
    }
}