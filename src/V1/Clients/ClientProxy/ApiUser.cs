using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using PSE.Customer.V1.Clients.ClientProxy.Interfaces;
using PSE.Customer.V1.Clients.Extensions;

namespace PSE.Customer.V1.Clients.ClientProxy
{
    /// <inheritdoc />
    public class ApiUser : IApiUser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiUser"/> class.
        /// Boring constructor method used with object initializer for test objects
        /// </summary>
        public ApiUser()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiUser"/> class with an authorization token.
        /// </summary>
        /// <param name="jwtEncodedString">The JWT encoded string for authorization.</param>
        public ApiUser(string jwtEncodedString)
        {
            SetJwtEncodedString(jwtEncodedString);
        }

        /// <inheritdoc />
        public void SetJwtEncodedString(string jwtEncodedString)
        {
            JwtEncodedString = jwtEncodedString;
            SecurityToken = new JwtSecurityToken(jwtEncodedString);
            CognitoUserId = GetClaimValue("cognito:username");
            if (long.TryParse(GetClaimValue("custom:bp"), out var bpId))
            {
                BPNumber = bpId;
            }
        }

        /// <inheritdoc />
        public string GetClaimValue(string type)
        {
            return SecurityToken.Claims.FirstOrDefault(c => c.Type == type)?.Value;
        }

        /// <inheritdoc />
        public DateTime? GetClaimValueAsDateTime(string type)
        {
            return GetClaimValue(type).FromUnixTimeSeconds();
        }

        /// <inheritdoc />
        public string Username { get; set; }

        /// <inheritdoc />
        public string CognitoUserId { get; set; }

        /// <inheritdoc />
        public long ContractAccountId { get; set; }

        /// <inheritdoc />
        public long BPNumber { get; set; }

        /// <inheritdoc />
        public string JwtEncodedString { get; set; }

        /// <inheritdoc />
        public JwtSecurityToken SecurityToken { get; set; }

        /// <inheritdoc />
        public DateTime? AuthTime => GetClaimValueAsDateTime("auth_time");

        /// <inheritdoc />
        public DateTime? ExpirationTime => GetClaimValueAsDateTime("exp");
    }
}
