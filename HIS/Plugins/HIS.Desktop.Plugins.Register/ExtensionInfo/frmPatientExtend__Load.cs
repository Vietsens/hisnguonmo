/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Controls.PopupLoader;
using Inventec.Core;
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

namespace HIS.Desktop.Plugins.Register.PatientExtend
{
    public partial class frmPatientExtend : HIS.Desktop.Utility.FormBase
    {
        //private HIS_HOUSEHOLD GetHouseHoldByCode(string code)
        //{
        //    HIS_HOUSEHOLD result = null;
        //    try
        //    {
        //        CommonParam param = new CommonParam();
        //        result = new BackendAdapter(param).Get<HIS_HOUSEHOLD>(RequestUriStore.HIS_HOUSE_HOLD_GET_BY_CODE, ApiConsumers.MosConsumer, code, param);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //    return result;
        //}

        private void LoadDefaultPatientInfo()
        {
            try
            {
                if (patientInformation != null)
                {
                    txtAdressRelation.Text = patientInformation.HT_ADDRESS;
                    if (patientInformation.BLOOD_ABO_ID > 0)
                    {
                        cboBlood.EditValue = patientInformation.BLOOD_ABO_ID;
                    }
                    else if (!String.IsNullOrEmpty(patientInformation.BLOOD_ABO_CODE))
                    {
                        var abo = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_ABO>().FirstOrDefault(o => o.BLOOD_ABO_CODE == patientInformation.BLOOD_ABO_CODE);
                        if (abo != null)
                        {
                            cboBlood.EditValue = abo.ID;
                        }
                    }
                    if (patientInformation.BLOOD_RH_ID > 0)
                    {
                        cboBloodRh.EditValue = patientInformation.BLOOD_RH_ID;
                    }
                    else if (!String.IsNullOrEmpty(patientInformation.BLOOD_RH_CODE))
                    {
                        var rh = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_RH>().FirstOrDefault(o => o.BLOOD_RH_CODE == patientInformation.BLOOD_RH_CODE);
                        if (rh != null)
                        {
                            cboBloodRh.EditValue = rh.ID;
                        }
                    }
                    if (patientInformation.HOUSEHOLD_RELATION_ID > 0)
                    {
                        cboHOUSEHOLD_RELATION_ID.EditValue = patientInformation.HOUSEHOLD_RELATION_ID;
                    }
                    else if (!String.IsNullOrEmpty(patientInformation.HOUSEHOLD_RELATION_NAME))
                    {
                        var hrl = this.houseHoldRelates.FirstOrDefault(o => o.HOUSEHOLD_RELATION_NAME == patientInformation.HOUSEHOLD_RELATION_NAME);
                        if (hrl != null)
                        {
                            cboHOUSEHOLD_RELATION_ID.EditValue = hrl.ID;
                        }
                    }
                    txtHOUSEHOLD_CODE.Text = patientInformation.HOUSEHOLD_CODE;

                    if (!String.IsNullOrEmpty(patientInformation.CMND_NUMBER))
                    {
                        txtCmndNumber.Text = patientInformation.CMND_NUMBER;
                    }

                    if ((patientInformation.CMND_DATE ?? 0) > 0)
                    {
                        dtCmndDate.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientInformation.CMND_DATE ?? 0).Value;
                    }

                    if (!String.IsNullOrEmpty(patientInformation.CMND_PLACE))
                    {
                        txtCmndPlace.Text = patientInformation.CMND_PLACE;
                    }

                    if (!String.IsNullOrEmpty(patientInformation.EMAIL))
                    {
                        txtEmail.Text = patientInformation.EMAIL;
                    }

                    if (!String.IsNullOrEmpty(patientInformation.MOTHER_NAME))
                    {
                        txtMotherName.Text = patientInformation.MOTHER_NAME;
                    }

                    if (!String.IsNullOrEmpty(patientInformation.FATHER_NAME))
                    {
                        txtFatherName.Text = patientInformation.FATHER_NAME;
                    }

                    if (!String.IsNullOrEmpty(patientInformation.HT_PROVINCE_CODE))
                    {
                        var province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).SingleOrDefault(o => o.PROVINCE_CODE == patientInformation.HT_PROVINCE_CODE);
                        if (province != null)
                        {
                            txtProvince.Text = patientInformation.HT_PROVINCE_CODE;
                            cboProvince.EditValue = province.ID;

                            List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> districts = new List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();
                            districts = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).Where(o => o.PROVINCE_CODE == patientInformation.HT_PROVINCE_CODE).ToList();

                            if (districts != null && districts.Count > 0)
                            {
                                InitComboDistrict(cboDistrict, districts);
                                if (!String.IsNullOrEmpty(patientInformation.HT_DISTRICT_CODE))
                                {
                                    var district = districts.SingleOrDefault(o => o.DISTRICT_CODE == patientInformation.HT_DISTRICT_CODE);
                                    if (district != null)
                                    {
                                        txtDistricts.Text = patientInformation.HT_DISTRICT_CODE;
                                        cboDistrict.EditValue = district.ID;

                                        List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE> communes = new List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>();
                                        communes = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).Where(o => o.DISTRICT_ID == district.ID).ToList();
                                        if (communes != null && communes.Count > 0)
                                        {
                                            InitComboCommune(cboCommune, communes);
                                            if (!String.IsNullOrEmpty(patientInformation.HT_COMMUNE_CODE))
                                            {
                                                var commune = communes.SingleOrDefault(o => o.COMMUNE_CODE == patientInformation.HT_COMMUNE_CODE);
                                                if (commune != null)
                                                {
                                                    txtCommune.Text = patientInformation.HT_COMMUNE_CODE;
                                                    cboCommune.EditValue = commune.ID;
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboBloodRH()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_RH_CODE", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_RH_CODE", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboBloodRh, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_RH>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboHoldHouse()
        {
            try
            {
                CommonParam param = new CommonParam();
                HID.Filter.HidHouseholdRelationFilter householdRelationFilter = new HID.Filter.HidHouseholdRelationFilter();
                householdRelationFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                this.houseHoldRelates = new BackendAdapter(param).Get<List<HID.EFMODEL.DataModels.HID_HOUSEHOLD_RELATION>>(RequestUriStore.HID_HOUSEHOLD_RELATION_GET, ApiConsumers.HidConsumer, householdRelationFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("HOUSEHOLD_RELATION_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("HOUSEHOLD_RELATION_NAME", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(cboHOUSEHOLD_RELATION_ID, this.houseHoldRelates, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboBloodABO()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_ABO_CODE", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_ABO_CODE", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboBlood, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_ABO>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboProvince(object control, List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PROVINCE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PROVINCE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PROVINCE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(control, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboDistrict(object control, List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DISTRICT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("RENDERER_DISTRICT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("RENDERER_DISTRICT_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(control, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCommune(object control, List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("COMMUNE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("RENDERER_COMMUNE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("RENDERER_COMMUNE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(control, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadProvinceCombo(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboCommune.Properties.DataSource = null;
                    cboCommune.EditValue = null;
                    txtCommune.Text = "";
                    cboDistrict.Properties.DataSource = null;
                    cboDistrict.EditValue = null;
                    txtDistricts.Text = "";
                    cboProvince.EditValue = null;
                    cboProvince.Focus();
                    cboProvince.ShowPopup();
                    PopupLoader.SelectFirstRowPopup(cboProvince);
                }
                else
                {
                    List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                    listResult = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).Where(o => o.PROVINCE_CODE.Contains(searchCode)).ToList();
                    if (listResult.Count == 1)
                    {
                        cboProvince.EditValue = listResult[0].ID;
                        txtProvince.Text = listResult[0].PROVINCE_CODE;
                        LoadDistrictsCombo("", listResult[0].ID.ToString(), false);
                        if (isExpand)
                        {
                            txtDistricts.Focus();
                            txtDistricts.SelectAll();
                        }
                    }
                    else if (listResult.Count > 1)
                    {
                        cboCommune.Properties.DataSource = null;
                        cboCommune.EditValue = null;
                        txtCommune.Text = "";
                        cboDistrict.Properties.DataSource = null;
                        cboDistrict.EditValue = null;
                        txtDistricts.Text = "";
                        cboProvince.EditValue = null;
                        if (isExpand)
                        {
                            cboProvince.Focus();
                            cboProvince.ShowPopup();
                            PopupLoader.SelectFirstRowPopup(cboProvince);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDistrictsCombo(string searchCode, string provinceId, bool isExpand)
        {
            try
            {
                List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();
                listResult = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).Where(o => o.DISTRICT_CODE.Contains(searchCode) && (provinceId == "" || o.PROVINCE_ID == Inventec.Common.TypeConvert.Parse.ToInt64(provinceId))).ToList();

                InitComboDistrict(cboDistrict, listResult);

                if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(provinceId) && listResult.Count > 0)
                {
                    cboCommune.Properties.DataSource = null;
                    cboCommune.EditValue = null;
                    txtCommune.Text = "";
                    txtDistricts.Text = "";
                    cboDistrict.EditValue = null;
                    cboDistrict.Focus();
                    cboDistrict.ShowPopup();
                    PopupLoader.SelectFirstRowPopup(cboDistrict);
                }
                else
                {
                    if (listResult.Count == 1)
                    {
                        cboDistrict.EditValue = listResult[0].ID;
                        txtDistricts.Text = listResult[0].DISTRICT_CODE;
                        LoadCommuneCombo("", listResult[0].ID.ToString(), false);
                        if (isExpand)
                        {
                            txtCommune.Focus();
                            txtCommune.SelectAll();
                        }
                    }
                    else if (listResult.Count > 1)
                    {
                        cboCommune.Properties.DataSource = null;
                        cboCommune.EditValue = null;
                        txtCommune.Text = "";
                        txtDistricts.Text = "";
                        cboDistrict.EditValue = null;
                        if (isExpand)
                        {
                            cboDistrict.Focus();
                            cboDistrict.ShowPopup();
                            PopupLoader.SelectFirstRowPopup(cboDistrict);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCommuneCombo(string searchCode, string districtID, bool isExpand)
        {
            try
            {
                List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>();
                listResult = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).Where(o => o.COMMUNE_CODE.Contains(searchCode) && (districtID == "" || o.DISTRICT_ID == Inventec.Common.TypeConvert.Parse.ToInt64(districtID))).ToList();

                InitComboCommune(cboCommune, listResult);

                if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(districtID) && listResult.Count > 0)
                {
                    cboCommune.EditValue = null;
                    txtCommune.Text = "";
                    cboCommune.Focus();
                    cboCommune.ShowPopup();
                    PopupLoader.SelectFirstRowPopup(cboCommune);
                }
                else
                {
                    if (listResult.Count == 1)
                    {
                        cboCommune.EditValue = listResult[0].ID;
                        txtCommune.Text = listResult[0].COMMUNE_CODE;
                        if (isExpand)
                        {
                            txtAdressRelation.Focus();
                            txtAdressRelation.SelectAll();
                        }
                    }
                    else if (isExpand && listResult.Count > 1)
                    {
                        cboCommune.EditValue = null;
                        cboCommune.Focus();
                        cboCommune.ShowPopup();
                        PopupLoader.SelectFirstRowPopup(cboCommune);
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
