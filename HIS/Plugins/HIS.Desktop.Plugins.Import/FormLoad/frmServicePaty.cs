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
    public partial class frmServicePaty : Form
    {
        List<ServicePatyImportADO> servicePatyAdos;
        List<ServicePatyImportADO> currentAdos;
        RefeshReference delegateRefresh;

        public frmServicePaty(List<ServicePatyImportADO> data, RefeshReference _delegate)
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

        private void CheckErrorLine(List<ServicePatyImportADO> dataSource)
        {
            try
            {
                var checkError = servicePatyAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
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

                    //if (!string.IsNullOrEmpty(item.PACKAGE_NUMBER))
                    //{
                    //    if (item.PACKAGE_NUMBER.Length > 100)
                    //    {
                    //        error += string.Format(Message.MessageImport.Maxlength, "PACKAGE_NUMBER");
                    //    }
                    //}

                    if (!string.IsNullOrEmpty(item.EXP_PRICE_STR))
                    {
                        if (checkNumber(item.EXP_PRICE_STR))
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "EXP_PRICE_STR");
                        }
                        else
                        {
                            var price = Inventec.Common.TypeConvert.Parse.ToDecimal(item.EXP_PRICE_STR);
                            if (price > 99999999999999 || price < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "EXP_PRICE_STR");
                            }
                            else
                                mateAdo.PRICE = price;
                        }
                    }
                    else
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "EXP_PRICE_STR");

                    if (!string.IsNullOrEmpty(item.EXP_VAT_RATIO_STR))
                    {
                        if (checkNumber(item.EXP_VAT_RATIO_STR))
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "EXP_VAT_RATIO_STR");
                        }
                        else
                        {
                            var price = Inventec.Common.TypeConvert.Parse.ToDecimal(item.EXP_VAT_RATIO_STR);
                            if (price > 1 || price < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "EXP_VAT_RATIO_STR");
                            }
                            else
                                mateAdo.VAT_RATIO = price;
                        }
                    }
                    else
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "EXP_VAT_RATIO_STR");

                    if (!string.IsNullOrEmpty(item.SERVICE_CODE))
                    {
                        if (item.SERVICE_CODE.Length > 25)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "SERVICE_CODE");
                        }
                        var serviceGet = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.SERVICE_CODE == item.SERVICE_CODE);
                        if (serviceGet != null)
                        {
                            mateAdo.SERVICE_ID = serviceGet.ID;
                            mateAdo.SERVICE_NAME = serviceGet.SERVICE_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "SERVICE_CODE");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "SERVICE_CODE");
                    }

                    if (!string.IsNullOrEmpty(item.PATIENT_TYPE_CODE))
                    {
                        if (item.PATIENT_TYPE_CODE.Length > 6)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "PATIENT_TYPE_CODE");
                        }
                        var mater = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == item.PATIENT_TYPE_CODE);
                        if (mater != null)
                        {
                            mateAdo.PATIENT_TYPE_ID = mater.ID;
                            mateAdo.PATIENT_TYPE_NAME = mater.PATIENT_TYPE_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "PATIENT_TYPE_CODE");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "PATIENT_TYPE_CODE");
                    }

                    if (!string.IsNullOrEmpty(item.BRANCH_CODE))
                    {
                        if (item.BRANCH_CODE.Length > 6)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "BRANCH_CODE");
                        }
                        var branchGet = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.BRANCH_CODE == item.BRANCH_CODE);
                        if (branchGet != null)
                        {
                            mateAdo.BRANCH_ID = branchGet.ID;
                            mateAdo.BRANCH_NAME = branchGet.BRANCH_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "BRANCH_CODE");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "BRANCH_CODE");
                    }

                    if (item.OVERTIME_PRICE != null)
                    {
                        if (item.OVERTIME_PRICE > 99999999999999 || item.OVERTIME_PRICE < 0)
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "OVERTIME_PRICE");
                        }
                    }

                    if (item.PRIORITY != null)
                    {
                        if (item.PRIORITY > 999999999999999999 || item.PRIORITY < 0)
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "PRIORITY");
                        }
                    }

                    if (item.INTRUCTION_NUMBER_FROM != null)
                    {
                        if (item.INTRUCTION_NUMBER_FROM > 999999999999999999 || item.INTRUCTION_NUMBER_FROM < 0)
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "INTRUCTION_NUMBER_FROM");
                        }
                    }

                    if (item.INTRUCTION_NUMBER_TO != null)
                    {
                        if (item.INTRUCTION_NUMBER_TO > 999999999999999999 || item.INTRUCTION_NUMBER_TO < 0)
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "INTRUCTION_NUMBER_TO");
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
                            error += string.Format(check, "FROM_TIME_STR");
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
                            error += string.Format(check, "TO_TIME_STR");
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
                            error += string.Format(check, "TREATMENT_FROM_TIME_STR");
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
                            error += string.Format(check, "TREATMENT_TO_TIME_STR");
                        }
                    }

                    if (item.DAY_FROM != null)
                    {
                        if (item.DAY_FROM > 7 || item.DAY_FROM < 0)
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "DAY_FROM");
                        }
                    }

                    if (item.DAY_TO != null)
                    {
                        if (item.DAY_TO > 7 || item.DAY_TO < 0)
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "DAY_TO");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HOUR_FROM))
                    {
                        string erro = "";
                        CheckHour(item.HOUR_FROM, ref erro);
                        if (!string.IsNullOrEmpty(erro))
                        {
                            error += string.Format(erro, "HOUR_FROM");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HOUR_TO))
                    {
                        string erro = "";
                        CheckHour(item.HOUR_TO, ref erro);
                        if (!string.IsNullOrEmpty(erro))
                        {
                            error += string.Format(erro, "HOUR_TO");
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
                                error += string.Format(Message.MessageImport.KhongHopLe, "EXECUTE_ROOM_CODES");
                                break;
                            }
                            else
                            {
                                roomIds += checkData.ID.ToString() + ",";
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
                                error += string.Format(Message.MessageImport.KhongHopLe, "REQUEST_ROOM_CODES");
                                break;
                            }
                            else
                            {
                                roomIds += checkData.ID.ToString() + ",";
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
                                error += string.Format(Message.MessageImport.KhongHopLe, "REQUEST_DEPARMENT_CODES");
                                break;

                            }
                            else
                            {
                                departmentIds += checkData.ID.ToString() + ",";
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
                CommonParam param = new CommonParam();
                var rs = new BackendAdapter(param).Post<List<HIS_SERVICE_PATY>>("api/HisServicePaty/CreateList", ApiConsumers.MosConsumer, data, param);
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
                        SetDataSource(servicePatyAdos);
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
                servicePatyAdos = new List<ServicePatyImportADO>();
                addServicePatyToProcessList(currentAdos, ref servicePatyAdos);
                SetDataSource(servicePatyAdos);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}
