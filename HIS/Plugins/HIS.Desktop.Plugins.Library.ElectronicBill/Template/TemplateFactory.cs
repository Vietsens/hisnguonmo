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
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Plugins.Library.ElectronicBill.Config;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.Template
{
    public class TemplateFactory
    {
        public static IRunTemplate MakeIRun(TemplateEnum.TYPE tempType, ElectronicBillDataInput dataInput)
        {
            IRunTemplate result = null;
            try
            {
                if (dataInput == null)
                {
                    throw new NullReferenceException();
                }

                HisConfigCFG.LoadConfigForDetail();

                ProcessDataSereServToSereServBill(tempType, ref dataInput);

                //nếu có cấu hình mẫu số hiển thị chi tiết thì gán sang mẫu 4 để hiển thị chi tiết dịch vụ
                if (tempType != TemplateEnum.TYPE.TemplateNhaThuoc && tempType != TemplateEnum.TYPE.Template10 && HisConfigCFG.listTempalteSymbol != null && HisConfigCFG.listTempalteSymbol.Count > 0 &&
                    !String.IsNullOrWhiteSpace(dataInput.SymbolCode) && !String.IsNullOrWhiteSpace(dataInput.TemplateCode) &&
                    HisConfigCFG.listTempalteSymbol.Contains(string.Format("{0}-{1}", dataInput.TemplateCode, dataInput.SymbolCode)))
                {
                    tempType = TemplateEnum.TYPE.Template4;
                }

                switch (tempType)
                {
                    case TemplateEnum.TYPE.TemplateNhaThuoc:
                        result = new TemplateNhaThuoc(dataInput);
                        break;
                    case TemplateEnum.TYPE.Template1:
                        result = new Template1(dataInput.Treatment.ID, dataInput.Branch, dataInput.SereServBill, dataInput);
                        break;
                    case TemplateEnum.TYPE.Template2:
                        result = new Template2(dataInput.Amount ?? 0);
                        break;
                    case TemplateEnum.TYPE.Template3:
                        result = new Template3(dataInput);
                        break;
                    case TemplateEnum.TYPE.Template4:
                        result = new Template4(dataInput);
                        break;
                    case TemplateEnum.TYPE.Template5:
                        result = new Template5(dataInput);
                        break;
                    case TemplateEnum.TYPE.Template6:
                        result = new Template6(dataInput);
                        break;
                    case TemplateEnum.TYPE.Template7:
                        result = new Template7(dataInput);
                        break;
                    case TemplateEnum.TYPE.Template8:
                        result = new Template8(dataInput);
                        break;
                    case TemplateEnum.TYPE.Template9:
                        result = new Template9(dataInput);
                        break;
                    case TemplateEnum.TYPE.Template10:
                        result = new Template10(dataInput);
                        break;
                    case TemplateEnum.TYPE.Template11:
                        result = new Template11(dataInput);
                        break;
                    default:
                        break;
                }

                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                LogSystem.Error("Factory khong khoi tao duoc doi tuong." + LogUtil.TraceData("dataInput", dataInput), ex);
                result = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        internal static void ProcessDataSereServToSereServBill(TemplateEnum.TYPE tempType, ref ElectronicBillDataInput dataInput)
        {
            try
            {
                if (dataInput != null && (dataInput.SereServBill == null || dataInput.SereServBill.Count <= 0) && dataInput.SereServs != null && dataInput.SereServs.Count > 0)
                {
                    dataInput.SereServBill = new List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_BILL>();
                    foreach (var sereServ in dataInput.SereServs)
                    {
                        MOS.EFMODEL.DataModels.HIS_SERE_SERV_BILL ssBill = new MOS.EFMODEL.DataModels.HIS_SERE_SERV_BILL();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_SERE_SERV_BILL>(ssBill, sereServ);
                        ssBill.SERE_SERV_ID = sereServ.ID;
                        ssBill.PRICE = sereServ.VIR_TOTAL_PATIENT_PRICE ?? 0;
                        ssBill.TDL_TREATMENT_ID = sereServ.TDL_TREATMENT_ID ?? 0;
                        ssBill.TDL_ADD_PRICE = sereServ.ADD_PRICE;
                        ssBill.TDL_AMOUNT = sereServ.AMOUNT;
                        ssBill.TDL_DISCOUNT = sereServ.DISCOUNT;
                        ssBill.TDL_EXECUTE_DEPARTMENT_ID = sereServ.TDL_EXECUTE_DEPARTMENT_ID;
                        ssBill.TDL_HEIN_LIMIT_PRICE = sereServ.HEIN_LIMIT_PRICE;
                        ssBill.TDL_HEIN_LIMIT_RATIO = sereServ.HEIN_LIMIT_RATIO;
                        ssBill.TDL_HEIN_NORMAL_PRICE = sereServ.HEIN_NORMAL_PRICE;
                        ssBill.TDL_HEIN_PRICE = sereServ.HEIN_PRICE;
                        ssBill.TDL_HEIN_RATIO = sereServ.HEIN_RATIO;
                        ssBill.TDL_HEIN_SERVICE_TYPE_ID = sereServ.TDL_HEIN_SERVICE_TYPE_ID;
                        ssBill.TDL_IS_OUT_PARENT_FEE = sereServ.IS_OUT_PARENT_FEE;
                        ssBill.TDL_LIMIT_PRICE = sereServ.LIMIT_PRICE;
                        ssBill.TDL_ORIGINAL_PRICE = sereServ.ORIGINAL_PRICE;
                        ssBill.TDL_OTHER_SOURCE_PRICE = sereServ.OTHER_SOURCE_PRICE;
                        ssBill.TDL_OVERTIME_PRICE = sereServ.OVERTIME_PRICE;
                        ssBill.TDL_PATIENT_TYPE_ID = sereServ.PATIENT_TYPE_ID;
                        ssBill.TDL_PRICE = sereServ.PRICE;
                        ssBill.TDL_PRIMARY_PRICE = sereServ.PRIMARY_PRICE;
                        ssBill.TDL_REQUEST_DEPARTMENT_ID = sereServ.TDL_REQUEST_DEPARTMENT_ID;
                        ssBill.TDL_SERE_SERV_PARENT_ID = sereServ.PARENT_ID;
                        ssBill.TDL_SERVICE_CODE = sereServ.TDL_SERVICE_CODE;
                        ssBill.TDL_SERVICE_ID = sereServ.SERVICE_ID;
                        ssBill.TDL_SERVICE_NAME = sereServ.TDL_SERVICE_NAME;
                        ssBill.TDL_SERVICE_TYPE_ID = sereServ.TDL_SERVICE_TYPE_ID;
                        ssBill.TDL_SERVICE_UNIT_ID = sereServ.TDL_SERVICE_UNIT_ID;
                        ssBill.TDL_TOTAL_HEIN_PRICE = sereServ.VIR_TOTAL_HEIN_PRICE;
                        ssBill.TDL_TOTAL_PATIENT_PRICE = sereServ.VIR_TOTAL_PATIENT_PRICE;
                        ssBill.TDL_TOTAL_PATIENT_PRICE_BHYT = sereServ.VIR_TOTAL_PATIENT_PRICE_BHYT;
                        ssBill.TDL_USER_PRICE = sereServ.USER_PRICE;
                        ssBill.TDL_VAT_RATIO = sereServ.VAT_RATIO;
                        ssBill.TDL_REAL_HEIN_PRICE = sereServ.VIR_HEIN_PRICE;
                        ssBill.TDL_REAL_PATIENT_PRICE = sereServ.VIR_PATIENT_PRICE;
                        ssBill.TDL_REAL_PRICE = sereServ.VIR_PRICE;
                        ssBill.TDL_SERVICE_REQ_ID = sereServ.SERVICE_REQ_ID;
                        ssBill.TDL_PRIMARY_PATIENT_TYPE_ID = sereServ.PRIMARY_PATIENT_TYPE_ID;

                        ssBill.VAT_RATIO = sereServ.VAT_RATIO;

                        if (tempType == TemplateEnum.TYPE.TemplateNhaThuoc && sereServ.PRICE <= 0 && (sereServ.VIR_PRICE ?? 0) > 0)
                        {
                            ssBill.TDL_PRICE = sereServ.VIR_PRICE ?? 0;
                        }
                        else if (tempType == TemplateEnum.TYPE.TemplateNhaThuoc)
                        {
                            ssBill.PRICE = sereServ.PRICE * sereServ.AMOUNT * (1 + sereServ.VAT_RATIO);
                            ssBill.TDL_SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
                        }
                        dataInput.SereServBill.Add(ssBill);
                    }
                }

                //loại bỏ các dịch vụ có số tiền thanh toán bằng 0.
                dataInput.SereServBill = dataInput.SereServBill.Where(o => o.PRICE != 0).ToList();

                dataInput.SereServBill.ForEach(o => o.TDL_TOTAL_PATIENT_PRICE_BHYT = o.TDL_TOTAL_PATIENT_PRICE_BHYT.HasValue && o.TDL_TOTAL_PATIENT_PRICE_BHYT > o.PRICE ? (decimal?)o.PRICE : o.TDL_TOTAL_PATIENT_PRICE_BHYT);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal static List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5> GetSereServWithVAT(ElectronicBillDataInput dataInput)
        {
            List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5> result = null;
            try
            {
                if (dataInput != null)
                {
                    if (dataInput.SereServs != null && dataInput.SereServs.Count > 0)
                    {
                        result = dataInput.SereServs.Where(o => o.VAT_RATIO > 0).ToList();
                    }
                    else if (dataInput.SereServBill != null && dataInput.SereServBill.Count > 0)
                    {
                        result = new List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5>();
                        foreach (var ssBill in dataInput.SereServBill)
                        {
                            if (!ssBill.TDL_VAT_RATIO.HasValue || ssBill.TDL_VAT_RATIO.Value <= 0)
                            {
                                continue;
                            }
                            MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5 ss = new MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5();

                            ss.ID = ssBill.SERE_SERV_ID;
                            ss.TDL_TREATMENT_ID = ssBill.TDL_TREATMENT_ID;
                            ss.ADD_PRICE = ssBill.TDL_ADD_PRICE;
                            ss.AMOUNT = ssBill.TDL_AMOUNT ?? 0;
                            ss.DISCOUNT = ssBill.TDL_DISCOUNT;
                            ss.TDL_EXECUTE_DEPARTMENT_ID = ssBill.TDL_EXECUTE_DEPARTMENT_ID ?? 0;
                            ss.HEIN_LIMIT_PRICE = ssBill.TDL_HEIN_LIMIT_PRICE;
                            ss.HEIN_LIMIT_RATIO = ssBill.TDL_HEIN_LIMIT_RATIO;
                            ss.HEIN_NORMAL_PRICE = ssBill.TDL_HEIN_NORMAL_PRICE;
                            ss.HEIN_PRICE = ssBill.TDL_HEIN_PRICE;
                            ss.HEIN_RATIO = ssBill.TDL_HEIN_RATIO;
                            ss.TDL_HEIN_SERVICE_TYPE_ID = ssBill.TDL_HEIN_SERVICE_TYPE_ID;
                            ss.IS_OUT_PARENT_FEE = ssBill.TDL_IS_OUT_PARENT_FEE;
                            ss.LIMIT_PRICE = ssBill.TDL_LIMIT_PRICE;
                            ss.ORIGINAL_PRICE = ssBill.TDL_ORIGINAL_PRICE ?? 0;
                            ss.OTHER_SOURCE_PRICE = ssBill.TDL_OTHER_SOURCE_PRICE;
                            ss.OVERTIME_PRICE = ssBill.TDL_OVERTIME_PRICE;
                            ss.PATIENT_TYPE_ID = ssBill.TDL_PATIENT_TYPE_ID ?? 0;
                            ss.PRICE = ssBill.TDL_PRICE ?? 0;
                            ss.PRIMARY_PRICE = ssBill.TDL_PRIMARY_PRICE;
                            ss.TDL_REQUEST_DEPARTMENT_ID = ssBill.TDL_REQUEST_DEPARTMENT_ID ?? 0;
                            ss.PARENT_ID = ssBill.TDL_SERE_SERV_PARENT_ID;
                            ss.TDL_SERVICE_CODE = ssBill.TDL_SERVICE_CODE;
                            ss.SERVICE_ID = ssBill.TDL_SERVICE_ID ?? 0;
                            ss.TDL_SERVICE_NAME = ssBill.TDL_SERVICE_NAME;
                            ss.TDL_SERVICE_TYPE_ID = ssBill.TDL_SERVICE_TYPE_ID ?? 0;
                            ss.TDL_SERVICE_UNIT_ID = ssBill.TDL_SERVICE_UNIT_ID ?? 0;
                            ss.VIR_TOTAL_HEIN_PRICE = ssBill.TDL_TOTAL_HEIN_PRICE;
                            ss.VIR_TOTAL_PATIENT_PRICE = ssBill.TDL_TOTAL_PATIENT_PRICE;
                            ss.VIR_TOTAL_PATIENT_PRICE_BHYT = ssBill.TDL_TOTAL_PATIENT_PRICE_BHYT;
                            ss.USER_PRICE = ssBill.TDL_USER_PRICE;
                            ss.VAT_RATIO = ssBill.TDL_VAT_RATIO ?? 0;
                            ss.VIR_HEIN_PRICE = ssBill.TDL_REAL_HEIN_PRICE;
                            ss.VIR_PATIENT_PRICE = ssBill.TDL_REAL_PATIENT_PRICE;
                            ss.VIR_PRICE = ssBill.TDL_REAL_PRICE;
                            ss.SERVICE_REQ_ID = ssBill.TDL_SERVICE_REQ_ID;
                            ss.PRIMARY_PATIENT_TYPE_ID = ssBill.TDL_PRIMARY_PATIENT_TYPE_ID;

                            result.Add(ss);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
