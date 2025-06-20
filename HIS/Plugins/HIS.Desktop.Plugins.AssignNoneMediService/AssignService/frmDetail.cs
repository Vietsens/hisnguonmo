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
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using static MPS.Processor.Mps000498.PDO.Mps000498PDO;
using static MPS.ProcessorBase.PrintConfig;

namespace HIS.Desktop.Plugins.AssignNoneMediService.AssignService
{
    public partial class frmDetail : HIS.Desktop.Utility.FormBase
    {
        HisNoneMediServiceReqResultSDO serviceReqComboResultSDO;
        HIS_SERVICE_REQ_TYPE serviceReqType__NKCB;
        const string commonString__true = "1";
        //bool isLoad = false;
        MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER currentHisPatientTypeAlter;
        MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ currentServiceReq;
        List<MOS.EFMODEL.DataModels.HIS_SERE_NMSE> currentSereServs;
        HisTreatmentWithPatientTypeInfoSDO currentHisTreatment;
        PopupMenu menu;
        Dictionary<string, object> dicParamPlus { get; set; }
        Dictionary<string, Inventec.Common.BarcodeLib.Barcode> dicImageBarcodePlus { get; set; }
        Dictionary<string, System.Drawing.Image> dicImagePlus { get; set; }
        Inventec.Desktop.Common.Modules.Module currentModule;
        /// <summary>
        /// </summary>
        public enum PrintTypeBMK
        {
            IN_PHIEU_YEU_CAU,
            PHIEU_XET_NGHIEM_DOM_SOI,
            TAO_BIEU_MAU_KHAC
        }

        public frmDetail(HisNoneMediServiceReqResultSDO serviceReqComboResultSDO,
            HisTreatmentWithPatientTypeInfoSDO _currentHisTreatment, Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
            this.serviceReqComboResultSDO = serviceReqComboResultSDO;
            this.currentHisTreatment = _currentHisTreatment;
            this.currentModule = currentModule;
            this.IsUseApplyFormClosingOption = false;
        }

        private void frmDetail_Load(object sender, EventArgs e)
        {

            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }

            serviceReqType__NKCB = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().SingleOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__OT);

            //isLoad = false;
            if (this.serviceReqComboResultSDO != null)
            {
                gridControlServiceReqView.DataSource = this.serviceReqComboResultSDO.ServiceReqs;
                //LayDuLieuChungInCacPhieuChiDinh();
                gridViewServiceReqView__TabService.SelectAll();
            }

