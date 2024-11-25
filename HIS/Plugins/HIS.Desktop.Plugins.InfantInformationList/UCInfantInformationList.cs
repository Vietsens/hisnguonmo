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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Utilities.Extensions;
using DevExpress.XtraEditors;
using MOS.SDO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using MOS.KskSignData;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.Data;
using System.Resources;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Plugins.InfantInformationList.ADO;
using System.IO;
using System.Diagnostics;
using System.Threading;
using Inventec.Common.SignLibrary.ServiceSign;
using EMR.WCF.DCO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace HIS.Desktop.Plugins.InfantInformationList
{
    public partial class UCInfantInformationList : UserControlBase
    {

        #region Derlare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int pageSize;
        List<HIS_DEPARTMENT> listDepartment;
        List<HIS_DEPARTMENT> departmentSelecteds;
        List<HIS_BRANCH> listBranchs;
        Inventec.Desktop.Common.Modules.Module currentModule { get; set; }
        private bool isNotLoadWhileChangeControlStateInFirst;
        private HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        private List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        private X509Certificate2 certificate;
        private string SerialNumber;
        #endregion
        #region ConstructorLoad
        public UCInfantInformationList(Inventec.Desktop.Common.Modules.Module module)
        {
            InitializeComponent();
            this.currentModule = module;
        }
        private void UCInfantInformationList_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                SetDefaultControl();
                InitComboDepartmentCheck();
                InitComboDepartment();
                InitControlState();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        #region Private Method
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.InfantInformationList.Resources.Lang", typeof(UCInfantInformationList).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCInfantInformationList.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCInfantInformationList.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCInfantInformationList.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCInfantInformationList.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCInfantInformationList.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCInfantInformationList.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCInfantInformationList.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCInfantInformationList.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCInfantInformationList.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCInfantInformationList.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UCInfantInformationList.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UCInfantInformationList.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("UCInfantInformationList.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("UCInfantInformationList.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("UCInfantInformationList.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("UCInfantInformationList.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("UCInfantInformationList.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("UCInfantInformationList.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("UCInfantInformationList.gridColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn19.Caption = Inventec.Common.Resource.Get.Value("UCInfantInformationList.gridColumn19.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn20.Caption = Inventec.Common.Resource.Get.Value("UCInfantInformationList.gridColumn20.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("UCInfantInformationList.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("UCInfantInformationList.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnRefresh.Caption = Inventec.Common.Resource.Get.Value("UCInfantInformationList.bbtnRefresh.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCInfantInformationList.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkSignFileCertUtil.Properties.Caption = Inventec.Common.Resource.Get.Value("UCInfantInformationList.chkSignFileCertUtil.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDongBo.Text = Inventec.Common.Resource.Get.Value("UCInfantInformationList.btnDongBo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDongBo.ToolTip = Inventec.Common.Resource.Get.Value("UCInfantInformationList.btnDongBo.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtPatientCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCInfantInformationList.txtPatientCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("UCInfantInformationList.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UCInfantInformationList.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarControl1.Text = Inventec.Common.Resource.Get.Value("UCInfantInformationList.navBarControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarGroup1.Caption = Inventec.Common.Resource.Get.Value("UCInfantInformationList.navBarGroup1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("UCInfantInformationList.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("UCInfantInformationList.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("UCInfantInformationList.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("UCInfantInformationList.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("UCInfantInformationList.layoutControl6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDepartmentName.Properties.NullText = Inventec.Common.Resource.Get.Value("UCInfantInformationList.cboDepartmentName.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarGroup2.Caption = Inventec.Common.Resource.Get.Value("UCInfantInformationList.navBarGroup2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarGroup3.Caption = Inventec.Common.Resource.Get.Value("UCInfantInformationList.navBarGroup3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtPatientName.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCInfantInformationList.txtPatientName.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtTreatmentCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCInfantInformationList.txtTreatmentCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void FillDataToGrid()
        {
            try
            {
                WaitingManager.Show();
                if (ucPaging.pagingGrid != null)
                {
                    pageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }
                LoadGridData(new CommonParam(0, pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadGridData, param, pageSize, gridControl1);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }
        private void LoadGridData(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<V_HIS_BABY>> apiResult = null;
                HisBabyViewFilter filter = new HisBabyViewFilter();
                SetFilter(ref filter);
                gridView1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<V_HIS_BABY>>("api/HisBaby/GetView", ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gridControl1.DataSource = data;
                    }
                    else
                    {
                        gridControl1.DataSource = null;

                    }
                    rowCount = (data == null ? 0 : data.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                }
                else
                {
                    rowCount = 0;
                    dataTotal = 0;
                    gridControl1.DataSource = null;
                }
                gridView1.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else return false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }
        private void SetFilter(ref HisBabyViewFilter filter)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCode.Text = code;
                    }
                    filter.TREATMENT_CODE__EXACT = txtTreatmentCode.Text;
                }
                if (!string.IsNullOrEmpty(txtPatientCode.Text))
                {
                    string code = txtPatientCode.Text.Trim();
                    if (code.Length < 10 && checkDigit(code))
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtPatientCode.Text = code;
                    }
                    filter.PATIENT_CODE__EXACT = txtPatientCode.Text;

                }
                filter.PATIENT_NAME = !String.IsNullOrEmpty(txtPatientName.Text.Trim()) ? txtPatientName.Text.Trim() : null;

                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                if (dtBornTimeFrom.EditValue != null && dtBornTimeFrom.DateTime != DateTime.MinValue)
                    filter.BORN_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtBornTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");
                if (dtBornTimeTo.EditValue != null && dtBornTimeTo.DateTime != DateTime.MinValue)
                    filter.BORN_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtBornTimeTo.EditValue).ToString("yyyyMMdd") + "235959");

                if (dtIssuedDateFrom.EditValue != null && dtIssuedDateFrom.DateTime != DateTime.MinValue)
                    filter.ISSUED_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtIssuedDateFrom.EditValue).ToString("yyyyMMdd") + "000000");
                if (dtIssuedDateTo.EditValue != null && dtIssuedDateTo.DateTime != DateTime.MinValue)
                    filter.ISSUED_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtIssuedDateTo.EditValue).ToString("yyyyMMdd") + "235959");

                if (cboSYNC_RESULT_TYPE.EditValue != null)
                {
                    switch (cboSYNC_RESULT_TYPE.SelectedIndex)
                    {
                        case 0:
                            filter.SYNC_RESULT_TYPE = null;
                            break;
                        case 1:
                            filter.SYNC_RESULT_TYPE = 1;
                            break;
                        case 2:
                            filter.SYNC_RESULT_TYPE = 2;
                            break;
                        case 3:
                            filter.SYNC_RESULT_TYPE = 3;
                            break;
                        default:
                            filter.SYNC_RESULT_TYPE = null;
                            break;
                    }
                }
                if (this.departmentSelecteds != null && this.departmentSelecteds.Count() > 0)
                {
                    filter.DEPARTMENT_IDs = this.departmentSelecteds.Select(o => o.ID).Distinct().ToList();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetDefaultControl()
        {
            try
            {
                listBranchs = BackendDataWorker.Get<HIS_BRANCH>().ToList();
                dtBornTimeFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.StartMonth() ?? 0));
                dtBornTimeTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.EndDay() ?? 0));
                dtIssuedDateFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.StartMonth() ?? 0));
                dtIssuedDateTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.EndDay() ?? 0));
                txtTreatmentCode.Text = "";
                txtPatientCode.Text = "";
                txtPatientName.Text = "";
                cboSYNC_RESULT_TYPE.SelectedIndex = 0;
                btnDongBo.Enabled = false;
                GridCheckMarksSelection gridCheckMark = cboDepartmentName.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboDepartmentName.Properties.View);
                }
                cboDepartmentName.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitControlState()
        {
            isNotLoadWhileChangeControlStateInFirst = true;
            try
            {
                this.controlStateWorker = new Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(this.currentModule.ModuleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkSignFileCertUtil.Name)
                        {
                            SerialNumber = item.VALUE;

                            chkSignFileCertUtil.Checked = !String.IsNullOrWhiteSpace(SerialNumber);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhileChangeControlStateInFirst = false;
        }
        #endregion
        #region EvenGridView
        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_BABY data = (MOS.EFMODEL.DataModels.V_HIS_BABY)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "SYNC_RESULT_TYPE_STR")
                        {
                            try
                            {
                                if (data.SYNC_RESULT_TYPE == 1)
                                {
                                    e.Value = Resources.ResourceMessage.ChuaDongBo;
                                }
                                else if (data.SYNC_RESULT_TYPE == 2)
                                {
                                    e.Value = Resources.ResourceMessage.ThanhCong;
                                }
                                else if (data.SYNC_RESULT_TYPE == 3)
                                {
                                    e.Value = Resources.ResourceMessage.ThatBai;
                                }
                                else
                                {
                                    e.Value = "";
                                }

                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao SYNC_RESULT_TYPE", ex);
                            }
                        }
                        else if (e.Column.FieldName == "TDL_SOCIAL_INSURANCE_NUMBER_STR")
                        {
                            try
                            {
                                if (!string.IsNullOrEmpty(data.TDL_SOCIAL_INSURANCE_NUMBER))
                                {
                                    e.Value = data.TDL_SOCIAL_INSURANCE_NUMBER;
                                }
                                else if (!string.IsNullOrEmpty(data.TDL_HEIN_CARD_NUMBER))
                                {
                                    e.Value = data.TDL_HEIN_CARD_NUMBER.Length >= 10 ? data.TDL_HEIN_CARD_NUMBER.Substring(data.TDL_HEIN_CARD_NUMBER.Length - 10) : null;
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn(ex);
                            }
                        }
                        else if (e.IsGetData && e.Column.FieldName == "CUSTOM_ADDRESS_STR")
                        {
                            try
                            {
                                string proviceCode = data.BIRTH_PROVINCE_CODE != null ? data.BIRTH_PROVINCE_CODE.ToString() : "_";
                                string dictrictCode = data.BIRTH_DISTRICT_CODE != null ? data.BIRTH_DISTRICT_CODE.ToString() : "_";
                                string communeCode = data.BIRTH_COMMUNE_CODE != null ? data.BIRTH_COMMUNE_CODE.ToString() : "_";
                                string addressFullText = "";

                                if (data.BIRTHPLACE_TYPE == 1)
                                {
                                    addressFullText = data.BIRTH_HOSPITAL_NAME + ", " + data.BIRTHPLACE + ", " + data.BIRTH_COMMUNE_NAME + ", " + data.BIRTH_DISTRICT_NAME + ", " + data.BIRTH_PROVINCE_NAME;
                                }
                                else if (data.BIRTHPLACE_TYPE == 2)
                                {
                                    addressFullText = data.BIRTH_HOSPITAL_NAME + ", " + data.BIRTHPLACE + ", " + data.BIRTH_COMMUNE_NAME + ", " + data.BIRTH_DISTRICT_NAME + ", " + data.BIRTH_PROVINCE_NAME;
                                }
                                else if (data.BIRTHPLACE_TYPE == 3)
                                {
                                    addressFullText = "Sinh tại nhà, " + data.BIRTHPLACE + ", " + data.BIRTH_COMMUNE_NAME + ", " + data.BIRTH_DISTRICT_NAME + ", " + data.BIRTH_PROVINCE_NAME;
                                }
                                else if (data.BIRTHPLACE_TYPE == 4)
                                {
                                    addressFullText = "Đẻ trên đường đi, " + data.BIRTHPLACE + ", " + data.BIRTH_COMMUNE_NAME + ", " + data.BIRTH_DISTRICT_NAME + ", " + data.BIRTH_PROVINCE_NAME;
                                }
                                else if (data.BIRTHPLACE_TYPE == 5)
                                {
                                    addressFullText = "Trẻ bị bỏ rơi, " + data.BIRTHPLACE + ", " + data.BIRTH_COMMUNE_NAME + ", " + data.BIRTH_DISTRICT_NAME + ", " + data.BIRTH_PROVINCE_NAME;
                                }
                                addressFullText = addressFullText.Replace(", , ", ", ").Replace(", ,", ",").TrimEnd(',', ' ');

                                string customAddress = String.Format("{0}:{1}:{2}:{3}", proviceCode, dictrictCode, communeCode, addressFullText);
                                e.Value = customAddress;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn(ex);
                            }
                        }
                        else if (e.IsGetData && e.Column.FieldName == "CUSTOM_BIRTHPLACE_TYPE")
                        {
                            try
                            {
                                string birthPlace = "";
                                if (data.BIRTHPLACE_TYPE == 1)
                                {
                                    birthPlace = data.BIRTH_HOSPITAL_NAME;
                                }
                                else if (data.BIRTHPLACE_TYPE == 2)
                                {
                                    birthPlace = data.BIRTH_HOSPITAL_NAME;
                                }
                                else if (data.BIRTHPLACE_TYPE == 3)
                                {
                                    birthPlace = "Sinh tại nhà";
                                }
                                else if (data.BIRTHPLACE_TYPE == 4)
                                {
                                    birthPlace = "Đẻ trên đường đi";
                                }
                                else if (data.BIRTHPLACE_TYPE == 5)
                                {
                                    birthPlace = "Trẻ bị bỏ rơi";
                                }
                                e.Value = birthPlace;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn(ex);
                            }
                        }
                        else if (e.Column.FieldName == "TDL_PATIENT_DOB_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao TDL_PATIENT_DOB", ex);
                            }
                        }
                        else if (e.Column.FieldName == "BIRTH_CERT_CODE_STR")
                        {
                            try
                            {
                                var branch = listBranchs.FirstOrDefault(o => o.ID == data.BRANCH_ID);
                                string bornTime = "", birthCert = "";
                                if (data.ISSUED_DATE != null)
                                {
                                    bornTime = data.ISSUED_DATE.ToString().Substring(2, 2);
                                }
                                if (data.BIRTH_CERT_NUM != null)
                                {
                                    birthCert = data.BIRTH_CERT_NUM.Value.ToString();
                                    if (data.BIRTH_CERT_NUM.ToString().Length < 5)
                                    {
                                        birthCert = String.Format("{0:00000}", data.BIRTH_CERT_NUM);
                                    }
                                }
                                e.Value = String.Format("{0}.GCS.{1}.{2}", birthCert, branch != null ? branch.HEIN_MEDI_ORG_CODE : null, bornTime);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao BIRTH_CERT_CODE_STR", ex);
                            }
                        }
                        else if (e.Column.FieldName == "BORN_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.BORN_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao BORN_TIME", ex);
                            }
                        }
                        else if (e.Column.FieldName == "IN_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IN_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao IN_TIME", ex);
                            }
                        }
                        else if (e.Column.FieldName == "OUT_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.OUT_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao OUT_TIME", ex);
                            }
                        }
                        else if (e.Column.FieldName == "ISSUED_DATE_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.ISSUED_DATE ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao ISSUED_DATE", ex);
                            }
                        }
                        else if (e.Column.FieldName == "SYNC_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.SYNC_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao SYNC_TIME", ex);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
        }
        public void Search()
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

        public void Refesh()
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
        #region EventClickPress
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultControl();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnRefresh_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnRefresh.Focus();
                    btnRefresh_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtServiceReqCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion
        #region EvenBarManager
        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        #region InitComboDepartment
        private void cboDepartmentName_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string departmentName = "";
                if (this.departmentSelecteds != null && this.departmentSelecteds.Count > 0)
                {
                    foreach (var item in this.departmentSelecteds)
                    {
                        departmentName += item.DEPARTMENT_NAME + ", ";

                    }
                }
                e.DisplayText = departmentName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
        }
        private void InitComboDepartment()
        {
            try
            {
                listDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                cboDepartmentName.Properties.DataSource = listDepartment;
                cboDepartmentName.Properties.DisplayMember = "DEPARTMENT_NAME";
                cboDepartmentName.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn column = cboDepartmentName.Properties.View.Columns.AddField("DEPARTMENT_NAME");
                column.VisibleIndex = 1;
                column.Width = 200;
                column.Caption = Resources.ResourceMessage.TatCa;
                cboDepartmentName.Properties.View.OptionsView.ShowColumnHeaders = true;
                cboDepartmentName.Properties.View.OptionsSelection.MultiSelect = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboDepartmentCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboDepartmentName.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(Event_Check);
                cboDepartmentName.Properties.Tag = gridCheck;
                cboDepartmentName.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboDepartmentName.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboDepartmentName.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void Event_Check(object sender, EventArgs e)
        {

            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                departmentSelecteds = new List<HIS_DEPARTMENT>();
                if (gridCheckMark != null)
                {
                    List<HIS_DEPARTMENT> departmentSelectedNews = new List<HIS_DEPARTMENT>();
                    foreach (HIS_DEPARTMENT item in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (item != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(item.DEPARTMENT_NAME);
                            departmentSelectedNews.Add(item);
                        }
                    }
                    this.departmentSelecteds = new List<HIS_DEPARTMENT>();
                    this.departmentSelecteds.AddRange(departmentSelectedNews);
                }
                this.cboDepartmentName.Text = sb.ToString();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void gridView1_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                if (gridView1.GetSelectedRows().Count() > 0)
                {
                    btnDongBo.Enabled = true;
                }
                else
                {
                    btnDongBo.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public string AppFilePathSignService()
        {
            try
            {
                string pathFolderTemp = Path.Combine(Path.Combine(Path.Combine(Application.StartupPath, "Integrate"), "EMR.SignProcessor"), "EMR.SignProcessor.exe");
                return pathFolderTemp;
            }
            catch (IOException exception)
            {
                Inventec.Common.Logging.LogSystem.Warn("Error create temp file: " + exception.Message);
                return "";
            }
        }
        private bool IsProcessOpen(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName == name || clsProcess.ProcessName == String.Format("{0}.exe", name) || clsProcess.ProcessName == String.Format("{0} (32 bit)", name) || clsProcess.ProcessName == String.Format("{0}.exe (32 bit)", name))
                {
                    return true;
                }
            }

            return false;
        }
        internal bool VerifyServiceSignProcessorIsRunning()
        {
            bool valid = false;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.1");
                string exeSignPath = AppFilePathSignService();
                if (File.Exists(exeSignPath))
                {
                    if (IsProcessOpen("EMR.SignProcessor"))
                    {
                        Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.2");
                        valid = true;
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.3");
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => exeSignPath), exeSignPath));
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.FileName = exeSignPath;
                        try
                        {
                            Process.Start(startInfo);
                            Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.4");
                            Thread.Sleep(500);
                            valid = true;
                            Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.5");
                        }
                        catch (Exception exx)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(exx);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
        private void chkSignFileCertUtil_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                timer1.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private List<ConnectionInfoADO> GetConnectionInfo()
        {
            List<ConnectionInfoADO> listInfo = new List<ConnectionInfoADO>();
            try
            {
                var ConnectionInfo = HisConfigs.Get<string>("MOS.HIS_BABY.CONNECTION_INFO");
                List<string> listBranch = new List<string>();
                if (ConnectionInfo != null)
                {
                    listBranch = ConnectionInfo.Split('|').ToList();
                }
                if (listBranch != null && listBranch.Count > 0)
                {
                    foreach (string branch in listBranch)
                    {
                        var li = branch.Split(';').ToList();
                        if (li != null && li.Count > 3)
                        {
                            ConnectionInfoADO connectionInfoADO = new ConnectionInfoADO();
                            connectionInfoADO.BranchCode = li[0];
                            connectionInfoADO.UserName = li[1];
                            connectionInfoADO.Password = li[2];
                            connectionInfoADO.Url = li[3];
                            listInfo.Add(connectionInfoADO);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                return listInfo;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        private void btnDongBo_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool rs = false;
                WaitingManager.Show();
                var rowHandles = gridView1.GetSelectedRows();
                if (rowHandles != null && rowHandles.Count() > 0)
                {
                    List<BabySyncSDO> listSyncData = new List<BabySyncSDO>();
                    if (chkSignFileCertUtil.Checked)
                    {
                        if (certificate == null && !String.IsNullOrWhiteSpace(SerialNumber))
                        {
                            certificate = Inventec.Common.SignFile.CertUtil.GetBySerial(SerialNumber, requirePrivateKey: true, validOnly: false);
                            if (certificate == null)
                            {
                                WaitingManager.Hide();
                                chkSignFileCertUtil.Checked = false;
                                if (XtraMessageBox.Show(Resources.ResourceMessage.TiepTucHSM, Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                                {
                                    return;
                                }
                            }
                        }
                        List<ConnectionInfoADO> listConnectionInfo = GetConnectionInfo();
                        if (listConnectionInfo != null && listConnectionInfo.Count > 0)
                        {
                            foreach (var item in listConnectionInfo)
                            {
                                HIS.Bhyt.Hssk.SyncDataProcess data = new Bhyt.Hssk.SyncDataProcess(HisConfigs.Get<string>("HIS.CHECK_HEIN_CARD.BHXH__ADDRESS"), item.UserName, item.Password, (string xmlData, string element, string serialNumber) =>
                                {
                                    if (VerifyServiceSignProcessorIsRunning())
                                    {
                                        SignProcessorClient signProcessorClient = new SignProcessorClient();
                                        return signProcessorClient.StringBase64SignXml(xmlData, element, serialNumber);
                                    }else
                                        return null;
                                });
                                foreach (var i in rowHandles)
                                {
                                    var row = (V_HIS_BABY)gridView1.GetRow(i);
                                    if (row != null)
                                    {
                                        string messageError = null;
                                        BabySyncSDO sdo = new BabySyncSDO();
                                        sdo.BabyID = row.ID;
                                        HIS_BRANCH branch = listBranchs.FirstOrDefault(o => o.ID == row.BRANCH_ID);
                                        if (certificate != null)
                                        {
                                            var treatment = GetTreatment_ByID(row.TREATMENT_ID);
                                            sdo.FileBase64Str = data.ProcessBornInfo(branch, row, treatment, certificate, ref messageError);
                                            if (!String.IsNullOrEmpty(messageError))
                                            {
                                                WaitingManager.Hide();
                                                XtraMessageBox.Show(String.Format("{0}: {1}", row.TREATMENT_CODE, messageError), Resources.ResourceMessage.ThongBao);
                                            }
                                        }
                                        listSyncData.Add(sdo);
                                    }
                                }
                            }
                        }
                        else
                        {
                            WaitingManager.Hide();
                            XtraMessageBox.Show(Resources.ResourceMessage.CauHinhKhaiBaoSaiDinhDang, Resources.ResourceMessage.ThongBao);
                            return;
                        }
                    }
                    else
                    {
                        foreach (var i in rowHandles)
                        {
                            var row = (V_HIS_BABY)gridView1.GetRow(i);
                            if (row != null)
                            {
                                BabySyncSDO sdo = new BabySyncSDO();
                                sdo.BabyID = row.ID;
                                listSyncData.Add(sdo);
                            }
                        }
                    }
                    rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisBaby/Sync", ApiConsumer.ApiConsumers.MosConsumer, listSyncData, SessionManager.ActionLostToken, param);
                    if (rs)
                    {
                        FillDataToGrid();
                    }
                }
                WaitingManager.Hide();

                #region Show message
                MessageManager.Show(this.ParentForm, param, rs);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private V_HIS_TREATMENT GetTreatment_ByID(long treatmentId)
        {
            V_HIS_TREATMENT result = null;
            try
            {
                if (treatmentId <= 0)
                    return null;
                CommonParam param = new CommonParam();
                HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                filter.ID = treatmentId;
                result = new BackendAdapter(param)
                    .Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                timer1.Stop();
                SerialNumber = null;
                if (chkSignFileCertUtil.Checked)
                {
                    if (VerifyServiceSignProcessorIsRunning())
                    {
                        WcfSignDCO wcfSignDCO = new WcfSignDCO();
                        wcfSignDCO.HwndParent = this.ParentForm.Handle;
                        string jsonData = JsonConvert.SerializeObject(wcfSignDCO);
                        SignProcessorClient signProcessorClient = new SignProcessorClient();
                        var wcfSignResultDCO = signProcessorClient.GetSerialNumber(jsonData);  //EDIT
                        if (wcfSignResultDCO != null)
                        {
                            SerialNumber = wcfSignResultDCO.OutputFile;
                        }
                    }
                    if (string.IsNullOrEmpty(SerialNumber))
                    {
                        chkSignFileCertUtil.Checked = false;
                        XtraMessageBox.Show(Resources.ResourceMessage.KhongLayDuocThongTinChungThuHoacChungThuKhongHopLe, Resources.ResourceMessage.ThongBao);
                    }
                }
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkSignFileCertUtil.Name && o.MODULE_LINK == this.currentModule.ModuleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = SerialNumber;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkSignFileCertUtil.Name;
                    csAddOrUpdate.VALUE = SerialNumber;
                    csAddOrUpdate.MODULE_LINK = this.currentModule.ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
