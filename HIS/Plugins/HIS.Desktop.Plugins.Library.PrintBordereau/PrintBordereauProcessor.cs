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
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.PrintBordereau.ADO;
using HIS.Desktop.Plugins.Library.PrintBordereau.Base;
using HIS.Desktop.Plugins.Library.PrintBordereau.Config;
using HIS.Desktop.Plugins.Library.PrintBordereau.Mps000120;
using HIS.Desktop.Plugins.Library.PrintBordereau.Mps000122;
using HIS.Desktop.Plugins.Library.PrintBordereau.Mps000124;
using HIS.Desktop.Plugins.Library.PrintBordereau.Mps000128;
using HIS.Desktop.Plugins.Library.PrintBordereau.Mps000158;
using HIS.Desktop.Plugins.Library.PrintBordereau.Mps000160;
using HIS.Desktop.Plugins.Library.PrintBordereau.Mps000162;
using HIS.Desktop.Plugins.Library.PrintBordereau.Mps000194;
using HIS.Desktop.Plugins.Library.PrintBordereau.Mps000196;
using HIS.Desktop.Plugins.Library.PrintBordereau.Mps000249;
using HIS.Desktop.Plugins.Library.PrintBordereau.Mps000251;
using HIS.Desktop.Plugins.Library.PrintBordereau.Mps000260;
using HIS.Desktop.Plugins.Library.PrintBordereau.Mps000261;
using HIS.Desktop.Plugins.Library.PrintBordereau.Mps000265;
using HIS.Desktop.Plugins.Library.PrintBordereau.Mps000279;
using HIS.Desktop.Plugins.Library.PrintBordereau.Mps000281;
using HIS.Desktop.Plugins.Library.PrintBordereau.Mps000285;
using HIS.Desktop.Plugins.Library.PrintBordereau.Mps000295;
using HIS.Desktop.Plugins.Library.PrintBordereau.MpsBehavior.Mps000302;
using Inventec.Common.SignLibrary.DTO;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintBordereau
{
    public partial class PrintBordereauProcessor : MpsDataBase
    {
        private long roomId { get; set; }
        private long roomTypeId { get; set; }
        private long patientId { get; set; }
        private long treatmentId { get; set; }
        private BordereauInitData BordereauInitData { get; set; }
        private ReloadMenuOption ReLoadMenuOption { get; set; }
        private Inventec.Common.FlexCelPrint.DelegateReturnEventPrint ReturnEventPrint { get; set; }
        private Dictionary<string, string> dicMpsReplace { get; set; }
        private Action<DocumentSignedUpdateIGSysResultDTO> dlg { get; set; }
        public bool IsActionButtonPrintBill { get; set; }
        public static Action<DocumentSignedUpdateIGSysResultDTO> DlgSendResultSigned { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_treatmentId"></param>
        /// <param name="_patientId"></param>
        /// <param name="_args">List V_HIS_TREATMENT_FEE, List V_HIS_DEPARTMENT_TRAN, V_HIS_PATIENT</param>
        public PrintBordereauProcessor(long _treatmentId, long _patientId, BordereauInitData _bordereauInitData, ReloadMenuOption _reLoadMenuOption)
        {
            this.patientId = _patientId;
            this.treatmentId = _treatmentId;
            this.BordereauInitData = _bordereauInitData;
            this.ReLoadMenuOption = _reLoadMenuOption;
        }

        public PrintBordereauProcessor(long _roomId, long _roomTypeId, long _treatmentId, long _patientId, BordereauInitData _bordereauInitData, ReloadMenuOption _reLoadMenuOption)
        {
            this.patientId = _patientId;
            this.treatmentId = _treatmentId;
            this.BordereauInitData = _bordereauInitData;
            this.ReLoadMenuOption = _reLoadMenuOption;
            this.roomId = _roomId;
            this.roomTypeId = _roomTypeId;
        }
        public PrintBordereauProcessor(long _roomId, long _roomTypeId, long _treatmentId, long _patientId, BordereauInitData _bordereauInitData, ReloadMenuOption _reLoadMenuOption, Action<DocumentSignedUpdateIGSysResultDTO> DlgSendResultSigned)
        {
            this.dlg = DlgSendResultSigned;
            this.patientId = _patientId;
            this.treatmentId = _treatmentId;
            this.BordereauInitData = _bordereauInitData;
            this.ReLoadMenuOption = _reLoadMenuOption;
            this.roomId = _roomId;
            this.roomTypeId = _roomTypeId;
        }
        public PrintBordereauProcessor(long _roomId, long _roomTypeId, long _treatmentId, long _patientId, BordereauInitData _bordereauInitData, ReloadMenuOption _reLoadMenuOption, Inventec.Common.FlexCelPrint.DelegateReturnEventPrint _returnEventPrintNow)
        {
            this.patientId = _patientId;
            this.treatmentId = _treatmentId;
            this.BordereauInitData = _bordereauInitData;
            this.ReLoadMenuOption = _reLoadMenuOption;
            this.roomId = _roomId;
            this.roomTypeId = _roomTypeId;
            this.ReturnEventPrint = _returnEventPrintNow;
        }

        /// <summary>
        /// In theo cấu hình : HIS.Desktop.Plugins.Library.Bordereau.MpsCodeDefault
        /// </summary>
        /// <param name="bordereauType">BordereauPrint.Type</param>
        public void Print(PrintOption.Value? printOption = null)
        {
            try
            {
                long bordereauType = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.BORDEREAU_TYPE_CONFIG));
                string mpsReplace = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.PRINT_DEFAULT_MPS_REPLACE);
                dicMpsReplace = this.GetMpsReplaceFromCFG(mpsReplace);

                this.InitData(BordereauInitData); //Khoi tao load co san
                this.InitMenu(printOption);

                if (printOption == PrintOption.Value.INIT_MENU)
                    return;

                //Print now
                this.LoadData(); // khong co thi load lai

                if (printOption.HasValue)
                    GlobalDataStore.CURRENT_PRINT_OPTION = printOption.Value;

                switch (bordereauType)
                {
                    case BordereauPrint.BORDER_TYPE___BASE:
                        bool isBHYT = false;
                        bool isVienPhi = false;
                        CheckBordereauType(ref isBHYT, ref isVienPhi);

                        if (this.PatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                        {
                            if (isBHYT && isVienPhi
                                && LoadPrintDefaultWithReplaceMpsV2(dicMpsReplace,
                                PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_BHYT,
                                PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_VIENPHI))
                            {
                                this.LoadPrintDefaultWithReplaceMps(dicMpsReplace, PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_BHYT);
                            }
                            else
                            {
                                if (isBHYT)
                                {
                                    this.LoadPrintDefaultWithReplaceMps(dicMpsReplace, PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_BHYT);
                                }
                                if (isVienPhi)
                                {
                                    this.LoadPrintDefaultWithReplaceMps(dicMpsReplace, PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_VIENPHI);
                                }
                            }
                        }
                        else if (this.PatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU
                            || this.PatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                        {
                            if (isBHYT && isVienPhi
                                && LoadPrintDefaultWithReplaceMpsV2(dicMpsReplace,
                                PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_BHYT,
                                PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_VIENPHI))
                            {
                                this.LoadPrintDefaultWithReplaceMps(dicMpsReplace, PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_BHYT);
                            }
                            else
                            {
                                if (isBHYT)
                                {
                                    this.LoadPrintDefaultWithReplaceMps(dicMpsReplace, PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_BHYT);
                                }
                                if (isVienPhi)
                                {
                                    this.LoadPrintDefaultWithReplaceMps(dicMpsReplace, PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_VIENPHI);
                                }
                            }
                        }
                        break;
                }

                //Sau khi in ngay, chuyen ve cau hinh khoi tao menu
                GlobalDataStore.CURRENT_PRINT_OPTION = PrintOption.Value.INIT_MENU;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Print(PrintOption.Value? printOption = null, PrintOption.PayType? payOption = null)
        {
            this.PayOption = payOption;
            this.Print(printOption);
        }

        public void Print(string printTypeCode, PrintOption.Value? printOption = null, PrintOption.PayType? payOption = null)
        {
            try
            {
                this.PayOption = payOption;
                string mpsReplace = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.PRINT_DEFAULT_MPS_REPLACE);
                dicMpsReplace = this.GetMpsReplaceFromCFG(mpsReplace);

                this.InitData(BordereauInitData); //Khoi tao load co san
                this.InitMenu(printOption);

                if (printOption.HasValue)
                    GlobalDataStore.CURRENT_PRINT_OPTION = printOption.Value;
                else
                    GlobalDataStore.CURRENT_PRINT_OPTION = PrintOption.Value.INIT_MENU;

                //Print now
                this.LoadData(); // khong co thi load lai
                if (dlg != null)
                    DlgSendResultSigned = dlg;
                this.LoadPrintDefaultWithReplaceMps(dicMpsReplace, printTypeCode);

                //Sau khi in ngay, chuyen ve cau hinh khoi tao menu
                GlobalDataStore.CURRENT_PRINT_OPTION = PrintOption.Value.INIT_MENU;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Khởi tạo menu bảng kê
        /// </summary>
        public void InitMenuPrint()
        {
            try
            {
                this.InitData(BordereauInitData);
                this.InitMenu(PrintOption.Value.INIT_MENU);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Khởi tạo menu bảng kê
        /// </summary>
        public void InitMenuPrint(PrintOption.PayType? payOption = null)
        {
            try
            {
                this.PayOption = payOption;
                this.InitData(BordereauInitData);
                this.InitMenu(PrintOption.Value.INIT_MENU);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RunPrint(string mpsCode)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);
                richEditorMain.RunPrintTemplate(mpsCode, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool DelegateRunPrinter(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                ILoad loadMps = null;

                if (!this.PayOption.HasValue && printCode == PrintTypeCodeWorker.PRINT_TYPE_CODE___TONG_HOP_6556__THEO_KHOA_PHONG_THANH_TOAN)
                {
                    this.PayOption = PrintOption.PayType.NOT_BILL;
                }
                else if (printCode == PrintTypeCodeWorker.PRINT_TYPE_CODE___YEU_CAU_THANH_TOAN)// in yêu cầu thanh toán luôn lọc ra dịch vụ chưa có tạm ứng và thanh toán
                {
                    this.PayOption = PrintOption.PayType.NOT_BILL_OR_DEPOSIT;
                }
                else if (!this.PayOption.HasValue)
                {
                    this.PayOption = PrintOption.PayType.ALL;
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => PayOption), PayOption));

                switch (this.PayOption)
                {
                    case PrintOption.PayType.ALL:
                        break;
                    case PrintOption.PayType.DEPOSIT:
                        List<HIS_SERE_SERV_DEPOSIT> ssDeposit = new List<HIS_SERE_SERV_DEPOSIT>();
                        if (this.SereServDeposits != null && this.SereServDeposits.Count > 0)
                        {
                            ssDeposit.AddRange(this.SereServDeposits);
                        }

                        //tạm ứng ko có hoàn ứng
                        if (this.SeseDepoRepays != null && this.SeseDepoRepays.Count > 0)
                        {
                            ssDeposit = ssDeposit.Where(o => !this.SeseDepoRepays.Exists(e => e.SERE_SERV_DEPOSIT_ID == o.ID && e.IS_CANCEL != 1)).ToList();
                        }

                        if (ssDeposit != null && ssDeposit.Count > 0)
                        {
                            SereServs = SereServs.Where(o => ssDeposit.Exists(e => e.SERE_SERV_ID == o.ID && e.IS_CANCEL != 1)).ToList();
                        }
                        else
                        {
                            SereServs = new List<HIS_SERE_SERV>();
                        }
                        break;
                    case PrintOption.PayType.NOT_DEPOSIT:
                        List<HIS_SERE_SERV_DEPOSIT> ssNotDeposit = new List<HIS_SERE_SERV_DEPOSIT>();
                        if (this.SereServDeposits != null && this.SereServDeposits.Count > 0)
                        {
                            ssNotDeposit.AddRange(this.SereServDeposits);
                        }

                        //tạm ứng ko có hoàn ứng
                        if (this.SeseDepoRepays != null && this.SeseDepoRepays.Count > 0)
                        {
                            ssNotDeposit = ssNotDeposit.Where(o => !this.SeseDepoRepays.Exists(e => e.SERE_SERV_DEPOSIT_ID == o.ID && e.IS_CANCEL != 1)).ToList();
                        }

                        if (ssNotDeposit != null && ssNotDeposit.Count > 0)
                        {
                            SereServs = SereServs.Where(o => !ssNotDeposit.Exists(e => e.SERE_SERV_ID == o.ID && e.IS_CANCEL != 1)).ToList();
                        }
                        break;
                    case PrintOption.PayType.BILL:
                        if (this.SereServBills != null && this.SereServBills.Count > 0)
                        {
                            SereServs = SereServs.Where(o => this.SereServBills.Exists(e => e.SERE_SERV_ID == o.ID && e.IS_CANCEL != 1)).ToList();
                        }
                        else
                        {
                            SereServs = new List<HIS_SERE_SERV>();
                        }
                        break;
                    case PrintOption.PayType.NOT_BILL:
                        if (this.SereServBills != null && this.SereServBills.Count > 0)
                        {
                            SereServs = SereServs.Where(o => !this.SereServBills.Exists(e => e.SERE_SERV_ID == o.ID && e.IS_CANCEL != 1)).ToList();
                        }
                        break;
                    case PrintOption.PayType.NOT_BILL_OR_DEPOSIT:
                        List<HIS_SERE_SERV_DEPOSIT> ssForNotDeposit = new List<HIS_SERE_SERV_DEPOSIT>();
                        if (this.SereServDeposits != null && this.SereServDeposits.Count > 0)
                        {
                            ssForNotDeposit.AddRange(this.SereServDeposits);
                        }

                        //tạm ứng ko có hoàn ứng
                        if (this.SeseDepoRepays != null && this.SeseDepoRepays.Count > 0)
                        {
                            ssForNotDeposit = ssForNotDeposit.Where(o => !this.SeseDepoRepays.Exists(e => e.SERE_SERV_DEPOSIT_ID == o.ID && e.IS_CANCEL != 1)).ToList();
                        }

                        if (ssForNotDeposit != null && ssForNotDeposit.Count > 0)
                        {
                            SereServs = SereServs.Where(o => !ssForNotDeposit.Exists(e => e.SERE_SERV_ID == o.ID && e.IS_CANCEL != 1)).ToList();
                        }

                        if (this.SereServBills != null && this.SereServBills.Count > 0)
                        {
                            SereServs = SereServs.Where(o => !this.SereServBills.Exists(e => e.SERE_SERV_ID == o.ID && e.IS_CANCEL != 1)).ToList();
                        }
                        break;
                    default:
                        break;
                }

                if (GlobalVariables.dicPrinter.ContainsKey(printCode))
                {
                    GlobalDataStore.PrinterName = GlobalVariables.dicPrinter[printCode];
                }
                else
                {
                    GlobalDataStore.PrinterName = "";
                }

                switch (printCode)
                {
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_BHYT:
                        loadMps = new Mps000120Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, Treatment, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, RoomName, UserNameReturnResult, "");
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_BHYT:
                        SAR_PRINT_TYPE pt1 = BackendDataWorker.Get<SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_BHYT);
                        printCode = PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_BHYT;
                        loadMps = new Mps000120Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, Treatment, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, RoomName, UserNameReturnResult, pt1 != null ? pt1.PRINT_TYPE_NAME : "");
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_VIENPHI:
                        loadMps = new Mps000122Behavior(this.roomId, SereServs, DepartmentTrans, TreatmentFees, Treatment, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, UserNameReturnResult, "");
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_VIENPHI:
                        printCode = PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_VIENPHI;
                        SAR_PRINT_TYPE pt2 = BackendDataWorker.Get<SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_VIENPHI);
                        loadMps = new Mps000122Behavior(this.roomId, SereServs, DepartmentTrans, TreatmentFees, Treatment, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, UserNameReturnResult, pt2 != null ? pt2.PRINT_TYPE_NAME : "");
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_BHYT__HAO_PHI:
                        loadMps = new Mps000158Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, Treatment, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, UserNameReturnResult, "");
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_BHYT__HAO_PHI:
                        printCode = PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_BHYT__HAO_PHI;
                        SAR_PRINT_TYPE pt3 = BackendDataWorker.Get<SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_BHYT__HAO_PHI);
                        loadMps = new Mps000158Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, Treatment, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, UserNameReturnResult, pt3 != null ? pt3.PRINT_TYPE_NAME : "");
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_VIENPHI__HAO_PHI:
                        loadMps = new Mps000160Behavior(this.roomId, SereServs, DepartmentTrans, TreatmentFees, Treatment, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, UserNameReturnResult, "");
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_VIENPHI__HAO_PHI:
                        printCode = PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_VIENPHI__HAO_PHI;
                        SAR_PRINT_TYPE pt4 = BackendDataWorker.Get<SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_VIENPHI__HAO_PHI);
                        loadMps = new Mps000160Behavior(this.roomId, SereServs, DepartmentTrans, TreatmentFees, Treatment, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, UserNameReturnResult, pt4 != null ? pt4.PRINT_TYPE_NAME : "");
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU__HAO_PHI:
                        loadMps = new Mps000162Behavior(this.roomId, SereServs, DepartmentTrans, TreatmentFees, Treatment, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, UserNameReturnResult, "");
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU__HAO_PHI:
                        printCode = PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU__HAO_PHI;
                        SAR_PRINT_TYPE pt5 = BackendDataWorker.Get<SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU__HAO_PHI);
                        loadMps = new Mps000162Behavior(this.roomId, SereServs, DepartmentTrans, TreatmentFees, Treatment, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, UserNameReturnResult, pt5 != null ? pt5.PRINT_TYPE_NAME : "");
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___TONG_HOP:
                        loadMps = new Mps000124Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, SereServDeposits, SeseDepoRepays, Treatment, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, UserNameReturnResult);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___TONG_HOP__NOI_TRU__BHYT:
                        printCode = PrintTypeCodeWorker.PRINT_TYPE_CODE___TONG_HOP__NGOAI_TRU__BHYT;
                        SAR_PRINT_TYPE pt6 = BackendDataWorker.Get<SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == PrintTypeCodeWorker.PRINT_TYPE_CODE___TONG_HOP__NOI_TRU__BHYT);
                        loadMps = new Mps000125Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, Treatment, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, UserNameReturnResult, pt6 != null ? pt6.PRINT_TYPE_NAME : "");
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___TONG_HOP__NGOAI_TRU__BHYT:
                        loadMps = new Mps000125Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, Treatment, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, UserNameReturnResult, "");
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___THEO_KHOA:
                        loadMps = new Mps000127Behavior(this.roomId, Patient, SereServs, DepartmentTrans, TreatmentFees, Treatment, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, UserNameReturnResult, CurrentDepartmentId);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___THEO_KHOA___BHYT:
                        loadMps = new Mps000193Behavior(this.roomId, Patient, SereServs, DepartmentTrans, TreatmentFees, Treatment, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, UserNameReturnResult, CurrentDepartmentId);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___TRONG_GOI_KY_THUAT_CAO:
                        loadMps = new Mps000128Behavior(this.roomId, Patient, SereServs, DepartmentTrans, TreatmentFees, Treatment, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, UserNameReturnResult, CurrentDepartmentId);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU__BHYT__TPTB:
                        loadMps = new Mps000194Behavior(this.roomId, this.PatientTypeAlter, Patient, SereServs, DepartmentTrans, TreatmentFees, Treatment, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, UserNameReturnResult, "");
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU__VIEN_PHI__TPTB:
                        loadMps = new Mps000196Behavior(this.roomId, Patient, SereServs, DepartmentTrans, TreatmentFees, Treatment, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, UserNameReturnResult, "");
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU__BHYT__TPTB:
                        printCode = PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU__BHYT__TPTB;
                        SAR_PRINT_TYPE pt7 = BackendDataWorker.Get<SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU__BHYT__TPTB);
                        loadMps = new Mps000194Behavior(this.roomId, this.PatientTypeAlter, Patient, SereServs, DepartmentTrans, TreatmentFees, Treatment, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, UserNameReturnResult, pt7 != null ? pt7.PRINT_TYPE_NAME : "");
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU__VIEN_PHI__TPTB:
                        printCode = PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU__VIEN_PHI__TPTB;
                        SAR_PRINT_TYPE pt8 = BackendDataWorker.Get<SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU__VIEN_PHI__TPTB);
                        loadMps = new Mps000196Behavior(this.roomId, Patient, SereServs, DepartmentTrans, TreatmentFees, Treatment, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, UserNameReturnResult, pt8 != null ? pt8.PRINT_TYPE_NAME : "");
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___IN_GIAY_PHU_THU:
                        loadMps = new Mps000224Behavior(this.roomId, SereServs, DepartmentTrans, TreatmentFees, Treatment, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, UserNameReturnResult);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_BHYT__CHUA_THANH_TOAN:
                        printCode = PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_BHYT;
                        loadMps = new Mps000249Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, Treatment, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, UserNameReturnResult, "", this.SereServBills);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_BHYT__CHUA_THANH_TOAN:
                        printCode = PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_BHYT;
                        SAR_PRINT_TYPE pt9 = BackendDataWorker.Get<SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_BHYT__CHUA_THANH_TOAN);
                        loadMps = new Mps000249Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, Treatment, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, UserNameReturnResult, pt9 != null ? pt9.PRINT_TYPE_NAME : "", this.SereServBills);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_VIEN_PHI__CHUA_THANH_TOAN:
                        printCode = PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_VIENPHI;
                        loadMps = new Mps000251Behavior(this.roomId, SereServs, DepartmentTrans, TreatmentFees, Treatment, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, UserNameReturnResult, "", this.SereServBills);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_VIEN_PHI__CHUA_THANH_TOAN:
                        printCode = PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_VIENPHI;
                        SAR_PRINT_TYPE pt10 = BackendDataWorker.Get<SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_VIEN_PHI__CHUA_THANH_TOAN);
                        loadMps = new Mps000251Behavior(this.roomId, SereServs, DepartmentTrans, TreatmentFees, Treatment, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, UserNameReturnResult, pt10 != null ? pt10.PRINT_TYPE_NAME : "", this.SereServBills);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___TONG_HOP_NGOAI_TRU__HAO_PHI:
                        loadMps = new Mps000260Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, Treatment, Rooms, Services, HeinServiceTypes, SereServDeposits, SeseDepoRepays, TotalDayTreatment, StatusTreatmentOut, DepartmentName, UserNameReturnResult, "", transReq, lstConfig);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___TONG_HOP_NOI_TRU__HAO_PHI:
                        printCode = PrintTypeCodeWorker.PRINT_TYPE_CODE___TONG_HOP_NGOAI_TRU__HAO_PHI;
                        SAR_PRINT_TYPE pt11 = BackendDataWorker.Get<SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == PrintTypeCodeWorker.PRINT_TYPE_CODE___TONG_HOP_NOI_TRU__HAO_PHI);
                        loadMps = new Mps000260Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, Treatment, Rooms, Services, HeinServiceTypes, SereServDeposits, SeseDepoRepays, TotalDayTreatment, StatusTreatmentOut, DepartmentName, UserNameReturnResult, pt11 != null ? pt11.PRINT_TYPE_NAME : "", transReq, lstConfig);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_VIEN_PHI_100__CHUA_THANH_TOAN:
                        loadMps = new Mps000265Behavior(this.roomId, SereServs, DepartmentTrans, TreatmentFees, Treatment, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, UserNameReturnResult, "", this.SereServBills);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_VIEN_PHI_100__CHUA_THANH_TOAN:
                        printCode = PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_VIEN_PHI_100__CHUA_THANH_TOAN;
                        SAR_PRINT_TYPE pt12 = BackendDataWorker.Get<SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_VIEN_PHI_100__CHUA_THANH_TOAN);
                        loadMps = new Mps000265Behavior(this.roomId, SereServs, DepartmentTrans, TreatmentFees, Treatment, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, UserNameReturnResult, pt12 != null ? pt12.PRINT_TYPE_NAME : "", this.SereServBills);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_BHYT__6556_QĐ_BYT:
                        loadMps = new Mps000279Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, Treatment, this.Patient, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, RoomName, UserNameReturnResult, "", this.PayOption, transReq, lstConfig);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_BHYT__6556_QĐ_BYT:
                        printCode = PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_BHYT__6556_QĐ_BYT;
                        SAR_PRINT_TYPE pt = BackendDataWorker.Get<SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_BHYT__6556_QĐ_BYT);
                        loadMps = new Mps000279Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, Treatment, this.Patient, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, RoomName, UserNameReturnResult, pt != null ? pt.PRINT_TYPE_NAME : "", this.PayOption, transReq, lstConfig);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_VIEN_PHI__6556_QĐ_BYT:
                        loadMps = new Mps000281Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, Treatment, this.Patient, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, RoomName, UserNameReturnResult, "", this.PayOption, transReq, lstConfig);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_VIEN_PHI__6556_QĐ_BYT:
                        printCode = PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_VIEN_PHI__6556_QĐ_BYT;
                        SAR_PRINT_TYPE pt13 = BackendDataWorker.Get<SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_VIEN_PHI__6556_QĐ_BYT);
                        loadMps = new Mps000281Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, Treatment, this.Patient, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, RoomName, UserNameReturnResult, pt13 != null ? pt13.PRINT_TYPE_NAME : "", this.PayOption, transReq, lstConfig);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_THUOC_VATTU_CHUONG_TRINH__6556_QĐ_BYT:
                        loadMps = new Mps000285Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, Treatment, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, RoomName, UserNameReturnResult, "");
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_THUOC_VATTU_CHUONG_TRINH__6556_QĐ_BYT:
                        printCode = PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_THUOC_VATTU_CHUONG_TRINH__6556_QĐ_BYT;
                        SAR_PRINT_TYPE pt14 = BackendDataWorker.Get<SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_THUOC_VATTU_CHUONG_TRINH__6556_QĐ_BYT);
                        loadMps = new Mps000285Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, Treatment, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, RoomName, UserNameReturnResult, pt14 != null ? pt14.PRINT_TYPE_NAME : "");
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___BANG_KE_VIEN_PHI_TONG_HOP:
                        loadMps = new Mps000295Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, Treatment, this.Patient, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, RoomName, UserNameReturnResult);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___BANG_KE_6556_TONG_HOP:
                        loadMps = new Mps000302Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, Treatment, this.Patient, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, RoomName, UserNameReturnResult, this.SereServBills, this.SereServDeposits, this.SeseDepoRepays, transReq, lstConfig);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_BHYT__6556_QĐ_BYT__THEO_KHOA:
                        LoadTransaction();
                        loadMps = new Mps000304.Mps000304Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, Treatment, this.Patient, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, RoomName, UserNameReturnResult, "", lstConfig, transReq, ListTransaction);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_BHYT__6556_QĐ_BYT__THEO_KHOA:
                        printCode = PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_BHYT__6556_QĐ_BYT__THEO_KHOA;
                        SAR_PRINT_TYPE pt15 = BackendDataWorker.Get<SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_BHYT__6556_QĐ_BYT__THEO_KHOA);
                        loadMps = new Mps000304.Mps000304Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, Treatment, this.Patient, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, RoomName, UserNameReturnResult, pt15 != null ? pt15.PRINT_TYPE_NAME : "", lstConfig, transReq, null);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_VIEN_PHI__6556_QĐ_BYT__THEO_KHOA:
                        LoadTransaction();
                        loadMps = new Mps000306.Mps000306Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, Treatment, this.Patient, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, RoomName, UserNameReturnResult, "", ListTransaction);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_VIEN_PHI__6556_QĐ_BYT__THEO_KHOA:
                        printCode = PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU_VIEN_PHI__6556_QĐ_BYT__THEO_KHOA;
                        SAR_PRINT_TYPE pt16 = BackendDataWorker.Get<SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_VIEN_PHI__6556_QĐ_BYT__THEO_KHOA);
                        loadMps = new Mps000306.Mps000306Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, Treatment, this.Patient, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, RoomName, UserNameReturnResult, pt16 != null ? pt16.PRINT_TYPE_NAME : "", null);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___TONG_HOP_GOI:
                        loadMps = new MpsBehavior.Mps000312.Mps000312Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, SereServDeposits, SeseDepoRepays, Treatment, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, UserNameReturnResult);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___BANG_KE_6556_TONG_HOP_GOI:
                        loadMps = new MpsBehavior.Mps000313.Mps000313Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, Treatment, this.Patient, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, RoomName, UserNameReturnResult);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___TONG_HOP_CCT:
                        loadMps = new MpsBehavior.Mps000314.Mps000314Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, Treatment, this.Patient, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, RoomName, UserNameReturnResult);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___TONG_HOP_6556__THEO_KHOA:
                        LoadTransactionView();
                        loadMps = new MpsBehavior.Mps000321.Mps000321Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, Treatment, this.Patient, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, RoomName, UserNameReturnResult, this.Transactions, this.PayOption,transReq,lstConfig);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU_BHYT__6556_QĐ_BYT_STENT_2:
                        loadMps = new MpsBehavior.Mps000348.Mps000348Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, Treatment, this.Patient, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, RoomName, UserNameReturnResult);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU__HAO_PHI_THEO_KHOA__6556_QĐ_BYT:
                        loadMps = new MpsBehavior.Mps000356.Mps000356Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, Treatment, this.Patient, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, RoomName, UserNameReturnResult, "");
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU__HAO_PHI_THEO_KHOA__6556_QĐ_BYT:
                        printCode = PrintTypeCodeWorker.PRINT_TYPE_CODE___NGOAI_TRU__HAO_PHI_THEO_KHOA__6556_QĐ_BYT;
                        SAR_PRINT_TYPE pt17 = BackendDataWorker.Get<SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == PrintTypeCodeWorker.PRINT_TYPE_CODE___NOI_TRU__HAO_PHI_THEO_KHOA__6556_QĐ_BYT);
                        loadMps = new MpsBehavior.Mps000356.Mps000356Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, Treatment, this.Patient, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, RoomName, UserNameReturnResult, pt17 != null ? pt17.PRINT_TYPE_NAME : "");
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___BANG_KE_DOI_TUONG_KHAC:
                        loadMps = new MpsBehavior.Mps000359.Mps000359Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, Treatment, this.Patient, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, RoomName, UserNameReturnResult, this.SereServBills, this.SereServDeposits, this.SeseDepoRepays, lstConfig, transReq);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___TONG_HOP_6556__THEO_KHOA_PHONG_THANH_TOAN:
                        loadMps = new MpsBehavior.Mps000441.Mps000441Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, Treatment, this.Patient, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, RoomName, UserNameReturnResult, this.SereServBills, this.PayOption);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___YEU_CAU_THANH_TOAN:
                        loadMps = new MpsBehavior.Mps000446.Mps000446Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, Treatment, this.Patient, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, RoomName, UserNameReturnResult, this.SereServBills, this.transReq2, this.lstConfig,this.IsActionButtonPrintBill);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___BANG_KE_6556_THEO_LOAI_DICH_VU:
                        loadMps = new MpsBehavior.Mps000463.Mps000463Behavior(this.roomId, this.PatientTypeAlter, SereServs, DepartmentTrans, TreatmentFees, Treatment, this.Patient, Rooms, Services, HeinServiceTypes, TotalDayTreatment, StatusTreatmentOut, DepartmentName, RoomName, UserNameReturnResult, this.SereServBills, this.SereServDeposits, this.SeseDepoRepays);
                        break;
                }

                result = loadMps != null ? loadMps.Load(printCode, fileName, this.ReturnEventPrint) : false;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
    }
}
