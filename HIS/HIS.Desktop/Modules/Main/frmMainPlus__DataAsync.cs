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
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraTab;
using HIS.Desktop.Base;
using Inventec.Common.Logging;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Common.Modules;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraBars;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Repository;
using System.Threading.Tasks;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.LocalData;
using System.Linq;
using System.Threading;
using MOS.EFMODEL.DataModels;
using SDA.EFMODEL.DataModels;
using HIS.Desktop.ModuleExt;

namespace HIS.Desktop.Modules.Main
{
    public partial class frmMain : RibbonForm
    {
        internal BackendDataWorkerAsync BackendDataWorkerAsync { get { return (BackendDataWorkerAsync)Worker.Get<BackendDataWorkerAsync>(); } }

        private async Task LoadDataAsync()
        {
            try
            {
                string isRunInBackgroundLoadDataToRamAfterLogin = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigKeys.KEY__HIS_DESKTOP_IS_RUN_IN_BACKGROUND_LOAD_DATA_TO_RAM_AFTER_LOGIN);

                if (isRunInBackgroundLoadDataToRamAfterLogin == GlobalVariables.CommonStringTrue)
                {
                    //TheadProcessInitRegisterBackgroundData();
                    LoadDataPatientType();
                    LoadDataPatientTypeAllow();
                    LoadDataServiceType();
                    LoadDataTreatmentType();
                    LoadDataRoom();
                    LoadDataUser();
                    LoadDataHisAgeType();

                    var moduleLinks = GlobalVariables.currentModuleRaws.Select(o => o.ModuleLink).ToArray();

                    ////Nếu người dùng có quyền vào chức năng Tiếp đón thì load sẵn các dữ liệu sẽ dùng trong chức năng này về ram
                    if (moduleLinks.Contains("HIS.Desktop.Plugins.RegisterV2") || moduleLinks.Contains("HIS.Desktop.Plugins.Register") || moduleLinks.Contains("HIS.Desktop.Plugins.RegisterV3"))
                    {
                        LoadDataGender();
                        LoadDataCareer();
                        LoadDataNational();
                        LoadDataEthnic();
                        LoadDataHisPosition();
                        LoadDataMilitaryRank();
                        LoadDataBhytWhiteList();
                        LoadDataPatientClassify();
                        LoadDataHisReceptionRoom();
                        var task1 = LoadDataProvince();
                        var task2 = LoadDataDistrict();
                        var task3 = LoadDataCommune();
                        await Task.WhenAll(task1, task2, task3);
                        var CommuneADOs = BackendDataWorkerSet.Set<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>(false, true);
                    }

                    ////Nếu người dùng có quyền vào chức năng Kê đơn thì load sẵn các dữ liệu sẽ dùng trong chức năng này về ram
                    if (moduleLinks.Contains("HIS.Desktop.Plugins.AssignPrescriptionPK") || moduleLinks.Contains("HIS.Desktop.Plugins.AssignPrescriptionYHCT"))
                    {
                        LoadDataHisMedicineType();
                        LoadDataHisMaterialType();
                        LoadDataMediStock();
                        LoadDataMestRoom();
                        LoadDataMestPatientType();
                        LoadDataMediStockMety();
                        LoadDataMestMetyDepa();

                        LoadDataMedicineUseForm();
                        LoadDataHisMedicineTypeRoom();
                        LoadDataHisEmployee();
                        LoadDataHisMestMetyUnit();
                        LoadDataTreatmentEndType();
                        LoadDataTreatmentEndTypeExt();
                        LoadDataHisExpMestReason();
                        LoadDataHisServiceCondition();
                    }

                    ////Nếu người dùng có quyền vào chức năng Chỉ định thì load sẵn các dữ liệu sẽ dùng trong chức năng này về ram
                    if (moduleLinks.Contains("HIS.Desktop.Plugins.AssignService"))
                    {                        
                        LoadDataServiceGroup();
                        LoadDataRoomTimes();
                    }

                    if (moduleLinks.Contains("HIS.Desktop.Plugins.AssignService") || moduleLinks.Contains("HIS.Desktop.Plugins.ExecuteRoom"))
                    {
                        LoadDataExecuteRoom();
                    }

                    if (moduleLinks.Contains("HIS.Desktop.Plugins.ServiceExecute")
                      || moduleLinks.Contains("HIS.Desktop.Plugins.ExamServiceReqExecute")
                      || moduleLinks.Contains("HIS.Desktop.Plugins.ExecuteRoom"))
                    {
                        LoadDataTextLib();
                        LoadDataTestIndex();
                        LoadDataKskContract();
                        LoadDataExeServiceModule();
                    }


                    if (moduleLinks.Contains("HIS.Desktop.Plugins.Transaction"))
                    {
                        await LoadDataTreatmentResult();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataKskContract()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                dynamic dfilter = new System.Dynamic.ExpandoObject();
                dfilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var dataKskContract = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<V_HIS_KSK_CONTRACT>>("api/HisKskContract/GetView", ApiConsumers.MosConsumer, dfilter, paramCommon);
                if (dataKskContract != null) BackendDataWorker.UpdateToRam(typeof(V_HIS_KSK_CONTRACT), dataKskContract, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataTestIndex()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                dynamic dfilter = new System.Dynamic.ExpandoObject();
                dfilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var dataTestIndex = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_TEST_INDEX>>("api/HisTestIndex/Get", ApiConsumers.MosConsumer, dfilter, paramCommon);
                if (dataTestIndex != null) BackendDataWorker.UpdateToRam(typeof(HIS_TEST_INDEX), dataTestIndex, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataTextLib()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                dynamic dfilter = new System.Dynamic.ExpandoObject();
                dfilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var dataTextLib = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_TEXT_LIB>>("api/HisTextLib/Get", ApiConsumers.MosConsumer, dfilter, paramCommon);
                if (dataTextLib != null) BackendDataWorker.UpdateToRam(typeof(HIS_TEXT_LIB), dataTextLib, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataExecuteRole()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                dynamic dfilter = new System.Dynamic.ExpandoObject();
                dfilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var dataExecuteRole = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_EXECUTE_ROLE>>("api/HisExecuteRole/Get", ApiConsumers.MosConsumer, dfilter, paramCommon);
                if (dataExecuteRole != null) BackendDataWorker.UpdateToRam(typeof(HIS_EXECUTE_ROLE), dataExecuteRole, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataCommune()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                dynamic dfilter = new System.Dynamic.ExpandoObject();
                dfilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var dataCommune = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<V_SDA_COMMUNE>>("api/SdaCommune/GetView", ApiConsumers.SdaConsumer, dfilter, paramCommon);
                if (dataCommune != null) BackendDataWorker.UpdateToRam(typeof(V_SDA_COMMUNE), dataCommune, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataDistrict()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                dynamic dfilter = new System.Dynamic.ExpandoObject();
                dfilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var dataDistrict = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<V_SDA_DISTRICT>>("api/SdaDistrict/GetView", ApiConsumers.SdaConsumer, dfilter, paramCommon);
                if (dataDistrict != null) BackendDataWorker.UpdateToRam(typeof(V_SDA_DISTRICT), dataDistrict, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private static async Task LoadDataProvince()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                dynamic dfilter = new System.Dynamic.ExpandoObject();
                dfilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var dataProvince = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<V_SDA_PROVINCE>>("api/SdaProvince/GetView", ApiConsumers.SdaConsumer, dfilter, paramCommon);
                if (dataProvince != null) BackendDataWorker.UpdateToRam(typeof(V_SDA_PROVINCE), dataProvince, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataMediStockMety()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataMediStockMety => 1");
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK_METY>>("api/HisMediStockMety/GetView", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK_METY), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataMediStockMety => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataMestMetyDepa()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataMestMetyDepa => 1");
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_MEST_METY_DEPA>>("api/HisMestMetyDepa/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_MEST_METY_DEPA), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataMestMetyDepa => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataHisMedicineTypeRoom()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataHisMedicineTypeRoom => 1");
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE_ROOM>>("api/HisMedicineTypeRoom/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE_ROOM), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataHisMedicineTypeRoom => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataExeServiceModule()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataExeServiceModule => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_EXE_SERVICE_MODULE>>("api/HisExeServiceModule/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_EXE_SERVICE_MODULE), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataExeServiceModule => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataTreatmentEndType()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataTreatmentEndType => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_TREATMENT_END_TYPE>>("api/HisTreatmentEndType/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_TREATMENT_END_TYPE), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataTreatmentEndType => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataTreatmentEndTypeExt()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataTreatmentEndTypeExt => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_TREATMENT_END_TYPE_EXT>>("api/HisTreatmentEndTypeExt/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_TREATMENT_END_TYPE_EXT), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataTreatmentEndTypeExt => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataHtu()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataHtu => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_HTU>>("api/HisHtu/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_HTU), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataHtu => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataTranPatiReason()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataTranPatiReason => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON>>("api/HisTranPatiReason/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataTranPatiReason => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataTranPatiForm()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataTranPatiForm => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM>>("api/HisTranPatiForm/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataTranPatiForm => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataBhytWhiteList()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataBhytWhiteList => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_BHYT_WHITELIST>>("api/HisBhytWhiteList/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_BHYT_WHITELIST), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataBhytWhiteList => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataWorkPlace()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataWorkPlace => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_WORK_PLACE>>("api/HisWorkPlace/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_WORK_PLACE), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataWorkPlace => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataEmergencyWtime()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataEmergencyWtime => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_EMERGENCY_WTIME>>("api/HisEmergencyWtime/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_EMERGENCY_WTIME), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataEmergencyWtime => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataOweType()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataOweType => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_OWE_TYPE>>("api/HisOweType/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_OWE_TYPE), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataOweType => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataMilitaryRank()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataMilitaryRank => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>>("api/HisMilitaryRank/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_MILITARY_RANK), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataMilitaryRank => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataHisPosition()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataHisPosition => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_POSITION>>("api/HisPosition/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_POSITION), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataHisPosition => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataHisAgeType()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataHisAgeType => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_AGE_TYPE>>("api/HisAgeType/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_AGE_TYPE), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataHisAgeType => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataHisExpMestReason()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataHisExpMestReason => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_REASON>>("api/HisExpMestReason/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_EXP_MEST_REASON), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataHisExpMestReason => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataHisServiceCondition()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataHisServiceCondition => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_SERVICE_CONDITION>>("api/HisServiceCondition/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_CONDITION), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataHisServiceCondition => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataHisReceptionRoom()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataHisReceptionRoom => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_RECEPTION_ROOM>>("api/HisReceptionRoom/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_RECEPTION_ROOM), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataHisReceptionRoom => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataPatientClassify()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataPatientClassify => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY>>("api/HisPatientClassify/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataPatientClassify => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataGender()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataGender => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_GENDER>>("api/HisGender/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_GENDER), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataGender => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataNational()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataEthnic => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<SDA.EFMODEL.DataModels.SDA_NATIONAL>>("api/SdaNational/Get", ApiConsumers.SdaConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(SDA.EFMODEL.DataModels.SDA_NATIONAL), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataEthnic => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataEthnic()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataEthnic => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<SDA.EFMODEL.DataModels.SDA_ETHNIC>>("api/SdaEthnic/Get", ApiConsumers.SdaConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(SDA.EFMODEL.DataModels.SDA_ETHNIC), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataEthnic => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataMestPatientType()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataMestPatientType => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_MEST_PATIENT_TYPE>>("api/HisMestPatientType/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_MEST_PATIENT_TYPE), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataMestPatientType => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataHisMedicineType()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataHisMedicineType => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>>("api/HisMedicineType/GetView", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataHisMedicineType => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataHisMaterialType()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataHisMaterialType => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE>>("api/HisMaterialType/GetView", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataHisMaterialType => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataHisMedicinePaty()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataHisMedicinePaty => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_MEDICINE_PATY>>("api/HisMedicinePaty/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_MEDICINE_PATY), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataHisMedicinePaty => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataCareer()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataCareer => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_CAREER>>("api/HisCareer/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_CAREER), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataCareer => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataMedicineUseForm()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataMedicineUseForm => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM>>("api/HisMedicineUseForm/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataMedicineUseForm => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataHisEmployee()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataHisEmployee => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisEmployeeFilter filter = new MOS.Filter.HisEmployeeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>>("api/HisEmployee/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_EMPLOYEE), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataHisEmployee => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataHisMestMetyUnit()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataHisMestMetyUnit => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisMestMetyUnitFilter filter = new MOS.Filter.HisMestMetyUnitFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_MEST_METY_UNIT>>("api/HisMestMetyUnit/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_MEST_METY_UNIT), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataHisMestMetyUnit => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataMestRoom()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataMestRoom => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM>>("api/HisMestRoom/GetView", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataMestRoom=> 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataEquipmentSet()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataEquipmentSet => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_EQUIPMENT_SET>>("api/HisEquipmentSet/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_EQUIPMENT_SET), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataEquipmentSet => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataExpMestTemplate()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataExpMestTemplate => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE>>("api/HisExpMestTemplate/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataExpMestTemplate => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataMediStock()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataMediStock => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>>("api/HisMediStock/GetView", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataMediStock => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataTreatmentResult()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataTreatmentResult => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisTreatmentResultFilter filter = new MOS.Filter.HisTreatmentResultFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_TREATMENT_RESULT>>("api/HisTreatmentResult/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_TREATMENT_RESULT), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataTreatmentResult => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataTreatmentType()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadHisTreatmentType => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>>("api/HisTreatmentType/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadHisTreatmentType => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataCashierRoom()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadHisCashierRoom => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_CASHIER_ROOM>>("api/HisCashierRoom/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_CASHIER_ROOM), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                var result1 = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM>>("api/HisCashierRoom/GetView", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result1 != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM), result1, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadHisCashierRoom => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataPatientType()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataPatientType => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>>("api/HisPatientType/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataPatientType => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataPatientTypeAllow()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataPatientTypeAllow => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW>>("api/HisPatientTypeAllow/GetView", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataPatientTypeAllow => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataExecuteRoom()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataExecuteRoom => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>>("api/HisExecuteRoom/GetView", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                CommonParam paramCommon1 = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter1 = new MOS.Filter.HisPatientTypeFilter();
                var result1 = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROOM>>("api/HisExecuteRoom/Get", ApiConsumers.MosConsumer, filter1, paramCommon1);

                if (result1 != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_EXECUTE_ROOM), result1, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataExecuteRoom => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataRoom()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataRoom => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.V_HIS_ROOM>>("api/HisRoom/GetView", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.V_HIS_ROOM), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataRoom => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataServiceType()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataServiceType => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE>>("api/HisServiceType/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataServiceType => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataExecuteGroup()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataServiceType => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_EXECUTE_GROUP>>("api/HisExecuteGroup/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_EXECUTE_GROUP), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataServiceType => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataUser()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataUser => 1");
                //long startBytes = GC.GetTotalMemory(true);
                CommonParam paramCommon = new CommonParam();
                ACS.Filter.AcsUserFilter filter = new ACS.Filter.AcsUserFilter();
                filter.ColumnParams = new List<string>() { "ID", "LOGINNAME", "USERNAME", "IS_ACTIVE", "EMAIL", "MOBILE" };
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<ACS.EFMODEL.DataModels.ACS_USER>>("api/AcsUser/GetDynamic", ApiConsumers.AcsConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(ACS.EFMODEL.DataModels.ACS_USER), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                //long memoryUsageusers = (GC.GetTotalMemory(true) - startBytes) / (1024 * 1024);
                //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => startBytes), startBytes) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => memoryUsageusers), memoryUsageusers));
                Inventec.Common.Logging.LogSystem.Info("GC.Collect. 1");
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                Inventec.Common.Logging.LogSystem.Info("GC.Collect. 2");
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataUser => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataServiceGroup()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataServiceType => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP>>("api/HisServiceGroup/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataServiceType => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataRoomTimes()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataRoomTimes => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisRoomTimeFilter filter = new MOS.Filter.HisRoomTimeFilter();
                filter.IS_ACTIVE = 1;
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_ROOM_TIME>>("api/HisRoomTime/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_ROOM_TIME), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataRoomTimes => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
