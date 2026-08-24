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
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Plugins.HisTreatmentRecordChecking.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.HisTreatmentRecordChecking.RecordChecking
{
    /// <summary>
    /// Task 53180 - mode 2: list every order of one doctor across records.
    /// Mode 1 (review a single record by code) is untouched and lives in __FillGrid.cs.
    /// </summary>
    public partial class FormHisTreatmentRecordChecking
    {
        #region Paging state

        /// <summary>Rows returned by the current page.</summary>
        private int rowCount = 0;
        /// <summary>Total rows matching the filter, taken from the API result.</summary>
        private int dataTotal = 0;
        /// <summary>Index of the first row of the current page.</summary>
        private int startPage = 0;

        #endregion

        /// <summary>
        /// Entry point of mode 2: validates, then loads the FIRST page and wires the pager.
        /// Paging itself is handled by LoadPagingByDoctor, the delegate given to ucPaging.
        /// </summary>
        private void FillDataToGridByDoctor()
        {
            try
            {
                if (!ValidateFilterByDoctor()) return;

                int pageSize = GetPageSize();

                // LoadPagingByDoctor tu quan ly WaitingManager, giong FillDataToGridByTreatment,
                // nen moi lan doi trang deu co loading chu khong chi lan tim dau tien.
                LoadPagingByDoctor(new CommonParam(0, pageSize));

                lciPaging.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                CommonParam pagingParam = new CommonParam();
                pagingParam.Limit = rowCount;
                pagingParam.Count = dataTotal;
                ucPaging.Init(LoadPagingByDoctor, pagingParam, pageSize, this.Gc_InfoRecord);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Page size from the pager, falling back to the per user configuration.</summary>
        private int GetPageSize()
        {
            try
            {
                if (ucPaging.pagingGrid != null && ucPaging.pagingGrid.PageSize > 0)
                {
                    return ucPaging.pagingGrid.PageSize;
                }
                return HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return 50;
        }

        /// <summary>
        /// Loads one page of orders of the selected doctor (QT-01 .. QT-06).
        /// Signature required by ucPaging.Init.
        /// </summary>
        private void LoadPagingByDoctor(object param)
        {
            try
            {
                // Cung dieu kien vao nhu FillDataToGridByTreatment: chua co loai van ban thi khong lam gi,
                // chua chon loai nao thi lay dong dau va lam moi caption luoi giua.
                if (CurrentType == null && ListDocumentType == null) return;
                if (CurrentType == null && ListDocumentType.Count > 0)
                {
                    CurrentType = ListDocumentType.First();
                    ProcessCaptionGridInfoRecord();
                }

                WaitingManager.Show();

                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);

                HisServiceReqForRecordCheckingFilter filter = new HisServiceReqForRecordCheckingFilter();
                filter.REQUEST_LOGINNAME = GetSelectedDoctorLoginName();
                filter.FROM_TIME = GetFilterFromTime();
                filter.TO_TIME = GetFilterToTime();
                filter.IS_END_TREATMENT = IsFilterFinishedTreatment();
                // QT-07 (loc theo loai van ban dang chon o luoi trai) xu ly phia client trong
                // ProcessFillDataToGrid(), giong het Cach 1 - filter cua API khong co DOCUMENT_TYPE_ID.
                filter.ORDER_FIELD = "CREATE_TIME";
                filter.ORDER_DIRECTION = "DESC";

                Inventec.Common.Logging.LogSystem.Debug("LoadPagingByDoctor INPUT____"
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));

                // API tra ve du lieu THO cua tung bang, giong api/HisTreatment/GetInfoForRecordChecking,
                // chi khac la pham vi NHIEU ho so nen Treatment thanh Treatments va co them 3 bang _SUM.
                var apiResult = new BackendAdapter(paramCommon).GetRO<HisServiceReqForRecordCheckingSDO>(
                    RecordCheckingUriStore.MOSHIS_HIS_TREATMENT_GET_SERVICE_REQ_FOR_RECORD_CHECKING,
                    ApiConsumers.MosConsumer, filter, paramCommon);

                HisServiceReqForRecordCheckingSDO pageData = (apiResult != null ? apiResult.Data : null);

                // Tach du lieu phang thanh tung ho so, moi ho so mot SDO giong het Cach 1,
                // nho vay dung lai nguyen ProcessDataADO() ma khong can xu ly du lieu rieng.
                List<HisTreatmentForRecordCheckingSDO> listSdo = SplitByTreatment(pageData);

                rowCount = listSdo.Count;
                dataTotal = (apiResult == null || apiResult.Param == null) ? 0 : (apiResult.Param.Count ?? 0);

                // Reset giong het FillDataToGridByTreatment truoc khi dung lai du lieu.
                ListDataInfoRecord = new List<InfoRecordADO>();
                CurrentDataInfoRecord = new List<InfoRecordADO>();

                // Dung san 3 bang tra cuu cho CA TRANG: thuoc chi 1 lot goi API, 2 danh muc chi duyet 1 lan.
                // Neu de trong ProcessDataADO thi moi ho so lai lam lai tu dau.
                PrepareLookupData(listSdo);

                foreach (var sdo in listSdo)
                {
                    ProcessDataADO(sdo);
                }

                // Thay cho EmrDocument() cua Cach 1: van ban cua ca trang, chi 1 lan goi API,
                // sau do chuan hoa dong "Chua xac dinh" y het Cach 1.
                ListDocument = LoadDocumentsOfTreatments(
                    listSdo.Where(o => o.Treatment != null
                                    && !string.IsNullOrWhiteSpace(o.Treatment.TREATMENT_CODE))
                           .Select(o => o.Treatment.TREATMENT_CODE)
                           .Distinct()
                           .ToList());

                // Trang moi -> bo luong ky cua trang truoc, roi nap san luong ky cho TOAN BO
                // van ban cua trang. Cach 1 chi nap khi click tung dong (ProcessDataGridDocument);
                // o day nhieu ho so nen nap truoc mot lot, nguoi dung thay ngay ten luong ky /
                // nguoi chua ky ma khong phai click qua tung y lenh.
                ResetEmrSignCache();
                GetDicEmrSign(ListDocument.Select(o => o.ID).Distinct().ToList());

                SyncUndefinedDocumentType();

                // Tu day tro di du lieu co hinh dang y het Cach 1 (ListDataInfoRecord + ListDocument),
                // nen chay DUNG chuoi xu ly cua Cach 1: loc theo loai VB, "Toi tao", trang thai van ban,
                // 2 o loc nhanh, sap xep va to mau luoi trai, bind luoi.
                RefreshDocumentTypeFlags();
                OrderListByCheckBox();
                ProcessFillDataToGrid();

                Inventec.Common.Logging.LogSystem.Debug(
                    "LoadPagingByDoctor RESULT____start=" + startPage
                    + "; pageRows=" + rowCount
                    + "; totalRows=" + dataTotal
                    + "; shown=" + (CurrentInfoRecord != null ? CurrentInfoRecord.Count : 0)
                    + "; documents=" + (ListDocument != null ? ListDocument.Count : 0));

                SessionManager.ProcessTokenLost(paramCommon);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Tach du lieu phang cua Cach 2 thanh danh sach SDO theo tung ho so,
        /// moi phan tu co hinh dang y het SDO cua Cach 1 (api/HisTreatment/GetInfoForRecordChecking).
        ///
        /// 4 bang HIS_SERVICE_REQ / HIS_TRACKING / HIS_CARE / HIS_DEBATE co san TREATMENT_ID nen nhom truc tiep.
        /// 3 bang HIS_INFUSION / HIS_MEDI_REACT / HIS_TRANSFUSION khong co TREATMENT_ID,
        /// phai qua bang _SUM tuong ung (INFUSION_SUM_ID / MEDI_REACT_SUM_ID / TRANSFUSION_SUM_ID -> _SUM.ID -> TREATMENT_ID).
        ///
        /// Toan bo dung ToLookup / Dictionary de tra cuu O(1), khong duyet long nhau.
        /// </summary>
        private List<HisTreatmentForRecordCheckingSDO> SplitByTreatment(HisServiceReqForRecordCheckingSDO data)
        {
            List<HisTreatmentForRecordCheckingSDO> result = new List<HisTreatmentForRecordCheckingSDO>();
            try
            {
                if (data == null || data.Treatments == null || data.Treatments.Count == 0) return result;

                // Nhom truc tiep theo TREATMENT_ID.
                ILookup<long, HIS_SERVICE_REQ> lkServiceReq =
                    (data.ServiceReqs ?? new List<HIS_SERVICE_REQ>()).ToLookup(o => o.TREATMENT_ID);
                ILookup<long, HIS_TRACKING> lkTracking =
                    (data.Trackings ?? new List<HIS_TRACKING>()).ToLookup(o => o.TREATMENT_ID);
                ILookup<long, HIS_CARE> lkCare =
                    (data.Cares ?? new List<HIS_CARE>()).ToLookup(o => o.TREATMENT_ID);
                ILookup<long, HIS_DEBATE> lkDebate =
                    (data.Debates ?? new List<HIS_DEBATE>()).ToLookup(o => o.TREATMENT_ID);

                // Nhom qua bang _SUM: dung _SUM.ID de suy ra TREATMENT_ID cua dong chi tiet.
                Dictionary<long, long> dicInfusionSum = BuildSumLookup(data.InfusionSums, o => o.ID, o => o.TREATMENT_ID);
                Dictionary<long, long> dicMediReactSum = BuildSumLookup(data.MediReactSums, o => o.ID, o => o.TREATMENT_ID);
                Dictionary<long, long> dicTransfusionSum = BuildSumLookup(data.TransfusionSums, o => o.ID, o => o.TREATMENT_ID);

                ILookup<long, HIS_INFUSION> lkInfusion = GroupBySum(data.Infusions, dicInfusionSum, o => o.INFUSION_SUM_ID);
                ILookup<long, HIS_MEDI_REACT> lkMediReact = GroupBySum(data.MediReacts, dicMediReactSum, o => o.MEDI_REACT_SUM_ID);
                ILookup<long, HIS_TRANSFUSION> lkTransfusion = GroupBySum(data.Transfusions, dicTransfusionSum, o => o.TRANSFUSION_SUM_ID);

                foreach (var treatment in data.Treatments)
                {
                    HisTreatmentForRecordCheckingSDO sdo = new HisTreatmentForRecordCheckingSDO();
                    sdo.Treatment = treatment;
                    sdo.ServiceReqs = lkServiceReq[treatment.ID].ToList();
                    sdo.Trackings = lkTracking[treatment.ID].ToList();
                    sdo.Cares = lkCare[treatment.ID].ToList();
                    sdo.Debates = lkDebate[treatment.ID].ToList();
                    sdo.Infusions = lkInfusion[treatment.ID].ToList();
                    sdo.MediReacts = lkMediReact[treatment.ID].ToList();
                    sdo.Transfusions = lkTransfusion[treatment.ID].ToList();
                    result.Add(sdo);
                }
            }
            catch (Exception ex)
            {
                result = new List<HisTreatmentForRecordCheckingSDO>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>Dung dictionary _SUM.ID -> TREATMENT_ID.</summary>
        private Dictionary<long, long> BuildSumLookup<T>(
            List<T> sums, Func<T, long> getId, Func<T, long> getTreatmentId)
        {
            Dictionary<long, long> result = new Dictionary<long, long>();
            try
            {
                if (sums == null) return result;
                foreach (var item in sums)
                {
                    result[getId(item)] = getTreatmentId(item);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Nhom cac dong chi tiet theo TREATMENT_ID suy ra tu bang _SUM.
        /// Dong nao khong tim thay _SUM tuong ung se bi bo qua (khong biet thuoc ho so nao).
        /// </summary>
        private ILookup<long, T> GroupBySum<T>(
            List<T> items, Dictionary<long, long> dicSum, Func<T, long> getSumId)
        {
            try
            {
                if (items == null) return new List<T>().ToLookup(o => 0L);

                return items.Where(o => dicSum.ContainsKey(getSumId(o)))
                            .ToLookup(o => dicSum[getSumId(o)]);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return new List<T>().ToLookup(o => 0L);
        }

        /// <summary>
        /// Loads every EMR document of the given records in a single call,
        /// so the status of all rows can be resolved without a per-row request.
        ///
        /// Loc theo TREATMENT_CODEs, KHONG dung TREATMENT_IDs: tren V_EMR_DOCUMENT cot
        /// TREATMENT_ID cho phep NULL, va bo loc TREATMENT_IDs cua EMR yeu cau
        /// o.TREATMENT_ID.HasValue nen bo qua moi van ban chua duoc gan TREATMENT_ID -
        /// dung nhom van ban "chua ky". Cach 1 loc bang TREATMENT_CODE__EXACT nen van thay
        /// nhung van ban do; dung TREATMENT_CODEs de hai cach khop nhau tung dong.
        /// </summary>
        private List<V_EMR_DOCUMENT> LoadDocumentsOfTreatments(List<string> treatmentCodes)
        {
            List<V_EMR_DOCUMENT> result = new List<V_EMR_DOCUMENT>();
            try
            {
                if (treatmentCodes == null || treatmentCodes.Count == 0) return result;

                CommonParam paramCommon = new CommonParam();
                EmrDocumentViewFilter filter = new EmrDocumentViewFilter();
                filter.TREATMENT_CODEs = treatmentCodes;
                // Same rule as EmrDocument() in mode 1.
                if (!chkIncludeCancelDoc.Checked) filter.IS_DELETE = false;

                result = new BackendAdapter(paramCommon).Get<List<V_EMR_DOCUMENT>>(
                    RecordCheckingUriStore.EMR_DOCUMENT_GETVIEW,
                    ApiConsumers.EmrConsumer, filter, SessionManager.ActionLostToken, paramCommon);

                if (result == null) result = new List<V_EMR_DOCUMENT>();
            }
            catch (Exception ex)
            {
                result = new List<V_EMR_DOCUMENT>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Opens the record of the focused row in mode 1 (double click).
        /// </summary>
        private void OpenTreatmentOfFocusedOrder()
        {
            try
            {
                InfoRecordADO row = Gv_InfoRecord.GetFocusedRow() as InfoRecordADO;
                if (row == null || string.IsNullOrWhiteSpace(row.TREATMENT_CODE)) return;

                cboRequestDoctor.EditValue = null;    // back to mode 1
                treatmentId = row.TREATMENT_ID;
                TxtTreatmentCode.Text = row.TREATMENT_CODE;

                ApplyModeUI();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
