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
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.UC.SereServTree;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExportXml
{
    public partial class UCExportXml
    {

        private void dtFromExecuteTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    dtToExecuteTime.Focus();
                    dtToExecuteTime.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtToExecuteTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtFindTreatmentCode.Focus();
                    txtFindTreatmentCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtFindTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtFindTreatmentCode.Text))
                    {
                        positionHandleControl = -1;
                        if (!dxValidationProvider1.Validate())
                            return;
                        WaitingManager.Show();
                        FillDataToGridHeinApproval();
                        txtFindTreatmentCode.SelectAll();
                        WaitingManager.Hide();
                    }
                    else
                    {
                        txtKeyword.Focus();
                        txtKeyword.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind.Focus();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewHeinApprovalBhyt_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HIS_HEIN_APPROVAL)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1 + (ucPaging1.pagingGrid.CurrentPage - 1) * ucPaging1.pagingGrid.PageSize;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "HEIN_CARD_FROM_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.HEIN_CARD_FROM_TIME);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "HEIN_CARD_TO_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.HEIN_CARD_TO_TIME);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "RIGHT_ROUTE_CODE_STR")
                        {
                            try
                            {
                                if (data.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                                {
                                    e.Value = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__HEIN_CARD__RIGHT_ROUTE_CODE__DT", Base.ResourceLangManager.LanguageUCExportXml, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                                }
                                else
                                {
                                    e.Value = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__HEIN_CARD__RIGHT_ROUTE_CODE__TT", Base.ResourceLangManager.LanguageUCExportXml, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "RIGHT_ROUTE_TYPE_CODE_STR")
                        {
                            try
                            {
                                if (data.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)
                                {
                                    e.Value = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__HEIN_CARD__RIGHT_ROUTE_TYPE_CODE__CC", Base.ResourceLangManager.LanguageUCExportXml, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                                }
                                else
                                {
                                    e.Value = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__HEIN_CARD__RIGHT_ROUTE_TYPE_CODE__GT", Base.ResourceLangManager.LanguageUCExportXml, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "UP_TO_STRANDARD_CODE_STR")
                        {
                            try
                            {
                                if (data.JOIN_5_YEAR == MOS.LibraryHein.Bhyt.HeinJoin5Year.HeinJoin5YearCode.TRUE && data.PAID_6_MONTH == MOS.LibraryHein.Bhyt.HeinPaid6Month.HeinPaid6MonthCode.TRUE)
                                {
                                    e.Value = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__HEIN_CARD__UP_TO_STANDARD_CODE__DAT", Base.ResourceLangManager.LanguageUCExportXml, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                                }
                                else
                                {
                                    e.Value = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__HEIN_CARD__UP_TO_STANDARD_CODE__KHONG_DAT", Base.ResourceLangManager.LanguageUCExportXml, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "HAS_BIRTH_CERTIFICATE_STR")
                        {
                            try
                            {
                                if (data.HAS_BIRTH_CERTIFICATE == MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.TRUE)
                                {
                                    e.Value = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__HEIN_CARD__HAS_BIRTH_CERTIFICATE", Base.ResourceLangManager.LanguageUCExportXml, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                                }
                                else
                                {
                                    e.Value = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__HEIN_CARD__NOT_BIRTH_CERTIFICATE", Base.ResourceLangManager.LanguageUCExportXml, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "EXECUTE_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.EXECUTE_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void gridViewHeinApprovalBhyt_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0)
                    return;
                WaitingManager.Show();
                this.currentHeinApproval = (V_HIS_HEIN_APPROVAL)gridViewHeinApprovalBhyt.GetFocusedRow();
                FillDateToTreeSereServByHeinApproval();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewHeinApprovalBhyt_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                listSelection = new List<V_HIS_HEIN_APPROVAL>();
                var listIndex = gridViewHeinApprovalBhyt.GetSelectedRows();
                foreach (var index in listIndex)
                {
                    var heinApproval = (V_HIS_HEIN_APPROVAL)gridViewHeinApprovalBhyt.GetRow(index);
                    if (heinApproval != null)
                    {
                        listSelection.Add(heinApproval);
                    }
                }

                if (listSelection.Count > 0)
                {
                    btnExportXml.Enabled = true;
                }
                else
                {
                    btnExportXml.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeSereServ_CustomDrawNodeCell(SereServADO data, DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e)
        {
            try
            {
                if (data != null && !e.Node.HasChildren)
                {
                    if (data.VIR_TOTAL_PATIENT_PRICE.HasValue && data.VIR_TOTAL_PATIENT_PRICE.Value > 0)
                    {
                        //if (data.BILL_ID.HasValue)
                        //    e.Appearance.ForeColor = Color.Blue;
                        //else if (e.Node.Checked)
                        //{
                        //    e.Appearance.ForeColor = Color.Blue;
                        //}
                        //else
                        //{
                        //    e.Appearance.ForeColor = Color.Black;
                        //}
                    }
                    else
                    {
                        e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Italic);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDateToTreeSereServByHeinApproval()
        {
            try
            {
                var listSereServ = new List<V_HIS_SERE_SERV_5>();
                if (this.currentHeinApproval != null)
                {
                    HisSereServView5Filter ssFilter = new HisSereServView5Filter();
                    ssFilter.HEIN_APPROVAL_ID = this.currentHeinApproval.ID;
                    listSereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumers.MosConsumer, ssFilter, null);
                }
                this.ssTreeProcessor.Reload(ucSereServTree, listSereServ);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
