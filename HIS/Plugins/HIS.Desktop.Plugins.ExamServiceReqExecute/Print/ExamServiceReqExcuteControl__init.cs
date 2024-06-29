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
using DevExpress.Utils.Menu;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Base;
using HIS.Desktop.Plugins.Library.PrintOtherForm;
using HIS.Desktop.Plugins.Library.PrintPrescription;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl : UserControlBase
    {
        V_HIS_PATIENT patient { get; set; }
        HIS_DHST dhst { get; set; }
        V_HIS_PATIENT_TYPE_ALTER patientTypeAlter { get; set; }
        List<V_HIS_DEPARTMENT_TRAN> departmentTrans { get; set; }
        //V_HIS_TRAN_PATI tranPatie { get; set; }
        List<V_HIS_SERE_SERV_5> sereServMedis { get; set; }
        List<HIS_EXP_MEST> expMests { get; set; }

        Inventec.Common.RichEditor.RichEditorStore richEditorMain;

        /// <summary>
        /// Khởi tạo nút in và in
        /// </summary>
        /// <param name="isAppoinment">Khởi tạo in hẹn khám và in</param>
        /// <param name="isBordereau">Khởi tạo in bệnh án ngoại trú và in</param>
        private void FillDataToButtonPrintAndAutoPrint()
        {
            try
            {
                richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), Inventec.Desktop.Common.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                DXPopupMenu menu = btnPrint_ExamService.DropDownControl as DXPopupMenu;

                if (menu == null || menu.Items == null || menu.Items.Count == 0)
                {
                    #region Khởi tạo mới các menu in ấn
                    menu = new DXPopupMenu();
                    DXMenuItem itemKhamBenhVaoVien = new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXAM_SERVICE_REQ_EXCUTE_CONTROL_PHIEU_KHAM_BENH_VAO_VIEN", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickInPhieuKhamBenh));
                    itemKhamBenhVaoVien.Tag = PrintType.KHAM_BENH_VAO_VIEN;
                    menu.Items.Add(itemKhamBenhVaoVien);

                    DXMenuItem itemBenhAnNgoaiTru = new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXAM_SERVICE_REQ_EXCUTE_CONTROL_BENH_AN_NGOAI_TRU", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickInPhieuKhamBenh));
                    itemBenhAnNgoaiTru.Tag = PrintType.BENH_AN_NGOAI_TRU;
                    menu.Items.Add(itemBenhAnNgoaiTru);

                    //Biểu mẫu khác phiếu phẫu thuật thủ thuật
                    DXSubMenuItem itemBieuMauKhac = new DXSubMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXAM_SERVICE_REQ_EXCUTE_CONTROL_BIEU_MAU_KHAC", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()));

                    DXMenuItem itemPhieuPTTT = new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXAM_SERVICE_REQ_EXCUTE_CONTROL_PHIEU_PTTT", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickInPhieuKhamBenh));
                    itemPhieuPTTT.Tag = PrintType.BIEU_MAU_KHAC_PHIEU_PTTT;
                    itemBieuMauKhac.Items.Add(itemPhieuPTTT);

                    DXMenuItem itemPhieuTuVan = new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXAM_SERVICE_REQ_EXCUTE_CONTROL_PHIEU_TU_VAN", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickInPhieuKhamBenh));
                    itemPhieuTuVan.Tag = PrintType.PHIEU_TU_VAN;
                    itemBieuMauKhac.Items.Add(itemPhieuTuVan);

                    DXMenuItem itemPhieuXacNhanXNHIV = new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXAM_SERVICE_REQ_EXCUTE_CONTROL_PHIEU_XAC_NHAN_DONG_Y_XET_NGHIEM_HIV_MPS401", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickInPhieuKhamBenh));
                    itemPhieuXacNhanXNHIV.Tag = PrintType.PHIEU_XAC_NHAN_DONG_Y_XET_NGHIEM_HIV_MPS401;
                    itemBieuMauKhac.Items.Add(itemPhieuXacNhanXNHIV);

                    // Phieu xet nghiem vi khuan lao
                    DXMenuItem itemPhieuXNVKLAO = new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXAM_SERVICE_REQ_EXCUTE_CONTROL_PHIEU_XET_NGHIEM_VI_KHUAN_LAO_MPS436", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickInPhieuKhamBenh));
                    itemPhieuXNVKLAO.Tag = PrintType.PHIEU_XET_NGHIEM_VI_KHUAN_LAO_MPS436;
                    itemBieuMauKhac.Items.Add(itemPhieuXNVKLAO);

                    DXMenuItem itemPXNXPertXPressSARsnCoV2 = new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXAM_SERVICE_REQ_EXCUTE_CONTROL_PHIEU_XET_NGHIEM_XPertXPressSARsnCOV2_MPS437", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickInPhieuKhamBenh));
                    itemPXNXPertXPressSARsnCoV2.Tag = PrintType.PHIEU_XET_NGHIEM_XPertXPressSARsnCOV2_MPS437;
                    itemBieuMauKhac.Items.Add(itemPXNXPertXPressSARsnCoV2);

                    // Phieu xet nghiem dom
                    DXMenuItem itemPhieuXNDom = new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXAM_SERVICE_REQ_EXCUTE_CONTROL_PHIEU_XET_NGHIEM_DOM_MPS438", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickInPhieuKhamBenh));
                    itemPhieuXNDom.Tag = PrintType.PHIEU_XET_NGHIEM_DOM_MPS438;
                    itemBieuMauKhac.Items.Add(itemPhieuXNDom);

                    DXMenuItem itemPhieuKBChuyenKhamPhongPTTT = new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXAM_SERVICE_REQ_EXCUTE_CONTROL_PHIEU_KB_CHUYEN_KHAM_PHONG_PTTT", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickInPhieuKhamBenh));
                    itemPhieuKBChuyenKhamPhongPTTT.Tag = PrintType.PHIEU_KHAM_BENH_CHUYEN_KHAM_PHONG_PTTT;
                    itemBieuMauKhac.Items.Add(itemPhieuKBChuyenKhamPhongPTTT);

                    DXMenuItem itemBenhAnNgoaiTruPTTTPhongKhamPTTT = new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXAM_SERVICE_REQ_EXCUTE_CONTROL_BENH_AN_DIEU_TRI_NGOAI_TRU_PTTT_PHONG_KHAM_PTTT", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickInPhieuKhamBenh));
                    itemBenhAnNgoaiTruPTTTPhongKhamPTTT.Tag = PrintType.BENH_AN_DIEU_TRI_NGOAI_TRU_PTTT_PHONG_KHAM_PTTT;
                    itemBieuMauKhac.Items.Add(itemBenhAnNgoaiTruPTTTPhongKhamPTTT);

                    DXMenuItem itemBangTracNghiemCoVaCamGiac = new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXAM_SERVICE_REQ_EXCUTE_CONTROL_BANG_TRAC_NGHIEM_CO_VA_CAM_GIAC", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickInPhieuKhamBenh));
                    itemBangTracNghiemCoVaCamGiac.Tag = PrintType.BANG_TRAC_NGHIEM_CO_VA_CAM_GIAC;
                    itemBieuMauKhac.Items.Add(itemBangTracNghiemCoVaCamGiac);

                    DXMenuItem itemPhieuKiemTraKhamSucKhoeDinhKy = new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXAM_SERVICE_REQ_EXCUTE_CONTROL_PHIEU_KIEM_TRA_KHAM_SUC_KHOE_DINH_KY", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickInPhieuKhamBenh));
                    itemPhieuKiemTraKhamSucKhoeDinhKy.Tag = PrintType.PHIEU_KIEM_TRA_KHAM_SUC_KHOE_DINH_KY;
                    itemBieuMauKhac.Items.Add(itemPhieuKiemTraKhamSucKhoeDinhKy);

                    menu.Items.Add(itemBieuMauKhac);

                    DXMenuItem itemXetNghiem = new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXAM_SERVICE_REQ_EXCUTE_CONTROL_IN_PHIEU_XET_NGHIEM", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(clickItemXetNghiem));
                    itemXetNghiem.Tag = PrintType.KET_QUA_XET_NGHIEM_TONG_HOP;
                    menu.Items.Add(itemXetNghiem);

                    DXMenuItem itemGopDonThuoc = new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXAM_SERVICE_REQ_EXCUTE_CONTROL_IN_GOP_DON_THUOC", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(clickItemInGopDonThuoc));
                    itemGopDonThuoc.Tag = PrintType.GOP_DON_THUOC;
                    menu.Items.Add(itemGopDonThuoc);

                    DXMenuItem itemTrichLuc = new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXAM_SERVICE_REQ_EXCUTE_CONTROL_IN_TRICH_LUC", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(clickItemTrichLuc));
                    itemTrichLuc.Tag = PrintType.TRICH_LUC;
                    menu.Items.Add(itemTrichLuc);

                    if (this.treatment != null && this.treatment.IS_PAUSE == 1)
                    {
                        DXMenuItem itemDonPhongKhamTongHop = new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXAM_SERVICE_REQ_EXCUTE_CONTROL_IN_DON_PHONG_KHAM_TONG_HOP", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(clickItemInDonPhongKhamTongHop));
                        itemDonPhongKhamTongHop.Tag = PrintType.DON_PHONG_KHAM_TONG_HOP;
                        menu.Items.Add(itemDonPhongKhamTongHop);
                    }


                    DXMenuItem itemBenhAnNgoaiChan = new DXMenuItem(Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnBANgoaiChan.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickInPhieuKhamBenh));
                    itemBenhAnNgoaiChan.Tag = PrintType.IN_BENH_AN_NGOAI_CHAN;
                    menu.Items.Add(itemBenhAnNgoaiChan);

                    DXMenuItem itemBenhAnCapCuu = new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXAM_SERVICE_REQ_EXCUTE_CONTROL_BENH_AN_CAP_CUU", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickInPhieuKhamBenh));
                    itemBenhAnCapCuu.Tag = PrintType.BENH_AN_CAP_CUU_MPS374;
                    menu.Items.Add(itemBenhAnCapCuu);

                    DXMenuItem itemDonthuoc = new DXMenuItem("In đơn thuốc", new EventHandler(clickInDonthuoc));
                    itemDonthuoc.Tag = PrintType.IN_DON_THUOC;
                    menu.Items.Add(itemDonthuoc);

                    DXMenuItem Mps000478Item = new DXMenuItem("Tóm tắt y lệnh phẫu thuật thủ thuật và đơn thuốc", new EventHandler(clickTomTatYLenhPTTTVaDonThuoc));
                    Mps000478Item.Tag = PrintType.TOM_TAT_Y_LENH_PTTT_VA_DON_THUOC;
                    menu.Items.Add(Mps000478Item);

                    DXMenuItem Mps000178Item = new DXMenuItem("In thẻ bệnh nhân", new EventHandler(clickInTheBn));
                    Mps000178Item.Tag = PrintType.IN_THE_BN;
                    menu.Items.Add(Mps000178Item);
                    #endregion
                }
                else
                {
                    #region Khởi tạo lại menu in ấn

                    //Trường hợp khi lưu thành công load lại thông tin hồ sơ điều trị
                    //
                    if (this.treatment != null && this.treatment.IS_PAUSE == 1)
                    {
                        DXMenuItem itemDonPhongKhamTongHop = new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXAM_SERVICE_REQ_EXCUTE_CONTROL_IN_DON_PHONG_KHAM_TONG_HOP", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(clickItemInDonPhongKhamTongHop));
                        itemDonPhongKhamTongHop.Tag = PrintType.DON_PHONG_KHAM_TONG_HOP;
                        bool isExist = false;
                        foreach (DXMenuItem item in menu.Items)
                        {
                            if (item.Tag == itemDonPhongKhamTongHop.Tag)
                            {
                                isExist = true;
                            }
                        }
                        if (!isExist)
                            menu.Items.Add(itemDonPhongKhamTongHop);                   
                    }

                    if(HisServiceReqResult != null && HisServiceReqResult.TreatmentFinishResult != null && HisServiceReqResult.TreatmentFinishResult.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET)
                    {
                        DXMenuItem itemNNTV = new DXMenuItem("Phiếu chẩn đoán nguyên nhân tử vong", new EventHandler(clickItemNguyenNhanTuVong));
                        itemNNTV.Tag = PrintType.PHIEU_CHAN_DOAN_NGUYEN_NHAN_TU_VONG;
                        bool isExistNNTV = false;
                        foreach (DXMenuItem item in menu.Items)
                        {
                            if (item.Tag == itemNNTV.Tag)
                            {
                                isExistNNTV = true;
                            }
                        }
                        if (!isExistNNTV)
                            menu.Items.Add(itemNNTV);
                    }    
                    #endregion
                }

                btnPrint_ExamService.DropDownControl = menu;

                ContextMenuStrip strip = new ContextMenuStrip();
                ToolStripItem item1 = new ToolStripMenuItem();
                item1.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXAM_SERVICE_REQ_EXCUTE_CONTROL_IN_PHIEU_YEU_CAU_KHAM", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                item1.Click += clickItemKham;
                item1.Tag = PrintType.IN_PHIEU_YEU_CAU;

                strip.Show(Cursor.Position.X, Cursor.Position.Y);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void clickItemNguyenNhanTuVong(object sender, EventArgs e)
        {
            try
            {
                PrintMps000485();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void clickInTheBn(object sender, EventArgs e)
        {
            try
            {
                PrintMps000178();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void clickTomTatYLenhPTTTVaDonThuoc(object sender, EventArgs e)
        {
            try
            {
                PrintMps000478();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void clickInDonthuoc(object sender, EventArgs e)
        {
            try
            {
                InDonPhongKhamTongHop(false, true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void clickItemTrichLuc(object sender, EventArgs e)
        {
            try
            {
                HIS_SERVICE_REQ req = new HIS_SERVICE_REQ();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(req, HisServiceReqView);
                var printTest = new HIS.Desktop.Plugins.Library.PrintTestTotal.PrintTestTotalProcessor(req);
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

        private void InitDonPhongKhamTongHop()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void clickItemInGopDonThuoc(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                //Load đơn phòng khám
                HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                serviceReqFilter.TREATMENT_ID = this.treatmentId;
                serviceReqFilter.SERVICE_REQ_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK };
                List<HIS_SERVICE_REQ> serviceReqs = new BackendAdapter(param)
                   .Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, serviceReqFilter, param);

                if (serviceReqs == null || serviceReqs.Count == 0)
                    return;
                //Load expmest
                HisExpMestFilter expMestFilter = new HisExpMestFilter();
                expMestFilter.SERVICE_REQ_IDs = serviceReqs.Select(o => o.ID).ToList();
                List<HIS_EXP_MEST> expMests = new BackendAdapter(param)
                     .Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, expMestFilter, param);

                if (expMests == null || expMests.Count == 0)
                    return;

                //Laays thuoc va tu trong kho

                HisExpMestMedicineFilter expMestMedicineFilter = new HisExpMestMedicineFilter();
                expMestMedicineFilter.EXP_MEST_IDs = expMests.Select(o => o.ID).ToList();
                List<HIS_EXP_MEST_MEDICINE> expMestMedicines = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/Get", ApiConsumers.MosConsumer, expMestMedicineFilter, param);

                HisExpMestMaterialFilter expMestMaterialFilter = new HisExpMestMaterialFilter();
                expMestMaterialFilter.EXP_MEST_IDs = expMests.Select(o => o.ID).ToList();
                List<HIS_EXP_MEST_MATERIAL> expMestMaterials = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/Get", ApiConsumers.MosConsumer, expMestMaterialFilter, param);


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
                PrintPrescriptionProcessor printPrescriptionProcessor = new PrintPrescriptionProcessor(OutPatientPresResultSDOForPrints, this.moduleData);

                printPrescriptionProcessor.Print(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__IN_GOP_DON_THUOC__MPS000234, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void clickItemXetNghiem(object sender, EventArgs e)
        {
            try
            {
                var printTest = new HIS.Desktop.Plugins.Library.PrintTestTotal.PrintTestTotalProcessor(this.moduleData.RoomId, treatment.ID);
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

        void clickItemKham(object sender, EventArgs e)
        {
            try
            {
                var bbtnItem1 = sender as ToolStripItem;
                PrintType type = (PrintType)(bbtnItem1.Tag);
                PrintProcess(type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        internal enum PrintType
        {
            KHAM_BENH_VAO_VIEN,
            BENH_AN_NGOAI_TRU,
            BIEU_MAU_KHAC_PHIEU_PTTT,
            KHAM_SUC_KHOE_CAN_BO,
            IN_PHIEU_YEU_CAU,
            IN_GIAY_HEN_KHAM,
            IN_GIAY_CHUYEN_VIEN,
            IN_GIAY_RA_VIEN,
            IN_GIAY_BAO_TU,
            YEU_CAU_KHAM_THEM,
            BANG_KE_NGOAI_TRU_BHYT,
            BANG_KE_NGOAI_TRU_VIEN_PHI,
            KET_QUA_XET_NGHIEM_TONG_HOP,
            GOP_DON_THUOC,
            DON_PHONG_KHAM_TONG_HOP,
            IN_GIAY_NGHI_OM,
            IN_GIAY_NGHI_DUONG_THAI,
            IN_GIAY_NGHI_HUONG_BHXH,
            TRICH_LUC,
            IN_BENH_AN_NGOAI_CHAN,
            BENH_AN_CAP_CUU_MPS374,
            IN_PHIEU_HEN_MO,
            PHIEU_TU_VAN,
            PHIEU_XAC_NHAN_DONG_Y_XET_NGHIEM_HIV_MPS401,
            PHIEU_XET_NGHIEM_VI_KHUAN_LAO_MPS436,
            PHIEU_XET_NGHIEM_DOM_MPS438,
            PHIEU_KHAM_BENH_CHUYEN_KHAM_PHONG_PTTT,
            BENH_AN_DIEU_TRI_NGOAI_TRU_PTTT_PHONG_KHAM_PTTT,
            BANG_TRAC_NGHIEM_CO_VA_CAM_GIAC,
            PHIEU_KIEM_TRA_KHAM_SUC_KHOE_DINH_KY,
            PHIEU_XET_NGHIEM_XPertXPressSARsnCOV2_MPS437,
            PHIEU_CHUYEN_VIEN,
            IN_DON_THUOC,
            TOM_TAT_Y_LENH_PTTT_VA_DON_THUOC,
            PHIEU_THU_THANH_TOAN,
            PHIEU_CHAN_DOAN_NGUYEN_NHAN_TU_VONG,
            IN_THE_BN
        }

        private void onClickInPhieuKhamBenh(object sender, EventArgs e)
        {
            try
            {

                var bbtnItem = sender as DXMenuItem;
                PrintType type = (PrintType)(bbtnItem.Tag);
                PrintProcess(type);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_BENH_AN_NGOAI_TRU__MPS000174:
                        LoadBieuMauPhieuYCBenhAnNgoaiTru(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_KHAM_BENH_VAO_VIEN__MPS000007:
                        LoadBieuMauPhieuYCKhamBenhVaoVien(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__BENH_AN_CAP_CUU__MPS000374:
                        LoadBieuMauPhieuYCKhamBenhCapCuu(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__BENH_AN_NGOAI_CHAN__MPS000362:
                        LoadBieuMauPhieuBenhAnNgoaiChan(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__GIAY_HEN_KHAM__MPS000010:
                        LoadBieuMauGiayHenKham(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__GIAY_RA_VIEN__MPS000008:
                        LoadBieuMauGiayRaVien(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__GIAY_CHUYEN_VIEN__MPS000011:
                        LoadBieuMauGiayChuyenVien(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__GIAY_BAO_TU__MPS000268:
                        LoadBieuMauGiayBaoTu(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__PHIEU_NGHI_OM__MPS000269:
                        LoadBieuMauGiayNghiOm(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__PHIEU_HEN_MO__MPS000389:
                        LoadBieuMauGiayHenMo(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__PHIEU_XAC_NHAN_DONG_Y_XET_NGHIEM_HIV__MPS000401:
                        LoadBieuMauGiayDongYXetNghiemHIV(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__PHIEU_XET_NGHIEM_VI_KHUAN_LAO__MPS000436:
                        LoadBieuMauGiayXetNghiemViKhuanLao(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__PHIEU_XET_NGHIEM_XPertXPressSARsnCOV2_MPS000437:
                        ExamServiceReqExcuteControl__Print__Sars_nCOV_2(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__PHIEU_XET_NGHIEM_DOM__MPS000438:
                        LoadBieuMauGiayXetNghiemDom(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__PHIEU_KIEM_TRA_KHAM_SUC_KHOE_DINH_KY__MPS000315:
                        LoadPhieuKiemTraKhamSucKhoeDinhKy(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__TOM_TAT_Y_LENH_PTTT_VA_DON_THUOC_MPS000478:
                        ProcessPrintMps000478(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__PHIEU_THU_THANH_TOAN_MPS000111:
                        ProcessPrintMps000111(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__NGUYEN_NHAN_TU_VONG:
                        ProcessPrintMps000485(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000178":
                        ProcessPrintMps000178(printTypeCode, fileName, ref result);
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

        private void ProcessPrintMps000111(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }
                if (resultEPayment != null)
                {
                    HisSereServFilter ssFilter = new HisSereServFilter();
                    ssFilter.IDs = resultEPayment.SereServs.Select(o => o.ID).ToList();
                    List<HIS_SERE_SERV> hisSereServs = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssFilter, null);

                    V_HIS_PATIENT_TYPE_ALTER patientAlter = new V_HIS_PATIENT_TYPE_ALTER();
                    if (patient != null)
                    {
                        HisPatientTypeAlterViewFilter ft = new HisPatientTypeAlterViewFilter();
                        ft.TDL_PATIENT_ID = patient.ID;
                        patientAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, ft, null).FirstOrDefault();
                    }
                    HisDepartmentTranLastFilter departLastFilter = new HisDepartmentTranLastFilter();
                    departLastFilter.TREATMENT_ID = (long)this.resultEPayment.Transaction.TREATMENT_ID;
                    departLastFilter.BEFORE_LOG_TIME = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    var departmentTran = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, departLastFilter, null);

                    MPS.Processor.Mps000111.PDO.Mps000111PDO pdo = new MPS.Processor.Mps000111.PDO.Mps000111PDO
                    (
                        resultEPayment.Transaction,
                        patient,
                        null,
                        hisSereServs,
                        departmentTran,
                        patientAlter,
                        (long)treatment.TDL_PATIENT_TYPE_ID
                    );
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void PrintProcess(PrintType printType)
        {
            try
            {
                switch (printType)
                {
                    case PrintType.KHAM_BENH_VAO_VIEN:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_KHAM_BENH_VAO_VIEN__MPS000007, DelegateRunPrinter);
                        break;
                    case PrintType.BENH_AN_CAP_CUU_MPS374:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__BENH_AN_CAP_CUU__MPS000374, DelegateRunPrinter);
                        break;
                    case PrintType.BENH_AN_NGOAI_TRU:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_BENH_AN_NGOAI_TRU__MPS000174, DelegateRunPrinter);
                        break;
                    case PrintType.IN_BENH_AN_NGOAI_CHAN:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__BENH_AN_NGOAI_CHAN__MPS000362, DelegateRunPrinter);
                        break;
                    //Biểu mẫu khác Phiếu PTTT
                    case PrintType.BIEU_MAU_KHAC_PHIEU_PTTT:
                        LoadBieuMauKhacPhieuPTTT();
                        break;
                    case PrintType.PHIEU_TU_VAN:
                        LoadBieuMauKhacPhieuTuVan();
                        break;
                    case PrintType.PHIEU_KHAM_BENH_CHUYEN_KHAM_PHONG_PTTT:
                        LoadBieuMauKhacPhieuKBChuyenKhamPhongPTTT();
                        break;
                    case PrintType.BENH_AN_DIEU_TRI_NGOAI_TRU_PTTT_PHONG_KHAM_PTTT:
                        LoadBenhAnDieuTriNgoaiTruPTTTPhongKhamPTTT();
                        break;
                    case PrintType.BANG_TRAC_NGHIEM_CO_VA_CAM_GIAC:
                        LoadBangTracNghiemCoVaCamGiac();
                        break;
                    case PrintType.PHIEU_XAC_NHAN_DONG_Y_XET_NGHIEM_HIV_MPS401:
                        DelegateRunPrinter(PrintTypeCodeWorker.PRINT_TYPE_CODE__PHIEU_XAC_NHAN_DONG_Y_XET_NGHIEM_HIV__MPS000401, null);
                        //Inventec.Common.Logging.LogSystem.Debug("PrintProcess" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => printType), printType));
                        //richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__PHIEU_XAC_NHAN_DONG_Y_XET_NGHIEM_HIV__MPS000401, null, DelegateRunPrinter, true);
                        break;

                    // Phieu xet nghiem vi khuan lao
                    case PrintType.PHIEU_XET_NGHIEM_VI_KHUAN_LAO_MPS436:
                        DelegateRunPrinter(PrintTypeCodeWorker.PRINT_TYPE_CODE__PHIEU_XET_NGHIEM_VI_KHUAN_LAO__MPS000436, null);
                        break;

                    case PrintType.PHIEU_XET_NGHIEM_XPertXPressSARsnCOV2_MPS437:
                        DelegateRunPrinter(PrintTypeCodeWorker.PRINT_TYPE_CODE__PHIEU_XET_NGHIEM_XPertXPressSARsnCOV2_MPS000437, null);
                        // LoadBieuMauXetNghiemXPertXPressSARsnCOV2();
                        break;

                    // Phieu xet nghiem dom
                    case PrintType.PHIEU_XET_NGHIEM_DOM_MPS438:
                        DelegateRunPrinter(PrintTypeCodeWorker.PRINT_TYPE_CODE__PHIEU_XET_NGHIEM_DOM__MPS000438, null);
                        break;

                    // Het bieu mau khac
                    case PrintType.KHAM_SUC_KHOE_CAN_BO:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_KHAM_SUC_KHOE_CAN_BO__MPS000013, DelegateRunPrinter);
                        break;
                    case PrintType.IN_PHIEU_YEU_CAU:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_KHAM__MPS000025, DelegateRunPrinter);
                        break;
                    case PrintType.IN_GIAY_HEN_KHAM:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__GIAY_HEN_KHAM__MPS000010, DelegateRunPrinter);
                        break;
                    case PrintType.YEU_CAU_KHAM_THEM:
                        InPhieuYeuCauDichVu(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__YEU_CAU_KHAM_THEM__MPS000071);
                        //richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__YEU_CAU_KHAM_THEM__MPS000071, DelegateRunPrinter);
                        break;
                    case PrintType.IN_GIAY_CHUYEN_VIEN:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__GIAY_CHUYEN_VIEN__MPS000011, DelegateRunPrinter);
                        break;
                    case PrintType.IN_GIAY_RA_VIEN:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__GIAY_RA_VIEN__MPS000008, DelegateRunPrinter);
                        break;
                    case PrintType.IN_GIAY_BAO_TU:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__GIAY_BAO_TU__MPS000268, DelegateRunPrinter);
                        break;
                    case PrintType.BANG_KE_NGOAI_TRU_BHYT:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NGOAI_TRU_BHYT__MPS000120, DelegateRunPrinter);
                        break;
                    case PrintType.BANG_KE_NGOAI_TRU_VIEN_PHI:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NGOAI_TRU_VIEN_PHI__MPS000122, DelegateRunPrinter);
                        break;
                    case PrintType.IN_GIAY_NGHI_OM:
                        richEditorMain.RunPrintTemplate("Mps000269", DelegateRunPrinter);
                        break;
                    case PrintType.IN_PHIEU_HEN_MO:
                        richEditorMain.RunPrintTemplate("Mps000389", DelegateRunPrinter);
                        break;
                    case PrintType.PHIEU_KIEM_TRA_KHAM_SUC_KHOE_DINH_KY:
                        richEditorMain.RunPrintTemplate("Mps000315", DelegateRunPrinter);
                        break;
                    case PrintType.PHIEU_THU_THANH_TOAN:
                        richEditorMain.RunPrintTemplate("Mps000111", DelegateRunPrinter);
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

        private void LoadBieuMauKhacPhieuPTTT()
        {
            try
            {
                //Lấy thông tin treatment
                //HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                //treatmentFilter.ID = treatmentId;
                //HIS_TREATMENT treatment = new BackendAdapter(new CommonParam())
                //    .Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, new CommonParam()).FirstOrDefault();

                Inventec.Common.Logging.LogSystem.Debug("HisServiceReqView.ID_____" + HisServiceReqView.ID + "_____sereServ.ID______" + "______treatment.ID_____" + treatment.ID.ToString() + "______treatment.PATIENT_ID______" + treatment.PATIENT_ID.ToString() + "________SERVICE_REQ____________" + Library.PrintOtherForm.Base.UpdateType.TYPE.SERVICE_REQ.ToString());

                PrintOtherFormProcessor printOtherFormProcessor = new PrintOtherFormProcessor(HisServiceReqView.ID, -1, treatment.ID, treatment.PATIENT_ID, Library.PrintOtherForm.Base.UpdateType.TYPE.SERVICE_REQ);
                printOtherFormProcessor.Print(HIS.Desktop.Plugins.Library.PrintOtherForm.Base.PrintType.TYPE.PHIEU_PHAU_THUAT_THU_THUAT);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadBieuMauKhacPhieuTuVan()
        {
            try
            {
                PrintOtherFormProcessor printOtherFormProcessor = new PrintOtherFormProcessor(HisServiceReqView.ID, Library.PrintOtherForm.Base.UpdateType.TYPE.OPEN_OTHER_ASS_TREATMENT);
                printOtherFormProcessor.Print(HIS.Desktop.Plugins.Library.PrintOtherForm.Base.PrintType.TYPE.MPS000400_PHIEU_TU_VAN);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadBieuMauKhacPhieuKBChuyenKhamPhongPTTT()
        {
            try
            {
                var ado = new HIS.Desktop.Plugins.Library.PrintOtherForm.Base.PrintOtherInputADO();
                ado.ServiceReqId = this.HisServiceReqView.ID;
                ado.TreatmentId = this.HisServiceReqView.TREATMENT_ID;
                ado.DhstId = this.HisServiceReqView.DHST_ID;
                ado.RoomId = this.currentModuleBase.RoomId;
                ado.PatientId = this.HisServiceReqView.TDL_PATIENT_ID;

                var printProcess = new HIS.Desktop.Plugins.Library.PrintOtherForm.PrintOtherFormProcessor(ado, Library.PrintOtherForm.Base.UpdateType.TYPE.OPEN_OTHER_ASS_TREATMENT);
                printProcess.Print(Library.PrintOtherForm.Base.PrintType.TYPE.MPS000407_PHIEU_KHAM_BENH_CHUYEN_KHAM_PHONG_PTTT);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadBenhAnDieuTriNgoaiTruPTTTPhongKhamPTTT()
        {
            try
            {
                var ado = new HIS.Desktop.Plugins.Library.PrintOtherForm.Base.PrintOtherInputADO();
                ado.TreatmentId = this.HisServiceReqView.TREATMENT_ID;
                ado.DhstId = this.HisServiceReqView.DHST_ID;

                var printProcess = new HIS.Desktop.Plugins.Library.PrintOtherForm.PrintOtherFormProcessor(ado, Library.PrintOtherForm.Base.UpdateType.TYPE.OPEN_OTHER_ASS_TREATMENT);
                printProcess.Print(Library.PrintOtherForm.Base.PrintType.TYPE.MPS000410_BENH_AN_DIEU_TRI_NGOAI_TRU_PTTT_PHONG_KHAM_PTTT);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadBangTracNghiemCoVaCamGiac()
        {
            try
            {
                var ado = new HIS.Desktop.Plugins.Library.PrintOtherForm.Base.PrintOtherInputADO();
                ado.TreatmentId = this.HisServiceReqView.TREATMENT_ID;

                var printProcess = new HIS.Desktop.Plugins.Library.PrintOtherForm.PrintOtherFormProcessor(ado, Library.PrintOtherForm.Base.UpdateType.TYPE.OPEN_OTHER_ASS_TREATMENT);
                printProcess.Print(Library.PrintOtherForm.Base.PrintType.TYPE.MPS000412_BANG_TRAC_NGHIEM_CO_VA_CAM_GIAC);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadBieuMauXetNghiemXPertXPressSARsnCOV2()
        {
            try
            {
                var ado = new HIS.Desktop.Plugins.Library.PrintOtherForm.Base.PrintOtherInputADO();
                ado.TreatmentId = this.HisServiceReqView.TREATMENT_ID;
                ado.DhstId = this.HisServiceReqView.DHST_ID;

                var printProcess = new HIS.Desktop.Plugins.Library.PrintOtherForm.PrintOtherFormProcessor(ado, Library.PrintOtherForm.Base.UpdateType.TYPE.OPEN_OTHER_ASS_TREATMENT);
                printProcess.Print(Library.PrintOtherForm.Base.PrintType.TYPE.MPS000410_BENH_AN_DIEU_TRI_NGOAI_TRU_PTTT_PHONG_KHAM_PTTT);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
