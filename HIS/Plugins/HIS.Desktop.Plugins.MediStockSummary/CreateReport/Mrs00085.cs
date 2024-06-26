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
using MPS.Processor.Mps000220.PDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MediStockSummary.CreateReport
{
    public class Mrs00085
    {
        decimal startBeginAmount;
        CommonParam paramGet = new CommonParam();
        List<Mrs00085RDO> ListRdo = new List<Mrs00085RDO>();
        List<Mrs00085RDO> ListByPackage = new List<Mrs00085RDO>();
        Dictionary<long, Mrs00085RDO> dicMaterial = new Dictionary<long, Mrs00085RDO>();
        List<V_HIS_SERE_SERV> ListSereServ = new List<V_HIS_SERE_SERV>();
        List<V_HIS_EXP_MEST_MATERIAL> listExpMestMaterialView = new List<V_HIS_EXP_MEST_MATERIAL>();
        List<V_HIS_SERE_SERV> listSereServ = new List<V_HIS_SERE_SERV>();
        HIS_DEPARTMENT department = new HIS_DEPARTMENT();
        HIS_MEDI_STOCK mateStock = new HIS_MEDI_STOCK();
        decimal BeginAmount = 0;
        decimal EndAmount = 0;
        private string a = "";
        long RoomId;
        internal FilterADO _ReportFilter { get; set; }

        public Mrs00085(long roomId)
        {
            this.RoomId = roomId;
        }

        public void LoadDataPrint220(FilterADO _FilterADO, string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                this._ReportFilter = _FilterADO;

                GetData();

                ProcessData();

                MPS.Processor.Mps000220.PDO.SingKey220 _SingKey220 = new MPS.Processor.Mps000220.PDO.SingKey220();

                if (this._ReportFilter.TIME_FROM > 0)
                {
                    _SingKey220.TIME_TO_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this._ReportFilter.TIME_FROM);
                }
                if (this._ReportFilter.TIME_TO > 0)
                {
                    _SingKey220.TIME_FROM_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this._ReportFilter.TIME_TO);
                }
                _SingKey220.BEGIN_AMOUNT = BeginAmount.ToString();
                _SingKey220.END_AMOUNT = EndAmount.ToString();
                V_HIS_MATERIAL_TYPE medicineType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == this._ReportFilter.MATERIAL_TYPE_ID);
                if (medicineType != null)
                {
                    _SingKey220.MATERIAL_TYPE_CODE = medicineType.MATERIAL_TYPE_CODE;
                    _SingKey220.MATERIAL_TYPE_NAME = medicineType.MATERIAL_TYPE_NAME;
                    _SingKey220.SERVICE_UNIT_NAME = medicineType.SERVICE_UNIT_NAME;
                    _SingKey220.NATIONAL_NAME = medicineType.NATIONAL_NAME;
                    _SingKey220.MANUFACTURER_NAME = medicineType.MANUFACTURER_NAME;
                    _SingKey220.DIC_OTHER_KEY = SetOtherKey(_SingKey220.DIC_OTHER_KEY, medicineType, medicineType.GetType().Name);
                }

                HIS_MEDI_STOCK mediStock = BackendDataWorker.Get<HIS_MEDI_STOCK>().FirstOrDefault(p => p.ID == this._ReportFilter.MEDI_STOCK_ID);
                if (mediStock != null)
                {
                    _SingKey220.MEDI_STOCK_CODE = mediStock.MEDI_STOCK_CODE;
                    _SingKey220.MEDI_STOCK_NAME = mediStock.MEDI_STOCK_NAME;
                }

                if (ListRdo != null && ListRdo.Count > 0)
                {
                    _SingKey220.MEDI_BEGIN_AMOUNT = ListRdo.First().BEGIN_AMOUNT.ToString();
                    _SingKey220.MEDI_END_AMOUNT = ListRdo.Last().END_AMOUNT.ToString();
                }

                var listMedicine = new List<Mrs00085RDO>();
                //if (ListRdo.Count == 0)
                {
                    listMedicine = dicMaterial.Values.ToList();
                }

                MPS.Processor.Mps000220.PDO.Mps000220PDO mps000220RDO = new MPS.Processor.Mps000220.PDO.Mps000220PDO(
                    ListRdo,
                    listMedicine,
                    ListByPackage,
                    _SingKey220
               );
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000220RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000220RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
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

                HisImpMestMaterialViewFilter impMateFilter = new HisImpMestMaterialViewFilter();
                impMateFilter.IMP_TIME_FROM = this._ReportFilter.TIME_FROM;
                impMateFilter.IMP_TIME_TO = this._ReportFilter.TIME_TO;
                impMateFilter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
                impMateFilter.MATERIAL_TYPE_ID = this._ReportFilter.MATERIAL_TYPE_ID;
                impMateFilter.MATERIAL_ID = this._ReportFilter.MediMateId;
                impMateFilter.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT;
                List<V_HIS_IMP_MEST_MATERIAL> hisImpMestMaterials = new BackendAdapter(null).Get<List<V_HIS_IMP_MEST_MATERIAL>>("api/HisImpMestMaterial/GetView", ApiConsumers.MosConsumer, impMateFilter, SessionManager.ActionLostToken, null);

                List<long> impMestIds = hisImpMestMaterials.Select(o => o.IMP_MEST_ID).Distinct().ToList();
                var aggrImpId = hisImpMestMaterials.Select(o => o.AGGR_IMP_MEST_ID ?? 0).Distinct().ToList();
                if (aggrImpId != null && aggrImpId.Count > 0)
                    impMestIds.AddRange(aggrImpId);

                //
                List<V_HIS_IMP_MEST> ImpMest = new List<V_HIS_IMP_MEST>();
                if (impMestIds != null && impMestIds.Count > 0)
                {
                    impMestIds = impMestIds.Distinct().ToList();
                    var skip = 0;
                    long i = 0;
                    while (impMestIds.Count - skip > 0)
                    {
                        i++;
                        var listIDs = impMestIds.Skip(skip).Take(ManagerConstant.MAX_REQUEST_LENGTH_PARAM).ToList();
                        skip = skip + ManagerConstant.MAX_REQUEST_LENGTH_PARAM;
                        HisImpMestViewFilter ImpFilter = new HisImpMestViewFilter()
                        {
                            IDs = listIDs
                        };
                        var x = new BackendAdapter(null).Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetView", ApiConsumers.MosConsumer, ImpFilter, SessionManager.ActionLostToken, null);
                        ImpMest.AddRange(x);
                    }
                }

                //
                List<long> sourceMestIds = ImpMest.Where(w => w.CHMS_EXP_MEST_ID.HasValue).Select(o => o.CHMS_EXP_MEST_ID.Value).Distinct().ToList();
                List<V_HIS_EXP_MEST> sourceMest = new List<V_HIS_EXP_MEST>();
                if (sourceMestIds != null && sourceMestIds.Count > 0)
                {
                    var skip = 0;
                    long i = 0;
                    while (sourceMestIds.Count - skip > 0)
                    {
                        i++;
                        var listIDs = sourceMestIds.Skip(skip).Take(ManagerConstant.MAX_REQUEST_LENGTH_PARAM).ToList();
                        skip = skip + ManagerConstant.MAX_REQUEST_LENGTH_PARAM;
                        HisExpMestViewFilter sourceFilter = new HisExpMestViewFilter()
                        {
                            IDs = listIDs
                        };
                        var x = new BackendAdapter(null).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, sourceFilter, SessionManager.ActionLostToken, null);
                        sourceMest.AddRange(x);
                    }
                }

                HisExpMestMaterialViewFilter expMateFilter = new HisExpMestMaterialViewFilter();
                expMateFilter.EXP_TIME_FROM = this._ReportFilter.TIME_FROM;
                expMateFilter.EXP_TIME_TO = this._ReportFilter.TIME_TO;
                expMateFilter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
                expMateFilter.MATERIAL_ID = this._ReportFilter.MediMateId;
                expMateFilter.MATERIAL_TYPE_ID = this._ReportFilter.MATERIAL_TYPE_ID;
                expMateFilter.IS_EXPORT = true;
                List<V_HIS_EXP_MEST_MATERIAL> hisExpMestMaterial = new BackendAdapter(null).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, expMateFilter, SessionManager.ActionLostToken, null);

                List<long> hisExpMestMaterialIds = hisExpMestMaterial.Select(o => o.EXP_MEST_ID ?? 0).Distinct().ToList();
                var aggrExpId = hisExpMestMaterial.Select(o => o.AGGR_EXP_MEST_ID ?? 0).ToList();
                if (aggrExpId != null && aggrExpId.Count > 0)
                    hisExpMestMaterialIds.AddRange(aggrExpId);

                List<V_HIS_EXP_MEST> hisExpMest = new List<V_HIS_EXP_MEST>();
                if (hisExpMestMaterialIds != null && hisExpMestMaterialIds.Count > 0)
                {
                    hisExpMestMaterialIds = hisExpMestMaterialIds.Distinct().ToList();
                    var skip = 0;
                    long i = 0;
                    while (hisExpMestMaterialIds.Count - skip > 0)
                    {
                        i++;
                        var listIDs = hisExpMestMaterialIds.Skip(skip).Take(ManagerConstant.MAX_REQUEST_LENGTH_PARAM).ToList();
                        skip = skip + ManagerConstant.MAX_REQUEST_LENGTH_PARAM;
                        HisExpMestViewFilter ExpFilter = new HisExpMestViewFilter()
                        {
                            IDs = listIDs
                        };
                        var x = new BackendAdapter(null).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, ExpFilter, SessionManager.ActionLostToken, null);
                        hisExpMest.AddRange(x);
                    }
                }

                //
                List<long> destMestIds = hisExpMest.Where(p => p.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK).Select(o => o.ID).ToList();
                List<V_HIS_IMP_MEST> destMest = new List<V_HIS_IMP_MEST>();
                if (destMestIds != null && destMestIds.Count > 0)
                {
                    var skip = 0;
                    long i = 0;
                    while (sourceMestIds.Count - skip > 0)
                    {
                        i++;
                        var listIDs = destMestIds.Skip(skip).Take(ManagerConstant.MAX_REQUEST_LENGTH_PARAM).ToList();
                        skip = skip + ManagerConstant.MAX_REQUEST_LENGTH_PARAM;
                        HisImpMestViewFilter destFilter = new HisImpMestViewFilter()
                        {
                            CHMS_EXP_MEST_IDs = listIDs
                        };
                        var x = new BackendAdapter(null).Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetView", ApiConsumers.MosConsumer, destFilter, SessionManager.ActionLostToken, null);
                        destMest.AddRange(x);
                    }
                }

                if (hisImpMestMaterials != null && hisImpMestMaterials.Count > 0)
                {
                    var listSub = (from r in hisImpMestMaterials select new Mrs00085RDO(r, ImpMest, sourceMest, impMestTypes, BackendDataWorker.Get<HIS_DEPARTMENT>(), BackendDataWorker.Get<V_HIS_ROOM>())).ToList();

                    this.ProcessGroupByImpMest(listSub);

                    this.ProcessGroupByImpMestPackage(listSub);
                }

                if (hisExpMestMaterial != null && hisExpMestMaterial.Count > 0)
                {
                    var listSub = (from r in hisExpMestMaterial select new Mrs00085RDO(r, hisExpMest, destMest, expMestTypes, BackendDataWorker.Get<HIS_MEDI_STOCK>(), BackendDataWorker.Get<HIS_DEPARTMENT>(), BackendDataWorker.Get<V_HIS_ROOM>())).ToList();

                    this.ProcessGroupByExpMest(listSub);

                    this.ProcessGroupByExpMestPackage(listSub);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);

                result = false;
            }
            return result;
        }

        private void ProcessGroupByExpMest(List<Mrs00085RDO> listSub)
        {
            var groupByCode = listSub.GroupBy(o => o.EXP_MEST_CODE).ToList();

            foreach (var group in groupByCode)
            {
                List<Mrs00085RDO> p = group.ToList<Mrs00085RDO>();
                Mrs00085RDO rdo = new Mrs00085RDO()
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
                    SECOND_MEST_CODE = p.First().SECOND_MEST_CODE,
                    IMP_PRICE = p.First().IMP_PRICE,
                    IMP_VAT_RATIO = p.First().IMP_VAT_RATIO,
                    IMP_AMOUNT_HOAN = p.First().CHMS_TYPE_ID == 2 ? p.Sum(q => q.IMP_AMOUNT) : null ,
                    IMP_AMOUNT_KHONG_GOM_HOAN = p.First().CHMS_TYPE_ID != 2 ? p.Sum(q => q.IMP_AMOUNT) : 0
                };
                ListRdo.Add(rdo);
            }
        }

        private void ProcessGroupByExpMestPackage(List<Mrs00085RDO> listSub)
        {
            var groupByCode = listSub.GroupBy(o => new { o.EXP_MEST_CODE, o.PACKAGE_NUMBER }).ToList();

            foreach (var group in groupByCode)
            {
                List<Mrs00085RDO> p = group.ToList<Mrs00085RDO>();
                Mrs00085RDO rdo = new Mrs00085RDO()
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
                    SECOND_MEST_CODE = p.First().SECOND_MEST_CODE,
                    IMP_PRICE = p.First().IMP_PRICE,
                    IMP_VAT_RATIO = p.First().IMP_VAT_RATIO,
                    IMP_AMOUNT_HOAN = p.First().CHMS_TYPE_ID == 2 ? p.Sum(q => q.IMP_AMOUNT) : null,
                   IMP_AMOUNT_KHONG_GOM_HOAN = p.First().CHMS_TYPE_ID != 2 ? p.Sum(q => q.IMP_AMOUNT) : 0
                };
                ListByPackage.Add(rdo);
            }
        }

        private void ProcessGroupByImpMest(List<Mrs00085RDO> listSub)
        {
            var groupByCode = listSub.GroupBy(o => o.IMP_MEST_CODE).ToList();

            foreach (var group in groupByCode)
            {
                List<Mrs00085RDO> p = group.ToList<Mrs00085RDO>();
                Mrs00085RDO rdo = new Mrs00085RDO()
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
                    IMP_AMOUNT_HOAN = p.First().CHMS_TYPE_ID == 2 ? p.Sum(q => q.IMP_AMOUNT) : null,
                    IMP_AMOUNT_KHONG_GOM_HOAN = p.First().CHMS_TYPE_ID != 2 ? p.Sum(q => q.IMP_AMOUNT) : 0
                };
                ListRdo.Add(rdo);
            }
        }

        private void ProcessGroupByImpMestPackage(List<Mrs00085RDO> listSub)
        {
            var groupByCode = listSub.GroupBy(o => new { o.IMP_MEST_CODE, o.PACKAGE_NUMBER }).ToList();

            foreach (var group in groupByCode)
            {
                List<Mrs00085RDO> p = group.ToList<Mrs00085RDO>();
                Mrs00085RDO rdo = new Mrs00085RDO()
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
                    IMP_AMOUNT_HOAN = p.First().CHMS_TYPE_ID == 2 ? p.Sum(q => q.IMP_AMOUNT) : null,
                    IMP_AMOUNT_KHONG_GOM_HOAN = p.First().CHMS_TYPE_ID != 2 ? p.Sum(q => q.IMP_AMOUNT) : 0
                };
                ListByPackage.Add(rdo);
            }
        }

        private bool ProcessData()
        {
            bool result = false;
            try
            {
                if (ListRdo != null || ListByPackage != null)
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
                //ListRdo = ListRdo.GroupBy(g => g.EXECUTE_DATE_STR).Select(s => new Mrs00085RDO
                //{
                //    EXECUTE_DATE_STR = s.First().EXECUTE_DATE_STR,
                //    EXP_AMOUNT = s.Sum(s1 => s1.EXP_AMOUNT),
                //    IMP_AMOUNT = s.Sum(s2 => s2.IMP_AMOUNT),
                //    EXPIRED_DATE_STR = (s.First(s3 => !String.IsNullOrWhiteSpace(s3.EXPIRED_DATE_STR)) ?? new Mrs00085RDO()).EXPIRED_DATE_STR,
                //    PACKAGE_NUMBER = string.Join(",", s.Select(o => o.PACKAGE_NUMBER).Distinct())
                //}).ToList();
                ListByPackage = ListByPackage.OrderBy(o => o.EXECUTE_TIME).ToList();
                //ListByPackage = ListByPackage.GroupBy(g => g.EXECUTE_DATE_STR).Select(s => new Mrs00085RDO
                //{
                //    EXECUTE_DATE_STR = s.First().EXECUTE_DATE_STR,
                //    EXP_AMOUNT = s.Sum(s1 => s1.EXP_AMOUNT),
                //    IMP_AMOUNT = s.Sum(s2 => s2.IMP_AMOUNT),
                //    EXPIRED_DATE_STR = (s.First(s3 => !String.IsNullOrWhiteSpace(s3.EXPIRED_DATE_STR)) ?? new Mrs00085RDO()).EXPIRED_DATE_STR,
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

                decimal packageEndAmount = startBeginAmount;
                foreach (var rdo in ListRdo)
                {
                    rdo.CalculateAmount(packageEndAmount);
                    packageEndAmount = rdo.END_AMOUNT;
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
                            ProcessBeinAmountMaterialByMediStockPeriod(paramGet, neighborPeriod);
                        }
                    }
                    else
                    {
                        // Trường hợp không có kỳ chốt gần nhât - chưa được chốt kỳ nào
                        ProcessBeinAmountMaterialNotMediStockPriod(paramGet);
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
        private void ProcessBeinAmountMaterialByMediStockPeriod(CommonParam paramGet, HIS_MEDI_STOCK_PERIOD neighborPeriod)
        {
            try
            {
                List<Mrs00085RDO> listrdo = new List<Mrs00085RDO>();
                HisMestPeriodMateViewFilter periodMateFilter = new HisMestPeriodMateViewFilter();
                periodMateFilter.MEDI_STOCK_PERIOD_ID = neighborPeriod.ID;
                periodMateFilter.MATERIAL_TYPE_ID = this._ReportFilter.MATERIAL_TYPE_ID;
                periodMateFilter.MATERIAL_ID = this._ReportFilter.MediMateId;
                List<V_HIS_MEST_PERIOD_MATE> hisMestPeriodMate = new BackendAdapter(null).Get<List<V_HIS_MEST_PERIOD_MATE>>("api/HisMestPeriodMate/GetView", ApiConsumers.MosConsumer, periodMateFilter, SessionManager.ActionLostToken, null);
                if (hisMestPeriodMate != null && hisMestPeriodMate.Count > 0)
                {
                    startBeginAmount = hisMestPeriodMate.Sum(s => s.AMOUNT);
                }
                ProcessMaterialBefore(hisMestPeriodMate);
                HisImpMestMaterialViewFilter impMateFilter = new HisImpMestMaterialViewFilter();
                impMateFilter.IMP_TIME_FROM = neighborPeriod.TO_TIME + 1;
                impMateFilter.IMP_TIME_TO = this._ReportFilter.TIME_FROM - 1;
                impMateFilter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
                impMateFilter.MATERIAL_TYPE_ID = this._ReportFilter.MATERIAL_TYPE_ID;
                impMateFilter.MATERIAL_ID = this._ReportFilter.MediMateId;
                impMateFilter.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT;
                List<V_HIS_IMP_MEST_MATERIAL> hisImpMestMaterial = new BackendAdapter(null).Get<List<V_HIS_IMP_MEST_MATERIAL>>("api/HisImpMestMaterial/GetView", ApiConsumers.MosConsumer, impMateFilter, SessionManager.ActionLostToken, null);
                if (hisImpMestMaterial != null && hisImpMestMaterial.Count > 0)
                {
                    startBeginAmount += hisImpMestMaterial.Sum(s => s.AMOUNT);
                }

                ProcessMaterialBefore(hisImpMestMaterial);
                HisExpMestMaterialViewFilter expMateFilter = new HisExpMestMaterialViewFilter();
                expMateFilter.EXP_TIME_FROM = neighborPeriod.TO_TIME;
                expMateFilter.EXP_TIME_TO = this._ReportFilter.TIME_FROM - 1;
                expMateFilter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
                expMateFilter.MATERIAL_TYPE_ID = this._ReportFilter.MATERIAL_TYPE_ID;
                expMateFilter.MATERIAL_ID = this._ReportFilter.MediMateId;
                expMateFilter.IS_EXPORT = true;
                List<V_HIS_EXP_MEST_MATERIAL> hisExpMestMaterial = new BackendAdapter(null).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, expMateFilter, SessionManager.ActionLostToken, null);
                ProcessMaterialBefore(hisExpMestMaterial);
                if (hisExpMestMaterial != null && hisExpMestMaterial.Count > 0)
                {
                    startBeginAmount -= hisExpMestMaterial.Sum(s => s.AMOUNT);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ProcessMaterialBefore(List<V_HIS_EXP_MEST_MATERIAL> hisExpMestMaterial)
        {
            try
            {
                foreach (var item in hisExpMestMaterial)
                {
                    Mrs00085RDO rdo = new Mrs00085RDO();
                    if (!dicMaterial.ContainsKey(item.MATERIAL_ID ?? 0))
                    {
                        dicMaterial[item.MATERIAL_ID ?? 0] = new Mrs00085RDO();
                        dicMaterial[item.MATERIAL_ID ?? 0].MATERIAL_ID = item.ID;
                        dicMaterial[item.MATERIAL_ID ?? 0].EXP_MEST_TYPE_NAME = "Tồn cuối kì";
                        dicMaterial[item.MATERIAL_ID ?? 0].EXECUTE_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._ReportFilter.TIME_TO);
                        dicMaterial[item.MATERIAL_ID ?? 0].PACKAGE_NUMBER = item.PACKAGE_NUMBER;
                        dicMaterial[item.MATERIAL_ID ?? 0].EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.EXPIRED_DATE ?? 0);
                        dicMaterial[item.MATERIAL_ID ?? 0].BEGIN_AMOUNT = -item.AMOUNT;
                        dicMaterial[item.MATERIAL_ID ?? 0].END_AMOUNT = -item.AMOUNT;
                        dicMaterial[item.MATERIAL_ID ?? 0].IMP_PRICE = item.IMP_PRICE;
                        dicMaterial[item.MATERIAL_ID ?? 0].IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                    }
                    else
                    {
                        dicMaterial[item.MATERIAL_ID ?? 0].BEGIN_AMOUNT -= item.AMOUNT;
                        dicMaterial[item.MATERIAL_ID ?? 0].END_AMOUNT -= item.AMOUNT;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessMaterialBefore(List<V_HIS_IMP_MEST_MATERIAL> hisImpMestMaterial)
        {
            try
            {
                foreach (var item in hisImpMestMaterial)
                {
                    Mrs00085RDO rdo = new Mrs00085RDO();
                    if (!dicMaterial.ContainsKey(item.MATERIAL_ID))
                    {
                        dicMaterial[item.MATERIAL_ID] = new Mrs00085RDO();
                        dicMaterial[item.MATERIAL_ID].MATERIAL_ID = item.ID;
                        dicMaterial[item.MATERIAL_ID].EXP_MEST_TYPE_NAME = "Tồn cuối kì";
                        dicMaterial[item.MATERIAL_ID].EXECUTE_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._ReportFilter.TIME_TO);
                        dicMaterial[item.MATERIAL_ID].PACKAGE_NUMBER = item.PACKAGE_NUMBER;
                        dicMaterial[item.MATERIAL_ID].EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.EXPIRED_DATE ?? 0);
                        dicMaterial[item.MATERIAL_ID].BEGIN_AMOUNT = item.AMOUNT;
                        dicMaterial[item.MATERIAL_ID].END_AMOUNT = item.AMOUNT;
                        dicMaterial[item.MATERIAL_ID].IMP_PRICE = item.IMP_PRICE;
                        dicMaterial[item.MATERIAL_ID].IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                        
                    }
                    else
                    {
                        dicMaterial[item.MATERIAL_ID].BEGIN_AMOUNT += item.AMOUNT;
                        dicMaterial[item.MATERIAL_ID].END_AMOUNT += item.AMOUNT;
                    }
                    if (item.CHMS_TYPE_ID == 2)
                    {

                        dicMaterial[item.MATERIAL_ID].IMP_AMOUNT_HOAN = item.AMOUNT;
                    }
                    else
                    {
                        dicMaterial[item.MATERIAL_ID].IMP_AMOUNT_KHONG_GOM_HOAN = item.AMOUNT;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessMaterialBefore(List<V_HIS_MEST_PERIOD_MATE> hisMestPeriodMedi)
        {
            try
            {
                foreach (var item in hisMestPeriodMedi)
                {
                    Mrs00085RDO rdo = new Mrs00085RDO();
                    if (!dicMaterial.ContainsKey(item.MATERIAL_ID))
                    {
                        dicMaterial[item.MATERIAL_ID] = new Mrs00085RDO();
                        dicMaterial[item.MATERIAL_ID].MATERIAL_ID = item.ID;
                        dicMaterial[item.MATERIAL_ID].EXP_MEST_TYPE_NAME = "Tồn cuối kì";
                        dicMaterial[item.MATERIAL_ID].EXECUTE_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._ReportFilter.TIME_TO);
                        dicMaterial[item.MATERIAL_ID].PACKAGE_NUMBER = item.PACKAGE_NUMBER;
                        dicMaterial[item.MATERIAL_ID].EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.EXPIRED_DATE ?? 0);
                        dicMaterial[item.MATERIAL_ID].BEGIN_AMOUNT = item.AMOUNT;
                        dicMaterial[item.MATERIAL_ID].END_AMOUNT = item.AMOUNT;
                        dicMaterial[item.MATERIAL_ID].IMP_PRICE = item.IMP_PRICE;
                        dicMaterial[item.MATERIAL_ID].IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                    }
                    else
                    {
                        dicMaterial[item.MATERIAL_ID].BEGIN_AMOUNT += item.AMOUNT;
                        dicMaterial[item.MATERIAL_ID].END_AMOUNT += item.AMOUNT;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Tính số lượng đầu kỳ không có kỳ dữ liệu gần nhất
        private void ProcessBeinAmountMaterialNotMediStockPriod(CommonParam paramGet)
        {
            try
            {

                HisImpMestMaterialViewFilter impMateFilter = new HisImpMestMaterialViewFilter();
                impMateFilter.IMP_TIME_TO = this._ReportFilter.TIME_FROM - 1;
                impMateFilter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
                impMateFilter.MATERIAL_TYPE_ID = this._ReportFilter.MATERIAL_TYPE_ID;
                impMateFilter.MATERIAL_ID = this._ReportFilter.MediMateId;
                impMateFilter.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT;
                List<V_HIS_IMP_MEST_MATERIAL> hisImpMestMaterial = new BackendAdapter(null).Get<List<V_HIS_IMP_MEST_MATERIAL>>("api/HisImpMestMaterial/GetView", ApiConsumers.MosConsumer, impMateFilter, SessionManager.ActionLostToken, null);
                if (hisImpMestMaterial != null && hisImpMestMaterial.Count > 0)
                {
                    startBeginAmount = hisImpMestMaterial.Sum(s => s.AMOUNT);
                }
                ProcessMaterialBefore(hisImpMestMaterial);
                HisExpMestMaterialViewFilter expMateFilter = new HisExpMestMaterialViewFilter();
                expMateFilter.EXP_TIME_TO = this._ReportFilter.TIME_FROM - 1;
                expMateFilter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
                expMateFilter.MATERIAL_TYPE_ID = this._ReportFilter.MATERIAL_TYPE_ID;
                expMateFilter.MATERIAL_ID = this._ReportFilter.MediMateId;
                expMateFilter.IS_EXPORT = true;
                List<V_HIS_EXP_MEST_MATERIAL> hisExpMestMaterial = new BackendAdapter(null).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, expMateFilter, SessionManager.ActionLostToken, null);
                if (hisExpMestMaterial != null && hisExpMestMaterial.Count > 0)
                {
                    startBeginAmount -= hisExpMestMaterial.Sum(s => s.AMOUNT);
                }
                ProcessMaterialBefore(hisExpMestMaterial);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
