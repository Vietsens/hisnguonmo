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
using FlexCel.Report;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000151.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000151
{
    public class Mps000151Processor : AbstractProcessor
    {
        Mps000151PDO rdo;
        private List<CareViewPrintADO> ListCareViewADO = new List<CareViewPrintADO>();
        private List<CareDetailViewPrintADO> ListCareDetailViewADO = new List<CareDetailViewPrintADO>();
        private List<CreatorADO> ListCreator = new List<CreatorADO>();
        private List<TotalTemplateADO> Total = new List<TotalTemplateADO>();
        private List<Mps000151PDO.CareDescription> _careDescription = new List<Mps000151PDO.CareDescription>();
        private List<Mps000151PDO.InstructionDescription> _instructionDescription = new List<Mps000151PDO.InstructionDescription>();
        public Mps000151Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000151PDO)rdoBase;
        }

        /// <summary>
        /// Ham xu ly du lieu da qua xu ly
        /// Tao ra cac doi tuong du lieu xu dung trong thu vien xu ly file excel
        /// </summary>
        /// <returns></returns>
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));

                ProcessSingleKey();
                ProcessListDataADO();

                this.SetSignatureKeyImageByCFG();

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "Total", Total);
                objectTag.AddObjectData(store, "Cares", ListCareViewADO);
                objectTag.AddObjectData(store, "CareDetails", ListCareDetailViewADO);
                objectTag.AddObjectData(store, "Creators", ListCreator);
                objectTag.AddObjectData(store, "Care", _careDescription);
                objectTag.AddObjectData(store, "InstructionDescription", _instructionDescription);
                objectTag.AddRelationship(store, "Total", "Cares", "PARENT_ID", "PARENT_ID");
                objectTag.AddRelationship(store, "Total", "CareDetails", "PARENT_ID", "PARENT_ID");
                objectTag.AddRelationship(store, "Total", "Creators", "PARENT_ID", "PARENT_ID");
                objectTag.AddRelationship(store, "Total", "Care", "PARENT_ID", "PARENT_ID");
                objectTag.AddRelationship(store, "Total", "InstructionDescription", "PARENT_ID", "PARENT_ID");
                objectTag.SetUserFunction(store, "FuncSameTitleRow", new CustomerFuncMergeSameData(ListCareViewADO, 1));
                objectTag.SetUserFunction(store, "FuncSameTitleCol", new CustomerFuncMergeSameData(ListCareViewADO, 2));
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void ProcessListDataADO()
        {
            try
            {
                if (rdo != null && rdo.ListHisCareByTreatment != null && rdo.ListHisCareByTreatment.Count > 0)
                {
                    if (rdo.ListHisDhstByHisCare != null && rdo.ListHisDhstByHisCare.Count > 0)
                    {
                        foreach (var item in rdo.ListHisCareByTreatment)
                        {
                            var dhst = rdo.ListHisDhstByHisCare.FirstOrDefault(o => o.CARE_ID == item.ID);
                            if (dhst != null)
                            {
                                item.HIS_DHST = dhst;
                            }
                        }
                    }
                    rdo.ListHisCareByTreatment = rdo.ListHisCareByTreatment.OrderBy(o => o.EXECUTE_TIME).ToList();
                    this._careDescription = new List<Mps000151PDO.CareDescription>();
                    this._instructionDescription = new List<Mps000151PDO.InstructionDescription>();
                    var skip = 0;
                    long id = 1;
                    while (rdo.ListHisCareByTreatment.Count - skip > 0)
                    {
                        var listHisCare = rdo.ListHisCareByTreatment.Skip(skip).Take(6).ToList();
                        skip += 6;

                        AddCareData(listHisCare, id);

                        TotalTemplateADO ado = new TotalTemplateADO(id);
                        Total.Add(ado);
                        id++;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AddCareData(List<HIS_CARE> listHisCare, long id)
        {
            try
            {
                if (listHisCare != null && listHisCare.Count > 0)
                {
                    List<CareViewPrintADO> lstCareViewPrintADO = new List<CareViewPrintADO>();
                    List<CareDetailViewPrintADO> lstCareDetailViewPrintADO = new List<CareDetailViewPrintADO>();
                    List<CreatorADO> _Creator = new List<CreatorADO>();

                    #region ------
                    for (int i = 0; i < 21; i++)
                    {
                        CareViewPrintADO careViewPrintSDO = new CareViewPrintADO();
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
                                careViewPrintSDO.CARE_TITLE1 = "Dấu hiệu sinh tồn";
                                careViewPrintSDO.CARE_TITLE2 = "SPO2";
                                break;
                            case 9:
                                careViewPrintSDO.CARE_TITLE1 = "Dấu hiệu sinh tồn";
                                careViewPrintSDO.CARE_TITLE2 = "Khác";
                                break;
                            case 10:
                                careViewPrintSDO.CARE_TITLE1 = "Nước tiểu (ml)";
                                break;
                            case 11:
                                careViewPrintSDO.CARE_TITLE1 = "Phân (g)";
                                break;
                            case 12:
                                careViewPrintSDO.CARE_TITLE1 = "Cân nặng (kg)";
                                break;
                            case 13:
                                careViewPrintSDO.CARE_TITLE1 = "Thực hiện y lệnh";
                                careViewPrintSDO.CARE_TITLE2 = "Thuốc thường quy";
                                break;
                            case 14:
                                careViewPrintSDO.CARE_TITLE1 = "Thực hiện y lệnh";
                                careViewPrintSDO.CARE_TITLE2 = "Thuốc bổ sung";
                                break;
                            case 15:
                                careViewPrintSDO.CARE_TITLE1 = "Thực hiện y lệnh";
                                careViewPrintSDO.CARE_TITLE2 = "Xét nghiệm";
                                break;
                            case 16:
                                careViewPrintSDO.CARE_TITLE1 = "Thực hiện y lệnh";
                                careViewPrintSDO.CARE_TITLE2 = "Chế độ ăn";
                                break;
                            case 17:
                                careViewPrintSDO.CARE_TITLE1 = "Vệ sinh/thay quần áo-ga";
                                break;
                            case 18:
                                careViewPrintSDO.CARE_TITLE1 = "HD nội quy";
                                break;
                            case 19:
                                careViewPrintSDO.CARE_TITLE1 = "Giáo dục sức khỏe";
                                break;
                            case 20:
                                careViewPrintSDO.CARE_TITLE1 = "Thực hiện y lệnh";
                                careViewPrintSDO.CARE_TITLE2 = "Điều trị PHCN";
                                break;
                            default:
                                break;
                        }

                        careViewPrintSDO.PARENT_ID = id;

                        lstCareViewPrintADO.Add(careViewPrintSDO);
                    }
                    #endregion

                    Mps000151PDO.CareDescription _careDescriptionPDO = new Mps000151PDO.CareDescription();
                    _careDescriptionPDO.CARE_DESCRIPTION = "Diễn biến";
                    _careDescriptionPDO.PARENT_ID = id;

                    for (int j = 0; j < listHisCare.Count; j++)
                    {
                        System.Reflection.PropertyInfo piDescription = typeof(Mps000151PDO.CareDescription).GetProperty("CARE_DESCRIPTION_" + (j + 1));
                        string dd = listHisCare[j].CARE_DESCRIPTION;
                        if (piDescription != null)
                            piDescription.SetValue(_careDescriptionPDO, dd);
                    }
                    this._careDescription.Add(_careDescriptionPDO);
                    Mps000151PDO.InstructionDescription _instruction = new Mps000151PDO.InstructionDescription();
                    _instruction.INSTRUCTION_DESCRIPTION = "Y lệnh";
                    _instruction.PARENT_ID = id;

                    for (int j = 0; j < listHisCare.Count; j++)
                    {
                        System.Reflection.PropertyInfo piDescription = typeof(Mps000151PDO.InstructionDescription).GetProperty("INSTRUCTION_DESCRIPTION_" + (j + 1));
                        string dd = listHisCare[j].INSTRUCTION_DESCRIPTION;
                        if (piDescription != null)
                            piDescription.SetValue(_instruction, dd);
                    }
                    this._instructionDescription.Add(_instruction);

                    for (int i = 0; i < lstCareViewPrintADO.Count; i++)
                    {
                        #region -----
                        switch (i)
                        {
                            case 0:
                                for (int j = 0; j < listHisCare.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], Inventec.Common.DateTime.Convert.TimeNumberToDateString(listHisCare[j].EXECUTE_TIME ?? 0));
                                }
                                break;
                            case 1:
                                for (int j = 0; j < listHisCare.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(listHisCare[j].EXECUTE_TIME ?? 0) == null ? "" : Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(listHisCare[j].EXECUTE_TIME ?? 0).Value.ToString("HH:mm"));
                                }
                                break;
                            case 2:
                                for (int j = 0; j < listHisCare.Count; j++)
                                {
                                    //var awarenest = lstHisAwareness.FirstOrDefault(o => o.ID == listHisCare[j].AWARENESS_ID);
                                    //if (awarenest != null)
                                    //{
                                    System.Reflection.PropertyInfo pi = typeof(CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], listHisCare[j].AWARENESS);
                                    //}
                                }
                                break;
                            case 3:
                                for (int j = 0; j < listHisCare.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], listHisCare[j].MUCOCUTANEOUS);//da\
                                }
                                break;
                            case 4:
                                for (int j = 0; j < listHisCare.Count; j++)
                                {
                                    if (listHisCare[j].HIS_DHST != null && listHisCare[j].HIS_DHST.PULSE.HasValue)
                                    {
                                        System.Reflection.PropertyInfo pi = typeof(CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                        pi.SetValue(lstCareViewPrintADO[i], listHisCare[j].HIS_DHST.PULSE.ToString());//mach 
                                    }
                                }
                                break;
                            case 5:
                                for (int j = 0; j < listHisCare.Count; j++)
                                {
                                    if (listHisCare[j].HIS_DHST != null && listHisCare[j].HIS_DHST.TEMPERATURE.HasValue)
                                    {
                                        System.Reflection.PropertyInfo pi = typeof(CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                        pi.SetValue(lstCareViewPrintADO[i], listHisCare[j].HIS_DHST.TEMPERATURE.ToString());//nhiet do
                                    }
                                }
                                break;
                            case 6:
                                for (int j = 0; j < listHisCare.Count; j++)
                                {
                                    if (listHisCare[j].HIS_DHST != null)
                                    {
                                        System.Reflection.PropertyInfo pi = typeof(CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                        string strBloodPressure = "";
                                        strBloodPressure += (listHisCare[j].HIS_DHST.BLOOD_PRESSURE_MAX.HasValue ? (listHisCare[j].HIS_DHST.BLOOD_PRESSURE_MAX).ToString() : "");
                                        strBloodPressure += (listHisCare[j].HIS_DHST.BLOOD_PRESSURE_MIN.HasValue ? "/" + (listHisCare[j].HIS_DHST.BLOOD_PRESSURE_MIN).ToString() : "");
                                        pi.SetValue(lstCareViewPrintADO[i], strBloodPressure);//huyet ap
                                    }
                                }
                                break;
                            case 7:
                                for (int j = 0; j < listHisCare.Count; j++)
                                {
                                    if (listHisCare[j].HIS_DHST != null && listHisCare[j].HIS_DHST.BREATH_RATE.HasValue)
                                    {
                                        System.Reflection.PropertyInfo pi = typeof(CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                        pi.SetValue(lstCareViewPrintADO[i], listHisCare[j].HIS_DHST.BREATH_RATE.ToString());//nhip tho
                                    }
                                }
                                break;
                            case 8:
                                for (int j = 0; j < listHisCare.Count; j++)
                                {
                                    if (listHisCare[j].HIS_DHST != null && listHisCare[j].HIS_DHST.SPO2.HasValue)
                                    {
                                        System.Reflection.PropertyInfo pi = typeof(CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                        pi.SetValue(lstCareViewPrintADO[i], listHisCare[j].HIS_DHST.SPO2.ToString());//SPO2
                                    }
                                }
                                break;
                            case 9:
                                for (int j = 0; j < listHisCare.Count; j++)
                                {
                                    if (listHisCare[j].HIS_DHST != null)
                                    {
                                        System.Reflection.PropertyInfo pi = typeof(CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                        pi.SetValue(lstCareViewPrintADO[i], listHisCare[j].HIS_DHST.NOTE);//KHAC
                                    }
                                }
                                break;
                            case 10:
                                for (int j = 0; j < listHisCare.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], listHisCare[j].URINE);//Nước tiểu (ml)
                                }
                                break;
                            case 11:
                                for (int j = 0; j < listHisCare.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], listHisCare[j].DEJECTA);//Phân (g) (ml)
                                }
                                break;
                            case 12:
                                for (int j = 0; j < listHisCare.Count; j++)//Cân nặng
                                {
                                    if (listHisCare[j].HIS_DHST != null && listHisCare[j].HIS_DHST.WEIGHT.HasValue)
                                    {
                                        System.Reflection.PropertyInfo pi = typeof(CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                        var weight = listHisCare[j].HIS_DHST.WEIGHT.ToString();
                                        pi.SetValue(lstCareViewPrintADO[i], weight.Trim() == "0" ? "" : weight);
                                    }
                                }

                                //for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                //{
                                //    System.Reflection.PropertyInfo pi = typeof(EXE.SDO.CareViewPrintSDO).GetProperty("CARE_" + (j + 1));
                                //    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].WEIGHT.ToString());//can nang
                                //}
                                break;
                            case 13:
                                for (int j = 0; j < listHisCare.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], listHisCare[j].HAS_MEDICINE == 1 ? "X" : "");//Thuốc thường quy
                                }
                                break;
                            case 14:
                                for (int j = 0; j < listHisCare.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], listHisCare[j].HAS_ADD_MEDICINE == 1 ? "X" : "");//Thuốc bổ sung
                                }
                                break;
                            case 15:
                                for (int j = 0; j < listHisCare.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], listHisCare[j].HAS_TEST == 1 ? "X" : "");//Xét nghiệm
                                }
                                break;
                            case 16:
                                for (int j = 0; j < listHisCare.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], listHisCare[j].NUTRITION);//Chế độ ăn
                                }
                                break;
                            case 17:
                                for (int j = 0; j < listHisCare.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], listHisCare[j].SANITARY);//Vệ sinh/thay quần áo-ga
                                }
                                break;
                            case 18:
                                for (int j = 0; j < listHisCare.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], listHisCare[j].TUTORIAL);//HD nội quy
                                }
                                break;
                            case 19:
                                for (int j = 0; j < listHisCare.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], listHisCare[j].EDUCATION);//Giáo dục sức khỏe
                                }
                                break;
                            case 20:
                                for (int j = 0; j < listHisCare.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], listHisCare[j].HAS_REHABILITATION == 1 ? "X" : "");//Thực hiện PHCN
                                }
                                break;
                            default:
                                break;
                        }
                        #endregion
                    }

                    CreatorADO creator = new CreatorADO();
                    for (int k = 0; k < listHisCare.Count; k++)
                    {
                        #region -----
                        System.Reflection.PropertyInfo piCreator = typeof(CreatorADO).GetProperty("CREATOR_" + (k + 1));
                        piCreator.SetValue(creator, listHisCare[k].CREATOR);

                        if (rdo.ListUser != null && rdo.ListUser.Count > 0)
                        {
                            var userName = rdo.ListUser.FirstOrDefault(p => p.LOGINNAME == listHisCare[k].CREATOR).USERNAME ?? "";
                            System.Reflection.PropertyInfo piUser = typeof(CreatorADO).GetProperty("USER_NAME_" + (k + 1));
                            piUser.SetValue(creator, userName);
                        }

                        //Add Theo dõi - Chăm sóc
                        List<V_HIS_CARE_DETAIL> lstHisCareDetail = new List<V_HIS_CARE_DETAIL>();
                        if (rdo.ListCareDetail != null && rdo.ListCareDetail.Count > 0)
                        {
                            lstHisCareDetail = rdo.ListCareDetail.Where(o => o.CARE_ID == listHisCare[k].ID).ToList();
                        }

                        if (lstHisCareDetail != null && lstHisCareDetail.Count > 0)
                        {
                            var careTypeIds = lstHisCareDetail.Select(o => o.CARE_TYPE_ID).Distinct().ToArray();
                            foreach (var caty in lstHisCareDetail)
                            {
                                if (!lstCareDetailViewPrintADO.Any(o => o.CARE_TYPE_ID == caty.CARE_TYPE_ID))
                                {
                                    CareDetailViewPrintADO careDetailViewPrintSDO = new CareDetailViewPrintADO();
                                    careDetailViewPrintSDO.CARE_TYPE_ID = caty.CARE_TYPE_ID;
                                    careDetailViewPrintSDO.CARE_TITLE = "Theo dõi - Chăm sóc";
                                    careDetailViewPrintSDO.CARE_DETAIL = caty.CARE_TYPE_NAME;
                                    careDetailViewPrintSDO.PARENT_ID = id;
                                    lstCareDetailViewPrintADO.Add(careDetailViewPrintSDO);
                                }
                            }


                            foreach (var item in lstCareDetailViewPrintADO)
                            {
                                var careDetailForOnes = lstHisCareDetail.Where(o => o.CARE_TYPE_ID == item.CARE_TYPE_ID).ToList();
                                if (careDetailForOnes != null && careDetailForOnes.Count > 0)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(CareDetailViewPrintADO).GetProperty("CARE_DETAIL_" + (k + 1));
                                    pi.SetValue(item, careDetailForOnes[0].CONTENT);
                                }
                            }
                        }
                        #endregion
                    }
                    creator.PARENT_ID = id;
                    _Creator.Add(creator);

                    int countCaTyPrint = 6 - lstCareDetailViewPrintADO.Count;
                    if (countCaTyPrint > 0)
                    {
                        for (int i = 0; i < countCaTyPrint; i++)
                        {
                            CareDetailViewPrintADO careDetailViewPrintSDO = new CareDetailViewPrintADO();
                            careDetailViewPrintSDO.CARE_TITLE = "Theo dõi - Chăm sóc";
                            careDetailViewPrintSDO.PARENT_ID = id;
                            lstCareDetailViewPrintADO.Add(careDetailViewPrintSDO);
                        }
                    }

                    this.ListCareDetailViewADO.AddRange(lstCareDetailViewPrintADO);
                    this.ListCareViewADO.AddRange(lstCareViewPrintADO);
                    this.ListCreator.AddRange(_Creator);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessSingleKey()
        {
            try
            {
                //AddObjectKeyIntoListkey<V_HIS_CARE_SUM>(rdo.currentSumCare,  false);
                AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo.Patient);
                AddObjectKeyIntoListkey<MPS.Processor.Mps000151.PDO.Mps000151PDO.Mps000151ADO>(rdo.mps000151ADO, false);

                if (rdo.Patient != null)
                {
                    SetSingleKey((new KeyValue(Mps000151ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo.Patient.DOB))));
                    SetSingleKey((new KeyValue(Mps000151ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.Patient.DOB))));
                    SetSingleKey((new KeyValue(Mps000151ExtendSingleKey.D_O_B, rdo.Patient.DOB.ToString().Substring(0, 4))));
                    SetSingleKey((new KeyValue(Mps000151ExtendSingleKey.GENDER_MALE, rdo.Patient.GENDER_CODE == rdo.genderCode__Male ? "X" : "")));
                    SetSingleKey((new KeyValue(Mps000151ExtendSingleKey.GENDER_FEMALE, rdo.Patient.GENDER_CODE == rdo.genderCode__FeMale ? "X" : "")));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.Patient.PATIENT_CODE);
                barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodePatientCode.IncludeLabel = false;
                barcodePatientCode.Width = 120;
                barcodePatientCode.Height = 40;
                barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodePatientCode.IncludeLabel = true;

                dicImage.Add(Mps000151ExtendSingleKey.BARCODE_PATIENT_CODE_STR, barcodePatientCode);

                //Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.departmentTran.TREATMENT_CODE);
                //barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                //barcodeTreatment.IncludeLabel = false;
                //barcodeTreatment.Width = 120;
                //barcodeTreatment.Height = 40;
                //barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                //barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                //barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                //barcodeTreatment.IncludeLabel = true;

                //dicImage.Add(Mps000151ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        class CustomerFuncMergeSameData : TFlexCelUserFunction
        {
            List<CareViewPrintADO> ListCares;
            int SameType;
            public CustomerFuncMergeSameData(List<CareViewPrintADO> listCare, int sameType)
            {
                ListCares = listCare;
                SameType = sameType;
            }
            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length <= 0)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");

                bool result = false;
                try
                {
                    int rowIndex = (int)parameters[0];
                    if (rowIndex >= 0)
                    {
                        switch (SameType)
                        {
                            //case 1:
                            //    if (rowIndex == 0 || rowIndex == 1 || rowIndex == 2 || rowIndex == 3 || rowIndex == 8 || rowIndex == 9 || rowIndex == 10 || rowIndex == 15 || rowIndex == 16 || rowIndex == 17)
                            //    {
                            //        result = true;
                            //    }
                            //    else
                            //    {
                            //        result = false;
                            //    }
                            //    break;
                            //case 2:
                            //    if (rowIndex == 5 || rowIndex == 6 || rowIndex == 7 || rowIndex == 12 || rowIndex == 13 || rowIndex == 14)
                            //    {
                            //        result = true;
                            //    }
                            //    else
                            //    {
                            //        result = false;
                            //    }
                            //    break;

                            case 1:
                                if (rowIndex == 0 || rowIndex == 1 || rowIndex == 2 || rowIndex == 3 || rowIndex == 10 || rowIndex == 11 || rowIndex == 12 || rowIndex == 17 || rowIndex == 18 || rowIndex == 19)
                                {
                                    result = true;
                                }
                                else
                                {
                                    result = false;
                                }
                                break;
                            case 2:
                                if (rowIndex == 5 || rowIndex == 6 || rowIndex == 7 || rowIndex == 8 || rowIndex == 9 || rowIndex == 14 || rowIndex == 15 || rowIndex == 16)
                                {
                                    result = true;
                                }
                                else
                                {
                                    result = false;
                                }
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {

                }

                return result;
            }
        }
    }
}
