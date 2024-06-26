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
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignServiceTestMulti.ADO;
using HIS.Desktop.Print;
using Inventec.Common.LocalStorage.SdaConfig;
using Inventec.Common.RichEditor;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.LibraryMessage;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignServiceTestMulti.AssignService
{
    public partial class frmAssignService : HIS.Desktop.Utility.FormBase
    {
        Library.PrintServiceReq.PrintServiceReqProcessor PrintServiceReqProcessor;
        private void InitMenuToButtonPrint()
        {
            try
            {
                HIS.UC.MenuPrint.MenuPrintProcessor menuPrintProcessor = new HIS.UC.MenuPrint.MenuPrintProcessor();
                List<HIS.UC.MenuPrint.ADO.MenuPrintADO> menuPrintADOs = new List<HIS.UC.MenuPrint.ADO.MenuPrintADO>();

                HIS.UC.MenuPrint.ADO.MenuPrintADO menuPrintADO__TongHop = new HIS.UC.MenuPrint.ADO.MenuPrintADO();
                menuPrintADO__TongHop.EventHandler = new EventHandler(OnClickInChiDinhDichVu);
                menuPrintADO__TongHop.PrintTypeCode = PrintTypeCodeStore.PRINT_TYPE_CODE__IN__PHIEU_YEU_CAU_CHI_DINH_TONG_HOP__MPS000037;
                menuPrintADO__TongHop.Tag = PrintTypeCodeStore.PRINT_TYPE_CODE__IN__PHIEU_YEU_CAU_CHI_DINH_TONG_HOP__MPS000037;
                menuPrintADOs.Add(menuPrintADO__TongHop);

                HIS.UC.MenuPrint.ADO.MenuPrintInitADO menuPrintInitADO = new HIS.UC.MenuPrint.ADO.MenuPrintInitADO(menuPrintADOs, BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>());
                menuPrintInitADO.ControlContainer = pnlPrintAssignService;
                var uc = menuPrintProcessor.Run(menuPrintInitADO);
                if (uc == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("menuPrintProcessor run fail. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => menuPrintInitADO), menuPrintInitADO));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnClickInChiDinhDichVu(object sender, EventArgs e)
        {
            try
            {
                string printTypeCode = "";
                if (sender is DevExpress.Utils.Menu.DXMenuItem)
                {
                    var bbtnItem = sender as DevExpress.Utils.Menu.DXMenuItem;
                    printTypeCode = (bbtnItem.Tag ?? "").ToString();
                }
                else if (sender is DevExpress.XtraEditors.SimpleButton)
                {
                    var bbtnItem = sender as DevExpress.XtraEditors.SimpleButton;
                    printTypeCode = (bbtnItem.Tag ?? "").ToString();
                }

                DelegateRunPrinter(printTypeCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool DelegateRunPrinter(string printTypeCode)
        {
            bool result = false;
            try
            {
                List<V_HIS_BED_LOG> bedLogs = new List<V_HIS_BED_LOG>();
                CommonParam param = new CommonParam();
                if (serviceReqComboResultSDO != null && serviceReqComboResultSDO.ServiceReqs != null && serviceReqComboResultSDO.ServiceReqs.Count > 0 && currentHisTreatment != null)
                {
                    MOS.Filter.HisBedLogViewFilter bedLogViewFilter = new MOS.Filter.HisBedLogViewFilter();
                    bedLogViewFilter.DEPARTMENT_IDs = serviceReqComboResultSDO.ServiceReqs.Select(o => o.REQUEST_DEPARTMENT_ID).Distinct().ToList();
                    bedLogViewFilter.TREATMENT_ID = currentHisTreatment.ID;
                    bedLogs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumer.ApiConsumers.MosConsumer, bedLogViewFilter, param);
                }
                PrintServiceReqProcessor = new Library.PrintServiceReq.PrintServiceReqProcessor(serviceReqComboResultSDO, currentHisTreatment, bedLogs);
                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__SERVICE_REQ_REGISTER:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__YEU_CAU_KHAM_CHUYEN_KHOA__MPS000071:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__IN__PHIEU_YEU_CAU_CHI_DINH_TONG_HOP__MPS000037:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_XET_NGHIEM__MPS000026:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_CHUAN_DOAN_HINH_ANH__MPS000028:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_THAM_DO_CHUC_NANG__MPS000038:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_NOI_SOI__MPS000029:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_SIEU_AM__MPS000030:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_THU_THUAT__MPS000031:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_PHAU_THUAT__MPS000036:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_DICH_VU_KHAC__MPS000040:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_GIUONG__MPS000042:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_PHUC_HOI_CHUC_NANG__MPS000053:
                        InPhieuYeuCauDichVu(printTypeCode);
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

        private void InPhieuYeuCauDichVu()
        {
            try
            {
                if (serviceReqComboResultSDO != null)
                {
                    List<V_HIS_BED_LOG> bedLogs = new List<V_HIS_BED_LOG>();
                    CommonParam param = new CommonParam();
                    if (serviceReqComboResultSDO != null && serviceReqComboResultSDO.ServiceReqs != null && serviceReqComboResultSDO.ServiceReqs.Count > 0 && currentHisTreatment != null)
                    {
                        MOS.Filter.HisBedLogViewFilter bedLogViewFilter = new MOS.Filter.HisBedLogViewFilter();
                        bedLogViewFilter.DEPARTMENT_IDs = serviceReqComboResultSDO.ServiceReqs.Select(o => o.REQUEST_DEPARTMENT_ID).Distinct().ToList();
                        bedLogViewFilter.TREATMENT_ID = currentHisTreatment.ID;
                        bedLogs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumer.ApiConsumers.MosConsumer, bedLogViewFilter, param);
                    }
                    var PrintServiceReqProcessor = new Library.PrintServiceReq.PrintServiceReqProcessor(serviceReqComboResultSDO, currentHisTreatment, bedLogs);
                    PrintServiceReqProcessor.SaveNPrint();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void InPhieuYeuCauDichVu(string printTypeCode)
        {
            try
            {
                if (PrintServiceReqProcessor != null)
                {
                    PrintServiceReqProcessor.Print(printTypeCode, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }
    }
}
