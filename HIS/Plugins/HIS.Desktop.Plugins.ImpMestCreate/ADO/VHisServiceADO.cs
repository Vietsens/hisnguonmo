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
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ImpMestCreate.ADO
{
    public class VHisServiceADO
    {
        public HIS_MEDICINE HisMedicine { get; set; }
        public List<HIS_MEDICINE_PATY> HisMedicinePatys { get; set; }

        public HIS_MATERIAL HisMaterial { get; set; }
        public List<HIS_MATERIAL_PATY> HisMaterialPatys { get; set; }

        public bool IsMedicine { get; set; }
        public List<VHisServicePatyADO> VHisServicePatys { get; set; }

        public bool CheckImportThieuChinhSachGia { get; set; }

        public long SERVICE_ID { get; set; }
        public long SERVICE_TYPE_ID { get; set; }

        public bool BanBangGiaNhap { get; set; }

        public long MEDI_MATE_ID { get; set; }
        public string MEDI_MATE_CODE { get; set; }
        public string MEDI_MATE_NAME { get; set; }
        public long SERVICE_UNIT_ID { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string PACKAGE_NUMBER { get; set; }

        public decimal CanImpAmount { get; set; }

        public string TDL_BID_GROUP_CODE { get; set; }
        public string TDL_BID_NUM_ORDER { get; set; }
        public string TDL_BID_YEAR { get; set; }
        public string TDL_BID_PACKAGE_CODE { get; set; }
        public string TDL_BID_NUMBER { get; set; }
        public string TDL_BID_EXTRA_CODE { get; set; }

        public decimal IMP_AMOUNT { get; set; }
        public decimal IMP_PRICE { get; set; }
        public decimal IMP_VAT_RATIO { get; set; }
        public decimal ImpVatRatio { get; set; }

        public decimal? BidImpPrice { get; set; }
        public decimal? BidImpVatRatio { get; set; }

        public long? EXPIRED_DATE { get; set; }

        public bool IsRequireHsd { get; set; }
        public long? BidId { get; set; }
        public long SupplierId { get; set; }
        public List<Error> Errors { get; set; }
        public List<Warm> Warms { get; set; }


        public bool IsReusable { get; set; }
        public bool IsIdentity { get; set; }
        public long? MAX_REUSE_COUNT { get; set; }
        public string SERIAL_NUMBER { get; set; }
        public decimal? VS_PRICE { get; set; }
        public long? MEDICINE_LINE_ID { get; set; }
        public List<ImpMestMaterialReusableSDO> SerialNumbers { get; set; }

        public long? DOCUMENT_PRICE { get; set; }

        public decimal BID_PRICE { get; set; }

        public decimal IMP_PRICE_PREVIOUS { get; set; }

        public long? VALID_FROM_TIME { get; set; }
        public long? VALID_TO_TIME { get; set; }
        public bool IsServiceUnitPrimary { get; set; }

        public string NATIONAL_NAME { get; set; }
        public string CONCENTRA { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public long? MANUFACTURER_ID { get; set; }
        public bool IsAllowMissingPkgInfo { get; set; }

        public decimal? CONTRACT_PRICE { get; set; }
        public long? MEDICAL_CONTRACT_ID { get; set; }
        public short? isBusiness { get; set; }
        public string packingTypeName { get; set; }
        public string heinServiceBhytName { get; set; }
        public long? MAP_MEDI_MATE_ID { get; set; }
        public long? monthLifespan { get; set; }

        public long? medicineUseFormId { get; set; }
        public string activeIngrBhytName { get; set; }
        public string activeIngrBhytCode { get; set; }
        public string dosageForm { get; set; }

        public short? isFunctionalFood { get; set; }

        public string MEDICINE_USE_FORM_CODE { get; set; }
        public string DOSAGE_FORM { get; set; }

        public long? MONTH_LIFESPAN { get; set; }
        public long? DAY_LIFESPAN { get; set; }
        public long? HOUR_LIFESPAN { get; set; }

        public string MONTH_LIFESPAN_STR { get; set; }
        public string DAY_LIFESPAN_STR { get; set; }
        public string HOUR_LIFESPAN_STR { get; set; }

        public decimal? TAX_RATIO { get; set; }

        public string WarningPrice { get; set; }
        public decimal? GiaBan { get; set; }
        public bool IsVaccin { get; set; }
        public long? STORAGE_CONDITION_ID { get; set; }
        public decimal? TEMPERATURE { get; set; }
        public decimal? ADJUST_AMOUNT { get; set; }
        public decimal? PRICING_MAX_REUSE_COUNT { get; set; }
        public long? DISCOUNT_FROM_DATE { get; set; }
        public long? DISCOUNT_TO_DATE { get; set; }
        public string TT_THAU { get; set; }
        public string INFORMATION_BID { get; set; }

        public VHisServiceADO(V_HIS_MEDICINE_TYPE data)
        {
            try
            {
                this.IsMedicine = true;
                if (data != null)
                {
                    this.MEDI_MATE_ID = data.ID;
                    this.MEDI_MATE_CODE = data.MEDICINE_TYPE_CODE;
                    this.MEDI_MATE_NAME = data.MEDICINE_TYPE_NAME;
                    if (data.IMP_UNIT_ID.HasValue)
                        this.SERVICE_UNIT_NAME = data.IMP_UNIT_NAME;
                    else
                        this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                    this.SERVICE_UNIT_ID = data.SERVICE_UNIT_ID;
                    this.IsRequireHsd = data.IS_REQUIRE_HSD == 1 ? true : false;
                    this.IMP_PRICE = data.IMP_PRICE ?? 0;
                    this.IMP_VAT_RATIO = data.IMP_VAT_RATIO ?? 0;
                    this.ImpVatRatio = this.IMP_VAT_RATIO * 100;
                    this.SERVICE_ID = data.SERVICE_ID;
                    this.SERVICE_TYPE_ID = data.SERVICE_TYPE_ID;
                    this.BanBangGiaNhap = data.IS_SALE_EQUAL_IMP_PRICE == 1 ? true : false;
                    this.HisMedicine = new HIS_MEDICINE();
                    this.HisMedicine.DOSAGE_FORM = data.DOSAGE_FORM;
                    this.HisMedicine.IS_SALE_EQUAL_IMP_PRICE = data.IS_SALE_EQUAL_IMP_PRICE;
                    this.HisMedicine.MEDICINE_TYPE_ID = data.ID;
                    this.MEDICINE_LINE_ID = data.MEDICINE_LINE_ID;
                    this.IsAllowMissingPkgInfo = data.ALLOW_MISSING_PKG_INFO == 1;
                    this.isBusiness = data.IS_BUSINESS;
                    this.REGISTER_NUMBER = data.REGISTER_NUMBER;
                    this.packingTypeName = data.PACKING_TYPE_NAME;
                    this.heinServiceBhytName = data.HEIN_SERVICE_BHYT_NAME;
                    this.activeIngrBhytName = data.ACTIVE_INGR_BHYT_NAME;
                    //this.dosageForm = data.DOSAGE_FORM;
                    this.medicineUseFormId = data.MEDICINE_USE_FORM_ID;
                    this.isFunctionalFood = data.IS_FUNCTIONAL_FOOD;
                    this.MEDICINE_USE_FORM_CODE = data.MEDICINE_USE_FORM_CODE;
                    this.IsVaccin = data.IS_VACCINE == 1 ? true : false;
                    this.STORAGE_CONDITION_ID = data.STORAGE_CONDITION_ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public VHisServiceADO(V_HIS_MATERIAL_TYPE data)
        {
            try
            {
                this.IsMedicine = false;
                this.IsVaccin = false;
                if (data != null)
                {
                    this.MEDI_MATE_ID = data.ID;
                    this.MEDI_MATE_CODE = data.MATERIAL_TYPE_CODE;
                    this.MEDI_MATE_NAME = data.MATERIAL_TYPE_NAME;
                    this.IsRequireHsd = data.IS_REQUIRE_HSD == 1 ? true : false;
                    if (data.IMP_UNIT_ID.HasValue)
                        this.SERVICE_UNIT_NAME = data.IMP_UNIT_NAME;
                    else
                        this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                    this.SERVICE_UNIT_ID = data.SERVICE_UNIT_ID;
                    this.IMP_PRICE = data.IMP_PRICE ?? 0;
                    this.IMP_VAT_RATIO = data.IMP_VAT_RATIO ?? 0;
                    this.ImpVatRatio = this.IMP_VAT_RATIO * 100;
                    this.SERVICE_ID = data.SERVICE_ID;
                    this.SERVICE_TYPE_ID = data.SERVICE_TYPE_ID;
                    this.BanBangGiaNhap = data.IS_SALE_EQUAL_IMP_PRICE == 1 ? true : false;
                    this.HisMaterial = new HIS_MATERIAL();
                    this.HisMaterial.IS_SALE_EQUAL_IMP_PRICE = data.IS_SALE_EQUAL_IMP_PRICE;
                    this.HisMaterial.MATERIAL_TYPE_ID = data.ID;

                    this.MAX_REUSE_COUNT = data.MAX_REUSE_COUNT;
                    this.IsReusable = data.IS_REUSABLE == 1 ? true : false;
                    this.IsIdentity = data.IS_IDENTITY_MANAGEMENT == 1 ? true : false;
                    this.isBusiness = data.IS_BUSINESS;
                    //this.REGISTER_NUMBER = data.REGISTER_NUMBER;
                    this.packingTypeName = data.PACKING_TYPE_NAME;
                    this.heinServiceBhytName = data.HEIN_SERVICE_BHYT_NAME;
                    this.PRICING_MAX_REUSE_COUNT = data.PRICING_MAX_REUSE_COUNT;
                    this.MAP_MEDI_MATE_ID = data.MATERIAL_TYPE_MAP_ID;
                    this.VS_PRICE = data.REUSE_FEE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public enum Error
        {
            ThieuMa, ThieuGiaNhap, ThieuVat, ThieuSoLuong, SaiGiaNhap, SaiVat, SaiSoLuong, SaiMa, MaxLengthGoiThau, MaxLengthNhomThau, MaxLengthQuocGia, MaxLenthNongDoHamLuong, MaxLengthMaHangSX, MaxLengthSoDangKy, MaQuetDinhThauVuotQuaKiTu, LoaiThongTinThauKhongHopLe, ThongTinThauVuotQuaKiTu, ThieuDangBaoChe

        }
        public enum Warm
        {
            ThangKhongHopLe, NgayKhongHopLe, GioKhongHopLe, TuoiThoThangVuotQuaDoDaiChoPhep, TuoiThoNgayVuotQuaDoDaiChoPhep, TuoiThoGioVuotQuaDoDaiChoPhep, KhongCoTuoiTho, KhongHopLe, KhongTonTai, HanDungKhongHopLe

        }
    }
}
