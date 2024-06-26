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
using SAR.EFMODEL.DataModels;
using System;
using System.Linq;

namespace MPS.Core
{
    class ProcessorFactory
    {
        internal static IProcessorPrint MakeProcessorBase(string printCode, string fileName, object data, MPS.Printer.PreviewType previewType, string printerName)
        {
            IProcessorPrint result = null;
            try
            {
                SAR_PRINT_TYPE config = null;
                try
                {
                    config = PrintConfig.PrintTypes.Where(o => o.PRINT_TYPE_CODE == printCode).SingleOrDefault();
                    if (config == null) throw new Exception("Khong tim duoc config.");
                }
                catch (Exception ex)
                {
                    throw new Exception("Khong truy van duoc du lieu cau hinh in an theo cac thong tin truyen vao. Kiem tra lai frontend & SAR_PRINT_TYPE. Khong the khoi tao doi tuong xu ly in an." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => printCode), printCode) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileName), fileName), ex);
                }

                try
                {
                    //Tat ca cac mau in an phai khai bao o day
                    //Moi khi them mot chuc nang in an moi thi bo xung them 1 dong
                    switch (config.PRINT_TYPE_CODE)
                    {
                        //case "Mps000001":
                        //    result = new Mps000001.Mps000001Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000002": // Bảng kê thanh toán Ngoại trú BHYT
                        //    result = new Mps000002.Mps000002Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000003": // Bảng kê thanh toán Ngoại trú BHYT - Template 8
                        //    result = new Mps000003.Mps000003Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000004": // Bảng kê thanh toán Ngoại trú Viện phí
                        //    result = new Mps000002.Mps000002Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000005": // Bảng kê thanh toán Nội trú BHYT
                        //    result = new Mps000002.Mps000002Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000006":// Bảng kê thanh toán Nội trú Viện Phí
                        //    result = new Mps000002.Mps000002Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        case "Mps000007"://Biểu mẫu phiếu yêu cầu khám bệnh vào viện
                            result = new Mps000007.Mps000007Processor(config, fileName, data, previewType, printerName);
                            break;
                        //case "Mps000008"://In giấy ra viện
                        //    result = new Mps000008.Mps000008Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000010"://In giấy hẹn khám
                        //    result = new Mps000010.Mps000010Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000011"://In giấy chuyển viện
                        //    result = new Mps000011.Mps000011Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000012"://Biểu mẫu phiếu yêu cầu bệnh án ngoại trú
                        //    result = new Mps000012.Mps000012Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000013"://Biểu mẫu phiếu yêu cầu khám sức khỏe cán bộ
                        //    result = new Mps000013.Mps000013Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000014"://Biểu mẫu phiếu yêu cầu in kết quả xét nghiệm
                        //    result = new Mps000014.Mps000014Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000015"://Biểu mẫu phiếu yêu In kết quả chụp cát lớp vi tính
                        //    result = new Mps000015.Mps000015Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000016"://Biểu mẫu phiếu yêu In kết qủa chiếu chụp cộng hưởng từ
                        //    result = new Mps000016.Mps000016Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000017"://Biểu mẫu phiếu yêu In kết qủa chiếu chụp X quang
                        //    result = new Mps000017.Mps000017Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000018"://Biểu mẫu phiếu yêu In kết qủa chiếu chụp X quang các dịch vụ
                        //    result = new Mps000018.Mps000018Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000019"://In Trích biên bản hội chẩn
                        //    result = new Mps000019.Mps000019Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000020"://In sổ biên bản hội chẩn
                        //    result = new Mps000020.Mps000020Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000021"://In phiếu kết quả nội soi
                        //    result = new Mps000021.Mps000021Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000022"://In kết quả điện tim
                        //    result = new Mps000022.Mps000022Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000023"://In kết quả điện não
                        //    result = new Mps000023.Mps000023Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000024"://In kết quả siêu âm
                        //    result = new Mps000024.Mps000024Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000025"://In Yêu cầu khám
                        //    result = new Mps000025.Mps000025Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000026"://In Yêu cầu xét nghiệm
                        //    result = new Mps000026.Mps000026Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000027"://In phiếu xét nghiệm đờm soi trực tiếp
                        //    result = new Mps000027.Mps000027Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000028"://In Yêu cầu chuẩn đoán hình ảnh
                        //    result = new Mps000028.Mps000028Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000029"://In Yêu cầu nội soi
                        //    result = new Mps000029.Mps000029Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000030"://In Yêu cầu siêu âm
                        //    result = new Mps000030.Mps000030Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000031"://In Yêu cầu thủ thuật
                        //    result = new Mps000031.Mps000031Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000032"://Phiếu kết quả nội soi
                        //    result = new Mps000031.Mps000031Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000033"://In phiếu phẫu thuật thủ thuật
                        //    result = new Mps000033.Mps000033Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000034"://In Phiếu yêu cầu khám
                        //    result = new Mps000034.Mps000034Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000035"://In giấy cam đoan Pttt và gây mê 
                        //    result = new Mps000035.Mps000035Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000036"://In yêu cầu phẫu thuật
                        //    result = new Mps000036.Mps000036Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000037"://In Phiếu chỉ định tổng hợp
                        //    result = new Mps000037.Mps000037Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000038"://In Phiếu yêu cầu thăm dò chứ năng
                        //    result = new Mps000038.Mps000038Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000039"://In Phiếu yêu cầu in kết quả tahwm dò chức năng
                        //    result = new Mps000039.Mps000039Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000040"://In Phiếu yêu cầu dịch vụ khác
                        //    result = new Mps000040.Mps000040Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000041"://In Phiếu yêu cầu in kết quả dịch vụ khác
                        //    result = new Mps000041.Mps000041Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000042"://In Phiếu yêu cầu giường
                        //    result = new Mps000042.Mps000042Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000043"://In Đơn thuốc vật tư
                        //    result = new Mps000043.Mps000043Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000044"://In Phiếu yêu cầu in đơn thuốc
                        //    result = new Mps000044.Mps000044Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000045"://Bảng kê theo khoa
                        //    result = new Mps000045.Mps000045Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        case "Mps000046"://In Phiếu lĩnh tổng hợp
                            result = new Mps000046.Mps000046Processor(config, fileName, data, previewType, printerName);
                            break;
                        case "Mps000047"://In Phiếu tra đối thuốc
                            result = new Mps000047.Mps000047Processor(config, fileName, data, previewType, printerName);
                            break;
                        case "Mps000048"://In Phiếu lĩnh gây nghiện hướng tâm thần
                            result = new Mps000048.Mps000048Processor(config, fileName, data, previewType, printerName);
                            break;
                        case "Mps000049"://In Phiếu lĩnh thuốc vật tư
                            result = new Mps000049.Mps000049Processor(config, fileName, data, previewType, printerName);
                            break;
                        //case "Mps000050"://In Phiếu yêu cầu in đơn thuốc y học cổ truyền
                        //    result = new Mps000050.Mps000050Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000051"://In Phiếu yêu cầu in đơn thuốc trong danh mục
                        //    result = new Mps000051.Mps000051Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000052"://Bảng kê gom nhóm theo khoa ngoại trú
                        //    result = new Mps000052.Mps000052Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000054"://In Phiếu yêu cầu in đơn thuốc tổng hợp
                        //    result = new Mps000054.Mps000054Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000055"://In Phiếu yêu cầu in giấy thử phản ứng thuốc
                        //    result = new Mps000055.Mps000055Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000056"://In Phiếu yêu cầu in kết quả giấy thử phản ứng thuốc
                        //    result = new Mps000056.Mps000056Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000057"://In Phiếu yêu cầu in kết quả giấy thử phản ứng thuốc
                        //    result = new Mps000057.Mps000057Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000058"://In Phiếu yêu 
                        //    result = new Mps000058.Mps000058Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000059"://Bảng kê gom nhóm theo khoa nội trú
                        //    result = new Mps000052.Mps000052Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000060"://Bảng kê không gom nhóm theo khoa nội trú
                        //    result = new Mps000060.Mps000060Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000061"://Bảng kê không gom nhóm theo khoa ngoại trú
                        //    result = new Mps000060.Mps000060Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000062"://In Phiếu yêu cầu in tờ điều trị
                        //    result = new Mps000062.Mps000062Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000063"://In Phiếu yêu cầu in kết quả PHCN tổng hợp
                        //    result = new Mps000063.Mps000063Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000064"://In Phiếu yêu cầu chỉ định kĩ thuật PHCN
                        //    result = new Mps000064.Mps000064Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000053"://In Phiếu yêu cầu PHCN
                        //    result = new Mps000053.Mps000053Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000065"://In Phiếu trích biên bản hội chẩn
                        //    result = new Mps000065.Mps000065Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000066"://In Phiếu sổ biên bản hội chẩn
                        //    result = new Mps000066.Mps000066Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000067"://In hướng dẫn sử dụng thuốc
                        //    result = new Mps000067.Mps000067Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000069"://In kết quả chăm sóc
                        //    result = new Mps000069.Mps000069Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000070"://In kết quả chăm sóc
                        //    result = new Mps000044.Mps000044Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000071"://Yêu cầu khám chuyển khoa
                        //    result = new Mps000071.Mps000071Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000072"://In tờ điều trị tổng hợp
                        //    result = new Mps000072.Mps000072Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000073"://In kết quả chăm sóc tổng hợp
                        //    result = new Mps000069.Mps000069Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        ////case "Mps000074"://In kết quả truyeefn dịch
                        ////		result = new Mps000074.Mps000074Processor(config, fileName, data, previewType, printerName);
                        ////		break;
                        //case "Mps000075"://In bảng kê bhyt ngoại trú tổng hợp
                        //    result = new Mps000075.Mps000075Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000076"://In bảng kê bhyt nội trú tổng hợp
                        //    result = new Mps000076.Mps000076Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000077"://In barcode
                        //    result = new Mps000077.Mps000077Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000078"://In danh sach phieu tra
                        //    result = new Mps000078.Mps000078Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000079"://In bangr kee theo khoa 2
                        //    result = new Mps000079.Mps000079Processor(config, fileName, data, previewType, printerName);
                        //    break;

                        //case "Mps000080"://In bangr kee BHYT tong hop
                        //    result = new Mps000080.Mps000080Processor(config, fileName, data, previewType, printerName);
                        //    break;

                        //case "Mps000081"://In bangr kee BHYT tong hop
                        //    result = new Mps000080.Mps000080Processor(config, fileName, data, previewType, printerName);
                        //    break;

                        //case "Mps000082"://In bangr tong hop Ngoai Tru 2
                        //    result = new Mps000082.Mps000082Processor(config, fileName, data, previewType, printerName);
                        //    break;

                        //case "Mps000083"://In bangr tong hop Noi Tru 2
                        //    result = new Mps000082.Mps000082Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000084"://In phiếu nhập thu hồi
                        //    result = new Mps000084.Mps000084Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000085"://Biên bản kiểm kê nhập từ nhà cung cấp
                        //    result = new Mps000085.Mps000085Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000086"://Phiếu xuất chuyển kho
                        //    result = new Mps000086.Mps000086Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000068"://In bệnh án y học cổ truyền
                        //    result = new Mps000068.Mps000068Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000087"://Bảng kê theo khoa BHYT
                        //    result = new Mps000087.Mps000087Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000088"://Phiếu công khai thuốc
                        //    result = new Mps000088.Mps000088Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        case "Mps000089"://Phiếu xuất chuyển kho thuốc gây nghiện hướng thần
                            result = new Mps000089.Mps000089Processor(config, fileName, data, previewType, printerName);
                            break;
                        case "Mps000090"://Phiếu xuất chuyển kho thuốc không phải gây nghiện hướng thần (Gồm cả vật tư)
                            result = new Mps000090.Mps000090Processor(config, fileName, data, previewType, printerName);
                            break;
                        //case "Mps000091"://Phiếu yêu cầu tạm ứng
                        //    result = new Mps000091.Mps000091Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000092"://Phiếu xuất bán
                        //    result = new Mps000092.Mps000092Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000093"://Phiếu trả tra đổi thuốc
                        //    result = new Mps000093.Mps000093Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000095"://Barcode beenhj nhan chuong trinh
                        //    result = new Mps000095.Mps000095Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        ////case "Mps000094"://In barcode HSBA
                        ////    result = new Mps000094.Mps000094Processor(config, fileName, data, previewType, printerName);
                        ////    break;
                        //case "Mps000096"://In kết quả xét nghiệm KXN
                        //    result = new Mps000096.Mps000096Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000097"://In biểu mẫu cách thức phẫu thuật
                        //    result = new Mps000097.Mps000097Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000098"://Bảng kê chi phí ktc
                        //    result = new Mps000098.Mps000098Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000099"://In hướng dẫn sử dụng thuốc
                        //    result = new Mps000099.Mps000099Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000100"://In phiếu trả thuốc, vật tư
                        //    result = new Mps000093.Mps000093Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000101"://In phiếu trả thuốc gây nghiện, hướng thần
                        //    result = new Mps000101.Mps000101Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000102"://In phiếu thu phí dịch vụ
                        //    result = new Mps000102.Mps000102Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000103"://Hóa đơn thanh toán theo yêu cầu dịch vụ
                        //    result = new Mps000103.Mps000103Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000104"://Thanh toan theo loai dich vu
                        //    result = new Mps000104.Mps000104Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000105"://Phiếu chỉ định dịch vụ dựa vào giao dịch thanh toán
                        //    result = new Mps000105.Mps000105Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000106"://Hóa đơn thanh toán chi tiết dịch vụ
                        //    result = new Mps000106.Mps000106Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000107"://Phieu linh mau
                        //    result = new Mps000107.Mps000107Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000108"://Phieu yeu cau mau
                        //    result = new Mps000108.Mps000108Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000109"://Phieu tạm ứng theo dịch vụ
                        //    result = new Mps000109.Mps000109Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000110"://Phieu hoàn ứng theo dịch vụ
                        //    result = new Mps000110.Mps000110Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000111"://Phiếu thu thanh toán tổng hợp
                        //    result = new Mps000111.Mps000111Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000112"://Phiếu thu tạm ứng tổng hợp
                        //    result = new Mps000112.Mps000112Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000113"://Phiếu thu hoàn ứng tổng hợp
                        //    result = new Mps000113.Mps000113Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000114"://Biên lại thu phí lệ phí
                        //    result = new Mps000114.Mps000114Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        case "Mps000115"://In hóa đơn
                            result = new Mps000115.Mps000115Processor(config, fileName, data, previewType, printerName);
                            break;
                        //case "Mps000116"://Phiếu công khai thuốc theo ngày
                        //    result = new Mps000116.Mps000116Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000117":// Phiếu dự trù thuốc
                        //    result = new Mps000117.Mps000117Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        ////case "Mps000118":// In đơn thuốc tổng hợp v2
                        ////    result = new Mps000118.Mps000118Processor(config, fileName, data, previewType, printerName);
                        ////    break;
                        //case "Mps000119"://In Chi tiết gói thầu
                        //    result = new Mps000119.Mps000119Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        ////case "Mps000120"://Bảng kê thanh toán ngoại trú, nội trú BHYT
                        ////    result = new Mps000120.Mps000120Processor(config, fileName, data, previewType, printerName);
                        ////    break;
                        ////case "Mps000121"://Bảng kê thanh toán ngoại trú, nội trú BHYT
                        ////    result = new Mps000120.Mps000120Processor(config, fileName, data, previewType, printerName);
                        ////    break;
                        ////case "Mps000122"://Bảng kê thanh toán ngoại trú,nội trú viện phí
                        ////    result = new Mps000122.Mps000122Processor(config, fileName, data, previewType, printerName);
                        ////    break;
                        ////case "Mps000123"://Bảng kê thanh toán ngoại trú,nội trú viện phí
                        ////    result = new Mps000122.Mps000122Processor(config, fileName, data, previewType, printerName);
                        ////    break;
                        ////case "Mps000124"://Bảng kê thanh toán tổng hợp
                        ////    result = new Mps000124.Mps000124Processor(config, fileName, data, previewType, printerName);
                        ////    break;
                        ////case "Mps000125"://Bảng kê thanh toán tổng hợp BHYT nội trú
                        ////    result = new Mps000125.Mps000125Processor(config, fileName, data, previewType, printerName);
                        ////    break;
                        ////case "Mps000126"://Bảng kê thanh toán tổng hợp BHYT ngoại trú
                        ////    result = new Mps000125.Mps000125Processor(config, fileName, data, previewType, printerName);
                        //    //break;
                        case "Mps000127"://Bảng kê thanh toán theo khoa
                            result = new Mps000127.Mps000127Processor(config, fileName, data, previewType, printerName);
                            break;
                        ////case "Mps000128"://Bảng kê chi phí ktc
                        ////    result = new Mps000128.Mps000128Processor(config, fileName, data, previewType, printerName);
                        ////    break;
                        //case "Mps000130"://Phiếu yêu cầu xuất trả nhà cung cấp
                        //    result = new Mps000130.Mps000130Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000131"://Phiếu tổng hợp tồn kho T_VT_M
                        //    result = new Mps000131.Mps000131Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000132":// Biên bản kiểm kê
                        //    result = new Mps000132.Mps000132Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        case "Mps000134":// Biên bản kiểm kê
                            result = new Mps000134.Mps000134Processor(config, fileName, data, previewType, printerName);
                            break;
                        case "Mps000135":// Phiếu xuất sử dụng (xuất cho khoa phòng)
                            result = new Mps000135.Mps000135Processor(config, fileName, data, previewType, printerName);
                            break;
                        //case "Mps000138":// In so thu tu tiep don
                        //    result = new Mps000138.Mps000138Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000140":// In phieu nhap mau
                        //    result = new Mps000140.Mps000140Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000141":// In phieu nhập từ nhà cung cấp
                        //    result = new Mps000141.Mps000141Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000142":// Phiếu nhập chuyển kho thuốc gây nghiện, hướng thần
                        //    result = new Mps000142.Mps000142Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000143":// Phiếu nhập chuyển kho 
                        //    result = new Mps000143.Mps000143Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000144":// In phiếu nhập thu hồi
                        //    result = new Mps000144.Mps000144Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000145":// Phiếu nhập chuyển kho thuốc không phải gây nghiện, hướng thần
                        //    result = new Mps000145.Mps000145Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000146":// Phiếu Truyền dịch
                        //    result = new Mps000146.Mps000146Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000147":// Phiếu hóa đơn thanh toán
                        //    result = new Mps000147.Mps000147Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000148":// Phiếu biên lai thanh toán
                        //    result = new Mps000148.Mps000148Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000149":// Phiếu Nhập Máu Từ Nhà Cung Cấp
                        //    result = new Mps000149.Mps000149Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        ////case "Mps000152":// Hóa đơn chi tiết dịch vụ
                        ////		result = new Mps000152.Mps000152Processor(config, fileName, data, previewType, printerName);
                        ////		break;
                        //case "Mps000154":// Phiếu xuất hao phí
                        //    result = new Mps000154.Mps000154Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000155":// Phiếu xuất thanh lý
                        //    result = new Mps000155.Mps000155Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        case "Mps000151":// Phiếu tổng hợp chăm sóc
                            result = new Mps000151.Mps000151Processor(config, fileName, data, previewType, printerName);
                            break;
                        //case "Mps000152":// Hóa đơn đỏ phân trang
                        //    result = new Mps000152.Mps000152Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        case "Mps000156":// Phiếu thử phản ứng thuốc
                            result = new Mps000156.Mps000156Processor(config, fileName, data, previewType, printerName);
                            break;
                        case "Mps000158":// Phiếu thử phản ứng thuốc
                            result = new Mps000158.Mps000158Processor(config, fileName, data, previewType, printerName);
                            break;
                        case "Mps000159":// Phiếu thử phản ứng thuốc
                            result = new Mps000158.Mps000158Processor(config, fileName, data, previewType, printerName);
                            break;
                        case "Mps000160":// Phiếu thử phản ứng thuốc
                            result = new Mps000160.Mps000160Processor(config, fileName, data, previewType, printerName);
                            break;
                        case "Mps000161":// Phiếu thử phản ứng thuốc
                            result = new Mps000160.Mps000160Processor(config, fileName, data, previewType, printerName);
                            break;
                        case "Mps000162":// Phiếu thử phản ứng thuốc
                            result = new Mps000162.Mps000162Processor(config, fileName, data, previewType, printerName);
                            break;
                        case "Mps000163":// Phiếu thử phản ứng thuốc
                            result = new Mps000162.Mps000162Processor(config, fileName, data, previewType, printerName);
                            break;
                        case "Mps000164":// Bang ke vien phi thanh toan 100%
                            result = new Mps000164.Mps000164Processor(config, fileName, data, previewType, printerName);
                            break;
                        case "Mps000165":// Phiếu xuất khác
                            result = new Mps000165.Mps000165Processor(config, fileName, data, previewType, printerName);
                            break;
                        //case "Mps000166": // Biên bản xác nhận hỏng
                        //    result = new Mps000166.Mps000166Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000167": // Biên bản xác nhận hỏng
                        //    result = new Mps000167.Mps000167Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000168": // Phiếu xuất mất mát
                        //    result = new Mps000168.Mps000168Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        case "Mps000169": //GN_HT
                            result = new Mps000169.Mps000169Processor(config, fileName, data, previewType, printerName);
                            break;
                        case "Mps000174": //Benh an ngoai tru
                            result = new Mps000174.Mps000174Processor(config, fileName, data, previewType, printerName);
                            break;
                        case "Mps000175": //Phiếu lĩnh vật tư
                            result = new Mps000175.Mps000175Processor(config, fileName, data, previewType, printerName);
                            break;
                        //case "Mps000176": //Phiếu lĩnh vật tư
                        //    result = new Mps000176.Mps000176Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000182": //bang ke ngoai tru BHYT 01bv
                        //    result = new Mps000182.Mps000182Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000183": //bang ke ngoai tru BHYT 01bv
                        //    result = new Mps000183.Mps000183Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000188": //bang ke ngoai tru BHYT 01bv
                        //    result = new Mps000188.Mps000188Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000189": //bang ke ngoai tru BHYT 01bv
                        //    result = new Mps000189.Mps000189Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000184": //bang ke ngoai tru BHYT 01bv
                        //    result = new Mps000184.Mps000184Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000185": //bang ke ngoai tru BHYT 01bv
                        //    result = new Mps000185.Mps000185Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000186": //bang ke ngoai tru BHYT 01bv
                        //    result = new Mps000186.Mps000186Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        //case "Mps000187": //bang ke ngoai tru BHYT 01bv
                        //    result = new Mps000187.Mps000187Processor(config, fileName, data, previewType, printerName);
                        //    break;
                        case "Mps000193": //bang ke ngoai tru BHYT 01bv
                            result = new Mps000193.Mps000193Processor(config, fileName, data, previewType, printerName);
                            break;
                        case "Mps000194": //bang ke ngoai tru BHYT 01bv
                            result = new Mps000194.Mps000194Processor(config, fileName, data, previewType, printerName);
                            break;
                        case "Mps000195": //bang ke ngoai tru BHYT 01bv
                            result = new Mps000194.Mps000194Processor(config, fileName, data, previewType, printerName);
                            break;
                        case "Mps000196": //bang ke ngoai tru BHYT 01bv
                            result = new Mps000196.Mps000196Processor(config, fileName, data, previewType, printerName);
                            break;
                        case "Mps000197": //bang ke ngoai tru BHYT 01bv
                            result = new Mps000196.Mps000196Processor(config, fileName, data, previewType, printerName);
                            break;
                        case "Mps000198": //Phieu xuat chuyen kho mau
                            result = new Mps000198.Mps000198Processor(config, fileName, data, previewType, printerName);
                            break;
                        //case "Mps000202": //Phieu xuat chuyen kho mau
                        //    result = new Mps000127.Mps000127Processor(config, fileName, data, previewType, printerName);
                        //    break;
                    }
                }
                catch (Exception)
                {
                    throw new NullReferenceException();
                }

            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => printCode), printCode) + data.GetType().ToString() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data), ex);
                result = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
