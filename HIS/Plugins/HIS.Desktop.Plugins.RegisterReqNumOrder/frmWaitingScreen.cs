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
using HIS.Desktop.LocalStorage.Location;
using Inventec.Common.Logging;
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
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.ConfigApplication;
using MOS.SDO;
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.BackendData;
using System.IO;

namespace HIS.Desktop.Plugins.RegisterReqNumOrder
{
    public partial class frmWaitingScreen : HIS.Desktop.Utility.FormBase
    {
        List<HisRegisterGateSDO> hisRegisterGates;
        internal MOS.EFMODEL.DataModels.V_HIS_ROOM room;
        Inventec.Desktop.Common.Modules.Module _module;
        List<ServiceReqADO> datas = null;
        List<ServiceReqADO> oldDatas = null;
        List<ServiceReqADO> listNhay = new List<ServiceReqADO>();
        ServiceReqADO currentCall { get; set; }
        RegisterReqNumOderADO currentAdo;
        string organizationName = "";
        private WorkPlaceSDO WorkPlaceSDO;
        string Note = "";
        public frmWaitingScreen(List<HisRegisterGateSDO> _hisRegisterGates, RegisterReqNumOderADO ado, Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            this.hisRegisterGates = _hisRegisterGates;
            this._module = module;
            this.currentAdo = ado;
        }

        private void frmWaitingScreen_Load(object sender, EventArgs e)
        {
            SetIcon();
            WorkPlaceSDO = WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == _module.RoomId);
            var branch = BackendDataWorker.Get<HIS_BRANCH>().Where(o => o.ID == WorkPlaceSDO.BranchId);
            try
            {
                if (currentAdo.HeaderSize > 0)
                {
                    this.gridViewWaiting.ColumnPanelRowHeight = (int)currentAdo.HeaderSize + 50;
                }

                lblDateTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0);
                this.Note = currentAdo.note;
                SetRow();
                FillDataToGridWaitingPatient(hisRegisterGates);

