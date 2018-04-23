using System;

namespace PSE.Authentication.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public static class ParameterStoreNames
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly string Environment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        /// <summary>
        /// 
        /// </summary>
        public static readonly string Prefix = $"/CI/MS/{Environment}/Authentication";

        public static class Cognito
        {
            /// <summary>
            /// 
            /// </summary>
            public static readonly string UserPoolId = $"/CI/MS/{Environment}/Cognito/UserPool";

            /// <summary>
            /// 
            /// </summary>
            public static readonly string ClientId = Prefix + "/Cognito/ClientId";

            /// <summary>
            /// 
            /// </summary>
            public static readonly string RegionEndpoint = $"/CI/MS/{Environment}/RegionEndpoint";
        }

        // TODO: move names here
        public static class SecurityQuestions
        {
        }

        // TODO: move names here
        public static class LockUser
        {
        }
    }
}