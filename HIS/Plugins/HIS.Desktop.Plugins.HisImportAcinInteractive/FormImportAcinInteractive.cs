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
using HIS.Desktop.Plugins.HisImportAcinInteractive.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisImportAcinInteractive
{
    public partial class FormImportAcinInteractive : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module _Module { get; set; }
        RefeshReference delegateRefresh;
        List<ImportADO> _ImportAdos;
        List<ImportADO> _CurrentAdos;
        List<V_HIS_ACIN_INTERACTIVE> ListAcinInteractive = new List<V_HIS_ACIN_INTERACTIVE>();
        List<HIS_ACTIVE_INGREDIENT> ListActiveIngredient = new List<HIS_ACTIVE_INGREDIENT>();
        List<HIS_INTERACTIVE_GRADE> ListInterGrade = new List<HIS_INTERACTIVE_GRADE>();
        
        List<ImportADO> errorLines = new List<ImportADO>();
        List<ImportADO> notErrorLines = new List<ImportADO>();
        string showingLines = "";
        private enum ShowingLines
        {
            Default,
            Error,
            NotError,
        }

        public FormImportAcinInteractive()
            : base()
        {
            InitializeComponent();
        }

        public FormImportAcinInteractive(Inventec.Desktop.Common.Modules.Module _module, RefeshReference _delegateRefresh)
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

        private void FormImportAcinInteractive_Load(object sender, EventArgs e)
        {
            try
            {
                if (this._Module != null)
                {
                    this.Text = this._Module.text;
                }
                SetIcon();
                LoadCurrentData();
                Thread thread = new Thread(() =>
                {
                    ListInterGrade = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_INTERACTIVE_GRADE>().Where(o => o.IS_ACTIVE == 1).ToList();
                });
                thread.Start();
               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCurrentData()
        {
            try
            {
                HisAcinInteractiveViewFilter filter = new HisAcinInteractiveViewFilter();
                ListAcinInteractive = new BackendAdapter(new CommonParam()).Get<List<V_HIS_ACIN_INTERACTIVE>>("api/HisAcinInteractive/GetView", ApiConsumers.MosConsumer, filter, null);

                HisActiveIngredientFilter actFilter = new HisActiveIngredientFilter();
                ListActiveIngredient = new BackendAdapter(new CommonParam()).Get<List<HIS_ACTIVE_INGREDIENT>>("api/HisActiveIngredient/Get", ApiConsumers.MosConsumer, actFilter, null);
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
                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_ACIN_INTERACTIVE.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog.Title = "Save File";
                    saveFileDialog.FileName = "IMPORT_ACIN_INTERACTIVE";
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
                        var hisServiceImport = import.GetWithCheck<ImportADO>(0);
                        if (hisServiceImport != null && hisServiceImport.Count > 0)
                        {
                            //List<ImportADO> listAfterRemove = new List<ImportADO>();
                            //foreach (var item in hisServiceImport)
                            //{
                            //    bool checkNull = string.IsNullOrEmpty(item.LOGINNAME)
                            //        && string.IsNullOrEmpty(item.USERNAME)
                            //        ;

                            //    if (!checkNull)
                            //    {
                            //        listAfterRemove.Add(item);
                            //    }
                            //}
                            WaitingManager.Hide();

                            this._CurrentAdos = hisServiceImport.Where(p => checkNull(p)).ToList();

                            if (this._CurrentAdos != null && this._CurrentAdos.Count > 0)
                            {
                                btnShowLineError.Enabled = true;
                                this._ImportAdos = new List<ImportADO>();
                                addServiceToProcessList(_CurrentAdos, ref this._ImportAdos);
                                SetDataSource(this._ImportAdos);
                            }

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

        bool checkNull(ImportADO data)
        {
            bool result = true;
            try
            {
                if (data != null)
                {
                    if (string.IsNullOrEmpty(data.ACTIVE_INGREDIENT_CODE)
                        && string.IsNullOrEmpty(data.CONFLICT_CODE))
                    {
                        result = false;
                    }
                }
                else
                    result = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void addServiceToProcessList(List<ImportADO> _service, ref List<ImportADO> _importAdoRef)
        {
            try
            {
                _importAdoRef = new List<ImportADO>();
                long i = 0;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListActiveIngredient), ListActiveIngredient));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _service), _service));
                foreach (var item in _service)
                {
                    i++;
                    string error = "";
                    var serAdo = new ImportADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ImportADO>(serAdo, item);
                    Inventec.Common.Logging.LogSystem.Error("STEP 1________________");
                    if (!string.IsNullOrEmpty(item.ACTIVE_INGREDIENT_CODE))
                    {
                        if (ListActiveIngredient == null || ListActiveIngredient.Count == 0)
                        {
                            error += string.Format(MessageImport.KhongXacDinhDuoc, item.ACTIVE_INGREDIENT_CODE);
                        }
                        else
                        {
                            var active = ListActiveIngredient.FirstOrDefault(o => o.ACTIVE_INGREDIENT_CODE.ToLower() == item.ACTIVE_INGREDIENT_CODE.Trim().ToLower());
                            if (active == null)
                            {
                                error += string.Format(MessageImport.KhongXacDinhDuoc, item.ACTIVE_INGREDIENT_CODE);
                            }
                            else
                            {
                                serAdo.ACTIVE_INGREDIENT_CODE = active.ACTIVE_INGREDIENT_CODE;
                                serAdo.ACTIVE_INGREDIENT_ID = active.ID;
                                serAdo.ACTIVE_INGREDIENT_NAME = active.ACTIVE_INGREDIENT_NAME;
                            }
                        }
                    }
                    else
                    {
                        error += string.Format(MessageImport.ThieuTruongDL, "mã hoạt chất");
                    }
                    Inventec.Common.Logging.LogSystem.Error("STEP 2________________");
                    if (!string.IsNullOrEmpty(item.CONFLICT_CODE))
                    {
                        if (ListActiveIngredient == null || ListActiveIngredient.Count == 0)
                        {
                            error += string.Format(MessageImport.KhongXacDinhDuoc, item.CONFLICT_CODE);
                        }
                        else
                        {
                            var active = ListActiveIngredient.FirstOrDefault(o => o.ACTIVE_INGREDIENT_CODE.ToLower() == item.CONFLICT_CODE.Trim().ToLower());
                            if (active == null)
                            {
                                error += string.Format(MessageImport.KhongXacDinhDuoc, item.CONFLICT_CODE);
                            }
                            else
                            {
                                serAdo.CONFLICT_CODE = active.ACTIVE_INGREDIENT_CODE;
                                serAdo.CONFLICT_ID = active.ID;
                                serAdo.CONFLICT_NAME = active.ACTIVE_INGREDIENT_NAME;
                            }
                        }
                    }
                    else
                    {
                        error += string.Format(MessageImport.ThieuTruongDL, "mã hoạt chất xung đột");
                    }
                    Inventec.Common.Logging.LogSystem.Error("STEP 3________________");
                    if (!string.IsNullOrEmpty(item.CONFLICT_CODE) && !string.IsNullOrEmpty(item.ACTIVE_INGREDIENT_CODE))
                    {
                         var dataOld = this.ListAcinInteractive.FirstOrDefault(o => o.ACTIVE_INGREDIENT_CODE.ToLower() == item.ACTIVE_INGREDIENT_CODE.Trim().ToLower() && o.CONFLICT_CODE.ToLower() == item.CONFLICT_CODE.Trim().ToLower());
                            if (dataOld != null)
                            {
                                error += string.Format(MessageImport.DaTonTai, item.ACTIVE_INGREDIENT_CODE, item.CONFLICT_CODE);
                            }
                            else
                            {
                                var checkTrung = _service.Where(o => o.ACTIVE_INGREDIENT_CODE.ToLower() == item.ACTIVE_INGREDIENT_CODE.Trim().ToLower() && o.CONFLICT_CODE.ToLower() == item.CONFLICT_CODE.Trim().ToLower()).ToList();
                                if (checkTrung != null && checkTrung.Count > 1)
                                {
                                    error += string.Format(MessageImport.FileImportDaTonTai, item.ACTIVE_INGREDIENT_CODE, item.CONFLICT_CODE);
                                }
                          }
                                        
                    }
                    else
                    {
                        error += string.Format(MessageImport.ThieuTruongDL, "mã hoạt chất và mã hoạt chất xung đột");
                    }
                    if (Encoding.UTF8.GetByteCount(serAdo.CONSEQUENCE) > 1000)
                    {
                        error += "Hậu quả có mã "+ string.Format(MessageImport.Maxlength, item.ACTIVE_INGREDIENT_CODE) ;
                    }
                    if (Encoding.UTF8.GetByteCount(serAdo.MECHANISM) > 1000)
                    {
                        error += "Cơ chế có mã " + string.Format(MessageImport.Maxlength, item.ACTIVE_INGREDIENT_CODE);
                    }
                    Inventec.Common.Logging.LogSystem.Error("STEP 3________________");
                    if  (item.INTERACTIVE_GRADE != null && !string.IsNullOrEmpty(item.INTERACTIVE_GRADE.ToString()))
                    {
                        serAdo.INTERACTIVE_GRADE = item.INTERACTIVE_GRADE;
                        var check = ListInterGrade.FirstOrDefault(o => o.INTERACTIVE_GRADE == item.INTERACTIVE_GRADE);
                        if (check != null)
                        {
                            serAdo.INTERACTIVE_GRADE_ID = check.ID;
                            serAdo.INTERACTIVE_GRADE_NAME = check.INTERACTIVE_GRADE_NAME;
						}
						else
						{
                            error += "Mức độ " + string.Format(MessageImport.KhongHopLe, item.INTERACTIVE_GRADE);
                        }                    
                    }
                    Inventec.Common.Logging.LogSystem.Error("STEP 4________________");
                    serAdo.DESCRIPTION = item.DESCRIPTION;
                    serAdo.INSTRUCTION = item.INSTRUCTION;
                    serAdo.ERROR = error;
                    serAdo.ID = i;
                  
                  
                    _importAdoRef.Add(serAdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataSource(List<ImportADO> dataSource)
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

        private void CheckErrorLine(List<ImportADO> dataSource)
        {
            try
            {
                var checkError = this._ImportAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
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
                    this.showingLines = ShowingLines.Error.ToString();
                    btnShowLineError.Text = "Dòng không lỗi";
                    var errorLine = this._ImportAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                }
                else
                {
                    this.showingLines = ShowingLines.NotError.ToString();
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = this._ImportAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
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
                var row = (ImportADO)gridViewData.GetFocusedRow();
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
                var row = (ImportADO)gridViewData.GetFocusedRow();
                if (row != null)
                {
                    if (this._ImportAdos != null && this._ImportAdos.Count > 0)
                    {
                        this._ImportAdos.Remove(row);
                        var dataCheck = this._ImportAdos.Where(o => o.ACTIVE_INGREDIENT_CODE.ToLower() == row.ACTIVE_INGREDIENT_CODE.Trim().ToLower() && o.CONFLICT_CODE.ToLower() == row.CONFLICT_CODE.Trim().ToLower()).ToList();
                        if (dataCheck != null && dataCheck.Count == 1)
                        {
                            if (!string.IsNullOrEmpty(dataCheck[0].ERROR))
                            {
                                string erro = string.Format(MessageImport.FileImportDaTonTai, dataCheck[0].ACTIVE_INGREDIENT_CODE, dataCheck[0].CONFLICT_CODE);
                                //string[] Codes = dataCheck[0].ERROR.Split('|');
                                dataCheck[0].ERROR = dataCheck[0].ERROR.Replace(erro, "");
                            }
                        }
                        this.notErrorLines = this._ImportAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                        this.errorLines = this._ImportAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                        if (this.showingLines == ShowingLines.NotError.ToString())
                        {
                            if (this.notErrorLines.Count > 0)
                            {
                                SetDataSource(this.notErrorLines);
                            }
                            else
                            {
                                SetDataSource(this.errorLines);
                            }
                        }
                        else if (this.showingLines == ShowingLines.Error.ToString())
                        {
                            if (this.errorLines.Count > 0)
                            {
                                SetDataSource(this.errorLines);
                            }
                            else
                            {
                                SetDataSource(this.notErrorLines);
                            }
                        }
                        else
                        {
                            SetDataSource(this._ImportAdos);
                        }
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
                    ImportADO pData = (ImportADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                List<HIS_ACIN_INTERACTIVE> datas = new List<HIS_ACIN_INTERACTIVE>();
            
                if (this._ImportAdos != null && this._ImportAdos.Count > 0)
                {
                    foreach (var item in this._ImportAdos)
                    {
                        HIS_ACIN_INTERACTIVE ado = new HIS_ACIN_INTERACTIVE();
                        ado.ACTIVE_INGREDIENT_ID = item.ACTIVE_INGREDIENT_ID;
                        ado.CONFLICT_ID = item.CONFLICT_ID;
                        ado.DESCRIPTION = item.DESCRIPTION;
                        ado.INSTRUCTION = item.INSTRUCTION;
                        ado.CONSEQUENCE = item.CONSEQUENCE;
                        ado.MECHANISM = item.MECHANISM;
                        ado.INTERACTIVE_GRADE_ID = item.INTERACTIVE_GRADE_ID;
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
                var dataImports = new BackendAdapter(param).Post<List<HIS_ACIN_INTERACTIVE>>("api/HisAcinInteractive/CreateList", ApiConsumers.MosConsumer, datas, param);
                WaitingManager.Hide();
                if (dataImports != null && dataImports.Count > 0)
                {
                    success = true;
                    btnImport.Enabled = false;
                    LoadCurrentData();
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
                    //BedRoomADO data = (BedRoomADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
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
