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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.Desktop.Plugins.ServiceReqList
{
    public partial class frmServiceReqList : HIS.Desktop.Utility.FormBase
    {
        // Bieu in "Phieu chi dinh tong hop" rieng cho vien co khai bao config
        // HIS.Desktop.Plugins.ServiceReqList.PrintTypeCodeChiDinhTongHop = Mps190001.
        // Vien khong khai bao config van in Mps000037 nhu cu.
        private const string PRINT_TYPE_CODE__MPS190001 = "Mps190001";

        // Danh sach y lenh phuc vu build du lieu in Mps190001.
        private List<ADO.ServiceReqADO> listServiceReqPrintMps190001;

        /// <summary>
        /// Lay ma bieu in cua nut "In phieu chi dinh TH" theo cau hinh cua vien.
        /// Khong khai bao config -> tra ve Mps000037 (mau chuan hien tai).
        /// </summary>
        private string GetPrintTypeCodeChiDinhTongHop()
        {
            string result = MPS000037;
            try
            {
                //Nap lai cau hinh tu backend truoc khi doc, de vien sua gia tri key la an ngay
                //khong phai thoat phan mem cho cache config nap lai.
                try
                {
                    HIS.Desktop.LocalStorage.HisConfig.ConfigLoader.Refresh();
                }
                catch (Exception exRefresh)
                {
                    //Refresh that bai thi doc theo cache dang co, khong chan luong in.
                    Inventec.Common.Logging.LogSystem.Warn(exRefresh);
                }

                string configValue = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(
                    Base.ConfigKey.PrintTypeCodeChiDinhTongHop);
                if (!String.IsNullOrWhiteSpace(configValue))
                {
                    result = configValue.Trim();
                }

                //Log de doi chieu khi vien khai bao config nhung van in mau cu.
                Inventec.Common.Logging.LogSystem.Debug(
                    "InPhieuChiDinhTH____config=[" + (configValue ?? "null") + "]____printTypeCode=[" + result + "]");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// In phieu chi dinh tong hop theo mau Mps190001.
        /// </summary>
        private void InPhieuChiDinhTongHopMps190001()
        {
            try
            {
                if (this.listServiceReq == null || this.listServiceReq.Count == 0)
                    return;

                this.listServiceReqPrintMps190001 = this.listServiceReq;

                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(
                    ApiConsumers.SarConsumer,
                    HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(),
                    GlobalVariables.TemnplatePathFolder);

                richEditorMain.RunPrintTemplate(PRINT_TYPE_CODE__MPS190001, DelegateRunPrinterMps190001);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Callback dung du lieu va goi MpsPrinter cho bieu in Mps190001.
        /// </summary>
        private bool DelegateRunPrinterMps190001(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                var selectedReqs = this.listServiceReqPrintMps190001;
                if (selectedReqs == null || selectedReqs.Count == 0)
                    return false;

                WaitingManager.Show();

                // Mau nay in duoc y lenh cua nhieu ho so dieu tri nen lay thoi gian vao vien theo tung ho so.
                Dictionary<long, HIS_TREATMENT> dicTreatment = LoadTreatmentsForPrintMps190001(selectedReqs);
                HIS_TREATMENT firstTreatment = null;

                List<MPS.Processor.Mps190001.PDO.ServiceReqADO> lstPrint =
                    new List<MPS.Processor.Mps190001.PDO.ServiceReqADO>();

                foreach (var item in selectedReqs)
                {
                    MPS.Processor.Mps190001.PDO.ServiceReqADO ado = new MPS.Processor.Mps190001.PDO.ServiceReqADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps190001.PDO.ServiceReqADO>(ado, item);

                    HIS_TREATMENT treatment = dicTreatment.ContainsKey(item.TREATMENT_ID) ? dicTreatment[item.TREATMENT_ID] : null;
                    if (firstTreatment == null)
                        firstTreatment = treatment;

                    ado.IN_TIME = treatment != null ? (long?)treatment.IN_TIME : null;
                    ado.LIST_SERVICE_NAME = GetListServiceNameByServiceReqId(item.ID);

                    lstPrint.Add(ado);
                }

                MPS.Processor.Mps190001.PDO.Mps190001PDO pdo =
                    new MPS.Processor.Mps190001.PDO.Mps190001PDO(lstPrint);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printCode))
                    printerName = GlobalVariables.dicPrinter[printCode];

                WaitingManager.Hide();

                if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(
                        printCode, fileName, pdo,
                        MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                }
                else
                {
                    // Nhieu ho so -> lay ho so cua y lenh dau tien de sinh thong tin ky so nhu cac bieu in gop khac.
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor()
                        .GenerateInputADOWithPrintTypeCode(
                            firstTreatment != null ? firstTreatment.TREATMENT_CODE : "",
                            printCode,
                            currentModule != null ? currentModule.RoomId : 0);

                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(
                        printCode, fileName, pdo,
                        MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName)
                    { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Lay ho so dieu tri cua cac y lenh duoc chon, gom theo TREATMENT_ID (moi ho so chi goi API 1 lan).
        /// </summary>
        private Dictionary<long, HIS_TREATMENT> LoadTreatmentsForPrintMps190001(List<ADO.ServiceReqADO> selectedReqs)
        {
            Dictionary<long, HIS_TREATMENT> result = new Dictionary<long, HIS_TREATMENT>();
            try
            {
                List<long> treatmentIds = selectedReqs.Select(o => o.TREATMENT_ID).Distinct().ToList();
                foreach (var treatmentId in treatmentIds)
                {
                    HIS_TREATMENT treatment = getTreatment(treatmentId);
                    if (treatment != null && !result.ContainsKey(treatmentId))
                    {
                        result.Add(treatmentId, treatment);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Gom ten cac dich vu cua 1 y lenh thanh chuoi de do vao key LIST_SERVICE_NAME tren mau in.
        /// </summary>
        private string GetListServiceNameByServiceReqId(long serviceReqId)
        {
            string result = "";
            try
            {
                List<V_HIS_SERE_SERV> sereServs = GetSereServByServiceReqId(serviceReqId);
                if (sereServs != null && sereServs.Count > 0)
                {
                    result = String.Join(", ", sereServs
                        .Where(o => !String.IsNullOrWhiteSpace(o.TDL_SERVICE_NAME))
                        .Select(o => o.TDL_SERVICE_NAME)
                        .ToList());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
