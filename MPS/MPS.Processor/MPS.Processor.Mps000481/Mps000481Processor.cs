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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000481.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000481
{
    public class Mps000481Processor : AbstractProcessor
    {
         Mps000481PDO rdo;
         List<SereServADO> SereServADOs;
         List<kskGeneralADO> kskGeneralADOs;
         List<TreatmentlADO> TreatmentlADOs;
         List<SereServTeinADO> SereServTeinADOs;

         List<SereServADO> SereServADO_SieuAms = new List<SereServADO>();
         List<SereServADO> SereServADO_DienTims = new List<SereServADO>();
         List<SereServADO> SereServADO_XQuangs = new List<SereServADO>();
         List<SereServADO> SereServADO_CTs = new List<SereServADO>();
         List<SereServADO> SereServADO_MRIs = new List<SereServADO>();
         List<SereServADO> SereServADO_PETs = new List<SereServADO>();
         List<SereServADO> SereServADO_NoiSois = new List<SereServADO>();
         List<SereServADO> SereServADO_MatDoXuongs = new List<SereServADO>();
         List<SereServADO> SereServADO_ThamDoChucNangKhacs = new List<SereServADO>();

         List<SereServADO> SereServADO_HuyetHocs = new List<SereServADO>();
         List<SereServADO> SereServADO_SinhHoas = new List<SereServADO>();
         List<SereServADO> SereServADO_UngThus = new List<SereServADO>();
         List<SereServADO> SereServADO_ViSinhs = new List<SereServADO>();
         List<SereServADO> SereServADO_NuocTieus = new List<SereServADO>();
         List<SereServADO> SereServADO_Phans = new List<SereServADO>();
         List<SereServADO> SereServADO_CoTuCungs = new List<SereServADO>();
         List<SereServADO> SereServADO_GiaiPhauBenhs = new List<SereServADO>();

         List<SereServTeinADO> SereServTeinADO_HuyetHocs = new List<SereServTeinADO>();
         List<SereServTeinADO> SereServTeinADO_SinhHoas = new List<SereServTeinADO>();
         List<SereServTeinADO> SereServTeinADO_UngThus = new List<SereServTeinADO>();
         List<SereServTeinADO> SereServTeinADO_ViSinhs = new List<SereServTeinADO>();
         List<SereServTeinADO> SereServTeinADO_NuocTieus = new List<SereServTeinADO>();
         List<SereServTeinADO> SereServTeinADO_Phans = new List<SereServTeinADO>();
         List<SereServTeinADO> SereServTeinADO_CoTuCungs = new List<SereServTeinADO>();
         List<SereServTeinADO> SereServTeinADO_GiaiPhauBenhs = new List<SereServTeinADO>();

        public Mps000481Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000481PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));
                
                SetSingleKey();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                objectTag.AddObjectData(store, "listTreatment", TreatmentlADOs);

                objectTag.AddObjectData(store, "listSieuAm", SereServADO_SieuAms);
                objectTag.AddObjectData(store, "listDienTim", SereServADO_DienTims);
                objectTag.AddObjectData(store, "listXQuang", SereServADO_XQuangs);
                objectTag.AddObjectData(store, "listCT", SereServADO_CTs);
                objectTag.AddObjectData(store, "listMRI", SereServADO_MRIs);
                objectTag.AddObjectData(store, "listPET", SereServADO_PETs);
                objectTag.AddObjectData(store, "listNoiSoi", SereServADO_NoiSois);
                objectTag.AddObjectData(store, "listMatDoXuong", SereServADO_MatDoXuongs);
                objectTag.AddObjectData(store, "listKhac", SereServADO_ThamDoChucNangKhacs);

                objectTag.AddObjectData(store, "listHuyetHoc", SereServADO_HuyetHocs);
                objectTag.AddObjectData(store, "listSinhHoa", SereServADO_SinhHoas);
                objectTag.AddObjectData(store, "listUngThu", SereServADO_UngThus);
                objectTag.AddObjectData(store, "listViSinh", SereServADO_ViSinhs);
                objectTag.AddObjectData(store, "listNuocTieu", SereServADO_NuocTieus);
                objectTag.AddObjectData(store, "listPhan", SereServADO_Phans);
                objectTag.AddObjectData(store, "listCoTuCung", SereServADO_CoTuCungs);
                objectTag.AddObjectData(store, "listGiaiPhauBenh", SereServADO_GiaiPhauBenhs);

                objectTag.AddObjectData(store, "SereServ_HH", SereServTeinADO_HuyetHocs);
                objectTag.AddObjectData(store, "SereServ_SH", SereServTeinADO_SinhHoas);
                objectTag.AddObjectData(store, "SereServ_UT", SereServTeinADO_UngThus);
                objectTag.AddObjectData(store, "SereServ_VS", SereServTeinADO_ViSinhs);
                objectTag.AddObjectData(store, "SereServ_NT", SereServTeinADO_NuocTieus);
                objectTag.AddObjectData(store, "SereServ_Phan", SereServTeinADO_Phans);
                objectTag.AddObjectData(store, "SereServ_CTC", SereServTeinADO_CoTuCungs);
                objectTag.AddObjectData(store, "SereServ_GPB", SereServTeinADO_GiaiPhauBenhs);

                objectTag.AddRelationship(store, "listHuyetHoc", "SereServ_HH", "ID", "SERE_SERV_ID");
                objectTag.AddRelationship(store, "listSinhHoa", "SereServ_SH", "ID", "SERE_SERV_ID");
                objectTag.AddRelationship(store, "listUngThu", "SereServ_UT", "ID", "SERE_SERV_ID");
                objectTag.AddRelationship(store, "listViSinh", "SereServ_VS", "ID", "SERE_SERV_ID");
                objectTag.AddRelationship(store, "listNuocTieu", "SereServ_NT", "ID", "SERE_SERV_ID");
                objectTag.AddRelationship(store, "listPhan", "SereServ_Phan", "ID", "SERE_SERV_ID");
                objectTag.AddRelationship(store, "listCoTuCung", "SereServ_CTC", "ID", "SERE_SERV_ID");
                objectTag.AddRelationship(store, "listGiaiPhauBenh", "SereServ_GPB", "ID", "SERE_SERV_ID");

                objectTag.AddRelationship(store, "listTreatment", "listSieuAm", "ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "listTreatment", "listDienTim", "ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "listTreatment", "listXQuang", "ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "listTreatment", "listCT", "ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "listTreatment", "listMRI", "ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "listTreatment", "listPET", "ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "listTreatment", "listNoiSoi", "ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "listTreatment", "listMatDoXuong", "ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "listTreatment", "listKhac", "ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "listTreatment", "listHuyetHoc", "ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "listTreatment", "listSinhHoa", "ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "listTreatment", "listUngThu", "ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "listTreatment", "listViSinh", "ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "listTreatment", "listNuocTieu", "ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "listTreatment", "listPhan", "ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "listTreatment", "listCoTuCung", "ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "listTreatment", "listGiaiPhauBenh", "ID", "TDL_TREATMENT_ID");

                objectTag.AddRelationship(store, "listTreatment", "SereServ_HH", "ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "listTreatment", "SereServ_SH", "ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "listTreatment", "SereServ_UT", "ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "listTreatment", "SereServ_VS", "ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "listTreatment", "SereServ_NT", "ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "listTreatment", "SereServ_Phan", "ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "listTreatment", "SereServ_CTC", "ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "listTreatment", "SereServ_GPB", "ID", "TDL_TREATMENT_ID");

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void SetSingleKey()
        {
            try
            {
                SereServADOs = new List<SereServADO>();
                kskGeneralADOs = new List<kskGeneralADO>();
                TreatmentlADOs = new List<TreatmentlADO>();

                SereServTeinADOs = new List<SereServTeinADO>();

                if (rdo.SereServs != null && rdo.SereServs.Count > 0)
                {
                    foreach (var itemSS in rdo.SereServs)
                    {
                        HIS_SERVICE service = new HIS_SERVICE();
                        HIS_SERE_SERV_EXT SereSErvExt = new HIS_SERE_SERV_EXT();

                        if (rdo.HisServices != null && rdo.HisServices.Count > 0)
                        {
                            service = rdo.HisServices.Where(o => o.ID == itemSS.SERVICE_ID).FirstOrDefault();
                        }

                        if (rdo.SereSErvExts != null && rdo.SereSErvExts.Count > 0)
                        {
                            SereSErvExt = rdo.SereSErvExts.Where(o => o.SERE_SERV_ID == itemSS.ID).FirstOrDefault();
                            if (SereSErvExt != null)
                            {
                                SereServADO ado = new SereServADO(itemSS, service, SereSErvExt);
                                SereServADOs.Add(ado);
                            }

                        }

                        if (!SereServADOs.Select(o => o.ID).Contains(itemSS.ID))
                        {
                            SereServADO ado = new SereServADO(itemSS, service);
                            SereServADOs.Add(ado);
                        }
                    }

                    if (rdo.SereServTeins != null && rdo.SereServTeins.Count > 0)
                    {
                        foreach (var itemTein in rdo.SereServTeins)
                        {
                            var TestIndex = rdo.TestIndexs.Where(o => o.ID == itemTein.TEST_INDEX_ID).FirstOrDefault();

                            SereServTeinADO ado = new SereServTeinADO(itemTein, TestIndex);

                            SereServTeinADOs.Add(ado);
                        }
                    }

                    this.SereServADO_SieuAms = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA, null, null, null); //Siêu âm
                    this.SereServADO_DienTims = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN, 1, null, null); //Điện tâm đồ (Điện tim)
                    this.SereServADO_XQuangs = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA, null, 1, null); //XQuang
                    this.SereServADO_CTs = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA, null, 2, null); //Chụp cắt lớp vi tính (CT)
                    this.SereServADO_MRIs = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA, null, 3, null); //Chụp cộng hưởng từ (MRI)
                    this.SereServADO_PETs = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA, null, 4, null); //PET/CT
                    this.SereServADO_NoiSois = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS, null, null, null); // Nội soi
                    this.SereServADO_MatDoXuongs = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN, 4, null, null); // Đo mật độ xương
                    this.SereServADO_ThamDoChucNangKhacs = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN, 0, null, null); // Các thăm dò chức năng khác


                    this.SereServADO_HuyetHocs = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN, null, null, 1); // Huyết học
                    this.SereServTeinADO_HuyetHocs = getSereServTeinADOADO(1);
                    this.SereServADO_SinhHoas = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN, null, null, 3); // Sinh hóa
                    this.SereServTeinADO_SinhHoas = getSereServTeinADOADO(null);
                    this.SereServADO_UngThus = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN, null, null, 8); // Dấu ấn ung thư
                    this.SereServTeinADO_UngThus = getSereServTeinADOADO(null);

                    this.SereServADO_ViSinhs = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN, null, null, 2); //Vi sinh , miễn dịch
                    this.SereServADO_ViSinhs.AddRange(getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN, null, null, 4));
                    this.SereServTeinADO_ViSinhs = getSereServTeinADOADO(null);

                    this.SereServADO_NuocTieus = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN, null, null, 7); // Nước tiểu
                    this.SereServTeinADO_NuocTieus = getSereServTeinADOADO(1);
                    this.SereServADO_Phans = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN, null, null, 9); // Phân
                    this.SereServTeinADO_Phans = getSereServTeinADOADO(null);
                    this.SereServADO_CoTuCungs = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN, null, null, 10); // Phiến đồ âm đạo, Cổ tử cung
                    this.SereServTeinADO_CoTuCungs = getSereServTeinADOADO(null);
                    this.SereServADO_GiaiPhauBenhs = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN, null, null, 6); // Giải phẫu bệnh
                    this.SereServTeinADO_GiaiPhauBenhs = getSereServTeinADOADO(null);
                }

                if (rdo.ksk_Generals != null && rdo.ksk_Generals.Count > 0)
                {
                    var lstkskGeneralADO = (from r in rdo.ksk_Generals select new kskGeneralADO(r, rdo.ServiceReqs, rdo.HealthExamRanks)).ToList();

                    var kskGeneralADOGroup = lstkskGeneralADO.GroupBy(o => o.TREATMENT_ID).ToList();
                    foreach (var itemG in kskGeneralADOGroup)
                    {
                        kskGeneralADO ado = new kskGeneralADO();

                        ado.TREATMENT_ID = itemG.FirstOrDefault().TREATMENT_ID;
                        ado.DISEASES = String.Join(";", itemG.Select(o => o.DISEASES).ToList());
                        ado.TREATMENT_INSTRUCTION = String.Join(";", itemG.Select(o => o.TREATMENT_INSTRUCTION).ToList());
                        ado.HEALTH_EXAM_RANK_CODE = String.Join(";", itemG.Select(o => o.HEALTH_EXAM_RANK_CODE).ToList());
                        ado.HEALTH_EXAM_RANK_NAME = String.Join(";", itemG.Select(o => o.HEALTH_EXAM_RANK_NAME).ToList());

                        kskGeneralADOs.Add(ado);
                    }
                }

                if (rdo.Treatments != null && rdo.Treatments.Count > 0)
                {
                    TreatmentlADOs = (from r in rdo.Treatments select new TreatmentlADO(r, rdo.HisPositions, kskGeneralADOs)).ToList();
                }

                Inventec.Common.Logging.LogSystem.Info("rdo.SereServs: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo.SereServs), rdo.SereServs));
                Inventec.Common.Logging.LogSystem.Info("rdo.HisServices: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo.HisServices), rdo.HisServices));
                Inventec.Common.Logging.LogSystem.Info("SereServADOs: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => SereServADOs), SereServADOs));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<SereServTeinADO> getSereServTeinADOADO(long? IsImportant)
        {
            List<SereServTeinADO> lstSereServTeinADO = new List<SereServTeinADO>();
            try
            {
                if (SereServTeinADOs != null && SereServTeinADOs.Count > 0)
                {
                    if (IsImportant != null)
                    {
                        lstSereServTeinADO = SereServTeinADOs.Where(o => o.IS_IMPORTANT == IsImportant).ToList();
                    }
                    else
                    {
                        lstSereServTeinADO = SereServTeinADOs;
                    }
                }
            }
            catch (Exception ex)
            {
                return new List<SereServTeinADO>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return lstSereServTeinADO;
        }

        private List<SereServADO> getSereServADO(long ServiceTypeId, long? FuexTypeId, long? DiimTypeId, long? TestTypeId)
        {
            List<SereServADO> lstSereServADO = new List<SereServADO>();
            try
            {
                if (this.SereServADOs != null && this.SereServADOs.Count > 0)
                {
                    lstSereServADO = this.SereServADOs.Where(o => o.TDL_SERVICE_TYPE_ID == ServiceTypeId).ToList();
                }

                if (lstSereServADO != null && lstSereServADO.Count > 0)
                {
                    if (FuexTypeId != null)
                    {
                        if (FuexTypeId > 0)
                        {
                            lstSereServADO = lstSereServADO.Where(o => o.FUEX_TYPE_ID == FuexTypeId).ToList();
                        }
                        else 
                        {
                            lstSereServADO = lstSereServADO.Where(o => o.FUEX_TYPE_ID == null).ToList();
                        }
                    }

                    if (DiimTypeId != null)
                    {
                        lstSereServADO = lstSereServADO.Where(o => o.DIIM_TYPE_ID == DiimTypeId).ToList();
                    }

                    if (TestTypeId != null)
                    {
                        lstSereServADO = lstSereServADO.Where(o => o.TEST_TYPE_ID == TestTypeId).ToList();
                    }

                    lstSereServADO = lstSereServADO.OrderBy(o => o.NUM_ORDER ?? 999999999999).ToList();
                }
            }
            catch (Exception ex)
            {
                return new List<SereServADO>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return lstSereServADO;
        }

        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                if (rdo.Treatments != null && rdo.Treatments.Count == 1)
                {
                    var Treatment = rdo.Treatments.FirstOrDefault();

                    log = "TREATMENT_CODE: " + Treatment.TREATMENT_CODE;

                    decimal SereServCount = 0;

                    foreach (var item in rdo.SereServs.ToList())
                    {
                        SereServCount += item.AMOUNT;
                    }

                    log += " SERE_SERV: " + SereServCount;
                }
            }
            catch (Exception ex)
            {
                log = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return log;
        }

        public override string ProcessUniqueCodeData()
        {
            string result = "";
            try
            {
                if (rdo.Treatments != null && rdo.Treatments.Count == 1)
                {
                    var Treatment = rdo.Treatments.FirstOrDefault();

                    string treatmentCode = "TREATMENT_CODE: " + Treatment.TREATMENT_CODE;

                    decimal SereServCount = 0;

                    foreach (var item in rdo.SereServs.ToList())
                    {
                        SereServCount += item.AMOUNT;
                    }
                    string SereServ = " SERE_SERV: " + SereServCount;

                    result = String.Format("{0} {1} {2}", printTypeCode, treatmentCode, SereServ);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

    }

}