                timerForScrollListPatient.Interval = (int)currentAdo.reloadTime * 1000;
                RegisterTimer(_module.ModuleLink, "timerForScrollListPatient", timerForScrollListPatient.Interval, timerForScrollListPatient_Tick);
                StartTimer(_module.ModuleLink, "timerForScrollListPatient");
                //FillDataToGridWaitingPatient(_hisRegisterGates);
                datas = (List<ServiceReqADO>)gridControlWaiting.DataSource;
                setFromConfigToControl();
                lblNote.Text = currentAdo.note;
                if (!string.IsNullOrEmpty(this.Note))
                {
                    var w = currentAdo.footerSize != null ? Convert.ToDecimal(currentAdo.footerSize) : Convert.ToDecimal(lblNote.Font.Size);
                    if ((Convert.ToDecimal(this.Note.Length) * (decimal)0.7 * w) > lblNote.Size.Width)
                    {
                        RegisterTimer(_module.ModuleLink, "timerMovingText", timerMovingText.Interval, timerMovingText_Tick);
                        StartTimer(_module.ModuleLink, "timerMovingText");
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentAdo.flickerTime), currentAdo.flickerTime));
                if (currentAdo.flickerTime > 0)
                {
                    RegisterTimer(_module.ModuleLink, "timerChangeColorRow", timerChangeColorRow.Interval, timerChangeColorRow_Tick);
                    StartTimer(_module.ModuleLink, "timerChangeColorRow");
                }
                this.WindowState = FormWindowState.Maximized;
                this.BringToFront();
                this.TopMost = true;
                this.Focus();
                InitRestoreLayoutGridViewFromXml(gridViewWaiting);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetRow()
        {
            try
            {
                long heightTable = (gridControlWaiting.Height + 50) - gridViewWaiting.ColumnPanelRowHeight;
                gridViewWaiting.RowHeight = ((int)heightTable / Convert.ToInt32(this.currentAdo.maxLine));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void setFromConfigToControl()
        {
            try
            {

                //ten benh vien
                //organizationName = WaitingScreenCFG.ORGANIZATION_NAME;

                //mau background
                List<int> parentBackColorCodes = WaitingScreenCFG.PARENT_BACK_COLOR_CODES;
                if (parentBackColorCodes != null && parentBackColorCodes.Count == 3)
                {
                    layoutControlGroup1.AppearanceGroup.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                }
                // màu chữ bác sĩ



                // màu nền grid patients
                List<int> gridpatientBackColorCodes = WaitingScreenCFG.GRID_PATIENTS_BACK_COLOR_CODES;
                if (gridpatientBackColorCodes != null && gridpatientBackColorCodes.Count == 3)
                {
                    gridViewWaiting.Appearance.Empty.BackColor = System.Drawing.Color.FromArgb(gridpatientBackColorCodes[0], gridpatientBackColorCodes[1], gridpatientBackColorCodes[2]);
                }

                // Màu nền tiêu đề
                if (!string.IsNullOrEmpty(this.currentAdo.backgroundColorTitle))
                {
                    List<int> colorTittle = new List<int>();
                    colorTittle = GetColorValues(currentAdo.backgroundColorTitle);
                    gridColumn1.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(colorTittle[0], colorTittle[1], colorTittle[2]);
                    gridColumn2.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(colorTittle[0], colorTittle[1], colorTittle[2]);
                }
                // Màu nền STT
                if (!string.IsNullOrEmpty(this.currentAdo.backgroundColorSTT))
                {
                    List<int> colorTittle = new List<int>();
                    colorTittle = GetColorValues(currentAdo.backgroundColorSTT);
                    gridColumn1.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(colorTittle[0], colorTittle[1], colorTittle[2]);
                }
                // Màu nền quầy
                if (!string.IsNullOrEmpty(this.currentAdo.backgroundColorStall))
                {
                    List<int> colorTittle = new List<int>();
                    colorTittle = GetColorValues(currentAdo.backgroundColorStall);
                    gridColumn2.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(colorTittle[0], colorTittle[1], colorTittle[2]);
                }
                // Cỡ chữ chân trang
                if (this.currentAdo.footerSize != null && this.currentAdo.footerSize > 0)
                {
                    lblDateTime.Font = new System.Drawing.Font("Arial", (float)this.currentAdo.footerSize);
                    lblNote.Font = new System.Drawing.Font("Arial", (float)this.currentAdo.footerSize);

                    lblDateTime.Size = new Size(Convert.ToInt32(currentAdo.footerSize * lblDateTime.Text.Length), lblDateTime.Size.Height);
                    //lblNote.Size = new Size(Convert.ToInt32(currentAdo.footerSize * lblNote.Text.Length), lblNote.Size.Height);
                }

                // màu chữ của header danh sách bệnh nhân
                List<int> gridpatientHeaderForceColorCodes = WaitingScreenCFG.GRID_PATIENTS_HEADER_FORCE_COLOR_CODES;
                if (gridpatientHeaderForceColorCodes != null && gridpatientHeaderForceColorCodes.Count == 3)
                {
                    gridColumn1.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(gridpatientHeaderForceColorCodes[0], gridpatientHeaderForceColorCodes[1], gridpatientHeaderForceColorCodes[2]);
                    gridColumn2.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(gridpatientHeaderForceColorCodes[0], gridpatientHeaderForceColorCodes[1], gridpatientHeaderForceColorCodes[2]);
                }

                // màu chữ của body danh sách bệnh nhân
                List<int> gridpatientBodyForceColorCodes = WaitingScreenCFG.GRID_PATIENTS_BODY_FORCE_COLOR_CODES;
                if (gridpatientBodyForceColorCodes != null && gridpatientBodyForceColorCodes.Count == 3)
                {
                    gridColumn1.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                    gridColumn2.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                }

                //cỡ chữ header
                if (this.currentAdo != null && this.currentAdo.HeaderSize > 0)
                {
                    gridColumn1.AppearanceHeader.Font = new System.Drawing.Font(gridColumn1.AppearanceHeader.Font.Name, (float)this.currentAdo.HeaderSize, FontStyle.Bold);
                    gridColumn2.AppearanceHeader.Font = new System.Drawing.Font(gridColumn1.AppearanceHeader.Font.Name, (float)this.currentAdo.HeaderSize, FontStyle.Bold);
                }

                //cỡ chữ cell
                if (this.currentAdo != null && this.currentAdo.CellSize > 0)
                {
                    gridColumn1.AppearanceCell.Font = new System.Drawing.Font(gridColumn1.AppearanceCell.Font.Name, (float)this.currentAdo.CellSize, FontStyle.Bold);
                    gridColumn2.AppearanceCell.Font = new System.Drawing.Font(gridColumn1.AppearanceCell.Font.Name, (float)this.currentAdo.CellSize, FontStyle.Bold);
                }

                // màu chữ của phân trang
                //List<int> pagingForceColorCodes = WaitingScreenCFG.PAGING_FORCE_COLOR_CODES;
                //if (pagingForceColorCodes != null && pagingForceColorCodes.Count == 3)
                //{
                //    lblPageForGridReadyPatientGrid.ForeColor = System.Drawing.Color.FromArgb(pagingForceColorCodes[0], pagingForceColorCodes[1], pagingForceColorCodes[2]);
                //}

                //// Chiều cao của tiêu đề
                //long? fontSizeHeader = WaitingScreenCFG.NUM_ODER_DO_CAO_TITTLE;
                //if (fontSizeHeader != null)
                //{
                //    gridColumn1.AppearanceHeader.Font = new System.Drawing.Font("Arial", (float)fontSizeHeader);
                //    gridColumn1.AppearanceHeader.Font = new System.Drawing.Font("Arial", (float)fontSizeHeader);
                //}
                //// Kích thước của từng dòng STT và cửa.
                //long? fontSizeCell = WaitingScreenCFG.NUM_ODER_DO_CAO_NUM_ORDER;
                //if (fontSizeCell != null)
                //{
                //    gridColumn1.AppearanceCell.Font = new System.Drawing.Font("Arial", (float)fontSizeCell);
                //    gridColumn1.AppearanceCell.Font = new System.Drawing.Font("Arial", (float)fontSizeCell);
                //}
                // màu chữ tên tổ chức

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private static List<int> GetColorValues(string code)
        {
            List<int> result = new List<int>();
            try
            {
                //string value = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(code);
                //string pattern = ",";
                //Regex myRegex = new Regex(pattern);
                //string[] Codes = myRegex.Split(value);

                string[] Codes = code.Split(',');

                //if (String.IsNullOrEmpty(value)) throw new ArgumentNullException(code);

                if (!(Codes != null) || Codes.Length <= 0) throw new ArgumentNullException(code + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => code), code));
                foreach (var item in Codes) ///
                {
                    result.Add(Inventec.Common.TypeConvert.Parse.ToInt32(item));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void timerReload_Tick(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        LoadWaitingPatientToCallForTimer();

        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void LoadWaitingPatientToCallForTimer()
        {
            LoadWaitingPatientForWaitingScreen();
        }

        private void LoadWaitingPatientForWaitingScreen()
        {
            try
            {
                Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(executeThreadWaitingPatientToCall));
                try
                {
                    thread.Start();
                }
                catch (Exception ex)
                {
                    LogSystem.Error(ex);
                    thread.Abort();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void executeThreadWaitingPatientToCall()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { StartTheadWaitingPatientToCall(); }));
                }
                else
                {
                    StartTheadWaitingPatientToCall();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void StartTheadWaitingPatientToCall()
        {
            FillDataToGridWaitingPatient(hisRegisterGates);
        }

        private void FillDataToGridWaitingPatient(List<HisRegisterGateSDO> lstRegisterGateSDOs)
        {
            try
            {
                datas = new List<ServiceReqADO>();
                List<ServiceReqADO> data = new List<ServiceReqADO>();
                CommonParam paramCommon = new CommonParam();
                HisRegisterReqViewFilter filter = new HisRegisterReqViewFilter();
                filter.CALL_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Today);
                filter.ORDER_FIELD = "CALL_TIME";
                filter.ORDER_DIRECTION = "DESC";

                if (lstRegisterGateSDOs != null && lstRegisterGateSDOs.Count() > 0)
                    filter.REGISTER_GATE_IDs = lstRegisterGateSDOs.Select(o => o.ID).ToList();


                if (gridControlWaiting.DataSource != null)
                {
                    this.oldDatas = (List<ServiceReqADO>)gridControlWaiting.DataSource;
                }
                var rs = new BackendAdapter(paramCommon).Get<List<V_HIS_REGISTER_REQ>>("api/HisRegisterReq/GetView", ApiConsumers.MosConsumer, filter, paramCommon).OrderByDescending(o => o.CALL_TIME).ThenByDescending(o => o.ID).ToList();
                if (currentAdo != null && currentAdo.maxLine > 0)
                    rs = rs.Take(Convert.ToInt16(currentAdo.maxLine)).ToList();
                if (rs != null && rs.Count() > 0)
                {
                    foreach (var item in rs)
                    {
                        ServiceReqADO ado = new ServiceReqADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<ServiceReqADO>(ado, item);
                        var checkNhay = listNhay.FirstOrDefault(o => o.NUM_ORDER == item.NUM_ORDER && o.REGISTER_GATE_ID == item.REGISTER_GATE_ID);
                        if (checkNhay != null)
                            ado.countTimer = checkNhay.countTimer;
                        else
                            ado.countTimer = currentAdo.flickerTime * 5;
                        data.Add(ado);
                    }
                    var lstNew = new List<ServiceReqADO>();
                    this.datas = data;
                    if (currentAdo != null && currentAdo.reloadTime > 0)
                    {
                        foreach (var item in data)
                        {
                            if (this.oldDatas != null && !this.oldDatas.Exists(o => o.ID == item.ID && o.CALL_TIME == item.CALL_TIME))
                            {
                                listNhay.Add(item);
                                lstNew.Add(item);
                            }
                        }
                    }

                    gridControlWaiting.DataSource = data;
                    timerCall.Start();
                    listCall.AddRange(lstNew.Distinct().ToList().OrderBy(o => o.CALL_TIME).ThenBy(o => o.ID).ToList());
                }

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        List<ServiceReqADO> listCall = new List<ServiceReqADO>();
        bool IsCalling { get; set; }
        internal void CallPatientByNumOder(ServiceReqADO data, string callPatientFormat)
        {
            try
            {
                IsCalling = true;
                Inventec.Speech.SpeechPlayer.TypeSpeechCFG = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("Inventec.Speech.TypeSpeechCFG");

                var NUM_ORDER_STR = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(data.NUM_ORDER.ToString());

                List<string> KEY_SINGLE = new List<string>() { "NUM_ORDER_STR", "NUM_ORDER", "NUM_ORDER_DISPLAY", "GATE_NAME", "REGISTER_GATE_CODE", "REGISTER_GATE_NAME" };
                var strCallsplit = callPatientFormat.Split(new string[] { "<#", ";>" }, System.StringSplitOptions.RemoveEmptyEntries);
                if (strCallsplit.ToList().Count > 0)
                {
                    foreach (var word in strCallsplit)
                    {
                        var checkKey = KEY_SINGLE.FirstOrDefault(o => o == word.ToUpper());
                        if (checkKey == null || checkKey.Count() == 0)
                        {
                            var strWordsplit = word.Split(new string[] { ",", ";", ".", "-", ":", "/" }, System.StringSplitOptions.RemoveEmptyEntries);
                            foreach (var item in strWordsplit)
                            {
                                Inventec.Speech.SpeechPlayer.SpeakSingle(item.Trim());
                            }
                        }
                        else
                        {
                            switch (word)
                            {
                                case "NUM_ORDER_STR":
                                    Inventec.Speech.SpeechPlayer.SpeakSingle(NUM_ORDER_STR.Trim());
                                    break;
                                case "NUM_ORDER":
                                    Inventec.Speech.SpeechPlayer.Speak(data.NUM_ORDER);
                                    break;
                                case "NUM_ORDER_DISPLAY":
                                    Inventec.Speech.SpeechPlayer.SpeakSingle(string.Format("{0}{1}", data.REGISTER_GATE_CODE, NUM_ORDER_STR).Trim());
                                    break;
                                case "GATE_NAME":
                                    Inventec.Speech.SpeechPlayer.SpeakSingle(data.CALL_PLACE);
                                    break;
                                case "REGISTER_GATE_CODE":
                                    Inventec.Speech.SpeechPlayer.SpeakSingle(data.REGISTER_GATE_CODE.Trim());
                                    break;
                                case "REGISTER_GATE_NAME":
                                    Inventec.Speech.SpeechPlayer.SpeakSingle(data.REGISTER_GATE_NAME);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                listCall = listCall.Where(o => !string.Format("{0}{1}", o.REGISTER_GATE_CODE, Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(o.NUM_ORDER.ToString())).Equals(string.Format("{0}{1}", data.REGISTER_GATE_CODE, NUM_ORDER_STR))).ToList();
                Inventec.Common.Logging.LogSystem.Debug("CallPatientByNumOder___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listCall.Select(o => o.NUM_ORDER)), listCall.Select(o => o.NUM_ORDER)));
                currentCall = null;
                IsCalling = false;
            }
            catch (Exception ex)
            {
                IsCalling = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timerForScrollListPatient_Tick()
        {
            try
            {
                ScrollListPatientProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ScrollListPatientProcess()
        {
            try
            {
                LoadWaitingPatientToCallForTimer();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timerChangeColorRow_Tick()
        {
            try
            {
                gridControlWaiting.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewWaiting_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                var result = (ServiceReqADO)gridViewWaiting.GetRow(e.RowHandle);

                if (result != null && listNhay != null && listNhay.Count > 0)
                {
                    var checkNhay = listNhay.FirstOrDefault(o => o.NUM_ORDER == result.NUM_ORDER && o.REGISTER_GATE_ID == result.REGISTER_GATE_ID);
                    if (checkNhay != null)
                    {
                        List<int> gridpatientBodyForceColorCodes = WaitingScreenCFG.GRID_PATIENTS_BODY_FORCE_COLOR_CODES;
                        e.HighPriority = true;
                        if (result.countTimer % 2 == 0)
                        {
                            e.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 0, 0);
                            e.Appearance.ForeColor = Color.White;
                        }
                        else
                        {
                            List<int> colorTittle = new List<int>();
                            colorTittle = GetColorValues(currentAdo.backgroundColorSTT);
                            e.Appearance.BackColor = Color.White;
                            e.Appearance.ForeColor = System.Drawing.Color.FromArgb(gridpatientBodyForceColorCodes[0], gridpatientBodyForceColorCodes[1], gridpatientBodyForceColorCodes[2]);
                        }
                        result.countTimer--;
                        checkNhay.countTimer = result.countTimer;
                        if (result.countTimer == 0)
                        {
                            result.countTimer = currentAdo.flickerTime * 5;
                            listNhay = listNhay.Where(o => !(o.NUM_ORDER == result.NUM_ORDER && o.REGISTER_GATE_ID == result.REGISTER_GATE_ID)).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void gridViewWaiting_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {

            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ServiceReqADO pData = (ServiceReqADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "NUM_ORDER_DISPLAY")
                    {
                        if (!string.IsNullOrEmpty(pData.FORMAT))
                        {
                            string str = pData.NUM_ORDER.ToString();
                            for (int i = 0; i < pData.FORMAT.Length; i++)
                            {
                                if (str.Length >= pData.FORMAT.Length)
                                    break;
                                str = str.Insert(i, "0");
                            }
                            e.Value = pData.REGISTER_GATE_CODE + str;
                        }
                        else
                        {
                            e.Value = pData.NUM_ORDER;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timerMovingText_Tick()
        {
            try
            {

                this.Note = Note.Insert(Note.Length, Note.Substring(0, 1));
                this.Note = Note.Substring(1, Note.Length - 1);
                lblNote.Text = this.Note;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timerCall_Tick(object sender, EventArgs e)
        {
            try
            {
                if (listCall != null && listCall.Count > 0 && !IsCalling)
                {
                    if (!string.IsNullOrEmpty(currentAdo.urlVoice) && File.Exists(currentAdo.urlVoice))
                    {
                        System.Media.SoundPlayer snd = new System.Media.SoundPlayer(currentAdo.urlVoice);
                        snd.PlaySync();
                    }
                    currentCall = listCall[0];
                    Task ts = new Task(() => { CallPatientByNumOder(listCall[0], currentAdo.configNotify); });
                    ts.Start();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
