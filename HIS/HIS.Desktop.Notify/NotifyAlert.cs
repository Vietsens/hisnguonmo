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
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using SDA.EFMODEL.DataModels;
using SDA.Filter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraBars.Alerter;
using System.Threading;
using DevExpress.Utils.About;

namespace HIS.Desktop.Notify
{
    public partial class NotifyAlert : ApplicationContext
    {
        string sdaBaseUri = "";
        string applicationCode = "";
        string tokenCode = "";
        int count = 0;
        int optionNotify = 0;
        int dem = 0;
        string loginname = "";
        frmNotify notifyAlert;
        bool isShowForm = false;
        CommonParam param = null;
        string departmentCode = null;
        string branchName = null;
        DevExpress.XtraBars.Alerter.AlertControl alertControl = new AlertControl();
        List<NotifyADO> listNotifyADO = new List<NotifyADO>();
        //List<NotifyADO> notifyAllow;
        NotifyADO itemNotify = null;
        NotifyIcon notify = new NotifyIcon();
        AlertCustom alert = new AlertCustom();
        int w = 0;
        int h = 0;
        public NotifyAlert(string _cmdLn)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Error("NotifyAlert_____");
                notify.Visible = true;
                notify.BalloonTipTitle = "Thông báo";
                notify.BalloonTipIcon = ToolTipIcon.Info;
                notify.DoubleClick += Notify_Click;
                notify.BalloonTipClicked += BalloonTip_Clicked;
                alertControl.AutoFormDelay = 5000;
                alertControl.FormShowingEffect = DevExpress.XtraBars.Alerter.AlertFormShowingEffect.SlideHorizontal;
                alertControl.ShowCloseButton = false;
                alertControl.BeforeFormShow += AlertControl_BeforeFormShow;
                alertControl.Buttons.Clear();
                alertControl.FormClosing += new AlertFormClosingEventHandler(alertControl_FormClosing);
                alertControl.FormLoad += new DevExpress.XtraBars.Alerter.AlertFormLoadEventHandler(alertControl_FormLoad);
                int time = 0;
                //"SdaBaseUri|http://sda.12c.vn|ApplicationCode|HIS|Time|3000|Count|3|Loginname|dunglh"
                ProcessCmd(_cmdLn, ref sdaBaseUri, ref tokenCode, ref applicationCode, ref time, ref count, ref loginname, ref optionNotify, ref departmentCode, ref branchName);
                if (time > 0)
                {
                    System.Windows.Forms.Timer timerNotify = new System.Windows.Forms.Timer();
                    timerNotify.Interval = time;
                    timerNotify.Tick += timerNotify_Tick;
                    timerNotify.Start();
                    timerNotify_Tick(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timerNotify_Tick(object sender, EventArgs e)
        {
            try
            {
                //bool check = false;
                var today = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);

                List<NotifyADO> listAdo = new List<NotifyADO>();
                List<NotifyADO> newNotifys = new List<NotifyADO>();

                Inventec.Common.WebApiClient.ApiConsumer sdaConsumer = new Inventec.Common.WebApiClient.ApiConsumer(sdaBaseUri, tokenCode, applicationCode);

                this.param = new CommonParam();
                this.param.Limit = 20;
                this.param.Start = 0;
                SdaNotifyFilter filter = new SdaNotifyFilter();
                filter.IS_ACTIVE = 1;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.WATCHED = false;
                filter.HAS_RECEIVER_LOGINNAME_OR_NULL = true;
                filter.NOW_TIME = Inventec.Common.DateTime.Get.Now().Value;
                filter.RECEIVER_DEPARTMENT_CODES_OR_NULL = departmentCode;
                filter.RECEIVER_LOGINNAMES_EXACT_OR_NULL = loginname;
                LogSystem.Debug(LogUtil.TraceData("Filter", filter));
                var rs = new BackendAdapter(this.param).GetRO<List<SDA_NOTIFY>>("/api/SdaNotify/Get", sdaConsumer, filter, this.param);
                int totalCount = 0;
                LogSystem.Debug(LogUtil.TraceData("RO", rs));
                if (rs != null && rs.Data != null && rs.Data.Count > 0)
                {
                    foreach (var item in rs.Data)
                    {
                        NotifyADO ado = new NotifyADO(item);
                        ado.SetIsRead(loginname);
                        listAdo.Add(ado);
                    }

                    newNotifys = listAdo.Where(o => !o.Status && !listNotifyADO.Any(a => a.ID == o.ID)).ToList();
                    listNotifyADO = listAdo;
                    if (newNotifys != null && newNotifys.Count > 0)
                    {
                        dem = 0;
                        LogSystem.Debug(LogUtil.TraceData("newNotifys.Count", newNotifys.Count));
                    }
                    if (rs.Param != null && rs.Param.Count.HasValue)
                    {
                        totalCount = rs.Param.Count ?? 0;
                        this.param = rs.Param;
                    }
                    else
                    {
                        totalCount = listAdo.Count;
                    }
                }
                else
                {
                    listNotifyADO = new List<NotifyADO>();
                }
                LogSystem.Debug(LogUtil.TraceData("listNotifyADO.Count", listNotifyADO.Count));
                if (dem < count)
                {
                    if (optionNotify == 1)
                    {
                        if (newNotifys != null && newNotifys.Count > 0)
                        {
                            foreach (var item in newNotifys)
                            {
                                Inventec.Common.Logging.LogSystem.Error("________________" + item.ID);
                                itemNotify = item;
                                AlertInfo info = new DevExpress.XtraBars.Alerter.AlertInfo(null, null);
                                info.Tag = item.ID;
                                alert = new AlertCustom(item, sdaConsumer, this.param, branchName, loginname,(result) =>
                                {
                                    if (result && alertControl.AlertFormList != null && alertControl.AlertFormList.Count > 0)
                                    {
                                        foreach (AlertForm form in alertControl.AlertFormList)
                                        {
                                            if (Int32.Parse(form.Tag.ToString()) == item.ID)
                                            {
                                                form.Close();
                                                break;
                                            }
                                        }
                                    }
                                });
                                alert.createAlert((wh) =>
                                {
                                    w = Int32.Parse(wh.Split('_')[0]);
                                    h = Int32.Parse(wh.Split('_')[1]);
                                });
                                alertControl.Show(Form.ActiveForm, info);
                            }
                        }
                    }
                    else
                    {
                        if (notifyAlert == null && totalCount > 0)
                        {
                            notifyAlert = new frmNotify(listNotifyADO, sdaConsumer, this.param, this.loginname,this.departmentCode);
                            notifyAlert.ShowDialog();
                            notifyAlert = null;
                            totalCount--;
                        }
                        if (newNotifys != null && newNotifys.Count >= dem && totalCount > 0)
                        {
                            notify.Icon = Properties.Resources.haveNotify;
                            notify.Text = "Có " + totalCount + " thông báo chưa đọc.";
                            notify.ShowBalloonTip(10000, "Thông báo", "Có " + totalCount + " thông báo chưa đọc.\n\n " + newNotifys[dem].TITLE, ToolTipIcon.Info);
                        }
                        else
                        {
                            notify.Icon = Properties.Resources.notNotify;
                            notify.Text = "Không có thông báo nào";
                        }
                        dem++;
                    }


                }
                else if (totalCount <= 0)
                {
                    notify.Icon = Properties.Resources.notNotify;
                    notify.Text = "Không có thông báo nào";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void alertControl_FormLoad(object sender, AlertFormLoadEventArgs e)
        {
            try
            {
                e.Buttons.PinButton.SetDown(true);
                alertControl.AlertFormList[alertControl.AlertFormList.Count - 1].Tag = itemNotify.ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private static void alertControl_FormClosing(object sender, AlertFormClosingEventArgs e)
        {
            try
            {
                if (e.CloseReason == AlertFormCloseReason.TimeUp)
                    e.Cancel = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void AlertControl_BeforeFormShow(object sender, AlertFormEventArgs e)
        {
            try
            {
                e.AlertForm.Size = new Size(w + 25, h + 25);
                alert.Bounds = new Rectangle(10, 22, w, h);
                e.AlertForm.Controls.Add(alert);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Notify_Click(object sender, EventArgs e)
        {
            try
            {
                if (notifyAlert == null)
                {
                    Inventec.Common.WebApiClient.ApiConsumer sdaConsumer = new Inventec.Common.WebApiClient.ApiConsumer(sdaBaseUri, tokenCode, applicationCode);
                    var today = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                    notify.Icon = Properties.Resources.notNotify;
                    notify.Text = "Không có thông báo nào";
                    notifyAlert = new frmNotify(listNotifyADO, sdaConsumer, this.param, this.loginname, this.departmentCode);
                    notifyAlert.ShowDialog();
                    notifyAlert = null;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BalloonTip_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (notifyAlert == null)
                {
                    Inventec.Common.WebApiClient.ApiConsumer sdaConsumer = new Inventec.Common.WebApiClient.ApiConsumer(sdaBaseUri, tokenCode, applicationCode);
                    var today = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                    notify.Icon = Properties.Resources.notNotify;
                    notify.Text = "Không có thông báo nào";
                    notifyAlert = new frmNotify(listNotifyADO, sdaConsumer, this.param, loginname, this.departmentCode);
                    notifyAlert.ShowDialog();
                    notifyAlert = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessCmd(string cmd, ref string uri, ref string token, ref string application, ref int _time, ref int _count, ref string loginName, ref int optionFormNotify, ref string departmentCode, ref string BranchName)
        {
            try
            {
                string[] tmpCmd = cmd.Split('|');
                for (int i = 0; i < tmpCmd.Count(); i++)
                {
                    if (tmpCmd[i] == "SdaBaseUri") uri = tmpCmd[i + 1];
                    if (tmpCmd[i] == "TokenCode") token = tmpCmd[i + 1];
                    if (tmpCmd[i] == "ApplicationCode") application = tmpCmd[i + 1];
                    if (tmpCmd[i] == "Time") _time = Convert.ToInt32(!string.IsNullOrEmpty(tmpCmd[i + 1]) ? tmpCmd[i + 1] : "0");
                    if (tmpCmd[i] == "Count") _count = Convert.ToInt32(!string.IsNullOrEmpty(tmpCmd[i + 1]) ? tmpCmd[i + 1] : "1");
                    if (tmpCmd[i] == "Loginname") loginName = tmpCmd[i + 1];
                    if (tmpCmd[i] == "OptionFormNotify") optionFormNotify = Convert.ToInt32(!string.IsNullOrEmpty(tmpCmd[i + 1]) ? tmpCmd[i + 1] : "0");
                    if (tmpCmd[i] == "ReceiverDepartment") departmentCode = tmpCmd[i + 1];
                    if (tmpCmd[i] == "BranchName") BranchName = tmpCmd[i + 1];
                    i++;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
