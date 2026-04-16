using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.KskInfomantionOfficials
{
    partial class frmKskInfomantionOfficials
    {
        public void PrintMps000507()
        {
            try
            {
                if (currentData == null) return;

                WaitingManager.Show();
                CommonParam param = new CommonParam();

                // 1. Service Req
                V_HIS_SERVICE_REQ serviceReq = null;
                {
                    var filter = new HisServiceReqFilter();
                    filter.ID = currentData.ID;
                    var rs = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, filter, param);
                    serviceReq = rs != null && rs.Count > 0 ? rs.FirstOrDefault() : null;
                }

                // 2. KSK General
                HIS_KSK_GENERAL kskGeneral = currentData.KSK_GENERAL;

                // 3. DHST
                HIS_DHST dhst = new HIS_DHST();
                if (kskGeneral != null && kskGeneral.HIS_DHST != null)
                {
                    dhst = kskGeneral.HIS_DHST;
                }

                // 4. Treatment
                V_HIS_TREATMENT_4 treatment = null;
                if (currentData.TREATMENT_ID > 0)
                {
                    var tFilter = new HisTreatmentView4Filter { ID = currentData.TREATMENT_ID };
                    var treatments = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_4>>("api/HisTreatment/GetView4", ApiConsumers.MosConsumer, tFilter, param);
                    treatment = treatments != null && treatments.Count > 0 ? treatments.FirstOrDefault() : null;
                }

                // 5. Health Exam Ranks
                var examRanks = BackendDataWorker.Get<HIS_HEALTH_EXAM_RANK>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                // 6. Disease Details (V_HIS_DISEASE_DETAIL)
                var diseaseDetailList = diseaseDetails ?? new List<V_HIS_DISEASE_DETAIL>();

                // 7. Disease Detail Results
                var diseaseResultList = diseaseResults ?? new List<HIS_DISEASE_DETAIL_RESULT>();

                WaitingManager.Hide();

                // Tạo PDO
                MPS.Processor.Mps000507.PDO.Mps000507PDO rdo = new MPS.Processor.Mps000507.PDO.Mps000507PDO(
                    kskGeneral,
                    serviceReq,
                    dhst,
                    treatment,
                    examRanks,
                    diseaseDetailList,
                    diseaseResultList
                );

                // Gọi print
                string mps000507Code = "Mps000507";
                Inventec.Common.RichEditor.RichEditorStore richEditor = new Inventec.Common.RichEditor.RichEditorStore(
                    ApiConsumer.ApiConsumers.SarConsumer,
                    HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(),
                    HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                richEditor.RunPrintTemplate(mps000507Code, delegate (string printTypeCode507, string fileName507)
                {
                    bool printResult = false;
                    try
                    {
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor()
                            .GenerateInputADOWithPrintTypeCode(
                                serviceReq != null ? serviceReq.TREATMENT_CODE : "",
                                mps000507Code,
                                moduleData != null ? moduleData.RoomId : 0);

                        MPS.ProcessorBase.Core.PrintData printData;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode507, fileName507, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode507, fileName507, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                        }
                        printResult = MPS.MpsPrinter.Run(printData);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                    return printResult;
                });
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
