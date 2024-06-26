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
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.ExportXmlQD130.ADO;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExportXmlQD130
{
    public partial class frmSettingSavePath : HIS.Desktop.Utility.FormBase
    {
        public SavePathADO savePathADO;
        Action<SavePathADO> actAfterSave;
        public frmSettingSavePath()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public frmSettingSavePath(SavePathADO savePathADO, Action<SavePathADO> actAfterSave)
        {
            try
            {
                InitializeComponent();
                this.savePathADO = savePathADO;
                this.actAfterSave = actAfterSave;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void frmSettingSavePath_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                SetCaptionByLanguageKey();
                if (savePathADO == null)
                    savePathADO = new SavePathADO();
                txtPathXml.Text = this.savePathADO.pathXml;
                txtPathXmlGDYK.Text = this.savePathADO.pathXmlGDYK;
                txtPathCollinearXml.Text = this.savePathADO.pathCollinearXml;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPathXml_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    this.savePathADO.pathXml = fbd.SelectedPath;
                    txtPathXml.Text = this.savePathADO.pathXml;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPathXmlGDYK_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    this.savePathADO.pathXmlGDYK = fbd.SelectedPath;
                    txtPathXmlGDYK.Text = this.savePathADO.pathXmlGDYK;
                }
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ExportXmlQD130.Resources.Lang", typeof(frmSettingSavePath).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.bbtnSave.Caption = Inventec.Common.Resource.Get.Value("frmSettingSavePath.bbtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmSettingSavePath.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPathXmlGDYK.Text = Inventec.Common.Resource.Get.Value("frmSettingSavePath.btnPathXmlGDYK.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPathXml.Text = Inventec.Common.Resource.Get.Value("frmSettingSavePath.btnPathXml.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPathSaveXml.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmSettingSavePath.lciPathSaveXml.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPathSaveXml.Text = Inventec.Common.Resource.Get.Value("frmSettingSavePath.lciPathSaveXml.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPathSaveXml12.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmSettingSavePath.lciPathSaveXml12.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPathSaveXml12.Text = Inventec.Common.Resource.Get.Value("frmSettingSavePath.lciPathSaveXml12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmSettingSavePath.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void frmSettingSavePath_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                this.actAfterSave(this.savePathADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPathCollinearXml_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    this.savePathADO.pathCollinearXml = fbd.SelectedPath;
                    txtPathCollinearXml.Text = this.savePathADO.pathCollinearXml;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
