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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ImportBlood
{
    public partial class FrmImportBlood : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module moduleData;
        long impMestId;
        DelegateSelectData delegateSelectData;
        UCImportBloodPlus uCImportBloodPlus;
        public FrmImportBlood()
        {
            InitializeComponent();
        }
        public FrmImportBlood(Inventec.Desktop.Common.Modules.Module moduleData, long impMestId, DelegateSelectData delegateSelectData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                this.moduleData = moduleData;
                this.impMestId = impMestId;
                this.delegateSelectData = delegateSelectData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FrmImportBlood_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();

                if (this.impMestId > 0 && this.moduleData != null)
                {
                    this.uCImportBloodPlus = new UCImportBloodPlus(this.moduleData, this.impMestId, this.delegateSelectData);
                    this.panelControl1.Controls.Add(this.uCImportBloodPlus);
                    this.uCImportBloodPlus.Dock = DockStyle.Fill;
                }

                this.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_IMPORT_BLOOD__TITTLE", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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

        private void bbtnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this.uCImportBloodPlus != null)
            {
                this.uCImportBloodPlus.BtnAdd();
            }
        }

        private void bbtnCapNhat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this.uCImportBloodPlus != null)
            {
                this.uCImportBloodPlus.BtnUpdate();
            }
        }

        private void bbtnHuy_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this.uCImportBloodPlus != null)
            {
                this.uCImportBloodPlus.BtnCancel();
            }
        }

        private void bbtnLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this.uCImportBloodPlus != null)
            {
                this.uCImportBloodPlus.BtnSave();
            }
        }

        private void bbtnLuuNhap_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this.uCImportBloodPlus != null)
            {
                this.uCImportBloodPlus.BtnSaveDraft();
            }
        }

        private void bbtnMoi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this.uCImportBloodPlus != null)
            {
                this.uCImportBloodPlus.BtnNew();
            }
        }

        private void bbtnIn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this.uCImportBloodPlus != null)
            {
                this.uCImportBloodPlus.BtnPrint();
                this.uCImportBloodPlus.CboPrint();
            }
        }
    }
}
