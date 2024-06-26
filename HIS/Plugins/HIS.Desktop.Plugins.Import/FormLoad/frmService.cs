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
    public partial class frmService : Form
    {
        List<ServiceImportADO> serviceAdos;
        List<ServiceImportADO> currentAdos;
        RefeshReference delegateRefresh;

        public frmService(List<ServiceImportADO> data, RefeshReference _delegate)
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

        private void CheckErrorLine(List<ServiceImportADO> dataSource)
        {
            try
            {
                var checkError = serviceAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
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
                    i++;
                    string error = "";
                    var serAdo = new ServiceImportADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ServiceImportADO>(serAdo, item);

                    if (!string.IsNullOrEmpty(item.PARENT_CODE))
                    {
                        if (item.PARENT_CODE.Length > 25)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "PARENT_CODE");
                        }
                        var getData = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.SERVICE_CODE == item.PARENT_CODE);
                        if (getData != null)
                        {
                            serAdo.PARENT_ID = getData.ID;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "PARENT_CODE");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.SERVICE_TYPE_CODE))
                    {
                        if (item.SERVICE_TYPE_CODE.Length > 2)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "SERVICE_TYPE_CODE");
                        }
                        var getData = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(o => o.SERVICE_TYPE_CODE == item.SERVICE_TYPE_CODE);
                        if (getData != null)
                        {
                            if (serAdo.PARENT_ID != null)
                            {
                                var dataRam = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == serAdo.PARENT_ID);
                                if (dataRam != null && dataRam.SERVICE_TYPE_ID != getData.ID)
                                {
                                    error += string.Format(Message.MessageImport.ChaVaConTrungNhau, "SERVICE_TYPE_CODE");
                                }
                            }
                            serAdo.SERVICE_TYPE_ID = getData.ID;
                            serAdo.SERVICE_TYPE_NAME = getData.SERVICE_TYPE_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "SERVICE_TYPE_CODE");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "SERVICE_TYPE_CODE");
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
                            error += string.Format(Message.MessageImport.KhongHopLe, "BILL_OPTION_STR");
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
                            error += string.Format(Message.MessageImport.KhongHopLe, "CPNG");
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
                            error += string.Format(Message.MessageImport.KhongHopLe, "MultiRequest");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.ICD_CM_CODE))
                    {
                        if (item.ICD_CM_CODE.Length > 10)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "ICD_CM_CODE");
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
                            error += string.Format(Message.MessageImport.KhongHopLe, "ICD_CM_CODE");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.PACKAGE_CODE))
                    {
                        if (item.PACKAGE_CODE.Length > 2)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "PACKAGE_CODE");
                        }
                        var package = BackendDataWorker.Get<HIS_PACKAGE>().FirstOrDefault(o => o.PACKAGE_CODE == item.PACKAGE_CODE);
                        if (package != null)
                        {
                            serAdo.PACKAGE_ID = package.ID;
                            if (!serAdo.PACKAGE_PRICE.HasValue)
                            {
                                error += string.Format(Message.MessageImport.ThieuTruongDL, "PACKAGE_PRICE");
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "PACKAGE_CODE");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.PTTT_GROUP_CODE))
                    {
                        if (item.PTTT_GROUP_CODE.Length > 2)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "PTTT_GROUP_CODE");
                        }
                        var ptttGroup = BackendDataWorker.Get<HIS_PTTT_GROUP>().FirstOrDefault(o => o.PTTT_GROUP_CODE == item.PTTT_GROUP_CODE);
                        if (ptttGroup != null)
                        {
                            if (serAdo.SERVICE_TYPE_ID > 0)
                            {
                                if (serAdo.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT && serAdo.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT)
                                {
                                    error += string.Format(Message.MessageImport.ChiChoPhepNhapVoiLoaiDichVuLaPTTT);
                                }
                                else
                                {
                                    serAdo.PTTT_GROUP_ID = ptttGroup.ID;
                                }
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "PTTT_GROUP_CODE");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.PTTT_METHOD_CODE))
                    {
                        if (item.PTTT_METHOD_CODE.Length > 6)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "PTTT_METHOD_CODE");
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
                            error += string.Format(Message.MessageImport.KhongHopLe, "PTTT_METHOD_CODE");
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
                            serAdo.HEIN_SERVICE_TYPE_ID = getData.ID;
                            serAdo.HEIN_SERVICE_TYPE_NAME = getData.HEIN_SERVICE_TYPE_NAME;
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
                            serAdo.SERVICE_UNIT_ID = getData.ID;
                            serAdo.SERVICE_UNIT_NAME = getData.SERVICE_UNIT_NAME;
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

                    if (!string.IsNullOrEmpty(item.BILL_PATIENT_TYPE_CODE))
                    {
                        if (item.BILL_PATIENT_TYPE_CODE.Length > 6)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "BILL_PATIENT_TYPE_CODE");
                        }
                        var getData = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == item.BILL_PATIENT_TYPE_CODE);
                        if (getData != null)
                        {
                            serAdo.BILL_PATIENT_TYPE_ID = getData.ID;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "BILL_PATIENT_TYPE_CODE");
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
                            serAdo.HEIN_LIMIT_PRICE_INTR_TIME = dateTime;
                        }
                        else
                        {
                            error += string.Format(check, "HEIN_LIMIT_PRICE_INTR_TIME_STR");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.SERVICE_CODE))
                    {
                        if (item.SERVICE_CODE.Length > 25)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "SERVICE_CODE");
                        }

                        var check = BackendDataWorker.Get<V_HIS_SERVICE>().Exists(o => o.SERVICE_CODE == item.SERVICE_CODE);
                        if (check)
                        {
                            error += string.Format(Message.MessageImport.DaTonTai, "SERVICE_CODE");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "SERVICE_CODE");
                    }

                    if (!string.IsNullOrEmpty(item.SERVICE_NAME))
                    {
                        if (item.SERVICE_NAME.Length > 500)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "SERVICE_NAME");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "SERVICE_NAME");
                    }

                    if (!string.IsNullOrEmpty(item.SPECIALITY_CODE))
                    {
                        if (serAdo.SERVICE_TYPE_ID > 0 && serAdo.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH)
                        {
                            error += string.Format(Message.MessageImport.ChiDuocNhaoVoiLoaiDichVuLaKham, "SPECIALITY_CODE");
                        }
                        if (item.SPECIALITY_CODE.Length > 3)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "SPECIALITY_CODE");
                        }
                    }

                    if (item.PACKAGE_PRICE.HasValue)
                    {
                        if (item.PACKAGE_PRICE.Value > 99999999999999 || item.PACKAGE_PRICE < 0)
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "PACKAGE_PRICE");
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

                    if (item.HEIN_LIMIT_PRICE.HasValue)
                    {
                        if (item.HEIN_LIMIT_PRICE.Value > 99999999999999 || item.HEIN_LIMIT_PRICE < 0)
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "HEIN_LIMIT_PRICE");
                        }
                    }

                    if (item.COGS.HasValue)
                    {
                        if (item.COGS.Value > 99999999999999 || item.COGS < 0)
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "COGS");
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

                    if (item.PACKAGE_PRICE.HasValue && serAdo.PACKAGE_ID == null)
                    {
                        error += string.Format(Message.MessageImport.ChiDuocNhapGiaGoiKhiCoGoi);
                    }

                    if (_serviceRef.Exists(o => o.SERVICE_CODE == item.SERVICE_CODE))
                    {
                        error += string.Format(Message.MessageImport.TonTaiTrungNhauTrongFileImport, "SERVICE_CODE");
                    }

                    serAdo.ERROR = error;
                    serAdo.ID = i;

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
                CommonParam param = new CommonParam();
                var rs = new BackendAdapter(param).Post<List<HIS_SERVICE>>("api/HisService/CreateList", ApiConsumers.MosConsumer, data, param);
                if (rs != null)
                {
                    success = true;
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
                        SetDataSource(serviceAdos);
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
                serviceAdos = new List<ServiceImportADO>();
                addServiceToProcessList(currentAdos, ref serviceAdos);
                SetDataSource(serviceAdos);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}
