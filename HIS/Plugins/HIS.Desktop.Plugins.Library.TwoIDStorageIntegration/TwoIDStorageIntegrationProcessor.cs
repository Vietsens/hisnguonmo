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

        private readonly ConfigCFG.StorageConfig config;
        private readonly string apiKey;
        private readonly string baseUri;
        public TwoIDStorageIntegrationProcessor()
        {
            HIS.Desktop.LocalStorage.EmrConfig.ConfigLoader.Refresh();
            config = ConfigCFG.GetStorageConfig();

            this.baseUri = config.ApiBaseUrl;
            this.apiKey = config.ApiKey;
            //this.secretKey = config.ApiSecret;
        }

        // Upload danh sách file
        public TwoIDApiRequestInput UploadFiles(string baseUri, string citizenNumber, object fingerprintFiles, object faceFiles, object handSignatureFiles, string apiKey, string transactionId, string hash)
        {
            return StorageApiClient.CreateMultipartRequest<TwoIDApiRequestInput>(
                baseUri,
                "/api/v1/files/uploads",
                citizenNumber,
                fingerprintFiles,
                faceFiles,
                handSignatureFiles,
                apiKey,
                transactionId,
                 hash
     );
        }
        // Lấy thông tin CCCD



        public TwoIDApiRequestInput GetCitizenInfo(string baseUri, string citizenNumber, string apiKey, string transactionId, string hash)
        {
            var input = new TwoIDApiRequestInput
            {
                citizenNumber = citizenNumber,
                apiKey = apiKey,
                transactionId = transactionId,
                hash = hash
            };

            return StorageApiClient.CreateRequest<TwoIDApiRequestInput>(baseUri, "/api/v1/citizens", input, "application/x-www-form-urlencoded");
        }
        //Download dữ liệu file
        public TwoIDApiRequestInput DownloadFile(string baseUri, string fileName, string apiKey, string transactionId, string hash)
        {

            var input = new TwoIDApiRequestInput
            {
                fileName = fileName,
                apiKey = apiKey,
                transactionId = transactionId,
                hash = hash
            };
            return StorageApiClient.CreateRequest<TwoIDApiRequestInput>(baseUri, "/api/v1/files/download", input, "application/x-www-form-urlencoded");
        }
        // Đồng bộ dữ liệu cá nhân
        public TwoIDApiRequestInput SyncPersonalData(string baseUri, TwoIDApiRequestInput input, string apiKey, string transactionId, string hash)
        {

            input.apiKey = apiKey;
            input.transactionId = transactionId;
            input.hash = hash;

            return TwoIDApiRequestInput.CallTwoIDApi<TwoIDApiRequestInput>(
                baseUri,
                "/api/v1/citizens",
                input,
                "application/json"
            );
        }


        //check CCCD

        public bool IsCitizenInfoExists(string baseUri,
      string citizenNumber,
      string apiKey,
      string transactionId,
      string hash)
        {
            try
            {
                var info = GetCitizenInfo(
                    baseUri,
                    citizenNumber,
                    apiKey,
                    transactionId,
                    hash
                );

                return info != null && !string.IsNullOrEmpty(info.citizenNumber);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }
        // Lưu trữ thông tin CCCD 
        public bool StoreCitizenInfo(
    string baseUri,
    TwoIDApiRequestInput citizen,
    object fingerprint,
    object faceId,
    object handSignature,
    string apiKey,
    string transactionId,
    string hash)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info(
    $"StoreCitizenInfo Input: citizenNumber={citizen.citizenNumber}, " +
    $"fingerprint={Newtonsoft.Json.JsonConvert.SerializeObject(fingerprint)}, " +
    $"faceId={Newtonsoft.Json.JsonConvert.SerializeObject(faceId)}, " +
    $"handSignature={Newtonsoft.Json.JsonConvert.SerializeObject(handSignature)}, " +
    $"apiKey={apiKey}, transactionId={transactionId}, hash={hash}");
                var uploadResult =
                    StorageApiClient.CreateMultipartRequest<TwoIDUploadResponse>(
                        baseUri,
                        "/api/v1/files/uploads",
                        citizen.citizenNumber,
                        fingerprint,
                        faceId,
                        handSignature,
                        apiKey,
                        transactionId,
                        hash
                    );

                if (uploadResult == null || !uploadResult.status)
                    throw new Exception("UploadFiles failed");

                citizen.fingerprint = uploadResult.GetUrl("FINGER_PRINT");
                citizen.faceId = uploadResult.GetUrl("FACE_ID");
                citizen.handSignature = uploadResult.GetUrl("HAND_SIGNATURE");

                StorageApiClient.CreateRequest<object>(
                    baseUri,
                    "/api/v1/citizens",
                    citizen,
                    "application/json"
                );

                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }


        public CitizenInfoWithImages GetCitizenInfoWithImages(string citizenNumber, string transactionId, string hash)
        {
            try
            {
                var info = GetCitizenInfo(baseUri, citizenNumber, apiKey, transactionId, hash);

                if (info != null)
                {
                    return new CitizenInfoWithImages
                    {
                        CitizenInfo = info,

                        FingerprintPaths = string.IsNullOrEmpty(info.fingerprint)
                            ? new List<string>()
                            : new List<string> { info.fingerprint },

                        FacePaths = string.IsNullOrEmpty(info.faceId)
                            ? new List<string>()
                            : new List<string> { info.faceId },

                        HandSignaturePaths = string.IsNullOrEmpty(info.handSignature)
                            ? new List<string>()
                            : new List<string> { info.handSignature }
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
        public byte[] DownloadImageFromPath(
    string imagePath,
    string transactionId,
    string hash)
        {
            try
            {
                if (string.IsNullOrEmpty(imagePath))
                    return null;

                var input = new TwoIDApiRequestInput
                {
                    fileName = imagePath,
                    apiKey = this.apiKey,   // ✅ FIX
                    transactionId = transactionId,
                    hash = hash
                };

                var result = StorageApiClient.CreateRequest<TwoIDDownloadResponse>(
                    baseUri,
                    "/api/v1/files/download",
                    input,
                    "application/x-www-form-urlencoded"
                );

                if (result == null || !result.status || string.IsNullOrEmpty(result.data))
                    return null;

                return Convert.FromBase64String(result.data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
        public List<byte[]> GetAllFingerprintImages(
    string citizenNumber,
    string transactionId,
    string hash)
        {
            try
            {
                var citizenWithImages =
                    GetCitizenInfoWithImages(citizenNumber, transactionId, hash);

                var images = new List<byte[]>();

                if (citizenWithImages?.FingerprintPaths != null)
                {
                    foreach (var path in citizenWithImages.FingerprintPaths)
                    {
                        var imageData =
                            DownloadImageFromPath(path, transactionId, hash);

                        if (imageData != null)
                        {
                            images.Add(imageData);
                        }
                    }
                }

                return images;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return new List<byte[]>();
            }
        }


        // Lấy tất cả ảnh khuôn mặt
        public List<byte[]> GetAllFaceImages(
            string citizenNumber,
            string apiKey,
            string transactionId,
            string hash)
        {
            try
            {
                var citizenWithImages = GetCitizenInfoWithImages(citizenNumber, transactionId, hash);
                var images = new List<byte[]>();

                if (citizenWithImages?.FacePaths != null)
                {
                    foreach (var path in citizenWithImages.FacePaths)
                    {
                        var imageData = DownloadImageFromPath(path, transactionId, hash);
                        if (imageData != null)
                        {
                            images.Add(imageData);
                        }
                    }
                }

                return images;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return new List<byte[]>();
            }
        }
        public class CitizenInfoWithImages
        {
            public TwoIDApiRequestInput CitizenInfo { get; set; }
            public List<string> FingerprintPaths { get; set; }
            public List<string> FacePaths { get; set; }
            public List<string> HandSignaturePaths { get; set; }

            public CitizenInfoWithImages()
            {
                FingerprintPaths = new List<string>();
                FacePaths = new List<string>();
                HandSignaturePaths = new List<string>();
            }
        }






    }
}
