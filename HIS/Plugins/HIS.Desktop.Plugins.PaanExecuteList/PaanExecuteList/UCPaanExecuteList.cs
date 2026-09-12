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
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.PaanExecuteList.ADO;
using HIS.Desktop.Plugins.PaanExecuteList.Resources;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using System.Resources;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PaanExecuteList.PaanExecuteList
{
    /// <summary>
    /// Man hinh danh sach benh nhan cho xu ly Giai phau benh.
    ///
    /// Nguon du lieu: api/HisSereServ/GetViewGpbl -> List&lt;V_HIS_SERE_SERV_GPBL&gt;
    /// Bo loc duoc giu tuong duong man hinh "Xu ly yeu cau kham/cls/pttt".
    /// </summary>
    public partial class UCPaanExecuteList : UserControlBase
    {
        #region Declare

        Inventec.Desktop.Common.Modules.Module currentModule;
        V_HIS_ROOM room = null;

        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int limit = 0;

        /// <summary>Danh sach dang hien tren luoi, giu lai de xu ly khi bam nut.</summary>
        List<PaanSereServADO> currentData = null;

        Inventec.Core.ApiResultObject<List<V_HIS_SERE_SERV_GPBL>> apiResult;

        /// <summary>Chan khong cho nap lai luoi khi dang khoi tao cac o loc.</summary>
        bool isLoadingControl = true;

        #endregion

        #region Constructor

        public UCPaanExecuteList()
        {
            InitializeComponent();
        }

        public UCPaanExecuteList(Inventec.Desktop.Common.Modules.Module currentModule)
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

        private void UCPaanExecuteList_Load(object sender, EventArgs e)
        {
            try
            {
                isLoadingControl = true;

                if (room == null && currentModule != null)
                {
                    room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentModule.RoomId);
                }

                LoadComboTimeType();
                LoadComboStatus();
                LoadComboDepartment();
                LoadComboRoom();
                LoadComboPatientType();

                SetCaptionByLanguageKey();
                HIS.Desktop.Plugins.PaanExecuteList.Base.ResourceLangManager.InitResourceLanguageManager();

                // Khoi phuc trang thai 2 o tick va 2 o so cong CPA.
                InitControlState();

                isLoadingControl = false;

                FillDataToGridControl();

                // Hai nhan dem duoi luoi.
                LoadServiceReqCount();
                LoadSereServCount();

                // Mo man hinh cho neu nguoi dung da tick.
                LoadDefaultScreenSaver();

                InitEnableControl();

                // Can lai bo cuc theo chieu rong that cua man hinh.
                LayoutFilterControls();
                LayoutRightButtons();
            }
            catch (Exception ex)
            {
                isLoadingControl = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Nap du lieu cho cac o loc

        /// <summary>Kieu loc thoi gian: theo Thang hoac theo Ngay.</summary>
        private void LoadComboTimeType()
        {
            try
            {
                List<ComboADO> data = new List<ComboADO>();
                data.Add(new ComboADO(TIME_TYPE__MONTH, "Tháng"));
                data.Add(new ComboADO(TIME_TYPE__DATE, "Ngày"));
                data.Add(new ComboADO(TIME_TYPE__YEAR, "Năm"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("name", "Kiểu thời gian", 80, 1));
                ControlEditorADO ado = new ControlEditorADO("name", "id", columnInfos, false, 80);
                ControlEditorLoader.Load(cboTimeType, data, ado);

                cboTimeType.EditValue = TIME_TYPE__MONTH;
                dtTime.DateTime = DateTime.Now;
                ApplyTimeMask();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Trang thai xu ly. Mac dinh "Chua ket thuc".</summary>
        private void LoadComboStatus()
        {
            try
            {
                List<ComboADO> data = new List<ComboADO>();
                data.Add(new ComboADO(STATUS__NOT_FINISHED, "Chưa kết thúc"));
                data.Add(new ComboADO(STATUS__ALL, "Tất cả"));
                data.Add(new ComboADO(STATUS__NOT_PROCESSED, "Chưa xử lý"));
                data.Add(new ComboADO(STATUS__PROCESSING, "Đang xử lý"));
                data.Add(new ComboADO(STATUS__FINISHED, "Hoàn thành"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("name", "Trạng thái", 120, 1));
                ControlEditorADO ado = new ControlEditorADO("name", "id", columnInfos, false, 120);
                ControlEditorLoader.Load(cboStatus, data, ado);

                cboStatus.EditValue = STATUS__NOT_FINISHED;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Khoa chi dinh.</summary>
        private void LoadComboDepartment()
        {
            try
            {
                List<HIS_DEPARTMENT> data = BackendDataWorker.Get<HIS_DEPARTMENT>()
                    .Where(o => o.IS_ACTIVE == 1)
                    .OrderBy(o => o.DEPARTMENT_NAME)
                    .ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "Mã khoa", 80, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "Tên khoa", 200, 2));
                ControlEditorADO ado = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 280);
                ControlEditorLoader.Load(cboDepartment, data, ado);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Phong chi dinh.</summary>
        private void LoadComboRoom()
        {
            try
            {
                List<V_HIS_ROOM> data = BackendDataWorker.Get<V_HIS_ROOM>()
                    .Where(o => o.IS_ACTIVE == 1)
                    .OrderBy(o => o.ROOM_NAME)
                    .ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ROOM_CODE", "Mã phòng", 80, 1));
                columnInfos.Add(new ColumnInfo("ROOM_NAME", "Tên phòng", 200, 2));
                ControlEditorADO ado = new ControlEditorADO("ROOM_NAME", "ID", columnInfos, false, 280);
                ControlEditorLoader.Load(cboRoom, data, ado);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Doi tuong thanh toan.</summary>
        private void LoadComboPatientType()
        {
            try
            {
                List<HIS_PATIENT_TYPE> data = BackendDataWorker.Get<HIS_PATIENT_TYPE>()
                    .Where(o => o.IS_ACTIVE == 1)
                    .OrderBy(o => o.PATIENT_TYPE_NAME)
                    .ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "Mã", 80, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "Đối tượng", 180, 2));
                ControlEditorADO ado = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 260);
                ControlEditorLoader.Load(cboPatientType, data, ado);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Nap du lieu luoi

        private void FillDataToGridControl()
        {
            try
            {
                FillDataToGrid(new CommonParam(0, (int)ConfigApplications.NumPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGrid, param, (int)ConfigApplications.NumPageSize, this.gridControlPaan);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid(object param)
        {
            try
            {
                WaitingManager.Show();

                // Du lieu luoi sap doi -> bo nho dem y lenh khong con dung nua.
                ClearServiceReqCache();

                startPage = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;

                CommonParam paramCommon = new CommonParam(startPage, limit);
                HisSereServViewGpblFilter filter = BuildFilter();

                Inventec.Common.Logging.LogSystem.Info("HisSereServViewGpblFilter: "
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));

                gridViewPaan.BeginUpdate();

                apiResult = new BackendAdapter(paramCommon).GetRO<List<V_HIS_SERE_SERV_GPBL>>(
                    PaanRequestUriStore.HIS_SERE_SERV_GET_VIEW_GPBL,
                    ApiConsumers.MosConsumer,
                    filter,
                    paramCommon);

                if (apiResult != null)
                {
                    WaitingManager.Hide();

                    var data = (List<V_HIS_SERE_SERV_GPBL>)apiResult.Data;
                    if (data != null)
                    {
                        currentData = data.Select(o => new PaanSereServADO(o)).ToList();

                        // Gan so thu tu sau khi da co thu tu sap xep tu server.
                        for (int i = 0; i < currentData.Count; i++)
                        {
                            currentData[i].STT = startPage + i + 1;
                        }

                        gridViewPaan.GridControl.DataSource = currentData;

                        rowCount = data.Count;
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);

                        if (data.Count == 1)
                        {
                            gridViewPaan.FocusedRowHandle = 0;
                        }
                    }
                    else
                    {
                        currentData = null;
                        gridViewPaan.GridControl.DataSource = null;
                    }

                    #region Process has exception
                    SessionManager.ProcessTokenLost((CommonParam)param);
                    #endregion
                }
                else
                {
                    currentData = null;
                    gridViewPaan.GridControl.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                gridViewPaan.EndUpdate();
                WaitingManager.Hide();
            }
        }

        /// <summary>
        /// Dung dieu kien loc tu cac o tren man hinh.
        ///
        /// CANH BAO QUAN TRONG:
        /// TDL_SERVICE_TYPE_ID LUON duoc gan, khong duoc bo di. Ngoai muc dich chot
        /// pham vi Giai phau benh, no con la dieu kien duy nhat chac chan thoa
        /// co che chong cao tai o Backend (ExpressionPropertySuffixChecker):
        /// neu khong co dieu kien nao co hau to ID/IDs/_EXACT/_CODE/_TIME/_DATE/_MONTH
        /// thi API se tra ve DANH SACH RONG ma khong bao loi gi.
        /// </summary>
        private HisSereServViewGpblFilter BuildFilter()
        {
            HisSereServViewGpblFilter filter = new HisSereServViewGpblFilter();
            try
            {
                // Chot pham vi Giai phau benh. KHONG duoc bo dong nay.
                filter.TDL_SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL;

                #region Thoi gian

                long timeType = GetComboValue(cboTimeType) ?? TIME_TYPE__MONTH;
                DateTime selected = dtTime.DateTime == DateTime.MinValue ? DateTime.Now : dtTime.DateTime;

                if (timeType == TIME_TYPE__DATE)
                {
                    filter.INTRUCTION_DATE__EQUAL = Convert.ToInt64(selected.ToString("yyyyMMdd"));
                }
                else if (timeType == TIME_TYPE__YEAR)
                {
                    // Loc ca nam: tu 01/01 00:00:00 den 31/12 23:59:59.
                    // Dung INTRUCTION_TIME_FROM/TO co san o Backend, khong can
                    // them dieu kien loc moi.
                    filter.INTRUCTION_TIME_FROM = Convert.ToInt64(selected.ToString("yyyy") + "0101000000");
                    filter.INTRUCTION_TIME_TO = Convert.ToInt64(selected.ToString("yyyy") + "1231235959");
                }
                else
                {
                    filter.INTRUCTION_MONTH__EQUAL = Convert.ToInt64(selected.ToString("yyyyMM"));
                }

                #endregion

                #region Trang thai

                long status = GetComboValue(cboStatus) ?? STATUS__NOT_FINISHED;
                if (status == STATUS__NOT_FINISHED)
                {
                    filter.SERVICE_REQ_STT_IDs = new List<long>
                    {
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL,
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL
                    };
                }
                else if (status == STATUS__NOT_PROCESSED)
                {
                    filter.SERVICE_REQ_STT_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL;
                }
                else if (status == STATUS__PROCESSING)
                {
                    filter.SERVICE_REQ_STT_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL;
                }
                else if (status == STATUS__FINISHED)
                {
                    filter.SERVICE_REQ_STT_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT;
                }
                // status == STATUS__ALL: khong gan gi, lay tat ca.

                #endregion

                #region Khoa / Phong / Doi tuong

                long? departmentId = GetComboValue(cboDepartment);
                if (departmentId.HasValue) filter.TDL_REQUEST_DEPARTMENT_ID = departmentId.Value;

                long? roomId = GetComboValue(cboRoom);
                if (roomId.HasValue) filter.TDL_REQUEST_ROOM_ID = roomId.Value;

                long? patientTypeId = GetComboValue(cboPatientType);
                if (patientTypeId.HasValue) filter.PATIENT_TYPE_ID = patientTypeId.Value;

                #endregion

                #region Tim kiem

                if (!String.IsNullOrWhiteSpace(txtServiceReqCode.Text))
                {
                    filter.SERVICE_REQ_CODE__EXACT = txtServiceReqCode.Text.Trim();
                }
                if (!String.IsNullOrWhiteSpace(txtPatientCode.Text))
                {
                    filter.TDL_PATIENT_CODE__EXACT = txtPatientCode.Text.Trim();
                }
                if (!String.IsNullOrWhiteSpace(txtSearchKey.Text))
                {
                    filter.KEY_WORD = txtSearchKey.Text.Trim();
                }

                #endregion

                filter.ORDER_FIELD = "TDL_INTRUCTION_TIME";
                filter.ORDER_DIRECTION = "DESC";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return filter;
        }

        /// <summary>Lay gia tri so tu combo, tra null neu chua chon.</summary>
        private long? GetComboValue(DevExpress.XtraEditors.GridLookUpEdit combo)
        {
            try
            {
                if (combo == null || combo.EditValue == null) return null;

                long value;
                if (long.TryParse(combo.EditValue.ToString(), out value) && value > 0)
                {
                    return value;
                }
                return null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        #endregion

        #region Su kien

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Bam nut X tren combo de bo chon (tra ve trang thai "tat ca").
        /// Khong gan su kien nay thi nut X hien nhung bam khong co tac dung.
        /// </summary>
        private void cboFilter_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button != null
                    && e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    DevExpress.XtraEditors.GridLookUpEdit combo = sender as DevExpress.XtraEditors.GridLookUpEdit;
                    if (combo != null)
                    {
                        combo.EditValue = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Doi dinh dang o chon thoi gian theo kieu loc dang chon.
        /// Chon "Nam" thi chi hien nam cho do roi mat.
        /// </summary>
        private void ApplyTimeMask()
        {
            try
            {
                long timeType = GetComboValue(cboTimeType) ?? TIME_TYPE__MONTH;

                if (timeType == TIME_TYPE__DATE)
                {
                    dtTime.Properties.Mask.EditMask = "dd/MM/yyyy";
                }
                else if (timeType == TIME_TYPE__YEAR)
                {
                    dtTime.Properties.Mask.EditMask = "yyyy";
                }
                else
                {
                    dtTime.Properties.Mask.EditMask = "MM/yyyy";
                }

                dtTime.Properties.Mask.UseMaskAsDisplayFormat = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Doi kieu loc thoi gian -> doi dinh dang o chon roi nap lai luoi.</summary>
        private void cboTimeType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                ApplyTimeMask();

                if (isLoadingControl) return;
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboFilter_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (isLoadingControl) return;
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtFilter_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGridControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Nut "Xu ly (Ctrl X)".</summary>
        private void btnProcess_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessSelectedRow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Nhay dup tren luoi cung mo man Tra ket qua.</summary>
        private void gridViewPaan_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                ProcessSelectedRow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Phim tat: Ctrl+X mo man Tra ket qua, Ctrl+F tim, F2/F3/F8 nhay vao o loc.
        /// Dung ProcessCmdKey de bat duoc phim ngay ca khi con tro dang o trong luoi.
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {
                if (keyData == (Keys.Control | Keys.X))
                {
                    ProcessSelectedRow();
                    return true;
                }
                if (keyData == (Keys.Control | Keys.F))
                {
                    FillDataToGridControl();
                    return true;
                }
                if (keyData == Keys.F2)
                {
                    txtSearchKey.Focus();
                    txtSearchKey.SelectAll();
                    return true;
                }
                if (keyData == Keys.F3)
                {
                    txtServiceReqCode.Focus();
                    txtServiceReqCode.SelectAll();
                    return true;
                }
                if (keyData == Keys.F8)
                {
                    txtPatientCode.Focus();
                    txtPatientCode.SelectAll();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>Danh so thu tu tren luoi.</summary>
        private void gridViewPaan_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.Column != null && e.Column.FieldName == "STT" && e.ListSourceRowIndex >= 0)
                {
                    e.DisplayText = (startPage + e.ListSourceRowIndex + 1).ToString();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Ngon ngu

        private void SetCaptionByLanguageKey()
        {
            try
            {
                // Nap bo tai nguyen ngon ngu cua chinh plugin nay.
                // Lam giong SamplePathologyReqUC.cs:1183.
                ResourceLanguageManager.LanguageResource = new ResourceManager(
                    "HIS.Desktop.Plugins.PaanExecuteList.Resources.Lang",
                    typeof(UCPaanExecuteList).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Hang so noi bo

        private const long TIME_TYPE__MONTH = 1;
        private const long TIME_TYPE__DATE = 2;
        private const long TIME_TYPE__YEAR = 3;

        private const long STATUS__NOT_FINISHED = 1;
        private const long STATUS__ALL = 2;
        private const long STATUS__NOT_PROCESSED = 3;
        private const long STATUS__PROCESSING = 4;
        private const long STATUS__FINISHED = 5;

        #endregion
    }
}
