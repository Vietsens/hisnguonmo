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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.LisSampleUpdate.ADO;
using HIS.Desktop.Plugins.LisSampleUpdate.Resources;
using HIS.Desktop.Plugins.LisSampleUpdate.Validation;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.PopupLoader;
using Inventec.Common.Integrate.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
using LIS.SDO;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Common.ToKhaiYTe;

namespace HIS.Desktop.Plugins.LisSampleUpdate
{
    public partial class frmUpdateLisSample : FormBase
    {
        private int positionHandle = -1;

        private LIS_SAMPLE sample;
        List<LIS_SAMPLE_TYPE> sampleTypes = new List<LIS_SAMPLE_TYPE>();
        bool isSearchOrderByXHT = false;
        internal bool isNotPatientDayDob = false;
        private List<HIS_MEDI_ORG> listMediOrg = new List<HIS_MEDI_ORG>();
        private static HIS_MEDI_ORG recentMediOrg = null;
        private bool isInit = false;
        private bool isNotLoadWhileChangeControlStateInFirst = false;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentBySessionControlStateRDO;
        const string moduleLink = "HIS.Desktop.Plugins.LisSampleUpdate";

        internal bool isDobTextEditKeyEnter;

        public frmUpdateLisSample(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
        }

        void InitControlState()
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = true;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentControlStateRDO), currentControlStateRDO));
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == ControlStateConstan.Province)
                        {
                            btnStateForcboProvince.Properties.Buttons[0].Visible = (item.VALUE == "1");
                            btnStateForcboProvince.Properties.Buttons[1].Visible = !(item.VALUE == "1");
                        }
                        else if (item.KEY == ControlStateConstan.District)
                        {
                            btnStateForcboDistrict.Properties.Buttons[0].Visible = (item.VALUE == "1");
                            btnStateForcboDistrict.Properties.Buttons[1].Visible = !(item.VALUE == "1");
                        }
                        else if (item.KEY == ControlStateConstan.Commune)
                        {
                            btnStateForcboCommune.Properties.Buttons[0].Visible = (item.VALUE == "1");
                            btnStateForcboCommune.Properties.Buttons[1].Visible = !(item.VALUE == "1");
                        }
                        else if (item.KEY == ControlStateConstan.SampleSender)
                        {
                            btnStateForcboMediOrgCode.Properties.Buttons[0].Visible = (item.VALUE == "1");
                            btnStateForcboMediOrgCode.Properties.Buttons[1].Visible = !(item.VALUE == "1");
                        }
                        else if (item.KEY == ControlStateConstan.SpecimenOrder)
                        {
                            btnStateFortxtSPECIMEN_ORDER.Properties.Buttons[0].Visible = (item.VALUE == "1");
                            btnStateFortxtSPECIMEN_ORDER.Properties.Buttons[1].Visible = !(item.VALUE == "1");
                        }
                    }

                    var csPin = this.currentControlStateRDO.Where(o => o.VALUE == "1").ToList();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csPin), csPin));
                    if (csPin != null && csPin.Count > 0)
                    {
                        this.currentBySessionControlStateRDO = controlStateWorker.GetDataBySession(moduleLink);
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentBySessionControlStateRDO), currentBySessionControlStateRDO));
                        if (this.currentBySessionControlStateRDO != null && this.currentBySessionControlStateRDO.Count > 0)
                        {
                            V_SDA_PROVINCE province = null;
                            V_SDA_DISTRICT district = null;

                            var proST = this.currentBySessionControlStateRDO.FirstOrDefault(t => t.KEY == ControlStateConstan.Province);
                            if (proST != null && !String.IsNullOrEmpty(proST.VALUE))
                            {
                                province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_CODE == proST.VALUE);
                                if (province != null)
                                {
                                    txtProvinceCode.Text = province.PROVINCE_CODE;
                                    cboProvince.EditValue = province.PROVINCE_CODE;
                                }
                            }
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => province), province));

                            var disST = this.currentBySessionControlStateRDO.FirstOrDefault(t => t.KEY == ControlStateConstan.District);
                            if (disST != null && !String.IsNullOrEmpty(disST.VALUE) && province != null)
                            {
                                district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().FirstOrDefault(o => (o.DISTRICT_CODE) == disST.VALUE && o.PROVINCE_CODE == province.PROVINCE_CODE);
                                if (district != null)
                                {
                                    if (province == null)
                                    {
                                        province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_CODE == district.PROVINCE_CODE);
                                        if (province != null)
                                        {
                                            txtProvinceCode.Text = province.PROVINCE_CODE;
                                            cboProvince.EditValue = province.PROVINCE_CODE;
                                        }
                                    }

                                    this.LoadHuyenCombo("", district.PROVINCE_CODE, false);
                                    txtDistrictCode.Text = district.DISTRICT_CODE;
                                    cboDistrict.EditValue = district.DISTRICT_CODE;
                                }
                            }
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => district), district));

                            foreach (var item in this.currentBySessionControlStateRDO)
                            {
                                var csPinOne = csPin.Where(o => o.KEY == item.KEY).FirstOrDefault();
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csPinOne), csPinOne)
                                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item));
                                if (csPinOne != null && csPinOne.VALUE == "1" && !String.IsNullOrEmpty(item.VALUE))
                                {
                                    if (item.KEY == ControlStateConstan.Commune)
                                    {
                                        //txtCommuneCode.Text = item.VALUE;
                                        //cboCommune.EditValue = item.VALUE;
                                        V_SDA_COMMUNE commune = null;
                                        if (district == null && province == null)
                                        {
                                            commune = !String.IsNullOrEmpty(item.VALUE) ? BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().FirstOrDefault(o =>
                        o.COMMUNE_CODE == item.VALUE) : null;

                                            district = commune != null ? BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().FirstOrDefault(o => (o.DISTRICT_CODE) == commune.DISTRICT_CODE) : null;

                                            if (district != null)
                                            {
                                                if (province == null)
                                                {
                                                    province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_CODE == district.PROVINCE_CODE);
                                                    if (province != null)
                                                    {
                                                        txtProvinceCode.Text = province.PROVINCE_CODE;
                                                        cboProvince.EditValue = province.PROVINCE_CODE;
                                                    }
                                                }

                                                this.LoadHuyenCombo("", district.PROVINCE_CODE, false);
                                                txtDistrictCode.Text = district.DISTRICT_CODE;
                                                cboDistrict.EditValue = district.DISTRICT_CODE;
                                            }
                                        }
                                        else
                                        {
                                            commune = !String.IsNullOrEmpty(item.VALUE) ? BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().FirstOrDefault(o =>
                        o.COMMUNE_CODE == item.VALUE
                        && (o.DISTRICT_CODE) == district.DISTRICT_CODE) : null;
                                        }

                                        if (commune != null && district != null && province != null)
                                        {
                                            this.LoadXaCombo("", commune.DISTRICT_CODE, false);
                                            this.cboCommune.EditValue = commune.COMMUNE_CODE;
                                            this.txtCommuneCode.Text = commune.COMMUNE_CODE;
                                            this.cboTHX.EditValue = "C" + commune.ID;//ID_RAW
                                            bool isSearchOrderByXHT = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS_DESKTOP_REGISTER__SEARCH_CODE__X/H/T") == "1" ? true : false;

                                            this.txtMaTHX.Text = isSearchOrderByXHT ? String.Format("{0}{1}{2}", commune.SEARCH_CODE, district.SEARCH_CODE, province.SEARCH_CODE) : String.Format("{0}{1}{2}", province.SEARCH_CODE, district.SEARCH_CODE, commune.SEARCH_CODE);
                                        }
                                        else if (province != null && district != null)
                                        {
                                            var dist = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().FirstOrDefault(o =>
                                    (o.DISTRICT_CODE == district.DISTRICT_CODE && o.PROVINCE_CODE == province.PROVINCE_CODE));

                                            if (dist != null)
                                            {
                                                var com = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>().FirstOrDefault(o =>
                                        o.ID == -dist.ID);
                                                if (com != null)
                                                {
                                                    this.cboTHX.EditValue = "C" + com.ID;
                                                    this.txtMaTHX.Text = com.SEARCH_CODE_COMMUNE;
                                                }
                                            }
                                        }
                                        else if (commune != null && district != null)
                                        {
                                            var communeTHX = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>().FirstOrDefault(o =>
                                            (o.SEARCH_CODE_COMMUNE) == (province.SEARCH_CODE + district.SEARCH_CODE)
                                            && o.ID < 0);
                                            if (communeTHX != null)
                                            {
                                                this.cboTHX.EditValue = communeTHX.ID_RAW;
                                                this.txtMaTHX.Text = communeTHX.SEARCH_CODE_COMMUNE;
                                            }
                                        }
                                    }
                                    else if (item.KEY == ControlStateConstan.SampleSender)
                                    {

                                        var srarr = item.VALUE.Split('|');
                                        if (srarr != null && srarr.Count() > 0)
                                        {
                                            txtMediOrgCode.Text = srarr[0];
                                            cboMediOrgCode.EditValue = srarr[0];
                                            if (srarr != null && srarr.Count() > 1)
                                                txtSampleSender.Text = srarr[1];
                                        }
                                    }
                                    else if (item.KEY == ControlStateConstan.SpecimenOrder)
                                    {
                                        txtSPECIMEN_ORDER.Text = item.VALUE;
                                    }
                                }
                            }
                        }
                    }
                }
                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmUpdateCondition_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                Config.HisConfigCFG.LoadConfig();
                this.btnSave.Enabled = false;
                this.ValidControl();
                this.ShowHideControlAddress();
                this.LoadComboSampleType();
                this.LoadHisMediOrg();
                this.InitComboCommonUtil(this.cboTHX, BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>(), "ID_RAW", "RENDERER_PDC_NAME", 400, "SEARCH_CODE_COMMUNE", 150);
                this.InitComboCommon(this.cboProvince, BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>(), "PROVINCE_CODE", "PROVINCE_NAME", "SEARCH_CODE");
                this.InitComboCommon(this.cboDistrict, BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>(), "DISTRICT_CODE", "RENDERER_DISTRICT_NAME", "SEARCH_CODE");
                this.InitComboCommon(this.cboCommune, BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>(), "COMMUNE_CODE", "RENDERER_COMMUNE_NAME", "SEARCH_CODE");
                this.InitComboCommon(this.cboGender, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>(), "GENDER_CODE", "GENDER_NAME", "GENDER_CODE");
                this.InitControlState();
                WaitingManager.Hide();
                this.KeyPreview = true;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowHideControlAddress()
        {
            try
            {
                isSearchOrderByXHT = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS_DESKTOP_REGISTER__SEARCH_CODE__X/H/T") == "1" ? true : false;
                if (isSearchOrderByXHT)
                {
                    lciTHX.Text = "X/H/T:";
                }
                else
                {
                    lciTHX.Text = "T/H/X:";
                }

                ValidateMaxlengthTextControl(this.txtAddress, 200);
                ValidateMaxlengthTextControl(this.txtPhone, 12);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboSampleType()
        {
            try
            {
                LisSampleTypeFilter filter = new LisSampleTypeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.LIS_RS.COMMON.IS_ACTIVE__TRUE;
                this.sampleTypes = new BackendAdapter(new CommonParam()).Get<List<LIS_SAMPLE_TYPE>>("api/LisSampleType/Get", ApiConsumers.LisConsumer, filter, null);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SAMPLE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SAMPLE_TYPE_NAME", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SAMPLE_TYPE_NAME", "ID", columnInfos, false, 400);
                ControlEditorLoader.Load(this.cboSampleType, this.sampleTypes, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadHisMediOrg()
        {
            try
            {
                HisMediOrgFilter filter = new HisMediOrgFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                this.listMediOrg = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDI_ORG>>("api/HisMediOrg/Get", ApiConsumers.MosConsumer, filter, null);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDI_ORG_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("MEDI_ORG_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_ORG_NAME", "MEDI_ORG_CODE", columnInfos, false, 400);
                ControlEditorLoader.Load(cboMediOrgCode, this.listMediOrg, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                this.ValidationSingleControl(cboSampleType, dxValidationProviderControl);
                this.ValidationSingleControl(txtBarcode, dxValidationProviderControl);
                this.ValidateMaxlengthTextControl(this.txtNote, 500);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandle = -1;
                if (!btnSave.Enabled || this.sample == null || !dxValidationProviderControl.Validate()) return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                LIS_SAMPLE sampleRaw = new LIS_SAMPLE();
                Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(sampleRaw, this.sample);
                sampleRaw.SAMPLE_TYPE_ID = cboSampleType.EditValue != null ? (long?)cboSampleType.EditValue : null;
                int idx = txtPatientName.Text.Trim().LastIndexOf(" ");
                if (idx > -1)
                {
                    sampleRaw.FIRST_NAME = txtPatientName.Text.Trim().Substring(idx).Trim();
                    sampleRaw.LAST_NAME = txtPatientName.Text.Trim().Substring(0, idx).Trim();
                }
                else
                {
                    sampleRaw.FIRST_NAME = txtPatientName.Text.Trim();
                    sampleRaw.LAST_NAME = "";
                }


                DateUtil.DateValidObject dateValidObject = DateUtil.ValidPatientDob(this.txtPatientDob.Text);
                if (dateValidObject != null)
                {
                    this.dtPatientDob.EditValue = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(dateValidObject.OutDate);
                    this.dtPatientDob.Update();
                    sampleRaw.DOB = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(this.dtPatientDob.DateTime) ?? 0;
                    sampleRaw.IS_HAS_NOT_DAY_DOB = dateValidObject.HasNotDayDob ? (short?)1 : null;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sampleRaw.DOB), sampleRaw.DOB)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sampleRaw.IS_HAS_NOT_DAY_DOB), sampleRaw.IS_HAS_NOT_DAY_DOB)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dateValidObject), dateValidObject));
                }

                sampleRaw.GENDER_CODE = cboGender.EditValue != null ? (string)cboGender.EditValue : null;
                sampleRaw.GENDER_NAME = cboGender.Text;
                sampleRaw.PROVINCE_CODE = cboProvince.EditValue != null ? (string)cboProvince.EditValue : null;
                sampleRaw.PROVINCE_NAME = cboProvince.Text;
                sampleRaw.DISTRICT_CODE = cboDistrict.EditValue != null ? (string)cboDistrict.EditValue : null;
                sampleRaw.DISTRICT_NAME = cboDistrict.Text;
                sampleRaw.COMMUNE_CODE = cboCommune.EditValue != null ? (string)cboCommune.EditValue : null;
                sampleRaw.COMMUNE_NAME = cboCommune.Text;
                sampleRaw.ADDRESS = txtAddress.Text;
                sampleRaw.PHONE_NUMBER = txtPhone.Text;
                if (cboMediOrgCode.EditValue != null)
                {
                    HIS_MEDI_ORG org = listMediOrg != null ? listMediOrg.FirstOrDefault(o => o.MEDI_ORG_CODE == cboMediOrgCode.EditValue.ToString()) : null;
                    sampleRaw.SAMPLE_SENDER_CODE = org != null ? org.MEDI_ORG_CODE : "";
                    sampleRaw.SAMPLE_SENDER = org != null ? org.MEDI_ORG_NAME : "";
                }
                else
                {
                    sampleRaw.SAMPLE_SENDER = txtSampleSender.Text ?? "";
                    sampleRaw.SAMPLE_SENDER_CODE = null;
                }
                if (this.dtSickTime.EditValue != null)
                    sampleRaw.SICK_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(this.dtSickTime.DateTime) ?? 0;
                else
                    sampleRaw.SICK_TIME = null;
                if (this.dtSampleTime.EditValue != null)
                    sampleRaw.SAMPLE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(this.dtSampleTime.DateTime) ?? 0;
                else
                    sampleRaw.SAMPLE_TIME = null;

                if (this.dtTGNhanMau.EditValue != null)
                    sampleRaw.RESULT_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(this.dtTGNhanMau.DateTime) ?? 0;
                else
                    sampleRaw.RESULT_TIME = null;
                if (!String.IsNullOrEmpty(txtSPECIMEN_ORDER.Text) && Inventec.Common.TypeConvert.Parse.ToInt64(txtSPECIMEN_ORDER.Text) > 0)
                {
                    sampleRaw.SPECIMEN_ORDER = Inventec.Common.TypeConvert.Parse.ToInt64(txtSPECIMEN_ORDER.Text);
                }
                else
                    sampleRaw.SPECIMEN_ORDER = null;
                sampleRaw.NOTE = txtNote.Text.Trim();

                LisSampleInfoSDO sdo = new LisSampleInfoSDO();
                sdo.Sample = sampleRaw;

                V_HIS_ROOM room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModuleBase.RoomId);
                if (room != null)
                {
                    sdo.WorkingBranchCode = room.BRANCH_CODE;
                    sdo.WorkingBranchName = room.BRANCH_NAME;
                    sdo.WorkingDepartmentCode = room.DEPARTMENT_CODE;
                    sdo.WorkingDepartmentName = room.DEPARTMENT_NAME;
                    sdo.WorkingRoomCode = room.ROOM_CODE;
                    sdo.WorkingRoomName = room.ROOM_NAME;
                }

                LogSystem.Info(LogUtil.TraceData("SampleInfoSDO", sdo));

                LIS_SAMPLE rowBe = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/UpdateInfo", ApiConsumers.LisConsumer, sdo, null);
                if (rowBe != null)
                {
                    success = true;
                    this.sample = rowBe;
                }
                WaitingManager.Hide();


                #region Show message
                MessageManager.Show(this, param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave.Focus();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
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

        private void cboTHX_Properties_GetNotInListValue(object sender, DevExpress.XtraEditors.Controls.GetNotInListValueEventArgs e)
        {
            try
            {
                if (e.FieldName == "RENDERER_PDC_NAME")
                {
                    var item = ((List<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>)this.cboTHX.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                    //e.Value = string.Format("{0} - {1} {2}{3}", item.PROVINCE_NAME, item.DISTRICT_INITIAL_NAME, item.DISTRICT_NAME, (String.IsNullOrEmpty(item.COMMUNE_NAME) ? "" : " - " + item.INITIAL_NAME + " " + item.COMMUNE_NAME));
                    {
                        if (isSearchOrderByXHT)
                        {
                            string x1 = (String.IsNullOrEmpty(item.COMMUNE_NAME) ? "" : "" + item.INITIAL_NAME + " " + item.COMMUNE_NAME);
                            string h1 = (String.IsNullOrEmpty(item.DISTRICT_INITIAL_NAME) ? "" : (String.IsNullOrEmpty(x1) ? "" : " - ") + item.DISTRICT_INITIAL_NAME) + (String.IsNullOrEmpty(item.DISTRICT_NAME) ? "" : " " + item.DISTRICT_NAME);
                            string t1 = (String.IsNullOrEmpty(item.PROVINCE_NAME) ? "" : " - " + item.PROVINCE_NAME);
                            e.Value = string.Format("{0}{1}{2}", x1, h1, t1);
                        }
                        else
                        {
                            string t1 = item.PROVINCE_NAME;

                            string h1 = (String.IsNullOrEmpty(item.DISTRICT_INITIAL_NAME) ? "" : " - " + item.DISTRICT_INITIAL_NAME);
                            string h2 = !String.IsNullOrEmpty(item.DISTRICT_NAME) ?
                                String.IsNullOrEmpty(h1) ? "- " + item.DISTRICT_NAME : item.DISTRICT_NAME : "";

                            string x1 = (String.IsNullOrEmpty(item.COMMUNE_NAME) ? "" : " - " + item.INITIAL_NAME + " " + item.COMMUNE_NAME);

                            e.Value = string.Format("{0}{1} {2}{3}", t1, h1, h2, x1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cboProvince_Properties_GetNotInListValue(object sender, DevExpress.XtraEditors.Controls.GetNotInListValueEventArgs e)
        {
            try
            {
                if (e.FieldName == "RENDERER_PROVINCE_NAME")
                {
                    var item = ((List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>)this.cboProvince.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0} {1}", "", item.PROVINCE_NAME);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCommune_Properties_GetNotInListValue(object sender, DevExpress.XtraEditors.Controls.GetNotInListValueEventArgs e)
        {
            try
            {
                if (e.FieldName == "RENDERER_COMMUNE_NAME")
                {
                    var item = ((List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>)this.cboCommune.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0} {1}", item.INITIAL_NAME, item.COMMUNE_NAME);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistrict_Properties_GetNotInListValue(object sender, DevExpress.XtraEditors.Controls.GetNotInListValueEventArgs e)
        {
            try
            {
                if (e.FieldName == "RENDERER_DISTRICT_NAME")
                {
                    var item = ((List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>)this.cboDistrict.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0} {1}", item.INITIAL_NAME, item.DISTRICT_NAME);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetSourceValueTHX(List<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO> communeADOs)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("UCAddressCombo--------SetSourceValueTHX------1-");
                if (communeADOs != null)
                    this.InitComboCommonUtil(this.cboTHX, communeADOs, "ID_RAW", "RENDERER_PDC_NAME", 400, "SEARCH_CODE_COMMUNE", 150);
                this.cboTHX.EditValue = null;
                this.cboTHX.Properties.Buttons[1].Visible = false;
                this.FocusShowpopup(this.cboTHX, false);
                Inventec.Common.Logging.LogSystem.Debug("UCAddressCombo--------SetSourceValueTHX------2-");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMaTHX_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string maTHX = (sender as DevExpress.XtraEditors.TextEdit).Text.Trim();
                    if (String.IsNullOrEmpty(maTHX))
                    {
                        this.SetSourceValueTHX(BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>());
                        return;
                    }
                    this.SetSourceValueTHX(BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>());//Load lai trong TH cbo bi set lai dataSource
                    this.cboTHX.EditValue = null;
                    List<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO> listResult = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>()
                        .Where(o => (o.SEARCH_CODE_COMMUNE != null
                            && o.SEARCH_CODE_COMMUNE.ToUpper().StartsWith(maTHX.ToUpper()))).ToList();
                    if (listResult != null && listResult.Count >= 1)
                    {
                        var dataNoCommunes = listResult.Where(o => o.ID < 0).ToList();
                        if (dataNoCommunes != null && dataNoCommunes.Count > 1)
                        {
                            this.SetSourceValueTHX(listResult);
                        }
                        else if (dataNoCommunes != null && dataNoCommunes.Count == 1)
                        {
                            this.cboTHX.Properties.Buttons[1].Visible = true;
                            this.cboTHX.EditValue = dataNoCommunes[0].ID_RAW;
                            this.txtMaTHX.Text = dataNoCommunes[0].SEARCH_CODE_COMMUNE;

                            var districtDTO = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().SingleOrDefault(o => o.DISTRICT_CODE == dataNoCommunes[0].DISTRICT_CODE);
                            if (districtDTO != null)
                            {
                                this.LoadHuyenCombo("", districtDTO.PROVINCE_CODE, false);
                                this.cboProvince.EditValue = districtDTO.PROVINCE_CODE;
                                this.txtProvinceCode.Text = districtDTO.PROVINCE_CODE;
                                //this.dlgSetAddressUCProvinceOfBirth(districtDTO, true);
                            }
                            this.LoadXaCombo("", dataNoCommunes[0].DISTRICT_CODE, false);
                            this.cboDistrict.EditValue = dataNoCommunes[0].DISTRICT_CODE;
                            this.txtDistrictCode.Text = dataNoCommunes[0].DISTRICT_CODE;

                            this.cboCommune.Focus();
                            this.cboCommune.ShowPopup();
                        }
                        else if (listResult.Count == 1)
                        {
                            this.SetSourceValueTHX(BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>());
                            this.cboTHX.Properties.Buttons[1].Visible = true;
                            this.cboTHX.EditValue = listResult[0].ID_RAW;
                            this.txtMaTHX.Text = listResult[0].SEARCH_CODE_COMMUNE;

                            var districtDTO = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.ID == listResult[0].DISTRICT_ID).SingleOrDefault();
                            if (districtDTO != null)
                            {
                                this.LoadHuyenCombo("", districtDTO.PROVINCE_CODE, false);
                                this.cboProvince.EditValue = districtDTO.PROVINCE_CODE;
                                this.txtProvinceCode.Text = districtDTO.PROVINCE_CODE;
                                //this.dlgSetAddressUCProvinceOfBirth(districtDTO, true);
                            }
                            this.LoadXaCombo("", listResult[0].DISTRICT_CODE, false);
                            this.cboDistrict.EditValue = listResult[0].DISTRICT_CODE;
                            this.txtDistrictCode.Text = listResult[0].DISTRICT_CODE;
                            this.cboCommune.EditValue = listResult[0].COMMUNE_CODE;
                            this.txtCommuneCode.Text = listResult[0].COMMUNE_CODE;

                            if (this.cboProvince.EditValue != null
                                && this.cboDistrict.EditValue != null
                                && this.cboCommune.EditValue != null)
                            {
                                FocusToAddress();
                            }
                            else
                            {
                                FocusToCommune();
                            }
                        }
                        else
                        {
                            this.SetSourceValueTHX(listResult);
                        }
                    }
                    else
                    {
                        //this.SetSourceValueTHX(null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTHX_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.cboTHX.EditValue = null;
                    this.cboTHX.Properties.Buttons[1].Visible = false;
                    this.txtMaTHX.Text = "";

                    this.SetValueHeinAddressByAddressOfPatient();
                    //this.SetValueForUCPlusInfo();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTHX_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboTHX.EditValue != null)
                    {
                        this.cboTHX.Properties.Buttons[1].Visible = true;
                        HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO commune = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>().SingleOrDefault(o => o.ID_RAW == (this.cboTHX.EditValue ?? "").ToString());
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.cboTHX.EditValue), this.cboTHX.EditValue));
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => commune), commune));
                        if (commune != null)
                        {
                            //Trường hợp chọn huyện/xã sẽ tự động điền thông tin vào ô tỉnh/huyện/xã & focus xuống ô địa chỉ
                            this.txtMaTHX.Text = commune.SEARCH_CODE_COMMUNE;
                            if (!String.IsNullOrEmpty(commune.DISTRICT_CODE))
                            {
                                var districtDTO = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().SingleOrDefault(o => o.DISTRICT_CODE == commune.DISTRICT_CODE);
                                if (districtDTO != null)
                                {
                                    this.LoadHuyenCombo("", districtDTO.PROVINCE_CODE, false);
                                    this.cboProvince.EditValue = districtDTO.PROVINCE_CODE;
                                    this.txtProvinceCode.Text = districtDTO.PROVINCE_CODE;
                                    //this.dlgSetAddressUCProvinceOfBirth(districtDTO, true);
                                }
                                this.LoadXaCombo("", commune.DISTRICT_CODE, false);
                                this.cboDistrict.EditValue = commune.DISTRICT_CODE;
                                this.txtDistrictCode.Text = commune.DISTRICT_CODE;

                                if (commune.ID < 0)
                                {
                                    FocusToAddress();
                                }
                                else
                                {
                                    this.cboCommune.EditValue = commune.COMMUNE_CODE;
                                    this.txtCommuneCode.Text = commune.COMMUNE_CODE;
                                    if (this.cboProvince.EditValue != null
                                        && this.cboDistrict.EditValue != null
                                        && this.cboCommune.EditValue != null)
                                    {
                                        FocusToAddress();
                                    }
                                    else
                                    {
                                        FocusToCommune();
                                    }
                                }
                            }
                            //Trường hợp chọn 1 dòng là tỉnh => chỉ điền giá trị vào ô tỉnh & focus xuống ô địa chỉ
                            else
                            {
                                this.LoadHuyenCombo("", commune.PROVINCE_CODE, false);
                                this.cboProvince.EditValue = commune.PROVINCE_CODE;
                                this.txtProvinceCode.Text = commune.PROVINCE_CODE;

                                FocusToAddress();
                            }
                        }
                    }
                    else
                    {
                        if (this.cboProvince.EditValue != null
                            && this.cboDistrict.EditValue != null
                            && this.cboCommune.EditValue != null)
                        {
                            FocusToAddress();
                        }
                        else
                        {
                            FocusToCommune();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtProvinceCode_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.txtProvinceCode.Text))
                {
                    this.cboProvince.Properties.DataSource = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtProvinceCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.LoadTinhThanhCombo((sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper(), true);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cboProvince_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboProvince.EditValue != null
                        && this.cboProvince.EditValue != this.cboProvince.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().SingleOrDefault(o => o.PROVINCE_CODE == cboProvince.EditValue.ToString());
                        if (province != null)
                        {
                            this.LoadHuyenCombo("", province.PROVINCE_CODE, false);
                            this.txtProvinceCode.Text = province.PROVINCE_CODE;
                            //this.dlgSetAddressUCProvinceOfBirth(province, true);
                        }
                    }
                    this.txtDistrictCode.Text = "";
                    FocusToDistrict();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProvince_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.cboProvince.EditValue != null && !(cboProvince.EditValue.Equals(this.cboProvince.OldEditValue == null ? "" : this.cboProvince.OldEditValue)))
                {
                    this.cboCommune.Properties.DataSource = null;
                    this.cboCommune.EditValue = null;
                    this.txtCommuneCode.Text = "";
                    this.txtDistrictCode.Text = "";
                    this.cboDistrict.EditValue = null;

                    this.SetValueHeinAddressByAddressOfPatient();
                    //this.SetValueForUCPlusInfo();
                }

                this.UpdateStateForAllControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Goi ham gan du lieu thong tinh hanh chinh tu su kien cboProvince_EditValuechanged khong thanh cong : \n" + ex);
            }
        }

        private void cboProvince_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboProvince.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().SingleOrDefault(o => o.PROVINCE_CODE == this.cboProvince.EditValue.ToString());
                        if (province != null)
                        {
                            this.LoadHuyenCombo("", province.PROVINCE_CODE, false);
                            this.txtProvinceCode.Text = province.PROVINCE_CODE;
                            this.txtDistrictCode.Text = "";
                            //this.dlgSetAddressUCProvinceOfBirth(province,true);
                            FocusToDistrict();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDistrictCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string provinceCode = "";
                    if (this.cboProvince.EditValue != null)
                    {
                        provinceCode = this.cboProvince.EditValue.ToString();
                    }
                    this.LoadHuyenCombo((sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper(), provinceCode, true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistrict_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboDistrict.EditValue != null
                        && this.cboDistrict.EditValue != this.cboDistrict.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>()
                            .SingleOrDefault(o => o.DISTRICT_CODE == this.cboDistrict.EditValue.ToString()
                                && (String.IsNullOrEmpty((this.cboProvince.EditValue ?? "").ToString()) || o.PROVINCE_CODE == (this.cboProvince.EditValue ?? "").ToString()));
                        if (district != null)
                        {
                            if (String.IsNullOrEmpty((this.cboProvince.EditValue ?? "").ToString()))
                            {
                                this.cboProvince.EditValue = district.PROVINCE_CODE;
                                this.txtProvinceCode.Text = district.PROVINCE_CODE;
                            }
                            this.LoadXaCombo("", district.DISTRICT_CODE, false);
                            this.txtDistrictCode.Text = district.DISTRICT_CODE;
                            this.cboDistrict.EditValue = district.DISTRICT_CODE;
                            this.cboCommune.EditValue = null;
                            this.txtCommuneCode.Text = "";
                        }
                    }
                    FocusToCommune();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistrict_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.cboDistrict.EditValue != null && !(this.cboDistrict.EditValue.Equals(this.cboDistrict.OldEditValue == null ? "" : this.cboDistrict.OldEditValue)))
                {
                    this.SetValueHeinAddressByAddressOfPatient();
                    //this.SetValueForUCPlusInfo();
                }
                this.UpdateStateForAllControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDistrict_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboDistrict.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>()
                            .SingleOrDefault(o => o.DISTRICT_CODE == this.cboDistrict.EditValue.ToString()
                               && (String.IsNullOrEmpty((this.cboProvince.EditValue ?? "").ToString()) || o.PROVINCE_CODE == (this.cboProvince.EditValue ?? "").ToString()));
                        if (district != null)
                        {
                            if (String.IsNullOrEmpty((this.cboProvince.EditValue ?? "").ToString()))
                            {
                                this.cboProvince.EditValue = district.PROVINCE_CODE;
                            }
                            this.LoadXaCombo("", district.DISTRICT_CODE, false);
                            this.txtDistrictCode.Text = district.DISTRICT_CODE;
                            this.cboCommune.EditValue = null;
                            this.txtCommuneCode.Text = "";
                            FocusToCommune();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCommuneCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string districtCode = "";
                    if (this.cboDistrict.EditValue != null)
                    {
                        districtCode = this.cboDistrict.EditValue.ToString();
                    }
                    this.LoadXaCombo((sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper(), districtCode, true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCommune_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboCommune.EditValue != null
                        && this.cboCommune.EditValue != cboCommune.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>()
                            .SingleOrDefault(o =>
                                o.COMMUNE_CODE == this.cboCommune.EditValue.ToString()
                                    && (String.IsNullOrEmpty((this.cboDistrict.EditValue ?? "").ToString()) || o.DISTRICT_CODE == (this.cboDistrict.EditValue ?? "").ToString())
                                );
                        if (commune != null)
                        {
                            this.txtCommuneCode.Text = commune.COMMUNE_CODE;
                            if (String.IsNullOrEmpty((this.cboProvince.EditValue ?? "").ToString()) && String.IsNullOrEmpty((this.cboDistrict.EditValue ?? "").ToString()))
                            {
                                this.cboDistrict.EditValue = commune.DISTRICT_CODE;
                                SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.ID == commune.DISTRICT_ID).FirstOrDefault();
                                if (district != null)
                                {
                                    this.cboProvince.EditValue = district.PROVINCE_CODE;
                                }
                            }
                        }
                    }
                    FocusToAddress();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCommune_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.cboCommune.EditValue != null && !(this.cboCommune.EditValue.Equals(this.cboCommune.OldEditValue == null ? "" : this.cboCommune.OldEditValue)))
                {
                    this.SetValueHeinAddressByAddressOfPatient();
                }
                this.UpdateStateForAllControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("cboCommune_EditValueChanged:\n" + ex);
            }
        }

        private void cboCommune_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboCommune.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>()
                            .SingleOrDefault(o =>
                                o.COMMUNE_CODE == this.cboCommune.EditValue.ToString()
                                && (String.IsNullOrEmpty((this.cboDistrict.EditValue ?? "").ToString()) || o.DISTRICT_CODE == (this.cboDistrict.EditValue ?? "").ToString()));
                        if (commune != null)
                        {
                            this.txtCommuneCode.Text = commune.COMMUNE_CODE;
                            if (String.IsNullOrEmpty((this.cboProvince.EditValue ?? "").ToString()) && String.IsNullOrEmpty((this.cboDistrict.EditValue ?? "").ToString()))
                            {
                                this.cboDistrict.EditValue = commune.DISTRICT_CODE;
                                SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.ID == commune.DISTRICT_ID).FirstOrDefault();
                                if (district != null)
                                {
                                    this.cboProvince.EditValue = district.PROVINCE_CODE;
                                }
                            }
                            FocusToAddress();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangeReplaceAddress(string cmd, string lever)
        {
            try
            {
                string address = "";
                address = this.txtAddress.Text;
                if (!string.IsNullOrEmpty(address) && !string.IsNullOrEmpty(cmd))
                {
                    string[] addressSplit = address.Split(',');
                    var datas = addressSplit.Where(p => p.Contains(cmd)).ToList();
                    if (datas != null && datas.Count > 0)
                    {
                        if (datas.Count == 1)
                        {
                            string addressNew = "," + datas[0];
                            if (address.Contains(addressNew))
                            {
                                address = address.Replace(addressNew, "");
                            }
                            else
                            {
                                address = address.Replace(datas[0], "");
                            }
                        }
                        else
                        {
                            string addressV2 = lever + " " + cmd;
                            var data = datas.FirstOrDefault(p => p.Contains(addressV2));
                            if (data != null)
                            {
                                string addressNew = "," + data;
                                if (address.Contains(addressNew))
                                {
                                    address = address.Replace(addressNew, "");
                                }
                                else
                                {
                                    address = address.Replace(data, "");
                                }
                            }
                        }
                        this.txtAddress.Text = address;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Hàm gán dữ liệu địa chỉ thẻ
        /// </summary>
        private void SetValueHeinAddressByAddressOfPatient()
        {
            try
            {
                SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = null;
                SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = null;
                SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = null;
                if (cboProvince.EditValue != null)
                {
                    province = BackendDataWorker.Get<V_SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_CODE == (string)this.cboProvince.EditValue);
                    this.ChangeReplaceAddress(cboProvince.Text, "Tỉnh");
                }
                if (this.cboDistrict.EditValue != null)
                {
                    district = BackendDataWorker.Get<V_SDA_DISTRICT>().FirstOrDefault(o => o.DISTRICT_CODE == (string)this.cboDistrict.EditValue);
                    this.ChangeReplaceAddress(cboDistrict.Text, "Huyện");
                }

                if (this.cboCommune.EditValue != null)
                {
                    commune = BackendDataWorker.Get<V_SDA_COMMUNE>().FirstOrDefault(o => o.COMMUNE_CODE == (string)this.cboCommune.EditValue);
                    this.ChangeReplaceAddress(cboCommune.Text, "Xã");
                }

                //if (isReadCard)
                //    return;
                //if (isPatientBHYT)//Bo di de th bn cu van fill du lieu uc hein
                //    return;

                //string address = "";
                //address = txtAddress.Text;

                //string heinAddress = string.Format("{0}{1}{2}{3}", address, (commune != null ? " " + commune.INITIAL_NAME + " " + commune.COMMUNE_NAME : ""), (district != null ? ", " + district.INITIAL_NAME + " " + district.DISTRICT_NAME : ""), (province != null ? ", " + province.PROVINCE_NAME : ""));
                //this.dlgSetAddressUCHein(heinAddress);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void LoadTinhThanhCombo(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    this.cboCommune.Properties.DataSource = null;
                    this.cboCommune.EditValue = null;
                    this.txtCommuneCode.Text = "";
                    this.cboDistrict.Properties.DataSource = null;
                    this.cboDistrict.EditValue = null;
                    this.txtDistrictCode.Text = "";
                    this.cboProvince.EditValue = null;
                    this.FocusShowpopup(this.cboProvince, false);
                }
                else
                {
                    List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                    listResult = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.SEARCH_CODE.Contains(searchCode) || o.PROVINCE_CODE == searchCode).ToList();
                    if (listResult.Count == 1)
                    {
                        bool isReLoadRef = false;
                        if (listResult[0].PROVINCE_CODE != (this.cboProvince.EditValue ?? "").ToString())
                        {
                            isReLoadRef = true;
                        }
                        if (isReLoadRef)
                        {
                            this.cboProvince.EditValue = listResult[0].PROVINCE_CODE;
                            this.txtProvinceCode.Text = listResult[0].PROVINCE_CODE;
                            this.LoadHuyenCombo("", listResult[0].PROVINCE_CODE, false);
                        }
                        if (isExpand)
                        {
                            this.txtDistrictCode.Focus();
                            this.txtDistrictCode.SelectAll();
                        }
                    }
                    else
                    {
                        this.cboCommune.Properties.DataSource = null;
                        this.cboCommune.EditValue = null;
                        this.txtCommuneCode.Text = "";
                        this.cboDistrict.Properties.DataSource = null;
                        this.cboDistrict.EditValue = null;
                        this.txtDistrictCode.Text = "";
                        this.cboProvince.EditValue = null;
                        if (isExpand)
                        {
                            this.cboProvince.Properties.DataSource = listResult;
                            this.FocusShowpopup(this.cboProvince, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadHuyenCombo(string searchCode, string provinceCode, bool isExpand)
        {
            try
            {
                List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();
                listResult = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => (String.IsNullOrEmpty(searchCode) || (!String.IsNullOrEmpty(searchCode) && (o.SEARCH_CODE.ToUpper().Contains(searchCode.ToUpper()) || o.DISTRICT_CODE == searchCode))) && (provinceCode == "" || o.PROVINCE_CODE == provinceCode)).ToList();

                bool isReLoadRef = false;
                if (listResult[0].DISTRICT_CODE != (this.cboDistrict.EditValue ?? "").ToString())
                {
                    isReLoadRef = true;
                }

                if (!isReLoadRef)
                {
                    if (isExpand)
                    {
                        this.txtCommuneCode.Focus();
                        this.txtCommuneCode.SelectAll();
                    }
                    return;
                }

                this.InitComboCommon(this.cboDistrict, listResult, "DISTRICT_CODE", "RENDERER_DISTRICT_NAME", "SEARCH_CODE");

                if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(provinceCode) && listResult.Count > 0)
                {
                    this.cboCommune.Properties.DataSource = null;
                    this.cboCommune.EditValue = null;
                    this.txtCommuneCode.Text = "";
                    this.txtDistrictCode.Text = "";
                    this.cboDistrict.EditValue = null;
                    this.FocusShowpopup(this.cboDistrict, false);
                }
                else
                {
                    if (listResult.Count == 1)
                    {
                        this.cboDistrict.EditValue = listResult[0].DISTRICT_CODE;
                        this.txtDistrictCode.Text = listResult[0].DISTRICT_CODE;
                        if (String.IsNullOrEmpty(this.cboProvince.Text))
                        {
                            this.cboProvince.EditValue = listResult[0].PROVINCE_CODE;
                            this.txtProvinceCode.Text = listResult[0].PROVINCE_CODE;
                        }
                        this.LoadXaCombo("", listResult[0].DISTRICT_CODE, false);

                        if (isExpand)
                        {
                            this.txtCommuneCode.Focus();
                            this.txtCommuneCode.SelectAll();
                        }
                    }
                    else
                    {
                        this.cboCommune.Properties.DataSource = null;
                        this.cboCommune.EditValue = null;
                        this.txtCommuneCode.Text = "";
                        this.cboDistrict.EditValue = null;
                        if (isExpand)
                        {
                            this.FocusShowpopup(this.cboDistrict, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadXaCombo(string searchCode, string districtCode, bool isExpand)
        {
            try
            {
                List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE> listResult = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>()
                    .Where(o => (String.IsNullOrEmpty(searchCode) || (!String.IsNullOrEmpty(searchCode) && ((o.SEARCH_CODE ?? "").Contains(searchCode ?? "") || o.COMMUNE_CODE == searchCode)))
                        && (String.IsNullOrEmpty(districtCode) || o.DISTRICT_CODE == districtCode)).ToList();
                this.InitComboCommon(this.cboCommune, listResult, "COMMUNE_CODE", "RENDERER_COMMUNE_NAME", "SEARCH_CODE");
                if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(districtCode) && listResult.Count > 0)
                {
                    this.cboCommune.EditValue = null;
                    this.txtCommuneCode.Text = "";
                    this.FocusShowpopup(this.cboCommune, false);
                }
                else
                {
                    if (listResult.Count == 1)
                    {
                        this.cboCommune.EditValue = listResult[0].COMMUNE_CODE;
                        this.txtCommuneCode.Text = listResult[0].COMMUNE_CODE;
                        this.cboDistrict.EditValue = listResult[0].DISTRICT_CODE;
                        this.txtDistrictCode.Text = listResult[0].DISTRICT_CODE;
                        //if (this.dlgSetAddressUCProvinceOfBirth != null)
                        //    this.dlgSetAddressUCProvinceOfBirth(listResult[0], true);
                        if (String.IsNullOrEmpty(this.cboProvince.Text))
                        {
                            SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.ID == listResult[0].DISTRICT_ID).FirstOrDefault();
                            if (district != null)
                            {
                                this.cboProvince.EditValue = district.PROVINCE_CODE;
                                this.txtProvinceCode.Text = district.PROVINCE_CODE;
                            }
                        }

                        if (isExpand)
                        {
                            this.txtAddress.Focus();
                            this.txtAddress.SelectAll();
                        }
                    }
                    else if (isExpand)
                    {
                        this.cboCommune.EditValue = null;
                        this.FocusShowpopup(this.cboCommune, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtBarcode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.sample = new LIS_SAMPLE();
                    if (!String.IsNullOrWhiteSpace(txtBarcode.Text))
                    {
                        LisSampleFilter filter = new LisSampleFilter();
                        filter.IS_ACTIVE = IMSys.DbConfig.LIS_RS.COMMON.IS_ACTIVE__TRUE;
                        filter.BARCODE__EXACT = txtBarcode.Text;
                        var samples = new BackendAdapter(new CommonParam()).Get<List<LIS_SAMPLE>>("api/LisSample/Get", ApiConsumers.LisConsumer, filter, null);
                        this.sample = samples != null && samples.Count > 0 ? samples.FirstOrDefault() : null;
                        this.btnSave.Enabled = (this.sample != null);
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sample), sample));
                        this.FillDataBySampleData();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataBySampleData()
        {
            try
            {
                this.isInit = true;
                isNotLoadWhileChangeControlStateInFirst = true;
                this.cboTHX.Properties.Buttons[1].Visible = false;
                this.txtAddress.Text = "";
                this.txtMaTHX.Text = "";
                this.txtQRKBYT.Text = "";
                this.cboTHX.EditValue = null;

                bool isProvincePin = false, isDistrictPin = false, IsCommunePin = false, isSampleSender = false, isSpecimenOrder = false;

                this.cboGender.EditValue = null;
                dtPatientDob.EditValue = null;
                txtPatientDob.EditValue = null;
                this.isNotPatientDayDob = false;

                cboSampleType.EditValue = this.sample != null ? this.sample.SAMPLE_TYPE_ID : null;
                txtPatientName.Text = this.sample != null ? (string.Format("{0} {1}", this.sample.LAST_NAME, this.sample.FIRST_NAME)) : "";
                if (this.sample != null && this.sample.DOB > 0)
                {
                    if (this.sample.IS_HAS_NOT_DAY_DOB == 1)
                    {
                        dtPatientDob.Text = this.sample.DOB.ToString().Substring(0, 4);
                        txtPatientDob.Text = this.sample.DOB.ToString().Substring(0, 4);
                        DateTime dtNgSinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.sample.DOB ?? 0) ?? DateTime.MinValue;
                        dtPatientDob.DateTime = dtNgSinh;
                        this.isNotPatientDayDob = true;
                    }
                    else
                    {
                        LoadNgayThangNamSinhBNToForm(this.sample.DOB ?? 0, false);
                    }
                    //dtDOB.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientDto.DOB);
                }

                if (this.currentBySessionControlStateRDO != null && this.currentBySessionControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentBySessionControlStateRDO)
                    {
                        var csPin = this.currentControlStateRDO != null ? this.currentControlStateRDO.Exists(o => o.VALUE != null && o.VALUE == "1" && o.KEY == item.KEY) : false;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csPin), csPin)
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item));
                        if (csPin && !String.IsNullOrEmpty(item.VALUE))
                        {
                            if (item.KEY == ControlStateConstan.Province)
                            {
                                isProvincePin = true;
                            }
                            if (item.KEY == ControlStateConstan.District)
                            {
                                isDistrictPin = true;
                            }
                            if (item.KEY == ControlStateConstan.Commune)
                            {
                                IsCommunePin = true;
                            }
                            if (item.KEY == ControlStateConstan.SampleSender)
                            {
                                isSampleSender = true;
                            }
                            if (item.KEY == ControlStateConstan.SpecimenOrder)
                            {
                                isSpecimenOrder = true;
                            }
                        }
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug(
                    Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentControlStateRDO), currentControlStateRDO)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentBySessionControlStateRDO), currentBySessionControlStateRDO)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isProvincePin), isProvincePin)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isDistrictPin), isDistrictPin)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => IsCommunePin), IsCommunePin)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isSampleSender), isSampleSender)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isSpecimenOrder), isSpecimenOrder));



                MOS.EFMODEL.DataModels.HIS_GENDER gioitinh = (this.sample != null && !String.IsNullOrEmpty(this.sample.GENDER_CODE)) ? BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().SingleOrDefault(o => o.GENDER_CODE == this.sample.GENDER_CODE) : null;
                if (gioitinh != null)
                {
                    this.cboGender.EditValue = gioitinh.GENDER_CODE;
                }

                V_SDA_PROVINCE province = null;
                if ((this.sample != null && !String.IsNullOrEmpty(this.sample.PROVINCE_NAME)))
                {
                    this.txtProvinceCode.Text = "";
                    this.cboProvince.EditValue = null;
                    province = (this.sample != null && !String.IsNullOrEmpty(this.sample.PROVINCE_NAME)) ? BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_NAME == this.sample.PROVINCE_NAME) : null;
                    if (province != null)
                    {
                        this.txtProvinceCode.Text = province.PROVINCE_CODE;
                        this.cboProvince.EditValue = province.PROVINCE_CODE;
                    }
                }
                else if (!isProvincePin)
                {
                    this.txtProvinceCode.Text = "";
                    this.cboProvince.EditValue = null;
                }

                V_SDA_DISTRICT district = null;
                if ((this.sample != null && !String.IsNullOrEmpty(this.sample.DISTRICT_NAME)))
                {
                    this.txtDistrictCode.Text = "";
                    this.cboDistrict.EditValue = null;
                    district = (this.sample != null && !String.IsNullOrEmpty(this.sample.DISTRICT_NAME)) ? BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().FirstOrDefault(o => (o.INITIAL_NAME + " " + o.DISTRICT_NAME) == this.sample.DISTRICT_NAME && o.PROVINCE_NAME == this.sample.PROVINCE_NAME) : null;
                    if (district != null)
                    {
                        if (province == null)
                        {
                            province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_CODE == district.PROVINCE_CODE);
                            if (province != null)
                            {
                                txtProvinceCode.Text = province.PROVINCE_CODE;
                                cboProvince.EditValue = province.PROVINCE_CODE;
                            }
                        }

                        this.LoadHuyenCombo("", district.PROVINCE_CODE, false);

                        this.txtDistrictCode.Text = district.DISTRICT_CODE;
                        this.cboDistrict.EditValue = district.DISTRICT_CODE;
                    }
                }
                else if (!isDistrictPin)
                {
                    this.txtDistrictCode.Text = "";
                    this.cboDistrict.EditValue = null;
                }

                if ((this.sample != null && !String.IsNullOrEmpty(this.sample.COMMUNE_NAME)))
                {
                    this.txtCommuneCode.Text = "";
                    this.cboCommune.EditValue = null;

                    var commune = (this.sample != null && !String.IsNullOrEmpty(this.sample.COMMUNE_NAME)) ? BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().FirstOrDefault(o =>
                    (o.INITIAL_NAME + " " + o.COMMUNE_NAME) == this.sample.COMMUNE_NAME
                    && (o.DISTRICT_INITIAL_NAME + " " + o.DISTRICT_NAME) == this.sample.DISTRICT_NAME) : null;
                    if (commune != null)
                    {
                        if (district == null)
                        {
                            district = (this.sample != null && !String.IsNullOrEmpty(commune.DISTRICT_NAME)) ? BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().FirstOrDefault(o => (o.INITIAL_NAME + " " + o.DISTRICT_NAME) == commune.DISTRICT_NAME && o.DISTRICT_CODE == commune.DISTRICT_CODE) : null;
                            if (district != null)
                            {
                                if (province == null)
                                {
                                    province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_CODE == district.PROVINCE_CODE);
                                    if (province != null)
                                    {
                                        txtProvinceCode.Text = province.PROVINCE_CODE;
                                        cboProvince.EditValue = province.PROVINCE_CODE;
                                    }
                                }

                                this.LoadHuyenCombo("", district.PROVINCE_CODE, false);

                                this.txtDistrictCode.Text = district.DISTRICT_CODE;
                                this.cboDistrict.EditValue = district.DISTRICT_CODE;
                            }
                        }

                        this.LoadXaCombo("", commune.DISTRICT_CODE, false);
                        this.cboCommune.EditValue = commune.COMMUNE_CODE;
                        this.txtCommuneCode.Text = commune.COMMUNE_CODE;
                        this.cboTHX.EditValue = "C" + commune.ID;//ID_RAW
                        bool isSearchOrderByXHT = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS_DESKTOP_REGISTER__SEARCH_CODE__X/H/T") == "1" ? true : false;

                        this.txtMaTHX.Text = isSearchOrderByXHT ? String.Format("{0}{1}{2}", commune.SEARCH_CODE, district.SEARCH_CODE, province.SEARCH_CODE) : String.Format("{0}{1}{2}", province.SEARCH_CODE, district.SEARCH_CODE, commune.SEARCH_CODE);
                    }
                    else if (this.sample != null && this.sample.PROVINCE_CODE != null && this.sample.DISTRICT_CODE != null)
                    {
                        if (String.IsNullOrEmpty(this.sample.COMMUNE_CODE))
                        {
                            var dist = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().FirstOrDefault(o =>
                    (o.DISTRICT_CODE == this.sample.DISTRICT_CODE && o.PROVINCE_CODE == this.sample.PROVINCE_CODE));

                            if (dist != null)
                            {
                                var com = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>().FirstOrDefault(o =>
                        o.ID == -dist.ID);
                                if (com != null)
                                {
                                    this.cboTHX.EditValue = "C" + com.ID;
                                    this.txtMaTHX.Text = com.SEARCH_CODE_COMMUNE;
                                }
                            }
                        }
                    }
                    else if (this.sample != null && this.sample.COMMUNE_CODE != null && this.sample.DISTRICT_CODE != null)
                    {
                        var communeTHX = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>().FirstOrDefault(o =>
                        (o.SEARCH_CODE_COMMUNE) == (province.SEARCH_CODE + district.SEARCH_CODE)
                        && o.ID < 0);
                        if (communeTHX != null)
                        {
                            this.cboTHX.EditValue = communeTHX.ID_RAW;
                            this.txtMaTHX.Text = communeTHX.SEARCH_CODE_COMMUNE;
                        }
                    }
                }
                else if (IsCommunePin)
                {
                    var commune = cboCommune.EditValue != null ? BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().FirstOrDefault(o =>
                        o.COMMUNE_CODE == cboCommune.EditValue.ToString()
                    ) : null;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => IsCommunePin), IsCommunePin)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => commune), commune));
                    if (commune != null && district == null)
                    {
                        district = (!String.IsNullOrEmpty(commune.DISTRICT_CODE)) ? BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().FirstOrDefault(o => o.DISTRICT_CODE == commune.DISTRICT_CODE) : null;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => district), district));
                        if (district != null)
                        {
                            if (province == null)
                            {
                                province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_CODE == district.PROVINCE_CODE);
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => province), province));
                                if (province != null)
                                {
                                    txtProvinceCode.Text = province.PROVINCE_CODE;
                                    cboProvince.EditValue = province.PROVINCE_CODE;
                                }
                            }

                            this.LoadHuyenCombo("", district.PROVINCE_CODE, false);

                            this.txtDistrictCode.Text = district.DISTRICT_CODE;
                            this.cboDistrict.EditValue = district.DISTRICT_CODE;

                            this.LoadXaCombo("", district.DISTRICT_CODE, false);

                            this.cboCommune.EditValue = commune.COMMUNE_CODE;
                            this.txtCommuneCode.Text = commune.COMMUNE_CODE;
                        }
                    }
                }
                else if (!IsCommunePin)
                {
                    this.txtCommuneCode.Text = "";
                    this.cboCommune.EditValue = null;
                }

                txtAddress.Text = this.sample != null ? this.sample.ADDRESS : "";
                txtPhone.Text = this.sample != null ? this.sample.PHONE_NUMBER : "";
                txtQrSdt.Text = this.sample != null ? this.sample.PHONE_NUMBER : "";
                if (!String.IsNullOrWhiteSpace(this.sample.SAMPLE_SENDER_CODE) || !String.IsNullOrWhiteSpace(this.sample.SAMPLE_SENDER))
                {
                    if (!String.IsNullOrWhiteSpace(this.sample.SAMPLE_SENDER_CODE))
                    {
                        cboMediOrgCode.EditValue = this.sample.SAMPLE_SENDER_CODE;
                        txtMediOrgCode.Text = this.sample.SAMPLE_SENDER_CODE;
                    }
                    else if (!String.IsNullOrWhiteSpace(this.sample.SAMPLE_SENDER))
                    {
                        cboMediOrgCode.EditValue = null;
                        txtMediOrgCode.Text = "";
                        txtSampleSender.Text = this.sample != null ? this.sample.SAMPLE_SENDER : "";
                    }
                    else if (recentMediOrg != null)
                    {
                        cboMediOrgCode.EditValue = recentMediOrg.MEDI_ORG_CODE;
                        txtMediOrgCode.Text = recentMediOrg.MEDI_ORG_CODE;
                    }
                    else
                    {
                        cboMediOrgCode.EditValue = null;
                        txtSampleSender.Text = "";
                        txtMediOrgCode.Text = "";
                    }
                }
                else if (!isSampleSender)
                {
                    cboMediOrgCode.EditValue = null;
                    txtSampleSender.Text = "";
                    txtMediOrgCode.Text = "";
                }
                //else if (recentMediOrg != null)
                //{
                //    cboMediOrgCode.EditValue = recentMediOrg.MEDI_ORG_CODE;
                //    txtMediOrgCode.Text = recentMediOrg.MEDI_ORG_CODE;
                //    txtSampleSender.Text = "";
                //}
                dtSickTime.EditValue = this.sample != null ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((long)(this.sample.SICK_TIME ?? 0)) : null;
                dtSampleTime.EditValue = this.sample != null ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.sample.SAMPLE_TIME ?? 0) : null;

                dtTGNhanMau.EditValue = this.sample != null ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.sample.RESULT_TIME ?? 0) : null;
                txtNote.Text = this.sample != null ? this.sample.NOTE : "";
                if ((this.sample != null && this.sample.SPECIMEN_ORDER != null))
                {
                    txtSPECIMEN_ORDER.Text = (this.sample != null && this.sample.SPECIMEN_ORDER != null) ? this.sample.SPECIMEN_ORDER.Value.ToString() : "";
                }
                else if (!isSpecimenOrder)
                {
                    txtSPECIMEN_ORDER.Text = "";
                }

                this.isInit = false;
                this.isNotLoadWhileChangeControlStateInFirst = false;
                this.UpdateStateForAllControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadNgayThangNamSinhBNToForm(long dob, bool hasNotDayDob)
        {
            try
            {
                this.isNotPatientDayDob = hasNotDayDob;
                LogSystem.Debug("Bat dau gan du lieu nam sinh benh nhan len form. p1: tinh toan nam sinh");
                if (dob > 0)
                {
                    DateTime dtNgSinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dob) ?? DateTime.MinValue;
                    this.dtPatientDob.EditValue = dtNgSinh;
                    if (hasNotDayDob)
                    {
                        this.txtPatientDob.Text = dtNgSinh.ToString("yyyy");
                    }
                    else
                    {
                        this.txtPatientDob.Text = dtNgSinh.ToString("dd/MM/yyyy");
                    }

                    //CalculatePatientAge.AgeObject ageObject = CalculatePatientAge.Calculate(dob);
                    //if (ageObject != null)
                    //{
                    //    this.txtAge.EditValue = ageObject.OutDate;
                    //    this.cboAge.EditValue = ageObject.AgeType;
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Validate

        private void ValidateProvince()
        {
            Valid_Province_Control validateProvince = new Valid_Province_Control();
            validateProvince.cboProvince = this.cboProvince;
            validateProvince.txtProvince = this.txtProvinceCode;
            validateProvince.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
            validateProvince.ErrorType = ErrorType.Warning;
            this.dxValidationProviderControl.SetValidationRule(txtProvinceCode, validateProvince);
        }

        private void ValidateDistrict()
        {
            Valid_District_Control validateProvince = new Valid_District_Control();
            validateProvince.cboDistrict = this.cboDistrict;
            validateProvince.txtDistrict = this.txtDistrictCode;
            validateProvince.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
            validateProvince.ErrorType = ErrorType.Warning;
            this.dxValidationProviderControl.SetValidationRule(txtDistrictCode, validateProvince);
        }

        private void ValidateCommune()
        {
            Valid_Commune_Control validateProvince = new Valid_Commune_Control();
            validateProvince.cboCommune = this.cboCommune;
            validateProvince.txtCommune = this.txtCommuneCode;
            validateProvince.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
            validateProvince.ErrorType = ErrorType.Warning;
            this.dxValidationProviderControl.SetValidationRule(txtCommuneCode, validateProvince);
        }

        private void ValidateMaxlengthTextControl(DevExpress.XtraEditors.TextEdit txtEdit, int maxlength)
        {
            TextEditMaxLengthValidationRule _rule = new TextEditMaxLengthValidationRule();
            _rule.txtEdit = txtEdit;
            _rule.maxlength = maxlength;
            _rule.isVali = false;
            _rule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
            _rule.ErrorType = ErrorType.Warning;
            this.dxValidationProviderControl.SetValidationRule(txtEdit, _rule);
        }

        #endregion

        #region Init Combo

        private void FocusShowpopup(LookUpEdit cboEditor, bool isSelectFirstRow)
        {
            try
            {
                cboEditor.Focus();
                cboEditor.ShowPopup();
                if (isSelectFirstRow)
                    PopupLoader.SelectFirstRowPopup(cboEditor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, string displayMemberCode)
        {
            try
            {
                InitComboCommonUtil(cboEditor, data, valueMember, displayMember, 0, displayMemberCode, 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCommonUtil(Control cboEditor, object data, string valueMember, string displayMember, int displayMemberWidth, string displayMemberCode, int displayMemberCodeWidth)
        {
            try
            {
                int popupWidth = 0;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                if (!String.IsNullOrEmpty(displayMemberCode))
                {
                    columnInfos.Add(new ColumnInfo(displayMemberCode, "", (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100), 1, true));
                    popupWidth += (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100);
                }
                if (!String.IsNullOrEmpty(displayMember))
                {
                    columnInfos.Add(new ColumnInfo(displayMember, "", (displayMemberWidth > 0 ? displayMemberWidth : 250), 2, true));
                    popupWidth += (displayMemberWidth > 0 ? displayMemberWidth : 250);
                }
                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, popupWidth);
                ControlEditorLoader.Load(cboEditor, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        #endregion

        #region Inside Focus Control

        private void FocusToAddress()
        {
            try
            {
                this.txtAddress.Focus();
                this.txtAddress.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FocusToProvince()
        {
            try
            {
                this.txtProvinceCode.Focus();
                this.txtProvinceCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusToDistrict()
        {
            try
            {
                this.txtDistrictCode.Focus();
                this.txtDistrictCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusToCommune()
        {
            try
            {
                this.txtCommuneCode.Focus();
                this.txtCommuneCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void cboSampleType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboSampleType.EditValue != null)
                    {
                        txtQrSdt.Focus();
                        txtQrSdt.SelectAll();
                    }
                }
                else if (e.KeyCode == Keys.Tab)
                {
                    txtQrSdt.Focus();
                    txtQrSdt.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSampleType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboSampleType.EditValue != null)
                    {
                        txtQrSdt.Focus();
                        txtQrSdt.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboGender.Focus();
                    cboGender.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboGender_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboGender.EditValue != null)
                    {
                        txtPatientDob.Focus();
                        txtPatientDob.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void txtPatientDob_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.isDobTextEditKeyEnter = true;

                    DateUtil.DateValidObject dateValidObject = DateUtil.ValidPatientDob(this.txtPatientDob.Text);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dateValidObject), dateValidObject));
                    if (dateValidObject.Age > 0 && String.IsNullOrEmpty(dateValidObject.Message))
                    {
                        //this.txtAge.Text = this.txtPatientDob.Text;
                        //this.cboAge.EditValue = 1;
                        this.txtPatientDob.Text = dateValidObject.Age.ToString();
                    }
                    else if (String.IsNullOrEmpty(dateValidObject.Message))
                    {
                        if (!dateValidObject.HasNotDayDob)
                        {
                            this.txtPatientDob.Text = dateValidObject.OutDate;
                            this.dtPatientDob.EditValue = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(dateValidObject.OutDate);
                            this.dtPatientDob.Update();
                        }
                    }

                    txtMaTHX.Focus();
                    txtMaTHX.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAddress_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPhone.Focus();
                    txtPhone.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPhone_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMediOrgCode.Focus();
                    txtMediOrgCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSampleSender_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("txtSampleSender_PreviewKeyDown.1");
                if (e.KeyCode == Keys.Enter)
                {
                    dtSickTime.Focus();
                    dtSickTime.SelectAll();
                }
                Inventec.Common.Logging.LogSystem.Debug("txtSampleSender_PreviewKeyDown.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtSickTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("dtSickTime_PreviewKeyDown.1");
                if (e.KeyCode == Keys.Enter)
                {
                    dtSampleTime.Focus();
                    dtSampleTime.SelectAll();
                }
                Inventec.Common.Logging.LogSystem.Debug("dtSickTime_PreviewKeyDown.1");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtSampleTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter && btnSave.Enabled)
                {
                    btnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediOrgCode_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("cboMediOrgCode_Closed.1");
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => e.CloseMode), e.CloseMode));
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMediOrgCode.EditValue != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("cboMediOrgCode_Closed.1.1");
                        dtSickTime.Focus();
                        dtSickTime.SelectAll();
                        Inventec.Common.Logging.LogSystem.Debug("cboMediOrgCode_Closed.1.2");
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("cboMediOrgCode_Closed.1.3");
                        txtSampleSender.Focus();
                        txtSampleSender.SelectAll();
                        Inventec.Common.Logging.LogSystem.Debug("cboMediOrgCode_Closed.1.4");
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("cboMediOrgCode_Closed.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediOrgCode_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("cboMediOrgCode_EditValueChanged.1");
                if (cboMediOrgCode.EditValue != null)
                {
                    HIS_MEDI_ORG org = this.listMediOrg != null ? this.listMediOrg.FirstOrDefault(o => o.MEDI_ORG_CODE == cboMediOrgCode.EditValue.ToString()) : null;
                    if (org != null)
                    {
                        txtMediOrgCode.Text = org.MEDI_ORG_CODE ?? "";
                        txtSampleSender.Text = "";
                        txtSampleSender.Enabled = false;
                        if (!this.isInit)
                        {
                            recentMediOrg = org;
                        }
                    }
                }
                else
                {
                    if (!this.isInit)
                    {
                        recentMediOrg = null;
                    }
                    txtMediOrgCode.Text = "";
                    txtSampleSender.Enabled = true;
                }
                this.UpdateStateForAllControl();
                Inventec.Common.Logging.LogSystem.Debug("cboMediOrgCode_EditValueChanged.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediOrgCode_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMediOrgCode.EditValue = null;
                }
                else if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    ProcessOpenModuleMediOrg();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessOpenModuleMediOrg()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisMediOrg").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.HisMediOrg'");
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.HisMediOrg' is not plugins");
                List<object> listArgs = new List<object>();
                listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId));
                var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId), listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                ((Form)extenceInstance).ShowDialog();
                WaitingManager.Show();
                this.LoadHisMediOrg();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMediOrgCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtMediOrgCode.Text))
                    {
                        string text = txtMediOrgCode.Text.Trim();
                        List<HIS_MEDI_ORG> lstData = this.listMediOrg != null ? this.listMediOrg.Where(o => o.MEDI_ORG_CODE.Contains(text)).ToList() : null;
                        if (lstData != null && lstData.Count == 1)
                        {
                            cboMediOrgCode.EditValue = lstData[0].MEDI_ORG_CODE;
                            dtSickTime.Focus();
                            dtSickTime.SelectAll();
                        }
                    }
                    else
                    {
                        cboMediOrgCode.Focus();
                        cboMediOrgCode.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediOrgCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("cboMediOrgCode_KeyUp.1");
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboMediOrgCode.EditValue != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("cboMediOrgCode_KeyUp.1.1");
                        dtSickTime.Focus();
                        dtSickTime.SelectAll();
                        Inventec.Common.Logging.LogSystem.Debug("cboMediOrgCode_KeyUp.1.2");
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("cboMediOrgCode_KeyUp.2.1");
                        txtSampleSender.Focus();
                        txtSampleSender.SelectAll();
                        Inventec.Common.Logging.LogSystem.Debug("cboMediOrgCode_KeyUp.2.1");
                    }
                }
                else if (e.KeyCode != Keys.Tab)
                {
                    Inventec.Common.Logging.LogSystem.Debug("cboMediOrgCode_KeyUp.2.3");
                    cboMediOrgCode.ShowPopup();
                }
                Inventec.Common.Logging.LogSystem.Debug("cboMediOrgCode_KeyUp.3");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientDob_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.txtPatientDob.Text)) return;

                string dob = "";
                if (this.txtPatientDob.Text.Contains("/"))
                    dob = PatientDobUtil.PatientDobToDobRaw(this.txtPatientDob.Text);

                if (!String.IsNullOrEmpty(dob))
                {
                    this.txtPatientDob.Text = dob;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientDob_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(this.txtPatientDob.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtPatientDob.EditValue = dt;
                        this.dtPatientDob.Update();
                    }
                    this.dtPatientDob.Visible = true;
                    this.dtPatientDob.ShowPopup();
                    this.dtPatientDob.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientDob_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                DateUtil.DateValidObject dateValidObject = DateUtil.ValidPatientDob(this.txtPatientDob.Text);
                if (dateValidObject != null)
                {
                    e.ErrorText = dateValidObject.Message;
                }

                AutoValidate = AutoValidate.EnableAllowFocusChange;
                e.ExceptionMode = ExceptionMode.DisplayError;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientDob_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '/'))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtPatientDob_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    this.dtPatientDob.Visible = false;

                    this.txtPatientDob.Text = dtPatientDob.DateTime.ToString("dd/MM/yyyy");

                    txtMaTHX.Focus();
                    txtMaTHX.SelectAll();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtPatientDob_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.dtPatientDob.Visible = true;
                    this.dtPatientDob.Update();
                    this.txtPatientDob.Text = this.dtPatientDob.DateTime.ToString("dd/MM/yyyy");

                    txtMaTHX.Focus();
                    txtMaTHX.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientDob_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.txtPatientDob.Text.Trim()))
                    return;
                DateUtil.DateValidObject dateValidObject = DateUtil.ValidPatientDob(this.txtPatientDob.Text);
                if (dateValidObject.Age > 0 && String.IsNullOrEmpty(dateValidObject.Message))
                {
                    //this.txtAge.Text = this.txtPatientDob.Text;
                    //this.cboAge.EditValue = 1;
                    this.txtPatientDob.Text = dateValidObject.Age.ToString();
                }
                else if (String.IsNullOrEmpty(dateValidObject.Message))
                {
                    if (!dateValidObject.HasNotDayDob)
                    {
                        this.txtPatientDob.Text = dateValidObject.OutDate;
                        this.dtPatientDob.EditValue = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(dateValidObject.OutDate);
                        this.dtPatientDob.Update();
                    }
                }
                else
                {
                    e.Cancel = true;
                    return;
                }

                this.isNotPatientDayDob = dateValidObject.HasNotDayDob;
                if (
                    ((this.txtPatientDob.EditValue ?? "").ToString() != (this.txtPatientDob.OldEditValue ?? "").ToString())
                    && (!String.IsNullOrEmpty(dateValidObject.OutDate))
                    )
                {
                    this.dxValidationProviderControl.RemoveControlError(this.txtPatientDob);
                    this.txtPatientDob.ErrorText = "";
                }
                if (this.isDobTextEditKeyEnter)
                {
                    txtMaTHX.Focus();
                    txtMaTHX.SelectAll();
                }

                this.isDobTextEditKeyEnter = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientDOB_Leave(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(dtPatientDob.Text))
                {
                    dtPatientDob.Text = PatientDobUtil.PatientDobToDobRaw(dtPatientDob.Text);
                }

                if (!String.IsNullOrEmpty(dtPatientDob.Text))
                {
                    string strDob = "";

                    if (dtPatientDob.Text.Length == 2 || dtPatientDob.Text.Length == 1)
                    {
                        int patientDob = Int32.Parse(dtPatientDob.Text);
                        if (patientDob < 7)
                        {
                            MessageBox.Show(ResourceMessage.NgaySinhKhongDuocNhoHon7);
                            dtPatientDob.Focus();
                            dtPatientDob.SelectAll();
                            return;
                        }
                        else
                        {
                            dtPatientDob.Text = (DateTime.Now.Year - patientDob).ToString();
                        }
                    }
                    else if (dtPatientDob.Text.Length == 4)
                    {
                        if (Inventec.Common.TypeConvert.Parse.ToInt64(dtPatientDob.Text) <= DateTime.Now.Year)
                        {
                            strDob = "01/01/" + dtPatientDob.Text;
                            isNotPatientDayDob = true;
                        }
                        else
                        {
                            MessageBox.Show(ResourceMessage.NhapNgaySinhKhongDungDinhDang);
                            dtPatientDob.Focus();
                            dtPatientDob.SelectAll();
                            return;
                        }

                    }
                    else if (dtPatientDob.Text.Length == 8)
                    {
                        strDob = dtPatientDob.Text.Substring(0, 2) + "/" + dtPatientDob.Text.Substring(2, 2) + "/" + dtPatientDob.Text.Substring(4, 4);
                        if (HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(strDob).Value.Date <= DateTime.Now.Date)
                        {
                            strDob = dtPatientDob.Text.Substring(0, 2) + "/" + dtPatientDob.Text.Substring(2, 2) + "/" + dtPatientDob.Text.Substring(4, 4);
                            dtPatientDob.Text = strDob;
                            isNotPatientDayDob = false;
                        }
                        else
                        {
                            MessageBox.Show(ResourceMessage.ThongTinNgaySinhPhaiNhoHonNgayHienTai);
                            dtPatientDob.Focus();
                            dtPatientDob.SelectAll();
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show(ResourceMessage.NhapNgaySinhKhongDungDinhDang);
                        dtPatientDob.Focus();
                        dtPatientDob.SelectAll();
                        return;
                    }


                    if (String.IsNullOrWhiteSpace(strDob))
                    {
                        strDob = dtPatientDob.Text;
                    }
                }
                else
                {
                    dtPatientDob.Focus();
                    dtPatientDob.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtSPECIMEN_ORDER_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => e.KeyChar), e.KeyChar)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => txtSPECIMEN_ORDER.Text), txtSPECIMEN_ORDER.Text));
                if (!char.IsControl(e.KeyChar) && ((e.KeyChar == '0' && String.IsNullOrEmpty(txtSPECIMEN_ORDER.Text)) || !char.IsDigit(e.KeyChar)))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProvince_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDistrict_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCommune_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSPECIMEN_ORDER_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnStateForcboProvince_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Glyph)
                {
                    if (e.Button.Index == 0)
                    {
                        btnStateForcboProvince.Properties.Buttons[0].Visible = false;
                        btnStateForcboProvince.Properties.Buttons[1].Visible = true;
                    }
                    else if (e.Button.Index == 1)
                    {
                        btnStateForcboProvince.Properties.Buttons[1].Visible = false;
                        btnStateForcboProvince.Properties.Buttons[0].Visible = true;
                    }
                    btnStateForcboProvince.Update();
                    HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.Province && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                    if (csAddOrUpdate != null)
                    {
                        csAddOrUpdate.VALUE = (btnStateForcboProvince.Properties.Buttons[0].Visible ? "1" : "");
                    }
                    else
                    {
                        csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                        csAddOrUpdate.KEY = ControlStateConstan.Province;
                        csAddOrUpdate.VALUE = (btnStateForcboProvince.Properties.Buttons[0].Visible ? "1" : "");
                        csAddOrUpdate.MODULE_LINK = moduleLink;
                        if (this.currentControlStateRDO == null)
                            this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                        this.currentControlStateRDO.Add(csAddOrUpdate);
                    }
                    this.controlStateWorker.SetData(this.currentControlStateRDO);

                    HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateValue = (this.currentBySessionControlStateRDO != null && this.currentBySessionControlStateRDO.Count > 0) ? this.currentBySessionControlStateRDO.Where(o => o.KEY == ControlStateConstan.Province && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                    if (csAddOrUpdateValue != null)
                    {
                        csAddOrUpdateValue.VALUE = (btnStateForcboProvince.Properties.Buttons[0].Visible ? (cboProvince.EditValue ?? "").ToString() : "");
                    }
                    else
                    {
                        csAddOrUpdateValue = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                        csAddOrUpdateValue.KEY = ControlStateConstan.Province;
                        csAddOrUpdateValue.VALUE = (btnStateForcboProvince.Properties.Buttons[0].Visible ? (cboProvince.EditValue ?? "").ToString() : "");
                        csAddOrUpdateValue.MODULE_LINK = moduleLink;
                        if (this.currentBySessionControlStateRDO == null)
                            this.currentBySessionControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                        this.currentBySessionControlStateRDO.Add(csAddOrUpdateValue);
                    }
                    this.controlStateWorker.SetDataBySession(this.currentBySessionControlStateRDO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnStateForcboDistrict_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Glyph)
                {
                    if (e.Button.Index == 0)
                    {
                        btnStateForcboDistrict.Properties.Buttons[0].Visible = false;
                        btnStateForcboDistrict.Properties.Buttons[1].Visible = true;
                    }
                    else if (e.Button.Index == 1)
                    {
                        btnStateForcboDistrict.Properties.Buttons[1].Visible = false;
                        btnStateForcboDistrict.Properties.Buttons[0].Visible = true;
                    }
                    btnStateForcboDistrict.Update();
                    HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.District && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                    if (csAddOrUpdate != null)
                    {
                        csAddOrUpdate.VALUE = (btnStateForcboDistrict.Properties.Buttons[0].Visible ? "1" : "");
                    }
                    else
                    {
                        csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                        csAddOrUpdate.KEY = ControlStateConstan.District;
                        csAddOrUpdate.VALUE = (btnStateForcboDistrict.Properties.Buttons[0].Visible ? "1" : "");
                        csAddOrUpdate.MODULE_LINK = moduleLink;
                        if (this.currentControlStateRDO == null)
                            this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                        this.currentControlStateRDO.Add(csAddOrUpdate);
                    }
                    this.controlStateWorker.SetData(this.currentControlStateRDO);

                    HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateValue = (this.currentBySessionControlStateRDO != null && this.currentBySessionControlStateRDO.Count > 0) ? this.currentBySessionControlStateRDO.Where(o => o.KEY == ControlStateConstan.District && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                    if (csAddOrUpdateValue != null)
                    {
                        csAddOrUpdateValue.VALUE = (btnStateForcboDistrict.Properties.Buttons[0].Visible ? (cboDistrict.EditValue ?? "").ToString() : "");
                    }
                    else
                    {
                        csAddOrUpdateValue = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                        csAddOrUpdateValue.KEY = ControlStateConstan.District;
                        csAddOrUpdateValue.VALUE = (btnStateForcboDistrict.Properties.Buttons[0].Visible ? (cboDistrict.EditValue ?? "").ToString() : "");
                        csAddOrUpdateValue.MODULE_LINK = moduleLink;
                        if (this.currentBySessionControlStateRDO == null)
                            this.currentBySessionControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                        this.currentBySessionControlStateRDO.Add(csAddOrUpdateValue);
                    }
                    this.controlStateWorker.SetDataBySession(this.currentBySessionControlStateRDO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnStateForcboCommune_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Glyph)
                {
                    if (e.Button.Index == 0)
                    {
                        btnStateForcboCommune.Properties.Buttons[0].Visible = false;
                        btnStateForcboCommune.Properties.Buttons[1].Visible = true;
                    }
                    else if (e.Button.Index == 1)
                    {
                        btnStateForcboCommune.Properties.Buttons[1].Visible = false;
                        btnStateForcboCommune.Properties.Buttons[0].Visible = true;
                    }
                    btnStateForcboCommune.Update();
                    HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.Commune && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                    if (csAddOrUpdate != null)
                    {
                        csAddOrUpdate.VALUE = (btnStateForcboCommune.Properties.Buttons[0].Visible ? "1" : "");
                    }
                    else
                    {
                        csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                        csAddOrUpdate.KEY = ControlStateConstan.Commune;
                        csAddOrUpdate.VALUE = (btnStateForcboCommune.Properties.Buttons[0].Visible ? "1" : "");
                        csAddOrUpdate.MODULE_LINK = moduleLink;
                        if (this.currentControlStateRDO == null)
                            this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                        this.currentControlStateRDO.Add(csAddOrUpdate);
                    }
                    this.controlStateWorker.SetData(this.currentControlStateRDO);

                    HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateValue = (this.currentBySessionControlStateRDO != null && this.currentBySessionControlStateRDO.Count > 0) ? this.currentBySessionControlStateRDO.Where(o => o.KEY == ControlStateConstan.Commune && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                    if (csAddOrUpdateValue != null)
                    {
                        csAddOrUpdateValue.VALUE = (btnStateForcboCommune.Properties.Buttons[0].Visible ? (cboCommune.EditValue ?? "").ToString() : "");
                    }
                    else
                    {
                        csAddOrUpdateValue = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                        csAddOrUpdateValue.KEY = ControlStateConstan.Commune;
                        csAddOrUpdateValue.VALUE = (btnStateForcboCommune.Properties.Buttons[0].Visible ? (cboCommune.EditValue ?? "").ToString() : "");
                        csAddOrUpdateValue.MODULE_LINK = moduleLink;
                        if (this.currentBySessionControlStateRDO == null)
                            this.currentBySessionControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                        this.currentBySessionControlStateRDO.Add(csAddOrUpdateValue);
                    }
                    this.controlStateWorker.SetDataBySession(this.currentBySessionControlStateRDO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnStateForcboMediOrgCode_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Glyph)
                {
                    if (e.Button.Index == 0)
                    {
                        btnStateForcboMediOrgCode.Properties.Buttons[0].Visible = false;
                        btnStateForcboMediOrgCode.Properties.Buttons[1].Visible = true;
                    }
                    else if (e.Button.Index == 1)
                    {
                        btnStateForcboMediOrgCode.Properties.Buttons[1].Visible = false;
                        btnStateForcboMediOrgCode.Properties.Buttons[0].Visible = true;
                    }
                    btnStateForcboMediOrgCode.Update();
                    HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.SampleSender && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                    if (csAddOrUpdate != null)
                    {
                        csAddOrUpdate.VALUE = (btnStateForcboMediOrgCode.Properties.Buttons[0].Visible ? "1" : "");
                    }
                    else
                    {
                        csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                        csAddOrUpdate.KEY = ControlStateConstan.SampleSender;
                        csAddOrUpdate.VALUE = (btnStateForcboMediOrgCode.Properties.Buttons[0].Visible ? "1" : "");
                        csAddOrUpdate.MODULE_LINK = moduleLink;
                        if (this.currentControlStateRDO == null)
                            this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                        this.currentControlStateRDO.Add(csAddOrUpdate);
                    }
                    this.controlStateWorker.SetData(this.currentControlStateRDO);

                    HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateValue = (this.currentBySessionControlStateRDO != null && this.currentBySessionControlStateRDO.Count > 0) ? this.currentBySessionControlStateRDO.Where(o => o.KEY == ControlStateConstan.SampleSender && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                    if (csAddOrUpdateValue != null)
                    {
                        csAddOrUpdateValue.VALUE = (btnStateForcboMediOrgCode.Properties.Buttons[0].Visible ? ((cboMediOrgCode.EditValue ?? "").ToString() + "|" + txtSampleSender.Text) : "");
                    }
                    else
                    {
                        csAddOrUpdateValue = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                        csAddOrUpdateValue.KEY = ControlStateConstan.SampleSender;
                        csAddOrUpdateValue.VALUE = (btnStateForcboMediOrgCode.Properties.Buttons[0].Visible ? ((cboMediOrgCode.EditValue ?? "").ToString() + "|" + txtSampleSender.Text) : "");
                        csAddOrUpdateValue.MODULE_LINK = moduleLink;
                        if (this.currentBySessionControlStateRDO == null)
                            this.currentBySessionControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                        this.currentBySessionControlStateRDO.Add(csAddOrUpdateValue);
                    }
                    this.controlStateWorker.SetDataBySession(this.currentBySessionControlStateRDO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnStateFortxtSPECIMEN_ORDER_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Glyph)
                {
                    if (e.Button.Index == 0)
                    {
                        btnStateFortxtSPECIMEN_ORDER.Properties.Buttons[0].Visible = false;
                        btnStateFortxtSPECIMEN_ORDER.Properties.Buttons[1].Visible = true;
                    }
                    else if (e.Button.Index == 1)
                    {
                        btnStateFortxtSPECIMEN_ORDER.Properties.Buttons[1].Visible = false;
                        btnStateFortxtSPECIMEN_ORDER.Properties.Buttons[0].Visible = true;
                    }
                    btnStateFortxtSPECIMEN_ORDER.Update();

                    HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.SpecimenOrder && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                    if (csAddOrUpdate != null)
                    {
                        csAddOrUpdate.VALUE = (btnStateFortxtSPECIMEN_ORDER.Properties.Buttons[0].Visible ? "1" : "");
                    }
                    else
                    {
                        csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                        csAddOrUpdate.KEY = ControlStateConstan.SpecimenOrder;
                        csAddOrUpdate.VALUE = (btnStateFortxtSPECIMEN_ORDER.Properties.Buttons[0].Visible ? "1" : "");
                        csAddOrUpdate.MODULE_LINK = moduleLink;
                        if (this.currentControlStateRDO == null)
                            this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                        this.currentControlStateRDO.Add(csAddOrUpdate);
                    }
                    this.controlStateWorker.SetData(this.currentControlStateRDO);

                    HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateValue = (this.currentBySessionControlStateRDO != null && this.currentBySessionControlStateRDO.Count > 0) ? this.currentBySessionControlStateRDO.Where(o => o.KEY == ControlStateConstan.SpecimenOrder && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                    if (csAddOrUpdateValue != null)
                    {
                        csAddOrUpdateValue.VALUE = (btnStateFortxtSPECIMEN_ORDER.Properties.Buttons[0].Visible ? txtSPECIMEN_ORDER.Text : "");
                    }
                    else
                    {
                        csAddOrUpdateValue = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                        csAddOrUpdateValue.KEY = ControlStateConstan.SpecimenOrder;
                        csAddOrUpdateValue.VALUE = (btnStateFortxtSPECIMEN_ORDER.Properties.Buttons[0].Visible ? txtSPECIMEN_ORDER.Text : "");
                        csAddOrUpdateValue.MODULE_LINK = moduleLink;
                        if (this.currentControlStateRDO == null)
                            this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                        this.currentBySessionControlStateRDO.Add(csAddOrUpdateValue);
                    }
                    this.controlStateWorker.SetDataBySession(this.currentBySessionControlStateRDO);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmUpdateLisSample_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                //UpdateStateForAllControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateStateForAllControl()
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isNotLoadWhileChangeControlStateInFirst), isNotLoadWhileChangeControlStateInFirst));
                    return;
                }

                var iscsPin = this.currentControlStateRDO.Exists(o => o.VALUE == "1");
                if (!iscsPin)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong co contorl nao duoc check PIN____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => iscsPin), iscsPin));
                    return;
                }

                if (this.currentBySessionControlStateRDO == null)
                    this.currentBySessionControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateValueForcboProvince = (this.currentBySessionControlStateRDO != null && this.currentBySessionControlStateRDO.Count > 0) ? this.currentBySessionControlStateRDO.Where(o => o.KEY == ControlStateConstan.Province && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdateValueForcboProvince != null)
                {
                    csAddOrUpdateValueForcboProvince.VALUE = (btnStateForcboProvince.Properties.Buttons[0].Visible ? (cboProvince.EditValue ?? "").ToString() : "");
                }
                else
                {
                    csAddOrUpdateValueForcboProvince = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdateValueForcboProvince.KEY = ControlStateConstan.Province;
                    csAddOrUpdateValueForcboProvince.VALUE = (btnStateForcboProvince.Properties.Buttons[0].Visible ? (cboProvince.EditValue ?? "").ToString() : "");
                    csAddOrUpdateValueForcboProvince.MODULE_LINK = moduleLink;
                    this.currentBySessionControlStateRDO.Add(csAddOrUpdateValueForcboProvince);
                }

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateValueForcboDistrict = (this.currentBySessionControlStateRDO != null && this.currentBySessionControlStateRDO.Count > 0) ? this.currentBySessionControlStateRDO.Where(o => o.KEY == ControlStateConstan.District && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdateValueForcboDistrict != null)
                {
                    csAddOrUpdateValueForcboDistrict.VALUE = (btnStateForcboDistrict.Properties.Buttons[0].Visible ? (cboDistrict.EditValue ?? "").ToString() : "");
                }
                else
                {
                    csAddOrUpdateValueForcboDistrict = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdateValueForcboDistrict.KEY = ControlStateConstan.District;
                    csAddOrUpdateValueForcboDistrict.VALUE = (btnStateForcboDistrict.Properties.Buttons[0].Visible ? (cboDistrict.EditValue ?? "").ToString() : "");
                    csAddOrUpdateValueForcboDistrict.MODULE_LINK = moduleLink;
                    this.currentBySessionControlStateRDO.Add(csAddOrUpdateValueForcboDistrict);
                }

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateValueForcboCommune = (this.currentBySessionControlStateRDO != null && this.currentBySessionControlStateRDO.Count > 0) ? this.currentBySessionControlStateRDO.Where(o => o.KEY == ControlStateConstan.Commune && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdateValueForcboCommune != null)
                {
                    csAddOrUpdateValueForcboCommune.VALUE = (btnStateForcboCommune.Properties.Buttons[0].Visible ? (cboCommune.EditValue ?? "").ToString() : "");
                }
                else
                {
                    csAddOrUpdateValueForcboCommune = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdateValueForcboCommune.KEY = ControlStateConstan.Commune;
                    csAddOrUpdateValueForcboCommune.VALUE = (btnStateForcboCommune.Properties.Buttons[0].Visible ? (cboCommune.EditValue ?? "").ToString() : "");
                    csAddOrUpdateValueForcboCommune.MODULE_LINK = moduleLink;
                    this.currentBySessionControlStateRDO.Add(csAddOrUpdateValueForcboCommune);
                }

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateValueForcboMediOrgCode = (this.currentBySessionControlStateRDO != null && this.currentBySessionControlStateRDO.Count > 0) ? this.currentBySessionControlStateRDO.Where(o => o.KEY == ControlStateConstan.SampleSender && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdateValueForcboMediOrgCode != null)
                {
                    csAddOrUpdateValueForcboMediOrgCode.VALUE = (btnStateForcboMediOrgCode.Properties.Buttons[0].Visible ? ((cboMediOrgCode.EditValue ?? "").ToString() + "|" + txtSampleSender.Text) : "");
                }
                else
                {
                    csAddOrUpdateValueForcboMediOrgCode = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdateValueForcboMediOrgCode.KEY = ControlStateConstan.SampleSender;
                    csAddOrUpdateValueForcboMediOrgCode.VALUE = (btnStateForcboMediOrgCode.Properties.Buttons[0].Visible ? ((cboMediOrgCode.EditValue ?? "").ToString() + "|" + txtSampleSender.Text) : "");
                    csAddOrUpdateValueForcboMediOrgCode.MODULE_LINK = moduleLink;
                    this.currentBySessionControlStateRDO.Add(csAddOrUpdateValueForcboMediOrgCode);
                }

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateValueFortxtSPECIMEN_ORDER = (this.currentBySessionControlStateRDO != null && this.currentBySessionControlStateRDO.Count > 0) ? this.currentBySessionControlStateRDO.Where(o => o.KEY == ControlStateConstan.SpecimenOrder && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdateValueFortxtSPECIMEN_ORDER != null)
                {
                    csAddOrUpdateValueFortxtSPECIMEN_ORDER.VALUE = (btnStateFortxtSPECIMEN_ORDER.Properties.Buttons[0].Visible ? txtSPECIMEN_ORDER.Text : "");
                }
                else
                {
                    csAddOrUpdateValueFortxtSPECIMEN_ORDER = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdateValueFortxtSPECIMEN_ORDER.KEY = ControlStateConstan.SpecimenOrder;
                    csAddOrUpdateValueFortxtSPECIMEN_ORDER.VALUE = (btnStateFortxtSPECIMEN_ORDER.Properties.Buttons[0].Visible ? txtSPECIMEN_ORDER.Text : "");
                    csAddOrUpdateValueFortxtSPECIMEN_ORDER.MODULE_LINK = moduleLink;
                    this.currentBySessionControlStateRDO.Add(csAddOrUpdateValueFortxtSPECIMEN_ORDER);
                }

                this.controlStateWorker.SetDataBySession(this.currentBySessionControlStateRDO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSPECIMEN_ORDER_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.UpdateStateForAllControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataQrCode()
        {
            try
            {
                if (string.IsNullOrEmpty(txtQrSdt.Text))
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn cần nhập số điện thoại cung cấp khi khai báo thông tin y tế", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            txtQrSdt.Focus();
                            txtQrSdt.SelectAll();
                            return;
                        }
                        else
                        {
                            txtPatientName.Focus();
                            txtPatientName.SelectAll();
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(txtQRKBYT.Text))
                        {
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Config.HisConfigCFG.KBYT_URL), Config.HisConfigCFG.KBYT_URL));
                            WaitingManager.Show();
                            var data = ToKhaiYTeProcessor.GetUserInfo(txtQRKBYT.Text, txtQrSdt.Text, Config.HisConfigCFG.KBYT_URL);
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                            string addressStr = "";
                            if (data != null)
                            {
                                

                                V_SDA_PROVINCE province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_CODE == data.provinceId);
                                if (province != null)
                                {
                                    txtProvinceCode.Text = province.PROVINCE_CODE;
                                    cboProvince.EditValue = province.PROVINCE_CODE;
                                }else{
                                    txtProvinceCode.Text = "";
                                    cboProvince.EditValue = null;
                                }
                                V_SDA_COMMUNE commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().FirstOrDefault(o => o.COMMUNE_CODE == data.townId);
                                if (commune != null)
                                {
                                    txtCommuneCode.Text = commune.COMMUNE_CODE;
                                    this.LoadXaCombo(txtCommuneCode.Text.Trim(), cboDistrict.EditValue !=null ? cboDistrict.EditValue.ToString() : "", true);
                                    cboCommune.EditValue = commune.COMMUNE_CODE;
                                }
                                else
                                {
                                   
                                    txtCommuneCode.Text = "";
                                    cboCommune.EditValue = null;
                                }
                                V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().FirstOrDefault(o => o.DISTRICT_CODE == data.districtId);
                                if (district != null)
                                {
                                    txtDistrictCode.Text = district.DISTRICT_CODE;
                                    cboDistrict.EditValue = district.DISTRICT_CODE;
                                }
                                else
                                {

                                    txtDistrictCode.Text = "";
                                    cboDistrict.EditValue = null;
                                }

                                addressStr = data.town + ", " + data.district + ", " + data.province;

                                string genderCode = "";
                                if (data.gender.ToLower() == "male")
                                {
                                    genderCode = "02";
                                }
                                else if (data.gender.ToLower() == "female")
                                {
                                    genderCode = "01";
                                }
                                else
                                {
                                    genderCode = "03";
                                }
                               
                                cboGender.EditValue = genderCode;
                                // "GENDER_CODE";

                                txtPatientName.Text = data.fullName;
                                txtPhone.Text = data.phone;
                                txtAddress.Text = data.address;
                                this.isNotPatientDayDob = false;
                                if (data.yearOfBirthday > 0)
                                {
                                    dtPatientDob.Text = data.yearOfBirthday.ToString().Substring(0, 4);
                                    txtPatientDob.Text = data.yearOfBirthday.ToString().Substring(0, 4);
                                    DateTime dtNgSinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.yearOfBirthday) ?? DateTime.MinValue;
                                    dtPatientDob.DateTime = dtNgSinh;
                                    this.isNotPatientDayDob = true;
                                }
                                WaitingManager.Hide();
                            }                         
                            else
                            {
                                WaitingManager.Hide();
                                DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy thông tin khai báo y tế", "Thông báo");
                                //txtPatientName.Text = "";
                                //txtPhone.Text = "";
                                //txtAddress.Text = "";
                                //dtPatientDob.DateTime = DateTime.MinValue;
                                //txtPatientDob.Text = "";
                                //cboCommune.EditValue = null;
                                //txtCommuneCode.Text = "";
                                //cboDistrict.EditValue = null;
                                //txtDistrictCode.Text = "";
                                //cboProvince.EditValue = null;
                                //cboProvince.Text = "";
                                //cboGender.EditValue = null;


                            }
                        }
                        else
                        {
                            txtPatientName.Focus();
                            txtPatientName.SelectAll();
                        }
                    }
                
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtQRKBYT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    GetDataQrCode();
                }
                    
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnFocus_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtQrSdt.Focus();
                txtQrSdt.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtQrSdt_Leave(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtQrSdt.Text))
                {
                    txtPhone.Text = txtQrSdt.Text.Trim();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtQrSdt_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtQRKBYT.Text))
                    {
                        GetDataQrCode();
                    }
                    else
                    {
                        txtQRKBYT.Focus();
                        txtQRKBYT.SelectAll();
                    }
                }
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboGender_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPatientDob.Focus();
                    txtPatientDob.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboGender.Focus();
                    cboGender.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        
    }
}

