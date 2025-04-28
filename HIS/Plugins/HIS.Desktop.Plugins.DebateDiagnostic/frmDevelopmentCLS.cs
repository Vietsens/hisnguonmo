using DevExpress.XtraGrid.Views.Base;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.DebateDiagnostic
{
    public partial class frmDevelopmentCLS : Form
    {
        public frmDevelopmentCLS()
        {
            InitializeComponent();
        }

        private void frmDevelopmentCLS_Load(object sender, EventArgs e)
        {
            try
            {
                //deStartTime.Properties.VistaEditTime = DevExpress.Utils.DefaultBoolean.True;
                //deEndTime.Properties.VistaEditTime = DevExpress.Utils.DefaultBoolean.True;
                deStartTime.EditValue = DateTime.Today.Date;
                deEndTime.EditValue = DateTime.Today.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
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
                            if (e.Column.FieldName == "TRACKING_TIME")
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
            try
            {
                if(e.KeyChar == (char)Keys.Enter)
                {
                    string searchText = txtSearch.Text.Trim();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
    }
}
