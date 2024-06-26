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
using His.Bhyt.InsuranceExpertise.XML;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Register.ADO;
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

namespace HIS.Desktop.Plugins.Register
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
                if (!String.IsNullOrEmpty(resultHistoryLDO.maDKBDMoi) && !resultHistoryLDO.maDKBD.Equals(resultHistoryLDO.maDKBDMoi))
                {
                    lblMaDKBD.Text = resultHistoryLDO.maDKBDMoi;
                }
                else
                {
                    lblMaDKBD.Text = resultHistoryLDO.maDKBD;
                }
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
                //gridControlHistory.BeginUpdate();
                //gridControlHistory.DataSource = resultHistoryLDO.dsLichSuKCB2018;
                //gridControlHistory.EndUpdate();
                if(resultHistoryLDO.dsLichSuKCB2018 == null || resultHistoryLDO.dsLichSuKCB2018.Count <= 0)
				{
                    return;
				}                    
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
                                e.Value = "Ra viện";
                            else if (data.tinhTrang == "2")
                                e.Value = "Chuyển viện";
                            else if (data.tinhTrang == "3")
                                e.Value = "Trốn viện";
                            else if (data.tinhTrang == "4")
                                e.Value = "Xin ra viện";
                        }
                        else if (e.Column.FieldName == "kqDieuTri")
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

        private void gridViewHistory_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var data = (ExamHistoryLDO)gridViewHistory.GetFocusedRow();
                if (data != null)
                {
                    CheckDetailClick(data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async void CheckDetailClick(ExamHistoryLDO _ExamHistoryLDO)
        {
            try
            {
                HIS.Desktop.Plugins.Library.CheckHeinGOV.HeinGOVManager heinGOVManager = new Library.CheckHeinGOV.HeinGOVManager(ResourceMessage.GoiSangCongBHXHTraVeMaLoi);

                List<TreeChiTietKCB> _TreeChiTietKCBs = new List<TreeChiTietKCB>();
                treeListKCB.DataSource = null;

                var dataNew = await heinGOVManager.CheckChiTietHS(_ExamHistoryLDO.maHoSo, "1");
                if (dataNew != null
                    && dataNew.ChiTietKCBLDO != null
                    && dataNew.ChiTietKCBLDO.hoSoKCB != null)
                {
                    var dataHeinSVTypes = BackendDataWorker.Get<HIS_HEIN_SERVICE_TYPE>();
                    //DVKyThuat,VatTu
                    if (dataNew.ChiTietKCBLDO.hoSoKCB.dsXml3 != null
                    && dataNew.ChiTietKCBLDO.hoSoKCB.dsXml3.Count > 0)
                    {
                        var listRootSety = dataNew.ChiTietKCBLDO.hoSoKCB.dsXml3.GroupBy(p => p.MaNhom);
                        foreach (var rootSety in listRootSety)
                        {
                            TreeChiTietKCB _SETY = new TreeChiTietKCB();
                            _SETY.CONCRETE_ID__IN_SETY = rootSety.First().MaNhom;
                            _SETY.TenDichVu = dataHeinSVTypes.FirstOrDefault(p => p.BHYT_CODE == _SETY.CONCRETE_ID__IN_SETY).HEIN_SERVICE_TYPE_NAME;
                            _TreeChiTietKCBs.Add(_SETY);

                            foreach (var item in rootSety)
                            {
                                TreeChiTietKCB leaf = new TreeChiTietKCB();
                                leaf.CONCRETE_ID__IN_SETY = _SETY.CONCRETE_ID__IN_SETY + "_" + item.Id;
                                leaf.PARENT_ID__IN_SETY = _SETY.CONCRETE_ID__IN_SETY;
                                leaf.MaDichVu = !string.IsNullOrEmpty(item.MaDichVu) ? item.MaDichVu : item.MaVatTu;
                                leaf.TenDichVu = item.TenDichVu;
                                leaf.DonViTinh = item.DonViTinh;
                                leaf.SoLuong = item.SoLuong;
                                leaf.DonGia = item.DonGia;
                                leaf.ThanhTien = item.ThanhTien;
                                _TreeChiTietKCBs.Add(leaf);
                            }
                        }
                    }

                    //Thuoc
                    if (dataNew.ChiTietKCBLDO.hoSoKCB.dsXml2 != null
                   && dataNew.ChiTietKCBLDO.hoSoKCB.dsXml2.Count > 0)
                    {
                        TreeChiTietKCB _SETY = new TreeChiTietKCB();
                        _SETY.CONCRETE_ID__IN_SETY = dataNew.ChiTietKCBLDO.hoSoKCB.dsXml2.First().MaNhom;
                        _SETY.TenDichVu = dataHeinSVTypes.FirstOrDefault(p => p.BHYT_CODE == _SETY.CONCRETE_ID__IN_SETY).HEIN_SERVICE_TYPE_NAME;
                        _TreeChiTietKCBs.Add(_SETY);

                        foreach (var item in dataNew.ChiTietKCBLDO.hoSoKCB.dsXml2)
                        {
                            TreeChiTietKCB leaf = new TreeChiTietKCB();
                            leaf.CONCRETE_ID__IN_SETY = _SETY.CONCRETE_ID__IN_SETY + "_" + item.Id;
                            leaf.PARENT_ID__IN_SETY = _SETY.CONCRETE_ID__IN_SETY;
                            leaf.MaDichVu = item.MaThuoc;
                            leaf.TenDichVu = item.TenThuoc;
                            leaf.DonViTinh = item.DonViTinh;
                            leaf.SoLuong = item.SoLuong;
                            leaf.DonGia = item.DonGia;
                            leaf.ThanhTien = item.ThanhTien;
                            _TreeChiTietKCBs.Add(leaf);
                        }
                    }
                }
                // _TreeChiTietKCBs = _TreeChiTietKCBs.OrderByDescending(o => o.TenDichVu).ToList();
                BindingList<TreeChiTietKCB> records = new BindingList<TreeChiTietKCB>(_TreeChiTietKCBs);
                treeListKCB.DataSource = records;
                treeListKCB.ExpandAll();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListKCB_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = (TreeChiTietKCB)treeListKCB.GetDataRecordByNode(e.Node);
                if (e.Node.HasChildren)
                {
                    e.Appearance.ForeColor = Color.Black;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
                else
                {
                    e.Appearance.ForeColor = Color.Black;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                PrintProcess("Mps000270");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void PrintProcess(string printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case "Mps000270":
                        richEditorMain.RunPrintTemplate("Mps000270", DelegateRunPrinter);
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000270":
                        Mps000270(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void Mps000270(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                MPS.Processor.Mps000270.PDO.Mps000270PDO mps000270RDO = new MPS.Processor.Mps000270.PDO.Mps000270PDO(
                this.resultHistoryLDO,
                BackendDataWorker.Get<HIS_MEDI_ORG>()
                );
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000270RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000270RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
