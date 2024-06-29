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
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Common.Logging;
using Inventec.UC.Paging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;
using HIS.Desktop.Plugins.EnterKskInfomantion.Resources;
using HIS.Desktop.Utilities.Extensions;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Plugins.EnterKskInfomantion.Config;
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraBars;
using ACS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.EnterKskInfomantion.ADO;
using Inventec.Common.Integrate.EditorLoader;
using System.IO;

namespace HIS.Desktop.Plugins.EnterKskInfomantion
{

    public partial class frmEnterKskInfomantion : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int positionHandle = -1;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.EnterKskInfomantion";
        List<HIS_SERVICE_REQ_STT> serviceReqSttSelecteds;
        List<HIS_SERVICE_REQ_STT> serviceReqSttList;
        ADO.ServiceReqADO currentData;
        ServiceReqStatus currentServiceReqSTT = ServiceReqStatus.Default;
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;

        GridColumn lastColumn = null;
        ToolTipControlInfo lastInfo = null;
        int lastRowHandle = -1;
        long currentTreatmentId = 0;
        int currentTabPageChoose = 0;
        bool statecheckColumn = false;
        List<ADO.ServiceReqADO> listData;
        List<ADO.ServiceReqADO> checkDataPrint = new List<ADO.ServiceReqADO>();
        List<ADO.ServiceReqADO> listChooseFromGrid = new List<ADO.ServiceReqADO>();

        List<HIS_DEPARTMENT> listDepartment;
        List<HIS_DEPARTMENT> _DepartmentSearchSelecteds;
        List<V_HIS_EXECUTE_ROOM> listExecuteRoom;
        List<V_HIS_EXECUTE_ROOM> _ExecuteRoomSearchSelecteds;
        List<V_HIS_EXECUTE_ROOM> listDepartmentRoom;
        #endregion

        public enum ServiceReqStatus
        {
            ChuaXuLy,
            DaXuLy,
            HoanThanh,
            Default,
        }

