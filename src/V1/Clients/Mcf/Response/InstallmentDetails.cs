namespace PSE.Customer.V1.Clients.Mcf.Response
{
    /// <summary>
    /// Due date and amount due for each specific installment payment
    /// </summary>
    public class InstallmentDetails
    {
        /// <summary>
        /// Gets or sets the amount due.
        /// </summary>
        /// <value>
        /// The amount due.
        /// </value>
        public decimal AmountDue { get; set; }

        /// <summary>
        /// Gets or sets the amount open.
        /// </summary>
        /// <value>
        /// The amount open.
        /// </value>
        public decimal AmountOpen { get; set; }

        /// <summary>
        /// Date amount is due in epoch time, e.g. "/Date(1518652800000)/"
        /// </summary>
        public string DueDate { get; set; }
    }
}
