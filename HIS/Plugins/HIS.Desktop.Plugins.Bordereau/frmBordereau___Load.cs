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
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.Bordereau.ADO;
using HIS.Desktop.Plugins.Bordereau.ChooseCondition;
using HIS.Desktop.Plugins.Bordereau.Config;
using HIS.Desktop.Plugins.Library.PrintBordereau;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
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

namespace HIS.Desktop.Plugins.Bordereau
{
    public partial class frmBordereau : FormBase
    {
        private List<HIS_SERE_SERV> GetSereServWithMinDuration(long patientId, List<long> serviceIds)
        {
            List<HIS_SERE_SERV> results = new List<HIS_SERE_SERV>();
            try
            {
                if (serviceIds == null || serviceIds.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong truyen danh sach serviceids");
                    return null;
                }

                List<V_HIS_SERVICE> services = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>(false, true)
                    .Where(o => serviceIds.Contains(o.ID) && o.MIN_DURATION.HasValue).ToList();
                if (services == null)
                    return null;

                List<ServiceDuration> serviceDurations = new List<ServiceDuration>();
                foreach (var item in services)
                {
                    ServiceDuration sd = new ServiceDuration();
                    sd.MinDuration = item.MIN_DURATION.Value;
                    sd.ServiceId = item.ID;
                    serviceDurations.Add(sd);
                }

                CommonParam param = new CommonParam();
                HisSereServMinDurationFilter hisSereServMinDurationFilter = new HisSereServMinDurationFilter();
                hisSereServMinDurationFilter.ServiceDurations = serviceDurations;
                hisSereServMinDurationFilter.PatientId = patientId;
                hisSereServMinDurationFilter.InstructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                results = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/GetExceedMinDuration", ApiConsumer.ApiConsumers.MosConsumer, hisSereServMinDurationFilter, param);

                if (results != null && results.Count > 0)
                {
                    var listSereServResultTemp = from SereServResult in results
                                                 group SereServResult by SereServResult.SERVICE_ID into g
                                                 orderby g.Key
                                                 select g.FirstOrDefault();
                    results = listSereServResultTemp.ToList();
                }
            }
            catch (Exception ex)
            {
                results = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return results;
        }

        internal void LoadCurrentTreatmentData()
        {
            try
            {
                if (treatmentId > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTreatmentViewFilter filter = new MOS.Filter.HisTreatmentViewFilter();
                    filter.ID = treatmentId;
                    currentTreatment = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadSereServDeposit()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSereServDepositFilter filter = new HisSereServDepositFilter();
                filter.TDL_TREATMENT_ID = treatmentId;
                sereServDeposits = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadMedicineMaterial()
        {
            try
            {
                var medicineIdList = this.SereServADOs
                   .Where(o => o.MEDICINE_ID.HasValue && o.MEDICINE_ID.Value > 0)
                   .Select(p => p.MEDICINE_ID.Value).Distinct().ToList();

                var materialIdList = this.SereServADOs
                    .Where(o => o.MATERIAL_ID.HasValue && o.MATERIAL_ID.Value > 0)
                    .Select(p => p.MATERIAL_ID.Value).Distinct().ToList();

                if (medicineIdList != null && medicineIdList.Count > 0)
                {
                    MOS.Filter.HisMedicineFilter medicineFilter = new HisMedicineFilter();
                    medicineFilter.IDs = medicineIdList;
                    this.MedicineList = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumer.ApiConsumers.MosConsumer, medicineFilter, null);
                }

                if (materialIdList != null && materialIdList.Count > 0)
                {
                    MOS.Filter.HisMaterialFilter materialFilter = new HisMaterialFilter();
                    materialFilter.IDs = materialIdList;
                    this.MaterialList = new BackendAdapter(new CommonParam()).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumer.ApiConsumers.MosConsumer, materialFilter, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void LoadCurrentPatientTypeAlter()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisPatientTypeAlterViewFilter filter = new HisPatientTypeAlterViewFilter();
                filter.TREATMENT_ID = treatmentId;
                currentHisPatientTypeAlters = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private List<MOS.EFMODEL.DataModels.HIS_OTHER_PAY_SOURCE> LoadDataOtherPaySource()
        {
            List<MOS.EFMODEL.DataModels.HIS_OTHER_PAY_SOURCE> datas = null;
            try
            {
                if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_OTHER_PAY_SOURCE>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_OTHER_PAY_SOURCE>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    HisOtherPaySourceFilter filter = new HisOtherPaySourceFilter();
                    datas = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_OTHER_PAY_SOURCE>>("api/HisOtherPaySource/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_OTHER_PAY_SOURCE), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
            }
            catch (Exception ex)
            {
                datas = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return datas;
        }

        internal void LoadAndInItComboCondition()
        {
            try
            {
                var dataCondition = BackendDataWorker.Get<HIS_SERVICE_CONDITION>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_CONDITION_CODE", "Mã", 80, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_CONDITION_NAME", "ID", columnInfos, true, 460);
                List<ServiceConditionADO> serviceConditionADOs = (from r in dataCondition select new ServiceConditionADO(r)).ToList();
                ControlEditorLoader.Load(repositoryItemGridLookUpEdit_Condition, serviceConditionADOs, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadAndInItComboOtherPaySource()
        {
            List<MOS.EFMODEL.DataModels.HIS_OTHER_PAY_SOURCE> datas = null;
            try
            {
                datas = OtherPaySources;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("OTHER_PAY_SOURCE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("OTHER_PAY_SOURCE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(repositoryItemGridLookUpEdit_OtherPaySource, datas, controlEditorADO);

                List<ColumnInfo> columnInfos1 = new List<ColumnInfo>();
                columnInfos1.Add(new ColumnInfo("OTHER_PAY_SOURCE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO1 = new ControlEditorADO("OTHER_PAY_SOURCE_NAME", "ID", columnInfos1, false, 250);
                ControlEditorLoader.Load(repositoryItemGridLookUpEdit_OtherPaySource_Disable, datas, controlEditorADO1);
            }
            catch (Exception ex)
            {
                datas = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal List<HIS_PACKAGE> LoadPackage()
        {
            List<HIS_PACKAGE> datas = null;
            try
            {
                if (BackendDataWorker.IsExistsKey<HIS_PACKAGE>())
                {
                    datas = BackendDataWorker.Get<HIS_PACKAGE>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    HisPackageFilter filter = new HisPackageFilter();
                    datas = new BackendAdapter(paramCommon).Get<List<HIS_PACKAGE>>("api/HisPackage/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(HIS_PACKAGE), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
            }
            catch (Exception ex)
            {
                datas = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return datas;
        }

        internal async Task LoadAndFillDataToReposPatientType()
        {
            try
            {
                await LoadCurrentPatientTypeAlterV2();
                Inventec.Common.Logging.LogSystem.Debug("InitComboRespositoryPatientTypeV2----------1");
                InitComboRespositoryPatientTypeV2(this.currentPatientTypeWithPatientTypeAlter);
                Inventec.Common.Logging.LogSystem.Debug("InitComboRespositoryPatientTypeV2----------2");
                Inventec.Common.Logging.LogSystem.Debug("InitComboRespositoryPrimaryPatientTypeV2----------1");
                InitComboRespositoryPrimaryPatientTypeV2(this.currentPatientTypeWithPatientTypeAlter);
                Inventec.Common.Logging.LogSystem.Debug("InitComboRespositoryPrimaryPatientTypeV2----------2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal async Task LoadCurrentPatientTypeAlterV2()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisPatientTypeAlterViewFilter filter = new HisPatientTypeAlterViewFilter();
                filter.TREATMENT_ID = treatmentId;
                currentHisPatientTypeAlters = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<HIS_PATIENT_TYPE> PatientTypeWithPatientTypeAlter()
        {
            List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> result = null;
            try
            {
                result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>(false, true);
                return result;
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private async Task InitComboRespositoryPatientTypeV2(List<HIS_PATIENT_TYPE> currentPatientTypeWithPatientTypeAlter)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(repositoryItemGridLookUpEdit_PatientType, currentPatientTypeWithPatientTypeAlter, controlEditorADO);

                List<ColumnInfo> columnInfo1s = new List<ColumnInfo>();
                columnInfo1s.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditor1ADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(repositoryItemLookUpEdit_PatientType_Disable, currentPatientTypeWithPatientTypeAlter, controlEditor1ADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task InitComboRespositoryPrimaryPatientTypeV2(List<HIS_PATIENT_TYPE> currentPatientTypeWithPatientTypeAlter)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(repositoryItemGridLookUpEditPrimaryPatientType, currentPatientTypeWithPatientTypeAlter, controlEditorADO);

                List<ColumnInfo> columnInfo1s = new List<ColumnInfo>();
                columnInfo1s.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditor1ADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(repositoryItemGridLookUpEditPrimaryPatientType_Disabled, currentPatientTypeWithPatientTypeAlter, controlEditor1ADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task InitComboRespositorySereServPackageV2()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("InitComboRespositorySereServPackageV2----------1---" + this.SereServADOs.Count);

                var sereServIsPackage2s = this.SereServADOs.Where(o =>
                    ((o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G
                    || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS
                    || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA
                    || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN
                    || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA))).ToList();

                if (sereServIsPackage2s == null || sereServIsPackage2s.Count == 0)
                {
                    //gridColumnDvDinhKem.Visible = false;
                    return;
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_REQ_CODE___SERVICE_CODE_NAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_NAME", "", 250, 2));
                ControlEditorADO controlEditor1ADO = new ControlEditorADO("SERVICE_REQ_CODE___SERVICE_CODE_NAME", "ID", columnInfos, false, 250);
                Inventec.Common.Logging.LogSystem.Debug("repositoryItemLookUpEditSereServPackage_Enabled----------1");
                ControlEditorLoader.Load(repositoryItemLookUpEditSereServPackage_Enabled, sereServIsPackage2s, controlEditor1ADO);
                Inventec.Common.Logging.LogSystem.Debug("repositoryItemLookUpEditSereServPackage_Enabled----------1");
                Inventec.Common.Logging.LogSystem.Debug("repositoryItemLookUpEditSereServPackage_Disabled----------1");
                ControlEditorLoader.Load(repositoryItemLookUpEditSereServPackage_Disabled, sereServIsPackage2s, controlEditor1ADO);
                Inventec.Common.Logging.LogSystem.Debug("repositoryItemLookUpEditSereServPackage_Disabled----------2");
                Inventec.Common.Logging.LogSystem.Debug("InitComboRespositorySereServPackageV2----------2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task initLoadPakageTackV2()
        {
            try
            {
                string packageCode3Day7Day = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PACKAGE.PACKAGE_CODE.3DAY7DAY");
                //List<HIS_PACKAGE> packages = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PACKAGE>(false, true);
                List<HIS_PACKAGE> packages = BackendDataWorker.Get<HIS_PACKAGE>();
                if (packages == null || packages.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Error("Khong tim thay thong tin goi nao!");
                }
                HIS_PACKAGE package37 = packages != null ? packages.FirstOrDefault(o => o.PACKAGE_CODE == packageCode3Day7Day) : null;
                this.sereServIsPackages = this.SereServADOs.Where(o =>
                    o.IS_NO_EXECUTE != 1
                    && (
                    (o.PACKAGE_ID.HasValue && package37 != null && o.PACKAGE_ID == package37.ID)
                    || (
                    (o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G
                    || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS
                    || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA
                    || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN
                    || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA)
                        )
                        &&
                        (currentDepartmentId == o.TDL_EXECUTE_DEPARTMENT_ID)
                        )
                        ).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboPayType()
        {
            try
            {
                var data = new PayTypeADO().PayTypeADOs;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("Option", "", 200, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("Option", "Value", columnInfos, false, 200);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                ControlEditorLoader.Load(this.cboPayType, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboPayType()
        {
            try
            {
                InitComboPayType();

                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(this.moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == ControlStateConstant.cboPayType)
                        {
                            this.payOption = (Library.PrintBordereau.Base.PrintOption.PayType)Enum.Parse(typeof(Library.PrintBordereau.Base.PrintOption.PayType), item.VALUE, true);
                        }
                    }
                }
                //giá trị mặc định combobox 'Trạng thái TT' theo key cấu hình 'HIS.Desktop.Plugins.Bordereau.CboPayTypeDefault'
                if (this.cboPayTypeDefault == "1")
                {
                    this.payOption = Library.PrintBordereau.Base.PrintOption.PayType.ALL;
                }
                else if (this.cboPayTypeDefault == "2")
                {
                    this.payOption = Library.PrintBordereau.Base.PrintOption.PayType.NOT_DEPOSIT;
                }
                else if (this.cboPayTypeDefault == "3")
                {
                    this.payOption = Library.PrintBordereau.Base.PrintOption.PayType.DEPOSIT;
                }
                else if (this.cboPayTypeDefault == "4")
                {
                    this.payOption = Library.PrintBordereau.Base.PrintOption.PayType.NOT_BILL;
                }
                else if (this.cboPayTypeDefault == "5")
                {
                    this.payOption = Library.PrintBordereau.Base.PrintOption.PayType.BILL;
                }
                cboPayType.EditValue = this.payOption;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<HIS_SERE_SERV> LoadSereServ()
        {
            List<HIS_SERE_SERV> sereServs = null;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("LoadSereServ-----------1");
                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServFilter hisSerwServFilter = new HisSereServFilter();
                hisSerwServFilter.TREATMENT_ID = treatmentId;
                sereServs = new List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>();
                sereServs = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, hisSerwServFilter, param).ToList();
                Inventec.Common.Logging.LogSystem.Debug("LoadSereServ-----------2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return sereServs;
        }

        private void LoadTotalPriceDataToTestServiceReq()
        {
            try
            {
                if (treatmentId > 0)
                {
                    CommonParam param = new CommonParam();
                    HisTreatmentFeeViewFilter searchFilter = new HisTreatmentFeeViewFilter();
                    searchFilter.ID = treatmentId;
                    treatmentFees = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>(HisRequestUriStore.HIS_TREATMENT_GETFEEVIEW, ApiConsumers.MosConsumer, searchFilter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadTotalPriceDataToTestServiceReqV2()
        {
            try
            {
                await GetTreatmentFeeV2();

                LoadDataFromTreatmentFeeV2();
                LoadPicUpdateQR();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task GetTreatmentFeeV2()
        {
            if (treatmentId > 0)
            {
                CommonParam param = new CommonParam();
                HisTreatmentFeeViewFilter searchFilter = new HisTreatmentFeeViewFilter();
                searchFilter.ID = treatmentId;
                treatmentFees = new BackendAdapter(param)
                .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>(HisRequestUriStore.HIS_TREATMENT_GETFEEVIEW, ApiConsumers.MosConsumer, searchFilter, param);
            }
        }

        private void LoadSereServBill()
        {
            try
            {
                //Load sereServ bill
                HisSereServBillFilter sereServBillFilter = new HisSereServBillFilter();
                sereServBillFilter.TDL_TREATMENT_ID = treatmentId;
                SereServBills = new BackendAdapter(new CommonParam())
                .Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, sereServBillFilter, new CommonParam());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private V_HIS_PATIENT_TYPE_ALTER GetPatientTypeAlterInstructionTime(long instructionTime)
        {
            V_HIS_PATIENT_TYPE_ALTER result = null;
            try
            {
                if (currentHisPatientTypeAlters != null && currentHisPatientTypeAlters.Count > 0)
                {
                    result = currentHisPatientTypeAlters.Where(o => o.LOG_TIME <= instructionTime).OrderByDescending(o => o.LOG_TIME).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void FillDataIntoPatientTypeCombo(MOS.EFMODEL.DataModels.HIS_SERE_SERV data, DevExpress.XtraEditors.GridLookUpEdit patientTypeCombo, long? patientTypeNotShowId, bool isPatientType = false)
        {
            try
            {
                //Get doi tuong benh nhan theo dich vu
                V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = this.GetPatientTypeAlterInstructionTime(data.TDL_INTRUCTION_TIME);

                List<HIS_PATIENT_TYPE_ALLOW> patientTypeAllows = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE_ALLOW>(false, true)
    .Where(o => o.PATIENT_TYPE_ID == patientTypeAlter.PATIENT_TYPE_ID && o.IS_ACTIVE == 1).ToList();

                List<long> patientTypeAllowIds = patientTypeAllows != null ? patientTypeAllows.Select(o => o.PATIENT_TYPE_ALLOW_ID).ToList() : new List<long>();

                var servicePatyInBranchs = BackendDataWorker.Get<V_HIS_SERVICE_PATY>().Where(o => o.BRANCH_ID == LocalStorage.LocalData.WorkPlace.GetBranchId()).ToList();
                if (servicePatyInBranchs != null && servicePatyInBranchs.Count > 0)
                {
                    var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>(false, true).Where(o => o.IS_ACTIVE == 1).ToList();
                    List<HIS_PATIENT_TYPE> dataCombo = new List<HIS_PATIENT_TYPE>();

                    //thêm đối tượng bệnh nhân 
                    if (!patientTypeAllowIds.Contains(this.currentTreatment.TDL_PATIENT_TYPE_ID ?? 0))
                    {
                        patientTypeAllowIds.Add(this.currentTreatment.TDL_PATIENT_TYPE_ID ?? 0);
                    }
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientTypeAllowIds), patientTypeAllowIds));

                    //đối với thuốc vật tư hiển thị các đối tượng được phép đổi
                    if (data.MEDICINE_ID.HasValue && data.MEDICINE_ID.Value > 0 && MedicineList != null && MedicineList.Count > 0)
                    {
                        dataCombo = patientType.Where(o => patientTypeAllowIds.Contains(o.ID)
                                                        && o.ID != HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT
                                                        && o.IS_RATION == null
                                                        && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                        if (patientTypeAlter.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT)
                        {
                            var checkMedicine = MedicineList.FirstOrDefault(o => o.ID == data.MEDICINE_ID.Value);
                            var medicineType = checkMedicine != null
                                ? BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == checkMedicine.MEDICINE_TYPE_ID)
                                : null;
                            if (medicineType != null && (!String.IsNullOrEmpty(medicineType.ACTIVE_INGR_BHYT_CODE) || !string.IsNullOrEmpty(AllowAssignOffListMedicineMaterialHeinCardNumberPrefix) && !string.IsNullOrEmpty(currentTreatment.TDL_HEIN_CARD_NUMBER) && AllowAssignOffListMedicineMaterialHeinCardNumberPrefix.Split(',').ToList().Exists(o => currentTreatment.TDL_HEIN_CARD_NUMBER.StartsWith(o)))
                                && (medicineType.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_TDM || medicineType.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_TL || medicineType.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_UT || (!string.IsNullOrEmpty(AllowAssignOffListMedicineMaterialHeinCardNumberPrefix) && !string.IsNullOrEmpty(currentTreatment.TDL_HEIN_CARD_NUMBER) && AllowAssignOffListMedicineMaterialHeinCardNumberPrefix.Split(',').ToList().Exists(o => currentTreatment.TDL_HEIN_CARD_NUMBER.StartsWith(o)) && medicineType.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_NDM && !String.IsNullOrEmpty(medicineType.ACTIVE_INGR_BHYT_CODE))))
                            {
                                var patyBhyt = patientType.FirstOrDefault(o => o.ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT
                                                                            && o.IS_RATION == null
                                                                            && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                                if (patyBhyt != null)
                                {
                                    dataCombo.Add(patyBhyt);
                                }
                            }
                        }
                    }
                    else if (data.MATERIAL_ID.HasValue && data.MATERIAL_ID.Value > 0 && MaterialList != null && MaterialList.Count > 0)
                    {
                        dataCombo = patientType.Where(o => patientTypeAllowIds.Contains(o.ID)
                                                        && o.ID != HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT
                                                        && o.IS_RATION == null
                                                        && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                        if (patientTypeAlter.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT)
                        {
                            var checkMaterial = MaterialList.FirstOrDefault(o => o.ID == data.MATERIAL_ID.Value);
                            var materialType = checkMaterial != null
                                ? BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == checkMaterial.MATERIAL_TYPE_ID)
                                : null;
                            if (materialType != null && !String.IsNullOrWhiteSpace(materialType.HEIN_SERVICE_BHYT_CODE)
                                && !String.IsNullOrWhiteSpace(materialType.HEIN_SERVICE_BHYT_NAME)
                                && (materialType.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT || materialType.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TDM || materialType.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TL || (!string.IsNullOrEmpty(AllowAssignOffListMedicineMaterialHeinCardNumberPrefix) && !string.IsNullOrEmpty(currentTreatment.TDL_HEIN_CARD_NUMBER) && AllowAssignOffListMedicineMaterialHeinCardNumberPrefix.Split(',').ToList().Exists(o => currentTreatment.TDL_HEIN_CARD_NUMBER.StartsWith(o)) && materialType.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_NDM)))
                            {
                                var patyBhyt = patientType.FirstOrDefault(o => o.ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT
                                                                            && o.IS_RATION == null
                                                                            && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                                if (patyBhyt != null)
                                {
                                    dataCombo.Add(patyBhyt);
                                }
                            }
                        }
                    }
                    else
                    {

                        long branchId = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId();
                        var listPaty = servicePatyInBranchs.Where(o => o.SERVICE_ID == data.SERVICE_ID).ToList();
                        foreach (var item in patientType)
                        {
                            if (item.ID == patientTypeNotShowId || !patientTypeAllowIds.Contains(item.ID))
                                continue;

                            V_HIS_SERVICE_PATY appliedServicePaty = null;

                            appliedServicePaty = MOS.ServicePaty.ServicePatyUtil.GetApplied(listPaty, branchId, data.TDL_EXECUTE_ROOM_ID, data.TDL_REQUEST_ROOM_ID, data.TDL_REQUEST_DEPARTMENT_ID, data.TDL_INTRUCTION_TIME, this.currentTreatment.IN_TIME, data.SERVICE_ID, item.ID, null, null, null, null);
                            if ((data.PACKAGE_ID.HasValue || data.SERVICE_CONDITION_ID.HasValue) && appliedServicePaty == null)
                            {
                                appliedServicePaty = MOS.ServicePaty.ServicePatyUtil.GetApplied(listPaty, branchId, data.TDL_EXECUTE_ROOM_ID, data.TDL_REQUEST_ROOM_ID, data.TDL_REQUEST_DEPARTMENT_ID, data.TDL_INTRUCTION_TIME, this.currentTreatment.IN_TIME, data.SERVICE_ID, item.ID, null, null, data.PACKAGE_ID, data.SERVICE_CONDITION_ID);
                                if (appliedServicePaty == null)
                                {
                                    appliedServicePaty = MOS.ServicePaty.ServicePatyUtil.GetApplied(listPaty, branchId, data.TDL_EXECUTE_ROOM_ID, data.TDL_REQUEST_ROOM_ID, data.TDL_REQUEST_DEPARTMENT_ID, data.TDL_INTRUCTION_TIME, this.currentTreatment.IN_TIME, data.SERVICE_ID, item.ID, null, null, null, data.SERVICE_CONDITION_ID);
                                }

                                if (appliedServicePaty == null)
                                {
                                    appliedServicePaty = MOS.ServicePaty.ServicePatyUtil.GetApplied(listPaty, branchId, data.TDL_EXECUTE_ROOM_ID, data.TDL_REQUEST_ROOM_ID, data.TDL_REQUEST_DEPARTMENT_ID, data.TDL_INTRUCTION_TIME, this.currentTreatment.IN_TIME, data.SERVICE_ID, item.ID, null, null, data.PACKAGE_ID, null);
                                }
                            }

                            if (appliedServicePaty != null)
                            {
                                dataCombo.Add(item);
                            }
                        }
                    }

                    var service = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == data.SERVICE_ID && o.IS_ACTIVE == 1);
                    var employee = BackendDataWorker.Get<HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() && o.IS_ACTIVE == 1);
                    if (isPatientType && service != null && service.DO_NOT_USE_BHYT == 1 && employee != null && employee.IS_ADMIN != 1)
                    {
                        dataCombo = dataCombo.Where(o => o.ID != HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT).ToList();
                    }

                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 250);
                    ControlEditorLoader.Load(patientTypeCombo, dataCombo, controlEditorADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataIntoSereServPackageCombo(DevExpress.XtraEditors.LookUpEdit cbo, SereServADO ss)
        {
            try
            {
                if (sereServIsPackages == null || sereServIsPackages.Count == 0)
                    sereServIsPackages = new List<SereServADO>();
                List<SereServADO> sereServADOTemps = (from r in sereServIsPackages select new SereServADO(r)).ToList();
                if (ss.PARENT_ID.HasValue)
                {
                    SereServADO ssPackage = this.SereServADOs.FirstOrDefault(o => o.ID == ss.PARENT_ID.Value);
                    if (ssPackage != null && !sereServADOTemps.Select(o => o.ID).Contains(ssPackage.ID))
                        sereServADOTemps.Add(ssPackage);
                }

                //Khong hien thi dich vu la chinh no
                sereServADOTemps = sereServADOTemps.Where(o => o.ID != ss.ID).ToList();


                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_REQ_CODE___SERVICE_CODE_NAME", "", 250, 3));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_REQ_CODE___SERVICE_CODE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, sereServADOTemps, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataIntoSereServPackageCombo(SereServADO data, DevExpress.XtraEditors.LookUpEdit cbo)
        {
            try
            {
                var sereServs = new List<SereServADO>();
                if (data.PARENT_ID.HasValue && !sereServIsPackages.Select(o => o.ID).Contains(data.PARENT_ID.Value))
                {
                    SereServADO sereServ = this.SereServADOs.FirstOrDefault(o => o.ID == data.PARENT_ID.Value);
                    if (sereServ != null && sereServ.IS_NO_EXECUTE != 1)
                        sereServs.Add(sereServ);
                }

                sereServs.AddRange(sereServIsPackages);
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_REQ_CODE___SERVICE_CODE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_REQ_CODE___SERVICE_CODE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, sereServs, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboPeopleReturnResult(object cbo)
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>(false, true);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToBorderauAndPrint()
        {
            try
            {
                ThreadCustomManager.ThreadResultCallBack(LoadTotalPriceDataToTestServiceReq, CallBackLoadTreatmentFee);
                List<HIS_SERE_SERV> hisSereServs = null;
                List<Action> methods = new List<Action>();
                methods.Add(LoadCurrentTreatmentData);
                methods.Add(LoadSereServBill);
                methods.Add(LoadSereServDeposit);
                methods.Add(() => { hisSereServs = LoadSereServ(); });
                methods.Add(LoadCurrentPatientTypeAlter);
                WaitingManager.Show();

                ThreadCustomManager.MultipleThreadWithJoin(methods);

                //this.InitColumnVisable(hisSereServs);

                this.SereServADOs = JoinToSereServADO(hisSereServs);

                WaitingManager.Hide();
                if (SereServADOs != null && SereServADOs.Count > 0)
                {
                    var sereServADOTemps = this.SereServADOs.OrderByDescending(o => o.TDL_INTRUCTION_TIME).ToList(); ;
                    if (chkAmount.Checked)
                    {
                        sereServADOTemps = sereServADOTemps.Where(o => o.AMOUNT > 0).ToList();
                    }

                    gridControlBordereau.DataSource = sereServADOTemps;
                }

                Inventec.Common.Logging.LogSystem.Debug("FillDataToButtonPrint----1");
                FillDataToButtonPrint();
                Inventec.Common.Logging.LogSystem.Debug("FillDataToButtonPrint----2");

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataToBorderauAndPrintV2()
        {
            try
            {
                // ThreadCustomManager.ThreadResultCallBack(LoadTotalPriceDataToTestServiceReq, CallBackLoadTreatmentFee);
                List<HIS_SERE_SERV> hisSereServs = null;
                //List<HIS_SERE_SERV_EXT> hisSereServExts = null;

                List<Action> methods = new List<Action>();
                methods.Add(LoadCurrentTreatmentData);
                methods.Add(LoadSereServBill);
                methods.Add(LoadSereServDeposit);
                methods.Add(() => { hisSereServs = LoadSereServ(); });
                methods.Add(LoadCurrentPatientTypeAlter);
                WaitingManager.Show();

                ThreadCustomManager.MultipleThreadWithJoin(methods);

                if (hisSereServs != null && hisSereServs.Count > 0)
                {
                    SetTxtFilm(hisSereServs, IMSys.DbConfig.HIS_RS.HIS_DIIM_TYPE.ID__XQ, lblXquang);
                    SetTxtFilm(hisSereServs, IMSys.DbConfig.HIS_RS.HIS_DIIM_TYPE.ID__MRI, lblMri);
                    SetTxtFilm(hisSereServs, IMSys.DbConfig.HIS_RS.HIS_DIIM_TYPE.ID__CT, lblCt);
                }

                this.packages = this.LoadPackage();
                this.OtherPaySources = LoadDataOtherPaySource();
                LoadAndInItComboOtherPaySource();
                LoadAndInItComboCondition();
                //this.InitColumnVisable(hisSereServs);
                List<SereServADO> sereServADODisplay = new List<SereServADO>();
                Inventec.Common.Logging.LogSystem.Debug("JoinToSereServADO-----------1");
                this.SereServADOs = JoinToSereServADO(hisSereServs);
                Inventec.Common.Logging.LogSystem.Debug("JoinToSereServADO-----------2");
                WaitingManager.Hide();

                if (SereServADOs != null && SereServADOs.Count > 0)
                {
                    InitComboRespositorySereServPackageV2();//xuandv==> gan luon du lieu goi tai day ==> hieu nag
                    var sereServADOTemps = this.SereServADOs.OrderByDescending(o => o.TDL_INTRUCTION_TIME).ToList(); ;
                    if (chkAmount.Checked)
                    {
                        sereServADOTemps = sereServADOTemps.Where(o => o.AMOUNT > 0).ToList();
                    }

                    sereServADODisplay.AddRange(sereServADOTemps);
                    if (chkAssignBlood.Checked)
                    {
                        HisExpMestBltyReqView2Filter ft = new HisExpMestBltyReqView2Filter();
                        ft.TDL_TREATMENT_ID = this.currentTreatment.ID;
                        ft.EXP_MEST_STT_IDs = new List<long> {
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE };
                        var dt = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_BLTY_REQ_2>>("api/HisExpMestBltyReq/GetView2", ApiConsumers.MosConsumer, ft, null);

                        foreach (var item in dt)
                        {
                            var service = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == item.SERVICE_ID);
                            SereServADO ado = new SereServADO(item, service);
                            sereServADODisplay.Add(ado);
                        }

                    }
                    gridControlBordereau.DataSource = sereServADODisplay;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServADOTemps), sereServADOTemps));
                }

                Inventec.Common.Logging.LogSystem.Debug("FillDataToButtonPrint----1");
                FillDataToButtonPrint();
                Inventec.Common.Logging.LogSystem.Debug("FillDataToButtonPrint----2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetTxtFilm(List<HIS_SERE_SERV> hisSereServs, long diimTypeId, LabelControl lbl)
        {
            try
            {
                var materialTypes = BackendDataWorker.Get<HIS_MATERIAL_TYPE>().ToList();

                var sv = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.DIIM_TYPE_ID == diimTypeId).ToList();
                var ss1 = hisSereServs.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA && sv.Exists(p => p.ID == o.SERVICE_ID));
                if (ss1 != null && ss1.Count() > 0)
                {
                    var ss2 = hisSereServs.Where(o => ss1.ToList().Exists(p => p.ID == o.PARENT_ID) && o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT);
                    if (ss2 != null && ss2.Count() > 0)
                    {
                        var ss3 = ss2.Where(o => materialTypes.FirstOrDefault(p => p.SERVICE_ID == o.SERVICE_ID && p.IS_FILM == 1) != null);
                        lbl.Text = ss3 != null && ss3.Count() > 0 ? ss3.Sum(o => o.AMOUNT).ToString() : null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<HIS_SERE_SERV_EXT> LoadSereServExt(List<long> ssIds)
        {
            List<HIS_SERE_SERV_EXT> sereServExts = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServExtFilter hisSerwServExtFilter = new HisSereServExtFilter();
                hisSerwServExtFilter.SERE_SERV_IDs = ssIds;
                sereServExts = new List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT>();
                sereServExts = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, hisSerwServExtFilter, param).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return sereServExts;
        }

        private List<SereServADO> JoinToSereServADO(List<HIS_SERE_SERV> sereServs)
        {
            List<SereServADO> sereServADOs = new List<SereServADO>();
            try
            {
                if (sereServs != null)
                {
                    var equipmentSets = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EQUIPMENT_SET>(false, true);
                    foreach (var item in sereServs)
                    {
                        SereServADO sereServADO = new SereServADO();

                        #region BASE
                        Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(sereServADO, item);
                        #endregion

                        V_HIS_SERVICE service = Services.FirstOrDefault(o => o.ID == item.SERVICE_ID);
                        if (service != null)
                        {
                            sereServADO.SERVICE_TYPE_ID = service.SERVICE_TYPE_ID;
                            sereServADO.SERVICE_CODE = service.SERVICE_CODE;
                            sereServADO.SERVICE_NAME = service.SERVICE_NAME;
                            sereServADO.SERVICE_TYPE_CODE = service.SERVICE_TYPE_CODE;
                            sereServADO.SERVICE_TYPE_NAME = service.SERVICE_TYPE_NAME;
                            sereServADO.SERVICE_UNIT_NAME = service.SERVICE_UNIT_NAME;
                            sereServADO.IS_ALLOW_EXPEND = service.IS_ALLOW_EXPEND;
                        }

                        HIS_DEPARTMENT department = Departments.FirstOrDefault(o => o.ID == item.TDL_REQUEST_DEPARTMENT_ID);
                        if (department != null)
                        {
                            sereServADO.REQUEST_DEPARTMENT_CODE = department.DEPARTMENT_CODE;
                            sereServADO.REQUEST_DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                        }

                        HIS_PATIENT_TYPE patientType = PatientTypes != null ? PatientTypes.FirstOrDefault(o => o.ID == item.PATIENT_TYPE_ID) : null;
                        if (patientType != null)
                        {
                            sereServADO.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                        }

                        HIS_OTHER_PAY_SOURCE otherPaySource = this.OtherPaySources != null ? this.OtherPaySources.FirstOrDefault(o => o.ID == item.OTHER_PAY_SOURCE_ID) : null;
                        if (otherPaySource != null)
                        {
                            sereServADO.OTHER_PAY_SOURCE_NAME = otherPaySource.OTHER_PAY_SOURCE_NAME;
                        }

                        HIS_EQUIPMENT_SET equipmentSet = equipmentSets != null ? equipmentSets.FirstOrDefault(o => o.ID == (sereServADO.EQUIPMENT_SET_ID ?? 0)) : null;
                        if (equipmentSet != null)
                        {
                            string numOrder = sereServADO.EQUIPMENT_SET_ORDER.HasValue ? String.Format("({0})", sereServADO.EQUIPMENT_SET_ORDER.Value) : "";
                            sereServADO.EQUIPMENT_SET_NAME__NUM_ORDER = String.Format("{0} {1}", equipmentSet.EQUIPMENT_SET_NAME, numOrder);
                        }

                        sereServADO.SERVICE_REQ_CODE___SERVICE_CODE = String.Format("{0} - {1}", item.TDL_SERVICE_REQ_CODE, sereServADO.SERVICE_CODE);
                        sereServADO.INSTRUCTION_TIME___SERVICE_REQ_CODE = String.Format("{0} ({1})", Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(item.TDL_INTRUCTION_TIME), item.TDL_SERVICE_REQ_CODE);
                        sereServADO.SERVICE_REQ_CODE___SERVICE_NAME = String.Format("({0}) {1}", item.TDL_SERVICE_REQ_CODE, sereServADO.SERVICE_NAME);
                        sereServADO.SERVICE_REQ_CODE___SERVICE_CODE_NAME = String.Format("{0} - {1} - {2}", item.TDL_SERVICE_REQ_CODE, sereServADO.SERVICE_CODE, sereServADO.SERVICE_NAME);

                        if (serviceReqs != null && serviceReqs.Count > 0)
                        {
                            HIS_SERVICE_REQ serviceReq = serviceReqs.FirstOrDefault(o => o.ID == item.SERVICE_REQ_ID);
                            if (serviceReq != null)
                            {
                                sereServADO.SERVICE_REQ_STT_ID = serviceReq.SERVICE_REQ_STT_ID;
                                sereServADO.LIS_STT_ID = serviceReq.LIS_STT_ID;
                            }
                        }

                        var executeRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(s => s.ID == item.TDL_EXECUTE_ROOM_ID);
                        if (executeRoom != null)
                        {
                            sereServADO.EXECUTE_ROOM_CODE = executeRoom.ROOM_CODE;
                            sereServADO.EXECUTE_ROOM_NAME = executeRoom.ROOM_NAME;
                        }

                        if (item.PACKAGE_ID.HasValue)
                        {
                            HIS_PACKAGE package = this.packages != null ? this.packages.FirstOrDefault(o => o.ID == item.PACKAGE_ID.Value) : null;
                            if (package != null)
                            {
                                sereServADO.PACKAGE_CODE = package.PACKAGE_CODE;
                                sereServADO.PACKAGE_NAME = package.PACKAGE_NAME;
                                sereServADO.PACKAGE_IS_NOT_FIXED_SERVICE = package.IS_NOT_FIXED_SERVICE;
                            }
                        }
                        if (item.SERVICE_CONDITION_ID.HasValue && sereServADO.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT)
                        {
                            HIS_SERVICE_CONDITION serviceCondition = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE_CONDITION>().FirstOrDefault(o => o.ID == (item.SERVICE_CONDITION_ID ?? 0));
                            sereServADO.SERVICE_CONDITION_NAME = serviceCondition != null ? serviceCondition.SERVICE_CONDITION_NAME : null;
                        }
                        sereServADOs.Add(sereServADO);
                    }
                }
            }
            catch (Exception ex)
            {
                sereServADOs = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return sereServADOs;
        }

        private void CallBackLoadTreatmentFee()
        {
            try
            {
                LoadDataFromTreatmentFee();
                LoadPicUpdateQR();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadLoginName(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboLoginName.Focus();
                    cboLoginName.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>(false, true).Where(o => o.USERNAME.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboLoginName.EditValue = data[0].LOGINNAME;
                            txtLoginName.Focus();
                            txtLoginName.SelectAll();
                            FillDataToButtonPrint();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.USERNAME == searchCode);
                            if (search != null)
                            {
                                cboLoginName.EditValue = search.ID;
                                txtLoginName.Focus();
                                txtLoginName.SelectAll();
                                FillDataToButtonPrint();
                            }
                            else
                            {
                                cboLoginName.EditValue = null;
                                cboLoginName.Focus();
                                cboLoginName.ShowPopup();
                            }
                        }
                    }
                    else
                    {
                        cboLoginName.EditValue = null;
                        cboLoginName.Focus();
                        cboLoginName.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadFromSdaConfig()
        {
            try
            {
                AllowAssignOffListMedicineMaterialHeinCardNumberPrefix = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.BHYT.ALLOW_ASSIGN_OFF_LIST_MEDICINE_MATERIAL__HEIN_CARD_NUMBER_PREFIX");
                AllowCheckIsNoExecute = (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKeys.ALLOW_CHECK_IS_NO_EXECUTE) == "1");
                AutoClosePrintAndForm = (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKeys.AUTO_CLOSE_PRINT_AND_FORM) == "1");
                string strIsSetPrimaryPatientType = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKeys.IS_SET_PRIMARY_PATIENT_TYPE);
                this.IsAllowNoExecuteForPaid = (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKeys.IS_ALLOW_NO_EXECUTE_FOR_PAID) == "1");
                IsSetPrimaryPatientType = (!String.IsNullOrEmpty(strIsSetPrimaryPatientType) && strIsSetPrimaryPatientType != "0");

                DepartmentPremissionEdits = new List<HIS_DEPARTMENT>();
                string departmentPremissionCodeCFG = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKeys.PERMISSION_EDIT_BY_DEPARTMENT);
                List<HIS_DEPARTMENT> departments = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>(false, true);
                if (!String.IsNullOrEmpty(departmentPremissionCodeCFG))
                {
                    String[] departmentCodes = departmentPremissionCodeCFG.Split(',');
                    DepartmentPremissionEdits = departmentCodes != null ? departments.Where(o => departmentCodes.Contains(o.DEPARTMENT_CODE)).ToList() : null;
                }
                this.cboPayTypeDefault = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKeys.cboPayType_DEFAULT);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Load combo người trả kết quả và giá trị mặc định là người dùng đăng nhập
        /// </summary>
        private void LoadPeopleReturnResult()
        {
            try
            {
                LoadComboPeopleReturnResult(cboLoginName);
                //set defauilt PeopleReturnResult
                string LoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                string userName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                cboLoginName.EditValue = LoginName;
                txtLoginName.Text = LoginName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadPeopleReturnResultV2()
        {
            try
            {
                LoadComboPeopleReturnResult(cboLoginName);
                //set defauilt PeopleReturnResult
                string LoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                string userName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                cboLoginName.EditValue = LoginName;
                txtLoginName.Text = LoginName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Load thông tin tạm ứng, bệnh nhân phải thu,...
        /// </summary>
        private void LoadDataFromTreatmentFee()
        {
            try
            {
                if (InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        decimal totalPrice = 0;
                        decimal totalHeinPrice = 0;
                        decimal totalPatientPrice = 0;
                        decimal totalDeposit = 0;
                        decimal totalBill = 0;
                        decimal totalBillTransferAmount = 0;
                        decimal totalRepay = 0;
                        decimal exemption = 0;
                        decimal depositPlus = 0;
                        decimal total_obtained_price = 0;

                        if (treatmentFees != null && treatmentFees.Count > 0)
                        {
                            totalPrice = treatmentFees[0].TOTAL_PRICE ?? 0; // tong tien
                            totalHeinPrice = treatmentFees[0].TOTAL_HEIN_PRICE ?? 0;
                            totalPatientPrice = treatmentFees[0].TOTAL_PATIENT_PRICE ?? 0; // bn tra
                            totalDeposit = treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0;
                            totalBill = treatmentFees[0].TOTAL_BILL_AMOUNT ?? 0;
                            totalBillTransferAmount = treatmentFees[0].TOTAL_BILL_TRANSFER_AMOUNT ?? 0;
                            exemption = 0;// HospitalFeeSum[0].TOTAL_EXEMPTION ?? 0;
                            totalRepay = treatmentFees[0].TOTAL_REPAY_AMOUNT ?? 0;
                            total_obtained_price = (totalDeposit + totalBill - totalBillTransferAmount - totalRepay + exemption);//Da thu benh nhan
                            decimal transfer = totalPatientPrice - total_obtained_price;//Phai thu benh nhan
                            depositPlus = transfer;//Nop them

                            lblTotalPrice.Text = Inventec.Common.Number.Convert.NumberToString(totalPrice, ConfigApplications.NumberSeperator);
                            lblTotalPatientPrice.Text = Inventec.Common.Number.Convert.NumberToString(totalPatientPrice, ConfigApplications.NumberSeperator);
                            lblTotalObtainedPrice.Text = Inventec.Common.Number.Convert.NumberToString(total_obtained_price, ConfigApplications.NumberSeperator);
                            lblTotalDepositPrice.Text = Inventec.Common.Number.Convert.NumberToString(transfer, ConfigApplications.NumberSeperator);
                            if (transfer < 0)
                            {
                                lblTotalDepositPrice.ForeColor = Color.Blue;
                            }
                            else
                            {
                                lblTotalDepositPrice.ForeColor = Color.Red;
                            }
                        }

                    }));
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void LoadDataFromTreatmentFeeV2()
        {
            try
            {
                if (InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        decimal totalPrice = 0;
                        decimal totalHeinPrice = 0;
                        decimal totalPatientPrice = 0;
                        decimal totalDeposit = 0;
                        decimal totalBill = 0;
                        decimal totalBillTransferAmount = 0;
                        decimal totalRepay = 0;
                        decimal exemption = 0;
                        decimal depositPlus = 0;
                        decimal total_obtained_price = 0;

                        if (treatmentFees != null && treatmentFees.Count > 0)
                        {
                            totalPrice = treatmentFees[0].TOTAL_PRICE ?? 0; // tong tien
                            totalHeinPrice = treatmentFees[0].TOTAL_HEIN_PRICE ?? 0;
                            totalPatientPrice = treatmentFees[0].TOTAL_PATIENT_PRICE ?? 0; // bn tra
                            totalDeposit = treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0;
                            totalBill = treatmentFees[0].TOTAL_BILL_AMOUNT ?? 0;
                            totalBillTransferAmount = treatmentFees[0].TOTAL_BILL_TRANSFER_AMOUNT ?? 0;
                            exemption = 0;// HospitalFeeSum[0].TOTAL_EXEMPTION ?? 0;
                            totalRepay = treatmentFees[0].TOTAL_REPAY_AMOUNT ?? 0;
                            total_obtained_price = (totalDeposit + totalBill - totalBillTransferAmount - totalRepay + exemption);//Da thu benh nhan
                            decimal transfer = totalPatientPrice - total_obtained_price;//Phai thu benh nhan
                            depositPlus = transfer;//Nop them

                            lblTotalPrice.Text = Inventec.Common.Number.Convert.NumberToString(totalPrice, ConfigApplications.NumberSeperator);
                            lblTotalPatientPrice.Text = Inventec.Common.Number.Convert.NumberToString(totalPatientPrice, ConfigApplications.NumberSeperator);
                            lblTotalObtainedPrice.Text = Inventec.Common.Number.Convert.NumberToString(total_obtained_price, ConfigApplications.NumberSeperator);
                            lblTotalDepositPrice.Text = Inventec.Common.Number.Convert.NumberToString(transfer, ConfigApplications.NumberSeperator);
                            if (transfer < 0)
                            {
                                lblTotalDepositPrice.ForeColor = Color.Blue;
                            }
                            else
                            {
                                lblTotalDepositPrice.ForeColor = Color.Red;
                            }
                        }
                    }));
                }
                else
                {
                    decimal totalPrice = 0;
                    decimal totalHeinPrice = 0;
                    decimal totalPatientPrice = 0;
                    decimal totalDeposit = 0;
                    decimal totalBill = 0;
                    decimal totalBillTransferAmount = 0;
                    decimal totalRepay = 0;
                    decimal exemption = 0;
                    decimal depositPlus = 0;
                    decimal total_obtained_price = 0;

                    if (treatmentFees != null && treatmentFees.Count > 0)
                    {
                        totalPrice = treatmentFees[0].TOTAL_PRICE ?? 0; // tong tien
                        totalHeinPrice = treatmentFees[0].TOTAL_HEIN_PRICE ?? 0;
                        totalPatientPrice = treatmentFees[0].TOTAL_PATIENT_PRICE ?? 0; // bn tra
                        totalDeposit = treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0;
                        totalBill = treatmentFees[0].TOTAL_BILL_AMOUNT ?? 0;
                        totalBillTransferAmount = treatmentFees[0].TOTAL_BILL_TRANSFER_AMOUNT ?? 0;
                        exemption = 0;// HospitalFeeSum[0].TOTAL_EXEMPTION ?? 0;
                        totalRepay = treatmentFees[0].TOTAL_REPAY_AMOUNT ?? 0;
                        total_obtained_price = (totalDeposit + totalBill - totalBillTransferAmount - totalRepay + exemption);//Da thu benh nhan
                        decimal transfer = totalPatientPrice - total_obtained_price;//Phai thu benh nhan
                        depositPlus = transfer;//Nop them

                        lblTotalPrice.Text = Inventec.Common.Number.Convert.NumberToString(totalPrice, ConfigApplications.NumberSeperator);
                        lblTotalPatientPrice.Text = Inventec.Common.Number.Convert.NumberToString(totalPatientPrice, ConfigApplications.NumberSeperator);
                        lblTotalObtainedPrice.Text = Inventec.Common.Number.Convert.NumberToString(total_obtained_price, ConfigApplications.NumberSeperator);
                        lblTotalDepositPrice.Text = Inventec.Common.Number.Convert.NumberToString(transfer, ConfigApplications.NumberSeperator);
                        if (transfer < 0)
                        {
                            lblTotalDepositPrice.ForeColor = Color.Blue;
                        }
                        else
                        {
                            lblTotalDepositPrice.ForeColor = Color.Red;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        public void VisableColumnInGrid()
        {
            try
            {
                if (IsSetPrimaryPatientType)
                {
                    gridColumnDoiTuongPT.Visible = true;
                }
                else
                {
                    gridColumnDoiTuongPT.Visible = false;
                }

                repositoryItem_expend_type_id_disable.Enabled = false;
                repositoryItem_expend_type_id_disable.ReadOnly = true;
                repositoryItemCheckEditIsNoExecute_Disable.Enabled = false;
                repositoryItemCheckEditIsNoExecute_Disable.ReadOnly = true;
                repositoryItemChkOutKtcFee_Disable.Enabled = false;
                repositoryItemChkOutKtcFee_Disable.ReadOnly = true;
                repositoryItemChkIsExpend_Disable.Enabled = false;
                repositoryItemChkIsExpend_Disable.ReadOnly = true;
                repositoryItemCheckEditIsNotUseBHYT_Disable.Enabled = false;
                repositoryItemCheckEditIsNotUseBHYT_Disable.ReadOnly = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Sửa chức năng bảng kê:
        //- Căn cứ vào treatment, nếu có thông tin quỹ thanh toán (fund_id trong his_treatment) thì bổ sung hiển thị cột "Quỹ thanh toán" 
        //- Giá trị cột này hiển thị theo trường "fund_id" trong his_sere_serv, cho phép bỏ chọn.

        //Quote #5
        //Updated by tiencb 4 days ago
        //Sửa lại:
        //- "fund_id" --> "is_fund_accepted". Cột "Quỹ chi trả" --> "Quỹ chấp nhận chi trả" và hiển thị dưới dạng "Checkbox" 
        //- Ở vùng thông tin hành chính bên trên, hiển thị tên "Quỹ chi trả" (căn cứ vào trường fund_id trong his_treatment)
        //- Lưu ý: Bổ sung check/uncheck đối với nhiều dịch vụ (sử dụng menu chuột phải)
        /// </summary>
        void InitTreatmentFun()
        {
            try
            {
                if (currentTreatment != null && currentTreatment.FUND_ID > 0)
                {
                    gridColIsFundAccepted.Visible = true;
                    //gridColIsFundAccepted.VisibleIndex = 24;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPatientInfo()
        {
            try
            {
                if (currentTreatment != null)
                {
                    lblPatientName.Text = currentTreatment.TDL_PATIENT_NAME;
                    lblAddress.Text = currentTreatment.TDL_PATIENT_ADDRESS;
                    lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentTreatment.TDL_PATIENT_DOB);
                    lblTreatmentCode.Text = currentTreatment.TREATMENT_CODE;
                    if (currentTreatment.FUND_ID > 0)
                    {
                        HisFundFilter hisFundFilter = new HisFundFilter();
                        hisFundFilter.ID = currentTreatment.FUND_ID;
                        var funds = new BackendAdapter(new CommonParam())
                          .Get<List<HIS_FUND>>("api/HisFund/Get", ApiConsumers.MosConsumer, hisFundFilter, new CommonParam()).FirstOrDefault();
                        lblFundName.Text = funds != null ? funds.FUND_NAME : "";
                    }
                }

                if (currentHisPatientTypeAlters != null && currentHisPatientTypeAlters.Count > 0)
                {
                    V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = currentHisPatientTypeAlters.OrderByDescending(o => o.LOG_TIME).FirstOrDefault();
                    lblPatientType.Text = patientTypeAlter.PATIENT_TYPE_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitColumnVisable(List<HIS_SERE_SERV> sereServs)
        {
            try
            {
                VisiableColumnStent(sereServs);
                VisiableColumnShareCount(sereServs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void VisiableColumnStent(List<HIS_SERE_SERV> sereServs)
        {
            try
            {
                bool existStent = false;
                if (sereServs != null && sereServs.Count > 0)
                {
                    foreach (var item in sereServs)
                    {
                        if (this.CheckIsStent(item.SERVICE_ID))
                        {
                            existStent = true;
                            break;
                        }
                    }
                }

                if (!existStent)
                {
                    gridColumnStentOrder.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void VisiableColumnShareCount(List<HIS_SERE_SERV> sereServs)
        {
            try
            {
                bool existG = false;
                if (sereServs != null && sereServs.Count > 0)
                {
                    foreach (var item in sereServs)
                    {
                        if (item.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G)
                        {
                            existG = true;
                            break;
                        }
                    }
                }

                if (!existG)
                {
                    gridColumnShareCount.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckIsStent(long serviceId)
        {
            bool result = false;
            try
            {
                List<V_HIS_MATERIAL_TYPE> source = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(false, true);
                V_HIS_MATERIAL_TYPE hisMaterial = source.FirstOrDefault((V_HIS_MATERIAL_TYPE o) => o.SERVICE_ID == serviceId && o.IS_STENT == 1);
                if (hisMaterial != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Co get hay khong dua vao key cau hinh nay : AllowCheckIsNoExecute
        /// </summary>
        private void LoadServiceReq()
        {
            try
            {
                if (AllowCheckIsNoExecute)
                {
                    CommonParam param = new CommonParam();
                    HisServiceReqFilter filter = new HisServiceReqFilter();
                    filter.TREATMENT_ID = this.treatmentId;
                    serviceReqs = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadServiceReqV2()
        {
            try
            {
                //if (AllowCheckIsNoExecute)
                {
                    CommonParam param = new CommonParam();
                    HisServiceReqFilter filter = new HisServiceReqFilter();
                    filter.TREATMENT_ID = this.treatmentId;
                    serviceReqs = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadcboHSBA()
        {
            try
            {
                List<TreatmentADO> lstTreatmentADO = new List<TreatmentADO>();
                if (this.currentTreatment.MEDI_RECORD_ID != null && this.currentTreatment.PROGRAM_ID != null)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                    filter.MEDI_RECORD_ID = this.currentTreatment.MEDI_RECORD_ID;
                    var data = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, filter, param);
                    if (data != null && data.Count > 0)
                    {
                        data = data.OrderByDescending(o => o.IN_TIME).ToList();
                        foreach (var item in data)
                        {
                            TreatmentADO ADO = new TreatmentADO(item);
                            lstTreatmentADO.Add(ADO);
                        }
                    }
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TREATMENT_CODE", "Mã điều trị", 100, 1));
                columnInfos.Add(new ColumnInfo("timeTreatment", "Thời gian vào", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("contentShow", "ID", columnInfos, false, 300);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboHSBA, lstTreatmentADO, controlEditorADO);
                cboHSBA.Properties.ImmediatePopup = true;
                if (lstTreatmentADO != null && lstTreatmentADO.Count > 0)
                {
                    cboHSBA.EditValue = lstTreatmentADO.Where(o => o.ID == this.currentTreatment.ID).FirstOrDefault().ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
