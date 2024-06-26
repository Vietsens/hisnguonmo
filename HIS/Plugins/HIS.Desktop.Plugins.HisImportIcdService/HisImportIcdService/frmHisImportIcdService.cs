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
using DevExpress.Data;
using DevExpress.XtraBars;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.HisImportIcdService;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using SDA.EFMODEL.DataModels;
using HIS.Desktop.Plugins.HisImportIcdService.ADO;
using System.Collections;
using System.Text.RegularExpressions;
using MOS.Filter;
using DevExpress.XtraEditors;

namespace HIS.Desktop.Plugins.HisImportIcdService.HisImportIcdService
{
    public partial class frmHisImportIcdService : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module _Module { get; set; }
        HIS.Desktop.Common.RefeshReference delegateRefresh;
        List<HIS_ICD_SERVICE> ListIcdService { get; set; }
        List<HIS_ICD> ListIcd { get; set; }
        List<HIS_SERVICE> ListService { get; set; }
        List<HIS_ACTIVE_INGREDIENT> ListActiveIngredient { get; set; }
        List<IcdServiceADO> IcdServiceAdos;
        List<IcdServiceADO> CurrentAdos;
        //List<SDA_GROUP> ListSdaGroups { get; set; }
        //List<HIS_DEPARTMENT> ListHisDepartment { get; set; }

        public frmHisImportIcdService()
        {
            InitializeComponent();
        }
        public frmHisImportIcdService(Inventec.Desktop.Common.Modules.Module _module)
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
        public frmHisImportIcdService(Inventec.Desktop.Common.Modules.Module _module, RefeshReference _delegateRefresh)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                this._Module = _module;
                this.delegateRefresh = _delegateRefresh;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
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

