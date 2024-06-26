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
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000072.PDO
{
  public partial class Mps000072PDO : RDOBase
  {
    public PatientADO currentPatient { get; set; }
    public TreatmentADO currentTreatment { get; set; }
    public List<HIS_DHST> lstDHST { get; set; }
    public V_HIS_SERVICE_REQ currentServiceReq { get; set; }
    public List<V_HIS_SERVICE_REQ> lstServiceReq { get; set; }
    public List<V_HIS_SERE_SERV> lstSereServ { get; set; }
    public List<HisTrackingGroupADOs> lstTrackingTimeADOs { get; set; }
    public List<HisServiceReqTrackingId> lstServiceReqPrint { get; set; }
    public List<Mps000062ADO> mps000062ADOs { get; set; }
    public string departmentName;
    public string roomName;
    public List<Mps000062ADOMedicines> lstMps000062ADOMedicines { get; set; }
    public List<Mps000062ADOServiceCLS> lstMps000062ADOServiceCLS { get; set; }
  }

  public class PatientADO : MOS.EFMODEL.DataModels.V_HIS_PATIENT
  {
    public string AGE { get; set; }
    public string DOB_STR { get; set; }
    public string CMND_DATE_STR { get; set; }
    public string DOB_YEAR { get; set; }
    public string GENDER_MALE { get; set; }
    public string GENDER_FEMALE { get; set; }

    public PatientADO()
    {

    }

    public PatientADO(V_HIS_PATIENT data)
    {
      try
      {
        if (data != null)
        {
          System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<V_HIS_PATIENT>();
          foreach (var item in pi)
          {
            item.SetValue(this, item.GetValue(data));
          }

          this.AGE = AgeUtil.CalculateFullAge(this.DOB);
          this.DOB_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.DOB);
          string temp = this.DOB.ToString();
          if (temp != null && temp.Length >= 8)
          {
            this.DOB_YEAR = temp.Substring(0, 4);
          }
        }
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }
  }

  public class TreatmentADO : MOS.EFMODEL.DataModels.V_HIS_TREATMENT
  {
    public string LOCK_TIME_STR { get; set; }
  }

  public class HisTrackingGroupADOs : MOS.EFMODEL.DataModels.HIS_TRACKING
  {
    public string TRACKING_TIME_STR { get; set; }

    //Xét nghiệm
    public string TEST_NAME { get; set; }
    public string TEST_DETAIL { get; set; }

    //Chuẩn đoán hình ảnh
    public string DIIM_NAME { get; set; }
    public string DIIM_DETAIL { get; set; }

    //Thủ thuật
    public string MISU_NAME { get; set; }
    public string MISU_DETAIL { get; set; }

    //Phẫu thuật
    public string SURG_NAME { get; set; }
    public string SURG_DETAIL { get; set; }

    //Thăm dò chức năng
    public string FUEX_NAME { get; set; }
    public string FUEX_DETAIL { get; set; }

    //Nội soi
    public string ENDO_NAME { get; set; }
    public string ENDO_DETAIL { get; set; }

    //Siêu âm
    public string SUIM_NAME { get; set; }
    public string SUIM_DETAIL { get; set; }

    //Thuốc vật tư
    public string PRES_NAME { get; set; }
    public string PRES_DETAIL { get; set; }

    //Phục hồi chức năng
    public string REHA_NAME { get; set; }
    public string REHA_DETAIL { get; set; }

    //Giường
    public string BED_NAME { get; set; }
    public string BED_DETAIL { get; set; }

    //Khác
    public string OTHER_NAME { get; set; }
    public string OTHER_DETAIL { get; set; }

    //ICD
    public string ICD_NAME_TRACKING { get; set; }
  }

  public class HisServiceReqTrackingId : MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ
  {
    public long TRACKING_ID_LONG { get; set; }
  }

  public class Mps000062ADO : MOS.EFMODEL.DataModels.HIS_TRACKING
  {
    public string TRACKING_TIME_STR { get; set; }

    public long? REMEDY_COUNT { get; set; }

    public string Tracking_Detail { get; set; }

    public string ICD_NAME_TRACKING { get; set; }
  }

  public class Mps000062ADOMedicines : MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE
  {
    public long TRACKING_ID { get; set; }
    public string TRACKING_TIME_STR { get; set; }
    public string Tracking_Detail { get; set; }
    public long? REMEDY_COUNT { get; set; }
    public decimal? Amount_By_Remedy_Count { get; set; }

    public Mps000062ADOMedicines(MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE data)
    {
      try
      {
        Inventec.Common.Mapper.DataObjectMapper.Map<Mps000062ADOMedicines>(this, data);
        if (data.MEDICINE_TYPE_NAME != "")
        {
          this.MEDICINE_TYPE_NAME = " - " + data.MEDICINE_TYPE_NAME;
        }

      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }
  }

  public class Mps000062ADOServiceCLS : MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_7
  {
    public long TRACKING_ID { get; set; }
    public string TRACKING_TIME_STR { get; set; }
    public string Tracking_Detail { get; set; }

    public Mps000062ADOServiceCLS(MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_7 data)
    {
      try
      {
        if (data != null)
        {
          Inventec.Common.Mapper.DataObjectMapper.Map<Mps000062ADOServiceCLS>(this, data);
        }
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }
  }
}
