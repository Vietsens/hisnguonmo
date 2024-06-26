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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Library.ElectronicBill.Config;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.Base
{
    internal class General
    {
        internal static Dictionary<string, string> DicDataBuyerInfo = null;

        /// <summary>
        /// trả ra các ký tự đầu tiên của từ trong chuỗi văn bản và viết hoa toàn bộ ký tự
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static string GetFirstWord(string name)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrWhiteSpace(name))
                {
                    var spl = name.Split(' ', ',', '.', '/').ToList();
                    foreach (var item in spl)
                    {
                        if (!String.IsNullOrWhiteSpace(item))
                        {
                            if (Char.IsLetter(item[0]))
                            {
                                result += item[0].ToString().ToUpper();
                            }
                        }
                    }
                }

                if (!String.IsNullOrWhiteSpace(result))
                {
                    result = Inventec.Common.String.Convert.UnSignVNese2(result);
                }
            }
            catch (Exception ex)
            {
                result = name.Split(' ', ',', '.').FirstOrDefault();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        internal static string GenarateXmlStringFromConfig(ElectronicBillDataInput inputData, Type dataType, Dictionary<string, string> dicReplateValue)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrWhiteSpace(HisConfigCFG.ElectronicBillXmlInvoicePlus) && inputData != null)
                {
                    string[] xmlKeys = HisConfigCFG.ElectronicBillXmlInvoicePlus.Split('|');
                    Dictionary<string, string> dicXmlKey = new Dictionary<string, string>();
                    foreach (var item in xmlKeys)
                    {
                        string[] keys = item.Split(':');
                        if (keys.Count() > 1)
                        {
                            string value = item.Replace(keys[0] + ":", "");
                            //không có dữ liệu thì gán null để mất thẻ trong XML
                            //dùng Empty để cho phép nhập khoảng trắng trong cấu hình
                            if (!String.IsNullOrEmpty(value))
                            {
                                //nếu có ký tự đặc biệt không thuộc ký tự xml thì thêm thẻ cdata
                                if (value.FirstOrDefault(ch => !System.Xml.XmlConvert.IsXmlChar(ch)) != null)
                                {
                                    dicXmlKey[keys[0]] = string.Format("<![CDATA[{0}]]>", value);
                                }
                                else
                                {
                                    dicXmlKey[keys[0]] = value;
                                }
                            }
                            else
                            {
                                dicXmlKey[keys[0]] = null;
                            }
                        }
                    }

                    Dictionary<string, string> dicXmlValues = ProcessDicValueString(inputData);

                    System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get(dataType);

                    List<string> dataResult = new List<string>();
                    foreach (var keys in dicXmlKey)
                    {
                        string value = keys.Value;
                        if (!String.IsNullOrWhiteSpace(value))
                        {
                            foreach (var values in dicXmlValues)
                            {
                                value = value.Replace(values.Key, values.Value);
                            }
                        }

                        bool isReplate = false;
                        foreach (var item in pi)
                        {
                            if (item.Name == keys.Key)
                            {
                                isReplate = true;
                                break;
                            }
                        }

                        //trả ra dữ liệu null để gán dữ liệu
                        if (isReplate)
                        {
                            dicReplateValue[keys.Key] = value;
                        }
                        else
                        {
                            dataResult.Add(string.Format("<{0}>{1}</{0}>", keys.Key, value));
                        }
                    }

                    result = string.Join("", dataResult);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        internal static Dictionary<string, string> ProcessDicValueString(ElectronicBillDataInput inputData)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            try
            {
                if (DicDataBuyerInfo != null && DicDataBuyerInfo.Count > 0)
                {
                    return DicDataBuyerInfo;
                }

                string yob = "";
                string age = "";
                string dobStr = "";
                string treatmentCode = "";
                string patientCode = "";
                string patientTypeName = "";
                string treatmentTypeName = "";
                string currentRoomDepartment = "";
                string cashierUsername = "";
                string cashierLoginname = "";
                string inTime = "";
                string outTime = "";
                string numOrder = "";
                string gender = "";
                string clinicalInTime = "";

                if (inputData.Treatment != null)
                {
                    long dob = inputData.Treatment.TDL_PATIENT_DOB;

                    long in_time = 0;
                    if (dob > 0)
                    {
                        yob = dob.ToString().Substring(0, 4);
                        in_time = inputData.Treatment != null ? inputData.Treatment.IN_TIME : (Inventec.Common.DateTime.Get.Now() ?? 0);
                        age = dob > 0 && in_time > 0 ? Inventec.Common.DateTime.Calculation.AgeString(dob, "", "tháng", "ngày", "giờ", in_time) : "";
                        dobStr = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dob);
                    }

                    inTime = in_time > 0 ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(in_time) : "";

                    if (inputData.Treatment.CLINICAL_IN_TIME.HasValue)
                    {
                        clinicalInTime = Inventec.Common.DateTime.Convert.TimeNumberToDateString(inputData.Treatment.CLINICAL_IN_TIME.Value);
                    }

                    treatmentCode = inputData.Treatment.TREATMENT_CODE;
                    patientCode = inputData.Treatment.TDL_PATIENT_CODE;
                    gender = inputData.Treatment.TDL_PATIENT_GENDER_NAME;

                    var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == inputData.Treatment.TDL_PATIENT_TYPE_ID);
                    patientTypeName = patientType != null ? patientType.PATIENT_TYPE_NAME : "";

                    var treatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == inputData.Treatment.TDL_TREATMENT_TYPE_ID);
                    treatmentTypeName = treatmentType != null ? treatmentType.TREATMENT_TYPE_NAME : "";

                    if (inputData.Treatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                    {
                        var endRoom = inputData.Treatment.END_ROOM_ID.HasValue ? BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == inputData.Treatment.END_ROOM_ID) : null;
                        if (endRoom != null)
                        {
                            currentRoomDepartment = endRoom.ROOM_NAME;
                        }
                        else
                        {
                            HisSereServFilter ssFilter = new HisSereServFilter();
                            ssFilter.TREATMENT_ID = inputData.Treatment.ID;
                            ssFilter.TDL_SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH;
                            var sereServKham = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssFilter, null);
                            if (sereServKham != null && sereServKham.Count > 0)
                            {
                                sereServKham = sereServKham.OrderByDescending(o => o.TDL_IS_MAIN_EXAM ?? 0).ThenByDescending(o => o.TDL_INTRUCTION_TIME).ToList();
                                var exeRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == sereServKham.First().TDL_EXECUTE_ROOM_ID);
                                if (exeRoom != null)
                                {
                                    currentRoomDepartment = exeRoom.ROOM_NAME;
                                }
                            }
                        }
                    }
                    else if (inputData.Treatment.LAST_DEPARTMENT_ID.HasValue)
                    {
                        var department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == inputData.Treatment.LAST_DEPARTMENT_ID.Value);
                        if (department != null)
                        {
                            currentRoomDepartment = department.DEPARTMENT_NAME;
                        }
                    }
                    else
                    {
                        HisDepartmentTranLastFilter filter = new HisDepartmentTranLastFilter();
                        filter.TREATMENT_ID = inputData.Treatment.ID;
                        V_HIS_DEPARTMENT_TRAN tran = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                        if (tran != null)
                        {
                            currentRoomDepartment = tran.DEPARTMENT_NAME;
                        }
                    }

                    if (inputData.Treatment.OUT_TIME.HasValue)
                    {
                        outTime = Inventec.Common.DateTime.Convert.TimeNumberToDateString(inputData.Treatment.OUT_TIME.Value);
                    }
                }

                if (inputData.Transaction != null)
                {
                    cashierLoginname = inputData.Transaction.CASHIER_LOGINNAME;
                    cashierUsername = inputData.Transaction.CASHIER_USERNAME;
                    numOrder = inputData.Transaction.NUM_ORDER + "";
                }
                else if (inputData.ListTransaction != null && inputData.ListTransaction.Count > 0)
                {
                    var lastTransaction = inputData.ListTransaction.OrderByDescending(o => o.ID).FirstOrDefault();

                    cashierLoginname = lastTransaction.CASHIER_LOGINNAME;
                    cashierUsername = lastTransaction.CASHIER_USERNAME;
                }

                string lydo = "Thu viện phí";

                if (inputData.Transaction != null && (!String.IsNullOrWhiteSpace(inputData.Transaction.EXEMPTION_REASON) || !String.IsNullOrWhiteSpace(inputData.Transaction.DESCRIPTION)))
                {
                    if (!String.IsNullOrWhiteSpace(inputData.Transaction.EXEMPTION_REASON))
                        lydo = inputData.Transaction.EXEMPTION_REASON;
                    else if (!String.IsNullOrWhiteSpace(inputData.Transaction.DESCRIPTION))
                        lydo = inputData.Transaction.DESCRIPTION;
                }

                string heinRatio = "";
                string heinCardNumber = "";
                if (inputData.LastPatientTypeAlter != null)
                {
                    decimal ratio = (new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(inputData.LastPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, inputData.LastPatientTypeAlter.HEIN_CARD_NUMBER, inputData.LastPatientTypeAlter.LEVEL_CODE, inputData.LastPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100;
                    heinRatio = ratio + "%";
                    heinCardNumber = inputData.LastPatientTypeAlter.HEIN_CARD_NUMBER;
                }

                result["YOB"] = yob;
                result["AGE"] = age;
                result["TREATMENT_CODE"] = treatmentCode;
                result["PATIENT_CODE"] = patientCode;
                result["PATIENT_TYPE_NAME"] = patientTypeName;
                result["TREATMENT_TYPE_NAME"] = treatmentTypeName;
                result["CURRENT_ROOM_DEPARTMENT"] = currentRoomDepartment;
                result["CASHIER_LOGINNAME"] = cashierLoginname;
                result["CASHIER_USERNAME"] = cashierUsername;
                result["IN_TIME"] = inTime;
                result["OUT_TIME"] = outTime;
                result["REASON"] = lydo;
                result["TRANSACTION_NUM_ORDER"] = numOrder;
                result["GENDER_NAME"] = gender;
                result["CLINICAL_IN_TIME"] = clinicalInTime;
                result["HEIN_RATIO"] = heinRatio;
                result["DOB_STR"] = dobStr;
                result["HEIN_CARD_NUMBER"] = heinCardNumber;
            }
            catch (Exception ex)
            {
                result = new Dictionary<string, string>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                DicDataBuyerInfo = result;
            }

            return result;
        }
    }
}
