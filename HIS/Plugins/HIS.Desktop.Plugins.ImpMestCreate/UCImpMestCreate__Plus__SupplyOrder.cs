using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraLayout;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ImpMestCreate.ADO;
using HIS.UC.MaterialType.ADO;
using HIS.UC.MedicineType.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ImpMestCreate
{
    /// <summary>
    /// Viec 39336 - Cung ung hang hoa: chon don dat hang doi chieu khi nhap tu nha cung cap.
    /// Chi hien khi khoa MOS.HIS_ANTICIPATE.SUPPLY.MEDI_STOCK_CODES co gia tri va kho nhap thuoc danh sach;
    /// khoa trong thi khong tao control, khong goi API, luu phieu nhu cu.
    /// </summary>
    public partial class UCImpMestCreate
    {
        private const string SUPPLY__CONFIG_KEY_MEDI_STOCK_CODES = "MOS.HIS_ANTICIPATE.SUPPLY.MEDI_STOCK_CODES";
        private const short SUPPLY__ORDER_STT_CONFIRMED = 2;
        private const string SUPPLY__FORMAT_AMOUNT = "#,##0.####";

        private List<string> supplyMediStockCodes;
        private LookUpEdit cboSupplyOrder;
        private TextEdit txtSupplyReplaceCode;
        private LayoutControlItem lciSupplyOrder;
        private LayoutControlItem lciSupplyReplaceCode;
        private GridColumn colSupplyOrderAmount;
        private GridColumn colSupplyOrderArrived;
        private GridColumn colSupplyOrderWait;
        private List<V_HIS_SUPPLY_ORDER> supplyOrders = new List<V_HIS_SUPPLY_ORDER>();
        private Dictionary<string, HisSupplyOrderDetailSDO> dicSupplyOrderDetail = new Dictionary<string, HisSupplyOrderDetailSDO>();
        private string supplyOrderLoadedKey;
        private bool isSupplyOrderBinding = false;
        private long? supplyOrderTreeFilterId;

        /// <summary>
        /// Truong don dat hang dang hien (kho thuoc danh sach cung ung + loai nhap NCC)
        /// </summary>
        internal bool IsSupplyOrderActive
        {
            get { return this.lciSupplyOrder != null && this.lciSupplyOrder.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always; }
        }

        internal long? GetSupplyOrderId()
        {
            if (!this.IsSupplyOrderActive || this.cboSupplyOrder.EditValue == null)
            {
                return null;
            }
            return Convert.ToInt64(this.cboSupplyOrder.EditValue);
        }

        internal string GetSupplyReplaceImpMestCode()
        {
            if (!this.GetSupplyOrderId().HasValue || String.IsNullOrWhiteSpace(this.txtSupplyReplaceCode.Text))
            {
                return null;
            }
            return this.txtSupplyReplaceCode.Text.Trim();
        }

        /// <summary>
        /// Gan don doi chieu vao phieu nhap. Truong khong hien thi thi giu nguyen gia tri cu cua phieu
        /// </summary>
        internal void SetSupplyOrderToImpMest(HIS_IMP_MEST impMest)
        {
            if (impMest == null || !this.IsSupplyOrderActive)
            {
                return;
            }
            impMest.SUPPLY_ORDER_ID = this.GetSupplyOrderId();
            impMest.SUPPLY_REPLACE_IMP_MEST_CODE = this.GetSupplyReplaceImpMestCode();
        }

        private void InitSupplyOrder()
        {
            try
            {
                if (this.cboSupplyOrder != null)
                {
                    return;
                }
                string value = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SUPPLY__CONFIG_KEY_MEDI_STOCK_CODES);
                this.supplyMediStockCodes = String.IsNullOrWhiteSpace(value)
                    ? new List<string>()
                    : value.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(o => o.Trim()).Where(o => o != "").Distinct().ToList();
                if (this.supplyMediStockCodes.Count == 0)
                {
                    return;
                }

                this.cboSupplyOrder = new LookUpEdit();
                this.cboSupplyOrder.Name = "cboSupplyOrder";
                this.cboSupplyOrder.Properties.ValueMember = "ID";
                this.cboSupplyOrder.Properties.DisplayMember = "SUPPLY_ORDER_CODE";
                this.cboSupplyOrder.Properties.NullText = "";
                this.cboSupplyOrder.Properties.ShowHeader = true;
                this.cboSupplyOrder.Properties.PopupWidth = 420;
                this.cboSupplyOrder.Properties.Columns.Add(new LookUpColumnInfo("SUPPLY_ORDER_CODE", "Số đơn", 100));
                this.cboSupplyOrder.Properties.Columns.Add(new LookUpColumnInfo("ORDER_TIME_DISPLAY", "Ngày đặt", 120));
                LookUpColumnInfo colTotal = new LookUpColumnInfo("TOTAL_PRICE", "Thành tiền", 110);
                colTotal.FormatType = DevExpress.Utils.FormatType.Numeric;
                colTotal.FormatString = "#,##0.##";
                this.cboSupplyOrder.Properties.Columns.Add(colTotal);
                this.cboSupplyOrder.Properties.Columns.Add(new LookUpColumnInfo("STATUS_DISPLAY", "Trạng thái", 90));
                this.cboSupplyOrder.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Delete));
                this.cboSupplyOrder.ToolTip = "Đơn đặt hàng đã xác nhận đặt của đúng kho và nhà cung cấp. Không chọn sẵn đơn nào";
                this.cboSupplyOrder.ButtonClick += (s, e) =>
                {
                    if (e.Button.Kind == ButtonPredefines.Delete)
                    {
                        this.cboSupplyOrder.EditValue = null;
                    }
                };
                this.cboSupplyOrder.EditValueChanged += this.cboSupplyOrder_EditValueChanged;

                this.txtSupplyReplaceCode = new TextEdit();
                this.txtSupplyReplaceCode.Name = "txtSupplyReplaceCode";
                this.txtSupplyReplaceCode.Properties.MaxLength = 50;
                this.txtSupplyReplaceCode.ToolTip = "Mã phiếu nhập đã hủy của cùng đơn mà phiếu này thay thế (nếu có)";

                // dong moi ngay tren luoi chi tiet, ca chieu ngang cua luoi
                this.layoutControl1.BeginUpdate();
                try
                {
                    this.lciSupplyOrder = this.layoutControlGroup1.AddItem("Đơn đặt hàng:", this.cboSupplyOrder, this.layoutControlItem20, DevExpress.XtraLayout.Utils.InsertType.Top);
                    this.lciSupplyReplaceCode = this.layoutControlGroup1.AddItem("Thay phiếu:", this.txtSupplyReplaceCode, this.lciSupplyOrder, DevExpress.XtraLayout.Utils.InsertType.Right);
                    foreach (LayoutControlItem lci in new LayoutControlItem[] { this.lciSupplyOrder, this.lciSupplyReplaceCode })
                    {
                        lci.SizeConstraintsType = SizeConstraintsType.Custom;
                        lci.MinSize = new System.Drawing.Size(120, 24);
                        lci.MaxSize = new System.Drawing.Size(0, 24);
                        lci.TextAlignMode = TextAlignModeItem.AutoSize;
                    }
                    this.lciSupplyOrder.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    this.lciSupplyOrder.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.lciSupplyReplaceCode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                finally
                {
                    this.layoutControl1.EndUpdate();
                }

                this.colSupplyOrderAmount = this.AddSupplyOrderColumn("SUPPLY_ORDER_AMOUNT", "Đặt", "Lượng đặt của mặt hàng trong đơn đã chọn (theo ĐVT)");
                this.colSupplyOrderArrived = this.AddSupplyOrderColumn("SUPPLY_ORDER_ARRIVED", "Đã về", "Lượng trên các phiếu nhập theo đơn đang ở Yêu cầu/Đã duyệt, chưa thực nhập (theo ĐVT)");
                this.colSupplyOrderWait = this.AddSupplyOrderColumn("SUPPLY_ORDER_WAIT", "Còn chờ", "Đặt - Đã về - Đã thực nhập (theo ĐVT)");
                this.gridViewImpMestDetail.CustomUnboundColumnData += this.gridViewImpMestDetail_SupplyOrderUnboundData;

                this.cboImpMestType.EditValueChanged += (s, e) => this.ReloadSupplyOrders();
                this.cboMediStock.EditValueChanged += (s, e) => this.ReloadSupplyOrders();
                this.txtNhaCC.EditValueChanged += (s, e) => this.ReloadSupplyOrders();
                this.ReloadSupplyOrders();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private GridColumn AddSupplyOrderColumn(string fieldName, string caption, string toolTip)
        {
            GridColumn col = new GridColumn();
            col.FieldName = fieldName;
            col.Caption = caption;
            col.ToolTip = toolTip;
            col.UnboundType = DevExpress.Data.UnboundColumnType.Decimal;
            col.Width = 70;
            col.OptionsColumn.AllowEdit = false;
            col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            col.DisplayFormat.FormatString = SUPPLY__FORMAT_AMOUNT;
            col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            col.AppearanceCell.ForeColor = System.Drawing.Color.DarkBlue;
            col.Visible = false;
            this.gridViewImpMestDetail.Columns.Add(col);
            return col;
        }

        private long? GetSelectedImpMestTypeIdForSupply()
        {
            return this.cboImpMestType.EditValue != null ? (long?)Convert.ToInt64(this.cboImpMestType.EditValue) : null;
        }

        /// <summary>
        /// Kho nhap + loai nhap NCC + nha cung cap: nap cac don Da xac nhan dat. Doi kho / NCC / loai nhap thi bo chon don
        /// </summary>
        private void ReloadSupplyOrders()
        {
            try
            {
                if (this.cboSupplyOrder == null || this.isSupplyOrderBinding)
                {
                    return;
                }
                long? stockId = this.cboMediStock.EditValue != null ? (long?)Convert.ToInt64(this.cboMediStock.EditValue) : null;
                long? supplierId = this.txtNhaCC.EditValue != null ? (long?)Convert.ToInt64(this.txtNhaCC.EditValue) : null;
                bool show = false;
                if (stockId.HasValue && this.GetSelectedImpMestTypeIdForSupply() == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                {
                    V_HIS_MEDI_STOCK stock = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == stockId.Value);
                    show = stock != null && this.supplyMediStockCodes.Contains(stock.MEDI_STOCK_CODE);
                }
                this.lciSupplyOrder.Visibility = show ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                string key = show && supplierId.HasValue ? stockId + "_" + supplierId : null;
                if (key == this.supplyOrderLoadedKey)
                {
                    this.ApplySupplyOrderSelection();
                    return;
                }
                this.supplyOrderLoadedKey = key;
                object oldValue = this.cboSupplyOrder.EditValue;
                this.supplyOrders = new List<V_HIS_SUPPLY_ORDER>();
                if (key != null)
                {
                    CommonParam param = new CommonParam();
                    HisSupplyOrderViewFilter filter = new HisSupplyOrderViewFilter();
                    filter.MEDI_STOCK_ID = stockId.Value;
                    filter.SUPPLIER_ID = supplierId.Value;
                    filter.SUPPLY_ORDER_STT = SUPPLY__ORDER_STT_CONFIRMED;
                    filter.ORDER_FIELD = "ORDER_TIME";
                    filter.ORDER_DIRECTION = "DESC";
                    this.supplyOrders = new BackendAdapter(param).Get<List<V_HIS_SUPPLY_ORDER>>("api/HisSupplyOrder/GetView", ApiConsumers.MosConsumer, filter, param) ?? new List<V_HIS_SUPPLY_ORDER>();

                    // phieu dang sua da gan don (co the don da huy sau do): van hien de nguoi dung thay
                    long? linkedId = this._currentImpMestUp != null ? this._currentImpMestUp.SUPPLY_ORDER_ID : null;
                    if (linkedId.HasValue && !this.supplyOrders.Any(o => o.ID == linkedId.Value))
                    {
                        HisSupplyOrderViewFilter idFilter = new HisSupplyOrderViewFilter();
                        idFilter.ID = linkedId.Value;
                        List<V_HIS_SUPPLY_ORDER> linked = new BackendAdapter(param).Get<List<V_HIS_SUPPLY_ORDER>>("api/HisSupplyOrder/GetView", ApiConsumers.MosConsumer, idFilter, param);
                        if (linked != null)
                        {
                            this.supplyOrders.AddRange(linked);
                        }
                    }
                }

                this.isSupplyOrderBinding = true;
                try
                {
                    this.cboSupplyOrder.Properties.DataSource = this.supplyOrders.Select(o => new
                    {
                        o.ID,
                        o.SUPPLY_ORDER_CODE,
                        ORDER_TIME_DISPLAY = o.ORDER_TIME.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(o.ORDER_TIME.Value) : "",
                        o.TOTAL_PRICE,
                        STATUS_DISPLAY = o.SUPPLY_ORDER_STT == SUPPLY__ORDER_STT_CONFIRMED ? "Đã xác nhận đặt" : "Đã hủy"
                    }).ToList();

                    // khong tu chon don (BR12): chi giu don dang chon hoac don da gan cua phieu dang sua
                    long? keepId = null;
                    if (oldValue != null && this.supplyOrders.Any(o => o.ID == Convert.ToInt64(oldValue)))
                    {
                        keepId = Convert.ToInt64(oldValue);
                    }
                    else if (this._currentImpMestUp != null && this._currentImpMestUp.SUPPLY_ORDER_ID.HasValue && this.supplyOrders.Any(o => o.ID == this._currentImpMestUp.SUPPLY_ORDER_ID.Value))
                    {
                        keepId = this._currentImpMestUp.SUPPLY_ORDER_ID;
                        this.txtSupplyReplaceCode.Text = this._currentImpMestUp.SUPPLY_REPLACE_IMP_MEST_CODE;
                    }
                    this.cboSupplyOrder.EditValue = keepId;
                }
                finally
                {
                    this.isSupplyOrderBinding = false;
                }
                this.LoadSupplyOrderDetail();
            }
            catch (Exception ex)
            {
                this.isSupplyOrderBinding = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSupplyOrder_EditValueChanged(object sender, EventArgs e)
        {
            if (this.isSupplyOrderBinding)
            {
                return;
            }
            this.LoadSupplyOrderDetail();
        }

        /// <summary>
        /// Luong dat / da ve cua don dang chon, hien them 3 cot tren luoi chi tiet
        /// </summary>
        private void LoadSupplyOrderDetail()
        {
            try
            {
                this.dicSupplyOrderDetail = new Dictionary<string, HisSupplyOrderDetailSDO>();
                long? orderId = this.GetSupplyOrderId();
                if (orderId.HasValue)
                {
                    CommonParam param = new CommonParam();
                    HisSupplyOrderDetailFilter filter = new HisSupplyOrderDetailFilter();
                    filter.SUPPLY_ORDER_ID = orderId.Value;
                    List<HisSupplyOrderDetailSDO> details = new BackendAdapter(param).Get<List<HisSupplyOrderDetailSDO>>("api/HisSupplyOrderDetail/Get", ApiConsumers.MosConsumer, filter, param) ?? new List<HisSupplyOrderDetailSDO>();
                    this.dicSupplyOrderDetail = details.GroupBy(o => SupplyOrderDetailKey(o.IS_MATERIAL, o.TYPE_ID)).ToDictionary(g => g.Key, g => g.First());
                }
                else if (this.txtSupplyReplaceCode != null && !this.isSupplyOrderBinding)
                {
                    this.txtSupplyReplaceCode.Text = "";
                }
                this.ApplySupplyOrderSelection();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ApplySupplyOrderSelection()
        {
            bool hasOrder = this.GetSupplyOrderId().HasValue;
            this.lciSupplyReplaceCode.Visibility = hasOrder ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            int index = this.gridColumn_ImpMestDetail_Amount.VisibleIndex >= 0 ? this.gridColumn_ImpMestDetail_Amount.VisibleIndex + 1 : this.gridViewImpMestDetail.VisibleColumns.Count;
            foreach (GridColumn col in new GridColumn[] { this.colSupplyOrderAmount, this.colSupplyOrderArrived, this.colSupplyOrderWait })
            {
                col.Visible = hasOrder;
                if (hasOrder)
                {
                    col.VisibleIndex = index++;
                }
            }
            this.gridControlImpMestDetail.RefreshDataSource();
            this.ReloadTypeTreeBySupplyOrder();
        }

        /// <summary>
        /// Dang chon don: danh sach thuoc / vat tu ben trai chi con mat hang co trong don (4.1.1). Chua chon don thi giu nguyen danh sach
        /// </summary>
        private List<MedicineTypeADO> FilterMedicineTypeBySupplyOrder(List<MedicineTypeADO> source)
        {
            if (source == null || !this.GetSupplyOrderId().HasValue || this.dicSupplyOrderDetail.Count == 0)
            {
                return source;
            }
            return source.Where(o => this.dicSupplyOrderDetail.ContainsKey(SupplyOrderDetailKey(false, o.ID))).ToList();
        }

        private List<MaterialTypeADO> FilterMaterialTypeBySupplyOrder(List<MaterialTypeADO> source)
        {
            if (source == null || !this.GetSupplyOrderId().HasValue || this.dicSupplyOrderDetail.Count == 0)
            {
                return source;
            }
            return source.Where(o => this.dicSupplyOrderDetail.ContainsKey(SupplyOrderDetailKey(true, o.ID))).ToList();
        }

        /// <summary>
        /// Nap lai 2 cay mat hang khi doi don dang chon (chon / bo chon / doi don khac)
        /// </summary>
        private void ReloadTypeTreeBySupplyOrder()
        {
            try
            {
                long? orderId = this.GetSupplyOrderId();
                if (orderId == this.supplyOrderTreeFilterId)
                {
                    return;
                }
                this.supplyOrderTreeFilterId = orderId;
                if (this.medicineProcessor != null && this.ucMedicineTypeTree != null && this.listMedicineType != null)
                {
                    this.medicineProcessor.Reload(this.ucMedicineTypeTree, this.FilterMedicineTypeBySupplyOrder(this.listMedicineType));
                }
                if (this.materialProcessor != null && this.ucMaterialTypeTree != null && this.listMaterialType != null)
                {
                    this.materialProcessor.Reload(this.ucMaterialTypeTree, this.FilterMaterialTypeBySupplyOrder(this.listMaterialType));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// He so quy doi don vi nhap -> DVT cua loai thuoc/vat tu; loai khong co don vi nhap tra 1
        /// </summary>
        private static decimal GetImpUnitConvertRatio(VHisServiceADO ado)
        {
            decimal? ratio = null;
            if (ado.IsMedicine)
            {
                var type = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == ado.MEDI_MATE_ID);
                if (type != null && type.IMP_UNIT_ID.HasValue)
                {
                    ratio = type.IMP_UNIT_CONVERT_RATIO;
                }
            }
            else
            {
                var type = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == ado.MEDI_MATE_ID);
                if (type != null && type.IMP_UNIT_ID.HasValue)
                {
                    ratio = type.IMP_UNIT_CONVERT_RATIO;
                }
            }
            return (ratio.HasValue && ratio.Value > 0) ? ratio.Value : 1;
        }

        private static string SupplyOrderDetailKey(bool isMaterial, long typeId)
        {
            return (isMaterial ? "MA_" : "ME_") + typeId;
        }

        private HisSupplyOrderDetailSDO GetSupplyOrderDetail(VHisServiceADO row)
        {
            HisSupplyOrderDetailSDO detail;
            if (row == null || !this.dicSupplyOrderDetail.TryGetValue(SupplyOrderDetailKey(!row.IsMedicine, row.MEDI_MATE_ID), out detail))
            {
                return null;
            }
            return detail;
        }

        private void gridViewImpMestDetail_SupplyOrderUnboundData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (!e.IsGetData || this.colSupplyOrderAmount == null
                    || (e.Column != this.colSupplyOrderAmount && e.Column != this.colSupplyOrderArrived && e.Column != this.colSupplyOrderWait))
                {
                    return;
                }
                HisSupplyOrderDetailSDO detail = this.GetSupplyOrderDetail(e.Row as VHisServiceADO);
                if (detail == null)
                {
                    e.Value = null;
                }
                else if (e.Column == this.colSupplyOrderAmount)
                {
                    e.Value = detail.AMOUNT;
                }
                else if (e.Column == this.colSupplyOrderArrived)
                {
                    e.Value = detail.ARRIVED_AMOUNT;
                }
                else
                {
                    // ARRIVED_AMOUNT chi gom phieu Yeu cau/Da duyet, phieu Thuc nhap nam o IMPORTED_AMOUNT
                    e.Value = Math.Max(detail.AMOUNT - detail.ARRIVED_AMOUNT - detail.IMPORTED_AMOUNT, 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Da ve + dang nhap > luong dat, hoac mat hang khong co trong don: canh bao Co/Khong, khong chan (Q10)
        /// </summary>
        private bool CheckSupplyOrderOverAmount()
        {
            try
            {
                long? orderId = this.GetSupplyOrderId();
                if (!orderId.HasValue || this.listServiceADO == null || this.listServiceADO.Count == 0)
                {
                    return true;
                }
                StringBuilder over = new StringBuilder();
                List<string> outside = new List<string>();
                foreach (var g in this.listServiceADO.GroupBy(o => SupplyOrderDetailKey(!o.IsMedicine, o.MEDI_MATE_ID)))
                {
                    VHisServiceADO first = g.First();
                    HisSupplyOrderDetailSDO detail;
                    if (!this.dicSupplyOrderDetail.TryGetValue(g.Key, out detail))
                    {
                        outside.Add(first.MEDI_MATE_CODE + " - " + first.MEDI_MATE_NAME);
                        continue;
                    }
                    // Mat hang co don vi nhap: so luong nhap tren man theo don vi nhap, don dat theo DVT -> quy doi truoc khi so
                    decimal importing = g.Sum(o => o.IMP_AMOUNT) * GetImpUnitConvertRatio(first);
                    if (detail.ARRIVED_AMOUNT + detail.IMPORTED_AMOUNT + importing > detail.AMOUNT)
                    {
                        over.AppendLine(string.Format(" - {0} - {1}: đặt {2}, đã về {3}, đã nhập {5}, nhập lần này {4}",
                            first.MEDI_MATE_CODE, first.MEDI_MATE_NAME,
                            detail.AMOUNT.ToString(SUPPLY__FORMAT_AMOUNT), detail.ARRIVED_AMOUNT.ToString(SUPPLY__FORMAT_AMOUNT), importing.ToString(SUPPLY__FORMAT_AMOUNT), detail.IMPORTED_AMOUNT.ToString(SUPPLY__FORMAT_AMOUNT)));
                    }
                }
                if (over.Length == 0 && outside.Count == 0)
                {
                    return true;
                }
                V_HIS_SUPPLY_ORDER order = this.supplyOrders.FirstOrDefault(o => o.ID == orderId.Value);
                StringBuilder message = new StringBuilder();
                if (over.Length > 0)
                {
                    message.AppendLine(string.Format("Phiếu nhập vượt lượng đặt của đơn {0}:", order != null ? order.SUPPLY_ORDER_CODE : ""));
                    message.Append(over.ToString());
                }
                if (outside.Count > 0)
                {
                    message.AppendLine("Mặt hàng không có trong đơn đặt hàng:");
                    foreach (string s in outside)
                    {
                        message.AppendLine(" - " + s);
                    }
                }
                message.AppendLine();
                message.Append("Bạn có muốn tiếp tục lưu?");
                return DevExpress.XtraEditors.XtraMessageBox.Show(message.ToString(), "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return true;
        }

        private void ResetSupplyOrder()
        {
            try
            {
                if (this.cboSupplyOrder == null)
                {
                    return;
                }
                this.isSupplyOrderBinding = true;
                this.cboSupplyOrder.EditValue = null;
                this.txtSupplyReplaceCode.Text = "";
                this.isSupplyOrderBinding = false;
                this.supplyOrderLoadedKey = null;
                this.ReloadSupplyOrders();
            }
            catch (Exception ex)
            {
                this.isSupplyOrderBinding = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
