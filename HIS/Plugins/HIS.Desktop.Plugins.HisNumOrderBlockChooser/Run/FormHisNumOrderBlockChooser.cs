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
                        if (timeSelected != null && timeSelected != 0)
                        {
                            UCTimes uc = new UCTimes(times.ToList(), SelectNumOrder, timeSelected);
                            uc.Dock = DockStyle.Fill;
                            tab.Controls.Add(uc);
                        }
                        else
                        {
                            UCTimes uc = new UCTimes(times.ToList(), SelectNumOrder);
                            uc.Dock = DockStyle.Fill;
                            tab.Controls.Add(uc);
                        }
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

        private void cboRoom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
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
                LoadDataNumOrder();
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
                    data.Date = Inventec.Common.TypeConvert.Parse.ToInt64(dtDate.DateTime.ToString("yyyyMMdd") + HourString + "00");
                }
                //data.SelectedTime = NumOrderBlockChooser.SelectedTime;
                data.RoomId = Inventec.Common.TypeConvert.Parse.ToInt64(cboRoom.EditValue.ToString());
                data.NumOrderBlock = this.NumOrderBlock;


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
