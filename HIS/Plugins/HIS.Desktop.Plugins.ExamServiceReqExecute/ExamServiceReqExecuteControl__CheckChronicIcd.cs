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
using HIS.Desktop.Plugins.ExamServiceReqExecute.Resources;
using HIS.UC.ExamTreatmentFinish.Run;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl
    {
        private static bool isIcdChronicPropertyLoaded;
        private static System.Reflection.PropertyInfo icdChronicProperty;

        /// <summary>
        /// True when the loaded MOS.EFMODEL has HIS_ICD.IS_CHRONIC.
        /// IS_CHRONIC is read through cached PropertyInfo so the plugin builds and runs with both
        /// the old MOS.EFMODEL (no column -> warning is skipped) and the new one.
        /// </summary>
        private static bool IsIcdChronicFieldSupported
        {
            get
            {
                if (!isIcdChronicPropertyLoaded)
                {
                    isIcdChronicPropertyLoaded = true;
                    try
                    {
                        icdChronicProperty = typeof(HIS_ICD).GetProperty("IS_CHRONIC");
                        if (icdChronicProperty == null)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("MOS.EFMODEL chua co HIS_ICD.IS_CHRONIC -> bo qua canh bao ICD chinh la benh man tinh (55058)");
                        }
                    }
                    catch (Exception ex)
                    {
                        icdChronicProperty = null;
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }
                return icdChronicProperty != null;
            }
        }

        /// <summary>
        /// 55058: khi ket thuc dieu tri tai phong kham, neu ICD chinh duoc danh dau la benh man tinh
        /// (HIS_ICD.IS_CHRONIC = 1) ma chua tick "Man tinh" thi hoi bac si co tiep tuc khong.
        /// Chi ap dung khi checkbox "Man tinh" dang hien va cho phep tick.
        /// Tra false khi bac si chon quay lai tick --> khong luu.
        /// </summary>
        private bool CheckChronicMainIcd(string mainIcdCode)
        {
            bool valid = true;
            try
            {
                var uc = this.ucTreatmentFinish as UCExamTreatmentFinish;
                if (uc == null || !uc.IsChronicEditable || uc.IsChronicChecked)
                    return true;

                if (String.IsNullOrWhiteSpace(mainIcdCode) || !IsIcdChronicFieldSupported)
                    return true;

                string icdCode = mainIcdCode.Trim();
                HIS_ICD chronicIcd = FindChronicIcd(this.currentIcds, icdCode);
                if (chronicIcd == null)
                    return true;

                Inventec.Common.Logging.LogSystem.Debug("CheckChronicMainIcd: ICD chinh la benh man tinh nhung chua tick Man tinh"
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => icdCode), icdCode));

                if (XtraMessageBox.Show(
                        String.Format(ResourceMessage.IcdChinhLaBenhManTinhChuaTichManTinh, chronicIcd.ICD_CODE + " - " + chronicIcd.ICD_NAME),
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    valid = false;
                    this.BeginInvoke(new Action(() => uc.FocusChronic()));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        /// <summary>
        /// currentIcds: ICD dang hoat dong, khong phai YHCT (nap 1 lan khi Load).
        /// Tim theo ma truoc, chi doc IS_CHRONIC (reflection) tren ban ghi tim duoc.
        /// </summary>
        private static HIS_ICD FindChronicIcd(List<HIS_ICD> icds, string icdCode)
        {
            HIS_ICD icd = icds != null ? icds.FirstOrDefault(o => o.ICD_CODE == icdCode) : null;
            if (icd == null)
                return null;
            short? isChronic = icdChronicProperty.GetValue(icd, null) as short?;
            return isChronic == 1 ? icd : null;
        }
    }
}
