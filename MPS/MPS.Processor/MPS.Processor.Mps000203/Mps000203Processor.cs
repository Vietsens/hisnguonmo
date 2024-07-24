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
using MPS.Processor.Mps000203.PDO;
using FlexCel.Report;
using MPS.ProcessorBase;

namespace MPS.Processor.Mps000203
{
    class Mps000203Processor : AbstractProcessor
    {
        Mps000203PDO rdo;
        List<Mps000203RDO> ListExpMestBlood;
        public Mps000203Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000203PDO)rdoBase;
        }
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                ListExpMestBlood = new List<Mps000203RDO>();
                ProcessBloodGover(rdo._Bloods);
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                ProcessSingleKey();
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "ListBlood1", rdo._Bloods);
                objectTag.AddObjectData(store, "ListBlood2", rdo._Bloods);
                objectTag.AddObjectData(store, "ListBlood3", rdo._Bloods);
                objectTag.AddObjectData(store, "ListExpMestBlood", ListExpMestBlood);
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ProcessBloodGover(List<V_HIS_EXP_MEST_BLOOD> bloods)
        {
            try
            {
                foreach(var blood in bloods)
                {
                    Mps000203RDO rdo = new Mps000203RDO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST_BLOOD>(rdo, blood);
                    if(blood.BLOOD_GIVE_ID != null)
                    {
                        var giver = this.rdo.listBloodGiver.FirstOrDefault(o => o.ID == blood.BLOOD_GIVE_ID);
                        MapDataToGiver(giver,ref rdo);
                    }
                    
                    ListExpMestBlood.Add(rdo);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MapDataToGiver(HIS_BLOOD_GIVER giver,ref Mps000203RDO rdo)
        {
            try
            {
                if(giver != null)
                {
                    rdo.TEST_AFTER_HIV = giver.TEST_AFTER_HIV != null ? (giver.TEST_AFTER_HIV == 0 ? "Âm tính" : "Dương tính") : "";
                    rdo.TEST_AFTER_HBV = giver.TEST_AFTER_HBV != null ? (giver.TEST_AFTER_HBV == 0 ? "Âm tính" : "Dương tính") : "";
                    rdo.TEST_AFTER_HCV = giver.TEST_AFTER_HCV != null ? (giver.TEST_AFTER_HCV == 0 ? "Âm tính" : "Dương tính") : "";
                    rdo.TEST_AFTER_GM = giver.TEST_AFTER_GM != null ? (giver.TEST_AFTER_GM == 0 ? "Âm tính" : "Dương tính") : "";
                    rdo.TEST_AFTER_KTBT = giver.TEST_AFTER_KTBT != null ? (giver.TEST_AFTER_KTBT == 0 ? "Âm tính" : "Dương tính") : "";
                    //NAT
                    rdo.TEST_AFTER_NAT_HIV = giver.TEST_AFTER_NAT_HIV != null ? (giver.TEST_AFTER_NAT_HIV == 0 ? "Âm tính" : "Dương tính") : "";
                    rdo.TEST_AFTER_NAT_HBV = giver.TEST_AFTER_NAT_HBV != null ? (giver.TEST_AFTER_NAT_HBV == 0 ? "Âm tính" : "Dương tính") : "";
                    rdo.TEST_AFTER_NAT_HCV = giver.TEST_AFTER_NAT_HCV != null ? (giver.TEST_AFTER_NAT_HCV == 0 ? "Âm tính" : "Dương tính") : "";
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ProcessSingleKey()
        {
            try
            {
                if (this.rdo._ExpMest != null)
                {
                    SetSingleKey(new KeyValue(Mps000203ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.rdo._ExpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000203ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.rdo._ExpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000203ExtendSingleKey.CREATE_DATE_SEPARATE, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this.rdo._ExpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000203ExtendSingleKey.EXP_REASON_STR,(this.rdo._mps000203ADO.EXP_REASON_NAME)));
                    SetSingleKey(new KeyValue(Mps000203ExtendSingleKey.EXP_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.rdo._ExpMest.FINISH_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000203ExtendSingleKey.DATE, (this.rdo._ExpMest.CREATE_TIME ?? 0).ToString().Substring(6,2)));
                    SetSingleKey(new KeyValue(Mps000203ExtendSingleKey.MONTH, (this.rdo._ExpMest.CREATE_TIME ?? 0).ToString().Substring(4, 2)));
                    SetSingleKey(new KeyValue(Mps000203ExtendSingleKey.YEAR, (this.rdo._ExpMest.CREATE_TIME ?? 0).ToString().Substring(0, 4)));

                    AddObjectKeyIntoListkey(this.rdo._ExpMest, false);
                    AddObjectKeyIntoListkey(this.rdo._mps000203ADO, false);
                }

                //if (this.rdo._Bloods != null && this.rdo._Bloods.Count > 0)
                //{
                    //if (rdo._ChmsExpMest.EXP_MEST_STT_ID == rdo.expMesttSttId__Approval || rdo._ChmsExpMest.EXP_MEST_STT_ID == rdo.expMesttSttId__Export)
                    //{
                    //    this.rdo._Bloods = this.rdo._Bloods.Where(o => o.IN_EXECUTE == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_BLTY.IN_EXECUTE__TRUE).ToList();
                    //}
                    //else if (rdo._ChmsExpMest.EXP_MEST_STT_ID == rdo.expMesttSttId__Request || rdo._ChmsExpMest.EXP_MEST_STT_ID == rdo.expMesttSttId__Reject || rdo._ChmsExpMest.EXP_MEST_STT_ID == rdo.expMesttSttId__Draft)
                    //{
                    //    this.rdo._Bloods = this.rdo._Bloods.Where(o => o.IN_REQUEST == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_BLTY.IN_REQUEST__TRUE).ToList();
                    //}
                    //foreach (var item in this.rdo._Bloods)
                    //{
                    //    this.rdo.listAdo.Add(new Mps000198ADO(item));
                    //}
                    //var Group = this.rdo._Bloods.GroupBy(g => new { g.BLOOD_TYPE_ID}).ToList();
                    //foreach (var group in Group)
                    //{
                    //    dicExpiredBlood.Clear();
                    //    var listByGroup = group.ToList<V_HIS_EXP_MEST_BLTY>();
                    //    foreach (var item in listByGroup)
                    //    {
                    //        string key = item.EXPIRED_DATE.HasValue ? item.EXPIRED_DATE.Value.ToString().Substring(0, 8) : "0";
                    //        if (!dicExpiredBlood.ContainsKey(key))
                    //            dicExpiredBlood[key] = new List<V_HIS_EXP_MEST_BLTY>();
                    //        dicExpiredBlood[key].Add(item);

                    //    }
                    //    foreach (var dic in dicExpiredBlood)
                    //    {
                    //        rdo.listAdo.Add(new Mps000198ADO(dic.Value));
                    //    }
                    //}
                //}

                //rdo.listAdo = rdo.listAdo.OrderBy(o => o.TYPE_ID).ThenByDescending(t => t.NUM_ORDER).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    //class CalculateMergerData : TFlexCelUserFunction
    //{
    //    long typeId = 0;
    //    long mediMateTypeId = 0;

    //    public override object Evaluate(object[] parameters)
    //    {
    //        if (parameters == null || parameters.Length <= 0)
    //            throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");
    //        bool result = false;
    //        try
    //        {
    //            long servicetypeId = Convert.ToInt64(parameters[0]);
    //            long mediMateId = Convert.ToInt64(parameters[1]);

    //            if (servicetypeId > 0 && mediMateId > 0)
    //            {
    //                if (this.typeId == servicetypeId && this.mediMateTypeId == mediMateId)
    //                {
    //                    return true;
    //                }
    //                else
    //                {
    //                    this.typeId = servicetypeId;
    //                    this.mediMateTypeId = mediMateId;
    //                    return false;
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Inventec.Common.Logging.LogSystem.Error(ex);
    //            result = false;
    //        }
    //        return result;
    //    }
    //}
    public class Mps000203RDO : V_HIS_EXP_MEST_BLOOD
    {
        public string TEST_AFTER_HIV { get; set; }
        public string TEST_AFTER_HBV { get; set; }
        public string TEST_AFTER_HCV { get; set; }
        public string TEST_AFTER_GM { get; set; }
        public string TEST_AFTER_KTBT { get; set; }
        public string TEST_AFTER_NAT_HIV { get; set; }
        public string TEST_AFTER_NAT_HCV { get; set; }
        public string TEST_AFTER_NAT_HBV { get; set; }
    }
}
