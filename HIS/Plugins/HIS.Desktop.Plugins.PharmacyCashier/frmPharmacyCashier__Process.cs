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
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.PharmacyCashier.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.PharmacyCashier
{
    public partial class frmPharmacyCashier : FormBase
    {

        private bool TakeBeanProccess(List<long> beanIds, decimal amount, MediMateADO ado)
        {
            bool result = false;
            try
            {
                CommonParam param = new CommonParam();
                TakeBeanSDO takeBeanSDO = new TakeBeanSDO();
                takeBeanSDO.BeanIds = beanIds;
                takeBeanSDO.ClientSessionKey = this.clientSessionKey;
                takeBeanSDO.Amount = amount;
                takeBeanSDO.MediStockId = this.mediStock.ID;
                takeBeanSDO.PatientTypeId = Convert.ToInt64(cboPatientType.EditValue);
                takeBeanSDO.TypeId = ado.MEDI_MATE_TYPE_ID;
                if (ado.ExpMestDetailId.HasValue)
                {
                    takeBeanSDO.ExpMestDetailIds = new List<long> { ado.ExpMestDetailId.Value };
                }
                if (ado.IsMedicine)
                {
                    List<HIS_MEDICINE_BEAN> medicineBeans = new BackendAdapter(param).Post<List<HIS_MEDICINE_BEAN>>("api/HisMedicineBean/Take", ApiConsumers.MosConsumer, takeBeanSDO, param);

                    if (medicineBeans == null || medicineBeans.Count == 0)
                    {
                        WaitingManager.Hide();
                        MessageManager.Show(this.ParentForm, param, false);
                        throw new Exception("Take is correct");
                    }
                    result = true;

                    ado.EXP_AMOUNT = amount;
                    ado.BeanIds = medicineBeans.Select(o => o.ID).ToList();
                    ado.TOTAL_PRICE = medicineBeans.Sum(s => (s.AMOUNT * s.TDL_MEDICINE_IMP_PRICE * (1 + s.TDL_MEDICINE_IMP_VAT_RATIO)));
                    ado.EXP_PRICE = ado.TOTAL_PRICE / ado.EXP_AMOUNT;
                    ado.OLD_AMOUNT = ado.EXP_AMOUNT;
                }
                else if (ado.IsMaterial)
                {
                    List<HIS_MATERIAL_BEAN> materialBeans = new BackendAdapter(param).Post<List<HIS_MATERIAL_BEAN>>("api/HisMaterialBean/Take", ApiConsumers.MosConsumer, takeBeanSDO, param);

                    if (materialBeans == null || materialBeans.Count == 0)
                    {
                        WaitingManager.Hide();
                        MessageManager.Show(this.ParentForm, param, false);
                        throw new Exception("Take is correct");
                    }
                    result = true;

                    ado.EXP_AMOUNT = amount;
                    ado.BeanIds = materialBeans.Select(o => o.ID).ToList();
                    ado.TOTAL_PRICE = materialBeans.Sum(s => (s.AMOUNT * s.TDL_MATERIAL_IMP_PRICE * (1 + s.TDL_MATERIAL_IMP_VAT_RATIO)));
                    ado.EXP_PRICE = ado.TOTAL_PRICE / ado.EXP_AMOUNT;
                    ado.OLD_AMOUNT = ado.EXP_AMOUNT;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void TakeBeanProccess(MediMateADO ado, decimal amount)
        {
            try
            {
                CommonParam param = new CommonParam();
                TakeBeanSDO takeBeanSDO = new TakeBeanSDO();
                takeBeanSDO.BeanIds = ado.BeanIds;
                takeBeanSDO.ClientSessionKey = this.clientSessionKey;
                takeBeanSDO.Amount = amount;
                takeBeanSDO.MediStockId = this.mediStock.ID;
                takeBeanSDO.PatientTypeId = Convert.ToInt64(cboPatientType.EditValue.ToString());
                takeBeanSDO.TypeId = ado.MEDI_MATE_TYPE_ID;
                if (ado.ExpMestDetailId.HasValue)
                {
                    takeBeanSDO.ExpMestDetailIds = new List<long> { ado.ExpMestDetailId.Value };
                }
                if (ado.IsMedicine)
                {
                    List<HIS_MEDICINE_BEAN> medicineBeans = new BackendAdapter(param).Post<List<HIS_MEDICINE_BEAN>>("api/HisMedicineBean/Take", ApiConsumers.MosConsumer, takeBeanSDO, param);

                    ado.EXP_AMOUNT = amount;
                    if (medicineBeans == null || medicineBeans.Count == 0)
                    {
                        ado.IsNotInStock = true;
                        return;
                    }

                    ado.IsNotInStock = false;

                    ado.BeanIds = medicineBeans.Select(o => o.ID).ToList();
                    ado.TOTAL_PRICE = medicineBeans.Sum(s => (s.AMOUNT * s.TDL_MEDICINE_IMP_PRICE * (1 + s.TDL_MEDICINE_IMP_VAT_RATIO)));
                }
                else if (ado.IsMaterial)
                {
                    List<HIS_MATERIAL_BEAN> materialBeans = new BackendAdapter(param).Post<List<HIS_MATERIAL_BEAN>>("api/HisMaterialBean/Take", ApiConsumers.MosConsumer, takeBeanSDO, param);

                    if (materialBeans == null || materialBeans.Count == 0)
                    {
                        ado.IsNotInStock = true;
                        return;
                    }
                    ado.IsNotInStock = false;

                    ado.BeanIds = materialBeans.Select(o => o.ID).ToList();
                    ado.TOTAL_PRICE = materialBeans.Sum(s => (s.AMOUNT * s.TDL_MATERIAL_IMP_PRICE * (1 + s.TDL_MATERIAL_IMP_VAT_RATIO)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CalulatePatientAge(string strDob)
        {
            try
            {
                this.dtPatientDob.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(strDob);
                if (this.dtPatientDob.EditValue != null && this.dtPatientDob.DateTime != DateTime.MinValue)
                {
                    DateTime dtNgSinh = this.dtPatientDob.DateTime;
                    TimeSpan diff = DateTime.Now - dtNgSinh;
                    long tongsogiay = diff.Ticks;
                    if (tongsogiay < 0)
                    {
                        this.txtAge.EditValue = "";
                        this.cboAge.EditValue = 4;
                        return;
                    }
                    DateTime newDate = new DateTime(tongsogiay);

                    int nam = newDate.Year - 1;
                    int thang = newDate.Month - 1;
                    int ngay = newDate.Day - 1;
                    int gio = newDate.Hour;
                    int phut = newDate.Minute;
                    int giay = newDate.Second;

                    if (nam >= 7)
                    {
                        this.cboAge.EditValue = 1;
                        this.txtAge.Enabled = false;
                        this.cboAge.Enabled = false;
                        this.txtAge.EditValue = DateTime.Now.Year - dtNgSinh.Year;
                    }
                    else if (nam > 0 && nam < 7)
                    {
                        if (nam == 6)
                        {
                            if (thang > 0 || ngay > 0)
                            {
                                this.cboAge.EditValue = 1;
                                this.txtAge.Enabled = false;
                                this.cboAge.Enabled = false;
                                this.txtAge.EditValue = DateTime.Now.Year - dtNgSinh.Year;
                            }
                            else
                            {
                                this.txtAge.EditValue = nam * 12 - 1;
                                this.cboAge.EditValue = 2;
                                this.txtAge.Enabled = false;
                                this.cboAge.Enabled = false;
                            }

                        }
                        else
                        {
                            this.txtAge.EditValue = nam * 12 + thang;
                            this.cboAge.EditValue = 2;
                            this.txtAge.Enabled = false;
                            this.cboAge.Enabled = false;
                        }

                    }
                    else
                    {
                        if (thang > 0)
                        {
                            this.txtAge.EditValue = thang.ToString();
                            this.cboAge.EditValue = 2;
                            this.txtAge.Enabled = false;
                            this.cboAge.Enabled = false;
                        }
                        else
                        {
                            if (ngay > 0)
                            {
                                this.txtAge.EditValue = ngay.ToString();
                                this.cboAge.EditValue = 3;
                                this.txtAge.Enabled = false;
                                this.cboAge.Enabled = false;
                            }
                            else
                            {
                                this.txtAge.EditValue = "";
                                this.cboAge.EditValue = 4;
                                this.txtAge.Enabled = true;
                                this.cboAge.Enabled = false;
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

        private void MetyMatyTypeInStock_RowClick(MetyMatyInStockADO ado)
        {
            try
            {
                this.currentMediMate = null;
                if (cboPatientType.EditValue == null)
                {
                    XtraMessageBox.Show("Bạn chưa chọn đối tượng mua", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return;
                }
                if (ado != null)
                {
                    txtMetyMatyPres.Text = ado.MedicineTypeName;
                    string key = "";
                    if (ado.IsMedicine)
                    {
                        key = String.Format(KEY_FORMAT, THUOC, ado.Id);
                    }
                    else
                    {
                        key = String.Format(KEY_FORMAT, VAT_TU, ado.Id);
                    }
                    if (this.dicMediMate.ContainsKey(key))
                    {
                        this.currentMediMate = dicMediMate[key];
                    }
                    else
                    {
                        this.currentMediMate = new MediMateADO(ado);
                        this.currentMediMate.PATIENT_TYPE_ID = Convert.ToInt64(cboPatientType.EditValue);
                    }
                    spinAmount.Focus();
                    spinAmount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataSourceGridMetyMaty()
        {
            try
            {
                gridControlMetyMatys.BeginUpdate();
                gridControlMetyMatys.DataSource = dicMediMate.Select(s => s.Value).ToList();
                gridControlMetyMatys.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                gridControlMetyMatys.EndUpdate();
            }
        }

        private void ResetCurrentMatyMety()
        {
            try
            {
                this.currentMediMate = null;
                this.spinAmount.Value = 0;
                this.txtMetyMatyPres.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TakeBeanByServiceReq(bool isHoldInfo = false)
        {
            try
            {
                if (this.serviceReq != null || (this.serviceReqByTreatmentList != null && this.serviceReqByTreatmentList.Count > 0))
                {
                    this.ReleaseAllAndResetGrid();

                    Dictionary<string, MediMateADO> dicTempAdo = new Dictionary<string, MediMateADO>();

                    if (!this.isNotAllowPres || isHoldInfo)
                    {
                        HisServiceReqMetyFilter metyFilter = new HisServiceReqMetyFilter();
                        if (this.serviceReq != null)
                        {
                            metyFilter.SERVICE_REQ_ID = this.serviceReq.ID;
                        }
                        else if (this.serviceReqByTreatmentList != null && this.serviceReqByTreatmentList.Count > 0)
                        {
                            metyFilter.SERVICE_REQ_IDs = this.serviceReqByTreatmentList.Select(o => o.ID).Distinct().ToList();
                        }

                        List<HIS_SERVICE_REQ_METY> serviceReqMetys = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ_METY>>("api/HisServiceReqMety/Get", ApiConsumers.MosConsumer, metyFilter, null);

                        if (serviceReqMetys != null && serviceReqMetys.Count > 0)
                        {
                            serviceReqMetys = serviceReqMetys.Where(o => o.MEDICINE_TYPE_ID.HasValue).ToList();
                            foreach (var serviceReqMety in serviceReqMetys)
                            {
                                MediMateADO ado = null;
                                string key = String.Format(this.KEY_FORMAT, THUOC, serviceReqMety.MEDICINE_TYPE_ID.Value);
                                if (dicTempAdo.ContainsKey(key))
                                {
                                    ado = dicTempAdo[key];
                                    ado.EXP_AMOUNT += serviceReqMety.AMOUNT;
                                    ado.TOTAL_PRICE = ado.EXP_AMOUNT * (ado.EXP_PRICE ?? 0);
                                }
                                else
                                {
                                    ado = new MediMateADO(serviceReqMety);
                                    dicTempAdo[key] = ado;
                                }

                                MetyMatyInStockADO mediInStockSDO = this.listInStockADOs.FirstOrDefault(o => o.IsMedicine && o.Id == serviceReqMety.MEDICINE_TYPE_ID);
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
                        if (this.serviceReq != null)
                        {
                            matyFilter.SERVICE_REQ_ID = this.serviceReq.ID;
                        }
                        else if (this.serviceReqByTreatmentList != null && this.serviceReqByTreatmentList.Count > 0)
                        {
                            matyFilter.SERVICE_REQ_IDs = this.serviceReqByTreatmentList.Select(o => o.ID).Distinct().ToList();
                        }
                        List<HIS_SERVICE_REQ_MATY> serviceReqMatys = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ_MATY>>("api/HisServiceReqMaty/Get", ApiConsumers.MosConsumer, matyFilter, null);
                        if (serviceReqMatys != null && serviceReqMatys.Count > 0)
                        {
                            serviceReqMatys = serviceReqMatys.Where(o => o.MATERIAL_TYPE_ID.HasValue).ToList();
                            foreach (var serviceReqMaty in serviceReqMatys)
                            {
                                MediMateADO ado = null;
                                string key = String.Format(this.KEY_FORMAT, VAT_TU, serviceReqMaty.MATERIAL_TYPE_ID.Value);
                                if (dicTempAdo.ContainsKey(key))
                                {
                                    ado = dicTempAdo[key];
                                    ado.EXP_AMOUNT += serviceReqMaty.AMOUNT;
                                    ado.TOTAL_PRICE = ado.EXP_AMOUNT * (ado.EXP_PRICE ?? 0);
                                }
                                else
                                {
                                    ado = new MediMateADO(serviceReqMaty);
                                    dicTempAdo[key] = ado;
                                }
                                MetyMatyInStockADO mateInStockSDO = this.listInStockADOs.FirstOrDefault(o => !o.IsMedicine && o.Id == serviceReqMaty.MATERIAL_TYPE_ID.Value);
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
                    }
                    //Take bean
                    this.TakeBeanAll(dicTempAdo);
                    this.SetDataSourceGridMetyMaty();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReleaseAllAndResetGrid(bool isHoldInfo = false)
        {
            try
            {
                if (isHoldInfo) return;
                ReleaseAll();
                dicMediMate = new Dictionary<string, MediMateADO>();
                SetDataSourceGridMetyMaty();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReleaseAll()
        {
            try
            {
                if (dicMediMate != null && dicMediMate.Count > 0 && this.resultSdo == null)
                {
                    CommonParam param = new CommonParam();
                    bool releaseMedicineAll = new BackendAdapter(param).Post<bool>("api/HisMedicineBean/ReleaseAll", ApiConsumers.MosConsumer, this.clientSessionKey, param);
                    if (!releaseMedicineAll)
                    {
                        LogSystem.Error("Release Medicine All False ____" + LogUtil.TraceData("listAdo", dicMediMate.Values));
                    }

                    bool releaseMaterialAll = new BackendAdapter(param).Post<bool>("api/HisMaterialBean/ReleaseAll", ApiConsumers.MosConsumer, this.clientSessionKey, param);
                    if (!releaseMaterialAll)
                    {
                        LogSystem.Error("Release Medicine All False ____" + LogUtil.TraceData("listAdo", dicMediMate.Values));
                    }
                }
                this.clientSessionKey = Guid.NewGuid().ToString();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TakeBeanAll(Dictionary<string, MediMateADO> dic)
        {
            try
            {
                if (dic != null)
                {

                    //Taken Bean Thuoc
                    List<MediMateADO> medicines = dic.Select(o => o.Value).Where(o => o.IsMedicine && !o.IsExceedsAvailable && !o.IsNotInStock).ToList();
                    if (medicines != null && medicines.Count > 0)
                    {
                        CommonParam param = new CommonParam();
                        List<TakeBeanSDO> takeBeanSDOs = new List<TakeBeanSDO>();
                        foreach (var medicine in medicines)
                        {
                            TakeBeanSDO takeBeanSDO = new TakeBeanSDO();
                            takeBeanSDO.ClientSessionKey = this.clientSessionKey;
                            takeBeanSDO.BeanIds = null;
                            takeBeanSDO.Amount = medicine.EXP_AMOUNT;
                            takeBeanSDO.MediStockId = this.mediStock.ID;
                            takeBeanSDO.PatientTypeId = Convert.ToInt64(cboPatientType.EditValue);
                            takeBeanSDO.TypeId = medicine.MEDI_MATE_TYPE_ID;
                            takeBeanSDOs.Add(takeBeanSDO);
                        }

                        List<TakeMedicineBeanListResultSDO> takeMedicines = new BackendAdapter(param).Post<List<TakeMedicineBeanListResultSDO>>("api/HisMedicineBean/TakeList", ApiConsumers.MosConsumer, takeBeanSDOs, param);
                        if (takeMedicines == null || takeMedicines.Count == 0)
                        {
                            param.Messages[0] = "(Thuốc) " + param.Messages[0];
                            MessageManager.Show(param, false);
                        }


                        foreach (var takeMedicine in takeMedicines)
                        {
                            if (takeMedicine.Request == null || takeMedicine.Result == null) continue;

                            string key = String.Format(this.KEY_FORMAT, THUOC, takeMedicine.Request.TypeId);
                            if (dic[key] != null)
                            {
                                List<HIS_MEDICINE_BEAN> medicineBeans = takeMedicine.Result.ToList();
                                var medicineBeanGroups = medicineBeans.GroupBy(o => new { o.TDL_MEDICINE_TYPE_ID });
                                foreach (var g in medicineBeanGroups)
                                {
                                    List<long> beanIds = g != null ? g.Select(o => o.ID).ToList() : null;
                                    dic[key].BeanIds = beanIds;
                                    dic[key].TOTAL_PRICE = g.Sum(s => (s.AMOUNT * s.TDL_MEDICINE_IMP_PRICE * (1 + s.TDL_MEDICINE_IMP_VAT_RATIO)));
                                    dic[key].EXP_PRICE = Math.Round((dic[key].TOTAL_PRICE ?? 0) / dic[key].EXP_AMOUNT, 4);
                                    dic[key].EXP_VAT_RATIO = 0;
                                    dicMediMate[key] = dic[key];
                                }
                            }
                        }
                    }

                    var dicErrors = dic.Where(o => o.Value.IsMedicine == true && (o.Value.IsExceedsAvailable || o.Value.IsNotInStock));
                    foreach (var isError in dic)
                    {
                        if (isError.Value.IsMedicine && (isError.Value.IsExceedsAvailable || isError.Value.IsNotInStock))
                            dicMediMate[isError.Key] = isError.Value;
                    }

                    //Taken Bean Thuoc
                    List<MediMateADO> materials = dic.Select(o => o.Value).Where(o => o.IsMaterial && !o.IsExceedsAvailable && !o.IsNotInStock).ToList();
                    if (materials != null && materials.Count > 0)
                    {
                        CommonParam param = new CommonParam();
                        List<TakeBeanSDO> takeBeanSDOs = new List<TakeBeanSDO>();
                        foreach (var mate in materials)
                        {
                            TakeBeanSDO takeBeanSDO = new TakeBeanSDO();
                            takeBeanSDO.ClientSessionKey = this.clientSessionKey;
                            takeBeanSDO.BeanIds = null;
                            takeBeanSDO.Amount = mate.EXP_AMOUNT;
                            takeBeanSDO.MediStockId = this.mediStock.ID;
                            takeBeanSDO.PatientTypeId = Convert.ToInt64(cboPatientType.EditValue);
                            takeBeanSDO.TypeId = mate.MEDI_MATE_TYPE_ID;
                            takeBeanSDOs.Add(takeBeanSDO);
                        }

                        List<TakeMaterialBeanListResultSDO> takeMaterials = new BackendAdapter(param).Post<List<TakeMaterialBeanListResultSDO>>("api/HisMaterialBean/TakeList", ApiConsumers.MosConsumer, takeBeanSDOs, param);
                        if (takeMaterials == null || takeMaterials.Count == 0)
                        {
                            param.Messages[0] = "(Vật tư) " + param.Messages[0];
                            MessageManager.Show(param, false);
                        }


                        foreach (var take in takeMaterials)
                        {
                            if (take.Request == null || take.Result == null) continue;

                            string key = String.Format(this.KEY_FORMAT, VAT_TU, take.Request.TypeId);
                            if (dic[key] != null)
                            {
                                List<HIS_MATERIAL_BEAN> materialBeans = take.Result.ToList();
                                var beanGroups = materialBeans.GroupBy(o => new { o.TDL_MATERIAL_TYPE_ID });
                                foreach (var g in beanGroups)
                                {
                                    List<long> beanIds = g != null ? g.Select(o => o.ID).ToList() : null;
                                    dic[key].BeanIds = beanIds;
                                    dic[key].TOTAL_PRICE = g.Sum(s => (s.AMOUNT * s.TDL_MATERIAL_IMP_PRICE * (1 + s.TDL_MATERIAL_IMP_VAT_RATIO)));
                                    dic[key].EXP_PRICE = Math.Round((dic[key].TOTAL_PRICE ?? 0) / dic[key].EXP_AMOUNT, 4);
                                    dic[key].EXP_VAT_RATIO = 0;
                                    dicMediMate[key] = dic[key];
                                }
                            }
                        }
                    }

                    foreach (var isError in dic)
                    {
                        if (isError.Value.IsMaterial && (isError.Value.IsExceedsAvailable || isError.Value.IsNotInStock))
                            dicMediMate[isError.Key] = isError.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetTotalPriceOfChildChoice(SereServADO data, TreeListNodes childs)
        {
            try
            {
                decimal totalChoicePrice = 0;
                if (childs != null && childs.Count > 0)
                {
                    foreach (TreeListNode item in childs)
                    {
                        var nodeData = (SereServADO)item.TreeList.GetDataRecordByNode(item);
                        if (nodeData == null) continue;
                        if (!item.HasChildren && item.Checked)
                        {
                            totalChoicePrice += (nodeData.VIR_TOTAL_PRICE ?? 0);
                        }
                    }
                }
                data.VIR_TOTAL_PRICE = totalChoicePrice;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AssignService_RowClick(HIS.Desktop.LocalStorage.BackendData.ADO.ServiceADO ado)
        {
            try
            {
                this.currentServiceAdo = null;
                if (cboServicePatientType.EditValue == null)
                {
                    XtraMessageBox.Show("Bạn chưa chọn đối tượng thanh toán", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return;
                }
                txtAssignService.Text = ado.SERVICE_NAME;
                var exists = listSereServAdo.FirstOrDefault(o => o.SERVICE_ID == ado.ID && o.IsAdd && o.PATIENT_TYPE_ID == Convert.ToInt64(cboServicePatientType.EditValue));
                if (exists == null)
                {
                    HIS_PATIENT_TYPE patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboServicePatientType.EditValue));
                    this.currentServiceAdo = new SereServADO(ado, patientType);
                    if (dtIntructionTime.EditValue != null && dtIntructionTime.DateTime != DateTime.MinValue)
                        this.currentServiceAdo.TDL_INTRUCTION_TIME = Convert.ToInt64(dtIntructionTime.DateTime.ToString("yyyyMMddHHmmss"));
                }
                else
                {
                    this.currentServiceAdo = exists;
                    this.currentServiceAdo.IsExists = true;
                }
                spinServiceAmount.Focus();
                spinServiceAmount.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetCurrentServiceAdo()
        {
            try
            {
                this.currentServiceAdo = null;
                this.spinServiceAmount.Value = 0;
                this.txtAssignService.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetRecentControl()
        {
            try
            {
                if (this.resultSdo != null)
                {
                    if (this.resultSdo.ExpMest != null)
                    {
                        lblRecentPatientName.Text = this.resultSdo.ExpMest.TDL_PATIENT_NAME ?? "";
                    }
                    else if (this.resultSdo.ServiceInvoices != null && this.resultSdo.ServiceInvoices.Count > 0)
                    {
                        lblRecentPatientName.Text = this.resultSdo.ServiceInvoices.FirstOrDefault().TDL_PATIENT_NAME ?? "";
                    }
                    decimal presTotal = 0;
                    decimal serviceTotal = 0;
                    if (this.resultSdo.ExpMest != null)
                    {
                        lblRecentExpMestCode.Text = this.resultSdo.ExpMest.EXP_MEST_CODE;
                        if (this.resultSdo.ExpMestMaterials != null && this.resultSdo.ExpMestMaterials.Count > 0)
                        {
                            presTotal += this.resultSdo.ExpMestMaterials.Sum(s => (s.AMOUNT * (s.PRICE ?? 0) * (1 + (s.VAT_RATIO ?? 0))));
                        }
                        if (this.resultSdo.ExpMestMedicines != null && this.resultSdo.ExpMestMedicines.Count > 0)
                        {
                            presTotal += this.resultSdo.ExpMestMedicines.Sum(s => (s.AMOUNT * (s.PRICE ?? 0) * (1 + (s.VAT_RATIO ?? 0))));
                        }
                        lblRecentTotalPres.Text = Inventec.Common.Number.Convert.NumberToString(presTotal, ConfigApplications.NumberSeperator);
                        if (this.expMestBill != null)
                        {
                            lblRecentPresTranCode.Text = this.expMestBill.TRANSACTION_CODE;
                            lblRecentPresTranNumOrder.Text = this.expMestBill.NUM_ORDER.ToString();
                        }
                    }
                    else
                    {
                        lblRecentTotalPres.Text = "0";
                        lblRecentPresTranCode.Text = "";
                        lblRecentPresTranNumOrder.Text = "";
                        lblRecentExpMestCode.Text = "";
                    }
                    if (this.resultSdo.ServiceInvoices != null && this.resultSdo.ServiceInvoices.Count > 0)
                    {
                        serviceTotal = this.resultSdo.ServiceInvoices.Sum(s => s.AMOUNT);
                        lblRecentTotalService.Text = Inventec.Common.Number.Convert.NumberToString(serviceTotal, ConfigApplications.NumberSeperator);
                        lblRecentServiceTranCode.Text = this.resultSdo.ServiceInvoices.LastOrDefault().TRANSACTION_CODE;
                        lblRecentServiceTranNumOrder.Text = this.resultSdo.ServiceInvoices.LastOrDefault().NUM_ORDER.ToString();
                    }
                    else
                    {
                        lblRecentTotalService.Text = "0";
                        lblRecentServiceTranCode.Text = "";
                        lblRecentServiceTranNumOrder.Text = "";
                    }

                    lblRecentTotalPrice.Text = Inventec.Common.Number.Convert.NumberToString(serviceTotal + presTotal, ConfigApplications.NumberSeperator);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SelectFirstRowPopup(GridLookUpEdit cbo)
        {
            try
            {
                if (cbo != null && cbo.IsPopupOpen)
                {
                    DevExpress.Utils.Win.IPopupControl popupEdit = cbo as DevExpress.Utils.Win.IPopupControl;
                    DevExpress.XtraEditors.Popup.PopupLookUpEditForm popupWindow = popupEdit.PopupWindow as DevExpress.XtraEditors.Popup.PopupLookUpEditForm;
                    if (popupWindow != null)
                    {
                        popupWindow.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
