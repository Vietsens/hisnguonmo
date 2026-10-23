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
using HIS.Desktop.Plugins.PublicMedicineMultiPatient.ADO;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.PublicMedicineMultiPatient.Base
{
    /// <summary>
    /// Builds the disclosure rows of every treatment from data loaded for many treatments at once.
    /// Grouping, amounts and dates follow HIS.Desktop.Plugins.PublicMedicineByPhased, except that a row whose
    /// service is not executed is skipped alone (the old screen stopped the whole loop there).
    /// </summary>
    internal class PublicMedicineRowBuilder
    {
        internal static Dictionary<long, List<ExpMestMediAndMateADO>> Build(
            List<HIS_SERVICE_REQ> serviceReqs,
            List<HIS_EXP_MEST> expMests,
            List<V_HIS_SERE_SERV> sereServs,
            List<V_HIS_EXP_MEST_MEDICINE> medicines,
            List<V_HIS_EXP_MEST_MATERIAL> materials,
            List<V_HIS_EXP_MEST_BLOOD> bloods,
            PublicMedicineFilterADO option)
        {
            Dictionary<long, List<ExpMestMediAndMateADO>> result = new Dictionary<long, List<ExpMestMediAndMateADO>>();
            try
            {
                if (serviceReqs == null || expMests == null || option == null)
                    return result;

                Dictionary<long, HIS_SERVICE_REQ> dicServiceReq = serviceReqs
                    .Where(o => o.IS_NO_EXECUTE != FlagConstant.IS_TRUE)
                    .GroupBy(o => o.ID)
                    .ToDictionary(o => o.Key, o => o.First());

                Dictionary<long, HIS_SERVICE_REQ> dicServiceReqByExpMest = new Dictionary<long, HIS_SERVICE_REQ>();
                foreach (var expMest in expMests)
                {
                    HIS_SERVICE_REQ serviceReq = null;
                    if (expMest.SERVICE_REQ_ID.HasValue && dicServiceReq.TryGetValue(expMest.SERVICE_REQ_ID.Value, out serviceReq))
                        dicServiceReqByExpMest[expMest.ID] = serviceReq;
                }

                List<V_HIS_SERE_SERV> noExecuteSereServs = sereServs != null
                    ? sereServs.Where(o => o.IS_NO_EXECUTE == FlagConstant.IS_TRUE).ToList()
                    : new List<V_HIS_SERE_SERV>();
                HashSet<long> noExecuteMedicineIds = new HashSet<long>(noExecuteSereServs.Where(o => o.EXP_MEST_MEDICINE_ID.HasValue).Select(o => o.EXP_MEST_MEDICINE_ID.Value));
                HashSet<long> noExecuteMaterialIds = new HashSet<long>(noExecuteSereServs.Where(o => o.EXP_MEST_MATERIAL_ID.HasValue).Select(o => o.EXP_MEST_MATERIAL_ID.Value));
                HashSet<long> noExecuteBloodIds = new HashSet<long>(noExecuteSereServs.Where(o => o.BLOOD_ID.HasValue).Select(o => o.BLOOD_ID.Value));

                List<ExpMestMediAndMateADO> rows = new List<ExpMestMediAndMateADO>();
                if (option.IsMedicine && medicines != null)
                {
                    rows.AddRange(BuildMedicineRows(medicines.Where(o => !noExecuteMedicineIds.Contains(o.ID)).ToList(), dicServiceReqByExpMest, option.IsTachHDSD));
                }
                if ((option.IsMaterial || option.IsChemical) && materials != null)
                {
                    List<ExpMestMediAndMateADO> materialRows = BuildMaterialRows(materials.Where(o => !noExecuteMaterialIds.Contains(o.ID)).ToList(), dicServiceReqByExpMest);
                    rows.AddRange(materialRows.Where(o => o.IS_CHEMICAL_SUBSTANCE == FlagConstant.IS_TRUE ? option.IsChemical : option.IsMaterial));
                }
                if (option.IsBlood && bloods != null)
                {
                    rows.AddRange(BuildBloodRows(bloods.Where(o => !noExecuteBloodIds.Contains(o.BLOOD_ID)).ToList(), dicServiceReqByExpMest));
                }
                if (!option.IsExpend)
                {
                    rows = rows.Where(o => o.IS_EXPEND != FlagConstant.IS_TRUE).ToList();
                }

                foreach (var group in rows.GroupBy(o => o.TreatmentId))
                {
                    result[group.Key] = group.ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private static List<ExpMestMediAndMateADO> BuildMedicineRows(List<V_HIS_EXP_MEST_MEDICINE> medicines, Dictionary<long, HIS_SERVICE_REQ> dicServiceReqByExpMest, bool isTachHDSD)
        {
            List<ExpMestMediAndMateADO> result = new List<ExpMestMediAndMateADO>();
            var groups = medicines.GroupBy(o => new
            {
                o.MEDICINE_TYPE_ID,
                TUTORIAL = isTachHDSD ? o.TUTORIAL : null,
                o.MEDICINE_ID,
                o.EXP_MEST_ID,
                o.IS_EXPEND,
                o.REQ_DEPARTMENT_ID,
                o.PATIENT_TYPE_ID
            });
            foreach (var group in groups)
            {
                V_HIS_EXP_MEST_MEDICINE first = group.First();
                HIS_SERVICE_REQ serviceReq = null;
                if (!first.EXP_MEST_ID.HasValue || !dicServiceReqByExpMest.TryGetValue(first.EXP_MEST_ID.Value, out serviceReq))
                    continue;

                ExpMestMediAndMateADO row = new ExpMestMediAndMateADO();
                Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMediAndMateADO>(row, first);
                SetServiceReqInfo(row, serviceReq);
                row.TYPE = (long)DisclosureRowType.Medicine;
                row.IS_EXPEND = first.IS_EXPEND;
                row.Service_Type_Id = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
                row.AMOUNT = group.Sum(o => o.AMOUNT - (o.TH_AMOUNT ?? 0));
                result.Add(row);
            }
            return result;
        }

        private static List<ExpMestMediAndMateADO> BuildMaterialRows(List<V_HIS_EXP_MEST_MATERIAL> materials, Dictionary<long, HIS_SERVICE_REQ> dicServiceReqByExpMest)
        {
            List<ExpMestMediAndMateADO> result = new List<ExpMestMediAndMateADO>();
            var groups = materials.GroupBy(o => new
            {
                o.MATERIAL_TYPE_ID,
                o.MATERIAL_ID,
                o.EXP_MEST_ID,
                o.IS_CHEMICAL_SUBSTANCE,
                o.IS_EXPEND,
                o.REQ_DEPARTMENT_ID,
                o.PATIENT_TYPE_ID
            });
            foreach (var group in groups)
            {
                V_HIS_EXP_MEST_MATERIAL first = group.First();
                HIS_SERVICE_REQ serviceReq = null;
                if (!first.EXP_MEST_ID.HasValue || !dicServiceReqByExpMest.TryGetValue(first.EXP_MEST_ID.Value, out serviceReq))
                    continue;

                ExpMestMediAndMateADO row = new ExpMestMediAndMateADO();
                Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMediAndMateADO>(row, first);
                row.MEDICINE_TYPE_NAME = first.MATERIAL_TYPE_NAME;
                row.MEDICINE_TYPE_CODE = first.MATERIAL_TYPE_CODE;
                row.MEDICINE_TYPE_ID = first.MATERIAL_TYPE_ID;
                row.MEDICINE_ID = first.MATERIAL_ID;
                SetServiceReqInfo(row, serviceReq);
                row.Service_Type_Id = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT;
                row.AMOUNT = group.Sum(o => o.AMOUNT - (o.TH_AMOUNT ?? 0));
                row.IS_EXPEND = first.IS_EXPEND;
                row.IS_CHEMICAL_SUBSTANCE = first.IS_CHEMICAL_SUBSTANCE;
                row.TYPE = first.IS_CHEMICAL_SUBSTANCE == FlagConstant.IS_TRUE
                    ? (long)DisclosureRowType.Chemical
                    : (long)DisclosureRowType.Material;
                result.Add(row);
            }
            return result;
        }

        private static List<ExpMestMediAndMateADO> BuildBloodRows(List<V_HIS_EXP_MEST_BLOOD> bloods, Dictionary<long, HIS_SERVICE_REQ> dicServiceReqByExpMest)
        {
            List<ExpMestMediAndMateADO> result = new List<ExpMestMediAndMateADO>();
            var groups = bloods.GroupBy(o => new
            {
                o.BLOOD_ID,
                o.EXP_MEST_ID,
                o.REQ_DEPARTMENT_ID,
                o.PATIENT_TYPE_ID
            });
            foreach (var group in groups)
            {
                V_HIS_EXP_MEST_BLOOD first = group.First();
                HIS_SERVICE_REQ serviceReq = null;
                if (!dicServiceReqByExpMest.TryGetValue(first.EXP_MEST_ID, out serviceReq))
                    continue;

                ExpMestMediAndMateADO row = new ExpMestMediAndMateADO();
                Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMediAndMateADO>(row, first);
                row.AMOUNT = group.Count();
                row.PRICE = first.IMP_PRICE;
                row.MEDICINE_TYPE_ID = first.BLOOD_TYPE_ID;
                row.MEDICINE_ID = first.BLOOD_ID;
                row.MEDICINE_TYPE_CODE = first.BLOOD_TYPE_CODE;
                row.MEDICINE_TYPE_NAME = first.BLOOD_TYPE_NAME;
                SetServiceReqInfo(row, serviceReq);
                row.Service_Type_Id = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU;
                row.TYPE = (long)DisclosureRowType.Blood;
                result.Add(row);
            }
            return result;
        }

        /// <summary>
        /// Date of the row = day of USE_TIME of the prescription, INTRUCTION_DATE when USE_TIME is empty
        /// </summary>
        private static void SetServiceReqInfo(ExpMestMediAndMateADO row, HIS_SERVICE_REQ serviceReq)
        {
            row.INTRUCTION_DATE = serviceReq.USE_TIME.HasValue
                ? (serviceReq.USE_TIME.Value / 1000000) * 1000000
                : serviceReq.INTRUCTION_DATE;
            row.INTRUCTION_TIME = serviceReq.USE_TIME ?? serviceReq.INTRUCTION_TIME;
            row.REQ_DEPARTMENT_ID = serviceReq.REQUEST_DEPARTMENT_ID;
            row.TreatmentId = serviceReq.TREATMENT_ID;
        }
    }
}
