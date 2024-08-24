using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisAlertByConfig.HisAlertByConfig
{
    public partial class frmHisAlertByConfig : HIS.Desktop.Utility.FormBase
    {

        Inventec.Desktop.Common.Modules.Module moduleData;
        public frmHisAlertByConfig(Inventec.Desktop.Common.Modules.Module moduleData) : base(moduleData)
        {
            InitializeComponent();

            try
            {
                this.moduleData = moduleData;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        V_HIS_ROOM Room;
        private void frmHisAlertByConfig_Load(object sender, EventArgs e)
        {
            try
            {
                Room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == moduleData.RoomId);
                if (Room != null)
                {
                    lblContent.Text = Room.ROOM_NAME + " - " + Room.DEPARTMENT_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnMedical_Click(object sender, EventArgs e)
        {
            CallApi(true);
        }

        private void btnSecurity_Click(object sender, EventArgs e)
        {
            CallApi(false);
        }
        private void CallApi(bool IsMedical)
        {

            try
            {
                if (Room == null)
                    return;
                HisAlertDepartmentFilter filter = new HisAlertDepartmentFilter();
                filter.DEPARTMENT_ID = Room.DEPARTMENT_ID;
                var AlertDepartment = new BackendAdapter(new CommonParam()).Get<List<HIS_ALERT_DEPARTMENT>>("/api/HisAlertDepartment/Get", ApiConsumers.MosConsumer, filter, null);
                if(AlertDepartment != null && AlertDepartment.Count > 0)
                {
                    if (IsMedical)
                        AlertDepartment = AlertDepartment.Where(o=>o.IS_MEDICAL == 1).ToList();
                    else
                        AlertDepartment = AlertDepartment.Where(o => o.IS_SECURITY == 1).ToList();

                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => AlertDepartment), AlertDepartment));
                if (AlertDepartment == null || AlertDepartment.Count == 0)
                {
                    XtraMessageBox.Show("Không thể tạo báo động khi chưa thiết lập khoa nhận thông báo");
                    return;
                }
                HisAlertSDO sdo = new HisAlertSDO();
                sdo.RequestRoomId = Room.ID;
                sdo.Title = IsMedical ? "Báo động đỏ y khoa" : "Báo động đỏ an ninh";
                sdo.Content = lblContent.Text;
                sdo.ReceiveDepartmentIds = AlertDepartment.Select(o => o.RECEIVE_DEPARTMENT_ID).ToList();
                sdo.AlertType = IsMedical ? 2 : 3;
                var Alert = new BackendAdapter(new CommonParam()).Post<HIS_ALERT>("/api/HisAlert/Create", ApiConsumers.MosConsumer, sdo, null);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Alert), Alert));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
    }
}
