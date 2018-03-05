using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using PSE.Customer.Configuration;
using PSE.Customer.V1.Repositories.Entities;
using PSE.Customer.V1.Repositories.Interfaces;
using PSE.Customer.V1.Repositories.Views;
using PSE.Customer.V1.Repositories;

namespace PSE.Customer.Tests.Unit.TestObjects
{
    public static class TestHelper
    {
        public static long ContractAccountId = 1239218391839128;
        public static int MaxLookBackMonths = 24;

        public static readonly TestUser TestUser1 = new TestUser
        {
            ContractAccountId = 220006073096,
            BPNumber = 1002736444,
            LastPasswordChanged = "12/20/2017 9:26:59 PM",
            Username = "RESIMPSON"
        };

        public static readonly TestUser TestUser2 = new TestUser
        {
            ContractAccountId = 200027879325,
            BPNumber = 1001694236,
            LastPasswordChanged = "12/20/2017 9:30:44 PM",
            Username = "KEVAMHEARN"
        };

        public static readonly TestUser TestUser3 = new TestUser
        {
            ContractAccountId = 200010619514,
            BPNumber = 1001361917,
            LastPasswordChanged = "12/20/2017 9:34:13 PM",
            Username = "DIAMONDSQUARELLC"
        };

        public static readonly TestUser TestUser4 = new TestUser
        {
            ContractAccountId = 200014665042,
            BPNumber = 1002505700,
            LastPasswordChanged = "1/6/2018 2:50:25 AM",
            Username = "GlennSJohnson"
        };

        /// <summary>
        /// Gets the application settings stored in appSettings.json.
        /// </summary>
        /// <returns></returns>
        public static AppSettings GetAppSettings()
        {
            var builder = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
              .AddEnvironmentVariables();
            var configuration = builder.Build();
            var appSettings = new AppSettings();
            configuration.GetSection("AppSettings").Bind(appSettings);

            return appSettings;
        }

        /// <summary>
        /// Gets the mock contract account repository.
        /// </summary>
        /// <param name="exceptionPairs">The exception pairs.</param>
        /// <returns></returns>
        public static IContractAccountRepository GetContractAccountRepository(params string[] exceptionPairs)
        {
            var exceptions = GetTestExceptions(exceptionPairs);
            var repository = new Mock<IContractAccountRepository>();
            repository.Setup(r => r.GetBusinessPartnerIdByContractAccount(It.IsAny<long>()))
                .Returns((long contractAccountId) =>
                {
                    Console.WriteLine($"IContractAccountRepository.GetBusinessPartnerIdByContractAccount(contractAccountId) was called with:\r\n\tcontractAccountId: {contractAccountId}");

                    if (exceptions.ContainsKey("GetBusinessPartnerIdByContractAccount"))
                    {
                        throw exceptions["GetBusinessPartnerIdByContractAccount"];
                    }

                    var accountView = new ContractAccountByBusinessPartnerView
                    {
                        BusinessPartnerId = TestUser1.BPNumber,
                        ContractAccountId = TestUser1.ContractAccountId
                    };

                    return Task.FromResult(accountView);
                });
            repository.Setup(r => r.GetContractAccount(It.IsAny<long>(), It.IsAny<long>()))
                .Returns((long businessPartnerId, long contractAccountId) =>
                {
                    Console.WriteLine($"IContractAccountRepository.GetContractAccount(businessPartnerId, contractAccountId) was called with:\r\n\businessPartnerId: {businessPartnerId}\r\n\tcontractAccountId: {contractAccountId}");

                    if (exceptions.ContainsKey("GetContractAccount"))
                    {
                        throw exceptions["GetContractAccount"];
                    }

                    var account = new ContractAccountEntity
                    {
                        Active = true,
                        AFTIndicator = true,
                        BusinessPartnerId = TestUser1.BPNumber,
                        ContractAccountId = TestUser1.ContractAccountId,
                        BalanceForward = 0.0M,
                        BankruptcyIndicator = false
                    };

                    return Task.FromResult(account);
                });

            return repository.Object;
        }

        /// <summary>
        /// Gets the console logger.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ILogger<T> GetLogger<T>()
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole(LogLevel.Trace, true);

            return loggerFactory.CreateLogger<T>();
        }

        public static IDictionary<string, Exception> GetTestExceptions(params string[] exceptions)
        {
            return exceptions.ToDictionary(kvp =>
            {
                return kvp.Split(":", StringSplitOptions.RemoveEmptyEntries)[0];
            },
            kvp =>
            {
                var message = kvp.Split(":", StringSplitOptions.RemoveEmptyEntries)[1];

                return new Exception(message);
            });
        }

        public static IDictionary<string, Exception> GetTestExceptions(params KeyValuePair<string, Exception>[] exceptions)
        {
            return exceptions.ToDictionary();
        }

        /// <summary>
        /// Converts the json to the specified type.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static T To<T>(this string target)
            where T : class
        {
            if (target == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<T>(target);
        }

        /// <summary>
        /// Converts a KeyValuePair collection to a dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">values</exception>
        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            return values.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        /// <summary>
        /// Converts the given object to json.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static string ToJson(this object target)
        {
            if (target == null)
            {
                return null;
            }

            return JsonConvert.SerializeObject(target);
        }
    }
}
