/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
namespace HIS.Desktop.Plugins.HemodialysisSchedule.Filter
{
    /// <summary>
    /// Filter tải gói vật tư (HIS_EXP_MEST_TEMPLATE) cho combo Gói vật tư:
    /// (CREATOR = userĐăngNhập OR IS_PUBLIC = 1) AND IS_KIDNEY = 1 AND IS_ACTIVE = 1.
    /// </summary>
    public class ExpMestTemplateFilter
    {
        public string CREATOR { get; set; }
        public short? IS_PUBLIC { get; set; }
        public short? IS_KIDNEY { get; set; }
        public short? IS_ACTIVE { get; set; }
    }
}
