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
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignServiceTestMulti.ADO;
using HIS.Desktop.Plugins.AssignServiceTestMulti.Resources;
using HIS.Desktop.Print;
using HIS.Desktop.Utilities.Extentions;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.LibraryMessage;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignServiceTestMulti.AssignService
{
    public partial class frmAssignService : HIS.Desktop.Utility.FormBase
    {
        private void ProcessServiceReqSDO(AssignTestForBloodSDO HisServiceReqSDO, List<SereServADO> dataSereServModel)
        {
            try
            {
                if (dataSereServModel != null && dataSereServModel.Count > 0)
                {
                    HisServiceReqSDO.ServiceReqDetails = new List<ServiceReqDetailSDO>();
                    foreach (var item in dataSereServModel)
                    {
                        ServiceReqDetailSDO serviceReqDetail = new MOS.SDO.ServiceReqDetailSDO();
                        serviceReqDetail.Amount = item.AMOUNT;
                        serviceReqDetail.PatientTypeId = item.PATIENT_TYPE_ID;
                        if (item.TDL_EXECUTE_ROOM_ID > 0)
                            serviceReqDetail.RoomId = item.TDL_EXECUTE_ROOM_ID;
                        serviceReqDetail.ServiceId = item.SERVICE_ID;
                        if (item.IsOutParentFee)
                            serviceReqDetail.IsOutParentFee = GlobalVariables.CommonNumberTrue;
                        if (item.IsExpend == true)
                            serviceReqDetail.IsExpend = GlobalVariables.CommonNumberTrue;
                        //if (serviceReqParentId > 0)
                        //    serviceReqDetail.ParentId = serviceReqParentId;
                        HisServiceReqSDO.ServiceReqDetails.Add(serviceReqDetail);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private long GetRoomId()
        {
            long roomId = 0;
            try
            {
                if (this.currentModule != null)
                {
                    roomId = this.currentModule.RoomId;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return roomId;
        }

        private void ProcessServiceReqSDO_ServiceIcd(AssignTestForBloodSDO HisServiceReqSDO)
        {
            try
            {
                //if (!string.IsNullOrEmpty(txtIcdServiceReq.Text))
                //{
                //    HisServiceReqSDO.IcdMainText = txtIcdServiceReq.Text;
                //}
                //if (cboIcdServiceReq.EditValue != null)
                //{
                //    HisServiceReqSDO.IcdId = (long)cboIcdServiceReq.EditValue;
                //}
                //if (!string.IsNullOrEmpty(txtIcdExtraNames.Text))
                //{
                //    HisServiceReqSDO.IcdText = txtIcdExtraNames.Text;
                //}
                //if (!string.IsNullOrEmpty(txtIcdExtraCodes.Text))
                //{
                //    HisServiceReqSDO.IcdSubCode = txtIcdExtraCodes.Text;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveServiceReqCombo(AssignTestForBloodSDO hisServiceReqSDO, bool isSaveAndPrint)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                WaitingManager.Show();
                hisServiceReqSDO.InstructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(this.dtInstructionTime.DateTime) ?? 0;
                hisServiceReqSDO.ServiceReqId = this.serviceReqId;
                this.serviceReqComboResultSDO = new BackendAdapter(param).Post<HisServiceReqListResultSDO>(RequestUriStore.HIS_TEST_SERVICE_REQ_TEST_EXPORT_BLOOD, ApiConsumers.MosConsumer, hisServiceReqSDO, ProcessLostToken, param);
                if (this.serviceReqComboResultSDO != null)
                {
                    if (processDataResult != null)
                        processDataResult(this.serviceReqComboResultSDO);

                    this.actionType = GlobalVariables.ActionView;
                    success = true;
                    this.SetEnableButtonControl(this.actionType);
                    this.isSaveAndPrint = isSaveAndPrint;
                    if (isSaveAndPrint)
                    {
                        //Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                        //InCacPhieuChiDinhProcess(richEditorMain);
                        InPhieuYeuCauDichVu();
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info("Goi api chi dinh dv ky thuat that bai. Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisServiceReqSDO), hisServiceReqSDO) + ". Du lieu dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqComboResultSDO), serviceReqComboResultSDO) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                }
                WaitingManager.Hide();

                #region Show message
                MessageManager.Show(this, param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private bool CheckValidDataInGridService(CommonParam param, List<SereServADO> serviceCheckeds__Send)
        {
            bool valid = true;
            try
            {
                if (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0)
                {
                    foreach (var item in serviceCheckeds__Send)
                    {
                        string messageErr = "";
                        messageErr = String.Format(ResourceMessage.CanhBaoDichVu, item.TDL_SERVICE_NAME);

                        if (item.PATIENT_TYPE_ID <= 0)
                        {
                            valid = false;
                            messageErr += ResourceMessage.KhongCoDoiTuongThanhToan;
                            Inventec.Common.Logging.LogSystem.Debug("Dich vu (" + item.TDL_SERVICE_CODE + "-" + item.TDL_SERVICE_NAME + " khong co doi tuong thanh toan.");
                        }
                        if (item.AMOUNT <= 0)
                        {
                            valid = false;
                            messageErr += ResourceMessage.KhongNhapSoLuong;
                            Inventec.Common.Logging.LogSystem.Debug("Dich vu (" + item.TDL_SERVICE_CODE + "-" + item.TDL_SERVICE_NAME + " khong co so luong.");
                        }
                        //if (item.TDL_EXECUTE_ROOM_ID <= 0)
                        //{
                        //    valid = false;
                        //    messageErr += ResourceMessage.KhongChonPhongXuLyDichVu;
                        //    Inventec.Common.Logging.LogSystem.Debug("Dich vu (" + item.SERVICE_CODE + "-" + item.SERVICE_NAME + " khong chon phong xu ly dich vu.");
                        //}

                        if (!valid)
                        {
                            param.Messages.Add(messageErr + ";");
                        }
                    }
                }
                else
                {
                    HIS.Desktop.LibraryMessage.MessageUtil.SetParam(param, HIS.Desktop.LibraryMessage.Message.Enum.ThongBaoDuLieuTrong);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private bool Valid(List<SereServADO> serviceCheckeds__Send)
        {
            CommonParam param = new CommonParam();
            bool valid = true;
            try
            {
                this.positionHandleControl = -1;
                valid = (this.dxValidationProviderControl.Validate());
                valid = valid && this.CheckValidDataInGridService(param, serviceCheckeds__Send);

                if (!valid)
                {
                    MessageManager.Show(param, null);
                    LogSystem.Debug(LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceCheckeds__Send), serviceCheckeds__Send));
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return valid;
        }

    }
}
