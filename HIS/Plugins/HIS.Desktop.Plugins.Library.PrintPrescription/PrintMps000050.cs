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
using HIS.Desktop.LocalStorage.ConfigSystem;
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
    class PrintMps000050
    {
        private V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter;
        private HIS_DHST hisDhst;
        private V_HIS_PATIENT vHisPatient;
        private HIS_SERVICE_REQ HisServiceReq_Exam;
        private short IS_TRUE = 1;
        private HIS_TREATMENT hisTreatment;
        string treatmentCode = "";
        private Inventec.Desktop.Common.Modules.Module currentModule;
        MPS.ProcessorBase.PrintConfig.PreviewType? previewType;
        List<ExpMestMedicineSDO> listSPHoTro = null;
        Inventec.Common.RichEditor.RichEditorStore richEditorMain;
        HIS_SERVICE_REQ HisPrescriptionSDOPrintPlus = null;
        bool printNow;
        private string expMestCode;
        string mediStockName = "";
        private const int VatTu = 2;
        private const int VatTuNgoaiKho = 4;
        long? keyOrderListData;

        Action<int, Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> SavedData;
        Action<string> CancelPrint;

        public PrintMps000050(string printTypeCode, string fileName, ref bool result,
            MOS.SDO.OutPatientPresResultSDO currentOutPresSDO,
            bool printNow, bool hasMediMate, Inventec.Desktop.Common.Modules.Module module,
            Inventec.Common.RichEditor.RichEditorStore _richEditorMain,
            MPS.ProcessorBase.PrintConfig.PreviewType? previewType,
            Action<int> countData,
            Action<int, Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> savedData,
            Action<string> cancelPrint)
        {
            try
            {
                this.printNow = printNow;
                this.richEditorMain = _richEditorMain;
                this.currentModule = module;
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
                    }

                    if (ExpMests == null || HisPrescriptionSDOPrintPlus == null)
                    {
                        result = false;
                        return;
                    }

                    if (HisPrescriptionSDOPrintPlus.USE_TIME == null)
                    {
                        HisPrescriptionSDOPrintPlus.USE_TIME = HisPrescriptionSDOPrintPlus.INTRUCTION_TIME;
                    }

                    //Lấy thông tin thẻ BHYT
                    HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
                    filter.TreatmentId = HisPrescriptionSDOPrintPlus.TREATMENT_ID;
                    filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                    var vHisPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(RequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);

                    new ThreadLoadDataMediMate(mediMateIn, mediMateOut, hisTreatment);

                    if (mediMateIn != null && mediMateIn.DicLstMediMateExpMestTypeADO != null && countData != null)
                    {
                        countData(mediMateIn.DicLstMediMateExpMestTypeADO.Values.SelectMany(s => s).Count());
                    }

                    if (mediMateOut != null && mediMateOut.DicLstMediMateExpMestTypeADO != null && countData != null)
                    {
                        countData(mediMateOut.DicLstMediMateExpMestTypeADO.Values.SelectMany(s => s).Count());
                    }

                    treatmentCode = (hisTreatment != null ? hisTreatment.TREATMENT_CODE : "");

                    foreach (var item in ExpMests)
                    {
                        expMestCode = item.EXP_MEST_CODE;
                        //Thong tin thuoc / vat tu
                        List<ExpMestMedicineSDO> lstMedicineExpmestTypeADO = new List<ExpMestMedicineSDO>();
                        List<ExpMestMedicineSDO> lstMedicineExpmestTypeTuTucADO = new List<ExpMestMedicineSDO>();
                        List<ExpMestMedicineSDO> lstMedicineExpmestTypeThuocHuongThanADO = new List<ExpMestMedicineSDO>();

                        var listGayNghien = new List<ExpMestMedicineSDO>();
                        var listHuongThan = new List<ExpMestMedicineSDO>();
                        var listTPCN = new List<ExpMestMedicineSDO>();
                        listSPHoTro = new List<ExpMestMedicineSDO>();

                        if (mediMateIn.DicLstMediMateExpMestTypeADO.ContainsKey(item.ID) &&
                            mediMateIn.DicLstMediMateExpMestTypeADO[item.ID] != null &&
                            mediMateIn.DicLstMediMateExpMestTypeADO[item.ID].Count > 0)
                        {
                            lstMedicineExpmestTypeADO.AddRange(mediMateIn.DicLstMediMateExpMestTypeADO[item.ID]);
                        }

                        if (mediMateOut.DicLstMediMateExpMestTypeADO.ContainsKey(item.SERVICE_REQ_ID ?? 0))
                        {
                            lstMedicineExpmestTypeTuTucADO.AddRange(mediMateOut.DicLstMediMateExpMestTypeADO[item.SERVICE_REQ_ID ?? 0]);
                        }

                        //Thuoc tong hop
                        if (lstMedicineExpmestTypeTuTucADO != null && lstMedicineExpmestTypeTuTucADO.Count > 0 && lstMedicineExpmestTypeADO != null)
                        {
                            lstMedicineExpmestTypeADO.AddRange(lstMedicineExpmestTypeTuTucADO);
                        }

                        long keyPrintType = Convert.ToInt64(ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__CHE_DO_IN_TACH_DON_THUOC));

                        if (keyPrintType == 0)
                        {
                            //Danh sach thuoc huong than,gây nghiện, 
                            lstMedicineExpmestTypeThuocHuongThanADO = lstMedicineExpmestTypeADO.Where(o =>
                                o.IS_NEUROLOGICAL == IS_TRUE || o.IS_ADDICTIVE == IS_TRUE ||
                                o.IS_FUNCTIONAL_FOOD == IS_TRUE
                                ).ToList();

                            listHuongThan = lstMedicineExpmestTypeADO.Where(o => o.IS_NEUROLOGICAL == IS_TRUE).ToList();
                            listGayNghien = lstMedicineExpmestTypeADO.Where(o => o.IS_ADDICTIVE == IS_TRUE).ToList();
                            listTPCN = lstMedicineExpmestTypeADO.Where(o => o.IS_FUNCTIONAL_FOOD == IS_TRUE).ToList();

                            //Bo thuoc huong than gay nghien ra khoi danh sach thuoc tong hop
                            if (lstMedicineExpmestTypeThuocHuongThanADO != null && lstMedicineExpmestTypeThuocHuongThanADO.Count > 0)
                            {
                                lstMedicineExpmestTypeADO = lstMedicineExpmestTypeADO.Except(lstMedicineExpmestTypeThuocHuongThanADO).ToList();
                            }
                        }
                        else if (keyPrintType == 2)// nambg issue #23730
                        {
                            //Danh sach thuoc huong than,gây nghiện, 
                            lstMedicineExpmestTypeThuocHuongThanADO = lstMedicineExpmestTypeADO.Where(o =>
                                o.IS_NEUROLOGICAL == IS_TRUE || o.IS_ADDICTIVE == IS_TRUE
                                ).ToList();

                            listHuongThan = lstMedicineExpmestTypeADO.Where(o => o.IS_NEUROLOGICAL == IS_TRUE).ToList();
                            listGayNghien = lstMedicineExpmestTypeADO.Where(o => o.IS_ADDICTIVE == IS_TRUE).ToList();
                            listSPHoTro = lstMedicineExpmestTypeADO.Where(o => o.IS_FUNCTIONAL_FOOD == IS_TRUE || o.Type == VatTu || o.Type == VatTuNgoaiKho).ToList();

                            //Bo thuoc huong than gay nghien ra khoi danh sach thuoc tong hop, ho tro
                            if (lstMedicineExpmestTypeThuocHuongThanADO != null && lstMedicineExpmestTypeThuocHuongThanADO.Count > 0)
                            {
                                lstMedicineExpmestTypeADO = lstMedicineExpmestTypeADO.Except(lstMedicineExpmestTypeThuocHuongThanADO).ToList();
                                listSPHoTro = ((listSPHoTro != null && listSPHoTro.Count() > 0) ? listSPHoTro.Except(lstMedicineExpmestTypeThuocHuongThanADO).ToList() : listSPHoTro);
                            }

                            // bo san pham ho tro don thuoc thuong
                            if (listSPHoTro != null && listSPHoTro.Count() > 0 && lstMedicineExpmestTypeADO != null && lstMedicineExpmestTypeADO.Count() > 0)
                            {
                                lstMedicineExpmestTypeADO = lstMedicineExpmestTypeADO.Except(listSPHoTro).ToList();
                            }
                        }

                        if (currentOutPresSDO.ServiceReqs != null &&
                            currentOutPresSDO.ServiceReqs.Count > 0)
                        {
                            HisPrescriptionSDOPrintPlus = currentOutPresSDO.ServiceReqs.FirstOrDefault(o => o.ID == item.SERVICE_REQ_ID);
                        }

                        if (HisPrescriptionSDOPrintPlus == null) continue;
                        if (HisPrescriptionSDOPrintPlus.USE_TIME == null)
                        {
                            HisPrescriptionSDOPrintPlus.USE_TIME = HisPrescriptionSDOPrintPlus.INTRUCTION_TIME;
                        }

                        CreateThreadGetCurrentData(HisPrescriptionSDOPrintPlus);

                        var room = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == item.MEDI_STOCK_ID);
                        mediStockName = room != null ? room.MEDI_STOCK_NAME : "";
                        MPS.Processor.Mps000050.PDO.Mps000050ADO mps000050ADO = new MPS.Processor.Mps000050.PDO.Mps000050ADO();
                        mps000050ADO.MEDI_STOCK_NAME = mediStockName;
                        mps000050ADO.EXP_MEST_CODE = item.EXP_MEST_CODE;

                        var executeDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.EXECUTE_DEPARTMENT_ID);
                        var requestDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.REQUEST_DEPARTMENT_ID);
                        mps000050ADO.EXECUTE_PHONE = executeDepartment != null ? executeDepartment.PHONE : "";
                        mps000050ADO.REQUEST_PHONE = requestDepartment != null ? requestDepartment.PHONE : "";

                        var acsUser = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME == HisPrescriptionSDOPrintPlus.REQUEST_LOGINNAME);
                        mps000050ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                        #region thuong
                        //không có đơn thuốc sẽ gán dữ liệu với ID = -1 để in
                        if (lstMedicineExpmestTypeADO != null && lstMedicineExpmestTypeADO.Count > 0
                            || (ExpMests.Count == 1 && ExpMests.First().ID == -1 && HisPrescriptionSDOPrintPlus.ID == -1))
                        {
                            List<MPS.Processor.Mps000050.PDO.ExpMestMedicineSDO> ExeExpMestMedicineSDO = new List<MPS.Processor.Mps000050.PDO.ExpMestMedicineSDO>();
                            foreach (var medimate in lstMedicineExpmestTypeADO)
                            {
                                MPS.Processor.Mps000050.PDO.ExpMestMedicineSDO ado = new MPS.Processor.Mps000050.PDO.ExpMestMedicineSDO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000050.PDO.ExpMestMedicineSDO>(ado, medimate);
                                ExeExpMestMedicineSDO.Add(ado);
                            }

                            ExeExpMestMedicineSDO = ExeExpMestMedicineSDO.OrderBy(o => o.NUM_ORDER).ToList();

                            MPS.Processor.Mps000050.PDO.Mps000050PDO mps000050RDO = new MPS.Processor.Mps000050.PDO.Mps000050PDO(
                                vHisPatientTypeAlter,
                                HisPrescriptionSDOPrintPlus,
                                ExeExpMestMedicineSDO,
                                mps000050ADO,
                                hisTreatment,
                                keyOrderListData);

                            Print.PrintData(printTypeCode, fileName, mps000050RDO, printNow, treatmentCode, ref result, this.currentModule != null ? currentModule.RoomId : 0, previewType, lstMedicineExpmestTypeADO.Count, this.SavedData);
                            //PrintData(printTypeCode, fileName, mps000050RDO, printNow, ref result);
                        }
                        #endregion

                        #region HT
                        if (listHuongThan != null && listHuongThan.Count > 0)
                        {
                            List<MPS.Processor.Mps000050.PDO.ExpMestMedicineSDO> ExeExpMestMedicineSDO = new List<MPS.Processor.Mps000050.PDO.ExpMestMedicineSDO>();
                            foreach (var medimate in listHuongThan)
                            {
                                MPS.Processor.Mps000050.PDO.ExpMestMedicineSDO ado = new MPS.Processor.Mps000050.PDO.ExpMestMedicineSDO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000050.PDO.ExpMestMedicineSDO>(ado, medimate);
                                ExeExpMestMedicineSDO.Add(ado);
                            }

                            ExeExpMestMedicineSDO = ExeExpMestMedicineSDO.OrderBy(o => o.NUM_ORDER).ToList();

                            MPS.Processor.Mps000050.PDO.Mps000050PDO mps000050RDO = new MPS.Processor.Mps000050.PDO.Mps000050PDO(
                                vHisPatientTypeAlter,
                                HisPrescriptionSDOPrintPlus,
                                ExeExpMestMedicineSDO,
                                mps000050ADO,
                                hisTreatment,
                                keyOrderListData);

                            Print.PrintData(printTypeCode, fileName, mps000050RDO, printNow, treatmentCode, ref result, this.currentModule != null ? currentModule.RoomId : 0, previewType, listHuongThan.Count, this.SavedData);
                            //PrintData(printTypeCode, fileName, mps000050RDO, printNow, ref result);
                        }
                        #endregion

                        #region GN
                        if (listGayNghien != null && listGayNghien.Count > 0)
                        {
                            List<MPS.Processor.Mps000050.PDO.ExpMestMedicineSDO> ExeExpMestMedicineSDO = new List<MPS.Processor.Mps000050.PDO.ExpMestMedicineSDO>();
                            foreach (var medimate in listGayNghien)
                            {
                                MPS.Processor.Mps000050.PDO.ExpMestMedicineSDO ado = new MPS.Processor.Mps000050.PDO.ExpMestMedicineSDO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000050.PDO.ExpMestMedicineSDO>(ado, medimate);
                                ExeExpMestMedicineSDO.Add(ado);
                            }
                            ExeExpMestMedicineSDO = ExeExpMestMedicineSDO.OrderBy(o => o.NUM_ORDER).ToList();

                            MPS.Processor.Mps000050.PDO.Mps000050PDO mps000050RDO = new MPS.Processor.Mps000050.PDO.Mps000050PDO(
                                vHisPatientTypeAlter,
                                HisPrescriptionSDOPrintPlus,
                                ExeExpMestMedicineSDO,
                                mps000050ADO,
                                hisTreatment,
                                keyOrderListData);

                            Print.PrintData(printTypeCode, fileName, mps000050RDO, printNow, treatmentCode, ref result, this.currentModule != null ? currentModule.RoomId : 0, previewType, listGayNghien.Count, this.SavedData);
                            //PrintData(printTypeCode, fileName, mps000050RDO, printNow, ref result);
                        }
                        #endregion

                        #region TPCN
                        if (listTPCN != null && listTPCN.Count > 0)
                        {
                            List<MPS.Processor.Mps000050.PDO.ExpMestMedicineSDO> ExeExpMestMedicineSDO = new List<MPS.Processor.Mps000050.PDO.ExpMestMedicineSDO>();
                            foreach (var medimate in listTPCN)
                            {
                                MPS.Processor.Mps000050.PDO.ExpMestMedicineSDO ado = new MPS.Processor.Mps000050.PDO.ExpMestMedicineSDO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000050.PDO.ExpMestMedicineSDO>(ado, medimate);
                                ExeExpMestMedicineSDO.Add(ado);
                            }
                            ExeExpMestMedicineSDO = ExeExpMestMedicineSDO.OrderBy(o => o.NUM_ORDER).ToList();

                            MPS.Processor.Mps000050.PDO.Mps000050PDO mps000050RDO = new MPS.Processor.Mps000050.PDO.Mps000050PDO(
                                vHisPatientTypeAlter,
                                HisPrescriptionSDOPrintPlus,
                                ExeExpMestMedicineSDO,
                                mps000050ADO,
                                hisTreatment,
                                keyOrderListData);

                            Print.PrintData(printTypeCode, fileName, mps000050RDO, printNow, treatmentCode, ref result, this.currentModule != null ? currentModule.RoomId : 0, previewType, listTPCN.Count, this.SavedData);
                            //PrintData(printTypeCode, fileName, mps000050RDO, printNow, ref result);
                        }
                        #endregion

                        #region in san pham ho tro
                        if (listSPHoTro != null && listSPHoTro.Count > 0)
                        {
                            InSanPhamHoTro();
                        }
                        #endregion

                        #region PX
                        //if (listPX != null && listPX.Count > 0)
                        //{
                        //    List<MPS.Processor.Mps000050.PDO.ExpMestMedicineSDO> ExeExpMestMedicineSDO = new List<MPS.Processor.Mps000050.PDO.ExpMestMedicineSDO>();
                        //    foreach (var medimate in listPX)
                        //    {
                        //        MPS.Processor.Mps000050.PDO.ExpMestMedicineSDO ado = new MPS.Processor.Mps000050.PDO.ExpMestMedicineSDO();
                        //        Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000050.PDO.ExpMestMedicineSDO>(ado, medimate);
                        //        ExeExpMestMedicineSDO.Add(ado);
                        //    }
                        //    ExeExpMestMedicineSDO = ExeExpMestMedicineSDO.OrderBy(o => o.NUM_ORDER).ToList();

                        //    MPS.Processor.Mps000050.PDO.Mps000050PDO mps000050RDO = new MPS.Processor.Mps000050.PDO.Mps000050PDO(
                        //        vHisPatientTypeAlter,
                        //        HisPrescriptionSDOPrintPlus,
                        //        ExeExpMestMedicineSDO,
                        //        mps000050ADO,
                        //        hisTreatment);

                        //    PrintData(printTypeCode, fileName, mps000050RDO, printNow, ref result);
                        //}
                        #endregion

                        #region doc
                        //if (listD != null && listD.Count > 0)
                        //{
                        //    List<MPS.Processor.Mps000050.PDO.ExpMestMedicineSDO> ExeExpMestMedicineSDO = new List<MPS.Processor.Mps000050.PDO.ExpMestMedicineSDO>();
                        //    foreach (var medimate in listD)
                        //    {
                        //        MPS.Processor.Mps000050.PDO.ExpMestMedicineSDO ado = new MPS.Processor.Mps000050.PDO.ExpMestMedicineSDO();
                        //        Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000050.PDO.ExpMestMedicineSDO>(ado, medimate);
                        //        ExeExpMestMedicineSDO.Add(ado);
                        //    }
                        //    ExeExpMestMedicineSDO = ExeExpMestMedicineSDO.OrderBy(o => o.NUM_ORDER).ToList();

                        //    MPS.Processor.Mps000050.PDO.Mps000050PDO mps000050RDO = new MPS.Processor.Mps000050.PDO.Mps000050PDO(
                        //        vHisPatientTypeAlter,
                        //        HisPrescriptionSDOPrintPlus,
                        //        ExeExpMestMedicineSDO,
                        //        mps000050ADO,
                        //        hisTreatment);

                        //    PrintData(printTypeCode, fileName, mps000050RDO, printNow, ref result);
                        //}
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InSanPhamHoTro()
        {
            richEditorMain.RunPrintTemplate(MPS.Processor.Mps000353.PDO.Mps000353PDO.PrintTypeCode, DelegateRunPrinter);
        }

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (listSPHoTro != null && listSPHoTro.Count > 0)
                {
                    List<MPS.Processor.Mps000353.PDO.ExpMestMedicineSDO> ExpMestMedicineSDO = new List<MPS.Processor.Mps000353.PDO.ExpMestMedicineSDO>();
                    var group = listSPHoTro.GroupBy(o => new { o.MEDICINE_TYPE_ID, o.MEDICINE_TYPE_NAME });
                    foreach (var aitem in group)
                    {
                        MPS.Processor.Mps000353.PDO.ExpMestMedicineSDO ado = new MPS.Processor.Mps000353.PDO.ExpMestMedicineSDO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000353.PDO.ExpMestMedicineSDO>(ado, aitem.First());
                        ado.AMOUNT = aitem.Sum(o => o.AMOUNT);
                        ExpMestMedicineSDO.Add(ado);
                    }
                    //Lọc thuốc theo thứ tự

                    ExpMestMedicineSDO = ExpMestMedicineSDO.OrderBy(o => o.NUM_ORDER ?? 99999).ToList();
                    V_HIS_ROOM executeRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.EXECUTE_ROOM_ID);
                    V_HIS_ROOM reqRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.REQUEST_ROOM_ID);

                    MPS.Processor.Mps000353.PDO.Mps000353ADO Mps000353ADO = new MPS.Processor.Mps000353.PDO.Mps000353ADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000353.PDO.Mps000353ADO>(Mps000353ADO, vHisPatient);
                    Mps000353ADO.EXECUTE_DEPARTMENT_NAME = executeRoom != null ? executeRoom.DEPARTMENT_NAME : "";
                    Mps000353ADO.EXECUTE_ROOM_NAME = executeRoom != null ? executeRoom.ROOM_NAME : "";
                    Mps000353ADO.EXP_MEST_CODE = expMestCode;
                    Mps000353ADO.REQUEST_DEPARTMENT_NAME = reqRoom != null ? reqRoom.DEPARTMENT_NAME : "";
                    Mps000353ADO.REQUEST_ROOM_NAME = reqRoom != null ? reqRoom.ROOM_NAME : "";
                    Mps000353ADO.MEDI_STOCK_NAME = mediStockName;
                    Mps000353ADO.TITLE = "";

                    var executeDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.EXECUTE_DEPARTMENT_ID);
                    var requestDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.REQUEST_DEPARTMENT_ID);
                    Mps000353ADO.EXECUTE_PHONE = executeDepartment != null ? executeDepartment.PHONE : "";
                    Mps000353ADO.REQUEST_PHONE = requestDepartment != null ? requestDepartment.PHONE : "";

                    var acsUser = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME == HisPrescriptionSDOPrintPlus.REQUEST_LOGINNAME);
                    Mps000353ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                    MPS.Processor.Mps000353.PDO.Mps000353PDO Mps000353RDO = new MPS.Processor.Mps000353.PDO.Mps000353PDO(
                        vHisPatientTypeAlter,
                        hisDhst,
                        HisPrescriptionSDOPrintPlus,
                        ExpMestMedicineSDO,
                        HisServiceReq_Exam,
                        hisTreatment,
                        Mps000353ADO,
                        null,
                        keyOrderListData);

                    Print.PrintData(printTypeCode, fileName, Mps000353RDO, printNow, treatmentCode, ref result, this.currentModule != null ? currentModule.RoomId : 0, previewType, listSPHoTro.Count, this.SavedData);
                    //PrintData(printTypeCode, fileName, Mps000353RDO, printNow, ref result);
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void CreateThreadGetCurrentData(HIS_SERVICE_REQ HisPrescriptionSDOPrintPlus)
        {
            if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Config.KEY_IsPrintPrescriptionNoThread) == "1")
            {
                GetTreatment(HisPrescriptionSDOPrintPlus);
                GetDataPatientType(HisPrescriptionSDOPrintPlus);
                GetDataDhst(HisPrescriptionSDOPrintPlus);
                GetDataExam(HisPrescriptionSDOPrintPlus);
                GetPatient(HisPrescriptionSDOPrintPlus);
            }
            else
            {
                Thread treatment = new Thread(GetTreatment);
                Thread patientType = new Thread(GetDataPatientType);
                Thread dhst = new Thread(GetDataDhst);
                Thread exam = new Thread(GetDataExam);
                Thread patient = new Thread(GetPatient);

                try
                {
                    treatment.Start(HisPrescriptionSDOPrintPlus);
                    patientType.Start(HisPrescriptionSDOPrintPlus);
                    exam.Start(HisPrescriptionSDOPrintPlus);
                    dhst.Start(HisPrescriptionSDOPrintPlus);
                    patient.Start(HisPrescriptionSDOPrintPlus);

                    treatment.Join();
                    patientType.Join();
                    exam.Join();
                    dhst.Join();
                    patient.Join();
                }
                catch (Exception ex)
                {
                    patientType.Abort();
                    exam.Abort();
                    dhst.Abort();
                    patient.Abort();
                    treatment.Abort();
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
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

        private void GetDataExam(object obj)
        {
            try
            {
                if (obj != null)
                {
                    var hIS_SERVICE_REQ = obj as HIS_SERVICE_REQ;
                    HisServiceReqFilter serviceReqfilter = new HisServiceReqFilter();
                    serviceReqfilter.TREATMENT_ID = hIS_SERVICE_REQ.TREATMENT_ID;
                    serviceReqfilter.SERVICE_REQ_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH };
                    var lstexamServiceReq = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, serviceReqfilter, null);

                    if (lstexamServiceReq != null && lstexamServiceReq.Count > 0)
                    {
                        if (lstexamServiceReq.Count == 1)
                        {
                            HisServiceReq_Exam = lstexamServiceReq.First();
                        }
                        else
                        {
                            List<HIS_SERVICE_REQ> lstExit = new List<HIS_SERVICE_REQ>();
                            foreach (var item in lstexamServiceReq)
                            {
                                if (item.EXECUTE_ROOM_ID == hIS_SERVICE_REQ.REQUEST_ROOM_ID && (!String.IsNullOrEmpty(item.FULL_EXAM) || !String.IsNullOrEmpty(item.PATHOLOGICAL_PROCESS)))
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
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetPatient(object obj)
        {
            try
            {
                vHisPatient = new V_HIS_PATIENT();
                HIS_SERVICE_REQ data = (HIS_SERVICE_REQ)obj;
                if (data != null)
                {
                    if (!string.IsNullOrEmpty(data.TDL_PATIENT_CODE))
                    {
                        CommonParam paramCommon = new CommonParam();
                        HisPatientViewFilter filter = new HisPatientViewFilter() { PATIENT_CODE = data.TDL_PATIENT_CODE };
                        var lstpatient = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_PATIENT>>(RequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                        if (lstpatient != null && lstpatient.Count > 0)
                        {
                            vHisPatient = lstpatient.FirstOrDefault();
                        }
                    }
                    else
                    {
                        vHisPatient.FIRST_NAME = data.TDL_PATIENT_FIRST_NAME;
                        vHisPatient.LAST_NAME = data.TDL_PATIENT_LAST_NAME;
                        vHisPatient.PHONE = data.TDL_PATIENT_PHONE;
                        vHisPatient.ADDRESS = data.TDL_PATIENT_ADDRESS;
                        vHisPatient.DOB = data.TDL_PATIENT_DOB;
                        vHisPatient.GENDER_NAME = data.TDL_PATIENT_GENDER_NAME;
                        vHisPatient.VIR_PATIENT_NAME = data.TDL_PATIENT_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataDhst(object obj)
        {
            try
            {
                if (obj != null)
                {
                    var hIS_SERVICE_REQ = obj as HIS_SERVICE_REQ;
                    //Dấu hiệu sinh tồn
                    HisDhstFilter hisDhstFilter = new HisDhstFilter();
                    hisDhstFilter.TREATMENT_ID = hIS_SERVICE_REQ.TREATMENT_ID;
                    var dhsts = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_DHST>>(RequestUriStore.HIS_DHST_GET, ApiConsumer.ApiConsumers.MosConsumer, hisDhstFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (dhsts != null && dhsts.Count > 0)
                    {
                        //in luôn lúc chỉ định không có thời gian tạo
                        long dateTimeNow = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 99999999999999;
                        //lấy dhst do ng kê đơn thực hiện. nếu ko có lấy dhst gần với thời gian kê đơn nhất
                        var dhstExecute = dhsts.Where(o => o.CREATE_TIME <= (hIS_SERVICE_REQ.CREATE_TIME ?? dateTimeNow) && o.EXECUTE_LOGINNAME == hIS_SERVICE_REQ.REQUEST_LOGINNAME).ToList();
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

        private void GetDataPatientType(object obj)
        {
            try
            {
                if (obj != null)
                {
                    var hIS_SERVICE_REQ = obj as HIS_SERVICE_REQ;
                    //Lấy thông tin thẻ BHYT
                    HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
                    filter.TreatmentId = hIS_SERVICE_REQ.TREATMENT_ID;
                    filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                    vHisPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(RequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
