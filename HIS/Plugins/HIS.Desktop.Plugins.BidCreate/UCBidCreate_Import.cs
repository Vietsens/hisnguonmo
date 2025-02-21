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
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.BidCreate.Forms;
using HIS.UC.MaterialType;
using HIS.UC.MedicineType;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.BidCreate
{
    public partial class UCBidCreate : HIS.Desktop.Utility.UserControlBase
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
                        this.listErrorImport = new List<ADO.MedicineTypeADO>();
                        var ImpMestListProcessor = import.Get<ADO.MedicineTypeADO>(0);
                        if (ImpMestListProcessor != null && ImpMestListProcessor.Count > 0)
                        {
                            var bidType = bidTypes.FirstOrDefault(o => o.BID_TYPE_CODE == ImpMestListProcessor.First().BID_TYPE_CODE);
                            if (bidType != null)
                            {
                                cboBidType.EditValue = bidType.ID;
                            }
                            this.ListMedicineTypeAdoProcess = new List<ADO.MedicineTypeADO>();
                            var listMedicine = ImpMestListProcessor.Where(o => !String.IsNullOrWhiteSpace(o.IS_MEDICINE) && o.IS_MEDICINE.Trim().ToLower() == Base.GlobalConfig.IsMedicine.ToLower()).ToList();
                            var listMaterial = ImpMestListProcessor.Where(o => String.IsNullOrWhiteSpace(o.IS_MEDICINE)&&o.IsNotNullRow).ToList();

                            this.listErrorImport = ImpMestListProcessor.Where(o => !String.IsNullOrWhiteSpace(o.IS_MEDICINE) && o.IS_MEDICINE.Trim().ToLower() != Base.GlobalConfig.IsMedicine.ToLower()).ToList();

                            if (this.listErrorImport != null && this.listErrorImport.Count > 0)
                            {
                                this.listErrorImport.ForEach(o => o.ErrorDescriptions.Add("Không xác định được thuốc hay vật tư"));
                            }

                            if (this.listErrorImport == null) this.listErrorImport = new List<ADO.MedicineTypeADO>();

                            addListMedicineTypeToProcessList(listMedicine);
                            addListMaterialTypeToProcessList(listMaterial);
                            LogSystem.Debug(LogUtil.TraceData(LogUtil.GetMemberName(() => ListMedicineTypeAdoProcess), ListMedicineTypeAdoProcess));

                            gridControlProcess.BeginUpdate();
                            gridControlProcess.DataSource = null;
                            gridControlProcess.DataSource = this.ListMedicineTypeAdoProcess;
                            gridControlProcess.EndUpdate();

                            var bid = ImpMestListProcessor.First();
                            txtBidApThauCode.Text = bid.BID_APTHAU_CODE;
                            txtBidName.Text = bid.BID_NAME;
                            txtBidNumber.Text = bid.BID_NUMBER;
                            txtBID.Text = bid.BID_EXTRA_CODE;

                            string ToTime = null;
                            foreach (var item in ImpMestListProcessor)
                            {
                                ToTime = item.VALID_TO_TIME;

                                if (!string.IsNullOrEmpty(ToTime))
                                {
                                    break;
                                }
                            }

                            string FromTime = null;
                            foreach (var item in ImpMestListProcessor)
                            {
                                FromTime = item.VALID_FROM_TIME;

                                if (!string.IsNullOrEmpty(ToTime))
                                {
                                    break;
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(FromTime) && 
                                DateTime.TryParseExact(FromTime, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateFromValue))
                            {
                                dtFromTime.DateTime = dateFromValue;
                            }
                            else
                            {
                                dtFromTime.EditValue = null;
                            }

                            if (!string.IsNullOrWhiteSpace(ToTime) &&
                                DateTime.TryParseExact(ToTime, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateToValue))
                            {
                                dtToTime.DateTime = dateToValue;
                            }
                            else
                            {
                                dtToTime.EditValue = null;
                            }

                            if (!String.IsNullOrEmpty(bid.BID_YEAR))
                            {
                                bool valid = true;

                                if (bid.BID_YEAR.Length > 4)
                                {
                                    valid = false;
                                }
                                foreach (char item in bid.BID_YEAR)
                                {
                                    if (!Char.IsDigit(item))
                                    {
                                        valid = false;
                                        break;
                                    }
                                }
                                if (valid)
                                {
                                    txtBidYear.Text = bid.BID_YEAR;
                                }


                                else
                                {
                                    MessageBox.Show(Resources.ResourceMessage.SaiDinhDangNam);
                                    txtBidYear.Focus();
                                    txtBidYear.SelectAll();
                                }
                            }
                            WaitingManager.Hide();
                            if (this.listErrorImport != null && this.listErrorImport.Count > 0)
                            {
                                frmImportError frm = new frmImportError(this.listErrorImport);
                                frm.ShowDialog();
                            }
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

                LogSystem.Debug(LogUtil.TraceData("medicineTypeImports.Count", medicineTypeImports.Count));
                foreach (var medicineTypeImport in medicineTypeImports)
                {
                    var medicineTypeNotExist = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.MEDICINE_TYPE_CODE == medicineTypeImport.MEDICINE_TYPE_CODE);
                    var bloodTypeNotExist = BackendDataWorker.Get<V_HIS_BLOOD_TYPE>().FirstOrDefault(o => o.BLOOD_TYPE_CODE == medicineTypeImport.MEDICINE_TYPE_CODE);

                    var medicineType = new ADO.MedicineTypeADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(medicineType, medicineTypeImport);

                    if (!string.IsNullOrWhiteSpace(medicineType.VALID_FROM_TIME) &&
                        !DateTime.TryParseExact(medicineType.VALID_FROM_TIME, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                    {
                        medicineType.ErrorDescriptions.Add("Định dạng thời gian từ không hợp lệ.");
                    }
                    else
                    {
                        medicineType.VALID_FROM_TIME = medicineTypeImport.VALID_FROM_TIME;
                    }     

                    if (!string.IsNullOrWhiteSpace(medicineType.VALID_TO_TIME) &&
                        !DateTime.TryParseExact(medicineType.VALID_TO_TIME, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                    {
                        medicineType.ErrorDescriptions.Add("Định dạng thời gian đến không hợp lệ.");
                    }
                    else
                    {
                        medicineType.VALID_TO_TIME = medicineTypeImport.VALID_TO_TIME;
                    }

                    if (DateTime.TryParseExact(medicineType.VALID_FROM_TIME, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateFromValue) &&
                        DateTime.TryParseExact(medicineType.VALID_TO_TIME, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateToValue) )
                    {
                        long? fromValue = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dateFromValue);
                        long? toValue = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dateToValue);

                        if (fromValue > toValue)
                        {
                            medicineType.ErrorDescriptions.Add("Thời gian từ không được lớn hơn thời gian đến");
                        }
                    }


                    if (medicineTypeNotExist == null && bloodTypeNotExist == null)
                    {
                        medicineType.ErrorDescriptions.Add("Mã thuốc không chính xác");
                        medicineType.Type = Base.GlobalConfig.THUOC;
                        //this.listErrorImport.Add(medicineType);
                    }
                    else if (medicineTypeNotExist != null)
                    {
                        medicineType.Type = Base.GlobalConfig.THUOC;
                        Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(medicineType, medicineTypeNotExist);
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
                            else
                            {
                                medicineType.ErrorDescriptions.Add("Mã đường dùng không chính xác");
                            }
                        }
                        //this.ListMedicineTypeAdoProcess.Insert(0, medicineType);
                    }
                    else if (bloodTypeNotExist != null)
                    {
                        medicineType.Type = Base.GlobalConfig.MAU;
                        Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(medicineType, bloodTypeNotExist);
                        medicineType.MEDICINE_TYPE_CODE = bloodTypeNotExist.BLOOD_TYPE_CODE;
                        medicineType.MEDICINE_TYPE_NAME = bloodTypeNotExist.BLOOD_TYPE_NAME;
                        //this.ListMedicineTypeAdoProcess.Insert(0, medicineType);
                    }

                    if (String.IsNullOrWhiteSpace(medicineTypeImport.SUPPLIER_CODE))
                    {
                        medicineType.ErrorDescriptions.Add("Không có mã nhà thầu");
                    }
                    else
                    {
                        var supplier = Base.GlobalConfig.ListSupplier.FirstOrDefault(o => o.SUPPLIER_CODE == medicineTypeImport.SUPPLIER_CODE);
                        if (supplier != null)
                        {
                            medicineType.SUPPLIER_ID = supplier.ID;
                            medicineType.SUPPLIER_CODE = supplier.SUPPLIER_CODE;
                            medicineType.SUPPLIER_NAME = supplier.SUPPLIER_NAME;
                        }
                        else
                        {
                            medicineType.ErrorDescriptions.Add("Mã nhà thầu không chính xác");
                        }
                    }

                    medicineType.IdRow = setIdRow(this.ListMedicineTypeAdoProcess);
                    medicineType.BID_NUM_ORDER = medicineTypeImport.BID_NUM_ORDER;

                    if (String.IsNullOrWhiteSpace(medicineType.BID_NUM_ORDER))
                    {
                        medicineType.ErrorDescriptions.Add("Không có số thứ tự thầu");
                    }
                    else if (Encoding.UTF8.GetByteCount(medicineType.BID_NUM_ORDER) > 50)
                    {
                        medicineType.ErrorDescriptions.Add("Số thứ tự thầu quá độ dài cho phép (50)");
                    }

                    medicineType.IMP_PRICE = Inventec.Common.TypeConvert.Parse.ToDecimal(medicineTypeImport.IMP_PRICE.ToString());
                    if ((medicineType.IMP_PRICE ?? 0) < 0)
                    {
                        medicineType.ErrorDescriptions.Add("Giá nhập phải lớn hơn hoặc bằng 0");
                    }

                    medicineType.AMOUNT = Inventec.Common.TypeConvert.Parse.ToDecimal(medicineTypeImport.AMOUNT.ToString());
                    if ((medicineType.AMOUNT ?? 0) <= 0)
                    {
                        medicineType.ErrorDescriptions.Add("Số lượng nhập phải lớn hơn 0");
                    }

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

                    if ((medicineType.IMP_VAT_RATIO ?? 0) < 0 || (medicineType.IMP_VAT_RATIO ?? 0) > 1)
                    {
                        medicineType.ErrorDescriptions.Add("Vat phải nằm trong khoảng 0 - 100");
                    }

                    medicineType.BID_PACKAGE_CODE = medicineTypeImport.BID_PACKAGE_CODE;

                    if (string.IsNullOrEmpty(medicineType.BID_PACKAGE_CODE))
                    {
                        medicineType.ErrorDescriptions.Add("Không có mã gói thầu");
                    }
                    else if (Encoding.UTF8.GetByteCount(medicineType.BID_PACKAGE_CODE) > 4)
                    {
                        medicineType.ErrorDescriptions.Add("Mã gói thầu lớn hơn 4 ký tự");
                    }

                    medicineType.BID_EXTRA_CODE = medicineTypeImport.BID_EXTRA_CODE;
                    if (String.IsNullOrWhiteSpace(medicineType.BID_EXTRA_CODE))
                    {
                        medicineType.ErrorDescriptions.Add("Mã quyết định thầu không hợp lệ");
                    }
                    else if (Encoding.UTF8.GetByteCount(medicineType.BID_EXTRA_CODE) > 50)
                    {
                        medicineType.ErrorDescriptions.Add("Mã quyết định thầu vượt quá độ dài cho phép (50)");
                    }

                    medicineType.BID_GROUP_CODE = medicineTypeImport.BID_GROUP_CODE;
                    if (!string.IsNullOrEmpty(medicineType.BID_GROUP_CODE) && Encoding.UTF8.GetByteCount(medicineType.BID_GROUP_CODE) > 4)
                    {
                        medicineType.ErrorDescriptions.Add("Mã nhóm thầu phải là 4 ký tự");
                    }
                    if (!String.IsNullOrWhiteSpace(medicineType.IS_MEDICINE))
                    {
                        medicineType.HEIN_SERVICE_BHYT_NAME = medicineTypeImport.HEIN_SERVICE_BHYT_NAME;
                        //if (String.IsNullOrWhiteSpace(medicineType.HEIN_SERVICE_BHYT_NAME))
                        //{
                        //    medicineType.ErrorDescriptions.Add("Không có tên BHYT");
                        //}
                        if (Encoding.UTF8.GetByteCount(medicineType.HEIN_SERVICE_BHYT_NAME) > 500)
                        {
                            medicineType.ErrorDescriptions.Add("Tên BHYT vượt quá độ dài cho phép (500)");
                        }

                        medicineType.PACKING_TYPE_NAME = medicineTypeImport.PACKING_TYPE_NAME;
                        //if (String.IsNullOrWhiteSpace(medicineType.PACKING_TYPE_NAME))
                        //{
                        //    medicineType.ErrorDescriptions.Add("Không có QCĐG");
                        //}
                        if (Encoding.UTF8.GetByteCount(medicineType.PACKING_TYPE_NAME) > 300)
                        {
                            medicineType.ErrorDescriptions.Add("QCĐG vượt quá độ dài cho phép (300)");
                        }

                        medicineType.REGISTER_NUMBER = medicineTypeImport.REGISTER_NUMBER;
                        if (String.IsNullOrWhiteSpace(medicineType.REGISTER_NUMBER))
                        {
                            medicineType.ErrorDescriptions.Add("Không có số đăng kí");
                        }
                        else if (Encoding.UTF8.GetByteCount(medicineType.REGISTER_NUMBER) > 500)
                        {
                            medicineType.ErrorDescriptions.Add("Tên BHYT vượt quá độ dài cho phép (500)");
                        }
                    }

                    medicineType.CONCENTRA = medicineTypeImport.CONCENTRA;
                    //if (String.IsNullOrWhiteSpace(medicineType.CONCENTRA))
                    //{
                    //    medicineType.ErrorDescriptions.Add("Không có nồng độ/hàm lượng");
                    //}
                    //else 
                        if (Encoding.UTF8.GetByteCount(medicineType.CONCENTRA) > 1000)
                    {
                        medicineType.ErrorDescriptions.Add("Nồng độ/hàm lượng vượt quá độ dài cho phép (1000)");
                    }

                    if (!String.IsNullOrWhiteSpace(medicineTypeImport.NATIONAL_NAME))
                    {
                        var national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().FirstOrDefault(o => o.NATIONAL_NAME.ToUpper() == medicineTypeImport.NATIONAL_NAME.ToUpper());

                        if (national == null)
                        {
                            medicineType.ErrorDescriptions.Add("Tên nước sản xuất không chính xác");
                        }
                        else
                        {
                            medicineType.NATIONAL_NAME = national.NATIONAL_NAME;
                        }
                    }
                    else if (Encoding.UTF8.GetByteCount(medicineTypeImport.NATIONAL_NAME) > 100)
                    {
                        medicineType.ErrorDescriptions.Add("Nước sản xuất vượt quá độ dài cho phép (100)");
                    }
                    //else
                    //{
                    //    medicineType.ErrorDescriptions.Add("Không có tên nước sản xuất");
                    //}

                    if (!String.IsNullOrWhiteSpace(medicineTypeImport.MONTH_LIFESPAN_STR))
                    {
                        medicineType.MONTH_LIFESPAN_STR = medicineTypeImport.MONTH_LIFESPAN_STR;
                        long number = 0;
                        bool isValid = long.TryParse(medicineTypeImport.MONTH_LIFESPAN_STR, out number);
                        if (isValid)
                        {
                            medicineType.MONTH_LIFESPAN = number;
                        }
                        else
                        {
                            medicineType.ErrorDescriptions.Add("Tuổi thọ tháng không hợp lệ.");
                        }
                        if (Encoding.UTF8.GetByteCount(medicineType.MONTH_LIFESPAN.ToString()) > 19)
                        {
                            medicineType.ErrorDescriptions.Add("Tuổi thọ tháng vượt quá độ dài cho phép (19)");
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(medicineTypeImport.DAY_LIFESPAN_STR))
                    {
                        medicineType.DAY_LIFESPAN_STR = medicineTypeImport.DAY_LIFESPAN_STR;
                        long number = 0;
                        bool isValid = long.TryParse(medicineTypeImport.DAY_LIFESPAN_STR, out number);
                        if (isValid)
                        {
                            medicineType.DAY_LIFESPAN = number;
                        }
                        else
                        {
                            medicineType.ErrorDescriptions.Add("Tuổi thọ ngày không hợp lệ.");
                        }
                        if (Encoding.UTF8.GetByteCount(medicineType.DAY_LIFESPAN.ToString()) > 19)
                        {
                            medicineType.ErrorDescriptions.Add("Tuổi thọ ngày vượt quá độ dài cho phép (19)");
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(medicineTypeImport.HOUR_LIFESPAN_STR))
                    {
                        medicineType.HOUR_LIFESPAN_STR = medicineTypeImport.HOUR_LIFESPAN_STR;
                        long number = 0;
                        bool isValid = long.TryParse(medicineTypeImport.HOUR_LIFESPAN_STR, out number);
                        if (isValid)
                        {
                            medicineType.HOUR_LIFESPAN = number;
                        }
                        else
                        {
                            medicineType.ErrorDescriptions.Add("Tuổi thọ ngày không hợp lệ.");
                        }
                        if (Encoding.UTF8.GetByteCount(medicineType.DAY_LIFESPAN.ToString()) > 19)
                        {
                            medicineType.ErrorDescriptions.Add("Tuổi thọ ngày vượt quá độ dài cho phép (19)");
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(medicineType.MONTH_LIFESPAN.ToString()) && !String.IsNullOrWhiteSpace(medicineType.DAY_LIFESPAN.ToString()) ||
                        !String.IsNullOrWhiteSpace(medicineType.MONTH_LIFESPAN.ToString()) && !String.IsNullOrWhiteSpace(medicineType.HOUR_LIFESPAN.ToString()) ||
                        !String.IsNullOrWhiteSpace(medicineType.DAY_LIFESPAN.ToString()) && !String.IsNullOrWhiteSpace(medicineType.HOUR_LIFESPAN.ToString()))
                    {
                        medicineType.ErrorDescriptions.Add("Tuổi thọ không hợp lệ");
                    }

                    //else if (String.IsNullOrWhiteSpace(medicineType.MONTH_LIFESPAN.ToString()) && String.IsNullOrWhiteSpace(medicineType.DAY_LIFESPAN.ToString()) && String.IsNullOrWhiteSpace(medicineType.HOUR_LIFESPAN.ToString()))
                    //{
                    //    medicineType.ErrorDescriptions.Add("Không có tuổi thọ");
                    //}

                    if (!String.IsNullOrWhiteSpace(medicineTypeImport.MANUFACTURER_CODE))
                    {
                        var manufacturer = BackendDataWorker.Get<HIS_MANUFACTURER>().FirstOrDefault(o => o.MANUFACTURER_CODE.ToUpper() == medicineTypeImport.MANUFACTURER_CODE.ToUpper());

                        if (manufacturer == null)
                        {
                            medicineType.ErrorDescriptions.Add("Mã hãng sản xuất không chính xác");
                        }
                        else
                        {
                            medicineType.MANUFACTURER_ID = manufacturer.ID;
                            medicineType.MANUFACTURER_NAME = manufacturer.MANUFACTURER_NAME;
                            medicineType.MANUFACTURER_CODE = manufacturer.MANUFACTURER_CODE;
                        }
                    }

                    //else
                    //{
                    //    medicineType.ErrorDescriptions.Add("Không có hãng sản xuất");
                    //}

                    if (medicineType.ErrorDescriptions.Count > 0)
                    {
                        listErrorImport.Add(medicineType);
                    }
                    else
                    {
                        this.ListMedicineTypeAdoProcess.Insert(0, medicineType);
                    }
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

                LogSystem.Debug(LogUtil.TraceData("materialTypeImports.Count", materialTypeImports.Count));

                foreach (var materialTypeImport in materialTypeImports)
                {

                    var medicineType = new ADO.MedicineTypeADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(medicineType, materialTypeImport);

                    if (!string.IsNullOrWhiteSpace(medicineType.VALID_TO_TIME) &&
                        !DateTime.TryParseExact(medicineType.VALID_TO_TIME, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                    {
                        medicineType.ErrorDescriptions.Add("Định dạng thời gian đến không hợp lệ.");
                    }
                    else
                    {
                        medicineType.VALID_TO_TIME = materialTypeImport.VALID_TO_TIME;
                    }

                    if (!string.IsNullOrWhiteSpace(medicineType.VALID_FROM_TIME) &&
                        !DateTime.TryParseExact(medicineType.VALID_FROM_TIME, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                    {
                        medicineType.ErrorDescriptions.Add("Định dạng thời gian từ không hợp lệ.");
                    }
                    else
                    {
                        medicineType.VALID_FROM_TIME = materialTypeImport.VALID_FROM_TIME;
                    }

                    if (DateTime.TryParseExact(medicineType.VALID_FROM_TIME, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateFromValue) &&
                        DateTime.TryParseExact(medicineType.VALID_TO_TIME, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateToValue))
                    {
                        long? fromValue = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dateFromValue);
                        long? toValue = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dateToValue);

                        if (fromValue > toValue)
                        {
                            medicineType.ErrorDescriptions.Add("Thời gian từ không được lớn hơn thời gian đến");
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(materialTypeImport.MEDICINE_TYPE_CODE)
                        && !String.IsNullOrWhiteSpace(materialTypeImport.MATERIAL_TYPE_MAP_CODE))
                    {
                        medicineType.ErrorDescriptions.Add("Vừa có mã vật tư vừa có mã vật tư tương đương");
                        listErrorImport.Add(medicineType);
                        continue;
                    }

                    if (String.IsNullOrWhiteSpace(materialTypeImport.MEDICINE_TYPE_CODE)
                        && String.IsNullOrWhiteSpace(materialTypeImport.MATERIAL_TYPE_MAP_CODE))
                    {
                        medicineType.ErrorDescriptions.Add("Không có mã vật tư và mã vật tư tương đương");
                        listErrorImport.Add(medicineType);
                        continue;
                    }

                    if (!String.IsNullOrWhiteSpace(materialTypeImport.MEDICINE_TYPE_CODE))
                    {
                        var materialTypeNotExist = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.MATERIAL_TYPE_CODE == materialTypeImport.MEDICINE_TYPE_CODE);

                        if (materialTypeNotExist == null)
                        {
                            medicineType.ErrorDescriptions.Add("Mã vật tư không chính xác");
                            //this.listErrorImport.Add(medicineType);
                            //continue;
                        }
                        else
                        {
                            Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(medicineType, materialTypeNotExist);
                            medicineType.MEDICINE_TYPE_CODE = materialTypeNotExist.MATERIAL_TYPE_CODE;
                            medicineType.MEDICINE_TYPE_NAME = materialTypeNotExist.MATERIAL_TYPE_NAME;
                            //this.ListMedicineTypeAdoProcess.Insert(0, medicineType);
                        }
                    }
                    else
                    {
                        LogSystem.Debug("MATERIAL_TYPE_MAP_CODE: " + materialTypeImport.MATERIAL_TYPE_MAP_CODE);
                        var materialTypeMap = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.MATERIAL_TYPE_MAP_CODE == materialTypeImport.MATERIAL_TYPE_MAP_CODE);

                        if (materialTypeMap == null)
                        {
                            medicineType.ErrorDescriptions.Add("Mã vật tư tương đương không chính xác hoặc chưa được thiết lập với vật tư khác");
                            //this.listErrorImport.Add(medicineType);
                            //continue;
                        }
                        else
                        {
                            medicineType.ID = materialTypeMap.MATERIAL_TYPE_MAP_ID.Value;
                            medicineType.MEDICINE_TYPE_CODE = materialTypeMap.MATERIAL_TYPE_MAP_CODE;
                            medicineType.MEDICINE_TYPE_NAME = materialTypeMap.MATERIAL_TYPE_MAP_NAME;
                            medicineType.IsMaterialTypeMap = true;
                            medicineType.SERVICE_UNIT_ID = materialTypeMap.SERVICE_UNIT_ID;
                            medicineType.SERVICE_UNIT_CODE = materialTypeMap.SERVICE_UNIT_CODE;
                            medicineType.SERVICE_UNIT_NAME = materialTypeMap.SERVICE_UNIT_NAME;
                            //this.ListMedicineTypeAdoProcess.Insert(0, medicineType);
                        }
                    }
                    medicineType.EXPIRED_DATE = materialTypeImport.EXPIRED_DATE;

                    if (String.IsNullOrWhiteSpace(materialTypeImport.SUPPLIER_CODE))
                    {
                        medicineType.ErrorDescriptions.Add("Không có mã nhà thầu");
                    }
                    else
                    {
                        var supplier = Base.GlobalConfig.ListSupplier.FirstOrDefault(o => o.SUPPLIER_CODE == materialTypeImport.SUPPLIER_CODE);
                        if (supplier != null)
                        {
                            medicineType.SUPPLIER_ID = supplier.ID;
                            medicineType.SUPPLIER_CODE = supplier.SUPPLIER_CODE;
                            medicineType.SUPPLIER_NAME = supplier.SUPPLIER_NAME;
                        }
                        else
                        {
                            medicineType.ErrorDescriptions.Add("Mã nhà thầu không chính xác");
                        }
                    }
                    medicineType.Type = Base.GlobalConfig.VATTU;
                    medicineType.IdRow = setIdRow(this.ListMedicineTypeAdoProcess);
                    medicineType.BID_NUM_ORDER = materialTypeImport.BID_NUM_ORDER;

                    if (String.IsNullOrWhiteSpace(medicineType.BID_NUM_ORDER))
                    {
                        medicineType.ErrorDescriptions.Add("Không có số thứ tự thầu");
                    }
                    else if (Encoding.UTF8.GetByteCount(medicineType.BID_NUM_ORDER) > 50)
                    {
                        medicineType.ErrorDescriptions.Add("Số thứ tự thầu quá độ dài cho phép (50)");
                    }

                    medicineType.IMP_PRICE = materialTypeImport.IMP_PRICE;
                    if ((medicineType.IMP_PRICE ?? 0) < 0)
                    {
                        medicineType.ErrorDescriptions.Add("Giá nhập phải lớn hơn hoặc bằng 0");
                    }

                    medicineType.AMOUNT = materialTypeImport.AMOUNT;
                    if ((medicineType.AMOUNT ?? 0) <= 0)
                    {
                        medicineType.ErrorDescriptions.Add("Số lượng nhập phải lớn hơn 0");
                    }

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
                    if ((medicineType.IMP_VAT_RATIO ?? 0) < 0 || (medicineType.IMP_VAT_RATIO ?? 0) > 1)
                    {
                        medicineType.ErrorDescriptions.Add("Vat phải nằm trong khoảng 0 - 100");
                    }

                    medicineType.BID_PACKAGE_CODE = materialTypeImport.BID_PACKAGE_CODE;
                    if (string.IsNullOrEmpty(medicineType.BID_PACKAGE_CODE))
                    {
                        medicineType.ErrorDescriptions.Add("Không có mã gói thầu");
                    }
                    else if (Encoding.UTF8.GetByteCount(medicineType.BID_PACKAGE_CODE) > 4)
                    {
                        medicineType.ErrorDescriptions.Add("Mã gói thầu không được lớn hơn 4 ký tự");
                    }

                    medicineType.BID_GROUP_CODE = materialTypeImport.BID_GROUP_CODE;
                    if (!string.IsNullOrEmpty(medicineType.BID_GROUP_CODE) && Encoding.UTF8.GetByteCount(medicineType.BID_GROUP_CODE) > 4)
                    {
                        medicineType.ErrorDescriptions.Add("Mã nhóm thầu phải là 4 ký tự");
                    }
                    if (String.IsNullOrWhiteSpace(medicineType.IS_MEDICINE))
                    {
                        medicineType.BID_MATERIAL_TYPE_CODE = materialTypeImport.BID_MATERIAL_TYPE_CODE;
                        //if (String.IsNullOrWhiteSpace(medicineType.BID_MATERIAL_TYPE_CODE))
                        //{
                        //    medicineType.ErrorDescriptions.Add("Không có mã trúng thầu");
                        //}
                        if (Encoding.UTF8.GetByteCount(medicineType.BID_MATERIAL_TYPE_CODE) > 50)
                        {
                            medicineType.ErrorDescriptions.Add("Mã trúng thầu vượt quá độ dài cho phép (50)");
                        }

                        medicineType.BID_MATERIAL_TYPE_NAME = materialTypeImport.BID_MATERIAL_TYPE_NAME;
                        //if (String.IsNullOrWhiteSpace(medicineType.BID_MATERIAL_TYPE_NAME))
                        //{
                        //    medicineType.ErrorDescriptions.Add("Không có tên trúng thầu");
                        //}
                        if (Encoding.UTF8.GetByteCount(medicineType.BID_MATERIAL_TYPE_NAME) > 500)
                        {
                            medicineType.ErrorDescriptions.Add("Tên trúng thầu vượt quá độ dài cho phép (500)");
                        }

                        medicineType.JOIN_BID_MATERIAL_TYPE_CODE = materialTypeImport.JOIN_BID_MATERIAL_TYPE_CODE;
                        //if (String.IsNullOrWhiteSpace(medicineType.JOIN_BID_MATERIAL_TYPE_CODE))
                        //{
                        //    medicineType.ErrorDescriptions.Add("Không có mã dự thầu");
                        //}
                        if (Encoding.UTF8.GetByteCount(medicineType.JOIN_BID_MATERIAL_TYPE_CODE) > 50)
                        {
                            medicineType.ErrorDescriptions.Add("Mã dự thầu vượt quá độ dài cho phép (50)");
                        }
                    }

                    medicineType.CONCENTRA = materialTypeImport.CONCENTRA;
                    //if (String.IsNullOrWhiteSpace(medicineType.CONCENTRA))
                    //{
                    //    medicineType.ErrorDescriptions.Add("Không có nồng độ/hàm lượng");
                    //}
                    //else
                        if (Encoding.UTF8.GetByteCount(medicineType.CONCENTRA) > 1000)
                    {
                        medicineType.ErrorDescriptions.Add("Nồng độ/hàm lượng vượt quá độ dài cho phép (1000)");
                    }

                    if (!String.IsNullOrWhiteSpace(materialTypeImport.NATIONAL_NAME))
                    {
                        var national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().FirstOrDefault(o => o.NATIONAL_NAME.ToUpper() == materialTypeImport.NATIONAL_NAME.ToUpper());

                        if (national == null)
                        {
                            medicineType.ErrorDescriptions.Add("Tên nước sản xuất không chính xác");
                        }
                        else
                        {
                            medicineType.NATIONAL_NAME = national.NATIONAL_NAME;
                        }
                    }
                    else if (Encoding.UTF8.GetByteCount(materialTypeImport.NATIONAL_NAME) > 100)
                    {
                        medicineType.ErrorDescriptions.Add("Nước sản xuất vượt quá độ dài cho phép (100)");
                    }
                    //else
                    //{
                    //    medicineType.ErrorDescriptions.Add("Không có tên nước sản xuất");
                    //}

                    if (!String.IsNullOrWhiteSpace(materialTypeImport.MONTH_LIFESPAN_STR))
                    {
                        medicineType.MONTH_LIFESPAN_STR = materialTypeImport.MONTH_LIFESPAN_STR;
                        long number = 0;
                        bool isValid = long.TryParse(materialTypeImport.MONTH_LIFESPAN_STR, out number);
                        if (isValid)
                        {
                            medicineType.MONTH_LIFESPAN = number;
                        }
                        else
                        {
                            medicineType.ErrorDescriptions.Add("Tuổi thọ tháng không hợp lệ.");
                        }
                        if (Encoding.UTF8.GetByteCount(medicineType.MONTH_LIFESPAN.ToString()) > 19)
                        {
                            medicineType.ErrorDescriptions.Add("Tuổi thọ tháng vượt quá độ dài cho phép (19)");
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(materialTypeImport.DAY_LIFESPAN_STR))
                    {
                        medicineType.DAY_LIFESPAN_STR = materialTypeImport.DAY_LIFESPAN_STR;
                        long number = 0;
                        bool isValid = long.TryParse(materialTypeImport.DAY_LIFESPAN_STR, out number);
                        if (isValid)
                        {
                            medicineType.DAY_LIFESPAN = number;
                        }
                        else
                        {
                            medicineType.ErrorDescriptions.Add("Tuổi thọ ngày không hợp lệ.");
                        }
                        if (Encoding.UTF8.GetByteCount(medicineType.DAY_LIFESPAN.ToString()) > 19)
                        {
                            medicineType.ErrorDescriptions.Add("Tuổi thọ ngày vượt quá độ dài cho phép (19)");
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(materialTypeImport.HOUR_LIFESPAN_STR))
                    {
                        medicineType.HOUR_LIFESPAN_STR = materialTypeImport.HOUR_LIFESPAN_STR;
                        long number = 0;
                        bool isValid = long.TryParse(materialTypeImport.HOUR_LIFESPAN_STR, out number);
                        if (isValid)
                        {
                            medicineType.HOUR_LIFESPAN = number;
                        }
                        else
                        {
                            medicineType.ErrorDescriptions.Add("Tuổi thọ ngày không hợp lệ.");
                        }
                        if (Encoding.UTF8.GetByteCount(medicineType.DAY_LIFESPAN.ToString()) > 19)
                        {
                            medicineType.ErrorDescriptions.Add("Tuổi thọ ngày vượt quá độ dài cho phép (19)");
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(medicineType.MONTH_LIFESPAN.ToString()) && !String.IsNullOrWhiteSpace(medicineType.DAY_LIFESPAN.ToString()) ||
                        !String.IsNullOrWhiteSpace(medicineType.MONTH_LIFESPAN.ToString()) && !String.IsNullOrWhiteSpace(medicineType.HOUR_LIFESPAN.ToString()) ||
                        !String.IsNullOrWhiteSpace(medicineType.DAY_LIFESPAN.ToString()) && !String.IsNullOrWhiteSpace(medicineType.HOUR_LIFESPAN.ToString()))
                    {
                        medicineType.ErrorDescriptions.Add("Tuổi thọ không hợp lệ");
                    }
                    //else if (String.IsNullOrWhiteSpace(medicineType.MONTH_LIFESPAN.ToString()) && String.IsNullOrWhiteSpace(medicineType.DAY_LIFESPAN.ToString()) && String.IsNullOrWhiteSpace(medicineType.HOUR_LIFESPAN.ToString()))
                    //{
                    //    medicineType.ErrorDescriptions.Add("Không có tuổi thọ");
                    //}
                    medicineType.BID_EXTRA_CODE = materialTypeImport.BID_EXTRA_CODE;
                    if (String.IsNullOrWhiteSpace(medicineType.BID_EXTRA_CODE))
                    {
                        medicineType.ErrorDescriptions.Add("Mã quyết định thầu không hợp lệ");
                    }
                    else if (Encoding.UTF8.GetByteCount(medicineType.BID_EXTRA_CODE) > 50)
                    {
                        medicineType.ErrorDescriptions.Add("Mã quyết định thầu vượt quá độ dài cho phép (50)");
                    }
                    if (!String.IsNullOrWhiteSpace(materialTypeImport.MANUFACTURER_CODE))
                    {
                        var manufacturer = BackendDataWorker.Get<HIS_MANUFACTURER>().FirstOrDefault(o => o.MANUFACTURER_CODE.ToUpper() == materialTypeImport.MANUFACTURER_CODE.ToUpper());

                        if (manufacturer == null)
                        {
                            medicineType.ErrorDescriptions.Add("Mã hãng sản xuất không chính xác");
                        }
                        else
                        {
                            medicineType.MANUFACTURER_ID = manufacturer.ID;
                            medicineType.MANUFACTURER_NAME = manufacturer.MANUFACTURER_NAME;
                            medicineType.MANUFACTURER_CODE = manufacturer.MANUFACTURER_CODE;
                        }
                    }
                    //else
                    //{
                    //    medicineType.ErrorDescriptions.Add("Không có hãng sản xuất");
                    //}

                    if (medicineType.ErrorDescriptions.Count > 0)
                    {
                        listErrorImport.Add(medicineType);
                    }
                    else
                    {
                        this.ListMedicineTypeAdoProcess.Insert(0, medicineType);
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
