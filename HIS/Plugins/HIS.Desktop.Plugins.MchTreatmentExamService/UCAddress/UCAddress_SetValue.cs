using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.MchTreatmentExamService.ADO;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.UCAdress
{
    public partial class UCAddress : HIS.Desktop.Utility.UserControlBase
    {

        public void SetValue(UCAddressADO data)
        {
            try
            {
                ClearDataAddress();
                if (data != null)
                {
                    if ((string.IsNullOrEmpty(data.District_Code) && string.IsNullOrEmpty(data.District_Name) && IsChangeStrucAdreess) || data.IsNoDistrict || (string.IsNullOrEmpty(data.District_Code) && !string.IsNullOrEmpty(data.Province_Code) && !string.IsNullOrEmpty(data.Commune_Code)))
                    {
                        if (!togChangeStructAdress.IsOn)
                        {
                            IsNotCheckToggleAddress = true;
                            togChangeStructAdress.IsOn = true;
                            ChangeComponentDistrict();
                            LoadSourceNoDistrict();
                            IsNotCheckToggleAddress = false;
                        }
                        SetValueNew(data);
                        return;
                    }
                    IsNotCheckToggleAddress = true;
                    togChangeStructAdress.IsOn = false;
                    ChangeComponentDistrict();
                    IsChangeValueMember = false;
                    IsNotCheckToggleAddress = false;
                    var province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().FirstOrDefault(o => o.PROVINCE_NAME == data.Province_Name);
                    if (province != null)
                    {
                        this.txtProvinceCode.Text = province.PROVINCE_CODE;
                        this.cboProvince.EditValue = province.PROVINCE_CODE;
                    }
                    var district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().FirstOrDefault(o => ((o.INITIAL_NAME + " " + o.DISTRICT_NAME) == data.District_Name || o.DISTRICT_NAME == data.District_Name || o.DISTRICT_CODE == data.District_Code) && o.PROVINCE_CODE == data.Province_Code);
                    if (district != null)
                    {
                        this.LoadHuyenCombo("", district.PROVINCE_CODE, false);
                        this.txtDistrictCode.Text = district.DISTRICT_CODE;
                        this.cboDistrict.EditValue = district.DISTRICT_CODE;
                    }
                    var commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().FirstOrDefault(o =>
                    ((o.INITIAL_NAME + " " + o.COMMUNE_NAME) == data.Commune_Name || o.COMMUNE_NAME == data.Commune_Name)
                    && ((o.DISTRICT_INITIAL_NAME + " " + o.DISTRICT_NAME) == data.District_Name || o.DISTRICT_NAME == data.District_Name));
                    if (commune != null)
                    {
                        this.LoadXaCombo("", IsChangeStrucAdreess ? commune.PROVINCE_CODE : commune.DISTRICT_CODE, false);
                        this.txtCommuneCode.Text = commune.COMMUNE_CODE;
                        this.cboCommune.EditValue = commune.COMMUNE_CODE;
                    }
                    
                    if (!string.IsNullOrEmpty(data.Address))
                    {
                        this.txtAddress.Text = data.Address;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void ResetValue()
        {
            try
            {
                ClearDataAddress();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetValueNew(UCAddressADO data)
        {
            try
            {
                var province = ((List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>)cboProvince.Properties.DataSource).FirstOrDefault(o => o.PROVINCE_NAME == data.Province_Name || o.PROVINCE_NAME.ToLower().Replace("thành phố", "").Replace("tỉnh", "") == data.Province_Name.ToLower().Replace("thành phố", "").Replace("tỉnh", ""));
                if (province != null)
                {
                    this.txtProvinceCode.Text = province.PROVINCE_CODE;
                    this.cboProvince.EditValue = province.PROVINCE_CODE;
                }
                var commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().FirstOrDefault(o =>
                ((o.INITIAL_NAME + " " + o.COMMUNE_NAME) == data.Commune_Name || o.COMMUNE_NAME == data.Commune_Name)
                && o.PROVINCE_CODE == (this.cboProvince.EditValue != null ? this.cboProvince.EditValue.ToString() : "") && o.IS_NO_DISTRICT == 1);
                if (commune != null)
                {
                    this.LoadXaCombo("", commune.PROVINCE_CODE, false, true);
                    this.txtDistrictCode.Text = commune.COMMUNE_CODE;
                    this.cboDistrict.EditValue = commune.COMMUNE_CODE;
                }
                else if (province != null)
                {
                    this.LoadXaCombo("", province.PROVINCE_CODE, false, true);
                }
                
                if (!string.IsNullOrEmpty(data.Address))
                {
                    this.txtAddress.Text = data.Address;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        /// <summary>
        /// Hàm gán dữ liệu địa chỉ thẻ
        /// </summary>
        private void SetValueHeinAddressByAddressOfPatient()
        {
            try
            {
                SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = null;
                SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = null;
                SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = null;
                string address = "";
                address = this.txtAddress.Text;
                if (cboProvince.EditValue != null)
                {
                    province = BackendDataWorker.Get<V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().FirstOrDefault(o => o.PROVINCE_CODE == (string)this.cboProvince.EditValue);
                    this.ChangeReplaceAddress(cboProvince.Text, "Tỉnh", ref address);
                }
                if (!IsChangeStrucAdreess)
                {
                    if (this.cboDistrict.EditValue != null)
                    {
                        district = BackendDataWorker.Get<V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().FirstOrDefault(o => o.DISTRICT_CODE == (string)this.cboDistrict.EditValue);
                        this.ChangeReplaceAddress(cboDistrict.Text, "Huyện", ref address);
                    }

                    if (this.cboCommune.EditValue != null)
                    {
                        commune = BackendDataWorker.Get<V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE && o.IS_NO_DISTRICT != 1).ToList().FirstOrDefault(o => o.COMMUNE_CODE == (string)this.cboCommune.EditValue);
                        this.ChangeReplaceAddress(cboCommune.Text, "Xã", ref address);
                    }
                }
                else
                {
                    commune = BackendDataWorker.Get<V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE && o.IS_NO_DISTRICT == 1).ToList().FirstOrDefault(o => o.COMMUNE_CODE == (string)this.cboDistrict.EditValue);
                    this.ChangeReplaceAddress(cboDistrict.Text, "Xã", ref address);
                }

                IsNotLoadChangeAddressTxt = true; this.txtAddress.Text = address;
                IsNotLoadChangeAddressTxt = false;

            }
            catch (Exception ex)
            {
                IsNotLoadChangeAddressTxt = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void ChangeReplaceAddress(string cmd, string lever, ref string address)
        {
            try
            {
                if (!string.IsNullOrEmpty(address) && !string.IsNullOrEmpty(cmd))
                {
                    string[] addressSplit = address.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    var datas = addressSplit.Where(p => p.ToLower().Contains(cmd.ToLower()) || cmd.ToLower().Contains(p.ToLower())).ToList();
                    if (datas != null && datas.Count > 0)
                    {
                        if (datas.Count == 1)
                        {
                            string addressNew = "," + datas[0];
                            if (address.Contains(addressNew))
                            {
                                address = address.Replace(addressNew, "");
                            }
                            else
                            {
                                address = address.Replace(datas[0], "");
                            }
                        }
                        else
                        {
                            string addressV2 = lever + " " + cmd;
                            var data = datas.FirstOrDefault(p => p.ToLower().Contains(addressV2.ToLower()));
                            if (data != null)
                            {
                                string addressNew = "," + data;
                                if (address.Contains(addressNew))
                                {
                                    address = address.Replace(addressNew, "");
                                }
                                else
                                {
                                    address = address.Replace(data, "");
                                }
                            }
                            var dataEquals = datas.FirstOrDefault(p => p.Trim().ToLower().Equals(cmd.ToLower()));
                            if (dataEquals != null)
                            {
                                string addressNew = "," + dataEquals;
                                if (address.Contains(addressNew))
                                {
                                    address = address.Replace(addressNew, "");
                                }
                                else
                                {
                                    address = address.Replace(dataEquals, "");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                IsNotLoadChangeAddressTxt = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        bool IsNotLoadChangeAddressTxt = false;
        private void NewEventClosedNoDistrict(object sender)
        {
            try
            {

                if (this.cboDistrict.EditValue != null
                    && this.cboDistrict.EditValue != this.cboDistrict.OldEditValue)
                {
                    SDA.EFMODEL.DataModels.V_SDA_COMMUNE district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList()
                        .FirstOrDefault(o => o.COMMUNE_CODE == this.cboDistrict.EditValue.ToString()
                            && (String.IsNullOrEmpty((this.cboProvince.EditValue ?? "").ToString()) || o.PROVINCE_CODE == (this.cboProvince.EditValue ?? "").ToString()));
                    if (district != null)
                    {
                        if (String.IsNullOrEmpty((this.cboProvince.EditValue ?? "").ToString()))
                        {
                            this.txtProvinceCode.Text = district.PROVINCE_CODE;
                            this.cboProvince.EditValue = district.PROVINCE_CODE;
                        }
                        this.txtDistrictCode.Text = district.SEARCH_CODE;
                        this.cboDistrict.EditValue = district.COMMUNE_CODE;
                    }
                }
                FocusToAddress();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void FocusToAddress()
        {
            try
            {

                this.txtAddress.Focus();
                this.txtAddress.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void FocusToCommune()
        {
            try
            {
                this.txtCommuneCode.Focus();
                this.txtCommuneCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
