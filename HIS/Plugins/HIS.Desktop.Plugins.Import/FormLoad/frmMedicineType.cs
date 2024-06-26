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
using DevExpress.Data;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Import.FormLoad
{
    public partial class frmMedicineType : Form
    {
        List<MedicineTypeImportADO> medicineTypeAdos;
        List<MedicineTypeImportADO> currentAdos;
        RefeshReference delegateRefresh;

        public frmMedicineType(List<MedicineTypeImportADO> data, RefeshReference _delegate)
        {
            InitializeComponent();
            try
            {
                this.currentAdos = data;
                this.delegateRefresh = _delegate;

                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckErrorLine(List<MedicineTypeImportADO> dataSource)
        {
            try
            {
                var checkError = medicineTypeAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
                if (!checkError)
                {
                    btnSave.Enabled = true;
                    btnShowLineError.Enabled = false;
                }
                else
                {
                    btnShowLineError.Enabled = true;
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetDataSource(List<MedicineTypeImportADO> dataSource)
        {
            try
            {
                gridControlMedicineType.BeginUpdate();
                gridControlMedicineType.DataSource = null;
                gridControlMedicineType.DataSource = dataSource;
                gridControlMedicineType.EndUpdate();
                CheckErrorLine(null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void convertDateStringToTimeNumber(string date, ref long? dateTime, ref string check)
        {
            try
            {
                if (date.Length > 14)
                {
                    check = Message.MessageImport.Maxlength;
                    return;
                }

                if (date.Length < 14)
                {
                    check = Message.MessageImport.KhongHopLe;
                    return;
                }

                if (date.Length > 0)
                {
                    if (!Inventec.Common.DateTime.Check.IsValidTime(date))
                    {
                        check = Message.MessageImport.KhongHopLe;
                        return;
                    }
                    dateTime = Inventec.Common.TypeConvert.Parse.ToInt64(date);
                }




                //string[] substring = date.Split('/');
                //if (substring != null)
                //{
                //    if (substring.Count() != 3)
                //    {
                //        check = false;
                //        return;
                //    }
                //    if (Inventec.Common.TypeConvert.Parse.ToInt64(substring[0]) < 0 || Inventec.Common.TypeConvert.Parse.ToInt64(substring[0]) > 31)
                //    {
                //        check = false;
                //        return;
                //    }
                //    if (Inventec.Common.TypeConvert.Parse.ToInt64(substring[1]) < 0 || Inventec.Common.TypeConvert.Parse.ToInt64(substring[1]) > 12)
                //    {
                //        check = false;
                //        return;
                //    }
                //    if (Inventec.Common.TypeConvert.Parse.ToInt64(substring[2]) < 0 || Inventec.Common.TypeConvert.Parse.ToInt64(substring[2]) > 9999)
                //    {
                //        check = false;
                //        return;
                //    }
                //}
                //string dateString = substring[2] + substring[1] + substring[0] + "000000";
                //dateTime = Inventec.Common.TypeConvert.Parse.ToInt64(dateString);

                ////date.Replace(" ", "");
                ////int idx = date.LastIndexOf("/");
                ////string year = date.Substring(idx + 1);
                ////string monthdate = date.Substring(0, idx);
                ////idx = monthdate.LastIndexOf("/");
                ////monthdate.Remove(idx);
                ////idx = monthdate.LastIndexOf("/");
                ////string month = monthdate.Substring(idx + 1);
                ////string dateStr = monthdate.Substring(0, idx);
                ////if (month.Length < 2)
                ////{
                ////    month = "0" + month;
                ////}
                ////if (dateStr.Length < 2)
                ////{
                ////    dateStr = "0" + dateStr;
                ////}
                ////datetime = year + month + dateStr;
                ////datetime.Replace("/", "");

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void addMedicineTypeToProcessList(List<MedicineTypeImportADO> _medicine, ref List<MedicineTypeImportADO> _medicineRef)
        {
            try
            {
                _medicineRef = new List<MedicineTypeImportADO>();
                long i = 0;
                foreach (var item in _medicine)
                {
                    i++;
                    string error = "";
                    var mediAdo = new MedicineTypeImportADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MedicineTypeImportADO>(mediAdo, item);
                    if (!string.IsNullOrEmpty(item.PARENT_CODE))
                    {
                        if (item.PARENT_CODE.Length > 25)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "PARENT_CODE");
                        }
                        var getData = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.MATERIAL_TYPE_CODE == item.PARENT_CODE);
                        if (getData != null)
                        {
                            mediAdo.PARENT_ID = getData.ID;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "PARENT_CODE");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.STOP_IMP))
                    {
                        if (item.STOP_IMP == "x")
                        {
                            mediAdo.IS_STOP_IMP = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "STOP_IMP");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.ANTIBIOTIC))
                    {
                        if (item.ANTIBIOTIC == "x")
                        {
                            mediAdo.IS_ANTIBIOTIC = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "ANTIBIOTIC");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.ALLOW_ODD))
                    {
                        if (item.ALLOW_ODD == "x")
                        {
                            mediAdo.IS_ALLOW_ODD = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "ALLOW_ODD");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.REQUIRE_HSD))
                    {
                        if (item.REQUIRE_HSD == "x")
                        {
                            mediAdo.IS_REQUIRE_HSD = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "REQUIRE_HSD");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.BUSINESS))
                    {
                        if (item.BUSINESS == "x")
                        {
                            mediAdo.IS_BUSINESS = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "BUSINESS");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.ALLOW_EXPORT_ODD))
                    {
                        if (item.ALLOW_EXPORT_ODD == "x")
                        {
                            mediAdo.IS_ALLOW_EXPORT_ODD = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "ALLOW_EXPORT_ODD");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.OUT_PARENT_FEE))
                    {
                        if (item.OUT_PARENT_FEE == "x")
                        {
                            mediAdo.IS_OUT_PARENT_FEE = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "CPNG");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.ADDICTIVE))
                    {
                        if (item.ADDICTIVE == "x")
                        {
                            mediAdo.IS_ADDICTIVE = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "ADDICTIVE");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.STAR_MARK))
                    {
                        if (item.STAR_MARK == "x")
                        {
                            mediAdo.IS_STAR_MARK = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "STAR_MARK");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.FUNCTIONAL_FOOD))
                    {
                        if (item.FUNCTIONAL_FOOD == "x")
                        {
                            mediAdo.IS_FUNCTIONAL_FOOD = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "FUNCTIONAL_FOOD");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.NEUROLOGICAL))
                    {
                        if (item.NEUROLOGICAL == "x")
                        {
                            mediAdo.IS_NEUROLOGICAL = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "NEUROLOGICAL");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.SALE_EQUAL_IMP_PRICE))
                    {
                        if (item.SALE_EQUAL_IMP_PRICE == "x")
                        {
                            mediAdo.IS_SALE_EQUAL_IMP_PRICE = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "SALE_EQUAL_IMP_PRICE");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_SERVICE_TYPE_CODE))
                    {
                        if (item.HEIN_SERVICE_TYPE_CODE.Length > 10)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "HEIN_SERVICE_TYPE_CODE");
                        }
                        var getData = BackendDataWorker.Get<HIS_HEIN_SERVICE_TYPE>().FirstOrDefault(o => o.HEIN_SERVICE_TYPE_CODE == item.HEIN_SERVICE_TYPE_CODE);
                        if (getData != null)
                        {
                            mediAdo.HEIN_SERVICE_TYPE_ID = getData.ID;
                            mediAdo.HEIN_SERVICE_TYPE_NAME = getData.HEIN_SERVICE_TYPE_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "HEIN_SERVICE_TYPE_CODE");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.SERVICE_UNIT_CODE))
                    {
                        if (item.SERVICE_UNIT_CODE.Length > 3)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "SERVICE_UNIT_CODE");
                        }
                        var getData = BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(o => o.SERVICE_UNIT_CODE == item.SERVICE_UNIT_CODE);
                        if (getData != null)
                        {
                            mediAdo.SERVICE_UNIT_ID = getData.ID;
                            mediAdo.SERVICE_UNIT_NAME = getData.SERVICE_UNIT_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "SERVICE_UNIT_CODE");
                        }

                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "SERVICE_UNIT_CODE");
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_IN_TIME_STR))
                    {
                        long? dateTime = null;
                        string check = "";
                        convertDateStringToTimeNumber(item.HEIN_LIMIT_PRICE_IN_TIME_STR, ref dateTime, ref check);
                        if (dateTime != null && string.IsNullOrEmpty(check))
                        {
                            mediAdo.HEIN_LIMIT_PRICE_IN_TIME = dateTime;
                        }
                        else
                        {
                            error += string.Format(check, "HEIN_LIMIT_PRICE_IN_TIME_STR");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_INTR_TIME_STR))
                    {
                        long? dateTime = null;
                        string check = "";
                        convertDateStringToTimeNumber(item.HEIN_LIMIT_PRICE_INTR_TIME_STR, ref dateTime, ref check);
                        if (dateTime != null && string.IsNullOrEmpty(check))
                        {
                            mediAdo.HEIN_LIMIT_PRICE_INTR_TIME = dateTime;
                        }
                        else
                        {
                            error += string.Format(check, "HEIN_LIMIT_PRICE_INTR_TIME_STR");
                        }
                    }

                    if (item.NUM_ORDER.HasValue)
                    {
                        if (item.NUM_ORDER.ToString().Length > 19 || item.NUM_ORDER < 0)
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "NUM_ORDER");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_SERVICE_BHYT_CODE))
                    {
                        if (item.HEIN_SERVICE_BHYT_CODE.Length > 20)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "HEIN_SERVICE_BHYT_CODE");
                        }
                        if (string.IsNullOrEmpty(item.HEIN_SERVICE_BHYT_NAME))
                        {
                            error += string.Format(Message.MessageImport.CoThiPhaiNhap, "HEIN_SERVICE_BHYT_CODE", "HEIN_SERVICE_BHYT_NAME");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_SERVICE_BHYT_NAME))
                    {
                        if (item.HEIN_SERVICE_BHYT_NAME.Length > 500)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "HEIN_SERVICE_BHYT_NAME");
                        }
                        if (string.IsNullOrEmpty(item.HEIN_SERVICE_BHYT_CODE))
                        {
                            error += string.Format(Message.MessageImport.CoThiPhaiNhap, "HEIN_SERVICE_BHYT_NAME", "HEIN_SERVICE_BHYT_CODE");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_ORDER))
                    {
                        if (item.HEIN_ORDER.Length > 20)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "HEIN_ORDER");
                        }
                    }


                    if (item.HEIN_LIMIT_RATIO.HasValue)
                    {
                        if (item.HEIN_LIMIT_RATIO.Value > 1 || item.HEIN_LIMIT_RATIO < 0)
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "HEIN_LIMIT_RATIO");
                        }
                    }

                    if (item.HEIN_LIMIT_RATIO_OLD.HasValue)
                    {
                        if (item.HEIN_LIMIT_RATIO_OLD.Value > 1 || item.HEIN_LIMIT_RATIO_OLD < 0)
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "HEIN_LIMIT_RATIO_OLD");
                        }
                    }

                    if (item.HEIN_LIMIT_PRICE_OLD.HasValue)
                    {
                        if (item.HEIN_LIMIT_PRICE_OLD.Value > 99999999999999 || item.HEIN_LIMIT_PRICE_OLD < 0)
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "HEIN_LIMIT_PRICE_OLD");
                        }
                    }

                    if (item.IMP_VAT_RATIO.HasValue)
                    {
                        if (item.IMP_VAT_RATIO.Value > 1 || item.IMP_VAT_RATIO < 0)
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "IMP_VAT_RATIO");
                        }
                    }

                    if (item.INTERNAL_PRICE.HasValue)
                    {
                        if (item.INTERNAL_PRICE.Value > 99999999999999 || item.INTERNAL_PRICE < 0)
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "INTERNAL_PRICE");
                        }
                    }

                    if (item.IMP_PRICE.HasValue)
                    {
                        if (item.IMP_PRICE.Value > 99999999999999 || item.IMP_PRICE < 0)
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "IMP_PRICE");
                        }
                    }

                    if (item.ALERT_EXPIRED_DATE.HasValue)
                    {
                        if ((item.ALERT_EXPIRED_DATE ?? 0) > 999999999999999999 || (item.ALERT_EXPIRED_DATE ?? 0) < 0)
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "ALERT_EXPIRED_DATE");
                        }
                    }

                    if (item.ALERT_MIN_IN_STOCK.HasValue)
                    {
                        if ((item.ALERT_MIN_IN_STOCK ?? 0) > 9999999999999999 || (item.ALERT_MIN_IN_STOCK ?? 0) < 0)
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "ALERT_MIN_IN_STOCK");
                        }
                    }

                    if (item.HEIN_LIMIT_PRICE.HasValue)
                    {
                        if (item.HEIN_LIMIT_PRICE.Value > 99999999999999 || item.HEIN_LIMIT_PRICE < 0)
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "HEIN_LIMIT_PRICE");
                        }
                    }

                    if ((item.HEIN_LIMIT_PRICE.HasValue || item.HEIN_LIMIT_PRICE_OLD.HasValue) && (item.HEIN_LIMIT_RATIO.HasValue || item.HEIN_LIMIT_RATIO_OLD.HasValue))
                    {
                        error += string.Format(Message.MessageImport.ChiDuocNhapGiaHoacTiLeTran);
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_IN_TIME_STR) && !string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_INTR_TIME_STR))
                    {
                        error += string.Format(Message.MessageImport.ChiDuocNhapTGVaoVienHoacTGChiDinh);
                    }

                    if (!string.IsNullOrEmpty(item.MEDICINE_TYPE_CODE))
                    {
                        if (item.MEDICINE_TYPE_CODE.Length > 25)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "MEDICINE_TYPE_CODE");
                        }

                        var check = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Exists(o => o.MEDICINE_TYPE_CODE == item.MEDICINE_TYPE_CODE);
                        if (check)
                        {
                            error += string.Format(Message.MessageImport.DaTonTai, "MEDICINE_TYPE_CODE");
                        }

                        var checkExel = _medicineRef.FirstOrDefault(o => o.MEDICINE_TYPE_CODE == item.MEDICINE_TYPE_CODE);
                        if (checkExel != null)
                        {
                            error += string.Format(Message.MessageImport.TonTaiTrungNhauTrongFileImport, "MEDICINE_TYPE_CODE");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "MEDICINE_TYPE_CODE");
                    }

                    if (!string.IsNullOrEmpty(item.MEDICINE_TYPE_NAME))
                    {
                        if (item.MEDICINE_TYPE_NAME.Length > 500)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "MEDICINE_TYPE_NAME");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "MEDICINE_TYPE_NAME");
                    }

                    if (!string.IsNullOrEmpty(item.PACKING_TYPE_CODE))
                    {
                        if (item.PACKING_TYPE_CODE.Length > 6)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "PACKING_TYPE_CODE");
                        }
                        var package = BackendDataWorker.Get<HIS_PACKING_TYPE>().FirstOrDefault(o => o.PACKING_TYPE_CODE == item.PACKING_TYPE_CODE);
                        if (package != null)
                        {
                            mediAdo.PACKING_TYPE_ID = package.ID;
                            mediAdo.PACKING_TYPE_NAME = package.PACKING_TYPE_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "PACKING_TYPE_CODE");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.NATIONAL_NAME))
                    {
                        if (item.NATIONAL_NAME.Length > 100)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "NATIONAL_NAME");
                        }
                        var national = BackendDataWorker.Get<SDA_NATIONAL>().FirstOrDefault(o => o.NATIONAL_NAME == item.NATIONAL_NAME);
                        if (national != null)
                        {
                            item.NATIONAL_NAME = national.NATIONAL_NAME;
                        }
                        else
                            error += string.Format(Message.MessageImport.KhongHopLe, "NATIONAL_NAME");
                    }

                    if (!string.IsNullOrEmpty(item.MANUFACTURER_CODE))
                    {
                        if (item.MANUFACTURER_CODE.Length > 6)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "MANUFACTURER_CODE");
                        }
                        var package = BackendDataWorker.Get<HIS_MANUFACTURER>().FirstOrDefault(o => o.MANUFACTURER_CODE == item.MANUFACTURER_CODE);
                        if (package != null)
                        {
                            mediAdo.MANUFACTURER_ID = package.ID;
                            mediAdo.MANUFACTURER_NAME = package.MANUFACTURER_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "MANUFACTURER_CODE");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.DESCRIPTION))
                    {
                        if (item.DESCRIPTION.Length > 2000)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "DESCRIPTION");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.CONCENTRA))
                    {
                        if (item.CONCENTRA.Length > 100)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "CONCENTRA");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.MEDICINE_TYPE_PROPRIETARY_NAME))
                    {
                        if (item.MEDICINE_TYPE_PROPRIETARY_NAME.Length > 200)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "MEDICINE_TYPE_PROPRIETARY_NAME");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.TUTORIAL))
                    {
                        if (item.TUTORIAL.Length > 2000)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "TUTORIAL");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.ACTIVE_INGR_BHYT_CODE))
                    {
                        if (item.ACTIVE_INGR_BHYT_CODE.Length > 20)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "ACTIVE_INGR_BHYT_CODE");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.ACTIVE_INGR_BHYT_NAME))
                    {
                        if (item.ACTIVE_INGR_BHYT_NAME.Length > 1000)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "ACTIVE_INGR_BHYT_NAME");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.REGISTER_NUMBER))
                    {
                        if (item.REGISTER_NUMBER.Length > 100)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "REGISTER_NUMBER");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.TCY_NUM_ORDER))
                    {
                        if (item.TCY_NUM_ORDER.Length > 20)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "TCY_NUM_ORDER");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.BYT_NUM_ORDER))
                    {
                        if (item.BYT_NUM_ORDER.Length > 50)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "BYT_NUM_ORDER");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.MEDICINE_USE_FORM_CODE))
                    {
                        if (item.MEDICINE_USE_FORM_CODE.Length > 6)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "MEDICINE_USE_FORM_CODE");
                        }
                        var package = BackendDataWorker.Get<HIS_MEDICINE_USE_FORM>().FirstOrDefault(o => o.MEDICINE_USE_FORM_CODE == item.MEDICINE_USE_FORM_CODE);
                        if (package != null)
                        {
                            mediAdo.MEDICINE_USE_FORM_ID = package.ID;
                            mediAdo.MEDICINE_USE_FORM_NAME = package.MEDICINE_USE_FORM_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "MEDICINE_USE_FORM_CODE");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "MEDICINE_USE_FORM_CODE");
                    }

                    if (!string.IsNullOrEmpty(item.MEDICINE_LINE_CODE))
                    {
                        if (item.MEDICINE_LINE_CODE.Length > 2)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "MEDICINE_LINE_CODE");
                        }
                        var package = BackendDataWorker.Get<HIS_MEDICINE_LINE>().FirstOrDefault(o => o.MEDICINE_LINE_CODE == item.MEDICINE_LINE_CODE);
                        if (package != null)
                        {
                            mediAdo.MEDICINE_LINE_ID = package.ID;
                            mediAdo.MEDICINE_LINE_NAME = package.MEDICINE_LINE_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "MEDICINE_LINE_CODE");
                        }
                    }

                    if (item.ALERT_MAX_IN_TREATMENT.HasValue)
                    {
                        if ((item.ALERT_MAX_IN_TREATMENT ?? 0) > 9999999999999999 || (item.ALERT_MAX_IN_TREATMENT ?? 0) < 0)
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "ALERT_MAX_IN_TREATMENT");
                        }
                    }

                    if (item.USE_ON_DAY.HasValue)
                    {
                        if ((item.USE_ON_DAY ?? 0) > 9999999999999999 || (item.USE_ON_DAY ?? 0) < 0)
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "USE_ON_DAY");
                        }
                    }

                    if (item.COGS.HasValue)
                    {
                        if ((item.COGS ?? 0) > 99999999999999 || (item.COGS ?? 0) < 0)
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "COGS");
                        }
                    }

                    mediAdo.ERROR = error;
                    mediAdo.ID = i;

                    _medicineRef.Add(mediAdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                WaitingManager.Show();

                List<HIS_MEDICINE_TYPE> listMedi = new List<HIS_MEDICINE_TYPE>();

                foreach (var item in medicineTypeAdos)
                {
                    HIS_MEDICINE_TYPE medi = new HIS_MEDICINE_TYPE();
                    HIS_SERVICE ser = new HIS_SERVICE();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_MEDICINE_TYPE>(medi, item);
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE>(ser, item);
                    ser.SERVICE_UNIT_ID = item.SERVICE_UNIT_ID;
                    //ser.SERVICE_TYPE_ID = item.SERVICE_TYPE_ID;
                    medi.HIS_SERVICE = ser;
                    listMedi.Add(medi);
                }
                CommonParam param = new CommonParam();

                if (listMedi != null && listMedi.Count > 0)
                {
                    var rs = new BackendAdapter(param).Post<List<HIS_MEDICINE_TYPE>>("api/HisMedicineType/CreateList", ApiConsumers.MosConsumer, listMedi, param);
                    if (rs != null)
                    {
                        success = true;
                    }
                }
                WaitingManager.Hide();
                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Btn_Show_Error_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (MedicineTypeImportADO)gridViewMedicineType.GetFocusedRow();
                if (row != null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(row.ERROR);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Btn_Delete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (MedicineTypeImportADO)gridViewMedicineType.GetFocusedRow();
                if (row != null)
                {
                    if (medicineTypeAdos != null && medicineTypeAdos.Count > 0)
                    {
                        medicineTypeAdos.Remove(row);
                        SetDataSource(medicineTypeAdos);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnShowLineError_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnShowLineError.Text == "Dòng lỗi")
                {
                    btnShowLineError.Text = "Dòng không lỗi";
                    var errorLine = medicineTypeAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = medicineTypeAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewMedicineType_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    MedicineTypeImportADO data = (MedicineTypeImportADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "ErrorLine")
                    {
                        if (!string.IsNullOrEmpty(data.ERROR))
                        {
                            e.RepositoryItem = Btn_ErrorLine;
                        }
                    }
                    else if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = Btn_Delete;
                    }
                    else if (e.Column.FieldName == "CPNG")
                    {
                        e.RepositoryItem = Item_Check;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewMedicineType_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MedicineTypeImportADO pData = (MedicineTypeImportADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(pData.CREATE_TIME.ToString()));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                        }
                    }

                    else if (e.Column.FieldName == "HEIN_LIMIT_PRICE_IN_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(pData.HEIN_LIMIT_PRICE_IN_TIME.ToString()));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao HEIN_LIMIT_PRICE_IN_TIME_STR", ex);
                        }
                    }

                    else if (e.Column.FieldName == "HEIN_LIMIT_PRICE_INTR_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(pData.HEIN_LIMIT_PRICE_INTR_TIME.ToString()));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao HEIN_LIMIT_PRICE_INTR_TIME_STR", ex);
                        }
                    }

                    else if (e.Column.FieldName == "HEIN_LIMIT_RATIO_STR")
                    {
                        try
                        {
                            e.Value = pData.HEIN_LIMIT_RATIO;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua CPNG", ex);
                        }
                    }

                    else if (e.Column.FieldName == "HEIN_LIMIT_RATIO_OLD_STR")
                    {
                        try
                        {
                            e.Value = pData.HEIN_LIMIT_RATIO_OLD;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua CPNG", ex);
                        }
                    }
                    else if (e.Column.FieldName == "KinhDoanh")
                    {
                        try
                        {
                            e.Value = pData.IS_BUSINESS == 1 ? true : false;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua KinhDoanh", ex);
                        }
                    }
                    else if (e.Column.FieldName == "BanBangGiaNhap")
                    {
                        try
                        {
                            e.Value = pData.IS_SALE_EQUAL_IMP_PRICE == 1 ? true : false;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua BBGiaNhap", ex);
                        }
                    }
                    else if (e.Column.FieldName == "BBNhapHSD")
                    {
                        try
                        {
                            e.Value = pData.IS_REQUIRE_HSD == 1 ? true : false;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua BBNhapHSD", ex);
                        }
                    }
                    else if (e.Column.FieldName == "DungNhap")
                    {
                        try
                        {
                            e.Value = pData.IS_STOP_IMP == 1 ? true : false;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua DungNhap", ex);
                        }
                    }
                    else if (e.Column.FieldName == "KhoXuatLe")
                    {
                        try
                        {
                            e.Value = pData.IS_ALLOW_EXPORT_ODD == 1 ? true : false;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua KhoXuatLe", ex);
                        }
                    }
                    else if (e.Column.FieldName == "KeLe")
                    {
                        try
                        {
                            e.Value = pData.IS_ALLOW_ODD == 1 ? true : false;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua KeLe", ex);
                        }
                    }
                    else if (e.Column.FieldName == "CPNG")
                    {
                        try
                        {
                            e.Value = pData.IS_OUT_PARENT_FEE == 1 ? true : false;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua CPNG", ex);
                        }
                    }

                    else if (e.Column.FieldName == "ADDICTIVE_STR")
                    {
                        try
                        {
                            e.Value = pData.IS_ADDICTIVE == 1 ? true : false;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua ADDICTIVE_STR", ex);
                        }
                    }

                    else if (e.Column.FieldName == "NEUROLOGICAL_STR")
                    {
                        try
                        {
                            e.Value = pData.IS_NEUROLOGICAL == 1 ? true : false;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua NEUROLOGICAL_STR", ex);
                        }
                    }

                    else if (e.Column.FieldName == "ANTIBIOTIC_STR")
                    {
                        try
                        {
                            e.Value = pData.IS_ANTIBIOTIC == 1 ? true : false;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua ANTIBIOTIC_STR", ex);
                        }
                    }

                    else if (e.Column.FieldName == "FUNCTIONAL_FOOD_STR")
                    {
                        try
                        {
                            e.Value = pData.IS_FUNCTIONAL_FOOD == 1 ? true : false;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua FUNCTIONAL_FOOD_STR", ex);
                        }
                    }

                    else if (e.Column.FieldName == "STAR_MARK_STR")
                    {
                        try
                        {
                            e.Value = pData.IS_STAR_MARK == 1 ? true : false;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua STAR_MARK_STR", ex);
                        }
                    }
                    //else if (e.Column.FieldName == "ACTIVE_ITEM")
                    //{
                    //    try
                    //    {
                    //        if (status == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    //            e.Value = Inventec.Common.Resource.Get.Value("frmHisMisuMedicineTypeType.HoatDong", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    //        else
                    //            e.Value = Inventec.Common.Resource.Get.Value("frmHisMisuMedicineTypeType.TamKhoa", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Inventec.Common.Logging.LogSystem.Error(ex);
                    //    }
                    //}
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(pData.MODIFY_TIME.ToString()));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua MODIFY_TIME", ex);
                        }
                    }

                    else if (e.Column.FieldName == "BILL_OPTION_STR")
                    {
                        try
                        {
                            if (pData.BILL_OPTION == null)
                                e.Value = "Hóa đơn thường";
                            else if (pData.BILL_OPTION == 1)
                                e.Value = "Tách chênh lệch vào hóa đơn dịch vụ";
                            else if (pData.BILL_OPTION == 2)
                                e.Value = "Hóa đơn dịch vụ";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua BILL_OPTION", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmMedicineType_Load(object sender, EventArgs e)
        {
            try
            {
                medicineTypeAdos = new List<MedicineTypeImportADO>();
                addMedicineTypeToProcessList(currentAdos, ref medicineTypeAdos);
                SetDataSource(medicineTypeAdos);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}
