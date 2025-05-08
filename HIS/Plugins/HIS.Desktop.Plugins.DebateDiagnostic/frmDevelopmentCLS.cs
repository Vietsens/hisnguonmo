using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTab;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.DebateDiagnostic.UcDebateDetail;
using Inventec.Common.Adapter;
using Inventec.Common.DateTime;
using Inventec.Common.Logging;
using Inventec.Common.Number;
using Inventec.Common.String;
using Inventec.Common.WebApiClient;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.DebateDiagnostic
{
    public partial class frmDevelopmentCLS : Form
    {
        internal long treatmentId;
        public UcOther ParentUcOther { get; set; }
        MOS.EFMODEL.DataModels.HIS_TRACKING tracking { get; set; }
        //private List<HIS_TRACKING> hisTracking = new List<HIS_TRACKING>();
        public frmDevelopmentCLS(long treatmentId)
        {
            InitializeComponent();
            this.treatmentId = treatmentId;
        }

        private void frmDevelopmentCLS_Load(object sender, EventArgs e)
        {
            try
            {
                deStartTime.EditValue = DateTime.Today.Date;
                deEndTime.EditValue = DateTime.Today.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                LoadTracking();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void gvDevelopmentCLS_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    if (((IList)((BaseView)sender).DataSource) != null && ((IList)((BaseView)sender).DataSource).Count > 0)
                    {
                        HIS_TRACKING hisTracking = (HIS_TRACKING)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                        if (hisTracking != null)
                        {
                            if (e.Column.FieldName == "TRACKING_TIMEStr")
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(hisTracking.TRACKING_TIME);
                            }
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);    
            }
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S))
            {
                btnSelect.PerformClick();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void LoadTracking()
        {
            try
            {               
                CommonParam param = new CommonParam();
                HisTrackingFilter filter = new HisTrackingFilter
                {
                    TREATMENT_ID = treatmentId,                   
                    TRACKING_TIME_FROM = Int64.Parse(deStartTime.DateTime.ToString("yyyyMMddHHmmss")),
                    TRACKING_TIME_TO = Int64.Parse(deEndTime.DateTime.ToString("yyyyMMddHHmmss")),
                    KEY_WORD = txtSearch.Text?.Trim()                    
                };           
                
                List<HIS_TRACKING> trackings = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_TRACKING>>("api/HisTracking/Get", ApiConsumers.MosConsumer, filter, param);
                if (trackings != null && trackings.Any())
                {
                    gcDevelopmentCLS.DataSource = trackings;
                }
                else
                {
                    gcDevelopmentCLS.DataSource = null;
                }

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedRows = gvDevelopmentCLS.GetSelectedRows();
                var trackingList = new List<HIS_TRACKING>();
                foreach (var rowHandle in selectedRows)
                { 
                    long trackingTime = System.Convert.ToInt64(gvDevelopmentCLS.GetRowCellValue(rowHandle, "TRACKING_TIME")?.ToString());
                    string contents = gvDevelopmentCLS.GetRowCellValue(rowHandle, "CONTENT")?.ToString();
                    string subclinicalCLS = gvDevelopmentCLS.GetRowCellValue(rowHandle, "SUBCLINICAL_PROCESSES")?.ToString();

                    trackingList.Add(new HIS_TRACKING
                    {
                        TRACKING_TIME = trackingTime,
                        CONTENT = contents,
                        SUBCLINICAL_PROCESSES = subclinicalCLS
                    });
                }
                var orderedSummary = string.Join("; ", trackingList.OrderBy(x => x.TRACKING_TIME)
                .Select(x =>
                {
                    var content = x.CONTENT?.Trim() ?? "";
                    var subclinical = x.SUBCLINICAL_PROCESSES?.Trim() ?? "";
                    return $"{content} + {subclinical}".Trim(' ', '+');
                }));    
                if(this.ParentUcOther != null)
                {
                    ParentUcOther.SetTreatmentTrackingText(orderedSummary);
                }
                this.Close();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                string keyword = txtSearch.Text.Trim().ToLower();
                if (e.KeyCode == Keys.Enter)
                {
                    LoadTracking();
                    if(gvDevelopmentCLS.RowCount > 0)
                    {
                        gvDevelopmentCLS.FocusedRowHandle = 0;
                        gvDevelopmentCLS.FocusedColumn = gvDevelopmentCLS.VisibleColumns[0];
                        gvDevelopmentCLS.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void deStartTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if(e.KeyCode == Keys.Enter)
                {
                    LoadTracking();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void deEndTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadTracking();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
    }
}
