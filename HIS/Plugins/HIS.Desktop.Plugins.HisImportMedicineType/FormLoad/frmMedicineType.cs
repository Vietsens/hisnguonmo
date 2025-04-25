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
using HIS.Desktop.Plugins.HisImportMedicineType.ADO;
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

namespace HIS.Desktop.Plugins.HisImportMedicineType.FormLoad
{
    public partial class frmMedicineType : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        List<MedicineTypeImportADO> medicineTypeAdos;
        List<MedicineTypeImportADO> currentAdos;
        RefeshReference delegateRefresh;
        bool addSuccess;
        bool checkClick;
        Inventec.Desktop.Common.Modules.Module currentModule;
        List<V_HIS_MEDICINE_TYPE> listMedicin;
        List<HIS_ACTIVE_INGREDIENT> listAcin;
        #endregion

        #region contructor
        public frmMedicineType(Inventec.Desktop.Common.Modules.Module module, RefeshReference _delegate)
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

        public frmMedicineType(Inventec.Desktop.Common.Modules.Module module)
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

        private void frmMedicineType_Load(object sender, EventArgs e)
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
                LoadValue();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadValue()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisMedicineTypeViewFilter mediFilter = new MOS.Filter.HisMedicineTypeViewFilter();
                mediFilter.ORDER_DIRECTION = "DESC";
                mediFilter.ORDER_FIELD = "MODIFY_TIME";
                listMedicin = new List<V_HIS_MEDICINE_TYPE>();
                listMedicin = new BackendAdapter(param).Get<List<V_HIS_MEDICINE_TYPE>>("/api/HisMedicineType/GetView", ApiConsumers.MosConsumer, mediFilter, param);
                // load medicine acin
                MOS.Filter.HisMedicineTypeAcinFilter acinFilter = new MOS.Filter.HisMedicineTypeAcinFilter();
                acinFilter.ORDER_DIRECTION = "DESC";
                acinFilter.ORDER_FIELD = "MODIFY_TIME";
                listAcin = new List<HIS_ACTIVE_INGREDIENT>();
                listAcin = new BackendAdapter(param).Get<List<HIS_ACTIVE_INGREDIENT>>("/api/HisActiveIngredient/Get", ApiConsumers.MosConsumer, acinFilter, param);

            }
            catch (Exception)
            {
                
                throw;
            }
        }
        #endregion

        #region Event
        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                var source = System.IO.Path.Combine(Application.StartupPath
                + "/Tmp/Imp", "IMPORT_MEDICINE.xlsx");

                if (File.Exists(source))
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                    saveFileDialog1.Title = "Save File";
                    saveFileDialog1.FileName = "IMPORT_MEDICINE";
                    saveFileDialog1.DefaultExt = "xlsx";
                    saveFileDialog1.Filter = "Excel files (*.xlsx)|All files (*.*)";
                    saveFileDialog1.FilterIndex = 2;
                    saveFileDialog1.RestoreDirectory = true;

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(source, saveFileDialog1.FileName);
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
                //WaitingManager.Show();
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    WaitingManager.Show();

                    var import = new Inventec.Common.ExcelImport.Import();
                    if (import.ReadFileExcel(ofd.FileName))
                    {
                        var hisMedicineTypeImport = import.GetWithCheck<MedicineTypeImportADO>(0);
                        if (hisMedicineTypeImport != null && hisMedicineTypeImport.Count > 0)
                        {
                            List<MedicineTypeImportADO> listAfterRemove = new List<MedicineTypeImportADO>();
                            foreach (var item in hisMedicineTypeImport)
                            {
                                listAfterRemove.Add(item);
                            }

                            foreach (var item in hisMedicineTypeImport)
                            {
                                bool checkNull = string.IsNullOrEmpty(item.MEDICINE_TYPE_CODE)
                                    && string.IsNullOrEmpty(item.MEDICINE_TYPE_NAME)
                                    && string.IsNullOrEmpty(item.SERVICE_UNIT_CODE)
                                    && string.IsNullOrEmpty(item.NUM_ORDER_STR)
                                    && string.IsNullOrEmpty(item.NATIONAL_NAME)
                                    && string.IsNullOrEmpty(item.HEIN_SERVICE_TYPE_CODE)
                                    && string.IsNullOrEmpty(item.MANUFACTURER_CODE)
                                    && string.IsNullOrEmpty(item.IMP_VAT_RATIO_STR)
                                    && string.IsNullOrEmpty(item.STOP_IMP)
                                    && string.IsNullOrEmpty(item.SALE_EQUAL_IMP_PRICE)
                                    && string.IsNullOrEmpty(item.HEIN_SERVICE_BHYT_CODE)
                                    && string.IsNullOrEmpty(item.HEIN_SERVICE_BHYT_NAME)
                                    && string.IsNullOrEmpty(item.HEIN_ORDER)
                                    && string.IsNullOrEmpty(item.MEDICINE_GROUP_CODE)
                                    && string.IsNullOrEmpty(item.HEIN_LIMIT_RATIO_OLD_STR)
                                    && string.IsNullOrEmpty(item.HEIN_LIMIT_RATIO_STR)
                                    && string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_STR)
                                    && string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_OLD_STR)
                                    && string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_IN_TIME_STR)
                                    && string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_INTR_TIME_STR)
                                    && string.IsNullOrEmpty(item.INTERNAL_PRICE_STR)
                                    && string.IsNullOrEmpty(item.REQUIRE_HSD)
                                    && string.IsNullOrEmpty(item.OUT_PARENT_FEE)
                                    && string.IsNullOrEmpty(item.BUSINESS)
                                    && string.IsNullOrEmpty(item.ALLOW_EXPORT_ODD)
                                    && string.IsNullOrEmpty(item.DESCRIPTION)
                                    && string.IsNullOrEmpty(item.CONCENTRA)
                                    && string.IsNullOrEmpty(item.PARENT_CODE)
                                    && string.IsNullOrEmpty(item.STAR_MARK)
                                    && string.IsNullOrEmpty(item.FUNCTIONAL_FOOD)
                                    && string.IsNullOrEmpty(item.MEDICINE_TYPE_PROPRIETARY_NAME)
                                    && string.IsNullOrEmpty(item.TUTORIAL)
                                    && string.IsNullOrEmpty(item.ACTIVE_INGR_BHYT_CODE)
                                    && string.IsNullOrEmpty(item.ACTIVE_INGR_BHYT_NAME)
                                    && string.IsNullOrEmpty(item.MEDICINE_USE_FORM_CODE)
                                    && string.IsNullOrEmpty(item.MEDICINE_LINE_CODE)
                                    && string.IsNullOrEmpty(item.REGISTER_NUMBER)
                                    && string.IsNullOrEmpty(item.BYT_NUM_ORDER)
                                    && string.IsNullOrEmpty(item.TCY_NUM_ORDER)
                                    && string.IsNullOrEmpty(item.ALERT_MAX_IN_TREATMENT_STR)
                                    && string.IsNullOrEmpty(item.ALERT_MAX_IN_PRESCRIPTION_STR)
                                     && string.IsNullOrEmpty(item.ALERT_MAX_IN_DAY_STR)
                                    && string.IsNullOrEmpty(item.IS_BLOCK_MAX_IN_DAY_STR)
                                    && string.IsNullOrEmpty(item.COGS_STR)
                                    && string.IsNullOrEmpty(item.RECORDING_TRANSACTION)
                                    && string.IsNullOrEmpty(item.IMP_PRICE_STR)
                                    && string.IsNullOrEmpty(item.ALERT_MIN_IN_STOCK_STR)
                                    && string.IsNullOrEmpty(item.NATIONAL_CODE)
                                    && string.IsNullOrEmpty(item.ALERT_EXPIRED_DATE_STR)
                                    && string.IsNullOrEmpty(item.QUALITY_STANDARDS)
                                    && string.IsNullOrEmpty(item.PREPROCESSING)
                                    && string.IsNullOrEmpty(item.PROCESSING)
                                    && string.IsNullOrEmpty(item.USED_PART)
                                    && string.IsNullOrEmpty(item.CONTRAINDICATION)
                                    && string.IsNullOrEmpty(item.DISTRIBUTED_AMOUNT)
                                    && string.IsNullOrEmpty(item.DOSAGE_FORM)
                                    && string.IsNullOrEmpty(item.VOLUME_STR);

                                if (checkNull)
                                {
                                    listAfterRemove.Remove(item);
                                }
                            }

                            WaitingManager.Hide();

                            this.currentAdos = listAfterRemove;

                            if (this.currentAdos != null && this.currentAdos.Count > 0)
                            {
                                btnSave.Enabled = true;
                                checkClick = false;
                                btnShowLineError.Enabled = true;
                                BtnExportErrorLine.Enabled = true;
                                medicineTypeAdos = new List<MedicineTypeImportADO>();
                                addMedicineTypeToProcessList(currentAdos, ref medicineTypeAdos);
                                bool exist = medicineTypeAdos.FirstOrDefault(o => o.IS_LESS_MANUFACTURER) != null;
                                if (medicineTypeAdos != null && medicineTypeAdos.Count > 0 && exist)
                                {
                                    var less = medicineTypeAdos.Where(o => o.IS_LESS_MANUFACTURER).ToList();
                                    frmWarning frm = new frmWarning(less, (DelegateRefreshData)DelegateWarning);
                                    frm.ShowDialog();
                                }

                                if (addSuccess)
                                {
                                    addMedicineTypeToProcessList(currentAdos, ref medicineTypeAdos);
                                }

                                SetDataSource(medicineTypeAdos);
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
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                WaitingManager.Show();

                List<HIS_MEDICINE_TYPE> listMedi = new List<HIS_MEDICINE_TYPE>();

                foreach (var item in medicineTypeAdos)
                {
                    HIS_MEDICINE_TYPE medi = new HIS_MEDICINE_TYPE();
                    HIS_SERVICE ser = new HIS_SERVICE();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_MEDICINE_TYPE>(medi, item);
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE>(ser, item);
                    ser.SERVICE_UNIT_ID = item.SERVICE_UNIT_ID;
                    ser.ID = 0;
                    ser.PARENT_ID = null;
                    medi.HIS_SERVICE = ser;
                    medi.ID = 0;
                    var valueList = listMediAcinCode.FirstOrDefault(x => x.Key == item.MEDICINE_TYPE_CODE).Value;
                    if (valueList != null && valueList.Count > 0)
                    {
                        foreach (var _i in valueList)
                        {
                            medi.HIS_MEDICINE_TYPE_ACIN.Add(_i);
                        }
                        
                    }
                    listMedi.Add(medi);
                }
                
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("listMedi", listMedi));
                CommonParam param = new CommonParam();

                if (listMedi != null && listMedi.Count > 0)
                {
                    var rs = new BackendAdapter(param).Post<List<HIS_MEDICINE_TYPE>>("api/HisMedicineType/CreateList", ApiConsumers.MosConsumer, listMedi, param);
                    if (rs != null)
                    {
                        success = true;
                        btnSave.Enabled = false;
                        BackendDataWorker.Reset<V_HIS_MEDICINE_TYPE>();
                        BackendDataWorker.Reset<V_HIS_SERVICE>();
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
                var row = (MedicineTypeImportADO)gridViewMedicineType.GetFocusedRow();
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
                var row = (MedicineTypeImportADO)gridViewMedicineType.GetFocusedRow();
                if (row != null)
                {
                    if (medicineTypeAdos != null && medicineTypeAdos.Count > 0)
                    {
                        medicineTypeAdos.Remove(row);
                        gridControlMedicineType.DataSource = null;
                        gridControlMedicineType.DataSource = medicineTypeAdos;
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
                    var errorLine = medicineTypeAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = medicineTypeAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMedicineType_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    MedicineTypeImportADO data = (MedicineTypeImportADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
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

        private void gridViewMedicineType_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                MedicineTypeImportADO pData = (MedicineTypeImportADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {

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
                    else if (e.Column.FieldName == "SoNgayDung")
                    {
                        try
                        {
                            e.Value = pData.IS_TREATMENT_DAY_COUNT == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua IS_TREATMENT_DAY_COUNT_STR", ex);
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
                    else if (e.Column.FieldName == "FUNCTIONAL_FOOD_STR")
                    {
                        try
                        {
                            e.Value = pData.IS_FUNCTIONAL_FOOD == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua FUNCTIONAL_FOOD_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "STAR_MARK_STR")
                    {
                        try
                        {
                            e.Value = pData.IS_STAR_MARK == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua STAR_MARK_STR", ex);
                        }
                    }
                    //else if (e.Column.FieldName == "ACTIVE_ITEM")
                    //{
                    //    try
                    //    {
                    //        if (status == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    //            e.Value = Inventec.Common.Resource.Get.Value("frmHisMisuMedicineTypeType.HoatDong", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    //        else
                    //            e.Value = Inventec.Common.Resource.Get.Value("frmHisMisuMedicineTypeType.TamKhoa", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                    else if (e.Column.FieldName == "is_block_max_in_day")
                    {
                        try
                        {
                            e.Value = pData.IS_BLOCK_MAX_IN_DAY == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot chan khi ke qua so luong/ngay IS_BLOCK_MAX_IN_DAY", ex);
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
                    else if (e.Column.FieldName == "VOLUME_STR")
                    {
                        try
                        {
                            e.Value = pData.VOLUME_STR;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot dung tich truyen", ex);
                        }
                    }
                    if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                    {
                        if (e.Column.FieldName == "SOURCE_MEDICINE_STR")
                        {
                            try
                            {
                                if (pData.SOURCE_MEDICINE == 1)
                                    e.Value = "Thuốc bắc";
                                else if (pData.SOURCE_MEDICINE == 2)
                                    e.Value = "Thuốc nam";

                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }
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
                    medicineTypeAdos = new List<MedicineTypeImportADO>();
                    addMedicineTypeToProcessList(currentAdos, ref medicineTypeAdos);
                    bool exist = (medicineTypeAdos.FirstOrDefault(o => o.IS_LESS_MANUFACTURER) != null);
                    if (medicineTypeAdos != null && medicineTypeAdos.Count > 0 && exist)
                    {
                        var less = medicineTypeAdos.Where(o => o.IS_LESS_MANUFACTURER).ToList();
                        frmWarning frm = new frmWarning(less, (DelegateRefreshData)DelegateWarning);
                        frm.ShowDialog();
                    }
                    SetDataSource(medicineTypeAdos);
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

                if (!this.medicineTypeAdos.Exists(o => !String.IsNullOrWhiteSpace(o.ERROR)))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy dòng lỗi");
                    return;
                }

                Inventec.Common.FlexCellExport.Store store = new Inventec.Common.FlexCellExport.Store(true);

                string templateFile = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Exp", "DanhSachLoiImportLoaiThuoc.xlsx");

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

                    var errorList = this.medicineTypeAdos.Where(o => !String.IsNullOrWhiteSpace(o.ERROR)).ToList();
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

        private void ProcessData(List<MedicineTypeImportADO> errorList, ref Inventec.Common.FlexCellExport.Store store)
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

        #region private_function
        private void CheckErrorLine()
        {
            try
            {
                if (medicineTypeAdos != null && medicineTypeAdos.Count > 0)
                {
                    var checkError = medicineTypeAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
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

        private void SetDataSource(List<MedicineTypeImportADO> dataSource)
        {
            try
            {
                gridControlMedicineType.BeginUpdate();
                gridControlMedicineType.DataSource = null;
                gridControlMedicineType.DataSource = dataSource;
                gridControlMedicineType.EndUpdate();
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

                //string[] substring = date.Split('/');
                //if (substring != null)
                //{
                //    if (substring.Count() != 3)
                //    {
                //        check = false;
                //        return;
                //    }
                //    if (Inventec.Common.TypeConvert.Parse.ToInt64(substring[0]) < 0 || Inventec.Common.TypeConvert.Parse.ToInt64(substring[0]) > 31)
                //    {
                //        check = false;
                //        return;
                //    }
                //    if (Inventec.Common.TypeConvert.Parse.ToInt64(substring[1]) < 0 || Inventec.Common.TypeConvert.Parse.ToInt64(substring[1]) > 12)
                //    {
                //        check = false;
                //        return;
                //    }
                //    if (Inventec.Common.TypeConvert.Parse.ToInt64(substring[2]) < 0 || Inventec.Common.TypeConvert.Parse.ToInt64(substring[2]) > 9999)
                //    {
                //        check = false;
                //        return;
                //    }
                //}
                //string dateString = substring[2] + substring[1] + substring[0] + "000000";
                //dateTime = Inventec.Common.TypeConvert.Parse.ToInt64(dateString);

                ////date.Replace(" ", "");
                ////int idx = date.LastIndexOf("/");
                ////string year = date.Substring(idx + 1);
                ////string monthdate = date.Substring(0, idx);
                ////idx = monthdate.LastIndexOf("/");
                ////monthdate.Remove(idx);
                ////idx = monthdate.LastIndexOf("/");
                ////string month = monthdate.Substring(idx + 1);
                ////string dateStr = monthdate.Substring(0, idx);
                ////if (month.Length < 2)
                ////{
                ////    month = "0" + month;
                ////}
                ////if (dateStr.Length < 2)
                ////{
                ////    dateStr = "0" + dateStr;
                ////}
                ////datetime = year + month + dateStr;
                ////datetime.Replace("/", "");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void addMedicineTypeToProcessList(List<MedicineTypeImportADO> _medicine, ref List<MedicineTypeImportADO> _medicineRef)
        {
            try
            {
                _medicineRef = new List<MedicineTypeImportADO>();
                long i = 0;
                foreach (var item in _medicine)
                {
                    i++;
                    string error = "";
                    var mediAdo = new MedicineTypeImportADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MedicineTypeImportADO>(mediAdo, item);
                    if (!string.IsNullOrEmpty(item.PARENT_CODE))
                    {
                        if (item.PARENT_CODE.Length > 25)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã cha");
                            mediAdo.PARENT_CODE_ERROR = 1;
                        }

                        var getData = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.MEDICINE_TYPE_CODE == item.PARENT_CODE);
                        if (getData != null)
                        {
                            mediAdo.PARENT_ID = getData.ID;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã cha");
                            mediAdo.PARENT_CODE_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.STOP_IMP))
                    {
                        if (item.STOP_IMP.Trim().ToLower() == "x")
                        {
                            mediAdo.IS_STOP_IMP = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Dừng nhập");
                            mediAdo.STOP_IMP_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.ALLOW_ODD))
                    {
                        if (item.ALLOW_ODD.Trim().ToLower() == "x")
                        {
                            mediAdo.IS_ALLOW_ODD = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Cho phép kê lẻ");
                            mediAdo.ALLOW_ODD_ERROR = 1;
                        }
                    }
                    if (string.IsNullOrEmpty(item.DOSAGE_FORM))
                    {
                        error += string.Format(Message.MessageImport.TruongDuBatBuoc, "dạng bào chế");
                        mediAdo.DOSAGE_FORM_ERROR = 1;
                    }
                    if (!string.IsNullOrEmpty(item.REQUIRE_HSD))
                    {
                        if (item.REQUIRE_HSD.Trim().ToLower() == "x")
                        {
                            mediAdo.IS_REQUIRE_HSD = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Bắt buộc nhập HSD");
                            mediAdo.REQUIRE_HSD_ERROR = 1;
                        }
                    }
                    
                    if (!string.IsNullOrEmpty(item.TDL_GENDER_CODE))
                    {
                        var gender = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.GENDER_CODE == item.TDL_GENDER_CODE);
                        if (gender != null)
                        {
                            mediAdo.TDL_GENDER_ID = gender.ID;
                            mediAdo.TDL_GENDER_NAME = gender.GENDER_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Giới tính");
                            mediAdo.TDL_GENDER_CODE_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.BUSINESS))
                    {
                        if (item.BUSINESS.Trim().ToLower() == "x")
                        {
                            mediAdo.IS_BUSINESS = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Thuốc kinh doanh");
                            mediAdo.BUSINESS_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.IS_TREATMENT_DAY_COUNT_STR))
                    {
                        if (item.IS_TREATMENT_DAY_COUNT_STR.Trim().ToLower() == "x")
                        {
                            mediAdo.IS_TREATMENT_DAY_COUNT = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Số ngày dùng trong tờ điều trị");
                            mediAdo.IS_TREATMENT_DAY_COUNT_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.ALLOW_EXPORT_ODD))
                    {
                        if (item.ALLOW_EXPORT_ODD.Trim().ToLower() == "x")
                        {
                            mediAdo.IS_ALLOW_EXPORT_ODD = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Cho phép xuất lẻ");
                            mediAdo.ALLOW_EXPORT_ODD_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.OUT_PARENT_FEE))
                    {
                        if (item.OUT_PARENT_FEE.Trim().ToLower() == "x")
                        {
                            mediAdo.IS_OUT_PARENT_FEE = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "CP ngoài gói");
                            mediAdo.OUT_PARENT_FEE_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.STAR_MARK))
                    {
                        if (item.STAR_MARK.Trim().ToLower() == "x")
                        {
                            mediAdo.IS_STAR_MARK = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Thuốc dấu * theo công văn 556");
                            mediAdo.STAR_MARK_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.FUNCTIONAL_FOOD))
                    {
                        if (item.FUNCTIONAL_FOOD.Trim().ToLower() == "x")
                        {
                            mediAdo.IS_FUNCTIONAL_FOOD = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Là thực phẩm chức năng");
                            mediAdo.FUNCTIONAL_FOOD_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.SALE_EQUAL_IMP_PRICE))
                    {
                        if (item.SALE_EQUAL_IMP_PRICE.Trim().ToLower() == "x")
                        {
                            mediAdo.IS_SALE_EQUAL_IMP_PRICE = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Bán bằng giá nhập");
                            mediAdo.SALE_EQUAL_IMP_PRICE_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_SERVICE_TYPE_CODE))
                    {
                        if (item.HEIN_SERVICE_TYPE_CODE.Length > 10)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã loại dịch vụ BH");
                            mediAdo.HEIN_SERVICE_TYPE_CODE_ERROR = 1;
                        }

                        var getData = BackendDataWorker.Get<HIS_HEIN_SERVICE_TYPE>().FirstOrDefault(o => o.HEIN_SERVICE_TYPE_CODE == item.HEIN_SERVICE_TYPE_CODE);
                        if (getData != null)
                        {
                            mediAdo.HEIN_SERVICE_TYPE_ID = getData.ID;
                            mediAdo.HEIN_SERVICE_TYPE_NAME = getData.HEIN_SERVICE_TYPE_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã loại dịch vụ BH");
                            mediAdo.HEIN_SERVICE_TYPE_CODE_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.SERVICE_UNIT_CODE))
                    {
                        if (item.SERVICE_UNIT_CODE.Length > 3)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Đơn vị tính");
                            mediAdo.SERVICE_UNIT_CODE_ERROR = 1;
                        }

                        var getData = BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(o => o.SERVICE_UNIT_CODE == item.SERVICE_UNIT_CODE);
                        if (getData != null)
                        {
                            mediAdo.SERVICE_UNIT_ID = getData.ID;
                            mediAdo.SERVICE_UNIT_NAME = getData.SERVICE_UNIT_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Đơn vị tính");
                            mediAdo.SERVICE_UNIT_CODE_ERROR = 1;
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Đơn vị tính");
                        mediAdo.SERVICE_UNIT_CODE_ERROR = 1;
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_IN_TIME_STR))
                    {
                        long? dateTime = null;
                        string check = "";
                        convertDateStringToTimeNumber(item.HEIN_LIMIT_PRICE_IN_TIME_STR, ref dateTime, ref check);
                        if (dateTime != null && string.IsNullOrEmpty(check))
                        {
                            mediAdo.HEIN_LIMIT_PRICE_IN_TIME = dateTime;
                        }
                        else
                        {
                            error += string.Format(check, "Áp dụng theo ngày vào viện");
                            mediAdo.HEIN_LIMIT_PRICE_IN_TIME_STR_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_INTR_TIME_STR))
                    {
                        long? dateTime = null;
                        string check = "";
                        convertDateStringToTimeNumber(item.HEIN_LIMIT_PRICE_INTR_TIME_STR, ref dateTime, ref check);
                        if (dateTime != null && string.IsNullOrEmpty(check))
                        {
                            mediAdo.HEIN_LIMIT_PRICE_INTR_TIME = dateTime;
                        }
                        else
                        {
                            error += string.Format(check, "Áp dụng theo TG chỉ định");
                            mediAdo.HEIN_LIMIT_PRICE_INTR_TIME_STR_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.NUM_ORDER_STR))
                    {
                        if (Inventec.Common.Number.Check.IsNumber(item.NUM_ORDER_STR))
                        {
                            mediAdo.NUM_ORDER = Inventec.Common.TypeConvert.Parse.ToInt64(item.NUM_ORDER_STR);
                            if (mediAdo.NUM_ORDER.ToString().Length > 19 || mediAdo.NUM_ORDER < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "STT hiện");
                                mediAdo.NUM_ORDER_STR_ERROR = 1;
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "STT hiện");
                            mediAdo.NUM_ORDER_STR_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_SERVICE_BHYT_CODE))
                    {
                        if (item.HEIN_SERVICE_BHYT_CODE.Length > 100)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã BHYT");
                            mediAdo.HEIN_SERVICE_BHYT_CODE_ERROR = 1;
                        }

                        if (string.IsNullOrEmpty(item.HEIN_SERVICE_BHYT_NAME))
                        {
                            error += string.Format(Message.MessageImport.CoThiPhaiNhap, "Mã BHYT", "Tên BHYT");
                            mediAdo.HEIN_SERVICE_BHYT_CODE_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_SERVICE_BHYT_NAME))
                    {
                        if (item.HEIN_SERVICE_BHYT_NAME.Length > 1500)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Tên BHYT");
                            mediAdo.HEIN_SERVICE_BHYT_NAME_ERROR = 1;
                        }

                        if (string.IsNullOrEmpty(item.HEIN_SERVICE_BHYT_CODE))
                        {
                            error += string.Format(Message.MessageImport.CoThiPhaiNhap, "Tên BHYT", "Mã BHYT");
                            mediAdo.HEIN_SERVICE_BHYT_NAME_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_ORDER))
                    {
                        if (item.HEIN_ORDER.Length > 20)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "STT BHYT");
                            mediAdo.HEIN_ORDER_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_LIMIT_RATIO_STR))
                    {
                        if (Inventec.Common.Number.Check.IsDecimal(item.HEIN_LIMIT_RATIO_STR))
                        {
                            mediAdo.HEIN_LIMIT_RATIO = Inventec.Common.TypeConvert.Parse.ToDecimal(item.HEIN_LIMIT_RATIO_STR);
                            if (mediAdo.HEIN_LIMIT_RATIO.Value > 1 || mediAdo.HEIN_LIMIT_RATIO < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Tỉ lệ trần mới");
                                mediAdo.HEIN_LIMIT_RATIO_STR_ERROR = 1;
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Tỉ lệ trần mới");
                            mediAdo.HEIN_LIMIT_RATIO_STR_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_LIMIT_RATIO_OLD_STR))
                    {
                        if (Inventec.Common.Number.Check.IsDecimal(item.HEIN_LIMIT_RATIO_OLD_STR))
                        {
                            mediAdo.HEIN_LIMIT_RATIO_OLD = Inventec.Common.TypeConvert.Parse.ToDecimal(item.HEIN_LIMIT_RATIO_OLD_STR);
                            if (mediAdo.HEIN_LIMIT_RATIO_OLD.Value > 1 || mediAdo.HEIN_LIMIT_RATIO < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Tỉ lệ trần cũ");
                                mediAdo.HEIN_LIMIT_RATIO_OLD_STR_ERROR = 1;
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Tỉ lệ trần cũ");
                            mediAdo.HEIN_LIMIT_RATIO_OLD_STR_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_OLD_STR))
                    {
                        if (Inventec.Common.Number.Check.IsDecimal(item.HEIN_LIMIT_PRICE_OLD_STR))
                        {
                            mediAdo.HEIN_LIMIT_PRICE_OLD = Inventec.Common.TypeConvert.Parse.ToDecimal(item.HEIN_LIMIT_PRICE_OLD_STR);
                            if (mediAdo.HEIN_LIMIT_PRICE_OLD.Value > 99999999999999 || mediAdo.HEIN_LIMIT_PRICE_OLD < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Giá trần cũ");
                                mediAdo.HEIN_LIMIT_PRICE_OLD_STR_ERROR = 1;
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Giá trần cũ");
                            mediAdo.HEIN_LIMIT_PRICE_OLD_STR_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.IMP_VAT_RATIO_STR))
                    {
                        if (Inventec.Common.Number.Check.IsDecimal(item.IMP_VAT_RATIO_STR))
                        {
                            mediAdo.IMP_VAT_RATIO = Inventec.Common.TypeConvert.Parse.ToDecimal(item.IMP_VAT_RATIO_STR);
                            if (mediAdo.IMP_VAT_RATIO.Value > 1 || mediAdo.IMP_VAT_RATIO < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "VAT");
                                mediAdo.IMP_VAT_RATIO_STR_ERROR = 1;
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "VAT");
                            mediAdo.IMP_VAT_RATIO_STR_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.INTERNAL_PRICE_STR))
                    {
                        if (Inventec.Common.Number.Check.IsDecimal(item.INTERNAL_PRICE_STR))
                        {
                            mediAdo.INTERNAL_PRICE = Inventec.Common.TypeConvert.Parse.ToDecimal(item.INTERNAL_PRICE_STR);
                            if (mediAdo.INTERNAL_PRICE.Value > 99999999999999 || mediAdo.INTERNAL_PRICE < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Giá nội bộ");
                                mediAdo.INTERNAL_PRICE_STR_ERROR = 1;
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Giá nội bộ");
                            mediAdo.INTERNAL_PRICE_STR_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.IMP_PRICE_STR))
                    {
                        if (Inventec.Common.Number.Check.IsDecimal(item.IMP_PRICE_STR))
                        {
                            mediAdo.IMP_PRICE = Inventec.Common.TypeConvert.Parse.ToDecimal(item.IMP_PRICE_STR);
                            if (mediAdo.IMP_PRICE.Value > 99999999999999 || mediAdo.IMP_PRICE < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Giá nhập");
                                mediAdo.IMP_PRICE_STR_ERROR = 1;
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Giá nhập");
                            mediAdo.IMP_PRICE_STR_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.VOLUME_STR))
                    {
                        if (Inventec.Common.Number.Check.IsDecimal(item.VOLUME_STR))
                        {
                            mediAdo.VOLUME = Inventec.Common.TypeConvert.Parse.ToDecimal(item.VOLUME_STR);
                            if (mediAdo.VOLUME.Value > 99999999999999 || mediAdo.VOLUME < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Dung tich truyen");
                                mediAdo.VOLUME_STR_ERROR = 1;
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Dung tich truyen");
                            mediAdo.VOLUME_STR_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.ALERT_EXPIRED_DATE_STR))
                    {
                        if (Inventec.Common.Number.Check.IsNumber(item.ALERT_EXPIRED_DATE_STR))
                        {
                            mediAdo.ALERT_EXPIRED_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(item.ALERT_EXPIRED_DATE_STR);

                            if ((mediAdo.ALERT_EXPIRED_DATE ?? 0) > 999999999999999999 || (mediAdo.ALERT_EXPIRED_DATE ?? 0) < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Cảnh báo HSD");
                                mediAdo.ALERT_EXPIRED_DATE_STR_ERROR = 1;
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Cảnh báo HSD");
                            mediAdo.ALERT_EXPIRED_DATE_STR_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.ALERT_MIN_IN_STOCK_STR))
                    {
                        if (Inventec.Common.Number.Check.IsDecimal(item.ALERT_MIN_IN_STOCK_STR))
                        {
                            mediAdo.ALERT_MIN_IN_STOCK = Inventec.Common.TypeConvert.Parse.ToDecimal(item.ALERT_MIN_IN_STOCK_STR);

                            if ((mediAdo.ALERT_MIN_IN_STOCK ?? 0) > 9999999999999999 || (mediAdo.ALERT_MIN_IN_STOCK ?? 0) < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Cảnh báo tồn kho");
                                mediAdo.ALERT_MIN_IN_STOCK_STR_ERROR = 1;
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Cảnh báo tồn kho");
                            mediAdo.ALERT_MIN_IN_STOCK_STR_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_STR))
                    {
                        if (Inventec.Common.Number.Check.IsDecimal(item.HEIN_LIMIT_PRICE_STR))
                        {
                            mediAdo.HEIN_LIMIT_PRICE = Inventec.Common.TypeConvert.Parse.ToDecimal(item.HEIN_LIMIT_PRICE_STR);
                            if (mediAdo.HEIN_LIMIT_PRICE.Value > 99999999999999 || mediAdo.HEIN_LIMIT_PRICE < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Giá trần mới");
                                mediAdo.HEIN_LIMIT_PRICE_STR_ERROR = 1;
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Giá trần mới");
                            mediAdo.HEIN_LIMIT_PRICE_STR_ERROR = 1;
                        }
                    }

                    if ((!string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_STR) || !string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_OLD_STR)) && (!string.IsNullOrEmpty(item.HEIN_LIMIT_RATIO_STR) || !string.IsNullOrEmpty(item.HEIN_LIMIT_RATIO_OLD_STR)))
                    {
                        error += string.Format(Message.MessageImport.ChiDuocNhapGiaHoacTiLeTran);
                        mediAdo.HEIN_LIMIT_PRICE_STR_ERROR = 1;
                        mediAdo.HEIN_LIMIT_PRICE_OLD_STR_ERROR = 1;
                        mediAdo.HEIN_LIMIT_RATIO_STR_ERROR = 1;
                        mediAdo.HEIN_LIMIT_RATIO_OLD_STR_ERROR = 1;
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_IN_TIME_STR) && !string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_INTR_TIME_STR))
                    {
                        error += string.Format(Message.MessageImport.ChiDuocNhapTGVaoVienHoacTGChiDinh);
                        mediAdo.HEIN_LIMIT_PRICE_IN_TIME_STR_ERROR = 1;
                        mediAdo.HEIN_LIMIT_PRICE_INTR_TIME_STR_ERROR = 1;
                    }

                    if (!string.IsNullOrEmpty(item.MEDICINE_TYPE_CODE))
                    {
                        if (!CheckMaxLenth(item.MEDICINE_TYPE_CODE, 25))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã loại thuốc");
                            mediAdo.MEDICINE_TYPE_CODE_ERROR = 1;
                        }

                        var check = listMedicin.Exists(o => o.MEDICINE_TYPE_CODE == item.MEDICINE_TYPE_CODE);
                        var _Data = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.MEDICINE_TYPE_CODE == item.MEDICINE_TYPE_CODE);
                        Inventec.Common.Logging.LogSystem.Debug("_______Kiểm tra mã loại thuốc " + check);
                        Inventec.Common.Logging.LogSystem.Debug("_______Kiểm tra mã loại thuốc data:" + Inventec.Common.Logging.LogUtil.TraceData("____HIS_MEDICINE_TYPE: ",_Data));
                        if (check)
                        {
                            Inventec.Common.Logging.LogSystem.Debug("_______Kiểm tra mã loại thuốc: mã loại thuốc đã tồn tại ");
                            error += string.Format(Message.MessageImport.DaTonTai, "Mã loại thuốc");
                            mediAdo.MEDICINE_TYPE_CODE_ERROR = 1;
                        }

                        var checkExel = _medicineRef.FirstOrDefault(o => o.MEDICINE_TYPE_CODE == item.MEDICINE_TYPE_CODE);
                        
                        if (checkExel != null)
                        {

                            error += string.Format(Message.MessageImport.TonTaiTrungNhauTrongFileImport, "Mã loại thuốc");
                            mediAdo.MEDICINE_TYPE_CODE_ERROR = 1;
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã loại thuốc");
                        mediAdo.MEDICINE_TYPE_CODE_ERROR = 1;
                    }

                    if (!string.IsNullOrEmpty(item.MEDICINE_TYPE_NAME))
                    {
                        if (!CheckMaxLenth(item.MEDICINE_TYPE_NAME, 1500))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Tên loại thuốc");
                            mediAdo.MEDICINE_TYPE_NAME_ERROR = 1;
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Tên loại thuốc");
                        mediAdo.MEDICINE_TYPE_NAME_ERROR = 1;
                    }

                    if (!string.IsNullOrEmpty(item.PACKING_TYPE_NAME))
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.PACKING_TYPE_NAME, 300))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Loại đóng gói");
                            mediAdo.PACKING_TYPE_NAME_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.NATIONAL_CODE))
                    {
                        var national = BackendDataWorker.Get<SDA_NATIONAL>().FirstOrDefault(o => o.NATIONAL_CODE == item.NATIONAL_CODE);
                        if (national != null)
                        {
                            mediAdo.NATIONAL_NAME = national.NATIONAL_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã quốc gia");
                            mediAdo.NATIONAL_CODE_ERROR = 1;
                        }
                    }
                    else if (!string.IsNullOrEmpty(item.NATIONAL_NAME))
                    {
                        if (!CheckMaxLenth(item.NATIONAL_NAME, 100))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Tên quốc gia");
                            mediAdo.NATIONAL_NAME_ERROR = 1;
                        }
                        else
                        {
                            mediAdo.NATIONAL_NAME = item.NATIONAL_NAME;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.MANUFACTURER_CODE))
                    {
                        if (!CheckMaxLenth(item.MANUFACTURER_CODE, 6))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã hãng sản xuất");
                            mediAdo.MANUFACTURER_CODE_ERROR = 1;
                        }

                        var package = BackendDataWorker.Get<HIS_MANUFACTURER>().FirstOrDefault(o => o.MANUFACTURER_CODE == item.MANUFACTURER_CODE);
                        if (package != null)
                        {
                            mediAdo.MANUFACTURER_ID = package.ID;
                            mediAdo.MANUFACTURER_NAME = package.MANUFACTURER_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã hãng sản xuất");
                            mediAdo.IS_LESS_MANUFACTURER = true;
                            mediAdo.MANUFACTURER_CODE_ERROR = 1;
                        }
                    }
                    else if (!string.IsNullOrEmpty(item.MANUFACTURER_NAME))
                    {
                        if (!CheckMaxLenth(item.MANUFACTURER_NAME, 100))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Tên hãng sản xuất");
                            mediAdo.MANUFACTURER_NAME_ERROR = 1;
                        }

                        var manufacturer = BackendDataWorker.Get<HIS_MANUFACTURER>().FirstOrDefault(o => o.MANUFACTURER_NAME == item.MANUFACTURER_NAME);
                        if (manufacturer != null)
                        {
                            mediAdo.MANUFACTURER_ID = manufacturer.ID;
                            mediAdo.MANUFACTURER_CODE = manufacturer.MANUFACTURER_CODE;
                            mediAdo.MANUFACTURER_NAME = manufacturer.MANUFACTURER_NAME;
                            item.MANUFACTURER_NAME = manufacturer.MANUFACTURER_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Tên hãng sản xuất");
                            mediAdo.IS_LESS_MANUFACTURER = true;
                            mediAdo.MANUFACTURER_NAME_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.MEDICINE_GROUP_CODE))
                    {
                        if (!CheckMaxLenth(item.MEDICINE_GROUP_CODE, 4))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Nhóm thuốc");
                            mediAdo.MEDICINE_GROUP_CODE_ERROR = 1;
                        }

                        var group = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().FirstOrDefault(o => o.MEDICINE_GROUP_CODE == item.MEDICINE_GROUP_CODE);
                        if (group != null)
                        {
                            mediAdo.MEDICINE_GROUP_ID = group.ID;
                            mediAdo.MEDICINE_GROUP_NAME = group.MEDICINE_GROUP_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Nhóm thuốc");
                            mediAdo.MEDICINE_GROUP_CODE_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.DESCRIPTION))
                    {
                        if (!CheckMaxLenth(item.DESCRIPTION, 2000))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mô tả");
                            mediAdo.DESCRIPTION_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.CONCENTRA))
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.CONCENTRA, 500))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Hàm lượng nồng độ");
                            mediAdo.CONCENTRA_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.MEDICINE_TYPE_PROPRIETARY_NAME))
                    {
                        if (!CheckMaxLenth(item.MEDICINE_TYPE_PROPRIETARY_NAME, 200))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Tên biệt dược");
                            mediAdo.MEDICINE_TYPE_PROPRIETARY_NAME_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.TUTORIAL))
                    {
                        if (!CheckMaxLenth(item.TUTORIAL, 2000))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "HDSD thuốc");
                            mediAdo.TUTORIAL_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.ACTIVE_INGR_BHYT_CODE))
                    {
                        if (!CheckMaxLenth(item.ACTIVE_INGR_BHYT_CODE, 500))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã hoạt chất");
                            mediAdo.ACTIVE_INGR_BHYT_CODE_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.ACTIVE_INGR_BHYT_NAME))
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.ACTIVE_INGR_BHYT_NAME, 1000))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Tên hoạt chất");
                            mediAdo.ACTIVE_INGR_BHYT_NAME_ERROR = 1;
                        }
                    }

                    if (item.SOURCE_MEDICINE != null)
                    {
                        if (item.SOURCE_MEDICINE.ToString() != "1" && item.SOURCE_MEDICINE.ToString() != "2")
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Nguồn gốc");
                            mediAdo.SOURCE_MEDICINE_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.REGISTER_NUMBER))
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.REGISTER_NUMBER, 500))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Số đăng ký");
                            mediAdo.REGISTER_NUMBER_ERROR = 1;
                        }
                    }

                    if (item.QUALITY_STANDARDS != null)
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.QUALITY_STANDARDS, 1000))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Tiêu chuẩn chất lượng");
                            mediAdo.QUALITY_STANDARDS_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.PREPROCESSING))
                    {
                        List<string> attachPreprocessingCodes = new List<string>();
                        List<string> attachPreprocessingErrors = new List<string>();
                        List<string> attachPreprocessingCodeAvaiables = new List<string>();
                        List<string> attachPreprocessingNameAvaiables = new List<string>();
                        if (item.PREPROCESSING.Contains(","))
                        {
                            attachPreprocessingCodes = item.PREPROCESSING.Split(',').ToList();
                        }
                        else
                        {
                            attachPreprocessingCodes = new List<string> { item.PREPROCESSING };
                        }
                        if (attachPreprocessingCodes.Count() > 0)
                        {
                            var _items = BackendDataWorker.Get<HIS_PROCESSING_METHOD>().Where(o => o.PROCESSING_METHOD_TYPE == 1).ToList();
                            foreach (var code in attachPreprocessingCodes)
                            {
                                var _item = _items.FirstOrDefault(o => o.PROCESSING_METHOD_CODE == code);
                                if (_item != null)
                                {
                                    attachPreprocessingCodeAvaiables.Add(code);
                                    attachPreprocessingNameAvaiables.Add(_item.PROCESSING_METHOD_NAME);
                                }
                                else
                                {
                                    attachPreprocessingErrors.Add(code);
                                }
                            }
                        }
                        if (attachPreprocessingCodeAvaiables.Count() > 0)
                        {
                            mediAdo.PREPROCESSING_NAMES = string.Join(";", attachPreprocessingNameAvaiables);
                        }
                        if (attachPreprocessingErrors.Count() > 0)
                        {
                            error += string.Format("Mã {0} không có trong danh mục Phương pháp chế biến", string.Join(";", attachPreprocessingErrors));
                            mediAdo.PREPROCESSING_ERROR = 1;
                        }

                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.PREPROCESSING, 1000))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Sơ chế");
                            mediAdo.PREPROCESSING_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.PROCESSING))
                    {
                        List<string> attachProcessingCodes = new List<string>();
                        List<string> attachProcessingErrors = new List<string>();
                        List<string> attachProcessingCodeAvaiables = new List<string>();
                        List<string> attachProcessingNameAvaiables = new List<string>();
                        if (item.PROCESSING.Contains(","))
                        {
                            attachProcessingCodes = item.PROCESSING.Split(',').ToList();
                        }
                        else
                        {
                            attachProcessingCodes = new List<string> { item.PROCESSING };
                        }
                        if (attachProcessingCodes.Count() > 0)
                        {
                            var _items = BackendDataWorker.Get<HIS_PROCESSING_METHOD>().Where(o => o.PROCESSING_METHOD_TYPE == 2).ToList();
                            foreach (var code in attachProcessingCodes)
                            {
                                var _item = _items.FirstOrDefault(o => o.PROCESSING_METHOD_CODE == code);
                                if (_item != null)
                                {
                                    attachProcessingCodeAvaiables.Add(code);
                                    attachProcessingNameAvaiables.Add(_item.PROCESSING_METHOD_NAME);
                                }
                                else
                                {
                                    attachProcessingErrors.Add(code);
                                }
                            }
                        }
                        if (attachProcessingCodeAvaiables.Count() > 0)
                        {
                            mediAdo.PREPROCESSING_NAMES = string.Join(";", attachProcessingNameAvaiables);
                        }
                        if (attachProcessingErrors.Count() > 0)
                        {
                            error += string.Format("Mã {0} không có trong danh mục Phương pháp chế biến", string.Join(";", attachProcessingErrors));
                            mediAdo.PROCESSING_ERROR = 1;
                        }

                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.PREPROCESSING, 1000))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Sơ chế");
                            mediAdo.PREPROCESSING_ERROR = 1;
                        }
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.PROCESSING, 1000))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Phức chế");
                            mediAdo.PROCESSING_ERROR = 1;
                        }
                    }
                    if (!string.IsNullOrEmpty(item.USED_PART))
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.USED_PART, 500))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Bộ phận dùng");
                            mediAdo.USED_PART_ERROR = 1;
                        }
                    }
                    if (!string.IsNullOrEmpty(item.CONTRAINDICATION))
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.CONTRAINDICATION, 4000))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Ghi chú chống chỉ định");
                            mediAdo.CONTRAINDICATION_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.DISTRIBUTED_AMOUNT))
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.DISTRIBUTED_AMOUNT, 500))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Số lượng phân bổ");
                            mediAdo.DISTRIBUTED_AMOUNT_ERROR = 1;
                        }
                    }
                    if (!string.IsNullOrEmpty(item.DOSAGE_FORM))
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.DOSAGE_FORM, 100))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Dạng bào chế");
                            mediAdo.DOSAGE_FORM_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.TCY_NUM_ORDER))
                    {
                        if (!CheckMaxLenth(item.TCY_NUM_ORDER, 20))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "STT theo TT31-12");
                            mediAdo.TCY_NUM_ORDER_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.BYT_NUM_ORDER))
                    {
                        if (!CheckMaxLenth(item.BYT_NUM_ORDER, 50))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "STT theo TT40 (Bộ y tế)");
                            mediAdo.BYT_NUM_ORDER_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.MEDICINE_USE_FORM_CODE))
                    {
                        if (!CheckMaxLenth(item.MEDICINE_USE_FORM_CODE, 6))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Dạng dùng");
                            mediAdo.MEDICINE_USE_FORM_CODE_ERROR = 1;
                        }

                        var package = BackendDataWorker.Get<HIS_MEDICINE_USE_FORM>().FirstOrDefault(o => o.MEDICINE_USE_FORM_CODE == item.MEDICINE_USE_FORM_CODE);
                        if (package != null)
                        {
                            mediAdo.MEDICINE_USE_FORM_ID = package.ID;
                            mediAdo.MEDICINE_USE_FORM_NAME = package.MEDICINE_USE_FORM_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Dạng dùng");
                            mediAdo.MEDICINE_USE_FORM_CODE_ERROR = 1;
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Dạng dùng");
                        mediAdo.MEDICINE_USE_FORM_CODE_ERROR = 1;
                    }

                    if (!string.IsNullOrEmpty(item.MEDICINE_LINE_CODE))
                    {
                        if (!CheckMaxLenth(item.MEDICINE_LINE_CODE, 2))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Dòng thuốc");
                            mediAdo.MEDICINE_LINE_CODE_ERROR = 1;
                        }

                        var package = BackendDataWorker.Get<HIS_MEDICINE_LINE>().FirstOrDefault(o => o.MEDICINE_LINE_CODE == item.MEDICINE_LINE_CODE);
                        if (package != null)
                        {
                            mediAdo.MEDICINE_LINE_ID = package.ID;
                            mediAdo.MEDICINE_LINE_NAME = package.MEDICINE_LINE_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Dòng thuốc");
                            mediAdo.MEDICINE_LINE_CODE_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.ALERT_MAX_IN_TREATMENT_STR))
                    {
                        if (Inventec.Common.Number.Check.IsDecimal(item.ALERT_MAX_IN_TREATMENT_STR))
                        {
                            mediAdo.ALERT_MAX_IN_TREATMENT = Inventec.Common.TypeConvert.Parse.ToDecimal(item.ALERT_MAX_IN_TREATMENT_STR);
                            if ((mediAdo.ALERT_MAX_IN_TREATMENT ?? 0) > 9999999999999999 || (mediAdo.ALERT_MAX_IN_TREATMENT ?? 0) < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "SL max có thể kê cho 1 hsđt");
                                mediAdo.ALERT_MAX_IN_TREATMENT_STR_ERROR = 1;
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "SL max có thể kê cho 1 hsđt");
                            mediAdo.ALERT_MAX_IN_TREATMENT_STR_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.ALERT_MAX_IN_PRESCRIPTION_STR))
                    {
                        if (Inventec.Common.Number.Check.IsDecimal(item.ALERT_MAX_IN_PRESCRIPTION_STR))
                        {
                            mediAdo.ALERT_MAX_IN_PRESCRIPTION = Inventec.Common.TypeConvert.Parse.ToDecimal(item.ALERT_MAX_IN_PRESCRIPTION_STR);
                            if ((mediAdo.ALERT_MAX_IN_PRESCRIPTION ?? 0) > 9999999999999999 || (mediAdo.ALERT_MAX_IN_PRESCRIPTION ?? 0) < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "SL tối đa / 1 đơn");
                                mediAdo.ALERT_MAX_IN_PRESCRIPTION_STR_ERROR = 1;
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "SL tối đa / 1 đơn");
                            mediAdo.ALERT_MAX_IN_PRESCRIPTION_STR_ERROR = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.ALERT_MAX_IN_DAY_STR))
                    {
                        if (Inventec.Common.Number.Check.IsDecimal(item.ALERT_MAX_IN_DAY_STR))
                        {
                            mediAdo.ALERT_MAX_IN_DAY = Inventec.Common.TypeConvert.Parse.ToDecimal(item.ALERT_MAX_IN_DAY_STR);
                            if ((mediAdo.ALERT_MAX_IN_DAY ?? 0) > 9999999999999999 || (mediAdo.ALERT_MAX_IN_DAY ?? 0) < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Số lượng tối đa/ngày");
                                mediAdo.ALERT_MAX_IN_DAY_STR_ERROR = 1;
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Số lượng tối đa/ngày");
                            mediAdo.ALERT_MAX_IN_DAY_STR_ERROR = 1;
                        }
                    }
                    if (!string.IsNullOrEmpty(item.IS_BLOCK_MAX_IN_DAY_STR))
                    {
                        if (item.IS_BLOCK_MAX_IN_DAY_STR.Trim().ToLower() == "x")
                        {
                            mediAdo.IS_BLOCK_MAX_IN_DAY = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Chặn khi kê quá số lượng/ngày");
                            mediAdo.IS_BLOCK_MAX_IN_DAY_STR_ERROR = 1;
                        }
                    }
                    if (!string.IsNullOrEmpty(item.COGS_STR))
                    {
                        if (Inventec.Common.Number.Check.IsDecimal(item.COGS_STR))
                        {
                            mediAdo.COGS = Inventec.Common.TypeConvert.Parse.ToDecimal(item.COGS_STR);
                            if ((mediAdo.COGS ?? 0) > 99999999999999 || (mediAdo.COGS ?? 0) < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Giá vốn");
                                mediAdo.COGS_STR_ERROR = 1;
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Giá vốn");
                            mediAdo.COGS_STR_ERROR = 1;
                        }
                    }
                    if (!string.IsNullOrEmpty(item.RECORDING_TRANSACTION))
                    {
                        if (!CheckMaxLenth(item.RECORDING_TRANSACTION, 20))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Định khoản kế toán");
                            mediAdo.RECORDING_TRANSACTION_ERROR = 1;
                        }
                        mediAdo.RECORDING_TRANSACTION = item.RECORDING_TRANSACTION;
                    }
                    if (!string.IsNullOrEmpty(item.ATC_CODES_STR))
                    {
                        List<string> attachAtcCodes = new List<string>();
                        List<string> attachAtcErrors = new List<string>();
                        List<string> attachAtcCodeAvaiables = new List<string>();
                        List<string> attachAtcNameAvaiables = new List<string>();
                        if (item.ATC_CODES_STR.Contains(","))
                        {
                            attachAtcCodes = item.ATC_CODES_STR.Split(',').ToList();
                        }
                        else
                        {
                            attachAtcCodes = new List<string> { item.ATC_CODES_STR };
                        }
                        if (attachAtcCodes.Count() > 0)
                        {
                            var atcs = BackendDataWorker.Get<HIS_ATC>();
                            foreach (var atcCode in attachAtcCodes)
                            {
                                var atc = atcs.FirstOrDefault(o => o.ATC_CODE == atcCode);
                                if (atc != null)
                                {
                                    attachAtcCodeAvaiables.Add(atcCode);
                                    attachAtcNameAvaiables.Add(atc.ATC_NAME);
                                }
                                else
                                {
                                    attachAtcErrors.Add(atcCode);
                                }
                            }
                        }
                        if (attachAtcCodeAvaiables.Count() > 0)
                        {
                            mediAdo.ATC_CODES = string.Join(";", attachAtcCodeAvaiables);
                            mediAdo.ATC_NAMES_STR = string.Join(";", attachAtcNameAvaiables);
                        }
                        if (attachAtcErrors.Count() > 0)
                        {
                            error += string.Format("Mã {0} không có trong danh mục Thiết lập ATC", string.Join(";", attachAtcErrors));
                            mediAdo.ATC_CODES_ERROR = 1;
                        }
                    }
                    if (!string.IsNullOrEmpty(item.PREPROCESSING_CODE_STR))
                    {
                        List<string> attachPreprocessingCodes = new List<string>();
                        List<string> attachPreprocessingErrors = new List<string>();
                        List<string> attachPreprocessingCodeAvaiables = new List<string>();
                        List<string> attachPreprocessingNameAvaiables = new List<string>();
                        if (item.PREPROCESSING_CODE_STR.Contains(","))
                        {
                            attachPreprocessingCodes = item.PREPROCESSING_CODE_STR.Split(',').ToList();
                        }
                        else
                        {
                            attachPreprocessingCodes = new List<string> { item.PREPROCESSING_CODE_STR };
                        }
                        if (attachPreprocessingCodes.Count() > 0)
                        {
                            var _items = BackendDataWorker.Get<HIS_PROCESSING_METHOD>().Where(o => o.PROCESSING_METHOD_TYPE == 1).ToList();
                            foreach (var code in attachPreprocessingCodes)
                            {
                                var _item = _items.FirstOrDefault(o => o.PROCESSING_METHOD_CODE == code);
                                if (_item != null)
                                {
                                    attachPreprocessingCodeAvaiables.Add(code);
                                    attachPreprocessingNameAvaiables.Add(_item.PROCESSING_METHOD_NAME);
                                }
                                else
                                {
                                    attachPreprocessingErrors.Add(code);
                                }
                            }
                        }
                        if (attachPreprocessingCodeAvaiables.Count() > 0)
                        {
                            mediAdo.PREPROCESSING_CODE = string.Join(";", attachPreprocessingCodeAvaiables);
                            mediAdo.PREPROCESSING_NAME_STR = string.Join(";", attachPreprocessingNameAvaiables);
                        }
                        if (attachPreprocessingErrors.Count() > 0)
                        {
                            error += string.Format("Mã {0} không có trong danh mục Phương pháp chế biến", string.Join(";", attachPreprocessingErrors));
                            mediAdo.PREPROCESSING_ERROR = 1;
                        }
                    }
                    if (!string.IsNullOrEmpty(item.PROCESSING_CODE_STR))
                    {
                        List<string> attachProcessingCodes = new List<string>();
                        List<string> attachProcessingErrors = new List<string>();
                        List<string> attachProcessingCodeAvaiables = new List<string>();
                        List<string> attachProcessingNameAvaiables = new List<string>();
                        if (item.PROCESSING_CODE_STR.Contains(","))
                        {
                            attachProcessingCodes = item.PROCESSING_CODE_STR.Split(',').ToList();
                        }
                        else
                        {
                            attachProcessingCodes = new List<string> { item.PROCESSING_CODE_STR };
                        }
                        if (attachProcessingCodes.Count() > 0)
                        {
                            var _items = BackendDataWorker.Get<HIS_PROCESSING_METHOD>().Where(o => o.PROCESSING_METHOD_TYPE == 2).ToList();
                            foreach (var code in attachProcessingCodes)
                            {
                                var _item = _items.FirstOrDefault(o => o.PROCESSING_METHOD_CODE == code);
                                if (_item != null)
                                {
                                    attachProcessingCodeAvaiables.Add(code);
                                    attachProcessingNameAvaiables.Add(_item.PROCESSING_METHOD_NAME);
                                }
                                else
                                {
                                    attachProcessingErrors.Add(code);
                                }
                            }
                        }
                        if (attachProcessingCodeAvaiables.Count() > 0)
                        {
                            mediAdo.PROCESSING_CODE = string.Join(";", attachProcessingCodeAvaiables);
                            mediAdo.PROCESSING_NAME_STR = string.Join(";", attachProcessingNameAvaiables);
                        }
                        if (attachProcessingErrors.Count() > 0)
                        {
                            error += string.Format("Mã {0} không có trong danh mục Phương pháp chế biến", string.Join(";", attachProcessingErrors));
                            mediAdo.PROCESSING_ERROR = 1;
                        }
                    }
                    if (!string.IsNullOrEmpty(item.HTU_CODE))
                    {

                        var htu = BackendDataWorker.Get<HIS_HTU>().FirstOrDefault(o => o.HTU_CODE.ToLower() == item.HTU_CODE.ToLower());
                        if (htu != null)
                        {
                            mediAdo.HTU_ID = htu.ID;
                            mediAdo.HTU_CODE = htu.HTU_CODE;
                            mediAdo.HTU_NAME = htu.HTU_NAME;
                        }
                        else
                        {
                            error += string.Format("Cách dùng {0} không có trong danh mục Cách dùng thuốc.", item.HTU_CODE);
                            mediAdo.HTU_NAME = item.HTU_CODE;
                            mediAdo.HTU_CODE_ERROR = 1;
                        }
                    }
                    List<string> listCode = new List<string>();
                    MOS.Filter.HisActiveIngredientFilter filter = new MOS.Filter.HisActiveIngredientFilter();
                    filter.ORDER_FIELD = "MODIFY_TIME";
                    filter.ORDER_DIRECTION = "DESC";
                    var _dataApi = new BackendAdapter(new CommonParam()).Get<List<HIS_ACTIVE_INGREDIENT>>("/api/HisActiveIngredient/Get", ApiConsumers.MosConsumer, filter, null);
                    if (!string.IsNullOrEmpty(item.ACTIVE_INGREDIENT_CODE_STR))
                    {
                        mediAdo.ACTIVE_INGREDIENT_CODE_STR = item.ACTIVE_INGREDIENT_CODE_STR;
                        string[] param = item.ACTIVE_INGREDIENT_CODE_STR.Split(';');

                        mediAdo.ACTIVE_INGREDIENT_CODE_STR = "";
                        this.listMediAcin = new List<HIS_MEDICINE_TYPE_ACIN>();
                        string nameAcin = "";
                        foreach (var _i in param)
                        {
                            listCode.Add(_i);
                            if (_dataApi != null)
                            {
                                var _data = _dataApi.FirstOrDefault(s => s.ACTIVE_INGREDIENT_CODE == _i);
                                if (_data != null)
                                {
                                    //DialogResult rs = MessageBox.Show("tim thay " + _i + " voi ten: " + _data.ACTIVE_INGREDIENT_NAME);
                                    
                                    HIS_MEDICINE_TYPE_ACIN mediAcin = new HIS_MEDICINE_TYPE_ACIN();
                                    mediAcin.ACTIVE_INGREDIENT_ID = _data.ID;
                                    if (!this.listMediAcin.Exists(s => s.ACTIVE_INGREDIENT_ID == _data.ID))
                                    {
                                        nameAcin += string.Format(_data.ACTIVE_INGREDIENT_NAME + ";");
                                        this.listMediAcin.Add(mediAcin);
                                    }
                                }
                                else
                                {
                                    error += string.Format("Mã hoạt chất " + _i + " không hợp lệ");
                                }
                            }
                            
                            
                        }
                        
                        if (this.listMediAcin != null && this.listMediAcin.Count > 0)
                        {
                            listMediAcinCode.Add(item.MEDICINE_TYPE_CODE, this.listMediAcin);
                            mediAdo.ACTIVE_INGREDIENT_CODE_STR = nameAcin;
                        }
                        else
                        {

                            mediAdo.ACTIVE_INGREDIENT_CODE_STR = item.ACTIVE_INGREDIENT_CODE_STR;
                        }
                        
                    

                    }
                    
                    mediAdo.ERROR = error;
                    mediAdo.ID = i;
                    mediAdo.IS_LEAF = 1;

                    _medicineRef.Add(mediAdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        
        List<HIS_MEDICINE_TYPE_ACIN> listMediAcin;
        Dictionary<string,List<HIS_MEDICINE_TYPE_ACIN>> listMediAcinCode = new Dictionary<string,List<HIS_MEDICINE_TYPE_ACIN>>();
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

        #region public_function
        public void DelegateWarning()
        {
            try
            {
                btnRefresh.Enabled = true;
                addSuccess = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
    }
}
