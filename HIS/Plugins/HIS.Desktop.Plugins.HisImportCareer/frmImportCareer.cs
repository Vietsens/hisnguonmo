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
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.HisImportCareer.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisImportCareer
{
    public partial class frmImportCareer : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module _Module { get; set; }
        RefeshReference delegateRefresh;
        List<CareerADO> _CareerAdos;
        List<CareerADO> _CurrentAdos;
        List<HIS_CAREER> _ListCareers { get; set; }

        public frmImportCareer()
        {
            InitializeComponent();
        }

        public frmImportCareer(Inventec.Desktop.Common.Modules.Module _module)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                this._Module = _module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmImportCareer(Inventec.Desktop.Common.Modules.Module _module, RefeshReference _delegateRefresh)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                this._Module = _module;
                this.delegateRefresh = _delegateRefresh;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmImportCareer_Load(object sender, EventArgs e)
        {
            try
            {
                if (this._Module != null)
                {
                    this.Text = this._Module.text;
                }
                SetIcon();
                LoadDataBed();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataBed()
        {
            try
            {
                _ListCareers = new List<HIS_CAREER>();
                MOS.Filter.HisCareerFilter filter = new MOS.Filter.HisCareerFilter();
                _ListCareers = new BackendAdapter(new CommonParam()).Get<List<HIS_CAREER>>("api/HisCareer/Get", ApiConsumers.MosConsumer, filter, null);
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
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDownLoadFile_Click(object sender, EventArgs e)
        {
            try
            {

                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_CAREER.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog.Title = "Save File";
                    saveFileDialog.FileName = "IMPORT_CAREER";
                    saveFileDialog.DefaultExt = "xlsx";
                    saveFileDialog.Filter = "Excel files (*.xlsx)|All files (*.*)";
                    saveFileDialog.FilterIndex = 2;
                    saveFileDialog.RestoreDirectory = true;

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(fileName, saveFileDialog.FileName);
                        MessageManager.Show(this.ParentForm, param, true);
                        if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn mở file ngay?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(saveFileDialog.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnChooseFile_Click(object sender, EventArgs e)
        {
            try
            {
                btnShowLineError.Text = "Dòng lỗi";

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    WaitingManager.Show();

                    var import = new Inventec.Common.ExcelImport.Import();
                    if (import.ReadFileExcel(ofd.FileName))
                    {
                        var hisServiceImport = import.GetWithCheck<CareerADO>(0);
                        if (hisServiceImport != null && hisServiceImport.Count > 0)
                        {
                            // Moi dong co du lieu = 1 nghe nghiep (file mau dang phang 8 cot)
                            List<CareerADO> listAfterRemove = new List<CareerADO>();
                            foreach (var item in hisServiceImport)
                            {
                                bool checkNull = string.IsNullOrEmpty(item.CAREER_CODE)
                                    && string.IsNullOrEmpty(item.CAREER_NAME);

                                if (!checkNull)
                                {
                                    listAfterRemove.Add(item);
                                }
                            }

                            WaitingManager.Hide();

                            this._CurrentAdos = listAfterRemove;

                            if (this._CurrentAdos != null && this._CurrentAdos.Count > 0)
                            {
                                btnShowLineError.Enabled = true;
                                this._CareerAdos = new List<CareerADO>();

                                Inventec.Common.Logging.LogSystem.Debug("+++++++++++++++ lần 1 ++++++++++" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this._CareerAdos), this._CareerAdos));
                                addServiceToProcessList(_CurrentAdos, ref this._CareerAdos);
                                Inventec.Common.Logging.LogSystem.Debug("+++++++++++++++ lần 2 ++++++++++" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this._CareerAdos), this._CareerAdos));
                                SetDataSource(this._CareerAdos);
                            }

                            SetStatisticText();

                            //btnSave.Enabled = true;
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
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Level 2/3/4 (QD 34/2020/QD-TTg)
        /// <summary>Do dai chuan cua ma nghe cap 5 theo QD 34/2020/QD-TTg</summary>
        private const int CAREER_CODE_LENGTH = 5;

        /// <summary>Ten nhom cap 2/3/4 tra tu danh muc da co trong he thong (ma cap -> ten cap)</summary>
        Dictionary<string, string> _dicLevel2NameInDb = new Dictionary<string, string>();
        Dictionary<string, string> _dicLevel3NameInDb = new Dictionary<string, string>();
        Dictionary<string, string> _dicLevel4NameInDb = new Dictionary<string, string>();

        /// <summary>
        /// Gom ten nhom cap 2/3/4 tu danh muc nghe nghiep da co (_ListCareers) de dien
        /// vao cac o ten cap bi de trong trong file import
        /// </summary>
        private void BuildLevelNameDictionariesFromDb()
        {
            try
            {
                _dicLevel2NameInDb = new Dictionary<string, string>();
                _dicLevel3NameInDb = new Dictionary<string, string>();
                _dicLevel4NameInDb = new Dictionary<string, string>();
                if (_ListCareers == null) return;
                foreach (var item in _ListCareers)
                {
                    AddLevelName(_dicLevel2NameInDb, item.LEVEL2_CODE, item.LEVEL2_NAME);
                    AddLevelName(_dicLevel3NameInDb, item.LEVEL3_CODE, item.LEVEL3_NAME);
                    AddLevelName(_dicLevel4NameInDb, item.LEVEL4_CODE, item.LEVEL4_NAME);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private static void AddLevelName(Dictionary<string, string> dic, string levelCode, string levelName)
        {
            if (string.IsNullOrEmpty(levelCode) || string.IsNullOrEmpty(levelName)) return;
            dic[levelCode.Trim()] = levelName;
        }

        /// <summary>
        /// Suy ma cap tu ma nghe cap 5 (level ky tu dau, chi khi ma du 5 ky tu)
        /// </summary>
        private static string GetLevelCode(string careerCode, int level)
        {
            string result = "";
            try
            {
                if (!string.IsNullOrEmpty(careerCode) && careerCode.Trim().Length == CAREER_CODE_LENGTH)
                {
                    result = careerCode.Trim().Substring(0, level);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Lay gia tri tu file; rong thi tra ve gia tri du phong (tu suy ma cap / tra ten nhom)
        /// </summary>
        private static string ValueOrFallback(string valueInFile, string fallback)
        {
            return !string.IsNullOrEmpty(valueInFile) ? valueInFile.Trim() : fallback;
        }

        private static string FindLevelNameInDb(Dictionary<string, string> dic, string levelCode)
        {
            string result = "";
            if (!string.IsNullOrEmpty(levelCode) && dic != null)
            {
                dic.TryGetValue(levelCode, out result);
            }
            return result ?? "";
        }

        /// <summary>
        /// Nhan thong ke: tong so dong doc duoc + so dong loi
        /// </summary>
        private void SetStatisticText()
        {
            try
            {
                int total = (this._CareerAdos != null ? this._CareerAdos.Count : 0);
                int errorCount = (this._CareerAdos != null ? this._CareerAdos.Count(o => !string.IsNullOrEmpty(o.ERROR)) : 0);
                lblStatistic.Text = string.Format("Tổng {0} dòng · {1} dòng lỗi", total, errorCount);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        /// <summary>
        ///
        /// </summary>
        /// <param name="_service"></param>
        private void addServiceToProcessList(List<CareerADO> _service, ref List<CareerADO> _careerRoomRef)
        {
            try
            {
                // Chuan bi ten nhom cap 2/3/4 tu danh muc da co (dung khi file de trong o ten cap)
                BuildLevelNameDictionariesFromDb();

                _careerRoomRef = new List<CareerADO>();
                long i = 0;
                foreach (var item in _service)
                {
                    i++;
                    string error = "";
                    var serAdo = new CareerADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<CareerADO>(serAdo, item);

                    if (!string.IsNullOrEmpty(item.CAREER_CODE))
                    {
                        if (item.CAREER_CODE.Length > CAREER_CODE_LENGTH)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã nghề nghiệp");
                        }
                        serAdo.CAREER_CODE = item.CAREER_CODE.Trim();
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã nghề nghiệp");
                    }

                    // Ma trung voi ma da co: cho phep ghi de (upsert), khong bao loi trung

                    if (!string.IsNullOrEmpty(item.CAREER_NAME))
                    {
                        if (item.CAREER_NAME.Length > 1000)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Tên nghề nghiệp");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Tên nghề nghiệp");
                    }

                    // Ma cap: uu tien gia tri trong file; de trong -> tu tach tu ma nghe (2/3/4 ky tu dau).
                    // Ten cap: uu tien gia tri trong file; de trong -> tra tu danh muc da co trong he thong.
                    // Khong tim thay -> de trong (quan tri nhap tay o man danh muc)
                    serAdo.LEVEL2_CODE = ValueOrFallback(item.LEVEL2_CODE, GetLevelCode(serAdo.CAREER_CODE, 2));
                    serAdo.LEVEL3_CODE = ValueOrFallback(item.LEVEL3_CODE, GetLevelCode(serAdo.CAREER_CODE, 3));
                    serAdo.LEVEL4_CODE = ValueOrFallback(item.LEVEL4_CODE, GetLevelCode(serAdo.CAREER_CODE, 4));
                    serAdo.LEVEL2_NAME = ValueOrFallback(item.LEVEL2_NAME, FindLevelNameInDb(_dicLevel2NameInDb, serAdo.LEVEL2_CODE));
                    serAdo.LEVEL3_NAME = ValueOrFallback(item.LEVEL3_NAME, FindLevelNameInDb(_dicLevel3NameInDb, serAdo.LEVEL3_CODE));
                    serAdo.LEVEL4_NAME = ValueOrFallback(item.LEVEL4_NAME, FindLevelNameInDb(_dicLevel4NameInDb, serAdo.LEVEL4_CODE));

                    foreach (var levelName in new string[] { serAdo.LEVEL2_NAME, serAdo.LEVEL3_NAME, serAdo.LEVEL4_NAME })
                    {
                        if (!string.IsNullOrEmpty(levelName) && levelName.Length > 1000)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Tên cấp");
                            break;
                        }
                    }

                    serAdo.ERROR = error;
                    serAdo.ID = i;
                    _careerRoomRef.Add(serAdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetDataSource(List<CareerADO> dataSource)
        {
            try
            {
                gridControlData.BeginUpdate();
                gridControlData.DataSource = null;
                gridControlData.DataSource = dataSource;
                gridControlData.EndUpdate();
                CheckErrorLine(null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckErrorLine(List<CareerADO> dataSource)
        {
            try
            {
                SetStatisticText();
                var checkError = this._CareerAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
                if (!checkError)
                {
                    btnImport.Enabled = true;
                    btnShowLineError.Enabled = false;
                }
                else
                {
                    btnShowLineError.Enabled = true;
                    btnImport.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnShowLineError_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnShowLineError.Text == "Dòng lỗi")
                {
                    btnShowLineError.Text = "Dòng không lỗi";
                    var errorLine = this._CareerAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = this._CareerAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton_ER_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (CareerADO)gridViewData.GetFocusedRow();
                if (row != null && !string.IsNullOrEmpty(row.ERROR))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(row.ERROR, "Thông báo");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButton_Delete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (CareerADO)gridViewData.GetFocusedRow();
                if (row != null)
                {
                    if (this._CareerAdos != null && this._CareerAdos.Count > 0)
                    {
                        this._CareerAdos.Remove(row);
                        var dataCheck = this._CareerAdos.Where(p => p.CAREER_CODE == row.CAREER_CODE && p.CAREER_NAME == row.CAREER_NAME).ToList();
                        if (dataCheck != null && dataCheck.Count == 1)
                        {
                            if (!string.IsNullOrEmpty(dataCheck[0].ERROR))
                            {
                                string erro = string.Format(Message.MessageImport.FileImportDaTonTai, dataCheck[0].CAREER_CODE);
                                //string[] Codes = dataCheck[0].ERROR.Split('|');
                                dataCheck[0].ERROR = dataCheck[0].ERROR.Replace(erro, "");
                            }

                        }
                        SetDataSource(this._CareerAdos);
                    }
                }
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
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    CareerADO pData = (CareerADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                WaitingManager.Show();
                List<CareerImportDTO> datas = new List<CareerImportDTO>();

                if (this._CareerAdos != null && this._CareerAdos.Count > 0)
                {
                    foreach (var item in this._CareerAdos)
                    {
                        CareerImportDTO ado = new CareerImportDTO();
                        ado.CAREER_CODE = item.CAREER_CODE;
                        ado.CAREER_NAME = item.CAREER_NAME;
                        ado.LEVEL2_CODE = item.LEVEL2_CODE;
                        ado.LEVEL2_NAME = item.LEVEL2_NAME;
                        ado.LEVEL3_CODE = item.LEVEL3_CODE;
                        ado.LEVEL3_NAME = item.LEVEL3_NAME;
                        ado.LEVEL4_CODE = item.LEVEL4_CODE;
                        ado.LEVEL4_NAME = item.LEVEL4_NAME;
                        datas.Add(ado);
                    }
                }
                else
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu rỗng", "Thông báo");
                    return;
                }

                CommonParam param = new CommonParam();
                var dataImports = new BackendAdapter(param).Post<List<HIS_CAREER>>("api/HisCareer/ImportList", ApiConsumers.MosConsumer, datas, param);
                WaitingManager.Hide();
                if (dataImports != null && dataImports.Count > 0)
                {
                    success = true;
                    btnImport.Enabled = false;
                    BackendDataWorker.Reset<HIS_CAREER>();
                    LoadDataBed();
                    if (this.delegateRefresh != null)
                    {
                        this.delegateRefresh();
                    }
                }

                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnImport.Enabled)
                    btnImport_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewData_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    //BedADO data = (BedADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    string error = (gridViewData.GetRowCellValue(e.RowHandle, "ERROR") ?? "").ToString();
                    if (e.Column.FieldName == "ERROR_")
                    {
                        if (!string.IsNullOrEmpty(error))
                        {
                            e.RepositoryItem = repositoryItemButton_ER;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
