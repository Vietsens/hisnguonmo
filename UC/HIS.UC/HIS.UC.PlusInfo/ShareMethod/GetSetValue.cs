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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraLayout;
using HIS.UC.PlusInfo.ADO;
using HIS.UC.PlusInfo;
using HIS.UC.PlusInfo.Config;
using HIS.Desktop.LocalStorage.BackendData;

namespace HIS.UC.PlusInfo.ShareMethod
{
    public class GetSetValue : IShareMethodData
    {
        UserControl control { get; set; }
        public Design.UCWorkPlace ucWorkPlace { get; set; }

        public UCPlusInfoADO GetValue(LayoutControl layoutGroup)
        {
            UCPlusInfo uc = new UCPlusInfo();
            UCPlusInfoADO dataGet = new UCPlusInfoADO();
            try
            {
                for ( int i = 0; i< layoutGroup.Controls.Count; i++)
                {
                    switch (layoutGroup.Controls[i].Name)
                    {
                        #region --------- Kiem tra control và get data -------------

                        case ChoiceControl.ucAddress:
                            dataGet.HT_ADDRESS =  uc.ucAddress1.GetValue();
                            break;
                        case ChoiceControl.ucWorkPlace:
                            if (this.ucWorkPlace != null)
                            {
                                dataGet.workPlace = this.ucWorkPlace.GetValue(this.ucWorkPlace);
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
                            dataGetBlood = uc.ucBlood1.GetValue();
                            if (dataGetBlood != null)
                            {
                                dataGet.BLOOD_ABO_CODE = dataGetBlood.BLOOD_ABO_CODE;
                                dataGet.BLOOD_ABO_ID = dataGetBlood.BLOOD_ABO_ID;
                            }
                            break;
                        case ChoiceControl.ucBloodRh:
                            UCPlusInfoADO dataGetBloodRh = new UCPlusInfoADO();
                            dataGetBloodRh = uc.ucBloodRh1.GetValue();
                            if (dataGetBloodRh != null)
                            {
                                dataGet.BLOOD_RH_CODE = dataGetBloodRh.BLOOD_RH_CODE;
                                dataGet.BLOOD_RH_ID = dataGetBloodRh.BLOOD_RH_ID;
                            }
                            break;
                        case ChoiceControl.ucCmndDate:
                            dataGet.CMND_DATE = uc.ucCMNDDate1.GetValue();
                            break;
                        case ChoiceControl.ucCmndNumber:
                            dataGet.CMND_NUMBER = uc.ucCmndNumber1.GetValue();
                            break;
                        case ChoiceControl.ucCmndPlace:
                            dataGet.CMND_PLACE = uc.ucCMNDPlace1.GetValue();
                            break;
                        case ChoiceControl.ucCommuneNow:
                            UCPlusInfoADO dataGetCommune = new UCPlusInfoADO();
                            dataGetCommune = uc.ucCommuneNow1.GetValue();
                            if (dataGetCommune != null)
                            {
                                dataGet.HT_COMMUNE_CODE = dataGetCommune.HT_COMMUNE_CODE;
                                dataGet.HT_COMMUNE_NAME = dataGetCommune.HT_COMMUNE_NAME;
                            }
                            break;
                        case ChoiceControl.ucDistrictNow:
                            UCPlusInfoADO dataGetDistrict = new UCPlusInfoADO();
                            dataGetDistrict = uc.ucDistrictNow1.GetValue();
                            if (dataGetDistrict != null)
                            {
                                dataGet.HT_DISTRICT_CODE = dataGetDistrict.HT_DISTRICT_CODE;
                                dataGet.HT_DISTRICT_NAME = dataGetDistrict.HT_DISTRICT_NAME;
                            }
                            break;
                        case ChoiceControl.ucEmai:
                            dataGet.EMAIL = uc.ucEmail1.GetValue();
                            break;
                        case ChoiceControl.ucEthnic:
                            UCPlusInfoADO dataGetEthnic= new UCPlusInfoADO();
                            dataGetEthnic = uc.ucEthnic1.GetValue();
                            dataGet.ETHNIC_NAME = dataGetEthnic.ETHNIC_NAME;
                            dataGet.ETHNIC_CODE = dataGetEthnic.ETHNIC_CODE;
                            break;
                        case ChoiceControl.ucFatherName:
                            dataGet.FATHER_NAME = uc.ucFather1.GetValue();
                            break;
                        case ChoiceControl.ucHouseHold:
                            dataGet.HOUSEHOLD_CODE = ((Design.UCHouseHold)control).GetValue();
                            break;
                        case ChoiceControl.ucHouseHoldRelative:
                            UCPlusInfoADO dataGetHouseHoldRelative = new UCPlusInfoADO();
                            dataGetHouseHoldRelative = uc.ucHouseHoldRelative1.GetValue();
                            dataGet.HOUSEHOLD_RELATION_ID = dataGetHouseHoldRelative.HOUSEHOLD_RELATION_ID;
                            dataGet.HOUSEHOLD_RELATION_NAME = dataGetHouseHoldRelative.HOUSEHOLD_RELATION_NAME;
                            break;
                        case ChoiceControl.ucMilitaryRank:
                            dataGet.MILITARYRANK_ID = uc.ucMilitaryRank1.GetValue();
                            break;
                        case ChoiceControl.ucMotherName:
                            dataGet.MOTHER_NAME = uc.ucMother1.GetValue();
                            break;
                        case ChoiceControl.ucNational:
                            UCPlusInfoADO dataGetNational = new UCPlusInfoADO();
                            dataGetNational = uc.ucNational1.GetValue();
                            dataGet.NATIONAL_CODE = dataGetNational.NATIONAL_CODE;
                            dataGet.NATIONAL_NAME = dataGetNational.NATIONAL_NAME;
                            break;
                        case ChoiceControl.ucPhoneNumber:
                            dataGet.PHONE_NUMBER = uc.ucPhone1.GetValue();
                            break;
                        case ChoiceControl.ucProgram:
                            dataGet.PROGRAM_ID = uc.ucProgram1.GetValue();
                            break;
                        case ChoiceControl.ucProvinceNow:
                            UCPlusInfoADO dataGetProvince = new UCPlusInfoADO();
                            dataGetProvince = uc.ucProvinceNow1.GetValue();
                            if (dataGetProvince != null)
                            {
                                dataGet.HT_PROVINCE_CODE = dataGetProvince.HT_PROVINCE_CODE;
                                dataGet.HT_PROVINCE_NAME = dataGetProvince.HT_PROVINCE_NAME;
                            }
                            break;
                        case ChoiceControl.ucProvinceOfBirth:
                            UCPlusInfoADO dataGetProvinceKS = new UCPlusInfoADO();
                            dataGetProvinceKS = uc.ucProvinceOfBirth1.GetValue();
                            if (dataGetProvinceKS != null)
                            {
                                dataGet.PROVINCE_OfBIRTH_CODE = dataGetProvinceKS.PROVINCE_OfBIRTH_CODE;
                                dataGet.PROVINCE_OfBIRTH_NAME = dataGetProvinceKS.PROVINCE_OfBIRTH_NAME;
                            }
                            break;
                        case ChoiceControl.ucMaHoNgheo:
                            dataGet.HONGHEO_CODE = uc.ucMaHoNgheo1.GetValue();
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

        public void SetValue(LayoutControl layoutGroup, UCPlusInfoADO dataSet)
        {
            try
            {
                for (int i = 0; i < layoutGroup.Controls.Count; i++ )
                {
                    switch (layoutGroup.Controls[i].Name)
                    {
                        #region --------- Kiem tra control và get data -------------

                        case ChoiceControl.ucAddress:
                            ((Design.UCAddressNow)control).SetValue(dataSet.HT_ADDRESS);
                            break;
                        case ChoiceControl.ucWorkPlace:
                            if (this.ucWorkPlace != null)
                            {
                                if (UCPlusInfo_Config.CheDoHienThiNoiLamViecManHinhDangKyTiepDon == 1)
                                    this.ucWorkPlace.SetValue(this.ucWorkPlace, dataSet.workPlaceADO.WORK_PLACE);
                                else
                                {
                                    if (dataSet.workPlaceADO.WORK_PLACE_ID > 0)
                                    {
                                        var workPlace = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_WORK_PLACE>().FirstOrDefault(o => o.ID == dataSet.workPlaceADO.WORK_PLACE_ID);
                                        if (workPlace != null)
                                            this.ucWorkPlace.SetValue(this.ucWorkPlace, workPlace);
                                        else
                                            this.ucWorkPlace.SetValue(this.ucWorkPlace, null);
                                    }
                                }
                            }
                            break;
                        case ChoiceControl.ucBlood:
                            ((Design.UCBlood)control).SetValue(dataSet);
                            break;
                        case ChoiceControl.ucBloodRh:
                            ((Design.UCBloodRh)control).SetValue(dataSet);
                            break;
                        case ChoiceControl.ucCmndDate:
                            ((Design.UCCMNDDate)control).SetValue(dataSet.CMND_DATE);
                            break;
                        case ChoiceControl.ucCmndNumber:
                            ((Design.UCCMND)control).SetValue(dataSet.CMND_NUMBER);
                            break;
                        case ChoiceControl.ucCmndPlace:
                            ((Design.UCCMNDPlace)control).SetValue(dataSet.CMND_PLACE);
                            break;
                        case ChoiceControl.ucCommuneNow:
                            ((Design.UCCommuneNow)control).SetValue(dataSet);
                            break;
                        case ChoiceControl.ucDistrictNow:
                            ((Design.UCDistrictNow)control).SetValue(dataSet);
                            break;
                        case ChoiceControl.ucEmai:
                            ((Design.UCEmail)control).SetValue(dataSet.EMAIL);
                            break;
                        case ChoiceControl.ucEthnic:
                            ((Design.UCEthnic)control).SetValue(dataSet);
                            break;
                        case ChoiceControl.ucFatherName:
                            ((Design.UCFatherName)control).SetValue(dataSet.FATHER_NAME);
                            break;
                        case ChoiceControl.ucHouseHold:
                            ((Design.UCHouseHold)control).SetValue(dataSet.HOUSEHOLD_CODE);
                            break;
                        case ChoiceControl.ucHouseHoldRelative:
                            ((Design.UCHouseHoldRelative)control).SetValue(dataSet);
                            break;
                        case ChoiceControl.ucMilitaryRank:
                            ((Design.UCMilitaryRank)control).SetValue(dataSet.MILITARYRANK_ID);
                            break;
                        case ChoiceControl.ucMotherName:
                            ((Design.UCMotherName)control).SetValue(dataSet.MOTHER_NAME);
                            break;
                        case ChoiceControl.ucNational:
                            ((Design.UCNational)control).SetValue(dataSet);
                            break;
                        case ChoiceControl.ucPhoneNumber:
                            ((Design.UCPhoneNumber)control).SetValue(dataSet.PHONE_NUMBER);
                            break;
                        case ChoiceControl.ucProgram:
                            ((Design.UCProgram)control).SetValue(dataSet.PATIENT_ID);
                            break;
                        case ChoiceControl.ucProvinceNow:
                            ((Design.UCProvinceNow)control).SetValue(dataSet);
                            break;
                        case ChoiceControl.ucProvinceOfBirth:
                            ((Design.UCProvinceOfBirth)control).SetValue(dataSet.PROVINCE_OfBIRTH_CODE);
                            break;
                        case ChoiceControl.ucMaHoNgheo:
                            ((Design.UCMaHoNgheo)control).SetValue(dataSet.PATIENT_ID,dataSet.HONGHEO_CODE);
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
            }
        }
    }
}
