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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.PrintBordereau;
using HIS.Desktop.Plugins.Library.PrintBordereau.ADO;
using HIS.Desktop.Plugins.TransactionBill.Base;
using HIS.Desktop.Plugins.TransactionBill.Config;
using HIS.Desktop.Utilities;
using HIS.UC.MenuPrint;
using HIS.UC.MenuPrint.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.LibraryHein.Bhyt.HeinTreatmentType;
using MPS.ADO.Bordereau;
using Newtonsoft.Json;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TransactionBill
{
    public partial class frmTransactionBill : HIS.Desktop.Utility.FormBase
    {
        private void FillDataToButtonPrint()
        {
            try
            {
                if (this.currentTreatment == null)
                    return;

                WaitingManager.Show();

                ReloadMenuOption reloadMenuBordereau = new ReloadMenuOption();
                reloadMenuBordereau.ReloadMenu = ReloadMenu;
                reloadMenuBordereau.Type = ReloadMenuOption.MenuType.DYNAMIC;
                BordereauInitData bordereauInitData = new BordereauInitData();

                AutoMapper.Mapper.CreateMap<V_HIS_TREATMENT_FEE, V_HIS_TREATMENT>();
                bordereauInitData.Treatment = AutoMapper.Mapper.Map<V_HIS_TREATMENT>(this.currentTreatment);
                AutoMapper.Mapper.CreateMap<V_HIS_SERE_SERV_5, HIS_SERE_SERV>();
                bordereauInitData.SereServs = AutoMapper.Mapper.Map<List<HIS_SERE_SERV>>(this.ListSereServTranfer);
                bordereauInitData.PatientTypeAlter = resultPatientType;
                HIS.Desktop.Plugins.Library.PrintBordereau.PrintBordereauProcessor processor = new PrintBordereauProcessor(this.currentModule != null ? this.currentModule.RoomId : 0, this.currentModule != null ? this.currentModule.RoomTypeId : 0, currentTreatment.ID, currentTreatment.PATIENT_ID, bordereauInitData, reloadMenuBordereau);
                processor.InitMenuPrint();

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitMenuToButtonPrint()
        {
            try
            {
                var checkMps000106 = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == HIS.Desktop.Print.PrintTypeCodeStore.PRINT_TYPE_CODE__HoaDonThanhToanChiTietDichVu_Mps000106);
                var checkMps000361 = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == "Mps000361");
                var checkMps000111 = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == HIS.Desktop.Print.PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuThuThanhToan_MPS000111);
                var checkMps000113 = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == HIS.Desktop.Print.PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuThuHoanUng_MPS000113);


                if (checkMps000106 != null && checkMps000361 != null && checkMps000111 != null && checkMps000113 != null)
                {
                    if (checkMps000106.IS_NO_GROUP != 1 && checkMps000361.IS_NO_GROUP != 1 && checkMps000111.IS_NO_GROUP != 1 && checkMps000113.IS_NO_GROUP != 1)
                    {
                        layoutControlItempanelMenuPrintBill.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        return;
                    }
                    else
                    {
                        layoutControlItempanelMenuPrintBill.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        HIS.UC.MenuPrint.MenuPrintProcessor menuPrintProcessor = new HIS.UC.MenuPrint.MenuPrintProcessor();
                        List<HIS.UC.MenuPrint.ADO.MenuPrintADO> menuPrintADOs = new List<HIS.UC.MenuPrint.ADO.MenuPrintADO>();

                        if (checkMps000106.IS_NO_GROUP == 1)
                        {
                            HIS.UC.MenuPrint.ADO.MenuPrintADO menuPrintADO__Mps000106 = new HIS.UC.MenuPrint.ADO.MenuPrintADO();
                            menuPrintADO__Mps000106.EventHandler = new EventHandler(onClickPhieuThuThanhToanChiTietDichVu);
                            menuPrintADO__Mps000106.PrintTypeCode = HIS.Desktop.Print.PrintTypeCodeStore.PRINT_TYPE_CODE__HoaDonThanhToanChiTietDichVu_Mps000106;
                            menuPrintADO__Mps000106.Tag = HIS.Desktop.Print.PrintTypeCodeStore.PRINT_TYPE_CODE__HoaDonThanhToanChiTietDichVu_Mps000106;
                            menuPrintADO__Mps000106.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__MPS000106_CAPTION", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                            menuPrintADO__Mps000106.Tooltip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__MPS000106", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                            menuPrintADOs.Add(menuPrintADO__Mps000106);
                        }
                        if (checkMps000361.IS_NO_GROUP == 1)
                        {
                            HIS.UC.MenuPrint.ADO.MenuPrintADO menuPrintADO__Mps000361 = new HIS.UC.MenuPrint.ADO.MenuPrintADO();
                            menuPrintADO__Mps000361.EventHandler = new EventHandler(onClickInThanhToanHoanUng);
                            menuPrintADO__Mps000361.PrintTypeCode = "Mps000361";
                            menuPrintADO__Mps000361.Tag = "Mps000361";
                            menuPrintADO__Mps000361.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__MPS000361_CAPTION", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                            menuPrintADO__Mps000361.Tooltip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__MPS000361", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());

                            menuPrintADOs.Add(menuPrintADO__Mps000361);
                        }

                        if (checkMps000111.IS_NO_GROUP == 1)
                        {
                            HIS.UC.MenuPrint.ADO.MenuPrintADO menuPrintADO__Mps000111 = new HIS.UC.MenuPrint.ADO.MenuPrintADO();
                            menuPrintADO__Mps000111.EventHandler = new EventHandler(onClickPhieuThuThanhToan);
                            menuPrintADO__Mps000111.PrintTypeCode = HIS.Desktop.Print.PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuThuThanhToan_MPS000111;
                            menuPrintADO__Mps000111.Tag = HIS.Desktop.Print.PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuThuThanhToan_MPS000111;
                            menuPrintADO__Mps000111.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__MPS000111_CAPTION", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                            menuPrintADO__Mps000111.Tooltip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__MPS000111", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());

                            menuPrintADOs.Add(menuPrintADO__Mps000111);
                        }

                        if (checkMps000113.IS_NO_GROUP == 1)
                        {
                            HIS.UC.MenuPrint.ADO.MenuPrintADO menuPrintADO__Mps000113 = new HIS.UC.MenuPrint.ADO.MenuPrintADO();
                            menuPrintADO__Mps000113.EventHandler = new EventHandler(onClickPhieuThuHoanUng);
                            menuPrintADO__Mps000113.PrintTypeCode = HIS.Desktop.Print.PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuThuHoanUng_MPS000113;
                            menuPrintADO__Mps000113.Tag = HIS.Desktop.Print.PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuThuHoanUng_MPS000113;
                            menuPrintADO__Mps000113.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__MPS000113_CAPTION", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                            menuPrintADO__Mps000113.Tooltip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__MPS000113", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());

                            menuPrintADOs.Add(menuPrintADO__Mps000113);
                        }

                        HIS.UC.MenuPrint.ADO.MenuPrintInitADO menuPrintInitADO = new HIS.UC.MenuPrint.ADO.MenuPrintInitADO(menuPrintADOs, BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>());
                        menuPrintInitADO.IsUsingShortCut = false;
                        menuPrintInitADO.ControlContainer = panelMenuPrintBill;
                        var uc = menuPrintProcessor.Run(menuPrintInitADO);
                        if (uc == null)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("menuPrintProcessor run fail. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => menuPrintInitADO), menuPrintInitADO));
                        }
                        //lciPrintAssignService.MinSize = new System.Drawing.Size(pnlPrintAssignService.Width, lciPrintAssignService.Height);
                        //lciPrintAssignService.MaxSize = new System.Drawing.Size(pnlPrintAssignService.Width, lciPrintAssignService.Height);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ReloadMenu(object data)
        {
            if (data != null)
            {
                if (data is List<MenuPrintADO>)
                {

                    MenuPrintProcessor menuPrintProcessor = new MenuPrintProcessor();
                    HIS.UC.MenuPrint.ADO.MenuPrintInitADO menuPrintInitADO = new HIS.UC.MenuPrint.ADO.MenuPrintInitADO(data as List<MenuPrintADO>, HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<SAR_PRINT_TYPE>());
                    menuPrintInitADO.ControlContainer = panelPrintBordereau;
                    var uc = menuPrintProcessor.Run(menuPrintInitADO);
                    if (uc == null)
                    {
                        LogSystem.Warn("Khoi tao uc print that bai trong chuc nang bang ke. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => uc), uc));
                    }
                }
            }
        }
    }
}
