using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using PSE.WebAPI.Core.Exceptions;
using PSE.Exceptions.Core.Interfaces;

namespace PSE.Customer.Extensions
{
    /// <summary>
    /// This class is used to simplify method level exception handling in the controller
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Converts an exception to an action result to allow a JSON response rather than a status code.
        /// </summary>
        /// <param name="source">The source exception.</param>
        /// <returns>A ContentResult that indicates </returns>
        public static IActionResult ToActionResult(this Exception source)
        {
            if (source is IPSEException pseSourceEx)
            {
                return source.ToActionResult((HttpStatusCode)pseSourceEx.StatusCode());
            }

            if (source is UnauthorizedAccessException)
            {
                return source.ToActionResult(HttpStatusCode.Unauthorized);
            }

            // Default error is 500
            return source.ToActionResult(HttpStatusCode.InternalServerError);
        }
    }
}
