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

namespace HIS.Desktop.Plugins.ExpMestOtherExport
{
    public partial class FromExpMestOtherExport : HIS.Desktop.Utility.FormBase
    {
        UCExpMestOtherExport ucExpMestOtherExport;
        long roomTypeId;
        long roomId;
        long expMestId;
        Inventec.Desktop.Common.Modules.Module moduleData;

        public FromExpMestOtherExport()
        {
            InitializeComponent();
        }

        public FromExpMestOtherExport(Inventec.Desktop.Common.Modules.Module _moduleData, long expMestId)
        {
            InitializeComponent();
            try
            {
                moduleData = _moduleData;
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                this.roomTypeId = _moduleData.RoomTypeId;
                this.roomId = _moduleData.RoomId;
                this.expMestId = expMestId;
                //InitMedicineTree();
                //InitMaterialTree();
                //InitExpMestMateGrid();
                //InitExpMestMediGrid();
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
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }

                //Chạy tool sinh resource key language sinh tự động đa ngôn ngữ -> copy vào đây
                //TODO
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ExpMestOtherExport.Resources.Lang", typeof(HIS.Desktop.Plugins.ExpMestOtherExport.FromExpMestOtherExport).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FromExpMestOtherExport.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FromExpMestOtherExport.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemAdd.Caption = Inventec.Common.Resource.Get.Value("FromExpMestOtherExport.barButtonItemAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemSave.Caption = Inventec.Common.Resource.Get.Value("FromExpMestOtherExport.barButtonItemSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemNew.Caption = Inventec.Common.Resource.Get.Value("FromExpMestOtherExport.barButtonItemNew.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FromExpMestOtherExport.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception expMestId)
            {
                Inventec.Common.Logging.LogSystem.Error(expMestId);
            }
        }

        private void FromExpMestOtherExport_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                if (expMestId != null && expMestId > 0)
                {
                    this.ucExpMestOtherExport = new UCExpMestOtherExport(this.moduleData, expMestId);
                    if (this.ucExpMestOtherExport != null)
                    {
                        this.panelControl.Controls.Add(this.ucExpMestOtherExport);
                        this.ucExpMestOtherExport.Dock = DockStyle.Fill;
                    }
                }
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
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (ucExpMestOtherExport != null)
                {
                    ucExpMestOtherExport.BtnAdd();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (ucExpMestOtherExport != null)
                {
                    ucExpMestOtherExport.BtnSave();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (ucExpMestOtherExport != null)
                {
                    ucExpMestOtherExport.BtnNew();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
