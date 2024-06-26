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
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionCLS.AssignPrescription
{
    public partial class frmIsExpendType : Form
    {
        DelegateSelectData ReloadData;
        public frmIsExpendType(DelegateSelectData ReloadData)
        {
            InitializeComponent();
            this.ReloadData = ReloadData;
        }

        private void frmExpendType_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                //InitComboExpendType();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguagefrmAssignPrescription = new ResourceManager("HIS.Desktop.Plugins.AssignPrescriptionCLS.Resources.Lang", typeof(HIS.Desktop.Plugins.AssignPrescriptionCLS.AssignPrescription.frmIsExpendType).Assembly);

                this.Text = Inventec.Common.Resource.Get.Value("frmExpendType.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmExpendType.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmExpendType.btnSave.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        //private void InitComboExpendType()
        //{
        //    try
        //    {
        //        List<ColumnInfo> columnInfos = new List<ColumnInfo>();
        //        columnInfos.Add(new ColumnInfo("EXPEND_TYPE_CODE", "", 100, 1));
        //        columnInfos.Add(new ColumnInfo("EXPEND_TYPE_NAME", "", 250, 2));
        //        ControlEditorADO controlEditorADO = new ControlEditorADO("EXPEND_TYPE_NAME", "ID", columnInfos, false, 350);
        //        ControlEditorLoader.Load(cboExpendType, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>(), controlEditorADO);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.ReloadData != null)
                {
                    this.ReloadData(chkIsExpendType.Checked);
                }
                this.Close();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
    }
}
