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
using MPS.Processor.Mps000389.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000389
{
    public class Mps000389Processor : AbstractProcessor
    {
        Mps000389PDO rdo;

        public Mps000389Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000389PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                SetSingleKey();
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
                if (rdo._Treatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000389ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._Treatment.TDL_PATIENT_DOB)));

                    string temp = rdo._Treatment.TDL_PATIENT_DOB.ToString();
                    if (temp != null && temp.Length >= 8)
                    {
                        SetSingleKey(new KeyValue(Mps000389ExtendSingleKey.YEAR_STR, temp.Substring(0, 4)));
                    }
                    SetSingleKey(new KeyValue(Mps000389ExtendSingleKey.AGE_STR, AgeUtil.CalculateFullAge(rdo._Treatment.TDL_PATIENT_DOB)));

                    SetSingleKey(new KeyValue(Mps000389ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo._Treatment.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000389ExtendSingleKey.SURG_APPOINTMENT_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo._Treatment.SURGERY_APPOINTMENT_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000389ExtendSingleKey.SURG_APPOINTMENT_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(rdo._Treatment.SURGERY_APPOINTMENT_TIME ?? 0)));

                    AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo._Treatment, false);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public static class AgeUtil
        {
            public static string CalculateFullAge(long ageNumber)
            {
                string tuoi;
                string cboAge;
                try
                {
                    DateTime dtNgSinh = Inventec.Common.TypeConvert.Parse.ToDateTime(Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ageNumber));
                    TimeSpan diff = DateTime.Now - dtNgSinh;
                    long tongsogiay = diff.Ticks;
                    if (tongsogiay < 0)
                    {
                        tuoi = "";
                        cboAge = "Tuổi";
                        return "";
                    }
                    DateTime newDate = new DateTime(tongsogiay);

                    int nam = newDate.Year - 1;
                    int thang = newDate.Month - 1;
                    int ngay = newDate.Day - 1;
                    int gio = newDate.Hour;
                    int phut = newDate.Minute;
                    int giay = newDate.Second;

                    if (nam > 0)
                    {
                        tuoi = nam.ToString();
                        cboAge = "Tuổi";
                    }
                    else
                    {
                        if (thang > 0)
                        {
                            tuoi = thang.ToString();
                            cboAge = "Tháng";
                        }
                        else
                        {
                            if (ngay > 0)
                            {
                                tuoi = ngay.ToString();
                                cboAge = "ngày";
                            }
                            else
                            {
                                tuoi = "";
                                cboAge = "Giờ";
                            }
                        }
                    }
                    return tuoi + " " + cboAge;
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                    return "";
                }
            }
        }
    }
}
