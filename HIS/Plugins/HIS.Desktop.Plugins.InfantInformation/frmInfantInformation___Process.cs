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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Common.RichEditor.Base;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.InfantInformation
{
    public partial class frmInfantInformation : HIS.Desktop.Utility.FormBase
    {
        private void UpdateDTOFromDataForm(ref HisBabySDO currentDTO)
        {
            try
            {
                //string gio = txtInfantBorntime.Text;
                //string hous = "";
                //if (string.IsNullOrEmpty(gio))
                //    hous = "000000";
                //else
                //    hous = gio.Trim().Split(':')[0] + gio.Split(':')[1] + gio.Split(':')[2];
                //if (dtdInfantdate.EditValue != null)
                //{
                //    string Infantdate__Date = dtdInfantdate.DateTime.ToString("yyyyMMdd");
                //    long Infantdate__Full = Inventec.Common.TypeConvert.Parse.ToInt64(Infantdate__Date + "" + hous);

                //    currentDTO.BORN_TIME = Infantdate__Full;
                //}
                //else
                //{ 

                //}
                currentDTO.DepartmentId = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.Module.RoomId).DepartmentId;
                long hour = 0;
                if (!string.IsNullOrEmpty(txtInfantBorntime.Text))
                {
                    hour = Inventec.Common.TypeConvert.Parse.ToInt64(txtInfantBorntime.Text.Replace(":", ""));
                }

                long date = 0;
                if (dtdInfantdate.EditValue != null)
                {
                    date = Inventec.Common.TypeConvert.Parse.ToInt64(dtdInfantdate.DateTime.ToString("yyyyMMdd"));
                }
                long datecreate = 0;
                
                if (date > 0)
                {
                    if (Convert.ToInt32(cboBirthPlaceType.EditValue) == 1)
                    {
                        long intime = this.treatment.IN_TIME / 1000000;
                        if (date < intime)
                        {
                            dxErrorProvider1.SetError(dtdInfantdate, "Thời gian sinh phải lớn hơn thời gian vào viện", ErrorType.Warning);
                        }
                        else if (date == intime)
                        {
                            var treatmentHour = Inventec.Common.TypeConvert.Parse.ToInt64(this.treatment.IN_TIME.ToString().Substring(8, 6));
                            if (hour < treatmentHour)
                            {
                                dxErrorProvider1.SetError(txtInfantBorntime, "Thời gian sinh phải lớn hơn thời gian vào viện", ErrorType.Warning);
                            }

                        }
                    }
                    
                    currentDTO.BornTime = date * 1000000 + hour;
                }
                else
                {

                    currentDTO.BornTime = null;
                }
                if (dtDeathDate.EditValue != null)
                {
                    currentDTO.DeathDate = Inventec.Common.TypeConvert.Parse.ToInt64(dtDeathDate.DateTime.ToString("yyyyMMdd000000"));
                }
                if (dteIssue.EditValue != null)
                {
                    currentDTO.IssueDateBaby = Inventec.Common.TypeConvert.Parse.ToInt64(dteIssue.DateTime.ToString("yyyyMMdd000000"));
                }
                currentDTO.TreatmentId = this.treatmentId;

                if (!String.IsNullOrEmpty(cboHisBirthSertBook.Text))
                {
                    currentDTO.BirthCertBookID = Inventec.Common.TypeConvert.Parse.ToInt64(cboHisBirthSertBook.EditValue.ToString());  //Sổ chứng sinh

                }
                else
                {
                    currentDTO.BirthCertBookID = null;
                }

                if (!String.IsNullOrEmpty(cboInfantTybe.Text))
                {
                    currentDTO.BornTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboInfantTybe.EditValue.ToString());  //cach sinh

                }
                else
                {
                    currentDTO.BornTypeId = null;

                }

                if (!String.IsNullOrEmpty(cboInfantPosition.Text))
                {
                    currentDTO.BornPositionId = Inventec.Common.TypeConvert.Parse.ToInt64(cboInfantPosition.EditValue.ToString());//ngoi thai

                }
                else
                {
                    currentDTO.BornPositionId = null;
                }

                if (!String.IsNullOrEmpty(cboInfantResult.Text))
                {
                    currentDTO.BornResultId = Inventec.Common.TypeConvert.Parse.ToInt64(cboInfantResult.EditValue.ToString());//tình trang
                }
                else
                {
                    currentDTO.BornResultId = null;
                }

                if (!String.IsNullOrEmpty(cboInfantGendercode.Text))
                {
                    currentDTO.GenderId = Inventec.Common.TypeConvert.Parse.ToInt64(cboInfantGendercode.EditValue.ToString());//gioi tinh
                }
                else
                {
                    currentDTO.GenderId = null;
                }

                if (spnInfantMonth.EditValue != null)
                {
                    currentDTO.BabyOrder = Inventec.Common.TypeConvert.Parse.ToInt64(spnInfantMonth.Value.ToString());//con thu
                }
                else
                {
                    currentDTO.BabyOrder = null;
                }

                if (spnChildLive.EditValue != null && spnChildLive.Value.ToString() != "0")
                {
                    currentDTO.CurrentAlive = Inventec.Common.TypeConvert.Parse.ToInt64(spnChildLive.Value.ToString());//số con hiện sống
                }
                else
                {
                    currentDTO.CurrentAlive = null;
                }

                if (!String.IsNullOrEmpty(txtInfantMonth.Text))
                {
                    currentDTO.MonthCount = Inventec.Common.TypeConvert.Parse.ToInt64(txtInfantMonth.Text);//so thang
                }
                else
                {
                    currentDTO.MonthCount = null;
                }

                if (!String.IsNullOrEmpty(txtInfantWeek.Text))
                {
                    currentDTO.WeekCount = Inventec.Common.TypeConvert.Parse.ToInt64(txtInfantWeek.Text);// so tuan
                }
                else
                {
                    currentDTO.WeekCount = null;
                }

                currentDTO.BabyName = txtInfantName.Text;

                if (txtUserGCS.Text != null && txtUserGCS.Text != "")
                {
                    currentDTO.IssuerLoginname = txtUserGCS.Text;
                }
                else
                {
                    currentDTO.IssuerLoginname = null;
                }
                if (cboUserGCS.EditValue != null && cboUserGCS.EditValue.ToString() != "")
                {
                    currentDTO.IssuerUsername = cboUserGCS.Text;
                }
                else
                {
                    currentDTO.IssuerUsername = null;
                }

                if (txtDirectorLoginname.Text != null && txtDirectorLoginname.Text != "")
                {
                    currentDTO.DirectorLoginname = txtDirectorLoginname.Text.Trim();
                }
                else
                {
                    currentDTO.DirectorLoginname = null;
                }
                if (cboDirectorUsername.EditValue != null && cboDirectorUsername.EditValue.ToString() != "")
                {
                    currentDTO.DirectorUsername = (cboDirectorUsername.Properties.DataSource as List<V_HIS_EMPLOYEE>).FirstOrDefault(o=>o.LOGINNAME ==  cboDirectorUsername.EditValue.ToString()).TDL_USERNAME;
                }
                else
                {
                    currentDTO.DirectorUsername = null;
                }


                currentDTO.IdentityNumber = txtCMT.Text;

                currentDTO.IssuePlace = txtNoicap.Text;


                if (txtNgaycap.EditValue != null)
                {
                    datecreate = Inventec.Common.TypeConvert.Parse.ToInt64(txtNgaycap.DateTime.ToString("yyyyMMdd000000"));
                    currentDTO.IssueDate = datecreate;
                }
                else
                {
                    currentDTO.IssueDate = null;
                }

                if (spnInfantHeight.EditValue != null)
                {
                    currentDTO.Height = spnInfantHeight.Value;//chiều cao
                }
                else
                {
                    currentDTO.Height = null;
                }
                if (spnInfantWeight.EditValue != null)
                {
                    currentDTO.Weight = spnInfantWeight.Value; //can nang
                }
                else
                {
                    currentDTO.Weight = null;
                }
                if (spnInfanthead.EditValue != null)
                {
                    currentDTO.Head = spnInfanthead.Value;//vong dau
                }
                else
                {
                    currentDTO.Head = null;
                }
                currentDTO.FatherName = txtFather.Text;//nguoi do de
                List<string> Midwife = new List<string>();
                if (!String.IsNullOrEmpty(txtInfantMidwife1.Text))
                {
                    Midwife.Add(txtInfantMidwife1.Text);
                }
                if (!String.IsNullOrEmpty(txtInfantMidwife2.Text))
                {
                    Midwife.Add(txtInfantMidwife2.Text);
                }
                if (!String.IsNullOrEmpty(txtInfantMidwife3.Text))
                {
                    Midwife.Add(txtInfantMidwife3.Text);
                }
                if (Midwife.Count > 0)
                {
                    currentDTO.Midwife = string.Join(";", Midwife);//nguoi do de
                }
                else
                {
                    currentDTO.Midwife = null;
                }

                if (cboEthnic.EditValue != null)
                {
                    currentDTO.EthnicCode = cboEthnic.EditValue.ToString();
                    var ethnic = BackendDataWorker.Get<SDA_ETHNIC>().FirstOrDefault(o => o.ETHNIC_CODE == cboEthnic.EditValue.ToString());
                    if (ethnic != null)
                        currentDTO.EthnicName = ethnic.ETHNIC_NAME;
                }
                else
                {
                    currentDTO.EthnicCode = null;
                    currentDTO.EthnicName = null;
                }
                currentDTO.IsSurgery = chkIsSurgery.Checked ? (short?)1 : null;
                currentDTO.HeinCardNumberTmp = txtHeinCardTmp.Text.ToUpper();

                currentDTO.MotherAddress = txtAddress.Text.Trim();
                currentDTO.Note = txtNote.Text.Trim();

                if (cboProvinceName.EditValue != null)
                {
                    currentDTO.MotherProvinceCode = cboProvinceName.EditValue.ToString();
                    currentDTO.MotherProvinceName = cboProvinceName.Text;
                }
                else
                {
                    currentDTO.MotherProvinceCode = null;
                    currentDTO.MotherProvinceName = null;
                }
                if (cboDistrictName.EditValue != null)
                {
                    currentDTO.MotherDistrictCode = cboDistrictName.EditValue.ToString();
                    currentDTO.MotherDistrictName = cboDistrictName.Text;
                }
                else
                {
                    currentDTO.MotherDistrictCode = null;
                    currentDTO.MotherDistrictName = null;
                }
                if (cboCommuneName.EditValue != null)
                {
                    currentDTO.MotherCommuneCode = cboCommuneName.EditValue.ToString();
                    currentDTO.MotherCommuneName = cboCommuneName.Text;
                }
                else
                {
                    currentDTO.MotherCommuneCode = null;
                    currentDTO.MotherCommuneName = null;
                }
                //Địa chỉ hiện tại
                currentDTO.HtAddress = txtHTAddress.Text;

                if (cboHTProvinceName.EditValue != null)
                {
                    var provinceHT = listProvince.FirstOrDefault(o => o.PROVINCE_CODE == cboHTProvinceName.EditValue.ToString());
                    if (provinceHT != null)
                    {
                        currentDTO.HtProvinceName = provinceHT.PROVINCE_NAME;
                        currentDTO.HtProvinceCode = provinceHT.PROVINCE_CODE;

                    }
                }
                else
                {
                    currentDTO.HtProvinceName = null;
                    currentDTO.HtProvinceCode = null;
                }

                if (cboHTDistrictName.EditValue != null)
                {
                    var districtHT = listDistrict.FirstOrDefault(o => o.DISTRICT_CODE == cboHTDistrictName.EditValue.ToString());
                    if (districtHT != null)
                    {
                        currentDTO.HtDistrictName = districtHT.DISTRICT_NAME;
                        currentDTO.HtDistrictCode = districtHT.DISTRICT_CODE;
                    }
                }
                else
                {
                    currentDTO.HtDistrictName = null;
                    currentDTO.HtDistrictCode = null;
                }

                if (cboHTCommuneName.EditValue != null)
                {
                    var communeHT = listCommune.FirstOrDefault(o => o.COMMUNE_CODE == cboHTCommuneName.EditValue.ToString());
                    if (communeHT != null)
                    {
                        currentDTO.HtCommuneName = communeHT.COMMUNE_NAME;
                        currentDTO.HtCommuneCode = communeHT.COMMUNE_CODE;
                    }
                }
                else
                {
                    currentDTO.HtCommuneName = null;
                    currentDTO.HtCommuneCode = null;
                }
                if (txtNumberChildrenBirth.EditValue != null)
                    currentDTO.NumberChildrenBirth = (long)txtNumberChildrenBirth.Value;
                else
                    currentDTO.NumberChildrenBirth = null;

                if (!String.IsNullOrEmpty(txtNumberOfPregnancies.Text))
                    currentDTO.NumberOfPregnancies = Convert.ToInt64(txtNumberOfPregnancies.Text);
                else
                    currentDTO.NumberOfPregnancies = null;
                if (cboBirthPlaceType.EditValue != null)
                {
                    currentDTO.BirthplaceType = Inventec.Common.TypeConvert.Parse.ToInt16(cboBirthPlaceType.EditValue.ToString());
                }
                else
                    currentDTO.BirthplaceType = null;
                if (cboBirthHospital.EditValue != null)
                {
                    currentDTO.BirthHospitalCode = cboBirthHospital.EditValue.ToString();
                    currentDTO.BirthHospitalName = cboBirthHospital.Text;
                }
                else
                {
                    currentDTO.BirthHospitalCode = null;
                    currentDTO.BirthHospitalName = null;
                }
                if (cboProvinceNameHospital.EditValue != null)
                {
                    currentDTO.BirthProvinceCode = cboProvinceNameHospital.EditValue.ToString();
                    currentDTO.BirthProvinceName = cboProvinceNameHospital.Text;
                }
                else
                {
                    currentDTO.BirthProvinceCode = null;
                    currentDTO.BirthProvinceName = null;
                }
                if (cboDistrictNameHospital.EditValue != null)
                {
                    currentDTO.BirthDistrictCode = cboDistrictNameHospital.EditValue.ToString();
                    currentDTO.BirthDistrictName = cboDistrictNameHospital.Text;
                }
                else
                {
                    currentDTO.BirthDistrictCode = null;
                    currentDTO.BirthDistrictName = null;
                }
                if (cboCommuneNameHospital.EditValue != null)
                {
                    currentDTO.BirthCommuneCode = cboCommuneNameHospital.EditValue.ToString();
                    currentDTO.BirthCommuneName = cboCommuneNameHospital.Text;
                }
                else
                {
                    currentDTO.BirthCommuneCode = null;
                    currentDTO.BirthCommuneName = null;
                }
                currentDTO.Birthplace = txtBirthPlace.Text;
                currentDTO.MethodStyle = txtMethodStyle.Text;
                //checkbox
                currentDTO.IsDifficultBirth = chkIsDifficultBirth.Checked;
                currentDTO.IsHaemorrhage = ChkIsHaemorrhage.Checked;
                currentDTO.IsUterineRupture = chkIsUterineRupture.Checked;
                currentDTO.IsHaemorrhage = ChkIsHaemorrhage.Checked;
                currentDTO.IsPuerperal = chkIsPuerperal.Checked;
                currentDTO.IsBacterialContamination = chkIsBacterialContamination.Checked;
                currentDTO.IsTetanus = chkIsTetanus.Checked;
                currentDTO.IsMotherDeath = chkIsMotherDeath.Checked;
                currentDTO.IsFetalDeath22Weeks = chkIsFetalDeath22Weeks.Checked;
                currentDTO.IsInjectK1 = chkIsInjeckK1.Checked;
                currentDTO.IsInjectB = chkIsInjeckB.Checked;
                if (chkPostpartumCare2.Checked)
                    currentDTO.PostpartumCare = 2;
                if (chkPostpartumCare6.Checked)
                    currentDTO.PostpartumCare = 6;
                if (!chkPostpartumCare6.Checked && !chkPostpartumCare2.Checked)
                    currentDTO.PostpartumCare = null;

                currentDTO.BabyInfoForTreatment = new BabyInfoForTreatmentSDO();

                currentDTO.BabyInfoForTreatment.NumberOfFullTermBirth = !string.IsNullOrEmpty(txtNumOfFullTermBirth.Text) ? (long?)Convert.ToInt64(txtNumOfFullTermBirth.Text) : null;


                currentDTO.BabyInfoForTreatment.NumberOfPrematureBirth = !string.IsNullOrEmpty(txtNumOfPrematureBirth.Text) ? (long?)Convert.ToInt64(txtNumOfPrematureBirth.Text) : null;

                currentDTO.BabyInfoForTreatment.NumberOfMiscarriage = !string.IsNullOrEmpty(txtNumOfMiscarriage.Text) ? (long?)Convert.ToInt64(txtNumOfMiscarriage.Text) : null;

                currentDTO.BabyInfoForTreatment.NumberOfTests = cboNumOfTest.EditValue != null ? (short?)Convert.ToInt16(cboNumOfTest.EditValue) : null;

                currentDTO.BabyInfoForTreatment.TestHiv = cboTestHiv.EditValue != null ? (short?)Convert.ToInt16(cboTestHiv.EditValue) : null;

                currentDTO.BabyInfoForTreatment.TestSyphilis = cboTestSyphilis.EditValue != null ? (short?)Convert.ToInt16(cboTestSyphilis.EditValue) : null;

                currentDTO.BabyInfoForTreatment.TestHepatitisB = cboTestHepatitisB.EditValue != null ? (short?)Convert.ToInt16(cboTestHepatitisB.EditValue) : null;

                currentDTO.BabyInfoForTreatment.IsTestBloodSugar = chkIsTestBloodSugar.Checked ? (short?)1 : null;
                currentDTO.BabyInfoForTreatment.IsEarlyNewbornCare = chkIsEarlyNewBornCare.Checked ? (short?)1 : null;

                currentDTO.BabyInfoForTreatment.NewbornCareAtHome = cboNewBornCareAtHome.EditValue != null ? (short?)Convert.ToInt16(cboNewBornCareAtHome.EditValue) : null;
                if (txtNumberOfBirth.EditValue != null)
                    currentDTO.BabyInfoForTreatment.NumberOfBirth = (long)(txtNumberOfBirth.Value);
                else
                    currentDTO.BabyInfoForTreatment.NumberOfBirth = null;
            }
            catch (Exception ex)
            {
                currentDTO = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}
