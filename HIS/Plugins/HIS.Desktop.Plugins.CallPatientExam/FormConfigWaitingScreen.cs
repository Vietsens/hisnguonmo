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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CallPatientExam
{
    public partial class FormConfigWaitingScreen : FormBase
    {
        private Inventec.Desktop.Common.Modules.Module moduleData;
        private System.Globalization.CultureInfo cultureLang;
        private const string frmWaitingScreenStr = "FormWaitingScreen99";
        private List<ADO.RoomADO> ListRoom = new List<ADO.RoomADO>();
        
        bool IsLoad = false;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.CallPatientExam";
        public FormConfigWaitingScreen()
        {
            InitializeComponent();
        }

        public FormConfigWaitingScreen(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            FormCollection fc = Application.OpenForms;
            foreach (Form frm in fc)
            {
                if (frm.Name == frmWaitingScreenStr)
                {
                    this.Close();
                    return;
                }
            }
            InitializeComponent();
            try
            {
                this.moduleData = moduleData;
                this.cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                this.Text = moduleData.text;
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationManager.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormConfigWaitingScreen_Load(object sender, EventArgs e)
        {
            try
            {
                IsLoad = true;
                //Load ngon ngu label control
                SetCaptionByLanguageKey();
                InitControlState();
                LoadDataToGrid();
                if (Application.OpenForms != null && Application.OpenForms.Count > 0)
                {
                    for (int i = 0; i < Application.OpenForms.Count; i++)
                    {
                        Form f = Application.OpenForms[i];
                        if (f.Name == frmWaitingScreenStr)
                        {
                            tgExtendMonitor.IsOn = true;
                        }
                    }
                }
                IsLoad = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToGrid()
        {
            try
            {
                if (moduleData != null && moduleData.RoomId > 0)
                {
                   
                    var currentRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == moduleData.RoomId);
                    if (currentRoom != null)
                    {
                        //LblRoom.Text = (currentRoom.ROOM_NAME + " (" + currentRoom.DEPARTMENT_NAME + ")").ToUpper();
                        ListRoom = new List<ADO.RoomADO>();
                        //o.DEPARTMENT_ID == currentRoom.DEPARTMENT_ID &&
                        var lstRoomDepartment = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().Where(o =>  o.IS_ACTIVE == (short) 1).ToList();
                        //var lstBedRoom = BackendDataWorker.Get<V_HIS_BED_ROOM>().Where(o => o.DEPARTMENT_ID == currentRoom.DEPARTMENT_ID && o.IS_ACTIVE == (short)1).ToList();
                        
                        if (lstRoomDepartment != null)
                        {
                            int index=0;
                            foreach (var item in lstRoomDepartment)
                            {
                                ADO.RoomADO ado = new ADO.RoomADO();

                                ado.EXECUTE_ROOM_CODE = item.EXECUTE_ROOM_CODE;
                                ado.EXECUTE_ROOM_NAME = item.EXECUTE_ROOM_NAME;
                                ado.IS_VACCINE = item.IS_VACCINE;
                                ado.IS_EXAM = item.IS_EXAM;
                                ado.ID = item.ROOM_ID;
                                if (item.ROOM_ID == currentRoom.ID)
                                {
                                    ado.IsCheck = true;
                                    ado.CategoryChoose = 2;
                                    ListRoom.Insert(index, ado);
                                    index++;
                                }
                                else
                                {
                                    ListRoom.Add(ado);
                                }
                            }
                        }

                    }
                    else
                    {
                       // LblRoom.Text = "";
                    }
                    var ListRoom_ = ListRoom.Distinct().Where(o => o.IS_EXAM == 1 || o.IS_VACCINE == 1).ToList();
                    gridControl1.BeginUpdate();
                   
                    if (chkRoom.Checked == true)
                    {
                        gridControl1.DataSource = ListRoom_.Where(o => o.ROOM_ID == moduleData.RoomId);
                    }
                    else
                    {
                        gridControl1.DataSource = ListRoom_;
                    }
                    
                    gridControl1.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                this.tgExtendMonitor.Properties.OnText = GetLanguageControl("HIS_DESKTOP_PLUGINS__CALL_PATIENT_DEPARTMENT__TOG_EXTEND_MONITOR__ON_TEXT");
                this.tgExtendMonitor.Properties.OffText = GetLanguageControl("HIS_DESKTOP_PLUGINS__CALL_PATIENT_DEPARTMENT__TOG_EXTEND_MONITOR__OFF_TEXT");
                this.Gc_RoomCode.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS__CALL_PATIENT_DEPARTMENT__GC_ROOM_CODE");
                this.Gc_RoomName.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS__CALL_PATIENT_DEPARTMENT__GC_ROOM_NAME");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetLanguageControl(string key)
        {
            return Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, cultureLang);
        }

        private void repositoryItemCheckRoom_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var check = (DevExpress.XtraEditors.CheckEdit)sender;
                if (check != null && check.Checked)
                {
                    var countCheck = ListRoom.Count(c => c.IsCheck);
                    if (countCheck > 5)
                    {
                        check.Checked = false;
                        DevExpress.XtraEditors.XtraMessageBox.Show("Số phòng được chọn không vượt quá 6");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void tgExtendMonitor_Toggled(object sender, EventArgs e)
        {
            try
            {
                if (IsLoad) return;

                if (tgExtendMonitor.IsOn)
                {
                    if (ListRoom.Exists(s => s.IsCheck))
                    {
                        var lstRoom = ListRoom.Where(o => o.IsCheck).ToList();                       
                        FormWaitingScreen99 screen = new FormWaitingScreen99(lstRoom, moduleData,lstRoom.FirstOrDefault().CategoryChoose);
                        ShowFormInExtendMonitor(screen);
                        this.Close();
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn phòng/buồng");
                        tgExtendMonitor.IsOn = !tgExtendMonitor.IsOn;
                    }
                }
                else
                {
                    if (Application.OpenForms != null && Application.OpenForms.Count > 0)
                    {
                        for (int i = 0; i < Application.OpenForms.Count; i++)
                        {
                            Form f = Application.OpenForms[i];
                            if (f.Name == frmWaitingScreenStr)
                            {
                                f.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal static void ShowFormInExtendMonitor(Form control)
        {
            try
            {
                Screen[] sc;
                sc = Screen.AllScreens;
                if (sc.Length <= 1)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy màn hình mở rộng");
                    control.Show();
                }
                else
                {
                    var sc2 = sc.FirstOrDefault(o => !o.Primary);
                    if (sc2 == null) sc2 = sc[1];

                    control.FormBorderStyle = FormBorderStyle.None;
                    control.Left = sc2.Bounds.Width;
                    control.Top = sc2.Bounds.Height;
                    control.StartPosition = FormStartPosition.Manual;
                    control.Location = sc2.Bounds.Location;
                    Point p = new Point(sc2.Bounds.Location.X, sc2.Bounds.Location.Y);
                    control.Location = p;
                    control.WindowState = FormWindowState.Maximized;
                    control.Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkRoom_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                //if (chkSign.Checked == false)
                //{
                //    chkPrintDocumentSigned.Checked = false;
                //}

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == Plugins.CallPatientDepartment.ControlStateConstan.chkRoom && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkRoom.Checked ? "1" : "");
                }
                else
                {
                    
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = Plugins.CallPatientDepartment.ControlStateConstan.chkRoom;
                    csAddOrUpdate.VALUE = (chkRoom.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                LoadDataToGrid();
                WaitingManager.Hide();
                
            }
            catch (Exception ex)
            {

               
            }
            
        }
        void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == Plugins.CallPatientDepartment.ControlStateConstan.chkRoom)
                        {
                            chkRoom.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
