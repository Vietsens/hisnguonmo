using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;

namespace HIS.Desktop.Plugins.DrugUsageAnalysis
{
    public partial class UCDrugUsageAnalysis : UserControlBase
    {
        private void gridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var RowCellClickBedRoom = (L_HIS_TREATMENT_BED_ROOM)gridView1.GetFocusedRow();
                gridView1.OptionsSelection.EnableAppearanceFocusedCell = true;
                gridView1.OptionsSelection.EnableAppearanceFocusedRow = true;
                if (RowCellClickBedRoom != null)
                {
                    LogTheadInSessionInfo(() => SelectPatient(RowCellClickBedRoom), "gridView1RowClick");
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SelectPatient(L_HIS_TREATMENT_BED_ROOM rowclickBedRoom)
        {
            try
            {
                LoadDataToTreeList(rowclickBedRoom.TREATMENT_ID);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                L_HIS_TREATMENT_BED_ROOM data = (L_HIS_TREATMENT_BED_ROOM)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (e.Column.FieldName == "STT")
                    {
                        if (ucPaging.pagingGrid != null)
                        {
                            e.Value = e.ListSourceRowIndex + 1 + (ucPaging.pagingGrid.CurrentPage - 1) * (ucPaging.pagingGrid.PageSize);
                        }
                        else
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                    }
                    else if (e.Column.FieldName == "TDL_PATIENT_DOB_STR")
                    {
                        try
                        {
                            //if (data.PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                            //{
                            //    e.Value = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                            //}
                            //else
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao TDL_PATIENT_DOB", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IN_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.IN_TIME);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao SYNC_TIME", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
