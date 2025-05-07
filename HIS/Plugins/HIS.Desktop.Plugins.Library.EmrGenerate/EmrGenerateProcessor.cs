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
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using Inventec.Common.Integrate;
using Inventec.Common.RichEditor;
using Inventec.Common.SignLibrary.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Library.EmrGenerate
{
    public class EmrGenerateProcessor
    {
        public EmrGenerateProcessor() { }

        const int use__Sign_USB = 1;
        const int use__Sign_HSM = 2;
        const int useOption__SignDefault_USB = 3;
        const int useOption__SignDefault_HSM = 4;

        bool isNotLoadWhileChangeControlStateInFirst;
        string ModuleLinkName = "HIS.Desktop.Plugins.Library.EmrGenerate";
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        //List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;

        void OpenEmrConfig(EMR.TDO.DocumentTDO documentTDO)
        {
            List<object> _listObj = new List<object>();

            Inventec.Desktop.Common.Modules.Module module = new Inventec.Desktop.Common.Modules.Module();
            module.ModuleLink = "EMR.Desktop.Plugins.EmrSign";
            module.ModuleTypeId = Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM;
            module.text = "Thiết lập ký";

            EMR.Filter.EmrDocumentFilter filter = new EMR.Filter.EmrDocumentFilter();
            filter.DOCUMENT_CODE__EXACT = documentTDO.DocumentCode;
            var apiResult = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<EMR.EFMODEL.DataModels.EMR_DOCUMENT>>(EMR.URI.EmrDocument.GET, HIS.Desktop.ApiConsumer.ApiConsumers.EmrConsumer, filter, SessionManager.ActionLostToken, null);
            if (apiResult != null && apiResult.Count > 0)
            {
                _listObj.Add(apiResult.Max(o => o.ID));//truyền vào id lớn nhất;

                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("EMR.Desktop.Plugins.EmrSign", module.RoomId, module.RoomTypeId, _listObj);
            }
        }

        public Inventec.Common.SignLibrary.ADO.InputADO GenerateInputADO(string treatmentCode, string documentCode, string documentName, long roomId = 0)
        {
            Inventec.Common.SignLibrary.ADO.InputADO inputADO = new Inventec.Common.SignLibrary.ADO.InputADO();
            inputADO.Treatment = new Inventec.Common.SignLibrary.DTO.TreatmentDTO();
            inputADO.Treatment.TREATMENT_CODE = treatmentCode;
            inputADO.DocumentName = documentName;
            inputADO.DocumentCode = documentCode;
            inputADO.DTI = String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}", ConfigSystems.URI_API_ACS, ConfigSystems.URI_API_EMR, ConfigSystems.URI_API_FSS, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetTokenData().TokenCode, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName(), ConfigSystems.URI_API_HPS);
            inputADO.IsSign = true;
            inputADO.IsPrintOnlyContent = HisConfigCFG.IsNotShowingSignInformation;
            if (HisConfigCFG.EmrSignType == use__Sign_USB)
            {
                inputADO.SignType = Inventec.Common.SignLibrary.SignType.USB;
            }
            else if (HisConfigCFG.EmrSignType == use__Sign_HSM)
            {
                inputADO.SignType = Inventec.Common.SignLibrary.SignType.HMS;
            }
            else if (HisConfigCFG.EmrSignType == useOption__SignDefault_USB)
            {
                inputADO.SignType = Inventec.Common.SignLibrary.SignType.OptionDefaultUsb;
            }
            else if (HisConfigCFG.EmrSignType == useOption__SignDefault_HSM)
            {
                inputADO.SignType = Inventec.Common.SignLibrary.SignType.OptionDefaultHsm;
            }
            else
            {
                inputADO.SignType = Inventec.Common.SignLibrary.SignType.USB;
            }
            inputADO.DlgOpenModuleConfig = OpenEmrConfig;

            inputADO.DlgCloseAfterSign = ActCheckedChangedCloseAfterSign;
            inputADO.IsCloseAfterSign = GetCheckedStateCloseAfterSign();

            inputADO.IsUsingSignPad = GetOptionUsingSignPad();
            inputADO.ActChangeUsingSignPad = ActChangeUsingSignPad;
            inputADO.ActSelectDevice = ActSelectDevice;
            inputADO.DeviceSignPadName = GetDeviceSignPadName();

            var room = roomId > 0 ? BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().Where(o => o.ID == roomId).FirstOrDefault() : null;
            inputADO.RoomCode = room != null ? room.ROOM_CODE : (RichEditorConfig.WorkingRoom != null ? RichEditorConfig.WorkingRoom.ROOM_CODE : "");
            inputADO.RoomName = room != null ? room.ROOM_NAME : (RichEditorConfig.WorkingRoom != null ? RichEditorConfig.WorkingRoom.ROOM_NAME : "");
            inputADO.RoomTypeCode = room != null ? room.ROOM_TYPE_CODE : (RichEditorConfig.WorkingRoom != null ? RichEditorConfig.WorkingRoom.ROOM_TYPE_CODE : "");
            inputADO.DepartmentCode = room != null ? room.DEPARTMENT_CODE : (RichEditorConfig.WorkingRoom != null ? RichEditorConfig.WorkingRoom.DEPARTMENT_CODE : "");
            inputADO.DepartmentName = room != null ? room.DEPARTMENT_NAME : (RichEditorConfig.WorkingRoom != null ? RichEditorConfig.WorkingRoom.DEPARTMENT_NAME : "");

            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("inputADO", inputADO));

            return inputADO;
        }

        public Inventec.Common.SignLibrary.ADO.InputADO GenerateInputADOWithPrintTypeCode(string treatmentCode, string printTypeCode)
        {
            return GenerateInputADOWithPrintTypeCode(treatmentCode, printTypeCode, true, null);
        }

        public Inventec.Common.SignLibrary.ADO.InputADO GenerateInputADOWithPrintTypeCode(string treatmentCode, string printTypeCode, bool isSign)
        {
            return GenerateInputADOWithPrintTypeCode(treatmentCode, printTypeCode, isSign, null);
        }

        public Inventec.Common.SignLibrary.ADO.InputADO GenerateInputADOWithPrintTypeCode(string treatmentCode, string printTypeCode, long? roomId)
        {
            return GenerateInputADOWithPrintTypeCode(treatmentCode, printTypeCode, true, roomId);
        }

        public Inventec.Common.SignLibrary.ADO.InputADO GenerateInputADOWithPrintTypeCode(string treatmentCode, string printTypeCode, bool isSign, long? roomId)
        {
            Inventec.Common.SignLibrary.ADO.InputADO inputADO = new Inventec.Common.SignLibrary.ADO.InputADO();
            inputADO.Treatment = new Inventec.Common.SignLibrary.DTO.TreatmentDTO();
            inputADO.Treatment.TREATMENT_CODE = treatmentCode;
            inputADO.DlgGetTreatment = GetTreatmentByCode;

            var printTypes = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>();
            var pr = printTypes.FirstOrDefault(o => o.PRINT_TYPE_CODE != null && o.PRINT_TYPE_CODE.ToLower() == printTypeCode.ToLower());
            if (pr != null)
            {
                inputADO.DocumentName = String.Format("{0}", pr.PRINT_TYPE_NAME);
                inputADO.DocumentTypeCode = pr.EMR_DOCUMENT_TYPE_CODE;
                inputADO.PrintTypeBusinessCodes = GetBussinessCode(pr.BUSINESS_CODES);
                inputADO.IsAutoChooseBusiness = (pr.IS_AUTO_CHOOSE_BUSINESS == 1);
            }

            inputADO.DTI = String.Format("{0}|{1}|{2}|{3}|{4}|{5}", ConfigSystems.URI_API_ACS, ConfigSystems.URI_API_EMR, ConfigSystems.URI_API_FSS, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetTokenData().TokenCode, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName());
            inputADO.IsSign = isSign;
            inputADO.IsSelectRangeRectangle = false;
            inputADO.IsPrintOnlyContent = HisConfigCFG.IsNotShowingSignInformation;
            if (HisConfigCFG.EmrSignType == use__Sign_USB)
            {
                inputADO.SignType = Inventec.Common.SignLibrary.SignType.USB;
            }
            else if (HisConfigCFG.EmrSignType == use__Sign_HSM)
            {
                inputADO.SignType = Inventec.Common.SignLibrary.SignType.HMS;
            }
            else if (HisConfigCFG.EmrSignType == useOption__SignDefault_USB)
            {
                inputADO.SignType = Inventec.Common.SignLibrary.SignType.OptionDefaultUsb;
            }
            else if (HisConfigCFG.EmrSignType == useOption__SignDefault_HSM)
            {
                inputADO.SignType = Inventec.Common.SignLibrary.SignType.OptionDefaultHsm;
            }
            else
            {
                inputADO.SignType = Inventec.Common.SignLibrary.SignType.USB;
            }
            inputADO.DlgOpenModuleConfig = OpenEmrConfig;
            var room = (roomId.HasValue && roomId > 0) ? BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().Where(o => o.ID == roomId).FirstOrDefault() : null;
            inputADO.RoomCode = room != null ? room.ROOM_CODE : (RichEditorConfig.WorkingRoom != null ? RichEditorConfig.WorkingRoom.ROOM_CODE : "");
            inputADO.RoomName = room != null ? room.ROOM_NAME : (RichEditorConfig.WorkingRoom != null ? RichEditorConfig.WorkingRoom.ROOM_NAME : "");
            inputADO.RoomTypeCode = room != null ? room.ROOM_TYPE_CODE : (RichEditorConfig.WorkingRoom != null ? RichEditorConfig.WorkingRoom.ROOM_TYPE_CODE : "");

            inputADO.ActSelectDevice = ActSelectDevice;
            inputADO.DeviceSignPadName = GetDeviceSignPadName();
            inputADO.IsUsingSignPad = GetOptionUsingSignPad();
            inputADO.ActChangeUsingSignPad = ActChangeUsingSignPad;
            inputADO.DlgCloseAfterSign = ActCheckedChangedCloseAfterSign;
            inputADO.IsCloseAfterSign = GetCheckedStateCloseAfterSign();

            inputADO.DepartmentCode = room != null ? room.DEPARTMENT_CODE : (RichEditorConfig.WorkingRoom != null ? RichEditorConfig.WorkingRoom.DEPARTMENT_CODE : "");
            inputADO.DepartmentName = room != null ? room.DEPARTMENT_NAME : (RichEditorConfig.WorkingRoom != null ? RichEditorConfig.WorkingRoom.DEPARTMENT_NAME : "");

            return inputADO;
        }

        TreatmentDTO GetTreatmentByCode(string treatmentCode)
        {
            TreatmentDTO treatmentDTOResult = new TreatmentDTO();
            try
            {
                //MOS.Filter.HisCardViewFilter cardViewFilter = new MOS.Filter.HisCardViewFilter();
                //cardViewFilter.TREATMENT_CODE__EXACT = treatmentCode;//TODO
                //var apiResult = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_CARD>>("api/HisCard/GetView", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, cardViewFilter, SessionManager.ActionLostToken, null);
                //if (apiResult != null && apiResult.Count > 0)
                //{
                //    treatmentDTOResult.CARD_CODE = apiResult[0].CARD_CODE;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return treatmentDTOResult;
        }

        bool? GetCheckedStateCloseAfterSign()
        {
            //TODO

            //ĐỌc từ file cache để lấy ra giá trị gán vào biến
            bool checkValues = false;
            try
            {

                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                var currentControlStateRDOs = controlStateWorker.GetData(ModuleLinkName);
                if (currentControlStateRDOs != null && currentControlStateRDOs.Count > 0)
                {
                    foreach (var item in currentControlStateRDOs)
                    {
                        if (item.KEY == "bbtnChkCloseAfterSign")
                        {
                            checkValues = item.VALUE == "1";
                            break;
                        }
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("dữ liệu currentControlStateRDOss.count: " + (currentControlStateRDOs != null ? currentControlStateRDOs.Count : 0));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return checkValues;
        }

        bool GetOptionUsingSignPad()
        {
            //TODO

            //ĐỌc từ file cache để lấy ra giá trị gán vào biến
            bool bValues = false;
            try
            {

                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                var currentControlStateRDOs = controlStateWorker.GetData("Inventec.Common.SignLibrary");
                if (currentControlStateRDOs != null && currentControlStateRDOs.Count > 0)
                {
                    foreach (var item in currentControlStateRDOs)
                    {
                        if (item.KEY == "IsUsingSignPad" && item.MODULE_LINK == "Inventec.Common.SignLibrary")
                        {
                            bValues = (item.VALUE == "1");
                            break;
                        }
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("GetOptionUsingSignPad: dữ liệu currentControlStateRDOss.Count: " + (currentControlStateRDOs != null ? currentControlStateRDOs.Count : 0) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => bValues), bValues));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return bValues;
        }

        private void ActChangeUsingSignPad(bool isUsingSignPad)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                var currentControlStateRDOs = controlStateWorker.GetData("Inventec.Common.SignLibrary");

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (currentControlStateRDOs != null && currentControlStateRDOs.Count > 0) ? currentControlStateRDOs.Where(o => o.KEY == "IsUsingSignPad").FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (isUsingSignPad ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = "IsUsingSignPad";
                    csAddOrUpdate.VALUE = (isUsingSignPad ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = "Inventec.Common.SignLibrary";
                    if (currentControlStateRDOs == null)
                        currentControlStateRDOs = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    currentControlStateRDOs.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(currentControlStateRDOs);

                Inventec.Common.Logging.LogSystem.Debug("ActChangeUsingSignPad____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isUsingSignPad), isUsingSignPad) + ", currentControlStateRDOs.Count:" + (currentControlStateRDOs != null ? currentControlStateRDOs.Count : 0)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        string GetDeviceSignPadName()
        {
            //TODO

            //ĐỌc từ file cache để lấy ra giá trị gán vào biến
            string strValues = "";
            try
            {

                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                var currentControlStateRDOs = controlStateWorker.GetData("HIS.Desktop.Plugins.EmrSigner");
                if (currentControlStateRDOs != null && currentControlStateRDOs.Count > 0)
                {
                    foreach (var item in currentControlStateRDOs)
                    {
                        if (item.KEY == "DeviceName" && item.MODULE_LINK == "HIS.Desktop.Plugins.EmrSigner")
                        {
                            strValues = item.VALUE;
                        }
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("dữ liệu currentControlStateRDOs.Count:" + (currentControlStateRDOs != null ? currentControlStateRDOs.Count : 0));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return strValues;
        }

        private void ActSelectDevice(string deviceName)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                var currentControlStateRDOs = controlStateWorker.GetData("HIS.Desktop.Plugins.EmrSigner");
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (currentControlStateRDOs != null && currentControlStateRDOs.Count > 0) ? currentControlStateRDOs.Where(o => o.KEY == "DeviceName").FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = deviceName;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = "DeviceName";
                    csAddOrUpdate.VALUE = deviceName;
                    csAddOrUpdate.MODULE_LINK = "HIS.Desktop.Plugins.EmrSigner";
                    if (currentControlStateRDOs == null)
                        currentControlStateRDOs = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    currentControlStateRDOs.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(currentControlStateRDOs);

                Inventec.Common.Logging.LogSystem.Debug("ActSelectDevice____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => deviceName), deviceName) + ", dữ liệu currentControlStateRDOs.Count:" + (currentControlStateRDOs != null ? currentControlStateRDOs.Count : 0)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ActCheckedChangedCloseAfterSign(bool checkValue)
        {
            //
            //Lưu vào file cache từ giá trị truyền vào
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                var currentControlStateRDOs = controlStateWorker.GetData(ModuleLinkName);
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (currentControlStateRDOs != null && currentControlStateRDOs.Count > 0) ? currentControlStateRDOs.Where(o => o.KEY == "bbtnChkCloseAfterSign").FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (checkValue ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = "bbtnChkCloseAfterSign";
                    csAddOrUpdate.VALUE = (checkValue ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ModuleLinkName;
                    if (currentControlStateRDOs == null)
                        currentControlStateRDOs = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    currentControlStateRDOs.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(currentControlStateRDOs);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private List<string> GetBussinessCode(string businessCodes)
        {
            List<string> results = new List<string>();
            try
            {
                if (!String.IsNullOrEmpty(businessCodes))
                {
                    var rs = businessCodes.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
                    if (rs != null && rs.Count() > 0)
                    {
                        results = rs.Where(o => o != "[" && o != "]").ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return results;
        }
    }
}
