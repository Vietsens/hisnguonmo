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

namespace HIS.Desktop.Plugins.TrackingCreate.ADO
{
    public class TreeSereServADO : HIS_SERE_SERV
    {
        public string CONCRETE_ID__IN_SETY { get; set; }
        public string PARENT_ID__IN_SETY { get; set; }
        public string SERVICE_REQ_CODE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public bool IS_THUHOI { get; set; }
        public bool IS_OUT_MEDI_MATE { get; set; }
        public bool IsNotShowMediAndMate {get; set; }
        public bool IsNotShowOutMediAndMate { get; set; }
        public long? NUM_ORDER { get; set; }
        public short? IS_EXECUTE_KIDNEY_PRES { get; set; }
        public short? PRESCRIPTION_TYPE_ID { get; set; }
        public string REQUEST_LOGINNAME { get; set; }
        public long SERVICE_REQ_STT_ID { get; set; }
        public short? IS_NO_EXECUTE { get; set; }
        public long TDL_PATIENT_DOB { get; set; }
        public string TDL_PATIENT_NAME { get; set; }
        public string TDL_PATIENT_GENDER_NAME { get; set; }
        public long LEVER { get; set; }
        public bool IS_RATION { get; set; }
        public long? TDL_USE_TIME { get; set; }
        public long? TDL_INTRUCTION_DATE { get; set; }
        public bool IsMedicinePreventive { get; set; }
        public long? TDL_TRACKING_ID { get; set; }
        public long? USED_FOR_TRACKING_ID { get; set; }
        public bool IsServiceUseForTracking { get; set; }
        public short? IS_TEMPORARY_PRES { get; set; }
        public short? IS_DISABLE { get; set; }
        public TAB_TYPE? tabType { get; set; }
        public enum TAB_TYPE
		{
            TAB_1,
            TAB_2
		}
        public TreeSereServADO() { }

        public TreeSereServADO(HIS_SERE_SERV data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<TreeSereServADO>(this, data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