        private void frmHisImportIcdService_Load(object sender, EventArgs e)
        {
            try
            {
                if (this._Module != null)
                {
                    this.Text = this._Module.text;
                }
                SetIcon();
                GetData();
                //LoadCurrentDataEmployee();
                //LoadCurrentDataUser();
                //LoadCurrentDataDepartment();
                //LoadCurrentSdaGroup();

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void GetData()
        {
            ListIcdService = BackendDataWorker.Get<HIS_ICD_SERVICE>();
            ListService = BackendDataWorker.Get<HIS_SERVICE>();
            ListActiveIngredient = BackendDataWorker.Get<HIS_ACTIVE_INGREDIENT>();
            ListIcd = BackendDataWorker.Get<HIS_ICD>();
        }


        private void btnDownLoadFile_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_ICD_SERVICE.xlsx");
                CommonParam param = new CommonParam();
                param.Messages = new List<string>();
                if (System.IO.File.Exists(fileName))
                {
                    saveFileDialog1.Title = "Save File";
                    saveFileDialog1.FileName = "IMPORT_ICD_SERVICE";
                    saveFileDialog1.DefaultExt = "xlsx";
                    saveFileDialog1.Filter = "Excel files (*.xlsx)|All files (*.*)";
                    saveFileDialog1.FilterIndex = 2;
                    saveFileDialog1.RestoreDirectory = true;

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        System.IO.File.Copy(fileName, saveFileDialog1.FileName);
                        MessageManager.Show(this.ParentForm, param, true);
                        if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn mở file ngay ?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(saveFileDialog1.FileName);
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
                        var icdServiceImport = import.GetWithCheck<IcdServiceADO>(0);
                        if (icdServiceImport != null && icdServiceImport.Count > 0)
                        {
                            List<IcdServiceADO> listAfterRemove = new List<IcdServiceADO>();
                            foreach (var item in icdServiceImport)
                            {
                                bool checkNull = string.IsNullOrEmpty(item.ICD_CODE);
                                if (!checkNull)
                                {
                                    listAfterRemove.Add(item);
                                }
                            }
                            WaitingManager.Hide();
                            this.CurrentAdos = listAfterRemove;
                            if (this.CurrentAdos != null && this.CurrentAdos.Count > 0)
                            {
                                btnShowLineError.Enabled = true;
                                this.IcdServiceAdos = new List<IcdServiceADO>();
                                addServiceToProcessList(CurrentAdos, ref this.IcdServiceAdos);
                                SetDataSource(this.IcdServiceAdos);

                            }
                        }
                        else
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show(" Import thất bại");

                        }
                    }
                    else
                    {

                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không dọc được file");

                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);

            }
        }
        private void checkErrorLine(List<IcdServiceADO> dataSource)
        {
            try
            {
                var checkerror = this.IcdServiceAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
                if (!checkerror)
                {
                    btnSave.Enabled = true;
                    btnShowLineError.Enabled = false;

                }
                else
                {
                    btnShowLineError.Enabled = true;
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetDataSource(List<IcdServiceADO> dataSource)
        {
            gridControl1.BeginUpdate();
            gridControl1.DataSource = null;
            gridControl1.DataSource = dataSource;
            gridControl1.EndUpdate();
            checkErrorLine(null);

        }
        private void addServiceToProcessList(List<IcdServiceADO> service, ref List<IcdServiceADO> IcdServiceRef)
        {
            try
            {
                IcdServiceRef = new List<IcdServiceADO>();
                long i = 0;
                foreach (var item in service)
                {
                    i++;
                    string error = "";
                    var serAdo = new IcdServiceADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<IcdServiceADO>(serAdo, item);
                    if (!string.IsNullOrEmpty(item.ICD_CODE))
                    {
                        if (item.ICD_CODE.Length > 10)
                        {
                            error += string.Format(Message.MessageImport.MaxLength, "Mã ICD");
                        }
                        var checkIcdCode = BackendDataWorker.Get<HIS_ICD>().Where(o => o.ICD_CODE == item.ICD_CODE).FirstOrDefault();
                        if (checkIcdCode != null)
                            serAdo.ICD_NAME = checkIcdCode.ICD_NAME;
                        else

                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã ICD");
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDl, "Mã ICD");

                    }

                    if (!string.IsNullOrEmpty(item.SERVICE_CODE))
                    {
                        if (item.SERVICE_CODE.Length > 25)
                        {
                            error += string.Format(Message.MessageImport.MaxLength, "Mã dịch vụ - kỹ thuật");
                        }
                        var checkServiceCode = BackendDataWorker.Get<HIS_SERVICE>().Where(o => o.SERVICE_CODE == item.SERVICE_CODE).FirstOrDefault();
                        if (checkServiceCode != null)
                            serAdo.SERVICE_NAME = checkServiceCode.SERVICE_NAME;
                        else
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã dịch vụ - kỹ thuật");
                    }

                    if (!string.IsNullOrEmpty(item.ACTIVE_INGREDIENT_CODE))
                    {
                        if (item.SERVICE_CODE.Length > 15)
                        {
                            error += string.Format(Message.MessageImport.MaxLength, "Mã hoạt chất");
                        }
                        var checkActiveIngredientCode = BackendDataWorker.Get<HIS_ACTIVE_INGREDIENT>().Where(o => o.ACTIVE_INGREDIENT_CODE == item.ACTIVE_INGREDIENT_CODE).FirstOrDefault();
                        if (checkActiveIngredientCode != null)
                        {
                            serAdo.ACTIVE_INGREDIENT_NAME = checkActiveIngredientCode.ACTIVE_INGREDIENT_NAME;
                        }
                        else
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã hoạt chất");
                    }
                    serAdo.IsIndication = !string.IsNullOrEmpty(item.ISINDICATION) && item.ISINDICATION.Trim().ToLower().Equals("x");
                    serAdo.IsContraindication = !string.IsNullOrEmpty(item.ISCONTRAINDICATION) && item.ISCONTRAINDICATION.Trim().ToLower().Equals("x");
                    serAdo.IsWarning = !string.IsNullOrEmpty(item.ISWARNING) && item.ISWARNING.Trim().ToLower().Equals("x");
                    if (item.IsWarning && (item.IsIndication || item.IsContraindication) || (item.IsContraindication && item.IsIndication))
                    {
                        List<string> lst = new List<string>();
                        if (item.IsWarning)
                        {
                            lst.Add("Cảnh báo khi chỉ định");
                        }
                        if (item.IsContraindication)
                        {
                            lst.Add("Chặn chỉ định");
                        }
                        if (item.IsIndication)
                        {
                            lst.Add("Cho phép chỉ định");
                        }
                        error += string.Format("{0} |", string.Format("Chỉ cho phép chọn 1 cột trong các cột {0}",string.Join(",", lst)));
                    }
                    if (string.IsNullOrEmpty(item.SERVICE_CODE) && string.IsNullOrEmpty(item.ACTIVE_INGREDIENT_CODE))
                    {
                        error += string.Format(Message.MessageImport.KhongTheCungTrong, " ");

                    }
                    if (!string.IsNullOrEmpty(item.SERVICE_CODE) && !string.IsNullOrEmpty(item.ACTIVE_INGREDIENT_CODE))
                    {
                        error += string.Format(Message.MessageImport.KhongTheCungCo, " ");

                    }

                    if (!string.IsNullOrEmpty(item.ICD_CODE) && string.IsNullOrEmpty(item.SERVICE_CODE) && !string.IsNullOrEmpty(item.ACTIVE_INGREDIENT_CODE))
                    {
                        var checkTrung = service.Where(p => p.ICD_CODE == item.ICD_CODE && p.ACTIVE_INGREDIENT_CODE == item.ACTIVE_INGREDIENT_CODE).ToList();
                        if (checkTrung != null && checkTrung.Count > 1)
                        {
                            error += string.Format(Message.MessageImport.FileImportDaTonTai, item.ICD_CODE, item.ACTIVE_INGREDIENT_CODE);

                        }
                    }

                    if (!string.IsNullOrEmpty(item.ICD_CODE) && !string.IsNullOrEmpty(item.SERVICE_CODE) && string.IsNullOrEmpty(item.ACTIVE_INGREDIENT_CODE))
                    {
                        var checkTrung = service.Where(p => p.ICD_CODE == item.ICD_CODE && p.SERVICE_CODE == item.SERVICE_CODE).ToList();
                        if (checkTrung != null && checkTrung.Count > 1)
                        {
                            error += string.Format(Message.MessageImport.FileImportDaTonTai, item.ICD_CODE, item.SERVICE_CODE);

                        }
                    }
                    serAdo.ERROR = error;
                    serAdo.ID = i;
                    IcdServiceRef.Add(serAdo);

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnShowLineError_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnShowLineError.Text == "Dòng lỗi")
                {
                    btnShowLineError.Text = "Dòng không lỗi";
                    var errorLine = this.IcdServiceAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = this.IcdServiceAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);

                }

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                WaitingManager.Show();
                List<HIS_ICD_SERVICE> datas = new List<HIS_ICD_SERVICE>();
                CommonParam param = new CommonParam();
                if (this.IcdServiceAdos != null && this.IcdServiceAdos.Count > 0)
                {
                    foreach (var item in this.IcdServiceAdos)
                    {
                        HIS_ICD_SERVICE ado = new HIS_ICD_SERVICE();
                        ado.ICD_CODE = item.ICD_CODE;
                        ado.ICD_NAME = item.ICD_NAME;
                        ado.CONTRAINDICATION_CONTENT = item.CONTRAINDICATION_CONTENT;
                        ado.IS_INDICATION = item.IsIndication ? (short?)1 : null;
                        ado.IS_CONTRAINDICATION = item.IsContraindication ? (short?)1 : null;
                        ado.IS_WARNING = item.IsWarning ? (short?)1 : null;
                        if (!string.IsNullOrEmpty(item.ACTIVE_INGREDIENT_CODE.Trim()))
                        {
                            ado.ACTIVE_INGREDIENT_ID = BackendDataWorker.Get<HIS_ACTIVE_INGREDIENT>().Where(o => o.ACTIVE_INGREDIENT_CODE == item.ACTIVE_INGREDIENT_CODE).FirstOrDefault().ID;

                        }
                        else
                            ado.ACTIVE_INGREDIENT_ID = null;

                        if (!string.IsNullOrEmpty(item.SERVICE_CODE.Trim()))
                        {
                            ado.SERVICE_ID = BackendDataWorker.Get<HIS_SERVICE>().Where(o => o.SERVICE_CODE == item.SERVICE_CODE).FirstOrDefault().ID;

                        }
                        else
                            ado.SERVICE_ID = null;
                        datas.Add(ado);
                    }
                }
                else
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu rỗng", "Thông báo");
                    return;
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => datas), datas));
                var icdService = new BackendAdapter(param).Post<List<HIS_ICD_SERVICE>>("api/HisIcdService/CreateList", ApiConsumers.MosConsumer, datas, param);
                WaitingManager.Hide();
                if (icdService != null && icdService.Count > 0)

                {
                    success = true;
                    btnSave.Enabled = false;
                    BackendDataWorker.Reset<HIS_ICD_SERVICE>();
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

        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    IcdServiceADO pData = (IcdServiceADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    string error = (gridView1.GetRowCellValue(e.RowHandle, "ERROR") ?? "").ToString();
                    if (e.Column.FieldName == "Error")
                    {
                        if (!string.IsNullOrEmpty(error))
                        {
                            e.RepositoryItem = btnLoi;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnLoi_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (IcdServiceADO)gridView1.GetFocusedRow();
                if (row != null && !string.IsNullOrEmpty(row.ERROR))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(row.ERROR, "Thông báo ");
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (IcdServiceADO)gridView1.GetFocusedRow();
                if (row != null)
                {

                    if (this.IcdServiceAdos != null && this.IcdServiceAdos.Count > 0)
                    {
                        this.IcdServiceAdos.Remove(row);
                        var dataCheck1 = this.IcdServiceAdos.Where(p => p.ICD_CODE == row.ICD_CODE && p.ACTIVE_INGREDIENT_CODE == row.ACTIVE_INGREDIENT_CODE).ToList();
                        var dataCheck2 = this.IcdServiceAdos.Where(p => p.ICD_CODE == row.ICD_CODE && p.SERVICE_CODE == row.SERVICE_CODE).ToList();
                        if ((dataCheck1 != null && dataCheck1.Count == 1) || (dataCheck2 != null && dataCheck2.Count == 1))
                        {
                            if (!string.IsNullOrEmpty(dataCheck1[0].ERROR) || !string.IsNullOrEmpty(dataCheck2[0].ERROR))
                            {
                                string errro1 = string.Format(Message.MessageImport.FileImportDaTonTai, dataCheck1[0].ICD_CODE);
                                string errro2 = string.Format(Message.MessageImport.FileImportDaTonTai, dataCheck2[0].ICD_CODE);
                                dataCheck1[0].ERROR = dataCheck1[0].ERROR.Replace(errro1, "");
                                dataCheck2[0].ERROR = dataCheck2[0].ERROR.Replace(errro2, "");
                            }
                        }
                        SetDataSource(this.IcdServiceAdos);
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnLuu_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {

                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var icdService = (IcdServiceADO)gridView1.GetFocusedRow();
                if (icdService != null)
                {
                    txtContraindicationContent.Text = icdService.CONTRAINDICATION_CONTENT;
                }
                ButtonEdit editor = sender as ButtonEdit;
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupControlContainerContent.ShowPopup(new Point(buttonPosition.X + 532, buttonPosition.Bottom + 200));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAcceptContent_Click(object sender, EventArgs e)
        {
            try
            {
                var icdService = (IcdServiceADO)gridView1.GetFocusedRow();
                if (icdService != null)
                {
                    icdService.CONTRAINDICATION_CONTENT = !string.IsNullOrEmpty(txtContraindicationContent.Text.Trim()) ? txtContraindicationContent.Text : null;
                }
                gridControl1.RefreshDataSource();
                popupControlContainerContent.HidePopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSkipContent_Click(object sender, EventArgs e)
        {
            try
            {
                txtContraindicationContent.Text = "";
                popupControlContainerContent.HidePopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
