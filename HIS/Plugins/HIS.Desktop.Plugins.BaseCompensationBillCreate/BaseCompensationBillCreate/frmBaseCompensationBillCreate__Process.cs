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
using AutoMapper;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Controls;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.SdaConfigKey.Config;
using HIS.Desktop.Plugins.BaseCompensationBillCreate.ADO;
using HIS.Desktop.Plugins.BaseCompensationBillCreate.Resources;
using HIS.Desktop.Plugins.BaseCompensationBillCreate.ValidationRules;
using Inventec.Common.Adapter;
using Inventec.Core;
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

namespace HIS.Desktop.Plugins.BaseCompensationBillCreate
{
    public partial class frmBaseCompensationBillCreate : HIS.Desktop.Utility.FormBase
    {
        void RefeshDataAfterSaveToDbMedicine()
        {
            try
            {
                gridControlDetailMedicineExpView.DataSource = null;

                List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE> listDataMedicine = (List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>)
                    dataResult.ExpMedicines;
                gridControlDetailMedicineExpView.DataSource = listDataMedicine;
                LoadDataToGridMedicineTypeInStock();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void LoadDataToGridMedicineTypeInStock()
        {
            CommonParam param = new CommonParam();
            try
            {
                gridControlMedicineType.BeginUpdate();
                MOS.Filter.HisMedicineTypeStockViewFilter filter = new MOS.Filter.HisMedicineTypeStockViewFilter();
                filter.IS_LEAF = true;
                //filter.KEY_WORD = txtKeyword__Medicine.Text.Trim();
                filter.MEDI_STOCK_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboMediStockExport.EditValue ?? "0").ToString());
                currentMedicineTypeInStockSDOs = new BackendAdapter(param).Get<List<MOS.SDO.HisMedicineTypeInStockSDO>>(HisRequestUriStore.HIS_MEDICINE_TYPE_IN_STOCK, ApiConsumers.MosConsumer, filter, param);
                gridControlMedicineType.DataSource = currentMedicineTypeInStockSDOs;
                gridControlMedicineType.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetControlAddPanel()
        {
            try
            {
                spinAmount.Value = 0;
                txtMedicineTypeCode.Text = "";
                txtMedicineTypeName.Text = "";
                txtServiceUnitName.Text = "";
                gridViewMedicineType.FindFilterText = "";
                gridViewMaterialType.FindFilterText = "";
                IList<Control> invalidBoXungControls = dxValidationProviderBoXung.GetInvalidControls();
                for (int i = invalidBoXungControls.Count - 1; i >= 0; i--)
                {
                    dxValidationProviderBoXung.RemoveControlError(invalidBoXungControls[i]);
                }

                var medicines = gridControlMedicineType.DataSource as List<HisMedicineTypeAmountSDO>;
                var materials = gridControlMaterialType.DataSource as List<HisMaterialTypeAmountSDO>;
                if (medicines != null && medicines.Count > 0)
                {
                    xtraTabControlExpMest.SelectedTabPageIndex = 0;
                    txtKeyword__Medicine.Focus();
                    txtKeyword__Medicine.SelectAll();
                }
                else if (materials != null && materials.Count > 0)
                {
                    xtraTabControlExpMest.SelectedTabPageIndex = 1;
                    txtKeyword__Material.Focus();
                    txtKeyword__Material.SelectAll();
                }
                else
                {
                    xtraTabControlExpMest.SelectedTabPageIndex = 0;
                    txtKeyword__Medicine.Focus();
                    txtKeyword__Medicine.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool CheckValidAmountGridMedicine()
        {
            bool valid = true;
            try
            {
                var medicinesMaterials = gridControlDetailMedicineProcess.DataSource as List<MssMedicineTypeSDO>;
                if (medicinesMaterials != null && medicinesMaterials.Count > 0)
                {
                    foreach (var item in medicinesMaterials)
                    {
                        if (item.IS_MEDICINE)
                        {
                            var listMedicineType = (List<MOS.SDO.HisMedicineTypeInStockSDO>)gridControlMedicineType.DataSource;
                            if (listMedicineType != null && listMedicineType.Count > 0)
                            {
                                var chkListMedicineType = listMedicineType.FirstOrDefault(o => o.Id == item.ID);
                                if (chkListMedicineType != null && ((chkListMedicineType.AvailableAmount < item.AMOUNT) || item.AMOUNT < 0))
                                {
                                    valid = false;
                                }
                            }
                        }
                        else
                        {
                            var listMaterialType = (List<MOS.SDO.HisMaterialTypeInStockSDO>)gridControlMaterialType.DataSource;
                            if (listMaterialType != null && listMaterialType.Count > 0)
                            {
                                var chkListMaterialType = listMaterialType.FirstOrDefault(o => o.Id == item.ID);
                                if (chkListMaterialType != null && ((chkListMaterialType.AvailableAmount < item.AMOUNT) || item.AMOUNT < 0))
                                {
                                    valid = false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        void UpdateDataFormToDTOHisChmsExpMest(HisChmsExpMestSDO data)
        {
            try
            {
                if (data != null)
                {
                    data.ExpMest.EXP_MEST_TYPE_ID = HisExpMestTypeCFG.HisExpMestTypeId__Chms;
                    data.ExpMest.EXP_MEST_STT_ID = HisExpMestSttCFG.HisExpMestSttId__Request;
                    if (cboMediStockExport.EditValue != null)
                        data.ExpMest.MEDI_STOCK_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboMediStockExport.EditValue ?? "0").ToString());
                    if (cboMediStockImport.EditValue != null && ActionType != GlobalVariables.ActionEdit)
                    {
                        var mediStockImp = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMediStockImport.EditValue ?? 0).ToString()));
                        data.ChmsExpMest.IMP_MEDI_STOCK_ID = (mediStockImp != null ? mediStockImp.ID : 0);
                    }
                    this.ListVHisMedicineTypeProcess = gridViewDetailMedicineProcess.DataSource as List<MssMedicineTypeSDO>;
                    if (this.ListVHisMedicineTypeProcess != null && this.ListVHisMedicineTypeProcess.Count > 0)
                    {
                        var medicineTypes = this.ListVHisMedicineTypeProcess.Where(o => o.IS_MEDICINE == true).ToList();
                        if (medicineTypes != null)
                        {
                            foreach (var item in medicineTypes)
                            {
                                HisMedicineTypeAmountSDO medicine = new HisMedicineTypeAmountSDO();
                                medicine.Amount = item.AMOUNT;
                                medicine.MedicineTypeId = item.ID;
                                data.ExpMedicines.Add(medicine);
                            }
                        }
                        var materialTypes = this.ListVHisMedicineTypeProcess.Where(o => o.IS_MEDICINE == false).ToList();
                        if (materialTypes != null)
                        {
                            foreach (var item in materialTypes)
                            {
                                HisMaterialTypeAmountSDO meterial = new HisMaterialTypeAmountSDO();
                                meterial.Amount = item.AMOUNT;
                                meterial.MaterialTypeId = item.ID;
                                data.ExpMaterials.Add(meterial);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void LoadDataToComboImpMedistock()
        {
            CommonParam param = new CommonParam();
            try
            {
                //lay ra cac tu truc cua khoa hin tai co cau hinh kho xuat - phong
                var mediStockImp = BackendDataWorker.Get<V_HIS_MEST_ROOM>().Where(o =>
                    o.MEDI_STOCK_ID == this.HisAggrExpMestRow.MEDI_STOCK_ID
                    && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                    && o.ROOM_TYPE_ID == HisRoomTypeCFG.HisRoomTypeId__Stock
                    && o.DEPARTMENT_ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentId
                    && BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Select(p => p.ROOM_ID).Contains(o.ROOM_ID)
                    ).ToList();

                cboMediStockImport.Properties.DataSource = mediStockImp;
                cboMediStockImport.Properties.DisplayMember = "ROOM_NAME";
                cboMediStockImport.Properties.ValueMember = "ROOM_ID";
                cboMediStockImport.Properties.ForceInitialize();
                cboMediStockImport.Properties.Columns.Clear();
                cboMediStockImport.Properties.Columns.Add(new LookUpColumnInfo("ROOM_CODE", "", 100));
                cboMediStockImport.Properties.Columns.Add(new LookUpColumnInfo("ROOM_NAME", "", 200));
                cboMediStockImport.Properties.ShowHeader = false;
                cboMediStockImport.Properties.ImmediatePopup = true;
                cboMediStockImport.Properties.PopupWidth = 300;

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void LoadDataToComboExpMedistock()
        {
            CommonParam param = new CommonParam();
            try
            {
                cboMediStockExport.Properties.DisplayMember = "MEDI_STOCK_NAME";
                cboMediStockExport.Properties.ValueMember = "ID";
                cboMediStockExport.Properties.ForceInitialize();
                cboMediStockExport.Properties.Columns.Clear();
                cboMediStockExport.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_CODE", "", 100));
                cboMediStockExport.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_NAME", "", 200));
                cboMediStockExport.Properties.ShowHeader = false;
                cboMediStockExport.Properties.ImmediatePopup = true;
                cboMediStockExport.Properties.PopupWidth = 300;
                cboMediStockExport.Properties.DataSource = BackendDataWorker.Get<V_HIS_MEDI_STOCK>();

                cboMediStockExport.EditValue = this.HisAggrExpMestRow.MEDI_STOCK_ID;
                txtMediStockExportCode.Text = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ID == this.HisAggrExpMestRow.MEDI_STOCK_ID).MEDI_STOCK_CODE;

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void LoadDetailDataByAggrExpMest()
        {
            try
            {
                var materialTypeInStockSDOInAggr = gridControlMaterialType.DataSource as List<MOS.SDO.HisMaterialTypeInStockSDO>;
                var medicineTypeInStockSDOInAggr = gridControlMedicineType.DataSource as List<MOS.SDO.HisMedicineTypeInStockSDO>;
                foreach (var item in this.ListVHisMedicineTypeProcess)
                {
                    if (item.IS_MEDICINE)
                    {
                        if (medicineTypeInStockSDOInAggr != null && medicineTypeInStockSDOInAggr.Count > 0)
                        {
                            var medicineInStock = medicineTypeInStockSDOInAggr.FirstOrDefault(o => o.ServiceId == item.SERVICE_ID);
                            if (medicineInStock != null)
                            {
                                item.ErrorMessage = ResourceMessageLang.SoLuongXuatLonHonSoLuongKhaDungTrongKho;
                                item.ErrorTypeAmount = ErrorType.Warning;
                                //medicineInStock.AvailableAmount ?? 0;
                            }
                            else
                            {
                                item.ErrorMessage = "";
                                item.ErrorTypeAmount = ErrorType.None;
                            }
                        }
                    }
                    else
                    {
                        if (materialTypeInStockSDOInAggr != null && materialTypeInStockSDOInAggr.Count > 0)
                        {
                            var materialInStock = materialTypeInStockSDOInAggr.FirstOrDefault(o => o.ServiceId == item.SERVICE_ID);
                            if (materialInStock != null && materialInStock.AvailableAmount < item.AMOUNT)
                            {
                                item.ErrorMessage = ResourceMessageLang.SoLuongXuatLonHonSoLuongKhaDungTrongKho;
                                item.ErrorTypeAmount = ErrorType.Warning;
                                //model.AMOUNT = materialInStock.AvailableAmount ?? 0;
                            }
                            else
                            {
                                item.ErrorMessage = "";
                                item.ErrorTypeAmount = ErrorType.None;
                            }
                        }
                    }
                }

                gridControlDetailMedicineProcess.DataSource = null;
                gridControlDetailMedicineProcess.DataSource = this.ListVHisMedicineTypeProcess;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetDataInForm()
        {
            try
            {
                this.positionHandle = -1;
                this.positionHandleControl = -1;

                this.ActionType = GlobalVariables.ActionAdd;

                IList<Control> invalidBoXungControls = dxValidationProviderBoXung.GetInvalidControls();
                for (int i = invalidBoXungControls.Count - 1; i >= 0; i--)
                {
                    dxValidationProviderBoXung.RemoveControlError(invalidBoXungControls[i]);
                }

                IList<Control> invalidControls = dxValidationProviderControl.GetInvalidControls();
                for (int i = invalidControls.Count - 1; i >= 0; i--)
                {
                    dxValidationProviderControl.RemoveControlError(invalidControls[i]);
                }

                this.HisMedicineTypeInStockSDO = new HisMedicineTypeInStockSDO();

                this.HisChmsExpMestSDO = new HisChmsExpMestSDO();
                this.HisChmsExpMestSDO.ExpMest = new HIS_EXP_MEST();
                this.HisChmsExpMestSDO.ChmsExpMest = new HIS_CHMS_EXP_MEST();
                this.HisChmsExpMestSDO.ExpMedicines = new List<HisMedicineTypeAmountSDO>();

                gridControlDetailMedicineProcess.DataSource = null;
                gridControlDetailMedicineExpView.DataSource = null;
                gridControlMedicineType.DataSource = null;
                gridViewMedicineType.FindFilterText = "";

                gridControlDetailMaterialExpView.DataSource = null;
                gridControlMaterialType.DataSource = null;
                gridViewMaterialType.FindFilterText = "";

                txtTotalPriceAllGridDetail.Text = "";

                txtMedicineTypeCode.Text = "";
                txtMedicineTypeName.Text = "";
                //txtNationalName.Text = "";
                //txtMenufactureName.Text = "";
                spinAmount.Value = 0;
                txtServiceUnitName.Text = "";
                cboPrint.Enabled = false;

                SetEnableButtonControl(this.ActionType);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<MssMedicineTypeSDO> LoadDetailDataByExpMestIds(List<long> expMestIds)
        {
            CommonParam param = new CommonParam();
            List<MssMedicineTypeSDO> results = new List<MssMedicineTypeSDO>();
            List<MssMedicineTypeSDO> medicineTypeTemp = new List<MssMedicineTypeSDO>();
            List<MssMedicineTypeSDO> medicineTypeProcess = new List<MssMedicineTypeSDO>();
            List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE> expMestManuMetyMatys = new List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>();//luu tat ca thuoc va vat tu
            try
            {
                int dem = 0;
                MOS.Filter.HisExpMestMedicineViewFilter mediFilter = new HisExpMestMedicineViewFilter();
                mediFilter.EXP_MEST_IDs = expMestIds;
                var expMestMedicines = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, param);
                if (expMestMedicines != null && expMestMedicines.Count > 0)
                {
                    var dataGroups = expMestMedicines.GroupBy(p => p.SERVICE_ID).Select(p => p.ToList()).ToList();
                    medicineTypeProcess = new List<MssMedicineTypeSDO>();
                    medicineTypeProcess = (
                        from m in dataGroups select new MssMedicineTypeSDO(m)
                        ).ToList();
                }

                MOS.Filter.HisExpMestMaterialViewFilter mateFilter = new HisExpMestMaterialViewFilter();
                mateFilter.EXP_MEST_IDs = expMestIds;
                var expMestMaterials = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, mateFilter, param);
                if (expMestMaterials != null && expMestMaterials.Count > 0)
                {
                    var dataGroups = expMestMaterials.GroupBy(p => p.SERVICE_ID).Select(p => p.ToList()).ToList();
                    medicineTypeProcess = new List<MssMedicineTypeSDO>();
                    medicineTypeProcess = (
                       from m in dataGroups select new MssMedicineTypeSDO(m)
                       ).ToList();
                }

                foreach (var item in medicineTypeTemp)
                {
                    decimal except = (Math.Round(item.AMOUNT) - item.AMOUNT);
                    decimal exceptRS = 0;
                    if (except != 0)
                    {
                        if (except > 0)
                        {
                            exceptRS = except;
                        }
                        else if (except < 0)
                        {
                            exceptRS = 1 + except;
                        }
                    }
                    if (exceptRS > 0)
                    {
                        MssMedicineTypeSDO model = new MssMedicineTypeSDO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MssMedicineTypeSDO>(model, item);
                        model.AMOUNT = exceptRS;
                        model.IdRow = dem;
                        dem += 1;
                        results.Add(model);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return results;
        }
    }
}
