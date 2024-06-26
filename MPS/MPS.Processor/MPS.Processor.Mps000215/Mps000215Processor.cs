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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ProcessorBase.Core;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000215.PDO;
using FlexCel.Report;
using MPS.ProcessorBase;

namespace MPS.Processor.Mps000215
{
    class Mps000215Processor : AbstractProcessor
    {
        Mps000215PDO rdo;
        List<Mps000215ADO> listAdoPrint = new List<Mps000215ADO>();
        List<Mps000215ADO> listAdoPrintGroup = new List<Mps000215ADO>();
        List<Mps000215ADODetail> listAdoPrintDetail = new List<Mps000215ADODetail>();
        List<Mps000215ADO> listMedicineType = new List<Mps000215ADO>();
        List<Mps000215ADO> lstMedicineParent = new List<Mps000215ADO>();
        const int countMax = 1500;
        public Mps000215Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000215PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                SetBarcodeKey();
                ProcessSingleKey();
                Inventec.Common.Logging.LogSystem.Debug("rdo.OderOptionKey:____" + rdo.OderOptionKey);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("listAdoPrint:____", listAdoPrint));
                if (listAdoPrint != null && listAdoPrint.Count > 0 && rdo.OderOptionKey > 0)
                {
                    switch (rdo.OderOptionKey)
                    {
                        case 1:
                            listAdoPrint = listAdoPrint.OrderBy(p => p.TYPE_ID).ThenBy(p => p.MEDI_MATE_NUM_ORDER).ThenBy(p => p.MEDI_MATE_TYPE_NAME).ToList();
                            break;
                        case 2:
                            listAdoPrint = listAdoPrint.OrderBy(p => p.MEDICINE_USE_FORM_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                            break;
                        case 3:
                            listAdoPrint = listAdoPrint.OrderByDescending(p => p.MEDICINE_USE_FORM_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                            break;
                        case 4:
                            listAdoPrint = listAdoPrint.OrderBy(p => p.SERVICE_UNIT_NAME).ThenBy(p => p.MEDI_MATE_TYPE_NAME).ToList();
                            break;
                    }

                }

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));
                if (listAdoPrintDetail != null && listAdoPrintDetail.Count > 0)
                {
                    listAdoPrintDetail = listAdoPrintDetail.OrderBy(o => o.TDL_PATIENT_FIRST_NAME).ToList();
                }
                GetMedicineGroup();
                GetMedicineParent();
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "ListMediMate1", listAdoPrint);
                objectTag.AddObjectData(store, "ListMediMate2", listAdoPrint);
                objectTag.AddObjectData(store, "ListMediMate3", listAdoPrint);
                objectTag.AddObjectData(store, "ListMediMateSplitedByPackage", listAdoPrintGroup);
                objectTag.AddObjectData(store, "ListMediMate1Detail", listAdoPrintDetail);
                objectTag.AddObjectData(store, "ListMediMate2Detail", listAdoPrintDetail);
                objectTag.AddObjectData(store, "ListMediMate3Detail", listAdoPrintDetail);
                objectTag.AddObjectData(store, "MedicineGroup", listMedicineType);
                objectTag.AddObjectData(store, "MedicineParent", lstMedicineParent);

                objectTag.AddRelationship(store, "ListMediMate1", "ListMediMate1Detail", "KEY_GROUP", "KEY_GROUP");
                objectTag.AddRelationship(store, "ListMediMate2", "ListMediMate2Detail", "KEY_GROUP", "KEY_GROUP");
                objectTag.AddRelationship(store, "ListMediMate3", "ListMediMate3Detail", "KEY_GROUP", "KEY_GROUP");
                objectTag.AddRelationship(store, "MedicineGroup", "ListMediMate1", "MEDICINE_GROUP_ID", "MEDICINE_GROUP_ID");
                objectTag.AddRelationship(store, "MedicineGroup", "ListMediMate2", "MEDICINE_GROUP_ID", "MEDICINE_GROUP_ID");
                objectTag.AddRelationship(store, "MedicineGroup", "ListMediMate3", "MEDICINE_GROUP_ID", "MEDICINE_GROUP_ID");
                objectTag.AddRelationship(store, "MedicineGroup", "ListMediMateSplitedByPackage", "MEDICINE_GROUP_ID", "MEDICINE_GROUP_ID");
                objectTag.AddRelationship(store, "MedicineGroup", "ListMediMate1Detail", "MEDICINE_GROUP_ID", "MEDICINE_GROUP_ID");
                objectTag.AddRelationship(store, "MedicineGroup", "ListMediMate2Detail", "MEDICINE_GROUP_ID", "MEDICINE_GROUP_ID");
                objectTag.AddRelationship(store, "MedicineGroup", "ListMediMate3Detail", "MEDICINE_GROUP_ID", "MEDICINE_GROUP_ID");

                objectTag.AddRelationship(store, "MedicineParent", "ListMediMate1", "MEDICINE_PARENT_ID", "MEDICINE_PARENT_ID");
                objectTag.AddRelationship(store, "MedicineParent", "ListMediMate2", "MEDICINE_PARENT_ID", "MEDICINE_PARENT_ID");
                objectTag.AddRelationship(store, "MedicineParent", "ListMediMate3", "MEDICINE_PARENT_ID", "MEDICINE_PARENT_ID");
                objectTag.AddRelationship(store, "MedicineParent", "ListMediMateSplitedByPackage", "MEDICINE_PARENT_ID", "MEDICINE_PARENT_ID");
                objectTag.AddRelationship(store, "MedicineParent", "ListMediMate1Detail", "MEDICINE_PARENT_ID", "MEDICINE_PARENT_ID");
                objectTag.AddRelationship(store, "MedicineParent", "ListMediMate2Detail", "MEDICINE_PARENT_ID", "MEDICINE_PARENT_ID");
                objectTag.AddRelationship(store, "MedicineParent", "ListMediMate3Detail", "MEDICINE_PARENT_ID", "MEDICINE_PARENT_ID");

                objectTag.SetUserFunction(store, "FuncMergeData11", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData12", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData21", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData22", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData31", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData32", new CalculateMergerData());
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        void ProcessSingleKey()
        {
            try
            {
                Dictionary<string, List<V_HIS_EXP_MEST_MEDICINE>> dicExpiredMedi = new Dictionary<string, List<V_HIS_EXP_MEST_MEDICINE>>();
                Dictionary<string, List<V_HIS_EXP_MEST_MATERIAL>> dicExpiredMate = new Dictionary<string, List<V_HIS_EXP_MEST_MATERIAL>>();

                string _keyname = "";
                switch (rdo._keyTitles)
                {
                    case keyTitles.tonghop:
                        _keyname = "TỔNG HỢP";
                        break;
                    case keyTitles.thuong:
                        _keyname = "THUỐC THƯỜNG";
                        break;
                    case keyTitles.vattu:
                        _keyname = "VẬT TƯ";
                        break;
                    case keyTitles.tienchat:
                        _keyname = "THUỐC TIỀN CHẤT";
                        break;
                    default:
                        break;
                }
                SetSingleKey(new KeyValue(Mps000215ExtendSingleKey.KEY_NAME_TITLES, _keyname));

                decimal totalPrice = 0;
                if (this.rdo._BcsExpMest != null)
                {
                    SetSingleKey(new KeyValue(Mps000215ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.rdo._BcsExpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000215ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.rdo._BcsExpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000215ExtendSingleKey.CREATE_DATE_SEPARATE, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this.rdo._BcsExpMest.CREATE_TIME ?? 0)));
                    AddObjectKeyIntoListkey(this.rdo._BcsExpMest, false);
                    if (rdo._MediStocks != null && rdo._MediStocks.Count > 0)
                    {
                        var data = rdo._MediStocks.FirstOrDefault(p => p.ID == this.rdo._BcsExpMest.MEDI_STOCK_ID);
                        if (data != null)
                        {
                            SetSingleKey(new KeyValue(Mps000215ExtendSingleKey.EXP_IS_CABINET, data.IS_CABINET));
                        }
                    }
                }

                long parent = 0;

                if (this.rdo._ExpMestMetyReqs != null && this.rdo._ExpMestMetyReqs.Count > 0)
                {
                    this.rdo._ExpMestMetyReqs = this.rdo._ExpMestMetyReqs.Where(o => o.AMOUNT > 0).ToList();

                    var GroupReqs = this.rdo._ExpMestMetyReqs.GroupBy(g => g.MEDICINE_TYPE_ID);
                    foreach (var req in GroupReqs)
                    {
                        var approve = this.rdo._Medicines != null ? this.rdo._Medicines.Where(o => req.Select(s => s.ID).Contains(o.EXP_MEST_METY_REQ_ID ?? 0) && o.MEDICINE_TYPE_ID == req.First().MEDICINE_TYPE_ID).ToList() : null;
                        var replaces = this.rdo._Medicines != null ? this.rdo._Medicines.Where(o => req.Select(s => s.ID).Contains(o.EXP_MEST_METY_REQ_ID ?? 0) && o.MEDICINE_TYPE_ID != req.First().MEDICINE_TYPE_ID).ToList() : null;

                        Mps000215ADO adoReq = new Mps000215ADO(rdo._BcsExpMest, req.ToList(), rdo._MedicineTypes, approve, false);
                        adoReq.KEY_GROUP = parent;
                        listAdoPrint.Add(adoReq);
                        if (approve != null && approve.Count > 0)
                        {
                            var dtGroup = approve.GroupBy(o => new { o.PACKAGE_NUMBER, o.EXPIRED_DATE }).ToList();
                        foreach (var item in dtGroup)
						{
                            Mps000215ADO adoReqGroup = new Mps000215ADO(rdo._BcsExpMest, req.ToList(), rdo._MedicineTypes,item.ToList(), false,true);
                            adoReqGroup.KEY_GROUP = parent;
                            listAdoPrintGroup.Add(adoReqGroup);
                        }
                        }
                        else
                        {
                            listAdoPrintGroup.Add(adoReq);

                        }

                        if (approve != null && approve.Count > 0)
                        {
                            if (this.rdo.ListTreatment != null && this.rdo.ListTreatment.Count > 0)
                            {
                                var treat = this.rdo.ListTreatment.Where(o => approve.Select(s => s.TDL_TREATMENT_ID).Contains(o.ID)).ToList();
                                foreach (var trea in treat)
                                {
                                    var appr = approve.Where(s => s.TDL_TREATMENT_ID == trea.ID).ToList();
                                    var re = req.Where(o => o.TREATMENT_ID == trea.ID).ToList();
                                    Mps000215ADODetail adoDetail = new Mps000215ADODetail(rdo._BcsExpMest, re, rdo._MedicineTypes, appr, false, trea);
                                    adoDetail.KEY_GROUP = parent;
                                    listAdoPrintDetail.Add(adoDetail);
                                }
                            }
                        }
                        else
                        {
                            if (this.rdo.ListTreatment != null && this.rdo.ListTreatment.Count > 0)
                            {
                                var treat = this.rdo.ListTreatment.Where(o => req.Select(s => s.TREATMENT_ID).Contains(o.ID)).ToList();
                                foreach (var trea in treat)
                                {
                                    var appr = approve.Where(s => s.TDL_TREATMENT_ID == trea.ID).ToList();
                                    var re = req.Where(o => o.TREATMENT_ID == trea.ID).ToList();
                                    Mps000215ADODetail adoDetail = new Mps000215ADODetail(rdo._BcsExpMest, re, rdo._MedicineTypes, appr, false, trea);
                                    adoDetail.KEY_GROUP = parent;
                                    listAdoPrintDetail.Add(adoDetail);
                                }
                            }
                        }
                        parent++;

                        if (replaces != null && replaces.Count > 0)
                        {
                            var dtGroupReplace = replaces.GroupBy(o => new { o.PACKAGE_NUMBER, o.EXPIRED_DATE }).ToList();
                            foreach (var item in dtGroupReplace)
                            {
                                Mps000215ADO adoReqGroup = new Mps000215ADO(rdo._BcsExpMest, req.ToList(), rdo._MedicineTypes, item.ToList(), false,true);
                                adoReqGroup.KEY_GROUP = parent;
                                listAdoPrintGroup.Add(adoReqGroup);
                            }

                            var Groups = replaces.GroupBy(g => g.MEDICINE_TYPE_ID).ToList();
                            foreach (var gr in Groups)
                            {
                                Mps000215ADO adoRp = new Mps000215ADO(rdo._BcsExpMest, req.ToList(), rdo._MedicineTypes, gr.ToList(), true);
                                adoRp.KEY_GROUP = parent;
                                listAdoPrint.Add(adoRp);

                                var treat = this.rdo.ListTreatment.Where(o => gr.Select(s => s.TDL_TREATMENT_ID).Contains(o.ID)).ToList();
                                foreach (var trea in treat)
                                {
                                    var appr = gr.Where(s => s.TDL_TREATMENT_ID == trea.ID).ToList();
                                    var re = req.Where(o => o.TREATMENT_ID == trea.ID).ToList();
                                    Mps000215ADODetail adoDetail = new Mps000215ADODetail(rdo._BcsExpMest, re, rdo._MedicineTypes, appr, true, trea);
                                    adoDetail.KEY_GROUP = parent;
                                    listAdoPrintDetail.Add(adoDetail);
                                }
                                parent++;
                            }
                        }
                    }
                }

                if (this.rdo._ExpMestMatyReqs != null && this.rdo._ExpMestMatyReqs.Count > 0)
                {
                    this.rdo._ExpMestMatyReqs = this.rdo._ExpMestMatyReqs.Where(o => o.AMOUNT > 0).ToList();
                    var GroupReqs = this.rdo._ExpMestMatyReqs.GroupBy(g => g.MATERIAL_TYPE_ID);
                    foreach (var req in GroupReqs)
                    {
                        var approve = this.rdo._Materials != null ? this.rdo._Materials.Where(o => req.Select(s => s.ID).Contains(o.EXP_MEST_MATY_REQ_ID ?? 0) && o.MATERIAL_TYPE_ID == req.First().MATERIAL_TYPE_ID).ToList() : null;
                        var replaces = this.rdo._Materials != null ? this.rdo._Materials.Where(o => req.Select(s => s.ID).Contains(o.EXP_MEST_MATY_REQ_ID ?? 0) && o.MATERIAL_TYPE_ID != req.First().MATERIAL_TYPE_ID).ToList() : null;

                        Mps000215ADO adoReq = new Mps000215ADO(rdo._BcsExpMest, req.ToList(), rdo._MaterialTypes, approve, false);
                        adoReq.KEY_GROUP = parent;
                        listAdoPrint.Add(adoReq);

                        if (approve != null && approve.Count > 0)
                        {
                            var dtGroup = approve.GroupBy(o => new { o.PACKAGE_NUMBER, o.EXPIRED_DATE }).ToList();
                            foreach (var item in dtGroup)
                            {
                                Mps000215ADO adoReqGroup = new Mps000215ADO(rdo._BcsExpMest, req.ToList(), rdo._MaterialTypes, item.ToList(), false,true);
                                adoReqGroup.KEY_GROUP = parent;
                                listAdoPrintGroup.Add(adoReqGroup);
                            }
						}
						else
						{
                            listAdoPrintGroup.Add(adoReq);

                        }


                        if (approve != null && approve.Count > 0)
                        {
                            if (this.rdo.ListTreatment != null && this.rdo.ListTreatment.Count > 0)
                            {
                                var treat = this.rdo.ListTreatment.Where(o => approve.Select(s => s.TDL_TREATMENT_ID).Contains(o.ID)).ToList();
                                foreach (var trea in treat)
                                {
                                    var appr = approve.Where(s => s.TDL_TREATMENT_ID == trea.ID).ToList();
                                    var re = req.Where(o => o.TREATMENT_ID == trea.ID).ToList();
                                    Mps000215ADODetail adoDetail = new Mps000215ADODetail(rdo._BcsExpMest, re, rdo._MaterialTypes, appr, false, trea);
                                    adoDetail.KEY_GROUP = parent;
                                    listAdoPrintDetail.Add(adoDetail);
                                }
                            }
                        }
                        else
                        {
                            if (this.rdo.ListTreatment != null && this.rdo.ListTreatment.Count > 0)
                            {
                                var treat = this.rdo.ListTreatment.Where(o => req.Select(s => s.TREATMENT_ID).Contains(o.ID)).ToList();
                                foreach (var trea in treat)
                                {
                                    var appr = approve.Where(s => s.TDL_TREATMENT_ID == trea.ID).ToList();
                                    var re = req.Where(o => o.TREATMENT_ID == trea.ID).ToList();
                                    Mps000215ADODetail adoDetail = new Mps000215ADODetail(rdo._BcsExpMest, re, rdo._MaterialTypes, appr, false, trea);
                                    adoDetail.KEY_GROUP = parent;
                                    listAdoPrintDetail.Add(adoDetail);
                                }
                            }
                        }
                        parent++;

                        if (replaces != null && replaces.Count > 0)
                        {
                            var dtGroupReplace = replaces.GroupBy(o => new { o.PACKAGE_NUMBER, o.EXPIRED_DATE }).ToList();
                            foreach (var item in dtGroupReplace)
                            {
                                Mps000215ADO adoReqGroup = new Mps000215ADO(rdo._BcsExpMest, req.ToList(), rdo._MaterialTypes, item.ToList(), false,true);
                                adoReqGroup.KEY_GROUP = parent;
                                listAdoPrintGroup.Add(adoReqGroup);
                            }

                            var Groups = replaces.GroupBy(g => g.MATERIAL_TYPE_ID).ToList();
                            foreach (var gr in Groups)
                            {
                                Mps000215ADO adoRp = new Mps000215ADO(rdo._BcsExpMest, req.ToList(), rdo._MaterialTypes, gr.ToList(), true);
                                adoRp.KEY_GROUP = parent;
                                listAdoPrint.Add(adoRp);

                                var treat = this.rdo.ListTreatment.Where(o => gr.Select(s => s.TDL_TREATMENT_ID).Contains(o.ID)).ToList();
                                foreach (var trea in treat)
                                {
                                    var appr = gr.Where(s => s.TDL_TREATMENT_ID == trea.ID).ToList();
                                    var re = req.Where(o => o.TREATMENT_ID == trea.ID).ToList();
                                    Mps000215ADODetail adoDetail = new Mps000215ADODetail(rdo._BcsExpMest, re, rdo._MaterialTypes, appr, true, trea);
                                    adoDetail.KEY_GROUP = parent;
                                    listAdoPrintDetail.Add(adoDetail);
                                }
                                parent++;
                            }
                        }
                    }
                }

                if (this.rdo._Medicines != null && this.rdo._Medicines.Count > 0)
                {
                    var Group = this.rdo._Medicines.GroupBy(g => new
                    {
                        g.MEDICINE_TYPE_ID,
                        g.PACKAGE_NUMBER,
                        g.SUPPLIER_ID,
                        g.IMP_PRICE,
                        g.IMP_VAT_RATIO
                    }).ToList();
                    foreach (var group in Group)
                    {
                        dicExpiredMedi.Clear();
                        var listByGroup = group.ToList<V_HIS_EXP_MEST_MEDICINE>();
                        foreach (var item in listByGroup)
                        {
                            totalPrice += (item.AMOUNT * (item.IMP_PRICE) * (item.IMP_VAT_RATIO + 1)) - (item.DISCOUNT ?? 0);
                        }
                    }
                }

                if (this.rdo._Materials != null && this.rdo._Materials.Count > 0)
                {
                    var Group = this.rdo._Materials.GroupBy(g => new
                    {
                        g.MATERIAL_TYPE_ID,
                        g.PACKAGE_NUMBER,
                        g.SUPPLIER_ID,
                        g.IMP_PRICE,
                        g.IMP_VAT_RATIO
                    }).ToList();
                    foreach (var group in Group)
                    {
                        dicExpiredMate.Clear();
                        var listByGroup = group.ToList<V_HIS_EXP_MEST_MATERIAL>();
                        foreach (var item in listByGroup)
                        {
                            totalPrice += (item.AMOUNT * (item.IMP_PRICE) * (item.IMP_VAT_RATIO + 1)) - (item.DISCOUNT ?? 0);
                        }
                    }
                }

                if (this.rdo.ListTreatment != null && this.rdo.ListTreatment.Count > 0)
                {
                    List<string> TreatmentCodeList = this.rdo.ListTreatment.Select(s => s.TREATMENT_CODE).Distinct().ToList();
                    if (TreatmentCodeList != null && TreatmentCodeList.Count <= countMax)
                    {
                        string treatCodes = String.Join(",", TreatmentCodeList);
                        SetSingleKey(new KeyValue(Mps000215ExtendSingleKey.TREATMENT_CODES, treatCodes));
                    }
                    else if (TreatmentCodeList != null && TreatmentCodeList.Count > countMax)
                    {
                        int skip = 0;
                        int count = 0;
                        while (TreatmentCodeList.Count - skip > 0)
                        {
                            var listIds = TreatmentCodeList.Skip(skip).Take(countMax).ToList();
                            skip += countMax;
                            if (count == 0)
                                SetSingleKey(new KeyValue(Mps000215ExtendSingleKey.TREATMENT_CODES, String.Join(",", listIds)));
                            else
                                SetSingleKey(new KeyValue(Mps000215ExtendSingleKey.TREATMENT_CODES + count, String.Join(",", listIds)));
                            count++;
                        }
                    }
                }

                if (this.rdo._BcsMoreInfoSDO != null)
                {
                    if (this.rdo._BcsMoreInfoSDO.ExpMestCodes != null && this.rdo._BcsMoreInfoSDO.ExpMestCodes.Count > 0)
                    {
                        List<string> ExpMestCodeList = this.rdo._BcsMoreInfoSDO.ExpMestCodes.Distinct().ToList();
                        if (ExpMestCodeList != null && ExpMestCodeList.Count <= countMax)
                        {
                            string expMestCodes = String.Join(",", ExpMestCodeList);
                            SetSingleKey(new KeyValue(Mps000215ExtendSingleKey.EXP_MEST_CODES, expMestCodes));
                        }
                        else if (ExpMestCodeList != null && ExpMestCodeList.Count > countMax)
                        {
                            int skip = 0;
                            int count = 0;
                            while (ExpMestCodeList.Count - skip > 0)
                            {
                                var listIds = ExpMestCodeList.Skip(skip).Take(countMax).ToList();
                                skip += countMax;
                                if (count == 0)
                                    SetSingleKey(new KeyValue(Mps000215ExtendSingleKey.EXP_MEST_CODES, String.Join(",", listIds)));
                                else
                                    SetSingleKey(new KeyValue(Mps000215ExtendSingleKey.EXP_MEST_CODES + count, String.Join(",", listIds)));
                                count++;
                            }
                        }
                    }

                    //expCodes = String.Join(",", this.rdo._BcsMoreInfoSDO.ExpMestCodes);
                    //SetSingleKey(new KeyValue(Mps000215ExtendSingleKey.EXP_MEST_CODES, expCodes));

                    if ((this.rdo.ListTreatment == null || this.rdo.ListTreatment.Count <= 0)
                        && (this.rdo._BcsMoreInfoSDO.TreatmentCodes != null && this.rdo._BcsMoreInfoSDO.TreatmentCodes.Count > 0))
                    {

                        List<string> TreatmentCodeList = this.rdo._BcsMoreInfoSDO.TreatmentCodes.Distinct().ToList();
                        if (TreatmentCodeList != null && TreatmentCodeList.Count <= countMax)
                        {
                            string treatCodes = String.Join(",", TreatmentCodeList);
                            SetSingleKey(new KeyValue(Mps000215ExtendSingleKey.TREATMENT_CODES, treatCodes));
                        }
                        else if (TreatmentCodeList != null && TreatmentCodeList.Count > countMax)
                        {
                            int skip = 0;
                            int count = 0;
                            while (TreatmentCodeList.Count - skip > 0)
                            {
                                var listIds = TreatmentCodeList.Skip(skip).Take(countMax).ToList();
                                skip += countMax;
                                if (count == 0)
                                    SetSingleKey(new KeyValue(Mps000215ExtendSingleKey.TREATMENT_CODES, String.Join(",", listIds)));
                                else
                                    SetSingleKey(new KeyValue(Mps000215ExtendSingleKey.TREATMENT_CODES + count, String.Join(",", listIds)));
                                count++;
                            }
                        }

                        //string treatCodes = String.Join(",", this.rdo._BcsMoreInfoSDO.TreatmentCodes);
                        //SetSingleKey(new KeyValue(Mps000215ExtendSingleKey.TREATMENT_CODES, treatCodes));
                    }

                    SetSingleKey(new KeyValue(Mps000215ExtendSingleKey.COUNT_PRESCRIPTION, this.rdo._BcsMoreInfoSDO.PrescriptionCount));
                }

                string sumpTotalPriceSeparate = Inventec.Common.Number.Convert.NumberToString(totalPrice, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                string approvalLoginname = String.Join(", ", GetListStringApprovalLogFromExpMestMedicineMaterial(rdo._Medicines, rdo._Materials));
                string expLoginName = String.Join(", ", GetListStringExpLogFromExpMestMedicineMaterial(rdo._Medicines, rdo._Materials));
                string expTime = String.Join(", ", GetListStringExpTimeLogFromExpMestMedicineMaterial(rdo._Medicines, rdo._Materials));
                SetSingleKey(new KeyValue(Mps000215ExtendSingleKey.EXP_TIME, expTime));
                SetSingleKey(new KeyValue(Mps000215ExtendSingleKey.APPROVAL_LOGINNAME, approvalLoginname));
                SetSingleKey(new KeyValue(Mps000215ExtendSingleKey.EXP_LOGINNAME, expLoginName));
                SetSingleKey(new KeyValue(Mps000215ExtendSingleKey.SUM_TOTAL_PRICE, totalPrice));
                SetSingleKey(new KeyValue(Mps000215ExtendSingleKey.SUM_TOTAL_PRICE_SEPARATE, sumpTotalPriceSeparate));
                string sumText = String.Format("0:0.####", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice));
                SetSingleKey(new KeyValue(Mps000215ExtendSingleKey.SUM_TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumText)));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<string> GetListStringApprovalLogFromExpMestMedicineMaterial(List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineList, List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialList)
        {
            List<string> result = new List<string>();
            try
            {
                List<string> expMestMedicineGroups = new List<string>();
                List<string> expMestMaterialGroups = new List<string>();
                if (expMestMedicineList != null && expMestMedicineList.Count > 0)
                {
                    expMestMedicineGroups = expMestMedicineList.Where(p => !string.IsNullOrEmpty(p.APPROVAL_LOGINNAME))
                    .GroupBy(o => o.APPROVAL_LOGINNAME)
                    .Select(p => p.First().APPROVAL_LOGINNAME)
                    .ToList();
                }
                if (expMestMaterialList != null && expMestMaterialList.Count > 0)
                {
                    expMestMaterialGroups = expMestMaterialList.Where(p => !string.IsNullOrEmpty(p.APPROVAL_LOGINNAME))
                    .GroupBy(o => o.APPROVAL_LOGINNAME)
                    .Select(p => p.First().APPROVAL_LOGINNAME)
                    .ToList();
                }
                result = expMestMedicineGroups.Union(expMestMaterialGroups).ToList();
            }
            catch (Exception ex)
            {
                result = new List<string>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        List<string> GetListStringExpLogFromExpMestMedicineMaterial(List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineList, List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialList)
        {
            List<string> result = new List<string>();
            try
            {
                List<string> expMestMedicineGroups = new List<string>();
                List<string> expMestMaterialGroups = new List<string>();
                if (expMestMedicineList != null && expMestMedicineList.Count > 0)
                {
                    expMestMedicineGroups = expMestMedicineList.Where(p => !string.IsNullOrEmpty(p.EXP_LOGINNAME))
                    .GroupBy(o => o.EXP_LOGINNAME)
                    .Select(p => p.First().EXP_LOGINNAME)
                    .ToList();
                }
                if (expMestMaterialList != null && expMestMaterialList.Count > 0)
                {
                    expMestMaterialGroups = expMestMaterialList.Where(p => !string.IsNullOrEmpty(p.EXP_LOGINNAME))
                    .GroupBy(o => o.EXP_LOGINNAME)
                    .Select(p => p.First().EXP_LOGINNAME)
                    .ToList();
                }
                result = expMestMedicineGroups.Union(expMestMaterialGroups).ToList();
            }
            catch (Exception ex)
            {
                result = new List<string>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        List<string> GetListStringExpTimeLogFromExpMestMedicineMaterial(List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineList, List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialList)
        {
            List<string> result = new List<string>();
            try
            {
                List<string> expMestMedicineGroups = new List<string>();
                List<string> expMestMaterialGroups = new List<string>();
                if (expMestMedicineList != null && expMestMedicineList.Count > 0)
                {
                    expMestMedicineGroups = expMestMedicineList.Where(p => p.EXP_TIME != null)
                    .GroupBy(o => o.EXP_TIME)
                    .Select(p => Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(p.First().EXP_TIME ?? 0))
                    .ToList();
                }
                if (expMestMaterialList != null && expMestMaterialList.Count > 0)
                {
                    expMestMaterialGroups = expMestMaterialList.Where(p => p.EXP_TIME != null)
                      .GroupBy(o => o.EXP_TIME)
                      .Select(p => Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(p.First().EXP_TIME ?? 0))
                      .ToList();
                }
                result = expMestMedicineGroups.Union(expMestMaterialGroups).ToList();
            }
            catch (Exception ex)
            {
                result = new List<string>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        internal void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo._BcsExpMest.EXP_MEST_CODE);
                barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodePatientCode.IncludeLabel = false;
                barcodePatientCode.Width = 120;
                barcodePatientCode.Height = 40;
                barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodePatientCode.IncludeLabel = true;

                dicImage.Add(Mps000215ExtendSingleKey.EXP_MEST_CODE_BARCODE, barcodePatientCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                log = LogDataExpMest(rdo._BcsExpMest.TDL_TREATMENT_CODE, rdo._BcsExpMest.EXP_MEST_CODE, "");
                log += string.Format("Kho: {0}, Phòng yêu cầu: {1}", rdo._BcsExpMest.MEDI_STOCK_NAME, rdo._BcsExpMest.REQ_ROOM_NAME);
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
                string _keyname = "";
                switch (rdo._keyTitles)
                {
                    case keyTitles.tonghop:
                        _keyname = "TH";
                        break;
                    case keyTitles.thuong:
                        _keyname = "TT";
                        break;
                    case keyTitles.vattu:
                        _keyname = "VT";
                        break;
                    default:
                        break;
                }

                if (rdo != null && rdo._BcsExpMest != null)
                    result = String.Format("{0}_{1}_{2}_{3}_{4}", printTypeCode, rdo._BcsExpMest.REQ_DEPARTMENT_CODE, rdo._BcsExpMest.MEDI_STOCK_CODE, rdo._BcsExpMest.EXP_MEST_CODE, _keyname);
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        private void GetMedicineGroup()
        {
            try
            {
                if (listAdoPrint != null && listAdoPrint.Count > 0)
                {
                    var group = listAdoPrint.GroupBy(o => new { o.MEDICINE_GROUP_ID, o.MEDICINE_GROUP_CODE, o.MEDICINE_GROUP_NAME });
                    foreach (var item in group)
                    {
                        listMedicineType.Add(item.ToList().First());
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetMedicineParent()
        {
            try
            {
                if (listAdoPrint != null && listAdoPrint.Count > 0)
                {
                    var group = listAdoPrint.GroupBy(o => o.MEDICINE_PARENT_ID);
                    foreach (var item in group)
                    {
                        lstMedicineParent.Add(item.ToList().First());
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }


    class CalculateMergerData : TFlexCelUserFunction
    {
        long typeId = 0;
        long mediMateTypeId = 0;

        public override object Evaluate(object[] parameters)
        {
            if (parameters == null || parameters.Length <= 0)
                throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");
            bool result = false;
            try
            {
                long servicetypeId = Convert.ToInt64(parameters[0]);
                long mediMateId = Convert.ToInt64(parameters[1]);

                if (servicetypeId > 0 && mediMateId > 0)
                {
                    if (this.typeId == servicetypeId && this.mediMateTypeId == mediMateId)
                    {
                        return true;
                    }
                    else
                    {
                        this.typeId = servicetypeId;
                        this.mediMateTypeId = mediMateId;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
    }
}
