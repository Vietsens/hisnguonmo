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
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Plugins.TreatmentInspection.ADO;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.LocalStorage.BackendData;
using System.Resources;
using HIS.Desktop.Plugins.TreatmentInspection.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using MOS.Filter;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors.Controls;

namespace HIS.Desktop.Plugins.TreatmentInspection.TreatmentInspection
{
    public partial class UCTreatmentInspection : UserControlBase
    {
        int rowCount = 0;
        int dataTotal = 0;
        int start = 0;
        Inventec.Desktop.Common.Modules.Module currentModule = null;

        MOS.EFMODEL.DataModels.V_HIS_TREATMENT_11 Treatment11;
        List<TrangThaiADO> listTrangThai;
        List<HIS_DEPARTMENT> listDepartment;
        List<V_HIS_TREATMENT_11> ListHisTreatment = new List<V_HIS_TREATMENT_11>();

        int lastRowHandle = -1;
        DevExpress.XtraGrid.Columns.GridColumn lastColumn = null;
        DevExpress.Utils.ToolTipControlInfo lastInfo = null;



        public UCTreatmentInspection(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCTreatmentInspection_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                GetDataCombo();

                LoadComboStatus();

                LoadComboDepartment();

                SetCaptionByLanguageKey();

                SetDefaultControl();

                FillDataToGrid();

                gridControl1.ToolTipController = this.toolTipController1;

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FillDataToGrid()
        {
            try
            {
                int pageSize;
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }

                FillDataToGridTreatment(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridTreatment, param, pageSize, gridControl1);
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
                WaitingManager.Show();
                start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);

                HisTreatmentView11Filter filter11 = new HisTreatmentView11Filter();


                if (!string.IsNullOrEmpty(txtPATIENTCODE.Text))
                {
                    string code = txtPATIENTCODE.Text.Trim();
                    if (code.Length < 10)
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtPATIENTCODE.Text = code;
                    }
                    filter11 = new HisTreatmentView11Filter();
                    filter11.PATIENT_CODE__EXACT = code;
                }
               


                if (!string.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCode.Text = code;
                    }
                    filter11 = new HisTreatmentView11Filter();
                    filter11.TREATMENT_CODE__EXACT = code;
                    FomatControlByTreatment();
                }
                else if (!string.IsNullOrEmpty(txtStorageCode.Text))
                {
                    filter11 = new HisTreatmentView11Filter();
                    filter11.MR_STORE_CODE__EXACT = txtStorageCode.Text.Trim();

                    FomatControlByMrStore();
                }
                else
                {
                    filter11.KEY_WORD = txtKeyWord.Text.Trim();

                    filter11.ORDER_FIELD = "MODIFY_TIME";
                    filter11.ORDER_DIRECTION = "DESC";

                    if ((dtStorageDateFrom.EditValue == null || dtStorageDateFrom.EditValue.ToString() == "") && (dtStorageDateTo.EditValue == null || dtStorageDateTo.EditValue.ToString() == "") && (dtInTimeFrom.EditValue == null || dtInTimeFrom.EditValue.ToString() == "") && (dtInTimeTo.EditValue == null || dtInTimeTo.EditValue.ToString() == "") && (dtOutTimeFrom.EditValue == null || dtOutTimeFrom.EditValue.ToString() == "") && (dtOutTimeTo.EditValue == null || dtOutTimeTo.EditValue.ToString() == ""))
                    {
                        WaitingManager.Hide();
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.TranhCaoTai, Resources.ResourceMessage.Thongbao, System.Windows.Forms.MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK)
                            return;
                    }

