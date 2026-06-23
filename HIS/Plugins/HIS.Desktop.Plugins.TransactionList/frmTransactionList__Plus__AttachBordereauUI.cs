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
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Plugins.TransactionList.Config;
using Inventec.Common.Logging;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.TransactionList
{
    public partial class frmTransactionList : HIS.Desktop.Utility.FormBase
    {
        #region Bo loc + cot "Dinh kem bang ke"
        // 3 radio (cbBordereauAll/Done/None) + 3 LayoutControlItem (lciBordereau*) khai bao trong
        // frmTransactionList.Designer.cs -> HIEN trong designer cho maintainer thay/sua.
        // O day chi dieu khien an/hien theo config + localize + ControlState. Cot grid tao runtime.
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnBordereauAttach;

        /// <summary>Chan CheckedChanged ghi ControlState khi dang nap trang thai lan dau.</summary>
        private bool isLoadingBordereauAttachState = false;

        /// <summary>KEY luu ControlState cho bo loc dinh kem bang ke.</summary>
        private const string BORDEREAU_ATTACH_STATE_KEY = "cbBordereauAttachStatusFilter";

        /// <summary>
        /// Dieu khien UI bo loc + cot "Dinh kem bang ke". 3 radio + 3 LCI da co san trong Designer.
        /// Config MOS.HIS_TRANSACTION.AUTO_ATTACH_BORDEREAU_HDDT__VNPT co gia tri -> giu hang loc +
        /// localize caption + them cot + nap ControlState. Rong -> AN hang loc + thu nhom "Trang Thai"
        /// ve chieu cao cu (giao dien nhu khi chua co tinh nang).
        /// Goi trong frmTransactionList_Load, sau InitControlState, truoc FillDataToGrid.
        /// </summary>
        private void InitBordereauAttachUI()
        {
            try
            {
                if (string.IsNullOrEmpty(HisConfigCFG.AutoAttachBordereauHddtVnpt))
                {
                    HideBordereauAttachRow();
                    return;
                }

                LocalizeBordereauAttachControls();
                AddBordereauAttachGridColumn();
                InitBordereauAttachControlState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Config tat -> an hang loc "Dinh kem bang ke" (Designer) + thu nhom ve chieu cao 2 hang (70).</summary>
        private void HideBordereauAttachRow()
        {
            try
            {
                if (this.lciBordereauAll != null) this.lciBordereauAll.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                if (this.lciBordereauDone != null) this.lciBordereauDone.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                if (this.lciBordereauNone != null) this.lciBordereauNone.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                // Designer dat GroupClientHeight = 95 (3 hang). Tat tinh nang -> ve 70 (2 hang) cho khong thua khoang trong.
                if (this.navTransactionStatus != null)
                    this.navTransactionStatus.GroupClientHeight = 70;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Config bat -> set caption 3 radio theo ngon ngu hien tai (Designer de mac dinh tieng Viet).</summary>
        private void LocalizeBordereauAttachControls()
        {
            try
            {
                if (this.cbBordereauAll != null)
                    this.cbBordereauAll.Properties.Caption = GetLangValue("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__NAVBAR_BORDEREAU_ATTACH_ALL");
                if (this.cbBordereauDone != null)
                    this.cbBordereauDone.Properties.Caption = GetLangValue("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__NAVBAR_BORDEREAU_ATTACH_DONE");
                if (this.cbBordereauNone != null)
                    this.cbBordereauNone.Properties.Caption = GetLangValue("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__NAVBAR_BORDEREAU_ATTACH_NONE");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AddBordereauAttachGridColumn()
        {
            try
            {
                if (this.gridViewTransaction == null || gridColumnBordereauAttach != null) return;

                gridColumnBordereauAttach = new DevExpress.XtraGrid.Columns.GridColumn();
                gridColumnBordereauAttach.Name = "gridColumnBordereauAttach";
                gridColumnBordereauAttach.FieldName = "BORDEREAU_ATTACH_STATUS";
                gridColumnBordereauAttach.Caption = GetLangValue("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__GC_BORDEREAU_ATTACH_STATUS");
                gridColumnBordereauAttach.OptionsColumn.AllowEdit = false;
                gridColumnBordereauAttach.Width = 110;
                // Dat TRUOC nhom 4 cot audit (ui_rules: 4 cot audit luon o cuoi grid)
                int auditVisibleIndex = this.gridColumn_Transaction_CreateTime != null ? this.gridColumn_Transaction_CreateTime.VisibleIndex : -1;
                gridColumnBordereauAttach.VisibleIndex = auditVisibleIndex >= 0 ? auditVisibleIndex : this.gridViewTransaction.Columns.Count;
                this.gridViewTransaction.Columns.Add(gridColumnBordereauAttach);

                this.gridViewTransaction.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(gridViewTransaction_BordereauCustomColumnDisplayText);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTransaction_BordereauCustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (gridColumnBordereauAttach == null || e.Column != gridColumnBordereauAttach) return;
                long val = Inventec.Common.TypeConvert.Parse.ToInt64(e.Value == null ? "" : e.Value.ToString());
                e.DisplayText = val == 1
                    ? GetLangValue("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__NAVBAR_BORDEREAU_ATTACH_DONE")
                    : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Gan gia tri loc BordereauAttachStatus vao filter (goi trong FillDataToGridTransaction).</summary>
        private void SetBordereauAttachFilter(HisTransactionViewFilter filter)
        {
            try
            {
                if (filter == null) return;
                if (string.IsNullOrEmpty(HisConfigCFG.AutoAttachBordereauHddtVnpt)) return;

                if (cbBordereauDone != null && cbBordereauDone.Checked)
                {
                    filter.BordereauAttachStatus = 1;   // Da dinh kem
                }
                else if (cbBordereauNone != null && cbBordereauNone.Checked)
                {
                    filter.BordereauAttachStatus = 0;   // Chua dinh kem
                }
                // else: Tat ca -> khong gan (null)
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region CheckedChanged — loai tru lan nhau (radio) + luu ControlState
        private void cbBordereauAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbBordereauAll.Checked)
                {
                    cbBordereauDone.Checked = false;
                    cbBordereauNone.Checked = false;
                }
                else if (!cbBordereauDone.Checked && !cbBordereauNone.Checked)
                {
                    cbBordereauAll.Checked = true;
                }
                SaveBordereauAttachState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbBordereauDone_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbBordereauDone.Checked)
                {
                    cbBordereauAll.Checked = false;
                    cbBordereauNone.Checked = false;
                }
                else if (!cbBordereauAll.Checked && !cbBordereauNone.Checked)
                {
                    cbBordereauDone.Checked = true;
                }
                SaveBordereauAttachState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbBordereauNone_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbBordereauNone.Checked)
                {
                    cbBordereauAll.Checked = false;
                    cbBordereauDone.Checked = false;
                }
                else if (!cbBordereauAll.Checked && !cbBordereauDone.Checked)
                {
                    cbBordereauNone.Checked = true;
                }
                SaveBordereauAttachState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region ControlState — nho lua chon bo loc giua cac phien
        private void InitBordereauAttachControlState()
        {
            try
            {
                isLoadingBordereauAttachState = true;
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    var item = this.currentControlStateRDO.FirstOrDefault(o => o.KEY == BORDEREAU_ATTACH_STATE_KEY);
                    if (item != null)
                    {
                        if (item.VALUE == "1") cbBordereauDone.Checked = true;
                        else if (item.VALUE == "0") cbBordereauNone.Checked = true;
                        else cbBordereauAll.Checked = true;
                    }
                }
                isLoadingBordereauAttachState = false;
            }
            catch (Exception ex)
            {
                isLoadingBordereauAttachState = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveBordereauAttachState()
        {
            if (isLoadingBordereauAttachState) return;
            if (string.IsNullOrEmpty(HisConfigCFG.AutoAttachBordereauHddtVnpt)) return;
            try
            {
                if (this.controlStateWorker == null) return;

                string value = (cbBordereauDone != null && cbBordereauDone.Checked) ? "1"
                    : ((cbBordereauNone != null && cbBordereauNone.Checked) ? "0" : "");

                if (this.currentControlStateRDO == null)
                    this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();

                var item = this.currentControlStateRDO.FirstOrDefault(o => o.KEY == BORDEREAU_ATTACH_STATE_KEY && o.MODULE_LINK == moduleLink);
                if (item != null)
                {
                    item.VALUE = value;
                }
                else
                {
                    this.currentControlStateRDO.Add(new HIS.Desktop.Library.CacheClient.ControlStateRDO
                    {
                        KEY = BORDEREAU_ATTACH_STATE_KEY,
                        MODULE_LINK = moduleLink,
                        VALUE = value
                    });
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private string GetLangValue(string key)
        {
            try
            {
                return Inventec.Common.Resource.Get.Value(
                    key,
                    Base.ResourceLangManager.LanguageFrmTransactionList,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }
        #endregion
    }
}
