using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.Plugins.HisImportMestMedicine.Base;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisImportMestMedicine
{
    // vCong 58605 - Tich hop Cong mua sam: khoa nut Sua thong tin NCC, Huy thuc nhap cua phieu nhap NCC
    // khi Cong mua sam khong cho sua/xoa. Chi phieu da thuc nhap moi co tren Cong mua sam.
    public partial class UCHisImportMestMedicine : UserControlBase
    {
        private readonly Dictionary<long, CongMuaSamCheckADO> dicCongMuaSam = new Dictionary<long, CongMuaSamCheckADO>();
        private readonly HashSet<long> congMuaSamChecking = new HashSet<long>();

        private void CheckCongMuaSamAsync(V_HIS_IMP_MEST data)
        {
            try
            {
                if (data == null || data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC
                    || data.IMP_MEST_STT_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT
                    || !CongMuaSamChecker.IsEnabled)
                {
                    return;
                }
                long id = data.ID;
                lock (congMuaSamChecking)
                {
                    if (!congMuaSamChecking.Add(id)) return;
                }
                Task.Factory.StartNew(() =>
                {
                    CongMuaSamCheckADO result = null;
                    try
                    {
                        result = CongMuaSamChecker.CheckImpMest(id);
                    }
                    finally
                    {
                        lock (congMuaSamChecking)
                        {
                            congMuaSamChecking.Remove(id);
                        }
                    }
                    if (result == null || this.IsDisposed || !this.IsHandleCreated) return;
                    this.BeginInvoke(new Action(() => this.ApplyCongMuaSam(id, result)));
                });
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ApplyCongMuaSam(long impMestId, CongMuaSamCheckADO result)
        {
            try
            {
                dicCongMuaSam[impMestId] = result;
                for (int i = 0; i < gridViewImportMestList.RowCount; i++)
                {
                    V_HIS_IMP_MEST row = gridViewImportMestList.GetRow(i) as V_HIS_IMP_MEST;
                    if (row != null && row.ID == impMestId)
                    {
                        gridViewImportMestList.RefreshRow(i);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Goi cuoi CustomRowCellEdit: chi khoa them, khong mo nut dang khoa.</summary>
        private void CongMuaSamOverrideButton(V_HIS_IMP_MEST data, CustomRowCellEditEventArgs e)
        {
            CongMuaSamCheckADO result = this.GetCongMuaSamLock(data);
            if (result == null) return;
            if (!result.CanEdit)
            {
                if (e.Column.FieldName == "EditNCC") e.RepositoryItem = Btn_EditInfoImpMestNCC_Disable;
                else if (e.Column.FieldName == "EDIT") e.RepositoryItem = repositoryItemButtonEditDisable;
            }
            if (!result.CanDelete)
            {
                if (e.Column.FieldName == "IMPORT_DISPLAY" && e.RepositoryItem == Btn_Cancel_Import_Enable) e.RepositoryItem = Btn_Cancel_Import_Disable;
                else if (e.Column.FieldName == "DISCARD_DISPLAY") e.RepositoryItem = repositoryItemButtonDiscardDisable;
            }
        }

        /// <summary>Ly do khoa de hien tooltip tren nut. Null khi khong khoa.</summary>
        private string GetCongMuaSamTooltip(V_HIS_IMP_MEST data, string fieldName)
        {
            CongMuaSamCheckADO result = this.GetCongMuaSamLock(data);
            if (result == null) return null;
            bool locked = (!result.CanEdit && (fieldName == "EditNCC" || fieldName == "EDIT"))
                || (!result.CanDelete && (fieldName == "IMPORT_DISPLAY" || fieldName == "DISCARD_DISPLAY"));
            if (!locked) return null;
            return "Cổng mua sắm: " + (String.IsNullOrWhiteSpace(result.Reason) ? "không cho phép thao tác này" : result.Reason);
        }

        private CongMuaSamCheckADO GetCongMuaSamLock(V_HIS_IMP_MEST data)
        {
            CongMuaSamCheckADO result;
            if (data == null || !dicCongMuaSam.TryGetValue(data.ID, out result) || result == null) return null;
            if (result.CanEdit && result.CanDelete) return null;
            return result;
        }
    }
}
