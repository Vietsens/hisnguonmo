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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Core;
using MOS.Filter;
using MOS.SDO;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.ADO;
using HIS.Desktop.Print;
using HIS.Desktop.Plugins.TreatmentLogList.Resources;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Logging;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.SdaConfigKey.Config;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.TreatmentLogList
{
 public partial class UCTreatmentProcessPartial : UserControl, IControlCallBack
    {
     Inventec.Desktop.Common.Modules.Module currentModule = null;
     internal MOS.SDO.HisTreatmentSDO currentDepartmentTran;
     internal V_HIS_TREATMENT_4 currentTreatment = new  V_HIS_TREATMENT_4();
     internal long currentTreatmentId = 0;

     internal List<HisTreatmentLogSDO> apiResult { get; set; }
long currentRoomId;
public UCTreatmentProcessPartial(Inventec.Desktop.Common.Modules.Module module, long treatmentId, long currentRoomId)
        {
            this.currentModule = module;
            this.currentTreatmentId = treatmentId;
            this.currentRoomId = currentRoomId;
						this.currentModule.RoomId = currentRoomId;
            InitializeComponent();
            MeShow();
            Init();
        }
       
        long logTypeId__DepartmentTran = 0;
        long logTypeId__MediRecord = 0;
        long logTypeId__patientTypeAlter = 0;
        long treatmentId = 0;

        public void MeShow()
        {
            CommonParam param = new CommonParam();
            try
            {

             logTypeId__DepartmentTran = HisTreatmentLogTypeCFG.TREATMENT_LOG_TYPE_ID__DEPARTMENT_TRAN;
             logTypeId__MediRecord = HisTreatmentLogTypeCFG.TREATMENT_LOG_TYPE_ID__MEDI_RECORD;
             logTypeId__patientTypeAlter = HisTreatmentLogTypeCFG.TREATMENT_LOG_TYPE_ID__PATIENT_TYPE_ALTER;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void Init()
        {
            CommonParam param = new CommonParam();
            try
            {
             logTypeId__DepartmentTran = HisTreatmentLogTypeCFG.TREATMENT_LOG_TYPE_ID__DEPARTMENT_TRAN;
             logTypeId__MediRecord = HisTreatmentLogTypeCFG.TREATMENT_LOG_TYPE_ID__MEDI_RECORD;
             logTypeId__patientTypeAlter = HisTreatmentLogTypeCFG.TREATMENT_LOG_TYPE_ID__PATIENT_TYPE_ALTER;
               HisTreatmentView4Filter filter = new HisTreatmentView4Filter();
filter.ID = currentTreatmentId;
currentTreatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT_4>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW,ApiConsumers.MosConsumer,filter,param).ToList().First();
                
                if (this.currentTreatment != null)
                {

                    if (currentTreatment.IS_PAUSE == IMSys.DbConfig.HIS_RS.HIS_TREATMENT.IS_PAUSE__TRUE)
                    {
                     LoadDataToGridTreatmentLog(this.currentTreatment);
                        btnSplitMediRecord.Enabled = false;
                        btnDepartmentTran.Enabled = false;
                        btnPatientTypeAlter.Enabled = false;
                    }
                    else
                    {
                     LoadDataToGridTreatmentLog(this.currentTreatment);
                        btnSplitMediRecord.Enabled = true;
                        btnDepartmentTran.Enabled = true;
                        btnPatientTypeAlter.Enabled = true;
                    }
                }
                else
                {
                    gridControlTreatmentLogs.DataSource = null;
                    btnSplitMediRecord.Enabled = false;
                    btnDepartmentTran.Enabled = false;
                    btnPatientTypeAlter.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Language()
        {
            try
            {
             //btnPatientTypeAlter.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TREATMENT_PROCESSOR_PARTIAL___BTN_PATIENT_TYPE_ALTER", ResourceLanguageManager.LanguageUCTreatmentProcessPartial, LanguageManager.GetCulture());
             //   btnDepartmentTran.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TREATMENT_PROCESSOR_PARTIAL___BTN_DEPARTMENT_TRAN", EXE.APP.Resources.ResourceLanguageManager.LanguageUCTreatmentProcessPartial, EXE.LOGIC.Base.LanguageManager.GetCulture());
             //   btnSplitMediRecord.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TREATMENT_PROCESSOR_PARTIAL___BTN_SPLIT_MEDI_RECORD", EXE.APP.Resources.ResourceLanguageManager.LanguageUCTreatmentProcessPartial, EXE.LOGIC.Base.LanguageManager.GetCulture());
             //   grdColTypeName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TREATMENT_PROCESSOR_PARTIAL___GRD_COL_TYPE_NAME", EXE.APP.Resources.ResourceLanguageManager.LanguageUCTreatmentProcessPartial, EXE.LOGIC.Base.LanguageManager.GetCulture());
             //   grdColTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TREATMENT_PROCESSOR_PARTIAL___GRD_COL_TIME", EXE.APP.Resources.ResourceLanguageManager.LanguageUCTreatmentProcessPartial, EXE.LOGIC.Base.LanguageManager.GetCulture());
             //   grdColDetail.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TREATMENT_PROCESSOR_PARTIAL___GRD_COL_DETAIL", EXE.APP.Resources.ResourceLanguageManager.LanguageUCTreatmentProcessPartial, EXE.LOGIC.Base.LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Load data vào GridViewTreatment
        public void LoadDataToGridTreatmentLog(V_HIS_TREATMENT_4 treatment)
        {
            try
            {
             if (treatment!=null)
                {
                 CommonParam param = new CommonParam();

                 apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HisTreatmentLogSDO>>(HisRequestUriStore.HIS_TREATMENT_LOG_GET, ApiConsumers.MosConsumer, param, treatment.ID, null);
                 if (apiResult != null)
                 {
                  gridviewTreatmentLogs.BeginUpdate();
                  gridviewTreatmentLogs.GridControl.DataSource = apiResult;
                  gridviewTreatmentLogs.EndDataUpdate();
                  
                 }
                    
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                
            }
        }

        string GetDetailHeinInfo(MOS.SDO.HisTreatmentLogSDO data)
        {
            StringBuilder info = new StringBuilder();
            try
            {
             if (data.HisPatientTypeAlter.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT)
                {
                    if (data.HisPatyAlterDetail != null)
                    {
                        var patyAlterBhyt = Newtonsoft.Json.JsonConvert.DeserializeObject<V_HIS_PATY_ALTER_BHYT>(data.HisPatyAlterDetail.ToString());
                        if (patyAlterBhyt != null)
                        {
                            info.Append(". Số thẻ BHYT: " + SetHeinCardNumberDisplayByNumber(SetHeinCardNumberDisplayByNumber(patyAlterBhyt.HEIN_CARD_NUMBER)) + ". Thời hạn: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patyAlterBhyt.HEIN_CARD_FROM_TIME) + " - " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patyAlterBhyt.HEIN_CARD_TO_TIME));
                        }
                    }
                }
                else if (data.HisPatientTypeAlter.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__KSK)
                {
                    var patyAlterKsk = Newtonsoft.Json.JsonConvert.DeserializeObject<V_HIS_PATY_ALTER_KSK>(data.HisPatyAlterDetail.ToString());
                    if (patyAlterKsk != null)
                    {
                        info.Append(". Tên: " + patyAlterKsk.CUSTOMER_NAME + ". Tỉ lệ đóng chi trả: " + (patyAlterKsk.RATIO * 100) + "%" + " - " + ". Trần viện phí: " + Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound((decimal)(patyAlterKsk.MAX_FEE ?? 0)));
                    }
                }
                //else if (data.HisPatientTypeAlter.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__AIA)
                //{
                //    var patyAlterKsk = Newtonsoft.Json.JsonConvert.DeserializeObject<V_HIS_PATY_ALTER_AIA>(data.HisPatyAlterDetail.ToString());
                //    if (patyAlterKsk != null)
                //    {
                //        info.Append(". Mã thẻ: " + patyAlterKsk.POLICY_NUMBER);
                //        if (patyAlterKsk.FROM_TIME > 0)
                //        {
                //            info.Append(". Hạn từ: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patyAlterKsk.FROM_TIME ?? 0));
                //        }
                //        if (patyAlterKsk.TO_TIME > 0)
                //        {
                //            info.Append(" - " + "đến: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patyAlterKsk.TO_TIME ?? 0));
                //        }
                //    }
                //}
                //else if (data.HisPatientTypeAlter.PATIENT_TYPE_ID == EXE.LOGIC.Config.HisPatientTypeCFG.PATIENT_TYPE_ID__BCV)
                //{
                //    var patyAlterDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<V_HIS_PATY_ALTER_BCV>(data.HisPatyAlterDetail.ToString());
                //    if (patyAlterDetail != null)
                //    {
                //        info.Append(". Mã thẻ: " + patyAlterDetail.POLICY_NUMBER);
                //        if (patyAlterDetail.FROM_TIME > 0)
                //        {
                //            info.Append(". Hạn từ: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patyAlterDetail.FROM_TIME ?? 0));
                //        }
                //        if (patyAlterDetail.TO_TIME > 0)
                //        {
                //            info.Append(" - " + "đến: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patyAlterDetail.TO_TIME ?? 0));
                //        }
                //    }
                //}
                //else if (data.HisPatientTypeAlter.PATIENT_TYPE_ID == EXE.LOGIC.Config.HisPatientTypeCFG.PATIENT_TYPE_ID__BIC)
                //{
                //    var patyAlterDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<V_HIS_PATY_ALTER_BIC>(data.HisPatyAlterDetail.ToString());
                //    if (patyAlterDetail != null)
                //    {
                //        info.Append(". Mã thẻ: " + patyAlterDetail.POLICY_NUMBER);
                //        if (patyAlterDetail.FROM_TIME > 0)
                //        {
                //            info.Append(". Hạn từ: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patyAlterDetail.FROM_TIME ?? 0));
                //        }
                //        if (patyAlterDetail.TO_TIME > 0)
                //        {
                //            info.Append(" - " + "đến: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patyAlterDetail.TO_TIME ?? 0));
                //        }
                //    }
                //}
                //else if (data.HisPatientTypeAlter.PATIENT_TYPE_ID == EXE.LOGIC.Config.HisPatientTypeCFG.PATIENT_TYPE_ID__BVTM)
                //{
                //    var patyAlterDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<V_HIS_PATY_ALTER_BVTM>(data.HisPatyAlterDetail.ToString());
                //    if (patyAlterDetail != null)
                //    {
                //        info.Append(". Mã thẻ: " + patyAlterDetail.POLICY_NUMBER);
                //        if (patyAlterDetail.FROM_TIME > 0)
                //        {
                //            info.Append(". Hạn từ: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patyAlterDetail.FROM_TIME ?? 0));
                //        }
                //        if (patyAlterDetail.TO_TIME > 0)
                //        {
                //            info.Append(" - " + "đến: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patyAlterDetail.TO_TIME ?? 0));
                //        }
                //    }
                //}
                //else if (data.HisPatientTypeAlter.PATIENT_TYPE_ID == EXE.LOGIC.Config.HisPatientTypeCFG.PATIENT_TYPE_ID__DAIICHI)
                //{
                //    var patyAlterDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<V_HIS_PATY_ALTER_DAIICHI>(data.HisPatyAlterDetail.ToString());
                //    if (patyAlterDetail != null)
                //    {
                //        info.Append(". Mã thẻ: " + patyAlterDetail.POLICY_NUMBER);
                //        if (patyAlterDetail.FROM_TIME > 0)
                //        {
                //            info.Append(". Hạn từ: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patyAlterDetail.FROM_TIME ?? 0));
                //        }
                //        if (patyAlterDetail.TO_TIME > 0)
                //        {
                //            info.Append(" - " + "đến: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patyAlterDetail.TO_TIME ?? 0));
                //        }
                //    }
                //}
                //else if (data.HisPatientTypeAlter.PATIENT_TYPE_ID == EXE.LOGIC.Config.HisPatientTypeCFG.PATIENT_TYPE_ID__GEL)
                //{
                //    var patyAlterDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<V_HIS_PATY_ALTER_GEL>(data.HisPatyAlterDetail.ToString());
                //    if (patyAlterDetail != null)
                //    {
                //        info.Append(". Mã thẻ: " + patyAlterDetail.POLICY_NUMBER);
                //        if (patyAlterDetail.FROM_TIME > 0)
                //        {
                //            info.Append(". Hạn từ: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patyAlterDetail.FROM_TIME ?? 0));
                //        }
                //        if (patyAlterDetail.TO_TIME > 0)
                //        {
                //            info.Append(" - " + "đến: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patyAlterDetail.TO_TIME ?? 0));
                //        }
                //    }
                //}
                //else if (data.HisPatientTypeAlter.PATIENT_TYPE_ID == EXE.LOGIC.Config.HisPatientTypeCFG.PATIENT_TYPE_ID__GENERALI)
                //{
                //    var patyAlterDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<V_HIS_PATY_ALTER_GENERALI>(data.HisPatyAlterDetail.ToString());
                //    if (patyAlterDetail != null)
                //    {
                //        info.Append(". Mã thẻ: " + patyAlterDetail.POLICY_NUMBER);
                //        if (patyAlterDetail.FROM_TIME > 0)
                //        {
                //            info.Append(". Hạn từ: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patyAlterDetail.FROM_TIME ?? 0));
                //        }
                //        if (patyAlterDetail.TO_TIME > 0)
                //        {
                //            info.Append(" - " + "đến: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patyAlterDetail.TO_TIME ?? 0));
                //        }
                //    }
                //}
                //else if (data.HisPatientTypeAlter.PATIENT_TYPE_ID == EXE.LOGIC.Config.HisPatientTypeCFG.PATIENT_TYPE_ID__INSMART)
                //{
                //    var patyAlterDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<V_HIS_PATY_ALTER_INSMART>(data.HisPatyAlterDetail.ToString());
                //    if (patyAlterDetail != null)
                //    {
                //        info.Append(". Mã thẻ: " + patyAlterDetail.POLICY_NUMBER);
                //        if (patyAlterDetail.FROM_TIME > 0)
                //        {
                //            info.Append(". Hạn từ: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patyAlterDetail.FROM_TIME ?? 0));
                //        }
                //        if (patyAlterDetail.TO_TIME > 0)
                //        {
                //            info.Append(" - " + "đến: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patyAlterDetail.TO_TIME ?? 0));
                //        }
                //    }
                //}
                //else if (data.HisPatientTypeAlter.PATIENT_TYPE_ID == EXE.LOGIC.Config.HisPatientTypeCFG.PATIENT_TYPE_ID__LIBERTY)
                //{
                //    var patyAlterDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<V_HIS_PATY_ALTER_LIBERTY>(data.HisPatyAlterDetail.ToString());
                //    if (patyAlterDetail != null)
                //    {
                //        info.Append(". Mã thẻ: " + patyAlterDetail.POLICY_NUMBER);
                //        if (patyAlterDetail.FROM_TIME > 0)
                //        {
                //            info.Append(". Hạn từ: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patyAlterDetail.FROM_TIME ?? 0));
                //        }
                //        if (patyAlterDetail.TO_TIME > 0)
                //        {
                //            info.Append(" - " + "đến: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patyAlterDetail.TO_TIME ?? 0));
                //        }
                //    }
                //}
                //else if (data.HisPatientTypeAlter.PATIENT_TYPE_ID == EXE.LOGIC.Config.HisPatientTypeCFG.PATIENT_TYPE_ID__PRUD)
                //{
                //    var patyAlterDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<V_HIS_PATY_ALTER_PRUD>(data.HisPatyAlterDetail.ToString());
                //    if (patyAlterDetail != null)
                //    {
                //        info.Append(". Mã thẻ: " + patyAlterDetail.POLICY_NUMBER);
                //        if (patyAlterDetail.FROM_TIME > 0)
                //        {
                //            info.Append(". Hạn từ: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patyAlterDetail.FROM_TIME ?? 0));
                //        }
                //        if (patyAlterDetail.TO_TIME > 0)
                //        {
                //            info.Append(" - " + "đến: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patyAlterDetail.TO_TIME ?? 0));
                //        }
                //    }
                //}
                //else if (data.HisPatientTypeAlter.PATIENT_TYPE_ID == EXE.LOGIC.Config.HisPatientTypeCFG.PATIENT_TYPE_ID__PTI)
                //{
                //    var patyAlterDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<V_HIS_PATY_ALTER_PTI>(data.HisPatyAlterDetail.ToString());
                //    if (patyAlterDetail != null)
                //    {
                //        info.Append(". Mã thẻ: " + patyAlterDetail.POLICY_NUMBER);
                //        if (patyAlterDetail.FROM_TIME > 0)
                //        {
                //            info.Append(". Hạn từ: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patyAlterDetail.FROM_TIME ?? 0));
                //        }
                //        if (patyAlterDetail.TO_TIME > 0)
                //        {
                //            info.Append(" - " + "đến: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patyAlterDetail.TO_TIME ?? 0));
                //        }
                //    }
                //}
                //else if (data.HisPatientTypeAlter.PATIENT_TYPE_ID == EXE.LOGIC.Config.HisPatientTypeCFG.PATIENT_TYPE_ID__PVI)
                //{
                //    var patyAlterDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<V_HIS_PATY_ALTER_PVI>(data.HisPatyAlterDetail.ToString());
                //    if (patyAlterDetail != null)
                //    {
                //        info.Append(". Mã thẻ: " + patyAlterDetail.POLICY_NUMBER);
                //        if (patyAlterDetail.FROM_TIME > 0)
                //        {
                //            info.Append(". Hạn từ: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patyAlterDetail.FROM_TIME ?? 0));
                //        }
                //        if (patyAlterDetail.TO_TIME > 0)
                //        {
                //            info.Append(" - " + "đến: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patyAlterDetail.TO_TIME ?? 0));
                //        }
                //    }
                //}
                //else if (data.HisPatientTypeAlter.PATIENT_TYPE_ID == EXE.LOGIC.Config.HisPatientTypeCFG.PATIENT_TYPE_ID__TMN)
                //{
                //    var patyAlterDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<V_HIS_PATY_ALTER_TMN>(data.HisPatyAlterDetail.ToString());
                //    if (patyAlterDetail != null)
                //    {
                //        info.Append(". Mã thẻ: " + patyAlterDetail.POLICY_NUMBER);
                //        if (patyAlterDetail.FROM_TIME > 0)
                //        {
                //            info.Append(". Hạn từ: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patyAlterDetail.FROM_TIME ?? 0));
                //        }
                //        if (patyAlterDetail.TO_TIME > 0)
                //        {
                //            info.Append(" - " + "đến: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patyAlterDetail.TO_TIME ?? 0));
                //        }
                //    }
                //}
                //else if (data.HisPatientTypeAlter.PATIENT_TYPE_ID == EXE.LOGIC.Config.HisPatientTypeCFG.PATIENT_TYPE_ID__VCLI)
                //{
                //    var patyAlterDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<V_HIS_PATY_ALTER_VCLI>(data.HisPatyAlterDetail.ToString());
                //    if (patyAlterDetail != null)
                //    {
                //        info.Append(". Mã thẻ: " + patyAlterDetail.POLICY_NUMBER);
                //        if (patyAlterDetail.FROM_TIME > 0)
                //        {
                //            info.Append(". Hạn từ: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patyAlterDetail.FROM_TIME ?? 0));
                //        }
                //        if (patyAlterDetail.TO_TIME > 0)
                //        {
                //            info.Append(" - " + "đến: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patyAlterDetail.TO_TIME ?? 0));
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return info.ToString();
        }


        public static string SetHeinCardNumberDisplayByNumber(string heinCardNumber)
        {
         string result = "";
         try
         {
          if (!String.IsNullOrWhiteSpace(heinCardNumber) && heinCardNumber.Length == 15)
          {
           string separateSymbol = "-";
           result = new StringBuilder().Append(heinCardNumber.Substring(0, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(2, 1)).Append(separateSymbol).Append(heinCardNumber.Substring(3, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(5, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(7, 3)).Append(separateSymbol).Append(heinCardNumber.Substring(10, 5)).ToString();
          }
          else
          {
           result = heinCardNumber;
          }
         }
         catch (Exception ex)
         {
          LogSystem.Error(ex);
          result = heinCardNumber;
         }
         return result;
        }



        private void gridviewTreatmentLogs_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.SDO.HisTreatmentLogSDO data = (MOS.SDO.HisTreatmentLogSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "IS_ACTIVE_DISPLAY")
                        {
                         if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                         {
//                                e.Value = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_UC_TREATMENT_PROCESS_PARTIAL_ACTIVITY", EXE.APP.Resources.ResourceLanguageManager.LanguageUCTreatmentProcessPartial
//, EXE.LOGIC.Base.LanguageManager.GetCulture());
//                                ;
                            }
                            else
                            {
//                                e.Value = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_UC_TREATMENT_PROCESS_PARTIAL_INTERIM_LOCK", EXE.APP.Resources.ResourceLanguageManager.LanguageUCTreatmentProcessPartial
//, EXE.LOGIC.Base.LanguageManager.GetCulture());
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "DETAIL_DATA")
                        {
                            long logTypeId = data.TREATMENT_LOG_TYPE_ID;
                            if (data.HisDepartmentTran != null)
                            {
                                if (data.HisDepartmentTran.IN_OUT == IMSys.DbConfig.HIS_RS.HIS_DEPARTMENT_TRAN.IN_OUT__IN)
                                {
                                    if (!String.IsNullOrEmpty(data.HisDepartmentTran.DEPARTMENT_NAME))
                                        e.Value = "Vào: " + data.HisDepartmentTran.DEPARTMENT_NAME + ".";
                                    //if (!String.IsNullOrEmpty(data.HisDepartmentTran.BED_ROOM_NAME))
                                    //    e.Value += " Buồng: " + data.HisDepartmentTran.BED_ROOM_NAME;
                                }
                                else
                                {
                                    string temp = "Ra: " + data.HisDepartmentTran.DEPARTMENT_NAME;
                                    if (!String.IsNullOrEmpty(data.HisDepartmentTran.NEXT_DEPARTMENT_NAME))
                                    {
                                        temp += ". " + "Tiếp theo: " + data.HisDepartmentTran.NEXT_DEPARTMENT_NAME + "." + " Tiếp nhận: " + (data.HisDepartmentTran.IS_RECEIVE == IMSys.DbConfig.HIS_RS.HIS_DEPARTMENT_TRAN.IS_RECEIVE__TRUE ? " Đã tiếp nhận" : " Chưa tiếp nhận");
                                    }
                                    e.Value = temp;
                                }
                            }
                            else if (data.HisMediRecord != null)
                            {
                                e.Value = "Mã: " + data.HisMediRecord.MEDI_RECORD_CODE + "";
                            }
                            else if (data.HisPatientTypeAlter != null)
                            {
                                e.Value = 
                                    "Tên: "+ data.HisPatientTypeAlter.PATIENT_TYPE_NAME;
                                e.Value += GetDetailHeinInfo(data);
                                e.Value += "." + " Loại ĐT: " + data.HisPatientTypeAlter.TREATMENT_TYPE_NAME;
                                e.Value += ".";
                            }
                        }
                        else if (e.Column.FieldName == "LOG_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.LOG_TIME);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion



        private void btnPatientTypeAlter_Click(object sender, EventArgs e)
        {
            try 
            {
             Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.CallPatientTypeAlter").FirstOrDefault();
             if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.CallPatientTypeAlter'");
             if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
             {
              List<object> listArgs = new List<object>();
              
              listArgs.Add((RefeshReference)Init);
              listArgs.Add(true);
              listArgs.Add(currentTreatmentId);

							listArgs .Add(PluginInstance .GetModuleWithWorkingRoom(moduleData ,this .currentModule.RoomId,this.currentModule.RoomTypeId));
							var extenceInstance = PluginInstance .GetPluginInstance(PluginInstance .GetModuleWithWorkingRoom(moduleData ,this .currentModule .RoomId ,this .currentModule .RoomTypeId) ,listArgs);
              if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

              ((Form)extenceInstance).ShowDialog();
             }
             }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDepartmentTran_Click(object sender, EventArgs e)
        {
            try
            {
            Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransDepartment").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TransDepartment'");
               
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    TransDepartmentADO transDepartmenADO = new TransDepartmentADO(currentTreatment.ID);
                    //Set data to assignBloodADO
                    transDepartmenADO.TreatmentId = currentTreatment.ID;
                    transDepartmenADO.DepartmentId = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == this.currentRoomId).First().DEPARTMENT_ID;
										moduleData.RoomId = this.currentRoomId;
                    listArgs.Add(this.currentRoomId);
                    listArgs.Add(transDepartmenADO);
										listArgs.Add((RefeshReference)Init);
										listArgs .Add(PluginInstance .GetModuleWithWorkingRoom(moduleData ,this .currentModule .RoomId ,this .currentModule .RoomTypeId));
										var extenceInstance = PluginInstance .GetPluginInstance(PluginInstance .GetModuleWithWorkingRoom(moduleData ,this .currentModule .RoomId ,this .currentModule .RoomTypeId) ,listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                   ((Form)extenceInstance).ShowDialog();
               }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void callOutInputTreatment(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
         try
         {
          var row = (HisTreatmentLogSDO)gridviewTreatmentLogs.GetFocusedRow();
          Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.CallOutInputTreatment").FirstOrDefault();
          if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.CallOutInputTreatment'");
          if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
          {
           List<object> listArgs = new List<object>();
           listArgs.Add((HisTreatmentLogSDO)row);
           listArgs.Add((RefeshReference)Init);
					 //listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData,));
					 var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData ,this .currentModule.RoomId,this.currentModule.RoomTypeId),listArgs); 
							 if(extenceInstance == null) throw new ArgumentNullException("moduleData is null");

           ((Form)extenceInstance).ShowDialog();
          }
         }
         catch (Exception ex)
         {
          Inventec.Common.Logging.LogSystem.Warn(ex);
         }
        }


        private void btnSplitMediRecord_Click(object sender, EventArgs e)
        {
            try
            {
             Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.CallBriefPatient").FirstOrDefault();
             if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.CallBriefPatient'");
             if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
             {
              List<object> listArgs = new List<object>();
             
              MOS.SDO.HisTreatmentLogSDO treatmentLog = (HisTreatmentLogSDO)gridviewTreatmentLogs.GetFocusedRow();
treatmentLog.HisMediRecord=null;
              listArgs.Add(treatmentLog);
              listArgs.Add(this.currentModule);

              listArgs.Add((RefeshReference)Init);
							//listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData));
							var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData,this.currentModule.RoomId,this.currentModule.RoomTypeId),listArgs);
              if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

              ((Form)extenceInstance).ShowDialog();
             }
           

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItembtnUpdate_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
             MOS.SDO.HisTreatmentLogSDO treatmentLog = (MOS.SDO.HisTreatmentLogSDO)gridviewTreatmentLogs.GetFocusedRow();
             if (treatmentLog != null)
             {
              bool isView = false;
              if (treatmentLog.TREATMENT_LOG_TYPE_ID == logTypeId__DepartmentTran)
              {
               isView = IsViewForm(treatmentLog.HisDepartmentTran);
               callOutInputTreatment(sender,e);
              }
              else if (treatmentLog.TREATMENT_LOG_TYPE_ID == logTypeId__MediRecord)
              {
               isView = IsViewForm(null);
               try
               {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.CallBriefPatient").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.CallBriefPatient'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                 List<object> listArgs = new List<object>();
                
                 listArgs.Add(treatmentLog);
                 listArgs.Add(this.currentModule);
                 listArgs.Add(isView);

                 listArgs.Add((RefeshReference)Init);
								 //listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData));
								 var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData,this.currentModule.RoomId,this.currentModule.RoomTypeId),listArgs);
                 if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                 ((Form)extenceInstance).ShowDialog();
                }
               
               }
               catch (Exception ex)
               {
                Inventec.Common.Logging.LogSystem.Warn(ex);
               }
              }
              else if (treatmentLog.TREATMENT_LOG_TYPE_ID == logTypeId__patientTypeAlter)
              {
               isView = IsViewForm(null);
               try
               {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.CallPatientTypeAlter").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.CallPatientTypeAlter'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                 List<object> listArgs = new List<object>();
                 listArgs.Add(treatmentLog);
                 listArgs.Add(this.currentModule);
                 listArgs.Add(isView);

                 listArgs.Add((RefeshReference)Init);
								 //listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData));
								 var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData,this.currentModule.RoomId,this.currentModule.RoomTypeId),listArgs);
                 if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                 ((Form)extenceInstance).ShowDialog();
                }
                }
               catch (Exception ex)
               {
                Inventec.Common.Logging.LogSystem.Warn(ex);
               }
              }
             }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItembtnDelete_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            bool notHandler = false;
            try
            {
             long treatmentId = this.currentTreatmentId;

             MOS.SDO.HisTreatmentLogSDO dataTreatmentLog = (MOS.SDO.HisTreatmentLogSDO)gridviewTreatmentLogs.GetFocusedRow();
             if (dataTreatmentLog != null)
             {
              if (MsgBox.Show(MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong), MsgBox.CaptionEnum.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
              {
               WaitingManager.Show();
               if (dataTreatmentLog.TREATMENT_LOG_TYPE_ID == logTypeId__DepartmentTran && dataTreatmentLog.HisDepartmentTran != null)
               {
                success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_TREATMENT_LOG_DELETE_DEPARTMENT, ApiConsumers.MosConsumer, dataTreatmentLog.HisDepartmentTran.ID, null);
//HisTreatmentLog.HisTreatmentLogLogic(param).DepartmentDelete<bool>(dataTreatmentLog.HisDepartmentTran.ID);
                WaitingManager.Hide();
                if (success) { LoadDataToGridTreatmentLog(currentTreatment); }
               }
               else if (dataTreatmentLog.TREATMENT_LOG_TYPE_ID == logTypeId__MediRecord)
               {
                success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_TREATMENT_LOG_DELETE_MEDI_RECORD, ApiConsumers.MosConsumer, dataTreatmentLog.HisMediRecord.ID, null); 
//EXE.LOGIC.HisTreatmentLog.HisTreatmentLogLogic(param).MediRecordDelete<bool>(dataTreatmentLog.HisMediRecord.ID);
                WaitingManager.Hide();
                if (success) { LoadDataToGridTreatmentLog(currentTreatment); }
               }
               else if (dataTreatmentLog.TREATMENT_LOG_TYPE_ID == logTypeId__patientTypeAlter)
               {
                success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_TREATMENT_LOG_DELETE_PATIENT_TYPE_ALTER, ApiConsumers.MosConsumer, dataTreatmentLog.HisPatientTypeAlter.ID, null);
//EXE.LOGIC.HisTreatmentLog.HisTreatmentLogLogic(param).PatientTypeAlterDelete<bool>(dataTreatmentLog.HisPatientTypeAlter.ID);
                WaitingManager.Hide();
                if (success) { LoadDataToGridTreatmentLog(currentTreatment); }
               }
               WaitingManager.Hide();
              }
              else
              {
               notHandler = true;
              }
             }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
                MessageUtil.SetParam(param, LibraryMessage.Message.Enum.HeThongTBXuatHienExceptionChuaKiemDuocSoat);
            }

            if (!notHandler)
            {
             #region Show message
             MessageManager.Show( param, success);
             #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
        }

        private void gridviewTreatmentLogs_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                MOS.SDO.HisTreatmentLogSDO data = null;
                if (e.RowHandle >= 0)
                {
                    bool isDisableButton = false;
                    bool isDisableDelButton = false;
                    data = (MOS.SDO.HisTreatmentLogSDO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "FieldBtnDelete")
                    {
                        isDisableDelButton = IsViewForm(data.HisDepartmentTran);
                        e.RepositoryItem = (isDisableDelButton == true ? repositoryItembtnDeleteDisable : repositoryItembtnDelete);
                    }
                    if (e.Column.FieldName == "FieldBtnUpdate")
                    {
                        isDisableButton = IsViewForm(data.HisDepartmentTran);
                        e.RepositoryItem = (isDisableButton == true ? repositoryItembtnUpdateDisable : repositoryItembtnUpdate);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool IsTreatmentPause()
        {
            bool result = false;
            try
            {
                //this.currentTreatment = PrintGlobalStore.getTreatment(this.treatmentId);
                result = (this.currentTreatment.IS_PAUSE == IMSys.DbConfig.HIS_RS.HIS_TREATMENT.IS_PAUSE__TRUE);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        bool IsDepartmentOut()
        {
            bool result = false;
            try
            {

						result = ((this.apiResult.Where(o => o.TREATMENT_LOG_TYPE_ID==HisTreatmentLogTypeCFG.TREATMENT_LOG_TYPE_ID__DEPARTMENT_TRAN).OrderByDescending(p=>p.ID).FirstOrDefault().HisDepartmentTran.DEPARTMENT_ID != BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == this.currentRoomId).Select(p => p.DEPARTMENT_ID).First()));
            }
            catch (Exception ex)
            {
             Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        bool IsViewForm(V_HIS_DEPARTMENT_TRAN data)
        {
            bool result = false;
            try
            {
                if (
                     IsTreatmentPause() || IsDepartmentOut())
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void UCTreatmentProcessPartial_Load(object sender, EventArgs e)
        {
            try
            {
                Language();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
