using System;
using Newtonsoft.Json;

namespace EMR.Desktop.Plugins.EmrPatientCertificateRegister.DTO
{
    public class RootResponse
    {
        public bool success { get; set; }
        public DateTime? serverTime { get; set; }
        public ResultCCCD result { get; set; }
    }

    public class ResultCCCD
    {
        public CccdData data { get; set; }
    }

    public class CccdData
    {
        // Thông tin cơ bản
        public string identifyNumber { get; set; }
        public string previousNumber { get; set; }
        public string name { get; set; }
        public string dateOfBirth { get; set; }
        public string sex { get; set; }
        public string address { get; set; }
        public string issueDate { get; set; }
        public string expiredDate { get; set; }
        public string issuePlace { get; set; }

        // Thêm nhiều trường trong JSON
        public string nationality { get; set; }
        public string nation { get; set; }
        public string religion { get; set; }
        public string character { get; set; }   // mô tả đặc điểm
        public string hometown { get; set; }
        public string fatherName { get; set; }
        public string motherName { get; set; }
        public string partnerName { get; set; }
        public string otherName { get; set; }
        public string email { get; set; }
        public string patientCode { get; set; }
        public string phone { get; set; }
        public string mrz { get; set; }
        public string dsCert { get; set; }
        public bool? isPass { get; set; }
        public double? score { get; set; }

        // Các cặp dữ liệu base64 (ảnh / dữ liệu DG)
        public string dg1DataBase64 { get; set; }
        public string dg2DataBase64 { get; set; }   // thường chứa ảnh (jpeg) theo file mẫu
        public string dg13DataBase64 { get; set; }
        public string dg14DataBase64 { get; set; }
        public string sodData { get; set; }

        // Có thể có nhiều trường ảnh: imageFront, imageChip, imageCap...
        public string imageFront { get; set; }   // tên cũ - giữ nếu service có
        public string imageChip { get; set; }    // xuất hiện trong JSON
        public string imageCap { get; set; }     // nếu có
        // ... nếu cần, thêm các trường khác từ JSON

        // Một số cờ/flags
        public int? verifySOD { get; set; }
        public int? aaCaAuthen { get; set; }
    }
}
