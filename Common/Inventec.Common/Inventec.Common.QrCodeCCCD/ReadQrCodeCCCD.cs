using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.QrCodeCCCD
{
    public class ReadQrCodeCCCD
    {
        public static CccdCardData ReadDataQrCode(string qrCode)
        {
            CccdCardData result = null;
            if (IsQrCodeCccd(qrCode))
            {
                string[] datas = qrCode.Split('|');
                result = new CccdCardData();
                result.CardData = datas[0];
                result.CmndData = datas[1];
                result.PatientName = datas[2];
                result.Dob = ProcessDate(datas[3]);
                result.Gender = datas[4];
                result.Address = datas[5];
                result.ReleaseDate = ProcessDate(datas[6]);
            }
            return result;
        }

        static string ProcessDate(string date)
        {
            string result = "";
            try
            {
                if (!System.String.IsNullOrEmpty(date))
                {
                    if (date.Length == 4)
                    {
                        result = date;
                    }
                    else if (date.Length == 6)
                    {
                        result = new StringBuilder().Append(date.Substring(0, 2)).Append("/").Append(date.Substring(2, 4)).ToString();
                    }
                    else if (date.Length == 8)
                    {
                        result = new StringBuilder().Append(date.Substring(0, 2)).Append("/").Append(date.Substring(2, 2)).Append("/").Append(date.Substring(4, 4)).ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }

        public static bool IsQrCodeCccd(string qrCode)
        {
            bool result = false;
            try
            {
                if (!String.IsNullOrWhiteSpace(qrCode))
                {
                    string[] datas = qrCode.Split('|');
                    if (datas.Length >= 7)
                    {
                        bool hasLetter = false;
                        foreach (var item in datas[0])
                        {
                            if (!char.IsDigit(item))
                            {
                                hasLetter = true;
                                break;
                            }
                        }

                        result = !hasLetter;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
