/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * Tao du tru 2: "Thong tin" filter (Tat ca / Day du thong tin BHYT / Thieu thong tin BHYT).
 * - The selected value is applied on each search (ShowUCControl): the type ids matching IS_MISS_BHYT_INFO
 *   are loaded from the type catalog and kept per item kind.
 * - Picking a value searches right away (same as Tim); Lam lai resets it to Tat ca without searching.
 * - BindPivotTree keeps only leaf type rows whose type id is in that set (plus their ancestor groups).
 */
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout.Utils;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.AnticipateCreateV2
{
    public partial class UCAnticipateCreateV2
    {
        #region ---BHYT info filter fields
        internal DevExpress.XtraEditors.ImageComboBoxEdit cboBhytInfo;
        internal DevExpress.XtraLayout.LayoutControlItem lciBhytInfo;

        // Type ids allowed at the latest search, one set per item kind. null = no filter (Tat ca).
        internal HashSet<long> allowedMedicineTypeIdsByBhyt = null;
        internal HashSet<long> allowedMaterialTypeIdsByBhyt = null;
        #endregion

        /// <summary>
        /// Creates the "Thong tin" box on the top filter row, between the keyword box and Tu ngay.
        /// It is inserted right of the stretchable keyword item (splitting a fixed-size item such as Tu ngay
        /// makes DevExpress open a new row); fallback: left of Tu ngay when the keyword box was not created.
        /// Called from InitImpExpFilter. The value is not saved to ControlState: it is Tat ca on every open.
        /// </summary>
        private void InitBhytInfoFilter(DevExpress.XtraLayout.LayoutControlItem searchItem, DevExpress.XtraLayout.LayoutControlItem fromDateItem)
        {
            try
            {
                if (this.layoutControl3 == null || (searchItem == null && fromDateItem == null)) return;

                cboBhytInfo = new DevExpress.XtraEditors.ImageComboBoxEdit();
                cboBhytInfo.Name = "cboBhytInfo";
                cboBhytInfo.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
                cboBhytInfo.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
                cboBhytInfo.Properties.Items.Add(new ImageComboBoxItem(GetBhytInfoText("UCAnticipateCreateV2.cboBhytInfo.All"), EnumBhytInfoFilter.All, -1));
                cboBhytInfo.Properties.Items.Add(new ImageComboBoxItem(GetBhytInfoText("UCAnticipateCreateV2.cboBhytInfo.Full"), EnumBhytInfoFilter.Full, -1));
                cboBhytInfo.Properties.Items.Add(new ImageComboBoxItem(GetBhytInfoText("UCAnticipateCreateV2.cboBhytInfo.Missing"), EnumBhytInfoFilter.Missing, -1));
                cboBhytInfo.EditValue = EnumBhytInfoFilter.All;
                cboBhytInfo.EditValueChanged += cboBhytInfo_EditValueChanged;

                this.layoutControl3.BeginUpdate();
                try
                {
                    string caption = GetBhytInfoText("UCAnticipateCreateV2.lciBhytInfo.Text");
                    lciBhytInfo = searchItem != null
                        ? this.layoutControl3.AddItem(caption, cboBhytInfo, searchItem, InsertType.Right)
                        : this.layoutControl3.AddItem(caption, cboBhytInfo, fromDateItem, InsertType.Left);
                    lciBhytInfo.Name = "lciBhytInfo";
                    lciBhytInfo.AppearanceItemCaption.Options.UseTextOptions = true;
                    lciBhytInfo.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    lciBhytInfo.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
                    lciBhytInfo.TextSize = new System.Drawing.Size(60, 20);
                    lciBhytInfo.MaxSize = new System.Drawing.Size(220, 26);
                    lciBhytInfo.MinSize = new System.Drawing.Size(220, 26);
                    lciBhytInfo.Size = new System.Drawing.Size(220, 26);
                    lciBhytInfo.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
                    lciBhytInfo.OptionsToolTip.ToolTip = GetBhytInfoText("UCAnticipateCreateV2.lciBhytInfo.OptionsToolTip.ToolTip");
                }
                finally
                {
                    this.layoutControl3.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetBhytInfoText(string key)
        {
            return Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        }

        /// <summary>Selected value of the "Thong tin" box; Tat ca when the box is missing or empty.</summary>
        private EnumBhytInfoFilter CurrentBhytInfoFilter
        {
            get
            {
                if (cboBhytInfo == null || !(cboBhytInfo.EditValue is EnumBhytInfoFilter)) return EnumBhytInfoFilter.All;
                return (EnumBhytInfoFilter)cboBhytInfo.EditValue;
            }
        }

        /// <summary>The box only applies to medicine and material; it is disabled (value kept) for blood.</summary>
        private void UpdateBhytInfoEnabled()
        {
            try
            {
                if (cboBhytInfo != null) cboBhytInfo.Enabled = !IsBloodChecked;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Picking a value reloads the list at once, like Tim. Skipped while the screen opens and during Lam lai
        /// (both run with formLoaded = false) and for blood (the box is disabled then).
        /// Typed SL du tru / Ghi chu are kept: they live in the per-type dictionaries, cleared only by Moi.
        /// </summary>
        private void cboBhytInfo_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!formLoaded || IsBloodChecked) return;
                ShowUCControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Lam lai: back to Tat ca and drop the stored type id sets (no search: formLoaded is false here).</summary>
        private void ResetBhytInfoFilter()
        {
            try
            {
                if (cboBhytInfo != null) cboBhytInfo.EditValue = EnumBhytInfoFilter.All;
                allowedMedicineTypeIdsByBhyt = null;
                allowedMaterialTypeIdsByBhyt = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Loads the type ids matching the selected value for one item kind (called on each search).
        /// Returns false when the call failed: the set is then empty (the list shows nothing, never everything)
        /// and the caller shows the message from param after hiding the waiting form.
        /// </summary>
        private bool LoadBhytInfoTypeIds(bool isMedicine, CommonParam param)
        {
            bool success = true;
            HashSet<long> ids = null;
            try
            {
                EnumBhytInfoFilter mode = CurrentBhytInfoFilter;
                if (mode != EnumBhytInfoFilter.All)
                {
                    bool isMissing = mode == EnumBhytInfoFilter.Missing;
                    // Separate methods: a client with an old MOS.Filter.dll fails here only, not the whole search.
                    ids = isMedicine ? GetMedicineTypeIdsByBhytInfo(isMissing, param) : GetMaterialTypeIdsByBhytInfo(isMissing, param);
                    if (ids == null)
                    {
                        ids = new HashSet<long>();
                        success = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                ids = new HashSet<long>();
                success = false;
            }

            if (isMedicine) allowedMedicineTypeIdsByBhyt = ids;
            else allowedMaterialTypeIdsByBhyt = ids;
            return success;
        }

        // No IS_ACTIVE / IS_BUSINESS (locked types still in stock must be classified), no IDs (Oracle IN > 1000),
        // no Start/Limit (the DAO pages only when both are set).
        private HashSet<long> GetMedicineTypeIdsByBhytInfo(bool isMissing, CommonParam param)
        {
            MOS.Filter.HisMedicineTypeViewFilter filter = new MOS.Filter.HisMedicineTypeViewFilter();
            filter.IS_MISS_BHYT_INFO = isMissing;
            filter.ColumnParams = new List<string> { "ID" };
            var data = new BackendAdapter(param).Get<List<V_HIS_MEDICINE_TYPE>>(RequestUriStore.HIS_MEDICINE_TYPE_GET_VIEW_DYNAMIC, ApiConsumers.MosConsumer, filter, param);
            return data != null ? new HashSet<long>(data.Where(o => o != null).Select(o => o.ID)) : null;
        }

        private HashSet<long> GetMaterialTypeIdsByBhytInfo(bool isMissing, CommonParam param)
        {
            MOS.Filter.HisMaterialTypeViewFilter filter = new MOS.Filter.HisMaterialTypeViewFilter();
            filter.IS_MISS_BHYT_INFO = isMissing;
            filter.ColumnParams = new List<string> { "ID" };
            var data = new BackendAdapter(param).Get<List<V_HIS_MATERIAL_TYPE>>(RequestUriStore.HIS_MATERIAL_TYPE_GET_VIEW_DYNAMIC, ApiConsumers.MosConsumer, filter, param);
            return data != null ? new HashSet<long>(data.Where(o => o != null).Select(o => o.ID)) : null;
        }
    }
}
