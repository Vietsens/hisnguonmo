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
        private bool CheckTimeInDepartment(List<long> listTime)
        {
            try
            {
                HisTreatmentViewFilter treatmentViewFilter = new HisTreatmentViewFilter();
                treatmentViewFilter.ID = this.treatment.ID;
                CommonParam param = new CommonParam();
                var treatment = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumer.ApiConsumers.MosConsumer, treatmentViewFilter, null).FirstOrDefault();
                foreach (var time in listTime)
                {
                    if(time < treatment.IN_TIME)
                    {
                        XtraMessageBox.Show(string.Format("Thời gian y lệnh phải lớn hơn thời gian vào viện ({0})", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(treatment.IN_TIME)), "Thông báo");
                        return false; 
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return true;
        }
    }
}
