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
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using MOS.Filter;
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.Plugins.ServiceKsk.Entity;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.ADO;
using AutoMapper;
using ACS.Filter;
using HIS.Desktop.LocalStorage.BackendData;
using ACS.EFMODEL.DataModels;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.UC.Service;
using HIS.UC.Service.ADO;
using HIS.UC.Ksk;
using HIS.UC.Ksk.ADO;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using DevExpress.XtraBars;
using MOS.SDO;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LibraryMessage;
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Modules;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.ServiceKsk
{
    public partial class UCServiceKsk : HIS.Desktop.Utility.UserControlBase
    {
        List<HIS_SERVICE_TYPE> ServiceType { get; set; }

        internal Inventec.Desktop.Common.Modules.Module currentModule;
        UCKskProcessor KskProcessor;
        UCServiceProcessor ServiceProcessor;
        UserControl ucGridControlService;
        UserControl ucGridControlKsk;
        internal List<HIS.UC.Ksk.KskADO> lstKskADOs { get; set; }
        internal List<HIS.UC.Service.ServiceADO> lstServiceKskADOs { get; set; }
        List<HIS_KSK> listKsk;
        List<V_HIS_SERVICE> listService;
        ServiceADO ServiceIdCheckByService = new ServiceADO();
        long isChoseService;
        long isChoseKsk;
        HIS_KSK KskIdCheckByKsk = new HIS_KSK();
        bool isCheckAll;
        //internal long servicetypeId;
        List<HIS_KSK_SERVICE> ServiceKsks { get; set; }
        List<HIS_KSK_SERVICE> ServiceKskViews { get; set; }
        //List<HIS_SERVICE_PATY> ServiceKsk { get; set; }
        V_HIS_SERVICE currentService;
        HIS.UC.Ksk.KskADO currentCopyKskAdo { get; set; }
        HIS.UC.Service.ServiceADO currentCopyServiceAdo { get; set; }
        KskADO clickKsk;
        int positionHandle = -1;
        Module moduleData;

        string moduleLink = "HIS.Desktop.Plugins.ServiceKsk";
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;

        List<V_HIS_SERVICE> listServiceFilter;

        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;

        int rowCount1 = 0;
        int dataTotal1 = 0;
        int startPage1 = 0;

        List<long> servicePatyIds;
        List<long> serviceRoomIds;

        string config = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.KSK");

        public UCServiceKsk(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
            this.moduleData = currentModule;
        }
        public UCServiceKsk(Inventec.Desktop.Common.Modules.Module currentModule, long ServiceType)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                if (this.currentModule != null)
                {
                    this.Text = currentModule.text;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public UCServiceKsk(V_HIS_SERVICE serviceData, Inventec.Desktop.Common.Modules.Module _moduleData)
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

        private void UCServiceKsk_Load(object sender, EventArgs e)
        {
            try
            {

                //var a = BranchDataWorker.ServicePatyInBranchs;
                var b = BackendDataWorker.Get<V_HIS_SERVICE_PATY>();

                servicePatyIds = BackendDataWorker.Get<V_HIS_SERVICE_PATY>().Where(p => p.BRANCH_ID == BranchDataWorker.GetCurrentBranchId() && p.PATIENT_TYPE_CODE == config).Select(o => o.SERVICE_ID).ToList();
                serviceRoomIds = BackendDataWorker.Get<V_HIS_SERVICE_ROOM>().Select(o => o.SERVICE_ID).ToList();

                listServiceFilter = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.IS_ACTIVE == 1 && servicePatyIds.Contains(o.ID)
                    && serviceRoomIds.Contains(o.ID)
                    && o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                    && o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                    && o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU).ToList();

                SetCaptionByLanguageKey();
                WaitingManager.Show();
                ValidateForm();
                InitControlState();
                LoadDataToCombo();
                LoadComboStatus();
                LoadComboKSKContract();
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
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
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
                        if (item.KEY == chkAutoSelectPrice.Name)
                        {
                            chkAutoSelectPrice.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                            var lstCheckAll = lstServiceKskADOs;
                            List<HIS.UC.Service.ServiceADO> lstChecks = new List<HIS.UC.Service.ServiceADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var ServiceCheckedNum = lstServiceKskADOs.Where(o => o.checkService == true).Count();
                                var ServiceNum = lstServiceKskADOs.Count();
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
                                        if (item.ID > 0)
                                        {
                                            item.checkService = true;
                                            //khi check chọn kiểm tra cấu hình phòng thực hiện nếu chỉ có 1 thì hiển thị luôn
                                            var serviceRoom = BackendDataWorker.Get<V_HIS_SERVICE_ROOM>().Where(o => o.SERVICE_ID == item.ID && o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL).ToList();
                                            if (serviceRoom != null && serviceRoom.Count == 1)
                                            {
                                                item.ROOM_ID = serviceRoom.First().ROOM_ID;
                                            }

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
                                        if (item.ID > 0)
                                        {
                                            item.checkService = false;
                                            item.ROOM_ID = 0;
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
                                ProcessTotalPrice(lstChecks);
                                ProcessTotalPriceKSK(lstChecks);
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

        private void ProcessTotalPrice(List<ServiceADO> lstChecks)
        {
            try
            {
                LblTongTien.Text = "0";
                var checkData = lstChecks.Where(o => o.checkService).ToList();
                var tongTien = checkData.Sum(s => s.AMOUNT * (s.PRICE ?? 0) * (1 + (s.VAT_RATIO ?? 0)));
                LblTongTien.Text = Inventec.Common.Number.Convert.NumberToString(tongTien, ConfigApplications.NumberSeperator);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessTotalPriceKSK(List<ServiceADO> lstChecks)
        {
            try
            {
                if (lstChecks.Count > 0)
                {
                    labelTotalPriceByKSK.Text = "0";
                    var checkData = lstChecks.Where(o => o.checkService).ToList();
                    var tongTienKSK = checkData.Sum(s => s.AMOUNT * (s.PRICE_KSK ?? 0) * (1 + (s.VAT_RATIO_KSK ?? 0)));
                    labelTotalPriceByKSK.Text = Inventec.Common.Number.Convert.NumberToString(tongTienKSK, ConfigApplications.NumberSeperator);
                    if (Convert.ToInt64(cboChoose.EditValue) == 2)
                        lblTotal.Text = Inventec.Common.Number.Convert.NumberToString(checkData.Count());
                }
                else
                {

                }
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
                    this.Text = moduleData.text;
                }
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ServiceKsk.Resources.Lang", typeof(HIS.Desktop.Plugins.ServiceKsk.UCServiceKsk).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCServiceKsk.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboChoose.Properties.NullText = Inventec.Common.Resource.Get.Value("UCServiceKsk.cboChoose.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCServiceKsk.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind2.Text = Inventec.Common.Resource.Get.Value("UCServiceKsk.btnFind2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UCServiceKsk.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind1.Text = Inventec.Common.Resource.Get.Value("UCServiceKsk.btnFind1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboServiceType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCServiceKsk.cboServiceType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword2.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCServiceKsk.txtKeyword2.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword1.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCServiceKsk.txtKeyword1.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("UCServiceKsk.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("UCServiceKsk.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("UCServiceKsk.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DeleteClick(KskADO data)
        {
            try
            {
                if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn xóa dữ liệu không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    CommonParam param = new CommonParam();
                    var deleteApi = new BackendAdapter(param).Post<bool>("api/HisKsk/Delete", ApiConsumer.ApiConsumers.MosConsumer, data.ID, param);
                    if (deleteApi)
                    {
                        FillDataToGrid2(this);
                    }

                    MessageManager.Show(this.ParentForm, param, deleteApi);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LockClick(KskADO data)
        {
            try
            {
                if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn bỏ khóa dữ liệu không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    HIS_KSK dataLock = new HIS_KSK();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_KSK>(dataLock, data);
                    var lockApi = new BackendAdapter(param).Post<HIS_KSK>("api/HisKsk/ChangeLock", ApiConsumer.ApiConsumers.MosConsumer, dataLock.ID, param);
                    if (lockApi != null)
                    {
                        success = true;
                        FillDataToGrid2(this);
                    }
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UnLockClick(KskADO data)
        {
            try
            {
                if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn khóa dữ liệu không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    HIS_KSK dataLock = new HIS_KSK();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_KSK>(dataLock, data);
                    var lockApi = new BackendAdapter(param).Post<HIS_KSK>("api/HisKsk/ChangeLock", ApiConsumer.ApiConsumers.MosConsumer, dataLock.ID, param);
                    if (lockApi != null)
                    {
                        success = true;
                        FillDataToGrid2(this);
                    }
                    MessageManager.Show(this.ParentForm, param, success);
                }
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
                ado.Check__Enable_CheckedChanged = Check__Enable_CheckedChanged;
                ado.btn_Radio_Enable_Click2 = btn_Radio_Enable_Click2;

                ServiceColumn colRadio2 = new ServiceColumn("   ", "radioService", 30, true);
                colRadio2.VisibleIndex = 0;
                colRadio2.Visible = false;
                colRadio2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListServiceColumn.Add(colRadio2);

                ServiceColumn colCheck2 = new ServiceColumn("   ", "checkService", 30, true);
                colCheck2.VisibleIndex = 1;
                colCheck2.image = imageCollectionService.Images[0];
                colCheck2.Visible = false;
                colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListServiceColumn.Add(colCheck2);

                ServiceColumn colMaDichvu = new ServiceColumn("Mã dịch vụ", "SERVICE_CODE", 50, false);
                colMaDichvu.VisibleIndex = 2;
                ado.ListServiceColumn.Add(colMaDichvu);

                ServiceColumn colTenDichvu = new ServiceColumn("Tên dịch vụ", "SERVICE_NAME", 230, false);
                colTenDichvu.VisibleIndex = 3;
                ado.ListServiceColumn.Add(colTenDichvu);

                ServiceColumn colPhongThucHien = new ServiceColumn("Phòng thực hiện", "ROOM_ID", 80, true);
                colPhongThucHien.VisibleIndex = 4;
                ado.ListServiceColumn.Add(colPhongThucHien);

                ServiceColumn colSoLuong = new ServiceColumn("Số lượng", "AMOUNT", 50, true);
                colSoLuong.VisibleIndex = 5;
                ado.ListServiceColumn.Add(colSoLuong);

                ServiceColumn colGia = new ServiceColumn("Giá theo gói", "PRICE", 80, true);
                colGia.VisibleIndex = 6;
                colGia.Format = new DevExpress.Utils.FormatInfo();
                colGia.Format.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                colGia.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.ListServiceColumn.Add(colGia);

                ServiceColumn colVAT = new ServiceColumn("VAT theo gói", "VAT_RATIO", 70, true);
                colVAT.VisibleIndex = 7;
                colVAT.Format = new DevExpress.Utils.FormatInfo();
                colVAT.Format.FormatString = "{0:0.##}";
                colVAT.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.ListServiceColumn.Add(colVAT);

                ServiceColumn colPriceKSK = new ServiceColumn("Giá KSK", "PRICE_KSK", 80, 8, true, false, null, "Giá theo chính sách giá được khai báo tương ứng với đối tượng Khám sức khỏe");
                //colPriceKSK.AllowEdit = false;
                //colPriceKSK.Tooltip = "Giá theo chính sách giá được khai báo tương ứng với đối tượng Khám sức khỏe";
                colPriceKSK.Format = new DevExpress.Utils.FormatInfo();
                colPriceKSK.Format.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                colPriceKSK.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                colPriceKSK.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                colPriceKSK.VisibleIndex = 8;
                ado.ListServiceColumn.Add(colPriceKSK);

                ServiceColumn colVatKSK = new ServiceColumn("VAT KSK", "VAT_RATIO_KSK", 80, true);
                colVatKSK.Tooltip = "VAT theo chính sách giá được khai báo tương ứng với đối tượng Khám sức khỏe";
                colVatKSK.AllowEdit = false;
                colPriceKSK.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                colVatKSK.VisibleIndex = 9;
                ado.ListServiceColumn.Add(colVatKSK);

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

        private void gridViewService_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                object Service = ServiceProcessor.GetDataGridView(ucGridControlService);
                if (Service is List<HIS.UC.Service.ServiceADO>)
                {
                    var lstServiceADOs = (List<HIS.UC.Service.ServiceADO>)Service;
                    if (lstServiceADOs != null && lstServiceADOs.Count > 0)
                    {
                        var dataChecked = lstServiceADOs.Where(p => p.checkService).ToList();
                        if (dataChecked != null && dataChecked.Count > 0)
                        {
                            ProcessTotalPrice(dataChecked);
                            ProcessTotalPriceKSK(dataChecked);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string AddStringByConfig(int num)
        {
            string str = "";
            try
            {
                if (num > 0)
                {
                    for (int i = 1; i <= num; i++)
                    {
                        str += "0";
                    }
                }
                else
                {
                    return str = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return str = "";
            }
            return str;
        }

        private void gridViewKsk_MouseKsk(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseKsk == 2)
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
                            var lstCheckAll = lstKskADOs;
                            List<HIS.UC.Ksk.KskADO> lstChecks = new List<HIS.UC.Ksk.KskADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var KskCheckedNum = lstKskADOs.Where(o => o.check1 == true).Count();
                                var KsktmNum = lstKskADOs.Count();
                                if (hi.Column.Image == imageCollectionKsk.Images[0])
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = imageCollectionKsk.Images[1];
                                }
                                else if (hi.Column.Image == imageCollectionKsk.Images[1])
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = imageCollectionKsk.Images[0];
                                }

                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            if (item.IS_ACTIVE == 1)
                                            {
                                                item.check1 = true;
                                            }
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
                                KskProcessor.Reload(ucGridControlKsk, lstChecks);
                            }
                            ProcessTotal(lstChecks.Where(o => o.check1).Count());
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

        private void ProcessTotal(int count)
        {
            try
            {
                if (Convert.ToInt64(cboChoose.EditValue) == 1)
                    lblTotal.Text = Inventec.Common.Number.Convert.NumberToString(count);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Check__Enable_CheckedChanged(ServiceADO data)
        {
            try
            {
                object Service = ServiceProcessor.GetDataGridView(ucGridControlService);
                if (Service is List<HIS.UC.Service.ServiceADO>)
                {

                    var lstServiceADOs = (List<HIS.UC.Service.ServiceADO>)Service;
                    if (lstServiceADOs != null && lstServiceADOs.Count > 0)
                    {
                        foreach (var item in lstServiceADOs)
                        {
                            if (item.checkService && chkAutoSelectPrice.Checked)
                            {
                                item.PRICE = item.PRICE_KSK;
                                item.VAT_RATIO = item.VAT_RATIO_KSK;
                            }
                        }
                        var dataChecked = lstServiceADOs.Where(p => p.checkService).ToList();
                        if (dataChecked != null && dataChecked.Count > 0)
                        {
                            ProcessTotalPrice(dataChecked);
                            ProcessTotalPriceKSK(dataChecked);
                        }
                        else
                        {
                            ProcessTotalPrice(new List<HIS.UC.Service.ServiceADO>());
                            ProcessTotalPriceKSK(new List<HIS.UC.Service.ServiceADO>());
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Check__Enable_CheckedChanged2(KskADO data)
        {
            try
            {
                object ksk = KskProcessor.GetDataGridView(ucGridControlKsk);
                if (ksk is List<HIS.UC.Ksk.KskADO>)
                {
                    var lstKskADOs = (List<HIS.UC.Ksk.KskADO>)ksk;
                    if (lstKskADOs != null && lstKskADOs.Count > 0)
                    {
                        var dataChecked = lstKskADOs.Where(p => p.check1).ToList();
                        if (dataChecked != null && dataChecked.Count > 0)
                        {
                            ProcessTotal(dataChecked.Count());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btn_Radio_Enable_Click2(ServiceADO data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                MOS.Filter.HisKskServiceFilter filter = new HisKskServiceFilter();
                filter.SERVICE_ID = data.ID;
                ServiceIdCheckByService = data;
                ServiceKsks = new BackendAdapter(param).Get<List<HIS_KSK_SERVICE>>(
                                    "api/HisKskService/Get",
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);

                List<HIS.UC.Ksk.KskADO> dataNew = new List<HIS.UC.Ksk.KskADO>();
                dataNew = (from r in listKsk select new KskADO(r)).ToList();
                if (ServiceKsks != null && ServiceKsks.Count > 0)
                {
                    foreach (var itemKsk in ServiceKsks)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == itemKsk.KSK_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }
                }
                if (data != null)
                {
                    LblTongTien.Text = "0";
                    var tongTien = data.AMOUNT * (data.PRICE ?? 0) * (1 + (data.VAT_RATIO ?? 0));
                    LblTongTien.Text = Inventec.Common.Number.Convert.NumberToString(tongTien, ConfigApplications.NumberSeperator);

                    labelTotalPriceByKSK.Text = "0";
                    var tongTienKSK = data.AMOUNT * (data.PRICE_KSK ?? 0) * (1 + (data.VAT_RATIO_KSK ?? 0));
                    labelTotalPriceByKSK.Text = Inventec.Common.Number.Convert.NumberToString(tongTienKSK, ConfigApplications.NumberSeperator);
                }

                dataNew = dataNew.OrderByDescending(p => p.check1).ToList();
                if (ucGridControlKsk != null)
                {
                    KskProcessor.Reload(ucGridControlKsk, dataNew);
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

        private void InitUcgrid2()
        {
            try
            {
                KskProcessor = new UCKskProcessor();
                KskInitADO ado = new KskInitADO();
                ado.ListKskColumn = new List<UC.Ksk.KskColumn>();
                ado.gridViewKsk_MouseDownKsk = gridViewKsk_MouseKsk;
                ado.btn_Radio_Enable_Click = btn_Radio_Enable_Click;
                ado.gridView_RowCellClick = gridViewKsk_RowCellClick;
                ado.Check__Enable_CheckedChanged2 = Check__Enable_CheckedChanged2;
                ado.gridViewKsk_CellValueChanged = gridViewKsk_CellValueChanged;
                ado.unLockClick = UnLockClick;
                ado.deleteClick = DeleteClick;
                ado.lockClick = LockClick;

                KskColumn colRadio1 = new KskColumn("   ", "radio1", 20, true);
                colRadio1.VisibleIndex = 0;
                colRadio1.Visible = false;
                colRadio1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListKskColumn.Add(colRadio1);

                KskColumn colCheck1 = new KskColumn("   ", "check1", 20, true);
                colCheck1.VisibleIndex = 1;
                colCheck1.image = imageCollectionKsk.Images[0];
                colCheck1.Visible = false;
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListKskColumn.Add(colCheck1);

                KskColumn colLock = new KskColumn("   ", "Lock", 20, true);
                colLock.VisibleIndex = 2;
                colLock.Visible = false;
                ado.ListKskColumn.Add(colLock);

                KskColumn colDelete = new KskColumn("   ", "Delete", 20, true);
                colDelete.VisibleIndex = 3;
                colDelete.Visible = false;
                ado.ListKskColumn.Add(colDelete);

                KskColumn colMaPhong = new KskColumn("Mã nhóm", "KSK_CODE", 60, false);
                colMaPhong.VisibleIndex = 4;
                ado.ListKskColumn.Add(colMaPhong);

                KskColumn colTenPhong = new KskColumn("Tên nhóm", "KSK_NAME", 100, false);
                colTenPhong.VisibleIndex = 5;
                ado.ListKskColumn.Add(colTenPhong);

                this.ucGridControlKsk = (UserControl)KskProcessor.Run(ado);
                if (ucGridControlKsk != null)
                {
                    this.panelControl2.Controls.Add(this.ucGridControlKsk);
                    this.ucGridControlKsk.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewKsk_RowCellClick(KskADO data)
        {
            try
            {
                this.clickKsk = data;
                FillDataKskToEditor(data);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                btnSaveKsk.Enabled = data.IS_ACTIVE == 1 ? true : false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewKsk_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                object dataKsk = KskProcessor.GetDataGridView(ucGridControlKsk);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("dataKsk----:", dataKsk));

                if (dataKsk is List<HIS.UC.Ksk.KskADO>)
                {
                    var lstKskADOs = (List<HIS.UC.Ksk.KskADO>)dataKsk;
                    if (lstKskADOs != null && lstKskADOs.Count > 0)
                    {
                        var dataChecked = lstKskADOs.Where(p => p.check1).ToList();
                        if (dataChecked != null && dataChecked.Count > 0)
                        {
                            ProcessTotal(dataChecked.Count());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataKskToEditor(KskADO data)
        {
            try
            {
                txtCode.Text = data.KSK_CODE;
                txtName.Text = data.KSK_NAME;
                cboKSKContract.EditValue = data.KSK_CONTRACT_ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btn_Radio_Enable_Click(HIS_KSK data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisKskServiceFilter filter = new HisKskServiceFilter();
                filter.KSK_ID = data.ID;
                KskIdCheckByKsk = data;
                ServiceKskViews = new BackendAdapter(param).Get<List<HIS_KSK_SERVICE>>(
                                         "api/HisKskService/Get",
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);

                List<HIS.UC.Service.ServiceADO> dataNew = new List<HIS.UC.Service.ServiceADO>();
                dataNew = (from r in listService select new HIS.UC.Service.ServiceADO(r)).ToList();
                if (ServiceKskViews != null && ServiceKskViews.Count > 0)
                {

                    foreach (var itemService in ServiceKskViews)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == itemService.SERVICE_ID);
                        if (check != null)
                        {
                            check.checkService = true;
                            check.ROOM_ID = itemService.ROOM_ID;
                            check.AMOUNT = itemService.AMOUNT;
                            if (itemService.PRICE != null)
                                check.PRICE = itemService.PRICE;
                            if (itemService.VAT_RATIO != null)
                                check.VAT_RATIO = itemService.VAT_RATIO * 100;

                            V_HIS_SERVICE_PATY price = BackendDataWorker.Get<V_HIS_SERVICE_PATY>().Where(o => o.SERVICE_ID == itemService.SERVICE_ID && o.PATIENT_TYPE_CODE == config && o.IS_ACTIVE == 1)
                                    .OrderByDescending(o => o.PRIORITY).ThenByDescending(o => o.ID).FirstOrDefault();
                            if (price != null)
                            {
                                check.PRICE_KSK = price.PRICE;
                                check.VAT_RATIO_KSK = price.VAT_RATIO;
                            }
                        }

                    }
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataNew), dataNew));
                    dataNew = dataNew.OrderByDescending(p => p.checkService).ToList();

                    if (ucGridControlService != null)
                    {
                        ServiceProcessor.Reload(ucGridControlService, dataNew);
                        ProcessTotalPrice(dataNew);
                        ProcessTotalPriceKSK(dataNew);
                    }
                    ProcessTotalPrice(dataNew);
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

        private void FillDataToGrid2(UCServiceKsk uCServiceKsk)
        {
            try
            {
                WaitingManager.Show();
                int numPageSize = 0;
                if (ucPaging2.pagingGrid != null)
                {
                    numPageSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging1(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPaging2.Init(LoadPaging1, param, numPageSize, (DevExpress.XtraGrid.GridControl)this.KskProcessor.GetGridControl(this.ucGridControlKsk));
                WaitingManager.Hide();

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPaging1(object param)
        {
            try
            {
                startPage1 = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage1, limit);

                WaitingManager.Show();
                listKsk = new List<HIS_KSK>();

                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_KSK>> apiResult = null;

                MOS.Filter.HisKskFilter KskFillter = new HisKskFilter();
                KskFillter.ORDER_FIELD = "MODIFY_TIME";
                KskFillter.ORDER_DIRECTION = "DESC";
                KskFillter.KEY_WORD = txtKeyword2.Text;
                if ((long)cboChoose.EditValue == 2)
                {
                    isChoseKsk = (long)cboChoose.EditValue;
                }

                apiResult = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_KSK>>(
                   "api/HisKsk/Get",
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                      KskFillter,
                    paramCommon);

                lstKskADOs = new List<KskADO>();
                if (apiResult != null)
                {
                    var sar = apiResult.Data;
                    if (sar != null)
                    {
                        listKsk = sar;
                        foreach (var item in listKsk)
                        {
                            KskADO roomaccountADO = new KskADO(item);
                            if (isChoseKsk == 2)
                            {
                                roomaccountADO.isKeyChoose = true;
                            }
                            lstKskADOs.Add(roomaccountADO);
                        }
                        rowCount1 = (sar == null ? 0 : sar.Count);
                        dataTotal1 = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }


                    if (ServiceKsks != null && ServiceKsks.Count > 0)
                    {
                        foreach (var itemUsername in ServiceKsks)
                        {
                            var check = lstKskADOs.FirstOrDefault(o => o.ID == itemUsername.KSK_ID && o.IS_ACTIVE == 1);
                            if (check != null)
                            {
                                check.check1 = true;
                            }
                        }
                    }
                    lstKskADOs = lstKskADOs.OrderByDescending(p => p.check1).Distinct().ToList();

                    if (ucGridControlKsk != null)
                    {
                        KskProcessor.Reload(ucGridControlKsk, lstKskADOs);
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

        private void FillDataToGrid1(UCServiceKsk UCServiceKsk)
        {
            try
            {
                WaitingManager.Show();
                int numPageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;

                ucPaging1.Init(LoadPaging, param, numPageSize);
                WaitingManager.Hide();

                //FillDataToGridService();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;

                CommonParam paramCommon = new CommonParam(startPage, limit);

                listService = new List<V_HIS_SERVICE>();
                var rs = new List<V_HIS_SERVICE>();

                if ((long)cboChoose.EditValue == 1)
                {
                    isChoseService = (long)cboChoose.EditValue;
                }

                if (servicePatyIds != null && servicePatyIds.Count > 0 && serviceRoomIds != null && serviceRoomIds.Count > 0)
                {
                    if (!string.IsNullOrEmpty(txtKeyword1.Text))
                    {
                        if (cboServiceType.EditValue != null)
                        {
                            rs = listServiceFilter.Where(o =>
                                (o.SERVICE_CODE.ToUpper().Contains(txtKeyword1.Text.Trim().ToUpper()) || o.SERVICE_NAME.ToUpper().Contains(txtKeyword1.Text.Trim().ToUpper()))
                                && o.SERVICE_TYPE_ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceType.EditValue.ToString())
                                ).OrderBy(p => p.SERVICE_CODE).ToList();
                        }
                        else
                        {
                            rs = listServiceFilter.Where(o =>
                                (o.SERVICE_CODE.ToUpper().Contains(txtKeyword1.Text.Trim().ToUpper()) || o.SERVICE_NAME.ToUpper().Contains(txtKeyword1.Text.Trim().ToUpper()))
                                ).OrderBy(p => p.SERVICE_CODE).ToList();
                        }
                    }
                    else if (cboServiceType.EditValue != null)
                    {
                        rs = listServiceFilter.Where(o =>
                                o.SERVICE_TYPE_ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceType.EditValue.ToString())
                                ).OrderBy(p => p.SERVICE_CODE).ToList();
                    }
                    else if (cboServiceType.EditValue == null)
                    {
                        rs = listServiceFilter.OrderBy(p => p.SERVICE_CODE).ToList();
                    }
                }

                lstServiceKskADOs = new List<ServiceADO>();

                if (rs != null && rs.Count > 0)
                {
                    dataTotal = rs.Count;
                    rs = rs.Skip(startPage).Take(limit).ToList();
                    listService = rs;
                    foreach (var item in listService)
                    {
                        ServiceADO ServiceKskADO = new ServiceADO(item);
                        if (isChoseService == 1)
                        {
                            ServiceKskADO.isKeyChooseService = true;
                        }
                        var amount = BackendDataWorker.Get<HIS_KSK_SERVICE>().Where(o => o.SERVICE_ID == item.ID)
                                .OrderByDescending(o => o.ID).FirstOrDefault();
                        if (amount != null)
                        {
                            ServiceKskADO.AMOUNT = amount.AMOUNT;
                        }

                        V_HIS_SERVICE_PATY price = BackendDataWorker.Get<V_HIS_SERVICE_PATY>().Where(o => o.SERVICE_ID == item.ID && o.PATIENT_TYPE_CODE == config && o.IS_ACTIVE == 1)
                                .OrderByDescending(o => o.PRIORITY).ThenByDescending(o => o.ID).FirstOrDefault();
                        if (price != null)
                        {
                            ServiceKskADO.PRICE_KSK = price.PRICE;
                            ServiceKskADO.VAT_RATIO_KSK = price.VAT_RATIO;
                        }

                        lstServiceKskADOs.Add(ServiceKskADO);
                    }
                }

                if (ServiceKskViews != null && ServiceKskViews.Count > 0)
                {
                    foreach (var itemUsername in ServiceKskViews)
                    {
                        var check = lstServiceKskADOs.FirstOrDefault(o => o.ID == itemUsername.SERVICE_ID);
                        if (check != null)
                        {
                            check.checkService = true;
                            var serviceRoom = BackendDataWorker.Get<HIS_KSK_SERVICE>().Where(o => o.SERVICE_ID == itemUsername.SERVICE_ID).OrderByDescending(o => o.ID).FirstOrDefault();
                            if (serviceRoom != null)
                            {
                                check.ROOM_ID = serviceRoom.ROOM_ID;
                            }
                        }
                    }
                }

                lstServiceKskADOs = lstServiceKskADOs.OrderByDescending(p => p.checkService).Distinct().ToList();

                rowCount = (lstServiceKskADOs == null ? 0 : lstServiceKskADOs.Count);
                if (ucGridControlService != null)
                {
                    ServiceProcessor.Reload(ucGridControlService, lstServiceKskADOs);
                    ProcessTotalPrice(lstServiceKskADOs);
                    ProcessTotalPriceKSK(lstServiceKskADOs);
                }

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid1_Default(UCServiceKsk UCServiceKsk)
        {
            //try
            //{
            //    int numPageSize;
            //    if (ucPaging1.pagingGrid != null)
            //    {
            //        numPageSize = ucPaging1.pagingGrid.PageSize;
            //    }
            //    else
            //    {
            //        numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
            //    }

            //    FillDataToGridService_Default(new CommonParam(0, numPageSize));

            //    CommonParam param = new CommonParam();
            //    ucPaging1.Init(FillDataToGridService_Default, param, numPageSize);
            //}
            //catch (Exception ex)
            //{

            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void FillDataToGridService()
        {
            try
            {
                WaitingManager.Show();

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridService_Default(object data)
        {
            //try
            //{
            //    WaitingManager.Show();
            //    listService = new List<V_HIS_SERVICE>();
            //    int start = ((CommonParam)data).Start ?? 0;
            //    int limit = ((CommonParam)data).Limit ?? 0;
            //    CommonParam param = new CommonParam(start, limit);
            //    MOS.Filter.HisServiceViewFilter ServiceFillter = new HisServiceViewFilter();
            //    ServiceFillter.IS_ACTIVE = 1;
            //    ServiceFillter.ID = this.currentService.ID;

            //    if (cboServiceType.EditValue != null)

            //        ServiceFillter.SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceType.EditValue ?? "0").ToString());

            //    if ((long)cboChoose.EditValue == 1)
            //    {
            //        isChoseService = (long)cboChoose.EditValue;
            //    }

            //    var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE>>(
            //                                         "api/HisService/GetView",
            //         HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
            //         ServiceFillter,
            //         param);

            //    lstServiceKskADOs = new List<ServiceADO>();

            //    if (rs != null && rs.Data.Count > 0)
            //    {

            //        listService = rs.Data;
            //        foreach (var item in listService)
            //        {
            //            ServiceADO ServiceKskADO = new ServiceADO(item);
            //            if (isChoseService == 1)
            //            {
            //                ServiceKskADO.isKeyChooseService = true;
            //                ServiceKskADO.radioService = true;
            //            }
            //            lstServiceKskADOs.Add(ServiceKskADO);
            //        }
            //    }

            //    if (ServiceKskViews != null && ServiceKskViews.Count > 0)
            //    {
            //        foreach (var itemUsername in ServiceKskViews)
            //        {
            //            var check = lstServiceKskADOs.FirstOrDefault(o => o.ID == itemUsername.SERVICE_ID);
            //            if (check != null)
            //            {
            //                check.checkService = true;
            //            }
            //        }
            //    }

            //    lstServiceKskADOs = lstServiceKskADOs.OrderByDescending(p => p.checkService).ToList();
            //    if (ucGridControlService != null)
            //    {
            //        ServiceProcessor.Reload(ucGridControlService, lstServiceKskADOs);
            //    }
            //    rowCount = (data == null ? 0 : lstServiceKskADOs.Count);
            //    dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
            //    WaitingManager.Hide();
            //}
            //catch (Exception ex)
            //{
            //    WaitingManager.Hide();
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void LoadDataToCombo()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceTypeFilter ServiceTypeFilter = new HisServiceTypeFilter();
                ServiceType = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERVICE_TYPE>>(
                             "api/HisServiceType/Get",
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    ServiceTypeFilter,
                    param);

                if (ServiceType != null && ServiceType.Count > 0)
                {
                    ServiceType = ServiceType.Where(o =>
                        o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU
                        && o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                        && o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                        ).ToList();
                }

                LoadDataToComboServiceType(cboServiceType, ServiceType);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboStatus()
        {
            try
            {
                List<Status> status = new List<Status>();
                status.Add(new Status(1, "Dịch vụ"));
                status.Add(new Status(2, "Nhóm khám sức khỏe"));

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

        private void LoadComboKSKContract()
        {
            try
            {
                List<V_HIS_KSK_CONTRACT> datas = BackendDataWorker.Get<V_HIS_KSK_CONTRACT>();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("KSK_CONTRACT_CODE", "Mã hợp đồng", 100, 0));
                columnInfos.Add(new ColumnInfo("WORK_PLACE_NAME", "Tên công ty", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("WORK_PLACE_NAME", "ID", columnInfos, true, 350);
                ControlEditorLoader.Load(cboKSKContract, datas, controlEditorADO);

                if (cboKSKContract.EditValue == null)
                    cboKSKContract.Properties.Buttons[1].Visible = false;
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

                cboServiceType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
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

        private void btnFind1_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid1(this);
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
                FillDataToGrid2(this);
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
                ServiceKskViews = null;
                ServiceKsks = null;
                isChoseKsk = 0;
                isChoseService = 0;
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
                btnSave.Focus();
                WaitingManager.Show();
                if (ucGridControlKsk != null && ucGridControlService != null)
                {
                    object Ksk = KskProcessor.GetDataGridView(ucGridControlKsk);
                    object Service = ServiceProcessor.GetDataGridView(ucGridControlService);
                    bool success = false;

                    List<string> thieuRoom = new List<string>();
                    List<string> thieuAmount = new List<string>();
                    List<string> saiSoLuong = new List<string>();
                    string message = "";

                    CommonParam param = new CommonParam();
                    if (isChoseService == 1)
                    {
                        if (Ksk is List<HIS.UC.Ksk.KskADO>)
                        {
                            lstKskADOs = (List<HIS.UC.Ksk.KskADO>)Ksk;

                            if (lstKskADOs != null && lstKskADOs.Count > 0)
                            {
                                //List<long> listServiceKsks = ServiceKsks.Select(p => p.SERVICE_ID).ToList();

                                var dataCheckeds = lstKskADOs.Where(p => p.check1 == true).ToList();

                                if (ServiceIdCheckByService != null)
                                {
                                    if (ServiceIdCheckByService.ROOM_ID <= 0)
                                        thieuRoom.Add(ServiceIdCheckByService.SERVICE_CODE);
                                    if (ServiceIdCheckByService.AMOUNT == null)
                                        thieuAmount.Add(ServiceIdCheckByService.SERVICE_CODE);
                                    if (ServiceIdCheckByService.AMOUNT <= 0)
                                        saiSoLuong.Add(ServiceIdCheckByService.SERVICE_CODE);
                                }

                                if (thieuRoom != null && thieuRoom.Count > 0)
                                {
                                    string serCode = string.Join(",", thieuRoom);
                                    message += "Dịch vụ với mã là: " + serCode + " có dữ liệu phòng trống. ";
                                }

                                if (saiSoLuong != null && saiSoLuong.Count > 0)
                                {
                                    string serCode = string.Join(",", saiSoLuong);
                                    message += "Dịch vụ với mã là: " + serCode + " có dữ liệu số lượng sai. ";
                                }

                                if (thieuAmount != null && thieuAmount.Count > 0)
                                {
                                    string serCode = string.Join(",", thieuAmount);
                                    message += "Dịch vụ với mã là: " + serCode + " có dữ liệu số lượng trống. ";
                                }

                                if (!string.IsNullOrEmpty(message) && dataCheckeds != null && dataCheckeds.Count > 0)
                                {
                                    WaitingManager.Hide();
                                    DevExpress.XtraEditors.XtraMessageBox.Show(message, "Thông báo");
                                    return;
                                }

                                var dataUpdate = new List<KskADO>();
                                //List xoa

                                var dataDeletes = ServiceKsks != null ? lstKskADOs.Where(o => ServiceKsks.Select(p => p.KSK_ID)
                                    .Contains(o.ID) && o.check1 == false).ToList() : null;


                                //list them
                                var dataCreates = dataCheckeds.Where(o => !ServiceKsks.Select(p => p.KSK_ID)
                                    .Contains(o.ID)).ToList();

                                if (dataCreates != null && dataCreates.Count == 0 && dataCheckeds != null && dataCheckeds.Count > 0)
                                {
                                    dataUpdate = dataCheckeds;
                                }
                                else if (dataCheckeds != null && dataCheckeds.Count > 0 && dataCreates != null && dataCreates.Count > 0)
                                    dataUpdate = dataCheckeds.Where(o => !dataCreates.Select(p => p.ID).Contains(o.ID)).ToList();

                                if (dataDeletes != null && dataDeletes.Count == 0 && dataCreates != null && dataCreates.Count == 0)
                                {
                                    success = true;
                                }

                                if (dataDeletes != null && dataDeletes.Count > 0)
                                {
                                    List<long> deleteSds = ServiceKsks.Where(o => dataDeletes.Select(p => p.ID)
                                        .Contains(o.KSK_ID)).Select(o => o.ID).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "api/HisKskService/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                              deleteSds,
                                              param);
                                    if (deleteResult)
                                        success = true;
                                    ServiceKsks = ServiceKsks.Where(o => !deleteSds.Contains(o.ID)).ToList();
                                }

                                if (dataCreates != null && dataCreates.Count > 0)
                                {
                                    List<HIS_KSK_SERVICE> ServiceKskCreates = new List<HIS_KSK_SERVICE>();
                                    foreach (var item in dataCreates)
                                    {
                                        HIS_KSK_SERVICE ServiceKsk = new HIS_KSK_SERVICE();
                                        ServiceKsk.SERVICE_ID = ServiceIdCheckByService.ID;
                                        //edward
                                        ServiceKsk.ROOM_ID = ServiceIdCheckByService.ROOM_ID;

                                        ServiceKsk.AMOUNT = ServiceIdCheckByService.AMOUNT;
                                        if (ServiceIdCheckByService.PRICE != null)
                                            ServiceKsk.PRICE = ServiceIdCheckByService.PRICE;
                                        if (ServiceIdCheckByService.VAT_RATIO != null)
                                            ServiceKsk.VAT_RATIO = ServiceIdCheckByService.VAT_RATIO / 100;
                                        ServiceKsk.KSK_ID = item.ID;
                                        ServiceKskCreates.Add(ServiceKsk);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<HIS_KSK_SERVICE>>(
                                               "api/HisKskService/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               ServiceKskCreates,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                    {
                                        success = true;
                                        ServiceKsks.AddRange(createResult);
                                        //btn_Radio_Enable_Click2(ServiceIdCheckByService);
                                    }
                                }

                                if (dataUpdate != null && dataUpdate.Count > 0)
                                {
                                    List<HIS_KSK_SERVICE> ServiceKskUpdates = new List<HIS_KSK_SERVICE>();
                                    foreach (var item in dataUpdate)
                                    {
                                        HIS_KSK_SERVICE ServiceKsk = new HIS_KSK_SERVICE();
                                        ServiceKsk.SERVICE_ID = ServiceIdCheckByService.ID;

                                        ServiceKsk.ROOM_ID = ServiceIdCheckByService.ROOM_ID;

                                        ServiceKsk.AMOUNT = ServiceIdCheckByService.AMOUNT;
                                        if (ServiceIdCheckByService.PRICE != null)
                                            ServiceKsk.PRICE = ServiceIdCheckByService.PRICE;
                                        if (ServiceIdCheckByService.VAT_RATIO != null)
                                            ServiceKsk.VAT_RATIO = ServiceIdCheckByService.VAT_RATIO / 100;

                                        var updateData = ServiceKsks.FirstOrDefault(o => o.SERVICE_ID == ServiceIdCheckByService.ID && o.KSK_ID == item.ID);
                                        if (updateData != null)
                                            ServiceKsk.ID = updateData.ID;
                                        ServiceKsk.KSK_ID = item.ID;
                                        ServiceKskUpdates.Add(ServiceKsk);
                                    }

                                    var updateResult = new BackendAdapter(param).Post<List<HIS_KSK_SERVICE>>(
                                               "api/HisKskService/UpdateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               ServiceKskUpdates,
                                               param);
                                    if (updateResult != null && updateResult.Count > 0)
                                    {
                                        success = true;
                                        //btn_Radio_Enable_Click2(ServiceIdCheckByService);
                                    }
                                }

                                btn_Radio_Enable_Click2(ServiceIdCheckByService);

                                lstKskADOs = lstKskADOs.OrderByDescending(p => p.check1).ToList();
                                if (ucGridControlKsk != null)
                                {
                                    KskProcessor.Reload(ucGridControlKsk, lstKskADOs);
                                }
                            }
                        }
                    }

                    if (isChoseKsk == 2)
                    {
                        if (Service is List<HIS.UC.Service.ServiceADO>)
                        {
                            lstServiceKskADOs = (List<HIS.UC.Service.ServiceADO>)Service;

                            if (lstServiceKskADOs != null && lstServiceKskADOs.Count > 0)
                            {
                                //List<long> listServiceKsks = ServiceKsk.Select(p => p.KSK_ID).ToList();

                                var dataChecked = lstServiceKskADOs.Where(p => p.checkService == true).ToList();

                                if (dataChecked != null && dataChecked.Count > 0)
                                {
                                    var checkThieuRoom = dataChecked.Where(o => o.ROOM_ID <= 0).ToList();
                                    if (checkThieuRoom != null && checkThieuRoom.Count > 0)
                                    {
                                        thieuRoom = checkThieuRoom.Select(o => o.SERVICE_CODE).ToList();
                                    }

                                    var checkThieuAmount = dataChecked.Where(o => o.AMOUNT == null).ToList();
                                    if (checkThieuAmount != null && checkThieuAmount.Count > 0)
                                    {
                                        thieuAmount = checkThieuAmount.Select(o => o.SERVICE_CODE).ToList();
                                    }

                                    var checkSaiAmount = dataChecked.Where(o => o.AMOUNT <= 0).ToList();
                                    if (checkSaiAmount != null && checkSaiAmount.Count > 0)
                                    {
                                        saiSoLuong = checkSaiAmount.Select(o => o.SERVICE_CODE).ToList();
                                    }
                                }

                                if (thieuRoom != null && thieuRoom.Count > 0)
                                {
                                    string serCode = string.Join(",", thieuRoom);
                                    message += "Dịch vụ với mã là: " + serCode + " có dữ liệu phòng trống. ";
                                }

                                if (saiSoLuong != null && saiSoLuong.Count > 0)
                                {
                                    string serCode = string.Join(",", saiSoLuong);
                                    message += "Dịch vụ với mã là: " + serCode + " có dữ liệu số lượng sai. ";
                                }

                                if (thieuAmount != null && thieuAmount.Count > 0)
                                {
                                    string serCode = string.Join(",", thieuAmount);
                                    message += "Dịch vụ với mã là: " + serCode + " có dữ liệu số lượng trống. ";
                                }

                                if (!string.IsNullOrEmpty(message))
                                {
                                    WaitingManager.Hide();
                                    DevExpress.XtraEditors.XtraMessageBox.Show(message, "Thông báo");
                                    return;
                                }

                                //List xoa
                                var dataUpdate = new List<ServiceADO>();

                                var dataDelete = lstServiceKskADOs.Where(o => ServiceKskViews.Select(p => p.SERVICE_ID)
                                    .Contains(o.ID) && o.checkService == false).ToList();

                                //list them
                                var dataCreate = dataChecked.Where(o => !ServiceKskViews.Select(p => p.SERVICE_ID)
                                    .Contains(o.ID)).ToList();

                                if (dataCreate != null && dataCreate.Count == 0 && dataChecked != null && dataChecked.Count > 0)
                                {
                                    dataUpdate = dataChecked;
                                }
                                else if (dataChecked != null && dataChecked.Count > 0 && dataCreate != null && dataCreate.Count > 0)
                                    dataUpdate = dataChecked.Where(o => !dataCreate.Select(p => p.ID).Contains(o.ID)).ToList();

                                if (dataDelete != null && dataDelete.Count > 0)
                                {

                                    List<long> deleteId = ServiceKskViews.Where(o => dataDelete.Select(p => p.ID)
                                        .Contains(o.SERVICE_ID)).Select(o => o.ID).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "api/HisKskService/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                              deleteId,
                                              param);
                                    if (deleteResult)
                                        success = true;
                                    ServiceKskViews = ServiceKskViews.Where(o => !deleteId.Contains(o.ID)).ToList();
                                }

                                if (dataCreate != null && dataCreate.Count > 0)
                                {
                                    List<HIS_KSK_SERVICE> ServiceKskCreate = new List<HIS_KSK_SERVICE>();
                                    foreach (var item in dataCreate)
                                    {
                                        HIS_KSK_SERVICE ServiceKskID = new HIS_KSK_SERVICE();
                                        ServiceKskID.KSK_ID = KskIdCheckByKsk.ID;
                                        ServiceKskID.SERVICE_ID = item.ID;
                                        ServiceKskID.AMOUNT = item.AMOUNT;

                                        ServiceKskID.ROOM_ID = item.ROOM_ID;

                                        if (item.PRICE != null)
                                            ServiceKskID.PRICE = item.PRICE;
                                        if (item.VAT_RATIO != null)
                                            ServiceKskID.VAT_RATIO = item.VAT_RATIO / 100;
                                        ServiceKskCreate.Add(ServiceKskID);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<HIS_KSK_SERVICE>>(
                                               "/api/HisKskService/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               ServiceKskCreate,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                    {
                                        success = true;
                                        ServiceKskViews.AddRange(createResult);
                                    }
                                }

                                if (dataUpdate != null && dataUpdate.Count > 0)
                                {
                                    List<HIS_KSK_SERVICE> ServiceKskUpdate = new List<HIS_KSK_SERVICE>();
                                    foreach (var item in dataUpdate)
                                    {
                                        HIS_KSK_SERVICE ServiceKskID = new HIS_KSK_SERVICE();
                                        ServiceKskID.KSK_ID = KskIdCheckByKsk.ID;
                                        ServiceKskID.SERVICE_ID = item.ID;
                                        ServiceKskID.AMOUNT = item.AMOUNT;

                                        ServiceKskID.ROOM_ID = item.ROOM_ID;

                                        if (item.PRICE != null)
                                            ServiceKskID.PRICE = item.PRICE;
                                        if (item.VAT_RATIO != null)
                                            ServiceKskID.VAT_RATIO = item.VAT_RATIO / 100;
                                        var updateData = ServiceKskViews.FirstOrDefault(o => o.SERVICE_ID == item.ID && o.KSK_ID == KskIdCheckByKsk.ID);
                                        if (updateData != null)
                                            ServiceKskID.ID = updateData.ID;
                                        ServiceKskUpdate.Add(ServiceKskID);
                                    }

                                    var updateResult = new BackendAdapter(param).Post<List<HIS_KSK_SERVICE>>(
                                               "/api/HisKskService/UpdateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               ServiceKskUpdate,
                                               param);
                                    if (updateResult != null && updateResult.Count > 0)
                                    {
                                        success = true;
                                    }
                                }

                                btn_Radio_Enable_Click(KskIdCheckByKsk);
                                lstServiceKskADOs = lstServiceKskADOs.OrderByDescending(p => p.checkService).ToList();
                                if (ucGridControlKsk != null)
                                {
                                    ServiceProcessor.Reload(ucGridControlService, lstServiceKskADOs);
                                    ProcessTotalPrice(lstServiceKskADOs);
                                    ProcessTotalPriceKSK(lstServiceKskADOs);
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
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid1(this);

                }
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
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid2(this);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FindShortcut1()
        {
            try
            {
                btnFind1_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FindShortcut2()
        {
            try
            {
                btnFind2_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SaveShortcut()
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

        private void txtKeyword1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboServiceType.Focus();
                    cboServiceType.ShowPopup();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
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
                    cboServiceType.Properties.Buttons[1].Visible = false;
                    cboServiceType.EditValue = null;
                }

                HisServiceTypeFilter filter = new HisServiceTypeFilter();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboChoose_EditValueChanged(object sender, EventArgs e)
        {
            try
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
                if (Inventec.Common.TypeConvert.Parse.ToInt64(cboChoose.EditValue.ToString()) == 1)
                {
                    layoutControlItem11.Text = "Tổng số nhóm KSK:";
                    lblTotal.Text = "";
                    layoutControlItem11.OptionsToolTip.ToolTip = "Tổng số nhóm khám sức khỏe";
                }
                else if (Inventec.Common.TypeConvert.Parse.ToInt64(cboChoose.EditValue.ToString()) == 2)
                {
                    lblTotal.Text = "";
                    layoutControlItem11.Text = "Tổng số dịch vụ:";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnSaveKsk_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Hide();
                bool success = false;
                positionHandle = -1;
                CommonParam param = new CommonParam();
                if (!dxValidationProvider1.Validate())
                    return;
                HIS_KSK ksk = new HIS_KSK();

                if (this.clickKsk != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_KSK>(ksk, this.clickKsk);
                }

                ksk.KSK_CODE = txtCode.Text;
                ksk.KSK_NAME = txtName.Text;
                if (cboKSKContract.EditValue != null)
                    ksk.KSK_CONTRACT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboKSKContract.EditValue.ToString());

                WaitingManager.Show();
                if (this.clickKsk == null)
                {
                    var createApi = new BackendAdapter(param).Post<HIS_KSK>("api/HisKsk/Create", ApiConsumer.ApiConsumers.MosConsumer, ksk, param);
                    if (createApi != null)
                    {
                        success = true;
                    }
                }
                else
                {
                    var updateApi = new BackendAdapter(param).Post<HIS_KSK>("api/HisKsk/Update", ApiConsumer.ApiConsumers.MosConsumer, ksk, param);
                    if (updateApi != null)
                    {
                        success = true;
                    }
                }

                if (success)
                {
                    FillDataToGrid2(this);
                    btnRefresh_Click(null, null);
                }

                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(txtCode);
                ValidationSingleControl(txtName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                txtCode.Text = "";
                txtName.Text = "";
                cboKSKContract.EditValue = null;
                this.clickKsk = null;
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                btnSaveKsk.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtName.Focus();
                    txtName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboKSKContract.Focus();
                    cboKSKContract.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public void SaveKsk()
        {
            try
            {
                if (btnSaveKsk.Enabled)
                    btnSaveKsk_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public void Refresh()
        {
            try
            {
                if (btnRefresh.Enabled)
                    btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                if (!btnImport.Enabled)
                    return;

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisImpKskService").FirstOrDefault();

                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.HisImpKskService'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {

                    moduleData.RoomId = this.moduleData.RoomId;
                    moduleData.RoomTypeId = this.moduleData.RoomTypeId;
                    List<object> listArgs = new List<object>();
                    listArgs.Add(moduleData);
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("extenceInstance is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (NullReferenceException ex)
            {
                //WaitingManager.Hide();
                //MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                //WaitingManager.Hide();
                //MessageBox.Show(MessageUtil.GetMessage(L.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkAutoSelectPrice_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkAutoSelectPrice.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkAutoSelectPrice.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkAutoSelectPrice.Name;
                    csAddOrUpdate.VALUE = (chkAutoSelectPrice.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKSKContract_ButtonClick(object sender, ButtonPressedEventArgs e)
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

        private void cboKSKContract_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboKSKContract.EditValue == null)
                    cboKSKContract.Properties.Buttons[1].Visible = false;
                else
                    cboKSKContract.Properties.Buttons[1].Visible = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}







