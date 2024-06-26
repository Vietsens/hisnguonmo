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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.MedicineTypeInStock;
using HIS.UC.MaterialTypeInStock;
using HIS.UC.ExpMestMedicineGrid;
using HIS.UC.ExpMestMaterialGrid;
using HIS.UC.ExpMestMedicineGrid.ADO;
using HIS.UC.ExpMestMaterialGrid.ADO;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors.Controls;
using MOS.SDO;
using MOS.Filter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.ExpMestSaleCreateV2.ADO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Plugins.ExpMestSaleCreateV2.Validation;
using DevExpress.Utils.Menu;
using Inventec.Common.Adapter;
using HIS.Desktop.Plugins.ExpMestSaleCreateV2.Base;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Controls.Session;
using ACS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.ExpMestSaleCreateV2.Config;

namespace HIS.Desktop.Plugins.ExpMestSaleCreateV2
{
    public partial class UCExpMestSaleCreateV2 : UserControl
    {
        private void ReleaseAll()
        {
            try
            {
                if (dicMediMateAdo != null && dicMediMateAdo.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    bool releaseMedicineAll = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(RequestUriStore.HIS_MEDICINE_BEAN__RELEASEBEANALL, ApiConsumers.MosConsumer, this.clientSessionKey, param);
                    if (!releaseMedicineAll)
                    {
                        Inventec.Common.Logging.LogSystem.Error("Release Medicine All False ____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dicMediMateAdo.Values), dicMediMateAdo.Values));
                    }

                    bool releaseMaterialAll = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(RequestUriStore.HIS_MATERIAL_BEAN__RELEASEBEANALL, ApiConsumers.MosConsumer, this.clientSessionKey, param);
                    if (!releaseMaterialAll)
                    {
                        Inventec.Common.Logging.LogSystem.Error("Release Medicine All False ____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dicMediMateAdo.Values), dicMediMateAdo.Values));
                    }
                }

                this.clientSessionKey = Guid.NewGuid().ToString();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessSave(ref bool success, ref CommonParam param)
        {
            try
            {
                success = false;
                if (dicMediMateAdo != null)
                {
                    if (ExistServiceNotInStock() || ExistServiceExceedsAvailable() || !CheckValiDiscount() || !CheckPatientDob())
                        return;
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dicMediMateAdo), dicMediMateAdo));
                HisExpMestSaleSDO hisExpMestSaleSDO = new HisExpMestSaleSDO();
                InitDataToSaleCreate(ref hisExpMestSaleSDO);

                if (moduleAction == GlobalDataStore.ModuleAction.EDIT && isShowMessUpdate)
                {
                    DialogResult myResult;
                    myResult = MessageBox.Show("Bạn có muốn sửa thông tin phiếu xuất bán không?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (myResult != DialogResult.OK)
                    {
                        return;
                    }
                }

                string uriRequest = moduleAction == GlobalDataStore.ModuleAction.EDIT ? RequestUriStore.HIS_EXP_MEST__SALE_UPDATE : RequestUriStore.HIS_EXP_MEST__SALE_CREATE;
                WaitingManager.Show();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisExpMestSaleSDO), hisExpMestSaleSDO));
                expMestResult = new BackendAdapter(param)
                    .Post<HisExpMestResultSDO>(uriRequest, ApiConsumers.MosConsumer, hisExpMestSaleSDO, param);
                WaitingManager.Hide();

                if (expMestResult != null)
                {
                    LoadDataToComboAccountBook();
                    success = true;
                    isShowMessUpdate = true;
                    if (expMestResult.ExpMest != null)
                        this.expMestId = expMestResult.ExpMest.ID;
                    this.SetLabelSave(GlobalDataStore.ModuleAction.EDIT);
                    if (expMestResult.ExpMedicines != null && expMestResult.ExpMedicines.Count > 0)
                    {
                        var expMedicineGroups = expMestResult.ExpMedicines.GroupBy(o => o.TDL_MEDICINE_TYPE_ID);
                        foreach (var expMedicineGroup in expMedicineGroups)
                        {
                            if (dicMediMateAdo.ContainsKey(expMedicineGroup.First().TDL_MEDICINE_TYPE_ID ?? 0))
                            {
                                dicMediMateAdo[expMedicineGroup.First().TDL_MEDICINE_TYPE_ID ?? 0].ExpMestDetailId = expMedicineGroup.FirstOrDefault().ID;
                            }
                        }
                    }
                    if (expMestResult.ExpMaterials != null && expMestResult.ExpMaterials.Count > 0)
                    {
                        var expMaterialGroups = expMestResult.ExpMaterials.GroupBy(o => o.TDL_MATERIAL_TYPE_ID);
                        foreach (var expMaterialGroup in expMaterialGroups)
                        {
                            if (dicMediMateAdo.ContainsKey(expMaterialGroup.First().TDL_MATERIAL_TYPE_ID ?? 0))
                            {
                                dicMediMateAdo[expMaterialGroup.First().TDL_MATERIAL_TYPE_ID ?? 0].ExpMestDetailId = expMaterialGroup.FirstOrDefault().ID;
                            }
                        }
                    }
                    SetControlByExpMest(expMestResult.ExpMest.EXP_MEST_CODE);
                    //gridControlExpMestDetail.DataSource = null;
                    FillDataToGridExpMest();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => expMestResult.ExpMest), expMestResult.ExpMest));
                    InitMenuPrint(expMestResult.ExpMest);

