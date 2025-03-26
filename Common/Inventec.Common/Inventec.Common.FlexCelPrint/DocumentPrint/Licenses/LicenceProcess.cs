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

namespace Inventec.Common.FlexCelPrint.DocumentPrint.License
{
    internal class LicenceProcess
    {
        internal static void SetLicenseForAspose()
        {
            try
            {
                if (!String.IsNullOrEmpty(Licenses.Aspose_Key))
                {
                    Stream Aspose_LStream = (Stream)new MemoryStream(Convert.FromBase64String(Licenses.Aspose_Key));
                    Aspose.Pdf.License license = new Aspose.Pdf.License();
                    license.SetLicense(Aspose_LStream);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void SetLicenseForAsposeCell()
        {
            try
            {
                if (!String.IsNullOrEmpty(Licenses.Aspose_Key))
                {
                    Stream Aspose_LStream = (Stream)new MemoryStream(Convert.FromBase64String(Licenses.Aspose_Key));
                    Aspose.Cells.License license = new Aspose.Cells.License();
                    license.SetLicense(Aspose_LStream);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
