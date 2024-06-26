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
using HIS.Desktop.Plugins.CallPatientTypeAlter.Resources;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CallPatientTypeAlter
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
                SetCaptionByLanguageKey();
                LoadInfo();
                LoadDataGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmCheckHeinCardGOV
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.CallPatientTypeAlter.Resources.Lang", typeof(frmCheckHeinCardGOV).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmCheckHeinCardGOV.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmCheckHeinCardGOV.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmCheckHeinCardGOV.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmCheckHeinCardGOV.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmCheckHeinCardGOV.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmCheckHeinCardGOV.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmCheckHeinCardGOV.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmCheckHeinCardGOV.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmCheckHeinCardGOV.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmCheckHeinCardGOV.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmCheckHeinCardGOV.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmCheckHeinCardGOV.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("frmCheckHeinCardGOV.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("frmCheckHeinCardGOV.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem18.Text = Inventec.Common.Resource.Get.Value("frmCheckHeinCardGOV.layoutControlItem18.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem19.Text = Inventec.Common.Resource.Get.Value("frmCheckHeinCardGOV.layoutControlItem19.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("frmCheckHeinCardGOV.layoutControlItem20.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("frmCheckHeinCardGOV.layoutControlItem21.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciKetQuaKiemTra.Text = Inventec.Common.Resource.Get.Value("frmCheckHeinCardGOV.lciKetQuaKiemTra.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmCheckHeinCardGOV.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmCheckHeinCardGOV.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                if (!String.IsNullOrEmpty(resultHistoryLDO.maTheMoi) && !resultHistoryLDO.maThe.Equals(resultHistoryLDO.maTheMoi))
                {
                    lblMaTheMoi.Text = resultHistoryLDO.maTheMoi;
                }
                else
                {
                    layoutControlItem14.Text = "Mã thẻ: ";
                    lblMaTheMoi.Text = resultHistoryLDO.maThe;
                }
                lblHoTen.Text = resultHistoryLDO.hoTen;
                lblMaDKBD.Text = resultHistoryLDO.maDKBD;
                lblGioiTinh.Text = resultHistoryLDO.gioiTinh;
                lblNgayDu5Nam.Text = resultHistoryLDO.ngayDu5Nam;
                lblKetQuaKiemTra.Text = "Thẻ hợp lệ.";
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
                var data = new List<ExamHistoryLDO>();
                gridControlHistory.BeginUpdate();
                resultHistoryLDO.dsLichSuKCB2018 = resultHistoryLDO.dsLichSuKCB2018.OrderByDescending(o => o.ngayRa).ToList();
                if (resultHistoryLDO.dsLichSuKCB2018.Count > 5)
                {
                    for (int i = 0; i < 5; i++)
                        data.Add(resultHistoryLDO.dsLichSuKCB2018[i]);
                    gridControlHistory.DataSource = data;
                }
                else
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
                                e.Value = ResourceMessage.RaVien;
                            else if (data.tinhTrang == "2")
                                e.Value = ResourceMessage.ChuyenVien;
                            else if (data.tinhTrang == "3")
                                e.Value = ResourceMessage.TrongVien;
                            else if (data.tinhTrang == "4")
                                e.Value = ResourceMessage.XinRaVien;
                        }
                        else if (e.Column.FieldName == "kqDieuTri")
                        {
                            if (data.kqDieuTri == "1")
                                e.Value = ResourceMessage.Khoi;
                            else if (data.kqDieuTri == "2")
                                e.Value = ResourceMessage.Do;
                            else if (data.kqDieuTri == "3")
                                e.Value = ResourceMessage.KhongThayDoi;
                            else if (data.kqDieuTri == "4")
                                e.Value = ResourceMessage.NangHon;
                            else if (data.kqDieuTri == "5")
                                e.Value = ResourceMessage.TuVong;
                        }
                        else if (e.Column.FieldName == "ngayVao_str" && !String.IsNullOrEmpty(data.ngayVao))
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.ngayVao);
                        }
                        else if (e.Column.FieldName == "ngayRa_str" && !String.IsNullOrEmpty(data.ngayRa))
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.ngayRa);
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
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                resultHistoryLDO = null;
                this.gridViewHistory.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewHistory_CustomUnboundColumnData);
                this.Load -= new System.EventHandler(this.CheckHeinCardGOV_Load);
                gridViewHistory.GridControl.DataSource = null;
                gridControlHistory.DataSource = null;
                layoutControlItem2 = null;
                lciKetQuaKiemTra = null;
                layoutControlItem21 = null;
                layoutControlItem20 = null;
                layoutControlItem19 = null;
                layoutControlItem18 = null;
                layoutControlItem17 = null;
                layoutControlItem16 = null;
                layoutControlItem15 = null;
                layoutControlItem14 = null;
                layoutControlItem13 = null;
                lblHoTen = null;
                lblMaTheMoi = null;
                lblDiaChi = null;
                lblGioiTinh = null;
                lblCoQuanBHXH = null;
                lblMaDKBD = null;
                lblGiaTriTheTu = null;
                lblGiaTriTheDen = null;
                lblNgayDu5Nam = null;
                lblKetQuaKiemTra = null;
                labelControl1 = null;
                gridColumn8 = null;
                gridColumn7 = null;
                gridColumn6 = null;
                gridColumn5 = null;
                gridColumn4 = null;
                gridColumn3 = null;
                gridColumn2 = null;
                gridColumn1 = null;
                layoutControlItem1 = null;
                gridViewHistory = null;
                gridControlHistory = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
