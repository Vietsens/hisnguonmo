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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.HisConfig;
using Newtonsoft.Json;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using DevExpress.Data;
using HIS.Desktop.LocalStorage.ConfigApplication;

namespace HIS.Desktop.Plugins.CustomerRequest
{
    public partial class UCCustomerRequest : UserControlBase
    {

        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        const string CONFIG_KEY__VPLUS_CUSTOMER_INFO = "HIS.Desktop.VPLUS_CUSTOMER_INFO";
        string stt_sản_phẩm = "1";
        string stt_phần_mềm = "2";
        string customerCode = "";
        string customerName = "";
        List<BINH_LUAN> lstDataBinhLuan = new List<BINH_LUAN>();
        YCKH currentYCKH = new YCKH();
        Inventec.Desktop.Common.Modules.Module currentModule;
        public UCCustomerRequest(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCCustomerRequest_Load(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
                FillDataFormList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                txtTieuDe.Text = "";
                txtNoiDung.Text = "";
                lblSTT.Text = "";
                lblLoaiYCKH.Text = "";
                lblNguoiTiepNhan.Text = "";
                lblThoiGianHoanThanh.Text = "";
                lblThoiGianTao.Text = "";
                lblThoiHanHoanThanh.Text = "";
                lblTrangThai.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataFormList()
        {
            try
            {
                WaitingManager.Show();

                int numPageSize = 0;
                if (ucPaging.pagingGrid != null)
                {
                    numPageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPaging, param, numPageSize);
                WaitingManager.Hide();


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;

                string customerInfo = HisConfigs.Get<string>(CONFIG_KEY__VPLUS_CUSTOMER_INFO);
                if (!String.IsNullOrEmpty(customerInfo))
                {
                    var cusInfoArr = customerInfo.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    if (cusInfoArr != null && cusInfoArr.Length > 2)
                    {
                        customerCode = cusInfoArr[0];
                        customerName = cusInfoArr[1];
                        stt_sản_phẩm = cusInfoArr[2];
                        stt_phần_mềm = cusInfoArr[3];
                    }
                }
                string LastUser = string.Format("{0}:{1}:{2}", customerCode, stt_sản_phẩm, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());

                var dataYCKH = HIS.Desktop.ApiConsumer.ApiConsumers.CrmConsumer.Get<object>("ords/vietsens/yckh/yckh/", null, null, 0, "người_dùng_cuối", LastUser, "limit", limit, "offset", startPage);

                Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData("dataYCKH_______", dataYCKH));

                DataYCKH ado = Newtonsoft.Json.JsonConvert.DeserializeObject<DataYCKH>(dataYCKH.ToString());

                if (ado.items != null && ado.items.Count() > 0)
                {
                    foreach (var item in ado.items)
                    {
                        YCKH yckh = new YCKH(item);
                        item.nội_dung = yckh.nội_dung;
                        item.url = yckh.url;
                        item.thời_gian_tạo = yckh.thời_gian_tạo;
                    }


                    grdCustomerRequestList.DataSource = ado.items;
                    rowCount = (ado == null ? 0 : ado.count);
                    dataTotal = (ado.items == null || ado.items.Count() == 0 ? 0 : ado.items.First().số_bản_ghi);

                }

                Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData("ado.items_______", ado.items));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grvCustomerRequestList_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (YCKH)grvCustomerRequestList.GetFocusedRow();
                if (row != null)
                {
                    this.currentYCKH = row;
                    FilldataToGridComment(GetAllCommentByYCKH(row.id));

                    FillDataToEditor(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FilldataToGridComment(DataBinhLuan ado)
        {
            try
            {
                grdComment.DataSource = null;
                grdComment.DataSource = ado.items;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private DataBinhLuan GetAllCommentByYCKH(string idYCKH)
        {
            DataBinhLuan ado = new DataBinhLuan();
            try
            {
                string uri = "ords/vietsens/binhluan/yckh/" + idYCKH;
                var dataComment = HIS.Desktop.ApiConsumer.ApiConsumers.CrmConsumer.Get<object>(uri, null, null, 0, null);
                Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData("dataComment_______", dataComment));
                ado = Newtonsoft.Json.JsonConvert.DeserializeObject<DataBinhLuan>(dataComment.ToString());
                if (ado.items != null && ado.items.Count() > 0)
                    lstDataBinhLuan = ado.items;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return ado;
        }

        private void FillDataToEditor(YCKH row)
        {
            try
            {
                lblSTT.Text = row.stt;
                txtTieuDe.Text = row.tiêu_đề;
                txtNoiDung.Text = row.nội_dung;
                lblNguoiTiepNhan.Text = row.người_tiếp_nhận;
                lblThoiGianTao.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(row.thời_gian_tạo) ?? 0);
                lblThoiHanHoanThanh.Text = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(row.thời_hạn_hoàn_thành) != null ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(row.thời_hạn_hoàn_thành) ?? 0) : "";
                lblThoiGianHoanThanh.Text = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(row.thời_hạn_hoàn_thành) != null ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(row.thời_gian_hoàn_thành) ?? 0) : "";
                lblTrangThai.Text = row.trạng_thái_yckh;
                lblLoaiYCKH.Text = row.loại_yckh;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grvCustomerRequestList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                YCKH data = null;
                if (e.RowHandle > -1)
                    data = (YCKH)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (e.RowHandle >= 0)
                    if (e.Column.FieldName == "DELETE")
                        e.RepositoryItem = (data.trạng_thái_yckh == "Mở" && string.IsNullOrEmpty(data.người_tiếp_nhận)) ? repositoryItemButtonDelete : repositoryItemButtonDeleteDis;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    CommonParam param = new CommonParam();
                    var rowData = (YCKH)grvCustomerRequestList.GetFocusedRow();
                    bool success = false;
                    if (rowData != null)
                    {
                        string uri = "ords/vietsens/yckh/yckh/" + rowData.id;
                        var apiResult = HIS.Desktop.ApiConsumer.ApiConsumers.CrmConsumer.DeleteWithouApiParam<string>(uri, 0);
                        if (string.IsNullOrEmpty(apiResult))
                        {
                            success = true;
                            FillDataFormList();
                            SetDefaultValue();
                        }
                    }
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool checkYCKH(YCKH yckh)
        {
            bool result = true;
            try
            {
                if (yckh.trạng_thái_yckh != "Mở" || !string.IsNullOrEmpty(yckh.người_tiếp_nhận))
                    result = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void btnLuuThongTinYCKH_Click(object sender, EventArgs e)
        {
            try
            {
                if (!checkYCKH(currentYCKH))
                {
                    MessageBox.Show("Xử lý thất bại. Chỉ cho phép chỉnh sửa trong trường hợp yêu cầu ở trạng thái mở và chưa có thông tin người tiếp nhận", "Thông báo");
                    return;
                }

                CommonParam param = new CommonParam();
                bool success = false;

                THONG_TIN_YCKH thongtin = new THONG_TIN_YCKH();
                thongtin.tiêu_đề = txtTieuDe.Text;
                thongtin.nội_dung = txtNoiDung.Text + currentYCKH.url;

                string uri = "ords/vietsens/yckh/yckh/" + currentYCKH.id;


                var apiResult = HIS.Desktop.ApiConsumer.ApiConsumers.CrmConsumer.PutWithouApiParam<object>(uri, thongtin, 0, param);

                if (apiResult == null)
                {
                    success = true;
                    FillDataFormList();
                    SetDefaultValue();
                }
                else
                {
                    param.Messages.Add("Tạo yêu cầu khách hàng thất bại");
                }
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEdit_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (BINH_LUAN)grvComment.GetFocusedRow();
                if (currentYCKH != null && !string.IsNullOrEmpty(currentYCKH.id))
                {
                    frmThemBinhLuan frm = new frmThemBinhLuan((HIS.Desktop.Common.DelegateReturnSuccess)Fill, null, rowData, false);
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonDeleteComment_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    CommonParam param = new CommonParam();
                    var rowData = (BINH_LUAN)grvComment.GetFocusedRow();

                    string uri = "ords/vietsens/binhluan/binhluan/" + rowData.id;
                    var apiDeleteComment = HIS.Desktop.ApiConsumer.ApiConsumers.CrmConsumer.DeleteWithouApiParam<string>(uri, 0);
                    if (string.IsNullOrEmpty(apiDeleteComment))
                    {
                        FilldataToGridComment(GetAllCommentByYCKH(currentYCKH.id));
                        MessageManager.Show(this.ParentForm, param, true);
                    }
                    else
                    {
                        MessageBox.Show(apiDeleteComment, "Thông báo");
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAddComment_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentYCKH != null && !string.IsNullOrEmpty(currentYCKH.id))
                {
                    frmThemBinhLuan frm = new frmThemBinhLuan((HIS.Desktop.Common.DelegateReturnSuccess)Fill, currentYCKH, null, true);
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Fill(bool success)
        {
            Inventec.Common.Logging.LogSystem.Info("success" + success);
            if (success)
                FilldataToGridComment(GetAllCommentByYCKH(currentYCKH.id));
        }

        private void grvComment_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                BINH_LUAN data = null;
                if (e.RowHandle > -1)
                    data = (BINH_LUAN)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "DELETE")
                        e.RepositoryItem = (data.tác_giả == string.Format("{0}:{1}:{2}", customerCode, stt_sản_phẩm, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName())) ? repositoryItemButtonDeleteComment : repositoryItemButtonDeleteCommentDis;
                    else if (e.Column.FieldName == "EDIT")
                        e.RepositoryItem = (data.tác_giả == string.Format("{0}:{1}:{2}", customerCode, stt_sản_phẩm, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName())) ? repositoryItemButtonEdit : repositoryItemButtonEditDis;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grvCustomerRequestList_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    YCKH dataRow = (YCKH)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "THOI_GIAN_TAO")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dataRow.thời_gian_tạo) ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                            }
                        }
                        else if (e.Column.FieldName == "THOI_HAN_HOAN_THANH")
                        {
                            try
                            {
                                var dt = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dataRow.thời_hạn_hoàn_thành);
                                e.Value = dt != null ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dt ?? 0) : "";
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao MODIFY_TIME", ex);
                            }
                        }
                        else if (e.Column.FieldName == "THOI_GIAN_HOAN_THANH")
                        {
                            try
                            {
                                var dt = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dataRow.thời_gian_hoàn_thành);
                                e.Value = dt != null ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dt ?? 0) : "";
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grvComment_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {

                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    BINH_LUAN dataRow = (BINH_LUAN)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "THOI_GIAN_SUA")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dataRow.thời_gian_sửa) ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                            }
                        }
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
