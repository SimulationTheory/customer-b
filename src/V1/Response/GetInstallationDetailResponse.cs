namespace PSE.Customer.V1.Response
{
    public class GetInstallationDetailResponse
    {
        public long InstallationId { get; set; }
        public bool MoveInEligibility { get; set; }
        public string DivisionId { get; set; }
        public long PremiseId { get; set; }
        public string InstallationGuid { get; set; }
        public long BusinessPartnerId { get; set; }
    }
}
