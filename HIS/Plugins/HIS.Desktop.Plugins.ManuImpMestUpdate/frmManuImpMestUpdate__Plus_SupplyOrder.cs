using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ManuImpMestUpdate.ADO;
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

namespace HIS.Desktop.Plugins.ManuImpMestUpdate
{
    /// <summary>
    /// Viec 39336 - Cung ung hang hoa: xem / doi don dat hang doi chieu cua phieu nhap NCC khi phieu con sua duoc.
    /// Chi hien khi khoa MOS.HIS_ANTICIPATE.SUPPLY.MEDI_STOCK_CODES co gia tri va kho nhap thuoc danh sach;
    /// truong khong hien thi thi luu giu nguyen don da gan cua phieu.
    /// </summary>
    public partial class frmManuImpMestUpdate
    {
        private const string SUPPLY__CONFIG_KEY_MEDI_STOCK_CODES = "MOS.HIS_ANTICIPATE.SUPPLY.MEDI_STOCK_CODES";
        private const short SUPPLY__ORDER_STT_CONFIRMED = 2;
        private const string SUPPLY__FORMAT_AMOUNT = "#,##0.####";

        private LookUpEdit cboSupplyOrder;
        private TextEdit txtSupplyReplaceCode;
        private LayoutControlItem lciSupplyOrder;
        private LayoutControlItem lciSupplyReplaceCode;

        private bool IsSupplyOrderActive
        {
            get { return this.lciSupplyOrder != null && this.lciSupplyOrder.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always; }
        }

        private void SetSupplyOrderToImpMest(HIS_IMP_MEST impMest)
        {
            if (impMest == null || !this.IsSupplyOrderActive)
            {
                return;
            }
            impMest.SUPPLY_ORDER_ID = this.cboSupplyOrder.EditValue != null ? (long?)Convert.ToInt64(this.cboSupplyOrder.EditValue) : null;
            impMest.SUPPLY_REPLACE_IMP_MEST_CODE = impMest.SUPPLY_ORDER_ID.HasValue && !String.IsNullOrWhiteSpace(this.txtSupplyReplaceCode.Text)
                ? this.txtSupplyReplaceCode.Text.Trim()
                : null;
        }

        private void InitSupplyOrder()
        {
            try
            {
                if (this.cboSupplyOrder != null || this.currentImpMest == null
                    || this.currentImpMest.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC
                    || !this.currentImpMest.SUPPLIER_ID.HasValue)
                {
                    return;
                }
                string value = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SUPPLY__CONFIG_KEY_MEDI_STOCK_CODES);
                List<string> codes = String.IsNullOrWhiteSpace(value)
                    ? new List<string>()
                    : value.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(o => o.Trim()).Where(o => o != "").Distinct().ToList();
                V_HIS_MEDI_STOCK stock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == this.currentImpMest.MEDI_STOCK_ID);
                if (codes.Count == 0 || stock == null || !codes.Contains(stock.MEDI_STOCK_CODE))
                {
                    return;
                }

                CommonParam param = new CommonParam();
                HisSupplyOrderViewFilter filter = new HisSupplyOrderViewFilter();
                filter.MEDI_STOCK_ID = this.currentImpMest.MEDI_STOCK_ID;
                filter.SUPPLIER_ID = this.currentImpMest.SUPPLIER_ID.Value;
                filter.SUPPLY_ORDER_STT = SUPPLY__ORDER_STT_CONFIRMED;
                filter.ORDER_FIELD = "ORDER_TIME";
                filter.ORDER_DIRECTION = "DESC";
                List<V_HIS_SUPPLY_ORDER> orders = new BackendAdapter(param).Get<List<V_HIS_SUPPLY_ORDER>>("api/HisSupplyOrder/GetView", ApiConsumers.MosConsumer, filter, param) ?? new List<V_HIS_SUPPLY_ORDER>();
                long? linkedId = this.currentImpMest.SUPPLY_ORDER_ID;
                if (linkedId.HasValue && !orders.Any(o => o.ID == linkedId.Value))
                {
                    HisSupplyOrderViewFilter idFilter = new HisSupplyOrderViewFilter();
                    idFilter.ID = linkedId.Value;
                    List<V_HIS_SUPPLY_ORDER> linked = new BackendAdapter(param).Get<List<V_HIS_SUPPLY_ORDER>>("api/HisSupplyOrder/GetView", ApiConsumers.MosConsumer, idFilter, param);
                    if (linked != null)
                    {
                        orders.AddRange(linked);
                    }
                }

