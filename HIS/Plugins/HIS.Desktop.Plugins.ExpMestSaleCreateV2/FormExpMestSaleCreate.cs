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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpMestSaleCreate
{
    public partial class FormExpMestSaleCreate : Form
    {
        UCExpMestSaleCreate ucExpMesstSaleCreate;
        //Inventec.Desktop.Common.Modules.Module currentModule;
        Inventec.Desktop.Common.Modules.Module module;
        long roomId;
        long roomTypeId;
        long expMestId;

        public FormExpMestSaleCreate()
        {
            InitializeComponent();
        }

        public FormExpMestSaleCreate(Inventec.Desktop.Common.Modules.Module module, long expMestId)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.module = module;
                if (this.module != null)
                {
                    this.roomTypeId = module.RoomTypeId;
                    this.roomId = module.RoomId;
                }
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

        private void FormExpMestSaleCreate_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                if (expMestId != null && expMestId > 0)
                {
                    this.ucExpMesstSaleCreate = new UCExpMestSaleCreate(module, expMestId);
                    if (this.ucExpMesstSaleCreate != null)
                    {
                        this.panelControl.Controls.Add(this.ucExpMesstSaleCreate);
                        this.ucExpMesstSaleCreate.Dock = DockStyle.Fill;
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
                if (ucExpMesstSaleCreate != null)
                {
                    ucExpMesstSaleCreate.BtnAdd();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemSavePrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (ucExpMesstSaleCreate != null)
                {
                    ucExpMesstSaleCreate.BtnSavePrint();
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
                if (ucExpMesstSaleCreate != null)
                {
                    ucExpMesstSaleCreate.BtnSave();
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
                if (ucExpMesstSaleCreate != null)
                {
                    ucExpMesstSaleCreate.BtnNew();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
