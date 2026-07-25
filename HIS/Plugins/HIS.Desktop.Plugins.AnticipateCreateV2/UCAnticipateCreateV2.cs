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
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTreeList.Data;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.AnticipateCreateV2.ADO;
using HIS.Desktop.Plugins.AnticipateCreateV2.Base;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using MOS.Filter;
using System.Dynamic;
using System.Reflection;
 
 
namespace HIS.Desktop.Plugins.AnticipateCreateV2
{
    public partial class UCAnticipateCreateV2 : HIS.Desktop.Utility.UserControlBase
    {
        #region ---Decalre
        List<HisMedicineInStockSDO> lstMediInStocks { get; set; }
        List<HisMaterialInStockSDO> lstMateInStocks { get; set; }

        List<HisBloodInStockSDO> lstBloodInStocks { get; set; }
        List<HisBloodTypeInStockSDO> lstBlood { get; set; }

        Dictionary<long, string> dicMedicineTypeDescription = new Dictionary<long, string>();
        Dictionary<long, string> dicMaterialTypeDescription = new Dictionary<long, string>();

        // vCong 52461 — Nước sản xuất + Số đăng ký theo loại (cột danh mục mới trên cây). Key = typeId.
        internal Dictionary<long, string> dicMedicineTypeNational = new Dictionary<long, string>();
        internal Dictionary<long, string> dicMaterialTypeNational = new Dictionary<long, string>();
        internal Dictionary<long, string> dicMedicineTypeRegister = new Dictionary<long, string>();
        internal Dictionary<long, string> dicMaterialTypeRegister = new Dictionary<long, string>();
        // vCong 52461 — Quy cách/đóng gói theo loại (PACKING_TYPE_NAME). Key = typeId.
        internal Dictionary<long, string> dicMedicineTypePacking = new Dictionary<long, string>();
        internal Dictionary<long, string> dicMaterialTypePacking = new Dictionary<long, string>();

        V_HIS_MEDI_STOCK currentMediStock = null;
        internal List<long> mediStockIds = new List<long>();
        bool isCheck;

        Inventec.Desktop.Common.Modules.Module moduleData;
        long RoomId;
        long RoomTypeId;
        MOS.Filter.HisMedicineStockViewFilter mediFilter;
        internal string fileName = "";

        internal List<HIS_MEDI_STOCK_MATY> HisMediStockMaty { get; set; }
        internal List<HIS_MEDI_STOCK_MATY> HisMediStockMatyByStocks { get; set; }

        internal List<HIS_MEDI_STOCK_METY> HisMediStockMety { get; set; }
        internal List<HIS_MEDI_STOCK_METY> HisMediStockMetyByStocks { get; set; }

        internal List<MediStockADO> _MediStocks { get; set; }

