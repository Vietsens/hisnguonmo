namespace HIS.Desktop.Plugins.SupplyAnticipateCreate.Base
{
    /// <summary>
    /// ACS_CONTROL codes of the supply process (same codes are checked by MOS: HisAnticipateSupplyCFG)
    /// </summary>
    class ControlCode
    {
        /// <summary>
        /// Approve / return supply anticipate (accountant)
        /// </summary>
        internal const string APPROVE = "HIS000062";

        /// <summary>
        /// Pharmacy confirms chemical anticipate
        /// </summary>
        internal const string PHARMACY_CONFIRM = "HIS000063";
    }
}
