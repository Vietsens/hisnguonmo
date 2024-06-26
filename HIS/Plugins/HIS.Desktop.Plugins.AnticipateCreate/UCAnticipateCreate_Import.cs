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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Core;
using Inventec.Desktop.Common.Message;

namespace HIS.Desktop.Plugins.AnticipateCreate
{
    public partial class UCAnticipateCreate :HIS.Desktop.Utility.UserControlBase
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
                        var ImpMestListProcessor = import.Get<ADO.MedicineTypeADO>(0);
                        if (ImpMestListProcessor != null && ImpMestListProcessor.Count > 0)
                        {
                            this.ListMedicineTypeAdoProcess = new List<ADO.MedicineTypeADO>();
                            var listMedicine = ImpMestListProcessor.Where(o => !String.IsNullOrWhiteSpace(o.IS_MEDICINE) && o.IS_MEDICINE.Trim().ToLower() == Base.GlobalConfig.IsMedicine.ToLower()).ToList();
                            var listMaterial = ImpMestListProcessor.Where(o => String.IsNullOrWhiteSpace(o.IS_MEDICINE)).ToList();
                            addListMedicineTypeToProcessList(listMedicine);
                            addListMaterialTypeToProcessList(listMaterial);

                            gridControlProcess.BeginUpdate();
                            gridControlProcess.DataSource = null;
                            gridControlProcess.DataSource = this.ListMedicineTypeAdoProcess;
                            gridControlProcess.EndUpdate();
                            WaitingManager.Hide();
                        }
                        else
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.HeThongTBKQXLYCCuaFrontendThatBai, Resources.ResourceMessage.ThongBao, DevExpress.Utils.DefaultBoolean.True);
                        }
                    }
                    else
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.HeThongTBKQXLYCCuaFrontendThatBai, Resources.ResourceMessage.ThongBao, DevExpress.Utils.DefaultBoolean.True);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //thêm thuốc, máu (từ chức năng import dl) vào danh sách xử lý 
        private void addListMedicineTypeToProcessList(List<ADO.MedicineTypeADO> medicineTypeImports)
        {
            try
            {
                if (medicineTypeImports == null || medicineTypeImports.Count == 0)
                    return;

                foreach (var medicineTypeImport in medicineTypeImports)
                {
                    var medicineTypeNotExist = Base.GlobalConfig.HisMedicineTypes.FirstOrDefault(o => o.MEDICINE_TYPE_CODE == medicineTypeImport.MEDICINE_TYPE_CODE);
                    var bloodTypeNotExist = Base.GlobalConfig.HisBloodTypes.FirstOrDefault(o => o.BLOOD_TYPE_CODE == medicineTypeImport.MEDICINE_TYPE_CODE);

                    this.medicineType = new ADO.MedicineTypeADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(this.medicineType, medicineTypeImport);

                    if (medicineTypeNotExist == null && bloodTypeNotExist == null) continue;

                    if (medicineTypeNotExist != null)
                    {
                        medicineType.Type = Base.GlobalConfig.THUOC;
                        Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(this.medicineType, medicineTypeNotExist);
                        medicineType.NATIONAL_NAME = medicineTypeNotExist.NATIONAL_NAME;
                        medicineType.MANUFACTURER_NAME = medicineTypeNotExist.MANUFACTURER_NAME;
                    }
                    else if (bloodTypeNotExist != null)
                    {
                        medicineType.Type = Base.GlobalConfig.MAU;
                        Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(this.medicineType, bloodTypeNotExist);
                        medicineType.MEDICINE_TYPE_CODE = bloodTypeNotExist.BLOOD_TYPE_CODE;
                        medicineType.MEDICINE_TYPE_NAME = bloodTypeNotExist.BLOOD_TYPE_NAME;
                    }

                    if (medicineTypeImport.SUPPLIER_CODE != null)
                    {
                        var supplier = Base.GlobalConfig.ListSupplier.FirstOrDefault(o => o.SUPPLIER_CODE == medicineTypeImport.SUPPLIER_CODE);
                        if (supplier != null)
                        {
                            medicineType.SUPPLIER_ID = supplier.ID;
                            medicineType.SUPPLIER_CODE = supplier.SUPPLIER_CODE;
                            medicineType.SUPPLIER_NAME = supplier.SUPPLIER_NAME;
                        }
                    }

                    medicineType.IdRow = setIdRow(this.ListMedicineTypeAdoProcess);
                    medicineType.IMP_PRICE = medicineTypeImport.IMP_PRICE;
                    medicineType.AMOUNT = medicineTypeImport.AMOUNT;
                    if (medicineTypeImport.SERVICE_UNIT_CODE != null)
                    {
                        var serrviceUnit = Base.GlobalConfig.ListServiceUnit.FirstOrDefault(o => o.SERVICE_UNIT_CODE == medicineTypeImport.SERVICE_UNIT_CODE);
                        if (serrviceUnit != null)
                        {
                            medicineType.SERVICE_UNIT_ID = serrviceUnit != null ? serrviceUnit.ID : 0;
                            medicineType.SERVICE_UNIT_NAME = serrviceUnit != null ? serrviceUnit.SERVICE_UNIT_NAME : "";
                        }
                    }

                    this.ListMedicineTypeAdoProcess.Insert(0, this.medicineType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        //thêm vật tư (từ chức năng import dl) vào danh sách xử lý 
        private void addListMaterialTypeToProcessList(List<ADO.MedicineTypeADO> materialTypeImports)
        {
            try
            {
                if (materialTypeImports == null || materialTypeImports.Count == 0)
                    return;

                foreach (var materialTypeImport in materialTypeImports)
                {
                    var materialTypeNotExist = Base.GlobalConfig.HisMaterialTypes.FirstOrDefault(o => o.MATERIAL_TYPE_CODE == materialTypeImport.MEDICINE_TYPE_CODE);

                    this.medicineType = new ADO.MedicineTypeADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(this.medicineType, materialTypeImport);
                    if (materialTypeNotExist == null) continue;
                    else
                    {
                        Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(this.medicineType, materialTypeNotExist);
                        medicineType.MEDICINE_TYPE_CODE = materialTypeNotExist.MATERIAL_TYPE_CODE;
                        medicineType.MEDICINE_TYPE_NAME = materialTypeNotExist.MATERIAL_TYPE_NAME;
                    }

                    if (materialTypeImport.SUPPLIER_CODE != null)
                    {
                        var supplier = Base.GlobalConfig.ListSupplier.FirstOrDefault(o => o.SUPPLIER_CODE == materialTypeImport.SUPPLIER_CODE);
                        if (supplier != null)
                        {
                            medicineType.SUPPLIER_ID = supplier.ID;
                            medicineType.SUPPLIER_CODE = supplier.SUPPLIER_CODE;
                            medicineType.SUPPLIER_NAME = supplier.SUPPLIER_NAME;
                        }
                    }

                    medicineType.Type = Base.GlobalConfig.VATTU;
                    medicineType.IdRow = setIdRow(this.ListMedicineTypeAdoProcess);
                    medicineType.IMP_PRICE = materialTypeImport.IMP_PRICE;
                    medicineType.AMOUNT = materialTypeImport.AMOUNT;
                    if (materialTypeImport.SERVICE_UNIT_CODE != null)
                    {
                        var serrviceUnit = Base.GlobalConfig.ListServiceUnit.FirstOrDefault(o => o.SERVICE_UNIT_CODE == materialTypeImport.SERVICE_UNIT_CODE);
                        if (serrviceUnit != null)
                        {
                            medicineType.SERVICE_UNIT_ID = serrviceUnit != null ? serrviceUnit.ID : 0;
                            medicineType.SERVICE_UNIT_NAME = serrviceUnit != null ? serrviceUnit.SERVICE_UNIT_NAME : "";
                        }
                    }

                    this.ListMedicineTypeAdoProcess.Insert(0, this.medicineType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}
