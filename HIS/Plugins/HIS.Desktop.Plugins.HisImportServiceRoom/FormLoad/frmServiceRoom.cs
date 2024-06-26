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
using HIS.Desktop.Plugins.HisImportServiceRoom.ADO;
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

namespace HIS.Desktop.Plugins.HisImportServiceRoom.FormLoad
{
    public partial class frmServiceRoom : HIS.Desktop.Utility.FormBase
    {
        List<ServiceRoomImportADO> serviceRoomAdos;
        List<ServiceRoomImportADO> currentAdos;
        RefeshReference delegateRefresh;
        Inventec.Desktop.Common.Modules.Module currentModule;
        bool checkClick;

        public frmServiceRoom(Inventec.Desktop.Common.Modules.Module module, RefeshReference _delegate)
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

        public frmServiceRoom(Inventec.Desktop.Common.Modules.Module module)
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

        private void CheckErrorLine(List<ServiceRoomImportADO> dataSource)
        {
            try
            {
                if (serviceRoomAdos != null && serviceRoomAdos.Count > 0)
                {
                    var checkError = serviceRoomAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
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

        private void SetDataSource(List<ServiceRoomImportADO> dataSource)
        {
            try
            {
                gridControlServiceRoom.BeginUpdate();
                gridControlServiceRoom.DataSource = null;
                gridControlServiceRoom.DataSource = dataSource;
                gridControlServiceRoom.EndUpdate();
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

        private void addServiceRoomToProcessList(List<ServiceRoomImportADO> _service, ref List<ServiceRoomImportADO> _serviceRef)
        {
            try
            {
                _serviceRef = new List<ServiceRoomImportADO>();
                long i = 0;
                foreach (var item in _service)
                {
                    i++;
                    string error = "";
                    var mateAdo = new ServiceRoomImportADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ServiceRoomImportADO>(mateAdo, item);
                    HIS_SERVICE_TYPE serviceTypeGet = null;
                    HIS_ROOM_TYPE roomTypeGet = null;
                    V_HIS_SERVICE serviceGet = null;
                    V_HIS_ROOM roomGet = null;

                    if (!string.IsNullOrEmpty(item.SERVICE_TYPE_CODE))
                    {
                        if (item.SERVICE_TYPE_CODE.Length > 2)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, " mã loại dịch vụ ");
                        }
                        serviceTypeGet = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(p => p.SERVICE_TYPE_CODE == item.SERVICE_TYPE_CODE);
                        if (serviceTypeGet != null)
                        {
                            mateAdo.SERVICE_TYPE_ID = serviceTypeGet.ID;
                            mateAdo.SERVICE_TYPE_NAME = serviceTypeGet.SERVICE_TYPE_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, " mã loại dịch vụ ");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, " mã loại dịch vụ ");
                    }

                    if (!string.IsNullOrEmpty(item.SERVICE_CODE))
                    {
                        if (item.SERVICE_CODE.Length > 25)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, " mã dịch vụ ");
                        }
                        var serviceGets = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.SERVICE_CODE == item.SERVICE_CODE);
                        if (serviceGets != null && serviceGets.Count() > 0)
                        {
                            if (serviceTypeGet != null)
                            {
                                serviceGet = serviceGets.FirstOrDefault(o => o.SERVICE_TYPE_ID == serviceTypeGet.ID);
                                if (serviceGet != null)
                                {
                                    mateAdo.SERVICE_ID = serviceGet.ID;
                                    mateAdo.SERVICE_NAME = serviceGet.SERVICE_NAME;
                                }
                                else
                                {
                                    error += string.Format(Message.MessageImport.DichVuKhongThuocLoaiDichVu, serviceTypeGet.SERVICE_TYPE_NAME);
                                }
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, " mã dịch vụ ");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, " mã dịch vụ ");
                    }

                    if (!string.IsNullOrEmpty(item.ROOM_TYPE_CODE))
                    {
                        if (item.ROOM_TYPE_CODE.Length > 2)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, " mã loại phòng ");
                        }
                        roomTypeGet = BackendDataWorker.Get<HIS_ROOM_TYPE>().FirstOrDefault(p => p.ROOM_TYPE_CODE == item.ROOM_TYPE_CODE);
                        if (roomTypeGet != null)
                        {
                            mateAdo.ROOM_TYPE_ID = roomTypeGet.ID;
                            mateAdo.ROOM_TYPE_NAME = roomTypeGet.ROOM_TYPE_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, " mã loại phòng ");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, " mã loại phòng");
                    }

