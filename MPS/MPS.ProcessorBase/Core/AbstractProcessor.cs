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
using Inventec.Common.QRCoder;
using Inventec.Common.SignLibrary;
using Inventec.Common.SignLibrary.ADO;
using Inventec.Common.SignLibrary.DTO;
using Inventec.Core;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MPS.ProcessorBase.Core
{
    public abstract class AbstractProcessor : ProcessorBase
    {
        protected Inventec.Common.FlexCellExport.Store store;
        protected Inventec.Common.TemplaterExport.Store templaterExportStore;
        protected Inventec.Common.XtraReportExport.Store xtraReportStore;
        protected Dictionary<string, Inventec.Common.BarcodeLib.Barcode> dicImage = null;

        private IntPtr _bufferPtr;
        public int BUFFER_SIZE = 1024 * 1024; // 1 MB
        private bool _disposed = false;

        abstract public bool ProcessData();

        public AbstractProcessor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            dicImage = new Dictionary<string, Inventec.Common.BarcodeLib.Barcode>();

            this.templateType = Utils.GetTemplateTypeFromFile(this.fileName);
            this.InitType();
            try
            {
                //_bufferPtr = Marshal.AllocHGlobal(BUFFER_SIZE);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public AbstractProcessor(CommonParam param, PrintData printData, MPS.ProcessorBase.PrintConfig.TemplateType templateType)
            : base(param, printData)
        {
            dicImage = new Dictionary<string, Inventec.Common.BarcodeLib.Barcode>();
            this.templateType = templateType;
            this.InitType();
            try
            {
                //_bufferPtr = Marshal.AllocHGlobal(BUFFER_SIZE);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public bool Run()
        {
            bool result = false;
            bool valid = true;
            try
            {
                Inventec.Common.Logging.LogSystem.Info(this.printTypeCode + ": AbstractProcessor.Run.1.1");
                var watch = System.Diagnostics.Stopwatch.StartNew();
                System.Diagnostics.Process proc = System.Diagnostics.Process.GetCurrentProcess();
                //Inventec.Common.Logging.LogSystem.Info("CalculateMemoryRam____Dung luong PM( GC.GetTotalMemory):" + ((decimal)startBytes / (1024 * 1024)) + "MB");
                Inventec.Common.Logging.LogAction.Debug(this.printTypeCode + ": Dung lượng RAM khi bắt đầu xử lý là: " + ((decimal)proc.PrivateMemorySize64 / (1024 * 1024)) + "MB");
                Inventec.Common.Logging.LogSystem.Info(this.printTypeCode + ": AbstractProcessor.1.2");
                //Gán các key dùng chung: CURRENT_TIME_STR, CURRENT_DATE_STR,...
                SetCommonSingleKey();

                //Khởi tạo các function dùng chung (các hàm nhúng trong file template): 
                SetCommonFunctionRun();


                //Hàm xử lý dữ liệu trước khi in, gán dữ liệu vào các đối tượng phục vụ cho việc đổ dữ liệu sử dụng thư viện in
                //lấy số bên trong mps vì dữ liệu gán trong hàm này UniqueCode
                valid = valid && ProcessData();
                UpdateDicSingleKeyFromStore();
                ProcessNumCopyPrint();
                valid = valid && ProcessDisablePrint();
                if (valid)
                {
                    var emrInputADO = ((Inventec.Common.SignLibrary.ADO.InputADO)this.printDataBase.EmrInputADO);
                    if (emrInputADO != null)
                    {
                        emrInputADO.BusinessCode = this.currentBussinessCode;
                        emrInputADO.PrintTypeBusinessCodes = this.printTypeBusinessCodes;
                        emrInputADO.IsAutoChooseBusiness = printType.IS_AUTO_CHOOSE_BUSINESS == 1;
                        emrInputADO.HisCode = ProcessUniqueCodeData();

                        //bên ngoài truyền vào thì dùng của bên ngoài. không có sẽ lấy theo mps
                        if (String.IsNullOrWhiteSpace(emrInputADO.DocumentGroupCode))
                        {
                            emrInputADO.DocumentGroupCode = this.printType.EMR_DOCUMENT_GROUP_CODE;
                        }

                        if (!ProcessColumnMaping(emrInputADO))
                        {
                            return false;
                        }

                        emrInputADO.SignerConfigs = ProcessSignerConfigs();
                    }
                    this.printDataBase.EmrInputADO = emrInputADO;
                    //proc = System.Diagnostics.Process.GetCurrentProcess();
                    Inventec.Common.Logging.LogAction.Debug(this.printTypeCode + ": Dung lượng RAM khi đã xử lý xong dữ liệu MPS trước khi thực hiện xuất file là: " + ((decimal)proc.PrivateMemorySize64 / (1024 * 1024)) + "MB");

                    if (previewType == PrintConfig.PreviewType.SaveFile)
                    {
                        if (!String.IsNullOrEmpty(saveFilePath))
                        {
                            if (File.Exists(saveFilePath))
                            {
                                try
                                {
                                    File.Delete(saveFilePath);
                                }
                                catch (Exception ex)
                                {
                                    Inventec.Common.Logging.LogSystem.Debug(ex);
                                }

                            }
                            result = OutFileRun();
                            printDataBase.saveFilePath = saveFilePath;
                        }
                        else if (saveMemoryStream != null)
                        {
                            OutFileStreamRunForSave();
                            result = (saveMemoryStream != null && saveMemoryStream.Length > 0);
                            printDataBase.saveMemoryStream = saveMemoryStream;
                        }
                        else
                        {
                            //Lỗi truyền vào thiếu dữ liệu bắt buộc
                            Inventec.Common.Logging.LogSystem.Warn("Truyen vao thieu du lieu bat buoc. saveFilePath = " + saveFilePath + ", saveMemoryStream.Length = " + (saveMemoryStream != null ? saveMemoryStream.Length : 0));
                            result = false;
                        }
                        //proc = System.Diagnostics.Process.GetCurrentProcess();
                        Inventec.Common.Logging.LogAction.Debug(this.printTypeCode + ": Dung lượng RAM sau khi đã xử lý xuất file kết quả MPS (PreviewType = SaveFile) là: " + ((decimal)proc.PrivateMemorySize64 / (1024 * 1024)) + "MB");
                    }
                    else if (previewType == PrintConfig.PreviewType.EmrShow)
                    {
                        OutFileStreamRunToPdf();
                        Inventec.Common.Logging.LogAction.Debug(this.printTypeCode + ": Dung lượng RAM sau khi đã xử lý xuất file kết quả MPS (PreviewType = EmrShow) là: " + ((decimal)proc.PrivateMemorySize64 / (1024 * 1024)) + "MB");
                        if (((this.saveMemoryStream != null && this.saveMemoryStream.Length > 0) || !String.IsNullOrEmpty(this.saveFilePath)) && this.printDataBase.EmrInputADO != null)
                        {
                            result = EmrShowClick();
                        }
                        Inventec.Common.Logging.LogAction.Debug(this.printTypeCode + ": Dung lượng RAM sau khi đã xử lý hiển thị popup xem ký và đóng popup (PreviewType = EmrShow) là: " + ((decimal)proc.PrivateMemorySize64 / (1024 * 1024)) + "MB");
                    }
                    else if (previewType == PrintConfig.PreviewType.EmrSignNow)
                    {
                        Inventec.Common.Logging.LogSystem.Info(this.printTypeCode + ": AbstractProcessor.Run.2");
                        OutFileStreamRunToPdf();
                        Inventec.Common.Logging.LogSystem.Info(this.printTypeCode + ": AbstractProcessor.Run.3");
                        Inventec.Common.Logging.LogAction.Debug(this.printTypeCode + ": Dung lượng RAM sau khi đã xử lý xuất file kết quả MPS (PreviewType = EmrSignNow) là: " + ((decimal)proc.PrivateMemorySize64 / (1024 * 1024)) + "MB");
                        if (((this.saveMemoryStream != null && this.saveMemoryStream.Length > 0) || !String.IsNullOrEmpty(this.saveFilePath)) && this.printDataBase.EmrInputADO != null)
                        {
                            result = EmrSignNowClick();
                        }
                        this.Dispose();
                        Inventec.Common.Logging.LogSystem.Info(this.printTypeCode + ": AbstractProcessor.Run.EmrSignNow.4");
                        Inventec.Common.Logging.LogAction.Debug(this.printTypeCode + ": Dung lượng RAM sau khi đã xử lý hiển thị ký ngay (PreviewType = EmrSignNow) là: " + ((decimal)proc.PrivateMemorySize64 / (1024 * 1024)) + "MB");
                    }
                    else if (previewType == PrintConfig.PreviewType.EmrSignAndPrintNow)
                    {
                        Inventec.Common.Logging.LogSystem.Info(this.printTypeCode + ": AbstractProcessor.Run.2");
                        OutFileStreamRunToPdf();
                        Inventec.Common.Logging.LogSystem.Info(this.printTypeCode + ": AbstractProcessor.Run.3");
                        Inventec.Common.Logging.LogAction.Debug(this.printTypeCode + ": Dung lượng RAM sau khi đã xử lý xuất file kết quả MPS (PreviewType = EmrSignAndPrintNow) là: " + ((decimal)proc.PrivateMemorySize64 / (1024 * 1024)) + "MB");
                        if (((this.saveMemoryStream != null && this.saveMemoryStream.Length > 0) || !String.IsNullOrEmpty(this.saveFilePath)) && this.printDataBase.EmrInputADO != null)
                        {
                            result = EmrSignAndPrintNowClick();
                        }
                        //System.Diagnostics.Process proc1 = System.Diagnostics.Process.GetCurrentProcess();
                        Inventec.Common.Logging.LogAction.Debug(this.printTypeCode + ": Dung lượng RAM sau khi đã xử lý hiển thị ký và in ngay (PreviewType = EmrSignAndPrintNow) là: " + ((decimal)proc.PrivateMemorySize64 / (1024 * 1024)) + "MB");
                        this.Dispose();
                        Inventec.Common.Logging.LogSystem.Info(this.printTypeCode + ": AbstractProcessor.Run.EmrSignAndPrintNow.4");
                    }
                    else if (previewType == PrintConfig.PreviewType.EmrSignAndPrintPreview)
                    {
                        Inventec.Common.Logging.LogSystem.Info(this.printTypeCode + ": AbstractProcessor.Run.2");
                        OutFileStreamRunToPdf();
                        Inventec.Common.Logging.LogAction.Debug(this.printTypeCode + ": Dung lượng RAM sau khi đã xử lý xuất file kết quả MPS (PreviewType = EmrSignAndPrintPreview) là: " + ((decimal)proc.PrivateMemorySize64 / (1024 * 1024)) + "MB");
                        Inventec.Common.Logging.LogSystem.Info(this.printTypeCode + ": AbstractProcessor.Run.3");
                        if (((this.saveMemoryStream != null && this.saveMemoryStream.Length > 0) || !String.IsNullOrEmpty(this.saveFilePath)) && this.printDataBase.EmrInputADO != null)
                        {
                            result = EmrSignAndPrintPreviewClick();
                        }
                        System.Diagnostics.Process proc1 = System.Diagnostics.Process.GetCurrentProcess();
                        Inventec.Common.Logging.LogAction.Debug(this.printTypeCode + ": Dung lượng RAM sau khi đã xử lý hiển thị ký và xem trước in (PreviewType = EmrSignAndPrintPreview) là: " + ((decimal)proc.PrivateMemorySize64 / (1024 * 1024)) + "MB");
                        Inventec.Common.Logging.LogSystem.Info(this.printTypeCode + ": AbstractProcessor.Run.EmrSignAndPrintPreview.4");
                    }
                    else if (previewType == PrintConfig.PreviewType.EmrCreateDocument)
                    {
                        Inventec.Common.Logging.LogSystem.Info(this.printTypeCode + ": AbstractProcessor.Run.2");
                        OutFileStreamRunToPdf();
                        Inventec.Common.Logging.LogSystem.Info(this.printTypeCode + ": AbstractProcessor.Run.3");
                        Inventec.Common.Logging.LogAction.Debug(this.printTypeCode + ": Dung lượng RAM sau khi đã xử lý xuất file kết quả MPS (PreviewType = EmrCreateDocument) là: " + ((decimal)proc.PrivateMemorySize64 / (1024 * 1024)) + "MB");
                        if (((this.saveMemoryStream != null && this.saveMemoryStream.Length > 0) || !String.IsNullOrEmpty(this.saveFilePath)) && this.printDataBase.EmrInputADO != null)
                        {
                            result = EmrCreateDocumentClick();
                        }
                        this.Dispose();
                        System.Diagnostics.Process proc1 = System.Diagnostics.Process.GetCurrentProcess();
                        Inventec.Common.Logging.LogAction.Debug(this.printTypeCode + ": Dung lượng RAM sau khi đã xử lý tạo văn bản (PreviewType = EmrCreateDocument) là: " + ((decimal)proc.PrivateMemorySize64 / (1024 * 1024)) + "MB");
                        Inventec.Common.Logging.LogSystem.Info(this.printTypeCode + ": AbstractProcessor.Run.EmrCreateDocument.4");
                    }

                    else
                    {
                        if (OutFileStreamRun())
                        {
                            this.printLog = EventLogPrintWithAfterPrint;
                            this.getNumOrderPrint = GetNumOrderPrintByUniqueCodeData;
                            if (!String.IsNullOrWhiteSpace(ProcessUniqueCodeData()))
                            {
                                this.ShowLog = ShowPrintLog;
                            }

                            Inventec.Common.Logging.LogAction.Debug(this.printTypeCode + ": Dung lượng RAM sau khi đã xử lý xuất file kết quả MPS (PreviewType = " + previewType.ToString() + ") là: " + ((decimal)proc.PrivateMemorySize64 / (1024 * 1024)) + "MB");
                            Inventec.Common.Logging.LogSystem.Info(this.printTypeCode + ": AbstractProcessor.Run.5");

                            //Hiển thị form xem in hoặc in luôn tùy theo cấu hình người dùng truyền vào
                            result = PrintPreviewRun();

                            System.Diagnostics.Process proc2 = System.Diagnostics.Process.GetCurrentProcess();
                            //Inventec.Common.Logging.LogSystem.Info("CalculateMemoryRam____Dung luong PM( GC.GetTotalMemory):" + ((decimal)startBytes / (1024 * 1024)) + "MB");
                            Inventec.Common.Logging.LogAction.Debug(this.printTypeCode + ": Dung lượng RAM sau khi đã hiển thị popup xem in và đóng, (PreviewType = " + previewType.ToString() + ") là: " + ((decimal)proc.PrivateMemorySize64 / (1024 * 1024)) + "MB");
                            Inventec.Common.Logging.LogSystem.Info(this.printTypeCode + ": AbstractProcessor.Run.6");
                        }
                    }
                }
                // Thời gian kết thúc
                watch.Stop();
                Inventec.Common.Logging.LogAction.Info(String.Format("{0}____{1}____{2}____{3}____{4}____{5}____{6}____{7}", PrintConfig.AppCode, PrintConfig.VersionApp, (double)((double)watch.ElapsedMilliseconds / (double)1000), this.printTypeCode, (previewType.ToString()), Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), PrintConfig.IpLocal, PrintConfig.CustomerCode));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);

                result = false;
            }
            return result;
        }

        private List<SignerConfigDTO> ProcessSignerConfigs()
        {
            List<SignerConfigDTO> result = null;
            try
            {
                if (this.printType != null && !String.IsNullOrWhiteSpace(this.printType.GEN_SIGNER_BY_KEY_CFG) && singleValueDictionary != null && singleValueDictionary.Count > 0)
                {
                    List<SignerByConfigADO> numCopyList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SignerByConfigADO>>(this.printType.GEN_SIGNER_BY_KEY_CFG);
                    if (numCopyList != null && numCopyList.Count > 0)
                    {
                        numCopyList = numCopyList.OrderBy(x => x.Num).ToList();
                        result = new List<SignerConfigDTO>();
                        long numOrder = 1;
                        foreach (var item in numCopyList)
                        {
                            string loginname = "";
                            if (singleValueDictionary.ContainsKey(item.Key) && singleValueDictionary[item.Key] != null)
                            {
                                loginname = singleValueDictionary[item.Key].ToString();
                            }

                            if (!String.IsNullOrEmpty(loginname) && !result.Exists(o => o.Loginname == loginname))
                            {
                                SignerConfigDTO dto = new SignerConfigDTO();
                                dto.Loginname = loginname;
                                dto.NumOrder = numOrder;
                                result.Add(dto);
                                numOrder++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void UpdateDicSingleKeyFromStore()
        {
            switch (this.templateType)
            {
                case PrintConfig.TemplateType.Excel:
                    singleValueDictionary = store.DictionaryTemplateKey;
                    break;
                case PrintConfig.TemplateType.Word:
                    singleValueDictionary = templaterExportStore.DictionaryTemplateKey;
                    break;
                case PrintConfig.TemplateType.XtraReport:
                    singleValueDictionary = xtraReportStore.DictionaryTemplateKey;
                    break;
            }
        }

        protected void SetTreatmentQrCodeBase()
        {
            try
            {
                if (singleValueDictionary != null && singleValueDictionary.Count > 0 && (singleValueDictionary.ContainsKey("TREATMENT_CODE")
                    || singleValueDictionary.ContainsKey("TDL_TREATMENT_CODE")))
                {
                    if (singleValueDictionary.ContainsKey("TREATMENT_CODE"))
                    {
                        SetQrCodeByKeyBase("TREATMENT_CODE", CommonKey._QRCODE_TREATMENT_CODE_COMMON_KEY);
                    }
                    else if (singleValueDictionary.ContainsKey("TDL_TREATMENT_CODE"))
                    {
                        SetQrCodeByKeyBase("TDL_TREATMENT_CODE", CommonKey._QRCODE_TREATMENT_CODE_COMMON_KEY);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        protected void SetPatientQrCodeBase()
        {
            try
            {
                if (singleValueDictionary != null && singleValueDictionary.Count > 0 && (singleValueDictionary.ContainsKey("PATIENT_CODE")
                    || singleValueDictionary.ContainsKey("TDL_PATIENT_CODE")))
                {
                    if (singleValueDictionary.ContainsKey("PATIENT_CODE"))
                    {
                        SetQrCodeByKeyBase("PATIENT_CODE", CommonKey._QRCODE_PATIENT_CODE_COMMON_KEY);
                    }
                    else if (singleValueDictionary.ContainsKey("TDL_PATIENT_CODE"))
                    {
                        SetQrCodeByKeyBase("TDL_PATIENT_CODE", CommonKey._QRCODE_PATIENT_CODE_COMMON_KEY);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        protected void SetQrCodeByKeyBase(string keyInDic, string keyQrcode)
        {
            try
            {
                string value = "";
                if (singleValueDictionary != null && singleValueDictionary.Count > 0 && singleValueDictionary.ContainsKey(keyInDic))
                {
                    object vl = null;
                    singleValueDictionary.TryGetValue(keyInDic, out vl);
                    if (vl != null)
                    {
                        value = vl.ToString();
                    }
                }

                if (!String.IsNullOrWhiteSpace(value))
                {
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(value, QRCodeGenerator.ECCLevel.Q);
                    BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
                    byte[] qrCodeImage = qrCode.GetGraphic(20);
                    SetSingleKey(keyQrcode, qrCodeImage);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Ánh xạ dữ liệu truyền vào hệ thống EMR:
        ///Nếu biểu in có giá trị cấu hình sinh chữ ký dựa vào tài khoản (GEN_SIGNATURE_BY_KEY_CFG trong SAR_PRINT_TYPE có giá trị) thì xử lý nghiệp vụ sau:
        ///- Căn cứ vào dữ liệu GEN_SIGNATURE_BY_KEY_CFG để lấy ra thông tin cấu hình. Cụ thể, giá trị của trường này sẽ có dạng chuỗi json của 1 list object có 2 trường dạng string là: LoginnameKey,SignatureKey
        ///vd:
        ///[{"LoginnameKey":"REQ_LOGINNAME","SignatureKey":"REQ_LOGINNAME_SIGNATURE"},{"LoginnameKey":"EXECUTE_LOGINNAME","SignatureKey":"EXECUTE_LOGINNAME_SIGNATURE"}]
        ///- Nếu chuỗi json ko bị lỗi (parse được theo cấu trúc trên), thì thực hiện xử lý:
        ///+ Với mỗi key tài khoản (LoginnameKey) được khai báo, thì thực hiện truy vấn sang dữ liệu tài khoản khai báo trên EMR (trong bảng EMR_SIGNER) để lấy ra dữ liệu ảnh chữ ký tương ứng
        ///+ Với mỗi ảnh chữ ký tương ứng, tự động sinh key ảnh chữ ký theo tên được khai báo ở trường SignatureKey
        ///+ Nếu các key được khai báo không có trong biểu mẫu thì không xử lý
        ///+ Nếu LoginnameKey hoặc SignatureKey không có giá trị thì không xử lý.
        /// </summary>
        protected void SetSignatureKeyImageByCFG()
        {
            try
            {
                if (singleValueDictionary != null && singleValueDictionary.Count > 0)
                {
                    if (PrintConfig.EmrSigners != null && PrintConfig.EmrSigners.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(this.printType.GEN_SIGNATURE_BY_KEY_CFG))
                        {
                            List<GenSignatureImageKeyADO> genSignatureByKetCFGs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GenSignatureImageKeyADO>>(this.printType.GEN_SIGNATURE_BY_KEY_CFG);
                            if (genSignatureByKetCFGs != null && genSignatureByKetCFGs.Count > 0)
                            {
                                foreach (var genSignature in genSignatureByKetCFGs)
                                {
                                    string loginNameKey = genSignature.LoginnameKey.Trim();
                                    if (singleValueDictionary.ContainsKey(loginNameKey) && singleValueDictionary[loginNameKey] != null && !String.IsNullOrEmpty(genSignature.LoginnameKey) && !String.IsNullOrEmpty(genSignature.SignatureKey))
                                    {
                                        string loginname = singleValueDictionary[loginNameKey].ToString();

                                        var signer = PrintConfig.EmrSigners.FirstOrDefault(o => o.LOGINNAME == loginname);

                                        if (signer != null && signer.SIGN_IMAGE != null)
                                        {
                                            if (!singleValueDictionary.ContainsKey(genSignature.SignatureKey))
                                            {
                                                singleValueDictionary.Add(genSignature.SignatureKey, signer.SIGN_IMAGE);
                                                Inventec.Common.Logging.LogSystem.Info("Bieu in co cau hinh GEN_SIGNATURE_BY_KEY_CFG=" + this.printType.GEN_SIGNATURE_BY_KEY_CFG + ", tim thay emrsigner hop le tuong ung voi (LoginnameKey " + genSignature.LoginnameKey + ", loginname value =" + loginname + ") &  chua co key " + genSignature.SignatureKey + " trong singleValueDictionary ==> insert key vao dic = " + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => genSignature), genSignature));
                                            }
                                            else
                                            {
                                                singleValueDictionary[genSignature.SignatureKey] = signer.SIGN_IMAGE;
                                                Inventec.Common.Logging.LogSystem.Info("Bieu in co cau hinh GEN_SIGNATURE_BY_KEY_CFG=" + this.printType.GEN_SIGNATURE_BY_KEY_CFG + ", tim thay emrsigner hop le tuong ung voi (LoginnameKey " + genSignature.LoginnameKey + ", loginname value =" + loginname + ") & da co key " + genSignature.SignatureKey + " trong singleValueDictionary ==> update key vao dic = " + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => genSignature), genSignature));
                                            }
                                        }
                                        else
                                        {
                                            Inventec.Common.Logging.LogSystem.Warn("Bieu in co cau hinh GEN_SIGNATURE_BY_KEY_CFG=" + this.printType.GEN_SIGNATURE_BY_KEY_CFG + ", tuy nhien khong tim thay emrsigner hoac tim duoc signer nhung khong co anh chu ky tuong ung voi (LoginnameKey " + genSignature.LoginnameKey + ", loginname value =" + loginname + ")____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => genSignature), genSignature)
                                                + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => signer), signer));
                                        }
                                    }
                                    else
                                    {
                                        Inventec.Common.Logging.LogSystem.Warn("Bieu in co cau hinh GEN_SIGNATURE_BY_KEY_CFG=" + this.printType.GEN_SIGNATURE_BY_KEY_CFG + ", tuy nhien du lieu khong hop le____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => genSignature), genSignature)
                                                );
                                    }
                                }
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Bieu in co cau hinh GEN_SIGNATURE_BY_KEY_CFG=" + this.printType.GEN_SIGNATURE_BY_KEY_CFG + ", tuy nhien co loi khong DeserializeObject duoc tu chuoi json de xu ly");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// 1. Ánh xạ dữ liệu truyền vào hệ thống EMR:
        ///- Căn cứ vào trường dữ liệu EMR_COLUMN_MAPPING trong SAR_PRINT_TYPE để lấy dữ liệu trên phiếu in truyền vào trường tương ứng của EMR_DOCUMENT.
        ///- Dữ liệu EMR_COLUMN_MAPPING sẽ có dạng chuỗi json của 1 list object có 2 trường dạng string là: EmrColumn,Key
        ///vd:
        ///[{"EmrColumn":"DocumentTime","Key":"TRACKING_TIME"},{"EmrColumn":"XXX1","Key":"YYY1"}]
        ///Khi đó, sẽ lấy dữ liệu từ key TRACKING_TIME để truyền vào trường DocumentTime của EMR.
        ///
        ///Lưu ý: Nếu trường của EMR là dữ liệu ngày(kiểu DateTime hoặc long) thì dữ liệu key để lấy dữ liệu có thể lấy với các biến kiểu long, DateTime, string date(vd: 09/08/2020 18:15, 09/08/2020)
        /// </summary>
        /// <param name="emrInputADO"></param>
        private bool ProcessColumnMaping(Inventec.Common.SignLibrary.ADO.InputADO emrInputADO)
        {
            bool success = true;
            try
            {
                if (singleValueDictionary != null && singleValueDictionary.Count > 0)
                {
                    if (!String.IsNullOrEmpty(this.printType.EMR_COLUMN_MAPPING))
                    {
                        try
                        {
                            List<EmrColumnMappingADO> emrColumnMappingADOs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<EmrColumnMappingADO>>(this.printType.EMR_COLUMN_MAPPING);
                            if (emrColumnMappingADOs != null && emrColumnMappingADOs.Count > 0)
                            {
                                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => emrColumnMappingADOs), emrColumnMappingADOs));
                                System.Reflection.PropertyInfo[] piInputADOs = typeof(Inventec.Common.SignLibrary.ADO.InputADO).GetProperties();
                                foreach (var emrColumn in emrColumnMappingADOs)
                                {
                                    if (singleValueDictionary.ContainsKey(emrColumn.Key))
                                    {
                                        object value = singleValueDictionary[emrColumn.Key];
                                        string valueDataType = "";
                                        var pi = piInputADOs.FirstOrDefault(t => t.Name == emrColumn.EmrColumn);
                                        if (pi != null && value != null)
                                        {
                                            try
                                            {
                                                if (pi.PropertyType.Equals(typeof(short)) || pi.PropertyType.Equals(typeof(short?)))
                                                {
                                                    pi.SetValue(emrInputADO, (short)(value));
                                                    valueDataType = "short";
                                                }
                                                else if (pi.PropertyType.Equals(typeof(decimal)) || pi.PropertyType.Equals(typeof(decimal?)))
                                                {
                                                    pi.SetValue(emrInputADO, (decimal)(value));
                                                    valueDataType = "decimal";
                                                }
                                                else if (pi.PropertyType.Equals(typeof(long)) || pi.PropertyType.Equals(typeof(long?)))
                                                {
                                                    DateTime? dt = null;
                                                    if (value.ToString().Contains("/") || value.ToString().Contains("-"))
                                                    {
                                                        string strDate = "", hours = "";
                                                        if (value.ToString().Contains(":"))
                                                        {
                                                            var arrDate = value.ToString().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                                            if (arrDate != null && arrDate.Count() > 1)
                                                            {
                                                                foreach (var itemD in arrDate)
                                                                {
                                                                    if (itemD.Contains(":"))
                                                                    {
                                                                        hours = itemD.Trim().Replace("h", ":");
                                                                        if (hours.Length == 5)
                                                                        {
                                                                            hours += ":00";
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        var arrDate1 = itemD.Trim().Split(new string[] { "/", "-" }, StringSplitOptions.RemoveEmptyEntries);
                                                                        if (arrDate1 != null && arrDate1.Count() > 0)
                                                                        {
                                                                            strDate = String.Format("{0}-{1}-{2}", arrDate1[2], arrDate1[1], arrDate1[0]);
                                                                        }
                                                                    }
                                                                }
                                                                dt = DateTime.Parse(String.Format("{0} {1}", strDate, hours));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            var arrDate1 = value.ToString().Split(new string[] { "/", "-" }, StringSplitOptions.RemoveEmptyEntries);
                                                            if (arrDate1 != null && arrDate1.Count() > 0)
                                                            {
                                                                strDate = String.Format("{0}-{1}-{2}", arrDate1[2], arrDate1[1], arrDate1[0]);
                                                                hours = "00:00:00";
                                                                dt = DateTime.Parse(String.Format("{0} {1}", strDate, hours));
                                                            }
                                                        }
                                                        pi.SetValue(emrInputADO, Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dt));
                                                    }
                                                    else
                                                        pi.SetValue(emrInputADO, (long)(value));

                                                    valueDataType = "long";
                                                }
                                                else if (pi.PropertyType.Equals(typeof(int)) || pi.PropertyType.Equals(typeof(int?)))
                                                {
                                                    pi.SetValue(emrInputADO, (int)(value));
                                                    valueDataType = "int";
                                                }
                                                else if (pi.PropertyType.Equals(typeof(double)) || pi.PropertyType.Equals(typeof(double?)))
                                                {
                                                    pi.SetValue(emrInputADO, (double)(value));
                                                    valueDataType = "double";
                                                }
                                                else if (pi.PropertyType.Equals(typeof(float)) || pi.PropertyType.Equals(typeof(float?)))
                                                {
                                                    pi.SetValue(emrInputADO, (float)(value));
                                                    valueDataType = "float";
                                                }
                                                else if (pi.PropertyType.Equals(typeof(bool)) || pi.PropertyType.Equals(typeof(bool?)))
                                                {
                                                    pi.SetValue(emrInputADO, (bool)(value));
                                                    valueDataType = "bool";
                                                }
                                                else if (pi.PropertyType.Equals(typeof(DateTime)) || pi.PropertyType.Equals(typeof(DateTime?)))
                                                {
                                                    DateTime? dt = null;
                                                    if (value.ToString().Contains("/") || value.ToString().Contains("-"))
                                                    {
                                                        string strDate = "", hours = "";
                                                        if (value.ToString().Contains(":"))
                                                        {
                                                            var arrDate = value.ToString().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                                            if (arrDate != null && arrDate.Count() > 1)
                                                            {
                                                                foreach (var itemD in arrDate)
                                                                {
                                                                    if (itemD.Contains(":"))
                                                                    {
                                                                        hours = itemD.Trim().Replace("h", ":");
                                                                        if (hours.Length == 5)
                                                                        {
                                                                            hours += ":00";
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        var arrDate1 = itemD.Trim().Split(new string[] { "/", "-" }, StringSplitOptions.RemoveEmptyEntries);
                                                                        if (arrDate1 != null && arrDate1.Count() > 0)
                                                                        {
                                                                            strDate = String.Format("{0}-{1}-{2}", arrDate1[2], arrDate1[1], arrDate1[0]);
                                                                        }
                                                                    }
                                                                }
                                                                dt = DateTime.Parse(String.Format("{0} {1}", strDate, hours));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            var arrDate1 = value.ToString().Split(new string[] { "/", "-" }, StringSplitOptions.RemoveEmptyEntries);
                                                            if (arrDate1 != null && arrDate1.Count() > 0)
                                                            {
                                                                strDate = String.Format("{0}-{1}-{2}", arrDate1[2], arrDate1[1], arrDate1[0]);
                                                                hours = "00:00:00";
                                                                dt = DateTime.Parse(String.Format("{0} {1}", strDate, hours));
                                                            }
                                                        }
                                                    }
                                                    else if (value is long || value is long?)
                                                        dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((long)value);
                                                    else if (value is DateTime || value is DateTime?)
                                                        dt = (DateTime?)value;
                                                    pi.SetValue(emrInputADO, dt);
                                                    valueDataType = "DateTime";
                                                }
                                                else
                                                {
                                                    pi.SetValue(emrInputADO, value);
                                                    valueDataType = "string";
                                                }
                                                object newValue = pi.GetValue(emrInputADO);
                                                Inventec.Common.Logging.LogSystem.Info("Get du lieu cua truong tu printtype success____EmrColumn=" + emrColumn.EmrColumn + ", Key=" + emrColumn.Key + ", Value=" + value + ", valueDataType=" + valueDataType + ", newValue = " + newValue + ", EMR_COLUMN_MAPPING=" + this.printType.EMR_COLUMN_MAPPING + ", printTypeCode=" + this.printType.PRINT_TYPE_CODE + ", printTypeName=" + this.printType.PRINT_TYPE_NAME);
                                            }
                                            catch (Exception ex1)
                                            {
                                                Inventec.Common.Logging.LogSystem.Warn(ex1);
                                                Inventec.Common.Logging.LogSystem.Warn("Get du lieu cua truong tu printtype that bai____EmrColumn=" + emrColumn.EmrColumn + ", Key=" + emrColumn.Key + ", EMR_COLUMN_MAPPING=" + this.printType.EMR_COLUMN_MAPPING);
                                            }
                                        }
                                        else
                                        {
                                            Inventec.Common.Logging.LogSystem.Warn("Get du lieu cua truong tu printtype that bai__Du lieu khong co gia tri____EmrColumn=" + emrColumn.EmrColumn + ", Key=" + emrColumn.Key + ", EMR_COLUMN_MAPPING=" + this.printType.EMR_COLUMN_MAPPING + ", value" + value);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Debug("Bieu in co cau hinh EMR_COLUMN_MAPPING=" + this.printType.EMR_COLUMN_MAPPING + ", tuy nhien co loi khong DeserializeObject duoc tu chuoi json de xu ly");
                            }
                        }
                        catch (Exception ex2)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex2);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return success;
        }

        /// <summary>
        ///2. Chặn người dùng in dựa theo giá trị của key được khai báo:
        ///- Căn cứ vào trường dữ liệu DISABLE_PRINT_BY_KEY_CFG trong SAR_PRINT_TYPE để kiểm tra và chặn người dùng in.
        ///- Giá trị lưu ở trường này có dạng chuỗi json từ 1 list object, mỗi object chứa 3 trường có kiểu dữ liệu là string: Key, Value và Message
        ///vd: [{"Key":"EXP_MEST_STT_ID","Value":"1","Message":"Không cho phép in khi phiếu chưa được duyệt"},{"Key":"EXP_MEST_STT_ID","Value":"4","Message":"Không cho phép in khi phiếu bị từ chối duyệt"}]
        ///--> Khi đó:
        ///+ Nếu người dùng nhấn nút "In", hoặc thực hiện tính năng "In ngay" (ở các nút dạng "Lưu in" hoặc "Lưu" và có check vào checkbox "In") thì kiểm tra:
        ///Nếu key EXP_MEST_STT_ID có giá trị là 1, thì hiển thị thông báo "Không cho phép in khi phiếu chưa được duyệt", không cho phép in
        ///Nếu key EXP_MEST_STT_ID có giá trị là 4, thì hiển thị thông báo "Không cho phép in khi phiếu bị từ chối duyệt", không cho phép in
        ///+ Nếu các key được khai báo không có trong biểu mẫu thì không xử lý
        /// </summary>
        /// <returns></returns>
        private bool ProcessDisablePrint()
        {
            bool result = true;
            try
            {
                if (!String.IsNullOrEmpty(this.printType.DISABLE_PRINT_BY_KEY_CFG) && singleValueDictionary != null && singleValueDictionary.Count > 0)
                {
                    try
                    {
                        List<DisablePrintByCFGADO> disablePrintByCFGADOs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DisablePrintByCFGADO>>(this.printType.DISABLE_PRINT_BY_KEY_CFG);
                        if (disablePrintByCFGADOs != null && disablePrintByCFGADOs.Count > 0)
                        {
                            foreach (var disablePrint in disablePrintByCFGADOs)
                            {
                                if (singleValueDictionary.ContainsKey(disablePrint.Key) && singleValueDictionary[disablePrint.Key] != null && !String.IsNullOrEmpty(disablePrint.Value))
                                {
                                    if (singleValueDictionary[disablePrint.Key].ToString() == disablePrint.Value)
                                    {
                                        if (!String.IsNullOrEmpty(disablePrint.Message))
                                            System.Windows.Forms.MessageBox.Show(disablePrint.Message);
                                        Inventec.Common.Logging.LogSystem.Info("Tim thay gia tri cua key chan khong cho in tuong ung voi cau hinh DISABLE_PRINT_BY_KEY_CFG____Key=" + disablePrint.Key + ", Value=" + disablePrint.Value + ", Message=" + disablePrint.Message);
                                        return false;
                                    }
                                }
                                else
                                {
                                    Inventec.Common.Logging.LogSystem.Warn("Bieu in co cau hinh DISABLE_PRINT_BY_KEY_CFG____Key=" + this.printType.DISABLE_PRINT_BY_KEY_CFG + ", tuy nhien du lieu khong hop le____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => disablePrint), disablePrint));
                                }
                            }
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Bieu in co cau hinh DISABLE_PRINT_BY_KEY_CFG=" + this.printType.DISABLE_PRINT_BY_KEY_CFG + ", tuy nhien co loi khong DeserializeObject duoc tu chuoi json de xu ly");
                        }
                    }
                    catch (Exception ex1)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex1);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }


        //        Nếu key cấu hình tài khoản “CONFIG_KEY__HIS_DESKTOP_PRINT_NOW__NUM_COPY” không được thiết lập hoặc MPS không thoản mãn điều kiện của key cấu hình(trường numCopy = 1) thì
        //Xử lý in số bản in dựa theo giá trị của key được khai báo:
        //Căn cứ vào trường dữ liệu NUM_COPY_BY_KEY_CFG trong SAR_PRINT_TYPE để kiểm tra và lấy số lượng bản in.
        //Giá trị lưu ở trường này có dạng chuỗi json từ 1 list object, mỗi object chứa 3 trường có kiểu dữ liệu là string: Key, Value và NumCopy
        //Gán lại dữ liệu của trường numCopy = NumCopy lấy được được ở NUM_COPY_BY_KEY_CFG đầu tiên thỏa mãn
        //Nếu không có dữ liệu nào thỏa mãn thiết lập thì mặc định in 1 bàn ghi
        private void ProcessNumCopyPrint()
        {
            try
            {
                if (numCopy == 1 && !String.IsNullOrEmpty(this.printType.NUM_COPY_BY_KEY_CFG) && singleValueDictionary != null && singleValueDictionary.Count > 0)
                {
                    try
                    {
                        List<NumCopyPrintByCFGADO> numCopyList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<NumCopyPrintByCFGADO>>(this.printType.NUM_COPY_BY_KEY_CFG);
                        if (numCopyList != null && numCopyList.Count > 0)
                        {
                            foreach (var num in numCopyList)
                            {
                                if (singleValueDictionary.ContainsKey(num.Key) && singleValueDictionary[num.Key] != null && !String.IsNullOrEmpty(num.Value))
                                {
                                    if (singleValueDictionary[num.Key].ToString() == num.Value)
                                    {
                                        if (!String.IsNullOrEmpty(num.NumCopy))
                                        {
                                            numCopy = Int32.Parse(num.NumCopy);
                                            Inventec.Common.Logging.LogSystem.Info("Tim thay gia tri cua key chan khong cho in tuong ung voi cau hinh NUM_COPY_BY_KEY_CFG____Key=" + num.Key + ", Value=" + num.Value + ", NumCopy=" + num.NumCopy);
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    Inventec.Common.Logging.LogSystem.Warn("Bieu in co cau hinh DISABLE_PRINT_BY_KEY_CFG____Key=" + this.printType.NUM_COPY_BY_KEY_CFG + ", tuy nhien du lieu khong hop le____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => num), num));
                                }
                            }
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Bieu in co cau hinh NUM_COPY_BY_KEY_CFG=" + this.printType.NUM_COPY_BY_KEY_CFG + ", tuy nhien co loi khong DeserializeObject duoc tu chuoi json de xu ly");
                        }
                    }
                    catch (Exception ex1)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex1);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        protected void InitType()
        {
            switch (this.templateType)
            {
                case PrintConfig.TemplateType.Excel:
                    store = new Inventec.Common.FlexCellExport.Store();
                    break;
                case PrintConfig.TemplateType.Word:
                    templaterExportStore = new Inventec.Common.TemplaterExport.Store();
                    break;
                case PrintConfig.TemplateType.XtraReport:
                    xtraReportStore = new Inventec.Common.XtraReportExport.Store();
                    break;
                default:
                    store = new Inventec.Common.FlexCellExport.Store();
                    break;
            }
        }

        protected bool PrintPreviewRun()
        {
            bool result = false;
            switch (this.templateType)
            {
                case PrintConfig.TemplateType.Excel:
                    result = PrintPreview(this.saveMemoryStream, this.fileName, store.DictionaryTemplateKey);
                    break;
                case PrintConfig.TemplateType.Word:
                    result = PrintPreviewWord(this.saveFilePath, this.fileName, templaterExportStore.DictionaryTemplateKey, this.isUserWordContent);
                    break;
                case PrintConfig.TemplateType.XtraReport:
                    xtraReportStore.ShowPreview(xtraReportStore.DictionaryTemplateKey, (Inventec.Common.SignLibrary.ADO.InputADO)this.printDataBase.EmrInputADO);
                    result = true;
                    break;
            }
            return result;
        }

        protected bool OutFileRun()
        {
            bool result = false;
            switch (this.templateType)
            {
                case PrintConfig.TemplateType.Excel:
                    result = store.OutFile(saveFilePath);
                    if ((Inventec.Common.SignLibrary.ADO.InputADO)this.printDataBase.EmrInputADO != null)
                    {
                        var emrADO = this.printDataBase.EmrInputADO as Inventec.Common.SignLibrary.ADO.InputADO;
                        if (emrADO != null)
                        {
                            emrADO.PaperSizeDefault = store.GetPaperSizeDefault();
                            this.printDataBase.EmrInputADO = emrADO;
                        }
                    }
                    break;
                case PrintConfig.TemplateType.Word:
                    ProcessDicImageBarcodeForWord();
                    saveFilePath = templaterExportStore.OutFile();
                    result = File.Exists(saveFilePath);
                    break;
                case PrintConfig.TemplateType.XtraReport:
                    //ProcessDicImageBarcodeForWord();
                    //this.saveFilePath = templaterExportStore.OutFile();
                    //this.saveMemoryStream = xtraReportStore.OutStream();
                    //this.saveMemoryStream.Position = 0;
                    result = true;
                    break;
            }

            return result;
        }

        protected bool OutFileStreamRun()
        {
            bool result = false;
            switch (this.templateType)
            {
                case PrintConfig.TemplateType.Excel:
                    this.saveMemoryStream = store.OutStream();
                    this.saveMemoryStream.Position = 0;
                    if ((Inventec.Common.SignLibrary.ADO.InputADO)this.printDataBase.EmrInputADO != null)
                    {
                        var emrADO = this.printDataBase.EmrInputADO as Inventec.Common.SignLibrary.ADO.InputADO;
                        if (emrADO != null)
                        {
                            emrADO.PaperSizeDefault = store.GetPaperSizeDefault();
                            this.printDataBase.EmrInputADO = emrADO;
                        }
                    }
                    result = true;
                    break;
                case PrintConfig.TemplateType.Word:
                    ProcessDicImageBarcodeForWord();
                    this.saveFilePath = templaterExportStore.OutFile();
                    //this.saveMemoryStream = templaterExportStore.OutStream();
                    //this.saveMemoryStream.Position = 0;
                    result = true;
                    break;
                case PrintConfig.TemplateType.XtraReport:
                    //ProcessDicImageBarcodeForWord();
                    //this.saveFilePath = templaterExportStore.OutFile();
                    //this.saveMemoryStream = xtraReportStore.OutPdfStream();
                    //this.saveMemoryStream.Position = 0;
                    result = true;
                    break;
            }
            return result;
        }

        protected bool OutFileStreamRunForSave()
        {
            bool result = false;
            switch (this.templateType)
            {
                case PrintConfig.TemplateType.Excel:
                    this.saveMemoryStream = store.OutStream();
                    this.saveMemoryStream.Position = 0;
                    if ((Inventec.Common.SignLibrary.ADO.InputADO)this.printDataBase.EmrInputADO != null)
                    {
                        var emrADO = this.printDataBase.EmrInputADO as Inventec.Common.SignLibrary.ADO.InputADO;
                        if (emrADO != null)
                        {
                            emrADO.PaperSizeDefault = store.GetPaperSizeDefault();
                            this.printDataBase.EmrInputADO = emrADO;
                        }
                    }
                    result = true;
                    break;
                case PrintConfig.TemplateType.Word:
                    ProcessDicImageBarcodeForWord();
                    //this.saveFilePath = templaterExportStore.OutFile();
                    this.saveMemoryStream = templaterExportStore.OutStream();
                    this.saveMemoryStream.Position = 0;
                    result = true;
                    break;
                case PrintConfig.TemplateType.XtraReport:
                    //ProcessDicImageBarcodeForWord();
                    //this.saveFilePath = templaterExportStore.OutFile();
                    this.saveMemoryStream = xtraReportStore.OutPdfStream();
                    this.saveMemoryStream.Position = 0;
                    result = true;
                    break;
            }
            return result;
        }

        protected bool OutFileStreamRunToPdf()
        {
            bool result = false;
            switch (this.templateType)
            {
                case PrintConfig.TemplateType.Excel:
                    this.saveMemoryStream = store.OutStreamPDF();
                    this.saveMemoryStream.Position = 0;
                    if ((Inventec.Common.SignLibrary.ADO.InputADO)this.printDataBase.EmrInputADO != null)
                    {
                        var emrADO = this.printDataBase.EmrInputADO as Inventec.Common.SignLibrary.ADO.InputADO;
                        if (emrADO != null)
                        {
                            emrADO.PaperSizeDefault = store.GetPaperSizeDefault();
                            this.printDataBase.EmrInputADO = emrADO;
                        }
                    }
                    result = true;
                    break;
                case PrintConfig.TemplateType.Word:
                    string saveFilePathTemp = templaterExportStore.OutFile();
                    result = Inventec.Common.FileConvert.Convert.DocToPdf(null, saveFilePathTemp, null, saveFilePath);
                    break;
                case PrintConfig.TemplateType.XtraReport:
                    this.saveMemoryStream = xtraReportStore.OutPdfStream();
                    this.saveMemoryStream.Position = 0;
                    result = true;
                    break;
            }

            Inventec.Common.Logging.LogSystem.Debug("OutFileStreamRunToPdf.1.1");
            System.Diagnostics.Process proc = System.Diagnostics.Process.GetCurrentProcess();
            //Inventec.Common.Logging.LogSystem.Info("CalculateMemoryRam____Dung luong PM( GC.GetTotalMemory):" + ((decimal)startBytes / (1024 * 1024)) + "MB");
            Inventec.Common.Logging.LogSystem.Info("OutFileStreamRunToPdf____Dung luong PM(proc.PrivateMemorySize64):" + ((decimal)proc.PrivateMemorySize64 / (1024 * 1024)) + "MB");
            Inventec.Common.Logging.LogSystem.Info("OutFileStreamRunToPdf.1.2");
            return result;
        }

        protected void ProcessDicImageBarcodeForWord()
        {
            try
            {
                if (this.dicImage != null && this.dicImage.Count > 0)
                {
                    this.dicImageReplaceBarcode = new Dictionary<string, System.Drawing.Image>();
                    foreach (KeyValuePair<string, Inventec.Common.BarcodeLib.Barcode> pair in dicImage)
                    {
                        try
                        {
                            if (!System.String.IsNullOrEmpty(pair.Value.RawData))
                            {
                                Inventec.Common.BarcodeLib.Barcode barcode = new Inventec.Common.BarcodeLib.Barcode();
                                barcode.Alignment = pair.Value.Alignment;
                                barcode.IncludeLabel = pair.Value.IncludeLabel;
                                barcode.RotateFlipType = pair.Value.RotateFlipType;
                                barcode.LabelPosition = pair.Value.LabelPosition;
                                barcode.EncodedType = pair.Value.EncodedType;
                                barcode.LabelFont = new System.Drawing.Font(barcode.LabelFont.FontFamily, 15);
                                barcode.Width = pair.Value.Width;
                                barcode.Height = pair.Value.Height;
                                var imgBarcode = barcode.Encode(barcode.EncodedType, pair.Value.RawData, barcode.Width, barcode.Height);

                                this.dicImageReplaceBarcode.Add(pair.Key, imgBarcode);
                                //sharpDocxStore.DictionaryTemplateKey[pair.Key] = "BAR CODE " + pair.Key;
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Du lieu truyen vao khong hop le. RawData = " + pair.Value.RawData);
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        protected void SetCommonFunctionRun()
        {
            switch (this.templateType)
            {
                case PrintConfig.TemplateType.Excel:
                    store.SetCommonFunctions();
                    break;
                case PrintConfig.TemplateType.Word:
                    templaterExportStore.SetCommonFunctions();
                    break;
            }
        }

        protected bool EmrShowClick()
        {
            bool success = false;
            try
            {
                Inventec.Common.Logging.LogAction.Debug(this.printTypeCode + ": Begin EmrShowClick");
                SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();
                string base64File = "";
                FileType fileType = FileType.Xlsx;
                switch (this.templateType)
                {
                    case PrintConfig.TemplateType.Excel:
                        fileType = FileType.Pdf;
                        base64File = System.Convert.ToBase64String(Utils.StreamToByte(this.saveMemoryStream));
                        break;
                    case PrintConfig.TemplateType.Word:
                        fileType = FileType.Pdf;
                        base64File = System.Convert.ToBase64String(Utils.FileToByte(this.saveFilePath));
                        break;
                    case PrintConfig.TemplateType.XtraReport:
                        fileType = FileType.Pdf;
                        base64File = System.Convert.ToBase64String(Utils.StreamToByte(this.saveMemoryStream));
                        break;
                }
                InputADO emrInputADO = (InputADO)this.printDataBase.EmrInputADO;
                if (emrInputADO != null)
                {
                    emrInputADO.PrintNumberCopies = (short?)numCopy;
                    if (!VerifySignConfigParam(emrInputADO))
                    {
                        //TODO
                    }
                }

                libraryProcessor.ShowPopup(base64File, fileType, emrInputADO);
                success = true;
                Inventec.Common.Logging.LogAction.Debug(this.printTypeCode + ": End EmrShowClick");
            }
            catch (Exception ex)
            {
                success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        protected bool EmrSignNowClick()
        {
            bool success = false;
            try
            {
                Inventec.Common.Logging.LogAction.Debug(this.printTypeCode + ": Begin EmrSignNowClick");
                SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();
                FileType fileType = FileType.Xlsx;
                string base64File = "";
                switch (this.templateType)
                {
                    case PrintConfig.TemplateType.Excel:
                        fileType = FileType.Pdf;
                        base64File = System.Convert.ToBase64String(Utils.StreamToByte(this.saveMemoryStream));
                        break;
                    case PrintConfig.TemplateType.Word:
                        fileType = FileType.Pdf;
                        base64File = System.Convert.ToBase64String(Utils.FileToByte(this.saveFilePath));
                        break;
                    case PrintConfig.TemplateType.XtraReport:
                        fileType = FileType.Pdf;
                        base64File = System.Convert.ToBase64String(Utils.StreamToByte(this.saveMemoryStream));
                        break;
                }
                InputADO emrInputADO = (InputADO)this.printDataBase.EmrInputADO;
                if (emrInputADO != null)
                {
                    emrInputADO.PrintNumberCopies = (short?)numCopy;
                }
                if (VerifySignConfigParam(emrInputADO))
                {
                    var rsSign = libraryProcessor.SignNow(base64File, fileType, emrInputADO);
                    success = (rsSign != null && rsSign.Success);
                }
                else
                {
                    libraryProcessor.ShowPopup(base64File, fileType, emrInputADO);
                    success = true;
                }

                if (!success)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Thuc hien tao van ban dien tu & ky luon voi bieu mau in " + printType.PRINT_TYPE_NAME + "(" + printType.PRINT_TYPE_CODE + ")" + " that bai____Du lieu dau vao:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => emrInputADO), emrInputADO) + "____Ket qua dau ra:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => success), success));
                }
                Inventec.Common.Logging.LogAction.Debug(this.printTypeCode + ": End EmrSignNowClick");
            }
            catch (Exception ex)
            {
                success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        protected bool EmrSignAndPrintPreviewClick()
        {
            bool success = false;
            try
            {
                Inventec.Common.Logging.LogAction.Debug(this.printTypeCode + ": Begin EmrSignAndPrintPreviewClick");
                SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();
                FileType fileType = FileType.Xlsx;
                string base64File = "";
                switch (this.templateType)
                {
                    case PrintConfig.TemplateType.Excel:
                        fileType = FileType.Pdf;
                        base64File = System.Convert.ToBase64String(Utils.StreamToByte(this.saveMemoryStream));
                        break;
                    case PrintConfig.TemplateType.Word:
                        fileType = FileType.Pdf;
                        base64File = System.Convert.ToBase64String(Utils.FileToByte(this.saveFilePath));
                        break;
                    case PrintConfig.TemplateType.XtraReport:
                        fileType = FileType.Pdf;
                        base64File = System.Convert.ToBase64String(Utils.StreamToByte(this.saveMemoryStream));
                        break;
                }
                InputADO emrInputADO = (InputADO)this.printDataBase.EmrInputADO;
                if (emrInputADO != null)
                    emrInputADO.PrintNumberCopies = (short?)numCopy;

                if (VerifySignConfigParam(emrInputADO))
                {
                    var rsSign = libraryProcessor.SignAndShowPrintPreview(base64File, fileType, emrInputADO);
                    success = (rsSign != null && rsSign.Success);
                }
                else
                {
                    libraryProcessor.ShowPopup(base64File, fileType, emrInputADO);
                    success = true;
                }

                if (!success)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Thuc hien tao van ban dien tu & ky luon voi bieu mau in " + printType.PRINT_TYPE_NAME + "(" + printType.PRINT_TYPE_CODE + ")" + " that bai____Du lieu dau vao:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => emrInputADO), emrInputADO) + "____Ket qua dau ra:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => success), success));
                }
                Inventec.Common.Logging.LogAction.Debug(this.printTypeCode + ": End EmrSignAndPrintPreviewClick");
            }
            catch (Exception ex)
            {
                success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        protected bool EmrSignAndPrintNowClick()
        {
            bool success = false;
            try
            {
                Inventec.Common.Logging.LogAction.Debug(this.printTypeCode + ": Begin EmrSignAndPrintNowClick");
                Inventec.Common.Logging.LogSystem.Debug(this.printTypeCode + ": EmrSignAndPrintNowClick.1");
                SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();
                FileType fileType = FileType.Xlsx;
                string base64File = "";
                switch (this.templateType)
                {
                    case PrintConfig.TemplateType.Excel:
                        fileType = FileType.Pdf;
                        base64File = System.Convert.ToBase64String(Utils.StreamToByte(this.saveMemoryStream));
                        break;
                    case PrintConfig.TemplateType.Word:
                        fileType = FileType.Pdf;
                        base64File = System.Convert.ToBase64String(Utils.FileToByte(this.saveFilePath));
                        break;
                    case PrintConfig.TemplateType.XtraReport:
                        fileType = FileType.Pdf;
                        base64File = System.Convert.ToBase64String(Utils.StreamToByte(this.saveMemoryStream));
                        break;
                }
                InputADO emrInputADO = (InputADO)this.printDataBase.EmrInputADO;
                if (emrInputADO != null)
                    emrInputADO.PrintNumberCopies = (short?)numCopy;
                Inventec.Common.Logging.LogSystem.Debug(this.printTypeCode + ": EmrSignAndPrintNowClick.2");
                if (VerifySignConfigParam(emrInputADO))
                {
                    Inventec.Common.Logging.LogSystem.Debug("EmrSignAndPrintNowClick.3");
                    var rsSign = libraryProcessor.SignAndPrintNow(base64File, fileType, emrInputADO);
                    success = (rsSign != null && rsSign.Success);
                    Inventec.Common.Logging.LogSystem.Debug(this.printTypeCode + ": EmrSignAndPrintNowClick.4");
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(this.printTypeCode + ": EmrSignAndPrintNowClick.5");
                    libraryProcessor.ShowPopup(base64File, fileType, emrInputADO);
                    success = true;
                    Inventec.Common.Logging.LogSystem.Debug(this.printTypeCode + ": EmrSignAndPrintNowClick.6");
                }

                if (!success)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Thuc hien tao van ban dien tu & ky luon voi bieu mau in " + printType.PRINT_TYPE_NAME + "(" + printType.PRINT_TYPE_CODE + ")" + " that bai____Du lieu dau vao:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => emrInputADO), emrInputADO) + "____Ket qua dau ra:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => success), success));
                }
                Inventec.Common.Logging.LogSystem.Debug(this.printTypeCode + ": EmrSignAndPrintNowClick.7");
                Inventec.Common.Logging.LogAction.Debug(this.printTypeCode + ": End EmrSignAndPrintNowClick");
            }
            catch (Exception ex)
            {
                success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        /// <summary>
        /// - Bổ sung loại đầu vào "Tạo văn bản" (EmrCreate).
        ///- Nếu đầu vào "Tạo văn bản" thì tự động tạo văn bản và trả kết quả tạo văn bản thành công hay thất bại (Không cần show form).
        ///- Khi truyền thiết lập ký được truyền từ bên ngoài vào nếu tồn tại tài khoản chưa được thiết lập người ký (EMR_SIGNER) thì thông báo: "Tài khoản 'dunglh', 'phuongdt', ... chưa được tạo thông tin người ký":
        ///  + Trường hợp truyền vào là "Hiển thị" (EmrShow), "Ký" (EmrSignNow), "Ký và in" (EmrSignAndPrint) thì chỉ hiển thị Văn bản không thực hiện các bước ký và in.
        ///  + Trường hợp truyền vào là "Tạo văn bản" thì không thực hiện tạo và trả kết quả tạo thất bại.
        /// </summary>
        /// <returns></returns>
        protected bool EmrCreateDocumentClick()
        {
            bool success = false;
            try
            {
                Inventec.Common.Logging.LogAction.Debug(this.printTypeCode + ": Begin EmrCreateDocumentClick");
                SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();
                FileType fileType = FileType.Xlsx;
                string base64File = "";
                switch (this.templateType)
                {
                    case PrintConfig.TemplateType.Excel:
                        fileType = FileType.Pdf;
                        base64File = System.Convert.ToBase64String(Utils.StreamToByte(this.saveMemoryStream));
                        break;
                    case PrintConfig.TemplateType.Word:
                        fileType = FileType.Pdf;
                        base64File = System.Convert.ToBase64String(Utils.FileToByte(this.saveFilePath));
                        break;
                    case PrintConfig.TemplateType.XtraReport:
                        fileType = FileType.Pdf;
                        base64File = System.Convert.ToBase64String(Utils.StreamToByte(this.saveMemoryStream));
                        break;
                }

                InputADO emrInputADO = (InputADO)this.printDataBase.EmrInputADO;
                if (emrInputADO != null)
                {
                    emrInputADO.PrintNumberCopies = (short?)numCopy;
                }
                if (VerifySignConfigParam(emrInputADO))
                {

                    success = libraryProcessor.CreateDocument(emrInputADO, base64File);
                    if (!success)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Thuc hien tao van ban dien tu " + printType.PRINT_TYPE_NAME + "(" + printType.PRINT_TYPE_CODE + ")" + " that bai____Du lieu dau vao:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => emrInputADO), emrInputADO) + "____Ket qua dau ra:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => success), success));
                    }
                }
                else
                {
                    success = false;
                }
                Inventec.Common.Logging.LogAction.Debug(this.printTypeCode + ": End EmrCreateDocumentClick");
            }
            catch (Exception ex)
            {
                success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        private bool VerifySignConfigParam(InputADO emrInputADO)
        {
            bool valid = true;
            try
            {
                if (emrInputADO != null && emrInputADO.SignerConfigs != null && emrInputADO.SignerConfigs.Count > 0)
                {
                    string messgeErrors = "";
                    var loginnamesigners = PrintConfig.EmrSigners.Where(o => o.IS_ACTIVE == 1).Select(o => o.LOGINNAME).ToList();
                    var signerConfigInvalids = emrInputADO.SignerConfigs.Where(o => !loginnamesigners.Contains(o.Loginname)).Select(o => o.Loginname).ToList();
                    if (signerConfigInvalids != null && signerConfigInvalids.Count > 0)
                    {
                        messgeErrors = String.Format("Tài khoản {0} chưa được tạo thông tin người ký.", String.Join(",", signerConfigInvalids));
                        System.Windows.Forms.MessageBox.Show(messgeErrors);
                        Inventec.Common.Logging.LogSystem.Warn(messgeErrors);
                        valid = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        /// <summary>
        /// lấy số lần in
        /// nếu không ghi log in thì bỏ qua
        /// </summary>
        /// <returns></returns>
        protected long GetNumOrderPrint(string UniqueCode)
        {
            //bằng 0 thì không thêm key để tránh nhầm lẫn có ghi log
            long result = 0;
            try
            {
                if (printType != null && printType.IS_PRINT_LOG == IMSys.DbConfig.SAR_RS.SAR_PRINT_TYPE.IS_PRINT_LOG_TRUE && !String.IsNullOrWhiteSpace(UniqueCode))
                {
                    result = 1;
                    SAR.Filter.SarPrintLogFilter filter = new SAR.Filter.SarPrintLogFilter();
                    filter.PRINT_TYPE_CODE__EXACT = printType.PRINT_TYPE_CODE;
                    filter.UNIQUE_CODE__EXACT = UniqueCode;
                    try
                    {
                        var apiResult = ApiConsumerStore.SarConsumer.Get<Inventec.Core.ApiResultObject<List<SAR_PRINT_LOG>>>("/api/SarPrintLog/Get", new CommonParam(), filter);
                        if (apiResult != null && apiResult.Data != null && apiResult.Data.Count > 0)
                        {
                            result = apiResult.Data.Max(o => o.NUM_ORDER) + 1;
                        }
                    }
                    catch (Exception) { }
                }
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => printType), printType));
                Inventec.Common.Logging.LogSystem.Info("numOrder:" + result);
            }
            catch (Exception ex)
            {
                result = 0;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Tạo trước rồi mới in
        /// false: Nếu tạo lỗi.
        /// true: Nếu loại in không ghi log hoặc ghi log thành công
        /// </summary>
        private bool EventLogPrintWithAfterPrint(ref string error, string printReason)
        {
            bool result = true;
            try
            {
                string loginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                if (printType != null && printType.DO_NOT_ALLOW_PRINT == 1 && (String.IsNullOrEmpty(printType.PRINT_EXCEPTION_LOGINNAME) || (!printType.PRINT_EXCEPTION_LOGINNAME.Contains(loginname))))
                {
                    error = "Bạn không được phép in biểu in này";
                    return false;
                }

                if (printType != null && printType.IS_PRINT_LOG == IMSys.DbConfig.SAR_RS.SAR_PRINT_TYPE.IS_PRINT_LOG_TRUE &&
                    singleValueDictionary.ContainsKey(CommonKey._CURRENT_NUM_ORDER_PRINT))
                {
                    long numOrder = 0;
                    long.TryParse(singleValueDictionary[CommonKey._CURRENT_NUM_ORDER_PRINT].ToString(), out numOrder);
                    if (numOrder > 0)
                    {
                        result = false;

                        if (printType.DO_NOT_ALLOW_REPRINT == 1 && (String.IsNullOrEmpty(printType.REPRINT_EXCEPTION_LOGINNAME) || (!printType.REPRINT_EXCEPTION_LOGINNAME.Contains(loginname))) && numOrder > 1)
                        {
                            error = "Biểu in này không cho phép in lại";
                            return result;
                        }
                        SAR.SDO.SarPrintLogSDO printLog = new SAR.SDO.SarPrintLogSDO();//SAR_PRINT_LOG
                        printLog.PrintTime = Inventec.Common.DateTime.Get.Now();
                        printLog.PrintTypeCode = printType.PRINT_TYPE_CODE;
                        printLog.PrintTypeName = printType.PRINT_TYPE_NAME;
                        printLog.Ip = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginAddress();
                        printLog.LogginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                        printLog.DataContent = ProcessPrintLogData();
                        printLog.UniqueCode = ProcessUniqueCodeData();
                        printLog.NumOrder = numOrder;
                        printLog.PrintReason = printReason;

                        Inventec.Core.ApiResultObject<SAR.SDO.SarPrintLogSDO> aro = null;
                        var param = new CommonParam();
                        try
                        {
                            aro = ApiConsumerStore.SarConsumer.Post<Inventec.Core.ApiResultObject<SAR.SDO.SarPrintLogSDO>>("/api/SarPrintLog/Create", param, printLog);
                        }
                        catch (Exception) { }

                        if (aro != null && aro.Data != null)
                        {
                            result = true;
                        }
                        else
                        {
                            if (param.Messages != null && param.Messages.Count > 0)
                            {
                                error = param.GetMessage();
                            }
                            else
                                error = "Đã có người thực hiện in trước bạn. Vui lòng in lại";

                            Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => aro), aro));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = true;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Nội dung data cần ghi lại
        /// </summary>
        /// <returns></returns>
        public virtual string ProcessPrintLogData()
        {
            string message = "";
            string treatmentCode = "";
            string serviceReqCode = "";
            string transactionCode = "";
            string expMestCode = "";
            string impMestCode = "";
            string patientCode = "";

            if (singleValueDictionary != null && singleValueDictionary.Count > 0)
            {
                foreach (var item in singleValueDictionary)
                {
                    if (String.IsNullOrWhiteSpace(treatmentCode) && item.Key.Contains("TREATMENT_CODE") && item.Value != null)
                    {
                        treatmentCode = item.Value.ToString();
                    }
                    else if (String.IsNullOrWhiteSpace(serviceReqCode) && item.Key.Contains("SERVICE_REQ_CODE") && item.Value != null)
                    {
                        serviceReqCode = item.Value.ToString();
                    }
                    else if (String.IsNullOrWhiteSpace(transactionCode) && item.Key.Contains("TRANSACTION_CODE") && item.Value != null)
                    {
                        transactionCode = item.Value.ToString();
                    }
                    else if (String.IsNullOrWhiteSpace(expMestCode) && item.Key.Contains("EXP_MEST_CODE") && item.Value != null)
                    {
                        expMestCode = item.Value.ToString();
                    }
                    else if (String.IsNullOrWhiteSpace(impMestCode) && item.Key.Contains("IMP_MEST_CODE") && item.Value != null)
                    {
                        impMestCode = item.Value.ToString();
                    }
                    else if (String.IsNullOrWhiteSpace(patientCode) && item.Key.Contains("PATIENT_CODE") && item.Value != null)
                    {
                        patientCode = item.Value.ToString();
                    }
                }
            }

            if (!String.IsNullOrWhiteSpace(patientCode))
            {
                message += string.Format("PATIENT_CODE: {0}. ", patientCode);
            }

            if (!String.IsNullOrWhiteSpace(treatmentCode))
            {
                message += string.Format("TREATMENT_CODE: {0}. ", treatmentCode);
            }

            if (!String.IsNullOrWhiteSpace(serviceReqCode))
            {
                message += string.Format("SERVICE_REQ_CODE: {0}. ", serviceReqCode);
            }

            if (!String.IsNullOrWhiteSpace(transactionCode))
            {
                message += string.Format("TRANSACTION_CODE: {0}. ", transactionCode);
            }

            if (!String.IsNullOrWhiteSpace(expMestCode))
            {
                message += string.Format("EXP_MEST_CODE: {0}. ", expMestCode);
            }

            if (!String.IsNullOrWhiteSpace(impMestCode))
            {
                message += string.Format("IMP_MEST_CODE: {0}. ", impMestCode);
            }

            return message;
        }

        public virtual string ProcessUniqueCodeData()
        {
            return null;
        }

        private void ShowPrintLog()
        {
            try
            {
                if (this.printDataBase.ShowPrintLog != null)
                {
                    this.printDataBase.ShowPrintLog(this.printDataBase.printTypeCode, ProcessUniqueCodeData());
                }
                else if (PrintConfig.ShowModulePrintLog != null)
                {
                    PrintConfig.ShowModulePrintLog(this.printDataBase.printTypeCode, ProcessUniqueCodeData());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private long GetNumOrderPrintByUniqueCodeData()
        {
            long result = 0;
            try
            {
                result = GetNumOrderPrint(ProcessUniqueCodeData());
            }
            catch (Exception ex)
            {
                result = 0;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public void Dispose()
        {
            try
            {
                if (PrintConfig.IsDisposeAfterProcess)
                {
                    string printTypeCodeTemp = printTypeCode;
                    Inventec.Common.Logging.LogSystem.Info(printTypeCodeTemp + ": Dispose.1");
                    Inventec.Common.Logging.LogAction.Debug(printTypeCodeTemp + ": Dispose.1");
                    try
                    {
                        if (saveMemoryStream != null && saveMemoryStream.Length > 0)
                        {
                            saveMemoryStream.Close();
                            saveMemoryStream.Dispose();
                        }
                        saveMemoryStream = null;
                    }
                    catch (Exception ex1)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex1);
                    }
                    DisposeExt();
                    Inventec.Common.Logging.LogAction.Debug(printTypeCodeTemp + ": Dispose.2");
                    Inventec.Common.Logging.LogSystem.Info(printTypeCodeTemp + ": Dispose.2");
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(printTypeCode + ": IsDisposeAfterProcess=" + PrintConfig.IsDisposeAfterProcess);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        protected void DisposeExt()
        {
            try
            {
                // Free any other managed objects here.
                store = null;
                templaterExportStore = null;
                xtraReportStore = null;
                dicImage = null;
                printType = null;
                fileName = null;
                singleValueDictionary = new Dictionary<string, object>();
                dicImageReplaceBarcode = null;
                printerName = null;
                printTypeCode = null;
                treatmentCodeOfPatient = null;
                currentBussinessCode = null;
                printTypeBusinessCodes = null;
                rdoBase = null;
                printDataBase = null;

                //GC.SuppressFinalize(this);
                // Free any unmanaged objects here.
                //Marshal.FreeHGlobal(_bufferPtr);
                //_disposed = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //~AbstractProcessor()
        //{
        //    Dispose(false);
        //}

    }
}
