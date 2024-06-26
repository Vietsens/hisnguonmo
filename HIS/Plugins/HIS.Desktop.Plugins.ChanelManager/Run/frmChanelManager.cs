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
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.PubSub.ADO;
using HIS.Desktop.Plugins.ChanelManager.ADO;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Resources;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ChanelManager.Run
{
    public partial class frmChanelManager : HIS.Desktop.Utility.FormBase
    {
        Socket _client;
        EndPoint _remoteEndPoint;
        int _noOfEventsFired = 0;
        const string ChanelHisProName = "HISPROCHANEL";
        const string SubscribeCommand = "Publish";
        List<CommandADO> commandADOs = new List<CommandADO>();

        public frmChanelManager(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);

                this.Text = module != null ? module.text : this.Text;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void frmChooseRoom_Load(object sender, EventArgs e)
        {
            try
            {
                string serviceInfo = HisConfigs.Get<String>("HIS.Desktop.PubSubServerInfo");
                if (!String.IsNullOrEmpty(serviceInfo))
                {
                    var psArr = serviceInfo.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    if (psArr != null && psArr.Count() == 3)
                    {
                        try
                        {
                            IPAddress serverIPAddress = IPAddress.Parse(psArr[0]);
                            int serverPort = Convert.ToInt32(psArr[2]);

                            _client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                            _remoteEndPoint = new IPEndPoint(serverIPAddress, serverPort);

                        }
                        catch (Exception ex)
                        {
                            WaitingManager.Hide();
                            LogSystem.Error(ex);
                        }
                        //txtChanel.Text = ChanelHisProName;
                        //txtCommand.Text = SubscribeCommand;
                    }
                }
                InitCommand();
                cboEventData.EditValue = "RestartApp";
                cboEventData.Focus();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void InitCommand()
        {
            try
            {
                this.commandADOs.Add(new CommandADO() { CommandName = "RestartApp", Description = "Khởi động lại phần mềm" });
                this.commandADOs.Add(new CommandADO() { CommandName = "ResetCache", Description = "Làm mới cache" });

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("Description", "", 100, 1));
                //columnInfos.Add(new ColumnInfo("Description", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("Description", "CommandName", columnInfos, false, 100);
                ControlEditorLoader.Load(this.cboEventData, this.commandADOs, controlEditorADO);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ChanelManager.Resources.Lang", typeof(HIS.Desktop.Plugins.ChanelManager.Run.frmChanelManager).Assembly);

                //////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                //this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmChooseRoom.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmChooseRoom.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.cboDepartment.Properties.NullText = Inventec.Common.Resource.Get.Value("frmChooseRoom.cboDepartment.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.btnChoice.Text = Inventec.Common.Resource.Get.Value("frmChooseRoom.btnChoice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.cboBranch.Properties.NullText = Inventec.Common.Resource.Get.Value("frmChooseRoom.cboBranch.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmChooseRoom.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.gridColumnCheck.Caption = Inventec.Common.Resource.Get.Value("frmChooseRoom.gridColumnCheck.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.gridColumnRoomCode.Caption = Inventec.Common.Resource.Get.Value("frmChooseRoom.gridColumnRoomCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.gridColumnRoomName.Caption = Inventec.Common.Resource.Get.Value("frmChooseRoom.gridColumnRoomName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.gridColumnRoomTypeName.Caption = Inventec.Common.Resource.Get.Value("frmChooseRoom.gridColumnRoomTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.gridColumnDepartmentName.Caption = Inventec.Common.Resource.Get.Value("frmChooseRoom.gridColumnDepartmentName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciBranch.Text = Inventec.Common.Resource.Get.Value("frmChooseRoom.lciBranch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmChooseRoom.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmChooseRoom.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.bar2.Text = Inventec.Common.Resource.Get.Value("frmChooseRoom.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.bbtnCtrlS.Caption = Inventec.Common.Resource.Get.Value("frmChooseRoom.bbtnCtrlS.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                if (this.currentModuleBase != null)
                {
                    this.Text = this.currentModuleBase.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtChanel_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboEventData.Focus();
                    cboEventData.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private async void btnChoice_Click(object sender, EventArgs e)
        {
            try
            {
                var eventData = cboEventData.EditValue;
                if (eventData == null)
                {
                    MessageManager.Show("Chưa chọn lệnh điều khiển");
                    return;
                }

                //string ips = "";
                //IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
                //if (localIPs != null && localIPs.Count() > 0)
                //{
                //    ips = String.Join(",", localIPs.Where(k => k.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).Select(k => k.ToString()));
                //    if (ips.EndsWith(","))
                //    {
                //        ips = ips.Substring(0, ips.Length - 2);
                //    }
                //}
                PubSubADO ado = new PubSubADO();
                if (eventData.ToString() == "RestartApp")
                {
                    ado.actionType = ActionType.RESET;

                }
                else if (eventData.ToString() == "ResetCache")
                {
                    ado.actionType = ActionType.REFRESH_CACHE;
                    var A = HIS.Desktop.XmlCacheMonitor.CacheMonitorKeyStore.Get();
                    var B = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<HIS_CACHE_MONITOR>>("api/HisCacheMonitor/Get", ApiConsumer.ApiConsumers.MosConsumer, new HisCacheMonitorFilter(), null);
                    if (B == null)
                        B = new List<HIS_CACHE_MONITOR>();
                    foreach (var item in A)
                    {
                        string api = "";
                        HIS_CACHE_MONITOR cacheMonitor = new HIS_CACHE_MONITOR();
                        if (B.Count() == 0 || !B.Exists(o => o.DATA_NAME == item.CacheMonitorKeyName))
                        {
                            api = "api/HisCacheMonitor/Create";
                            cacheMonitor.DATA_NAME = item.CacheMonitorKeyName;
                        }
                        else
                        {
                            cacheMonitor = B.FirstOrDefault(o => o.DATA_NAME == item.CacheMonitorKeyName);
                            api = "api/HisCacheMonitor/Update";
                        }

                        cacheMonitor.IS_RELOAD = 0;
                        var rs = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Post<HIS_CACHE_MONITOR>(api, ApiConsumer.ApiConsumers.MosConsumer, cacheMonitor, null);

                        if (rs != null && !B.Exists(o => o.ID == rs.ID))
                            B.Add(rs);

                    }
                }
                ado.Message = txtMessage.Text;
                ado.ExecuteTime = DateTime.Now;
                ado.Uuid = Guid.NewGuid().ToString();

                var success = await LocalStorage.PubSub.PubSubAction.SendMessage(ado);
                MessageManager.Show(this, new CommonParam(), success);
                //this.Close();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void frmChooseRoom_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        #region Shortcut
        private void bbtnCtrlS_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.btnChoice_Click(null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        #endregion

        private void cboEventData_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    //string eventData = cboEventData.EditValue.ToString();
                    //if (eventData == "UploadLog")
                    //{
                    //    lciFortxtClientIp.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    //    txtClientIp.Text = "";
                    //    txtClientIp.Focus();
                    //    txtClientIp.SelectAll();
                    //}
                    //else
                    //{
                    //    lciFortxtClientIp.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
