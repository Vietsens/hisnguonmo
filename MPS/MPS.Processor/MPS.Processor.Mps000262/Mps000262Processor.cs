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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000262.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000262
{
    public class Mps000262Processor : AbstractProcessor
    {
        List<Mps000262SDO> listExpMestMedicine;
        List<Mps000262CommonInfoADO> listTreatmentInfo;

        Mps000262PDO rdo;
        public Mps000262Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000262PDO)rdoBase;
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
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                SetSingleKey();

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "ExpMest", listExpMestMedicine);
                objectTag.AddObjectData(store, "Info", listTreatmentInfo);
                objectTag.AddRelationship(store, "Info", "ExpMest", "KEY_GROUP", "KEY_GROUP");
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void SetSingleKey()
        {
            try
            {
                if (rdo != null && rdo.ListExpMest != null && rdo.ListExpMest.Count > 0)
                {
                    listTreatmentInfo = new List<Mps000262CommonInfoADO>();
                    listExpMestMedicine = new List<Mps000262SDO>();

                    var groupPatient = rdo.ListExpMest.GroupBy(o => o.TDL_TREATMENT_ID);

                    foreach (var itemPatient in groupPatient)
                    {
                        if (itemPatient == null || itemPatient.Count() == 0 || itemPatient.FirstOrDefault() == null || itemPatient.FirstOrDefault().TDL_TREATMENT_ID == null)
                        {
                            continue;
                        }

                        var groupByIntructionDate = itemPatient.GroupBy(o => o.TDL_INTRUCTION_DATE);

                        foreach (var grIntruction in groupByIntructionDate)
                        {
                            List<HIS_SERVICE_REQ> HomePres = new List<HIS_SERVICE_REQ>();
                            List<HIS_SERVICE_REQ> Other = new List<HIS_SERVICE_REQ>();
                            if (rdo.ListServiceReq != null && rdo.ListServiceReq.Count > 0)
                            {
                                HomePres = rdo.ListServiceReq.Where(o => grIntruction.Select(s => s.SERVICE_REQ_ID ?? 0).Contains(o.ID) && o.IS_HOME_PRES == 1).ToList();
                                Other = rdo.ListServiceReq.Where(o => grIntruction.Select(s => s.SERVICE_REQ_ID ?? 0).Contains(o.ID) && (!o.IS_HOME_PRES.HasValue || o.IS_HOME_PRES != 1)).ToList();
                            }

                            if (HomePres != null && HomePres.Count > 0)
                            {
                                ProcessListData(grIntruction.Where(o => HomePres.Select(s => s.ID).Contains(o.SERVICE_REQ_ID ?? 0)).ToList(), true);
                            }

                            if (Other != null && Other.Count > 0)
                            {
                                ProcessListData(grIntruction.Where(o => Other.Select(s => s.ID).Contains(o.SERVICE_REQ_ID ?? 0)).ToList(), false);
                            }
                            else if (HomePres == null || HomePres.Count <= 0)
                            {
                                ProcessListData(grIntruction.ToList(), false);
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

        private void ProcessListData(List<V_HIS_EXP_MEST> lstExpMest, bool isHome)
        {
            try
            {
                if (lstExpMest != null && lstExpMest.Count > 0)
                {
                    V_HIS_TREATMENT _Treatment = rdo.ListTreatment != null ? rdo.ListTreatment.FirstOrDefault(o => o.ID == lstExpMest.First().TDL_TREATMENT_ID) : null;

                    V_HIS_TREATMENT_BED_ROOM _TreatmentBedRoom = rdo.ListTreatmentBedRoom != null ? rdo.ListTreatmentBedRoom.FirstOrDefault(o => o.TREATMENT_ID == lstExpMest.First().TDL_TREATMENT_ID) : null;

                    Mps000262CommonInfoADO ado = new Mps000262CommonInfoADO(_Treatment, _TreatmentBedRoom, lstExpMest.First());

                    string KEY_GROUP = _Treatment.TREATMENT_CODE + lstExpMest.First().TDL_INTRUCTION_DATE + isHome;

                    ado.KEY_GROUP = KEY_GROUP;
                    listTreatmentInfo.Add(ado);

                    List<V_HIS_EXP_MEST_MEDICINE> expmestMedicine = rdo.ListExpMestMedicines != null ? rdo.ListExpMestMedicines.Where(p => lstExpMest.Select(o => o.ID).Contains(p.EXP_MEST_ID ?? 0)).ToList() : null;
                    List<V_HIS_EXP_MEST_MATERIAL> expmestMaterial = rdo.ListExpMestMaterials != null ? rdo.ListExpMestMaterials.Where(p => lstExpMest.Select(o => o.ID).Contains(p.EXP_MEST_ID ?? 0)).ToList() : null;

                    if (expmestMedicine != null && expmestMedicine.Count > 0)
                    {
                        var groupExpMestMedicine = expmestMedicine.GroupBy(o => o.MEDICINE_TYPE_ID);
                        foreach (var item in groupExpMestMedicine)
                        {
                            var firstItem = item.FirstOrDefault();
                            Mps000262SDO mps000262SDO = new Mps000262SDO();
                            AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MEDICINE, Mps000262SDO>();
                            mps000262SDO = AutoMapper.Mapper.Map<Mps000262SDO>(firstItem);
                            mps000262SDO.TOTAL_AMOUNT = item.Sum(o => o.AMOUNT);
                            string amountStr = string.Format("{0:0.####}", (mps000262SDO.TOTAL_AMOUNT));
                            if (((mps000262SDO.TOTAL_AMOUNT % 1) == 0) && mps000262SDO.TOTAL_AMOUNT >= 1 && mps000262SDO.TOTAL_AMOUNT <= 9)
                            {
                                amountStr = "0" + amountStr;
                            }

                            mps000262SDO.TOTAL_AMOUNT_STR = amountStr;
                            mps000262SDO.KEY_GROUP = KEY_GROUP;

                            listExpMestMedicine.Add(mps000262SDO);
                        }
                    }

                    if (expmestMaterial != null && expmestMaterial.Count > 0)
                    {
                        var groupExpMestMaterial = expmestMaterial.GroupBy(o => o.MATERIAL_TYPE_ID);
                        foreach (var item in groupExpMestMaterial)
                        {
                            var firstItem = item.FirstOrDefault();
                            Mps000262SDO mps000262SDO = new Mps000262SDO();
                            AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MATERIAL, Mps000262SDO>();
                            mps000262SDO = AutoMapper.Mapper.Map<Mps000262SDO>(firstItem);
                            mps000262SDO.TOTAL_AMOUNT = item.Sum(o => o.AMOUNT);
                            mps000262SDO.MEDICINE_TYPE_CODE = item.First().MATERIAL_TYPE_CODE;
                            mps000262SDO.MEDICINE_TYPE_NAME = item.First().MATERIAL_TYPE_NAME;
                            mps000262SDO.MEDICINE_TYPE_NUM_ORDER = item.First().MATERIAL_TYPE_NUM_ORDER;
                            mps000262SDO.MEDICINE_NUM_ORDER = item.First().MATERIAL_NUM_ORDER;

                            string amountStr = string.Format("{0:0.####}", (mps000262SDO.TOTAL_AMOUNT));
                            if (((mps000262SDO.TOTAL_AMOUNT % 1) == 0) && mps000262SDO.TOTAL_AMOUNT >= 1 && mps000262SDO.TOTAL_AMOUNT <= 9)
                            {
                                amountStr = "0" + amountStr;
                            }

                            mps000262SDO.TOTAL_AMOUNT_STR = amountStr;
                            mps000262SDO.KEY_GROUP = KEY_GROUP;

                            listExpMestMedicine.Add(mps000262SDO);
                        }
                    }
                }
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
                if (rdo != null && rdo.AggrExpMest != null)
                    log = String.Format("{0}_{1}_{2}_{3}", rdo.AggrExpMest.EXP_MEST_CODE, rdo.AggrExpMest.TDL_INTRUCTION_DATE, rdo.AggrExpMest.MEDI_STOCK_CODE, rdo.AggrExpMest.REQ_DEPARTMENT_CODE);
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
                long countMediMate = 0;
                if (rdo.ListExpMestMaterials != null) countMediMate += rdo.ListExpMestMaterials.Count;
                if (rdo.ListExpMestMedicines != null) countMediMate += rdo.ListExpMestMedicines.Count;

                if (rdo != null && rdo.AggrExpMest != null)
                    result = String.Format("{0}_{1}_{2}_{3}_{4}_{5}", this.printTypeCode, rdo.AggrExpMest.EXP_MEST_CODE, rdo.AggrExpMest.REQ_DEPARTMENT_CODE, rdo.AggrExpMest.REQ_LOGINNAME, rdo.ListExpMest.Count, countMediMate);
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
