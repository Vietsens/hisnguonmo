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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils.Menu;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Print;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Common.Adapter;
using HIS.Desktop.Plugins.ServiceReqSessionDetail.ADO;
using HIS.UC.ServiceReqDetail;
using HIS.UC.ServiceReqDetail.ADO;
using HIS.Desktop.ADO;

namespace HIS.Desktop.Plugins.ServiceReqSessionDetail
{
    public partial class frmServiceReqSessionDetail : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE> expMestMedicineAlls;
        List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MATERIAL> expMestMaterialAlls;
        List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_BLOOD> expMestBloodsAlls;
        Inventec.Desktop.Common.Modules.Module moduleData = null;
        ServiceReqSessionDetailADO ServiceReqSessionDetailADO;

        internal List<HIS.UC.ServiceReqDetail.ServiceReqDetailADO> lstServiceReqDetailADOs { get; set; }
        #endregion

        #region Construct
        public frmServiceReqSessionDetail(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                SetPrintTypeToMps();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmServiceReqSessionDetail(ServiceReqSessionDetailADO serviceReqSessionDetail, Inventec.Desktop.Common.Modules.Module moduleData)
            : this(moduleData)
        {
            try
            {
                this.ServiceReqSessionDetailADO = serviceReqSessionDetail;
                this.Text = moduleData.text;
                this.moduleData = moduleData;
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
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }

                //Chạy tool sinh resource key language sinh tự động đa ngôn ngữ -> copy vào đây
                //TODO
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ServiceReqSessionDetail.Resources.Lang", typeof(HIS.Desktop.Plugins.ServiceReqSessionDetail.frmServiceReqSessionDetail).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormBidDetail.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void frmServiceReqSessionDetail_Load(object sender, EventArgs e)
        {
            try
            {
                //Gan ngon ngu
                //LoadKeysFromlanguage();
                SetCaptionByLanguageKey();
                //Load du lieu
                List<ServiceReqDetailCommonADO> serviceReqs = GetServiceReq(ServiceReqSessionDetailADO);
                if (serviceReqs != null && serviceReqs.Count() > 0)
                {
                    List<long> expMestIds = new List<long>();
                    expMestIds = serviceReqs.Select(o => o.EXP_MEST_ID).Distinct().ToList();
                    GetDataToGridAll(expMestIds);
                    InitTabPages(serviceReqs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method


        List<string> GetListStringExpLoginname(List<ServiceReqDetailADO> ListServiceReqDetailADO)
        {
            List<string> result = new List<string>();
            try
            {
                List<string> expMestMedicineGroups = new List<string>();
                List<string> expMestMaterialGroups = new List<string>();
                List<string> expMestBloodGroups = new List<string>();
                var expMestMedicineList = ListServiceReqDetailADO.Where(o => o.TYPE == 1).ToList();
                var expMestMaterialList = ListServiceReqDetailADO.Where(o => o.TYPE == 2).ToList();
                var expMestBloodList = ListServiceReqDetailADO.Where(o => o.TYPE == 3).ToList();

                if (expMestMedicineList != null && expMestMedicineList.Count > 0)
                {
                    expMestMedicineGroups = expMestMedicineList.Where(p => !string.IsNullOrEmpty(p.EXP_LOGINNAME))
                    .GroupBy(o => o.EXP_LOGINNAME)
                    .Select(p => p.First().EXP_LOGINNAME)
                    .ToList();
                }
                if (expMestMaterialList != null && expMestMaterialList.Count > 0)
                {
                    expMestMaterialGroups = expMestMaterialList.Where(p => !string.IsNullOrEmpty(p.EXP_LOGINNAME))
                    .GroupBy(o => o.EXP_LOGINNAME)
                    .Select(p => p.First().EXP_LOGINNAME)
                    .ToList();
                }

                if (expMestBloodList != null && expMestBloodList.Count > 0)
                {
                    expMestBloodGroups = expMestBloodList.Where(p => !string.IsNullOrEmpty(p.EXP_LOGINNAME))
                    .GroupBy(o => o.EXP_LOGINNAME)
                    .Select(p => p.First().EXP_LOGINNAME)
                    .ToList();
                }
                result = expMestMedicineGroups.Union(expMestMaterialGroups).Union(expMestBloodGroups).ToList();
            }
            catch (Exception ex)
            {
                result = new List<string>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        List<string> GetListStringApprovalLoginname(List<ServiceReqDetailADO> ListServiceReqDetailADO)
        {
            List<string> result = new List<string>();
            try
            {
                List<string> expMestMedicineGroups = new List<string>();
                List<string> expMestMaterialGroups = new List<string>();
                List<string> expMestBloodGroups = new List<string>();
                var expMestMedicineList = ListServiceReqDetailADO.Where(o => o.TYPE == 1).ToList();
                var expMestMaterialList = ListServiceReqDetailADO.Where(o => o.TYPE == 2).ToList();
                var expMestBloodList = ListServiceReqDetailADO.Where(o => o.TYPE == 3).ToList();

                if (expMestMedicineList != null && expMestMedicineList.Count > 0)
                {
                    expMestMedicineGroups = expMestMedicineList.Where(p => !string.IsNullOrEmpty(p.APPROVAL_LOGINNAME))
                    .GroupBy(o => o.APPROVAL_LOGINNAME)
                    .Select(p => p.First().APPROVAL_LOGINNAME)
                    .ToList();
                }
                if (expMestMaterialList != null && expMestMaterialList.Count > 0)
                {
                    expMestMaterialGroups = expMestMaterialList.Where(p => !string.IsNullOrEmpty(p.APPROVAL_LOGINNAME))
                    .GroupBy(o => o.APPROVAL_LOGINNAME)
                    .Select(p => p.First().APPROVAL_LOGINNAME)
                    .ToList();
                }

                if (expMestBloodList != null && expMestBloodList.Count > 0)
                {
                    expMestBloodGroups = expMestBloodList.Where(p => !string.IsNullOrEmpty(p.APPROVAL_LOGINNAME))
                    .GroupBy(o => o.APPROVAL_LOGINNAME)
                    .Select(p => p.First().APPROVAL_LOGINNAME)
                    .ToList();
                }
                result = expMestMedicineGroups.Union(expMestMaterialGroups).Union(expMestBloodGroups).ToList();
            }
            catch (Exception ex)
            {
                result = new List<string>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        List<string> GetListStringExpTime(List<ServiceReqDetailADO> ListServiceReqDetailADO)
        {
            List<string> result = new List<string>();
            try
            {
                List<string> expMestMedicineGroups = new List<string>();
                List<string> expMestMaterialGroups = new List<string>();
                List<string> expMestBloodGroups = new List<string>();
                var expMestMedicineList = ListServiceReqDetailADO.Where(o => o.TYPE == 1).ToList();
                var expMestMaterialList = ListServiceReqDetailADO.Where(o => o.TYPE == 2).ToList();
                var expMestBloodList = ListServiceReqDetailADO.Where(o => o.TYPE == 3).ToList();
                if (expMestMedicineList != null && expMestMedicineList.Count > 0)
                {
                    expMestMedicineGroups = expMestMedicineList.Where(p => p.EXP_TIME != null)
                    .GroupBy(o => o.EXP_TIME)
                    .Select(p => Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(p.First().EXP_TIME ?? 0))
                    .ToList();
                }
                if (expMestMaterialList != null && expMestMaterialList.Count > 0)
                {
                    expMestMaterialGroups = expMestMaterialList.Where(p => p.EXP_TIME != null)
                      .GroupBy(o => o.EXP_TIME)
                      .Select(p => Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(p.First().EXP_TIME ?? 0))
                      .ToList();
                }
                if (expMestBloodList != null && expMestBloodList.Count > 0)
                {
                    expMestBloodGroups = expMestBloodList.Where(p => p.EXP_TIME != null)
                    .GroupBy(o => o.EXP_TIME)
                    .Select(p => Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(p.First().EXP_TIME ?? 0))
                    .ToList();
                }
                result = expMestMedicineGroups.Union(expMestMaterialGroups).Union(expMestBloodGroups).ToList();
            }
            catch (Exception ex)
            {
                result = new List<string>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void InitTabPages(List<ServiceReqDetailCommonADO> ServiceReqs)
        {
            try
            {
                DevExpress.XtraTab.XtraTabPage[] xtraTabPages = new DevExpress.XtraTab.XtraTabPage[ServiceReqs.Count()];

                for (int i = 0; i < ServiceReqs.Count(); i++)
                {
                    UserControl ucGridControlServiceReq = new UserControl();
                    DevExpress.XtraEditors.PanelControl panelControl = new DevExpress.XtraEditors.PanelControl(); ;
                    UCServiceReqDetailProcessor UCServiceReqProcessor = new UCServiceReqDetailProcessor();

                    ServiceReqDetailInitADO ado = new ServiceReqDetailInitADO();

                    ado.Grid_CustomUnboundColumnData = grid_CustomUnboundColumnData;
                    ado.gridView_RowCellStyle = grid_RowCellStyle;

                    ado.ListServiceReqDetailColumn = new List<ServiceReqDetailColumn>();

                    ServiceReqDetailColumn colStt = new ServiceReqDetailColumn("STT", "STT", 30, false);
                    colStt.VisibleIndex = 0;
                    colStt.UnboundColumnType = UnboundColumnType.Object;
                    ado.ListServiceReqDetailColumn.Add(colStt);

                    ServiceReqDetailColumn colMaPhong = new ServiceReqDetailColumn("Mã", "CODE", 60, false);
                    colMaPhong.VisibleIndex = 1;
                    ado.ListServiceReqDetailColumn.Add(colMaPhong);

                    ServiceReqDetailColumn colTenPhong = new ServiceReqDetailColumn("Tên", "NAME", 100, false);
                    colTenPhong.VisibleIndex = 2;
                    ado.ListServiceReqDetailColumn.Add(colTenPhong);

                    DevExpress.Utils.FormatInfo infoAmount = new DevExpress.Utils.FormatInfo();
                    infoAmount.FormatString = "#,##0.00";
                    infoAmount.FormatType = DevExpress.Utils.FormatType.Custom;
                    ServiceReqDetailColumn colLoaiPhong = new ServiceReqDetailColumn("Số lượng", "AMOUNT", 70, false);
                    colLoaiPhong.VisibleIndex = 3;
                    colLoaiPhong.Format = infoAmount;
                    ado.ListServiceReqDetailColumn.Add(colLoaiPhong);

                    ServiceReqDetailColumn colKhoa = new ServiceReqDetailColumn("Đơn vị tính", "SERVICE_UNIT_NAME", 100, false);
                    colKhoa.VisibleIndex = 4;
                    ado.ListServiceReqDetailColumn.Add(colKhoa);

                    ServiceReqDetailColumn colHDSD = new ServiceReqDetailColumn("HDSD", "TUTORIAL", 200, false);
                    colHDSD.VisibleIndex = 5;
                    ado.ListServiceReqDetailColumn.Add(colHDSD);

                    ucGridControlServiceReq = (UserControl)UCServiceReqProcessor.Run(ado);
                    if (ucGridControlServiceReq != null)
                    {
                        var details = this.lstServiceReqDetailADOs.Where(o => o.EXP_MEST_ID == ServiceReqs[i].EXP_MEST_ID).ToList();
                        long dem = 0;
                        foreach (var detail in details)
                        {
                            detail.STT = ++dem;
                        }
                        panelControl.Controls.Add(ucGridControlServiceReq);
                        ucGridControlServiceReq.Dock = DockStyle.Fill;

                        //var ExpLoginnameList = GetListStringExpLoginname(details);
                        //ServiceReqs[i].EXP_LOGINNAME = String.Join(", ", ExpLoginnameList.Where(p => !String.IsNullOrEmpty(p)).Select(o => o));
                        //var ExpTimeList = GetListStringExpTime(details);
                        //ServiceReqs[i].EXP_TIME = String.Join(", ", ExpTimeList.Where(p => !String.IsNullOrEmpty(p)).Select(o => o));

                        //var ApprovalLoginnameList = GetListStringApprovalLoginname(details);
                        //ServiceReqs[i].APPROVAL_USER_NAME = String.Join(", ", ApprovalLoginnameList.Where(p => !String.IsNullOrEmpty(p)).Select(o => o));
                        UCServiceReqProcessor.Reload(ucGridControlServiceReq, details, ServiceReqs[i]);
                    }

                    DevExpress.XtraTab.XtraTabPage xtraTabPageItem = new DevExpress.XtraTab.XtraTabPage();
                    xtraTabPageItem.Text = ServiceReqs[i].SERVICE_REQ_CODE;

                    panelControl.Dock = DockStyle.Fill;

                    xtraTabPageItem.Controls.Add(panelControl);
                    xtraTabPages[i] = xtraTabPageItem;
                }

                this.xtraTabControl1.TabPages.AddRange(xtraTabPages);

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void grid_RowCellStyle(ServiceReqDetailADO data, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (data != null && data.TYPE == 1)
                {
                    //e.Appearance.ForeColor = Color.Gray; //Giao dịch đã bị hủy => Màu nâu
                    //e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Italic);
                    //e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void grid_CustomUnboundColumnData(ServiceReqDetailADO data, CustomColumnDataEventArgs e)
        {
            try
            {
                //if (e.Column.FieldName == "STT")
                //{
                //    e.Value = e.ListSourceRowIndex + 1;
                //}
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<ServiceReqDetailCommonADO> GetServiceReq(ServiceReqSessionDetailADO serviceReqSessionDetail)
        {
            List<ServiceReqDetailCommonADO> result = new List<ServiceReqDetailCommonADO>();
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();

                if (!String.IsNullOrWhiteSpace(serviceReqSessionDetail.SessionCode))
                {
                    filter.SESSION_CODE__EXACT = serviceReqSessionDetail.SessionCode;
                }
                else if (serviceReqSessionDetail.ServiceReqId != null)
                {
                    filter.ID = serviceReqSessionDetail.ServiceReqId;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info("HIS.Desktop.Plugins.ServiceReqSessionDetail SessionCode is null and ServiceReqId is null");
                    return result;
                }

                var serviceReqs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);

                if (serviceReqs == null || serviceReqs.Count() == 0)
                {
                    return result;
                }

                if (!String.IsNullOrWhiteSpace(serviceReqSessionDetail.SessionCode))
                {
                    serviceReqs = serviceReqs.Where(o => o.INTRUCTION_TIME == serviceReqSessionDetail.IntructionTime).ToList();
                }

                // get exp_mest by service_req
                MOS.Filter.HisExpMestViewFilter expMestFilter = new MOS.Filter.HisExpMestViewFilter();
                expMestFilter.SERVICE_REQ_IDs = serviceReqs.Select(o => o.ID).ToList();
                var expMests = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestFilter, param);

                foreach (var item in serviceReqs)
                {
                    ServiceReqDetailCommonADO serviceReqAdo = new ServiceReqDetailCommonADO(item);
                    serviceReqAdo.SERVICE_REQ_CODE = item.SERVICE_REQ_CODE;
                    if (expMests != null && expMests.Count() > 0)
                    {
                        var checkExpMest = expMests.FirstOrDefault(o => o.SERVICE_REQ_ID == item.ID);
                        if (checkExpMest != null)
                        {
                            serviceReqAdo.EXP_MEST_CODE = checkExpMest.EXP_MEST_CODE;
                            serviceReqAdo.MEDI_STOCK_NAME = checkExpMest.MEDI_STOCK_NAME;
                            //serviceReqAdo.EXP_LOGINNAME = checkExpMest.ex; TODO
                            serviceReqAdo.EXP_MEST_ID = checkExpMest.ID;
                            //serviceReqAdo.EXP_TIME = checkExpMest.ex
                            serviceReqAdo.EXP_MEST_REASON_NAME = checkExpMest.EXP_MEST_REASON_NAME;
                            serviceReqAdo.EXP_MEST_STT_NAME = checkExpMest.EXP_MEST_STT_NAME;
                            //serviceReqAdo.APPROVAL_USER_NAME = checkExpMest.app

                        }
                    }

                    result.Add(serviceReqAdo);
                }
            }
            catch (Exception ex)
            {
                return null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetPrintTypeToMps()
        {
            try
            {
                if (MPS.PrintConfig.PrintTypes == null || MPS.PrintConfig.PrintTypes.Count == 0)
                {
                    MPS.PrintConfig.PrintTypes = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeysFromlanguage()
        {
            try
            {
                var cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataToGridAll(List<long> ExpMestIds)
        {
            try
            {
                WaitingManager.Show();
                lstServiceReqDetailADOs = new List<ServiceReqDetailADO>();
                this.expMestBloodsAlls = new List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_BLOOD>();
                this.expMestMaterialAlls = new List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MATERIAL>();
                this.expMestMedicineAlls = new List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>();
                var medicines = GetDataMedicineAll(ExpMestIds);
                var materials = GetDataMaterialAll(ExpMestIds);
                var bloods = GetDataBloodAll(ExpMestIds);
                lstServiceReqDetailADOs.AddRange(medicines);
                lstServiceReqDetailADOs.AddRange(materials);
                lstServiceReqDetailADOs.AddRange(bloods);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private List<ServiceReqDetailADO> GetDataMedicineAll(List<long> ExpMestIds)
        {
            List<ServiceReqDetailADO> result = new List<ServiceReqDetailADO>();
            try
            {

                CommonParam param = new CommonParam();

                MOS.Filter.HisExpMestMedicineViewFilter filter = new MOS.Filter.HisExpMestMedicineViewFilter();
                filter.EXP_MEST_IDs = ExpMestIds;
                expMestMedicineAlls = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (expMestMedicineAlls != null && expMestMedicineAlls.Count > 0)
                {
                    foreach (var item in expMestMedicineAlls)
                    {
                        ServiceReqDetailADO bidAdo = new ServiceReqDetailADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<ServiceReqDetailADO>(bidAdo, item);
                        bidAdo.ID = item.ID;
                        bidAdo.NAME = item.MEDICINE_TYPE_NAME;
                        bidAdo.CODE = item.MEDICINE_TYPE_CODE;
                        bidAdo.TYPE = 1;// thuoc
                        bidAdo.AMOUNT = item.AMOUNT;
                        bidAdo.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                        bidAdo.TUTORIAL = item.TUTORIAL;
                        result.Add(bidAdo);
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<ServiceReqDetailADO> GetDataMaterialAll(List<long> ExpMestIds)
        {
            List<ServiceReqDetailADO> result = new List<ServiceReqDetailADO>();
            try
            {
                CommonParam param = new CommonParam();

                MOS.Filter.HisExpMestMaterialViewFilter filter = new MOS.Filter.HisExpMestMaterialViewFilter();
                filter.EXP_MEST_IDs = ExpMestIds;
                this.expMestMaterialAlls = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MATERIAL>>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (this.expMestMaterialAlls != null && this.expMestMaterialAlls.Count > 0)
                {
                    foreach (var item in this.expMestMaterialAlls)
                    {
                        ServiceReqDetailADO bidAdo = new ServiceReqDetailADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<ServiceReqDetailADO>(bidAdo, item);
                        bidAdo.ID = item.ID;
                        bidAdo.NAME = item.MATERIAL_TYPE_NAME;
                        bidAdo.CODE = item.MATERIAL_TYPE_CODE;
                        bidAdo.TYPE = 2;// vat tu
                        bidAdo.AMOUNT = item.AMOUNT;
                        bidAdo.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                        result.Add(bidAdo);
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<ServiceReqDetailADO> GetDataBloodAll(List<long> ExpMestIds)
        {
            List<ServiceReqDetailADO> result = new List<ServiceReqDetailADO>();
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestBloodViewFilter filter = new MOS.Filter.HisExpMestBloodViewFilter();
                filter.EXP_MEST_IDs = ExpMestIds;
                this.expMestBloodsAlls = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_BLOOD>>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_BLOOD_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (expMestBloodsAlls != null && expMestBloodsAlls.Count > 0)
                {
                    foreach (var item in expMestBloodsAlls)
                    {
                        ServiceReqDetailADO bidAdo = new ServiceReqDetailADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<ServiceReqDetailADO>(bidAdo, item);
                        bidAdo.ID = item.ID;
                        bidAdo.NAME = item.BLOOD_TYPE_NAME;
                        bidAdo.CODE = item.BLOOD_TYPE_CODE;
                        bidAdo.TYPE = 3;// mau
                        bidAdo.AMOUNT = 1;
                        bidAdo.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                        result.Add(bidAdo);
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        #region in
        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = true;
            try
            {
                WaitingManager.Show();
                //MPS.Processor.Mps000119.PDO.Mps000119PDO Mps000119PDO = new MPS.Processor.Mps000119.PDO.Mps000119PDO(bidDetail, bidprintAdo);
                //MPS.ProcessorBase.Core.PrintData PrintData = null;

                //if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                //{
                //    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000119PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, null);
                //}
                //else
                //{
                //    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000119PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, null);
                //}
                //WaitingManager.Hide();
                //MPS.MpsPrinter.Run(PrintData);
                //MPS.Core.Mps000119.Mps000119RDO mps119Rdo = new MPS.Core.Mps000119.Mps000119RDO(bidDetail, bidprintAdo);
                //if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                //{
                //    result = MPS.Printer.Run(printTypeCode, fileName, mps119Rdo, MPS.Printer.PreviewType.PrintNow);
                //}
                //else
                //    result = MPS.Printer.Run(printTypeCode, fileName, mps119Rdo);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void cboPrint_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditor = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), PrintStoreLocation.ROOT_PATH);
                richEditor.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__ChiTietGoiThau__MPS000119, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            cboPrint_Click(null, null);
        }
        #endregion
        #endregion

        #region Public method

        #endregion
    }
}
