using ACS.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.TreatmentList.Base;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.TreatmentList
{
    /// <summary>
    /// 58013 - "Sửa dịch vụ" button: edit services of many KSK contract patients at once.
    /// The button is only visible while the grid data is filtered by a KSK contract.
    /// </summary>
    public partial class UCTreatmentList
    {
        private const string MODULE_LINK__KSK_SERVICE_EDIT_LIST = "HIS.Desktop.Plugins.KskServiceEditList";

        /// <summary>
        /// KSK contract id that was applied to the current grid data.
        /// Null when the grid is not filtered by a KSK contract.
        /// </summary>
        private long? appliedKskContractId = null;

        /// <summary>
        /// Remember the contract filter actually used to load the grid, then refresh the button.
        /// Must be called with the filter sent to the API (not the combo value), because searching
        /// by treatment code rebuilds the filter and clears the contract combo.
        /// </summary>
        private void SetAppliedKskContract(long? kskContractId)
        {
            try
            {
                this.appliedKskContractId = kskContractId;
                this.SetVisibleBtnKskServiceEdit();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetVisibleBtnKskServiceEdit()
        {
            try
            {
                bool isVisible = this.appliedKskContractId.HasValue
                    && this.appliedKskContractId.Value > 0
                    && this.IsAllowKskServiceEdit();

                this.lciBtnKskServiceEdit.Visibility = isVisible
                    ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                    : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Control code not declared in ACS -> not controlled (allowed).
        /// Declared -> the user must own it through a role.
        /// </summary>
        private bool IsAllowKskServiceEdit()
        {
            bool result = false;
            try
            {
                ACS_CONTROL control = BackendDataWorker.Get<ACS_CONTROL>()
                    .FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnKskServiceEdit);
                if (control == null || control.IS_ANONYMOUS == 1)
                {
                    result = true;
                }
                else
                {
                    result = this.controlAcs != null
                        && this.controlAcs.Any(o => o.CONTROL_CODE == ControlCode.BtnKskServiceEdit);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void btnKskServiceEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.lciBtnKskServiceEdit.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                    || !this.appliedKskContractId.HasValue)
                    return;

                int[] selectedRows = gridViewtreatmentList.GetSelectedRows();
                List<V_HIS_TREATMENT_4> treatments = selectedRows != null
                    ? selectedRows.Select(i => gridViewtreatmentList.GetRow(i) as V_HIS_TREATMENT_4)
                        .Where(o => o != null).ToList()
                    : new List<V_HIS_TREATMENT_4>();

                if (!treatments.Any())
                {
                    XtraMessageBox.Show(Resources.ResourceMessage.ChuaChonBenhNhanSuaDichVuKsk,
                        Resources.ResourceMessage.Thongbao);
                    return;
                }

                List<string> invalidCodes = treatments
                    .Where(o => o.TDL_KSK_CONTRACT_ID != this.appliedKskContractId)
                    .Select(o => o.TREATMENT_CODE).ToList();
                if (invalidCodes.Any())
                {
                    XtraMessageBox.Show(String.Format(Resources.ResourceMessage.HoSoKhongThuocHopDongKsk,
                        String.Join(", ", invalidCodes)), Resources.ResourceMessage.Thongbao);
                    return;
                }

                V_HIS_KSK_CONTRACT contract = this.listKskContract != null
                    ? this.listKskContract.FirstOrDefault(o => o.ID == this.appliedKskContractId.Value)
                    : null;
                if (contract == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Khong tim thay hop dong KSK. "
                        + Inventec.Common.Logging.LogUtil.TraceData(
                            Inventec.Common.Logging.LogUtil.GetMemberName(() => this.appliedKskContractId), this.appliedKskContractId));
                    return;
                }

                treatments = treatments.GroupBy(o => o.ID).Select(g => g.First()).ToList();
                Inventec.Common.Logging.LogSystem.Debug(
                    Inventec.Common.Logging.LogUtil.TraceData("KskContractId", contract.ID)
                    + Inventec.Common.Logging.LogUtil.TraceData("TreatmentCount", treatments.Count));

                List<object> listArgs = new List<object>();
                listArgs.Add(treatments);
                listArgs.Add(contract);
                listArgs.Add((RefeshReference)BtnSearch);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(MODULE_LINK__KSK_SERVICE_EDIT_LIST,
                    this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