                this.cboSupplyOrder = new LookUpEdit();
                this.cboSupplyOrder.Name = "cboSupplyOrder";
                this.cboSupplyOrder.Properties.ValueMember = "ID";
                this.cboSupplyOrder.Properties.DisplayMember = "SUPPLY_ORDER_CODE";
                this.cboSupplyOrder.Properties.NullText = "";
                this.cboSupplyOrder.Properties.PopupWidth = 420;
                this.cboSupplyOrder.Properties.Columns.Add(new LookUpColumnInfo("SUPPLY_ORDER_CODE", "Số đơn", 100));
                this.cboSupplyOrder.Properties.Columns.Add(new LookUpColumnInfo("ORDER_TIME_DISPLAY", "Ngày đặt", 120));
                LookUpColumnInfo colTotal = new LookUpColumnInfo("TOTAL_PRICE", "Thành tiền", 110);
                colTotal.FormatType = DevExpress.Utils.FormatType.Numeric;
                colTotal.FormatString = "#,##0.##";
                this.cboSupplyOrder.Properties.Columns.Add(colTotal);
                this.cboSupplyOrder.Properties.Columns.Add(new LookUpColumnInfo("STATUS_DISPLAY", "Trạng thái", 90));
                this.cboSupplyOrder.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Delete));
                this.cboSupplyOrder.ToolTip = "Đơn đặt hàng đã xác nhận đặt của đúng kho và nhà cung cấp";
                this.cboSupplyOrder.Properties.DataSource = orders.Select(o => new
                {
                    o.ID,
                    o.SUPPLY_ORDER_CODE,
                    ORDER_TIME_DISPLAY = o.ORDER_TIME.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(o.ORDER_TIME.Value) : "",
                    o.TOTAL_PRICE,
                    STATUS_DISPLAY = o.SUPPLY_ORDER_STT == SUPPLY__ORDER_STT_CONFIRMED ? "Đã xác nhận đặt" : "Đã hủy"
                }).ToList();
                this.cboSupplyOrder.EditValue = linkedId.HasValue && orders.Any(o => o.ID == linkedId.Value) ? (object)linkedId.Value : null;
                this.cboSupplyOrder.ButtonClick += (s, e) =>
                {
                    if (e.Button.Kind == ButtonPredefines.Delete)
                    {
                        this.cboSupplyOrder.EditValue = null;
                    }
                };
                this.cboSupplyOrder.EditValueChanged += (s, e) =>
                {
                    bool hasOrder = this.cboSupplyOrder.EditValue != null;
                    if (!hasOrder)
                    {
                        this.txtSupplyReplaceCode.Text = "";
                    }
                    this.lciSupplyReplaceCode.Visibility = hasOrder ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                };

                this.txtSupplyReplaceCode = new TextEdit();
                this.txtSupplyReplaceCode.Name = "txtSupplyReplaceCode";
                this.txtSupplyReplaceCode.Properties.MaxLength = 50;
                this.txtSupplyReplaceCode.ToolTip = "Mã phiếu nhập đã hủy của cùng đơn mà phiếu này thay thế (nếu có)";
                this.txtSupplyReplaceCode.Text = this.currentImpMest.SUPPLY_REPLACE_IMP_MEST_CODE;

                // dong moi ngay tren luoi chi tiet, ca chieu ngang cua luoi
                this.layoutControl1.BeginUpdate();
                try
                {
                    this.lciSupplyOrder = this.layoutControlGroup1.AddItem("Đơn đặt hàng:", this.cboSupplyOrder, this.layoutControlItem1, DevExpress.XtraLayout.Utils.InsertType.Top);
                    this.lciSupplyReplaceCode = this.layoutControlGroup1.AddItem("Thay phiếu:", this.txtSupplyReplaceCode, this.lciSupplyOrder, DevExpress.XtraLayout.Utils.InsertType.Right);
                    foreach (LayoutControlItem lci in new LayoutControlItem[] { this.lciSupplyOrder, this.lciSupplyReplaceCode })
                    {
                        lci.SizeConstraintsType = SizeConstraintsType.Custom;
                        lci.MinSize = new System.Drawing.Size(120, 24);
                        lci.MaxSize = new System.Drawing.Size(0, 24);
                        lci.TextAlignMode = TextAlignModeItem.AutoSize;
                    }
                    this.lciSupplyOrder.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    this.lciSupplyOrder.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.lciSupplyReplaceCode.Visibility = this.cboSupplyOrder.EditValue != null ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                finally
                {
                    this.layoutControl1.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Phieu khac da ve + da nhap + so luong tren phieu nay > luong dat, hoac mat hang khong co trong don:
        /// canh bao Co/Khong, khong chan (Q10). Da ve cua may chu da gom chinh phieu nay neu phieu dang o Yeu cau/Da duyet va gan cung don
        /// </summary>
        private bool CheckSupplyOrderOverAmount()
        {
            try
            {
                if (!this.IsSupplyOrderActive || this.cboSupplyOrder.EditValue == null || this.listServiceADO == null || this.listServiceADO.Count == 0)
                {
                    return true;
                }
                long orderId = Convert.ToInt64(this.cboSupplyOrder.EditValue);
                CommonParam param = new CommonParam();
                HisSupplyOrderDetailFilter filter = new HisSupplyOrderDetailFilter();
                filter.SUPPLY_ORDER_ID = orderId;
                List<HisSupplyOrderDetailSDO> details = new BackendAdapter(param).Get<List<HisSupplyOrderDetailSDO>>("api/HisSupplyOrderDetail/Get", ApiConsumers.MosConsumer, filter, param);
                if (details == null)
                {
                    return true;
                }
                Dictionary<string, HisSupplyOrderDetailSDO> dicDetail = details.GroupBy(o => SupplyOrderDetailKey(o.IS_MATERIAL, o.TYPE_ID)).ToDictionary(g => g.Key, g => g.First());
                Dictionary<string, decimal> ownAmounts = this.GetOwnArrivedAmounts(orderId);

                StringBuilder over = new StringBuilder();
                List<string> outside = new List<string>();
                foreach (var g in this.listServiceADO.GroupBy(o => SupplyOrderDetailKey(!o.IsMedicine, o.MEDI_MATE_ID)))
                {
                    VHisServiceADO first = g.First();
                    HisSupplyOrderDetailSDO detail;
                    if (!dicDetail.TryGetValue(g.Key, out detail))
                    {
                        outside.Add(first.MEDI_MATE_CODE + " - " + first.MEDI_MATE_NAME);
                        continue;
                    }
                    decimal own;
                    ownAmounts.TryGetValue(g.Key, out own);
                    decimal arrivedOther = Math.Max(detail.ARRIVED_AMOUNT - own, 0);
                    decimal importing = g.Sum(o => o.IMP_AMOUNT);
                    if (arrivedOther + detail.IMPORTED_AMOUNT + importing > detail.AMOUNT)
                    {
                        over.AppendLine(string.Format(" - {0} - {1}: đặt {2}, phiếu khác đã về {3}, đã nhập {4}, phiếu này {5}",
                            first.MEDI_MATE_CODE, first.MEDI_MATE_NAME,
                            detail.AMOUNT.ToString(SUPPLY__FORMAT_AMOUNT), arrivedOther.ToString(SUPPLY__FORMAT_AMOUNT),
                            detail.IMPORTED_AMOUNT.ToString(SUPPLY__FORMAT_AMOUNT), importing.ToString(SUPPLY__FORMAT_AMOUNT)));
                    }
                }
                if (over.Length == 0 && outside.Count == 0)
                {
                    return true;
                }
                StringBuilder message = new StringBuilder();
                if (over.Length > 0)
                {
                    message.AppendLine(string.Format("Phiếu nhập vượt lượng đặt của đơn {0}:", this.cboSupplyOrder.Text));
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

        /// <summary>
        /// Luong goc cua chinh phieu nay da duoc may chu tinh vao "Da ve" cua don (phieu o Yeu cau / Da duyet va dang gan don do)
        /// </summary>
        private Dictionary<string, decimal> GetOwnArrivedAmounts(long orderId)
        {
            Dictionary<string, decimal> result = new Dictionary<string, decimal>();
            if (this.currentImpMest == null || this.currentImpMest.SUPPLY_ORDER_ID != orderId
                || (this.currentImpMest.IMP_MEST_STT_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST
                    && this.currentImpMest.IMP_MEST_STT_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL))
            {
                return result;
            }
            CommonParam param = new CommonParam();
            HisImpMestMedicineViewFilter mediFilter = new HisImpMestMedicineViewFilter();
            mediFilter.IMP_MEST_ID = this.currentImpMest.ID;
            List<V_HIS_IMP_MEST_MEDICINE> medicines = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, param);
            if (medicines != null)
            {
                foreach (var g in medicines.GroupBy(o => o.MEDICINE_TYPE_ID))
                {
                    result[SupplyOrderDetailKey(false, g.Key)] = g.Sum(o => o.AMOUNT);
                }
            }
            HisImpMestMaterialViewFilter mateFilter = new HisImpMestMaterialViewFilter();
            mateFilter.IMP_MEST_ID = this.currentImpMest.ID;
            List<V_HIS_IMP_MEST_MATERIAL> materials = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, mateFilter, param);
            if (materials != null)
            {
                foreach (var g in materials.GroupBy(o => o.MATERIAL_TYPE_ID))
                {
                    result[SupplyOrderDetailKey(true, g.Key)] = g.Sum(o => o.AMOUNT);
                }
            }
            return result;
        }

        private static string SupplyOrderDetailKey(bool isMaterial, long typeId)
        {
            return (isMaterial ? "MA_" : "ME_") + typeId;
        }
    }
}
