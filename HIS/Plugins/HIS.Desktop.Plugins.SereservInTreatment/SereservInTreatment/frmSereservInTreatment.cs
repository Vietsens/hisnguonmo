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
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Inventec.Common.Logging;
using DevExpress.Data;
using MOS.Filter;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.ADO;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.LocalStorage.BackendData;

namespace HIS.Desktop.Plugins.SereservInTreatment.SereservInTreatment
{
    public partial class frmSereservInTreatment : HIS.Desktop.Utility.FormBase
    {
        long treatmentId = 0;
        MOS.EFMODEL.DataModels.V_HIS_TREATMENT currentHisTreatment { get; set; }
        MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER currentHisPatientTypeAlter = null;
        Dictionary<long, HIS_PATIENT_TYPE> dicPatientType;
        Inventec.Desktop.Common.Modules.Module currentModule;

        public frmSereservInTreatment(Inventec.Desktop.Common.Modules.Module module, SereservInTreatmentADO sereservInTreatmentADO)
		:base(module)
        {
            try
            {
                InitializeComponent();

                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                    this.currentModule = module;
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }

                this.treatmentId = sereservInTreatmentADO.TreatmentId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmSereservInTreatment_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                dicPatientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().ToDictionary(o => o.ID, o => o);
                FillDataPatientInfo();
                FillDataToGird();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.SereservInTreatment.Resources.Lang", typeof(HIS.Desktop.Plugins.SereservInTreatment.SereservInTreatment.frmSereservInTreatment).Assembly);
                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.gridColSerSevSTT.Caption = Inventec.Common.Resource.Get.Value("frmSereservInTreatment.gridColSerSevSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColSerSevView.Caption = Inventec.Common.Resource.Get.Value("frmSereservInTreatment.gridColSerSevView.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColSerSevPrint.Caption = Inventec.Common.Resource.Get.Value("frmSereservInTreatment.gridColSerSevPrint.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gcolServiceTypeName.Caption = Inventec.Common.Resource.Get.Value("frmSereservInTreatment.gcolServiceTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmSereservInTreatment.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColSerSevCode.Caption = Inventec.Common.Resource.Get.Value("frmSereservInTreatment.gridColSerSevCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColSerSevName.Caption = Inventec.Common.Resource.Get.Value("frmSereservInTreatment.gridColSerSevName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColSerSevUnitName.Caption = Inventec.Common.Resource.Get.Value("frmSereservInTreatment.gridColSerSevUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColSerSevAmount.Caption = Inventec.Common.Resource.Get.Value("frmSereservInTreatment.gridColSerSevAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColSerSevTypeName.Caption = Inventec.Common.Resource.Get.Value("frmSereservInTreatment.gridColSerSevTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColOtherPrintForm.Caption = Inventec.Common.Resource.Get.Value("frmSereservInTreatment.gridColOtherPrintForm.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmSereservInTreatment.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcilblTreatmentCode.Text = Inventec.Common.Resource.Get.Value("frmSereservInTreatment.lcilblTreatmentCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcilblPatientName.Text = Inventec.Common.Resource.Get.Value("frmSereservInTreatment.lcilblPatientName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcilblPatientDob.Text = Inventec.Common.Resource.Get.Value("frmSereservInTreatment.lcilblPatientDob.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcilblGenderName.Text = Inventec.Common.Resource.Get.Value("frmSereservInTreatment.lcilblGenderName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcilblPatientAddress.Text = Inventec.Common.Resource.Get.Value("frmSereservInTreatment.lcilblPatientAddress.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcilblPatientTypeName.Text = Inventec.Common.Resource.Get.Value("frmSereservInTreatment.lcilblPatientTypeName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private MOS.EFMODEL.DataModels.V_HIS_TREATMENT LoadDataToCurrentTreatmentData(long treatmentId)
        {
            MOS.EFMODEL.DataModels.V_HIS_TREATMENT treatment = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentViewFilter filter = new MOS.Filter.HisTreatmentViewFilter();
                filter.ID = treatmentId;

                var listTreatment = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (listTreatment != null && listTreatment.Count > 0)
                {
                    treatment = listTreatment[0];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return treatment;
        }

        private MOS.EFMODEL.DataModels.V_HIS_PATIENT LoadDataToCurrentPatientData(long patientId)
        {
            MOS.EFMODEL.DataModels.V_HIS_PATIENT patient = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientViewFilter filter = new MOS.Filter.HisPatientViewFilter();
                filter.ID = patientId;
                patient = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param).SingleOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return patient;
        }

        private void LoadCurrentPatientTypeAlter(long treatmentId, long intructionTime, ref MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER hisPatientTypeAlter)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
                filter.TreatmentId = treatmentId;
                if (intructionTime > 0)
                    filter.InstructionTime = intructionTime;
                else
                    filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                hisPatientTypeAlter = new BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataPatientInfo()
        {
            try
            {
                if (this.treatmentId > 0)
                {
                    this.currentHisTreatment = LoadDataToCurrentTreatmentData(treatmentId);                  
                    LoadCurrentPatientTypeAlter(treatmentId, 0, ref currentHisPatientTypeAlter);

                    lblTreatmentCode.Text = this.currentHisTreatment.TREATMENT_CODE;
                    lblPatientName.Text = this.currentHisTreatment.TDL_PATIENT_NAME;
                    lblPatientTypeName.Text = currentHisPatientTypeAlter.PATIENT_TYPE_NAME;
                    lblGenderName.Text = this.currentHisTreatment.TDL_PATIENT_GENDER_NAME;
                    lblPatientAddress.Text = this.currentHisTreatment.TDL_PATIENT_ADDRESS;
                    lblPatientDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentHisTreatment.TDL_PATIENT_DOB) + "(" + HIS.Desktop.Utility.AgeHelper.CalculateAgeFromYear(this.currentHisTreatment.TDL_PATIENT_DOB) + ")";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGird()
        {
            try
            {
                WaitingManager.Show();
                if (this.treatmentId > 0)
                {
                    CommonParam paramCommon = new CommonParam();
                    List<HIS_SERE_SERV> apiResult = null;
                    HisSereServFilter filter = new HisSereServFilter();
                    filter.TREATMENT_ID = this.treatmentId;
                    grdViewSereServServiceReqTab.BeginUpdate();
                    apiResult = new BackendAdapter(paramCommon).Get<List<HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    if (apiResult != null)
                    {
                        List<V_HIS_SERE_SERV_5> data = new List<V_HIS_SERE_SERV_5>();
                        foreach (var item in apiResult)
                        {
                            V_HIS_SERE_SERV_5 dss = new V_HIS_SERE_SERV_5();
                            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV_5>(dss, item);
                            dss.PATIENT_TYPE_NAME = (dicPatientType[dss.PATIENT_TYPE_ID] ?? new HIS_PATIENT_TYPE()).PATIENT_TYPE_NAME;
                            dss.SERVICE_TYPE_NAME = (BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(o => o.ID == item.TDL_SERVICE_TYPE_ID) ?? new HIS_SERVICE_TYPE()).SERVICE_TYPE_NAME;
                            data.Add(dss);
                        }

                        data = data.OrderBy(o => o.TDL_INTRUCTION_TIME).ThenBy(o => o.SERVICE_TYPE_NAME).ThenBy(o => o.TDL_SERVICE_NAME).ToList();
                        grdViewSereServServiceReqTab.GridControl.DataSource = data;
                    }
                    grdViewSereServServiceReqTab.EndUpdate();

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                    #endregion
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }
        
        private void grdViewSereServServiceReqTab_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5 data = (MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "INTRUCTION_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.TDL_INTRUCTION_TIME);
                        }
                        else if (e.Column.FieldName == "PATIENT_TYPE_NAME_DISPLAY")
                        {
                            e.Value = (dicPatientType[data.PATIENT_TYPE_ID] ?? new HIS_PATIENT_TYPE()).PATIENT_TYPE_NAME;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grdViewSereServServiceReqTab_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            try
            {
                var info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
                string rowValue = Convert.ToString(grdViewSereServServiceReqTab.GetGroupRowValue(e.RowHandle, gcolServiceTypeName));
                info.GroupText = rowValue;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
    }
}
