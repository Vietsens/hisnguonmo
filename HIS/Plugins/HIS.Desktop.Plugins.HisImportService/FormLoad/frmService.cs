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
using HIS.Desktop.Plugins.HisImportService.ADO;
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

namespace HIS.Desktop.Plugins.HisImportService.FormLoad
{
    public partial class frmService : HIS.Desktop.Utility.FormBase
    {
        List<ServiceImportADO> serviceAdos;
        List<ServiceImportADO> currentAdos;
        RefeshReference delegateRefresh;
        bool checkClick;
        Inventec.Desktop.Common.Modules.Module currentModule;
        Dictionary<string, V_HIS_SERVICE> DicService;

        public frmService(Inventec.Desktop.Common.Modules.Module module, RefeshReference _delegate)
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

        public frmService(Inventec.Desktop.Common.Modules.Module module)
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

        private void CheckErrorLine(List<ServiceImportADO> dataSource)
        {
            try
            {
                if (serviceAdos != null && serviceAdos.Count > 0)
                {
                    var checkError = serviceAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
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

        private void SetDataSource(List<ServiceImportADO> dataSource)
        {
            try
            {
                gridControlService.BeginUpdate();
                gridControlService.DataSource = null;
                gridControlService.DataSource = dataSource;
                gridControlService.EndUpdate();
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

        private void addServiceToProcessList(List<ServiceImportADO> _service, ref List<ServiceImportADO> _serviceRef)
        {
            try
            {
                _serviceRef = new List<ServiceImportADO>();
                long i = 0;
                foreach (var item in _service)
                {
                    Inventec.Common.Logging.LogSystem.Warn("_serviceRef: " + _serviceRef.Count);
                    i++;
                    string error = "";
                    var serAdo = new ServiceImportADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ServiceImportADO>(serAdo, item);
                    if (!string.IsNullOrEmpty(item.PTTT_GROUP_CODE))
                        item.PTTT_GROUP_CODE = item.PTTT_GROUP_CODE.Trim();

                    if (!string.IsNullOrEmpty(item.PARENT_CODE))
                    {
                        if (item.PARENT_CODE.Length > 25)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Cha");
                        }
                        //var getData = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.SERVICE_CODE == item.PARENT_CODE);
                        if (DicService.ContainsKey(item.PARENT_CODE))
                        {
                            serAdo.PARENT_ID = DicService[item.PARENT_CODE].ID;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Cha");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.GENDER_CODE))
                    {
                        var gender = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.GENDER_CODE == item.GENDER_CODE);
                        if (gender != null)
                        {
                            serAdo.GENDER_ID = gender.ID;
                            serAdo.GENDER_NAME = gender.GENDER_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Giới tính");
                        }
                    }
                    if (!string.IsNullOrEmpty(item.EMR_FORM_CODES))
                    {
                        var codes = item.EMR_FORM_CODES.Split(',');
                        if(codes.Count() > 10)
                        {
                            error += error += string.Format(Message.MessageImport.Maxlength, "Mã phiếu vỏ bệnh án");
                        }
                        var dataEmr = BackendDataWorker.Get<HIS_EMR_FORM>().Where(s => codes.Contains(s.EMR_FORM_CODE));
                        if (dataEmr.Any())
                        {
                            string error_code = "";
                            foreach(var _code in codes)
                            {
                                if (!dataEmr.Select(s => s.EMR_FORM_CODE).ToList().Contains(_code))
                                {
                                    error_code += _code+",";
                                }
                            }
                            if (!string.IsNullOrEmpty(error_code))
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Mã phiếu vỏ bệnh án.("+error_code+")");
                            }
                            serAdo.EMR_FORM_CODES = string.Join(",", dataEmr.Select(s => s.EMR_FORM_CODE).ToList());
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã phiếu vỏ bệnh án.("+item.EMR_FORM_CODES+")");
                        }
                    }
                    if (!string.IsNullOrEmpty(item.SERVICE_TYPE_CODE))
                    {
                        if (item.SERVICE_TYPE_CODE.Length > 2)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Loại dịch vụ");
                        }
                        var getData = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(o => o.SERVICE_TYPE_CODE == item.SERVICE_TYPE_CODE);
                        if (getData != null)
                        {
                            if (serAdo.PARENT_ID != null)
                            {
                                var dataRam = DicService[item.PARENT_CODE];
                                if (dataRam != null && dataRam.SERVICE_TYPE_ID != getData.ID)
                                {
                                    error += string.Format(Message.MessageImport.ChaVaConTrungNhau, "Loại dịch vụ");
                                }
                            }
                            serAdo.SERVICE_TYPE_ID = getData.ID;
                            serAdo.SERVICE_TYPE_NAME = getData.SERVICE_TYPE_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Loại dịch vụ");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Loại dịch vụ");
                    }

                    if (!string.IsNullOrEmpty(item.BILL_OPTION_STR))
                    {
                        if (item.BILL_OPTION_STR == "2")
                        {
                            serAdo.BILL_OPTION = 1;
                        }
                        else if (item.BILL_OPTION_STR == "3")
                        {
                            serAdo.BILL_OPTION = 2;
                        }
                        else if (item.BILL_OPTION_STR == "1")
                        {
                            serAdo.BILL_OPTION = null;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Loại hóa đơn TT");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.CPNG))
                    {
                        if (item.CPNG == "x")
                        {
                            serAdo.IS_OUT_PARENT_FEE = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Chi phí ngoài gói");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.MultiRequest))
                    {
                        if (item.MultiRequest == "x")
                        {
                            serAdo.IS_MULTI_REQUEST = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Chỉ định > 1");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.ICD_CM_CODE))
                    {
                        if (item.ICD_CM_CODE.Length > 10)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã ICD CM");
                        }
                        var icdCm = BackendDataWorker.Get<HIS_ICD_CM>().FirstOrDefault(o => o.ICD_CM_CODE == item.ICD_CM_CODE);
                        if (icdCm != null)
                        {
                            if (serAdo.SERVICE_TYPE_ID > 0)
                            {
                                if (serAdo.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT && serAdo.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT)
                                {
                                    error += string.Format(Message.MessageImport.ChiChoPhepNhapVoiLoaiDichVuLaPTTT);
                                }
                                else
                                {
                                    serAdo.ICD_CM_ID = icdCm.ID;
                                }
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã ICD CM");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.PACKAGE_CODE))
                    {
                        if (item.PACKAGE_CODE.Length > 2)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Gói");
                        }
                        var package = BackendDataWorker.Get<HIS_PACKAGE>().FirstOrDefault(o => o.PACKAGE_CODE == item.PACKAGE_CODE);
                        if (package != null)
                        {
                            serAdo.PACKAGE_ID = package.ID;
                            if (string.IsNullOrEmpty(item.PACKAGE_PRICE_STR))
                            {
                                error += string.Format(Message.MessageImport.ThieuTruongDL, "Giá gói");
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Gói");
                        }
                    }


                    if (!string.IsNullOrEmpty(item.PTTT_GROUP_CODE))
                    {
                        if (item.PTTT_GROUP_CODE.Length > 2)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Nhóm PTTT");
                        }
                        var ptttGroup = BackendDataWorker.Get<HIS_PTTT_GROUP>().FirstOrDefault(o => o.PTTT_GROUP_CODE == item.PTTT_GROUP_CODE);
                        if (ptttGroup != null)
                        {
                            if (serAdo.SERVICE_TYPE_ID > 0)
                            {
                                //if (serAdo.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT && serAdo.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT)
                                //{
                                //    error += string.Format(Message.MessageImport.ChiChoPhepNhapVoiLoaiDichVuLaPTTT);
                                //}
                                //else
                                //{
                                serAdo.PTTT_GROUP_ID = ptttGroup.ID;
                                //}
                            }
                            else
                            {
                                serAdo.PTTT_GROUP_ID = null;
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Nhóm PTTT");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.PTTT_METHOD_CODE))
                    {
                        if (item.PTTT_METHOD_CODE.Length > 6)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Phương pháp PTTT");
                        }
                        var ptttMethod = BackendDataWorker.Get<HIS_PTTT_METHOD>().FirstOrDefault(o => o.PTTT_METHOD_CODE == item.PTTT_METHOD_CODE);
                        if (ptttMethod != null)
                        {
                            if (serAdo.SERVICE_TYPE_ID > 0)
                            {
                                if (serAdo.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT && serAdo.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT)
                                {
                                    error += string.Format(Message.MessageImport.ChiChoPhepNhapVoiLoaiDichVuLaPTTT);
                                }
                                else
                                {
                                    serAdo.PTTT_METHOD_ID = ptttMethod.ID;
                                }
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Phương pháp PTTT");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_SERVICE_TYPE_CODE))
                    {
                        if (item.HEIN_SERVICE_TYPE_CODE.Length > 10)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Loại DV BHYT");
                        }
                        var getData = BackendDataWorker.Get<HIS_HEIN_SERVICE_TYPE>().FirstOrDefault(o => o.HEIN_SERVICE_TYPE_CODE == item.HEIN_SERVICE_TYPE_CODE);
                        if (getData != null)
                        {
                            serAdo.HEIN_SERVICE_TYPE_ID = getData.ID;
                            serAdo.HEIN_SERVICE_TYPE_NAME = getData.HEIN_SERVICE_TYPE_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Loại DV BHYT");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.SERVICE_UNIT_CODE))
                    {
                        if (item.SERVICE_UNIT_CODE.Length > 3)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Đơn vị tính");
                        }
                        var getData = BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(o => o.SERVICE_UNIT_CODE == item.SERVICE_UNIT_CODE);
                        if (getData != null)
                        {
                            serAdo.SERVICE_UNIT_ID = getData.ID;
                            serAdo.SERVICE_UNIT_NAME = getData.SERVICE_UNIT_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Đơn vị tính");
                        }

                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Đơn vị tính");
                    }

                    if (!string.IsNullOrEmpty(item.BILL_PATIENT_TYPE_CODE))
                    {
                        if (item.BILL_PATIENT_TYPE_CODE.Length > 6)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Đối tượng thanh toán bắt buộc");
                        }
                        var getData = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == item.BILL_PATIENT_TYPE_CODE);
                        if (getData != null)
                        {
                            serAdo.BILL_PATIENT_TYPE_ID = getData.ID;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Đối tượng thanh toán bắt buộc");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_IN_TIME_STR))
                    {
                        long? dateTime = null;
                        string check = "";
                        convertDateStringToTimeNumber(item.HEIN_LIMIT_PRICE_IN_TIME_STR, ref dateTime, ref check);
                        if (dateTime != null && string.IsNullOrEmpty(check))
                        {
                            serAdo.HEIN_LIMIT_PRICE_IN_TIME = dateTime;
                        }
                        else
                        {
                            error += string.Format(check, "Thời gian theo ngày vào viện");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_INTR_TIME_STR))
                    {
                        long? dateTime = null;
                        string check = "";
                        convertDateStringToTimeNumber(item.HEIN_LIMIT_PRICE_INTR_TIME_STR, ref dateTime, ref check);
                        if (dateTime != null && string.IsNullOrEmpty(check))
                        {
                            serAdo.HEIN_LIMIT_PRICE_INTR_TIME = dateTime;
                        }
                        else
                        {
                            error += string.Format(check, "Thời gian theo ngày chỉ định");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.SERVICE_CODE))
                    {
                        if (item.SERVICE_CODE.Length > 25)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã dịch vụ");
                        }

                        var check = DicService.ContainsKey(item.SERVICE_CODE);
                        if (check)
                        {
                            error += string.Format(Message.MessageImport.DaTonTai, "Mã dịch vụ");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã dịch vụ");
                    }

                    if (!string.IsNullOrEmpty(item.SERVICE_NAME))
                    {
                        if (Encoding.UTF8.GetByteCount(item.SERVICE_NAME.Trim()) > 1500)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Tên dịch vụ");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Tên dịch vụ");
                    }

                    if (!string.IsNullOrEmpty(item.SPECIALITY_CODE))
                    {
                        if (serAdo.SERVICE_TYPE_ID > 0 && serAdo.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH)
                        {
                            error += string.Format(Message.MessageImport.ChiDuocNhaoVoiLoaiDichVuLaKham, "Mã chuyên khoa");
                        }
                        if (item.SPECIALITY_CODE.Length > 3)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã chuyên khoa");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.ESTIMATE_DURATION_STR))
                    {
                        if (Inventec.Common.Number.Check.IsDecimal(item.ESTIMATE_DURATION_STR))
                        {
                            serAdo.ESTIMATE_DURATION = Inventec.Common.TypeConvert.Parse.ToDecimal(item.ESTIMATE_DURATION_STR);
                            if (serAdo.ESTIMATE_DURATION.Value > 99999999999999 || serAdo.ESTIMATE_DURATION < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Thời gian dự kiến");
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Thời gian dự kiến");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.PACKAGE_PRICE_STR))
                    {
                        if (Inventec.Common.Number.Check.IsDecimal(item.PACKAGE_PRICE_STR))
                        {
                            serAdo.PACKAGE_PRICE = Inventec.Common.TypeConvert.Parse.ToDecimal(item.PACKAGE_PRICE_STR);
                            if (serAdo.PACKAGE_PRICE.Value > 99999999999999 || serAdo.PACKAGE_PRICE < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Giá gói");
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Giá gói");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.NUM_ORDER_STR))
                    {
                        if (Inventec.Common.Number.Check.IsNumber(item.NUM_ORDER_STR))
                        {
                            serAdo.NUM_ORDER = Inventec.Common.TypeConvert.Parse.ToInt64(item.NUM_ORDER_STR);
                            if (serAdo.NUM_ORDER.ToString().Length > 19 || serAdo.NUM_ORDER < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "STT hiện");
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "STT hiện");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_SERVICE_BHYT_CODE))
                    {
                        if (item.HEIN_SERVICE_BHYT_CODE.Length > 100)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã DV BHYT");
                        }
                        if (string.IsNullOrEmpty(item.HEIN_SERVICE_BHYT_NAME))
                        {
                            error += string.Format(Message.MessageImport.CoThiPhaiNhap, "Mã DV BHYT", "Tên DV BHYT");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_SERVICE_BHYT_NAME))
                    {
                        if (Encoding.UTF8.GetByteCount(item.HEIN_SERVICE_BHYT_NAME.Trim()) > 1500)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Tên DV BHYT");
                        }
                        if (string.IsNullOrEmpty(item.HEIN_SERVICE_BHYT_CODE))
                        {
                            error += string.Format(Message.MessageImport.CoThiPhaiNhap, "Tên DV BHYT", "Mã DV BHYT");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_ORDER))
                    {
                        if (item.HEIN_ORDER.Length > 20)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Số thứ tự BHYT");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_LIMIT_RATIO_STR))
                    {
                        if (Inventec.Common.Number.Check.IsDecimal(item.HEIN_LIMIT_RATIO_STR))
                        {
                            serAdo.HEIN_LIMIT_RATIO = Inventec.Common.TypeConvert.Parse.ToDecimal(item.HEIN_LIMIT_RATIO_STR);
                            if (serAdo.HEIN_LIMIT_RATIO.Value > 1 || serAdo.HEIN_LIMIT_RATIO < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Tỉ lệ trần mới");
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Tỉ lệ trần mới");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_LIMIT_RATIO_OLD_STR))
                    {
                        if (Inventec.Common.Number.Check.IsDecimal(item.HEIN_LIMIT_RATIO_OLD_STR))
                        {
                            serAdo.HEIN_LIMIT_RATIO_OLD = Inventec.Common.TypeConvert.Parse.ToDecimal(item.HEIN_LIMIT_RATIO_OLD_STR);
                            if (serAdo.HEIN_LIMIT_RATIO_OLD.Value > 1 || serAdo.HEIN_LIMIT_RATIO < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Tỉ lệ trần cũ");
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Tỉ lệ trần cũ");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_OLD_STR))
                    {
                        if (Inventec.Common.Number.Check.IsDecimal(item.HEIN_LIMIT_PRICE_OLD_STR))
                        {
                            serAdo.HEIN_LIMIT_PRICE_OLD = Inventec.Common.TypeConvert.Parse.ToDecimal(item.HEIN_LIMIT_PRICE_OLD_STR);
                            if (serAdo.HEIN_LIMIT_PRICE_OLD.Value > 99999999999999 || serAdo.HEIN_LIMIT_PRICE_OLD < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Giá trần cũ");
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Giá trần cũ");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_STR))
                    {
                        if (Inventec.Common.Number.Check.IsDecimal(item.HEIN_LIMIT_PRICE_STR))
                        {
                            serAdo.HEIN_LIMIT_PRICE = Inventec.Common.TypeConvert.Parse.ToDecimal(item.HEIN_LIMIT_PRICE_STR);
                            if (serAdo.HEIN_LIMIT_PRICE.Value > 99999999999999 || serAdo.HEIN_LIMIT_PRICE < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Giá trần mới");
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Giá trần mới");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.COGS_STR))
                    {
                        if (Inventec.Common.Number.Check.IsDecimal(item.COGS_STR))
                        {
                            serAdo.COGS = Inventec.Common.TypeConvert.Parse.ToDecimal(item.COGS_STR);
                            if ((serAdo.COGS ?? 0) > 99999999999999 || (serAdo.COGS ?? 0) < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Giá vốn");
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Giá vốn");
                        }
                    }

                    if ((!string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_STR) || !string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_OLD_STR)) && (!string.IsNullOrEmpty(item.HEIN_LIMIT_RATIO_STR) || !string.IsNullOrEmpty(item.HEIN_LIMIT_RATIO_OLD_STR)))
                    {
                        error += string.Format(Message.MessageImport.ChiDuocNhapGiaHoacTiLeTran);
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_IN_TIME_STR) && !string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_INTR_TIME_STR))
                    {
                        error += string.Format(Message.MessageImport.ChiDuocNhapTGVaoVienHoacTGChiDinh);
                    }

                    if (!string.IsNullOrEmpty(item.PACKAGE_PRICE_STR) && serAdo.PACKAGE_ID == null)
                    {
                        error += string.Format(Message.MessageImport.ChiDuocNhapGiaGoiKhiCoGoi);
                    }

                    if (_serviceRef.Exists(o => o.SERVICE_CODE == item.SERVICE_CODE))
                    {
                        error += string.Format(Message.MessageImport.TonTaiTrungNhauTrongFileImport, "Mã dịch vụ");
                    }

                    if (!string.IsNullOrEmpty(item.DESCRIPTION) && Encoding.UTF8.GetByteCount(item.DESCRIPTION.Trim()) > 2000)
                    {
                        error += string.Format(Message.MessageImport.Maxlength, "Diễn giải");
                    }
                    if (item.ALLOW_SEND_PACS_STR != null)
                    {
                        if (!string.IsNullOrEmpty(item.ALLOW_SEND_PACS_STR) && item.ALLOW_SEND_PACS_STR.ToLower() == "x")
                        {
                            serAdo.ALLOW_SEND_PACS = (short)1;
                        }
                    }
                    else
                    {
                        if (serAdo.SERVICE_TYPE_ID == 3 || serAdo.SERVICE_TYPE_ID == 5 || serAdo.SERVICE_TYPE_ID == 10 || serAdo.SERVICE_TYPE_ID == 9)
                        {
                            serAdo.ALLOW_SEND_PACS_STR = "x";
                            serAdo.ALLOW_SEND_PACS = (short)1;
                        }
                    }
                    serAdo.ERROR = error;
                    serAdo.ID = i;
                    serAdo.IS_LEAF = 1;

                    _serviceRef.Add(serAdo);
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
                AutoMapper.Mapper.CreateMap<ServiceImportADO, HIS_SERVICE>();
                var data = AutoMapper.Mapper.Map<List<HIS_SERVICE>>(serviceAdos);

                if (data != null && data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        item.ID = 0;
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("Data Import." + Inventec.Common.Logging.LogUtil.TraceData("serviceAdos", data));
                CommonParam param = new CommonParam();
                var rs = new BackendAdapter(param).Post<List<HIS_SERVICE>>("api/HisService/CreateList", ApiConsumers.MosConsumer, data, param);
                if (rs != null)
                {
                    success = true;
                    btnSave.Enabled = false;
                    BackendDataWorker.Reset<V_HIS_SERVICE>();
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
                var row = (ServiceImportADO)gridViewService.GetFocusedRow();
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
                var row = (ServiceImportADO)gridViewService.GetFocusedRow();
                if (row != null)
                {
                    if (serviceAdos != null && serviceAdos.Count > 0)
                    {
                        serviceAdos.Remove(row);
                        gridControlService.DataSource = null;
                        gridControlService.DataSource = serviceAdos;
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
                    var errorLine = serviceAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = serviceAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewService_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    ServiceImportADO data = (ServiceImportADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
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
                    else if (e.Column.FieldName == "CPNG_STR")
                    {
                        e.RepositoryItem = Item_Check;
                    }
                    else if (e.Column.FieldName == "MultiRequest_Str")
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

        private void gridViewService_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ServiceImportADO pData = (ServiceImportADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                    else if (e.Column.FieldName == "PACKAGE_NAME_STR")
                    {
                        try
                        {
                            var data = BackendDataWorker.Get<HIS_PACKAGE>().FirstOrDefault(o => o.ID == pData.PACKAGE_ID);
                            if (data != null)
                            {
                                e.Value = data.PACKAGE_NAME;
                            }

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
                    else if (e.Column.FieldName == "PTTT_GROUP_NAME_STR")
                    {
                        try
                        {
                            var data = BackendDataWorker.Get<HIS_PTTT_GROUP>().FirstOrDefault(o => o.ID == pData.PTTT_GROUP_ID);
                            if (data != null)
                            {
                                e.Value = data.PTTT_GROUP_NAME;
                            }

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua PTTT_GROUP_NAME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "PTTT_METHOD_NAME_STR")
                    {
                        try
                        {
                            var data = BackendDataWorker.Get<HIS_PTTT_METHOD>().FirstOrDefault(o => o.ID == pData.PTTT_METHOD_ID);
                            if (data != null)
                            {
                                e.Value = data.PTTT_METHOD_NAME;
                            }

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua PTTT_METHOD_NAME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "BILL_PATIENT_TYPE_NAME")
                    {
                        try
                        {
                            var data = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == pData.BILL_PATIENT_TYPE_ID);
                            if (data != null)
                            {
                                e.Value = data.PATIENT_TYPE_NAME;
                            }

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua BILL_PATIENT_TYPE_NAME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "ICD_CM_NAME_STR")
                    {
                        try
                        {
                            var data = BackendDataWorker.Get<HIS_ICD_CM>().FirstOrDefault(o => o.ID == pData.ICD_CM_ID);
                            if (data != null)
                            {
                                e.Value = data.ICD_CM_NAME;
                            }

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua ICD_CM_NAME", ex);
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
                    else if (e.Column.FieldName == "CPNG_STR")
                    {
                        try
                        {
                            e.Value = pData.CPNG == "x" ? true : false;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua CPNG", ex);
                        }
                    }
                    else if (e.Column.FieldName == "MultiRequest_Str")
                    {
                        try
                        {
                            e.Value = pData.MultiRequest == "x" ? true : false;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua MultiRequest_Str", ex);
                        }
                    }
                    //else if (e.Column.FieldName == "ACTIVE_ITEM")
                    //{
                    //    try
                    //    {
                    //        if (status == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    //            e.Value = Inventec.Common.Resource.Get.Value("frmHisMisuServiceType.HoatDong", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    //        else
                    //            e.Value = Inventec.Common.Resource.Get.Value("frmHisMisuServiceType.TamKhoa", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                    else if (e.Column.FieldName == "ALLOW_SEND_PACS_DISPLAY")
                    {
                        try
                        {
                            e.Value = !string.IsNullOrEmpty(pData.ALLOW_SEND_PACS_STR) && pData.ALLOW_SEND_PACS_STR.ToLower() == "x";
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

        private void frmService_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                btnSave.Enabled = false;
                btnShowLineError.Enabled = false;
                DicService = BackendDataWorker.Get<V_HIS_SERVICE>().ToDictionary(o => o.SERVICE_CODE, o => o);
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
                + "/Tmp/Imp", "IMPORT_SERVICE.xlsx");

                if (File.Exists(source))
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                    saveFileDialog1.Title = "Save File";
                    saveFileDialog1.FileName = "IMPORT_SERVICE";
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
                        var hisServiceImport = import.GetWithCheck<ServiceImportADO>(0);
                        if (hisServiceImport != null && hisServiceImport.Count > 0)
                        {
                            List<ServiceImportADO> listAfterRemove = new List<ServiceImportADO>();
                            foreach (var item in hisServiceImport)
                            {
                                listAfterRemove.Add(item);
                            }

                            foreach (var item in hisServiceImport)
                            {
                                bool checkNull = string.IsNullOrEmpty(item.SERVICE_CODE)
                                    && string.IsNullOrEmpty(item.SERVICE_NAME)
                                    && string.IsNullOrEmpty(item.SERVICE_UNIT_CODE)
                                    && string.IsNullOrEmpty(item.SERVICE_TYPE_CODE)
                                    && item.NUM_ORDER_STR == null
                                    && string.IsNullOrEmpty(item.PTTT_METHOD_CODE)
                                    && string.IsNullOrEmpty(item.HEIN_SERVICE_TYPE_CODE)
                                    && string.IsNullOrEmpty(item.BILL_PATIENT_TYPE_CODE)
                                    && item.COGS_STR == null
                                    && item.ESTIMATE_DURATION_STR == null
                                    && string.IsNullOrEmpty(item.CPNG)
                                    && string.IsNullOrEmpty(item.PTTT_GROUP_CODE)
                                    && string.IsNullOrEmpty(item.ICD_CM_CODE)
                                    && string.IsNullOrEmpty(item.HEIN_SERVICE_BHYT_CODE)
                                    && string.IsNullOrEmpty(item.HEIN_SERVICE_BHYT_NAME)
                                    && string.IsNullOrEmpty(item.HEIN_ORDER)
                                    && item.HEIN_LIMIT_RATIO_OLD_STR == null
                                    && item.HEIN_LIMIT_RATIO_STR == null
                                    && item.HEIN_LIMIT_PRICE_STR == null
                                    && item.HEIN_LIMIT_PRICE_OLD_STR == null
                                    && string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_IN_TIME_STR)
                                    && string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_INTR_TIME_STR)
                                    && item.PACKAGE_PRICE_STR == null
                                    && string.IsNullOrEmpty(item.SPECIALITY_CODE)
                                    && string.IsNullOrEmpty(item.PACKAGE_CODE)
                                    && string.IsNullOrEmpty(item.MultiRequest)
                                    && string.IsNullOrEmpty(item.BILL_OPTION_STR)
                                    && string.IsNullOrEmpty(item.PARENT_CODE)
                                    && string.IsNullOrEmpty(item.DESCRIPTION);

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
                                serviceAdos = new List<ServiceImportADO>();
                                addServiceToProcessList(currentAdos, ref serviceAdos);
                                SetDataSource(serviceAdos);
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
