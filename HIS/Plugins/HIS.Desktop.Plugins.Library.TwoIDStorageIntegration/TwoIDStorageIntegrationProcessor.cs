using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Inventec.Common.Logging;

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

            var result = StorageApiClient.CreateRequest<TwoIDApiResult>(baseUri, "/api/v1/citizens/info", input, "application/x-www-form-urlencoded");
            
            if (result != null && result.status && result.data != null)
            {
                // Map TwoIDCitizenData sang TwoIDApiRequestInput
                return new TwoIDApiRequestInput
                {
                    citizenNumber = result.data.citizenNumber,
                    fullName = result.data.fullName,
                    dateOfBirth = result.data.dateOfBirth,
                    gender = result.data.gender,
                    residencePlace = result.data.residencePlace,
                    issueDate = result.data.issueDate,
                    expiredDate = result.data.expiredDate,
                    status = "ACTIVE",
                    
                    // Map path ảnh
                    fingerprint = result.data.fingerPrintImage,
                    faceId = result.data.faceImage,
                    handSignature = result.data.handSignatureImage
                };
            }
            
            return null;
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
        /// <summary>
        /// Lưu trữ thông tin CCCD
        /// - Nếu đã có thông tin: Upload file mới → Lấy path → Đồng bộ dữ liệu với path mới
        /// - Nếu chưa có: Chỉ upload file
        /// </summary>
        public bool StoreCitizenInfo(
            string baseUri,
            TwoIdRequestCitizens citizen,
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

                // Bước 1: Upload file để tạo path (multipart/form-data với file ảnh)
                Inventec.Common.Logging.LogSystem.Info("StoreCitizenInfo: Bắt đầu upload file");
                var uploadResult = StorageApiClient.CreateMultipartRequest<TwoIDUploadResponse>(
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
                {
                    Inventec.Common.Logging.LogSystem.Error("StoreCitizenInfo: Upload file thất bại");
                    throw new Exception("UploadFiles failed");
                }

                Inventec.Common.Logging.LogSystem.Info("StoreCitizenInfo: Upload file thành công");
                Inventec.Common.Logging.LogSystem.Debug("uploadResult: " + Newtonsoft.Json.JsonConvert.SerializeObject(uploadResult));

                // Lấy các path từ kết quả upload
                string fingerprintPath = uploadResult.GetUrl("FINGER_PRINT");
                string faceIdPath = uploadResult.GetUrl("FACE_ID");
                string handSignaturePath = uploadResult.GetUrl("HAND_SIGNATURE");

                Inventec.Common.Logging.LogSystem.Info($"StoreCitizenInfo: Paths - fingerprint={fingerprintPath}, faceId={faceIdPath}, handSignature={handSignaturePath}");

                // Bước 2: Kiểm tra xem đã có thông tin CCCD hay chưa
                bool isExist = IsCitizenInfoExists(baseUri, citizen.citizenNumber, apiKey, transactionId, hash);
                Inventec.Common.Logging.LogSystem.Info($"StoreCitizenInfo: CCCD {citizen.citizenNumber} đã tồn tại = {isExist}");

                // Đã có thông tin → Cập nhật với path mới
                Inventec.Common.Logging.LogSystem.Info("StoreCitizenInfo: Cập nhật thông tin CCCD");

                // Gán path mới vào citizen
                citizen.fingerPrintImage = fingerprintPath;
                citizen.faceImage = faceIdPath;
                citizen.handSignatureImage = handSignaturePath;
                string header = "?apiKey=" + apiKey + "&transactionId=" + transactionId + "&hash=" + hash;
                // Gọi API đồng bộ dữ liệu cá nhân (application/json)
                var syncResult = StorageApiClient.CreateRequest<object>(
                    baseUri,
                    "/api/v1/citizens"+header,
                    citizen,
                    "application/json"
                );
                if( syncResult != null)
                {

                    Inventec.Common.Logging.LogSystem.Info("StoreCitizenInfo: Đồng bộ dữ liệu thành công");
                    return true;
                }
                else
                {
                    return false;
                }

                //if (isExist)
                //{
                    
                //}
                //else
                //{
                //    // Chưa có thông tin → Chỉ cần upload file (đã làm ở bước 1)
                //    Inventec.Common.Logging.LogSystem.Info("StoreCitizenInfo: Tạo mới thông tin CCCD - Chỉ upload file");
                //}

                //return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("StoreCitizenInfo: Lỗi - " + ex.ToString());
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
    string hash, 
    string type)
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
                    hash = hash,
                    type = type
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
                            DownloadImageFromPath(path, transactionId, hash, "base64");

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
                        var imageData = DownloadImageFromPath(path, transactionId, hash, "base64");
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
