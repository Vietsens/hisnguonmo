using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.KskSyncList.ADO
{
    /// <summary>
    /// Item gui len api/HisKskSync/SaveSyncResult (muc 3.2.2 PTTK_44350).
    /// Cac truong PATIENT_CODE / KskTypeName chi dung de hien thi tren hop thoai
    /// ket qua day lo (Scene 4) - Backend bo qua truong thua.
    /// </summary>
    public class KskSyncResultADO
    {
        // Khoa upsert HIS_KSK_SYNC theo (KSK_TYPE_ID, KSK_RECORD_ID)
        public long KSK_TYPE_ID { get; set; }
        public long KSK_RECORD_ID { get; set; }

        // Ket qua: 2 = Da dong bo, 3 = That bai
        public short SYNC_RESULT_TYPE { get; set; }
        public long SYNC_TIME { get; set; }
        public string TRANSACTION_CODE { get; set; }
        public string SYNC_FAILD_REASON { get; set; }
        public string REGISTRATION_NO { get; set; }

        // Hien thi tren hop thoai ket qua (khong gui ve DB)
        public string PATIENT_CODE { get; set; }
        public string KskTypeName { get; set; }

        // Ghi chu khi THANH CONG (khong gui ve DB) — vd cong VLG tiep nhan bat dong bo (QUEUED) /
        // tiep nhan co canh bao (ACCEPTED_WITH_WARNING). Null/rong -> hien thi "✓ Đã đồng bộ" nhu cu.
        public string SuccessNote { get; set; }

        public bool IsSuccess
        {
            get { return SYNC_RESULT_TYPE == 2; }
        }

        // Hien thi tren hop thoai ket qua day lo (Scene 4)
        public string ResultText
        {
            get
            {
                if (IsSuccess)
                    return "✓ Đã đồng bộ" + (string.IsNullOrEmpty(SuccessNote) ? "" : " — " + SuccessNote);
                return "✗ Thất bại" + (string.IsNullOrEmpty(SYNC_FAILD_REASON) ? "" : " — " + SYNC_FAILD_REASON);
            }
        }

        public string TransactionDisplay
        {
            get { return string.IsNullOrEmpty(TRANSACTION_CODE) ? "—" : TRANSACTION_CODE; }
        }
    }
}
