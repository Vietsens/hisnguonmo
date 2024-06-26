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
using HIS.Desktop.Plugins.ImportHisCashierAddCfg.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
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
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ImportHisCashierAddCfg.FormLoad
{
    public partial class frmCashierAddCfg : HIS.Desktop.Utility.FormBase
    {
        List<CashierAddCfgImportADO> serviceAdos;
        List<CashierAddCfgImportADO> currentAdos;
        RefeshReference delegateRefresh;
        bool checkClick;
        Inventec.Desktop.Common.Modules.Module currentModule;
        Dictionary<string, HIS_CASHIER_ADD_CONFIG> DicService;
        List<V_HIS_ROOM> roomlist;
        List<HIS_CASHIER_ROOM> CashierRoomList;

        public frmCashierAddCfg(Inventec.Desktop.Common.Modules.Module module, RefeshReference _delegate)
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

        public frmCashierAddCfg(Inventec.Desktop.Common.Modules.Module module)
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

        private void CheckErrorLine(List<CashierAddCfgImportADO> dataSource)
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

        private void SetDataSource(List<CashierAddCfgImportADO> dataSource)
        {
            try
            {
                gridControl1.BeginUpdate();
                gridControl1.DataSource = null;
                gridControl1.DataSource = dataSource;
                gridControl1.EndUpdate();
                CheckErrorLine(null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string convertDay(string day) 
        {
            string result = "";
            switch (day)
            {
                case "1":
                    result = "Chủ nhật";
                    break;
                case "2":
                    result = "Thứ 2";
                    break;
                case "3":
                    result = "Thứ 3";
                    break;
                case "4":
                    result = "Thứ 4";
                    break;
                case "5":
                    result = "Thứ 5";
                    break;
                case "6":
                    result = "Thứ 6";
                    break;
                case "7":
                    result = "Thứ 7";
                    break;
                default:
                    break;
            }

            return result;
        }

        public bool IsTime(string inputString)
        {
            bool check = false;
            try
            {
                if (inputString.Length > 5)
                {
                    check = false;
                }
                else if (inputString.Length < 5)
                {
                    check = false;
                }
                else
                {

                    if (inputString.IndexOf(":") == 2)
                    {
                        string[] substring = inputString.Split(':');
                        if (substring.Count() == 2)
                        {
                            int value1, value2;
                            bool check1 = false, check2 = false;

                            if (int.TryParse(substring[0], out value1)) check1 = true;
                            else check1 = false;
                            if (int.TryParse(substring[1], out value2)) check2 = true;
                            else check2 = false;

                            if (check1 == true && check2 == true)
                            {
                                check = true;
                            }
                            else
                            {
                                check = false;
                            }
                        }
                        else
                        {
                            check = false;
                        }
                    }
                    else
                    {
                        check = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return check;

        }

        private void GetTime(string inputstring, ref string time, ref string error)
        {
            try
            {

                string[] substring = inputstring.Split(':');
                if (substring.Count() == 2)
                {
                    int hours = Int32.Parse(substring[0]), minute = Int32.Parse(substring[1]);

                    if (hours < 0 || hours > 23)
                    {
                        error += Message.MessageImport.ThoiGianKhongChinhXac;
                    }

                    if (minute < 0 || minute > 59)
                    {
                        error += Message.MessageImport.ThoiGianKhongChinhXac;
                    }

                    time = substring[0] + substring[1];

                }
                else
                {
                    error += Message.MessageImport.DinhDangThoiGian;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void addServiceToProcessList(List<CashierAddCfgImportADO> _service, ref List<CashierAddCfgImportADO> _serviceRef)
        {
            try
            {
                _serviceRef = new List<CashierAddCfgImportADO>();
                long i = 0;
                roomlist = new List<V_HIS_ROOM>();
                CashierRoomList = new List<HIS_CASHIER_ROOM>();
                roomlist = BackendDataWorker.Get<V_HIS_ROOM>();
                CashierRoomList = BackendDataWorker.Get<HIS_CASHIER_ROOM>();

                foreach (var item in _service)
                {
                    i++;
                    string error = "";
                    var serAdo = new CashierAddCfgImportADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<CashierAddCfgImportADO>(serAdo, item);

                    if (!string.IsNullOrEmpty(item.REQUEST_ROOM_CODE_STR) || !string.IsNullOrEmpty(item.EXECUTE_ROOM_CODE_STR))
                    {
                        
                        if (!string.IsNullOrEmpty(item.REQUEST_ROOM_CODE_STR))
                        {
                            var REQUEST_ROOM_NAME = roomlist.Where(o => o.ROOM_CODE.ToLower() == item.REQUEST_ROOM_CODE_STR.ToLower()).ToList();

                            if (REQUEST_ROOM_NAME != null && REQUEST_ROOM_NAME.Count > 0)
                            {
                                item.REQUEST_ROOM_NAME_STR = REQUEST_ROOM_NAME.FirstOrDefault().ROOM_NAME;
                            }
                            else 
                            {
                                error += string.Format(Message.MessageImport.MaPhongKhongTonTai, item.REQUEST_ROOM_CODE_STR);
                            }
                        }

                        if (!string.IsNullOrEmpty(item.EXECUTE_ROOM_CODE_STR))
                        {
                            var EXECUTE_ROOM = roomlist.Where(o => o.ROOM_CODE.ToLower() == item.EXECUTE_ROOM_CODE_STR.ToLower()).ToList();
                            if (EXECUTE_ROOM != null && EXECUTE_ROOM.Count >0)
                            {
                                item.EXECUTE_ROOM_NAME_STR = EXECUTE_ROOM.FirstOrDefault().ROOM_NAME;
                            }
                            else
                            {
                                error += string.Format(Message.MessageImport.MaPhongKhongTonTai, item.EXECUTE_ROOM_CODE_STR);
                            }
                        }
                    }
                    else 
                    {
                        error += Message.MessageImport.ThieuMaPhongYeuCauVaMaPhongXuLy;
                    }

                    if (!string.IsNullOrEmpty(item.CASHIER_ROOM_CODE_STR))
                    {
                        var CASHIER_ROOM = CashierRoomList.Where(o => o.CASHIER_ROOM_CODE.ToLower() == item.CASHIER_ROOM_CODE_STR.ToLower()).ToList();

                        if (CASHIER_ROOM != null && CASHIER_ROOM.Count >0)
                        {
                            item.CASHIER_ROOM_NAME_STR = CASHIER_ROOM.FirstOrDefault().CASHIER_ROOM_NAME;
                        }
                        else 
                        {
                            error += string.Format(Message.MessageImport.MaPhongKhongTonTai, item.CASHIER_ROOM_CODE_STR);
                        }
                    }
                    else 
                    {
                        error += Message.MessageImport.ThieuMaPhongThuNgan;
                    }

                    if (!string.IsNullOrEmpty(item.INSTR_DAY_FROM_STR) || !string.IsNullOrEmpty(item.INSTR_DAY_TO_STR))
                    {
                        string errorDay1 = null, errorDay2 = null;

                        //if (Equals(item.INSTR_DAY_FROM_STR, "1") || Equals(item.INSTR_DAY_FROM_STR, "2") || Equals(item.INSTR_DAY_FROM_STR, "3") || Equals(item.INSTR_DAY_FROM_STR, "4") || Equals(item.INSTR_DAY_FROM_STR, "5") || Equals(item.INSTR_DAY_FROM_STR, "6") || Equals(item.INSTR_DAY_FROM_STR, "7") || Equals(item.INSTR_DAY_TO_STR, "1") || Equals(item.INSTR_DAY_TO_STR, "2") || Equals(item.INSTR_DAY_TO_STR, "3") || Equals(item.INSTR_DAY_TO_STR, "4") || Equals(item.INSTR_DAY_TO_STR, "5") || Equals(item.INSTR_DAY_TO_STR, "6") || Equals(item.INSTR_DAY_TO_STR, "7"))
                        //{
                        //    //TODO
                        //}
                        //else
                        //{
                        //    error += Message.MessageImport.NgayKhongChinhXac;
                        //}

                        if (!string.IsNullOrEmpty(item.INSTR_DAY_FROM_STR))
                        {
                            if (Equals(item.INSTR_DAY_FROM_STR, "1") || Equals(item.INSTR_DAY_FROM_STR, "2") || Equals(item.INSTR_DAY_FROM_STR, "3") || Equals(item.INSTR_DAY_FROM_STR, "4") || Equals(item.INSTR_DAY_FROM_STR, "5") || Equals(item.INSTR_DAY_FROM_STR, "6") || Equals(item.INSTR_DAY_FROM_STR, "7"))
                            {
                                item.INSTR_DAY_FROM = long.Parse(item.INSTR_DAY_FROM_STR);
                                item.INSTR_DAY_FROM_STR = convertDay(item.INSTR_DAY_FROM_STR);
                            }
                            else 
                            {
                                errorDay1 = Message.MessageImport.NgayKhongChinhXac;
                                item.INSTR_DAY_FROM = null;
                                item.INSTR_DAY_FROM_STR = item.INSTR_DAY_FROM_STR;
                            }
                        }
                        else
                        {
                            item.INSTR_DAY_FROM = null;
                            item.INSTR_DAY_FROM_STR = null;
                        }
                        if (!string.IsNullOrEmpty(item.INSTR_DAY_TO_STR))
                        {
                            if (Equals(item.INSTR_DAY_TO_STR, "1") || Equals(item.INSTR_DAY_TO_STR, "2") || Equals(item.INSTR_DAY_TO_STR, "3") || Equals(item.INSTR_DAY_TO_STR, "4") || Equals(item.INSTR_DAY_TO_STR, "5") || Equals(item.INSTR_DAY_TO_STR, "6") || Equals(item.INSTR_DAY_TO_STR, "7"))
                            {
                                item.INSTR_DAY_TO = long.Parse(item.INSTR_DAY_TO_STR);
                                item.INSTR_DAY_TO_STR = convertDay(item.INSTR_DAY_TO_STR);
                            }
                            else
                            {
                                errorDay2 = Message.MessageImport.NgayKhongChinhXac;
                                item.INSTR_DAY_TO = null;
                                item.INSTR_DAY_TO_STR = item.INSTR_DAY_TO_STR;
                            }
                        }
                        else
                        {
                            item.INSTR_DAY_TO = null;
                            item.INSTR_DAY_TO_STR = null;
                        }

                        if (String.Compare(errorDay1, errorDay2, true) == 0)
                        {
                            error += errorDay1;
                        }
                        else 
                        {
                            if (!string.IsNullOrEmpty(errorDay1))
                            {
                                error += errorDay1;
                            }
                            else if (!string.IsNullOrEmpty(errorDay2))
                            {
                                error += errorDay2;
                            }
                        }

                        if (item.INSTR_DAY_FROM != null && item.INSTR_DAY_TO !=null)
                        {
                            if (item.INSTR_DAY_FROM > item.INSTR_DAY_TO)
                            {
                                error += Message.MessageImport.NgayTuNhoHonNgayDen;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(item.INSTR_TIME_FROM_STR) || !string.IsNullOrEmpty(item.INSTR_TIME_TO_STR))
                    {
                        bool timeForm = true, timeTo = true;
                        if (!string.IsNullOrEmpty(item.INSTR_TIME_FROM_STR))
                        {
                            timeForm = IsTime(item.INSTR_TIME_FROM_STR);
                        }

                        if (!string.IsNullOrEmpty(item.INSTR_TIME_TO_STR))
                        {
                            timeTo = IsTime(item.INSTR_TIME_TO_STR);
                        }

                        if (!timeForm || !timeTo)
                        {
                            error += Message.MessageImport.DinhDangThoiGian;
                        }else
                        {
                            
                            if (!string.IsNullOrEmpty(item.INSTR_TIME_FROM_STR))
                            {
                                string TIME_FROM = "", errorF = "";
                                GetTime(item.INSTR_TIME_FROM_STR, ref TIME_FROM, ref errorF);
                                error += errorF;
                                item.INSTR_TIME_FROM = TIME_FROM;
                            }

                            if (!string.IsNullOrEmpty(item.INSTR_TIME_TO_STR))
                            {
                                string TIME_TO = "", errorT = "";
                                GetTime(item.INSTR_TIME_TO_STR, ref TIME_TO, ref errorT);

                                item.INSTR_TIME_TO = TIME_TO;
                            }

                            if (!string.IsNullOrEmpty(item.INSTR_TIME_FROM_STR) && !string.IsNullOrEmpty(item.INSTR_TIME_TO_STR))
                            {
                                if (Int32.Parse(item.INSTR_TIME_FROM) > Int32.Parse(item.INSTR_TIME_TO))
                                {
                                    error += Message.MessageImport.ThoiGianTuNhoHonNgayDen;
                                }
                            }
                        }

                    }

                    if (!string.IsNullOrEmpty(item.IS_NOT_PRIORITY_STR))
                    {
                        if (item.IS_NOT_PRIORITY_STR == "X" || item.IS_NOT_PRIORITY_STR=="x")
                        {
                            item.IS_NOT_PRIORITY = 1;
                        }
                        else
                        {
                            item.IS_NOT_PRIORITY = null;
                        }
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
                List<HIS_CASHIER_ADD_CONFIG> data = new List<HIS_CASHIER_ADD_CONFIG>();

                for (int i = 0; i < serviceAdos.Count; i++)
                {
                    HIS_CASHIER_ADD_CONFIG item=new HIS_CASHIER_ADD_CONFIG();
                    string TIME_FROM = "", TIME_TO = "", a = "";

                    if (!string.IsNullOrEmpty(serviceAdos[i].REQUEST_ROOM_CODE_STR))
                    {
                        item.REQUEST_ROOM_ID = roomlist.FirstOrDefault(o => o.ROOM_CODE.ToLower() == serviceAdos[i].REQUEST_ROOM_CODE_STR.ToLower()).ID;
                    }
                    else 
                    {
                        item.REQUEST_ROOM_ID = null;
                    }


                    if (!string.IsNullOrEmpty(serviceAdos[i].EXECUTE_ROOM_CODE_STR))
                    {
                        item.EXECUTE_ROOM_ID = roomlist.FirstOrDefault(o => o.ROOM_CODE.ToLower() == serviceAdos[i].EXECUTE_ROOM_CODE_STR.ToLower()).ID;
                    }else
                    {
                        item.EXECUTE_ROOM_ID =null;
                    }

                    item.CASHIER_ROOM_ID = CashierRoomList.FirstOrDefault(o => o.CASHIER_ROOM_CODE.ToLower() == serviceAdos[i].CASHIER_ROOM_CODE_STR.ToLower()).ID;

                    if (!string.IsNullOrEmpty(serviceAdos[i].INSTR_DAY_FROM_STR))
                    {
                        item.INSTR_DAY_FROM = long.Parse(serviceAdos[i].INSTR_DAY_FROM_STR);
                    }
                    else
                    {
                        item.INSTR_DAY_FROM = null;
                    }

                    if (!string.IsNullOrEmpty(serviceAdos[i].INSTR_DAY_TO_STR))
                    {
                        item.INSTR_DAY_TO = long.Parse(serviceAdos[i].INSTR_DAY_TO_STR);
                    }
                    else 
                    {
                        item.INSTR_DAY_TO = null;
                    }


                    GetTime(serviceAdos[i].INSTR_TIME_FROM_STR, ref TIME_FROM, ref a);
                    item.INSTR_TIME_FROM = TIME_FROM;

                    GetTime(serviceAdos[i].INSTR_TIME_TO_STR, ref TIME_TO, ref a);
                    item.INSTR_TIME_TO = TIME_TO;

                    if (serviceAdos[i].IS_NOT_PRIORITY_STR == "X" || serviceAdos[i].IS_NOT_PRIORITY_STR == "x")
                    {
                        item.IS_NOT_PRIORITY = 1;
                    }
                    else 
                    {
                        item.IS_NOT_PRIORITY = null;
                    }

                    data.Add(item);
                }

                  Inventec.Common.Logging.LogSystem.Debug("đữ liệu data: "+ Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));

                if (data != null && data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        item.ID = 0;
                    }
                }
                
                CommonParam param = new CommonParam();
                var rs = new BackendAdapter(param).Post<List<HIS_CASHIER_ADD_CONFIG>>("api/HisCashierAddConfig/CreateList", ApiConsumers.MosConsumer, data, param);
                if (rs != null)
                {
                    success = true;
                    btnSave.Enabled = false;
                    BackendDataWorker.Reset<HIS_CASHIER_ADD_CONFIG>();
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
                var row = (CashierAddCfgImportADO)gridView1.GetFocusedRow();
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
                var row = (CashierAddCfgImportADO)gridView1.GetFocusedRow();
                if (row != null)
                {
                    if (serviceAdos != null && serviceAdos.Count > 0)
                    {
                        serviceAdos.Remove(row);
                        gridControl1.DataSource = null;
                        gridControl1.DataSource = serviceAdos;
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

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    CashierAddCfgImportADO  data = (CashierAddCfgImportADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
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
                    else if (e.Column.FieldName == "IS_NOT_PRIORITY_STR1")
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

        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    CashierAddCfgImportADO pData = (CashierAddCfgImportADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "REQUEST_ROOM_NAME")
                    {
                        try
                        {
                            var REQUEST =roomlist.Where(o => o.ROOM_CODE.ToLower() == pData.REQUEST_ROOM_CODE_STR.ToLower()).ToList();
                            if (REQUEST != null && REQUEST.Count > 0)
                            {
                                e.Value = REQUEST.FirstOrDefault().ROOM_NAME;
                            }
                            else 
                            {
                                e.Value = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ten phong yeu cau REQUEST_ROOM_NAME_STR", ex);
                        }
                    }

                    else if (e.Column.FieldName == "EXECUTE_ROOM_NAME")
                    {
                        try
                        {
                             var EXECUTE = roomlist.Where(o => o.ROOM_CODE.ToLower() == pData.EXECUTE_ROOM_CODE_STR.ToLower()).ToList();
                             if (EXECUTE != null && EXECUTE.Count > 0)
                             {
                                 e.Value = EXECUTE.FirstOrDefault().ROOM_NAME;
                             }
                             else
                             {
                                 e.Value = null;
                             }

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ten phong xu ly EXECUTE_ROOM_NAME_STR", ex);
                        }
                    }

                    else if (e.Column.FieldName == "CASHIER_ROOM_NAME")
                    {
                        try
                        {
                            var CASHIER = CashierRoomList.Where(o => o.CASHIER_ROOM_CODE.ToLower() == pData.CASHIER_ROOM_CODE_STR.ToLower()).ToList();
                            if (CASHIER != null && CASHIER.Count > 0)
                            {
                                e.Value = CASHIER.FirstOrDefault().CASHIER_ROOM_NAME;
                            }
                            else
                            {
                                e.Value = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ten phong thu ngan CASHIER_ROOM_NAME_STR", ex);
                        }
                    }

                    else if (e.Column.FieldName == "DAY_FROM_STR")
                    {
                        try
                        {
                            if (Equals(pData.INSTR_DAY_FROM_STR, "1") || Equals(pData.INSTR_DAY_FROM_STR, "2") || Equals(pData.INSTR_DAY_FROM_STR, "3") || Equals(pData.INSTR_DAY_FROM_STR, "4") || Equals(pData.INSTR_DAY_FROM_STR, "5") || Equals(pData.INSTR_DAY_FROM_STR, "6") || Equals(pData.INSTR_DAY_FROM_STR, "7"))
                            {
                                e.Value = convertDay(pData.INSTR_DAY_FROM_STR);
                                
                            }
                            else 
                            {
                                e.Value = pData.INSTR_DAY_FROM_STR;
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay chi dinh tu INSTR_DAY_FROM_STR", ex);
                        }
                    }

                    else if (e.Column.FieldName == "DAY_TO_STR")
                    {
                        try
                        {
                            if (Equals(pData.INSTR_DAY_TO_STR, "1") || Equals(pData.INSTR_DAY_TO_STR, "2") || Equals(pData.INSTR_DAY_TO_STR, "3") || Equals(pData.INSTR_DAY_TO_STR, "4") || Equals(pData.INSTR_DAY_TO_STR, "5") || Equals(pData.INSTR_DAY_TO_STR, "6") || Equals(pData.INSTR_DAY_TO_STR, "7"))
                            {
                                e.Value = convertDay(pData.INSTR_DAY_TO_STR);
                            }
                            else 
                            {
                                e.Value = pData.INSTR_DAY_TO_STR;
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay chi dinh den INSTR_DAY_TO_STR", ex);
                        }
                    }

                    else if (e.Column.FieldName == "IS_NOT_PRIORITY_STR1")
                    {
                        try
                        {
                            if (pData.IS_NOT_PRIORITY_STR == "X" || pData.IS_NOT_PRIORITY_STR == "x")
                            {
                                e.Value = true;
                            }
                            else 
                            {
                                e.Value = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot không ưu tiên IS_NOT_PRIORITY_STR1", ex);
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ImportHisCashierAddCfg.Resources.Lang", typeof(HIS.Desktop.Plugins.ImportHisCashierAddCfg.FormLoad.frmCashierAddCfg).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmCashierAddCfg.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDownload.Text = Inventec.Common.Resource.Get.Value("frmCashierAddCfg.btnDownload.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnImport.Text = Inventec.Common.Resource.Get.Value("frmCashierAddCfg.btnImport.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnShowLineError.Text = Inventec.Common.Resource.Get.Value("frmCashierAddCfg.btnShowLineError.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmCashierAddCfg.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmCashierAddCfg.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmCashierAddCfg.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmCashierAddCfg.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmCashierAddCfg.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmCashierAddCfg.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmCashierAddCfg.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmCashierAddCfg.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmCashierAddCfg.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmCashierAddCfg.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmCashierAddCfg.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmCashierAddCfg.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmCashierAddCfg.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmCashierAddCfg.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmCashierAddCfg.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("frmCashierAddCfg.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("frmCashierAddCfg.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmCashierAddCfg.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmCashierAddCfg_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                SetCaptionByLanguageKey();
                btnSave.Enabled = false;
                btnShowLineError.Enabled = false;
                DicService = BackendDataWorker.Get<HIS_CASHIER_ADD_CONFIG>().ToDictionary(o => o.ID.ToString(), o => o);
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
                + "/Tmp/Imp", "IMPORT_CASHIER_ADD_CONFIG.xlsx");

                if (File.Exists(source))
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                    saveFileDialog1.Title = "Save File";
                    saveFileDialog1.FileName = "IMPORT_CASHIER_ADD_CONFIG";
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
                        var hisServiceImport = import.GetWithCheck<CashierAddCfgImportADO>(0);
                        if (hisServiceImport != null && hisServiceImport.Count > 0)
                        {
                            List<CashierAddCfgImportADO> listAfterRemove = new List<CashierAddCfgImportADO>();
                            foreach (var item in hisServiceImport)
                            {
                                listAfterRemove.Add(item);
                            }

                            foreach (var item in hisServiceImport)
                            {
                                bool checkNull = string.IsNullOrEmpty(item.REQUEST_ROOM_CODE_STR)
                                    && string.IsNullOrEmpty(item.EXECUTE_ROOM_CODE_STR)
                                    && string.IsNullOrEmpty(item.CASHIER_ROOM_CODE_STR)
                                    && string.IsNullOrEmpty(item.INSTR_DAY_FROM_STR)
                                    && string.IsNullOrEmpty(item.INSTR_DAY_TO_STR)
                                    && string.IsNullOrEmpty(item.INSTR_TIME_FROM_STR)
                                    && string.IsNullOrEmpty(item.INSTR_TIME_TO_STR)
                                    && string.IsNullOrEmpty(item.IS_NOT_PRIORITY_STR);

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
                                serviceAdos = new List<CashierAddCfgImportADO>();
                                addServiceToProcessList(currentAdos, ref serviceAdos);
                                SetDataSource(serviceAdos);
                            }
                            //btnSave.Enabled = true;
                        }
                        else
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Không có dữ liệu từ file import");
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

        private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
               DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
               if (e.RowHandle >= 0)
               {
                   CashierAddCfgImportADO data = (CashierAddCfgImportADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                   
                       if (!string.IsNullOrEmpty(data.ERROR))
                       {
                           e.Appearance.ForeColor = Color.Red;
                       }
               }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);                
            }
            
        }


    }
}
