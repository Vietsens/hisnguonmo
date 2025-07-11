using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.Desktop.Plugins.DrugUsageAnalysis
{
    public partial class UCDrugUsageAnalysis : UserControlBase
    {
        private List<V_HIS_BED_ROOM> BedRoomSelecteds { get; set; }
        private List<V_HIS_BED_ROOM> BedRoomDataSource { get; set; } = new List<V_HIS_BED_ROOM>();
        private void InitComboBedRoom()
        {
            try
            {
                this.BedRoomDataSource = BackendDataWorker.Get<V_HIS_BED_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                .OrderBy(o => o.BED_ROOM_NAME).ToList();
                this.InitCombo(cboBedRoom, BedRoomDataSource, 
                    "BED_ROOM_NAME",
                    "ID",
                    cboBedRoom_MarksSelection,
                    cboBedRoom_CustomDisplayText
                    );
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBedRoom_MarksSelection(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        List<V_HIS_BED_ROOM> sgSelectedNews = new List<V_HIS_BED_ROOM>();
                        foreach (V_HIS_BED_ROOM rv in (gridCheckMark).Selection)
                        {
                            if (rv != null)
                            {
                                if (sb.ToString().Length > 0) { sb.Append(", "); }
                                sb.Append(rv.BED_ROOM_NAME.ToString());
                                sgSelectedNews.Add(rv);
                            }
                        }
                        this.BedRoomSelecteds = new List<V_HIS_BED_ROOM>();
                        this.BedRoomSelecteds.AddRange(sgSelectedNews);
                    }
                    this.cboBedRoom.Text = sb.ToString();
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

        private void SetValueBedRoom(GridLookUpEdit gridLookUpEdit)
        {
            try
            {
                var allData = LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_BED_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .ToList();
                if (this.DepartmentSelecteds != null && this.DepartmentSelecteds.Count > 0)
                {
                    allData = allData.Where(bed => this.DepartmentSelecteds.Exists(dep => bed.DEPARTMENT_ID == dep.ID)).ToList();
                }
                gridLookUpEdit.Properties.DataSource = allData;
                this.cboClearSelection(gridLookUpEdit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void cboBedRoom_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit 
                    ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection 
                    : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null || gridCheckMark.Selection == null || gridCheckMark.Selection.Count == 0)
                {
                    e.DisplayText = "";
                    return;
                }
                StringBuilder sb = new StringBuilder();
                foreach (V_HIS_BED_ROOM rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }

                    sb.Append(rv.BED_ROOM_NAME.ToString());
                }
                if (BedRoomSelecteds != null && BedRoomSelecteds.Count == this.BedRoomDataSource.Count)
                {
                    sb = new StringBuilder("Tất cả");
                }
                e.DisplayText = sb.ToString();
                var g = sender as DevExpress.XtraEditors.GridLookUpEdit;
                if (g != null)
                {
                    g.Text = e.DisplayText;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessSelectBedRoom()
        {
            try
            {

                //GridCheckMarksSelection gridCheckMark = cboBedRoom.Properties.Tag as GridCheckMarksSelection;
                //if (gridCheckMark != null)
                //{
                //    gridCheckMark.ClearSelection(cboBedRoom.Properties.View);
                //}
                //if (cboBedRoom.Properties.Tag != null)
                //{
                //    List<HIS_DEPARTMENT> ds = cboBedRoom.Properties.DataSource as List<HIS_DEPARTMENT>;

                //    HIS_DEPARTMENT row = ds != null ? ds.FirstOrDefault(o => o.ID == departmentId) : null;
                //    if (row != null)
                //    {
                //        List<HIS_DEPARTMENT> selects = new List<HIS_DEPARTMENT>();
                //        selects.Add(row);
                //        gridCheckMark.SelectAll(selects);
                //    }
                //}
                //else
                //{
                //    this.cboClearSelection(cboBedRoom);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
