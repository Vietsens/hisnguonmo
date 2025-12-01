using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTab;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using EMR.EFMODEL.DataModels;
using EMR.SDO;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.TreatmentHistory.ADO;
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
using static System.Windows.Forms.ImageList;

namespace HIS.Desktop.Plugins.TreatmentHistory
{
    public partial class UCPanelControlTreeSere7 : UserControl
    {
        UserControl ucSereServ;
        HIS_DHST dhst = new HIS_DHST();
        TreeSereServ7Processor treeSereServ7Processor;
        ImageList.ImageCollection imageCollection;
        DHisSereServ2 TreeClickData;
        private HIS_SERVICE_REQ mainExamReq;
        Inventec.Desktop.Common.Modules.Module currentModule;
        long departmentID = 0;
        bool IsExpandList = true;
        long wkRoomId = 0, wkRoomTypeId = 0;

        UCTreeListService ucAll, ucCLS, ucMediMate, ucOrther;

        public UCPanelControlTreeSere7()
        {
            InitializeComponent();
        }

        public UCPanelControlTreeSere7(Inventec.Desktop.Common.Modules.Module _currentModule)
        {
            InitializeComponent();
            try
            {
                //imageCollection = image;
                this.currentModule = _currentModule;
                this.wkRoomId = currentModule != null ? currentModule.RoomId : 0;
                this.wkRoomTypeId = currentModule != null ? currentModule.RoomTypeId : 0;

                SafeInitUc();
                InitExamReadOnly();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // Nếu đã dùng SafeInitUc trong constructor thì Load không cần làm gì nữa
        private void UCPanelControlTreeSere7_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Khởi tạo các UCTreeListService nếu chưa khởi tạo.
        /// Đảm bảo chỉ add control 1 lần, tránh Exception double parent.
        /// </summary>
        private void SafeInitUc()
        {
            try
            {
                if (ucAll != null) return; // đã init rồi thì thôi

                ucAll = new UCTreeListService(imageCollection1, currentModule);
                ucMediMate = new UCTreeListService(imageCollection1, currentModule);
                ucCLS = new UCTreeListService(imageCollection1, currentModule);
                ucOrther = new UCTreeListService(imageCollection1, currentModule);

                pcAll.Controls.Add(ucAll);
                ucAll.Dock = DockStyle.Fill;

                pcCLS.Controls.Add(ucCLS);
                ucCLS.Dock = DockStyle.Fill;

                pcMediMateBlood.Controls.Add(ucMediMate);
                ucMediMate.Dock = DockStyle.Fill;

                pcOther.Controls.Add(ucOrther);
                ucOrther.Dock = DockStyle.Fill;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeView_Click(ADO.SereServADO data)
        {
            try
            {
                if (data != null)
                {
                    TreeClickData = data;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<ADO.SereServADO> GroupDataByTracking(List<DHisSereServ2> dataNew, List<HIS_SERVICE_REQ> dataServiceReq)
        {
            List<ADO.SereServADO> SereServADOs = new List<ADO.SereServADO>();
            try
            {
                if (dataNew == null || dataNew.Count == 0)
                    return SereServADOs;

                var room = (currentModule != null)
                    ? BackendDataWorker.Get<HIS_ROOM>().FirstOrDefault(p => p.ID == currentModule.RoomId)
                    : null;

                var departmentId = room != null ? room.DEPARTMENT_ID : 0;

                var listRootByTracking = dataNew
                    .OrderByDescending(o => o.TRACKING_TIME)
                    .GroupBy(o => o.TRACKING_TIME)
                    .ToList();

                foreach (var tracking in listRootByTracking)
                {
                    #region GrandFather
                    ADO.SereServADO ssRootTrackingTime = new ADO.SereServADO();
                    ssRootTrackingTime.CONCRETE_ID__IN_SETY = tracking.First().TRACKING_TIME + "_";
                    string dayHospitalize = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(tracking.First().TRACKING_TIME ?? 0);
                    ssRootTrackingTime.SERVICE_CODE = !string.IsNullOrEmpty(dayHospitalize)
                        ? (System.String.Format("{0:dd/MM/yyyy HH:mm}", dayHospitalize))
                            .Substring(0, (System.String.Format("{0:dd/MM/yyyy HH:mm}", dayHospitalize)).Length - 3)
                        : "Chưa tạo tờ điều trị";
                    SereServADOs.Add(ssRootTrackingTime);
                    int count = 0;
                    #endregion

                    var listRootType = tracking.GroupBy(g => g.TDL_SERVICE_TYPE_ID).ToList();
                    foreach (var types in listRootType)
                    {
                        #region Parent
                        count++;
                        ADO.SereServADO ssRootType = new ADO.SereServADO();
                        ssRootType.CONCRETE_ID__IN_SETY = ssRootTrackingTime.CONCRETE_ID__IN_SETY + "_" + types.First().TRACKING_TIME + "_" + count;
                        ssRootType.PARENT_ID__IN_SETY = ssRootTrackingTime.CONCRETE_ID__IN_SETY;
                        var serviceType = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(p => p.ID == types.First().TDL_SERVICE_TYPE_ID);
                        long idSerReqType = 0;
                        long idDepartment = 0;
                        long idExecuteDepartment = 0;
                        short? IsTemporaryPres = 0;

                        if (dataServiceReq != null && dataServiceReq.Count > 0)
                        {
                            var req = dataServiceReq.FirstOrDefault(o => o.ID == types.First().SERVICE_REQ_ID);
                            if (req != null)
                            {
                                idSerReqType = req.SERVICE_REQ_TYPE_ID;
                                idDepartment = req.REQUEST_DEPARTMENT_ID;
                                idExecuteDepartment = req.EXECUTE_DEPARTMENT_ID;
                                IsTemporaryPres = req.IS_TEMPORARY_PRES;
                            }
                        }

                        ssRootType.TRACKING_TIME = types.First().TRACKING_TIME;
                        ssRootType.TDL_SERVICE_TYPE_ID = types.First().TDL_SERVICE_TYPE_ID;
                        ssRootType.SERVICE_CODE = serviceType != null ? serviceType.SERVICE_TYPE_NAME : null;
                        #endregion

                        SereServADOs.Add(ssRootType);

                        var listRootSety = types.GroupBy(g => g.SERVICE_REQ_ID).ToList();
                        foreach (var rootSety in listRootSety)
                        {
                            #region Child
                            ADO.SereServADO ssRootSety = new ADO.SereServADO();
                            ssRootSety.CONCRETE_ID__IN_SETY = ssRootType.CONCRETE_ID__IN_SETY + "_" + rootSety.First().SERVICE_REQ_ID;
                            ssRootSety.PARENT_ID__IN_SETY = ssRootType.CONCRETE_ID__IN_SETY;
                            ssRootSety.REQUEST_DEPARTMENT_ID = idDepartment;
                            ssRootSety.EXECUTE_DEPARTMENT_ID = idExecuteDepartment;
                            ssRootSety.SERVICE_REQ_TYPE_ID = BackendDataWorker
                                .Get<HIS_SERVICE_REQ_TYPE>()
                                .FirstOrDefault(p => p.ID == idSerReqType)?.ID ?? 0;

                            if (rootSety.First().USE_TIME.HasValue)
                            {
                                ssRootSety.REQUEST_DEPARTMENT_NAME = string.Format(
                                    "Dự trù: {0}",
                                    Inventec.Common.DateTime.Convert.TimeNumberToDateString(rootSety.First().USE_TIME.Value));
                            }

                            ssRootSety.TRACKING_TIME = rootSety.First().TRACKING_TIME;
                            ssRootSety.SERVICE_REQ_ID = rootSety.First().SERVICE_REQ_ID;
                            ssRootSety.SERVICE_REQ_STT_ID = rootSety.First().SERVICE_REQ_STT_ID;
                            ssRootSety.TDL_SERVICE_TYPE_ID = rootSety.First().TDL_SERVICE_TYPE_ID;
                            ssRootSety.PRESCRIPTION_TYPE_ID = rootSety.First().PRESCRIPTION_TYPE_ID;
                            ssRootSety.TDL_TREATMENT_ID = rootSety.First().TDL_TREATMENT_ID;
                            ssRootSety.REQUEST_LOGINNAME = rootSety.First().REQUEST_LOGINNAME;
                            ssRootSety.REQUEST_DEPARTMENT_ID = rootSety.First().REQUEST_DEPARTMENT_ID ?? 0;
                            ssRootSety.SERVICE_CODE = rootSety.First().SERVICE_REQ_CODE;
                            ssRootSety.SERVICE_REQ_CODE = rootSety.First().SERVICE_REQ_CODE;
                            ssRootSety.IS_TEMPORARY_PRES = IsTemporaryPres;

                            if (dataServiceReq != null && dataServiceReq.Count > 0)
                            {
                                var serviceReq = dataServiceReq.FirstOrDefault(o => o.ID == rootSety.First().SERVICE_REQ_ID) ?? new HIS_SERVICE_REQ();
                                ssRootSety.SAMPLE_TIME = serviceReq.SAMPLE_TIME;
                                ssRootSety.RECEIVE_SAMPLE_TIME = serviceReq.RECEIVE_SAMPLE_TIME;
                            }

                            ssRootSety.SERVICE_NAME = string.Format("- {0} - {1}",
                                rootSety.First().REQUEST_ROOM_NAME,
                                rootSety.First().REQUEST_DEPARTMENT_NAME);

                            var time = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rootSety.First().TDL_INTRUCTION_TIME ?? 0);
                            ssRootSety.NOTE_ADO = time.Substring(0, time.Count() - 3);

                            if ((rootSety.First().REQUEST_LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()
                                    || CheckLoginAdmin.IsAdmin(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()))
                                && (rootSety.First().SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL
                                    || HisConfigs.Get<string>("MOS.HIS_SERVICE_REQ.ALLOW_MODIFYING_OF_STARTED") == "1"
                                    || (HisConfigs.Get<string>("MOS.HIS_SERVICE_REQ.ALLOW_MODIFYING_OF_STARTED") == "2"
                                        && ssRootSety.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH))
                                && rootSety.First().IS_NO_EXECUTE != 1)
                            {
                                ssRootSety.IsEnableEdit = true;
                            }

                            if ((rootSety.First().REQUEST_LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()
                                    || CheckLoginAdmin.IsAdmin(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName())
                                    || (rootSety.First().REQUEST_DEPARTMENT_ID == departmentId
                                        && ssRootSety.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH))
                                && rootSety.First().SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                            {
                                ssRootSety.IsEnableDelete = true;
                            }

                            SereServADOs.Add(ssRootSety);
                            #endregion

                            int d = 0;
                            foreach (var item in rootSety)
                            {
                                d++;
                                #region Child (+n)
                                ADO.SereServADO ado = new ADO.SereServADO(item);
                                ado.IS_TEMPORARY_PRES = IsTemporaryPres;
                                ado.CONCRETE_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY + "_" + d;
                                ado.PARENT_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY;
                                ado.child = 4;

                                if (!string.IsNullOrWhiteSpace(item.TUTORIAL))
                                {
                                    ado.NOTE_ADO = string.Format("{0}. {1}", item.TUTORIAL, item.INSTRUCTION_NOTE);
                                }
                                else
                                {
                                    ado.NOTE_ADO = string.Format("{0}", item.INSTRUCTION_NOTE);
                                }

                                ado.AMOUNT_SER = string.Format("{0} - {1}", item.AMOUNT, item.SERVICE_UNIT_NAME);
                                SereServADOs.Add(ado);
                                #endregion
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SereServADOs = new List<ADO.SereServADO>();
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return SereServADOs;
        }

        private void HandleLoadToTabs(List<DHisSereServ2> dataNew, List<HIS_SERVICE_REQ> dataServiceReq, List<ADO.SereServADO> sereServADOs)
        {
            // sort
            sereServADOs = sereServADOs
                .OrderBy(o => o.PARENT_ID__IN_SETY)
                .ThenBy(p => p.SERVICE_CODE)
                .ThenBy(o => o.SERVICE_NAME)
                .ToList();

            // ALL
            ucAll.ReLoad(
                treeView_Click,
                GroupDataByTracking(dataNew, dataServiceReq),
                null, null, null
            );

            // CLS
            var listCLS = sereServADOs.Where(o =>
                o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA ||
                o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA ||
                o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS ||
                o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL ||
                o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN ||
                o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN
            ).ToList();

            ucCLS.ReLoad(treeView_Click, listCLS, null, null, null);

            // MediMate
            var listMediMate = sereServADOs.Where(o =>
                o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC ||
                o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT ||
                o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU
            ).ToList();

            ucMediMate.ReLoad(treeView_Click, listMediMate, null, null, null);

            // Other
            var listOther = sereServADOs.Where(o =>
                o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN ||
                o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G ||
                o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH ||
                o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC ||
                o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN ||
                o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT ||
                o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT
            ).ToList();

            ucOrther.ReLoad(treeView_Click, listOther, null, null, null);

            // reload tabs
            IsExpandList = true;
            tabSereServ.SelectedTabPage = tabSereServ.TabPages[3];
            tabSereServ.SelectedTabPage = tabSereServ.TabPages[2];
            tabSereServ.SelectedTabPage = tabSereServ.TabPages[1];
            tabSereServ.SelectedTabPage = tabSereServ.TabPages[0];
        }

        private void HandleEmptyTabs()
        {
            ucAll.ReLoad(null, null, null, null, null);
            ucCLS.ReLoad(null, null, null, null, null);
            ucMediMate.ReLoad(null, null, null, null, null);
            ucOrther.ReLoad(null, null, null, null, null);
        }

        public void LoadTabs(List<DHisSereServ2> dataNew, List<HIS_SERVICE_REQ> dataServiceReq, List<ADO.SereServADO> sereServADOs)
        {
            if (sereServADOs == null || sereServADOs.Count == 0)
            {
                HandleEmptyTabs();
                return;
            }

            HandleLoadToTabs(dataNew, dataServiceReq, sereServADOs);
        }

        public void SelectedTab(int index)
        {
            tabSereServ.SelectedTabPageIndex = index;
        }
        public bool ToggleExpand()
        {
            try
            {

                if (ucAll == null || ucMediMate == null || ucCLS == null || ucOrther == null)
                    return IsExpandList;

                // đảo trạng thái
                bool newExpand = !IsExpandList;

                ucAll.Expand(newExpand);
                ucMediMate.Expand(newExpand);
                ucCLS.Expand(newExpand);
                ucOrther.Expand(newExpand);

                IsExpandList = newExpand;

                // trả về trạng thái hiện tại sau khi toggle
                return IsExpandList;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return IsExpandList;
            }
        }
        private void InitExamReadOnly()
        {
            txtLyDoKham.Properties.ReadOnly = true;
            txtQuaTrinhBenhLy.Properties.ReadOnly = true;
            txtTienSuBenh.Properties.ReadOnly = true;
            txtKhamToanThan.Properties.ReadOnly = true;
            txtKhamBoPhan.Properties.ReadOnly = true;
            txtTomTatCLS.Properties.ReadOnly = true;
            txtCDSoBo.Properties.ReadOnly = true;
            txtMaHDT.Properties.ReadOnly = true;
            txtTenHDT.Properties.ReadOnly = true;

            txtMach.Properties.ReadOnly = true;
            txtNhietDo.Properties.ReadOnly = true;
            txtHuyetApMax.Properties.ReadOnly = true;
            txtHuyetApMin.Properties.ReadOnly = true;
            txtNhipTho.Properties.ReadOnly = true;
            txtCanNang.Properties.ReadOnly = true;
            txtChieuCao.Properties.ReadOnly = true;
            txtMaCD.Properties.ReadOnly = true;
            txtTenCD.Properties.ReadOnly = true;
            txtMaNguyenNhan.Properties.ReadOnly = true;
            txtTenNguyenNhan.Properties.ReadOnly = true;
            txtMaBenhPhu.Properties.ReadOnly = true;
            txtTenBenhPhu.Properties.ReadOnly = true;
        }
        private void ClearExamTab()
        {
            txtLyDoKham.EditValue = null;
            txtQuaTrinhBenhLy.EditValue = null;
            txtTienSuBenh.EditValue = null;
            txtKhamToanThan.EditValue = null;
            txtKhamBoPhan.EditValue = null;
            txtTomTatCLS.EditValue = null;
            txtCDSoBo.EditValue = null;
            txtMaHDT.EditValue = null;
            txtTenHDT.EditValue = null;

            txtMach.EditValue = null;
            txtNhietDo.EditValue = null;
            txtHuyetApMax.EditValue = null;
            txtHuyetApMin.EditValue = null;
            txtNhipTho.EditValue = null;
            txtCanNang.EditValue = null;
            txtChieuCao.EditValue = null;
            txtMaCD.EditValue = null;
            txtTenCD.EditValue = null;
            txtMaNguyenNhan.EditValue = null;
            txtTenNguyenNhan.EditValue = null;
            txtMaBenhPhu.EditValue = null;
            txtTenBenhPhu.EditValue = null;
        }
        private HIS_DHST GetLatestDhstByTreatment(long treatmentId)
        {
            if (treatmentId <= 0) return null;

            HIS_DHST dhst = null;

            try
            {
                HisDhstFilter dhstFilter = new HisDhstFilter();
                dhstFilter.TREATMENT_ID = treatmentId;
                dhstFilter.ORDER_FIELD = "EXECUTE_TIME";
                dhstFilter.ORDER_DIRECTION = "DESC";

                CommonParam param = new CommonParam();

                var listDHST = new BackendAdapter(param)
                    .Get<List<HIS_DHST>>("api/HisDHST/Get", ApiConsumers.MosConsumer, dhstFilter, param);

                if (listDHST != null && listDHST.Count > 0)
                {
                    listDHST = listDHST
                        .OrderByDescending(o => o.EXECUTE_TIME)
                        .ThenByDescending(o => o.ID)
                        .ToList();

                    var firstDhst = listDHST[0];

                    // nếu bản đầu tiên đã đủ dữ liệu thì dùng luôn
                    if (firstDhst.WEIGHT.HasValue && firstDhst.HEIGHT.HasValue &&
                        firstDhst.TEMPERATURE.HasValue && firstDhst.BREATH_RATE.HasValue &&
                        firstDhst.CHEST.HasValue && firstDhst.BELLY.HasValue &&
                        firstDhst.BLOOD_PRESSURE_MAX.HasValue && firstDhst.BLOOD_PRESSURE_MIN.HasValue &&
                        firstDhst.PULSE.HasValue && firstDhst.SPO2.HasValue)
                    {
                        dhst = firstDhst;
                    }
                    else
                    {
                        dhst = firstDhst;

                        // ghép thêm thông tin còn thiếu từ các lần đo khác
                        foreach (var item in listDHST)
                        {
                            if (dhst != null && dhst.WEIGHT.HasValue && !dhst.HEIGHT.HasValue && item.HEIGHT.HasValue)
                            {
                                dhst.HEIGHT = item.HEIGHT;
                            }
                            else if (dhst != null && dhst.HEIGHT.HasValue && !dhst.WEIGHT.HasValue && item.WEIGHT.HasValue)
                            {
                                dhst.WEIGHT = item.WEIGHT;
                            }
                            else if (dhst != null && !dhst.HEIGHT.HasValue && !dhst.WEIGHT.HasValue)
                            {
                                dhst.WEIGHT = item.WEIGHT;
                                dhst.HEIGHT = item.HEIGHT;
                            }
                            MapInformationDhstEmpty(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return dhst;
        }
        private void MapInformationDhstEmpty(HIS_DHST item)
        {
            try
            {
                if (!dhst.TEMPERATURE.HasValue && item.TEMPERATURE.HasValue)
                    dhst.TEMPERATURE = item.TEMPERATURE;
                if (!dhst.BREATH_RATE.HasValue && item.BREATH_RATE.HasValue)
                    dhst.BREATH_RATE = item.BREATH_RATE;
                if (!dhst.CHEST.HasValue && item.CHEST.HasValue)
                    dhst.CHEST = item.CHEST;
                if (!dhst.BELLY.HasValue && item.BELLY.HasValue)
                    dhst.BELLY = item.BELLY;
                if (!dhst.BLOOD_PRESSURE_MAX.HasValue && item.BLOOD_PRESSURE_MAX.HasValue)
                    dhst.BLOOD_PRESSURE_MAX = item.BLOOD_PRESSURE_MAX;
                if (!dhst.BLOOD_PRESSURE_MIN.HasValue && item.BLOOD_PRESSURE_MIN.HasValue)
                    dhst.BLOOD_PRESSURE_MIN = item.BLOOD_PRESSURE_MIN;
                if (!dhst.PULSE.HasValue && item.PULSE.HasValue)
                    dhst.PULSE = item.PULSE;
                if (!dhst.SPO2.HasValue && item.SPO2.HasValue)
                    dhst.SPO2 = item.SPO2;
                if (!dhst.URINE.HasValue && item.URINE.HasValue)
                    dhst.URINE = item.URINE;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadVitalSigns(HIS_DHST dhst)
        {
            txtMach.EditValue = null;
            txtNhietDo.EditValue = null;
            txtHuyetApMax.EditValue = null;
            txtHuyetApMin.EditValue = null;
            txtNhipTho.EditValue = null;
            txtCanNang.EditValue = null;
            txtChieuCao.EditValue = null;

            if (dhst == null) return;

            txtMach.EditValue = dhst.PULSE;
            txtNhietDo.EditValue = dhst.TEMPERATURE;
            txtHuyetApMax.EditValue = dhst.BLOOD_PRESSURE_MAX;
            txtHuyetApMin.EditValue = dhst.BLOOD_PRESSURE_MIN;
            txtNhipTho.EditValue = dhst.BREATH_RATE;
            txtCanNang.EditValue = dhst.WEIGHT;
            txtChieuCao.EditValue = dhst.HEIGHT;
            lblBMI.Text = dhst.VIR_BMI.ToString();
        }
        public void LoadExamInfo(long treatmentId)
        {
            try
            {
                ClearExamTab();
                mainExamReq = null;

                if (treatmentId <= 0)
                    return;

                // 1. Lấy y lệnh khám chính
                var param = new CommonParam();
                var filter = new HisServiceReqFilter();
                filter.TREATMENT_ID = treatmentId;
                filter.IS_MAIN_EXAM = true;

                var list = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(
                    "api/HisServiceReq/Get",
                    ApiConsumers.MosConsumer,
                    filter,
                    param);

                var examReq = (list != null) ? list.FirstOrDefault() : null;
                if (examReq == null)
                {
                    return;
                }

                mainExamReq = examReq;

                txtLyDoKham.EditValue = examReq.HOSPITALIZATION_REASON;
                txtQuaTrinhBenhLy.EditValue = examReq.PATHOLOGICAL_PROCESS;
                txtTienSuBenh.EditValue = examReq.PATHOLOGICAL_HISTORY;
                txtKhamToanThan.EditValue = examReq.FULL_EXAM;
                txtKhamBoPhan.EditValue = examReq.PART_EXAM;
                txtTomTatCLS.EditValue = examReq.SUBCLINICAL;
                txtCDSoBo.EditValue = examReq.PROVISIONAL_DIAGNOSIS;

                txtMaHDT.EditValue = examReq.NEXT_TREAT_INTR_CODE;
                txtTenHDT.EditValue = examReq.NEXT_TREATMENT_INSTRUCTION;

                var dhst = GetLatestDhstByTreatment(treatmentId);
                LoadVitalSigns(dhst);

                txtMaCD.EditValue = examReq.ICD_CODE;
                txtTenCD.EditValue = examReq.ICD_NAME;

                txtMaNguyenNhan.EditValue = examReq.ICD_CAUSE_CODE;
                txtTenNguyenNhan.EditValue = examReq.ICD_CAUSE_NAME;

                txtMaBenhPhu.EditValue = examReq.ICD_SUB_CODE;
                txtTenBenhPhu.EditValue = examReq.ICD_TEXT;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }

}