        HIS_KSK_GENERAL HisKskGeneral = new HIS_KSK_GENERAL();
        #region Construct
        public frmEnterKskInfomantion(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                gridControlServiceReq.ToolTipController = toolTipControllerGrid;

                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Private method
        private void frmEnterKskInfomantion_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                ResetPatientInfoDisplayed();
                ResetFormData();

                dtFrom.DateTime = DateTime.Now;
                dtTo.DateTime = DateTime.Now;

                GridCheckMarksSelection gridCheckMark = cboServiceReqStt.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    this.serviceReqSttList = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE_REQ_STT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).OrderBy(o => o.SERVICE_REQ_STT_NAME).ToList();
                    gridCheckMark.SelectAll(this.serviceReqSttList);
                }

                this.currentServiceReqSTT = ServiceReqStatus.Default;
                EnableControlChanged(this.currentServiceReqSTT);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
        private void SetDefaultFocus()
        {
            try
            {
                txtServiceReqCodeForSearch.Focus();
                txtServiceReqCodeForSearch.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void InitTabIndex()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
        private void SetFocusEditor()
        {
            try
            {
                txtServiceReqCodeForSearch.Focus();
                txtServiceReqCodeForSearch.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void ChangedDataRow(ADO.ServiceReqADO data)
        {
            try
            {
                if (data != null)
                {
                    currentTreatmentId = data.TREATMENT_ID;
                    this.listChooseFromGrid = new List<ADO.ServiceReqADO>() { data };

                    this.currentData = data;
                    SetCurrentServiceReqSTT(data);

                    GetServiceReqData(data);

                    FillDataToEditorControl(data);

                    EnableControlChanged(this.currentServiceReqSTT);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCurrentServiceReqSTT(ADO.ServiceReqADO data)
        {
            try
            {
                long statusId = data.SERVICE_REQ_STT_ID;
                if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                {
                    this.currentServiceReqSTT = ServiceReqStatus.ChuaXuLy;
                }
                else if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                {
                    this.currentServiceReqSTT = ServiceReqStatus.DaXuLy;
                }
                else if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                {
                    this.currentServiceReqSTT = ServiceReqStatus.HoanThanh;
                }
                else
                {
                    this.currentServiceReqSTT = ServiceReqStatus.Default;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private HIS_DHST GetDHSTByID(long id)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisDhstFilter filter = new MOS.Filter.HisDhstFilter();
                filter.ID = id;
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var result = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, filter, param).SingleOrDefault();
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        private void GetServiceReqData(ADO.ServiceReqADO data)
        {
            try
            {
                if (data != null)
                {

                    CommonParam paramKskGeneral = new CommonParam();
                    MOS.Filter.HisKskGeneralFilter filterKskGeneral = new MOS.Filter.HisKskGeneralFilter();
                    filterKskGeneral.SERVICE_REQ_ID = data.ID;
                    filterKskGeneral.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultKskGeneral = new BackendAdapter(paramKskGeneral).Get<List<MOS.EFMODEL.DataModels.HIS_KSK_GENERAL>>("api/HisKskGeneral/Get", ApiConsumers.MosConsumer, filterKskGeneral, paramKskGeneral).SingleOrDefault();
                    if (resultKskGeneral != null)
                    {
                        if (resultKskGeneral.DHST_ID != null && resultKskGeneral.DHST_ID > 0)
                        {
                            HIS_DHST dhstGeneral = GetDHSTByID((long)resultKskGeneral.DHST_ID);
                            resultKskGeneral.HIS_DHST = dhstGeneral;
                        }
                        data.KSK_GENERAL = resultKskGeneral;
                    }
                    else
                    {
                        data.KSK_GENERAL = new HIS_KSK_GENERAL();
                    }

                    CommonParam paramKskOccupational = new CommonParam();
                    MOS.Filter.HisKskOccupationalFilter filterKskOccupational = new MOS.Filter.HisKskOccupationalFilter();
                    filterKskOccupational.SERVICE_REQ_ID = data.ID;
                    filterKskOccupational.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultKskOccupational = new BackendAdapter(paramKskOccupational).Get<List<MOS.EFMODEL.DataModels.HIS_KSK_OCCUPATIONAL>>("api/HisKskOccupational/Get", ApiConsumers.MosConsumer, filterKskOccupational, paramKskOccupational).SingleOrDefault();
                    if (resultKskOccupational != null)
                    {
                        if (resultKskOccupational.DHST_ID != null && resultKskOccupational.DHST_ID > 0)
                        {
                            HIS_DHST dhstOccupational = GetDHSTByID((long)resultKskOccupational.DHST_ID);
                            resultKskOccupational.HIS_DHST = dhstOccupational;
                        }
                        data.KSK_OCCUPATIONAL = resultKskOccupational;
                    }
                    else
                    {
                        data.KSK_OCCUPATIONAL = new HIS_KSK_OCCUPATIONAL();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(ADO.ServiceReqADO data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToPatientInfoDisplayed(data);
                    FillDataToTabKhamChung(data);
                    FillDataToTabKetLuan(data);
                    FillDataToTabBenhNgheNghiep(data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToPatientInfoDisplayed(ADO.ServiceReqADO data)
        {
            try
            {
                if (data != null)
                {
                    lblServiceReqCode.Text = data.SERVICE_REQ_CODE;
                    lblTreatmentCode.Text = data.TDL_TREATMENT_CODE;
                    lblPatientCode.Text = data.TDL_PATIENT_CODE;
                    lblPatientName.Text = data.TDL_PATIENT_NAME;
                    lblGender.Text = data.TDL_PATIENT_GENDER_NAME;
                    if (data.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        lblBirthDate.Text = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    else
                        lblBirthDate.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                    lblIntructionTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.INTRUCTION_TIME);
                    lblGeneralCode.Text = data.KSK_GENERAL.KSK_GENERAL_CODE;
                    if (!String.IsNullOrEmpty(data.TDL_PATIENT_AVATAR_URL))
                    {
                        MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(data.TDL_PATIENT_AVATAR_URL);
                        pictureEditAvatar.Image = Image.FromStream(stream);
                        pictureEditAvatar.Image.Tag = data.TDL_PATIENT_AVATAR_URL;
                    }
                    else
                    {
                        string pathLocal = GetPathDefault();
                        pictureEditAvatar.Image = Image.FromFile(pathLocal);
                    }
                    if (data.TDL_KSK_CONTRACT_ID != null)
                    {
                        var result = BackendDataWorker.Get<HIS_KSK_CONTRACT>().Where(o => o.ID == data.TDL_KSK_CONTRACT_ID).FirstOrDefault();
                        if (result != null)
                        {
                            lblKSKContract.Text = result.KSK_CONTRACT_CODE;
                        }
                        else
                            lblKSKContract.Text = "";
                    }
                    else
                    {
                        lblKSKContract.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetPathDefault()
        {
            string imageDefaultPath = string.Empty;
            try
            {
                string localPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                imageDefaultPath = localPath + "\\Img\\ImageStorage\\notImage.jpg";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return imageDefaultPath;
        }

        private void FillDataToTabKhamChung(ADO.ServiceReqADO data)
        {
            try
            {
                if (data != null && data.KSK_GENERAL != null)
                {
                    if (data.KSK_GENERAL.HIS_DHST != null)
                    {
                        if (data.KSK_GENERAL.HIS_DHST.HEIGHT != null)
                        {
                            txtHeightTab1.Value = (Decimal)data.KSK_GENERAL.HIS_DHST.HEIGHT;
                        }
                        else
                        {
                            txtHeightTab1.ResetText();
                            txtHeightTab1.EditValue = null;
                        }

                        if (data.KSK_GENERAL.HIS_DHST.WEIGHT != null)
                        {
                            txtWeightTab1.Value = (Decimal)data.KSK_GENERAL.HIS_DHST.WEIGHT;
                        }
                        else
                        {
                            txtWeightTab1.ResetText();
                            txtWeightTab1.EditValue = null;
                        }
                        lblBMITab1.Text = (data.KSK_GENERAL.HIS_DHST.VIR_BMI != null) ? data.KSK_GENERAL.HIS_DHST.VIR_BMI.ToString() : "";
                        if (data.KSK_GENERAL.HIS_DHST.VIR_BMI == null || data.KSK_GENERAL.HIS_DHST.VIR_BMI == 0)
                        {
                            lblBmiDisplayTextTab1.Text = "";
                        }
                        if (data.KSK_GENERAL.HIS_DHST.PULSE != null)
                        {
                            txtPulseTab1.Value = (Decimal)data.KSK_GENERAL.HIS_DHST.PULSE;
                        }
                        else
                        {
                            txtPulseTab1.ResetText();
                            txtPulseTab1.EditValue = null;
                        }

                        if (data.KSK_GENERAL.HIS_DHST.BLOOD_PRESSURE_MAX != null)
                        {
                            txtBloodPressureMaxTab1.Value = (Decimal)data.KSK_GENERAL.HIS_DHST.BLOOD_PRESSURE_MAX;
                        }
                        else
                        {
                            txtBloodPressureMaxTab1.ResetText();
                            txtBloodPressureMaxTab1.EditValue = null;
                        }

                        if (data.KSK_GENERAL.HIS_DHST.BLOOD_PRESSURE_MIN != null)
                        {
                            txtBloodPressureMinTab1.Value = (Decimal)data.KSK_GENERAL.HIS_DHST.BLOOD_PRESSURE_MIN;
                        }
                        else
                        {
                            txtBloodPressureMinTab1.ResetText();
                            txtBloodPressureMinTab1.EditValue = null;
                        }

                        if (data.KSK_GENERAL == null || data.KSK_GENERAL.CONCLUSION_TIME == null)
                        {
                            cboDayResult.DateTime = DateTime.Now;
                        }
                        else
                        {
                            cboDayResult.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.KSK_GENERAL.CONCLUSION_TIME ?? 0) ?? DateTime.Now;
                        }

                        if (data != null && data.KSK_GENERAL != null)
                        {
                            cboBSketluan.EditValue = data.KSK_GENERAL.CONCLUDER_LOGINNAME;
                        }
                        else
                        {
                            cboBSketluan.EditValue = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        }

                        if (data.KSK_GENERAL.HIS_DHST.TEMPERATURE != null)
                        {
                            txtTemperatureTab1.Value = (Decimal)data.KSK_GENERAL.HIS_DHST.TEMPERATURE;
                        }
                        else
                        {
                            txtTemperatureTab1.ResetText();
                            txtTemperatureTab1.EditValue = null;
                        }

                        if (data.KSK_GENERAL.HIS_DHST.BREATH_RATE != null)
                        {
                            txtBreathRateTab1.Value = (Decimal)data.KSK_GENERAL.HIS_DHST.BREATH_RATE;
                        }
                        else
                        {
                            txtBreathRateTab1.ResetText();
                            txtBreathRateTab1.EditValue = null;
                        }
                    }
                    else
                    {
                        txtHeightTab1.ResetText();
                        txtHeightTab1.EditValue = null;
                        txtWeightTab1.ResetText();
                        txtWeightTab1.EditValue = null;
                        lblBMITab1.Text = "";
                        txtPulseTab1.ResetText();
                        txtPulseTab1.EditValue = null;
                        txtBloodPressureMaxTab1.ResetText();
                        txtBloodPressureMaxTab1.EditValue = null;
                        txtBloodPressureMinTab1.ResetText();
                        txtBloodPressureMinTab1.EditValue = null;
                        txtTemperatureTab1.ResetText();
                        txtTemperatureTab1.EditValue = null;
                        txtBreathRateTab1.ResetText();
                        txtBreathRateTab1.EditValue = null;
                    }
                    txtExamCirculationTab1.Text = !string.IsNullOrEmpty(data.KSK_GENERAL.EXAM_CIRCULATION) ? data.KSK_GENERAL.EXAM_CIRCULATION : ResourceMessage.BinhThuong;
                    cboExamCirculationRankTab1.EditValue = (data.KSK_GENERAL.EXAM_CIRCULATION_RANK != null) ? data.KSK_GENERAL.EXAM_CIRCULATION_RANK : null;
                    txtExamRepiratoryTab1.Text = !string.IsNullOrEmpty(data.KSK_GENERAL.EXAM_RESPIRATORY) ? data.KSK_GENERAL.EXAM_RESPIRATORY : ResourceMessage.BinhThuong;
                    cboExamRepiratoryRankTab1.EditValue = (data.KSK_GENERAL.EXAM_RESPIRATORY_RANK != null) ? data.KSK_GENERAL.EXAM_RESPIRATORY_RANK : null;
                    txtExamDigestionTab1.Text = !string.IsNullOrEmpty(data.KSK_GENERAL.EXAM_DIGESTION) ? data.KSK_GENERAL.EXAM_DIGESTION : ResourceMessage.BinhThuong;
                    cboExamDigestionRankTab1.EditValue = (data.KSK_GENERAL.EXAM_DIGESTION_RANK != null) ? data.KSK_GENERAL.EXAM_DIGESTION_RANK : null;
                    txtExamOENDTab1.Text = !string.IsNullOrEmpty(data.KSK_GENERAL.EXAM_OEND) ? data.KSK_GENERAL.EXAM_OEND : ResourceMessage.BinhThuong;
                    cboExamOENDRankTab1.EditValue = (data.KSK_GENERAL.EXAM_OEND_RANK != null) ? data.KSK_GENERAL.EXAM_OEND_RANK : null;
                    txtExamMuscleBoneTab1.Text = !string.IsNullOrEmpty(data.KSK_GENERAL.EXAM_MUSCLE_BONE) ? data.KSK_GENERAL.EXAM_MUSCLE_BONE : ResourceMessage.BinhThuong;
                    cboExamMuscleBoneRankTab1.EditValue = (data.KSK_GENERAL.EXAM_MUSCLE_BONE_RANK != null) ? data.KSK_GENERAL.EXAM_MUSCLE_BONE_RANK : null;
                    txtExamNeurologicalTab1.Text = !string.IsNullOrEmpty(data.KSK_GENERAL.EXAM_NEUROLOGICAL) ? data.KSK_GENERAL.EXAM_NEUROLOGICAL : ResourceMessage.BinhThuong;
                    cboExamNeurologicalRankTab1.EditValue = (data.KSK_GENERAL.EXAM_NEUROLOGICAL_RANK != null) ? data.KSK_GENERAL.EXAM_NEUROLOGICAL_RANK : null;
                    txtMentalTab1.Text = !string.IsNullOrEmpty(data.KSK_GENERAL.EXAM_MENTAL) ? data.KSK_GENERAL.EXAM_MENTAL : ResourceMessage.BinhThuong;
                    cboMentalRankTab1.EditValue = (data.KSK_GENERAL.EXAM_MENTAL_RANK != null) ? data.KSK_GENERAL.EXAM_MENTAL_RANK : null;
                    txtExamDermatologyTab1.Text = !string.IsNullOrEmpty(data.KSK_GENERAL.EXAM_DERMATOLOGY) ? data.KSK_GENERAL.EXAM_DERMATOLOGY : ResourceMessage.BinhThuong;
                    cboExamDermatologyRankTab1.EditValue = (data.KSK_GENERAL.EXAM_DERMATOLOGY_RANK != null) ? data.KSK_GENERAL.EXAM_DERMATOLOGY_RANK : null;
                    txtExamKidneyUrologyTab1.Text = !string.IsNullOrEmpty(data.KSK_GENERAL.EXAM_KIDNEY_UROLOGY) ? data.KSK_GENERAL.EXAM_KIDNEY_UROLOGY : ResourceMessage.BinhThuong;
                    cboExamKidneyUrologyRankTab1.EditValue = (data.KSK_GENERAL.EXAM_KIDNEY_UROLOGY_RANK != null) ? data.KSK_GENERAL.EXAM_KIDNEY_UROLOGY_RANK : null;
                    txtSurgeryTab1.Text = !string.IsNullOrEmpty(data.KSK_GENERAL.EXAM_SURGERY) ? data.KSK_GENERAL.EXAM_SURGERY : ResourceMessage.BinhThuong;
                    cboSurgeryRankTab1.EditValue = (data.KSK_GENERAL.EXAM_SURGERY_RANK != null) ? data.KSK_GENERAL.EXAM_SURGERY_RANK : null;
                    txtEyeTab1.Text = !string.IsNullOrEmpty(data.KSK_GENERAL.EXAM_EYE) ? data.KSK_GENERAL.EXAM_EYE : ResourceMessage.BinhThuong;
                    cboEyeRankTab1.EditValue = (data.KSK_GENERAL.EXAM_EYE_RANK != null) ? data.KSK_GENERAL.EXAM_EYE_RANK : null;
                    txtExamENTTab1.Text = !string.IsNullOrEmpty(data.KSK_GENERAL.EXAM_ENT) ? data.KSK_GENERAL.EXAM_ENT : ResourceMessage.BinhThuong;
                    cboExamENTRankTab1.EditValue = (data.KSK_GENERAL.EXAM_ENT_RANK != null) ? data.KSK_GENERAL.EXAM_ENT_RANK : null;
                    txtExamStomatologyTab1.Text = !string.IsNullOrEmpty(data.KSK_GENERAL.EXAM_STOMATOLOGY) ? data.KSK_GENERAL.EXAM_STOMATOLOGY : ResourceMessage.BinhThuong;
                    cboExamStomatologyRankTab1.EditValue = (data.KSK_GENERAL.EXAM_STOMATOLOGY_RANK != null) ? data.KSK_GENERAL.EXAM_STOMATOLOGY_RANK : null;

                    cboDayResult.DateTime = DateTime.Now;
                    if (data.KSK_GENERAL != null && data.KSK_GENERAL.CONCLUSION_TIME != null)
                    {
                        cboDayResult.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.KSK_GENERAL.CONCLUSION_TIME ?? 0) ?? DateTime.Now;
                    }

                    cboBSketluan.EditValue = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    if (data != null && !string.IsNullOrEmpty(data.KSK_GENERAL.CONCLUDER_LOGINNAME))
                    {
                        cboBSketluan.EditValue = data.KSK_GENERAL.CONCLUDER_LOGINNAME;
                    }
                    else if (data != null && !string.IsNullOrEmpty(data.EXECUTE_LOGINNAME))
                    {
                        cboBSketluan.EditValue = data.EXECUTE_LOGINNAME;
                    }

                    memCDHA.Text = data.KSK_GENERAL.NOTE_DIIM;
                    memNoteBlood.Text = data.KSK_GENERAL.NOTE_BLOOD;
                    memNoteTestUre.Text = data.KSK_GENERAL.NOTE_TEST_URINE;
                    memNoteTestOth.Text = data.KSK_GENERAL.NOTE_TEST_OTHER;

                    if (data.KSK_GENERAL.HEALTH_EXAM_RANK_ID != null)
                    {
                        cboPLSucKhoeTab1.EditValue = data.KSK_GENERAL.HEALTH_EXAM_RANK_ID;
                    }
                    else
                    {
                        var healExamRanks = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_HEALTH_EXAM_RANK>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                        cboPLSucKhoeTab1.EditValue = healExamRanks.First().ID;

                    }
                    txtDiseasesTab1.Text = data.KSK_GENERAL.DISEASES;
                    txtTreatmentInstructionTab1.Text = data.KSK_GENERAL.TREATMENT_INSTRUCTION;


                    txtExamObstetic.Text = !string.IsNullOrEmpty(data.KSK_GENERAL.EXAM_OBSTETRIC) ? data.KSK_GENERAL.EXAM_OBSTETRIC : ResourceMessage.BinhThuong;
                    txtExamOccupationalTherapy.Text = !string.IsNullOrEmpty(data.KSK_GENERAL.EXAM_OCCUPATIONAL_THERAPY) ? data.KSK_GENERAL.EXAM_OCCUPATIONAL_THERAPY : ResourceMessage.BinhThuong;
                    txtExamTraditional.Text = !string.IsNullOrEmpty(data.KSK_GENERAL.EXAM_TRADITIONAL) ? data.KSK_GENERAL.EXAM_TRADITIONAL : ResourceMessage.BinhThuong;
                    txtExamNutrion.Text = !string.IsNullOrEmpty(data.KSK_GENERAL.EXAM_NUTRION) ? data.KSK_GENERAL.EXAM_NUTRION : ResourceMessage.BinhThuong;

                    cboExamObsteticRank.EditValue = data.KSK_GENERAL.EXAM_OBSTETRIC_RANK;
                    cboExamOccupationalTherapyRank.EditValue = data.KSK_GENERAL.EXAM_OCCUPATIONAL_THERAPY_RANK;
                    cboExamTraditionalRank.EditValue = data.KSK_GENERAL.EXAM_TRADITIONAL_RANK;
                    cboExamNutrionRank.EditValue = data.KSK_GENERAL.EXAM_NUTRION_RANK;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToTabKetLuan(ADO.ServiceReqADO data)
        {
            try
            {
                if (data != null)
                {
                    txtConclusionClinicalTab2.Text = data.CONCLUSION_CLINICAL;
                    txtConclusionSubclinicalTab2.Text = data.CONCLUSION_SUBCLINICAL;
                    txtOccupationalDiseaseTab2.Text = data.OCCUPATIONAL_DISEASE;
                    txtConclusionConsultationTab2.Text = data.CONCLUSION_CONSULTATION;
                    txtExamConclusionTab2.Text = data.EXAM_CONCLUSION;
                    txtConclusionTab2.Text = data.CONCLUSION;
                    txtProvisionalDiagnosisTab2.Text = data.PROVISIONAL_DIAGNOSIS;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToTabBenhNgheNghiep(ADO.ServiceReqADO data)
        {
            try
            {
                if (data != null && data.KSK_OCCUPATIONAL != null)
                {
                    if (data.KSK_OCCUPATIONAL.HIS_DHST != null)
                    {
                        if (data.KSK_OCCUPATIONAL.HIS_DHST.HEIGHT != null)
                        {
                            txtHeightTab3.Value = (Decimal)data.KSK_OCCUPATIONAL.HIS_DHST.HEIGHT;
                        }
                        else
                        {
                            txtHeightTab3.ResetText();
                            txtHeightTab3.EditValue = null;
                        }

                        if (data.KSK_OCCUPATIONAL.HIS_DHST.WEIGHT != null)
                        {
                            txtWeightTab3.Value = (Decimal)data.KSK_OCCUPATIONAL.HIS_DHST.WEIGHT;
                        }
                        else
                        {
                            txtWeightTab3.ResetText();
                            txtWeightTab3.EditValue = null;
                        }
                        lblBMITab3.Text = (data.KSK_OCCUPATIONAL.HIS_DHST.VIR_BMI != null) ? data.KSK_OCCUPATIONAL.HIS_DHST.VIR_BMI.ToString() : "";
                        if (data.KSK_OCCUPATIONAL.HIS_DHST.VIR_BMI == null || data.KSK_OCCUPATIONAL.HIS_DHST.VIR_BMI == 0)
                        {
                            lblBmiDisplayTextTab3.Text = "";
                        }
                        if (data.KSK_OCCUPATIONAL.HIS_DHST.PULSE != null)
                        {
                            txtPulseTab3.Value = (Decimal)data.KSK_OCCUPATIONAL.HIS_DHST.PULSE;
                        }
                        else
                        {
                            txtPulseTab3.ResetText();
                            txtPulseTab3.EditValue = null;
                        }

                        if (data.KSK_OCCUPATIONAL.HIS_DHST.BLOOD_PRESSURE_MAX != null)
                        {
                            txtBloodPressureMaxTab3.Value = (Decimal)data.KSK_OCCUPATIONAL.HIS_DHST.BLOOD_PRESSURE_MAX;
                        }
                        else
                        {
                            txtBloodPressureMaxTab3.ResetText();
                            txtBloodPressureMaxTab3.EditValue = null;
                        }

                        if (data.KSK_OCCUPATIONAL.HIS_DHST.BLOOD_PRESSURE_MIN != null)
                        {
                            txtBloodPressureMinTab3.Value = (Decimal)data.KSK_OCCUPATIONAL.HIS_DHST.BLOOD_PRESSURE_MIN;
                        }
                        else
                        {
                            txtBloodPressureMinTab3.ResetText();
                            txtBloodPressureMinTab3.EditValue = null;
                        }

                        if (data.KSK_OCCUPATIONAL.HIS_DHST.TEMPERATURE != null)
                        {
                            txtTemperatureTab3.Value = (Decimal)data.KSK_OCCUPATIONAL.HIS_DHST.TEMPERATURE;
                        }
                        else
                        {
                            txtTemperatureTab3.ResetText();
                            txtTemperatureTab3.EditValue = null;
                        }

                        if (data.KSK_OCCUPATIONAL.HIS_DHST.BREATH_RATE != null)
                        {
                            txtBreathRateTab3.Value = (Decimal)data.KSK_OCCUPATIONAL.HIS_DHST.BREATH_RATE;
                        }
                        else
                        {
                            txtBreathRateTab3.ResetText();
                            txtBreathRateTab3.EditValue = null;
                        }
                    }
                    else
                    {
                        txtHeightTab3.ResetText();
                        txtHeightTab3.EditValue = null;
                        txtWeightTab3.ResetText();
                        txtWeightTab3.EditValue = null;
                        lblBMITab3.Text = "";
                        txtPulseTab3.ResetText();
                        txtPulseTab3.EditValue = null;
                        txtBloodPressureMaxTab3.ResetText();
                        txtBloodPressureMaxTab3.EditValue = null;
                        txtBloodPressureMinTab3.ResetText();
                        txtBloodPressureMinTab3.EditValue = null;
                        txtTemperatureTab3.ResetText();
                        txtTemperatureTab3.EditValue = null;
                        txtBreathRateTab3.ResetText();
                        txtBreathRateTab3.EditValue = null;
                    }
                    txtExamCirculationTab3.Text = !string.IsNullOrEmpty(data.KSK_OCCUPATIONAL.EXAM_CIRCULATION) ? data.KSK_OCCUPATIONAL.EXAM_CIRCULATION : ResourceMessage.BinhThuong;
                    cboExamCirculationRankTab3.EditValue = (data.KSK_OCCUPATIONAL.EXAM_CIRCULATION_RANK != null) ? data.KSK_OCCUPATIONAL.EXAM_CIRCULATION_RANK : null;
                    txtExamRepiratoryTab3.Text = !string.IsNullOrEmpty(data.KSK_OCCUPATIONAL.EXAM_RESPIRATORY) ? data.KSK_OCCUPATIONAL.EXAM_RESPIRATORY : ResourceMessage.BinhThuong;
                    cboExamRepiratoryRankTab3.EditValue = (data.KSK_OCCUPATIONAL.EXAM_RESPIRATORY_RANK != null) ? data.KSK_OCCUPATIONAL.EXAM_RESPIRATORY_RANK : null;
                    txtExamDigestionTab3.Text = !string.IsNullOrEmpty(data.KSK_OCCUPATIONAL.EXAM_DIGESTION) ? data.KSK_OCCUPATIONAL.EXAM_DIGESTION : ResourceMessage.BinhThuong;
                    cboExamDigestionRankTab3.EditValue = (data.KSK_OCCUPATIONAL.EXAM_DIGESTION_RANK != null) ? data.KSK_OCCUPATIONAL.EXAM_DIGESTION_RANK : null;
                    txtExamOENDTab3.Text = !string.IsNullOrEmpty(data.KSK_OCCUPATIONAL.EXAM_OEND) ? data.KSK_OCCUPATIONAL.EXAM_OEND : ResourceMessage.BinhThuong;
                    cboExamOENDRankTab3.EditValue = (data.KSK_OCCUPATIONAL.EXAM_OEND_RANK != null) ? data.KSK_OCCUPATIONAL.EXAM_OEND_RANK : null;
                    txtExamMuscleBoneTab3.Text = !string.IsNullOrEmpty(data.KSK_OCCUPATIONAL.EXAM_MUSCLE_BONE) ? data.KSK_OCCUPATIONAL.EXAM_MUSCLE_BONE : ResourceMessage.BinhThuong;
                    cboExamMuscleBoneRankTab3.EditValue = (data.KSK_OCCUPATIONAL.EXAM_MUSCLE_BONE_RANK != null) ? data.KSK_OCCUPATIONAL.EXAM_MUSCLE_BONE_RANK : null;
                    txtExamNeurologicalTab3.Text = !string.IsNullOrEmpty(data.KSK_OCCUPATIONAL.EXAM_NEUROLOGICAL) ? data.KSK_OCCUPATIONAL.EXAM_NEUROLOGICAL : ResourceMessage.BinhThuong;
                    cboExamNeurologicalRankTab3.EditValue = (data.KSK_OCCUPATIONAL.EXAM_NEUROLOGICAL_RANK != null) ? data.KSK_OCCUPATIONAL.EXAM_NEUROLOGICAL_RANK : null;
                    txtMentalTab3.Text = !string.IsNullOrEmpty(data.KSK_OCCUPATIONAL.EXAM_MENTAL) ? data.KSK_OCCUPATIONAL.EXAM_MENTAL : ResourceMessage.BinhThuong;
                    cboMentalRankTab3.EditValue = (data.KSK_OCCUPATIONAL.EXAM_MENTAL_RANK != null) ? data.KSK_OCCUPATIONAL.EXAM_MENTAL_RANK : null;
                    txtExamDermatologyTab3.Text = !string.IsNullOrEmpty(data.KSK_OCCUPATIONAL.EXAM_DERMATOLOGY) ? data.KSK_OCCUPATIONAL.EXAM_DERMATOLOGY : ResourceMessage.BinhThuong;
                    cboExamDermatologyRankTab3.EditValue = (data.KSK_OCCUPATIONAL.EXAM_DERMATOLOGY_RANK != null) ? data.KSK_OCCUPATIONAL.EXAM_DERMATOLOGY_RANK : null;
                    txtExamKidneyUrologyTab3.Text = !string.IsNullOrEmpty(data.KSK_OCCUPATIONAL.EXAM_KIDNEY_UROLOGY) ? data.KSK_OCCUPATIONAL.EXAM_KIDNEY_UROLOGY : ResourceMessage.BinhThuong;
                    cboExamKidneyUrologyRankTab3.EditValue = (data.KSK_OCCUPATIONAL.EXAM_KIDNEY_UROLOGY_RANK != null) ? data.KSK_OCCUPATIONAL.EXAM_KIDNEY_UROLOGY_RANK : null;
                    txtSurgeryTab3.Text = !string.IsNullOrEmpty(data.KSK_OCCUPATIONAL.EXAM_SURGERY) ? data.KSK_OCCUPATIONAL.EXAM_SURGERY : ResourceMessage.BinhThuong;
                    cboSurgeryRankTab3.EditValue = (data.KSK_OCCUPATIONAL.EXAM_SURGERY_RANK != null) ? data.KSK_OCCUPATIONAL.EXAM_SURGERY_RANK : null;
                    txtEyeTab3.Text = !string.IsNullOrEmpty(data.KSK_OCCUPATIONAL.EXAM_EYE) ? data.KSK_OCCUPATIONAL.EXAM_EYE : ResourceMessage.BinhThuong;
                    cboEyeRankTab3.EditValue = (data.KSK_OCCUPATIONAL.EXAM_EYE_RANK != null) ? data.KSK_OCCUPATIONAL.EXAM_EYE_RANK : null;
                    txtExamENTTab3.Text = !string.IsNullOrEmpty(data.KSK_OCCUPATIONAL.EXAM_ENT) ? data.KSK_OCCUPATIONAL.EXAM_ENT : ResourceMessage.BinhThuong;
                    cboExamENTRankTab3.EditValue = (data.KSK_OCCUPATIONAL.EXAM_ENT_RANK != null) ? data.KSK_OCCUPATIONAL.EXAM_ENT_RANK : null;
                    txtExamStomatologyTab3.Text = !string.IsNullOrEmpty(data.KSK_OCCUPATIONAL.EXAM_STOMATOLOGY) ? data.KSK_OCCUPATIONAL.EXAM_STOMATOLOGY : ResourceMessage.BinhThuong;
                    cboExamStomatologyRankTab3.EditValue = (data.KSK_OCCUPATIONAL.EXAM_STOMATOLOGY_RANK != null) ? data.KSK_OCCUPATIONAL.EXAM_STOMATOLOGY_RANK : null;

                    txtExamNailTab3.Text = !string.IsNullOrEmpty(data.KSK_OCCUPATIONAL.EXAM_NAIL) ? data.KSK_OCCUPATIONAL.EXAM_NAIL : ResourceMessage.BinhThuong;
                    cboExamNailRankTab3.EditValue = (data.KSK_OCCUPATIONAL.EXAM_NAIL_RANK != null) ? data.KSK_OCCUPATIONAL.EXAM_NAIL_RANK : null;
                    txtExamMucosaTab3.Text = !string.IsNullOrEmpty(data.KSK_OCCUPATIONAL.EXAM_MUCOSA) ? data.KSK_OCCUPATIONAL.EXAM_MUCOSA : ResourceMessage.BinhThuong;
                    cboExamMucosaRankTab3.EditValue = (data.KSK_OCCUPATIONAL.EXAM_MUCOSA_RANK != null) ? data.KSK_OCCUPATIONAL.EXAM_MUCOSA_RANK : null;
                    txtExamHematopoieticTab3.Text = !string.IsNullOrEmpty(data.KSK_OCCUPATIONAL.EXAM_HEMATOPOIETIC) ? data.KSK_OCCUPATIONAL.EXAM_HEMATOPOIETIC : ResourceMessage.BinhThuong;
                    cboExamHematopoieticRankTab3.EditValue = (data.KSK_OCCUPATIONAL.EXAM_HEMATOPOIETIC_RANK != null) ? data.KSK_OCCUPATIONAL.EXAM_HEMATOPOIETIC_RANK : null;
                    txtExamMotionTab3.Text = !string.IsNullOrEmpty(data.KSK_OCCUPATIONAL.EXAM_MOTION) ? data.KSK_OCCUPATIONAL.EXAM_MOTION : ResourceMessage.BinhThuong;
                    cboExamMotionRankTab3.EditValue = (data.KSK_OCCUPATIONAL.EXAM_MOTION_RANK != null) ? data.KSK_OCCUPATIONAL.EXAM_MOTION_RANK : null;
                    txtExamCardiovascularTab3.Text = !string.IsNullOrEmpty(data.KSK_OCCUPATIONAL.EXAM_CARDIOVASCULAR) ? data.KSK_OCCUPATIONAL.EXAM_CARDIOVASCULAR : ResourceMessage.BinhThuong;
                    cboExamCardiovascularRankTab3.EditValue = (data.KSK_OCCUPATIONAL.EXAM_CARDIOVASCULAR_RANK != null) ? data.KSK_OCCUPATIONAL.EXAM_CARDIOVASCULAR_RANK : null;
                    txtExamLymphNodesTab3.Text = !string.IsNullOrEmpty(data.KSK_OCCUPATIONAL.EXAM_LYMPH_NODES) ? data.KSK_OCCUPATIONAL.EXAM_LYMPH_NODES : ResourceMessage.BinhThuong;
                    cboExamLymphNodesRankTab3.EditValue = (data.KSK_OCCUPATIONAL.EXAM_LYMPH_NODES_RANK != null) ? data.KSK_OCCUPATIONAL.EXAM_LYMPH_NODES_RANK : null;
                    txtExamCapillaryTab3.Text = !string.IsNullOrEmpty(data.KSK_OCCUPATIONAL.EXAM_CAPILLARY) ? data.KSK_OCCUPATIONAL.EXAM_CAPILLARY : ResourceMessage.BinhThuong;
                    cboExamCapillaryRankTab3.EditValue = (data.KSK_OCCUPATIONAL.EXAM_CAPILLARY_RANK != null) ? data.KSK_OCCUPATIONAL.EXAM_CAPILLARY_RANK : null;

                    cboPLSucKhoeTab3.EditValue = data.KSK_OCCUPATIONAL.HEALTH_EXAM_RANK_ID;
                    txtDiseasesTab3.Text = data.KSK_OCCUPATIONAL.DISEASES;
                    txtTreatmentInstructionTab3.Text = data.KSK_OCCUPATIONAL.TREATMENT_INSTRUCTION;
                    txtConclusionTab3.Text = data.KSK_OCCUPATIONAL.CONCLUSION;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToDHSTBmiTab1()
        {
            try
            {
                if (txtHeightTab1.Value == null || txtHeightTab1.Value == 0 || txtWeightTab1.Value == null || txtWeightTab1.Value == 0)
                {
                    lblBmiDisplayTextTab1.Text = "";
                    return;
                }
                decimal bmi = 0;
                if (txtHeightTab1.Value != null && txtHeightTab1.Value != 0)
                {
                    bmi = (txtWeightTab1.Value) / ((txtHeightTab1.Value / 100) * (txtHeightTab1.Value / 100));
                }
                lblBMITab1.Text = Math.Round(bmi, 2) + "";
                if (bmi < 16)
                {
                    lblBmiDisplayTextTab1.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.SKINNY.III", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                else if (16 <= bmi && bmi < 17)
                {
                    lblBmiDisplayTextTab1.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.SKINNY.II", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                else if (17 <= bmi && bmi < (decimal)18.5)
                {
                    lblBmiDisplayTextTab1.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.SKINNY.I", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                else if ((decimal)18.5 <= bmi && bmi < 25)
                {
                    lblBmiDisplayTextTab1.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.BMIDISPLAY.NORMAL", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                else if (25 <= bmi && bmi < 30)
                {
                    lblBmiDisplayTextTab1.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.BMIDISPLAY.OVERWEIGHT", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                else if (30 <= bmi && bmi < 35)
                {
                    lblBmiDisplayTextTab1.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.BMIDISPLAY.OBESITY.I", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                else if (35 <= bmi && bmi < 40)
                {
                    lblBmiDisplayTextTab1.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.BMIDISPLAY.OBESITY.II", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                else if (40 < bmi)
                {
                    lblBmiDisplayTextTab1.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.BMIDISPLAY.OBESITY.III", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToDHSTBmiTab3()
        {
            try
            {
                if (txtHeightTab3.Value == null || txtHeightTab3.Value == 0 || txtWeightTab3.Value == null || txtWeightTab3.Value == 0)
                {
                    lblBMITab3.Text = "";
                    lblBmiDisplayTextTab3.Text = "";
                    return;
                }
                decimal bmi = 0;
                if (txtHeightTab3.Value != null && txtHeightTab3.Value != 0)
                {
                    bmi = (txtWeightTab3.Value) / ((txtHeightTab3.Value / 100) * (txtHeightTab3.Value / 100));
                }
                lblBMITab3.Text = Math.Round(bmi, 2) + "";
                if (bmi < 16)
                {
                    lblBmiDisplayTextTab3.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.SKINNY.III", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                else if (16 <= bmi && bmi < 17)
                {
                    lblBmiDisplayTextTab3.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.SKINNY.II", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                else if (17 <= bmi && bmi < (decimal)18.5)
                {
                    lblBmiDisplayTextTab3.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.SKINNY.I", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                else if ((decimal)18.5 <= bmi && bmi < 25)
                {
                    lblBmiDisplayTextTab3.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.BMIDISPLAY.NORMAL", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                else if (25 <= bmi && bmi < 30)
                {
                    lblBmiDisplayTextTab3.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.BMIDISPLAY.OVERWEIGHT", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                else if (30 <= bmi && bmi < 35)
                {
                    lblBmiDisplayTextTab3.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.BMIDISPLAY.OBESITY.I", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                else if (35 <= bmi && bmi < 40)
                {
                    lblBmiDisplayTextTab3.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.BMIDISPLAY.OBESITY.II", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                else if (40 < bmi)
                {
                    lblBmiDisplayTextTab3.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.BMIDISPLAY.OBESITY.III", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Public method
        public void MeShow()
        {
            try
            {
                //Load ngon ngu label control
                SetCaptionByLanguageKey();

                //Fill data into datasource combo
                FillDataToControlsForm();

                //Gan gia tri mac dinh
                SetDefaultValue();

                //Load du lieu
                FillDataToGridControl();

                //Set enable control default
                EnableControlChanged(this.currentServiceReqSTT);

                //Set tabindex control
                InitTabIndex();

                //Set validate rule
                ValidateForm();

                // SetControlState
                InitControlState();


                //Focus default
                SetDefaultFocus();

                SetCheckAllColumn(this.statecheckColumn);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void loadcomboBSketluan()
        {
            try
            {
                var acsUser = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 400);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboBSketluan, acsUser, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == ControlStateConstant.chkIsFinish)
                        {
                            chkIsFinish.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableControlChanged(ServiceReqStatus serviceReqStatus)
        {
            if (this.currentData == null)
            {
                this.currentServiceReqSTT = ServiceReqStatus.Default;
                serviceReqStatus = ServiceReqStatus.Default;
            }
            switch (serviceReqStatus)
            {
                case ServiceReqStatus.ChuaXuLy:
                    btnFinish.Enabled = true;
                    btnUnfinish.Enabled = false;
                    break;
                case ServiceReqStatus.DaXuLy:
                    btnFinish.Enabled = true;
                    btnUnfinish.Enabled = false;
                    break;
                case ServiceReqStatus.HoanThanh:
                    btnFinish.Enabled = false;
                    btnUnfinish.Enabled = true;
                    break;
                case ServiceReqStatus.Default:
                    btnFinish.Enabled = false;
                    btnUnfinish.Enabled = false;
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Tooltip
        private void toolTipControllerGrid_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlServiceReq)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlServiceReq.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;
                            string text = "";
                            if (info.Column.FieldName == "TRANGTHAI_IMG")
                            {
                                long serviceReqSTTId = Inventec.Common.TypeConvert.Parse.ToInt64((view.GetRowCellValue(lastRowHandle, "SERVICE_REQ_STT_ID") ?? "").ToString());
                                if (serviceReqSTTId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                                {
                                    text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.ToolTipControl.CXL", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                                }
                                else if (serviceReqSTTId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                                {
                                    text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.ToolTipControl.DXL", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                                }
                                else if (serviceReqSTTId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                                {
                                    text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.ToolTipControl.KT", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                                }

                            }
                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region ValidationProvider
        private void dxValidationProviderEditorInfo_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Shortcut
        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnFinish_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnFinish_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnUnfinish_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnUnfinish_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnF2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtServiceReqCodeForSearch.Focus();
                txtServiceReqCodeForSearch.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Button-click

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
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
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            try
            {
                FinishProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnUnfinish_Click(object sender, EventArgs e)
        {
            try
            {
                UnfinishProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                PrintProcess(printTypeCode, this.listChooseFromGrid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region GridViewServiceReq
        private void gridViewServiceReq_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ADO.ServiceReqADO dataRow = (ADO.ServiceReqADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    else if (e.Column.FieldName == "TRANGTHAI_IMG")
                    {
                        //Chua xu ly: mau trang
                        //dang xu ly: mau vang
                        //Da ket thuc: mau den

                        long statusId = dataRow.SERVICE_REQ_STT_ID;
                        if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                        {
                            e.Value = imageListIcon.Images[0];
                        }
                        else if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                        {
                            e.Value = imageListIcon.Images[1];
                        }
                        else if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                        {
                            e.Value = imageListIcon.Images[2];
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }
                    else if (e.Column.FieldName == "INSTRUCTION_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.INTRUCTION_TIME);
                    }
                    else if (e.Column.FieldName == "DOB")
                    {
                        if (dataRow.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                            e.Value = dataRow.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                        else
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataRow.TDL_PATIENT_DOB);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceReq_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    ADO.ServiceReqADO data = (ADO.ServiceReqADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];


                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void gridViewServiceReq_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    ADO.ServiceReqADO data = (ADO.ServiceReqADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewServiceReq_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlServiceReq_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                if (rowData != null)
                {
                    ChangedDataRow(rowData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlServiceReq_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var rowData = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                if (rowData != null)
                {
                    ChangedDataRow(rowData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void txtServiceReqCodeForSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtServiceReqCodeForSearch.Text.Trim()))
                    {
                        txtTreatmentCodeForSearch.Text = "";
                        txtPatientCodeForSearch.Text = "";
                        FillDataToGridControl();
                        txtServiceReqCodeForSearch.Focus();
                        txtServiceReqCodeForSearch.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTreatmentCodeForSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtTreatmentCodeForSearch.Text.Trim()))
                    {
                        txtServiceReqCodeForSearch.Text = "";
                        txtPatientCodeForSearch.Text = "";
                        FillDataToGridControl();
                        txtTreatmentCodeForSearch.Focus();
                        txtTreatmentCodeForSearch.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientCodeForSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtPatientCodeForSearch.Text.Trim()))
                    {
                        txtTreatmentCodeForSearch.Text = "";
                        txtServiceReqCodeForSearch.Text = "";
                        FillDataToGridControl();
                        txtPatientCodeForSearch.Focus();
                        txtPatientCodeForSearch.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceReqStt_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.SERVICE_REQ_STT_NAME.ToString());
                }
                if (serviceReqSttSelecteds == null || serviceReqSttSelecteds.Count == 0 || serviceReqSttList.Count == gridCheckMark.Selection.Count)
                {
                    e.DisplayText = ResourceMessage.TatCa;
                }
                else
                {
                    e.DisplayText = sb.ToString();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceReqStt_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    cboKSKContract.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKSKContract_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboKSKContract.EditValue = null;
                    cboKSKContract.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKSKContract_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboKSKContract.EditValue != null)
                    {
                        cboKSKContract.Properties.Buttons[1].Visible = true;
                    }
                    btnSearch.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsFinish_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.chkIsFinish && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkIsFinish.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.chkIsFinish;
                    csAddOrUpdate.VALUE = (chkIsFinish.Checked ? "1" : "");
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

        private void txtHeightTab1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                FillDataToDHSTBmiTab1();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtWeightTab1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                FillDataToDHSTBmiTab1();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeightTab3_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                FillDataToDHSTBmiTab3();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtWeightTab3_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                FillDataToDHSTBmiTab3();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamCirculationRankTab1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExamCirculationRankTab1.Text.Trim() == "")
                {
                    cboExamCirculationRankTab1.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboExamDigestionRankTab1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExamDigestionRankTab1.Text.Trim() == "")
                {
                    cboExamDigestionRankTab1.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboExamMuscleBoneRankTab1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExamMuscleBoneRankTab1.Text.Trim() == "")
                {
                    cboExamMuscleBoneRankTab1.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboMentalRankTab1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboMentalRankTab1.Text.Trim() == "")
                {
                    cboMentalRankTab1.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboExamKidneyUrologyRankTab1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExamKidneyUrologyRankTab1.Text.Trim() == "")
                {
                    cboExamKidneyUrologyRankTab1.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboEyeRankTab1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboEyeRankTab1.Text.Trim() == "")
                {
                    cboEyeRankTab1.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboExamStomatologyRankTab1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExamStomatologyRankTab1.Text.Trim() == "")
                {
                    cboExamStomatologyRankTab1.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboExamRepiratoryRankTab1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExamRepiratoryRankTab1.Text.Trim() == "")
                {
                    cboExamRepiratoryRankTab1.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboExamOENDRankTab1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExamOENDRankTab1.Text.Trim() == "")
                {
                    cboExamOENDRankTab1.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboExamNeurologicalRankTab1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExamNeurologicalRankTab1.Text.Trim() == "")
                {
                    cboExamNeurologicalRankTab1.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboExamDermatologyRankTab1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExamDermatologyRankTab1.Text.Trim() == "")
                {
                    cboExamDermatologyRankTab1.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboSurgeryRankTab1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboSurgeryRankTab1.Text.Trim() == "")
                {
                    cboSurgeryRankTab1.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboExamENTRankTab1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExamENTRankTab1.Text.Trim() == "")
                {
                    cboExamENTRankTab1.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboPLSucKhoeTab1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPLSucKhoeTab1.Text.Trim() == "")
                {
                    cboPLSucKhoeTab1.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboExamCirculationRankTab3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExamCirculationRankTab3.Text.Trim() == "")
                {
                    cboExamCirculationRankTab3.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboExamDigestionRankTab3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExamDigestionRankTab3.Text.Trim() == "")
                {
                    cboExamDigestionRankTab3.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboExamMuscleBoneRankTab3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExamMuscleBoneRankTab3.Text.Trim() == "")
                {
                    cboExamMuscleBoneRankTab3.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboMentalRankTab3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboMentalRankTab3.Text.Trim() == "")
                {
                    cboMentalRankTab3.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboExamKidneyUrologyRankTab3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExamKidneyUrologyRankTab3.Text.Trim() == "")
                {
                    cboExamKidneyUrologyRankTab3.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboEyeRankTab3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboEyeRankTab3.Text.Trim() == "")
                {
                    cboEyeRankTab3.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboExamStomatologyRankTab3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExamStomatologyRankTab3.Text.Trim() == "")
                {
                    cboExamStomatologyRankTab3.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboExamMucosaRankTab3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExamMucosaRankTab3.Text.Trim() == "")
                {
                    cboExamMucosaRankTab3.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboExamMotionRankTab3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExamMotionRankTab3.Text.Trim() == "")
                {
                    cboExamMotionRankTab3.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboExamLymphNodesRankTab3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExamLymphNodesRankTab3.Text.Trim() == "")
                {
                    cboExamLymphNodesRankTab3.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboExamRepiratoryRankTab3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExamRepiratoryRankTab3.Text.Trim() == "")
                {
                    cboExamRepiratoryRankTab3.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboExamOENDRankTab3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExamOENDRankTab3.Text.Trim() == "")
                {
                    cboExamOENDRankTab3.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboExamNeurologicalRankTab3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExamNeurologicalRankTab3.Text.Trim() == "")
                {
                    cboExamNeurologicalRankTab3.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboExamDermatologyRankTab3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExamDermatologyRankTab3.Text.Trim() == "")
                {
                    cboExamDermatologyRankTab3.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboSurgeryRankTab3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboSurgeryRankTab3.Text.Trim() == "")
                {
                    cboSurgeryRankTab3.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboExamENTRankTab3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExamENTRankTab3.Text.Trim() == "")
                {
                    cboExamENTRankTab3.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboExamNailRankTab3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExamNailRankTab3.Text.Trim() == "")
                {
                    cboExamNailRankTab3.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboExamHematopoieticRankTab3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExamHematopoieticRankTab3.Text.Trim() == "")
                {
                    cboExamHematopoieticRankTab3.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboExamCardiovascularRankTab3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExamCardiovascularRankTab3.Text.Trim() == "")
                {
                    cboExamCardiovascularRankTab3.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboExamCapillaryRankTab3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExamCapillaryRankTab3.Text.Trim() == "")
                {
                    cboExamCapillaryRankTab3.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboPLSucKhoeTab3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPLSucKhoeTab3.Text.Trim() == "")
                {
                    cboPLSucKhoeTab3.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void btnChooseResult_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ContentSubclinical").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ContentSubclinical");
                else if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentTreatmentId);
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)SelectDataResult);
                    listArgs.Add(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId));
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }
        private void SelectDataResult(object data)
        {
            try
            {
                if (data != null && data is string)
                {
                    string dienBien = data as string;
                    if (currentTabPageChoose == 0)
                    {
                        memCDHA.Text = dienBien;
                    }
                    else if (currentTabPageChoose == 1)
                    {
                        memNoteBlood.Text = dienBien;
                    }
                    else if (currentTabPageChoose == 2)
                    {
                        memNoteTestUre.Text = dienBien;
                    }
                    else
                    {
                        memNoteTestOth.Text = dienBien;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }

        }

        private void xtraTabControl2_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                if (xtraTabControl2.SelectedTabPageIndex == 0)
                {
                    currentTabPageChoose = 0;
                }
                else if (xtraTabControl2.SelectedTabPageIndex == 1)
                {
                    currentTabPageChoose = 1;
                }
                else if (xtraTabControl2.SelectedTabPageIndex == 2)
                {
                    currentTabPageChoose = 2;
                }
                else if (xtraTabControl2.SelectedTabPageIndex == 3)
                {
                    currentTabPageChoose = 3;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void SetCheckAllColumn(bool state)
        {
            try
            {
                this.gridColumnCheck.ImageAlignment = StringAlignment.Center;
                this.gridColumnCheck.Image = (state ? this.imageCollection1.Images[1] : this.imageCollection1.Images[0]);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewServiceReq_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InColumnPanel)
                    {
                        if (hi.Column.FieldName == "IsChecked")
                        {
                            statecheckColumn = !statecheckColumn;
                            this.SetCheckAllColumn(statecheckColumn);
                            this.GridCheckChange(statecheckColumn);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void GridCheckChange(bool checkedAll)
        {
            try
            {
                foreach (var item in this.listData)
                {
                    item.IsChecked = checkedAll;
                }
                this.gridViewServiceReq.BeginUpdate();
                this.gridViewServiceReq.GridControl.DataSource = this.listData;
                this.gridViewServiceReq.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceReq_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                checkDataPrint = this.listData.Where(o => o.IsChecked).ToList();
                if (checkDataPrint == null || checkDataPrint.Count == 0)
                {
                    var row = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    if (row != null)
                    {
                        checkDataPrint.Add(row);
                    }
                }
                var PopupMenuProcessor = new PopupMenuProcessorCheck(barManager1, RightMenuCheck_Click, checkDataPrint);
                PopupMenuProcessor.InitMenu();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RightMenuCheck_Click(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem)
                {
                    PopupMenuProcessorCheck.ItemType type = (PopupMenuProcessorCheck.ItemType)(e.Item.Tag);
                    switch (type)
                    {
                        case PopupMenuProcessorCheck.ItemType.InMps449:
                            PrintProcess(printTypeCode, checkDataPrint);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExamObsteticRank_TextChanged(object sender, EventArgs e)
        {

            try
            {
                if (cboExamObsteticRank.Text.Trim() == "")
                {
                    cboExamObsteticRank.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExamTraditionalRank_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExamTraditionalRank.Text.Trim() == "")
                {
                    cboExamTraditionalRank.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExamOccupationalTherapyRank_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExamOccupationalTherapyRank.Text.Trim() == "")
                {
                    cboExamOccupationalTherapyRank.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExamNutrionRank_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExamNutrionRank.Text.Trim() == "")
                {
                    cboExamNutrionRank.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartmentToSearch_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string str = "";
                if (_DepartmentSearchSelecteds != null && _DepartmentSearchSelecteds.Count > 0)
                {
                    if (_DepartmentSearchSelecteds.Count == listDepartment.Count)
                    {
                        str = ResourceMessage.TatCa;
                    }
                    else
                    {
                        foreach (var item in _DepartmentSearchSelecteds)
                        {
                            str += item.DEPARTMENT_NAME + ", ";
                        }
                    }

                }

                e.DisplayText = str;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamRoomToSearch_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboExamRoomToSearch.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDepartmentToSearch_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDepartmentToSearch.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamRoomToSearch_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string str = "";
                if (_ExecuteRoomSearchSelecteds != null && _ExecuteRoomSearchSelecteds.Count > 0)
                {
                    if (_ExecuteRoomSearchSelecteds.Count == listDepartmentRoom.Count)
                    {
                        str = ResourceMessage.TatCa;
                    }
                    else
                    {
                        foreach (var item in _ExecuteRoomSearchSelecteds)
                        {
                            str += item.EXECUTE_ROOM_NAME + ", ";
                        }
                    }
                }

                e.DisplayText = str;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeightTab1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != ','))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtWeightTab1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != ','))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPulseTab1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != ','))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBloodPressureMaxTab1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != ','))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBloodPressureMinTab1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != ','))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTemperatureTab1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != ','))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBreathRateTab1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != ','))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtHeightTab3_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != ','))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtWeightTab3_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != ','))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPulseTab3_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != ','))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBloodPressureMaxTab3_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != ','))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBloodPressureMinTab3_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != ','))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTemperatureTab3_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != ','))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBreathRateTab3_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != ','))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                HisKskGeneral = null;
                listDepartmentRoom = null;
                _ExecuteRoomSearchSelecteds = null;
                listExecuteRoom = null;
                _DepartmentSearchSelecteds = null;
                listDepartment = null;
                listChooseFromGrid = null;
                checkDataPrint = null;
                listData = null;
                statecheckColumn = false;
                currentTabPageChoose = 0;
                currentTreatmentId = 0;
                lastRowHandle = 0;
                lastInfo = null;
                lastColumn = null;
                moduleData = null;
                dicOrderTabIndexControl = null;
                currentData = null;
                serviceReqSttList = null;
                serviceReqSttSelecteds = null;
                moduleLink = null;
                currentControlStateRDO = null;
                controlStateWorker = null;
                positionHandle = 0;
                pagingGrid = null;
                startPage = 0;
                dataTotal = 0;
                rowCount = 0;
                listSereServExt = null;
                listTestIndex = null;
                listDhst = null;
                listHealthExamRank = null;
                listSereServTein = null;
                listSereServ = null;
                listKskGeneral = null;
                listServiceReqForPrint = null;
                printNow = false;
                _KSK_param = null;
                richEditorMain = null;
                this.txtServiceReqCodeForSearch.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtServiceReqCodeForSearch_PreviewKeyDown);
                this.txtTreatmentCodeForSearch.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtTreatmentCodeForSearch_PreviewKeyDown);
                this.chkIsFinish.CheckedChanged -= new System.EventHandler(this.chkIsFinish_CheckedChanged);
                this.btnSave.Click -= new System.EventHandler(this.btnSave_Click);
                this.btnFinish.Click -= new System.EventHandler(this.btnFinish_Click);
                this.btnUnfinish.Click -= new System.EventHandler(this.btnUnfinish_Click);
                this.btnPrint.Click -= new System.EventHandler(this.btnPrint_Click);
                this.bbtnSave.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnSave_ItemClick);
                this.bbtnFinish.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnFinish_ItemClick);
                this.bbtnUnfinish.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnUnfinish_ItemClick);
                this.bbtnF2.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnF2_ItemClick);
                this.bbtnSearch.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnSearch_ItemClick);
                this.btnChooseResult.Click -= new System.EventHandler(this.btnChooseResult_Click);
                this.xtraTabControl2.SelectedPageChanged -= new DevExpress.XtraTab.TabPageChangedEventHandler(this.xtraTabControl2_SelectedPageChanged);
                this.cboMentalRankTab1.TextChanged -= new System.EventHandler(this.cboMentalRankTab1_TextChanged);
                this.cboExamENTRankTab1.TextChanged -= new System.EventHandler(this.cboExamENTRankTab1_TextChanged);
                this.cboSurgeryRankTab1.TextChanged -= new System.EventHandler(this.cboSurgeryRankTab1_TextChanged);
                this.cboExamDermatologyRankTab1.TextChanged -= new System.EventHandler(this.cboExamDermatologyRankTab1_TextChanged);
                this.cboExamNeurologicalRankTab1.TextChanged -= new System.EventHandler(this.cboExamNeurologicalRankTab1_TextChanged);
                this.cboExamOENDRankTab1.TextChanged -= new System.EventHandler(this.cboExamOENDRankTab1_TextChanged);
                this.cboExamRepiratoryRankTab1.TextChanged -= new System.EventHandler(this.cboExamRepiratoryRankTab1_TextChanged);
                this.cboExamStomatologyRankTab1.TextChanged -= new System.EventHandler(this.cboExamStomatologyRankTab1_TextChanged);
                this.cboEyeRankTab1.TextChanged -= new System.EventHandler(this.cboEyeRankTab1_TextChanged);
                this.cboExamKidneyUrologyRankTab1.TextChanged -= new System.EventHandler(this.cboExamKidneyUrologyRankTab1_TextChanged);
                this.cboExamMuscleBoneRankTab1.TextChanged -= new System.EventHandler(this.cboExamMuscleBoneRankTab1_TextChanged);
                this.cboExamDigestionRankTab1.TextChanged -= new System.EventHandler(this.cboExamDigestionRankTab1_TextChanged);
                this.cboExamCirculationRankTab1.TextChanged -= new System.EventHandler(this.cboExamCirculationRankTab1_TextChanged);
                this.cboPLSucKhoeTab1.TextChanged -= new System.EventHandler(this.cboPLSucKhoeTab1_TextChanged);
                this.txtHeightTab1.EditValueChanged -= new System.EventHandler(this.txtHeightTab1_EditValueChanged);
                this.txtHeightTab1.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.txtHeightTab1_KeyPress);
                this.txtWeightTab1.EditValueChanged -= new System.EventHandler(this.txtWeightTab1_EditValueChanged);
                this.txtWeightTab1.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.txtWeightTab1_KeyPress);
                this.txtPulseTab1.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.txtPulseTab1_KeyPress);
                this.txtBloodPressureMaxTab1.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.txtBloodPressureMaxTab1_KeyPress);
                this.txtBloodPressureMinTab1.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.txtBloodPressureMinTab1_KeyPress);
                this.txtTemperatureTab1.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.txtTemperatureTab1_KeyPress);
                this.txtBreathRateTab1.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.txtBreathRateTab1_KeyPress);
                this.cboExamObsteticRank.TextChanged -= new System.EventHandler(this.cboExamObsteticRank_TextChanged);
                this.cboExamTraditionalRank.TextChanged -= new System.EventHandler(this.cboExamTraditionalRank_TextChanged);
                this.cboExamOccupationalTherapyRank.TextChanged -= new System.EventHandler(this.cboExamOccupationalTherapyRank_TextChanged);
                this.cboExamNutrionRank.TextChanged -= new System.EventHandler(this.cboExamNutrionRank_TextChanged);
                this.cboExamLymphNodesRankTab3.TextChanged -= new System.EventHandler(this.cboExamLymphNodesRankTab3_TextChanged);
                this.cboExamCapillaryRankTab3.TextChanged -= new System.EventHandler(this.cboExamCapillaryRankTab3_TextChanged);
                this.cboExamCardiovascularRankTab3.TextChanged -= new System.EventHandler(this.cboExamCardiovascularRankTab3_TextChanged);
                this.cboExamHematopoieticRankTab3.TextChanged -= new System.EventHandler(this.cboExamHematopoieticRankTab3_TextChanged);
                this.cboExamNailRankTab3.TextChanged -= new System.EventHandler(this.cboExamNailRankTab3_TextChanged);
                this.cboExamMotionRankTab3.TextChanged -= new System.EventHandler(this.cboExamMotionRankTab3_TextChanged);
                this.cboExamMucosaRankTab3.TextChanged -= new System.EventHandler(this.cboExamMucosaRankTab3_TextChanged);
                this.cboExamCirculationRankTab3.TextChanged -= new System.EventHandler(this.cboExamCirculationRankTab3_TextChanged);
                this.cboExamDigestionRankTab3.TextChanged -= new System.EventHandler(this.cboExamDigestionRankTab3_TextChanged);
                this.cboExamMuscleBoneRankTab3.TextChanged -= new System.EventHandler(this.cboExamMuscleBoneRankTab3_TextChanged);
                this.cboExamKidneyUrologyRankTab3.TextChanged -= new System.EventHandler(this.cboExamKidneyUrologyRankTab3_TextChanged);
                this.cboEyeRankTab3.TextChanged -= new System.EventHandler(this.cboEyeRankTab3_TextChanged);
                this.cboExamStomatologyRankTab3.TextChanged -= new System.EventHandler(this.cboExamStomatologyRankTab3_TextChanged);
                this.cboExamRepiratoryRankTab3.TextChanged -= new System.EventHandler(this.cboExamRepiratoryRankTab3_TextChanged);
                this.cboExamOENDRankTab3.TextChanged -= new System.EventHandler(this.cboExamOENDRankTab3_TextChanged);
                this.cboExamNeurologicalRankTab3.TextChanged -= new System.EventHandler(this.cboExamNeurologicalRankTab3_TextChanged);
                this.cboExamDermatologyRankTab3.TextChanged -= new System.EventHandler(this.cboExamDermatologyRankTab3_TextChanged);
                this.cboSurgeryRankTab3.TextChanged -= new System.EventHandler(this.cboSurgeryRankTab3_TextChanged);
                this.cboExamENTRankTab3.TextChanged -= new System.EventHandler(this.cboExamENTRankTab3_TextChanged);
                this.cboMentalRankTab3.TextChanged -= new System.EventHandler(this.cboMentalRankTab3_TextChanged);
                this.cboPLSucKhoeTab3.TextChanged -= new System.EventHandler(this.cboPLSucKhoeTab3_TextChanged);
                this.txtHeightTab3.EditValueChanged -= new System.EventHandler(this.txtHeightTab3_EditValueChanged);
                this.txtHeightTab3.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.txtHeightTab3_KeyPress);
                this.txtPulseTab3.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.txtPulseTab3_KeyPress);
                this.txtWeightTab3.EditValueChanged -= new System.EventHandler(this.txtWeightTab3_EditValueChanged);
                this.txtWeightTab3.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.txtWeightTab3_KeyPress);
                this.txtBloodPressureMaxTab3.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.txtBloodPressureMaxTab3_KeyPress);
                this.txtBloodPressureMinTab3.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.txtBloodPressureMinTab3_KeyPress);
                this.txtTemperatureTab3.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.txtTemperatureTab3_KeyPress);
                this.txtBreathRateTab3.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.txtBreathRateTab3_KeyPress);
                this.cboKSKContract.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboKSKContract_Closed);
                this.cboKSKContract.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboKSKContract_ButtonClick);
                this.btnSearch.Click -= new System.EventHandler(this.btnSearch_Click);
                this.txtPatientCodeForSearch.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPatientCodeForSearch_PreviewKeyDown);
                this.gridControlServiceReq.Click -= new System.EventHandler(this.gridControlServiceReq_Click);
                this.gridControlServiceReq.DoubleClick -= new System.EventHandler(this.gridControlServiceReq_DoubleClick);
                this.gridViewServiceReq.RowCellStyle -= new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridViewServiceReq_RowCellStyle);
                this.gridViewServiceReq.CustomRowCellEdit -= new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewServiceReq_CustomRowCellEdit);
                this.gridViewServiceReq.PopupMenuShowing -= new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gridViewServiceReq_PopupMenuShowing);
                this.gridViewServiceReq.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewServiceReq_CustomUnboundColumnData);
                this.gridViewServiceReq.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.gridViewServiceReq_KeyDown);
                this.gridViewServiceReq.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.gridViewServiceReq_MouseDown);
                this.cboServiceReqStt.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboServiceReqStt_Closed);
                this.cboServiceReqStt.CustomDisplayText -= new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboServiceReqStt_CustomDisplayText);
                this.cboDepartmentToSearch.Properties.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboDepartmentToSearch_Properties_ButtonClick);
                this.cboDepartmentToSearch.CustomDisplayText -= new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboDepartmentToSearch_CustomDisplayText);
                this.cboExamRoomToSearch.Properties.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboExamRoomToSearch_Properties_ButtonClick);
                this.cboExamRoomToSearch.CustomDisplayText -= new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboExamRoomToSearch_CustomDisplayText);
                this.toolTipControllerGrid.GetActiveObjectInfo -= new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.toolTipControllerGrid_GetActiveObjectInfo);
                this.dxValidationProviderEditorInfo.ValidationFailed -= new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProviderEditorInfo_ValidationFailed);
                this.txtPatientNameForSearch.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPatientNameForSearch_PreviewKeyDown);
                this.Load -= new System.EventHandler(this.frmEnterKskInfomantion_Load);
                gridView23.GridControl.DataSource = null;
                cboExamRoomToSearch.Properties.DataSource = null;
                gridView22.GridControl.DataSource = null;
                cboDepartmentToSearch.Properties.DataSource = null;
                gridView21.GridControl.DataSource = null;
                cboBSketluan.Properties.DataSource = null;
                gridView20.GridControl.DataSource = null;
                cboExamNutrionRank.Properties.DataSource = null;
                gridView19.GridControl.DataSource = null;
                cboExamOccupationalTherapyRank.Properties.DataSource = null;
                gridView18.GridControl.DataSource = null;
                cboExamTraditionalRank.Properties.DataSource = null;
                gridView17.GridControl.DataSource = null;
                cboExamObsteticRank.Properties.DataSource = null;
                gridView16.GridControl.DataSource = null;
                cboServiceReqStt.Properties.DataSource = null;
                gridLookUpEdit28View.GridControl.DataSource = null;
                cboExamMucosaRankTab3.Properties.DataSource = null;
                gridLookUpEdit29View.GridControl.DataSource = null;
                cboExamMotionRankTab3.Properties.DataSource = null;
                gridLookUpEdit30View.GridControl.DataSource = null;
                cboExamNailRankTab3.Properties.DataSource = null;
                gridLookUpEdit31View.GridControl.DataSource = null;
                cboExamHematopoieticRankTab3.Properties.DataSource = null;
                gridLookUpEdit32View.GridControl.DataSource = null;
                cboExamCardiovascularRankTab3.Properties.DataSource = null;
                gridLookUpEdit33View.GridControl.DataSource = null;
                cboExamCapillaryRankTab3.Properties.DataSource = null;
                gridLookUpEdit34View.GridControl.DataSource = null;
                cboExamLymphNodesRankTab3.Properties.DataSource = null;
                gridView8.GridControl.DataSource = null;
                cboPLSucKhoeTab3.Properties.DataSource = null;
                gridView10.GridControl.DataSource = null;
                cboMentalRankTab3.Properties.DataSource = null;
                gridView15.GridControl.DataSource = null;
                cboExamENTRankTab3.Properties.DataSource = null;
                gridView3.GridControl.DataSource = null;
                cboSurgeryRankTab3.Properties.DataSource = null;
                gridView4.GridControl.DataSource = null;
                cboExamDermatologyRankTab3.Properties.DataSource = null;
                gridView5.GridControl.DataSource = null;
                cboExamNeurologicalRankTab3.Properties.DataSource = null;
                gridView9.GridControl.DataSource = null;
                cboExamOENDRankTab3.Properties.DataSource = null;
                gridView7.GridControl.DataSource = null;
                cboExamRepiratoryRankTab3.Properties.DataSource = null;
                gridView14.GridControl.DataSource = null;
                cboExamStomatologyRankTab3.Properties.DataSource = null;
                gridView2.GridControl.DataSource = null;
                cboEyeRankTab3.Properties.DataSource = null;
                gridView12.GridControl.DataSource = null;
                cboExamKidneyUrologyRankTab3.Properties.DataSource = null;
                gridView13.GridControl.DataSource = null;
                cboExamMuscleBoneRankTab3.Properties.DataSource = null;
                gridView6.GridControl.DataSource = null;
                cboExamDigestionRankTab3.Properties.DataSource = null;
                gridView11.GridControl.DataSource = null;
                cboExamCirculationRankTab3.Properties.DataSource = null;
                gridLookUpEdit14View.GridControl.DataSource = null;
                cboPLSucKhoeTab1.Properties.DataSource = null;
                gridLookUpEdit1View.GridControl.DataSource = null;
                cboExamCirculationRankTab1.Properties.DataSource = null;
                gridLookUpEdit2View.GridControl.DataSource = null;
                cboExamDigestionRankTab1.Properties.DataSource = null;
                gridLookUpEdit3View.GridControl.DataSource = null;
                cboExamMuscleBoneRankTab1.Properties.DataSource = null;
                gridLookUpEdit4View.GridControl.DataSource = null;
                cboExamKidneyUrologyRankTab1.Properties.DataSource = null;
                gridLookUpEdit5View.GridControl.DataSource = null;
                cboEyeRankTab1.Properties.DataSource = null;
                gridLookUpEdit6View.GridControl.DataSource = null;
                cboExamStomatologyRankTab1.Properties.DataSource = null;
                gridLookUpEdit7View.GridControl.DataSource = null;
                cboExamRepiratoryRankTab1.Properties.DataSource = null;
                gridLookUpEdit8View.GridControl.DataSource = null;
                cboExamOENDRankTab1.Properties.DataSource = null;
                gridLookUpEdit9View.GridControl.DataSource = null;
                cboExamNeurologicalRankTab1.Properties.DataSource = null;
                gridLookUpEdit10View.GridControl.DataSource = null;
                cboExamDermatologyRankTab1.Properties.DataSource = null;
                gridLookUpEdit11View.GridControl.DataSource = null;
                cboSurgeryRankTab1.Properties.DataSource = null;
                gridLookUpEdit12View.GridControl.DataSource = null;
                cboExamENTRankTab1.Properties.DataSource = null;
                gridLookUpEdit13View.GridControl.DataSource = null;
                cboMentalRankTab1.Properties.DataSource = null;
                gridViewServiceReq.GridControl.DataSource = null;
                gridControlServiceReq.DataSource = null;
                gridView1.GridControl.DataSource = null;
                cboKSKContract.Properties.DataSource = null;
                emptySpaceItem4 = null;
                grdColExecuteRoomName = null;
                grdColExecuteDepartmentName = null;
                gridView23 = null;
                cboExamRoomToSearch = null;
                gridView22 = null;
                cboDepartmentToSearch = null;
                layoutControlItem149 = null;
                layoutControlItem148 = null;
                gridView21 = null;
                cboBSketluan = null;
                cboDayResult = null;
                layoutControlItem147 = null;
                lblGeneralCode = null;
                layoutControlItem146 = null;
                layoutControlItem145 = null;
                gridView20 = null;
                cboExamNutrionRank = null;
                gridView19 = null;
                cboExamOccupationalTherapyRank = null;
                gridView18 = null;
                cboExamTraditionalRank = null;
                gridView17 = null;
                cboExamObsteticRank = null;
                layoutControlItem144 = null;
                layoutControlItem143 = null;
                layoutControlItem142 = null;
                layoutControlItem67 = null;
                layoutControlItem66 = null;
                layoutControlItem65 = null;
                layoutControlItem64 = null;
                layoutControlItem31 = null;
                txtExamObstetic = null;
                txtExamOccupationalTherapy = null;
                txtExamTraditional = null;
                txtExamNutrion = null;
                layoutControlItem27 = null;
                label3 = null;
                layoutControlItem28 = null;
                layoutControlItem63 = null;
                label1 = null;
                label2 = null;
                imageCollection2 = null;
                imageCollection1 = null;
                repositoryItemCheckEdit1 = null;
                gridColumnCheck = null;
                emptySpaceItem5 = null;
                layoutControlItem62 = null;
                btnChooseResult = null;
                memNoteTestOth = null;
                memNoteTestUre = null;
                memNoteBlood = null;
                memCDHA = null;
                layoutControlItem141 = null;
                xtraTabPage8 = null;
                xtraTabPage7 = null;
                xtraTabPage6 = null;
                xtraTabPage4 = null;
                xtraTabControl2 = null;
                layoutControlItem140 = null;
                lblBmiDisplayTextTab3 = null;
                layoutControlItem139 = null;
                lblBmiDisplayTextTab1 = null;
                layoutControlItem92 = null;
                layoutControlItem91 = null;
                labelControl14 = null;
                labelControl13 = null;
                layoutControlItem87 = null;
                layoutControlItem88 = null;
                layoutControlItem84 = null;
                layoutControlItem83 = null;
                labelControl7 = null;
                labelControl11 = null;
                labelControl10 = null;
                labelControl12 = null;
                layoutControlItem32 = null;
                layoutControlItem24 = null;
                layoutControlItem23 = null;
                labelControl1 = null;
                labelControl2 = null;
                labelControl6 = null;
                txtBreathRateTab3 = null;
                txtTemperatureTab3 = null;
                txtBloodPressureMinTab3 = null;
                txtBloodPressureMaxTab3 = null;
                txtWeightTab3 = null;
                txtPulseTab3 = null;
                txtHeightTab3 = null;
                txtBreathRateTab1 = null;
                txtTemperatureTab1 = null;
                txtBloodPressureMinTab1 = null;
                txtBloodPressureMaxTab1 = null;
                txtPulseTab1 = null;
                txtWeightTab1 = null;
                txtHeightTab1 = null;
                imageListIcon = null;
                gridView16 = null;
                cboServiceReqStt = null;
                bbtnSearch = null;
                dxErrorProvider = null;
                dxValidationProviderEditorInfo = null;
                toolTipControllerGrid = null;
                bbtnF2 = null;
                bbtnUnfinish = null;
                bbtnFinish = null;
                bbtnSave = null;
                barDockControl4 = null;
                barDockControl3 = null;
                barDockControl2 = null;
                barDockControl1 = null;
                bar1 = null;
                barManager1 = null;
                layoutControlItem138 = null;
                layoutControlItem137 = null;
                layoutControlItem136 = null;
                layoutControlItem135 = null;
                layoutControlItem134 = null;
                layoutControlItem133 = null;
                layoutControlItem132 = null;
                layoutControlItem131 = null;
                txtExamCapillaryTab3 = null;
                gridLookUpEdit28View = null;
                cboExamMucosaRankTab3 = null;
                gridLookUpEdit29View = null;
                cboExamMotionRankTab3 = null;
                gridLookUpEdit30View = null;
                cboExamNailRankTab3 = null;
                gridLookUpEdit31View = null;
                cboExamHematopoieticRankTab3 = null;
                gridLookUpEdit32View = null;
                cboExamCardiovascularRankTab3 = null;
                gridLookUpEdit33View = null;
                cboExamCapillaryRankTab3 = null;
                gridLookUpEdit34View = null;
                cboExamLymphNodesRankTab3 = null;
                layoutControlItem125 = null;
                layoutControlItem124 = null;
                layoutControlItem123 = null;
                layoutControlItem122 = null;
                layoutControlItem121 = null;
                layoutControlItem120 = null;
                layoutControlItem130 = null;
                layoutControlItem129 = null;
                layoutControlItem128 = null;
                emptySpaceItem22 = null;
                emptySpaceItem20 = null;
                emptySpaceItem19 = null;
                emptySpaceItem16 = null;
                layoutControlItem127 = null;
                layoutControlItem126 = null;
                layoutControlItem119 = null;
                layoutControlItem118 = null;
                layoutControlItem117 = null;
                layoutControlItem116 = null;
                layoutControlItem115 = null;
                layoutControlItem114 = null;
                layoutControlItem113 = null;
                layoutControlItem112 = null;
                layoutControlItem111 = null;
                layoutControlItem110 = null;
                layoutControlItem109 = null;
                layoutControlItem108 = null;
                layoutControlItem107 = null;
                layoutControlItem106 = null;
                layoutControlItem105 = null;
                layoutControlItem104 = null;
                layoutControlItem103 = null;
                layoutControlItem102 = null;
                layoutControlItem101 = null;
                layoutControlItem100 = null;
                layoutControlItem99 = null;
                layoutControlItem98 = null;
                layoutControlItem97 = null;
                layoutControlItem96 = null;
                layoutControlItem95 = null;
                layoutControlItem94 = null;
                layoutControlItem93 = null;
                layoutControlItem90 = null;
                layoutControlItem89 = null;
                layoutControlItem86 = null;
                layoutControlItem85 = null;
                layoutControlItem82 = null;
                emptySpaceItem15 = null;
                emptySpaceItem14 = null;
                layoutControlItem81 = null;
                layoutControlItem80 = null;
                layoutControlItem79 = null;
                layoutControlGroup8 = null;
                txtTreatmentInstructionTab3 = null;
                txtDiseasesTab3 = null;
                gridView8 = null;
                cboPLSucKhoeTab3 = null;
                labelControl17 = null;
                labelControl19 = null;
                gridView10 = null;
                cboMentalRankTab3 = null;
                gridView15 = null;
                cboExamENTRankTab3 = null;
                gridView3 = null;
                cboSurgeryRankTab3 = null;
                gridView4 = null;
                cboExamDermatologyRankTab3 = null;
                gridView5 = null;
                cboExamNeurologicalRankTab3 = null;
                gridView9 = null;
                cboExamOENDRankTab3 = null;
                gridView7 = null;
                cboExamRepiratoryRankTab3 = null;
                gridView14 = null;
                cboExamStomatologyRankTab3 = null;
                gridView2 = null;
                cboEyeRankTab3 = null;
                gridView12 = null;
                cboExamKidneyUrologyRankTab3 = null;
                gridView13 = null;
                cboExamMuscleBoneRankTab3 = null;
                gridView6 = null;
                cboExamDigestionRankTab3 = null;
                gridView11 = null;
                cboExamCirculationRankTab3 = null;
                txtSurgeryTab3 = null;
                txtExamENTTab3 = null;
                txtExamDermatologyTab3 = null;
                txtExamStomatologyTab3 = null;
                txtEyeTab3 = null;
                txtExamKidneyUrologyTab3 = null;
                txtMentalTab3 = null;
                txtExamNeurologicalTab3 = null;
                txtExamMuscleBoneTab3 = null;
                txtExamOENDTab3 = null;
                txtExamDigestionTab3 = null;
                txtExamRepiratoryTab3 = null;
                txtExamCirculationTab3 = null;
                lblBMITab3 = null;
                txtExamNailTab3 = null;
                txtConclusionTab3 = null;
                txtExamMucosaTab3 = null;
                txtExamHematopoieticTab3 = null;
                txtExamMotionTab3 = null;
                txtExamCardiovascularTab3 = null;
                txtExamLymphNodesTab3 = null;
                layoutControlGroup6 = null;
                layoutControlTab3 = null;
                layoutControlItem78 = null;
                layoutControlItem77 = null;
                layoutControlItem76 = null;
                layoutControlItem75 = null;
                layoutControlItem74 = null;
                layoutControlItem73 = null;
                emptySpaceItem6 = null;
                layoutControlItem72 = null;
                txtConclusionClinicalTab2 = null;
                txtConclusionSubclinicalTab2 = null;
                txtOccupationalDiseaseTab2 = null;
                txtConclusionConsultationTab2 = null;
                txtProvisionalDiagnosisTab2 = null;
                txtExamConclusionTab2 = null;
                txtConclusionTab2 = null;
                layoutControlItem71 = null;
                layoutControlItem70 = null;
                gridLookUpEdit14View = null;
                cboPLSucKhoeTab1 = null;
                txtDiseasesTab1 = null;
                txtTreatmentInstructionTab1 = null;
                layoutControlItem61 = null;
                emptySpaceItem7 = null;
                layoutControlItem69 = null;
                layoutControlItem68 = null;
                labelControl9 = null;
                layoutControlItem52 = null;
                layoutControlItem60 = null;
                layoutControlItem59 = null;
                layoutControlItem58 = null;
                layoutControlItem57 = null;
                layoutControlItem56 = null;
                layoutControlItem55 = null;
                layoutControlItem54 = null;
                layoutControlItem53 = null;
                layoutControlItem51 = null;
                layoutControlItem50 = null;
                layoutControlItem49 = null;
                layoutControlItem48 = null;
                layoutControlItem47 = null;
                layoutControlItem46 = null;
                layoutControlItem45 = null;
                layoutControlItem44 = null;
                layoutControlItem43 = null;
                layoutControlItem42 = null;
                layoutControlItem41 = null;
                layoutControlItem40 = null;
                layoutControlItem39 = null;
                layoutControlItem37 = null;
                layoutControlItem36 = null;
                layoutControlItem34 = null;
                txtExamCirculationTab1 = null;
                txtExamRepiratoryTab1 = null;
                txtExamDigestionTab1 = null;
                txtExamOENDTab1 = null;
                txtExamMuscleBoneTab1 = null;
                txtExamNeurologicalTab1 = null;
                txtMentalTab1 = null;
                txtExamKidneyUrologyTab1 = null;
                txtEyeTab1 = null;
                txtExamStomatologyTab1 = null;
                txtExamDermatologyTab1 = null;
                txtExamENTTab1 = null;
                txtSurgeryTab1 = null;
                gridLookUpEdit1View = null;
                cboExamCirculationRankTab1 = null;
                gridLookUpEdit2View = null;
                cboExamDigestionRankTab1 = null;
                gridLookUpEdit3View = null;
                cboExamMuscleBoneRankTab1 = null;
                gridLookUpEdit4View = null;
                cboExamKidneyUrologyRankTab1 = null;
                gridLookUpEdit5View = null;
                cboEyeRankTab1 = null;
                gridLookUpEdit6View = null;
                cboExamStomatologyRankTab1 = null;
                gridLookUpEdit7View = null;
                cboExamRepiratoryRankTab1 = null;
                gridLookUpEdit8View = null;
                cboExamOENDRankTab1 = null;
                gridLookUpEdit9View = null;
                cboExamNeurologicalRankTab1 = null;
                gridLookUpEdit10View = null;
                cboExamDermatologyRankTab1 = null;
                gridLookUpEdit11View = null;
                cboSurgeryRankTab1 = null;
                gridLookUpEdit12View = null;
                cboExamENTRankTab1 = null;
                gridLookUpEdit13View = null;
                cboMentalRankTab1 = null;
                layoutControlItem30 = null;
                layoutControlItem29 = null;
                layoutControlItem26 = null;
                layoutControlItem25 = null;
                layoutControlItem17 = null;
                layoutControlItem22 = null;
                emptySpaceItem2 = null;
                layoutControlItem21 = null;
                layoutControlItem19 = null;
                layoutControlItem18 = null;
                lblBMITab1 = null;
                lblKetQuaKhamLamSang = null;
                layoutControlGroup5 = null;
                layoutControlTab2 = null;
                layoutControlGroup3 = null;
                layoutControlTab1 = null;
                grdColGender = null;
                grdColServiceReqCode = null;
                layoutControlItem13 = null;
                layoutControlItem12 = null;
                layoutControlItem11 = null;
                layoutControlItem10 = null;
                layoutControlItem9 = null;
                layoutControlItem8 = null;
                emptySpaceItem3 = null;
                layoutControlItem6 = null;
                layoutControlItem5 = null;
                panelControl3 = null;
                xtraTabPage3 = null;
                panelControl2 = null;
                xtraTabPage2 = null;
                panelControl1 = null;
                xtraTabPage1 = null;
                xtraTabControl1 = null;
                btnPrint = null;
                btnUnfinish = null;
                btnFinish = null;
                btnSave = null;
                chkIsFinish = null;
                txtTreatmentCodeForSearch = null;
                txtServiceReqCodeForSearch = null;
                layoutControlItem4 = null;
                layoutControlItem38 = null;
                lciAddress = null;
                layoutControlItem20 = null;
                lciTreatmentType = null;
                layoutControlItem7 = null;
                lciDOB = null;
                lciPatientName = null;
                lciPatientCode = null;
                layoutControlGroup4 = null;
                lblServiceReqCode = null;
                lblTreatmentCode = null;
                lblPatientCode = null;
                lblIntructionTime = null;
                lblGender = null;
                lblPatientName = null;
                lblKSKContract = null;
                lblBirthDate = null;
                layoutControlPatientInfo = null;
                groupBoxPatientInfo = null;
                layoutControlItem2 = null;
                layoutControlGroup2 = null;
                layoutControl3 = null;
                layoutControlItem1 = null;
                lciKSKContract = null;
                layoutControlItem14 = null;
                layoutControlItem16 = null;
                layoutControlItem15 = null;
                layoutControlItem35 = null;
                lciIntructionDate = null;
                layoutControlItem33 = null;
                layoutControlItem3 = null;
                Root = null;
                repositoryItemPictureEdit1 = null;
                grdColBirthDate = null;
                grdColInstrctionTime = null;
                grdColTreatmentCode = null;
                grdColPatientCode = null;
                grdColPatientName = null;
                grdColStatus = null;
                grdColSTT = null;
                gridViewServiceReq = null;
                gridControlServiceReq = null;
                txtPatientCodeForSearch = null;
                txtPatientNameForSearch = null;
                btnSearch = null;
                ucPaging = null;
                dtFrom = null;
                dtTo = null;
                gridView1 = null;
                cboKSKContract = null;
                layoutControl2 = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientNameForSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtPatientNameForSearch.Text.Trim()))
                    {
                        txtTreatmentCodeForSearch.Text = "";
                        txtServiceReqCodeForSearch.Text = "";
                        txtPatientCodeForSearch.Text = "";
                        FillDataToGridControl();
                        txtPatientNameForSearch.Focus();
                        txtPatientNameForSearch.SelectAll();
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
