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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Utility;
using HIS.UC.PlusInfo.ADO;
using HIS.UC.PlusInfo.Config;
using HIS.UC.WorkPlace;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraLayout;
using HIS.UC.PlusInfo.ShareMethod;
using Inventec.Desktop.Common.Message;

namespace HIS.UC.PlusInfo
{
    public partial class UCPlusInfo : UserControlBase
    {
        #region Get - Set Data In UserControl

        private UCPlusInfoADO GetValueInUCPlusInfo(List<UserControl> listControl)
        {
            UCPlusInfoADO dataGet = new UCPlusInfoADO();
            try
            {
                for (int i = 0; i < listControl.Count; i++)
                {
                    switch (listControl[i].Name)
                    {
                        #region --------- Kiem tra control và get data -------------

                        case ChoiceControl.ucAddress:
                            dataGet.HT_ADDRESS = this.ucAddress1 != null ? this.ucAddress1.GetValue() : "";
                            break;
                        case ChoiceControl.ucAddressOfBirth:
                            dataGet.ADDRESS_OfBIRTH = this.ucAddressOfBirth1 != null ? this.ucAddressOfBirth1.GetValue() : "";
                            break;
                        case ChoiceControl.ucHohName:
                            dataGet.HOH_NAME = this.ucHohName1 != null ? this.ucHohName1.GetValue() : "";
                            break;
                        case ChoiceControl.ucWorkPlace:
                            if (this.ucWorkPlace1 != null)
                            {
                                dataGet.workPlace = this.ucWorkPlace1.GetValue(this.ucWorkPlace1);
                                if (dataGet.workPlace != null && (dataGet.workPlace is long || dataGet.workPlace is long?) && (long?)dataGet.workPlace > 0)
                                {
                                    if (dataGet.workPlaceADO == null)
                                    {
                                        dataGet.workPlaceADO = new WorkPlaceADO();
                                        dataGet.workPlaceADO.WORK_PLACE_ID = (long?)dataGet.workPlace;
                                    }
                                }
                                else if (dataGet.workPlace != null && dataGet.workPlace is string)
                                {
                                    if (dataGet.workPlaceADO == null)
                                    {
                                        dataGet.workPlaceADO = new WorkPlaceADO();
                                        dataGet.workPlaceADO.WORK_PLACE = (string)dataGet.workPlace;
                                    }
                                }
                            }
                            break;
                        case ChoiceControl.ucBlood:
                            UCPlusInfoADO dataGetBlood = new UCPlusInfoADO();
                            dataGetBlood = this.ucBlood1 != null ? this.ucBlood1.GetValue() : new UCPlusInfoADO();
                            if (dataGetBlood != null)
                            {
                                dataGet.BLOOD_ABO_CODE = dataGetBlood.BLOOD_ABO_CODE;
                                dataGet.BLOOD_ABO_ID = dataGetBlood.BLOOD_ABO_ID;
                            }
                            break;
                        case ChoiceControl.ucBloodRh:
                            UCPlusInfoADO dataGetBloodRh = new UCPlusInfoADO();
                            dataGetBloodRh = this.ucBloodRh1 != null ? this.ucBloodRh1.GetValue() : new UCPlusInfoADO();
                            if (dataGetBloodRh != null)
                            {
                                dataGet.BLOOD_RH_CODE = dataGetBloodRh.BLOOD_RH_CODE;
                                dataGet.BLOOD_RH_ID = dataGetBloodRh.BLOOD_RH_ID;
                            }
                            break;
                        case ChoiceControl.ucCmndDate:
                            dataGet.CMND_DATE = this.ucCMNDDate1 != null ? this.ucCMNDDate1.GetValue() : null;
                            break;
                        case ChoiceControl.ucCmndNumber:
                            if (this.ucCmndNumber1 != null)
                            {
                                var textCmnd = this.ucCmndNumber1 != null ? this.ucCmndNumber1.GetValue() : "";
                                if (!string.IsNullOrEmpty(textCmnd))
                                {
                                    try
                                    {
                                        textCmnd = textCmnd.Trim();
                                        var checkNumber = Int64.Parse(textCmnd);
                                        if (textCmnd.Length == 12)
                                        {
                                            dataGet.CCCD_NUMBER = textCmnd;
                                        }
                                        else if (textCmnd.Length == 9)
                                        {
                                            dataGet.CMND_NUMBER = textCmnd;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        if (textCmnd.Length <= 9)
                                        {
                                            dataGet.PASSPORT_NUMBER = textCmnd;
                                        }
                                    }
                                }
                            }

                            break;
                        case ChoiceControl.ucCmndPlace:
                            dataGet.CMND_PLACE = this.ucCMNDPlace1 != null ? this.ucCMNDPlace1.GetValue() : "";
                            break;
                        case ChoiceControl.ucCommuneNow:
                            UCPlusInfoADO dataGetCommune = new UCPlusInfoADO();
                            dataGetCommune = this.ucCommuneNow1 != null ? this.ucCommuneNow1.GetValue() : new UCPlusInfoADO();
                            if (dataGetCommune != null)
                            {
                                dataGet.HT_COMMUNE_CODE = dataGetCommune.HT_COMMUNE_CODE;
                                dataGet.HT_COMMUNE_NAME = dataGetCommune.HT_COMMUNE_NAME;
                            }
                            break;
                        case ChoiceControl.ucDistrictNow:
                            UCPlusInfoADO dataGetDistrict = new UCPlusInfoADO();
                            dataGetDistrict = this.ucDistrictNow1 != null ? this.ucDistrictNow1.GetValue() : new UCPlusInfoADO();
                            if (dataGetDistrict != null)
                            {
                                dataGet.HT_DISTRICT_CODE = dataGetDistrict.HT_DISTRICT_CODE;
                                dataGet.HT_DISTRICT_NAME = dataGetDistrict.HT_DISTRICT_NAME;
                            }
                            break;
                        case ChoiceControl.ucEmai:
                            dataGet.EMAIL = this.ucEmail1 != null ? this.ucEmail1.GetValue() : "";
                            break;
                        case ChoiceControl.ucPatientStoreCode:
                            dataGet.PATIENT_STORE_CODE = this.ucPatientStoreCode != null ? this.ucPatientStoreCode.GetValue() : "";
                            break;
                        case ChoiceControl.ucEthnic:
                            UCPlusInfoADO dataGetEthnic = new UCPlusInfoADO();
                            dataGetEthnic = this.ucEthnic1 != null ? this.ucEthnic1.GetValue() : new UCPlusInfoADO();
                            dataGet.ETHNIC_NAME = dataGetEthnic != null ? dataGetEthnic.ETHNIC_NAME : "";
                            dataGet.ETHNIC_CODE = dataGetEthnic != null ? dataGetEthnic.ETHNIC_CODE : "";
                            break;
                        case ChoiceControl.ucFatherName:
                            dataGet.FATHER_NAME = this.ucFather1 != null ? this.ucFather1.GetValue() : "";
                            break;
                        case ChoiceControl.ucHouseHold:
                            dataGet.HOUSEHOLD_CODE = this.ucHouseHoldNumber1 != null ? this.ucHouseHoldNumber1.GetValue() : "";
                            break;
                        case ChoiceControl.ucHouseHoldRelative:
                            UCPlusInfoADO dataGetHouseHoldRelative = new UCPlusInfoADO();
                            dataGetHouseHoldRelative = this.ucHouseHoldNumber1 != null ? this.ucHouseHoldRelative1.GetValue() : new UCPlusInfoADO();
                            dataGet.HOUSEHOLD_RELATION_ID = dataGetHouseHoldRelative.HOUSEHOLD_RELATION_ID;
                            dataGet.HOUSEHOLD_RELATION_NAME = dataGetHouseHoldRelative.HOUSEHOLD_RELATION_NAME;
                            break;
                        case ChoiceControl.ucMilitaryRank:
                            dataGet.MILITARYRANK_ID = this.ucMilitaryRank1 != null ? this.ucMilitaryRank1.GetValue() : null;
                            break;
                        case ChoiceControl.ucMotherName:
                            dataGet.MOTHER_NAME = this.ucMother1 != null ? this.ucMother1.GetValue() : "";
                            break;
                        case ChoiceControl.ucNational:
                            UCPlusInfoADO dataGetNational = new UCPlusInfoADO();
                            dataGetNational = this.ucNational1 != null ? this.ucNational1.GetValue() : new UCPlusInfoADO();
                            dataGet.NATIONAL_CODE = dataGetNational.NATIONAL_CODE;
                            dataGet.NATIONAL_NAME = dataGetNational.NATIONAL_NAME;
                            dataGet.MPS_NATIONAL_CODE = dataGetNational.MPS_NATIONAL_CODE;
                            break;
                        case ChoiceControl.ucPhoneNumber:
                            dataGet.PHONE_NUMBER = this.ucPhone1 != null ? this.ucPhone1.GetValue() : "";
                            break;
                        case ChoiceControl.ucProgram:
                            dataGet.PROGRAM_ID = this.ucProgram1 != null ? this.ucProgram1.GetValue() : 0;
                            break;
                        case ChoiceControl.ucProvinceNow:
                            UCPlusInfoADO dataGetProvince = new UCPlusInfoADO();
                            dataGetProvince = this.ucProvinceNow1 != null ? this.ucProvinceNow1.GetValue() : new UCPlusInfoADO();
                            if (dataGetProvince != null)
                            {
                                dataGet.HT_PROVINCE_CODE = dataGetProvince.HT_PROVINCE_CODE;
                                dataGet.HT_PROVINCE_NAME = dataGetProvince.HT_PROVINCE_NAME;
                            }
                            break;
                        case ChoiceControl.ucProvinceOfBirth:
                            UCPlusInfoADO dataGetProvinceKS = new UCPlusInfoADO();
                            dataGetProvinceKS = this.ucProvinceOfBirth1 != null ? this.ucProvinceOfBirth1.GetValue() : new UCPlusInfoADO();
                            if (dataGetProvinceKS != null)
                            {
                                dataGet.PROVINCE_OfBIRTH_CODE = dataGetProvinceKS.PROVINCE_OfBIRTH_CODE;
                                dataGet.PROVINCE_OfBIRTH_NAME = dataGetProvinceKS.PROVINCE_OfBIRTH_NAME;
                            }
                            break;
                        case ChoiceControl.ucCommuneOfBirth:
                            UCPlusInfoADO dataGetCommuneOfBirth = new UCPlusInfoADO();
                            dataGetCommuneOfBirth = this.ucCommuneOfBirth1 != null ? this.ucCommuneOfBirth1.GetValue() : new UCPlusInfoADO();
                            if (dataGetCommuneOfBirth != null)
                            {
                                dataGet.COMMUNE_OfBIRTH_CODE = dataGetCommuneOfBirth.COMMUNE_OfBIRTH_CODE;
                                dataGet.COMMUNE_OfBIRTH_NAME = dataGetCommuneOfBirth.COMMUNE_OfBIRTH_NAME;
                            }
                            break;
                        case ChoiceControl.ucDistrictOfBirth:
                            UCPlusInfoADO dataGetDistrictOfBirth = new UCPlusInfoADO();
                            dataGetDistrictOfBirth = this.ucDistrictOfBirth1 != null ? this.ucDistrictOfBirth1.GetValue() : new UCPlusInfoADO();
                            if (dataGetDistrictOfBirth != null)
                            {
                                dataGet.DISTRICT_OfBIRTH_CODE = dataGetDistrictOfBirth.DISTRICT_OfBIRTH_CODE;
                                dataGet.DISTRICT_OfBIRTH_NAME = dataGetDistrictOfBirth.DISTRICT_OfBIRTH_NAME;
                            }
                            break;
                        case ChoiceControl.ucMaHoNgheo:
                            dataGet.HONGHEO_CODE = this.ucMaHoNgheo1 != null ? this.ucMaHoNgheo1.GetValue() : "";
                            break;
                        case ChoiceControl.ucHrmKskCode:
                            if (this._isShowControlHrmKskCode)
                                dataGet.HRM_KSK_CODE = this.ucHrmKskCode != null ? this.ucHrmKskCode.GetValue() : "";
                            break;
                        case ChoiceControl.ucTaxCode:
                            dataGet.TAX_CODE = this.ucTaxCode != null ? this.ucTaxCode.GetValue() : "";
                            break;
                        case ChoiceControl.UcCheckBoxCCCD:
                            dataGet.IsCheckBoxCCCD = this.ucCheckBoxCCCD != null ? this.ucCheckBoxCCCD.GetValue() : true;
                            break;
                        default:
                            break;

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                dataGet = null;
            }
            return dataGet;
        }

        private void SetValueInUCPlusInfo(List<UserControl> listControl, UCPlusInfoADO dataSet)
        {
            try
            {
                for (int i = 0; i < listControl.Count; i++)
                {
                    try
                    {
                        switch (listControl[i].Name)
                        {
                            #region --------- Kiem tra control và get data -------------

                            case ChoiceControl.ucAddress:
                                this.ucAddress1.SetValue(dataSet.HT_ADDRESS, dataSet.PATIENT_ID);
                                break;
                            case ChoiceControl.ucAddressOfBirth:
                                this.ucAddressOfBirth1.SetValue(dataSet.ADDRESS_OfBIRTH, dataSet.PATIENT_ID);
                                break;
                            case ChoiceControl.ucWorkPlace:
                                if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.CheDoHienThiNoiLamViecManHinhDangKyTiepDon == 1)
                                    this.ucWorkPlace1.SetValue(this.ucWorkPlace1, dataSet.workPlaceADO.WORK_PLACE);
                                else
                                {
                                    if (dataSet.workPlaceADO.WORK_PLACE_ID > 0)
                                    {
                                        var workPlace = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_WORK_PLACE>().FirstOrDefault(o => o.ID == dataSet.workPlaceADO.WORK_PLACE_ID);
                                        if (workPlace != null)
                                            this.ucWorkPlace1.SetValue(this.ucWorkPlace1, workPlace);
                                        else
                                            this.ucWorkPlace1.SetValue(this.ucWorkPlace1, null);
                                    }
                                    else
                                        this.ucWorkPlace1.SetValue(this.ucWorkPlace1, null);
                                }
                                break;
                            case ChoiceControl.ucBlood:
                                this.ucBlood1.SetValue(dataSet);
                                break;
                            case ChoiceControl.ucBloodRh:
                                this.ucBloodRh1.SetValue(dataSet);
                                break;
                            case ChoiceControl.ucCmndDate:
                                this.ucCMNDDate1.SetValue(dataSet.CMND_DATE);
                                break;
                            case ChoiceControl.ucCmndNumber:
                                if (!string.IsNullOrEmpty(dataSet.PASSPORT_NUMBER))
                                    this.ucCmndNumber1.SetValue(dataSet.PASSPORT_NUMBER);
                                else if (!string.IsNullOrEmpty(dataSet.CMND_NUMBER))
                                    this.ucCmndNumber1.SetValue(dataSet.CMND_NUMBER);
                                else if (!string.IsNullOrEmpty(dataSet.CCCD_NUMBER))
                                    this.ucCmndNumber1.SetValue(dataSet.CCCD_NUMBER);
                                else
                                    this.ucCmndNumber1.SetValue(null);
                                break;
                            case ChoiceControl.ucCmndPlace:
                                this.ucCMNDPlace1.SetValue(dataSet.CMND_PLACE);
                                break;
                            case ChoiceControl.ucProvinceNow:
                                this.ucProvinceNow1.SetValue(dataSet);
                                break;
                            case ChoiceControl.ucDistrictNow:
                                this.ucDistrictNow1.SetValue(dataSet);
                                break;
                            case ChoiceControl.ucCommuneNow:
                                this.ucCommuneNow1.SetValue(dataSet);
                                break;
                            case ChoiceControl.ucCommuneOfBirth:
                                this.ucCommuneOfBirth1.SetValue(dataSet);
                                break;
                            case ChoiceControl.ucDistrictOfBirth:
                                this.ucDistrictOfBirth1.SetValue(dataSet);
                                break;
                            case ChoiceControl.ucEmai:
                                this.ucEmail1.SetValue(dataSet.EMAIL);
                                break;
                            case ChoiceControl.ucPatientStoreCode:
                                this.ucPatientStoreCode.SetValue(dataSet.PATIENT_STORE_CODE);
                                break;
                            case ChoiceControl.ucEthnic:
                                this.ucEthnic1.SetValue(dataSet);
                                break;
                            case ChoiceControl.ucFatherName:
                                this.ucFather1.SetValue(dataSet.FATHER_NAME);
                                break;
                            case ChoiceControl.ucHohName:
                                this.ucHohName1.SetValue(dataSet.HOH_NAME);
                                break;
                            case ChoiceControl.ucHouseHold:
                                this.ucHouseHoldNumber1.SetValue(dataSet.HOUSEHOLD_CODE);
                                break;
                            case ChoiceControl.ucHouseHoldRelative:
                                this.ucHouseHoldRelative1.SetValue(dataSet);
                                break;
                            case ChoiceControl.ucMilitaryRank:
                                this.ucMilitaryRank1.SetValue(dataSet.MILITARYRANK_ID);
                                break;
                            case ChoiceControl.ucMotherName:
                                this.ucMother1.SetValue(dataSet.MOTHER_NAME);
                                break;
                            case ChoiceControl.ucNational:
                                this.ucNational1.SetValue(dataSet);
                                break;
                            case ChoiceControl.ucPhoneNumber:
                                this.ucPhone1.SetValue(dataSet.PHONE_NUMBER);
                                break;
                            case ChoiceControl.ucProgram:
                                this.ucProgram1.SetValue(dataSet.PATIENT_ID, dataSet.PROGRAM_ID);
                                break;
                            case ChoiceControl.ucProvinceOfBirth:
                                this.ucProvinceOfBirth1.SetValue(dataSet.PROVINCE_OfBIRTH_CODE);
                                break;
                            case ChoiceControl.ucMaHoNgheo:
                                this.ucMaHoNgheo1.SetValue(dataSet.PATIENT_ID, dataSet.HONGHEO_CODE);
                                break;
                            case ChoiceControl.ucHrmKskCode:
                                this.ucHrmKskCode.SetValue(dataSet.HRM_KSK_CODE);
                                break;
                            case ChoiceControl.ucTaxCode:
                                this.ucTaxCode.SetValue(dataSet.TAX_CODE);
                                break;
                            case ChoiceControl.ucExamHistory:
                                this.ucExamHistory.SetValue(dataSet.PATIENT_ID);
                                break;
                            default:
                                break;

                            #endregion
                        }
                    }
                    catch (Exception exx)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(exx);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Get - Set - Refresh Data outside UserControl

        public void SetValue(UCPlusInfoADO dataSet)
        {
            try
            {
                this.SetValueInUCPlusInfo(this.listControl, dataSet);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public UCPlusInfoADO GetValue()
        {
            try
            {
                return this.GetValueInUCPlusInfo(this.listControl);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        public void RefreshUserControl()
        {
            try
            {
                WaitingManager.Show();
                UCPlusInfoADO dataRefresh = new UCPlusInfoADO();
                dataRefresh.workPlaceADO = new WorkPlaceADO();
                this.SetValueInUCPlusInfo(this.listControl, dataRefresh);
                this.ResetRequiredField();
                Inventec.Common.Logging.LogSystem.Info("Tiep don: PlusInfo/RefreshUserControl :\n" + dataRefresh);
                if (this.ucEthnic1 != null)
                    this.ucEthnic1.LoadEthnicBase();
                if (this.ucNational1 != null)
                    this.ucNational1.LoadNationalBase();
                if (this.ucDistrictNow1 != null)
                    this.ucDistrictNow1.RefreshUserControl(true);
                if (this.ucCommuneNow1 != null)
                    this.ucCommuneNow1.RefreshUserControl(true);
                if (this.ucCmndNumber1 != null)
                    this.ucCmndNumber1.RefreshUserControl(true);
                if (this.ucDistrictOfBirth1 != null)
                    this.ucDistrictOfBirth1.RefreshUserControl(true);
                if (this.ucCommuneOfBirth1 != null)
                    this.ucCommuneOfBirth1.RefreshUserControl(true);
                if (this.ucExamHistory != null)
                    this.ucExamHistory.RefreshUserControl();
                if (this.ucHrmKskCode != null)
                    this.ucHrmKskCode.RefreshData();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

    }
}
