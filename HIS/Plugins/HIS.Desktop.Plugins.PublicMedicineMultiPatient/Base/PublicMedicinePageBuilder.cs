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
using HIS.Desktop.Plugins.PublicMedicineMultiPatient.ADO;
using MPS.Processor.Mps000522.PDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HIS.Desktop.Plugins.PublicMedicineMultiPatient.Base
{
    /// <summary>
    /// Splits the rows of one treatment into day pages (daySize days per page) and builds the print rows of each page,
    /// the same way as AddDataToDatetime of HIS.Desktop.Plugins.PublicMedicineByPhased.
    /// A page without any print row is not returned.
    /// </summary>
    internal class PublicMedicinePageBuilder
    {
        /// <summary>Number of DayN properties on Mps000522ADO and Mps000522ByMediEndMate</summary>
        private const int MAX_DAY_COLUMN = 60;
        private const int DAY_SIZE_DEFAULT = 10;

        private static readonly Dictionary<string, PropertyInfo> PageProperties = GetProperties(typeof(Mps000522ADO));
        private static readonly Dictionary<string, PropertyInfo> RowProperties = GetProperties(typeof(Mps000522ByMediEndMate));

        internal static List<Mps000522ADO> Build(List<ExpMestMediAndMateADO> rows, bool isTachHDSD, int daySize)
        {
            List<Mps000522ADO> result = new List<Mps000522ADO>();
            try
            {
                if (rows == null || rows.Count == 0)
                    return result;
                if (daySize <= 0)
                    daySize = DAY_SIZE_DEFAULT;
                if (daySize > MAX_DAY_COLUMN)
                    daySize = MAX_DAY_COLUMN;

                List<long> dates = rows.Select(o => o.INTRUCTION_DATE).Distinct().OrderBy(o => o).ToList();
                List<List<ExpMestMediAndMateADO>> groups = rows
                    .GroupBy(o => new
                    {
                        o.MEDICINE_TYPE_ID,
                        TUTORIAL = isTachHDSD ? o.TUTORIAL : "",
                        o.PRICE,
                        o.Service_Type_Id,
                        o.IS_EXPEND,
                        o.VAT_RATIO
                    })
                    .Select(o => o.ToList())
                    .ToList();

                for (int start = 0; start < dates.Count; start += daySize)
                {
                    List<long> pageDates = dates.Skip(start).Take(daySize).ToList();
                    Mps000522ADO page = new Mps000522ADO();
                    for (int j = 0; j < MAX_DAY_COLUMN; j++)
                    {
                        string header = j < pageDates.Count ? ToDayMonth(pageDates[j]) : "";
                        SetValue(PageProperties, page, "Day" + (j + 1), header);
                        SetValue(PageProperties, page, "DayAndYear" + (j + 1), header);
                    }

                    page.Mps000522ByMediEndMateADOs = new List<Mps000522ByMediEndMate>();
                    foreach (List<ExpMestMediAndMateADO> groupRows in groups)
                    {
                        List<ExpMestMediAndMateADO> rowsInPage = groupRows.Where(o => pageDates.Contains(o.INTRUCTION_DATE)).ToList();
                        decimal amountInPage = rowsInPage.Sum(o => o.AMOUNT);
                        if (amountInPage <= 0)
                            continue;

                        Mps000522ByMediEndMate printRow = MakePrintRow(groupRows.First());
                        printRow.AMOUNT = amountInPage;
                        printRow.AMOUNT_STRING = amountInPage + "";
                        for (int j = 0; j < MAX_DAY_COLUMN; j++)
                        {
                            List<ExpMestMediAndMateADO> dayRows = j < pageDates.Count
                                ? rowsInPage.Where(o => o.INTRUCTION_DATE == pageDates[j]).ToList()
                                : new List<ExpMestMediAndMateADO>();
                            bool hasDay = dayRows.Count > 0;
                            SetValue(RowProperties, printRow, "Day" + (j + 1), hasDay ? dayRows.Sum(o => o.AMOUNT) : 0m);
                            SetValue(RowProperties, printRow, "MORNING_Day" + (j + 1), hasDay ? dayRows.Sum(o => ParseDose(o.MORNING)).ToString() : "");
                            SetValue(RowProperties, printRow, "NOON_Day" + (j + 1), hasDay ? dayRows.Sum(o => ParseDose(o.NOON)).ToString() : "");
                            SetValue(RowProperties, printRow, "AFTERNOON_Day" + (j + 1), hasDay ? dayRows.Sum(o => ParseDose(o.AFTERNOON)).ToString() : "");
                            SetValue(RowProperties, printRow, "EVENING_Day" + (j + 1), hasDay ? dayRows.Sum(o => ParseDose(o.EVENING)).ToString() : "");
                        }
                        page.Mps000522ByMediEndMateADOs.Add(printRow);
                    }

                    if (page.Mps000522ByMediEndMateADOs.Count == 0)
                        continue;
                    page.Mps000522ByMediEndMateADOs = page.Mps000522ByMediEndMateADOs.OrderBy(o => o.MEDICINE_TYPE_NAME).ToList();
                    result.Add(page);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private static Mps000522ByMediEndMate MakePrintRow(ExpMestMediAndMateADO first)
        {
            Mps000522ByMediEndMate printRow = new Mps000522ByMediEndMate();
            printRow.MEDICINE_TYPE_NAME = first.MEDICINE_TYPE_NAME;
            printRow.MEDICINE_TYPE_ID = first.MEDICINE_TYPE_ID;
            printRow.PRICE = first.PRICE;
            printRow.IS_EXPEND = first.IS_EXPEND;
            printRow.Service_Type_Id = first.Service_Type_Id;
            printRow.SERVICE_ID = first.SERVICE_ID;
            printRow.SERVICE_UNIT_NAME = first.SERVICE_UNIT_NAME;
            printRow.REQ_DEPARTMENT_ID = first.REQ_DEPARTMENT_ID;
            printRow.VAT_RATIO = first.VAT_RATIO;
            printRow.MEDICINE_GROUP_NUM_ORDER = first.MEDICINE_GROUP_NUM_ORDER;
            printRow.MEDICINE_USE_FORM_NUM_ORDER = first.MEDICINE_USE_FORM_NUM_ORDER;
            printRow.NUM_ORDER = first.NUM_ORDER;
            printRow.TUTORIAL = first.TUTORIAL;
            printRow.CONCENTRA = first.CONCENTRA;
            // Only medicine rows have active ingredients (V_HIS_EXP_MEST_MEDICINE); material and blood rows stay empty
            printRow.ACTIVE_INGR_BHYT_CODE = first.ACTIVE_INGR_BHYT_CODE;
            printRow.ACTIVE_INGR_BHYT_NAME = first.ACTIVE_INGR_BHYT_NAME;
            return printRow;
        }

        /// <summary>
        /// Dose of a session (MORNING, NOON, AFTERNOON, EVENING) written as "a", "a/b", "a+b", "a-b" or "a*b"
        /// </summary>
        internal static decimal ParseDose(string dose)
        {
            decimal result = 0;
            try
            {
                if (String.IsNullOrWhiteSpace(dose))
                    return result;

                if (dose.Contains("/"))
                {
                    string[] parts = dose.Split('/');
                    result = Convert.ToDecimal(parts[0].Trim()) / Convert.ToDecimal(parts[1].Trim());
                }
                else if (dose.Contains("+"))
                {
                    string[] parts = dose.Split('+');
                    result = Convert.ToDecimal(parts[0].Trim()) + Convert.ToDecimal(parts[1].Trim());
                }
                else if (dose.Contains("-"))
                {
                    string[] parts = dose.Split('-');
                    result = Convert.ToDecimal(parts[0].Trim()) - Convert.ToDecimal(parts[1].Trim());
                }
                else if (dose.Contains("*"))
                {
                    string[] parts = dose.Split('*');
                    result = Convert.ToDecimal(parts[0].Trim()) * Convert.ToDecimal(parts[1].Trim());
                }
                else
                {
                    result = Convert.ToDecimal(dose);
                }
            }
            catch (Exception ex)
            {
                result = 0;
                Inventec.Common.Logging.LogSystem.Warn("PublicMedicineMultiPatient: khong doc duoc lieu dung '" + dose + "'", ex);
            }
            return result;
        }

        private static string ToDayMonth(long timeNumber)
        {
            string dateString = Inventec.Common.DateTime.Convert.TimeNumberToDateString(timeNumber) ?? "";
            return dateString.Length >= 5 ? dateString.Substring(0, 5) : dateString;
        }

        private static Dictionary<string, PropertyInfo> GetProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .GroupBy(o => o.Name)
                .ToDictionary(o => o.Key, o => o.First());
        }

        private static void SetValue(Dictionary<string, PropertyInfo> properties, object target, string name, object value)
        {
            PropertyInfo property = null;
            if (properties.TryGetValue(name, out property) && property.CanWrite)
                property.SetValue(target, value);
        }
    }
}
