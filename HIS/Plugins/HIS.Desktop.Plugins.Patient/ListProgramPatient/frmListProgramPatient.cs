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
using Inventec.Desktop.Common.LanguageManager;
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

namespace HIS.Desktop.Plugins.Patient.ListProgramPatient
{
    public partial class frmListProgramPatient : Form
    {
        public frmListProgramPatient()
        {
            InitializeComponent();
            this.SetCaptionByLanguageKey();
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmListProgramPatient
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource__frmListProgramPatient = new ResourceManager("HIS.Desktop.Plugins.Patient.Resources.Lang", typeof(frmListProgramPatient).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmListProgramPatient.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource__frmListProgramPatient, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmListProgramPatient.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource__frmListProgramPatient, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmListProgramPatient.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource__frmListProgramPatient, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmListProgramPatient.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource__frmListProgramPatient, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmListProgramPatient.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource__frmListProgramPatient, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmListProgramPatient.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource__frmListProgramPatient, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmListProgramPatient.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource__frmListProgramPatient, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmListProgramPatient.Text", Resources.ResourceLanguageManager.LanguageResource__frmListProgramPatient, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
