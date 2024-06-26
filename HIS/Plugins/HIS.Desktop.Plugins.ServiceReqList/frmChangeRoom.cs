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
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Plugins.ServiceReqList.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServiceReqList
{
    public partial class frmChangeRoom : Form
    {
        int positionHandleControl = -1;
        List<ServiceReqADO> listServiceReqADO;
        List<HIS_EXECUTE_ROOM> rooms;
        Inventec.Desktop.Common.Modules.Module Module;

        public frmChangeRoom()
        {
            InitializeComponent();
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public frmChangeRoom(Inventec.Desktop.Common.Modules.Module module,List<ServiceReqADO> _listServiceReq)
        {
            InitializeComponent();
            try
            {
                this.listServiceReqADO = _listServiceReq;
                this.Module = module;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmChangeRoom_Load(object sender, EventArgs e)
        {
            SetCaptionByLanguageKey();
            LoadComboExcuteRoom();
            LoadDefaultValue();
        }

        private void LoadDefaultValue()
        {
            try
            {
                var check = this.listServiceReqADO.Where(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH).FirstOrDefault();
                if (check != null)
                {
                    chkClinic.Checked = true;
                }
                cboRoom.EditValue = null;
                txtRoom.Text = "";
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

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmChangeRoom.layoutControl1.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.btnSaveRoom.Text = Inventec.Common.Resource.Get.Value("frmChangeRoom.btnSaveRoom.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.cboRoom.Properties.NullText = Inventec.Common.Resource.Get.Value("frmChangeRoom.cboRoom.Properties.NullText", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.chkOperatingRoom.Properties.Caption = Inventec.Common.Resource.Get.Value("frmChangeRoom.chkOperatingRoom.Properties.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.chkEmergencyRoom.Properties.Caption = Inventec.Common.Resource.Get.Value("frmChangeRoom.chkEmergencyRoom.Properties.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.chkClinic.Properties.Caption = Inventec.Common.Resource.Get.Value("frmChangeRoom.chkClinic.Properties.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmChangeRoom.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmChangeRoom.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmChangeRoom.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmChangeRoom.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmChangeRoom.bar2.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.bbtnSaveRoom.Caption = Inventec.Common.Resource.Get.Value("frmChangeRoom.bbtnSaveRoom.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmChangeRoom.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboExcuteRoom()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExecuteRoomFilter filter = new HisExecuteRoomFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                var data = new BackendAdapter(param).Get<List<HIS_EXECUTE_ROOM>>("api/HisExecuteRoom/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, null).ToList();

                if (chkClinic.Checked)
                {
                    data = data.Where(o => o.IS_EXAM == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                }
                if (chkEmergencyRoom.Checked)
                {
                    data = data.Where(o => o.IS_EMERGENCY == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                }
                if (chkOperatingRoom.Checked)
                {
                    data = data.Where(o => o.IS_SURGERY == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                }
                this.rooms = data;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROOM_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboRoom, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void LoadExeRoomCombo(string searchCode)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboRoom.EditValue = null;
                    cboRoom.Focus();
                    cboRoom.ShowPopup();
                }
                else
                {
                    List<HIS_EXECUTE_ROOM> searchs = null;
                    var listDataRoom = this.rooms.Where(o => o.EXECUTE_ROOM_CODE.ToUpper().Contains(searchCode.ToUpper())).ToList();
                    if (listDataRoom != null && listDataRoom.Count > 0)
                    {
                        searchs = (listDataRoom.Count == 1) ? listDataRoom : (listDataRoom.Where(o => o.EXECUTE_ROOM_CODE.ToUpper() == searchCode.ToUpper()).ToList());
                    }
                    if (searchs != null && searchs.Count == 1)
                    {
                        txtRoom.Text = searchs[0].EXECUTE_ROOM_CODE;
                        cboRoom.EditValue = searchs[0].ID;
                    }
                    else
                    {
                        cboRoom.Focus();
                        cboRoom.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSaveRoom_Click(object sender, EventArgs e)
        {
            
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                this.positionHandleControl = -1;

                ChangeRoomSDO hisChangeRoom = new ChangeRoomSDO();

                UpdateData(ref hisChangeRoom);

                WaitingManager.Show();
                var result = new BackendAdapter(param).Post<bool>(ApiConsumer.HisRequestUriStore.HIS_SERVICE_REQ_CHANGEROOMLIST, ApiConsumer.ApiConsumers.MosConsumer, hisChangeRoom, param);
                if (result == true)
                {
                    success = true;
                    //lblNumOrder.Text = this.currentHisServiceReq.NUM_ORDER != null ? this.currentHisServiceReq.NUM_ORDER.ToString() : "";
                }

                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this, param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void UpdateData(ref ChangeRoomSDO hisChangeRoom)
        {
            try
            {
                hisChangeRoom.ServiceReqIds = this.listServiceReqADO.Select(o => o.ID).ToList();
                var roomId = this.rooms.Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboRoom.EditValue.ToString())).FirstOrDefault().ROOM_ID;
                hisChangeRoom.ExecuteRoomId = roomId;
                hisChangeRoom.RequestRoomId = Module.RoomId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSaveRoom_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSaveRoom_Click(null, null);
        }

        private void chkClinic_CheckedChanged(object sender, EventArgs e)
        {
            LoadComboExcuteRoom();
            cboRoom.EditValue = null;
            txtRoom.Text = "";
            chkClinic.Focus();
        }

        private void chkEmergencyRoom_CheckedChanged(object sender, EventArgs e)
        {
            LoadComboExcuteRoom();
            cboRoom.EditValue = null;
            txtRoom.Text = "";
            chkEmergencyRoom.Focus();
        }

        private void chkOperatingRoom_CheckedChanged(object sender, EventArgs e)
        {
            LoadComboExcuteRoom();
            cboRoom.EditValue = null;
            txtRoom.Text = "";
            chkOperatingRoom.Focus();
        }

        private void cboRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboRoom.EditValue != null)
                    {
                        var data = this.rooms.Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboRoom.EditValue ?? 0).ToString())).FirstOrDefault();
                        if (data != null)
                        {
                            txtRoom.Text = data.EXECUTE_ROOM_CODE;
                            cboRoom.Properties.Buttons[1].Visible = true;
                            btnSaveRoom.Focus();

                            //e.Handled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRoom_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //if (cboRoom.EditValue != null)
                    //{
                    //    MOS.EFMODEL.DataModels.HIS_EXECUTE_ROOM data = this.rooms.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboRoom.EditValue ?? 0).ToString()));
                    //    if (data != null)
                    //    {
                    //        txtRoom.Text = data.EXECUTE_ROOM_CODE;

                    //        e.Handled = true;
                    //    }
                    //}
                    btnSaveRoom.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRoom_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboRoom.Properties.Buttons[1].Visible = false;
                    cboRoom.EditValue = null;
                    txtRoom.Text = "";
                    txtRoom.Focus();
                    txtRoom.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtRoom_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadExeRoomCombo(txtRoom.Text);
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkClinic_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkEmergencyRoom.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkEmergencyRoom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkOperatingRoom.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkOperatingRoom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtRoom.Focus();
                    txtRoom.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmChangeRoom_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Module = null;
                rooms = null;
                listServiceReqADO = null;
                positionHandleControl = 0;
                this.btnSaveRoom.Click -= new System.EventHandler(this.btnSaveRoom_Click);
                this.cboRoom.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboRoom_Closed);
                this.cboRoom.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboRoom_ButtonClick);
                this.cboRoom.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.cboRoom_KeyDown);
                this.txtRoom.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtRoom_KeyDown);
                this.chkOperatingRoom.CheckedChanged -= new System.EventHandler(this.chkOperatingRoom_CheckedChanged);
                this.chkOperatingRoom.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkOperatingRoom_PreviewKeyDown);
                this.chkEmergencyRoom.CheckedChanged -= new System.EventHandler(this.chkEmergencyRoom_CheckedChanged);
                this.chkEmergencyRoom.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkEmergencyRoom_PreviewKeyDown);
                this.chkClinic.CheckedChanged -= new System.EventHandler(this.chkClinic_CheckedChanged);
                this.chkClinic.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkClinic_PreviewKeyDown);
                this.bbtnSaveRoom.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnSaveRoom_ItemClick);
                this.Load -= new System.EventHandler(this.frmChangeRoom_Load);
                gridLookUpEdit1View.GridControl.DataSource = null;
                barDockControlRight = null;
                barDockControlLeft = null;
                barDockControlBottom = null;
                barDockControlTop = null;
                bbtnSaveRoom = null;
                bar2 = null;
                barManager1 = null;
                emptySpaceItem1 = null;
                layoutControlItem6 = null;
                layoutControlItem5 = null;
                layoutControlItem4 = null;
                layoutControlItem3 = null;
                layoutControlItem2 = null;
                layoutControlItem1 = null;
                chkClinic = null;
                chkEmergencyRoom = null;
                chkOperatingRoom = null;
                txtRoom = null;
                gridLookUpEdit1View = null;
                cboRoom = null;
                btnSaveRoom = null;
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
