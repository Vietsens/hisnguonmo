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
using HIS.Desktop.Plugins.AssignPrescriptionCLS.ADO;
using MOS.EFMODEL.DataModels;
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.AssignPrescriptionCLS
{
    class MediMatyCreateWorker
    {
        public delegate object GetDataAmountOutOfStock(MediMatyTypeADO model, long serviceId, long meidStockId);
        public delegate void SetDefaultMediStockForData(MediMatyTypeADO model);
        public delegate HIS_PATIENT_TYPE ChoosePatientTypeDefaultlService(long patientTypeId, MediMatyTypeADO medimaty);
        public delegate HIS_PATIENT_TYPE ChoosePatientTypeDefaultlServiceOther(long patientTypeId, long serviceId, long serviceTypeId);
        public delegate long GetPatientTypeId();
        public delegate int GetNumRow();
        public delegate void SetNumRow();
        public delegate List<MediMatyTypeADO> GetMediMatyTypeADOs();
        public delegate bool GetIsAutoCheckExpend();

        public GetDataAmountOutOfStock getDataAmountOutOfStock;
        public SetDefaultMediStockForData setDefaultMediStockForData;
        public ChoosePatientTypeDefaultlService choosePatientTypeDefaultlService;
        public ChoosePatientTypeDefaultlServiceOther choosePatientTypeDefaultlServiceOther;
        public GetPatientTypeId getPatientTypeId;
        public GetNumRow getNumRow;
        public SetNumRow setNumRow;
        public GetMediMatyTypeADOs getMediMatyTypeADOs;
        public GetIsAutoCheckExpend getIsAutoCheckExpend;

        internal MediMatyCreateWorker(GetDataAmountOutOfStock _getDataAmountOutOfStock, SetDefaultMediStockForData _setDefaultMediStockForData, ChoosePatientTypeDefaultlService _choosePatientTypeDefaultlService, ChoosePatientTypeDefaultlServiceOther _choosePatientTypeDefaultlServiceOther, GetPatientTypeId _getPatientTypeId, GetNumRow _getNumRow, SetNumRow _setNumRow, GetMediMatyTypeADOs _getMediMatyTypeADOs, GetIsAutoCheckExpend _getIsAutoCheckExpend)
        {
            this.getDataAmountOutOfStock = _getDataAmountOutOfStock;
            this.setDefaultMediStockForData = _setDefaultMediStockForData;
            this.choosePatientTypeDefaultlService = _choosePatientTypeDefaultlService;
            this.choosePatientTypeDefaultlServiceOther = _choosePatientTypeDefaultlServiceOther;
            this.getPatientTypeId = _getPatientTypeId;
            this.getNumRow = _getNumRow;
            this.setNumRow = _setNumRow;
            this.getMediMatyTypeADOs = _getMediMatyTypeADOs;
            this.getIsAutoCheckExpend = _getIsAutoCheckExpend;
        }
    }
}
