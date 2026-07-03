/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
namespace HIS.Desktop.Plugins.HemodialysisSchedule.ADO
{
    /// <summary>Ca chạy thận (1..5) cho combo Ca.</summary>
    public class ShiftADO
    {
        public short ID { get; set; }
        public string NAME { get; set; }

        public ShiftADO() { }

        public ShiftADO(short id, string name)
        {
            this.ID = id;
            this.NAME = name;
        }
    }
}