            List<MOS.EFMODEL.DataModels.HIS_TEST_SAMPLE_TYPE> testSampleType = LoadTestSampleType();
            InitComboRepositoryTestSampleType(testSampleType);

        }


        internal List<MOS.EFMODEL.DataModels.HIS_TEST_SAMPLE_TYPE> LoadTestSampleType()
        {
            List<MOS.EFMODEL.DataModels.HIS_TEST_SAMPLE_TYPE> datas = null;
            try
            {

                if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_TEST_SAMPLE_TYPE>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TEST_SAMPLE_TYPE>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    HisTestSampleTypeFilter filter = new HisTestSampleTypeFilter();
                    datas = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_TEST_SAMPLE_TYPE>>("api/HisTestSampleType/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);

                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_TEST_SAMPLE_TYPE), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
                datas = datas != null ? datas.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;

            }
            catch (Exception ex)
            {
                datas = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return datas;
        }

        private void InitComboRepositoryTestSampleType(List<MOS.EFMODEL.DataModels.HIS_TEST_SAMPLE_TYPE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TEST_SAMPLE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("TEST_SAMPLE_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TEST_SAMPLE_TYPE_NAME", "ID", columnInfos, false, 350);
                if (data != null)
                {
                    ControlEditorLoader.Load(this.repositoryItemGridLookUpEdit_TestSampleType, (data != null ? data.OrderBy(o => o.TEST_SAMPLE_TYPE_NAME).ToList() : null), controlEditorADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRepositoryTestSampleType(List<MOS.EFMODEL.DataModels.HIS_TEST_SAMPLE_TYPE> data, DevExpress.XtraEditors.GridLookUpEdit patientTypeCombo)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TEST_SAMPLE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("TEST_SAMPLE_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TEST_SAMPLE_TYPE_NAME", "ID", columnInfos, false, 350);
                if (data != null)
                {
                    ControlEditorLoader.Load(patientTypeCombo, (data != null ? data.OrderBy(o => o.TEST_SAMPLE_TYPE_NAME).ToList() : null), controlEditorADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void gridViewServiceReqView__TabService_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_SERVICE_REQ dataRow = (V_HIS_SERVICE_REQ)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    //else if (e.Column.FieldName == "PRIORIRY_DISPLAY")
                    //{
                    //    long priority = (dataRow.PRIORITY ?? 0);
                    //    if ((priority == 1))
                    //    {
                    //        e.Value = imageListPriority.Images[0];
                    //    }
                    //}
                    else if (e.Column.FieldName == "INTRUCTION_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.INTRUCTION_TIME);
                    }
                }
                else
                {
                    e.Value = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceReqView__TabService_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ data = null;

                if (e.RowHandle > -1)
                {
                    data = (MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "PRINT_DOM_SOI")
                    {
                        //if (data.SERVICE_REQ_TYPE_ID == (serviceReqType__Test != null ? serviceReqType__Test.ID : 0))
                        //{
                        //    e.RepositoryItem = ButtonEditPrintDomSoi;
                        //}
                    }
                    if (e.Column.FieldName == "TEST_SAMPLE_TYPE_ID")
                    {
                        if (data.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN)
                            e.RepositoryItem = repositoryItemGridLookUpEdit_TestSampleType;

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PrintByServiceReqType(MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ currentServiceReq)
        {
            try
            {
                if (currentServiceReq != null)
                {
                    currentSereServs = this.serviceReqComboResultSDO.SereNmse.Where(o => o.SERVICE_REQ_ID == currentServiceReq.ID).ToList();
                    if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__NKCB.ID)
                    {
                        PrintMps("Mps000502");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool PrintMps(string printTypeCode)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000502":
                        HisNoneMediServiceReqResultSDO sdo = new HisNoneMediServiceReqResultSDO(new List<V_HIS_SERVICE_REQ>() { this.currentServiceReq }, this.serviceReqComboResultSDO.SereNmse.Where(o => o.SERVICE_REQ_ID == currentServiceReq.ID).ToList());
                        var proc = new HIS.Desktop.Plugins.Library.PrintServiceReq.PrintServiceReqProcessor(sdo, this.currentHisTreatment, PreviewType.Show);
                        proc.InPhieuChiDinhNgoaiKhamBenh(printTypeCode);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }


        private void LoadBtnPrintGridServiceReqView(DevExpress.XtraBars.BarManager barManager)
        {
            try
            {
                if (menu == null)
                    menu = new PopupMenu(barManager);

                menu.ItemLinks.Clear();

                BarButtonItem itemInPhieuYeuCau = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LANGUAGEFRMASSIGNSERVICE_MERGER_PRINT_IN_PHIEU_YEU_CAU", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), 1);
                itemInPhieuYeuCau.Tag = PrintTypeBMK.IN_PHIEU_YEU_CAU;
                itemInPhieuYeuCau.ItemClick += new ItemClickEventHandler(onClickItemPrint);
                menu.AddItems(new BarItem[] { itemInPhieuYeuCau });
                menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PrintProcess(PrintTypeBMK printType)
        {
            try
            {
                switch (printType)
                {
                    case PrintTypeBMK.IN_PHIEU_YEU_CAU:
                        #region aa
                        if (currentServiceReq != null)
                        {
                            currentSereServs = serviceReqComboResultSDO.SereNmse.Where(o => o.SERVICE_REQ_ID == currentServiceReq.ID).ToList();

                            if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__NKCB.ID)
                            {
                                PrintMps("Mps000502");
                            }
                        }
                        #endregion
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        void onClickItemPrint(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                var bbtnItem1 = e.Item as BarButtonItem;
                PrintTypeBMK type = (PrintTypeBMK)(bbtnItem1.Tag);
                PrintProcess(type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ButtonEditPrintDomSoi_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (barManager2 == null)
                {
                    barManager2 = new DevExpress.XtraBars.BarManager();

                }
                barManager2.Form = this;
                LoadBtnPrintGridServiceReqView(barManager2);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnPrint_ItemClick(object sender, ItemClickEventArgs e)
        {
            btnPrint_Click(null, null);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                gridViewServiceReqView__TabService.PostEditor();

                List<V_HIS_SERVICE_REQ> ServiceReqSelects = new List<V_HIS_SERVICE_REQ>();

                foreach (var item in gridViewServiceReqView__TabService.GetSelectedRows())
                {
                    currentServiceReq = (V_HIS_SERVICE_REQ)gridViewServiceReqView__TabService.GetRow(item);
                    PrintByServiceReqType(currentServiceReq);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceReqView__TabService_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InRowCell)
                    {
                        this.currentServiceReq = (V_HIS_SERVICE_REQ)gridViewServiceReqView__TabService.GetRow(hi.RowHandle);
                        if (hi.Column.FieldName == "PrintDetail")
                        {
                            PrintByServiceReqType(this.currentServiceReq);
                        }
                    }

                    if (hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;

                            int rowHandle = gridViewServiceReqView__TabService.GetVisibleRowHandle(hi.RowHandle);
                            var dataRow = (V_HIS_SERVICE_REQ)gridViewServiceReqView__TabService.GetRow(rowHandle);
                            if (dataRow != null)
                            {
                                //if (hi.Column.FieldName == "IsChecked" && (dataRow.IsAllowChecked == false))
                                //{
                                //    (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                                //    return;
                                //}
                            }

                            view.ShowEditor();
                            CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
                            if (checkEdit == null)
                                return;

                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {
                                checkEdit.Checked = !checkEdit.Checked;
                                view.CloseEditor();
                            }
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemGridLookUpEdit_TestSampleType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemGridLookUpEdit_TestSampleType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

        }

        private void gridControlServiceReqView_Click(object sender, EventArgs e)
        {

        }

        private void gridViewServiceReqView__TabService_ShownEditor(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
