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

using HIS.Desktop.Plugins.Library.TreatmentEndTypeExt.Run;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.SDO;
using HIS.Desktop.Common;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.Library.TreatmentEndTypeExt
{
    public class TreatmentEndTypeExtProcessor : BussinessBase
    {
        object frm;
        FormEnum.TYPE formType;
        DelegateSelectData ReloadDataTreatmentEndTypeExt;
        HIS_TREATMENT_END_TYPE_EXT TreatmentEndTypeExt;
        public TreatmentEndTypeExtProcessor()
            : base()
        {
        }

        public TreatmentEndTypeExtProcessor(HIS_TREATMENT_END_TYPE_EXT treatmentEndTypeExt, DelegateSelectData _reloadDataTreatmentEndTypeExt)
            : base()
        {
            this.TreatmentEndTypeExt = treatmentEndTypeExt;
            this.ReloadDataTreatmentEndTypeExt = _reloadDataTreatmentEndTypeExt;
        }

        public TreatmentEndTypeExtProcessor(CommonParam paramBusiness)
            : base(paramBusiness)
        {
        }

        public void Run(List<object> arg)
        {
            frm = null;
            try
            {
                if (TreatmentEndTypeExt == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("treatmentEndTypeExt is null");
                    return;
                }

                switch (TreatmentEndTypeExt.ID)
                {
                    case IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM:
                        formType = FormEnum.TYPE.NGHI_OM;
                        break;
                    case IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_DUONG_THAI:
                        formType = FormEnum.TYPE.NGHI_DUONG_THAI;
                        break;
                    //case IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_VIEC_HUONG_BHXH:
                    //    formType = FormEnum.TYPE.NGHI_VIEC_HUONG_BHXH;
                    //    break;
                    case IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__HEN_MO:
                        formType = FormEnum.TYPE.HEN_MO;
                        break;
                    default:
                        break;
                }

                IRun behavior = RunFactory.MakeTreatmentEndTypeExt(param, arg);
                frm = behavior != null ? (behavior.Run(this.formType, this.ReloadDataTreatmentEndTypeExt)) : null;
                if (frm != null)
                {
                    ((Form)(frm)).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
