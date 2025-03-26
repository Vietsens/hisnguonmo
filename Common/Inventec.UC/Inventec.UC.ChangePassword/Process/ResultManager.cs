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
using DevExpress.Utils;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ChangePassword.Process
{
    class ResultManager
    {
        public static void ShowMessage(CommonParam param, bool? success)
        {
            try
            {
                if (success.HasValue)
                    MessageUtil.SetResultParam(param, success.Value);
                string message = MessageUtil.GetMessageAlert(param);
                if (!String.IsNullOrEmpty(message))
                    DevExpress.XtraEditors.XtraMessageBox.Show(message, MessageUtil.GetMessage(Message.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), DefaultBoolean.True);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
