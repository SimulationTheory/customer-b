using Newtonsoft.Json;
using PSE.RestUtility.Core.Json;
using PSE.WebAPI.Core.Interfaces;
using System;

namespace PSE.Customer.V1.Clients.Mcf.Request
{
    public class CreateCustomerInteractionRequest : IAPIRequest
    {
        private string _interactionRecordReasonID = String.Empty;
        private string _channelID = String.Empty;

        /// <summary>
        /// Gets or sets the business partner id
        /// </summary>
        /// <value>
        ///   The business partner identifier.
        /// </value>
        public string AccountID { get; set; }

        /// <summary>
        /// Gets or sets the interaction  description
        /// </summary>
        /// <value>
        ///   The note of contents of customer interaction
        /// </value>
        public string Description { get; set; }
        
        /// <summary>
        /// Gets or sets the interaction note
        /// </summary>
        /// <value>
        ///   The note about contents of customer interaction
        /// </value>
        public string Note { get; set; } = String.Empty;
        
        /// <summary>
        /// Gets or sets the premise id
        /// </summary>
        /// <value>
        ///   The premise id
        /// </value>
        [JsonConverter(typeof(ToStringJsonConverter))]
        public long PremiseID { get; set; }

        /// <summary>
        /// Gets or sets the business agreement id
        /// </summary>
        /// <value>
        ///   The account contract id
        /// </value>
        [JsonConverter(typeof(ToStringJsonConverter))]
        public long BusinessAgreementID { get; set; }

        /// <summary>
        /// Gets or sets the interaction priority
        /// </summary>
        /// <value>
        ///   See priority enum for values
        /// </value>
        [JsonConverter(typeof(ToStringJsonConverter))]
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the interaction record id returned by SAP
        /// </summary>
        /// <value>
        ///   Returned by SAP if record is created successfully
        /// </value>
        public string InteractionRecordReasonID
        {
            get { return _interactionRecordReasonID ?? String.Empty; }
            set { _interactionRecordReasonID = value; }
        }
        
        /// <summary>
        /// Gets or sets the interaction record category id returned by SAP
        /// </summary>
        /// <value>
        ///   Returned by SAP if record is created successfully
        /// </value>
        public string InteractionRecordCategory1 { get; set; } = "";
        
        /// <summary>
        /// Gets or sets the interaction record category id returned by SAP
        /// </summary>
        /// <value>
        ///   Returned by SAP if record is created successfully
        /// </value>
        public string InteractionRecordCategory2 { get; set; } = "";
        /// <summary>
        /// Gets or sets the interaction record category guid returned by SAP
        /// </summary>
        /// <value>
        ///   Returned by SAP if record is created successfully
        /// </value>
        public string InteractionRecordCategory1GUID { get; set; } = "";
        
        /// <summary>
        /// Gets or sets the interaction record category guid returned by SAP
        /// </summary>
        /// <value>
        ///   Returned by SAP if record is created successfully
        /// </value>
        public string InteractionRecordCategory2GUID { get; set; } = "";
        /// <summary>
        /// Gets or sets the channel id
        /// </summary>
        /// <value>
        ///   string containing channel id
        /// </value>
        public string ChannelID
        {
            get { return _channelID ?? String.Empty; }
            set { _channelID = value; }
        }

        /// <summary>
        /// Gets or sets the incoming flag
        /// </summary>
        /// <value>
        ///   set to true
        /// </value>
        public bool IncomingFlag { get; set; }
        
        /// <summary>
        /// Gets or sets the document status id
        /// </summary>
        /// <value>
        ///   string containing document status id
        /// </value>
        public string DocumentStatusID { get; set; }
         
    }
}
      