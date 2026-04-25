using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using EMR.Desktop.Plugins.EmrExamCategory.ADO;

namespace EMR.Desktop.Plugins.EmrExamCategory
{
    public partial class EmrExamCategoryForm
    {
        #region Save All
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveAllChanges();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SaveAllChanges();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SaveAllChanges()
        {
            CommonParam param = new CommonParam();
            try
            {
                gridViewCat.CloseEditor();
                gridViewCat.UpdateCurrentRow();
                gridViewRule.CloseEditor();
                gridViewRule.UpdateCurrentRow();

                if (!ValidateBeforeSave())
                    return;

                // SaveAll chỉ xử lý NEW + UPDATED. DELETE dùng API riêng lẻ ngay khi bấm icon X.
                bool hasChanges = listCategory.Any(c => c.ROW_STATE == RowState.NEW || c.ROW_STATE == RowState.UPDATED)
                    || listRule.Any(r => r.ROW_STATE == RowState.NEW || r.ROW_STATE == RowState.UPDATED);
                if (!hasChanges)
                    return;

                WaitingManager.Show();

                // Build payload theo tài liệu spec — dùng ROW_STATE + temp ID âm.
                // NEW category: ID = temp âm (-1, -2...) để rule tham chiếu trước khi save.
                // BE map lại EXAM_CATEGORY_ID (âm) → ID thực sau create.
                // GROUP_CODE do BE tự gán từ user session (không cần FE truyền).
                var saveAllSDO = new EmrExamCategorySaveAllSDO();

                saveAllSDO.Categories = listCategory
                    .Where(c => c.ROW_STATE == RowState.NEW || c.ROW_STATE == RowState.UPDATED)
                    .Select(c => new EmrExamCategoryTDO
                    {
                        ID = c.ID,
                        CATEGORY_CODE = c.CATEGORY_CODE,
                        CATEGORY_NAME = c.CATEGORY_NAME,
                        NUM_ORDER = c.NUM_ORDER,
                        ROW_STATE = c.ROW_STATE.ToString()
                    }).ToList();

                saveAllSDO.Rules = listRule
                    .Where(r => r.ROW_STATE == RowState.NEW || r.ROW_STATE == RowState.UPDATED)
                    .Select(r => new EmrDocumentPairRuleTDO
                    {
                        ID = r.ID,
                        PATTERN = r.PATTERN,
                        MATCH_TYPE = r.MATCH_TYPE,
                        KEY_EXTRACTOR = r.KEY_EXTRACTOR,
                        EXAM_CATEGORY_ID = r.EXAM_CATEGORY_ID,
                        NUM_ORDER = r.NUM_ORDER,
                        ROW_STATE = r.ROW_STATE.ToString()
                    }).ToList();

                LogSystem.Debug(
                    Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => saveAllSDO), saveAllSDO));

                var result = new BackendAdapter(param).Post<EmrExamCategorySaveAllResultSDO>(
                    EmrRequestUriStore.EMR_EXAM_CATEGORY_SAVE_ALL,
                    ApiConsumers.EmrConsumer,
                    saveAllSDO,
                    param);

                WaitingManager.Hide();

                // Log BE response to diagnose whether BE actually persisted items.
                LogSystem.Debug(
                    "SaveAll RESPONSE: " +
                    Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));

