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
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.Library.TreatmentEndTypeExt.Data;
using HIS.Desktop.Plugins.Library.TreatmentEndTypeExt.Run;
using HIS.Desktop.Plugins.Library.TreatmentEndTypeExt.SickLeave;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Library.TreatmentEndTypeExt.Run
{
    public sealed class RunBehavior : IRun
    {
        List<object> data;
        long treatmentId;
        TreatmentEndTypeExtData SickLeaveData;
        HisTreatmentFinishSDO treatmentFinishSDO;
        List<BabyADO> MaternityLeaveDatas;
        Inventec.Desktop.Common.Modules.Module moduleData;
        bool isShowCheckPrint;
        public RunBehavior()
            : base()
        {
        }

        public RunBehavior(CommonParam param, List<object> _data)
            : base()
        {
            this.data = _data;
        }

        object IRun.Run(FormEnum.TYPE _formType, DelegateSelectData _reloadDataTreatmentEndTypeExt)
        {
            object frm = null;
            try
            {
                InitData();
                if (_formType == FormEnum.TYPE.NGHI_OM || _formType == FormEnum.TYPE.NGHI_DUONG_THAI)
                {
                    frm = new frmSickLeave(moduleData, treatmentId, _formType, this.SickLeaveData, _reloadDataTreatmentEndTypeExt, treatmentFinishSDO);
                }
                else
                {
                    frm = new Surgery.frmSurgery(moduleData, treatmentId, _formType, this.SickLeaveData, _reloadDataTreatmentEndTypeExt, isShowCheckPrint);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return frm;
        }

        private void InitData()
        {
            try
            {
                if (data != null && data.Count > 0)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        if (data[i] is long)
                            this.treatmentId = (long)data[i];
                        else if (data[i] is TreatmentEndTypeExtData)
                            this.SickLeaveData = (TreatmentEndTypeExtData)data[i];
                        else if (data[i] is HisTreatmentFinishSDO)
                            this.treatmentFinishSDO = (HisTreatmentFinishSDO)data[i];
                        else if (data[i] is Inventec.Desktop.Common.Modules.Module)
                            this.moduleData = (Inventec.Desktop.Common.Modules.Module)data[i];
                        else if (data[i] is bool)
                            this.isShowCheckPrint = (bool)data[i];
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
