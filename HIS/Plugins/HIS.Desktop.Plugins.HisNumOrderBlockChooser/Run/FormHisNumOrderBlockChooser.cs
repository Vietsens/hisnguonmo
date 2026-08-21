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
using DevExpress.XtraTab;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.HisNumOrderBlockChooser.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisNumOrderBlockChooser.Run
{
    public partial class FormHisNumOrderBlockChooser : FormBase
    {
        private Inventec.Desktop.Common.Modules.Module currentModule;
        private List<MOS.EFMODEL.DataModels.V_HIS_ROOM> ListRoom;
        private NumOrderBlockChooserADO NumOrderBlockChooser;
        private Action<ResultChooseNumOrderBlockADO> DelegateChooseData;
        private HisNumOrderBlockSDO NumOrderBlock;
        bool? isNeedTimeString = null;
        long? timeSelected = null;
        string HourString = "0000";

        //Giu cho khung gio kham (viec 54282)
        private const string URI_HOLD = "api/HisNumOrderBlock/Hold";
        private const string URI_RELEASE = "api/HisNumOrderBlock/Release";
        private const string URI_HOLD_SETTING = "api/HisNumOrderBlock/GetHoldSetting";
        private int holdMinute = 0;
        private long? holdingNumOrderIssueId = null;
        private long? holdExpireTime = null;
        private long serverTimeAtHold = 0;
        private DateTime holdStartedAt = DateTime.MinValue;
        private bool isChooseConfirmed = false;
        private System.Windows.Forms.Timer timerHold = null;
        private string baseTitle = "";
        private int tickCount = 0;

        public FormHisNumOrderBlockChooser(Inventec.Desktop.Common.Modules.Module currentModule, NumOrderBlockChooserADO numOrderBlockChooser, bool? _isNeedTime = null)
            : base(currentModule)
        {
            // TODO: Complete member initialization
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                if (numOrderBlockChooser != null)
                {
                    this.ListRoom = numOrderBlockChooser.ListRoom;
                    this.NumOrderBlockChooser = numOrderBlockChooser;
                    this.DelegateChooseData = numOrderBlockChooser.DelegateChooseData;
                    this.isNeedTimeString = _isNeedTime;
                }
                SetIcon();
                this.Text = currentModule.text;
                //Nha cho khi dong man ma chua bam Chon (viec 54282)
                this.FormClosing += new FormClosingEventHandler(FormHisNumOrderBlockChooser_FormClosing);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormHisNumOrderBlockChooser_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                InitCboRoom();
                SetDefaultData();
                LoadDataNumOrder();
                InitHold();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Giu cho khung gio kham (viec 54282)

        /// <summary>
        /// Doc so phut giu cho tu may chu. Bang 0 = tinh nang tat, man hinh chay y nhu truoc.
        /// </summary>
        private void InitHold()
        {
            try
            {
                this.baseTitle = this.Text;
                CommonParam param = new CommonParam();
                HisNumOrderBlockHoldSettingSDO setting = new BackendAdapter(param)
                    .Get<HisNumOrderBlockHoldSettingSDO>(URI_HOLD_SETTING, ApiConsumers.MosConsumer, (long)0, param);
                this.holdMinute = setting != null ? setting.HoldMinute : 0;

                if (this.holdMinute > 0)
                {
                    this.timerHold = new System.Windows.Forms.Timer();
                    this.timerHold.Interval = 1000;
                    this.timerHold.Tick += new EventHandler(timerHold_Tick);
                    this.timerHold.Start();
                }
            }
            catch (Exception ex)
            {
                //Khong lay duoc cau hinh thi coi nhu tat tinh nang, khong chan nguoi dung
                this.holdMinute = 0;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timerHold_Tick(object sender, EventArgs e)
        {
            try
            {
                //Chua giu o nao: dinh ky nap lai luoi de thay cho nguoi khac vua giu.
                //Chi lam khi chua chon gi de khong pha lua chon dang co.
                if (!this.holdingNumOrderIssueId.HasValue)
                {
                    this.tickCount++;
                    if (this.tickCount >= 15)
                    {
                        this.tickCount = 0;
                        LoadDataNumOrder();
                    }
                    return;
                }

                if (!this.holdExpireTime.HasValue)
                {
                    return;
                }

                //Dem nguoc theo gio may chu luc giu, khong phu thuoc dong ho may tram
                long secondRemain = GetSecondRemain();
                if (secondRemain > 0)
                {
                    this.Text = string.Format("{0} — đang giữ chỗ {1:00}:{2:00}", this.baseTitle, secondRemain / 60, secondRemain % 60);
                }
                else
                {
                    //Het han giu: bo lua chon va nap lai luoi
                    this.Text = this.baseTitle;
                    this.holdingNumOrderIssueId = null;
                    this.holdExpireTime = null;
                    this.NumOrderBlock = null;
                    this.lblStt.Text = "0";
                    //Xoa luon gio da chon, neu khong lan bam Chon sau se lay gio cu (viec 54282)
                    this.HourString = "0000";
                    XtraMessageBox.Show("Đã quá thời gian giữ chỗ. Vui lòng chọn lại khung giờ khám.");
                    LoadDataNumOrder();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private long GetSecondRemain()
        {
            try
            {
                if (!this.holdExpireTime.HasValue || this.serverTimeAtHold <= 0)
                {
                    return 0;
                }
                DateTime? expire = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.holdExpireTime.Value);
                DateTime? atHold = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.serverTimeAtHold);
                if (!expire.HasValue || !atHold.HasValue)
                {
                    return 0;
                }
                double total = expire.Value.Subtract(atHold.Value).TotalSeconds;
                double elapsed = DateTime.Now.Subtract(this.holdStartedAt).TotalSeconds;
                return (long)Math.Floor(total - elapsed);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return 0;
            }
        }

        /// <summary>
        /// Giu block vua chon. Neu dang giu o khac thi nha o cu trong cung mot lan goi.
        /// </summary>
        private bool DoHold(HisNumOrderBlockSDO block)
        {
            try
            {
                if (this.holdMinute <= 0 || block == null || cboRoom.EditValue == null)
                {
                    return true;
                }

                CommonParam param = new CommonParam();
                HisNumOrderBlockHoldSDO sdo = new HisNumOrderBlockHoldSDO();
                sdo.NumOrderBlockId = block.NUM_ORDER_BLOCK_ID;
                sdo.IssueDate = Inventec.Common.TypeConvert.Parse.ToInt64(dtDate.DateTime.ToString("yyyyMMdd") + "000000");
                sdo.RoomId = Inventec.Common.TypeConvert.Parse.ToInt64(cboRoom.EditValue.ToString());
                sdo.HoldChannel = "HIS";
                sdo.ReleaseNumOrderIssueId = this.holdingNumOrderIssueId;

                HisNumOrderBlockHoldResultSDO result = new BackendAdapter(param)
                    .Post<HisNumOrderBlockHoldResultSDO>(URI_HOLD, ApiConsumers.MosConsumer, sdo, param);

                if (result == null)
                {
                    string message = GetMessage(param);
                    XtraMessageBox.Show(!string.IsNullOrWhiteSpace(message) ? message : "Không giữ được khung giờ khám. Vui lòng chọn khung giờ khác.");
                    LoadDataNumOrder();
                    return false;
                }

                this.holdingNumOrderIssueId = result.NumOrderIssueId;
                this.holdExpireTime = result.HoldExpireTime;
                this.serverTimeAtHold = result.ServerTime;
                this.holdStartedAt = DateTime.Now;

                //Ve lai luoi de o vua nha doi mau ngay. Dua ra khoi su kien click roi moi nap
                //vi nap lai se huy chinh cai nut dang xu ly su kien.
                this.BeginInvoke(new Action(() => { try { LoadDataNumOrder(); } catch (Exception exr) { Inventec.Common.Logging.LogSystem.Error(exr); } }));
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// Nha block dang giu (doi o khac, dong man, huy chon)
        /// </summary>
        private void DoRelease()
        {
            try
            {
                if (!this.holdingNumOrderIssueId.HasValue || this.holdingNumOrderIssueId.Value <= 0)
                {
                    return;
                }

                CommonParam param = new CommonParam();
                HisNumOrderBlockReleaseSDO sdo = new HisNumOrderBlockReleaseSDO();
                sdo.NumOrderIssueId = this.holdingNumOrderIssueId.Value;
                new BackendAdapter(param).Post<bool>(URI_RELEASE, ApiConsumers.MosConsumer, sdo, param);

                this.holdingNumOrderIssueId = null;
                this.holdExpireTime = null;
                this.Text = this.baseTitle;
            }
            catch (Exception ex)
            {
                //Nha that bai thi cung khong chan nguoi dung: het han se tu nha
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetMessage(CommonParam param)
        {
            try
            {
                if (param != null && param.Messages != null && param.Messages.Count > 0)
                {
                    return string.Join(Environment.NewLine, param.Messages);
                }
                return "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return "";
            }
        }

        private void FormHisNumOrderBlockChooser_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (this.timerHold != null)
                {
                    this.timerHold.Stop();
                }
                //Chua bam Chon ma dong man thi tra lai cho cho nguoi khac
                if (!this.isChooseConfirmed)
                {
                    DoRelease();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

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

        private void SetCaptionByLanguageKey()
        {
            try
            {////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisNumOrderBlockChooser.Resources.Lang", typeof(HIS.Desktop.Plugins.HisNumOrderBlockChooser.Run.FormHisNumOrderBlockChooser).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormHisNumOrderBlockChooser.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblStt.Text = Inventec.Common.Resource.Get.Value("FormHisNumOrderBlockChooser.lblStt.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnChoose.Text = Inventec.Common.Resource.Get.Value("FormHisNumOrderBlockChooser.btnChoose.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.xtraTabPage1.Text = Inventec.Common.Resource.Get.Value("FormHisNumOrderBlockChooser.xtraTabPage1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboRoom.Properties.NullText = Inventec.Common.Resource.Get.Value("FormHisNumOrderBlockChooser.cboRoom.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRoom.Text = Inventec.Common.Resource.Get.Value("FormHisNumOrderBlockChooser.lciRoom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDate.Text = Inventec.Common.Resource.Get.Value("FormHisNumOrderBlockChooser.lciDate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciStt.Text = Inventec.Common.Resource.Get.Value("FormHisNumOrderBlockChooser.lciStt.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitCboRoom()
        {
            try
            {
                LoadDataToCombo(cboRoom, this.ListRoom, "ID", "", "ROOM_NAME");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCombo(GridLookUpEdit cbo, object data, string valueMember, string code, string name)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                int with = 0;
                if (!String.IsNullOrWhiteSpace(code))
                {
                    columnInfos.Add(new ColumnInfo(code, "", 150, 1));
                    with += 150;
                }

                columnInfos.Add(new ColumnInfo(name, "", 250, 2));
                with += 250;
                ControlEditorADO controlEditorADO = new ControlEditorADO(name, valueMember, columnInfos, false, with);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultData()
        {
            try
            {
                this.dtDate.EditValue = DateTime.Now;

                if (this.NumOrderBlockChooser != null)
                {
                    this.cboRoom.Enabled = !this.NumOrderBlockChooser.DisableRoom;
                    this.dtDate.Enabled = !this.NumOrderBlockChooser.DisableDate;
                    this.btnNext.Enabled = !this.NumOrderBlockChooser.DisableDate;
                    this.btnPrevious.Enabled = !this.NumOrderBlockChooser.DisableDate;

                    if (this.NumOrderBlockChooser.DefaultRoomId.HasValue)
                    {
                        this.cboRoom.EditValue = this.NumOrderBlockChooser.DefaultRoomId.Value;
                    }
                    //else if (this.ListRoom != null && this.ListRoom.Count > 0)
                    //{
                    //    this.cboRoom.EditValue = this.ListRoom.First().ID;
                    //}

                    if (this.NumOrderBlockChooser.DefaultDate.HasValue && this.NumOrderBlockChooser.DefaultDate.Value > 0)
                    {
                        this.dtDate.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.NumOrderBlockChooser.DefaultDate.Value);
                    }
                    if (this.NumOrderBlockChooser.SelectedTime.HasValue)
                    {
                        this.timeSelected = this.NumOrderBlockChooser.SelectedTime;
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Error("NumOrderBlockChooser null");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataNumOrder()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisNumOrderBlockOccupiedStatusFilter filter = new HisNumOrderBlockOccupiedStatusFilter();

                if (dtDate.EditValue != null && dtDate.DateTime != DateTime.MinValue)
                {
                    filter.ISSUE_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(dtDate.DateTime.ToString("yyyyMMdd") + "000000");
                }

                if (cboRoom.EditValue != null)
                {
                    filter.ROOM_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboRoom.EditValue.ToString());
                }

                var apiResult = new BackendAdapter(param).Get<List<HisNumOrderBlockSDO>>("api/HisNumOrderBlock/GetOccupiedStatus", ApiConsumers.MosConsumer, filter, param);

                ProcessCreateTab(apiResult);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessCreateTab(List<HisNumOrderBlockSDO> dataNumOrder)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataNumOrder), dataNumOrder));
                xtraTabTotal.TabPages.Clear();
                if (dataNumOrder != null && dataNumOrder.Count > 0)
                {
                    var groupTime = dataNumOrder.GroupBy(o => new { o.ROOM_TIME_ID, o.ROOM_TIME_NAME, o.ROOM_TIME_FROM, o.ROOM_TIME_TO }).ToList();
                    foreach (var times in groupTime)
                    {
                        XtraTabPage tab = new XtraTabPage();
                        tab.Text = !String.IsNullOrWhiteSpace(times.Key.ROOM_TIME_NAME) ? times.Key.ROOM_TIME_NAME : string.Format("{0} - {1}", Base.GlobalVariablesProcess.GenerateHour(times.Key.ROOM_TIME_FROM), Base.GlobalVariablesProcess.GenerateHour(times.Key.ROOM_TIME_TO));
                        //Luat "da qua gio" chi ap cho hom nay; ngay tuong lai moi o deu con hieu luc (viec 54282)
                        bool isToday = (dtDate.EditValue != null && dtDate.DateTime != DateTime.MinValue)
                                        ? dtDate.DateTime.Date == DateTime.Now.Date
                                        : true;

                        UCTimes uc = new UCTimes(times.ToList(), SelectNumOrder,
                                                 (timeSelected != null && timeSelected != 0) ? timeSelected : null,
                                                 isToday);
                        uc.Dock = DockStyle.Fill;
                        tab.Controls.Add(uc);
                        xtraTabTotal.TabPages.Add(tab);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SelectNumOrder(TimeADO data)
        {
            try
            {
                //Khung gio cau hinh sai gio/phut (vi du 13:60) thi chan ngay tu luc bam,
                //khong giu cho va khong tra ve man cha (viec 54282)
                if (data != null && !IsHourHopLe(data.HOUR_STR))
                {
                    XtraMessageBox.Show(string.Format("Khung giờ khám {0} đang được cấu hình sai giờ/phút. Vui lòng chọn khung giờ khác hoặc báo quản trị hệ thống.",
                        !string.IsNullOrWhiteSpace(data.HOUR_STR) ? data.HOUR_STR : "này"));
                    return;
                }

                //Giu cho ngay khi bam chon o gio, khong doi toi luc bam Chon (viec 54282)
                if (data != null && !DoHold(data))
                {
                    return;
                }

                this.NumOrderBlock = data;
                if (data != null)
                {
                    this.lblStt.Text = data.NUM_ORDER + "";
                    this.HourString = data.HOUR_STR.Replace(":", "");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Chuoi gio dang "HH:mm" (hoac "HHmm") co hop le khong (viec 54282).
        /// HIS_NUM_ORDER_BLOCK.FROM_TIME tung bi sinh sai thanh 1360, 1390, 1480 - phut &gt; 59.
        /// Nhung gia tri do ghep lai thanh so 14 chu so vo nghia, ham doi so sang ngay gio
        /// tra ve null va o "TG hen kham" cua man cha bi xoa trang.
        /// </summary>
        private bool IsHourHopLe(string hourStr)
        {
            int gio;
            int phut;
            return TachGioPhut(hourStr, out gio, out phut);
        }

        private bool TachGioPhut(string hourStr, out int gio, out int phut)
        {
            gio = 0;
            phut = 0;
            try
            {
                if (string.IsNullOrWhiteSpace(hourStr))
                {
                    return false;
                }
                string so = hourStr.Replace(":", "");
                if (so.Length < 4)
                {
                    return false;
                }
                if (!int.TryParse(so.Substring(0, 2), out gio))
                {
                    return false;
                }
                if (!int.TryParse(so.Substring(2, 2), out phut))
                {
                    return false;
                }
                return gio >= 0 && gio <= 23 && phut >= 0 && phut <= 59;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private void cboRoom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                BoLuaChonHienTai();
                LoadDataNumOrder();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtDate_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                BoLuaChonHienTai();
                LoadDataNumOrder();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Doi phong hoac doi ngay thi o gio dang chon khong con y nghia: nha cho da giu
        /// va xoa gio cu, neu khong lan bam Chon sau se tra ve gio cua ngay/phong truoc (viec 54282).
        /// </summary>
        private void BoLuaChonHienTai()
        {
            try
            {
                DoRelease();
                this.NumOrderBlock = null;
                this.HourString = "0000";
                if (this.lblStt != null)
                {
                    this.lblStt.Text = "0";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtDate.Enabled)
                {
                    if (dtDate.EditValue != null && dtDate.DateTime != DateTime.MinValue)
                    {
                        dtDate.EditValue = dtDate.DateTime.AddDays(-1);
                    }
                    else
                    {
                        dtDate.EditValue = DateTime.Now.AddDays(-1);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtDate.Enabled)
                {
                    if (dtDate.EditValue != null && dtDate.DateTime != DateTime.MinValue)
                    {
                        dtDate.EditValue = dtDate.DateTime.AddDays(1);
                    }
                    else
                    {
                        dtDate.EditValue = DateTime.Now.AddDays(1);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.DelegateChooseData == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("lỗi tích hợp module DelegateChooseData null");
                    return;
                }

                if (cboRoom.EditValue == null)
                {
                    XtraMessageBox.Show("Bạn chưa chọn phòng.");
                    return;
                }

                if (this.NumOrderBlock == null)
                {
                    XtraMessageBox.Show("Bạn chưa chọn số thứ tự.");
                    return;
                }

                ResultChooseNumOrderBlockADO data = new ResultChooseNumOrderBlockADO();
                data.Date = Inventec.Common.TypeConvert.Parse.ToInt64(dtDate.DateTime.ToString("yyyyMMdd") + "000000");
                if (isNeedTimeString == true)
                {
                    //Dung DateTime that thay vi ghep chuoi: gio sai (13:60) tung cho ra so
                    //20260910136000 khong doc nguoc lai duoc, man cha gan null va o ngay hen
                    //bi trong ma khong bao gi (viec 54282)
                    int gio;
                    int phut;
                    if (!TachGioPhut(this.HourString, out gio, out phut))
                    {
                        Inventec.Common.Logging.LogSystem.Warn("HOLD54282 HourString khong hop le: " + this.HourString);
                        XtraMessageBox.Show(string.Format("Khung giờ khám {0} đang được cấu hình sai giờ/phút. Vui lòng chọn khung giờ khác hoặc báo quản trị hệ thống.",
                            !string.IsNullOrWhiteSpace(this.HourString) ? this.HourString : "này"));
                        return;
                    }
                    DateTime gioHen = dtDate.DateTime.Date.AddHours(gio).AddMinutes(phut);
                    data.Date = Inventec.Common.TypeConvert.Parse.ToInt64(gioHen.ToString("yyyyMMddHHmmss"));
                }
                //data.SelectedTime = NumOrderBlockChooser.SelectedTime;
                data.RoomId = Inventec.Common.TypeConvert.Parse.ToInt64(cboRoom.EditValue.ToString());
                data.NumOrderBlock = this.NumOrderBlock;
                //Chuyen ma giu cho sang man cha de gui kem luc luu lich kham (viec 54282)
                data.NumOrderIssueId = this.holdingNumOrderIssueId;
                data.HoldExpireTime = this.holdExpireTime;
                data.NumOrder = this.NumOrderBlock != null ? (long?)this.NumOrderBlock.NUM_ORDER : null;

                //Da chot lua chon: khong nha cho khi dong man
                this.isChooseConfirmed = true;

                DelegateChooseData(data);
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
