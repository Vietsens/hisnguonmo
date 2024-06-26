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
using MPS.Processor.MPS000308.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.MPS000308
{
    public class Mps000308Processor : AbstractProcessor
    {
        Mps000308PDO rdo;
        public Mps000308Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000308PDO)rdoBase;
        }
        NumberStyles style = NumberStyles.Any;

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
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();

                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                SetSingleKey();

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
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

        private void SetSingleKey()
        {
            try
            {
                if (rdo._PatientTypeAlter != null && !String.IsNullOrWhiteSpace(rdo._PatientTypeAlter.ADDRESS))
                {
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.TDL_HIEN_CARD_ADDRESS, rdo._PatientTypeAlter.ADDRESS));
                }

                if (rdo._Patient != null)
                {

                    if (!String.IsNullOrWhiteSpace(rdo._Patient.CMND_NUMBER))
                    {
                        SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.CMND_NUMBER, rdo._Patient.CMND_NUMBER));
                        SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.CMND_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._Patient.CMND_DATE ?? 0)));
                        SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.CMND_PLACE, rdo._Patient.CMND_PLACE ?? ""));
                    }
                    else
                    {
                        SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.CMND_NUMBER, rdo._Patient.CCCD_NUMBER));
                        SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.CMND_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._Patient.CCCD_DATE ?? 0)));
                        SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.CMND_PLACE, rdo._Patient.CCCD_PLACE ?? ""));
                    }
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.ETHNIC_NAME, rdo._Patient.ETHNIC_NAME ?? ""));
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.ADDRESS, rdo._Patient.ADDRESS ?? ""));
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.HT_ADDRESS, rdo._Patient.HT_ADDRESS ?? ""));
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.HT_COMMUNE_NAME, rdo._Patient.HT_COMMUNE_NAME ?? ""));
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.HT_DISTRICT_NAME, rdo._Patient.HT_DISTRICT_NAME ?? ""));
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.HT_PROVINCE_NAME, rdo._Patient.HT_PROVINCE_NAME ?? ""));
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.VIR_HT_ADDRESS, rdo._Patient.VIR_HT_ADDRESS ?? ""));
                }

                if (rdo._Treatment != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo._Treatment, false);
                    if (rdo._Treatment.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                    {
                        SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.DOB_STR, rdo._Treatment.TDL_PATIENT_DOB.ToString().Substring(0, 4)));
                    }
                    else
                    {
                        SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._Treatment.TDL_PATIENT_DOB)));
                    }
                    switch (rdo._Treatment.NUMBER_OF_TESTS ?? -1)
                    {
                        case 1:
                            SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.NUMBER_OF_TESTS_STR, "3 lần /3 kỳ"));
                            break;
                        case 2:
                            SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.NUMBER_OF_TESTS_STR, "≥ 4 lần / 3 kỳ"));
                            break;
                        default:
                            SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.NUMBER_OF_TESTS_STR, ""));
                            break;
                    }
                    switch (rdo._Treatment.TEST_HIV ?? -1)
                    {
                        case 1:
                            SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.TEST_HIV_STR, "Trước và trong mang thai"));
                            break;
                        case 2:
                            SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.TEST_HIV_STR, "Trong chuyển dạ"));
                            break;
                        default:
                            SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.TEST_HIV_STR, ""));
                            break;
                    }
                    switch (rdo._Treatment.TEST_SYPHILIS ?? -1)
                    {
                        case 1:
                            SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.TEST_SYPHILIS_STR, "Trong mang thai"));
                            break;
                        case 2:
                            SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.TEST_SYPHILIS_STR, "Trong chuyển dạ"));
                            break;
                        default:
                            SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.TEST_SYPHILIS_STR, ""));
                            break;
                    }
                    switch (rdo._Treatment.TEST_HEPATITIS_B ?? -1)
                    {
                        case 1:
                            SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.TEST_HEPATITIS_B_STR, "Trong mang thai"));
                            break;
                        case 2:
                            SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.TEST_HEPATITIS_B_STR, "Trong chuyển dạ"));
                            break;
                        default:
                            SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.TEST_HEPATITIS_B_STR, ""));
                            break;
                    }
                    switch (rdo._Treatment.NEWBORN_CARE_AT_HOME ?? -1)
                    {
                        case 1:
                            SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.NEWBORN_CARE_AT_HOME_STR, "Tuần đầu"));
                            break;
                        case 2:
                            SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.NEWBORN_CARE_AT_HOME_STR, "Từ tuần 2 đến hết 6"));
                            break;
                        default:
                            SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.NEWBORN_CARE_AT_HOME_STR, ""));
                            break;
                    }
                }

                if (rdo._CurrentBaby != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_BABY>(rdo._CurrentBaby, false);
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.BABY_ETHNIC_CODE, rdo._CurrentBaby.ETHNIC_CODE ?? ""));
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.BABY_ETHNIC_NAME, rdo._CurrentBaby.ETHNIC_NAME ?? ""));
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.BABY_FATHER_NAME, rdo._CurrentBaby.FATHER_NAME ?? ""));
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.BABY_GENDER_CODE, rdo._CurrentBaby.GENDER_CODE ?? ""));
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.BABY_GENDER_NAME, rdo._CurrentBaby.GENDER_NAME ?? ""));
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.BABY_HEAD, rdo._CurrentBaby.HEAD));
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.BABY_HEIGHT, rdo._CurrentBaby.HEIGHT));
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.BABY_MIDWIFE, rdo._CurrentBaby.MIDWIFE ?? ""));
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.BABY_MONTH_COUNT, rdo._CurrentBaby.MONTH_COUNT));
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.BABY_NAME, rdo._CurrentBaby.BABY_NAME ?? ""));
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.BABY_ORDER, rdo._CurrentBaby.BABY_ORDER));
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.BABY_WEEK_COUNT, rdo._CurrentBaby.WEEK_COUNT));
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.BABY_WEIGHT, rdo._CurrentBaby.WEIGHT));
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.BORN_POSITION_CODE, rdo._CurrentBaby.BORN_POSITION_CODE ?? ""));
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.BORN_POSITION_NAME, rdo._CurrentBaby.BORN_POSITION_NAME ?? ""));
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.BORN_RESULT_CODE, rdo._CurrentBaby.BORN_RESULT_CODE ?? ""));
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.BORN_RESULT_NAME, rdo._CurrentBaby.BORN_RESULT_NAME ?? ""));
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.BORN_TIME, rdo._CurrentBaby.BORN_TIME));
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.BORN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo._CurrentBaby.BORN_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.BORN_TYPE_CODE, rdo._CurrentBaby.BORN_TYPE_CODE));
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.BORN_TYPE_NAME, rdo._CurrentBaby.BORN_TYPE_NAME));
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.BORN_TIME, rdo._CurrentBaby.BORN_TIME));
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.BORN_TIME, rdo._CurrentBaby.BORN_TIME));
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.CURRENT_ALIVE, rdo._CurrentBaby.CURRENT_ALIVE));
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.BIRTH_CERT_NUM, rdo._CurrentBaby.BIRTH_CERT_NUM));
                    SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.NOTE, rdo._CurrentBaby.NOTE));
                }

                SetSingleKey(new KeyValue(Mps000308ExtendSingleKey.BABY_COUNT, rdo._CountBaby));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {

                log = "TREATMENT_CODE:" + rdo._Treatment.TREATMENT_CODE;
                log += "BIRTH_CERT_NUM:" + rdo._CurrentBaby.BIRTH_CERT_NUM;
                log += "BIRTH_CERT_BOOK_ID:" + rdo._CurrentBaby.BIRTH_CERT_BOOK_ID;
                log += "BABY_NAME  : " + rdo._CurrentBaby.BABY_NAME;
                log += "BABY_NOTE:" + rdo._CurrentBaby.NOTE;
            }
            catch (Exception ex)
            {
                log = "";
                Inventec.Common.Logging.LogSystem.Error("___1 ##" + ex);
            }
            return log;
        }

        public override string ProcessUniqueCodeData()
        {
            string result = "";
            try
            {
                if (rdo != null)
                {
                    if (rdo._Treatment != null && rdo._CurrentBaby != null)
                    {

                        result = String.Format("{0} TREATMENT_CODE:{1} BABY_ID:{2}", this.printTypeCode, rdo._Treatment.TREATMENT_CODE, rdo._CurrentBaby.ID);
                    }
                    else
                    {

                        result = String.Format("{0} TREATMENT_CODE:{1} BABY_ID:{2}", this.printTypeCode, "", "");
                    }
                    Inventec.Common.Logging.LogSystem.Info("UniqueCodeData: " + result);

                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error("___2 ##" + ex);
            }
            return result;
        }
    }
}
