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
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.SurgServiceReqExecute.Config;
using HIS.Desktop.Plugins.SurgServiceReqExecute.FormSurgAssignAndCopy;
using HIS.Desktop.Plugins.SurgServiceReqExecute.Resources;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.LibraryMessage;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute.FormSurgAssignAndCopy
{
    public partial class frmMultiIntructonTime : Form
    {
        //qtcode
        internal static bool IsCheckDepartmentInTimeWhenPresOrAssign;
        private List<HIS_DEPARTMENT_TRAN> ListDepartmentTranCheckTime = null;
        private List<HIS_CO_TREATMENT> ListCoTreatmentCheckTime = null;
        V_HIS_TREATMENT treatment;
        DelegateSelectMultiDate delegateSelectData;
        List<DateTime?> oldDatas;
        Inventec.Desktop.Common.Modules.Module moduleData;
        DateTime timeSelested;
        public string CallerButton { get; set; }
        public frmMultiIntructonTime()
        {
            InitializeComponent();
            this.SetCaptionByLanguageKey();
        }
        public frmMultiIntructonTime(List<DateTime?> datas, DateTime time, DelegateSelectMultiDate selectData, string caller, Inventec.Desktop.Common.Modules.Module moduleData, V_HIS_TREATMENT treatment)
        {
            try
            {
                InitializeComponent();
                HisConfigCFG.LoadConfig();
                this.delegateSelectData = selectData;
                this.oldDatas = datas;
                this.timeSelested = time;
                this.CallerButton = caller;
                this.moduleData = moduleData;
                this.treatment = treatment;
                //this.SetCaptionByLanguageKey();
                this.SetCaptionByLanguageKeyNew();
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmMultiIntructonTime
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.SurgServiceReqExecute.Resources.Lang", typeof(frmMultiIntructonTime).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                if (CallerButton == "Ngay y lệnh")
                {
                    this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmMultiIntructonTime.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    this.btnChoose.Text = Inventec.Common.Resource.Get.Value("frmMultiIntructonTime.btnChoose.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    this.lblCalendaInput.Text = Inventec.Common.Resource.Get.Value("frmMultiIntructonTime.lblCalendaInput.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    this.lblTimeInput.Text = Inventec.Common.Resource.Get.Value("frmMultiIntructonTime.lblTimeInput.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    this.Text = Inventec.Common.Resource.Get.Value("frmMultiIntructonTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                else if (CallerButton == "Ngay dự trù")
                {
                    this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmMultiIntructonTime.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    this.btnChoose.Text = Inventec.Common.Resource.Get.Value("frmMultiIntructonTime.btnChoose.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    this.lblCalendaInput.Text = Inventec.Common.Resource.Get.Value("frmMultiIntructonTime.lblCalendaInputDT.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    this.lblTimeInput.Text = Inventec.Common.Resource.Get.Value("frmMultiIntructonTime.lblTimeInputDT.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    this.Text = Inventec.Common.Resource.Get.Value("frmMultiIntructonTimeDT.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        private void SetCaptionByLanguageKey()
        {
            try
            {
                this.Text = Inventec.Common.Resource.Get.Value("AssignService.lciTimeAssign.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                lblTimeInput.Text = Inventec.Common.Resource.Get.Value("FormMultiChooseDate__CaptionTimeInput", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                lblCalendaInput.Text = Inventec.Common.Resource.Get.Value("FormMultiChooseDate__CaptionCalendaInput", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                btnChoose.Text = Inventec.Common.Resource.Get.Value("FormMultiChooseDate__CaptionBtnChoose", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmMultiIntructonTime1_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.oldDatas != null && this.oldDatas.Count > 0)
                {
                    //Add datepcker with data
                    foreach (var item in this.oldDatas)
                    {
                        if (item != null && item.Value != DateTime.MinValue)
                        {
                            calendarIntructionTime.AddSelection(item.Value);
                        }
                    }
                }
                if (this.timeSelested != DateTime.MinValue)
                {
                    timeIntruction.EditValue = this.timeSelested.ToString("HH:mm");
                }
                else
                {
                    timeIntruction.EditValue = DateTime.Now.ToString("HH:mm");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnChooose_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool isSelected = false;
                List<DateTime?> listSelected = new List<DateTime?>();
                foreach (DateRange item in calendarIntructionTime.SelectedRanges)
                {
                    //if(item)
                    if (item != null)
                    {
                        var dt = item.StartDate;
                        while (dt.Date < item.EndDate.Date)
                        {
                            isSelected = true;
                            listSelected.Add(dt);
                            dt = dt.AddDays(1);
                        }
                    }
                }
                WaitingManager.Hide();
                if (isSelected)
                {
                    System.DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 2);
                    System.DateTime answer = today.Add(timeIntruction.TimeSpan);
                    this.timeSelested = answer;
                    if (delegateSelectData != null)
                        delegateSelectData(listSelected, this.timeSelested);
                    this.Close();
                }
                else
                {
                    MessageManager.Show(ResourceMessage.ChuaChonNgayChiDinh);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void btnChoose_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool isSelected = false;
                List<long> listTime = new List<long>();
                List<DateTime?> listSelected = new List<DateTime?>();

                foreach (DateRange item in calendarIntructionTime.SelectedRanges)
                {
                    if (item != null)
                    {
                        var dt = item.StartDate;
                        //bool first = true;
                        while (dt.Date < item.EndDate.Date)
                        {
                            isSelected = true;
                            listSelected.Add(dt); 
                            //long timeNumber = long.Parse(dt.ToString("yyyyMMddHHmmss"));
                            DateTime dtWithTime = dt.Date.Add(timeIntruction.TimeSpan);
                            long timeNumber = long.Parse(dtWithTime.ToString("yyyyMMddHHmmss"));
                            listTime.Add(timeNumber);
                            dt = dt.AddDays(1);
                        }
                    }
                }

                if (isSelected)
                {
                    if (HisConfigCFG.IsCheckDepartmentInTimeWhenPresOrAssign)
                    {
                        WaitingManager.Hide();
                        bool isValidTime = CheckTimeInDepartment(listTime);
                        //bool isValidTime = true; 
                        if (!isValidTime)
                        {
                            return;
                        }
                    }
                    else
                    {
                        WaitingManager.Hide();
                        bool isValidTimeInTime = CheckTimeInTime(listTime);
                        if (!isValidTimeInTime)
                        {
                            return; 
                        }
                    }


                    System.DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 2);
                    System.DateTime answer = today.Add(timeIntruction.TimeSpan);
                    this.timeSelested = answer;

                    if (delegateSelectData != null)
                        delegateSelectData(listSelected, this.timeSelested);

                    WaitingManager.Hide();
                    
                    this.Close();
                }
                else
                {
                    WaitingManager.Hide();
                    MessageManager.Show(ResourceMessage.ChuaChonNgayChiDinh);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private bool CheckTimeInTime(List<long> listTime)
        {
            bool result = true; 
            foreach(var intructionTime in listTime)
            {
                if (intructionTime < this.treatment.IN_TIME)
                {
                        XtraMessageBox.Show("Thời gian y lệnh phải lớn hơn thời gian vào viện", "Thông báo");
                        return false;
                }
            }
            return result;
        }

        private bool CheckTimeInDepartment(List<long> listTime)
        {
            bool result = true;
            try
            {
                V_HIS_ROOM currentWorkingRoom = null;
                currentWorkingRoom = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().First(o => o.ID == this.moduleData.RoomId);
                CommonParam paramGet = new CommonParam();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listTime), listTime));
                HisDepartmentTranFilter filter = new HisDepartmentTranFilter();
                filter.TREATMENT_ID = this.treatment.ID;
                this.ListDepartmentTranCheckTime = new BackendAdapter(paramGet).Get<List<HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                //danh sách các lần chuyển khoa
                List<HIS_DEPARTMENT_TRAN> curremtTrans = null;
                if (this.ListDepartmentTranCheckTime != null && this.ListDepartmentTranCheckTime.Count > 0)
                {
                    curremtTrans = this.ListDepartmentTranCheckTime.Where(o => o.DEPARTMENT_ID == currentWorkingRoom.DEPARTMENT_ID && o.DEPARTMENT_IN_TIME.HasValue).ToList();
                }

                List<HIS_CO_TREATMENT> currentCo = null;
                HisCoTreatmentFilter filter1 = new HisCoTreatmentFilter();
                filter1.TDL_TREATMENT_ID = this.treatment.ID;
                this.ListCoTreatmentCheckTime = new BackendAdapter(paramGet).Get<List<HIS_CO_TREATMENT>>("api/HisCoTreatment/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                if (this.ListCoTreatmentCheckTime != null && this.ListCoTreatmentCheckTime.Count > 0)
                {
                    currentCo = this.ListCoTreatmentCheckTime.Where(o => o.DEPARTMENT_ID == currentWorkingRoom.DEPARTMENT_ID && o.START_TIME.HasValue).ToList();
                }

                foreach (var intructionTime in listTime)
                {
                    bool hasTran = false;

                    List<string> times = new List<string>();
                    if (curremtTrans != null && curremtTrans.Count > 0)
                    {
                        curremtTrans = curremtTrans.OrderBy(o => o.DEPARTMENT_IN_TIME ?? 0).ToList();

                        long fromTime = 0;
                        long toTime = 0;

                        foreach (var item in curremtTrans)
                        {
                            fromTime = item.DEPARTMENT_IN_TIME ?? 0;
                            toTime = long.MaxValue;
                            HIS_DEPARTMENT_TRAN nextTran = this.ListDepartmentTranCheckTime.FirstOrDefault(o => o.PREVIOUS_ID == item.ID);
                            if (nextTran != null)
                            {
                                toTime = nextTran.DEPARTMENT_IN_TIME ?? long.MaxValue;
                            }

                            hasTran = hasTran || (fromTime <= intructionTime && intructionTime <= toTime);

                            times.Add(string.Format("từ {0}{1}", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(fromTime),
                            (toTime > 0 && toTime != long.MaxValue) ? " đến " + Inventec.Common.DateTime.Convert.TimeNumberToTimeString(toTime) : ""));
                        }
                    }

                    if (!hasTran && times.Count > 0 && currentCo != null && currentCo.Count > 0)
                    {
                        times.Clear();
                    }

                    if (!hasTran && currentCo != null && currentCo.Count > 0)
                    {
                        currentCo = currentCo.OrderBy(o => o.START_TIME ?? 0).ToList();
                        long fromTime = 0;
                        long toTime = 0;

                        foreach (var item in currentCo)
                        {
                            fromTime = item.START_TIME ?? 0;
                            toTime = item.FINISH_TIME ?? long.MaxValue;

                            hasTran = hasTran || (fromTime <= intructionTime && intructionTime <= toTime);

                            times.Add(string.Format("từ {0}{1}", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(fromTime),
                            (toTime > 0 && toTime != long.MaxValue) ? " đến " + Inventec.Common.DateTime.Convert.TimeNumberToTimeString(toTime) : ""));
                        }
                    }
                    if (!hasTran)
                    {
                        //XtraMessageBox.Show(string.Format(ResourceMessage.ThoiGianYLenhKhongThuocKhoangThoiGianTrongKhoa,
                        //   string.Join(",", times)), "Thông báo");
                        XtraMessageBox.Show("Thời gian y lệnh phải nằm trong thời gian bệnh nhân hiện diện tại khoa", "Thông báo");
                        //this.isNotLoadWhileChangeInstructionTimeInFirst = true;
                        //this.dtInstructionTime.Focus();
                        //this.isNotLoadWhileChangeInstructionTimeInFirst = false;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
