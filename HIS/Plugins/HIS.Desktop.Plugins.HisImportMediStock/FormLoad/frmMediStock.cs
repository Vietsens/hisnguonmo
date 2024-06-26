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
using HIS.Desktop.Plugins.HisImportMediStock.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
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

namespace HIS.Desktop.Plugins.HisImportMediStock.FormLoad
{
    public partial class frmMediStock : HIS.Desktop.Utility.FormBase
    {
        List<MediStockImportADO> mediStockAdos;
        List<MediStockImportADO> currentAdos;
        List<V_HIS_MEDI_STOCK> listMediStock;
        List<HIS_DEPARTMENT> listDepartment;
        List<HIS_EXP_MEST_TYPE> listExpMestType;
        List<HIS_IMP_MEST_TYPE> listImpMestType;

        RefeshReference delegateRefresh;
        string fileName;
        bool checkClick;
        Inventec.Desktop.Common.Modules.Module currentModule;

        public frmMediStock(Inventec.Desktop.Common.Modules.Module module, RefeshReference _delegate)
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

        public frmMediStock(Inventec.Desktop.Common.Modules.Module module)
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

        private void CheckErrorLine(List<MediStockImportADO> dataSource)
        {
            try
            {
                if (mediStockAdos != null && mediStockAdos.Count > 0)
                {
                    var checkError = mediStockAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
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

        private void SetDataSource(List<MediStockImportADO> dataSource)
        {
            try
            {
                gridControlMediStock.BeginUpdate();
                gridControlMediStock.DataSource = null;
                gridControlMediStock.DataSource = dataSource;
                gridControlMediStock.EndUpdate();
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void addMediStockToProcessList(List<MediStockImportADO> _mediStock, ref List<MediStockImportADO> _mediStockRef)
        {
            try
            {
                _mediStockRef = new List<MediStockImportADO>();
                long i = 0;
                foreach (var item in _mediStock)
                {
                    i++;
                    string error = "";
                    var mateAdo = new MediStockImportADO();
                    mateAdo.dicMediStockExty = new Dictionary<long, HIS_MEDI_STOCK_EXTY>();
                    mateAdo.dicMediStockImty = new Dictionary<long, HIS_MEDI_STOCK_IMTY>();

                    Inventec.Common.Mapper.DataObjectMapper.Map<MediStockImportADO>(mateAdo, item);

                    if (!string.IsNullOrEmpty(item.DEPARTMENT_CODE))
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.DEPARTMENT_CODE, 20))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã khoa");
                        }
                        if (listDepartment != null && listDepartment.Count > 0)
                        {
                            var departmentGet = listDepartment.FirstOrDefault(o => o.DEPARTMENT_CODE == item.DEPARTMENT_CODE);
                            if (departmentGet != null)
                            {
                                mateAdo.DEPARTMENT_ID = departmentGet.ID;
                                mateAdo.DEPARTMENT_NAME = departmentGet.DEPARTMENT_NAME;
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

                    if (!string.IsNullOrEmpty(item.MEDI_STOCK_CODE))
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.MEDI_STOCK_CODE, 20))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã kho");
                        }
                        if (listMediStock != null && listMediStock.Count > 0)
                        {
                            var check = listMediStock.Exists(o => o.MEDI_STOCK_CODE == item.MEDI_STOCK_CODE);
                            if (check)
                            {
                                error += string.Format(Message.MessageImport.DaTonTai, "Mã kho");
                            }
                        }
                        var checkExel = _mediStockRef.FirstOrDefault(o => o.MEDI_STOCK_CODE == item.MEDI_STOCK_CODE);
                        if (checkExel != null)
                        {
                            error += string.Format(Message.MessageImport.TonTaiTrungNhauTrongFileImport, "Mã kho");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã kho");
                    }

                    if (!string.IsNullOrEmpty(item.MEDI_STOCK_NAME))
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.MEDI_STOCK_NAME, 100))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Tên kho");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Tên kho");
                    }

