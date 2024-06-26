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
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.TreatmentList.Base;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.RichEditor.DAL;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
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

                BarButtonItem itemGiayNghiOm = new BarButtonItem(barManager, "Giấy nghỉ việc hưởng bhxh");
                itemGiayNghiOm.ItemClick += new ItemClickEventHandler(GiayNghiOmClick);

                BarButtonItem itemGiayTuVong = new BarButtonItem(barManager, "Giấy tử vong");
                itemGiayTuVong.ItemClick += new ItemClickEventHandler(GiayTuVongClick);

                BarButtonItem itemGiayKetQuaXetNghiem = new BarButtonItem(barManager, "Giấy Tổng hợp kết quả xét nghiệm");
                itemGiayKetQuaXetNghiem.ItemClick += new ItemClickEventHandler(GiayKetQuaXetNghiemClick);

                BarButtonItem itemTheBenhNhan = new BarButtonItem(barManager, "Thẻ bệnh nhân");
                itemTheBenhNhan.ItemClick += new ItemClickEventHandler(TheBenhNhanClick);

                BarButtonItem itemTrichLuc = new BarButtonItem(barManager, "Trích lục");
                itemTrichLuc.ItemClick += new ItemClickEventHandler(InTomTatBenhAnClick);

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
                    if (treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN
                        || treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CTCV
                        || treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN
                        || treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN
                        || treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__XINRAVIEN
                        || treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET
                        || treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__KHAC)
                    {
                        menu.AddItems(new BarItem[] { itemRaVien });
                    }
                    if (treatment.OUT_TIME != null)
                    {
                        menu.AddItems(new BarItem[] { itemGiayPTTT });
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

                menu.AddItems(new BarItem[] { itemKhamBenhVaoVien, itemBenhAnNgoaiTru, itemGiayKetQuaXetNghiem, itemTheBenhNhan, itemHoSoQuanLySucKhoeCaNhan, itemBieuMauKhac, itemTrichLuc });

                menu.ShowPopup(Cursor.Position);
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

                BarButtonItem itemGiayKetQuaXetNghiem = new BarButtonItem(barManager, "Giấy Tổng hợp kết quả xét nghiệm");
                itemGiayKetQuaXetNghiem.ItemClick += new ItemClickEventHandler(GiayKetQuaXetNghiemClick);

                BarButtonItem itemTheBenhNhan = new BarButtonItem(barManager, "Thẻ bệnh nhân");
                itemTheBenhNhan.ItemClick += new ItemClickEventHandler(TheBenhNhanClick);

                BarButtonItem itemTrichLuc = new BarButtonItem(barManager, "Trích lục");
                itemTrichLuc.ItemClick += new ItemClickEventHandler(InTomTatBenhAnClick);

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
                    if (treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN
                        || treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CTCV
                        || treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN
                        || treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN
                        || treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__XINRAVIEN
                        || treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET
                        || treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__KHAC)
                    {
                        menu.AddItems(new BarItem[] { itemRaVien });
                    }
                    if (treatment.OUT_TIME != null)
                    {
                        menu.AddItems(new BarItem[] { itemGiayPTTT });
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

                menu.AddItems(new BarItem[] { itemKhamBenhVaoVien, itemBenhAnNgoaiTru, itemGiayKetQuaXetNghiem, itemTheBenhNhan, itemHoSoQuanLySucKhoeCaNhan, itemBieuMauKhac, itemTomTatBA, itemTrichLuc });

                menu.ShowPopup(Cursor.Position);
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
                var printProcess = new HIS.Desktop.Plugins.Library.PrintTreatmentFinish.PrintTreatmentFinishProcessor(hisTreatment, currentModule != null ? currentModule.RoomId : 0);
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
                var printProcess = new HIS.Desktop.Plugins.Library.PrintTreatmentFinish.PrintTreatmentFinishProcessor(hisTreatment, currentModule != null ? currentModule.RoomId : 0);
                printProcess.Print(PrintTypeCodeWorker.PRINT_TYPE_CODE__IN_GIAY_HEN_KHAM__MPS000010, false);
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
    }
}
