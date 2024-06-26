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

using MPS;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ADO;

namespace MPS.Core.Mps000062
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000062RDO : RDOBase
    {
        internal V_HIS_TREATMENT currentTreatment { get; set; }
        internal List<HIS_TRACKING> lstTracking { get; set; }
        internal List<HIS_DHST> lstDHST { get; set; }
        internal List<V_HIS_EXP_MEST_MEDICINE> lstExpMestMedicine { get; set; }
        internal List<V_HIS_SERE_SERV_7> lstSereServCLS { get; set; }
        internal List<V_HIS_PRESCRIPTION> lstPrescription { get; set; }
        internal List<HIS_ICD> lstIcdId { get; set; }
        internal List<HIS_CARE> lstHisCare { get; set; }
        internal List<V_HIS_CARE_DETAIL> lstHisCareDetail { get; set; }
        internal List<HIS_SERVICE_REQ> lstServiceReq { get; set; }
        internal MOS.SDO.WorkPlaceSDO workPlaceSDO { get; set; }
        public long keyVienTim { get; set; }
        public List<V_HIS_EXP_MEST_MATERIAL> listVHisMaterial { get; set; }

        public Mps000062RDO(
            V_HIS_TREATMENT currentTreatment,
            List<HIS_TRACKING> lstTracking,
            List<HIS_DHST> lstDHST,
            List<V_HIS_PRESCRIPTION> lstPrescription,
            List<V_HIS_EXP_MEST_MEDICINE> lstExpMestMedicine,
            List<V_HIS_SERE_SERV_7> lstSereServCLS,
            List<HIS_ICD> lstIcdId,
            List<HIS_CARE> lstHisCare,
            List<V_HIS_CARE_DETAIL> lstHisCareDetail,
            List<HIS_SERVICE_REQ> lstServiceReq,
            MOS.SDO.WorkPlaceSDO workPlaceSDO,
            long keyVienTim,
            List<V_HIS_EXP_MEST_MATERIAL> listVHisMaterial
            )
        {
            try
            {
                this.currentTreatment = currentTreatment;
                this.lstTracking = lstTracking;
                this.lstDHST = lstDHST;
                this.lstPrescription = lstPrescription;
                this.lstExpMestMedicine = lstExpMestMedicine;
                this.lstSereServCLS = lstSereServCLS;
                this.lstIcdId = lstIcdId;
                this.lstHisCare = lstHisCare;
                this.lstHisCareDetail = lstHisCareDetail;
                this.lstServiceReq = lstServiceReq;
                this.workPlaceSDO = workPlaceSDO;
                this.keyVienTim = keyVienTim;
                this.listVHisMaterial = listVHisMaterial;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal Dictionary<long, V_HIS_PRESCRIPTION> dicPrescription { get; set; }
        internal Dictionary<long, List<V_HIS_EXP_MEST_MEDICINE>> dicVHisExpMestMedicine { get; set; }
        internal Dictionary<long, List<V_HIS_EXP_MEST_MATERIAL>> dicVHisExpMestMaterial { get; set; }
        public Mps000062RDO(
            V_HIS_TREATMENT currentTreatment,
            List<HIS_TRACKING> lstTracking,
            List<HIS_DHST> lstDHST,
            Dictionary<long, V_HIS_PRESCRIPTION> dicPrescription,
            Dictionary<long, List<V_HIS_EXP_MEST_MEDICINE>> dicVHisExpMestMedicine,
            Dictionary<long, List<V_HIS_EXP_MEST_MATERIAL>> dicVHisExpMestMaterial,
            List<V_HIS_SERE_SERV_7> lstSereServCLS,
            List<HIS_ICD> lstIcdId,
            List<HIS_CARE> lstHisCare,
            List<V_HIS_CARE_DETAIL> lstHisCareDetail,
            List<HIS_SERVICE_REQ> lstServiceReq,
            MOS.SDO.WorkPlaceSDO workPlaceSDO,
            long keyVienTim
            )
        {
            try
            {
                this.currentTreatment = currentTreatment;
                this.lstTracking = lstTracking;
                this.lstDHST = lstDHST;
                this.dicPrescription = dicPrescription;
                this.dicVHisExpMestMedicine = dicVHisExpMestMedicine;
                this.dicVHisExpMestMaterial = dicVHisExpMestMaterial;
                this.lstSereServCLS = lstSereServCLS;
                this.lstIcdId = lstIcdId;
                this.lstHisCare = lstHisCare;
                this.lstHisCareDetail = lstHisCareDetail;
                this.lstServiceReq = lstServiceReq;
                this.workPlaceSDO = workPlaceSDO;
                this.keyVienTim = keyVienTim;
                if (keyVienTim == 1)
                {
                    if (this.dicPrescription != null && this.dicPrescription.Count > 0)
                    {
                        this.lstPrescription = this.dicPrescription.Select(p => p.Value).ToList();
                    }
                    if (this.dicVHisExpMestMedicine != null && this.dicVHisExpMestMedicine.Count > 0)
                    {
                        foreach (var item in this.dicVHisExpMestMedicine.Select(p => p.Value).ToList())
                        {
                            this.lstExpMestMedicine.AddRange(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal override void SetSingleKey()
        {
            try
            {
                if (currentTreatment != null)
                {
                    keyValues.Add(new KeyValue(Mps000062ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(currentTreatment.DOB)));
                }
                keyValues.Add(new KeyValue(Mps000062ExtendSingleKey.DEPARTMENT_NAME, workPlaceSDO.DepartmentName));
                keyValues.Add(new KeyValue(Mps000062ExtendSingleKey.ROOM_NAME, workPlaceSDO.RoomName));
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TREATMENT>(currentTreatment, keyValues);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
