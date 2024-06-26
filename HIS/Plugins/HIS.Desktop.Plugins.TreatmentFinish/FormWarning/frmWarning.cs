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
using HIS.Desktop.Plugins.TreatmentFinish.ADO;
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

namespace HIS.Desktop.Plugins.TreatmentFinish.FormWarning
{
    public delegate void DelegateCheckSkip(bool IsSkip);

    public partial class frmWarning : Form
    {
        DelegateCheckSkip _checkSkip;
        List<WarningADO> _listWarningADO;
        bool _isCheckSkip = false;
        #region Construct
        public frmWarning()
        {
            InitializeComponent();
            try
            {
                SetIcon();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmWarning(DelegateCheckSkip CheckSkip, List<WarningADO> ListWarningADO, bool IsCheckSkip)
            :this()
        {
            try
            {
                this._checkSkip = CheckSkip;
                this._listWarningADO = ListWarningADO;
                this._isCheckSkip = IsCheckSkip;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void frmWarning_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                if (this._listWarningADO != null && this._listWarningADO.Count > 0 && this._listWarningADO.All(o => o.IsSkippable))
                {
                    lciSkip.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    chkSkip.Checked = this._isCheckSkip;
                }
                else
                {
                    lciSkip.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    chkSkip.Checked = false;
                }
                LoadDataToGridWarning(this._listWarningADO);
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.TreatmentFinish.Resources.Lang", typeof(frmWarning).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControlRoot.Text = Inventec.Common.Resource.Get.Value("frmWarning.layoutControlRoot.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkSkip.Properties.Caption = Inventec.Common.Resource.Get.Value("frmWarning.chkSkip.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnDescription.Caption = Inventec.Common.Resource.Get.Value("frmWarning.gridColumnDescription.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmWarning.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #region Private method

        private void LoadDataToGridWarning(List<WarningADO> data)
        {
            try
            {
                gridControlWarning.BeginUpdate();
                gridControlWarning.DataSource = null;
                gridControlWarning.DataSource = data;
                gridControlWarning.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        
        #endregion

        private void chkSkip_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (lciSkip.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                {
                    if (this._checkSkip != null)
                    {
                        this._checkSkip(chkSkip.Checked);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
