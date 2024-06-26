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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout.Utils;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
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

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription
{
    public partial class frmMultiIntructonTime : Form
    {
        DelegateSelectMultiDate delegateSelectData;
        List<DateTime?> oldDatas;
        DateTime timeSelested;
        bool notTime = false;
        public frmMultiIntructonTime()
        {
            InitializeComponent();
            this.SetCaptionByLanguageKeyNew();
        }
        public frmMultiIntructonTime(List<DateTime?> datas, DateTime? time, DelegateSelectMultiDate selectData)
        {
            try
            {
                InitializeComponent();
                this.delegateSelectData = selectData;
                this.oldDatas = datas;
                if (time != null)
                {
                    this.timeSelested = time.Value;
                }
                else 
                {
                    this.notTime = true;
                }
                this.SetCaptionByLanguageKey();
                this.DefaultVisibility();

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
                Resources.ResourceLanguageManager.LanguageResourcefrmMultiIntructonTime = new ResourceManager("HIS.Desktop.Plugins.AssignPrescriptionPK.Resources.Lang", typeof(frmMultiIntructonTime).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmMultiIntructonTime.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMultiIntructonTime, LanguageManager.GetCulture());
                this.lblCalendaInputFromTo.Text = Inventec.Common.Resource.Get.Value("frmMultiIntructonTime.lblCalendaInputFromTo.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMultiIntructonTime, LanguageManager.GetCulture());
                this.lblcheckbox.Text = Inventec.Common.Resource.Get.Value("frmMultiIntructonTime.lblcheckbox.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMultiIntructonTime, LanguageManager.GetCulture());
                this.checkConsecutive.Properties.Caption = Inventec.Common.Resource.Get.Value("frmMultiIntructonTime.checkConsecutive.Properties.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmMultiIntructonTime, LanguageManager.GetCulture());
                this.btnChoose.Text = Inventec.Common.Resource.Get.Value("frmMultiIntructonTime.btnChoose.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMultiIntructonTime, LanguageManager.GetCulture());
                this.lblCalendaInput.Text = Inventec.Common.Resource.Get.Value("frmMultiIntructonTime.lblCalendaInput.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMultiIntructonTime, LanguageManager.GetCulture());
                this.lblTimeInput.Text = Inventec.Common.Resource.Get.Value("frmMultiIntructonTime.lblTimeInput.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMultiIntructonTime, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmMultiIntructonTime.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMultiIntructonTime, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmMultiIntructonTime.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMultiIntructonTime, LanguageManager.GetCulture());
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
                if (!this.notTime)
                {
                    this.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciTimeAssign.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    lblTimeInput.Text = Inventec.Common.Resource.Get.Value("FormMultiChooseDate__CaptionTimeInput", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    lblCalendaInput.Text = Inventec.Common.Resource.Get.Value("FormMultiChooseDate__CaptionCalendaInput", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    btnChoose.Text = Inventec.Common.Resource.Get.Value("FormMultiChooseDate__CaptionBtnChoose", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                else 
                {
                    this.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciTimeAssign.UseTime.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    lblCalendaInput.Text = Inventec.Common.Resource.Get.Value("FormMultiChooseDate__CaptionCalendaInput.UseTime", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                    this.layoutControlItem7.Visibility = LayoutVisibility.Never;
                    this.layoutControlItem6.Visibility = LayoutVisibility.Never;
                    this.layoutControlItem1.Visibility = LayoutVisibility.Never;
                    this.layoutControlItem2.Visibility = LayoutVisibility.Never;
                    this.layoutControlItem8.Visibility = LayoutVisibility.Never;
                    this.layoutControlItem9.Visibility = LayoutVisibility.Never;
                    this.layoutControlItem10.Visibility = LayoutVisibility.Never;
                }
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

        private void btnChoose_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool isSelected = false;
                List<DateTime?> listSelected = new List<DateTime?>();
                if (!this.checkConsecutive.Checked)
                {
                    foreach (DateRange item in calendarIntructionTime.SelectedRanges)
                    {
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
                }
                else
                {
                    if (this.dtIntructionTimeFrom != null && this.dtIntructionTimeTo != null)
                    {
                        System.DateTime? dateBefore = this.dtIntructionTimeFrom.DateTime;
                        System.DateTime? dateAfter = this.dtIntructionTimeTo.DateTime;
                        if (dateBefore != null && dateAfter != null)
                        {
                            TimeSpan difference = dateAfter.Value - dateBefore.Value;

                            if (difference.Days < 0)
                            {
                                MessageManager.Show(ResourceMessage.NgayChiDinhTuLonHonNgayChiDinhDen);
                                return;
                            }
                            else
                            {
                                var dt = this.dtIntructionTimeFrom.DateTime;
                                while (dt.Date <= this.dtIntructionTimeTo.DateTime.Date)
                                {
                                    isSelected = true;
                                    listSelected.Add(dt);
                                    dt = dt.AddDays(1);
                                }
                            }
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
                    if (this.notTime)
                    {
                        this.timeSelested = new DateTime();
                        if (delegateSelectData != null)
                            delegateSelectData(listSelected, this.timeSelested);
                        this.Close();
                    }
                    else
                    {
                        MessageManager.Show(ResourceMessage.ChuaChonNgayChiDinh);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void checkConsecutive_CheckedChanged(object sender, EventArgs e)
        {

            try
            {
                DefaultVisibility();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            
        }

        private void DefaultVisibility()
        {
            try
            {
                 if (this.checkConsecutive.Checked)
                {
                    this.dtIntructionTimeFrom.EditValue = DateTime.Now;
                    this.dtIntructionTimeTo.EditValue = DateTime.Now;
                    this.layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.layoutControlItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.layoutControlItem10.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.Size = new Size(364, 161);
                }
                else
                {
                    this.layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.layoutControlItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.layoutControlItem10.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.Size = new Size(364, 388);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

    }
}
