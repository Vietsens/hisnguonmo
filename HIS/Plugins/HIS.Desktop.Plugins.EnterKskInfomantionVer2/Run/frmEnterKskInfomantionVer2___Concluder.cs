/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Bổ sung trường "Người khám" ở phần Kết luận cho tab KSK trên 18 tuổi + dưới 18 tuổi.
 * Giá trị lưu vào HIS_KSK_GENERAL.CONCLUDER_LOGINNAME + CONCLUDER_USERNAME (giống tab KSK định kỳ).
 * Nhúng combo bằng code lúc Load (nhất quán với cách nhúng cụm ICD tiền sử).
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        /// <summary>Combo "Người khám" (kết luận) bổ sung theo tab (1=trên 18, 2=dưới 18).</summary>
        private readonly Dictionary<int, GridLookUpEdit> dicConcluderCboExt = new Dictionary<int, GridLookUpEdit>();

        /// <summary>
        /// Cấu hình combo "Người khám" kết luận cho tab trên/dưới 18 tuổi. Combo khai báo sẵn trong Designer
        /// (cboConcluderLoginName2/3 + GridView riêng) — giống cboConcluderLoginName tab định kỳ. Gọi 1 lần ở Load.
        /// </summary>
        private void InitConcluderComboForTabs()
        {
            try
            {
                RegisterConcluderCombo(1, this.cboConcluderLoginName2); // Ksk trên 18 tuổi
                RegisterConcluderCombo(2, this.cboConcluderLoginName3); // Ksk dưới 18 tuổi
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Nạp danh sách người khám + wire nút xóa cho 1 combo (đã khai báo trong Designer).</summary>
        private void RegisterConcluderCombo(int tabIndex, GridLookUpEdit cbo)
        {
            try
            {
                if (cbo == null || dicConcluderCboExt.ContainsKey(tabIndex)) return;
                // Cấu hình datasource + cột (danh sách người khám) — tái dùng helper của tab định kỳ.
                SetDataCboExamLoginName(cbo);
                // Nút Delete: xóa giá trị đang chọn (tái dùng handler chung của form).
                cbo.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.ClearData_ButtonClick);
                dicConcluderCboExt[tabIndex] = cbo;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Đổ người khám kết luận từ HIS_KSK_GENERAL vào combo (dùng chung lượt khám). Gọi ở Load sau Init.</summary>
        private void LoadConcluderComboExt()
        {
            try
            {
                if (currentKskGeneral == null) return;
                foreach (var cbo in dicConcluderCboExt.Values)
                    if (cbo != null) cbo.EditValue = currentKskGeneral.CONCLUDER_LOGINNAME;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Ghi người khám kết luận (theo tab đang lưu) vào HIS_KSK_GENERAL trước khi gọi API.</summary>
        private void FillConcluderExtToGeneral(HIS_KSK_GENERAL g)
        {
            if (g == null) return;
            try
            {
                int tab = xtraTabControl1.SelectedTabPageIndex;
                GridLookUpEdit cbo;
                if (!dicConcluderCboExt.TryGetValue(tab, out cbo) || cbo == null) return;

                string login = cbo.EditValue != null ? cbo.EditValue.ToString() : null;
                g.CONCLUDER_LOGINNAME = string.IsNullOrWhiteSpace(login) ? null : login;
                g.CONCLUDER_USERNAME = !string.IsNullOrEmpty(g.CONCLUDER_LOGINNAME)
                    ? BackendDataWorker.Get<V_HIS_EMPLOYEE>()
                        .FirstOrDefault(o => o.LOGINNAME == g.CONCLUDER_LOGINNAME)?.TDL_USERNAME
                    : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Hiển thị Ngày kết luận từ HIS_KSK_GENERAL.CONCLUSION_TIME vào 3 ô mới (tab trên 18 /
        /// dưới 18 / trẻ &lt;6) — giống pattern tab định kỳ: có giá trị thì hiển thị, chưa có = hôm nay.
        /// Gọi ở Load sau LoadConcluderComboExt.
        /// </summary>
        private void LoadConclusionTimeExt()
        {
            try
            {
                DateTime val = DateTime.Now;
                if (currentKskGeneral != null && currentKskGeneral.CONCLUSION_TIME != null && currentKskGeneral.CONCLUSION_TIME > 0)
                    val = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentKskGeneral.CONCLUSION_TIME.Value) ?? DateTime.Now;
                dteConclusionTime2.DateTime = val;
                dteConclusionTime3.DateTime = val;
                dteConclusionTime8.DateTime = val;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Truyền Ngày kết luận vào HIS_KSK_GENERAL.CONCLUSION_TIME khi Lưu ở các tab ngoài "định kỳ"
        /// (tab định kỳ đã tự lưu qua GetValueGeneral):
        /// - Trên 18 / dưới 18 / trẻ &lt;6: dùng ô Ngày kết luận mới (hiển thị theo GENERAL).
        /// - Lái xe định kỳ / nghề nghiệp: ô ngày riêng lưu bảng riêng, đồng thời truyền kèm sang GENERAL.
        /// Chưa có bản ghi GENERAL trong SDO -&gt; tạo (gán ID hiện có + SERVICE_REQ_ID) để BE upsert đúng dòng.
        /// </summary>
        private void ApplyConclusionTimeToKskGeneralSdo(MOS.SDO.HisServiceReqKskExecuteV2SDO sdo)
        {
            try
            {
                if (sdo == null) return;
                DateEdit dte = null;
                switch (xtraTabControl1.SelectedTabPageIndex)
                {
                    case 1: dte = dteConclusionTime2; break;               // Ksk trên 18 tuổi
                    case 2: dte = dteConclusionTime3; break;               // Ksk dưới 18 tuổi
                    case 3: dte = dteConclusionTimePeriodDriver; break;    // Ksk lái xe (định kỳ)
                    case 6: dte = dteConclusionTimeOccupational; break;    // Ksk nghề nghiệp
                    case 7: dte = dteConclusionTime8; break;               // Trẻ em dưới 6 tuổi
                }
                if (dte == null || dte.EditValue == null) return;
                if (sdo.KskGeneral == null) sdo.KskGeneral = new MOS.SDO.KskGeneralV2SDO();
                if (sdo.KskGeneral.HisKskGeneral == null)
                {
                    sdo.KskGeneral.HisKskGeneral = new HIS_KSK_GENERAL();
                    if (currentKskGeneral != null) sdo.KskGeneral.HisKskGeneral.ID = currentKskGeneral.ID;
                    if (currentServiceReq != null) sdo.KskGeneral.HisKskGeneral.SERVICE_REQ_ID = currentServiceReq.ID;
                }
                sdo.KskGeneral.HisKskGeneral.CONCLUSION_TIME =
                    Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dte.DateTime);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }
    }
}
