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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using MOS.EFMODEL.DataModels;
using HIS.UC.Hospitalize.ADO;
using MOS.SDO;
using MOS.Filter;
using Inventec.Common.Adapter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using HIS.UC.Hospitalize.Config;
using HIS.UC.SecondaryIcd.ADO;

namespace HIS.UC.Hospitalize.Run
{
    public partial class UCHospitalize : UserControl
    {
        public HospitalizeExamADO GetValue()
        {
            HospitalizeExamADO hospitalize = null;
            try
            {
                this.positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                    return hospitalize;
                ////Check co hoa don hay khong
                //if (CheckPrescriptionExist())
                //{
                //    DialogResult dialogResult = DevExpress.XtraEditors.XtraMessageBox.Show("Bệnh nhân đã được kê thuốc. Bạn có muốn cho bệnh nhân nhập viện không?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                //    if (dialogResult == DialogResult.No)
                //        return null;
                //}

                hospitalize = new HospitalizeExamADO();
                hospitalize.HisDepartmentTranHospitalizeSDO = new HisDepartmentTranHospitalizeSDO();

                if (!HisConfig.IsUsingServerTime)
                {
                    if (dtLogTime.EditValue != null && dtLogTime.DateTime != DateTime.MinValue)
                        hospitalize.HisDepartmentTranHospitalizeSDO.Time = long.Parse(dtLogTime.DateTime.ToString("yyyyMMddHHmm00"));
                    //hospitalize.HisDepartmentTranHospitalizeSDO.Time = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtLogTime.DateTime) ?? 0;
                }
                hospitalize.HisDepartmentTranHospitalizeSDO.TreatmentId = hospitalizeInitADO.TreatmentId.Value;
                if (cboDepartment.EditValue != null)
                {
                    hospitalize.HisDepartmentTranHospitalizeSDO.DepartmentId = Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue).ToString());
                }
                if (hospitalizeInitADO.RoomId.HasValue)
                    hospitalize.HisDepartmentTranHospitalizeSDO.RequestRoomId = hospitalizeInitADO.RoomId.Value;
                if (cboBedRoom.EditValue != null)
                {
                    hospitalize.HisDepartmentTranHospitalizeSDO.BedRoomId = Inventec.Common.TypeConvert.Parse.ToInt64((cboBedRoom.EditValue).ToString());
                }
                hospitalize.HisDepartmentTranHospitalizeSDO.TreatmentTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentType.EditValue ?? "0").ToString());

                if (chkPrintHospitalizeExam.CheckState == CheckState.Checked)
                    hospitalize.IsPrintHospitalizeExam = true;
                else
                    hospitalize.IsPrintHospitalizeExam = false;
                hospitalize.IsPrintMps178 = chkPrintMps178.Checked;
                if (chkSign.CheckState == CheckState.Checked)
                    hospitalize.IsSign = true;
                else
                    hospitalize.IsSign = false;

                if (chkPrintDocumentSigned.CheckState == CheckState.Checked)
                    hospitalize.IsPrintSign = true;
                else
                    hospitalize.IsPrintSign = false;

                if (dtFinishTime.EditValue != null && dtFinishTime.DateTime != DateTime.MinValue)
                {
                    hospitalize.FinishTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtFinishTime.DateTime) ?? 0;
                }

                if (chkEmergency.CheckState == CheckState.Checked)
                {
                    hospitalize.HisDepartmentTranHospitalizeSDO.IsEmergency = true;
                }
                else 
                {
                    hospitalize.HisDepartmentTranHospitalizeSDO.IsEmergency = false;
                }


                hospitalize.HisDepartmentTranHospitalizeSDO.RelativeName = txtRELATIVE_NAME.Text;
                hospitalize.HisDepartmentTranHospitalizeSDO.RelativePhone = txtRELATIVE_PHONE.Text;
                hospitalize.HisDepartmentTranHospitalizeSDO.RelativeAddress = txtRELATIVE_ADDRESS.Text;
                hospitalize.HisDepartmentTranHospitalizeSDO.InHospitalizationReasonCode = txtHospitalReasonCode.Text;
                hospitalize.HisDepartmentTranHospitalizeSDO.InHospitalizationReasonName = btnHospitalReasonName.Text;
                if (cboCareer.EditValue != null)
                {
                    long? careerId = Inventec.Common.TypeConvert.Parse.ToInt64(cboCareer.EditValue.ToString());
                    hospitalize.HisDepartmentTranHospitalizeSDO.CareerId = careerId > 0 ? careerId : null;
                }

                hospitalize.icdADOInTreatment = this.UcIcdGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                hospitalize.TraditionalIcdADOInTreatment = this.UcTraditionalIcdGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                hospitalize.icdSubADOInTreatment = this.UcSecondaryIcdGetValue() as SecondaryIcdDataADO;
                hospitalize.tradtionalIcdSub = this.UcTraditionalSecondaryIcdGetValue() as SecondaryIcdDataADO;
                hospitalize.Note = txtNote.Text.Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return hospitalize;
        }

        private bool CheckPrescriptionExist()
        {
            bool result = false;
            try
            {
                if (!hospitalizeInitADO.DepartmentId.HasValue)
                    throw new Exception("DepartmentID is null");
                CommonParam param = new CommonParam();
                HisServiceReqFilter filter = new HisServiceReqFilter();
                filter.TREATMENT_ID = hospitalizeInitADO.TreatmentId;
                filter.SERVICE_REQ_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT };
                var services = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, filter, param);
                if (services != null && services.Count > 0)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
