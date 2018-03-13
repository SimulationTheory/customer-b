using System;
using System.Diagnostics.CodeAnalysis;

namespace PSE.MCFClient.Core.Models
{
    [Obsolete]
    [ExcludeFromCodeCoverage]
    public class QueryParameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}