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
using MPS.Processor.Mps000206.PDO;
using MPS.ProcessorBase.Core;
using SCN.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000206
{
    class Mps000206Processor : AbstractProcessor
    {
        Mps000206PDO rdo;

        public Mps000206Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000206PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Processor();
                SetSingleKey();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "ServiceReqTest", rdo._SereServTests);
                objectTag.AddObjectData(store, "Allergic", rdo._ScnAllergic);
                objectTag.AddObjectData(store, "Disability", rdo._ScnDisability);
                objectTag.AddObjectData(store, "Disease", rdo._ScnDisease);
                objectTag.AddObjectData(store, "HealthRisk", rdo._ScnHealthRisk);
                objectTag.AddObjectData(store, "Surgery", rdo._ScnSurgery);

               

                objectTag.AddObjectData(store, "DiseaseTypeRelatives", rdo._ScnDiseaseTypeRelatives);
                objectTag.AddObjectData(store, "AllergicTypeRelatives", rdo._ScnAllergicTypeRelatives);
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        
        void Processor()
        {
            try
            {
                //if (rdo._ScnVaccination != null && rdo._ScnVaccination.Count > 0 && rdo._ScnVaccinationTypes != null && rdo._ScnVaccinationTypes.Count > 0)
                //{
                //    var typeGroup = rdo._ScnVaccinationTypes.GroupBy(p => p.VACCINATION_GROUP_ID).Select(p => p.ToList()).ToList();
                //    for (int i = 0; i < typeGroup.Count; i++)
                //    {
                //        List<long> typeIds = typeGroup[i].Select(p => p.ID).ToList();
                //        if (i == 0)
                //        {
                //            listVaccin1 = rdo._ScnVaccination.Where(p => typeIds.Contains(p.VACCINATION_TYPE_ID)).ToList();
                //        }
                //        else if (i == 1)
                //        {
                //            listVaccin2 = rdo._ScnVaccination.Where(p => typeIds.Contains(p.VACCINATION_TYPE_ID)).ToList();
                //        }
                //        else if (i == 2)
                //        {
                //            listVaccin3 = rdo._ScnVaccination.Where(p => typeIds.Contains(p.VACCINATION_TYPE_ID)).ToList();
                //        }
                //        else if (i == 3)
                //        {
                //            listVaccin4 = rdo._ScnVaccination.Where(p => typeIds.Contains(p.VACCINATION_TYPE_ID)).ToList();
                //        }
                //        else if (i == 4)
                //        {
                //            listVaccin5 = rdo._ScnVaccination.Where(p => typeIds.Contains(p.VACCINATION_TYPE_ID)).ToList();
                //            break;
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void SetSingleKey()
        {
            try
            {
                AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo._ServiceReqExam, false);
                AddObjectKeyIntoListkey<HIS_DHST>(rdo._Dhst, false);
                AddObjectKeyIntoListkey<V_HIS_TREATMENT_4>(rdo._Treatment, false);
                AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo._Patient);
                // SetSingleKey(new KeyValue(Mps000206ExtendSingleKey.SUM_TOTAL_PRICE, totalPrice));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
