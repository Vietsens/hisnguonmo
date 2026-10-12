using HIS.Desktop.Library.CacheClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.CallPatientCashierV3
{
    /// <summary>Đọc / ghi cấu hình màn chờ thu ngân 3 vào ControlState (lưu theo máy + module, như màn chờ phòng khám 6).</summary>
    internal class ConfigStateStore
    {
        internal const string MODULE_LINK = "HIS.Desktop.Plugins.CallPatientCashierV3";
        internal const string KEY_CONFIG = "WaitingScreenCashierV3Config";

        private ControlStateWorker worker;
        private List<ControlStateRDO> current;

        internal WaitingScreenConfigADO Load(string departmentTitle, long? defaultDepartmentId)
        {
            WaitingScreenConfigADO cfg = null;
            try
            {
                this.worker = new ControlStateWorker();
                this.current = this.worker.GetData(MODULE_LINK);
                if (this.current != null && this.current.Count > 0)
                {
                    ControlStateRDO rdo = this.current.FirstOrDefault(o => o.KEY == KEY_CONFIG);
                    if (rdo != null && !string.IsNullOrWhiteSpace(rdo.VALUE))
                    {
                        cfg = JsonConvert.DeserializeObject<WaitingScreenConfigADO>(rdo.VALUE);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                cfg = null;
            }
            if (cfg == null)
            {
                cfg = WaitingScreenConfigADO.CreateDefault(departmentTitle, defaultDepartmentId);
            }
            cfg.Normalize();
            return cfg;
        }

        internal bool Save(WaitingScreenConfigADO cfg)
        {
            try
            {
                if (this.worker == null) this.worker = new ControlStateWorker();
                if (this.current == null) this.current = new List<ControlStateRDO>();
                string json = JsonConvert.SerializeObject(cfg);
                ControlStateRDO rdo = this.current.FirstOrDefault(o => o.KEY == KEY_CONFIG && o.MODULE_LINK == MODULE_LINK);
                if (rdo != null)
                {
                    rdo.VALUE = json;
                }
                else
                {
                    rdo = new ControlStateRDO();
                    rdo.KEY = KEY_CONFIG;
                    rdo.VALUE = json;
                    rdo.MODULE_LINK = MODULE_LINK;
                    this.current.Add(rdo);
                }
                this.worker.SetData(this.current);
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }
    }
}
