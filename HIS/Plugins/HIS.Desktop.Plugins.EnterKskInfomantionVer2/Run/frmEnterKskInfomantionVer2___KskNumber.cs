using Inventec.Common.Logging;
using System;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        /// <summary>
        /// Hien thi "So thu tu KSK" o banner thong tin BN. BE tu sinh so (KskExecuteV2 goi PKG_KSK_CODE);
        /// FE CHI HIEN THI. Chi 4 loai co so: tab 1 = tren 18, 2 = duoi 18, 5 = KSK khac, 7 = tre em duoi 6.
        ///
        /// Nguon du lieu (QUAN TRONG - vi form LAZY-LOAD tab):
        ///   - current*  : chi set khi tab do da duoc fill (EnsureTabLoaded) HOAC sau khi Luu (tra ve tu API).
        ///   - pre*      : nap SAN luc mo form (PrefetchFormData -> api/HisKskSync/GetKskData), du ca 4 loai.
        /// => Uu tien current* (moi nhat sau Luu), fallback pre* (co ngay luc mo du chua bam sang tab).
        ///
        /// Khi dang o tab KHONG phai loai co so (0 dinh ky / 3,4 lai xe / 6 nghe nghiep): van hien
        /// so cua loai co so ma CA NAY thuc su co (over18 -> under18 -> other -> under6) de mo ca la thay ngay.
        /// </summary>
        private void UpdateKskNumberDisplay()
        {
            try
            {
                if (txtKskNumber == null) return;
                txtKskNumber.Text = ResolveKskDisplayCode(); // null/rong -> de trong
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Chon ma de hien: uu tien tab dang mo neu la loai co so; neu tab hien tai la loai co so nhung
        /// khong co ma -> de trong (loai do that su khong co ho so). Neu tab KHONG phai loai co so ->
        /// fallback lay ma cua loai co so dau tien ma ca nay co.
        /// </summary>
        private string ResolveKskDisplayCode()
        {
            int tab = xtraTabControl1.SelectedTabPageIndex;

            string c = KskCodeForTab(tab);
            if (!string.IsNullOrWhiteSpace(c)) return c;

            bool isNumberedTab = (tab == 1 || tab == 2 || tab == 5 || tab == 7);
            if (!isNumberedTab)
            {
                foreach (int t in new[] { 1, 2, 5, 7 })
                {
                    c = KskCodeForTab(t);
                    if (!string.IsNullOrWhiteSpace(c)) return c;
                }
            }
            return null;
        }

        /// <summary>Ma so KSK cua 1 tab loai co so: current* (fresh) truoc, pre* (prefetch) sau.</summary>
        private string KskCodeForTab(int tab)
        {
            switch (tab)
            {
                case 1: // tren 18
                    if (currentKskOverEight != null) return currentKskOverEight.KSK_OVER_EIGHTEEN_CODE;
                    if (preKskOverEighteens != null && preKskOverEighteens.Count > 0) return preKskOverEighteens[0].KSK_OVER_EIGHTEEN_CODE;
                    return null;
                case 2: // duoi 18
                    if (currentKskUnderEight != null) return currentKskUnderEight.KSK_UNDER_EIGHTEEN_CODE;
                    if (preKskUnderEighteens != null && preKskUnderEighteens.Count > 0) return preKskUnderEighteens[0].KSK_UNDER_EIGHTEEN_CODE;
                    return null;
                case 5: // KSK khac
                    if (currentKskOther != null) return currentKskOther.KSK_OTHER_CODE;
                    if (preKskOthers != null && preKskOthers.Count > 0) return preKskOthers[0].KSK_OTHER_CODE;
                    return null;
                case 7: // tre duoi 6
                    if (currentKskUnderSixEf != null) return currentKskUnderSixEf.KSK_UNDER_SIX_CODE;
                    if (preKskUnderSixes != null && preKskUnderSixes.Count > 0) return preKskUnderSixes[0].KSK_UNDER_SIX_CODE;
                    return null;
            }
            return null;
        }
    }
}