        bool isCheckAll = false;
        public string IdComboStt { get; set; }
        public long IdComboPatientType { get; set; }
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        
        string ModuleLink = "HIS.Desktop.Plugins.AnticipateCreateV2";
        bool IsInitForm = false;
        // vCong 52461 — khôi phục chế độ "Dự trù theo: Thầu" ở deferred (tránh nạp bid-API lúc load).
        bool restoreSupplyMode = false;
        #endregion
        public UCAnticipateCreateV2(Inventec.Desktop.Common.Modules.Module module, long roomId, long roomTypeId)
            : base(module)
        {
            InitializeComponent();
            try
            {
                WaitingManager.Show();
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.moduleData = module;
                this.RoomId = roomId;
                this.RoomTypeId = roomTypeId;
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.AnticipateCreateV2.Resources.Lang", typeof(UCAnticipateCreateV2).Assembly);
                InitPivotTrees();
                InitDutruGrid();                 // vCong 52461 — tách panel: cây (trên) + grid dự trù (dưới)
                ShowResultControl(treeListPivot); // hiển thị cây mặc định vào Panel1
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        } 

        private void UCAnticipateCreateV2_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                InitDescriptionDictionaries();
                LoadDataGridMediStock();
                //ShowUCControl();
                var date = Inventec.Common.TypeConvert.Parse.ToDateTime(DateTime.Now.ToString());
                var DateTo = date.Year;
                DateTo++;
                var lastDayOfMonthTo = DateTime.DaysInMonth(DateTo, date.Month);
                var dat2 = Inventec.Common.TypeConvert.Parse.ToDateTime(lastDayOfMonthTo + "/" + date.Month + "/" + DateTo);
                InitImpExpFilter();
                InitAnticipateModeControls();   // vCong 52461 — radio Dự trù theo + combobox + 3 radio loại + nhóm lọc phải
                InitControlState();             // vCong 52461 — khôi phục trạng thái vào control runtime (nguồn sự thật)
                HideLotRelatedControls();      // Pivot theo kho: an cac bo loc gan voi dong LO (khong con dong lo)
                lcgChonKho.Expanded = false;   // Mac dinh thu gon panel chon kho (thu gon ngang) -> layoutControl3 fill
                formLoaded = true;

                // vCong 52461 — TỐI ƯU LOAD: dựng xong giao diện (Load trả về + form vẽ) RỒI mới nạp data.
                // ShowUCControl nặng (3 API: GetView tồn + GetWithImpExp + GetForAnticipate × nhiều kho) → nếu gọi
                // đồng bộ ngay trong Load thì form chưa vẽ đã đơ. BeginInvoke đẩy việc nạp xuống sau khi form hiển thị
                // (ShowUCControl tự bật WaitingManager → hiện "đang tải" thay vì treo trắng).
                if (this.mediStockIds != null && this.mediStockIds.Count > 0)
                {
                    this.BeginInvoke(new MethodInvoker(delegate
                    {
                        try
                        {
                            // Khôi phục chế độ "Dự trù theo: Thầu" (nạp bid-API) sau khi form đã hiển thị.
                            if (restoreSupplyMode && pluginRdoBySupply != null && !pluginRdoBySupply.Checked)
                                pluginRdoBySupply.Checked = true;
                            ShowUCControl();
                        }
                        catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
                    }));
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDescriptionDictionaries()
        {
            try
            {
                var medTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE>().GroupBy(o => o.ID).ToDictionary(g => g.Key, g => g.FirstOrDefault());
                var matTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE>().GroupBy(o => o.ID).ToDictionary(g => g.Key, g => g.FirstOrDefault());

                dicMedicineTypeDescription = medTypes.ToDictionary(kv => kv.Key, kv => kv.Value != null ? kv.Value.DESCRIPTION : null);
                dicMaterialTypeDescription = matTypes.ToDictionary(kv => kv.Key, kv => kv.Value != null ? kv.Value.DESCRIPTION : null);
                // vCong 52461 — Nước SX + Số đăng ký theo loại
                dicMedicineTypeNational = medTypes.ToDictionary(kv => kv.Key, kv => kv.Value != null ? kv.Value.NATIONAL_NAME : null);
                dicMaterialTypeNational = matTypes.ToDictionary(kv => kv.Key, kv => kv.Value != null ? kv.Value.NATIONAL_NAME : null);
                dicMedicineTypeRegister = medTypes.ToDictionary(kv => kv.Key, kv => kv.Value != null ? kv.Value.REGISTER_NUMBER : null);
                dicMaterialTypeRegister = matTypes.ToDictionary(kv => kv.Key, kv => kv.Value != null ? kv.Value.REGISTER_NUMBER : null);
                dicMedicineTypePacking = medTypes.ToDictionary(kv => kv.Key, kv => kv.Value != null ? kv.Value.PACKING_TYPE_NAME : null);
                dicMaterialTypePacking = matTypes.ToDictionary(kv => kv.Key, kv => kv.Value != null ? kv.Value.PACKING_TYPE_NAME : null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCboIsActive()
        {
            //try
            //{
            //    List<StatusADO> li = new List<StatusADO>();
            //    li.Add(new StatusADO("01", Inventec.Common.Resource.Get.Value("UCAnticipateCreateV2.cboValue1", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture())));
            //    li.Add(new StatusADO("02", Inventec.Common.Resource.Get.Value("UCAnticipateCreateV2.cboValue2", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture())));
            //    li.Add(new StatusADO("03", Inventec.Common.Resource.Get.Value("UCAnticipateCreateV2.cboValue3", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture())));
            //    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            //    columnInfos.Add(new ColumnInfo("ID", "", 50, 1));
            //    columnInfos.Add(new ColumnInfo("STATUS_NAME", "", 100, 2));
            //    ControlEditorADO controlEditorADO = new ControlEditorADO("STATUS_NAME", "ID", columnInfos, false, 150);
            //    ControlEditorLoader.Load(cboIsActive, li, controlEditorADO);
            //    cboIsActive.EditValue = "01";
            //    IdComboStt = "01";
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void CheckCboPatient()
        {
            try
            {
                var medi = BackendDataWorker.Get<HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == RoomId);
                if (medi != null)
                {
                    if (medi.IS_BUSINESS == 1)
                    {
                        var codeFee = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE");
                        var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == codeFee);
                        IdComboPatientType = patientType != null ? patientType.ID : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitControlState()
        {

           

            try
            {
                IsInitForm = true;
                if (chkAlertNew != null) chkAlertNew.Checked = false;

                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(this.ModuleLink);

                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item_ in this.currentControlStateRDO)
                    {
                        // Toàn bộ control lọc runtime là nguồn sự thật (control Designer gốc đã gỡ).
                        if (chkAlertNew != null && item_.KEY == chkAlertNew.Name)
                        {
                            chkAlertNew.Checked = item_.VALUE == "1";
                        }
                        else if (rdoThuocNew != null && item_.KEY == rdoThuocNew.Name)
                        {
                            if (item_.VALUE == "1") rdoThuocNew.Checked = true;   // Loại = Thuốc
                        }
                        else if (rdoVatTuNew != null && item_.KEY == rdoVatTuNew.Name)
                        {
                            if (item_.VALUE == "1") rdoVatTuNew.Checked = true;   // Loại = Vật tư
                        }
                        else if (rdoMauNew != null && item_.KEY == rdoMauNew.Name)
                        {
                            if (item_.VALUE == "1") rdoMauNew.Checked = true;     // Loại = Máu
                        }
                        else if (pluginChkHideGroup != null && item_.KEY == pluginChkHideGroup.Name)
                        {
                            // "Ẩn dòng nhóm" — control tạo runtime trong InitImpExpFilter (chạy trước InitControlState)
                            pluginChkHideGroup.Checked = item_.VALUE == "1";
                        }
                        else if (chkShowZeroNew != null && item_.KEY == chkShowZeroNew.Name)
                        {
                            // "Hiển thị dòng hết"
                            chkShowZeroNew.Checked = item_.VALUE == "1";
                        }
                        else if (pluginRdoBySupply != null && item_.KEY == pluginRdoBySupply.Name)
                        {
                            // "Dự trù theo: Thầu" — KHÔNG set radio ở đây (tránh EnsureBidDataLoaded lúc load);
                            // đánh dấu để khôi phục ở deferred sau khi form hiển thị.
                            restoreSupplyMode = item_.VALUE == "1";
                        }
                    }
                }
                IsInitForm = false;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Lưu trạng thái 1 control (check/radio) vào ControlState theo KEY. Bỏ qua khi đang init.</summary>
        internal void SaveCheckState(string key, bool isChecked)
        {
            try
            {
                if (IsInitForm || string.IsNullOrEmpty(key)) return;
                var item = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                    ? this.currentControlStateRDO.FirstOrDefault(o => o.KEY == key && o.MODULE_LINK == this.ModuleLink)
                    : null;
                if (item != null)
                {
                    item.VALUE = isChecked ? "1" : "";
                }
                else
                {
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(new HIS.Desktop.Library.CacheClient.ControlStateRDO
                    {
                        KEY = key,
                        VALUE = isChecked ? "1" : "",
                        MODULE_LINK = this.ModuleLink
                    });
                }
                if (this.controlStateWorker != null)
                    this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitCboPatientType()
        {
            try
            {
                // vCong 52461 — cboPatientType đã bị gỡ khỏi Designer (không dùng trong dự trù). Giữ shell no-op.
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

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCAnticipateCreateV2.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCAnticipateCreateV2.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCAnticipateCreateV2.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyWork.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCAnticipateCreateV2.txtKeyWork.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewMediStock.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("UCAnticipateCreateV2.gridViewMediStock.OptionsFind.FindNullPrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnCheck.Caption = Inventec.Common.Resource.Get.Value("UCAnticipateCreateV2.gridColumnCheck.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCAnticipateCreateV2.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCAnticipateCreateV2.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCAnticipateCreateV2.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCAnticipateCreateV2.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataMediStockMetyAndMaty()
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisMediStockMatyFilter HisMediStockMatyFilter = new MOS.Filter.HisMediStockMatyFilter();
                HisMediStockMaty = new List<HIS_MEDI_STOCK_MATY>();
                HisMediStockMaty = new BackendAdapter(param).Get<List<HIS_MEDI_STOCK_MATY>>("/api/HisMediStockMaty/Get", ApiConsumers.MosConsumer, HisMediStockMatyFilter, param);

                MOS.Filter.HisMediStockMetyFilter HisMediStockMetyFilter = new MOS.Filter.HisMediStockMetyFilter();
                HisMediStockMety = new List<HIS_MEDI_STOCK_METY>();
                HisMediStockMety = new BackendAdapter(param).Get<List<HIS_MEDI_STOCK_METY>>("/api/HisMediStockMety/Get", ApiConsumers.MosConsumer, HisMediStockMetyFilter, param);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowUCControl()
        {
            try
            {
                // Giữ lại text tìm trên cây kết quả (bind lại sẽ làm mất FindFilterText)
                string preFindText = treeListPivot != null ? treeListPivot.FindFilterText : null;

                this.mediStockIds = new List<long>();
                if (this._MediStocks != null && this._MediStocks.Count > 0)
                {
                    this.mediStockIds.AddRange(this._MediStocks.Where(p => p.IsCheck == true).Select(p => p.ID).ToList());
                }
                if (this.mediStockIds.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn kho", "Thông báo");
                    return;
                }

                // Khoảng thời gian nhập/xuất: Từ ngày phải <= Đến ngày
                long? impFromTime;
                long? impToTime;
                if (!TryGetImpExpRange(out impFromTime, out impToTime))
                {
                    return;
                }

                bool isIncludeBaseAmount = (this.mediStockIds.Count == 1 && this._MediStocks.Any(a => a.ID == mediStockIds.FirstOrDefault() && a.IS_CABINET == 1));

                // Cảnh báo tồn tối thiểu: chỉ khả dụng khi xem đúng 1 kho (giữ hành vi cũ)
                if (chkAlertNew != null)
                {
                    chkAlertNew.Enabled = this.mediStockIds.Count == 1;
                    if (!chkAlertNew.Enabled && chkAlertNew.Checked)
                    {
                        chkAlertNew.Checked = false;
                    }
                }
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                if (IsMedicineChecked)
                {
                    // Thuốc — cây tồn GỘP theo loại trên các kho đã chọn (không gửi INCLUDE_MEDI_STOCK;
                    // số liệu theo từng kho lấy từ GetWithImpExp để pivot thành cột kho động)
                    MOS.Filter.HisMedicineStockViewFilter mediFilter = new MOS.Filter.HisMedicineStockViewFilter();
                    mediFilter.MEDI_STOCK_IDs = this.mediStockIds;
                    mediFilter.INCLUDE_EMPTY = IsShowLineZeroChecked;
                    mediFilter.INCLUDE_BASE_AMOUNT = isIncludeBaseAmount;

                    lstMediInStocks = new BackendAdapter(param).Get<List<HisMedicineInStockSDO>>(HisRequestUriStore.HIS_MEDICINE_GETVIEW_IN_STOCK_MEDICINE_TYPE_TREE, ApiConsumers.MosConsumer, mediFilter, param);
                    BuildImpExpDictionary(true, impFromTime, impToTime);
                    BuildAnticipateData(true, impFromTime, impToTime);   // vCong 52461 — số liệu dự trù theo loại

                    if (lstMediInStocks != null && lstMediInStocks.Count > 0)
                    {
                        var dataMediStocks = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => this.mediStockIds.Contains(p.ID) && p.IS_BUSINESS == 1).ToList();
                        if (dataMediStocks != null && dataMediStocks.Count == this.mediStockIds.Count)
                        {
                            var dataMediTypes = BackendDataWorker.Get<HIS_MEDICINE_TYPE>().Where(p => p.IS_BUSINESS == 1).ToList();
                            if (dataMediTypes != null && dataMediTypes.Count > 0)
                            {
                                lstMediInStocks = lstMediInStocks.Where(p => dataMediTypes.Select(o => o.ID).Distinct().ToList().Contains(p.MEDICINE_TYPE_ID)).ToList();
                            }
                            else
                            {
                                lstMediInStocks = new List<HisMedicineInStockSDO>();
                            }
                        }
                        else
                        {
                            var dataMediStocksBUSINESS = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => this.mediStockIds.Contains(p.ID) && p.IS_BUSINESS != 1).ToList();
                            if (dataMediStocksBUSINESS != null && dataMediStocksBUSINESS.Count == this.mediStockIds.Count)
                            {
                                var dataMediTypes = BackendDataWorker.Get<HIS_MEDICINE_TYPE>().Where(p => p.IS_BUSINESS != 1).ToList();
                                if (dataMediTypes != null && dataMediTypes.Count > 0)
                                {
                                    lstMediInStocks = lstMediInStocks.Where(p => dataMediTypes.Select(o => o.ID).Distinct().ToList().Contains(p.MEDICINE_TYPE_ID)).ToList();
                                }
                                else
                                {
                                    lstMediInStocks = new List<HisMedicineInStockSDO>();
                                }
                            }
                        }
                    }

                    ShowResultControl(treeListPivot);
                    BindPivotTree(true);
                }
                else if (IsMaterialChecked)
                {
                    // Vật tư — tương tự thuốc
                    MOS.Filter.HisMaterialStockViewFilter mateFilter = new MOS.Filter.HisMaterialStockViewFilter();
                    mateFilter.MEDI_STOCK_IDs = this.mediStockIds;
                    mateFilter.INCLUDE_EMPTY = IsShowLineZeroChecked;
                    mateFilter.INCLUDE_BASE_AMOUNT = isIncludeBaseAmount;

                    lstMateInStocks = new BackendAdapter(param).Get<List<HisMaterialInStockSDO>>(HisRequestUriStore.HIS_MATERIAL_GETVIEW_IN_STOCK_MATERIAL_TYPE_TREE, ApiConsumers.MosConsumer, mateFilter, param);
                    BuildImpExpDictionary(false, impFromTime, impToTime);
                    BuildAnticipateData(false, impFromTime, impToTime);   // vCong 52461 — số liệu dự trù theo loại

                    if (lstMateInStocks != null && lstMateInStocks.Count > 0)
                    {
                        var dataMediStocks = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => this.mediStockIds.Contains(p.ID) && p.IS_BUSINESS == 1).ToList();
                        if (dataMediStocks != null && dataMediStocks.Count == this.mediStockIds.Count)
                        {
                            var dataMateTypes = BackendDataWorker.Get<HIS_MATERIAL_TYPE>().Where(p => p.IS_BUSINESS == 1).ToList();
                            if (dataMateTypes != null && dataMateTypes.Count > 0)
                            {
                                lstMateInStocks = lstMateInStocks.Where(p => dataMateTypes.Select(o => o.ID).Distinct().ToList().Contains(p.MATERIAL_TYPE_ID)).ToList();
                            }
                            else
                            {
                                lstMateInStocks = new List<HisMaterialInStockSDO>();
                            }
                        }
                        else
                        {
                            var dataMediStocksBUSINESS = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => this.mediStockIds.Contains(p.ID) && p.IS_BUSINESS != 1).ToList();
                            if (dataMediStocksBUSINESS != null && dataMediStocksBUSINESS.Count == this.mediStockIds.Count)
                            {
                                var dataMateTypes = BackendDataWorker.Get<HIS_MATERIAL_TYPE>().Where(p => p.IS_BUSINESS != 1).ToList();
                                if (dataMateTypes != null && dataMateTypes.Count > 0)
                                {
                                    lstMateInStocks = lstMateInStocks.Where(p => dataMateTypes.Select(o => o.ID).Distinct().ToList().Contains(p.MATERIAL_TYPE_ID)).ToList();
                                }
                                else
                                {
                                    lstMateInStocks = new List<HisMaterialInStockSDO>();
                                }
                            }
                        }
                    }

                    ShowResultControl(treeListPivot);
                    BindPivotTree(false);
                }
                else
                {
                    // Máu — bind thẳng HisBloodInStockSDO vào cây tự dựng (fix tab Máu trống — task 48711)
                    HisBloodStockViewFilter filterBlood = new HisBloodStockViewFilter();
                    filterBlood.MEDI_STOCK_IDs = this.mediStockIds;
                    lstBloodInStocks = new BackendAdapter(param).Get<List<HisBloodInStockSDO>>("api/HisBlood/GetInStockBloodWithTypeTree", ApiConsumers.MosConsumer, filterBlood, param);
                    if (lstBloodInStocks != null)
                    {
                        lstBloodInStocks = lstBloodInStocks.OrderBy(o => o.BloodTypeCode).ToList();
                    }
                    ShowResultControl(treeListBlood);
                    BindBloodTree();
                }
                // Gán lại text tìm trên cây kết quả sau khi bind (nếu có text sẽ tự bung node khớp)
                if (!IsBloodChecked && treeListPivot != null && !string.IsNullOrEmpty(preFindText))
                {
                    treeListPivot.ApplyFindFilter(preFindText);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataGridMediStock()
        {
            try
            {
                WaitingManager.Show();
                gridControlMediStock.DataSource = null;
                var _WorkPlace = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.RoomId);
                if (_WorkPlace != null)
                {
                    _MediStocks = new List<MediStockADO>();

                    var datas = BackendDataWorker.Get<V_HIS_USER_ROOM>().Where(p => p.LOGINNAME.Trim() == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName().Trim() && _WorkPlace.BranchId == p.BRANCH_ID).ToList();
                    List<V_HIS_MEDI_STOCK> dataMediStocks = new List<V_HIS_MEDI_STOCK>();
                    if (datas != null)
                    {
                        List<long> roomIds = datas.Select(p => p.ROOM_ID).ToList();
                        dataMediStocks = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => roomIds.Contains(p.ROOM_ID)).ToList();
                    }

                    if (dataMediStocks != null && dataMediStocks.Count > 0)
                    {
                        this._MediStocks.AddRange((from r in dataMediStocks select new MediStockADO(r)).ToList());
                        // Yeu cau vCong: thong ke TAT CA cac kho -> tu tich chon toan bo kho kha dung khi mo man hinh.
                        this.mediStockIds.Clear();
                        foreach (var stock in this._MediStocks)
                        {
                            stock.IsCheck = true;
                            this.mediStockIds.Add(stock.ID);
                        }
                        // Kho cua phong hien tai duoc dua len dau danh sach cho de nhin.
                        var data = _MediStocks.FirstOrDefault(p => p.ROOM_ID == this.RoomId);
                        if (data != null)
                        {
                            data.TYPE = 1;
                        }
                        this._MediStocks = this._MediStocks.OrderBy(p => p.TYPE).ToList();
                        gridControlMediStock.DataSource = _MediStocks;
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediStock_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HIS_MEDI_STOCK)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataGridMediStock();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataGridMediStock();
                ClearResultPanel();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click_1(object sender, EventArgs e)
        {
            try
            {
                ShowUCControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefesh_Click_1(object sender, EventArgs e)
        {
            try
            {
                // ----- Reset bộ lọc về mặc định (không tự tìm khi reset) -----
                bool prevLoaded = formLoaded;
                formLoaded = false;
                if (rdoThuocNew != null) rdoThuocNew.Checked = true;   // Loại = Thuốc (mặc định)
                if (txtKeyWork != null) txtKeyWork.Text = ""; // Từ khóa rỗng
                if (pluginTxtTreeSearch != null) pluginTxtTreeSearch.Text = "";
                currentTreeKeyword = "";
                ResetImpExpFilterDefault();                  // Từ ngày = 01 tháng hiện tại, Đến ngày = hôm nay
                formLoaded = prevLoaded;

                // ----- Xóa kết quả -----
                this.mediStockIds = new List<long>();
                if (dicMediImpExp != null) dicMediImpExp.Clear();
                if (dicMateImpExp != null) dicMateImpExp.Clear();
                LoadDataGridMediStock();                     // nạp lại danh sách kho, tích sẵn kho hiện tại
                ClearResultPanel();                          // hiển thị cây kết quả trống (theo Loại mặc định)
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void bbtnSearch()
        {
            try
            {
                btnSearch_Click_1(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void bbtnRefesh()
        {
            try
            {
                btnRefesh_Click_1(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            try
            {
                LoadPrint();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPrint()
        {
            try
            {
                // Xuất Excel trực tiếp từ cây kết quả đang hiển thị — khớp 100% màn hình pivot
                // (bao gồm n cột kho động); thay cho xuất theo file template cố định Tmp/Exp/*.xls.
                ExportActiveTreeToExcel();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyWork_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                SearchClick(strValue);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SearchClick(string keyword)
        {
            try
            {
                List<MediStockADO> listResult = new List<MediStockADO>();
                if (!String.IsNullOrEmpty(keyword.Trim()))
                {
                    List<MediStockADO> rearchResult = new List<MediStockADO>();

                    rearchResult = this._MediStocks.Where(o =>
                                                     ((o.DEPARTMENT_CODE ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                     || (o.DEPARTMENT_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                     || (o.MEDI_STOCK_CODE ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                     || (o.MEDI_STOCK_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                     || (o.ROOM_TYPE_CODE ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                     || (o.ROOM_TYPE_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                     )
                                                     ).Distinct().ToList();


                    listResult.AddRange(rearchResult);
                }
                else
                {
                    listResult.AddRange(this._MediStocks);
                }
                gridControlMediStock.BeginUpdate();
                if (listResult != null && listResult.Count > 0)
                {
                    listResult = listResult.OrderBy(p => p.TYPE).ToList();
                }

                gridControlMediStock.DataSource = listResult;
                gridControlMediStock.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediStock_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            view.ShowEditor();
                            DevExpress.XtraEditors.CheckEdit checkEdit = view.ActiveEditor as DevExpress.XtraEditors.CheckEdit;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {

                                checkEdit.Checked = !checkEdit.Checked;
                                view.CloseEditor();

                                var dataRow = (MediStockADO)gridViewMediStock.GetRow(hi.RowHandle);
                                if (checkEdit.Checked && dataRow != null)
                                {
                                    dataRow.TYPE = 1;
                                }
                                else
                                {
                                    dataRow.TYPE = 999;
                                }
                                if (this._MediStocks != null && this._MediStocks.Count > 0)
                                {
                                    var dataChecks = this._MediStocks.Where(p => p.IsCheck == true).ToList();
                                    if (dataChecks != null && dataChecks.Count > 0)
                                    {
                                        gridColumnCheck.Image = imageListIcon.Images[5];
                                    }
                                    else
                                    {
                                        gridColumnCheck.Image = imageListIcon.Images[6];
                                    }

                                }
                            }
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                    }
                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "IsCheck")
                        {
                            gridColumnCheck.Image = imageListIcon.Images[5];
                            gridViewMediStock.BeginUpdate();
                            if (this._MediStocks == null)
                                this._MediStocks = new List<MediStockADO>();
                            if (isCheckAll == true)
                            {
                                foreach (var item in this._MediStocks)
                                {
                                    item.IsCheck = true;
                                    item.TYPE = 1;
                                }
                                isCheckAll = false;
                            }
                            else
                            {
                                gridColumnCheck.Image = imageListIcon.Images[6];
                                foreach (var item in this._MediStocks)
                                {
                                    item.IsCheck = false;
                                    item.TYPE = 999;
                                }
                                isCheckAll = true;
                            }
                            gridViewMediStock.EndUpdate();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Pivot theo kho: các bộ lọc gắn với dòng LÔ không còn tác dụng (grid chỉ còn dòng loại)
        /// → ẩn khỏi giao diện. Không xóa control/handler để không đụng Designer.
        /// </summary>
        private void HideLotRelatedControls()
        {
            try
            {
                var lotItems = new DevExpress.XtraLayout.LayoutControlItem[]
                {
                    
                };
                foreach (var lci in lotItems)
                {
                    if (lci != null)
                        lci.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
