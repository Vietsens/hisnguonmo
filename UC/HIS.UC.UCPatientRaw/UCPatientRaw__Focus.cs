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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.UC.UCPatientRaw.ClassUCPatientRaw;
using DevExpress.Utils.Menu;
using HIS.UC.UCPatientRaw.ADO;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.DelegateRegister;
using MOS.SDO;
using Inventec.Desktop.Common.Message;

namespace HIS.UC.UCPatientRaw
{
    public partial class UCPatientRaw : HIS.Desktop.Utility.UserControlBase
    {

        #region Focus UserControl

        public void FocusUserControl()
        {
            try
            {
                this.txtPatientCode.Focus();
                this.txtPatientCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void Set3WBhytCode(string code)
		{
			try
			{
                BhytCode = code;
                var check = dataWhiteList.FirstOrDefault(o => o.BHYT_WHITELIST_CODE == BhytCode);
                if(check!=null)
				{
                    BhytWhiteListtId = check.ID;
				}
				else
				{
                    BhytWhiteListtId = 0;

                }                    
                Inventec.Common.Logging.LogSystem.Info("3 ký tự đầu tiên của mã BHYT : "+BhytCode + "__ID: " + BhytWhiteListtId);
                if(lciPatientClassifyNew.Visible)
                    ChangePatientClassify(true);
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
		}
        public void GetCardSDO(HisCardSDO sdo)
        {
            try
            {
                if (sdo == null)
                    return;
                WaitingManager.Show();
                this.hrmEmployeeCode = "";
                this.dataResult = new DataResultADO();
                isAlertTreatmentEndInDay = false;
                btnCodeFind.Text = ResourceMessage.typeCodeFind__SoThe;
                this.typeCodeFind = ResourceMessage.typeCodeFind__SoThe;
                txtPatientCode.Text = sdo.CardCode;
                SearchPatientByCodeOrQrCode(txtPatientCode.Text.Trim());
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FocusToPatientName()
        {
            try
            {
                this.txtPatientName.Focus();
                this.txtPatientName.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FocusToPatientType()
        {
            try
            {
                this.txtPatientTypeCode.Focus();
                this.txtPatientTypeCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FocusNextUserControl(DelegateFocusNextUserControl _dlgFocusNextUserControl)
        {
            try
            {
                if (_dlgFocusNextUserControl != null)
                {
                    this.dlgFocusNextUserControl = _dlgFocusNextUserControl;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FocusToUCRelativeWhenPatientIsChild(DelegateFocusNextUserControl _dlgFocusToUCRelativeWhenPatientIsChild)
        {
            try
            {
                if (_dlgFocusToUCRelativeWhenPatientIsChild != null)
                {
                    this.dlgFocusToUCRelativeWhenPatientIsChild = _dlgFocusToUCRelativeWhenPatientIsChild;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetDelegateFocusNextUserControlWhenPatientIsChild(DelegateSetFocusWhenPatientIsChild _dlgSetFocusWhenPatientIsChild)
        {
            try
            {
                if (_dlgSetFocusWhenPatientIsChild != null)
                {
                    this.dlgSetFocusWhenPatientIsChild = _dlgSetFocusWhenPatientIsChild;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetDelegateShowControlHrmKskCode(DelegateShowControlHrmKskCode _dlgShowControlHrmKskCode)
        {
            try
            {
                if (_dlgShowControlHrmKskCode != null)
                {
                    this.dlgShowControlHrmKskCode = _dlgShowControlHrmKskCode;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetDelegateShowControlHrmKskCodeNotValid(DelegateShowControlHrmKskCodeNotValid _dlgShowControlHrmKskCodeNotValid)
        {
            try
            {
                if (_dlgShowControlHrmKskCodeNotValid != null)
                {
                    this.dlgShowControlHrmKskCodeNotValid = _dlgShowControlHrmKskCodeNotValid;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetDelegateShowControlGuaranteeLoginname(DelegateShowControlGuaranteeLoginname _dlg)
        {
            try
            {
                if (_dlg != null)
                {
                    this.dlgShowControlGuaranteeLoginname = _dlg;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetDelegateSendPatientName(DelegateSendPatientName dlg)
        {
            try
            {
                if (dlg != null)
                {
                    this.dlgSendPatientName = dlg;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void SetDelegateSendPatientSDO(DelegateSendPatientSDO dlg)
        {
            try
            {
                if (dlg != null)
                {
                    this.dlgSendPatientSdo = dlg;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Refresh UserControl

        public void RefreshUserControl()
        {
            try
            {
                DisableControlOldPatientInformationOption(true);
                popupControlContainer1.HidePopup();
                this.dataHeinCardFromQrCccd = null;
                this.dlgSendPatientSdo(null);
                this.lstSend = null;
                this.ResultDataADO = new Desktop.Plugins.Library.CheckHeinGOV.ResultDataADO();
                this._UCPatientRawADO = new UCPatientRawADO();
                this.qrCodeBHYTHeinCardData = new Inventec.Common.QrCodeBHYT.HeinCardData();
                this.txtPatientCode.Text = "";
                this.txtPatientName.Text = "";
                this.txtPatientDob.Text = "";
                this.txtPatientDob.ErrorText = "";
                this.dtPatientDob.EditValue = null;
                this.txtAge.Text = "";
                this.cboAge.EditValue = null;
                this.patientTD3 = null;
                this.cboEthnic.EditValue = null;
                this.txtEthnicCode.Text = "";
                this.currentMilitaryRank = null;
                this.currentPatientClassify = null;
                this.currentPosition = null;
                this.currentWorkPlace = null;
                this.txtPosition.Text = "";
                this.txtMilitaryRank.Text = "";
                this.txtWorkPlace.Text = "";
                this.txtPatientClassify.Text = "";
                this.cboGender.EditValue = null;
                this.currentPatientSDO = null;
                this.cardSearch = null;
                this.txtCareerCode.Text = null;
                this.cboCareer.EditValue = null;
                HIS_CAREER career = HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.CareerBase;
                if(career != null && (cboCareer.Properties.DataSource as List<HIS_CAREER>).Exists(o=>o.ID == career.ID))
                {
                    this.txtCareerCode.Text = career.CAREER_CODE;
                    this.cboCareer.EditValue = career.ID;
                }
                lciWorkPlaceNameNew.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciMilitaryRankNew.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciPositionNew.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                FillDefaultData_Carrer_Gender_PatientType(HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.UsingPatientTypeOfPreviousPatient);
                this.isReadQrCode = false;
                this.typeReceptionForm = null;
                this.ResetRequiredField();
                if (this.dlgEnableFindType != null)
                {
                    this.dlgEnableFindType(this.typeCodeFind == ResourceMessage.typeCodeFind__SoThe || this.typeCodeFind == ResourceMessage.typeCodeFind__MaTV);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDefaultData_Carrer_Gender_PatientType(bool usingPatientTypeOfPreviousPatient = false)
        {
            try
            {
                cboPrimaryPatientType.EditValue = null;

                if (usingPatientTypeOfPreviousPatient)
                {
                    Inventec.Common.Logging.LogSystem.Info("Co cau hinh lay doi tuong benh nhan cua lan tiep don truoc do___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => usingPatientTypeOfPreviousPatient), usingPatientTypeOfPreviousPatient));
                }
                else
                {
                    var patientTypeDefault = HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.PatientTypeDefault;
                    if (patientTypeDefault != null && patientTypeDefault.ID > 0)
                    {
                        IsLoadFromSearchTxtCode = true;
                        this.txtPatientTypeCode.Text = patientTypeDefault.PATIENT_TYPE_CODE;
                        this.cboPatientType.EditValue = patientTypeDefault.ID;
                        IsLoadFromSearchTxtCode = false;
                    }
                    else
                    {
                        this.currentPatientType = null;
                        this.txtPatientTypeCode.Text = "";
                        this.cboPatientType.EditValue = null;
                        if (isDefault)
                        {
                            IsLoadFromSearchTxtCode = true;
                            cboPatientType.EditValue = paties[0].ID;
                            IsLoadFromSearchTxtCode = false;
                        }
                    }
                }
                var careerDefault = HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.CareerBase;
                if (careerDefault != null && BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).SingleOrDefault(o => o.ID == careerDefault.ID) != null)
                {
                    this.txtCareerCode.Text = careerDefault.CAREER_CODE;
                    this.cboCareer.EditValue = careerDefault.ID;
                }

                var genderDefault = HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.GenderBase;
                if (genderDefault != null && genderDefault.ID > 0)
                {
                    this.cboGender.EditValue = genderDefault.ID;
                }

                this.LoadEthnicBase();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetNameControl(string ado)
		{
			try
			{
                if (!string.IsNullOrEmpty(ado))
                    this.baseNameControl = ado;
			}
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
		}

        #endregion

        #region Set Delegate Visible UCHein

        public void SetDelegateVisibleUCHein(DelegateVisibleUCHein _dlgVisibleUCHein)
        {
            try
            {
                if (_dlgVisibleUCHein != null)
                    this.dlgVisibleUCHein = _dlgVisibleUCHein;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
    }
}
