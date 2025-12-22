using System;

namespace HIS.Desktop.Plugins.Library.TwoIDStorageIntegration
{
    /// <summary>
    /// Response t? 2ID Storage API
    /// </summary>
    public class TwoIDApiResult
    {
        public bool status { get; set; }
        public string timestamp { get; set; }
        public string transId { get; set; }
        public TwoIDCitizenData data { get; set; }
    }

    /// <summary>
    /// Thông tin công dân t? 2ID Storage
    /// </summary>
    public class TwoIDCitizenData
    {
        public string fullName { get; set; }
        public string citizenNumber { get; set; }
        public string oldIdentifyNumber { get; set; }
        public string dateOfBirth { get; set; }
        public string gender { get; set; }
        public string nationality { get; set; }
        public string originPlace { get; set; }
        public string ethnic { get; set; }
        public string religion { get; set; }
        public string residencePlace { get; set; }
        public string issuePlace { get; set; }
        public string issueDate { get; set; }
        public string expiredDate { get; set; }
        public string identification { get; set; }
        
        /// <summary>
        /// Path ?nh vân tay: "001202024259/e8a69094-1477-4198-be6a-da4d5bf368b6.jpg"
        /// </summary>
        public string fingerPrintImage { get; set; }
        
        /// <summary>
        /// Path ?nh khuôn m?t: "001202024259/e8a69094-1477-4198-be6a-da4d5bf368b6.jpg"
        /// </summary>
        public string faceImage { get; set; }
        
        /// <summary>
        /// Path ?nh ch? ký: "001202024259/a8adc286-d08d-428c-9470-aa26dceefabd.png"
        /// </summary>
        public string handSignatureImage { get; set; }
        
        public string idCardVerifyResult { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }
}
