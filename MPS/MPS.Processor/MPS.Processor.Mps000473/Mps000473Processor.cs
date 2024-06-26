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
using MPS.Processor.Mps000473.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000473
{
    public partial class Mps000473Processor : AbstractProcessor
    {
        Mps000473PDO rdo;
        public Mps000473Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            if (rdoBase != null && rdoBase is Mps000473PDO)
            {
                rdo = (Mps000473PDO)rdoBase;
            }
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Mps000473PDO rdo", rdo));

                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));
                SetSingleKey();
                this.SetSignatureKeyImageByCFG();
                singleTag.ProcessData(store, singleValueDictionary);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        //private void ProcessDataPrint()
        //{
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}

        private void SetSingleKey()
        {
            try
            {
                AddObjectKeyIntoListkey<HIS_PATIENT>(rdo.patient ?? new HIS_PATIENT(), false);
                SetSingleKey(new KeyValue(Mps000473ExtendSingleKey.GENDER, rdo.patient.GENDER_ID));
                AddObjectKeyIntoListkey<HIS_PATIENT_TYPE_ALTER>(rdo.patientAlter ?? new HIS_PATIENT_TYPE_ALTER(), false);
                if (rdo.treatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000473ExtendSingleKey.IN_TIME, rdo.treatment.IN_TIME));
                    SetSingleKey(new KeyValue(Mps000473ExtendSingleKey.ICD_CODE_TREATMENT, rdo.treatment.ICD_CODE));
                    SetSingleKey(new KeyValue(Mps000473ExtendSingleKey.ICD_NAME_TREATMENT, rdo.treatment.ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000473ExtendSingleKey.ICD_SUB_CODE_TREATMENT, rdo.treatment.ICD_SUB_CODE));
                    SetSingleKey(new KeyValue(Mps000473ExtendSingleKey.ICD_TEXT_TREATMENT, rdo.treatment.ICD_TEXT));
                }
                if (rdo.bedLog != null)
                {
                    SetSingleKey(new KeyValue(Mps000473ExtendSingleKey.BED_NAME, rdo.bedLog.BED_NAME));
                    SetSingleKey(new KeyValue(Mps000473ExtendSingleKey.BED_CODE, rdo.bedLog.BED_CODE));
                    SetSingleKey(new KeyValue(Mps000473ExtendSingleKey.BED_ROOM_CODE, rdo.bedLog.BED_ROOM_CODE));
                    SetSingleKey(new KeyValue(Mps000473ExtendSingleKey.BED_ROOM_NAME, rdo.bedLog.BED_ROOM_NAME));
                }
                if (rdo.serviceReq != null)
                {
                    SetSingleKey(new KeyValue(Mps000473ExtendSingleKey.ICD_CODE_REQ, rdo.serviceReq.ICD_CODE));
                    SetSingleKey(new KeyValue(Mps000473ExtendSingleKey.ICD_NAME_REQ, rdo.serviceReq.ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000473ExtendSingleKey.ICD_SUB_CODE_REQ, rdo.serviceReq.ICD_SUB_CODE));
                    SetSingleKey(new KeyValue(Mps000473ExtendSingleKey.ICD_TEXT_REQ, rdo.serviceReq.ICD_TEXT));
                }
                SetSingleKey(new KeyValue("LOGGIN_NAME", rdo.loginName));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
