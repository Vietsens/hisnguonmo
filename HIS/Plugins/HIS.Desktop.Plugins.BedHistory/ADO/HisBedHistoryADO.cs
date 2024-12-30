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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.BedHistory.ADO
{
    public class HisBedHistoryADO : V_HIS_BED_LOG
    {
        public bool IsChecked { get; set; }
        public DateTime startTime { get; set; }
        public DateTime? finishTime { get; set; }

        public bool IsSave { get; set; }//luu du lieu them moi/ sua du lieu
        public int Action { get; set; }
        public string BED_SERVICE_TYPE_CODE { get; set; }
        public long BED_CODE_ID { get; set; }
        public long? MaxIntructionTime { get; set; }
        public long? BILL_PATIENT_TYPE_ID { get; set; }
        public bool IS_NOT_CHANGE_BILL_PATY { get; set; }
        public bool IS_NOT_CHANGE_BILL_PATY_DEFAULT { get; set; }
        public bool IsContainAppliedPatientType { get; set; }
        public bool Error { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeBedId { get; set; }
        public string ErrorMessageBedId { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeFinishTime { get; set; }
        public string ErrorMessageFinishTime { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeStartTime { get; set; }
        public string ErrorMessageStartTime { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeBebServiceTypeId { get; set; }
        public string ErrorMessageBebServiceTypeId { get; set; }

        public string ErrorMessagePrimaryPatientTypeId { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypePrimaryPatientTypeId { get; set; }
        public string ErrorMessagePatientTypeId { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypePatientTypeId { get; set; }
        public bool IsBedStretcher { get; set; }
        public bool HasDefaultPatientTypeId { get; set; }
        public bool HasServiceReq { get; set; }
        public long BedIdAfterSave { get; set; }
        public HisBedHistoryADO()
        {
            this.startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
        }

        public HisBedHistoryADO(V_HIS_BED_LOG data, int action, bool isSave, List<HIS_SERVICE_REQ> listServiceReq, List<HisBedADO> hisBedADOs)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<HisBedHistoryADO>(this, data);
                    Inventec.Common.Logging.LogSystem.Debug("HisBedHistoryADO.1");

                    this.startTime = data.START_TIME > 0 ? (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.START_TIME) : this.startTime;
                    if (data.FINISH_TIME != null)
                    {
                        //this.finishTime = (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.FINISH_TIME ?? 0);
                        this.finishTime = data.FINISH_TIME.HasValue
                                        ? (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.FINISH_TIME.Value)
                                        : (DateTime?)null;

                    }
                    else
                    {
                        this.finishTime = null;
                    }
                    Inventec.Common.Logging.LogSystem.Debug("HisBedHistoryADO.2");
                    this.BED_CODE_ID = data.BED_ID;
                    var bed = hisBedADOs.FirstOrDefault(o => o.ID == data.BED_ID);
                    Inventec.Common.Logging.LogSystem.Debug("HisBedHistoryADO.3");
                    this.IsBedStretcher = bed != null ? bed.IS_BED_STRETCHER == 1 : false;
                    if (this.BED_SERVICE_TYPE_ID != null && this.BED_SERVICE_TYPE_ID.HasValue)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("HisBedHistoryADO.4");
                        var bedType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == this.BED_SERVICE_TYPE_ID.Value);
                        this.BED_SERVICE_TYPE_CODE = bedType != null ? bedType.SERVICE_CODE : "";
                        this.BILL_PATIENT_TYPE_ID = bedType != null ? (long?)bedType.BILL_PATIENT_TYPE_ID : null;
                        this.IS_NOT_CHANGE_BILL_PATY = bedType != null ? bedType.IS_NOT_CHANGE_BILL_PATY == 1 : false;
                        this.IS_NOT_CHANGE_BILL_PATY_DEFAULT = this.IS_NOT_CHANGE_BILL_PATY;
                    }
                    else
                        this.BED_SERVICE_TYPE_CODE = null;
                    Inventec.Common.Logging.LogSystem.Debug("HisBedHistoryADO.5");
                    if (data.SERVICE_REQ_ID.HasValue || (listServiceReq != null && listServiceReq.Exists(o => o.BED_LOG_ID == data.ID)))
                    {
                        this.HasServiceReq = true;
                        if (listServiceReq != null && listServiceReq.Any(o => o.BED_LOG_ID == data.ID))
                        {
                            Inventec.Common.Logging.LogSystem.Debug("HisBedHistoryADO.6");
                            this.MaxIntructionTime = listServiceReq
                                .Where(o => o.BED_LOG_ID == data.ID)
                                .Max(o => o.INTRUCTION_TIME);
                        }

                    }
                }
                else
                {
                    this.startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                }
                this.Action = action;
                this.IsSave = isSave;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            
        }
    }
}
