using DevExpress.XtraGrid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CallPatientExamV2
{
    public partial class frmWaitingScreen : Form
    {
        ServiceReqGateADO ServiceReqGateADO = new ServiceReqGateADO();
        Timer timerReload = new Timer();
        Timer timerCallPatient = new Timer();
        public frmWaitingScreen(ServiceReqGateADO ado)
        {
            InitializeComponent();
            ServiceReqGateADO = ado;
        }

        private void frmWaitingScreen_Load(object sender, EventArgs e)
        {
            ProcessShowUcRoom();
            timerReload.Interval = (int)ServiceReqGateADO.timeReload * 1000;
            timerReload.Tick += TimerReload_Tick;
            timerReload.Enabled = true;
            timerReload.Start();


            timerCallPatient.Interval = 500;
            timerCallPatient.Tick += TimerCall_Tick;
            timerCallPatient.Enabled = true;
            timerCallPatient.Start();
        }

        private void TimerCall_Tick(object sender, EventArgs e)
        {
            try
            {
                timerCallPatient.Stop();
                if (ChooseRoomForWaitingScreenProcess.StackServiceReqCall == null ||
                    !ChooseRoomForWaitingScreenProcess.StackServiceReqCall.Any(x => x.IsCalling))
                {
                    return;
                }
                var itemsToRemove = new List<ServiceReqCallADO>();
                foreach (var item in ChooseRoomForWaitingScreenProcess.StackServiceReqCall)
                {
                    CallPatient(item.ServiceReq);
                    item.IsCalling = false;
                    itemsToRemove.Add(item);
                }
                foreach (var item in itemsToRemove)
                {
                    ChooseRoomForWaitingScreenProcess.StackServiceReqCall.Remove(item);
                }
                timerCallPatient.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void CallPatient(HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                if (serviceReq == null)
                    return;
                Inventec.Speech.SpeechPlayer.TypeSpeechCFG = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("Inventec.Speech.TypeSpeechCFG");
                List<string> KEY_SINGLE = new List<string>() { "ROOM_NAME", "PATIENT_NAME", "YOB", "NUM_ORDER", "YOB_STR", "NUM_ORDER_STR", "ROOM_ADDRESS_SPLIT" };
                var strCallsplit = ServiceReqGateADO.configNotify.Split(new string[] { "<#", ";>" }, System.StringSplitOptions.RemoveEmptyEntries);
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
                                case "ROOM_NAME":
                                    string room = ServiceReqGateADO.roomGateSDOs.FirstOrDefault(o => o.ID == serviceReq.EXECUTE_ROOM_ID).ROOM_NAME;
                                    Inventec.Speech.SpeechPlayer.SpeakSingle(room);
                                    break;
                                case "PATIENT_NAME":
                                    Inventec.Speech.SpeechPlayer.Speak(serviceReq.TDL_PATIENT_NAME);
                                    break;
                                case "YOB":
                                    long year = Int64.Parse(serviceReq.TDL_PATIENT_DOB.ToString().Substring(0, 4));
                                    Inventec.Speech.SpeechPlayer.Speak(year);
                                    break;
                                case "NUM_ORDER":
                                    if (serviceReq.NUM_ORDER.HasValue)
                                        Inventec.Speech.SpeechPlayer.Speak(serviceReq.NUM_ORDER.Value);
                                    break;
                                case "YOB_STR":
                                    long yob = Int64.Parse(serviceReq.TDL_PATIENT_DOB.ToString().Substring(0, 4));
                                    Inventec.Speech.SpeechPlayer.Speak(Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(yob.ToString()));
                                    break;
                                case "NUM_ORDER_STR":
                                    if (serviceReq.NUM_ORDER.HasValue)
                                        Inventec.Speech.SpeechPlayer.Speak(Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(serviceReq.NUM_ORDER.ToString()));
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TimerReload_Tick(object sender, EventArgs e)
        {
            try
            {
                timerReload.Stop();
                LoadDataGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadDataGrid()
        {
            try
            {
                Task.Factory.StartNew(ExecuteThreadLoadDataGrid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ExecuteThreadLoadDataGrid()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { FillDataToGridControl(); }));
                }
                else
                {
                    FillDataToGridControl();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void FillDataToGridControl()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();

                filter.EXECUTE_ROOM_IDs = ServiceReqGateADO.roomGateSDOs.Select(s => s.ID).ToList();
                filter.HAS_EXECUTE = true;
                filter.NOT_IN_SERVICE_REQ_TYPE_IDs = new List<long> {
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G};

                filter.INTRUCTION_DATE_FROM = Inventec.Common.DateTime.Get.StartDay();
                filter.INTRUCTION_DATE_TO = Inventec.Common.DateTime.Get.EndDay();

                filter.SERVICE_REQ_STT_IDs = new List<long>() {
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL};

                var result = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, param);

                timerReload.Start();
                if (result != null && result.Count > 0)
                {
                    var dicRoom = result.GroupBy(k => k.EXECUTE_ROOM_ID).ToDictionary(k => k.Key, v => v.ToList());
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new MethodInvoker(delegate
                        {
                            UpdateUc(dicRoom);
                        }));
                    }
                    else
                    {
                        UpdateUc(dicRoom);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateUc(Dictionary<long, List<HIS_SERVICE_REQ>> dicRoom)
        {

            try
            {
                foreach (var room in dicRoom)
                {
                    foreach (Control ctrl in layoutControl1.Controls)
                    {
                        if (ctrl is UcRoom)
                        {
                            var ucRoom = ctrl as UcRoom;
                            if (ucRoom.Tag != null && ucRoom.Tag.ToString() == room.Key.ToString())
                            {
                                ucRoom.ReloadData(room.Value);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void ProcessShowUcRoom()
        {
            try
            {
                int ucCount = ServiceReqGateADO.roomGateSDOs.Count;
                int ucWidth = layoutControl1.Width / ucCount;
                int xOffset = 0;

                for (int i = 0; i < ucCount; i++)
                {
                    UcRoom uc = new UcRoom(ServiceReqGateADO);
                    uc.Tag = ServiceReqGateADO.roomGateSDOs[i].ID;
                    uc.Width = ucWidth;
                    uc.Height = layoutControl1.Height;
                    uc.Location = new Point(xOffset, 0);
                    layoutControl1.Controls.Add(uc);
                    xOffset += ucWidth;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
