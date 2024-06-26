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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Plugins.HisRationSum.ADO;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraBars;
using HIS.Desktop.LocalStorage.LocalData;

namespace HIS.Desktop.Plugins.HisRationSum
{
    public partial class UCHisRationSum : HIS.Desktop.Utility.UserControlBase
    {
        BarManager barManager = null;
        PopupMenuProcessor popupMenuProcessor = null;

        internal enum PrintType
        {
            IN_TONG_HOP,
            IN_CHI_TIET,
            CHIA_SUAT_AN
        }

        private void onClickBtnPrintTongHop()
        {
            try
            {
                PrintType type = new PrintType();
                type = PrintType.IN_TONG_HOP;
                PrintProcess(type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void onClickBtnPrintChiTiet()
        {
            try
            {
                PrintType type = new PrintType();
                type = PrintType.IN_CHI_TIET;
                PrintProcess(type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void onClickBtnPrintChiaSuatAn()
        {
            try
            {
                PrintType type = new PrintType();
                type = PrintType.CHIA_SUAT_AN;
                PrintProcess(type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Print()
        {
            try
            {
                if (this.barManager == null)
                    this.barManager = new DevExpress.XtraBars.BarManager();
                this.barManager.Images = imageCollection2;
                this.barManager.Form = this;
                this.popupMenuProcessor = new PopupMenuProcessor(this.barManager, Transaction_MouseRightClick, null);
                this.popupMenuProcessor.InitMenu();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Transaction_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem)
                {
                    var type = (PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case PopupMenuProcessor.ItemType.InTongHop:
                            onClickBtnPrintTongHop();
                            break;
                        case PopupMenuProcessor.ItemType.InChiTiet:
                            onClickBtnPrintChiTiet();
                            break;
                        case PopupMenuProcessor.ItemType.ChiaSuatAn:
                            onClickBtnPrintChiaSuatAn();
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintProcess(PrintType printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintType.IN_TONG_HOP:
                        richEditorMain.RunPrintTemplate(MPS.Processor.Mps000273.PDO.Mps000273PDO.printTypeCode, DelegateRunPrinter);
                        break;
                    case PrintType.IN_CHI_TIET:
                        richEditorMain.RunPrintTemplate(MPS.Processor.Mps000274.PDO.Mps000274PDO.printTypeCode, DelegateRunPrinter);
                        break;
                    case PrintType.CHIA_SUAT_AN:
                        richEditorMain.RunPrintTemplate(MPS.Processor.Mps000329.PDO.Mps000329PDO.printTypeCode, DelegateRunPrinter);
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case MPS.Processor.Mps000273.PDO.Mps000273PDO.printTypeCode:
                        LoadBieuMauTongHop(printTypeCode, fileName, ref result);
                        break;
                    case MPS.Processor.Mps000274.PDO.Mps000274PDO.printTypeCode:
                        LoadBieuMauChiTiet(printTypeCode, fileName, ref result);
                        break;
                    case MPS.Processor.Mps000329.PDO.Mps000329PDO.printTypeCode:
                        LoadBieuMauChiaSuatAn(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        internal void LoadBieuMauTongHop(string printTypeCode, string fileName, ref bool result)
        {
            try
            {

                CommonParam param = new CommonParam();

                var parent = this.gridViewServiceReqRationSumDetail.GetFocusedRow();
                if (parent != null)
                {
                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    WaitingManager.Show();
                    var rationSum = (MOS.EFMODEL.DataModels.V_HIS_RATION_SUM)gridViewRationSum.GetFocusedRow();

                    /*SereServ1ADO serviceADO = (SereServ1ADO)parent;
                    var childRens = this.dataSource.Where(o => o.PARENT_ID_STR == serviceADO.CHILD_ID);

                    List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_1> sereServs = new List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_1>();
                    foreach (var item in childRens)
                    {
                        V_HIS_SERE_SERV_1 ss = new V_HIS_SERE_SERV_1();
                        AutoMapper.Mapper.CreateMap<SereServADO, V_HIS_SERE_SERV_1>();
                        ss = AutoMapper.Mapper.Map<V_HIS_SERE_SERV_1>(item);
                        ss.AMOUNT = item.AMOUNT_SUM;
                        sereServs.Add(ss);
                    }
                    */
                    SereServ15ADO serviceADO = (SereServ15ADO)parent;

                    //Inventec.Common.Mapper.DataObjectMapper.Map<List<V_HIS_SERE_SERV_1>>(ListSereServs, ListSereServs1);

                    AutoMapper.Mapper.CreateMap<V_HIS_SERE_SERV_15, V_HIS_SERE_SERV_1>();
                    List<V_HIS_SERE_SERV_1> listSereServ1 = AutoMapper.Mapper.Map<List<V_HIS_SERE_SERV_1>>(ListSereServs);

                    var sereServs = listSereServ1.Where(o => o.TDL_REQUEST_DEPARTMENT_ID == serviceADO.TDL_REQUEST_DEPARTMENT_ID).ToList();
                    
                    
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(serviceADO.TDL_TREATMENT_CODE, printTypeCode, roomId);

                    MPS.Processor.Mps000273.PDO.Mps000273PDO mps000077RDO = new MPS.Processor.Mps000273.PDO.Mps000273PDO(
                           rationSum,
                           sereServs,
                           null
                           );
                    WaitingManager.Hide();
                    //MPS.ProcessorBase.Core.PrintData PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000077RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    //result = MPS.MpsPrinter.Run(PrintData);

                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000077RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        internal void LoadBieuMauChiTiet(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();

                var parent = this.gridViewServiceReqRationSumDetail.GetFocusedRow();
                if (parent != null)
                {
                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    WaitingManager.Show();
                    var rationSum = (MOS.EFMODEL.DataModels.V_HIS_RATION_SUM)gridViewRationSum.GetFocusedRow();
                    //this.ListSereServs

                    SereServ15ADO serviceADO15 = (SereServ15ADO)parent;
                    var childRens = this.ListSereServs.Where(o => o.TDL_REQUEST_DEPARTMENT_ID == serviceADO15.TDL_REQUEST_DEPARTMENT_ID).ToList();

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(serviceADO15.TDL_TREATMENT_CODE, printTypeCode, roomId);
                    
                    MPS.Processor.Mps000274.PDO.Mps000274PDO mps000077RDO = new MPS.Processor.Mps000274.PDO.Mps000274PDO(
                           rationSum,
                           childRens,
                           null
                           );
                    WaitingManager.Hide();
                    //MPS.ProcessorBase.Core.PrintData PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000077RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    //result = MPS.MpsPrinter.Run(PrintData);

                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000077RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        internal void LoadBieuMauChiaSuatAn(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                List<MOS.EFMODEL.DataModels.V_HIS_RATION_SUM> apiResultRationSum = null;
                MOS.Filter.HisRationSumViewFilter rationSumViewFilter = new MOS.Filter.HisRationSumViewFilter();
                rationSumViewFilter.ROOM_ID = this.roomId;
                rationSumViewFilter.ORDER_FIELD = "MODIFY_TIME";
                rationSumViewFilter.ORDER_DIRECTION = "DESC";


                apiResultRationSum = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).Get<List<MOS.EFMODEL.DataModels.V_HIS_RATION_SUM>>
                    ("api/HisRationSum/GetView", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, rationSumViewFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("apiResultRationSum.Count", apiResultRationSum.Count));
                if (apiResultRationSum != null && apiResultRationSum.Count > 0)
                {
                    paramCommon = new CommonParam();
                    MOS.Filter.HisSereServView1Filter filter = new MOS.Filter.HisSereServView1Filter();
                    filter.RATION_SUM_IDs = apiResultRationSum.Select(o => o.ID).Distinct().ToList();
                    filter.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN;
                    //filter.INTRUCTION_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(DateTime.Now.ToString("yyyyMMdd000000"));
                    //filter.INTRUCTION_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(DateTime.Now.ToString("yyyyMMdd235959"));
                    List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_1> childRens = new Inventec.Common.Adapter.BackendAdapter
                        (paramCommon).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_1>>
                        ("api/HisSereServ/GetView1", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("childRens.Count", childRens.Count));
                    if (childRens != null && childRens.Count > 0)
                    {
                        string printerName = "";
                        if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                        {
                            printerName = GlobalVariables.dicPrinter[printTypeCode];
                        }

                        WaitingManager.Show();

                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(childRens.First().TDL_TREATMENT_CODE, printTypeCode, roomId);

                        //TODO
                        MPS.Processor.Mps000329.PDO.Mps000329PDO mps000077RDO = new MPS.Processor.Mps000329.PDO.Mps000329PDO(
                               childRens,
                               null,
                               null
                               );

                        mps000077RDO.SingleKeyValue = new MPS.Processor.Mps000329.PDO.SingleKeyValue();
                        mps000077RDO.SingleKeyValue.INTRUCTION_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(DateTime.Now.ToString("yyyyMMdd") + "000000");
                        //mps000077RDO.SingleKeyValue.DEPARTMENT_NAME = "";//TODO
                        //mps000077RDO.SingleKeyValue.DEPARTMENT_ID = 0;//TODO

                        WaitingManager.Hide();

                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000077RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                    }
                    else
                    {
                        MessageManager.Show("Không có dữ liệu suất ăn để in");
                    }
                }
                else
                {
                    MessageManager.Show("Không có dữ liệu suất ăn để in");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }
    }
}
