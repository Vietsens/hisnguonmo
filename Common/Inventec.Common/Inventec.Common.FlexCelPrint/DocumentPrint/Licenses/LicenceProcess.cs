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
