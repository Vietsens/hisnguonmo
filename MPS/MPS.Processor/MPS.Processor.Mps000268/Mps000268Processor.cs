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
using MPS.Processor.Mps000268.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000268
{
    public class Mps000268Processor : AbstractProcessor
    {
        Mps000268PDO rdo;
        public Mps000268Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000268PDO)rdoBase;
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
                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                SetSingleKey();
                SetImageKey();
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                store.SetCommonFunctions();

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
                AddObjectKeyIntoListkey(rdo.PatientView, false);
                if (rdo.ViewTreatment != null)
                {
                    AddObjectKeyIntoListkey(rdo.ViewTreatment, false);
                }
                else if (rdo.TreatmentView != null)
                {
                    AddObjectKeyIntoListkey(rdo.TreatmentView, false);
                }

                AddObjectKeyIntoListkey(rdo.ado, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        internal void SetImageKey()
        {
            try
            {
                bool isBhytAndAvtNull = true;
                if (rdo.ViewTreatment != null && !String.IsNullOrEmpty(rdo.ViewTreatment.TDL_PATIENT_AVATAR_URL))
                {
                    SetSingleImage(Mps000268ExtendSingleKey.IMG_AVATAR, rdo.ViewTreatment.TDL_PATIENT_AVATAR_URL);
                    isBhytAndAvtNull = false;
                }
                else if (rdo.TreatmentView != null && !String.IsNullOrEmpty(rdo.TreatmentView.TDL_PATIENT_AVATAR_URL))
                {
                    SetSingleImage(Mps000268ExtendSingleKey.IMG_AVATAR, rdo.TreatmentView.TDL_PATIENT_AVATAR_URL);
                    isBhytAndAvtNull = false;
                }

                if (isBhytAndAvtNull)
                {
                    SetSingleKey(Mps000268ExtendSingleKey.AVT_AND_BHYT_NULL, "1");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetSingleImage(string key, string imageUrl)
        {
            try
            {
                MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(imageUrl);
                if (stream != null)
                {
                    SetSingleKey(new KeyValue(key, stream.ToArray()));
                }
                else
                {
                    SetSingleKey(new KeyValue(key, ""));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                string treatmentCode = null;
                long? deathCertNum = null;
                if (rdo.TreatmentView != null)
                {
                    treatmentCode = rdo.TreatmentView.TREATMENT_CODE;
                    deathCertNum = rdo.TreatmentView.DEATH_CERT_NUM;
                }
                else if (rdo.ViewTreatment != null)
                {
                    treatmentCode = rdo.TreatmentView.TREATMENT_CODE;
                    deathCertNum = rdo.TreatmentView.DEATH_CERT_NUM;
                }

                if (rdo != null && !String.IsNullOrEmpty(treatmentCode))
                {
                    log += string.Format(" TREATMENT_CODE:{0}", treatmentCode);
                    log += string.Format(" DEATH_CERT_NUM:{0}", deathCertNum.HasValue ? deathCertNum.Value.ToString() : "");
                    log = Inventec.Common.String.CountVi.SubStringVi(log, 2000);
                }
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
                string treatmentCode = null;
                long? deathCertNum = null;
                if (rdo.TreatmentView != null)
                {
                    treatmentCode = rdo.TreatmentView.TREATMENT_CODE;
                    deathCertNum = rdo.TreatmentView.DEATH_CERT_NUM;
                }
                else if (rdo.ViewTreatment != null)
	            {
                    treatmentCode = rdo.TreatmentView.TREATMENT_CODE;
                    deathCertNum = rdo.TreatmentView.DEATH_CERT_NUM;
	            }

                if (rdo != null && !String.IsNullOrEmpty(treatmentCode))
                    result = String.Format("{0} {1} {2}", this.printTypeCode, string.Format("TREATMENT_CODE:{0}", treatmentCode), string.Format("DEATH_CERT_NUM:{0}", deathCertNum.HasValue ? deathCertNum.Value.ToString() : ""));
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        class CalculateMergerData : TFlexCelUserFunction
        {
            long typeId = 0;
            long mediMateTypeId = 0;

            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length <= 0)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");
                bool result = false;
                try
                {
                    long servicetypeId = Convert.ToInt64(parameters[0]);
                    long mediMateId = Convert.ToInt64(parameters[1]);

                    if (servicetypeId > 0 && mediMateId > 0)
                    {
                        if (this.typeId == servicetypeId && this.mediMateTypeId == mediMateId)
                        {
                            return true;
                        }
                        else
                        {
                            this.typeId = servicetypeId;
                            this.mediMateTypeId = mediMateId;
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    result = false;
                }
                return result;
            }
        }
    }
}
