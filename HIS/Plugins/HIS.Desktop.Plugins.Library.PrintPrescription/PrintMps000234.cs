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
using ACS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using HIS.Desktop.Plugins.Library.PrintPrescription.ADO;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintPrescription
{
    class PrintMps000234
    {
        private V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter;
        private HIS_SERVICE_REQ HisServiceReq_Exam;
        private HIS_SERVICE_REQ hisServiceReq_CurentExam;
        private HIS_DHST hisDhst;
        private HIS_SERVICE_REQ HisPrescriptionSDOPrintPlus;
        private string mediStockName;
        private string expMestCode;
        private V_HIS_ROOM executeRoom;
        private V_HIS_ROOM reqRoom;
        private V_HIS_PATIENT vHisPatient;
        private HIS_TREATMENT hisTreatment;
        private List<HIS_SERE_SERV> hisListSereServClsTt;
        string treatmentCode = "";

        short IS_TRUE = 1;

        List<ExpMestMedicineSDO> listGayNghien;
        List<ExpMestMedicineSDO> listHuongThan;
        List<ExpMestMedicineSDO> listTPCN;
        private int numCopy;
        private bool printNow;
        MPS.ProcessorBase.PrintConfig.PreviewType? previewType;
        Inventec.Common.RichEditor.RichEditorStore richEditorMain;
        private Inventec.Desktop.Common.Modules.Module currentModule;
        long? keyOrderListData;

        Action<int, Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> SavedData;
        Action<string> CancelPrint;

        public PrintMps000234(string printTypeCode, string fileName, ref bool result,
            MOS.SDO.OutPatientPresResultSDO currentOutPresSDO,
            bool printNow, bool hasMediMate,
            Inventec.Common.RichEditor.RichEditorStore _richEditorMain,
            HIS_EXP_MEST expMestPrimary, Inventec.Desktop.Common.Modules.Module module,
            MPS.ProcessorBase.PrintConfig.PreviewType? previewType,
            Action<int> countData,
            Action<int, Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> savedData,
            Action<string> cancelPrint,
            bool CallFromPrescription,
            bool IsNotShowTaken
            )
        {
            try
            {
                this.printNow = printNow;
                this.currentModule = module;
                this.richEditorMain = _richEditorMain;
                this.previewType = previewType;
                this.SavedData = savedData;
                this.CancelPrint = cancelPrint;
                string keyOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Config.ASSIGN_PRESCRIPTION_ODER_OPTION);
                if (!String.IsNullOrWhiteSpace(keyOption))
                {
                    keyOrderListData = Inventec.Common.TypeConvert.Parse.ToInt64(keyOption);
                }
                
                if (currentOutPresSDO != null)
                {
                    vHisPatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                    HisServiceReq_Exam = new HIS_SERVICE_REQ();
                    hisServiceReq_CurentExam = new HIS_SERVICE_REQ();
                    hisDhst = new HIS_DHST();
                    mediStockName = "";
                    HisPrescriptionSDOPrintPlus = new HIS_SERVICE_REQ();
                    executeRoom = new V_HIS_ROOM();
                    reqRoom = new V_HIS_ROOM();
                    List<long> exeDepaIds = new List<long>();
                    List<long> reqDepaIds = new List<long>();

                    List<MOS.EFMODEL.DataModels.HIS_EXP_MEST> ExpMests = null;
                    ThreadMedicineADO mediMateIn = null;
                    ThreadMedicineADO mediMateOut = null;

                    if (currentOutPresSDO.ExpMests != null &&
                        currentOutPresSDO.ExpMests.Count > 0)
                    {
                        ExpMests = currentOutPresSDO.ExpMests;
                        mediMateIn = new ThreadMedicineADO(currentOutPresSDO, hasMediMate);
                        mediMateOut = new ThreadMedicineADO(currentOutPresSDO, hasMediMate);
                        HisPrescriptionSDOPrintPlus = currentOutPresSDO.ServiceReqs.FirstOrDefault();
                        exeDepaIds = currentOutPresSDO.ServiceReqs.Select(s => s.EXECUTE_DEPARTMENT_ID).Distinct().ToList();
                        reqDepaIds = currentOutPresSDO.ServiceReqs.Select(s => s.REQUEST_DEPARTMENT_ID).Distinct().ToList();
                    }

                    //thông tin hồ sơ lấy từ thông tin y lệnh nên bắt buộc phải truyền vào
                    if (ExpMests == null || ExpMests.Count <= 0 || HisPrescriptionSDOPrintPlus == null)
                    {
                        result = false;
                        return;
                    }

                    if (IsNotShowTaken)
                    {
                        ExpMests = ExpMests.Where(o => o.IS_NOT_TAKEN != 1).ToList();
                        if (ExpMests == null)
                        {
                            result = false;
                            return;
                        }
                    }

                    CreateThreadGetCurrentData(HisPrescriptionSDOPrintPlus);

                    new ThreadLoadDataMediMate(mediMateIn, mediMateOut, hisTreatment);

                    if (mediMateIn != null && mediMateIn.DicLstMediMateExpMestTypeADO != null && countData != null)
                    {
                        countData(mediMateIn.DicLstMediMateExpMestTypeADO.Values.SelectMany(s => s).Count());
                    }

                    if (mediMateOut != null && mediMateOut.DicLstMediMateExpMestTypeADO != null && countData != null)
                    {
                        countData(mediMateOut.DicLstMediMateExpMestTypeADO.Values.SelectMany(s => s).Count());
                    }

                    //Thong tin thuoc / vat tu trong kho
                    List<MPS.Processor.Mps000234.PDO.ExpMestMedicineSDO> ExpMestMedicineSDO = new List<MPS.Processor.Mps000234.PDO.ExpMestMedicineSDO>();
                    //Thong tin thuoc / vat tu trong kho va ngoai kho
                    List<MPS.Processor.Mps000234.PDO.ExpMestMedicineSDO> ExpMestMedicineSDOIncludeOutStock = new List<MPS.Processor.Mps000234.PDO.ExpMestMedicineSDO>();

                    MPS.Processor.Mps000234.PDO.Mps000234ADO mps000234ADO = new MPS.Processor.Mps000234.PDO.Mps000234ADO();
                    List<ACS.EFMODEL.DataModels.ACS_USER> acsUsers = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                    List<HIS_SERVICE_REQ> listReq = new List<HIS_SERVICE_REQ>();

                    //Thuốc gây nghiện trong kho
                    List<MPS.Processor.Mps000234.PDO.ExpMestMedicineSDO> listGayNghien234 = new List<MPS.Processor.Mps000234.PDO.ExpMestMedicineSDO>();
                    //Thuốc hướng thần  trong kho
                    List<MPS.Processor.Mps000234.PDO.ExpMestMedicineSDO> listHuongThan234 = new List<MPS.Processor.Mps000234.PDO.ExpMestMedicineSDO>();
                    //Thuốc có chứa dược chất gây nghiện trong kho
                    List<MPS.Processor.Mps000234.PDO.ExpMestMedicineSDO> listHCGN234 = new List<MPS.Processor.Mps000234.PDO.ExpMestMedicineSDO>();
                    //Thuốc có chứa dược chất hướng thần trong kho
                    List<MPS.Processor.Mps000234.PDO.ExpMestMedicineSDO> listHCHT234 = new List<MPS.Processor.Mps000234.PDO.ExpMestMedicineSDO>();

                    //Thuốc gây nghiện ngoài kho
                    List<MPS.Processor.Mps000234.PDO.ExpMestMedicineSDO> listOutStockGN234 = new List<MPS.Processor.Mps000234.PDO.ExpMestMedicineSDO>();
                    //Thuốc hướng thần ngoài kho
                    List<MPS.Processor.Mps000234.PDO.ExpMestMedicineSDO> listOutStockHT234 = new List<MPS.Processor.Mps000234.PDO.ExpMestMedicineSDO>();
                    //Thuốc có chứa dược chất gây nghiện ngoài kho
                    List<MPS.Processor.Mps000234.PDO.ExpMestMedicineSDO> listOutStockHCGN234 = new List<MPS.Processor.Mps000234.PDO.ExpMestMedicineSDO>();
                    //Thuốc có chứa dược chất hướng thần ngoài kho
                    List<MPS.Processor.Mps000234.PDO.ExpMestMedicineSDO> listOutStockHCHT234 = new List<MPS.Processor.Mps000234.PDO.ExpMestMedicineSDO>();

                    foreach (var item in ExpMests)
                    {
                        expMestCode = item.EXP_MEST_CODE;
                        var room = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == item.MEDI_STOCK_ID);
                        mediStockName = room != null ? room.MEDI_STOCK_NAME : "";

                        //Thong tin thuoc / vat tu trong kho
                        List<ExpMestMedicineSDO> lstMedicineExpmestTypeADO = new List<ExpMestMedicineSDO>();

                        //Thong tin thuoc / vat tu trong kho va ngoai kho
                        List<ExpMestMedicineSDO> lstMedicineExpmestTypeADOIncludeOutStock = new List<ExpMestMedicineSDO>();

                        listGayNghien = new List<ExpMestMedicineSDO>();
                        listHuongThan = new List<ExpMestMedicineSDO>();
                        listTPCN = new List<ExpMestMedicineSDO>();

                        if (mediMateIn.DicLstMediMateExpMestTypeADO.ContainsKey(item.ID) &&
                            mediMateIn.DicLstMediMateExpMestTypeADO[item.ID] != null &&
                            mediMateIn.DicLstMediMateExpMestTypeADO[item.ID].Count > 0)
                        {
                            lstMedicineExpmestTypeADO.AddRange(mediMateIn.DicLstMediMateExpMestTypeADO[item.ID]);
                            lstMedicineExpmestTypeADOIncludeOutStock.AddRange(mediMateIn.DicLstMediMateExpMestTypeADO[item.ID]);
                        }

                        if (mediMateOut.DicLstMediMateExpMestTypeADO.ContainsKey(item.SERVICE_REQ_ID ?? 0) &&
                            mediMateOut.DicLstMediMateExpMestTypeADO[item.SERVICE_REQ_ID ?? 0] != null &&
                            mediMateOut.DicLstMediMateExpMestTypeADO[item.SERVICE_REQ_ID ?? 0].Count > 0)
                        {
                            lstMedicineExpmestTypeADOIncludeOutStock.AddRange(mediMateOut.DicLstMediMateExpMestTypeADO[item.SERVICE_REQ_ID ?? 0]);
                        }

                        if (currentOutPresSDO.ServiceReqs != null &&
                            currentOutPresSDO.ServiceReqs.Count > 0)
                        {
                            if (IsNotShowTaken)
                            {
                                HisPrescriptionSDOPrintPlus = currentOutPresSDO.ServiceReqs.FirstOrDefault(o => o.ID == item.SERVICE_REQ_ID && o.IS_NO_EXECUTE != 1);
                            }
                            else
                            {
                                HisPrescriptionSDOPrintPlus = currentOutPresSDO.ServiceReqs.FirstOrDefault(o => o.ID == item.SERVICE_REQ_ID);
                            }
                        }

                        if (HisPrescriptionSDOPrintPlus == null) continue;
                        if (HisPrescriptionSDOPrintPlus.USE_TIME == null)
                        {
                            HisPrescriptionSDOPrintPlus.USE_TIME = HisPrescriptionSDOPrintPlus.INTRUCTION_TIME;
                        }

                        executeRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.EXECUTE_ROOM_ID);
                        reqRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.REQUEST_ROOM_ID);

                        numCopy = 1;
                        if (NumCopy.NumCopyConfig.NumCopys != null && NumCopy.NumCopyConfig.NumCopys.Count > 0)
                        {
                            var num = NumCopy.NumCopyConfig.NumCopys.Where(o => o.Id == vHisPatientTypeAlter.PATIENT_TYPE_ID).ToList();
                            if (num != null && num.Count > 0) numCopy = num[0].Num;
                        }
                        if (numCopy <= 0) numCopy = 1;

                        if (CallFromPrescription && HisPrescriptionSDOPrintPlus.IS_SUB_PRES == 1)
                        {
                            numCopy = 1;
                        }

                        if (lstMedicineExpmestTypeADO != null && lstMedicineExpmestTypeADO.Count > 0)
                        {
                            foreach (var aitem in lstMedicineExpmestTypeADO)
                            {
                                MPS.Processor.Mps000234.PDO.ExpMestMedicineSDO ado = new MPS.Processor.Mps000234.PDO.ExpMestMedicineSDO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000234.PDO.ExpMestMedicineSDO>(ado, aitem);
                                ado.EXP_MEST_CODE = item.EXP_MEST_CODE;
                                ACS_USER user = acsUsers.FirstOrDefault(o => o.LOGINNAME == item.CREATOR);

                                ado.CREATOR_NAME = user != null ? user.USERNAME : "";
                                ado.REQ_ROOM_ID = reqRoom.ID;
                                ado.MEDI_STOCK_ID = item.MEDI_STOCK_ID;
                                ExpMestMedicineSDO.Add(ado);
                            }
                        }

                        if (lstMedicineExpmestTypeADOIncludeOutStock != null && lstMedicineExpmestTypeADOIncludeOutStock.Count > 0)
                        {
                            foreach (var aitem in lstMedicineExpmestTypeADOIncludeOutStock)
                            {
                                MPS.Processor.Mps000234.PDO.ExpMestMedicineSDO ado = new MPS.Processor.Mps000234.PDO.ExpMestMedicineSDO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000234.PDO.ExpMestMedicineSDO>(ado, aitem);
                                ado.EXP_MEST_ID = item.ID;
                                ado.EXP_MEST_CODE = item.EXP_MEST_CODE;
                                ACS_USER user = acsUsers.FirstOrDefault(o => o.LOGINNAME == item.CREATOR);

                                ado.CREATOR_NAME = user != null ? user.USERNAME : "";
                                ado.REQ_ROOM_ID = reqRoom.ID;
                                ado.MEDI_STOCK_ID = item.MEDI_STOCK_ID;
                                ExpMestMedicineSDOIncludeOutStock.Add(ado);
                            }
                        }
                    }

                    if (ExpMests != null && ExpMests.Count > 0)
                    {
                        CommonParam param = new CommonParam();
                        HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                        serviceReqFilter.IDs = ExpMests.Select(o => o.SERVICE_REQ_ID ?? 0).Distinct().ToList();
                        listReq = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(RequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumer.ApiConsumers.MosConsumer, serviceReqFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        
                        if (IsNotShowTaken)
                        {
                            listReq = listReq.Where(o => o.IS_NO_EXECUTE != 1).ToList();
                            if (listReq == null || listReq.Count <= 0)
                            {
                                result = false;
                                return;
                            }
                        }
                    }

                    var IsSeparatePrintingGN = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN).IS_SEPARATE_PRINTING ?? 0;
                    var IsSeparatePrintingHT = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT).IS_SEPARATE_PRINTING ?? 0;

                    if (IsSeparatePrintingGN == 1)
                    {
                        listGayNghien234 = ExpMestMedicineSDO.Where(o => o.IS_ADDICTIVE == IS_TRUE).OrderBy(o => o.NUM_ORDER ?? 99999).ToList();
                        ExpMestMedicineSDO = ExpMestMedicineSDO.Where(o => o.IS_ADDICTIVE != IS_TRUE).ToList();
                        listOutStockGN234 = ExpMestMedicineSDOIncludeOutStock.Where(o => o.IS_ADDICTIVE == IS_TRUE).OrderBy(o => o.NUM_ORDER ?? 99999).ToList();
                        ExpMestMedicineSDOIncludeOutStock = ExpMestMedicineSDOIncludeOutStock.Where(o => o.IS_ADDICTIVE != IS_TRUE).ToList();

                        listHCGN234 = ExpMestMedicineSDO.Where(o => o.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCGN).OrderBy(o => o.NUM_ORDER).ToList();
                        listOutStockHCGN234 = ExpMestMedicineSDOIncludeOutStock.Where(o => o.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCGN).OrderBy(o => o.NUM_ORDER).ToList();

                        //Bo thuoc co chua duoc chat GN ra khoi danh sach thuoc tong hop
                        if (listHCGN234 != null && listHCGN234.Count > 0)
                        {
                            ExpMestMedicineSDO = ExpMestMedicineSDO.Except(listHCGN234).ToList();
                        }
                        if (listOutStockHCGN234 != null && listOutStockHCGN234.Count() > 0)
                        {
                            ExpMestMedicineSDOIncludeOutStock = ExpMestMedicineSDOIncludeOutStock.Except(listOutStockHCGN234).ToList();

                        }
                    }

                    if (IsSeparatePrintingHT == 1)
                    {
                        listHuongThan234 = ExpMestMedicineSDO.Where(o => o.IS_NEUROLOGICAL == IS_TRUE).OrderBy(o => o.NUM_ORDER ?? 99999).ToList();
                        ExpMestMedicineSDO = ExpMestMedicineSDO.Where(o => o.IS_NEUROLOGICAL != IS_TRUE).ToList();
                        listOutStockHT234 = ExpMestMedicineSDOIncludeOutStock.Where(o => o.IS_NEUROLOGICAL == IS_TRUE).OrderBy(o => o.NUM_ORDER ?? 99999).ToList();
                        ExpMestMedicineSDOIncludeOutStock = ExpMestMedicineSDOIncludeOutStock.Where(o => o.IS_NEUROLOGICAL != IS_TRUE).ToList();

                        listHCHT234 = ExpMestMedicineSDO.Where(o => o.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCHT).OrderBy(o => o.NUM_ORDER).ToList();
                        listOutStockHCHT234 = ExpMestMedicineSDOIncludeOutStock.Where(o => o.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCHT).OrderBy(o => o.NUM_ORDER).ToList();

                        //Bo thuoc co chua duoc chat HT ra khoi danh sach thuoc tong hop
                        if (listHCHT234 != null && listHCHT234.Count > 0)
                        {
                            ExpMestMedicineSDO = ExpMestMedicineSDO.Except(listHCHT234).ToList();
                        }
                        if (listOutStockHCHT234 != null && listOutStockHCHT234.Count() > 0)
                        {
                            ExpMestMedicineSDOIncludeOutStock = ExpMestMedicineSDOIncludeOutStock.Except(listOutStockHCHT234).ToList();

                        }
                    }

                    //Xếp thuốc theo thứ tự
                    ExpMestMedicineSDO = ExpMestMedicineSDO.OrderBy(o => o.NUM_ORDER ?? 99999).ToList();
                    ExpMestMedicineSDOIncludeOutStock = ExpMestMedicineSDOIncludeOutStock.OrderBy(o => o.NUM_ORDER ?? 99999).ToList();

                    Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000234.PDO.Mps000234ADO>(mps000234ADO, vHisPatient);
                    if (expMestPrimary != null)
                    {
                        mps000234ADO.EXP_MEST_CODE = expMestPrimary.EXP_MEST_CODE;
                        mps000234ADO.NUMBER_ORDER_OF_DAY = expMestPrimary.NUM_ORDER.ToString();
                    }
                    else if (!String.IsNullOrEmpty(treatmentCode))
                    {
                        CommonParam param = new CommonParam();
                        HisExpMestFilter hisExpMestFilter = new HisExpMestFilter();
                        hisExpMestFilter.TDL_TREATMENT_CODE__EXACT = treatmentCode;
                        hisExpMestFilter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK;
                        var listHisExpMest = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumer.ApiConsumers.MosConsumer, hisExpMestFilter, param);
                        if (listHisExpMest != null && listHisExpMest.Count > 0)
                        {
                            mps000234ADO.EXP_MEST_CODE = listHisExpMest.FirstOrDefault().EXP_MEST_CODE;
                            mps000234ADO.NUMBER_ORDER_OF_DAY = listHisExpMest.FirstOrDefault().NUM_ORDER.ToString();
                        }
                    }

                    if (hisTreatment != null)
                    {
                        CommonParam param = new CommonParam();
                        HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                        treatmentFilter.ID = hisTreatment.ID;
                        var listTreatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumer.ApiConsumers.MosConsumer, treatmentFilter, param);
                        if (listTreatment != null && listTreatment.Count > 0)
                        {
                            mps000234ADO.HisTreatment = listTreatment.FirstOrDefault();
                        }
                    }

                    var executeDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => exeDepaIds.Contains(o.ID)).ToList();
                    var requestDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => reqDepaIds.Contains(o.ID)).ToList();
                    mps000234ADO.EXECUTE_PHONE = executeDepartment != null && executeDepartment.Count > 0 ? string.Join(",", executeDepartment.Select(s => s.PHONE).ToList()) : "";
                    mps000234ADO.REQUEST_PHONE = requestDepartment != null && requestDepartment.Count > 0 ? string.Join(",", requestDepartment.Select(s => s.PHONE).ToList()) : "";

                    var acsUser = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME == HisPrescriptionSDOPrintPlus.REQUEST_LOGINNAME);
                    mps000234ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                    Inventec.Common.Logging.LogSystem.Debug("ConfigApplicationWorker.Get<string>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__PrintSplitTemplateMps000234Option)" + ConfigApplicationWorker.Get<string>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__PrintSplitTemplateMps000234Option));
                    if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__PrintSplitTemplateMps000234Option) == "1")
                    {
                        #region GN
                        foreach (var itemN in listGayNghien234)
                        {
                            ExpMestMedicineSDO ado = new ExpMestMedicineSDO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMedicineSDO>(ado, itemN);
                            listGayNghien.Add(ado);
                        }
                        if (listGayNghien.Count > 0)
                            InGayNghien();
                        listGayNghien = new List<ExpMestMedicineSDO>();
                        foreach (var itemN in listOutStockGN234)
                        {
                            if (itemN.Type == 3)
                            {
                                ExpMestMedicineSDO ado = new ExpMestMedicineSDO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMedicineSDO>(ado, itemN);
                                listGayNghien.Add(ado);
                            }
                        }
                        if (listGayNghien.Count > 0)
                            InGayNghien();
                        #endregion

                        #region HT
                        foreach (var itemN in listHuongThan234)
                        {
                            ExpMestMedicineSDO ado = new ExpMestMedicineSDO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMedicineSDO>(ado, itemN);
                            listHuongThan.Add(ado);
                        }
                        if (listHuongThan.Count > 0)
                            InHuongThan();
                        listHuongThan = new List<ExpMestMedicineSDO>();
                        foreach (var itemN in listOutStockHT234)
                        {
                            if (itemN.Type == 3)
                            {
                                ExpMestMedicineSDO ado = new ExpMestMedicineSDO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMedicineSDO>(ado, itemN);
                                listHuongThan.Add(ado);
                            }
                        }
                        if (listHuongThan.Count > 0)
                            InHuongThan();
                        #endregion
                    }
                    else
                    {

                        #region Gây nghiện
                        if ((listGayNghien234 != null && listGayNghien234.Count > 0) || (listOutStockGN234 != null && listOutStockGN234.Count > 0))
                        {
                            mps000234ADO.TITLE = "\"N\"";
                            MPS.Processor.Mps000234.PDO.Mps000234PDO mps000234RDO = new MPS.Processor.Mps000234.PDO.Mps000234PDO(
                                        vHisPatientTypeAlter,
                                        hisDhst,
                                        HisPrescriptionSDOPrintPlus,
                                        listGayNghien234,
                                        HisServiceReq_Exam,
                                        mps000234ADO,
                                        listReq,
                                        BackendDataWorker.Get<ACS_USER>().ToList(),
                                        BackendDataWorker.Get<V_HIS_ROOM>().ToList(),
                                        listOutStockGN234,
                                        hisServiceReq_CurentExam,
                                        hisListSereServClsTt);

                            Print.PrintData(printTypeCode, fileName, mps000234RDO, printNow, treatmentCode, ref result, this.currentModule != null ? currentModule.RoomId : 0, previewType, listOutStockGN234.Count, this.SavedData, numCopy);
                        }
                        #endregion

                        #region Hướng thần
                        if ((listHuongThan234 != null && listHuongThan234.Count > 0) || (listOutStockHT234 != null && listOutStockHT234.Count > 0))
                        {
                            mps000234ADO.TITLE = "\"H\"";
                            MPS.Processor.Mps000234.PDO.Mps000234PDO mps000234RDO = new MPS.Processor.Mps000234.PDO.Mps000234PDO(
                                        vHisPatientTypeAlter,
                                        hisDhst,
                                        HisPrescriptionSDOPrintPlus,
                                        listHuongThan234,
                                        HisServiceReq_Exam,
                                        mps000234ADO,
                                        listReq,
                                        BackendDataWorker.Get<ACS_USER>().ToList(),
                                        BackendDataWorker.Get<V_HIS_ROOM>().ToList(),
                                        listOutStockHT234,
                                        hisServiceReq_CurentExam,
                                        hisListSereServClsTt);

                            Print.PrintData(printTypeCode, fileName, mps000234RDO, printNow, treatmentCode, ref result, this.currentModule != null ? currentModule.RoomId : 0, previewType, listOutStockHT234.Count, this.SavedData, numCopy);
                        }
                        #endregion
                    }

                    #region Thuốc có chứa dược chất gây nghiện
                    if ((listHCGN234 != null && listHCGN234.Count > 0) || (listOutStockHCGN234 != null && listOutStockHCGN234.Count > 0))
                    {
                        mps000234ADO.TITLE = "Thuốc có chứa dược chất gây nghiện";
                        MPS.Processor.Mps000234.PDO.Mps000234PDO mps000234RDO = new MPS.Processor.Mps000234.PDO.Mps000234PDO(
                                    vHisPatientTypeAlter,
                                    hisDhst,
                                    HisPrescriptionSDOPrintPlus,
                                    listHCGN234,
                                    HisServiceReq_Exam,
                                    mps000234ADO,
                                    listReq,
                                    BackendDataWorker.Get<ACS_USER>().ToList(),
                                    BackendDataWorker.Get<V_HIS_ROOM>().ToList(),
                                    listOutStockGN234,
                                    hisServiceReq_CurentExam,
                                    hisListSereServClsTt);

                        Print.PrintData(printTypeCode, fileName, mps000234RDO, printNow, treatmentCode, ref result, this.currentModule != null ? currentModule.RoomId : 0, previewType, listOutStockHCGN234.Count, this.SavedData, numCopy);
                    }
                    #endregion

                    #region Thuốc có chứa dược chất hướng thần
                    if ((listHCHT234 != null && listHCHT234.Count > 0) || (listOutStockHCHT234 != null && listOutStockHCHT234.Count > 0))
                    {
                        mps000234ADO.TITLE = "Thuốc có chứa dược chất hướng thần";
                        MPS.Processor.Mps000234.PDO.Mps000234PDO mps000234RDO = new MPS.Processor.Mps000234.PDO.Mps000234PDO(
                                    vHisPatientTypeAlter,
                                    hisDhst,
                                    HisPrescriptionSDOPrintPlus,
                                    listHCHT234,
                                    HisServiceReq_Exam,
                                    mps000234ADO,
                                    listReq,
                                    BackendDataWorker.Get<ACS_USER>().ToList(),
                                    BackendDataWorker.Get<V_HIS_ROOM>().ToList(),
                                    listOutStockHCHT234,
                                    hisServiceReq_CurentExam,
                                    hisListSereServClsTt);

                        Print.PrintData(printTypeCode, fileName, mps000234RDO, printNow, treatmentCode, ref result, this.currentModule != null ? currentModule.RoomId : 0, previewType, listOutStockHCHT234.Count, this.SavedData, numCopy);
                    }
                    #endregion

                    #region không in tách hoặc chỉ in các thuốc k phải GN, HT
                    //không có đơn thuốc sẽ gán dữ liệu với ID = -1 để in
                    if ((ExpMestMedicineSDO != null && ExpMestMedicineSDO.Count > 0)
                        || (ExpMestMedicineSDOIncludeOutStock != null && ExpMestMedicineSDOIncludeOutStock.Count > 0)
                        || (ExpMests.Count == 1 && ExpMests.First().ID == -1 && HisPrescriptionSDOPrintPlus.ID == -1))
                    {
                        mps000234ADO.TITLE = "";
                        MPS.Processor.Mps000234.PDO.Mps000234PDO mps000234RDO = new MPS.Processor.Mps000234.PDO.Mps000234PDO(
                                    vHisPatientTypeAlter,
                                    hisDhst,
                                    HisPrescriptionSDOPrintPlus,
                                    ExpMestMedicineSDO,
                                    HisServiceReq_Exam,
                                    mps000234ADO,
                                    listReq,
                                    BackendDataWorker.Get<ACS_USER>().ToList(),
                                    BackendDataWorker.Get<V_HIS_ROOM>().ToList(),
                                    ExpMestMedicineSDOIncludeOutStock,
                                    hisServiceReq_CurentExam,
                                    hisListSereServClsTt);

                        Print.PrintData(printTypeCode, fileName, mps000234RDO, printNow, treatmentCode, ref result, this.currentModule != null ? currentModule.RoomId : 0, previewType, ExpMestMedicineSDOIncludeOutStock.Count, this.SavedData, numCopy);
                        //PrintData(printTypeCode, fileName, mps000234RDO, printNow, numCopy, ref result);
                    }
                    #endregion

                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InGayNghien()
        {
            richEditorMain.RunPrintTemplate(MPS.Processor.Mps000181.PDO.Mps000181PDO.PrintTypeCode, DelegateRunPrinter);
        }

        private void InHuongThan()
        {
            richEditorMain.RunPrintTemplate(MPS.Processor.Mps000192.PDO.Mps000192PDO.PrintTypeCode, DelegateRunPrinter);
        }

        private void InThucPham()
        {
            richEditorMain.RunPrintTemplate(MPS.Processor.Mps000191.PDO.Mps000191PDO.PrintTypeCode, DelegateRunPrinter);
        }

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case MPS.Processor.Mps000181.PDO.Mps000181PDO.PrintTypeCode:
                        DelegatePrintGNHT(printTypeCode, fileName, ref result);
                        break;
                    case MPS.Processor.Mps000191.PDO.Mps000191PDO.PrintTypeCode:
                        DelegatePrintTPCN(printTypeCode, fileName, ref result);
                        break;
                    case MPS.Processor.Mps000192.PDO.Mps000192PDO.PrintTypeCode:
                        DelegatePrintHT(printTypeCode, fileName, ref result);
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

        private void DelegatePrintGNHT(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (listGayNghien != null && listGayNghien.Count > 0)
                {
                    //Lọc thuốc theo thứ tự
                    listGayNghien = listGayNghien.OrderBy(o => o.NUM_ORDER ?? 99999).ToList();
                    List<MPS.Processor.Mps000181.PDO.ExpMestMedicineSDO> listN = new List<MPS.Processor.Mps000181.PDO.ExpMestMedicineSDO>();
                    foreach (var itemN in listGayNghien)
                    {
                        MPS.Processor.Mps000181.PDO.ExpMestMedicineSDO ado = new MPS.Processor.Mps000181.PDO.ExpMestMedicineSDO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000181.PDO.ExpMestMedicineSDO>(ado, itemN);
                        listN.Add(ado);
                    }
                    MPS.Processor.Mps000181.PDO.Mps000181ADO Mps000181ADO = new MPS.Processor.Mps000181.PDO.Mps000181ADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000181.PDO.Mps000181ADO>(Mps000181ADO, vHisPatient);
                    Mps000181ADO.EXECUTE_DEPARTMENT_NAME = executeRoom != null ? executeRoom.DEPARTMENT_NAME : "";
                    Mps000181ADO.EXECUTE_ROOM_NAME = executeRoom != null ? executeRoom.ROOM_NAME : "";
                    Mps000181ADO.EXP_MEST_CODE = expMestCode;
                    Mps000181ADO.REQUEST_DEPARTMENT_NAME = reqRoom != null ? reqRoom.DEPARTMENT_NAME : "";
                    Mps000181ADO.REQUEST_ROOM_NAME = reqRoom != null ? reqRoom.ROOM_NAME : "";
                    Mps000181ADO.MEDI_STOCK_NAME = mediStockName;
                    Mps000181ADO.TITLE = "\"N\"";

                    var acsUser = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME == HisPrescriptionSDOPrintPlus.REQUEST_LOGINNAME);
                    Mps000181ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                    MPS.Processor.Mps000181.PDO.Mps000181PDO mps000181PDO = new MPS.Processor.Mps000181.PDO.Mps000181PDO(
                        vHisPatientTypeAlter,
                        hisDhst,
                        HisPrescriptionSDOPrintPlus,
                        listN,
                        HisServiceReq_Exam,
                        hisTreatment,
                        Mps000181ADO,
                        null,
                        keyOrderListData);

                    Print.PrintData(printTypeCode, fileName, mps000181PDO, printNow, treatmentCode, ref result, this.currentModule != null ? currentModule.RoomId : 0, previewType, listGayNghien.Count, this.SavedData, numCopy);
                    //PrintData(printTypeCode, fileName, mps000181PDO, printNow, numCopy, ref result);
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DelegatePrintTPCN(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (listTPCN != null && listTPCN.Count > 0)
                {
                    List<MPS.Processor.Mps000191.PDO.ExpMestMedicineSDO> listTP = new List<MPS.Processor.Mps000191.PDO.ExpMestMedicineSDO>();
                    foreach (var item in listTPCN)
                    {
                        MPS.Processor.Mps000191.PDO.ExpMestMedicineSDO sdo = new MPS.Processor.Mps000191.PDO.ExpMestMedicineSDO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000191.PDO.ExpMestMedicineSDO>(sdo, item);
                        listTP.Add(sdo);
                    }

                    MPS.Processor.Mps000191.PDO.Mps000191ADO mps000191ADO = new MPS.Processor.Mps000191.PDO.Mps000191ADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000191.PDO.Mps000191ADO>(mps000191ADO, vHisPatient);
                    mps000191ADO.EXECUTE_DEPARTMENT_NAME = executeRoom != null ? executeRoom.DEPARTMENT_NAME : "";
                    mps000191ADO.EXECUTE_ROOM_NAME = executeRoom != null ? executeRoom.ROOM_NAME : "";
                    mps000191ADO.EXP_MEST_CODE = expMestCode;
                    mps000191ADO.REQUEST_DEPARTMENT_NAME = reqRoom != null ? reqRoom.DEPARTMENT_NAME : "";
                    mps000191ADO.REQUEST_ROOM_NAME = reqRoom != null ? reqRoom.ROOM_NAME : "";
                    mps000191ADO.MEDI_STOCK_NAME = mediStockName;
                    mps000191ADO.TITLE = "\"TP\"";

                    var acsUser = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME == HisPrescriptionSDOPrintPlus.REQUEST_LOGINNAME);
                    mps000191ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                    MPS.Processor.Mps000191.PDO.Mps000191PDO mps000191PDO = new MPS.Processor.Mps000191.PDO.Mps000191PDO(
                        vHisPatientTypeAlter,
                        hisDhst,
                        HisPrescriptionSDOPrintPlus,
                        listTP,
                        HisServiceReq_Exam,
                        hisTreatment,
                        mps000191ADO,
                        null,
                        keyOrderListData);

                    Print.PrintData(printTypeCode, fileName, mps000191PDO, printNow, treatmentCode, ref result, this.currentModule != null ? currentModule.RoomId : 0, previewType, listTPCN.Count, this.SavedData, numCopy);
                    //PrintData(printTypeCode, fileName, mps000191PDO, printNow, 1, ref result);
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DelegatePrintHT(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (listHuongThan != null && listHuongThan.Count > 0)
                {
                    List<MPS.Processor.Mps000192.PDO.ExpMestMedicineSDO> list = new List<MPS.Processor.Mps000192.PDO.ExpMestMedicineSDO>();
                    foreach (var item in listHuongThan)
                    {
                        MPS.Processor.Mps000192.PDO.ExpMestMedicineSDO sdo = new MPS.Processor.Mps000192.PDO.ExpMestMedicineSDO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000192.PDO.ExpMestMedicineSDO>(sdo, item);
                        list.Add(sdo);
                    }

                    MPS.Processor.Mps000192.PDO.Mps000192ADO mps000192ADO = new MPS.Processor.Mps000192.PDO.Mps000192ADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000192.PDO.Mps000192ADO>(mps000192ADO, vHisPatient);
                    mps000192ADO.EXECUTE_DEPARTMENT_NAME = executeRoom != null ? executeRoom.DEPARTMENT_NAME : "";
                    mps000192ADO.EXECUTE_ROOM_NAME = executeRoom != null ? executeRoom.ROOM_NAME : "";
                    mps000192ADO.EXP_MEST_CODE = expMestCode;
                    mps000192ADO.REQUEST_DEPARTMENT_NAME = reqRoom != null ? reqRoom.DEPARTMENT_NAME : "";
                    mps000192ADO.REQUEST_ROOM_NAME = reqRoom != null ? reqRoom.ROOM_NAME : "";
                    mps000192ADO.MEDI_STOCK_NAME = mediStockName;
                    mps000192ADO.TITLE = "\"H\"";

                    var acsUser = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME == HisPrescriptionSDOPrintPlus.REQUEST_LOGINNAME);
                    mps000192ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                    MPS.Processor.Mps000192.PDO.Mps000192PDO mps000192PDO = new MPS.Processor.Mps000192.PDO.Mps000192PDO(
                        vHisPatientTypeAlter,
                        hisDhst,
                        HisPrescriptionSDOPrintPlus,
                        list,
                        HisServiceReq_Exam,
                        hisTreatment,
                        mps000192ADO,
                        null,
                        keyOrderListData);

                    Print.PrintData(printTypeCode, fileName, mps000192PDO, printNow, treatmentCode, ref result, this.currentModule != null ? currentModule.RoomId : 0, previewType, listHuongThan.Count, this.SavedData, numCopy);
                    //PrintData(printTypeCode, fileName, mps000192PDO, printNow, numCopy, ref result);
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Thread
        private void CreateThreadGetCurrentData(HIS_SERVICE_REQ HisPrescriptionResultSDOs)
        {
            if (HisPrescriptionResultSDOs != null)
            {
                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Config.KEY_IsPrintPrescriptionNoThread) == "1")
                {
                    GetDataPatientTypeDhst(HisPrescriptionResultSDOs);
                    GetDataExam(HisPrescriptionResultSDOs);
                    GetPatient(HisPrescriptionResultSDOs);
                    GetTreatment(HisPrescriptionResultSDOs);
                }
                else
                {
                    Thread patientType = new Thread(GetDataPatientTypeDhst);
                    Thread exam = new Thread(GetDataExam);
                    Thread patient = new Thread(GetPatient);
                    Thread treatment = new Thread(GetTreatment);
                    try
                    {
                        patientType.Start(HisPrescriptionResultSDOs);
                        exam.Start(HisPrescriptionResultSDOs);
                        patient.Start(HisPrescriptionResultSDOs);
                        treatment.Start(HisPrescriptionResultSDOs);

                        patientType.Join();
                        exam.Join();
                        patient.Join();
                        treatment.Join();
                    }
                    catch (Exception ex)
                    {
                        patientType.Abort();
                        exam.Abort();
                        patient.Abort();
                        treatment.Abort();
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }
            }
        }

        private void GetPatient(object obj)
        {
            try
            {
                if (obj != null)
                {
                    HIS_SERVICE_REQ data = (HIS_SERVICE_REQ)obj;
                    CommonParam paramCommon = new CommonParam();
                    HisPatientViewFilter filter = new HisPatientViewFilter() { PATIENT_CODE = data.TDL_PATIENT_CODE };
                    var lstpatient = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_PATIENT>>(RequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);

                    if (lstpatient != null && lstpatient.Count > 0)
                    {
                        vHisPatient = lstpatient.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataExam(object obj)
        {
            try
            {
                if (obj != null)
                {
                    HIS_SERVICE_REQ data = (HIS_SERVICE_REQ)obj;
                    HisServiceReqFilter serviceReqfilter = new HisServiceReqFilter();
                    serviceReqfilter.TREATMENT_ID = data.TREATMENT_ID;
                    serviceReqfilter.SERVICE_REQ_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH };
                    var lstexamServiceReq = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, serviceReqfilter, null);

                    if (lstexamServiceReq != null && lstexamServiceReq.Count > 0)
                    {
                        var lstHisServiceReq_CurentExam = lstexamServiceReq.Where(o => o.EXECUTE_ROOM_ID == currentModule.RoomId);
                        if (lstHisServiceReq_CurentExam != null && lstHisServiceReq_CurentExam.Count() > 0)
                        {
                            hisServiceReq_CurentExam = lstHisServiceReq_CurentExam.First();
                        }

                        if (lstexamServiceReq.Count == 1)
                        {
                            HisServiceReq_Exam = lstexamServiceReq.First();
                        }
                        else
                        {
                            List<HIS_SERVICE_REQ> lstExit = new List<HIS_SERVICE_REQ>();
                            foreach (var item in lstexamServiceReq)
                            {
                                if (item.EXECUTE_ROOM_ID == data.REQUEST_ROOM_ID && (!String.IsNullOrEmpty(item.FULL_EXAM) || !String.IsNullOrEmpty(item.PATHOLOGICAL_PROCESS)))
                                {
                                    lstExit.Add(item);
                                }
                            }

                            if (lstExit.Count > 0)
                            {

                                if (lstExit.Count == 1)
                                {
                                    HisServiceReq_Exam = lstExit.First();
                                }
                                else
                                {
                                    HisServiceReq_Exam = lstExit.OrderByDescending(o => o.INTRUCTION_TIME).ThenByDescending(o => o.MODIFY_TIME).FirstOrDefault();
                                }
                            }
                            else
                            {
                                HisServiceReq_Exam = lstexamServiceReq.OrderByDescending(o => o.INTRUCTION_TIME).ThenByDescending(o => o.MODIFY_TIME).FirstOrDefault();
                            }
                        }
                    }
                    if (HisConfigs.Get<string>(Config.TAKE_INFO_TT_CLS) == "1")
                    {
                        List<long> cls_tt = new List<long>() {
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT,
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA,
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS,
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA };

                        CommonParam paramCommon = new CommonParam();
                        HisServiceReqFilter reqchildFilter = new HisServiceReqFilter();
                        reqchildFilter.PARENT_ID = HisServiceReq_Exam.ID;
                        var lstServiceReqChild = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_SERVICE_REQ>>(RequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumer.ApiConsumers.MosConsumer, reqchildFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                        if (lstServiceReqChild != null && lstServiceReqChild.Count > 0)
                        {
                            lstServiceReqChild = lstServiceReqChild.Where(o => cls_tt.Contains(o.SERVICE_REQ_TYPE_ID)).ToList();
                            if (lstServiceReqChild != null && lstServiceReqChild.Count > 0)
                            {
                                HisSereServFilter ssFilter = new HisSereServFilter();
                                ssFilter.SERVICE_REQ_IDs = lstServiceReqChild.Select(s => s.ID).ToList();
                                hisListSereServClsTt = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumer.ApiConsumers.MosConsumer, ssFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
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

        private void GetDataPatientTypeDhst(object obj)
        {
            try
            {
                if (obj != null)
                {
                    HIS_SERVICE_REQ data = (HIS_SERVICE_REQ)obj;

                    CommonParam param = new CommonParam();

                    //Lấy thông tin thẻ BHYT
                    HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
                    filter.TreatmentId = data.TREATMENT_ID;
                    filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                    vHisPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(RequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    //Dấu hiệu sinh tồn
                    MOS.Filter.HisDhstFilter hisDhstFilter = new HisDhstFilter();
                    hisDhstFilter.TREATMENT_ID = data.TREATMENT_ID;
                    var dhsts = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_DHST>>(RequestUriStore.HIS_DHST_GET, ApiConsumer.ApiConsumers.MosConsumer, hisDhstFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (dhsts != null && dhsts.Count > 0)
                    {
                        //hisDhst = dhsts.FirstOrDefault(o => o.EXECUTE_TIME <= hIS_SERVICE_REQ.INTRUCTION_TIME);
                        //hisDhst = dhsts.FirstOrDefault();
                        //in luôn lúc chỉ định không có thời gian tạo
                        long dateTimeNow = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 99999999999999;
                        //lấy dhst do ng kê đơn thực hiện. nếu ko có lấy dhst gần với thời gian kê đơn nhất
                        var dhstExecute = dhsts.Where(o => o.CREATE_TIME <= (data.CREATE_TIME ?? dateTimeNow) && o.EXECUTE_LOGINNAME == data.REQUEST_LOGINNAME).ToList();
                        if (dhstExecute != null && dhstExecute.Count > 0)
                        {
                            hisDhst = dhstExecute.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault();
                        }
                        else
                            hisDhst = dhsts.OrderByDescending(o => o.ID).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetTreatment(object obj)
        {
            try
            {
                if (obj != null)
                {
                    HIS_SERVICE_REQ data = (HIS_SERVICE_REQ)obj;

                    CommonParam paramCommon = new CommonParam();
                    HisTreatmentFilter filter = new HisTreatmentFilter() { ID = data.TREATMENT_ID };
                    var lstTreatment = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_TREATMENT>>(RequestUriStore.HIS_TREATMENT_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    if (lstTreatment != null && lstTreatment.Count > 0)
                    {
                        hisTreatment = lstTreatment.FirstOrDefault();
                        treatmentCode = hisTreatment.TREATMENT_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
    }
}
