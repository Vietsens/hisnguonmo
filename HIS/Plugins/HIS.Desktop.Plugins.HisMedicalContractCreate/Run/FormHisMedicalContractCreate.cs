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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Common.Modules;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using MOS.Filter;
using HIS.Desktop.Plugins.HisMedicalContractCreate.ADO;
using HIS.Desktop.Plugins.HisMedicalContractCreate.Validation;

namespace HIS.Desktop.Plugins.HisMedicalContractCreate.Run
{
    public partial class FormHisMedicalContractCreate : FormBase
    {
        private Inventec.Desktop.Common.Modules.Module module;
        private long medicalContractId;
        private Common.DelegateRefreshData refreshData;
        private int cboSupplierNum;//1 - cboSupplier; 2 - cboDocumentSupplier
        private int positionHandle = -1;

        private V_HIS_MEDI_STOCK currentStock = null;
        private bool isInit = true;
        private bool hasBusiness = false;
        private bool hasNotBusiness = false;

        private HIS_MEDICAL_CONTRACT CurrentContact;
        private HIS_MEDICAL_CONTRACT ContactPrint;

        private List<V_HIS_BID_1> ListBid = new List<V_HIS_BID_1>();
        private HIS.UC.MedicineType.MedicineTypeProcessor medicineTypeProcessor;
        private HIS.UC.MaterialType.MaterialTypeTreeProcessor materialTypeProcessor;
        private UserControl ucMedicineType;
        private UserControl ucMaterialType;
        private HIS.UC.MedicineType.ADO.MedicineTypeADO medicineType;
        private HIS.UC.MaterialType.ADO.MaterialTypeADO materialType;

        private UC_LoadEdit ucContractMety;
        private UC_LoadEdit ucContractMaty;

        private List<V_HIS_MEDICINE_TYPE> ListHisMedicineType = new List<V_HIS_MEDICINE_TYPE>();
        private List<V_HIS_MATERIAL_TYPE> ListHisMaterialType = new List<V_HIS_MATERIAL_TYPE>();
        private List<V_HIS_MEDICINE_TYPE> ListHisMedicineTypeAllData = new List<V_HIS_MEDICINE_TYPE>();
        private List<V_HIS_MATERIAL_TYPE> ListHisMaterialTypeAllData = new List<V_HIS_MATERIAL_TYPE>();
        private int ActionType = 0;
        public string bidGroupCodeSelected;

        ADO.MetyMatyADO EditMedicine;
        ADO.MetyMatyADO EditMaterial;

        private List<ADO.MetyMatyADO> ListMedicineADO = new List<ADO.MetyMatyADO>();
        private List<ADO.MetyMatyADO> ListMaterialADO = new List<ADO.MetyMatyADO>();
        private List<ADO.MetyMatyADO> ListMedicineADOTemp = new List<ADO.MetyMatyADO>();
        private List<ADO.MetyMatyADO> ListMaterialADOTemp = new List<ADO.MetyMatyADO>();
        HisMedicalContractSDO MedicalContractSDO;
        private List<HIS_MEDI_CONTRACT_METY> mediContractMety = new List<HIS_MEDI_CONTRACT_METY>();
        private List<HIS_MEDI_CONTRACT_MATY> mediContractMaty = new List<HIS_MEDI_CONTRACT_MATY>();

        Task taskLoadAsyncDataToCbo { get; set; }
        Task taskLoadAsyncCurrentDataEdit { get; set; }
        Task taskLoadAsyncMediStock { get; set; }

        Task taskFillAsyncDataToGridMedicineType { get; set; }
        Task taskFillAsyncDataToGridMaterialType { get; set; }
        Dictionary<long, List<V_HIS_BID_MEDICINE_TYPE>> DicBidMedicineType { get; set; }
        Dictionary<long, List<V_HIS_BID_MATERIAL_TYPE>> DicBidMaterialType { get; set; }
        Dictionary<int, TabLoadStatusADO> DicMedicineStatus { get; set; }
        Dictionary<int, TabLoadStatusADO> DicMaterialStatus { get; set; }
        public FormHisMedicalContractCreate(Module module, long medicalContractId, Common.DelegateRefreshData refreshData)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.module = module;
                this.medicalContractId = medicalContractId;
                this.refreshData = refreshData;
                this.Text = module.text;
                InitializeMedicineType();
                InitializeMaterialType();
                InitGridEdit();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormHisMedicalContractCreate_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetCaptionByLanguageKey();
                SetDefaultValueControl();
                ValidControl();
                LoadAsyncForm();

