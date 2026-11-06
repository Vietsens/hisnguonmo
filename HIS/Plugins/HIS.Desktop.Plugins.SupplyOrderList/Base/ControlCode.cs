namespace HIS.Desktop.Plugins.SupplyOrderList.Base
{
    /// <summary>
    /// ACS_CONTROL codes of the supply process (same codes are checked by MOS: HisAnticipateSupplyCFG)
    /// </summary>
    class ControlCode
    {
        /// <summary>
        /// Confirm / cancel supply order (pharmacy)
        /// </summary>
        internal const string ORDER = "HIS000064";

        /// <summary>
        /// Send supply order email to the supplier
        /// </summary>
        internal const string SEND_MAIL = "HIS000065";
    }
}
