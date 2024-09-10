using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.QrCodeBHYT
{
    public class ReadQrCodeHeinCard
    {
        public ReadQrCodeHeinCard() { }

        string ProcessDateInvalid(string inputDate)
        {
            string strResult = "";
            try
            {
                if (!System.String.IsNullOrEmpty(inputDate))
                {
                    string[] arrDobPartial = inputDate.Split('/');
                    if (arrDobPartial != null && arrDobPartial.Length > 0)
                    {
                        for (int i = 0; i < arrDobPartial.Length; i++)
                        {
                            if (!System.String.IsNullOrEmpty(arrDobPartial[i]))
                            {
                                if (arrDobPartial[i].Length == 1)
                                {
                                    strResult += ((arrDobPartial[i].Length == 1 ? "0" : "") + arrDobPartial[i]);
                                }
                                else if (arrDobPartial[i].Length == 2 || arrDobPartial[i].Length == 4)
                                {
                                    strResult += (arrDobPartial[i]);
                                }
                                if (i < arrDobPartial.Length - 1)
                                {
                                    strResult += "/";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }

            return strResult;
        }

        public HeinCardData ReadDataQrCode(string qrCodeInput)
        {
            HeinCardData result = null;
            try
            {
                string[] arrSplits = qrCodeInput.Split('|');
                if (arrSplits != null && arrSplits.Length > 0)
                {
                    result = new HeinCardData();
                    result.HeinCardNumber = arrSplits[0];
                    result.PatientName = arrSplits[1];
                    result.Dob = ProcessDateInvalid(arrSplits[2]);
                    result.Gender = arrSplits[3];
                    result.Address = arrSplits[4];
                    string mediOrgData = arrSplits[5];
                    if (!System.String.IsNullOrEmpty(mediOrgData))
                    {
                        try
                        {
                            string[] arrSplitsMediOrg;
                            if (mediOrgData.Contains("-"))
                            {
                                arrSplitsMediOrg = mediOrgData.Split('-');
                            }
                            else if (mediOrgData.Contains("–"))
                            {
                                arrSplitsMediOrg = mediOrgData.Split('–');
                            }
                            else
                            {
                                arrSplitsMediOrg = mediOrgData.Split(' ');
                            }
                            if (arrSplitsMediOrg != null && arrSplitsMediOrg.Length > 0)
                            {
                                foreach (var item in arrSplitsMediOrg)
                                {
                                    result.MediOrgCode += item.Trim();
                                }
                            }
                        }
                        catch { }
                    }
                    result.FromDate = ProcessDateInvalid(arrSplits[6]);
                    result.ToDate = ProcessDateInvalid(arrSplits[7]);
                    result.IssueDate = ProcessDateInvalid(arrSplits[8]);
                    result.ManagerCodeBHXH = arrSplits[9];
                    result.ParentName = arrSplits[10];
                    result.LiveAreaCode = arrSplits[11];
                    result.FineYearMonthDate = arrSplits[12];
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
