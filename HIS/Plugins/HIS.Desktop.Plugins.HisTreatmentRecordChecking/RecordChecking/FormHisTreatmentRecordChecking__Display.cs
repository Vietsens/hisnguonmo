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
    /// Grid display helpers: unbound columns, signer names, tooltips.
    /// </summary>
    public partial class FormHisTreatmentRecordChecking
    {
        /// <summary>
        /// Computes the document status of one order (QT-09).
        /// QT-10: FullySigned requires EVERY document to be signed - use All(), never Exists().
        /// Priority: NoDocument > NotSigned > Signing > FullySigned.
        /// </summary>
        private EnumRecordDocumentStatus CalcDocumentStatus(List<V_EMR_DOCUMENT> docs)
        {
            try
            {
                if (docs == null || docs.Count == 0)
                    return EnumRecordDocumentStatus.NoDocument;

                if (docs.All(o => string.IsNullOrEmpty(o.SIGNERS)))
                    return EnumRecordDocumentStatus.NotSigned;

                if (docs.All(o => !string.IsNullOrEmpty(o.SIGNERS) && string.IsNullOrEmpty(o.UN_SIGNERS)))
                    return EnumRecordDocumentStatus.FullySigned;

                return EnumRecordDocumentStatus.Signing;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return EnumRecordDocumentStatus.NoDocument;
        }

        /// <summary>Icon of a document status shown in the STT column of the order grid.</summary>
        private System.Drawing.Image GetDocumentStatusImage(EnumRecordDocumentStatus status)
        {
            try
            {
                switch (status)
                {
                    case EnumRecordDocumentStatus.NoDocument: return imageList1.Images[3];
                    case EnumRecordDocumentStatus.NotSigned: return imageList1.Images[5];
                    case EnumRecordDocumentStatus.Signing: return imageList1.Images[2];
                    case EnumRecordDocumentStatus.FullySigned: return imageList1.Images[4];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        /// <summary>Localised display name of a document status.</summary>
        private string GetDocumentStatusName(EnumRecordDocumentStatus status)
        {
            switch (status)
            {
                case EnumRecordDocumentStatus.NoDocument: return GetLangValue("DocStatus.NoDocument");
                case EnumRecordDocumentStatus.NotSigned: return GetLangValue("DocStatus.NotSigned");
                case EnumRecordDocumentStatus.Signing: return GetLangValue("DocStatus.Signing");
                case EnumRecordDocumentStatus.FullySigned: return GetLangValue("DocStatus.FullySigned");
                default: return "";
            }
        }

        private void Gv_EmrDocument_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_EMR_DOCUMENT data = (V_EMR_DOCUMENT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            if (data.IS_DELETE == IMSys.DbConfig.HIS_RS.COMMON.IS_DELETE__TRUE)
                            {
                                e.Value = imageList1.Images[5];
                            }
                            else if (string.IsNullOrEmpty(data.SIGNERS))//chưa ký
                            {
                                e.Value = null;
                            }
                            else if (!string.IsNullOrEmpty(data.SIGNERS) && !string.IsNullOrEmpty(data.UN_SIGNERS))//Đang ký
                            {
                                e.Value = imageList1.Images[1];
                            }
                            else //Đã ký
                            {
                                e.Value = imageList1.Images[0];
                            }
                        }
                        else if (e.Column.FieldName == "SIGNERS_Str")
                        {
                            if (String.IsNullOrWhiteSpace(data.SIGNERS))
                            {
                                e.Value = null;
                            }
                            else
                            {
                                e.Value = GetSigners(data.SIGNERS);
                            }
                        }
                        else if (e.Column.FieldName == "UN_SIGNERS_Str")
                        {
                            if (String.IsNullOrWhiteSpace(data.UN_SIGNERS))
                            {
                                e.Value = null;
                            }
                            else
                            {
                                e.Value = GetSigners(data.UN_SIGNERS);
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_Str")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "DOCUMENT_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.DOCUMENT_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetSigners(string str)
        {
            string result = "";
            try
            {
                if (String.IsNullOrWhiteSpace(str))
                    return result;
                List<string> listStr = new List<string>();
                var list = str.Split(',');
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        if (String.IsNullOrWhiteSpace(item))
                            continue;
                        string signer = "";
                        if (item.Contains("#@!@#")) //Mã bệnh nhân
                        {
                            try
                            {
                                int index = item.LastIndexOf("#@!@#");
                                signer = item.Substring(index + 5, 10) + " - " + GetLangValue("Common.PatientSign");//Ma benh nhan
                            }
                            catch (Exception)
                            {
                                var user = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => o.LOGINNAME.Trim() == item.Trim()).FirstOrDefault();
                                signer = String.Format("{0}{1}", item, user != null ? (" - " + user.USERNAME) : "");
                            }
                        }
                        else
                        {
                            var user = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => o.LOGINNAME.Trim() == item.Trim()).FirstOrDefault();
                            if (user != null)
                            {
                                signer = String.Format("{0}{1}", item, " - " + user.USERNAME);
                            }
                            else
                            {
                                // Not a user login name: the token is an EMR sign flow id.
                                long signId = 0;
                                V_EMR_SIGN emrSign = null;
                                if (long.TryParse(item.Trim(), out signId)
                                    && dicVEmrSign != null
                                    && dicVEmrSign.TryGetValue(signId, out emrSign)
                                    && emrSign != null)
                                {
                                    signer = emrSign.FLOW_CODE + " - " + emrSign.FLOW_NAME;
                                }
                                else
                                {
                                    signer = item;
                                }
                            }
                        }
                        listStr.Add(signer);
                    }
                    result = String.Join("; ", listStr);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void Gv_InfoRecord_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    InfoRecordADO data = (InfoRecordADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            // DOC_STATUS is pre-computed by ProcessDocumentStatus() before binding,
                            // so this handler only maps a status to its icon.
                            e.Value = GetDocumentStatusImage(data.DOC_STATUS);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && (e.SelectedControl == Gc_InfoRecord || e.SelectedControl == Gc_EmrDocument))
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = ((DevExpress.XtraGrid.GridControl)e.SelectedControl).FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column || lastGrid != e.SelectedControl.Name)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;
                            lastGrid = e.SelectedControl.Name;
                            string text = "";
                            if (info.Column.FieldName == "STT")
                            {
                                if (e.SelectedControl == Gc_InfoRecord)
                                {
                                    InfoRecordADO data = (InfoRecordADO)view.GetRow(info.RowHandle);
                                    if (data != null)
                                    {
                                        // Reuse the status resolved before binding (QT-09, QT-10).
                                        text = !string.IsNullOrEmpty(data.DOC_STATUS_NAME)
                                            ? data.DOC_STATUS_NAME
                                            : GetDocumentStatusName(data.DOC_STATUS);
                                    }
                                }
                                else if (e.SelectedControl == Gc_EmrDocument)
                                {
                                    V_EMR_DOCUMENT data = (V_EMR_DOCUMENT)view.GetRow(info.RowHandle);
                                    if (data != null)
                                    {
                                        if (data.IS_DELETE == IMSys.DbConfig.HIS_RS.COMMON.IS_DELETE__TRUE)
                                        {
                                            text = GetLangValue("DocStatus.Cancelled");
                                        }
                                        else if (string.IsNullOrEmpty(data.SIGNERS))//chua ky
                                        {
                                            text = GetLangValue("DocStatus.NotSigned");
                                        }
                                        else if (!string.IsNullOrEmpty(data.SIGNERS) && !string.IsNullOrEmpty(data.UN_SIGNERS))//dang ky
                                        {
                                            text = GetLangValue("DocStatus.Signing");
                                        }
                                        else//da ky
                                        {
                                            text = GetLangValue("DocStatus.FullySigned");
                                        }
                                    }
                                }
                            }
                            else if (info.Column.FieldName == "CREATOR")
                            {
                                InfoRecordADO data = (InfoRecordADO)view.GetRow(info.RowHandle);
                                if (data != null && !string.IsNullOrEmpty(data.CREATOR))
                                {
                                    text = GetLangValue("Gv_IR_Creator.ToolTip");
                                }

                            }

                            lastInfo = new DevExpress.Utils.ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
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
