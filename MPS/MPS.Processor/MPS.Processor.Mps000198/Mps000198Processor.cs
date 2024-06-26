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
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ProcessorBase.Core;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000198.PDO;
using FlexCel.Report;
using MPS.ProcessorBase;

namespace MPS.Processor.Mps000198
{
    class Mps000198Processor : AbstractProcessor
    {
        Mps000198PDO rdo;
        public Mps000198Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000198PDO)rdoBase;
        }
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                ProcessSingleKey();

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "ListBlood1", rdo.listAdo);
                objectTag.AddObjectData(store, "ListBlood2", rdo.listAdo);
                objectTag.AddObjectData(store, "ListBlood3", rdo.listAdo);
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        List<string> GetListStringApprovalLogFromExpMestBloods(List<V_HIS_EXP_MEST_BLOOD> expMestBloodList)
        {
            List<string> result = new List<string>();
            try
            {
                List<string> expMestBloodGroups = new List<string>();
                if (expMestBloodList != null && expMestBloodList.Count > 0)
                {
                    expMestBloodGroups = expMestBloodList.Where(p => !string.IsNullOrEmpty(p.APPROVAL_LOGINNAME))
                    .GroupBy(o => o.APPROVAL_LOGINNAME)
                    .Select(p => p.First().APPROVAL_LOGINNAME)
                    .ToList();
                }
                result = expMestBloodGroups;
            }
            catch (Exception ex)
            {
                result = new List<string>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        List<string> GetListStringExpLogFromExpMestBloods(List<V_HIS_EXP_MEST_BLOOD> expMestBloodList)
        {
            List<string> result = new List<string>();
            try
            {
                List<string> expMestBloodGroups = new List<string>();
                if (expMestBloodList != null && expMestBloodList.Count > 0)
                {
                    expMestBloodGroups = expMestBloodList.Where(p => !string.IsNullOrEmpty(p.EXP_LOGINNAME))
                    .GroupBy(o => o.EXP_LOGINNAME)
                    .Select(p => p.First().EXP_USERNAME)
                    .ToList();
                }

                result = expMestBloodGroups;
            }
            catch (Exception ex)
            {
                result = new List<string>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        List<string> GetListStringExpTimeLogFromExpMestBloods(List<V_HIS_EXP_MEST_BLOOD> expMestBloodList)
        {
            List<string> result = new List<string>();
            try
            {
                List<string> expMestBloodGroups = new List<string>();
                if (expMestBloodList != null && expMestBloodList.Count > 0)
                {
                    expMestBloodGroups = expMestBloodList.Where(p => p.EXP_TIME != null)
                    .GroupBy(o => o.EXP_TIME)
                    .Select(p => Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(p.First().EXP_TIME ?? 0))
                    .ToList();
                }
                result = expMestBloodGroups;
            }
            catch (Exception ex)
            {
                result = new List<string>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        void ProcessSingleKey()
        {
            try
            {
                decimal totalPrice = 0;
                if (this.rdo._ChmsExpMest != null)
                {
                    SetSingleKey(new KeyValue(Mps000198ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.rdo._ChmsExpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000198ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.rdo._ChmsExpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000198ExtendSingleKey.CREATE_DATE_SEPARATE, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this.rdo._ChmsExpMest.CREATE_TIME ?? 0)));
                    if (rdo._MediStocks != null && rdo._MediStocks.Count > 0)
                    {
                        var data = rdo._MediStocks.FirstOrDefault(p => p.ID == this.rdo._ChmsExpMest.MEDI_STOCK_ID);
                        if (data != null)
                        {
                            SetSingleKey(new KeyValue(Mps000198ExtendSingleKey.EXP_IS_CABINET, data.IS_CABINET));
                        }

                        var ImpMedistock = rdo._MediStocks.FirstOrDefault(o => o.ID == this.rdo._ChmsExpMest.IMP_MEDI_STOCK_ID);
                        if (ImpMedistock != null)
                        {
                            SetSingleKey(new KeyValue(Mps000198ExtendSingleKey.IMP_MEDISTOCK_CODE, ImpMedistock.MEDI_STOCK_CODE));
                            SetSingleKey(new KeyValue(Mps000198ExtendSingleKey.IMP_MEDISTOCK_NAME, ImpMedistock.MEDI_STOCK_NAME));
                        }
                    }
                    AddObjectKeyIntoListkey(this.rdo._ChmsExpMest, false);
                }

                if (this.rdo._ExpMestBltyReqs != null && this.rdo._ExpMestBltyReqs.Count > 0)
                {
                    var Groups = this.rdo._ExpMestBltyReqs.GroupBy(g => new
                    {
                        g.BLOOD_TYPE_ID,
                        g.IS_EXPEND
                    }).Select(p => p.ToList()).ToList();
                    rdo.listAdo.AddRange(from r in Groups
                                         select new Mps000198ADO(rdo._ChmsExpMest,
                                             r,
                                             rdo._ExpMestBloods,
                                             rdo._BloodTypes,
                                             rdo._BloodABOs,
                                             rdo._BloodRHs,
                                             rdo.expMesttSttId__Approval,
                                             rdo.expMesttSttId__Export));
                }

                if (this.rdo._ExpMestBloods != null && this.rdo._ExpMestBloods.Count > 0)
                {
                    var Group = this.rdo._ExpMestBloods.GroupBy(g => new
                    {
                        g.BLOOD_TYPE_ID,
                        g.PACKAGE_NUMBER,
                        g.SUPPLIER_ID,
                        g.IMP_PRICE,
                        g.IMP_VAT_RATIO
                    }).ToList();
                    foreach (var group in Group)
                    {
                        var listByGroup = group.ToList<V_HIS_EXP_MEST_BLOOD>();
                        foreach (var item in listByGroup)
                        {
                            totalPrice += (1 * (item.IMP_PRICE) * (item.IMP_VAT_RATIO + 1)) - (item.DISCOUNT ?? 0);
                        }
                    }
                }
                string approvalLoginname = String.Join(", ", GetListStringApprovalLogFromExpMestBloods(rdo._ExpMestBloods));
                string expLoginName = String.Join(", ", GetListStringExpLogFromExpMestBloods(rdo._ExpMestBloods));
                string expTime = String.Join(", ", GetListStringExpTimeLogFromExpMestBloods(rdo._ExpMestBloods));
                SetSingleKey(new KeyValue(Mps000198ExtendSingleKey.APROVAL_USERNAME, approvalLoginname));
                SetSingleKey(new KeyValue(Mps000198ExtendSingleKey.EXP_USERNAME, expLoginName));
                SetSingleKey(new KeyValue(Mps000198ExtendSingleKey.EXP_TIME_STR, expTime));
                SetSingleKey(new KeyValue(Mps000198ExtendSingleKey.IS_PLAY_CHECK, rdo._keyPhieuTra));
                SetSingleKey(new KeyValue(Mps000198ExtendSingleKey.KEY_NAMES, rdo.KeyNames));
                rdo.listAdo = rdo.listAdo.OrderBy(o => o.TYPE_ID).ThenByDescending(t => t.NUM_ORDER).ThenByDescending(p => p.BLOOD_TYPE_NAME).ToList();

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
                if (rdo != null && rdo._ChmsExpMest != null)
                {
                    log = LogDataImpMest("", rdo._ChmsExpMest.EXP_MEST_CODE, rdo._ChmsExpMest.REQ_DEPARTMENT_NAME + "_" + rdo._ChmsExpMest.MEDI_STOCK_NAME);
                }
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
                if (rdo != null && rdo._ChmsExpMest != null)
                    result = String.Format("{0}_{1}_{2}_{3}_{4}", printTypeCode, rdo._ChmsExpMest.EXP_MEST_CODE, rdo._ChmsExpMest.MEDI_STOCK_CODE, rdo._ExpMestBloods.Count, rdo.KeyNames);
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
