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
    /// Grid binding pipeline: FillDataToGrid and the per-grid processors.
    /// </summary>
    public partial class FormHisTreatmentRecordChecking
    {
        /// <summary>
        /// Entry point of the grid pipeline. Routes to the mode selected by the doctor filter (QT-01).
        /// </summary>
        private void FillDataToGrid()
        {
            try
            {
                if (IsFilterByDoctorMode())
                {
                    FillDataToGridByDoctor();   // mode 2 - task 53180
                    return;
                }

                FillDataToGridByTreatment();    // mode 1 - unchanged
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Mode 1: review one record identified by its code. Behaviour unchanged.
        /// </summary>
        private void FillDataToGridByTreatment()
        {
            try
            {
                if (String.IsNullOrWhiteSpace(TxtTreatmentCode.Text) && !treatmentId.HasValue) return;
                if (CurrentType == null && ListDocumentType == null) return;

                WaitingManager.Show();
                if (CurrentType == null && ListDocumentType.Count > 0)
                {
                    CurrentType = ListDocumentType.First();
                    // CurrentType is only known here, so refresh the grid captions now.
                    // ProcessCaptionGridInfoRecord() called during Load exits early because CurrentType was still null.
                    ProcessCaptionGridInfoRecord();
                }

                ListDataInfoRecord = new List<InfoRecordADO>();
                CurrentDataInfoRecord = new List<InfoRecordADO>();

                GetDataTreatment();
                EmrDocument();
                RefreshDocumentTypeFlags();
                OrderListByCheckBox();
                ProcessFillDataToGrid();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Recomputes the colour flags of the document type grid from ListDocument:
        ///   IsHasDocument               -> the type has at least one document
        ///   IsHasDocumentNoPatientSign  -> a document still misses the patient signature
        /// Shared by both modes so the left grid always reflects the data on screen.
        /// </summary>
        private void RefreshDocumentTypeFlags()
        {
            try
            {
                if (ListDocumentType == null) return;

                // Cac loai van ban dang co Y LENH o luoi giua. 9 loai trong ListTypeId hien y lenh
                // chu khong hien van ban, nen y lenh chua tao van ban van la "co du lieu".
                HashSet<long> typeIdsHasRecord = new HashSet<long>();
                if (ListDataInfoRecord != null)
                {
                    foreach (var record in ListDataInfoRecord)
                    {
                        typeIdsHasRecord.Add(record.DOCUMENT_TYPE_ID);
                    }
                }

                List<EmrDocumentTypeADO> ListDocumentTypeTemp = new List<EmrDocumentTypeADO>();
                foreach (var item in ListDocumentType)
                {
                    EmrDocumentTypeADO ado = new EmrDocumentTypeADO(item);
                    var data = (ListDocument != null && ListDocument.Count > 0) ? ListDocument.Where(o => (o.DOCUMENT_TYPE_ID ?? 0) == item.ID).ToList() : null;
                    ado.IsHasDocument = (data != null && data.Count > 0);
                    ado.IsHasData = ado.IsHasDocument || typeIdsHasRecord.Contains(item.ID);
                    if (item.PATIENT_MUST_SIGN == 1 && ado.IsHasDocument)
                    {
                        var dataNoPatientSign = data.FirstOrDefault(o => String.IsNullOrEmpty(o.SIGNERS) || !o.SIGNERS.Contains("#@!@#" + o.PATIENT_CODE));
                        if (dataNoPatientSign != null)
                            ado.IsHasDocumentNoPatientSign = true;
                    }
                    ListDocumentTypeTemp.Add(ado);
                }
                ListDocumentType = ListDocumentTypeTemp;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessFillDataToGrid()
        {
            try
            {
                Gc_InfoRecord.DataSource = null;
                Gc_EmrDocument.DataSource = null;
                if (CurrentType == null) return;

                if (ListTypeId.Contains(CurrentType.ID))
                {
                    LciTotalInfo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    ProcessDataGridInfoRecord();
                }
                else
                {
                    LciTotalInfo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    ProcessDataGridDocument();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Documents belonging to one order, taken from the pre-grouped lookup.
        /// Matching rule is the same as GetDocumentByInfoRecod: same document type
        /// and HIS_CODE containing the SEARCH_CODE of the order.
        /// </summary>
        private List<V_EMR_DOCUMENT> MatchDocuments(ILookup<long, V_EMR_DOCUMENT> docsByType, InfoRecordADO record)
        {
            List<V_EMR_DOCUMENT> result = new List<V_EMR_DOCUMENT>();
            try
            {
                if (docsByType == null || record == null) return result;
                if (string.IsNullOrWhiteSpace(record.SEARCH_CODE))
                {
                    // No search code: fall back to the original rule (documents not owned by any order).
                    return GetDocumentByInfoRecod(record);
                }

                result = docsByType[record.DOCUMENT_TYPE_ID]
                    .Where(o => !string.IsNullOrWhiteSpace(o.HIS_CODE) && o.HIS_CODE.Contains(record.SEARCH_CODE))
                    .ToList();
            }
            catch (Exception ex)
            {
                result = new List<V_EMR_DOCUMENT>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Resolves DOC_STATUS / DOC_STATUS_NAME for every row (QT-09, QT-10).
        /// Call once, right before binding the grid.
        /// </summary>
        private void ProcessDocumentStatus(List<InfoRecordADO> records)
        {
            try
            {
                if (records == null || records.Count == 0) return;

                // Group the documents by type once. Without it every row would scan the
                // whole document list, which is O(rows * documents) in mode 2.
                ILookup<long, V_EMR_DOCUMENT> docsByType = (ListDocument != null)
                    ? ListDocument.ToLookup(o => o.DOCUMENT_TYPE_ID ?? 0)
                    : null;

                foreach (var record in records)
                {
                    record.DOC_STATUS = CalcDocumentStatus(MatchDocuments(docsByType, record));
                    record.DOC_STATUS_NAME = GetDocumentStatusName(record.DOC_STATUS);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessDataGridInfoRecord()
        {
            try
            {
                if (ListDocument != null && CurrentType != null)
                {
                    //if (CurrentType.ID == IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_RESULT)
                    //{
                    //    CurrentInfoRecord = ListDataInfoRecord.Where(o => o.DOCUMENT_TYPE_ID == IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_ASSIGN).ToList();
                    //}
                    //else
                    {

                        //var ListServiceAssign = ListDataInfoRecord.Where(o => o.DOCUMENT_TYPE_ID == IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_ASSIGN).ToList();

                        //if (ListServiceAssign != null && ListServiceAssign.Count > 0)
                        //{
                        //    if (ListDocument != null && ListDocument.Count > 0)
                        //    {
                        //        foreach (var item in ListServiceAssign)
                        //        {
                        //            var docs = ListDocument.Where(o => (o.DOCUMENT_TYPE_ID ?? 0) == IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_RESULT && !String.IsNullOrWhiteSpace(o.HIS_CODE) && o.HIS_CODE.Contains(item.SEARCH_CODE)).ToList();
                        //            if (docs != null && docs.Count > 0)
                        //            {
                        //                InfoRecordADO ado = new InfoRecordADO();

                        //                ado.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_RESULT;
                        //                ado.CODE = item.CODE;
                        //                ado.TYPE = item.TYPE;
                        //                ado.CREATE_TIME_STR = item.CREATE_TIME_STR;
                        //                ado.DEPARTMENT_NAME = item.DEPARTMENT_NAME;
                        //                ado.SEARCH_CODE = item.SEARCH_CODE;
                        //                ado.REQ_TYPE_STT_ID = item.REQ_TYPE_STT_ID;
                        //                ado.CREATOR = item.CREATOR;

                        //                ListDataInfoRecord.Add(ado);
                        //            }
                        //        }
                        //    }
                        //}

                        CurrentInfoRecord = ListDataInfoRecord.Where(o => o.DOCUMENT_TYPE_ID == CurrentType.ID).ToList();
                    }


                    var documents = ListDocument.Where(o => o.DOCUMENT_TYPE_ID == CurrentType.ID).ToList();
                    if (documents != null && documents.Count > 0)
                    {
                        InfoRecordADO ado = new InfoRecordADO();
                        ado.TYPE = GetLangValue("Common.Other");
                        ado.DOCUMENT_TYPE_ID = CurrentType.ID;

                        var docs = GetDocumentByInfoRecod(ado);
                        if (docs != null && docs.Count > 0)
                        {
                            CurrentInfoRecord.Add(ado);
                        }
                    }

                    if (chkToiTao.Checked && CurrentInfoRecord != null)
                    {
                        CurrentInfoRecord = CurrentInfoRecord.Where(o => o.CREATOR == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()).ToList();
                    }

                    // QT-09 / QT-10: resolve the document status ONCE per row, before binding.
                    // Doing it inside CustomUnboundColumnData would re-run the whole document
                    // lookup on every cell repaint - see performance.md.
                    ProcessDocumentStatus(CurrentInfoRecord);

                    // QT-11 / QT-12: quick filters read DOC_STATUS, so they run afterwards.
                    CurrentInfoRecord = ApplyQuickFilters(CurrentInfoRecord);

                    Gc_InfoRecord.BeginUpdate();
                    Gc_InfoRecord.DataSource = CurrentInfoRecord;
                    Gc_InfoRecord.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataGridDocument()
        {
            try
            {
                List<V_EMR_DOCUMENT> documents = new List<V_EMR_DOCUMENT>();
                if (ListDocument != null && CurrentType != null)
                {
                    documents = ListDocument.Where(o => (o.DOCUMENT_TYPE_ID ?? 0) == CurrentType.ID).ToList();
                    if (ListTypeId.Contains(CurrentType.ID))
                    {
                        var record = (InfoRecordADO)Gv_InfoRecord.GetFocusedRow();
                        if (LciTotalInfo.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && record != null)
                        {
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => record), record));
                            documents = GetDocumentByInfoRecod(record, CurrentType.ID);
                        }
                    }
                }
                if (chkToiTao.Checked && documents != null)
                {
                    documents = documents.Where(o => o.CREATOR == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()).ToList();
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => CurrentType), CurrentType)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListTypeId), ListTypeId)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => documents), documents));

                List<long> Id = documents.Select(s => s.ID).ToList();
                this.GetDicEmrSign(Id);

                Gc_EmrDocument.BeginUpdate();
                Gc_EmrDocument.DataSource = documents;
                Gc_EmrDocument.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Xoa cache luong ky. Goi khi bat dau MOT lan tra soat moi (doi ho so o Cach 1,
        /// doi trang / doi bo loc o Cach 2) de khong giu lai du lieu cua lan truoc.
        /// </summary>
        private void ResetEmrSignCache()
        {
            try
            {
                dicVEmrSign = new Dictionary<long, V_EMR_SIGN>();
                loadedEmrSignDocumentIds = new HashSet<long>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Nap luong ky cua cac van ban dua vao, gop them vao dicVEmrSign.
        ///
        /// Gop (thay vi dung lai tu dau moi lan goi) de Cach 2 nap san duoc cho CA TRANG
        /// ngay sau khi lay danh sach van ban - nguoi dung thay ten luong ky / nguoi chua ky
        /// ma khong phai click tung dong. Gop an toan vi khoa la V_EMR_SIGN.ID (khoa chinh
        /// cua luong ky), va GetSigners chi tra nhung ma xuat hien trong SIGNERS / UN_SIGNERS
        /// cua dung van ban dang hien thi nen ban ghi thua khong bao gio bi hien nham.
        ///
        /// Van ban da hoi roi thi bo qua, nen click qua lai giua cac dong khong goi lai API.
        /// </summary>
        private void GetDicEmrSign(List<long> lstDocumentId)
        {
            try
            {
                if (lstDocumentId == null || lstDocumentId.Count == 0) return;

                List<long> missing = lstDocumentId
                    .Where(o => !loadedEmrSignDocumentIds.Contains(o))
                    .Distinct()
                    .ToList();

                if (missing.Count == 0) return;

                // Chia lo giong GetMedicineById: menh de IN cua Oracle khong qua 1000 phan tu.
                int skip = 0;
                while (missing.Count - skip > 0)
                {
                    List<long> chunk = missing.Skip(skip).Take(500).ToList();
                    skip += 500;

                    CommonParam paramCommon = new CommonParam();
                    EmrSignViewFilter filter = new EmrSignViewFilter();
                    filter.DOCUMENT_IDs = chunk;

                    var datas = new BackendAdapter(paramCommon).Get<List<V_EMR_SIGN>>(
                        "api/EmrSign/GetView", ApiConsumers.EmrConsumer, filter, paramCommon)
                        ?? new List<V_EMR_SIGN>();

                    foreach (var item in datas)
                    {
                        dicVEmrSign[item.ID] = item;
                    }
                }

                foreach (var id in missing)
                {
                    loadedEmrSignDocumentIds.Add(id);
                }

                Inventec.Common.Logging.LogSystem.Debug(
                    "GetDicEmrSign____ hoi them " + missing.Count + " van ban; tong luong ky da nap = " + dicVEmrSign.Count);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<V_EMR_DOCUMENT> GetDocumentByInfoRecod(InfoRecordADO record, long documentTypeId = 0)
        {
            List<V_EMR_DOCUMENT> result = new List<V_EMR_DOCUMENT>();
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => record), record));
                if (record != null)
                {
                    result = ListDocument.Where(o => (o.DOCUMENT_TYPE_ID ?? 0) == (documentTypeId > 0 ? documentTypeId : record.DOCUMENT_TYPE_ID)).ToList();
                    if (!String.IsNullOrWhiteSpace(record.SEARCH_CODE))
                    {
                        result = result.Where(o => !String.IsNullOrWhiteSpace(o.HIS_CODE) && o.HIS_CODE.Contains(record.SEARCH_CODE)).ToList();
                    }
                    else
                    {
                        List<string> searchCode = CurrentInfoRecord.Select(s => s.SEARCH_CODE).ToList();
                        foreach (var item in searchCode)
                        {
                            if (!String.IsNullOrWhiteSpace(item))
                            {
                                result = result.Where(o => String.IsNullOrWhiteSpace(o.HIS_CODE) || !o.HIS_CODE.Contains(item)).ToList();
                            }
                        }
                    }
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));
                }
            }
            catch (Exception ex)
            {
                result = new List<V_EMR_DOCUMENT>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ProcessCaptionGridInfoRecord()
        {
            try
            {
                if (CurrentType == null) return;

                string code = GetLangValue("GridCaption.Code");
                string type = GetLangValue("GridCaption.Type");
                string time = GetLangValue("GridCaption.Time");
                string depa = GetLangValue("GridCaption.Department");
                string creator = GetLangValue("GridCaption.Creator");
                if (CurrentType.ID == IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_RESULT)
                {
                    code = GetLangValue("GridCaption.ServiceReqCode");
                    time = GetLangValue("GridCaption.IntructionTime");
                    depa = GetLangValue("GridCaption.RequestDepartment");
                    creator = GetLangValue("GridCaption.RequestUser");
                }
                else
                {
                    if (CurrentType.ID == IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_ASSIGN
                        || CurrentType.ID == IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_RESULT
                        || CurrentType.ID == IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__PRESCRIPTION)
                    {
                        code = GetLangValue("GridCaption.ServiceReqCode");
                        time = GetLangValue("GridCaption.IntructionTime");
                        depa = GetLangValue("GridCaption.RequestDepartment");
                        creator = GetLangValue("GridCaption.RequestUser");
                    }
                    else if (CurrentType.ID == IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__INFUSION
                        || CurrentType.ID == IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__MEDI_REACT)
                    {
                        time = GetLangValue("GridCaption.InfusionTime");
                        depa = GetLangValue("GridCaption.Solution");
                    }
                    else if (CurrentType.ID == IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__DEBATE)
                    {
                        time = GetLangValue("GridCaption.DebateTime");
                    }
                }


                Gv_IR_Code.Caption = code;
                Gv_IR_Type.Caption = type;
                Gv_IR_CreateTime.Caption = time;
                Gv_IR_DepartmentName.Caption = depa;
                Gv_IR_Creator.Caption = creator;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
