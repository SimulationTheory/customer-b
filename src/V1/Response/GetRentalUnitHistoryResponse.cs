using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSE.Account.V1.Response
{
    public class GetListofAccountActivity
    {
        public string OccupiedStatus { get; set; }

        public string ContractAccountOpenDate { get; set; }

        public string ContractAccountCloseDate { get; set; }

        public List<GetRentalUnitHistoryResponse> activities { get; set; }

    }

    public class GetRentalUnitHistoryResponse
    {

        public string DateOfActivity { get; set; }

        public string Activity { get; set; }

        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; }

        public string DocumentID { get; set; }


      
        public string InsertURL { get; set; }

        public List<Premise> PremisesMoveInDetails { get; set; }





    }

    public class Premise
    {
        public string PremiseID { get; set; }

        public List<Installations> ServiceInstallations { get; set; }
    }

    public class Installations
    {

        public string InstallationID { get; set; }

        public string Division { get; set; }
        
        public List<MoveInDetails> MoveInDetail { get; set; }

    }
    public class MoveInDetails
    {
        public string MoveIn { get; set; }

        public string MoveOut { get; set; }

        public bool TenantOccupied { get; set; } 
    }
}
