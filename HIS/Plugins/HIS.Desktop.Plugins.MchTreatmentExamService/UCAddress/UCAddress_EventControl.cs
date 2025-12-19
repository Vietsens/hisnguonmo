using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Logging;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.UCAdress
{
    public partial class UCAddress : HIS.Desktop.Utility.UserControlBase
    {
        private void txtProvinceCode_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.txtProvinceCode.Text))
                {
                    this.cboProvince.Properties.DataSource = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE && (IsChangeStrucAdreess ? o.IS_NO_DISTRICT == 1 : o.IS_NO_DISTRICT != 1)).ToList();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtProvinceCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.LoadTinhThanhCombo((sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper(), true);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }

        }

        private void txtDistrictCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string provinceCode = "";
                    if (this.cboProvince.EditValue != null)
                    {
                        provinceCode = this.cboProvince.EditValue.ToString();
                    }
                    this.LoadHuyenCombo((sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper(), provinceCode, true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistrict_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (IsChangeStrucAdreess)
                    {
                        NewEventClosedNoDistrict(sender);
                        return;
                    }
                    if (this.cboDistrict.EditValue != null
                        && this.cboDistrict.EditValue != this.cboDistrict.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList()
                            .SingleOrDefault(o => o.DISTRICT_CODE == this.cboDistrict.EditValue.ToString()
                                && (String.IsNullOrEmpty((this.cboProvince.EditValue ?? "").ToString()) || o.PROVINCE_CODE == (this.cboProvince.EditValue ?? "").ToString()));
                        if (district != null)
                        {
                            if (String.IsNullOrEmpty((this.cboProvince.EditValue ?? "").ToString()))
                            {
                                this.txtProvinceCode.Text = district.PROVINCE_CODE;
                                this.cboProvince.EditValue = district.PROVINCE_CODE;
                            }
                            this.LoadXaCombo("", district.DISTRICT_CODE, false);
                            this.txtDistrictCode.Text = district.SEARCH_CODE;
                            this.cboDistrict.EditValue = district.DISTRICT_CODE;
                            this.cboCommune.EditValue = null;
                            this.txtCommuneCode.Text = "";
                        }
                    }
                    FocusToCommune();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistrict_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (IsChangeValueMember)
                {
                    IsChangeValueMember = false;
                    cboDistrict.EditValue = null;
                }
                if (this.cboDistrict.EditValue != null && !(this.cboDistrict.EditValue.Equals(this.cboDistrict.OldEditValue == null ? "" : this.cboDistrict.OldEditValue)))
                {
                    this.SetValueHeinAddressByAddressOfPatient();
                }

                if (cboDistrict.EditValue == null || (!IsChangeStrucAdreess ? cboDistrict.EditValue != null && !(cboDistrict.Properties.DataSource as List<V_SDA_DISTRICT>).Exists(o => o.DISTRICT_CODE == cboDistrict.EditValue.ToString()) : cboDistrict.EditValue != null && !(cboDistrict.Properties.DataSource as List<V_SDA_COMMUNE>).Exists(o => o.COMMUNE_CODE == cboDistrict.EditValue.ToString())))
                {
                    this.cboDistrict.EditValue = null;
                    this.txtDistrictCode.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDistrict_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboDistrict.EditValue != null)
                    {
                        if (IsChangeStrucAdreess)
                        {
                            NewEventClosedNoDistrict(sender);
                            return;
                        }
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList()
                            .SingleOrDefault(o => o.DISTRICT_CODE == this.cboDistrict.EditValue.ToString()
                               && (String.IsNullOrEmpty((this.cboProvince.EditValue ?? "").ToString()) || o.PROVINCE_CODE == (this.cboProvince.EditValue ?? "").ToString()));
                        if (district != null)
                        {
                            if (String.IsNullOrEmpty((this.cboProvince.EditValue ?? "").ToString()))
                            {
                                this.cboProvince.EditValue = district.PROVINCE_CODE;
                            }
                            this.LoadXaCombo("", district.DISTRICT_CODE, false);
                            this.txtDistrictCode.Text = district.SEARCH_CODE;
                            this.cboCommune.EditValue = null;
                            this.txtCommuneCode.Text = "";
                            FocusToCommune();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCommuneCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string districtCode = "";
                    if (this.cboDistrict.EditValue != null)
                    {
                        districtCode = this.cboDistrict.EditValue.ToString();
                    }
                    this.LoadXaCombo((sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper(), districtCode, true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCommune_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboCommune.EditValue != null
                        && this.cboCommune.EditValue != cboCommune.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList()
                            .SingleOrDefault(o =>
                                o.COMMUNE_CODE == this.cboCommune.EditValue.ToString()
                                    && (String.IsNullOrEmpty((this.cboDistrict.EditValue ?? "").ToString()) || o.DISTRICT_CODE == (this.cboDistrict.EditValue ?? "").ToString())
                                );
                        if (commune != null)
                        {
                            this.txtCommuneCode.Text = commune.SEARCH_CODE;
                            if (String.IsNullOrEmpty((this.cboProvince.EditValue ?? "").ToString()) && String.IsNullOrEmpty((this.cboDistrict.EditValue ?? "").ToString()))
                            {
                                this.cboDistrict.EditValue = commune.DISTRICT_CODE;
                                SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().Where(o => o.ID == commune.DISTRICT_ID).FirstOrDefault();
                                if (district != null)
                                {
                                    this.cboProvince.EditValue = district.PROVINCE_CODE;
                                }
                            }
                        }
                    }
                    FocusToAddress();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCommune_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.cboCommune.EditValue != null && !(this.cboCommune.EditValue.Equals(this.cboCommune.OldEditValue == null ? "" : this.cboCommune.OldEditValue)))
                {
                    this.SetValueHeinAddressByAddressOfPatient();
                }
                if (cboCommune.EditValue == null || (cboCommune.EditValue != null && !(cboCommune.Properties.DataSource as List<V_SDA_COMMUNE>).Exists(o => o.COMMUNE_CODE == cboCommune.EditValue.ToString())))
                {
                    this.cboCommune.EditValue = null;
                    this.txtCommuneCode.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("cboCommune_EditValueChanged:\n" + ex);
            }
        }

        private void cboCommune_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboCommune.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList()
                            .SingleOrDefault(o =>
                                o.COMMUNE_CODE == this.cboCommune.EditValue.ToString()
                                && (String.IsNullOrEmpty((this.cboDistrict.EditValue ?? "").ToString()) || o.DISTRICT_CODE == (this.cboDistrict.EditValue ?? "").ToString()));
                        if (commune != null)
                        {
                            this.txtCommuneCode.Text = commune.SEARCH_CODE;
                            if (String.IsNullOrEmpty((this.cboProvince.EditValue ?? "").ToString()) && String.IsNullOrEmpty((this.cboDistrict.EditValue ?? "").ToString()))
                            {
                                this.cboDistrict.EditValue = commune.DISTRICT_CODE;
                                SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().Where(o => o.ID == commune.DISTRICT_ID).FirstOrDefault();
                                if (district != null)
                                {
                                    this.cboProvince.EditValue = district.PROVINCE_CODE;
                                }
                            }
                            FocusToAddress();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProvince_EditValueChanged(object sender, EventArgs e)
        {
            try
            {

                if (this.cboProvince.EditValue != null && !(cboProvince.EditValue.Equals(this.cboProvince.OldEditValue == null ? "" : this.cboProvince.OldEditValue)))
                {
                    this.cboCommune.Properties.DataSource = null;
                    this.cboCommune.EditValue = null;
                    this.txtCommuneCode.Text = "";
                    this.txtDistrictCode.Text = "";
                    this.cboDistrict.EditValue = null;

                    this.SetValueHeinAddressByAddressOfPatient();
                }
                if (cboProvince.EditValue == null || (cboProvince.EditValue != null && !(cboProvince.Properties.DataSource as List<V_SDA_PROVINCE>).Exists(o => o.PROVINCE_CODE == cboProvince.EditValue.ToString())))
                {
                    this.cboProvince.EditValue = null;
                    this.txtDistrictCode.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Goi ham gan du lieu thong tinh hanh chinh tu su kien cboProvince_EditValuechanged khong thanh cong : \n" + ex);
            }
        }

        private void cboProvince_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboProvince.EditValue != null
                        && this.cboProvince.EditValue != this.cboProvince.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = ((List<V_SDA_PROVINCE>)cboProvince.Properties.DataSource).Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE && (IsChangeStrucAdreess ? o.IS_NO_DISTRICT == 1 : o.IS_NO_DISTRICT != 1)).ToList().FirstOrDefault(o => o.PROVINCE_CODE == cboProvince.EditValue.ToString());
                        if (province != null)
                        {
                            this.LoadHuyenCombo("", province.PROVINCE_CODE, false);
                            this.txtProvinceCode.Text = province.SEARCH_CODE;
                        }
                    }
                    FocusToDistrict();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProvince_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboProvince.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().SingleOrDefault(o => o.PROVINCE_CODE == this.cboProvince.EditValue.ToString());
                        if (province != null)
                        {
                            this.LoadHuyenCombo("", province.PROVINCE_CODE, false);
                            this.txtProvinceCode.Text = province.SEARCH_CODE;
                            this.txtDistrictCode.Text = "";
                            FocusToDistrict();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProvince_Properties_GetNotInListValue(object sender, DevExpress.XtraEditors.Controls.GetNotInListValueEventArgs e)
        {
            try
            {
                if (e.FieldName == "RENDERER_PROVINCE_NAME")
                {
                    var item = ((List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>)this.cboProvince.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0} {1}", "", item.PROVINCE_NAME);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistrict_Properties_GetNotInListValue(object sender, DevExpress.XtraEditors.Controls.GetNotInListValueEventArgs e)
        {
            try
            {
                if (e.RecordIndex > -1)
                {
                    if (e.FieldName == "RENDERER_DISTRICT_NAME")
                    {
                        var item = ((List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>)this.cboDistrict.Properties.DataSource)[e.RecordIndex];
                        if (item != null)
                            e.Value = string.Format("{0} {1}", item.INITIAL_NAME, item.DISTRICT_NAME);
                    }
                    if (e.FieldName == "RENDERER_COMMUNE_NAME")
                    {
                        var item = ((List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>)this.cboDistrict.Properties.DataSource)[e.RecordIndex];
                        if (item != null)
                            e.Value = string.Format("{0} {1}", item.INITIAL_NAME, item.COMMUNE_NAME);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCommune_Properties_GetNotInListValue(object sender, DevExpress.XtraEditors.Controls.GetNotInListValueEventArgs e)
        {
            try
            {
                if (e.FieldName == "RENDERER_COMMUNE_NAME")
                {
                    var item = ((List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>)this.cboCommune.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0} {1}", item.INITIAL_NAME, item.COMMUNE_NAME);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAddress_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                string address = (String.IsNullOrEmpty(txtAddress.Text) == true ? "" : this.txtAddress.Text);
                if (!IsNotLoadChangeAddressTxt)
                {
                    this.SetValueHeinAddressByAddressOfPatient();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Goi ham gan du lieu thong tin hanh chinh tu event txtAddress_EditValueChanged khong thanh cong:\n" + ex);
            }
        }
    }
}
