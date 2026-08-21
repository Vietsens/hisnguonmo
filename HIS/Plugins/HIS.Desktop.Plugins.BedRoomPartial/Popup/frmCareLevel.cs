using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Integrate;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.BedRoomPartial.Popup
{
    public partial class frmCareLevel : Form
    {
        long treatmentId;
        DelegateRefreshData RefreshData { get; set; }
        List<HIS_CARE_LEVEL> lstCareLevel { get; set; }

        public frmCareLevel(long treatmentId, DelegateRefreshData RefreshData)
        {
            InitializeComponent();

            try
            {
                SetIcon();
                this.treatmentId = treatmentId;
                this.RefreshData = RefreshData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmCareLevel_Load(object sender, EventArgs e)
        {
            LoadDataToCboCareLevel();
            LoadCurrentCareLevel();
        }

        /// <summary>
        /// Nap danh muc phan cap cham soc (HIS_CARE_LEVEL) vao combo
        /// </summary>
        private void LoadDataToCboCareLevel()
        {
            try
            {
                lstCareLevel = BackendDataWorker.Get<HIS_CARE_LEVEL>();
                if (lstCareLevel == null)
                {
                    lstCareLevel = new List<HIS_CARE_LEVEL>();
                }

                lstCareLevel = lstCareLevel.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                                                    && o.IS_DELETE != IMSys.DbConfig.HIS_RS.COMMON.IS_DELETE__TRUE)
                                           .OrderBy(o => o.CARE_LEVEL_CODE)
                                           .ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("CARE_LEVEL_CODE", "Mã", 80, 1));
                columnInfos.Add(new ColumnInfo("CARE_LEVEL_NAME", "Tên", 190, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("CARE_LEVEL_NAME", "ID", columnInfos, false, 300);
                ControlEditorLoader.Load(cboCarreLevel, lstCareLevel, controlEditorADO);

                cboCarreLevel.Properties.ImmediatePopup = true;
                cboCarreLevel.Properties.PopupFormMinSize = new Size(300, cboCarreLevel.Properties.PopupFormMinSize.Height);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Lay phan cap cham soc dang luu tren ho so dieu tri de hien thi len combo
        /// </summary>
        private void LoadCurrentCareLevel()
        {
            try
            {
                if (treatmentId <= 0 || lstCareLevel == null || lstCareLevel.Count == 0)
                    return;

                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                MOS.Filter.HisTreatmentFilter filter = new MOS.Filter.HisTreatmentFilter();
                filter.ID = treatmentId;
                var treatments = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param);
                HIS_TREATMENT treatment = (treatments != null ? treatments.FirstOrDefault() : null);

                if (treatment != null
                    && treatment.CARE_LEVEL_ID.HasValue
                    && lstCareLevel.Exists(o => o.ID == treatment.CARE_LEVEL_ID.Value))
                {
                    cboCarreLevel.EditValue = treatment.CARE_LEVEL_ID.Value;
                }

                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            Inventec.Core.CommonParam common = new Inventec.Core.CommonParam();
            try
            {
                if (treatmentId <= 0)
                {
                    MessageBox.Show(this, "Không xác định được hồ sơ điều trị.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cboCarreLevel.EditValue == null)
                {
                    MessageBox.Show(this, "Bạn chưa chọn phân cấp chăm sóc.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboCarreLevel.Focus();
                    return;
                }

                long careLevelId = Inventec.Common.TypeConvert.Parse.ToInt64(cboCarreLevel.EditValue.ToString());
                if (careLevelId <= 0)
                {
                    MessageBox.Show(this, "Bạn chưa chọn phân cấp chăm sóc.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboCarreLevel.Focus();
                    return;
                }

                Inventec.Desktop.Common.Message.WaitingManager.Show();
                HisTreatmentUpdateCareLevelSDO sdo = new HisTreatmentUpdateCareLevelSDO();

                sdo.Id = treatmentId;
                sdo.CareLevelId = careLevelId;

                Inventec.Common.Logging.LogSystem.Info("frmCareLevel.UpdateCareLevel"
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));

                bool result = new Inventec.Common.Adapter.BackendAdapter(common).Post<bool>("api/HisTreatment/UpdateCareLevel", ApiConsumers.MosConsumer, sdo, common);

                Inventec.Desktop.Common.Message.WaitingManager.Hide();
                Inventec.Desktop.Common.Message.MessageManager.Show(this, common, result);

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(common);
                #endregion

                if (result)
                {
                    //Luu thanh cong: refresh lai luoi ben ngoai va dong form
                    if (this.RefreshData != null)
                        this.RefreshData();
                    this.Close();
                }
                else
                {
                    //Luu that bai: giu form de nguoi dung chon lai
                    cboCarreLevel.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            simpleButton1_Click(null, null);
        }

        private void cboCarreLevel_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (cboCarreLevel.EditValue != null && e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                cboCarreLevel.EditValue = null;
        }
    }
}
