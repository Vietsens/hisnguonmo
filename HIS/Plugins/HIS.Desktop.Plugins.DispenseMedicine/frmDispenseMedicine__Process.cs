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
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.Plugins.DispenseMedicine.ADO;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.DispenseMedicine
{
    public partial class frmDispenseMedicine : FormBase
    {
        private void ProcessDataToCreate(List<DispenseMedyMatyADO> dispenseMatyADOs, ref HisDispenseSDO hisDispenseSDO)
        {
            try
            {
                hisDispenseSDO.RequestRoomId = this.roomId;
                hisDispenseSDO.MediStockId = this.mediStock.ID;
                hisDispenseSDO.Amount = spinMetyAmount.Value;
                hisDispenseSDO.DispenseTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                if (dtExpTime.EditValue != null)
                    hisDispenseSDO.ExpiredDate = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtExpTime.DateTime);
                hisDispenseSDO.HeinDocumentNumber = txtHeinDocumentNumber.Text;
                hisDispenseSDO.MedicineTypeId = this.currentMetyThanhPham.MEDICINE_TYPE_ID;
                hisDispenseSDO.PackageNumber = txtPackageNumber.Text;
                //hisDispenseSDO. = new List<HisDispenseMetySDO>();
                foreach (var item in dispenseMatyADOs)
                {
                    if (item.ServiceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                    {
                        if (hisDispenseSDO.MedicineTypes == null)
                            hisDispenseSDO.MedicineTypes = new List<HisDispenseMetySDO>();
                        HisDispenseMetySDO dispenseMetySDO = new HisDispenseMetySDO();
                        dispenseMetySDO.MedicineTypeId = item.PreparationMediMatyTypeId;
                        dispenseMetySDO.Amount = item.Amount;
                        hisDispenseSDO.MedicineTypes.Add(dispenseMetySDO);
                    }
                    else if (item.ServiceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                    {

                        if (hisDispenseSDO.MaterialTypes == null)
                            hisDispenseSDO.MaterialTypes = new List<HisDispenseMatySDO>();
                        HisDispenseMatySDO dispenseMatySDO = new HisDispenseMatySDO();
                        dispenseMatySDO.MaterialTypeId = item.PreparationMediMatyTypeId;
                        dispenseMatySDO.Amount = item.Amount;
                        hisDispenseSDO.MaterialTypes.Add(dispenseMatySDO);
                    }
                }
                hisDispenseSDO.MedicinePaties = new List<HIS_MEDICINE_PATY>();
                List<MedicinePatyADO> medicPatyADOHasPrices = medicinePatyADOs.Where(o => o.Price.HasValue).ToList();

                foreach (var item in medicPatyADOHasPrices)
                {
                    HIS_MEDICINE_PATY medicinePaty = new HIS_MEDICINE_PATY();
                    medicinePaty.PATIENT_TYPE_ID = item.PatientTypeId;
                    medicinePaty.EXP_PRICE = item.Price ?? 0;
                    if (item.Vat.HasValue)
                    {
                        medicinePaty.EXP_VAT_RATIO = (item.Vat ?? 0) / 100;
                    }

                    hisDispenseSDO.MedicinePaties.Add(medicinePaty);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessDataToUpdate(List<DispenseMedyMatyADO> dispenseMatyADOs, ref HisDispenseUpdateSDO hisDispenseSDO)
        {
            try
            {
                hisDispenseSDO.RequestRoomId = this.roomId;
                hisDispenseSDO.Amount = spinMetyAmount.Value;
                hisDispenseSDO.DispenseTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                if (dtExpTime.EditValue != null)
                    hisDispenseSDO.ExpiredDate = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtExpTime.DateTime);
                hisDispenseSDO.HeinDocumentNumber = txtHeinDocumentNumber.Text;
                hisDispenseSDO.PackageNumber = txtPackageNumber.Text;
                hisDispenseSDO.Id = this.dispenseId ?? 0;

                foreach (var item in dispenseMatyADOs)
                {
                    if (item.ServiceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                    {
                        if (hisDispenseSDO.MedicineTypes == null)
                            hisDispenseSDO.MedicineTypes = new List<HisDispenseMetySDO>();
                        HisDispenseMetySDO dispenseMetySDO = new HisDispenseMetySDO();
                        dispenseMetySDO.MedicineTypeId = item.PreparationMediMatyTypeId;
                        dispenseMetySDO.Amount = item.Amount;
                        hisDispenseSDO.MedicineTypes.Add(dispenseMetySDO);
                    }
                    else if (item.ServiceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                    {

                        if (hisDispenseSDO.MaterialTypes == null)
                            hisDispenseSDO.MaterialTypes = new List<HisDispenseMatySDO>();
                        HisDispenseMatySDO dispenseMatySDO = new HisDispenseMatySDO();
                        dispenseMatySDO.MaterialTypeId = item.PreparationMediMatyTypeId;
                        dispenseMatySDO.Amount = item.Amount;
                        hisDispenseSDO.MaterialTypes.Add(dispenseMatySDO);
                    }
                }
                hisDispenseSDO.MedicinePaties = new List<HIS_MEDICINE_PATY>();
                List<MedicinePatyADO> medicPatyADOHasPrices = medicinePatyADOs.Where(o => o.Price.HasValue).ToList();

                foreach (var item in medicPatyADOHasPrices)
                {
                    HIS_MEDICINE_PATY medicinePaty = new HIS_MEDICINE_PATY();
                    medicinePaty.PATIENT_TYPE_ID = item.PatientTypeId;
                    medicinePaty.EXP_PRICE = item.Price ?? 0;
                    if (item.Vat.HasValue)
                        medicinePaty.EXP_VAT_RATIO = (item.Vat ?? 0) / 100;
                    medicinePaty.ID = item.Id;
                    medicinePaty.MEDICINE_ID = item.MedicineId;
                    hisDispenseSDO.MedicinePaties.Add(medicinePaty);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CapNhatKhaDungThuocChePham(long materialTypeId, long serviceTypeId, decimal amount, PROCESS process)
        {
            //foreach (var item in mediStockD1SDOs)
            //{
            //    if (item.ID == materialTypeId && item.SERVICE_TYPE_ID == serviceTypeId)
            //    {
            //        if (process == PROCESS.CONG)
            //        {
            //            item.AMOUNT += amount;
            //        }
            //        else if (process == PROCESS.TRU)
            //        {
            //            item.AMOUNT -= amount;
            //        }
            //    }
            //}
            //gridViewThuocCP.GridControl.RefreshDataSource();
        }

        public enum PROCESS
        {
            TRU,
            CONG
        }
    }
}
