using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.MessageBoxForm
{
    public partial class frmMutlilPatientReasonInBatch : Form
    {
        List<MediMatyTypeADO> lstData = new List<MediMatyTypeADO>();
        Action<List<MediMatyTypeADO>> lstCallback { get; set; }
        Action<List<MediMatyTypeADO>> lstCallbackDelete { get; set; }
        bool IsClickSave { get; set; }
        public frmMutlilPatientReasonInBatch(List<MediMatyTypeADO> lstData, Action<List<MediMatyTypeADO>> lstCallback)
        {
            InitializeComponent();
            this.lstData = lstData;
            this.lstCallback = lstCallback;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmMutlilPatientReasonInBatch_Load(object sender, EventArgs e)
        {

            try
            {
                gridControl1.DataSource = lstData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void gridView1_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            try
            {
                var info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
                info.GroupText = Convert.ToString(this.gridView1.GetGroupRowValue(e.RowHandle, this.gridColumn1) ?? "").Split('_')[0].ToString();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            try
            {
                var patientNotExceedReason = lstData.Where(o => string.IsNullOrEmpty(o.EXCEED_LIMIT_IN_BATCH_REASON)).ToList();
                if (patientNotExceedReason != null && patientNotExceedReason.Count > 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Bệnh nhân {0} chưa có lý do kê vượt quá thuốc trong đợt điều trị", string.Join(", ", patientNotExceedReason.Select(o => o.PATIENT_NAME_BY_TREATMENT_CODE.Split('_')[1] + " - "+ o.PATIENT_NAME_BY_TREATMENT_CODE.Split('_')[0]).Distinct().ToList())), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK);
                    return;
                }
                if (lstCallback != null)
                {
                    lstCallback(lstData);
                }
                this.IsClickSave = true;
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void frmMutlilPatientReasonInBatch_FormClosing(object sender, FormClosingEventArgs e)
        {

            try
            {
                if (!IsClickSave)
                {
                    btnSave.Focus();
                    var patientNotExceedReason = lstData.Where(o => string.IsNullOrEmpty(o.EXCEED_LIMIT_IN_BATCH_REASON)).ToList();
                    if (patientNotExceedReason != null && patientNotExceedReason.Count > 0)
                    {
                        var groupByPatientName = patientNotExceedReason.GroupBy(o => o.PATIENT_NAME_BY_TREATMENT_CODE).ToList();
                        List<string> lstPatient = new List<string>();
                        foreach (var item in groupByPatientName)
                        {
                            lstPatient.Add(string.Format("{0} có {1} chưa nhập lý do kê vượt quá thuốc trong đợt điều trị.", item.Key.Split('_')[1] + " - " + item.Key.Split('_')[0], string.Join(", ", item.ToList().Select(o => o.MEDICINE_TYPE_NAME).ToList())));
                        }

                        if (DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Danh sách bệnh nhân:\r\n{0}\r\nNếu đóng sẽ không lưu thuốc vừa xử lý. Bạn có muốn tiếp tục không?", string.Join("\r\n", lstPatient)), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            e.Cancel = true;
                            return;
                        }
                    }

                    if (lstCallback != null)
                    {
                        lstCallback(lstData.Where(o => !string.IsNullOrEmpty(o.EXCEED_LIMIT_IN_BATCH_REASON)).ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave.Focus();
            btnSave_Click(null, null);
        }
    }
}
