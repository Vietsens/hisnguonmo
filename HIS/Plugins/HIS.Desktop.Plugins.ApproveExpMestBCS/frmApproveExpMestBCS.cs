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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ApproveExpMestBCS.ADO;
using HIS.Desktop.Plugins.ApproveExpMestBCS.Config;
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ApproveExpMestBCS
{
    public partial class frmApproveExpMestBCS : FormBase
    {
        private long expMestId;
        DelegateSelectData delegateSelectData = null;

        HIS_EXP_MEST currentExpMest = null;
        V_HIS_MEDI_STOCK mediStock = null;

        List<HisMedicineTypeInStockSDO> listMediTypeInStock = new List<HisMedicineTypeInStockSDO>();
        List<HisMaterialTypeInStockSDO> listMateTypeInStock = new List<HisMaterialTypeInStockSDO>();

        List<MedicineTypeADO> medicineAdos = new List<MedicineTypeADO>();
        List<MaterialTypeADO> materialAdos = new List<MaterialTypeADO>();
        List<HIS_EXP_MEST_METY_REQ> expMestMetyReqs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_MATY_REQ> expMestMatyReqs = new List<HIS_EXP_MEST_MATY_REQ>();

        private HisExpMestResultSDO resultSDO = null;

        #region SplitByPatient
        /// <summary>Cờ chỉ trạng thái hiện tại grid đang tách theo bệnh nhân hay không</summary>
        private bool isSplitByPatient = false;
        /// <summary>Cache thông tin điều trị theo TREATMENT_ID — load 1 lần</summary>
        private Dictionary<long, V_HIS_TREATMENT> treatmentDict = new Dictionary<long, V_HIS_TREATMENT>();
        #endregion

        #region ControlState
        private HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        private List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        private bool isNotLoadWhileChangeControlStateInFirst = false;
        private const string MODULE_LINK = "HIS.Desktop.Plugins.ApproveExpMestBCS";
        private const string KEY_SPLIT_BY_PATIENT = "chkSplitByPatient";
        #endregion

        public frmApproveExpMestBCS(Inventec.Desktop.Common.Modules.Module currentModule, long expmestid, DelegateSelectData _delegateSelectData)
            : base(currentModule)
        {
            InitializeComponent();
            this.expMestId = expmestid;
            this.delegateSelectData = _delegateSelectData;
            HisConfig.LoadConfig();
        }

        private void frmApproveExpMestBCS_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                // Runtime ẩn 4 cột Tên BN/Mã ĐT (DevExpress designer hay tự xóa Visible=false)
                this.gridColumn_Medicine_PatientName.Visible = false;
                this.gridColumn_Medicine_TreatmentCode.Visible = false;
                this.gridColumn_Material_PatientName.Visible = false;
                this.gridColumn_Material_TreatmentCode.Visible = false;
                this.SetCaptionByLanguageKey();
                this.LoadExpMest();
                this.VisibleColumnBCS();
                this.LoadDataInStock();
                this.LoadDataMedicine();
                this.LoadDataMaterial();
                this.LoadTreatmentDict();
                // Đọc trạng thái checkbox từ cache TRƯỚC auto-replace —
                // theo spec TH1 Auto Replace: nếu đang CHECK thì tách BN trước rồi mới replace.
                this.InitControlState();
                // Spec: list treatment_id rỗng → hiển thị như hiện tại (không split, ẩn 2 cột)
                if (this.isSplitByPatient && this.HasAnyTreatmentId())
                {
                    this.SplitMedicineAdosByPatient();
                    this.SplitMaterialAdosByPatient();
                    this.SetSplitColumnsVisible(true);
                }
                else
                {
                    this.isSplitByPatient = false; // force group mode khi không có treatment
                }
                this.LoadDataAutoReplace();
                gridControlMedicine.MouseDown += new MouseEventHandler(gridControlMedicine_MouseDown);
                gridControlMaterial.MouseDown += new MouseEventHandler(gridControlMaterial_MouseDown);
                if (medicineAdos != null && medicineAdos.Count > 0)
                {
                    this.xtraTabControl.SelectedTabPageIndex = 0;
                }
                else if (materialAdos != null && materialAdos.Count > 0)
                {
                    this.xtraTabControl.SelectedTabPageIndex = 1;
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager(
                    "HIS.Desktop.Plugins.ApproveExpMestBCS.Resources.Lang",
                    typeof(frmApproveExpMestBCS).Assembly);

                this.Text = GetLangValue("frmApproveExpMestBCS.Text", this.Text);
                this.lciDescription.Text = GetLangValue("frmApproveExpMestBCS.lciDescription.Text", this.lciDescription.Text);
                this.lciSplitByPatient.Text = GetLangValue("frmApproveExpMestBCS.chkSplitByPatient.Text", this.lciSplitByPatient.Text);
                this.btnSave.Text = GetLangValue("frmApproveExpMestBCS.btnSave.Text", this.btnSave.Text);
                this.barButtonSave.Caption = this.btnSave.Text;
                this.xtraTabPageMedicine.Text = GetLangValue("frmApproveExpMestBCS.xtraTabPageMedicine.Text", this.xtraTabPageMedicine.Text);
                this.xtraTabPageMaterial.Text = GetLangValue("frmApproveExpMestBCS.xtraTabPageMaterial.Text", this.xtraTabPageMaterial.Text);

                this.gridColumn_Medicine_PatientName.Caption = GetLangValue("frmApproveExpMestBCS.gridColumn_Medicine_PatientName.Caption", this.gridColumn_Medicine_PatientName.Caption);
                this.gridColumn_Medicine_TreatmentCode.Caption = GetLangValue("frmApproveExpMestBCS.gridColumn_Medicine_TreatmentCode.Caption", this.gridColumn_Medicine_TreatmentCode.Caption);
                this.gridColumn_Medicine_ReplaceName.Caption = GetLangValue("frmApproveExpMestBCS.gridColumn_Medicine_ReplaceName.Caption", this.gridColumn_Medicine_ReplaceName.Caption);
                this.gridColumn_Medicine_MedicineTypeName.Caption = GetLangValue("frmApproveExpMestBCS.gridColumn_Medicine_MedicineTypeName.Caption", this.gridColumn_Medicine_MedicineTypeName.Caption);
                this.gridColumn_Medicine_MedicineTypeCode.Caption = GetLangValue("frmApproveExpMestBCS.gridColumn_Medicine_MedicineTypeCode.Caption", this.gridColumn_Medicine_MedicineTypeCode.Caption);
                this.gridColumn_Medicine_IngrActiveBhytName.Caption = GetLangValue("frmApproveExpMestBCS.gridColumn_Medicine_IngrActiveBhytName.Caption", this.gridColumn_Medicine_IngrActiveBhytName.Caption);
                this.gridColumn_Medicine_ServiceUnitName.Caption = GetLangValue("frmApproveExpMestBCS.gridColumn_Medicine_ServiceUnitName.Caption", this.gridColumn_Medicine_ServiceUnitName.Caption);
                this.gridColumn_Medicine_Amount.Caption = GetLangValue("frmApproveExpMestBCS.gridColumn_Medicine_Amount.Caption", this.gridColumn_Medicine_Amount.Caption);
                this.gridColumn_Medicine_DdAmount.Caption = GetLangValue("frmApproveExpMestBCS.gridColumn_Medicine_DdAmount.Caption", this.gridColumn_Medicine_DdAmount.Caption);
                this.gridColumn_Medicine_AvailAmount.Caption = GetLangValue("frmApproveExpMestBCS.gridColumn_Medicine_AvailAmount.Caption", this.gridColumn_Medicine_AvailAmount.Caption);
                this.gridColumn_Medicine_YcdAmount.Caption = GetLangValue("frmApproveExpMestBCS.gridColumn_Medicine_YcdAmount.Caption", this.gridColumn_Medicine_YcdAmount.Caption);
                this.gridColumn_Medicine_ReplaceAmount.Caption = GetLangValue("frmApproveExpMestBCS.gridColumn_Medicine_ReplaceAmount.Caption", this.gridColumn_Medicine_ReplaceAmount.Caption);

                this.gridColumn_Material_PatientName.Caption = GetLangValue("frmApproveExpMestBCS.gridColumn_Material_PatientName.Caption", this.gridColumn_Material_PatientName.Caption);
                this.gridColumn_Material_TreatmentCode.Caption = GetLangValue("frmApproveExpMestBCS.gridColumn_Material_TreatmentCode.Caption", this.gridColumn_Material_TreatmentCode.Caption);
                this.gridColumn_Material_ReplaceName.Caption = GetLangValue("frmApproveExpMestBCS.gridColumn_Material_ReplaceName.Caption", this.gridColumn_Material_ReplaceName.Caption);
                this.gridColumn_Material_MaterialTypeName.Caption = GetLangValue("frmApproveExpMestBCS.gridColumn_Material_MaterialTypeName.Caption", this.gridColumn_Material_MaterialTypeName.Caption);
                this.gridColumn_Material_MaterialTypeCode.Caption = GetLangValue("frmApproveExpMestBCS.gridColumn_Material_MaterialTypeCode.Caption", this.gridColumn_Material_MaterialTypeCode.Caption);
                this.gridColumn_Material_ServiceUnitName.Caption = GetLangValue("frmApproveExpMestBCS.gridColumn_Material_ServiceUnitName.Caption", this.gridColumn_Material_ServiceUnitName.Caption);
                this.gridColumn_Material_Amount.Caption = GetLangValue("frmApproveExpMestBCS.gridColumn_Material_Amount.Caption", this.gridColumn_Material_Amount.Caption);
                this.gridColumn_Material_DdAmount.Caption = GetLangValue("frmApproveExpMestBCS.gridColumn_Material_DdAmount.Caption", this.gridColumn_Material_DdAmount.Caption);
                this.gridColumn_Material_AvailAmount.Caption = GetLangValue("frmApproveExpMestBCS.gridColumn_Material_AvailAmount.Caption", this.gridColumn_Material_AvailAmount.Caption);
                this.gridColumn_Material_YcdAmount.Caption = GetLangValue("frmApproveExpMestBCS.gridColumn_Material_YcdAmount.Caption", this.gridColumn_Material_YcdAmount.Caption);
                this.gridColumn_Material_ReplaceAmount.Caption = GetLangValue("frmApproveExpMestBCS.gridColumn_Material_ReplaceAmount.Caption", this.gridColumn_Material_ReplaceAmount.Caption);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetLangValue(string key, string fallback)
        {
            try
            {
                string val = Inventec.Common.Resource.Get.Value(
                    key,
                    Resources.ResourceLanguageManager.LanguageResource,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                return string.IsNullOrEmpty(val) ? fallback : val;
            }
            catch { return fallback; }
        }

        private void VisibleColumnBCS()
        {
            try
            {
                if (HisConfig.IS_ALLOW_REPLACE == "1")
                {
                    gridColumn_Medicine_ReplaceName.Visible = true;
                    gridColumn_Medicine_Replace.Visible = true;
                    gridColumn_Medicine_ReplaceAmount.Visible = true;
                    gridColumn_Material_ReplaceName.Visible = true;
                    gridColumn_Material_Replace.Visible = true;
                    gridColumn_Material_ReplaceAmount.Visible = true;
                }
                else
                {
                    gridColumn_Medicine_ReplaceName.Visible = false;
                    gridColumn_Medicine_Replace.Visible = false;
                    gridColumn_Medicine_ReplaceAmount.Visible = false;
                    gridColumn_Material_ReplaceName.Visible = false;
                    gridColumn_Material_Replace.Visible = false;
                    gridColumn_Material_ReplaceAmount.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExpMest()
        {
            try
            {
                HisExpMestFilter chmsFilter = new HisExpMestFilter();
                chmsFilter.ID = expMestId;
                chmsFilter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS;
                List<HIS_EXP_MEST> expMests = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, chmsFilter, null);
                currentExpMest = expMests != null ? expMests.FirstOrDefault() : null;

                this.mediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == this.currentModuleBase.RoomId);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataInStock()
        {
            CommonParam param = new CommonParam();
            try
            {
                HisMedicineTypeStockViewFilter mediFilter = new HisMedicineTypeStockViewFilter();
                mediFilter.MEDI_STOCK_ID = this.mediStock.ID;
                mediFilter.IS_LEAF = true;
                if (HisConfig.IsDontPresExpiredItem)
                {
                    mediFilter.EXPIRED_DATE__NULL_OR_GREATER_THAN_OR_EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(DateTime.Now.ToString("yyyyMMdd") + "000000");
                }

                listMediTypeInStock = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisMedicineTypeInStockSDO>>(HisRequestUriStore.HIS_MEDICINE_TYPE_IN_STOCK, ApiConsumers.MosConsumer, mediFilter, null);

                HisMaterialTypeStockViewFilter mateFilter = new HisMaterialTypeStockViewFilter();
                mateFilter.MEDI_STOCK_ID = this.mediStock.ID;
                mateFilter.IS_LEAF = true;
                if (HisConfig.IsDontPresExpiredItem)
                {
                    mateFilter.EXPIRED_DATE__NULL_OR_GREATER_THAN_OR_EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(DateTime.Now.ToString("yyyyMMdd") + "000000");
                }

                listMateTypeInStock = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisMaterialTypeInStockSDO>>(HisRequestUriStore.HIS_MATERIAL_TYPE_GET_IN_STOCK, ApiConsumers.MosConsumer, mateFilter, null);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("listMateTypeInStock ", listMateTypeInStock));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataMedicine()
        {
            try
            {
                HisExpMestMetyReqFilter metyReqFilter = new HisExpMestMetyReqFilter();
                metyReqFilter.EXP_MEST_ID = this.currentExpMest.ID;
                List<HIS_EXP_MEST_METY_REQ> metyReqs = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/Get", ApiConsumers.MosConsumer, metyReqFilter, null);

                this.expMestMetyReqs = metyReqs;

                List<HIS_EXP_MEST_MEDICINE> medicines = null;
                if (HisConfig.IS_ALLOW_REPLACE == "1" && metyReqs != null && metyReqs.Count > 0)
                {
                    HisExpMestMedicineFilter medicineFilter = new HisExpMestMedicineFilter();
                    medicineFilter.EXP_MEST_ID = this.currentExpMest.ID;
                    medicines = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/Get", ApiConsumer.ApiConsumers.MosConsumer, medicineFilter, null);
                }

                if (metyReqs != null && metyReqs.Count > 0)
                {
                    var Groups = metyReqs.GroupBy(g => g.MEDICINE_TYPE_ID).ToList();
                    foreach (var group in Groups)
                    {
                        List<HIS_EXP_MEST_METY_REQ> listGroup = group.ToList();
                        MedicineTypeADO ado = new MedicineTypeADO(listGroup);

                        if (listMediTypeInStock == null)
                        {
                            listMediTypeInStock = new List<HisMedicineTypeInStockSDO>();
                        }
                        var medicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == group.Key);
                        var inStock = listMediTypeInStock.FirstOrDefault(p => p.Id == group.Key);

                        if (inStock != null)
                        {
                            ado.AVAIL_AMOUNT = (inStock.AvailableAmount ?? 0);
                            ado.TON_KHO = (inStock.TotalAmount ?? 0);
                            ado.MEDICINE_TYPE_CODE = inStock.MedicineTypeCode;
                            ado.IsReplace = false;
                            ado.MEDICINE_TYPE_NAME = inStock.MedicineTypeName;
                            ado.MEDICINE_TYPE_ID = inStock.Id;
                            ado.ACTIVE_INGR_BHYT_NAME = inStock.ActiveIngrBhytName;
                            ado.ACTIVE_INGR_BHYT_CODE = inStock.ActiveIngrBhytCode;
                            ado.SERVICE_UNIT_NAME = inStock.ServiceUnitName;
                            ado.CONCENTRA = inStock.Concentra;

                            if ((ado.AMOUNT - ado.DD_AMOUNT) > (inStock.AvailableAmount ?? 0))
                            {
                                ado.YCD_AMOUNT = inStock.AvailableAmount ?? 0;
                            }
                            else
                            {
                                ado.YCD_AMOUNT = ado.AMOUNT - ado.DD_AMOUNT;
                            }
                            ado.IsCheck = true;
                            if (ado.DD_AMOUNT == ado.AMOUNT)
                            {
                                ado.IsCheck = false;
                            }

                            if (medicineType != null && medicineType.IS_ALLOW_EXPORT_ODD == 1)
                            {
                                ado.IS_ALLOW_EXPORT_ODD = true;
                            }
                            else
                                ado.YCD_AMOUNT = (int)ado.YCD_AMOUNT;

                        }
                        else if (medicineType != null)
                        {
                            ado.MEDICINE_TYPE_CODE = medicineType.MEDICINE_TYPE_CODE;
                            ado.MEDICINE_TYPE_NAME = medicineType.MEDICINE_TYPE_NAME;
                            ado.ACTIVE_INGR_BHYT_NAME = medicineType.ACTIVE_INGR_BHYT_NAME;
                            ado.ACTIVE_INGR_BHYT_CODE = medicineType.ACTIVE_INGR_BHYT_CODE;
                            ado.SERVICE_UNIT_NAME = medicineType.SERVICE_UNIT_NAME;
                            ado.CONCENTRA = medicineType.CONCENTRA;
                        }
                        medicineAdos.Add(ado);
                    }
                }

                if (medicines != null && medicines.Count > 0)
                {
                    var Groups = medicines.GroupBy(g => g.EXP_MEST_METY_REQ_ID).ToList();
                    foreach (var group in Groups)
                    {
                        List<HIS_EXP_MEST_MEDICINE> listGroup = group.ToList();
                        MedicineTypeADO replaceAdo = medicineAdos.FirstOrDefault(o => o.Requests.Any(a => a.ID == group.Key));
                        if (replaceAdo == null) continue;
                        listGroup = listGroup.Where(o => o.TDL_MEDICINE_TYPE_ID != replaceAdo.MEDICINE_TYPE_ID).ToList();
                        var GroupByType = listGroup.GroupBy(g => g.TDL_MEDICINE_TYPE_ID).ToList();
                        foreach (var item in GroupByType)
                        {
                            List<HIS_EXP_MEST_MEDICINE> list = item.ToList();
                            MedicineTypeADO ado = medicineAdos.FirstOrDefault(o => o.MEDICINE_TYPE_ID == item.Key && o.REPLACE_MEDICINE_TYPE_ID == replaceAdo.MEDICINE_TYPE_ID);
                            if (ado == null)
                            {
                                ado = new MedicineTypeADO(list);
                                ado.IsApproved = true;
                                replaceAdo.DD_AMOUNT = replaceAdo.DD_AMOUNT - ado.DD_AMOUNT;
                                replaceAdo.TT_AMOUNT = replaceAdo.TT_AMOUNT + ado.DD_AMOUNT;

                                ado.REPLACE_MEDICINE_TYPE_ID = replaceAdo.MEDICINE_TYPE_ID;
                                ado.REPLACE_MEDICINE_TYPE_NAME = replaceAdo.MEDICINE_TYPE_NAME;

                                if (listMediTypeInStock == null)
                                {
                                    listMediTypeInStock = new List<HisMedicineTypeInStockSDO>();
                                }
                                var medicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == ado.MEDICINE_TYPE_ID);

                                var inStock = listMediTypeInStock.FirstOrDefault(p => p.Id == ado.MEDICINE_TYPE_ID);
                                if (inStock != null)
                                {
                                    ado.AVAIL_AMOUNT = inStock.AvailableAmount ?? 0;
                                    ado.MEDICINE_TYPE_CODE = inStock.MedicineTypeCode;
                                    ado.IsReplace = true;
                                    ado.MEDICINE_TYPE_NAME = inStock.MedicineTypeName;
                                    ado.MEDICINE_TYPE_ID = inStock.Id;
                                    ado.ACTIVE_INGR_BHYT_NAME = inStock.ActiveIngrBhytName;
                                    ado.ACTIVE_INGR_BHYT_CODE = inStock.ActiveIngrBhytCode;
                                    ado.SERVICE_UNIT_NAME = inStock.ServiceUnitName;
                                    ado.YCD_AMOUNT = 0;
                                    ado.IsCheck = false;
                                    ado.CONCENTRA = inStock.Concentra;

                                    if (medicineType != null && medicineType.IS_ALLOW_EXPORT_ODD == 1)
                                    {
                                        ado.IS_ALLOW_EXPORT_ODD = true;
                                    }
                                    else
                                        ado.YCD_AMOUNT = (int)ado.YCD_AMOUNT;

                                }
                                else if (medicineType != null)
                                {
                                    ado.MEDICINE_TYPE_CODE = medicineType.MEDICINE_TYPE_CODE;
                                    ado.MEDICINE_TYPE_NAME = medicineType.MEDICINE_TYPE_NAME;
                                    ado.ACTIVE_INGR_BHYT_NAME = medicineType.ACTIVE_INGR_BHYT_NAME;
                                    ado.ACTIVE_INGR_BHYT_CODE = medicineType.ACTIVE_INGR_BHYT_CODE;
                                    ado.SERVICE_UNIT_NAME = medicineType.SERVICE_UNIT_NAME;
                                    ado.CONCENTRA = medicineType.CONCENTRA;
                                }
                                medicineAdos.Add(ado);
                            }
                            else
                            {
                                decimal ddAmount = list.Sum(s => s.AMOUNT);
                                ado.DD_AMOUNT = ado.DD_AMOUNT + ddAmount;
                                replaceAdo.DD_AMOUNT = replaceAdo.DD_AMOUNT - ddAmount;
                                replaceAdo.TT_AMOUNT = replaceAdo.TT_AMOUNT + ddAmount;
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

        private void LoadDataMaterial()
        {
            try
            {
                HisExpMestMatyReqFilter matyReqFilter = new HisExpMestMatyReqFilter();
                matyReqFilter.EXP_MEST_ID = this.currentExpMest.ID;
                List<HIS_EXP_MEST_MATY_REQ> matyReqs = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/Get", ApiConsumers.MosConsumer, matyReqFilter, null);

                this.expMestMatyReqs = matyReqs;

                List<HIS_EXP_MEST_MATERIAL> materials = null;
                if (HisConfig.IS_ALLOW_REPLACE == "1" && matyReqs != null && matyReqs.Count > 0)
                {
                    HisExpMestMaterialFilter materialFilter = new HisExpMestMaterialFilter();
                    materialFilter.EXP_MEST_ID = this.currentExpMest.ID;
                    materials = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/Get", ApiConsumer.ApiConsumers.MosConsumer, materialFilter, null);
                }

                if (matyReqs != null && matyReqs.Count > 0)
                {
                    var Groups = matyReqs.GroupBy(g => g.MATERIAL_TYPE_ID).ToList();
                    foreach (var group in Groups)
                    {
                        List<HIS_EXP_MEST_MATY_REQ> listGroup = group.ToList();
                        MaterialTypeADO ado = new MaterialTypeADO(listGroup);

                        if (listMateTypeInStock == null)
                        {
                            listMateTypeInStock = new List<HisMaterialTypeInStockSDO>();
                        }
                        var materialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == group.Key);
                        var inStock = listMateTypeInStock.FirstOrDefault(p => p.Id == group.Key);

                        if (inStock != null)
                        {
                            ado.AVAIL_AMOUNT = (inStock.AvailableAmount ?? 0);
                            ado.TON_KHO = (inStock.TotalAmount ?? 0);
                            ado.MATERIAL_TYPE_CODE = inStock.MaterialTypeCode;
                            ado.IsReplace = false;
                            ado.MATERIAL_TYPE_NAME = inStock.MaterialTypeName;
                            ado.MATERIAL_TYPE_ID = inStock.Id;
                            ado.SERVICE_UNIT_NAME = inStock.ServiceUnitName;
                            ado.CONCENTRA = inStock.Concentra;

                            if ((ado.AMOUNT - ado.DD_AMOUNT) > (inStock.AvailableAmount ?? 0))
                            {
                                ado.YCD_AMOUNT = inStock.AvailableAmount ?? 0;
                            }
                            else
                            {
                                ado.YCD_AMOUNT = ado.AMOUNT - ado.DD_AMOUNT;
                            }
                            ado.IsCheck = true;
                            if (ado.DD_AMOUNT == ado.AMOUNT)
                            {
                                ado.IsCheck = false;
                            }

                            if (materialType != null && materialType.IS_ALLOW_EXPORT_ODD == 1)
                            {
                                ado.IS_ALLOW_EXPORT_ODD = true;
                            }
                            else
                                ado.YCD_AMOUNT = (int)ado.YCD_AMOUNT;

                        }
                        else if (materialType != null)
                        {
                            ado.MATERIAL_TYPE_CODE = materialType.MATERIAL_TYPE_CODE;
                            ado.MATERIAL_TYPE_NAME = materialType.MATERIAL_TYPE_NAME;
                            ado.SERVICE_UNIT_NAME = materialType.SERVICE_UNIT_NAME;
                            ado.CONCENTRA = materialType.CONCENTRA;
                        }
                        materialAdos.Add(ado);
                    }
                }

                if (materials != null && materials.Count > 0)
                {
                    var Groups = materials.GroupBy(g => g.EXP_MEST_MATY_REQ_ID).ToList();
                    foreach (var group in Groups)
                    {
                        List<HIS_EXP_MEST_MATERIAL> listGroup = group.ToList();
                        MaterialTypeADO replaceAdo = materialAdos.FirstOrDefault(o => o.Requests.Any(a => a.ID == group.Key));
                        if (replaceAdo == null) continue;
                        listGroup = listGroup.Where(o => o.TDL_MATERIAL_TYPE_ID != replaceAdo.MATERIAL_TYPE_ID).ToList();

                        var GroupByType = listGroup.GroupBy(g => g.TDL_MATERIAL_TYPE_ID).ToList();
                        foreach (var item in GroupByType)
                        {
                            List<HIS_EXP_MEST_MATERIAL> list = item.ToList();
                            MaterialTypeADO ado = materialAdos.FirstOrDefault(o => o.MATERIAL_TYPE_ID == item.Key && o.REPLACE_MATERIAL_TYPE_ID == replaceAdo.MATERIAL_TYPE_ID);
                            if (ado == null)
                            {
                                ado = new MaterialTypeADO(listGroup);
                                ado.IsApproved = true;
                                replaceAdo.DD_AMOUNT = replaceAdo.DD_AMOUNT - ado.DD_AMOUNT;
                                replaceAdo.TT_AMOUNT = replaceAdo.TT_AMOUNT + ado.DD_AMOUNT;

                                ado.REPLACE_MATERIAL_TYPE_ID = replaceAdo.MATERIAL_TYPE_ID;
                                ado.REPLACE_MATERIAL_TYPE_NAME = replaceAdo.MATERIAL_TYPE_NAME;

                                if (listMateTypeInStock == null)
                                {
                                    listMateTypeInStock = new List<HisMaterialTypeInStockSDO>();
                                }
                                var materialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == ado.MATERIAL_TYPE_ID);

                                var inStock = listMateTypeInStock.FirstOrDefault(p => p.Id == ado.MATERIAL_TYPE_ID);
                                if (inStock != null)
                                {
                                    ado.AVAIL_AMOUNT = inStock.AvailableAmount ?? 0;
                                    ado.MATERIAL_TYPE_CODE = inStock.MaterialTypeCode;
                                    ado.IsReplace = true;
                                    ado.MATERIAL_TYPE_NAME = inStock.MaterialTypeName;
                                    ado.MATERIAL_TYPE_ID = inStock.Id;
                                    ado.SERVICE_UNIT_NAME = inStock.ServiceUnitName;
                                    ado.YCD_AMOUNT = 0;
                                    ado.IsCheck = false;
                                    ado.CONCENTRA = inStock.Concentra;

                                    if (materialType != null && materialType.IS_ALLOW_EXPORT_ODD == 1)
                                    {
                                        ado.IS_ALLOW_EXPORT_ODD = true;
                                    }
                                    else
                                        ado.YCD_AMOUNT = (int)ado.YCD_AMOUNT;

                                }
                                else if (materialType != null)
                                {
                                    ado.MATERIAL_TYPE_CODE = materialType.MATERIAL_TYPE_CODE;
                                    ado.MATERIAL_TYPE_NAME = materialType.MATERIAL_TYPE_NAME;
                                    ado.SERVICE_UNIT_NAME = materialType.SERVICE_UNIT_NAME;
                                    ado.CONCENTRA = materialType.CONCENTRA;
                                }
                                materialAdos.Add(ado);
                            }
                            else
                            {
                                decimal ddAmount = list.Sum(s => s.AMOUNT);
                                ado.DD_AMOUNT = ado.DD_AMOUNT + ddAmount;
                                replaceAdo.DD_AMOUNT = replaceAdo.DD_AMOUNT - ddAmount;
                                replaceAdo.TT_AMOUNT = replaceAdo.TT_AMOUNT + ddAmount;
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

        private void LoadDataAutoReplace()
        {
            try
            {
                if (HisConfig.IS_ALLOW_REPLACE == "1" && HisConfig.IS_AUTO_REPLACE == "1")
                {
                    HisMediStockReplaceSDOFilter filter = new HisMediStockReplaceSDOFilter();
                    filter.MediStockId = this.mediStock.ID;
                    // Khi split → dùng Distinct để tránh trùng (mỗi loại thuốc có nhiều row BN)
                    filter.MaterialTypeIds = materialAdos != null ? materialAdos.Where(o => !o.IsReplace && !o.IsApproved).Select(s => s.MATERIAL_TYPE_ID).Distinct().ToList() : null;
                    filter.MedicineTypeIds = medicineAdos != null ? medicineAdos.Where(o => !o.IsReplace && !o.IsApproved).Select(s => s.MEDICINE_TYPE_ID).Distinct().ToList() : null;

                    HisMediStockReplaceSDO replaceSDO = new BackendAdapter(new CommonParam()).Get<HisMediStockReplaceSDO>("api/HisMediStock/GetReplaceSDO", ApiConsumers.MosConsumer, filter, null);

                    if (replaceSDO != null)
                    {
                        // Thứ tự BN cần duyệt thấp nhất ưu tiên trước khi split.
                        var lisMediReqs = medicineAdos != null
                            ? medicineAdos.Where(o => !o.IsReplace && !o.IsApproved)
                                .OrderBy(o => o.AMOUNT - o.CURRENT_DD_AMOUNT).ToList()
                            : null;
                        var lisMateReqs = materialAdos != null
                            ? materialAdos.Where(o => !o.IsReplace && !o.IsApproved)
                                .OrderBy(o => o.AMOUNT - o.CURRENT_DD_AMOUNT).ToList()
                            : null;

                        if (lisMediReqs != null && lisMediReqs.Count > 0 && replaceSDO.MedicineReplaces != null && replaceSDO.MedicineReplaces.Count > 0)
                        {
                            foreach (var item in lisMediReqs)
                            {
                                decimal needApprove = item.AMOUNT - item.CURRENT_DD_AMOUNT;
                                if (item.IsReplace || item.IsApproved || item.TON_KHO > 0 || needApprove <= 0) continue;
                                L_HIS_EXP_MEST_MEDICINE replace = replaceSDO.MedicineReplaces != null ? replaceSDO.MedicineReplaces.FirstOrDefault(o => o.MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID) : null;
                                if (replace == null) continue;
                                var inStock = listMediTypeInStock.FirstOrDefault(p => p.Id == replace.REPLACE_MEDICINE_TYPE_ID);
                                if (inStock == null || (inStock.AvailableAmount ?? 0) <= 0) continue;

                                MedicineTypeADO replaceMedicine = new MedicineTypeADO();
                                replaceMedicine.REPLACE_MEDICINE_TYPE_ID = item.MEDICINE_TYPE_ID;
                                replaceMedicine.REPLACE_MEDICINE_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                                replaceMedicine.MEDICINE_TYPE_CODE = inStock.MedicineTypeCode;
                                replaceMedicine.MEDICINE_TYPE_NAME = inStock.MedicineTypeName;
                                replaceMedicine.MEDICINE_TYPE_ID = inStock.Id;
                                replaceMedicine.ACTIVE_INGR_BHYT_NAME = inStock.ActiveIngrBhytName;
                                replaceMedicine.ACTIVE_INGR_BHYT_CODE = inStock.ActiveIngrBhytCode;
                                replaceMedicine.SERVICE_UNIT_NAME = inStock.ServiceUnitName;
                                replaceMedicine.AVAIL_AMOUNT = inStock.AvailableAmount ?? 0;
                                replaceMedicine.IsCheck = true;
                                replaceMedicine.IsReplace = true;
                                replaceMedicine.AMOUNT = 0;
                                replaceMedicine.CONCENTRA = inStock.Concentra;

                                decimal vailable = (inStock.AvailableAmount ?? 0);
                                if (vailable >= needApprove)
                                {
                                    replaceMedicine.YCD_AMOUNT = needApprove;
                                }
                                else
                                {
                                    replaceMedicine.YCD_AMOUNT = vailable;
                                }
                                if (!replaceMedicine.IS_ALLOW_EXPORT_ODD)
                                    replaceMedicine.YCD_AMOUNT = (int)replaceMedicine.YCD_AMOUNT;

                                // Khi split: copy thông tin BN sang row thay thế + Requests lọc theo BN
                                if (isSplitByPatient)
                                {
                                    replaceMedicine.TREATMENT_ID = item.TREATMENT_ID;
                                    replaceMedicine.PATIENT_NAME = item.PATIENT_NAME;
                                    replaceMedicine.TREATMENT_CODE = item.TREATMENT_CODE;
                                    replaceMedicine.Requests = item.Requests;
                                    // remove theo (REPLACE_MEDICINE_TYPE_ID, TREATMENT_ID) để không xóa row BN khác
                                    medicineAdos.RemoveAll(o => o.REPLACE_MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID
                                        && o.TREATMENT_ID == item.TREATMENT_ID
                                        && !o.IsApproved);
                                }
                                else
                                {
                                    // nếu đã tồn tại thuốc thay thế thì remove
                                    medicineAdos.RemoveAll(o => o.REPLACE_MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID && !o.IsApproved);
                                }
                                medicineAdos.Add(replaceMedicine);

                                // Giảm tồn kho để BN sau không cấp phát vượt — spec: BN cần ít duyệt trước
                                inStock.AvailableAmount = vailable - replaceMedicine.YCD_AMOUNT;

                                item.YCD_AMOUNT = ((item.AMOUNT - replaceMedicine.YCD_AMOUNT) >= item.YCD_AMOUNT ? item.YCD_AMOUNT : (item.AMOUNT - replaceMedicine.YCD_AMOUNT));
                                if (item.YCD_AMOUNT <= 0)
                                {
                                    item.IsCheck = false;
                                }
                            }
                        }

                        if (lisMateReqs != null && lisMateReqs.Count > 0 && replaceSDO.MaterialReplaces != null && replaceSDO.MaterialReplaces.Count > 0)
                        {
                            foreach (var item in lisMateReqs)
                            {
                                decimal needApprove = item.AMOUNT - item.CURRENT_DD_AMOUNT;
                                if (item.IsReplace || item.IsApproved || item.TON_KHO > 0 || needApprove <= 0) continue;
                                L_HIS_EXP_MEST_MATERIAL replace = replaceSDO.MaterialReplaces != null ? replaceSDO.MaterialReplaces.FirstOrDefault(o => o.MATERIAL_TYPE_ID == item.MATERIAL_TYPE_ID) : null;
                                if (replace == null) continue;
                                var inStock = listMateTypeInStock.FirstOrDefault(p => p.Id == replace.REPLACE_MATERIAL_TYPE_ID);
                                if (inStock == null || (inStock.AvailableAmount ?? 0) <= 0) continue;
                                MaterialTypeADO replaceMaterial = new MaterialTypeADO();
                                replaceMaterial.REPLACE_MATERIAL_TYPE_ID = item.MATERIAL_TYPE_ID;
                                replaceMaterial.REPLACE_MATERIAL_TYPE_NAME = item.MATERIAL_TYPE_NAME;
                                replaceMaterial.MATERIAL_TYPE_CODE = inStock.MaterialTypeCode;
                                replaceMaterial.MATERIAL_TYPE_NAME = inStock.MaterialTypeName;
                                replaceMaterial.MATERIAL_TYPE_ID = inStock.Id;
                                replaceMaterial.SERVICE_UNIT_NAME = inStock.ServiceUnitName;
                                replaceMaterial.AVAIL_AMOUNT = inStock.AvailableAmount ?? 0;
                                replaceMaterial.IsCheck = true;
                                replaceMaterial.IsReplace = true;
                                replaceMaterial.AMOUNT = 0;
                                replaceMaterial.CONCENTRA = inStock.Concentra;
                                decimal vailable = (inStock.AvailableAmount ?? 0);
                                if (vailable >= needApprove)
                                {
                                    replaceMaterial.YCD_AMOUNT = needApprove;
                                }
                                else
                                {
                                    replaceMaterial.YCD_AMOUNT = vailable;
                                }
                                if (!replaceMaterial.IS_ALLOW_EXPORT_ODD)
                                    replaceMaterial.YCD_AMOUNT = (int)replaceMaterial.YCD_AMOUNT;

                                if (isSplitByPatient)
                                {
                                    replaceMaterial.TREATMENT_ID = item.TREATMENT_ID;
                                    replaceMaterial.PATIENT_NAME = item.PATIENT_NAME;
                                    replaceMaterial.TREATMENT_CODE = item.TREATMENT_CODE;
                                    replaceMaterial.Requests = item.Requests;
                                    materialAdos.RemoveAll(o => o.REPLACE_MATERIAL_TYPE_ID == item.MATERIAL_TYPE_ID
                                        && o.TREATMENT_ID == item.TREATMENT_ID
                                        && !o.IsApproved);
                                }
                                else
                                {
                                    // nếu đã tồn tại vật tư thay thế thì remove
                                    materialAdos.RemoveAll(o => o.REPLACE_MATERIAL_TYPE_ID == item.MATERIAL_TYPE_ID && !o.IsApproved);
                                }
                                materialAdos.Add(replaceMaterial);

                                inStock.AvailableAmount = vailable - replaceMaterial.YCD_AMOUNT;

                                item.YCD_AMOUNT = ((item.AMOUNT - replaceMaterial.YCD_AMOUNT) >= item.YCD_AMOUNT ? item.YCD_AMOUNT : (item.AMOUNT - replaceMaterial.YCD_AMOUNT));
                                if (item.YCD_AMOUNT <= 0)
                                {
                                    item.IsCheck = false;
                                }
                            }
                        }
                    }
                }
                gridControlMedicine.BeginUpdate();
                gridControlMedicine.DataSource = medicineAdos;
                gridControlMedicine.EndUpdate();

                gridControlMaterial.BeginUpdate();
                gridControlMaterial.DataSource = materialAdos;
                gridControlMaterial.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0 || e.Column.FieldName != "YCD_AMOUNT")
                    return;
                var data = (MedicineTypeADO)gridViewMedicine.GetRow(e.RowHandle);
                if (data != null)
                {
                    bool valid = true;
                    string message = "";
                    if (!data.IsReplace)
                    {
                        if (data.YCD_AMOUNT <= 0)
                        {
                            valid = false;
                            message = "Số lượng duyệt phải lớn hơn 0";
                        }
                        else if (data.YCD_AMOUNT > (data.AMOUNT - (data.CURRENT_DD_AMOUNT + data.CURRENT_YC_AMOUNT)))
                        {
                            valid = false;
                            message = String.Format("Số lượng duyệt {0} lớn hơn số lượng yêu cầu {1}", data.YCD_AMOUNT, (data.AMOUNT - (data.CURRENT_DD_AMOUNT + data.CURRENT_YC_AMOUNT)));
                        }
                        else if (!data.IS_ALLOW_EXPORT_ODD)
                        {
                            decimal x = Math.Abs(Math.Round(data.YCD_AMOUNT, 3) - Math.Floor(data.YCD_AMOUNT));
                            if (x > 0)
                            {
                                valid = false;
                                message = "Không cho phép duyệt lẻ";
                            }
                        }
                    }
                    else
                    {
                        // Khi split: match đúng BN; ngược lại match theo loại thuốc
                        var req = isSplitByPatient
                            ? medicineAdos.FirstOrDefault(o => !o.IsReplace && o.MEDICINE_TYPE_ID == data.REPLACE_MEDICINE_TYPE_ID && o.TREATMENT_ID == data.TREATMENT_ID)
                            : medicineAdos.FirstOrDefault(o => !o.IsReplace && o.MEDICINE_TYPE_ID == data.REPLACE_MEDICINE_TYPE_ID);
                        if (data.YCD_AMOUNT <= 0)
                        {
                            valid = false;
                            message = "Số lượng duyệt phải lớn hơn 0";
                        }
                        else if (req == null)
                        {
                            valid = false;
                            message = "Không tìm thấy dòng yêu cầu tương ứng";
                        }
                        else if (data.YCD_AMOUNT > (req.AMOUNT - req.CURRENT_DD_AMOUNT))
                        {
                            valid = false;
                            message = String.Format("Số lượng duyệt {0} lớn hơn số lượng yêu cầu {1}", data.YCD_AMOUNT, (req.AMOUNT - req.CURRENT_DD_AMOUNT));
                        }
                        else if (!data.IS_ALLOW_EXPORT_ODD)
                        {
                            decimal x = Math.Abs(Math.Round(data.YCD_AMOUNT, 3) - Math.Floor(data.YCD_AMOUNT));
                            if (x > 0)
                            {
                                valid = false;
                                message = "Không cho phép duyệt lẻ";
                            }
                        }
                        if (req != null)
                        {
                            req.YCD_AMOUNT = ((req.AMOUNT - data.YCD_AMOUNT) >= req.YCD_AMOUNT ? req.YCD_AMOUNT : (req.AMOUNT - data.YCD_AMOUNT));
                            req.CURRENT_YC_AMOUNT = data.YCD_AMOUNT;
                        }
                    }
                    if (!valid)
                        gridViewMedicine.SetColumnError(gridViewMedicine.FocusedColumn, message);
                    else
                        gridViewMedicine.ClearColumnErrors();
                }
                gridControlMedicine.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (MedicineTypeADO)gridViewMedicine.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "Replace")
                        {
                            if (data.IsCheck && !data.IsReplace)
                                e.RepositoryItem = repositoryItemButtonReplaceMedicine_Enable;
                            else
                                e.RepositoryItem = repositoryItemButtonReplaceMedicine_Disable;
                        }
                        else if (e.Column.FieldName == "IsCheck")
                        {
                            if (data.IsApproved)
                                e.RepositoryItem = repositoryItemCheckMedicine_Disable;
                            else
                                e.RepositoryItem = repositoryItemCheckMedicine_Enable;
                        }
                        else if (e.Column.FieldName == "YCD_AMOUNT")
                        {
                            if (data.IsCheck && (!data.IsApproved))
                                e.RepositoryItem = repositoryItemSpinMedicineYcdAmount;
                            else
                                e.RepositoryItem = repositoryItemSpinMedicineYcdAmount_Disable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_InvalidRowException(object sender, DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventArgs e)
        {
            try
            {
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (MedicineTypeADO)gridViewMedicine.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if ((data.AMOUNT - data.DD_AMOUNT) > data.AVAIL_AMOUNT)
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                        else if (data.IsReplace)// thuốc thay thế
                        {
                            e.Appearance.ForeColor = Color.Blue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0 || e.Column.FieldName != "YCD_AMOUNT")
                    return;
                var data = (MaterialTypeADO)gridViewMaterial.GetRow(e.RowHandle);
                if (data != null)
                {
                    bool valid = true;
                    string message = "";
                    if (!data.IsReplace)
                    {
                        if (data.YCD_AMOUNT <= 0)
                        {
                            valid = false;
                            message = "Số lượng duyệt phải lớn hơn 0";
                        }
                        else if (data.YCD_AMOUNT > (data.AMOUNT - (data.CURRENT_DD_AMOUNT + data.CURRENT_YC_AMOUNT)))
                        {
                            valid = false;
                            message = String.Format("Số lượng duyệt {0} lớn hơn số lượng yêu cầu {1}", data.YCD_AMOUNT, (data.AMOUNT - (data.CURRENT_DD_AMOUNT + data.CURRENT_YC_AMOUNT)));
                        }
                        else if (!data.IS_ALLOW_EXPORT_ODD)
                        {
                            decimal x = Math.Abs(Math.Round(data.YCD_AMOUNT, 3) - Math.Floor(data.YCD_AMOUNT));
                            if (x > 0)
                            {
                                valid = false;
                                message = "Không cho phép duyệt lẻ";
                            }
                        }
                    }
                    else
                    {
                        // Khi split: match đúng BN; ngược lại match theo loại vật tư
                        var req = isSplitByPatient
                            ? materialAdos.FirstOrDefault(o => !o.IsReplace && o.MATERIAL_TYPE_ID == data.REPLACE_MATERIAL_TYPE_ID && o.TREATMENT_ID == data.TREATMENT_ID)
                            : materialAdos.FirstOrDefault(o => !o.IsReplace && o.MATERIAL_TYPE_ID == data.REPLACE_MATERIAL_TYPE_ID);
                        if (data.YCD_AMOUNT <= 0)
                        {
                            valid = false;
                            message = "Số lượng duyệt phải lớn hơn 0";
                        }
                        else if (req == null)
                        {
                            valid = false;
                            message = "Không tìm thấy dòng yêu cầu tương ứng";
                        }
                        else if (data.YCD_AMOUNT > (req.AMOUNT - req.CURRENT_DD_AMOUNT))
                        {
                            valid = false;
                            message = String.Format("Số lượng duyệt {0} lớn hơn số lượng yêu cầu {1}", data.YCD_AMOUNT, (req.AMOUNT - req.CURRENT_DD_AMOUNT));
                        }
                        else if (!data.IS_ALLOW_EXPORT_ODD)
                        {
                            decimal x = Math.Abs(Math.Round(data.YCD_AMOUNT, 3) - Math.Floor(data.YCD_AMOUNT));
                            if (x > 0)
                            {
                                valid = false;
                                message = "Không cho phép duyệt lẻ";
                            }
                        }
                        if (valid && req != null)
                        {
                            req.YCD_AMOUNT = ((req.AMOUNT - data.YCD_AMOUNT) >= req.YCD_AMOUNT ? req.YCD_AMOUNT : (req.AMOUNT - data.YCD_AMOUNT));
                            req.CURRENT_YC_AMOUNT = data.YCD_AMOUNT;
                        }
                    }
                    if (!valid)
                        gridViewMaterial.SetColumnError(gridViewMaterial.FocusedColumn, message);
                    else
                        gridViewMaterial.ClearColumnErrors();
                }
                gridControlMaterial.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (MaterialTypeADO)gridViewMaterial.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "Replace")
                        {
                            if (data.IsCheck && !data.IsReplace)
                                e.RepositoryItem = repositoryItemButtonReplaceMaterial_Enable;
                            else
                                e.RepositoryItem = repositoryItemButtonReplaceMaterial_Disable;
                        }
                        else if (e.Column.FieldName == "IsCheck")
                        {
                            if (data.IsApproved)
                                e.RepositoryItem = repositoryItemCheckMaterial_Disable;
                            else
                                e.RepositoryItem = repositoryItemCheckMaterial_Enable;
                        }
                        else if (e.Column.FieldName == "YCD_AMOUNT")
                        {
                            if (data.IsCheck && (!data.IsApproved))
                                e.RepositoryItem = repositoryItemSpinMaterialYcdAmount;
                            else
                                e.RepositoryItem = repositoryItemSpinMaterialYcdAmount_Disable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_InvalidRowException(object sender, DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventArgs e)
        {
            try
            {
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (MaterialTypeADO)gridViewMaterial.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if ((data.AMOUNT - data.DD_AMOUNT) > data.AVAIL_AMOUNT)
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                        else if (data.IsReplace)// thuốc thay thế
                        {
                            e.Appearance.ForeColor = Color.Blue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Split-By-Patient Logic

        /// <summary>
        /// Có bất kỳ request nào mang TREATMENT_ID hay không.
        /// Nếu FALSE → không tách BN, hiển thị như chế độ group.
        /// </summary>
        private bool HasAnyTreatmentId()
        {
            if (expMestMetyReqs != null && expMestMetyReqs.Any(r => r.TREATMENT_ID.HasValue && r.TREATMENT_ID.Value > 0))
                return true;
            if (expMestMatyReqs != null && expMestMatyReqs.Any(r => r.TREATMENT_ID.HasValue && r.TREATMENT_ID.Value > 0))
                return true;
            return false;
        }

        /// <summary>
        /// Load V_HIS_TREATMENT theo tất cả TREATMENT_ID có trong MetyReq/MatyReq — 1 API call.
        /// Nếu không có req nào mang TREATMENT_ID → dict rỗng (sẽ hiển thị như hiện tại khi split).
        /// </summary>
        private void LoadTreatmentDict()
        {
            try
            {
                treatmentDict = new Dictionary<long, V_HIS_TREATMENT>();

                var ids = new HashSet<long>();
                if (expMestMetyReqs != null)
                {
                    foreach (var r in expMestMetyReqs)
                        if (r.TREATMENT_ID.HasValue && r.TREATMENT_ID.Value > 0)
                            ids.Add(r.TREATMENT_ID.Value);
                }
                if (expMestMatyReqs != null)
                {
                    foreach (var r in expMestMatyReqs)
                        if (r.TREATMENT_ID.HasValue && r.TREATMENT_ID.Value > 0)
                            ids.Add(r.TREATMENT_ID.Value);
                }
                if (ids.Count == 0) return;

                // Dùng GetView (V_HIS_TREATMENT có sẵn TREATMENT_CODE + TDL_PATIENT_NAME)
                // — endpoint /Get có vấn đề lỗi 500 trên 1 số môi trường khi dùng filter IDs
                var filter = new HisTreatmentViewFilter();
                filter.IDs = ids.ToList();
                var list = new BackendAdapter(new CommonParam())
                    .Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, filter, null);
                if (list != null)
                {
                    foreach (var t in list)
                        if (!treatmentDict.ContainsKey(t.ID)) treatmentDict.Add(t.ID, t);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Đọc trạng thái chkSplitByPatient từ ControlState. Flag chặn CheckedChanged trong lúc set.
        /// </summary>
        private void InitControlState()
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = true;
                controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                currentControlStateRDO = controlStateWorker.GetData(MODULE_LINK);

                if (currentControlStateRDO != null && currentControlStateRDO.Count > 0)
                {
                    var item = currentControlStateRDO.FirstOrDefault(o => o.KEY == KEY_SPLIT_BY_PATIENT);
                    if (item != null && item.VALUE == "1")
                    {
                        this.chkSplitByPatient.Checked = true;
                        this.isSplitByPatient = true;
                    }
                }
                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                isNotLoadWhileChangeControlStateInFirst = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveControlState()
        {
            try
            {
                if (controlStateWorker == null)
                    controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                if (currentControlStateRDO == null)
                    currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();

                var item = currentControlStateRDO.FirstOrDefault(
                    o => o.KEY == KEY_SPLIT_BY_PATIENT && o.MODULE_LINK == MODULE_LINK);
                if (item != null)
                {
                    item.VALUE = chkSplitByPatient.Checked ? "1" : "";
                }
                else
                {
                    currentControlStateRDO.Add(new HIS.Desktop.Library.CacheClient.ControlStateRDO
                    {
                        KEY = KEY_SPLIT_BY_PATIENT,
                        MODULE_LINK = MODULE_LINK,
                        VALUE = chkSplitByPatient.Checked ? "1" : ""
                    });
                }
                controlStateWorker.SetData(currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetSplitColumnsVisible(bool visible)
        {
            try
            {
                if (visible)
                {
                    // Gán VisibleIndex để đặt 2 cột BN ở đầu grid — DevExpress tự shift các cột khác.
                    this.gridColumn_Medicine_PatientName.VisibleIndex = 1;
                    this.gridColumn_Medicine_PatientName.Visible = true;
                    this.gridColumn_Medicine_TreatmentCode.VisibleIndex = 2;
                    this.gridColumn_Medicine_TreatmentCode.Visible = true;

                    this.gridColumn_Material_PatientName.VisibleIndex = 1;
                    this.gridColumn_Material_PatientName.Visible = true;
                    this.gridColumn_Material_TreatmentCode.VisibleIndex = 2;
                    this.gridColumn_Material_TreatmentCode.Visible = true;
                }
                else
                {
                    // Set Visible = false — DevExpress tự đưa VisibleIndex = -1.
                    this.gridColumn_Medicine_PatientName.Visible = false;
                    this.gridColumn_Medicine_TreatmentCode.Visible = false;
                    this.gridColumn_Material_PatientName.Visible = false;
                    this.gridColumn_Material_TreatmentCode.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Clone metadata (thông tin loại thuốc, tồn kho, flags) — KHÔNG copy số lượng/requests/patient info.
        /// </summary>
        private MedicineTypeADO CloneMedicineMetadata(MedicineTypeADO src)
        {
            return new MedicineTypeADO
            {
                MEDICINE_TYPE_ID = src.MEDICINE_TYPE_ID,
                MEDICINE_TYPE_CODE = src.MEDICINE_TYPE_CODE,
                MEDICINE_TYPE_NAME = src.MEDICINE_TYPE_NAME,
                ACTIVE_INGR_BHYT_CODE = src.ACTIVE_INGR_BHYT_CODE,
                ACTIVE_INGR_BHYT_NAME = src.ACTIVE_INGR_BHYT_NAME,
                SERVICE_UNIT_NAME = src.SERVICE_UNIT_NAME,
                CONCENTRA = src.CONCENTRA,
                TON_KHO = src.TON_KHO,
                AVAIL_AMOUNT = src.AVAIL_AMOUNT,
                IS_ALLOW_EXPORT_ODD = src.IS_ALLOW_EXPORT_ODD,
                IsReplace = src.IsReplace,
                IsApproved = src.IsApproved,
                REPLACE_MEDICINE_TYPE_ID = src.REPLACE_MEDICINE_TYPE_ID,
                REPLACE_MEDICINE_TYPE_NAME = src.REPLACE_MEDICINE_TYPE_NAME
            };
        }

        private MaterialTypeADO CloneMaterialMetadata(MaterialTypeADO src)
        {
            return new MaterialTypeADO
            {
                MATERIAL_TYPE_ID = src.MATERIAL_TYPE_ID,
                MATERIAL_TYPE_CODE = src.MATERIAL_TYPE_CODE,
                MATERIAL_TYPE_NAME = src.MATERIAL_TYPE_NAME,
                ACTIVE_INGR_BHYT_NAME = src.ACTIVE_INGR_BHYT_NAME,
                SERVICE_UNIT_NAME = src.SERVICE_UNIT_NAME,
                CONCENTRA = src.CONCENTRA,
                TON_KHO = src.TON_KHO,
                AVAIL_AMOUNT = src.AVAIL_AMOUNT,
                IS_ALLOW_EXPORT_ODD = src.IS_ALLOW_EXPORT_ODD,
                IsReplace = src.IsReplace,
                IsApproved = src.IsApproved,
                REPLACE_MATERIAL_TYPE_ID = src.REPLACE_MATERIAL_TYPE_ID,
                REPLACE_MATERIAL_TYPE_NAME = src.REPLACE_MATERIAL_TYPE_NAME
            };
        }

        /// <summary>
        /// Gán thông tin bệnh nhân từ treatmentDict. Không có → để null/empty (hiển thị dòng để trống 2 cột BN).
        /// </summary>
        private void ApplyPatientInfo(MedicineTypeADO ado, long? treatmentId)
        {
            ado.TREATMENT_ID = treatmentId;
            ado.PATIENT_NAME = "";
            ado.TREATMENT_CODE = "";
            if (treatmentId.HasValue && treatmentId.Value > 0
                && treatmentDict != null && treatmentDict.ContainsKey(treatmentId.Value))
            {
                var t = treatmentDict[treatmentId.Value];
                ado.PATIENT_NAME = t.TDL_PATIENT_NAME;
                ado.TREATMENT_CODE = t.TREATMENT_CODE;
            }
        }

        private void ApplyPatientInfo(MaterialTypeADO ado, long? treatmentId)
        {
            ado.TREATMENT_ID = treatmentId;
            ado.PATIENT_NAME = "";
            ado.TREATMENT_CODE = "";
            if (treatmentId.HasValue && treatmentId.Value > 0
                && treatmentDict != null && treatmentDict.ContainsKey(treatmentId.Value))
            {
                var t = treatmentDict[treatmentId.Value];
                ado.PATIENT_NAME = t.TDL_PATIENT_NAME;
                ado.TREATMENT_CODE = t.TREATMENT_CODE;
            }
        }

        /// <summary>
        /// Biến medicineAdos từ dạng grouped sang dạng tách theo bệnh nhân.
        /// Row yêu cầu: split theo TREATMENT_ID của HIS_EXP_MEST_METY_REQ.
        /// Row đã duyệt (IsApproved): giữ nguyên.
        /// Row thay thế (IsReplace) chưa duyệt: phân bổ YCD cho BN có số lượng duyệt thấp nhất trước (TH2 manual replace).
        /// </summary>
        private void SplitMedicineAdosByPatient()
        {
            try
            {
                if (medicineAdos == null) return;
                var result = new List<MedicineTypeADO>();
                var pendingReplaces = new List<MedicineTypeADO>();

                // Pass 1: split row yêu cầu; giữ row IsApproved; lưu row IsReplace để xử lý sau
                foreach (var ado in medicineAdos)
                {
                    if (ado.IsApproved)
                    {
                        result.Add(ado);
                        continue;
                    }
                    if (ado.IsReplace)
                    {
                        pendingReplaces.Add(ado);
                        continue;
                    }
                    if (ado.Requests == null || ado.Requests.Count == 0)
                    {
                        result.Add(ado);
                        continue;
                    }

                    var byTreatment = ado.Requests
                        .GroupBy(r => r.TREATMENT_ID)
                        .OrderBy(g => g.Sum(r => r.AMOUNT - (r.DD_AMOUNT ?? 0)))
                        .ToList();

                    // Tồn còn lại cho loại thuốc này — phân bổ cumulative theo thứ tự BN cần ít → cần nhiều
                    decimal remainingAvail = ado.AVAIL_AMOUNT;

                    foreach (var grp in byTreatment)
                    {
                        var reqs = grp.ToList();
                        var splitAdo = CloneMedicineMetadata(ado);
                        splitAdo.Requests = reqs;
                        splitAdo.AMOUNT = reqs.Sum(r => r.AMOUNT);
                        splitAdo.DD_AMOUNT = reqs.Sum(r => r.DD_AMOUNT ?? 0);
                        splitAdo.CURRENT_DD_AMOUNT = splitAdo.DD_AMOUNT;
                        splitAdo.TT_AMOUNT = 0;
                        // AVAIL_AMOUNT hiển thị = tồn còn lại khả dụng cho BN này
                        splitAdo.AVAIL_AMOUNT = remainingAvail < 0 ? 0 : remainingAvail;
                        ApplyPatientInfo(splitAdo, grp.Key);

                        decimal needApprove = splitAdo.AMOUNT - splitAdo.DD_AMOUNT;
                        splitAdo.YCD_AMOUNT = Math.Min(needApprove, splitAdo.AVAIL_AMOUNT);
                        if (splitAdo.YCD_AMOUNT < 0) splitAdo.YCD_AMOUNT = 0;
                        if (!splitAdo.IS_ALLOW_EXPORT_ODD)
                            splitAdo.YCD_AMOUNT = (int)splitAdo.YCD_AMOUNT;
                        splitAdo.IsCheck = splitAdo.YCD_AMOUNT > 0;

                        remainingAvail -= splitAdo.YCD_AMOUNT;
                        result.Add(splitAdo);
                    }
                }

                // Pass 2: phân bổ row thay thế cho BN có số lượng duyệt thấp nhất trước
                var requestLookup = result
                    .Where(o => !o.IsReplace && !o.IsApproved && o.TREATMENT_ID.HasValue)
                    .GroupBy(o => o.MEDICINE_TYPE_ID)
                    .ToDictionary(g => g.Key, g => g.OrderBy(x => x.AMOUNT - x.CURRENT_DD_AMOUNT).ToList());

                foreach (var repl in pendingReplaces)
                {
                    if (!repl.REPLACE_MEDICINE_TYPE_ID.HasValue
                        || !requestLookup.ContainsKey(repl.REPLACE_MEDICINE_TYPE_ID.Value)
                        || requestLookup[repl.REPLACE_MEDICINE_TYPE_ID.Value].Count == 0)
                    {
                        result.Add(repl);
                        continue;
                    }

                    var reqBNs = requestLookup[repl.REPLACE_MEDICINE_TYPE_ID.Value];
                    decimal remaining = repl.YCD_AMOUNT;
                    bool addedAny = false;

                    foreach (var reqBN in reqBNs)
                    {
                        if (remaining <= 0) break;
                        decimal needBN = reqBN.AMOUNT - reqBN.CURRENT_DD_AMOUNT;
                        if (needBN <= 0) continue;
                        decimal alloc = Math.Min(remaining, needBN);

                        var splitRepl = CloneMedicineMetadata(repl);
                        splitRepl.AMOUNT = 0;
                        splitRepl.YCD_AMOUNT = alloc;
                        splitRepl.IsCheck = true;
                        splitRepl.Requests = reqBN.Requests;
                        ApplyPatientInfo(splitRepl, reqBN.TREATMENT_ID);
                        result.Add(splitRepl);
                        remaining -= alloc;
                        addedAny = true;
                    }

                    // Trường hợp các BN không còn nhu cầu nhưng YCD replace vẫn dư → gán vào BN đầu
                    if (remaining > 0 && !addedAny)
                    {
                        var first = reqBNs.First();
                        var splitRepl = CloneMedicineMetadata(repl);
                        splitRepl.AMOUNT = 0;
                        splitRepl.YCD_AMOUNT = remaining;
                        splitRepl.IsCheck = true;
                        splitRepl.Requests = first.Requests;
                        ApplyPatientInfo(splitRepl, first.TREATMENT_ID);
                        result.Add(splitRepl);
                    }
                }

                medicineAdos = result;
                isSplitByPatient = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SplitMaterialAdosByPatient()
        {
            try
            {
                if (materialAdos == null) return;
                var result = new List<MaterialTypeADO>();
                var pendingReplaces = new List<MaterialTypeADO>();

                foreach (var ado in materialAdos)
                {
                    if (ado.IsApproved)
                    {
                        result.Add(ado);
                        continue;
                    }
                    if (ado.IsReplace)
                    {
                        pendingReplaces.Add(ado);
                        continue;
                    }
                    if (ado.Requests == null || ado.Requests.Count == 0)
                    {
                        result.Add(ado);
                        continue;
                    }

                    var byTreatment = ado.Requests
                        .GroupBy(r => r.TREATMENT_ID)
                        .OrderBy(g => g.Sum(r => r.AMOUNT - (r.DD_AMOUNT ?? 0)))
                        .ToList();

                    decimal remainingAvail = ado.AVAIL_AMOUNT;

                    foreach (var grp in byTreatment)
                    {
                        var reqs = grp.ToList();
                        var splitAdo = CloneMaterialMetadata(ado);
                        splitAdo.Requests = reqs;
                        splitAdo.AMOUNT = reqs.Sum(r => r.AMOUNT);
                        splitAdo.DD_AMOUNT = reqs.Sum(r => r.DD_AMOUNT ?? 0);
                        splitAdo.CURRENT_DD_AMOUNT = splitAdo.DD_AMOUNT;
                        splitAdo.TT_AMOUNT = 0;
                        splitAdo.AVAIL_AMOUNT = remainingAvail < 0 ? 0 : remainingAvail;
                        ApplyPatientInfo(splitAdo, grp.Key);

                        decimal needApprove = splitAdo.AMOUNT - splitAdo.DD_AMOUNT;
                        splitAdo.YCD_AMOUNT = Math.Min(needApprove, splitAdo.AVAIL_AMOUNT);
                        if (splitAdo.YCD_AMOUNT < 0) splitAdo.YCD_AMOUNT = 0;
                        if (!splitAdo.IS_ALLOW_EXPORT_ODD)
                            splitAdo.YCD_AMOUNT = (int)splitAdo.YCD_AMOUNT;
                        splitAdo.IsCheck = splitAdo.YCD_AMOUNT > 0;

                        remainingAvail -= splitAdo.YCD_AMOUNT;
                        result.Add(splitAdo);
                    }
                }

                var requestLookup = result
                    .Where(o => !o.IsReplace && !o.IsApproved && o.TREATMENT_ID.HasValue)
                    .GroupBy(o => o.MATERIAL_TYPE_ID)
                    .ToDictionary(g => g.Key, g => g.OrderBy(x => x.AMOUNT - x.CURRENT_DD_AMOUNT).ToList());

                foreach (var repl in pendingReplaces)
                {
                    if (!repl.REPLACE_MATERIAL_TYPE_ID.HasValue
                        || !requestLookup.ContainsKey(repl.REPLACE_MATERIAL_TYPE_ID.Value)
                        || requestLookup[repl.REPLACE_MATERIAL_TYPE_ID.Value].Count == 0)
                    {
                        result.Add(repl);
                        continue;
                    }

                    var reqBNs = requestLookup[repl.REPLACE_MATERIAL_TYPE_ID.Value];
                    decimal remaining = repl.YCD_AMOUNT;
                    bool addedAny = false;

                    foreach (var reqBN in reqBNs)
                    {
                        if (remaining <= 0) break;
                        decimal needBN = reqBN.AMOUNT - reqBN.CURRENT_DD_AMOUNT;
                        if (needBN <= 0) continue;
                        decimal alloc = Math.Min(remaining, needBN);

                        var splitRepl = CloneMaterialMetadata(repl);
                        splitRepl.AMOUNT = 0;
                        splitRepl.YCD_AMOUNT = alloc;
                        splitRepl.IsCheck = true;
                        splitRepl.Requests = reqBN.Requests;
                        ApplyPatientInfo(splitRepl, reqBN.TREATMENT_ID);
                        result.Add(splitRepl);
                        remaining -= alloc;
                        addedAny = true;
                    }

                    if (remaining > 0 && !addedAny)
                    {
                        var first = reqBNs.First();
                        var splitRepl = CloneMaterialMetadata(repl);
                        splitRepl.AMOUNT = 0;
                        splitRepl.YCD_AMOUNT = remaining;
                        splitRepl.IsCheck = true;
                        splitRepl.Requests = first.Requests;
                        ApplyPatientInfo(splitRepl, first.TREATMENT_ID);
                        result.Add(splitRepl);
                    }
                }

                materialAdos = result;
                isSplitByPatient = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Gộp các ADO đang tách theo bệnh nhân về dạng group theo MEDICINE_TYPE_ID.
        /// Row thay thế gộp theo (MEDICINE_TYPE_ID + REPLACE_MEDICINE_TYPE_ID).
        /// Nếu người dùng đã sửa YCD_AMOUNT → tính cộng dồn cho row đại diện.
        /// </summary>
        private void GroupMedicineAdosByType()
        {
            try
            {
                if (medicineAdos == null) return;
                var result = new List<MedicineTypeADO>();

                // Row yêu cầu (IsReplace=false, IsApproved=false) → gộp theo MEDICINE_TYPE_ID
                var requestGroups = medicineAdos
                    .Where(o => !o.IsReplace && !o.IsApproved)
                    .GroupBy(o => o.MEDICINE_TYPE_ID)
                    .ToList();

                foreach (var grp in requestGroups)
                {
                    var first = grp.First();
                    var merged = CloneMedicineMetadata(first);
                    merged.Requests = new List<HIS_EXP_MEST_METY_REQ>();
                    foreach (var a in grp)
                    {
                        if (a.Requests != null) merged.Requests.AddRange(a.Requests);
                    }
                    merged.AMOUNT = merged.Requests.Sum(r => r.AMOUNT);
                    merged.DD_AMOUNT = merged.Requests.Sum(r => r.DD_AMOUNT ?? 0);
                    merged.CURRENT_DD_AMOUNT = merged.DD_AMOUNT;
                    // YCD: cộng dồn các row split đã tick
                    merged.YCD_AMOUNT = grp.Where(a => a.IsCheck).Sum(a => a.YCD_AMOUNT);
                    if (merged.YCD_AMOUNT < 0) merged.YCD_AMOUNT = 0;
                    if (!merged.IS_ALLOW_EXPORT_ODD)
                        merged.YCD_AMOUNT = (int)merged.YCD_AMOUNT;
                    merged.IsCheck = merged.YCD_AMOUNT > 0;
                    merged.TREATMENT_ID = null;
                    merged.PATIENT_NAME = "";
                    merged.TREATMENT_CODE = "";
                    result.Add(merged);
                }

                // Row thay thế/đã duyệt → gộp theo (MEDICINE_TYPE_ID + REPLACE_MEDICINE_TYPE_ID)
                var otherGroups = medicineAdos
                    .Where(o => o.IsReplace || o.IsApproved)
                    .GroupBy(o => new { o.MEDICINE_TYPE_ID, o.REPLACE_MEDICINE_TYPE_ID, o.IsApproved })
                    .ToList();

                foreach (var grp in otherGroups)
                {
                    var first = grp.First();
                    var merged = CloneMedicineMetadata(first);
                    merged.Requests = new List<HIS_EXP_MEST_METY_REQ>();
                    foreach (var a in grp)
                    {
                        if (a.Requests != null) merged.Requests.AddRange(a.Requests);
                    }
                    merged.AMOUNT = grp.Sum(a => a.AMOUNT);
                    merged.DD_AMOUNT = grp.Sum(a => a.DD_AMOUNT);
                    merged.YCD_AMOUNT = grp.Where(a => a.IsCheck).Sum(a => a.YCD_AMOUNT);
                    if (merged.YCD_AMOUNT < 0) merged.YCD_AMOUNT = 0;
                    if (!merged.IS_ALLOW_EXPORT_ODD)
                        merged.YCD_AMOUNT = (int)merged.YCD_AMOUNT;
                    merged.IsCheck = merged.YCD_AMOUNT > 0 || merged.IsApproved;
                    merged.TREATMENT_ID = null;
                    merged.PATIENT_NAME = "";
                    merged.TREATMENT_CODE = "";
                    result.Add(merged);
                }

                medicineAdos = result;
                isSplitByPatient = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GroupMaterialAdosByType()
        {
            try
            {
                if (materialAdos == null) return;
                var result = new List<MaterialTypeADO>();

                var requestGroups = materialAdos
                    .Where(o => !o.IsReplace && !o.IsApproved)
                    .GroupBy(o => o.MATERIAL_TYPE_ID)
                    .ToList();

                foreach (var grp in requestGroups)
                {
                    var first = grp.First();
                    var merged = CloneMaterialMetadata(first);
                    merged.Requests = new List<HIS_EXP_MEST_MATY_REQ>();
                    foreach (var a in grp)
                    {
                        if (a.Requests != null) merged.Requests.AddRange(a.Requests);
                    }
                    merged.AMOUNT = merged.Requests.Sum(r => r.AMOUNT);
                    merged.DD_AMOUNT = merged.Requests.Sum(r => r.DD_AMOUNT ?? 0);
                    merged.CURRENT_DD_AMOUNT = merged.DD_AMOUNT;
                    merged.YCD_AMOUNT = grp.Where(a => a.IsCheck).Sum(a => a.YCD_AMOUNT);
                    if (merged.YCD_AMOUNT < 0) merged.YCD_AMOUNT = 0;
                    if (!merged.IS_ALLOW_EXPORT_ODD)
                        merged.YCD_AMOUNT = (int)merged.YCD_AMOUNT;
                    merged.IsCheck = merged.YCD_AMOUNT > 0;
                    merged.TREATMENT_ID = null;
                    merged.PATIENT_NAME = "";
                    merged.TREATMENT_CODE = "";
                    result.Add(merged);
                }

                var otherGroups = materialAdos
                    .Where(o => o.IsReplace || o.IsApproved)
                    .GroupBy(o => new { o.MATERIAL_TYPE_ID, o.REPLACE_MATERIAL_TYPE_ID, o.IsApproved })
                    .ToList();

                foreach (var grp in otherGroups)
                {
                    var first = grp.First();
                    var merged = CloneMaterialMetadata(first);
                    merged.Requests = new List<HIS_EXP_MEST_MATY_REQ>();
                    foreach (var a in grp)
                    {
                        if (a.Requests != null) merged.Requests.AddRange(a.Requests);
                    }
                    merged.AMOUNT = grp.Sum(a => a.AMOUNT);
                    merged.DD_AMOUNT = grp.Sum(a => a.DD_AMOUNT);
                    merged.YCD_AMOUNT = grp.Where(a => a.IsCheck).Sum(a => a.YCD_AMOUNT);
                    if (merged.YCD_AMOUNT < 0) merged.YCD_AMOUNT = 0;
                    if (!merged.IS_ALLOW_EXPORT_ODD)
                        merged.YCD_AMOUNT = (int)merged.YCD_AMOUNT;
                    merged.IsCheck = merged.YCD_AMOUNT > 0 || merged.IsApproved;
                    merged.TREATMENT_ID = null;
                    merged.PATIENT_NAME = "";
                    merged.TREATMENT_CODE = "";
                    result.Add(merged);
                }

                materialAdos = result;
                isSplitByPatient = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkSplitByPatient_CheckedChanged(object sender, EventArgs e)
        {
            if (isNotLoadWhileChangeControlStateInFirst) return;
            try
            {
                WaitingManager.Show();
                if (chkSplitByPatient.Checked)
                {
                    // Spec: list treatment_id rỗng → hiển thị như hiện tại (vẫn lưu state CHECK nhưng không tách).
                    if (!HasAnyTreatmentId())
                    {
                        SetSplitColumnsVisible(false);
                        isSplitByPatient = false;
                        SaveControlState();
                        WaitingManager.Hide();
                        return;
                    }
                    // UNCHECK → CHECK: tách theo BN
                    SplitMedicineAdosByPatient();
                    SplitMaterialAdosByPatient();
                    SetSplitColumnsVisible(true);
                }
                else
                {
                    // CHECK → UNCHECK: gộp lại, cộng dồn YCD
                    if (isSplitByPatient)
                    {
                        GroupMedicineAdosByType();
                        GroupMaterialAdosByType();
                    }
                    SetSplitColumnsVisible(false);
                }

                gridControlMedicine.BeginUpdate();
                gridControlMedicine.DataSource = medicineAdos;
                gridControlMedicine.EndUpdate();

                gridControlMaterial.BeginUpdate();
                gridControlMaterial.DataSource = materialAdos;
                gridControlMaterial.EndUpdate();

                SaveControlState();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSave.Enabled) return;
                //get data 2 grid bỏ bản ghi nào mà có medicine_type_id null or = 0,, material_type_id cũng thế
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                HisExpMestApproveSDO data = new HisExpMestApproveSDO();
                data.ExpMestId = this.currentExpMest.ID;
                data.ReqRoomId = this.currentModuleBase.RoomId;
                data.IsFinish = true;
                data.Description = txtDescription.Text;
                data.Materials = new List<ExpMaterialTypeSDO>();
                data.Medicines = new List<ExpMedicineTypeSDO>();

                if (!this.MakeMaterial(ref data))
                {
                    return;
                }

                if (!this.MakeMedicine(ref data))
                {
                    return;
                }

                if (data.Medicines.Count == 0 && data.Materials.Count == 0)
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn thuốc, vật tư", "Thông báo");
                    return;
                }

                LogSystem.Info("Input api/HisExpMest/Approve: " + LogUtil.TraceData("data", data));
                var rs = new BackendAdapter(param).Post<HisExpMestResultSDO>("api/HisExpMest/Approve", ApiConsumers.MosConsumer, data, param);
                if (rs != null)
                {
                    success = true;
                    resultSDO = rs;
                    this.delegateSelectData(resultSDO);
                    this.LoadDataInStock();
                    this.LoadDataMedicine();
                    this.LoadDataMaterial();

                }

                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool MakeMedicine(ref HisExpMestApproveSDO data)
        {
            if (medicineAdos != null && medicineAdos.Count > 0)
            {
                Mapper.CreateMap<HIS_EXP_MEST_METY_REQ, HIS_EXP_MEST_METY_REQ>();
                List<HIS_EXP_MEST_METY_REQ> metyReqAlls = Mapper.Map<List<HIS_EXP_MEST_METY_REQ>>(this.expMestMetyReqs);
                var checkMedicines = medicineAdos.Where(o => o.MEDICINE_TYPE_ID > 0 && o.IsCheck == true).ToList();
                var requests = checkMedicines.Where(o => !o.IsReplace).ToList();
                foreach (var medicine in requests)
                {
                    decimal ycdAmount = medicine.YCD_AMOUNT;

                    if (ycdAmount <= 0)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng thuốc phải lớn hơn 0", "Thông báo");
                        return false;
                    }

                    if (ycdAmount > medicine.AVAIL_AMOUNT)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Số lượng thuốc lớn hơn tồn kho: {0}", medicine.MEDICINE_TYPE_NAME), "Thông báo");
                        return false;
                    }

                    if (ycdAmount > (medicine.AMOUNT - (medicine.CURRENT_DD_AMOUNT + medicine.CURRENT_YC_AMOUNT)))
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Số lượng thuốc lớn hơn yêu cầu: {0}", medicine.MEDICINE_TYPE_NAME), "Thông báo");
                        return false;
                    }

                    List<HIS_EXP_MEST_METY_REQ> metyReqs = metyReqAlls.Where(o => medicine.Requests.Any(a => a.ID == o.ID)).ToList();

                    metyReqs = metyReqs.OrderBy(o => o.AMOUNT).ToList();

                    if (medicine.SelectedBeans != null && medicine.SelectedBeans.Count > 0)
                    {
                        // Có chọn lô: phân bổ theo từng lô, mỗi lô tạo SDO riêng với MedicineId
                        decimal remainingYcd = ycdAmount;
                        foreach (var bean in medicine.SelectedBeans)
                        {
                            if (remainingYcd <= 0) break;
                            decimal beanYcd = Math.Min(bean.AMOUNT, remainingYcd);
                            foreach (var req in metyReqs)
                            {
                                if (beanYcd <= 0) break;
                                decimal reqAvail = req.AMOUNT - (req.DD_AMOUNT ?? 0);
                                if (reqAvail <= 0) continue;
                                decimal distAmount = Math.Min(reqAvail, beanYcd);
                                ExpMedicineTypeSDO sdo = new ExpMedicineTypeSDO();
                                sdo.Amount = distAmount;
                                sdo.ExpMestMetyReqId = req.ID;
                                sdo.MedicineTypeId = medicine.MEDICINE_TYPE_ID;
                                sdo.TreatmentId = req.TREATMENT_ID;
                                sdo.MedicineId = bean.MEDICINE_ID;
                                req.DD_AMOUNT = (req.DD_AMOUNT ?? 0) + distAmount;
                                beanYcd -= distAmount;
                                remainingYcd -= distAmount;
                                data.Medicines.Add(sdo);
                            }
                        }
                    }
                    else
                    {
                        foreach (var req in metyReqs)
                        {
                            decimal availAmount = req.AMOUNT - (req.DD_AMOUNT ?? 0);
                            if (availAmount <= 0) continue;
                            if (ycdAmount <= 0) break;
                            if (availAmount >= ycdAmount)
                            {
                                ExpMedicineTypeSDO sdo = new ExpMedicineTypeSDO();
                                sdo.Amount = ycdAmount;
                                sdo.ExpMestMetyReqId = req.ID;
                                sdo.MedicineTypeId = medicine.MEDICINE_TYPE_ID;
                                sdo.TreatmentId = req.TREATMENT_ID;
                                req.DD_AMOUNT = (req.DD_AMOUNT ?? 0) + ycdAmount;
                                ycdAmount = 0;
                                data.Medicines.Add(sdo);
                            }
                            else
                            {
                                ExpMedicineTypeSDO sdo = new ExpMedicineTypeSDO();
                                sdo.Amount = availAmount;
                                sdo.ExpMestMetyReqId = req.ID;
                                sdo.MedicineTypeId = medicine.MEDICINE_TYPE_ID;
                                sdo.TreatmentId = req.TREATMENT_ID;
                                req.DD_AMOUNT = (req.DD_AMOUNT ?? 0) + availAmount;
                                ycdAmount = ycdAmount - availAmount;
                                data.Medicines.Add(sdo);
                            }
                        }
                    }
                }

                var replaces = checkMedicines.Where(o => o.IsReplace).ToList();
                foreach (var repl in replaces)
                {
                    // Khi split: match đúng BN
                    var request = isSplitByPatient
                        ? medicineAdos.FirstOrDefault(o => !o.IsReplace && o.MEDICINE_TYPE_ID == repl.REPLACE_MEDICINE_TYPE_ID && o.TREATMENT_ID == repl.TREATMENT_ID)
                        : medicineAdos.FirstOrDefault(o => !o.IsReplace && o.MEDICINE_TYPE_ID == repl.REPLACE_MEDICINE_TYPE_ID);
                    if (request == null) continue;
                    decimal ycdAmount = repl.YCD_AMOUNT;

                    if (ycdAmount <= 0)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng thuốc phải lớn hơn 0", "Thông báo");
                        return false;
                    }

                    if (ycdAmount > repl.AVAIL_AMOUNT)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Số lượng thuốc lớn hơn tồn kho: {0}", repl.MEDICINE_TYPE_NAME), "Thông báo");
                        return false;
                    }
                    if (ycdAmount > (request.AMOUNT - request.CURRENT_DD_AMOUNT))
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Số lượng thuốc lớn hơn yêu cầu: {0} (Thay thế cho {1})", repl.MEDICINE_TYPE_NAME, request.MEDICINE_TYPE_NAME), "Thông báo");
                        return false;
                    }

                    List<HIS_EXP_MEST_METY_REQ> metyReqs = metyReqAlls.Where(o => request.Requests.Any(a => a.ID == o.ID)).ToList();

                    metyReqs = metyReqs.OrderBy(o => o.AMOUNT).ToList();
                    foreach (var req in metyReqs)
                    {
                        decimal availAmount = req.AMOUNT - (req.DD_AMOUNT ?? 0);
                        if (availAmount <= 0) continue;
                        if (ycdAmount <= 0) break;
                        if (availAmount >= ycdAmount)
                        {
                            ExpMedicineTypeSDO sdo = new ExpMedicineTypeSDO();
                            sdo.Amount = ycdAmount;
                            sdo.ExpMestMetyReqId = req.ID;
                            sdo.MedicineTypeId = repl.MEDICINE_TYPE_ID;
                            sdo.TreatmentId = req.TREATMENT_ID;
                            req.DD_AMOUNT = (req.DD_AMOUNT ?? 0) + ycdAmount;
                            ycdAmount = 0;
                            data.Medicines.Add(sdo);
                        }
                        else
                        {
                            ExpMedicineTypeSDO sdo = new ExpMedicineTypeSDO();
                            sdo.Amount = availAmount;
                            sdo.ExpMestMetyReqId = req.ID;
                            sdo.MedicineTypeId = repl.MEDICINE_TYPE_ID;
                            sdo.TreatmentId = req.TREATMENT_ID;
                            req.DD_AMOUNT = (req.DD_AMOUNT ?? 0) + availAmount;
                            ycdAmount = ycdAmount - availAmount;
                            data.Medicines.Add(sdo);
                        }
                    }
                }
            }
            return true;
        }

        private bool MakeMaterial(ref HisExpMestApproveSDO data)
        {
            if (materialAdos != null && materialAdos.Count > 0)
            {
                Mapper.CreateMap<HIS_EXP_MEST_MATY_REQ, HIS_EXP_MEST_MATY_REQ>();
                List<HIS_EXP_MEST_MATY_REQ> matyReqAlls = Mapper.Map<List<HIS_EXP_MEST_MATY_REQ>>(this.expMestMatyReqs);

                var checkMaterials = materialAdos.Where(o => o.MATERIAL_TYPE_ID > 0 && o.IsCheck == true).ToList();
                var requests = checkMaterials.Where(o => !o.IsReplace).ToList();
                foreach (var material in requests)
                {
                    decimal ycdAmount = material.YCD_AMOUNT;

                    if (ycdAmount <= 0)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng vật tư phải lớn hơn 0", "Thông báo");
                        return false;
                    }

                    if (ycdAmount > material.AVAIL_AMOUNT)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Số lượng vật tư lớn hơn tồn kho: {0}", material.MATERIAL_TYPE_NAME), "Thông báo");
                        return false;
                    }

                    if (ycdAmount > (material.AMOUNT - (material.CURRENT_DD_AMOUNT + material.CURRENT_YC_AMOUNT)))
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Số lượng vật tư lớn hơn yêu cầu: {0}", material.MATERIAL_TYPE_NAME), "Thông báo");
                        return false;
                    }

                    List<HIS_EXP_MEST_MATY_REQ> matyReqs = matyReqAlls.Where(o => material.Requests.Any(a => a.ID == o.ID)).ToList();

                    matyReqs = matyReqs.OrderBy(o => o.AMOUNT).ToList();

                    if (material.SelectedBeans != null && material.SelectedBeans.Count > 0)
                    {
                        // Có chọn lô: phân bổ theo từng lô, mỗi lô tạo SDO riêng với MaterialId
                        decimal remainingYcd = ycdAmount;
                        foreach (var bean in material.SelectedBeans)
                        {
                            if (remainingYcd <= 0) break;
                            decimal beanYcd = Math.Min(bean.AMOUNT, remainingYcd);
                            foreach (var req in matyReqs)
                            {
                                if (beanYcd <= 0) break;
                                decimal reqAvail = req.AMOUNT - (req.DD_AMOUNT ?? 0);
                                if (reqAvail <= 0) continue;
                                decimal distAmount = Math.Min(reqAvail, beanYcd);
                                ExpMaterialTypeSDO sdo = new ExpMaterialTypeSDO();
                                sdo.Amount = distAmount;
                                sdo.ExpMestMatyReqId = req.ID;
                                sdo.MaterialTypeId = material.MATERIAL_TYPE_ID;
                                sdo.TreatmentId = req.TREATMENT_ID;
                                // TODO: sdo.MaterialId = bean.MATERIAL_ID; // bật khi MOS.SDO.dll bổ sung ExpMaterialTypeSDO.MaterialId 
                                req.DD_AMOUNT = (req.DD_AMOUNT ?? 0) + distAmount;
                                beanYcd -= distAmount;
                                remainingYcd -= distAmount;
                                data.Materials.Add(sdo);
                            }
                        }
                    }
                    else
                    {
                        foreach (var req in matyReqs)
                        {
                            decimal availAmount = req.AMOUNT - (req.DD_AMOUNT ?? 0);
                            if (availAmount <= 0) continue;
                            if (ycdAmount <= 0) break;
                            if (availAmount >= ycdAmount)
                            {
                                ExpMaterialTypeSDO sdo = new ExpMaterialTypeSDO();
                                sdo.Amount = ycdAmount;
                                sdo.ExpMestMatyReqId = req.ID;
                                sdo.MaterialTypeId = material.MATERIAL_TYPE_ID;
                                sdo.TreatmentId = req.TREATMENT_ID;
                                req.DD_AMOUNT = (req.DD_AMOUNT ?? 0) + ycdAmount;
                                ycdAmount = 0;
                                data.Materials.Add(sdo);
                            }
                            else
                            {
                                ExpMaterialTypeSDO sdo = new ExpMaterialTypeSDO();
                                sdo.Amount = availAmount;
                                sdo.ExpMestMatyReqId = req.ID;
                                sdo.MaterialTypeId = material.MATERIAL_TYPE_ID;
                                sdo.TreatmentId = req.TREATMENT_ID;
                                req.DD_AMOUNT = (req.DD_AMOUNT ?? 0) + availAmount;
                                ycdAmount = ycdAmount - availAmount;
                                data.Materials.Add(sdo);
                            }
                        }
                    }
                }

                var replaces = checkMaterials.Where(o => o.IsReplace).ToList();
                foreach (var repl in replaces)
                {
                    // Khi split: match đúng BN
                    var request = isSplitByPatient
                        ? materialAdos.FirstOrDefault(o => !o.IsReplace && o.MATERIAL_TYPE_ID == repl.REPLACE_MATERIAL_TYPE_ID && o.TREATMENT_ID == repl.TREATMENT_ID)
                        : materialAdos.FirstOrDefault(o => !o.IsReplace && o.MATERIAL_TYPE_ID == repl.REPLACE_MATERIAL_TYPE_ID);
                    if (request == null) continue;
                    decimal ycdAmount = repl.YCD_AMOUNT;

                    if (ycdAmount <= 0)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng vật tư phải lớn hơn 0", "Thông báo");
                        return false;
                    }

                    if (ycdAmount > repl.AVAIL_AMOUNT)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Số lượng vật tư lớn hơn tồn kho: {0}", repl.MATERIAL_TYPE_NAME), "Thông báo");
                        return false;
                    }
                    if (ycdAmount > (request.AMOUNT - request.CURRENT_DD_AMOUNT))
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Số lượng vật tư lớn hơn yêu cầu: {0} (Thay thế cho {1})", repl.MATERIAL_TYPE_NAME, request.MATERIAL_TYPE_NAME), "Thông báo");
                        return false;
                    }

                    List<HIS_EXP_MEST_MATY_REQ> matyReqs = matyReqAlls.Where(o => request.Requests.Any(a => a.ID == o.ID)).ToList(); ;

                    matyReqs = matyReqs.OrderBy(o => o.AMOUNT).ToList();
                    foreach (var req in matyReqs)
                    {
                        decimal availAmount = req.AMOUNT - (req.DD_AMOUNT ?? 0);
                        if (availAmount <= 0) continue;
                        if (ycdAmount <= 0) break;
                        if (availAmount >= ycdAmount)
                        {
                            ExpMaterialTypeSDO sdo = new ExpMaterialTypeSDO();
                            sdo.Amount = ycdAmount;
                            sdo.ExpMestMatyReqId = req.ID;
                            sdo.MaterialTypeId = repl.MATERIAL_TYPE_ID;
                            sdo.TreatmentId = req.TREATMENT_ID;
                            req.DD_AMOUNT = (req.DD_AMOUNT ?? 0) + ycdAmount;
                            ycdAmount = 0;
                            data.Materials.Add(sdo);
                        }
                        else
                        {
                            ExpMaterialTypeSDO sdo = new ExpMaterialTypeSDO();
                            sdo.Amount = availAmount;
                            sdo.ExpMestMatyReqId = req.ID;
                            sdo.MaterialTypeId = repl.MATERIAL_TYPE_ID;
                            sdo.TreatmentId = req.TREATMENT_ID;
                            req.DD_AMOUNT = (req.DD_AMOUNT ?? 0) + availAmount;
                            ycdAmount = ycdAmount - availAmount;
                            data.Materials.Add(sdo);
                        }
                    }
                }
            }
            return true;
        }

        private void repositoryItemButtonReplaceMedicine_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                focusMedicine = (MedicineTypeADO)gridViewMedicine.GetFocusedRow();
                if (focusMedicine != null)
                {
                    this.ReplaceForm(focusMedicine);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonReplaceMaterial_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                focusMaterial = (MaterialTypeADO)gridViewMaterial.GetFocusedRow();
                if (focusMaterial != null)
                {
                    this.ReplaceForm(focusMaterial);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        MedicineTypeADO focusMedicine = null;

        private void ReplaceForm(MedicineTypeADO expMestFocus)
        {
            try
            {
                frmReplace frm = new frmReplace(expMestFocus, this.currentModuleBase.RoomId, ReplaceMedicineSave);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReplaceMedicineSave(object prescription)
        {
            try
            {
                if (prescription != null && prescription is MetyMatyTypeADO)
                {
                    var expMestMedicineSelect = (MetyMatyTypeADO)prescription;
                    // thuốc thay thế
                    MedicineTypeADO replaceMedicine = new MedicineTypeADO();
                    replaceMedicine.REPLACE_MEDICINE_TYPE_ID = focusMedicine.MEDICINE_TYPE_ID;
                    replaceMedicine.REPLACE_MEDICINE_TYPE_NAME = focusMedicine.MEDICINE_TYPE_NAME;
                    replaceMedicine.MEDICINE_TYPE_CODE = expMestMedicineSelect.MedicineTypeCode;
                    replaceMedicine.MEDICINE_TYPE_NAME = expMestMedicineSelect.MedicineTypeName;
                    replaceMedicine.MEDICINE_TYPE_ID = expMestMedicineSelect.Id;
                    replaceMedicine.ACTIVE_INGR_BHYT_NAME = expMestMedicineSelect.ActiveIngrBhytName;
                    replaceMedicine.ACTIVE_INGR_BHYT_CODE = expMestMedicineSelect.ActiveIngrBhytCode;
                    replaceMedicine.SERVICE_UNIT_NAME = expMestMedicineSelect.ServiceUnitName;
                    replaceMedicine.AVAIL_AMOUNT = expMestMedicineSelect.AvailableAmount ?? 0;
                    replaceMedicine.IsCheck = true;
                    replaceMedicine.IsReplace = true;
                    replaceMedicine.AMOUNT = 0;
                    replaceMedicine.YCD_AMOUNT = expMestMedicineSelect.YCD_AMOUNT;
                    // Khi đang split: copy patient info + Requests của BN đang thay thế
                    if (isSplitByPatient)
                    {
                        replaceMedicine.TREATMENT_ID = focusMedicine.TREATMENT_ID;
                        replaceMedicine.PATIENT_NAME = focusMedicine.PATIENT_NAME;
                        replaceMedicine.TREATMENT_CODE = focusMedicine.TREATMENT_CODE;
                        replaceMedicine.Requests = focusMedicine.Requests;
                        medicineAdos.RemoveAll(o => o.REPLACE_MEDICINE_TYPE_ID == focusMedicine.MEDICINE_TYPE_ID
                            && o.TREATMENT_ID == focusMedicine.TREATMENT_ID
                            && !o.IsApproved);
                    }
                    else
                    {
                        // nếu đã tồn tại thuốc thay thế thì remove
                        medicineAdos.RemoveAll(o => o.REPLACE_MEDICINE_TYPE_ID == focusMedicine.MEDICINE_TYPE_ID && !o.IsApproved);
                    }
                    medicineAdos.Add(replaceMedicine);

                    foreach (var item in medicineAdos)
                    {
                        if (item == focusMedicine)
                        {
                            item.YCD_AMOUNT = ((item.AMOUNT - replaceMedicine.YCD_AMOUNT) >= item.YCD_AMOUNT ? item.YCD_AMOUNT : (item.AMOUNT - replaceMedicine.YCD_AMOUNT));
                            item.CURRENT_YC_AMOUNT = replaceMedicine.YCD_AMOUNT;
                        }
                        if (item.YCD_AMOUNT <= 0)
                        {
                            item.IsCheck = false;
                        }
                    }
                    gridControlMedicine.BeginUpdate();
                    gridControlMedicine.DataSource = medicineAdos;
                    gridControlMedicine.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        MaterialTypeADO focusMaterial = null;

        private void ReplaceForm(MaterialTypeADO expMestFocus)
        {
            try
            {
                frmReplace frm = new frmReplace(expMestFocus, this.currentModuleBase.RoomId, ReplaceMaterialSave);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReplaceMaterialSave(object prescription)
        {
            try
            {
                if (prescription != null && prescription is MetyMatyTypeADO)
                {
                    var materialSelect = (MetyMatyTypeADO)prescription;
                    // thuốc thay thế
                    MaterialTypeADO replaceMaterial = new MaterialTypeADO();
                    replaceMaterial.REPLACE_MATERIAL_TYPE_ID = focusMaterial.MATERIAL_TYPE_ID;
                    replaceMaterial.REPLACE_MATERIAL_TYPE_NAME = focusMaterial.MATERIAL_TYPE_NAME;
                    replaceMaterial.MATERIAL_TYPE_CODE = materialSelect.MedicineTypeCode;
                    replaceMaterial.MATERIAL_TYPE_NAME = materialSelect.MedicineTypeName;
                    replaceMaterial.MATERIAL_TYPE_ID = materialSelect.Id;
                    replaceMaterial.ACTIVE_INGR_BHYT_NAME = materialSelect.ActiveIngrBhytName;
                    replaceMaterial.SERVICE_UNIT_NAME = materialSelect.ServiceUnitName;
                    replaceMaterial.AVAIL_AMOUNT = materialSelect.AvailableAmount ?? 0;
                    replaceMaterial.IsCheck = true;
                    replaceMaterial.IsReplace = true;
                    replaceMaterial.AMOUNT = 0;
                    replaceMaterial.YCD_AMOUNT = materialSelect.YCD_AMOUNT;
                    // Khi đang split: copy patient info + Requests của BN đang thay thế
                    if (isSplitByPatient)
                    {
                        replaceMaterial.TREATMENT_ID = focusMaterial.TREATMENT_ID;
                        replaceMaterial.PATIENT_NAME = focusMaterial.PATIENT_NAME;
                        replaceMaterial.TREATMENT_CODE = focusMaterial.TREATMENT_CODE;
                        replaceMaterial.Requests = focusMaterial.Requests;
                        materialAdos.RemoveAll(o => o.REPLACE_MATERIAL_TYPE_ID == focusMaterial.MATERIAL_TYPE_ID
                            && o.TREATMENT_ID == focusMaterial.TREATMENT_ID
                            && !o.IsApproved);
                    }
                    else
                    {
                        // nếu đã tồn tại vật tư thay thế thì remove
                        materialAdos.RemoveAll(o => o.REPLACE_MATERIAL_TYPE_ID == focusMaterial.MATERIAL_TYPE_ID && !o.IsApproved);
                    }
                    materialAdos.Add(replaceMaterial);

                    foreach (var item in materialAdos)
                    {
                        if (item == focusMaterial)
                        {
                            item.YCD_AMOUNT = ((item.AMOUNT - replaceMaterial.YCD_AMOUNT) >= item.YCD_AMOUNT ? item.YCD_AMOUNT : (item.AMOUNT - replaceMaterial.YCD_AMOUNT));
                            item.CURRENT_YC_AMOUNT = replaceMaterial.YCD_AMOUNT;
                        }
                        if (item.YCD_AMOUNT <= 0)
                        {
                            item.IsCheck = false;
                        }
                    }
                    gridControlMaterial.BeginUpdate();
                    gridControlMaterial.DataSource = materialAdos;
                    gridControlMaterial.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void repositoryItemCheckMedicine_Enable_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                gridViewMedicine.PostEditor();
                gridControlMedicine.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCheckMaterial_Enable_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                gridViewMaterial.PostEditor();
                gridControlMaterial.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridControlMedicine_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button != MouseButtons.Right) return;
                var hitInfo = gridViewMedicine.CalcHitInfo(e.Location);
                if (!hitInfo.InRow || hitInfo.RowHandle < 0) return;
                gridViewMedicine.FocusedRowHandle = hitInfo.RowHandle;
                var data = (MedicineTypeADO)gridViewMedicine.GetRow(hitInfo.RowHandle);
                if (data != null)
                    OpenSelectLoForm(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridControlMaterial_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button != MouseButtons.Right) return;
                var hitInfo = gridViewMaterial.CalcHitInfo(e.Location);
                if (!hitInfo.InRow || hitInfo.RowHandle < 0) return;
                gridViewMaterial.FocusedRowHandle = hitInfo.RowHandle;
                var data = (MaterialTypeADO)gridViewMaterial.GetRow(hitInfo.RowHandle);
                if (data != null)
                    OpenSelectLoForm(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void OpenSelectLoForm(MedicineTypeADO ado)
        {
            try
            {
                frmSelectLo frm = new frmSelectLo(ado, this.mediStock.ID);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    gridControlMedicine.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void OpenSelectLoForm(MaterialTypeADO ado)
        {
            try
            {
                frmSelectLo frm = new frmSelectLo(ado, this.mediStock.ID);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    gridControlMaterial.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
