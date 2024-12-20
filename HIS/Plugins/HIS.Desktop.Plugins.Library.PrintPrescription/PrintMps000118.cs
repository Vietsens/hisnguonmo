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
    class PrintMps000118
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
        private HIS_MEDI_RECORD hisMediRecord;
        private List<HIS_SERE_SERV> hisListSereServClsTt;
        private HIS_EXP_MEST HisExpMest = null;
        string treatmentCode = "";

        short IS_TRUE = 1;

        List<ExpMestMedicineSDO> listGayNghien;
        List<ExpMestMedicineSDO> listHuongThan;
        List<ExpMestMedicineSDO> listTPCN;
        List<ExpMestMedicineSDO> listSPHoTro;
        List<ExpMestMedicineSDO> listCoChuaDuocChatGN;
        List<ExpMestMedicineSDO> listCoChuaDuocChatHT;
        private int numCopy;
        private bool printNow;
        private bool printDCGN = false;
        private bool printDCHT = false;
        private const int VatTu = 2;
        private const int VatTuNgoaiKho = 4;
        Inventec.Common.RichEditor.RichEditorStore richEditorMain;
        private Inventec.Desktop.Common.Modules.Module currentModule;
        MPS.ProcessorBase.PrintConfig.PreviewType? previewType;
        long? keyOrderListData;

        private List<HIS_CONFIG> lstConfigs;
        private HIS_TRANS_REQ transReq;

        Action<int, Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> SavedData;
        Action<string> CancelPrint;

        public PrintMps000118(string printTypeCode, string fileName, ref bool result,
            MOS.SDO.OutPatientPresResultSDO currentOutPresSDO,
            bool printNow, bool hasMediMate,
            Inventec.Common.RichEditor.RichEditorStore _richEditorMain,
            Inventec.Desktop.Common.Modules.Module module,
            MPS.ProcessorBase.PrintConfig.PreviewType? previewType, bool? hasOutHospital,
            List<HIS_TRANS_REQ> _lstTransReq, List<HIS_CONFIG> _lstConfig,
            Action<int> countData,
            Action<int, Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> savedData,
            Action<string> cancelPrint,
            bool CallFromPrescription)
        {
            try
            {
                this.currentModule = module;
                this.printNow = printNow;
                this.previewType = previewType;
                this.richEditorMain = _richEditorMain;
                this.SavedData = savedData;
                this.CancelPrint = cancelPrint;
                this.lstConfigs = _lstConfig;
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

                    List<MOS.EFMODEL.DataModels.HIS_EXP_MEST> ExpMests = null;
                    ThreadMedicineADO mediMateIn = null;
                    ThreadMedicineADO mediMateOut = null;

                    if (currentOutPresSDO.ExpMests != null &&
                        currentOutPresSDO.ExpMests.Count > 0)
                    {
                        ExpMests = currentOutPresSDO.ExpMests;
                        mediMateIn = new ThreadMedicineADO(currentOutPresSDO, hasMediMate, hasOutHospital);
                        mediMateOut = new ThreadMedicineADO(currentOutPresSDO, hasMediMate, hasOutHospital);
                        HisPrescriptionSDOPrintPlus = currentOutPresSDO.ServiceReqs.FirstOrDefault();
                    }

                    if (ExpMests == null || ExpMests.Count <= 0 || HisPrescriptionSDOPrintPlus == null)
                    {
                        result = false;
                        return;
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

                    treatmentCode = (hisTreatment != null ? hisTreatment.TREATMENT_CODE : "");

                    foreach (var item in ExpMests)
                    {
                        this.HisExpMest = item;
                        expMestCode = item.EXP_MEST_CODE;
                        var room = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == item.MEDI_STOCK_ID);
                        mediStockName = room != null ? room.MEDI_STOCK_NAME : "";

                        //Thong tin thuoc / vat tu
                        List<ExpMestMedicineSDO> lstMedicineExpmestTypeADO = new List<ExpMestMedicineSDO>();
                        List<ExpMestMedicineSDO> lstMedicineExpmestTypeTuTucADO = new List<ExpMestMedicineSDO>();
                        List<ExpMestMedicineSDO> lstMedicineExpmestTypeThuocHuongThanADO = new List<ExpMestMedicineSDO>();

                        listGayNghien = new List<ExpMestMedicineSDO>();
                        listHuongThan = new List<ExpMestMedicineSDO>();
                        listTPCN = new List<ExpMestMedicineSDO>();
                        listSPHoTro = new List<ExpMestMedicineSDO>();
                        listCoChuaDuocChatGN = new List<ExpMestMedicineSDO>();
                        listCoChuaDuocChatHT = new List<ExpMestMedicineSDO>();

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

                        if (currentOutPresSDO.ServiceReqs != null &&
                            currentOutPresSDO.ServiceReqs.Count > 0)
                        {
                            HisPrescriptionSDOPrintPlus = currentOutPresSDO.ServiceReqs.FirstOrDefault(o => o.ID == item.SERVICE_REQ_ID);
                        }
                        if (HisPrescriptionSDOPrintPlus == null) continue;
                        if (_lstTransReq != null && _lstTransReq.Count > 0)
                        {
                            this.transReq = _lstTransReq.FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.TRANS_REQ_ID);
                        }
                        if (HisPrescriptionSDOPrintPlus.USE_TIME == null)
                        {
                            HisPrescriptionSDOPrintPlus.USE_TIME = HisPrescriptionSDOPrintPlus.INTRUCTION_TIME;
                        }

                        executeRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.EXECUTE_ROOM_ID);
                        reqRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.REQUEST_ROOM_ID);

                        List<ExpMestMedicineSDO> listNgoaiVien = new List<ExpMestMedicineSDO>();
                        List<ExpMestMedicineSDO> listTThuongNgoaiKho = new List<ExpMestMedicineSDO>();

                        long keyPrintType = Convert.ToInt64(ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__CHE_DO_IN_TACH_DON_THUOC));

                        var _ThuocCoChuaDCGNCFG = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCGN);
                        var _ThuocCoChuaDCHTCFG = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCHT);

                        if (keyPrintType == 0)// nambg keyPrintType != 1
                        {
                            //danh sách thuốc ngoài viện
                            listNgoaiVien = lstMedicineExpmestTypeADO.Where(o => o.IS_OUT_HOSPITAL == IS_TRUE).ToList();

                            //Bo thuoc ngoài viện ra khoi danh sach thuoc tong hop
                            if (listNgoaiVien != null && listNgoaiVien.Count > 0)
                            {
                                lstMedicineExpmestTypeADO = lstMedicineExpmestTypeADO.Except(listNgoaiVien).ToList();
                            }

                            //Danh sach thuoc huong than,gây nghiện, 
                            lstMedicineExpmestTypeThuocHuongThanADO = lstMedicineExpmestTypeADO.Where(o =>
                                o.IS_NEUROLOGICAL == IS_TRUE || o.IS_ADDICTIVE == IS_TRUE || o.IS_FUNCTIONAL_FOOD == IS_TRUE
                                ).ToList();

                            listHuongThan = lstMedicineExpmestTypeADO.Where(o => o.IS_NEUROLOGICAL == IS_TRUE).ToList();
                            listGayNghien = lstMedicineExpmestTypeADO.Where(o => o.IS_ADDICTIVE == IS_TRUE).ToList();
                            listTPCN = lstMedicineExpmestTypeADO.Where(o => o.IS_FUNCTIONAL_FOOD == IS_TRUE).ToList();

                            if (_ThuocCoChuaDCGNCFG != null && _ThuocCoChuaDCGNCFG.IS_SEPARATE_PRINTING == 1)
                            {
                                //ds thuoc co chua duoc chat gay nghien
                                listCoChuaDuocChatGN = lstMedicineExpmestTypeADO.Where(o => o.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCGN).ToList();
                            }
                            if (_ThuocCoChuaDCHTCFG != null && _ThuocCoChuaDCHTCFG.IS_SEPARATE_PRINTING == 1)
                            {
                                //ds thuoc co chua duoc chat gay nghien
                                listCoChuaDuocChatHT = lstMedicineExpmestTypeADO.Where(o => o.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCHT).ToList();
                            }

                            //Bo thuoc huong than gay nghien ra khoi danh sach thuoc tong hop
                            if (lstMedicineExpmestTypeThuocHuongThanADO != null && lstMedicineExpmestTypeThuocHuongThanADO.Count > 0)
                            {
                                lstMedicineExpmestTypeADO = lstMedicineExpmestTypeADO.Except(lstMedicineExpmestTypeThuocHuongThanADO).ToList();
                            }
                            //Bo thuoc co chua duoc chat GN ra khoi danh sach thuoc tong hop
                            if (listCoChuaDuocChatGN != null && listCoChuaDuocChatGN.Count > 0)
                            {
                                lstMedicineExpmestTypeADO = lstMedicineExpmestTypeADO.Except(listCoChuaDuocChatGN).ToList();
                            }
                            //Bo thuoc co chua duoc chat HT ra khoi danh sach thuoc tong hop
                            if (listCoChuaDuocChatHT != null && listCoChuaDuocChatHT.Count > 0)
                            {
                                lstMedicineExpmestTypeADO = lstMedicineExpmestTypeADO.Except(listCoChuaDuocChatHT).ToList();
                            }
                        }
                        else if (keyPrintType == 1) // 
                        {
                            if (_ThuocCoChuaDCGNCFG != null && _ThuocCoChuaDCGNCFG.IS_SEPARATE_PRINTING == 1)
                            {
                                //ds thuoc co chua duoc chat gay nghien
                                listCoChuaDuocChatGN = lstMedicineExpmestTypeADO.Where(o => o.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCGN).ToList();
                            }
                            if (_ThuocCoChuaDCHTCFG != null && _ThuocCoChuaDCHTCFG.IS_SEPARATE_PRINTING == 1)
                            {
                                //ds thuoc co chua duoc chat gay nghien
                                listCoChuaDuocChatHT = lstMedicineExpmestTypeADO.Where(o => o.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCHT).ToList();
                            }
                            //Bo thuoc co chua duoc chat GN ra khoi danh sach thuoc tong hop
                            if (listCoChuaDuocChatGN != null && listCoChuaDuocChatGN.Count > 0)
                            {
                                lstMedicineExpmestTypeADO = lstMedicineExpmestTypeADO.Except(listCoChuaDuocChatGN).ToList();
                            }
                            //Bo thuoc co chua duoc chat HT ra khoi danh sach thuoc tong hop
                            if (listCoChuaDuocChatGN != null && listCoChuaDuocChatGN.Count > 0)
                            {
                                lstMedicineExpmestTypeADO = lstMedicineExpmestTypeADO.Except(listCoChuaDuocChatHT).ToList();
                            }

                        }
                        else if (keyPrintType == 2)// nambg issue #23730
                        {
                            //danh sách thuốc ngoài viện
                            listNgoaiVien = lstMedicineExpmestTypeADO.Where(o => o.IS_OUT_HOSPITAL == IS_TRUE).ToList();

                            //Bo thuoc ngoài viện ra khoi danh sach thuoc tong hop
                            if (listNgoaiVien != null && listNgoaiVien.Count > 0)
                            {
                                lstMedicineExpmestTypeADO = lstMedicineExpmestTypeADO.Except(listNgoaiVien).ToList();
                            }

                            //Danh sach thuoc huong than,gây nghiện, 
                            lstMedicineExpmestTypeThuocHuongThanADO = lstMedicineExpmestTypeADO.Where(o =>
                                o.IS_NEUROLOGICAL == IS_TRUE || o.IS_ADDICTIVE == IS_TRUE
                                ).ToList();

                            listHuongThan = lstMedicineExpmestTypeADO.Where(o => o.IS_NEUROLOGICAL == IS_TRUE).ToList();
                            listGayNghien = lstMedicineExpmestTypeADO.Where(o => o.IS_ADDICTIVE == IS_TRUE).ToList();
                            listSPHoTro = lstMedicineExpmestTypeADO.Where(o => o.IS_FUNCTIONAL_FOOD == IS_TRUE || o.Type == VatTu || o.Type == VatTuNgoaiKho).ToList();

                            //Bo thuoc huong than gay nghien ra khoi danh sach thuoc tong hop
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
                        else if (keyPrintType == 3)
                        {

                            //Danh sach thuoc huong than,gây nghiện
                            lstMedicineExpmestTypeThuocHuongThanADO = lstMedicineExpmestTypeADO.Where(o =>
                                o.IS_NEUROLOGICAL == IS_TRUE || o.IS_ADDICTIVE == IS_TRUE
                                ).ToList();

                            listHuongThan = lstMedicineExpmestTypeADO.Where(o => o.IS_NEUROLOGICAL == IS_TRUE).ToList();
                            listGayNghien = lstMedicineExpmestTypeADO.Where(o => o.IS_ADDICTIVE == IS_TRUE).ToList();

                            List<ExpMestMedicineSDO> lstMedicineExpmestTypeKhongChua_GN_HT_ADO = new List<ExpMestMedicineSDO>();
                            //Bo thuoc huong than gay nghien ra khoi danh sach thuoc tong hop
                            if (lstMedicineExpmestTypeThuocHuongThanADO != null && lstMedicineExpmestTypeThuocHuongThanADO.Count > 0)
                            {
                                lstMedicineExpmestTypeKhongChua_GN_HT_ADO = lstMedicineExpmestTypeADO.Except(lstMedicineExpmestTypeThuocHuongThanADO).ToList();
                            }
                            else
                            {
                                lstMedicineExpmestTypeKhongChua_GN_HT_ADO.AddRange(lstMedicineExpmestTypeADO);
                            }

                            //Lấy ra danh sách thuốc thường trong kho không chứa thuốc gây nghiện, hướng thần
                            if (lstMedicineExpmestTypeKhongChua_GN_HT_ADO != null && lstMedicineExpmestTypeKhongChua_GN_HT_ADO.Count > 0)
                            {
                                lstMedicineExpmestTypeADO = lstMedicineExpmestTypeKhongChua_GN_HT_ADO.Where(o => o.Type == 1 || o.Type == 2).ToList();
                                Inventec.Common.Logging.LogSystem.Debug("Danh sach thuoc thuong trong kho: " + Inventec.Common.Logging.LogUtil.TraceData("lstMedicineExpmestTypeADO", lstMedicineExpmestTypeADO));
                            }

                            //Lọc bỏ danh sách thuốc thường trong kho
                            if (lstMedicineExpmestTypeADO != null && lstMedicineExpmestTypeADO.Count > 0)
                            {
                                lstMedicineExpmestTypeKhongChua_GN_HT_ADO = lstMedicineExpmestTypeKhongChua_GN_HT_ADO.Except(lstMedicineExpmestTypeADO).ToList();
                            }
                            if (lstMedicineExpmestTypeKhongChua_GN_HT_ADO != null && lstMedicineExpmestTypeKhongChua_GN_HT_ADO.Count > 0)
                            {
                                
                                listSPHoTro = lstMedicineExpmestTypeKhongChua_GN_HT_ADO.Where(o => o.IS_FUNCTIONAL_FOOD == IS_TRUE || o.Type == 4).ToList();
                                listTThuongNgoaiKho = lstMedicineExpmestTypeKhongChua_GN_HT_ADO.Except(listSPHoTro).ToList();
                            }
                        }
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

                        #region in thuong
                        //không có đơn thuốc sẽ gán dữ liệu với ID = -1 để in
                        if (lstMedicineExpmestTypeADO != null && lstMedicineExpmestTypeADO.Count > 0
                            || (ExpMests.Count == 1 && ExpMests.First().ID == -1 && HisPrescriptionSDOPrintPlus.ID == -1))
                        {
                            List<MPS.Processor.Mps000118.PDO.ExpMestMedicineSDO> ExpMestMedicineSDO = new List<MPS.Processor.Mps000118.PDO.ExpMestMedicineSDO>();
                            var group = lstMedicineExpmestTypeADO.GroupBy(o => new { o.MEDICINE_TYPE_ID, o.MEDICINE_TYPE_NAME });
                            foreach (var aitem in group)
                            {
                                MPS.Processor.Mps000118.PDO.ExpMestMedicineSDO ado = new MPS.Processor.Mps000118.PDO.ExpMestMedicineSDO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000118.PDO.ExpMestMedicineSDO>(ado, aitem.First());
                                ado.AMOUNT = aitem.Sum(o => o.AMOUNT);
                                ExpMestMedicineSDO.Add(ado);
                            }
                            //Lọc thuốc theo thứ tự
                            ExpMestMedicineSDO = ExpMestMedicineSDO.OrderBy(o => o.NUM_ORDER ?? 99999).ToList();

                            MPS.Processor.Mps000118.PDO.Mps000118ADO mps000118ADO = new MPS.Processor.Mps000118.PDO.Mps000118ADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000118.PDO.Mps000118ADO>(mps000118ADO, vHisPatient);
                            mps000118ADO.EXECUTE_DEPARTMENT_NAME = executeRoom != null ? executeRoom.DEPARTMENT_NAME : "";
                            mps000118ADO.EXECUTE_ROOM_NAME = executeRoom != null ? executeRoom.ROOM_NAME : "";
                            mps000118ADO.EXP_MEST_CODE = expMestCode;
                            mps000118ADO.REQUEST_DEPARTMENT_NAME = reqRoom != null ? reqRoom.DEPARTMENT_NAME : "";
                            mps000118ADO.REQUEST_ROOM_NAME = reqRoom != null ? reqRoom.ROOM_NAME : "";
                            mps000118ADO.MEDI_STOCK_NAME = mediStockName;
                            mps000118ADO.TITLE = "";
                            mps000118ADO.MEDI_RECORD_STORE_CODE = this.hisMediRecord != null ? this.hisMediRecord.STORE_CODE : "";

                            var executeDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.EXECUTE_DEPARTMENT_ID);
                            var requestDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.REQUEST_DEPARTMENT_ID);
                            mps000118ADO.EXECUTE_PHONE = executeDepartment != null ? executeDepartment.PHONE : "";
                            mps000118ADO.REQUEST_PHONE = requestDepartment != null ? requestDepartment.PHONE : "";

                            var acsUser = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME == HisPrescriptionSDOPrintPlus.REQUEST_LOGINNAME);
                            mps000118ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                            MPS.Processor.Mps000118.PDO.Mps000118PDO mps000118RDO = new MPS.Processor.Mps000118.PDO.Mps000118PDO(
                                vHisPatientTypeAlter,
                                hisDhst,
                                HisPrescriptionSDOPrintPlus,
                                ExpMestMedicineSDO,
                                HisServiceReq_Exam,
                                hisTreatment,
                                mps000118ADO,
                                hisListSereServClsTt,
                                keyOrderListData,
                                item,
                                hisServiceReq_CurentExam,
                                transReq,
                                _lstConfig);

                            Print.PrintData(printTypeCode, fileName, mps000118RDO, printNow, treatmentCode, ref result, this.currentModule != null ? currentModule.RoomId : 0, previewType, lstMedicineExpmestTypeADO.Count, this.SavedData, numCopy);
                            //PrintData(printTypeCode, fileName, mps000118RDO, printNow, numCopy, ref result);
                        }
                        #endregion
                        #region in thuong NK
                        if (listTThuongNgoaiKho != null && listTThuongNgoaiKho.Count > 0)
                        {
                            List<MPS.Processor.Mps000118.PDO.ExpMestMedicineSDO> ExpMestMedicineSDO = new List<MPS.Processor.Mps000118.PDO.ExpMestMedicineSDO>();
                            var group = listTThuongNgoaiKho.GroupBy(o => new { o.MEDICINE_TYPE_ID, o.MEDICINE_TYPE_NAME });
                            foreach (var aitem in group)
                            {
                                MPS.Processor.Mps000118.PDO.ExpMestMedicineSDO ado = new MPS.Processor.Mps000118.PDO.ExpMestMedicineSDO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000118.PDO.ExpMestMedicineSDO>(ado, aitem.First());
                                ado.AMOUNT = aitem.Sum(o => o.AMOUNT);
                                ExpMestMedicineSDO.Add(ado);
                            }
                            //Lọc thuốc theo thứ tự
                            ExpMestMedicineSDO = ExpMestMedicineSDO.OrderBy(o => o.NUM_ORDER ?? 99999).ToList();

                            MPS.Processor.Mps000118.PDO.Mps000118ADO mps000118ADO = new MPS.Processor.Mps000118.PDO.Mps000118ADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000118.PDO.Mps000118ADO>(mps000118ADO, vHisPatient);
                            mps000118ADO.EXECUTE_DEPARTMENT_NAME = executeRoom != null ? executeRoom.DEPARTMENT_NAME : "";
                            mps000118ADO.EXECUTE_ROOM_NAME = executeRoom != null ? executeRoom.ROOM_NAME : "";
                            mps000118ADO.EXP_MEST_CODE = expMestCode;
                            mps000118ADO.REQUEST_DEPARTMENT_NAME = reqRoom != null ? reqRoom.DEPARTMENT_NAME : "";
                            mps000118ADO.REQUEST_ROOM_NAME = reqRoom != null ? reqRoom.ROOM_NAME : "";
                            mps000118ADO.MEDI_STOCK_NAME = mediStockName;
                            mps000118ADO.TITLE = "NK";
                            mps000118ADO.MEDI_RECORD_STORE_CODE = this.hisMediRecord != null ? this.hisMediRecord.STORE_CODE : "";

                            var executeDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.EXECUTE_DEPARTMENT_ID);
                            var requestDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.REQUEST_DEPARTMENT_ID);
                            mps000118ADO.EXECUTE_PHONE = executeDepartment != null ? executeDepartment.PHONE : "";
                            mps000118ADO.REQUEST_PHONE = requestDepartment != null ? requestDepartment.PHONE : "";

                            var acsUser = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME == HisPrescriptionSDOPrintPlus.REQUEST_LOGINNAME);
                            mps000118ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                            MPS.Processor.Mps000118.PDO.Mps000118PDO mps000118RDO = new MPS.Processor.Mps000118.PDO.Mps000118PDO(
                                vHisPatientTypeAlter,
                                hisDhst,
                                HisPrescriptionSDOPrintPlus,
                                ExpMestMedicineSDO,
                                HisServiceReq_Exam,
                                hisTreatment,
                                mps000118ADO,
                                hisListSereServClsTt,
                                keyOrderListData,
                                item,
                                hisServiceReq_CurentExam,
                                transReq,
                                _lstConfig);

                            Print.PrintData(printTypeCode, fileName, mps000118RDO, printNow, treatmentCode, ref result, this.currentModule != null ? currentModule.RoomId : 0, previewType, listTThuongNgoaiKho.Count, this.SavedData, numCopy);
                        }
                        #endregion

                        #region gay Nghien V2 3 lien
                        if (listGayNghien != null && listGayNghien.Count > 0)
                        {
                            InGayNghien();
                        }

                        if (listHuongThan != null && listHuongThan.Count > 0)
                        {
                            InHuongThan();
                        }
                        #endregion

                        #region in san pham ho tro
                        if (listSPHoTro != null && listSPHoTro.Count > 0)
                        {
                            InSanPhamHoTro();
                        }
                        #endregion

                        #region TPCN
                        if (listTPCN != null && listTPCN.Count > 0)
                        {
                            InThucPham();
                        }
                        #endregion

                        #region in Ngoai vien
                        if (listNgoaiVien != null && listNgoaiVien.Count > 0)
                        {
                            List<MPS.Processor.Mps000118.PDO.ExpMestMedicineSDO> ExpMestMedicineSDO = new List<MPS.Processor.Mps000118.PDO.ExpMestMedicineSDO>();
                            var group = listNgoaiVien.GroupBy(o => new { o.MEDICINE_TYPE_ID, o.MEDICINE_TYPE_NAME });
                            foreach (var aitem in group)
                            {
                                MPS.Processor.Mps000118.PDO.ExpMestMedicineSDO ado = new MPS.Processor.Mps000118.PDO.ExpMestMedicineSDO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000118.PDO.ExpMestMedicineSDO>(ado, aitem.First());
                                ado.AMOUNT = aitem.Sum(o => o.AMOUNT);
                                ExpMestMedicineSDO.Add(ado);
                            }
                            //Lọc thuốc theo thứ tự
                            ExpMestMedicineSDO = ExpMestMedicineSDO.OrderBy(o => o.NUM_ORDER ?? 99999).ToList();

                            MPS.Processor.Mps000118.PDO.Mps000118ADO mps000118ADO = new MPS.Processor.Mps000118.PDO.Mps000118ADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000118.PDO.Mps000118ADO>(mps000118ADO, vHisPatient);
                            mps000118ADO.EXECUTE_DEPARTMENT_NAME = executeRoom != null ? executeRoom.DEPARTMENT_NAME : "";
                            mps000118ADO.EXECUTE_ROOM_NAME = executeRoom != null ? executeRoom.ROOM_NAME : "";
                            mps000118ADO.EXP_MEST_CODE = expMestCode;
                            mps000118ADO.REQUEST_DEPARTMENT_NAME = reqRoom != null ? reqRoom.DEPARTMENT_NAME : "";
                            mps000118ADO.REQUEST_ROOM_NAME = reqRoom != null ? reqRoom.ROOM_NAME : "";
                            mps000118ADO.MEDI_STOCK_NAME = mediStockName;
                            mps000118ADO.TITLE = "NV";
                            mps000118ADO.MEDI_RECORD_STORE_CODE = this.hisMediRecord != null ? this.hisMediRecord.STORE_CODE : "";

                            var executeDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.EXECUTE_DEPARTMENT_ID);
                            var requestDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.REQUEST_DEPARTMENT_ID);
                            mps000118ADO.EXECUTE_PHONE = executeDepartment != null ? executeDepartment.PHONE : "";
                            mps000118ADO.REQUEST_PHONE = requestDepartment != null ? requestDepartment.PHONE : "";

                            var acsUser = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME == HisPrescriptionSDOPrintPlus.REQUEST_LOGINNAME);
                            mps000118ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                            MPS.Processor.Mps000118.PDO.Mps000118PDO mps000118RDO = new MPS.Processor.Mps000118.PDO.Mps000118PDO(
                                vHisPatientTypeAlter,
                                hisDhst,
                                HisPrescriptionSDOPrintPlus,
                                ExpMestMedicineSDO,
                                HisServiceReq_Exam,
                                hisTreatment,
                                mps000118ADO,
                                hisListSereServClsTt,
                                keyOrderListData,
                                item,
                                hisServiceReq_CurentExam,
                                transReq,
                                _lstConfig);

                            Print.PrintData(printTypeCode, fileName, mps000118RDO, printNow, treatmentCode, ref result, this.currentModule != null ? currentModule.RoomId : 0, previewType, listNgoaiVien.Count, this.SavedData, numCopy);
                            //PrintData(printTypeCode, fileName, mps000118RDO, printNow, numCopy, ref result);
                        }
                        #endregion

                        #region thuốc có chứa dược chất GN
                        if (listCoChuaDuocChatGN != null && listCoChuaDuocChatGN.Count > 0)
                        {
                            this.printDCGN = true;
                            InHCGN();
                        }
                        #endregion

                        #region thuốc có chứa dược chất HT
                        if (listCoChuaDuocChatHT != null && listCoChuaDuocChatHT.Count > 0)
                        {
                            this.printDCHT = true;
                            InHCHT();
                        }
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

        private void InDoc()
        {
            richEditorMain.RunPrintTemplate(MPS.Processor.Mps000238.PDO.Mps000238PDO.PrintTypeCode, DelegateRunPrinter);
        }

        private void InPhongXa()
        {
            richEditorMain.RunPrintTemplate(MPS.Processor.Mps000237.PDO.Mps000237PDO.PrintTypeCode, DelegateRunPrinter);
        }

        private void InGayNghien()
        {
            richEditorMain.RunPrintTemplate(MPS.Processor.Mps000181.PDO.Mps000181PDO.PrintTypeCode, DelegateRunPrinter);
        }

        private void InHCGN()
        {
            richEditorMain.RunPrintTemplate(MPS.Processor.Mps000181.PDO.Mps000181PDO.PrintTypeCode, DelegateRunPrinter);
        }
        private void InHCHT()
        {
            richEditorMain.RunPrintTemplate(MPS.Processor.Mps000192.PDO.Mps000192PDO.PrintTypeCode, DelegateRunPrinter);
        }
        private void InSanPhamHoTro()
        {
            richEditorMain.RunPrintTemplate(MPS.Processor.Mps000353.PDO.Mps000353PDO.PrintTypeCode, DelegateRunPrinter);
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
                        if (printDCGN)
                            DelegatePrintHCGN(printTypeCode, fileName, ref result);
                        else
                            DelegatePrintGNHT(printTypeCode, fileName, ref result);
                        break;
                    case MPS.Processor.Mps000191.PDO.Mps000191PDO.PrintTypeCode:
                        DelegatePrintTPCN(printTypeCode, fileName, ref result);
                        break;
                    case MPS.Processor.Mps000192.PDO.Mps000192PDO.PrintTypeCode:
                        if (printDCHT)
                            DelegatePrintHCHT(printTypeCode, fileName, ref result);
                        else
                            DelegatePrintHT(printTypeCode, fileName, ref result);
                        break;
                    //case MPS.Processor.Mps000237.PDO.Mps000237PDO.PrintTypeCode:
                    //    DelegatePrintPX(printTypeCode, fileName, ref result);
                    //    break;
                    //case MPS.Processor.Mps000238.PDO.Mps000238PDO.PrintTypeCode:
                    //    DelegatePrintD(printTypeCode, fileName, ref result);
                    //    break;
                    case MPS.Processor.Mps000353.PDO.Mps000353PDO.PrintTypeCode:
                        DelegatePrintSPHoTro(printTypeCode, fileName, ref result);
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

        private void DelegatePrintHCHT(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (listCoChuaDuocChatHT != null && listCoChuaDuocChatHT.Count > 0)
                {
                    List<MPS.Processor.Mps000192.PDO.ExpMestMedicineSDO> list = new List<MPS.Processor.Mps000192.PDO.ExpMestMedicineSDO>();
                    var group = listCoChuaDuocChatHT.GroupBy(o => new { o.MEDICINE_TYPE_ID, o.MEDICINE_TYPE_NAME });
                    foreach (var item in group)
                    {
                        MPS.Processor.Mps000192.PDO.ExpMestMedicineSDO sdo = new MPS.Processor.Mps000192.PDO.ExpMestMedicineSDO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000192.PDO.ExpMestMedicineSDO>(sdo, item.First());
                        sdo.AMOUNT = item.Sum(o => o.AMOUNT);
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
                    mps000192ADO.TITLE = "Thuốc có chứa dược chất hướng thần";

                    var executeDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.EXECUTE_DEPARTMENT_ID);
                    var requestDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.REQUEST_DEPARTMENT_ID);
                    mps000192ADO.EXECUTE_PHONE = executeDepartment != null ? executeDepartment.PHONE : "";
                    mps000192ADO.REQUEST_PHONE = requestDepartment != null ? requestDepartment.PHONE : "";

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
                        hisListSereServClsTt,
                        keyOrderListData,
                        this.HisExpMest,
                        this.transReq,
                        this.lstConfigs);

                    Print.PrintData(printTypeCode, fileName, mps000192PDO, printNow, treatmentCode, ref result, this.currentModule != null ? currentModule.RoomId : 0, previewType, listCoChuaDuocChatHT.Count, this.SavedData, numCopy);
                    //PrintData(printTypeCode, fileName, mps000192PDO, printNow, numCopy, ref result);
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void DelegatePrintHCGN(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (listCoChuaDuocChatGN != null && listCoChuaDuocChatGN.Count > 0)
                {
                    //Lọc thuốc theo thứ tự
                    listCoChuaDuocChatGN = listCoChuaDuocChatGN.OrderBy(o => o.NUM_ORDER ?? 99999).ToList();
                    List<MPS.Processor.Mps000181.PDO.ExpMestMedicineSDO> listHCGN = new List<MPS.Processor.Mps000181.PDO.ExpMestMedicineSDO>();
                    var group = listCoChuaDuocChatGN.GroupBy(o => new { o.MEDICINE_TYPE_ID, o.MEDICINE_TYPE_NAME });
                    foreach (var itemN in group)
                    {
                        MPS.Processor.Mps000181.PDO.ExpMestMedicineSDO ado = new MPS.Processor.Mps000181.PDO.ExpMestMedicineSDO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000181.PDO.ExpMestMedicineSDO>(ado, itemN.First());
                        ado.AMOUNT = itemN.Sum(o => o.AMOUNT);
                        listHCGN.Add(ado);
                    }
                    MPS.Processor.Mps000181.PDO.Mps000181ADO Mps000181ADO = new MPS.Processor.Mps000181.PDO.Mps000181ADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000181.PDO.Mps000181ADO>(Mps000181ADO, vHisPatient);
                    Mps000181ADO.EXECUTE_DEPARTMENT_NAME = executeRoom != null ? executeRoom.DEPARTMENT_NAME : "";
                    Mps000181ADO.EXECUTE_ROOM_NAME = executeRoom != null ? executeRoom.ROOM_NAME : "";
                    Mps000181ADO.EXP_MEST_CODE = expMestCode;
                    Mps000181ADO.REQUEST_DEPARTMENT_NAME = reqRoom != null ? reqRoom.DEPARTMENT_NAME : "";
                    Mps000181ADO.REQUEST_ROOM_NAME = reqRoom != null ? reqRoom.ROOM_NAME : "";
                    Mps000181ADO.MEDI_STOCK_NAME = mediStockName;
                    Mps000181ADO.TITLE = "Thuốc có chứa dược chất gây nghiện";

                    var executeDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.EXECUTE_DEPARTMENT_ID);
                    var requestDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.REQUEST_DEPARTMENT_ID);
                    Mps000181ADO.EXECUTE_PHONE = executeDepartment != null ? executeDepartment.PHONE : "";
                    Mps000181ADO.REQUEST_PHONE = requestDepartment != null ? requestDepartment.PHONE : "";

                    var acsUser = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME == HisPrescriptionSDOPrintPlus.REQUEST_LOGINNAME);
                    Mps000181ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                    MPS.Processor.Mps000181.PDO.Mps000181PDO mps000181PDO = new MPS.Processor.Mps000181.PDO.Mps000181PDO(
                        vHisPatientTypeAlter,
                        hisDhst,
                        HisPrescriptionSDOPrintPlus,
                        listHCGN,
                        HisServiceReq_Exam,
                        hisTreatment,
                        Mps000181ADO,
                        hisListSereServClsTt,
                        keyOrderListData,
                        this.HisExpMest,
                        this.transReq,
                        this.lstConfigs);

                    Print.PrintData(printTypeCode, fileName, mps000181PDO, printNow, treatmentCode, ref result, this.currentModule != null ? currentModule.RoomId : 0, previewType, listCoChuaDuocChatGN.Count, this.SavedData, numCopy);
                    //PrintData(printTypeCode, fileName, mps000181PDO, printNow, numCopy, ref result);
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DelegatePrintSPHoTro(string printTypeCode, string fileName, ref bool result)
        {
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
                        hisListSereServClsTt,
                        keyOrderListData,
                        this.HisExpMest);

                    Print.PrintData(printTypeCode, fileName, Mps000353RDO, printNow, treatmentCode, ref result, this.currentModule != null ? currentModule.RoomId : 0, previewType, listSPHoTro.Count, this.SavedData, numCopy);
                    //PrintData(printTypeCode, fileName, Mps000353RDO, printNow, 1, ref result);
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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
                    var group = listGayNghien.GroupBy(o => new { o.MEDICINE_TYPE_ID, o.MEDICINE_TYPE_NAME });
                    foreach (var itemN in group)
                    {
                        MPS.Processor.Mps000181.PDO.ExpMestMedicineSDO ado = new MPS.Processor.Mps000181.PDO.ExpMestMedicineSDO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000181.PDO.ExpMestMedicineSDO>(ado, itemN.First());
                        ado.AMOUNT = itemN.Sum(o => o.AMOUNT);
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

                    var executeDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.EXECUTE_DEPARTMENT_ID);
                    var requestDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.REQUEST_DEPARTMENT_ID);
                    Mps000181ADO.EXECUTE_PHONE = executeDepartment != null ? executeDepartment.PHONE : "";
                    Mps000181ADO.REQUEST_PHONE = requestDepartment != null ? requestDepartment.PHONE : "";

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
                        hisListSereServClsTt,
                        keyOrderListData,
                        this.HisExpMest,
                        this.transReq,
                        this.lstConfigs);

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
                    var group = listTPCN.GroupBy(o => new { o.MEDICINE_TYPE_ID, o.MEDICINE_TYPE_NAME });
                    foreach (var item in group)
                    {
                        MPS.Processor.Mps000191.PDO.ExpMestMedicineSDO sdo = new MPS.Processor.Mps000191.PDO.ExpMestMedicineSDO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000191.PDO.ExpMestMedicineSDO>(sdo, item.First());
                        sdo.AMOUNT = item.Sum(o => o.AMOUNT);
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

                    var executeDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.EXECUTE_DEPARTMENT_ID);
                    var requestDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.REQUEST_DEPARTMENT_ID);
                    mps000191ADO.EXECUTE_PHONE = executeDepartment != null ? executeDepartment.PHONE : "";
                    mps000191ADO.REQUEST_PHONE = requestDepartment != null ? requestDepartment.PHONE : "";

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
                        hisListSereServClsTt,
                        keyOrderListData,
                        this.HisExpMest);

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
                    var group = listHuongThan.GroupBy(o => new { o.MEDICINE_TYPE_ID, o.MEDICINE_TYPE_NAME });
                    foreach (var item in group)
                    {
                        MPS.Processor.Mps000192.PDO.ExpMestMedicineSDO sdo = new MPS.Processor.Mps000192.PDO.ExpMestMedicineSDO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000192.PDO.ExpMestMedicineSDO>(sdo, item.First());
                        sdo.AMOUNT = item.Sum(o => o.AMOUNT);
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

                    var executeDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.EXECUTE_DEPARTMENT_ID);
                    var requestDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.REQUEST_DEPARTMENT_ID);
                    mps000192ADO.EXECUTE_PHONE = executeDepartment != null ? executeDepartment.PHONE : "";
                    mps000192ADO.REQUEST_PHONE = requestDepartment != null ? requestDepartment.PHONE : "";

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
                        hisListSereServClsTt,
                        keyOrderListData,
                        this.HisExpMest,
                        this.transReq,
                        this.lstConfigs);

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
                        GetMediRecord(hisTreatment.MEDI_RECORD_ID);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetMediRecord(long? mediRecordId)
        {
            try
            {
                if (mediRecordId.HasValue && mediRecordId > 0)
                {
                    CommonParam paramCommon = new CommonParam();
                    HisMediRecordFilter filter = new HisMediRecordFilter();
                    filter.ID = mediRecordId;
                    var lstMediRecord = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_MEDI_RECORD>>(RequestUriStore.HIS_MEDI_RECORD_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    if (lstMediRecord != null && lstMediRecord.Count > 0)
                    {
                        hisMediRecord = lstMediRecord.FirstOrDefault();
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
                HIS_SERVICE_REQ data = (HIS_SERVICE_REQ)obj;
                vHisPatient = new V_HIS_PATIENT();
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

        private void GetDataExam(object obj)
        {
            try
            {
                GetExamWithPres((HIS_SERVICE_REQ)obj);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetExamWithPres(HIS_SERVICE_REQ hIS_SERVICE_REQ)
        {
            try
            {
                HisServiceReqFilter serviceReqfilter = new HisServiceReqFilter();
                serviceReqfilter.TREATMENT_ID = hIS_SERVICE_REQ.TREATMENT_ID;
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
                            if (item.EXECUTE_ROOM_ID == hIS_SERVICE_REQ.REQUEST_ROOM_ID && (!String.IsNullOrEmpty(item.FULL_EXAM) || !String.IsNullOrEmpty(item.PATHOLOGICAL_PROCESS)))
                            {
                                lstExit.Add(item);
                            }

                            if (item.ID == hIS_SERVICE_REQ.PARENT_ID && !lstExit.Exists(o => o.ID == item.ID))
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataPatientTypeDhst(object obj)
        {
            try
            {
                GetPatientDhst((HIS_SERVICE_REQ)obj);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetPatientDhst(HIS_SERVICE_REQ hIS_SERVICE_REQ)
        {
            try
            {
                CommonParam param = new CommonParam();

                //Lấy thông tin thẻ BHYT
                HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
                filter.TreatmentId = hIS_SERVICE_REQ.TREATMENT_ID;
                filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                vHisPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(RequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                //Dấu hiệu sinh tồn
                MOS.Filter.HisDhstFilter hisDhstFilter = new HisDhstFilter();
                hisDhstFilter.TREATMENT_ID = hIS_SERVICE_REQ.TREATMENT_ID;
                var dhsts = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_DHST>>(RequestUriStore.HIS_DHST_GET, ApiConsumer.ApiConsumers.MosConsumer, hisDhstFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (dhsts != null && dhsts.Count > 0)
                {
                    //hisDhst = dhsts.FirstOrDefault(o => o.EXECUTE_TIME <= hIS_SERVICE_REQ.INTRUCTION_TIME);
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
    }
}
