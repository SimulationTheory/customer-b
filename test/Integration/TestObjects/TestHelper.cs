using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PSE.Cassandra.Core.Extensions;
using PSE.Customer.Configuration;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Repositories.DefinedTypes;
using PSE.WebAPI.Core.Configuration;
using PSE.WebAPI.Core.Configuration.Interfaces;

namespace PSE.Customer.Tests.Integration.TestObjects
{
    public static class TestHelper
    {
        /// <summary>
        /// Test Java Web Token string for PaDev1
        /// </summary>
        public static string PaDev1JwtToken =
            "eyJraWQiOiJSRGlmUFwvbEQ0Q0VVNXJadEJQeWdFSDI5S1haNThoOUtwXC9hRWxzK2t3bUE9IiwiYWxnIjoiUlMyNTYifQ.eyJzdWIiOiI2YWM4MjA4My1jZDJhLTRlY2UtYTk2MC1iNzY3NGJiOTQwNTAiLCJhdWQiOiI2dGpzc2ZyaGh0cGw1dnRsYWN1bTdjdHVjbyIsImVtYWlsX3ZlcmlmaWVkIjpmYWxzZSwidG9rZW5fdXNlIjoiaWQiLCJhdXRoX3RpbWUiOjE1MjI0NDYyODEsImlzcyI6Imh0dHBzOlwvXC9jb2duaXRvLWlkcC51cy13ZXN0LTIuYW1hem9uYXdzLmNvbVwvdXMtd2VzdC0yX1pmZzBmVVNwUyIsImN1c3RvbTpicCI6IjEwMDI2NDcwNzAiLCJjb2duaXRvOnVzZXJuYW1lIjoiMTFmZDA0MzktNWZmZi00YzEzLWFmNjgtZTJiY2FiNTY3ZmY4IiwiZXhwIjoxNTIyNDQ5ODgxLCJpYXQiOjE1MjI0NDYyODEsImVtYWlsIjoidGVzdHVzZXJwYURldjFAdGVzdC5jb20ifQ.NSW-YjqiP3vCik1U2yVh3_VD5sBGqnP5zoNE-_EUHl8_p4vpMRK2kHo56_96f8Y0KO90F1we8IY1FFXWVNkwADybmQoIf3EpGDgCbyFPEq6H_jf1o3zHB8vJOa0ex-d4hZyCne4Ee68NZZQNgPU6CW6OrsijExkL-l0MQz73VZjDJdb-r7gaswKzLvzW4xhipiC3ylzWvKrnVEIJ-bjIeO4ideXCErvJyPM4MYKJhYp6iNi5-RMtEyXVTbY7s-ZP4BoKXrneb4Vmj6XfRRnXewJwJq09ULIQyTj7dFrOM84a0L5qp9ta-mT8yJQFkESr3OahaaA8sXsbU9OxpKS7-Q";

        /// <summary>
        /// This account is set up to use MCF and has a mailing address and phone number
        /// </summary>
        /// <remarks>
        /// The JwtToken is a access token in a valid format but has expired
        /// </remarks>
        public static readonly TestUser PaDev1 = new TestUser
        {
            BPNumber = 1002647070,
            Username = "testuserpadev1",
            JwtEncodedString = PaDev1JwtToken,
            Email = "test7@test.com",
            Address = new AddressDefinedType
            {
                AddressLine1 = "1717 S 18TH ST",
                City = "MOUNT VERNON",
                State = "WA",
                PostalCode = "98274-6031"
            },
            Phones = new List<Phone>
            {
                new Phone
                {
                    Type = PhoneType.Cell,
                    Number = "4254573067"
                },
                new Phone
                {
                    Type = PhoneType.Work,
                    Number = "4258824091"
                },
                new Phone
                {
                    Type = PhoneType.Home,
                    Number = "4258824004"
                }
            }
        };

        /// <summary>
        /// Account that is supposed to have an active installment plan
        /// </summary>
        public static readonly TestUser ActivePaUser = new TestUser
        {
            Username = "testpaactive02",
            Email = "testpaactive02@test.com",
            ContractAccountId = 200019410436
        };

        /// <summary>
        /// Account that is supposed to have an mailing addresses 
        /// </summary>
        public static readonly TestUser ActiveMaUser = new TestUser
        {
            Username = "testuser6",
            Email = "testpaactive02@test.com",
            ContractAccountId = 220007739539
        };
        /// <summary>
        /// Account for creating a user interaction record 
        /// </summary>
        public static readonly TestUser ActiveInteractionUser = new TestUser
        {
            Username = "donaldmcconnell",
            BPNumber = 1001840105
                   
        };
        /// <summary>
        /// Gets the core options.
        /// </summary>
        /// <returns></returns>
        public static ICoreOptions GetCoreOptions()
        {
            var options = new CoreOptions(ServiceConfiguration.AppName);

            return options;
        }

      

        /// <summary>
        /// Gets a new service collection with logging enabled and ApplicationLifetime registered.
        /// </summary>
        /// <returns></returns>
        public static IServiceCollection GetServiceCollection()
        {
            var coreOptions = GetCoreOptions();
            var services = new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Trace);
                    builder.AddConsole();
                })
                .AddSingleton<IApplicationLifetime, ApplicationLifetime>()
                .AddSingleton(coreOptions)
                .AddSingleton(Options.Create(coreOptions.Configuration.CassandraSettings))
                .Configure<AppSettings>(a => a.MaxLookBackMonths = 24);

            ServiceCollectionExtensions.ClearCassandraSettings();

            return services;
        }
    }
}
