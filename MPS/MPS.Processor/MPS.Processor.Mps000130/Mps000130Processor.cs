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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000130.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000130
{
    public class Mps000130Processor : AbstractProcessor
    {
        Mps000130PDO rdo;
        public Mps000130Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000130PDO)rdoBase;
        }

        /// <summary>
        /// Ham xu ly du lieu da qua xu ly
        /// Tao ra cac doi tuong du lieu xu dung trong thu vien xu ly file excel
        /// </summary>
        /// <returns></returns>
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));

                ProcessSingleKey();
                singleTag.ProcessData(store, singleValueDictionary);
                //barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "ListMedicine", rdo.listMrs000130ADO);
                objectTag.AddObjectData(store, "LstMedicine", rdo.listMrs000130ADO);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        List<string> GetListStringApprovalLogFromExpMestMedicineMaterial(List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineList, List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialList, List<V_HIS_EXP_MEST_BLOOD> expMestBloods)
        {
            List<string> result = new List<string>();
            try
            {
                List<string> expMestMedicineGroups = new List<string>();
                List<string> expMestMaterialGroups = new List<string>();
                List<string> expMestBloodGroups = new List<string>();
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
                if (expMestBloods != null && expMestBloods.Count > 0)
                {
                    expMestBloodGroups = expMestBloods.Where(p => !string.IsNullOrEmpty(p.APPROVAL_LOGINNAME))
                    .GroupBy(o => o.APPROVAL_LOGINNAME)
                    .Select(p => p.First().APPROVAL_LOGINNAME)
                    .ToList();
                }
                result = expMestMedicineGroups.Union(expMestMaterialGroups).Union(expMestBloodGroups).ToList();
            }
            catch (Exception ex)
            {
                result = new List<string>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        List<string> GetListStringExpLogFromExpMestMedicineMaterial(List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineList, List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialList, List<V_HIS_EXP_MEST_BLOOD> expMestBloods)
        {
            List<string> result = new List<string>();
            try
            {
                List<string> expMestMedicineGroups = new List<string>();
                List<string> expMestMaterialGroups = new List<string>();
                List<string> expMestBloodGroups = new List<string>();
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
                if (expMestBloods != null && expMestBloods.Count > 0)
                {
                    expMestBloodGroups = expMestBloods.Where(p => !string.IsNullOrEmpty(p.EXP_LOGINNAME))
                    .GroupBy(o => o.EXP_LOGINNAME)
                    .Select(p => p.First().EXP_LOGINNAME)
                    .ToList();
                }
                result = expMestMedicineGroups.Union(expMestMaterialGroups).Union(expMestBloodGroups).ToList();
            }
            catch (Exception ex)
            {
                result = new List<string>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        List<string> GetListStringExpTimeLogFromExpMestMedicineMaterial(List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineList, List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialList, List<V_HIS_EXP_MEST_BLOOD> expMestBloods)
        {
            List<string> result = new List<string>();
            try
            {
                List<string> expMestMedicineGroups = new List<string>();
                List<string> expMestMaterialGroups = new List<string>();
                List<string> expMestBloodGroups = new List<string>();
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
                if (expMestBloods != null && expMestBloods.Count > 0)
                {
                    expMestBloodGroups = expMestBloods.Where(p => p.EXP_TIME != null)
                      .GroupBy(o => o.EXP_TIME)
                      .Select(p => Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(p.First().EXP_TIME ?? 0))
                      .ToList();
                }
                result = expMestMedicineGroups.Union(expMestMaterialGroups).Union(expMestBloodGroups).ToList();
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
                AddObjectKeyIntoListkey<HIS_EXP_MEST>(rdo.expMest, false);

                if (rdo.expMest != null)
                {
                    SetSingleKey(new KeyValue(Mps000130ExtendSingleKey.EXP_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString((rdo.expMest.FINISH_TIME ?? 0))));
                    if (rdo.expMest.SUPPLIER_ID > 0)
                    {
                        var supplier = rdo._Suppliers.FirstOrDefault(p => p.ID == rdo.expMest.SUPPLIER_ID);
                        if (supplier != null)
                        {
                            AddObjectKeyIntoListkey<HIS_SUPPLIER>(supplier, false);
                        }
                    }
                    if (rdo.expMest.MEDI_STOCK_ID > 0)
                    {
                        var mediStock = rdo._MediStocks.FirstOrDefault(p => p.ID == rdo.expMest.MEDI_STOCK_ID);
                        if (mediStock != null)
                        {
                            AddObjectKeyIntoListkey<V_HIS_MEDI_STOCK>(mediStock, false);
                        }
                    }
                }

                rdo.listMrs000130ADO = new List<MPS.Processor.Mps000130.PDO.Mps000130ADO>();
                if (rdo._Medicines != null && rdo._Medicines.Count > 0)
                {
                    rdo.listMrs000130ADO.AddRange((from r in rdo._Medicines select new MPS.Processor.Mps000130.PDO.Mps000130ADO(r)).ToList());
                }
                if (rdo._Materials != null && rdo._Materials.Count > 0)
                {
                    rdo.listMrs000130ADO.AddRange((from r in rdo._Materials select new MPS.Processor.Mps000130.PDO.Mps000130ADO(r)).ToList());
                }
                if (rdo._Bloods != null && rdo._Bloods.Count > 0)
                {
                    rdo.listMrs000130ADO.AddRange((from r in rdo._Bloods select new MPS.Processor.Mps000130.PDO.Mps000130ADO(r)).ToList());
                }
                if (rdo.listMrs000130ADO != null)
                {
                    decimal sumPrice = rdo.listMrs000130ADO.Sum(p => p.TOTAL_PRICE);
                    string TotalPriceSeparate = Inventec.Common.Number.Convert.NumberToString(sumPrice, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                    SetSingleKey(new KeyValue(Mps000130ExtendSingleKey.TOTAL_PRICE, sumPrice));
                    SetSingleKey(new KeyValue(Mps000130ExtendSingleKey.TOTAL_PRICE_SEPARATE, TotalPriceSeparate));
                    SetSingleKey(new KeyValue(Mps000130ExtendSingleKey.SUM_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(sumPrice).ToString())));
                }

                string approvalLoginname = String.Join(", ", GetListStringApprovalLogFromExpMestMedicineMaterial(rdo._Medicines, rdo._Materials,rdo._Bloods));
                string expLoginName = String.Join(", ", GetListStringExpLogFromExpMestMedicineMaterial(rdo._Medicines, rdo._Materials,rdo._Bloods));
                string expTime = String.Join(", ", GetListStringExpTimeLogFromExpMestMedicineMaterial(rdo._Medicines, rdo._Materials,rdo._Bloods));
                SetSingleKey(new KeyValue(Mps000130ExtendSingleKey.EXP_TIME, expTime));
                SetSingleKey(new KeyValue(Mps000130ExtendSingleKey.APPROVAL_LOGINNAME, approvalLoginname));
                SetSingleKey(new KeyValue(Mps000130ExtendSingleKey.EXP_LOGINNAME, expLoginName));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
