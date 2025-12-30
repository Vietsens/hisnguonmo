using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.TwoIDStorageIntegration
{
    public class TwoIdRequestCitizens
    {
        // URL ảnh (sau Upload)
        public string citizenNumber { get; set; }
        //common

        public string apiKey { get; set; }
        public string transactionId { get; set; }
        public string hash { get; set; }

        //Download

        public string fileName { get; set; }
        public string type { get; set; }

        //Sync

        public string fullName { get; set; }
        //[JsonConverter(typeof(DateOnlyConverter))]
        public string dateOfBirth { get; set; }
        //public DateTime dateOfBirth { get; set; }
        public string residencePlace { get; set; }
        //[JsonConverter(typeof(DateOnlyConverter))]
        public string issueDate { get; set; }

        //[JsonConverter(typeof(DateOnlyConverter))]
        public string expiredDate { get; set; }
        public object idCardVerifyResult { get; set; }

        public string gender { get; set; }
        public string status { get; set; }

        //url upload
        public string fingerPrintImage { get; set; }
        public string faceImage { get; set; }
        public string handSignatureImage { get; set; }

        public TwoIdRequestCitizens()
        {

        }

        public TwoIdRequestCitizens(TwoIdRequestCitizens data)
        {
            if (data != null)
            {
                this.citizenNumber = data.citizenNumber;
                this.apiKey = data.apiKey;
                this.transactionId = data.transactionId;
                this.hash = data.hash;

                this.fileName = data.fileName;
                this.type = data.type;
                this.fullName = data.fullName;
                this.dateOfBirth = data.dateOfBirth;
                this.residencePlace = data.residencePlace;
                this.issueDate = data.issueDate;
                this.expiredDate = data.expiredDate;
                this.idCardVerifyResult = data.idCardVerifyResult;
                this.fingerPrintImage = data.fingerPrintImage;
                this.faceImage = data.faceImage;
                this.handSignatureImage = data.handSignatureImage;
            }
        }


        public static T CallTwoIDApi<T>(string baseUri, string requestUri, TwoIDApiRequestInput input, string contentType)
        {
            return StorageApiClient.CreateRequest<T>(baseUri, requestUri, input, contentType);
        }

    }


}
