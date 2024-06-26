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
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.ADO;
using AutoMapper;
using ACS.Filter;
using HIS.Desktop.LocalStorage.BackendData;
using ACS.EFMODEL.DataModels;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.UC.PatientType;
using HIS.UC.PatientType.ADO;
using HIS.UC.MediStock;
using HIS.UC.MediStock.ADO;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using HIS.Desktop.Common;
using DevExpress.XtraBars;
using MOS.SDO;
using HIS.Desktop.Plugins.MestPatientType.Entity;

namespace HIS.Desktop.Plugins.MestPatientType
{
    public partial class UCPatientTypeMediStock : HIS.Desktop.Utility.UserControlBase
    {

        internal Inventec.Desktop.Common.Modules.Module currentModule;
        UCMediStockProcessor MediStockProcessor;
        UCPatientTypeProcessor PatientTypeProcessor;
        UserControl ucGridControlPatientType;
        UserControl ucGridControlMediStock;
        int rowCount = 0;
        int dataTotal = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        internal List<HIS.UC.MediStock.MediStockADO> lstMediStockADOs { get; set; }
        internal List<HIS.UC.PatientType.PatientTypeADO> lstPatientTypeADOs { get; set; }
        List<V_HIS_MEDI_STOCK> listMediStock;
        List<HIS_PATIENT_TYPE> listPatientType;
        long PatientTypeIdCheckByPatientType = 0;
        long isChosePatientType;
        long isChoseMediStock;
        long MediStockIdCheckByMediStock;
        bool isCheckAll;
        List<HIS_MEST_PATIENT_TYPE> PatientTypeMediStocks { get; set; }
        List<HIS_MEST_PATIENT_TYPE> PatientTypeMediStockViews { get; set; }
        V_HIS_MEDI_STOCK currentMediStock;
        HIS.UC.PatientType.PatientTypeADO currentCopyPatientTypeAdo;
        HIS.UC.MediStock.MediStockADO CurrentMediStockCopyAdo;

        public UCPatientTypeMediStock()
        {
            InitializeComponent();

        }

