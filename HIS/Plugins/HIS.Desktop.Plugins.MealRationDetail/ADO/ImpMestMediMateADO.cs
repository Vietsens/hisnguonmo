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

namespace HIS.Desktop.Plugins.ApproveAggrImpMest.ADO
{
    public class ImpMestMediMateADO
    {
        public long MEDI_MATE_ID { get; set; }
        public long MEDI_MATE_TYPE_ID { get; set; }
        public string MEDI_MATE_TYPE_NAME { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public decimal AMOUNT { get; set; }
        public bool IsMedicine { get; set; }
        public bool IsMaterial { get; set; }
        public string TYPE { get; set; }
        public long? MEDICINE_GROUP_ID { get; set; }
        public long IMP_MEST_ID { get; set; }
        public decimal? CONVERT_RATIO { get; set; }
        public string CONVERT_UNIT_CODE { get; set; }
        public string CONVERT_UNIT_NAME { get; set; }

        public ImpMestMediMateADO(V_HIS_IMP_MEST_MEDICINE impMestMedicine)
        {
            try
            {
                this.IsMedicine = true;
                this.MEDI_MATE_ID = impMestMedicine.MEDICINE_ID;
                this.MEDI_MATE_TYPE_ID = impMestMedicine.MEDICINE_TYPE_ID;
                this.MEDI_MATE_TYPE_NAME = impMestMedicine.MEDICINE_TYPE_NAME;
                this.SERVICE_UNIT_NAME = impMestMedicine.SERVICE_UNIT_NAME;
                this.PACKAGE_NUMBER = impMestMedicine.PACKAGE_NUMBER;
                this.AMOUNT = impMestMedicine.AMOUNT;
                this.MEDICINE_GROUP_ID = impMestMedicine.MEDICINE_GROUP_ID;
                this.TYPE = "Thuốc";
                this.IMP_MEST_ID = impMestMedicine.IMP_MEST_ID;
                this.CONVERT_RATIO = impMestMedicine.CONVERT_RATIO;
                this.CONVERT_UNIT_CODE = impMestMedicine.CONVERT_UNIT_CODE;
                this.CONVERT_UNIT_NAME = impMestMedicine.CONVERT_UNIT_NAME;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public ImpMestMediMateADO(V_HIS_IMP_MEST_MATERIAL impMestMaterial)
        {
            try
            {
                this.IsMaterial = true;
                this.MEDI_MATE_ID = impMestMaterial.MATERIAL_ID;
                this.MEDI_MATE_TYPE_ID = impMestMaterial.MATERIAL_TYPE_ID;
                this.MEDI_MATE_TYPE_NAME = impMestMaterial.MATERIAL_TYPE_NAME;
                this.SERVICE_UNIT_NAME = impMestMaterial.SERVICE_UNIT_NAME;
                this.PACKAGE_NUMBER = impMestMaterial.PACKAGE_NUMBER;
                this.MEDICINE_GROUP_ID = null;
                this.AMOUNT = impMestMaterial.AMOUNT;
                this.TYPE = "Vật tư";
                this.IMP_MEST_ID = impMestMaterial.IMP_MEST_ID;
                this.CONVERT_RATIO = impMestMaterial.CONVERT_RATIO;
                this.CONVERT_UNIT_CODE = impMestMaterial.CONVERT_UNIT_CODE;
                this.CONVERT_UNIT_NAME = impMestMaterial.CONVERT_UNIT_NAME;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
