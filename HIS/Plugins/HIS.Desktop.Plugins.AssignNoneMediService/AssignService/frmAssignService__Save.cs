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
using ACS.EFMODEL.DataModels;
using DevExpress.Office.Utils;
using EMR.Filter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignNoneMediService.ADO;
using HIS.Desktop.Plugins.AssignNoneMediService.Config;
using HIS.Desktop.Plugins.AssignNoneMediService.Resources;
using HIS.Desktop.Plugins.Library.AlertWarningFee;
using HIS.Desktop.Print;
using HIS.Desktop.Utilities.Extensions;
using HIS.UC.Icd.ADO;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Common.SignLibrary.DTO;
using Inventec.Core;
using Inventec.Desktop.Common.LibraryMessage;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignNoneMediService.AssignService
{
    public partial class frmAssignService : HIS.Desktop.Utility.FormBase
    {
        private void ProcessSaveData(bool isSaveAndPrint, bool printTH, bool isSaveAndShow, bool isSign = false, bool isPrintDocumentSigned = false)
        {
            try
            {
                if (!ValidForSave()) return;

                if (this.gridViewServiceProcess.IsEditing)
                    this.gridViewServiceProcess.CloseEditor();

                if (this.gridViewServiceProcess.FocusedRowModified)
                    this.gridViewServiceProcess.UpdateCurrentRow();

                bool isValid = true;

                List<SereServADO> serviceCheckeds__Send = this.ServiceIsleafADOs.FindAll(o => o.IsChecked);
              
                if (ucIcdYhct != null)
                    isValid = isValid && (bool)icdYhctProcessor.ValidationIcd(ucIcdYhct);
                if (ucSecondaryIcdYhct != null)
                    isValid = isValid && subIcdYhctProcessor.GetValidate(ucSecondaryIcdYhct);
                isValid = isValid && this.Valid(serviceCheckeds__Send);
                isValid = isValid && this.CheckIcd(new List<V_HIS_TREATMENT_BED_ROOM> { new V_HIS_TREATMENT_BED_ROOM() { TREATMENT_ID = currentTreatment.ID, ICD_CODE = txtIcdCode.Text.Trim(), ICD_SUB_CODE = txtIcdSubCode.Text.Trim() } });
                List<string> lstIcd = new List<string>();
                if (!string.IsNullOrEmpty(txtIcdCode.Text))
                {
                    var arrIcdCode = txtIcdCode.Text.Trim().Split(';');
                    foreach (var item in arrIcdCode)
                    {
                        if (!string.IsNullOrEmpty(item))
                            lstIcd.Add(item);
                    }
                }
                List<string> lstSubIcd = new List<string>();
                if (!string.IsNullOrEmpty(txtIcdSubCode.Text))
                {
                    var arrIcdCode = txtIcdSubCode.Text.Trim().Split(';');
                    foreach (var item in arrIcdCode)
                    {
                        if (!string.IsNullOrEmpty(item))
                            lstSubIcd.Add(item);
                    }
                }
                isValid = isValid && ValidICD();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceCheckeds__Send), serviceCheckeds__Send));
                if (isValid)
                {
                    HisNoneMediServiceReqSDO serviceReqSDO = new HisNoneMediServiceReqSDO();
                    serviceReqSDO.SereNmseSDOs = new List<HIS_SERE_NMSE>();
                    this.ProcessServiceReqSDO(serviceReqSDO, serviceCheckeds__Send);
                    this.ProcessServiceReqSDOForIcd(serviceReqSDO);
                    this.SaveServiceReqCombo(serviceReqSDO, isSaveAndPrint, printTH, isSaveAndShow, isSign, isPrintDocumentSigned);
                    this.isCheckAssignServiceSimultaneityOption = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        V_HIS_SERVICE_REQ vServiceReq;
        private bool IsReloadIcd = false;

        private void ProcessServiceReqSDO(HisNoneMediServiceReqSDO serviceReqSDO, List<SereServADO> dataSereServModel)
        {
            try
            {
                if(cboAssignRoom.EditValue != null)
                    serviceReqSDO.ExecuteRoomId = Convert.ToInt64(cboAssignRoom.EditValue);
                if (this.currentHisTreatment != null)
                    serviceReqSDO.TreatmentId = currentHisTreatment.ID;

                if (this.serviceReqParentId != 0)
                    serviceReqSDO.ParentServiceReqId = this.serviceReqParentId;

                if (this.txtDescription.Text != "")
                    serviceReqSDO.Description = this.txtDescription.Text.Trim();
                ACS_USER acsUser = null;
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                if (cboUser.EditValue != null)
                {
                    acsUser = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME.Equals(cboUser.EditValue.ToString()));
                }

                if (acsUser == null)
                {
                    acsUser = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME.Equals(loginName));
                }

                if (acsUser != null)
                {
                    serviceReqSDO.RequestLoginName = acsUser.LOGINNAME;
                    serviceReqSDO.RequestUserName = acsUser.USERNAME;
                    txtLoginName.Text = acsUser.LOGINNAME;
                    cboUser.EditValue = acsUser.LOGINNAME;
                }
                serviceReqSDO.RequestRoomId = currentModule.RoomId;
                if (dataSereServModel != null && dataSereServModel.Count > 0)
                {
                    foreach (var item in dataSereServModel)
                    {
                        HIS_SERE_NMSE sdo = new HIS_SERE_NMSE();
                        sdo.NONE_MEDI_SERVICE_ID = item.ID;
                        sdo.PRICE = item.ADD_PRICE > 0 ? item.ADD_PRICE.Value : item.PRICE;
                        sdo.TDL_NONE_MEDI_SERVICE_NAME = item.TDL_SERVICE_NAME;
                        sdo.TDL_NONE_MEDI_SERVICE_CODE = item.TDL_SERVICE_CODE;
                        sdo.AMOUNT = item.AMOUNT;
                        sdo.VAT_RATIO = item.VAT_RATIO / 100;
                        sdo.TDL_TREATMENT_ID = currentTreatment.ID;
                        sdo.TDL_TREATMENT_CODE = currentTreatment.TREATMENT_CODE;
                        sdo.TDL_SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                        serviceReqSDO.SereNmseSDOs.Add(sdo);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessServiceReqSDOForIcd(HisNoneMediServiceReqSDO serviceReqSDO)
        {
            try
            {
                var icdValue = UcIcdGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                if (icdValue != null)
                {
                    serviceReqSDO.IcdCode = icdValue.ICD_CODE;
                    if (!string.IsNullOrEmpty(icdValue.ICD_CODE))
                    {
                        serviceReqSDO.IcdCode = icdValue.ICD_CODE;
                    }
                    serviceReqSDO.IcdName = icdValue.ICD_NAME;
                }


                var subIcd = UcSecondaryIcdGetValue() as HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO;
                if (subIcd != null)
                {
                    serviceReqSDO.IcdSubCode = subIcd.ICD_SUB_CODE;
                    serviceReqSDO.IcdText = subIcd.ICD_TEXT;
                }


                var icdTranditional = this.icdYhctProcessor.GetValue(this.ucIcdYhct);
                if (icdTranditional != null && icdTranditional is IcdInputADO)
                {
                    serviceReqSDO.TraditionalIcdCode = ((IcdInputADO)icdTranditional).ICD_CODE;
                    serviceReqSDO.TraditionalIcdName = ((IcdInputADO)icdTranditional).ICD_NAME;
                }
                var subIcdTranditional = subIcdYhctProcessor.GetValue(ucSecondaryIcdYhct);
                if (subIcdTranditional != null && subIcdTranditional is SecondaryIcdDataADO)
                {
                    serviceReqSDO.TraditionalIcdSubCode = ((SecondaryIcdDataADO)subIcdTranditional).ICD_SUB_CODE;
                    serviceReqSDO.TraditionalIcdText = ((SecondaryIcdDataADO)subIcdTranditional).ICD_TEXT;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveServiceReqCombo(HisNoneMediServiceReqSDO serviceReqSDO, bool issaveandprint, bool printTH, bool isSaveAndShow, bool isSign = false, bool isPrintPreview = false, bool IsPatientSelect = false, string patientName = null, string treatmentCode = null)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                WaitingManager.Show();
                serviceReqSDO.InstructionTimes = intructionTimeSelecteds;//TODO

                Inventec.Common.Logging.LogSystem.Debug("Luu chi dinh____Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqSDO), serviceReqSDO));
                //Gọi api chỉ định dv
                var rs = new BackendAdapter(param).Post<HisNoneMediServiceReqResultSDO>("api/HisServiceReq/AssignNoneMediService", ApiConsumers.MosConsumer, serviceReqSDO, ProcessLostToken, param);
                Inventec.Common.Logging.LogSystem.Info("this.serviceReqComboResultSDO: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));

                if (rs != null)
                {
                    btnShowDetail.Enabled = true;
                    this.serviceReqComboResultSDO = rs;

                    // distint để tránh lặp #27825
                    if (this.serviceReqComboResultSDO.ServiceReqs != null && this.serviceReqComboResultSDO.ServiceReqs.Count > 0)
                    {
                        this.serviceReqComboResultSDO.ServiceReqs = this.serviceReqComboResultSDO.ServiceReqs.GroupBy(x => x.ID).Select(x => x.FirstOrDefault()).ToList();
                    }

                    success = true;
                    this.toggleSwitchDataChecked.EditValue = true;

                    //this.actionType = GlobalVariables.ActionEdit;
                    this.SetEnableButtonControl(this.actionType);
                    this.isSaveAndPrint = issaveandprint;
                    if (this.isSaveAndPrint || isPrintPreview || isSign)
                    {
                        BtnPrint.PerformClick();
                    }
                    btnSave.Enabled = btnSaveAndPrint.Enabled = false;

                }
                WaitingManager.Hide();

                if (!IsPatientSelect)
                {
                    #region Show message
                    MessageManager.Show(this, param, success);
                    #endregion

                    #region Process has exception
                    SessionManager.ProcessTokenLost(param);
                    #endregion

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

    }
}
