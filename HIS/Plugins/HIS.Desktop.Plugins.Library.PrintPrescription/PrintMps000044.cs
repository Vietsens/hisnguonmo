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
    class PrintMps000044
    {
        private V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter;
        private HIS_DHST hisDhst;
        private V_HIS_PATIENT vHisPatient;
        private short IS_TRUE = 1;
        private HIS_TREATMENT hisTreatment;
        private HIS_SERVICE_REQ HisServiceReq_Exam;
        private HIS_SERVICE_REQ hisServiceReq_CurentExam;
        string treatmentCode = "";
        private Inventec.Desktop.Common.Modules.Module currentModule;
        private string expMestCode;
        private const int VatTu = 2;
        private const int VatTuNgoaiKho = 4;
        HIS_SERVICE_REQ HisPrescriptionSDOPrintPlus = null;
        List<ExpMestMedicineSDO> listSPHoTro = null;
        HIS_EXP_MEST HisExpMest = null;
        Inventec.Common.RichEditor.RichEditorStore richEditorMain;
        string mediStockName = "";
        bool printNow;
        MPS.ProcessorBase.PrintConfig.PreviewType? previewType;
        bool? hasOutHospital;
        long? keyOrderListData;

        Action<int, Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> SavedData;
        Action<string> CancelPrint;

        public PrintMps000044(string printTypeCode, string fileName, ref bool result,
            MOS.SDO.OutPatientPresResultSDO currentOutPresSDO,
            bool printNow, bool hasMediMate, Inventec.Desktop.Common.Modules.Module module,
            Inventec.Common.RichEditor.RichEditorStore _richEditorMain,
            MPS.ProcessorBase.PrintConfig.PreviewType? previewType, bool? _hasOutHospital,
            List<HIS_TRANS_REQ> _lstTransReq, List<HIS_CONFIG> _lstConfig,
            Action<int> countData,
            Action<int, Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> savedData,
            Action<string> cancelPrint)
        {
            try
            {
                this.currentModule = module;
                this.richEditorMain = _richEditorMain;
                this.printNow = printNow;
                this.previewType = previewType;
                this.hasOutHospital = _hasOutHospital;
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
                    hisDhst = new HIS_DHST();

                    List<MOS.EFMODEL.DataModels.HIS_EXP_MEST> ExpMests = null;
                    hisServiceReq_CurentExam = new HIS_SERVICE_REQ();
                    ThreadMedicineADO mediMateIn = null;
                    ThreadMedicineADO mediMateOut = null;

                    if (currentOutPresSDO.ExpMests != null &&
                        currentOutPresSDO.ExpMests.Count > 0)
                    {
                        ExpMests = currentOutPresSDO.ExpMests;
                        mediMateIn = new ThreadMedicineADO(currentOutPresSDO, hasMediMate, this.hasOutHospital);
                        mediMateOut = new ThreadMedicineADO(currentOutPresSDO, hasMediMate, this.hasOutHospital);
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
                        //Thong tin thuoc / vat tu
                        List<ExpMestMedicineSDO> lstMedicineExpmestTypeADO = new List<ExpMestMedicineSDO>();
                        List<ExpMestMedicineSDO> lstMedicineExpmestTypeTuTucADO = new List<ExpMestMedicineSDO>();
                        List<ExpMestMedicineSDO> lstMedicineExpmestTypeThuocHuongThanADO = new List<ExpMestMedicineSDO>();
                        List<ExpMestMedicineSDO> listTThuongNgoaiKho = new List<ExpMestMedicineSDO>();
                        var listGayNghien = new List<ExpMestMedicineSDO>();
                        var listCoChuaDuocChatGN = new List<ExpMestMedicineSDO>();
                        var listCoChuaDuocChatHT = new List<ExpMestMedicineSDO>();
                        var listHuongThan = new List<ExpMestMedicineSDO>();
                        var listLao = new List<ExpMestMedicineSDO>();
                        var listTPCN = new List<ExpMestMedicineSDO>();
                        var listNgoaiVien = new List<ExpMestMedicineSDO>();
                        var listTienChat = new List<ExpMestMedicineSDO>();
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

                        long keyPrintType = Convert.ToInt64(ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__CHE_DO_IN_TACH_DON_THUOC)); //lấy theo cấu hình tài khoản

                        var laoCFG = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO);
                        var _ThuocCoChuaDCGNCFG = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCGN);
                        var _ThuocCoChuaDCHTCFG = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCHT);


                        if (keyPrintType == 0) // nambg keyPrintType != 1
                        {
                            //danh sách thuốc ngoài viện
                            listNgoaiVien = lstMedicineExpmestTypeADO.Where(o => o.IS_OUT_HOSPITAL == IS_TRUE).ToList();
                            //danh sách thuốc tiền chất
                            listTienChat = lstMedicineExpmestTypeADO.Where(o => o.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC).ToList();

                            //Bo thuoc ngoài viện ra khoi danh sach thuoc tong hop
                            if (listNgoaiVien != null && listNgoaiVien.Count > 0)
                            {
                                lstMedicineExpmestTypeADO = lstMedicineExpmestTypeADO.Except(listNgoaiVien).ToList();
                            }

                            //Danh sach thuoc huong than,gây nghiện, 
                            lstMedicineExpmestTypeThuocHuongThanADO = lstMedicineExpmestTypeADO.Where(o =>
                                o.IS_NEUROLOGICAL == IS_TRUE || o.IS_ADDICTIVE == IS_TRUE ||
                                o.IS_FUNCTIONAL_FOOD == IS_TRUE
                                ).ToList();
                            //
                            listHuongThan = lstMedicineExpmestTypeADO.Where(o => o.IS_NEUROLOGICAL == IS_TRUE).ToList();

                            //  Nếu không phải bệnh nhân ngoại trú thì cho phép In gộp thuốc Tiền chất chung phiếu với thuốc Hướng thần
                            if (hisTreatment != null && hisTreatment.TDL_TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
                            {
                                if (listTienChat != null && listTienChat.Count() > 0) listHuongThan.AddRange(listTienChat.ToList());

                                //Bo thuoc tiền chất ra khoi danh sach thuoc tong hop
                                lstMedicineExpmestTypeADO = lstMedicineExpmestTypeADO.Except(listTienChat).ToList();

                            }
                            listGayNghien = lstMedicineExpmestTypeADO.Where(o => o.IS_ADDICTIVE == IS_TRUE).ToList();
                            listTPCN = lstMedicineExpmestTypeADO.Where(o => o.IS_FUNCTIONAL_FOOD == IS_TRUE).ToList();
                            if (laoCFG != null && laoCFG.IS_SEPARATE_PRINTING == 1)
                            {
                                listLao = lstMedicineExpmestTypeADO.Where(o => o.IS_TUBERCULOSIS == IS_TRUE).ToList();
                            }

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

                            //Bo thuoc lao ra khoi danh sach thuoc tong hop
                            if (listLao != null && listLao.Count > 0)
                            {
                                lstMedicineExpmestTypeADO = lstMedicineExpmestTypeADO.Except(listLao).ToList();
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

                            //danh sách thuốc tiền chất
                            listTienChat = lstMedicineExpmestTypeADO.Where(o => o.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC).ToList();

                            //Bo thuoc ngoài viện ra khoi danh sach thuoc tong hop
                            if (listNgoaiVien != null && listNgoaiVien.Count > 0)
                            {
                                lstMedicineExpmestTypeADO = lstMedicineExpmestTypeADO.Except(listNgoaiVien).ToList();
                            }

                            //Danh sach thuoc huong than,gây nghiện
                            lstMedicineExpmestTypeThuocHuongThanADO = lstMedicineExpmestTypeADO.Where(o =>
                                o.IS_NEUROLOGICAL == IS_TRUE || o.IS_ADDICTIVE == IS_TRUE
                                ).ToList();

                            listHuongThan = lstMedicineExpmestTypeADO.Where(o => o.IS_NEUROLOGICAL == IS_TRUE).ToList();
                            //  Nếu không phải bệnh nhân ngoại trú thì cho phép In gộp thuốc Tiền chất chung phiếu với thuốc Hướng thần
                            if (hisTreatment != null && hisTreatment.TDL_TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
                            {
                                if (listTienChat != null && listTienChat.Count() > 0) listHuongThan.AddRange(listTienChat.ToList());

                                //Bo thuoc tiền chất ra khoi danh sach thuoc tong hop
                                lstMedicineExpmestTypeADO = lstMedicineExpmestTypeADO.Except(listTienChat).ToList();

                            }
                            listGayNghien = lstMedicineExpmestTypeADO.Where(o => o.IS_ADDICTIVE == IS_TRUE).ToList();
                            listSPHoTro = lstMedicineExpmestTypeADO.Where(o => o.IS_FUNCTIONAL_FOOD == IS_TRUE || o.Type == VatTu || o.Type == VatTuNgoaiKho).ToList();

                            if (laoCFG != null && laoCFG.IS_SEPARATE_PRINTING == 1)
                            {
                                listLao = lstMedicineExpmestTypeADO.Where(o => o.IS_TUBERCULOSIS == IS_TRUE).ToList();
                            }

                            //Bo thuoc lao ra khoi danh sach thuoc tong hop
                            if (listLao != null && listLao.Count > 0)
                            {
                                lstMedicineExpmestTypeADO = lstMedicineExpmestTypeADO.Except(listLao).ToList();
                                listSPHoTro = ((listSPHoTro != null && listSPHoTro.Count() > 0) ? listSPHoTro.Except(listLao).ToList() : listSPHoTro);
                            }

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
                        else if(keyPrintType == 3)
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

                        var room = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == item.MEDI_STOCK_ID);
                        mediStockName = room != null ? room.MEDI_STOCK_NAME : "";

                        if (currentOutPresSDO.ServiceReqs != null &&
                            currentOutPresSDO.ServiceReqs.Count > 0)
                        { 
                            HisPrescriptionSDOPrintPlus = currentOutPresSDO.ServiceReqs.FirstOrDefault(o => o.SERVICE_REQ_CODE == item.TDL_SERVICE_REQ_CODE || o.ID == item.SERVICE_REQ_ID);
                        }

                        if (HisPrescriptionSDOPrintPlus == null) continue;
                        HIS_TRANS_REQ transReq = null;
                        if (_lstTransReq != null && _lstTransReq.Count > 0)
                        {
                            transReq = _lstTransReq.FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.TRANS_REQ_ID);
                        }

                        if (HisPrescriptionSDOPrintPlus.USE_TIME == null)
                        {
                            HisPrescriptionSDOPrintPlus.USE_TIME = HisPrescriptionSDOPrintPlus.INTRUCTION_TIME;
                        }

                        var bed = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.REQUEST_ROOM_ID);

                        MPS.Processor.Mps000044.PDO.Mps000044ADO mps000044ADO = new MPS.Processor.Mps000044.PDO.Mps000044ADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000044.PDO.Mps000044ADO>(mps000044ADO, vHisPatient);
                        mps000044ADO.EXP_MEST_CODE = item.EXP_MEST_CODE;
                        mps000044ADO.MEDI_STOCK_NAME = room != null ? room.MEDI_STOCK_NAME : "";
                        mps000044ADO.BED_ROOM_NAME = bed != null ? bed.ROOM_NAME : "";

                        var executeDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.EXECUTE_DEPARTMENT_ID);
                        var requestDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.REQUEST_DEPARTMENT_ID);
                        mps000044ADO.EXECUTE_PHONE = executeDepartment != null ? executeDepartment.PHONE : "";
                        mps000044ADO.REQUEST_PHONE = requestDepartment != null ? requestDepartment.PHONE : "";

                        var acsUser = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME == HisPrescriptionSDOPrintPlus.REQUEST_LOGINNAME);
                        mps000044ADO.REQUEST_USER_MOBILE = acsUser != null ? acsUser.MOBILE : "";

                        #region thuong
                        //không có đơn thuốc sẽ gán dữ liệu với ID = -1 để in
                        if (lstMedicineExpmestTypeADO != null && lstMedicineExpmestTypeADO.Count > 0
                            || (ExpMests.Count == 1 && ExpMests.First().ID == -1 && HisPrescriptionSDOPrintPlus.ID == -1))
                        {
                            mps000044ADO.KEY_NAME_TITLES = "";
                            //Lọc thuốc theo thứ tự
                            lstMedicineExpmestTypeADO = lstMedicineExpmestTypeADO.OrderBy(o => o.NUM_ORDER).ToList();
                            List<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO> ExeExpMestMedicineSDO = new List<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO>();
                            foreach (var medimate in lstMedicineExpmestTypeADO)
                            {
                                MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO sdo = new MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO>(sdo, medimate);
                                ExeExpMestMedicineSDO.Add(sdo);
                            }

                            MPS.Processor.Mps000044.PDO.Mps000044PDO mps000044RDO = new MPS.Processor.Mps000044.PDO.Mps000044PDO(
                                vHisPatientTypeAlter,
                                hisDhst,
                                HisPrescriptionSDOPrintPlus,
                                ExeExpMestMedicineSDO,
                                mps000044ADO,
                                hisTreatment,
                                keyOrderListData,
                                item,
                                hisServiceReq_CurentExam,
                                transReq,
                                _lstConfig
                                );

                            Print.PrintData(printTypeCode, fileName, mps000044RDO, printNow, treatmentCode, ref result, this.currentModule != null ? currentModule.RoomId : 0, previewType, lstMedicineExpmestTypeADO.Count, this.SavedData);
                            //PrintData(printTypeCode, fileName, mps000044RDO, printNow, ref result);
                        }
                        #endregion
                        #region in thuong NK
                        if (listTThuongNgoaiKho != null && listTThuongNgoaiKho.Count > 0)
                        {
                            List<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO> ExpMestMedicineSDO = new List<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO>();
                            var group = listTThuongNgoaiKho.GroupBy(o => new { o.MEDICINE_TYPE_ID, o.MEDICINE_TYPE_NAME });
                            foreach (var aitem in group)
                            {
                                MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO ado = new MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO>(ado, aitem.First());
                                ado.AMOUNT = aitem.Sum(o => o.AMOUNT);
                                ExpMestMedicineSDO.Add(ado);
                            }
                            //Lọc thuốc theo thứ tự
                            ExpMestMedicineSDO = ExpMestMedicineSDO.OrderBy(o => o.NUM_ORDER ?? 99999).ToList();
                            mps000044ADO.KEY_NAME_TITLES = "NK";


                            MPS.Processor.Mps000044.PDO.Mps000044PDO mps000044RDO = new MPS.Processor.Mps000044.PDO.Mps000044PDO(
                               vHisPatientTypeAlter,
                               hisDhst,
                               HisPrescriptionSDOPrintPlus,
                               ExpMestMedicineSDO,
                               mps000044ADO,
                               hisTreatment,
                               keyOrderListData,
                               item,
                               hisServiceReq_CurentExam,
                               transReq,
                               _lstConfig
                               );

                            Print.PrintData(printTypeCode, fileName, mps000044RDO, printNow, treatmentCode, ref result, this.currentModule != null ? currentModule.RoomId : 0, previewType, lstMedicineExpmestTypeADO.Count, this.SavedData);
                        }
                        #endregion
                        #region gay nghien
                        if (listGayNghien != null && listGayNghien.Count > 0)
                        {

                            mps000044ADO.KEY_NAME_TITLES = "\"N\"";
                            //Lọc thuốc theo thứ tự
                            listGayNghien = listGayNghien.OrderBy(o => o.NUM_ORDER).ToList();
                            List<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO> ExeExpMestMedicineSDO = new List<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO>();
                            foreach (var medimate in listGayNghien)
                            {
                                MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO sdo = new MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO>(sdo, medimate);
                                ExeExpMestMedicineSDO.Add(sdo);
                            }

                            MPS.Processor.Mps000044.PDO.Mps000044PDO mps000044RDO = new MPS.Processor.Mps000044.PDO.Mps000044PDO(
                                vHisPatientTypeAlter,
                                hisDhst,
                                HisPrescriptionSDOPrintPlus,
                                ExeExpMestMedicineSDO,
                                mps000044ADO,
                                hisTreatment,
                                keyOrderListData,
                                item,
                                hisServiceReq_CurentExam,
                                transReq,
                                _lstConfig
                                );
                            Print.PrintData(printTypeCode, fileName, mps000044RDO, printNow, treatmentCode, ref result, this.currentModule != null ? currentModule.RoomId : 0, previewType, listGayNghien.Count, this.SavedData);
                            //PrintData(printTypeCode, fileName, mps000044RDO, printNow, ref result);


                        }
                        #endregion

                        #region huong Than
                        if (listHuongThan != null && listHuongThan.Count > 0)
                        {
                            mps000044ADO.KEY_NAME_TITLES = "\"H\"";
                            //Lọc thuốc theo thứ tự
                            listHuongThan = listHuongThan.OrderBy(o => o.NUM_ORDER).ToList();
                            List<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO> ExeExpMestMedicineSDO = new List<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO>();
                            foreach (var medimate in listHuongThan)
                            {
                                MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO sdo = new MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO>(sdo, medimate);
                                ExeExpMestMedicineSDO.Add(sdo);
                            }
                            MPS.Processor.Mps000044.PDO.Mps000044PDO mps000044RDO = new MPS.Processor.Mps000044.PDO.Mps000044PDO(
                                vHisPatientTypeAlter,
                                hisDhst,
                                HisPrescriptionSDOPrintPlus,
                                ExeExpMestMedicineSDO,
                                mps000044ADO,
                                hisTreatment,
                                keyOrderListData,
                                item,
                                hisServiceReq_CurentExam,
                                transReq,
                                _lstConfig
                                );

                            Print.PrintData(printTypeCode, fileName, mps000044RDO, printNow, treatmentCode, ref result, this.currentModule != null ? currentModule.RoomId : 0, previewType, listHuongThan.Count, this.SavedData);
                            //PrintData(printTypeCode, fileName, mps000044RDO, printNow, ref result);
                        }
                        #endregion

                        #region lao
                        if (listLao != null && listLao.Count > 0)
                        {
                            mps000044ADO.KEY_NAME_TITLES = "\"L\"";
                            listLao = listLao.OrderBy(o => o.NUM_ORDER).ToList();
                            List<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO> ExeExpMestMedicineSDO = new List<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO>();
                            foreach (var medimate in listLao)
                            {
                                MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO sdo = new MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO>(sdo, medimate);
                                ExeExpMestMedicineSDO.Add(sdo);
                            }

                            MPS.Processor.Mps000044.PDO.Mps000044PDO mps000044RDO = new MPS.Processor.Mps000044.PDO.Mps000044PDO(
                                vHisPatientTypeAlter,
                                hisDhst,
                                HisPrescriptionSDOPrintPlus,
                                ExeExpMestMedicineSDO,
                                mps000044ADO,
                                hisTreatment,
                                keyOrderListData,
                                item,
                                hisServiceReq_CurentExam,
                                transReq,
                                _lstConfig
                               );
                            Print.PrintData(printTypeCode, fileName, mps000044RDO, printNow, treatmentCode, ref result, this.currentModule != null ? currentModule.RoomId : 0, previewType, listLao.Count, this.SavedData);
                        }

                        #endregion

                        #region Thuc Pham Chuc Nang
                        if (listTPCN != null && listTPCN.Count > 0)
                        {
                            mps000044ADO.KEY_NAME_TITLES = "\"TP\"";
                            //Lọc thuốc theo thứ tự
                            listTPCN = listTPCN.OrderBy(o => o.NUM_ORDER).ToList();
                            List<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO> ExeExpMestMedicineSDO = new List<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO>();
                            foreach (var medimate in listTPCN)
                            {
                                MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO sdo = new MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO>(sdo, medimate);
                                ExeExpMestMedicineSDO.Add(sdo);
                            }

                            MPS.Processor.Mps000044.PDO.Mps000044PDO mps000044RDO = new MPS.Processor.Mps000044.PDO.Mps000044PDO(
                                vHisPatientTypeAlter,
                                hisDhst,
                                HisPrescriptionSDOPrintPlus,
                                ExeExpMestMedicineSDO,
                                mps000044ADO,
                                hisTreatment,
                                keyOrderListData,
                                item,
                                hisServiceReq_CurentExam,
                                transReq,
                                _lstConfig
                                );

                            Print.PrintData(printTypeCode, fileName, mps000044RDO, printNow, treatmentCode, ref result, this.currentModule != null ? currentModule.RoomId : 0, previewType, listTPCN.Count, this.SavedData);
                            //PrintData(printTypeCode, fileName, mps000044RDO, printNow, ref result);
                        }
                        #endregion

                        #region in san pham ho tro
                        if (listSPHoTro != null && listSPHoTro.Count > 0)
                        {
                            InSanPhamHoTro();
                        }
                        #endregion

                        #region Phong Xa
                        //if (listPX != null && listPX.Count > 0)
                        //{
                        //    mps000044ADO.KEY_NAME_TITLES = "\"PX\"";
                        //    //Lọc thuốc theo thứ tự
                        //    listPX = listPX.OrderBy(o => o.NUM_ORDER).ToList();
                        //    List<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO> ExeExpMestMedicineSDO = new List<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO>();
                        //    foreach (var medimate in listPX)
                        //    {
                        //        MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO sdo = new MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO();
                        //        Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO>(sdo, medimate);
                        //        ExeExpMestMedicineSDO.Add(sdo);
                        //    }

                        //    MPS.Processor.Mps000044.PDO.Mps000044PDO mps000044RDO = new MPS.Processor.Mps000044.PDO.Mps000044PDO(
                        //        vHisPatientTypeAlter,
                        //        hisDhst,
                        //        HisPrescriptionSDOPrintPlus,
                        //        ExeExpMestMedicineSDO,
                        //        mps000044ADO,
                        //        hisTreatment);

                        //    PrintData(printTypeCode, fileName, mps000044RDO, printNow, ref result);
                        //}
                        #endregion

                        #region Doc
                        //if (listD != null && listD.Count > 0)
                        //{
                        //    mps000044ADO.KEY_NAME_TITLES = "\"D\"";
                        //    //Lọc thuốc theo thứ tự
                        //    listD = listD.OrderBy(o => o.NUM_ORDER).ToList();
                        //    List<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO> ExeExpMestMedicineSDO = new List<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO>();
                        //    foreach (var medimate in listD)
                        //    {
                        //        MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO sdo = new MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO();
                        //        Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO>(sdo, medimate);
                        //        ExeExpMestMedicineSDO.Add(sdo);
                        //    }

                        //    MPS.Processor.Mps000044.PDO.Mps000044PDO mps000044RDO = new MPS.Processor.Mps000044.PDO.Mps000044PDO(
                        //        vHisPatientTypeAlter,
                        //        hisDhst,
                        //        HisPrescriptionSDOPrintPlus,
                        //        ExeExpMestMedicineSDO,
                        //        mps000044ADO,
                        //        hisTreatment);

                        //    PrintData(printTypeCode, fileName, mps000044RDO, printNow, ref result);
                        //}
                        #endregion

                        #region ngoai vien
                        if (listNgoaiVien != null && listNgoaiVien.Count > 0)
                        {
                            mps000044ADO.KEY_NAME_TITLES = "\"NV\"";
                            //Lọc thuốc theo thứ tự
                            listNgoaiVien = listNgoaiVien.OrderBy(o => o.NUM_ORDER).ToList();
                            List<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO> ExeExpMestMedicineSDO = new List<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO>();
                            foreach (var medimate in listNgoaiVien)
                            {
                                MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO sdo = new MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO>(sdo, medimate);
                                ExeExpMestMedicineSDO.Add(sdo);
                            }

                            MPS.Processor.Mps000044.PDO.Mps000044PDO mps000044RDO = new MPS.Processor.Mps000044.PDO.Mps000044PDO(
                                vHisPatientTypeAlter,
                                hisDhst,
                                HisPrescriptionSDOPrintPlus,
                                ExeExpMestMedicineSDO,
                                mps000044ADO,
                                hisTreatment,
                                keyOrderListData,
                                item,
                                hisServiceReq_CurentExam,
                                transReq,
                                _lstConfig
                                );

                            Print.PrintData(printTypeCode, fileName, mps000044RDO, printNow, treatmentCode, ref result, this.currentModule != null ? currentModule.RoomId : 0, previewType, listNgoaiVien.Count, this.SavedData);
                            //PrintData(printTypeCode, fileName, mps000044RDO, printNow, ref result);
                        }
                        #endregion

                        #region thuốc có chứa dược chất GN
                        if (listCoChuaDuocChatGN != null && listCoChuaDuocChatGN.Count > 0)
                        {

                            mps000044ADO.KEY_NAME_TITLES = "Thuốc có chứa dược chất gây nghiện";
                            //Lọc thuốc theo thứ tự
                            listCoChuaDuocChatGN = listCoChuaDuocChatGN.OrderBy(o => o.NUM_ORDER).ToList();
                            List<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO> ExeExpMestMedicineSDO = new List<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO>();
                            foreach (var medimate in listCoChuaDuocChatGN)
                            {
                                MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO sdo = new MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO>(sdo, medimate);
                                ExeExpMestMedicineSDO.Add(sdo);
                            }

                            MPS.Processor.Mps000044.PDO.Mps000044PDO mps000044RDO = new MPS.Processor.Mps000044.PDO.Mps000044PDO(
                                vHisPatientTypeAlter,
                                hisDhst,
                                HisPrescriptionSDOPrintPlus,
                                ExeExpMestMedicineSDO,
                                mps000044ADO,
                                hisTreatment,
                                keyOrderListData,
                                item,
                                hisServiceReq_CurentExam,
                                transReq,
                                _lstConfig
                                );
                            Print.PrintData(printTypeCode, fileName, mps000044RDO, printNow, treatmentCode, ref result, this.currentModule != null ? currentModule.RoomId : 0, previewType, listCoChuaDuocChatGN.Count, this.SavedData);
                            //PrintData(printTypeCode, fileName, mps000044RDO, printNow, ref result);


                        }
                        #endregion

                        #region thuốc có chứa dược chất HT
                        if (listCoChuaDuocChatHT != null && listCoChuaDuocChatHT.Count > 0)
                        {

                            mps000044ADO.KEY_NAME_TITLES = "Thuốc có chứa dược chất hướng thần";
                            //Lọc thuốc theo thứ tự
                            listCoChuaDuocChatHT = listCoChuaDuocChatHT.OrderBy(o => o.NUM_ORDER).ToList();
                            List<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO> ExeExpMestMedicineSDO = new List<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO>();
                            foreach (var medimate in listCoChuaDuocChatHT)
                            {
                                MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO sdo = new MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000044.PDO.ExpMestMedicineSDO>(sdo, medimate);
                                ExeExpMestMedicineSDO.Add(sdo);
                            }

                            MPS.Processor.Mps000044.PDO.Mps000044PDO mps000044RDO = new MPS.Processor.Mps000044.PDO.Mps000044PDO(
                                vHisPatientTypeAlter,
                                hisDhst,
                                HisPrescriptionSDOPrintPlus,
                                ExeExpMestMedicineSDO,
                                mps000044ADO,
                                hisTreatment,
                                keyOrderListData,
                                item,
                                hisServiceReq_CurentExam,
                                transReq,
                                _lstConfig
                                );
                            Print.PrintData(printTypeCode, fileName, mps000044RDO, printNow, treatmentCode, ref result, this.currentModule != null ? currentModule.RoomId : 0, previewType, listCoChuaDuocChatHT.Count, this.SavedData);
                            //PrintData(printTypeCode, fileName, mps000044RDO, printNow, ref result);


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

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
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

        private void InSanPhamHoTro()
        {
            richEditorMain.RunPrintTemplate(MPS.Processor.Mps000353.PDO.Mps000353PDO.PrintTypeCode, DelegateRunPrinter);
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
                        keyOrderListData,
                        this.HisExpMest);

                    Print.PrintData(printTypeCode, fileName, Mps000353RDO, printNow, treatmentCode, ref result, this.currentModule != null ? currentModule.RoomId : 0, previewType, listSPHoTro.Count, this.SavedData);
                    //PrintData(printTypeCode, fileName, Mps000353RDO, printNow, ref result);
                }
            }
            catch (Exception ex)
            {
                this.CancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThreadGetCurrentData(HIS_SERVICE_REQ HisPrescriptionSDOPrintPlus)
        {

            if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Config.KEY_IsPrintPrescriptionNoThread) == "1")
            {
                GetDataPatientType(HisPrescriptionSDOPrintPlus);
                GetDataDhst(HisPrescriptionSDOPrintPlus);
                GetDataExam(HisPrescriptionSDOPrintPlus);
                GetPatient(HisPrescriptionSDOPrintPlus);
                GetTreatment(HisPrescriptionSDOPrintPlus);
            }
            else
            {
                Thread patientType = new Thread(GetDataPatientType);
                Thread dhst = new Thread(GetDataDhst);
                Thread exam = new Thread(GetDataExam);
                Thread patient = new Thread(GetPatient);
                Thread treatment = new Thread(GetTreatment);
                try
                {
                    patientType.Start(HisPrescriptionSDOPrintPlus);
                    exam.Start(HisPrescriptionSDOPrintPlus);
                    dhst.Start(HisPrescriptionSDOPrintPlus);
                    patient.Start(HisPrescriptionSDOPrintPlus);
                    treatment.Start(HisPrescriptionSDOPrintPlus);

                    patientType.Join();
                    exam.Join();
                    dhst.Join();
                    patient.Join();
                    treatment.Join();
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
                GetDhst((HIS_SERVICE_REQ)obj);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDhst(HIS_SERVICE_REQ hIS_SERVICE_REQ)
        {
            try
            {
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataPatientType(object obj)
        {
            try
            {
                GetPatientType((HIS_SERVICE_REQ)obj);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetPatientType(HIS_SERVICE_REQ hIS_SERVICE_REQ)
        {
            try
            {
                //Lấy thông tin thẻ BHYT
                HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
                filter.TreatmentId = hIS_SERVICE_REQ.TREATMENT_ID;
                filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                vHisPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(RequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
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
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
