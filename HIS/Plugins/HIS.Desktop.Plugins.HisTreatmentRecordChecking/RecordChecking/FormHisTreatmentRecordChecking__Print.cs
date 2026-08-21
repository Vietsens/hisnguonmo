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
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using EMR.SDO;
using EMR.TDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.HisTreatmentRecordChecking.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.SignLibrary;
using Inventec.Common.SignLibrary.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace HIS.Desktop.Plugins.HisTreatmentRecordChecking.RecordChecking
{
    /// <summary>
    /// Document view / digital signature popup.
    /// </summary>
    public partial class FormHisTreatmentRecordChecking
    {
        private void repositoryItemButtonView_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (V_EMR_DOCUMENT)Gv_EmrDocument.GetFocusedRow();
                if (row != null)
                {
                    V_HIS_ROOM room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.moduleData.RoomId);
                    CommonParam param = new CommonParam();
                    EmrDocumentDownloadFileSDO filter = new EmrDocumentDownloadFileSDO();
                    filter.EmrDocumentViewFilter = new EmrDocumentViewFilter();
                    filter.EmrDocumentViewFilter.ID = row.ID;
                    filter.IsMerge = false;
                    filter.IsShowPatientSign = null;
                    filter.IsShowWatermark = null;
                    if (room != null)
                    {
                        filter.RoomCode = room.ROOM_CODE;
                        filter.DepartmentCode = room.DEPARTMENT_CODE;
                    }

                    filter.IsView = true;

                    var listDocumentFile = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<EmrDocumentFileSDO>>("api/EmrDocument/DownloadFile", ApiConsumers.EmrConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    if (listDocumentFile != null && listDocumentFile.Count > 0)
                    {
                        String temFile = Path.GetTempFileName();
                        temFile = temFile.Replace(".tmp", ".pdf");
                        Utils.ByteToFile(Utils.StreamToByte(new MemoryStream(Convert.FromBase64String(listDocumentFile.FirstOrDefault().Base64Data))), temFile);
                        
                        SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();

                        InputADO inputADO = new InputADO();
                        inputADO.DTI = String.Format("{0}|{1}|{2}|{3}|{4}|{5}", ConfigSystems.URI_API_ACS, ConfigSystems.URI_API_EMR, ConfigSystems.URI_API_FSS, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetTokenData().TokenCode, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName());
                        inputADO.IsSave = false;
                        if ((row.REJECTER == null
                   && row.NEXT_SIGNER == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName())
                   || (((row.NEXT_SIGNER == null && (row.SIGNERS == null || !row.SIGNERS.Contains("#@!@#" + row.PATIENT_CODE))) || (row.NEXT_SIGNER != null && row.NEXT_SIGNER.Contains("#@!@#" + row.PATIENT_CODE)))
                           && HIS.Desktop.LocalStorage.EmrConfig.EmrConfigs.Get<string>("EMR.EMR_DOCUMENT.PATIENT_SIGN.OPTION") == "3"))
                        {
                            inputADO.IsSign = true;
                        }
                        else
                            inputADO.IsSign = false;


                        inputADO.IsSave = false;
                        inputADO.IsExport = false;

                        inputADO.IsPrint = true;

                        inputADO.IsEnableButtonPrint = controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == "EMR000002") != null;
                        inputADO.IsShowPatientSign = true;
                        //Mở popup 
                        inputADO.Treatment = new Inventec.Common.SignLibrary.DTO.TreatmentDTO();
                        inputADO.Treatment.TREATMENT_CODE = row.TREATMENT_CODE;//mã hồ sơ điều trị

                        inputADO.DocumentCode = row.DOCUMENT_CODE;
                        inputADO.DocumentName = row.DOCUMENT_NAME;//Tên văn bản cần tạo

                        inputADO.DlgOpenModuleConfig = OpenSignConfig;
                        if (!String.IsNullOrWhiteSpace(temFile) && System.IO.File.Exists(temFile))
                        {
                            libraryProcessor.ShowPopup(temFile, inputADO);
                            BtnSearch_Click(null,null);
                        }
                        else
                        {
                            XtraMessageBox.Show(
                                Resources.ResourceMessage.KhongXacDinhDuocVanBanKy,
                                HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(
                                    HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        if (System.IO.File.Exists(temFile)) System.IO.File.Delete(temFile);
                    }
                    else
                    {
                        #region Hien thi message thong bao
                        MessageManager.Show(this, param, false);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void OpenSignConfig(DocumentTDO obj)
        {
            try
            {
                if (obj != null)
                {
                    EMR.Filter.EmrDocumentFilter filter = new EMR.Filter.EmrDocumentFilter();
                    filter.DOCUMENT_CODE__EXACT = obj.DocumentCode;
                    var apiResult = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<EMR.EFMODEL.DataModels.EMR_DOCUMENT>>(EMR.URI.EmrDocument.GET, ApiConsumer.ApiConsumers.EmrConsumer, filter, SessionManager.ActionLostToken, null);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        List<object> _listObj = new List<object>();
                        _listObj.Add(apiResult.Max(o => o.ID));//truyền vào id lớn nhất;

                        HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("EMR.Desktop.Plugins.EmrSign", moduleData.RoomId, moduleData.RoomTypeId, _listObj);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
