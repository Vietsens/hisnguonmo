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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LibraryMessage;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using HIS.Desktop.ADO;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using HIS.Desktop.ApiConsumer;
using MOS.Filter;
using Inventec.Common.Adapter;
using DevExpress.XtraEditors;
using HIS.Desktop.Plugins.HisExportMestMedicine.Base;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;

namespace HIS.Desktop.Plugins.HisExportMestMedicine
{
    public partial class UCHisExportMestMedicine : UserControlBase
    {
        bool isExpWithExpTime = false;
        long? ExpTime = null;

        /// <summary>
        /// Cot "Du tru mau": mo chi tiet benh an dien tu cua ho so dieu tri tren dong dang chon.
        /// Dung lai nguyen logic cua ban DLL dang chay tai vien (dich nguoc tu IL ngay 14/09/2026),
        /// vi ma nguon cua cot nay chua tung duoc dua len kho ma nguon.
        /// </summary>
        private void repositoryItemButtonViewBloodRequest_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = gridView.GetFocusedRow() as V_HIS_EXP_MEST_2;
                if (rowData == null)
                {
                    MessageManager.Show("Bạn chưa chọn phiếu xuất.");
                    return;
                }

                // Cau hinh bang 1: mo ngay Phieu cung cap mau va thanh phan mau (Mps000108).
                // Khac 1 hoac khong khai bao: giu nguyen hanh vi cu la mo man chi tiet benh an.
                if (HisConfigCFG.ViewBloodSupplySlipOption == "1")
                {
                    // Phieu cung cap mau chi phat sinh tu DON MAU. Cac loai phieu khac trong kho mau
                    // (chuyen kho, nhap tra, hao phi...) khong co phieu nay nen chan ngay tu dau,
                    // khong goi API lay du lieu roi moi bao loi.
                    if (rowData.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM)
                    {
                        MessageManager.Show("Phiếu này không phải đơn máu nên không có phiếu cung cấp máu và thành phần máu.");
                        return;
                    }

                    ShowBloodSupplySlip();
                    return;
                }

                if (string.IsNullOrWhiteSpace(rowData.TDL_TREATMENT_CODE))
                {
                    MessageManager.Show("Phiếu xuất không gắn hồ sơ điều trị nên không mở được chi tiết bệnh án.");
                    return;
                }

                WaitingManager.Show();

