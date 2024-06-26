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
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.Library.PrintOtherForm.Base;
using HIS.Desktop.Plugins.Library.PrintOtherForm.RunPrintTemplate;
using Inventec.Common.Adapter;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000246
{
    public partial class Mps000246Behavior : MpsDataBase, ILoad
    {
        private void SetDicParamEkipUser(ref Dictionary<string, object> dicParamPlus)
        {
            try
            {
                List<HIS_EXECUTE_ROLE> executeRoles = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EXECUTE_ROLE>();
                if (executeRoles != null && executeRoles.Count > 0)
                {
                    foreach (var item in executeRoles)
                    {
                        dicParamPlus.Add("USERNAME_EXECUTE_ROLE_" + item.EXECUTE_ROLE_CODE, "");
                        dicParamPlus.Add("LOGIN_NAME_EXECUTE_ROLE_" + item.EXECUTE_ROLE_CODE, "");
                    }
                }

                //Ekip thuc hien
                if (this.ekipUsers != null)
                {
                    foreach (var item in ekipUsers)
                    {
                        if (dicParamPlus.ContainsKey("USERNAME_EXECUTE_ROLE_" + item.EXECUTE_ROLE_CODE))
                        {
                            dicParamPlus["USERNAME_EXECUTE_ROLE_" + item.EXECUTE_ROLE_CODE] = item.USERNAME;
                        }
                        if (dicParamPlus.ContainsKey("LOGIN_NAME_EXECUTE_ROLE_" + item.EXECUTE_ROLE_CODE))
                        {
                            dicParamPlus["LOGIN_NAME_EXECUTE_ROLE_" + item.EXECUTE_ROLE_CODE] = item.LOGINNAME;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDicParamPatientType(ref Dictionary<string, object> dicParamPlus)
        {
            try
            {
                if (this.patient != null)
                {
                    AddKeyIntoDictionaryPrint<V_HIS_PATIENT>(patient, dicParamPlus);
                    dicParamPlus.Add("AGE", this.CalculateFullAge(patient.DOB));
                }
                else
                {
                    V_HIS_PATIENT temp = new V_HIS_PATIENT();
                    AddKeyIntoDictionaryPrint<V_HIS_PATIENT>(temp, dicParamPlus);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDicParamPatient(ref Dictionary<string, object> dicParamPlus)
        {
            try
            {
                if (this.patient != null)
                {
                    AddKeyIntoDictionaryPrint<V_HIS_PATIENT>(patient, dicParamPlus);
                    dicParamPlus.Add("AGE", this.CalculateFullAge(patient.DOB));
                }
                else
                {
                    V_HIS_PATIENT temp = new V_HIS_PATIENT();
                    AddKeyIntoDictionaryPrint<V_HIS_PATIENT>(temp, dicParamPlus);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDicParamSereServPTTT(ref Dictionary<string, object> dicParamPlus)
        {
            try
            {
                if (this.sereServPTTT != null && this.sereServ != null)
                {
                    AddKeyIntoDictionaryPrint<V_HIS_SERE_SERV_REHA>(this.sereServPTTT, dicParamPlus);
                }
                else
                {
                    V_HIS_SERE_SERV_REHA temp = new V_HIS_SERE_SERV_REHA();
                    AddKeyIntoDictionaryPrint<V_HIS_SERE_SERV_REHA>(temp, dicParamPlus);
                }

                //if (!String.IsNullOrEmpty((string)(dicParamPlus["BEFORE_PTTT_ICD_TEXT"] == null?"":dicParamPlus["BEFORE_PTTT_ICD_TEXT"])))
                //{
                //    dicParamPlus["BEFORE_PTTT_ICD_NAME"] = dicParamPlus["BEFORE_PTTT_ICD_TEXT"];
                //}

                //if (!String.IsNullOrEmpty((string)(dicParamPlus["AFTER_PTTT_ICD_TEXT"] == null ? "" : dicParamPlus["AFTER_PTTT_ICD_TEXT"])))
                //{
                //    dicParamPlus["AFTER_PTTT_ICD_NAME"] = dicParamPlus["AFTER_PTTT_ICD_TEXT"];
                //}

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDicParamSereServExt(ref Dictionary<string, object> dicParamPlus)
        {
            try
            {
                if (this.sereServExt != null)
                {
                    AddKeyIntoDictionaryPrint<HIS_SERE_SERV_EXT>(this.sereServExt, dicParamPlus);
                }
                else
                {
                    HIS_SERE_SERV_EXT temp = new HIS_SERE_SERV_EXT();
                    AddKeyIntoDictionaryPrint<HIS_SERE_SERV_EXT>(temp, dicParamPlus);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDicParamServiceReq(ref Dictionary<string, object> dicParamPlus)
        {
            try
            {
                if (this.serviceReq != null)
                {
                    if (String.IsNullOrEmpty(this.serviceReq.REQUEST_DEPARTMENT_NAME))
                    {
                        HIS_DEPARTMENT department = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == this.serviceReq.REQUEST_DEPARTMENT_ID);
                        this.serviceReq.REQUEST_DEPARTMENT_NAME = department != null ? department.DEPARTMENT_NAME : "";
                    }
                    dicParamPlus.Add("REQUEST_DEPARTMENT_NAME", this.serviceReq.REQUEST_DEPARTMENT_NAME);

                    if (String.IsNullOrEmpty(this.serviceReq.REQUEST_ROOM_NAME))
                    {
                        V_HIS_ROOM room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.serviceReq.REQUEST_ROOM_ID);
                        this.serviceReq.REQUEST_ROOM_NAME = room != null ? room.ROOM_NAME : "";
                    }
                    AddKeyIntoDictionaryPrint<V_HIS_SERVICE_REQ>(this.serviceReq, dicParamPlus);
                }
                else
                {
                    V_HIS_SERVICE_REQ temp = new V_HIS_SERVICE_REQ();
                    AddKeyIntoDictionaryPrint<V_HIS_SERVICE_REQ>(temp, dicParamPlus);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDicParamBedAndBedRoomFromTreatment(ref Dictionary<string, object> dicParamPlus)
        {
            try
            {
                if (treatmentBedRooms != null && treatmentBedRooms.Count > 0)
                {
                    var bedRoom = treatmentBedRooms.FirstOrDefault(o => o.REMOVE_TIME == null);
                    dicParamPlus.Add("BED_ROOM_NAME", bedRoom != null ? bedRoom.BED_ROOM_NAME : "");
                    dicParamPlus.Add("BED_NAME", bedRoom != null ? bedRoom.BED_NAME : "");
                }
                else
                {
                    dicParamPlus.Add("BED_ROOM_NAME", "");
                    dicParamPlus.Add("BED_NAME", "");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal string CalculateFullAge(long ageNumber)
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

        private void AddKeyIntoDictionaryPrint<T>(T data, Dictionary<string, object> dicParamPlus)
        {
            try
            {
                PropertyInfo[] pis = typeof(T).GetProperties();
                if (pis != null && pis.Length > 0)
                {
                    foreach (var pi in pis)
                    {
                        var searchKey = dicParamPlus.SingleOrDefault(o => o.Key == pi.Name);
                        if (String.IsNullOrEmpty(searchKey.Key))
                        {
                            dicParamPlus.Add(pi.Name, pi.GetValue(data) != null ? pi.GetValue(data) : "");
                        }
                        else
                        {
                            dicParamPlus[pi.Name] = pi.GetValue(data) != null ? pi.GetValue(data) : "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
