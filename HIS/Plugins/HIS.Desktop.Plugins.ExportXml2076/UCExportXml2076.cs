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
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExportXml2076.Base;
using Inventec.Core;
using HIS.Desktop.LocalStorage.ConfigApplication;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Logging;
using DevExpress.XtraEditors;
using HIS.Desktop.Plugins.ExportXml2076.Resources;
using HIS.Desktop.LibraryMessage;
using His.Bhyt.ExportXml.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.Plugins.ExportXml2076.Config;
using ACS.EFMODEL.DataModels;
using DevExpress.Utils;
using DevExpress.XtraGrid;

namespace HIS.Desktop.Plugins.ExportXml2076
{
    public partial class UCExportXml2076 : UserControlBase
    {
        private HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        private List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;

        private HIS_BRANCH currentBranch = null;
        int rowCount = 0;
        int dataTotal = 0;
        int start = 0;
        int limit = 0;

        private V_HIS_TREATMENT_10 currentTreatment = null;

        private bool isInit = true;

        public UCExportXml2076(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            HisConfigCFG.LoadConfig();
        }

        private void UCExportXml2076_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();

                this.InitUser();
                this.InitControlState();
                this.InitComboBranch();
                this.InitComboTreatmentType();
                this.SetDefaultValueControl();
                this.FillDataToGridTreatment();
                this.isInit = false;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {
            try
            {
                txtPathSave.Text = "";
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(ControlStateConstant.MODULE_LINK);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == ControlStateConstant.TXT_PATH_SAVE)
                        {
                            txtPathSave.Text = item.VALUE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void InitComboTreatmentType()
        {
            try
            {
                List<HIS_TREATMENT_TYPE> listAll = new List<HIS_TREATMENT_TYPE>();

                HIS_TREATMENT_TYPE all = new HIS_TREATMENT_TYPE();
                all.ID = 0;
                all.TREATMENT_TYPE_CODE = "00";
                all.TREATMENT_TYPE_NAME = "Tất cả";
                listAll.Add(all);

                listAll.AddRange(BackendDataWorker.Get<HIS_TREATMENT_TYPE>());

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TREATMENT_TYPE_NAME", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TREATMENT_TYPE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboTreatmentType, listAll, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void InitComboBranch()
        {
            try
            {
                List<HIS_BRANCH> listAll = new List<HIS_BRANCH>();

                HIS_BRANCH all = new HIS_BRANCH();
                all.ID = 0;
                all.BRANCH_CODE = "00";
                all.BRANCH_NAME = "Tất cả";

                listAll.Add(all);

                listAll.AddRange(BackendDataWorker.Get<HIS_BRANCH>());

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BRANCH_NAME", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BRANCH_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboBranch, listAll, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task InitUser()
        {
            try
            {
                if (!BackendDataWorker.IsExistsKey<ACS_USER>())
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    List<ACS_USER> datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<ACS.EFMODEL.DataModels.ACS_USER>>("api/AcsUser/Get", HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, filter, paramCommon);

                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(ACS.EFMODEL.DataModels.ACS_USER), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                dtOutTimeFrom.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                dtOutTimeTo.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                cboTreatmentType.EditValue = (long)0;
                txtTreatmentCode.Text = "";
                txtPatientCode.Text = "";
                txtKeyword.Text = "";
                this.currentBranch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == WorkPlace.GetBranchId());
                //cboBranch.EditValue = this.currentBranch.ID;
                cboStatus.SelectedIndex = 1;
                cboXmlType.SelectedIndex = 0;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridTreatment()
        {
            try
            {
                int pageSize = 0;
                if (ucPaging.pagingGrid != null)
                {
                    pageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }
                if (pageSize <= 0) pageSize = 50;
                FillDataToGridTreatment(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(FillDataToGridTreatment, param, pageSize, this.gridControlTreatment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridTreatment(object param)
        {
            try
            {

                currentTreatment = null;
                this.FillDataByTreatmentSelect();
                List<V_HIS_TREATMENT_10> listData = new List<V_HIS_TREATMENT_10>();

                start = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);

                HisTreatmentView10Filter filter = new HisTreatmentView10Filter();
                filter.ORDER_DIRECTION = "ACS";
                filter.ORDER_FIELD = "OUT_TIME";
                //filter.TDL_PATIENT_TYPE_IDs = new List<long>()
                //{
                //    HisConfigCFG.PATIENT_TYPE_ID__BHYT
                //};
                if (cboBranch.EditValue != null && Convert.ToInt64(cboBranch.EditValue) > 0)
                    filter.BRANCH_ID = Convert.ToInt64(cboBranch.EditValue);

                filter.IS_PAUSE = true;
                filter.XML2076_TYPE = ENUM_XML2076_TYPE.ALL;

                if (cboXmlType.SelectedIndex == 1)
                {
                    filter.XML2076_TYPE = ENUM_XML2076_TYPE.CT03;
                }
                else if (cboXmlType.SelectedIndex == 2)
                {
                    filter.XML2076_TYPE = ENUM_XML2076_TYPE.CT04;
                }
                else if (cboXmlType.SelectedIndex == 3)
                {
                    filter.XML2076_TYPE = ENUM_XML2076_TYPE.CT05;
                }
                else if (cboXmlType.SelectedIndex == 4)
                {
                    filter.XML2076_TYPE = ENUM_XML2076_TYPE.CT06;
                }
                else if (cboXmlType.SelectedIndex == 5)
                {
                    filter.XML2076_TYPE = ENUM_XML2076_TYPE.CT07;
                }

                if (!String.IsNullOrEmpty(txtTreatmentCode.Text.Trim()))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (Char.IsDigit(code.FirstOrDefault()))
                    {
                        if (code.Length < 12)
                        {
                            try
                            {
                                code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                                txtTreatmentCode.Text = code;
                                filter.TREATMENT_CODE__EXACT = code;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else
                        {
                            filter.TREATMENT_CODE__EXACT = code;
                        }
                    }
                }
                else if (!String.IsNullOrEmpty(txtPatientCode.Text.Trim()))
                {
                    string code = txtPatientCode.Text.Trim();
                    if (Char.IsDigit(code.FirstOrDefault()))
                    {
                        if (code.Length < 10)
                        {
                            try
                            {
                                code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                                txtPatientCode.Text = code;
                                filter.PATIENT_CODE__EXACT = code;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else
                        {
                            filter.PATIENT_CODE__EXACT = code;
                        }
                    }
                }

                if (String.IsNullOrEmpty(filter.TREATMENT_CODE__EXACT) && String.IsNullOrEmpty(filter.PATIENT_CODE__EXACT))
                {
                    if (!String.IsNullOrWhiteSpace(txtKeyword.Text))
                    {
                        filter.KEY_WORD = txtKeyword.Text.Trim();
                    }

                    if (cboTreatmentType.EditValue != null && (long)cboTreatmentType.EditValue > 0)
                    {
                        filter.TREATMENT_TYPE_ID = (long)cboTreatmentType.EditValue;
                    }


                    if (cboStatus.SelectedIndex == 1)
                    {
                        filter.IS_EXPORTED_XML2076 = false;
                    }
                    else if (cboStatus.SelectedIndex == 2)
                    {
                        filter.IS_EXPORTED_XML2076 = true;
                    }

                    if (dtOutTimeFrom.EditValue != null && dtOutTimeFrom.DateTime != DateTime.MinValue)
                    {
                        filter.OUT_TIME_FROM = Convert.ToInt64(dtOutTimeFrom.DateTime.ToString("yyyyMMddHHmm") + "00");
                    }
                    if (dtOutTimeTo.EditValue != null && dtOutTimeTo.DateTime != DateTime.MinValue)
                    {
                        filter.OUT_TIME_TO = Convert.ToInt64(dtOutTimeTo.DateTime.ToString("yyyyMMddHHmm") + "59");
                    }
                }

                var result = new BackendAdapter(paramCommon).GetRO<List<V_HIS_TREATMENT_10>>("api/HisTreatment/GetView10", ApiConsumers.MosConsumer, filter, paramCommon);
                if (result != null)
                {
                    listData = result.Data;
                    rowCount = (listData == null ? 0 : listData.Count);
                    dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
                }
                gridControlTreatment.BeginUpdate();
                gridControlTreatment.DataSource = listData;
                gridControlTreatment.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataByTreatmentSelect()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    lblAddress.Text = this.currentTreatment.TDL_PATIENT_ADDRESS ?? "";
                    lblCarrer.Text = this.currentTreatment.TDL_PATIENT_CAREER_NAME ?? "";
                    if (this.currentTreatment.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == (short)1)
                    {
                        lblDob.Text = this.currentTreatment.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    }
                    else
                    {
                        lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentTreatment.TDL_PATIENT_DOB);
                    }
                    lblEndDepartment.Text = this.currentTreatment.END_DEPARTMENT_NAME ?? "";
                    lblEthnicName.Text = this.currentTreatment.ETHNIC_NAME ?? "";
                    lblGenderName.Text = this.currentTreatment.TDL_PATIENT_GENDER_NAME ?? "";
                    lblHeinCardNumber.Text = this.currentTreatment.TDL_HEIN_CARD_NUMBER ?? "";
                    lblIcdName.Text = this.currentTreatment.ICD_NAME ?? "";
                    lblIcdText.Text = this.currentTreatment.ICD_TEXT ?? "";
                    lblInTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(this.currentTreatment.IN_TIME) ?? "";
                    lblOutTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(this.currentTreatment.OUT_TIME ?? 0) ?? "";
                    lblPatientCode.Text = this.currentTreatment.TDL_PATIENT_CODE ?? "";
                    lblPatientName.Text = this.currentTreatment.TDL_PATIENT_NAME ?? "";
                    lblRelativeName.Text = this.currentTreatment.TDL_PATIENT_RELATIVE_NAME ?? "";
                    lblTreatmentCode.Text = this.currentTreatment.TREATMENT_CODE ?? "";
                    lblTreatmentEndType.Text = this.currentTreatment.TREATMENT_END_TYPE_NAME ?? "";
                    lblTreatmentEndTypeExt.Text = this.currentTreatment.TREATMENT_END_TYPE_EXT_NAME ?? "";
                    lblTreatmentResult.Text = this.currentTreatment.TREATMENT_RESULT_NAME ?? "";
                    lblTreatmentType.Text = this.currentTreatment.TREATMENT_TYPE_NAME ?? "";
                    lblWorkPlace.Text = this.currentTreatment.TDL_PATIENT_WORK_PLACE ?? (this.currentTreatment.TDL_PATIENT_WORK_PLACE_NAME ?? "");
                    lblAdvise.Text = this.currentTreatment.ADVISE ?? "";
                    txtClinicalNote.Text = this.currentTreatment.CLINICAL_NOTE ?? "";
                    txtSubclinicalResult.Text = this.currentTreatment.SUBCLINICAL_RESULT ?? "";
                    txtTreatmentMethod.Text = this.currentTreatment.TREATMENT_METHOD ?? "";

                    if (this.currentTreatment.TREATMENT_END_TYPE_EXT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM
                        || this.currentTreatment.TREATMENT_END_TYPE_EXT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_DUONG_THAI)
                    {
                        lblSickRelativeName.Text = this.currentTreatment.TDL_PATIENT_RELATIVE_NAME ?? "";
                        lblSickTime.Text = String.Format("{0} - {1}", Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentTreatment.SICK_LEAVE_FROM ?? 0), Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentTreatment.SICK_LEAVE_TO ?? 0));
                        lblSickUser.Text = String.Format("{0} - {1}", this.currentTreatment.SICK_LOGINNAME, this.currentTreatment.SICK_USERNAME);
                        lblExtraCode.Text = this.currentTreatment.EXTRA_END_CODE ?? "";
                        if (!String.IsNullOrEmpty(this.currentTreatment.TDL_PATIENT_WORK_PLACE_NAME))
                            lblSickWorkPlace.Text = this.currentTreatment.TDL_PATIENT_WORK_PLACE_NAME;
                        else
                            lblSickWorkPlace.Text = this.currentTreatment.TDL_PATIENT_WORK_PLACE ?? "";
                    }
                    else
                    {
                        lblSickRelativeName.Text = "";
                        lblSickTime.Text = "";
                        lblSickUser.Text = "";
                        lblExtraCode.Text = "";
                        lblSickWorkPlace.Text = "";
                    }

                    HisBabyViewFilter babyFilter = new HisBabyViewFilter();
                    babyFilter.TREATMENT_ID = this.currentTreatment.ID;
                    List<V_HIS_BABY> listBabys = new BackendAdapter(new CommonParam()).Get<List<V_HIS_BABY>>("api/HisBaby/GetView", ApiConsumers.MosConsumer, babyFilter, null);
                    listBabys = listBabys != null ? listBabys.Where(o => o.BIRTH_CERT_BOOK_ID.HasValue).ToList() : null;
                    gridControlBabys.BeginUpdate();
                    gridControlBabys.DataSource = listBabys;
                    gridControlBabys.EndUpdate();
                }
                else
                {
                    lblAddress.Text = "";
                    lblCarrer.Text = "";
                    lblDob.Text = "";
                    lblEndDepartment.Text = "";
                    lblEthnicName.Text = "";
                    lblGenderName.Text = "";
                    lblHeinCardNumber.Text = "";
                    lblIcdName.Text = "";
                    lblIcdText.Text = "";
                    lblInTime.Text = "";
                    lblOutTime.Text = "";
                    lblPatientCode.Text = "";
                    lblPatientName.Text = "";
                    lblRelativeName.Text = "";
                    lblTreatmentCode.Text = "";
                    lblTreatmentEndType.Text = "";
                    lblTreatmentEndTypeExt.Text = "";
                    lblTreatmentResult.Text = "";
                    lblTreatmentType.Text = "";
                    lblWorkPlace.Text = "";
                    lblAdvise.Text = "";

                    txtClinicalNote.Text = "";
                    txtSubclinicalResult.Text = "";
                    txtTreatmentMethod.Text = "";

                    lblSickRelativeName.Text = "";
                    lblSickTime.Text = "";
                    lblSickUser.Text = "";
                    lblExtraCode.Text = "";
                    lblSickWorkPlace.Text = "";

                    gridControlBabys.BeginUpdate();
                    gridControlBabys.DataSource = null;
                    gridControlBabys.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBranch_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    dtOutTimeFrom.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBranch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtOutTimeFrom.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtOutTimeFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    dtOutTimeTo.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtOutTimeFrom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtOutTimeTo.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtOutTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    cboTreatmentType.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtOutTimeTo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboTreatmentType.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTreatmentType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtTreatmentCode.Focus();
                    txtTreatmentCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTreatmentType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTreatmentCode.Focus();
                    txtTreatmentCode.SelectAll();
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
                    if (!String.IsNullOrWhiteSpace(txtTreatmentCode.Text))
                    {
                        btnFind_Click(null, null);
                    }
                    else
                    {
                        txtPatientCode.Focus();
                        txtPatientCode.SelectAll();
                    }
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
                    if (!String.IsNullOrWhiteSpace(txtPatientCode.Text))
                    {
                        btnFind_Click(null, null);
                    }
                    else
                    {
                        txtKeyword.Focus();
                        txtKeyword.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboStatus.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboStatus_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboStatus_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnFind.Enabled) return;
                WaitingManager.Show();
                FillDataToGridTreatment();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatment_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex < 0 || !e.IsGetData || e.Column.UnboundType == DevExpress.Data.UnboundColumnType.Bound)
                    return;
                var data = (V_HIS_TREATMENT_10)gridViewTreatment.GetRow(e.ListSourceRowIndex);
                if (data != null)
                {
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + (ucPaging.pagingGrid.CurrentPage - 1) * ucPaging.pagingGrid.PageSize;
                    }
                    else if (e.Column.FieldName == "DOB_STR")
                    {
                        if (data.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == (short)1)
                        {
                            e.Value = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                        }
                        else
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                        }
                    }
                    else if (e.Column.FieldName == "IN_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.IN_TIME);
                    }
                    else if (e.Column.FieldName == "OUT_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.OUT_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "STATUS")
                    {
                        if (data.IS_EXPORTED_XML2076 == (short)1 || !String.IsNullOrWhiteSpace(data.XML2076_URL))
                        {
                            e.Value = "Đã xuất";
                        }
                        else
                        {
                            e.Value = "Chưa xuất";
                        }
                    }
                    else if (e.Column.FieldName == "SICK_HEIN_CARD_NUMBER_STR")
                    {
                        if (!string.IsNullOrEmpty(data.SICK_HEIN_CARD_NUMBER))
                            e.Value = data.SICK_HEIN_CARD_NUMBER;
                        else
                            e.Value = data.TDL_HEIN_CARD_NUMBER;
                    }
                    //else if (e.Column.FieldName == "STATUS_ICON") 
                    //{
                    //    if (data.XML2076_RESULT == 1) // Lỗi đồng bộ
                    //    {
                    //        e.Value = !string.IsNullOrEmpty(data.XML130_DESC)
                    //            ? Properties.Resources.caution 
                    //            : Properties.Resources.cross;   
                    //    }
                    //    else if (data.XML2076_RESULT == 2) 
                    //    {
                    //        e.Value = Properties.Resources.check; 
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewBabys_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex < 0 || !e.IsGetData || e.Column.UnboundType == DevExpress.Data.UnboundColumnType.Bound)
                    return;
                var data = (V_HIS_BABY)gridViewBabys.GetRow(e.ListSourceRowIndex);
                if (data != null)
                {
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "BORN_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.BORN_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPathSave_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)
                {
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        if (this.MakeFolderSave(fbd.SelectedPath))
                        {
                            txtPathSave.Text = fbd.SelectedPath;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool MakeFolderSave(string folderPath)
        {
            bool result = false;
            try
            {
                if (!String.IsNullOrWhiteSpace(folderPath))
                {
                    if (System.IO.Directory.Exists(folderPath))
                    {
                        return true;
                    }
                    var dicInfo = System.IO.Directory.CreateDirectory(folderPath);
                    if (dicInfo == null)
                    {
                        LogSystem.Warn("Tao folderPath luu file that bai");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void txtPathSave_EditValueChanged(object sender, EventArgs e)
        {
            try
            {

                if (isInit)
                {
                    return;
                }

                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.TXT_PATH_SAVE && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = txtPathSave.Text ?? "";
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.TXT_PATH_SAVE;
                    csAddOrUpdate.VALUE = txtPathSave.Text ?? "";
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnExportXml_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnExportXml.Enabled) return;
                gridViewTreatment.PostEditor();
                int[] seleteds = gridViewTreatment.GetSelectedRows();
                if (seleteds == null || seleteds.Length <= 0)
                {
                    XtraMessageBox.Show(ResourceMessage.BanChuaChonHoSoXuatXml2076, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), DevExpress.Utils.DefaultBoolean.True);
                    return;
                }

                if (String.IsNullOrWhiteSpace(txtPathSave.Text))
                {
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        txtPathSave.Text = fbd.SelectedPath;
                    }
                    else
                    {
                        return;
                    }
                }
                bool success = false;
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                List<V_HIS_TREATMENT_10> listSeleted = new List<V_HIS_TREATMENT_10>();
                foreach (int index in seleteds)
                {
                    V_HIS_TREATMENT_10 treat = (V_HIS_TREATMENT_10)gridViewTreatment.GetRow(index);
                    listSeleted.Add(treat);
                }


                success = this.GenerateXml2076(ref param, listSeleted);

                WaitingManager.Hide();
                if (success && param.Messages.Count == 0)
                {
                    MessageManager.Show(this.ParentForm, param, success);
                    this.gridViewTreatment.BeginDataUpdate();
                    this.gridViewTreatment.EndDataUpdate();
                }
                else
                {
                    MessageManager.Show(param, success);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool GenerateXml2076(ref CommonParam paramExport, List<V_HIS_TREATMENT_10> listSelected)
        {
            bool result = false;
            try
            {
                if (listSelected.Count > 0)
                {
                    if (!GlobalConfigStore.IsInit)
                        if (!this.SetDataToLocalXml())
                        {
                            paramExport.Messages.Add(ResourceMessage.KhongThieLapDuocCauHinhDuLieuXuatXml);
                            return result;
                        }
                    GlobalConfigStore.PathSaveXml = txtPathSave.Text;

                    int skip = 0;
                    while (listSelected.Count - skip > 0)
                    {
                        List<V_HIS_TREATMENT_10> limit = listSelected.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                        skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                        string message = "";
                        HisBabyViewFilter babyFilter = new HisBabyViewFilter();
                        babyFilter.TREATMENT_IDs = limit.Select(s => s.ID).ToList();
                        List<V_HIS_BABY> babys = new BackendAdapter(new CommonParam()).Get<List<V_HIS_BABY>>("api/HisBaby/GetView", ApiConsumers.MosConsumer, babyFilter, null);
                        //babys = babys != null ? babys.Where(o => o.BIRTH_CERT_BOOK_ID.HasValue).ToList() : null;
                        babys = babys != null ? babys.ToList() : null;
                        message = ProcessExportXmlDetail(ref result, limit, babys);

                        if (!String.IsNullOrEmpty(message))
                        {
                            paramExport.Messages.Add(message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private string ProcessExportXmlDetail(ref bool isSuccess, List<V_HIS_TREATMENT_10> listTreatment, List<V_HIS_BABY> listBaby)
        {
            string result = "";
            List<V_HIS_BABY> babys = listBaby != null ? listBaby.Where(o => o.BIRTH_CERT_BOOK_ID.HasValue).ToList() : null;
            Dictionary<string, List<string>> DicErrorMess = new Dictionary<string, List<string>>();
            try
            {
                List<V_HIS_TREATMENT_10> listSuccess = new List<V_HIS_TREATMENT_10>();

                Dictionary<long, List<V_HIS_BABY>> dicBaby = new Dictionary<long, List<V_HIS_BABY>>();

                if (babys != null && babys.Count > 0)
                {
                    foreach (V_HIS_BABY item in babys)
                    {
                        if (!dicBaby.ContainsKey(item.TREATMENT_ID))
                            dicBaby[item.TREATMENT_ID] = new List<V_HIS_BABY>();
                        dicBaby[item.TREATMENT_ID].Add(item);
                    }
                }
                string xmlTypeCode = null;
                if (cboXmlType.SelectedIndex == 1)
                {
                    xmlTypeCode = "CT03";
                }
                else if (cboXmlType.SelectedIndex == 2)
                {
                    xmlTypeCode = "CT04";
                }
                else if (cboXmlType.SelectedIndex == 3)
                {
                    xmlTypeCode = "CT05";
                }
                else if (cboXmlType.SelectedIndex == 4)
                {
                    xmlTypeCode = "CT06";
                }
                else if (cboXmlType.SelectedIndex == 5)
                {
                    xmlTypeCode = "CT07";
                }
                List<string> lstTreatmentCode = new List<string>();
                foreach (V_HIS_TREATMENT_10 treat in listTreatment)
                {
                    Inventec.Common.Logging.LogSystem.Error(treat.ADVISE + "   START #######################_______________________________________");
                    var lstBabyNotHasBirthCertBook = listBaby.Where(o => !o.BIRTH_CERT_BOOK_ID.HasValue && o.TREATMENT_ID == treat.ID).FirstOrDefault();
                    if (lstBabyNotHasBirthCertBook != null)
                    {
                        lstTreatmentCode.Add(treat.TREATMENT_CODE);
                    }
                    else
                    {
                        InputADO ado = new InputADO();
                        ado.Treatment2076 = treat;
                        Inventec.Common.Logging.LogSystem.Error(treat.ADVISE + "  END  #######################_______________________________________");
                        ado.Employees = BackendDataWorker.Get<HIS_EMPLOYEE>();
                        ado.Branch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == treat.BRANCH_ID);
                        ado.AcsUsers = BackendDataWorker.Get<ACS_USER>();
                        if (dicBaby.ContainsKey(treat.ID))
                        {
                            ado.Babys = dicBaby[treat.ID];
                        }
                        ado.XML2076__DOC_CODE = xmlTypeCode;
                        ado.ConfigData = BackendDataWorker.Get<HIS_CONFIG>().ToList();
                        His.Bhyt.ExportXml.CreateXmlMain xmlMain = new His.Bhyt.ExportXml.CreateXmlMain(ado);

                        string errorMess = "";
                        //string errorCode = "";
                        var fullFileName = xmlMain.Run2076Path(ref errorMess);

                        if (String.IsNullOrWhiteSpace(fullFileName))
                        {
                            LogSystem.Info("Khong tao duoc XML2076 cho Ho so duyet bhyt TreatmentCode: " + LogUtil.TraceData("TREATMENT_CODE", treat.TREATMENT_CODE));
                            if (!DicErrorMess.ContainsKey(errorMess))
                            {
                                DicErrorMess[errorMess] = new List<string>();
                            }
                        }
                        else
                        {
                            isSuccess = true;
                            listSuccess.Add(treat);
                        }
                    }

                }
                if (lstTreatmentCode != null && lstTreatmentCode.Count > 0)
                {
                    WaitingManager.Hide();
                    string treatmentCode = String.Join(",", lstTreatmentCode);
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.KhongCoSoChungSinh + ": " + treatmentCode, ResourceMessage.Thongbao, System.Windows.Forms.MessageBoxButtons.OK);
                }
                if (DicErrorMess.Count > 0)
                {
                    foreach (var item in DicErrorMess)
                    {
                        result += String.Format("{0}:{1}. ", item.Key, String.Join(",", item.Value));
                    }
                }

                List<V_HIS_TREATMENT_10> listUpdate = listSuccess.Where(o => o.IS_EXPORTED_XML2076 != (short)1).ToList();

                //this.UpdateIsExported(listUpdate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        private void UpdateIsExported(List<V_HIS_TREATMENT_10> listUpdate)
        {
            try
            {
                if (listUpdate != null && listUpdate.Count > 0)
                {
                    CommonParam upParam = new CommonParam();
                    bool rs = new BackendAdapter(upParam).Post<bool>("api/HisTreatment/UpdateExportedXml2076", ApiConsumers.MosConsumer, listUpdate.Select(s => s.ID).ToList(), upParam);
                    if (!rs)
                    {
                        LogSystem.Warn("Update IS_EXPORTED_XML2076 cho Treatment that bai: \n" + String.Join(", ", listUpdate.Select(s => s.TREATMENT_CODE).ToList()));
                    }
                    else
                    {
                        listUpdate.ForEach(o => o.IS_EXPORTED_XML2076 = (short)1);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool SetDataToLocalXml()
        {
            bool result = false;
            try
            {
                if (this.currentBranch == null)
                {
                    return result;
                }

                GlobalConfigStore.Branch = this.currentBranch;

                GlobalConfigStore.IsInit = true;
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public void BtnFind()
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnExportXml()
        {
            try
            {
                btnExportXml_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                LogSystem.Info("repositoryItemBtnExport_Click. 1");
                V_HIS_TREATMENT_10 row = (V_HIS_TREATMENT_10)gridViewTreatment.GetFocusedRow();
                if (row != null)
                {
                    LogSystem.Info("repositoryItemBtnExport_Click. 2");
                    if (String.IsNullOrWhiteSpace(txtPathSave.Text))
                    {
                        FolderBrowserDialog fbd = new FolderBrowserDialog();
                        if (fbd.ShowDialog() == DialogResult.OK)
                        {
                            txtPathSave.Text = fbd.SelectedPath;
                        }
                        else
                        {
                            return;
                        }
                    }
                    bool success = false;
                    CommonParam param = new CommonParam();
                    WaitingManager.Show();
                    List<V_HIS_TREATMENT_10> listSeleted = new List<V_HIS_TREATMENT_10>();
                    listSeleted.Add(row);


                    success = this.GenerateXml2076(ref param, listSeleted);

                    WaitingManager.Hide();
                    if (success && param.Messages.Count == 0)
                    {
                        MessageManager.Show(this.ParentForm, param, success);
                        this.gridViewTreatment.BeginDataUpdate();
                        this.gridViewTreatment.EndDataUpdate();
                    }
                    else
                    {
                        MessageManager.Show(param, success);
                    }
                }
                LogSystem.Info("repositoryItemBtnExport_Click. 3");
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatment_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    var rowData = (V_HIS_TREATMENT_10)view.GetRow(e.RowHandle);
                    if (rowData == null) return;

                    if (e.Column.FieldName == "STATUS_ICON") 
                    {
                        if (rowData.XML2076_RESULT == 1) // Lỗi đồng bộ
                        {
                            if (rowData.XML2076_DESC != null)
                            {
                                e.RepositoryItem = btnError;
                            }
                            else
                            {
                                e.RepositoryItem = btnFail; 
                            }
                        }
                        else if (rowData.XML2076_RESULT == 2) // Gửi thành công
                        {
                            e.RepositoryItem = btnSuccess;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnError_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (V_HIS_TREATMENT_10)gridViewTreatment.GetFocusedRow();
                if (row != null && !string.IsNullOrEmpty(row.XML2076_DESC))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(row.XML2076_DESC, "Thông tin lỗi");
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
    }
}