                    //dicMediMateAdo.Clear();

                    if (chkPrintMess.Checked)
                    {
                        if (HisConfigCFG.IsMustBeFinishBeforePrinting())
                        {
                            Inventec.Common.Logging.LogSystem.Debug("IsMustBeFinishBeforePrinting ");
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => expMestResult), expMestResult));
                            Inventec.Common.Logging.LogSystem.Debug(" EXP_MEST_STT_ID: " + expMestResult.ExpMest.EXP_MEST_STT_ID );
                            Inventec.Common.Logging.LogSystem.Debug(" ID__DONE: " + IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE);
                            if (expMestResult.ExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                            {
                                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                                store.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__PhieuXuatBan_MPS000092, deletePrintTemplate);
                            }
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Debug("Thực hiện gọi hàm in PhieuXuatBan_MPS000092 ");
                            Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                            store.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__PhieuXuatBan_MPS000092, deletePrintTemplate);
                        }
                    }
                    if (chkGuild.Checked)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Thực hiện gọi hàm in HDSD ");
                        Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                        store.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__HuongDanSuDungThuoc_MPS000099, deletePrintTemplate);
                    }
                }

                MessageManager.Show(this.ParentForm, param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
                success = false;
            }
        }

        private void SetLabelSave(GlobalDataStore.ModuleAction actionModule)
        {
            try
            {
                if (actionModule == GlobalDataStore.ModuleAction.ADD)
                {
                    //btnSavePrint.Text = "Lưu in (Ctrl I)";
                    btnSave.Text = "Lưu (Ctrl S)";
                }
                else if (actionModule == GlobalDataStore.ModuleAction.EDIT)
                {

                    //btnSavePrint.Text = "Sửa in (Ctrl I)";
                    btnSave.Text = "Sửa (Ctrl S)";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void SetControlByExpMest(string expMestCode)
        {
            try
            {
                if (!String.IsNullOrEmpty(expMestCode))
                {
                    this.clientSessionKey = Guid.NewGuid().ToString();
                    moduleAction = GlobalDataStore.ModuleAction.EDIT;
                    lblExpMestCode.Text = expMestCode;
                    //btnSave.Text = "Sửa";
                    //btnSavePrint.Text = "Sửa In";
                    //btnSave.Enabled = false;
                    //btnSavePrint.Enabled = false;
                }
                else
                {
                    moduleAction = GlobalDataStore.ModuleAction.ADD;
                    lblExpMestCode.Text = "";
                    //btnSave.Text = "Lưu";
                    //btnSavePrint.Text = "Lưu In";
                    //btnSave.Enabled = true;
                    //btnSavePrint.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private bool ExistServiceNotPaty()
        {
            bool result = false;
            try
            {
                List<MediMateTypeADO> mediMatyFails = dicMediMateAdo.Select(o => o.Value).Where(o => o.IsNotHasServicePaty).ToList();
                if (mediMatyFails != null && mediMatyFails.Count > 0)
                {
                    MessageBox.Show("Tồn tại thuốc hoặc vật tư không có chính sách giá", "Thông báo");
                    result = true;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool ExistServiceNotInStock()
        {
            bool result = false;
            try
            {
                List<MediMateTypeADO> mediMatyFails = dicMediMateAdo.Select(o => o.Value).Where(o => o.IsNotInStock).ToList();
                if (mediMatyFails != null && mediMatyFails.Count > 0)
                {
                    MessageBox.Show("Tồn tại thuốc hoặc vật tư không có trong kho", "Thông báo");
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool ExistServiceExceedsAvailable()
        {
            bool result = false;
            try
            {
                List<MediMateTypeADO> mediMatyFails = dicMediMateAdo.Select(o => o.Value).Where(o => o.IsExceedsAvailable).ToList();
                if (mediMatyFails != null && mediMatyFails.Count > 0)
                {
                    MessageBox.Show("Tồn tại thuốc hoặc vật tư vượt quá khả dụng", "Thông báo");
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckValiDiscount()
        {
            bool rs = true;
            try
            {
                if (spinDiscountRatio.EditValue != null && (spinDiscountRatio.Value >= 100 || spinDiscountRatio.Value < 0))
                {
                    MessageBox.Show("Chiết khấu không được âm và phải nhỏ hơn 100%", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    rs = false;
                }
            }
            catch (Exception ex)
            {
                rs = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return rs;
        }

        private bool CheckPatientDob()
        {
            bool rs = true;
            try
            {
                if (!String.IsNullOrWhiteSpace(txtPatientDob.Text))
                {
                    DateUtil.DateValidObject dateValidObject = DateUtil.ValidPatientDob(this.txtPatientDob.Text);
                    if (dateValidObject != null && !String.IsNullOrWhiteSpace(dateValidObject.Message))
                    {
                        MessageBox.Show(dateValidObject.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        rs = false;
                    }
                }
            }
            catch (Exception ex)
            {
                rs = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return rs;
        }

        private void FillDataToGridExpMest()
        {
            try
            {
                if (dicMediMateAdo != null)
                {
                    gridControlExpMestMedicine.DataSource = dicMediMateAdo.Select(o => o.Value).Where(o => o.IsMedicine == true).ToList();
                    gridControlExpMestMaterial.DataSource = dicMediMateAdo.Select(o => o.Value).Where(o => o.IsMaterial == true).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDataToSaleCreate(ref HisExpMestSaleSDO hisExpMestSaleSDO)
        {
            try
            {
                hisExpMestSaleSDO.ClientSessionKey = this.clientSessionKey;
                hisExpMestSaleSDO.MediStockId = this.mediStock.ID;
                hisExpMestSaleSDO.PatientName = txtVirPatientName.Text;
                if (cboGender.EditValue != null)
                    hisExpMestSaleSDO.PatientGenderId = Inventec.Common.TypeConvert.Parse.ToInt64(cboGender.EditValue.ToString());
                hisExpMestSaleSDO.PatientAddress = txtAddress.Text;
                if (this.serviceReq != null && !checkIsVisitor.Checked)
                    hisExpMestSaleSDO.PrescriptionId = this.serviceReq.ID;
                hisExpMestSaleSDO.ReqRoomId = this.roomId;
                hisExpMestSaleSDO.Description = txtDescription.Text;
                if (spinDiscountRatio.EditValue != null)
                    hisExpMestSaleSDO.Discount = spinDiscount.Value;
                if (dtIntructionTime.EditValue != null)
                    hisExpMestSaleSDO.InstructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtIntructionTime.DateTime);

                if (this.moduleAction == GlobalDataStore.ModuleAction.EDIT
                    && this.expMestId.HasValue
                    && this.expMestId.Value > 0)
                {
                    hisExpMestSaleSDO.ExpMestId = this.expMestId;
                }

                if (!String.IsNullOrEmpty(txtLoginName.Text))
                {
                    hisExpMestSaleSDO.PrescriptionReqLoginname = txtLoginName.Text;
                }

                hisExpMestSaleSDO.PrescriptionReqLoginname = txtLoginName.Text;
                hisExpMestSaleSDO.PrescriptionReqUsername = txtPresUser.Text;
                hisExpMestSaleSDO.PatientAccountNumber = txtTdlPatientAccountNumber.Text;
                hisExpMestSaleSDO.PatientTaxCode = txtTdlPatientTaxCode.Text;
                hisExpMestSaleSDO.PatientWorkPlace = txtTdlPatientWorkPlace.Text;
                if (chkExp.Checked)
                {
                    hisExpMestSaleSDO.CreateBill = true;
                    hisExpMestSaleSDO.CashierRoomId = Int64.Parse(cboUserRoom.EditValue.ToString());
                    hisExpMestSaleSDO.AccountBookId = Int64.Parse(cboAccountBook.EditValue.ToString());
                    hisExpMestSaleSDO.TransactionNumOrder = spnNumber.Enabled ? (long?)spnNumber.Value : null;
                    hisExpMestSaleSDO.PayFormId = Int64.Parse(cboPayForm.EditValue.ToString());
                }
                //if (!String.IsNullOrEmpty(txtPatientDob.Text))
                //{
                //    dtPatientDob.Text = txtPatientDob.Text;
                //    if (txtPatientDob.Text.Length == 4)
                //    {
                //        dtPatientDob.Text = "01/01/" + txtPatientDob.Text;
                //    }
                //    string dateDob = dtPatientDob.DateTime.ToString("yyyyMMdd");
                //    string timeDob = "00";
                //    hisExpMestSaleSDO.PatientDob = Inventec.Common.TypeConvert.Parse.ToInt64(dateDob + timeDob + "0000");
                //}
                DateUtil.DateValidObject dateValidObject = DateUtil.ValidPatientDob(this.txtPatientDob.Text);
                if (dateValidObject != null && !String.IsNullOrWhiteSpace(dateValidObject.OutDate))
                {
                    this.dtPatientDob.EditValue = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(dateValidObject.OutDate);
                    this.dtPatientDob.Update();
                    hisExpMestSaleSDO.PatientDob = Convert.ToInt64(this.dtPatientDob.DateTime.ToString("yyyyMMdd") + "000000");
                    if (dateValidObject.HasNotDayDob)
                    {
                        hisExpMestSaleSDO.IsHasNotDayDob = true;
                    }
                }
                if (cboPatientType != null)
                    hisExpMestSaleSDO.PatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString());

                if (dicMediMateAdo != null)
                {
                    List<MediMateTypeADO> expMedicines = dicMediMateAdo.Select(o => o.Value).Where(o => o.IsMedicine == true).ToList();
                    if (expMedicines != null && expMedicines.Count > 0)
                    {
                        List<long> beanIds = new List<long>();
                        List<ExpMedicineTypeSDO> medicines = new List<ExpMedicineTypeSDO>();
                        foreach (var expMedicine in expMedicines)
                        {
                            beanIds.AddRange(expMedicine.BeanIds);
                            ExpMedicineTypeSDO medicine = new ExpMedicineTypeSDO();
                            medicine.Amount = expMedicine.EXP_AMOUNT;
                            medicine.Description = expMedicine.NOTE;
                            medicine.Tutorial = expMedicine.TUTORIAL;
                            medicine.MedicineTypeId = expMedicine.MEDI_MATE_TYPE_ID;
                            medicine.NumOfDays = expMedicine.DayNum;

                            if (expMedicine.IsCheckExpPrice)
                            {
                                medicine.Price = expMedicine.EXP_PRICE * (1 + (expMedicine.Profit ?? 0));
                                medicine.VatRatio = expMedicine.EXP_VAT_RATIO;
                            }

                            if (expMedicine.DISCOUNT_RATIO.HasValue)
                            {
                                medicine.DiscountRatio = expMedicine.DISCOUNT_RATIO;
                            }
                            medicines.Add(medicine);
                        }
                        hisExpMestSaleSDO.MedicineBeanIds = beanIds;
                        hisExpMestSaleSDO.Medicines = medicines;
                    }

                    List<MediMateTypeADO> expMaterials = dicMediMateAdo.Select(o => o.Value).Where(o => o.IsMaterial == true).ToList();
                    if (expMaterials != null && expMaterials.Count > 0)
                    {
                        List<long> beanIds = new List<long>();
                        List<ExpMaterialTypeSDO> materials = new List<ExpMaterialTypeSDO>();
                        foreach (var expMaterial in expMaterials)
                        {
                            beanIds.AddRange(expMaterial.BeanIds);
                            ExpMaterialTypeSDO material = new ExpMaterialTypeSDO();
                            material.Amount = expMaterial.EXP_AMOUNT;
                            material.Description = expMaterial.NOTE;
                            material.Tutorial = expMaterial.TUTORIAL;
                            material.MaterialTypeId = expMaterial.MEDI_MATE_TYPE_ID;

                            if (expMaterial.IsCheckExpPrice)
                            {
                                material.Price = expMaterial.EXP_PRICE * (1 + (expMaterial.Profit ?? 0));
                                material.VatRatio = expMaterial.EXP_VAT_RATIO;
                            }
                            if (expMaterial.DISCOUNT_RATIO.HasValue)
                            {
                                material.DiscountRatio = expMaterial.DISCOUNT_RATIO;
                            }
                            materials.Add(material);
                        }
                        hisExpMestSaleSDO.MaterialBeanIds = beanIds;
                        hisExpMestSaleSDO.Materials = materials;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TakeBeanFromPrescription()
        {
            try
            {
                if (this.serviceReq != null)
                {
                    ReleaseAllAndResetGrid();
                    Dictionary<long, MediMateTypeADO> dicMediMateTempAdo = new Dictionary<long, MediMateTypeADO>();
                    CommonParam param = new CommonParam();

                    HisServiceReqMetyFilter metyFilter = new HisServiceReqMetyFilter();
                    metyFilter.SERVICE_REQ_ID = this.serviceReq.ID;
                    List<HIS_SERVICE_REQ_METY> serviceReqMetys = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_METY>>("api/HisServiceReqMety/Get", ApiConsumers.MosConsumer, metyFilter, param);

                    if (serviceReqMetys != null && serviceReqMetys.Count > 0)
                    {
                        serviceReqMetys = serviceReqMetys.Where(o => o.MEDICINE_TYPE_ID.HasValue).ToList();
                        foreach (var serviceReqMety in serviceReqMetys)
                        {
                            long? intructionTime = null;
                            if (dtIntructionTime.EditValue != null)
                                intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtIntructionTime.DateTime) ?? 0;
                            MediMateTypeADO ado = null;
                            if (dicMediMateTempAdo.ContainsKey(serviceReqMety.MEDICINE_TYPE_ID.Value))
                            {
                                ado = dicMediMateTempAdo[serviceReqMety.MEDICINE_TYPE_ID.Value];
                                ado.EXP_AMOUNT += serviceReqMety.AMOUNT;
                                ado.TOTAL_PRICE = ado.EXP_AMOUNT * (ado.EXP_PRICE ?? 0);
                            }
                            else
                            {
                                ado = new MediMateTypeADO(serviceReqMety, intructionTime);
                                dicMediMateTempAdo[serviceReqMety.MEDICINE_TYPE_ID.Value] = ado;
                            }

                            HisMedicineTypeInStockSDO mediInStockSDO = this.mediInStocks.FirstOrDefault(o => o.Id == serviceReqMety.MEDICINE_TYPE_ID);
                            if (mediInStockSDO != null)
                            {
                                ado.NATIONAL_NAME = mediInStockSDO.NationalName;
                                ado.MANUFACTURER_NAME = mediInStockSDO.ManufacturerName;
                                ado.REGISTER_NUMBER = mediInStockSDO.RegisterNumber;
                                ado.SERVICE_UNIT_NAME = mediInStockSDO.ServiceUnitName;
                                ado.AVAILABLE_AMOUNT = mediInStockSDO.AvailableAmount;
                                if (mediInStockSDO.AvailableAmount < ado.EXP_AMOUNT)
                                {
                                    ado.IsExceedsAvailable = true;
                                }
                                ado.ACTIVE_INGR_BHYT_CODE = mediInStockSDO.ActiveIngrBhytCode;
                                ado.ACTIVE_INGR_BHYT_NAME = mediInStockSDO.ActiveIngrBhytName;
                                ado.CONCENTRA = mediInStockSDO.Concentra;
                            }
                            else
                            {
                                ado.IsNotInStock = true;
                            }
                        }
                    }

                    HisServiceReqMatyFilter matyFilter = new HisServiceReqMatyFilter();
                    matyFilter.SERVICE_REQ_ID = this.serviceReq.ID;
                    List<HIS_SERVICE_REQ_MATY> serviceReqMatys = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_MATY>>("api/HisServiceReqMaty/Get", ApiConsumers.MosConsumer, matyFilter, param);
                    if (serviceReqMatys != null && serviceReqMatys.Count > 0)
                    {
                        serviceReqMatys = serviceReqMatys.Where(o => o.MATERIAL_TYPE_ID.HasValue).ToList();
                        foreach (var serviceReqMaty in serviceReqMatys)
                        {

                            MediMateTypeADO ado = null;
                            if (dicMediMateTempAdo.ContainsKey(serviceReqMaty.MATERIAL_TYPE_ID.Value))
                            {
                                ado = dicMediMateTempAdo[serviceReqMaty.MATERIAL_TYPE_ID.Value];
                                ado.EXP_AMOUNT += serviceReqMaty.AMOUNT;
                                ado.TOTAL_PRICE = ado.EXP_AMOUNT * (ado.EXP_PRICE ?? 0);
                            }
                            else
                            {
                                ado = new MediMateTypeADO(serviceReqMaty);
                                dicMediMateTempAdo[serviceReqMaty.MATERIAL_TYPE_ID.Value] = ado;
                            }
                            HisMaterialTypeInStockSDO mateInStockSDO = this.mateInStocks.FirstOrDefault(o => o.Id == serviceReqMaty.MATERIAL_TYPE_ID.Value);
                            if (mateInStockSDO != null)
                            {
                                ado.NATIONAL_NAME = mateInStockSDO.NationalName;
                                ado.MANUFACTURER_NAME = mateInStockSDO.ManufacturerName;
                                ado.SERVICE_UNIT_NAME = mateInStockSDO.ServiceUnitName;
                                ado.AVAILABLE_AMOUNT = mateInStockSDO.AvailableAmount;
                                if (mateInStockSDO.AvailableAmount < serviceReqMaty.AMOUNT)
                                {
                                    ado.IsExceedsAvailable = true;
                                }
                            }
                            else
                            {
                                ado.IsNotInStock = true;
                            }
                        }
                    }

                    //Take bean
                    TakeBeanMedicineAll(dicMediMateTempAdo);
                    TakeBeanMaterialAll(dicMediMateTempAdo);
                    gridControlExpMestDetail.DataSource = dicMediMateAdo.Select(o => o.Value);
                    SetTotalPriceExpMestDetail();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReleaseAllAndResetGrid()
        {
            try
            {
                ReleaseAll();
                dicMediMateAdo = new Dictionary<long, MediMateTypeADO>();
                gridControlExpMestDetail.DataSource = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DiscountDisplayProcess(bool discountFocus, bool discountRatioFocus, SpinEdit spinDiscount, SpinEdit spinDiscountRatio, decimal totalPrice)
        {
            try
            {
                if (discountFocus && !discountRatioFocus && spinDiscount.EditValue != null && totalPrice > 0)
                {
                    spinDiscountRatio.Value = (spinDiscount.Value / totalPrice) * 100;
                }
                if (discountRatioFocus && !discountFocus && spinDiscountRatio.EditValue != null && totalPrice > 0)
                {
                    spinDiscount.Value = (spinDiscountRatio.Value / 100) * totalPrice;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DiscountByAmountAndPriceChange()
        {
            try
            {
                if (this.spinExpPrice.EditValue != null && this.spinExpPrice.Value > 0 && this.spinAmount.EditValue != null && this.spinAmount.Value > 0)
                {
                    if (this.discountDetailFocus)
                    {
                        spinDiscountDetailRatio.Value = (this.spinDiscountDetail.Value / (this.spinAmount.Value * this.spinExpPrice.Value) * 100);
                    }
                    if (this.discountDetailRatioFocus)
                    {
                        spinDiscountDetail.Value = (this.spinDiscountDetailRatio.Value / 100) * (this.spinAmount.Value * this.spinExpPrice.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void DeleteRowExpMestDetail()
        {
            try
            {
                var mediMate = gridViewExpMestDetail.GetFocusedRow() as MediMateTypeADO;
                if (mediMate != null)
                {
                    bool release = false;
                    CommonParam param = new CommonParam();
                    ReleaseBeanSDO releaseBean = new ReleaseBeanSDO();
                    releaseBean.BeanIds = dicMediMateAdo[mediMate.MEDI_MATE_TYPE_ID].BeanIds;
                    releaseBean.ClientSessionKey = this.clientSessionKey;
                    releaseBean.MediStockId = this.mediStock.ID;
                    releaseBean.TypeId = dicMediMateAdo[mediMate.MEDI_MATE_TYPE_ID].MEDI_MATE_TYPE_ID;

                    if (mediMate.IsMedicine)
                    {
                        release = new BackendAdapter(param)
                   .Post<bool>(RequestUriStore.HIS_MEDICINE_BEAN__RELEASE, ApiConsumers.MosConsumer, releaseBean, param);
                    }
                    else if (mediMate.IsMaterial)
                    {
                        release = new BackendAdapter(param)
                       .Post<bool>(RequestUriStore.HIS_MATERIAL_BEAN__RELEASE, ApiConsumers.MosConsumer, releaseBean, param);
                    }

                    if (release)
                    {
                        dicMediMateAdo.Remove(mediMate.MEDI_MATE_TYPE_ID);
                        gridControlExpMestDetail.DataSource = dicMediMateAdo.Select(o => o.Value);
                        SetTotalPriceExpMestDetail();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void EditRowExpMestDetail()
        {
            try
            {
                this.currentMediMate = gridViewExpMestDetail.GetFocusedRow() as MediMateTypeADO;
                if (this.currentMediMate != null)
                {
                    this.Action = GlobalDataStore.ActionEdit;
                    btnAdd.Enabled = true;
                    txtTutorial.Text = this.currentMediMate.TUTORIAL;
                    txtNote.Text = this.currentMediMate.NOTE;
                    spinAmount.Value = this.currentMediMate.EXP_AMOUNT;
                    checkImpExpPrice.Enabled = true;
                    checkImpExpPrice.CheckState = this.currentMediMate.IsCheckExpPrice ?
                        CheckState.Checked : CheckState.Unchecked;
                    if (this.currentMediMate.Profit != null)
                    {
                        spinExpPrice.EditValue = null;
                        spinProfit.Value = (this.currentMediMate.Profit ?? 0) * 100;
                    }
                    else
                    {
                        spinExpPrice.Value = this.currentMediMate.EXP_PRICE ?? 0;
                        spinProfit.EditValue = null;
                    }

                    if (this.currentMediMate.EXP_VAT_RATIO.HasValue && this.currentMediMate.EXP_VAT_RATIO.Value > 0)
                        spinExpVatRatio.Value = (this.currentMediMate.EXP_VAT_RATIO.Value * 100);
                    else
                        spinExpVatRatio.EditValue = null;
                    if (this.currentMediMate.DISCOUNT_RATIO.HasValue && this.currentMediMate.DISCOUNT_RATIO.Value > 0)
                    {
                        spinDiscountDetailRatio.Value = this.currentMediMate.DISCOUNT_RATIO.Value * 100;
                        spinDiscountDetail.Value = this.currentMediMate.DISCOUNT_RATIO.Value * (this.currentMediMate.TOTAL_PRICE ?? 0);
                        this.discountDetailRatioFocus = true;
                    }
                    else
                    {
                        spinDiscountDetailRatio.EditValue = null;
                        spinDiscountDetail.EditValue = null;
                    }
                    if (this.currentMediMate.IsMedicine)
                    {
                        txtTutorial.Enabled = true;
                        if (this.currentMediMate.DayNum.HasValue)
                        {
                            spinDayNum.Value = this.currentMediMate.DayNum.Value;
                            oldDayNum = this.currentMediMate.DayNum.Value;
                        }
                        else
                        {
                            spinDayNum.EditValue = null;
                            oldDayNum = 1;
                        }
                        spinDayNum.Enabled = true;
                    }
                    else
                    {
                        txtTutorial.Enabled = false;
                        spinDayNum.Enabled = false;
                        spinDayNum.EditValue = null;
                    }

                    spinAmount.Focus();
                    spinAmount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