                    if (!string.IsNullOrEmpty(item.ROOM_CODE))
                    {
                        if (item.ROOM_CODE.Length > 20)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, " mã phòng ");
                        }
                        var roomGets = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ROOM_CODE == item.ROOM_CODE);
                        if (roomGets != null && roomGets.Count() > 0)
                        {
                            if (roomTypeGet != null)
                            {
                                roomGet = roomGets.FirstOrDefault(o => o.ROOM_TYPE_ID == roomTypeGet.ID);
                                if (roomGet != null)
                                {
                                    mateAdo.ROOM_ID = roomGet.ID;
                                    mateAdo.ROOM_NAME = roomGet.ROOM_NAME;
                                    mateAdo.DEPARTMENT_CODE = roomGet.DEPARTMENT_CODE;
                                    mateAdo.DEPARTMENT_NAME = roomGet.DEPARTMENT_NAME;
                                }
                                else
                                {
                                    error += string.Format(Message.MessageImport.phongKhongThuocLoaiPhong, roomTypeGet.ROOM_TYPE_NAME);
                                }
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, " mã phòng ");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, " mã phòng ");
                    }

                    //if (!string.IsNullOrEmpty(item.ROOM_CODE) && roomTypeGet != null)
                    //{
                    //    if (item.ROOM_CODE.Length > 10)
                    //    {
                    //        error += string.Format(Message.MessageImport.Maxlength, " mã phòng ");
                    //    }
                    //    roomGet = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(p => p.ROOM_CODE == item.ROOM_CODE);
                    //    if (roomGet != null)
                    //    {
                    //        mateAdo.ROOM_ID = roomGet.ID;
                    //        mateAdo.ROOM_NAME = roomGet.ROOM_NAME;
                    //        mateAdo.DEPARTMENT_CODE = roomGet.DEPARTMENT_CODE;
                    //        mateAdo.DEPARTMENT_NAME = roomGet.DEPARTMENT_NAME;
                    //        mateAdo.DEPARTMENT_ID = roomGet.DEPARTMENT_ID;
                    //    }
                    //    else
                    //    {
                    //        error += string.Format(Message.MessageImport.KhongHopLe, " mã phòng ");
                    //    }
                    //}
                    //else
                    //{
                    //    error += string.Format(Message.MessageImport.ThieuTruongDL, " mã phòng ");
                    //}

                    // check trùng dữ liệu đã có trên hệ thống hoặc đã có trên template
                    //var checkTemp = _service.Where(o => o.SERVICE_CODE == item.SERVICE_CODE && o.SERVICE_TYPE_CODE == item.SERVICE_TYPE_CODE && o.ROOM_CODE == item.ROOM_CODE);
                    //if (checkTemp != null && checkTemp.Count() >= 2)
                    //{
                    //    var checkTempOther = checkTemp.Where(o => o.SERVICE_CODE == item.SERVICE_CODE);
                    //    if (checkTempOther != null && checkTempOther.Count() > 0)
                    //    {
                    //        error += string.Format(Message.MessageImport.DuLieuDaTonTaiTrenTemplate, item.SERVICE_CODE, item.SERVICE_TYPE_CODE, item.ROOM_CODE);
                    //    }
                    //}

                    if (_serviceRef.Exists(o => o.SERVICE_CODE == item.SERVICE_CODE && o.SERVICE_TYPE_CODE == item.SERVICE_TYPE_CODE && o.ROOM_CODE == item.ROOM_CODE && o.ROOM_TYPE_CODE == item.ROOM_TYPE_CODE))
                    {
                        error += string.Format(Message.MessageImport.TonTaiTrungNhauTrongFileImport, " (Mã dịch vụ, mã phòng, mã loại dịch vụ, mã loại phòng) ");
                    }

                    if (roomGet != null && roomGet.ID > 0 && serviceTypeGet != null && serviceTypeGet.ID > 0 && serviceGet != null && serviceGet.ID > 0 && roomTypeGet != null)
                    {
                        MOS.Filter.HisServiceRoomViewFilter serviceRoomViewFilter = new MOS.Filter.HisServiceRoomViewFilter();
                        serviceRoomViewFilter.SERVICE_ID = serviceGet.ID;
                        serviceRoomViewFilter.SERVICE_TYPE_ID = serviceTypeGet.ID;
                        serviceRoomViewFilter.ROOM_ID = roomGet.ID;
                        serviceRoomViewFilter.ROOM_TYPE_ID = roomTypeGet.ID;
                        var serviceRooms = new BackendAdapter(null).Get<List<V_HIS_SERVICE_ROOM>>("api/HisServiceRoom/GetView", ApiConsumer.ApiConsumers.MosConsumer, serviceRoomViewFilter, null);
                        if (serviceRooms != null && serviceRooms.Count > 0)
                        {
                            error += string.Format(Message.MessageImport.DuLieuDaTonTaiTrenHeThong, item.SERVICE_CODE, item.SERVICE_TYPE_CODE, item.ROOM_CODE, item.ROOM_TYPE_CODE);
                        }
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                WaitingManager.Show();
                AutoMapper.Mapper.CreateMap<ServiceRoomImportADO, HIS_SERVICE_ROOM>();
                var data = AutoMapper.Mapper.Map<List<HIS_SERVICE_ROOM>>(serviceRoomAdos);
                CommonParam param = new CommonParam();
                var rs = new BackendAdapter(param).Post<List<HIS_SERVICE_PATY>>("api/HisServiceRoom/CreateList", ApiConsumers.MosConsumer, data, param);
                if (rs != null)
                {
                    success = true;
                    BackendDataWorker.Reset<V_HIS_SERVICE_ROOM>();
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
                var row = (ServiceRoomImportADO)gridViewServiceRoom.GetFocusedRow();
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
                var row = (ServiceRoomImportADO)gridViewServiceRoom.GetFocusedRow();
                if (row != null)
                {
                    if (serviceRoomAdos != null && serviceRoomAdos.Count > 0)
                    {
                        serviceRoomAdos.Remove(row);
                        SetDataSource(serviceRoomAdos);
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
                    var errorLine = serviceRoomAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = serviceRoomAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
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
                    ServiceRoomImportADO data = (ServiceRoomImportADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
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
                    ServiceRoomImportADO pData = (ServiceRoomImportADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        private void frmServiceRoom_Load(object sender, EventArgs e)
        {
            try
            {
                btnSave.Enabled = false;
                btnShowLineError.Enabled = false;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
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
                + "/Tmp/Imp", "IMPORT_SERVICE_ROOM.xlsx");

                if (File.Exists(source))
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                    saveFileDialog1.Title = "Save File";
                    saveFileDialog1.FileName = "IMPORT_SERVICE_ROOM";
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
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    WaitingManager.Show();

                    var import = new Inventec.Common.ExcelImport.Import();
                    if (import.ReadFileExcel(ofd.FileName))
                    {
                        var hisServiceRoomImport = import.GetWithCheck<ServiceRoomImportADO>(0);
                        if (hisServiceRoomImport != null && hisServiceRoomImport.Count > 0)
                        {
                            List<ServiceRoomImportADO> listAfterRemove = new List<ServiceRoomImportADO>();
                            foreach (var item in hisServiceRoomImport)
                            {
                                listAfterRemove.Add(item);
                            }

                            foreach (var item in hisServiceRoomImport)
                            {
                                bool checkNull = string.IsNullOrEmpty(item.SERVICE_CODE)
                                    && string.IsNullOrEmpty(item.ROOM_CODE)
                                    && string.IsNullOrEmpty(item.SERVICE_TYPE_CODE);

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
                                serviceRoomAdos = new List<ServiceRoomImportADO>();
                                addServiceRoomToProcessList(currentAdos, ref serviceRoomAdos);
                                SetDataSource(serviceRoomAdos);
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
                    WaitingManager.Hide();

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
