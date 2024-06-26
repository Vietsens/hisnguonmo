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
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.RegisterExamKiosk.ADO;
using HIS.UC.Icd.ADO;
using Inventec.Common.Controls.EditorLoader;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.RegisterExamKiosk.Popup.InputSave
{
    public partial class frmInputSave1 : Form
    {
        HIS.Desktop.Common.DelegateSelectData selectData;
        HIS.UC.Icd.IcdProcessor icdProcessor;
        UserControl ucIcd = null;
        List<RightRouteTypeADO> rightAdo;

        public frmInputSave1()
        {
            InitializeComponent();
        }

        public frmInputSave1(HIS.Desktop.Common.DelegateSelectData data)
        {
            InitializeComponent();
            try
            {
                this.selectData = data;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void GetDataRightRouteType()
        {
            try
            {
                rightAdo = new List<RightRouteTypeADO>();
                rightAdo.Add(new RightRouteTypeADO(1, MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.APPOINTMENT, "Hẹn khám"));
                rightAdo.Add(new RightRouteTypeADO(2, MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY, "Cấp cứu"));
                rightAdo.Add(new RightRouteTypeADO(3, MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT, "Giới thiệu"));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadCombo()
        {
            try
            {
                GetDataRightRouteType();
                HIS.UC.Icd.ADO.IcdInitADO icdAdo = new UC.Icd.ADO.IcdInitADO();
                string sizeText = "16";
                icdAdo.Height = 40;
                icdAdo.Width = 812;
                icdAdo.SizeText = Inventec.Common.TypeConvert.Parse.ToFloat(sizeText);
                icdAdo.DataIcds = BackendDataWorker.Get<HIS_ICD>();
                icdAdo.LblIcdMain = "Bệnh chính";
                icdProcessor = new UC.Icd.IcdProcessor();
                ucIcd = (UserControl)icdProcessor.Run(icdAdo);

                if (ucIcd != null)
                {
                    this.layoutControl2.Controls.Add(ucIcd);
                    ucIcd.Dock = DockStyle.Fill;
                }
                LoadComboRightRoute();
                //InitCombo(cboTruongHop, rightAdo, "RightRouteName", "RightRouteCode", "RightRouteCode", "RightRouteName");
                InitCombo(cboNoiChuyenDen, BackendDataWorker.Get<HIS_MEDI_ORG>(), "MEDI_ORG_NAME", "MEDI_ORG_CODE", "MEDI_ORG_CODE", "MEDI_ORG_NAME");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadComboRightRoute()
        {
            try
            {
                rightAdo = new List<RightRouteTypeADO>();
                rightAdo.Add(new RightRouteTypeADO(1, "CC", "Cấp cứu"));
                rightAdo.Add(new RightRouteTypeADO(2, "HK", "Hẹn khám"));
                rightAdo.Add(new RightRouteTypeADO(3, "GT", "Giới thiệu"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("RightRouteTypeCode", "", 150, 1));
                columnInfos.Add(new ColumnInfo("RightRouteTypeName", "", 350, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("RightRouteTypeName", "RightRouteTypeCode", columnInfos, false, 500);
                ControlEditorLoader.Load(cboTruongHop, rightAdo, controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCombo(GridLookUpEdit cbo, object data, string displayMember, string valueMember, string column1, string column2)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo(column1, "", 100, 1));
                columnInfos.Add(new ColumnInfo(column2, "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, 350);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
                //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("data", data));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void frmInputSave1_Load(object sender, EventArgs e)
        {
            try
            {
                LoadCombo();
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
                if (this.selectData != null)
                {
                    InputSaveADO ado = new InputSaveADO();
                    if (cboTruongHop.EditValue != null)
                    {
                        ado.RightRouteTypeCode = cboTruongHop.EditValue.ToString();
                    }
                    if (cboNoiChuyenDen.EditValue != null)
                    {
                        ado.MediOrgCode = cboNoiChuyenDen.EditValue.ToString();
                        ado.MediOrgName = cboNoiChuyenDen.Text;
                    }

                    ado.InCode = txtSoChuyenVien.Text;

                    IcdInputADO inputAdo = (IcdInputADO)icdProcessor.GetValue(ucIcd);
                    if (inputAdo != null)
                    {
                        ado.IcdCode = inputAdo.ICD_CODE;
                        ado.IcdName = inputAdo.ICD_NAME;
                    }

                    this.selectData(ado);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNoiChuyenDen_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtNoiChuyenDenCode.Text))
                    {
                        cboNoiChuyenDen.EditValue = null;
                        cboNoiChuyenDen.Focus();
                        cboNoiChuyenDen.ShowPopup();
                    }
                    else
                    {
                        List<HIS_MEDI_ORG> searchs = null;
                        var listData1 = BackendDataWorker.Get<HIS_MEDI_ORG>().Where(o => o.MEDI_ORG_CODE.ToUpper().Contains(txtNoiChuyenDenCode.Text.ToUpper()) && o.IS_ACTIVE == 1).ToList();
                        if (listData1 != null && listData1.Count > 0)
                        {
                            searchs = (listData1.Count == 1) ? listData1 : (listData1.Where(o => o.MEDI_ORG_CODE.ToUpper() == txtNoiChuyenDenCode.Text.ToUpper()).ToList());
                        }
                        if (searchs != null && searchs.Count == 1)
                        {
                            txtNoiChuyenDenCode.Text = searchs[0].MEDI_ORG_CODE;
                            cboNoiChuyenDen.EditValue = searchs[0].MEDI_ORG_CODE;
                            icdProcessor.FocusControl(ucIcd);
                        }
                        else
                        {
                            cboNoiChuyenDen.Focus();
                            cboNoiChuyenDen.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNoiChuyenDen_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboNoiChuyenDen.EditValue != null)
                    {
                        var data = BackendDataWorker.Get<HIS_MEDI_ORG>().SingleOrDefault(o => o.MEDI_ORG_CODE == (cboNoiChuyenDen.EditValue ?? "").ToString());
                        if (data != null)
                        {
                            txtNoiChuyenDenCode.Text = data.MEDI_ORG_CODE;
                            icdProcessor.FocusControl(ucIcd);
                        }
                    }
                    else
                    {
                        cboNoiChuyenDen.Focus();
                        cboNoiChuyenDen.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNoiChuyenDen_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboNoiChuyenDen.EditValue != null)
                    {
                        var data = BackendDataWorker.Get<HIS_MEDI_ORG>().SingleOrDefault(o => o.MEDI_ORG_CODE == (cboNoiChuyenDen.EditValue ?? "").ToString());
                        if (data != null)
                        {
                            txtNoiChuyenDenCode.Text = data.MEDI_ORG_CODE;
                            icdProcessor.FocusControl(ucIcd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTruongHop_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtTruongHop.Text))
                    {
                        cboTruongHop.EditValue = null;
                        cboTruongHop.Focus();
                        cboTruongHop.ShowPopup();
                    }
                    else
                    {
                        List<RightRouteTypeADO> searchs = null;
                        var listData1 = rightAdo.Where(o => o.RightRouteTypeCode.ToUpper().Contains(txtTruongHop.Text.ToUpper())).ToList();
                        if (listData1 != null && listData1.Count > 0)
                        {
                            searchs = (listData1.Count == 1) ? listData1 : (listData1.Where(o => o.RightRouteTypeCode.ToUpper() == txtTruongHop.Text.ToUpper()).ToList());
                        }
                        if (searchs != null && searchs.Count == 1)
                        {
                            txtTruongHop.Text = searchs[0].RightRouteTypeCode;
                            cboTruongHop.EditValue = searchs[0].RightRouteTypeCode;
                            txtNoiChuyenDenCode.Focus();
                            txtNoiChuyenDenCode.SelectAll();
                        }
                        else
                        {
                            cboTruongHop.Focus();
                            cboTruongHop.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTruongHop_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboTruongHop.EditValue != null)
                    {
                        var data = rightAdo.SingleOrDefault(o => o.RightRouteTypeCode == (cboTruongHop.EditValue ?? "").ToString());
                        if (data != null)
                        {
                            txtTruongHop.Text = data.RightRouteTypeCode;
                            txtNoiChuyenDenCode.Focus();
                            txtNoiChuyenDenCode.SelectAll();
                        }
                    }
                    else
                    {
                        cboTruongHop.Focus();
                        cboTruongHop.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTruongHop_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboTruongHop.EditValue != null)
                    {
                        var data = rightAdo.SingleOrDefault(o => o.RightRouteTypeCode == (cboTruongHop.EditValue ?? "").ToString());
                        if (data != null)
                        {
                            txtTruongHop.Text = data.RightRouteTypeCode;
                            txtNoiChuyenDenCode.Focus();
                            txtNoiChuyenDenCode.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSoChuyenVien_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTruongHop.Focus();
                    txtTruongHop.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }



    }
}
