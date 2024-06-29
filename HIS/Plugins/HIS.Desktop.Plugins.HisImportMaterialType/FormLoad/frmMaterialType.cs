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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.HisImportMaterialType.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisImportMaterialType.FormLoad
{
    public partial class frmMaterialType : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        List<MaterialTypeImportADO> materialTypeAdos;
        List<MaterialTypeImportADO> currentAdos;
        RefeshReference delegateRefresh;
        bool checkClick;
        bool addSuccess;
        Inventec.Desktop.Common.Modules.Module currentModule;
        #endregion

        #region Contructor
        public frmMaterialType(Inventec.Desktop.Common.Modules.Module module, RefeshReference _delegate)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
                this.delegateRefresh = _delegate;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmMaterialType(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmMaterialType_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }

                btnSave.Enabled = false;
                btnShowLineError.Enabled = false;
                BtnExportErrorLine.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        #endregion

        #region Event
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                List<HIS_MATERIAL_TYPE> listMater = new List<HIS_MATERIAL_TYPE>();
                foreach (var item in materialTypeAdos)
                {
                    HIS_MATERIAL_TYPE mater = new HIS_MATERIAL_TYPE();
                    HIS_SERVICE ser = new HIS_SERVICE();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_MATERIAL_TYPE>(mater, item);
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE>(ser, item);
                    ser.SERVICE_UNIT_ID = item.SERVICE_UNIT_ID;
                    ser.ID = 0;
                    ser.PARENT_ID = null;
                    mater.HIS_SERVICE = ser;
                    mater.ID = 0;
                    listMater.Add(mater);
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("listMater___:", listMater));

                if (listMater != null && listMater.Count > 0)
                {
                    var rs = new BackendAdapter(param).Post<List<HIS_MATERIAL_TYPE>>("api/HisMaterialType/CreateList", ApiConsumers.MosConsumer, listMater, param);
                    if (rs != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("rs___:", rs));
                        success = true;
                        btnSave.Enabled = false;
                        BackendDataWorker.Reset<V_HIS_MATERIAL_TYPE>();
                        BackendDataWorker.Reset<V_HIS_SERVICE>();
                        BackendDataWorker.Reset<HIS_MATERIAL_TYPE>();
                        if (this.delegateRefresh != null)
                        {
                            this.delegateRefresh();
                        }
                    }
                }

                WaitingManager.Hide();
                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Btn_Show_Error_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (MaterialTypeImportADO)gridViewMaterialType.GetFocusedRow();
                if (row != null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(row.ERROR);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Btn_Delete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (MaterialTypeImportADO)gridViewMaterialType.GetFocusedRow();
                if (row != null)
                {
                    if (materialTypeAdos != null && materialTypeAdos.Count > 0)
                    {
                        materialTypeAdos.Remove(row);

                        gridControlMaterialType.DataSource = null;
                        gridControlMaterialType.DataSource = materialTypeAdos;
                        CheckErrorLine();
                        if (checkClick)
                        {
                            if (btnShowLineError.Text == "Dòng lỗi")
                            {
                                btnShowLineError.Text = "Dòng không lỗi";
                            }
                            else
                            {
                                btnShowLineError.Text = "Dòng lỗi";
                            }
                            btnShowLineError_Click(null, null);
                        }
                    }
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
                checkClick = true;
                if (btnShowLineError.Text == "Dòng lỗi")
                {
                    btnShowLineError.Text = "Dòng không lỗi";
                    var errorLine = materialTypeAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = materialTypeAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMaterialType_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    MaterialTypeImportADO data = (MaterialTypeImportADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "ErrorLine")
                    {
                        if (!string.IsNullOrEmpty(data.ERROR))
                        {
                            e.RepositoryItem = Btn_ErrorLine;
                        }
                    }
                    else if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = Btn_Delete;
                    }
                    else if (e.Column.FieldName == "CPNG")
                    {
                        e.RepositoryItem = Item_Check;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMaterialType_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MaterialTypeImportADO pData = (MaterialTypeImportADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(pData.CREATE_TIME.ToString()));
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "HEIN_LIMIT_PRICE_IN_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(pData.HEIN_LIMIT_PRICE_IN_TIME.ToString()));
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao HEIN_LIMIT_PRICE_IN_TIME_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "HEIN_LIMIT_PRICE_INTR_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(pData.HEIN_LIMIT_PRICE_INTR_TIME.ToString()));
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao HEIN_LIMIT_PRICE_INTR_TIME_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "HEIN_LIMIT_RATIO_STR")
                    {
                        try
                        {
                            e.Value = pData.HEIN_LIMIT_RATIO;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua CPNG", ex);
                        }
                    }
                    else if (e.Column.FieldName == "HEIN_LIMIT_RATIO_OLD_STR")
                    {
                        try
                        {
                            e.Value = pData.HEIN_LIMIT_RATIO_OLD;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua CPNG", ex);
                        }
                    }
                    else if (e.Column.FieldName == "KinhDoanh")
                    {
                        try
                        {
                            e.Value = pData.IS_BUSINESS == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua KinhDoanh", ex);
                        }
                    }
                    else if (e.Column.FieldName == "BanBangGiaNhap")
                    {
                        try
                        {
                            e.Value = pData.IS_SALE_EQUAL_IMP_PRICE == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua BBGiaNhap", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_NOT_SHOW_TRACKING_DISPLAY")
                    {
                        try
                        {
                            e.Value = pData.IS_NOT_SHOW_TRACKING == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot Không hiển thị trên tờ điều trị", ex);
                        }
                    }   
                    else if (e.Column.FieldName == "BBNhapHSD")
                    {
                        try
                        {
                            e.Value = pData.IS_REQUIRE_HSD == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua BBNhapHSD", ex);
                        }
                    }
                    else if (e.Column.FieldName == "DungNhap")
                    {
                        try
                        {
                            e.Value = pData.IS_STOP_IMP == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua DungNhap", ex);
                        }
                    }
                    else if (e.Column.FieldName == "KhoXuatLe")
                    {
                        try
                        {
                            e.Value = pData.IS_ALLOW_EXPORT_ODD == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua KhoXuatLe", ex);
                        }
                    }
                    else if (e.Column.FieldName == "KeLe")
                    {
                        try
                        {
                            e.Value = pData.IS_ALLOW_ODD == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua KeLe", ex);
                        }
                    }
                    else if (e.Column.FieldName == "KiThuatCao")
                    {
                        try
                        {
                            e.Value = pData.IS_IN_KTC_FEE == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua KiThuatCao", ex);
                        }
                    }
                    else if (e.Column.FieldName == "Stent")
                    {
                        try
                        {
                            e.Value = pData.IS_STENT == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua Stent", ex);
                        }
                    }
                    else if (e.Column.FieldName == "HaoPhi")
                    {
                        try
                        {
                            e.Value = pData.IS_AUTO_EXPEND == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua HaoPhi", ex);
                        }
                    }
                    else if (e.Column.FieldName == "HoaChat")
                    {
                        try
                        {
                            e.Value = pData.IS_CHEMICAL_SUBSTANCE == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua HoaChat", ex);
                        }
                    }
                    else if (e.Column.FieldName == "CPNG")
                    {
                        try
                        {
                            e.Value = pData.IS_OUT_PARENT_FEE == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua CPNG", ex);
                        }
                    }
                    //else if (e.Column.FieldName == "ACTIVE_ITEM")
                    //{
                    //    try
                    //    {
                    //        if (status == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    //            e.Value = Inventec.Common.Resource.Get.Value("frmHisMisuMaterialTypeType.HoatDong", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    //        else
                    //            e.Value = Inventec.Common.Resource.Get.Value("frmHisMisuMaterialTypeType.TamKhoa", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Inventec.Common.Logging.LogSystem.Error(ex);
                    //    }
                    //}
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(pData.MODIFY_TIME.ToString()));
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua MODIFY_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "BILL_OPTION_STR")
                    {
                        try
                        {
                            if (pData.BILL_OPTION == null)
                                e.Value = "Hóa đơn thường";
                            else if (pData.BILL_OPTION == 1)
                                e.Value = "Tách chênh lệch vào hóa đơn dịch vụ";
                            else if (pData.BILL_OPTION == 2)
                                e.Value = "Hóa đơn dịch vụ";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua BILL_OPTION", ex);
                        }
                    }
                    //else if (e.Column.FieldName == "IS_NOT_SHOW_TRACKING_DISPLAY")
                    //{
                    //    try
                    //    {
                    //        e.Value = pData.IS_NOT_SHOW_TRACKING != null && pData.IS_NOT_SHOW_TRACKING == 1;
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua BILL_OPTION", ex);
                    //    }
                    //}
                    else if (e.Column.FieldName == "VatTuDinhDanh")
                    {
                        try
                        {
                            e.Value = pData.IS_IDENTITY_MANAGEMENT == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua VatTuDinhDanh", ex);
                        }
                    }
                    }
                }
            
    
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                var source = System.IO.Path.Combine(Application.StartupPath
                + "/Tmp/Imp", "IMPORT_MATERIAL.xlsx");

                if (File.Exists(source))
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                    saveFileDialog1.Title = "Save File";
                    saveFileDialog1.FileName = "IMPORT_MATERIAL";
                    saveFileDialog1.DefaultExt = "xlsx";
                    saveFileDialog1.Filter = "Excel files (*.xlsx)|All files (*.*)";
                    saveFileDialog1.FilterIndex = 2;
                    saveFileDialog1.RestoreDirectory = true;

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(source, saveFileDialog1.FileName, true);
                        DevExpress.XtraEditors.XtraMessageBox.Show("Tải file thành công");
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy file import");
                }
            }
            catch (Exception ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Tải file thất bại");
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                addSuccess = false;

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    WaitingManager.Show();
                    var import = new Inventec.Common.ExcelImport.Import();
                    if (import.ReadFileExcel(ofd.FileName))
                    {
                        var hisMaterialTypeImport = import.GetWithCheck<MaterialTypeImportADO>(0);
                        if (hisMaterialTypeImport != null && hisMaterialTypeImport.Count > 0)
                        {
                            List<MaterialTypeImportADO> listAfterRemove = new List<MaterialTypeImportADO>();

                            foreach (var item in hisMaterialTypeImport)
                            {
                                listAfterRemove.Add(item);
                            }

                            foreach (var item in hisMaterialTypeImport)
                            {
                                bool checkNull = string.IsNullOrEmpty(item.MATERIAL_TYPE_CODE)
                                    && string.IsNullOrEmpty(item.MATERIAL_TYPE_NAME)
                                    && string.IsNullOrEmpty(item.SERVICE_UNIT_CODE)
                                    && item.NUM_ORDER_STR == null
                                    && string.IsNullOrEmpty(item.NATIONAL_NAME)
                                    && string.IsNullOrEmpty(item.HEIN_SERVICE_TYPE_CODE)
                                    && string.IsNullOrEmpty(item.MANUFACTURER_CODE)
                                    && item.IMP_VAT_RATIO_STR == null
                                    && string.IsNullOrEmpty(item.STOP_IMP)
                                    && string.IsNullOrEmpty(item.STENT)
                                    && string.IsNullOrEmpty(item.SALE_EQUAL_IMP_PRICE)
                                    && string.IsNullOrEmpty(item.HEIN_SERVICE_BHYT_CODE)
                                    && string.IsNullOrEmpty(item.HEIN_SERVICE_BHYT_NAME)
                                    && string.IsNullOrEmpty(item.HEIN_ORDER)
                                    && item.HEIN_LIMIT_RATIO_OLD_STR == null
                                    && item.HEIN_LIMIT_RATIO_STR == null
                                    && item.HEIN_LIMIT_PRICE_STR == null
                                    && item.HEIN_LIMIT_PRICE_OLD_STR == null
                                    && string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_IN_TIME_STR)
                                    && string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_INTR_TIME_STR)
                                    && item.INTERNAL_PRICE_STR == null
                                    && string.IsNullOrEmpty(item.REQUIRE_HSD)
                                    && string.IsNullOrEmpty(item.OUT_PARENT_FEE)
                                    && string.IsNullOrEmpty(item.IN_KTC_FEE)
                                    && string.IsNullOrEmpty(item.CHEMICAL_SUBSTANCE)
                                    && string.IsNullOrEmpty(item.BUSINESS)
                                    && string.IsNullOrEmpty(item.AUTO_EXPEND)
                                    && string.IsNullOrEmpty(item.ALLOW_EXPORT_ODD)
                                    && string.IsNullOrEmpty(item.DESCRIPTION)
                                    && string.IsNullOrEmpty(item.CONCENTRA)
                                    && string.IsNullOrEmpty(item.PARENT_CODE)
                                    && string.IsNullOrEmpty(item.TDL_GENDER_CODE)
                                    && string.IsNullOrEmpty(item.ALERT_MAX_IN_PRESCRIPTION_STR)
                                    && string.IsNullOrEmpty(item.ALERT_MAX_IN_DAY_STR)
                                    && string.IsNullOrEmpty(item.MATERIAL_GROUP_BHYT)
                                    && string.IsNullOrEmpty(item.NOT_SHOW_TRACKING_STR)
                                    && item.IMP_PRICE_STR == null
                                    && item.COGS_STR == null
                                    && item.ALERT_MIN_IN_STOCK_STR == null
                                    && item.ALERT_EXPIRED_DATE_STR == null;

                                if (checkNull)
                                {
                                    listAfterRemove.Remove(item);
                                }
                            }
                            this.currentAdos = listAfterRemove;
                            if (this.currentAdos != null && this.currentAdos.Count > 0)
                            {
                                btnSave.Enabled = true;
                                checkClick = false;
                                btnShowLineError.Enabled = true;
                                BtnExportErrorLine.Enabled = true;
                                materialTypeAdos = new List<MaterialTypeImportADO>();
                                addMaterialTypeToProcessList(currentAdos, ref materialTypeAdos);
                                bool exist = (materialTypeAdos.FirstOrDefault(o => o.IS_LESS_MANUFACTURER) != null);
                                if (materialTypeAdos != null && materialTypeAdos.Count > 0 && exist)
                                {
                                    var less = materialTypeAdos.Where(o => o.IS_LESS_MANUFACTURER).ToList();
                                    frmWarning frm = new frmWarning(less, (DelegateRefreshData)DelegateWarning);
                                    frm.ShowDialog();
                                }

                                if (addSuccess)
                                {
                                    addMaterialTypeToProcessList(currentAdos, ref materialTypeAdos);
                                }

                                SetDataSource(materialTypeAdos);
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
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.currentAdos != null && this.currentAdos.Count > 0)
                {
                    btnSave.Enabled = true;
                    checkClick = false;
                    btnShowLineError.Enabled = true;
                    BtnExportErrorLine.Enabled = true;
                    materialTypeAdos = new List<MaterialTypeImportADO>();
                    addMaterialTypeToProcessList(currentAdos, ref materialTypeAdos);
                    bool exist = (materialTypeAdos.FirstOrDefault(o => o.IS_LESS_MANUFACTURER) != null);
                    if (materialTypeAdos != null && materialTypeAdos.Count > 0 && exist)
                    {
                        var less = materialTypeAdos.Where(o => o.IS_LESS_MANUFACTURER).ToList();
                        frmWarning frm = new frmWarning(less, (DelegateRefreshData)DelegateWarning);
                        frm.ShowDialog();
                    }
                    SetDataSource(materialTypeAdos);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BtnExportErrorLine_Click(object sender, EventArgs e)
        {
            try
            {
                if (!BtnExportErrorLine.Enabled) return;

                if (!this.materialTypeAdos.Exists(o => !String.IsNullOrWhiteSpace(o.ERROR)))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy dòng lỗi");
                    return;
                }

                Inventec.Common.FlexCellExport.Store store = new Inventec.Common.FlexCellExport.Store(true);

                string templateFile = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Exp", "DanhSachLoiImportLoaiVatTu.xlsx");

                //chọn đường dẫn
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Excel 2007 later file (*.xlsx)|*.xlsx|Excel 97-2003 file(*.xls)|*.xls";
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //getdata
                    WaitingManager.Show();

                    if (String.IsNullOrEmpty(templateFile))
                    {
                        store = null;
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Không tìm thấy file", templateFile));
                        return;
                    }

                    store.ReadTemplate(System.IO.Path.GetFullPath(templateFile));
                    if (store.TemplatePath == "")
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Biểu mẫu đang mở hoặc không tồn tại file template. Vui lòng kiểm tra lại. (" + templateFile + ")");
                        return;
                    }

                    var errorList = this.materialTypeAdos.Where(o => !String.IsNullOrWhiteSpace(o.ERROR)).ToList();
                    ProcessData(errorList, ref store);
                    WaitingManager.Hide();

                    if (store != null)
                    {
                        try
                        {
                            if (store.OutFile(saveFileDialog1.FileName))
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show("Xuất file thành công");

                                if (MessageBox.Show("Bạn có muốn mở file?",
                                    "Thông báo", MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question) == DialogResult.Yes)
                                    System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Xử lý thất bại");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessData(List<MaterialTypeImportADO> errorList, ref Inventec.Common.FlexCellExport.Store store)
        {
            try
            {
                if (errorList != null && errorList.Count > 0)
                {
                    //errorList.ForEach(o => o.ErrorDesc = o.ERROR);

                    Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                    Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                    store.SetCommonFunctions();
                    objectTag.AddObjectData(store, "Export", errorList);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region private method
        private void CheckErrorLine()
        {
            try
            {
                if (materialTypeAdos != null && materialTypeAdos.Count > 0)
                {
                    var checkError = materialTypeAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
                    if (!checkError)
                    {
                        btnSave.Enabled = true;
                    }
                    else
                    {
                        btnSave.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataSource(List<MaterialTypeImportADO> dataSource)
        {
            try
            {
                gridControlMaterialType.BeginUpdate();
                gridControlMaterialType.DataSource = null;
                gridControlMaterialType.DataSource = dataSource;
                gridControlMaterialType.EndUpdate();
                CheckErrorLine();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void convertDateStringToTimeNumber(string date, ref long? dateTime, ref string check)
        {
            try
            {
                if (date.Length > 14)
                {
                    check = Message.MessageImport.Maxlength;
                    return;
                }

                if (date.Length < 14)
                {
                    check = Message.MessageImport.KhongHopLe;
                    return;
                }

                if (date.Length > 0)
                {
                    if (!Inventec.Common.DateTime.Check.IsValidTime(date))
                    {
                        check = Message.MessageImport.KhongHopLe;
                        return;
                    }
                    dateTime = Inventec.Common.TypeConvert.Parse.ToInt64(date);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void addMaterialTypeToProcessList(List<MaterialTypeImportADO> _material, ref List<MaterialTypeImportADO> _materialRef)
        {
            try
            {
                _materialRef = new List<MaterialTypeImportADO>();
                long i = 0;
                foreach (var item in _material)
                {
                    i++;
                    string error = "";
                    var mateAdo = new MaterialTypeImportADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MaterialTypeImportADO>(mateAdo, item);
                    if (!string.IsNullOrEmpty(item.PARENT_CODE))
                    {
                        if (!CheckMaxLenth(item.PARENT_CODE, 25))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã cha");
                            mateAdo.PARENT_CODE_ERROR = 1;
                        }
                        var getData = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.MATERIAL_TYPE_CODE == item.PARENT_CODE);
                        if (getData != null)
                        {
                            mateAdo.PARENT_ID = getData.ID;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã cha");
                            mateAdo.PARENT_CODE_ERROR = 1;
                        }
                    }




                    if (!string.IsNullOrEmpty(item.STOP_IMP))
                    {
                        if (item.STOP_IMP.Trim().ToLower() == "x")
                        {
                            mateAdo.IS_STOP_IMP = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Dừng nhập");
                            mateAdo.STOP_IMP_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.IN_KTC_FEE))
                    {
                        if (item.IN_KTC_FEE.Trim().ToLower() == "x")
                        {
                            mateAdo.IS_IN_KTC_FEE = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Kĩ thuật cao");
                            mateAdo.IN_KTC_FEE_ERROR = 1;
                        }
                    }
                    if (!string.IsNullOrEmpty(item.MATERIAL_GROUP_BHYT))
                    {
                        if (Inventec.Common.String.CountVi.Count(item.MATERIAL_GROUP_BHYT) > 500)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Nhóm Vật tư BHYT");
                            mateAdo.MATERIAL_GROUP_BHYT_ERROR = 1;
                        }
                    }
                    
                    if (!string.IsNullOrEmpty(item.ALLOW_ODD))
                    {
                        if (item.ALLOW_ODD.Trim().ToLower() == "x")
                        {
                            mateAdo.IS_ALLOW_ODD = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Kê lẻ");
                            mateAdo.ALLOW_ODD_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.STENT))
                    {
                        if (item.STENT.Trim().ToLower() == "x")
                        {
                            mateAdo.IS_STENT = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Là stent");
                            mateAdo.STENT_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.REQUIRE_HSD))
                    {
                        if (item.REQUIRE_HSD.Trim().ToLower() == "x")
                        {
                            mateAdo.IS_REQUIRE_HSD = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Bắt buộc nhập HSD");
                            mateAdo.REQUIRE_HSD_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.CHEMICAL_SUBSTANCE))
                    {
                        if (item.CHEMICAL_SUBSTANCE.Trim().ToLower() == "x")
                        {
                            mateAdo.IS_CHEMICAL_SUBSTANCE = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Là hóa chất");
                            mateAdo.CHEMICAL_SUBSTANCE_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.BUSINESS))
                    {
                        if (item.BUSINESS.Trim().ToLower() == "x")
                        {
                            mateAdo.IS_BUSINESS = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Là vật tư kinh doanh");
                            mateAdo.BUSINESS_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.AUTO_EXPEND))
                    {
                        if (item.AUTO_EXPEND.Trim().ToLower() == "x")
                        {
                            mateAdo.IS_AUTO_EXPEND = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Tự động hao phí");
                            mateAdo.AUTO_EXPEND_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.ALLOW_EXPORT_ODD))
                    {
                        if (item.ALLOW_EXPORT_ODD.Trim().ToLower() == "x")
                        {
                            mateAdo.IS_ALLOW_EXPORT_ODD = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Xuất lẻ");
                            mateAdo.ALLOW_EXPORT_ODD_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.OUT_PARENT_FEE))
                    {
                        if (item.OUT_PARENT_FEE.Trim().ToLower() == "x")
                        {
                            mateAdo.IS_OUT_PARENT_FEE = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "CP ngoài gói");
                            mateAdo.OUT_PARENT_FEE_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.SALE_EQUAL_IMP_PRICE))
                    {
                        if (item.SALE_EQUAL_IMP_PRICE.Trim().ToLower() == "x")
                        {
                            mateAdo.IS_SALE_EQUAL_IMP_PRICE = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Bán bằng giá nhập");
                            mateAdo.SALE_EQUAL_IMP_PRICE_ERROR = 1;
                        }
                    }
                    if (!string.IsNullOrEmpty(item.NOT_SHOW_TRACKING_STR))
                    {
                        if (item.NOT_SHOW_TRACKING_STR.Trim().ToLower() == "x")
                        {
                            mateAdo.IS_NOT_SHOW_TRACKING = 1;
                        }
                    }
                    if (!string.IsNullOrEmpty(item.HEIN_SERVICE_TYPE_CODE))
                    {
                        if (!CheckMaxLenth(item.HEIN_SERVICE_TYPE_CODE, 10))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã loại dịch vụ BHYT");
                            mateAdo.HEIN_SERVICE_TYPE_CODE_ERROR = 1;
                        }

                        var getData = BackendDataWorker.Get<HIS_HEIN_SERVICE_TYPE>().FirstOrDefault(o => o.HEIN_SERVICE_TYPE_CODE == item.HEIN_SERVICE_TYPE_CODE);
                        if (getData != null)
                        {
                            mateAdo.HEIN_SERVICE_TYPE_ID = getData.ID;
                            mateAdo.HEIN_SERVICE_TYPE_NAME = getData.HEIN_SERVICE_TYPE_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã loại dịch vụ BHYT");
                            mateAdo.HEIN_SERVICE_TYPE_CODE_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.SERVICE_UNIT_CODE))
                    {
                        if (!CheckMaxLenth(item.SERVICE_UNIT_CODE, 3))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã đơn vị tính");
                            mateAdo.SERVICE_UNIT_CODE_ERROR = 1;
                        }

                        var getData = BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(o => o.SERVICE_UNIT_CODE == item.SERVICE_UNIT_CODE);
                        if (getData != null)
                        {
                            mateAdo.SERVICE_UNIT_ID = getData.ID;
                            mateAdo.SERVICE_UNIT_NAME = getData.SERVICE_UNIT_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã đơn vị tính");
                            mateAdo.SERVICE_UNIT_CODE_ERROR = 1;
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã đơn vị tính");
                        mateAdo.SERVICE_UNIT_CODE_ERROR = 1;
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_IN_TIME_STR))
                    {
                        long? dateTime = null;
                        string check = "";
                        convertDateStringToTimeNumber(item.HEIN_LIMIT_PRICE_IN_TIME_STR, ref dateTime, ref check);
                        if (dateTime != null && string.IsNullOrEmpty(check))
                        {
                            mateAdo.HEIN_LIMIT_PRICE_IN_TIME = dateTime;
                        }
                        else
                        {
                            error += string.Format(check, "Thời gian theo ngày vào viện");
                            mateAdo.HEIN_LIMIT_PRICE_IN_TIME_STR_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_INTR_TIME_STR))
                    {
                        long? dateTime = null;
                        string check = "";
                        convertDateStringToTimeNumber(item.HEIN_LIMIT_PRICE_INTR_TIME_STR, ref dateTime, ref check);
                        if (dateTime != null && string.IsNullOrEmpty(check))
                        {
                            mateAdo.HEIN_LIMIT_PRICE_INTR_TIME = dateTime;
                        }
                        else
                        {
                            error += string.Format(check, "Thời gian theo ngày chỉ định");
                            mateAdo.HEIN_LIMIT_PRICE_INTR_TIME_STR_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.NUM_ORDER_STR))
                    {
                        if (Inventec.Common.Number.Check.IsNumber(item.NUM_ORDER_STR))
                        {
                            mateAdo.NUM_ORDER = Inventec.Common.TypeConvert.Parse.ToInt64(item.NUM_ORDER_STR);
                            if (!CheckMaxLenth(mateAdo.NUM_ORDER.ToString(), 19) || mateAdo.NUM_ORDER < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "STT hiện");
                                mateAdo.NUM_ORDER_STR_ERROR = 1;
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "STT hiện");
                            mateAdo.NUM_ORDER_STR_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_SERVICE_BHYT_CODE))
                    {
                        if (!CheckMaxLenth(item.HEIN_SERVICE_BHYT_CODE, 100))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã dịch vụ BHYT");
                            mateAdo.HEIN_SERVICE_BHYT_CODE_ERROR = 1;
                        }

                        if (string.IsNullOrEmpty(item.HEIN_SERVICE_BHYT_NAME))
                        {
                            error += string.Format(Message.MessageImport.CoThiPhaiNhap, "Mã dịch vụ BHYT", "Tên dịch vụ BHYT");
                            mateAdo.HEIN_SERVICE_BHYT_CODE_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_SERVICE_BHYT_NAME))
                    {
                        if (!CheckMaxLenth(item.HEIN_SERVICE_BHYT_NAME, 500))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Tên dịch vụ BHYT");
                            mateAdo.HEIN_SERVICE_BHYT_NAME_ERROR = 1;
                        }

                        if (string.IsNullOrEmpty(item.HEIN_SERVICE_BHYT_CODE))
                        {
                            error += string.Format(Message.MessageImport.CoThiPhaiNhap, "Tên dịch vụ BHYT", "Mã dịch vụ BHYT");
                            mateAdo.HEIN_SERVICE_BHYT_NAME_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_ORDER))
                    {
                        if (!CheckMaxLenth(item.HEIN_ORDER, 20))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "STT BHYT");
                            mateAdo.HEIN_ORDER_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_LIMIT_RATIO_STR))
                    {
                        if (Inventec.Common.Number.Check.IsDecimal(item.HEIN_LIMIT_RATIO_STR))
                        {
                            mateAdo.HEIN_LIMIT_RATIO = Inventec.Common.TypeConvert.Parse.ToDecimal(item.HEIN_LIMIT_RATIO_STR);
                            if (mateAdo.HEIN_LIMIT_RATIO.Value > 1 || mateAdo.HEIN_LIMIT_RATIO < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Tỉ lệ trần mới");
                                mateAdo.HEIN_LIMIT_RATIO_STR_ERROR = 1;
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Tỉ lệ trần mới");
                            mateAdo.HEIN_LIMIT_RATIO_STR_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_LIMIT_RATIO_OLD_STR))
                    {
                        if (Inventec.Common.Number.Check.IsDecimal(item.HEIN_LIMIT_RATIO_OLD_STR))
                        {
                            mateAdo.HEIN_LIMIT_RATIO_OLD = Inventec.Common.TypeConvert.Parse.ToDecimal(item.HEIN_LIMIT_RATIO_OLD_STR);
                            if (mateAdo.HEIN_LIMIT_RATIO_OLD.Value > 1 || mateAdo.HEIN_LIMIT_RATIO < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Tỉ lệ trần cũ");
                                mateAdo.HEIN_LIMIT_RATIO_OLD_STR_ERROR = 1;
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Tỉ lệ trần cũ");
                            mateAdo.HEIN_LIMIT_RATIO_OLD_STR_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_OLD_STR))
                    {
                        if (Inventec.Common.Number.Check.IsDecimal(item.HEIN_LIMIT_PRICE_OLD_STR))
                        {
                            mateAdo.HEIN_LIMIT_PRICE_OLD = Inventec.Common.TypeConvert.Parse.ToDecimal(item.HEIN_LIMIT_PRICE_OLD_STR);
                            if (mateAdo.HEIN_LIMIT_PRICE_OLD.Value > 99999999999999 || mateAdo.HEIN_LIMIT_PRICE_OLD < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Giá trần cũ");
                                mateAdo.HEIN_LIMIT_PRICE_OLD_STR_ERROR = 1;
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Giá trần cũ");
                            mateAdo.HEIN_LIMIT_PRICE_OLD_STR_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.IMP_VAT_RATIO_STR))
                    {
                        if (Inventec.Common.Number.Check.IsDecimal(item.IMP_VAT_RATIO_STR))
                        {
                            mateAdo.IMP_VAT_RATIO = Inventec.Common.TypeConvert.Parse.ToDecimal(item.IMP_VAT_RATIO_STR);
                            if (mateAdo.IMP_VAT_RATIO.Value > 1 || mateAdo.IMP_VAT_RATIO < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "VAT ");
                                mateAdo.IMP_VAT_RATIO_STR_ERROR = 1;
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "VAT");
                            mateAdo.IMP_VAT_RATIO_STR_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.INTERNAL_PRICE_STR))
                    {
                        if (Inventec.Common.Number.Check.IsDecimal(item.INTERNAL_PRICE_STR))
                        {
                            mateAdo.INTERNAL_PRICE = Inventec.Common.TypeConvert.Parse.ToDecimal(item.INTERNAL_PRICE_STR);
                            if (mateAdo.INTERNAL_PRICE.Value > 99999999999999 || mateAdo.INTERNAL_PRICE < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Giá nội bộ");
                                mateAdo.INTERNAL_PRICE_STR_ERROR = 1;
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Giá nội bộ");
                            mateAdo.INTERNAL_PRICE_STR_ERROR = 1;
                        }
                    }



                    if (!string.IsNullOrEmpty(item.IMP_PRICE_STR))
                    {
                        if (Inventec.Common.Number.Check.IsDecimal(item.IMP_PRICE_STR))
                        {
                            mateAdo.IMP_PRICE = Inventec.Common.TypeConvert.Parse.ToDecimal(item.IMP_PRICE_STR);
                            if (mateAdo.IMP_PRICE.Value > 99999999999999 || mateAdo.IMP_PRICE < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Giá nhập");
                                mateAdo.IMP_PRICE_STR_ERROR = 1;
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Giá nhập");
                            mateAdo.IMP_PRICE_STR_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.ALERT_EXPIRED_DATE_STR))
                    {
                        if (Inventec.Common.Number.Check.IsNumber(item.ALERT_EXPIRED_DATE_STR))
                        {
                            mateAdo.ALERT_EXPIRED_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(item.ALERT_EXPIRED_DATE_STR);

                            if ((mateAdo.ALERT_EXPIRED_DATE ?? 0) > 999999999999999999 || (mateAdo.ALERT_EXPIRED_DATE ?? 0) < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Cảnh báo HSD");
                                mateAdo.ALERT_EXPIRED_DATE_STR_ERROR = 1;
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Cảnh báo HSD");
                            mateAdo.ALERT_EXPIRED_DATE_STR_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.ALERT_MIN_IN_STOCK_STR))
                    {
                        if (Inventec.Common.Number.Check.IsDecimal(item.ALERT_MIN_IN_STOCK_STR))
                        {
                            mateAdo.ALERT_MIN_IN_STOCK = Inventec.Common.TypeConvert.Parse.ToDecimal(item.ALERT_MIN_IN_STOCK_STR);

                            if ((mateAdo.ALERT_MIN_IN_STOCK ?? 0) > 9999999999999999 || (mateAdo.ALERT_MIN_IN_STOCK ?? 0) < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Cảnh báo tồn kho");
                                mateAdo.ALERT_MIN_IN_STOCK_STR_ERROR = 1;
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Cảnh báo tồn kho");
                            mateAdo.ALERT_MIN_IN_STOCK_STR_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_STR))
                    {
                        if (Inventec.Common.Number.Check.IsDecimal(item.HEIN_LIMIT_PRICE_STR))
                        {
                            mateAdo.HEIN_LIMIT_PRICE = Inventec.Common.TypeConvert.Parse.ToDecimal(item.HEIN_LIMIT_PRICE_STR);
                            if (mateAdo.HEIN_LIMIT_PRICE.Value > 99999999999999 || mateAdo.HEIN_LIMIT_PRICE < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Giá trần mới");
                                mateAdo.HEIN_LIMIT_PRICE_STR_ERROR = 1;
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Giá trần mới");
                            mateAdo.HEIN_LIMIT_PRICE_STR_ERROR = 1;
                        }
                    }

                    if ((!string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_STR) || !string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_OLD_STR)) && (!string.IsNullOrEmpty(item.HEIN_LIMIT_RATIO_STR) || !string.IsNullOrEmpty(item.HEIN_LIMIT_RATIO_OLD_STR)))
                    {
                        error += string.Format(Message.MessageImport.ChiDuocNhapGiaHoacTiLeTran);
                        mateAdo.HEIN_LIMIT_PRICE_STR_ERROR = 1;
                        mateAdo.HEIN_LIMIT_PRICE_OLD_STR_ERROR = 1;
                        mateAdo.HEIN_LIMIT_RATIO_STR_ERROR = 1;
                        mateAdo.HEIN_LIMIT_RATIO_OLD_STR_ERROR = 1;
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_IN_TIME_STR) && !string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_INTR_TIME_STR))
                    {
                        error += string.Format(Message.MessageImport.ChiDuocNhapTGVaoVienHoacTGChiDinh);
                        mateAdo.HEIN_LIMIT_PRICE_IN_TIME_STR_ERROR = 1;
                        mateAdo.HEIN_LIMIT_PRICE_INTR_TIME_STR_ERROR = 1;
                    }

                    if (!string.IsNullOrEmpty(item.MATERIAL_TYPE_CODE))
                    {
                        if (!CheckMaxLenth(item.MATERIAL_TYPE_CODE, 25))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã vật tư");
                            mateAdo.MATERIAL_TYPE_CODE_ERROR = 1;
                        }
                        var check = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Exists(o => o.MATERIAL_TYPE_CODE == item.MATERIAL_TYPE_CODE);
                        if (check)
                        {
                            error += string.Format(Message.MessageImport.DaTonTai, "Mã vật tư");
                            mateAdo.MATERIAL_TYPE_CODE_ERROR = 1;
                        }

                        var checkExel = _materialRef.FirstOrDefault(o => o.MATERIAL_TYPE_CODE == item.MATERIAL_TYPE_CODE);
                        if (checkExel != null)
                        {
                            error += string.Format(Message.MessageImport.TonTaiTrungNhauTrongFileImport, "Mã vật tư");
                            mateAdo.MATERIAL_TYPE_CODE_ERROR = 1;
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã vật tư");
                        mateAdo.MATERIAL_TYPE_CODE_ERROR = 1;
                    }

                    if (!string.IsNullOrEmpty(item.MATERIAL_TYPE_NAME))
                    {
                        if (!CheckMaxLenth(item.MATERIAL_TYPE_NAME, 1500))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Tên vật tư");
                            mateAdo.MATERIAL_TYPE_NAME_ERROR = 1;
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Tên vật tư");
                        mateAdo.MATERIAL_TYPE_NAME_ERROR = 1;
                    }

                    if (!string.IsNullOrEmpty(item.PACKING_TYPE_NAME))
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.PACKING_TYPE_NAME, 300))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Loại đóng gói");
                            mateAdo.PACKING_TYPE_NAME_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.NATIONAL_NAME))
                    {
                        if (!CheckMaxLenth(item.NATIONAL_NAME, 100))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Tên quốc gia");
                            mateAdo.NATIONAL_NAME_ERROR = 1;
                        }
                        else
                        {
                            mateAdo.NATIONAL_NAME = item.NATIONAL_NAME;
                        }
                    }


                    if (!string.IsNullOrEmpty(item.MANUFACTURER_CODE))
                    {
                        if (!CheckMaxLenth(item.MANUFACTURER_CODE, 6))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã hãng sản xuất");
                            mateAdo.MANUFACTURER_CODE_ERROR = 1;
                        }

                        var package = BackendDataWorker.Get<HIS_MANUFACTURER>().FirstOrDefault(o => o.MANUFACTURER_CODE == item.MANUFACTURER_CODE);
                        if (package != null)
                        {
                            mateAdo.MANUFACTURER_ID = package.ID;
                            mateAdo.MANUFACTURER_NAME = package.MANUFACTURER_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã hãng sản xuất");
                            mateAdo.IS_LESS_MANUFACTURER = true;
                            mateAdo.MANUFACTURER_CODE_ERROR = 1;
                        }
                    }
                    else if (!string.IsNullOrEmpty(item.MANUFACTURER_NAME))
                    {
                        if (!CheckMaxLenth(item.MANUFACTURER_NAME, 100))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Tên hãng sản xuất");
                            mateAdo.MANUFACTURER_NAME_ERROR = 1;
                        }

                        var manufacturer = BackendDataWorker.Get<HIS_MANUFACTURER>().FirstOrDefault(o => o.MANUFACTURER_NAME == item.MANUFACTURER_NAME);
                        if (manufacturer != null)
                        {
                            mateAdo.MANUFACTURER_ID = manufacturer.ID;
                            mateAdo.MANUFACTURER_CODE = manufacturer.MANUFACTURER_CODE;
                            mateAdo.MANUFACTURER_NAME = manufacturer.MANUFACTURER_NAME;
                            item.MANUFACTURER_NAME = manufacturer.MANUFACTURER_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Tên hãng sản xuất");
                            mateAdo.IS_LESS_MANUFACTURER = true;
                            mateAdo.MANUFACTURER_NAME_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.DESCRIPTION))
                    {
                        if (!CheckMaxLenth(item.DESCRIPTION, 2000))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mô tả");
                            mateAdo.DESCRIPTION_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.CONCENTRA))
                    {
                        if (!CheckMaxLenth(item.CONCENTRA, 100))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Hàm lượng, nồng độ");
                            mateAdo.CONCENTRA_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.ALERT_MAX_IN_PRESCRIPTION_STR))
                    {
                        if (Inventec.Common.Number.Check.IsDecimal(item.ALERT_MAX_IN_PRESCRIPTION_STR))
                        {
                            mateAdo.ALERT_MAX_IN_PRESCRIPTION = Inventec.Common.TypeConvert.Parse.ToDecimal(item.ALERT_MAX_IN_PRESCRIPTION_STR);
                            if ((mateAdo.ALERT_MAX_IN_PRESCRIPTION ?? 0) > 9999999999999999 || (mateAdo.ALERT_MAX_IN_PRESCRIPTION ?? 0) < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "SL tối đa / 1 đơn");
                                mateAdo.ALERT_MAX_IN_PRESCRIPTION_STR_ERROR = 1;
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "SL tối đa / 1 đơn");
                            mateAdo.ALERT_MAX_IN_PRESCRIPTION_STR_ERROR = 1;
                        }
                    }
                    if (!string.IsNullOrEmpty(item.ALERT_MAX_IN_DAY_STR))
                    {
                        if (Inventec.Common.Number.Check.IsDecimal(item.ALERT_MAX_IN_DAY_STR))
                        {
                            mateAdo.ALERT_MAX_IN_DAY = Inventec.Common.TypeConvert.Parse.ToDecimal(item.ALERT_MAX_IN_DAY_STR);
                            if ((mateAdo.ALERT_MAX_IN_DAY ?? 0) > 9999999999999999 || (mateAdo.ALERT_MAX_IN_DAY ?? 0) < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Số lượng tối đa/ngày");
                                mateAdo.ALERT_MAX_IN_DAY_STR_ERROR = 1;
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Số lượng tối đa/ngày");
                            mateAdo.ALERT_MAX_IN_DAY_STR_ERROR = 1;
                        }
                    }
                    if (!string.IsNullOrEmpty(item.COGS_STR))
                    {
                        if (Inventec.Common.Number.Check.IsDecimal(item.COGS_STR))
                        {
                            mateAdo.COGS = Inventec.Common.TypeConvert.Parse.ToDecimal(item.COGS_STR);
                            if ((mateAdo.COGS ?? 0) > 99999999999999 || (mateAdo.COGS ?? 0) < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Giá vốn");
                                mateAdo.COGS_STR_ERROR = 1;
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Giá vốn");
                            mateAdo.COGS_STR_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.TDL_GENDER_CODE))
                    {
                        var gender = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.GENDER_CODE == item.TDL_GENDER_CODE);
                        if (gender != null)
                        {
                            mateAdo.TDL_GENDER_ID = gender.ID;
                            mateAdo.TDL_GENDER_NAME = gender.GENDER_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Giới tính");
                            mateAdo.TDL_GENDER_CODE_ERROR = 1;
                        }
                    }
                    if (!string.IsNullOrEmpty(item.RECORDING_TRANSACTION))
                    {
                        if (!CheckMaxLenth(item.RECORDING_TRANSACTION, 20))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Định khoản kế toán");
                            mateAdo.RECORDING_TRANSACTION_ERROR = 1;
                        }
                        mateAdo.RECORDING_TRANSACTION = item.RECORDING_TRANSACTION;
                    }

                    if (!string.IsNullOrEmpty(item.IDENTITY_MANAGEMEN))
                    {
                        if (item.IDENTITY_MANAGEMEN.Trim().ToLower() == "x")
                        {
                            mateAdo.IS_IDENTITY_MANAGEMENT = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Vật tư định danh");
                            mateAdo.IDENTITY_MANAGEMEN_ERROR = 1;
                        }
                    }

                    mateAdo.ERROR = error;
                    mateAdo.ID = i;
                    mateAdo.IS_LEAF = 1;

                    _materialRef.Add(mateAdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private bool CheckMaxLenth(string Str, int Length)
        {
            try
            {
                return (Str != null && Encoding.UTF8.GetByteCount(Str.Trim()) <= Length);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }
        #endregion

        #region public method
        public void DelegateWarning()
        {
            try
            {
                this.btnRefresh.Enabled = true;
                this.addSuccess = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void gridControlMaterialType_Click(object sender, EventArgs e)
        {

        }
    }
}
