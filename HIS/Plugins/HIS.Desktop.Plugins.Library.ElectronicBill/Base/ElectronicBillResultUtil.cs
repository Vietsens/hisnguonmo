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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.Base
{
    public class ElectronicBillResultUtil
    {
        public static void Set(ref ElectronicBillResult electronicBillResult, bool success, string message)
        {
            try
            {
                if (electronicBillResult == null)
                {
                    electronicBillResult = new ElectronicBillResult();
                }

                if (electronicBillResult.Messages == null)
                {
                    electronicBillResult.Messages = new List<string>();
                }
                electronicBillResult.Success = success;
                if (!String.IsNullOrEmpty(message))
                {
                    electronicBillResult.Messages.Add(message);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void Set(ref ElectronicBillResult electronicBillResult, bool success, List<string> messages)
        {
            try
            {
                if (electronicBillResult == null)
                {
                    electronicBillResult = new ElectronicBillResult();
                }

                if (electronicBillResult.Messages == null)
                {
                    electronicBillResult.Messages = new List<string>();
                }
                electronicBillResult.Success = success;
                if (messages != null && messages.Count > 0)
                {
                    electronicBillResult.Messages.AddRange(messages);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region file
        public static string ProcessPdfFileResult(string base64string)
        {
            string result = "";
            try
            {
                if (!string.IsNullOrWhiteSpace(base64string))
                {
                    byte[] data = System.Convert.FromBase64String(base64string);
                    result = ProcessPdfFileResult(data);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public static string ProcessPdfFileResult(byte[] fileToBytes)
        {
            string result = "";
            try
            {
                string tempFileName = Path.GetTempFileName();
                tempFileName = tempFileName.Replace("tmp", "pdf");
                try
                {
                    File.WriteAllBytes(tempFileName, fileToBytes);
                    result = tempFileName;
                }
                catch (Exception ex)
                {
                    result = "";
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        #endregion
    }
}