                WaitingManager.Hide();
                cboSupplier.Focus();
                cboSupplier.SelectAll();
                this.isInit = false;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadAsyncForm()
        {
            try
            {
                this.taskLoadAsyncDataToCbo = LoadAsyncDataToCbo();

                this.taskLoadAsyncCurrentDataEdit = LoadAsyncCurrentDataEdit();

                this.taskLoadAsyncMediStock = LoadAsyncMediStock();

                this.taskFillAsyncDataToGridMedicineType = FillAsyncDataToGridMedicineType();
                this.taskFillAsyncDataToGridMaterialType = FillAsyncDataToGridMaterialType();

                await this.taskLoadAsyncDataToCbo;
                await this.taskLoadAsyncCurrentDataEdit;
                await this.taskLoadAsyncMediStock;

                await this.taskFillAsyncDataToGridMedicineType;
                await this.taskFillAsyncDataToGridMaterialType;
                ReloadDataGridMetyMaty();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool isLoadingAsyncMediStock = false;
        private async Task LoadAsyncMediStock()
        {
            try
            {
                Task t = new Task(
                    () =>
                    {
                        this.currentStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == this.currentModuleBase.RoomId);
                    }
                );
                t.Start();
                await t;
                if (medicalContractId > 0)
                {
                    await taskLoadAsyncCurrentDataEdit;
                }
                isLoadingAsyncMediStock = true;

                if (this.currentStock != null && this.currentStock.IS_BUSINESS == (short)1)
                {
                    chkIsOnlyShowByBusiness.Text = Resources.ResourceLanguageManager.ChiHienThiThuocVatTuKD;
                    chkIsOnlyShowByBusiness.ToolTip = Resources.ResourceLanguageManager.ChiHienThiThuocVatTuKinhDoanh;
                    if (this.medicalContractId <= 0)
                    {
                        chkIsOnlyShowByBusiness.Checked = true;
                    }
                    else
                    {
                        if (this.hasNotBusiness)
                        {
                            chkIsOnlyShowByBusiness.Checked = false;
                        }
                        else
                        {
                            chkIsOnlyShowByBusiness.Checked = true;
                        }
                    }
                }
                else
                {
                    chkIsOnlyShowByBusiness.Text = Resources.ResourceLanguageManager.ChiHienThiThuocVatTuKhongKD;
                    chkIsOnlyShowByBusiness.ToolTip = Resources.ResourceLanguageManager.ChiHienThiThuocVatTuKhongKinhDoanh;
                    if (this.medicalContractId <= 0)
                    {
                        chkIsOnlyShowByBusiness.Checked = true;
                    }
                    else
                    {
                        if (this.hasBusiness)
                        {
                            chkIsOnlyShowByBusiness.Checked = false;
                        }
                        else
                        {
                            chkIsOnlyShowByBusiness.Checked = true;
                        }
                    }
                }
                isLoadingAsyncMediStock = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSupplier_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)
                {
                    cboSupplierNum = 1;
                    List<object> listArgs = new List<object>();
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)RefreshSupplierData);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisSupplier", module.RoomId, module.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSupplier_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Immediate || e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboBid.Enabled)
                    {
                        if (cboSupplier.EditValue != null)
                        {
                            ReloadDataBid(Inventec.Common.TypeConvert.Parse.ToInt64(cboSupplier.EditValue.ToString()));
                            //Khi thầu không có trong nhà cung cấp và thay đổi nhà cung cấp danh sách thuốc hoặc vật tư sẽ không tải lại và lấy dữ liệu từ danh mục 
                            if (DicMedicineStatus == null)
                                DicMedicineStatus = new Dictionary<int, TabLoadStatusADO>();
                            if (!DicMedicineStatus.ContainsKey(0))
                                DicMedicineStatus[0] = new TabLoadStatusADO();
                            if (DicMaterialStatus == null)
                                DicMaterialStatus = new Dictionary<int, TabLoadStatusADO>();
                            if (!DicMaterialStatus.ContainsKey(1))
                                DicMaterialStatus[1] = new TabLoadStatusADO();
                            if (cboBid.EditValue == null)
                            {
                                DicMedicineStatus[0].supplierId = Inventec.Common.TypeConvert.Parse.ToInt64(cboSupplier.EditValue.ToString());
                                DicMaterialStatus[1].supplierId = Inventec.Common.TypeConvert.Parse.ToInt64(cboSupplier.EditValue.ToString());
                            }
                        }
                        ReloadDataGridMetyMaty();
                        cboDocumentSupplier.EditValue = cboSupplier.EditValue;
                        cboBid.Focus();
                        cboBid.SelectAll();
                    }
                    else
                    {
                        txtMedicalContractCode.Focus();
                        txtMedicalContractCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSupplier_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboBid.Enabled)
                    {
                        cboBid.Focus();
                        cboBid.SelectAll();
                    }
                    else
                    {
                        txtMedicalContractCode.Focus();
                        txtMedicalContractCode.SelectAll();
                    }
                }
                //else if (!cboSupplier.IsPopupOpen)
                //{
                //    cboSupplier.ShowPopup();
                //    cboSupplier.Text += e.KeyData;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBid_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (cboBid.Enabled && e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboBid.EditValue = null;
                    ReLoadControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReLoadControl()
        {
            try
            {


                spHourLifespan.Enabled = true;
                spDayLifespan.Enabled = true;
                spMonthLifespan.Enabled = true;
                spImpVat.Enabled = true;
                lciHour.Enabled = true;
                lciPnNationalName.Enabled = true;
                dtExpiredDate.Enabled = true;
                txtRegisterNumber.Enabled = true;
                txtNationalName.Enabled = true;
                chkEditNational.Enabled = true;
                txtConcentra.Enabled = true;
                txtManufacturerCode.Enabled = true;
                cboManufacturerName.Enabled = true;
                txtQDThau.Enabled = true;
                txtNhomThau.Enabled = true;
                txtGhiChu.Enabled = true;

                spHourLifespan.EditValue = null;
                spDayLifespan.EditValue = null;
                spMonthLifespan.EditValue = null;
                spImpVat.EditValue = null;
                spContractPrice.EditValue = null;
                spImpPrice.EditValue = null;
                spAmount.EditValue = null;
                dtExpiredDate.EditValue = null;
                txtRegisterNumber.Text = null;
                txtNationalName.Text = null;
                txtConcentra.Text = null;
                txtManufacturerCode.EditValue = null;
                cboManufacturerName.EditValue = null;
                txtQDThau.Text = null;
                txtGhiChu.Text = null;
                txtNhomThau.Text = null;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBid_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Immediate || e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtMedicalContractCode.Focus();
                    txtMedicalContractCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBid_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMedicalContractCode.Focus();
                    txtMedicalContractCode.SelectAll();
                }
                //else if (!cboBid.IsPopupOpen)
                //{
                //    cboBid.ShowPopup();
                //    cboBid.Text += e.KeyData;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBid_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboBid.Enabled && this.isLoadingAsyncMedicalContract == false)
                {
                    //ReloadDataGridMetyMaty(Inventec.Common.TypeConvert.Parse.ToInt64((cboBid.EditValue ?? 0).ToString()));
                    ReloadDataGridMetyMaty();
                    ReLoadControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMedicalContractCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMedicalContractName.Focus();
                    txtMedicalContractName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMedicalContractName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDocumentSupplier.Focus();
                    cboDocumentSupplier.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDocumentSupplier_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)
                {
                    cboSupplierNum = 2;
                    List<object> listArgs = new List<object>();
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)RefreshSupplierData);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisSupplier", module.RoomId, module.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDocumentSupplier_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Immediate || e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtVentureAgreening.Focus();
                    txtVentureAgreening.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDocumentSupplier_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtVentureAgreening.Focus();
                    txtVentureAgreening.SelectAll();
                }
                //else if (!cboDocumentSupplier.IsPopupOpen)
                //{
                //    cboDocumentSupplier.ShowPopup();
                //    cboDocumentSupplier.Text += e.KeyData;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtVentureAgreening_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtValidFromDate.Focus();
                    dtValidFromDate.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtValidFromDate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtValidToDate.Focus();
                    dtValidToDate.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtValidToDate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNote.Focus();
                    txtNote.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spContractPrice.Focus();
                    spContractPrice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spContractPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spImpVat.Focus();
                    spImpVat.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spContractPrice_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                spPriceVat.EditValue = spContractPrice.Value;
                //  spImpVat.EditValue = (spContractPrice.Value / spImpPrice.Value - 1) * 100 ;
                spImpPrice.EditValue = spContractPrice.Value / (1 + spImpVat.Value / 100);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spImpVat_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtImportDate.Focus();
                    dtImportDate.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spImpVat_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                spPriceVat.EditValue = spContractPrice.Value;
                //   spContractPrice.EditValue = spImpPrice.Value * (1 + spImpVat.Value / 100);
                spImpPrice.EditValue = spContractPrice.Value / (1 + spImpVat.Value / 100);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtExpiredDate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spMonthLifespan.Enabled == false)
                    {
                        if (spDayLifespan.Enabled == false)
                        {
                            if (spHourLifespan.Enabled == false)
                            {
                                txtRegisterNumber.Focus();
                                txtRegisterNumber.SelectAll();
                            }
                            else
                            {
                                spHourLifespan.Focus();
                                spHourLifespan.SelectAll();
                            }
                        }
                        else
                        {
                            spDayLifespan.Focus();
                            spDayLifespan.SelectAll();
                        }

                    }
                    else
                    {
                        spMonthLifespan.Focus();
                        spMonthLifespan.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }



        private void spMonthLifespan_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spMonthLifespan.EditValue != null && spMonthLifespan.Value > 0)
                    {
                        if (txtRegisterNumber.Enabled == false)
                        {
                            txtNationalName.Focus();
                            txtNationalName.SelectAll();
                        }
                        else
                        {
                            txtRegisterNumber.Focus();
                            txtRegisterNumber.SelectAll();
                        }
                    }
                    else
                    {
                        if (spDayLifespan.Enabled == false)
                        {
                            if (spHourLifespan.Enabled == false)
                            {
                                if (txtRegisterNumber.Enabled == false)
                                {
                                    txtNationalName.Focus();
                                    txtNationalName.SelectAll();
                                }
                                else
                                {
                                    txtRegisterNumber.Focus();
                                    txtRegisterNumber.SelectAll();
                                }
                            }
                            else
                            {
                                spHourLifespan.Focus();
                                spHourLifespan.SelectAll();
                            }
                        }
                        else
                        {
                            spDayLifespan.Focus();
                            spDayLifespan.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spMonthLifespan_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (spMonthLifespan.EditValue != null && spMonthLifespan.Value > 0)
                {
                    spDayLifespan.Enabled = false;
                    spHourLifespan.Enabled = false;
                    lciHour.Enabled = false;
                }
                else
                {
                    spHourLifespan.Enabled = true;
                    spDayLifespan.Enabled = true;
                    lciHour.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spDayLifespan_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spDayLifespan.EditValue != null && spDayLifespan.Value > 0)
                    {
                        if (txtRegisterNumber.Enabled == false)
                        {
                            txtNationalName.Focus();
                            txtNationalName.SelectAll();
                        }
                        else
                        {
                            txtRegisterNumber.Focus();
                            txtRegisterNumber.SelectAll();
                        }
                    }
                    else
                    {

                        if (spHourLifespan.Enabled == false)
                        {
                            if (txtRegisterNumber.Enabled == false)
                            {
                                txtNationalName.Focus();
                                txtNationalName.SelectAll();
                            }
                            else
                            {
                                txtRegisterNumber.Focus();
                                txtRegisterNumber.SelectAll();
                            }
                        }
                        else
                        {
                            spHourLifespan.Focus();
                            spHourLifespan.SelectAll();
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spDayLifespan_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (spDayLifespan.EditValue != null && spDayLifespan.Value > 0)
                {
                    spHourLifespan.Enabled = false;
                    spMonthLifespan.Enabled = false;
                    lciHour.Enabled = false;
                }
                else
                {
                    spHourLifespan.Enabled = true;
                    spMonthLifespan.Enabled = true;
                    lciHour.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void spHourLifespan_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (spHourLifespan.EditValue != null && spHourLifespan.Value > 0)
                {
                    spDayLifespan.Enabled = false;
                    spMonthLifespan.Enabled = false;
                }
                else
                {
                    spDayLifespan.Enabled = true;
                    spMonthLifespan.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtRegisterNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtNationalName.Visible)
                    {
                        txtNationalName.Focus();
                        txtNationalName.SelectAll();
                    }
                    else
                    {
                        cboNationalName.Focus();
                        cboNationalName.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNationalName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    chkEditNational.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNationalName_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    txtNationalName.Text = "";
                    cboNationalName.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNationalName_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboNationalName.EditValue != null)
                    {
                        var data = BackendDataWorker.Get<SDA_NATIONAL>().FirstOrDefault
                            (o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboNationalName.EditValue.ToString()));
                        if (data != null)
                        {
                            txtNationalName.Text = data.NATIONAL_NAME;
                            if (txtConcentra.Enabled)
                            {
                                txtConcentra.Focus();
                                txtConcentra.SelectAll();
                            }
                            else
                            {
                                txtManufacturerCode.Focus();
                                txtManufacturerCode.SelectAll();
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

        private void chkEditNational_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkEditNational.CheckState == CheckState.Checked)
                {
                    cboNationalName.Visible = false;
                    txtNationalName.Visible = true;
                    if (!String.IsNullOrEmpty(cboNationalName.Text))
                    {
                        txtNationalName.Text = cboNationalName.Text;
                    }

                    txtNationalName.Focus();
                    txtNationalName.SelectAll();
                }
                else if (chkEditNational.CheckState == CheckState.Unchecked)
                {
                    txtNationalName.Visible = false;
                    cboNationalName.Visible = true;
                    txtNationalName.Text = cboNationalName.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtConcentra_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtManufacturerCode.Focus();
                    txtManufacturerCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtManufacturerCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtManufacturerCode.Text.Trim()))
                    {
                        string code = txtManufacturerCode.Text.Trim().ToLower();
                        var listData = BackendDataWorker.Get<HIS_MANUFACTURER>().Where(o => o.MANUFACTURER_CODE.ToLower().Contains(code)).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.MANUFACTURER_CODE.ToLower() == code).ToList() : listData) : null;
                        if (result != null && result.Count > 0)
                        {
                            showCbo = false;
                            txtManufacturerCode.Text = result.First().MANUFACTURER_CODE;
                            cboManufacturerName.EditValue = result.First().ID;
                            btnAdd.Focus();
                            e.Handled = true;
                        }
                    }
                    if (showCbo)
                    {
                        cboManufacturerName.Focus();
                        cboManufacturerName.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboManufacturerName_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboManufacturerName.EditValue = null;
                    txtManufacturerCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboManufacturerName_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboManufacturerName.EditValue != null)
                    {
                        var data = BackendDataWorker.Get<HIS_MANUFACTURER>().FirstOrDefault
                            (o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboManufacturerName.EditValue.ToString()));
                        if (data != null)
                        {
                            txtManufacturerCode.Text = data.MANUFACTURER_CODE;
                            btnAdd.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void xtraTabMetyMate_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                SetDefaultValueControlDetail();
                if (xtraTabMetyMate.SelectedTabPage == tabMedicineType) // thuoc
                {
                    medicineTypeProcessor.FocusKeyword(ucMedicineType);
                    txtRegisterNumber.Enabled = true;
                    EnableLeftControl(true);
                }
                else if (xtraTabMetyMate.SelectedTabPage == tabMaterialType) // vat tu
                {
                    materialTypeProcessor.FocusKeyword(ucMaterialType);
                    txtRegisterNumber.Enabled = false;
                    EnableLeftControl(true);
                }
                ReloadDataGridMetyMaty();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void dxValidationProviderContract_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProviderMetyMate_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barBtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnAdd_Click(null, null);
        }

        private void barBtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }
        long? DiscountFromDate = null;
        long? DiscountToDate = null;
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                btnAdd.Focus();
                positionHandle = -1;

                DiscountFromDate = null;
                DiscountToDate = null;
                bool IsWarnDate = false;
                if (dteDiscountFromDate.EditValue != null && dteDiscountFromDate.DateTime != DateTime.MinValue)
                {
                    if (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteDiscountFromDate.DateTime) >= Int64.Parse(DateTime.Now.ToString("yyyyMMdd000000")))
                        DiscountFromDate = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteDiscountFromDate.DateTime);
                    else
                    {
                        IsWarnDate = true;
                    }    
                }
                if (dteDiscountToDate.EditValue != null && dteDiscountToDate.DateTime != DateTime.MinValue)
                {
                    if (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteDiscountToDate.DateTime) >= Int64.Parse(DateTime.Now.ToString("yyyyMMdd000000")))
                        DiscountToDate = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteDiscountToDate.DateTime);
                    else
                        IsWarnDate = true;
                }

                if ((DiscountFromDate != null && DiscountToDate != null && DiscountFromDate > DiscountToDate) || IsWarnDate)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show
                            ("Thời gian giảm giá không hợp lệ");
                    return;
                }


                //if ((spContractPrice.EditValue == null || spContractPrice.Value == 0) && (DiscountFromDate == null || DiscountToDate == null))
                //{
                //    PriceAmountValidationRule impPriceValidate = new PriceAmountValidationRule();
                //    impPriceValidate.spinImpPrice = spContractPrice;
                //    impPriceValidate.ErrorText = Resources.ResourceLanguageManager.ThieuTruongDuLieuBatBuoc;
                //    impPriceValidate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                //    dxValidationProviderMetyMate.SetValidationRule(spContractPrice, impPriceValidate);
                //}
                //else
                //{
                //    dxValidationProviderMetyMate.SetValidationRule(spContractPrice, null);
                //}

               
                bool valid = true;
                valid = dxValidationProviderContract.Validate() && valid;

                valid = dxValidationProviderMetyMate.Validate() && valid;

                if (!valid) return;

                //      if (!dxValidationProviderMetyMate.Validate()) return;
                if (xtraTabMetyMate.SelectedTabPage == tabMedicineType) // thuoc
                {
                    if (ListMedicineADO != null && ListMedicineADO.Count > 0 && (this.ActionType == GlobalVariables.ActionAdd || (this.ActionType == GlobalVariables.ActionEdit && ListMedicineADO.Where(o=> o.ID == this.medicineType.ID).Count() > 1)))
                    {
                        var itemExistDiscountDate = ListMedicineADO.FirstOrDefault(o => o.ID == this.medicineType.ID && ((DiscountToDate != 0 && DiscountFromDate != 0 && (o.DISCOUNT_FROM_DATE ?? 0) <= (DiscountToDate ?? 0) && (DiscountFromDate ?? 0) <=(o.DISCOUNT_TO_DATE ?? 0)) || (DiscountFromDate != 0 && (o.DISCOUNT_FROM_DATE ?? 0) <= (DiscountFromDate ?? 0) && (DiscountFromDate ?? 0) <= (o.DISCOUNT_TO_DATE ?? 0)) || (DiscountToDate != 0 && (o.DISCOUNT_FROM_DATE ?? 0) <= (DiscountToDate ?? 0) && (DiscountToDate ?? 0) <= (o.DISCOUNT_TO_DATE ?? 0))));
                        if (itemExistDiscountDate != null && (itemExistDiscountDate.DISCOUNT_FROM_DATE.HasValue || itemExistDiscountDate.DISCOUNT_TO_DATE.HasValue))
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show
                         (string.Format("Thuốc {0} có thời gian giảm giá{1}{2}.", itemExistDiscountDate.MEDICINE_TYPE_NAME, itemExistDiscountDate.DISCOUNT_FROM_DATE != null ? string.Format(" từ {0}", Inventec.Common.DateTime.Convert.TimeNumberToDateString(itemExistDiscountDate.DISCOUNT_FROM_DATE ?? 0)) : "", itemExistDiscountDate.DISCOUNT_TO_DATE != null ? string.Format(" đến {0}", Inventec.Common.DateTime.Convert.TimeNumberToDateString(itemExistDiscountDate.DISCOUNT_TO_DATE ?? 0)) : ""));
                            return;
                        }    
                    }

                    if (!addMedicine())
                    {
                        return;
                    }

                    this.medicineTypeProcessor.FocusKeyword(this.ucMedicineType);
                }
                else if (xtraTabMetyMate.SelectedTabPage == tabMaterialType) // vat tu
                {
                    if (ListMedicineADO != null && ListMedicineADO.Count > 0 && (this.ActionType == GlobalVariables.ActionAdd || (this.ActionType == GlobalVariables.ActionEdit && ListMedicineADO.Where(o => o.ID == this.materialType.ID).Count() > 1)))
                    {
                        var itemExistDiscountDate = ListMedicineADO.FirstOrDefault(o => o.ID == this.materialType.ID && ((DiscountToDate != 0 && DiscountFromDate != 0 && (o.DISCOUNT_FROM_DATE ?? 0) <= (DiscountToDate ?? 0) && (DiscountFromDate ?? 0) <= (o.DISCOUNT_TO_DATE ?? 0)) || (DiscountFromDate != 0 && (o.DISCOUNT_FROM_DATE ?? 0) <= (DiscountFromDate ?? 0) && (DiscountFromDate ?? 0) <= (o.DISCOUNT_TO_DATE ?? 0)) || (DiscountToDate != 0 && (o.DISCOUNT_FROM_DATE ?? 0) <= (DiscountToDate ?? 0) && (DiscountToDate ?? 0) <= (o.DISCOUNT_TO_DATE ?? 0))));
                        if (itemExistDiscountDate != null && (itemExistDiscountDate.DISCOUNT_FROM_DATE.HasValue || itemExistDiscountDate.DISCOUNT_TO_DATE.HasValue))
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show
                         (string.Format("Vật tư {0} có thời gian giảm giá{1}{2}.", itemExistDiscountDate.MEDICINE_TYPE_NAME, itemExistDiscountDate.DISCOUNT_FROM_DATE != null ? string.Format(" từ {0}", Inventec.Common.DateTime.Convert.TimeNumberToDateString(itemExistDiscountDate.DISCOUNT_FROM_DATE ?? 0)) : "", itemExistDiscountDate.DISCOUNT_TO_DATE != null ? string.Format(" đến {0}", Inventec.Common.DateTime.Convert.TimeNumberToDateString(itemExistDiscountDate.DISCOUNT_TO_DATE ?? 0)) : ""));
                            return;
                        }
                    }
                    if (!addMaterial())
                    {
                        return;
                    }

                    this.materialTypeProcessor.FocusKeyword(this.ucMaterialType);
                }

                cboBid.Enabled = false;
                cboSupplier.Enabled = false;

                UpdateGrid();
                ClearForAdd();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            ucContractMety.PostEditor();
            ucContractMaty.PostEditor();
            CommonParam paramCommon = new CommonParam();
            bool success = false;

            try
            {
                this.positionHandle = -1;

                if (!dxValidationProviderContract.Validate())
                    return;

                if (CheckValidDataInGridService(ref paramCommon, this.ListMedicineADO, this.ListMaterialADO))
                {
                    WaitingManager.Hide();
                    GetDataForSave();
                    if (this.MedicalContractSDO == null ||
                        this.MedicalContractSDO.MaterialTypes == null ||
                        this.MedicalContractSDO.MedicineTypes == null ||
                        (this.MedicalContractSDO.MaterialTypes.Count <= 0 &&
                        this.MedicalContractSDO.MedicineTypes.Count <= 0))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show
                            (Resources.ResourceLanguageManager.ThieuTruongDuLieuBatBuoc);
                        return;
                    }

                    List<String> nameContractPriceChange = new List<string>();

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("______________MedicalContractSDO____MedicalContractSDO___________" + Inventec.Common.Logging.LogUtil.GetMemberName(() => MedicalContractSDO), MedicalContractSDO));
                    if (this.medicalContractId > 0)
                    {
                        MedicalContractSDO.Id = this.medicalContractId;
                        if (mediContractMaty != null && mediContractMaty.Count > 0)
                        {
                            if (this.ListMaterialADO != null && this.ListMaterialADO.Count > 0)
                            {
                                foreach (var item in this.ListMaterialADO)
                                {
                                    if (mediContractMaty.FirstOrDefault(o => o.MATERIAL_TYPE_ID == item.ID && o.ID == item.CONTRACT_MATY_METY_ID) != null)
                                    {
                                        if (item.CONTRACT_PRICE != mediContractMaty.FirstOrDefault(o => o.MATERIAL_TYPE_ID == item.ID && o.ID == item.CONTRACT_MATY_METY_ID).CONTRACT_PRICE)
                                        {
                                            nameContractPriceChange.Add(item.MEDICINE_TYPE_NAME);
                                        }
                                    }
                                }
                            }
                        }
                        if (mediContractMety != null && mediContractMety.Count > 0)
                        {
                            if (this.ListMedicineADO != null && this.ListMedicineADO.Count > 0)
                            {
                                foreach (var item in this.ListMedicineADO)
                                {
                                    if (mediContractMety.FirstOrDefault(o => o.MEDICINE_TYPE_ID == item.ID && o.ID == item.CONTRACT_MATY_METY_ID) != null)
                                    {
                                        if (item.CONTRACT_PRICE != mediContractMety.FirstOrDefault(o => o.MEDICINE_TYPE_ID == item.ID && o.ID == item.CONTRACT_MATY_METY_ID).CONTRACT_PRICE)
                                        {
                                            nameContractPriceChange.Add(item.MEDICINE_TYPE_NAME);
                                        }
                                    }
                                }
                            }
                        }
                        if (nameContractPriceChange.Count > 0)
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Các thuốc/vật tư {0} có thay đổi thông tin giá hợp đồng, hệ thống sẽ tự động cập nhật lại giá bán dựa vào thông tin giá hợp đồng mới và thiết lập thặng dư. Bạn có chắc muốn thực hiện không?", string.Join(", ", nameContractPriceChange)), "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                            {
                                return;
                            }
                        }
                        ContactPrint = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Post<HIS_MEDICAL_CONTRACT>("api/HisMedicalContract/UpdateSdo", ApiConsumer.ApiConsumers.MosConsumer, MedicalContractSDO, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    }
                    else
                    {
                        ContactPrint = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Post<HIS_MEDICAL_CONTRACT>("api/HisMedicalContract/CreateSdo", ApiConsumer.ApiConsumers.MosConsumer, MedicalContractSDO, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    }

                    if (ContactPrint != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ContactPrint), ContactPrint));
                        medicalContractId = ContactPrint.ID;
                        UpdateDetailId();
                        success = true;
                        SetDefaultValueControlDetail();
                        if (refreshData != null)
                            refreshData();
                    }
                    else
                        Inventec.Common.Logging.LogSystem.Warn("ContactPrint null");
                }
                WaitingManager.Hide();
                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this, paramCommon, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                long supplierId = Inventec.Common.TypeConvert.Parse.ToInt64((cboSupplier.EditValue ?? 0).ToString());
                if (supplierId <= 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Vui lòng chọn nhà thầu trước khi nhập khẩu dữ liệu", Resources.ResourceLanguageManager.ThongBao);
                    return;
                }

                if (cboSupplier.Enabled)
                {
                    cboSupplier.Enabled = false;
                }

                bool success = false;
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    WaitingManager.Show();
                    var import = new Inventec.Common.ExcelImport.Import();
                    if (import.ReadFileExcel(ofd.FileName))
                    {
                        var ImpMestListProcessor = import.Get<ADO.MetyMatyADO>(0);
                        if (ImpMestListProcessor != null && ImpMestListProcessor.Count > 0)
                        {
                            var listMedicine = ImpMestListProcessor.Where(o => !String.IsNullOrWhiteSpace(o.IS_MEDICINE)).ToList();
                            var listMaterial = ImpMestListProcessor.Where(o => String.IsNullOrWhiteSpace(o.IS_MEDICINE)).ToList();

                            this.ListMaterialADO = new List<ADO.MetyMatyADO>();
                            this.ListMedicineADO = new List<ADO.MetyMatyADO>();

                            List<ADO.MetyMatyADO> listError = new List<ADO.MetyMatyADO>();
                            addListMedicineTypeToProcessList(listMedicine, ref listError);
                            addListMaterialTypeToProcessList(listMaterial, ref listError);

                            if (listError != null && listError.Count > 0)
                            {
                                List<string> mess = new List<string>();
                                var medi = listError.Where(o => !String.IsNullOrWhiteSpace(o.IS_MEDICINE)).ToList();
                                var mate = listError.Where(o => String.IsNullOrWhiteSpace(o.IS_MEDICINE)).ToList();

                                if (medi != null && medi.Count > 0)
                                {
                                    var messageErr = String.Format(Resources.ResourceLanguageManager.CanhBaoThuoc, string.Join(";", medi.Select(s => string.Format("{0}({1})", s.MEDICINE_TYPE_NAME, s.MEDICINE_TYPE_CODE))));
                                    messageErr += Resources.ResourceLanguageManager.BiTrung;
                                    mess.Add(messageErr);
                                }

                                if (mate != null && mate.Count > 0)
                                {
                                    var messageErr = String.Format(Resources.ResourceLanguageManager.CanhBaoVatTu, string.Join(";", mate.Select(s => string.Format("{0}({1})", s.MEDICINE_TYPE_NAME, s.MEDICINE_TYPE_CODE))));
                                    messageErr += Resources.ResourceLanguageManager.BiTrung;
                                    mess.Add(messageErr);
                                }

                                MessageBox.Show(string.Join(";", mess));
                            }

                            success = true;
                            UpdateGrid();
                            WaitingManager.Hide();
                        }
                    }

                    if (!success)
                    {
                        CommonParam paramCommon = new CommonParam();
                        Inventec.Desktop.Common.Message.MessageManager.Show(this, paramCommon, success);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsOnlyShowByBusiness_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isInit || this.isLoadingAsyncMediStock)
                {
                    return;
                }
                CreateThreadLoadData();
                IsFirstLoadMedicineType = false;
                IsFirstLoadMaterialType = false;
                DicMedicineStatus = new Dictionary<int, ADO.TabLoadStatusADO>();
                DicMaterialStatus = new Dictionary<int, TabLoadStatusADO>();
                ReloadDataGridMetyMaty();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void CreateThreadLoadData()
        {
            Inventec.Common.Logging.LogSystem.Debug("CreateThreadLoadData_____");
            Thread ThreadMedicine = new Thread(new ThreadStart(FillDataToGridMedicineType));
            Thread ThreadMaterial = new Thread(new ThreadStart(FillDataToGridMaterialType));
            try
            {
                ThreadMedicine.Start();
                ThreadMaterial.Start();

                ThreadMedicine.Join();
                ThreadMaterial.Join();
            }
            catch (Exception ex)
            {
                ThreadMedicine.Abort();
                ThreadMaterial.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtImportDate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    long bidId = Inventec.Common.TypeConvert.Parse.ToInt64((cboBid.EditValue ?? 0).ToString());
                    if (bidId > 0)
                    {
                        btnAdd.Focus();
                    }
                    else
                    {
                        dtExpiredDate.Focus();
                        dtExpiredDate.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDowloadFile_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_MEDICAL_CONTRACT.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog1.Title = "Save File";
                    saveFileDialog1.FileName = "IMPORT_MEDICAL_CONTRACT";
                    saveFileDialog1.DefaultExt = "xlsx";
                    saveFileDialog1.Filter = "Excel files (*.xlsx)|All files (*.*)";
                    saveFileDialog1.FilterIndex = 2;
                    saveFileDialog1.RestoreDirectory = true;
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(fileName, saveFileDialog1.FileName);
                        MessageManager.Show(this.ParentForm, param, true);
                        if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn mở file ngay?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
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

        private void spHourLifespan_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtRegisterNumber.Enabled == false)
                    {
                        txtNationalName.Focus();
                        txtNationalName.SelectAll();
                        Inventec.Common.Logging.LogSystem.Debug("~~~~~~~~~~~~~~1");
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("~~~~~~~~~~~~~~2");

                        txtRegisterNumber.Focus();
                        txtRegisterNumber.SelectAll();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkEditNational_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                    txtConcentra.Focus();
                    txtConcentra.SelectAll();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboManufacturerName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboManufacturerName.EditValue != null)
                    {
                        btnAdd.Focus();
                    }
                    else
                    {
                        cboManufacturerName.ShowPopup();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spImpPrice_EditValueChanged(object sender, EventArgs e)
        {

            try
            {
                spContractPrice.EditValue = spImpPrice.Value * (1 + spImpVat.Value / 100);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.medicalContractId = 0;
                cboBid.Enabled = true;
                cboSupplier.Enabled = true;
                ReLoadControl();
                ListHisMedicineTypeAllData = null;
                ListHisMaterialTypeAllData = null;
                CreateThreadLoadData();
                this.IsFirstLoadMedicineType = false;
                this.IsFirstLoadMaterialType = false;
                SetDefaultValueControl();
                this.ListMaterialADO = new List<ADO.MetyMatyADO>();
                this.ListMedicineADO = new List<ADO.MetyMatyADO>();
                UpdateGrid();
                ClearForAdd();
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderContract, dxErrorProviderContract);
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderMetyMate, dxErrorProviderMetyMate);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<HIS.UC.MedicineType.ADO.MedicineTypeADO> listMedicineType = new List<HIS.UC.MedicineType.ADO.MedicineTypeADO>();
        List<HIS.UC.MaterialType.ADO.MaterialTypeADO> listMaterialType = new List<HIS.UC.MaterialType.ADO.MaterialTypeADO>();
        List<UC.MedicineType.ADO.MedicineTypeADO> listMedicineType_Bid = new List<UC.MedicineType.ADO.MedicineTypeADO>();
        List<UC.MaterialType.ADO.MaterialTypeADO> listMaterialType_Bid = new List<UC.MaterialType.ADO.MaterialTypeADO>();
        bool IsFirstLoadMedicineType = false;
        bool IsFirstLoadMaterialType = false;
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                long bidId = 0;
                if (cboBid.EditValue != null)
                    bidId = Inventec.Common.TypeConvert.Parse.ToInt64((cboBid.EditValue ?? 0).ToString());
                long supplier = Inventec.Common.TypeConvert.Parse.ToInt64((cboSupplier.EditValue ?? "0").ToString());
                if (DicMedicineStatus == null)
                    DicMedicineStatus = new Dictionary<int, TabLoadStatusADO>();
                if (!DicMedicineStatus.ContainsKey(0))
                    DicMedicineStatus[0] = new TabLoadStatusADO();
                if (DicMedicineStatus[0].bidId == bidId && DicMedicineStatus[0].supplierId == supplier)
                    return;
                DicMedicineStatus[0].bidId = bidId;
                DicMedicineStatus[0].supplierId = supplier;
                if (DicBidMedicineType == null)
                    DicBidMedicineType = new Dictionary<long, List<V_HIS_BID_MEDICINE_TYPE>>();
                if (!DicBidMedicineType.ContainsKey(bidId) && bidId > 0)
                {
                    CommonParam param = new CommonParam();
                    HisBidMedicineTypeViewFilter filter = new HisBidMedicineTypeViewFilter();
                    filter.IS_ACTIVE = 1;
                    if (bidId > 0)
                        filter.BID_ID = bidId;
                    DicBidMedicineType[bidId] = new BackendAdapter(param).Get<List<V_HIS_BID_MEDICINE_TYPE>>("api/HisBidMedicineType/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, SessionManager.ActionLostToken, param);
                }
                if (bidId > 0)
                {
                    var lstBidMedicine = DicBidMedicineType[bidId].Where(o => o.SUPPLIER_ID == supplier).ToList();
                    this.listMedicineType_Bid = new List<HIS.UC.MedicineType.ADO.MedicineTypeADO>();
                    if (lstBidMedicine != null && lstBidMedicine.Count > 0)
                    {
                        int progess = 0;
                        int skip = 0;
                        int count = lstBidMedicine.Count;
                        while (count > 0)
                        {
                            int limit = (count <= 100) ? count : 100;
                            var listSub = lstBidMedicine.Skip(skip).Take(limit).ToList();
                            //
                            foreach (var item in listSub)
                            {
                                var medicineType = this.ListHisMedicineType.Where(o => o.ID == item.MEDICINE_TYPE_ID).FirstOrDefault();
                                if (medicineType == null)
                                    continue;

                                UC.MedicineType.ADO.MedicineTypeADO medicineTypeADO = new UC.MedicineType.ADO.MedicineTypeADO(medicineType);
                                //medicineTypeADO.ID = item.ID;
                                medicineTypeADO.AMOUNT_IN_BID = item.AMOUNT;
                                medicineTypeADO.IMP_PRICE_IN_BID = item.IMP_PRICE;
                                medicineTypeADO.IMP_VAT_RATIO_IN_BID = (item.IMP_VAT_RATIO.HasValue ? item.IMP_VAT_RATIO * 100 : null);
                                medicineTypeADO.AMOUNT_IN_CONTRACT = item.TDL_CONTRACT_AMOUNT;
                                medicineTypeADO.BidGroupCode = item.BID_GROUP_CODE;
                                medicineTypeADO.KeyField = Base.StaticMethod.GetTypeKey(item.MEDICINE_TYPE_ID, item.BID_GROUP_CODE, item.ID);
                                listMedicineType_Bid.Add(medicineTypeADO);
                            }
                            //
                            progess = (int)Math.Floor((decimal)(100 * this.listMedicineType_Bid.Count / lstBidMedicine.Count));
                            backgroundWorker1.ReportProgress(progess);
                            skip += 100;
                            count -= 100;
                        }
                    }
                    else
                    {
                        backgroundWorker1.ReportProgress(100);
                    }
                }
                else
                {
                    if (!IsFirstLoadMedicineType)
                    {
                        this.listMedicineType = new List<HIS.UC.MedicineType.ADO.MedicineTypeADO>();
                        if (ListHisMedicineType != null && ListHisMedicineType.Count > 0)
                            this.listMedicineType.AddRange((from o in ListHisMedicineType select new HIS.UC.MedicineType.ADO.MedicineTypeADO(o)).ToList());
                        //int progess = 0;
                        //int skip = 0;
                        //int count = this.ListHisMedicineType.Count;
                        //while (count > 0)
                        //{
                        //    int limit = (count <= 100) ? count : 100;
                        //    var listSub = this.ListHisMedicineType.Skip(skip).Take(limit).ToList();
                        //    this.listMedicineType.AddRange((from o in listSub select new HIS.UC.MedicineType.ADO.MedicineTypeADO(o)).ToList());
                        //    progess = (int)Math.Floor((decimal)(100 * this.listMedicineType.Count / this.ListHisMedicineType.Count));
                        //    backgroundWorker1.ReportProgress(progess);
                        //    skip += 100;
                        //    count -= 100;
                        //}
                        IsFirstLoadMedicineType = true;
                    }
                    backgroundWorker1.ReportProgress(100);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                long bidId = 0;
                if (cboBid.EditValue != null)
                    bidId = Inventec.Common.TypeConvert.Parse.ToInt64((cboBid.EditValue ?? 0).ToString());
                long supplier = Inventec.Common.TypeConvert.Parse.ToInt64((cboSupplier.EditValue ?? "0").ToString());
                if (DicMaterialStatus == null)
                    DicMaterialStatus = new Dictionary<int, TabLoadStatusADO>();
                if (!DicMaterialStatus.ContainsKey(1))
                    DicMaterialStatus[1] = new TabLoadStatusADO();
                if (DicMaterialStatus[1].bidId == bidId && DicMaterialStatus[1].supplierId == supplier)
                    return;
                DicMaterialStatus[1].bidId = bidId;
                DicMaterialStatus[1].supplierId = supplier;
                if (DicBidMaterialType == null)
                    DicBidMaterialType = new Dictionary<long, List<V_HIS_BID_MATERIAL_TYPE>>();
                if (!DicBidMaterialType.ContainsKey(bidId) && bidId > 0)
                {
                    CommonParam param = new CommonParam();
                    HisBidMaterialTypeViewFilter filter = new HisBidMaterialTypeViewFilter();
                    filter.IS_ACTIVE = 1;
                    if (bidId > 0)
                        filter.BID_ID = bidId;
                    DicBidMaterialType[bidId] = new BackendAdapter(param).Get<List<V_HIS_BID_MATERIAL_TYPE>>("api/HisBidMaterialType/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, SessionManager.ActionLostToken, param);
                }
                if (bidId > 0)
                {
                    var lstBidMaterial = DicBidMaterialType[bidId].Where(o => o.SUPPLIER_ID == supplier).ToList();

                    this.listMaterialType_Bid = new List<HIS.UC.MaterialType.ADO.MaterialTypeADO>();
                    if (lstBidMaterial != null && lstBidMaterial.Count > 0)
                    {
                        int progess = 0;
                        int skip = 0;
                        int count = lstBidMaterial.Count;
                        while (count > 0)
                        {
                            int limit = (count <= 100) ? count : 100;
                            var listSub = lstBidMaterial.Skip(skip).Take(limit).ToList();
                            //
                            foreach (var item in listSub)
                            {
                                var materialType = this.ListHisMaterialType.FirstOrDefault(o => o.ID == item.MATERIAL_TYPE_ID);
                                if (materialType == null)
                                    continue;

                                UC.MaterialType.ADO.MaterialTypeADO materialTypeADO = new UC.MaterialType.ADO.MaterialTypeADO(materialType);
                                materialTypeADO.AMOUNT_IN_BID = item.AMOUNT;
                                materialTypeADO.IMP_PRICE_IN_BID = item.IMP_PRICE;
                                materialTypeADO.IMP_VAT_RATIO_IN_BID = item.IMP_VAT_RATIO.HasValue ? item.IMP_VAT_RATIO * 100 : null;
                                materialTypeADO.AMOUNT_IN_CONTRACT = item.TDL_CONTRACT_AMOUNT;
                                materialTypeADO.BidGroupCode = item.BID_GROUP_CODE;
                                materialTypeADO.KeyField = Base.StaticMethod.GetTypeKey(item.MATERIAL_TYPE_ID ?? 0, item.BID_GROUP_CODE, item.ID);
                                if (materialType.IMP_UNIT_ID.HasValue)
                                {
                                    materialTypeADO.SERVICE_UNIT_NAME = materialType.IMP_UNIT_NAME;
                                }

                                listMaterialType_Bid.Add(materialTypeADO);
                            }

                            progess = (int)Math.Floor((decimal)(100 * this.listMedicineType_Bid.Count / lstBidMaterial.Count));
                            backgroundWorker2.ReportProgress(progess);
                            skip += 100;
                            count -= 100;
                        }

                    }
                    else
                    {
                        backgroundWorker2.ReportProgress(100);
                    }
                }
                else
                {
                    if (!IsFirstLoadMaterialType)
                    {
                        this.listMaterialType = new List<HIS.UC.MaterialType.ADO.MaterialTypeADO>();
                        if (ListHisMaterialType != null && ListHisMaterialType.Count > 0)
                            this.listMaterialType.AddRange((from o in ListHisMaterialType select new HIS.UC.MaterialType.ADO.MaterialTypeADO(o)).ToList());
                        //int progess = 0;
                        //int skip = 0;
                        //int count = this.ListHisMaterialType.Count;
                        //while (count > 0)
                        //{
                        //    int limit = (count <= 100) ? count : 100;
                        //    var listSub = this.ListHisMaterialType.Skip(skip).Take(limit).ToList();
                        //    this.listMaterialType.AddRange((from o in listSub select new HIS.UC.MaterialType.ADO.MaterialTypeADO(o)).ToList());
                        //    progess = (int)Math.Floor((decimal)(100 * this.listMaterialType.Count / this.ListHisMaterialType.Count));
                        //    backgroundWorker2.ReportProgress(progess);
                        //    skip += 100;
                        //    count -= 100;
                        //}
                        IsFirstLoadMaterialType = true;
                    }
                    backgroundWorker2.ReportProgress(100);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                long bidId = 0;
                if (cboBid.EditValue != null)
                    bidId = Inventec.Common.TypeConvert.Parse.ToInt64((cboBid.EditValue ?? 0).ToString());

                if (bidId > 0)
                {
                    this.medicineTypeProcessor.Reload(this.ucMedicineType, this.listMedicineType_Bid);
                }
                else
                {
                    this.medicineTypeProcessor.Reload(this.ucMedicineType, this.listMedicineType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                long bidId = 0;
                if (cboBid.EditValue != null)
                    bidId = Inventec.Common.TypeConvert.Parse.ToInt64((cboBid.EditValue ?? 0).ToString());

                if (bidId > 0)
                {
                    this.materialTypeProcessor.Reload(this.ucMaterialType, this.listMaterialType_Bid);
                }
                else
                {
                    this.materialTypeProcessor.Reload(this.ucMaterialType, this.listMaterialType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnVentureAgreening_Click(object sender, EventArgs e)
        {
            try
            {
                SimpleButton editor = sender as SimpleButton;
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupControlContainer1.ShowPopup(new System.Drawing.Point(buttonPosition.X - 300, buttonPosition.Bottom + 20));

                txtSearchVentureAgreening.Text = "";
                List<HIS_SUPPLIER> dataSupplier = BackendDataWorker.Get<HIS_SUPPLIER>().Where(o => o.IS_ACTIVE == 1).ToList();
                gridControl2.BeginUpdate();
                gridControl2.DataSource = null;
                gridControl2.DataSource = dataSupplier;
                gridControl2.EndUpdate();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearchVentureAgreening_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtSearchVentureAgreening.Text))
                    {
                        List<HIS_SUPPLIER> dataSupplier = BackendDataWorker.Get<HIS_SUPPLIER>().Where(o => o.IS_ACTIVE == 1 &&
                            (o.SUPPLIER_CODE.ToLower().Contains(txtSearchVentureAgreening.Text.ToLower()) ||
                            o.SUPPLIER_NAME.ToLower().Contains(txtSearchVentureAgreening.Text.ToLower()))).ToList();
                        gridControl2.BeginUpdate();
                        gridControl2.DataSource = null;
                        gridControl2.DataSource = dataSupplier;
                        gridControl2.EndUpdate();
                    }
                    else 
                    {
                        List<HIS_SUPPLIER> dataSupplier = BackendDataWorker.Get<HIS_SUPPLIER>().Where(o => o.IS_ACTIVE == 1).ToList();
                        gridControl2.BeginUpdate();
                        gridControl2.DataSource = null;
                        gridControl2.DataSource = dataSupplier;
                        gridControl2.EndUpdate();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void grdVentureAgreening_Click(object sender, EventArgs e)
        {
            try
            {
                    var data = (HIS_SUPPLIER)grdVentureAgreening.GetFocusedRow();
                    if (data != null)
                    {
                        txtVentureAgreening.Text = data.SUPPLIER_NAME;
                        popupControlContainer1.HidePopup();
                    }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            
        }
    }
}
