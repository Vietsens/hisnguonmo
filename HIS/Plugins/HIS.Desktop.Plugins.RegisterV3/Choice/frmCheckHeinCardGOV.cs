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
using DevExpress.XtraGrid.Views.Base;
using His.Bhyt.InsuranceExpertise.LDO;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
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

namespace HIS.Desktop.Plugins.RegisterV3
{
    public partial class frmCheckHeinCardGOV : HIS.Desktop.Utility.FormBase
    {
        ResultHistoryLDO resultHistoryLDO = null;

        public frmCheckHeinCardGOV()
        {
            InitializeComponent();
        }

        public frmCheckHeinCardGOV(ResultHistoryLDO resultHistoryLDO)
        {
            InitializeComponent();
            try
            {
                this.resultHistoryLDO = resultHistoryLDO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void CheckHeinCardGOV_Load(object sender, EventArgs e)
        {
            try
            {
                LoadInfo();
                LoadDataGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadInfo()
        {
            try
            {
                lblCoQuanBHXH.Text = resultHistoryLDO.cqBHXH;
                lblDiaChi.Text = resultHistoryLDO.diaChi;
                lblGiaTriTheDen.Text = resultHistoryLDO.gtTheDen;
                lblGiaTriTheTu.Text = resultHistoryLDO.gtTheTu;
                lblGioiTinh.Text = resultHistoryLDO.gioiTinh;
                lblHoTen.Text = resultHistoryLDO.hoTen;
                lblMaDKBD.Text = resultHistoryLDO.maDKBD;
                lblMaKhuVuc.Text = resultHistoryLDO.maKV;
                lblNgayDu5Nam.Text = resultHistoryLDO.ngayDu5Nam;
                lblMaKetQua.Text = resultHistoryLDO.maKetQua;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadDataGridControl()
        {
            try
            {
                gridControlHistory.BeginUpdate();
                gridControlHistory.DataSource = resultHistoryLDO.dsLichSuKCB2018;
                gridControlHistory.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewHistory_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ExamHistoryLDO data = (ExamHistoryLDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "tinhTrang_str")
                        {
                            if (data.tinhTrang == "1")
                                e.Value = "Ra viện";
                            else if (data.tinhTrang == "2")
                                e.Value = "Chuyển viện";
                            else if (data.tinhTrang == "3")
                                e.Value = "Trốn viện";
                            else if (data.tinhTrang == "4")
                                e.Value = "Xin ra viện";
                        }
                        else if (e.Column.FieldName == "kqDieuTri_str")
                        {
                            if (data.kqDieuTri == "1")
                                e.Value = "Khỏi";
                            else if (data.kqDieuTri == "2")
                                e.Value = "Đỡ";
                            else if (data.kqDieuTri == "3")
                                e.Value = "Không thay đổi";
                            else if (data.kqDieuTri == "4")
                                e.Value = "Nặng hơn";
                            else if (data.kqDieuTri == "5")
                                e.Value = "Tử vong";
                        }
                        else if (e.Column.FieldName == "cskcbbd_name")
                        {
                            var medi = BackendDataWorker.Get<HIS_MEDI_ORG>().FirstOrDefault(o => o.MEDI_ORG_CODE == data.maCSKCB);
                            e.Value = medi != null ? medi.MEDI_ORG_NAME : "";
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
