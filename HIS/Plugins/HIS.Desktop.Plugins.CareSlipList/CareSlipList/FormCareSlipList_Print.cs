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
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Print;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using Inventec.Core;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.BackendData;

namespace HIS.Desktop.Plugins.CareSlipList.CareSlipList
{
    public partial class FormCareSlipList : Form
    {
        //long treatmentId = 0;
        internal List<MPS.ADO.CareViewPrintADO> lstCareViewPrintADO { get; set; }
        internal List<MPS.ADO.CareDetailViewPrintADO> lstCareDetailViewPrintADO { get; set; }
        internal List<HIS_CARE> careByTreatmentHasIcd { get; set; }

        void onClickPrintCare()
        {
            try
            {
                PrintTypeCare type = new PrintTypeCare();
                type = PrintTypeCare.IN_KET_QUA_CHAM_SOC_TONG_HOP;
                PrintProcess(type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        internal enum PrintTypeCare
        {
            IN_KET_QUA_CHAM_SOC_TONG_HOP,
        }

        void PrintProcess(PrintTypeCare printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintTypeCare.IN_KET_QUA_CHAM_SOC_TONG_HOP:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuChamSocTongHop_MPS000151, DelegateRunPrinterCare);
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
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuChamSocTongHop_MPS000151:
                        LoadBieuMauPhieuYCKetQuaChamSocTongHop(printTypeCode, fileName, ref result);
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

        private void LoadBieuMauPhieuYCKetQuaChamSocTongHop(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                var currentPatient = PrintGlobalStore.getPatient(treatmentID);

                var departmentTran = PrintGlobalStore.getDepartmentTran(treatmentID);

                currentHisTreatment = PrintGlobalStore.getTreatment(treatmentID);

                LoadDataToRadioGroupAwareness(ref lstHisAwareness);
                LoadDataToGridControlCareDetail(ref lstHisCareType);
                AddCareData(treatmentID);
                //
                HIS_ICD icd = new HIS_ICD();
                if (currentHisTreatment != null)
                {
                    icd = BackendDataWorker.Get<HIS_ICD>().Where(p => p.ID == currentHisTreatment.ICD_ID).FirstOrDefault();
                }

                MPS.ADO.Mps000151ADO mps000151ADO = new MPS.ADO.Mps000151ADO();
                if (printCareSum != null)
                {
                    if (printCareSum.ICD_MAIN_TEXT != null)
                    {
                        mps000151ADO.ICD_MAIN_TEXT = printCareSum.ICD_MAIN_TEXT;
                    }
                    mps000151ADO.ICD_TEXT = printCareSum.ICD_TEXT;
                    mps000151ADO.ICD_NAME = printCareSum.ICD_NAME;
                    mps000151ADO.ICD_CODE = printCareSum.ICD_CODE;
                }
                    

                mps000151ADO.REQUEST_DEAPARTMENT_NAME = WorkPlace.WorkPlaceSDO.SingleOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentName;
                mps000151ADO.REQUEST_ROOM_NAME = WorkPlace.WorkPlaceSDO.SingleOrDefault(p => p.RoomId == this.currentModule.RoomId).RoomName;

                MPS.Core.Mps000151.Mps000151RDO mps000151RDO = new MPS.Core.Mps000151.Mps000151RDO(
                  currentPatient,
                  mps000151ADO,
                  careByTreatmentHasIcd,
                lstCareViewPrintADO,
                lstCareDetailViewPrintADO
                    );
                WaitingManager.Hide();
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.Printer.Run(printTypeCode, fileName, mps000151RDO, MPS.Printer.PreviewType.PrintNow);
                }
                else
                {
                    result = MPS.Printer.Run(printTypeCode, fileName, mps000151RDO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                    this.lstCareViewPrintADO = new List<MPS.ADO.CareViewPrintADO>();
                    this.lstCareDetailViewPrintADO = new List<MPS.ADO.CareDetailViewPrintADO>();

                    for (int i = 0; i < 18; i++)
                    {
                        MPS.ADO.CareViewPrintADO careViewPrintSDO = new MPS.ADO.CareViewPrintADO();
                        switch (i)
                        {
                            case 0:
                                careViewPrintSDO.CARE_TITLE1 = "Ngày tháng";
                                break;
                            case 1:
                                careViewPrintSDO.CARE_TITLE1 = "Giờ";
                                break;
                            case 2:
                                careViewPrintSDO.CARE_TITLE1 = "Ý thức";
                                break;
                            case 3:
                                careViewPrintSDO.CARE_TITLE1 = "Da, niêm mạc";
                                break;
                            case 4:
                                careViewPrintSDO.CARE_TITLE2 = "Mạch (lần/phút)";
                                careViewPrintSDO.CARE_TITLE1 = "Dấu hiệu sinh tồn";
                                break;
                            case 5:
                                careViewPrintSDO.CARE_TITLE1 = "Dấu hiệu sinh tồn";
                                careViewPrintSDO.CARE_TITLE2 = "Nhiệt độ (độ C)";
                                break;
                            case 6:
                                careViewPrintSDO.CARE_TITLE1 = "Dấu hiệu sinh tồn";
                                careViewPrintSDO.CARE_TITLE2 = "Huyết áp (mmHg)";
                                break;
                            case 7:
                                careViewPrintSDO.CARE_TITLE1 = "Dấu hiệu sinh tồn";
                                careViewPrintSDO.CARE_TITLE2 = "Nhịp thở (lần/phút)";
                                break;
                            case 8:
                                careViewPrintSDO.CARE_TITLE1 = "Nước tiểu (ml)";
                                break;
                            case 9:
                                careViewPrintSDO.CARE_TITLE1 = "Phân (g)";
                                break;
                            case 10:
                                careViewPrintSDO.CARE_TITLE1 = "Cân nặng (kg)";
                                break;
                            case 11:
                                careViewPrintSDO.CARE_TITLE1 = "Thực hiện y lệnh";
                                careViewPrintSDO.CARE_TITLE2 = "Thuốc thường quy";
                                break;
                            case 12:
                                careViewPrintSDO.CARE_TITLE1 = "Thực hiện y lệnh";
                                careViewPrintSDO.CARE_TITLE2 = "Thuốc bổ sung";
                                break;
                            case 13:
                                careViewPrintSDO.CARE_TITLE1 = "Thực hiện y lệnh";
                                careViewPrintSDO.CARE_TITLE2 = "Xét nghiệm";
                                break;
                            case 14:
                                careViewPrintSDO.CARE_TITLE1 = "Thực hiện y lệnh";
                                careViewPrintSDO.CARE_TITLE2 = "Chế độ ăn";
                                break;
                            case 15:
                                careViewPrintSDO.CARE_TITLE1 = "Vệ sinh/thay quần áo-ga";
                                break;
                            case 16:
                                careViewPrintSDO.CARE_TITLE1 = "HD nội quy";
                                break;
                            case 17:
                                careViewPrintSDO.CARE_TITLE1 = "Giáo dục sức khỏe";
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
                    MOS.Filter.HisCareFilter hisCareFilter = new MOS.Filter.HisCareFilter();
                    hisCareFilter.TREATMENT_ID = currentTreatmentId;
                    List<MOS.EFMODEL.DataModels.HIS_CARE> lstHisCareByTreatment = new BackendAdapter(paramGet).Get<List<HIS_CARE>>(ApiConsumer.HisRequestUriStore.HIS_CARE_GET, ApiConsumers.MosConsumer, hisCareFilter, paramGet);

                    if (lstHisCareByTreatment != null && lstHisCareByTreatment.Count > 0)
                    {
                        lstHisCareByTreatment = lstHisCareByTreatment.Skip(0).Take(6).ToList();
                        careByTreatmentHasIcd = new List<HIS_CARE>();
                        careByTreatmentHasIcd = lstHisCareByTreatment;//.Where(o => (o.ICD_ID != null || !String.IsNullOrEmpty(o.ICD_TEXT))).OrderByDescending(o => o.EXECUTE_TIME).ToList();

                        foreach (var item in lstHisCareByTreatment)
                        {
                            if (item.DHST_ID > 0)
                            {
                                MOS.Filter.HisDhstFilter hisDHSTFilter = new HisDhstFilter();
                                hisDHSTFilter.ID = item.DHST_ID;

                                List<MOS.EFMODEL.DataModels.HIS_DHST> hisDHST = new BackendAdapter(paramGet).Get<List<HIS_DHST>>(ApiConsumer.HisRequestUriStore.HIS_DHST_GET, ApiConsumers.MosConsumer, hisDHSTFilter, paramGet);
                                if (hisDHST != null && hisDHST.Count > 0)
                                {
                                    item.HIS_DHST = hisDHST.FirstOrDefault();
                                }
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
                                    System.Reflection.PropertyInfo pi = typeof(MPS.ADO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], Inventec.Common.DateTime.Convert.TimeNumberToDateString(lstHisCareByTreatment[j].EXECUTE_TIME ?? 0));
                                }
                                break;
                            case 1:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.ADO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(lstHisCareByTreatment[j].EXECUTE_TIME ?? 0) == null ? "" : Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(lstHisCareByTreatment[j].EXECUTE_TIME ?? 0).Value.ToString("HH:mm"));
                                }
                                break;
                            case 2:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    var awarenest = lstHisAwareness.FirstOrDefault(o => o.ID == lstHisCareByTreatment[j].AWARENESS_ID);
                                    //if (awarenest != null)
                                    //{
                                    System.Reflection.PropertyInfo pi = typeof(MPS.ADO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].AWARENESS);
                                    //}
                                }
                                break;
                            case 3:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.ADO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].MUCOCUTANEOUS);//da\
                                }
                                break;
                            case 4:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    if (lstHisCareByTreatment[j].HIS_DHST != null)
                                    {
                                        System.Reflection.PropertyInfo pi = typeof(MPS.ADO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                        pi.SetValue(lstCareViewPrintADO[i], ((long)lstHisCareByTreatment[j].HIS_DHST.PULSE).ToString());//mach 
                                    }
                                }
                                break;
                            case 5:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    if (lstHisCareByTreatment[j].HIS_DHST != null)
                                    {
                                        System.Reflection.PropertyInfo pi = typeof(MPS.ADO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                        pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].HIS_DHST.TEMPERATURE.ToString());//nhiet do
                                    }
                                }
                                break;
                            case 6:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    if (lstHisCareByTreatment[j].HIS_DHST != null)
                                    {
                                        System.Reflection.PropertyInfo pi = typeof(MPS.ADO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
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
                                    if (lstHisCareByTreatment[j].HIS_DHST != null)
                                    {
                                        System.Reflection.PropertyInfo pi = typeof(MPS.ADO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                        pi.SetValue(lstCareViewPrintADO[i], ((long)lstHisCareByTreatment[j].HIS_DHST.BREATH_RATE).ToString());//nhip tho
                                    }
                                }
                                break;
                            case 8:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.ADO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].URINE);//Nước tiểu (ml)
                                }
                                break;
                            case 9:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.ADO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].DEJECTA);//Phân (g) (ml)
                                }
                                break;
                            case 10:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)//Cân nặng
                                {
                                    if (lstHisCareByTreatment[j].HIS_DHST != null)
                                    {
                                        System.Reflection.PropertyInfo pi = typeof(MPS.ADO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                        var weight = ((long)lstHisCareByTreatment[j].HIS_DHST.WEIGHT).ToString();
                                        pi.SetValue(lstCareViewPrintADO[i], weight.Trim() == "0" ? "" : weight);
                                    }
                                }

                                //for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                //{
                                //    System.Reflection.PropertyInfo pi = typeof(EXE.SDO.CareViewPrintSDO).GetProperty("CARE_" + (j + 1));
                                //    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].WEIGHT.ToString());//can nang
                                //}
                                break;
                            case 11:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.ADO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].HAS_MEDICINE == 1 ? "X" : "");//Thuốc thường quy
                                }
                                break;
                            case 12:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.ADO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].HAS_ADD_MEDICINE == 1 ? "X" : "");//Thuốc bổ sung
                                }
                                break;
                            case 13:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.ADO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].HAS_TEST == 1 ? "X" : "");//Xét nghiệm
                                }
                                break;
                            case 14:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.ADO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].NUTRITION);//Chế độ ăn
                                }
                                break;
                            case 15:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.ADO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].SANITARY);//Vệ sinh/thay quần áo-ga
                                }
                                break;
                            case 16:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.ADO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].TUTORIAL);//HD nội quy
                                }
                                break;
                            case 17:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.ADO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].EDUCATION);//Giáo dục sức khỏe
                                }
                                break;
                            default:
                                break;
                        }
                    }

                    for (int k = 0; k < lstHisCareByTreatment.Count; k++)
                    {
                        //Add Theo dõi - Chăm sóc
                        MOS.Filter.HisCareDetailViewFilter hisCareDetailFilter = new MOS.Filter.HisCareDetailViewFilter();
                        hisCareDetailFilter.CARE_ID = lstHisCareByTreatment[k].ID;
                        //if (fromTime != null && fromTime.Value != DateTime.MinValue)
                        //    hisCareDetailFilter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(fromTime.Value).ToString("yyyyMMddHHmm") + "00");
                        //if (toTime != null && toTime.Value != DateTime.MinValue)
                        //    hisCareDetailFilter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(toTime.Value).ToString("yyyyMMddHHmm") + "00");

                        List<MOS.EFMODEL.DataModels.V_HIS_CARE_DETAIL> lstHisCareDetail = new BackendAdapter(paramGet).Get<List<V_HIS_CARE_DETAIL>>(ApiConsumer.HisRequestUriStore.HIS_CARE_DETAIL_GETVIEW, ApiConsumers.MosConsumer, hisCareDetailFilter, paramGet);


                        if (lstHisCareDetail != null && lstHisCareDetail.Count > 0)
                        {
                            var careTypeIds = lstHisCareDetail.Select(o => o.CARE_TYPE_ID).Distinct().ToArray();
                            foreach (var caty in lstHisCareDetail)
                            {
                                if (!this.lstCareDetailViewPrintADO.Any(o => o.CARE_TYPE_ID == caty.CARE_TYPE_ID))
                                {
                                    MPS.ADO.CareDetailViewPrintADO careDetailViewPrintSDO = new MPS.ADO.CareDetailViewPrintADO();
                                    careDetailViewPrintSDO.CARE_TYPE_ID = caty.CARE_TYPE_ID;
                                    careDetailViewPrintSDO.CARE_TITLE = "Theo dõi - Chăm sóc";
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
                                    System.Reflection.PropertyInfo pi = typeof(MPS.ADO.CareDetailViewPrintADO).GetProperty("CARE_DETAIL_" + (k + 1));
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
                            MPS.ADO.CareDetailViewPrintADO careDetailViewPrintSDO = new MPS.ADO.CareDetailViewPrintADO();
                            careDetailViewPrintSDO.CARE_TITLE = "Theo dõi - Chăm sóc";
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
