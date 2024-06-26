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
using DevExpress.XtraEditors.Repository;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.RegisterVaccination.FormEdit
{
    public partial class frmMedicine : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;

        V_HIS_EXP_MEST_MEDICINE currentMedicine { get; set; }

        long _VaccinId = 0;

        public frmMedicine()
        {
            InitializeComponent();
        }

        public frmMedicine(Inventec.Desktop.Common.Modules.Module _Module, long _vaccinId)
        {
            InitializeComponent();
            try
            {
                this.currentModule = _Module;
                this._VaccinId = _vaccinId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmMedicine_Load(object sender, EventArgs e)
        {
            try
            {
                SetIconFrm();

                LoadDataComboKetQuaTiem();

                LoadDataToGridExpMestMedicine();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetIconFrm()
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

        private void LoadDataToGridExpMestMedicine()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExpMestMedicineViewFilter hisExpMestMedicineViewFilter = new HisExpMestMedicineViewFilter();
                hisExpMestMedicineViewFilter.TDL_VACCINATION_ID = this._VaccinId;

                var rsApi = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumer.ApiConsumers.MosConsumer, hisExpMestMedicineViewFilter, param);

                if (rsApi != null && rsApi.Count > 0)
                {
                    LoadComboLoGrid(rsApi.Select(p => p.MEDI_STOCK_ID).Distinct().ToList());
                }
                gridControlMedicine.BeginUpdate();
                gridControlMedicine.DataSource = rsApi;
                gridControlMedicine.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMedicine_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                var row = (V_HIS_EXP_MEST_MEDICINE)gridViewMedicine.GetRow(e.RowHandle);
                if (row != null)
                {
                    if (e.Column.FieldName == "VACCINATION_RESULT_ID")
                    {
                        e.RepositoryItem = cboKetQuaTiem_Enable;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void gridViewMedicine_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                currentMedicine = (V_HIS_EXP_MEST_MEDICINE)gridViewMedicine.GetFocusedRow();
                if (currentMedicine != null)
                {
                    LoadComboLoThuoc();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataComboKetQuaTiem()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisVaccinationResultFilter hisVaccinationResultFilter = new HisVaccinationResultFilter();
                hisVaccinationResultFilter.IS_ACTIVE = 1;
                var vaccinationResult = new BackendAdapter(param).Get<List<HIS_VACCINATION_RESULT>>("api/HisVaccinationResult/Get", ApiConsumer.ApiConsumers.MosConsumer, hisVaccinationResultFilter, param);

                InitCombo(cboKetQuaTiem_Enable, "VACCINATION_RESULT_NAME", "ID", "VACCINATION_RESULT_CODE", vaccinationResult);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboLoGrid(List<long> _mediStockIds)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisMedicineStockViewFilter medicineFilter = new HisMedicineStockViewFilter();
                medicineFilter.IS_LEAF = 1;
                medicineFilter.MEDI_STOCK_IDs = _mediStockIds;
                medicineFilter.MEDICINE_TYPE_IS_ACTIVE = true;
                medicineFilter.ORDER_DIRECTION = "ASC";
                medicineFilter.ORDER_FIELD = "MEDICINE_TYPE_NAME";

                var medicine = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HisMedicineInStockSDO>>("/api/HisMedicine/GetInStockMedicine", ApiConsumers.MosConsumer, medicineFilter, param);

                if (medicine != null && medicine.Count > 0)
                {
                    var availableMedicine = medicine.Where(o => (o.AvailableAmount != null && o.AvailableAmount > 0)).ToList();
                    InitCombo(cboLoVaccine_Enable, "PACKAGE_NUMBER", "ID", "AvailableAmount", medicine);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboLoThuoc()
        {
            try
            {
                if (this.currentMedicine != null)
                {
                    CommonParam param = new CommonParam();
                    HisMedicineStockViewFilter medicineFilter = new HisMedicineStockViewFilter();
                    medicineFilter.MEDICINE_TYPE_ID = currentMedicine.MEDICINE_TYPE_ID;
                    medicineFilter.MEDI_STOCK_ID = this.currentMedicine.MEDI_STOCK_ID;
                    medicineFilter.IS_LEAF = 1;
                    medicineFilter.MEDICINE_TYPE_IS_ACTIVE = true;
                    medicineFilter.ORDER_DIRECTION = "ASC";
                    medicineFilter.ORDER_FIELD = "MEDICINE_TYPE_NAME";

                    var medicine = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HisMedicineInStockSDO>>("/api/HisMedicine/GetInStockMedicine", ApiConsumers.MosConsumer, medicineFilter, param);

                    if (medicine != null && medicine.Count > 0)
                    {
                        var availableMedicine = medicine.Where(o =>
                            (o.AvailableAmount != null
                            && o.AvailableAmount > 0)
                            && o.ID != this.currentMedicine.MEDICINE_ID
                            && o.AvailableAmount >= this.currentMedicine.AMOUNT
                            && o.IMP_PRICE == this.currentMedicine.IMP_PRICE
                            && o.IMP_VAT_RATIO == this.currentMedicine.IMP_VAT_RATIO
                            ).ToList();
                        InitCombo(cboMedicine, "PACKAGE_NUMBER", "ID", "AvailableAmount", availableMedicine);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCombo(RepositoryItem control, string displayMember, string valueMember, string column1, object data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();

                if (!string.IsNullOrEmpty(column1))
                {
                    columnInfos.Add(new ColumnInfo(column1, "", 100, 1));
                }
                columnInfos.Add(new ColumnInfo(displayMember, "", 250, 2));

                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, 350);
                ControlEditorLoader.Load(control, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCombo(Control control, string displayMember, string valueMember, string column1, object data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();

                columnInfos.Add(new ColumnInfo(displayMember, "", 250, 1));
                if (!string.IsNullOrEmpty(column1))
                {
                    columnInfos.Add(new ColumnInfo(column1, "", 100, 2));
                }

                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, 350);
                ControlEditorLoader.Load(control, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboMedicine.EditValue == null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn lô", "Thông báo");
                    return;
                }
                bool success = false;
                CommonParam param = new CommonParam();
                HisVaccinationChangeMedicineSDO sdo = new HisVaccinationChangeMedicineSDO();
                sdo.ExpMestMedicineId = this.currentMedicine.ID;
                sdo.NewMedicineId = Inventec.Common.TypeConvert.Parse.ToInt64(this.cboMedicine.EditValue.ToString());
                sdo.WorkingRoomId = currentModule.RoomId;

                var rsApi = new BackendAdapter(param).Post<HIS_EXP_MEST_MEDICINE>("api/HisVaccination/ChangeMedicine", ApiConsumers.MosConsumer, sdo, param);
                if (rsApi != null)
                {
                    success = true;
                    LoadDataToGridExpMestMedicine();
                }
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

    }
}
