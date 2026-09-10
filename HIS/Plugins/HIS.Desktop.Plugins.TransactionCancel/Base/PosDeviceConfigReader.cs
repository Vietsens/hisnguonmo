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
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace HIS.Desktop.Plugins.TransactionCancel.Base
{
    /// <summary>
    /// Doc cau hinh cua tien trinh POS.WCFService (WCF.exe.config).
    /// Hop dong WCF IService1 chi co Sale/Void/cauhinh nen khong hoi duoc ten hang POS qua WCF;
    /// doc truc tiep file cau hinh la cach lay thong tin ma khong phai doi hop dong
    /// va build lai toan bo cac plugin dang dung POS.
    /// </summary>
    class PosDeviceConfigReader
    {
        /// <summary>Ten hang POS Techcombank - trung gia tri trong WCF.Common.PosBankName.TCB.</summary>
        internal const string BANK_NAME_TECHCOMBANK = "POS TECHCOMBANK";

        private const string CONFIG_RELATIVE_PATH = @"Integrate\POS.WCFService\WCF.exe.config";
        private const string CONFIG_KEY_NAME_BANK = "namebank";

        /// <summary>
        /// Lay ten hang may POS dang duoc cau hinh tren may tram.
        /// Tra ve chuoi rong neu chua cau hinh hoac khong doc duoc file.
        /// </summary>
        internal static string GetBankName()
        {
            try
            {
                string path = Path.Combine(Application.StartupPath, CONFIG_RELATIVE_PATH);
                if (!File.Exists(path))
                {
                    Inventec.Common.Logging.LogSystem.Info(
                        "PosDeviceConfigReader: khong tim thay file cau hinh POS. Path=" + path);
                    return "";
                }

                XDocument document = XDocument.Load(path);
                if (document.Root == null)
                {
                    return "";
                }

                XElement appSettings = document.Root.Element("appSettings");
                if (appSettings == null)
                {
                    return "";
                }

                XElement entry = appSettings.Elements("add").FirstOrDefault(
                    o => o.Attribute("key") != null
                        && String.Equals(o.Attribute("key").Value, CONFIG_KEY_NAME_BANK, StringComparison.OrdinalIgnoreCase));

                if (entry == null || entry.Attribute("value") == null)
                {
                    return "";
                }

                return entry.Attribute("value").Value.Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }

        /// <summary>
        /// May POS Techcombank chi co lenh "ocan" de huy THAO TAC dang cho o man hinh quet the,
        /// khong huy duoc giao dich da duoc ngan hang duyet.
        /// Vi vay nghiep vu huy giao dich qua POS cua HIS khong ap dung duoc cho hang nay.
        /// </summary>
        internal static bool IsVoidSupported()
        {
            try
            {
                string bankName = GetBankName();
                return !String.Equals(bankName, BANK_NAME_TECHCOMBANK, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return true;
            }
        }
    }
}
