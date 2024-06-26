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
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.OtherFormAssTreatment.Base;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Common.RichEditor;
using Inventec.Common.RichEditor.Base;
using Inventec.Common.WordContent;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using SAR.EFMODEL.DataModels;
using SAR.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.OtherFormAssTreatment
{
    public partial class frmOtherFormAssTreatment : HIS.Desktop.Utility.FormBase
    {
        private void SetDicParamPatient(ref Dictionary<string, object> dicParamPlus)
        {
            try
            {
                if (this.Patient != null)
                {
                    TemplateKeyProcessor.AddKeyIntoDictionaryPrint<V_HIS_PATIENT>(Patient, dicParamPlus);
                    TemplateKeyProcessor.SetSingleKey(dicParamPlus, "AGE", MPS.AgeUtil.CalculateFullAge(Patient.DOB));
                    TemplateKeyProcessor.SetSingleKey(dicParamPlus, "DOB_YEAR", Patient.DOB.ToString().Substring(0, 4));
                    if (!String.IsNullOrWhiteSpace(Patient.CMND_NUMBER))
                    {
                        dicParamPlus["CMND_CCCD_NUMBER"] = Patient.CMND_NUMBER;
                        dicParamPlus["CMND_CCCD_DATE"] = Inventec.Common.DateTime.Convert.TimeNumberToDateString(Patient.CMND_DATE ?? 0);
                        dicParamPlus["CMND_CCCD_PLACE"] = Patient.CMND_PLACE;
                    }
                    else
                    {
                        dicParamPlus["CMND_CCCD_NUMBER"] = Patient.CCCD_NUMBER;
                        dicParamPlus["CMND_CCCD_DATE"] = Inventec.Common.DateTime.Convert.TimeNumberToDateString(Patient.CCCD_DATE ?? 0);
                        dicParamPlus["CMND_CCCD_PLACE"] = Patient.CCCD_PLACE;
                    }
                }
                else
                {
                    V_HIS_PATIENT temp = new V_HIS_PATIENT();
                    TemplateKeyProcessor.AddKeyIntoDictionaryPrint<V_HIS_PATIENT>(temp, dicParamPlus);
                    TemplateKeyProcessor.SetSingleKey(dicParamPlus, "AGE", "");
                    TemplateKeyProcessor.SetSingleKey(dicParamPlus, "DOB_YEAR", "");
                    dicParamPlus["CMND_CCCD_NUMBER"] = "";
                    dicParamPlus["CMND_CCCD_DATE"] = "";
                    dicParamPlus["CMND_CCCD_PLACE"] = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDicParamTreatment(ref Dictionary<string, object> dicParamPlus)
        {
            try
            {
                if (this.Treatment != null)
                {
                    // AddKeyIntoDictionaryPrint<V_HIS_TREATMENT>(this.Treatment, dicParamPlus);
                    TemplateKeyProcessor.SetSingleKey(dicParamPlus, "AGE_TREATMENT", MPS.AgeUtil.CalculateFullAge(this.Treatment.TDL_PATIENT_DOB));
                }
                else
                {
                    //V_HIS_TREATMENT temp = new V_HIS_TREATMENT();
                    //AddKeyIntoDictionaryPrint<V_HIS_TREATMENT>(temp, dicParamPlus);
                    TemplateKeyProcessor.SetSingleKey(dicParamPlus, "AGE_TREATMENT", "");
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
                if (this.TreatmentBedRooms != null && this.TreatmentBedRooms.Count > 0)
                {
                    var bedRoom = this.TreatmentBedRooms.FirstOrDefault(o => o.REMOVE_TIME == null);
                    TemplateKeyProcessor.SetSingleKey(dicParamPlus, "BED_ROOM_NAME", bedRoom != null ? bedRoom.BED_ROOM_NAME : "");
                    TemplateKeyProcessor.SetSingleKey(dicParamPlus, "BED_NAME", bedRoom != null ? bedRoom.BED_NAME : "");
                }
                else
                {
                    TemplateKeyProcessor.SetSingleKey(dicParamPlus, "BED_ROOM_NAME", "");
                    TemplateKeyProcessor.SetSingleKey(dicParamPlus, "BED_NAME", "");
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void AddKeyIntoDictionaryPrint<T>(T data, Dictionary<string, object> dicParamPlus)
        //{
        //    try
        //    {
        //        PropertyInfo[] pis = typeof(T).GetProperties();
        //        if (pis != null && pis.Length > 0)
        //        {
        //            foreach (var pi in pis)
        //            {
        //                var searchKey = dicParamPlus.SingleOrDefault(o => o.Key == pi.Name);
        //                if (String.IsNullOrEmpty(searchKey.Key))
        //                {
        //                    TemplateKeyProcessor.SetSingleKey(dicParamPlus,pi.Name, pi.GetValue(data) != null ? pi.GetValue(data) : "");
        //                }
        //                else
        //                {
        //                    dicParamPlus[pi.Name] = pi.GetValue(data) != null ? pi.GetValue(data) : "";
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
    }
}
