using DevExpress.XtraGrid.Columns;
using System;

namespace HIS.Desktop.Plugins.HisImportMestMedicine
{
    /// <summary>
    /// Viec 39336 - Cung ung hang hoa: cot "Don dat hang" va "Thay phieu" (V_HIS_IMP_MEST), chi hien khi khoa
    /// MOS.HIS_ANTICIPATE.SUPPLY.MEDI_STOCK_CODES co gia tri
    /// </summary>
    public partial class UCHisImportMestMedicine
    {
        private const string SUPPLY__CONFIG_KEY_MEDI_STOCK_CODES = "MOS.HIS_ANTICIPATE.SUPPLY.MEDI_STOCK_CODES";

        private void InitSupplyOrderColumns()
        {
            try
            {
                string value = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SUPPLY__CONFIG_KEY_MEDI_STOCK_CODES);
                if (String.IsNullOrWhiteSpace(value) || this.gridViewImportMestList.Columns.ColumnByFieldName("SUPPLY_ORDER_CODE") != null)
                {
                    return;
                }
                int index = this.gCImpMestCode.VisibleIndex >= 0 ? this.gCImpMestCode.VisibleIndex + 1 : this.gridViewImportMestList.VisibleColumns.Count;
                this.AddSupplyColumn("SUPPLY_ORDER_CODE", "Đơn đặt hàng", "Đơn đặt hàng đối chiếu của phiếu nhập từ nhà cung cấp", 100, index);
                this.AddSupplyColumn("SUPPLY_REPLACE_IMP_MEST_CODE", "Thay phiếu", "Mã phiếu nhập đã hủy mà phiếu này thay thế", 100, index + 1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AddSupplyColumn(string fieldName, string caption, string toolTip, int width, int visibleIndex)
        {
            GridColumn col = new GridColumn();
            col.FieldName = fieldName;
            col.Caption = caption;
            col.ToolTip = toolTip;
            col.Width = width;
            col.OptionsColumn.AllowEdit = false;
            this.gridViewImportMestList.Columns.Add(col);
            col.VisibleIndex = visibleIndex;
        }
    }
}
