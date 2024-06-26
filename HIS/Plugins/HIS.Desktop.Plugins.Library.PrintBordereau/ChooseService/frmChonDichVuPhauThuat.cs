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
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Plugins.Library.PrintBordereau.ADO;
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

namespace HIS.Desktop.Plugins.Library.PrintBordereau.ChooseService
{
    public delegate void DelegateSereServKTC(object data);

    public partial class frmChonDichVuPhauThuat : Form
    {
        List<HIS_SERE_SERV> sereServs = new List<HIS_SERE_SERV>();
        List<SereServADO> SereServADOs { get; set; }
        HIS_SERE_SERV sereServ = new HIS_SERE_SERV();
        DelegateSereServKTC sereServKTCChoose;

        public frmChonDichVuPhauThuat()
        {
            InitializeComponent();
        }

        public frmChonDichVuPhauThuat(List<HIS_SERE_SERV> sereServs, DelegateSereServKTC sereServKTCChoose)
        {
            InitializeComponent();
            this.sereServKTCChoose = sereServKTCChoose;
            if (sereServs != null && sereServs.Count > 0)
            {
                this.sereServs = sereServs.OrderBy(o => o.TDL_SERVICE_CODE).ThenBy(o => o.ID).ToList();
            }
        }

        private void LoadDataToControl()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboDichVuPhauThuat, SereServADOs, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDichVuPhauThuat_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDichVuPhauThuat.EditValue != null)
                    {
                        SereServADO data = SereServADOs.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDichVuPhauThuat.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtDichVuPhauThuat.Text = data.SERVICE_CODE;
                            cboDichVuPhauThuat.Properties.Buttons[1].Visible = true;
                            btnChonDichVu.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDichVuPhauThuat_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDichVuPhauThuat.Properties.Buttons[1].Visible = false;
                    cboDichVuPhauThuat.EditValue = null;
                    txtDichVuPhauThuat.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDichVuPhauThuat_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadServiceCombo(strValue, false, cboDichVuPhauThuat, btnChonDichVu, sereServs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnChonDichVu_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboDichVuPhauThuat.EditValue != null)
                {
                    sereServ = sereServs.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDichVuPhauThuat.EditValue ?? 0).ToString()));
                    if (sereServKTCChoose != null)
                        sereServKTCChoose(sereServ);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy dịch vụ phẫu thuật thủ thuật");
                }

                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmChonDichVuPhauThuat_Load(object sender, EventArgs e)
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(Inventec.Desktop.Common.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                SereServADOs = new List<SereServADO>();
                List<V_HIS_SERVICE> services = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>();
                foreach (var item in this.sereServs)
                {
                    V_HIS_SERVICE service = services.FirstOrDefault(o => o.ID == item.SERVICE_ID);
                    AutoMapper.Mapper.CreateMap<HIS_SERE_SERV, SereServADO>();
                    SereServADO sereServADO = AutoMapper.Mapper.Map<HIS_SERE_SERV, SereServADO>(item);
                    if (service != null)
                    {
                        sereServADO.SERVICE_CODE = service.SERVICE_CODE;
                        sereServADO.SERVICE_NAME = service.SERVICE_NAME;
                    }
                    SereServADOs.Add(sereServADO);
                }

                LoadDataToControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadServiceCombo(string searchCode, bool isExpand, DevExpress.XtraEditors.GridLookUpEdit cboService, DevExpress.XtraEditors.SimpleButton btn, List<HIS_SERE_SERV> sereServKTCs)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboService.EditValue = null;
                    cboService.Focus();
                    cboService.ShowPopup();
                }
                else
                {
                    var data = sereServKTCs.Where(o => o.TDL_SERVICE_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboService.EditValue = data[0].ID;
                            txtDichVuPhauThuat.Text = data[0].TDL_SERVICE_CODE;
                        }
                        else if (data.Count > 1)
                        {
                            cboService.EditValue = null;
                            cboService.Focus();
                            cboService.ShowPopup();
                        }
                    }
                    else
                    {
                        cboService.EditValue = null;
                        cboService.Focus();
                        cboService.ShowPopup();
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
