using HIS.Desktop.LocalStorage.BackendData;
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

        private void LoadHuyenCombo(string searchCode, string provinceCode, bool isExpand)
        {
            try
            {
                if (IsChangeStrucAdreess)
                {
                    LoadXaComboNoDistrict(searchCode, provinceCode, isExpand);
                    return;
                }
                List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();
                listResult = BackendDataWorker.Get<V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().Where(o => ((o.SEARCH_CODE ?? "").ToUpper().Contains(searchCode.ToUpper()) || o.DISTRICT_CODE == searchCode) && (provinceCode == "" || o.PROVINCE_CODE == provinceCode)).ToList();

                bool isReLoadRef = false;
                if (listResult[0].DISTRICT_CODE != (this.cboDistrict.EditValue ?? "").ToString())
                {
                    isReLoadRef = true;
                }

                if (!isReLoadRef)
                {
                    if (isExpand)
                    {
                        this.txtCommuneCode.Focus();
                        this.txtCommuneCode.SelectAll();
                    }
                    return;
                }

                Helper.ComboCommon.InitComboCommon(this.cboDistrict, listResult, "DISTRICT_CODE", "RENDERER_DISTRICT_NAME", "SEARCH_CODE");

                if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(provinceCode) && listResult.Count > 0)
                {
                    this.cboCommune.Properties.DataSource = null;
                    this.cboCommune.EditValue = null;
                    this.txtCommuneCode.Text = "";
                    this.txtDistrictCode.Text = "";
                    this.cboDistrict.EditValue = null;
                    this.FocusShowpopup(this.cboDistrict, false);
                }
                else
                {
                    if (listResult.Count == 1)
                    {
                        this.txtDistrictCode.Text = listResult[0].DISTRICT_CODE;
                        this.cboDistrict.EditValue = listResult[0].DISTRICT_CODE;
                        if (String.IsNullOrEmpty(this.cboProvince.Text))
                        {
                            this.txtProvinceCode.Text = listResult[0].PROVINCE_CODE;
                            this.cboProvince.EditValue = listResult[0].PROVINCE_CODE;
                        }
                        this.LoadXaCombo("", listResult[0].DISTRICT_CODE, false);

                        if (isExpand)
                        {
                            this.txtCommuneCode.Focus();
                            this.txtCommuneCode.SelectAll();
                        }
                    }
                    else
                    {
                        this.cboCommune.Properties.DataSource = null;
                        this.cboCommune.EditValue = null;
                        this.txtCommuneCode.Text = "";
                        this.cboDistrict.EditValue = null;
                        if (isExpand)
                        {
                            this.FocusShowpopup(this.cboDistrict, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadXaCombo(string searchCode, string districtCode, bool isExpand, bool IsNoDistrict = false)
        {
            try
            {
                if (IsNoDistrict)
                {
                    LoadXaComboNoDistrict(searchCode, districtCode, isExpand);
                    return;
                }
                List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE> listResult = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList()
                    .Where(o => ((o.SEARCH_CODE ?? "").Contains(searchCode ?? "") || o.COMMUNE_CODE == searchCode)
                        && (String.IsNullOrEmpty(districtCode) || o.DISTRICT_CODE == districtCode)).ToList();
                Helper.ComboCommon.InitComboCommon(this.cboCommune, listResult, "COMMUNE_CODE", "RENDERER_COMMUNE_NAME", "SEARCH_CODE");
                if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(districtCode) && listResult.Count > 0)
                {
                    this.cboCommune.EditValue = null;
                    this.txtCommuneCode.Text = "";
                    this.FocusShowpopup(this.cboCommune, false);
                }
                else
                {
                    if (listResult.Count == 1)
                    {
                        this.txtCommuneCode.Text = listResult[0].SEARCH_CODE;
                        this.cboCommune.EditValue = listResult[0].COMMUNE_CODE;

                        this.txtDistrictCode.Text = listResult[0].DISTRICT_CODE;
                        this.cboDistrict.EditValue = listResult[0].DISTRICT_CODE;
                        if (String.IsNullOrEmpty(this.cboProvince.Text))
                        {
                            SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().Where(o => o.ID == listResult[0].DISTRICT_ID).FirstOrDefault();
                            if (district != null)
                            {
                                this.txtProvinceCode.Text = district.PROVINCE_CODE;
                                this.cboProvince.EditValue = district.PROVINCE_CODE;
                            }
                        }

                        if (isExpand)
                        {
                            this.txtAddress.Focus();
                            this.txtAddress.SelectAll();
                        }
                    }
                    else if (isExpand)
                    {
                        this.cboCommune.EditValue = null;
                        this.FocusShowpopup(this.cboCommune, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadXaComboNoDistrict(string searchCode, string proviceCode, bool isExpand)
        {
            List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE> listResult = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList()
                    .Where(o => ((o.SEARCH_CODE ?? "").Contains(searchCode ?? "") || o.COMMUNE_CODE == searchCode || string.IsNullOrEmpty(searchCode ?? ""))
                        && (String.IsNullOrEmpty(proviceCode) || o.PROVINCE_CODE == proviceCode) && o.IS_NO_DISTRICT == 1).ToList();
            Helper.ComboCommon.InitComboCommon(this.cboDistrict, listResult, "COMMUNE_CODE", "RENDERER_COMMUNE_NAME", "SEARCH_CODE");
            if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(proviceCode) && listResult.Count > 0)
            {
                this.cboDistrict.EditValue = null;
                this.txtDistrictCode.Text = "";
                this.FocusShowpopup(this.cboDistrict, false);
            }
            else
            {
                if (listResult.Count == 1)
                {
                    this.txtDistrictCode.Text = listResult[0].COMMUNE_CODE;
                    this.cboDistrict.EditValue = listResult[0].COMMUNE_CODE;

                    if (String.IsNullOrEmpty(this.cboProvince.Text))
                    {
                        this.txtProvinceCode.Text = listResult[0].PROVINCE_CODE;
                        this.cboProvince.EditValue = listResult[0].PROVINCE_CODE;
                    }

                    if (isExpand)
                    {
                        this.txtAddress.Focus();
                        this.txtAddress.SelectAll();
                    }
                }
                else if (isExpand)
                {
                    this.cboDistrict.EditValue = null;
                    this.FocusShowpopup(this.cboDistrict, false);
                }
            }
        }

        private void LoadTinhThanhCombo(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    this.cboCommune.Properties.DataSource = null;
                    this.cboCommune.EditValue = null;
                    this.txtCommuneCode.Text = "";
                    this.cboDistrict.Properties.DataSource = null;
                    this.cboDistrict.EditValue = null;
                    this.txtDistrictCode.Text = "";
                    this.cboProvince.EditValue = null;
                    this.FocusShowpopup(this.cboProvince, false);
                }
                else
                {
                    List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                    listResult = BackendDataWorker.Get<V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE && (IsChangeStrucAdreess ? o.IS_NO_DISTRICT == 1 : o.IS_NO_DISTRICT != 1)).ToList().Where(o => (o.SEARCH_CODE ?? "").Contains(searchCode) || o.PROVINCE_CODE == searchCode).ToList();
                    if (listResult.Count == 1)
                    {
                        bool isReLoadRef = false;
                        if (listResult[0].PROVINCE_CODE != (this.cboProvince.EditValue ?? "").ToString())
                        {
                            isReLoadRef = true;
                        }
                        if (isReLoadRef)
                        {
                            this.txtProvinceCode.Text = listResult[0].SEARCH_CODE;
                            this.cboProvince.EditValue = listResult[0].PROVINCE_CODE;
                            this.LoadHuyenCombo("", listResult[0].PROVINCE_CODE, false);
                        }
                        if (isExpand)
                        {
                            this.txtDistrictCode.Focus();
                            this.txtDistrictCode.SelectAll();
                        }
                    }
                    else
                    {
                        this.cboCommune.Properties.DataSource = null;
                        this.cboCommune.EditValue = null;
                        this.txtCommuneCode.Text = "";
                        this.cboDistrict.Properties.DataSource = null;
                        this.cboDistrict.EditValue = null;
                        this.txtDistrictCode.Text = "";
                        this.cboProvince.EditValue = null;
                        if (isExpand)
                        {
                            this.cboProvince.Properties.DataSource = listResult;
                            this.FocusShowpopup(this.cboProvince, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
