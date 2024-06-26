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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.Processor.Mps000218.PDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MediStockSummary.CreateReport
{
    public class Mrs00075
    {
        List<Mrs00075RDO> ListRdo = new List<Mrs00075RDO>();
        Dictionary<long, Mrs00075RDO> dicMedicine = new Dictionary<long, Mrs00075RDO>();
        decimal startBeginAmount;

        internal FilterADO _ReportFilter { get; set; }
        long RoomId;
        List<string> lstPackageNumber = new List<string>();
        List<long?> lstExpiredDate = new List<long?>();

        public Mrs00075(long roomId)
        {
            this.RoomId = roomId;
        }

        public void LoadDataPrint218(FilterADO _FilterADO, string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                this._ReportFilter = _FilterADO;

                GetData();

                ProcessData();

                MPS.Processor.Mps000218.PDO.SingKey218 _SingKey218 = new MPS.Processor.Mps000218.PDO.SingKey218();

                if (this._ReportFilter.TIME_FROM > 0)
                {
                    _SingKey218.TIME_TO_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this._ReportFilter.TIME_FROM);
                }
                if (this._ReportFilter.TIME_TO > 0)
                {
                    _SingKey218.TIME_FROM_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this._ReportFilter.TIME_TO);
                }

                V_HIS_MEDICINE_TYPE medicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == this._ReportFilter.MEDICINE_TYPE_ID);
                if (medicineType != null)
                {
                    _SingKey218.MEDICINE_TYPE_CODE = medicineType.MEDICINE_TYPE_CODE;
                    _SingKey218.MEDICINE_TYPE_NAME = medicineType.MEDICINE_TYPE_NAME;
                    _SingKey218.SERVICE_UNIT_NAME = medicineType.SERVICE_UNIT_NAME;
                    _SingKey218.NATIONAL_NAME = medicineType.NATIONAL_NAME;
                    _SingKey218.MANUFACTURER_NAME = medicineType.MANUFACTURER_NAME;
                    _SingKey218.DIC_OTHER_KEY = SetOtherKey(_SingKey218.DIC_OTHER_KEY, medicineType, medicineType.GetType().Name);
                }

                HIS_MEDI_STOCK mediStock = BackendDataWorker.Get<HIS_MEDI_STOCK>().FirstOrDefault(p => p.ID == this._ReportFilter.MEDI_STOCK_ID);
                if (mediStock != null)
                {
                    _SingKey218.MEDI_STOCK_CODE = mediStock.MEDI_STOCK_CODE;
                    _SingKey218.MEDI_STOCK_NAME = mediStock.MEDI_STOCK_NAME;
                }

                if (ListRdo != null && ListRdo.Count > 0)
                {
                    _SingKey218.MEDI_BEGIN_AMOUNT = ListRdo.First().BEGIN_AMOUNT.ToString();
                    _SingKey218.MEDI_END_AMOUNT = ListRdo.Last().END_AMOUNT.ToString();
                }

                List<Mrs00075RDO> listMedicine = new List<Mrs00075RDO>();
                //if (ListRdo.Count == 0)
                {
                    listMedicine = dicMedicine.Values.ToList();
                }

                MPS.Processor.Mps000218.PDO.Mps000218PDO mps000218RDO = new MPS.Processor.Mps000218.PDO.Mps000218PDO(
                    ListRdo,
                    listMedicine,
                    _SingKey218
               );
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000218RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000218RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
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
            bool result = true;
            try
            {
                CommonParam param = new CommonParam();

                HisImpMestMedicineViewFilter impMediFilter = new HisImpMestMedicineViewFilter();
                impMediFilter.IMP_TIME_FROM = this._ReportFilter.TIME_FROM;
                impMediFilter.IMP_TIME_TO = this._ReportFilter.TIME_TO;
                impMediFilter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
                impMediFilter.MEDICINE_TYPE_ID = this._ReportFilter.MEDICINE_TYPE_ID;
                impMediFilter.MEDICINE_ID = this._ReportFilter.MediMateId;
                impMediFilter.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT;
                List<V_HIS_IMP_MEST_MEDICINE> hisImpMestMedicine = new BackendAdapter(null).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, impMediFilter, SessionManager.ActionLostToken, null);

                HisExpMestMedicineViewFilter expMediFilter = new HisExpMestMedicineViewFilter();
                expMediFilter.EXP_TIME_FROM = this._ReportFilter.TIME_FROM;
                expMediFilter.EXP_TIME_TO = this._ReportFilter.TIME_TO;
                expMediFilter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
                expMediFilter.MEDICINE_TYPE_ID = this._ReportFilter.MEDICINE_TYPE_ID;
                expMediFilter.MEDICINE_ID = this._ReportFilter.MediMateId;
                expMediFilter.IS_EXPORT = true;
                List<V_HIS_EXP_MEST_MEDICINE> hisExpMestMedicine = new BackendAdapter(null).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, expMediFilter, SessionManager.ActionLostToken, null);

                if (param.HasException)
                {
                    throw new Exception("Co exception xay ra tai DAOGET trong qua trinh lay du lieu MRS00075");
                }

                if (hisImpMestMedicine != null && hisImpMestMedicine.Count > 0)
                {
                    ListRdo.AddRange((from r in hisImpMestMedicine select new Mrs00075RDO(r)).ToList());
                }
                if (hisExpMestMedicine != null && hisExpMestMedicine.Count > 0)
                {
                    ListRdo.AddRange((from r in hisExpMestMedicine select new Mrs00075RDO(r)).ToList());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private bool ProcessData()
        {
            bool result = true;
            try
            {
                ProcessBeginAndEndAmount();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
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
                    ListRdo = ListRdo.GroupBy(g => g.EXECUTE_DATE_STR).Select(s => new Mrs00075RDO
                    {
                        EXECUTE_DATE_STR = s.First().EXECUTE_DATE_STR,
                        EXP_AMOUNT = s.Sum(s1 => s1.EXP_AMOUNT),
                        IMP_AMOUNT = s.Sum(s2 => s2.IMP_AMOUNT),
                        EXPIRED_DATE_STR = (s.FirstOrDefault(s3 => !String.IsNullOrWhiteSpace(s3.EXPIRED_DATE_STR)) ?? new Mrs00075RDO()).EXPIRED_DATE_STR,
                        PACKAGE_NUMBER = string.Join(",", s.Select(o => o.PACKAGE_NUMBER).Distinct()),
                        IMP_AMOUNT_HOAN = s.First().CHMS_TYPE_ID == 2 ? s.Sum(q => q.IMP_AMOUNT) : 0,
                        IMP_AMOUNT_KHONG_GOM_HOAN = s.First().CHMS_TYPE_ID != 2 ? s.Sum(q => q.IMP_AMOUNT) : 0,
                        DOCUMENT_NUMBER = s.First().DOCUMENT_NUMBER
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
                lstExpiredDate = new List<long?>();
                lstPackageNumber = new List<string>();
                HisMestPeriodMediViewFilter periodMediFilter = new HisMestPeriodMediViewFilter();
                periodMediFilter.MEDI_STOCK_PERIOD_ID = neighborPeriod.ID;
                periodMediFilter.MEDICINE_TYPE_ID = this._ReportFilter.MEDICINE_TYPE_ID;
                periodMediFilter.MEDICINE_ID = this._ReportFilter.MediMateId;
                List<V_HIS_MEST_PERIOD_MEDI> hisMestPeriodMedi = new BackendAdapter(null).Get<List<V_HIS_MEST_PERIOD_MEDI>>("api/HisMestPeriodMedi/GetView", ApiConsumers.MosConsumer, periodMediFilter, SessionManager.ActionLostToken, null);
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
                expMediFilter.EXP_TIME_FROM = neighborPeriod.TO_TIME;
                expMediFilter.EXP_TIME_TO = this._ReportFilter.TIME_FROM - 1;
                expMediFilter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
                expMediFilter.MEDICINE_ID = this._ReportFilter.MediMateId;
                expMediFilter.MEDICINE_TYPE_ID = this._ReportFilter.MEDICINE_TYPE_ID;
                expMediFilter.IS_EXPORT = true;
                List<V_HIS_EXP_MEST_MEDICINE> hisExpMestMedicine = new BackendAdapter(null).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, expMediFilter, SessionManager.ActionLostToken, null);
                if (hisExpMestMedicine != null && hisExpMestMedicine.Count > 0)
                {
                    startBeginAmount -= hisExpMestMedicine.Sum(s => s.AMOUNT);

                    if (hisImpMestMedicine != null && hisImpMestMedicine.Count > 0)
                    {
                        var ExpMestMedicineG = hisExpMestMedicine.GroupBy(o => o.PACKAGE_NUMBER);
                        foreach (var item in ExpMestMedicineG)
                        {
                            decimal SumExpMest = item.Sum(o => o.AMOUNT);
                            decimal SumImpMest = hisImpMestMedicine.Where(o => o.PACKAGE_NUMBER == item.FirstOrDefault().PACKAGE_NUMBER).Sum(p => p.AMOUNT);

                            if (SumImpMest == SumExpMest)
                            {
                                lstPackageNumber.Add(item.FirstOrDefault().PACKAGE_NUMBER);
                                lstExpiredDate.Add(item.FirstOrDefault().EXPIRED_DATE);
                            }
                        }
                    }
                }
                ProcessMedicineBefore(hisExpMestMedicine);
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
                    Mrs00075RDO rdo = new Mrs00075RDO();
                    if (!dicMedicine.ContainsKey(item.MEDICINE_TYPE_ID))
                    {
                        dicMedicine[item.MEDICINE_TYPE_ID] = new Mrs00075RDO();
                        dicMedicine[item.MEDICINE_TYPE_ID].EXECUTE_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._ReportFilter.TIME_TO);
                        dicMedicine[item.MEDICINE_TYPE_ID].BEGIN_AMOUNT = item.VIR_END_AMOUNT ?? 0;
                        dicMedicine[item.MEDICINE_TYPE_ID].END_AMOUNT = item.VIR_END_AMOUNT ?? 0;
                    }
                    else
                    {
                        dicMedicine[item.MEDICINE_TYPE_ID].BEGIN_AMOUNT += item.VIR_END_AMOUNT ?? 0;
                        dicMedicine[item.MEDICINE_TYPE_ID].END_AMOUNT += item.VIR_END_AMOUNT ?? 0;
                    }
                    if (!String.IsNullOrWhiteSpace(item.PACKAGE_NUMBER))
                    {
                        if (!String.IsNullOrWhiteSpace(dicMedicine[item.MEDICINE_TYPE_ID].PACKAGE_NUMBER))
                        {
                            List<string> package = dicMedicine[item.MEDICINE_TYPE_ID].PACKAGE_NUMBER.Split(',').ToList();
                            if (!package.Contains(item.PACKAGE_NUMBER))
                            {
                                package.Add(item.PACKAGE_NUMBER);
                            }

                            dicMedicine[item.MEDICINE_TYPE_ID].PACKAGE_NUMBER = string.Join(",", package.Distinct());
                        }
                        else
                        {
                            dicMedicine[item.MEDICINE_TYPE_ID].PACKAGE_NUMBER = item.PACKAGE_NUMBER;
                        }
                    }

                    if (item.EXPIRED_DATE.HasValue)
                    {
                        if (!String.IsNullOrWhiteSpace(dicMedicine[item.MEDICINE_TYPE_ID].EXPIRED_DATE_STR))
                        {
                            List<string> expDate = dicMedicine[item.MEDICINE_TYPE_ID].EXPIRED_DATE_STR.Split(',').ToList();
                            string date = Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.EXPIRED_DATE ?? 0);
                            if (!expDate.Contains(date))
                            {
                                expDate.Add(date);
                            }

                            dicMedicine[item.MEDICINE_TYPE_ID].EXPIRED_DATE_STR = string.Join(",", expDate.Distinct());
                        }
                        else
                        {
                            dicMedicine[item.MEDICINE_TYPE_ID].EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.EXPIRED_DATE ?? 0);
                        }
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
                    Mrs00075RDO rdo = new Mrs00075RDO();
                    if (!dicMedicine.ContainsKey(item.MEDICINE_TYPE_ID))
                    {
                        dicMedicine[item.MEDICINE_TYPE_ID] = new Mrs00075RDO();
                        dicMedicine[item.MEDICINE_TYPE_ID].EXECUTE_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._ReportFilter.TIME_TO);
                        dicMedicine[item.MEDICINE_TYPE_ID].BEGIN_AMOUNT = item.AMOUNT;
                        dicMedicine[item.MEDICINE_TYPE_ID].END_AMOUNT = item.AMOUNT;


                    }
                    else
                    {
                        dicMedicine[item.MEDICINE_TYPE_ID].BEGIN_AMOUNT += item.AMOUNT;
                        dicMedicine[item.MEDICINE_TYPE_ID].END_AMOUNT += item.AMOUNT;
                    }
                    dicMedicine[item.MEDICINE_TYPE_ID].DOCUMENT_NUMBER = item.DOCUMENT_NUMBER;
                    if (item.CHMS_TYPE_ID == 2)
                    {

                        dicMedicine[item.MEDICINE_TYPE_ID].IMP_AMOUNT_HOAN = item.AMOUNT;
                    }
                    else
                    {
                        dicMedicine[item.MEDICINE_TYPE_ID].IMP_AMOUNT_KHONG_GOM_HOAN = item.AMOUNT;
                    }
                    if (item.AMOUNT > 0)
                    {
                        if (!String.IsNullOrWhiteSpace(item.PACKAGE_NUMBER))
                        {
                            if (!String.IsNullOrWhiteSpace(dicMedicine[item.MEDICINE_TYPE_ID].PACKAGE_NUMBER))
                            {
                                List<string> package = dicMedicine[item.MEDICINE_TYPE_ID].PACKAGE_NUMBER.Split(',').ToList();
                                if (!package.Contains(item.PACKAGE_NUMBER))
                                {
                                    package.Add(item.PACKAGE_NUMBER);
                                }

                                dicMedicine[item.MEDICINE_TYPE_ID].PACKAGE_NUMBER = string.Join(",", package.Distinct());
                            }
                            else
                            {
                                dicMedicine[item.MEDICINE_TYPE_ID].PACKAGE_NUMBER = item.PACKAGE_NUMBER;
                            }
                        }

                        if (item.EXPIRED_DATE.HasValue)
                        {
                            if (!String.IsNullOrWhiteSpace(dicMedicine[item.MEDICINE_TYPE_ID].EXPIRED_DATE_STR))
                            {
                                List<string> expDate = dicMedicine[item.MEDICINE_TYPE_ID].EXPIRED_DATE_STR.Split(',').ToList();
                                string date = Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.EXPIRED_DATE ?? 0);
                                if (!expDate.Contains(date))
                                {
                                    expDate.Add(date);
                                }

                                dicMedicine[item.MEDICINE_TYPE_ID].EXPIRED_DATE_STR = string.Join(",", expDate.Distinct());
                            }
                            else
                            {
                                dicMedicine[item.MEDICINE_TYPE_ID].EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.EXPIRED_DATE ?? 0);
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

        private void ProcessMedicineBefore(List<V_HIS_EXP_MEST_MEDICINE> hisExpMestMedicine)
        {
            try
            {
                foreach (var item in hisExpMestMedicine)
                {
                    Mrs00075RDO rdo = new Mrs00075RDO();
                    if (!dicMedicine.ContainsKey(item.MEDICINE_TYPE_ID))
                    {
                        dicMedicine[item.MEDICINE_TYPE_ID] = new Mrs00075RDO();
                        dicMedicine[item.MEDICINE_TYPE_ID].EXECUTE_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._ReportFilter.TIME_TO);
                        dicMedicine[item.MEDICINE_TYPE_ID].BEGIN_AMOUNT = -item.AMOUNT;
                        dicMedicine[item.MEDICINE_TYPE_ID].END_AMOUNT = -item.AMOUNT;
                    }
                    else
                    {
                        dicMedicine[item.MEDICINE_TYPE_ID].BEGIN_AMOUNT -= item.AMOUNT;
                        dicMedicine[item.MEDICINE_TYPE_ID].END_AMOUNT -= item.AMOUNT;
                    }

                    if (!String.IsNullOrWhiteSpace(item.PACKAGE_NUMBER))
                    {
                        if (!String.IsNullOrWhiteSpace(dicMedicine[item.MEDICINE_TYPE_ID].PACKAGE_NUMBER))
                        {

                            List<string> package = dicMedicine[item.MEDICINE_TYPE_ID].PACKAGE_NUMBER.Split(',').ToList();
                            if (!package.Contains(item.PACKAGE_NUMBER))
                            {
                                package.Add(item.PACKAGE_NUMBER);
                            }

                            bool chkPackageNumber = (lstPackageNumber != null && lstPackageNumber.Count > 0) ? lstPackageNumber.Contains(item.PACKAGE_NUMBER) : false;
                            if (chkPackageNumber)
                            {
                                package.Remove(item.PACKAGE_NUMBER);
                            }

                            dicMedicine[item.MEDICINE_TYPE_ID].PACKAGE_NUMBER = string.Join(",", package.Distinct());
                        }
                        else
                        {
                            dicMedicine[item.MEDICINE_TYPE_ID].PACKAGE_NUMBER = item.PACKAGE_NUMBER;
                        }
                    }

                    if (item.EXPIRED_DATE.HasValue)
                    {
                        if (!String.IsNullOrWhiteSpace(dicMedicine[item.MEDICINE_TYPE_ID].EXPIRED_DATE_STR))
                        {
                            List<string> expDate = dicMedicine[item.MEDICINE_TYPE_ID].EXPIRED_DATE_STR.Split(',').ToList();
                            string date = Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.EXPIRED_DATE ?? 0);
                            if (!expDate.Contains(date))
                            {
                                expDate.Add(date);
                            }

                            bool chkExpiredDate = (lstExpiredDate != null && lstExpiredDate.Count > 0) ? lstExpiredDate.Contains(item.EXPIRED_DATE) : false;
                            if (chkExpiredDate)
                            {
                                expDate.Remove(date);
                            }

                            dicMedicine[item.MEDICINE_TYPE_ID].EXPIRED_DATE_STR = string.Join(",", expDate.Distinct());
                        }
                        else
                        {
                            dicMedicine[item.MEDICINE_TYPE_ID].EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.EXPIRED_DATE ?? 0);
                        }
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
                lstPackageNumber = new List<string>();
                lstExpiredDate = new List<long?>();

                HisImpMestMedicineViewFilter impMediFilter = new HisImpMestMedicineViewFilter();
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
                expMediFilter.EXP_TIME_TO = this._ReportFilter.TIME_FROM - 1;
                expMediFilter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
                expMediFilter.MEDICINE_TYPE_ID = this._ReportFilter.MEDICINE_TYPE_ID;
                expMediFilter.MEDICINE_ID = this._ReportFilter.MediMateId;
                expMediFilter.IS_EXPORT = true;
                List<V_HIS_EXP_MEST_MEDICINE> hisExpMestMedicine = new BackendAdapter(null).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, expMediFilter, SessionManager.ActionLostToken, null);
                if (hisExpMestMedicine != null && hisExpMestMedicine.Count > 0)
                {
                    startBeginAmount -= hisExpMestMedicine.Sum(s => s.AMOUNT);

                    if (hisImpMestMedicine != null && hisImpMestMedicine.Count > 0)
                    {
                        var ExpMestMedicineG = hisExpMestMedicine.GroupBy(o => o.PACKAGE_NUMBER);
                        foreach (var item in ExpMestMedicineG)
                        {
                            decimal SumExpMest = item.Sum(o => o.AMOUNT);
                            decimal SumImpMest = hisImpMestMedicine.Where(o => o.PACKAGE_NUMBER == item.FirstOrDefault().PACKAGE_NUMBER).Sum(p => p.AMOUNT);

                            if (SumImpMest == SumExpMest)
                            {
                                lstPackageNumber.Add(item.FirstOrDefault().PACKAGE_NUMBER);
                                lstExpiredDate.Add(item.FirstOrDefault().EXPIRED_DATE);
                            }
                        }
                    }
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
