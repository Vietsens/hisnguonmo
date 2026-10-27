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
                txtPathXmlTT12.Text = this.savePathADO.pathXmlTT12;
                this.InitPathXml48h();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Viec 61380: dong Thu muc luu XML ho so du 48 gio. Tao luc chay va CHI tao khi vien bat
        //HIS.QD_130_BYT.AUTO_EXPORT_XML_AFTER_48H = 1 - vien khong dung thi cua so giu nguyen nhu cu.
        private DevExpress.XtraEditors.TextEdit txtPathXml48h;
        private DevExpress.XtraEditors.SimpleButton btnPathXml48h;
        private DevExpress.XtraLayout.LayoutControlItem lciPathXml48h;

        private void InitPathXml48h()
        {
            try
            {
                if (HisConfigCFG.QD_130_BYT__AUTO_EXPORT_XML_AFTER_48H != "1")
                    return;

                this.txtPathXml48h = new DevExpress.XtraEditors.TextEdit();
                this.txtPathXml48h.Name = "txtPathXml48h";
                this.txtPathXml48h.MenuManager = this.barManager1;
                this.txtPathXml48h.StyleController = this.layoutControl1;
                this.txtPathXml48h.Text = this.savePathADO.pathXml48h;
                //Cho go tay duong dan thu muc mang dung chung (\\may-chu\thu-muc). Ghi ngay khi doi gia tri vi
                //FormBase huy control truoc su kien FormClosed, luc do khong doc lai o nhap duoc nua.
                this.txtPathXml48h.EditValueChanged += new System.EventHandler(this.txtPathXml48h_EditValueChanged);

                this.btnPathXml48h = new DevExpress.XtraEditors.SimpleButton();
                this.btnPathXml48h.Name = "btnPathXml48h";
                this.btnPathXml48h.Image = global::HIS.Desktop.Plugins.ExportXmlQD130.Properties.Resources.Folder;
                this.btnPathXml48h.StyleController = this.layoutControl1;
                this.btnPathXml48h.Click += new System.EventHandler(this.btnPathXml48h_Click);

                int bottomBefore = GetLayoutBottom();
                this.layoutControl1.BeginUpdate();
                try
                {
                    this.lciPathXml48h = this.layoutControl1.AddItem(string.Empty, this.txtPathXml48h, this.lciPathXml12, DevExpress.XtraLayout.Utils.InsertType.Bottom);
                    this.lciPathXml48h.Name = "lciPathXml48h";
                    this.lciPathXml48h.AppearanceItemCaption.Options.UseTextOptions = true;
                    this.lciPathXml48h.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    this.lciPathXml48h.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
                    this.lciPathXml48h.TextSize = new System.Drawing.Size(370, 20);
                    this.lciPathXml48h.TextToControlDistance = 5;
                    this.lciPathXml48h.Text = Inventec.Common.Resource.Get.Value("frmSettingSavePath.lciPathXml48h.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    this.lciPathXml48h.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmSettingSavePath.lciPathXml48h.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                    //Nut dat duoi nut cua dong TT12 (cot nut ben phai), o nhap dat duoi o TT12 - giu dung bo cuc hai cot nhu bon dong san co
                    DevExpress.XtraLayout.LayoutControlItem lciBtn = this.layoutControl1.AddItem(string.Empty, this.btnPathXml48h, this.layoutControlItem5, DevExpress.XtraLayout.Utils.InsertType.Bottom);
                    lciBtn.Name = "lciBtnPathXml48h";
                    lciBtn.TextVisible = false;
                    lciBtn.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
                    lciBtn.MinSize = new System.Drawing.Size(29, 28);
                    lciBtn.MaxSize = new System.Drawing.Size(29, 28);
                }
                finally
                {
                    this.layoutControl1.EndUpdate();
                }
                //Cua so cao them dung bang phan bo cuc vua dai ra, khong de dong moi bi cat va hien thanh cuon
                int addHeight = GetLayoutBottom() - bottomBefore;
                this.ClientSize = new System.Drawing.Size(this.ClientSize.Width, this.ClientSize.Height + (addHeight > 0 ? addHeight : 28));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private int GetLayoutBottom()
        {
            int bottom = 0;
            foreach (DevExpress.XtraLayout.BaseLayoutItem item in this.layoutControl1.Root.Items)
            {
                bottom = Math.Max(bottom, item.Location.Y + item.Height);
            }
            return bottom;
        }

        private void txtPathXml48h_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                string path48h = this.txtPathXml48h.Text;
                this.savePathADO.pathXml48h = String.IsNullOrWhiteSpace(path48h) ? null : path48h.Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPathXml48h_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    this.savePathADO.pathXml48h = fbd.SelectedPath;
                    txtPathXml48h.Text = this.savePathADO.pathXml48h;
                }
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

        private void btnPathXmlTT12_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    this.savePathADO.pathXmlTT12 = fbd.SelectedPath;
                    txtPathXmlTT12.Text = this.savePathADO.pathXmlTT12;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
