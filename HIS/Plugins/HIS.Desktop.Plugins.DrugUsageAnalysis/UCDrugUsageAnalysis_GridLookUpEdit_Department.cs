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
        List<HIS_DEPARTMENT> DepartmentSelecteds;
        List<HIS_DEPARTMENT> DepartmentsDataSource;
        private UCDrugUsageAnalysis ucDrugUsageAnalysis;
        private void InitComboDepartment()
        {
            this.DepartmentsDataSource = BackendDataWorker.Get<HIS_DEPARTMENT>()
                .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                .OrderBy(o => o.DEPARTMENT_NAME).ToList();
            this.InitCombo(cboDepartment,
                DepartmentsDataSource,
                 "DEPARTMENT_NAME",
                 "ID",
                cboDepartment_MarksSelection,
                cboDepartment_CustomDisplayText
                );
        }

        private void cboDepartment_MarksSelection(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_DEPARTMENT> sgSelectedNews = new List<HIS_DEPARTMENT>();
                    foreach (HIS_DEPARTMENT rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.DEPARTMENT_NAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this.DepartmentSelecteds = new List<HIS_DEPARTMENT>();
                    this.DepartmentSelecteds.AddRange(sgSelectedNews);
                    this.cboDepartment.Text = sb.ToString();
                    //
                    SetValueBedRoom(cboBedRoom);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void cboDepartment_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null || gridCheckMark.Selection == null || gridCheckMark.Selection.Count == 0)
                {
                    e.DisplayText = "";
                    return;
                }
                foreach (HIS_DEPARTMENT rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }

                    sb.Append(rv.DEPARTMENT_NAME.ToString());
                }
                if (DepartmentSelecteds != null && DepartmentSelecteds.Count == this.DepartmentsDataSource.Count)
                {
                    sb = new StringBuilder("Tất cả");
                }
                e.DisplayText = sb.ToString();
                var g = sender as DevExpress.XtraEditors.GridLookUpEdit;
                g.Text = e.DisplayText;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessSelectDepartment()
        {
            try
            {
                //GridCheckMarksSelection gridCheckMark = cboDepartment.Properties.Tag as GridCheckMarksSelection;
                //if (gridCheckMark != null)
                //{
                //    gridCheckMark.ClearSelection(cboDepartment.Properties.View);
                //}
                //if (cboDepartment.Properties.Tag != null)
                //{
                //    List<HIS_DEPARTMENT> ds = cboDepartment.Properties.DataSource as List<HIS_DEPARTMENT>;

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
                //    this.cboClearSelection(cboDepartment);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