        public UCPatientTypeMediStock(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
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

        public UCPatientTypeMediStock(V_HIS_MEDI_STOCK PatientTypeData, Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
            try
            {
                this.currentMediStock = PatientTypeData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void UCMediStockService_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                WaitingManager.Show();
                LoadDataToCombo();
                LoadComboStatus();
                InitUcgridViewPatientType();
                InitUcgridViewMediStock();
                if (this.currentMediStock == null)
                {
                    FillDataToGrid1__PatientType(this);
                    FillDataToGrid2__MediStock(this);
                }
                else
                {
                    FillDataToGrid1__MediStock_Default(this);
                    FillDataToGrid1__PatientType(this);
                    var room = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == currentMediStock.ID);
                    if (room != null)
                    {
                        btn_Radio_Enable_Click(room);
                        //cboMediStockType.EditValue = room.MEDI_STOCK_TYPE_ID;
                    }
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPatientType_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChosePatientType == 1)
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
                        if (hi.Column.FieldName == "checkMedi")
                        {
                            var lstCheckAll = lstPatientTypeADOs;
                            List<HIS.UC.PatientType.PatientTypeADO> lstChecks = new List<HIS.UC.PatientType.PatientTypeADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var ServiceCheckedNum = lstPatientTypeADOs.Where(o => o.checkMedi == true).Count();
                                var ServiceNum = lstPatientTypeADOs.Count();
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
                                            item.checkMedi = true;
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
                                            item.checkMedi = false;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = true;
                                }

                                PatientTypeProcessor.Reload(ucGridControlPatientType, lstChecks);


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

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.MestPatientType.Resources.Lang", typeof(HIS.Desktop.Plugins.MestPatientType.UCPatientTypeMediStock).Assembly);

                //////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                //this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCPatientTypeMediStock.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.cboChoose.Properties.NullText = Inventec.Common.Resource.Get.Value("UCPatientTypeMediStock.cboChoose.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                ////this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCPatientTypeMediStock.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCPatientTypeMediStock.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.btnFind2.Text = Inventec.Common.Resource.Get.Value("UCPatientTypeMediStock.btnFind2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.btnSave.Text = Inventec.Common.Resource.Get.Value("UCPatientTypeMediStock.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.btnFind1.Text = Inventec.Common.Resource.Get.Value("UCPatientTypeMediStock.btnFind1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.cboMediStockType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCPatientTypeMediStock.cboServiceType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.txtKeyword2.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCPatientTypeMediStock.txtKeyword2.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.txtKeyword1.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCPatientTypeMediStock.txtKeyword1.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciMediStockType.Text = Inventec.Common.Resource.Get.Value("UCPatientTypeMediStock.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("UCPatientTypeMediStock.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.bar2.Text = Inventec.Common.Resource.Get.Value("UCPatientTypeMediStock.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitUcgridViewPatientType()
        {
            try
            {
                PatientTypeProcessor = new UCPatientTypeProcessor();
                PatientTypeInitADO ado = new PatientTypeInitADO();
                ado.ListPatientTypeColumn = new List<UC.PatientType.PatientTypeColumn>();
                ado.gridViewPatientType_MouseDownMedi = gridViewPatientType_MouseDown;
                ado.btn_Radio_Enable_Click_Medi = btn_Radio_Enable_Click1;
                ado.gridView_MouseRightClick = PatientTypeGridView_MouseRightClick;

                PatientTypeColumn colRadio2 = new PatientTypeColumn("   ", "radioMedi", 30, true);
                colRadio2.VisibleIndex = 0;
                colRadio2.Visible = false;
                colRadio2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListPatientTypeColumn.Add(colRadio2);

                PatientTypeColumn colCheck2 = new PatientTypeColumn("   ", "checkMedi", 30, true);
                colCheck2.VisibleIndex = 1;
                colCheck2.image = imageCollectionService.Images[0];
                colCheck2.Visible = false;
                colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListPatientTypeColumn.Add(colCheck2);

                PatientTypeColumn colMaDichvu = new PatientTypeColumn("Mã đối tượng", "PATIENT_TYPE_CODE", 60, false);
                colMaDichvu.VisibleIndex = 2;
                ado.ListPatientTypeColumn.Add(colMaDichvu);

                PatientTypeColumn colTenDichvu = new PatientTypeColumn("Tên đối tượng", "PATIENT_TYPE_NAME", 300, false);
                colTenDichvu.VisibleIndex = 3;
                ado.ListPatientTypeColumn.Add(colTenDichvu);

                //PatientTypeColumn colMaLoaidichvu = new PatientTypeColumn("Đơn vị tính", "SERVICE_UNIT_NAME", 80, false);
                //colMaLoaidichvu.VisibleIndex = 4;
                //ado.ListPatientTypeColumn.Add(colMaLoaidichvu);

                this.ucGridControlPatientType = (UserControl)PatientTypeProcessor.Run(ado);
                if (ucGridControlPatientType != null)
                {
                    this.panelControl1.Controls.Add(this.ucGridControlPatientType);
                    this.ucGridControlPatientType.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediStock_MouseMediStock(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseMediStock == 2)
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
                        if (hi.Column.FieldName == "checkMest")
                        {
                            var lstCheckAll = lstMediStockADOs;
                            List<HIS.UC.MediStock.MediStockADO> lstChecks = new List<HIS.UC.MediStock.MediStockADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var MediStockCheckedNum = lstMediStockADOs.Where(o => o.checkMest == true).Count();
                                var MediStocktmNum = lstMediStockADOs.Count();
                                if ((MediStockCheckedNum > 0 && MediStockCheckedNum < MediStocktmNum) || MediStockCheckedNum == 0)
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = imageCollectionMediStock.Images[1];
                                }

                                if (MediStockCheckedNum == MediStocktmNum)
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = imageCollectionMediStock.Images[0];
                                }

                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.checkMest = true;
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
                                            item.checkMest = false;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = true;
                                }

                                MediStockProcessor.Reload(ucGridControlMediStock, lstChecks);


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

        private void btn_Radio_Enable_Click1(HIS_PATIENT_TYPE data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisMestPatientTypeFilter filter = new HisMestPatientTypeFilter();
                filter.PATIENT_TYPE_ID = data.ID;
                //if (this.cboMediStockType.EditValue != null&&(long)this.cboMediStockType.EditValue!=0)
                //{ 
                //filter.
                //}
                PatientTypeIdCheckByPatientType = data.ID;

                PatientTypeMediStocks = new BackendAdapter(param).Get<List<HIS_MEST_PATIENT_TYPE>>(
                                    "api/HisMestPatientType/Get",
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<HIS.UC.MediStock.MediStockADO> dataNew = new List<HIS.UC.MediStock.MediStockADO>();
                dataNew = (from r in listMediStock select new MediStockADO(r)).ToList();
                if (PatientTypeMediStocks != null && PatientTypeMediStocks.Count > 0)
                {
                    foreach (var itemMediStock in PatientTypeMediStocks)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == itemMediStock.MEDI_STOCK_ID);
                        if (check != null)
                        {
                            check.checkMest = true;
                        }
                    }
                }
                dataNew = dataNew.OrderByDescending(p => p.checkMest).ToList();
                if (ucGridControlMediStock != null)
                {
                    MediStockProcessor.Reload(ucGridControlMediStock, dataNew);
                }
                else
                {
                    FillDataToGrid2__MediStock(this);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcgridViewMediStock()
        {
            try
            {
                MediStockProcessor = new UCMediStockProcessor();
                MediStockInitADO ado = new MediStockInitADO();
                ado.ListMediStockColumn = new List<UC.MediStock.MediStockColumn>();
                ado.gridViewMediStock_MouseDownMest = gridViewMediStock_MouseMediStock;
                ado.btn_Radio_Enable_Click1 = btn_Radio_Enable_Click;
                ado.GridView_MouseRightClick = MediStockGridView_MouseRightClick;

                MediStockColumn colRadio1 = new MediStockColumn("   ", "radioMest", 30, true);
                colRadio1.VisibleIndex = 0;
                colRadio1.Visible = false;
                colRadio1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMediStockColumn.Add(colRadio1);

                MediStockColumn colCheck1 = new MediStockColumn("   ", "checkMest", 30, true);
                colCheck1.VisibleIndex = 1;
                colCheck1.image = imageCollectionMediStock.Images[0];
                colCheck1.Visible = false;
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMediStockColumn.Add(colCheck1);

                MediStockColumn colMaPhong = new MediStockColumn("Mã Kho", "MEDI_STOCK_CODE", 60, false);
                colMaPhong.VisibleIndex = 2;
                ado.ListMediStockColumn.Add(colMaPhong);

                MediStockColumn colTenPhong = new MediStockColumn("Tên Kho", "MEDI_STOCK_NAME", 100, false);
                colTenPhong.VisibleIndex = 3;
                ado.ListMediStockColumn.Add(colTenPhong);


                this.ucGridControlMediStock = (UserControl)MediStockProcessor.Run(ado);
                if (ucGridControlMediStock != null)
                {
                    this.panelControl2.Controls.Add(this.ucGridControlMediStock);
                    this.ucGridControlMediStock.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btn_Radio_Enable_Click(V_HIS_MEDI_STOCK data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisMestPatientTypeFilter filter = new HisMestPatientTypeFilter();
                filter.MEDI_STOCK_ID = data.ID;
                MediStockIdCheckByMediStock = data.ID;
                PatientTypeMediStockViews = new BackendAdapter(param).Get<List<HIS_MEST_PATIENT_TYPE>>(
                                         "api/HisMestPatientType/Get",

                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<HIS.UC.PatientType.PatientTypeADO> dataNew = new List<HIS.UC.PatientType.PatientTypeADO>();
                dataNew = (from r in listPatientType select new HIS.UC.PatientType.PatientTypeADO(r)).ToList();
                if (PatientTypeMediStockViews != null && PatientTypeMediStockViews.Count > 0)
                {

                    foreach (var itemService in PatientTypeMediStockViews)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == itemService.PATIENT_TYPE_ID);
                        if (check != null)
                        {
                            check.checkMedi = true;
                        }
                    }

                    dataNew = dataNew.OrderByDescending(p => p.checkMedi).ToList();

                    if (ucGridControlPatientType != null)
                    {
                        PatientTypeProcessor.Reload(ucGridControlPatientType, dataNew);
                    }
                }
                else
                {
                    FillDataToGrid1__PatientType(this);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid2__MediStock(UCPatientTypeMediStock uCMediStockService)
        {
            try
            {
                int numPageSize;
                if (ucPaging2.pagingGrid != null)
                {
                    numPageSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                FillDataToGridMediStock(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPaging2.Init(FillDataToGridMediStock, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridMediStock(object data)
        {
            try
            {
                WaitingManager.Show();
                listMediStock = new List<V_HIS_MEDI_STOCK>();
                int start1 = ((CommonParam)data).Start ?? 0;
                int limit1 = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start1, limit1);
                MOS.Filter.HisMediStockViewFilter MediStockFillter = new HisMediStockViewFilter();
                MediStockFillter.IS_ACTIVE = 1;
                MediStockFillter.ORDER_FIELD = "MODIFY_TIME";
                MediStockFillter.ORDER_DIRECTION = "DESC";
                MediStockFillter.KEY_WORD = txtKeyword2.Text;
                if ((long)cboChoose.EditValue == 2)
                {
                    isChoseMediStock = (long)cboChoose.EditValue;
                }
                //if (cboMediStockType.EditValue != null)
                //    MediStockFillter.MEDI_STOCK_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboMediStockType.EditValue ?? "0").ToString());
                var sar = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>>(
                   HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_MEDI_STOCK_GETVIEW,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                      MediStockFillter,
                    param);
                List<long> listRadio = (lstMediStockADOs ?? new List<MediStockADO>()).Where(o => o.radioMest == true).Select(p => p.ID).ToList();
                lstMediStockADOs = new List<MediStockADO>();
                if (sar != null && sar.Data.Count > 0)
                {
                    listMediStock = sar.Data;
                    foreach (var item in listMediStock)
                    {
                        MediStockADO roomaccountADO = new MediStockADO(item);
                        if (isChoseMediStock == 2)
                        {
                            roomaccountADO.isKeyChooseMest = true;
                        }
                        lstMediStockADOs.Add(roomaccountADO);
                    }
                }

                if (PatientTypeMediStocks != null && PatientTypeMediStocks.Count > 0)
                {
                    foreach (var itemUsername in PatientTypeMediStocks)
                    {
                        var check = lstMediStockADOs.FirstOrDefault(o => o.ID == itemUsername.MEDI_STOCK_ID);
                        if (check != null)
                        {
                            check.checkMest = true;
                        }
                    }
                }
                if (listRadio != null && listRadio.Count > 0)
                {
                    foreach (var rd in listRadio)
                    {
                        var radio = lstMediStockADOs.FirstOrDefault(o => o.ID == rd);
                        if (radio != null)
                        {
                            radio.radioMest = radio.isKeyChooseMest;
                        }
                    }
                }
                lstMediStockADOs = lstMediStockADOs.OrderByDescending(p => p.checkMest).ThenByDescending(p => p.radioMest).Distinct().ToList();

                if (ucGridControlMediStock != null)
                {
                    MediStockProcessor.Reload(ucGridControlMediStock, lstMediStockADOs);
                }
                rowCount1 = (data == null ? 0 : lstMediStockADOs.Count);
                dataTotal1 = (sar.Param == null ? 0 : sar.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid1__PatientType(UCPatientTypeMediStock UCMediStockService)
        {
            try
            {
                int numPageSize;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                FillDataToGridPatientType(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridPatientType, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid1__MediStock_Default(UCPatientTypeMediStock UCMediStockService)
        {
            try
            {
                int numPageSize;
                if (ucPaging2.pagingGrid != null)
                {
                    numPageSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                FillDataToGridMediStock_Default(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging2.Init(FillDataToGridMediStock_Default, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridPatientType(object data)
        {
            try
            {
                WaitingManager.Show();
                listPatientType = new List<HIS_PATIENT_TYPE>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisPatientTypeFilter ServiceFillter = new HisPatientTypeFilter();
                ServiceFillter.IS_ACTIVE = 1;
                ServiceFillter.ORDER_FIELD = "MODIFY_TIME";
                ServiceFillter.ORDER_DIRECTION = "DESC";
                ServiceFillter.KEY_WORD = txtKeyword1.Text;

                if ((long)cboChoose.EditValue == 1)
                {
                    isChosePatientType = (long)cboChoose.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>>(
                                                     "api/HisPatientType/Get",
                     HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                     ServiceFillter,
                     param);
                List<long> listRadio = (lstPatientTypeADOs ?? new List<PatientTypeADO>()).Where(o => o.radioMedi == true).Select(p => p.ID).ToList();
                lstPatientTypeADOs = new List<PatientTypeADO>();

                if (rs != null && rs.Data.Count > 0)
                {

                    listPatientType = rs.Data;
                    foreach (var item in listPatientType)
                    {
                        PatientTypeADO MediStockServiceADO = new PatientTypeADO(item);
                        if (isChosePatientType == 1)
                        {
                            MediStockServiceADO.isKeyChooseMedi = true;
                        }
                        lstPatientTypeADOs.Add(MediStockServiceADO);
                    }
                }

                if (PatientTypeMediStockViews != null && PatientTypeMediStockViews.Count > 0)
                {
                    foreach (var itemUsername in PatientTypeMediStockViews)
                    {
                        var check = lstPatientTypeADOs.FirstOrDefault(o => o.ID == itemUsername.PATIENT_TYPE_ID);
                        if (check != null)
                        {
                            check.checkMedi = true;
                        }
                    }
                }

                if (listRadio != null && listRadio.Count > 0)
                {
                    foreach (var rd in listRadio)
                    {
                        var check = lstPatientTypeADOs.FirstOrDefault(o => o.ID == rd);
                        if (check != null)
                        {
                            check.radioMedi = check.isKeyChooseMedi;
                        }
                    }
                }

                lstPatientTypeADOs = lstPatientTypeADOs.OrderByDescending(p => p.checkMedi).ThenByDescending(p => p.radioMedi).Distinct().ToList();
                if (ucGridControlPatientType != null)
                {
                    PatientTypeProcessor.Reload(ucGridControlPatientType, lstPatientTypeADOs);
                }
                rowCount = (data == null ? 0 : lstPatientTypeADOs.Count);
                dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridMediStock_Default(object data)
        {
            try
            {
                WaitingManager.Show();
                listMediStock = new List<V_HIS_MEDI_STOCK>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisMediStockViewFilter hisMediStockViewFilter = new HisMediStockViewFilter();
                hisMediStockViewFilter.IS_ACTIVE = 1;
                hisMediStockViewFilter.ID = this.currentMediStock.ID;

                //if (cboServiceType.EditValue != null)

                //    ServiceFillter.SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceType.EditValue ?? "0").ToString());

                if ((long)cboChoose.EditValue == 2)
                {
                    isChoseMediStock = (long)cboChoose.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>>(
                                                     "api/HisMediStock/GetView",
                     HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                     hisMediStockViewFilter,
                     param);

                this.lstMediStockADOs = new List<MediStockADO>();

                if (rs != null && rs.Data.Count > 0)
                {
                    this.listMediStock = new List<V_HIS_MEDI_STOCK>();
                    this.listMediStock = rs.Data;
                    foreach (var item in this.listMediStock)
                    {
                        MediStockADO MediStockServiceADO = new MediStockADO(item);
                        if (isChoseMediStock == 2)
                        {
                            MediStockServiceADO.isKeyChooseMest = true;
                            MediStockServiceADO.radioMest = true;
                        }
                        this.lstMediStockADOs.Add(MediStockServiceADO);
                    }
                }

                if (PatientTypeMediStockViews != null && PatientTypeMediStockViews.Count > 0)
                {
                    foreach (var itemUsername in PatientTypeMediStockViews)
                    {
                        var check = this.lstMediStockADOs.FirstOrDefault(o => o.ID == itemUsername.MEDI_STOCK_ID);
                        if (check != null)
                        {
                            check.checkMest = true;
                        }
                    }
                }

                this.lstMediStockADOs = this.lstMediStockADOs.OrderByDescending(p => p.checkMest).ToList();
                if (ucGridControlMediStock != null)
                {
                    MediStockProcessor.Reload(ucGridControlMediStock, this.lstMediStockADOs);
                }
                rowCount = (data == null ? 0 : lstMediStockADOs.Count);
                dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCombo()
        {
            try
            {
                //MediStockType = BackendDataWorker.Get<HIS_MEDI_STOCK_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList();
                //LoadDataToComboServiceType(cboMediStockType, MediStockType);
                //cboMediStockType.EditValue = IMSys.DbConfig.HIS_RS.HIS_MEDI_STOCK_TYPE.ID__XL;
                //cboMediStockType.Enabled = false;

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
                status.Add(new Status(1, "Đối tượng"));
                status.Add(new Status(2, "Kho"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboChoose, status, controlEditorADO);
                cboChoose.EditValue = status[1].id;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void LoadDataToComboServiceType(DevExpress.XtraEditors.GridLookUpEdit cboServiceType, List<HIS_MEDI_STOCK_TYPE> ServiceType)
        //{
        //    try
        //    {
        //        cboServiceType.Properties.DataSource = ServiceType;
        //        cboServiceType.Properties.DisplayMember = "MEDI_STOCK_TYPE_NAME";
        //        cboServiceType.Properties.ValueMember = "ID";

        //        cboServiceType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
        //        cboServiceType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
        //        cboServiceType.Properties.ImmediatePopup = true;
        //        cboServiceType.ForceInitialize();
        //        cboServiceType.Properties.View.Columns.Clear();

        //        GridColumn aColumnCode = cboServiceType.Properties.View.Columns.AddField("MEDI_STOCK_TYPE_CODE");
        //        aColumnCode.Caption = "Mã";
        //        aColumnCode.Visible = true;
        //        aColumnCode.VisibleIndex = 1;
        //        aColumnCode.Width = 100;

        //        GridColumn aColumnName = cboServiceType.Properties.View.Columns.AddField("MEDI_STOCK_TYPE_NAME");
        //        aColumnName.Caption = "Tên";
        //        aColumnName.Visible = true;
        //        aColumnName.VisibleIndex = 2;
        //        aColumnName.Width = 200;
        //    }
        //    catch (Exception ex)
        //    {

        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void btnFind1_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FillDataToGrid1__PatientType(this);
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
                FillDataToGrid2__MediStock(this);
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
                PatientTypeMediStockViews = null;
                PatientTypeMediStocks = null;
                isChoseMediStock = 0;
                isChosePatientType = 0;
                FillDataToGrid1__PatientType(this);
                FillDataToGrid2__MediStock(this);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            bool success = false;
            CommonParam param = new CommonParam();
            try
            {

                if (ucGridControlMediStock != null && ucGridControlPatientType != null)
                {
                    object MediStock = MediStockProcessor.GetDataGridView(ucGridControlMediStock);
                    object Service = PatientTypeProcessor.GetDataGridView(ucGridControlPatientType);
                    if (isChosePatientType == 1)
                    {
                        if (this.lstPatientTypeADOs == null || !this.lstPatientTypeADOs.Exists(o => o.radioMedi))
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn đối tượng", "Thông báo");
                            return;
                        }
                        if (MediStock is List<HIS.UC.MediStock.MediStockADO>)
                        {
                            lstMediStockADOs = (List<HIS.UC.MediStock.MediStockADO>)MediStock;

                            if (lstMediStockADOs != null && lstMediStockADOs.Count > 0)
                            {
                                //List<long> listServiceMediStocks = ServiceMediStocks.Select(p => p.SERVICE_ID).ToList();

                                var dataCheckeds = lstMediStockADOs.Where(p => p.checkMest == true).ToList();

                                //       //List xoa

                                var dataDeletes = lstMediStockADOs.Where(o => PatientTypeMediStocks.Select(p => p.MEDI_STOCK_ID)
                                    .Contains(o.ID) && o.checkMest == false).ToList();


                                //list them
                                var dataCreates = dataCheckeds.Where(o => !PatientTypeMediStocks.Select(p => p.MEDI_STOCK_ID)
                                    .Contains(o.ID)).ToList();
                                if (dataDeletes.Count == 0 && dataCreates.Count == 0)
                                {
                                    if (PatientTypeMediStocks.Where(o => lstMediStockADOs.Exists(p => p.ID == o.MEDI_STOCK_ID)).ToList().Count == 0)
                                    {
                                        MessageBox.Show("Chưa chọn Kho");
                                        return;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Đối tượng đã thiết lập cho các Kho được chọn");
                                        return;
                                    }
                                }
                                if (dataCheckeds != null)
                                {
                                    success = true;
                                }
                                WaitingManager.Show();
                                if (dataDeletes != null && dataDeletes.Count > 0)
                                {
                                    List<long> deleteSds = PatientTypeMediStocks.Where(o => dataDeletes.Select(p => p.ID)
                                        .Contains(o.MEDI_STOCK_ID)).Select(o => o.ID).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "api/HisMestPatientType/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                              deleteSds,
                                              param);
                                    if (deleteResult)
                                        success = true;
                                    PatientTypeMediStocks = PatientTypeMediStocks.Where(o => !deleteSds.Contains(o.ID)).ToList();
                                }

                                if (dataCreates != null && dataCreates.Count > 0)
                                {
                                    List<HIS_MEST_PATIENT_TYPE> ServiceMediStockCreates = new List<HIS_MEST_PATIENT_TYPE>();
                                    foreach (var item in dataCreates)
                                    {
                                        HIS_MEST_PATIENT_TYPE ServiceMediStock = new HIS_MEST_PATIENT_TYPE();
                                        ServiceMediStock.PATIENT_TYPE_ID = PatientTypeIdCheckByPatientType;
                                        ServiceMediStock.MEDI_STOCK_ID = item.ID;
                                        ServiceMediStockCreates.Add(ServiceMediStock);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<HIS_MEST_PATIENT_TYPE>>(
                                               "api/HisMestPatientType/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               ServiceMediStockCreates,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                        success = true;
                                    AutoMapper.Mapper.CreateMap<HIS_MEST_PATIENT_TYPE, HIS_MEST_PATIENT_TYPE>();
                                    var vCreateResults = AutoMapper.Mapper.Map<List<HIS_MEST_PATIENT_TYPE>, List<HIS_MEST_PATIENT_TYPE>>(createResult);
                                    PatientTypeMediStocks.AddRange(vCreateResults);
                                }

                                WaitingManager.Hide();
                                lstMediStockADOs = lstMediStockADOs.OrderByDescending(p => p.checkMest).ToList();
                                if (ucGridControlMediStock != null)
                                {
                                    MediStockProcessor.Reload(ucGridControlMediStock, lstMediStockADOs);
                                }
                            }

                        }
                    }

                    if (isChoseMediStock == 2)
                    {
                        if (this.lstMediStockADOs == null || !this.lstMediStockADOs.Exists(o => o.radioMest))
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn Kho", "Thông báo");
                            return;
                        }
                        if (Service is List<HIS.UC.PatientType.PatientTypeADO>)
                        {
                            lstPatientTypeADOs = (List<HIS.UC.PatientType.PatientTypeADO>)Service;

                            if (lstPatientTypeADOs != null && lstPatientTypeADOs.Count > 0)
                            {
                                //List<long> listMediStockServices = ServiceMediStock.Select(p => p.MEDI_STOCK_ID).ToList();

                                var dataChecked = lstPatientTypeADOs.Where(p => p.checkMedi == true).ToList();
                                //List xoa

                                var dataDelete = lstPatientTypeADOs.Where(o => PatientTypeMediStockViews.Select(p => p.PATIENT_TYPE_ID)
                                    .Contains(o.ID) && o.checkMedi == false).ToList();

                                //list them
                                var dataCreate = dataChecked.Where(o => !PatientTypeMediStockViews.Select(p => p.PATIENT_TYPE_ID)
                                    .Contains(o.ID)).ToList();


                                if (dataDelete.Count == 0 && dataCreate.Count == 0)
                                {
                                    if (PatientTypeMediStockViews.Where(o => lstPatientTypeADOs.Exists(p => p.ID == o.PATIENT_TYPE_ID)).ToList().Count == 0)
                                    {
                                        MessageBox.Show("Chưa chọn đối tượng");
                                        return;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Kho đã thiết lập cho các đối tượng được chọn");
                                        return;
                                    }
                                }
                                if (dataChecked != null)
                                {
                                    success = true;
                                }
                                WaitingManager.Show();
                                if (dataDelete != null && dataDelete.Count > 0)
                                {

                                    List<long> deleteId = PatientTypeMediStockViews.Where(o => dataDelete.Select(p => p.ID)
                                        .Contains(o.PATIENT_TYPE_ID)).Select(o => o.ID).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "api/HisMestPatientType/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                              deleteId,
                                              param);
                                    if (deleteResult)
                                        success = true;
                                    PatientTypeMediStockViews = PatientTypeMediStockViews.Where(o => !deleteId.Contains(o.ID)).ToList();
                                }

                                if (dataCreate != null && dataCreate.Count > 0)
                                {
                                    List<HIS_MEST_PATIENT_TYPE> ServiceMediStockCreate = new List<HIS_MEST_PATIENT_TYPE>();
                                    foreach (var item in dataCreate)
                                    {
                                        HIS_MEST_PATIENT_TYPE ServiceMediStockID = new HIS_MEST_PATIENT_TYPE();
                                        ServiceMediStockID.MEDI_STOCK_ID = MediStockIdCheckByMediStock;
                                        ServiceMediStockID.PATIENT_TYPE_ID = item.ID;
                                        ServiceMediStockCreate.Add(ServiceMediStockID);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<HIS_MEST_PATIENT_TYPE>>(
                                               "/api/HisMestPatientType/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               ServiceMediStockCreate,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                        success = true;
                                    AutoMapper.Mapper.CreateMap<HIS_MEST_PATIENT_TYPE, HIS_MEST_PATIENT_TYPE>();
                                    var vCreateResults = AutoMapper.Mapper.Map<List<HIS_MEST_PATIENT_TYPE>, List<HIS_MEST_PATIENT_TYPE>>(createResult);
                                    PatientTypeMediStockViews.AddRange(vCreateResults);
                                }
                                WaitingManager.Hide();
                                lstPatientTypeADOs = lstPatientTypeADOs.OrderByDescending(p => p.checkMedi).ToList();
                                if (ucGridControlMediStock != null)
                                {
                                    PatientTypeProcessor.Reload(ucGridControlPatientType, lstPatientTypeADOs);
                                }
                            }
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            MessageManager.Show(this.ParentForm, param, success);
        }

        private void txtKeyword1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid1__PatientType(this);

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
                    FillDataToGrid2__MediStock(this);
                }
                WaitingManager.Hide();
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
                    //cboMediStockType.Focus();
                    //cboMediStockType.ShowPopup();
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

        private void cboMediStockType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            //try
            //{
            //    if (e.CloseMode == PopupCloseMode.Normal)
            //    {
            //        if (cboMediStockType.EditValue != null)
            //        {
            //            HIS_MEDI_STOCK_TYPE data = MediStockType.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMediStockType.EditValue.ToString()));
            //            if (data != null)
            //            {
            //                cboMediStockType.Properties.Buttons[1].Visible = true;
            //                btnFind1.Focus();
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        //private void cboServiceType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Button.Kind == ButtonPredefines.Delete)
        //        {
        //            //cboMediStockType.EditValue = null;
        //        }

        //        HisServiceTypeFilter filter = new HisServiceTypeFilter();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void cboChoose_EditValueChanged(object sender, EventArgs e)
        {
            ////try
            ////{
            ////    if (cboMediStockType.EditValue != null)
            ////    {
            ////        HIS_MEDI_STOCK_TYPE data = MediStockType.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMediStockType.EditValue.ToString()));
            ////        if (data != null)
            ////        {
            ////            cboMediStockType.Properties.Buttons[1].Visible = true;
            ////            btnFind1.Focus();
            ////        }
            ////    }



            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}

        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    WaitingManager.Show();
            //    List<object> listArgs = new List<object>();
            //    listArgs.Add((RefeshReference)RefreshData);
            //    if (this.currentModule != null)
            //    {
            //        CallModule callModule = new CallModule(CallModule.HisImportServiceMediStock, currentModule.MediStockId, currentModule.MediStockTypeId, listArgs);
            //    }
            //    else
            //    {
            //        CallModule callModule = new CallModule(CallModule.HisImportServiceMediStock, 0, 0, listArgs);
            //    }
            //    WaitingManager.Hide();
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        public void RefreshData()
        {
            try
            {
                if (this.currentMediStock == null)
                {
                    FillDataToGrid1__PatientType(this);
                    FillDataToGrid2__MediStock(this);
                }
                else
                {
                    FillDataToGrid1__MediStock_Default(this);
                    FillDataToGrid1__PatientType(this);
                    var room = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == this.currentMediStock.ID);
                    btn_Radio_Enable_Click(room);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void PatientTypeGridView_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem) && sender != null && sender is HIS.UC.PatientType.PatientTypeADO)
                {
                    var type = (HIS.UC.PatientType.Popup.PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case HIS.UC.PatientType.Popup.PopupMenuProcessor.ItemType.Copy:
                            {
                                if (isChosePatientType != 1)
                                {
                                    MessageManager.Show("Vui lòng chọn đối tượng!");
                                    break;
                                }
                                this.currentCopyPatientTypeAdo = (HIS.UC.PatientType.PatientTypeADO)sender;
                                break;
                            }
                        case HIS.UC.PatientType.Popup.PopupMenuProcessor.ItemType.Paste:
                            {
                                var currentPaste = (HIS.UC.PatientType.PatientTypeADO)sender;
                                bool success = false;
                                CommonParam param = new CommonParam();
                                if (isChosePatientType != 1)
                                {
                                    MessageManager.Show(" Vui lòng chọn đối tượng !");
                                }
                                if (this.currentCopyPatientTypeAdo == null)
                                {
                                    MessageManager.Show("Vui lòng copy!");
                                    break;
                                }
                                if (this.currentCopyPatientTypeAdo != null && currentPaste != null && isChosePatientType == 1)
                                {
                                    if (this.currentCopyPatientTypeAdo.ID == currentPaste.ID)
                                    {
                                        MessageManager.Show("Trùng dữ liệu copy và paste");
                                        break;
                                    }

                                    HisMestPatientTypeCopyByPatientTypeSDO hisMestMatyCopyByMatySDO = new HisMestPatientTypeCopyByPatientTypeSDO();
                                    hisMestMatyCopyByMatySDO.CopyPatientTypeId = currentCopyPatientTypeAdo.ID;
                                    hisMestMatyCopyByMatySDO.PastePatientTypeId = currentPaste.ID;
                                    var result = new BackendAdapter(param).Post<List<HIS_MEST_PATIENT_TYPE>>("api/HisMestPatientType/CopyByPatientType", ApiConsumer.ApiConsumers.MosConsumer, hisMestMatyCopyByMatySDO, param);
                                    if (result != null)
                                    {
                                        success = true;
                                        List<HIS.UC.MediStock.MediStockADO> dataNew = new List<HIS.UC.MediStock.MediStockADO>();
                                        dataNew = (from r in listMediStock select new MediStockADO(r)).ToList();
                                        if (result != null && result.Count > 0)
                                        {
                                            foreach (var itemMediStock in result)
                                            {
                                                var check = dataNew.FirstOrDefault(o => o.ID == itemMediStock.MEDI_STOCK_ID);
                                                if (check != null)
                                                {
                                                    check.checkMest = true;
                                                }
                                            }
                                        }
                                        dataNew = dataNew.OrderByDescending(p => p.checkMest).ToList();
                                        if (ucGridControlMediStock != null)
                                        {
                                            MediStockProcessor.Reload(ucGridControlMediStock, dataNew);
                                        }
                                        else
                                        {
                                            FillDataToGrid2__MediStock(this);
                                        }
                                    }
                                }
                                MessageManager.Show(this.ParentForm, param, success);
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MediStockGridView_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem) && sender != null && sender is HIS.UC.MediStock.MediStockADO)
                {
                    var type = (HIS.UC.MediStock.Popup.PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case HIS.UC.MediStock.Popup.PopupMenuProcessor.ItemType.Copy:
                            {
                                if (isChoseMediStock != 2)
                                {
                                    MessageManager.Show("Vui lòng chọn Kho!");
                                    break;
                                }
                                this.CurrentMediStockCopyAdo = (HIS.UC.MediStock.MediStockADO)sender;
                                break;
                            }
                        case HIS.UC.MediStock.Popup.PopupMenuProcessor.ItemType.Paste:
                            {
                                var currentPaste = (HIS.UC.MediStock.MediStockADO)sender;
                                bool success = false;
                                CommonParam param = new CommonParam();
                                if (this.CurrentMediStockCopyAdo == null && isChoseMediStock != 2)
                                {
                                    MessageManager.Show("Vui lòng copy!");
                                    break;
                                }
                                if (this.CurrentMediStockCopyAdo != null && currentPaste != null && isChoseMediStock == 2)
                                {
                                    if (this.CurrentMediStockCopyAdo.ID == currentPaste.ID)
                                    {
                                        MessageManager.Show("Trùng dữ liệu copy và paste");
                                        break;
                                    }
                                    HisMestPatientTypeCopyByMediStockSDO hisMestMatyCopyByMatySDO = new HisMestPatientTypeCopyByMediStockSDO();
                                    hisMestMatyCopyByMatySDO.CopyMediStockId = CurrentMediStockCopyAdo.ID;
                                    hisMestMatyCopyByMatySDO.PasteMediStockId = currentPaste.ID;
                                    var result = new BackendAdapter(param).Post<List<HIS_MEST_PATIENT_TYPE>>("api/HisMestPatientType/CopyByMediStock", ApiConsumer.ApiConsumers.MosConsumer, hisMestMatyCopyByMatySDO, param);
                                    if (result != null)
                                    {
                                        success = true;
                                        List<HIS.UC.PatientType.PatientTypeADO> dataNew = new List<HIS.UC.PatientType.PatientTypeADO>();
                                        dataNew = (from r in listPatientType select new HIS.UC.PatientType.PatientTypeADO(r)).ToList();
                                        if (result != null && result.Count > 0)
                                        {

                                            foreach (var itemService in result)
                                            {
                                                var check = dataNew.FirstOrDefault(o => o.ID == itemService.PATIENT_TYPE_ID);
                                                if (check != null)
                                                {
                                                    check.checkMedi = true;
                                                }
                                            }

                                            dataNew = dataNew.OrderByDescending(p => p.checkMedi).ToList();

                                            if (ucGridControlPatientType != null)
                                            {
                                                PatientTypeProcessor.Reload(ucGridControlPatientType, dataNew);
                                            }
                                        }
                                        else
                                        {
                                            FillDataToGrid1__PatientType(this);
                                        }
                                    }
                                }
                                MessageManager.Show(this.ParentForm, param, success);
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}







