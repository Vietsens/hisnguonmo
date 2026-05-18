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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute
{
    public partial class SurgServiceReqExecuteControl : UserControlBase
    {
        /// <summary>
        /// Validate time chain between PT (surgery) and child VTYT (material) service requests.
        /// Order: PT.INTRUCTION < VTYT.INTRUCTION < PT.START < VTYT.START < VTYT.FINISH < PT.FINISH
        /// Only validates when VTYT contains material types with IS_REQUIRE_TIME_VALIDATE = 1.
        /// Unconditional required: VTYT.INTRUCTION_TIME, PT.INTRUCTION_TIME.
        /// Conditional fields (VTYT.START/FINISH, PT.START/FINISH): each cross-check rule
        /// fires only when BOTH input fields of that rule are > 0.
        /// </summary>
        private bool ValidateVtytTimeWithParentPT()
        {
            try
            {
                if (this.serviceReq == null)
                    return true;

                long ptInstructionTime = this.serviceReq.INTRUCTION_TIME;
                long ptStartTime = dtStart.EditValue == null
                    ? 0
                    : (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtStart.DateTime) ?? 0);
                long ptFinishTime = dtFinish.EditValue == null
                    ? 0
                    : (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtFinish.DateTime) ?? 0);

                // Get material type IDs that require time validation (from RAM cache, no API call)
                var materialTypeIdsRequireValidate = GetMaterialTypeIdsRequireTimeValidate();
                if (materialTypeIdsRequireValidate == null || materialTypeIdsRequireValidate.Count == 0)
                    return true;

                WaitingManager.Show();

                // Get child VTYT service requests (DONDT / DONTT) linked to this PT
                var childVtytServiceReqs = GetChildVtytServiceReqs();
                if (childVtytServiceReqs == null || childVtytServiceReqs.Count == 0)
                {
                    WaitingManager.Hide();
                    return true;
                }

                // Get material links for all child VTYTs via 2 paths:
                // Path 1: HIS_SERVICE_REQ_MATY (declared consumable materials)
                // Path 2: HIS_EXP_MEST -> HIS_EXP_MEST_MATERIAL (actual exported materials / don trong kho)
                var childServiceReqIds = childVtytServiceReqs.Select(o => o.ID).ToList();
                var serviceReqMatys = GetServiceReqMatys(childServiceReqIds);
                var expMestMaterialTypeIds = GetExpMestMaterialTypeIds(childServiceReqIds);

                WaitingManager.Hide();

                // Filter VTYTs that have materials requiring time validation (from either path)
                var vtytIdsWithValidateMaterial = new HashSet<long>();

                // Path 1: SERVICE_REQ_MATY
                if (serviceReqMatys != null && serviceReqMatys.Count > 0)
                {
                    foreach (var maty in serviceReqMatys)
                    {
                        if (materialTypeIdsRequireValidate.Contains(maty.MATERIAL_TYPE_ID ?? 0))
                        {
                            vtytIdsWithValidateMaterial.Add(maty.SERVICE_REQ_ID);
                        }
                    }
                }

                // Path 2: EXP_MEST_MATERIAL (don trong kho)
                if (expMestMaterialTypeIds != null && expMestMaterialTypeIds.Count > 0)
                {
                    foreach (var kvp in expMestMaterialTypeIds)
                    {
                        if (materialTypeIdsRequireValidate.Contains(kvp.Value))
                        {
                            vtytIdsWithValidateMaterial.Add(kvp.Key);
                        }
                    }
                }

                if (vtytIdsWithValidateMaterial.Count == 0)
                    return true;

                // Validate time chain for each qualifying VTYT
                List<string> errors = new List<string>();
                bool hasStartTimeError = false;
                bool hasFinishTimeError = false;

                foreach (var vtyt in childVtytServiceReqs)
                {
                    if (!vtytIdsWithValidateMaterial.Contains(vtyt.ID))
                        continue;

                    ValidateVtytTimeChain(vtyt, ptInstructionTime, ptStartTime, ptFinishTime,
                        errors, ref hasStartTimeError, ref hasFinishTimeError);
                }

                if (errors.Count > 0)
                {
                    // Show warning icon on PT time controls
                    SetVtytTimeWarningIcon(hasStartTimeError, hasFinishTimeError);

                    string errorMessage = string.Join("\r\n", errors);
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        errorMessage,
                        Resources.ResourceMessage.ThongBao,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return false;
                }

                // Clear warning icons when valid
                ClearVtytTimeWarningIcon();
                return true;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                return true;
            }
        }

        /// <summary>
        /// Validate the 6-point time chain for a single VTYT against its parent PT.
        /// PT.INTRUCTION < VTYT.INTRUCTION < PT.START < VTYT.START < VTYT.FINISH < PT.FINISH
        /// Only checks rules where both values are available (progressive validation).
        /// </summary>
        private void ValidateVtytTimeChain(
            HIS_SERVICE_REQ vtyt,
            long ptInstructionTime,
            long ptStartTime,
            long ptFinishTime,
            List<string> errors,
            ref bool hasStartTimeError,
            ref bool hasFinishTimeError)
        {
            try
            {
                long vtytInstructionTime = vtyt.INTRUCTION_TIME;
                long vtytStartTime = vtyt.START_TIME ?? 0;
                long vtytFinishTime = vtyt.FINISH_TIME ?? 0;

                string vtytCode = vtyt.SERVICE_REQ_CODE ?? vtyt.ID.ToString();

                // Unconditional required: VTYT.INTRUCTION_TIME must be set
                if (vtytInstructionTime <= 0)
                {
                    errors.Add(string.Format(Resources.ResourceMessage.ThoiGianYLenhVTYTKhongDuocDeTrong, vtytCode));
                    return;
                }

                // Cross-checks: validate each rule only when BOTH input fields > 0
                // R2: PT.INTRUCTION < VTYT.INTRUCTION
                if (ptInstructionTime > 0 && vtytInstructionTime <= ptInstructionTime)
                {
                    errors.Add(string.Format(
                        Resources.ResourceMessage.ThoiGianYLenhVTYTPHaiSauThoiGianYLenhPT,
                        vtytCode,
                        FormatTimeDisplay(vtytInstructionTime),
                        FormatTimeDisplay(ptInstructionTime)));
                }

                // R3: VTYT.INTRUCTION < PT.START
                if (ptStartTime > 0 && vtytInstructionTime >= ptStartTime)
                {
                    errors.Add(string.Format(
                        Resources.ResourceMessage.ThoiGianYLenhVTYTPhaiTruocThoiGianBatDauPT,
                        vtytCode,
                        FormatTimeDisplay(vtytInstructionTime),
                        FormatTimeDisplay(ptStartTime)));
                    hasStartTimeError = true;
                }

                // R4: PT.START < VTYT.START
                if (ptStartTime > 0 && vtytStartTime > 0 && vtytStartTime <= ptStartTime)
                {
                    errors.Add(string.Format(
                        Resources.ResourceMessage.ThoiGianBatDauVTYTPhaiSauThoiGianBatDauPT,
                        vtytCode,
                        FormatTimeDisplay(vtytStartTime),
                        FormatTimeDisplay(ptStartTime)));
                    hasStartTimeError = true;
                }

                // R5: VTYT.START < VTYT.FINISH
                if (vtytStartTime > 0 && vtytFinishTime > 0 && vtytFinishTime <= vtytStartTime)
                {
                    errors.Add(string.Format(
                        Resources.ResourceMessage.ThoiGianKetThucVTYTPhaiSauThoiGianBatDauVTYT,
                        vtytCode,
                        FormatTimeDisplay(vtytFinishTime),
                        FormatTimeDisplay(vtytStartTime)));
                }

                // R6: VTYT.FINISH < PT.FINISH
                if (vtytFinishTime > 0 && ptFinishTime > 0 && vtytFinishTime >= ptFinishTime)
                {
                    errors.Add(string.Format(
                        Resources.ResourceMessage.ThoiGianKetThucVTYTPhaiTruocThoiGianKetThucPT,
                        vtytCode,
                        FormatTimeDisplay(vtytFinishTime),
                        FormatTimeDisplay(ptFinishTime)));
                    hasFinishTimeError = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Show warning icon on dtStart / dtFinish when VTYT time validation fails.
        /// </summary>
        private void SetVtytTimeWarningIcon(bool hasStartTimeError, bool hasFinishTimeError)
        {
            try
            {
                if (hasStartTimeError)
                {
                    this.dxErrorProviver.SetError(dtStart,
                        Resources.ResourceMessage.ThoiGianBatDauPTKhongHopLeVoiVTYTCon,
                        ErrorType.Warning);
                }
                if (hasFinishTimeError)
                {
                    this.dxErrorProviver.SetError(dtFinish,
                        Resources.ResourceMessage.ThoiGianKetThucPTKhongHopLeVoiVTYTCon,
                        ErrorType.Warning);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Clear VTYT time warning icons on dtStart / dtFinish.
        /// </summary>
        private void ClearVtytTimeWarningIcon()
        {
            try
            {
                this.dxErrorProviver.SetError(dtStart, "", ErrorType.None);
                this.dxErrorProviver.SetError(dtFinish, "", ErrorType.None);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Get material type IDs that have IS_REQUIRE_TIME_VALIDATE = 1.
        /// Uses reflection to safely access the new field (backward compatible).
        /// </summary>
        private HashSet<long> GetMaterialTypeIdsRequireTimeValidate()
        {
            var result = new HashSet<long>();
            try
            {
                var allMaterialTypes = BackendDataWorker.Get<HIS_MATERIAL_TYPE>();
                if (allMaterialTypes == null || allMaterialTypes.Count == 0)
                    return result;

                // Use reflection to access IS_REQUIRE_TIME_VALIDATE (field may not exist in older EFMODEL)
                var propertyInfo = typeof(HIS_MATERIAL_TYPE).GetProperty("IS_REQUIRE_TIME_VALIDATE");
                if (propertyInfo == null)
                    return result;

                foreach (var mt in allMaterialTypes)
                {
                    var value = propertyInfo.GetValue(mt, null);
                    if (value != null)
                    {
                        short shortVal = 0;
                        if (value is short)
                            shortVal = (short)value;
                        else if (value is Nullable<short>)
                            shortVal = ((Nullable<short>)value).GetValueOrDefault();

                        if (shortVal == 1)
                        {
                            result.Add(mt.ID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Get child VTYT service requests linked to the current PT via PARENT_ID.
        /// Only returns DONDT (don dieu tri) and DONTT (don tu truc) types that are not deleted.
        /// </summary>
        private List<HIS_SERVICE_REQ> GetChildVtytServiceReqs()
        {
            List<HIS_SERVICE_REQ> result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisServiceReqFilter filter = new HisServiceReqFilter();
                filter.PARENT_ID = this.serviceReq.ID;

                var childServiceReqs = new BackendAdapter(param)
                    .Get<List<HIS_SERVICE_REQ>>(
                        RequestUriStore.HIS_SERVICE_REQ_GET,
                        ApiConsumers.MosConsumer,
                        filter,
                        param);

                if (childServiceReqs != null && childServiceReqs.Count > 0)
                {
                    long donDtId = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT;
                    long donTtId = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT;

                    result = childServiceReqs.Where(o =>
                        (o.SERVICE_REQ_TYPE_ID == donDtId || o.SERVICE_REQ_TYPE_ID == donTtId)
                        && o.IS_DELETE != IMSys.DbConfig.HIS_RS.COMMON.IS_DELETE__TRUE
                    ).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Get HIS_SERVICE_REQ_MATY records for the given service request IDs.
        /// Links service requests to their material types.
        /// </summary>
        private List<HIS_SERVICE_REQ_MATY> GetServiceReqMatys(List<long> serviceReqIds)
        {
            List<HIS_SERVICE_REQ_MATY> result = null;
            try
            {
                if (serviceReqIds == null || serviceReqIds.Count == 0)
                    return result;

                CommonParam param = new CommonParam();
                HisServiceReqMatyFilter filter = new HisServiceReqMatyFilter();
                filter.SERVICE_REQ_IDs = serviceReqIds;
                result = new BackendAdapter(param)
                    .Get<List<HIS_SERVICE_REQ_MATY>>(
                        RequestUriStore.HIS_SERVICE_REQ_MATY_GET,
                        ApiConsumers.MosConsumer,
                        filter,
                        param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Get material type IDs from EXP_MEST_MATERIAL (don trong kho) for given service request IDs.
        /// Returns list of (ServiceReqId, MaterialTypeId) pairs.
        /// Chain: HIS_SERVICE_REQ -> HIS_EXP_MEST (SERVICE_REQ_ID) -> HIS_EXP_MEST_MATERIAL (EXP_MEST_ID)
        /// </summary>
        private List<KeyValuePair<long, long>> GetExpMestMaterialTypeIds(List<long> serviceReqIds)
        {
            var result = new List<KeyValuePair<long, long>>();
            try
            {
                if (serviceReqIds == null || serviceReqIds.Count == 0)
                    return result;

                // Step 1: Get EXP_MEST linked to child VTYT service requests
                CommonParam param = new CommonParam();
                HisExpMestFilter expMestFilter = new HisExpMestFilter();
                expMestFilter.SERVICE_REQ_IDs = serviceReqIds;
                var expMests = new BackendAdapter(param)
                    .Get<List<HIS_EXP_MEST>>(
                        RequestUriStore.HIS_EXP_MEST_GET,
                        ApiConsumers.MosConsumer,
                        expMestFilter,
                        param);

                if (expMests == null || expMests.Count == 0)
                    return result;

                // Build lookup: ExpMestId -> ServiceReqId
                var expMestToServiceReq = new Dictionary<long, long>();
                foreach (var em in expMests)
                {
                    if (em.SERVICE_REQ_ID.HasValue && !expMestToServiceReq.ContainsKey(em.ID))
                    {
                        expMestToServiceReq[em.ID] = em.SERVICE_REQ_ID.Value;
                    }
                }

                // Step 2: Get EXP_MEST_MATERIAL for those EXP_MESTs
                var expMestIds = expMests.Select(o => o.ID).ToList();
                HisExpMestMaterialFilter materialFilter = new HisExpMestMaterialFilter();
                materialFilter.EXP_MEST_IDs = expMestIds;
                var expMestMaterials = new BackendAdapter(param)
                    .Get<List<HIS_EXP_MEST_MATERIAL>>(
                        RequestUriStore.HIS_EXP_MEST_MATERIAL_GET,
                        ApiConsumers.MosConsumer,
                        materialFilter,
                        param);

                if (expMestMaterials == null || expMestMaterials.Count == 0)
                    return result;

                // Step 3: Map back to (ServiceReqId, MaterialTypeId)
                foreach (var mat in expMestMaterials)
                {
                    long serviceReqId;
                    if (mat.TDL_MATERIAL_TYPE_ID > 0
                        && expMestToServiceReq.TryGetValue(mat.EXP_MEST_ID ?? 0, out serviceReqId))
                    {
                        result.Add(new KeyValuePair<long, long>(serviceReqId, mat.TDL_MATERIAL_TYPE_ID ?? 0));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Format a time number (yyyyMMddHHmmss) to display string.
        /// </summary>
        private string FormatTimeDisplay(long timeNumber)
        {
            try
            {
                return Inventec.Common.DateTime.Convert.TimeNumberToTimeString(timeNumber);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return timeNumber.ToString();
            }
        }
    }
}
