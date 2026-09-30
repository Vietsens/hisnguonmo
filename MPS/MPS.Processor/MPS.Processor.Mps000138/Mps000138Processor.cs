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
using AutoMapper;
using FlexCel.Report;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using MPS.Processor.Mps000138.ADO;
using MPS.Processor.Mps000138.PDO;
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000138
{
    public class Mps000138Processor : AbstractProcessor
    {
        Mps000138PDO rdo;

        public Mps000138Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000138PDO)rdoBase;
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
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                ProcessSingleKey();
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


        void ProcessSingleKey()
        {
            try
            {
                if (rdo._RegisterReq != null)
                {
                    SetSingleKey(new KeyValue(Mps000138ExtendSingleKey.REGISTER_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo._RegisterReq.REGISTER_TIME)));
                    SetSingleKey(new KeyValue(Mps000138ExtendSingleKey.REGISTER_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._RegisterReq.REGISTER_TIME)));
                    AddObjectKeyIntoListkey<V_HIS_REGISTER_REQ>(rdo._RegisterReq, false);
                }
                SetSingleKey(new KeyValue(Mps000138ExtendSingleKey.LAST_CALLED_NUM_ORDER, rdo._RegisterReqLastCalled != null ? rdo._RegisterReqLastCalled.NUM_ORDER : 0));

                // Luon dat khoa de mau in khong bi thieu the khi so lay theo duong khong dinh danh
                SetSingleKey(new KeyValue(Mps000138ExtendSingleKey.IDENTITY_PATIENT_NAME, this.GetIdentityPatientName()));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Lay ho ten nguoi benh trong chuoi JSON cua ban ghi cap so.
        /// Khong doc duoc thi tra ve chuoi rong, phieu in ra nhu truoc.
        /// </summary>
        string GetIdentityPatientName()
        {
            try
            {
                if (rdo._RegisterReq == null || String.IsNullOrWhiteSpace(rdo._RegisterReq.IDENTITY_JSON))
                {
                    return "";
                }

                return ReadJsonStringValue(rdo._RegisterReq.IDENTITY_JSON, "PatientName");
            }
            catch (Exception ex)
            {
                // Chuoi JSON hong thi khong chan viec in phieu
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }

        /// <summary>
        /// Doc gia tri chuoi cua mot khoa trong chuoi JSON phang.
        /// Tu tach de du an in khong phai them phu thuoc thu vien JSON.
        /// </summary>
        static string ReadJsonStringValue(string json, string key)
        {
            try
            {
                string marker = "\"" + key + "\"";
                int keyIndex = json.IndexOf(marker, StringComparison.Ordinal);
                if (keyIndex < 0)
                {
                    return "";
                }

                int colonIndex = json.IndexOf(':', keyIndex + marker.Length);
                if (colonIndex < 0)
                {
                    return "";
                }

                int openQuote = json.IndexOf('"', colonIndex + 1);
                if (openQuote < 0)
                {
                    return "";
                }

                StringBuilder value = new StringBuilder();
                for (int i = openQuote + 1; i < json.Length; i++)
                {
                    char current = json[i];
                    if (current == '\\' && i + 1 < json.Length)
                    {
                        // Giu nguyen ky tu sau dau thoat, du cho ho ten nen khong xu ly \u
                        i++;
                        value.Append(json[i]);
                        continue;
                    }
                    if (current == '"')
                    {
                        break;
                    }
                    value.Append(current);
                }
                return value.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }
    }
}
