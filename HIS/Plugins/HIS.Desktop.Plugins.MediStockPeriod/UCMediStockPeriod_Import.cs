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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.MediStockPeriod.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MediStockPeriod
{
    public partial class UCMediStockPeriod
    {
        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    WaitingManager.Show();

                    var import = new Inventec.Common.ExcelImport.Import();
                    if (import.ReadFileExcel(ofd.FileName))
                    {
                        var hisServiceImport = import.GetWithCheck<MestPeriodMediMateADO>(0);
                        if (hisServiceImport != null && hisServiceImport.Count > 0)
                        {
                            List<MestPeriodMediMateADO> listAfterRemove = new List<MestPeriodMediMateADO>();

                            foreach (var item in hisServiceImport)
                            {
                                if (string.IsNullOrEmpty(item.METY_MATY_CODE))
                                    continue;

                                bool checkNull = string.IsNullOrEmpty(item.METY_MATY_CODE)
                                   || string.IsNullOrEmpty(item.AMOUNT_STR);

                                if (!checkNull)
                                {
                                    listAfterRemove.Add(item);
                                }
                            }
                            WaitingManager.Hide();


                            if (listAfterRemove != null && listAfterRemove.Count > 0)
                            {
                                List<MestPeriodMediMateADO> _BedAdos = new List<MestPeriodMediMateADO>();
                                addServiceToProcessList(listAfterRemove, ref _BedAdos);
                                SetDataSource(_BedAdos);
                            }

                            //btnSave.Enabled = true;
                        }
                        else
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Import thất bại");
                        }
                    }
                    else
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không đọc được file");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // update lại số lượng kiểm kê
        private void SetDataSource(List<MestPeriodMediMateADO> dataSource)
        {
            try
            {
                if (dataSource != null && dataSource.Count() > 0)
                {
                    var medicines = dataSource.Where(o => o.IS_MEDICINE.ToLower() == "x").ToList();

                    if (medicines != null && medicines.Count > 0)
                    {
                        foreach (var item in ListMestPeriodMety)
                        {
                            List<MestPeriodMediMateADO> checkMedicine = new List<MestPeriodMediMateADO>();

                            var checkPackageNumber = medicines.Where(o => o.METY_MATY_CODE.Trim() == item.MEDICINE_TYPE_CODE
                                && (o.PACKAGE_NUMBER == item.PACKAGE_NUMBER || ((String.IsNullOrEmpty(o.PACKAGE_NUMBER) && String.IsNullOrEmpty(item.PACKAGE_NUMBER))))).ToList();

                            var checkExpiredDate = medicines.Where(o => o.METY_MATY_CODE.Trim() == item.MEDICINE_TYPE_CODE
                               && ((o.EXPIRED_DATE.HasValue && item.EXPIRED_DATE.HasValue && Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.EXPIRED_DATE.Value) == Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.EXPIRED_DATE.Value))
                                || (!o.EXPIRED_DATE.HasValue && !item.EXPIRED_DATE.HasValue))).ToList();

                            if (checkPackageNumber != null && checkPackageNumber.Count() > 0 && checkExpiredDate != null && checkExpiredDate.Count() > 0)
                            {
                                checkMedicine = checkPackageNumber.Where(o => checkExpiredDate.Select(p => p.ID).Contains(o.ID)).ToList();
                            }
                            else if (checkPackageNumber != null && checkPackageNumber.Count() > 0)
                            {
                                checkMedicine = checkPackageNumber;
                            }
                            else if (checkExpiredDate != null)
                            {
                                checkMedicine = checkExpiredDate;
                            }

                            if (checkMedicine != null && checkMedicine.Count() > 0)
                            {
                                item.KK_AMOUNT = checkMedicine[0].AMOUNT;
                            }
                        }

                        gridControlMedicine.BeginUpdate();
                        gridControlMedicine.DataSource = null;
                        gridControlMedicine.DataSource = ListMestPeriodMety;
                        gridControlMedicine.EndUpdate();
                    }

                    var materials = dataSource.Where(o => string.IsNullOrEmpty(o.IS_MEDICINE)).ToList();
                    if (materials != null && materials.Count() > 0)
                    {
                        foreach (var item in ListMestPeriodMaty)
                        {
                            List<MestPeriodMediMateADO> checkMaterial = new List<MestPeriodMediMateADO>();

                            var checkPackageNumber = materials.Where(o => o.METY_MATY_CODE.Trim() == item.MATERIAL_TYPE_CODE
                                && (o.PACKAGE_NUMBER == item.PACKAGE_NUMBER || ((String.IsNullOrEmpty(o.PACKAGE_NUMBER) && String.IsNullOrEmpty(item.PACKAGE_NUMBER))))).ToList();

                            var checkExpiredDate = materials.Where(o => o.METY_MATY_CODE.Trim() == item.MATERIAL_TYPE_CODE
                               && ((o.EXPIRED_DATE.HasValue && item.EXPIRED_DATE.HasValue && Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.EXPIRED_DATE.Value) == Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.EXPIRED_DATE.Value))
                                || (!o.EXPIRED_DATE.HasValue && !item.EXPIRED_DATE.HasValue))).ToList();

                            if (checkPackageNumber != null && checkPackageNumber.Count() > 0 && checkExpiredDate != null && checkExpiredDate.Count() > 0)
                            {
                                checkMaterial = checkPackageNumber.Where(o => checkExpiredDate.Select(p => p.ID).Contains(o.ID)).ToList();
                            }
                            else if (checkPackageNumber != null && checkPackageNumber.Count() > 0)
                            {
                                checkMaterial = checkPackageNumber;
                            }
                            else if (checkExpiredDate != null)
                            {
                                checkMaterial = checkExpiredDate;
                            }

                            if (checkMaterial != null && checkMaterial.Count() > 0)
                            {
                                item.KK_AMOUNT = checkMaterial[0].AMOUNT;
                            }
                        }

                        gridControlMaterial.BeginUpdate();
                        gridControlMaterial.DataSource = null;
                        gridControlMaterial.DataSource = ListMestPeriodMaty;
                        gridControlMaterial.EndUpdate();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void addServiceToProcessList(List<MestPeriodMediMateADO> _service, ref List<MestPeriodMediMateADO> _bedRoomRef)
        {
            try
            {
                _bedRoomRef = new List<MestPeriodMediMateADO>();
                long i = 0;
                foreach (var item in _service)
                {
                    i++;
                    string error = "";
                    var serAdo = new MestPeriodMediMateADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MestPeriodMediMateADO>(serAdo, item);

                    if (!string.IsNullOrEmpty(item.METY_MATY_CODE))
                    {
                        if (item.METY_MATY_CODE.Length > 25)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã thuốc/ vật tư");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã thuốc/ vật tư");
                    }

                    var checkTrung12 = _service.Where(p => p.METY_MATY_CODE == item.METY_MATY_CODE && p.IS_MEDICINE == item.IS_MEDICINE && p.PACKAGE_NUMBER == item.PACKAGE_NUMBER).ToList();
                    if (checkTrung12 != null && checkTrung12.Count > 1)
                    {
                        error += string.Format(Message.MessageImport.FileImportDaTonTai, item.METY_MATY_CODE);
                    }

                    if (!string.IsNullOrEmpty(item.AMOUNT_STR))
                    {
                        long amount = 0;
                        if (Int64.TryParse(item.AMOUNT_STR, out amount))
                        {
                            if (amount < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Số lượng kiểm kê (<0)");
                            }
                            else
                            {
                                serAdo.AMOUNT = amount;
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Số lượng kiểm kê");
                        }

                    }

                    if (!string.IsNullOrEmpty(item.EXPIRED_DATE_STR))
                    {
                        DateTime expiredDate;
                        if (DateTime.TryParse(item.EXPIRED_DATE_STR, out expiredDate))
                        {
                            if (expiredDate == null)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Hạn sử dụng");
                            }
                            else
                            {
                                serAdo.EXPIRED_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(expiredDate);
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Số lượng kiểm kê");
                        }

                    }
                    serAdo.ID = i;

                    _bedRoomRef.Add(serAdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}
