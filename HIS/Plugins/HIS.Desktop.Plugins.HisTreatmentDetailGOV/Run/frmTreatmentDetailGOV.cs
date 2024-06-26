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
using AutoMapper;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using His.Bhyt.InsuranceExpertise.LDO;
using His.Bhyt.InsuranceExpertise.XML;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
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
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisTreatmentDetailGOV.Run
{
    public partial class frmHisTreatmentDetailGOV : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        string _maHoSo = "";

        public frmHisTreatmentDetailGOV()
        {
            InitializeComponent();
        }

        public frmHisTreatmentDetailGOV(Inventec.Desktop.Common.Modules.Module currentModule, string maHoSo)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this._maHoSo = maHoSo;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void frmHisTreatmentDetailGOV_Load(object sender, EventArgs e)
        {
            try
            {
                SetIconFrm();
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                //  var dat123 = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.HEIN_SERVICE_BHYT_CODE == "N03.05.010");
                CheckDetailClick();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async void CheckDetailClick()
        {
            try
            {
                HIS.Desktop.Plugins.Library.CheckHeinGOV.HeinGOVManager heinGOVManager = new Library.CheckHeinGOV.HeinGOVManager("MaLoiGOV___");

                List<TreeChiTietKCB> _TreeChiTietKCBs = new List<TreeChiTietKCB>();
                treeListKCB.DataSource = null;

                var dataNew = await heinGOVManager.CheckChiTietHS(this._maHoSo, "1");
                if (dataNew != null
                    && dataNew.ChiTietKCBLDO != null
                    && dataNew.ChiTietKCBLDO.hoSoKCB != null)
                {
                    FillData(dataNew.ChiTietKCBLDO.hoSoKCB.xml1);

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

        private void FillData(xml1 data)
        {
            try
            {
                if (data != null)
                {
                    txtMaBenhNhan.Text = data.MaBn;
                    txtMaTheBHYT.Text = data.MaThe;
                    txtKhuVuc.Text = data.MaKhuvuc;
                    txtKCBBanDau.Text = BackendDataWorker.Get<HIS_MEDI_ORG>().FirstOrDefault(p => p.MEDI_ORG_CODE == data.MaDkbd).MEDI_ORG_NAME;//code==ten
                    txtHoTen.Text = data.HoTen;
                    txtDiaChi.Text = data.DiaChi;
                    txtNgaySinh.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.NgaySinh);
                    txtGioiTinh.Text = data.GioiTinh == "2" ? "Nữ" : "Nam";
                    txtTenChaMe.Text = data.TenChame;
                    txtHanTu.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.GtTheTu);
                    txtHanDen.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.GtTheDen);
                    txtMienCUngChiTra.Text = data.Mieuta;
                    txtCanNang.Text = data.CanNang;
                    txtMaKhoa.Text = data.MaKhoa;
                    //txtMaBacSi.Text = data.MaBacsy;
                    txtMaBenh.Text = data.MaBenh;
                    txtMaBenhKhac.Text = data.MaBenhkhac;
                    txtNgayVao.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(Inventec.Common.TypeConvert.Parse.ToInt64(data.NgayVao + "00"));
                    txtNgayRa.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(Inventec.Common.TypeConvert.Parse.ToInt64(data.NgayRa + "00"));
                    txtMaTaiNan.Text = data.MaTaiNan;
                    txtNoiChuyenDen.Text = data.MaNoiChuyen;
                    // txtLyDoVaoVien.Text = data.MaLydoVvien;
                    // txtTenBacSi.Text = data.MaBacsy;
                    txtTenBenh.Text = data.TenBenh;
                    txtSoNgayDieuTri.Text = data.SoNgayDtri;
                    txtNgayThanhToan.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(Inventec.Common.TypeConvert.Parse.ToInt64(data.NgayTtoan + "00"));

                    string name = "";
                    if (data.TinhTrangRv == "1")
                        name = "Ra viện";
                    else if (data.TinhTrangRv == "2")
                        name = "Chuyển viện";
                    else if (data.TinhTrangRv == "3")
                        name = "Trốn viện";
                    else if (data.TinhTrangRv == "4")
                        name = "Xin ra viện";
                    txtTinhTrangRaVien.Text = name;
                }
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

    }
}
