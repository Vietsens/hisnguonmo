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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.MediStockSummary.ADO;
using HIS.Desktop.Plugins.MediStockSummary.Base;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.Processor.Mps000217.PDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MediStockSummary.CreateReport
{
    public class Mrs00067
    {
        decimal startBeginAmount;
        CommonParam paramGet = new CommonParam();
        List<Mrs00067RDO> ListRdo = new List<Mrs00067RDO>();
        List<Mrs00067RDO> ListByPackages = new List<Mrs00067RDO>();
        Dictionary<long, Mrs00067RDO> dicMedicine = new Dictionary<long, Mrs00067RDO>();
        List<V_HIS_SERE_SERV> ListSereServ = new List<V_HIS_SERE_SERV>();
        List<V_HIS_EXP_MEST_MEDICINE> listExpMestMedicineView = new List<V_HIS_EXP_MEST_MEDICINE>();
        List<V_HIS_SERE_SERV> listSereServ = new List<V_HIS_SERE_SERV>();
        HIS_DEPARTMENT department = new HIS_DEPARTMENT();
        HIS_MEDI_STOCK mediStock = new HIS_MEDI_STOCK();
        decimal BeginAmount = 0;
        decimal EndAmount = 0;
        private string a = "";
        long RoomId;

        public Mrs00067(long roomId)
        {
            this.RoomId = roomId;
        }

        internal FilterADO _ReportFilter { get; set; }

        public void LoadDataPrint217(FilterADO _FilterADO, string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                this._ReportFilter = _FilterADO;
                GetData();

                ProcessData();

                MPS.Processor.Mps000217.PDO.SingKey217 _SingKey217 = new MPS.Processor.Mps000217.PDO.SingKey217();

                if (this._ReportFilter.TIME_FROM > 0)
                {
                    _SingKey217.TIME_TO_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this._ReportFilter.TIME_FROM);
                }
                if (this._ReportFilter.TIME_TO > 0)
                {
                    _SingKey217.TIME_FROM_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this._ReportFilter.TIME_TO);
                }
                _SingKey217.BEGIN_AMOUNT = BeginAmount.ToString();
                _SingKey217.END_AMOUNT = EndAmount.ToString();
                V_HIS_MEDICINE_TYPE medicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == this._ReportFilter.MEDICINE_TYPE_ID);
                if (medicineType != null)
                {
                    _SingKey217.MEDICINE_TYPE_CODE = medicineType.MEDICINE_TYPE_CODE;
                    _SingKey217.MEDICINE_TYPE_NAME = medicineType.MEDICINE_TYPE_NAME;
                    _SingKey217.SERVICE_UNIT_NAME = medicineType.SERVICE_UNIT_NAME;
                    _SingKey217.NATIONAL_NAME = medicineType.NATIONAL_NAME;
                    _SingKey217.MANUFACTURER_NAME = medicineType.MANUFACTURER_NAME;
                    _SingKey217.ACTIVE_INGR_BHYT_NAME = medicineType.ACTIVE_INGR_BHYT_NAME;
                    _SingKey217.DIC_OTHER_KEY = SetOtherKey(_SingKey217.DIC_OTHER_KEY, medicineType, medicineType.GetType().Name);

                }

                HIS_MEDI_STOCK mediStock = BackendDataWorker.Get<HIS_MEDI_STOCK>().FirstOrDefault(p => p.ID == this._ReportFilter.MEDI_STOCK_ID);
                if (mediStock != null)
                {
                    _SingKey217.MEDI_STOCK_CODE = mediStock.MEDI_STOCK_CODE;
                    _SingKey217.MEDI_STOCK_NAME = mediStock.MEDI_STOCK_NAME;
                }

                if (ListRdo != null && ListRdo.Count > 0)
                {
                    _SingKey217.MEDI_BEGIN_AMOUNT = ListRdo.First().BEGIN_AMOUNT.ToString();
                    _SingKey217.MEDI_END_AMOUNT = ListRdo.Last().END_AMOUNT.ToString();
                }

                var listMedicine = new List<Mrs00067RDO>();
                //if (ListRdo.Count == 0)
                {
                    listMedicine = dicMedicine.Values.ToList();
                }

                MPS.Processor.Mps000217.PDO.Mps000217PDO mps000217RDO = new MPS.Processor.Mps000217.PDO.Mps000217PDO(
                    ListRdo,
                    listMedicine, 
                    ListByPackages, 
                    _SingKey217
               );
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000217RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000217RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                }
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode("", printTypeCode, RoomId);
                PrintData.EmrInputADO = inputADO;

                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private Dictionary<string, object> SetOtherKey(Dictionary<string, object> dicOtherKey, object data, string prefix)
        {
            try
            {
                if (dicOtherKey == null) dicOtherKey = new Dictionary<string, object>();
                foreach (var prop in data.GetType().GetProperties())
                {
                    if (!dicOtherKey.ContainsKey(string.Format("{0}__{1}", prefix, prop.Name)))
                        dicOtherKey.Add(string.Format("{0}__{1}", prefix, prop.Name), prop.GetValue(data, null));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return dicOtherKey;
        }

        private bool GetData()
        {
            var result = true;
            try
            {
                List<HIS_EXP_MEST_TYPE> expMestTypes = BackendDataWorker.Get<HIS_EXP_MEST_TYPE>();
                List<HIS_IMP_MEST_TYPE> impMestTypes = BackendDataWorker.Get<HIS_IMP_MEST_TYPE>();

                HisImpMestMedicineViewFilter impMediFilter = new HisImpMestMedicineViewFilter();
                impMediFilter.IMP_TIME_FROM = this._ReportFilter.TIME_FROM;
                impMediFilter.IMP_TIME_TO = this._ReportFilter.TIME_TO;
                impMediFilter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
                impMediFilter.MEDICINE_TYPE_ID = this._ReportFilter.MEDICINE_TYPE_ID;
                impMediFilter.MEDICINE_ID = this._ReportFilter.MediMateId;
                impMediFilter.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT;
                List<V_HIS_IMP_MEST_MEDICINE> hisImpMestMedicines = new BackendAdapter(null).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, impMediFilter, SessionManager.ActionLostToken, null);
                List<long> impMestIds = hisImpMestMedicines.Select(o => o.IMP_MEST_ID).ToList();
                var aggrImpId = hisImpMestMedicines.Select(o => o.AGGR_IMP_MEST_ID ?? 0).ToList();
                if (aggrImpId != null && aggrImpId.Count > 0)
                    impMestIds.AddRange(aggrImpId);

                //
                List<V_HIS_IMP_MEST> ImpMest = new List<V_HIS_IMP_MEST>();
                if (impMestIds != null && impMestIds.Count > 0)
                {
                    impMestIds = impMestIds.Distinct().ToList();
                    var skip = 0;
                    while (impMestIds.Count - skip > 0)
                    {
                        var listIDs = impMestIds.Skip(skip).Take(ManagerConstant.MAX_REQUEST_LENGTH_PARAM).ToList();
                        skip = skip + ManagerConstant.MAX_REQUEST_LENGTH_PARAM;
                        HisImpMestViewFilter ImpFilter = new HisImpMestViewFilter()
                        {
                            IDs = listIDs
                        };
                        var x = new BackendAdapter(null).Get<List<V_HIS_IMP_MEST>>(HisRequestUriStore.HIS_IMP_MEST_GETVIEW, ApiConsumers.MosConsumer, ImpFilter, SessionManager.ActionLostToken, null);
                        ImpMest.AddRange(x);
                    }
                }

                //
                List<long> sourceMestIds = ImpMest.Where(o => o.CHMS_EXP_MEST_ID.HasValue).Select(s => s.CHMS_EXP_MEST_ID.Value).ToList();
                List<V_HIS_EXP_MEST> sourceMest = new List<V_HIS_EXP_MEST>();
                if (sourceMestIds != null && sourceMestIds.Count > 0)
                {
                    var skip = 0;
                    while (sourceMestIds.Count - skip > 0)
                    {
                        var listIDs = sourceMestIds.Skip(skip).Take(ManagerConstant.MAX_REQUEST_LENGTH_PARAM).ToList();
                        skip = skip + ManagerConstant.MAX_REQUEST_LENGTH_PARAM;
                        HisExpMestViewFilter sourceFilter = new HisExpMestViewFilter()
                        {
                            IDs = listIDs,
                        };
                        var x = new BackendAdapter(null).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, sourceFilter, SessionManager.ActionLostToken, null);
                        sourceMest.AddRange(x);
                    }
                }

                HisExpMestMedicineViewFilter expMediFilter = new HisExpMestMedicineViewFilter();
                expMediFilter.EXP_TIME_FROM = this._ReportFilter.TIME_FROM;
                expMediFilter.EXP_TIME_TO = this._ReportFilter.TIME_TO;
                expMediFilter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
                expMediFilter.MEDICINE_TYPE_ID = this._ReportFilter.MEDICINE_TYPE_ID;
                expMediFilter.IS_EXPORT = true;
                expMediFilter.MEDICINE_ID = this._ReportFilter.MediMateId;
                List<V_HIS_EXP_MEST_MEDICINE> hisExpMestMedicine = new BackendAdapter(null).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, expMediFilter, SessionManager.ActionLostToken, null);
             
                LogSystem.Debug("hisExpMestMedicine.Count: " + hisExpMestMedicine.Count);

                List<long> hisExpMestMedicineIds = hisExpMestMedicine.Select(o => o.EXP_MEST_ID ?? 0).ToList();
                var aggrExpMest = hisExpMestMedicine.Select(o => o.AGGR_EXP_MEST_ID ?? 0).ToList();
                if (aggrExpMest != null && aggrExpMest.Count > 0)
                    hisExpMestMedicineIds.AddRange(aggrExpMest);

                List<V_HIS_EXP_MEST> hisExpMest = new List<V_HIS_EXP_MEST>();
                if (hisExpMestMedicineIds != null && hisExpMestMedicineIds.Count > 0)
                {
                    hisExpMestMedicineIds = hisExpMestMedicineIds.Distinct().ToList();
                    var skip = 0;
                    while (hisExpMestMedicineIds.Count - skip > 0)
                    {
                        var listIDs = hisExpMestMedicineIds.Skip(skip).Take(ManagerConstant.MAX_REQUEST_LENGTH_PARAM).ToList();
                        skip = skip + ManagerConstant.MAX_REQUEST_LENGTH_PARAM;
                        HisExpMestViewFilter ExpFilter = new HisExpMestViewFilter()
                        {
                            IDs = listIDs
                        };
                        var x = new BackendAdapter(null).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, ExpFilter, SessionManager.ActionLostToken, null);
                        hisExpMest.AddRange(x);
                    }
                }

                //
                List<long> destMestIds = hisExpMest.Where(p => p.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK).Select(o => o.ID).ToList();
                List<V_HIS_IMP_MEST> destMest = new List<V_HIS_IMP_MEST>();
                if (destMestIds != null && destMestIds.Count > 0)
                {
                    var skip = 0;
                    while (sourceMestIds.Count - skip > 0)
                    {
                        var listIDs = destMestIds.Skip(skip).Take(ManagerConstant.MAX_REQUEST_LENGTH_PARAM).ToList();
                        skip = skip + ManagerConstant.MAX_REQUEST_LENGTH_PARAM;
                        HisImpMestViewFilter destFilter = new HisImpMestViewFilter()
                        {
                            CHMS_EXP_MEST_IDs = listIDs,
                        };
                        var x = new BackendAdapter(null).Get<List<V_HIS_IMP_MEST>>(HisRequestUriStore.HIS_IMP_MEST_GETVIEW, ApiConsumers.MosConsumer, destFilter, SessionManager.ActionLostToken, null);
                        destMest.AddRange(x);
                    }
                }

                if (hisImpMestMedicines != null && hisImpMestMedicines.Count > 0)
                {
                    var listSub = (from r in hisImpMestMedicines select new Mrs00067RDO(r, ImpMest, sourceMest, impMestTypes, BackendDataWorker.Get<HIS_DEPARTMENT>(), BackendDataWorker.Get<V_HIS_ROOM>())).ToList();

                    this.ProcessGroupByImpMest(listSub);
                    this.ProcessGroupByImpMestPackage(listSub);
                }

                if (hisExpMestMedicine != null && hisExpMestMedicine.Count > 0)
                {
                    var listSub = (from r in hisExpMestMedicine select new Mrs00067RDO(r, hisExpMest, destMest, expMestTypes, BackendDataWorker.Get<HIS_MEDI_STOCK>(), BackendDataWorker.Get<HIS_DEPARTMENT>(), BackendDataWorker.Get<V_HIS_ROOM>())).ToList();

                    this.ProcessGroupByExpMest(listSub);
                    this.ProcessGroupByExpMestPackage(listSub);
                }

                LogSystem.Debug("ListRdo.Count: " + ListRdo.Count);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);

                result = false;
            }
            return result;
        }

        private void ProcessGroupByExpMest(List<Mrs00067RDO> listSub)
        {
            var groupByCode = listSub.GroupBy(o => o.EXP_MEST_CODE).ToList();

            foreach (var group in groupByCode)
            {
                List<Mrs00067RDO> p = group.ToList<Mrs00067RDO>();
                Mrs00067RDO rdo = new Mrs00067RDO()
                {
                    EXP_MEST_CODE = p.First().EXP_MEST_CODE,
                    EXECUTE_DATE_STR = p.First().EXECUTE_DATE_STR,
                    EXECUTE_TIME = p.First().EXECUTE_TIME,
                    IMP_MEST_CODE = p.First().IMP_MEST_CODE,
                    DESCRIPTION = p.First().DESCRIPTION,
                    DESCRIPTION_DETAIL = p.First().DESCRIPTION_DETAIL,
                    CHMS_TYPE_ID = p.First().CHMS_TYPE_ID,
                    IMP_MEST_TYPE_NAME = p.First().IMP_MEST_TYPE_NAME,
                    EXP_MEST_TYPE_NAME = p.First().EXP_MEST_TYPE_NAME,
                    PACKAGE_NUMBER = string.Join(", ", p.Select(q => q.PACKAGE_NUMBER).Distinct().ToList()),
                    EXPIRED_DATE = p.First().EXPIRED_DATE,
                    EXPIRED_DATE_STR = p.First().EXPIRED_DATE_STR,
                    BEGIN_AMOUNT = p.OrderBy(q => q.EXECUTE_TIME).First().BEGIN_AMOUNT,
                    IMP_AMOUNT = p.Sum(q => q.IMP_AMOUNT) == 0 ? null : p.Sum(q => q.IMP_AMOUNT),
                    EXP_AMOUNT = p.Sum(q => q.EXP_AMOUNT) == 0 ? null : p.Sum(q => q.EXP_AMOUNT),
                    END_AMOUNT = p.OrderBy(q => q.EXECUTE_TIME).Last().END_AMOUNT,
                    REQ_DEPARTMENT_NAME = p.First().REQ_DEPARTMENT_NAME,
                    REQUEST_DEPARTMENT_NAME = p.First().REQUEST_DEPARTMENT_NAME,
                    REQ_ROOM_NAME = p.First().REQ_ROOM_NAME,
                    MEDI_STOCK_NAME = p.First().MEDI_STOCK_NAME,
                    EXP_MEDI_STOCK_CODE = p.First().EXP_MEDI_STOCK_CODE,
                    EXP_MEDI_STOCK_NAME = p.First().EXP_MEDI_STOCK_NAME,
                    IMP_MEDI_STOCK_CODE = p.First().IMP_MEDI_STOCK_CODE,
                    IMP_MEDI_STOCK_NAME = p.First().IMP_MEDI_STOCK_NAME,
                    CLIENT_NAME = p.First().CLIENT_NAME,
                    VIR_PATIENT_NAME = p.First().VIR_PATIENT_NAME,
                    VIR_PATIENT_ADDRESS = p.First().VIR_PATIENT_ADDRESS,
                    TREATMENT_CODE = p.First().TREATMENT_CODE,
                    SUPPLIER_NAME = p.First().SUPPLIER_NAME,
                    SECOND_MEST_CODE = p.First().SECOND_MEST_CODE,
                    IMP_PRICE = p.First().IMP_PRICE,
                    IMP_VAT_RATIO = p.First().IMP_VAT_RATIO,
                    IMP_AMOUNT_HOAN = p.First().CHMS_TYPE_ID == 2 ? p.Sum(q => q.IMP_AMOUNT) : 0,
                    IMP_AMOUNT_KHONG_GOM_HOAN = p.First().CHMS_TYPE_ID != 2 ? p.Sum(q => q.IMP_AMOUNT) : 0
                };
                ListRdo.Add(rdo);
            }
        }

        private void ProcessGroupByExpMestPackage(List<Mrs00067RDO> listSub)
        {
            var groupByCode = listSub.GroupBy(o => new { o.EXP_MEST_CODE, o.PACKAGE_NUMBER }).ToList();

            foreach (var group in groupByCode)
            {
                List<Mrs00067RDO> p = group.ToList<Mrs00067RDO>();
                Mrs00067RDO rdo = new Mrs00067RDO()
                {
                    EXP_MEST_CODE = p.First().EXP_MEST_CODE,
                    EXECUTE_DATE_STR = p.First().EXECUTE_DATE_STR,
                    EXECUTE_TIME = p.First().EXECUTE_TIME,
                    IMP_MEST_CODE = p.First().IMP_MEST_CODE,
                    DESCRIPTION = p.First().DESCRIPTION,
                    DESCRIPTION_DETAIL = p.First().DESCRIPTION_DETAIL,
                    CHMS_TYPE_ID = p.First().CHMS_TYPE_ID,
                    IMP_MEST_TYPE_NAME = p.First().IMP_MEST_TYPE_NAME,
                    EXP_MEST_TYPE_NAME = p.First().EXP_MEST_TYPE_NAME,
                    PACKAGE_NUMBER = p.First().PACKAGE_NUMBER,
                    EXPIRED_DATE = p.First().EXPIRED_DATE,
                    EXPIRED_DATE_STR = p.First().EXPIRED_DATE_STR,
                    BEGIN_AMOUNT = p.OrderBy(q => q.EXECUTE_TIME).First().BEGIN_AMOUNT,
                    IMP_AMOUNT = p.Sum(q => q.IMP_AMOUNT) == 0 ? null : p.Sum(q => q.IMP_AMOUNT),
                    EXP_AMOUNT = p.Sum(q => q.EXP_AMOUNT) == 0 ? null : p.Sum(q => q.EXP_AMOUNT),
                    END_AMOUNT = p.OrderBy(q => q.EXECUTE_TIME).Last().END_AMOUNT,
                    REQ_DEPARTMENT_NAME = p.First().REQ_DEPARTMENT_NAME,
                    REQUEST_DEPARTMENT_NAME = p.First().REQUEST_DEPARTMENT_NAME,
                    REQ_ROOM_NAME = p.First().REQ_ROOM_NAME,
                    MEDI_STOCK_NAME = p.First().MEDI_STOCK_NAME,
                    EXP_MEDI_STOCK_CODE = p.First().EXP_MEDI_STOCK_CODE,
                    EXP_MEDI_STOCK_NAME = p.First().EXP_MEDI_STOCK_NAME,
                    IMP_MEDI_STOCK_CODE = p.First().IMP_MEDI_STOCK_CODE,
                    IMP_MEDI_STOCK_NAME = p.First().IMP_MEDI_STOCK_NAME,
                    CLIENT_NAME = p.First().CLIENT_NAME,
                    VIR_PATIENT_NAME = p.First().VIR_PATIENT_NAME,
                    VIR_PATIENT_ADDRESS = p.First().VIR_PATIENT_ADDRESS,
                    TREATMENT_CODE = p.First().TREATMENT_CODE,
                    SUPPLIER_NAME = p.First().SUPPLIER_NAME,
                    SECOND_MEST_CODE = p.First().SECOND_MEST_CODE,
                    IMP_PRICE = p.First().IMP_PRICE,
                    IMP_VAT_RATIO = p.First().IMP_VAT_RATIO
                };
                ListByPackages.Add(rdo);
            }
        }

        private void ProcessGroupByImpMest(List<Mrs00067RDO> listSub)
        {
            var groupByCode = listSub.GroupBy(o => o.IMP_MEST_CODE).ToList();

            foreach (var group in groupByCode)
            {
                List<Mrs00067RDO> p = group.ToList<Mrs00067RDO>();
                Mrs00067RDO rdo = new Mrs00067RDO()
                {
                    EXP_MEST_CODE = p.First().EXP_MEST_CODE,
                    EXECUTE_DATE_STR = p.First().EXECUTE_DATE_STR,
                    EXECUTE_TIME = p.First().EXECUTE_TIME,
                    IMP_MEST_CODE = p.First().IMP_MEST_CODE,
                    DESCRIPTION = p.First().DESCRIPTION,
                    DESCRIPTION_DETAIL = p.First().DESCRIPTION_DETAIL,
                    CHMS_TYPE_ID = p.First().CHMS_TYPE_ID,
                    IMP_MEST_TYPE_NAME = p.First().IMP_MEST_TYPE_NAME,
                    EXP_MEST_TYPE_NAME = p.First().EXP_MEST_TYPE_NAME,
                    PACKAGE_NUMBER = string.Join(", ", p.Select(q => q.PACKAGE_NUMBER).Distinct().ToList()),
                    EXPIRED_DATE = p.First().EXPIRED_DATE,
                    EXPIRED_DATE_STR = p.First().EXPIRED_DATE_STR,
                    BEGIN_AMOUNT = p.OrderBy(q => q.EXECUTE_TIME).First().BEGIN_AMOUNT,
                    IMP_AMOUNT = p.Sum(q => q.IMP_AMOUNT) == 0 ? null : p.Sum(q => q.IMP_AMOUNT),
                    EXP_AMOUNT = p.Sum(q => q.EXP_AMOUNT) == 0 ? null : p.Sum(q => q.EXP_AMOUNT),
                    END_AMOUNT = p.OrderBy(q => q.EXECUTE_TIME).Last().END_AMOUNT,
                    REQ_DEPARTMENT_NAME = p.First().REQ_DEPARTMENT_NAME,
                    REQ_ROOM_NAME = p.First().REQ_ROOM_NAME,
                    MEDI_STOCK_NAME = p.First().MEDI_STOCK_NAME,
                    EXP_MEDI_STOCK_CODE = p.First().EXP_MEDI_STOCK_CODE,
                    EXP_MEDI_STOCK_NAME = p.First().EXP_MEDI_STOCK_NAME,
                    IMP_MEDI_STOCK_CODE = p.First().IMP_MEDI_STOCK_CODE,
                    IMP_MEDI_STOCK_NAME = p.First().IMP_MEDI_STOCK_NAME,
                    CLIENT_NAME = p.First().CLIENT_NAME,
                    VIR_PATIENT_NAME = p.First().VIR_PATIENT_NAME,
                    VIR_PATIENT_ADDRESS = p.First().VIR_PATIENT_ADDRESS,
                    TREATMENT_CODE = p.First().TREATMENT_CODE,
                    SUPPLIER_NAME = p.First().SUPPLIER_NAME,
                    SECOND_MEST_CODE = p.First().SECOND_MEST_CODE,
                    IMP_PRICE = p.First().IMP_PRICE,
                    IMP_VAT_RATIO = p.First().IMP_VAT_RATIO,

                    IMP_AMOUNT_HOAN = p.First().CHMS_TYPE_ID == 2 ? p.Sum(q => q.IMP_AMOUNT) : 0,
                    IMP_AMOUNT_KHONG_GOM_HOAN = p.First().CHMS_TYPE_ID != 2 ? p.Sum(q => q.IMP_AMOUNT) : 0,
                    DOCUMENT_NUMBER = p.First().DOCUMENT_NUMBER
                };
                ListRdo.Add(rdo);
            }
        }

        private void ProcessGroupByImpMestPackage(List<Mrs00067RDO> listSub)
        {
            var groupByCode = listSub.GroupBy(o => new { o.IMP_MEST_CODE, o.PACKAGE_NUMBER }).ToList();

            foreach (var group in groupByCode)
            {
                List<Mrs00067RDO> p = group.ToList<Mrs00067RDO>();
                Mrs00067RDO rdo = new Mrs00067RDO()
                {
                    EXP_MEST_CODE = p.First().EXP_MEST_CODE,
                    EXECUTE_DATE_STR = p.First().EXECUTE_DATE_STR,
                    EXECUTE_TIME = p.First().EXECUTE_TIME,
                    IMP_MEST_CODE = p.First().IMP_MEST_CODE,
                    DESCRIPTION = p.First().DESCRIPTION,
                    DESCRIPTION_DETAIL = p.First().DESCRIPTION_DETAIL,
                    CHMS_TYPE_ID = p.First().CHMS_TYPE_ID,
                    IMP_MEST_TYPE_NAME = p.First().IMP_MEST_TYPE_NAME,
                    EXP_MEST_TYPE_NAME = p.First().EXP_MEST_TYPE_NAME,
                    PACKAGE_NUMBER = p.First().PACKAGE_NUMBER,
                    EXPIRED_DATE = p.First().EXPIRED_DATE,
                    EXPIRED_DATE_STR = p.First().EXPIRED_DATE_STR,
                    BEGIN_AMOUNT = p.OrderBy(q => q.EXECUTE_TIME).First().BEGIN_AMOUNT,
                    IMP_AMOUNT = p.Sum(q => q.IMP_AMOUNT) == 0 ? null : p.Sum(q => q.IMP_AMOUNT),
                    EXP_AMOUNT = p.Sum(q => q.EXP_AMOUNT) == 0 ? null : p.Sum(q => q.EXP_AMOUNT),
                    END_AMOUNT = p.OrderBy(q => q.EXECUTE_TIME).Last().END_AMOUNT,
                    REQ_DEPARTMENT_NAME = p.First().REQ_DEPARTMENT_NAME,
                    REQ_ROOM_NAME = p.First().REQ_ROOM_NAME,
                    MEDI_STOCK_NAME = p.First().MEDI_STOCK_NAME,
                    EXP_MEDI_STOCK_CODE = p.First().EXP_MEDI_STOCK_CODE,
                    EXP_MEDI_STOCK_NAME = p.First().EXP_MEDI_STOCK_NAME,
                    IMP_MEDI_STOCK_CODE = p.First().IMP_MEDI_STOCK_CODE,
                    IMP_MEDI_STOCK_NAME = p.First().IMP_MEDI_STOCK_NAME,
                    CLIENT_NAME = p.First().CLIENT_NAME,
                    VIR_PATIENT_NAME = p.First().VIR_PATIENT_NAME,
                    VIR_PATIENT_ADDRESS = p.First().VIR_PATIENT_ADDRESS,
                    TREATMENT_CODE = p.First().TREATMENT_CODE,
                    SUPPLIER_NAME = p.First().SUPPLIER_NAME,
                    SECOND_MEST_CODE = p.First().SECOND_MEST_CODE,
                    IMP_PRICE = p.First().IMP_PRICE,
                    IMP_VAT_RATIO = p.First().IMP_VAT_RATIO,
                    IMP_AMOUNT_HOAN = p.First().CHMS_TYPE_ID == 2 ? p.Sum(q => q.IMP_AMOUNT) : 0,
                    IMP_AMOUNT_KHONG_GOM_HOAN = p.First().CHMS_TYPE_ID != 2 ? p.Sum(q => q.IMP_AMOUNT) : 0,
                    DOCUMENT_NUMBER = p.First().DOCUMENT_NUMBER
                };
                ListByPackages.Add(rdo);
            }
        }

        private bool ProcessData()
        {
            bool result = false;
            try
            {
                if (ListRdo != null || ListByPackages != null)
                {
                    ProcessBeginAndEndAmount();
                }
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                ListRdo.Clear();

            }
            return result;
        }

        private void ProcessBeginAndEndAmount()
        {
            try
            {
                CommonParam paramGet = new CommonParam();
                ProcessGetPeriod(paramGet);
                ListRdo = ListRdo.OrderBy(o => o.EXECUTE_TIME).ToList();
                //ListRdo = ListRdo.GroupBy(g => g.EXECUTE_DATE_STR).Select(s => new Mrs00067RDO
                //{
                //    EXECUTE_DATE_STR = s.First().EXECUTE_DATE_STR,
                //    EXP_AMOUNT = s.Sum(s1 => s1.EXP_AMOUNT),
                //    IMP_AMOUNT = s.Sum(s2 => s2.IMP_AMOUNT),
                //    EXPIRED_DATE_STR = (s.First(s3 => !String.IsNullOrWhiteSpace(s3.EXPIRED_DATE_STR)) ?? new Mrs00067RDO()).EXPIRED_DATE_STR,
                //    PACKAGE_NUMBER = string.Join(",", s.Select(o => o.PACKAGE_NUMBER).Distinct())
                //}).ToList();
                ListByPackages = ListByPackages.OrderBy(o => o.EXECUTE_TIME).ToList();
                //ListByPackages = ListByPackages.GroupBy(g => g.EXECUTE_DATE_STR).Select(s => new Mrs00067RDO
                //{
                //    EXECUTE_DATE_STR = s.First().EXECUTE_DATE_STR,
                //    EXP_AMOUNT = s.Sum(s1 => s1.EXP_AMOUNT),
                //    IMP_AMOUNT = s.Sum(s2 => s2.IMP_AMOUNT),
                //    EXPIRED_DATE_STR = (s.First(s3 => !String.IsNullOrWhiteSpace(s3.EXPIRED_DATE_STR)) ?? new Mrs00067RDO()).EXPIRED_DATE_STR,
                //    PACKAGE_NUMBER = string.Join(",", s.Select(o => o.PACKAGE_NUMBER).Distinct())
                //}).ToList();
                decimal previousEndAmount = startBeginAmount;
                BeginAmount = startBeginAmount;
                foreach (var rdo in ListRdo)
                {
                    rdo.CalculateAmount(previousEndAmount);
                    previousEndAmount = rdo.END_AMOUNT;
                }
                EndAmount = previousEndAmount;

                decimal packgeEndAmount = startBeginAmount;
                foreach (var rdo in ListByPackages)
                {
                    rdo.CalculateAmount(packgeEndAmount);
                    packgeEndAmount = rdo.END_AMOUNT;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // Lay kỳ chốt gần nhất với thời gian từ của báo cáo
        private void ProcessGetPeriod(CommonParam paramGet)
        {
            try
            {
                HisMediStockPeriodFilter periodFilter = new HisMediStockPeriodFilter();
                periodFilter.TO_TIME_TO = this._ReportFilter.TIME_FROM;
                periodFilter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
                List<HIS_MEDI_STOCK_PERIOD> hisMediStockPeriod = new BackendAdapter(null).Get<List<HIS_MEDI_STOCK_PERIOD>>("api/HisMediStockPeriod/Get", ApiConsumers.MosConsumer, periodFilter, SessionManager.ActionLostToken, null);
                if (!paramGet.HasException)
                {
                    if (hisMediStockPeriod != null && hisMediStockPeriod.Count > 0)
                    {
                        //Trường hợp có kỳ được chốt gần nhất
                        HIS_MEDI_STOCK_PERIOD neighborPeriod = hisMediStockPeriod.OrderByDescending(d => d.TO_TIME).ToList()[0];
                        if (neighborPeriod != null)
                        {
                            ProcessBeinAmountMedicineByMediStockPeriod(paramGet, neighborPeriod);
                        }
                    }
                    else
                    {
                        // Trường hợp không có kỳ chốt gần nhât - chưa được chốt kỳ nào
                        ProcessBeinAmountMedicineNotMediStockPriod(paramGet);
                    }

                    if (paramGet.HasException)
                    {
                        throw new DataMisalignedException("Co exception xay ra tai DAOGET trong qua trinh tong hop du lieu.");
                    }
                }
                else
                {
                    throw new DataMisalignedException("Co exception xay ra tai DAOGET trong qua trinh tong hop du lieu.");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // Tính số lượng đầu kỳ có kỳ dữ liệu gần nhất
        private void ProcessBeinAmountMedicineByMediStockPeriod(CommonParam paramGet, HIS_MEDI_STOCK_PERIOD neighborPeriod)
        {
            try
            {
                List<Mrs00067RDO> listrdo = new List<Mrs00067RDO>();
                HisMestPeriodMediViewFilter periodMetyFilter = new HisMestPeriodMediViewFilter();
                periodMetyFilter.MEDI_STOCK_PERIOD_ID = neighborPeriod.ID;
                periodMetyFilter.MEDICINE_TYPE_ID = this._ReportFilter.MEDICINE_TYPE_ID;
                periodMetyFilter.MEDICINE_ID = this._ReportFilter.MediMateId;
                List<V_HIS_MEST_PERIOD_MEDI> hisMestPeriodMedi = new BackendAdapter(null).Get<List<V_HIS_MEST_PERIOD_MEDI>>("api/HisMestPeriodMedi/GetView", ApiConsumers.MosConsumer, periodMetyFilter, SessionManager.ActionLostToken, null);
                if (hisMestPeriodMedi != null && hisMestPeriodMedi.Count > 0)
                {
                    startBeginAmount = hisMestPeriodMedi.Sum(s => s.AMOUNT);
                }
                ProcessMedicineBefore(hisMestPeriodMedi);
                HisImpMestMedicineViewFilter impMediFilter = new HisImpMestMedicineViewFilter();
                impMediFilter.IMP_TIME_FROM = neighborPeriod.TO_TIME + 1;
                impMediFilter.IMP_TIME_TO = this._ReportFilter.TIME_FROM - 1;
                impMediFilter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
                impMediFilter.MEDICINE_TYPE_ID = this._ReportFilter.MEDICINE_TYPE_ID;
                impMediFilter.MEDICINE_ID = this._ReportFilter.MediMateId;
                impMediFilter.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT;
                List<V_HIS_IMP_MEST_MEDICINE> hisImpMestMedicine = new BackendAdapter(null).Get<List<V_HIS_IMP_MEST_MEDICINE>>("api/HisImpMestMedicine/GetView", ApiConsumers.MosConsumer, impMediFilter, SessionManager.ActionLostToken, null);
                if (hisImpMestMedicine != null && hisImpMestMedicine.Count > 0)
                {
                    startBeginAmount += hisImpMestMedicine.Sum(s => s.AMOUNT);
                }

                ProcessMedicineBefore(hisImpMestMedicine);
                HisExpMestMedicineViewFilter expMediFilter = new HisExpMestMedicineViewFilter();
                expMediFilter.EXP_TIME_FROM = neighborPeriod.TO_TIME + 1;
                expMediFilter.EXP_TIME_TO = this._ReportFilter.TIME_FROM - 1;
                expMediFilter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
                expMediFilter.MEDICINE_TYPE_ID = this._ReportFilter.MEDICINE_TYPE_ID;
                expMediFilter.MEDICINE_ID = this._ReportFilter.MediMateId;
                expMediFilter.IS_EXPORT = true;
                List<V_HIS_EXP_MEST_MEDICINE> hisExpMestMedicine = new BackendAdapter(null).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, expMediFilter, SessionManager.ActionLostToken, null);
                ProcessMedicineBefore(hisExpMestMedicine);
                if (hisExpMestMedicine != null && hisExpMestMedicine.Count > 0)
                {
                    startBeginAmount -= hisExpMestMedicine.Sum(s => s.AMOUNT);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessMedicineBefore(List<V_HIS_EXP_MEST_MEDICINE> hisExpMestMedicine)
        {
            try
            {
                foreach (var item in hisExpMestMedicine)
                {
                    Mrs00067RDO rdo = new Mrs00067RDO();
                    if (!dicMedicine.ContainsKey(item.MEDICINE_ID ?? 0))
                    {
                        dicMedicine[item.MEDICINE_ID ?? 0] = new Mrs00067RDO();
                        dicMedicine[item.MEDICINE_ID ?? 0].MEDICINE_ID = item.ID;
                        dicMedicine[item.MEDICINE_ID ?? 0].EXP_MEST_TYPE_NAME = "Tồn cuối kỳ";
                        dicMedicine[item.MEDICINE_ID ?? 0].EXECUTE_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._ReportFilter.TIME_TO);
                        dicMedicine[item.MEDICINE_ID ?? 0].PACKAGE_NUMBER = item.PACKAGE_NUMBER;
                        dicMedicine[item.MEDICINE_ID ?? 0].EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.EXPIRED_DATE ?? 0);
                        dicMedicine[item.MEDICINE_ID ?? 0].BEGIN_AMOUNT = -item.AMOUNT;
                        dicMedicine[item.MEDICINE_ID ?? 0].END_AMOUNT = -item.AMOUNT;
                        dicMedicine[item.MEDICINE_ID ?? 0].IMP_PRICE = item.IMP_PRICE;
                        dicMedicine[item.MEDICINE_ID ?? 0].IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                    }
                    else
                    {
                        dicMedicine[item.MEDICINE_ID ?? 0].BEGIN_AMOUNT -= item.AMOUNT;
                        dicMedicine[item.MEDICINE_ID ?? 0].END_AMOUNT -= item.AMOUNT;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessMedicineBefore(List<V_HIS_IMP_MEST_MEDICINE> hisImpMestMedicine)
        {
            try
            {
                foreach (var item in hisImpMestMedicine)
                {
                    Mrs00067RDO rdo = new Mrs00067RDO();
                    if (!dicMedicine.ContainsKey(item.MEDICINE_ID))
                    {
                        dicMedicine[item.MEDICINE_ID] = new Mrs00067RDO();
                        dicMedicine[item.MEDICINE_ID].MEDICINE_ID = item.ID;
                        dicMedicine[item.MEDICINE_ID].EXP_MEST_TYPE_NAME = "Tồn cuối kỳ";
                        dicMedicine[item.MEDICINE_ID].EXECUTE_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._ReportFilter.TIME_TO);
                        dicMedicine[item.MEDICINE_ID].PACKAGE_NUMBER = item.PACKAGE_NUMBER;
                        dicMedicine[item.MEDICINE_ID].EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.EXPIRED_DATE ?? 0);
                        dicMedicine[item.MEDICINE_ID].BEGIN_AMOUNT = item.AMOUNT;
                        dicMedicine[item.MEDICINE_ID].END_AMOUNT = item.AMOUNT;
                        dicMedicine[item.MEDICINE_ID].IMP_PRICE = item.IMP_PRICE;
                        dicMedicine[item.MEDICINE_ID].IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                        dicMedicine[item.MEDICINE_ID].DOCUMENT_NUMBER = item.DOCUMENT_NUMBER;
                    }
                    else
                    {
                        dicMedicine[item.MEDICINE_ID].BEGIN_AMOUNT += item.AMOUNT;
                        dicMedicine[item.MEDICINE_ID].END_AMOUNT += item.AMOUNT;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessMedicineBefore(List<V_HIS_MEST_PERIOD_MEDI> hisMestPeriodMedi)
        {
            try
            {
                foreach (var item in hisMestPeriodMedi)
                {
                    Mrs00067RDO rdo = new Mrs00067RDO();
                    if (!dicMedicine.ContainsKey(item.MEDICINE_ID))
                    {
                        dicMedicine[item.MEDICINE_ID] = new Mrs00067RDO();
                        dicMedicine[item.MEDICINE_ID].MEDICINE_ID = item.ID;
                        dicMedicine[item.MEDICINE_ID].EXP_MEST_TYPE_NAME = "Tồn cuối kỳ";
                        dicMedicine[item.MEDICINE_ID].EXECUTE_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._ReportFilter.TIME_TO);
                        dicMedicine[item.MEDICINE_ID].PACKAGE_NUMBER = item.PACKAGE_NUMBER;
                        dicMedicine[item.MEDICINE_ID].EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.EXPIRED_DATE ?? 0);
                        dicMedicine[item.MEDICINE_ID].BEGIN_AMOUNT = item.AMOUNT;
                        dicMedicine[item.MEDICINE_ID].END_AMOUNT = item.AMOUNT;
                        dicMedicine[item.MEDICINE_ID].IMP_PRICE = item.IMP_PRICE;
                        dicMedicine[item.MEDICINE_ID].IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                    }
                    else
                    {
                        dicMedicine[item.MEDICINE_ID].BEGIN_AMOUNT += item.AMOUNT;
                        dicMedicine[item.MEDICINE_ID].END_AMOUNT += item.AMOUNT;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Tính số lượng đầu kỳ không có kỳ dữ liệu gần nhất
        private void ProcessBeinAmountMedicineNotMediStockPriod(CommonParam paramGet)
        {
            try
            {
                HisImpMestMedicineViewFilter impMediFilter = new HisImpMestMedicineViewFilter();
                impMediFilter.IMP_TIME_TO = this._ReportFilter.TIME_FROM - 1;
                impMediFilter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
                impMediFilter.MEDICINE_TYPE_ID = this._ReportFilter.MEDICINE_TYPE_ID;
                impMediFilter.MEDICINE_ID = this._ReportFilter.MediMateId;
                impMediFilter.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT;
                List<V_HIS_IMP_MEST_MEDICINE> hisImpMestMedicine = new BackendAdapter(null).Get<List<V_HIS_IMP_MEST_MEDICINE>>("api/HisImpMestMedicine/GetView", ApiConsumers.MosConsumer, impMediFilter, SessionManager.ActionLostToken, null);
                if (hisImpMestMedicine != null && hisImpMestMedicine.Count > 0)
                {
                    startBeginAmount = hisImpMestMedicine.Sum(s => s.AMOUNT);
                }

                ProcessMedicineBefore(hisImpMestMedicine);
                HisExpMestMedicineViewFilter expMediFilter = new HisExpMestMedicineViewFilter();
                expMediFilter.EXP_TIME_TO = this._ReportFilter.TIME_FROM - 1;
                expMediFilter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
                expMediFilter.MEDICINE_TYPE_ID = this._ReportFilter.MEDICINE_TYPE_ID;
                expMediFilter.MEDICINE_ID = this._ReportFilter.MediMateId;
                expMediFilter.IS_EXPORT = true;
                List<V_HIS_EXP_MEST_MEDICINE> hisExpMestMedicine = new BackendAdapter(null).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, expMediFilter, SessionManager.ActionLostToken, null);
                if (hisExpMestMedicine != null && hisExpMestMedicine.Count > 0)
                {
                    startBeginAmount -= hisExpMestMedicine.Sum(s => s.AMOUNT);
                }
                ProcessMedicineBefore(hisExpMestMedicine);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
