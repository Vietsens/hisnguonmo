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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.HisImportServicePaty.ADO;
using HIS.Desktop.Plugins.HisImportServicePaty.Config;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisImportServicePaty.FormLoad
{
    public partial class frmServicePaty : HIS.Desktop.Utility.FormBase
    {
        List<ServicePatyImportADO> servicePatyAdos;
        List<ServicePatyImportADO> currentAdos;
        RefeshReference delegateRefresh;
        Inventec.Desktop.Common.Modules.Module currentModule;
        bool checkClick;

        public frmServicePaty(Inventec.Desktop.Common.Modules.Module module, RefeshReference _delegate)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
                this.delegateRefresh = _delegate;

                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmServicePaty(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;

                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckErrorLine(List<ServicePatyImportADO> dataSource)
        {
            try
            {
                if (servicePatyAdos != null && servicePatyAdos.Count > 0)
                {
                    var checkError = servicePatyAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
                    if (!checkError)
                    {
                        btnSave.Enabled = true;
                    }
                    else
                    {
                        btnSave.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetDataSource(List<ServicePatyImportADO> dataSource)
        {
            try
            {
                gridControlServicePaty.BeginUpdate();
                gridControlServicePaty.DataSource = null;
                gridControlServicePaty.DataSource = dataSource;
                gridControlServicePaty.EndUpdate();
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

        private void addServicePatyToProcessList(List<ServicePatyImportADO> _service, ref List<ServicePatyImportADO> _serviceRef)
        {
            try
            {
                _serviceRef = new List<ServicePatyImportADO>();
                long i = 0;
                foreach (var item in _service)
                {
                    i++;
                    string error = "";
                    var mateAdo = new ServicePatyImportADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ServicePatyImportADO>(mateAdo, item);

                    if (!string.IsNullOrEmpty(item.PRICE_STR))
                    {
                        if (checkNumber(item.PRICE_STR))
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Giá");
                        }
                        else
                        {
                            var price = Inventec.Common.TypeConvert.Parse.ToDecimal(item.PRICE_STR);
                            if (price > 99999999999999 || price < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Giá");
                            }
                            else
                                mateAdo.PRICE = price;
                        }
                    }
                    else
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Giá");

                    if (!string.IsNullOrEmpty(item.VAT_RATIO_STR))
                    {
                        if (checkNumber(item.VAT_RATIO_STR))
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "VAT");
                        }
                        else
                        {
                            var price = Inventec.Common.TypeConvert.Parse.ToDecimal(item.VAT_RATIO_STR);
                            if (price > 1 || price < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "VAT");
                            }
                            else
                                mateAdo.VAT_RATIO = price;
                        }
                    }
                    else
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "VAT");
                    /// mới làm 
                    if (!string.IsNullOrEmpty(item.SERVICE_RATIO_STR))
                    {
                        if (checkNumber(item.SERVICE_RATIO_STR))
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "tỉ lệ thanh toán");
                        }
                        else
                        {
                            var price = Inventec.Common.TypeConvert.Parse.ToDecimal(item.SERVICE_RATIO_STR);
                            if (price < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "tỉ lệ thanh toán");
                            }
                            else
                                mateAdo.SERVICE_RATIO = price;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.SERVICE_CODE))
                    {
                        if (item.SERVICE_CODE.Length > 25)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã dịch vụ");
                        }

                        //if (mateAdo.SERVICE_TYPE_ID > 0)
                        //{
                        //var serviceGet = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.SERVICE_CODE == item.SERVICE_CODE && o.SERVICE_TYPE_ID == mateAdo.SERVICE_TYPE_ID);
                        var serviceGet = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.SERVICE_CODE == item.SERVICE_CODE);
                        if (serviceGet != null)
                        {
                            if (serviceGet.IS_ACTIVE == 1)
                            {
                                mateAdo.SERVICE_ID = serviceGet.ID;
                                mateAdo.SERVICE_NAME = serviceGet.SERVICE_NAME;
                            }
                            else
                            {
                                error += string.Format(Message.MessageImport.DaBiKhoa, "Mã dịch vụ");
                            }

                            if (serviceGet.IS_LEAF != 1)
                            {
                                error += string.Format(Message.MessageImport.PhaiLaLa, "Mã dịch vụ");
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã dịch vụ");
                        }
                        //}
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã dịch vụ");
                    }

                    if (!string.IsNullOrEmpty(item.SERVICE_CONDITION_CODE))
                    {
                        var serviceGet = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.SERVICE_CODE == item.SERVICE_CODE);
                        if (item.SERVICE_CONDITION_CODE.Length > 20)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã điều kiện");
                        }
                        if (serviceGet != null)
                        {
                            var serviceCondition = BackendDataWorker.Get<HIS_SERVICE_CONDITION>().FirstOrDefault(o => o.SERVICE_CONDITION_CODE == item.SERVICE_CONDITION_CODE && o.SERVICE_ID == serviceGet.ID);
                            if (serviceCondition != null)
                            {
                                if (serviceCondition.IS_ACTIVE == 1)
                                {
                                    mateAdo.SERVICE_CONDITION_ID = serviceCondition.ID;
                                    mateAdo.SERVICE_CONDITION_CODE = serviceCondition.SERVICE_CONDITION_CODE;
                                }
                                else
                                {
                                    error += string.Format(Message.MessageImport.DaBiKhoa, "Mã điều kiện");
                                }
                            }
                            else
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Mã điều kiện");
                            }
                        }
                        //}
                    }

                    if (!string.IsNullOrEmpty(item.PACKAGE_CODE))
                    {
                        if (item.PACKAGE_CODE.Length > 4)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã gói");
                        }

                        var Package = BackendDataWorker.Get<HIS_PACKAGE>().FirstOrDefault(o => o.PACKAGE_CODE == item.PACKAGE_CODE);
                        if (Package != null)
                        {
                            mateAdo.PACKAGE_ID = Package.ID;
                            mateAdo.PACKAGE_NAME = Package.PACKAGE_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã gói");
                        }
                    }


                    if (!string.IsNullOrEmpty(item.PATIENT_TYPE_CODE))
                    {
                        if (item.PATIENT_TYPE_CODE.Length > 6)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Đối tượng thanh toán");
                        }
                        var mater = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == item.PATIENT_TYPE_CODE);
                        if (mater != null)
                        {
                            mateAdo.PATIENT_TYPE_ID = mater.ID;
                            mateAdo.PATIENT_TYPE_NAME = mater.PATIENT_TYPE_NAME;
                            if (mater.ID == AppConfigs.patientTypeId_BHYT)
                            {
                                if (mateAdo.SERVICE_ID > 0)
                                {
                                    var serviceGet = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == mateAdo.SERVICE_ID);
                                    if (serviceGet != null)
                                    {
                                        if (serviceGet.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC && (string.IsNullOrEmpty(serviceGet.ACTIVE_INGR_BHYT_CODE) || string.IsNullOrEmpty(serviceGet.ACTIVE_INGR_BHYT_NAME))
                                            || serviceGet.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC && (string.IsNullOrEmpty(serviceGet.HEIN_SERVICE_BHYT_CODE) || string.IsNullOrEmpty(serviceGet.HEIN_SERVICE_BHYT_NAME)))
                                        {
                                            error += "Dịch vụ chưa cấu hình BHYT khi thiết lập giá cho đối tượng BHYT";
                                        }
                                        //if (serviceGet.HEIN_SERVICE_TYPE_ID == null)
                                        //{
                                        //    error += "Dịch vụ chưa cấu hình Loại dịch vụ BH khi thiết lập giá cho đối tượng BHYT";
                                        //}
                                    }
                                }
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Đối tượng thanh toán");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Đối tượng thanh toán");
                    }

                    if (!string.IsNullOrEmpty(item.BRANCH_CODE))
                    {
                        if (item.BRANCH_CODE.Length > 6)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Chi nhánh");
                        }
                        var branchGet = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.BRANCH_CODE == item.BRANCH_CODE);
                        if (branchGet != null)
                        {
                            mateAdo.BRANCH_ID = branchGet.ID;
                            mateAdo.BRANCH_NAME = branchGet.BRANCH_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Chi nhánh");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Chi nhánh");
                    }

                    if (!string.IsNullOrEmpty(item.OVERTIME_PRICE_STR))
                    {
                        if (checkNumber(item.OVERTIME_PRICE_STR))
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Giá chênh lệch");
                        }
                        else
                        {
                            var price = Inventec.Common.TypeConvert.Parse.ToDecimal(item.OVERTIME_PRICE_STR);
                            if (price > 99999999999999 || price < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Giá chênh lệch");
                            }
                            else if (price > mateAdo.PRICE && !string.IsNullOrEmpty(item.PRICE_STR))
                            {
                                error += string.Format(Message.MessageImport.PhaiLonHon, "Giá", "Giá chênh lệch");
                            }
                            else
                                mateAdo.OVERTIME_PRICE = price;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.PRIORITY_STR))
                    {
                        if (Inventec.Common.Number.Check.IsNumber(item.PRIORITY_STR))
                        {
                            mateAdo.PRIORITY = Inventec.Common.TypeConvert.Parse.ToInt64(item.PRIORITY_STR);
                            if ((mateAdo.PRIORITY ?? 0) > 999999999999999999 || (mateAdo.PRIORITY ?? 0) < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Độ ưu tiên");
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Độ ưu tiên");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.INTRUCTION_NUMBER_FROM_STR))
                    {
                        if (Inventec.Common.Number.Check.IsNumber(item.INTRUCTION_NUMBER_FROM_STR))
                        {
                            mateAdo.INTRUCTION_NUMBER_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(item.INTRUCTION_NUMBER_FROM_STR);
                            if ((mateAdo.INTRUCTION_NUMBER_FROM ?? 0) > 999999999999999999 || (mateAdo.INTRUCTION_NUMBER_FROM ?? 0) < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Từ lần chỉ định thứ");
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Từ lần chỉ định thứ");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.INTRUCTION_NUMBER_TO_STR))
                    {
                        if (Inventec.Common.Number.Check.IsNumber(item.INTRUCTION_NUMBER_TO_STR))
                        {
                            mateAdo.INTRUCTION_NUMBER_TO = Inventec.Common.TypeConvert.Parse.ToInt64(item.INTRUCTION_NUMBER_TO_STR);
                            if ((mateAdo.INTRUCTION_NUMBER_TO ?? 0) > 999999999999999999 || (mateAdo.INTRUCTION_NUMBER_TO ?? 0) < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Đến lần chỉ định thứ");
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Đến lần chỉ định thứ");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.FROM_TIME_STR))
                    {
                        long? dateTime = null;
                        string check = "";
                        convertDateStringToTimeNumber(item.FROM_TIME_STR, ref dateTime, ref check);
                        if (dateTime != null && string.IsNullOrEmpty(check))
                        {
                            mateAdo.FROM_TIME = dateTime;
                        }
                        else
                        {
                            error += string.Format(check, "Áp dụng từ");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.TO_TIME_STR))
                    {
                        long? dateTime = null;
                        string check = "";
                        convertDateStringToTimeNumber(item.TO_TIME_STR, ref dateTime, ref check);
                        if (dateTime != null && string.IsNullOrEmpty(check))
                        {
                            mateAdo.TO_TIME = dateTime;
                        }
                        else
                        {
                            error += string.Format(check, "Áp dụng đến");
                        }
                    }

                    if (mateAdo.FROM_TIME != null && mateAdo.TO_TIME != null)
                    {
                        if (mateAdo.FROM_TIME > mateAdo.TO_TIME)
                        {
                            error += string.Format(Message.MessageImport.PhaiLonHon, "Áp dụng đến", "Áp dụng từ");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.TREATMENT_FROM_TIME_STR))
                    {
                        long? dateTime = null;
                        string check = "";
                        convertDateStringToTimeNumber(item.TREATMENT_FROM_TIME_STR, ref dateTime, ref check);
                        if (dateTime != null && string.IsNullOrEmpty(check))
                        {
                            mateAdo.TREATMENT_FROM_TIME = dateTime;
                        }
                        else
                        {
                            error += string.Format(check, "Điều trị từ");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.TREATMENT_TO_TIME_STR))
                    {
                        long? dateTime = null;
                        string check = "";
                        convertDateStringToTimeNumber(item.TREATMENT_TO_TIME_STR, ref dateTime, ref check);
                        if (dateTime != null && string.IsNullOrEmpty(check))
                        {
                            mateAdo.TREATMENT_TO_TIME = dateTime;
                        }
                        else
                        {
                            error += string.Format(check, "Điều trị đến");
                        }
                    }

                    if (mateAdo.TREATMENT_FROM_TIME != null && mateAdo.TREATMENT_TO_TIME != null)
                    {
                        if (mateAdo.TREATMENT_FROM_TIME > mateAdo.TREATMENT_TO_TIME)
                        {
                            error += string.Format(Message.MessageImport.PhaiLonHon, "Điều trị đến", "Điều trị từ");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.DAY_FROM_STR))
                    {
                        if (Inventec.Common.Number.Check.IsNumber(item.DAY_FROM_STR))
                        {
                            mateAdo.DAY_FROM = Inventec.Common.TypeConvert.Parse.ToInt16(item.DAY_FROM_STR);
                            if ((mateAdo.DAY_FROM ?? 0) > 7 || (mateAdo.DAY_FROM ?? 0) < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Thứ từ");
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Thứ từ");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.DAY_TO_STR))
                    {
                        if (Inventec.Common.Number.Check.IsNumber(item.DAY_TO_STR))
                        {
                            mateAdo.DAY_TO = Inventec.Common.TypeConvert.Parse.ToInt16(item.DAY_TO_STR);
                            if ((mateAdo.DAY_TO ?? 0) > 7 || (mateAdo.DAY_TO ?? 0) < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Thứ đến");
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Thứ đến");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HOUR_FROM))
                    {
                        string erro = "";
                        CheckHour(item.HOUR_FROM, ref erro);
                        if (!string.IsNullOrEmpty(erro))
                        {
                            error += string.Format(erro, "Giờ từ");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HOUR_TO))
                    {
                        string erro = "";
                        CheckHour(item.HOUR_TO, ref erro);
                        if (!string.IsNullOrEmpty(erro))
                        {
                            error += string.Format(erro, "Giờ đến");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.EXECUTE_ROOM_CODES))
                    {
                        var split = item.EXECUTE_ROOM_CODES.Split(',');
                        string roomIds = "";
                        foreach (var sp in split)
                        {
                            var checkData = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ROOM_CODE == sp);
                            if (checkData == null)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Phòng thực hiện");
                                break;
                            }
                            else
                            {
                                if (checkData.IS_ACTIVE == 1)
                                {
                                    roomIds += checkData.ID.ToString() + ",";
                                }
                                else
                                {
                                    error += string.Format(Message.MessageImport.TonTaiDaBiKhoa, "Phòng thực hiện");
                                    break;
                                }
                            }
                        }
                        mateAdo.EXECUTE_ROOM_IDS = roomIds;
                    }

                    if (!string.IsNullOrEmpty(item.REQUEST_ROOM_CODES))
                    {
                        var split = item.REQUEST_ROOM_CODES.Split(',');
                        string roomIds = "";
                        foreach (var sp in split)
                        {
                            var checkData = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ROOM_CODE == sp);
                            if (checkData == null)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Phòng yêu cầu");
                                break;
                            }
                            else
                            {
                                if (checkData.IS_ACTIVE == 1)
                                {
                                    roomIds += checkData.ID.ToString() + ",";
                                }
                                else
                                {
                                    error += string.Format(Message.MessageImport.TonTaiDaBiKhoa, "Phòng yêu cầu");
                                    break;
                                }
                            }
                        }
                        mateAdo.REQUEST_ROOM_IDS = roomIds;
                    }

                    if (!string.IsNullOrEmpty(item.REQUEST_DEPARMENT_CODES))
                    {
                        var split = item.REQUEST_DEPARMENT_CODES.Split(',');
                        string departmentIds = "";
                        foreach (var sp in split)
                        {
                            var checkData = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.DEPARTMENT_CODE == sp);
                            if (checkData == null)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Khoa yêu cầu");
                                break;

                            }
                            else
                            {
                                if (checkData.IS_ACTIVE == 1)
                                {
                                    departmentIds += checkData.ID.ToString() + ",";
                                }
                                else
                                {
                                    error += string.Format(Message.MessageImport.TonTaiDaBiKhoa, "Khoa yêu cầu");
                                    break;
                                }
                            }
                        }

                        mateAdo.REQUEST_DEPARMENT_IDS = departmentIds;
                    }

                    mateAdo.ERROR = error;
                    mateAdo.ID = i;

                    _serviceRef.Add(mateAdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void CheckHour(string input, ref string check)
        {
            try
            {
                if (input.Length > 4)
                {
                    check = Message.MessageImport.KhongHopLe;
                    return;
                }
                else
                {
                    if (checkHour(input))
                    {
                        check = Message.MessageImport.KhongHopLe;
                        return;
                    }
                    else
                    {
                        var gio = input.Substring(0, 2);
                        var phut = input.Substring(2, 2);

                        if (Inventec.Common.TypeConvert.Parse.ToInt32(gio) > 24 || Inventec.Common.TypeConvert.Parse.ToInt32(gio) < 0)
                        {
                            check = Message.MessageImport.KhongHopLe;
                            return;
                        }
                        if (Inventec.Common.TypeConvert.Parse.ToInt32(phut) > 60 || Inventec.Common.TypeConvert.Parse.ToInt32(phut) < 0 || Inventec.Common.TypeConvert.Parse.ToInt32(phut) % 5 != 0)
                        {
                            check = Message.MessageImport.KhongHopLe;
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (!char.IsNumber(s[i]))
                        return result = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private bool checkNumber(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (!char.IsNumber(s[i]) && s[i] != ',')
                        return result = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private bool checkHour(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (!char.IsNumber(s[i]))
                        return result = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                WaitingManager.Show();
                AutoMapper.Mapper.CreateMap<ServicePatyImportADO, HIS_SERVICE_PATY>();
                var data = AutoMapper.Mapper.Map<List<HIS_SERVICE_PATY>>(servicePatyAdos);
                if (data != null && data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        item.ID = 0;
                        item.SERVICE_RATIO = item.SERVICE_RATIO / 100;
                    }
                }
                CommonParam param = new CommonParam();
                var rs = new BackendAdapter(param).Post<List<HIS_SERVICE_PATY>>("api/HisServicePaty/CreateList", ApiConsumers.MosConsumer, data, param);
                if (rs != null)
                {
                    success = true;
                    btnSave.Enabled = false;
                    BackendDataWorker.Reset<V_HIS_SERVICE_PATY>();
                    if (this.delegateRefresh != null)
                    {
                        this.delegateRefresh();
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
                var row = (ServicePatyImportADO)gridViewServicePaty.GetFocusedRow();
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
                var row = (ServicePatyImportADO)gridViewServicePaty.GetFocusedRow();
                if (row != null)
                {
                    if (servicePatyAdos != null && servicePatyAdos.Count > 0)
                    {
                        servicePatyAdos.Remove(row);
                        gridControlServicePaty.DataSource = null;
                        gridControlServicePaty.DataSource = servicePatyAdos;
                        CheckErrorLine(null);
                        if (checkClick)
                        {
                            if (btnShowLineError.Text == "Dòng lỗi")
                            {
                                btnShowLineError.Text = "Dòng không lỗi";
                            }
                            else
                            {
                                btnShowLineError.Text = "Dòng lỗi";
                            }
                            btnShowLineError_Click(null, null);
                        }
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
                checkClick = true;
                if (btnShowLineError.Text == "Dòng lỗi")
                {
                    btnShowLineError.Text = "Dòng không lỗi";
                    var errorLine = servicePatyAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = servicePatyAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewServicePaty_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    ServicePatyImportADO data = (ServicePatyImportADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
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
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewServicePaty_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ServicePatyImportADO pData = (ServicePatyImportADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                    //else if (e.Column.FieldName == "ACTIVE_ITEM")
                    //{
                    //    try
                    //    {
                    //        if (status == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    //            e.Value = Inventec.Common.Resource.Get.Value("frmHisMisuServicePatyType.HoatDong", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    //        else
                    //            e.Value = Inventec.Common.Resource.Get.Value("frmHisMisuServicePatyType.TamKhoa", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmServicePaty_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                btnSave.Enabled = false;
                btnShowLineError.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                var source = System.IO.Path.Combine(Application.StartupPath
                + "/Tmp/Imp", "IMPORT_SERVICE_PATY.xlsx");

                if (File.Exists(source))
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                    saveFileDialog1.Title = "Save File";
                    saveFileDialog1.FileName = "IMPORT_SERVICE_PATY";
                    saveFileDialog1.DefaultExt = "xlsx";
                    saveFileDialog1.Filter = "Excel files (*.xlsx)|All files (*.*)";
                    saveFileDialog1.FilterIndex = 2;
                    saveFileDialog1.RestoreDirectory = true;

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(source, saveFileDialog1.FileName);
                        DevExpress.XtraEditors.XtraMessageBox.Show("Tải file thành công");
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy file import");
                }
            }
            catch (Exception ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Tải file thất bại");
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                //WaitingManager.Show();
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    WaitingManager.Show();

                    var import = new Inventec.Common.ExcelImport.Import();
                    if (import.ReadFileExcel(ofd.FileName))
                    {
                        var hisServicePatyImport = import.GetWithCheck<ServicePatyImportADO>(0);
                        if (hisServicePatyImport != null && hisServicePatyImport.Count > 0)
                        {
                            List<ServicePatyImportADO> listAfterRemove = new List<ServicePatyImportADO>();
                            foreach (var item in hisServicePatyImport)
                            {
                                listAfterRemove.Add(item);
                            }

                            foreach (var item in hisServicePatyImport)
                            {
                                bool checkNull = string.IsNullOrEmpty(item.SERVICE_CODE)
                                    && string.IsNullOrEmpty(item.PATIENT_TYPE_CODE)
                                    && string.IsNullOrEmpty(item.REQUEST_DEPARMENT_CODES)
                                    && string.IsNullOrEmpty(item.REQUEST_ROOM_CODES)
                                    && string.IsNullOrEmpty(item.TO_TIME_STR)
                                    && string.IsNullOrEmpty(item.FROM_TIME_STR)
                                    && string.IsNullOrEmpty(item.HOUR_FROM)
                                    && string.IsNullOrEmpty(item.HOUR_TO)
                                    && string.IsNullOrEmpty(item.TREATMENT_FROM_TIME_STR)
                                    && string.IsNullOrEmpty(item.TREATMENT_TO_TIME_STR)
                                    && string.IsNullOrEmpty(item.EXECUTE_ROOM_CODES)
                                    && string.IsNullOrEmpty(item.BRANCH_CODE)
                                    && item.DAY_FROM_STR == null
                                    && item.DAY_TO_STR == null
                                     && item.INTRUCTION_NUMBER_FROM_STR == null
                                      && item.INTRUCTION_NUMBER_TO_STR == null
                                     && item.OVERTIME_PRICE_STR == null
                                     && item.PRICE_STR == null
                                     && item.VAT_RATIO_STR == null
                                     && item.PRIORITY_STR == null;

                                if (checkNull)
                                {
                                    listAfterRemove.Remove(item);
                                }
                            }

                            WaitingManager.Hide();

                            this.currentAdos = listAfterRemove;

                            if (this.currentAdos != null && this.currentAdos.Count > 0)
                            {
                                checkClick = false;
                                btnSave.Enabled = true;
                                btnShowLineError.Enabled = true;
                                servicePatyAdos = new List<ServicePatyImportADO>();
                                addServicePatyToProcessList(currentAdos, ref servicePatyAdos);
                                SetDataSource(servicePatyAdos);
                            }

                            //btnSave.Enabled = true;
                        }
                        else
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Import thất bại");
                        }
                    }
                    else
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không đọc được file");
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSave.Enabled)
                {
                    btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}
