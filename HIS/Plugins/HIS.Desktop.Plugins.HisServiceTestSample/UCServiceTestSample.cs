using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.HisServiceTestSample.entity;
using HIS.UC.Service;
using HIS.UC.Service.ADO;
using HIS.UC.TestSample;
using HIS.UC.TestSample.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisServiceTestSample
{
    public partial class UCServiceTestSample : HIS.Desktop.Utility.UserControlBase
    {
        
        List<HIS_SERVICE_TYPE> ServiceType { get; set; }
        internal List<HIS.UC.TestSample.ADO.TestSampleADO> lstTestSampleADOs { get; set; }
        internal List<HIS.UC.Service.ServiceADO> lstServiceTestSampleADOs { get; set; }
        UCTestSampleProcessor TestSampleProcessor;
        UCServiceProcessor ServiceProcessor;
        UserControl ucGridControlService;
        UserControl ucGridControlTestSample;
        long ServiceIdCheckByService = 0;
        List<HIS_TEST_SAMPLE_TYPE> listTestSample;
        List<V_HIS_SERVICE> listService;
        List<HIS_SERVICE_TESA> ServiceTesa { get; set; }
        List<HIS_SERVICE_TESA> ServiceTesaViews { get; set; }

        long TestSampleIdCheckByService = 0;
        long isChoseService;
        long isChoseTestSample;
        bool isCheckAll;
        long TestSampleIdCheckByTestSample;
        int rowCount = 0;
        int dataTotal = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        V_HIS_SERVICE currentService;

        HIS.UC.Service.ServiceADO currentCopyServiceAdo { get; set; } 

        public UCServiceTestSample()
        {
            InitializeComponent();
        }
        public UCServiceTestSample(V_HIS_SERVICE serviceData, Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
            try
            {
                this.currentService = serviceData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void UCServiceTestSample_Load(object sender, EventArgs e)
        {
            try
            {
                LoadDataToCombo();
                LoadComboStatus();
                InitUcgrid1();
                InitUcgrid2();
                if (this.currentService == null)
                {
                    FillDataToGrid1(this);
                    FillDataToGrid2(this);
                }
                else
                {
                    FillDataToGrid1_Default(this);
                    FillDataToGrid2(this);
                    btn_Radio_Enable_Click1(this.currentService);
                }

                WaitingManager.Hide();
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
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceTypeFilter ServiceTypeFilter = new HisServiceTypeFilter();
                ServiceTypeFilter.IDs = new List<long>
                {
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL
                }; 
                ServiceType = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERVICE_TYPE>>(
                             "api/HisServiceType/Get",
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    ServiceTypeFilter,
                    param);
                LoadDataToComboServiceType(cboServiceType, ServiceType);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToComboServiceType(DevExpress.XtraEditors.GridLookUpEdit cboServiceType, List<HIS_SERVICE_TYPE> ServiceType)
        {
            try
            {
                cboServiceType.Properties.DataSource = ServiceType;
                cboServiceType.Properties.DisplayMember = "SERVICE_TYPE_NAME";
                cboServiceType.Properties.ValueMember = "ID";

                cboServiceType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                cboServiceType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboServiceType.Properties.ImmediatePopup = true;
                cboServiceType.ForceInitialize();
                cboServiceType.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cboServiceType.Properties.View.Columns.AddField("SERVICE_TYPE_CODE");  
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cboServiceType.Properties.View.Columns.AddField("SERVICE_TYPE_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;
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
                List<Status> status = new List<Status>();
                status.Add(new Status(1, "Dịch vụ"));
                status.Add(new Status(2, "Mẫu bệnh phẩm"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboChoose, status, controlEditorADO);
                cboChoose.EditValue = status[0].id;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitUcgrid1()
        {
            try
            {
                ServiceProcessor = new UCServiceProcessor();
                ServiceInitADO ado = new ServiceInitADO();
                ado.ListServiceColumn = new List<UC.Service.ServiceColumn>();
                ado.gridViewService_MouseDownMest = gridViewService_MouseDown;
                ado.btn_Radio_Enable_Click1 = btn_Radio_Enable_Click1;
                //ado.gridView_MouseRightClick = ServiceGridView_MouseRightClick;

                ServiceColumn colRadio2 = new ServiceColumn("   ", "radioService", 30, true);
                colRadio2.VisibleIndex = 0;
                colRadio2.Visible = false;
                colRadio2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListServiceColumn.Add(colRadio2);

                ServiceColumn colCheck2 = new ServiceColumn("   ", "checkService", 30, true);
                colCheck2.VisibleIndex = 1;
                //colCheck2.image = imageCollectionService.Images[0];
                colCheck2.Visible = false;
                colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListServiceColumn.Add(colCheck2);

                ServiceColumn colMaDichvu = new ServiceColumn("Mã dịch vụ", "SERVICE_CODE", 60, false);
                colMaDichvu.VisibleIndex = 2;
                ado.ListServiceColumn.Add(colMaDichvu);

                ServiceColumn colTenDichvu = new ServiceColumn("Tên dịch vụ", "SERVICE_NAME", 300, false);
                colTenDichvu.VisibleIndex = 3;
                ado.ListServiceColumn.Add(colTenDichvu);

                ServiceColumn colMaLoaidichvu = new ServiceColumn("Loại dịch vụ", "SERVICE_TYPE_NAME", 80, false);
                colMaLoaidichvu.VisibleIndex = 4;
                ado.ListServiceColumn.Add(colMaLoaidichvu);

                this.ucGridControlService = (UserControl)ServiceProcessor.Run(ado);
                if (ucGridControlService != null)
                {
                    this.panelControl1.Controls.Add(this.ucGridControlService);
                    this.ucGridControlService.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewService_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseService == 1)
                {
                    return;
                }

                WaitingManager.Show();
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "checkService")
                        {
                            var lstCheckAll = lstServiceTestSampleADOs;
                            List<HIS.UC.Service.ServiceADO> lstChecks = new List<HIS.UC.Service.ServiceADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var ServiceCheckedNum = lstServiceTestSampleADOs.Where(o => o.checkService == true).Count();
                                var ServiceNum = lstServiceTestSampleADOs.Count();
                                if ((ServiceCheckedNum > 0 && ServiceCheckedNum < ServiceNum) || ServiceCheckedNum == 0)
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = imageCollectionService.Images[1];
                                }

                                if (ServiceCheckedNum == ServiceNum)
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = imageCollectionService.Images[0];
                                }

                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.checkService = true;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = false;
                                }
                                else
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.checkService = false;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = true;
                                }

                                ServiceProcessor.Reload(ucGridControlService, lstChecks);


                            }
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

        private void btn_Radio_Enable_Click1(V_HIS_SERVICE data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceTesaFilter filter = new HisServiceTesaFilter();
                filter.SERVICE_ID = data.ID;
                ServiceIdCheckByService = data.ID;
                ServiceTesa = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERVICE_TESA>>(
                                    "api/HisServiceTesa/Get",
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<TestSampleADO> lstADO = new List<TestSampleADO>();
                lstADO = (from r in listTestSample select new TestSampleADO(r)).ToList();
                if (ServiceTesa != null && ServiceTesa.Count > 0)
                {
                    foreach (var itemRoom in ServiceTesa)
                    {
                        var check = lstADO.FirstOrDefault(o => o.ID == itemRoom.TEST_SAME_TYPE_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }
                }
                lstADO = lstADO.OrderByDescending(p => p.check1).ToList();
                if (ucGridControlTestSample != null)
                {
                    TestSampleProcessor.Reload(ucGridControlTestSample, lstADO);
                }
                else
                {
                    FillDataToGrid2(this);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void ServiceGridView_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    try
        //    {
        //        if ((e.Item is BarButtonItem) && sender != null && sender is HIS.UC.Service.ServiceADO)
        //        {
        //            var type = (HIS.UC.Service.Popup.PopupMenuProcessor.ItemType)e.Item.Tag;
        //            switch (type)
        //            {
        //                case HIS.UC.Service.Popup.PopupMenuProcessor.ItemType.Copy:
        //                    {
        //                        if (isChoseService != 1)
        //                        {
        //                            MessageManager.Show("Vui lòng chọn dịch vụ!");
        //                            break;
        //                        }
        //                        this.currentCopyServiceAdo = (HIS.UC.Service.ServiceADO)sender;
        //                        break;
        //                    }
        //                case HIS.UC.Service.Popup.PopupMenuProcessor.ItemType.Paste:
        //                    {
        //                        var currentPaste = (HIS.UC.Service.ServiceADO)sender;
        //                        bool success = false;
        //                        CommonParam param = new CommonParam();
        //                        if (this.currentCopyServiceAdo == null && isChoseService != 1)
        //                        {
        //                            MessageManager.Show("Vui lòng copy!");
        //                            break;
        //                        }
        //                        if (this.currentCopyServiceAdo != null && currentPaste != null && isChoseService == 1)
        //                        {
        //                            if (this.currentCopyServiceAdo.ID == currentPaste.ID)
        //                            {
        //                                MessageManager.Show("Trùng dữ liệu copy và paste");
        //                                break;
        //                            }
        //                            HisServiceRetyCatCopyByServiceSDO hisMestMatyCopyByMatySDO = new HisServiceRetyCatCopyByServiceSDO();
        //                            hisMestMatyCopyByMatySDO.CopyServiceId = this.currentCopyServiceAdo.ID;
        //                            hisMestMatyCopyByMatySDO.PasteServiceId = currentPaste.ID;
        //                            var result = new BackendAdapter(param).Post<List<HIS_SERVICE_TESA>>("api/HisServiceRetyCat/CopyByService", ApiConsumer.ApiConsumers.MosConsumer, hisMestMatyCopyByMatySDO, param);
        //                            if (result != null)
        //                            {
        //                                success = true;
        //                                ServiceTesa = result;
        //                                List<HIS.UC.ReportRetyCat.ReportRetyCatADO> dataNew = new List<HIS.UC.ReportRetyCat.ReportRetyCatADO>();
        //                                dataNew = (from r in listReportRetyCat select new ReportRetyCatADO(r)).ToList();
        //                                if (ServiceTesa != null && ServiceTesa.Count > 0)
        //                                {
        //                                    foreach (var itemRoom in ServiceTesa)
        //                                    {
        //                                        var check = dataNew.FirstOrDefault(o => o.ID == itemRoom.REPORT_TYPE_CAT_ID);
        //                                        if (check != null)
        //                                        {
        //                                            check.check1 = true;
        //                                        }
        //                                    }
        //                                }
        //                                dataNew = dataNew.OrderByDescending(p => p.check1).ToList();
        //                                if (ucGridControlReportRetyCat != null)
        //                                {
        //                                    ReportRetyCatProcessor.Reload(ucGridControlReportRetyCat, dataNew);
        //                                }
        //                                else
        //                                {
        //                                    FillDataToGridReportTypeCat(this);
        //                                }
        //                            }
        //                        }
        //                        MessageManager.Show(this.ParentForm, param, success);
        //                        break;
        //                    }
        //                default:
        //                    break;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void FillDataToGrid2(UCServiceTestSample uCServiceTestSample)
        {
            try
            {
                TestSampleIdCheckByTestSample = 0;
                int numPageSize;
                if (ucPaging2.pagingGrid != null)
                {
                    numPageSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                FillDataToGridTestSample(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPaging2.Init(FillDataToGridTestSample, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridTestSample(object data)
        {
            try
            {
                WaitingManager.Show();
                listTestSample = new List<HIS_TEST_SAMPLE_TYPE>();
                int start1 = ((CommonParam)data).Start ?? 0;
                int limit1 = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start1, limit1);
                MOS.Filter.HisTestSampleTypeFilter TestSampleFillter = new HisTestSampleTypeFilter();
                TestSampleFillter.IS_ACTIVE = 1;
                TestSampleFillter.ORDER_FIELD = "MODIFY_TIME";
                TestSampleFillter.ORDER_DIRECTION = "DESC";
                TestSampleFillter.KEY_WORD = txtKeyword2.Text;
                if ((long)cboChoose.EditValue == 2)
                {
                    isChoseTestSample = (long)cboChoose.EditValue;
                }

                var sar = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.HIS_TEST_SAMPLE_TYPE>>(
                   "api/HisTestSampleType/Get",
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                      TestSampleFillter,
                    param);

                lstTestSampleADOs = new List<TestSampleADO>();
                if (sar != null && sar.Data.Count > 0)
                {
                    listTestSample = sar.Data;
                    foreach (var item in listTestSample)
                    {
                        TestSampleADO roomaccountADO = new TestSampleADO(item);
                        if (isChoseTestSample == 2)
                        {
                            roomaccountADO.isKeyChoose = true;
                        }
                        lstTestSampleADOs.Add(roomaccountADO);
                    }
                }

                if (ServiceTesa != null && ServiceTesa.Count > 0)
                {
                    foreach (var itemUsername in ServiceTesa)
                    {
                        var check = lstTestSampleADOs.FirstOrDefault(o => o.ID == itemUsername.TEST_SAME_TYPE_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }
                }
                lstTestSampleADOs = lstTestSampleADOs.OrderByDescending(p => p.check1).Distinct().ToList();

                if (ucGridControlTestSample != null)
                {
                    TestSampleProcessor.Reload(ucGridControlTestSample, lstTestSampleADOs);
                }
                rowCount1 = (data == null ? 0 : lstTestSampleADOs.Count);
                dataTotal1 = (sar.Param == null ? 0 : sar.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcgrid2()
        {
            try
            {
                TestSampleProcessor = new UCTestSampleProcessor();
                TestSampleInitADO ado = new TestSampleInitADO();
                ado.ListTestSampleColumn = new List<UC.TestSample.TestSampleColumn>();
                ado.GridViewTestSample_MouseDown = gridViewTestSample_MouseDown;
                ado.btn_Radio_Enable_Click = btn_Radio_Enable_Click;
                //ado.gridView_MouseRightClick = MachineGridView_MouseRightClick;

                TestSampleColumn colRadio1 = new TestSampleColumn("   ", "radio1", 30, true);
                colRadio1.VisibleIndex = 0;
                colRadio1.Visible = false;
                colRadio1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListTestSampleColumn.Add(colRadio1);

                TestSampleColumn colCheck1 = new TestSampleColumn("   ", "check1", 30, true);
                colCheck1.VisibleIndex = 1;
                //colCheck1.image = imageCollectionTestSample.Images[0];
                colCheck1.Visible = false;
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListTestSampleColumn.Add(colCheck1);

                TestSampleColumn colMaMau = new TestSampleColumn("Mã mẫu bệnh phẩm", "TEST_SAMPLE_TYPE_CODE", 60, false);
                colMaMau.VisibleIndex = 2;
                ado.ListTestSampleColumn.Add(colMaMau);

                TestSampleColumn colTenMau = new TestSampleColumn("Tên mẫu bệnh phẩm", "TEST_SAMPLE_TYPE_NAME", 100, false);
                colTenMau.VisibleIndex = 3;
                ado.ListTestSampleColumn.Add(colTenMau);


                this.ucGridControlTestSample = (UserControl)TestSampleProcessor.Run(ado);
                if (ucGridControlTestSample != null)
                {
                    this.panelControl2.Controls.Add(this.ucGridControlTestSample);
                    this.ucGridControlTestSample.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTestSample_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseTestSample == 2)
                {
                    return;
                }

                WaitingManager.Show();
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "check1")
                        {
                            var lstCheckAll = lstTestSampleADOs;
                            List<TestSampleADO> lstChecks = new List<TestSampleADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var TestSampleCheckedNum = lstTestSampleADOs.Where(o => o.check1 == true).Count();
                                var TestSampletmNum = lstTestSampleADOs.Count();
                                if ((TestSampleCheckedNum > 0 && TestSampleCheckedNum < TestSampletmNum) || TestSampleCheckedNum == 0)
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = imageCollectionTestSample.Images[1];
                                }

                                if (TestSampleCheckedNum == TestSampletmNum)
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = imageCollectionTestSample.Images[0];
                                }

                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.check1 = true;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = false;
                                }
                                else
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.check1 = false;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = true;
                                }

                                TestSampleProcessor.Reload(ucGridControlTestSample, lstChecks);


                            }
                        }
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

        private void btn_Radio_Enable_Click(HIS_TEST_SAMPLE_TYPE data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceTesaFilter filter = new HisServiceTesaFilter();
                filter.TEST_SAME_TYPE_ID = data.ID;
                TestSampleIdCheckByTestSample = data.ID;
                ServiceTesaViews = new BackendAdapter(param).Get<List<HIS_SERVICE_TESA>>(
                                         "api/HisServiceTesa/Get",

                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<HIS.UC.Service.ServiceADO> dataNew = new List<HIS.UC.Service.ServiceADO>();
                dataNew = (from r in listService select new HIS.UC.Service.ServiceADO(r)).ToList();
                if (ServiceTesaViews != null && ServiceTesaViews.Count > 0)
                {

                    foreach (var itemService in ServiceTesaViews)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == itemService.SERVICE_ID);
                        if (check != null)
                        {
                            check.checkService = true;
                        }
                    }

                    dataNew = dataNew.OrderByDescending(p => p.checkService).ToList();

                    if (ucGridControlService != null)
                    {
                        ServiceProcessor.Reload(ucGridControlService, dataNew);
                    }
                }
                else
                {
                    FillDataToGrid1(this);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid1(UCServiceTestSample UCServiceTestSample)
        {
            try
            {
                ServiceIdCheckByService = 0;
                int numPageSize;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                FillDataToGridService(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridService, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridService(object data)
        {
            try
            {
                WaitingManager.Show();
                listService = new List<V_HIS_SERVICE>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisServiceViewFilter ServiceFillter = new HisServiceViewFilter();
                ServiceFillter.IS_ACTIVE = 1;
                ServiceFillter.ORDER_FIELD = "MODIFY_TIME";
                ServiceFillter.ORDER_DIRECTION = "DESC";
                ServiceFillter.KEY_WORD = txtKeyword1.Text;

                if (cboServiceType.EditValue != null)

                    ServiceFillter.SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceType.EditValue ?? "0").ToString());
                else
                    ServiceFillter.SERVICE_TYPE_IDs = BackendDataWorker.Get<HIS_SERVICE_TYPE>().Where(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN || o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL).Select(o => o.ID).ToList();

                if ((long)cboChoose.EditValue == 1)
                {
                    isChoseService = (long)cboChoose.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE>>(
                                                     "api/HisService/GetView",
                     HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                     ServiceFillter,
                     param);

                lstServiceTestSampleADOs = new List<ServiceADO>();

                if (rs != null && rs.Data.Count > 0)
                {

                    listService = rs.Data;
                    foreach (var item in listService)
                    {
                        ServiceADO ServiceTestSampleADO = new ServiceADO(item);
                        if (isChoseService == 1)
                        {
                            ServiceTestSampleADO.isKeyChooseService = true;
                        }
                        lstServiceTestSampleADOs.Add(ServiceTestSampleADO);
                    }
                }

                if (ServiceTesaViews != null && ServiceTesaViews.Count > 0)
                {
                    foreach (var itemUsername in ServiceTesaViews)
                    {
                        var check = lstServiceTestSampleADOs.FirstOrDefault(o => o.ID == itemUsername.SERVICE_ID);
                        if (check != null)
                        {
                            check.checkService = true;
                        }
                    }
                }

                lstServiceTestSampleADOs = lstServiceTestSampleADOs.OrderByDescending(p => p.checkService).Distinct().ToList();
                if (ucGridControlService != null)
                {
                    ServiceProcessor.Reload(ucGridControlService, lstServiceTestSampleADOs);
                }
                rowCount = (data == null ? 0 : lstServiceTestSampleADOs.Count);
                dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid1_Default(UCServiceTestSample UCServiceTestSample)
        {
            try
            {
                ServiceIdCheckByService = 0;
                int numPageSize;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                FillDataToGridService_Default(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridService_Default, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridService_Default(object data)
        {
            try
            {
                WaitingManager.Show();
                listService = new List<V_HIS_SERVICE>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisServiceViewFilter ServiceFillter = new HisServiceViewFilter();
                ServiceFillter.IS_ACTIVE = 1;
                ServiceFillter.ID = this.currentService.ID;

                if (cboServiceType.EditValue != null)

                    ServiceFillter.SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceType.EditValue ?? "0").ToString());

                if ((long)cboChoose.EditValue == 1)
                {
                    isChoseService = (long)cboChoose.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE>>(
                                                     "api/HisService/GetView",
                     HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                     ServiceFillter,
                     param);

                lstServiceTestSampleADOs = new List<ServiceADO>();

                if (rs != null && rs.Data.Count > 0)
                {

                    listService = rs.Data;
                    foreach (var item in listService)
                    {
                        ServiceADO ServiceTestSampleADO = new ServiceADO(item);
                        if (isChoseService == 1)
                        {
                            ServiceTestSampleADO.isKeyChooseService = true;
                            ServiceTestSampleADO.radioService = true;
                        }
                        lstServiceTestSampleADOs.Add(ServiceTestSampleADO);
                    }
                }

                if (ServiceTesaViews != null && ServiceTesaViews.Count > 0)
                {
                    foreach (var itemUsername in ServiceTesaViews)
                    {
                        var check = lstServiceTestSampleADOs.FirstOrDefault(o => o.ID == itemUsername.SERVICE_ID);
                        if (check != null)
                        {
                            check.checkService = true;
                        }
                    }
                }

                lstServiceTestSampleADOs = lstServiceTestSampleADOs.OrderByDescending(p => p.checkService).ToList();
                if (ucGridControlService != null)
                {
                    ServiceProcessor.Reload(ucGridControlService, lstServiceTestSampleADOs);
                }
                rowCount = (data == null ? 0 : lstServiceTestSampleADOs.Count);
                dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind1_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FillDataToGrid1(this);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind2_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FillDataToGrid2(this);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboChoose_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                ServiceTesaViews = null;
                ServiceTesa = null;
                isChoseTestSample = 0;
                isChoseService = 0;
                TestSampleIdCheckByService = 0;
                ServiceIdCheckByService = 0;
                FillDataToGrid1(this);
                FillDataToGrid2(this);
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
                WaitingManager.Show();
                if (ucGridControlTestSample != null && ucGridControlService != null)
                {
                    object TestSample = TestSampleProcessor.GetDataGridView(ucGridControlTestSample);
                    object Service = ServiceProcessor.GetDataGridView(ucGridControlService);
                    bool success = false;
                    CommonParam param = new CommonParam();
                    if (isChoseService == 1)
                    {
                        if (ServiceIdCheckByService == 0)
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn dịch vụ");
                            return;
                        }

                        if (TestSample is List<HIS.UC.TestSample.ADO.TestSampleADO>)
                        {
                            lstTestSampleADOs = (List<TestSampleADO>)TestSample;

                            if (lstTestSampleADOs != null && lstTestSampleADOs.Count > 0)
                            {
                                //List<long> listServiceMachines = ServiceMachines.Select(p => p.SERVICE_ID).ToList();

                                var dataCheckeds = lstTestSampleADOs.Where(p => p.check1 == true).ToList();

                                //List xoa

                                var dataDeletes = lstTestSampleADOs.Where(o => ServiceTesa.Select(p => p.TEST_SAME_TYPE_ID)
                                    .Contains(o.ID) && o.check1 == false).ToList();


                                //list them
                                var dataCreates = dataCheckeds.Where(o => !ServiceTesa.Select(p => p.TEST_SAME_TYPE_ID)
                                    .Contains(o.ID)).ToList();

                                if (dataDeletes != null && dataDeletes.Count == 0 && dataCreates != null && dataCreates.Count == 0)
                                {
                                    WaitingManager.Hide();
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn mẫu", "Thông báo");
                                    return;
                                }

                                if (dataDeletes != null && dataDeletes.Count > 0)
                                {
                                    var deleteSds = ServiceTesa.Where(o => dataDeletes.Select(p => p.ID)
                                        .Contains(o.TEST_SAME_TYPE_ID)).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "api/HisServiceTesa/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                              deleteSds,
                                              param);
                                    if (deleteResult)
                                        success = true;
                                    ServiceTesa = ServiceTesa.Where(o => !deleteSds.Any(x => x.ID == o.ID)).ToList();
                                }

                                if (dataCreates != null && dataCreates.Count > 0)
                                {
                                    List<HIS_SERVICE_TESA> ServiceTesaCreates = new List<HIS_SERVICE_TESA>();
                                    foreach (var item in dataCreates)
                                    {
                                        HIS_SERVICE_TESA ServiceTesa = new HIS_SERVICE_TESA();
                                        ServiceTesa.SERVICE_ID = ServiceIdCheckByService;
                                        ServiceTesa.TEST_SAME_TYPE_ID = item.ID;
                                        ServiceTesaCreates.Add(ServiceTesa);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<HIS_SERVICE_TESA>>(
                                               "api/HisServiceTesa/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               ServiceTesaCreates,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                        success = true;
                                    AutoMapper.Mapper.CreateMap<HIS_SERVICE_TESA, HIS_SERVICE_TESA>();
                                    var vCreateResults = AutoMapper.Mapper.Map<List<HIS_SERVICE_TESA>, List<HIS_SERVICE_TESA>>(createResult);
                                    ServiceTesa.AddRange(vCreateResults);
                                }

                                lstTestSampleADOs = lstTestSampleADOs.OrderByDescending(p => p.check1).ToList();
                                if (ucGridControlTestSample != null)
                                {
                                    TestSampleProcessor.Reload(ucGridControlTestSample, lstTestSampleADOs);
                                }
                            }
                        }
                    }

                    if (isChoseTestSample == 2)
                    {
                        if (TestSampleIdCheckByTestSample == 0)
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn mẫu");
                            return;
                        }

                        if (Service is List<HIS.UC.Service.ServiceADO>)
                        {
                            lstServiceTestSampleADOs = (List<HIS.UC.Service.ServiceADO>)Service;

                            if (lstServiceTestSampleADOs != null && lstServiceTestSampleADOs.Count > 0)
                            {
                                //List<long> listServiceMachines = ServiceMachine.Select(p => p.MACHINE_ID).ToList();

                                var dataChecked = lstServiceTestSampleADOs.Where(p => p.checkService == true).ToList();
                                //List xoa

                                var dataDelete = lstServiceTestSampleADOs.Where(o => ServiceTesaViews.Select(p => p.SERVICE_ID)
                                    .Contains(o.ID) && o.checkService == false).ToList();

                                //list them
                                var dataCreate = dataChecked.Where(o => !ServiceTesaViews.Select(p => p.SERVICE_ID)
                                    .Contains(o.ID)).ToList();

                                if (dataDelete != null && dataDelete.Count == 0 && dataCreate != null && dataCreate.Count == 0)
                                {
                                    WaitingManager.Hide();
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn dịch vụ", "Thông báo");
                                    return;
                                }

                                if (dataDelete != null && dataDelete.Count > 0)
                                {

                                    var delete = ServiceTesaViews.Where(o => dataDelete.Select(x => x.ID).Contains(o.SERVICE_ID ?? 0)).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "api/HisServiceTesa/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                              delete,
                                              param);
                                    if (deleteResult)
                                        success = true;
                                    ServiceTesaViews = ServiceTesaViews.Where(o => !delete.Any(x => x.ID == o.ID)).ToList();
                                }

                                if (dataCreate != null && dataCreate.Count > 0)
                                {
                                    List<HIS_SERVICE_TESA> ServiceTestSampleCreate = new List<HIS_SERVICE_TESA>();
                                    foreach (var item in dataCreate)
                                    {
                                        HIS_SERVICE_TESA ServiceTestSampleID = new HIS_SERVICE_TESA();
                                        ServiceTestSampleID.TEST_SAME_TYPE_ID = TestSampleIdCheckByTestSample;
                                        ServiceTestSampleID.SERVICE_ID = item.ID;
                                        ServiceTestSampleCreate.Add(ServiceTestSampleID);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<HIS_SERVICE_TESA>>(
                                               "api/HisServiceTesa/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               ServiceTestSampleCreate,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                        success = true;
                                    AutoMapper.Mapper.CreateMap<HIS_SERVICE_TESA, HIS_SERVICE_TESA>();
                                    var vCreateResults = AutoMapper.Mapper.Map<List<HIS_SERVICE_TESA>, List<HIS_SERVICE_TESA>>(createResult);
                                    ServiceTesaViews.AddRange(vCreateResults);
                                }

                                lstServiceTestSampleADOs = lstServiceTestSampleADOs.OrderByDescending(p => p.checkService).ToList();
                                if (ucGridControlService != null)
                                {
                                    ServiceProcessor.Reload(ucGridControlService, lstServiceTestSampleADOs);
                                }
                            }
                        }
                    }
                    MessageManager.Show(this.ParentForm, param, success);
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid1(this);

                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid2(this);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {

                if (e.KeyCode == Keys.Enter)
                {
                    txtKeyword2.Focus();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServiceType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboServiceType.EditValue != null)
                    {
                        HIS_SERVICE_TYPE data = ServiceType.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceType.EditValue.ToString()));
                        if (data != null)
                        {
                            cboServiceType.Properties.Buttons[1].Visible = true;
                            btnFind1.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboServiceType.EditValue = null;
                    cboServiceType.Properties.Buttons[1].Visible = false;
                    cboServiceType.Refresh();
                }

                HisServiceTypeFilter filter = new HisServiceTypeFilter();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServiceType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboServiceType.EditValue == null)
                {
                    cboServiceType.Properties.Buttons[1].Visible = false;
                }
                else
                {
                    cboServiceType.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
