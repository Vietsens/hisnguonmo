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
    public partial class frmMaterialType : Form
    {
        List<MaterialTypeImportADO> materialTypeAdos;
        List<MaterialTypeImportADO> currentAdos;
        RefeshReference delegateRefresh;

        public frmMaterialType(List<MaterialTypeImportADO> data, RefeshReference _delegate)
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

        private void CheckErrorLine(List<MaterialTypeImportADO> dataSource)
        {
            try
            {
                var checkError = materialTypeAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
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

        private void SetDataSource(List<MaterialTypeImportADO> dataSource)
        {
            try
            {
                gridControlMaterialType.BeginUpdate();
                gridControlMaterialType.DataSource = null;
                gridControlMaterialType.DataSource = dataSource;
                gridControlMaterialType.EndUpdate();
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

        private void addMaterialTypeToProcessList(List<MaterialTypeImportADO> _material, ref List<MaterialTypeImportADO> _materialRef)
        {
            try
            {
                _materialRef = new List<MaterialTypeImportADO>();
                long i = 0;
                foreach (var item in _material)
                {
                    i++;
                    string error = "";
                    var mateAdo = new MaterialTypeImportADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MaterialTypeImportADO>(mateAdo, item);
                    if (!string.IsNullOrEmpty(item.PARENT_CODE))
                    {
                        if (item.PARENT_CODE.Length > 25)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "PARENT_CODE");
                        }
                        var getData = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.MATERIAL_TYPE_CODE == item.PARENT_CODE);
                        if (getData != null)
                        {
                            mateAdo.PARENT_ID = getData.ID;
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
                            mateAdo.IS_STOP_IMP = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "STOP_IMP");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.IN_KTC_FEE))
                    {
                        if (item.IN_KTC_FEE == "x")
                        {
                            mateAdo.IS_IN_KTC_FEE = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "IN_KTC_FEE");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.ALLOW_ODD))
                    {
                        if (item.ALLOW_ODD == "x")
                        {
                            mateAdo.IS_ALLOW_ODD = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "ALLOW_ODD");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.STENT))
                    {
                        if (item.STENT == "x")
                        {
                            mateAdo.IS_STENT = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "STENT");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.REQUIRE_HSD))
                    {
                        if (item.REQUIRE_HSD == "x")
                        {
                            mateAdo.IS_REQUIRE_HSD = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "REQUIRE_HSD");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.CHEMICAL_SUBSTANCE))
                    {
                        if (item.CHEMICAL_SUBSTANCE == "x")
                        {
                            mateAdo.IS_CHEMICAL_SUBSTANCE = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "CHEMICAL_SUBSTANCE");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.BUSINESS))
                    {
                        if (item.BUSINESS == "x")
                        {
                            mateAdo.IS_BUSINESS = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "BUSINESS");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.AUTO_EXPEND))
                    {
                        if (item.AUTO_EXPEND == "x")
                        {
                            mateAdo.IS_AUTO_EXPEND = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "AUTO_EXPEND");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.ALLOW_EXPORT_ODD))
                    {
                        if (item.ALLOW_EXPORT_ODD == "x")
                        {
                            mateAdo.IS_ALLOW_EXPORT_ODD = 1;
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
                            mateAdo.IS_OUT_PARENT_FEE = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "CPNG");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.SALE_EQUAL_IMP_PRICE))
                    {
                        if (item.SALE_EQUAL_IMP_PRICE == "x")
                        {
                            mateAdo.IS_SALE_EQUAL_IMP_PRICE = 1;
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
                            mateAdo.HEIN_SERVICE_TYPE_ID = getData.ID;
                            mateAdo.HEIN_SERVICE_TYPE_NAME = getData.HEIN_SERVICE_TYPE_NAME;
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
                            mateAdo.SERVICE_UNIT_ID = getData.ID;
                            mateAdo.SERVICE_UNIT_NAME = getData.SERVICE_UNIT_NAME;
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
                            mateAdo.HEIN_LIMIT_PRICE_IN_TIME = dateTime;
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
                            mateAdo.HEIN_LIMIT_PRICE_INTR_TIME = dateTime;
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
                        if (item.HEIN_LIMIT_RATIO_OLD.Value > 1 || item.HEIN_LIMIT_RATIO < 0)
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

                    if (!string.IsNullOrEmpty(item.MATERIAL_TYPE_CODE))
                    {
                        if (item.MATERIAL_TYPE_CODE.Length > 25)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "MATERIAL_TYPE_CODE");
                        }

                        var check = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Exists(o => o.MATERIAL_TYPE_CODE == item.MATERIAL_TYPE_CODE);
                        if (check)
                        {
                            error += string.Format(Message.MessageImport.DaTonTai, "MATERIAL_TYPE_CODE");
                        }
                        var checkExel = _materialRef.FirstOrDefault(o => o.MATERIAL_TYPE_CODE == item.MATERIAL_TYPE_CODE);
                        if (checkExel != null)
                        {
                            error += string.Format(Message.MessageImport.TonTaiTrungNhauTrongFileImport, "MATERIAL_TYPE_CODE");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "MATERIAL_TYPE_CODE");
                    }

                    if (!string.IsNullOrEmpty(item.MATERIAL_TYPE_NAME))
                    {
                        if (item.MATERIAL_TYPE_NAME.Length > 500)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "MATERIAL_TYPE_NAME");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "MATERIAL_TYPE_NAME");
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
                            mateAdo.PACKING_TYPE_ID = package.ID;
                            mateAdo.PACKING_TYPE_NAME = package.PACKING_TYPE_NAME;
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
                            mateAdo.PACKING_TYPE_ID = package.ID;
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

                    if (item.COGS.HasValue)
                    {
                        if ((item.COGS ?? 0) > 99999999999999 || (item.COGS ?? 0) < 0)
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "COGS");
                        }
                    }

                    mateAdo.ERROR = error;
                    mateAdo.ID = i;

                    _materialRef.Add(mateAdo);
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

                List<HIS_MATERIAL_TYPE> listMater = new List<HIS_MATERIAL_TYPE>();

                foreach (var item in materialTypeAdos)
                {
                    HIS_MATERIAL_TYPE mater = new HIS_MATERIAL_TYPE();
                    HIS_SERVICE ser = new HIS_SERVICE();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_MATERIAL_TYPE>(mater, item);
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE>(ser, item);
                    ser.SERVICE_UNIT_ID = item.SERVICE_UNIT_ID;
                    //ser.SERVICE_TYPE_ID = item.SERVICE_TYPE_ID;
                    mater.HIS_SERVICE = ser;
                    listMater.Add(mater);
                }
                CommonParam param = new CommonParam();

                if (listMater != null && listMater.Count > 0)
                {
                    var rs = new BackendAdapter(param).Post<List<HIS_MATERIAL_TYPE>>("api/HisMaterialType/CreateList", ApiConsumers.MosConsumer, listMater, param);
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
                var row = (MaterialTypeImportADO)gridViewMaterialType.GetFocusedRow();
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
                var row = (MaterialTypeImportADO)gridViewMaterialType.GetFocusedRow();
                if (row != null)
                {
                    if (materialTypeAdos != null && materialTypeAdos.Count > 0)
                    {
                        materialTypeAdos.Remove(row);
                        SetDataSource(materialTypeAdos);
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
                    var errorLine = materialTypeAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = materialTypeAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewMaterialType_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    MaterialTypeImportADO data = (MaterialTypeImportADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
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

        private void gridViewMaterialType_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MaterialTypeImportADO pData = (MaterialTypeImportADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                    else if (e.Column.FieldName == "KiThuatCao")
                    {
                        try
                        {
                            e.Value = pData.IS_IN_KTC_FEE == 1 ? true : false;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua KiThuatCao", ex);
                        }
                    }
                    else if (e.Column.FieldName == "Stent")
                    {
                        try
                        {
                            e.Value = pData.IS_STENT == 1 ? true : false;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua Stent", ex);
                        }
                    }
                    else if (e.Column.FieldName == "HaoPhi")
                    {
                        try
                        {
                            e.Value = pData.IS_AUTO_EXPEND == 1 ? true : false;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua HaoPhi", ex);
                        }
                    }
                    else if (e.Column.FieldName == "HoaChat")
                    {
                        try
                        {
                            e.Value = pData.IS_CHEMICAL_SUBSTANCE == 1 ? true : false;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua HoaChat", ex);
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
                    //else if (e.Column.FieldName == "ACTIVE_ITEM")
                    //{
                    //    try
                    //    {
                    //        if (status == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    //            e.Value = Inventec.Common.Resource.Get.Value("frmHisMisuMaterialTypeType.HoatDong", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    //        else
                    //            e.Value = Inventec.Common.Resource.Get.Value("frmHisMisuMaterialTypeType.TamKhoa", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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

        private void frmMaterialType_Load(object sender, EventArgs e)
        {
            try
            {
                materialTypeAdos = new List<MaterialTypeImportADO>();
                addMaterialTypeToProcessList(currentAdos, ref materialTypeAdos);
                SetDataSource(materialTypeAdos);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}
