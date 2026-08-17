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
using ACS.EFMODEL.DataModels;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using EMR.SDO;
using EMR.TDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.HisTreatmentRecordChecking.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.SignLibrary;
using Inventec.Common.SignLibrary.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace HIS.Desktop.Plugins.HisTreatmentRecordChecking.RecordChecking
{
    /// <summary>
    /// Grid events: row click, row style, unbound data, sorting, control state of filter checkboxes, resize.
    /// </summary>
    public partial class FormHisTreatmentRecordChecking
    {
        private void Gv_EmrDocument_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var row = (V_EMR_DOCUMENT)Gv_EmrDocument.GetFocusedRow();
                if (row != null)
                {
                    if (e.Column.FieldName == "CREATOR" && !string.IsNullOrEmpty(row.CREATOR))
                    {
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.InfoUser").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.InfoUser'");
                        if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.InfoUser' is not plugins");
                        List<object> listArgs = new List<object>();
                        listArgs.Add(row.CREATOR);
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, moduleData.RoomId, moduleData.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");
                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GvEmrDocumentType_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    EmrDocumentTypeADO CurrentType = (EmrDocumentTypeADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "DOCUMENT_TYPE_NAME")
                    {
                        if (CurrentType.IsHasDocumentNoPatientSign)
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                        else if (CurrentType.IsHasDocument)
                        {
                            e.Appearance.ForeColor = Color.Blue;
                        }
                        else
                        {
                            e.Appearance.ForeColor = Color.Black;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void OrderListByCheckBox()
        {
            try
            {
                if (chkUuTien.Checked)
                {
                    // Day len dau moi loai CO DU LIEU o luoi giua (co van ban HOAC co y lenh),
                    // khong chi loai da co van ban - xem EmrDocumentTypeADO.IsHasData.
                    var lstSortTemp = ListDocumentType.Where(o => o.IsHasData).OrderByDescending(o => o.IsHasData).ThenByDescending(o => o.NUM_ORDER).ThenBy(o => o.DOCUMENT_TYPE_NAME).ToList();
                    lstSortTemp.AddRange(ListDocumentType.Where(o => !o.IsHasData && o.NUM_ORDER == null).OrderByDescending(o => o.NUM_ORDER).ThenBy(o => o.DOCUMENT_TYPE_NAME).ToList());
                    lstSortTemp.AddRange(ListDocumentType.Where(o => !o.IsHasData && o.NUM_ORDER != null).OrderByDescending(o => o.NUM_ORDER).ThenBy(o => o.DOCUMENT_TYPE_NAME).ToList());
                    ListDocumentType = lstSortTemp;
                }
                else
                {
                    var lstSortTemp = ListDocumentType.Where(o => o.NUM_ORDER == null).OrderBy(o => o.DOCUMENT_TYPE_NAME).ToList();
                    lstSortTemp.AddRange(ListDocumentType.Where(o => o.NUM_ORDER != null).OrderByDescending(o => o.NUM_ORDER).ThenBy(o => o.DOCUMENT_TYPE_NAME).ToList());
                    ListDocumentType = lstSortTemp;
                }
                GcEmrDocumentType.BeginUpdate();
                GcEmrDocumentType.DataSource = ListDocumentType;
                GcEmrDocumentType.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkUuTien_CheckedChanged(object sender, EventArgs e)
        {
            // Block while InitControlState() is restoring the saved state,
            // otherwise the handler re-sorts the grid and writes back to SQLite during Load.
            if (IsLoadFirstForm) return;

            try
            {

                OrderListByCheckBox();


                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListDocumentType.Select(o => new { o.DOCUMENT_TYPE_NAME, o.NUM_ORDER }).ToList()), ListDocumentType.Select(o => new { o.DOCUMENT_TYPE_NAME, o.NUM_ORDER }).ToList()));
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (currentControlStateRDO != null && currentControlStateRDO.Count > 0) ? currentControlStateRDO.Where(o => o.KEY == chkUuTien.Name && o.MODULE_LINK == "HIS.Desktop.Plugins.HisTreatmentRecordChecking").FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = chkUuTien.Checked ? "1" : "0";
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkUuTien.Name;
                    csAddOrUpdate.VALUE = chkUuTien.Checked ? "1" : "0";
                    csAddOrUpdate.MODULE_LINK = "HIS.Desktop.Plugins.HisTreatmentRecordChecking";
                    if (currentControlStateRDO == null)
                        currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    currentControlStateRDO.Add(csAddOrUpdate);
                }
                controlStateWorker.SetData(currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Gv_Treatment_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_TREATMENT data = (V_HIS_TREATMENT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "PATIENT_DOB_ForDisplay")
                        {
                            string patientDOB = (view.GetRowCellValue(e.ListSourceRowIndex, "TDL_PATIENT_DOB") ?? "").ToString();
                            e.Value = patientDOB.Length >= 4 ? patientDOB.Substring(0, 4) : "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Gv_Treatment_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    V_HIS_TREATMENT data = (V_HIS_TREATMENT)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.RowHandle == view.FocusedRowHandle && this.hasSelectedTreatment)
                    {
                        e.Appearance.FontStyleDelta = FontStyle.Bold;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void Gv_Treatment_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    V_HIS_TREATMENT data = (V_HIS_TREATMENT)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        this.hasSelectedTreatment = true;
                        TxtTreatmentCode.Text = data.TREATMENT_CODE;
                        Gv_Treatment.RefreshRow(e.RowHandle);
                        FillDataToGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void chkToiTao_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void FormHisTreatmentRecordChecking_Resize(object sender, EventArgs e)
        {
            try
            {
                SetCustomSizeForGridView(ref Gv_Treatment);
                SetCustomSizeForGridView(ref Gv_EmrDocument);
                SetCustomSizeForGridView(ref Gv_InfoRecord);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void SetCustomSizeForGridView(ref DevExpress.XtraGrid.Views.Grid.GridView gridView)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridViewInfo info = gridView.GetViewInfo() as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridViewInfo;
                var listEmrDocumentCols = gridView.Columns.ToList();
                if (info.Bounds.Width > listEmrDocumentCols.Sum(o => o.Width))
                {
                    gridView.OptionsView.ColumnAutoWidth = true;
                }
                else
                {
                    gridView.OptionsView.ColumnAutoWidth = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void chkIncludeCancelDoc_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (IsLoadFirstForm)
                    return;
                FillDataToGrid();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (currentControlStateRDO != null && currentControlStateRDO.Count > 0) ? currentControlStateRDO.Where(o => o.KEY == chkIncludeCancelDoc.Name && o.MODULE_LINK == "HIS.Desktop.Plugins.HisTreatmentRecordChecking").FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = chkIncludeCancelDoc.Checked ? "1" : "0";
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkIncludeCancelDoc.Name;
                    csAddOrUpdate.VALUE = chkIncludeCancelDoc.Checked ? "1" : "0";
                    csAddOrUpdate.MODULE_LINK = "HIS.Desktop.Plugins.HisTreatmentRecordChecking";
                    if (currentControlStateRDO == null)
                        currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    currentControlStateRDO.Add(csAddOrUpdate);
                }
                controlStateWorker.SetData(currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void Gv_EmrDocument_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (V_EMR_DOCUMENT)Gv_EmrDocument.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (data.IS_DELETE == IMSys.DbConfig.HIS_RS.COMMON.IS_DELETE__TRUE)
                        {
                            e.Appearance.ForeColor = Color.Red;
                            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
                        }
                        var documentType = ListDocumentType != null && ListDocumentType.Count > 0 ? ListDocumentType.FirstOrDefault(o => o.ID == data.DOCUMENT_TYPE_ID) : null;
                        if (documentType != null && documentType.PATIENT_MUST_SIGN == 1 && (String.IsNullOrEmpty(data.SIGNERS) || !data.SIGNERS.Contains("#@!@#" + data.PATIENT_CODE)))
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Gv_InfoRecord_Click(object sender, EventArgs e)
        {
            ChangeAllEditColumn(false);
        }

        private void Gv_InfoRecord_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (IsFilterByDoctorMode())
                {
                    // Mode 2: jump to the record of the focused order and switch back to mode 1.
                    OpenTreatmentOfFocusedOrder();
                    return;
                }

                ChangeAllEditColumn(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ChangeAllEditColumn(bool IsAllowEdit)
        {
            try
            {
                if (Gv_InfoRecord.FocusedRowHandle > -1)
                {
                    var columnFocus = Gv_InfoRecord.FocusedColumn;
                    if (columnFocus.FieldName == "CODE")
                    {
                        if (IsAllowEdit)
                        {

                        }
                        foreach (var item in Gv_InfoRecord.Columns.ToList())
                        {
                            if (item.FieldName == columnFocus.FieldName)
                            {
                                item.OptionsColumn.AllowEdit = IsAllowEdit;
                                item.OptionsColumn.ReadOnly = true;
                                break;
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
    }
}
