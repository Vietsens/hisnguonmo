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
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.FeeHospitalWarning
{
    class HisConfigWarning
    {
        //format: Diện điều trị: danh sách giá:danh sách màu | (phân cách bởi dấu ; và có cùng số lượng, nếu khác sẽ loại bỏ dòng không có đi theo thứ tự từ trái qua)
        private const string key = "HIS.Desktop.Plugins.FeeHospitalWarning.WarningHeinPrice";

        internal static List<WarningColor> GetConfigWarning()
        {
            List<WarningColor> warningColor = new List<WarningColor>();
            try
            {
                var valueConfig = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(key);
                if (!String.IsNullOrWhiteSpace(valueConfig))
                {
                    var typeCfg = valueConfig.Trim().Split('|');
                    foreach (var item in typeCfg)
                    {
                        var splitCf = item.Trim().Split(':');
                        if (splitCf.Length == 3)
                        {
                            var type = splitCf[0];

                            var treatmentType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.TREATMENT_TYPE_CODE == type);
                            if (treatmentType == null || warningColor.Exists(o => o.TreatmentTypeId == treatmentType.ID))
                            {
                                continue;
                            }

                            var priceStr = splitCf[1].Split(';');
                            var colorStr = splitCf[2].Split(';');

                            int min = priceStr.Length;
                            if (colorStr.Length < priceStr.Length)
                            {
                                min = colorStr.Length;
                            }

                            for (int i = 0; i < min; i++)
                            {
                                WarningColor ado = new WarningColor();
                                ado.TreatmentTypeId = treatmentType.ID;
                                ado.Price = Inventec.Common.TypeConvert.Parse.ToDecimal(priceStr[i]);
                                ado.Color = colorStr[i];
                                warningColor.Add(ado);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                warningColor = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return warningColor;
        }

        internal static HIS_WARNING_FEE_CFG GetWarningHeinFee(long patientTypeId, long treatmentTypeId, bool isRightMediorg, decimal totalHeinPrice)
        {
            try
            {
                if (totalHeinPrice > 0)
                {
                    List<HIS_WARNING_FEE_CFG> results = BackendDataWorker.Get<HIS_WARNING_FEE_CFG>();
                    results = (results != null && results.Count > 0 ? results.Where(o =>
                        o.PATIENT_TYPE_ID == patientTypeId &&
                        o.TREATMENT_TYPE_ID == treatmentTypeId &&
                        (isRightMediorg ? o.IS_RIGHT_MEDI_ORG == 1 : (o.IS_RIGHT_MEDI_ORG == null || o.IS_RIGHT_MEDI_ORG != 1))).ToList() : null);
                    if (results != null && results.Count > 0)
                    {
                        var warningFeeFirst = results.Where(o => o.WARNING_PRICE <= totalHeinPrice).OrderByDescending(o => o.WARNING_PRICE).FirstOrDefault();
                        return warningFeeFirst;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return null;
        }
    }

    class WarningColor
    {
        public long TreatmentTypeId { get; set; }
        public decimal Price { get; set; }
        public string Color { get; set; }
    }
}
