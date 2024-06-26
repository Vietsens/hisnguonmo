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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Utility;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
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

namespace HIS.Desktop.Plugins.TreatmentIcdEdit
{
    public partial class FormSovaovien : FormBase
    {
        long ID;
        object lblInCode;
        HIS.Desktop.Common.DelegateSelectData dataSelect;
        public FormSovaovien(HIS.Desktop.Common.DelegateSelectData dataSelect_, long ID)
        {
            try
            {
                InitializeComponent();
                this.ID = ID;
                this.dataSelect = dataSelect_;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                HIS_TREATMENT histreament = new HIS_TREATMENT();
                histreament.ID = ID;
                histreament.IN_CODE = txtSoVaoVien.Text;
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TREATMENT>(
                         "api/HisTreatment/UpdateIncode",
                        ApiConsumers.MosConsumer,
                        histreament,
                        param);
                if (rs != null)
                {
                    success = true;
                }

                MessageManager.Show(this.ParentForm, param, success);
                if (true)
                {
                    dataSelect(txtSoVaoVien.Text);
                    this.Close();
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bTN_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtnSave_Click(null, null);
        }

        private void FormSovaovien_Load(object sender, EventArgs e)
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
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
                Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit = new ResourceManager("HIS.Desktop.Plugins.TreatmentIcdEdit.Resources.Lang", typeof(FormSovaovien).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.label1.Text = Inventec.Common.Resource.Get.Value("FormSovaovien.label1.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.BtnSave.Text = Inventec.Common.Resource.Get.Value("FormSovaovien.BtnSave.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormSovaovien.bar1.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.bTN.Caption = Inventec.Common.Resource.Get.Value("FormSovaovien.bTN.Caption", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormSovaovien.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                dataSelect = null;
                lblInCode = null;
                ID = 0;
                this.BtnSave.Click -= new System.EventHandler(this.BtnSave_Click);
                this.bTN.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.bTN_ItemClick);
                this.Load -= new System.EventHandler(this.FormSovaovien_Load);
                barDockControlRight = null;
                barDockControlLeft = null;
                barDockControlBottom = null;
                barDockControlTop = null;
                bTN = null;
                bar1 = null;
                barManager1 = null;
                BtnSave = null;
                txtSoVaoVien = null;
                label1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
