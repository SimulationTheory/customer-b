using System;
using System.IdentityModel.Tokens.Jwt;

namespace PSE.Customer.V1.Clients.ClientProxy.Interfaces
{
    /// <summary>
    /// Represents the currently authenticated user for tests
    /// </summary>
    public interface IApiUser
    {
        /// <summary>
        /// Sets the JWT encoded string, usually as a response to logging in
        /// </summary>
        /// <param name="jwtEncodedString">The JWT encoded string.</param>
        void SetJwtEncodedString(string jwtEncodedString);

        /// <summary>
        /// Gets the claim value from the security token.
        /// </summary>
        /// <param name="type">The type of claim to get.</param>
        /// <returns>The value of the claim if found, or null if not found</returns>
        string GetClaimValue(string type);

        /// <summary>
        /// Gets the claim value as date time, assuming the value is an "epoch time",
        /// or the number of seconds (or milliseconds) since Jan/01/1970.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        DateTime? GetClaimValueAsDateTime(string type);

        /// <summary>
        /// Username
        /// </summary>
        string Username { get; }

        /// <summary>
        /// Gets the cognito user identifier.
        /// </summary>
        /// <value>
        /// A Guid that is a foreign key from the user_auth table to cognito
        /// </value>
        string CognitoUserId { get; }

        /// <summary>
        /// Contract account ID
        /// </summary>
        long ContractAccountId { get; }

        /// <summary>
        /// Business partner number
        /// </summary>
        long BPNumber { get; }

        /// <summary>
        /// Token that is set when the user logs in (in string form).
        /// Maps to SecurityToken.RawData
        /// </summary>
        string JwtEncodedString { get; }

        /// <summary>
        /// Security information that is set when the user logs in along with decoded information like
        /// claims, BP ID, username, etc
        /// </summary>
        JwtSecurityToken SecurityToken { get; }

        /// <summary>
        /// Gets the date and time when this authorization token was issued
        /// </summary>
        /// <value>
        /// The authorization timestamp.
        /// </value>
        DateTime? AuthTime { get; }

        /// <summary>
        /// Gets the date and time when this authorization token is no longer valid.
        /// </summary>
        /// <value>
        /// The expiration timestamp.
        /// </value>
        DateTime? ExpirationTime { get; }
    }
}
