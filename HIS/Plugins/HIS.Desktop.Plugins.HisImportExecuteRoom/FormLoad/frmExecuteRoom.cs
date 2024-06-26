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
using HIS.Desktop.Plugins.HisImportExecuteRoom.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
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

namespace HIS.Desktop.Plugins.HisImportExecuteRoom.FormLoad
{
    public partial class frmExecuteRoom : HIS.Desktop.Utility.FormBase
    {
        List<ExecuteRoomImportADO> executeRoomAdos;
        List<ExecuteRoomImportADO> currentAdos;
        List<HIS_EXECUTE_ROOM> listExecuteRoom;
        List<HIS_ROOM_GROUP> listRoomGroup;
        List<HIS_SPECIALITY> listSpeciality;
        RefeshReference delegateRefresh;
        bool checkClick;
        Inventec.Desktop.Common.Modules.Module currentModule;

        public frmExecuteRoom(Inventec.Desktop.Common.Modules.Module module, RefeshReference _delegate)
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

        public frmExecuteRoom(Inventec.Desktop.Common.Modules.Module module)
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

        private void CheckErrorLine(List<ExecuteRoomImportADO> dataSource)
        {
            try
            {
                if (executeRoomAdos != null && executeRoomAdos.Count > 0)
                {
                    var checkError = executeRoomAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
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

        private void SetDataSource(List<ExecuteRoomImportADO> dataSource)
        {
            try
            {
                gridControlExecuteRoom.BeginUpdate();
                gridControlExecuteRoom.DataSource = null;
                gridControlExecuteRoom.DataSource = dataSource;
                gridControlExecuteRoom.EndUpdate();
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

        private void addExecuteRoomToProcessList(List<ExecuteRoomImportADO> _executeRoom, ref List<ExecuteRoomImportADO> _executeRoomRef)
        {
            try
            {
                _executeRoomRef = new List<ExecuteRoomImportADO>();
                long i = 0;
                foreach (var item in _executeRoom)
                {
                    i++;
                    string error = "";
                    var mateAdo = new ExecuteRoomImportADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ExecuteRoomImportADO>(mateAdo, item);

                    if (!string.IsNullOrEmpty(item.DEPARTMENT_CODE))
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.DEPARTMENT_CODE, 20))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã khoa");
                        }
                        var department = BackendDataWorker.Get<HIS_DEPARTMENT>();
                        if (department != null && department.Count > 0)
                        {
                            var serviceGet = department.FirstOrDefault(o => o.DEPARTMENT_CODE == item.DEPARTMENT_CODE);
                            if (serviceGet != null)
                            {
                                mateAdo.DEPARTMENT_ID = serviceGet.ID;
                                mateAdo.DEPARTMENT_NAME = serviceGet.DEPARTMENT_NAME;
                            }
                            else
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Mã khoa");
                            }
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã khoa");
                    }

                    if (!string.IsNullOrEmpty(item.ROOM_GROUP_CODE))
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.ROOM_GROUP_CODE, 10))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã nhóm phòng");
                        }

                        if (listRoomGroup != null && listRoomGroup.Count > 0)
                        {
                            var roomGroupGet = listRoomGroup.FirstOrDefault(o => o.ROOM_GROUP_CODE == item.ROOM_GROUP_CODE);
                            if (roomGroupGet != null)
                            {
                                mateAdo.ROOM_GROUP_ID = roomGroupGet.ID;
                                mateAdo.ROOM_GROUP_NAME = roomGroupGet.ROOM_GROUP_NAME;
                            }
                            else
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Mã nhóm phòng");
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(item.SPECIALITY_CODE))
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.SPECIALITY_CODE, 50))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã chuyên khoa");
                        }

                        if (listSpeciality != null && listSpeciality.Count > 0)
                        {
                            var specialityGet = listSpeciality.FirstOrDefault(o => o.SPECIALITY_CODE == item.SPECIALITY_CODE);
                            if (specialityGet != null)
                            {
                                mateAdo.SPECIALITY_ID = specialityGet.ID;
                                mateAdo.SPECIALITY_NAME = specialityGet.SPECIALITY_NAME;
                            }
                            else
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Mã chuyên khoa");
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(item.EXECUTE_ROOM_CODE))
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.EXECUTE_ROOM_CODE, 20))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã phòng xử lý");
                        }
                        if (listExecuteRoom != null && listExecuteRoom.Count > 0)
                        {
                            var check = listExecuteRoom.Exists(o => o.EXECUTE_ROOM_CODE == item.EXECUTE_ROOM_CODE);
                            if (check)
                            {
                                error += string.Format(Message.MessageImport.DaTonTai, "Mã phòng xử lý");
                            }
                        }
                        var checkExel = _executeRoomRef.FirstOrDefault(o => o.EXECUTE_ROOM_CODE == item.EXECUTE_ROOM_CODE);
                        if (checkExel != null)
                        {
                            error += string.Format(Message.MessageImport.TonTaiTrungNhauTrongFileImport, "Mã phòng xử lý");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã phòng xử lý");
                    }

                    if (!string.IsNullOrEmpty(item.EXECUTE_ROOM_NAME))
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.EXECUTE_ROOM_NAME, 100))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Tên phòng xử lý");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Tên phòng xử lý");
                    }

                    if (!string.IsNullOrEmpty(item.NUM_ORDER_STR))
                    {
                        if (Inventec.Common.Number.Check.IsNumber(item.NUM_ORDER_STR))
                        {
                            mateAdo.NUM_ORDER = Inventec.Common.TypeConvert.Parse.ToInt64(item.NUM_ORDER_STR);
                            if (mateAdo.NUM_ORDER.ToString().Length > 19 || mateAdo.NUM_ORDER < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "STT hiện");
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "STT hiện");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.ORDER_ISSUE_CODE))
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.ORDER_ISSUE_CODE, 10))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã sinh STT");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.ADDRESS))
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.ADDRESS, 200))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Địa chỉ");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.MAX_REQUEST_BY_DAY_STR))
                    {
                        if (Inventec.Common.Number.Check.IsNumber(item.MAX_REQUEST_BY_DAY_STR))
                        {
                            mateAdo.MAX_REQUEST_BY_DAY = Inventec.Common.TypeConvert.Parse.ToInt64(item.MAX_REQUEST_BY_DAY_STR);
                            if (mateAdo.MAX_REQUEST_BY_DAY.ToString().Length > 19 || mateAdo.MAX_REQUEST_BY_DAY < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Số lượt xử lý tối đa/ngày");
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Số lượt xử lý tối đa/ngày");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.MAX_REQ_BHYT_BY_DAY_STR))
                    {
                        if (Inventec.Common.Number.Check.IsNumber(item.MAX_REQ_BHYT_BY_DAY_STR))
                        {
                            mateAdo.MAX_REQ_BHYT_BY_DAY = Inventec.Common.TypeConvert.Parse.ToInt64(item.MAX_REQ_BHYT_BY_DAY_STR);
                            if (mateAdo.MAX_REQ_BHYT_BY_DAY.ToString().Length > 19 || mateAdo.MAX_REQ_BHYT_BY_DAY < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Số lượt BHYT");
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Số lượt BHYT");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.HOLD_ORDER_STR))
                    {
                        if (Inventec.Common.Number.Check.IsNumber(item.HOLD_ORDER_STR))
                        {
                            mateAdo.HOLD_ORDER = Inventec.Common.TypeConvert.Parse.ToInt64(item.HOLD_ORDER_STR);
                            if (mateAdo.HOLD_ORDER.ToString().Length > 19 || mateAdo.HOLD_ORDER < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Dải STT ưu tiên");
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Dải STT ưu tiên");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.EMERGENCY))
                    {
                        if (item.EMERGENCY == "x")
                        {
                            mateAdo.IS_EMERGENCY = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Phòng cấp cứu");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.EXAM))
                    {
                        if (item.EXAM == "x")
                        {
                            mateAdo.IS_EXAM = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Là phòng khám");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.PAUSE))
                    {
                        if (item.PAUSE == "x")
                        {
                            mateAdo.IS_PAUSE = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Tạm dừng");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.RESTRICT_EXECUTE_ROOM))
                    {
                        if (item.RESTRICT_EXECUTE_ROOM == "x")
                        {
                            mateAdo.IS_RESTRICT_EXECUTE_ROOM = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Giới hạn chỉ định phòng thực hiện");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.USE_KIOSK))
                    {
                        if (item.USE_KIOSK == "x")
                        {
                            mateAdo.IS_USE_KIOSK = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Là phòng kiosk");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.RESTRICT_MEDICINE_TYPE))
                    {
                        if (item.RESTRICT_MEDICINE_TYPE == "x")
                        {
                            mateAdo.IS_RESTRICT_MEDICINE_TYPE = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Giới hạn sử dụng thuốc");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.RESTRICT_TIME))
                    {
                        if (item.RESTRICT_TIME == "x")
                        {
                            mateAdo.IS_RESTRICT_TIME = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Giới hạn thời gian hoạt động");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.SPECIALITY))
                    {
                        if (item.SPECIALITY == "x")
                        {
                            mateAdo.IS_SPECIALITY = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Là phòng chuyên khoa");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.SURGERY))
                    {
                        if (item.SURGERY == "x")
                        {
                            mateAdo.IS_SURGERY = 1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Là phòng mổ");
                        }
                    }

                    mateAdo.ERROR = error;
                    mateAdo.ID = i;

                    _executeRoomRef.Add(mateAdo);
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

                List<MOS.SDO.HisExecuteRoomSDO> executeRoomSDOs = new List<MOS.SDO.HisExecuteRoomSDO>();

                //AutoMapper.Mapper.CreateMap<ExecuteRoomImportADO, HIS_EXECUTE_ROOM>();
                //var data = AutoMapper.Mapper.Map<List<HIS_EXECUTE_ROOM>>(executeRoomAdos);
                if (executeRoomAdos != null && executeRoomAdos.Count > 0)
                {
                    foreach (var item in executeRoomAdos)
                    {
                        item.ID = 0;
                        MOS.SDO.HisExecuteRoomSDO sdo = new MOS.SDO.HisExecuteRoomSDO();
                        HIS_ROOM room = new HIS_ROOM();
                        HIS_EXECUTE_ROOM executeRoom = new HIS_EXECUTE_ROOM();

                        room.ROOM_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL;
                        room.DEPARTMENT_ID = item.DEPARTMENT_ID;
                        room.IS_PAUSE = item.IS_PAUSE;
                        room.IS_USE_KIOSK = item.IS_USE_KIOSK;
                        room.ORDER_ISSUE_CODE = item.ORDER_ISSUE_CODE;
                        room.ROOM_GROUP_ID = item.ROOM_GROUP_ID;
                        room.IS_RESTRICT_TIME = item.IS_RESTRICT_TIME;
                        room.IS_RESTRICT_EXECUTE_ROOM = item.IS_RESTRICT_EXECUTE_ROOM;
                        room.IS_RESTRICT_MEDICINE_TYPE = item.IS_RESTRICT_MEDICINE_TYPE;
                        room.SPECIALITY_ID = item.SPECIALITY_ID;
                        room.HOLD_ORDER = item.HOLD_ORDER;
                        room.ADDRESS = item.ADDRESS;

                        executeRoom.EXECUTE_ROOM_CODE = item.EXECUTE_ROOM_CODE;
                        executeRoom.EXECUTE_ROOM_NAME = item.EXECUTE_ROOM_NAME;
                        executeRoom.IS_EMERGENCY = item.IS_EMERGENCY;
                        executeRoom.IS_SPECIALITY = item.IS_SPECIALITY;
                        executeRoom.IS_SURGERY = item.IS_SURGERY;
                        executeRoom.IS_EXAM = item.IS_EXAM;
                        executeRoom.NUM_ORDER = item.NUM_ORDER;
                        executeRoom.MAX_REQUEST_BY_DAY = item.MAX_REQUEST_BY_DAY;
                        executeRoom.MAX_REQ_BHYT_BY_DAY = item.MAX_REQ_BHYT_BY_DAY;

                        sdo.HisRoom = room;
                        sdo.HisExecuteRoom = executeRoom;

                        executeRoomSDOs.Add(sdo);
                    }
                }
                CommonParam param = new CommonParam();
                var rs = new BackendAdapter(param)
                        .Post<List<MOS.SDO.HisExecuteRoomSDO>>("api/HisExecuteRoom/CreateList", ApiConsumers.MosConsumer, executeRoomSDOs, param);
                if (rs != null)
                {
                    success = true;
                    btnSave.Enabled = false;
                    GetData();
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
                var row = (ExecuteRoomImportADO)gridViewExecuteRoom.GetFocusedRow();
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
                var row = (ExecuteRoomImportADO)gridViewExecuteRoom.GetFocusedRow();
                if (row != null)
                {
                    if (executeRoomAdos != null && executeRoomAdos.Count > 0)
                    {
                        executeRoomAdos.Remove(row);
                        gridControlExecuteRoom.DataSource = null;
                        gridControlExecuteRoom.DataSource = executeRoomAdos;
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
                    var errorLine = executeRoomAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = executeRoomAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewExecuteRoom_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    ExecuteRoomImportADO data = (ExecuteRoomImportADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
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

        private void gridViewExecuteRoom_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ExecuteRoomImportADO pData = (ExecuteRoomImportADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                    //            e.Value = Inventec.Common.Resource.Get.Value("frmHisMisuExecuteRoomType.HoatDong", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    //        else
                    //            e.Value = Inventec.Common.Resource.Get.Value("frmHisMisuExecuteRoomType.TamKhoa", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                    else if (e.Column.FieldName == "CapCuu")
                    {
                        try
                        {
                            e.Value = pData.IS_EMERGENCY == 1 ? true : false;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua PhongCapCuu", ex);
                        }
                    }
                    else if (e.Column.FieldName == "PhongMo")
                    {
                        try
                        {
                            e.Value = pData.IS_SURGERY == 1 ? true : false;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua PhongMo", ex);
                        }
                    }
                    else if (e.Column.FieldName == "ChuyenKhoa")
                    {
                        try
                        {
                            e.Value = pData.IS_SPECIALITY == 1 ? true : false;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua PhongChuyenKhoa", ex);
                        }
                    }
                    else if (e.Column.FieldName == "PhongKham")
                    {
                        try
                        {
                            e.Value = pData.IS_EXAM == 1 ? true : false;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua PhongKham", ex);
                        }
                    }
                    else if (e.Column.FieldName == "PhongKiosk")
                    {
                        try
                        {
                            e.Value = pData.IS_USE_KIOSK == 1 ? true : false;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua PhongKiosk", ex);
                        }
                    }
                    else if (e.Column.FieldName == "TamDung")
                    {
                        try
                        {
                            e.Value = pData.IS_PAUSE == 1 ? true : false;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua TamDung", ex);
                        }
                    }
                    else if (e.Column.FieldName == "GioiHanChiDinhPTH")
                    {
                        try
                        {
                            e.Value = pData.IS_RESTRICT_EXECUTE_ROOM == 1 ? true : false;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua GioiHanChiDinhPTH", ex);
                        }
                    }
                    else if (e.Column.FieldName == "GioiHanSDThuoc")
                    {
                        try
                        {
                            e.Value = pData.IS_RESTRICT_MEDICINE_TYPE == 1 ? true : false;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua GioiHanSDThuoc", ex);
                        }
                    }
                    else if (e.Column.FieldName == "GioiHanTGHD")
                    {
                        try
                        {
                            e.Value = pData.IS_RESTRICT_TIME == 1 ? true : false;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua GioiHanTGHD", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetData()
        {
            try
            {
                listExecuteRoom = new List<HIS_EXECUTE_ROOM>();
                CommonParam param = new CommonParam();

                HisExecuteRoomFilter executeRoomFilter = new HisExecuteRoomFilter();
                listExecuteRoom = new BackendAdapter(param).Get<List<HIS_EXECUTE_ROOM>>("api/HisExecuteRoom/Get", ApiConsumers.MosConsumer, executeRoomFilter, param);

                HisRoomGroupFilter roomGroupFilter = new HisRoomGroupFilter();
                listRoomGroup = new BackendAdapter(param).Get<List<HIS_ROOM_GROUP>>("api/HisRoomGroup/Get", ApiConsumers.MosConsumer, roomGroupFilter, param);

                HisSpecialityFilter specialityFilter = new HisSpecialityFilter();
                listSpeciality = new BackendAdapter(param).Get<List<HIS_SPECIALITY>>("api/HisSpeciality/Get", ApiConsumers.MosConsumer, specialityFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void frmExecuteRoom_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                btnSave.Enabled = false;
                btnShowLineError.Enabled = false;
                GetData();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                var source = System.IO.Path.Combine(Application.StartupPath
                + "/Tmp/Imp", "IMPORT_EXECUTE_ROOM.xlsx");

                if (File.Exists(source))
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                    saveFileDialog1.Title = "Save File";
                    saveFileDialog1.FileName = "IMPORT_EXECUTE_ROOM";
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
                        var hisExecuteRoomImport = import.GetWithCheck<ExecuteRoomImportADO>(0);
                        if (hisExecuteRoomImport != null && hisExecuteRoomImport.Count > 0)
                        {
                            List<ExecuteRoomImportADO> listAfterRemove = new List<ExecuteRoomImportADO>();
                            foreach (var item in hisExecuteRoomImport)
                            {
                                listAfterRemove.Add(item);
                            }

                            foreach (var item in hisExecuteRoomImport)
                            {
                                bool checkNull = string.IsNullOrEmpty(item.EXECUTE_ROOM_NAME)
                                    && string.IsNullOrEmpty(item.NUM_ORDER_STR)
                                    && string.IsNullOrEmpty(item.EXECUTE_ROOM_CODE)
                                    && string.IsNullOrEmpty(item.DEPARTMENT_CODE)
                                    && string.IsNullOrEmpty(item.ROOM_GROUP_CODE)
                                    && string.IsNullOrEmpty(item.ORDER_ISSUE_CODE)
                                    && string.IsNullOrEmpty(item.MAX_REQ_BHYT_BY_DAY_STR)
                                    && string.IsNullOrEmpty(item.MAX_REQUEST_BY_DAY_STR)
                                    && string.IsNullOrEmpty(item.HOLD_ORDER_STR)
                                    && string.IsNullOrEmpty(item.SPECIALITY_CODE)
                                    && string.IsNullOrEmpty(item.ADDRESS)
                                    && string.IsNullOrEmpty(item.EMERGENCY)
                                    && string.IsNullOrEmpty(item.EXAM)
                                    && string.IsNullOrEmpty(item.PAUSE)
                                    && string.IsNullOrEmpty(item.RESTRICT_EXECUTE_ROOM)
                                    && string.IsNullOrEmpty(item.RESTRICT_MEDICINE_TYPE)
                                    && string.IsNullOrEmpty(item.RESTRICT_TIME)
                                    && string.IsNullOrEmpty(item.SPECIALITY)
                                    && string.IsNullOrEmpty(item.SURGERY)
                                    && string.IsNullOrEmpty(item.USE_KIOSK);

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
                                executeRoomAdos = new List<ExecuteRoomImportADO>();
                                addExecuteRoomToProcessList(currentAdos, ref executeRoomAdos);
                                SetDataSource(executeRoomAdos);
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
