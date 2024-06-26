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
using HIS.Desktop.Plugins.Bordereau.ADO;
using HIS.Desktop.Plugins.Bordereau.Base;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Bordereau.ChooseService
{
    public partial class frmChooseService : FormBase
    {
        List<SereServADO> SereServADOs { get; set; }
        List<SereServADO> SereServADOPackages { get; set; }
        long currentDepartmentId { get; set; }
        DelegateSelectData refeshData {get;set;}
        List<SereServADO> sereServADOSelecteds { get; set; }

        public frmChooseService(List<SereServADO> _sereServADOs,List<SereServADO> _sereServADOSelecteds, long _currentDepartmentId, DelegateSelectData _refeshData)
        {
            InitializeComponent();
            SetCaptionByLanguageKey();
            try
            {
                this.SereServADOs = _sereServADOs;
                this.currentDepartmentId = _currentDepartmentId;
                this.refeshData = _refeshData;
                this.sereServADOSelecteds = _sereServADOSelecteds;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmChooseService_Load(object sender, EventArgs e)
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(Inventec.Desktop.Common.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
                LoadServicePackage();
                LoadCboService();
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
                if (cboService.EditValue != null && this.refeshData!=null)
                {
                    SereServADO sereServADO = this.SereServADOPackages.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboService.EditValue.ToString()));
                    if (sereServADO != null)
                    {
                        this.refeshData(sereServADO);
                        this.Close();
                    }
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
                ////Khoi tao doi tuong resource

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmChooseService.layoutControl1.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.btnChoose.Text = Inventec.Common.Resource.Get.Value("frmChooseService.btnChoose.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.cboService.Properties.NullText = Inventec.Common.Resource.Get.Value("frmChooseService.cboService.Properties.NullText", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmChooseService.layoutControlItem2.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmChooseService.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmChooseService_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                sereServADOSelecteds = null;
                refeshData = null;
                currentDepartmentId = 0;
                SereServADOPackages = null;
                SereServADOs = null;
                this.btnChoose.Click -= new System.EventHandler(this.btnChoose_Click);
                this.Load -= new System.EventHandler(this.frmChooseService_Load);
                gridLookUpEdit1View.GridControl.DataSource = null;
                cboService.Properties.DataSource = null;
                emptySpaceItem1 = null;
                layoutControlItem3 = null;
                btnChoose = null;
                layoutControlItem2 = null;
                gridLookUpEdit1View = null;
                cboService = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
