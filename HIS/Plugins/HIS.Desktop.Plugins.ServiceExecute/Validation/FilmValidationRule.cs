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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ServiceExecute.Validation
{
    class FilmValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtNumberOfFilm;
        internal long serviceReqTypeId;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool vaild = false;
            try
            {
                if (txtNumberOfFilm == null) return vaild;
                if (!string.IsNullOrEmpty(txtNumberOfFilm.Text.Trim()))
                {
                    long result = 0;
                    if (!long.TryParse(txtNumberOfFilm.Text.Trim(), out result) || result <= 0)
                    {
                        ErrorText = "Số film phải nhập số lượng lớn hơn 0";
                        return vaild;
                    }
                }

                var numOfFilm = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ServiceExecuteCFG.NumberOfFilmCFG);
                if (serviceReqTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA && !string.IsNullOrEmpty(numOfFilm) && numOfFilm.Trim() == "1")
                {
                    if (string.IsNullOrEmpty(txtNumberOfFilm.Text.Trim()))
                    {
                        ErrorText = "Trường dữ liệu bắt buộc";
                        return vaild;
                    }
                    if (txtNumberOfFilm.Text.Length > 15)
                    {
                        ErrorText = "Quá độ dài cho phép 15 ký tự";
                        return vaild;
                    }
                }
                vaild = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return vaild;
        }
    }
}
