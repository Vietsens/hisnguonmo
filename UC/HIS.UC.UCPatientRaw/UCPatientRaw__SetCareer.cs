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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.BackendData;

namespace HIS.UC.UCPatientRaw
{
    public partial class UCPatientRaw : HIS.Desktop.Utility.UserControlBase
    {
        public void SetCareerByCardNumber(string heinCardNumder)
        {
            try
            {
                //Khi người dùng nhập thẻ BHYT, nếu đầu mã thẻ là TE1, thì tự động chọn giá trị của trường "Nghề nghiệp" là "Trẻ em dưới 6 tuổi"
                //27/10/2017 => sửa lại => Căn cứ vào đầu thẻ BHYT và dữ liệu cấu hình trong bảng HIS_BHYT_WHITELIST để tự động điền nghề nghiệp tương ứng
                MOS.EFMODEL.DataModels.HIS_CAREER career = GetCareerByBhytWhiteListConfig(heinCardNumder);
                if (career == null)
                {
                    if (this.dtPatientDob.DateTime != DateTime.MinValue)
                    {
                        if (this.dtPatientDob.DateTime != DateTime.MinValue && MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(this.dtPatientDob.DateTime))
                        {
                            career = HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.CareerUnder6Age;
                        }
                        else if (DateTime.Now.Year - this.dtPatientDob.DateTime.Year <= 18)
                        {
                            career = HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.CareerHS;
                        }
                        else if (this.cboCareer.EditValue == null)//#15903 Them IF 
                        {
                            career = HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.CareerBase;
                        }
                    }
                    else if (this.cboCareer.EditValue == null)//#15903 Them IF 
                    {
                        career = HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.CareerBase;
                    }
                }
                if (career != null && career.ID > 0)
                {            
                    this.txtCareerCode.Text = career.CAREER_CODE;
                    this.cboCareer.EditValue = career.ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
