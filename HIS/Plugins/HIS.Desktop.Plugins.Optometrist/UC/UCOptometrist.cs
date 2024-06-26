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
using MOS.EFMODEL.DataModels;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using HIS.Desktop.LocalStorage.BackendData;
using ACS.EFMODEL.DataModels;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.ViewInfo;
using MOS.SDO;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;

namespace HIS.Desktop.Plugins.Optometrist.UC
{
    public partial class UCOptometrist : UserControlBase
    {
        private Inventec.Desktop.Common.Modules.Module currentModule;
        private long serviceReqId;
        private HIS_SERVICE_REQ serviceReq;
        private List<ACS_USER> listDataUser = new List<ACS_USER>();
        private int positionHandleControl = -1;

        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        const string moduleLink = "HIS.Desktop.Plugins.Optometrist";

        public UCOptometrist()
        {
            InitializeComponent();
        }

        public UCOptometrist(Inventec.Desktop.Common.Modules.Module currentModule, long serviceReqId)
            : base(currentModule)
        {
            InitializeComponent();
            this.currentModule = currentModule;
            this.serviceReqId = serviceReqId;
        }

        private void UCOptometrist_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetCaptionByLanguageKey();

                isNotLoadWhileChangeControlStateInFirst = true;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkEnd.Name)
                        {
                            chkEnd.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkPrint.Name)
                        {
                            chkPrint.Checked = item.VALUE == "1";
                        }
                    }
                }

                InitCboExecuteName();
                GetServiceReq();
                ResetDataControl();
                FillDataControl();
                FillDataGrid();
                ValidateControl();
                btnSave.Enabled = serviceReq.SERVICE_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT;
                btnEndReq.Enabled = serviceReq.SERVICE_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT;
                isNotLoadWhileChangeControlStateInFirst = false;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.Optometrist.Resources.Lang", typeof(HIS.Desktop.Plugins.Optometrist.UC.UCOptometrist).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkEnd.Properties.Caption = Inventec.Common.Resource.Get.Value("UCOptometrist.chkEnd.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPrint.Properties.Caption = Inventec.Common.Resource.Get.Value("UCOptometrist.chkPrint.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEndReq.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.btnEndReq.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl9.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.labelControl9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl8.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.labelControl8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl10.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.labelControl6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl7.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.labelControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl5.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.labelControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl2.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.labelControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl1.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.labelControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAgain.Properties.Caption = Inventec.Common.Resource.Get.Value("UCOptometrist.chkAgain.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkFirst.Properties.Caption = Inventec.Common.Resource.Get.Value("UCOptometrist.chkFirst.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTimeType.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciTimeType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciOptometristTime.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciOptometristTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciThiLuc.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciThiLuc.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciForesightRight.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCOptometrist.lciForesightRight.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciForesightRight.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciForesightRight.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciForesightRightGlass.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciForesightRightGlass.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciForesightRightUsingGlass.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciForesightRightUsingGlass.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciGlassDegreeR.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciGlassDegreeR.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciForesightLeft.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCOptometrist.lciForesightLeft.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciForesightLeft.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciForesightLeft.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciForesightLeftGlass.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciForesightLeftGlass.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciForesightLeftUsingGlass.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciForesightLeftUsingGlass.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciGlassDegreeL.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciGlassDegreeL.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRefractometer.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciRefractometer.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRefractometry.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciRefractometry.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMicroscopy.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciMicroscopy.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRefractometryRight.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCOptometrist.lciRefractometryRight.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRefractometryRight.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciRefractometryRight.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRefractometryLeft.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciRefractometryLeft.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRefractometryLeft.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCOptometrist.lciRefractometryLeft.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBeforeLightReflectionR.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCOptometrist.lciBeforeLightReflectionR.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBeforeLightReflectionR.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciBeforeLightReflectionR.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBeforeLightReflectionL.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCOptometrist.lciBeforeLightReflectionL.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBeforeLightReflectionL.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciBeforeLightReflectionL.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAfterLightReflectionL.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCOptometrist.lciAfterLightReflectionL.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAfterLightReflectionL.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciAfterLightReflectionL.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAfterLightReflectionR.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCOptometrist.lciAfterLightReflectionR.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAfterLightReflectionR.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciAfterLightReflectionR.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAjustableGlass.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciAjustableGlass.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAjustableGlassR.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCOptometrist.lciAjustableGlassR.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAjustableGlassR.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciAjustableGlassR.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAjustableGlassL.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCOptometrist.lciAjustableGlassL.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAjustableGlassL.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciAjustableGlassL.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNearsightGlass.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciNearsightGlass.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNearsightGlassRight.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCOptometrist.lciNearsightGlassRight.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNearsightGlassRight.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciNearsightGlassRight.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNearsightGlassLeft.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCOptometrist.lciNearsightGlassLeft.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNearsightGlassLeft.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciNearsightGlassLeft.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNearsightGlassReadingDist.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciNearsightGlassReadingDist.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNearsightGlassPupilDist.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCOptometrist.lciNearsightGlassPupilDist.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNearsightGlassPupilDist.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciNearsightGlassPupilDist.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciReoptometristAppointment.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciReoptometristAppointment.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_ServiceCode.Caption = Inventec.Common.Resource.Get.Value("UCOptometrist.Gc_ServiceCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_ServiceName.Caption = Inventec.Common.Resource.Get.Value("UCOptometrist.Gc_ServiceName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_ServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("UCOptometrist.Gc_ServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_ServiceUnitName.ToolTip = Inventec.Common.Resource.Get.Value("UCOptometrist.Gc_ServiceUnitName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_Amount.Caption = Inventec.Common.Resource.Get.Value("UCOptometrist.Gc_Amount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExecuteName.Properties.NullText = Inventec.Common.Resource.Get.Value("UCOptometrist.cboExecuteName.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientCode.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciPatientCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientName.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciPatientName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDob.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciDob.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAddress.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciAddress.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciReqName.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.lciReqName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciReqName.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateControl()
        {
            try
            {
                ValidationControlMaxLength(txtAfterLightReflectionL, 50, false);
                ValidationControlMaxLength(txtAfterLightReflectionR, 50, false);
                ValidationControlMaxLength(txtAjustableGlass, 50, false);
                ValidationControlMaxLength(txtAjustableGlassL, 50, false);
                ValidationControlMaxLength(txtAjustableGlassR, 50, false);
                ValidationControlMaxLength(txtBeforeLightReflectionL, 50, false);
                ValidationControlMaxLength(txtBeforeLightReflectionR, 50, false);
                ValidationControlMaxLength(txtForesightLeft, 50, false);
                ValidationControlMaxLength(txtForesightLeftGlass, 50, false);
                ValidationControlMaxLength(txtForesightLeftUsingGlass, 50, false);
                ValidationControlMaxLength(txtForesightRight, 50, false);
                ValidationControlMaxLength(txtForesightRightGlass, 50, false);
                ValidationControlMaxLength(txtForesightRightUsingGlass, 50, false);
                ValidationControlMaxLength(txtGlassDegreeL, 50, false);
                ValidationControlMaxLength(txtGlassDegreeR, 50, false);
                ValidationControlMaxLength(txtNearsightGlassLeft, 50, false);
                ValidationControlMaxLength(txtNearsightGlassPupilDist, 50, false);
                ValidationControlMaxLength(txtNearsightGlassReadingDist, 50, false);
                ValidationControlMaxLength(txtNearsightGlassRight, 50, false);
                ValidationControlMaxLength(txtOptometristTime, 100, false);
                ValidationControlMaxLength(txtRefractometryLeft, 50, false);
                ValidationControlMaxLength(txtRefractometryRight, 50, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationControlMaxLength(BaseEdit control, int? maxLength, bool IsRequired)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxLength;
                validate.IsRequired = IsRequired;
                validate.ErrorText = "Nhập quá kí tự cho phép [" + maxLength + "]";
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetServiceReq()
        {
            try
            {
                if (serviceReqId > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                    filter.ID = serviceReqId;
                    filter.IS_ACTIVE = 1;
                    var apiResult = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>
                        (ApiConsumer.HisRequestUriStore.HIS_SERVICE_REQ_GET_, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        serviceReq = apiResult.First();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataGrid()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisSereServFilter filter = new MOS.Filter.HisSereServFilter();
                filter.SERVICE_REQ_ID = serviceReq.ID;
                filter.IS_ACTIVE = 1;
                var apiResult = new BackendAdapter(paramCommon).Get<List<HIS_SERE_SERV>>
                    (ApiConsumer.HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (apiResult != null && apiResult.Count > 0)
                {
                    gridControlSereServ.DataSource = apiResult;
                    gridControlSereServ.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataControl()
        {
            try
            {
                if (serviceReq != null)
                {
                    lblAddress.Text = serviceReq.TDL_PATIENT_ADDRESS;
                    lblPatientCode.Text = serviceReq.TDL_PATIENT_CODE;
                    lblPatientName.Text = serviceReq.TDL_PATIENT_NAME;
                    lblReqName.Text = string.Format("{1} - {0}", serviceReq.REQUEST_USERNAME, serviceReq.REQUEST_LOGINNAME);
                    if (serviceReq.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                        lblDob.Text = serviceReq.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    else
                        lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(serviceReq.TDL_PATIENT_DOB);

                    txtAfterLightReflectionL.Text = serviceReq.AFTER_LIGHT_REFLECTION_RIGHT;
                    txtAfterLightReflectionR.Text = serviceReq.AFTER_LIGHT_REFLECTION_LEFT;
                    txtAjustableGlass.Text = serviceReq.AJUSTABLE_GLASS_FORESIGHT;
                    txtAjustableGlassL.Text = serviceReq.AJUSTABLE_GLASS_FORESIGHT_L;
                    txtAjustableGlassR.Text = serviceReq.AJUSTABLE_GLASS_FORESIGHT_R;
                    txtBeforeLightReflectionL.Text = serviceReq.BEFORE_LIGHT_REFLECTION_LEFT;
                    txtBeforeLightReflectionR.Text = serviceReq.BEFORE_LIGHT_REFLECTION_RIGHT;
                    txtForesightLeft.Text = serviceReq.FORESIGHT_LEFT_EYE;
                    txtForesightLeftGlass.Text = serviceReq.FORESIGHT_LEFT_GLASS_HOLE;
                    txtForesightLeftUsingGlass.Text = serviceReq.FORESIGHT_LEFT_USING_GLASS;
                    txtForesightRight.Text = serviceReq.FORESIGHT_RIGHT_EYE;
                    txtForesightRightGlass.Text = serviceReq.FORESIGHT_RIGHT_GLASS_HOLE;
                    txtForesightRightUsingGlass.Text = serviceReq.FORESIGHT_RIGHT_USING_GLASS;
                    txtGlassDegreeL.Text = serviceReq.FORESIGHT_USING_GLASS_DEGREE_L;
                    txtGlassDegreeR.Text = serviceReq.FORESIGHT_USING_GLASS_DEGREE_R;
                    txtNearsightGlassLeft.Text = serviceReq.NEARSIGHT_GLASS_LEFT_EYE;
                    txtNearsightGlassPupilDist.Text = serviceReq.NEARSIGHT_GLASS_PUPIL_DIST;
                    txtNearsightGlassReadingDist.Text = serviceReq.NEARSIGHT_GLASS_READING_DIST;
                    txtNearsightGlassRight.Text = serviceReq.NEARSIGHT_GLASS_RIGHT_EYE;
                    txtOptometristTime.Text = serviceReq.OPTOMETRIST_TIME;
                    txtRefractometryLeft.Text = serviceReq.REFACTOMETRY_LEFT_EYE;
                    txtRefractometryRight.Text = serviceReq.REFACTOMETRY_RIGHT_EYE;
                    if (serviceReq.REOPTOMETRIST_APPOINTMENT.HasValue)
                    {
                        spReoptometristAppointment.Value = serviceReq.REOPTOMETRIST_APPOINTMENT.Value;
                    }

                    if (serviceReq.IS_FIRST_OPTOMETRIST == 1)
                    {
                        chkFirst.Checked = true;
                        chkAgain.Checked = false;
                    }
                    else
                    {
                        chkFirst.Checked = false;
                        chkAgain.Checked = true;
                    }

                    txtExecuteLoginname.Text = serviceReq.EXECUTE_LOGINNAME;
                }

                string loginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var user = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME == loginname);
                if (user != null)
                {
                    txtExecuteLoginname.Text = user.LOGINNAME;
                    cboExecuteName.EditValue = user.LOGINNAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetDataControl()
        {
            try
            {
                lblAddress.Text = "";
                lblPatientCode.Text = "";
                lblPatientName.Text = "";
                lblReqName.Text = "";
                lblDob.Text = "";
                txtAfterLightReflectionL.Text = "";
                txtAfterLightReflectionR.Text = "";
                txtAjustableGlass.Text = "";
                txtAjustableGlassL.Text = "";
                txtAjustableGlassR.Text = "";
                txtBeforeLightReflectionL.Text = "";
                txtBeforeLightReflectionR.Text = "";
                txtExecuteLoginname.Text = "";
                txtForesightLeft.Text = "";
                txtForesightLeftGlass.Text = "";
                txtForesightLeftUsingGlass.Text = "";
                txtForesightRight.Text = "";
                txtForesightRightGlass.Text = "";
                txtForesightRightUsingGlass.Text = "";
                txtGlassDegreeL.Text = "";
                txtGlassDegreeR.Text = "";
                txtNearsightGlassLeft.Text = "";
                txtNearsightGlassPupilDist.Text = "";
                txtNearsightGlassReadingDist.Text = "";
                txtNearsightGlassRight.Text = "";
                txtOptometristTime.Text = "";
                txtRefractometryLeft.Text = "";
                txtRefractometryRight.Text = "";
                spReoptometristAppointment.EditValue = null;
                chkFirst.Checked = true;
                chkAgain.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitCboExecuteName()
        {
            try
            {
                List<ACS_USER> datas = BackendDataWorker.Get<ACS_USER>();
                List<HIS_EMPLOYEE> employeeList = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>();
                listDataUser = new List<ACS_USER>();

                foreach (var item in employeeList)
                {
                    if (String.IsNullOrWhiteSpace(item.LOGINNAME)) continue;

                    ACS_USER user = new ACS_USER();
                    user.LOGINNAME = item.LOGINNAME;
                    var check = datas.FirstOrDefault(o => o.LOGINNAME == item.LOGINNAME);
                    if (check != null)
                    {
                        user.USERNAME = check.USERNAME;
                        user.MOBILE = check.MOBILE;
                        user.PASSWORD = check.PASSWORD;
                    }

                    listDataUser.Add(user);
                }

                listDataUser = listDataUser.OrderBy(o => o.USERNAME).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 750);
                ControlEditorLoader.Load(cboExecuteName, listDataUser, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExecuteLoginname_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToLower();
                    LoadExecuteName(strValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExecuteName(string searchCode)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboExecuteName.Focus();
                    cboExecuteName.ShowPopup();
                }
                else
                {
                    var data = listDataUser.Where(o => o.LOGINNAME.ToLower().Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            txtExecuteLoginname.Text = data[0].LOGINNAME;
                            cboExecuteName.EditValue = data[0].LOGINNAME;
                            chkFirst.Focus();
                            chkFirst.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.LOGINNAME == searchCode);
                            if (search != null)
                            {
                                cboExecuteName.EditValue = search.LOGINNAME;
                                chkFirst.Focus();
                                chkFirst.SelectAll();
                            }
                            else
                            {
                                cboExecuteName.EditValue = null;
                                cboExecuteName.Focus();
                                cboExecuteName.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboExecuteName.EditValue = null;
                        cboExecuteName.Focus();
                        cboExecuteName.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExecuteName_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (cboExecuteName.EditValue != null)
                    {
                        var data = listDataUser.FirstOrDefault(o => o.LOGINNAME.ToLower() == (cboExecuteName.EditValue ?? 0).ToString().ToLower());
                        if (data != null)
                        {
                            txtExecuteLoginname.Text = data.LOGINNAME;
                            chkFirst.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExecuteName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboExecuteName.EditValue != null)
                    {
                        var data = listDataUser.FirstOrDefault(o => o.LOGINNAME.ToLower() == (cboExecuteName.EditValue ?? 0).ToString().ToLower());
                        if (data != null)
                        {
                            txtExecuteLoginname.Text = data.LOGINNAME;
                            chkFirst.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkFirst_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkAgain.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAgain_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtOptometristTime.Focus();
                    txtOptometristTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtOptometristTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtForesightRight.Focus();
                    txtForesightRight.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtForesightRight_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtForesightRightGlass.Focus();
                    txtForesightRightGlass.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtForesightRightGlass_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtForesightRightUsingGlass.Focus();
                    txtForesightRightUsingGlass.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtForesightRightUsingGlass_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtGlassDegreeR.Focus();
                    txtGlassDegreeR.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtGlassDegreeR_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtForesightLeft.Focus();
                    txtForesightLeft.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtForesightLeft_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtForesightLeftGlass.Focus();
                    txtForesightLeftGlass.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtForesightLeftGlass_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtForesightLeftUsingGlass.Focus();
                    txtForesightLeftUsingGlass.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtForesightLeftUsingGlass_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtGlassDegreeL.Focus();
                    txtGlassDegreeL.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtGlassDegreeL_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtRefractometryRight.Focus();
                    txtRefractometryRight.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtRefractometryRight_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBeforeLightReflectionR.Focus();
                    txtBeforeLightReflectionR.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBeforeLightReflectionR_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAfterLightReflectionR.Focus();
                    txtAfterLightReflectionR.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtAfterLightReflectionR_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtRefractometryLeft.Focus();
                    txtRefractometryLeft.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtRefractometryLeft_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBeforeLightReflectionL.Focus();
                    txtBeforeLightReflectionL.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBeforeLightReflectionL_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAfterLightReflectionL.Focus();
                    txtAfterLightReflectionL.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtAfterLightReflectionL_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAjustableGlass.Focus();
                    txtAjustableGlass.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtAjustableGlass_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAjustableGlassR.Focus();
                    txtAjustableGlassR.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtAjustableGlassR_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAjustableGlassL.Focus();
                    txtAjustableGlassL.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtAjustableGlassL_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNearsightGlassRight.Focus();
                    txtNearsightGlassRight.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNearsightGlassRight_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNearsightGlassLeft.Focus();
                    txtNearsightGlassLeft.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNearsightGlassLeft_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNearsightGlassReadingDist.Focus();
                    txtNearsightGlassReadingDist.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNearsightGlassReadingDist_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNearsightGlassPupilDist.Focus();
                    txtNearsightGlassPupilDist.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNearsightGlassPupilDist_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spReoptometristAppointment.Focus();
                    spReoptometristAppointment.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spReoptometristAppointment_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkFirst_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkFirst.Checked)
                {
                    chkAgain.Checked = false;
                }
                else if (!chkAgain.Checked)
                {
                    chkFirst.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAgain_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkAgain.Checked)
                {
                    chkFirst.Checked = false;
                }
                else if (!chkFirst.Checked)
                {
                    chkAgain.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkEnd_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkEnd.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkEnd.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkEnd.Name;
                    csAddOrUpdate.VALUE = (chkEnd.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPrint_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPrint.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrint.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPrint.Name;
                    csAddOrUpdate.VALUE = (chkPrint.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;
                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;
                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.Focus();
                        edit.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSave.Enabled) return;

                if (!dxValidationProvider1.Validate()) return;

                CommonParam param = new CommonParam();
                bool success = false;

                HisServiceReqOptometristSDO sdo = new HisServiceReqOptometristSDO();
                ProcessDataSdo(ref sdo);

                var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_SERVICE_REQ>("api/HisServiceReq/Optometrist", ApiConsumer.ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (result != null)
                {
                    serviceReq = result;
                    success = true;

                    if (chkEnd.Checked)
                    {
                        btnEndReq_Click(null, null);
                    }

                    if (chkPrint.Checked)
                    {
                        btnPrint_Click(null, null);
                    }

                    btnSave.Enabled = false;
                }

                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataSdo(ref HisServiceReqOptometristSDO sdo)
        {
            try
            {
                if (sdo == null) sdo = new HisServiceReqOptometristSDO();

                sdo.AfterLightReflectionLeft = txtAfterLightReflectionL.Text;
                sdo.AfterLightReflectionRight = txtAfterLightReflectionR.Text;
                sdo.AjustableGlassForesight = txtAjustableGlass.Text;
                sdo.AjustableGlassForesightL = txtAjustableGlassL.Text;
                sdo.AjustableGlassForesightR = txtAjustableGlassR.Text;
                sdo.BeforeLightReflectionLeft = txtBeforeLightReflectionL.Text;
                sdo.BeforeLightReflectionRight = txtBeforeLightReflectionR.Text;
                sdo.ExecuteLoginname = cboExecuteName.EditValue.ToString();
                sdo.ExecuteUsername = cboExecuteName.Text;
                sdo.ForesightLeftEye = txtForesightLeft.Text;
                sdo.ForesightLeftGlassHole = txtForesightLeftGlass.Text;
                sdo.ForesightLeftUsingGlass = txtForesightLeftUsingGlass.Text;
                sdo.ForesightRightEye = txtForesightRight.Text;
                sdo.ForesightRightGlassHole = txtForesightRightGlass.Text;
                sdo.ForesightRightUsingGlass = txtForesightRightUsingGlass.Text;
                sdo.ForesightUsingGlassDegreeL = txtGlassDegreeL.Text;
                sdo.ForesightUsingGlassDegreeR = txtGlassDegreeR.Text;
                //sdo.IsFinish = chkEnd.Checked;
                sdo.IsFirstOptometrist = chkFirst.Checked;
                sdo.NearsightGlassLeftEye = txtNearsightGlassLeft.Text;
                sdo.NearsightGlassPupilDist = txtNearsightGlassPupilDist.Text;
                sdo.NearsightGlassReadingDist = txtNearsightGlassReadingDist.Text;
                sdo.NearsightGlassRightEye = txtNearsightGlassRight.Text;
                sdo.OptometristTime = txtOptometristTime.Text;
                sdo.RefactometryLeftEye = txtRefractometryLeft.Text;
                sdo.RefactometryRightEye = txtRefractometryRight.Text;
                if (spReoptometristAppointment.EditValue != null)
                {
                    sdo.ReoptometristAppointment = Inventec.Common.TypeConvert.Parse.ToInt64(spReoptometristAppointment.EditValue.ToString());
                }

                sdo.ServiceReqId = serviceReq.ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnEndReq_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnEndReq.Enabled) return;
                if (serviceReq == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("currentServiceReq is null");
                    return;
                }

                bool success = false;
                CommonParam param = new CommonParam();
                var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_SERVICE_REQ>("api/HisServiceReq/Finish", ApiConsumer.ApiConsumers.MosConsumer, serviceReq.ID, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (result != null)
                {
                    success = true;
                    btnEndReq.Enabled = false;
                    btnSave.Enabled = false;
                }

                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                richEditorMain.RunPrintTemplate("Mps000386", DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000386":
                        LoadDonKinh(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void LoadDonKinh(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.serviceReq != null)
                {
                    WaitingManager.Show();
                    MPS.Processor.Mps000386.PDO.Mps000386PDO pdo = new MPS.Processor.Mps000386.PDO.Mps000386PDO(serviceReq);
                    WaitingManager.Hide();

                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                    }
                    else
                    {
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(serviceReq.TDL_TREATMENT_CODE, printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void OptometristSave()
        {
            btnSave_Click(null, null);
        }

        public void OptometristPrint()
        {
            btnPrint_Click(null, null);
        }

        public void End()
        {
            btnEndReq_Click(null, null);
        }

        private void gridViewSereServ_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HIS_SERE_SERV dataRow = (HIS_SERE_SERV)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow == null) return;

                    if (e.Column.FieldName == "SERVICE_UNIT_NAME")
                    {
                        var unit = BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(o => o.ID == dataRow.TDL_SERVICE_UNIT_ID);
                        if (unit != null)
                        {
                            e.Value = unit.SERVICE_UNIT_NAME;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSereServ_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    HIS_SERE_SERV sereServ = (HIS_SERE_SERV)gridViewSereServ.GetRow(e.RowHandle);
                    if (sereServ != null)
                    {
                        if (sereServ.IS_NO_EXECUTE == 1)
                        {
                            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
