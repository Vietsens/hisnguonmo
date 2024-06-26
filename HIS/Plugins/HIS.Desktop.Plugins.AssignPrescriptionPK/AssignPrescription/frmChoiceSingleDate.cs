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
    public partial class frmChoiceSingleDate : Form
    {
        DateTime? oldData;
        Action<DateTime> actDateSelected;
        public frmChoiceSingleDate(DateTime? data, Action<DateTime> actDateSelected)
        {
            InitializeComponent();
            this.oldData = data;
            this.actDateSelected = actDateSelected;
            this.SetCaptionByLanguageKeyNew();
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmChoiceSingleDate
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguagefrmChoiceSingleDate = new ResourceManager("HIS.Desktop.Plugins.AssignPrescriptionPK.Resources.Lang", typeof(frmChoiceSingleDate).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmChoiceSingleDate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguagefrmChoiceSingleDate, LanguageManager.GetCulture());
                this.btnChoose.Text = Inventec.Common.Resource.Get.Value("frmChoiceSingleDate.btnChoose.Text", Resources.ResourceLanguageManager.LanguagefrmChoiceSingleDate, LanguageManager.GetCulture());
                this.lciFordtIntructionTime.Text = Inventec.Common.Resource.Get.Value("frmChoiceSingleDate.lciFordtIntructionTime.Text", Resources.ResourceLanguageManager.LanguagefrmChoiceSingleDate, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmChoiceSingleDate.Text", Resources.ResourceLanguageManager.LanguagefrmChoiceSingleDate, LanguageManager.GetCulture());
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
                this.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciTimeAssign.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                lciFordtIntructionTime.Text = Inventec.Common.Resource.Get.Value("FormMultiChooseDate__CaptionCalendaInput", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                btnChoose.Text = Inventec.Common.Resource.Get.Value("FormMultiChooseDate__CaptionBtnChoose", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmChoiceSingleDate_Load(object sender, EventArgs e)
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                if (oldData != null && oldData != DateTime.MinValue)
                {
                    dtIntructionTime.EditValue = oldData;
                    this.timeIntruction.EditValue = oldData.Value.ToString("HH:mm");
                }
                else
                {
                    dtIntructionTime.EditValue = DateTime.Now;
                    this.timeIntruction.EditValue = DateTime.Now.ToString("HH:mm");
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
                if (this.actDateSelected != null && dtIntructionTime != null && dtIntructionTime.DateTime != DateTime.MinValue)
                {
                    System.DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 1);
                    var timeSelested = today.Add(this.timeIntruction.TimeSpan);
                    this.actDateSelected(new DateTime(dtIntructionTime.DateTime.Year, dtIntructionTime.DateTime.Month, dtIntructionTime.DateTime.Day, timeSelested.Hour, timeSelested.Minute, 0));
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
    }
}
