using DevExpress.XtraEditors.Controls;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.DrugUsageAnalysis
{
    public partial class UCDrugUsageAnalysis : UserControlBase
    {
        List<HIS_PATIENT_TYPE> PatientTypeSelecteds;
        List<HIS_PATIENT_TYPE> PatientTypeDataSource = new List<HIS_PATIENT_TYPE>();

        private void InitComboPatientType()
        {
            try
            {
                HisPatientTypeFilter filter = new HisPatientTypeFilter()
                {
                    IS_ACTIVE = 1
                };
                CommonParam commonParam = new CommonParam();
                this.PatientTypeDataSource = new BackendAdapter(commonParam).Get<List<HIS_PATIENT_TYPE>>("api/HisPatientType/Get", ApiConsumers.MosConsumer, filter, commonParam)
                   .OrderBy(o => o.PATIENT_TYPE_NAME).ToList();
                this.InitCombo(cboPatientType,
                    PatientTypeDataSource,
                    "PATIENT_TYPE_NAME", 
                    "ID",
                    cboPatientType_MarksSelection,
                    cboPatientType_CustomDisplayText
                    );
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void cboPatientType_MarksSelection(object sender, EventArgs e)
        {

            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                PatientTypeSelecteds = new List<HIS_PATIENT_TYPE>();
                if (gridCheckMark != null)
                {
                    List<HIS_PATIENT_TYPE> erSelectedNews = new List<HIS_PATIENT_TYPE>();
                    foreach (HIS_PATIENT_TYPE er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(er.PATIENT_TYPE_NAME);
                            erSelectedNews.Add(er);
                        }

                    }
                    PatientTypeSelecteds = new List<HIS_PATIENT_TYPE>();
                    PatientTypeSelecteds.AddRange(erSelectedNews);
                    this.cboPatientType.Text = sb.ToString();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string paytientType = "";
                if (this.PatientTypeSelecteds != null)
                {
                    if (this.PatientTypeSelecteds != null && this.PatientTypeSelecteds.Count > 0 && this.PatientTypeSelecteds.Count < this.PatientTypeDataSource.Count)
                    {
                        foreach (var item in this.PatientTypeSelecteds)
                        {

                            paytientType += item.PATIENT_TYPE_NAME;

                            if (!(item == PatientTypeSelecteds.Last()))
                            {
                                paytientType += ", ";
                            }

                        }
                    }
                    else if (this.PatientTypeSelecteds.Count == this.PatientTypeDataSource.Count)
                    {
                        paytientType = "Tất cả";
                    }
                }
                e.DisplayText = paytientType;
                var g = sender as DevExpress.XtraEditors.GridLookUpEdit;
                g.Text = e.DisplayText;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
        }

        private void ProcessSelectPatientType()
        {
            try
            {

                //GridCheckMarksSelection gridCheckMark = cboPatientType.Properties.Tag as GridCheckMarksSelection;
                //if (gridCheckMark != null)
                //{
                //    gridCheckMark.ClearSelection(cboPatientType.Properties.View);
                //}
                //if (cboPatientType.Properties.Tag != null)
                //{
                //    List<HIS_DEPARTMENT> ds = cboPatientType.Properties.DataSource as List<HIS_DEPARTMENT>;

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
                //    this.cboClearSelection(cboPatientType);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
