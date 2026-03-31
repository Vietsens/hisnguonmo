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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.ApproveExpMestBCS.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ApproveExpMestBCS
{
    public partial class frmSelectLo : DevExpress.XtraEditors.XtraForm
    {
        private MedicineTypeADO medicineAdo;
        private MaterialTypeADO materialAdo;
        private long mediStockId;
        private bool isMedicine;

        private List<MedicineBeanADO> medicineBeans = new List<MedicineBeanADO>();
        private List<MaterialBeanADO> materialBeans = new List<MaterialBeanADO>();

        public frmSelectLo(MedicineTypeADO ado, long mediStockId)
        {
            InitializeComponent();
            this.medicineAdo = ado;
            this.mediStockId = mediStockId;
            this.isMedicine = true;
        }

        public frmSelectLo(MaterialTypeADO ado, long mediStockId)
        {
            InitializeComponent();
            this.materialAdo = ado;
            this.mediStockId = mediStockId;
            this.isMedicine = false;
        }

        private void frmSelectLo_Load(object sender, EventArgs e)
        {
            try
            {
                this.KeyPreview = true;
                this.KeyDown += frmSelectLo_KeyDown;

                SetupMedicineGrid();
                SetupMaterialGrid();

                if (isMedicine)
                {
                    tabThuoc.SelectedTab = tabPage1;
                    LoadMedicineBeans();
                }
                else
                {
                    tabThuoc.SelectedTab = tabPage2;
                    LoadMaterialBeans();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetupMedicineGrid()
        {
            gridControlMedicine.Dock = DockStyle.Fill;
            gridViewMedicine.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;

            var repoCheck = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            repoCheck.AutoHeight = false;
            repoCheck.ValueChecked = true;
            repoCheck.ValueUnchecked = false;
            repoCheck.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            gridControlMedicine.RepositoryItems.Add(repoCheck);

            // QUAN TRỌNG: Designer đặt UnboundType = Object → phải reset về Bound để bind đúng vào MedicineBeanADO.IsCheck
            gridColumn1.UnboundType = DevExpress.Data.UnboundColumnType.Bound;
            gridColumn1.Caption = " ";
            gridColumn1.FieldName = "IsCheck";
            gridColumn1.Width = 40;
            gridColumn1.ColumnEdit = repoCheck;
            gridColumn1.OptionsColumn.AllowEdit = true;
            gridColumn1.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            gridColumn1.OptionsColumn.AllowMove = false;

            gridColumn2.Caption = "Mã thuốc";
            gridColumn2.FieldName = "MEDICINE_TYPE_CODE";
            gridColumn2.Width = 100;
            gridColumn2.OptionsColumn.AllowEdit = false;

            gridColumn3.Caption = "Tên thuốc";
            gridColumn3.FieldName = "MEDICINE_TYPE_NAME";
            gridColumn3.Width = 200;
            gridColumn3.OptionsColumn.AllowEdit = false;

            gridColumn4.Caption = "Số lô";
            gridColumn4.FieldName = "PACKAGE_NUMBER";
            gridColumn4.Width = 120;
            gridColumn4.OptionsColumn.AllowEdit = false;

            gridColumn5.Caption = "Hạn sử dụng";
            gridColumn5.FieldName = "EXPIRED_DATE_STR";
            gridColumn5.UnboundType = DevExpress.Data.UnboundColumnType.String;
            gridColumn5.Width = 90;
            gridColumn5.OptionsColumn.AllowEdit = false;

            gridColumn6.Caption = "Khả dụng";
            gridColumn6.FieldName = "AMOUNT";
            gridColumn6.Width = 90;
            gridColumn6.OptionsColumn.AllowEdit = false;
            gridColumn6.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridColumn6.DisplayFormat.FormatString = "N2";

            gridColumn7.Visible = false;
            gridColumn8.Visible = false;
        }

        private void SetupMaterialGrid()
        {
            gridControlMaterial.Dock = DockStyle.Fill;
            gridViewMaterial.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;

            var repoCheck = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            repoCheck.AutoHeight = false;
            repoCheck.ValueChecked = true;
            repoCheck.ValueUnchecked = false;
            repoCheck.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            gridControlMaterial.RepositoryItems.Add(repoCheck);

            // QUAN TRỌNG: Designer đặt UnboundType = Object → phải reset về Bound để bind đúng vào MaterialBeanADO.IsCheck
            gridColumn9.UnboundType = DevExpress.Data.UnboundColumnType.Bound;
            gridColumn9.Caption = " ";
            gridColumn9.FieldName = "IsCheck";
            gridColumn9.Width = 40;
            gridColumn9.ColumnEdit = repoCheck;
            gridColumn9.OptionsColumn.AllowEdit = true;
            gridColumn9.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            gridColumn9.OptionsColumn.AllowMove = false;

            gridColumn10.Caption = "Mã vật tư";
            gridColumn10.FieldName = "MATERIAL_TYPE_CODE";
            gridColumn10.Width = 100;
            gridColumn10.OptionsColumn.AllowEdit = false;

            gridColumn11.Caption = "Tên vật tư";
            gridColumn11.FieldName = "MATERIAL_TYPE_NAME";
            gridColumn11.Width = 200;
            gridColumn11.OptionsColumn.AllowEdit = false;

            gridColumn12.Caption = "Số lô";
            gridColumn12.FieldName = "PACKAGE_NUMBER";
            gridColumn12.Width = 120;
            gridColumn12.OptionsColumn.AllowEdit = false;

            gridColumn13.Caption = "Hạn sử dụng";
            gridColumn13.FieldName = "EXPIRED_DATE_STR";
            gridColumn13.UnboundType = DevExpress.Data.UnboundColumnType.String;
            gridColumn13.Width = 90;
            gridColumn13.OptionsColumn.AllowEdit = false;

            gridColumn14.Caption = "Khả dụng";
            gridColumn14.FieldName = "AMOUNT";
            gridColumn14.Width = 90;
            gridColumn14.OptionsColumn.AllowEdit = false;
            gridColumn14.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridColumn14.DisplayFormat.FormatString = "N2";

            gridColumn15.Visible = false;
            gridColumn16.Visible = false;
        }

        private void LoadMedicineBeans()
        {
            try
            {
                LogSystem.Info("Input api/HisMedicineBean/GetView: " + LogUtil.TraceData("data", medicineAdo));
                HisMedicineBeanViewFilter filter = new HisMedicineBeanViewFilter();
                filter.MEDI_STOCK_ID = this.mediStockId;

                var raw = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDICINE_BEAN>>(
                    "api/HisMedicineBean/GetView", ApiConsumers.MosConsumer, filter, null);
                long nowDate = 0;
                var now = Inventec.Common.DateTime.Get.Now();
                if (now.HasValue)
                {
                    nowDate = long.Parse(now.Value.ToString().Substring(0, 8));
                }

                //medicineBeans = (raw ?? new List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_BEAN>())
                //    .Where(b =>
                //    (b.MEDI_STOCK_ID ?? 0) == this.mediStockId
                //    && b.MEDICINE_TYPE_ID == this.medicineAdo.MEDICINE_TYPE_ID
                //    && b.AMOUNT > 0
                //    && (b.EXPIRED_DATE.Value >= nowDate))
                //    .OrderBy(b => b.EXPIRED_DATE ?? long.MaxValue)
                //    .ThenBy(b => b.PACKAGE_NUMBER)
                //    .Select(b => new MedicineBeanADO
                //    {
                //        ID = b.ID,
                //        PACKAGE_NUMBER = b.PACKAGE_NUMBER,  
                //        EXPIRED_DATE = b.EXPIRED_DATE,
                //        AMOUNT = b.AMOUNT,
                //        MEDI_STOCK_ID = b.MEDI_STOCK_ID ?? 0,
                //        MEDICINE_TYPE_CODE = this.medicineAdo.MEDICINE_TYPE_CODE,
                //        MEDICINE_TYPE_NAME = this.medicineAdo.MEDICINE_TYPE_NAME
                //    }).ToList();
                medicineBeans = (raw ?? new List<V_HIS_MEDICINE_BEAN>())
                    .Where(x =>
                        (x.MEDI_STOCK_ID ?? 0) == this.mediStockId
                        && x.MEDICINE_TYPE_ID == this.medicineAdo.MEDICINE_TYPE_ID
                        && x.AMOUNT > 0
                        && (!x.EXPIRED_DATE.HasValue || long.Parse(x.EXPIRED_DATE.Value.ToString().Substring(0, 8)) > nowDate)
                        && x.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .OrderBy(x => x.EXPIRED_DATE ?? long.MaxValue)
                    .ThenBy(x => x.PACKAGE_NUMBER)
                    .Select(x => new MedicineBeanADO
                    {
                        ID = x.ID,
                        MEDICINE_ID = x.MEDICINE_ID,
                        MEDICINE_TYPE_ID = this.medicineAdo.MEDICINE_TYPE_ID,
                        MEDICINE_TYPE_CODE = this.medicineAdo.MEDICINE_TYPE_CODE,
                        MEDICINE_TYPE_NAME = this.medicineAdo.MEDICINE_TYPE_NAME,
                        PACKAGE_NUMBER = x.PACKAGE_NUMBER,
                        EXPIRED_DATE = x.EXPIRED_DATE,
                        AMOUNT = x.AMOUNT,
                        MEDI_STOCK_ID = x.MEDI_STOCK_ID ?? 0
                    }).ToList();

                if (medicineAdo.SelectedBeans != null)
                {
                    foreach (var bean in medicineBeans)
                        bean.IsCheck = medicineAdo.SelectedBeans.Any(s => s.ID == bean.ID);
                }

                gridControlMedicine.DataSource = medicineBeans;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadMaterialBeans()
        {
            try
            {
                HisMaterialBeanViewFilter filter = new HisMaterialBeanViewFilter();
                filter.MEDI_STOCK_ID = this.mediStockId;

                var raw = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MATERIAL_BEAN>>(
                    "api/HisMaterialBean/GetView", ApiConsumers.MosConsumer, filter, null);
                long nowDate = 0;
                var now = Inventec.Common.DateTime.Get.Now();
                if (now.HasValue)
                {
                    nowDate = long.Parse(now.Value.ToString().Substring(0, 8));
                }

                materialBeans = (raw ?? new List<V_HIS_MATERIAL_BEAN>())
                    .Where(x =>
                        (x.MEDI_STOCK_ID ?? 0) == this.mediStockId
                        && x.MATERIAL_TYPE_ID == this.materialAdo.MATERIAL_TYPE_ID
                        && x.AMOUNT > 0
                        && (!x.EXPIRED_DATE.HasValue || long.Parse(x.EXPIRED_DATE.Value.ToString().Substring(0, 8)) > nowDate)
                        && x.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .OrderBy(b => b.EXPIRED_DATE ?? long.MaxValue)
                    .ThenBy(b => b.PACKAGE_NUMBER)
                    .Select(b => new MaterialBeanADO
                    {
                        ID = b.ID,
                        MATERIAL_ID = b.MATERIAL_ID,
                        MATERIAL_TYPE_ID = this.materialAdo.MATERIAL_TYPE_ID,
                        MATERIAL_TYPE_CODE = this.materialAdo.MATERIAL_TYPE_CODE,
                        MATERIAL_TYPE_NAME = this.materialAdo.MATERIAL_TYPE_NAME,
                        PACKAGE_NUMBER = b.PACKAGE_NUMBER,
                        EXPIRED_DATE = b.EXPIRED_DATE,
                        AMOUNT = b.AMOUNT,
                        MEDI_STOCK_ID = b.MEDI_STOCK_ID ?? 0
                    }).ToList();

                if (materialAdo.SelectedBeans != null)
                {
                    foreach (var bean in materialBeans)
                        bean.IsCheck = materialAdo.SelectedBeans.Any(s => s.ID == bean.ID);
                }

                gridControlMaterial.DataSource = materialBeans;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmSelectLo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
                Save();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void Save()
        {
            try
            {
                if (isMedicine)
                {
                    gridViewMedicine.PostEditor();

                    var selected = medicineBeans
                        .Where(b => b.IsCheck)
                        .GroupBy(b => b.ID)
                        .Select(g => g.First())
                        .ToList();

                    if (selected.Count == 0)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Vui lòng chọn ít nhất một số lô", "Thông báo");
                        return;
                    }

                    medicineAdo.SelectedBeans = selected;
                    medicineAdo.AVAIL_AMOUNT = selected.Sum(b => b.AMOUNT);
                }
                else
                {
                    gridViewMaterial.PostEditor();

                    var selected = materialBeans
                        .Where(b => b.IsCheck)
                        .GroupBy(b => b.ID)
                        .Select(g => g.First())
                        .ToList();

                    if (selected.Count == 0)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Vui lòng chọn ít nhất một số lô", "Thông báo");
                        return;
                    }

                    materialAdo.SelectedBeans = selected;
                    materialAdo.AVAIL_AMOUNT = selected.Sum(b => b.AMOUNT);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            if (e.IsGetData && e.Column.FieldName == "EXPIRED_DATE_STR")
            {
                var data = ((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex] as MedicineBeanADO;
                if (data != null)
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE ?? 0);
            }
        }

        private void gridViewMaterial_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            if (e.IsGetData && e.Column.FieldName == "EXPIRED_DATE_STR")
            {
                var data = ((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex] as MaterialBeanADO;
                if (data != null)
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE ?? 0);
            }
        }
    }
}