                List<object> listArgs = new List<object>();
                listArgs.Add(rowData.TDL_TREATMENT_CODE);

                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(
                    "HIS.Desktop.Plugins.EmrDocument",
                    this.currentModule.RoomId,
                    this.currentModule.RoomTypeId,
                    listArgs);

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }
        /// <summary>
        /// Mo ngay ban xem truoc Phieu cung cap mau va thanh phan mau
        /// (mau in Mps000108) cua dong dang chon, theo ma phieu xuat.
        /// Khuon lay tu chuc nang Ke don mau: HIS.Desktop.Plugins.HisAssignBlood,
        /// tep frmHisAssignBlood__Plus__Print.cs, ham dung du lieu in dong 120-178.
        /// Khac khuon goc: lay ma phieu xuat tu dong luoi thay vi tu bien cua form,
        /// va mo o che do XEM TRUOC thay vi in thang ra may in.
        /// </summary>
        private void ShowBloodSupplySlip()
        {
            try
            {
                // Phai di qua RunPrintTemplate de lay TEN FILE MAU thuc te cua vien,
                // vi cung mot ma Mps000108 moi vien dung mot file mau khac nhau.
                // RunPrintTemplate se goi nguoc lai deletePrintTemplate kem printTypeCode + fileName.
                // BUOC 1: tim van ban DA KY cua phieu nay trong EMR.
                // Nghiep vu chot moi phieu chi co DUY NHAT ban ky cuoi cung co gia tri, nen da co
                // ban ky thi phai mo dung ban do, KHONG dung lai phieu moi tu du lieu hien tai.
                // Khong nho MPS lam viec nay duoc: moi che do PreviewType cua MPS (ke ca nhom Emr*)
                // deu dung file MOI roi moi dua vao popup - da vap 3 lan ngay 17-18/09/2026.
                if (ShowSignedBloodSupplySlip())
                {
                    return;
                }

                // BUOC 2: chua co ban ky thi dung phieu moi nhu cu.
                Inventec.Common.RichEditor.RichEditorStore storeBloodSlip = new Inventec.Common.RichEditor.RichEditorStore(
                    ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                storeBloodSlip.RunPrintTemplate(HIS.Desktop.Print.PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000108, deletePrintTemplate);
                return;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Tim va mo van ban DA KY cua phieu (neo theo HIS_CODE = ma mau in + ma xuat).
        /// Tra ve true neu da mo duoc ban da ky; false neu chua co ban ky nao.
        /// Chep theo plugin EmrDocument: EmrDocumentForm.cs dong 1485-1495 va 1518-1558.
        /// Diem chot: chi van ban co LAST_VERSION_URL moi la ban DA KY that.
        /// </summary>
        private bool ShowSignedBloodSupplySlip()
        {
            try
            {
                var rowData = gridView.GetFocusedRow() as V_HIS_EXP_MEST_2;
                if (rowData == null || string.IsNullOrWhiteSpace(rowData.EXP_MEST_CODE))
                    return false;

                // HIS_CODE phai khop dung cong thuc ma Mps000108Processor.ProcessUniqueCodeData() sinh ra.
                string hisCode = string.Format("{0}_{1}",
                    HIS.Desktop.Print.PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000108,
                    rowData.EXP_MEST_CODE);

                CommonParam param = new CommonParam();

                EMR.Filter.EmrDocumentViewFilter docFilter = new EMR.Filter.EmrDocumentViewFilter();
                docFilter.HIS_CODE__EXACT = hisCode;
                docFilter.IS_DELETE = false;
                docFilter.ORDER_FIELD = "ID";
                docFilter.ORDER_DIRECTION = "DESC";

                var documents = new BackendAdapter(param).Get<List<EMR.EFMODEL.DataModels.V_EMR_DOCUMENT>>(
                    "api/EmrDocument/GetView", ApiConsumers.EmrConsumer, docFilter, param);

                if (documents == null || documents.Count == 0)
                    return false;

                // Ban ky cuoi cung: da sap ID giam dan nen lay ban DAU TIEN co LAST_VERSION_URL.
                var signedDoc = documents.FirstOrDefault(o => !string.IsNullOrWhiteSpace(o.LAST_VERSION_URL));
                if (signedDoc == null)
                    return false;

                WaitingManager.Show();

                EMR.SDO.EmrDocumentDownloadFileSDO downloadSdo = new EMR.SDO.EmrDocumentDownloadFileSDO();
                EMR.Filter.EmrDocumentViewFilter fileFilter = new EMR.Filter.EmrDocumentViewFilter();
                fileFilter.ID = signedDoc.ID;
                downloadSdo.EmrDocumentViewFilter = fileFilter;
                downloadSdo.HisCode = hisCode;

                var documentFiles = new BackendAdapter(param).Post<List<EMR.SDO.EmrDocumentFileSDO>>(
                    "api/EmrDocument/DownloadFile", ApiConsumers.EmrConsumer, downloadSdo, param);

                WaitingManager.Hide();

                if (documentFiles == null || documentFiles.Count == 0
                    || string.IsNullOrWhiteSpace(documentFiles[0].Base64Data))
                {
                    // Co ban ghi van ban nhung khong tai duoc tep: bao cho nguoi dung biet,
                    // KHONG am tham dung phieu moi vi se hien ra ban khac voi ban da ky.
                    MessageManager.Show("Phiếu này đã có bản ký nhưng không tải được tệp. Vui lòng liên hệ quản trị.");
                    return true;
                }

                // ShowPopup BAT BUOC phai co inputADO. Truyen null thi bi chan voi thong bao
                // "Tinh nang chi danh cho benh an dien tu" (log WARN ngay 18/09/2026).
                // Dung GenerateInputADO theo khuon EmrDocumentForm.cs dong 1456.
                Inventec.Common.SignLibrary.ADO.InputADO viewInputADO =
                    new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor()
                        .GenerateInputADO(
                            signedDoc.TREATMENT_CODE,
                            signedDoc.DOCUMENT_CODE,
                            signedDoc.DOCUMENT_NAME,
                            this.currentModule.RoomId);

                if (viewInputADO != null)
                {
                    // Man nay chi XEM ban da ky, khong ky va khong sua.
                    viewInputADO.IsSign = false;
                    viewInputADO.IsSave = false;
                    viewInputADO.IsExport = false;
                    viewInputADO.IsPrint = true;
                    viewInputADO.IsShowPatientSign = true;
                }

                Inventec.Common.SignLibrary.SignLibraryGUIProcessor libraryProcessor =
                    new Inventec.Common.SignLibrary.SignLibraryGUIProcessor();
                libraryProcessor.ShowPopup(documentFiles[0].Base64Data,
                    Inventec.Common.SignLibrary.FileType.Pdf, viewInputADO);

                return true;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                // Loi khi tra ban da ky thi quay ve luong dung phieu moi, khong chan nguoi dung.
                return false;
            }
        }

        /// <summary>
        /// Buoc 2: dung du lieu va in, duoc RunPrintTemplate goi nguoc lai kem ten file mau that.
        /// </summary>
        private void InPhieuCungCapMau(string printTypeCode, string fileName)
        {
            try
            {
                var rowData = gridView.GetFocusedRow() as V_HIS_EXP_MEST_2;
                if (rowData == null)
                {
                    MessageManager.Show("Bạn chưa chọn phiếu xuất.");
                    return;
                }

                WaitingManager.Show();
                CommonParam param = new CommonParam();

                // 1. Phieu xuat: PDO nhan kieu BANG HIS_EXP_MEST, con dong luoi la kieu KHUNG NHIN
                // V_HIS_EXP_MEST_2, nen phai lay lai ban ghi theo ma chu khong truyen thang duoc.
                HisExpMestFilter expMestFilter = new HisExpMestFilter();
                expMestFilter.ID = rowData.ID;
                var expMests = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>(
                    "api/HisExpMest/Get", ApiConsumers.MosConsumer, expMestFilter, param);
                HIS_EXP_MEST expMest = (expMests != null ? expMests.FirstOrDefault() : null);
                if (expMest == null)
                {
                    WaitingManager.Hide();
                    return;
                }

                // 2. Che pham mau yeu cau (loc theo ma phieu xuat)
                HisExpMestBltyReqView1Filter bltyFilter = new HisExpMestBltyReqView1Filter();
                bltyFilter.EXP_MEST_ID = expMest.ID;
                var expMestBltys = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_BLTY_REQ_1>>(
                    "/api/HisExpMestBltyReq/GetView1", ApiConsumers.MosConsumer, bltyFilter, param);

                // 3. Don vi mau thuc xuat (loc theo ma phieu xuat)
                HisExpMestBloodViewFilter bloodFilter = new HisExpMestBloodViewFilter();
                bloodFilter.EXP_MEST_ID = expMest.ID;
                var expMestBloods = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_BLOOD>>(
                    "api/HisExpMestBlood/GetView", ApiConsumers.MosConsumer, bloodFilter, param);

                // Phieu khong co du lieu mau thi bao cho nguoi dung, dung mo cua so xem truoc trang tron.
                if ((expMestBltys == null || expMestBltys.Count == 0)
                    && (expMestBloods == null || expMestBloods.Count == 0))
                {
                    WaitingManager.Hide();
                    MessageManager.Show("Phiếu xuất này chưa có dữ liệu máu nên không xem được phiếu cung cấp máu và thành phần máu.");
                    return;
                }

                // 4-5. Ho so dieu tri va giuong benh (theo cot TDL_TREATMENT_ID cua phieu xuat)
                V_HIS_TREATMENT treatment = null;
                List<V_HIS_TREATMENT_BED_ROOM> treatmentBedRooms = null;
                if (expMest.TDL_TREATMENT_ID.HasValue)
                {
                    HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                    treatmentFilter.ID = expMest.TDL_TREATMENT_ID.Value;
                    var treatments = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>(
                        "api/HisTreatment/GetView", ApiConsumers.MosConsumer, treatmentFilter, param);
                    treatment = (treatments != null ? treatments.FirstOrDefault() : null);

                    HisTreatmentBedRoomViewFilter bedRoomFilter = new HisTreatmentBedRoomViewFilter();
                    bedRoomFilter.TREATMENT_ID = expMest.TDL_TREATMENT_ID.Value;
                    bedRoomFilter.IS_IN_ROOM = true;
                    treatmentBedRooms = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM>>(
                        "api/HisTreatmentBedRoom/GetView", ApiConsumers.MosConsumer, bedRoomFilter, param);
                }

                // 6-7. Y lenh va dich vu con. Phai lay y lenh TRUOC vi dich vu con loc theo y lenh do.
                // Luu y ten cot: SERVICE_REQ_ID, KHONG phai TDL_SERVICE_REQ_ID.
                V_HIS_SERVICE_REQ serviceReq = null;
                List<V_HIS_SERE_SERV_1> sereServs = null;
                if (expMest.SERVICE_REQ_ID.HasValue)
                {
                    HisServiceReqViewFilter serviceReqFilter = new HisServiceReqViewFilter();
                    serviceReqFilter.ID = expMest.SERVICE_REQ_ID.Value;
                    var serviceReqs = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>(
                        "api/HisServiceReq/GetView", ApiConsumers.MosConsumer, serviceReqFilter, param);
                    serviceReq = (serviceReqs != null ? serviceReqs.FirstOrDefault() : null);

                    if (serviceReq != null)
                    {
                        HisSereServView1Filter sereServFilter = new HisSereServView1Filter();
                        sereServFilter.SERVICE_REQ_PARENT_ID = serviceReq.ID;
                        sereServs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_1>>(
                            "api/HisSereServ/GetView1", ApiConsumers.MosConsumer, sereServFilter, param);
                    }
                }

                MPS.Processor.Mps000108.PDO.Mps000108PDO mps000108PDO = new MPS.Processor.Mps000108.PDO.Mps000108PDO(
                    expMest,
                    expMestBltys,
                    treatment,
                    serviceReq,
                    expMestBloods,
                    treatmentBedRooms,
                    sereServs);

                // May in cau hinh san cho ma mau in nay, neu co.
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                // Du lieu ky so dien tu. Chep theo khuon Ke don mau:
                // frmHisAssignBlood__Plus__Print.cs dong 156-158.
                Inventec.Common.SignLibrary.ADO.InputADO inputADO =
                    new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor()
                        .GenerateInputADOWithPrintTypeCode(
                            (treatment != null ? treatment.TREATMENT_CODE : ""),
                            printTypeCode,
                            this.currentModule.RoomId);

                WaitingManager.Hide();

                // PHAI dung PreviewType.EmrShow.
                // Enum PreviewType co 9 gia tri, chia 2 nhom:
                //   - Nhom in thuong: Show=0, ShowDialog=1, PrintNow=2, SaveFile=3
                //     -> chi dung file roi hien thi/in, KHONG he cham toi EMR.
                //   - Nhom EMR: EmrShow=4, EmrSignNow=5, EmrSignAndPrintNow=6,
                //     EmrCreateDocument=7, EmrSignAndPrintPreview=8
                //     -> moi di qua popup EMR, noi goi VerifyHisCode de tra ra van ban DA KY.
                // EmrShow goi EmrShowClick() (AbstractProcessor.cs:1270), truyen emrInputADO
                // mang HisCode vao popup, popup tra ra ban da ky neu co; chua co thi cho ky.
                // Da vap 2 lan: dung ShowDialog roi PrintNow, ca hai deu ra ban in moi chua ky
                // vi khong thuoc nhom EMR (17/09/2026).
                MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(
                    printTypeCode,
                    fileName,
                    mps000108PDO,
                    MPS.ProcessorBase.PrintConfig.PreviewType.EmrShow,
                    printerName) { EmrInputADO = inputADO });
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void repositoryItemButtonViewDetail_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var rowDataExpMest = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                V_HIS_EXP_MEST ExpMestData = new V_HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(ExpMestData, rowDataExpMest);

                if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__PL)
                {
                    HIS.Desktop.ADO.ApproveAggrExpMestSDO exeMestView = new HIS.Desktop.ADO.ApproveAggrExpMestSDO(ExpMestData.ID, ExpMestData.EXP_MEST_STT_ID);
                    List<object> listArgs = new List<object>();
                    listArgs.Add(exeMestView);
                    CallModule callModule = new CallModule(CallModule.ApproveAggrExpMest, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
                else if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(ExpMestData);
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                    CallModule callModule = new CallModule(CallModule.AggrExpMestDetail, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
                else if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(ExpMestData);
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                    new CallModule("HIS.Desktop.Plugins.ExpMestDetailBCS", this.roomId, this.roomTypeId, listArgs);
                    WaitingManager.Hide();
                }
                else
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(ExpMestData);
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                    CallModule callModule = new CallModule(CallModule.ExpMestViewDetail, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void ButtonEnableEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowDataExpMest = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                V_HIS_EXP_MEST expMestData = new V_HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(expMestData, rowDataExpMest);
                if (expMestData != null)
                {
                    if (expMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__KHAC)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(expMestData.ID);
                        listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                        CallModule callModule = new CallModule(CallModule.ExpMestOtherExport, this.roomId, this.roomTypeId, listArgs);


                    }
                    else if (expMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(expMestData.ID);
                        listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                        if (HisConfigCFG.EXP_MEST_SALE__MODULE_UPDATE_OPTION_SELECT == "1")
                        {
                            CallModule callModule = new CallModule(CallModule.ExpMestSaleCreateV2, this.roomId, this.roomTypeId, listArgs);
                        }
                        else
                        {
                            CallModule callModule = new CallModule(CallModule.ExpMestSaleCreate, this.roomId, this.roomTypeId, listArgs);
                        }
                    }
                    else if (expMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(expMestData);
                        CallModule callModule = new CallModule(CallModule.ExpMestChmsUpdate, this.roomId, this.roomTypeId, listArgs);

                        RefreshData();
                    }
                    else if (expMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(expMestData);
                        CallModule callModule = new CallModule(CallModule.ExpMestDepaUpdate, this.roomId, this.roomTypeId, listArgs);

                        RefreshData();
                    }
                    else if (expMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__TNCC)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(expMestData);
                        CallModule callModule = new CallModule(CallModule.ManuExpMestCreate, this.roomId, this.roomTypeId, listArgs);

                        RefreshData();
                    }
                    else
                        MessageManager.Show(Resources.ResourceMessage.ChucNangDangPhatTrienVuiLongThuLaiSau);
                }
                else
                    MessageManager.Show(Resources.ResourceMessage.ChucNangDangPhatTrienVuiLongThuLaiSau);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEnableDiscard_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (DevExpress.XtraEditors.XtraMessageBox.Show(
                    Resources.ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong,
                    Resources.ResourceMessage.ThongBao,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var rowDataExpMest = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                    V_HIS_EXP_MEST row = new V_HIS_EXP_MEST();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(row, rowDataExpMest);
                    if (row != null)
                    {
                        WaitingManager.Show();
                        if (row.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                        {
                            HisExpMestSDO sdo = new HisExpMestSDO();
                            sdo.ExpMestId = row.ID;
                            sdo.ReqRoomId = this.roomId;
                            if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS)
                            {
                                if (row.BCS_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST.BCS_TYPE__ID__PRES_DETAIL)
                                {
                                    var apiresul = new BackendAdapter(param).Post<bool>("/api/HisExpMest/BaseCompensationDelete", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                                    if (apiresul)
                                    {
                                        success = true;
                                        RefreshData();
                                    }
                                }
                                else if (row.BCS_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST.BCS_TYPE__ID__BASE)
                                {
                                    var apiresul = new BackendAdapter(param).Post<bool>("/api/HisExpMest/CompensationByBaseDelete", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                                    if (apiresul)
                                    {
                                        success = true;
                                        RefreshData();
                                    }
                                }
                                else
                                {
                                    var apiresul = new BackendAdapter(param).Post<bool>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_DELETE, ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                                    if (apiresul)
                                    {
                                        success = true;
                                        RefreshData();
                                    }
                                }
                            }
                            else
                            {
                                var apiresul = new BackendAdapter(param).Post<bool>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_DELETE, ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                                if (apiresul)
                                {
                                    success = true;
                                    RefreshData();
                                }
                            }
                        }
                        else
                        {
                            var apiresul = new BackendAdapter(param).Post<bool>("api/HisExpMest/AggrExamDelete", ApiConsumer.ApiConsumers.MosConsumer, row.ID, param);
                            if (apiresul)
                            {
                                success = true;
                                RefreshData();
                            }
                        }

                        WaitingManager.Hide();
                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void ButtonEnableApproval_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {

                var rowDataExpMest = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                V_HIS_EXP_MEST row = new V_HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(row, rowDataExpMest);
                bool success = false;
                bool success_ = true;
                if (row != null)
                {
                    if (row.EXP_MEST_TYPE_CODE == "01")
                    {  // Đơn phòng khám
                        DateTime? t1 = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(row.CREATE_TIME ?? 0);
                        DateTime? t2 = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(row.MODIFY_TIME ?? 0);
                        string TB;

                        if (t1 != null && t2 != null)
                        {
                            TimeSpan? diff = t2 - t1;
                            TimeSpan interval = new TimeSpan(0, 0, 60);
                            if (diff > interval)
                            {
                                if (row.EXP_MEST_TYPE_CODE == "01")
                                {

                                    if (HisConfigCFG.WARM_MODIFIEDPRESCRIPTIONOPTION == "1")
                                    {
                                        TB = "Phiếu xuất " + row.EXP_MEST_CODE + " đã có sự chỉnh sửa. Bạn có chắc muốn duyệt không?";
                                        if (DevExpress.XtraEditors.XtraMessageBox.Show(TB, "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                        {
                                            success_ = true;
                                        }
                                        else
                                        {
                                            success_ = false;
                                        }
                                    }
                                    else
                                    {
                                        success_ = true;
                                    }
                                }

                            }
                        }
                    }
                    if (success_)
                    {
                        if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP
                             || row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM
                             || (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK && rowDataExpMest.IS_REQUEST_BY_PACKAGE != 1)
                        )
                        {
                            List<object> listArgs = new List<object>();
                            listArgs.Add(row.ID);
                            listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                            CallModule callModule = new CallModule(CallModule.BrowseExportTicket, this.roomId, this.roomTypeId, listArgs);
                            WaitingManager.Hide();

                        }
                        else if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS)
                        {
                            List<object> listArgs = new List<object>();
                            listArgs.Add(row.ID);
                            listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                            CallModule callModule = new CallModule(CallModule.ApprovalExpMestBcs, this.roomId, this.roomTypeId, listArgs);
                            WaitingManager.Hide();
                        }
                        else if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT)
                        {
                            WaitingManager.Show();
                            //bool success = false;
                            CommonParam param = new CommonParam();
                            HisExpMestSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestSDO();
                            hisExpMestApproveSDO.ExpMestId = row.ID;
                            hisExpMestApproveSDO.ReqRoomId = this.roomId;
                            if (gridControl.DataSource != null)
                            {
                                var datagridcontrol = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>(
                           "api/HisExpMest/InPresApprove", ApiConsumers.MosConsumer, hisExpMestApproveSDO, param);
                                if (rs != null && rs.ID != null)
                                {
                                    foreach (var item in datagridcontrol)
                                    {
                                        if (item.ID == rs.ID)
                                        {
                                            var ExpMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == rs.EXP_MEST_STT_ID);
                                            item.EXP_MEST_STT_ID = ExpMestSTT.ID;
                                            item.EXP_MEST_STT_CODE = ExpMestSTT.EXP_MEST_STT_CODE;
                                            item.EXP_MEST_STT_NAME = ExpMestSTT.EXP_MEST_STT_NAME;
                                            item.LAST_APPROVAL_LOGINNAME = rs.LAST_APPROVAL_LOGINNAME;
                                            item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                            break;
                                        }
                                    }
                                    success = true;
                                    gridView.BeginUpdate();
                                    datagridcontrol = datagridcontrol.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                    gridControl.DataSource = datagridcontrol;
                                    gridView.EndUpdate();
                                }
                            }
                            WaitingManager.Hide();
                            #region Show message
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                            #endregion

                        }
                        else if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                        {
                            WaitingManager.Show();
                            //bool success = false;
                            CommonParam param = new CommonParam();
                            HisExpMestSDO hisExpMestSDO = new MOS.SDO.HisExpMestSDO();
                            hisExpMestSDO.ExpMestId = row.ID;
                            hisExpMestSDO.ReqRoomId = this.roomId;

                            var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<HIS_EXP_MEST>>("api/HisExpMest/AggrExamApprove", ApiConsumers.MosConsumer, hisExpMestSDO, param);
                            if (rs != null && rs.Count > 0)
                            {
                                success = true;
                                RefreshData();
                            }

                            WaitingManager.Hide();
                            #region Show message
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                            #endregion

                        }
                        else
                        {
                            WaitingManager.Show();
                            //bool success = false;
                            CommonParam param = new CommonParam();
                            HisExpMestApproveSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestApproveSDO();

                            hisExpMestApproveSDO.ExpMestId = row.ID;
                            hisExpMestApproveSDO.IsFinish = true;
                            hisExpMestApproveSDO.ReqRoomId = this.roomId;
                            if (gridControl.DataSource != null)
                            {
                                var datagridcontrol = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisExpMestResultSDO>("api/HisExpMest/Approve", ApiConsumers.MosConsumer, hisExpMestApproveSDO, param);
                                if (rs != null)
                                {
                                    foreach (var item in datagridcontrol)
                                    {
                                        if (item.ID == rs.ExpMest.ID)
                                        {
                                            var ExpMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == rs.ExpMest.EXP_MEST_STT_ID);
                                            item.EXP_MEST_STT_ID = ExpMestSTT.ID;
                                            item.EXP_MEST_STT_CODE = ExpMestSTT.EXP_MEST_STT_CODE;
                                            item.EXP_MEST_STT_NAME = ExpMestSTT.EXP_MEST_STT_NAME;
                                            item.LAST_APPROVAL_LOGINNAME = rs.ExpMest.LAST_APPROVAL_LOGINNAME;
                                            item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                            break;
                                        }
                                    }
                                    success = true;
                                    gridView.BeginUpdate();
                                    datagridcontrol = datagridcontrol.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                    gridControl.DataSource = datagridcontrol;
                                    gridView.EndUpdate();
                                }
                            }
                            WaitingManager.Hide();
                            #region Show message
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                            #endregion
                        }

                        CommonParam paramMediStock = new CommonParam();
                        HisMediStockExtyFilter extyFilter = new HisMediStockExtyFilter();
                        extyFilter.MEDI_STOCK_ID = row.MEDI_STOCK_ID;
                        var listMediStockExty = new BackendAdapter(paramMediStock).Get<List<HIS_MEDI_STOCK_EXTY>>("api/HisMediStockExty/Get", ApiConsumers.MosConsumer, extyFilter, paramMediStock).ToList();
                        if (listMediStockExty != null && listMediStockExty.Count > 0)
                        {
                            if (listMediStockExty.FirstOrDefault().IS_AUTO_EXECUTE == 1 && success && chkInHDSD.Checked)
                            {
                                Inventec.Common.Logging.LogSystem.Debug("Thực hiện gọi hàm in HDSD ");
                                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                                store.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__HuongDanSuDungThuoc_MPS000099, deletePrintTemplate);
                            }
                        }
                    }





                    //WaitingManager.Show();
                    //bool success = false;
                    //CommonParam param = new CommonParam();
                    //MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2 row = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                    ////if (row.EXP_MEST_TYPE_ID == Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__CHMS)
                    ////{
                    ////    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.BrowseExportTicket").FirstOrDefault();
                    ////    if (moduleData == null)
                    ////    {
                    ////        Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.BrowseExportTicket");
                    ////        MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                    ////    }
                    ////    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    ////    {
                    ////        List<object> listArgs = new List<object>();
                    ////        listArgs.Add(row.ID);
                    ////        listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                    ////        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                    ////        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ////        WaitingManager.Hide();
                    ////        ((Form)extenceInstance).ShowDialog();
                    ////    }
                    ////    else
                    ////    {
                    ////        MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                    ////    }
                    ////}
                    ////else
                    ////{
                    //MOS.EFMODEL.DataModels.HIS_EXP_MEST data = new MOS.EFMODEL.DataModels.HIS_EXP_MEST();
                    //Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_EXP_MEST>(data, row);
                    //data.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE;
                    //var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_UPDATE_STATUS, ApiConsumer.ApiConsumers.MosConsumer, data, param);
                    //if (apiresul != null)
                    //{
                    //    success = true;
                    //    RefreshData();
                    //}
                    //WaitingManager.Hide();
                    //#region Show message
                    //Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                    //#endregion

                    //#region Process has exception
                    //HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    //#endregion
                    ////}
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void ButtonEnableDisApproval_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowDataExpMest = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                V_HIS_EXP_MEST row = new V_HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(row, rowDataExpMest);
                CommonParam param = new CommonParam();
                bool success = false;
                if (row != null)
                {
                    WaitingManager.Show();

                    HisExpMestSDO ado = new HisExpMestSDO();
                    ado.ExpMestId = row.ID;
                    ado.ReqRoomId = this.roomId;
                    if (gridControl.DataSource != null)
                    {
                        var datagridcontrol = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                        var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/Decline", ApiConsumer.ApiConsumers.MosConsumer, ado, param);
                        if (apiresul != null)
                        {
                            foreach (var item in datagridcontrol)
                            {
                                if (item.ID == apiresul.ID)
                                {
                                    var ExpMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresul.EXP_MEST_STT_ID);
                                    item.EXP_MEST_STT_ID = ExpMestSTT.ID;
                                    item.EXP_MEST_STT_CODE = ExpMestSTT.EXP_MEST_STT_CODE;
                                    item.EXP_MEST_STT_NAME = ExpMestSTT.EXP_MEST_STT_NAME;
                                    item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                    break;
                                }
                            }
                            success = true;
                            gridView.BeginUpdate();
                            datagridcontrol = datagridcontrol.OrderByDescending(p => p.MODIFY_TIME).ToList();
                            gridControl.DataSource = datagridcontrol;
                            gridView.EndUpdate();
                        }
                    }
                }
                WaitingManager.Hide();
                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }
        private void checkExport(long? expTime)
        {
            try
            {
                isExpWithExpTime = false;
                ExpTime = expTime;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void ButtonEnableActualExport_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                isExpWithExpTime = false;
                bool success = false;
                CommonParam param = new CommonParam();
                var rowDataExpMest = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                V_HIS_EXP_MEST row = new V_HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(row, rowDataExpMest);
                if (row != null)
                {
                    if (rowDataExpMest != null && rowDataExpMest.EXP_MEST_TYPE_ID == 1 && HisConfigCFG.AllowEditExpTime == "1")
                    {
                        isExpWithExpTime = true;
                        frmMessage frm = new frmMessage(row, checkExport);
                        frm.ShowDialog();
                    }
                    if (isExpWithExpTime == false)
                    {
                        if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS)
                        {
                            bool IsFinish = false;
                            if (row.IS_EXPORT_EQUAL_APPROVE == 1)
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show("Đã xuất hết số lượng duyệt", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                            else if (row.IS_EXPORT_EQUAL_APPROVE == null || row.IS_EXPORT_EQUAL_APPROVE != 1)
                            {
                                HisExpMestMetyReqFilter expMestMetyReqFilter = new HisExpMestMetyReqFilter();
                                expMestMetyReqFilter.EXP_MEST_ID = row.ID;

                                var listExpMestMetyReq = new BackendAdapter(param).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/Get", ApiConsumers.MosConsumer, expMestMetyReqFilter, param);

                                HisExpMestMatyReqFilter expMestMatyReqFilter = new HisExpMestMatyReqFilter();
                                expMestMatyReqFilter.EXP_MEST_ID = row.ID;

                                var listExpMestMatyReq = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/Get", ApiConsumers.MosConsumer, expMestMatyReqFilter, param);

                                List<AmountADO> amountAdo = new List<AmountADO>();

                                if (listExpMestMetyReq != null && listExpMestMetyReq.Count > 0)
                                {
                                    foreach (var item in listExpMestMetyReq)
                                    {
                                        var ado = new AmountADO(item);
                                        amountAdo.Add(ado);
                                    }
                                }

                                if (listExpMestMatyReq != null && listExpMestMatyReq.Count > 0)
                                {
                                    foreach (var item in listExpMestMatyReq)
                                    {
                                        var ado = new AmountADO(item);
                                        amountAdo.Add(ado);
                                    }
                                }

                                if (amountAdo != null && amountAdo.Count > 0)
                                {
                                    var dataAdo = amountAdo.Where(o => o.Amount > o.Dd_Amount || o.Dd_Amount == null).ToList();
                                    //if (dataAdo != null && dataAdo.Count > 0)
                                    //{
                                    //    if (XtraMessageBox.Show("Phiếu chưa duyệt đủ số lượng yêu cầu. Bạn có muốn hoàn thành phiếu xuất?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                    //    {
                                    //        IsFinish = true;
                                    //    }
                                    //}
                                    //else
                                    IsFinish = true;
                                }

                            }

                            HisExpMestExportSDO sdo = new HisExpMestExportSDO();
                            sdo.ExpMestId = row.ID;
                            sdo.ReqRoomId = this.roomId;
                            sdo.IsFinish = IsFinish;
                            if (ExpTime != null && ExpTime > 0)
                            {
                                sdo.ExpTime = ExpTime;
                            }
                            if (gridControl.DataSource != null)
                            {
                                var datagridcontrol = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                var apiresult = new Inventec.Common.Adapter.BackendAdapter
                                    (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                                    (ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_EXPORT, ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                                if (apiresult != null)
                                {
                                    foreach (var item in datagridcontrol)
                                    {
                                        if (item.ID == apiresult.ID)
                                        {
                                            var ExpMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresult.EXP_MEST_STT_ID);
                                            item.EXP_MEST_STT_ID = ExpMestSTT.ID;
                                            item.EXP_MEST_STT_CODE = ExpMestSTT.EXP_MEST_STT_CODE;
                                            item.EXP_MEST_STT_NAME = ExpMestSTT.EXP_MEST_STT_NAME;
                                            item.LAST_EXP_LOGINNAME = apiresult.LAST_EXP_LOGINNAME;
                                            item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                            break;
                                        }
                                    }
                                    success = true;
                                    gridView.BeginUpdate();
                                    datagridcontrol = datagridcontrol.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                    gridControl.DataSource = datagridcontrol;
                                    gridView.EndUpdate();
                                }
                            }
                            #region Show message
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                            #endregion

                        }
                        else if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT)
                        {
                            HisExpMestSDO sdo = new HisExpMestSDO();
                            sdo.ExpMestId = row.ID;
                            sdo.ReqRoomId = this.roomId;
                            //sdo.IsFinish = true;
                            if (gridControl.DataSource != null)
                            {
                                var datagridcontrol = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                var apiresult = new Inventec.Common.Adapter.BackendAdapter
                                    (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                                    ("api/HisExpMest/InPresExport", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                                if (apiresult != null)
                                {
                                    foreach (var item in datagridcontrol)
                                    {
                                        if (item.ID == apiresult.ID)
                                        {
                                            var ExpMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresult.EXP_MEST_STT_ID);
                                            item.EXP_MEST_STT_ID = ExpMestSTT.ID;
                                            item.EXP_MEST_STT_CODE = ExpMestSTT.EXP_MEST_STT_CODE;
                                            item.EXP_MEST_STT_NAME = ExpMestSTT.EXP_MEST_STT_NAME;
                                            item.LAST_EXP_LOGINNAME = apiresult.LAST_EXP_LOGINNAME;
                                            item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                            break;
                                        }
                                    }
                                    success = true;
                                    gridView.BeginUpdate();
                                    datagridcontrol = datagridcontrol.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                    gridControl.DataSource = datagridcontrol;
                                    gridView.EndUpdate();
                                }
                            }
                            #region Show message
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                            #endregion
                        }
                        else if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                        {
                            HisExpMestSDO sdo = new HisExpMestSDO();
                            sdo.ExpMestId = row.ID;
                            sdo.ReqRoomId = this.roomId;
                            //sdo.IsFinish = true;
                            if (gridControl.DataSource != null)
                            {
                                var datagridcontrol = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                var apiresult = new Inventec.Common.Adapter.BackendAdapter
                                    (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                                    ("api/HisExpMest/AggrExamExport", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                                if (apiresult != null)
                                {
                                    foreach (var item in datagridcontrol)
                                    {
                                        if (item.ID == apiresult.ID)
                                        {
                                            var ExpMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresult.EXP_MEST_STT_ID);
                                            item.EXP_MEST_STT_ID = ExpMestSTT.ID;
                                            item.EXP_MEST_STT_CODE = ExpMestSTT.EXP_MEST_STT_CODE;
                                            item.EXP_MEST_STT_NAME = ExpMestSTT.EXP_MEST_STT_NAME;
                                            item.LAST_EXP_LOGINNAME = apiresult.LAST_EXP_LOGINNAME;
                                            item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                            break;
                                        }
                                    }
                                    success = true;
                                    gridView.BeginUpdate();
                                    datagridcontrol = datagridcontrol.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                    gridControl.DataSource = datagridcontrol;
                                    gridView.EndUpdate();
                                }
                            }
                            #region Show message
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                            #endregion

                        }
                        else
                        {
                            HisExpMestExportSDO sdo = new HisExpMestExportSDO();
                            sdo.ExpMestId = row.ID;
                            sdo.ReqRoomId = this.roomId;
                            sdo.IsFinish = true;
                            if (ExpTime != null && ExpTime > 0)
                            {
                                sdo.ExpTime = ExpTime;
                            }
                            if (gridControl.DataSource != null)
                            {
                                var datagridcontrol = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                var apiresult = new Inventec.Common.Adapter.BackendAdapter
                                    (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                                    (ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_EXPORT, ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                                if (apiresult != null)
                                {
                                    foreach (var item in datagridcontrol)
                                    {
                                        if (item.ID == apiresult.ID)
                                        {
                                            var ExpMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresult.EXP_MEST_STT_ID);
                                            item.EXP_MEST_STT_ID = ExpMestSTT.ID;
                                            item.EXP_MEST_STT_CODE = ExpMestSTT.EXP_MEST_STT_CODE;
                                            item.EXP_MEST_STT_NAME = ExpMestSTT.EXP_MEST_STT_NAME;
                                            item.LAST_EXP_LOGINNAME = apiresult.LAST_EXP_LOGINNAME;
                                            item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                            break;
                                        }
                                    }
                                    success = true;
                                    gridView.BeginUpdate();
                                    datagridcontrol = datagridcontrol.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                    gridControl.DataSource = datagridcontrol;
                                    gridView.EndUpdate();
                                }
                            }
                            #region Show message
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                            #endregion

                        }
                    }
                }
                if (success && chkInHDSD.Checked)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Thực hiện gọi hàm in HDSD ");
                    Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    store.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__HuongDanSuDungThuoc_MPS000099, deletePrintTemplate);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void ButtonCopyExportMest_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var expMestData = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                //SessionManager.GetFormMain().ExportMedicineForCopyExpMestClick(expMestData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEnableMobaImpCreate_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var rowDataExpMest = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                V_HIS_EXP_MEST ExpMestData = new V_HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(ExpMestData, rowDataExpMest);

                if (ExpMestData != null)
                {
                    if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(ExpMestData.ID);
                        CallModule callModule = new CallModule(CallModule.MobaDepaCreate, this.roomId, this.roomTypeId, listArgs);

                        WaitingManager.Hide();
                        FillDataApterClose(ExpMestData);
                    }
                    else if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(ExpMestData.ID);
                        CallModule callModule = new CallModule(CallModule.MobaBloodCreate, this.roomId, this.roomTypeId, listArgs);

                        WaitingManager.Hide();
                        FillDataApterClose(ExpMestData);
                    }
                    else if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(ExpMestData.ID);
                        CallModule callModule = new CallModule(CallModule.MobaSaleCreate, this.roomId, this.roomTypeId, listArgs);

                        WaitingManager.Hide();
                        FillDataApterClose(ExpMestData);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataApterClose(MOS.EFMODEL.DataModels.V_HIS_EXP_MEST ExpMestData)
        {
            try
            {
                FillDataToControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataApterSave(object prescription)
        {
            try
            {
                if (prescription != null)
                {
                    if (gridControl.DataSource != null)
                    {
                        HisExpMestResultSDO ado = new HisExpMestResultSDO();
                        if (prescription is HisExpMestResultSDO)
                            ado = (HisExpMestResultSDO)prescription;
                        else if (prescription is HIS_EXP_MEST)
                        {
                            ado.ExpMest = new HIS_EXP_MEST();
                            ado.ExpMest = (HIS_EXP_MEST)prescription;
                        }

                        List<V_HIS_EXP_MEST_2> datagridcontrol = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                        foreach (var item in datagridcontrol)
                        {
                            if (item.ID == ado.ExpMest.ID)
                            {
                                var ExpMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == ado.ExpMest.EXP_MEST_STT_ID);
                                item.EXP_MEST_STT_ID = ExpMestSTT.ID;
                                item.EXP_MEST_STT_CODE = ExpMestSTT.EXP_MEST_STT_CODE;
                                item.EXP_MEST_STT_NAME = ExpMestSTT.EXP_MEST_STT_NAME;
                                item.TDL_BLOOD_CODE = ado.ExpMest.TDL_BLOOD_CODE;
                                item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                break;
                            }
                        }
                        gridView.BeginUpdate();
                        datagridcontrol = datagridcontrol.OrderByDescending(p => p.MODIFY_TIME).ToList();
                        gridControl.DataSource = datagridcontrol;
                        gridView.EndUpdate();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonRequest_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn hủy duyệt không?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var rowDataExpMest = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                    V_HIS_EXP_MEST row = new V_HIS_EXP_MEST();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(row, rowDataExpMest);
                    if (row != null)
                    {
                        if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                        {
                            WaitingManager.Show();
                            bool success = false;
                            CommonParam param = new CommonParam();

                            HisExpMestSDO data = new HisExpMestSDO();
                            data.ExpMestId = row.ID;
                            data.ReqRoomId = this.roomId;
                            if (gridControl.DataSource != null)
                            {
                                var datagridcontrol = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/AggrExamUnapprove", ApiConsumer.ApiConsumers.MosConsumer, data, param);
                                if (apiresul != null)
                                {
                                    foreach (var item in datagridcontrol)
                                    {
                                        if (item.ID == apiresul.ID)
                                        {
                                            var ExpMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresul.EXP_MEST_STT_ID);
                                            item.EXP_MEST_STT_ID = ExpMestSTT.ID;
                                            item.EXP_MEST_STT_CODE = ExpMestSTT.EXP_MEST_STT_CODE;
                                            item.EXP_MEST_STT_NAME = ExpMestSTT.EXP_MEST_STT_NAME;
                                            item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                            break;
                                        }
                                    }
                                    success = true;
                                    gridView.BeginUpdate();
                                    datagridcontrol = datagridcontrol.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                    gridControl.DataSource = datagridcontrol;
                                    gridView.EndUpdate();
                                }
                            }
                            WaitingManager.Hide();
                            #region Show message
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                            #endregion
                        }
                        else if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT)
                        {
                            WaitingManager.Show();
                            bool success = false;
                            CommonParam param = new CommonParam();

                            HisExpMestSDO data = new HisExpMestSDO();
                            data.ExpMestId = row.ID;
                            data.ReqRoomId = this.roomId;
                            if (gridControl.DataSource != null)
                            {
                                var datagridcontrol = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/InPresUnapprove", ApiConsumer.ApiConsumers.MosConsumer, data, param);
                                if (apiresul != null)
                                {
                                    foreach (var item in datagridcontrol)
                                    {
                                        if (item.ID == apiresul.ID)
                                        {
                                            var ExpMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresul.EXP_MEST_STT_ID);
                                            item.EXP_MEST_STT_ID = ExpMestSTT.ID;
                                            item.EXP_MEST_STT_CODE = ExpMestSTT.EXP_MEST_STT_CODE;
                                            item.EXP_MEST_STT_NAME = ExpMestSTT.EXP_MEST_STT_NAME;
                                            item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                            break;
                                        }
                                    }
                                    success = true;
                                    gridView.BeginUpdate();
                                    datagridcontrol = datagridcontrol.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                    gridControl.DataSource = datagridcontrol;
                                    gridView.EndUpdate();
                                }
                            }
                            WaitingManager.Hide();
                            #region Show message
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                            #endregion

                        }
                        else
                        {
                            WaitingManager.Show();
                            bool success = false;
                            CommonParam param = new CommonParam();

                            HisExpMestSDO data = new HisExpMestSDO();
                            data.ExpMestId = row.ID;
                            data.ReqRoomId = this.roomId;
                            if (gridControl.DataSource != null)
                            {
                                var datagridcontrol = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/Unapprove", ApiConsumer.ApiConsumers.MosConsumer, data, param);
                                if (apiresul != null)
                                {
                                    foreach (var item in datagridcontrol)
                                    {
                                        if (item.ID == apiresul.ID)
                                        {
                                            var ExpMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresul.EXP_MEST_STT_ID);
                                            item.EXP_MEST_STT_ID = ExpMestSTT.ID;
                                            item.EXP_MEST_STT_CODE = ExpMestSTT.EXP_MEST_STT_CODE;
                                            item.EXP_MEST_STT_NAME = ExpMestSTT.EXP_MEST_STT_NAME;
                                            item.TDL_BLOOD_CODE = apiresul.TDL_BLOOD_CODE;
                                            item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                            break;
                                        }
                                    }
                                    success = true;
                                    gridView.BeginUpdate();
                                    datagridcontrol = datagridcontrol.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                    gridControl.DataSource = datagridcontrol;
                                    gridView.EndUpdate();
                                }
                            }
                            WaitingManager.Hide();
                            #region Show message
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonAssignTest_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var rowDataExpMest = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                V_HIS_EXP_MEST expMestData = new V_HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(expMestData, rowDataExpMest);
                if (expMestData != null)
                {
                    List<object> listArgs = new List<object>();
                    AssignServiceTestADO assignBloodADO = new AssignServiceTestADO(0, 0, 0, null);
                    GetTreatmentIdFromResultData(expMestData, ref assignBloodADO);
                    listArgs.Add(assignBloodADO);
                    CallModule callModule = new CallModule(CallModule.AssignServiceTest, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetTreatmentIdFromResultData(MOS.EFMODEL.DataModels.V_HIS_EXP_MEST resultExpMest, ref AssignServiceTestADO assignBloodADO)
        {
            try
            {
                long __expMestId = ((resultExpMest != null && resultExpMest.ID > 0) ? resultExpMest.ID : 0);
                MOS.Filter.HisExpMestViewFilter expFilter = new MOS.Filter.HisExpMestViewFilter();
                expFilter.ID = __expMestId;
                var listExp = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST>>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expFilter, null);
                if (resultExpMest != null && listExp.Count == 1)
                {
                    MOS.Filter.HisTreatmentView2Filter treatmentView2Filter = new MOS.Filter.HisTreatmentView2Filter();
                    treatmentView2Filter.PATIENT_ID = listExp.First().TDL_PATIENT_ID;
                    var listTreatment = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_2>>(ApiConsumer.HisRequestUriStore.HIS_TREATMENT_GETVIEW_2, ApiConsumer.ApiConsumers.MosConsumer, treatmentView2Filter, null);
                    if (listTreatment != null && listTreatment.Count == 1)
                    {
                        assignBloodADO.TreatmentId = listTreatment.First().ID;
                        assignBloodADO.GenderName = listTreatment.First().TDL_PATIENT_GENDER_NAME;
                        assignBloodADO.PatientDob = (listTreatment.First().TDL_PATIENT_DOB);
                        assignBloodADO.PatientName = listTreatment.First().TDL_PATIENT_NAME;
                        assignBloodADO.ExpMestId = __expMestId;
                        assignBloodADO.ServiceReqId = listExp.First().SERVICE_REQ_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Btn_Bill_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowDataExpMest = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                if (rowDataExpMest != null && !rowDataExpMest.BILL_ID.HasValue && rowDataExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(rowDataExpMest.ID);
                    DelegateSelectData dl = new DelegateSelectData(this.ProcessRefressDelefate);
                    listArgs.Add(dl);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.MedicineSaleBill", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Btn_CancelBill_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowDataExpMest = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                if (rowDataExpMest != null
                    && rowDataExpMest.BILL_ID.HasValue
                    && rowDataExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN)
                {
                    if (!HisConfigCFG.EXPORT_SALE__MUST_BILL || rowDataExpMest.EXP_MEST_STT_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                    {

                        List<object> listArgs = new List<object>();
                        listArgs.Add(rowDataExpMest.BILL_ID.Value);
                        listArgs.Add(rowDataExpMest);
                        // Viec 3082: sau khi huy hoa don thanh cong -> hoan kho + huy duyet de phieu ve trang thai YEU CAU.
                        // Lay ma phieu TRUOC khi huy vi BE se set BILL_ID = null khi huy giao dich (khong tim lai duoc theo bill).
                        string expMestCodeForRestore = rowDataExpMest.EXP_MEST_CODE;
                        DelegateSelectData dl = new DelegateSelectData((rs) =>
                        {
                            ExpMestRestoreStockWorker.RestoreAfterCancelInvoice(this.ParentForm, new List<string> { expMestCodeForRestore }, this.currentModule.RoomId);
                            this.ProcessRefressDelefate(rs);
                        });
                        listArgs.Add(dl);
                        HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.TransactionCancel", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                        Inventec.Common.Logging.LogSystem.Debug("End call  HIS.Desktop.Plugins.TransactionCancel");
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("NOT call 1  HIS.Desktop.Plugins.TransactionCancel");
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("NOT call 2  HIS.Desktop.Plugins.TransactionCancel");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void ProcessRefressDelefate(object data)
        {
            try
            {
                if (data != null)
                {
                    RefreshData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool deletePrintTemplate(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (!String.IsNullOrEmpty(printTypeCode) && !String.IsNullOrEmpty(fileName))
                {
                    switch (printTypeCode)
                    {
                        case PrintTypeCodeWorker.PRINT_TYPE_CODE__HuongDanSuDungThuoc_MPS000099:
                            InHuongDanSuDungThuoc(printTypeCode, fileName);
                            break;
                        case HIS.Desktop.Print.PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000108:
                            InPhieuCungCapMau(printTypeCode, fileName);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void InHuongDanSuDungThuoc(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                var rowDataExpMest = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();

                if (rowDataExpMest != null)
                {
                    CommonParam param = new CommonParam();

                    V_HIS_EXP_MEST ExpMestData = new V_HIS_EXP_MEST();
                    //List<V_HIS_EXP_MEST> lstExpMestData = new List<V_HIS_EXP_MEST>();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(ExpMestData, rowDataExpMest);

                    //lstExpMestData.Add(ExpMestData);

                    HisExpMestMedicineViewFilter expMestMedicineFilter = new HisExpMestMedicineViewFilter();
                    expMestMedicineFilter.EXP_MEST_ID = rowDataExpMest.ID;
                    List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetVIew", ApiConsumers.MosConsumer, expMestMedicineFilter, param);


                    //HisExpMestMaterialFilter expMestMaterialFilter = new HisExpMestMaterialFilter();
                    //expMestMaterialFilter.EXP_MEST_ID = rowDataExpMest.ID;
                    //List<V_HIS_EXP_MEST_MATERIAL> expMestMaterial = new BackendAdapter(param)
                    //    .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetVIew", ApiConsumers.MosConsumer, expMestMaterialFilter, param);

                    MPS.Processor.Mps000099.PDO.Mps000099PDO rdo = new MPS.Processor.Mps000099.PDO.Mps000099PDO(ExpMestData, expMestMedicines);


                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public int data { get; set; }
    }
}