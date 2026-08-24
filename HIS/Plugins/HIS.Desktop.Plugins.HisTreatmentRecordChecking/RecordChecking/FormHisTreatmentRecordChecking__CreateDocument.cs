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
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.HisTreatmentRecordChecking.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisTreatmentRecordChecking.RecordChecking
{
    /// <summary>
    /// Task 53180 - create a document for the order currently selected (QT-13 .. QT-20).
    ///
    /// Where the choices come from:
    ///   HIS.Desktop.Plugins.EmrDocumentType  -> EMR_DOCUMENT_TYPE (left grid, already loaded)
    ///   SAR.Desktop.Plugins.SarPrintType     -> SAR_PRINT_TYPE, linked by EMR_DOCUMENT_TYPE_CODE
    /// Nothing is hard coded: the print templates offered are exactly what the
    /// hospital configured on those two screens.
    /// </summary>
    public partial class FormHisTreatmentRecordChecking
    {
        /// <summary>
        /// True when this order may still get a document (QT-14).
        /// </summary>
        private bool CanCreateDocument(InfoRecordADO row)
        {
            try
            {
                return row != null
                    && row.DOC_STATUS == EnumRecordDocumentStatus.NoDocument
                    && CurrentType != null
                    && !string.IsNullOrWhiteSpace(row.SEARCH_CODE);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return false;
        }

        /// <summary>
        /// Hides the button on rows that already have a document, so only the rows
        /// still to be completed show it (QT-14).
        /// </summary>
        private void Gv_InfoRecord_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column != Gv_IR_CreateDoc) return;

                InfoRecordADO row = Gv_InfoRecord.GetRow(e.RowHandle) as InfoRecordADO;
                if (!CanCreateDocument(row))
                {
                    e.RepositoryItem = repositoryItemTextEdit1;   // plain cell = no button
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Button of one row was pressed.</summary>
        private void repositoryItemButtonCreateDoc_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CreateDocumentForFocusedRow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Print templates configured for the document type currently selected (QT-15).
        /// Read from the local cache - no API call.
        /// </summary>
        private List<SAR_PRINT_TYPE> GetPrintTypesOfCurrentDocumentType()
        {
            List<SAR_PRINT_TYPE> result = new List<SAR_PRINT_TYPE>();
            try
            {
                if (CurrentType == null || string.IsNullOrWhiteSpace(CurrentType.DOCUMENT_TYPE_CODE))
                    return result;

                result = BackendDataWorker.Get<SAR_PRINT_TYPE>()
                    .Where(o => o.EMR_DOCUMENT_TYPE_CODE == CurrentType.DOCUMENT_TYPE_CODE
                             && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .OrderBy(o => o.PRINT_TYPE_NAME)
                    .ToList();
            }
            catch (Exception ex)
            {
                result = new List<SAR_PRINT_TYPE>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Offers the print templates of the current document type for the focused order
        /// and creates the document (QT-13 .. QT-18).
        /// </summary>
        private void CreateDocumentForFocusedRow()
        {
            try
            {
                InfoRecordADO row = Gv_InfoRecord.GetFocusedRow() as InfoRecordADO;
                if (!CanCreateDocument(row)) return;   // QT-13, QT-14

                List<SAR_PRINT_TYPE> printTypes = GetPrintTypesOfCurrentDocumentType();

                Inventec.Common.Logging.LogSystem.Debug("btnCreateDocument____documentTypeCode="
                    + (CurrentType != null ? CurrentType.DOCUMENT_TYPE_CODE : "")
                    + "; printTypes=" + printTypes.Count);

                if (printTypes.Count == 0)                       // QT-16
                {
                    XtraMessageBox.Show(
                        Resources.ResourceMessage.LoaiVanBanChuaCauHinhBieuIn,
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(
                            HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (printTypes.Count == 1)                       // QT-17
                {
                    CreateDocumentByPrintType(row, printTypes[0].PRINT_TYPE_CODE);
                    return;
                }

                ShowPrintTypeMenu(printTypes);                  // QT-18
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Lets the user pick one template when the document type has several (QT-18).
        /// </summary>
        private void ShowPrintTypeMenu(List<SAR_PRINT_TYPE> printTypes)
        {
            try
            {
                // Same popup pattern as the other plugins (PopupMenu + BarButtonItem).
                PopupMenu menu = new PopupMenu(barManager1);
                foreach (var printType in printTypes)
                {
                    BarButtonItem item = new BarButtonItem(barManager1, printType.PRINT_TYPE_NAME);
                    item.Tag = printType.PRINT_TYPE_CODE;
                    item.ItemClick += new ItemClickEventHandler(PrintTypeMenuItem_Click);
                    menu.AddItem(item);
                }

                menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintTypeMenuItem_Click(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e == null || e.Item == null || e.Item.Tag == null) return;

                InfoRecordADO row = Gv_InfoRecord.GetFocusedRow() as InfoRecordADO;
                if (row == null) return;

                CreateDocumentByPrintType(row, e.Item.Tag.ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Routes the chosen template to the print library able to build its data.
        /// Only the order based document types are supported for now; anything else
        /// reports back instead of failing (QT-16).
        /// </summary>
        private void CreateDocumentByPrintType(InfoRecordADO row, string printTypeCode)
        {
            try
            {
                if (row == null || string.IsNullOrWhiteSpace(printTypeCode)) return;

                bool isServiceReqDocument =
                    row.DOCUMENT_TYPE_ID == IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_ASSIGN
                    || row.DOCUMENT_TYPE_ID == IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_RESULT;

                if (!isServiceReqDocument)
                {
                    XtraMessageBox.Show(
                        Resources.ResourceMessage.BieuMauChuaDuocHoTro,
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(
                            HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                CreateDocumentForServiceReq(row, printTypeCode);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Builds the input of Library.PrintServiceReq and runs it.
        /// Data shape copied from the assign screens that already use this library.
        /// </summary>
        private void CreateDocumentForServiceReq(InfoRecordADO row, string printTypeCode)
        {
            try
            {
                WaitingManager.Show();

                // 1. The order itself, identified by its code.
                CommonParam paramCommon = new CommonParam();
                HisServiceReqViewFilter serviceReqFilter = new HisServiceReqViewFilter();
                serviceReqFilter.SERVICE_REQ_CODE = row.CODE;

                V_HIS_SERVICE_REQ serviceReq = new BackendAdapter(paramCommon).Get<List<V_HIS_SERVICE_REQ>>(
                    "api/HisServiceReq/GetView", ApiConsumers.MosConsumer, serviceReqFilter,
                    SessionManager.ActionLostToken, paramCommon)?.FirstOrDefault();

                if (serviceReq == null)
                {
                    WaitingManager.Hide();
                    XtraMessageBox.Show(
                        Resources.ResourceMessage.KhongTimThayYLenh,
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(
                            HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2. Services of that order.
                HisSereServViewFilter sereServFilter = new HisSereServViewFilter();
                sereServFilter.SERVICE_REQ_ID = serviceReq.ID;
                List<V_HIS_SERE_SERV> sereServs = new BackendAdapter(paramCommon).Get<List<V_HIS_SERE_SERV>>(
                    "api/HisSereServ/GetView", ApiConsumers.MosConsumer, sereServFilter,
                    SessionManager.ActionLostToken, paramCommon) ?? new List<V_HIS_SERE_SERV>();

                HisServiceReqListResultSDO sdo = new HisServiceReqListResultSDO();
                sdo.ServiceReqs = new List<V_HIS_SERVICE_REQ>() { serviceReq };
                sdo.SereServs = sereServs;

                // 3. Treatment. HisTreatmentWithPatientTypeInfoSDO is only a mapped HIS_TREATMENT.
                HIS_TREATMENT treatment = GetTreatmentForPrint(row, serviceReq.TREATMENT_ID, paramCommon);
                if (treatment == null)
                {
                    WaitingManager.Hide();
                    XtraMessageBox.Show(
                        Resources.ResourceMessage.KhongTimThayHoSo,
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(
                            HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                HisTreatmentWithPatientTypeInfoSDO treatmentSdo = new HisTreatmentWithPatientTypeInfoSDO();
                Inventec.Common.Mapper.DataObjectMapper.Map<HisTreatmentWithPatientTypeInfoSDO>(treatmentSdo, treatment);

                // 4. Bed logs of the requesting department.
                HisBedLogViewFilter bedLogFilter = new HisBedLogViewFilter();
                bedLogFilter.TREATMENT_ID = treatment.ID;
                bedLogFilter.DEPARTMENT_IDs = new List<long>() { serviceReq.REQUEST_DEPARTMENT_ID };
                List<V_HIS_BED_LOG> bedLogs = new BackendAdapter(paramCommon).Get<List<V_HIS_BED_LOG>>(
                    "api/HisBedLog/GetView", ApiConsumers.MosConsumer, bedLogFilter,
                    SessionManager.ActionLostToken, paramCommon) ?? new List<V_HIS_BED_LOG>();

                WaitingManager.Hide();

                // 5. Create the EMR document. EmrSignNow means: build the document and sign it,
                //    without sending anything to a printer.
                var printProcessor = new HIS.Desktop.Plugins.Library.PrintServiceReq.PrintServiceReqProcessor(
                    sdo, treatmentSdo, bedLogs,
                    this.moduleData != null ? this.moduleData.RoomId : (long?)null,
                    MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow);

                printProcessor.Print(printTypeCode);

                Inventec.Common.Logging.LogUtil.LogActionSuccess(
                    "FormHisTreatmentRecordChecking", "CreateDocument",
                    Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());

                // QT-19: refresh so the status of the order is up to date.
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Treatment used for printing: already loaded in mode 1, fetched by id in mode 2.
        /// </summary>
        private HIS_TREATMENT GetTreatmentForPrint(InfoRecordADO row, long treatmentIdOfServiceReq, CommonParam paramCommon)
        {
            try
            {
                if (!IsFilterByDoctorMode() && CurrentTreatment != null && CurrentTreatment.Treatment != null)
                {
                    return CurrentTreatment.Treatment;
                }

                HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.ID = (row != null && row.TREATMENT_ID.HasValue)
                    ? row.TREATMENT_ID.Value
                    : treatmentIdOfServiceReq;

                return new BackendAdapter(paramCommon).Get<List<HIS_TREATMENT>>(
                    "api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter,
                    SessionManager.ActionLostToken, paramCommon)?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }
    }
}
