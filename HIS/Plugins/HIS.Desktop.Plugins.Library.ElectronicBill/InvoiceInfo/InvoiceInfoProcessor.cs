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
using HIS.Desktop.Plugins.Library.ElectronicBill.Config;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.InvoiceInfo
{
    class InvoiceInfoProcessor
    {
        internal static InvoiceInfoADO GetData(Base.ElectronicBillDataInput dataInput, bool isFillByTreatment = true)
        {
            InvoiceInfoADO result = new InvoiceInfoADO();
            try
            {
                string patientCode = "";
                string treatmentCode = "";

                if (dataInput.Transaction != null)
                {
                    result.BuyerOrganization = dataInput.Transaction.BUYER_ORGANIZATION;
                    result.BuyerTaxCode = dataInput.Transaction.BUYER_TAX_CODE;
                    result.BuyerAccountNumber = dataInput.Transaction.BUYER_ACCOUNT_NUMBER;
                    result.BuyerAddress = dataInput.Transaction.BUYER_ADDRESS ?? " ";
                    result.BuyerPhone = dataInput.Transaction.BUYER_PHONE;
                    result.BuyerName = dataInput.Transaction.BUYER_NAME;
                    result.BuyerDob = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataInput.Transaction.TDL_PATIENT_DOB ?? 0);
                    result.BuyerCode = dataInput.Transaction.TDL_PATIENT_CODE;
                    result.BuyerGender = dataInput.Transaction.TDL_PATIENT_GENDER_NAME;
                    result.Note = dataInput.Transaction.DESCRIPTION;
                    result.TransactionTime = dataInput.Transaction.TRANSACTION_TIME;

                    result.BuyerEmail = dataInput.Transaction.BUYER_EMAIL;

                    patientCode = dataInput.Transaction.TDL_PATIENT_CODE;
                    treatmentCode = dataInput.Transaction.TDL_TREATMENT_CODE;
                    
                    
                    if (dataInput.Transaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__CK)
                    {
                        result.PaymentMethod = "CK";
                    }
                    else if (dataInput.Transaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TM)
                    {
                        result.PaymentMethod = "TM";
                    }
                    else if (dataInput.Transaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK)
                    {
                        result.PaymentMethod = "TM/CK";
                    }
                    else if (dataInput.Transaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE || dataInput.Transaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QUET_THE)
                    {
                        result.PaymentMethod = "THE";
                    }
                }
                else if (dataInput.ListTransaction != null && dataInput.ListTransaction.Count > 0)
                {
                    
                    var legal = dataInput.ListTransaction.Where(o => !String.IsNullOrWhiteSpace(o.BUYER_ORGANIZATION)).OrderByDescending(o => o.TRANSACTION_TIME).FirstOrDefault();
                    if (legal != null)
                    {
                        result.BuyerOrganization = legal.BUYER_ORGANIZATION;
                    }

                    var tax = dataInput.ListTransaction.Where(o => !String.IsNullOrWhiteSpace(o.BUYER_TAX_CODE)).OrderByDescending(o => o.TRANSACTION_TIME).FirstOrDefault();
                    if (tax != null)
                    {
                        result.BuyerTaxCode = tax.BUYER_TAX_CODE;
                    }

                    var account = dataInput.ListTransaction.Where(o => !String.IsNullOrWhiteSpace(o.BUYER_ACCOUNT_NUMBER)).OrderByDescending(o => o.TRANSACTION_TIME).FirstOrDefault();
                    if (account != null)
                    {
                        result.BuyerAccountNumber = account.BUYER_ACCOUNT_NUMBER;
                    }

                    var address = dataInput.ListTransaction.Where(o => !String.IsNullOrWhiteSpace(o.BUYER_ADDRESS)).OrderByDescending(o => o.TRANSACTION_TIME).FirstOrDefault();
                    if (address != null)
                    {
                        result.BuyerAddress = address.BUYER_ADDRESS ?? " ";
                    }

                    var phone = dataInput.ListTransaction.Where(o => !String.IsNullOrWhiteSpace(o.BUYER_PHONE)).OrderByDescending(o => o.TRANSACTION_TIME).FirstOrDefault();
                    if (phone != null)
                    {
                        result.BuyerPhone = legal.BUYER_PHONE;
                    }

                    var name = dataInput.ListTransaction.Where(o => !String.IsNullOrWhiteSpace(o.BUYER_NAME)).OrderByDescending(o => o.TRANSACTION_TIME).FirstOrDefault();
                    if (name != null)
                    {
                        result.BuyerName = name.BUYER_NAME;
                    }

                    var bob = dataInput.ListTransaction.Where(o => o.TDL_PATIENT_DOB.HasValue).OrderByDescending(o => o.TRANSACTION_TIME).FirstOrDefault();
                    if (bob != null)
                    {
                        result.BuyerDob = Inventec.Common.DateTime.Convert.TimeNumberToDateString(bob.TDL_PATIENT_DOB ?? 0);
                    }

                    var tranPatientCode = dataInput.ListTransaction.Where(o => !String.IsNullOrWhiteSpace(o.TDL_PATIENT_CODE)).OrderByDescending(o => o.TRANSACTION_TIME).FirstOrDefault();
                    if (tranPatientCode != null)
                    {
                        result.BuyerCode = tranPatientCode.TDL_PATIENT_CODE;
                        patientCode = tranPatientCode.TDL_PATIENT_CODE;
                    }

                    var tranTreatmentCode = dataInput.ListTransaction.Where(o => !String.IsNullOrWhiteSpace(o.TDL_TREATMENT_CODE)).OrderByDescending(o => o.TRANSACTION_TIME).FirstOrDefault();
                    if (tranTreatmentCode != null)
                    {
                        treatmentCode = tranTreatmentCode.TDL_TREATMENT_CODE;
                    }

                    var gender = dataInput.ListTransaction.Where(o => !String.IsNullOrWhiteSpace(o.TDL_PATIENT_GENDER_NAME)).OrderByDescending(o => o.TRANSACTION_TIME).FirstOrDefault();
                    if (gender != null)
                    {
                        result.BuyerGender = gender.TDL_PATIENT_GENDER_NAME;
                    }

                    var description = dataInput.ListTransaction.Where(o => !String.IsNullOrWhiteSpace(o.DESCRIPTION)).OrderByDescending(o => o.TRANSACTION_TIME).ToList();
                    if (description != null && description.Count > 0)
                    {
                        result.Note = string.Join("; ", description);
                    }

                    result.TransactionTime = dataInput.ListTransaction.Max(o => o.TRANSACTION_TIME);
                }

                if (dataInput.Treatment != null && isFillByTreatment)
                {
                    if (String.IsNullOrWhiteSpace(result.BuyerOrganization))
                        result.BuyerOrganization = dataInput.Treatment.TDL_PATIENT_WORK_PLACE_NAME ?? dataInput.Treatment.TDL_PATIENT_WORK_PLACE ?? "";

                    if (String.IsNullOrWhiteSpace(result.BuyerTaxCode))
                        result.BuyerTaxCode = dataInput.Treatment.TDL_PATIENT_TAX_CODE;

                    if (String.IsNullOrWhiteSpace(result.BuyerAccountNumber))
                        result.BuyerAccountNumber = dataInput.Treatment.TDL_PATIENT_ACCOUNT_NUMBER;

                    if (String.IsNullOrWhiteSpace(result.BuyerAddress))
                        result.BuyerAddress = dataInput.Treatment.TDL_PATIENT_ADDRESS ?? " ";

                    if (String.IsNullOrWhiteSpace(result.BuyerDob) || result.BuyerDob == "0")
                        result.BuyerDob = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataInput.Treatment.TDL_PATIENT_DOB);

                    if (String.IsNullOrWhiteSpace(result.BuyerCode))
                        result.BuyerCode = dataInput.Treatment.TDL_PATIENT_CODE;

                    if (String.IsNullOrWhiteSpace(result.BuyerName))
                        result.BuyerName = dataInput.Treatment.TDL_PATIENT_NAME;

                    if (String.IsNullOrWhiteSpace(result.BuyerPhone))
                        result.BuyerPhone = dataInput.Treatment.TDL_PATIENT_PHONE ?? dataInput.Treatment.TDL_PATIENT_MOBILE;

                    if (String.IsNullOrWhiteSpace(result.BuyerGender))
                        result.BuyerGender = dataInput.Treatment.TDL_PATIENT_GENDER_NAME;

                    if (String.IsNullOrWhiteSpace(patientCode))
                        patientCode = dataInput.Treatment.TDL_PATIENT_CODE;

                    if (String.IsNullOrWhiteSpace(treatmentCode))
                        treatmentCode = dataInput.Treatment.TREATMENT_CODE;
                }

                if (!String.IsNullOrWhiteSpace(result.BuyerTaxCode))
                    result.BuyerTaxCode = result.BuyerTaxCode.Trim();

                if (!String.IsNullOrWhiteSpace(result.BuyerAccountNumber))
                    result.BuyerAccountNumber = result.BuyerAccountNumber.Trim();

                if (!String.IsNullOrWhiteSpace(result.BuyerAddress))
                    result.BuyerAddress = result.BuyerAddress.Trim();

                if (!String.IsNullOrWhiteSpace(result.BuyerCode))
                    result.BuyerCode = result.BuyerCode.Trim();

                if (!String.IsNullOrWhiteSpace(result.BuyerName))
                    result.BuyerName = result.BuyerName.Trim();

                if (!String.IsNullOrWhiteSpace(result.BuyerPhone))
                    result.BuyerPhone = result.BuyerPhone.Trim();

                if (!String.IsNullOrWhiteSpace(result.BuyerOrganization))
                    result.BuyerOrganization = result.BuyerOrganization.Trim();

                if (!String.IsNullOrWhiteSpace(result.BuyerEmail))
                    result.BuyerEmail = result.BuyerEmail.Trim();

                if (!String.IsNullOrWhiteSpace(result.Note))
                    result.Note = result.Note.Trim();

                if (!String.IsNullOrWhiteSpace(result.PaymentMethod))
                    result.PaymentMethod = result.PaymentMethod.Trim();

                if (Config.HisConfigCFG.BuyerCodeOption == "1")
                {
                    result.BuyerCode = patientCode;
                }
                else if (Config.HisConfigCFG.BuyerCodeOption == "2")
                {
                    result.BuyerCode = treatmentCode;
                }
                else
                {
                    result.BuyerCode = "";
                }
                //Nếu thông tin đơn vị không có giá trị thì hiển thị thông tin tên người mua hàng vào thông tin đơn vị.
                if (Config.HisConfigCFG.BuyerOrganizationOption == "1" && String.IsNullOrWhiteSpace(result.BuyerOrganization))
                {
                    result.BuyerOrganization = result.BuyerName;
                }
                //Cho phép khai báo các thông tin gắn kèm với tên của người mua trên hóa đơn điện tử
                if (!String.IsNullOrWhiteSpace(HisConfigCFG.BuyerNameOption))
                {
                    Dictionary<string, string> dicTreatmentValues = Base.General.ProcessDicValueString(dataInput);
                    string addName = HisConfigCFG.BuyerNameOption;
                    foreach (var item in dicTreatmentValues)
                    {
                        addName = addName.Replace(string.Format("%{0}%", item.Key), item.Value);
                    }

                    result.BuyerName += " " + addName;
                }
            }
            catch (Exception ex)
            {
                result = new InvoiceInfoADO();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
