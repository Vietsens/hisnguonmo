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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.ADO
{
    public class ImportADO
    {
        public List<ServiceImportADO> serviceAdos { get; set; }
        public List<MaterialTypeImportADO> materialAdos { get; set; }
        public List<MedicineTypeImportADO> medicineAdos { get; set; }
        public List<MaterialPatyImportADO> materialPatyAdos { get; set; }
        public List<MedicinePatyImportADO> medicinePatyAdos { get; set; }
        public List<ServicePatyImportADO> servicePatyAdos { get; set; }
    }

    public class ServiceImportADO : V_HIS_SERVICE
    {
        public string CPNG { get; set; }
        public string MultiRequest { get; set; }
        public string ICD_CM_CODE { get; set; }
        public string PTTT_GROUP_CODE { get; set; }
        public string PTTT_METHOD_CODE { get; set; }
        public string PACKAGE_CODE { get; set; }
        public string BILL_PATIENT_TYPE_CODE { get; set; }
        public string HEIN_LIMIT_PRICE_IN_TIME_STR { get; set; }
        public string HEIN_LIMIT_PRICE_INTR_TIME_STR { get; set; }
        public string BILL_OPTION_STR { get; set; }
        public string PARENT_CODE { get; set; }

        public string ERROR { get; set; }

        public ServiceImportADO()
        {
        }

        public ServiceImportADO(V_HIS_SERVICE data)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<ServiceImportADO>(this, data);
        }
    }

    public class MaterialTypeImportADO : V_HIS_MATERIAL_TYPE
    {
        public string CHEMICAL_SUBSTANCE { get; set; }
        public string ALLOW_EXPORT_ODD { get; set; }
        public string ALLOW_ODD { get; set; }
        public string AUTO_EXPEND { get; set; }
        public string BUSINESS { get; set; }
        public string IN_KTC_FEE { get; set; }
        public string OUT_PARENT_FEE { get; set; }
        public string REQUIRE_HSD { get; set; }
        public string SALE_EQUAL_IMP_PRICE { get; set; }
        public string STENT { get; set; }
        public string STOP_IMP { get; set; }
        public decimal? COGS { get; set; }
        public string HEIN_LIMIT_PRICE_IN_TIME_STR { get; set; }
        public string HEIN_LIMIT_PRICE_INTR_TIME_STR { get; set; }
        public string PARENT_CODE { get; set; }

        public string ERROR { get; set; }

        public MaterialTypeImportADO()
        {
        }

        public MaterialTypeImportADO(V_HIS_MATERIAL_TYPE data)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<MaterialTypeImportADO>(this, data);
        }

        public long PACKING_TYPE_ID { get; set; }

        public string PACKING_TYPE_CODE { get; set; }
    }

    public class MedicineTypeImportADO : V_HIS_MEDICINE_TYPE
    {
        public string ADDICTIVE { get; set; }
        public string ALLOW_EXPORT_ODD { get; set; }
        public string ALLOW_ODD { get; set; }
        public string ANTIBIOTIC { get; set; }
        public string BUSINESS { get; set; }
        public string FUNCTIONAL_FOOD { get; set; }
        public string OUT_PARENT_FEE { get; set; }
        public string REQUIRE_HSD { get; set; }
        public string SALE_EQUAL_IMP_PRICE { get; set; }
        public string NEUROLOGICAL { get; set; }
        public string STOP_IMP { get; set; }
        public decimal? COGS { get; set; }
        public string STAR_MARK { get; set; }
        public string HEIN_LIMIT_PRICE_IN_TIME_STR { get; set; }
        public string HEIN_LIMIT_PRICE_INTR_TIME_STR { get; set; }
        public string PARENT_CODE { get; set; }

        public string ERROR { get; set; }

        public MedicineTypeImportADO()
        {
        }

        public MedicineTypeImportADO(V_HIS_MEDICINE_TYPE data)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<MedicineTypeImportADO>(this, data);
        }

        public long PACKING_TYPE_ID { get; set; }

        public string PACKING_TYPE_CODE { get; set; }

        public int IS_NEUROLOGICAL { get; set; }

        public int IS_ANTIBIOTIC { get; set; }

        public int IS_ADDICTIVE { get; set; }
    }

    public class MedicinePatyImportADO : V_HIS_MEDICINE_PATY
    {
        public string EXP_PRICE_STR { get; set; }
        public string EXP_VAT_RATIO_STR { get; set; }

        public string ERROR { get; set; }

        public MedicinePatyImportADO()
        {
        }

        public MedicinePatyImportADO(V_HIS_MEDICINE_PATY data)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<MedicinePatyImportADO>(this, data);
        }
    }

    public class MaterialPatyImportADO : V_HIS_MATERIAL_PATY
    {
        public string EXP_PRICE_STR { get; set; }
        public string EXP_VAT_RATIO_STR { get; set; }

        public string ERROR { get; set; }

        public MaterialPatyImportADO()
        {
        }

        public MaterialPatyImportADO(V_HIS_MATERIAL_PATY data)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<MaterialPatyImportADO>(this, data);
        }
    }

    public class ServicePatyImportADO : V_HIS_SERVICE_PATY
    {
        public string EXP_PRICE_STR { get; set; }
        public string EXP_VAT_RATIO_STR { get; set; }
        public string TREATMENT_FROM_TIME_STR { get; set; }
        public string TREATMENT_TO_TIME_STR { get; set; }
        public string FROM_TIME_STR { get; set; }
        public string TO_TIME_STR { get; set; }
        public string EXECUTE_ROOM_CODES { get; set; }
        public string REQUEST_DEPARMENT_CODES { get; set; }
        public string REQUEST_ROOM_CODES { get; set; }
        
        public string ERROR { get; set; }

        public ServicePatyImportADO()
        {
        }

        public ServicePatyImportADO(V_HIS_SERVICE_PATY data)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<ServicePatyImportADO>(this, data);
        }
    }
    
}
