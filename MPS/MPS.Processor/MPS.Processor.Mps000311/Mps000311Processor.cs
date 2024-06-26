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
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using MPS.ProcessorBase.Core;
using MPS.Processor.Mps000311.PDO;
using Inventec.Core;
using MPS.ProcessorBase;
using System.Text;
using System.Linq;

namespace MPS.Processor.Mps000311
{
    class Mps000311Processor : AbstractProcessor
    {
        Mps000311PDO rdo;
        public Mps000311Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000311PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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
                SetBarcodeKey();
                SetSingleKey();
                ProcessListSereServ();
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "SereServFollow", rdo.SereServFollows);


                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void ProcessListSereServ()
        {
            try
            {
                foreach (var item in rdo.SereServFollows)
                {
                    item.SERVICE_UNIT_NAME = (rdo.ServiceUnit.FirstOrDefault(o => o.ID == item.TDL_SERVICE_UNIT_ID) ?? new MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT()).SERVICE_UNIT_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetSingleKey()
        {
            try
            {
                SetSingleKey(new KeyValue(Mps000311ExtendSingleKey.PATIENT_CODE, rdo.Treatment.TDL_PATIENT_CODE));
                SetSingleKey(new KeyValue(Mps000311ExtendSingleKey.TREATMENT_CODE, rdo.Treatment.TREATMENT_CODE));
                SetSingleKey(new KeyValue(Mps000311ExtendSingleKey.DOB_YEAR, rdo.Treatment.TDL_PATIENT_DOB.ToString().Substring(0, 4)));
                SetSingleKey(new KeyValue(Mps000311ExtendSingleKey.GENDER_NAME, rdo.Treatment.TDL_PATIENT_GENDER_NAME));
                if (!string.IsNullOrEmpty(rdo.SereServ.HEIN_CARD_NUMBER))
                {
                    SetSingleKey(new KeyValue(Mps000311ExtendSingleKey.HEIN_CARD_NUMBER_1, rdo.SereServ.HEIN_CARD_NUMBER.Substring(0, 2)));
                    SetSingleKey(new KeyValue(Mps000311ExtendSingleKey.HEIN_CARD_NUMBER_2, rdo.SereServ.HEIN_CARD_NUMBER.Substring(2, 1)));
                    SetSingleKey(new KeyValue(Mps000311ExtendSingleKey.HEIN_CARD_NUMBER_3, rdo.SereServ.HEIN_CARD_NUMBER.Substring(3, 2)));
                    SetSingleKey(new KeyValue(Mps000311ExtendSingleKey.HEIN_CARD_NUMBER_4, rdo.SereServ.HEIN_CARD_NUMBER.Substring(5, 2)));
                    SetSingleKey(new KeyValue(Mps000311ExtendSingleKey.HEIN_CARD_NUMBER_5, rdo.SereServ.HEIN_CARD_NUMBER.Substring(7, 3)));
                    SetSingleKey(new KeyValue(Mps000311ExtendSingleKey.HEIN_CARD_NUMBER_6, rdo.SereServ.HEIN_CARD_NUMBER.Substring(10, 5)));
                }
                SetSingleKey(new KeyValue(Mps000311ExtendSingleKey.ICD_NAME, rdo.Treatment.ICD_NAME));
                SetSingleKey(new KeyValue(Mps000311ExtendSingleKey.PATIENT_TYPE_NAME, (rdo.PatientType.FirstOrDefault(o => o.ID == rdo.SereServ.PATIENT_TYPE_ID) ?? new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE()).PATIENT_TYPE_NAME));
                SetSingleKey(new KeyValue(Mps000311ExtendSingleKey.REQUEST_DEPARTMENT_NAME, (rdo.Room.FirstOrDefault(o => o.ID == rdo.SereServ.TDL_REQUEST_ROOM_ID) ?? new MOS.EFMODEL.DataModels.V_HIS_ROOM()).DEPARTMENT_NAME));
                SetSingleKey(new KeyValue(Mps000311ExtendSingleKey.REQUEST_ROOM_NAME, (rdo.Room.FirstOrDefault(o => o.ID == rdo.SereServ.TDL_REQUEST_ROOM_ID) ?? new MOS.EFMODEL.DataModels.V_HIS_ROOM()).ROOM_NAME));
                SetSingleKey(new KeyValue(Mps000311ExtendSingleKey.TDL_SERVICE_NAME, rdo.SereServ.TDL_SERVICE_NAME));
                SetSingleKey(new KeyValue(Mps000311ExtendSingleKey.VIR_ADDRESS, rdo.Treatment.TDL_PATIENT_ADDRESS));
                SetSingleKey(new KeyValue(Mps000311ExtendSingleKey.VIR_PATIENT_NAME, rdo.Treatment.TDL_PATIENT_NAME));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
