namespace PSE.Customer.Tests.Integration.TestObjects
{
    public static class TestData
    {
        public static class PaymentArrangementData
        {
            /// <summary>
            /// Faked payment arrangement data.  Body is actual, but the PaymentArrangementNav array is empty.
            /// </summary>
            public static string ActivePaUserFakeGetData = @"
{
    ""d"": {
        ""results"": [
            {
                ""__metadata"": {
                    ""id"": ""https://10.41.53.54:8001/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/PaymentArrangementSet(ContractAccountID='200019410436',InstallmentPlanNumber='')"",
                    ""uri"": ""https://10.41.53.54:8001/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/PaymentArrangementSet(ContractAccountID='200019410436',InstallmentPlanNumber='')"",
                    ""type"": ""ERP_UTILITIES_UMC.PaymentArrangement""
                },
                ""Channel"": """",
                ""ContractAccountID"": ""200019410436"",
                ""InstallmentPlanNumber"": """",
                ""InstallmentPlanType"": """",
                ""TestRun"": """",
                ""NoOfPaymentsPermitted"": ""000"",
                ""PaStatus"": ""D"",
                ""ReasonCode"": """",
                ""Reason"": """",
                ""PaymentArrangementNav"": {
                    ""results"": [
                        {
                            ""__metadata"": {
                                ""id"": ""http://ssapngdn2apv1.puget.com:8002/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/InstallmentPlansSet(InstallmentPlanNumber='400000002557',InstallmentPlanType='05')"",
                                ""uri"": ""http://ssapngdn2apv1.puget.com:8002/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/InstallmentPlansSet(InstallmentPlanNumber='400000002557',InstallmentPlanType='05')"",
                                ""type"": ""ERP_UTILITIES_UMC.InstallmentPlans""
                            },
                            ""InstallmentPlanNumber"": ""400000002557"",
                            ""InstallmentPlanType"": ""05"",
                            ""NoOfInstallments"": ""002"",
                            ""InstallmentPlansNav"": {
                                ""results"": [
                                    {
                                        ""__metadata"": {
                                            ""id"": ""http://ssapngdn2apv1.puget.com:8002/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/IpIdSet(AmountDue=500.000m,AmountOpen=500.000m)"",
                                            ""uri"": ""http://ssapngdn2apv1.puget.com:8002/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/IpIdSet(AmountDue=500.000m,AmountOpen=500.000m)"",
                                            ""type"": ""ERP_UTILITIES_UMC.InstallmentDetails""
                                        },
                                        ""AmountDue"": ""500.000"",
                                        ""AmountOpen"": ""0.000"",
                                        ""DueDate"": ""/Date(1518652800000)/""
                                    },
                                    {
                                        ""__metadata"": {
                                            ""id"": ""http://ssapngdn2apv1.puget.com:8002/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/IpIdSet(AmountDue=228.510m,AmountOpen=228.510m)"",
                                            ""uri"": ""http://ssapngdn2apv1.puget.com:8002/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/IpIdSet(AmountDue=228.510m,AmountOpen=228.510m)"",
                                            ""type"": ""ERP_UTILITIES_UMC.InstallmentDetails""
                                        },
                                        ""AmountDue"": ""228.510"",
                                        ""AmountOpen"": ""228.510"",
                                        ""DueDate"": ""/Date(1519862400000)/""
                                    }
                                ]
                            }
                        }
                    ]
                },
                ""PayArrangeCreateNav"": { ""__deferred"": { ""uri"": ""https://10.41.53.54:8001/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/PaymentArrangementSet(ContractAccountID='200019410436',InstallmentPlanNumber='')/PayArrangeCreateNav"" } }
            }
        ]
    }
}";
        }
    }
}
