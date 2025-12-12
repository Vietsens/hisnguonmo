using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.Library.TwoIDStorageIntegration
{
    public class TwoIDStorageIntegrationProcessor
    {
        static void Main(string[] args)
        {
            // Example usage
            var processor = new TwoIDStorageIntegrationProcessor();
            // Call methods as needed
            Console.WriteLine("App started.");
        }
        private readonly ConfigCFG.StorageConfig config;
        public TwoIDStorageIntegrationProcessor()
        {
            config = ConfigCFG.GetStorageConfig();
        }

        // 1. Upload danh sách file
        public TwoIDApiRequestInput UploadFiles(string citizenNumber, string apiKey, string transactionId, string hash)
        {
            return TwoIDApiRequestInput.CallTwoIDApi<TwoIDApiRequestInput>(
                config.ApiBaseUrl,
                "/api/v1/files/uploads",
                citizenNumber,
                null, null, null,
                apiKey,
                transactionId,
                hash,
                "application/json"
            );
        }
        // 2. Lấy thông tin CCCD
        public TwoIDApiRequestInput GetCitizenInfo(string citizenNumber, string apiKey, string transactionId, string hash)
        {
            return TwoIDApiRequestInput.CallTwoIDApi<TwoIDApiRequestInput>(
                config.ApiBaseUrl,
                "/api/v1/citizens",
                null,
                null, null, null,
                apiKey,
                transactionId,
                hash,
                 "x-www-form-urlencoded"
            );
        }
        // 3. Download dữ liệu file
        public TwoIDApiRequestInput DownloadFile(string citizenNumber, string apiKey, string transactionId, string hash)
        {
            return TwoIDApiRequestInput.CallTwoIDApi<TwoIDApiRequestInput>(
                config.ApiBaseUrl,
                "/api/v1/files/download",
                citizenNumber,
                null, null, null,
                apiKey,
                transactionId,
                hash,
                "application/x-www-form-urlencoded"

            );
        }
        // 4. Đồng bộ dữ liệu cá nhân
        public TwoIDApiRequestInput SyncPersonalData(
            string citizenNumber,
            List<string> fingerprint,
            List<string> faceId,
            List<string> handSignature,
            string apiKey,
            string transactionId,
            string hash,
            string contentType)
        {
            return TwoIDApiRequestInput.CallTwoIDApi<TwoIDApiRequestInput>(
                config.ApiBaseUrl,
                "/api/v1/citizens",
                citizenNumber,
                fingerprint,
                faceId,
                handSignature,
                apiKey,
                transactionId,
                hash,
                "application/json"
            );
        }
    }
}
