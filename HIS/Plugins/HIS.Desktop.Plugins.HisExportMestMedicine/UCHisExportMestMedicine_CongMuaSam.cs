using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.Plugins.HisExportMestMedicine.Base;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisExportMestMedicine
{
    // vCong 58605 - Tich hop Cong mua sam: khoa Huy thuc xuat cua phieu xuat tra NCC khi Cong mua sam khong cho.
    // Chi phieu tra da thuc xuat moi co tren Cong mua sam.
    public partial class UCHisExportMestMedicine : UserControlBase
    {
        private readonly Dictionary<long, CongMuaSamCheckADO> dicCongMuaSam = new Dictionary<long, CongMuaSamCheckADO>();
        private readonly HashSet<long> congMuaSamChecking = new HashSet<long>();

        private static bool IsCongMuaSamReceipt(long expMestTypeId, long expMestSttId)
        {
            return expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__TNCC
                && expMestSttId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;
        }

        private void CheckCongMuaSamAsync(V_HIS_EXP_MEST_2 data)
        {
            try
            {
                if (data == null || !IsCongMuaSamReceipt(data.EXP_MEST_TYPE_ID, data.EXP_MEST_STT_ID) || !CongMuaSamChecker.IsEnabled)
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
                        result = CongMuaSamChecker.CheckExpMest(id);
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

        private void ApplyCongMuaSam(long expMestId, CongMuaSamCheckADO result)
        {
            try
            {
                dicCongMuaSam[expMestId] = result;
                for (int i = 0; i < gridView.RowCount; i++)
                {
                    V_HIS_EXP_MEST_2 row = gridView.GetRow(i) as V_HIS_EXP_MEST_2;
                    if (row != null && row.ID == expMestId)
                    {
                        gridView.RefreshRow(i);
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
        private void CongMuaSamOverrideButton(V_HIS_EXP_MEST_2 data, CustomRowCellEditEventArgs e)
        {
            CongMuaSamCheckADO result = data != null ? this.GetCongMuaSamLock(data.ID) : null;
            if (result == null) return;
            if (!result.CanDelete && e.Column.FieldName == "EXPORT_DISPLAY" && e.RepositoryItem == Btn_HuyThucXuat_Enable)
            {
                e.RepositoryItem = Btn_HuyThucXuat_Disable;
            }
        }

        private string GetCongMuaSamTooltip(V_HIS_EXP_MEST_2 data, string fieldName)
        {
            CongMuaSamCheckADO result = data != null ? this.GetCongMuaSamLock(data.ID) : null;
            if (result == null || result.CanDelete || fieldName != "EXPORT_DISPLAY") return null;
            return ReasonText(result);
        }

        /// <summary>Menu chuot phai Huy thuc xuat: Cong mua sam khong cho thi bao ly do, khong goi API.</summary>
        private bool IsCongMuaSamBlockCancelExport(V_HIS_EXP_MEST row)
        {
            if (row == null || !IsCongMuaSamReceipt(row.EXP_MEST_TYPE_ID, row.EXP_MEST_STT_ID)) return false;
            CongMuaSamCheckADO result = this.GetCongMuaSamLock(row.ID);
            if (result == null || result.CanDelete) return false;
            XtraMessageBox.Show(ReasonText(result), "Thông báo");
            return true;
        }

        private static string ReasonText(CongMuaSamCheckADO result)
        {
            return "Cổng mua sắm: " + (String.IsNullOrWhiteSpace(result.Reason) ? "không cho phép thao tác này" : result.Reason);
        }

        private CongMuaSamCheckADO GetCongMuaSamLock(long expMestId)
        {
            CongMuaSamCheckADO result;
            if (!dicCongMuaSam.TryGetValue(expMestId, out result) || result == null) return null;
            if (result.CanEdit && result.CanDelete) return null;
            return result;
        }
    }
}
