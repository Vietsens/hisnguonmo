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
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.CheckCross.ADO;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CheckCross
{
    public partial class frmCheckCross : HIS.Desktop.Utility.FormBase
    {
        internal Inventec.Desktop.Common.Modules.Module currentModule;

        private List<ImportADO> _ImportADOs_1 { get; set; }
        private List<ImportADO> _ImportADOs_2 { get; set; }
        private List<ImportADO> _DataChecks { get; set; }

        public frmCheckCross()
        {
            InitializeComponent();
        }

        public frmCheckCross(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmCheckCross_Load(object sender, EventArgs e)
        {
            try
            {
                SetIconFrm();
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                SetDefaultValue1();
                SetDefaultValue2();
                LoadCboFilter();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValue1()
        {
            try
            {
                this.lblCoTatCa1.Text = 0 + " bệnh nhân";
                this.lblTongChi1.Text = "0";
                this.lblBHTT1.Text = "0";
                this.lblBNCT1.Text = "0";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValue2()
        {
            try
            {
                this.lblCoTatCa2.Text = 0 + " bệnh nhân";
                this.lblTongChi2.Text = "0";
                this.lblBHTT2.Text = "0";
                this.lblBNCT2.Text = "0";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnChooseImport_Click(object sender, EventArgs e)
        {
            try
            {
                this._ImportADOs_1 = new List<ImportADO>();
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    WaitingManager.Show();
                    var import = new Inventec.Common.ExcelImport.Import();
                    if (import.ReadFileExcel(ofd.FileName))
                    {
                        this._ImportADOs_1 = import.GetWithCheck<ImportADO>(0);
                        if (this._ImportADOs_1 != null && this._ImportADOs_1.Count > 0)
                        {
                            this._ImportADOs_1 = this._ImportADOs_1.Where(p => !string.IsNullOrEmpty(p.MA_BN) && !string.IsNullOrEmpty(p.HO_TEN) && !string.IsNullOrEmpty(p.GIOI_TINH) && !string.IsNullOrEmpty(p.NGAY_SINH)).ToList();
                            this.lblCoTatCa1.Text = this._ImportADOs_1.Count + " bệnh nhân";
                            this.lblTongChi1.Text = Inventec.Common.Number.Convert.NumberToString(this._ImportADOs_1.Sum(p => Inventec.Common.TypeConvert.Parse.ToDecimal(p.T_TONGCHI)), ConfigApplications.NumberSeperator);
                            this.lblBHTT1.Text = Inventec.Common.Number.Convert.NumberToString(this._ImportADOs_1.Sum(p => Inventec.Common.TypeConvert.Parse.ToDecimal(p.T_BHTT)), ConfigApplications.NumberSeperator);
                            this.lblBNCT1.Text = Inventec.Common.Number.Convert.NumberToString(this._ImportADOs_1.Sum(p => Inventec.Common.TypeConvert.Parse.ToDecimal(p.T_BNTT)), ConfigApplications.NumberSeperator);
                            btnDeleteList1.Enabled = true;


                            foreach (var item in this._ImportADOs_1)
                            {
                                item.MA_BN = item.MA_BN.Trim();
                                item.HO_TEN = item.HO_TEN.Trim();
                                item.GIOI_TINH = item.GIOI_TINH.Trim();
                                item.NGAY_SINH = item.NGAY_SINH.Trim();
                                item.MA_THE = item.MA_THE.Trim();
                                item.NGAY_VAO = item.NGAY_VAO.Trim();
                                item.NGAY_RA = item.NGAY_RA.Trim();
                                item.MA_BENH = item.MA_BENH.Trim();
                                item.MA_BENHKHAC = item.MA_BENHKHAC.Trim();
                            }
                        }
                        else
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Import thất bại");
                        }
                    }
                    else
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không đọc được file");
                    }
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnChooseFile2_Click(object sender, EventArgs e)
        {
            try
            {
                this._ImportADOs_2 = new List<ImportADO>();
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    WaitingManager.Show();
                    var import = new Inventec.Common.ExcelImport.Import();
                    if (import.ReadFileExcel(ofd.FileName))
                    {
                        this._ImportADOs_2 = import.GetWithCheck<ImportADO>(0);
                        if (this._ImportADOs_2 != null && this._ImportADOs_2.Count > 0)
                        {
                            this._ImportADOs_2 = this._ImportADOs_2.Where(p => !string.IsNullOrEmpty(p.MA_BN) && !string.IsNullOrEmpty(p.HO_TEN) && !string.IsNullOrEmpty(p.GIOI_TINH) && !string.IsNullOrEmpty(p.NGAY_SINH)).ToList();
                            this.lblCoTatCa2.Text = this._ImportADOs_2.Count + " bệnh nhân";
                            this.lblTongChi2.Text = Inventec.Common.Number.Convert.NumberToString(this._ImportADOs_2.Sum(p => Inventec.Common.TypeConvert.Parse.ToDecimal(p.T_TONGCHI)), ConfigApplications.NumberSeperator);
                            this.lblBHTT2.Text = Inventec.Common.Number.Convert.NumberToString(this._ImportADOs_2.Sum(p => Inventec.Common.TypeConvert.Parse.ToDecimal(p.T_BHTT)), ConfigApplications.NumberSeperator);
                            this.lblBNCT2.Text = Inventec.Common.Number.Convert.NumberToString(this._ImportADOs_2.Sum(p => Inventec.Common.TypeConvert.Parse.ToDecimal(p.T_BNTT)), ConfigApplications.NumberSeperator);
                            btnDeleteList2.Enabled = true;
                        }
                        else
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Import thất bại");
                        }
                    }
                    else
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không đọc được file");
                    }
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCboFilter()
        {
            try
            {
                List<FilterADO> _filterAdos = new List<FilterADO>();

                _filterAdos.Add(new FilterADO(1, "01", "Bệnh nhân có trên HIS mà không có trên cổng"));
                _filterAdos.Add(new FilterADO(2, "02", "Bệnh nhân lệch tổng chi"));
                _filterAdos.Add(new FilterADO(3, "03", "Bệnh nhân sai ICD"));
                _filterAdos.Add(new FilterADO(4, "04", "Bệnh nhân có trên cổng mà không có trên HIS"));
                _filterAdos.Add(new FilterADO(5, "05", "Hồ sơ gửi lên cổng bị trùng"));
                _filterAdos.Add(new FilterADO(6, "06", "Lệch BHTT, BNCT"));
                _filterAdos.Add(new FilterADO(7, "07", "Bệnh nhân dưới 4h"));
                // _filterAdos.Add(new FilterADO(8, "08", "Lệch tổng chi"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboFilter, _filterAdos, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Process(long d)
        {
            try
            {
                switch (d)
                {
                    case 1:
                        CoTrenHISMaKhongCoTrenCong();
                        break;
                    case 2:
                        BenhNhanLechTongChi();
                        break;
                    case 3:
                        BenhNhanSaiICD();
                        break;
                    case 4:
                        CoTrenCongMaKhongCoTrenHIS();
                        break;
                    case 5:
                        HoSoGuiLenCongBiTrung();
                        break;
                    case 6:
                        LechBHTTorBNTT();
                        break;
                    case 7:
                        BenhNhanDuoi4Gio();
                        break;
                    case 8:
                        //TODO
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDoiSoat_Click(object sender, EventArgs e)
        {
            try
            {
                this.lblMesss.Text = "";

                if (this.cboFilter.EditValue != null)
                {
                    if (this._ImportADOs_1 == null || this._ImportADOs_1.Count == 0)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu Import 1 rỗng", "Thông báo");
                        return;
                    }
                    if (this._ImportADOs_2 == null || this._ImportADOs_2.Count == 0)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu Import 2 rỗng", "Thông báo");
                        return;
                    }
                    this._DataChecks = new List<ImportADO>();
                    this.gridControlData.DataSource = null;
                    Process((long)this.cboFilter.EditValue);

                    this.lblMesss.Text = "Có " + this._DataChecks.Count + " bệnh nhân";

                    this.gridControlData.DataSource = _DataChecks;
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn điều kiện", "Thông báo");
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CoTrenHISMaKhongCoTrenCong()
        {
            try
            {
                List<string> maBN2s = this._ImportADOs_2.Select(p => p.MA_BN.Trim()).Distinct().ToList();
                this._DataChecks = this._ImportADOs_1.Where(p => CheckCoTrenHISMaKhongCoTrenCong(p)).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckCoTrenHISMaKhongCoTrenCong(ImportADO ado)
        {
            bool result = true;
            try
            {
                var data = this._ImportADOs_2.FirstOrDefault(p => p.MA_BN.Trim() == ado.MA_BN.Trim()
                     && p.HO_TEN.Trim() == ado.HO_TEN.Trim()
                     && p.GIOI_TINH.Trim() == ado.GIOI_TINH.Trim()
                     && p.NGAY_SINH.Trim() == ado.NGAY_SINH.Trim()
                     && p.MA_THE.Trim() == ado.MA_THE.Trim()
                     && p.NGAY_VAO.Trim() == ado.NGAY_VAO.Trim()
                     && p.NGAY_RA.Trim() == ado.NGAY_RA.Trim());
                if (data != null && !string.IsNullOrEmpty(data.MA_BN))
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void BenhNhanLechTongChi()
        {
            try
            {
                List<string> maBN2s = this._ImportADOs_2.Select(p => p.MA_BN.Trim()).Distinct().ToList();
                this._DataChecks = this._ImportADOs_1.Where(p => maBN2s.Contains(p.MA_BN.Trim()) && CheckLechTongChi(p)).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckLechTongChi(ImportADO ado)
        {
            bool result = false;
            try
            {
                var data = this._ImportADOs_2.FirstOrDefault(p => p.MA_BN.Trim() == ado.MA_BN.Trim()
                   && p.HO_TEN.Trim() == ado.HO_TEN.Trim()
                   && p.GIOI_TINH.Trim() == ado.GIOI_TINH.Trim()
                   && p.NGAY_SINH.Trim() == ado.NGAY_SINH.Trim()
                   && p.MA_THE.Trim() == ado.MA_THE.Trim()
                   && p.NGAY_VAO.Trim() == ado.NGAY_VAO.Trim()
                   && p.NGAY_RA.Trim() == ado.NGAY_RA.Trim());
                if (data != null && data.T_TONGCHI.Trim() != ado.T_TONGCHI.Trim())
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void BenhNhanSaiICD()
        {
            try
            {
                List<string> maBN2s = this._ImportADOs_2.Select(p => p.MA_BN.Trim()).Distinct().ToList();
                this._DataChecks = this._ImportADOs_1.Where(p => maBN2s.Contains(p.MA_BN.Trim()) && CheckSaiICD(p)).ToList();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckSaiICD(ImportADO ado)
        {
            bool result = false;
            try
            {
                var data = this._ImportADOs_2.FirstOrDefault(p => p.MA_BN.Trim() == ado.MA_BN.Trim()
                    && p.HO_TEN.Trim() == ado.HO_TEN.Trim()
                    && p.GIOI_TINH.Trim() == ado.GIOI_TINH.Trim()
                    && p.NGAY_SINH.Trim() == ado.NGAY_SINH.Trim()
                    && p.MA_THE.Trim() == ado.MA_THE.Trim()
                    && p.NGAY_VAO.Trim() == ado.NGAY_VAO.Trim()
                    && p.NGAY_RA.Trim() == ado.NGAY_RA.Trim());
                if (data != null && (data.MA_BENH.Trim() != ado.MA_BENH.Trim() || data.MA_BENHKHAC.Trim() != ado.MA_BENHKHAC.Trim()))
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void CoTrenCongMaKhongCoTrenHIS()
        {
            try
            {
                this._DataChecks = this._ImportADOs_2.Where(p => CheckCoTrenCongMaKhongCoTrenHIS(p)).ToList();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckCoTrenCongMaKhongCoTrenHIS(ImportADO ado)
        {
            bool result = true;
            try
            {
                var data = this._ImportADOs_1.FirstOrDefault(p => p.MA_BN.Trim() == ado.MA_BN.Trim()
                     && p.HO_TEN.Trim() == ado.HO_TEN.Trim()
                     && p.GIOI_TINH.Trim() == ado.GIOI_TINH.Trim()
                     && p.NGAY_SINH.Trim() == ado.NGAY_SINH.Trim()
                     && p.MA_THE.Trim() == ado.MA_THE.Trim()
                     && p.NGAY_VAO.Trim() == ado.NGAY_VAO.Trim()
                     && p.NGAY_RA.Trim() == ado.NGAY_RA.Trim());
                if (data != null && !string.IsNullOrEmpty(data.MA_BN.Trim()))
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void HoSoGuiLenCongBiTrung()
        {
            try
            {
                List<ImportADO> ados = new List<ImportADO>();
                var dataGroups = this._ImportADOs_2.GroupBy(p => new { p.MA_BN, p.HO_TEN, p.GIOI_TINH, p.NGAY_SINH, p.NGAY_VAO, p.MA_THE }).Select(p => p.ToList()).ToList();
                foreach (var items in dataGroups)
                {
                    if (items.Count > 1)
                    {
                        ados.AddRange(items);
                        //var items2 = items.OrderBy(p => p.NGAY_VAO).ToList();
                        //for (int i = 0; i < items2.Count; i++)
                        //{
                        //    for (int j = 0; j < items2.Count; j++)
                        //    {
                        //        if (Inventec.Common.TypeConvert.Parse.ToInt64(items2[j].NGAY_VAO)
                        //            >= Inventec.Common.TypeConvert.Parse.ToInt64(items2[i].NGAY_VAO)
                        //            && Inventec.Common.TypeConvert.Parse.ToInt64(items2[j].NGAY_VAO)
                        //            <= Inventec.Common.TypeConvert.Parse.ToInt64(items2[i].NGAY_RA)
                        //            )
                        //        {
                        //            ados.Add(items2[j]);
                        //        }
                        //        else if (Inventec.Common.TypeConvert.Parse.ToInt64(items2[j].NGAY_RA)
                        //            >= Inventec.Common.TypeConvert.Parse.ToInt64(items2[i].NGAY_VAO)
                        //            && Inventec.Common.TypeConvert.Parse.ToInt64(items2[j].NGAY_RA)
                        //            <= Inventec.Common.TypeConvert.Parse.ToInt64(items2[i].NGAY_RA)
                        //            )
                        //        {
                        //            ados.Add(items2[j]);
                        //        }
                        //    }
                        //}
                    }
                }
                this._DataChecks = ados;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LechBHTTorBNTT()
        {
            try
            {
                List<string> maBN2s = this._ImportADOs_2.Select(p => p.MA_BN.Trim()).Distinct().ToList();
                this._DataChecks = this._ImportADOs_1.Where(p => maBN2s.Contains(p.MA_BN.Trim()) && CheckLechBHTTorBNTT(p)).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckLechBHTTorBNTT(ImportADO ado)
        {
            bool result = false;
            try
            {
                var data = this._ImportADOs_2.FirstOrDefault(p => p.MA_BN.Trim() == ado.MA_BN.Trim()
                      && p.HO_TEN.Trim() == ado.HO_TEN.Trim()
                      && p.GIOI_TINH.Trim() == ado.GIOI_TINH.Trim()
                      && p.NGAY_SINH.Trim() == ado.NGAY_SINH.Trim()
                      && p.MA_THE.Trim() == ado.MA_THE.Trim()
                      && p.NGAY_VAO.Trim() == ado.NGAY_VAO.Trim()
                      && p.NGAY_RA.Trim() == ado.NGAY_RA.Trim());
                if (data != null && (data.T_BHTT.Trim() != ado.T_BHTT.Trim() || data.T_BNTT.Trim() != ado.T_BNTT.Trim()))
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void BenhNhanDuoi4Gio()
        {
            try
            {
                //TODO Lay Ca 2 DS
                List<ImportADO> ados = new List<ImportADO>();
                ados.AddRange(this._ImportADOs_2.Where(p => CheckBenhNhanDuoi4Gio(p)).ToList());
                ados.AddRange(this._ImportADOs_1.Where(p => CheckBenhNhanDuoi4Gio(p)).ToList());
                if (ados != null && ados.Count > 0)
                {
                    var datatGroups = ados.GroupBy(p => new { p.MA_BN, p.HO_TEN, p.GIOI_TINH, p.NGAY_SINH, p.NGAY_VAO, p.MA_THE }).Select(p => p.FirstOrDefault()).ToList();
                    this._DataChecks = datatGroups;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckBenhNhanDuoi4Gio(ImportADO ado)
        {
            bool result = false;
            try
            {
                long ngayRa = Inventec.Common.TypeConvert.Parse.ToInt64(ado.NGAY_RA);
                long ngayVao = Inventec.Common.TypeConvert.Parse.ToInt64(ado.NGAY_VAO);

                result = DayOfTreatment(ngayVao, ngayRa);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void btnExcelExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (this._DataChecks != null && this._DataChecks.Count > 0)
                {
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        WaitingManager.Show();
                        bool success = false;
                        CommonParam param = new CommonParam();
                        Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                        Inventec.Common.FlexCellExport.Store store = new Inventec.Common.FlexCellExport.Store();
                        Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();


                        string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Exp\\", "DanhSachDoiSoat.xls");
                        store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                        store.SetCommonFunctions();
                        objectTag.AddObjectData(store, "ExportResult", this._DataChecks);

                        WaitingManager.Hide();
                        success = store.OutFile(saveFileDialog.FileName);
                        MessageManager.Show(this.ParentForm, null, success);
                        if (!success)
                            return;
                        if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn mở file ngay?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(saveFileDialog.FileName);
                        }
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu đối soát rỗng", "Thông báo");
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDeleteList1_Click(object sender, EventArgs e)
        {
            try
            {
                this._ImportADOs_1 = new List<ImportADO>();
                this.SetDefaultValue1();
                btnDeleteList1.Enabled = false;
                gridControlData.DataSource = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDeleteList2_Click(object sender, EventArgs e)
        {
            try
            {
                this._ImportADOs_2 = new List<ImportADO>();
                this.SetDefaultValue2();
                btnDeleteList2.Enabled = false;
                gridControlData.DataSource = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewData_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (ImportADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "NGAY_RA_STR")
                        {
                            e.Value = data.NGAY_RA.Length == 12 ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(data.NGAY_RA + "00")) : data.NGAY_RA;
                        }
                        else if (e.Column.FieldName == "NGAY_VAO_STR")
                        {
                            e.Value = data.NGAY_VAO.Length == 12 ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(data.NGAY_VAO + "00")) : data.NGAY_VAO;
                        }
                        else if (e.Column.FieldName == "NGAY_SINH_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(Inventec.Common.TypeConvert.Parse.ToInt64(data.NGAY_SINH));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DayOfTreatment(long? timeIn, long? timeOut)
        {
            bool result = false;
            try
            {
                if (!timeIn.HasValue || !timeOut.HasValue || timeIn > timeOut)
                    return result;

                DateTime dtIn = TimeNumberToSystemDateTime(timeIn.Value) ?? DateTime.Now;
                DateTime dtOut = TimeNumberToSystemDateTime(timeOut.Value) ?? DateTime.Now;
                TimeSpan ts = new TimeSpan();
                ts = (TimeSpan)(dtOut - dtIn);

                //Cung 1 ngay va nho hon 4h
                if (timeIn.Value.ToString().Substring(0, 8) == timeOut.Value.ToString().Substring(0, 8))
                {
                    if (ts.TotalMinutes < 240)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        private static System.DateTime? TimeNumberToSystemDateTime(long time)
        {
            System.DateTime? result = null;
            try
            {
                if (time > 0)
                {
                    result = System.DateTime.ParseExact(time.ToString(), "yyyyMMddHHmm",
                                       System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }
    }
}