                bool success = (result != null);
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                if (success)
                {
                    tempCategoryIdCounter = -1;
                    LoadAllCategories();
                    LoadAllRules();

                    Inventec.Common.Logging.LogUtil.LogActionSuccess(
                        "EmrExamCategoryForm", "SaveAll", loginName);

                    if (this.delegateSelect != null)
                        this.delegateSelect(null);
                }
                else
                {
                    // Log BE error details to diagnose EMR002 root cause
                    string msgs = (param.Messages != null && param.Messages.Count > 0)
                        ? string.Join(" | ", param.Messages) : "(no messages)";
                    string bugs = (param.BugCodes != null && param.BugCodes.Count > 0)
                        ? string.Join(" | ", param.BugCodes) : "(no bugcodes)";
                    LogSystem.Error(
                        "SaveAll FAILED. BE Messages: " + msgs + " | BE BugCodes: " + bugs
                        + Inventec.Common.Logging.LogUtil.TraceData(
                            Inventec.Common.Logging.LogUtil.GetMemberName(() => saveAllSDO), saveAllSDO));

                    Inventec.Common.Logging.LogUtil.LogActionFail(
                        "EmrExamCategoryForm", "SaveAll", loginName);
                }

                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Warn(ex);
            }
        }

        private static readonly string[] VALID_MATCH_TYPES = { "PREFIX", "CONTAINS", "REGEX" };

        // KEY_EXTRACTOR format per BE spec: seg[N] or seg[N]+seg[M] (e.g. "seg[2]", "seg[1]+seg[3]")
        private static readonly System.Text.RegularExpressions.Regex KEY_EXTRACTOR_PATTERN =
            new System.Text.RegularExpressions.Regex(@"^seg\[\d+\](\+seg\[\d+\])*$");

        private bool ValidateBeforeSave()
        {
            // Validate categories (NEW + UPDATED)
            var activeCats = listCategory.Where(c => c.ROW_STATE != RowState.DELETED).ToList();
            var changedCats = activeCats.Where(c => c.ROW_STATE == RowState.NEW || c.ROW_STATE == RowState.UPDATED).ToList();
            int totalActiveCat = activeCats.Count;
            foreach (var item in changedCats)
            {
                if (string.IsNullOrWhiteSpace(item.CATEGORY_CODE))
                {
                    ShowWarning(Resource.ResourceMessage.CategoryCodeRequired);
                    return false;
                }
                if (string.IsNullOrWhiteSpace(item.CATEGORY_NAME))
                {
                    ShowWarning(Resource.ResourceMessage.CategoryNameRequired);
                    return false;
                }
                // Spec: NUM_ORDER >= 1, <= tổng items active (không tính DELETED)
                if (item.NUM_ORDER < 1 || item.NUM_ORDER > totalActiveCat)
                {
                    ShowWarning(Resource.ResourceMessage.CategoryNumOrderInvalid(totalActiveCat));
                    return false;
                }
            }

            // Check duplicate CATEGORY_CODE among active items only (exclude DELETED)
            var duplicateCodes = activeCats
                .GroupBy(c => (c.CATEGORY_CODE ?? "").Trim().ToUpper())
                .Where(g => g.Key.Length > 0 && g.Count() > 1)
                .Select(g => g.Key)
                .ToList();
            if (duplicateCodes.Any())
            {
                ShowWarning(Resource.ResourceMessage.CategoryCodeDuplicated(string.Join(", ", duplicateCodes)));
                return false;
            }

            // Validate rules (NEW + UPDATED)
            var activeRules = listRule.Where(r => r.ROW_STATE != RowState.DELETED).ToList();
            var changedRules = activeRules.Where(r => r.ROW_STATE == RowState.NEW || r.ROW_STATE == RowState.UPDATED).ToList();
            foreach (var item in changedRules)
            {
                if (string.IsNullOrWhiteSpace(item.PATTERN))
                {
                    ShowWarning(Resource.ResourceMessage.RulePatternRequired);
                    return false;
                }
                if (string.IsNullOrWhiteSpace(item.MATCH_TYPE) || !VALID_MATCH_TYPES.Contains(item.MATCH_TYPE))
                {
                    ShowWarning(Resource.ResourceMessage.RuleMatchTypeInvalid);
                    return false;
                }
                // Validate REGEX pattern compiles successfully
                if (item.MATCH_TYPE == "REGEX")
                {
                    try
                    {
                        new System.Text.RegularExpressions.Regex(item.PATTERN);
                    }
                    catch
                    {
                        ShowWarning(Resource.ResourceMessage.RuleRegexInvalid(item.PATTERN));
                        return false;
                    }
                }
                if (string.IsNullOrWhiteSpace(item.KEY_EXTRACTOR))
                {
                    ShowWarning(Resource.ResourceMessage.RuleKeyExtractorRequired);
                    return false;
                }
                if (!KEY_EXTRACTOR_PATTERN.IsMatch(item.KEY_EXTRACTOR.Trim()))
                {
                    ShowWarning(Resource.ResourceMessage.RuleKeyExtractorInvalid(item.KEY_EXTRACTOR));
                    return false;
                }
                if (item.EXAM_CATEGORY_ID == null)
                {
                    ShowWarning(Resource.ResourceMessage.RuleExamCategoryRequired);
                    return false;
                }
                // Spec: NUM_ORDER >= 1
                if (item.NUM_ORDER < 1)
                {
                    ShowWarning(Resource.ResourceMessage.RuleNumOrderInvalid);
                    return false;
                }
            }

            // Check duplicate NUM_ORDER within same EXAM_CATEGORY_ID (active rules only)
            var duplicateOrders = activeRules
                .GroupBy(r => new { r.EXAM_CATEGORY_ID, r.NUM_ORDER })
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();
            if (duplicateOrders.Any())
            {
                var catName = activeCats.FirstOrDefault(c => c.ID == duplicateOrders[0].EXAM_CATEGORY_ID);
                ShowWarning(Resource.ResourceMessage.RuleNumOrderDuplicated(catName != null ? catName.CATEGORY_NAME : ""));
                return false;
            }

            return true;
        }

        private static void ShowWarning(string message)
        {
            MessageBox.Show(
                message,
                MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
        #endregion

        #region Refresh
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                RefreshAll();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void bbtnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                RefreshAll();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void RefreshAll()
        {
            try
            {
                tempCategoryIdCounter = -1;
                LoadAllCategories();
                LoadAllRules();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        #endregion
    }
}
