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
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.CareCreate.Resources;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CareCreate
{
    public partial class CareCreate : HIS.Desktop.Utility.FormBase
    {
        internal List<MPS.Processor.Mps000069.PDO.CareViewPrintADO> lstCareViewPrintADO { get; set; }
        internal List<MPS.Processor.Mps000069.PDO.CareDetailViewPrintADO> lstCareDetailViewPrintADO { get; set; }
        internal List<HIS_CARE> careByTreatmentHasIcd { get; set; }

        internal List<MPS.Processor.Mps000069.PDO.CreatorADO> _CreatorADOs = null;
        internal List<MPS.Processor.Mps000069.PDO.CareDescription> _careDescription { get; set; }
        internal List<MPS.Processor.Mps000069.PDO.InstructionDescription> _instructionDescription { get; set; }

        PopupMenu menu;
        internal void LoadBtnPrintGridViewCare(DevExpress.XtraBars.BarManager barManager)
        {
            try
            {
                if (menu == null)
                    menu = new PopupMenu(barManager);

                menu.ItemLinks.Clear();

                BarButtonItem itemInPhieuKetQuaChamSoc = new BarButtonItem(barManager, "Phiếu kết quả chăm sóc", 1);
                itemInPhieuKetQuaChamSoc.Tag = PrintTypeCare.IN_KET_QUA_CHAM_SOC;
                itemInPhieuKetQuaChamSoc.ItemClick += new ItemClickEventHandler(onClickIteamCare);

                menu.AddItems(new BarItem[] { itemInPhieuKetQuaChamSoc });
                menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void onClickIteamCare(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                var bbtnItem1 = e.Item as BarButtonItem;
                PrintTypeCare type = (PrintTypeCare)(bbtnItem1.Tag);
                PrintProcess(type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        void onClickPrintCare()
        {
            try
            {
                if (gridViewCareDetail.HasColumnErrors)
                    return;
                if (!btnCboPrint.Enabled)
                    return;
                PrintTypeCare type = new PrintTypeCare();
                type = PrintTypeCare.IN_KET_QUA_CHAM_SOC;
                PrintProcess(type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal enum PrintTypeCare
        {
            IN_KET_QUA_CHAM_SOC,
            IN_PHIEU_CHAM_SOC_QY7,
            IN_PHIEU_CHAM_SOC_CAP_I
        }

        void PrintProcess(PrintTypeCare printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintTypeCare.IN_KET_QUA_CHAM_SOC:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_KET_QUA_CHAM_SOC__MPS000069, DelegateRunPrinterCare);
                        break;
                    case PrintTypeCare.IN_PHIEU_CHAM_SOC_QY7:
                        richEditorMain.RunPrintTemplate("Mps000229", DelegateRunPrinterCare);
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

        bool DelegateRunPrinterCare(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_KET_QUA_CHAM_SOC__MPS000069:
                        LoadBieuMauPhieuYCKetQuaChamSoc(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000229":
                        LoadBieuMauPhieuQy7(printTypeCode, fileName, ref result);
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

        private void LoadBieuMauPhieuYCKetQuaChamSoc(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                this._CreatorADOs = new List<MPS.Processor.Mps000069.PDO.CreatorADO>();
                var currentPatient = PrintGlobalStore.getPatient(currentTreatmentId);
                MPS.Processor.Mps000069.PDO.PatientADO patientADO69 = new MPS.Processor.Mps000069.PDO.PatientADO();
                if (currentPatient != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000069.PDO.PatientADO>(patientADO69, currentPatient);
                }

                var departmentTran = PrintGlobalStore.getDepartmentTran(currentTreatmentId);

                HIS_TREATMENT currentTreatment = new HIS_TREATMENT();
                MOS.Filter.HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.ID = currentTreatmentId;
                currentTreatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();

                LoadDataToRadioGroupAwareness(ref lstHisAwareness);
                LoadDataToGridControlCareDetail(ref lstHisCareType);

                AddCareData(currentTreatmentId);
                //
                MPS.Processor.Mps000069.PDO.Mps000069ADO mps000069ADO = new MPS.Processor.Mps000069.PDO.Mps000069ADO();
                if (currentTreatment != null)
                {
                    if (!string.IsNullOrEmpty(currentTreatment.ICD_CODE))
                    {
                        mps000069ADO.ICD_CODE = currentTreatment.ICD_CODE;
                        var icd = BackendDataWorker.Get<HIS_ICD>().Where(p => p.ICD_CODE == currentTreatment.ICD_CODE).FirstOrDefault();
                        if (icd != null && icd.ICD_NAME != currentTreatment.ICD_NAME)
                        {
                            mps000069ADO.ICD_MAIN_TEXT = currentTreatment.ICD_NAME;
                        }
                        else
                        {
                            mps000069ADO.ICD_NAME = icd.ICD_NAME;
                        }

                    }

                    mps000069ADO.ICD_TEXT = currentTreatment.ICD_TEXT;
                    mps000069ADO.ICD_SUB_CODE = currentTreatment.ICD_SUB_CODE;
                }

                mps000069ADO.DEPARTMENT_NAME = WorkPlace.WorkPlaceSDO.SingleOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentName;
                mps000069ADO.ROOM_NAME = WorkPlace.WorkPlaceSDO.SingleOrDefault(p => p.RoomId == this.currentModule.RoomId).RoomName;
                MOS.Filter.HisTreatmentBedRoomViewFilter bedRoomFilter = new HisTreatmentBedRoomViewFilter();
                bedRoomFilter.TREATMENT_ID = this.currentTreatmentId;
                bedRoomFilter.ORDER_FIELD = "CREATE_TIME";
                bedRoomFilter.ORDER_DIRECTION = "DESC";
                var rsBedName = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM>>(HisRequestUriStore.HIS_TREATMENT_BED_ROOM_GETVIEW, ApiConsumers.MosConsumer, bedRoomFilter, param).FirstOrDefault();
                if (rsBedName != null)
                {
                    mps000069ADO.BED_NAME = rsBedName.BED_NAME;
                }
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((currentTreatment != null ? currentTreatment.TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);


                // long keyPrintMerge = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKeys.CONFIG_KEY__HIS_DESKTOP_PLUGINS_CARE_IS_PRINT_MERGE));
                // if (keyPrintMerge == 1)
                // {
                // string uniqueTime = "";// (_TrackingPrints != null && _TrackingPrints.Count > 0) ? _TrackingPrints[0].TRACKING_TIME + "" : "";
                // inputADO.MergeCode = String.Format("{0}_{1}_{2}", printTypeCode, uniqueTime, (Treatment != null ? Treatment.TREATMENT_CODE : ""));
                // }
                // Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => inputADO.MergeCode), inputADO.MergeCode));


                MPS.Processor.Mps000069.PDO.Mps000069PDO mps000069RDO = new MPS.Processor.Mps000069.PDO.Mps000069PDO(
                    patientADO69,
                    mps000069ADO,
                    currentTreatment,
                    careByTreatmentHasIcd,
                    lstCareViewPrintADO,
                    lstCareDetailViewPrintADO,
                     this._CreatorADOs,
                     this._careDescription,
                     this._instructionDescription
                                );
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000069RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000069RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                }
                result = MPS.MpsPrinter.Run(PrintData);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void AddCareData(long currentTreatmentId)
        {
            try
            {
                if (currentTreatmentId > 0)
                {
                    WaitingManager.Show();
                    this._careDescription = new List<MPS.Processor.Mps000069.PDO.CareDescription>();
                    this.lstCareViewPrintADO = new List<MPS.Processor.Mps000069.PDO.CareViewPrintADO>();
                    this.lstCareDetailViewPrintADO = new List<MPS.Processor.Mps000069.PDO.CareDetailViewPrintADO>();
                    this._instructionDescription = new List<MPS.Processor.Mps000069.PDO.InstructionDescription>();
                    #region ------
                    for (int i = 0; i < 19; i++)
                    {
                        MPS.Processor.Mps000069.PDO.CareViewPrintADO careViewPrintSDO = new MPS.Processor.Mps000069.PDO.CareViewPrintADO();
                        switch (i)
                        {
                            case 0:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.NgayThang;// "Ngày tháng";
                                break;
                            case 1:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.Gio;// "Giờ";
                                break;
                            case 2:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.YThuc;// "Ý thức";
                                break;
                            case 3:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.DaNiemMac;//"Da, niêm mạc";
                                break;
                            case 4:
                                careViewPrintSDO.CARE_TITLE2 = ResourceMessage.Mach;// "Mạch (lần/phút)";
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.DauHieuSinhTon;// "Dấu hiệu sinh tồn";
                                break;
                            case 5:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.DauHieuSinhTon;// "Dấu hiệu sinh tồn";
                                careViewPrintSDO.CARE_TITLE2 = ResourceMessage.NhietDo;// "Nhiệt độ (độ C)";
                                break;
                            case 6:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.DauHieuSinhTon;//"Dấu hiệu sinh tồn";
                                careViewPrintSDO.CARE_TITLE2 = ResourceMessage.HuyetAp;//"Huyết áp (mmHg)";
                                break;
                            case 7:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.DauHieuSinhTon;// "Dấu hiệu sinh tồn";
                                careViewPrintSDO.CARE_TITLE2 = ResourceMessage.NhipTho;//"Nhịp thở (lần/phút)";
                                break;
                            case 8:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.DauHieuSinhTon;// "Dấu hiệu sinh tồn";
                                careViewPrintSDO.CARE_TITLE2 = ResourceMessage.Khac;//"khác";
                                break;
                            case 9:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.NuocTieu;//"Nước tiểu (ml)";
                                break;
                            case 10:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.Phan;// "Phân (g)";
                                break;
                            case 11:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.CanNang;//"Cân nặng (kg)";
                                break;
                            case 12:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.ThucHienYLenh;//"Thực hiện y lệnh";
                                careViewPrintSDO.CARE_TITLE2 = ResourceMessage.ThuocThuongQuy;//"Thuốc thường quy";
                                break;
                            case 13:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.ThucHienYLenh;//"Thực hiện y lệnh";
                                careViewPrintSDO.CARE_TITLE2 = ResourceMessage.ThuocBoSung;//"Thuốc bổ sung";
                                break;
                            case 14:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.ThucHienYLenh;//"Thực hiện y lệnh";
                                careViewPrintSDO.CARE_TITLE2 = ResourceMessage.XetNghiem;//"Xét nghiệm";
                                break;
                            case 15:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.ThucHienYLenh;//"Thực hiện y lệnh";
                                careViewPrintSDO.CARE_TITLE2 = ResourceMessage.CheDoAn;//"Chế độ ăn";
                                break;
                            case 16:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.VeSinh;//"Vệ sinh/thay quần áo-ga";
                                break;
                            case 17:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.HuongDanNoiQuy;//"HD nội quy";
                                break;
                            case 18:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.GiaoDucSucKhoe;//"Giáo dục sức khỏe";
                                break;
                            default:
                                break;
                        }

                        careViewPrintSDO.CARE_1 = "";
                        careViewPrintSDO.CARE_2 = "";
                        careViewPrintSDO.CARE_3 = "";
                        careViewPrintSDO.CARE_4 = "";
                        careViewPrintSDO.CARE_5 = "";
                        careViewPrintSDO.CARE_6 = "";
                        careViewPrintSDO.CARE_7 = "";
                        careViewPrintSDO.CARE_8 = "";
                        careViewPrintSDO.CARE_9 = "";
                        careViewPrintSDO.CARE_10 = "";
                        careViewPrintSDO.CARE_11 = "";
                        careViewPrintSDO.CARE_12 = "";
                        this.lstCareViewPrintADO.Add(careViewPrintSDO);
                    }

                    CommonParam paramGet = new CommonParam();
                    List<MOS.EFMODEL.DataModels.HIS_CARE> lstHisCareByTreatment = new List<HIS_CARE>();
                    if (this.hisCareCurrent != null)
                    {
                        lstHisCareByTreatment.Add(this.hisCareCurrent);
                    }
                    if (lstHisCareByTreatment != null && lstHisCareByTreatment.Count > 0)
                    {
                        lstHisCareByTreatment = lstHisCareByTreatment.Skip(0).Take(6).ToList();
                        careByTreatmentHasIcd = new List<HIS_CARE>();
                        careByTreatmentHasIcd = lstHisCareByTreatment;

                        foreach (var item in lstHisCareByTreatment)
                        {
                            MOS.Filter.HisDhstFilter hisDHSTFilter = new HisDhstFilter();
                            hisDHSTFilter.CARE_ID = item.ID;

                            List<MOS.EFMODEL.DataModels.HIS_DHST> hisDHST = new BackendAdapter(paramGet).Get<List<HIS_DHST>>(HisRequestUriStore.HIS_DHST_GET, ApiConsumers.MosConsumer, hisDHSTFilter, paramGet);
                            if (hisDHST != null && hisDHST.Count > 0)
                            {
                                item.HIS_DHST = hisDHST.FirstOrDefault();
                            }
                        }
                    }
                    for (int i = 0; i < lstCareViewPrintADO.Count; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], Inventec.Common.DateTime.Convert.TimeNumberToDateString(lstHisCareByTreatment[j].EXECUTE_TIME ?? 0));
                                }
                                break;
                            case 1:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(lstHisCareByTreatment[j].EXECUTE_TIME ?? 0) == null ? "" : Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(lstHisCareByTreatment[j].EXECUTE_TIME ?? 0).Value.ToString("HH:mm"));
                                }
                                break;
                            case 2:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    var awarenest = lstHisAwareness.FirstOrDefault(o => o.ID == lstHisCareByTreatment[j].AWARENESS_ID);
                                    //if (awarenest != null)
                                    //{
                                    System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].AWARENESS);
                                    //}
                                }
                                break;
                            case 3:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].MUCOCUTANEOUS);//da\
                                }
                                break;
                            case 4:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    if (lstHisCareByTreatment[j].HIS_DHST != null && lstHisCareByTreatment[j].HIS_DHST.PULSE.HasValue)
                                    {
                                        System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                        pi.SetValue(lstCareViewPrintADO[i], ((long)lstHisCareByTreatment[j].HIS_DHST.PULSE).ToString());//mach 
                                    }
                                }
                                break;
                            case 5:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    if (lstHisCareByTreatment[j].HIS_DHST != null && lstHisCareByTreatment[j].HIS_DHST.TEMPERATURE.HasValue)
                                    {
                                        System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                        pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].HIS_DHST.TEMPERATURE.ToString());//nhiet do
                                    }
                                }
                                break;
                            case 6:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    if (lstHisCareByTreatment[j].HIS_DHST != null)
                                    {
                                        System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                        string strBloodPressure = "";
                                        strBloodPressure += (lstHisCareByTreatment[j].HIS_DHST.BLOOD_PRESSURE_MAX.HasValue ? ((long)lstHisCareByTreatment[j].HIS_DHST.BLOOD_PRESSURE_MAX).ToString() : "");
                                        strBloodPressure += (lstHisCareByTreatment[j].HIS_DHST.BLOOD_PRESSURE_MIN.HasValue ? "/" + ((long)lstHisCareByTreatment[j].HIS_DHST.BLOOD_PRESSURE_MIN).ToString() : "");
                                        pi.SetValue(lstCareViewPrintADO[i], strBloodPressure);//huyet ap
                                    }
                                }
                                break;
                            case 7:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    if (lstHisCareByTreatment[j].HIS_DHST != null && lstHisCareByTreatment[j].HIS_DHST.BREATH_RATE.HasValue)
                                    {
                                        System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                        pi.SetValue(lstCareViewPrintADO[i], ((long)lstHisCareByTreatment[j].HIS_DHST.BREATH_RATE).ToString());//nhip tho
                                    }
                                }
                                break;
                            case 8:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    if (lstHisCareByTreatment[j].HIS_DHST != null)
                                    {
                                        System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                        pi.SetValue(lstCareViewPrintADO[i], (lstHisCareByTreatment[j].HIS_DHST.NOTE));//khác
                                    }
                                }
                                break;
                            case 9:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].URINE);//Nước tiểu (ml)
                                }
                                break;
                            case 10:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].DEJECTA);//Phân (g) (ml)
                                }
                                break;
                            case 11:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)//Cân nặng
                                {
                                    if (lstHisCareByTreatment[j].HIS_DHST != null && lstHisCareByTreatment[j].HIS_DHST.WEIGHT.HasValue)
                                    {
                                        System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                        var weight = ((long)lstHisCareByTreatment[j].HIS_DHST.WEIGHT).ToString();
                                        pi.SetValue(lstCareViewPrintADO[i], weight.Trim() == "0" ? "" : weight);
                                    }
                                }
                                break;
                            case 12:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].HAS_MEDICINE == 1 ? "X" : "");//Thuốc thường quy
                                }
                                break;
                            case 13:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].HAS_ADD_MEDICINE == 1 ? "X" : "");//Thuốc bổ sung
                                }
                                break;
                            case 14:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].HAS_TEST == 1 ? "X" : "");//Xét nghiệm
                                }
                                break;
                            case 15:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].NUTRITION);//Chế độ ăn
                                }
                                break;
                            case 16:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].SANITARY);//Vệ sinh/thay quần áo-ga
                                }
                                break;
                            case 17:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].TUTORIAL);//HD nội quy
                                }
                                break;
                            case 18:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].EDUCATION);//Giáo dục sức khỏe
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    #endregion

                    //Diễn biến

                    MPS.Processor.Mps000069.PDO.CareDescription _careDescriptionPDO = new MPS.Processor.Mps000069.PDO.CareDescription();
                    _careDescriptionPDO.CARE_DESCRIPTION = ResourceMessage.DienBien;
                    this._careDescription.Add(_careDescriptionPDO);

                    for (int h = 0; h < _careDescription.Count; h++)
                    {
                        for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                        {
							try
							{
                                System.Reflection.PropertyInfo piDescription = typeof(MPS.Processor.Mps000069.PDO.CareDescription).GetProperty("CARE_DESCRIPTION_" + (j + 1));
                                string dd = lstHisCareByTreatment[j].CARE_DESCRIPTION;
                                piDescription.SetValue(_careDescription[h], dd);
                            }
							catch (Exception ex)
							{
                                Inventec.Common.Logging.LogSystem.Error("CARE_DESCRIPTION" + j+"__"+ex);
                            }
                           
                        }
                    }


                    MPS.Processor.Mps000069.PDO.InstructionDescription _instruction = new MPS.Processor.Mps000069.PDO.InstructionDescription();
                    _instruction.INSTRUCTION_DESCRIPTION = "Y lệnh";
                    this._instructionDescription.Add(_instruction);

                    for (int h = 0; h < _instructionDescription.Count; h++)
                    {
                        for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                        {
							try
							{
                                System.Reflection.PropertyInfo piDescriptionA = typeof(MPS.Processor.Mps000069.PDO.InstructionDescription).GetProperty("INSTRUCTION_DESCRIPTION_" + (j + 1));
                                string dd = lstHisCareByTreatment[j].INSTRUCTION_DESCRIPTION;
                                piDescriptionA.SetValue(_instructionDescription[h], dd);
                            }
							catch (Exception ex)
							{
                                Inventec.Common.Logging.LogSystem.Error("INSTRUCTION_DESCRIPTION" + j+"__"+ex);
                            }
                           
                        }
                    }

                    for (int k = 0; k < lstHisCareByTreatment.Count; k++)
                    {
                        //In 1 cai, check lại với in nhiều//xuandv
                        MPS.Processor.Mps000069.PDO.CreatorADO _creator = new MPS.Processor.Mps000069.PDO.CreatorADO();
                        _creator.CARE_ID = lstHisCareByTreatment[k].ID;

                        System.Reflection.PropertyInfo piCreator = typeof(MPS.Processor.Mps000069.PDO.CreatorADO).GetProperty("CREATOR_" + (k + 1));
                        piCreator.SetValue(_creator, lstHisCareByTreatment[k].CREATOR);

                        var userName = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(p => p.LOGINNAME == lstHisCareByTreatment[k].CREATOR).USERNAME;
                        System.Reflection.PropertyInfo piUser = typeof(MPS.Processor.Mps000069.PDO.CreatorADO).GetProperty("USER_NAME_" + (k + 1));
                        piUser.SetValue(_creator, userName);

                        this._CreatorADOs.Add(_creator);
                        //Add Theo dõi - Chăm sóc
                        MOS.Filter.HisCareDetailViewFilter hisCareDetailFilter = new MOS.Filter.HisCareDetailViewFilter();
                        hisCareDetailFilter.CARE_ID = lstHisCareByTreatment[k].ID;

                        List<MOS.EFMODEL.DataModels.V_HIS_CARE_DETAIL> lstHisCareDetail = new BackendAdapter(paramGet).Get<List<V_HIS_CARE_DETAIL>>(HisRequestUriStore.HIS_CARE_DETAIL_GETVIEW, ApiConsumers.MosConsumer, hisCareDetailFilter, paramGet);


                        if (lstHisCareDetail != null && lstHisCareDetail.Count > 0)
                        {
                            // var careTypeIds = lstHisCareDetail.Select(o => o.CARE_TYPE_ID).Distinct().ToArray();
                            foreach (var caty in lstHisCareDetail)
                            {
                                if (!this.lstCareDetailViewPrintADO.Any(o => o.CARE_TYPE_ID == caty.CARE_TYPE_ID))
                                {
                                    MPS.Processor.Mps000069.PDO.CareDetailViewPrintADO careDetailViewPrintSDO = new MPS.Processor.Mps000069.PDO.CareDetailViewPrintADO();
                                    careDetailViewPrintSDO.CARE_TYPE_ID = caty.CARE_TYPE_ID;
                                    careDetailViewPrintSDO.CARE_TITLE = ResourceMessage.TheoDoiChamSoc;// "Theo dõi - Chăm sóc";
                                    careDetailViewPrintSDO.CARE_DETAIL = caty.CARE_TYPE_NAME;
                                    careDetailViewPrintSDO.CARE_DETAIL_1 = "";
                                    careDetailViewPrintSDO.CARE_DETAIL_2 = "";
                                    careDetailViewPrintSDO.CARE_DETAIL_3 = "";
                                    careDetailViewPrintSDO.CARE_DETAIL_4 = "";
                                    careDetailViewPrintSDO.CARE_DETAIL_5 = "";
                                    careDetailViewPrintSDO.CARE_DETAIL_6 = "";
                                    this.lstCareDetailViewPrintADO.Add(careDetailViewPrintSDO);
                                }
                            }


                            foreach (var item in this.lstCareDetailViewPrintADO)
                            {
                                var careDetailForOnes = lstHisCareDetail.Where(o => o.CARE_TYPE_ID == item.CARE_TYPE_ID).ToList();
                                if (careDetailForOnes != null && careDetailForOnes.Count > 0)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareDetailViewPrintADO).GetProperty("CARE_DETAIL_" + (k + 1));
                                    pi.SetValue(item, careDetailForOnes[0].CONTENT);
                                }
                            }
                        }

                    }

                    int countCaTyPrint = 6 - this.lstCareDetailViewPrintADO.Count;
                    if (countCaTyPrint > 0)
                    {
                        for (int i = 0; i < countCaTyPrint; i++)
                        {
                            MPS.Processor.Mps000069.PDO.CareDetailViewPrintADO careDetailViewPrintSDO = new MPS.Processor.Mps000069.PDO.CareDetailViewPrintADO();
                            careDetailViewPrintSDO.CARE_TITLE = ResourceMessage.TheoDoiChamSoc;// "Theo dõi - Chăm sóc";
                            careDetailViewPrintSDO.CARE_DETAIL_1 = "";
                            careDetailViewPrintSDO.CARE_DETAIL_2 = "";
                            careDetailViewPrintSDO.CARE_DETAIL_3 = "";
                            careDetailViewPrintSDO.CARE_DETAIL_4 = "";
                            careDetailViewPrintSDO.CARE_DETAIL_5 = "";
                            careDetailViewPrintSDO.CARE_DETAIL_6 = "";
                            this.lstCareDetailViewPrintADO.Add(careDetailViewPrintSDO);
                        }
                    }
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void InitMenuToButtonPrint()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemPhieuChamSoc = new DXMenuItem("Phiếu chăm sóc", new EventHandler(OnClickInPhieuChamSoc));
                itemPhieuChamSoc.Tag = PrintTypeCare.IN_KET_QUA_CHAM_SOC;
                menu.Items.Add(itemPhieuChamSoc);

                DXMenuItem itemInChamSocQy7 = new DXMenuItem("Phiếu chăm sóc _ Y lệnh", new EventHandler(OnClickInPhieuChamSoc));
                itemInChamSocQy7.Tag = PrintTypeCare.IN_PHIEU_CHAM_SOC_QY7;
                menu.Items.Add(itemInChamSocQy7);

                btnCboPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnClickInPhieuChamSoc(object sender, EventArgs e)
        {
            try
            {
                var bbtnItem = sender as DXMenuItem;
                PrintTypeCare type = (PrintTypeCare)(bbtnItem.Tag);
                PrintProcess(type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadBieuMauPhieuQy7(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                var _Treatment = PrintGlobalStore.getTreatment(this.currentTreatmentId);
                MOS.Filter.HisTreatmentBedRoomViewFilter filter = new HisTreatmentBedRoomViewFilter();
                filter.TREATMENT_ID = this.currentTreatmentId;
                filter.ORDER_FIELD = "CREATE_TIME";
                filter.ORDER_DIRECTION = "DESC";
                V_HIS_TREATMENT_BED_ROOM _TreatmetnbedRoom = new V_HIS_TREATMENT_BED_ROOM();
                var TreatmetnbedRooms = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetView", ApiConsumers.MosConsumer, filter, null);
                if (TreatmetnbedRooms != null && TreatmetnbedRooms.Count > 0)
                {
                    _TreatmetnbedRoom = TreatmetnbedRooms.FirstOrDefault();
                }
                long keyPrintMerge = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKeys.CONFIG_KEY__HIS_DESKTOP_PLUGINS_CARE_IS_PRINT_MERGE));

                List<HIS_CARE> _CareChecks = new List<HIS_CARE>();
                _CareChecks.Add(this.hisCareCurrent);
                if (_CareChecks != null && _CareChecks.Count > 0)
                {
                    _CareChecks = _CareChecks.OrderBy(p => p.EXECUTE_TIME).ToList();
                                      
                    if (keyPrintMerge == 1 && _CareChecks.Count != 1)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Khi bật cấu hình in gộp phiếu chăm sóc chỉ được phép chọn 1 bản ghi phiếu chăm sóc để in", "Thông báo");
                        return;
                    }
                }

                var mps000229ADO = new MPS.Processor.Mps000229.PDO.Mps000229ADO();

                mps000229ADO.DEPARTMENT_NAME = WorkPlace.WorkPlaceSDO.SingleOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentName;
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_Treatment != null ? _Treatment.TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);

                if (keyPrintMerge == 1)
                {
                    string uniqueTime = "";// (_TrackingPrints != null && _TrackingPrints.Count > 0) ? _TrackingPrints[0].TRACKING_TIME + "" : "";
                    inputADO.MergeCode = String.Format("{0}_{1}_{2}", printTypeCode, uniqueTime, (_Treatment != null ? _Treatment.TREATMENT_CODE : ""));
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => inputADO.MergeCode), inputADO.MergeCode));

                MPS.Processor.Mps000229.PDO.Mps000229PDO mps000229RDO = new MPS.Processor.Mps000229.PDO.Mps000229PDO(
                  _Treatment,
                  _CareChecks,
                  _TreatmetnbedRoom,
                  mps000229ADO
                    );
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000229RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000229RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }
    }
}
