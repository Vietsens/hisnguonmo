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
using DevExpress.XtraBars;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.PrintPrescription;
using HIS.Desktop.Plugins.TreatmentList.Base;
using HIS.Desktop.Plugins.TreatmentList.Config;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.RichEditor.DAL;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentList
{
    public partial class UCTreatmentList : UserControlBase
    {
        Inventec.Common.RichEditor.RichEditorStore richEditorMain;

        private void LoadPrintTreatment(DevExpress.XtraBars.BarManager barManager)
        {
            try
            {
                richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumerStore.SarConsumer, UriBaseStore.URI_API_SAR, LanguageManager.GetLanguage(), Inventec.Desktop.Common.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                V_HIS_TREATMENT_4 treatment = gridViewtreatmentList.GetFocusedRow() as V_HIS_TREATMENT_4;
                PopupMenu menu = new PopupMenu(barManager);
                menu.ItemLinks.Clear();

                BarButtonItem itemHenKham = new BarButtonItem(barManager, "Phiếu hẹn khám");
                itemHenKham.ItemClick += new ItemClickEventHandler(GiayHenKhamClick);

                BarButtonItem itemHenMo = new BarButtonItem(barManager, "Phiếu hẹn mổ");
                itemHenMo.ItemClick += new ItemClickEventHandler(GiayHenMoClick);

                BarButtonItem itemChuyenVien = new BarButtonItem(barManager, "Phiếu chuyển viện");
                itemChuyenVien.ItemClick += new ItemClickEventHandler(GiayChuyenVienClick);

                BarButtonItem itemRaVien = new BarButtonItem(barManager, "Phiếu ra viện");
                itemRaVien.ItemClick += new ItemClickEventHandler(GiayRaVienClick);

                BarButtonItem itemBenhAnNgoaiTru = new BarButtonItem(barManager, "Bệnh án ngoại trú");
                itemBenhAnNgoaiTru.ItemClick += new ItemClickEventHandler(InBenhAnNgoaiTruClick);

                BarButtonItem itemKhamBenhVaoVien = new BarButtonItem(barManager, "Giấy khám bệnh vào viện");
                itemKhamBenhVaoVien.ItemClick += new ItemClickEventHandler(GiayKhamBenhVaoVienClick);

                BarButtonItem itemGiayPTTT = new BarButtonItem(barManager, "Giấy chứng nhận phẫu thuật");
                itemGiayPTTT.ItemClick += new ItemClickEventHandler(GiayPTTTClick);

                BarButtonItem itemGiayTT = new BarButtonItem(barManager, "Giấy chứng nhận thủ thuật");
                itemGiayTT.ItemClick += new ItemClickEventHandler(GiayTTClick);

                BarButtonItem itemGiayNghiOm = new BarButtonItem(barManager, "Giấy nghỉ việc hưởng bhxh");
                itemGiayNghiOm.ItemClick += new ItemClickEventHandler(GiayNghiOmClick);

                BarButtonItem itemGiayTuVong = new BarButtonItem(barManager, "Giấy tử vong");
                itemGiayTuVong.ItemClick += new ItemClickEventHandler(GiayTuVongClick);

                BarButtonItem itemGiayKetQuaXetNghiem = new BarButtonItem(barManager, "Tóm tắt kết quả CLS (Giấy tổng hợp kết quả xét nghiệm)");
                itemGiayKetQuaXetNghiem.ItemClick += new ItemClickEventHandler(GiayKetQuaXetNghiemClick);

                BarButtonItem itemTheBenhNhan = new BarButtonItem(barManager, "Thẻ bệnh nhân");
                itemTheBenhNhan.ItemClick += new ItemClickEventHandler(TheBenhNhanClick);

                BarButtonItem itemTrichLuc = new BarButtonItem(barManager, "Trích lục");
                itemTrichLuc.ItemClick += new ItemClickEventHandler(InTomTatBenhAnClick);

                BarButtonItem itemInGiayXacNhanBenhNhanCapCuu = new BarButtonItem(barManager, "In giấy xác nhận bệnh nhân cấp cứu");
                itemInGiayXacNhanBenhNhanCapCuu.ItemClick += new ItemClickEventHandler(InGiayXacNhanBenhNhanCapCuuClick);

                if (treatment != null)
                {
                    if (treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN)
                    {
                        menu.AddItems(new BarItem[] { itemChuyenVien });
                    }
                    else if (treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN)
                    {
                        menu.AddItems(new BarItem[] { itemHenKham });
                    }
                    if (treatment.TREATMENT_END_TYPE_EXT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__HEN_MO)
                    {
                        menu.AddItems(new BarItem[] { itemHenMo });
                    }
                    if (treatment.TREATMENT_END_TYPE_ID.HasValue)
                    {
                        menu.AddItems(new BarItem[] { itemRaVien });
                    }
                    if (treatment.TREATMENT_END_TYPE_ID.HasValue
                            && (treatment.TREATMENT_END_TYPE_ID ?? 0) == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__XINRAVIEN
                            && (treatment.TREATMENT_RESULT_ID ?? 0) == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__NANG
                    )
                    {
                        BarButtonItem itemXinVe = new BarButtonItem(barManager, "Phiếu tóm tắt thông tin bệnh nặng xin về");
                        itemXinVe.ItemClick += new ItemClickEventHandler(BenhNangXinVe);
                        menu.AddItems(new BarItem[] { itemXinVe });
                    }
                    if (treatment.TREATMENT_END_TYPE_ID.HasValue
                        && (treatment.TREATMENT_END_TYPE_ID ?? 0) == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET
                        )
                    {
                        BarButtonItem itemTuVong = new BarButtonItem(barManager, "Phiếu chẩn đoán nguyên nhân tử vong");
                        itemTuVong.ItemClick += new ItemClickEventHandler(ThongTinTuVong);
                        menu.AddItems(new BarItem[] { itemTuVong });
                    }
                    if (treatment.OUT_TIME != null)
                    {
                        menu.AddItems(new BarItem[] { itemGiayPTTT, itemGiayTT });
                    }

                    if (treatment.IS_PAUSE == 1 && treatment.TREATMENT_END_TYPE_EXT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM)
                        menu.AddItems(new BarItem[] { itemGiayNghiOm });
                    if (treatment.IS_PAUSE == 1 && treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET)
                        menu.AddItems(new BarItem[] { itemGiayTuVong });
                    BarButtonItem itemGiayXacNhanNoiTru = new BarButtonItem(barManager, "Giấy xác nhận điều trị nội trú");
                    itemGiayXacNhanNoiTru.ItemClick += new ItemClickEventHandler(GiayXacNhanDieuTriNoiTruClick);
                    if (treatment.IS_PAUSE == 1 && treatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                        menu.AddItems(new BarItem[] { itemGiayXacNhanNoiTru });
                }

                BarButtonItem itemHoSoQuanLySucKhoeCaNhan = new BarButtonItem(barManager, "Hồ sơ quản lý sức khỏe cá nhân");
                itemHoSoQuanLySucKhoeCaNhan.ItemClick += new ItemClickEventHandler(HoSoQuanLySucKhoeCaNhan);

                BarButtonItem itemBieuMauKhac = new BarButtonItem(barManager, "Biểu mẫu khác");
                itemBieuMauKhac.ItemClick += new ItemClickEventHandler(BieuMauKhacClick);

                BarButtonItem itemDonThuoc = new BarButtonItem(barManager, "In đơn thuốc");
                itemDonThuoc.ItemClick += new ItemClickEventHandler(InDonThuoc);

                menu.AddItems(new BarItem[] { itemKhamBenhVaoVien, itemBenhAnNgoaiTru, itemGiayKetQuaXetNghiem, itemTheBenhNhan, itemHoSoQuanLySucKhoeCaNhan, itemBieuMauKhac, itemTrichLuc });
                if (treatment != null && treatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                    menu.AddItems(new BarItem[] { itemDonThuoc });

                if (treatment.IS_EMERGENCY == 1 && treatment.TDL_PATIENT_TYPE_ID == Config.HisConfigCFG.PatientTypeId__BHYT)
                {
                    menu.AddItems(new BarItem[] { itemInGiayXacNhanBenhNhanCapCuu });
                }

                BarButtonItem itemDonThuocVaPTTT = new BarButtonItem(barManager, "Tóm tắt y lệnh phẫu thuật thủ thuật và đơn thuốc");
                itemDonThuocVaPTTT.ItemClick += new ItemClickEventHandler(InDonThuocPTTT);
                menu.AddItems(new BarItem[] { itemDonThuocVaPTTT });

                menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataInDonThuoc(V_HIS_TREATMENT_4 treatment)
        {
            try
            {
                CommonParam param = new CommonParam();
                //Load expmest
                HisExpMestFilter expMestFilter = new HisExpMestFilter();
                expMestFilter.TDL_TREATMENT_ID = treatment.ID;
                List<HIS_EXP_MEST> expMests = new BackendAdapter(param)
                     .Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, expMestFilter, param);

                List<HIS_EXP_MEST> expMestsFake = new List<HIS_EXP_MEST>();
                List<HIS_SERVICE_REQ> ServiceReqFake = new List<HIS_SERVICE_REQ>();
                if ((expMests == null || expMests.Count == 0) || (expMests != null && (expMests.Where(o => o.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK).ToList() == null || expMests.Where(o => o.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK).ToList().Count == 0)))
                {
                    if (!HisConfigCFG.IsAllowPrintNoMedicine)
                    {
                        return;
                    }
                    else
                    {
                        HIS_EXP_MEST obj = new HIS_EXP_MEST();
                        //Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST>(obj, treatment);
                        obj.ID = -1;
                        obj.SERVICE_REQ_ID = -1;
                        expMestsFake.Add(obj);

                        HIS_SERVICE_REQ hIS_SERVICE_REQ = new HIS_SERVICE_REQ();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(hIS_SERVICE_REQ, treatment);
                        hIS_SERVICE_REQ.ID = -1;
                        hIS_SERVICE_REQ.TREATMENT_ID = treatment.ID;
                        hIS_SERVICE_REQ.REQUEST_ROOM_ID = currentModule.RoomId;
                        ServiceReqFake.Add(hIS_SERVICE_REQ);
                    }
                }



                HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                serviceReqFilter.TREATMENT_ID = treatment.ID;
                List<HIS_SERVICE_REQ> serviceReqs = new BackendAdapter(param)
                     .Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, serviceReqFilter, param);

                //Lays thuoc vat tu trong kho
                IEnumerable<IGrouping<long?, HIS_EXP_MEST>> expMestGroups = null;
                List<HIS_EXP_MEST_MEDICINE> expMestMedicines = null;
                List<HIS_EXP_MEST_MATERIAL> expMestMaterials = null;
                if (expMests != null && expMests.Count > 0)
                {
                    HisExpMestMedicineFilter expMestMedicineFilter = new HisExpMestMedicineFilter();
                    expMestMedicineFilter.EXP_MEST_IDs = expMests.Select(o => o.ID).ToList();
                    expMestMedicines = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/Get", ApiConsumers.MosConsumer, expMestMedicineFilter, param);

                    HisExpMestMaterialFilter expMestMaterialFilter = new HisExpMestMaterialFilter();
                    expMestMaterialFilter.EXP_MEST_IDs = expMests.Select(o => o.ID).ToList();
                    expMestMaterials = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/Get", ApiConsumers.MosConsumer, expMestMaterialFilter, param);

                    expMestGroups = expMests.Where(o => o.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK).GroupBy(o => o.AGGR_EXP_MEST_ID);
                }
                string printTypeCode = HisConfigCFG.MPS_PrintPrescription;
                if (string.IsNullOrEmpty(printTypeCode))
                {
                    printTypeCode = "Mps000234";

                }

                if (expMestGroups != null && expMestGroups.ToList().Count > 0)
                {
                    if (printTypeCode == "Mps000234")
                    {
                        List<OutPatientPresResultSDO> OutPatientPresResultSDOForPrints = new List<OutPatientPresResultSDO>();
                        if ((expMestMedicines != null && expMestMedicines.Count > 0)
                                    || (expMestMaterials != null && expMestMaterials.Count > 0))
                        {
                            OutPatientPresResultSDO outPatientPresResultSDO = new OutPatientPresResultSDO();
                            outPatientPresResultSDO.ExpMests = expMests;
                            outPatientPresResultSDO.ServiceReqs = serviceReqs;
                            outPatientPresResultSDO.Medicines = expMestMedicines;
                            outPatientPresResultSDO.Materials = expMestMaterials;
                            OutPatientPresResultSDOForPrints.Add(outPatientPresResultSDO);
                        }
                        PrintPrescriptionProcessor printPrescriptionProcessor = new PrintPrescriptionProcessor(OutPatientPresResultSDOForPrints, this.currentModule);

                        printPrescriptionProcessor.Print("Mps000234", false);
                    }
                    else
                    {
                        #region Có ExpMest
                        foreach (var listExpMest in expMestGroups)
                        {
                            if (listExpMest.First().AGGR_EXP_MEST_ID == null)
                            {
                                foreach (var expMest in listExpMest)
                                {
                                    List<long> serviceReqIdTemps = new List<long> { expMest.SERVICE_REQ_ID ?? 0 };
                                    List<long> expMestIdTemps = new List<long> { expMest.ID };
                                    List<HIS_SERVICE_REQ> serviceReqTemps = serviceReqs.Where(o => serviceReqIdTemps.Contains(o.ID)).ToList();
                                    List<HIS_EXP_MEST_MEDICINE> expMestMedicineTemps = expMestMedicines.Where(o => expMestIdTemps.Contains(o.EXP_MEST_ID ?? 0)).ToList();
                                    List<HIS_EXP_MEST_MATERIAL> expMestMaterialTemps = expMestMaterials.Where(o => expMestIdTemps.Contains(o.EXP_MEST_ID ?? 0)).ToList();

                                    List<OutPatientPresResultSDO> OutPatientPresResultSDOForPrints = new List<OutPatientPresResultSDO>();
                                    if ((expMestMedicineTemps != null && expMestMedicineTemps.Count > 0)
                                                || (expMestMaterialTemps != null && expMestMaterialTemps.Count > 0))
                                    {
                                        OutPatientPresResultSDO outPatientPresResultSDO = new OutPatientPresResultSDO();
                                        outPatientPresResultSDO.ExpMests = new List<HIS_EXP_MEST> { expMest };
                                        outPatientPresResultSDO.ServiceReqs = serviceReqTemps;
                                        outPatientPresResultSDO.Medicines = expMestMedicineTemps;
                                        outPatientPresResultSDO.Materials = expMestMaterialTemps;
                                        OutPatientPresResultSDOForPrints.Add(outPatientPresResultSDO);
                                    }
                                    PrintPrescriptionProcessor printPrescriptionProcessor = new PrintPrescriptionProcessor(OutPatientPresResultSDOForPrints, expMest, this.currentModule);
                                    printPrescriptionProcessor.Print(printTypeCode, false);
                                }
                            }
                            else
                            {
                                HIS_EXP_MEST expMestPrimary = expMests.FirstOrDefault(o => o.ID == listExpMest.First().AGGR_EXP_MEST_ID);
                                List<long> serviceReqIdTemps = listExpMest.Select(o => o.SERVICE_REQ_ID ?? 0).ToList();
                                List<long> expMestIdTemps = listExpMest.Select(o => o.ID).ToList();

                                List<HIS_SERVICE_REQ> serviceReqTemps = serviceReqs.Where(o => serviceReqIdTemps.Contains(o.ID)).ToList();
                                List<HIS_EXP_MEST_MEDICINE> expMestMedicineTemps = expMestMedicines.Where(o => expMestIdTemps.Contains(o.EXP_MEST_ID ?? 0)).ToList();
                                List<HIS_EXP_MEST_MATERIAL> expMestMaterialTemps = expMestMaterials.Where(o => expMestIdTemps.Contains(o.EXP_MEST_ID ?? 0)).ToList();

                                List<OutPatientPresResultSDO> OutPatientPresResultSDOForPrints = new List<OutPatientPresResultSDO>();
                                if ((expMestMedicineTemps != null && expMestMedicineTemps.Count > 0)
                                            || (expMestMaterialTemps != null && expMestMaterialTemps.Count > 0))
                                {
                                    OutPatientPresResultSDO outPatientPresResultSDO = new OutPatientPresResultSDO();
                                    outPatientPresResultSDO.ExpMests = listExpMest.ToList();
                                    outPatientPresResultSDO.ServiceReqs = serviceReqTemps;
                                    outPatientPresResultSDO.Medicines = expMestMedicineTemps;
                                    outPatientPresResultSDO.Materials = expMestMaterialTemps;
                                    OutPatientPresResultSDOForPrints.Add(outPatientPresResultSDO);
                                }
                                PrintPrescriptionProcessor printPrescriptionProcessor = new PrintPrescriptionProcessor(OutPatientPresResultSDOForPrints, expMestPrimary, this.currentModule);
                                printPrescriptionProcessor.Print(printTypeCode, false);
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    List<OutPatientPresResultSDO> OutPatientPresResultSDOForPrints = new List<OutPatientPresResultSDO>();

                    OutPatientPresResultSDO outPatientPresResultSDO = new OutPatientPresResultSDO();
                    outPatientPresResultSDO.ExpMests = expMestsFake;
                    outPatientPresResultSDO.ServiceReqs = ServiceReqFake;
                    outPatientPresResultSDO.Medicines = new List<HIS_EXP_MEST_MEDICINE>();
                    outPatientPresResultSDO.Materials = new List<HIS_EXP_MEST_MATERIAL>();
                    OutPatientPresResultSDOForPrints.Add(outPatientPresResultSDO);

                    PrintPrescriptionProcessor printPrescriptionProcessor = new PrintPrescriptionProcessor(OutPatientPresResultSDOForPrints, expMestsFake.First(), this.currentModule);
                    printPrescriptionProcessor.Print(printTypeCode, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InDonThuoc(object sender, ItemClickEventArgs e)
        {
            try
            {
                V_HIS_TREATMENT_4 treatment = gridViewtreatmentList.GetFocusedRow() as V_HIS_TREATMENT_4;
                SetDataInDonThuoc(treatment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPrintTreatment(DevExpress.XtraBars.BarManager barManager, V_HIS_TREATMENT_4 data)
        {
            try
            {
                richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumerStore.SarConsumer, UriBaseStore.URI_API_SAR, LanguageManager.GetLanguage(), Inventec.Desktop.Common.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                V_HIS_TREATMENT_4 treatment = data;
                PopupMenu menu = new PopupMenu(barManager);
                menu.ItemLinks.Clear();

                BarButtonItem itemHenKham = new BarButtonItem(barManager, "Phiếu hẹn khám");
                itemHenKham.ItemClick += new ItemClickEventHandler(GiayHenKhamClick);


                BarButtonItem itemHenMo = new BarButtonItem(barManager, "Phiếu hẹn mổ");
                itemHenMo.ItemClick += new ItemClickEventHandler(GiayHenMoClick);

                BarButtonItem itemGiayXacNhanNoiTru = new BarButtonItem(barManager, "Giấy xác nhận điều trị nội trú");
                itemGiayXacNhanNoiTru.ItemClick += new ItemClickEventHandler(GiayXacNhanDieuTriNoiTruClick);

                BarButtonItem itemChuyenVien = new BarButtonItem(barManager, "Phiếu chuyển viện");

                itemChuyenVien.ItemClick += new ItemClickEventHandler(GiayChuyenVienClick);

                BarButtonItem itemRaVien = new BarButtonItem(barManager, "Phiếu ra viện");
                itemRaVien.ItemClick += new ItemClickEventHandler(GiayRaVienClick);

                BarButtonItem itemBenhAnNgoaiTru = new BarButtonItem(barManager, "Bệnh án ngoại trú");
                itemBenhAnNgoaiTru.ItemClick += new ItemClickEventHandler(InBenhAnNgoaiTruClick);

                BarButtonItem itemKhamBenhVaoVien = new BarButtonItem(barManager, "Giấy khám bệnh vào viện");
                itemKhamBenhVaoVien.ItemClick += new ItemClickEventHandler(GiayKhamBenhVaoVienClick);

                BarButtonItem itemGiayNghiOm = new BarButtonItem(barManager, "Giấy nghỉ việc hưởng BHXH");
                itemGiayNghiOm.ItemClick += new ItemClickEventHandler(GiayNghiOmClick);

                BarButtonItem itemGiayTuVong = new BarButtonItem(barManager, "Giấy tử vong");
                itemGiayTuVong.ItemClick += new ItemClickEventHandler(GiayTuVongClick);

                BarButtonItem itemGiayPTTT = new BarButtonItem(barManager, "Giấy chứng nhận phẫu thuật");
                itemGiayPTTT.ItemClick += new ItemClickEventHandler(GiayPTTTClick);

                BarButtonItem itemGiayTT = new BarButtonItem(barManager, "Giấy chứng nhận thủ thuật");
                itemGiayTT.ItemClick += new ItemClickEventHandler(GiayTTClick);

                BarButtonItem itemGiayKetQuaXetNghiem = new BarButtonItem(barManager, "Tóm tắt kết quả CLS (Giấy tổng hợp kết quả xét nghiệm)");
                itemGiayKetQuaXetNghiem.ItemClick += new ItemClickEventHandler(GiayKetQuaXetNghiemClick);

                BarButtonItem itemTheBenhNhan = new BarButtonItem(barManager, "Thẻ bệnh nhân");
                itemTheBenhNhan.ItemClick += new ItemClickEventHandler(TheBenhNhanClick);

                BarButtonItem itemTrichLuc = new BarButtonItem(barManager, "Phiếu kết quả khám bệnh (trích lục)");
                itemTrichLuc.ItemClick += new ItemClickEventHandler(InTomTatBenhAnClick);

                BarButtonItem itemInGiayXacNhanBenhNhanCapCuu = new BarButtonItem(barManager, "In giấy xác nhận bệnh nhân cấp cứu");
                itemInGiayXacNhanBenhNhanCapCuu.ItemClick += new ItemClickEventHandler(InGiayXacNhanBenhNhanCapCuuClick);
                if (treatment != null)
                {
                    if (treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN)
                    {
                        menu.AddItems(new BarItem[] { itemChuyenVien });
                    }
                    else if (treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN || treatment.APPOINTMENT_TIME != null)
                    {
                        menu.AddItems(new BarItem[] { itemHenKham });

                    }
                    if (treatment.TREATMENT_END_TYPE_EXT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__HEN_MO)
                    {
                        menu.AddItems(new BarItem[] { itemHenMo });
                    }
                    if (treatment.TREATMENT_END_TYPE_ID.HasValue)
                    {
                        menu.AddItems(new BarItem[] { itemRaVien });
                    }
                    if (treatment.TREATMENT_END_TYPE_ID.HasValue
                       && (treatment.TREATMENT_END_TYPE_ID ?? 0) == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__XINRAVIEN
                       && (treatment.TREATMENT_RESULT_ID ?? 0) == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__NANG
                       )
                    {
                        BarButtonItem itemXinVe = new BarButtonItem(barManager, "Phiếu tóm tắt thông tin bệnh nặng xin về");
                        itemXinVe.ItemClick += new ItemClickEventHandler(BenhNangXinVe);
                        menu.AddItems(new BarItem[] { itemXinVe });
                    }
                    if (treatment.TREATMENT_END_TYPE_ID.HasValue
                        && (treatment.TREATMENT_END_TYPE_ID ?? 0) == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET
                        )
                    {
                        BarButtonItem itemTuVong = new BarButtonItem(barManager, "Phiếu chẩn đoán nguyên nhân tử vong");
                        itemTuVong.ItemClick += new ItemClickEventHandler(ThongTinTuVong);
                        menu.AddItems(new BarItem[] { itemTuVong });
                    }
                    if (treatment.OUT_TIME != null)
                    {
                        menu.AddItems(new BarItem[] { itemGiayPTTT, itemGiayTT });
                    }
                    
                    if (treatment.IS_PAUSE == 1 && treatment.TREATMENT_END_TYPE_EXT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM)
                        menu.AddItems(new BarItem[] { itemGiayNghiOm });
                    if (treatment.IS_PAUSE == 1 && treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET)
                        menu.AddItems(new BarItem[] { itemGiayTuVong });

                }

                BarButtonItem itemHoSoQuanLySucKhoeCaNhan = new BarButtonItem(barManager, "Hồ sơ quản lý sức khỏe cá nhân");
                itemHoSoQuanLySucKhoeCaNhan.ItemClick += new ItemClickEventHandler(HoSoQuanLySucKhoeCaNhan);

                BarButtonItem itemBieuMauKhac = new BarButtonItem(barManager, "Biểu mẫu khác");
                itemBieuMauKhac.ItemClick += new ItemClickEventHandler(BieuMauKhacClick);

                BarButtonItem itemTomTatBA = new BarButtonItem(barManager, "Tóm tắt bệnh án");
                itemTomTatBA.ItemClick += new ItemClickEventHandler(InTomTatBenhAnClick330or331);
                //Inventec.Common.Logging.LogSystem.Info("treatment.IS_PAUSE " + treatment.IS_PAUSE + " treatment.TDL_TREATMENT_TYPE_ID " + treatment.TDL_TREATMENT_TYPE_ID);
                if (treatment.IS_PAUSE == 1 && treatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                    menu.AddItems(new BarItem[] { itemGiayXacNhanNoiTru });

                BarButtonItem itemDonThuoc = new BarButtonItem(barManager, "In đơn thuốc");
                itemDonThuoc.ItemClick += new ItemClickEventHandler(InDonThuoc);
                menu.AddItems(new BarItem[] { itemKhamBenhVaoVien, itemBenhAnNgoaiTru, itemGiayKetQuaXetNghiem, itemTheBenhNhan, itemHoSoQuanLySucKhoeCaNhan, itemBieuMauKhac, itemTomTatBA, itemTrichLuc });
               
                
                if (treatment.IS_EMERGENCY == 1 && treatment.TDL_PATIENT_TYPE_ID == Config.HisConfigCFG.PatientTypeId__BHYT)
                {
                    menu.AddItems(new BarItem[] { itemInGiayXacNhanBenhNhanCapCuu });
                }
                if (treatment != null && treatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                    menu.AddItems(new BarItem[] { itemDonThuoc });

                BarButtonItem itemDonThuocVaPTTT = new BarButtonItem(barManager, "Tóm tắt y lệnh phẫu thuật thủ thuật và đơn thuốc");
                itemDonThuocVaPTTT.ItemClick += new ItemClickEventHandler(InDonThuocPTTT);
                menu.AddItems(new BarItem[] { itemDonThuocVaPTTT });


                menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

		private void InDonThuocPTTT(object sender, ItemClickEventArgs e)
		{
			try
			{
                currentTreatmentPrint = gridViewtreatmentList.GetFocusedRow() as V_HIS_TREATMENT_4;
                richEditorMain.RunPrintTemplate("Mps000478", DelegateRunPrinter);
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
 		}

		private void InTomTatBenhAnClick330or331(object sender, ItemClickEventArgs e)
        {
            try
            {
                V_HIS_TREATMENT_4 treatment4 = gridViewtreatmentList.GetFocusedRow() as V_HIS_TREATMENT_4;
                if (treatment4 != null)
                {
                    if (treatment4.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                    {
                        richEditorMain.RunPrintTemplate("Mps000330", DelegateRunPrinter);
                    }
                    else if (treatment4.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
                    {
                        richEditorMain.RunPrintTemplate("Mps000331", DelegateRunPrinter);
                    }
                    else
                    {
                        InTomTatBenhAnClick(null, null);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BieuMauKhacClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "SAR.Desktop.Plugins.SarPrintList").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = SAR.Desktop.Plugins.SarPrintList");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    V_HIS_TREATMENT_4 treatment4 = gridViewtreatmentList.GetFocusedRow() as V_HIS_TREATMENT_4;
                    HIS.Desktop.ADO.SarPrintADO sarPrint = new SarPrintADO();
                    sarPrint.JSON_PRINT_ID = treatment4.JSON_PRINT_ID;
                    sarPrint.JsonPrintResult = JsonPrintResult;
                    sarPrint.IsFinished = treatment4.IS_PAUSE == 1 ? true : false;
                    listArgs.Add(sarPrint);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void JsonPrintResult(object data)
        {
            try
            {
                if (data.GetType() == typeof(string))
                {
                    string jsonPrintId = data as string;
                    V_HIS_TREATMENT_4 treatment4 = gridViewtreatmentList.GetFocusedRow() as V_HIS_TREATMENT_4;
                    HIS_TREATMENT treatment = new HIS_TREATMENT();
                    treatment.ID = treatment4.ID;
                    treatment.JSON_PRINT_ID = jsonPrintId;

                    CommonParam param = new CommonParam();
                    var result = new BackendAdapter(param)
                    .Post<MOS.EFMODEL.DataModels.HIS_TREATMENT>("api/HisTreatment/UpdateJsonPrintId", ApiConsumers.MosConsumer, treatment, param);
                    if (result != null)
                    {
                        treatment4.JSON_PRINT_ID = result.JSON_PRINT_ID;
                        gridControlTreatmentList.RefreshDataSource();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void GiayKetQuaXetNghiemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                var treatment = new V_HIS_TREATMENT();
                V_HIS_TREATMENT_4 treatment4 = gridViewtreatmentList.GetFocusedRow() as V_HIS_TREATMENT_4;
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_TREATMENT>(treatment, treatment4);
                var printTest = new HIS.Desktop.Plugins.Library.PrintTestTotal.PrintTestTotalProcessor(this.currentModule.RoomId, treatment);
                if (printTest != null)
                {
                    printTest.Print();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GiayPTTTClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                V_HIS_TREATMENT_4 dataRow = gridViewtreatmentList.GetFocusedRow() as V_HIS_TREATMENT_4;
                if (dataRow != null)
                {
                    //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ListSurgMisuByTreatment").FirstOrDefault();
                    //if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ListSurgMisuByTreatment");
                    //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //{
                    List<object> listArgs = new List<object>();
                    listArgs.Add(dataRow.ID);
                    listArgs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT.ToString());
                    //    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    //    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    //    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    //    ((Form)extenceInstance).ShowDialog();
                    //}
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.ListSurgMisuByTreatment", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GiayTTClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                V_HIS_TREATMENT_4 dataRow = gridViewtreatmentList.GetFocusedRow() as V_HIS_TREATMENT_4;
                if (dataRow != null)
                {
                    //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ListSurgMisuByTreatment").FirstOrDefault();
                    //if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ListSurgMisuByTreatment");
                    //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //{
                    List<object> listArgs = new List<object>();
                    listArgs.Add(dataRow.ID);
                    listArgs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT.ToString());
                    //    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    //    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    //    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    //    ((Form)extenceInstance).ShowDialog();
                    //}
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.ListSurgMisuByTreatment", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GiayRaVienClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                //richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__IN_GIAY_RA_VIEN__MPS000008, DelegateRunPrinter);
                V_HIS_TREATMENT_4 treatment4 = gridViewtreatmentList.GetFocusedRow() as V_HIS_TREATMENT_4;
                var hisTreatment = new HIS_TREATMENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(hisTreatment, treatment4);
                var printProcess = new HIS.Desktop.Plugins.Library.PrintTreatmentFinish.PrintTreatmentFinishProcessor(hisTreatment, currentModule != null ? currentModule.RoomId : 0);
                printProcess.Print(PrintTypeCodeWorker.PRINT_TYPE_CODE__IN_GIAY_RA_VIEN__MPS000008, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InBenhAnNgoaiTruClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_BENH_AN_NGOAI_TRU__MPS000174, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GiayChuyenVienClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                //richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__IN_GIAY_CHUYEN_VIEN__MPS000011, DelegateRunPrinter);
                V_HIS_TREATMENT_4 treatment4 = gridViewtreatmentList.GetFocusedRow() as V_HIS_TREATMENT_4;
                var hisTreatment = new HIS_TREATMENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(hisTreatment, treatment4);
                LoadServiceReq(treatment4.ID);
                var printProcess = new HIS.Desktop.Plugins.Library.PrintTreatmentFinish.PrintTreatmentFinishProcessor(hisTreatment, serviceReqExamEndType, currentModule != null ? currentModule.RoomId : 0);
                printProcess.Print(PrintTypeCodeWorker.PRINT_TYPE_CODE__IN_GIAY_CHUYEN_VIEN__MPS000011, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GiayHenKhamClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                //richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__IN_GIAY_HEN_KHAM__MPS000010, DelegateRunPrinter);
                V_HIS_TREATMENT_4 treatment4 = gridViewtreatmentList.GetFocusedRow() as V_HIS_TREATMENT_4;
                var hisTreatment = new HIS_TREATMENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(hisTreatment, treatment4);
                LoadServiceReq(treatment4.ID);
                var printProcess = new HIS.Desktop.Plugins.Library.PrintTreatmentFinish.PrintTreatmentFinishProcessor(hisTreatment,serviceReqExamEndType, currentModule != null ? currentModule.RoomId : 0);
                printProcess.Print(PrintTypeCodeWorker.PRINT_TYPE_CODE__IN_GIAY_HEN_KHAM__MPS000010, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void GiayHenMoClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                //richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__IN_GIAY_HEN_KHAM__MPS000010, DelegateRunPrinter);
                V_HIS_TREATMENT_4 treatment4 = gridViewtreatmentList.GetFocusedRow() as V_HIS_TREATMENT_4;
                var hisTreatment = new HIS_TREATMENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(hisTreatment, treatment4);
                var printProcess = new HIS.Desktop.Plugins.Library.PrintTreatmentFinish.PrintTreatmentFinishProcessor(hisTreatment, currentModule != null ? currentModule.RoomId : 0);
                printProcess.Print(PrintTypeCodeWorker.PRINT_TYPE_CODE__IN_GIAY_HEN_MO__Mps000389, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GiayKhamBenhVaoVienClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__IN_GIAY_KHAM_BENH_VAO_VIEN__MPS000007, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GiayXacNhanDieuTriNoiTruClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__MPS000399, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GiayNghiOmClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                Mps000269("Mps000269");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GiayTuVongClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                Mps000268("Mps000268");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TheBenhNhanClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__THE_BENH_NHAN__MPS000178, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void HoSoQuanLySucKhoeCaNhan(object sender, ItemClickEventArgs e)
        {
            try
            {
                richEditorMain.RunPrintTemplate("Mps000206", DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__IN_GIAY_RA_VIEN__MPS000008:
                        InGiayRaVien(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__IN_GIAY_HEN_KHAM__MPS000010:
                        InGiayHenKham(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__IN_GIAY_HEN_MO__Mps000389:
                        InGiayHenMo(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__IN_GIAY_CHUYEN_VIEN__MPS000011:
                        InGiayChuyenVien(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_BENH_AN_NGOAI_TRU__MPS000174:
                        LoadBieuMauPhieuYCBenhAnNgoaiTru(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__IN_GIAY_KHAM_BENH_VAO_VIEN__MPS000007:
                        LoadBieuMauKhamBenhVaoVien(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__THE_BENH_NHAN__MPS000178:
                        LoadBieuMauTheBenhNhan(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000206":
                        Mps000206(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000330":
                        Mps000330(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000331":
                        Mps000331(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__MPS000399:
                        LoadBieuMauGiayXacNhanDieuTriNoiTru(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000478":
                        LoadDonthuocPTTT(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000484":
                        ProcessPrintMps000484(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000485":
                        ProcessPrintMps000485(printTypeCode, fileName, ref result);
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

		private void LoadDonthuocPTTT(string printTypeCode, string fileName, ref bool result)
		{
            try
            {
                WaitingManager.Show();
           
                var treatment = new V_HIS_TREATMENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_TREATMENT>(treatment, currentTreatmentPrint);


                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestViewFilter prescriptionViewFIlter = new HisExpMestViewFilter();
                prescriptionViewFIlter.TDL_TREATMENT_ID = treatment.ID;

                var lstExpMest = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, prescriptionViewFIlter, param) ?? new List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST>();
                List<long> expMestIds = null;
                if (lstExpMest != null && lstExpMest.Count > 0)
                {
                    expMestIds = lstExpMest.Select(o => o.ID).Distinct().ToList();
                }
                MOS.Filter.HisExpMestMedicineViewFilter medicineFilter = new MOS.Filter.HisExpMestMedicineViewFilter();
                medicineFilter.TDL_TREATMENT_ID = treatment.ID;
                if (expMestIds != null && expMestIds.Count > 0)
                    medicineFilter.EXP_MEST_IDs = expMestIds;
                var expMestMedicine = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, medicineFilter, param) ?? new List<V_HIS_EXP_MEST_MEDICINE>();
                if (expMestMedicine != null && expMestMedicine.Count > 0)
                {
                    expMestMedicine = expMestMedicine.Where(o => o.IS_EXPEND != 1).ToList();
                }


                HisSereServViewFilter filter = new HisSereServViewFilter();
                filter.TREATMENT_ID = treatment.ID;
                var rsSereServ = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumers.MosConsumer, filter, param);

                if (rsSereServ != null && rsSereServ.Count > 0)
                {
                    rsSereServ = rsSereServ.Where(o => o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT || o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT || o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM).ToList();
                }
                WaitingManager.Hide();


                MPS.Processor.Mps000478.PDO.Mps000478PDO rdo = new MPS.Processor.Mps000478.PDO.Mps000478PDO(
                    treatment,
                    rsSereServ,
                    expMestMedicine                
                    );

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment != null ? treatment.TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

		private void InBangKiemTruocTiemChungClick()
        {
            try
            {
                richEditorMain.RunPrintTemplate("Mps000358", DelegateRunPrinterInBangKiemTruocTiemChung);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool DelegateRunPrinterInBangKiemTruocTiemChung(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                WaitingManager.Show();

                V_HIS_TREATMENT_4 treatment4 = gridViewtreatmentList.GetFocusedRow() as V_HIS_TREATMENT_4;

                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = treatment4.PATIENT_ID;
                var currentPatient = new BackendAdapter(param)
                          .Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, patientFilter, param).FirstOrDefault();

                MPS.Processor.Mps000358.PDO.SingleKeyValue singleKeyValue = new MPS.Processor.Mps000358.PDO.SingleKeyValue();
                //{
                //    LoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(),
                //    Username = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()
                //};


                MPS.Processor.Mps000358.PDO.Mps000358PDO mps000358PDO = new MPS.Processor.Mps000358.PDO.Mps000358PDO(
                    currentPatient,
                    singleKeyValue
                    );

                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode) && !String.IsNullOrEmpty(GlobalVariables.dicPrinter[printTypeCode]))
                {
                    if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000358PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, GlobalVariables.dicPrinter[printTypeCode]);
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000358PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, GlobalVariables.dicPrinter[printTypeCode]);
                    }
                }
                else
                {
                    if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000358PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000358PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                }

                PrintData.EmrInputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(treatment4.TREATMENT_CODE, printTypeCode, currentModule.RoomId);
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void InTomTatBenhAnClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                V_HIS_TREATMENT_4 treatment4 = gridViewtreatmentList.GetFocusedRow() as V_HIS_TREATMENT_4;
                var printTest = new HIS.Desktop.Plugins.Library.PrintTestTotal.PrintTestTotalProcessor(0, treatment4.ID);
                if (printTest != null)
                {
                    printTest.Print("Mps000316");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InGiayXacNhanBenhNhanCapCuuClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                richEditorMain.RunPrintTemplate("Mps000473", InGiayXacNhanBenhNhanCapCuu);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool InGiayXacNhanBenhNhanCapCuu(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                WaitingManager.Show();
               
                V_HIS_TREATMENT_4 treatment4 = gridViewtreatmentList.GetFocusedRow() as V_HIS_TREATMENT_4;

                var hisTreatment = new HIS_TREATMENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(hisTreatment, treatment4);

                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientFilter patientFilter = new HisPatientFilter();
                patientFilter.ID = treatment4.PATIENT_ID;
                var currentPatient = new BackendAdapter(param)
                          .Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, patientFilter, param).FirstOrDefault();


                CommonParam param_ = new CommonParam();
                MOS.Filter.HisPatientTypeAlterFilter patientFilter_ = new HisPatientTypeAlterFilter();
                patientFilter_.TREATMENT_ID = treatment4.ID;
                var currentPatientAlter = new BackendAdapter(param)
                          .Get<List<HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/Get", ApiConsumers.MosConsumer, patientFilter_, param_).OrderByDescending(o=>o.ID).FirstOrDefault();


                CommonParam params_ = new CommonParam();
                MOS.Filter.HisServiceReqFilter HisServiceReqFilter_ = new HisServiceReqFilter();
                HisServiceReqFilter_.TREATMENT_ID = treatment4.ID;
                HisServiceReqFilter_.IS_MAIN_EXAM = true;
                var currentHisServiceReq = new BackendAdapter(param)
                          .Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, HisServiceReqFilter_, params_).FirstOrDefault();



                CommonParam params__ = new CommonParam();
                MOS.Filter.HisBedLogViewFilter HisBedLogViewFilter_ = new HisBedLogViewFilter();
                HisBedLogViewFilter_.TREATMENT_ID = treatment4.ID;

                var currentHisBedLogView = new BackendAdapter(param)
                          .Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumers.MosConsumer, HisBedLogViewFilter_, params__).OrderByDescending(o => o.ID).FirstOrDefault();

                string login = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                MPS.Processor.Mps000473.PDO.Mps000473PDO mps000473PDO = new MPS.Processor.Mps000473.PDO.Mps000473PDO(currentPatient, currentPatientAlter, hisTreatment, currentHisBedLogView, currentHisServiceReq, login);

                MPS.ProcessorBase.Core.PrintData PrintData = null;

                WaitingManager.Hide();

                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode) && !String.IsNullOrEmpty(GlobalVariables.dicPrinter[printTypeCode]))
                {
                    if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000473PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, GlobalVariables.dicPrinter[printTypeCode]);
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000473PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, GlobalVariables.dicPrinter[printTypeCode]);
                    }
                }
                else
                {
                    if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000473PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000473PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                }

                PrintData.EmrInputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(treatment4.TREATMENT_CODE, printTypeCode, currentModule.RoomId);
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
