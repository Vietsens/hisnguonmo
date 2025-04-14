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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using MOS.Filter;

namespace HIS.Desktop.Plugins.BidCreate
{
    public partial class UCBidCreate : HIS.Desktop.Utility.UserControlBase
    {
        public List<HIS_BID_MEDICINE_TYPE> BidMedicineType { get; private set; }
        public List<HIS_BID_MATERIAL_TYPE> BidMaterialType { get; private set; }
        #region Click
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (lciBtnAdd.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never) return;
                positionHandleLeft = -1;
                if (!btnAdd.Enabled && this.ActionType == GlobalVariables.ActionEdit)
                    return;
                btnAdd.Focus();
                if (!dxValidationProviderLeft.Validate()) return;
                if (xtraTabControl1.SelectedTabPageIndex == 0) // thuoc
                {
                    if (!WarningBhytInfo()) return;                   
                    var aMedicineType = this.ListMedicineTypeAdoProcess.FirstOrDefault(
                        o => o.ID == this.medicineType.ID &&
                        o.Type == Base.GlobalConfig.THUOC &&
                        o.SUPPLIER_ID == (long)cboSupplier.EditValue &&
                        o.BID_GROUP_CODE == txtBidGroupCode.Text
                        );
                    if (aMedicineType != null && aMedicineType.ID > 0)
                    {
                        this.ListMedicineTypeAdoProcess.RemoveAll(o => o == aMedicineType);
                        addMedicine();
                    }
                    else addMedicine();
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 1) // vat tu
                {
                    var aMaterialType = this.ListMedicineTypeAdoProcess.FirstOrDefault(o =>
                        o.ID == this.materialType.ID &&
                        o.IsMaterialTypeMap == this.materialType.IsMaterialTypeMap &&
                        o.Type == Base.GlobalConfig.VATTU &&
                        o.SUPPLIER_ID == (long)cboSupplier.EditValue &&
                       o.BID_GROUP_CODE == txtBidGroupCode.Text);
                    if (aMaterialType != null && aMaterialType.ID > 0)
                    {
                        this.ListMedicineTypeAdoProcess.RemoveAll(o => o == aMaterialType);
                        addMaterial();
                    }
                    else addMaterial();
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 2) // Mau
                {
                    var aBloodType = this.ListMedicineTypeAdoProcess.FirstOrDefault(o => o.ID == this.bloodType.ID &&
                        o.Type == Base.GlobalConfig.MAU &&
                        o.SUPPLIER_ID == (long)cboSupplier.EditValue &&
                         o.BID_GROUP_CODE == txtBidGroupCode.Text);
                    if (aBloodType != null && aBloodType.ID > 0)
                    {
                        this.ListMedicineTypeAdoProcess.RemoveAll(o => o == aBloodType);
                        addBlood();
                    }
                    else addBlood();
                }
                gridControlProcess.BeginUpdate();
                gridControlProcess.DataSource = null;
                gridControlProcess.DataSource = this.ListMedicineTypeAdoProcess;
                gridControlProcess.EndUpdate();
                SetValueForAdd();
                ResetLeftControl();
                FocusTab();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleLeft = -1;
                if (lciBtnUpdate.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never) return;
                if (!dxValidationProviderLeft.Validate())
                    return;
                btnUpdate.Focus();
                if (this.medicineType != null)
                {
                    this.ListMedicineTypeAdoProcess.RemoveAll(o => o == this.medicineType);
                    if (this.medicineType.Type == Base.GlobalConfig.THUOC)
                    {
                        if (!WarningBhytInfo()) return;
                        addMedicine();
                    }
                    if (this.medicineType.Type == Base.GlobalConfig.VATTU) addMaterial();
                    if (this.medicineType.Type == Base.GlobalConfig.MAU) addBlood();
                }
                this.ActionType = GlobalVariables.ActionAdd;
                gridControlProcess.BeginUpdate();
                gridControlProcess.DataSource = null;
                gridControlProcess.DataSource = this.ListMedicineTypeAdoProcess;
                gridControlProcess.EndUpdate();
                SetDefaultValueControlLeft();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDiscard_Click(object sender, EventArgs e)
        {
            try
            {
                if (lciBtnDiscard.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never) return;
                this.ActionType = GlobalVariables.ActionAdd;
                SetDefaultValueControlLeft();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            btnSave.Focus();
            gridViewProcess.PostEditor();
            CommonParam paramCommon = new CommonParam();
            try
            {
                BidMaterialType = null;
                BidMedicineType = null;
                this.positionHandleRight = -1;
                if (!getDataForProcess()) return;
                if (!dxValidationProviderRight.Validate())
                    return;
                bool success = false;

                if (this.ActionType == GlobalVariables.ActionAdd && CheckValidDataInGridService(ref paramCommon, ListMedicineTypeAdoProcess))
                {
                    WaitingManager.Show();
                    if (this.bidModel == null ||
                        this.bidModel.HIS_BID_MEDICINE_TYPE == null ||
                        this.bidModel.HIS_BID_MATERIAL_TYPE == null ||
                        this.bidModel.HIS_BID_BLOOD_TYPE == null ||
                        (this.bidModel.HIS_BID_MEDICINE_TYPE.Count <= 0 &&
                        this.bidModel.HIS_BID_MATERIAL_TYPE.Count <= 0 &&
                        this.bidModel.HIS_BID_BLOOD_TYPE.Count <= 0))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show
                            (Resources.ResourceMessage.ThieuTruongDuLieuBatBuoc);
                        return;
                    }

                    Inventec.Common.Logging.LogSystem.Debug("HIS.Desktop.Plugins.BidCreate bidModel: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => bidModel), bidModel));

                    bidPrint = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Post<MOS.EFMODEL.DataModels.HIS_BID>(ApiConsumer.HisRequestUriStore.HIS_BID_CREATE, ApiConsumer.ApiConsumers.MosConsumer, bidModel, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    if (bidPrint != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("RESULT: "+Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => bidPrint), bidPrint));
                        success = true;
                        EnableButton(GlobalVariables.ActionView);
                        txtRegisterNumber.Text = "";
                        txtManufacture.Text = "";
                        txtNationalMainText.Text = "";
                        txtConcentra.Text = "";
                        txtSupplierCode.Text = "";
                        cboSupplier.EditValue = null;
                        cboDosageForm.EditValue = null;
                        cboNational.EditValue = null;
                        cboManufacture.EditValue = null;
                        spinAmount.Value = 0;
                        spinImpPrice.Value = 0;
                        spinImpVat.Value = 0;
                        spinImpMoreRatio.Value = 0;
                        txtBidNumOrder.Text = "";
                        txtBidGroupCode.Text = "";
                        txtBidPackageCode.Text = "";
                        txtMaTT.Text = "";
                        txtTenTT.Text = "";
                        LoadBidMedicineType(bidPrint.ID);
                        LoadBidMaterialType(bidPrint.ID);
                        gridControlProcess.RefreshDataSource();
                    }
                    WaitingManager.Hide();
                }
                else if (this.ActionType == GlobalVariables.ActionEdit)
                {
                    string messageErr = "";
                    if (medicineType.Type == Base.GlobalConfig.THUOC)
                    {
                        messageErr = String.Format(Resources.ResourceMessage.CanhBaoThuoc, medicineType.MEDICINE_TYPE_NAME);
                    }
                    else if (medicineType.Type == Base.GlobalConfig.VATTU)
                    {
                        messageErr = String.Format(Resources.ResourceMessage.CanhBaoVatTu, medicineType.MEDICINE_TYPE_NAME);
                    }
                    else if (medicineType.Type == Base.GlobalConfig.MAU)
                    {
                        messageErr = String.Format(Resources.ResourceMessage.CanhBaoMau, medicineType.MEDICINE_TYPE_NAME);
                    }

                    paramCommon.Messages.Add(String.Format(Resources.ResourceMessage.DangSua, messageErr));
                }
                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, paramCommon, success);
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
        private void LoadBidMedicineType(long bidId)
        {

            try
            {
                CommonParam paramCommon = new CommonParam();
                HisBidMedicineTypeFilter filter = new HisBidMedicineTypeFilter();
                filter.BID_ID = bidId;
                BidMedicineType = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_BID_MEDICINE_TYPE>>("api/HisBidMedicineType/get", ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void LoadBidMaterialType(long bidId)
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                HisBidMaterialTypeFilter filter = new HisBidMaterialTypeFilter();
                filter.BID_ID = bidId;
                BidMaterialType = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_BID_MATERIAL_TYPE>>("api/HisBidMaterialType/get", ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
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
                SetDefaultValueControl();
                EnableButton(this.ActionType);
                VisibleButton(this.ActionType);
                tabMaterial = true;
                tabBlood = true;
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool getDataForProcess()
        {
            bool result = false;
            try
            {
                this.bidModel = new MOS.EFMODEL.DataModels.HIS_BID();
                this.bidModel.HIS_BID_BLOOD_TYPE = new List<MOS.EFMODEL.DataModels.HIS_BID_BLOOD_TYPE>();
                this.bidModel.HIS_BID_MATERIAL_TYPE = new List<MOS.EFMODEL.DataModels.HIS_BID_MATERIAL_TYPE>();
                this.bidModel.HIS_BID_MEDICINE_TYPE = new List<MOS.EFMODEL.DataModels.HIS_BID_MEDICINE_TYPE>();
                this.bidModel.BID_NAME = txtBidName.Text.Trim();
                if (cboBidForm.EditValue != null)
                    this.bidModel.BID_FORM_ID = Int64.Parse(cboBidForm.EditValue.ToString());
                this.bidModel.BID_EXTRA_CODE = txtBID.Text.Trim(); 
                this.bidModel.BID_NUMBER = txtBidNumber.Text.Trim();
                this.bidModel.BID_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboBidType.EditValue.ToString());
                this.bidModel.BID_YEAR = txtBidYear.Text.Trim();
                this.bidModel.BID_APTHAU_CODE = txtBidApThauCode.Text.Trim();
                if (dtFromTime.EditValue != null && dtFromTime.DateTime != DateTime.MinValue && dtToTime.EditValue != null && dtToTime.DateTime != DateTime.MinValue)
                {
                    if (dtToTime.DateTime < dtFromTime.DateTime)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Thời gian kết thúc không thể nhỏ hơn thời gian bắt đầu", "THÔNG BÁO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dtFromTime.EditValue = null;
                        dtToTime.EditValue = null;
                        dtFromTime.Focus();
                        return false;
                    }
                }

                WaitingManager.Hide();
                if (dtFromTime.EditValue != null && dtFromTime.DateTime != DateTime.MinValue)
                    this.bidModel.VALID_FROM_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtFromTime.DateTime);
                else
                    this.bidModel.VALID_FROM_TIME = null;

                if (dtToTime.EditValue != null && dtToTime.DateTime != DateTime.MinValue)
                    this.bidModel.VALID_TO_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtToTime.DateTime) + 235959;
                else
                    this.bidModel.VALID_TO_TIME = null;


                foreach (var item in this.ListMedicineTypeAdoProcess)
                {
                    if (item.Type == Base.GlobalConfig.THUOC)
                    {
                        var bidMedicineType = new MOS.EFMODEL.DataModels.HIS_BID_MEDICINE_TYPE();
                        bidMedicineType.AMOUNT = (decimal)(item.AMOUNT ?? 0);
                        bidMedicineType.IMP_PRICE = (decimal)(item.IMP_PRICE ?? 0);
                        bidMedicineType.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                        bidMedicineType.IMP_MORE_RATIO = item.ImpMoreRatio!=null ? item.ImpMoreRatio/100 : null;
                        bidMedicineType.ADJUST_AMOUNT = item.ADJUST_AMOUNT;
                        bidMedicineType.MEDICINE_TYPE_ID = item.ID;
                        bidMedicineType.BID_NUM_ORDER = item.BID_NUM_ORDER;
                        bidMedicineType.SUPPLIER_ID = (long)(item.SUPPLIER_ID ?? 0);
                        bidMedicineType.BID_GROUP_CODE = item.BID_GROUP_CODE;
                        bidMedicineType.BID_PACKAGE_CODE = item.BID_PACKAGE_CODE;
                        bidMedicineType.EXPIRED_DATE = item.EXPIRED_DATE;
                        bidMedicineType.CONCENTRA = item.CONCENTRA;

                        bidMedicineType.MANUFACTURER_ID = item.MANUFACTURER_ID;
                        bidMedicineType.MEDICINE_REGISTER_NUMBER = item.REGISTER_NUMBER;
                        bidMedicineType.NATIONAL_NAME = item.NATIONAL_NAME;

                        bidMedicineType.HEIN_SERVICE_BHYT_NAME = item.HEIN_SERVICE_BHYT_NAME;
                        bidMedicineType.PACKING_TYPE_NAME = item.PACKING_TYPE_NAME;

                        bidMedicineType.DAY_LIFESPAN = item.DAY_LIFESPAN;
                        bidMedicineType.HOUR_LIFESPAN = item.HOUR_LIFESPAN;
                        bidMedicineType.MONTH_LIFESPAN = item.MONTH_LIFESPAN;

                        bidMedicineType.ACTIVE_INGR_BHYT_NAME = item.ACTIVE_INGR_BHYT_NAME;
                        bidMedicineType.DOSAGE_FORM = item.DOSAGE_FORM;
                        bidMedicineType.MEDICINE_USE_FORM_ID = item.MEDICINE_USE_FORM_ID;

                        this.bidModel.HIS_BID_MEDICINE_TYPE.Add(bidMedicineType);

                    }
                    else if (item.Type == Base.GlobalConfig.VATTU)
                    {
                        var bidMaterialType = new MOS.EFMODEL.DataModels.HIS_BID_MATERIAL_TYPE();
                        bidMaterialType.AMOUNT = (decimal)(item.AMOUNT ?? 0);
                        bidMaterialType.IMP_PRICE = (decimal)(item.IMP_PRICE ?? 0);
                        bidMaterialType.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                        bidMaterialType.IMP_MORE_RATIO = item.ImpMoreRatio != null ? item.ImpMoreRatio/100 : null;
                        bidMaterialType.ADJUST_AMOUNT = item.ADJUST_AMOUNT;
                        if (item.IsMaterialTypeMap)
                        {
                            bidMaterialType.MATERIAL_TYPE_MAP_ID = item.ID;
                        }
                        else
                        {
                            bidMaterialType.MATERIAL_TYPE_ID = item.ID;
                        }
                        bidMaterialType.BID_NUM_ORDER = item.BID_NUM_ORDER;
                        bidMaterialType.SUPPLIER_ID = (long)(item.SUPPLIER_ID ?? 0);
                        bidMaterialType.BID_GROUP_CODE = item.BID_GROUP_CODE;
                        bidMaterialType.BID_PACKAGE_CODE = item.BID_PACKAGE_CODE;
                        bidMaterialType.EXPIRED_DATE = item.EXPIRED_DATE;

                        bidMaterialType.CONCENTRA = item.CONCENTRA;

                        bidMaterialType.MANUFACTURER_ID = item.MANUFACTURER_ID;
                        bidMaterialType.NATIONAL_NAME = item.NATIONAL_NAME;

                        bidMaterialType.BID_MATERIAL_TYPE_CODE = item.BID_MATERIAL_TYPE_CODE;
                        bidMaterialType.BID_MATERIAL_TYPE_NAME = item.BID_MATERIAL_TYPE_NAME;
                        bidMaterialType.JOIN_BID_MATERIAL_TYPE_CODE = item.JOIN_BID_MATERIAL_TYPE_CODE;

                        bidMaterialType.DAY_LIFESPAN = item.DAY_LIFESPAN;
                        bidMaterialType.HOUR_LIFESPAN = item.HOUR_LIFESPAN;
                        bidMaterialType.MONTH_LIFESPAN = item.MONTH_LIFESPAN;
                        bidMaterialType.INFORMATION_BID = item.INFORMATION_BID;
                        this.bidModel.HIS_BID_MATERIAL_TYPE.Add(bidMaterialType);
                    }
                    else if (item.Type == Base.GlobalConfig.MAU)
                    {
                        var bidBloodType = new MOS.EFMODEL.DataModels.HIS_BID_BLOOD_TYPE();
                        bidBloodType.AMOUNT = (decimal)(item.AMOUNT ?? 0);
                        bidBloodType.IMP_PRICE = (decimal)(item.IMP_PRICE ?? 0);
                        bidBloodType.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                        bidBloodType.BLOOD_TYPE_ID = item.ID;
                        bidBloodType.BID_NUM_ORDER = item.BID_NUM_ORDER;
                        bidBloodType.SUPPLIER_ID = (long)(item.SUPPLIER_ID ?? 0);

                        this.bidModel.HIS_BID_BLOOD_TYPE.Add(bidBloodType);
                    }
                }
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private bool CheckValidDataInGridService(ref CommonParam param, List<ADO.MedicineTypeADO> MedicineCheckeds__Send)
        {
            bool valid = true;
            try
            {
                if (MedicineCheckeds__Send != null && MedicineCheckeds__Send.Count > 0)
                {
                    bool IsFirst = true;
                    int count = 0;
                    foreach (var item in MedicineCheckeds__Send)
                    {
                        string messageErr = "";
                        bool result = true;
                        if (item.Type == Base.GlobalConfig.THUOC)
                        {
                            messageErr = String.Format(Resources.ResourceMessage.CanhBaoThuoc, item.MEDICINE_TYPE_NAME);
                        }
                        else if (item.Type == Base.GlobalConfig.VATTU)
                        {
                            messageErr = String.Format(Resources.ResourceMessage.CanhBaoVatTu, item.MEDICINE_TYPE_NAME);
                        }
                        else if (item.Type == Base.GlobalConfig.MAU)
                        {
                            messageErr = String.Format(Resources.ResourceMessage.CanhBaoMau, item.MEDICINE_TYPE_NAME);
                        }

                        if (item.SUPPLIER_ID == null || item.SUPPLIER_ID <= 0)
                        {
                            result = false;
                            messageErr +=" " + Resources.ResourceMessage.KhongCoNhaCungCap;
                        }

                        if (item.AMOUNT < 0)
                        {
                            result = false;
                            messageErr += " " + Resources.ResourceMessage.SoLuongKhongDuocAm;
                        }
                        if (item.AMOUNT == 0 && (item.ADJUST_AMOUNT == null || item.ADJUST_AMOUNT == 0))
                        {
                            result = false;
                            messageErr += "bắt buộc phải nhập số lượng điều tiết";
                            if (IsFirst)
                            {
                                gridViewProcess.FocusedColumn = GvProcess_GcAdjustAmount;
                                gridViewProcess.FocusedRowHandle = count;
                                IsFirst = false;
                            }
                        }
                        count++;

                        if (!String.IsNullOrEmpty(item.BID_NUM_ORDER) && item.BID_NUM_ORDER.Length > 50)
                        {
                            result = false;
                            messageErr += " " + Resources.ResourceMessage.SttThauQuaDai;
                        }

                        if (!String.IsNullOrWhiteSpace(item.BID_GROUP_CODE) && item.BID_GROUP_CODE.Length > 4)
                        {
                            result = false;
                            messageErr += " " + Resources.ResourceMessage.NhomThauQuaDai;
                        }

                        if (!String.IsNullOrWhiteSpace(item.BID_PACKAGE_CODE) && item.Type != Base.GlobalConfig.VATTU && item.BID_PACKAGE_CODE.Length > 4)
                        {
                            result = false;
                            messageErr += " " + Resources.ResourceMessage.GoiThauQuaDai;
                        }

                        if (!String.IsNullOrWhiteSpace(item.BID_PACKAGE_CODE) && item.Type == Base.GlobalConfig.VATTU && item.BID_PACKAGE_CODE.Length > 4)
                        {
                            result = false;
                            messageErr += " mã gói thầu dài hơn 4 ký tự";
                        }

                        var listItem = MedicineCheckeds__Send.Where(o => o.MEDICINE_TYPE_CODE == item.MEDICINE_TYPE_CODE).ToList();
                        if (listItem != null && listItem.Count > 1)
                        {
                            foreach (var i in listItem)
                            {
                                if (i.SUPPLIER_ID == item.SUPPLIER_ID && i.IdRow != item.IdRow 
                                    && i.BID_GROUP_CODE == item.BID_GROUP_CODE
                                    )
                                {
                                    result = false;
                                    messageErr += " " + Resources.ResourceMessage.BiTrung;
                                    break;
                                }
                            }
                        }

                        if (!result)
                        {
                            param.Messages.Add(messageErr);
                        }
                    }
                }
                else
                {
                    param.Messages.Add(Resources.ResourceMessage.ThieuTruongDuLieuBatBuoc);
                }

                if (param.Messages.Count > 0)
                {
                    param.Messages = param.Messages.Distinct().ToList();
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                valid = false;
            }
            return valid;
        }
        #endregion

        #region Delete edit
        private void cboSupplier_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    //cboSupplier.Properties.Buttons[1].Visible = false;
                    cboSupplier.EditValue = null;
                    txtSupplierCode.Text = "";
                    txtSupplierCode.Focus();
                    txtSupplierCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonDelete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (ADO.MedicineTypeADO)gridViewProcess.GetFocusedRow();
                idRow = row.IdRow;
                foreach (var item in this.ListMedicineTypeAdoProcess)
                {
                    if (idRow == item.IdRow)
                    {
                        this.ListMedicineTypeAdoProcess.RemoveAll(o => o.IdRow == idRow);
                        idRow = -1;
                        break;
                    }
                }
                ResetLeftControl();
                gridControlProcess.BeginUpdate();
                gridControlProcess.DataSource = null;
                gridControlProcess.DataSource = this.ListMedicineTypeAdoProcess;
                gridControlProcess.EndUpdate();
                SetDefaultValueControlLeft();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (ADO.MedicineTypeADO)gridViewProcess.GetFocusedRow();
                if (row != null)
                {
                    this.ActionType = GlobalVariables.ActionEdit;
                    VisibleButton(this.ActionType);
                    ResetLeftControl();
                    this.medicineType = row;

                    txtRegisterNumber.Text = row.REGISTER_NUMBER;
                    txtConcentra.Text = row.CONCENTRA;
                    txtManufacture.Text = row.MANUFACTURER_NAME;

                    var National = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get
                        <SDA.EFMODEL.DataModels.SDA_NATIONAL>().Where
                        (o => o.NATIONAL_NAME == row.NATIONAL_NAME).ToList();
                    if (National != null && National.Count > 0)
                    {
                        txtNationalMainText.Text = National[0].NATIONAL_NAME;
                        cboNational.EditValue = National[0].ID;
                        txtNationalMainText.Visible = false;
                        cboNational.Visible = true;
                        chkEditNational.CheckState = CheckState.Unchecked;
                    }
                    else
                    {
                        txtNationalMainText.Text = row.NATIONAL_NAME;
                        cboNational.EditValue = null;
                        txtNationalMainText.Visible = true;
                        cboNational.Visible = false;
                        chkEditNational.CheckState = CheckState.Checked;
                    }
                    cboManufacture.EditValue = row.MANUFACTURER_ID;
                    if (row.Type == Base.GlobalConfig.THUOC)
                    {
                        EnableLeftControl(true);
                        xtraTabControl1.SelectedTabPageIndex = 0;
                        this.medicineType = row;
                        txtActiveBhyt.Text = row.ACTIVE_INGR_BHYT_NAME ?? "";
                        //cboDosageForm.EditValue = row.DOSAGE_FORM ?? "";
                        var dosageItem = dataDosageForm.FirstOrDefault(o => o.DOSAGE_FORM_NAME == row.DOSAGE_FORM);
                        if (dosageItem != null)
                        {
                            cboDosageForm.EditValue = dosageItem.ID;
                        }
                        else
                        {
                            cboDosageForm.EditValue = null;
                        }

                        cboMediUserForm.EditValue = row.MEDICINE_USE_FORM_ID;
                        txtTenBHYT.Text = row.HEIN_SERVICE_BHYT_NAME ?? "";
                        txtQCĐG.Text = row.PACKING_TYPE_NAME ?? "";
                        cboInformationBid.SelectedIndex = -1;
                        layoutControlItem19.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                    else if (row.Type == Base.GlobalConfig.VATTU)
                    {
                        EnableLeftControl(true);
                        txtRegisterNumber.Enabled = false;
                        xtraTabControl1.SelectedTabPageIndex = 1;
                        if (this.materialType == null)
                            this.materialType = new ADO.MaterialTypeADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MaterialTypeADO>(this.materialType, row);
                        this.materialType.MATERIAL_TYPE_CODE = row.MEDICINE_TYPE_CODE;
                        this.materialType.MATERIAL_TYPE_NAME = row.MEDICINE_TYPE_NAME;
                        txtMaDT.Text = row.JOIN_BID_MATERIAL_TYPE_CODE ?? "";
                        txtMaTT.Text = row.BID_MATERIAL_TYPE_CODE ?? "";
                        txtTenTT.Text = row.BID_MATERIAL_TYPE_NAME ?? "";
                        layoutControlItem19.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        layoutControlItem19.Size = lciMaDT.Size;
                        cboInformationBid.SelectedIndex = (int)(row.INFORMATION_BID ?? 0) - 1;
                    }
                    else if (row.Type == Base.GlobalConfig.MAU)
                    {
                        cboInformationBid.SelectedIndex = -1;
                        layoutControlItem19.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        EnableLeftControl(false);
                        xtraTabControl1.SelectedTabPageIndex = 2;
                        if (this.bloodType == null)
                            this.bloodType = new ADO.BloodTypeADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<ADO.BloodTypeADO>(this.bloodType, row);
                        this.bloodType.BLOOD_TYPE_CODE = row.MEDICINE_TYPE_CODE;
                        this.bloodType.BLOOD_TYPE_NAME = row.MEDICINE_TYPE_NAME;
                    }
                    var supplier = Base.GlobalConfig.ListSupplier.FirstOrDefault(o => o.ID == row.SUPPLIER_ID);
                    Base.GlobalConfig.ListSupplier.Any(o => o.ID > 0);
                    if (supplier != null)
                    {
                        txtSupplierCode.Text = supplier.SUPPLIER_CODE;
                        cboSupplier.EditValue = supplier.ID;
                        cboSupplier.Properties.Buttons[1].Visible = true;        
                    }
                    else
                    {
                        txtSupplierCode.Text = "";
                        cboSupplier.EditValue = null;
                        //cboManufacture.Properties.Buttons[1].Visible = false;
                    }
                    spinAmount.EditValue = row.AMOUNT;
                    spinImpPrice.EditValue = row.IMP_PRICE;
                    spinImpVat.EditValue = row.IMP_VAT_RATIO * 100;
                    spinImpMoreRatio.EditValue = row.ImpMoreRatio;
                    txtBidNumOrder.Text = row.BID_NUM_ORDER;
                    txtBidGroupCode.Text = row.BID_GROUP_CODE;
                    txtBidPackageCode.Text = row.BID_PACKAGE_CODE;

                    if (row.EXPIRED_DATE.HasValue)
                    {
                        DtExpiredDate.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(row.EXPIRED_DATE.Value);
                    }
                    LogSystem.Debug(LogUtil.TraceData("Row", row));
                    if (row.DAY_LIFESPAN.HasValue)
                    {
                        spinDayLifeSpan.EditValue = row.DAY_LIFESPAN.Value;
                    }
                    else
                    {
                        spinDayLifeSpan.EditValue = null;
                    }
                    if (row.MONTH_LIFESPAN.HasValue)
                    {
                        spinMonthLifeSpan.EditValue = row.MONTH_LIFESPAN.Value;
                    }
                    else
                    {
                        spinMonthLifeSpan.EditValue = null;
                    }
                    if (row.HOUR_LIFESPAN.HasValue)
                    {
                        spinHourLifeSpan.EditValue = row.HOUR_LIFESPAN.Value;
                    }
                    else
                    {
                        spinHourLifeSpan.EditValue = null;
                    }

                    spinAmount.Focus();
                    spinAmount.SelectAll();
                    dxValidationProviderLeft.RemoveControlError(spinImpPrice);
                    dxValidationProviderLeft.RemoveControlError(spinAmount);
                    dxValidationProviderLeft.RemoveControlError(spinImpVat);
                    dxValidationProviderLeft.RemoveControlError(txtBidNumOrder);
                    dxValidationProviderLeft.RemoveControlError(cboSupplier);
                    dxValidationProviderLeft.RemoveControlError(cboNational);
                    dxValidationProviderLeft.RemoveControlError(cboManufacture);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Enter
        private void spinAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinImpPrice.Focus();
                    spinImpPrice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinImpPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinImpVat.Focus();
                    spinImpVat.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinImpVat_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBidNumOrder.Focus();
                    txtBidNumOrder.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBidNumOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSupplierCode.Focus();
                    txtSupplierCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSupplierCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtSupplierCode.Text.Trim()))
                    {
                        string code = txtSupplierCode.Text.Trim().ToLower();
                        var listData = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SUPPLIER>().Where(o => o.SUPPLIER_CODE.ToLower().Contains(code)).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.SUPPLIER_CODE.ToLower() == code).ToList() : listData) : null;
                        if (result != null && result.Count > 0)
                        {
                            showCbo = false;
                            txtSupplierCode.Text = result.First().SUPPLIER_CODE;
                            cboSupplier.EditValue = result.First().ID;
                            SendKeys.Send("{TAB}");
                            SendKeys.Send("{TAB}");
                        }
                    }
                    if (showCbo)
                    {
                        cboSupplier.Focus();
                        cboSupplier.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSupplier_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboSupplier.EditValue != null)
                    {
                        var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get
                            <MOS.EFMODEL.DataModels.HIS_SUPPLIER>().FirstOrDefault
                            (o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboSupplier.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtSupplierCode.Text = data.SUPPLIER_CODE;
                            cboSupplier.Properties.Buttons[1].Visible = true;
                            SendKeys.Send("{TAB}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtBidName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBidNumber.Focus();
                    txtBidNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBidNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboBidType.Focus();
                    cboBidType.ShowPopup();
                }
                dxValidationProviderRight.RemoveControlError(txtBidNumber);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBidType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBidYear.Focus();
                    txtBidYear.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBidType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboBidType.EditValue != null)
                    {
                        var data = bidTypes.FirstOrDefault
                            (o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboBidType.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtBidYear.Focus();
                            txtBidYear.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Add
        private void addMedicine()
        {
            try
            {
                this.medicineType.IMP_PRICE = spinImpPrice.Value;
                this.medicineType.BID_NUM_ORDER = txtBidNumOrder.Text;
                this.medicineType.IMP_VAT_RATIO = spinImpVat.Value / 100;
                this.medicineType.ImpVatRatio = spinImpVat.Value;
                this.medicineType.ImpMoreRatio = spinImpMoreRatio.Value;
                this.medicineType.AMOUNT = spinAmount.Value;
                this.medicineType.BID_GROUP_CODE = txtBidGroupCode.Text;
                this.medicineType.BID_PACKAGE_CODE = txtBidPackageCode.Text;
                this.medicineType.HEIN_SERVICE_BHYT_NAME = txtTenBHYT.Text.Trim();
                this.medicineType.PACKING_TYPE_NAME = txtQCĐG.Text.Trim();
                this.medicineType.ACTIVE_INGR_BHYT_NAME = txtActiveBhyt.Text.Trim();
                //this.medicineType.DOSAGE_FORM = txtDosageForm.Text.Trim();
                if (cboMediUserForm.EditValue != null)
                {
                    HIS_MEDICINE_USE_FORM useForm = BackendDataWorker.Get<HIS_MEDICINE_USE_FORM>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboMediUserForm.EditValue));
                    if (useForm != null)
                    {
                        this.medicineType.MEDICINE_USE_FORM_ID = useForm.ID;
                        this.medicineType.MEDICINE_USE_FORM_CODE = useForm.MEDICINE_USE_FORM_CODE;
                        this.medicineType.MEDICINE_USE_FORM_NAME = useForm.MEDICINE_USE_FORM_NAME;
                    }
                    else
                    {
                        this.medicineType.MEDICINE_USE_FORM_ID = null;
                        this.medicineType.MEDICINE_USE_FORM_CODE = null;
                        this.medicineType.MEDICINE_USE_FORM_NAME = null;
                    }
                }
                else
                {
                    this.medicineType.MEDICINE_USE_FORM_ID = null;
                    this.medicineType.MEDICINE_USE_FORM_CODE = null;
                    this.medicineType.MEDICINE_USE_FORM_NAME = null;
                }

                if (DtExpiredDate.EditValue != null)
                {
                    if (DtExpiredDate.DateTime < DateTime.Today)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.HanSuDungKhongDuocNhoHonNgayHienTai, Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                    }
                    this.medicineType.EXPIRED_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DtExpiredDate.DateTime);
                }
                else
                {
                    this.medicineType.EXPIRED_DATE = null;
                }

                this.medicineType.IdRow = setIdRow(this.ListMedicineTypeAdoProcess);
                if (cboSupplier.EditValue != null)
                {
                    this.medicineType.SUPPLIER_ID = Inventec.Common.TypeConvert.Parse.ToInt32(cboSupplier.EditValue.ToString());
                    this.medicineType.SUPPLIER_NAME = cboSupplier.Text.Trim();
                }
                else
                    this.medicineType.SUPPLIER_ID = null;

                Inventec.Common.Logging.LogSystem.Debug("txtNationalMainText.Text " + txtNationalMainText.Text);
                this.medicineType.NATIONAL_NAME = txtNationalMainText.Text.Trim();
                Inventec.Common.Logging.LogSystem.Debug("txtNationalMainText.Text.Trim() " + txtNationalMainText.Text.Trim());

                if (cboManufacture.EditValue != null)
                {
                    this.medicineType.MANUFACTURER_ID = Inventec.Common.TypeConvert.Parse.ToInt32(cboManufacture.EditValue.ToString());
                    this.medicineType.MANUFACTURER_NAME = cboManufacture.Text.Trim();
                }
                else
                {
                    this.medicineType.MANUFACTURER_ID = null;
                    this.medicineType.MANUFACTURER_NAME = "";
                }

                this.medicineType.CONCENTRA = txtConcentra.Text.Trim();
                this.medicineType.REGISTER_NUMBER = txtRegisterNumber.Text.Trim();

                if (spinHourLifeSpan.EditValue != null)
                {
                    this.medicineType.HOUR_LIFESPAN = (long)spinHourLifeSpan.Value;
                }
                else
                {
                    this.medicineType.HOUR_LIFESPAN = null;
                }

                if (spinDayLifeSpan.EditValue != null)
                {
                    this.medicineType.DAY_LIFESPAN = (long)spinDayLifeSpan.Value;
                }
                else
                {
                    this.medicineType.DAY_LIFESPAN = null;
                }

                if (spinMonthLifeSpan.EditValue != null)
                {
                    this.medicineType.MONTH_LIFESPAN = (long)spinMonthLifeSpan.Value;
                }
                else
                {
                    this.medicineType.MONTH_LIFESPAN = null;
                }
                if (cboDosageForm.EditValue != null)
                {
                    this.medicineType.DOSAGE_FORM = dataDosageForm.FirstOrDefault(o => o.ID == (long)cboDosageForm.EditValue).DOSAGE_FORM_NAME;                        
                }
                else
                {
                    cboDosageForm.EditValue = null;
                    this.medicineType.DOSAGE_FORM = null;          
                }

                this.ListMedicineTypeAdoProcess.Add(this.medicineType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void addMaterial()
        {
            try
            {
                this.materialType.IMP_PRICE = spinImpPrice.Value;
                this.materialType.BID_NUM_ORDER = txtBidNumOrder.Text;
                this.materialType.IMP_VAT_RATIO = spinImpVat.Value / 100;
                this.materialType.AMOUNT = spinAmount.Value;
                ADO.MedicineTypeADO aMedicineSdo = new ADO.MedicineTypeADO();
                AutoMapper.Mapper.CreateMap<ADO.MaterialTypeADO, ADO.MedicineTypeADO>();
                aMedicineSdo = AutoMapper.Mapper.Map<ADO.MaterialTypeADO, ADO.MedicineTypeADO>(this.materialType);
                aMedicineSdo.ImpVatRatio = spinImpVat.Value;
                aMedicineSdo.ImpMoreRatio = spinImpMoreRatio.Value;
                aMedicineSdo.MEDICINE_TYPE_CODE = this.materialType.MATERIAL_TYPE_CODE;
                aMedicineSdo.MEDICINE_TYPE_NAME = this.materialType.MATERIAL_TYPE_NAME;
                aMedicineSdo.BID_GROUP_CODE = txtBidGroupCode.Text;
                aMedicineSdo.BID_PACKAGE_CODE = txtBidPackageCode.Text;
                aMedicineSdo.BID_MATERIAL_TYPE_CODE = txtMaTT.Text.Trim();
                aMedicineSdo.BID_MATERIAL_TYPE_NAME = txtTenTT.Text.Trim();
                aMedicineSdo.JOIN_BID_MATERIAL_TYPE_CODE = txtMaDT.Text.Trim();
                aMedicineSdo.IdRow = setIdRow(this.ListMedicineTypeAdoProcess);
                aMedicineSdo.INFORMATION_BID = this.cboInformationBid.SelectedIndex != -1 ? (long?)(this.cboInformationBid.SelectedIndex + 1) : null;

                if (DtExpiredDate.EditValue != null)
                {
                    if (DtExpiredDate.DateTime < DateTime.Today)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.HanSuDungKhongDuocNhoHonNgayHienTai, Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                    }
                    aMedicineSdo.EXPIRED_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DtExpiredDate.DateTime);
                }
                else
                {
                    aMedicineSdo.EXPIRED_DATE = null;
                }

                if (cboSupplier.EditValue != null)
                {
                    aMedicineSdo.SUPPLIER_ID = Inventec.Common.TypeConvert.Parse.ToInt32(cboSupplier.EditValue.ToString());
                    aMedicineSdo.SUPPLIER_NAME = cboSupplier.Text.Trim();
                }
                else
                    aMedicineSdo.SUPPLIER_ID = null;

                aMedicineSdo.NATIONAL_NAME = txtNationalMainText.Text.Trim();

                if (cboManufacture.EditValue != null)
                {
                    aMedicineSdo.MANUFACTURER_ID = Inventec.Common.TypeConvert.Parse.ToInt32(cboManufacture.EditValue.ToString());
                    aMedicineSdo.MANUFACTURER_NAME = cboManufacture.Text.Trim();
                }
                else
                {
                    aMedicineSdo.MANUFACTURER_ID = null;
                    aMedicineSdo.MANUFACTURER_NAME = "";
                }
                aMedicineSdo.CONCENTRA = txtConcentra.Text.Trim();


                if (spinHourLifeSpan.EditValue != null)
                {
                    aMedicineSdo.HOUR_LIFESPAN = (long)spinHourLifeSpan.Value;
                }
                else
                {
                    aMedicineSdo.HOUR_LIFESPAN = null;
                }

                if (spinDayLifeSpan.EditValue != null)
                {
                    aMedicineSdo.DAY_LIFESPAN = (long)spinDayLifeSpan.Value;
                }
                else
                {
                    aMedicineSdo.DAY_LIFESPAN = null;
                }

                if (spinMonthLifeSpan.EditValue != null)
                {
                    aMedicineSdo.MONTH_LIFESPAN = (long)spinMonthLifeSpan.Value;
                }
                else
                {
                    aMedicineSdo.MONTH_LIFESPAN = null;
                }

                this.ListMedicineTypeAdoProcess.Add(aMedicineSdo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void addBlood()
        {
            try
            {
                this.bloodType.IMP_PRICE = spinImpPrice.Value;
                this.bloodType.BID_NUM_ORDER = txtBidNumOrder.Text;
                this.bloodType.IMP_VAT_RATIO = spinImpVat.Value / 100;
                this.bloodType.AMOUNT = spinAmount.Value;
                ADO.MedicineTypeADO aMedicineSdo = new ADO.MedicineTypeADO();
                AutoMapper.Mapper.CreateMap<ADO.BloodTypeADO, ADO.MedicineTypeADO>();
                aMedicineSdo = AutoMapper.Mapper.Map<ADO.BloodTypeADO, ADO.MedicineTypeADO>(this.bloodType);
                aMedicineSdo.ImpVatRatio = spinImpVat.Value;
                aMedicineSdo.MEDICINE_TYPE_CODE = this.bloodType.BLOOD_TYPE_CODE;
                aMedicineSdo.MEDICINE_TYPE_NAME = this.bloodType.BLOOD_TYPE_NAME;
                aMedicineSdo.BID_GROUP_CODE = txtBidGroupCode.Text;
                aMedicineSdo.BID_PACKAGE_CODE = txtBidPackageCode.Text;
                aMedicineSdo.IdRow = setIdRow(this.ListMedicineTypeAdoProcess);
                if (cboSupplier.EditValue != null)
                {
                    aMedicineSdo.SUPPLIER_ID = Inventec.Common.TypeConvert.Parse.ToInt32(cboSupplier.EditValue.ToString());
                    aMedicineSdo.SUPPLIER_NAME = cboSupplier.Text.Trim();
                }
                else
                    aMedicineSdo.SUPPLIER_ID = null;

                this.ListMedicineTypeAdoProcess.Add(aMedicineSdo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private double setIdRow(List<ADO.MedicineTypeADO> medicineTypes)
        {
            double currentIdRow = 0;
            try
            {
                if (medicineTypes != null && medicineTypes.Count > 0)
                {
                    var maxIdRow = medicineTypes.Max(o => o.IdRow);
                    currentIdRow = ++maxIdRow;
                }
                else
                {
                    currentIdRow = 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return currentIdRow;
        }
        #endregion

        bool WarningBhytInfo()
        {
            bool valid = true;
            try
            {
                if (this.medicineType != null && this.medicineType.IS_BUSINESS != (short)1)
                {
                    List<string> fields = new List<string>();
                    Control focusControl = null;
                    if (String.IsNullOrWhiteSpace(txtRegisterNumber.Text))
                    {
                        fields.Add(Resources.ResourceMessage.SoDangKy);
                        if (focusControl == null) focusControl = txtRegisterNumber;
                    }
                    if (String.IsNullOrWhiteSpace(txtQCĐG.Text))
                    {
                        fields.Add(Resources.ResourceMessage.QuyCachDongGoi);
                        if (focusControl == null) focusControl = txtQCĐG;
                    }
                    if (String.IsNullOrWhiteSpace(txtTenBHYT.Text))
                    {
                        fields.Add(Resources.ResourceMessage.TenBHYT);
                        if (focusControl == null) focusControl = txtTenBHYT;
                    }
                    if (String.IsNullOrWhiteSpace(txtActiveBhyt.Text))
                    {
                        fields.Add(Resources.ResourceMessage.HoatChat);
                        if (focusControl == null) focusControl = txtActiveBhyt;
                    }
                    if (cboMediUserForm.EditValue == null)
                    {
                        fields.Add(Resources.ResourceMessage.DuongDung);
                        if (focusControl == null) focusControl = cboMediUserForm;
                    }
                    if(cboDosageForm.EditValue == null)
                    {
                        fields.Add(Resources.ResourceMessage.Dangbaoche);
                        if (focusControl == null) focusControl = cboDosageForm;
                    }
                    if (fields.Count > 0)
                    {
                        if (XtraMessageBox.Show(String.Format(Resources.ResourceMessage.BanChuaNhapCacTruongMuonTiepTuc, string.Join(", ", fields)), Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo, DevExpress.Utils.DefaultBoolean.True) != DialogResult.Yes)
                        {
                            if (focusControl != null) focusControl.Focus();
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }
    }
}
