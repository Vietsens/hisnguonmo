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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK
{
    class ValidActiveIngrBhytWorker
    {
        /// <summary>     
        /// Cảnh báo thuốc đã kê trong 1 đơn có cùng hoạt chất bhyt chùng không, có thì đưa ra cảnh báo
        /// Cảnh báo khi lưu
        /// </summary>
        internal static bool Valid(List<MediMatyTypeADO> mediMatyTypeADOs, MediMatyTypeADO mediMatyTypeADO)
        {
            bool valid = true;
            try
            {
                if ((mediMatyTypeADOs != null && mediMatyTypeADOs.Count > 0) || mediMatyTypeADO != null)
                {
                    string medicineTypeNames = "";                    

                    var mediMatyTypeADOGroups = mediMatyTypeADOs.Where(o => o.ACTIVE_INGR_BHYT_CODE != null).GroupBy(o => o.ACTIVE_INGR_BHYT_CODE.ToUpper());
                    foreach (var g in mediMatyTypeADOGroups)
                    {
                        if (g.Count() > 1)
                        {
                            medicineTypeNames += (g.Aggregate((i, j) => new MediMatyTypeADO { MEDICINE_TYPE_NAME = i.MEDICINE_TYPE_NAME + ";" + j.MEDICINE_TYPE_NAME }).MEDICINE_TYPE_NAME) + "\r\n";
                        }
                    }

                    if (!String.IsNullOrEmpty(medicineTypeNames))
                    {
                        DialogResult myResult;
                        myResult = MessageBox.Show(String.Format(ResourceMessage.CacThuocCoCUngHoatChat_BanCoMuonTiepTuc, medicineTypeNames), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (myResult != System.Windows.Forms.DialogResult.Yes)
                            valid = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

    }
}
