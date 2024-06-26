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
using HIS.Desktop.Plugins.CompareBhytInfo.ADO;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CompareBhytInfo
{
    public partial class FormCompareBhytInfo : FormBase
    {
        private Inventec.Desktop.Common.Modules.Module ModuleData;
        private List<BhytMedicineADO> ListThuocADO;
        private List<BhytMaterialADO> ListVatTuADO;
        private List<BhytServiceADO> ListDvktADO;
        private List<XmlFileInfoADO> ListXml;
        private int Count = 0;

        public FormCompareBhytInfo()
        {
            InitializeComponent();
        }

        public FormCompareBhytInfo(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            this.ModuleData = moduleData;
            InitRestoreLayoutGridViewFromXml(gridViewDvkt);
            InitRestoreLayoutGridViewFromXml(gridViewThuoc);
            InitRestoreLayoutGridViewFromXml(gridViewVatTu);
            InitRestoreLayoutGridViewFromXml(gridViewXml);
        }

        private void FormCompareBhytInfo_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                LblCount.Text = "0";
                LblTotal.Text = "/0";
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.CompareBhytInfo.Resources.Lang", typeof(HIS.Desktop.Plugins.CompareBhytInfo.FormCompareBhytInfo).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabDanhMuc.Text = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.xtraTabDanhMuc.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabThuoc.Text = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.xtraTabThuoc.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvThuoc_Gc_ActiveIngrBhytCode.Caption = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.GvThuoc_Gc_ActiveIngrBhytCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvThuoc_Gc_HeinServiceTypeName.Caption = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.GvThuoc_Gc_HeinServiceTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvThuoc_Gc_ActiveIngrBhytName.Caption = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.GvThuoc_Gc_ActiveIngrBhytName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvThuoc_Gc_RegisterNumber.Caption = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.GvThuoc_Gc_RegisterNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvThuoc_Gc_ServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.GvThuoc_Gc_ServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvThuoc_Gc_Concentra.Caption = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.GvThuoc_Gc_Concentra.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvThuoc_Gc_BidNumber.Caption = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.GvThuoc_Gc_BidNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvThuoc_Gc_BidGroupCode.Caption = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.GvThuoc_Gc_BidGroupCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvThuoc_Gc_BidPackageCode.Caption = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.GvThuoc_Gc_BidPackageCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvThuoc_Gc_HstBhytCode.Caption = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.GvThuoc_Gc_HstBhytCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvThuoc_Gc_Price.Caption = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.GvThuoc_Gc_Price.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.TxtThuocSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.TxtThuocSearch.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabVatTu.Text = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.xtraTabVatTu.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvVatTu_Gc_HeinServiceTypeCode.Caption = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.GvVatTu_Gc_HeinServiceTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvVatTu_Gc_HeinServiceTypeName.Caption = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.GvVatTu_Gc_HeinServiceTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvVatTu_Gc_ServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.GvVatTu_Gc_ServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvVatTu_Gc_BidNumber.Caption = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.GvVatTu_Gc_BidNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvVatTu_Gc_BidGroupCode.Caption = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.GvVatTu_Gc_BidGroupCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvVatTu_Gc_BidPackageCode.Caption = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.GvVatTu_Gc_BidPackageCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvVatTu_Gc_BidYear.Caption = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.GvVatTu_Gc_BidYear.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvVatTu_Gc_HstBhytCode.Caption = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.GvVatTu_Gc_HstBhytCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvVatTu_Gc_Price.Caption = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.GvVatTu_Gc_Price.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.TxtVatTuSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.TxtVatTuSearch.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabDvkt.Text = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.xtraTabDvkt.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.layoutControl6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvDvkt_Gc_HeinServiceTypeCode.Caption = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.GvDvkt_Gc_HeinServiceTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvDvkt_Gc_HeinServiceTypeName.Caption = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.GvDvkt_Gc_HeinServiceTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvDvkt_Gc_ServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.GvDvkt_Gc_ServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvDvkt_Gc_HstBhytCode.Caption = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.GvDvkt_Gc_HstBhytCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvDvkt_Gc_Price.Caption = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.GvDvkt_Gc_Price.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.TxtDvktSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.TxtDvktSearch.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.BtnImport.Text = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.BtnImport.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.BtnTemplate.Text = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.BtnTemplate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabCheckXml.Text = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.xtraTabCheckXml.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.TxtXmlSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.TxtXmlSearch.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvXml_Gc_MaBn.Caption = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.GvXml_Gc_MaBn.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvXml_Gc_MaLk.Caption = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.GvXml_Gc_MaLk.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvXml_Gc_HoTen.Caption = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.GvXml_Gc_HoTen.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvXml_Gc_Error.Caption = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.GvXml_Gc_Error.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl1.Text = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.labelControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.BtnChooseFolder.Text = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.BtnChooseFolder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.BtnExportError.Text = Inventec.Common.Resource.Get.Value("FormCompareBhytInfo.BtnExportError.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BtnTemplate_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_BHYT_INFO.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.Title = "Save File";
                        saveFileDialog.FileName = "IMPORT_BHYT_INFO";
                        saveFileDialog.DefaultExt = "xlsx";
                        saveFileDialog.Filter = "Excel files (*.xlsx)|All files (*.*)";
                        saveFileDialog.FilterIndex = 2;
                        saveFileDialog.RestoreDirectory = true;

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            File.Copy(fileName, saveFileDialog.FileName);
                            MessageManager.Show(this, param, true);
                            if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn mở file ngay?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                System.Diagnostics.Process.Start(saveFileDialog.FileName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async void BtnImport_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Multiselect = false;
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        WaitingManager.Show();
                        var import = new Inventec.Common.ExcelImport.Import();
                        if (import.ReadFileExcel(openFileDialog.FileName))
                        {
                            await TaskProcessGridThuoc(import);
                            await TaskProcessGridVatTu(import);
                            await TaskProcessGridDichVu(import);
                        }
                        else
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Không đọc được file");
                        }
                    }
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private Task TaskProcessGridThuoc(Inventec.Common.ExcelImport.Import import)
        {
            return Task.Run(() => ProcessGridThuoc(import));
        }

        private void ProcessGridThuoc(Inventec.Common.ExcelImport.Import import)
        {
            try
            {
                ListThuocADO = new List<BhytMedicineADO>();
                if (import != null)
                {
                    ListThuocADO = import.GetWithCheck<BhytMedicineADO>(0);
                    if (ListThuocADO != null && ListThuocADO.Count > 0)
                    {
                        ListThuocADO = ListThuocADO.Where(o => !String.IsNullOrWhiteSpace(o.ACTIVE_INGR_BHYT_CODE)
                            && !String.IsNullOrWhiteSpace(o.ACTIVE_INGR_BHYT_NAME)
                            && !String.IsNullOrWhiteSpace(o.CONCENTRA)
                            && !String.IsNullOrWhiteSpace(o.HEIN_SERVICE_TYPE_NAME)
                            && !String.IsNullOrWhiteSpace(o.HST_BHYT_CODE)
                            && !String.IsNullOrWhiteSpace(o.REGISTER_NUMBER)
                            && !String.IsNullOrWhiteSpace(o.SERVICE_UNIT_NAME)
                            && o.PRICE > 0
                            ).ToList();
                    }
                }

                gridControlThuoc.DataSource = ListThuocADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private Task TaskProcessGridVatTu(Inventec.Common.ExcelImport.Import import)
        {
            return Task.Run(() => ProcessGridVatTu(import));
        }

        private void ProcessGridVatTu(Inventec.Common.ExcelImport.Import import)
        {
            try
            {
                ListVatTuADO = new List<BhytMaterialADO>();
                if (import != null)
                {
                    ListVatTuADO = import.GetWithCheck<BhytMaterialADO>(1);
                    if (ListVatTuADO != null && ListVatTuADO.Count > 0)
                    {
                        ListVatTuADO = ListVatTuADO.Where(o => !String.IsNullOrWhiteSpace(o.HEIN_SERVICE_TYPE_CODE)
                            && !String.IsNullOrWhiteSpace(o.HEIN_SERVICE_TYPE_NAME)
                            && !String.IsNullOrWhiteSpace(o.HST_BHYT_CODE)
                            && !String.IsNullOrWhiteSpace(o.SERVICE_UNIT_NAME)
                            && !String.IsNullOrWhiteSpace(o.BID_NUMBER)
                            && !String.IsNullOrWhiteSpace(o.BID_YEAR)
                            && o.PRICE > 0
                            ).ToList();
                    }
                }

                gridControlVatTu.DataSource = ListVatTuADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private Task TaskProcessGridDichVu(Inventec.Common.ExcelImport.Import import)
        {
            return Task.Run(() => ProcessGridDichVu(import));
        }

        private void ProcessGridDichVu(Inventec.Common.ExcelImport.Import import)
        {
            try
            {
                ListDvktADO = new List<BhytServiceADO>();
                if (import != null)
                {
                    ListDvktADO = import.GetWithCheck<BhytServiceADO>(2);
                    if (ListDvktADO != null && ListDvktADO.Count > 0)
                    {
                        ListDvktADO = ListDvktADO.Where(o => !String.IsNullOrWhiteSpace(o.HEIN_SERVICE_TYPE_CODE)
                            && !String.IsNullOrWhiteSpace(o.HEIN_SERVICE_TYPE_NAME)
                            && !String.IsNullOrWhiteSpace(o.HST_BHYT_CODE)
                            && !String.IsNullOrWhiteSpace(o.SERVICE_UNIT_NAME)
                            && o.PRICE > 0
                            ).ToList();
                    }
                }

                gridControlDvkt.DataSource = ListDvktADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TxtThuocSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Down)
                {
                    gridViewThuoc.Focus();
                    gridViewThuoc.FocusedRowHandle = 0;
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(TxtThuocSearch.Text) && ListThuocADO != null && ListThuocADO.Count > 0)
                    {
                        string KEY_WORD = TxtThuocSearch.Text.ToLower().Trim();
                        var lstThuoc = ListThuocADO.Where(o =>
                            o.ACTIVE_INGR_BHYT_CODE.ToLower().Contains(KEY_WORD) ||
                            o.ACTIVE_INGR_BHYT_NAME.ToLower().Contains(KEY_WORD) ||
                            o.BID_GROUP_CODE.ToLower().Contains(KEY_WORD) ||
                            o.BID_NUMBER.ToLower().Contains(KEY_WORD) ||
                            o.BID_PACKAGE_CODE.ToLower().Contains(KEY_WORD) ||
                            o.CONCENTRA.ToLower().Contains(KEY_WORD) ||
                            o.HEIN_SERVICE_TYPE_NAME.ToLower().Contains(KEY_WORD) ||
                            o.HST_BHYT_CODE.ToLower().Contains(KEY_WORD) ||
                            o.SERVICE_UNIT_NAME.ToLower().Contains(KEY_WORD) ||
                            o.REGISTER_NUMBER.ToLower().Contains(KEY_WORD)).ToList();

                        gridControlThuoc.BeginUpdate();
                        gridControlThuoc.DataSource = lstThuoc;
                        gridControlThuoc.EndUpdate();
                    }
                    else
                    {
                        gridControlThuoc.BeginUpdate();
                        gridControlThuoc.DataSource = ListThuocADO;
                        gridControlThuoc.EndUpdate();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TxtVatTuSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Down)
                {
                    gridViewVatTu.Focus();
                    gridViewVatTu.FocusedRowHandle = 0;
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(TxtVatTuSearch.Text) && ListVatTuADO != null && ListVatTuADO.Count > 0)
                    {
                        string KEY_WORD = TxtVatTuSearch.Text.ToLower().Trim();
                        var lstVatTu = ListVatTuADO.Where(o =>
                            o.BID_GROUP_CODE.ToLower().Contains(KEY_WORD) ||
                            o.BID_NUMBER.ToLower().Contains(KEY_WORD) ||
                            o.BID_PACKAGE_CODE.ToLower().Contains(KEY_WORD) ||
                            o.HEIN_SERVICE_TYPE_CODE.ToLower().Contains(KEY_WORD) ||
                            o.HEIN_SERVICE_TYPE_NAME.ToLower().Contains(KEY_WORD) ||
                            o.HST_BHYT_CODE.ToLower().Contains(KEY_WORD) ||
                            o.SERVICE_UNIT_NAME.ToLower().Contains(KEY_WORD)).ToList();

                        gridControlVatTu.BeginUpdate();
                        gridControlVatTu.DataSource = lstVatTu;
                        gridControlVatTu.EndUpdate();
                    }
                    else
                    {
                        gridControlVatTu.BeginUpdate();
                        gridControlVatTu.DataSource = ListVatTuADO;
                        gridControlVatTu.EndUpdate();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TxtDvktSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Down)
                {
                    gridViewDvkt.Focus();
                    gridViewDvkt.FocusedRowHandle = 0;
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(TxtDvktSearch.Text) && ListDvktADO != null && ListDvktADO.Count > 0)
                    {
                        string KEY_WORD = TxtDvktSearch.Text.ToLower().Trim();
                        var lstDv = ListDvktADO.Where(o =>
                            o.HEIN_SERVICE_TYPE_CODE.ToLower().Contains(KEY_WORD) ||
                            o.HEIN_SERVICE_TYPE_NAME.ToLower().Contains(KEY_WORD) ||
                            o.HST_BHYT_CODE.ToLower().Contains(KEY_WORD) ||
                            o.SERVICE_UNIT_NAME.ToLower().Contains(KEY_WORD)).ToList();

                        gridControlDvkt.BeginUpdate();
                        gridControlDvkt.DataSource = lstDv;
                        gridControlDvkt.EndUpdate();
                    }
                    else
                    {
                        gridControlDvkt.BeginUpdate();
                        gridControlDvkt.DataSource = ListDvktADO;
                        gridControlDvkt.EndUpdate();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async void BtnChooseFolder_Click(object sender, EventArgs e)
        {
            try
            {
                //không có thông tin danh mục bhyt sẽ không check
                if ((ListThuocADO == null || ListThuocADO.Count == 0) && (ListVatTuADO == null || ListVatTuADO.Count == 0) && (ListDvktADO == null || ListDvktADO.Count == 0))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceLanguageManager.KhongCoThongTinDanhMucBhyt);
                    return;
                }

                ListXml = new List<XmlFileInfoADO>();
                Count = 0;

                string folderName = "";
                using (FolderBrowserDialog fd = new FolderBrowserDialog())
                {
                    if (fd.ShowDialog() == DialogResult.OK)
                    {
                        folderName = fd.SelectedPath;
                    }
                }

                if (!String.IsNullOrWhiteSpace(folderName))
                {
                    List<string> fileEntries = new List<string>();
                    var fileEntriesXml = System.IO.Directory.GetFiles(folderName, "*.xml", SearchOption.TopDirectoryOnly);
                    if (fileEntriesXml != null && fileEntriesXml.Count() > 0)
                    {
                        fileEntries.AddRange(fileEntriesXml.ToList());
                    }

                    if (fileEntries == null || fileEntries.Count() == 0)
                    {
                        WaitingManager.Hide();
                        Inventec.Common.Logging.LogSystem.Error("Folder khong co file nao: " + folderName);
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceLanguageManager.KhongTimThayFile);
                        return;
                    }

                    LblTotal.Text = "/" + fileEntries.Count();
                    foreach (var item in fileEntries)
                    {
                        await StartExecuteFile(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task StartExecuteFile(string item)
        {
            try
            {
                if (ListXml == null)
                {
                    ListXml = new List<XmlFileInfoADO>();
                }

                XmlFileInfoADO xml = await TaskCheckFile(item);
                if (xml != null)
                {
                    if (!String.IsNullOrWhiteSpace(xml.ERROR))
                    {
                        BtnExportError.Enabled = true;
                    }

                    lock (ListXml)
                    {
                        Count += 1;
                        LblCount.Text = Count.ToString();
                    }

                    ListXml.Add(xml);
                }

                gridControlXml.BeginUpdate();
                gridControlXml.DataSource = ListXml;
                gridControlXml.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private Task<XmlFileInfoADO> TaskCheckFile(string fileName)
        {
            return Task.Run(() => CheckXmlFile(fileName));
        }

        private XmlFileInfoADO CheckXmlFile(string fileName)
        {
            XmlFileInfoADO result = new XmlFileInfoADO();
            try
            {
                if (!String.IsNullOrWhiteSpace(fileName))
                {
                    var check = new XMLCheck.CheckProcessor(fileName);
                    check.CheckError(ListThuocADO, ListVatTuADO, ListDvktADO, ref result);
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void TxtXmlSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Down)
                {
                    gridViewXml.Focus();
                    gridViewXml.FocusedRowHandle = 0;
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(TxtXmlSearch.Text) && ListXml != null && ListXml.Count > 0)
                    {
                        string KEY_WORD = TxtXmlSearch.Text.ToLower().Trim();
                        var lstXml = ListXml.Where(o =>
                            o.ERROR.ToLower().Contains(KEY_WORD) ||
                            o.HO_TEN.ToLower().Contains(KEY_WORD) ||
                            o.MA_BN.ToLower().Contains(KEY_WORD) ||
                            o.MA_LK.ToLower().Contains(KEY_WORD)).ToList();

                        gridControlXml.BeginUpdate();
                        gridControlXml.DataSource = lstXml;
                        gridControlXml.EndUpdate();
                    }
                    else
                    {
                        gridControlXml.BeginUpdate();
                        gridControlXml.DataSource = ListXml;
                        gridControlXml.EndUpdate();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnExportError_Click(object sender, EventArgs e)
        {
            try
            {
                if (ListXml != null && ListXml.Count > 0)
                {
                    var listError = ListXml.Where(o => !String.IsNullOrWhiteSpace(o.ERROR)).ToList();
                    if (listError != null && listError.Count > 0)
                    {
                        using (SaveFileDialog saveFile = new SaveFileDialog())
                        {
                            saveFile.Filter = "Excel file|*.xlsx|All file|*.*";
                            if (saveFile.ShowDialog() == DialogResult.OK)
                            {
                                gridControlXml.DataSource = ListXml;
                                gridControlXml.ExportToXlsx(saveFile.FileName);
                                if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn mở file ngay?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    System.Diagnostics.Process.Start(saveFile.FileName);
                                }
                            }
                        }
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceLanguageManager.KhongCoDuLieuLoi);
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceLanguageManager.KhongCoDuLieuXml);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonShowXml_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                XmlFileInfoADO row = (XmlFileInfoADO)gridViewXml.GetFocusedRow();
                if (row != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(row.FileName);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.XMLViewer", ModuleData.RoomId, ModuleData.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
