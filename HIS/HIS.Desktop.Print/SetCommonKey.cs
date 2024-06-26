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
using HIS.Desktop.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Print
{
    public class SetCommonKey
    {
        public const string MOS__ORGANIZATION_NAME = "MOS.ORGANIZATION_NAME";
        public const string MOS__PARENT_ORGANIZATION_NAME = "MOS.PARENT_ORGANIZATION_NAME";

        public static void SetCommonSingleKey(Dictionary<string, object> singleValueDictionary)
        {
            try
            {
                //singleValueDictionary.Add(CommonKey._PARENT_ORGANIZATION_NAME, OrganizationCFG.PARENT_ORGANIZATION_NAME);//TODO
                //singleValueDictionary.Add(CommonKey._ORGANIZATION_NAME, OrganizationCFG.ORGANIZATION_NAME);//TODO
                singleValueDictionary.Add(CommonKey._PARENT_ORGANIZATION_NAME, HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.PARENT_ORGANIZATION_NAME);
                singleValueDictionary.Add(CommonKey._ORGANIZATION_NAME, HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.ORGANIZATION_NAME);

                System.DateTime now = System.DateTime.Now;
                singleValueDictionary.Add(CommonKey._CURRENT_TIME_STR, now.ToString("dd/MM/yyyy HH:mm:ss"));
                singleValueDictionary.Add(CommonKey._CURRENT_DATE_STR, now.ToString("dd/MM/yyyy"));
                singleValueDictionary.Add(CommonKey._CURRENT_MONTH_STR, now.ToString("MM/yyyy"));
                singleValueDictionary.Add(CommonKey._CURRENT_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.SystemDateTimeToDateSeparateString(now));
                singleValueDictionary.Add(CommonKey._CURRENT_TIME_SEPARATE_STR, GlobalReportQuery.GetCurrentTime());
                singleValueDictionary.Add(CommonKey._CURRENT_TIME_SEPARATE_BEGIN_TIME_STR, GlobalReportQuery.GetCurrentDateSeparateFullTime());
                singleValueDictionary.Add(CommonKey._CURRENT_TIME_SEPARATE_WITHOUT_SECOND_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(Inventec.Common.DateTime.Get.Now() ?? 0));
                singleValueDictionary.Add(CommonKey._CURRENT_MONTH_SEPARATE_STR, Inventec.Common.DateTime.Convert.SystemDateTimeToMonthSeparateString(now));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //internal static void SetCommonBarcodeKey(Dictionary<string, System.Drawing.Image> imgValueDictionary, string key, string value)
        //{
        //    try
        //    {
        //        Inventec.Common.BarcodeLib.Barcode barcode = new Inventec.Common.BarcodeLib.Barcode(value);

        //        barcode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
        //        barcode.Width = 120;
        //        barcode.Height = 40;
        //        barcode.RotateFlipType = System.Drawing.RotateFlipType.Rotate180FlipXY;
        //        barcode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
        //        barcode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
        //        barcode.IncludeLabel = false;

        //        imgValueDictionary.Add(key, barcode.Encode(Inventec.Common.BarcodeLib.TYPE.CODE128, value, 120, 40));
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

    }
}
