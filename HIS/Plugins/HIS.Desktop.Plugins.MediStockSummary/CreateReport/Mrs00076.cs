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
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.Processor.Mps000219.PDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MediStockSummary.CreateReport
{
    public class Mrs00076
    {
        decimal startBeginAmount;
        CommonParam paramGet = new CommonParam();
        List<Mrs00076RDO> ListRdo = new List<Mrs00076RDO>();
        List<V_HIS_EXP_MEST_MATERIAL> listExpMestMaterialView = new List<V_HIS_EXP_MEST_MATERIAL>();
        List<V_HIS_IMP_MEST_MATERIAL> hisImpMestMaterial = new List<V_HIS_IMP_MEST_MATERIAL>();
        List<V_HIS_EXP_MEST_MATERIAL> hisExpMestMaterial = new List<V_HIS_EXP_MEST_MATERIAL>();
        HIS_DEPARTMENT department = new HIS_DEPARTMENT();
        HIS_MEDI_STOCK mediStock = new HIS_MEDI_STOCK();
        List<V_HIS_EXP_MEST> listHisExpMest = new List<V_HIS_EXP_MEST>();
        Dictionary<long, Mrs00076RDO> dicMaterial = new Dictionary<long, Mrs00076RDO>();

        internal FilterADO _ReportFilter { get; set; }
        long RoomId;

        public Mrs00076(long roomId)
        {
            this.RoomId = roomId;
        }

        public void LoadDataPrint219(FilterADO _FilterADO, string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                this._ReportFilter = _FilterADO;

                GetData();

                ProcessData();

                MPS.Processor.Mps000219.PDO.SingKey219 _SingKey219 = new MPS.Processor.Mps000219.PDO.SingKey219();

                if (this._ReportFilter.TIME_FROM > 0)
                {
                    _SingKey219.TIME_TO_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this._ReportFilter.TIME_FROM);
                }
                if (this._ReportFilter.TIME_TO > 0)
                {
                    _SingKey219.TIME_FROM_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this._ReportFilter.TIME_TO);
                }
                V_HIS_MATERIAL_TYPE medicineType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == this._ReportFilter.MATERIAL_TYPE_ID);
                if (medicineType != null)
                {
                    _SingKey219.MATERIAL_TYPE_CODE = medicineType.MATERIAL_TYPE_CODE;
                    _SingKey219.MATERIAL_TYPE_NAME = medicineType.MATERIAL_TYPE_NAME;
                    _SingKey219.SERVICE_UNIT_NAME = medicineType.SERVICE_UNIT_NAME;
                    _SingKey219.NATIONAL_NAME = medicineType.NATIONAL_NAME;
                    _SingKey219.MANUFACTURER_NAME = medicineType.MANUFACTURER_NAME;
                    _SingKey219.DIC_OTHER_KEY = SetOtherKey(_SingKey219.DIC_OTHER_KEY, medicineType, medicineType.GetType().Name);
                }

                HIS_MEDI_STOCK mediStock = BackendDataWorker.Get<HIS_MEDI_STOCK>().FirstOrDefault(p => p.ID == this._ReportFilter.MEDI_STOCK_ID);
                if (mediStock != null)
                {
                    _SingKey219.MEDI_STOCK_CODE = mediStock.MEDI_STOCK_CODE;
                    _SingKey219.MEDI_STOCK_NAME = mediStock.MEDI_STOCK_NAME;
                }

                if (ListRdo != null && ListRdo.Count > 0)
                {
                    _SingKey219.MEDI_BEGIN_AMOUNT = ListRdo.First().BEGIN_AMOUNT.ToString();
                    _SingKey219.MEDI_END_AMOUNT = ListRdo.Last().END_AMOUNT.ToString();
                }


                var listMedicine = new List<Mrs00076RDO>();
                //if (ListRdo.Count == 0)
                {
                    listMedicine = dicMaterial.Values.ToList();
                }
                Inventec.Common.Logging.LogSystem.Debug("LISTMATERIAL:__MPS219" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listMedicine), listMedicine));
              
                MPS.Processor.Mps000219.PDO.Mps000219PDO mps000219RDO = new MPS.Processor.Mps000219.PDO.Mps000219PDO(
                    ListRdo,
                    listMedicine,
                    _SingKey219
               );
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000219RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000219RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
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
                HisImpMestMaterialViewFilter impMateFilter = new HisImpMestMaterialViewFilter();
                impMateFilter.IMP_TIME_FROM = this._ReportFilter.TIME_FROM;
                impMateFilter.IMP_TIME_TO = this._ReportFilter.TIME_TO;
                impMateFilter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
                impMateFilter.MATERIAL_TYPE_ID = this._ReportFilter.MATERIAL_TYPE_ID;
                impMateFilter.MATERIAL_ID = this._ReportFilter.MediMateId;
                impMateFilter.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT;
                hisImpMestMaterial = new BackendAdapter(null).Get<List<V_HIS_IMP_MEST_MATERIAL>>("api/HisImpMestMaterial/GetView", ApiConsumers.MosConsumer, impMateFilter, SessionManager.ActionLostToken, null);

                HisExpMestMaterialViewFilter expMateFilter = new HisExpMestMaterialViewFilter();
                expMateFilter.EXP_TIME_FROM = this._ReportFilter.TIME_FROM;
                expMateFilter.EXP_TIME_TO = this._ReportFilter.TIME_TO;
                expMateFilter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
                expMateFilter.MATERIAL_TYPE_ID = this._ReportFilter.MATERIAL_TYPE_ID;
                expMateFilter.MATERIAL_ID = this._ReportFilter.MediMateId;
                expMateFilter.IS_EXPORT = true;
                hisExpMestMaterial = new BackendAdapter(null).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, expMateFilter, SessionManager.ActionLostToken, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);

                result = false;
            }
            return result;
        }

        private bool ProcessData()
        {
            bool result = false;
            try
            {
                if (hisImpMestMaterial != null && hisImpMestMaterial.Count > 0)
                {
                    ListRdo.AddRange((from r in hisImpMestMaterial select new Mrs00076RDO(r)).ToList());
                }
                if (hisExpMestMaterial != null && hisExpMestMaterial.Count > 0)
                {
                    ListRdo.AddRange((from r in hisExpMestMaterial select new Mrs00076RDO(r)).ToList());
                }
                ProcessBeginAndEndAmount();
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
                if (ListRdo != null && ListRdo.Count > 0)
                {
                    ListRdo = ListRdo.OrderBy(o => o.EXECUTE_TIME).ToList();
                    ListRdo = ListRdo.GroupBy(g => g.EXECUTE_DATE_STR).Select(s => new Mrs00076RDO
                    {
                        EXECUTE_DATE_STR = s.First().EXECUTE_DATE_STR,
                        EXP_AMOUNT = s.Sum(s1 => s1.EXP_AMOUNT),
                        IMP_AMOUNT = s.Sum(s2 => s2.IMP_AMOUNT),
                        EXPIRED_DATE_STR = (s.FirstOrDefault(s3 => !String.IsNullOrWhiteSpace(s3.EXPIRED_DATE_STR)) ?? new Mrs00076RDO()).EXPIRED_DATE_STR,
                        BID_NUMBER = string.Join(",", s.Select(o => o.BID_NUMBER).Distinct()),
                        PACKAGE_NUMBER = string.Join(",", s.Select(o => o.PACKAGE_NUMBER).Distinct()),
                        IMP_AMOUNT_HOAN = s.First().CHMS_TYPE_ID == 2 ? s.Sum(q => q.IMP_AMOUNT):0 ,
                        IMP_AMOUNT_KHONG_GOM_HOAN = s.First().CHMS_TYPE_ID != 2 ? s.Sum(q => q.IMP_AMOUNT) : 0
                    }).ToList();
                    decimal previousEndAmount = startBeginAmount;
                    foreach (var rdo in ListRdo)
                    {
                        rdo.CalculateAmount(previousEndAmount);
                        previousEndAmount = rdo.END_AMOUNT;
                    }
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
                List<V_HIS_IMP_MEST_MATERIAL> hisImpMestMedicine = new BackendAdapter(null).Get<List<V_HIS_IMP_MEST_MATERIAL>>("api/HisImpMestMaterial/GetView", ApiConsumers.MosConsumer, impMateFilter, SessionManager.ActionLostToken, null);
                if (hisImpMestMedicine != null && hisImpMestMedicine.Count > 0)
                {
                    startBeginAmount += hisImpMestMedicine.Sum(s => s.AMOUNT);
                }
                ProcessMaterialBefore(hisImpMestMedicine);

                HisExpMestMaterialViewFilter expMateFilter = new HisExpMestMaterialViewFilter();
                expMateFilter.EXP_TIME_FROM = neighborPeriod.TO_TIME;
                expMateFilter.EXP_TIME_TO = this._ReportFilter.TIME_FROM - 1;
                expMateFilter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
                expMateFilter.MATERIAL_TYPE_ID = this._ReportFilter.MATERIAL_TYPE_ID;
                expMateFilter.MATERIAL_ID = this._ReportFilter.MediMateId;
                expMateFilter.IS_EXPORT = true;
                List<V_HIS_EXP_MEST_MATERIAL> hisExpMestMedicine = new BackendAdapter(null).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, expMateFilter, SessionManager.ActionLostToken, null);
                if (hisExpMestMedicine != null && hisExpMestMedicine.Count > 0)
                {
                    startBeginAmount -= hisExpMestMedicine.Sum(s => s.AMOUNT);
                }
                ProcessMaterialBefore(hisExpMestMaterial);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessMaterialBefore(List<V_HIS_MEST_PERIOD_MATE> hisMestPeriodMate)
        {
            try
            {
                foreach (var item in hisMestPeriodMate)
                {
                    Mrs00076RDO rdo = new Mrs00076RDO();
                    if (!dicMaterial.ContainsKey(item.MATERIAL_TYPE_ID))
                    {
                        dicMaterial[item.MATERIAL_TYPE_ID] = new Mrs00076RDO();
                        dicMaterial[item.MATERIAL_TYPE_ID].EXECUTE_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._ReportFilter.TIME_TO);
                        dicMaterial[item.MATERIAL_TYPE_ID].BEGIN_AMOUNT = item.VIR_END_AMOUNT ?? 0;
                        dicMaterial[item.MATERIAL_TYPE_ID].END_AMOUNT = item.VIR_END_AMOUNT ?? 0;
                    }
                    else
                    {
                        dicMaterial[item.MATERIAL_TYPE_ID].BEGIN_AMOUNT += item.VIR_END_AMOUNT ?? 0;
                        dicMaterial[item.MATERIAL_TYPE_ID].END_AMOUNT += item.VIR_END_AMOUNT ?? 0;
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
                    Mrs00076RDO rdo = new Mrs00076RDO();
                    if (!dicMaterial.ContainsKey(item.MATERIAL_TYPE_ID))
                    {
                        dicMaterial[item.MATERIAL_TYPE_ID] = new Mrs00076RDO();
                        dicMaterial[item.MATERIAL_TYPE_ID].EXECUTE_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._ReportFilter.TIME_TO);
                        dicMaterial[item.MATERIAL_TYPE_ID].BEGIN_AMOUNT = item.AMOUNT;
                        dicMaterial[item.MATERIAL_TYPE_ID].END_AMOUNT = item.AMOUNT;
                       
                    }
                    else
                    {
                        dicMaterial[item.MATERIAL_TYPE_ID].BEGIN_AMOUNT += item.AMOUNT;
                        dicMaterial[item.MATERIAL_TYPE_ID].END_AMOUNT += item.AMOUNT;
                    }
                    if (item.CHMS_TYPE_ID == 2)
                    {

                        dicMaterial[item.MATERIAL_TYPE_ID].IMP_AMOUNT_HOAN = item.AMOUNT;
                    }
                    else
                    {
                        dicMaterial[item.MATERIAL_TYPE_ID].IMP_AMOUNT_KHONG_GOM_HOAN = item.AMOUNT;
                    }
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
                    Mrs00076RDO rdo = new Mrs00076RDO();
                    if (!dicMaterial.ContainsKey(item.MATERIAL_TYPE_ID))
                    {
                        dicMaterial[item.MATERIAL_TYPE_ID] = new Mrs00076RDO();
                        dicMaterial[item.MATERIAL_TYPE_ID].EXECUTE_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._ReportFilter.TIME_TO);
                        dicMaterial[item.MATERIAL_TYPE_ID].BEGIN_AMOUNT = -item.AMOUNT;
                        dicMaterial[item.MATERIAL_TYPE_ID].END_AMOUNT = -item.AMOUNT;
                    }
                    else
                    {
                        dicMaterial[item.MATERIAL_TYPE_ID].BEGIN_AMOUNT -= item.AMOUNT;
                        dicMaterial[item.MATERIAL_TYPE_ID].END_AMOUNT -= item.AMOUNT;
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
                List<V_HIS_IMP_MEST_MATERIAL> hisImpMestMedicine = new BackendAdapter(null).Get<List<V_HIS_IMP_MEST_MATERIAL>>("api/HisImpMestMaterial/GetView", ApiConsumers.MosConsumer, impMateFilter, SessionManager.ActionLostToken, null);
                if (hisImpMestMedicine != null && hisImpMestMedicine.Count > 0)
                {
                    startBeginAmount += hisImpMestMedicine.Sum(s => s.AMOUNT);
                }
                this.ProcessMaterialBefore(hisImpMestMedicine);
                HisExpMestMaterialViewFilter expMateFilter = new HisExpMestMaterialViewFilter();
                expMateFilter.EXP_TIME_TO = this._ReportFilter.TIME_FROM - 1;
                expMateFilter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
                expMateFilter.MATERIAL_TYPE_ID = this._ReportFilter.MATERIAL_TYPE_ID;
                expMateFilter.MATERIAL_ID = this._ReportFilter.MediMateId;
                expMateFilter.IS_EXPORT = true;
                List<V_HIS_EXP_MEST_MATERIAL> hisExpMestMedicine = new BackendAdapter(null).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, expMateFilter, SessionManager.ActionLostToken, null);
                if (hisExpMestMedicine != null && hisExpMestMedicine.Count > 0)
                {
                    startBeginAmount -= hisExpMestMedicine.Sum(s => s.AMOUNT);
                }
                ProcessMaterialBefore(hisExpMestMedicine);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
