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
using MPS.Processor.Mps000108.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000108
{
    public class Mps000108Processor : AbstractProcessor
    {
        Mps000108PDO rdo;

        public Mps000108Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000108PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                SetSingleKey();
                SetBarcodeKey();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                objectTag.AddObjectData(store, "ExpMestBlood", rdo.HisExpMestBltys);
                objectTag.AddObjectData(store, "ExpMestBltys", rdo.HisExpMestBltys);
                objectTag.AddObjectData(store, "ViewExpMestBlood", rdo.ExpMestBloodList);
                objectTag.AddObjectData(store, "Services", rdo.sereServ1s);
                objectTag.AddRelationship(store, "ExpMestBltys", "ViewExpMestBlood", "ID", "EXP_MEST_BLTY_REQ_ID");

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        internal void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode expMestCodeBar = new Inventec.Common.BarcodeLib.Barcode(rdo.ExpMest.EXP_MEST_CODE);
                expMestCodeBar.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                expMestCodeBar.IncludeLabel = false;
                expMestCodeBar.Width = 120;
                expMestCodeBar.Height = 40;
                expMestCodeBar.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                expMestCodeBar.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                expMestCodeBar.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                expMestCodeBar.IncludeLabel = true;

                dicImage.Add(Mps000108ExtendSingleKey.EXP_MEST_CODE_BARCODE, expMestCodeBar);

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
                if (rdo.ServiceReq != null)
                {
                    SetSingleKey(new KeyValue(Mps000108ExtendSingleKey.ICD_MAIN_TEXT, rdo.ServiceReq.ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000108ExtendSingleKey.DOB_YEAR_STR, rdo.ServiceReq.TDL_PATIENT_DOB.ToString().Substring(0, 4)));

                    if (rdo.Treatment == null) rdo.Treatment = new V_HIS_TREATMENT();

                    string Address = !String.IsNullOrEmpty(rdo.ServiceReq.TDL_PATIENT_ADDRESS) ?
                        rdo.ServiceReq.TDL_PATIENT_ADDRESS :
                        rdo.Treatment.TDL_PATIENT_ADDRESS;
                    SetSingleKey(new KeyValue(Mps000108ExtendSingleKey.ADDRESS, Address));
                    SetSingleKey(new KeyValue(Mps000108ExtendSingleKey.VIR_ADDRESS, Address));

                    string career = !String.IsNullOrEmpty(rdo.ServiceReq.TDL_PATIENT_CAREER_NAME) ?
                        rdo.ServiceReq.TDL_PATIENT_CAREER_NAME :
                        rdo.Treatment.TDL_PATIENT_CAREER_NAME;
                    SetSingleKey(new KeyValue(Mps000108ExtendSingleKey.CAREER_NAME, career));

                    string code = !String.IsNullOrEmpty(rdo.ServiceReq.TDL_PATIENT_CODE) ?
                        rdo.ServiceReq.TDL_PATIENT_CODE :
                        rdo.Treatment.TDL_PATIENT_CODE;
                    SetSingleKey(new KeyValue(Mps000108ExtendSingleKey.PATIENT_CODE, code));

                    long dob = rdo.ServiceReq.TDL_PATIENT_DOB > 0 ?
                        rdo.ServiceReq.TDL_PATIENT_DOB :
                        rdo.Treatment.TDL_PATIENT_DOB;
                    SetSingleKey(new KeyValue(Mps000108ExtendSingleKey.DOB, dob > 0 ? dob : 00000000000000));

                    string gender = !String.IsNullOrEmpty(rdo.ServiceReq.TDL_PATIENT_GENDER_NAME) ?
                        rdo.ServiceReq.TDL_PATIENT_GENDER_NAME :
                        rdo.Treatment.TDL_PATIENT_GENDER_NAME;
                    SetSingleKey(new KeyValue(Mps000108ExtendSingleKey.GENDER_NAME, gender));

                    string rank = !String.IsNullOrEmpty(rdo.ServiceReq.TDL_PATIENT_MILITARY_RANK_NAME) ?
                        rdo.ServiceReq.TDL_PATIENT_MILITARY_RANK_NAME :
                        rdo.Treatment.TDL_PATIENT_MILITARY_RANK_NAME;
                    SetSingleKey(new KeyValue(Mps000108ExtendSingleKey.MILITARY_RANK_NAME, rank));

                    string name = !String.IsNullOrEmpty(rdo.ServiceReq.TDL_PATIENT_NAME) ?
                        rdo.ServiceReq.TDL_PATIENT_NAME :
                        rdo.Treatment.TDL_PATIENT_NAME;
                    SetSingleKey(new KeyValue(Mps000108ExtendSingleKey.VIR_PATIENT_NAME, name));

                    string qg = !String.IsNullOrEmpty(rdo.ServiceReq.TDL_PATIENT_NATIONAL_NAME) ?
                        rdo.ServiceReq.TDL_PATIENT_NATIONAL_NAME :
                        rdo.Treatment.TDL_PATIENT_NATIONAL_NAME;
                    SetSingleKey(new KeyValue(Mps000108ExtendSingleKey.NATIONAL_NAME, qg));

                    string work = !String.IsNullOrEmpty(rdo.ServiceReq.TDL_PATIENT_WORK_PLACE) ?
                        rdo.ServiceReq.TDL_PATIENT_WORK_PLACE :
                        rdo.Treatment.TDL_PATIENT_WORK_PLACE;
                    SetSingleKey(new KeyValue(Mps000108ExtendSingleKey.WORK_PLACE, work));

                    string work_name = !String.IsNullOrEmpty(rdo.ServiceReq.TDL_PATIENT_WORK_PLACE_NAME) ?
                        rdo.ServiceReq.TDL_PATIENT_WORK_PLACE_NAME :
                        rdo.Treatment.TDL_PATIENT_WORK_PLACE_NAME;
                    SetSingleKey(new KeyValue(Mps000108ExtendSingleKey.WORK_PLACE_NAME, work_name));

                    AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.ServiceReq, false);
                }

                if (rdo.Treatment != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.Treatment, false);
                }

                if (rdo.ExpMest != null)
                {
                    SetSingleKey(new KeyValue(Mps000108ExtendSingleKey.EXP_MEST_CODE, rdo.ExpMest.EXP_MEST_CODE));
                }
                if (rdo.treatmentBedRooms != null && rdo.treatmentBedRooms.Count() > 0)
                {
                    SetSingleKey(new KeyValue(Mps000108ExtendSingleKey.BED_CODE, string.Join(";", rdo.treatmentBedRooms.Select(o => o.BED_CODE).ToList().Distinct())));
                    SetSingleKey(new KeyValue(Mps000108ExtendSingleKey.BED_NAME, string.Join(";", rdo.treatmentBedRooms.Select(o => o.BED_NAME).ToList().Distinct())));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
