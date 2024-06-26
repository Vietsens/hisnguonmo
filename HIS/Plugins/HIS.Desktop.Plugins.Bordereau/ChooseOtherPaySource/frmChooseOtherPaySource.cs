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
using Inventec.Desktop.Common.LanguageManager;
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

namespace HIS.Desktop.Plugins.Bordereau.ChooseOtherPaySource
{
    public partial class frmChooseOtherPaySource : Form
    {
        List<SereServADO> sereServADOSelecteds { get; set; }
        DelegateSelectData refeshData { get; set; }

        public frmChooseOtherPaySource(List<SereServADO> _sereServADOSelecteds, DelegateSelectData _refeshData)
        {
            InitializeComponent();
            SetCaptionByLanguageKey();
            try
            {
                this.sereServADOSelecteds = _sereServADOSelecteds;
                this.refeshData = _refeshData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSave.Enabled)
                {
                    btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboOtherPaySource.EditValue == null)
                {
                    if (MessageBox.Show("Bạn chưa chọn nguồn khác. Bạn có chắc muốn thực hiện không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        refeshData(null);
                        this.Close();
                    }
                    else
                    {
                        cboOtherPaySource.Focus();
                    }
                }
                else
                {
                    refeshData(Inventec.Common.TypeConvert.Parse.ToInt64(cboOtherPaySource.EditValue.ToString()));
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmChoosePatientType_Load(object sender, EventArgs e)
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(Inventec.Desktop.Common.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                LoadAndFillDataToReposiOtherPaySource();


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboPatientType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {

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
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmChooseOtherPaySource.layoutControl1.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmChooseOtherPaySource.btnSave.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.cboOtherPaySource.Properties.NullText = Inventec.Common.Resource.Get.Value("frmChooseOtherPaySource.cboOtherPaySource.Properties.NullText", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmChooseOtherPaySource.layoutControlItem1.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmChooseOtherPaySource.bar1.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmChooseOtherPaySource.barButtonItem1.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmChooseOtherPaySource.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmChooseOtherPaySource_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                refeshData = null;
                sereServADOSelecteds = null;
                this.btnSave.Click -= new System.EventHandler(this.btnSave_Click);
                this.cboOtherPaySource.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboPatientType_Closed);
                this.barButtonItem1.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
                this.Load -= new System.EventHandler(this.frmChoosePatientType_Load);
                gridLookUpEdit1View.GridControl.DataSource = null;
                cboOtherPaySource.Properties.DataSource = null;
                barDockControlRight = null;
                barDockControlLeft = null;
                barDockControlBottom = null;
                barDockControlTop = null;
                barButtonItem1 = null;
                bar1 = null;
                barManager1 = null;
                emptySpaceItem1 = null;
                layoutControlItem2 = null;
                layoutControlItem1 = null;
                layoutControlGroup1 = null;
                gridLookUpEdit1View = null;
                cboOtherPaySource = null;
                btnSave = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
