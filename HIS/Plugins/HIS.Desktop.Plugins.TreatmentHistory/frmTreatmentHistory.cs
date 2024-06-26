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
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.TreatmentHistory.Resources;
using HIS.Desktop.Utility;
using HIS.UC.TreeSereServ7;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentHistory
{
    public partial class frmTreatmentHistory : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        internal TreatmentHistoryADO currentInput;

        int rowCount = 0;
        int dataTotal = 0;
        public PagingGrid pagingGrid;

        UserControl ucSereServ;
        TreeSereServ7Processor treeSereServ7Processor;

        int lastRowHandle = -1;
        GridColumn lastColumn = null;
        ToolTipControlInfo lastInfo = null;

        L_HIS_TREATMENT rowCellClick { get; set; }

        HIS_SERVICE_REQ serviceReq2Focus { get; set; }

        List<HIS_SERVICE_REQ> listServiceReq_CurrentTreatment = null;

        public frmTreatmentHistory()
        {
            InitializeComponent();
        }

        public frmTreatmentHistory(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmTreatmentHistory(Inventec.Desktop.Common.Modules.Module currentModule, TreatmentHistoryADO currentInput)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this.currentInput = currentInput;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmTreatmentHistory_Load(object sender, EventArgs e)
        {
            try
            {
                SetIconFrm();
                LoadDataToCombo();
                LoadDataToComboStatus();
                SetCaptionByLanguageKey();
                Base.ResourceLangManager.InitResourceLanguageManager();
                SetValueDefault();
                InitUcSereServ();
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }

                if (this.currentInput != null)
                {
                    txtTreatmentCode.Text = this.currentInput.treatment_code;
                    txtPatientCode.Text = this.currentInput.patient_code;
                }
                LoadDataGridTreatment5();
                gridControlHisTreatment5.ToolTipController = toolTipController;
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.TreatmentHistory.Resources.Lang", typeof(HIS.Desktop.Plugins.TreatmentHistory.frmTreatmentHistory).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.btnVienPhi.Text = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.btnVienPhi.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.btnCloseLeaft.Text = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.btnCloseLeaft.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.btnOpenLeaft.Text = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.btnOpenLeaft.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColDepartment.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.treeListColDepartment.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.treeListColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.treeListColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.treeListColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColSTT.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.gridColSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColStatus.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.gridColStatus.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColTreatmentCode.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.gridColTreatmentCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColPatientCode.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.gridColPatientCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColPatientName.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.gridColPatientName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColDOB.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.gridColDOB.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColGenderName.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.gridColGenderName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColIcdMain.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.gridColIcdMain.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColIcdText.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.gridColIcdText.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColInTime.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.gridColInTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColOutTime.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.gridColOutTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.txtKeyWord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtPatientCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.txtPatientCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtTreatmentCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.txtTreatmentCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButton__F1.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.barButton__F1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButton__F2.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.barButton__F2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButton__F3.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.barButton__F3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButton__CrtlF.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.barButton__CrtlF.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToCombo()
        {
            try
            {
                //List<HIS.Desktop.Plugins.TreatmentHistory.ComboADO> status = new List<HIS.Desktop.Plugins.TreatmentHistory.ComboADO>();
                //status.Add(new HIS.Desktop.Plugins.TreatmentHistory.ComboADO(0, ResourceMessage.TatCa));
                //status.Add(new HIS.Desktop.Plugins.TreatmentHistory.ComboADO(1, ResourceMessage.ChuaDay));
                //status.Add(new HIS.Desktop.Plugins.TreatmentHistory.ComboADO(2, ResourceMessage.DaDay));

                //List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("statusName", ResourceMessage.TrangThai, 50, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, true, 50);
                //ControlEditorLoader.Load(cboFind, status, controlEditorADO);

                //cboFind.EditValue = status[0].id;
                //cboFind.Properties.Buttons[1].Visible = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToComboStatus()
        {
            try
            {
                List<HIS.Desktop.Plugins.TreatmentHistory.ComboADO> status = new List<HIS.Desktop.Plugins.TreatmentHistory.ComboADO>();
                status.Add(new HIS.Desktop.Plugins.TreatmentHistory.ComboADO(5, ResourceMessage.TatCa));
                status.Add(new HIS.Desktop.Plugins.TreatmentHistory.ComboADO(0, ResourceMessage.DangDieuTri));
                status.Add(new HIS.Desktop.Plugins.TreatmentHistory.ComboADO(1, ResourceMessage.DaKetThucDieuTri));
                status.Add(new HIS.Desktop.Plugins.TreatmentHistory.ComboADO(2, ResourceMessage.DaDuyetKhoaTaiChinh));
                status.Add(new HIS.Desktop.Plugins.TreatmentHistory.ComboADO(3, ResourceMessage.DaDuyetKhoaBaoHiem));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", ResourceMessage.TrangThai, 50, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, true, 50);
                ControlEditorLoader.Load(cboStatus, status, controlEditorADO);

                cboStatus.EditValue = status[0].id;
                cboStatus.Properties.Buttons[1].Visible = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetValueDefault()
        {
            try
            {
                txtKeyWord.Text = "";
                txtPatientCode.Text = "";
                txtTreatmentCode.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcSereServ()
        {
            try
            {
                treeSereServ7Processor = new TreeSereServ7Processor();
                TreeSereServ7ADO ado = new TreeSereServ7ADO();
                ado.SelectImageCollection = this.imageCollection1;
                ado.StateImageCollection = this.imageCollection1;
                ado.TreeSereServ7_GetStateImage = treeSereServ_GetStateImage;
                ado.TreeSereServ7_StateImageClick = treeSereServ_StateImageClick;
                ado.TreeSereServ7_GetSelectImage = treeSereServ_GetSelectImage;
                ado.TreeSereServ7_CustomNodeCellEdit = treeSereServ_CustomNodeCellEdit;
                ado.SereServNodeCellStyle = treeSereServ_NodeCellStyle;
                ado.IsShowSearchPanel = false;
                ado.DepartmentID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId) != null ? HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentId : 0;
                ado.TreeSereServ7Columns = new List<TreeSereServ7Column>();
                //ado.TreeSereServ7_CustomDrawNodeCell = treeSereServ_CustomDrawNodeCell;

                //Sửa yêu cầu
                //TreeSereServ7Column colEditServiceReq = new TreeSereServ7Column("   ", "EditServiceReq", 30, true);
                //colEditServiceReq.VisibleIndex = 0;
                //ado.TreeSereServ7Columns.Add(colEditServiceReq);

                //Column btn
                TreeSereServ7Column serviceBtn = new TreeSereServ7Column("   ", "SendTestServiceReq", 30, true);
                serviceBtn.VisibleIndex = 1;
                ado.TreeSereServ7Columns.Add(serviceBtn);

                //Column mã dịch vụ
                TreeSereServ7Column serviceCodeCol = new TreeSereServ7Column(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TREATMENT_HISTORY__TREE_SERE_SERV__COLUMN_SERVICE_CODE", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_CODE", 150, false);
                serviceCodeCol.VisibleIndex = 2;
                ado.TreeSereServ7Columns.Add(serviceCodeCol);

                //Column tên dịch vụ
                TreeSereServ7Column serviceNameCol = new TreeSereServ7Column(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TREATMENT_HISTORY__TREE_SERE_SERV__COLUMN_SERVICE_NAME", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_NAME", 370, false);
                serviceNameCol.VisibleIndex = 3;
                ado.TreeSereServ7Columns.Add(serviceNameCol);

                //Column mã yêu cầu
                TreeSereServ7Column serviceReqCodeCol = new TreeSereServ7Column(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TREATMENT_HISTORY__TREE_SERE_SERV__COLUMN_SERVICE_REQ_CODE", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_REQ_CODE", 100, false);
                serviceReqCodeCol.VisibleIndex = 4;
                ado.TreeSereServ7Columns.Add(serviceReqCodeCol);

                //Column ghi chú
                TreeSereServ7Column noteCol = new TreeSereServ7Column(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TREATMENT_HISTORY__TREE_SERE_SERV__COLUMN_NOTE", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "NOTE_ADO", 250, false);
                noteCol.VisibleIndex = 5;
                ado.TreeSereServ7Columns.Add(noteCol);

                this.ucSereServ = (UserControl)treeSereServ7Processor.Run(ado);
                if (this.ucSereServ != null)
                {
                    this.panelControlTreeSere7.Controls.Add(this.ucSereServ);
                    this.ucSereServ.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Event GridTreatment
        private void LoadDataGridTreatment5()
        {
            try
            {
                int pageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }
                ucPagingTreatment5(new CommonParam(0, pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(ucPagingTreatment5, param, pageSize, gridControlHisTreatment5);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        int startPage = 0;
        private void ucPagingTreatment5(object param)
        {
            try
            {
                WaitingManager.Show();
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.L_HIS_TREATMENT>> apiResult = new ApiResultObject<List<MOS.EFMODEL.DataModels.L_HIS_TREATMENT>>();
                // MOS.Filter.HisTreatmentFilter filter = new MOS.Filter.HisTreatmentFilter();
                MOS.Filter.HisTreatmentLViewFilter filter = new HisTreatmentLViewFilter();
                if (this.currentInput != null)
                {
                    if (currentInput.treatmentId > 0 && currentInput.treatment_code != null)
                    {
                        filter.ID = currentInput.treatmentId;
                        filter.TREATMENT_CODE__EXACT = currentInput.treatment_code;
                    }
                    else if (currentInput.patientId > 0 && currentInput.patient_code != null)
                    {
                        // filter.PATIENT_ID = currentInput.patientId;
                        filter.PATIENT_CODE__EXACT = currentInput.patient_code;
                    }
                }
                else // mở từ menu
                {
                    filter.KEY_WORD = txtKeyWord.Text.Trim();
                    if (!String.IsNullOrEmpty(txtTreatmentCode.Text))
                    {
                        string code = txtTreatmentCode.Text.Trim();
                        if (code.Length < 12)
                        {
                            code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        }
                        filter.TREATMENT_CODE__EXACT = code;
                        txtTreatmentCode.Text = code;
                    }
                    if (!String.IsNullOrEmpty(txtPatientCode.Text))
                    {
                        string code = txtPatientCode.Text.Trim();
                        if (code.Length < 10)
                        {
                            code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        }
                        filter.PATIENT_CODE__EXACT = code;
                        txtPatientCode.Text = code;
                    }
                }

                //if (cboFind.EditValue != null)
                //{
                //    //Chưa đấy
                //    if ((long)cboFind.EditValue == 1)
                //    {
                //        filter.IS_YDT_UPLOAD = false;
                //    }
                //    //Đã đẩy
                //    else if ((long)cboFind.EditValue == 2)
                //    {
                //        filter.IS_YDT_UPLOAD = true;
                //    }
                //}
                if (cboStatus.EditValue != null)
                {
                    if ((long)cboStatus.EditValue == 0)//DangDieuTri
                    {
                        filter.IS_PAUSE = false;
                    }
                    else if ((long)cboStatus.EditValue == 1)//DaKetThuc
                    {
                        filter.IS_PAUSE = true;
                    }
                    else if ((long)cboStatus.EditValue == 2)//DaDuyetKhoaTaiChinh
                    {
                        filter.IS_ACTIVE = 0;
                        filter.IS_LOCK_HEIN = false;
                    }
                    else if ((long)cboStatus.EditValue == 3)//DaDuyetKhoaBaoHiem
                    {
                        filter.IS_LOCK_HEIN = true;
                    }
                }

                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                //Inventec.Common.Logging.LogSystem.Error("Get HIS_TREATMENT >>>>>>>>1<<<<<<<<<: ");
                apiResult = new BackendAdapter(paramCommon).GetRO<List<L_HIS_TREATMENT>>("api/HisTreatment/GetLView", ApiConsumers.MosConsumer, filter, paramCommon);
                //Inventec.Common.Logging.LogSystem.Error("Get HIS_TREATMENT >>>>>>>>2<<<<<<<<<: ");
                gridControlHisTreatment5.DataSource = null;
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gridControlHisTreatment5.BeginUpdate();
                        gridControlHisTreatment5.DataSource = data;
                        gridControlHisTreatment5.EndUpdate();
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);


                        #region --- Check personCode ---
                        //if (!string.IsNullOrEmpty(filter.TDL_PATIENT_CODE__EXACT) || !string.IsNullOrEmpty(filter.TREATMENT_CODE__EXACT))
                        //{
                        //    MOS.Filter.HisPatientFilter _patientFilter = new HisPatientFilter();
                        //    //_patientFilter.PATIENT_CODE__EXACT = filter.TDL_PATIENT_CODE__EXACT;
                        //    _patientFilter.ID = data[0].PATIENT_ID;
                        //    var patients = new BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GET, ApiConsumers.MosConsumer, _patientFilter, null);
                        //    if (patients != null && patients.Count > 0 && !string.IsNullOrEmpty(patients[0].PATIENT_CODE))
                        //        btnYBaDienTu.Enabled = true;
                        //}

                        #endregion

                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewHisTreatment5_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.L_HIS_TREATMENT data = (MOS.EFMODEL.DataModels.L_HIS_TREATMENT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "IN_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IN_TIME);
                        }
                        else if (e.Column.FieldName == "OUT_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.OUT_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "DOB_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                            //e.Value = HIS.Desktop.Utility.AgeHelper.CalculateAgeFromYear(data.DOB) + "  tuổi";
                        }
                        else if (e.Column.FieldName == "ICD_DISPLAY")
                        {
                            if (!string.IsNullOrEmpty(data.ICD_CODE))
                            {
                                if (!String.IsNullOrEmpty(data.ICD_NAME))
                                {
                                    e.Value = data.ICD_CODE + " - " + data.ICD_NAME;
                                }
                            }
                        }
                        else if (e.Column.FieldName == "ICD_TEXT_DISPLAY")
                        {
                            if (!String.IsNullOrEmpty(data.ICD_TEXT))
                            {
                                e.Value = data.ICD_SUB_CODE + " - " + data.ICD_TEXT;
                            }
                        }
                        else if (e.Column.FieldName == "STATUS_DISPLAY")
                        {
                            #region --- STATUS ---
                            short status_ispause = Inventec.Common.TypeConvert.Parse.ToInt16((data.IS_PAUSE ?? -1).ToString());
                            decimal status_islock = Inventec.Common.TypeConvert.Parse.ToDecimal((data.IS_ACTIVE ?? -1).ToString());
                            short status_islockhein = Inventec.Common.TypeConvert.Parse.ToInt16((data.IS_LOCK_HEIN ?? -1).ToString());


                            //Status
                            //1- dang dieu tri
                            //2- da ket thuc
                            //3- khóa hồ sơ
                            //4- duyệt bhyt
                            if (status_islockhein != 1)
                            {
                                if (status_islock == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                {
                                    if (status_ispause != 1)
                                    {
                                        e.Value = imageList1.Images[0];
                                    }
                                    else
                                    {
                                        e.Value = imageList1.Images[1];
                                    }
                                }
                                else
                                {
                                    e.Value = imageList1.Images[2];
                                }
                            }
                            else
                            {
                                e.Value = imageList1.Images[3];
                            }
                            #endregion
                        }
                        //else if (e.Column.FieldName == "YDT_DISPLAY")
                        //{
                        //    if (data.IS_YDT_UPLOAD == 1)
                        //        e.Value = imageList1.Images[4];
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewHisTreatment5_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                rowCellClick = new L_HIS_TREATMENT();
                rowCellClick = (L_HIS_TREATMENT)gridViewHisTreatment5.GetFocusedRow();
                gridViewHisTreatment5.OptionsSelection.EnableAppearanceFocusedCell = true;
                gridViewHisTreatment5.OptionsSelection.EnableAppearanceFocusedRow = true;
                if (rowCellClick != null)
                {
                    //MessageBox.Show(rowCellClick.TREATMENT_CODE + "");
                    LoadDataTreeServiceReq2(this, rowCellClick);
                }
                if (ucSereServ != null)
                {
                    treeSereServ7Processor.Reload(ucSereServ, new List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_7>());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Event Tree ServiceReq
        private void LoadDataTreeServiceReq2(frmTreatmentHistory control, L_HIS_TREATMENT treatment )
        {
            try
            {
                if (treatment.ID > 0)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();

                    MOS.Filter.HisDepartmentTranViewFilter DepartmentTranFilter = new MOS.Filter.HisDepartmentTranViewFilter();
                    DepartmentTranFilter.TREATMENT_ID = treatment.ID;
                    DepartmentTranFilter.ORDER_DIRECTION = "ASC";
                    DepartmentTranFilter.ORDER_FIELD = "DEPARTMENT_IN_TIME";
                    var currentDepartmentTran = new BackendAdapter(param).Get<List<V_HIS_DEPARTMENT_TRAN>>(HisRequestUriStore.HIS_DEPARTMENT_TRAN_GETVIEW, ApiConsumers.MosConsumer, DepartmentTranFilter, param);

                    if (currentDepartmentTran != null && currentDepartmentTran.Count > 0)
                    {
                        currentDepartmentTran.OrderBy(o => o.DEPARTMENT_IN_TIME ?? 99999999999999).ThenBy(o => o.ID).ToList();

                        Inventec.Common.Logging.LogSystem.Info("currentDepartmentTran: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentDepartmentTran), currentDepartmentTran));

                        MOS.Filter.HisServiceReqFilter serviceReqFilter = new MOS.Filter.HisServiceReqFilter();
                        serviceReqFilter.TREATMENT_ID = treatment.ID;
                        this.listServiceReq_CurrentTreatment = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, serviceReqFilter, param);

                        tree_HisServiceReq2.Nodes.Clear();
                        for (int i = 0; i < currentDepartmentTran.Count; i++)
                        {
                            if (listServiceReq_CurrentTreatment != null && listServiceReq_CurrentTreatment.Count > 0)
                            {
                                long? DepartmentOutTime = null;
                                string Status = "";
                                //DateTime? D_OutTime = null, D_InTime = null;
                                string D_OutTime = "", D_InTime = "";
                                List<HIS_SERVICE_REQ> lstServiceReq = new List<HIS_SERVICE_REQ>();


                                lstServiceReq = listServiceReq_CurrentTreatment.Where(o => o.REQUEST_DEPARTMENT_ID == currentDepartmentTran[i].DEPARTMENT_ID && o.INTRUCTION_TIME >= (currentDepartmentTran[i].DEPARTMENT_IN_TIME ?? 99999999999999)).ToList();

                                if (i == currentDepartmentTran.Count - 1)
                                {
                                    if (rowCellClick != null && rowCellClick.OUT_TIME != null)
                                    {
                                        DepartmentOutTime = rowCellClick.OUT_TIME;
                                        Status = rowCellClick.TREATMENT_END_TYPE_NAME;

                                        if (treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN)
                                        {
                                            Status = rowCellClick.TREATMENT_END_TYPE_NAME + "(" + (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(rowCellClick.APPOINTMENT_TIME ?? 0) ?? DateTime.Now).ToString("dd/MM/yyyy") + ")";


                                        }
                                    }
                                }
                                else if (i + 1 < currentDepartmentTran.Count)
                                {
                                    if (currentDepartmentTran[i + 1].DEPARTMENT_IN_TIME.HasValue)
                                    {
                                        lstServiceReq = lstServiceReq.Where(o => o.INTRUCTION_TIME < (currentDepartmentTran[i + 1].DEPARTMENT_IN_TIME ?? 0)).ToList();
                                    }
                                    DepartmentOutTime = currentDepartmentTran[i + 1].DEPARTMENT_IN_TIME;
                                    Status = String.Format(ResourceMessage.ChuyenSangKhoa, currentDepartmentTran[i + 1].DEPARTMENT_NAME);
                                }

                                if (i == 0)
                                {
                                    var ServiceReqFirst = listServiceReq_CurrentTreatment.OrderBy(o => o.INTRUCTION_TIME).FirstOrDefault();

                                    if (ServiceReqFirst.INTRUCTION_TIME < currentDepartmentTran[i].DEPARTMENT_IN_TIME)
                                    {
                                        DepartmentOutTime = ServiceReqFirst.INTRUCTION_TIME;

                                        if (!lstServiceReq.Any(o => o.ID == ServiceReqFirst.ID))
                                        {
                                            lstServiceReq.Add(ServiceReqFirst);
                                        }
                                    }

                                }


                                D_InTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(currentDepartmentTran[i].DEPARTMENT_IN_TIME ?? 0);
                                D_OutTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(DepartmentOutTime ?? 0);

                                TreeListNode parentForRootNodes = null;
                                TreeListNode rootNode = tree_HisServiceReq2.AppendNode(
                             new object[] { currentDepartmentTran[i].DEPARTMENT_NAME, null, null, null, D_InTime, D_OutTime, Status },
                             parentForRootNodes, null);
                                if (lstServiceReq != null && lstServiceReq.Count > 0)
                                {
                                    CreateChildNode(rootNode, lstServiceReq, control);
                                }
                            }
                            else
                            {
                                tree_HisServiceReq2.Nodes.Clear();
                            }

                            //var lstParent = currentServiceReq.GroupBy(p => p.REQUEST_DEPARTMENT_ID).Select(grc => grc.First()).ToList();
                            //if (lstParent != null && lstParent.Count > 0)
                            //{
                            //    tree_HisServiceReq2.Nodes.Clear();
                            //    TreeListNode parentForRootNodes = null;
                            //    foreach (var item in lstParent)
                            //    {
                            //        TreeListNode rootNode = tree_HisServiceReq2.AppendNode(
                            //     new object[] { item.REQUEST_DEPARTMENT_NAME, null, null, null, null, null, null },
                            //     parentForRootNodes, null);
                            //        CreateChildNode(rootNode, item, currentServiceReq, control);
                            //    }
                            //}

                        }
                    }
                    else
                    {
                        tree_HisServiceReq2.Nodes.Clear();
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal static void CreateChildNode(TreeListNode rootNode, List<HIS_SERVICE_REQ> listChilds, frmTreatmentHistory control)
        {
            try
            {
                //var listChilds = lstHisServiceReq.Where(o => o.REQUEST_DEPARTMENT_ID == hisServiceReq.REQUEST_DEPARTMENT_ID).ToList();
                listChilds = listChilds.GroupBy(p => p.INTRUCTION_DATE).Select(gr => gr.First()).OrderBy(p => p.INTRUCTION_DATE).ToList();
                if (listChilds != null && listChilds.Count > 0)
                {
                    foreach (var itemChild in listChilds)
                    {
                        var intructime = Inventec.Common.DateTime.Convert.TimeNumberToDateString(itemChild.INTRUCTION_DATE.ToString());
                        TreeListNode childNode = control.tree_HisServiceReq2.AppendNode(
                        new object[] { intructime, null, null },
                        rootNode, itemChild);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void tree_HisServiceReq2_Click(object sender, EventArgs e)
        {
            try
            {
                TreeList tree = sender as TreeList;
                TreeListHitInfo hi = tree.CalcHitInfo(tree.PointToClient(Control.MousePosition));
                var data = tree_HisServiceReq2.GetDataRecordByNode(hi.Node);
                if (hi.Node != null)
                {
                    serviceReq2Focus = new HIS_SERVICE_REQ();
                    serviceReq2Focus = (HIS_SERVICE_REQ)hi.Node.Tag;
                    if (serviceReq2Focus != null)
                    {
                        LoadDataSereServ7(serviceReq2Focus);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void LoadDataSereServ7(HIS_SERVICE_REQ currentServiceReq)
        {
            try
            {
                WaitingManager.Show();
                if (currentServiceReq != null)
                {
                    CommonParam param = new CommonParam();

                    DHisSereServ2Filter _sereServ2Filter = new DHisSereServ2Filter();
                    _sereServ2Filter.TREATMENT_ID = currentServiceReq.TREATMENT_ID;
                    _sereServ2Filter.INTRUCTION_DATE = currentServiceReq.INTRUCTION_DATE;
                    var dataNew = new BackendAdapter(param).Get<List<DHisSereServ2>>("api/HisSereServ/GetDHisSereServ2", ApiConsumers.MosConsumer, _sereServ2Filter, param);

                    List<V_HIS_SERE_SERV_7> _sereServ7s = new List<V_HIS_SERE_SERV_7>();
                    if (dataNew != null && dataNew.Count > 0)
                    {
                        foreach (var item in dataNew)
                        {
                            V_HIS_SERE_SERV_7 ado = new V_HIS_SERE_SERV_7();
                            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV_7>(ado, item);
                            ado.TDL_REQUEST_DEPARTMENT_ID = item.REQUEST_DEPARTMENT_ID ?? 0;
                            ado.ID = item.SERE_SERV_ID ?? 0;
                            ado.TDL_SERVICE_CODE = item.SERVICE_CODE;
                            ado.TDL_SERVICE_NAME = item.SERVICE_NAME;
                            ado.TDL_SERVICE_REQ_CODE = item.SERVICE_REQ_CODE;
                            var serviceType = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(p => p.ID == item.TDL_SERVICE_TYPE_ID);
                            ado.SERVICE_TYPE_NAME = serviceType != null ? serviceType.SERVICE_TYPE_NAME : null;
                            ado.SERVICE_TYPE_CODE = serviceType != null ? serviceType.SERVICE_TYPE_CODE : null;
                            _sereServ7s.Add(ado);
                        }
                    }


                    if (_sereServ7s != null && _sereServ7s.Count > 0)
                    {
                        if (ucSereServ != null)
                        {
                            treeSereServ7Processor.Reload(ucSereServ, currentServiceReq.REQUEST_DEPARTMENT_ID, _sereServ7s);
                        }
                    }
                    else
                    {
                        if (ucSereServ != null)
                        {
                            treeSereServ7Processor.Reload(ucSereServ, new List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_7>(),null);
                        }
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButton__F1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtTreatmentCode.Focus();
                txtTreatmentCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButton__F2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtPatientCode.Focus();
                txtPatientCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButton__F3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtKeyWord.Focus();
                txtKeyWord.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButton__CrtlF_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                this.currentInput = null;
                gridViewHisTreatment5.OptionsSelection.EnableAppearanceFocusedCell = false;
                gridViewHisTreatment5.OptionsSelection.EnableAppearanceFocusedRow = false;
                if (ucSereServ != null)
                {
                    treeSereServ7Processor.Reload(ucSereServ, new List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_7>());
                }
                tree_HisServiceReq2.ClearNodes();
                LoadDataGridTreatment5();//ktra lại
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void toolTipController_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlHisTreatment5)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlHisTreatment5.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;

                            string text = "";
                            if (info.Column.FieldName == "STATUS_DISPLAY")
                            {
                                short status_ispause = Inventec.Common.TypeConvert.Parse.ToInt16((view.GetRowCellValue(lastRowHandle, "IS_PAUSE") ?? "-1").ToString());
                                decimal status_islock = Inventec.Common.TypeConvert.Parse.ToDecimal((view.GetRowCellValue(lastRowHandle, "IS_ACTIVE") ?? "-1").ToString());
                                short status_islockhein = Inventec.Common.TypeConvert.Parse.ToInt16((view.GetRowCellValue(lastRowHandle, "IS_LOCK_HEIN") ?? "-1").ToString());
                                //Status
                                //1- dang dieu tri
                                //2- da ket thuc
                                //3- khóa hồ sơ
                                //4- duyệt bhyt
                                if (status_islockhein != 1)
                                {
                                    if (status_islock == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                    {
                                        if (status_ispause != 1)
                                            text = ResourceMessage.DangDieuTri;
                                        else
                                            text = ResourceMessage.DaKetThucDieuTri;
                                    }
                                    else
                                        text = ResourceMessage.DaDuyetKhoaTaiChinh;
                                }
                                else
                                    text = ResourceMessage.DaDuyetKhoaBaoHiem;
                            }
                            else if (info.Column.FieldName == "YDT_DISPLAY")
                            {
                                if (Inventec.Common.TypeConvert.Parse.ToInt16((view.GetRowCellValue(lastRowHandle, "IS_YDT_UPLOAD") ?? "-1").ToString()) == 1)
                                {
                                    text = ResourceMessage.DaDayThongTinLenHeThongYBaDienTu;
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

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtTreatmentCode.Text))
                    {
                        btnSearch.Focus();
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
                    if (!String.IsNullOrEmpty(txtPatientCode.Text))
                    {
                        btnSearch.Focus();
                    }
                    else
                    {
                        txtKeyWord.Focus();
                        txtKeyWord.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton__Send_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                MOS.EFMODEL.DataModels.HIS_SERVICE_REQ serviceReqDTO = new MOS.EFMODEL.DataModels.HIS_SERVICE_REQ();
                var currentSS = (V_HIS_SERE_SERV_7)treeSereServ7Processor.GetValueFocus(ucSereServ);
                if (currentSS != null)
                {
                    var resend = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_TEST_SERVICE_REQ_RESEND, ApiConsumers.MosConsumer, currentSS.SERVICE_REQ_ID, param);
                    if (resend)
                    {
                        success = true;
                    }
                }
                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewHisTreatment5_DoubleClick(object sender, EventArgs e)
        {

        }

        private void btnVienPhi_Click(object sender, EventArgs e)
        {
            try
            {
                rowCellClick = new L_HIS_TREATMENT();
                rowCellClick = (L_HIS_TREATMENT)gridViewHisTreatment5.GetFocusedRow();
                if (rowCellClick != null)
                {
                    WaitingManager.Show();
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrHospitalFees").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.AggrHospitalFees'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(rowCellClick.ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryEditServiceReq__Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var currentSS = (V_HIS_SERE_SERV_7)treeSereServ7Processor.GetValueFocus(ucSereServ);
                if (currentSS != null)
                {
                    WaitingManager.Show();
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignServiceEdit").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignServiceEdit");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        AssignServiceEditADO assignServiceEditADO = new ADO.AssignServiceEditADO(currentSS.SERVICE_REQ_ID ?? 0, currentSS.TDL_INTRUCTION_TIME, RefeshDataByTree);
                        listArgs.Add(assignServiceEditADO);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RefeshDataByTree()
        {
            try
            {
                LoadDataSereServ7(serviceReq2Focus);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit__VienPhi_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                rowCellClick = new L_HIS_TREATMENT();
                rowCellClick = (L_HIS_TREATMENT)gridViewHisTreatment5.GetFocusedRow();
                if (rowCellClick != null)
                {
                    WaitingManager.Show();
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrHospitalFees").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.AggrHospitalFees'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(rowCellClick.ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnYBaDienTu_Click(object sender, EventArgs e)
        {
            try
            {
                //if (!btnYBaDienTu.Enabled)
                //    return;
                //bool IsHID = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("CONFIG_KEY__IS_USE_HID_SYNC") == "1" ? true : false;
                //if (IsHID)
                //{
                //    CommonParam param = new CommonParam();
                //    string mess = "";
                //    var dataGrids = (List<L_HIS_TREATMENT>)gridControlHisTreatment5.DataSource;
                //    if (dataGrids != null && dataGrids.Count > 0)
                //    {
                //        WaitingManager.Show();
                //        var dataSendYDTs = dataGrids.Where(p => p.IS_YDT_UPLOAD != 1 && p.IS_PAUSE == 1).ToList();
                //        if (dataSendYDTs != null && dataSendYDTs.Count > 0)
                //        {
                //            List<long> _treatmentIds = dataSendYDTs.Select(p => p.ID).ToList();
                //            var result = new BackendAdapter(param).Post<bool>("api/Histreatment/UploadYdt", ApiConsumers.MosConsumer, _treatmentIds, param);
                //            WaitingManager.Hide();
                //            if (result)
                //            {
                //                btnSearch_Click(null, null);
                //            }
                //            MessageManager.Show(this.ParentForm, param, result);
                //        }
                //        else
                //            mess = ResourceMessage.KhongCoHoSoNaoDaKetThucMaChuaDayYBaDienTu;
                //    }
                //    else
                //        mess = ResourceMessage.DuLieuRong;
                //    if (!string.IsNullOrEmpty(mess))
                //    {
                //        WaitingManager.Hide();
                //        DevExpress.XtraEditors.XtraMessageBox.Show(mess, ResourceMessage.ThongBao);
                //        return;
                //    }
                //}
                //else
                //{
                //    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.KhongDayDuocLenHID, ResourceMessage.ThongBao);
                //    return;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewHisTreatment5_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    long IS_YDT_UPLOAD = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewHisTreatment5.GetRowCellValue(e.RowHandle, "IS_YDT_UPLOAD") ?? "0").ToString());
                    if (e.Column.FieldName == "YDT_DISPLAY" && IS_YDT_UPLOAD == 1)
                    {
                        e.RepositoryItem = repositoryItemPictureEdit2;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboStatus_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboStatus.Properties.Buttons[1].Visible = false;
                    cboStatus.EditValue = null;
                    btnSearch.Focus();
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
                    if (cboStatus.EditValue != null)
                    {
                        cboStatus.Properties.Buttons[1].Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void tree_HisServiceReq2_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {

        }

        private void gridControlHisTreatment5_Click(object sender, EventArgs e)
        {

        }


    }
}
