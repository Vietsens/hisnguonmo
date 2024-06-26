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

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription
{
    public partial class frmMedicineDebateConfirm : Form
    {
        Action<int> actChooseDebateType;
        string message = "";

        public frmMedicineDebateConfirm(Action<int> _actChooseDebateType, string _message)
        {
            InitializeComponent();
            this.actChooseDebateType = _actChooseDebateType;
            this.message = _message;
            SetCaptionByLanguageKey();
        }

        private void frmMedicineDebateConfirm_Load(object sender, EventArgs e)
        {
            txtMessage.Text = this.message;
            btnCreateDebate.Focus();
        }


        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmMedicineDebateConfirm
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguagefrmMedicineDebateConfirm = new ResourceManager("HIS.Desktop.Plugins.AssignPrescriptionPK.Resources.Lang", typeof(frmMedicineDebateConfirm).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmMedicineDebateConfirm.layoutControl1.Text", Resources.ResourceLanguageManager.LanguagefrmMedicineDebateConfirm, LanguageManager.GetCulture());
                this.btnClose.Text = Inventec.Common.Resource.Get.Value("frmMedicineDebateConfirm.btnClose.Text", Resources.ResourceLanguageManager.LanguagefrmMedicineDebateConfirm, LanguageManager.GetCulture());
                this.btnCreateDebate.Text = Inventec.Common.Resource.Get.Value("frmMedicineDebateConfirm.btnCreateDebate.Text", Resources.ResourceLanguageManager.LanguagefrmMedicineDebateConfirm, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmMedicineDebateConfirm.Text", Resources.ResourceLanguageManager.LanguagefrmMedicineDebateConfirm, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        private void btnCreateDebate_Click(object sender, EventArgs e)
        {
            try
            {
                if (actChooseDebateType != null)
                {
                    actChooseDebateType(ValidAcinInteractiveWorker.actChooseDebateType__Create);                    
                }
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (actChooseDebateType != null)
                {
                    actChooseDebateType(ValidAcinInteractiveWorker.actChooseDebateType__None);
                }
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
