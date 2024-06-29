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
using HIS.Desktop.DelegateRegister;
using DevExpress.XtraLayout;
using DevExpress.XtraEditors;
using MOS.SDO;

namespace HIS.UC.UCServiceRoomInfo
{
    public partial class UCServiceRoomInfo : HIS.Desktop.Utility.UserControlBase
    {
		public List<ServiceReqDetailSDO> GetDetail()
        {
            List<ServiceReqDetailSDO> results = new List<ServiceReqDetailSDO>();
            try
            {
                if (layoutControl2.Root.Items != null && layoutControl2.Root.Items.Count > 0)
                {
                    foreach (LayoutControlItem item in layoutControl2.Root.Items)
                    {
                        if (item != null && (item.Control is UserControl || item.Control is XtraUserControl))
                        {
                            var dataServ = this.roomExamServiceProcessor.GetDetailSDO(item.Control) as List<ServiceReqDetailSDO>;
                            if (dataServ != null && dataServ.Count > 0)
                            {
                                foreach (var itemPT in dataServ)
                                {
                                    if (cboPatientType.EditValue != null)
                                        itemPT.PatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString());
                                    if (lciCboPatientTypePhuThu.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && CboPatientTypePrimary.EditValue != null)
                                    {
                                        itemPT.PrimaryPatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(CboPatientTypePrimary.EditValue.ToString());
                                    }
                                }
                                results.AddRange(dataServ);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return results;
        }
    }
}
