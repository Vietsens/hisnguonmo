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
        public TwoIDStorageIntegrationProcessor()
        {
            config = ConfigCFG.GetStorageConfig();
        }

        // Upload danh sách file
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
        // Lấy thông tin CCCD
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
        //Download dữ liệu file
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
        // Đồng bộ dữ liệu cá nhân
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

        //check CCCD
        public bool IsCitizenInfoExists(string citizenNumber, string apiKey, string transactionId, string hash)
        {
            try
            {
                var info = GetCitizenInfo(citizenNumber, apiKey, transactionId, hash);
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
            string citizenNumber,
            List<string> fingerprint,
            List<string> faceId,
            List<string> handSignature,
            string apiKey,
            string transactionId,
            string hash)
        {
            try
            {
                if (IsCitizenInfoExists(citizenNumber, apiKey, transactionId, hash))
                {
                  
                    var uploadResult = UploadFiles(citizenNumber, apiKey, transactionId, hash);

                  
                    var syncResult = SyncPersonalData(
                        citizenNumber,
                        fingerprint,
                        faceId,
                        handSignature,
                        apiKey,
                        transactionId,
                        hash,
                        "application/json"
                    );
                    
                    return syncResult != null;
                }
                else
                {
                    
                    var uploadResult = UploadFiles(citizenNumber, apiKey, transactionId, hash);
                    return uploadResult != null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }
        
        public bool UpdateCitizenInfo(
            string citizenNumber,
            List<string> fingerprint,
            List<string> faceId,
            List<string> handSignature,
            string apiKey,
            string transactionId,
            string hash)
        {
            try
            {
                
                var uploadResult = UploadFiles(citizenNumber, apiKey, transactionId, hash);

               
                var syncResult = SyncPersonalData(
                    citizenNumber,
                    fingerprint,
                    faceId,
                    handSignature,
                    apiKey,
                    transactionId,
                    hash,
                    "application/json"
                );

                return syncResult != null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }
        public bool CreateCitizenInfo(
            string citizenNumber,
            string apiKey,
            string transactionId,
            string hash)
        {
            try
            {
                var uploadResult = UploadFiles(citizenNumber, apiKey, transactionId, hash);
                return uploadResult != null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }
        public CitizenInfoWithImages GetCitizenInfoWithImages(
            string citizenNumber,
            string apiKey,
            string transactionId,
            string hash)
        {
            try
            {
                var info = GetCitizenInfo(citizenNumber, apiKey, transactionId, hash);

                if (info != null)
                {
                    return new CitizenInfoWithImages
                    {
                        CitizenInfo = info,
                        FingerprintPaths = info.fingerprint ?? new List<string>(),
                        FacePaths = info.faceId ?? new List<string>(),
                        HandSignaturePaths = info.handSignature ?? new List<string>()
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
        string citizenNumber,
        string imagePath,
        string apiKey,
        string transactionId,
        string hash)
        {
            try
            {
                if (string.IsNullOrEmpty(imagePath))
                    return null;

                var downloadResult = DownloadFile(citizenNumber, apiKey, transactionId, hash);

                if (downloadResult == null)
                    return null;

              
                return new byte[0]; 
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null; 
            }
        }
        public List<byte[]> GetAllFingerprintImages(
            string citizenNumber,
            string apiKey,
            string transactionId,
            string hash)
        {
            try
            {
                var citizenWithImages = GetCitizenInfoWithImages(citizenNumber, apiKey, transactionId, hash);
                var images = new List<byte[]>();

                if (citizenWithImages?.FingerprintPaths != null)
                {
                    foreach (var path in citizenWithImages.FingerprintPaths)
                    {
                        var imageData = DownloadImageFromPath(citizenNumber, path, apiKey, transactionId, hash);
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
                var citizenWithImages = GetCitizenInfoWithImages(citizenNumber, apiKey, transactionId, hash);
                var images = new List<byte[]>();

                if (citizenWithImages?.FacePaths != null)
                {
                    foreach (var path in citizenWithImages.FacePaths)
                    {
                        var imageData = DownloadImageFromPath(citizenNumber, path, apiKey, transactionId, hash);
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
