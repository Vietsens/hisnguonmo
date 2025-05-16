using HIS.Desktop.ApiConsumer;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExamSpecialist.ExamSpecialist
{
    public partial class frmReject : Form
    {
        private HIS_SPECIALIST_EXAM speciaListExam;
        private HIS.Desktop.Common.DelegateRefreshData dlgRefresh { get; set; }
        public frmReject(HIS_SPECIALIST_EXAM speciaListExam, HIS.Desktop.Common.DelegateRefreshData dlgRefresh)
        {
            this.speciaListExam = speciaListExam;
            this.dlgRefresh = dlgRefresh;
            string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
            this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            InitializeComponent();
        }
        private void frmReject_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            txtReason.Text = speciaListExam.REJECT_APPROVAL_REASON; 
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(txtReason.Text.Trim()))
                {
                    dxErrorProvider1.SetError(txtReason, "Trường dữ liệu bắt buộc", DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning);
                    return;
                }
                if(Inventec.Common.String.CountVi.Count(txtReason.Text) > 4000)
                {
                    dxErrorProvider1.SetError(txtReason, "Không nhập quá 4000 ký tự", DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning);
                    return; 
                }    
                CommonParam param = new CommonParam();
                speciaListExam.IS_APPROVAL = 2;//Từ chối
                speciaListExam.REJECT_APPROVAL_REASON = txtReason.Text.Trim();
                speciaListExam.INVITE_TYPE = 1;
                speciaListExam.EXAM_TIME = null;
                speciaListExam.EXAM_EXECUTE_CONTENT = null;
                speciaListExam.EXAM_EXCUTE = null;
                var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_SPECIALIST_EXAM>("api/HisSpecialistExam/Update", ApiConsumers.MosConsumer, speciaListExam, param);
                Inventec.Common.Logging.LogSystem.Debug("API Create Result: " + Inventec.Common.Logging.LogUtil.TraceData("DataA", result));
                if (result != null)
                {
                    if (dlgRefresh != null)
                        dlgRefresh();
                    this.Close();
                    MessageManager.Show(this, param, true);
                }
                else
                {
                    MessageBox.Show("Lưu thất bại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void frmReject_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                btnSave_Click(null, null);
                e.Handled = true;
            }
        }
    }
}
