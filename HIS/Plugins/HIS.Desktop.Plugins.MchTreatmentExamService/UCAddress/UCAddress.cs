using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.MchTreatmentExamService.ADO;
using Inventec.Common.Controls.PopupLoader;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.LanguageManager;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.UCAdress
{
    public partial class UCAddress : HIS.Desktop.Utility.UserControlBase
    {
        private bool IsChangeStrucAdreess { get; set; }
        private Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        private List<Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        private string moduleLink = "HIS.Desktop.Plugins.MchTreatmentExamService";
        private bool IsNotCheckToggleAddress { get; set; }
        private bool IsInitForm { get; set; } = false;

        private bool IsChangeValueMember = false;

        public UCAddress()
        {
            InitializeComponent();
        }

        private void UCAdress_Load(object sender, EventArgs e)
        {
            IsInitForm = true;
            SetDefaultDataToControl();
            InitControlState();
            IsInitForm = false;
        }
        private void FocusShowpopup(LookUpEdit cboEditor, bool isSelectFirstRow)
        {
            try
            {
                cboEditor.Focus();
                cboEditor.ShowPopup();
                if (isSelectFirstRow)
                    PopupLoader.SelectFirstRowPopup(cboEditor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item_ in this.currentControlStateRDO)
                    {
                        if (item_.KEY == togChangeStructAdress.Name)
                        {
                            togChangeStructAdress.IsOn = item_.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void togChangeStructAdress_Toggled(object sender, EventArgs e)
        {
            try
            {
                SetToolTipTog(sender);
                IsChangeStrucAdreess = togChangeStructAdress.IsOn;
                if (IsNotCheckToggleAddress)
                {
                    IsNotCheckToggleAddress = false;
                    return;
                }
                //if (!IsNotCheckToggleAddress && currentPatientSDO != null && currentPatientSDO.ID > 0 && (cboProvince.EditValue != null || cboDistrict.EditValue != null || cboCommune.EditValue != null) && DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Cần nhập lại thông tin địa chỉ theo định dạng {1}. Nhấn \"Có\" để tiếp tục thực hiện", currentPatientSDO.PATIENT_CODE, !togChangeStructAdress.IsOn ? "Tỉnh/Huyện/Xã" : "Tỉnh/Xã"), "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                //{
                //    IsNotCheckToggleAddress = true;
                //    IsChangeStrucAdreess = togChangeStructAdress.IsOn = !togChangeStructAdress.IsOn;
                //    return;
                //}
                SetDefaultDataToControl();
                if (IsChangeStrucAdreess)
                {
                    LoadSourceNoDistrict();
                }
                ClearDataAddress();
                IsNotCheckToggleAddress = false;
                if (IsInitForm)
                    return;
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == togChangeStructAdress.Name && o.MODULE_LINK == this.moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (togChangeStructAdress.IsOn ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = togChangeStructAdress.Name;
                    csAddOrUpdate.VALUE = (togChangeStructAdress.IsOn ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = this.moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetToolTipTog(object sender)
        {

            try
            {
                var toggleSwitch = sender as DevExpress.XtraEditors.ToggleSwitch;
                if (toggleSwitch != null)
                {
                    string tooltipText = !toggleSwitch.IsOn
                        ? "Sử dụng cấu trúc địa chỉ mới Xã - Tỉnh (không có Huyện)"
                        : "Sử dụng cấu trúc địa chỉ Xã - Huyện - Tỉnh";
                    toggleSwitch.ToolTip = tooltipText;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void LoadSourceNoDistrict()
        {
            IsChangeValueMember = true;
            Helper.ComboCommon.InitComboCommon(this.cboProvince, BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE && o.IS_NO_DISTRICT == 1).ToList(), "PROVINCE_CODE", "PROVINCE_NAME", "SEARCH_CODE");
            Helper.ComboCommon.InitComboCommon(this.cboDistrict, BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE && o.PROVINCE_ID.HasValue).ToList(), "COMMUNE_CODE", "RENDERER_COMMUNE_NAME", "SEARCH_CODE");
        }
        private void ClearDataAddress()
        {
            this.txtCommuneCode.Text = "";
            this.cboCommune.EditValue = null;
            this.txtProvinceCode.Text = "";
            this.cboProvince.EditValue = null;
            this.txtDistrictCode.Text = "";
            this.cboDistrict.EditValue = null;
            this.txtAddress.Text = "";
        }
        private void SetDefaultDataToControl()
        {
            try
            {
                IsChangeValueMember = true;
                Helper.ComboCommon.InitComboCommon(this.cboProvince, BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE && o.IS_NO_DISTRICT != 1).ToList(), "PROVINCE_CODE", "PROVINCE_NAME", "SEARCH_CODE");
                Helper.ComboCommon.InitComboCommon(this.cboDistrict, BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList(), "DISTRICT_CODE", "RENDERER_DISTRICT_NAME", "SEARCH_CODE");
                Helper.ComboCommon.InitComboCommon(this.cboCommune, BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE && o.IS_NO_DISTRICT != 1).ToList(), "COMMUNE_CODE", "RENDERER_COMMUNE_NAME", "SEARCH_CODE");
                ChangeComponentDistrict();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        private void ChangeComponentDistrict()
        {
            try
            {
                if (IsChangeStrucAdreess)
                {
                    lciDistrict.Text = lciCommune.Text;
                    lciCommune.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciAddress.TextSize = new Size(50, 20);

                }
                else
                {
                    lciDistrict.Text = "Huyện:";
                    lciCommune.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciAddress.TextSize = new Size(50, 20);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FocusToDistrict()
        {
            try
            {
                this.txtDistrictCode.Focus();
                this.txtDistrictCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProvince_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            try
            {
                if(e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete) {
                    cboProvince.EditValue = null;
                    txtProvinceCode.Text = "";
                    cboDistrict.EditValue = null;
                    txtDistrictCode.Text = "";
                    cboCommune.EditValue = null;
                    txtCommuneCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboDistrict_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboDistrict.EditValue = null;
                    txtDistrictCode.Text = "";
                    cboCommune.EditValue = null;
                    txtCommuneCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCommune_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboCommune.EditValue = null;
                    txtCommuneCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
