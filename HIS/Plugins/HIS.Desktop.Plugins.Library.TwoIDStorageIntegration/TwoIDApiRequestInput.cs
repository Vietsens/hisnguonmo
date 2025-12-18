using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using HIS.Desktop.Plugins.Library.TwoIDStorageIntegration;

namespace HIS.Desktop.Plugins.Library.TwoIDStorageIntegration
{
    public class TwoIDApiRequestInput
    {

        public string citizenNumber { get; set; }
        public List<string> fingerprint { get; set; }
        public List<string> faceId { get; set; }
        public List<string> handSignature { get; set; }
        public string apiKey { get; set; }
        public string transactionId { get; set; }
        public string hash { get; set; }
        public string fileName { get; set; }
        public string type { get; set; }
        public string fullName { get; set; }
        public DateTime dateOfBirth { get; set; }
        public string residencePlace { get; set; }
        public DateTime issueDate { get; set; }
        public DateTime expiredDate { get; set; }
        public List<string> idCardVerifyResult { get; set; }




        public TwoIDApiRequestInput()
        {
            fingerprint = new List<string>();
            faceId = new List<string>();
            handSignature = new List<string>();
            idCardVerifyResult = new List<string>();
        }

        public TwoIDApiRequestInput(TwoIDApiRequestInput data)
        {
            if (data != null)
            {
                this.citizenNumber = data.citizenNumber;
                this.fingerprint = data.fingerprint ?? new List<string>();
                this.faceId = data.faceId ?? new List<string>();
                this.handSignature = data.handSignature ?? new List<string>();
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
                this.idCardVerifyResult = data.idCardVerifyResult ?? new List<string>();

            }
        }
        public static T CallTwoIDApi<T>(
   string baseUri,
   string requestUri,
   string citizenNumber,
   List<string> fingerprint,
   List<string> faceId,
   List<string> handSignature,
   string apiKey,
   string transactionId,
   string hash,
   string fileName,
   string type,
   string fullName,
   DateTime dateOfBirth,
   string residencePlace,
   DateTime issueDate,
   DateTime expiredDate,
   List<string> idCardVerifyResult,
   string contentType)
        {
            var input = new TwoIDApiRequestInput
            {
                citizenNumber = citizenNumber,
                fingerprint = fingerprint ?? new List<string>(),
                faceId = faceId ?? new List<string>(),
                handSignature = handSignature ?? new List<string>(),
                apiKey = apiKey,
                transactionId = transactionId,
                hash = hash,

                fileName = fileName,
                type = type,
                fullName = fullName,
                dateOfBirth = dateOfBirth,
                residencePlace = residencePlace,
                issueDate = issueDate,
                expiredDate = expiredDate,
                idCardVerifyResult = idCardVerifyResult ?? new List<string>()
            };

            return StorageApiClient.CreateRequest<T>(baseUri, requestUri, input, contentType);


        }


        public static T CallTwoIDBasic<T>(
    string baseUri,
    string requestUri,
    string citizenNumber,
    List<string> fingerprint,
    List<string> faceId,
    List<string> handSignature,
    string apiKey,
    string transactionId,
    string hash,
    string contentType)
        {
            var input = new TwoIDApiRequestInput
            {
                citizenNumber = citizenNumber,
                fingerprint = fingerprint ?? new List<string>(),
                faceId = faceId ?? new List<string>(),
                handSignature = handSignature ?? new List<string>(),
                apiKey = apiKey,
                transactionId = transactionId,
                hash = hash
            };

            return StorageApiClient.CreateRequest<T>(baseUri, requestUri, input, contentType);
        
        
        }

        public static T CallTwoIDApi<T>(
    string baseUri,
    string requestUri,
    TwoIDApiRequestInput input,
    string contentType)
        {
            return StorageApiClient.CreateRequest<T>(
                baseUri,
                requestUri,
                input,
                contentType
            );
        }

    }
}
