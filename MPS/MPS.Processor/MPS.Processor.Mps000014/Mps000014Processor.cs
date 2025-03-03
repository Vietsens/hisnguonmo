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
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.Processor.Mps000014.PDO;
using MPS.ProcessorBase.Core;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000014
{
    public class Mps000014Processor : AbstractProcessor
    {
        Mps000014PDO rdo;
        public Mps000014Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000014PDO)rdoBase;
        }
        NumberStyles style = NumberStyles.Any;

        public void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo._Treatment.TDL_PATIENT_CODE);
                barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodePatientCode.IncludeLabel = false;
                barcodePatientCode.Width = 120;
                barcodePatientCode.Height = 40;
                barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodePatientCode.IncludeLabel = true;

                dicImage.Add(Mps000014ExtendSingleKey.BARCODE_PATIENT_CODE_STR, barcodePatientCode);

                Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo._Treatment.TREATMENT_CODE);
                barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatment.IncludeLabel = false;
                barcodeTreatment.Width = 120;
                barcodeTreatment.Height = 40;
                barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatment.IncludeLabel = true;

                dicImage.Add(Mps000014ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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
                List<Mps000014PDO.TestSereServADO> ExesereServs = new List<Mps000014PDO.TestSereServADO>();

                List<Mps000014PDO.TestSereServADO> parents = new List<Mps000014PDO.TestSereServADO>();
                List<Mps000014PDO.TestSereServADO> childrens = new List<Mps000014PDO.TestSereServADO>();

                if (rdo._SereServNumOder != null && rdo._SereServNumOder.Count > 0)
                {
                    ExesereServs = this.ProcessByService();
                    this.ProcessByServiceParent(ref parents, ref childrens);
                }

                GetTestIndexRanges(ExesereServs);
                GetTestIndexRanges(childrens);

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();

                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                SetBarcodeKey();
                SetSingleKey();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "ServiceTest", ExesereServs);
                objectTag.AddObjectData(store, "ServiceParentTest", parents);
                objectTag.AddObjectData(store, "IndexTest", childrens);

                objectTag.AddRelationship(store, "ServiceParentTest", "IndexTest", "RelationshipId", "RelationshipId");

                result = true;
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("rdo._ServiceReq_--:  :    " + Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo._ServiceReq), rdo._ServiceReq));
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("rdo.hisdhst_--:  " + Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo.hisdhst), rdo.hisdhst));

            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private List<Mps000014PDO.TestSereServADO> ProcessByService()
        {
            List<Mps000014PDO.TestSereServADO> rs = new List<Mps000014PDO.TestSereServADO>();
            try
            {
                rdo._SereServNumOder = rdo._SereServNumOder.OrderByDescending(p => p.SERVICE_NUM_ODER).ThenBy(p => p.TDL_SERVICE_NAME).ToList();
                int dem = 1;
                foreach (var item in rdo._SereServNumOder)
                {
                    Mps000014PDO.TestSereServADO exeSereServ = new Mps000014PDO.TestSereServADO();
                    var testSS = rdo._SereServTeins.Where(p => p.SERE_SERV_ID == item.ID).ToList();
                    if (testSS != null && testSS.Count == 1 && testSS[0].IS_NOT_SHOW_SERVICE == 1)
                    {
                        exeSereServ.SERVICE_NAME = dem + ". " + item.TDL_SERVICE_NAME;
                        exeSereServ.TEST_INDEX_RANGE = "";
                        exeSereServ.TEST_INDEX_UNIT_NAME = testSS.First().TEST_INDEX_UNIT_NAME;
                        exeSereServ.TEST_INDEX_ID = testSS.First().TEST_INDEX_ID ?? 0;
                        exeSereServ.VALUE = testSS.First().VALUE;
                        exeSereServ.LEAVEN = testSS.First().LEAVEN;
                        exeSereServ.RESULT_CODE = testSS.First().RESULT_CODE;
                        exeSereServ.MACHINE_CODE = testSS.First().MACHINE_CODE;
                        exeSereServ.MACHINE_NAME = testSS.First().MACHINE_NAME;
                        exeSereServ.MACHINE_GROUP_CODE = testSS.First().MACHINE_GROUP_CODE;

                        exeSereServ.PROCESS_CODE = rdo.ProcessCode.Where(o => o.ID == item.SERVICE_ID).First().PROCESS_CODE;

                        if (testSS.First().IS_IMPORTANT == 1)
                        {
                            exeSereServ.IS_IMPORTANT = "X";
                        }
                        rs.Add(exeSereServ);
                    }
                    else if (testSS != null && (testSS.Count > 1 || (testSS.Count == 1 && testSS[0].IS_NOT_SHOW_SERVICE != 1)))
                    {
                        exeSereServ.SERVICE_NAME = dem + ". " + item.TDL_SERVICE_NAME;
                        exeSereServ.TEST_INDEX_RANGE = "";
                        exeSereServ.TEST_INDEX_UNIT_NAME = "";
                        exeSereServ.VALUE = "";

                        exeSereServ.PROCESS_CODE = "";

                        rs.Add(exeSereServ);
                        if (rdo._SereServTeins != null && rdo._SereServTeins.Count > 0)
                        {
                            rdo._SereServTeins = rdo._SereServTeins.OrderByDescending(o => o.NUM_ORDER ?? 0)
                    .ThenBy(p => p.TEST_INDEX_NAME).ToList();
                            List<Mps000014PDO.TestSereServADO> listSS = new List<Mps000014PDO.TestSereServADO>();
                            foreach (var sereServTein in rdo._SereServTeins)
                            {
                                if (sereServTein.SERE_SERV_ID == item.ID)
                                {
                                    Mps000014PDO.TestSereServADO exeSereServTein = new Mps000014PDO.TestSereServADO();
                                    exeSereServTein.SERVICE_NAME = "    " + sereServTein.TEST_INDEX_NAME;
                                    exeSereServTein.TEST_INDEX_UNIT_NAME = sereServTein.TEST_INDEX_UNIT_NAME;
                                    exeSereServTein.VALUE = sereServTein.VALUE;
                                    exeSereServTein.LEAVEN = sereServTein.LEAVEN;
                                    exeSereServTein.TEST_INDEX_ID = sereServTein.TEST_INDEX_ID ?? 0;
                                    exeSereServTein.RESULT_CODE = sereServTein.RESULT_CODE;
                                    exeSereServTein.MACHINE_CODE = sereServTein.MACHINE_CODE;
                                    exeSereServTein.MACHINE_NAME = sereServTein.MACHINE_NAME;
                                    exeSereServTein.MACHINE_GROUP_CODE = sereServTein.MACHINE_GROUP_CODE;
                                    exeSereServTein.NOTE = sereServTein.NOTE;
                                    exeSereServTein.PROCESS_CODE = rdo.ProcessCode.Where(o => o.ID == item.SERVICE_ID).First().PROCESS_CODE;

                                    if (sereServTein.IS_IMPORTANT == 1)
                                    {
                                        exeSereServTein.IS_IMPORTANT = "X";
                                    }
                                    rs.Add(exeSereServTein);
                                }
                            }
                        }
                    }
                    dem++;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                rs = new List<Mps000014PDO.TestSereServADO>();
            }
            return rs;
        }

        private void ProcessByServiceParent(ref List<Mps000014PDO.TestSereServADO> parents, ref List<Mps000014PDO.TestSereServADO> childrens)
        {
            try
            {
                rdo._SereServNumOder = rdo._SereServNumOder.OrderByDescending(o => o.ServiceParentOrder).ThenByDescending(o => o.ServiceParentName).ThenByDescending(p => p.SERVICE_NUM_ODER).ThenBy(p => p.TDL_SERVICE_NAME).ToList();
                long printNumOrder = 1;
                var Groups = rdo._SereServNumOder.GroupBy(g => (g.ServiceParentId ?? 0)).ToList();
                List<string> parentNames = new List<string>();
                foreach (var group in Groups)
                {
                    Mps000014PDO.TestSereServADO parent = new Mps000014PDO.TestSereServADO();
                    parent.RelationshipId = group.Key.ToString();
                    if (group.Key == 0)
                    {
                        parent.SERVICE_NAME = "Khác";
                    }
                    else
                    {
                        parent.SERVICE_NAME = group.FirstOrDefault().ServiceParentName;
                    }
                    parents.Add(parent);
                    parentNames.Add(parent.SERVICE_NAME);
                    List<Mps000014PDO.TestSereServADO> childrensInParents = new List<Mps000014PDO.TestSereServADO>();
                    foreach (SereServNumOder child in group.ToList())
                    {
                        List<V_HIS_SERE_SERV_TEIN> testSS = rdo._SereServTeins.Where(p => p.SERE_SERV_ID == child.ID).ToList();
                        if (testSS != null && testSS.Count > 0)
                        {
                            foreach (var sereServTein in testSS)
                            {
                                Mps000014PDO.TestSereServADO exeSereServTein = new Mps000014PDO.TestSereServADO();
                                exeSereServTein.SERVICE_NAME = sereServTein.TEST_INDEX_NAME;
                                exeSereServTein.TEST_INDEX_UNIT_NAME = sereServTein.TEST_INDEX_UNIT_NAME;
                                exeSereServTein.VALUE = sereServTein.VALUE;
                                exeSereServTein.LEAVEN = sereServTein.LEAVEN;
                                exeSereServTein.TEST_INDEX_ID = sereServTein.TEST_INDEX_ID ?? 0;
                                exeSereServTein.RESULT_CODE = sereServTein.RESULT_CODE;
                                exeSereServTein.MACHINE_CODE = sereServTein.MACHINE_CODE;
                                exeSereServTein.MACHINE_NAME = sereServTein.MACHINE_NAME;
                                exeSereServTein.MACHINE_GROUP_CODE = sereServTein.MACHINE_GROUP_CODE;
                                exeSereServTein.NOTE = sereServTein.NOTE;
                                exeSereServTein.RelationshipId = parent.RelationshipId;
                                exeSereServTein.PRINT_NUM_ORDER = printNumOrder;
                                if (sereServTein.IS_IMPORTANT == 1)
                                {
                                    exeSereServTein.IS_IMPORTANT = "X";
                                }
                                exeSereServTein.TEST_INDEX_GROUP_ID = sereServTein.TEST_INDEX_GROUP_ID;
                                exeSereServTein.TEST_INDEX_GROUP_CODE = sereServTein.TEST_INDEX_GROUP_CODE;
                                exeSereServTein.TEST_INDEX_GROUP_NAME = sereServTein.TEST_INDEX_GROUP_NAME;

                                exeSereServTein.PROCESS_CODE = rdo.ProcessCode.Where(o => o.ID == child.SERVICE_ID).First().PROCESS_CODE;

                                childrensInParents.Add(exeSereServTein);
                                childrens.Add(exeSereServTein);
                                printNumOrder++;
                            }
                        }
                    }

                    if (childrensInParents.Any(a => a.TEST_INDEX_GROUP_ID.HasValue))
                    {
                        bool isHoldParent = false;
                        childrensInParents = childrensInParents.OrderBy(o => o.TEST_INDEX_GROUP_CODE).ToList();
                        var childGroups = childrensInParents.GroupBy(g => g.TEST_INDEX_GROUP_ID ?? 0);
                        foreach (var cGroup in childGroups)
                        {
                            List<Mps000014PDO.TestSereServADO> lstChild = cGroup.ToList();
                            if (cGroup.Key == 0)
                            {
                                isHoldParent = true;
                                continue;
                            }
                            else
                            {
                                Mps000014PDO.TestSereServADO cParent = new Mps000014PDO.TestSereServADO();
                                cParent.RelationshipId = "TEST_INDEX_GROUP_" + cGroup.Key.ToString();
                                cParent.SERVICE_NAME = lstChild.FirstOrDefault().TEST_INDEX_GROUP_NAME;
                                parents.Add(cParent);
                                lstChild.ForEach(o => o.RelationshipId = cParent.RelationshipId);
                            }
                        }
                        if (!isHoldParent)
                        {
                            parents.Remove(parent);
                        }
                    }
                }

                SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.SERVICE_PARENT_NAMES, String.Join(" - ", parentNames)));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessMaxMixValue(Mps000014PDO.TestSereServADO ti, V_HIS_TEST_INDEX_RANGE testIndexRange)
        {
            try
            {
                Decimal minValue = 0, maxValue = 0, value = 0, warMin = 0, warMax = 0;
                if (ti != null && testIndexRange != null)
                {
                    if (!String.IsNullOrWhiteSpace(testIndexRange.MIN_VALUE))
                    {
                        if (Decimal.TryParse((testIndexRange.MIN_VALUE ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out minValue))
                        {
                            ti.MIN_VALUE = minValue;
                        }
                        else
                        {
                            ti.MIN_VALUE = null;
                        }
                    }
                    if (!String.IsNullOrWhiteSpace(testIndexRange.MAX_VALUE))
                    {
                        if (Decimal.TryParse((testIndexRange.MAX_VALUE ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out maxValue))
                        {
                            ti.MAX_VALUE = maxValue;
                        }
                        else
                        {
                            ti.MAX_VALUE = null;
                        }
                    }
                    if (!String.IsNullOrWhiteSpace(testIndexRange.WARNING_MIN_VALUE))
                    {
                        if (Decimal.TryParse((testIndexRange.WARNING_MIN_VALUE ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out warMin))
                        {
                            ti.WARNING_MIN_VALUE = warMin;
                        }
                        else
                        {
                            ti.WARNING_MIN_VALUE = null;
                        }
                    }
                    if (!String.IsNullOrWhiteSpace(testIndexRange.WARNING_MAX_VALUE))
                    {
                        if (Decimal.TryParse((testIndexRange.WARNING_MAX_VALUE ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out warMax))
                        {
                            ti.WARNING_MAX_VALUE = warMax;
                        }
                        else
                        {
                            ti.WARNING_MAX_VALUE = null;
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(ti.VALUE))
                    {
                        if (Decimal.TryParse((ti.VALUE ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out value))
                        {
                            ti.VALUE_NUM = value;
                        }
                        else
                        {
                            ti.VALUE_NUM = null;
                        }
                    }

                    ti.VALUE_HL = ti.VALUE + "";

                    this.ProcessHighLowValue(ti, testIndexRange);
                    this.ProcessHighLowWarningValue(ti, testIndexRange);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessHighLowValue(Mps000014PDO.TestSereServADO ti, V_HIS_TEST_INDEX_RANGE testIndexRange)
        {
            if (!String.IsNullOrEmpty(testIndexRange.NORMAL_VALUE))
            {
                ti.VALUE_RANGE = testIndexRange.NORMAL_VALUE;
                if (!String.IsNullOrWhiteSpace(ti.VALUE_RANGE) && ti.VALUE != null && ti.VALUE.ToString().ToUpper() == ti.VALUE_RANGE.ToUpper())
                {
                    ti.HIGH_OR_LOW = "";
                }
                else
                {
                    ti.HIGH_OR_LOW = " ";
                }
            }
            else
            {
                ti.VALUE_RANGE = "";

                if (testIndexRange.IS_ACCEPT_EQUAL_MIN == 1 && testIndexRange.IS_ACCEPT_EQUAL_MAX == null)
                {
                    if (testIndexRange.MIN_VALUE != null)
                    {
                        ti.VALUE_RANGE += testIndexRange.MIN_VALUE + "<= ";
                    }

                    ti.VALUE_RANGE += "X";

                    if (testIndexRange.MAX_VALUE != null)
                    {
                        ti.VALUE_RANGE += " < " + testIndexRange.MAX_VALUE;
                    }

                    if (ti.VALUE_NUM != null && ti.MIN_VALUE != null && ti.MIN_VALUE <= ti.VALUE_NUM && ti.MAX_VALUE != null && ti.VALUE_NUM < ti.MAX_VALUE)
                    {
                        ti.HIGH_OR_LOW = "";
                        ti.VALUE_HL = ti.VALUE + "";
                    }
                    else if (ti.VALUE_NUM != null && ti.MIN_VALUE != null && ti.VALUE_NUM < ti.MIN_VALUE)
                    {
                        ti.HIGH_OR_LOW = "L";
                        ti.VALUE_HL = ti.VALUE + "L";
                    }
                    else if (ti.VALUE_NUM != null && ti.MAX_VALUE != null && ti.MAX_VALUE <= ti.VALUE_NUM)
                    {
                        ti.HIGH_OR_LOW = "H";
                        ti.VALUE_HL = ti.VALUE + "H";
                    }
                    else
                    {
                        ti.HIGH_OR_LOW = "";
                        ti.VALUE_HL = ti.VALUE + "";
                    }
                }
                else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == 1 && testIndexRange.IS_ACCEPT_EQUAL_MAX == 1)
                {
                    if (testIndexRange.MIN_VALUE != null)
                    {
                        ti.VALUE_RANGE += testIndexRange.MIN_VALUE + "<= ";
                    }

                    ti.VALUE_RANGE += "X";

                    if (testIndexRange.MAX_VALUE != null)
                    {
                        ti.VALUE_RANGE += " <= " + testIndexRange.MAX_VALUE;
                    }

                    if (ti.VALUE_NUM != null && ti.MIN_VALUE != null && ti.MIN_VALUE <= ti.VALUE_NUM && ti.MAX_VALUE != null && ti.VALUE_NUM <= ti.MAX_VALUE)
                    {
                        ti.HIGH_OR_LOW = "";
                        ti.VALUE_HL = ti.VALUE + "";
                    }
                    else if (ti.VALUE_NUM != null && ti.MIN_VALUE != null && ti.VALUE_NUM < ti.MIN_VALUE)
                    {
                        ti.HIGH_OR_LOW = "L";
                        ti.VALUE_HL = ti.VALUE + "L";
                    }
                    else if (ti.VALUE_NUM != null && ti.MAX_VALUE != null && ti.MAX_VALUE < ti.VALUE_NUM)
                    {
                        ti.HIGH_OR_LOW = "H";
                        ti.VALUE_HL = ti.VALUE + "H";
                    }
                    else
                    {
                        ti.HIGH_OR_LOW = "";
                        ti.VALUE_HL = ti.VALUE_RANGE + "";
                    }
                }
                else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == null && testIndexRange.IS_ACCEPT_EQUAL_MAX == 1)
                {
                    if (testIndexRange.MIN_VALUE != null)
                    {
                        ti.VALUE_RANGE += testIndexRange.MIN_VALUE + "< ";
                    }

                    ti.VALUE_RANGE += "X";

                    if (testIndexRange.MAX_VALUE != null)
                    {
                        ti.VALUE_RANGE += " <= " + testIndexRange.MAX_VALUE;
                    }

                    if (ti.VALUE_NUM != null && ti.MIN_VALUE != null && ti.MIN_VALUE < ti.VALUE_NUM && ti.MAX_VALUE != null && ti.VALUE_NUM <= ti.MAX_VALUE)
                    {
                        ti.HIGH_OR_LOW = "";
                        ti.VALUE_HL = ti.VALUE + "";
                    }
                    else if (ti.VALUE_NUM != null && ti.MIN_VALUE != null && ti.VALUE_NUM < ti.MIN_VALUE)
                    {
                        ti.HIGH_OR_LOW = "L";
                        ti.VALUE_HL = ti.VALUE + "L";
                    }
                    else if (ti.VALUE_NUM != null && ti.MAX_VALUE != null && ti.MAX_VALUE < ti.VALUE_NUM)
                    {
                        ti.HIGH_OR_LOW = "H";
                        ti.VALUE_HL = ti.VALUE + "H";
                    }
                    else
                    {
                        ti.HIGH_OR_LOW = "";
                        ti.VALUE_HL = ti.VALUE_RANGE + "";
                    }
                }
                else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == null && testIndexRange.IS_ACCEPT_EQUAL_MAX == null)
                {
                    if (testIndexRange.MIN_VALUE != null)
                    {
                        ti.VALUE_RANGE += testIndexRange.MIN_VALUE + "< ";
                    }

                    ti.VALUE_RANGE += "X";

                    if (testIndexRange.MAX_VALUE != null)
                    {
                        ti.VALUE_RANGE += " < " + testIndexRange.MAX_VALUE;
                    }

                    if (ti.VALUE_NUM != null && ti.MIN_VALUE != null && ti.MIN_VALUE < ti.VALUE_NUM && ti.MAX_VALUE != null && ti.VALUE_NUM < ti.MAX_VALUE)
                    {
                        ti.HIGH_OR_LOW = "";
                        ti.VALUE_HL = ti.VALUE + "";
                    }
                    else if (ti.VALUE_NUM != null && ti.MIN_VALUE != null && ti.VALUE_NUM <= ti.MIN_VALUE)
                    {
                        ti.HIGH_OR_LOW = "L";
                        ti.VALUE_HL = ti.VALUE + "L";
                    }
                    else if (ti.VALUE_NUM != null && ti.MAX_VALUE != null && ti.MAX_VALUE <= ti.VALUE_NUM)
                    {
                        ti.HIGH_OR_LOW = "H";
                        ti.VALUE_HL = ti.VALUE + "H";
                    }
                    else
                    {
                        ti.HIGH_OR_LOW = "";
                        ti.VALUE_HL = ti.VALUE_RANGE + "";
                    }
                }
            }
        }

        private void ProcessHighLowWarningValue(Mps000014PDO.TestSereServADO ti, V_HIS_TEST_INDEX_RANGE testIndexRange)
        {
            ti.WARNING_DESCRIPTION = "";

            if (testIndexRange.IS_ACCEPT_EQUAL_WARNING_MIN == 1 && testIndexRange.IS_ACCEPT_EQUAL_WARNING_MAX == null)
            {
                if (testIndexRange.WARNING_MIN_VALUE != null)
                {
                    ti.WARNING_DESCRIPTION += testIndexRange.WARNING_MIN_VALUE + "<= ";
                }

                //ti.WARNING_DESCRIPTION += " ";

                if (testIndexRange.WARNING_MAX_VALUE != null)
                {
                    ti.WARNING_DESCRIPTION += " < " + testIndexRange.WARNING_MAX_VALUE;
                }

                if (ti.VALUE_NUM != null && ti.WARNING_MIN_VALUE != null && ti.VALUE_NUM < ti.WARNING_MIN_VALUE)
                {
                    ti.WARNING_NOTE = "Báo động";
                }
                else if (ti.VALUE_NUM != null && ti.WARNING_MAX_VALUE != null && ti.WARNING_MAX_VALUE <= ti.VALUE_NUM)
                {
                    ti.WARNING_NOTE = "Báo động";
                }
            }
            else if (testIndexRange.IS_ACCEPT_EQUAL_WARNING_MIN == 1 && testIndexRange.IS_ACCEPT_EQUAL_WARNING_MAX == 1)
            {
                if (testIndexRange.WARNING_MIN_VALUE != null)
                {
                    ti.WARNING_DESCRIPTION += testIndexRange.WARNING_MIN_VALUE + "<= ";
                }

                //ti.WARNING_DESCRIPTION += " ";

                if (testIndexRange.WARNING_MAX_VALUE != null)
                {
                    ti.WARNING_DESCRIPTION += " <= " + testIndexRange.WARNING_MAX_VALUE;
                }

                if (ti.VALUE_NUM != null && ti.WARNING_MIN_VALUE != null && ti.VALUE_NUM < ti.WARNING_MIN_VALUE)
                {
                    ti.WARNING_NOTE = "Báo động";
                }
                else if (ti.VALUE_NUM != null && ti.WARNING_MAX_VALUE != null && ti.WARNING_MAX_VALUE < ti.VALUE_NUM)
                {
                    ti.WARNING_NOTE = "Báo động";
                }
            }
            else if (testIndexRange.IS_ACCEPT_EQUAL_WARNING_MIN == null && testIndexRange.IS_ACCEPT_EQUAL_WARNING_MAX == 1)
            {
                if (testIndexRange.WARNING_MIN_VALUE != null)
                {
                    ti.WARNING_DESCRIPTION += testIndexRange.WARNING_MIN_VALUE + "< ";
                }

                //ti.WARNING_DESCRIPTION += " ";

                if (testIndexRange.WARNING_MAX_VALUE != null)
                {
                    ti.WARNING_DESCRIPTION += " <= " + testIndexRange.WARNING_MAX_VALUE;
                }

                if (ti.VALUE_NUM != null && ti.WARNING_MIN_VALUE != null && ti.VALUE_NUM < ti.WARNING_MIN_VALUE)
                {
                    ti.WARNING_NOTE = "Báo động";
                }
                else if (ti.VALUE_NUM != null && ti.WARNING_MAX_VALUE != null && ti.WARNING_MAX_VALUE < ti.VALUE_NUM)
                {
                    ti.WARNING_NOTE = "Báo động";
                }
            }
            else if (testIndexRange.IS_ACCEPT_EQUAL_WARNING_MIN == null && testIndexRange.IS_ACCEPT_EQUAL_WARNING_MAX == null)
            {
                if (testIndexRange.WARNING_MIN_VALUE != null)
                {
                    ti.WARNING_DESCRIPTION += testIndexRange.WARNING_MIN_VALUE + "< ";
                }

                //ti.WARNING_DESCRIPTION += " ";

                if (testIndexRange.WARNING_MAX_VALUE != null)
                {
                    ti.WARNING_DESCRIPTION += " < " + testIndexRange.WARNING_MAX_VALUE;
                }

                if (ti.VALUE_NUM != null && ti.WARNING_MIN_VALUE != null && ti.VALUE_NUM <= ti.WARNING_MIN_VALUE)
                {
                    ti.WARNING_NOTE = "Báo động";
                }
                else if (ti.VALUE_NUM != null && ti.WARNING_MAX_VALUE != null && ti.WARNING_MAX_VALUE <= ti.VALUE_NUM)
                {
                    ti.WARNING_NOTE = "Báo động";
                }
            }
        }

        private void ProcessMaxMixValue(Mps000014PDO.TestSereServADO ti, string description)
        {
            try
            {
                if (ti != null && !String.IsNullOrWhiteSpace(description))
                {
                    string[] values = description.Split('(', ' ', '-', ')');
                    values = values != null ? values.Where(o => !String.IsNullOrWhiteSpace(o)).ToArray() : null;
                    Decimal minValue, maxValue;

                    if (values != null && values.Length > 1 && Decimal.TryParse((values[0] ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out minValue) && Decimal.TryParse((values[1] ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out maxValue))
                    {
                        ti.MAX_VALUE = maxValue;
                        ti.MIN_VALUE = minValue;
                    }
                    else
                    {
                        ti.MIN_VALUE = null;
                        ti.MAX_VALUE = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX_RANGE GetTestIndexRange(long dob, long? genderId, long testIndexId, ref List<V_HIS_TEST_INDEX_RANGE> testIndexRanges)
        {
            MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX_RANGE testIndexRange = new V_HIS_TEST_INDEX_RANGE();
            try
            {
                if (testIndexRanges != null && testIndexRanges.Count > 0)
                {
                    long? now = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                    int age = 0;

                    List<V_HIS_TEST_INDEX_RANGE> query = new List<V_HIS_TEST_INDEX_RANGE>();

                    foreach (var item in testIndexRanges)
                    {
                        if (item.TEST_INDEX_ID == testIndexId)
                        {
                            if (item.AGE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__YEAR)
                            {
                                age = Inventec.Common.DateTime.Calculation.DifferenceTime(dob, now ?? 0, Inventec.Common.DateTime.Calculation.UnitDifferenceTime.DAY) / 365;
                            }
                            else if (item.AGE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__MONTH)
                            {
                                age = Inventec.Common.DateTime.Calculation.DifferenceMonth(dob, now ?? 0);
                            }
                            else if (item.AGE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__DAY)
                            {
                                age = Inventec.Common.DateTime.Calculation.DifferenceDate(dob, now ?? 0);
                            }
                            else if (item.AGE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__HOUR)
                            {
                                age = Inventec.Common.DateTime.Calculation.DifferenceTime(dob, now ?? 0, Inventec.Common.DateTime.Calculation.UnitDifferenceTime.HOUR);
                            }

                            if (((item.AGE_FROM.HasValue && item.AGE_FROM.Value <= age) || !item.AGE_FROM.HasValue)
                            && ((item.AGE_TO.HasValue && item.AGE_TO.Value >= age) || !item.AGE_TO.HasValue))
                            {
                                query.Add(item);
                            }
                        }
                    }
                    if (genderId == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE)
                    {
                        query = query.Where(o => o.IS_MALE == 1).ToList();
                    }
                    else if (genderId == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
                    {
                        query = query.Where(o => o.IS_FEMALE == 1).ToList();
                    }
                    testIndexRange = query.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                testIndexRange = new V_HIS_TEST_INDEX_RANGE();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return testIndexRange;
        }

        void GetTestIndexRanges(List<Mps000014PDO.TestSereServADO> ExesereServs)
        {
            try
            {
                var testIndexRangeAll = rdo._TestIndexRangeAll;
                foreach (var hisSereServTeinSDO in ExesereServs)
                {

                    V_HIS_TEST_INDEX_RANGE testIndexRange = new V_HIS_TEST_INDEX_RANGE();
                    testIndexRange = GetTestIndexRange(rdo._ServiceReq.TDL_PATIENT_DOB, rdo.GenderId, hisSereServTeinSDO.TEST_INDEX_ID, ref testIndexRangeAll);
                    if (testIndexRange != null)
                    {
                        ProcessMaxMixValue(hisSereServTeinSDO, testIndexRange);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetSingleKey()    
        {
            try
            {
                AddObjectKeyIntoListkey<MLCTADO>(rdo.mLCTADO, false);
                if (rdo._PatyAlterBhyt != null && !String.IsNullOrWhiteSpace(rdo._PatyAlterBhyt.HEIN_CARD_NUMBER))
                {
                    SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.HEIN_CARD_NUMBER, rdo._PatyAlterBhyt.HEIN_CARD_NUMBER));

                    SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._PatyAlterBhyt.HEIN_CARD_FROM_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._PatyAlterBhyt.HEIN_CARD_TO_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.HEIN_MEDI_ORG_CODE, rdo._PatyAlterBhyt.HEIN_MEDI_ORG_CODE));

                    SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.IS_HEIN, "X"));
                    if (rdo._PatyAlterBhyt.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (rdo._PatyAlterBhyt.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                        {
                            SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (rdo._PatyAlterBhyt.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (rdo._PatyAlterBhyt.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }
                    //Dia chi the
                    SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.HEIN_CARD_ADDRESS, rdo._PatyAlterBhyt.ADDRESS));
                }
                else
                    SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.IS_NOT_HEIN, "X"));

                if (rdo._ServiceReq != null)
                {
                    SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo._ServiceReq.TDL_PATIENT_DOB)));
                    SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.INTRUCTION_TIME_FULL_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo._ServiceReq.INTRUCTION_TIME)));
                    SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.INTRUCTION_DATE_FULL_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._ServiceReq.INTRUCTION_TIME)));
                    if (rdo._ServiceReq.FINISH_TIME != null)
                    {
                        SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.FINISH_TIME_FULL_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo._ServiceReq.FINISH_TIME ?? 0)));
                        SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.FINISH_DATE_FULL_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._ServiceReq.FINISH_TIME ?? 0)));
                    }
                    else
                    {
                        SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.FINISH_TIME_FULL_STR, ""));
                        SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.FINISH_DATE_FULL_STR, ""));
                    }
                    SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.REQ_ICD_NAME, rdo._ServiceReq.ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.REQ_ICD_CODE, rdo._ServiceReq.ICD_CODE));
                    SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.REQ_ICD_SUB_CODE, rdo._ServiceReq.ICD_SUB_CODE));
                    SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.REQ_ICD_TEXT, rdo._ServiceReq.ICD_TEXT));

                }
                if (rdo._Treatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.DOB, rdo._Treatment.TDL_PATIENT_DOB));

                    HIS_MEDICINE_TYPE vaccine = GetVaccineFromVaccineId(rdo._Treatment.VACCINE_ID);
                    SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.VACCINE_CODE, vaccine != null ? vaccine.MEDICINE_TYPE_CODE : null));
                    SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.VACCINE_NAME, vaccine != null ? vaccine.MEDICINE_TYPE_NAME : null));

                }

                AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo._Treatment, false);
                AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo._PatyAlterBhyt, false);
                AddObjectKeyIntoListkey<HIS_DHST>(rdo.hisdhst, false);
                AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo._ServiceReq, false);


                if (rdo.ratio_text > 0)
                {
                    SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.RATIO, rdo.ratio_text));
                    SetSingleKey(new KeyValue(Mps000014ExtendSingleKey.RATIO_STR, (rdo.ratio_text * 100) + " %"));
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HIS_MEDICINE_TYPE GetVaccineFromVaccineId(long? vaccineId)
        {
            HIS_MEDICINE_TYPE result = null;
            try
            {
                if (vaccineId == null || vaccineId <= 0)
                    return result;
                CommonParam param = new CommonParam();
                HisMedicineTypeFilter filter = new HisMedicineTypeFilter();
                filter.ID = vaccineId;
                result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_MEDICINE_TYPE>>("api/HisMedicineType/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                if (rdo != null && rdo._ServiceReq != null)
                {
                    log += "In kết quả xét nghiệm";
                    log += string.Format(" TREATMENT_CODE:{0}", rdo._ServiceReq.TREATMENT_CODE);
                    log += string.Format(" SERVICE_REQ_CODE:{0}", rdo._ServiceReq.SERVICE_REQ_CODE);
                    log += string.Format(" . Chi tiết: {0}", this.ProcessDetailIndex());
                    log = Inventec.Common.String.CountVi.SubStringVi(log, 2000);
                }
            }
            catch (Exception ex)
            {
                log = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return log;
        }

        private string ProcessDetailIndex()
        {
            if (rdo._SereServNumOder != null && rdo._SereServTeins != null)
            {
                List<string> listDetail = new List<string>();
                foreach (var item in rdo._SereServNumOder)
                {
                    string detail = item.TDL_SERVICE_NAME + "(";
                    var testSS = rdo._SereServTeins.Where(p => p.SERE_SERV_ID == item.ID).ToList();
                    List<string> indexs = new List<string>();
                    foreach (var ssTein in testSS)
                    {
                        indexs.Add(String.Format("{0}:{1}", ssTein.TEST_INDEX_CODE ?? "", ssTein.VALUE ?? ""));
                    }
                    detail = detail + String.Join(";", indexs) + ")";
                    listDetail.Add(detail);
                }

                return String.Join(";", listDetail);
            }
            return "";
        }

        public override string ProcessUniqueCodeData()
        {
            string result = "";
            try
            {
                string parentCode = "";
                if (rdo._SereServNumOder != null && rdo._SereServNumOder.Count > 0)
                {
                    List<string> codes = rdo._SereServNumOder.Select(s => s.ServiceParentCode).Distinct().ToList();
                    if (codes != null && codes.Count > 0)
                    {
                        parentCode = string.Join(",", codes.OrderByDescending(o => o).ToList());
                    }
                }

                if (rdo != null && rdo._ServiceReq != null)
                    result = String.Format("{0} {1} {2} {3}", this.printTypeCode, string.Format("TREATMENT_CODE:{0}", rdo._ServiceReq.TREATMENT_CODE), string.Format("SERVICE_REQ_CODE:{0}", rdo._ServiceReq.SERVICE_REQ_CODE), parentCode);
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
