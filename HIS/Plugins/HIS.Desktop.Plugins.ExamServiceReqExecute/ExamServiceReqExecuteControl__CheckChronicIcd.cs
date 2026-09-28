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
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl
    {
        private static bool? isIcdChronicFieldSupported;

        /// <summary>
        /// True when the loaded MOS.EFMODEL has HIS_ICD.IS_CHRONIC.
        /// The IS_CHRONIC access lives in a NoInlining method, so an older EFMODEL at runtime only
        /// turns this warning off instead of failing ProcessTreatmentFinish (MissingMethodException).
        /// </summary>
        private static bool IsIcdChronicFieldSupported
        {
            get
            {
                if (!isIcdChronicFieldSupported.HasValue)
                {
                    try
                    {
                        isIcdChronicFieldSupported = typeof(HIS_ICD).GetProperty("IS_CHRONIC") != null;
                        if (!isIcdChronicFieldSupported.Value)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("MOS.EFMODEL chua co HIS_ICD.IS_CHRONIC -> bo qua canh bao ICD chinh la benh man tinh (55058)");
                        }
                    }
                    catch (Exception ex)
                    {
                        isIcdChronicFieldSupported = false;
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }
                return isIcdChronicFieldSupported.Value;
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
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static HIS_ICD FindChronicIcd(List<HIS_ICD> icds, string icdCode)
        {
            return icds != null ? icds.FirstOrDefault(o => o.IS_CHRONIC == 1 && o.ICD_CODE == icdCode) : null;
        }
    }
}
