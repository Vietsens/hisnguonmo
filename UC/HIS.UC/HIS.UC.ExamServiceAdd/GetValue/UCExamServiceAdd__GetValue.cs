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
using MOS.SDO;
using HIS.UC.ExamServiceAdd.ADO;
using MOS.Filter;
using Inventec.Common.Adapter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using MOS.EFMODEL.DataModels;
using HIS.UC.HisExamServiceAdd.Config;

namespace HIS.UC.ExamServiceAdd.Run
{
    public partial class UCExamServiceAdd : UserControl
    {
        public HisServiceReqExamAdditionSDO GetValue()
        {
            HisServiceReqExamAdditionSDO result = null;
            try
            {
                this.positionHandle = -1;
                if (dxValidationProvider1.Validate())
                {
                    result = new HisServiceReqExamAdditionSDO();
                    result.AdditionRoomId = Inventec.Common.TypeConvert.Parse.ToInt64((cboExecuteRoom.EditValue ?? 0).ToString());
                    if (cboExamService.EditValue != null)
                    {
                        result.AdditionServiceId = Inventec.Common.TypeConvert.Parse.ToInt64(cboExamService.EditValue.ToString());
                    }

                    if (!HisConfig.IsUsingServerTime)
                        result.InstructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtIntructionTime.DateTime) ?? 0;
                    if (chkIsPrimary.Checked) result.IsPrimary = true;
                    if (chkChangeDepartment.Checked) result.IsChangeDepartment = true;
                    if (chkKetThucKhamHienTai.Checked) result.IsFinishCurrent = true;
                    if (chkNotRequireFeeNonBHYT.Checked) result.IsNotRequireFee = true;

                    if (examServiceAddInitADO.ServiceReqId.HasValue)
                    {
                        CommonParam param = new CommonParam();
                        HisSereServFilter sereServFilter = new HisSereServFilter();
                        sereServFilter.SERVICE_REQ_ID = examServiceAddInitADO.ServiceReqId.Value;
                        HIS_SERE_SERV sereServ = new BackendAdapter(param)
                     .Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, sereServFilter, param).FirstOrDefault();
                        result.CurrentSereServId = sereServ.ID;
                    }

                    if (examServiceAddInitADO.roomId.HasValue)
                    {
                        result.RequestRoomId = examServiceAddInitADO.roomId.Value;
                    }
                    if (this.sereServ != null)
                    {
                        result.CurrentSereServId = this.sereServ.ID;
                    }

                    if (cboPatientType.EditValue != null)
                    {
                        result.PatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString());
                    }

                    if (cboPrimaryPatientType.EditValue != null 
                        && (HisConfig.IsSetPrimaryPatientType == "1" || HisConfig.IsSetPrimaryPatientType == "2"))
                    {
                        result.PrimaryPatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPrimaryPatientType.EditValue.ToString());
                    }
                    if (NUM_ORDER_BLOCK_ID != null && NUM_ORDER_BLOCK_ID > 0)
                    {
                       //result.xxx = NUM_ORDER_BLOCK_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
