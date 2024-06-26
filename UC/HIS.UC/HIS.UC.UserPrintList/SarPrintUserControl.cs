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
using Inventec.Core;
using Inventec.Common.Adapter;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HIS.UC.UserPrintList.SarPrintUserControl
{
    public partial class SarPrintUserControl : UserControl
    {
        #region Declare
        public string sarPrintId;
        #endregion

        #region Contructor
        public SarPrintUserControl()
        {
            InitializeComponent();
        }
        public SarPrintUserControl(string SarPrintId)
        {
            try
            {
                this.sarPrintId = SarPrintId;
                FillDataToSarPrintGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Process
        public void FillDataToSarPrintGridControl()
        {
            try
            {
                if (!String.IsNullOrEmpty(this.sarPrintId))
                {
                    List<long> sarPrintIds = new List<long>();
                    var arrIds = this.sarPrintId.Split(',', ';');
                    if (arrIds != null && arrIds.Length > 0)
                    {
                        foreach (var id in arrIds)
                        {
                            long printId = Inventec.Common.TypeConvert.Parse.ToInt64(id);
                            if (printId > 0)
                            {
                                sarPrintIds.Add(printId);
                            }
                        }
                    }
                    SAR.Filter.SarPrintFilter printFilter = new SAR.Filter.SarPrintFilter();
                    if (this.sarPrintId == null || sarPrintIds.Count == 0)
                    {
                        printFilter.ID = -1;
                    }
                    else
                        printFilter.IDs = sarPrintIds;
                    var prints = new BackendAdapter(new CommonParam()).Get<List<SAR.EFMODEL.DataModels.SAR_PRINT>>(UserPrintListConfig.SAR_PRINT_GET, UserPrintListConfig.SarConsumer, printFilter, new CommonParam());
                    gridControlSarPrint.BeginUpdate();
                    gridControlSarPrint.DataSource = null;
                    gridControlSarPrint.DataSource = prints;
                    gridControlSarPrint.EndUpdate();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void gridViewSarPrint_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    SAR.EFMODEL.DataModels.SAR_PRINT data = (SAR.EFMODEL.DataModels.SAR_PRINT)((System.Collections.IList)((DevExpress.XtraGrid.Views.Base.BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonEditEditSarPrint_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var sarPrint = (SAR.EFMODEL.DataModels.SAR_PRINT)gridViewSarPrint.GetFocusedRow();
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(UserPrintListConfig.SarConsumer, UserPrintListConfig.URI_API_SAR, UserPrintListConfig.Language, UserPrintListConfig.TemnplateFolderPath);
                richEditorMain.RunPrintEditor(sarPrint, updateDataSuccess);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public bool updateDataSuccess(SAR.EFMODEL.DataModels.SAR_PRINT sarPrint)
        {
            FillDataToSarPrintGridControl();
            return true;
        }
    }
}