                    if (dtStorageDateFrom.EditValue != null && dtStorageDateFrom.DateTime != DateTime.MinValue)
                    {
                        filter11.STORE_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtStorageDateFrom.EditValue).ToString("yyyyMMdd") + "000000");
                    }
                    if (dtStorageDateTo.EditValue != null && dtStorageDateTo.DateTime != DateTime.MinValue)
                    {
                        filter11.STORE_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtStorageDateTo.EditValue).ToString("yyyyMMdd") + "235959");
                    }
                    if (dtInTimeFrom.EditValue != null && dtInTimeFrom.DateTime != DateTime.MinValue)
                    {
                        filter11.IN_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtInTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");
                    }
                    if (dtInTimeTo.EditValue != null && dtInTimeTo.DateTime != DateTime.MinValue)
                    {
                        filter11.IN_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtInTimeTo.EditValue).ToString("yyyyMMdd") + "235959");
                    }
                    if (dtOutTimeFrom.EditValue != null && dtOutTimeFrom.DateTime != DateTime.MinValue)
                    {
                        filter11.OUT_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtOutTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");
                    }
                    if (dtOutTimeTo.EditValue != null && dtOutTimeTo.DateTime != DateTime.MinValue)
                    {
                        filter11.OUT_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtOutTimeTo.EditValue).ToString("yyyyMMdd") + "235959");
                    }
                    if (cboDepartmentLast.EditValue != null)
                    {
                        filter11.LAST_DEPARTMENT_ID = (long?)cboDepartmentLast.EditValue;
                    }
                    if (cboDepartmentEnd.EditValue != null)
                    {
                        filter11.END_DEPARTMENT_ID = (long?)cboDepartmentEnd.EditValue;
                    }
                    if (cboStatus.EditValue != null)
                    {
                        if ((long)cboStatus.EditValue == (long)1)
                        {
                            filter11.IS_PAUSE = false;
                        }
                        else if ((long)cboStatus.EditValue == (long)2)
                        {
                            filter11.IS_PAUSE = true;
                            filter11.HAS_MEDI_RECORD = false;
                        }
                        else if ((long)cboStatus.EditValue == (long)3)
                        {
                            filter11.HAS_MEDI_RECORD = true;
                            filter11.HAS_RECORD_INSPECTION_STT_ID = false;
                        }
                        else if ((long)cboStatus.EditValue == (long)4)
                        {
                            filter11.RECORD_INSPECTION_STT_ID = 1;
                        }
                        else if ((long)cboStatus.EditValue == (long)5)
                        {
                            filter11.RECORD_INSPECTION_STT_ID = 2;
                        }
                    }
                }

                Inventec.Common.Logging.LogSystem.Info("filter11: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter11), filter11));

                var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<V_HIS_TREATMENT_11>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW11, ApiConsumers.MosConsumer, filter11, paramCommon);

                if (result != null)
                {
                    ListHisTreatment = (List<V_HIS_TREATMENT_11>)result.Data;
                    gridControl1.BeginUpdate();
                    gridControl1.DataSource = null;
                    gridControl1.DataSource = ListHisTreatment;
                    rowCount = (ListHisTreatment == null ? 0 : ListHisTreatment.Count);
                    dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
                    gridControl1.EndUpdate();
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FomatControlByMrStore()
        {
            try
            {
                btnUnSave.Enabled = false;
                btnDuyetGiamDinh.Enabled = false;

                txtTreatmentCode.Text = "";
                txtKeyWord.Text = "";

                dtStorageDateFrom.EditValue = null;
                dtStorageDateTo.EditValue = null;
                dtInTimeFrom.EditValue = null;
                dtInTimeTo.EditValue = null;
                dtOutTimeFrom.EditValue = null;
                dtOutTimeTo.EditValue = null;

                dtStorageDateFrom.Text = "";
                dtStorageDateTo.Text = "";
                dtInTimeFrom.Text = "";
                dtInTimeTo.Text = "";
                dtOutTimeFrom.Text = "";
                dtOutTimeTo.Text = "";

                cboDepartmentLast.EditValue = null;
                cboDepartmentEnd.EditValue = null;
                cboStatus.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FomatControlByTreatment()
        {
            try
            {
                btnUnSave.Enabled = false;
                btnDuyetGiamDinh.Enabled = false;

                txtStorageCode.Text = "";
                txtKeyWord.Text = "";

                dtStorageDateFrom.EditValue = null;
                dtStorageDateTo.EditValue = null;
                dtInTimeFrom.EditValue = null;
                dtInTimeTo.EditValue = null;
                dtOutTimeFrom.EditValue = null;
                dtOutTimeTo.EditValue = null;

                dtStorageDateFrom.Text = "";
                dtStorageDateTo.Text = "";
                dtInTimeFrom.Text = "";
                dtInTimeTo.Text = "";
                dtOutTimeFrom.Text = "";
                dtOutTimeTo.Text = "";

                cboDepartmentLast.EditValue = null;
                cboDepartmentEnd.EditValue = null;
                cboStatus.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.TreatmentInspection.Resources.Lang", typeof(HIS.Desktop.Plugins.TreatmentInspection.TreatmentInspection.UCTreatmentInspection).Assembly);
                ////Gan gia tri cho cac control editor co Text/Caption/NullText/NullValuePrompt/FindNullPrompt

                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.layoutControl1.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.layoutControl2.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.layoutControl3.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.txtTreatmentCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.txtTreatmentCode.Properties.NullValuePrompt", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtStorageCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.txtStorageCode.Properties.NullValuePrompt", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.txtKeyword.Properties.NullValuePrompt", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.layoutControlItem10.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.layoutControlItem10.OptionsToolTip.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnUnSave.Text = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.btnUnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDuyetGiamDinh.Text = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.btnDuyetGiamDinh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.gridColumn1.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.gridColumn2.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.gridColumn3.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.gridColumn4.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.gridColumn5.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.gridColumn6.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.gridColumn7.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.gridColumn8.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.gridColumn9.Caption", ResourceLanguageManager.LanguageResource,
LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.gridColumn10.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.gridColumn11.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.gridColumn12.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.gridColumn13.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.gridColumn13.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.gridColumn14.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.gridColumn14.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.gridColumn15.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.gridColumn15.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.gridColumn16.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.gridColumn17.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn18.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.gridColumn18.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn19.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.gridColumn19.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn20.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.gridColumn20.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn21.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.gridColumn21.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn22.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.gridColumn22.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn23.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.gridColumn23.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn24.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentInspection.gridColumn24.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                if (this.currentModule != null && !String.IsNullOrEmpty(this.currentModule.text))
                {
                    this.Text = this.currentModule.text;
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
                btnUnSave.Enabled = false;
                btnDuyetGiamDinh.Enabled = false;

                txtTreatmentCode.Text = "";
                txtStorageCode.Text = "";
                txtKeyWord.Text = "";

                dtStorageDateFrom.EditValue = null;
                dtStorageDateTo.EditValue = null;
                dtInTimeFrom.EditValue = null;
                dtInTimeTo.EditValue = null;
                dtOutTimeFrom.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                dtOutTimeTo.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                cboDepartmentLast.EditValue = null;
                cboDepartmentEnd.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboDepartment()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 200, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboDepartmentLast, listDepartment, controlEditorADO);
                ControlEditorLoader.Load(cboDepartmentEnd, listDepartment, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboStatus()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TrangThai", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TrangThai", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboStatus, listTrangThai, controlEditorADO);

                cboStatus.EditValue = (long)3;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }



        private void GetDataCombo()
        {
            try
            {
                listDepartment = new List<HIS_DEPARTMENT>();
                listTrangThai = new List<TrangThaiADO>();

                listDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_CLINICAL == 1).ToList();

                listTrangThai.Add(new TrangThaiADO(1, "Chưa kết thúc"));
                listTrangThai.Add(new TrangThaiADO(2, "Đã kết thúc, chưa lưu bệnh án"));
                listTrangThai.Add(new TrangThaiADO(3, "Đã lưu bệnh án, chưa duyệt"));
                listTrangThai.Add(new TrangThaiADO(4, "Duyệt"));
                listTrangThai.Add(new TrangThaiADO(5, "Từ chối duyệt"));

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
                btnUnSave.Enabled = false;
                btnDuyetGiamDinh.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnUnSave_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                List<V_HIS_TREATMENT_11> listTreatment = new List<V_HIS_TREATMENT_11>();
                string ErrorMediRecordId = "", ErrorRecordInspectionSttId = "";
                var rowHandles = gridView1.GetSelectedRows();
                if (rowHandles != null && rowHandles.Count() > 0)
                {
                    foreach (var i in rowHandles)
                    {
                        var row = (V_HIS_TREATMENT_11)gridView1.GetRow(i);
                        if (row != null)
                        {
                            if (row.MEDI_RECORD_ID == null)
                            {
                                ErrorMediRecordId += " " + row.TREATMENT_CODE + ",";
                            }
                            if (row.RECORD_INSPECTION_STT_ID == 1)
                            {
                                ErrorRecordInspectionSttId += " " + row.TREATMENT_CODE + ",";
                            }

                            listTreatment.Add(row);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(ErrorMediRecordId))
                {
                    ErrorMediRecordId = ErrorMediRecordId.Substring(0, ErrorMediRecordId.Length - 1);
                    WaitingManager.Hide();

                    DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.ChuaLuutuBenhAn, ErrorMediRecordId));
                    return;
                }

                if (!string.IsNullOrEmpty(ErrorRecordInspectionSttId))
                {
                    ErrorRecordInspectionSttId = ErrorRecordInspectionSttId.Substring(0, ErrorRecordInspectionSttId.Length - 1);
                    WaitingManager.Hide();

                    DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.DaDuyetGiamDinh, ErrorRecordInspectionSttId));
                    return;
                }

                if (listTreatment != null && listTreatment.Count > 0)
                {
                    List<long> lstTreatmentId = listTreatment.Select(o => o.ID).Distinct().ToList();

                    var result = new BackendAdapter(param).Post<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_OUT_OF_MEDI_RECORD_LIST, ApiConsumers.MosConsumer, lstTreatmentId, param);
                    if (result != null && result.Count > 0)
                    {
                        success = true;
                        FillDataToGrid();
                    }
                }
                WaitingManager.Hide();
                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDuyetGiamDinh_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                List<V_HIS_TREATMENT_11> listTreatment = new List<V_HIS_TREATMENT_11>();
                string ErrorMediRecordId = "", ErrorRecordInspectionSttId = "";
                var rowHandles = gridView1.GetSelectedRows();
                if (rowHandles != null && rowHandles.Count() > 0)
                {
                    foreach (var i in rowHandles)
                    {
                        var row = (V_HIS_TREATMENT_11)gridView1.GetRow(i);
                        if (row != null)
                        {
                            if (row.MEDI_RECORD_ID == null)
                            {
                                ErrorMediRecordId += " " + row.TREATMENT_CODE + ",";
                            }
                            if (row.RECORD_INSPECTION_STT_ID == 1)
                            {
                                ErrorRecordInspectionSttId += " " + row.TREATMENT_CODE + ",";
                            }

                            listTreatment.Add(row);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(ErrorMediRecordId))
                {
                    ErrorMediRecordId = ErrorMediRecordId.Substring(0, ErrorMediRecordId.Length - 1);
                    WaitingManager.Hide();

                    DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.ChuaLuutuBenhAn, ErrorMediRecordId));
                    return;
                }

                if (!string.IsNullOrEmpty(ErrorRecordInspectionSttId))
                {
                    ErrorRecordInspectionSttId = ErrorRecordInspectionSttId.Substring(0, ErrorRecordInspectionSttId.Length - 1);
                    WaitingManager.Hide();

                    DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.DaDuyetGiamDinh, ErrorRecordInspectionSttId));
                    return;
                }

                if (listTreatment != null && listTreatment.Count > 0)
                {
                    List<long> lstTreatmentId = listTreatment.Select(o => o.ID).Distinct().ToList();

                    var result = new BackendAdapter(param).Post<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_RECORD_INSPECTION_APPROVE, ApiConsumers.MosConsumer, lstTreatmentId, param);
                    if (result != null && result.Count > 0)
                    {
                        success = true;
                        FillDataToGrid();
                    }
                }

                WaitingManager.Hide();
                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

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

        private void bbtnDuyetGiamDinh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnDuyetGiamDinh.Enabled)
                {
                    btnDuyetGiamDinh_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnGView_ButtonClick(V_HIS_TREATMENT_11 data)
        {
            try
            {
                var row = data;
                if (row != null)
                {
                    // popup yêu cầu xem
                    List<object> _listObj = new List<object>();
                    _listObj.Add(row.TREATMENT_CODE);
                    _listObj.Add(false);
                    _listObj.Add((HIS.Desktop.Common.RefeshReference)FillDataToGrid);
                    // _listObj.Add((HIS.Desktop.Common.DelegateRefreshData)FillDataToGrid);

                    CommonParam param = new CommonParam();
                    bool success = false;
                    var result = new BackendAdapter(param).Post<List<HIS_TREATMENT>>("api/HisTreatment/DocumentViewCount", ApiConsumers.MosConsumer, data.ID, param);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data.ID), data.ID));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));


                    if (result != null)
                    {
                        //data.DOCUMENT_VIEW_COUNT += 1;
                        //gridControl1.BeginUpdate();
                        //gridView1.RefreshData();
                        //gridControl1.RefreshDataSource();
                        //gridControl1.EndUpdate();
                    }


                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.EmrDocument", currentModule.RoomId, currentModule.RoomTypeId, _listObj);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnGDelete_ButtonClick(V_HIS_TREATMENT_11 data)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowData = data;
                if (rowData != null)
                {
                    bool success = false;
                    List<long> lstTreatmentId = new List<long>();
                    lstTreatmentId.Add(rowData.ID);

                    var result = new BackendAdapter(param).Post<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_OUT_OF_MEDI_RECORD_LIST, ApiConsumers.MosConsumer, lstTreatmentId, param);
                    if (result != null && result.Count > 0)
                    {
                        success = true;
                        FillDataToGrid();
                    }
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGDuyet_ButtonClick(V_HIS_TREATMENT_11 data)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowData = data;
                if (rowData != null)
                {
                    bool success = false;
                    List<long> lstTreatmentId = new List<long>();
                    lstTreatmentId.Add(rowData.ID);

                    var result = new BackendAdapter(param).Post<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_RECORD_INSPECTION_APPROVE, ApiConsumers.MosConsumer, lstTreatmentId, param);
                    if (result != null)
                    {
                        success = true;
                        FillDataToGrid();
                    }
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnGHuyDuyet_ButtonClick(V_HIS_TREATMENT_11 data)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowData = data;
                if (rowData != null)
                {
                    bool success = false;

                    var result = new BackendAdapter(param).Post<HIS_TREATMENT>(HisRequestUriStore.HIS_TREATMENT_RECORD_INSPECTION_UN_APPROVE, ApiConsumers.MosConsumer, rowData.ID, param);
                    if (result != null)
                    {
                        success = true;
                        FillDataToGrid();
                    }
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnGTuChoiDuyet_ButtonClick(V_HIS_TREATMENT_11 data)
        {
            try
            {
                var rowData = data;
                if (rowData != null)
                {
                    frmRefuseApprove RefuseApprove = new frmRefuseApprove(rowData.ID);
                    RefuseApprove.ShowDialog();
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnGHuyTuChoi_ButtonClick(V_HIS_TREATMENT_11 data)
        {
            try
            {
                var rowData = data;
                if (rowData != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();

                    var result = new BackendAdapter(param).Post<HIS_TREATMENT>(HisRequestUriStore.HIS_TREATMENT_RECORD_INSPECTION_UN_REJECT, ApiConsumers.MosConsumer, rowData.ID, param);
                    if (result != null)
                    {
                        success = true;
                        FillDataToGrid();
                    }

                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HIS_TREATMENT_11)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + start;
                        }
                        else if (e.Column.FieldName == "Status")
                        {
                            if (data.MEDI_RECORD_ID == null)
                            {
                                e.Value = imageList1.Images[0];
                            }
                            else if (data.RECORD_INSPECTION_STT_ID == null)
                            {
                                e.Value = imageList1.Images[1];
                            }
                            else if (data.RECORD_INSPECTION_STT_ID == 1)
                            {
                                e.Value = imageList1.Images[2];
                            }
                            else if (data.RECORD_INSPECTION_STT_ID == 2)
                            {
                                e.Value = imageList1.Images[3];
                            }
                        }
                        else if (e.Column.FieldName == "TDL_PATIENT_DOB_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                            if (data.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                            {
                                e.Value = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                            }
                        }
                        else if (e.Column.FieldName == "IN_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IN_TIME);
                        }
                        else if (e.Column.FieldName == "OUT_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.OUT_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MR_STORE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MR_STORE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "ICD")
                        {
                            e.Value = data.ICD_CODE + " - " + data.ICD_NAME;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControl1)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControl1.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;
                            V_HIS_TREATMENT_11 dataRow = (V_HIS_TREATMENT_11)((IList)((BaseView)gridView1).DataSource)[info.RowHandle];
                            string text = "";

                            if (info.Column.FieldName == "Status")
                            {
                                if (dataRow.MEDI_RECORD_ID == null)
                                {
                                    text = ResourceMessage.MauTrang;
                                }
                                else if (dataRow.RECORD_INSPECTION_STT_ID == null)
                                {
                                    text = ResourceMessage.Mauvang;
                                }
                                else if (dataRow.RECORD_INSPECTION_STT_ID == 1)
                                {
                                    text = ResourceMessage.MauXanh;
                                }
                                else if (dataRow.RECORD_INSPECTION_STT_ID == 2)
                                {
                                    text = ResourceMessage.MauDo;
                                }
                            }
                            lastInfo = new DevExpress.Utils.ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    V_HIS_TREATMENT_11 data = (V_HIS_TREATMENT_11)((IList)((BaseView)sender).DataSource)[e.RowHandle];

                    if (e.Column.FieldName == "DELETE")
                    {
                        if (data.RECORD_INSPECTION_STT_ID == 2 || data.RECORD_INSPECTION_STT_ID == null)
                        {
                            e.RepositoryItem = btnGDelete;
                        }
                        else
                        {
                            e.RepositoryItem = btnGEnable;
                        }
                    }
                    else if (e.Column.FieldName == "DUYET")
                    {
                        if (data.RECORD_INSPECTION_STT_ID == null && data.MEDI_RECORD_ID != null)
                        {
                            e.RepositoryItem = btnGDuyet;
                        }
                        else if (data.RECORD_INSPECTION_STT_ID == 1)
                        {
                            e.RepositoryItem = btnGHuyDuyet;
                        }
                        else
                        {
                            e.RepositoryItem = btnGDuyet_Disable;
                        }
                    }
                    else if (e.Column.FieldName == "TUCHOI")
                    {
                        if (data.RECORD_INSPECTION_STT_ID == null && data.MEDI_RECORD_ID != null)
                        {
                            e.RepositoryItem = btnGTuChoiDuyet;
                        }
                        else if (data.RECORD_INSPECTION_STT_ID == 2)
                        {
                            e.RepositoryItem = btnGHuyTuChoi;
                        }
                        else
                        {
                            e.RepositoryItem = btnGTuChoi_Disable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                if (gridView1.GetSelectedRows().Count() > 0)
                {
                    btnUnSave.Enabled = true;
                    btnDuyetGiamDinh.Enabled = true;
                }
                else
                {
                    btnUnSave.Enabled = false;
                    btnDuyetGiamDinh.Enabled = false;
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
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtStorageCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (hi.InRowCell)
                    {
                        var treatmentData = (V_HIS_TREATMENT_11)view.GetRow(hi.RowHandle);

                        if (hi.Column.FieldName == "VIEW")
                        {
                            btnGView_ButtonClick(treatmentData);
                        }
                        else if (hi.Column.FieldName == "DELETE")
                        {
                            if (treatmentData.RECORD_INSPECTION_STT_ID == 2 || treatmentData.RECORD_INSPECTION_STT_ID == null)
                            {
                                btnGDelete_ButtonClick(treatmentData);
                            }
                        }
                        else if (hi.Column.FieldName == "DUYET")
                        {
                            if (treatmentData.RECORD_INSPECTION_STT_ID == null && treatmentData.MEDI_RECORD_ID != null)
                            {
                                btnGDuyet_ButtonClick(treatmentData);
                            }
                            else if (treatmentData.RECORD_INSPECTION_STT_ID == 1)
                            {
                                btnGHuyDuyet_ButtonClick(treatmentData);
                            }
                        }
                        else if (hi.Column.FieldName == "TUCHOI")
                        {
                            if (treatmentData.RECORD_INSPECTION_STT_ID == null && treatmentData.MEDI_RECORD_ID != null)
                            {
                                btnGTuChoiDuyet_ButtonClick(treatmentData);
                            }
                            else if (treatmentData.RECORD_INSPECTION_STT_ID == 2)
                            {
                                btnGHuyTuChoi_ButtonClick(treatmentData);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboStatus_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboStatus.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartmentLast_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDepartmentLast.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartmentEnd_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDepartmentEnd.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartmentLast_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboDepartmentLast.EditValue != null)
                {
                    cboDepartmentLast.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboDepartmentLast.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartmentEnd_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboDepartmentEnd.EditValue != null)
                {
                    cboDepartmentEnd.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboDepartmentEnd.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboStatus_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboStatus.EditValue != null)
                {
                    cboStatus.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboStatus.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPATIENTCODE_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            try
            {

                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
    }
}
