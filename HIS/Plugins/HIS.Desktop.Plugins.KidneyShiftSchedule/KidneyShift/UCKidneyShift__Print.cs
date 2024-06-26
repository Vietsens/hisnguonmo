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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.KidneyShiftSchedule.ADO;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.KidneyShiftSchedule.KidneyShift
{
    public partial class UCKidneyShift : UserControlBase
    {
        const string printTypeCode = "MPS000320";
        private Inventec.Common.RichEditor.RichEditorStore richEditorMain;
        private bool printNow;
        List<V_HIS_SERVICE_REQ_9> serviceReq9ForPrints;
        string treatmentCode = "";

        /// <summary>
        /// in ngay
        /// </summary>
        /// <param name="PrintTypeCode">mã in</param>
        public void Print(string PrintTypeCode)
        {
            try
            {
                Print(PrintTypeCode, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="PrintTypeCode">Mã in (44,50,118)</param>
        /// <param name="PrintNow">true/false</param>
        public void Print(string PrintTypeCode, bool PrintNow)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("Begin Print Prescription");
                this.printNow = PrintNow;
                richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);

                richEditorMain.RunPrintTemplate(PrintTypeCode, DelegateRunPrinter);
                Inventec.Common.Logging.LogSystem.Info("End Print Prescription");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinter(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                if (ProcessDataForPrint())
                {
                    WaitingManager.Show();
                    treatmentCode = serviceReq9ForPrints.FirstOrDefault().TDL_TREATMENT_CODE;
                    var executeRoom = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().Where(o => o.ROOM_ID == (long)cboExecuteRoom.EditValue).FirstOrDefault();

                    MPS.Processor.Mps000320.PDO.Mps000320PDO mps000320RDO = new MPS.Processor.Mps000320.PDO.Mps000320PDO(executeRoom, serviceReq9ForPrints, this.currentMachines);
                    if (dateDateForSearchServiceReqKidneyshift.EditValue != null)
                        mps000320RDO.Day = dateDateForSearchServiceReqKidneyshift.DateTime;
                    mps000320RDO.FromTime = dateWeekFrom.DateTime;
                    mps000320RDO.ToTime = dateWeekTo.DateTime;
                    if (cboDayOfWeekForSearchServiceReqKidneyshift.EditValue != null)
                        mps000320RDO.Thu = (int)cboDayOfWeekForSearchServiceReqKidneyshift.EditValue;
                    WaitingManager.Hide();
                    PrintData(printTypeCode, fileName, mps000320RDO, printNow, ref result);
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

        private bool ProcessDataForPrint()
        {
            try
            {
                var seviceReqADOPrint = gridControlServiceReqKidneyshift.DataSource as List<ServiceReqADO>;

                AutoMapper.Mapper.CreateMap<ServiceReqADO, MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_9>();
                this.serviceReq9ForPrints = AutoMapper.Mapper.Map<List<ServiceReqADO>, List<V_HIS_SERVICE_REQ_9>>(seviceReqADOPrint);

                if (this.serviceReq9ForPrints == null || this.serviceReq9ForPrints.Count == 0)
                {
                    Inventec.Desktop.Common.Message.MessageManager.Show("Không có dữ liệu để in");
                    return false;
                }
                if (cboExecuteRoom.EditValue == null)
                {
                    Inventec.Desktop.Common.Message.MessageManager.Show("Chưa chọn phòng xử lý");
                    return false;
                }

                if (dateDateForSearchServiceReqKidneyshift.EditValue == null)
                {
                    Inventec.Desktop.Common.Message.MessageManager.Show("Chưa chọn ngày chạy thận");
                    cboDayOfWeekForSearchServiceReqKidneyshift.Focus();
                    cboDayOfWeekForSearchServiceReqKidneyshift.ShowPopup();
                    return false;
                }
            }
            catch (Exception ex)
            { 
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return true;
            //123
        }

        private void PrintData(string printTypeCode, string fileName, object data, bool printNow, ref bool result)
        {
            try
            {
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                if (printNow)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                }
                //else if (ConfigApplicationWorker.Get<string>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__IS_PRINT_NOW) == "1")
                //{
                //    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                //}
                else if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                }
                else
                {
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.treatmentCode, printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);

                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
