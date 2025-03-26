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
using HIS.Desktop.Plugins.HisMedicalContractImport.ADO;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisMedicalContractImport
{
    public partial class frmHisMedicalContractImport
    {
        private void addServiceToProcessList(List<HisMedicalContractADO> _currentAdos, ref List<HisMedicalContractADO> _medicalContractAdos)
        {
            try
            {
                _medicalContractAdos = new List<HisMedicalContractADO>();
                this.dicMedicalContract = new Dictionary<string, HisMedicalContractADO>();
                long i = 0;

                //phân loại thuốc ỏ vật tư
                foreach (var item in _currentAdos)
                {
                    if (!string.IsNullOrEmpty(item.IS_MEDICINE) && item.IS_MEDICINE.Trim().ToUpper() == "X")
                    {
                        item.isMedicine = true;
                        item.dataType_forDisplay = "Thuốc";
                    }
                    else
                    {
                        item.isMedicine = false;
                        item.dataType_forDisplay = "Vật tư";
                    }
                }
                //Validate:
                foreach (var item in _currentAdos)
                {
                    i++;
                    List<string> errors = new List<string>();
                    var addingAdo = new HisMedicalContractADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HisMedicalContractADO>(addingAdo, item);
                    bool checkTrungDB = false;
                    bool checkTrungTrongFile = false;
                    bool checkPrivate = true;

                    List<V_HIS_MEDICAL_CONTRACT> medicalContract = new List<V_HIS_MEDICAL_CONTRACT>();
                    medicalContract = null;
                    // check trung trong file import
                    if (!string.IsNullOrEmpty(item.MEDICAL_CONTRACT_CODE) && !string.IsNullOrEmpty(item.MEDICINE_TYPE_CODEorMATERIAL_TYPE_CODE))
                    {
                        var count = _currentAdos.Where(o => o.MEDICAL_CONTRACT_CODE == item.MEDICAL_CONTRACT_CODE
                                                    && o.isMedicine == item.isMedicine
                                                    && o.MEDICINE_TYPE_CODEorMATERIAL_TYPE_CODE == item.MEDICINE_TYPE_CODEorMATERIAL_TYPE_CODE
                                                    ).ToList();
                        if (count.Count > 1)
                            checkTrungTrongFile = true;
                    }
                    if (checkTrungTrongFile)
                    {
                        if (item.isMedicine)
                        {
                            errors.Add(string.Format(Message.MessageImport.TonTaiTrungMaThuocTrongFileImport, item.MEDICAL_CONTRACT_CODE, item.MEDICINE_TYPE_CODEorMATERIAL_TYPE_CODE));
                        }
                        else
                        {
                            errors.Add(string.Format(Message.MessageImport.TonTaiTrungMaVatTuTrongFileImport, item.MEDICAL_CONTRACT_CODE, item.MEDICINE_TYPE_CODEorMATERIAL_TYPE_CODE));
                        }
                    }
                    // check neu du lieu trong file import da ton tai trong DB cung nha cung cap
                    //if (isFirstTime)

                    //CHECKKK
                    if (!string.IsNullOrEmpty(item.MEDICAL_CONTRACT_CODE) && !string.IsNullOrEmpty(item.MEDICINE_TYPE_CODEorMATERIAL_TYPE_CODE))
                    {
                        List<V_HIS_MEDICAL_CONTRACT> rs = rsDB.Where(o => o.MEDICAL_CONTRACT_CODE == item.MEDICAL_CONTRACT_CODE && o.SUPPLIER_CODE == item.SUPPLIER_CODE).ToList();
                        if (rs != null && rs.Count > 0)
                        {
                            checkTrungDB = true;
                        }
                    }
                    if (checkTrungDB)
                    {
                        errors.Add(string.Format(Message.MessageImport.DBDaTonTai, item.MEDICAL_CONTRACT_CODE));
                    }

                    HIS_MEDICINE_TYPE medicineType = null;  //loại thuốc
                    HIS_MATERIAL_TYPE materialType = null;  //loại Vật tư
                    // check tinh phu hop cua cac du lieu truyen vao
                    if (item.isMedicine)
                    {
                        //Mã thuốc, tên thuốc, Mã BHYT, Mã đơn vị tính
                        if (!string.IsNullOrEmpty(item.MEDICINE_TYPE_CODEorMATERIAL_TYPE_CODE))
                        {
                            if (item.MEDICINE_TYPE_CODEorMATERIAL_TYPE_CODE.Length > 25)
                            {
                                errors.Add(string.Format(Message.MessageImport.Maxlength, "Mã thuốc"));
                            }
                            medicineType = BackendDataWorker.Get<HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.MEDICINE_TYPE_CODE == item.MEDICINE_TYPE_CODEorMATERIAL_TYPE_CODE);
                            if (medicineType != null)
                            {
                                addingAdo.MEDICINE_TYPE_CODEorMATERIAL_TYPE_CODE = medicineType.MEDICINE_TYPE_CODE;
                                addingAdo.MEDICINE_TYPE_NAMEorMATERIAL_TYPE_NAME = medicineType.MEDICINE_TYPE_NAME;
                                addingAdo.BHYT = medicineType.ACTIVE_INGR_BHYT_CODE;
                                var serviceUnit = BackendDataWorker.Get<HIS_SERVICE_UNIT>().Where(o => o.ID == medicineType.TDL_SERVICE_UNIT_ID).FirstOrDefault();
                                addingAdo.SERVICE_UNIT_CODE = serviceUnit != null ? serviceUnit.SERVICE_UNIT_CODE : "";

                                addingAdo.EXPIRED_DATE = medicineType.LAST_EXPIRED_DATE;
                                if (medicineType.LAST_EXPIRED_DATE != null)
                                    addingAdo.EXPIRED_DATE_ForDisplay = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(medicineType.LAST_EXPIRED_DATE ?? 0);
                                else
                                    addingAdo.EXPIRED_DATE_ForDisplay = "";

                                addingAdo.CONCENTRA = medicineType.CONCENTRA;

                                if (medicineType.IS_ACTIVE != 1)
                                    errors.Add(string.Format(Message.MessageImport.DuLieuDaKhoa, "Thuốc"));
                            }
                            else
                            {
                                errors.Add(string.Format(Message.MessageImport.KhongHopLe, "Mã thuốc"));
                            }
                        }
                        else
                        {
                            errors.Add(string.Format(Message.MessageImport.ThieuTruongDL, "Mã thuốc/vt"));
                        }
                        //Mã đường dùng
                        if (!string.IsNullOrEmpty(item.MEDICINE_USE_FORM_CODE))
                        {
                            if (item.MEDICINE_USE_FORM_CODE.Length > 6)
                            {
                                errors.Add(string.Format(Message.MessageImport.Maxlength, "Mã đường dùng"));
                            }
                            var rs = BackendDataWorker.Get<HIS_MEDICINE_USE_FORM>().FirstOrDefault(p => p.MEDICINE_USE_FORM_CODE == item.MEDICINE_USE_FORM_CODE);
                            if (rs != null)
                            {
                                addingAdo.MEDICINE_USE_FORM_CODE = rs.MEDICINE_USE_FORM_CODE;
                                addingAdo.MEDICINE_USE_FORM_NAME = rs.MEDICINE_USE_FORM_NAME;

                                if (rs.IS_ACTIVE != 1)
                                    errors.Add(string.Format(Message.MessageImport.DuLieuDaKhoa, "Dữ liệu đường dùng"));
                            }
                            else
                            {
                                errors.Add(string.Format(Message.MessageImport.KhongHopLe, "Mã đường dùng"));
                            }
                        }
                        else
                        {
                            errors.Add(string.Format(Message.MessageImport.ThieuTruongDL, "Mã đường dùng"));
                        }
                        //Hoạt chất
                        if (!string.IsNullOrEmpty(item.ACTIVE_INGR_BHYT_NAME))
                        {
                            if (Inventec.Common.String.CountVi.Count(item.ACTIVE_INGR_BHYT_NAME) > 1000)
                            {
                                errors.Add(string.Format(Message.MessageImport.Maxlength, "Tên hoạt chất theo quy định BHYT"));
                            }
                        }
                        //Số đăng ký
                        if (!string.IsNullOrEmpty(item.MEDICINE_REGISTER_NUMBER))
                        {
                            if (item.MEDICINE_REGISTER_NUMBER.Length > 500)
                            {
                                errors.Add(string.Format(Message.MessageImport.Maxlength, "Số đăng ký"));
                            }
                        }
                    }
                    else
                    {
                        //Mã vật tư, tên vật tư, Mã BHYT, Mã đơn vị tính
                        if (!string.IsNullOrEmpty(item.MEDICINE_TYPE_CODEorMATERIAL_TYPE_CODE))
                        {
                            if (item.MEDICINE_TYPE_CODEorMATERIAL_TYPE_CODE.Length > 25)
                            {
                                errors.Add(string.Format(Message.MessageImport.Maxlength, "Mã vật tư"));
                            }
                            materialType = BackendDataWorker.Get<HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.MATERIAL_TYPE_CODE == item.MEDICINE_TYPE_CODEorMATERIAL_TYPE_CODE);
                            if (materialType != null)
                            {
                                addingAdo.MEDICINE_TYPE_CODEorMATERIAL_TYPE_CODE = materialType.MATERIAL_TYPE_CODE;
                                addingAdo.MEDICINE_TYPE_NAMEorMATERIAL_TYPE_NAME = materialType.MATERIAL_TYPE_NAME;
                                addingAdo.BHYT = "";
                                var serviceUnit = BackendDataWorker.Get<HIS_SERVICE_UNIT>().Where(o => o.ID == materialType.TDL_SERVICE_UNIT_ID).FirstOrDefault();
                                addingAdo.SERVICE_UNIT_CODE = serviceUnit != null ? serviceUnit.SERVICE_UNIT_CODE : "";

                                addingAdo.EXPIRED_DATE = materialType.LAST_EXPIRED_DATE;
                                if (materialType.LAST_EXPIRED_DATE != null)
                                    addingAdo.EXPIRED_DATE_ForDisplay = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(materialType.LAST_EXPIRED_DATE ?? 0);
                                else
                                    addingAdo.EXPIRED_DATE_ForDisplay = "";

                                addingAdo.CONCENTRA = materialType.CONCENTRA;

                                if (materialType.IS_ACTIVE != 1)
                                    errors.Add(string.Format(Message.MessageImport.DuLieuDaKhoa, "Vật tư"));
                            }
                            else
                            {
                                errors.Add(string.Format(Message.MessageImport.KhongHopLe, "Mã vật tư"));
                            }
                        }
                        else
                        {
                            errors.Add(string.Format(Message.MessageImport.ThieuTruongDL, "Mã thuốc/vt"));
                        }
                    }

                    //Hãng sản xuất
                    if (!string.IsNullOrEmpty(item.MANUFACTURER_CODE))
                    {
                        if (item.MANUFACTURER_CODE.Length > 6)
                        {
                            errors.Add(string.Format(Message.MessageImport.Maxlength, "Mã hãng sản xuất"));
                        }
                        var rs = BackendDataWorker.Get<HIS_MANUFACTURER>().FirstOrDefault(p => p.MANUFACTURER_CODE == item.MANUFACTURER_CODE);
                        if (rs != null)
                        {
                            addingAdo.MANUFACTURER_CODE = rs.MANUFACTURER_CODE;
                            addingAdo.MANUFACTURER_ID = rs.ID;

                            if (rs.IS_ACTIVE != 1)
                                errors.Add(string.Format(Message.MessageImport.DuLieuDaKhoa, "Hãng sản xuất"));
                        }
                        else
                        {
                            errors.Add(string.Format(Message.MessageImport.KhongHopLe, "Mã hãng sản xuất"));
                        }
                    }
                    //Nước sản xuất
                    if (!string.IsNullOrEmpty(item.NATIONAL_NAME))
                    {
                        if (Inventec.Common.String.CountVi.Count(item.NATIONAL_NAME) > 1000)
                        {
                            errors.Add(string.Format(Message.MessageImport.Maxlength, "Nước sản xuất"));
                        }
                    }

                    V_HIS_BID_MEDICINE_TYPE bidMedicineType = null;
                    V_HIS_BID_MATERIAL_TYPE bidMaterialType = null;

                    //Quyết định thầu, Nhóm thầu
                    if (!string.IsNullOrEmpty(item.BID_NUMBER))
                    {
                        if (item.BID_NUMBER.Length > 50)
                        {
                            errors.Add(string.Format(Message.MessageImport.Maxlength, "Sổ quyết định thầu"));
                        }

                        List<HIS_BID> rsListBID = this.ListBid != null ? this.ListBid.Where(p => p.BID_NUMBER == item.BID_NUMBER).ToList() : null;
                        if (rsListBID != null && rsListBID.Count() > 0)
                        {
                            //Validate có tồn tại dữ liệu mã thuốc/vật tư thuộc quyết định thầu
                            List<long> listBIDId = rsListBID.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Select(o => o.ID).ToList();
                            if (listBIDId != null && listBIDId.Count() > 0)
                            {
                                if (item.isMedicine)
                                {
                                    if (medicineType != null)
                                    {
                                        bidMedicineType = this.storeBidMedicineType.Where(o => listBIDId.Contains(o.BID_ID) && o.MEDICINE_TYPE_ID == medicineType.ID)
                                                                                    .OrderByDescending(o => o.BID_YEAR)
                                                                                    .ThenByDescending(o => o.ID)
                                                                                    .FirstOrDefault();
                                        if (bidMedicineType == null)
                                        {
                                            errors.Add(string.Format(Message.MessageImport.ThauKhongCoMaThuocVT, item.BID_NUMBER, "mã thuốc " + medicineType.MEDICINE_TYPE_CODE));
                                        }
                                        else
                                        {
                                            addingAdo.BID_ID = bidMedicineType.BID_ID;
                                        }
                                    }
                                }
                                else
                                {
                                    if (materialType != null)
                                    {
                                        bidMaterialType = this.storeBidMaterialType.Where(o => listBIDId.Contains(o.BID_ID) && o.MATERIAL_TYPE_ID == materialType.ID)
                                                                                    .OrderByDescending(o => o.BID_YEAR)
                                                                                    .ThenByDescending(o => o.ID)
                                                                                    .FirstOrDefault();

                                        if (bidMaterialType == null)
                                        {
                                            errors.Add(string.Format(Message.MessageImport.ThauKhongCoMaThuocVT, item.BID_NUMBER, "mã vật tư " + materialType.MATERIAL_TYPE_CODE));
                                        }
                                        else
                                        {
                                            addingAdo.BID_ID = bidMaterialType.BID_ID;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                errors.Add(string.Format(Message.MessageImport.DuLieuDaKhoa, "Quyết định thầu"));
                            }
                        }
                        else
                        {
                            errors.Add(string.Format(Message.MessageImport.KhongHopLe, "Sổ quyết định thầu"));
                        }
                    }
                    //Nhóm thầu
                    if (!string.IsNullOrEmpty(item.BID_GROUP_CODE))
                    {
                        if (item.BID_GROUP_CODE.Length > 2)
                        {
                            errors.Add(string.Format(Message.MessageImport.Maxlength, "Mã nhóm thầu"));
                        }
                    }

                    //Nhà cung cấp
                    if (!string.IsNullOrEmpty(item.SUPPLIER_CODE))
                    {
                        if (item.SUPPLIER_CODE.Length > 10)
                        {
                            errors.Add(string.Format(Message.MessageImport.Maxlength, "Mã nhà cung cấp"));
                        }
                        var rs = BackendDataWorker.Get<HIS_SUPPLIER>().FirstOrDefault(p => p.SUPPLIER_CODE == item.SUPPLIER_CODE);
                        if (rs != null)
                        {
                            addingAdo.SUPPLIER_CODE = rs.SUPPLIER_CODE;
                            addingAdo.SUPPLIER_ID = rs.ID;

                            if (rs.IS_ACTIVE != 1)
                                errors.Add(string.Format(Message.MessageImport.DuLieuDaKhoa, "Dữ liệu nhà cung cấp"));
                        }
                        else
                        {
                            errors.Add(string.Format(Message.MessageImport.KhongHopLe, "Mã nhà cung cấp"));
                        }
                    }
                    else
                    {
                        errors.Add(string.Format(Message.MessageImport.ThieuTruongDL, "Nhà cung cấp"));
                    }

                    //Số lượng
                    if (item.AMOUNT <= 0)
                    {
                        errors.Add(string.Format(Message.MessageImport.KhongHopLe, "Số lượng"));
                    }
                    //Giá hợp đồng
                    if (item.CONTRACT_PRICE == null || item.CONTRACT_PRICE < 0)
                    {
                        errors.Add(string.Format(Message.MessageImport.KhongHopLe, "Giá hợp đồng"));
                    }
                    //thoi gian giam tu
                    if ((item.CONTRACT_PRICE == null || item.CONTRACT_PRICE == 0) && item.DISCOUNT_FROM_DATE == null)
                    {
                        errors.Add(string.Format(Message.MessageImport.ThieuTruongDL, "Thời gian giảm giá từ"));
                    }
                    //thoi gian giam đén
                    if ((item.CONTRACT_PRICE == null || item.CONTRACT_PRICE == 0) && item.DISCOUNT_TO_DATE == null)
                    {
                        errors.Add(string.Format(Message.MessageImport.ThieuTruongDL, "Thời gian giảm giá đến"));
                    }
                    //Thoi gian giam từ - đến
                    if (item.DISCOUNT_FROM_DATE != null)
                    {
                        var rsDate = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(item.DISCOUNT_FROM_DATE ?? 0);
                        if (rsDate != null && rsDate >= DateTime.Now)
                        {
                            addingAdo.DISCOUNT_FROM_DATE_STR = Inventec.Common.DateTime.Convert.SystemDateTimeToDateString(rsDate);
                        }
                        else
                        {
                            addingAdo.DISCOUNT_FROM_DATE_STR = item.DISCOUNT_FROM_DATE.ToString();
                            errors.Add(string.Format(Message.MessageImport.KhongHopLe, "Thời gian giảm giá từ"));
                        }
                    }
                    if (item.DISCOUNT_TO_DATE != null)
                    {
                        var rsDate = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(item.DISCOUNT_TO_DATE ?? 0);
                        if (rsDate != null && rsDate >= DateTime.Now)
                        {
                            addingAdo.DISCOUNT_TO_DATE_STR = Inventec.Common.DateTime.Convert.SystemDateTimeToDateString(rsDate);
                        }
                        else
                        {
                            addingAdo.DISCOUNT_TO_DATE_STR = item.DISCOUNT_TO_DATE.ToString();
                            errors.Add(string.Format(Message.MessageImport.KhongHopLe, "Thời gian giảm giá đến"));
                        }
                    }
                    if (item.DISCOUNT_FROM_DATE != null && item.DISCOUNT_TO_DATE != null && item.DISCOUNT_FROM_DATE > item.DISCOUNT_TO_DATE)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show
                                ("Thời gian giảm giá không hợp lệ");
                        return;
                    }

                    if (item.isMedicine)
                    {
                        if (bidMedicineType != null && (item.CONTRACT_PRICE ?? 0) > ((bidMedicineType.IMP_PRICE ?? 0) * (1 + (bidMedicineType.IMP_VAT_RATIO ?? 0))))
                        {
                            errors.Add("Giá hợp đồng lớn hơn giá trong thầu");
                        }
                    }
                    else
                    {
                        if (bidMaterialType != null && (item.CONTRACT_PRICE ?? 0) > ((bidMaterialType.IMP_PRICE ?? 0) * (1 + (bidMaterialType.IMP_VAT_RATIO ?? 0))))
                        {
                            errors.Add("Giá hợp đồng lớn hơn giá trong thầu");
                        }
                    }

                    //Hạn nhập
                    if (item.IMP_EXPIRED_DATE != null)
                    {
                        var rsDate = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(item.IMP_EXPIRED_DATE ?? 0);
                        if (rsDate != null && rsDate > DateTime.MinValue)
                        {
                            addingAdo.IMP_EXPIRED_DATE_ForDisplay = Inventec.Common.DateTime.Convert.SystemDateTimeToDateString(rsDate);
                        }
                        else
                        {
                            addingAdo.IMP_EXPIRED_DATE_ForDisplay = item.IMP_EXPIRED_DATE.ToString();
                            errors.Add(string.Format(Message.MessageImport.KhongHopLe, "Hạn nhập"));
                        }
                    }
                    //Hạn dùng
                    if (addingAdo.EXPIRED_DATE == null || addingAdo.EXPIRED_DATE <= 0)
                    {
                        if (item.EXPIRED_DATE != null)
                        {
                            var rsDate = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(item.EXPIRED_DATE ?? 0);
                            if (rsDate != null && rsDate > DateTime.MinValue)
                            {
                                addingAdo.EXPIRED_DATE_ForDisplay = Inventec.Common.DateTime.Convert.SystemDateTimeToDateString(rsDate);
                            }
                            else
                            {
                                addingAdo.EXPIRED_DATE_ForDisplay = item.EXPIRED_DATE.ToString();
                                errors.Add(string.Format(Message.MessageImport.KhongHopLe, "Hạn dùng"));
                            }
                        }
                    }
                    //Nồng độ/HL
                    if (!string.IsNullOrEmpty(item.CONCENTRA))
                    {
                        if (Inventec.Common.String.CountVi.Count(item.CONCENTRA) > 1000)
                        {
                            errors.Add(string.Format(Message.MessageImport.Maxlength, "Nồng độ/HL"));
                        }
                        addingAdo.CONCENTRA = item.CONCENTRA;
                    }

                    //Số hợp đồng
                    if (!string.IsNullOrEmpty(item.MEDICAL_CONTRACT_CODE))
                    {
                        if (item.MEDICAL_CONTRACT_CODE.Length > 50)
                        {
                            errors.Add(string.Format(Message.MessageImport.Maxlength, "Số hợp đồng"));
                        }
                    }
                    else
                    {
                        errors.Add(string.Format(Message.MessageImport.ThieuTruongDL, "Số hợp đồng"));
                    }
                    //Ngày hiệu lực từ - đến
                    if (item.VALID_FROM_DATE != null)
                    {
                        var rsDate = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(item.VALID_FROM_DATE ?? 0);
                        if (rsDate != null && rsDate > DateTime.MinValue)
                        {
                            addingAdo.VALID_FROM_DATE_ForDisplay = Inventec.Common.DateTime.Convert.SystemDateTimeToDateString(rsDate);
                        }
                        else
                        {
                            addingAdo.VALID_FROM_DATE_ForDisplay = item.VALID_FROM_DATE.ToString();
                            errors.Add(string.Format(Message.MessageImport.KhongHopLe, "Ngày hiệu lực từ"));
                        }
                    }
                    if (item.VALID_TO_DATE != null)
                    {
                        var rsDate = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(item.VALID_TO_DATE ?? 0);
                        if (rsDate != null && rsDate > DateTime.MinValue)
                        {
                            addingAdo.VALID_TO_DATE_ForDisplay = Inventec.Common.DateTime.Convert.SystemDateTimeToDateString(rsDate);
                        }
                        else
                        {
                            addingAdo.VALID_TO_DATE_ForDisplay = item.VALID_TO_DATE.ToString();
                            errors.Add(string.Format(Message.MessageImport.KhongHopLe, "Ngày hiệu lực đến"));
                        }
                    }
                    //Thỏa thuận liên doanh
                    if (!string.IsNullOrEmpty(item.VENTURE_AGREENING))
                    {
                        if (Inventec.Common.String.CountVi.Count(item.VENTURE_AGREENING) > 500)
                        {
                            errors.Add(string.Format(Message.MessageImport.Maxlength, "Thỏa thuận liên doanh"));
                        }
                    }
                    //Ghi chú
                    if (!string.IsNullOrEmpty(item.NOTE))
                    {
                        if (Inventec.Common.String.CountVi.Count(item.NOTE) > 4000)
                        {
                            errors.Add(string.Format(Message.MessageImport.Maxlength, "Ghi chú"));
                        }
                    }
                    //Tuổi thọ tháng, ngày, giờ
                    if (item.MONTH_LIFESPAN != null)
                    {
                        if (item.MONTH_LIFESPAN < 0)
                        {
                            errors.Add(string.Format(Message.MessageImport.KhongHopLe, "Tuổi thọ tháng"));
                        }
                    }
                    if (item.DAY_LIFESPAN != null)
                    {
                        if (item.DAY_LIFESPAN < 0)
                        {
                            errors.Add(string.Format(Message.MessageImport.KhongHopLe, "Tuổi thọ ngày"));
                        }
                    }
                    if (item.HOUR_LIFESPAN != null)
                    {
                        if (item.HOUR_LIFESPAN < 0)
                        {
                            errors.Add(string.Format(Message.MessageImport.KhongHopLe, "Tuổi thọ giờ"));
                        }
                    }

                    //đồng thời không có thông tin giá nhập và vat nhập hoặc giá nhập vat nhập âm thì báo lỗi
                    if ((item.IMP_PRICE ?? 0) < 0 && (item.IMP_VAT_RATIO ?? 0) < 0)
                    {
                        errors.Add(string.Format(Message.MessageImport.KhongHopLe, "Giá nhập"));
                    }

                    
                    // validate dữ liệu hợp đồng có cùng mã hợp đồng
                    if (this.dicMedicalContract.ContainsKey(addingAdo.MEDICAL_CONTRACT_CODE) && this.dicMedicalContract[addingAdo.MEDICAL_CONTRACT_CODE] != null)
                    {
                        bool isValid = true;
                        string mess = "";
                        //if (addingAdo.SUPPLIER_CODE == this.dicMedicalContract[addingAdo.MEDICAL_CONTRACT_CODE].SUPPLIER_CODE)
                        //{
                        //    errors.Add(string.Format("Đã tồn tại dữ liệu hợp đồng với mã {0} cùng dữ liệu: nhà thầu {0}", addingAdo.MEDICAL_CONTRACT_CODE, addingAdo.SUPPLIER_CODE));
                        //}
                        if (addingAdo.BID_NUMBER != this.dicMedicalContract[addingAdo.MEDICAL_CONTRACT_CODE].BID_NUMBER)
                        {
                            isValid = false;
                            if (mess != "") mess += ", ";
                            mess += "quyết định thầu";
                        }
                        if (addingAdo.VALID_FROM_DATE != this.dicMedicalContract[addingAdo.MEDICAL_CONTRACT_CODE].VALID_FROM_DATE)
                        {
                            isValid = false;
                            if (mess != "") mess += ", ";
                            mess += "ngày hiệu lực từ";
                        }
                        if (addingAdo.VALID_TO_DATE != this.dicMedicalContract[addingAdo.MEDICAL_CONTRACT_CODE].VALID_TO_DATE)
                        {
                            isValid = false;
                            if (mess != "") mess += ", ";
                            mess += "ngày hiệu lực đến";
                        }
                        if (addingAdo.VENTURE_AGREENING != this.dicMedicalContract[addingAdo.MEDICAL_CONTRACT_CODE].VENTURE_AGREENING)
                        {
                            isValid = false;
                            if (mess != "") mess += ", ";
                            mess += "thỏa thuận liên doanh";
                        }

                        if (!isValid)
                        {
                            errors.Add(string.Format(Message.MessageImport.DulieuHopDongKhongHopLe, addingAdo.MEDICAL_CONTRACT_CODE, mess));
                        }
                    }
                    else
                    {
                        this.dicMedicalContract.Add(item.MEDICAL_CONTRACT_CODE, addingAdo);
                    }

                    // Mã, tên công ty xuất hoá đơn                
                    if (!string.IsNullOrEmpty(item.DOCUMENT_SUPPLIER_CODE))
                    {
                        if (item.DOCUMENT_SUPPLIER_CODE.Length > 10)
                        {
                            errors.Add(string.Format(Message.MessageImport.Maxlength, "Mã Cty xuất hoá đơn"));
                        }
                        var rs = BackendDataWorker.Get<HIS_SUPPLIER>().FirstOrDefault(p => p.SUPPLIER_CODE == item.DOCUMENT_SUPPLIER_CODE);
                        if (rs != null)
                        {
                            addingAdo.DOCUMENT_SUPPLIER_ID = rs.ID;
                            addingAdo.DOCUMENT_SUPPLIER_CODE = rs.SUPPLIER_CODE;
                            addingAdo.DOCUMENT_SUPPLIER_NAME = rs.SUPPLIER_NAME;

                            if (rs.IS_ACTIVE != 1)
                                errors.Add(string.Format(Message.MessageImport.DuLieuDaKhoa, "Mã Cty xuất hoá đơn"));

                            var checkSHD = _currentAdos
                                        .Where(o => o.MEDICAL_CONTRACT_CODE == item.MEDICAL_CONTRACT_CODE)
                                        .GroupBy(p => p.MEDICAL_CONTRACT_CODE)
                                        .Select(group => new { ContractCode = group.Key, CompanyCount = group.Select(p => p.DOCUMENT_SUPPLIER_CODE).Distinct().Count() })
                                        .ToList();

                            foreach (var result in checkSHD)
                            {
                                if (result.CompanyCount > 1)
                                {
                                    errors.Add(string.Format("Số hợp đồng {0} có {1} công ty xuất hóa đơn khác nhau", result.ContractCode, result.CompanyCount));
                                }
                            }
                        }
                        else
                        {
                            errors.Add(string.Format(Message.MessageImport.KhongHopLe, "Mã Cty xuất hoá đơn"));
                        }
                    }

                    addingAdo.STT = i;

                    if (_medicalContractAdos != null && _medicalContractAdos.Count > 0)
                    {
                        var itemExistDiscountDate = _medicalContractAdos.FirstOrDefault(o => o.isMedicine == addingAdo.isMedicine && o.MEDICINE_TYPE_CODEorMATERIAL_TYPE_CODE == addingAdo.MEDICINE_TYPE_CODEorMATERIAL_TYPE_CODE && o.DISCOUNT_FROM_DATE == addingAdo.DISCOUNT_FROM_DATE && o.DISCOUNT_TO_DATE == addingAdo.DISCOUNT_TO_DATE);
                        if (itemExistDiscountDate != null)
                        {
                            errors.Add(string.Format("Thuốc {0} có thời gian giảm giá {1}{2}.", itemExistDiscountDate.MEDICINE_TYPE_NAMEorMATERIAL_TYPE_NAME, itemExistDiscountDate.DISCOUNT_FROM_DATE != null ? string.Format(" từ {0}", Inventec.Common.DateTime.Convert.TimeNumberToDateString(itemExistDiscountDate.DISCOUNT_FROM_DATE ?? 0)) : "", itemExistDiscountDate.DISCOUNT_TO_DATE != null ? string.Format(" đến {0}", Inventec.Common.DateTime.Convert.TimeNumberToDateString(itemExistDiscountDate.DISCOUNT_TO_DATE ?? 0)) : ""));
                        }
                    }

                    if (errors != null && errors.Count > 0)
                    {
                        addingAdo.ERROR = string.Join("| ", errors);
                    }

                    _medicalContractAdos.Add(addingAdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
