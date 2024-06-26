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
using FlexCel.Report;
using Inventec.Common.Logging;
using Inventec.Common.QRCoder;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000472.ADO;
using MPS.Processor.Mps000472.PDO;
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace MPS.Processor.Mps000472
{
    public partial class Mps000472Processor : AbstractProcessor
    {
        Mps000472PDO rdo;
        List<MRCheckSummaryADO> ListMRCheckSummary;
        List<HIS_MR_CHECKLIST>  ListMRChecklist;
        List<MRChecklistADO> ListMRChecklistADO;
        public Mps000472Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            if (rdoBase != null && rdoBase is Mps000472PDO)
            {
                rdo = (Mps000472PDO)rdoBase;
            }
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Mps000472PDO rdo", rdo));

                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                SetBarcodeKey();
                SetListData();
                SetSingleKey();

                SetImageKey();

                List<ListMRChecklistADO> ListMRChecklistADOs = new List<ListMRChecklistADO>();

                if (rdo.ListMRChecklist != null)
                {
                    this.ListMRChecklistADO = new List<MRChecklistADO>();
                    foreach (var MRCheckSummary in this.ListMRCheckSummary)
                    {
                        List<HIS_MR_CHECKLIST> listChecklist_OfThis = rdo.ListMRChecklist.Where(o => o.MR_CHECK_SUMMARY_ID == MRCheckSummary.ID).ToList() ?? new List<HIS_MR_CHECKLIST>();
                        
                        foreach (var item in listChecklist_OfThis)
                        {
                            MRChecklistADO ado = new MRChecklistADO(item, MRCheckSummary.Stt);
                            this.ListMRChecklistADO.Add(ado);
                        }
                    }

                    var listMR_CHECK_ITEM_ID = rdo.ListMRChecklist.Select(o => o.MR_CHECK_ITEM_ID).Distinct();
                    foreach (var item in listMR_CHECK_ITEM_ID)
                    {
                        var latestMrChecklist = this.ListMRChecklistADO.Where(o => o.MR_CHECK_ITEM_ID == item)
                                                                    .OrderByDescending(o => o.MODIFY_TIME ?? 0).ToList() ?? new List<MRChecklistADO>();

                        ListMRChecklistADO ado = new ListMRChecklistADO(latestMrChecklist);
                        ListMRChecklistADOs.Add(ado);
                    }
                }

                this.SetSignatureKeyImageByCFG();
                singleTag.ProcessData(store, singleValueDictionary);
                //barCodeTag.ProcessData(store, dicImage);
                rdo.ListMRCheckItemType = rdo.ListMRCheckItemType != null ? rdo.ListMRCheckItemType.OrderBy(o => o.NUM_ORDER).ToList() : null;
                rdo.ListMRCheckItem = rdo.ListMRCheckItem != null ? rdo.ListMRCheckItem.OrderBy(o => o.NUM_ORDER).ToList() : null;

                objectTag.AddObjectData(store, "ListMRCheckSummary", this.ListMRCheckSummary ?? new List<MRCheckSummaryADO>());
                objectTag.AddObjectData(store, "ListMRCheckItemType", rdo.ListMRCheckItemType ?? new List<HIS_MR_CHECK_ITEM_TYPE>());
                //objectTag.AddObjectData(store, "ListMRChecklist", this.ListMRChecklist ?? new List<HIS_MR_CHECKLIST>());
                objectTag.AddObjectData(store, "ListMRCheckItem", rdo.ListMRCheckItem ?? new List<HIS_MR_CHECK_ITEM>());
                objectTag.AddObjectData(store, "ListMRCheckItemA", ListMRChecklistADOs);

                //objectTag.AddObjectData<ListMRChecklistADO>(this.store, "ListMRCheckItemA", ListMRChecklistADOs);

                objectTag.AddRelationship(store, "ListMRCheckItemType", "ListMRCheckItem", "ID", "CHECK_ITEM_TYPE_ID");
                //objectTag.AddRelationship(store, "ListMRCheckItem", "ListMRChecklist", "ID", "MR_CHECK_ITEM_ID");
                objectTag.AddRelationship(store, "ListMRCheckItem", "ListMRCheckItemA", "ID", "MR_CHECK_ITEM_ID");

                objectTag.SetUserFunction(this.store, "FlFuncElement", new Mps000472Processor.FlFuncElementFunction());

                //SetMRChecklist(ref objectTag);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void SetMRChecklist(ref Inventec.Common.FlexCellExport.ProcessObjectTag objectTag)
        {
            try
            {
                if (rdo.ListMRChecklist != null)
                {
                    List<HIS_MR_CHECKLIST>  ListMRChecklist_forAdd = new List<HIS_MR_CHECKLIST>();
                    if (this.ListMRCheckSummary != null)
                    {
                        foreach (var item in this.ListMRCheckSummary)
                        {
                            if (item.CHECK_PLACE == 2)
                            {
                                ListMRChecklist_forAdd = rdo.ListMRChecklist.Where(o => o.MR_CHECK_SUMMARY_ID == item.ID).ToList();
                                objectTag.AddObjectData(store, "ListMRChecklist"+item.Stt, ListMRChecklist_forAdd ?? new List<HIS_MR_CHECKLIST>());
                                objectTag.AddRelationship(store, "ListMRCheckItem", "ListMRChecklist"+item.Stt, "ID", "MR_CHECK_ITEM_ID");
                            }
                        }
                        var listSttNotAdd = this.ListMRCheckSummary.Where(o => o.CHECK_PLACE != 2).Select(o => o.Stt).ToList();
                        foreach (var stt in listSttNotAdd)
                        {
                            objectTag.AddObjectData(store, "ListMRChecklist" + stt, new List<HIS_MR_CHECKLIST>());
                            objectTag.AddRelationship(store, "ListMRCheckItem", "ListMRChecklist" + stt, "ID", "MR_CHECK_ITEM_ID");
                        }
                    }
                }
                //Add empty list
                var countList = this.ListMRCheckSummary != null ? this.ListMRCheckSummary.Count() : 0;
                for (int i = countList; i < 5; i++)
                {
                    objectTag.AddObjectData(store, "ListMRChecklist" + i, new List<HIS_MR_CHECKLIST>());
                    objectTag.AddRelationship(store, "ListMRCheckItem", "ListMRChecklist" + i, "ID", "MR_CHECK_ITEM_ID");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetListData()
        {
            try
            {
                if (rdo.ListMRCheckSummary != null)
                {
                    List<HIS_MR_CHECK_SUMMARY> ListMRCheckSummary_WithOrder;
                    ListMRCheckSummary_WithOrder = rdo.ListMRCheckSummary.Where(o => o.IS_APPROVED != 1)
                                                            .OrderBy(o => o.MEDI_RECORD_SUBMIT_DATE.HasValue)
                                                            .OrderBy(o => o.MEDI_RECORD_SUBMIT_DATE).ToList() ?? new List<HIS_MR_CHECK_SUMMARY>();
                    ListMRCheckSummary_WithOrder.AddRange(rdo.ListMRCheckSummary.Where(o => o.IS_APPROVED == 1).ToList() ?? new List<HIS_MR_CHECK_SUMMARY>());

                    this.ListMRCheckSummary = new List<MRCheckSummaryADO>();
                    long stt = 1;
                    foreach (var item in ListMRCheckSummary_WithOrder)
                    {
                        MRCheckSummaryADO newAdo = new MRCheckSummaryADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MRCheckSummaryADO>(newAdo, item);
                        if (item.IS_APPROVED == 1)
                        {
                            //Đã sắp xếp xuống cuối danh sách
                            newAdo.Stt = stt;
                            newAdo.PhanLoaiBenhAn = "Bệnh án đạt yêu cầu: chuyển lưu trữ";
                            
                        }
                        else
                        {
                            newAdo.Stt = stt;
                            newAdo.PhanLoaiBenhAn = String.Format("Bệnh án chưa đạt lần {0}: Trả lại khoa", stt);
                            stt++;
                        }
                        this.ListMRCheckSummary.Add(newAdo);
                    }
                }
                //checkList Khoa Tự KT
                if (rdo.ListMRChecklist != null)
                {
                    var listMRCheckSummaryID_Khoa = this.ListMRCheckSummary.Where(o => o.CHECK_PLACE == 2).Select(o => o.ID).ToList() ?? new List<long>();

                    this.ListMRChecklist = new List<HIS_MR_CHECKLIST>();
                    var listMR_CHECK_ITEM_ID = rdo.ListMRChecklist.Select(o => o.MR_CHECK_ITEM_ID).Distinct();
                    foreach (var item in listMR_CHECK_ITEM_ID)
                    {
                        HIS_MR_CHECKLIST latestMrChecklist = rdo.ListMRChecklist.Where(o => o.MR_CHECK_ITEM_ID == item 
                                                                                        && listMRCheckSummaryID_Khoa.Contains(o.MR_CHECK_SUMMARY_ID))
                                                                                .OrderByDescending(o => o.MODIFY_TIME ?? 0).FirstOrDefault();
                        if (latestMrChecklist != null)
                            this.ListMRChecklist.Add(latestMrChecklist);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetSingleKey()
        {
            try
            {
                if (rdo.Treatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000472ExtendSingleKey.TDL_PATIENT_NAME, rdo.Treatment.TDL_PATIENT_NAME));
                    SetSingleKey(new KeyValue(Mps000472ExtendSingleKey.TDL_PATIENT_CODE, rdo.Treatment.TDL_PATIENT_CODE));
                    string outDate = rdo.Treatment.OUT_DATE != null ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.Treatment.OUT_DATE.ToString()) : "";
                    SetSingleKey(new KeyValue(Mps000472ExtendSingleKey.OUT_DATE, outDate));

                    AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.Treatment, false);
                }
                if (rdo.ListMRCheckSummary != null)
                {
                    var firstMRSubmitDate = rdo.ListMRCheckSummary.Select(o => o.MEDI_RECORD_SUBMIT_DATE).FirstOrDefault();
                    string strFirstMRSubmitDate = firstMRSubmitDate != null ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(firstMRSubmitDate.ToString()) : "";
                    SetSingleKey(new KeyValue(Mps000472ExtendSingleKey.FIRST_MR_SUBMIT_DATE, strFirstMRSubmitDate));

                    var lateDatenumber = rdo.ListMRCheckSummary.Max(o => o.LATE_DATE_NUMBER ?? 0);
                    SetSingleKey(new KeyValue(Mps000472ExtendSingleKey.LATE_DATE_NUMBER, lateDatenumber));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetBarcodeKey()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void SetImageKey()
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                log = "Mã điều trị: " + rdo.Treatment.TREATMENT_CODE;
            }
            catch (Exception ex)
            {
                log = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return log;
        }

        public override string ProcessUniqueCodeData()
        {
            string result = "";
            try
            {
                if (rdo != null && rdo.Treatment != null)
                {
                    string treatmentCode = "TREATMENT_CODE:" + rdo.Treatment.TREATMENT_CODE;

                    result = String.Format("{0} {1}", printTypeCode, treatmentCode);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private string ProcessParentName(string name)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrWhiteSpace(name))
                {
                    List<string> word = name.Split(' ').ToList();
                    foreach (string item in word)
                    {
                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            result += char.ToUpper(item[0]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private class FlFuncElementFunction : TFlexCelUserFunction
        {

            object result = null;
            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length < 2)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");


                try
                {
                    //string KeyGet = Convert.ToString(parameters[1]);
                    string KeyGet = "";
                    if (!String.IsNullOrEmpty(parameters[1].ToString()))
                    {
                        KeyGet = parameters[1].ToString().Replace("\"", string.Empty).Trim();
                    }

                    if (parameters[0] is Dictionary<string, int>)
                    {
                        Dictionary<string, int> DicGet = parameters[0] as Dictionary<string, int>;
                        if (String.IsNullOrEmpty(KeyGet)) return DicGet.Values.Sum();
                        if (!DicGet.ContainsKey(KeyGet))
                        {
                            return null;//
                        }
                        result = DicGet[KeyGet];
                    }
                    else if (parameters[0] is Dictionary<string, long>)
                    {
                        Dictionary<string, long> DicGet = parameters[0] as Dictionary<string, long>;
                        if (String.IsNullOrEmpty(KeyGet)) return DicGet.Values.Sum();
                        if (!DicGet.ContainsKey(KeyGet))
                        {
                            return null;
                        }
                        result = DicGet[KeyGet];
                    }
                    else if (parameters[0] is Dictionary<string, decimal>)
                    {
                        Dictionary<string, decimal> DicGet = parameters[0] as Dictionary<string, decimal>;
                        if (String.IsNullOrEmpty(KeyGet)) return DicGet.Values.Sum();
                        if (!DicGet.ContainsKey(KeyGet))
                        {
                            return null;
                        }
                        result = DicGet[KeyGet];
                    }
                    else if (parameters[0] is Dictionary<string, string>)
                    {
                        Dictionary<string, string> DicGet = parameters[0] as Dictionary<string, string>;
                        if (String.IsNullOrEmpty(KeyGet)) return null;
                        if (!DicGet.ContainsKey(KeyGet))
                        {
                            return null;
                        }
                        result = DicGet[KeyGet];
                    }
                    else
                    {
                        result = null;
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    return null;
                }

                return result;
            }
        }
    }
}
