using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.LocalStorage.Location;
using Inventec.Desktop.Core;
using System;
using System.Collections;
using System.Configuration;
using System.Drawing;

namespace Inventec.Desktop.Plugins.Plugin
{
    public partial class frmPluginManager : DevExpress.XtraEditors.XtraForm
    {
        public frmPluginManager()
        {
            InitializeComponent();
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void frmPluginManager_Load(object sender, EventArgs e)
        {
            try
            {
                gridViewPlugins.BeginUpdate();
                gridViewPlugins.GridControl.DataSource = Platform.PluginManager.Plugins;
                gridViewPlugins.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewPlugins_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    PluginInfo data = (PluginInfo)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    if (e.Column.FieldName == "NameDisplay")
                    {
                        e.Value = (data != null ? data.Name + ".dll" : "");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlPlugins_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (PluginInfo)gridViewPlugins.GetFocusedRow();
                if (row != null)
                {
                    gridViewLibOfPlugin.BeginUpdate();
                    gridViewLibOfPlugin.GridControl.DataSource = row.Extensions;
                    gridViewLibOfPlugin.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}