/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
namespace HIS.Desktop.Plugins.HemodialysisSchedule.ADO
{
    /// <summary>
    /// DTO gói vật tư (HIS_EXP_MEST_TEMPLATE) dùng cho LookUp inline edit cột "Gói vật tư".
    /// </summary>
    public class ExpMestTemplateADO
    {
        public long ID { get; set; }
        public string EXP_MEST_TEMPLATE_CODE { get; set; }
        public string EXP_MEST_TEMPLATE_NAME { get; set; }
        public short? IS_PUBLIC { get; set; }
        public short? IS_KIDNEY { get; set; }
        public short? IS_ACTIVE { get; set; }
        public string CREATOR { get; set; }
    }
}
