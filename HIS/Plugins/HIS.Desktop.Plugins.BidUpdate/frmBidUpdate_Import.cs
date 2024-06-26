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
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.BidUpdate
{
    public partial class frmBidUpdate : HIS.Desktop.Utility.FormBase
    {
        private List<ADO.MedicineTypeADO> ListAdoImport;

        private void BtnImport_Click(object sender, EventArgs e)
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
                            this.ListAdoImport = new List<ADO.MedicineTypeADO>();
                            var listMedicine = ImpMestListProcessor.Where(o => !String.IsNullOrWhiteSpace(o.IS_MEDICINE) && o.IS_MEDICINE.Trim().ToLower() == Base.GlobalConfig.IsMedicine.ToLower()).ToList();
                            var listMaterial = ImpMestListProcessor.Where(o => String.IsNullOrWhiteSpace(o.IS_MEDICINE)&&o.IsNotNullRow).ToList();
                            addListMedicineTypeToProcessList(listMedicine);
                            addListMaterialTypeToProcessList(listMaterial);

                            List<ADO.MedicineTypeADO> listError = new List<ADO.MedicineTypeADO>();
                            if (ListAdoImport != null && ListAdoImport.Count > 0)
                            {
                                foreach (var item in ListAdoImport)
                                {
                                    var same = ListMedicineTypeAdoProcess.Where(o => o.Type == item.Type && o.MEDICINE_TYPE_CODE == item.MEDICINE_TYPE_CODE).ToList();
                                    if (same != null && same.Count > 0) listError.Add(item);
                                    else ListMedicineTypeAdoProcess.Add(item);
                                }
                            }

                            if (listError != null && listError.Count > 0)
                            {
                                List<string> mess = new List<string>();
                                var medi = listError.Where(o => o.Type == Base.GlobalConfig.THUOC).ToList();
                                var mate = listError.Where(o => o.Type == Base.GlobalConfig.VATTU).ToList();
                                var blo = listError.Where(o => o.Type == Base.GlobalConfig.MAU).ToList();

                                if (medi != null && medi.Count > 0)
                                {
                                    var messageErr = String.Format(Resources.ResourceMessage.CanhBaoThuoc, string.Join(";", medi.Select(s => string.Format("{0}({1})", s.MEDICINE_TYPE_NAME, s.MEDICINE_TYPE_CODE))));
                                    messageErr += Resources.ResourceMessage.BiTrung;
                                    mess.Add(messageErr);
                                }

                                if (mate != null && mate.Count > 0)
                                {
                                    var messageErr = String.Format(Resources.ResourceMessage.CanhBaoVatTu, string.Join(";", mate.Select(s => string.Format("{0}({1})", s.MEDICINE_TYPE_NAME, s.MEDICINE_TYPE_CODE))));
                                    messageErr += Resources.ResourceMessage.BiTrung;
                                    mess.Add(messageErr);
                                }

                                if (blo != null && blo.Count > 0)
                                {
                                    var messageErr = String.Format(Resources.ResourceMessage.CanhBaoMau, string.Join(";", blo.Select(s => string.Format("{0}({1})", s.MEDICINE_TYPE_NAME, s.MEDICINE_TYPE_CODE))));
                                    messageErr += Resources.ResourceMessage.BiTrung;
                                    mess.Add(messageErr);
                                }

                                MessageBox.Show(string.Join(";", mess));
                            }

                            UpdateGrid();

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
                    #region new
                    var medicineTypeNotExist = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.MEDICINE_TYPE_CODE == medicineTypeImport.MEDICINE_TYPE_CODE);
                    var bloodTypeNotExist = BackendDataWorker.Get<V_HIS_BLOOD_TYPE>().FirstOrDefault(o => o.BLOOD_TYPE_CODE == medicineTypeImport.MEDICINE_TYPE_CODE);

                    var medicineType = new ADO.MedicineTypeADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(medicineType, medicineTypeImport);

                    if (medicineTypeNotExist == null && bloodTypeNotExist == null) continue;

                    if (medicineTypeNotExist != null)
                    {
                        medicineType.Type = Base.GlobalConfig.THUOC;
                        Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(medicineType, medicineTypeNotExist);
                    }
                    else if (bloodTypeNotExist != null)
                    {
                        medicineType.Type = Base.GlobalConfig.MAU;
                        Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(medicineType, bloodTypeNotExist);
                        medicineType.MEDICINE_TYPE_CODE = bloodTypeNotExist.BLOOD_TYPE_CODE;
                        medicineType.MEDICINE_TYPE_NAME = bloodTypeNotExist.BLOOD_TYPE_NAME;
                    }

                    medicineType.EXPIRED_DATE = medicineTypeImport.EXPIRED_DATE;
                    if (!String.IsNullOrWhiteSpace(medicineTypeImport.MEDICINE_USE_FORM_CODE))
                    {
                        HIS_MEDICINE_USE_FORM useForm = BackendDataWorker.Get<HIS_MEDICINE_USE_FORM>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.MEDICINE_USE_FORM_CODE == medicineTypeImport.MEDICINE_USE_FORM_CODE);
                        if (useForm != null)
                        {
                            medicineType.MEDICINE_USE_FORM_CODE = useForm.MEDICINE_USE_FORM_CODE;
                            medicineType.MEDICINE_USE_FORM_ID = useForm.ID;
                            medicineType.MEDICINE_USE_FORM_NAME = useForm.MEDICINE_USE_FORM_NAME;
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(medicineTypeImport.SUPPLIER_CODE))
                    {
                        var supplier = Base.GlobalConfig.ListSupplier.FirstOrDefault(o => o.SUPPLIER_CODE == medicineTypeImport.SUPPLIER_CODE);
                        if (supplier != null)
                        {
                            medicineType.SUPPLIER_ID = supplier.ID;
                            medicineType.SUPPLIER_CODE = supplier.SUPPLIER_CODE;
                            medicineType.SUPPLIER_NAME = supplier.SUPPLIER_NAME;
                        }
                    }

                    medicineType.IdRow = setIdRow(this.ListAdoImport);

                    medicineType.IMP_PRICE = Inventec.Common.TypeConvert.Parse.ToDecimal(medicineTypeImport.IMP_PRICE.ToString());
                    medicineType.AMOUNT = Inventec.Common.TypeConvert.Parse.ToDecimal(medicineTypeImport.AMOUNT.ToString());

                    if (medicineTypeImport.SERVICE_UNIT_CODE != null)
                    {
                        var serrviceUnit = Base.GlobalConfig.ListServiceUnit.FirstOrDefault(o => o.SERVICE_UNIT_CODE == medicineTypeImport.SERVICE_UNIT_CODE);
                        if (serrviceUnit != null)
                        {
                            medicineType.SERVICE_UNIT_ID = serrviceUnit != null ? serrviceUnit.ID : 0;
                            medicineType.SERVICE_UNIT_NAME = serrviceUnit != null ? serrviceUnit.SERVICE_UNIT_NAME : "";
                        }
                    }

                    if (medicineTypeImport.IMP_VAT_RATIO != null)
                    {
                        medicineType.ImpVatRatio = medicineTypeImport.IMP_VAT_RATIO;
                        medicineType.IMP_VAT_RATIO = medicineTypeImport.IMP_VAT_RATIO / 100;
                    }
                    else
                    {
                        medicineType.ImpVatRatio = 0;
                        medicineType.IMP_VAT_RATIO = 0;
                    }

                    if (!String.IsNullOrWhiteSpace(medicineTypeImport.BID_NUM_ORDER))
                        medicineType.BID_NUM_ORDER = medicineTypeImport.BID_NUM_ORDER;

                    if (!String.IsNullOrWhiteSpace(medicineTypeImport.BID_PACKAGE_CODE))
                        medicineType.BID_PACKAGE_CODE = medicineTypeImport.BID_PACKAGE_CODE;

                    if (!String.IsNullOrWhiteSpace(medicineTypeImport.BID_GROUP_CODE))
                        medicineType.BID_GROUP_CODE = medicineTypeImport.BID_GROUP_CODE;

                    if (!String.IsNullOrWhiteSpace(medicineTypeImport.HEIN_SERVICE_BHYT_NAME))
                        medicineType.HEIN_SERVICE_BHYT_NAME = medicineTypeImport.HEIN_SERVICE_BHYT_NAME;

                    if (!String.IsNullOrWhiteSpace(medicineTypeImport.PACKING_TYPE_NAME))
                        medicineType.PACKING_TYPE_NAME = medicineTypeImport.PACKING_TYPE_NAME;

                    if (!String.IsNullOrWhiteSpace(medicineTypeImport.REGISTER_NUMBER))
                        medicineType.REGISTER_NUMBER = medicineTypeImport.REGISTER_NUMBER;

                    if (!String.IsNullOrWhiteSpace(medicineTypeImport.CONCENTRA))
                        medicineType.CONCENTRA = medicineTypeImport.CONCENTRA;

                    if (!String.IsNullOrWhiteSpace(medicineTypeImport.NATIONAL_NAME))
                    {
                        var national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().FirstOrDefault(o => o.NATIONAL_NAME.ToUpper() == medicineTypeImport.NATIONAL_NAME.ToUpper());
                        if (national != null)
                        {
                            medicineType.NATIONAL_NAME = national.NATIONAL_NAME;
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(medicineTypeImport.MONTH_LIFESPAN_STR))
                    {
                        medicineType.MONTH_LIFESPAN_STR = medicineTypeImport.MONTH_LIFESPAN_STR;
                        long number = 0;
                        bool isValid = long.TryParse(medicineTypeImport.MONTH_LIFESPAN_STR, out number);
                        if (isValid && number < 19)
                        {
                            medicineType.MONTH_LIFESPAN = number;
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(medicineTypeImport.DAY_LIFESPAN_STR))
                    {
                        medicineType.DAY_LIFESPAN_STR = medicineTypeImport.DAY_LIFESPAN_STR;
                        long number = 0;
                        bool isValid = long.TryParse(medicineTypeImport.DAY_LIFESPAN_STR, out number);
                        if (isValid && number < 19)
                        {
                            medicineType.DAY_LIFESPAN = number;
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(medicineTypeImport.HOUR_LIFESPAN_STR))
                    {
                        medicineType.HOUR_LIFESPAN_STR = medicineTypeImport.HOUR_LIFESPAN_STR;
                        long number = 0;
                        bool isValid = long.TryParse(medicineTypeImport.HOUR_LIFESPAN_STR, out number);
                        if (isValid && number < 19)
                        {
                            medicineType.HOUR_LIFESPAN = number;
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(medicineTypeImport.BID_EXTRA_CODE))
                    {
                        medicineType.BID_EXTRA_CODE = medicineTypeImport.BID_EXTRA_CODE;
                    }

                    //if (!String.IsNullOrWhiteSpace(medicineType.MONTH_LIFESPAN.ToString()) && !String.IsNullOrWhiteSpace(medicineType.DAY_LIFESPAN.ToString()) ||
                    //    !String.IsNullOrWhiteSpace(medicineType.MONTH_LIFESPAN.ToString()) && !String.IsNullOrWhiteSpace(medicineType.HOUR_LIFESPAN.ToString()) ||
                    //    !String.IsNullOrWhiteSpace(medicineType.DAY_LIFESPAN.ToString()) && !String.IsNullOrWhiteSpace(medicineType.HOUR_LIFESPAN.ToString()))
                    //{
                    //    continue;
                    //}

                    if (!String.IsNullOrWhiteSpace(medicineTypeImport.MANUFACTURER_CODE))
                    {
                        var manufacturer = BackendDataWorker.Get<HIS_MANUFACTURER>().FirstOrDefault(o => o.MANUFACTURER_CODE.ToUpper() == medicineTypeImport.MANUFACTURER_CODE.ToUpper());
                        if (manufacturer != null)
                        {
                            medicineType.MANUFACTURER_ID = manufacturer.ID;
                            medicineType.MANUFACTURER_NAME = manufacturer.MANUFACTURER_NAME;
                            medicineType.MANUFACTURER_CODE = manufacturer.MANUFACTURER_CODE;
                        }
                    }

                    this.ListAdoImport.Insert(0, medicineType);
                    #endregion

                    #region old
                    //var medicineTypeNotExist = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.MEDICINE_TYPE_CODE == medicineTypeImport.MEDICINE_TYPE_CODE);
                    //var bloodTypeNotExist = BackendDataWorker.Get<V_HIS_BLOOD_TYPE>().FirstOrDefault(o => o.BLOOD_TYPE_CODE == medicineTypeImport.MEDICINE_TYPE_CODE);

                    //var medicineType = new ADO.MedicineTypeADO();
                    //Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(medicineType, medicineTypeImport);

                    //if (medicineTypeNotExist == null && bloodTypeNotExist == null) continue;

                    //if (medicineTypeNotExist != null)
                    //{
                    //    medicineType.Type = Base.GlobalConfig.THUOC;
                    //    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(medicineType, medicineTypeNotExist);
                    //    //medicineType.NATIONAL_NAME = medicineTypeNotExist.NATIONAL_NAME;
                    //    //medicineType.MANUFACTURER_NAME = medicineTypeNotExist.MANUFACTURER_NAME;
                    //    medicineType.EXPIRED_DATE = medicineTypeImport.EXPIRED_DATE;
                    //    if (!String.IsNullOrWhiteSpace(medicineTypeImport.MEDICINE_USE_FORM_CODE))
                    //    {
                    //        HIS_MEDICINE_USE_FORM useForm = BackendDataWorker.Get<HIS_MEDICINE_USE_FORM>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.MEDICINE_USE_FORM_CODE == medicineTypeImport.MEDICINE_USE_FORM_CODE);
                    //        if (useForm != null)
                    //        {
                    //            medicineType.MEDICINE_USE_FORM_CODE = useForm.MEDICINE_USE_FORM_CODE;
                    //            medicineType.MEDICINE_USE_FORM_ID = useForm.ID;
                    //            medicineType.MEDICINE_USE_FORM_NAME = useForm.MEDICINE_USE_FORM_NAME;
                    //        }
                    //    }
                    //}
                    //else if (bloodTypeNotExist != null)
                    //{
                    //    medicineType.Type = Base.GlobalConfig.MAU;
                    //    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(medicineType, bloodTypeNotExist);
                    //    medicineType.MEDICINE_TYPE_CODE = bloodTypeNotExist.BLOOD_TYPE_CODE;
                    //    medicineType.MEDICINE_TYPE_NAME = bloodTypeNotExist.BLOOD_TYPE_NAME;
                    //}

                    //if (medicineTypeImport.SUPPLIER_CODE != null)
                    //{
                    //    var supplier = Base.GlobalConfig.ListSupplier.FirstOrDefault(o => o.SUPPLIER_CODE == medicineTypeImport.SUPPLIER_CODE);
                    //    if (supplier != null)
                    //    {
                    //        medicineType.SUPPLIER_ID = supplier.ID;
                    //        medicineType.SUPPLIER_CODE = supplier.SUPPLIER_CODE;
                    //        medicineType.SUPPLIER_NAME = supplier.SUPPLIER_NAME;
                    //    }
                    //}

                    //medicineType.IdRow = setIdRow(this.ListAdoImport);
                    //medicineType.BID_NUM_ORDER = medicineTypeImport.BID_NUM_ORDER;
                    //medicineType.IMP_PRICE = Inventec.Common.TypeConvert.Parse.ToDecimal(medicineTypeImport.IMP_PRICE.ToString());
                    //medicineType.AMOUNT = Inventec.Common.TypeConvert.Parse.ToDecimal(medicineTypeImport.AMOUNT.ToString());
                    //if (medicineTypeImport.SERVICE_UNIT_CODE != null)
                    //{
                    //    var serrviceUnit = Base.GlobalConfig.ListServiceUnit.FirstOrDefault(o => o.SERVICE_UNIT_CODE == medicineTypeImport.SERVICE_UNIT_CODE);
                    //    if (serrviceUnit != null)
                    //    {
                    //        medicineType.SERVICE_UNIT_ID = serrviceUnit != null ? serrviceUnit.ID : 0;
                    //        medicineType.SERVICE_UNIT_NAME = serrviceUnit != null ? serrviceUnit.SERVICE_UNIT_NAME : "";
                    //    }
                    //}

                    //if (medicineTypeImport.IMP_VAT_RATIO != null)
                    //{
                    //    medicineType.ImpVatRatio = medicineTypeImport.IMP_VAT_RATIO;
                    //    medicineType.IMP_VAT_RATIO = medicineTypeImport.IMP_VAT_RATIO / 100;
                    //}
                    //else
                    //{
                    //    medicineType.ImpVatRatio = 0;
                    //    medicineType.IMP_VAT_RATIO = 0;
                    //}

                    //medicineType.BID_PACKAGE_CODE = medicineTypeImport.BID_PACKAGE_CODE;
                    //medicineType.BID_GROUP_CODE = medicineTypeImport.BID_GROUP_CODE;

                    //this.ListAdoImport.Insert(0, medicineType);
                    #endregion
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
                    #region new
                    var medicineType = new ADO.MedicineTypeADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(medicineType, materialTypeImport);

                    if (!String.IsNullOrWhiteSpace(materialTypeImport.MEDICINE_TYPE_CODE)
                        && !String.IsNullOrWhiteSpace(materialTypeImport.MATERIAL_TYPE_MAP_CODE))
                    {
                        continue;
                    }

                    if (String.IsNullOrWhiteSpace(materialTypeImport.MEDICINE_TYPE_CODE)
                        && String.IsNullOrWhiteSpace(materialTypeImport.MATERIAL_TYPE_MAP_CODE))
                    {
                        continue;
                    }

                    if (!String.IsNullOrWhiteSpace(materialTypeImport.MEDICINE_TYPE_CODE))
                    {
                        var materialTypeNotExist = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.MATERIAL_TYPE_CODE == materialTypeImport.MEDICINE_TYPE_CODE);
                        if (materialTypeNotExist != null)
                        {
                            Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(medicineType, materialTypeNotExist);
                            medicineType.MEDICINE_TYPE_CODE = materialTypeNotExist.MATERIAL_TYPE_CODE;
                            medicineType.MEDICINE_TYPE_NAME = materialTypeNotExist.MATERIAL_TYPE_NAME;
                        }
                    }
                    else
                    {
                        LogSystem.Debug("MATERIAL_TYPE_MAP_CODE: " + materialTypeImport.MATERIAL_TYPE_MAP_CODE);
                        var materialTypeMap = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.MATERIAL_TYPE_MAP_CODE == materialTypeImport.MATERIAL_TYPE_MAP_CODE);
                        if (materialTypeMap != null)
                        {
                            medicineType.ID = materialTypeMap.MATERIAL_TYPE_MAP_ID.Value;
                            medicineType.MEDICINE_TYPE_CODE = materialTypeMap.MATERIAL_TYPE_MAP_CODE;
                            medicineType.MEDICINE_TYPE_NAME = materialTypeMap.MATERIAL_TYPE_MAP_NAME;
                            medicineType.IsMaterialTypeMap = true;
                            medicineType.SERVICE_UNIT_ID = materialTypeMap.SERVICE_UNIT_ID;
                            medicineType.SERVICE_UNIT_CODE = materialTypeMap.SERVICE_UNIT_CODE;
                            medicineType.SERVICE_UNIT_NAME = materialTypeMap.SERVICE_UNIT_NAME;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    medicineType.EXPIRED_DATE = materialTypeImport.EXPIRED_DATE;

                    if (!String.IsNullOrWhiteSpace(materialTypeImport.SUPPLIER_CODE))
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
                    medicineType.IdRow = setIdRow(this.ListAdoImport);
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

                    if (materialTypeImport.IMP_VAT_RATIO != null)
                    {
                        medicineType.ImpVatRatio = materialTypeImport.IMP_VAT_RATIO;
                        medicineType.IMP_VAT_RATIO = materialTypeImport.IMP_VAT_RATIO / 100;
                    }
                    else
                    {
                        medicineType.ImpVatRatio = 0;
                        medicineType.IMP_VAT_RATIO = 0;
                    }

                    if (!String.IsNullOrWhiteSpace(materialTypeImport.BID_NUM_ORDER))
                        medicineType.BID_NUM_ORDER = materialTypeImport.BID_NUM_ORDER;

                    if (!String.IsNullOrWhiteSpace(materialTypeImport.BID_PACKAGE_CODE))
                        medicineType.BID_PACKAGE_CODE = materialTypeImport.BID_PACKAGE_CODE;

                    if (!String.IsNullOrWhiteSpace(materialTypeImport.BID_GROUP_CODE))
                        medicineType.BID_GROUP_CODE = materialTypeImport.BID_GROUP_CODE;

                    if (!String.IsNullOrWhiteSpace(materialTypeImport.BID_MATERIAL_TYPE_CODE))
                        medicineType.BID_MATERIAL_TYPE_CODE = materialTypeImport.BID_MATERIAL_TYPE_CODE;

                    if (!String.IsNullOrWhiteSpace(materialTypeImport.BID_MATERIAL_TYPE_NAME))
                        medicineType.BID_MATERIAL_TYPE_NAME = materialTypeImport.BID_MATERIAL_TYPE_NAME;

                    if (!String.IsNullOrWhiteSpace(materialTypeImport.JOIN_BID_MATERIAL_TYPE_CODE))
                        medicineType.JOIN_BID_MATERIAL_TYPE_CODE = materialTypeImport.JOIN_BID_MATERIAL_TYPE_CODE;

                    if (!String.IsNullOrWhiteSpace(materialTypeImport.CONCENTRA))
                        medicineType.CONCENTRA = materialTypeImport.CONCENTRA;

                    if (!String.IsNullOrWhiteSpace(materialTypeImport.NATIONAL_NAME))
                    {
                        var national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().FirstOrDefault(o => o.NATIONAL_NAME.ToUpper() == materialTypeImport.NATIONAL_NAME.ToUpper());
                        if (national != null)
                        {
                            medicineType.NATIONAL_NAME = national.NATIONAL_NAME;
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(materialTypeImport.MONTH_LIFESPAN_STR))
                    {
                        medicineType.MONTH_LIFESPAN_STR = materialTypeImport.MONTH_LIFESPAN_STR;
                        long number = 0;
                        bool isValid = long.TryParse(materialTypeImport.MONTH_LIFESPAN_STR, out number);
                        if (isValid && number < 19)
                        {
                            medicineType.MONTH_LIFESPAN = number;
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(materialTypeImport.DAY_LIFESPAN_STR))
                    {
                        medicineType.DAY_LIFESPAN_STR = materialTypeImport.DAY_LIFESPAN_STR;
                        long number = 0;
                        bool isValid = long.TryParse(materialTypeImport.DAY_LIFESPAN_STR, out number);
                        if (isValid && number < 19)
                        {
                            medicineType.DAY_LIFESPAN = number;
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(materialTypeImport.HOUR_LIFESPAN_STR))
                    {
                        medicineType.HOUR_LIFESPAN_STR = materialTypeImport.HOUR_LIFESPAN_STR;
                        long number = 0;
                        bool isValid = long.TryParse(materialTypeImport.HOUR_LIFESPAN_STR, out number);
                        if (isValid && number < 19)
                        {
                            medicineType.HOUR_LIFESPAN = number;
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(medicineType.MONTH_LIFESPAN.ToString()) && !String.IsNullOrWhiteSpace(medicineType.DAY_LIFESPAN.ToString()) ||
                        !String.IsNullOrWhiteSpace(medicineType.MONTH_LIFESPAN.ToString()) && !String.IsNullOrWhiteSpace(medicineType.HOUR_LIFESPAN.ToString()) ||
                        !String.IsNullOrWhiteSpace(medicineType.DAY_LIFESPAN.ToString()) && !String.IsNullOrWhiteSpace(medicineType.HOUR_LIFESPAN.ToString()))
                    {
                        continue;
                    }

                    if (!String.IsNullOrWhiteSpace(materialTypeImport.MANUFACTURER_CODE))
                    {
                        var manufacturer = BackendDataWorker.Get<HIS_MANUFACTURER>().FirstOrDefault(o => o.MANUFACTURER_CODE.ToUpper() == materialTypeImport.MANUFACTURER_CODE.ToUpper());
                        if (manufacturer != null)
                        {
                            medicineType.MANUFACTURER_ID = manufacturer.ID;
                            medicineType.MANUFACTURER_NAME = manufacturer.MANUFACTURER_NAME;
                            medicineType.MANUFACTURER_CODE = manufacturer.MANUFACTURER_CODE;
                        }
                    }
                    if (!String.IsNullOrWhiteSpace(materialTypeImport.BID_EXTRA_CODE))
                    {
                        medicineType.BID_EXTRA_CODE = materialTypeImport.BID_EXTRA_CODE;
                    }

                    this.ListAdoImport.Insert(0, medicineType);
                    #endregion

                    #region old
                    //if (!String.IsNullOrWhiteSpace(materialTypeImport.MEDICINE_TYPE_CODE)
                    //    && !String.IsNullOrWhiteSpace(materialTypeImport.MATERIAL_TYPE_MAP_CODE))
                    //{
                    //    continue;
                    //}
                    //if (String.IsNullOrWhiteSpace(materialTypeImport.MEDICINE_TYPE_CODE)
                    //    && String.IsNullOrWhiteSpace(materialTypeImport.MATERIAL_TYPE_MAP_CODE))
                    //{
                    //    continue;
                    //}

                    //var medicineType = new ADO.MedicineTypeADO();
                    //Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(medicineType, materialTypeImport);

                    //if (!String.IsNullOrWhiteSpace(materialTypeImport.MEDICINE_TYPE_CODE))
                    //{
                    //    var materialTypeNotExist = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.MATERIAL_TYPE_CODE == materialTypeImport.MEDICINE_TYPE_CODE);
                    //    if (materialTypeNotExist == null) continue;
                    //    else
                    //    {
                    //        Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(medicineType, materialTypeNotExist);
                    //        medicineType.MEDICINE_TYPE_CODE = materialTypeNotExist.MATERIAL_TYPE_CODE;
                    //        medicineType.MEDICINE_TYPE_NAME = materialTypeNotExist.MATERIAL_TYPE_NAME;
                    //    }
                    //}
                    //else
                    //{
                    //    LogSystem.Debug("MATERIAL_TYPE_MAP_CODE: " + materialTypeImport.MATERIAL_TYPE_MAP_CODE);
                    //    var materialTypeMap = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.MATERIAL_TYPE_MAP_CODE == materialTypeImport.MATERIAL_TYPE_MAP_CODE);
                    //    if (materialTypeMap == null)
                    //    {
                    //        continue;
                    //    }
                    //    else
                    //    {
                    //        medicineType.ID = materialTypeMap.MATERIAL_TYPE_MAP_ID.Value;
                    //        medicineType.MEDICINE_TYPE_CODE = materialTypeMap.MATERIAL_TYPE_MAP_CODE;
                    //        medicineType.MEDICINE_TYPE_NAME = materialTypeMap.MATERIAL_TYPE_MAP_NAME;
                    //        medicineType.IsMaterialTypeMap = true;
                    //        medicineType.SERVICE_UNIT_ID = materialTypeMap.SERVICE_UNIT_ID;
                    //        medicineType.SERVICE_UNIT_CODE = materialTypeMap.SERVICE_UNIT_CODE;
                    //        medicineType.SERVICE_UNIT_NAME = materialTypeMap.SERVICE_UNIT_NAME;
                    //    }
                    //}

                    //if (materialTypeImport.SUPPLIER_CODE != null)
                    //{
                    //    var supplier = Base.GlobalConfig.ListSupplier.FirstOrDefault(o => o.SUPPLIER_CODE == materialTypeImport.SUPPLIER_CODE);
                    //    if (supplier != null)
                    //    {
                    //        medicineType.SUPPLIER_ID = supplier.ID;
                    //        medicineType.SUPPLIER_CODE = supplier.SUPPLIER_CODE;
                    //        medicineType.SUPPLIER_NAME = supplier.SUPPLIER_NAME;
                    //    }
                    //}
                    //medicineType.Type = Base.GlobalConfig.VATTU;
                    //medicineType.IdRow = setIdRow(this.ListAdoImport);
                    //medicineType.BID_NUM_ORDER = materialTypeImport.BID_NUM_ORDER;
                    //medicineType.IMP_PRICE = materialTypeImport.IMP_PRICE;
                    //medicineType.AMOUNT = materialTypeImport.AMOUNT;
                    //if (materialTypeImport.SERVICE_UNIT_CODE != null)
                    //{
                    //    var serrviceUnit = Base.GlobalConfig.ListServiceUnit.FirstOrDefault(o => o.SERVICE_UNIT_CODE == materialTypeImport.SERVICE_UNIT_CODE);
                    //    if (serrviceUnit != null)
                    //    {
                    //        medicineType.SERVICE_UNIT_ID = serrviceUnit != null ? serrviceUnit.ID : 0;
                    //        medicineType.SERVICE_UNIT_NAME = serrviceUnit != null ? serrviceUnit.SERVICE_UNIT_NAME : "";
                    //    }
                    //}

                    //if (materialTypeImport.IMP_VAT_RATIO != null)
                    //{
                    //    medicineType.ImpVatRatio = materialTypeImport.IMP_VAT_RATIO;
                    //    medicineType.IMP_VAT_RATIO = materialTypeImport.IMP_VAT_RATIO / 100;
                    //}
                    //else
                    //{
                    //    medicineType.ImpVatRatio = 0;
                    //    medicineType.IMP_VAT_RATIO = 0;
                    //}

                    //medicineType.BID_PACKAGE_CODE = materialTypeImport.BID_PACKAGE_CODE;
                    //medicineType.BID_GROUP_CODE = materialTypeImport.BID_GROUP_CODE;
                    //medicineType.EXPIRED_DATE = materialTypeImport.EXPIRED_DATE;

                    //this.ListAdoImport.Insert(0, medicineType);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
