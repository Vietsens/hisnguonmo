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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.MessageBoxForm
{
    public partial class frmBlockNoAdd : DevExpress.XtraEditors.XtraForm
    {
        List<IcdServiceADO> lstIcdServiceADOs = new List<IcdServiceADO>();
        Action<bool> ActionContinue;
        bool? IsClickYN = null;

        public frmBlockNoAdd(List<IcdServiceADO> _lstIcdServiceADO, Action<bool> ActionContinue = null)
        {
            InitializeComponent();

            try
            {
                this.lstIcdServiceADOs = _lstIcdServiceADO;
                this.ActionContinue = ActionContinue;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResourcefrmBlockNoAdd = new ResourceManager("HIS.Desktop.Plugins.AssignPrescriptionPK.Resources.Lang", typeof(frmBlockNoAdd).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmBlockNoAdd.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResourcefrmBlockNoAdd, LanguageManager.GetCulture());
                this.lblText.Text = Inventec.Common.Resource.Get.Value("frmBlockNoAdd.lblText.Text", Resources.ResourceLanguageManager.LanguageResourcefrmBlockNoAdd, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmBlockNoAdd.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmBlockNoAdd, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmBlockNoAdd.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmBlockNoAdd, LanguageManager.GetCulture());
                this.btnN.Text = Inventec.Common.Resource.Get.Value("frmBlockNoAdd.btnClose.Text", Resources.ResourceLanguageManager.LanguageResourcefrmBlockNoAdd, LanguageManager.GetCulture());

                this.Text = Inventec.Common.Resource.Get.Value("frmBlockNoAdd.Text", Resources.ResourceLanguageManager.LanguageResourcefrmBlockNoAdd, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmBlockNoAdd_Load(object sender, EventArgs e)
        {
            FillDataToControl();
        }

        private void FillDataToControl()
        {
            try
            {
                if(ActionContinue != null)
                {
                    layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    btnN.Text = "Không";
                }
                else
                {
                    emptySpaceItem1.Size = new Size(480,26);
                }    
                gridViewIcdService.BeginUpdate();
                gridViewIcdService.GridControl.DataSource = this.lstIcdServiceADOs;
                gridViewIcdService.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.ActionContinue != null)
                    ActionContinue(false);
                IsClickYN = false;
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

		private void frmBlockNoAdd_FormClosing(object sender, FormClosingEventArgs e)
		{
            try
            {
                if (this.ActionContinue != null && IsClickYN == null)
                    ActionContinue(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

		private void gridViewIcdService_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
		{
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    IcdServiceADO data_ManuMedicineADO = (IcdServiceADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data_ManuMedicineADO != null)
                    {

                        if (e.Column.FieldName == "ICD_CODE_NAME")
                        {
                            e.Value = data_ManuMedicineADO.ICD_CODE + " - " + data_ManuMedicineADO.ICD_NAME;
                        }
                    }
                    else
                    {
                        e.Value = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnY_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.ActionContinue != null)
                    ActionContinue(true);
                IsClickYN = true;
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
