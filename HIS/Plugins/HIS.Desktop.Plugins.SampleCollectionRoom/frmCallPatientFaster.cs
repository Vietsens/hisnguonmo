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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.SampleCollectionRoom.Config;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
using LIS.SDO;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SampleCollectionRoom
{
    public partial class frmCallPatientFaster : HIS.Desktop.Utility.FormBase
    {
        LIS_SAMPLE rowSample;
        DelegateSelectData refreshDataAfterSuccess;
        Inventec.Desktop.Common.Modules.Module moduleData;

        public frmCallPatientFaster(Inventec.Desktop.Common.Modules.Module _moduleData, DelegateSelectData _refreshDataAfterSuccess)
            : base(_moduleData)
        {
            try
            {
                InitializeComponent();
                this.moduleData = _moduleData;
                this.refreshDataAfterSuccess = _refreshDataAfterSuccess;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmGetSampleFaster_Load(object sender, EventArgs e)
        {
            try
            {
                txtBarcode.Focus();
                txtBarcode.SelectAll();
                SetIcon();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetSampleByBarCode()
        {
            try
            {
                CommonParam param = new CommonParam();
                LisSampleFilter sampleFilter = new LisSampleFilter();
                sampleFilter.SERVICE_REQ_CODE__EXACT = this.txtBarcode.Text.Trim();
                var LisSamples = new BackendAdapter(param).Get<List<LIS_SAMPLE>>("api/LisSample/Get", ApiConsumer.ApiConsumers.LisConsumer, sampleFilter, param);
                if (LisSamples != null && LisSamples.Count == 1)
                {
                    this.rowSample = LisSamples.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnGetSampleFaster_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtBarcode.Text))
                {
                    GetSampleByBarCode();
                    if (this.rowSample != null)
                    {
                        UpdateDicCallPatient(this.rowSample);
                        LoadCallPatientByThread(this.rowSample);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void LoadCallPatientByThread(object param)
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataToControlCallPatientThread));
            try
            {
                thread.Start(param);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                thread.Abort();
            }
        }

        private void UpdateDicCallPatient(LIS_SAMPLE lisSample)
        {
            try
            {
                if (lisSample != null)
                {

                    if (!CallPatientDataWorker.DicCallPatient.ContainsKey(this.moduleData.RoomId))
                    {
                        CallPatientDataWorker.DicCallPatient.Add(this.moduleData.RoomId, new List<ServiceReq1ADO>());
                    }
                    foreach (var dic in HIS.Desktop.LocalStorage.BackendData.CallPatientDataWorker.DicCallPatient)
                    {
                        if (dic.Key == this.moduleData.RoomId)
                        {
                            ServiceReq1ADO ado = dic.Value != null ? dic.Value.FirstOrDefault(o => !String.IsNullOrWhiteSpace(o.SERVICE_REQ_CODE) && o.SERVICE_REQ_CODE.Contains(lisSample.SERVICE_REQ_CODE)) : null; ;

                            if (ado == null)
                            {
                                ServiceReq1ADO serviceReq1ADO = new ServiceReq1ADO();
                                serviceReq1ADO.ID = lisSample.ID;
                                serviceReq1ADO.SERVICE_REQ_CODE = lisSample.SERVICE_REQ_CODE;
                                serviceReq1ADO.TDL_PATIENT_CODE = lisSample.PATIENT_CODE;
                                serviceReq1ADO.TDL_PATIENT_NAME = (lisSample.LAST_NAME ?? "" + " " + lisSample.FIRST_NAME ?? "").Trim();
                                serviceReq1ADO.NUM_ORDER = lisSample.CALL_SAMPLE_ORDER;
                                serviceReq1ADO.TDL_PATIENT_DOB = lisSample.DOB ?? 0;
                                dic.Value.Add(serviceReq1ADO);
                            }

                            foreach (var item in dic.Value)
                            {
                                if (!String.IsNullOrWhiteSpace(item.SERVICE_REQ_CODE) && (item.SERVICE_REQ_CODE == lisSample.SERVICE_REQ_CODE || item.SERVICE_REQ_CODE.Contains(lisSample.SERVICE_REQ_CODE)))
                                {
                                    item.CallPatientSTT = true;
                                }
                                else
                                {
                                    item.CallPatientSTT = false;
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

        internal void LoadDataToControlCallPatientThread(object param)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { CallPatient(param); }));
                }
                else
                {
                    CallPatient(param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void CallPatient(object param)
        {
            try
            {
                if (param is LIS_SAMPLE)
                {
                    var data = param as LIS_SAMPLE;
                    if (data != null)
                    {
                        CallPatientByNumOder(data.LAST_NAME + " " + data.FIRST_NAME, data.CALL_SAMPLE_ORDER ?? 0, data.EXECUTE_ROOM_NAME);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void CallPatientByNumOder(string patientName, long numOder, string examRoomName)
        {
            try
            {
                string moiBenhNhanStr = HisConfigs.Get<string>(SdaConfigKeys.CALL_PATIENT_MOI_BENH_NHAN);
                string coSoSttStr = HisConfigs.Get<string>(SdaConfigKeys.CALL_PATIENT_CO_STT);
                string denStr = HisConfigs.Get<string>(SdaConfigKeys.CALL_PATIENT_DEN);

                Inventec.Speech.SpeechPlayer.SpeakSingle(moiBenhNhanStr);
                Inventec.Speech.SpeechPlayer.Speak(patientName);
                Inventec.Speech.SpeechPlayer.SpeakSingle(coSoSttStr);
                Inventec.Speech.SpeechPlayer.Speak(numOder);
                Inventec.Speech.SpeechPlayer.SpeakSingle(denStr);
                Inventec.Speech.SpeechPlayer.SpeakSingle(examRoomName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtBarcode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnGetSampleFaster_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnCallPatient_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnGetSampleFaster_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
