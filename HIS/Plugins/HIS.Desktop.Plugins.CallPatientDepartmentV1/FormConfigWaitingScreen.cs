using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Utility;
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

namespace HIS.Desktop.Plugins.CallPatientDepartmentV1
{
    public partial class FormConfigWaitingScreen : FormBase
    {
        private Inventec.Desktop.Common.Modules.Module moduleData;
        private System.Globalization.CultureInfo cultureLang;
        private const string frmWaitingScreenStr = "FormWaitingScreenV1";
        private List<ADO.RoomADO> ListRoom = new List<ADO.RoomADO>();
        private List<ADO.RoomADO> ListRoomFilter = new List<ADO.RoomADO>();
        private DXErrorProvider dxErrorProvider = new DXErrorProvider();
        bool IsLoad = false;

        private const string ModuleLinkName = "HIS.Desktop.Plugins.CallPatientDepartmentV1";
        private const string KEY_CHECKED_ROOM_IDS = "CHECKED_ROOM_IDS";
        private const string KEY_ROOM_ORDERS = "ROOM_ORDERS";
        private const string KEY_RELOAD_TIME = "RELOAD_TIME";
        private HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        private List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;

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
                SetCaptionByLanguageKey();

                LoadDataToGrid();
                InitControlState();

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
                        LblRoom.Text = (currentRoom.ROOM_NAME + " (" + currentRoom.DEPARTMENT_NAME + ")").ToUpper();
                        ListRoom = new List<ADO.RoomADO>();
                        var lstRoomDepartment = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().Where(o => o.DEPARTMENT_ID == currentRoom.DEPARTMENT_ID && o.IS_ACTIVE == (short)1).ToList();
                        if (lstRoomDepartment != null)
                        {
                            int index = 0;
                            foreach (var item in lstRoomDepartment)
                            {
                                ADO.RoomADO ado = new ADO.RoomADO();
                                ado.ROOM_ID = item.ROOM_ID;
                                ado.EXECUTE_ROOM_CODE = item.EXECUTE_ROOM_CODE;
                                ado.EXECUTE_ROOM_NAME = item.EXECUTE_ROOM_NAME;
                                ado.OrderIndex = 0;

                                if (item.ROOM_ID == currentRoom.ID)
                                {
                                    ado.IsCheck = true;
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
                        LblRoom.Text = "";
                    }

                    ListRoomFilter = new List<ADO.RoomADO>(ListRoom);
                    gridControl1.BeginUpdate();
                    gridControl1.DataSource = ListRoomFilter;
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
                this.tgExtendMonitor.Properties.OnText = "Bật màn hình mở rộng";
                this.tgExtendMonitor.Properties.OffText = "Bật màn hình mở rộng";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCheckRoom_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
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
                    if (!ValidateReloadTime())
                    {
                        tgExtendMonitor.IsOn = !tgExtendMonitor.IsOn;
                        return;
                    }

                    if (ListRoom.Exists(s => s.IsCheck))
                    {
                        int reloadTime = (int)spinReloadTime.Value;
                        SaveControlState();

                        var lstRoom = ListRoom.Where(o => o.IsCheck).OrderBy(o => o.OrderIndex).ToList();
                        FormWaitingScreenV1 screen = new FormWaitingScreenV1(lstRoom, moduleData, reloadTime);
                        ShowFormInExtendMonitor(screen);
                        this.Close();
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn phòng");
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

        private bool ValidateReloadTime()
        {
            bool isValid = true;
            try
            {
                dxErrorProvider.ClearErrors();
                if (spinReloadTime.EditValue == null || string.IsNullOrEmpty(spinReloadTime.Text) || (int)spinReloadTime.Value < 1)
                {
                    dxErrorProvider.SetError(spinReloadTime, "Bắt buộc nhập thời gian tải lại (tối thiểu 1 giây)", ErrorType.Warning);
                    isValid = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return isValid;
        }

        private void spinReloadTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (spinReloadTime.EditValue != null && !string.IsNullOrEmpty(spinReloadTime.Text) && (int)spinReloadTime.Value >= 1)
                {
                    dxErrorProvider.ClearErrors();
                }
                else
                {
                    dxErrorProvider.SetError(spinReloadTime, "Bắt buộc nhập thời gian tải lại (tối thiểu 1 giây)", ErrorType.Warning);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(ModuleLinkName);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    string checkedRoomIds = "";
                    string roomOrders = "";
                    string reloadTime = "";

                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == KEY_CHECKED_ROOM_IDS)
                            checkedRoomIds = item.VALUE;
                        else if (item.KEY == KEY_ROOM_ORDERS)
                            roomOrders = item.VALUE;
                        else if (item.KEY == KEY_RELOAD_TIME)
                            reloadTime = item.VALUE;
                    }

                    // Restore checked rooms
                    if (!string.IsNullOrEmpty(checkedRoomIds))
                    {
                        var arrIds = checkedRoomIds.Split(',');
                        // Bỏ check mặc định trước
                        foreach (var room in ListRoom)
                        {
                            room.IsCheck = false;
                        }
                        foreach (var idStr in arrIds)
                        {
                            long roomId;
                            if (long.TryParse(idStr.Trim(), out roomId))
                            {
                                var room = ListRoom.FirstOrDefault(o => o.ROOM_ID == roomId);
                                if (room != null)
                                    room.IsCheck = true;
                            }
                        }
                    }

                    // Restore order index
                    if (!string.IsNullOrEmpty(roomOrders))
                    {
                        // Format: roomId:orderIndex,roomId:orderIndex,...
                        var arrOrders = roomOrders.Split(',');
                        foreach (var pair in arrOrders)
                        {
                            var parts = pair.Split(':');
                            if (parts.Length == 2)
                            {
                                long roomId;
                                int orderIndex;
                                if (long.TryParse(parts[0].Trim(), out roomId) && int.TryParse(parts[1].Trim(), out orderIndex))
                                {
                                    var room = ListRoom.FirstOrDefault(o => o.ROOM_ID == roomId);
                                    if (room != null)
                                        room.OrderIndex = orderIndex;
                                }
                            }
                        }
                    }

                    // Restore reload time
                    if (!string.IsNullOrEmpty(reloadTime))
                    {
                        int time;
                        if (int.TryParse(reloadTime, out time) && time >= 1)
                        {
                            spinReloadTime.Value = time;
                        }
                    }

                    // Refresh grid
                    ListRoomFilter = new List<ADO.RoomADO>(ListRoom);
                    gridControl1.BeginUpdate();
                    gridControl1.DataSource = ListRoomFilter;
                    gridControl1.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveControlState()
        {
            try
            {
                if (this.currentControlStateRDO == null)
                    this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();

                Action<string, string> upsert = (key, value) =>
                {
                    var existing = this.currentControlStateRDO.FirstOrDefault(o => o.KEY == key && o.MODULE_LINK == ModuleLinkName);
                    if (existing != null)
                    {
                        existing.VALUE = value;
                    }
                    else
                    {
                        this.currentControlStateRDO.Add(new HIS.Desktop.Library.CacheClient.ControlStateRDO
                        {
                            KEY = key,
                            VALUE = value,
                            MODULE_LINK = ModuleLinkName
                        });
                    }
                };

                // Lưu danh sách phòng được tích
                var checkedRooms = ListRoom.Where(o => o.IsCheck).ToList();
                string checkedRoomIds = string.Join(",", checkedRooms.Select(o => o.ROOM_ID.ToString()));
                upsert(KEY_CHECKED_ROOM_IDS, checkedRoomIds);

                // Lưu vị trí của phòng (chỉ lưu phòng có OrderIndex > 0)
                var roomsWithOrder = ListRoom.Where(o => o.OrderIndex > 0).ToList();
                string roomOrders = string.Join(",", roomsWithOrder.Select(o => o.ROOM_ID + ":" + o.OrderIndex));
                upsert(KEY_ROOM_ORDERS, roomOrders);

                // Lưu thời gian tải lại
                upsert(KEY_RELOAD_TIME, ((int)spinReloadTime.Value).ToString());

                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSearch_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                string keyword = (txtSearch.Text ?? "").Trim().ToLower();
                if (string.IsNullOrEmpty(keyword))
                {
                    ListRoomFilter = new List<ADO.RoomADO>(ListRoom);
                }
                else
                {
                    ListRoomFilter = ListRoom.Where(o =>
                        (!string.IsNullOrEmpty(o.EXECUTE_ROOM_CODE) && o.EXECUTE_ROOM_CODE.ToLower().Contains(keyword)) ||
                        (!string.IsNullOrEmpty(o.EXECUTE_ROOM_NAME) && o.EXECUTE_ROOM_NAME.ToLower().Contains(keyword))
                    ).ToList();
                }
                gridControl1.BeginUpdate();
                gridControl1.DataSource = ListRoomFilter;
                gridControl1.EndUpdate();
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
    }
}
