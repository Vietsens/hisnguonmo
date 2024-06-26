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
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Plugins.TrackingInMediRecord.ADO;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;

namespace HIS.Desktop.Plugins.TrackingInMediRecord.TrackingInMediRecord
{
    public partial class frmTrackingInMediRecord : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        Inventec.Desktop.Common.Modules.Module moduleData = null;
        V_HIS_MEDI_RECORD MediRecord { get; set; }
        List<HIS_TREATMENT> lsttreatment;
        List<TrackingADO> TrackingADOs;
        List<HIS_TRACKING> lstTracking;

        List<HIS_TRACKING> lstTrackingCheck;

        internal List<HIS_IMP_MEST> _ImpMests_input { get; set; }
        internal List<V_HIS_IMP_MEST_MEDICINE> _ImpMestMedis { get; set; }
        internal List<V_HIS_IMP_MEST_MATERIAL> _ImpMestMates { get; set; }
        internal bool _IsMaterial { get; set; }

        #endregion

        #region Construct

        public frmTrackingInMediRecord()
            : this(null, null)
        {

        }

        public frmTrackingInMediRecord(Inventec.Desktop.Common.Modules.Module module, V_HIS_MEDI_RECORD _MediRecord)
            : base(module)
        {
            try
            {
                InitializeComponent();
                this.moduleData = module;
                this.MediRecord = _MediRecord;
                try
                {
                    this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method
        #region load
        private void frmTrackingInMediRecord_Load(object sender, EventArgs e)
        {
            FillDataToControl();
            LoadKeysFromlanguage();
            SetDefaultValueControl();
            CheckConfigIsMaterial();

        }

        private void FillDataToControl()
        {
            try
            {
                if (this.MediRecord != null)
                {
                    this.lsttreatment = new List<HIS_TREATMENT>();
                    this.lstTracking = new List<HIS_TRACKING>();
                    this.TrackingADOs = new List<TrackingADO>();

                    CommonParam param = new CommonParam();
                    HisTreatmentFilter filter = new HisTreatmentFilter();
                    filter.PATIENT_ID = this.MediRecord.PATIENT_ID;

                    this.lsttreatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, filter, param);
                }

                if (this.lsttreatment != null && this.lsttreatment.Count > 0)
                {
                    CommonParam param1 = new CommonParam();
                    HisTrackingFilter TrackingFilter = new HisTrackingFilter();
                    TrackingFilter.TREATMENT_IDs = this.lsttreatment.Select(o => o.ID).ToList();
                    TrackingFilter.ORDER_DIRECTION = "ASC";
                    TrackingFilter.ORDER_FIELD = "TRACKING_TIME";

                    this.lstTracking = new BackendAdapter(param1).Get<List<HIS_TRACKING>>(HisRequestUriStore.HIS_TRACKING_GET, ApiConsumers.MosConsumer, TrackingFilter, param1);
                }

                if (this.lstTracking != null && this.lstTracking.Count > 0)
                {
                    foreach (var item in this.lstTracking)
                    {
                        TrackingADO MediRecordado = new TrackingADO(item);
                        MediRecordado.TREATMENT_CODE = this.lsttreatment.FirstOrDefault(o => o.ID == item.TREATMENT_ID).TREATMENT_CODE;
                        MediRecordado.IN_TIME = this.lsttreatment.FirstOrDefault(o => o.ID == item.TREATMENT_ID).IN_TIME;
                        MediRecordado.OUT_TIME = this.lsttreatment.FirstOrDefault(o => o.ID == item.TREATMENT_ID).OUT_TIME;

                        this.TrackingADOs.Add(MediRecordado);
                    }
                }
                if (this.TrackingADOs != null && this.TrackingADOs.Count > 0)
                {
                    this.TrackingADOs = this.TrackingADOs.OrderByDescending(o => o.IN_TIME).ThenByDescending(p => p.TRACKING_TIME).ToList();
                }
                gridView1.BeginUpdate();
                gridView1.GridControl.DataSource = this.TrackingADOs;
                gridView1.EndUpdate();
                gridView1.ExpandAllGroups();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        void CheckConfigIsMaterial()
        {
            try
            {
                long configQY7 = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKING_IS_MATERIAL));
                if (configQY7 != 1)
                {
                    this._IsMaterial = true;
                }
                else
                {
                    this._IsMaterial = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                this.lblStoreCode.Text = this.MediRecord.STORE_CODE;
                this.lblStoreTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.MediRecord.STORE_TIME ?? 0);
                string kho = "";
                if (this.MediRecord.DATA_STORE_ID > 0)
                {
                    CommonParam param = new CommonParam();
                    HisDataStoreFilter filter = new HisDataStoreFilter();
                    filter.ID = this.MediRecord.DATA_STORE_ID;

                    kho = new BackendAdapter(param).Get<List<HIS_DATA_STORE>>(HisRequestUriStore.HIS_DATA_STORE_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault().DATA_STORE_NAME;
                }
                else
                    kho = "";
                this.lblDataStoreName.Text = kho;
                this.lblPatientCode.Text = this.MediRecord.PATIENT_CODE;
                this.lblFullName.Text = this.MediRecord.VIR_PATIENT_NAME;
                this.lblGender.Text = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.ID == this.MediRecord.GENDER_ID).GENDER_NAME;
                this.lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.MediRecord.DOB);
                this.btnPrint.Enabled = false;
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
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                ////Gan gia tri cho cac control editor co Text/Caption/NullText/NullValuePrompt/FindNullPrompt

                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmTrackingInMediRecord.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmTrackingInMediRecord.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmTrackingInMediRecord.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("frmTrackingInMediRecord.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnPrint.Caption = Inventec.Common.Resource.Get.Value("frmTrackingInMediRecord.bbtnPrint.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmTrackingInMediRecord.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmTrackingInMediRecord.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmTrackingInMediRecord.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmTrackingInMediRecord.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmTrackingInMediRecord.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmTrackingInMediRecord.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmTrackingInMediRecord.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmTrackingInMediRecord.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmTrackingInMediRecord.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmTrackingInMediRecord.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmTrackingInMediRecord.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmTrackingInMediRecord.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("frmTrackingInMediRecord.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmTrackingInMediRecord.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmTrackingInMediRecord.layoutControlItem4.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmTrackingInMediRecord.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmTrackingInMediRecord.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmTrackingInMediRecord.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmTrackingInMediRecord.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmTrackingInMediRecord.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmTrackingInMediRecord.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmTrackingInMediRecord.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());


                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region event
        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    TrackingADO dataRow = (TrackingADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "TRACKING_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.TRACKING_TIME);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong TRACKING_TIME_STR", ex);
                            }
                        }
                        else if (e.Column.FieldName == "DEPARTMENT_NAME")
                        {
                            try
                            {
                                e.Value = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == dataRow.DEPARTMENT_ID).DEPARTMENT_NAME;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong DEPARTMENT_NAME", ex);
                            }
                        }
                        else if (e.Column.FieldName == "ROOM_NAME")
                        {
                            try
                            {
                                e.Value = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == dataRow.ROOM_ID).ROOM_NAME;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong ROOM_NAME", ex);
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.CREATE_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong CREATE_TIME_STR", ex);
                            }
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.MODIFY_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong MODIFY_TIME_STR", ex);
                            }
                        }
                        else if (e.Column.FieldName == "TREATMENT_ID_STR")
                        {
                            try
                            {
                                e.Value = string.Format("{0} ({1} - {2})", dataRow.TREATMENT_CODE, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.IN_TIME), Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.OUT_TIME ?? 0));
                            }
                            catch (Exception ex)
                            {
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


        private void repositoryItemCheck_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var row = (TrackingADO)gridView1.GetFocusedRow();
                if (row != null && TrackingADOs != null)
                {
                    var checkedit = (DevExpress.XtraEditors.CheckEdit)sender;
                    foreach (var item in TrackingADOs)
                    {
                        if (item.TREATMENT_ID == row.TREATMENT_ID)
                        {
                            item.IsCheck = checkedit.Checked;
                        }
                    }

                    gridControl1.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void gridView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (gridView1.RowCount > 0)
                {
                    lstTrackingCheck = new List<HIS_TRACKING>();
                    for (int i = 0; i < gridView1.SelectedRowsCount; i++)
                    {
                        if (gridView1.GetSelectedRows()[i] >= 0)
                        {
                            lstTrackingCheck.Add((HIS_TRACKING)gridView1.GetRow(gridView1.GetSelectedRows()[i]));
                        }
                    }

                    if (gridView1.GetSelectedRows().Count() > 0)
                    {
                        btnPrint.Enabled = true;
                    }
                    else
                    {
                        btnPrint.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        #endregion

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                PrintProcess(PrintType.IN_TO_DIEU_TRI_TRONG_BENH_AN);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomColumnSort(object sender, CustomColumnSortEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "TREATMENT_ID_STR" && e.ListSourceRowIndex1 >= 0)
                {
                    object val1 = gridView1.GetListSourceRowCellValue(e.ListSourceRowIndex1, "TRACKING_TIME");
                    object val2 = gridView1.GetListSourceRowCellValue(e.ListSourceRowIndex2, "TRACKING_TIME");
                    e.Handled = true;
                    e.Result = System.Collections.Comparer.Default.Compare(val1, val2);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
