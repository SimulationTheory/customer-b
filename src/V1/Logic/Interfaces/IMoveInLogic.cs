using PSE.Customer.V1.Models;

namespace PSE.Customer.V1.Logic.Interfaces
{
    public interface IMoveInLogic
    {
        /// <summary>
        /// Get move in late payment costs associated with provided contract account id 
        /// </summary>
        /// <param name="contractAccountId"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
        ReconnectStatusResponse GetMoveInLatePayment(long contractAccountId, string jwt);


        //ReconnectionResponse GetReconnectAmountDueAndCa(string contractAccountId);
    }
}