                    if (!string.IsNullOrEmpty(item.PARENT_CODE))
                    {
                        var getData = listMediStock.FirstOrDefault(o => o.MEDI_STOCK_CODE == item.PARENT_CODE);
                        if (getData != null)
                        {
                            mateAdo.PARENT_ID = getData.ID;
                            mateAdo.PARENT_NAME = getData.MEDI_STOCK_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã cha");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.BHYT_HEAD_CODE))
                    {
                        if (item.BHYT_HEAD_CODE.Length > 200)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Đầu mã thẻ BHYT");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.NOT_IN_BHYT_HEAD_CODE))
                    {
                        if (item.NOT_IN_BHYT_HEAD_CODE.Length > 200)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Đầu mã thẻ BHYT không cho phép");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.AUTO_APPROVE_EXPORT))
                    {
                        var listIds = item.AUTO_APPROVE_EXPORT.Split(',').ToList();
                        if (listIds != null && listIds.Count > 0)
                        {
                            foreach (var id in listIds)
                            {
                                var exp = listExpMestType.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(id));
                                if (exp != null)
                                {
                                    if (mateAdo.dicMediStockExty.ContainsKey(exp.ID))
                                    {
                                        mateAdo.dicMediStockExty[exp.ID].IS_AUTO_APPROVE = 1;
                                    }

                                    if (!mateAdo.dicMediStockExty.ContainsKey(exp.ID))
                                    {
                                        HIS_MEDI_STOCK_EXTY MediStockExty = new HIS_MEDI_STOCK_EXTY();
                                        MediStockExty.EXP_MEST_TYPE_ID = exp.ID;
                                        MediStockExty.IS_AUTO_APPROVE = 1;
                                        mateAdo.dicMediStockExty[exp.ID] = MediStockExty;
                                    }
                                }
                                else
                                {
                                    error += "Loại xuất tự động duyệt: " + id + "không đúng|";
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(item.AUTO_EXECUTE_EXPORT))
                    {
                        var listIds = item.AUTO_EXECUTE_EXPORT.Split(',').ToList();
                        if (listIds != null && listIds.Count > 0)
                        {
                            foreach (var id in listIds)
                            {
                                var exp = listExpMestType.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(id));
                                if (exp != null)
                                {
                                    if (mateAdo.dicMediStockExty.ContainsKey(exp.ID))
                                    {
                                        mateAdo.dicMediStockExty[exp.ID].IS_AUTO_EXECUTE = 1;
                                    }

                                    if (!mateAdo.dicMediStockExty.ContainsKey(exp.ID))
                                    {
                                        HIS_MEDI_STOCK_EXTY MediStockExty = new HIS_MEDI_STOCK_EXTY();
                                        MediStockExty.EXP_MEST_TYPE_ID = exp.ID;
                                        MediStockExty.IS_AUTO_EXECUTE = 1;
                                        mateAdo.dicMediStockExty[exp.ID] = MediStockExty;
                                    }
                                }
                                else
                                {
                                    error += "Loại xuất tự động xuất: " + id + "không đúng|";
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(item.AUTO_APPROVE_IMPORT))
                    {
                        var listIds = item.AUTO_APPROVE_IMPORT.Split(',').ToList();
                        if (listIds != null && listIds.Count > 0)
                        {
                            foreach (var id in listIds)
                            {
                                var imp = listImpMestType.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(id));
                                if (imp != null)
                                {
                                    if (mateAdo.dicMediStockImty.ContainsKey(imp.ID))
                                    {
                                        mateAdo.dicMediStockImty[imp.ID].IS_AUTO_APPROVE = 1;
                                    }

                                    if (!mateAdo.dicMediStockImty.ContainsKey(imp.ID))
                                    {
                                        HIS_MEDI_STOCK_IMTY MediStockImty = new HIS_MEDI_STOCK_IMTY();
                                        MediStockImty.IMP_MEST_TYPE_ID = imp.ID;
                                        MediStockImty.IS_AUTO_APPROVE = 1;
                                        mateAdo.dicMediStockImty[imp.ID] = MediStockImty;
                                    }
                                }
                                else
                                {
                                    error += "Loại nhập tự động duyệt: " + id + "không đúng|";
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(item.AUTO_EXECUTE_IMPORT))
                    {
                        var listIds = item.AUTO_EXECUTE_IMPORT.Split(',').ToList();
                        if (listIds != null && listIds.Count > 0)
                        {
                            foreach (var id in listIds)
                            {
                                var imp = listImpMestType.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(id));
                                if (imp != null)
                                {
                                    if (mateAdo.dicMediStockImty.ContainsKey(imp.ID))
                                    {
                                        mateAdo.dicMediStockImty[imp.ID].IS_AUTO_EXECUTE = 1;
                                    }

                                    if (!mateAdo.dicMediStockImty.ContainsKey(imp.ID))
                                    {
                                        HIS_MEDI_STOCK_IMTY MediStockImty = new HIS_MEDI_STOCK_IMTY();
                                        MediStockImty.IMP_MEST_TYPE_ID = imp.ID;
                                        MediStockImty.IS_AUTO_EXECUTE = 1;
                                        mateAdo.dicMediStockImty[imp.ID] = MediStockImty;
                                    }
                                }
                                else
                                {
                                    error += "Loại nhập tự động nhập: " + id + "không đúng|";
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(item.IS_ALLOW_IMP_SUPPLIER_STR))
                    {
                        if (item.IS_ALLOW_IMP_SUPPLIER_STR == "x")
                        {
                            mateAdo.IS_ALLOW_IMP_SUPPLIER = 1;
                            mateAdo.ALLOW_IMP_SUPPLIER = true;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Cho phép nhập từ NCC");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.IS_AUTO_CREATE_CHMS_IMP_STR))
                    {
                        if (item.IS_AUTO_CREATE_CHMS_IMP_STR == "x")
                        {
                            mateAdo.IS_AUTO_CREATE_CHMS_IMP = 1;
                            mateAdo.AUTO_CREATE_CHMS_IMP = true;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Tự động tạo yêu cầu nhập chuyển kho");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.IS_BLOOD_STR))
                    {
                        if (item.IS_BLOOD_STR == "x")
                        {
                            mateAdo.IS_BLOOD = 1;
                            mateAdo.BLOOD = true;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Là kho máu");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.IS_NEW_MEDICINE_STR))
                    {
                        if (item.IS_NEW_MEDICINE_STR == "x")
                        {
                            mateAdo.IS_NEW_MEDICINE = 1;
                            mateAdo.NEW_MEDICINE = true;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Tân dược");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.IS_TRADITIONAL_MEDICINE_STR))
                    {
                        if (item.IS_TRADITIONAL_MEDICINE_STR == "x")
                        {
                            mateAdo.IS_TRADITIONAL_MEDICINE = 1;
                            mateAdo.TRADITIONAL_MEDICINE = true;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "YHCT");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.IS_BUSINESS_STR))
                    {
                        if (item.IS_BUSINESS_STR == "x")
                        {
                            mateAdo.IS_BUSINESS = 1;
                            mateAdo.BUSINESS = true;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Kho kinh doanh");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.IS_CABINET_STR))
                    {
                        if (item.IS_CABINET_STR == "x")
                        {
                            mateAdo.IS_CABINET = 1;
                            mateAdo.CABINET = true;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Là tủ trực");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.IS_GOODS_RESTRICT_STR))
                    {
                        if (item.IS_GOODS_RESTRICT_STR == "x")
                        {
                            mateAdo.IS_GOODS_RESTRICT = 1;
                            mateAdo.GOODS_RESTRICT = true;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Quản lý hạn chế các loại thuốc vật tư lưu trữ");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.IS_ODD_STR))
                    {
                        if (item.IS_ODD_STR == "x")
                        {
                            mateAdo.IS_ODD = 1;
                            mateAdo.ODD = true;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Kho thuốc lẻ");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.IS_SHOW_DDT_STR))
                    {
                        if (item.IS_SHOW_DDT_STR == "x")
                        {
                            mateAdo.IS_SHOW_DDT = 1;
                            mateAdo.SHOW_DDT = true;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Hiển thị đơn điều trị");
                        }
                    }

                    mateAdo.ERROR = error;
                    mateAdo.ID = i;

                    _mediStockRef.Add(mateAdo);
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

                List<HisMediStockSDO> listSdos = new List<HisMediStockSDO>();

                foreach (var item in mediStockAdos)
                {
                    HisMediStockSDO sdo = new HisMediStockSDO();

                    HIS_MEDI_STOCK mediStock = new HIS_MEDI_STOCK();

                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_MEDI_STOCK>(mediStock, item);
                    mediStock.ID = 0;

                    sdo.HisMediStockExtys = item.dicMediStockExty.Select(o => o.Value).ToList();
                    sdo.HisMediStockImtys = item.dicMediStockImty.Select(o => o.Value).ToList();

                    sdo.HisRoom = new HIS_ROOM();
                    sdo.HisRoom.DEPARTMENT_ID = item.DEPARTMENT_ID;
                    sdo.HisMediStock = mediStock;

                    listSdos.Add(sdo);
                }

                CommonParam param = new CommonParam();
                var rs = new BackendAdapter(param).Post<List<HisMediStockSDO>>("api/HisMediStock/CreateList", ApiConsumers.MosConsumer, listSdos, param);
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
                var row = (MediStockImportADO)gridViewMediStock.GetFocusedRow();
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
                var row = (MediStockImportADO)gridViewMediStock.GetFocusedRow();
                if (row != null)
                {
                    if (mediStockAdos != null && mediStockAdos.Count > 0)
                    {
                        mediStockAdos.Remove(row);
                        List<MediStockImportADO> newMediStockAdos = new List<MediStockImportADO>();
                        addMediStockToProcessList(mediStockAdos, ref newMediStockAdos);
                        mediStockAdos = new List<MediStockImportADO>();
                        mediStockAdos.AddRange(newMediStockAdos);
                        gridControlMediStock.DataSource = null;
                        gridControlMediStock.DataSource = mediStockAdos;
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
                    var errorLine = mediStockAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = mediStockAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewMediStock_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    MediStockImportADO data = (MediStockImportADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
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

        private void gridViewMediStock_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MediStockImportADO pData = (MediStockImportADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                    //            e.Value = Inventec.Common.Resource.Get.Value("frmHisMisuMediStockType.HoatDong", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    //        else
                    //            e.Value = Inventec.Common.Resource.Get.Value("frmHisMisuMediStockType.TamKhoa", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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

        private void GetData()
        {
            try
            {
                listMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.IS_ACTIVE == 1).ToList();
                listDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == 1).ToList();
                listExpMestType = BackendDataWorker.Get<HIS_EXP_MEST_TYPE>();
                listImpMestType = BackendDataWorker.Get<HIS_IMP_MEST_TYPE>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void frmMediStock_Load(object sender, EventArgs e)
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
                + "/Tmp/Imp", "IMPORT_MEDI_STOCK.xlsx");

                if (File.Exists(source))
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                    saveFileDialog1.Title = "Save File";
                    saveFileDialog1.FileName = "IMPORT_MEDI_STOCK";
                    saveFileDialog1.DefaultExt = "xlsx";
                    saveFileDialog1.Filter = "Excel files (*.xlsx)|All files (*.*)";
                    saveFileDialog1.FilterIndex = 2;
                    saveFileDialog1.RestoreDirectory = true;

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(source, saveFileDialog1.FileName, true);
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

                    if (!string.IsNullOrEmpty(ofd.FileName))
                    {
                        Refresh(ofd.FileName);
                    }

                    WaitingManager.Hide();

                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                //btnRefresh.Enabled = false;
            }
        }

        private void Refresh(string fileName)
        {
            try
            {
                var import = new Inventec.Common.ExcelImport.Import();
                if (import.ReadFileExcel(fileName))
                {
                    var hisMediStockImport = import.GetWithCheck<MediStockImportADO>(0);
                    if (hisMediStockImport != null && hisMediStockImport.Count > 0)
                    {
                        this.fileName = fileName;
                        //btnRefresh.Enabled = true;
                        List<MediStockImportADO> listAfterRemove = new List<MediStockImportADO>();
                        foreach (var item in hisMediStockImport)
                        {
                            listAfterRemove.Add(item);
                        }

                        foreach (var item in hisMediStockImport)
                        {
                            bool checkNull = string.IsNullOrEmpty(item.AUTO_APPROVE_EXPORT)
                            && string.IsNullOrEmpty(item.AUTO_APPROVE_IMPORT)
                            && string.IsNullOrEmpty(item.AUTO_EXECUTE_EXPORT)
                            && string.IsNullOrEmpty(item.AUTO_EXECUTE_IMPORT)
                            && string.IsNullOrEmpty(item.BHYT_HEAD_CODE)
                            && string.IsNullOrEmpty(item.DEPARTMENT_CODE)
                            && string.IsNullOrEmpty(item.IS_ALLOW_IMP_SUPPLIER_STR)
                            && string.IsNullOrEmpty(item.IS_AUTO_CREATE_CHMS_IMP_STR)
                            && string.IsNullOrEmpty(item.IS_BLOOD_STR)
                            && string.IsNullOrEmpty(item.IS_NEW_MEDICINE_STR)
                            && string.IsNullOrEmpty(item.IS_TRADITIONAL_MEDICINE_STR)
                            && string.IsNullOrEmpty(item.IS_BUSINESS_STR)
                            && string.IsNullOrEmpty(item.IS_CABINET_STR)
                            && string.IsNullOrEmpty(item.IS_GOODS_RESTRICT_STR)
                            && string.IsNullOrEmpty(item.IS_ODD_STR)
                            && string.IsNullOrEmpty(item.IS_SHOW_DDT_STR)
                            && string.IsNullOrEmpty(item.MEDI_STOCK_CODE)
                            && string.IsNullOrEmpty(item.NOT_IN_BHYT_HEAD_CODE)
                            && string.IsNullOrEmpty(item.PARENT_CODE)
                            && string.IsNullOrEmpty(item.MEDI_STOCK_NAME);

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
                            mediStockAdos = new List<MediStockImportADO>();
                            addMediStockToProcessList(currentAdos, ref mediStockAdos);
                            SetDataSource(mediStockAdos);
                        }

                        //btnSave.Enabled = true;
                    }
                    else
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Import thất bại");
                        //btnRefresh.Enabled = false;
                        fileName = "";
                    }
                }
                else
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không đọc được file");
                    //btnRefresh.Enabled = false;
                    fileName = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.fileName))
                    Refresh(this.fileName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}
