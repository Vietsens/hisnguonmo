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
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Config;
using HIS.Desktop.Plugins.Library.PrintTreatmentFinish;
using HIS.Desktop.Print;
using HIS.UC.MenuPrint.ADO;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using MPS.ProcessorBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionCLS.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        List<HIS.UC.MenuPrint.ADO.MenuPrintADO> menuPrintADOs;
        private async Task InitMenuToButtonPrint()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("InitMenuToButtonPrint .1");
                HIS.UC.MenuPrint.MenuPrintProcessor menuPrintProcessor = new HIS.UC.MenuPrint.MenuPrintProcessor();
                menuPrintADOs = new List<HIS.UC.MenuPrint.ADO.MenuPrintADO>();

                //HIS.UC.MenuPrint.ADO.MenuPrintADO menuPrintADO__ThuocTongHop = new HIS.UC.MenuPrint.ADO.MenuPrintADO();
                //menuPrintADO__ThuocTongHop.EventHandler = new EventHandler(OnClickPrintWithPrintTypeCfg);
                //menuPrintADO__ThuocTongHop.PrintTypeCode = PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__IN_DON_THUOC_TONG_HOP__MPS000118;
                //menuPrintADO__ThuocTongHop.Tag = PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__IN_DON_THUOC_TONG_HOP__MPS000118;
                //menuPrintADOs.Add(menuPrintADO__ThuocTongHop);

                //HIS.UC.MenuPrint.ADO.MenuPrintADO menuPrintADO__CacDonThuoc = new HIS.UC.MenuPrint.ADO.MenuPrintADO();
                //menuPrintADO__CacDonThuoc.EventHandler = new EventHandler(OnClickPrintWithPrintTypeCfg);
                //menuPrintADO__CacDonThuoc.PrintTypeCode = PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_DON_THUOC__MPS000044;
                //menuPrintADO__CacDonThuoc.Tag = PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_DON_THUOC__MPS000044;
                //menuPrintADOs.Add(menuPrintADO__CacDonThuoc);

                //HIS.UC.MenuPrint.ADO.MenuPrintADO menuPrintADO__YHocCoTruyen = new HIS.UC.MenuPrint.ADO.MenuPrintADO();
                //menuPrintADO__YHocCoTruyen.EventHandler = new EventHandler(OnClickPrintWithPrintTypeCfg);
                //menuPrintADO__YHocCoTruyen.PrintTypeCode = PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_DON_THUOC_Y_HOC_CO_TRUYEN__MPS000050;
                //menuPrintADO__YHocCoTruyen.Tag = PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_DON_THUOC_Y_HOC_CO_TRUYEN__MPS000050;
                //menuPrintADOs.Add(menuPrintADO__YHocCoTruyen);

                HIS.UC.MenuPrint.ADO.MenuPrintADO menuPrintADO__KeKhaiThuocVatTu = new HIS.UC.MenuPrint.ADO.MenuPrintADO();
                menuPrintADO__KeKhaiThuocVatTu.EventHandler = new EventHandler(OnClickPrintWithPrintTypeCfg);
                menuPrintADO__KeKhaiThuocVatTu.PrintTypeCode = PrintTypeCodes.PRINT_TYPE_CODE__BIEUMAU__PHIEU_KE_KHAI_THUOC_VATU__MPS000338;
                menuPrintADO__KeKhaiThuocVatTu.Tag = PrintTypeCodes.PRINT_TYPE_CODE__BIEUMAU__PHIEU_KE_KHAI_THUOC_VATU__MPS000338;
                menuPrintADOs.Add(menuPrintADO__KeKhaiThuocVatTu);

                lciPrintAssignPrescription.MaxSize = new System.Drawing.Size(440, 30);
                lciPrintAssignPrescription.MinSize = new System.Drawing.Size(140, 30);

                HIS.UC.MenuPrint.ADO.MenuPrintInitADO menuPrintInitADO = new HIS.UC.MenuPrint.ADO.MenuPrintInitADO(menuPrintADOs, BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>());
                menuPrintInitADO.MinSizeHeight = this.lcibtnSave.MinSize.Height;
                menuPrintInitADO.MaxSizeHeight = this.lcibtnSave.MaxSize.Height;
                menuPrintInitADO.ControlContainer = this.layoutControlPrintAssignPrescription;
                var menuResultADO = menuPrintProcessor.Run(menuPrintInitADO) as MenuPrintResultADO;
                if (menuResultADO == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("menuPrintProcessor run fail. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => menuPrintInitADO), menuPrintInitADO));
                }
                this.lciPrintAssignPrescription.Update();
                Inventec.Common.Logging.LogSystem.Debug("InitMenuToButtonPrint .2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Hàm khi báo cho sự kiện nhấn nút in động
        /// Đặt đúng tên OnClickPrintWithPrintTypeCfg
        /// Trường hợp có nhiều nút in cần gen động trong 1 chức năng bổ sung sau, hiện tại chưa hỗ trợ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickPrintWithPrintTypeCfg(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("OnClickPrintWithPrintTypeCfg.1");
                //khi click nhiều lầu nút in sẽ chỉ hiển thị preview 1 lần
                bool isOpenPreview = false;
                foreach (Form item in Application.OpenForms)
                {
                    if (item.Name == "frmSetupPrintPreview")
                    {
                        isOpenPreview = true;
                        break;
                    }
                }
                if (isOpenPreview)
                {
                    return;
                }

                string printTypeCode = "";
                if (sender is DXMenuItem)
                {
                    var bbtnItem = sender as DXMenuItem;
                    printTypeCode = (bbtnItem.Tag ?? "").ToString();
                }
                else if (sender is SimpleButton)
                {
                    var bbtnItem = sender as SimpleButton;
                    printTypeCode = (bbtnItem.Tag ?? "").ToString();
                }
                //qtcode
                PrescriptionPrintShow(printTypeCode, true, PrintConfig.PreviewType.EmrSignAndPrintNow);

                //PrescriptionPrintShow(printTypeCode, false);
                Inventec.Common.Logging.LogSystem.Debug("OnClickPrintWithPrintTypeCfg.2____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => printTypeCode), printTypeCode));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PrescriptionPrintNow()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == this.ModuleLink).FirstOrDefault();
                if (!string.IsNullOrEmpty(HisConfigCFG.AllowSignaturePrintModuleLinks) && ("," + HisConfigCFG.AllowSignaturePrintModuleLinks + ",").Contains("," + this.ModuleLink + ",") && moduleData != null)
                {
                    PrescriptionPrintShow(PrintTypeCodes.PRINT_TYPE_CODE__BIEUMAU__PHIEU_KE_KHAI_THUOC_VATU__MPS000338, true, PrintConfig.PreviewType.EmrSignAndPrintNow);
                }
                else
                {
                    PrescriptionPrintShow(PrintTypeCodes.PRINT_TYPE_CODE__BIEUMAU__PHIEU_KE_KHAI_THUOC_VATU__MPS000338, true, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PrescriptionPrintShow(string printTypeCode, bool isPrintNow, PrintConfig.PreviewType? previewType = null)
        //private void PrescriptionPrintShow(string printTypeCode, bool isPrintNow, bool check, )
        {
            try
            {
                HIS.Desktop.Plugins.Library.PrintPrescription.PrintPrescriptionProcessor printPrescriptionProcessor;

                List<SubclinicalPresResultSDO> OutPatientPresResultSDOForPrints = new List<SubclinicalPresResultSDO>();
                SubclinicalPresResultSDO OutPatientPresResultSDO = new SubclinicalPresResultSDO();

                if (outPrescriptionResultSDOs != null)
                    OutPatientPresResultSDOForPrints.Add(outPrescriptionResultSDOs);
                else if (this.oldServiceReq != null)
                {
                    if (this.oldExpMest != null)
                    {
                        OutPatientPresResultSDO.ExpMests = new List<HIS_EXP_MEST>();
                        OutPatientPresResultSDO.ExpMests.Add(oldExpMest);
                    }

                    if (this.oldServiceReq != null)
                    {
                        OutPatientPresResultSDO.ServiceReqs = new List<HIS_SERVICE_REQ>();
                        OutPatientPresResultSDO.ServiceReqs.Add(this.oldServiceReq);
                    }

                    if (this.expMestMedicineEditPrints != null)
                    {
                        AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MEDICINE, HIS_EXP_MEST_MEDICINE>();
                        OutPatientPresResultSDO.Medicines = AutoMapper.Mapper.Map<List<HIS_EXP_MEST_MEDICINE>>(this.expMestMedicineEditPrints);
                    }

                    if (this.expMestMaterialEditPrints != null)
                    {
                        AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MATERIAL, HIS_EXP_MEST_MATERIAL>();
                        OutPatientPresResultSDO.Materials = AutoMapper.Mapper.Map<List<HIS_EXP_MEST_MATERIAL>>(this.expMestMaterialEditPrints);
                    }

                    OutPatientPresResultSDOForPrints.Add(OutPatientPresResultSDO);
                }
                //MPS.ProcessorBase.PrintConfig.PreviewType previewType;
                printPrescriptionProcessor = new Library.PrintPrescription.PrintPrescriptionProcessor(OutPatientPresResultSDOForPrints, this.currentSereServ, this.currentModule);
                 printPrescriptionProcessor.Print(printTypeCode, isPrintNow, previewType);
                //printPrescriptionProcessor.Print(printTypeCode, isPrintNow, previewType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private bool HasSigner()
        //{
        //    return MPS.ProcessorBase.PrintConfig.EmrSigners != null
        //        && MPS.ProcessorBase.PrintConfig.EmrSigners.Count > 0;
        //}

        //private void PrescriptionSavePrintShow(string printTypeCode, bool isPrintNow)
        //{
        //    try
        //    {
        //        if (printTypeCode == PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_RA_VIEN__MPS000008
        //            || printTypeCode == PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_HEN_KHAM__MPS000010
        //            || printTypeCode == PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_CHUYEN_VIEN__MPS000011)
        //        {
        //            HIS_TREATMENT treatment = new HIS_TREATMENT();
        //            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(treatment, currentTreatmentWithPatientType);

        //            PrintTreatmentFinishProcessor printTreatmentFinishProcessor = new PrintTreatmentFinishProcessor(treatment, currentModule != null ? currentModule.RoomId : 0);
        //            if (printTypeCode == PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_HEN_KHAM__MPS000010)
        //            {
        //                printTreatmentFinishProcessor.Print(MPS.Processor.Mps000010.PDO.Mps000010PDO.printTypeCode, isPrintNow);
        //            }
        //            else if (printTypeCode == PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_CHUYEN_VIEN__MPS000011)
        //            {
        //                printTreatmentFinishProcessor.Print(MPS.Processor.Mps000011.PDO.Mps000011PDO.printTypeCode, isPrintNow);
        //            }
        //            else
        //            {
        //                printTreatmentFinishProcessor.Print(MPS.Processor.Mps000008.PDO.Mps000008PDO.printTypeCode, isPrintNow);
        //            }
        //        }
        //        else
        //        {
        //            HIS.Desktop.Plugins.Library.PrintPrescription.PrintPrescriptionProcessor printPrescriptionProcessor;
        //            if (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
        //            {
        //                List<InPatientPresResultSDO> InPatientPresResultSDOForPrints = new List<InPatientPresResultSDO>();
        //                InPatientPresResultSDOForPrints.Add(inPrescriptionResultSDOs);
        //                printPrescriptionProcessor = new Library.PrintPrescription.PrintPrescriptionProcessor(InPatientPresResultSDOForPrints, this.currentModule);
        //            }
        //            else
        //            {
        //                List<OutPatientPresResultSDO> OutPatientPresResultSDOForPrints = new List<OutPatientPresResultSDO>();
        //                OutPatientPresResultSDO OutPatientPresResultSDO = new OutPatientPresResultSDO();

        //                if (String.IsNullOrEmpty(printTypeCode) && ((this.expMestMedicinePrints != null && this.expMestMedicinePrints.Count > 0)
        //                    || (this.expMestMaterialPrints != null && this.expMestMaterialPrints.Count > 0)))
        //                {
        //                    if (this.expMestPrints != null)
        //                    {
        //                        OutPatientPresResultSDO.ExpMests = expMestPrints;
        //                    }

        //                    if (this.serviceReqPrints != null)
        //                    {
        //                        OutPatientPresResultSDO.ServiceReqs = serviceReqPrints;
        //                    }

        //                    if (this.expMestMedicinePrints != null)
        //                    {
        //                        OutPatientPresResultSDO.Medicines = expMestMedicinePrints;
        //                    }

        //                    if (this.expMestMaterialPrints != null)
        //                    {
        //                        OutPatientPresResultSDO.Materials = expMestMaterialPrints;
        //                    }
        //                }

        //                if (outPrescriptionResultSDOs != null)
        //                    OutPatientPresResultSDOForPrints.Add(outPrescriptionResultSDOs);

        //                OutPatientPresResultSDOForPrints.Add(OutPatientPresResultSDO);
        //                printPrescriptionProcessor = new Library.PrintPrescription.PrintPrescriptionProcessor(OutPatientPresResultSDOForPrints, this.currentModule);
        //            }
        //            if (isPrintNow)
        //                printPrescriptionProcessor.Print();
        //            else
        //                printPrescriptionProcessor.Print(printTypeCode, isPrintNow);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
    }
}
