using DevExpress.XtraEditors;
using DevExpress.XtraTreeList.Columns;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
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

namespace HIS.Desktop.Plugins.MedicalStoreV2.Popup
{
    public partial class frmStoreInfomation : Form
    {
        private List<HIS_LOCATION_STORE> lstLocationStore;
        private long mediRecordId;
        private HIS_MEDI_RECORD currentMediRecord;
        private Action refeshData;

        public frmStoreInfomation(long _mediRecordId, Action _refeshData)
        {
            InitializeComponent();
            try
            {
                this.mediRecordId = _mediRecordId;
                this.refeshData = _refeshData;
                SetIcon();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmStoreInfomation_Load(object sender, EventArgs e)
        {
            try
            {
                LoadComboLocationStore();
                LoadMediRecordData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadMediRecordData()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisMediRecordFilter filter = new HisMediRecordFilter();
                filter.ID = this.mediRecordId;

                var mediRecordList = new BackendAdapter(param).Get<List<HIS_MEDI_RECORD>>(
                    "api/HisMediRecord/Get",
                    ApiConsumers.MosConsumer,
                    filter,
                    param);

                if (mediRecordList != null && mediRecordList.Count > 0)
                {
                    this.currentMediRecord = mediRecordList.First();
                    SetDefaultData();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy thông tin bệnh án", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboLocationStore()
        {
            try
            {
                // Lấy dữ liệu vị trí từ Backend và sắp xếp theo Mã, Tên
                lstLocationStore = BackendDataWorker.Get<HIS_LOCATION_STORE>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .OrderBy(o => o.LOCATION_STORE_CODE)
                    .ThenBy(o => o.LOCATION_STORE_NAME)
                    .ToList();

                // Cấu hình TreeListLookUpEdit
                cboLoacationStore.Properties.DataSource = lstLocationStore;
                cboLoacationStore.Properties.DisplayMember = "LOCATION_STORE_NAME";
                cboLoacationStore.Properties.ValueMember = "ID";

                // Cấu hình TreeList để hiển thị dạng cây phân cấp
                cboLoacationStore.Properties.TreeList.KeyFieldName = "ID";
                cboLoacationStore.Properties.TreeList.ParentFieldName = "PARENT_ID";

                // Xóa các cột cũ nếu có
                cboLoacationStore.Properties.TreeList.Columns.Clear();

                // Thêm cột Mã vị trí
                TreeListColumn colCode = cboLoacationStore.Properties.TreeList.Columns.Add();
                colCode.FieldName = "LOCATION_STORE_CODE";
                colCode.Caption = "Mã";
                colCode.Visible = true;
                colCode.VisibleIndex = 0;
                colCode.Width = 100;

                // Thêm cột Tên vị trí
                TreeListColumn colName = cboLoacationStore.Properties.TreeList.Columns.Add();
                colName.FieldName = "LOCATION_STORE_NAME";
                colName.Caption = "Tên vị trí";
                colName.Visible = true;
                colName.VisibleIndex = 1;
                colName.Width = 250;

                // Cấu hình các thuộc tính
                cboLoacationStore.Properties.PopupFormSize = new Size(400, 300);
                cboLoacationStore.Properties.ImmediatePopup = true;
                cboLoacationStore.Properties.TreeList.OptionsView.ShowCheckBoxes = false;
                cboLoacationStore.Properties.TreeList.OptionsSelection.EnableAppearanceFocusedCell = false;
                cboLoacationStore.Properties.TreeList.OptionsView.ShowIndicator = false;
                cboLoacationStore.Properties.TreeList.OptionsView.AutoWidth = false;
                cboLoacationStore.Properties.TreeList.OptionsBehavior.EnableFiltering = true;

                // Expand tất cả các node
                cboLoacationStore.Properties.TreeList.ExpandAll();

                // Thêm button xóa
                cboLoacationStore.Properties.Buttons.Clear();
                cboLoacationStore.Properties.Buttons.Add(new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo));
                cboLoacationStore.Properties.Buttons.Add(new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultData()
        {
            try
            {
                if (currentMediRecord != null)
                {
                    // Hiển thị mã lưu trữ
                    txtStoreCode.Text = currentMediRecord.STORE_CODE;
                    txtStoreCode.Enabled = false; // Không cho sửa mã lưu trữ

                    // Hiển thị vị trí mặc định nếu có LOCATION_STORE_ID
                    if (currentMediRecord.LOCATION_STORE_ID.HasValue)
                    {
                        cboLoacationStore.EditValue = currentMediRecord.LOCATION_STORE_ID.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;

                if (currentMediRecord == null)
                {
                    MessageBox.Show("Không có thông tin bệnh án để cập nhật", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Copy toàn bộ object hiện tại
                HIS_MEDI_RECORD updateData = new HIS_MEDI_RECORD();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_MEDI_RECORD>(updateData, currentMediRecord);

                // Chỉ update field cần sửa
                if (cboLoacationStore.EditValue != null)
                {
                    updateData.LOCATION_STORE_ID = Convert.ToInt64(cboLoacationStore.EditValue);
                }
                else
                {
                    updateData.LOCATION_STORE_ID = null;
                }

                WaitingManager.Show();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("HisMediRecord/Update Input", updateData));

                // Gọi API cập nhật
                var result = new BackendAdapter(param).Post<HIS_MEDI_RECORD>(
                    "api/HisMediRecord/Update",
                    ApiConsumers.MosConsumer,
                    updateData,
                    param);

                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("HisMediRecord/Update Output", result));

                if (result != null)
                {
                    success = true;
                    MessageManager.Show(this, param, success);

                    // Refresh lại dữ liệu
                    if (this.refeshData != null)
                    {
                        this.refeshData();
                    }

                    this.Close();
                }
                else
                {
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                MessageBox.Show("Có lỗi xảy ra khi cập nhật thông tin", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cboLoacationStore_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboLoacationStore.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboLoacationStore_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboLoacationStore.EditValue != null)
                {
                    var data = lstLocationStore.FirstOrDefault(o => o.ID == Int64.Parse(cboLoacationStore.EditValue.ToString()));
                    //if (data != null)
                    //{
                    //    txtStoreCode.Text = data.LOCATION_STORE_CODE;
                    //}
                }
                //else { txtStoreCode.Text = null; }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}