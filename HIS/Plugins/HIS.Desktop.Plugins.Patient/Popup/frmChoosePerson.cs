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
using HID.EFMODEL.DataModels;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
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

namespace HIS.Desktop.Plugins.Patient.Popup
{
    public partial class frmChoosePerson : Form
    {
        HIS.Desktop.Common.DelegateSelectData delegateSelect;
        List<HID_PERSON> currentPerson;
        V_HIS_PATIENT currentPatient;

        public frmChoosePerson(HIS.Desktop.Common.DelegateSelectData delegateSelect, List<HID_PERSON> data, V_HIS_PATIENT patient)
        {
            InitializeComponent();
            try
            {
                this.delegateSelect = delegateSelect;
                this.currentPerson = data;
                this.currentPatient = patient;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Btn_Select_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (HID_PERSON)gridViewPerson.GetFocusedRow();
                if (row != null)
                {
                    if (this.delegateSelect != null)
                    {
                        this.delegateSelect(row);
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void frmChoosePerson_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetCaptionByLanguageKey();
                label1.Text = this.currentPatient.PATIENT_CODE;
                label2.Text = this.currentPatient.VIR_PATIENT_NAME;
                label3.Text = this.currentPatient.GENDER_NAME;
                label4.Text = this.currentPatient.VIR_ADDRESS;
                gridControlPerson.BeginUpdate();
                gridControlPerson.DataSource = this.currentPerson;
                gridControlPerson.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmChoosePerson
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource__frmChoosePerson = new ResourceManager("HIS.Desktop.Plugins.Patient.Resources.Lang", typeof(frmChoosePerson).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmChoosePerson.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource__frmChoosePerson, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmChoosePerson.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource__frmChoosePerson, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmChoosePerson.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource__frmChoosePerson, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmChoosePerson.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource__frmChoosePerson, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmChoosePerson.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource__frmChoosePerson, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmChoosePerson.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource__frmChoosePerson, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmChoosePerson.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource__frmChoosePerson, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmChoosePerson.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource__frmChoosePerson, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmChoosePerson.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource__frmChoosePerson, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmChoosePerson.Text", Resources.ResourceLanguageManager.LanguageResource__frmChoosePerson, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
