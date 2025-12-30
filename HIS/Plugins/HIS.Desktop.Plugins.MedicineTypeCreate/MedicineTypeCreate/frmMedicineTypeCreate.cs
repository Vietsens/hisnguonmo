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
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using SDA.EFMODEL.DataModels;
using HIS.Desktop.Plugins.MedicineTypeCreate.ADO;
using System.Windows;
using MOS.SDO;
using HIS.Desktop.Utilities.Extensions;
using System.Text;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Columns;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Data;
using HIS.Desktop.Plugins.MedicineTypeCreate.Popup;
using HIS.Desktop.Plugins.MedicineTypeCreate.Config;
using HIS.Desktop.ADO;
using HIS.Desktop.Utility;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MedicineTypeCreate.MedicineTypeCreate
{
    public partial class frmMedicineTypeCreate : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int positionHandleControlMedicineTypeInfo = -1;
        MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE currentVHisMedicineTypeDTODefault;
        MOS.EFMODEL.DataModels.V_HIS_SERVICE currentVHisServiceDTODefault;
        MOS.EFMODEL.DataModels.HIS_STORAGE_CONDITION currentHisStoreCondition;
        int ActionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;// mac dinh la them
        long? currentMedicineTypeId = null;
        Inventec.Desktop.Common.Modules.Module module;
        HIS_MEDICINE_TYPE resultData = null;
        DelegateSelectData delegateSelect;
        List<HIS_ACTIVE_INGREDIENT> activeIngrBhyts;
        List<HIS_ACTIVE_INGREDIENT> activeIngredients;
        List<HIS_ATC> Acts;
        List<HIS_ATC_GROUP> ActGroups;
        V_HIS_SERVICE currentRightClick;
        V_HIS_SERVICE currentRightClickDefault;
        HIS.UC.National.NationalProcessor nationalProcessor;
        UserControl ucNational;
        List<HIS_DOSAGE_FORM> dataDosageForm;
        internal List<ADO.VHisServicePatyADO> lsVHisServicePaty { get; set; }
        internal List<ADO.VHisServicePatyADO> lsVHisServicePatyBegin = new List<VHisServicePatyADO>();
        internal List<HIS_SERVICE_PATY> ServicePatyCreate { get; set; }
        internal List<HIS_SERVICE_PATY> ServicePatyUpdate { get; set; }
        internal List<long> patientServicePatyError { get; set; }
        V_HIS_MEDICINE_TYPE MedicineType;
        HIS_BRANCH BranchID;
        string newValue = "";

        List<HIS_DEPARTMENT> BlockDepartment__Seleced = new List<HIS_DEPARTMENT>();
        List<long> oldBlockDepartmentIds = null;
        List<HIS_CONTRAINDICATION> contraindicationSelecteds = new List<HIS_CONTRAINDICATION>();
        List<long> oldContraindicationSelecteds = null;
        List<HIS_CONTRAINDICATION> datas = new List<HIS_CONTRAINDICATION>();
        List<HIS_PROCESSING_METHOD> lstPreProcessing = new List<HIS_PROCESSING_METHOD>();
        List<HIS_PROCESSING_METHOD> lstProcessing = new List<HIS_PROCESSING_METHOD>();
        const string timerInitForm = "timerInitForm";
        List<HIS_SOURCE_MEDICINE> listHisSourceMedicine = new List<HIS_SOURCE_MEDICINE>();
        List<AMedicineTypeADO> lstMedicine { get; set; }
        List<AConfigADO> lstConfig { get; set; }
        private bool chkKD = false;
        private bool _isReloadingConfigCombo = false;
        enum ContainerClick
        {
            None,
            TCCL,
            Tutorial,
            Description,
            Contraindication,
            Preprocessing,
            Processing,
            UsedPart,
            DosageForm,
            DistributedAmount,
            ContentWarning,
        }
        HIS_DOSAGE_FORM dosaFormCustom = null;
        ContainerClick currentContainerClick = ContainerClick.None;


        //qtcode
        private List<HIS_DEPA_PATIENT_TYPE> depaPatientTypes = new List<HIS_DEPA_PATIENT_TYPE>();
        public bool isCalledApi = false;
        public bool isClickPick = false;

        #endregion

        #region Contructor
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="_module">module</param>
        /// <param name="medicineTypeId">id loai thuoc</param>
        /// <param name="_delegateSelect">delegate gui doi tuong sang form khac (edit du lieu)</param>
        /// <param name="_delegateRefreshData">reload du lieu form khac</param>
        /// <param name="_actionType">them hay sua du lieu???</param>
        public frmMedicineTypeCreate(Inventec.Desktop.Common.Modules.Module _module, long? medicineTypeId, int _actionType, DelegateSelectData _delegateSelect)
            : base(_module)
        {
            try
            {
                WaitingManager.Show();
                InitializeComponent();
                this.currentMedicineTypeId = medicineTypeId;
                this.module = _module;
                delegateSelect = _delegateSelect;
                // mac dinh la them moi du lieu, neu sua thi gan lai actionType
                if (medicineTypeId != null && medicineTypeId > 0)
                {
                    this.ActionType = _actionType;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmMedicineTypeCreate()
        {
            try
            {
                WaitingManager.Show();
                InitializeComponent();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Private method
        private void frmEditInfoPatient_Load(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath);
                    this.BranchID = HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.Branch;
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
                HisConfigCFG.LoadConfig();
                WaitingManager.Show(this);
                dataDosageForm = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DOSAGE_FORM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                InitUcNational();
                InitComboServiceUnit();
                InitComboOtherPay();
                SetDefaultData();
                SetCaptionByLanguageKey();
                ValidataForm();

                InitCheck(cboBlockDepartment, SelectionGrid__BlockDepartment);
                InitCombo(cboBlockDepartment, BackendDataWorker.Get<HIS_DEPARTMENT>(), "DEPARTMENT_NAME", "ID");

                InitContraindicationCheck();
                InitComboContraindication();

                InitComboPreserveCodition();
                InitComboNguonGoc();
                LoadCboLoaiThuoc();
                LoadCboConfig();


                if (ChkIsSpecificHeinPrice.Checked)
                {
                    btnGiaTran.Enabled = true;
                }
                else
                {
                    btnGiaTran.Enabled = false;
                }
                
                if (this.currentMedicineTypeId != null && this.currentMedicineTypeId > 0 && this.ActionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                {
                    btnDieuChinhLieu.Enabled = true;
                    btnRefresh.Enabled = false;
                    CommonParam param = new CommonParam();
                    //Load Current MedcineType
                    MOS.Filter.HisMedicineTypeViewFilter medicineTypeViewFilter = new MOS.Filter.HisMedicineTypeViewFilter();
                    medicineTypeViewFilter.ID = currentMedicineTypeId;
                    currentVHisMedicineTypeDTODefault = new BackendAdapter(param).Get<List<V_HIS_MEDICINE_TYPE>>(HisRequestUri.HIS_MEDICINE_TYPE_GETVIEW, ApiConsumers.MosConsumer, medicineTypeViewFilter, param).SingleOrDefault();
                    if (currentVHisMedicineTypeDTODefault != null)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(currentVHisMedicineTypeDTODefault.DOSAGE_FORM);
                        Inventec.Common.Logging.LogSystem.Warn(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentVHisMedicineTypeDTODefault), currentVHisMedicineTypeDTODefault));
                        CommonParam pramservice = new CommonParam();
                        HisServiceViewFilter filter = new HisServiceViewFilter();
                        filter.ID = currentVHisMedicineTypeDTODefault.SERVICE_ID;
                        currentVHisServiceDTODefault = new BackendAdapter(pramservice).Get<List<V_HIS_SERVICE>>("api/HisService/GetView", ApiConsumers.MosConsumer, filter, pramservice).SingleOrDefault();
                        rdoUpdateAll.ReadOnly = false;
                        rdoUpdateNotFee.ReadOnly = false;
                        rdoUpdateNotFee.CheckState = CheckState.Checked;
                        btnEditInfo.Enabled = true;
                        FillDataMedicineTypeToControl(currentVHisMedicineTypeDTODefault, currentVHisServiceDTODefault);
                        btnSave.Enabled = (currentVHisMedicineTypeDTODefault.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                    }
                    else
                    {
                        SetNullSpinControl();
                    }
                }
                else
                {
                    SetNullSpinControl();
                }
                if (this.ActionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                {
                    rdoWarning.Checked = true;
                    rdoWarning1.Checked = true;
                    var mediStock = BackendDataWorker.Get<HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == this.module.RoomId);
                }

                FillBlockDepartment();
                FillContraindication();
                FillDataToGridConrolServicePaty();
                RegisterTimer(this.module.ModuleLink, timerInitForm, 1000, timerInitForm_Tick);
                StartTimer(this.module.ModuleLink, timerInitForm);
                if(this.ActionType == GlobalVariables.ActionAdd)
                {
                    btnRefresh_Click(null, null);
                    SetCauHinhFromData(
                        new V_HIS_MEDICINE_TYPE(),
                        new V_HIS_SERVICE()
                    );
                }
                Inventec.Common.Logging.LogAction.Info(this.module.ModuleLink + ": [StartTimer - Load du lieu len cac combobox]");
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboServiceUnit()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceUnitFilter filter = new HisServiceUnitFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_SERVICE_UNIT>>("api/HisServiceUnit/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_UNIT_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(this.cboImpUnit, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<AMedicineTypeADO> getListAMedicineType()
        {
            try
            {
                List<AMedicineTypeADO> listADO = new List<AMedicineTypeADO>()
                {
                    new AMedicineTypeADO { ID = 1,  NAME = "Hóa chất" },
                    new AMedicineTypeADO { ID = 2,  NAME = "Thực phẩm chức năng" },
                    new AMedicineTypeADO { ID = 3,  NAME = "Thuốc dấu sao *" },
                    new AMedicineTypeADO { ID = 4,  NAME = "Generic" },
                    new AMedicineTypeADO { ID = 5,  NAME = "Vaccine" },
                    new AMedicineTypeADO { ID = 6,  NAME = "Vitamin A" },
                    new AMedicineTypeADO { ID = 7,  NAME = "Tiêm chủng mở rộng" },
                    new AMedicineTypeADO { ID = 8,  NAME = "Sinh phẩm" },
                    new AMedicineTypeADO { ID = 9,  NAME = "Ô xy" },
                    new AMedicineTypeADO { ID = 10, NAME = "Gây tê" },
                    new AMedicineTypeADO { ID = 11, NAME = "Biệt dược gốc" },
                    new AMedicineTypeADO { ID = 12, NAME = "Thuốc chạy thận" },
                    new AMedicineTypeADO { ID = 13, NAME = "Nguyên liệu điều chế" }

                };
                return listADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        private List<AConfigADO> getConfigType()
        {
            try
            {
                List<AConfigADO> listADO = new List<AConfigADO>()
                {
                    new AConfigADO { ID = 1,  NAME = "Dừng nhập" },
                    new AConfigADO { ID = 2,  NAME = "Chi phí ngoài gói" },
                    new AConfigADO { ID = 3,  NAME = "Phải nhập hạn sử dụng" },
                    new AConfigADO { ID = 4, NAME = "Cho kê lẻ" },
                    new AConfigADO { ID = 5,  NAME = "Cho xuất lẻ" },
                    new AConfigADO { ID = 6,  NAME = "Tách phần bù" },
                    new AConfigADO { ID = 7,  NAME = "Không bắt buộc số lô, hạn sử dụng" },
                    new AConfigADO { ID = 8,  NAME = "Phải dự trù" },
                    new AConfigADO { ID = 9,  NAME = "Tự động hao phí" },
                    new AConfigADO { ID = 10,  NAME = "Có nguồn chi trả khác" },
                    new AConfigADO { ID = 11, NAME = "Đếm số ngày dùng" },
                    new AConfigADO { ID = 12, NAME = "Không đếm số ngày dùng" },
                    new AConfigADO { ID = 13, NAME = "Ngoài DRG" },
                    new AConfigADO { ID = 14, NAME = "Thuốc ngoại viện" },
                    new AConfigADO { ID = 15, NAME = "Thuốc kinh doanh" },
                    new AConfigADO { ID = 16, NAME = "Thuốc quầy thuốc" }
                };
                return listADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        private List<AConfigADO> getConfigTypeCheckKD()
        {
            try
            {
                List<AConfigADO> listADO = new List<AConfigADO>()
                {
                    new AConfigADO { ID = 1,  NAME = "Dừng nhập" },
                    new AConfigADO { ID = 2,  NAME = "Chi phí ngoài gói" },
                    new AConfigADO { ID = 3,  NAME = "Phải nhập hạn sử dụng" },
                    new AConfigADO { ID = 4, NAME = "Cho kê lẻ" },
                    new AConfigADO { ID = 5,  NAME = "Cho xuất lẻ" },
                    new AConfigADO { ID = 6,  NAME = "Tách phần bù" },
                    new AConfigADO { ID = 7,  NAME = "Không bắt buộc số lô, hạn sử dụng" },
                    new AConfigADO { ID = 8,  NAME = "Phải dự trù" },
                    new AConfigADO { ID = 9,  NAME = "Tự động hao phí" },
                    new AConfigADO { ID = 10,  NAME = "Có nguồn chi trả khác" },
                    new AConfigADO { ID = 11, NAME = "Đếm số ngày dùng" },
                    new AConfigADO { ID = 12, NAME = "Không đếm số ngày dùng" },
                    new AConfigADO { ID = 13, NAME = "Ngoài DRG" },
                    new AConfigADO { ID = 14, NAME = "Thuốc ngoại viện" },
                    new AConfigADO { ID = 15, NAME = "Thuốc kinh doanh" },
                    new AConfigADO { ID = 16, NAME = "Thuốc quầy thuốc" }
                };
                return listADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        private void LoadCboLoaiThuoc()
        {
            try
            {
                List<AMedicineTypeADO> listADO = getListAMedicineType();
                InitComboLoaiThuoc(listADO);
                InitComboLoaiThuocCheck();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCboConfig()
        {
            try
            {
                List<AConfigADO> listADO = getConfigType();
                InitComboConfig(listADO);
                //InitComboConfigCheck();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // Danh sách ID cần disable
        private readonly HashSet<long> disabledIds = new HashSet<long> { 16 };

        private void InitComboConfig(List<AConfigADO> list)
        {
            try
            {
                cboConfig.Properties.View.Columns.Clear();
                cboConfig.Properties.DataSource = null;
                cboConfig.Properties.DataSource = list;
                cboConfig.Properties.DisplayMember = "NAME";
                cboConfig.Properties.ValueMember = "ID";
                cboConfig.Properties.NullText = "";
                cboConfig.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                cboConfig.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                cboConfig.Properties.View.OptionsView.GroupDrawMode = DevExpress.XtraGrid.Views.Grid.GroupDrawMode.Office;
                cboConfig.Properties.View.OptionsView.HeaderFilterButtonShowMode = DevExpress.XtraEditors.Controls.FilterButtonShowMode.SmartTag;
                //cboLoaiThuoc.Properties.View.OptionsView.ShowAutoFilterRow = true;
                cboConfig.Properties.View.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
                cboConfig.Properties.View.OptionsView.ShowDetailButtons = false;
                cboConfig.Properties.View.OptionsView.ShowGroupPanel = false;
                cboConfig.Properties.View.OptionsView.ShowIndicator = false;

                DevExpress.XtraGrid.Columns.GridColumn columnCode = cboConfig.Properties.View.Columns.AddField("NAME");
                columnCode.Caption = "Loại";
                columnCode.Visible = true;
                columnCode.VisibleIndex = 2;
                columnCode.Width = 300;

                //cboLoaiThuoc.Properties.View.OptionsView.ColumnAutoWidth = false;
                //cboLoaiThuoc.Properties.PopupFormSize = new System.Drawing.Size(200, 250);
                //cboLoaiThuoc.Properties.PopupFormMinSize = new System.Drawing.Size(200, 250);
                //column.Caption = "Tất cả";
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboConfig.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(Event_Check_1);
                cboConfig.Properties.Tag = gridCheck;
                cboConfig.Properties.View.OptionsView.ShowColumnHeaders = true;
                cboConfig.Properties.View.OptionsSelection.MultiSelect = true;
                cboConfig.Properties.View.RowStyle += View_RowStyle;
                cboConfig.Properties.View.MouseDown += View_MouseDown;
                cboConfig.Properties.View.CustomColumnDisplayText += View_CustomDisplayText;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void View_CustomDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                var row = view.GetRow(e.ListSourceRowIndex) as AConfigADO;
                if (row == null) return;

                // Đảm bảo text luôn hiển thị cho cột Name
                if (e.Column.FieldName == "Name" || e.Column.FieldName == "CAU_HINH_NAME")
                {
                    e.DisplayText = row.NAME; // Force hiển thị tên
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void View_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                var row = view.GetRow(e.RowHandle) as AConfigADO;
                if (row == null) return;

                // Định nghĩa các ràng buộc
                Dictionary<string, string> dependencies = new Dictionary<string, string>
        {
            {"Thuốc quầy thuốc", "Thuốc kinh doanh"}
        };

                // Kiểm tra từng ràng buộc
                foreach (var dependency in dependencies)
                {
                    string childName = dependency.Key;
                    string parentName = dependency.Value;

                    if (row.NAME == childName)
                    {
                        bool parentChosen = lstConfig.Any(x => x.NAME == parentName);

                        if (!parentChosen)
                        {
                            // Đổi màu toàn bộ row thành xám
                            e.Appearance.ForeColor = System.Drawing.Color.Gray;
                            e.Appearance.BackColor = System.Drawing.Color.WhiteSmoke;
                            break; // Thoát sau khi xử lý
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void View_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                var hitInfo = view.CalcHitInfo(new System.Drawing.Point(e.X, e.Y));

                if (hitInfo.InRow && hitInfo.Column != null)
                {
                    var row = view.GetRow(hitInfo.RowHandle) as AConfigADO;
                    if (row == null) return;

                    // Định nghĩa ràng buộc
                    Dictionary<string, string> dependencies = new Dictionary<string, string>
            {
                {"Thuốc quầy thuốc", "Thuốc kinh doanh"}
            };

                    // Kiểm tra nếu click vào item phụ thuộc
                    if (dependencies.ContainsKey(row.NAME))
                    {
                        string parentName = dependencies[row.NAME];
                        bool parentSelected = lstConfig.Any(x => x.NAME == parentName);

                        if (!parentSelected)
                        {
                            // Chặn sự kiện click - không cho phép chọn
                            ((Control)sender).Capture = false;

                            // Có thể hiện thông báo nếu muốn
                            // MessageBox.Show($"Bạn cần chọn '{parentName}' trước!");

                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboLoaiThuoc(List<AMedicineTypeADO> list)
        {
            try
            {
                cboLoaiThuoc.Properties.DataSource = list;
                cboLoaiThuoc.Properties.DisplayMember = "NAME";
                cboLoaiThuoc.Properties.ValueMember = "ID";
                cboLoaiThuoc.Properties.NullText = "";
                cboLoaiThuoc.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                cboLoaiThuoc.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                cboLoaiThuoc.Properties.View.OptionsView.GroupDrawMode = DevExpress.XtraGrid.Views.Grid.GroupDrawMode.Office;
                cboLoaiThuoc.Properties.View.OptionsView.HeaderFilterButtonShowMode = DevExpress.XtraEditors.Controls.FilterButtonShowMode.SmartTag;
                //cboLoaiThuoc.Properties.View.OptionsView.ShowAutoFilterRow = true;
                cboLoaiThuoc.Properties.View.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
                cboLoaiThuoc.Properties.View.OptionsView.ShowDetailButtons = false;
                cboLoaiThuoc.Properties.View.OptionsView.ShowGroupPanel = false;
                cboLoaiThuoc.Properties.View.OptionsView.ShowIndicator = false;

                DevExpress.XtraGrid.Columns.GridColumn columnCode = cboLoaiThuoc.Properties.View.Columns.AddField("NAME");
                columnCode.Caption = "Loại";
                columnCode.Visible = true;
                columnCode.VisibleIndex = 2;
                columnCode.Width = 200;
                cboLoaiThuoc.Properties.View.OptionsView.ColumnAutoWidth = false;
                cboLoaiThuoc.Properties.View.OptionsView.RowAutoHeight = false;
                cboLoaiThuoc.Properties.PopupFormMinSize = new System.Drawing.Size(200, 250);
                cboLoaiThuoc.Properties.PopupFormSize = new System.Drawing.Size(200, 250);

                //column.Caption = "Tất cả";
                cboLoaiThuoc.Properties.View.OptionsView.ShowColumnHeaders = true;
                cboLoaiThuoc.Properties.View.OptionsSelection.MultiSelect = true;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void InitComboOtherPay()
        {
            try
            {
                CommonParam param = new CommonParam();

                HisOtherPaySourceFilter filter = new HisOtherPaySourceFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var lsTestType = new BackendAdapter(param).Get<List<HIS_OTHER_PAY_SOURCE>>("api/HisOtherPaySource/Get", ApiConsumers.MosConsumer, filter, param);

                List<ColumnInfo> colum = new List<ColumnInfo>();
                colum.Add(new ColumnInfo("OTHER_PAY_SOURCE_CODE", "Mã", 100, 1));
                colum.Add(new ColumnInfo("OTHER_PAY_SOURCE_NAME", "Tên", 250, 2));
                ControlEditorADO controlEditADO = new ControlEditorADO("OTHER_PAY_SOURCE_NAME", "ID", colum, true, 350);
                ControlEditorLoader.Load(cboOTHER_PAY_SOURCE, lsTestType, controlEditADO);
                cboOTHER_PAY_SOURCE.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void InitComboPreserveCodition()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisStorageConditionFilter filter = new HisStorageConditionFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var currentHisStoreCondition = new BackendAdapter(param).Get<List<HIS_STORAGE_CONDITION>>("api/HisStorageCondition/Get", ApiConsumers.MosConsumer, filter, param).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("STORAGE_CONDITION_CODE", "Mã", 100, 1));
                columnInfos.Add(new ColumnInfo("STORAGE_CONDITION_NAME", "Tên", 250, 2));
                columnInfos.Add(new ColumnInfo("FROM_TEMPERATURE", "Nhiệt độ từ", 120, 3));
                columnInfos.Add(new ColumnInfo("TO_TEMPERATURE", "Nhiệt độ đến", 120, 4));
                ControlEditorADO controlEditorADO = new ControlEditorADO("STORAGE_CONDITION_NAME", "ID", columnInfos, false, 590);
                controlEditorADO.ShowHeader = true;
                ControlEditorLoader.Load(cboPreserveCondition, currentHisStoreCondition, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboNguonGoc()
        {
            try
            {
                listHisSourceMedicine = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SOURCE_MEDICINE>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SOURCE_MEDICINE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SOURCE_MEDICINE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SOURCE_MEDICINE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboNguonGoc, listHisSourceMedicine.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList(), controlEditorADO);
                cboNguonGoc.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangeDataComboMedicineType()
        {
            try
            {
                if (ChkIsSpecificHeinPrice.Checked)
                {
                    btnGiaTran.Enabled = true;
                }
                else
                {
                    btnGiaTran.Enabled = false;
                }

                if (this.currentMedicineTypeId != null && this.currentMedicineTypeId > 0 && this.ActionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                {
                    btnRefresh.Enabled = false;
                    CommonParam param = new CommonParam();
                    //Load Current MedcineType
                    MOS.Filter.HisMedicineTypeViewFilter medicineTypeViewFilter = new MOS.Filter.HisMedicineTypeViewFilter();
                    medicineTypeViewFilter.ID = currentMedicineTypeId;
                    currentVHisMedicineTypeDTODefault = new BackendAdapter(param).Get<List<V_HIS_MEDICINE_TYPE>>(HisRequestUri.HIS_MEDICINE_TYPE_GETVIEW, ApiConsumers.MosConsumer, medicineTypeViewFilter, param).SingleOrDefault();
                    if (currentVHisMedicineTypeDTODefault != null)
                    {
                        CommonParam pramservice = new CommonParam();
                        HisServiceViewFilter filter = new HisServiceViewFilter();
                        filter.ID = currentVHisMedicineTypeDTODefault.SERVICE_ID;
                        currentVHisServiceDTODefault = new BackendAdapter(pramservice).Get<List<V_HIS_SERVICE>>("api/HisService/GetView", ApiConsumers.MosConsumer, filter, pramservice).SingleOrDefault();
                        rdoUpdateAll.ReadOnly = false;
                        rdoUpdateNotFee.ReadOnly = false;
                        rdoUpdateNotFee.CheckState = CheckState.Checked;
                        FillDataMedicineTypeToControl(currentVHisMedicineTypeDTODefault, currentVHisServiceDTODefault);
                        btnSave.Enabled = (currentVHisMedicineTypeDTODefault.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                    }
                    else
                    {
                        SetNullSpinControl();
                    }
                }
                else
                {
                    SetNullSpinControl();
                }

                FillDataToGridConrolServicePaty();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void InitUcNational()
        {
            try
            {
                panel.Dock = DockStyle.Fill;
                nationalProcessor = new HIS.UC.National.NationalProcessor();
                HIS.UC.National.ADO.NationalInitADO ado = new HIS.UC.National.ADO.NationalInitADO();
                ado.DelegateNextFocus = NextFocusNational;
                ado.Width = 440;
                ado.Height = 22;
                ado.DataNationals = BackendDataWorker.Get<SDA_NATIONAL>();
                Inventec.Common.Logging.LogSystem.Info("Count national: " + BackendDataWorker.Get<SDA_NATIONAL>().Count());
                ucNational = (UserControl)nationalProcessor.Run(ado);

                if (ucNational != null)
                {
                    this.panel.Controls.Add(ucNational);
                    ucNational.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void NextFocusNational()
        {
            try
            {
                txtManufactureCode.Focus();
                txtManufactureCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetNullSpinControl()
        {
            try
            {
                spinNumOrder.EditValue = null;
                spinUseOnDay.EditValue = null;
                spinUseInDay.EditValue = null;
                spinAgeFrom.EditValue = null;
                spinAgeTo.EditValue = null;
                spinAlertMinInStock.EditValue = null;
                spinImpPrice.EditValue = null;
                spinImpVatRatio.EditValue = null;
                spinInternalPrice.EditValue = null;
                spinLastExpPrice.EditValue = null;
                spinLastExpVatRatio.EditValue = null;
                spinVolume.EditValue = null;
                dtHeinLimitPriceIntrTime.EditValue = null;
                dtHSD.EditValue = null;
                txtDescription.Text = "";
                txtContraindication.Text = "";
                txtRecordingTransaction.Text = "";
                cboGender.EditValue = null;
                cboRank.EditValue = null;
                spinAlertExpiredDate.EditValue = null;
                cboMedicineLine.Properties.Buttons[1].Visible = false;
                cboServiceUnit.Properties.Buttons[1].Visible = false;
                cboMedicineTypeParent.Properties.Buttons[1].Visible = false;
                cboHeinServiceType.Properties.Buttons[1].Visible = false;
                cboMedicineUseForm.Properties.Buttons[1].Visible = false;
                cboManufacture.Properties.Buttons[1].Visible = false;
                cboNguonGoc.Properties.Buttons[1].Visible = false;
                cboDosageForm.Properties.Buttons[1].Visible = false;
                cboHowToUse.Properties.Buttons[1].Visible = false;
                cboGender.Properties.Buttons[1].Visible = false;
                cboImpUnit.EditValue = null;
                spUnitConvertRatio.EditValue = null;
                txtTCCL.Text = "";
                txtTutorial.Text = "";
                txtScientificName.Text = "";
                txtPreprocessing.Text = "";
                txtPreprocessingName.Text = "";
                txtProcessing.Text = "";
                txtProcessingName.Text = "";
                txtUsedPart.Text = "";
                cboDosageForm.EditValue = null;
                cboHowToUse.EditValue = null;
                txtDistributedAmount.Text = "";
                txtOTHER_PAY_SOURCE.Text = "";
                cboOTHER_PAY_SOURCE.EditValue = null;
                txtBhytWhiteListCodes.Text = "";
                txtBhytWhiteListCodes.Enabled = false;
                txtOTHER_PAY_SOURCE.Enabled = false;
                txtContentWarning.Enabled = false;
                btnContentWarning.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DefaultGroup()
        {
            try
            {
                ResetControl(spinImpPrice);
                ResetControl(spinAgeFrom);
                ResetControl(spinAgeTo);
                ResetControl(spinImpVatRatio);
                ResetControl(spinInternalPrice);
                ResetControl(spinLastExpPrice);
                ResetControl(spinLastExpVatRatio);
                ResetControl(txtHeinLimitRatioOld);
                ResetControl(dtHeinLimitPriceIntrTime);
                ResetControl(dtHSD);
                ResetControl(txtHeinLimitRatio);
                ResetControl(chkIsSaleEqualImpPrice);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetControl(BaseEdit control)
        {
            try
            {
                control.EditValue = null;
                control.ResetText();
                dxValidationMedicineType.RemoveControlError(control);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGridConrolServicePaty()
        {
            try
            {
                List<ADO.VHisServicePatyADO> lstHisPaty = new List<ADO.VHisServicePatyADO>();
                grdViewHisServicePaty.BeginUpdate();
                grdControlHisServicePaty.DataSource = null;
                CommonParam param = new CommonParam();
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>> data = null;
                HisServicePatyViewFilter filter = new HisServicePatyViewFilter();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                if (this.resultData != null)
                {
                    filter.SERVICE_ID = resultData.SERVICE_ID;
                    data = new BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>>(
                    "api/HisServicePaty/GetView", ApiConsumers.MosConsumer, filter, param);
                }
                else if (this.currentVHisMedicineTypeDTODefault != null)
                {
                    filter.SERVICE_ID = currentVHisMedicineTypeDTODefault.SERVICE_ID;
                    data = new BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>>(
                   "api/HisServicePaty/GetView", ApiConsumers.MosConsumer, filter, param);
                }

                if (data != null)
                {
                    var Result = (List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>)data.Data;
                    if (Result != null && Result.Count > 0)
                    {
                        int i = 0;
                        foreach (var item in Result)
                        {
                            ADO.VHisServicePatyADO HisServicePaty = new ADO.VHisServicePatyADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<ADO.VHisServicePatyADO>(HisServicePaty, item);
                            if (i == 0)
                            {
                                HisServicePaty.Action = GlobalVariables.ActionAdd;
                            }
                            else
                                HisServicePaty.Action = GlobalVariables.ActionEdit;
                            HisServicePaty.VAT_RATIO = HisServicePaty.VAT_RATIO != null ? HisServicePaty.VAT_RATIO * 100 : 0;
                            //HisServicePaty.BRANCH_ID = this.BranchID;
                            lstHisPaty.Add(HisServicePaty);
                            i++;
                        }

                        if (lstHisPaty != null && lstHisPaty.Count > 0)
                        {
                            this.lsVHisServicePaty = lstHisPaty;
                            grdControlHisServicePaty.DataSource = lstHisPaty;
                        }
                    }
                    else
                    {
                        ADO.VHisServicePatyADO HisServicePaty = new ADO.VHisServicePatyADO();
                        HisServicePaty.Action = GlobalVariables.ActionAdd;
                        HisServicePaty.BRANCH_ID = this.BranchID.ID;
                        HisServicePaty.BRANCH_NAME = this.BranchID.BRANCH_NAME;
                        lstHisPaty.Add(HisServicePaty);
                        grdControlHisServicePaty.DataSource = null;
                        grdControlHisServicePaty.DataSource = lstHisPaty;
                        this.lsVHisServicePaty = lstHisPaty;
                    }
                }
                else
                {
                    if (chkKD)
                    {
                        List<HIS_PATIENT_TYPE> listPatientTypeForSale = new List<HIS_PATIENT_TYPE>();
                        var queryData = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.IS_FOR_SALE_EXP == 1 && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                        if (queryData != null && queryData.Count() > 0)
                        {
                            listPatientTypeForSale = queryData.ToList();
                        }
                        foreach (var item in listPatientTypeForSale)
                        {
                            ADO.VHisServicePatyADO HisServicePaty = new ADO.VHisServicePatyADO();
                            HisServicePaty.Action = GlobalVariables.ActionAdd;
                            HisServicePaty.PATIENT_TYPE_ID = item.ID;
                            HisServicePaty.BRANCH_ID = this.BranchID.ID;
                            HisServicePaty.BRANCH_NAME = this.BranchID.BRANCH_NAME;
                            lstHisPaty.Add(HisServicePaty);
                        }
                        if (lstHisPaty.Count == 0)
                        {
                            ADO.VHisServicePatyADO HisServicePaty = new ADO.VHisServicePatyADO();
                            HisServicePaty.Action = GlobalVariables.ActionAdd;
                            HisServicePaty.BRANCH_ID = this.BranchID.ID;
                            HisServicePaty.BRANCH_NAME = this.BranchID.BRANCH_NAME;
                            lstHisPaty.Add(HisServicePaty);
                        }
                        grdControlHisServicePaty.DataSource = null;
                        grdControlHisServicePaty.DataSource = lstHisPaty;
                        this.lsVHisServicePaty = lstHisPaty;
                    }
                    else
                    {
                        ADO.VHisServicePatyADO HisServicePaty = new ADO.VHisServicePatyADO();
                        HisServicePaty.Action = GlobalVariables.ActionAdd;
                        HisServicePaty.BRANCH_ID = this.BranchID.ID;
                        HisServicePaty.BRANCH_NAME = this.BranchID.BRANCH_NAME;
                        lstHisPaty.Add(HisServicePaty);
                        grdControlHisServicePaty.DataSource = null;
                        grdControlHisServicePaty.DataSource = lstHisPaty;
                        this.lsVHisServicePaty = lstHisPaty;
                    }
                }

                grdViewHisServicePaty.EndUpdate();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void ResetFormData()
        {
            try
            {
                if (!layoutControl6.IsInitialized) return;
                layoutControl6.BeginUpdate();

                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControl6.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                            dxValidationMedicineType.RemoveControlError(fomatFrm);
                            txtMedicineTypeCode.Focus();
                            txtMedicineTypeCode.SelectAll();
                        }
                    }

                    SetNullSpinControl();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    layoutControl6.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataMedicineTypeToControl(V_HIS_MEDICINE_TYPE hIS_MEDICINE_TYPE, V_HIS_SERVICE HIS_Services)
        {
            try
            {
                Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hIS_MEDICINE_TYPE), hIS_MEDICINE_TYPE);
                currentRightClick = HIS_Services;
                cboMedicineType.EditValue = hIS_MEDICINE_TYPE.ID;
                txtMedicineType.Text = hIS_MEDICINE_TYPE.MEDICINE_TYPE_CODE;
                txtMedicineTypeCode.Text = hIS_MEDICINE_TYPE.MEDICINE_TYPE_CODE;
                txtMedicineTypeName.Text = hIS_MEDICINE_TYPE.MEDICINE_TYPE_NAME;
                txtServiceUnitCode.Text = hIS_MEDICINE_TYPE.SERVICE_UNIT_CODE;
                cboServiceUnit.EditValue = hIS_MEDICINE_TYPE.SERVICE_UNIT_ID;
                cboMedicineTypeParent.EditValue = hIS_MEDICINE_TYPE.PARENT_ID;
                cboMedicineLine.EditValue = hIS_MEDICINE_TYPE.MEDICINE_LINE_ID;
                txtManufactureCode.Text = hIS_MEDICINE_TYPE.MANUFACTURER_CODE;
                cboManufacture.EditValue = hIS_MEDICINE_TYPE.MANUFACTURER_ID;
                var sourceMedicine = listHisSourceMedicine.FirstOrDefault(o => o.ID == hIS_MEDICINE_TYPE.SOURCE_MEDICINE);
                if (sourceMedicine != null && sourceMedicine.IS_ACTIVE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    cboNguonGoc.Properties.DataSource = listHisSourceMedicine.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE || o.ID == hIS_MEDICINE_TYPE.SOURCE_MEDICINE);
                cboNguonGoc.EditValue = hIS_MEDICINE_TYPE.SOURCE_MEDICINE;
                if (dataDosageForm == null || dataDosageForm.Count == 0)
                {
                    dataDosageForm = new List<HIS_DOSAGE_FORM>();
                }
                var data = dataDosageForm.FirstOrDefault(o => o.DOSAGE_FORM_NAME.Equals(hIS_MEDICINE_TYPE.DOSAGE_FORM ?? ""));
                if (data != null) //Edit
                    cboDosageForm.EditValue = data.DOSAGE_FORM_NAME;
                else if (!string.IsNullOrEmpty(hIS_MEDICINE_TYPE.DOSAGE_FORM))
                {
                    dosaFormCustom = new HIS_DOSAGE_FORM() { ID = -1, DOSAGE_FORM_NAME = hIS_MEDICINE_TYPE.DOSAGE_FORM };
                    dataDosageForm.Add(dosaFormCustom);
                    InitDosageForm();
                    cboDosageForm.EditValue = hIS_MEDICINE_TYPE.DOSAGE_FORM;
                }

                cboHowToUse.EditValue = hIS_MEDICINE_TYPE.HTU_ID;
                spinImpPrice.EditValue = hIS_MEDICINE_TYPE.IMP_PRICE;
                spinImpVatRatio.EditValue = hIS_MEDICINE_TYPE.IMP_VAT_RATIO != null ? hIS_MEDICINE_TYPE.IMP_VAT_RATIO * 100 : null;

                spinInternalPrice.EditValue = hIS_MEDICINE_TYPE.INTERNAL_PRICE;
                spinLastExpPrice.EditValue = hIS_MEDICINE_TYPE.LAST_EXP_PRICE;
                spinLastExpVatRatio.EditValue = hIS_MEDICINE_TYPE.LAST_EXP_VAT_RATIO != null ? hIS_MEDICINE_TYPE.LAST_EXP_VAT_RATIO * 100 : null;
                txtConcentra.Text = hIS_MEDICINE_TYPE.CONCENTRA;
                txtPackingTypeCode.Text = hIS_MEDICINE_TYPE.PACKING_TYPE_NAME;
                txtMedicineUseFormCode.Text = hIS_MEDICINE_TYPE.MEDICINE_USE_FORM_CODE;
                cboMedicineUseForm.EditValue = hIS_MEDICINE_TYPE.MEDICINE_USE_FORM_ID;
                spinVolume.EditValue = hIS_MEDICINE_TYPE.VOLUME;

                

                

                if (hIS_MEDICINE_TYPE.IS_SALE_EQUAL_IMP_PRICE == 1)
                {
                    chkIsSaleEqualImpPrice.CheckState = CheckState.Checked;
                }
                else
                {
                    chkIsSaleEqualImpPrice.CheckState = CheckState.Unchecked;
                }
                if (hIS_MEDICINE_TYPE.IS_BLOCK_MAX_IN_PRESCRIPTION == 1)
                {
                    rdoBlock.Checked = true;
                }
                else
                {
                    rdoWarning.Checked = true;
                }
                if (hIS_MEDICINE_TYPE.IS_BLOCK_MAX_IN_DAY == 1)
                {
                    rdoBlock1.Checked = true;
                }
                else
                {
                    rdoWarning1.Checked = true;
                }               

                

                if (hIS_MEDICINE_TYPE.IS_NO_HEIN_LIMIT_FOR_SPECIAL == 1)
                {
                    chkIsNoHeinLimitForSpecial.CheckState = CheckState.Checked;
                }
                else
                {
                    chkIsNoHeinLimitForSpecial.CheckState = CheckState.Unchecked;
                }

                if (HIS_Services.IS_SPECIFIC_HEIN_PRICE == 1)
                {
                    ChkIsSpecificHeinPrice.CheckState = CheckState.Checked;
                }
                else
                {
                    ChkIsSpecificHeinPrice.CheckState = CheckState.Unchecked;
                }
                cboMedicineGroup.EditValue = hIS_MEDICINE_TYPE.MEDICINE_GROUP_ID;
                spinUseOnDay.EditValue = hIS_MEDICINE_TYPE.ALERT_MAX_IN_PRESCRIPTION;
                spinUseInDay.EditValue = hIS_MEDICINE_TYPE.ALERT_MAX_IN_DAY;
                spinAgeFrom.EditValue = HIS_Services.AGE_FROM;
                spinAgeTo.EditValue = HIS_Services.AGE_TO;
                txtActiveIngrBhytCode.Text = hIS_MEDICINE_TYPE.ACTIVE_INGR_BHYT_CODE;
                txtActiveIngrBhytName.Text = hIS_MEDICINE_TYPE.ACTIVE_INGR_BHYT_NAME;
                txtHeinLimitRatioOld.Text = hIS_MEDICINE_TYPE.HEIN_LIMIT_RATIO_OLD != null ? string.Format("{0:0.####}", (hIS_MEDICINE_TYPE.HEIN_LIMIT_RATIO_OLD ?? 0) * 100) : "";
                txtHeinLimitRatio.Text = hIS_MEDICINE_TYPE.HEIN_LIMIT_RATIO != null ? string.Format("{0:0.####}", (hIS_MEDICINE_TYPE.HEIN_LIMIT_RATIO ?? 0) * 100) : "";
                spinAlertMinInStock.EditValue = hIS_MEDICINE_TYPE.ALERT_MIN_IN_STOCK;
                spinAlertExpiredDate.EditValue = hIS_MEDICINE_TYPE.ALERT_EXPIRED_DATE;
                spinNumOrder.EditValue = hIS_MEDICINE_TYPE.NUM_ORDER;
                txtHeinServiceBhytName.Text = hIS_MEDICINE_TYPE.HEIN_SERVICE_BHYT_NAME;
                txtTCCL.Text = hIS_MEDICINE_TYPE.QUALITY_STANDARDS;
                txtTutorial.Text = hIS_MEDICINE_TYPE.TUTORIAL;
                txtDescription.Text = hIS_MEDICINE_TYPE.DESCRIPTION;
                txtContentWarning.Text = hIS_MEDICINE_TYPE.ODD_WARNING_CONTENT;
                txtContraindication.Text = hIS_MEDICINE_TYPE.CONTRAINDICATION;

                cboImpUnit.EditValue = hIS_MEDICINE_TYPE.IMP_UNIT_ID ?? null;
                if (cboImpUnit.EditValue != null)
                    spUnitConvertRatio.Value = hIS_MEDICINE_TYPE.IMP_UNIT_CONVERT_RATIO ?? 1;
                else
                    spUnitConvertRatio.EditValue = null;
                cboOTHER_PAY_SOURCE.EditValue = HIS_Services.OTHER_PAY_SOURCE_ID;
                txtOTHER_PAY_SOURCE.Text = HIS_Services.OTHER_PAY_SOURCE_ICDS;
                if (cboOTHER_PAY_SOURCE.EditValue == null)
                {
                    txtOTHER_PAY_SOURCE.Enabled = false;
                }
                txtBhytWhiteListCodes.Text = HIS_Services.BHYT_WHITELIST_CODES;
                if (cboOTHER_PAY_SOURCE.EditValue == null)
                {
                    txtBhytWhiteListCodes.Enabled = false;
                }
                txtRecordingTransaction.Text = hIS_MEDICINE_TYPE.RECORDING_TRANSACTION;
                txtBytNumOrder.Text = hIS_MEDICINE_TYPE.BYT_NUM_ORDER;
                txtRegisterNumber.Text = hIS_MEDICINE_TYPE.REGISTER_NUMBER;
                txtTcyNumOrder.Text = hIS_MEDICINE_TYPE.TCY_NUM_ORDER;
                txtSttTT20.Text = hIS_MEDICINE_TYPE.NUM_ORDER_CIRCULARS20;
                dtHeinLimitPriceIntrTime.EditValue = hIS_MEDICINE_TYPE.HEIN_LIMIT_PRICE_INTR_TIME != null ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hIS_MEDICINE_TYPE.HEIN_LIMIT_PRICE_INTR_TIME ?? 0) : null;
                dtHSD.EditValue = hIS_MEDICINE_TYPE.LAST_EXPIRED_DATE != null ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hIS_MEDICINE_TYPE.LAST_EXPIRED_DATE ?? 0) : null;
                cboPreserveCondition.EditValue = hIS_MEDICINE_TYPE.STORAGE_CONDITION_ID;
                cboNguonGoc.EditValue = hIS_MEDICINE_TYPE.SOURCE_MEDICINE;
                cboContraindication.EditValue = hIS_MEDICINE_TYPE.CONTRAINDICATION_IDS;

                var medicineTypeParent = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE>().SingleOrDefault(o => o.ID == hIS_MEDICINE_TYPE.PARENT_ID);
                txtMedicineTypeParentCode.Text = medicineTypeParent != null ? medicineTypeParent.MEDICINE_TYPE_CODE : "";
                HIS.UC.National.ADO.NationalInputADO ado = new UC.National.ADO.NationalInputADO();

                var national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().FirstOrDefault(o => o.NATIONAL_NAME == hIS_MEDICINE_TYPE.NATIONAL_NAME);
                if (national != null)
                {
                    if (nationalProcessor != null && ucNational != null)
                    {
                        ado.NATIONAL_CODE = national.NATIONAL_CODE;
                        ado.NATIONAL_NAME = national.NATIONAL_NAME;
                        nationalProcessor.SetValue(ucNational, ado);
                    }
                }
                else
                {
                    ado.NATIONAL_NAME = hIS_MEDICINE_TYPE.NATIONAL_NAME;
                    nationalProcessor.SetValue(ucNational, ado);
                }



                if (hIS_MEDICINE_TYPE.HEIN_SERVICE_TYPE_ID != null)
                {
                    cboHeinServiceType.EditValue = hIS_MEDICINE_TYPE.HEIN_SERVICE_TYPE_ID;
                }
                else
                {
                    cboHeinServiceType.EditValue = null;
                }

                #region thuoc
                if (cboLoaiThuoc.Properties.DataSource == null)
                {
                    List<AMedicineTypeADO> listADO = getListAMedicineType();
                    cboLoaiThuoc.Properties.DataSource = listADO;
                }
                GridCheckMarksSelection gridCheckChiSo = cboLoaiThuoc.Properties.Tag as GridCheckMarksSelection;
                var arr = new List<string>();

                if (hIS_MEDICINE_TYPE.IS_VACCINE == 1)
                    arr.Add("Vaccin");

                if (hIS_MEDICINE_TYPE.IS_CHEMICAL_SUBSTANCE == 1)
                    arr.Add("Hóa chất");

                if (hIS_MEDICINE_TYPE.IS_OXYGEN == 1)
                    arr.Add("Ô xy");

                if (hIS_MEDICINE_TYPE.IS_GENERIC == 1)
                    arr.Add("Generic");

                if (hIS_MEDICINE_TYPE.IS_FUNCTIONAL_FOOD == 1)
                    arr.Add("TPCN");

                if (hIS_MEDICINE_TYPE.IS_VITAMIN_A == 1)
                    arr.Add("Vitamin A");

                if (hIS_MEDICINE_TYPE.IS_TCMR == 1)
                    arr.Add("TCMR");

                if (hIS_MEDICINE_TYPE.IS_BIOLOGIC == 1)
                    arr.Add("Sinh phẩm");

                if (hIS_MEDICINE_TYPE.IS_ANAESTHESIA == 1)
                    arr.Add("Gây tê");

                if (hIS_MEDICINE_TYPE.IS_ORIGINAL_BRAND_NAME == 1)
                    arr.Add("Biệt dược gốc");

                if (hIS_MEDICINE_TYPE.IS_STAR_MARK == 1)
                    arr.Add("Thuốc dấu *");

                if (hIS_MEDICINE_TYPE.IS_KIDNEY == 1)
                    arr.Add("Thuốc chạy thận");

                if (hIS_MEDICINE_TYPE.IS_RAW_MEDICINE == 1)
                    arr.Add("NLBC");

                string listArrayMedicine = string.Join(", ", arr);

                ProcessSelectBS(listArrayMedicine, gridCheckChiSo);
                #endregion

                if (cboConfig.Properties.DataSource == null)
                {
                    List<AConfigADO> listADO = getConfigType();
                    cboConfig.Properties.DataSource = listADO;
                }

                SetCauHinhFromData(hIS_MEDICINE_TYPE, HIS_Services);


                //#region config
                //GridCheckMarksSelection gridCheckConfig = cboConfig.Properties.Tag as GridCheckMarksSelection;
                //var cArr = new List<string>();

                //if (hIS_MEDICINE_TYPE.IS_STOP_IMP == 1)
                //    cArr.Add("Dừng nhập");

                //if (hIS_MEDICINE_TYPE.IS_OUT_PARENT_FEE == 1)
                //    cArr.Add("Chi phí ngoài gói");

                //if (hIS_MEDICINE_TYPE.IS_REQUIRE_HSD == 1)
                //    cArr.Add("Phải nhập hạn sử dụng");

                //if (hIS_MEDICINE_TYPE.IS_ALLOW_EXPORT_ODD == 1)
                //    cArr.Add("Cho xuất lẻ");

                //if (hIS_MEDICINE_TYPE.IS_SPLIT_COMPENSATION == 1)
                //    cArr.Add("Tách phần bù");

                //if (hIS_MEDICINE_TYPE.ALLOW_MISSING_PKG_INFO == 1)
                //    cArr.Add("Không bắt buộc số lô, hạn sử dụng");

                //if (hIS_MEDICINE_TYPE.IS_MUST_PREPARE == 1)
                //    cArr.Add("Phải dự trù");

                //if (hIS_MEDICINE_TYPE.IS_AUTO_EXPEND == 1)
                //    cArr.Add("Tự động hao phí");

                //if (hIS_MEDICINE_TYPE.IS_OTHER_SOURCE_PAID == 1)
                //    cArr.Add("Có nguồn chi trả khác");

                //if (hIS_MEDICINE_TYPE.IS_TREATMENT_DAY_COUNT == 1)
                //    cArr.Add("Đếm số ngày dùng");

                //if (hIS_MEDICINE_TYPE.IS_NOT_TREATMENT_DAY_COUNT == 1)
                //    cArr.Add("Không đếm số ngày dùng");

                //if (HIS_Services.IS_OUT_OF_DRG == 1)
                //    cArr.Add("Ngoài DRG");

                //if (hIS_MEDICINE_TYPE.IS_OUT_HOSPITAL == 1)
                //    cArr.Add("Thuốc ngoại viện");

                //if (hIS_MEDICINE_TYPE.IS_BUSINESS == 1)
                //    cArr.Add("Thuốc kinh doanh");

                //if (hIS_MEDICINE_TYPE.IS_ALLOW_ODD == 1)
                //    cArr.Add("Cho kê lẻ");
                //if(hIS_MEDICINE_TYPE.IS_DRUG_STORE == 1)
                //    cArr.Add("Thuốc quầy thuốc");
                //string listArrayConfig = string.Join(", ", cArr);

                //ProcessSelectConfig(listArrayConfig, gridCheckConfig);
                //#endregion


                if (hIS_MEDICINE_TYPE.TDL_GENDER_ID != null)
                {
                    var gender = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.ID == hIS_MEDICINE_TYPE.TDL_GENDER_ID);
                    if (gender != null)
                    {
                        cboGender.EditValue = gender.ID;
                    }
                }
                else
                {
                    cboGender.EditValue = null;
                }

                if (hIS_MEDICINE_TYPE.RANK != null)
                {
                    cboRank.EditValue = hIS_MEDICINE_TYPE.RANK;
                }
                else
                {
                    cboRank.EditValue = null;
                }



                txtMedicineNationalCode.Text = hIS_MEDICINE_TYPE.MEDICINE_NATIONAL_CODE ?? "";



                ReloadActiveIngredientData();
                if (!String.IsNullOrEmpty(hIS_MEDICINE_TYPE.ATC_CODES))
                {
                    ReloadATCData(hIS_MEDICINE_TYPE.ATC_CODES);
                }
                else
                {
                    this.txtACT_Code.Text = "";
                    this.txtACT_Name.Text = "";
                }
                
                if (txtMedicineType.Enabled)
                {
                    txtMedicineType.Focus();
                    txtMedicineType.SelectAll();
                }
                else
                {
                    txtMedicineTypeCode.Focus();
                    txtMedicineTypeCode.SelectAll();
                }



                this.txtScientificName.Text = hIS_MEDICINE_TYPE.SCIENTIFIC_NAME;
                this.txtPreprocessing.Text = hIS_MEDICINE_TYPE.PREPROCESSING_CODE;
                //this.txtPreprocessingName.Text = hIS_MEDICINE_TYPE.PREPROCESSING;
                //HUANNH
                InitProcessingMethod();
                if (!string.IsNullOrEmpty(hIS_MEDICINE_TYPE.PREPROCESSING_CODE) && lstPreProcessing != null)
                {
                    var codes = hIS_MEDICINE_TYPE.PREPROCESSING_CODE.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    var names = lstPreProcessing
                        .Where(x => codes.Contains(x.PROCESSING_METHOD_CODE))
                        .Select(x => x.PROCESSING_METHOD_NAME)
                        .ToList();
                    this.txtPreprocessingName.Text = string.Join(";", names);
                }
                else
                {
                    this.txtPreprocessingName.Text = "";
                }
                
                this.txtProcessing.Text = hIS_MEDICINE_TYPE.PROCESSING_CODE;
                if (!string.IsNullOrEmpty(hIS_MEDICINE_TYPE.PROCESSING_CODE) && lstProcessing != null)
                {
                    var codes = hIS_MEDICINE_TYPE.PROCESSING_CODE.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    var names = lstProcessing
                        .Where(x => codes.Contains(x.PROCESSING_METHOD_CODE))
                        .Select(x => x.PROCESSING_METHOD_NAME)
                        .ToList();
                    this.txtProcessingName.Text = string.Join(";", names);
                }
                else
                {
                    this.txtProcessingName.Text = "";
                }
                this.txtPreprocessing.Text = hIS_MEDICINE_TYPE.PREPROCESSING_CODE;
                this.txtUsedPart.Text = hIS_MEDICINE_TYPE.USED_PART;
                //this.cboDosageForm.EditValue = hIS_MEDICINE_TYPE.DOSAGE_FORM;
                this.cboHowToUse.EditValue = hIS_MEDICINE_TYPE.HTU_ID;
                this.txtDistributedAmount.Text = hIS_MEDICINE_TYPE.DISTRIBUTED_AMOUNT;
                spinUseInTreat.EditValue = hIS_MEDICINE_TYPE.ALERT_MAX_IN_TREATMENT;
                this.UseInTreat = hIS_MEDICINE_TYPE.ALERT_MAX_IN_TREATMENT;
                chkWarningInTreat.Checked = hIS_MEDICINE_TYPE.IS_BLOCK_MAX_IN_TREATMENT == 1 ? false : true;
                chkBlockInTreat.Checked = hIS_MEDICINE_TYPE.IS_BLOCK_MAX_IN_TREATMENT != 1 ? false : true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCauHinhFromData(V_HIS_MEDICINE_TYPE hIS_MEDICINE_TYPE, V_HIS_SERVICE hIS_Services)
        {
            try
            {                
                var gridCheck = cboConfig.Properties.Tag as GridCheckMarksSelection;
                
                if (gridCheck != null)
                {
                    gridCheck.SelectionChanged -= Event_Check_1; // Tạm bỏ event
                }

                var allItems = cboConfig.Properties.DataSource as List<AConfigADO>;
                var gridView = cboConfig.Properties.View as GridView;

                if (allItems == null || gridCheck == null) return;
                lstConfig = new List<AConfigADO>();
                // Clear và add
                lstConfig.Clear();
                gridCheck.ClearSelection(gridView);

                // Add items
                if (hIS_MEDICINE_TYPE.IS_STOP_IMP == 1)
                {
                    var item = allItems.FirstOrDefault(x => x.NAME == "Dừng nhập");
                    if (item != null)
                    {
                        gridCheck.Selection.Add(item);
                        lstConfig.Add(item);
                    }
                }

                if (hIS_MEDICINE_TYPE.IS_OUT_PARENT_FEE == 1)
                {
                    var item = allItems.FirstOrDefault(x => x.NAME == "Chi phí ngoài gói");
                    if (item != null)
                    {
                        gridCheck.Selection.Add(item);
                        lstConfig.Add(item);
                    }
                }

                if (hIS_MEDICINE_TYPE.IS_REQUIRE_HSD == 1)
                {
                    var item = allItems.FirstOrDefault(x => x.NAME == "Phải nhập hạn sử dụng");
                    if (item != null)
                    {
                        gridCheck.Selection.Add(item);
                        lstConfig.Add(item);
                    }
                }

                if (hIS_MEDICINE_TYPE.IS_ALLOW_EXPORT_ODD == 1)
                {
                    var item = allItems.FirstOrDefault(x => x.NAME == "Cho xuất lẻ");
                    if (item != null)
                    {
                        gridCheck.Selection.Add(item);
                        lstConfig.Add(item);
                    }
                }

                if (hIS_MEDICINE_TYPE.IS_SPLIT_COMPENSATION == 1)
                {
                    var item = allItems.FirstOrDefault(x => x.NAME == "Tách phần bù");
                    if (item != null)
                    {
                        gridCheck.Selection.Add(item);
                        lstConfig.Add(item);
                    }
                }

                if (hIS_MEDICINE_TYPE.ALLOW_MISSING_PKG_INFO == 1)
                {
                    var item = allItems.FirstOrDefault(x => x.NAME == "Không bắt buộc số lô, hạn sử dụng");
                    if (item != null)
                    {
                        gridCheck.Selection.Add(item);
                        lstConfig.Add(item);
                    }
                }

                if (hIS_MEDICINE_TYPE.IS_MUST_PREPARE == 1)
                {
                    var item = allItems.FirstOrDefault(x => x.NAME == "Phải dự trù");
                    if (item != null)
                    {
                        gridCheck.Selection.Add(item);
                        lstConfig.Add(item);
                    }
                }

                if (hIS_MEDICINE_TYPE.IS_AUTO_EXPEND == 1)
                {
                    var item = allItems.FirstOrDefault(x => x.NAME == "Tự động hao phí");
                    if (item != null)
                    {
                        gridCheck.Selection.Add(item);
                        lstConfig.Add(item);
                    }
                }

                if (hIS_MEDICINE_TYPE.IS_TREATMENT_DAY_COUNT == 1)
                {
                    var item = allItems.FirstOrDefault(x => x.NAME == "Đếm số ngày dùng");
                    if (item != null)
                    {
                        gridCheck.Selection.Add(item);
                        lstConfig.Add(item);
                    }
                }

                if (hIS_MEDICINE_TYPE.IS_OTHER_SOURCE_PAID == 1)
                {
                    var item = allItems.FirstOrDefault(x => x.NAME == "Có nguồn chi trả khác");
                    if (item != null)
                    {
                        gridCheck.Selection.Add(item);
                        lstConfig.Add(item);
                    }
                }

                if (hIS_MEDICINE_TYPE.IS_NOT_TREATMENT_DAY_COUNT == 1)
                {
                    var item = allItems.FirstOrDefault(x => x.NAME == "Không đếm số ngày dùng");
                    if (item != null)
                    {
                        gridCheck.Selection.Add(item);
                        lstConfig.Add(item);
                    }
                }

                if (hIS_Services.IS_OUT_OF_DRG == 1)
                {
                    var item = allItems.FirstOrDefault(x => x.NAME == "Ngoài DRG");
                    if (item != null)
                    {
                        gridCheck.Selection.Add(item);
                        lstConfig.Add(item);
                    }
                }

                if (hIS_MEDICINE_TYPE.IS_OUT_HOSPITAL == 1)
                {
                    var item = allItems.FirstOrDefault(x => x.NAME == "Thuốc ngoại viện");
                    if (item != null)
                    {
                        gridCheck.Selection.Add(item);
                        lstConfig.Add(item);
                    }
                }

                var mediStock = BackendDataWorker.Get<HIS_MEDI_STOCK>()
    .FirstOrDefault(o => o.ROOM_ID == this.module.RoomId);

                bool isBusiness =
                    (mediStock?.IS_BUSINESS == 1) ||
                    (hIS_MEDICINE_TYPE.IS_BUSINESS == 1);

                if (isBusiness)
                {
                    var item = allItems.FirstOrDefault(x => x.NAME == "Thuốc kinh doanh");
                    if (item != null && !lstConfig.Any(x => x.ID == item.ID))
                    {
                        gridCheck.Selection.Add(item);
                        lstConfig.Add(item);
                    }
                }


                if (hIS_MEDICINE_TYPE.IS_ALLOW_ODD == 1)
                {
                    var item = allItems.FirstOrDefault(x => x.NAME == "Cho kê lẻ");
                    if (item != null)
                    {
                        gridCheck.Selection.Add(item);
                        lstConfig.Add(item);
                    }
                }

                if (hIS_MEDICINE_TYPE.IS_DRUG_STORE == 1)
                {
                    var item = allItems.FirstOrDefault(x => x.NAME == "Thuốc quầy thuốc");
                    if (item != null)
                    {
                        gridCheck.Selection.Add(item);
                        lstConfig.Add(item);
                    }
                }

                // Update UI manually
                cboConfig.EditValue = lstConfig;
                cboConfig.Text = string.Join(", ", lstConfig.Select(s => s.NAME));

                // Re-enable event
                if (gridCheck != null)
                {
                    gridCheck.SelectionChanged += Event_Check_1; // Đăng ký lại event
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReloadATCData(String data)
        {
            try
            {
                this.Acts = new List<HIS_ATC>();
                string[] arls = data.Split(new char[] { ',' });
                if (arls != null && arls.Count() > 0)
                {
                    var lsdataATC = BackendDataWorker.Get<HIS_ATC>().ToList();
                    for (int i = 0; i < arls.Count(); i++)
                    {
                        if (lsdataATC != null)
                        {
                            var dataATC = lsdataATC.FirstOrDefault(o => o.ATC_CODE.ToUpper().Trim() == arls[i].ToUpper().Trim());
                            this.Acts.Add(dataATC);
                        }
                    }
                    if (this.Acts != null)
                    {
                        this.txtACT_Code.Text = string.Join(",", Acts.Select(o => o.ATC_CODE).ToList());
                        this.txtACT_Name.Text = string.Join(",", Acts.Select(o => o.ATC_NAME).ToList());

                        this.txtATCGroup_Code.Text = "";
                        this.txtATCGroup_Name.Text = "";
                        ActGroups = BackendDataWorker.Get<HIS_ATC_GROUP>().ToList();
                        var atcCodes = this.Acts.Select(o => o.ATC_CODE.Length > 5 ? o.ATC_CODE.Substring(0, 5) : o.ATC_CODE).ToList();
                        if (ActGroups != null && ActGroups.Count > 0 && atcCodes != null && atcCodes.Count > 0)
                        {
                            var atcGroups = ActGroups.Where(o => atcCodes.Contains(o.ATC_GROUP_CODE)).ToList();
                            if (atcGroups != null && atcGroups.Count > 0)
                            {
                                this.txtATCGroup_Code.Text = string.Join(",", atcGroups.Select(o => o.ATC_GROUP_CODE).ToList());
                                this.txtATCGroup_Name.Text = string.Join(",", atcGroups.Select(o => o.ATC_GROUP_NAME).ToList());
                            }
                        }
                        else if (HisConfigCFG.AtcCodeOverlarWarningOption == "2" && atcCodes != null && atcCodes.Count > 0)
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Các mã ATC sau chưa có nhóm ATC: {0}. Bạn có muốn thêm nhóm ATC không?", String.Join(",", atcCodes)), "Thông báo", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                            {
                                List<Object> listArgs = new List<object>();
                                if (this.module == null)
                                {
                                    CallModule callModule = new CallModule(CallModule.HisAtcGroup, 0, 0, listArgs);
                                }
                                else
                                {
                                    CallModule callModule = new CallModule(CallModule.HisAtcGroup, this.module.RoomId, this.module.RoomTypeId, listArgs);
                                }
                                ActGroups = BackendDataWorker.Get<HIS_ATC_GROUP>().ToList();
                                if (ActGroups != null && ActGroups.Count > 0 && atcCodes != null && atcCodes.Count > 0)
                                {
                                    var atcGroups = ActGroups.Where(o => atcCodes.Contains(o.ATC_GROUP_CODE)).ToList();
                                    if (atcGroups != null && atcGroups.Count > 0)
                                    {
                                        this.txtATCGroup_Code.Text = string.Join(",", atcGroups.Select(o => o.ATC_GROUP_CODE).ToList());
                                        this.txtATCGroup_Name.Text = string.Join(",", atcGroups.Select(o => o.ATC_GROUP_NAME).ToList());
                                    }
                                }
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

        private void ReloadATCGroupData(String data)
        {
            try
            {
                this.ActGroups = new List<HIS_ATC_GROUP>();
                string[] arls = data.Split(new char[] { ',' });
                if (arls != null && arls.Count() > 0)
                {
                    var lsdataATCGroup = BackendDataWorker.Get<HIS_ATC_GROUP>().ToList();
                    for (int i = 0; i < arls.Count(); i++)
                    {
                        if (lsdataATCGroup != null)
                        {
                            var dataATC = lsdataATCGroup.FirstOrDefault(o => o.ATC_GROUP_CODE.ToUpper().Trim() == arls[i].ToUpper().Trim());
                            this.ActGroups.Add(dataATC);
                        }
                    }
                    if (this.ActGroups != null)
                    {
                        this.txtATCGroup_Code.Text = string.Join(",", ActGroups.Select(o => o.ATC_GROUP_CODE).ToList());
                        this.txtATCGroup_Name.Text = string.Join(",", ActGroups.Select(o => o.ATC_GROUP_NAME).ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ReloadActiveIngredientData()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisMedicineTypeAcinViewFilter medicineTypeAcinFilter = new HisMedicineTypeAcinViewFilter();
                medicineTypeAcinFilter.MEDICINE_TYPE_ID = this.currentMedicineTypeId;
                var medicineTypeAcin = new BackendAdapter(param).Get<List<V_HIS_MEDICINE_TYPE_ACIN>>(ApiConsumer.HisRequestUriStore.HIS_MEDICINE_TYPE_ACIN_GETVIEW, ApiConsumers.MosConsumer, medicineTypeAcinFilter, param);
                if (medicineTypeAcin != null)
                {
                    this.activeIngredients = medicineTypeAcin.GroupBy(p => p.ACTIVE_INGREDIENT_ID).Select(o => new HIS_ACTIVE_INGREDIENT() { ID = o.First().ACTIVE_INGREDIENT_ID, ACTIVE_INGREDIENT_CODE = o.First().ACTIVE_INGREDIENT_CODE, ACTIVE_INGREDIENT_NAME = o.First().ACTIVE_INGREDIENT_NAME }).ToList();
                    this.txtActiveIngredientCode.Text = string.Join(" + ", activeIngredients.Select(o => o.ACTIVE_INGREDIENT_CODE).ToList());
                    this.txtActiveIngredientName.Text = string.Join(" + ", activeIngredients.Select(o => o.ACTIVE_INGREDIENT_NAME).ToList());
                }
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
                spUnitConvertRatio.Enabled = false;
                btnEditInfo.Enabled = false;
                if (this.ActionType == GlobalVariables.ActionEdit)
                {
                    txtMedicineType.Enabled = true;
                    cboMedicineType.Enabled = true;
                }
                else
                {
                    txtMedicineType.Enabled = false;
                    cboMedicineType.Enabled = false;
                }
                chkWarningInTreat.Checked = true;
                spinUseInTreat.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        //abc
        /// <summary>
        /// Load giữ liệu lên các combobox
        /// </summary>
        public void FillDataToControlsForm()
        {
            try
            {
                InitDosageForm();
                InitProcessingMethod();
                InitServiceUnit();
                InitMedicineTypeParent();
                InitMedicineLine();
                InitManufacture();
                InitMedicineGroup();
                InitMedincineUseForm();
                InitHeinServiceType();
                InitComboGender();
                InitRank();
                LoadDatatoComboPatientType();
                InitMedicineType();
                InitHowToUse();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        private void InitDosageForm()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DOSAGE_FORM_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("DOSAGE_FORM_NAME", "", 100, 2));
                var a = HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer;
                ControlEditorADO controlEditorADO = new ControlEditorADO("DOSAGE_FORM_NAME", "DOSAGE_FORM_NAME", columnInfos, false, 150);
                ControlEditorLoader.Load(cboDosageForm, dataDosageForm, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillBlockDepartment()
        {
            try
            {
                if (this.currentMedicineTypeId.HasValue && this.currentMedicineTypeId.Value > 0 && this.ActionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                {
                    HisMestMetyDepaFilter filter = new HisMestMetyDepaFilter();
                    filter.MEDICINE_TYPE_ID = this.currentMedicineTypeId.Value;
                    filter.HAS_MEDI_STOCK_ID = false;

                    List<HIS_MEST_METY_DEPA> matyDepas = new BackendAdapter(new CommonParam()).Get<List<HIS_MEST_METY_DEPA>>("api/HisMestMetyDepa/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                    this.oldBlockDepartmentIds = matyDepas != null ? matyDepas.Where(o => o.IS_JUST_PRESCRIPTION != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Select(s => s.DEPARTMENT_ID).Distinct().ToList() : null;
                }

                GridCheckMarksSelection gridCheckMark = cboBlockDepartment.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboBlockDepartment.Properties.View);
                    if (this.oldBlockDepartmentIds != null && this.oldBlockDepartmentIds.Count > 0)
                    {
                        List<HIS_DEPARTMENT> seleceds = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => this.oldBlockDepartmentIds.Contains(o.ID)).ToList();
                        gridCheckMark.SelectAll(seleceds);

                        string displayText = String.Join(", ", seleceds.Select(s => s.DEPARTMENT_NAME).ToList());
                        cboBlockDepartment.Text = displayText;
                        cboBlockDepartment.ToolTip = displayText;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetDataToMedicineLine(object _medicineLine)
        {
            try
            {
                if (_medicineLine != null && _medicineLine is MOS.EFMODEL.DataModels.HIS_MEDICINE_LINE)
                {
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("MEDICINE_LINE_CODE", "", 50, 1));
                    columnInfos.Add(new ColumnInfo("MEDICINE_LINE_NAME", "", 100, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_LINE_NAME", "ID", columnInfos, false, 150);
                    MOS.Filter.HisMedicineLineFilter filter = new HisMedicineLineFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    CommonParam param = new CommonParam();
                    var heinServiceBhyts = new BackendAdapter(param).Get<List<HIS_MEDICINE_LINE>>(HisRequestUriStore.HIS_MEDICINE_LINE_GET, ApiConsumers.MosConsumer, filter, param);
                    ControlEditorLoader.Load(cboMedicineLine, heinServiceBhyts, controlEditorADO);
                    cboMedicineLine.EditValue = ((MOS.EFMODEL.DataModels.HIS_MEDICINE_LINE)_medicineLine).ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RefreshDataToMedicineTypeParent(object _medicineType)
        {
            try
            {
                if (_medicineType != null && _medicineType is MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE)
                {
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_CODE", "", 50, 1));
                    columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_NAME", "", 100, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_TYPE_NAME", "ID", columnInfos, false, 150);
                    MOS.Filter.HisMedicineTypeFilter filter = new HisMedicineTypeFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    filter.IS_LEAF = null;
                    CommonParam param = new CommonParam();
                    var data = new BackendAdapter(param).Get<List<HIS_MEDICINE_TYPE>>(HisRequestUriStore.HIS_MEDICINE_TYPE_GET, ApiConsumers.MosConsumer, filter, param);
                    ControlEditorLoader.Load(cboMedicineTypeParent, data, controlEditorADO);
                    cboMedicineTypeParent.EditValue = ((MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE)_medicineType).ID;
                    txtMedicineTypeParentCode.Text = ((MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE)_medicineType).MEDICINE_TYPE_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SendDataAfterSave()
        {
            try
            {
                if (this.delegateSelect != null)
                {
                    this.delegateSelect(resultData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region ---Fill data to combo---
        private void InitMedincineUseForm()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDICINE_USE_FORM_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("MEDICINE_USE_FORM_NAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_USE_FORM_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(cboMedicineUseForm, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitMedicineGroup()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDICINE_GROUP_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("MEDICINE_GROUP_NAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_GROUP_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(cboMedicineGroup, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_GROUP>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboGender()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("GENDER_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("GENDER_NAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("GENDER_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(cboGender, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitHowToUse()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("HTU_NAME", "", 100, 2));
                var a = HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer;
                ControlEditorADO controlEditorADO = new ControlEditorADO("HTU_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(cboHowToUse, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_HTU>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitManufacture()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MANUFACTURER_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("MANUFACTURER_NAME", "", 100, 2));
                var a = HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer;
                ControlEditorADO controlEditorADO = new ControlEditorADO("MANUFACTURER_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(cboManufacture, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MANUFACTURER>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitServiceUnit()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_NAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_UNIT_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(cboServiceUnit, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitProcessingMethod()
        {
            try
            {
                lstProcessing = BackendDataWorker.Get<HIS_PROCESSING_METHOD>().Where(o => o.PROCESSING_METHOD_TYPE == 2).ToList();
                lstPreProcessing = BackendDataWorker.Get<HIS_PROCESSING_METHOD>().Where(o => o.PROCESSING_METHOD_TYPE == 1).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitRank()
        {
            try
            {
                List<RankADO> rankADOs = new List<RankADO>();

                RankADO ado1 = new RankADO();
                ado1.RANK = IMSys.DbConfig.HIS_RS.HIS_MEDICINE_TYPE.MEDICINE_TYPE_RANK__1;
                rankADOs.Add(ado1);

                RankADO ado2 = new RankADO();
                ado2.RANK = IMSys.DbConfig.HIS_RS.HIS_MEDICINE_TYPE.MEDICINE_TYPE_RANK__2;
                rankADOs.Add(ado2);

                RankADO ado3 = new RankADO();
                ado3.RANK = IMSys.DbConfig.HIS_RS.HIS_MEDICINE_TYPE.MEDICINE_TYPE_RANK__3;
                rankADOs.Add(ado3);

                RankADO ado4 = new RankADO();
                ado4.RANK = IMSys.DbConfig.HIS_RS.HIS_MEDICINE_TYPE.MEDICINE_TYPE_RANK__4;
                rankADOs.Add(ado4);

                RankADO ado5 = new RankADO();
                ado5.RANK = IMSys.DbConfig.HIS_RS.HIS_MEDICINE_TYPE.MEDICINE_TYPE_RANK__5;
                rankADOs.Add(ado5);

                cboRank.Properties.DataSource = rankADOs;
                cboRank.Properties.DisplayMember = "RANK";
                cboRank.Properties.ValueMember = "RANK";
                cboRank.Properties.TextEditStyle = TextEditStyles.Standard;
                //cboSupplier.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Default;
                cboRank.Properties.ImmediatePopup = true;
                cboRank.ForceInitialize();
                cboRank.Properties.View.Columns.Clear();
                cboRank.Properties.PopupFormSize = new System.Drawing.Size(30, 100);

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cboRank.Properties.View.Columns.AddField("RANK");
                aColumnCode.Caption = "";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 30;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitHeinServiceType()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("HEIN_SERVICE_TYPE_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("HEIN_SERVICE_TYPE_NAME", "", 100, 2));
                var a = HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer;
                ControlEditorADO controlEditorADO = new ControlEditorADO("HEIN_SERVICE_TYPE_NAME", "ID", columnInfos, false, 150);
                List<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE> source = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE>();
                if (source != null)
                {
                    ControlEditorLoader.Load(cboHeinServiceType, source.Where(o => new List<long>() { IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_TDM, IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_NDM, IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_UT, IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_TL }.Contains(o.ID)).ToList(), controlEditorADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitMedicineTypeParent()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_NAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_TYPE_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(cboMedicineTypeParent, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE>().Where(o => o.IS_LEAF == null).ToList(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitMedicineType()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_TYPE_NAME", "ID", columnInfos, false, 300);
                ControlEditorLoader.Load(cboMedicineType, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitMedicineLine()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDICINE_LINE_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("MEDICINE_LINE_NAME", "", 100, 2));
                var a = HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer;
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_LINE_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(cboMedicineLine, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_LINE>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
        private void UpdatePatientDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE medicineType) //CHECKKK
        {
            try
            {
                if (this.ActionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                {
                    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE, MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE>();
                    medicineType = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE>(currentVHisMedicineTypeDTODefault);
                    medicineType.ID = currentVHisMedicineTypeDTODefault.ID;
                }

                medicineType.HIS_SERVICE = new HIS_SERVICE();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE>(medicineType.HIS_SERVICE, currentRightClick);
                if (this.ActionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit && currentVHisMedicineTypeDTODefault != null)
                {
                    medicineType.HIS_SERVICE.ID = currentVHisMedicineTypeDTODefault.SERVICE_ID;
                    medicineType.HIS_SERVICE.SERVICE_UNIT_ID = currentVHisMedicineTypeDTODefault.SERVICE_UNIT_ID;
                    medicineType.HIS_SERVICE.SERVICE_TYPE_ID = currentVHisMedicineTypeDTODefault.SERVICE_TYPE_ID;
                    medicineType.VACCINE_TYPE_ID = null;
                }
                medicineType.ALERT_MAX_IN_TREATMENT = this.UseInTreat;
                if (chkBlockInTreat.Checked) medicineType.IS_BLOCK_MAX_IN_TREATMENT = 1;
                else medicineType.IS_BLOCK_MAX_IN_TREATMENT = null;
                if (spinNumOrder.EditValue != null)
                {
                    medicineType.NUM_ORDER = Inventec.Common.TypeConvert.Parse.ToInt64((spinNumOrder.EditValue ?? "").ToString());
                }
                else
                {
                    medicineType.NUM_ORDER = null;
                }

                if (spinAgeFrom.EditValue != null)
                {
                    medicineType.HIS_SERVICE.AGE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64((spinAgeFrom.EditValue ?? "").ToString());
                }
                else
                {
                    medicineType.HIS_SERVICE.AGE_FROM = null;
                }

                if (cboNguonGoc.EditValue != null && cboNguonGoc.EditValue.ToString() != "")
                {
                    medicineType.SOURCE_MEDICINE = Inventec.Common.TypeConvert.Parse.ToSByte(cboNguonGoc.EditValue.ToString());
                }
                else
                {
                    medicineType.SOURCE_MEDICINE = null;
                }

                if (cboPreserveCondition.EditValue != null && cboPreserveCondition.EditValue.ToString() != "")
                {
                    medicineType.STORAGE_CONDITION_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboPreserveCondition.EditValue).ToString());
                }
                else
                {
                    medicineType.STORAGE_CONDITION_ID = null;
                }

                if (contraindicationSelecteds != null && contraindicationSelecteds.Count > 0)
                {
                    string listContraindicationIds = String.Join(",", contraindicationSelecteds.Select(s => s.ID).ToList()); ;

                    medicineType.CONTRAINDICATION_IDS = listContraindicationIds;
                }

                if (spinAgeTo.EditValue != null)
                {
                    medicineType.HIS_SERVICE.AGE_TO = Inventec.Common.TypeConvert.Parse.ToInt64((spinAgeTo.EditValue ?? "").ToString());
                }
                else
                {
                    medicineType.HIS_SERVICE.AGE_TO = null;
                }



                if (ChkIsSpecificHeinPrice.CheckState == CheckState.Checked)
                    medicineType.HIS_SERVICE.IS_SPECIFIC_HEIN_PRICE = 1;
                else
                    medicineType.HIS_SERVICE.IS_SPECIFIC_HEIN_PRICE = null;

                if (cboOTHER_PAY_SOURCE.EditValue != null)
                {
                    medicineType.HIS_SERVICE.OTHER_PAY_SOURCE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboOTHER_PAY_SOURCE.EditValue ?? "0").ToString());
                }
                else
                {
                    medicineType.HIS_SERVICE.OTHER_PAY_SOURCE_ID = null;
                }
                if (txtOTHER_PAY_SOURCE.Enabled)
                {
                    medicineType.HIS_SERVICE.OTHER_PAY_SOURCE_ICDS = !string.IsNullOrEmpty(txtOTHER_PAY_SOURCE.Text.Trim()) ? txtOTHER_PAY_SOURCE.Text.Trim() : null;
                }
                else
                    medicineType.HIS_SERVICE.OTHER_PAY_SOURCE_ICDS = null;

                if (txtBhytWhiteListCodes.Enabled)
                {
                    medicineType.HIS_SERVICE.BHYT_WHITELIST_CODES = !string.IsNullOrEmpty(txtBhytWhiteListCodes.Text.Trim()) ? txtBhytWhiteListCodes.Text.Trim() : null;
                }
                else
                    medicineType.HIS_SERVICE.BHYT_WHITELIST_CODES = null;

                medicineType.ACTIVE_INGR_BHYT_CODE = txtActiveIngrBhytCode.Text.Trim();
                medicineType.ACTIVE_INGR_BHYT_NAME = txtActiveIngrBhytName.Text.Trim();
                medicineType.HIS_SERVICE.HEIN_SERVICE_BHYT_NAME = txtHeinServiceBhytName.Text.Trim();
                //medicineType.HIS_SERVICE.HEIN_LIMIT_PRICE_OLD = txtHeinLimitPriceOld.Text;
                medicineType.MEDICINE_TYPE_CODE = txtMedicineTypeCode.Text.Trim();
                medicineType.MEDICINE_TYPE_NAME = txtMedicineTypeName.Text;
                medicineType.DOSAGE_FORM = cboDosageForm.Text.Trim();

                if (spinAlertExpiredDate.EditValue != null)
                {
                    medicineType.ALERT_EXPIRED_DATE = Inventec.Common.TypeConvert.Parse.ToInt64((spinAlertExpiredDate.EditValue ?? "").ToString());
                }
                else
                    medicineType.ALERT_EXPIRED_DATE = null;

                medicineType.CONCENTRA = txtConcentra.Text;

                if (cboManufacture.EditValue != null)
                {
                    medicineType.MANUFACTURER_ID = Inventec.Common.TypeConvert.Parse.ToInt32((cboManufacture.EditValue ?? "").ToString());
                }
                else
                {
                    medicineType.MANUFACTURER_ID = null;
                }

                if (cboHowToUse.EditValue != null)
                {
                    medicineType.HTU_ID = Inventec.Common.TypeConvert.Parse.ToInt32((cboHowToUse.EditValue ?? "").ToString());
                }
                else
                {
                    medicineType.HTU_ID = null;
                }

                if (cboMedicineLine.EditValue != null)
                {
                    medicineType.MEDICINE_LINE_ID = Inventec.Common.TypeConvert.Parse.ToInt32((cboMedicineLine.EditValue ?? "").ToString());
                }
                else
                {
                    medicineType.MEDICINE_LINE_ID = null;
                }

                if (cboMedicineUseForm.EditValue != null)
                {
                    medicineType.MEDICINE_USE_FORM_ID = Inventec.Common.TypeConvert.Parse.ToInt32((cboMedicineUseForm.EditValue ?? "").ToString());
                }
                else
                {
                    medicineType.MEDICINE_USE_FORM_ID = null;
                }

                medicineType.PACKING_TYPE_NAME = txtPackingTypeCode.Text;

                if (cboMedicineGroup.EditValue != null)
                {
                    medicineType.MEDICINE_GROUP_ID = Inventec.Common.TypeConvert.Parse.ToInt32((cboMedicineGroup.EditValue ?? "").ToString());
                }
                else
                {
                    medicineType.MEDICINE_GROUP_ID = null;
                }

                medicineType.DESCRIPTION = txtDescription.Text;
                medicineType.ODD_WARNING_CONTENT = txtContentWarning.Text.Trim();
                medicineType.CONTRAINDICATION = txtContraindication.Text;
                medicineType.RECORDING_TRANSACTION = txtRecordingTransaction.Text.Trim();
                if (cboImpUnit.EditValue != null)
                {
                    medicineType.IMP_UNIT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboImpUnit.EditValue.ToString());
                    medicineType.IMP_UNIT_CONVERT_RATIO = spUnitConvertRatio.Value;
                }
                else
                {
                    medicineType.IMP_UNIT_ID = null;
                    medicineType.IMP_UNIT_CONVERT_RATIO = null;
                }

                medicineType.HIS_SERVICE.SERVICE_UNIT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceUnit.EditValue ?? "").ToString());
                if (dtHeinLimitPriceIntrTime.EditValue != null && dtHeinLimitPriceIntrTime.DateTime != DateTime.MinValue)
                    medicineType.HIS_SERVICE.HEIN_LIMIT_PRICE_INTR_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(
                       Convert.ToDateTime(dtHeinLimitPriceIntrTime.EditValue).ToString("yyyyMMddHHmm") + "00");
                else
                    medicineType.HIS_SERVICE.HEIN_LIMIT_PRICE_INTR_TIME = null;

                if (dtHSD.EditValue != null && dtHSD.DateTime != DateTime.MinValue)
                    medicineType.LAST_EXPIRED_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(
                       Convert.ToDateTime(dtHSD.EditValue).ToString("yyyyMMddHHmm") + "00");
                else
                    medicineType.LAST_EXPIRED_DATE = null;

                if (!string.IsNullOrEmpty(txtHeinLimitRatio.Text))
                {
                    medicineType.HIS_SERVICE.HEIN_LIMIT_RATIO = CustomParse.ConvertCustom(txtHeinLimitRatio.Text) / 100;
                }
                else
                    medicineType.HIS_SERVICE.HEIN_LIMIT_RATIO = null;

                if (!string.IsNullOrEmpty(txtHeinLimitRatioOld.Text))
                {
                    medicineType.HIS_SERVICE.HEIN_LIMIT_RATIO_OLD = CustomParse.ConvertCustom(txtHeinLimitRatioOld.Text) / 100;
                }
                else
                    medicineType.HIS_SERVICE.HEIN_LIMIT_RATIO_OLD = null;

                if (spinImpPrice.EditValue != null)
                {
                    medicineType.IMP_PRICE = (decimal)spinImpPrice.Value;
                }
                else
                    medicineType.IMP_PRICE = null;

                if (spinImpVatRatio.EditValue != null)
                {
                    medicineType.IMP_VAT_RATIO = ((decimal)spinImpVatRatio.Value / 100);
                }
                else
                {
                    medicineType.IMP_VAT_RATIO = null;
                }

                if (spinInternalPrice.EditValue != null)
                {
                    medicineType.INTERNAL_PRICE = (decimal)spinInternalPrice.Value;
                }
                else
                {
                    medicineType.INTERNAL_PRICE = null;
                }

                if (spinLastExpVatRatio.EditValue != null)
                {
                    medicineType.LAST_EXP_VAT_RATIO = ((decimal)spinLastExpVatRatio.Value / 100);
                }
                else
                {
                    medicineType.LAST_EXP_VAT_RATIO = null;
                }

                if (spinLastExpPrice.EditValue != null)
                {
                    medicineType.LAST_EXP_PRICE = (decimal)spinLastExpPrice.Value;
                }
                else
                {
                    medicineType.LAST_EXP_PRICE = null;
                }

                #region Danh sach thuoc
                //if (lstMedicine != null && lstMedicine.Count > 0)
                {
                    if (lstMedicine.Any(x=>x.ID == 1)) medicineType.IS_CHEMICAL_SUBSTANCE = 1;
                    else medicineType.IS_CHEMICAL_SUBSTANCE = null;

                    if (lstMedicine.Any(x => x.ID == 5)) medicineType.IS_VACCINE = 1;
                    else medicineType.IS_VACCINE = null;

                    if (lstMedicine.Any(x => x.ID == 9))
                    {
                        medicineType.HIS_SERVICE.HEIN_SERVICE_BHYT_CODE = medicineType.ACTIVE_INGR_BHYT_CODE;
                        medicineType.IS_OXYGEN = 1;
                    }
                    else medicineType.IS_OXYGEN = null;

                    if (lstMedicine.Any(x => x.ID == 4)) medicineType.IS_GENERIC = 1;
                    else medicineType.IS_GENERIC = null;

                    if (lstMedicine.Any(x => x.ID == 2)) medicineType.IS_FUNCTIONAL_FOOD = 1;
                    else medicineType.IS_FUNCTIONAL_FOOD = null;

                    if (lstMedicine.Any(x => x.ID == 6)) medicineType.IS_VITAMIN_A = 1;
                    else medicineType.IS_VITAMIN_A = null;

                    if (lstMedicine.Any(x => x.ID == 7)) medicineType.IS_TCMR = 1;
                    else medicineType.IS_TCMR = null;

                    if (lstMedicine.Any(x => x.ID == 8)) medicineType.IS_BIOLOGIC = 1;
                    else medicineType.IS_BIOLOGIC = null;

                    if (lstMedicine.Any(x => x.ID == 3)) medicineType.IS_STAR_MARK = 1;
                    else medicineType.IS_STAR_MARK = null;

                    if (lstMedicine.Any(x => x.ID == 10)) medicineType.IS_ANAESTHESIA = 1;
                    else medicineType.IS_ANAESTHESIA = null;

                    if (lstMedicine.Any(x => x.ID == 11)) medicineType.IS_ORIGINAL_BRAND_NAME = 1;
                    else medicineType.IS_ORIGINAL_BRAND_NAME = null;

                    if (lstMedicine.Any(x => x.ID == 12)) medicineType.IS_KIDNEY = 1;
                    else medicineType.IS_KIDNEY = null;

                    if (lstMedicine.Any(x => x.ID == 13)) medicineType.IS_RAW_MEDICINE = 1;
                    else medicineType.IS_RAW_MEDICINE = null;
                }
                #endregion
                {
                    if (lstConfig.Any(x => x.ID == 1)) medicineType.IS_STOP_IMP = 1;
                    else medicineType.IS_STOP_IMP = null;

                    if (lstConfig.Any(x => x.ID == 5)) medicineType.IS_ALLOW_EXPORT_ODD = 1;
                    else medicineType.IS_ALLOW_EXPORT_ODD = null;

                    if (lstConfig.Any(x => x.ID == 9))
                    {
                        medicineType.IS_AUTO_EXPEND = 1;
                    }
                    else medicineType.IS_AUTO_EXPEND = null;

                    if (lstConfig.Any(x => x.ID == 4)) medicineType.IS_ALLOW_ODD = 1;
                    else medicineType.IS_ALLOW_ODD = null;

                    if (lstConfig.Any(x => x.ID == 2)) medicineType.HIS_SERVICE.IS_OUT_PARENT_FEE = 1;
                    else medicineType.HIS_SERVICE.IS_OUT_PARENT_FEE = null;

                    if (lstConfig.Any(x => x.ID == 6)) medicineType.IS_SPLIT_COMPENSATION = 1;
                    else medicineType.IS_SPLIT_COMPENSATION = null;

                    if (lstConfig.Any(x => x.ID == 7)) medicineType.ALLOW_MISSING_PKG_INFO = 1;
                    else medicineType.ALLOW_MISSING_PKG_INFO = null;

                    if (lstConfig.Any(x => x.ID == 8)) medicineType.IS_MUST_PREPARE = 1;
                    else medicineType.IS_MUST_PREPARE = null;

                    if (lstConfig.Any(x => x.ID == 3)) medicineType.IS_REQUIRE_HSD = 1;
                    else medicineType.IS_REQUIRE_HSD = null;

                    if (lstConfig.Any(x => x.ID == 10)) medicineType.HIS_SERVICE.IS_OTHER_SOURCE_PAID = 1;
                    else medicineType.HIS_SERVICE.IS_OTHER_SOURCE_PAID = null;

                    if (lstConfig.Any(x => x.ID == 11)) medicineType.IS_TREATMENT_DAY_COUNT = 1;
                    else medicineType.IS_TREATMENT_DAY_COUNT = null;

                    if (lstConfig.Any(x => x.ID == 12)) medicineType.IS_NOT_TREATMENT_DAY_COUNT = 1;
                    else medicineType.IS_NOT_TREATMENT_DAY_COUNT = null;

                    if (lstConfig.Any(x => x.ID == 13)) medicineType.HIS_SERVICE.IS_OUT_OF_DRG = 1;
                    else medicineType.HIS_SERVICE.IS_OUT_OF_DRG = null;

                    if (lstConfig.Any(x => x.ID == 14)) medicineType.IS_OUT_HOSPITAL = 1;
                    else medicineType.IS_OUT_HOSPITAL = null;

                    if (lstConfig.Any(x => x.ID == 15)) medicineType.IS_BUSINESS = 1;
                    else medicineType.IS_BUSINESS = null;

                    if (lstConfig.Any(x => x.ID == 16)) medicineType.IS_DRUG_STORE = 1;
                    else medicineType.IS_DRUG_STORE = null;
                }


                if (rdoBlock.Checked)
                {
                    medicineType.IS_BLOCK_MAX_IN_PRESCRIPTION = 1;
                }
                else
                {
                    medicineType.IS_BLOCK_MAX_IN_PRESCRIPTION = null;
                }
                if (rdoBlock1.Checked)
                {
                    medicineType.IS_BLOCK_MAX_IN_DAY = 1;
                }
                else
                {
                    medicineType.IS_BLOCK_MAX_IN_DAY = null;
                }



                if (chkIsSaleEqualImpPrice.CheckState == CheckState.Checked)
                    medicineType.IS_SALE_EQUAL_IMP_PRICE = 1;
                else
                    medicineType.IS_SALE_EQUAL_IMP_PRICE = null;


                if (cboManufacture.EditValue != null)
                {
                    medicineType.MANUFACTURER_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboManufacture.EditValue ?? "").ToString());
                }
                else
                {
                    medicineType.MANUFACTURER_ID = null;
                }

                if (cboMedicineTypeParent.EditValue != null)
                {
                    medicineType.PARENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboMedicineTypeParent.EditValue ?? "").ToString());
                }
                else
                {
                    medicineType.PARENT_ID = null;
                }

                if (ucNational != null && nationalProcessor != null)
                {
                    var nati = (HIS.UC.National.ADO.NationalInputADO)nationalProcessor.GetValue(ucNational);
                    medicineType.NATIONAL_NAME = nati.NATIONAL_NAME;
                }


                if (spinAlertMinInStock.EditValue != null)
                {
                    medicineType.ALERT_MIN_IN_STOCK = (decimal)spinAlertMinInStock.EditValue;
                }
                else
                    medicineType.ALERT_MIN_IN_STOCK = null;
                medicineType.QUALITY_STANDARDS = txtTCCL.Text;
                medicineType.TUTORIAL = txtTutorial.Text;
                if (spinUseOnDay.EditValue != null)
                {
                    medicineType.ALERT_MAX_IN_PRESCRIPTION = (decimal)spinUseOnDay.Value;
                }
                else
                    medicineType.ALERT_MAX_IN_PRESCRIPTION = null;
                if (spinUseInDay.EditValue != null)
                {
                    medicineType.ALERT_MAX_IN_DAY = (decimal)spinUseInDay.Value;
                }
                else
                    medicineType.ALERT_MAX_IN_DAY = null;
                medicineType.BYT_NUM_ORDER = txtBytNumOrder.Text;
                medicineType.REGISTER_NUMBER = txtRegisterNumber.Text;
                medicineType.TCY_NUM_ORDER = txtTcyNumOrder.Text;
                medicineType.NUM_ORDER_CIRCULARS20 = txtSttTT20.Text;







                if (cboHeinServiceType.EditValue != null)
                {
                    medicineType.HIS_SERVICE.HEIN_SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboHeinServiceType.EditValue ?? "").ToString());
                }
                else
                    medicineType.HIS_SERVICE.HEIN_SERVICE_TYPE_ID = null;

                




                if (cboGender.EditValue != null)
                {
                    medicineType.TDL_GENDER_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboGender.EditValue.ToString());
                    medicineType.HIS_SERVICE.GENDER_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboGender.EditValue.ToString());
                }
                else
                {
                    medicineType.TDL_GENDER_ID = null;
                    medicineType.HIS_SERVICE.GENDER_ID = null;
                }

                if (cboRank.EditValue != null)
                {
                    medicineType.RANK = Inventec.Common.TypeConvert.Parse.ToInt64(cboRank.EditValue.ToString());
                }
                else
                {
                    medicineType.RANK = null;
                }

                medicineType.MEDICINE_NATIONAL_CODE = txtMedicineNationalCode.Text;
                if (this.activeIngredients != null)
                {
                    List<HIS_MEDICINE_TYPE_ACIN> medicineTypeAcins = new List<HIS_MEDICINE_TYPE_ACIN>();
                    foreach (var item in this.activeIngredients)
                    {
                        medicineTypeAcins.Add(new HIS_MEDICINE_TYPE_ACIN() { IS_ACTIVE = 1, ACTIVE_INGREDIENT_ID = item.ID, MEDICINE_TYPE_ID = (this.currentMedicineTypeId ?? 0) });
                    }
                    medicineType.HIS_MEDICINE_TYPE_ACIN = medicineTypeAcins;
                }



                if (spinVolume.EditValue != null)
                {
                    medicineType.VOLUME = ((decimal)spinVolume.Value);
                }
                else
                {
                    medicineType.VOLUME = null;
                }

                medicineType.ATC_CODES = txtACT_Code.Text.Trim();
                medicineType.ATC_GROUP_CODES = txtATCGroup_Code.Text.Trim();
                medicineType.SCIENTIFIC_NAME = txtScientificName.Text.Trim();
                medicineType.PREPROCESSING_CODE = txtPreprocessing.Text.Trim();
                medicineType.PREPROCESSING = txtPreprocessingName.Text.Trim();
                medicineType.PROCESSING_CODE = txtProcessing.Text.Trim();
                medicineType.PROCESSING = txtProcessingName.Text.Trim();
                medicineType.USED_PART = txtUsedPart.Text.Trim();
                medicineType.DISTRIBUTED_AMOUNT = txtDistributedAmount.Text.Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SuccessLog(HIS_MEDICINE_TYPE result)
        {
            try
            {
                if (result != null)
                {
                    //string message = String.Format(EXE.LOGIC.Base.EventLogUtil.SetLog(His.EventLog.Message.Enum.SuaThongTinBenhNhan), result.PATIENT_CODE, result.VIR_PATIENT_NAME, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(result.DOB), cboGender.Text, currentHisPatientTypeAlter.TREATMENT_ID, currentHisPatientTypeAlter.TREATMENT_TYPE_NAME);
                    //His.EventLog.Logger.Log(LOGIC.LocalStore.GlobalStore.APPLICATION_CODE, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), message, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginAddress());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void RefreshDataToMedicineUseForm(object medicineUseForm)
        {
            try
            {
                if (medicineUseForm != null && medicineUseForm is MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM)
                {
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("MEDICINE_USE_FORM_CODE", "", 50, 1));
                    columnInfos.Add(new ColumnInfo("MEDICINE_USE_FORM_NAME", "", 100, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_USE_FORM_NAME", "ID", columnInfos, false, 150);
                    MOS.Filter.HisMedicineUseFormFilter filter = new HisMedicineUseFormFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    CommonParam param = new CommonParam();
                    var hisMedicineUseForms = new BackendAdapter(param).Get<List<HIS_MEDICINE_USE_FORM>>(HisRequestUriStore.HIS_MEDICINE_USE_FORM_GET, ApiConsumers.MosConsumer, filter, param);
                    ControlEditorLoader.Load(cboMedicineUseForm, hisMedicineUseForms, controlEditorADO);
                    cboMedicineUseForm.EditValue = ((MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM)medicineUseForm).ID;
                    txtMedicineUseFormCode.Text = ((MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM)medicineUseForm).MEDICINE_USE_FORM_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDatatoComboPatientType()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(repositoryItemGridLookUpPatientType, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList(), controlEditorADO);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void LoadACTCode()
        {
            try
            {
                btnACT_Click(null, null);
                if (this.Acts != null)
                {
                    this.txtACT_Code.Text = string.Join(",", Acts.Select(o => o.ATC_CODE).ToList());
                    this.txtACT_Name.Text = string.Join(",", Acts.Select(o => o.ATC_NAME).ToList());
                    this.txtATCGroup_Code.Text = "";
                    this.txtATCGroup_Name.Text = "";
                    ActGroups = BackendDataWorker.Get<HIS_ATC_GROUP>().ToList();
                    var atcCodes = this.Acts.Select(o => o.ATC_CODE.Length > 5 ? o.ATC_CODE.Substring(0, 5) : o.ATC_CODE).ToList();
                    if (ActGroups != null && ActGroups.Count > 0 && atcCodes != null && atcCodes.Count > 0)
                    {
                        var atcGroups = ActGroups.Where(o => atcCodes.Contains(o.ATC_GROUP_CODE)).ToList();
                        if (atcGroups != null && atcGroups.Count > 0)
                        {
                            this.txtATCGroup_Code.Text = string.Join(",", atcGroups.Select(o => o.ATC_GROUP_CODE).ToList());
                            this.txtATCGroup_Name.Text = string.Join(",", atcGroups.Select(o => o.ATC_GROUP_NAME).ToList());
                        }
                    }
                    else if (HisConfigCFG.AtcCodeOverlarWarningOption == "2" && atcCodes != null && atcCodes.Count > 0)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Các mã ATC sau chưa có nhóm ATC: {0}. Bạn có muốn thêm nhóm ATC không?", String.Join(",", atcCodes)), "Thông báo", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            List<Object> listArgs = new List<object>();
                            if (this.module == null)
                            {
                                CallModule callModule = new CallModule(CallModule.HisAtcGroup, 0, 0, listArgs);
                            }
                            else
                            {
                                CallModule callModule = new CallModule(CallModule.HisAtcGroup, this.module.RoomId, this.module.RoomTypeId, listArgs);
                            }
                            ActGroups = BackendDataWorker.Get<HIS_ATC_GROUP>().ToList();
                            if (ActGroups != null && ActGroups.Count > 0 && atcCodes != null && atcCodes.Count > 0)
                            {
                                var atcGroups = ActGroups.Where(o => atcCodes.Contains(o.ATC_GROUP_CODE)).ToList();
                                if (atcGroups != null && atcGroups.Count > 0)
                                {
                                    this.txtATCGroup_Code.Text = string.Join(",", atcGroups.Select(o => o.ATC_GROUP_CODE).ToList());
                                    this.txtATCGroup_Name.Text = string.Join(",", atcGroups.Select(o => o.ATC_GROUP_NAME).ToList());
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool Check()
        {
            bool result = true;
            try
            {
                if (this.lsVHisServicePaty == null || this.lsVHisServicePaty.Count == 0)
                    throw new ArgumentNullException("Du lieu dau vao khong hop le. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lsVHisServicePaty), lsVHisServicePaty));

                var groupPatientType = from p in this.lsVHisServicePaty
                                       group p by new
                                       {
                                           p.PATIENT_TYPE_ID,
                                           p.BRANCH_ID,
                                           p.FROM_TIME,
                                           p.TO_TIME
                                       } into g
                                       select new { Key = g.Key, CareDetail = g.ToList() };
                if (groupPatientType != null && groupPatientType.Count() > 0)
                {
                    foreach (var item in groupPatientType)
                    {
                        if (item.CareDetail.Count > 1)
                        {
                            result = false;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void SaveProcessorsHisServicePaty(ref CommonParam param)
        {
            try
            {
                DataUpdateAndInsert();
                if (this.patientServicePatyError != null && this.patientServicePatyError.Count() > 0)
                {
                    string str = "";
                    var listPatientType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                    foreach (var item in patientServicePatyError.Distinct())
                    {
                        str += listPatientType.FirstOrDefault(o => o.ID == item).PATIENT_TYPE_NAME + ",";
                    }
                    MessageBox.Show("Lưu chính sách giá thất bại, ngày áp dụng từ không được lớn hơn ngày áp dụng đến. Đối tượng " + str, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                WaitingManager.Show();
                if (this.ServicePatyCreate != null && this.ServicePatyCreate.Count > 0)
                {
                    CommonParam commonpara = new CommonParam();
                    var resultData1 = new BackendAdapter(commonpara).Post<List<HIS_SERVICE_PATY>>(
                   "api/HisServicePaty/CreateList", ApiConsumers.MosConsumer, ServicePatyCreate, commonpara);
                    if (resultData1 != null)
                    {
                        ServicePatyCreate = null;
                        BackendDataWorker.Reset<HIS_SERVICE_PATY>();
                    }
                    else if ((commonpara.Messages != null && commonpara.Messages.Count > 0)
                        || (commonpara.BugCodes != null && commonpara.BugCodes.Count > 0))
                    {
                        param.Messages.AddRange(commonpara.Messages);
                        param.BugCodes.AddRange(commonpara.BugCodes);
                    }
                }

                if (this.ServicePatyUpdate != null && this.ServicePatyUpdate.Count > 0)
                {
                    CommonParam commonparam = new CommonParam();
                    var resultData = new BackendAdapter(commonparam).Post<List<HIS_SERVICE_PATY>>(
                   "api/HisServicePaty/UpdateList", ApiConsumers.MosConsumer, ServicePatyUpdate, commonparam);
                    if (resultData != null)
                    {
                        ServicePatyUpdate = null;
                        BackendDataWorker.Reset<HIS_SERVICE_PATY>();
                    }
                    else if ((commonparam.Messages != null && commonparam.Messages.Count > 0)
                        || (commonparam.BugCodes != null && commonparam.BugCodes.Count > 0))
                    {
                        param.Messages.AddRange(commonparam.Messages);
                        param.BugCodes.AddRange(commonparam.BugCodes);
                    }
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void DataUpdateAndInsert()
        {
            try
            {
                if (this.lsVHisServicePaty != null && this.lsVHisServicePaty.Count > 0)
                {
                    this.ServicePatyCreate = new List<HIS_SERVICE_PATY>();
                    this.ServicePatyUpdate = new List<HIS_SERVICE_PATY>();
                    this.patientServicePatyError = new List<long>();
                    foreach (var item in this.lsVHisServicePaty)
                    {
                        if (item.FROM_TIME > item.TO_TIME)
                        {
                            this.patientServicePatyError.Add(item.PATIENT_TYPE_ID);
                        }
                        else
                        {
                            if (item.ID > 0 && item.PATIENT_TYPE_ID > 0 && resultData.SERVICE_ID > 0 && item.PRICE != null && item.VAT_RATIO >= 0 && item.VAT_RATIO <= 100)
                            {
                                HIS_SERVICE_PATY data = new HIS_SERVICE_PATY();
                                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_PATY>(data, item);
                                data.VAT_RATIO = item.VAT_RATIO != null ? item.VAT_RATIO / 100 : 0;
                                this.ServicePatyUpdate.Add(data);
                            }
                            else if (resultData.SERVICE_ID > 0 && item.PATIENT_TYPE_ID > 0 && item.PRICE != null && item.VAT_RATIO >= 0 && item.VAT_RATIO <= 100)
                            {
                                HIS_SERVICE_PATY data = new HIS_SERVICE_PATY();
                                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_PATY>(data, item);
                                data.VAT_RATIO = item.VAT_RATIO != null ? item.VAT_RATIO / 100 : 0;
                                //data.BRANCH_ID = this.BranchID;
                                data.SERVICE_ID = resultData.SERVICE_ID;
                                this.ServicePatyCreate.Add(data);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveBlockDepartment(ref CommonParam param)
        {
            try
            {
                WaitingManager.Show();
                List<long> selectedDepartmentIds = new List<long>();
                List<long> oldDepartmentIds = new List<long>();

                if (this.oldBlockDepartmentIds != null)
                {
                    oldDepartmentIds = this.oldBlockDepartmentIds;
                }

                if (this.BlockDepartment__Seleced != null)
                {
                    selectedDepartmentIds = this.BlockDepartment__Seleced.Select(s => s.ID).Distinct().ToList();
                }

                if (selectedDepartmentIds.Count == 0 && oldDepartmentIds.Count == 0)
                {
                    return;
                }

                if (selectedDepartmentIds.Exists(e => !oldDepartmentIds.Contains(e))
                    || oldDepartmentIds.Exists(e => !selectedDepartmentIds.Contains(e)))
                {
                    CommonParam commonpara = new CommonParam();
                    HisMestMetyDepaByMedicineSDO sdo = new HisMestMetyDepaByMedicineSDO();
                    sdo.MedicineTypeId = resultData.ID;
                    sdo.DepartmentIds = selectedDepartmentIds;
                    var rs = new BackendAdapter(commonpara).Post<bool>("api/HisMestMetyDepa/CreateByMedicine", ApiConsumers.MosConsumer, sdo, commonpara);
                    if (!rs && (commonpara.Messages != null && commonpara.Messages.Count > 0)
                        || (commonpara.BugCodes != null && commonpara.BugCodes.Count > 0))
                    {
                        param.Messages.AddRange(commonpara.Messages);
                        param.BugCodes.AddRange(commonpara.BugCodes);
                    }

                    this.oldBlockDepartmentIds = new List<long>();
                    if (selectedDepartmentIds.Count > 0)
                    {
                        this.oldBlockDepartmentIds.AddRange(selectedDepartmentIds);
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void UpdateData(MOS.SDO.HisMedicineTypeSDO medicineTypeSdo)
        {
            try
            {
                if (medicineTypeSdo != null)
                {
                    if (!rdoUpdateAll.Checked && !rdoUpdateNotFee.Checked)
                    {
                        medicineTypeSdo.UpdateSereServ = null;
                    }
                    else if (rdoUpdateNotFee.Checked)
                    {
                        medicineTypeSdo.UpdateSereServ = false;
                    }
                    else if (rdoUpdateAll.Checked)
                    {
                        medicineTypeSdo.UpdateSereServ = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void resultActiveIngredients(object[] args)
        {
            try
            {
                if (args.Count() > 0 && args[0] is List<HIS_ACTIVE_INGREDIENT>)
                {
                    this.activeIngredients = (List<HIS_ACTIVE_INGREDIENT>)args[0];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void resultActs(object[] args)
        {
            try
            {
                if (args.Count() > 0 && args[0] is List<HIS_ATC>)
                {
                    this.Acts = (List<HIS_ATC>)args[0];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadActiveIngredient()
        {
            try
            {
                btnMedicineTypeAcin_Click(null, null);
                if (this.activeIngredients != null)
                {
                    this.txtActiveIngredientCode.Text = string.Join(" + ", activeIngredients.Select(o => o.ACTIVE_INGREDIENT_CODE).ToList());
                    this.txtActiveIngredientName.Text = string.Join(" + ", activeIngredients.Select(o => o.ACTIVE_INGREDIENT_NAME).ToList());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void setEnable(bool check)
        {
            try
            {
                txtHeinLimitRatioOld.Enabled = check;
                txtHeinLimitRatio.Enabled = check;
                dtHeinLimitPriceIntrTime.Enabled = check;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void setNull()
        {
            try
            {
                txtHeinLimitRatioOld.EditValue = null;
                txtHeinLimitRatio.EditValue = null;
                dtHeinLimitPriceIntrTime.EditValue = null;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void setDataDefault(ref V_HIS_SERVICE data)
        {
            try
            {
                data.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void OnRightClick(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                List<object> listArgs = new List<object>();
                if (this.currentRightClick != null)
                {
                    listArgs.Add(this.currentRightClick);
                }
                else
                {
                    listArgs.Add(this.currentRightClickDefault);
                }

                CallModule callModule = new CallModule(CallModule.HisServiceHein, 0, 0, listArgs);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Event
        #region ---Click
        private void txtACT_Code_Click(object sender, EventArgs e)
        {
            try
            {
                LoadACTCode();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpEdit1_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboGender.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        //
        private void btnGiaTran_Click(object sender, EventArgs e)
        {
            try
            {
                V_HIS_SERVICE data = new V_HIS_SERVICE();
                setDataDefault(ref data);
                currentRightClickDefault = data;
                OnRightClick(null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtActiveIngredientCode_Click(object sender, EventArgs e)
        {
            try
            {
                LoadActiveIngredient();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnServicePaty_Click(object sender, EventArgs e)
        {
            try
            {
                List<object> listArgs = new List<object>();
                MOS.EFMODEL.DataModels.V_HIS_SERVICE service = new V_HIS_SERVICE();
                if (this.currentVHisMedicineTypeDTODefault != null || this.resultData != null)
                {
                    // get service by service_id
                    MOS.Filter.HisServiceViewFilter serviceViewFilter = new HisServiceViewFilter();
                    if (this.resultData != null)
                    {
                        serviceViewFilter.ID = this.resultData.SERVICE_ID;
                    }
                    else if (this.currentVHisMedicineTypeDTODefault != null)
                    {
                        serviceViewFilter.ID = currentVHisMedicineTypeDTODefault.SERVICE_ID;
                    }

                    var services = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE>>("api/HisService/GetView", ApiConsumer.ApiConsumers.MosConsumer, serviceViewFilter, new CommonParam());
                    if (services != null && services.Count > 0)
                    {
                        service = services.FirstOrDefault();
                    }
                }

                listArgs.Add(service);
                if (this.module == null)
                {
                    CallModule callModule = new CallModule(CallModule.HisServicePatyList, 0, 0, listArgs);
                }
                else
                {
                    CallModule callModule = new CallModule(CallModule.HisServicePatyList, this.module.RoomId, this.module.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnItem_Add_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                ADO.VHisServicePatyADO VHisSerVicePaty = new VHisServicePatyADO();
                VHisSerVicePaty.Action = GlobalVariables.ActionEdit;
                VHisSerVicePaty.BRANCH_ID = this.BranchID.ID;
                VHisSerVicePaty.BRANCH_NAME = this.BranchID.BRANCH_NAME;
                this.lsVHisServicePaty.Add(VHisSerVicePaty);
                grdControlHisServicePaty.DataSource = null;
                grdControlHisServicePaty.DataSource = this.lsVHisServicePaty;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnItem_Delete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                if (MessageBox.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    bool success = false;
                    var HisServicePaty = (ADO.VHisServicePatyADO)grdViewHisServicePaty.GetFocusedRow();
                    if (HisServicePaty != null)
                    {
                        success = true;
                        this.lsVHisServicePaty.Remove(HisServicePaty);
                        if (HisServicePaty.ID > 0)
                        {
                            HIS_SERVICE_PATY data = new HIS_SERVICE_PATY();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_PATY>(data, HisServicePaty);
                            success = new BackendAdapter(param).Post<bool>("api/HisServicePaty/Delete", ApiConsumers.MosConsumer, data, param);
                            if (success)
                            {
                                BackendDataWorker.Reset<HIS_SERVICE_PATY>();
                            }
                        }
                        grdControlHisServicePaty.DataSource = null;
                        grdControlHisServicePaty.DataSource = this.lsVHisServicePaty;
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void cboRank_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboRank.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSave.Enabled == true)
                {
                    btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceUnit_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboServiceUnit.Properties.Buttons[1].Visible = true;
                    cboServiceUnit.EditValue = null;
                    txtServiceUnitCode.Text = "";
                    txtServiceUnitCode.Focus();
                    txtServiceUnitCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataMedicine()
        {
            try
            {
                this.MedicineType = new V_HIS_MEDICINE_TYPE();
                if (cboServiceUnit.EditValue != null)
                {
                    this.MedicineType.SERVICE_UNIT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceUnit.EditValue.ToString());
                    this.MedicineType.SERVICE_UNIT_CODE = txtServiceUnitCode.Text.Trim();
                }

                if (cboMedicineUseForm.EditValue != null)
                {
                    this.MedicineType.MEDICINE_USE_FORM_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboMedicineUseForm.EditValue.ToString());
                    this.MedicineType.MEDICINE_USE_FORM_CODE = txtMedicineUseFormCode.Text.Trim();
                }

                if (cboMedicineLine.EditValue != null)
                {
                    this.MedicineType.MEDICINE_LINE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboMedicineLine.EditValue.ToString());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicineTypeParent_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                SetDataMedicine();
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMedicineTypeParent.Properties.Buttons[1].Visible = true;
                    cboMedicineTypeParent.EditValue = null;
                    txtMedicineTypeParentCode.Text = "";
                    txtMedicineTypeParentCode.Focus();
                    txtMedicineTypeParentCode.SelectAll();
                }
                else if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add((DelegateSelectData)RefreshDataToMedicineTypeParent);
                    listArgs.Add((V_HIS_MEDICINE_TYPE)this.MedicineType);
                    if (this.module == null)
                    {
                        CallModule callModule = new CallModule(CallModule.MedicineTypeCreateParent, 0, 0, listArgs);
                    }
                    else
                    {
                        CallModule callModule = new CallModule(CallModule.MedicineTypeCreateParent, this.module.RoomId, this.module.RoomTypeId, listArgs);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicineLine_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMedicineLine.Properties.Buttons[1].Visible = true;
                    cboMedicineLine.EditValue = null;
                }
                else if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add((DelegateSelectData)SetDataToMedicineLine);
                    if (this.module == null)
                    {
                        CallModule callModule = new CallModule(CallModule.HisMedicineLine, 0, 0, listArgs);
                    }
                    else
                    {
                        CallModule callModule = new CallModule(CallModule.HisMedicineLine, this.module.RoomId, this.module.RoomTypeId, listArgs);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboManufacture_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboManufacture.Properties.Buttons[1].Visible = true;
                    cboManufacture.EditValue = null;
                    txtManufactureCode.Text = "";
                    txtManufactureCode.Focus();
                    txtManufactureCode.SelectAll();
                }
                else if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add((DelegateSelectData)refreshDataToManufacturerCombo);
                    if (this.module == null)
                    {
                        CallModule callModule = new CallModule(CallModule.HisManufacturer, 0, 0, listArgs);
                    }
                    else
                    {
                        CallModule callModule = new CallModule(CallModule.HisManufacturer, this.module.RoomId, this.module.RoomTypeId, listArgs);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void refreshDataToManufacturerCombo(object manufacturer)
        {
            try
            {
                if (manufacturer != null && manufacturer is MOS.EFMODEL.DataModels.HIS_MANUFACTURER)
                {
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("MANUFACTURER_CODE", "", 50, 1));
                    columnInfos.Add(new ColumnInfo("MANUFACTURER_NAME", "", 100, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("MANUFACTURER_NAME", "ID", columnInfos, false, 150);
                    MOS.Filter.HisManufacturerFilter filter = new HisManufacturerFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    CommonParam param = new CommonParam();
                    var hisManufacturers = new BackendAdapter(param).Get<List<HIS_MANUFACTURER>>(HisRequestUriStore.HIS_MANUFACTURER_GET, ApiConsumers.MosConsumer, filter, param);
                    ControlEditorLoader.Load(cboManufacture, hisManufacturers, controlEditorADO);
                    cboManufacture.EditValue = ((MOS.EFMODEL.DataModels.HIS_MANUFACTURER)manufacturer).ID;
                    txtManufactureCode.Text = ((MOS.EFMODEL.DataModels.HIS_MANUFACTURER)manufacturer).MANUFACTURER_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void refreshDataToDosageFormCombo(object dosageform)
        {
            try
            {
                BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_DOSAGE_FORM>();
                dataDosageForm = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DOSAGE_FORM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                if (dosaFormCustom != null)
                    dataDosageForm.Add(dosaFormCustom);
                InitDosageForm();
                if (dosageform != null && dosageform is MOS.EFMODEL.DataModels.HIS_DOSAGE_FORM)
                {
                    //cboDosageForm.EditValue = ((MOS.EFMODEL.DataModels.HIS_DOSAGE_FORM)dosageform).DOSAGE_FORM_NAME;
                }
                else
                {
                    cboDosageForm.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicineUseForm_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    //txtMedicineUseFormCode

                    cboMedicineUseForm.Properties.Buttons[1].Visible = true;
                    cboMedicineUseForm.EditValue = null;
                    txtMedicineUseFormCode.Text = "";
                    txtPackingTypeCode.Focus();
                    txtPackingTypeCode.SelectAll();
                }
                else if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add((DelegateSelectData)RefreshDataToMedicineUseForm);
                    if (this.module == null)
                    {
                        CallModule callModule = new CallModule(CallModule.HisMedicineUseForm, 0, 0, listArgs);
                    }
                    else
                    {
                        CallModule callModule = new CallModule(CallModule.HisMedicineUseForm, this.module.RoomId, this.module.RoomTypeId, listArgs);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            btnSave.Focus();
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                if (txtOTHER_PAY_SOURCE.Enabled)
                    ValidationControlMaxLength(txtOTHER_PAY_SOURCE, 200, false);
                this.positionHandleControlMedicineTypeInfo = -1;
                if (!dxValidationMedicineType.Validate())
                    return;
                if (!(bool)nationalProcessor.ValidationNational(ucNational))
                    return;
                if (this.ActionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionView)
                    return;
                if (Encoding.UTF8.GetByteCount(txtProcessing.Text.Trim()) + Encoding.UTF8.GetByteCount(txtPreprocessing.Text.Trim()) > 255)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Tổng độ dài của mã sơ chế và mã phức chế không được vượt quá 255 ký tự", "Thông báo", System.Windows.Forms.MessageBoxButtons.OK);
                    return;
                }
                if (dxErrorProvider1.HasErrors) return;
                WaitingManager.Show();
                MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE currentMedicineTypeDTO = new MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE();

                UpdatePatientDTOFromDataForm(ref currentMedicineTypeDTO);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentMedicineTypeDTO), currentMedicineTypeDTO));


                if (ActionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                {
                    resultData = new BackendAdapter(param).Post<HIS_MEDICINE_TYPE>(HisRequestUriStore.HIS_MEDICINE_TYPE_CREATE, ApiConsumers.MosConsumer, currentMedicineTypeDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        if (this.depaPatientTypes != null && this.depaPatientTypes.Count > 0)
                        {
                            foreach (var item in this.depaPatientTypes)
                            {
                                item.SERVICE_ID = resultData.SERVICE_ID;
                            }
                            var createResult = new BackendAdapter(param).Post<List<HIS_DEPA_PATIENT_TYPE>>("api/HisDepaPatientType/CreateList", ApiConsumers.MosConsumer, this.depaPatientTypes, param);
                            if (createResult == null)
                            {
                                success = false;
                            }
                        }
                        BackendDataWorker.Reset<HIS_MATERIAL_TYPE>();
                        BackendDataWorker.Reset<HIS_DEPA_PATIENT_TYPE>();
                    }

                }
                else if (ActionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                {
                    MOS.SDO.HisMedicineTypeSDO medicineTypeSdo = new MOS.SDO.HisMedicineTypeSDO();
                    medicineTypeSdo.HisMedicineType = currentMedicineTypeDTO;
                    UpdateData(medicineTypeSdo);
                    resultData = new BackendAdapter(param).Post<HIS_MEDICINE_TYPE>(HisRequestUri.HIS_MEDICINE_TYPE_UPDATE_SDO, ApiConsumers.MosConsumer, medicineTypeSdo, param);

                    HisDepaPatientTypeFilter filter = new HisDepaPatientTypeFilter();
                    filter.SERVICE_ID = resultData.SERVICE_ID;
                    var oldDepaPatientTypes = new BackendAdapter(param).Get<List<HIS_DEPA_PATIENT_TYPE>>("api/HisDepaPatientType/Get", ApiConsumers.MosConsumer, filter, param);
                    if (oldDepaPatientTypes != null && oldDepaPatientTypes.Count > 0 && this.isClickPick == true)
                    {
                        var deleteResult = new BackendAdapter(param).Post<bool>("api/HisDepaPatientType/DeleteList", ApiConsumers.MosConsumer, oldDepaPatientTypes.Select(p => p.ID).ToList(), param);
                        if (!deleteResult)
                        {
                            success = false;
                        }
                    }
                    if (this.depaPatientTypes != null && this.depaPatientTypes.Count > 0)
                    {
                        foreach (var item in this.depaPatientTypes)
                        {
                            item.SERVICE_ID = resultData.SERVICE_ID;
                        }
                        var createResult = new BackendAdapter(param).Post<List<HIS_DEPA_PATIENT_TYPE>>("api/HisDepaPatientType/CreateList", ApiConsumers.MosConsumer, this.depaPatientTypes, param);
                        if (createResult == null)
                        {
                            success = false;
                        }
                    }
                    BackendDataWorker.Reset<HIS_MATERIAL_TYPE>();
                    BackendDataWorker.Reset<HIS_DEPA_PATIENT_TYPE>();

                }

                if (resultData != null)
                {
                    success = true;
                    this.currentMedicineTypeId = resultData.ID;
                    btnDieuChinhLieu.Enabled = true;
                    WaitingManager.Hide();
                    btnSave.Enabled = false;
                    btnDepartmentPatientType.Enabled = false;
                    btnRefresh.Enabled = true;
                    btnEditInfo.Enabled = true;
                    // nếu thuốc là lá mới thiết lập chính sách giá
                    txtMedicineTypeCode.Text = resultData.MEDICINE_TYPE_CODE;
                    SuccessLog(resultData);

                    BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE>();
                    InitMedicineTypeParent();
                    SendDataAfterSave();
                    MOS.Filter.HisMedicineTypeViewFilter medicineTypeViewFilter = new HisMedicineTypeViewFilter();
                    medicineTypeViewFilter.ID = resultData.ID;
                    this.currentVHisMedicineTypeDTODefault = new BackendAdapter(param).Get<List<V_HIS_MEDICINE_TYPE>>("api/HisMedicineType/GetView", ApiConsumer.ApiConsumers.MosConsumer, medicineTypeViewFilter, param).FirstOrDefault();
                    BackendDataWorker.Reset<V_HIS_SERVICE>();
                    if (this.chkIsNoHeinLimitForSpecial.Checked || (this.currentVHisMedicineTypeDTODefault != null && currentVHisMedicineTypeDTODefault.IS_NO_HEIN_LIMIT_FOR_SPECIAL == 1))
                    {
                        CommonParam pramservice = new CommonParam();
                        HisServiceViewFilter filter = new HisServiceViewFilter();
                        filter.ID = resultData.SERVICE_ID;
                        var service = new BackendAdapter(pramservice).Get<List<V_HIS_SERVICE>>("api/HisService/GetView", ApiConsumers.MosConsumer, filter, pramservice).SingleOrDefault();
                        if (service != null)
                        {
                            HisServiceSDO adoUpdate = new HisServiceSDO();
                            HIS_SERVICE ado = new HIS_SERVICE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE>(ado, service);
                            ado.IS_NO_HEIN_LIMIT_FOR_SPECIAL = this.chkIsNoHeinLimitForSpecial.Checked ? IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE : (short)0;
                            adoUpdate.HisService = ado;
                            var dataUpdateService = new BackendAdapter(param).Post<HIS_SERVICE>("api/HisService/UpdateSdo", ApiConsumers.MosConsumer, adoUpdate, param);
                        }
                        BackendDataWorker.Reset<V_HIS_SERVICE>();
                    }
                    if (Check())
                    {
                        SaveProcessorsHisServicePaty(ref param);
                    }
                    else
                    {
                        MessageBox.Show("Lưu chính sách giá thất bại, không thể lưu cùng loại đối tượng trên cùng một chi nhánh.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    SaveBlockDepartment(ref param);
                    SaveContraindication(ref param);
                }

                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }


        private void SaveContraindication(ref CommonParam param)
        {
            try
            {
                WaitingManager.Show();
                List<long> selectedContaindications = new List<long>();
                List<long> oldContraindicationIds = new List<long>();

                if (this.oldContraindicationSelecteds != null)
                {
                    oldContraindicationIds = this.oldContraindicationSelecteds;
                }

                if (this.contraindicationSelecteds != null)
                {
                    selectedContaindications = this.contraindicationSelecteds.Select(s => s.ID).Distinct().ToList();
                }

                if (selectedContaindications.Count == 0 && selectedContaindications.Count == 0)
                {
                    return;
                }

                if (selectedContaindications.Exists(e => !oldContraindicationIds.Contains(e))
                    || oldContraindicationIds.Exists(e => !selectedContaindications.Contains(e)))
                {
                    this.oldContraindicationSelecteds = new List<long>();
                    if (selectedContaindications.Count > 0)
                    {
                        this.oldContraindicationSelecteds.AddRange(selectedContaindications);
                    }
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void cboHeinServiceType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboHeinServiceType.Properties.Buttons[1].Visible = true;
                    cboHeinServiceType.EditValue = null;
                    txtManufactureCode.Focus();
                    txtManufactureCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnMedicineTypeAcin_Click(object sender, EventArgs e)
        {
            try
            {
                List<object> listArgs = new List<object>();
                MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY servicePaty = new V_HIS_SERVICE_PATY();

                if (this.currentMedicineTypeId != null && this.currentMedicineTypeId > 0)
                {
                    listArgs.Add(this.currentMedicineTypeId);
                }

                if (this.activeIngredients != null)
                {
                    List<V_HIS_MEDICINE_TYPE_ACIN> listMedicineTypeAcin = this.activeIngredients.Select(o => new V_HIS_MEDICINE_TYPE_ACIN() { ACTIVE_INGREDIENT_ID = o.ID, ACTIVE_INGREDIENT_CODE = o.ACTIVE_INGREDIENT_CODE, ACTIVE_INGREDIENT_NAME = o.ACTIVE_INGREDIENT_NAME, MEDICINE_TYPE_ID = (this.currentMedicineTypeId ?? 0) }).ToList();

                    listArgs.Add(listMedicineTypeAcin);
                }
                else
                {
                    listArgs.Add(new List<V_HIS_MEDICINE_TYPE_ACIN>());
                }

                DelegateReturnMutilObject resultActiveIngredient = resultActiveIngredients;
                listArgs.Add(resultActiveIngredient);
                if (this.module == null)
                {
                    CallModule callModule = new CallModule(CallModule.HisMedicineTypeAcin, 0, 0, listArgs);
                }
                else
                {
                    CallModule callModule = new CallModule(CallModule.HisMedicineTypeAcin, this.module.RoomId, this.module.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnACT_Click(object sender, EventArgs e)
        {
            try
            {
                List<object> listArgs = new List<object>();

                if (this.Acts != null)
                {
                    listArgs.Add(Acts);
                }
                else
                {
                    listArgs.Add(new List<HIS_ATC>());
                }

                DelegateReturnMutilObject resultAct = resultActs;
                listArgs.Add(resultAct);
                if (this.module == null)
                {
                    CallModule callModule = new CallModule(CallModule.HisATC, 0, 0, listArgs);
                }
                else
                {
                    CallModule callModule = new CallModule(CallModule.HisATC, this.module.RoomId, this.module.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtActiveIngrBhytCode_Click(object sender, EventArgs e)
        {

        }

        private void txtActiveIngredientCode_Click_1(object sender, EventArgs e)
        {
            try
            {
                LoadActiveIngredient();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnRefresh.Enabled)
                    btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                DefaultGroup();
                ResetFormData();
                positionHandleControlMedicineTypeInfo = -1;
                currentVHisMedicineTypeDTODefault = null;
                currentVHisServiceDTODefault = null;
                ActionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;// mac dinh la them
                currentMedicineTypeId = null;
                resultData = null;
                delegateSelect = null;
                activeIngrBhyts = null;
                activeIngredients = null;
                btnSave.Enabled = true;
                btnGiaTran.Enabled = false;
                this.currentRightClick = null;
                rdoWarning1.Checked = true;
                chkIsNoHeinLimitForSpecial.Checked = false;
                ChkIsSpecificHeinPrice.Checked = false;
                txtContentWarning.Enabled = false;
                txtContentWarning.Text = "";
                FillDataToGridConrolServicePaty();
                if (ucNational != null)
                {
                    nationalProcessor.Reload(ucNational, null);
                }
                txtMedicineType.Text = "";
                txtMedicineType.Enabled = false;
                cboMedicineType.EditValue = null;
                cboMedicineType.Enabled = false;
                btnDieuChinhLieu.Enabled = false;
                spinUseInTreat.EditValue = null;
                this.UseInTreat = null;
                chkWarningInTreat.Checked = true;
                GridCheckMarksSelection gridcboCauHinh = cboConfig.Properties.Tag as GridCheckMarksSelection;
                gridcboCauHinh.ClearSelection(cboConfig.Properties.View);
                cboConfig.Text = "";
                GridCheckMarksSelection gridcboThuoc = cboLoaiThuoc.Properties.Tag as GridCheckMarksSelection;
                gridcboThuoc.ClearSelection(cboConfig.Properties.View);
                cboLoaiThuoc.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RestControl()
        {
            try
            {
                DefaultGroup();
                ResetFormData();
                positionHandleControlMedicineTypeInfo = -1;
                currentVHisMedicineTypeDTODefault = null;
                currentVHisServiceDTODefault = null;
                ActionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;// mac dinh la them
                resultData = null;
                delegateSelect = null;
                activeIngrBhyts = null;
                activeIngredients = null;
                btnSave.Enabled = true;
                btnGiaTran.Enabled = false;
                this.currentRightClick = null;
                chkIsNoHeinLimitForSpecial.Checked = false;
                ChkIsSpecificHeinPrice.Checked = false;
                FillDataToGridConrolServicePaty();
                if (ucNational != null)
                {
                    nationalProcessor.Reload(ucNational, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicineGroup_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMedicineGroup.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_SERVICE)).ToString(), false);
                BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_SERVICE_PATY)).ToString(), false);
                BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_MEDICINE_TYPE)).ToString(), false);
                BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_SERVICE_ROOM)).ToString(), false);
                BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_MEDICINE_PATY)).ToString(), false);
                BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_ACIN_INTERACTIVE)).ToString(), false);
                MessageManager.Show("Xử lý thành công");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                BackendDataWorker.Reset<V_HIS_ACIN_INTERACTIVE>();
                BackendDataWorker.Reset<V_HIS_SERVICE>();
                BackendDataWorker.Reset<V_HIS_SERVICE_PATY>();
                BackendDataWorker.Reset<V_HIS_MEDICINE_TYPE>();
                BackendDataWorker.Reset<V_HIS_SERVICE_ROOM>();
                BackendDataWorker.Reset<V_HIS_MEDICINE_PATY>();
                BackendDataWorker.Reset<MedicineMaterialTypeComboADO>();
                MessageManager.Show("Xử lý thành công");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboGender_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboGender.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        public String GetTextFromClipboard()
        {
            String returnHtmlText = "";
            try
            {
                if (System.Windows.Forms.Clipboard.ContainsText(System.Windows.Forms.TextDataFormat.Text))
                {
                    returnHtmlText = System.Windows.Forms.Clipboard.GetText(System.Windows.Forms.TextDataFormat.Text);
                }
            }
            catch (Exception ex)
            {
                returnHtmlText = "";
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return returnHtmlText;
        }

        private void txtContraindication_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtUsedPart.Focus();
                    txtUsedPart.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtRecordingTransaction_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboImpUnit.Focus();
                    cboImpUnit.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void dtHSD_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dtHSD.EditValue != null)
                    {
                        txtRecordingTransaction.Focus();
                        txtRecordingTransaction.SelectAll();
                    }
                    else
                    {
                        dtHSD.Focus();
                        dtHSD.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtMedicineType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    if (!string.IsNullOrEmpty(strValue.Trim()))
                    {
                        var data = BackendDataWorker.Get<HIS_MEDICINE_TYPE>().Where(o => o.MEDICINE_TYPE_CODE.ToUpper().Trim() == strValue.ToUpper().Trim()).ToList();
                        if (data != null && data.Count == 1)
                        {
                            cboMedicineType.EditValue = data[0].ID;
                            txtMedicineType.Text = data[0].MEDICINE_TYPE_CODE;
                            txtMedicineTypeCode.Focus();
                            txtMedicineTypeCode.SelectAll();
                        }
                        else
                        {
                            cboMedicineType.Focus();
                            cboMedicineType.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cboMedicineType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(cboMedicineType.Text))
                    {
                        var listMedicine = BackendDataWorker.Get<HIS_MEDICINE_TYPE>().Where(o => o.MEDICINE_TYPE_NAME.ToUpper().Trim().Contains(cboMedicineType.Text.ToUpper().Trim())).ToList();
                        if (listMedicine != null && listMedicine.Count == 1)
                        {
                            cboMedicineType.EditValue = listMedicine[0].ID;
                            txtMedicineType.Text = listMedicine[0].MEDICINE_TYPE_NAME;
                            txtMedicineTypeCode.Focus();
                            txtMedicineTypeCode.SelectAll();
                        }
                        else
                        {
                            cboMedicineType.ShowPopup();
                            cboMedicineType.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPackingTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtManufactureCode.Focus();
                    txtManufactureCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMedicineUseFormCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadMedicineUseForm(strValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsAddictive_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsNeurological_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinUseOnDay_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinAlertMinInStock.Focus();
                    spinAlertMinInStock.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsAntibiotic_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsStopImp_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinAlertExpiredDate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceUnit_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(cboServiceUnit.Text))
                    {
                        string key = cboServiceUnit.Text.ToLower();
                        var listData = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT>().Where(o => o.SERVICE_UNIT_CODE.ToLower().Contains(key) || o.SERVICE_UNIT_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboServiceUnit.EditValue = listData.First().ID;
                            txtServiceUnitCode.Text = listData.First().SERVICE_UNIT_CODE;
                        }
                    }
                    if (!valid)
                    {
                        cboServiceUnit.Focus();
                        cboServiceUnit.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboManufacture_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(cboManufacture.Text))
                    {
                        string key = cboManufacture.Text.ToLower();
                        var listData = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MANUFACTURER>().Where(o => o.MANUFACTURER_CODE.ToLower().Contains(key) || o.MANUFACTURER_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboManufacture.EditValue = listData.First().ID;
                            txtManufactureCode.Text = listData.First().MANUFACTURER_CODE;
                            txtPackingTypeCode.Focus();
                            txtPackingTypeCode.SelectAll();
                        }
                    }
                    if (!valid)
                    {
                        cboManufacture.Focus();
                        cboManufacture.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDosageForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(cboDosageForm.Text))
                    {
                        string key = cboDosageForm.Text.ToLower();
                        var listData = dataDosageForm.Where(o => o.DOSAGE_FORM_CODE.ToLower().Contains(key) || o.DOSAGE_FORM_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboDosageForm.EditValue = listData.First().ID;
                            txtProcessing.Focus();
                            txtProcessing.SelectAll();
                        }
                    }
                    if (!valid)
                    {
                        cboDosageForm.Focus();
                        cboDosageForm.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMedicineTypeParent_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(cboServiceUnit.Text))
                    {
                        string key = cboMedicineTypeParent.Text.ToLower();
                        var listData = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE>().Where(o => o.MEDICINE_TYPE_CODE.ToLower().Contains(key) || o.MEDICINE_TYPE_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboMedicineTypeParent.EditValue = listData.First().ID;
                            txtMedicineTypeParentCode.Text = listData.First().MEDICINE_TYPE_CODE;
                            cboHeinServiceType.Focus();
                            cboHeinServiceType.SelectAll();
                        }
                    }
                    if (!valid)
                    {
                        cboMedicineTypeParent.Focus();
                        cboMedicineTypeParent.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMedicineLine_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(cboMedicineLine.Text))
                    {
                        string key = cboMedicineLine.Text.ToLower();
                        var listData = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_LINE>().Where(o => o.MEDICINE_LINE_CODE.ToLower().Contains(key) || o.MEDICINE_LINE_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboMedicineLine.EditValue = listData.First().ID;
                            txtServiceUnitCode.Focus();
                            txtServiceUnitCode.SelectAll();

                        }
                    }
                    if (!valid)
                    {
                        cboMedicineLine.Focus();
                        cboMedicineLine.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboHeinServiceType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(cboHeinServiceType.Text))
                    {
                        string key = cboHeinServiceType.Text.ToLower();
                        var listData = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE>().Where(o => o.HEIN_SERVICE_TYPE_CODE.ToLower().Contains(key) || o.HEIN_SERVICE_TYPE_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboHeinServiceType.EditValue = listData.First().ID;
                            txtHeinServiceBhytName.Focus();
                            txtHeinServiceBhytName.SelectAll();
                        }
                    }
                    if (!valid)
                    {
                        cboHeinServiceType.Focus();
                        cboHeinServiceType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkCPNG_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtProcessing.Focus();
                    txtProcessing.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChkIsSpecificHeinPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (btnGiaTran.Enabled)
                    {
                        btnGiaTran.Focus();
                    }
                    else
                    {
                        chkIsNoHeinLimitForSpecial.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void chkIsNoHeinLimitForSpecial_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboGender.Focus();
                    cboGender.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtMedicineGroup_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadMedicineGroup(strValue);
                    //spinEstimanteDuration.Focus();
                    //spinEstimanteDuration.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicineGroup_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(cboMedicineGroup.Text))
                    {
                        string key = cboMedicineGroup.Text.ToLower();
                        var listData = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_GROUP>().Where(o => o.MEDICINE_GROUP_CODE.ToLower().Contains(key) || o.MEDICINE_GROUP_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboMedicineGroup.EditValue = listData.First().ID;
                            this.nationalProcessor.FocusControl(ucNational);
                            if (listData.First().IS_AUTO_TREATMENT_DAY_COUNT == 1)
                            {
                            }
                            else if (ActionType != HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                            {
                            }
                        }
                        else if (ActionType != HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                        {
                        }
                    }
                    else if (ActionType != HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                    {
                    }
                    if (!valid)
                    {
                        cboMedicineGroup.Focus();
                        cboMedicineGroup.ShowPopup();
                        if (ActionType != HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit) ;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicineUseForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(cboMedicineGroup.Text))
                    {
                        string key = cboMedicineGroup.Text.ToLower();
                        var listData = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM>().Where(o => o.MEDICINE_USE_FORM_CODE.ToLower().Contains(key) || o.MEDICINE_USE_FORM_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboMedicineUseForm.EditValue = listData.First().ID;
                            txtMedicineUseFormCode.Text = listData.First().MEDICINE_USE_FORM_CODE;
                            txtConcentra.Focus();
                            txtConcentra.SelectAll();
                        }
                    }
                    if (!valid)
                    {
                        cboMedicineUseForm.Focus();
                        cboMedicineUseForm.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtServiceUnitCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadServiceUnit(strValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMedicineTypeParentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadMedicineTypeParent(strValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtManufactureCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadManufacturer(strValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMedicineTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMedicineTypeName.Focus();
                    txtMedicineTypeName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void txtMedicineTypeName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtScientificName.Focus();
                    txtScientificName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void txtActiveIngredientCode_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtActiveIngrBhytCode.Focus();
                    txtActiveIngrBhytCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void txtActiveIngredientName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtActiveIngrBhytCode.Focus();
                    txtActiveIngrBhytCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void txtActiveIngrBhytCode_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtActiveIngrBhytName.Focus();
                    txtActiveIngrBhytName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void txtActiveIngrBhytName_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtACT_Code.Focus();
                    txtACT_Code.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtConcentra_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtRegisterNumber.Focus();
                    txtRegisterNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtRegisterNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboMedicineGroup.Focus();
                    cboMedicineGroup.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtPackingTypeCode_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinVolume.Focus();
                    spinVolume.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtTcyNumOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBytNumOrder.Focus();
                    txtBytNumOrder.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void spinNumOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMedicineNationalCode.Focus();
                    txtMedicineNationalCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtMedicineNationalCode_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTCCL.Focus();
                    txtTCCL.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtTutorial_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPreprocessing.Focus();
                    txtPreprocessing.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ckhHoachat_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void chkFood_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void chkIsStarMark_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void chkIsVaccine_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void chkIsVitaminA_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void chkISTCMR_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtHeinLimitRatio_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtHeinLimitRatioOld.Focus();
                    txtHeinLimitRatioOld.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtHeinLimitRatioOld_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtHeinLimitPriceIntrTime.Focus();
                    dtHeinLimitPriceIntrTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void a_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dtHeinLimitPriceIntrTime.EditValue != null)
                    {
                        ChkIsSpecificHeinPrice.Focus();
                    }
                    else
                    {
                        dtHeinLimitPriceIntrTime.Focus();
                        dtHeinLimitPriceIntrTime.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinAgeFrom_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinAgeTo.Focus();
                    spinAgeTo.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void spinAgeTo_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinAlertExpiredDate.Focus();
                    spinAlertExpiredDate.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void spinAlertExpiredDate_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinAlertMinInStock.Focus();
                    spinAlertMinInStock.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void spinAlertMinInStock_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinUseOnDay.Focus();
                    spinUseOnDay.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void spinUseOnDay_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    rdoWarning.Focus();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void chkIsStopImp_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void chkCPNG_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void chkIsExprireDate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void chkIsAllowOdd_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void chkIsAllowExportOdd_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void chkIsBusiness_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinImpPrice.Focus();
                    spinImpPrice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void checkEdit30_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void chkIsKedney_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ckhIsRaw_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void chkIsExpend_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void chkIsOtherSourcePaid_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void spinImpPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinImpVatRatio.Focus();
                    spinImpVatRatio.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void spinImpVatRatio_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinInternalPrice.Focus();
                    spinInternalPrice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void spinInternalPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsSaleEqualImpPrice.Focus();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void chkIsSaleEqualImpPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinLastExpPrice.Focus();
                    spinLastExpPrice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtBytNumOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSttTT20.Focus();
                    txtSttTT20.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtHeinServiceBhytName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtActiveIngredientCode.Focus();
                    txtActiveIngredientCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void rdoUpdateNotFee_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    rdoUpdateAll.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void rdoUpdateAll_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ckhIsRaw_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboGender_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboRank.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRank_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinAgeFrom.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtACT_Code_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMedicineUseFormCode.Focus();
                    txtMedicineUseFormCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtACT_Name_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMedicineUseFormCode.Focus();
                    txtMedicineUseFormCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void spinLastExpPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinLastExpVatRatio.Focus();
                    spinLastExpVatRatio.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region ---KeyUp
        private void chkIsExprireDate_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region ---KeyDown---
        private void spinLastExpVatRatio_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region ---Close
        private void cboRank_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboGender.EditValue != null)
                    {
                        spinAgeFrom.Focus();
                        spinAgeFrom.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void textEdit27_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMedicineType.EditValue != null)
                    {
                        var medicineType = BackendDataWorker.Get<HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMedicineType.EditValue.ToString() ?? ""));
                        if (medicineType != null)
                        {
                            txtMedicineType.Text = medicineType.MEDICINE_TYPE_CODE;
                            txtMedicineTypeCode.Focus();
                            txtMedicineTypeCode.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cboServiceUnit_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboServiceUnit.EditValue != null)
                    {
                        var serviceUnit = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceUnit.EditValue ?? "").ToString()));
                        if (serviceUnit != null)
                        {
                            txtServiceUnitCode.Text = serviceUnit.SERVICE_UNIT_CODE;
                            cboServiceUnit.Properties.Buttons[1].Visible = true;
                            txtMedicineTypeParentCode.Focus();
                            txtMedicineTypeParentCode.SelectAll();
                        }
                        else
                        {
                            cboServiceUnit.Focus();
                            cboServiceUnit.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicineTypeParent_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMedicineTypeParent.EditValue != null)
                    {
                        var medicineTypeParent = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMedicineTypeParent.EditValue ?? "").ToString()));
                        if (medicineTypeParent != null)
                        {
                            txtMedicineTypeParentCode.Text = medicineTypeParent.MEDICINE_TYPE_CODE;
                            cboMedicineTypeParent.Properties.Buttons[1].Visible = true;
                            cboHeinServiceType.Focus();
                            cboHeinServiceType.SelectAll();
                        }
                        else
                        {
                            cboMedicineTypeParent.Focus();
                            cboMedicineTypeParent.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicineLine_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMedicineLine.EditValue != null)
                    {
                        var medicineLine = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_LINE>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMedicineLine.EditValue ?? "").ToString()));
                        if (medicineLine != null)
                        {
                            cboMedicineLine.Properties.Buttons[1].Visible = true;
                            txtServiceUnitCode.Focus();
                            txtServiceUnitCode.SelectAll();

                        }
                        else
                        {
                            cboMedicineLine.Focus();
                            cboMedicineLine.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboManufacture_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboManufacture.EditValue != null)
                    {
                        var manufacturer = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MANUFACTURER>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboManufacture.EditValue ?? "").ToString()));
                        if (manufacturer != null)
                        {
                            txtManufactureCode.Text = manufacturer.MANUFACTURER_CODE;
                            cboManufacture.Properties.Buttons[1].Visible = true;
                            txtPackingTypeCode.Focus();
                            txtPackingTypeCode.SelectAll();
                        }
                        else
                        {
                            cboManufacture.Focus();
                            cboManufacture.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicineUseForm_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMedicineUseForm.EditValue != null)
                    {
                        var medicineUseForm = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMedicineUseForm.EditValue ?? "").ToString()));
                        if (medicineUseForm != null)
                        {
                            txtMedicineUseFormCode.Text = medicineUseForm.MEDICINE_USE_FORM_CODE;
                            cboMedicineUseForm.Properties.Buttons[1].Visible = true;
                            txtConcentra.Focus();
                            txtConcentra.SelectAll();
                        }
                        else
                        {
                            cboMedicineUseForm.Focus();
                            cboMedicineUseForm.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHeinServiceType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboHeinServiceType.EditValue != null)
                    {
                        var medicineLine = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboHeinServiceType.EditValue ?? "").ToString()));
                        if (medicineLine != null)
                        {

                            cboHeinServiceType.Properties.Buttons[1].Visible = true;
                            txtHeinServiceBhytName.Focus();
                            txtHeinServiceBhytName.SelectAll();
                        }
                        else
                        {
                            cboHeinServiceType.Focus();
                            cboHeinServiceType.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicineGroup_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMedicineGroup.EditValue != null)
                    {
                        var medicineGroup = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_GROUP>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMedicineGroup.EditValue ?? "").ToString()));
                        if (medicineGroup != null)
                        {
                            this.nationalProcessor.FocusControl(ucNational);

                            if (medicineGroup.IS_AUTO_TREATMENT_DAY_COUNT == 1)
                            {
                            }
                            else if (ActionType != HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                            {
                            }
                        }
                        else
                        {
                            if (ActionType != HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit) ;
                            cboMedicineGroup.Focus();
                            cboMedicineGroup.ShowPopup();
                        }
                    }
                    else if (ActionType != HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboGender_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboGender.EditValue != null)
                    {
                        var data = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboGender.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            cboRank.Focus();
                            cboRank.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region ---ValidationFailed
        private void dxValidationProviderPatientInfo_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControlMedicineTypeInfo == -1)
                {
                    positionHandleControlMedicineTypeInfo = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControlMedicineTypeInfo > edit.TabIndex)
                {
                    positionHandleControlMedicineTypeInfo = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region ---Even Gridview---
        private void grdViewHisServicePaty_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
        {

        }

        private void grdControlHisServicePaty_ProcessGridKey(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Tab)
                {
                    GridControl grid = sender as GridControl;
                    GridView view = grid.FocusedView as GridView;
                    if ((e.Modifiers == Keys.None && view.IsLastRow && view.FocusedColumn.VisibleIndex == view.VisibleColumns.Count - 1) || (e.Modifiers == Keys.Shift && view.IsFirstRow && view.FocusedColumn.VisibleIndex == 0))
                    {
                        if (view.IsEditing)
                            view.CloseEditor();
                        btnSave.Focus();
                        e.Handled = true;
                    }
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    GridControl grid = sender as GridControl;
                    GridView view = grid.FocusedView as GridView;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void grdViewHisServicePaty_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                ADO.VHisServicePatyADO data = null;
                if (e.RowHandle > -1)
                {
                    data = (ADO.VHisServicePatyADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "BtnAddDelete")
                    {
                        if (data.Action == GlobalVariables.ActionAdd)
                        {
                            e.RepositoryItem = btnItem_Add;
                        }
                        else
                        {
                            e.RepositoryItem = btnItem_Delete;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void grdViewHisServicePaty_InvalidRowException(object sender, InvalidRowExceptionEventArgs e)
        {
            try
            {
                //Suppress displaying the error message box
                e.ExceptionMode = ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void grdViewHisServicePaty_ValidateRow(object sender, ValidateRowEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    GridView view = sender as GridView;
                    GridColumn onOrderCol = view.Columns["PATIENT_TYPE_ID"];
                    GridColumn onOrderColPrice = view.Columns["PRICE"];
                    GridColumn onOrderColVatRatio = view.Columns["VAT_RATIO"];
                    var data = (ADO.VHisServicePatyADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data.PATIENT_TYPE_ID < 0)
                    {
                        view.SetColumnError(onOrderCol, "Trường dữ liệu bắt buộc");
                    }

                    if (data.PRICE < 0)
                    {
                        view.SetColumnError(onOrderColPrice, "Trường dữ liệu bắt buộc");
                    }

                    if (data.VAT_RATIO < 0)
                    {
                        view.SetColumnError(onOrderColVatRatio, "Trường dữ liệu bắt buộc");
                    }
                    else
                    {
                        if (data.VAT_RATIO > 100)
                        {
                            view.SetColumnError(onOrderColVatRatio, "Giá trị trong khoảng 0-100");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region ---CheckedChanged---
        private void rdoUpdateNotFee_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.rdoUpdateNotFee.Checked && this.rdoUpdateAll.Checked)
                {
                    this.rdoUpdateAll.Checked = !this.rdoUpdateNotFee.Checked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void rdoUpdateAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.rdoUpdateNotFee.Checked && this.rdoUpdateAll.Checked)
                {
                    this.rdoUpdateNotFee.Checked = !this.rdoUpdateAll.Checked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsBusiness_CheckedChanged(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ChkIsSpecificHeinPrice_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (ChkIsSpecificHeinPrice.CheckState == CheckState.Checked)
                {
                    btnGiaTran.Enabled = true;
                    setEnable(false);
                }
                else
                {
                    btnGiaTran.Enabled = false;
                    setEnable(true);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region ---EditValueChanging---
        private void chkIsBusiness_EditValueChanging(object sender, ChangingEventArgs e)
        {
            try
            {
                if (ChkIsSpecificHeinPrice.Checked)
                {
                    chkIsSaleEqualImpPrice.Checked = true;
                    chkIsSaleEqualImpPrice.Enabled = true;
                }
                else
                {
                    chkIsSaleEqualImpPrice.Enabled = false;
                    chkIsSaleEqualImpPrice.Checked = false;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ChkIsSpecificHeinPrice_EditValueChanging(object sender, ChangingEventArgs e)
        {
            try
            {
                if (ChkIsSpecificHeinPrice.Checked)
                {
                    btnGiaTran.Enabled = false;
                    setEnable(true);
                }
                else
                {
                    btnGiaTran.Enabled = true;
                    setNull();
                    setEnable(false);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cboGender_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboGender.EditValue != null)
                {
                    cboGender.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboGender.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicineType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.currentMedicineTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboMedicineType.EditValue.ToString() ?? "");
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationMedicineType, dxErrorProvider1);
                positionHandleControlMedicineTypeInfo = -1;
                currentVHisMedicineTypeDTODefault = null;
                currentVHisServiceDTODefault = null;
                resultData = null;
                activeIngrBhyts = null;
                activeIngredients = null;
                btnSave.Enabled = true;
                btnGiaTran.Enabled = false;
                this.currentRightClick = null;
                chkIsNoHeinLimitForSpecial.Checked = false;
                ChkIsSpecificHeinPrice.Checked = false;
                FillDataToGridConrolServicePaty();
                if (ucNational != null)
                {
                    nationalProcessor.Reload(ucNational, null);
                }
                ChangeDataComboMedicineType();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        #endregion

        private void timerInitForm_Tick()
        {
            try
            {
                StopTimer(this.module.ModuleLink, timerInitForm);
                Inventec.Common.Logging.LogAction.Info(this.module.ModuleLink + ": [StopTimer - Load du lieu len cac combobox]");
                FillDataToControlsForm();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void chkIsTreatmentDayCount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cboBlockDepartment_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string display = "";
                foreach (var item in this.BlockDepartment__Seleced)
                {
                    if (display.Trim().Length > 0)
                    {
                        display += ", " + item.DEPARTMENT_NAME;
                    }
                    else
                        display = item.DEPARTMENT_NAME;
                }
                e.DisplayText = display;
                cboBlockDepartment.ToolTip = display;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBlockDepartment_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    cboOTHER_PAY_SOURCE.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SelectionGrid__BlockDepartment(object sender, EventArgs e)
        {
            try
            {
                BlockDepartment__Seleced = new List<HIS_DEPARTMENT>();
                foreach (HIS_DEPARTMENT rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        BlockDepartment__Seleced.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkAllowMissingInfoPkg_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void rdoWarning_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoWarning.Checked)
                {
                    rdoBlock.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void rdoBlock_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoBlock.Checked)
                {
                    rdoWarning.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void rdoWarning_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    rdoBlock.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void rdoBlock_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinUseInDay.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsAllowOdd_CheckedChanged(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkTachPhanBu_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSave.Enabled == true)
                {
                    btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnRefresh.Enabled)
                    btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnTCCL_Click(object sender, EventArgs e)
        {
            try
            {
                SimpleButton editor = sender as SimpleButton;
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupControlContainerTextEdit.ShowPopup(new System.Drawing.Point(buttonPosition.X - 12, buttonPosition.Bottom + 80));
                this.currentContainerClick = ContainerClick.TCCL;
                memoContainer.Text = txtTCCL.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnHDSD_Click(object sender, EventArgs e)
        {
            try
            {
                SimpleButton editor = sender as SimpleButton;
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupControlContainerTextEdit.ShowPopup(new System.Drawing.Point(buttonPosition.X - 12, buttonPosition.Bottom + 80));
                this.currentContainerClick = ContainerClick.Tutorial;
                memoContainer.Text = txtTutorial.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNote_Click(object sender, EventArgs e)
        {
            try
            {
                SimpleButton editor = sender as SimpleButton;
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupControlContainerTextEdit.ShowPopup(new System.Drawing.Point(buttonPosition.X - 12, buttonPosition.Bottom + 80));
                this.currentContainerClick = ContainerClick.Description;
                memoContainer.Text = txtDescription.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnContraindication_Click(object sender, EventArgs e)
        {
            try
            {
                SimpleButton editor = sender as SimpleButton;
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupControlContainerTextEdit.ShowPopup(new System.Drawing.Point(buttonPosition.X - 12, buttonPosition.Bottom + 80));
                this.currentContainerClick = ContainerClick.Contraindication;
                memoContainer.Text = txtContraindication.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnOKHDSD_Click(object sender, EventArgs e)
        {
            try
            {
                switch (this.currentContainerClick)
                {
                    case ContainerClick.TCCL:
                        txtTCCL.Text = memoContainer.Text;
                        break;
                    case ContainerClick.Tutorial:
                        txtTutorial.Text = memoContainer.Text;
                        break;
                    case ContainerClick.Description:
                        txtDescription.Text = memoContainer.Text;
                        break;
                    case ContainerClick.ContentWarning:
                        if (!dxValidationMedicineType.Validate(memoContainer))
                            return;
                        txtContentWarning.Text = memoContainer.Text;
                        break;
                    case ContainerClick.Contraindication:
                        txtContraindication.Text = memoContainer.Text;
                        break;
                    case ContainerClick.UsedPart:
                        txtUsedPart.Text = memoContainer.Text;
                        break;
                    case ContainerClick.DistributedAmount:
                        txtDistributedAmount.Text = memoContainer.Text;
                        break;
                    case ContainerClick.None:
                    default:
                        break;
                }
                popupControlContainerTextEdit.HidePopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCancelHDSD_Click(object sender, EventArgs e)
        {
            try
            {
                this.currentContainerClick = ContainerClick.None;
                popupControlContainerTextEdit.HidePopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPreserveCondition_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPreserveCondition.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPreserveCondition_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboPreserveCondition.EditValue != null)
                    {
                        cboDosageForm.Focus();
                        cboDosageForm.SelectAll();
                    }
                    else
                    {
                        cboPreserveCondition.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cboNguonGoc_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboNguonGoc.EditValue = null;
                }
                else if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    List<object> listArgs = new List<object>();
                    if (this.module == null)
                    {
                        CallModule callModule = new CallModule(CallModule.HisSourceMedicine, 0, 0, listArgs);
                    }
                    else
                    {
                        CallModule callModule = new CallModule(CallModule.HisSourceMedicine, this.module.RoomId, this.module.RoomTypeId, listArgs);
                    }
                    BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_SOURCE_MEDICINE>();
                    InitComboNguonGoc();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboNguonGoc_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboNguonGoc.EditValue != null)
                    {
                        txtTutorial.Focus();
                        txtTutorial.SelectAll();
                    }
                    else
                    {
                        cboNguonGoc.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cboNguonGoc_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboNguonGoc.EditValue != null)
                    {
                        txtTutorial.Focus();
                        txtTutorial.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPreserveCondition_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboPreserveCondition.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_STORAGE_CONDITION gt = BackendDataWorker.Get<HIS_STORAGE_CONDITION>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPreserveCondition.EditValue.ToString()));
                        if (gt != null)
                        {
                            txtDistributedAmount.Focus();
                            txtDistributedAmount.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region cbo trường hợp chông chỉ định
        private void InitComboContraindication()
        {
            try
            {
                HisContraindicationFilter filter = new HisContraindicationFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                this.datas = new BackendAdapter(new CommonParam()).Get<List<HIS_CONTRAINDICATION>>("api/HisContraindication/Get", ApiConsumers.MosConsumer, filter, null);

                cboContraindication.Properties.DataSource = this.datas;
                cboContraindication.Properties.DisplayMember = "CONTRAINDICATION_NAME";
                cboContraindication.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cboContraindication.Properties.View.Columns.AddField("CONTRAINDICATION_NAME");

                col2.VisibleIndex = 1;
                col2.Width = 350;
                col2.Caption = "Tất cả";
                cboContraindication.Properties.PopupFormWidth = 350;
                cboContraindication.Properties.View.OptionsView.ShowColumnHeaders = true;
                cboContraindication.Properties.View.OptionsSelection.MultiSelect = true;
                cboContraindication.Properties.View.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DefaultBoolean.True;

                GridCheckMarksSelection gridCheckMark = cboContraindication.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(cboContraindication.Properties.DataSource);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitContraindicationCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboContraindication.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__Contraindication);
                cboContraindication.Properties.Tag = gridCheck;
                cboContraindication.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboContraindication.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboContraindication.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__Contraindication(object sender, EventArgs e)
        {
            try
            {
                contraindicationSelecteds = new List<HIS_CONTRAINDICATION>();
                foreach (MOS.EFMODEL.DataModels.HIS_CONTRAINDICATION rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        contraindicationSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboContraindication_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    cboDosageForm.Focus();
                    cboDosageForm.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboContraindication_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string display = "";
                foreach (var item in this.contraindicationSelecteds)
                {
                    if (display.Trim().Length > 0)
                    {
                        display += ", " + item.CONTRAINDICATION_NAME;
                    }
                    else
                        display = item.CONTRAINDICATION_NAME;
                }
                e.DisplayText = display;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillContraindication()
        {
            try
            {
                if (this.currentMedicineTypeId.HasValue && this.currentMedicineTypeId.Value > 0 && this.ActionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                {
                    this.oldContraindicationSelecteds = new List<long>();
                    HisMedicineTypeFilter filter = new HisMedicineTypeFilter();
                    filter.ID = this.currentMedicineTypeId.Value;

                    List<HIS_MEDICINE_TYPE> datas = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDICINE_TYPE>>("api/HisMedicineType/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null);

                    HIS_MEDICINE_TYPE data = datas.FirstOrDefault();

                    if (!String.IsNullOrEmpty(data.CONTRAINDICATION_IDS))
                    {
                        string[] str = data.CONTRAINDICATION_IDS.Split(',');

                        foreach (var item in str)
                        {
                            long p = long.Parse(item);
                            this.oldContraindicationSelecteds.Add(p);
                        }
                    }
                }
                GridCheckMarksSelection gridCheckMark = cboContraindication.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboContraindication.Properties.View);
                    if (this.oldContraindicationSelecteds != null && this.oldContraindicationSelecteds.Count > 0)
                    {
                        List<HIS_CONTRAINDICATION> seleceds = this.datas.Where(o => this.oldContraindicationSelecteds.Contains(o.ID)).ToList();
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => seleceds), seleceds));
                        gridCheckMark.SelectAll(seleceds);

                        string displayText = String.Join(", ", seleceds.Select(s => s.CONTRAINDICATION_NAME).ToList());
                        cboContraindication.Text = displayText;
                        Inventec.Common.Logging.LogSystem.Debug("this.oldContraindicationSelecteds.Count " + this.oldContraindicationSelecteds.Count);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void cboContraindication_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDosageForm.Focus();
                    cboDosageForm.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void checkIsOxygen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void chkIsOutHospital_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtHeinLimitRatio.Focus();
                    txtHeinLimitRatio.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cboImpUnit_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboImpUnit.EditValue != null)
                    spUnitConvertRatio.Enabled = true;
                else
                {
                    spUnitConvertRatio.Enabled = false;
                    spUnitConvertRatio.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }

        }

        private void cboImpUnit_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboImpUnit.EditValue = null;
                    spUnitConvertRatio.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spUnitConvertRatio_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void cboImpUnit_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void spUnitConvertRatio_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (spUnitConvertRatio.Enabled == true)
                {
                    if (!String.IsNullOrWhiteSpace(spUnitConvertRatio.Text))
                    {
                        if (spUnitConvertRatio.Text.IndexOf('0') == spUnitConvertRatio.Text.Length - 1)
                        {
                            string a = spUnitConvertRatio.Text.Substring(0, spUnitConvertRatio.Text.Length - 1);
                            spUnitConvertRatio.Text = a;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void txtScientificName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboMedicineLine.Focus();
                    cboMedicineLine.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPreprocessing_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtPreprocessing.Text.Trim()))
                    {
                        txtPreprocessingName.Focus();
                        txtPreprocessingName.SelectAll();
                    }
                    else
                    {
                        List<HIS_PROCESSING_METHOD> lstCor = new List<HIS_PROCESSING_METHOD>();
                        List<string> lstF = new List<string>();
                        var splText = txtPreprocessing.Text.Trim().Split(';');
                        foreach (var item in splText)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                var pre = lstPreProcessing.FirstOrDefault(o => o.PROCESSING_METHOD_CODE.Equals(item));
                                if (pre != null)
                                {
                                    lstCor.Add(pre);
                                }
                                else
                                {
                                    lstF.Add(item);
                                }
                            }
                        }
                        if (lstCor != null && lstCor.Count > 0)
                        {
                            txtPreprocessing.Text = string.Join(";", lstCor.Select(o => o.PROCESSING_METHOD_CODE));
                            txtPreprocessingName.Text = string.Join(";", lstCor.Select(o => o.PROCESSING_METHOD_NAME));
                        }
                        else
                        {
                            txtPreprocessing.Text = txtPreprocessingName.Text = null;
                        }
                        if (lstF != null && lstF.Count > 0)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Dữ liệu không có trong danh mục phương pháp chế biến. Mã {0}", string.Join(";", lstF)), "Thông báo", System.Windows.Forms.MessageBoxButtons.OK);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtProcessing_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtProcessing.Text.Trim()))
                    {
                        txtProcessingName.Focus();
                        txtProcessingName.SelectAll();
                    }
                    else
                    {
                        List<HIS_PROCESSING_METHOD> lstCor = new List<HIS_PROCESSING_METHOD>();
                        List<string> lstF = new List<string>();
                        var splText = txtProcessing.Text.Trim().Split(';');
                        foreach (var item in splText)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                var pre = lstProcessing.FirstOrDefault(o => o.PROCESSING_METHOD_CODE.Equals(item));
                                if (pre != null)
                                {
                                    lstCor.Add(pre);
                                }
                                else
                                {
                                    lstF.Add(item);
                                }
                            }
                        }
                        if (lstCor != null && lstCor.Count > 0)
                        {
                            txtProcessing.Text = string.Join(";", lstCor.Select(o => o.PROCESSING_METHOD_CODE));
                            txtProcessingName.Text = string.Join(";", lstCor.Select(o => o.PROCESSING_METHOD_NAME));
                        }
                        else
                        {
                            txtProcessing.Text = txtProcessingName.Text = null;
                        }
                        if (lstF != null && lstF.Count > 0)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Dữ liệu không có trong danh mục phương pháp chế biến. Mã {0}", string.Join(";", lstF)), "Thông báo", System.Windows.Forms.MessageBoxButtons.OK);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnMemoProcessing_Click(object sender, EventArgs e)
        {
            try
            {
                SimpleButton editor = sender as SimpleButton;
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupControlContainerTextEdit.ShowPopup(new System.Drawing.Point(buttonPosition.X - 12, buttonPosition.Bottom + 80));
                this.currentContainerClick = ContainerClick.Processing;
                memoContainer.Text = txtProcessing.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtUsedPart_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboContraindication.Focus();
                    cboContraindication.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnMemoUsedPart_Click(object sender, EventArgs e)
        {
            try
            {
                SimpleButton editor = sender as SimpleButton;
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupControlContainerTextEdit.ShowPopup(new System.Drawing.Point(buttonPosition.X - 12, buttonPosition.Bottom + 80));
                this.currentContainerClick = ContainerClick.UsedPart;
                memoContainer.Text = txtUsedPart.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDosageForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboPreserveCondition.Focus();
                    cboPreserveCondition.SelectAll();
                    cboPreserveCondition.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtDistributedAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtProcessing.Focus();
                    txtProcessing.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnMemoDistributedAmount_Click(object sender, EventArgs e)
        {
            try
            {
                SimpleButton editor = sender as SimpleButton;
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupControlContainerTextEdit.ShowPopup(new System.Drawing.Point(buttonPosition.X - 12, buttonPosition.Bottom + 80));
                this.currentContainerClick = ContainerClick.DistributedAmount;
                memoContainer.Text = txtDistributedAmount.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsBusiness_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void chkGayTe_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void chKNgoaiDRG_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsNotTreatmentDayCount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsTreatmentDayCount_CheckedChanged(object sender, EventArgs e)
        {

            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsNotTreatmentDayCount_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboOTHER_PAY_SOURCE_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboOTHER_PAY_SOURCE.EditValue != null)
                {
                    cboOTHER_PAY_SOURCE.Properties.Buttons[1].Visible = true;
                    txtOTHER_PAY_SOURCE.Enabled = true;
                    txtBhytWhiteListCodes.Enabled = true;txtBhytWhiteListCodes.Text = null;
                    if (txtOTHER_PAY_SOURCE.Enabled)
                        ValidationControlMaxLength(txtOTHER_PAY_SOURCE, 200, false);
                    if (txtBhytWhiteListCodes.Enabled)
                        ValidationControlMaxLength(txtBhytWhiteListCodes, 500, false);
                }
                else
                {
                    cboOTHER_PAY_SOURCE.Properties.Buttons[1].Visible = false;
                    txtOTHER_PAY_SOURCE.Enabled = false;
                    txtOTHER_PAY_SOURCE.Text = null;
                    txtBhytWhiteListCodes.Enabled = false;
                    txtBhytWhiteListCodes.Text = null;
                    dxValidationMedicineType.SetValidationRule(txtOTHER_PAY_SOURCE, null);
                    dxValidationMedicineType.SetValidationRule(txtBhytWhiteListCodes, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboOTHER_PAY_SOURCE_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboOTHER_PAY_SOURCE.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboOTHER_PAY_SOURCE_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboOTHER_PAY_SOURCE.EditValue != null)
                    {
                        txtOTHER_PAY_SOURCE.Focus();
                        txtOTHER_PAY_SOURCE.SelectAll();
                    }
                    else
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtOTHER_PAY_SOURCE_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }



            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTCCL_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboNguonGoc.EditValue != null)
                    {
                        cboNguonGoc.Focus();
                        cboNguonGoc.SelectAll();
                    }
                    else
                    {
                        cboNguonGoc.Focus();
                        cboNguonGoc.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //qtcode chú ý
        private void cboMedicineLine_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboMedicineLine.EditValue != null)
                {
                    var medicineLine = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_LINE>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMedicineLine.EditValue ?? "").ToString()));
                    if (medicineLine != null)
                    {
                        cboMedicineLine.Properties.Buttons[1].Visible = true;
                        ValidatecboMedicineUseForm(medicineLine.DO_NOT_REQUIRED_USE_FORM != 1);
                        if (medicineLine.ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_LINE.ID__VT_YHCT)
                        {
                            dxValidationMedicineType.SetValidationRule(cboDosageForm, null);
                            ValidatecboDosageForm(true);
                        }
                        else
                        {
                            dxValidationMedicineType.SetValidationRule(cboDosageForm, null);
                            ValidatecboDosageForm(false);
                        }
                    }                    
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicineUseForm_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboMedicineUseForm.EditValue != null)
                {
                    var medicineUseForm = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMedicineUseForm.EditValue ?? "").ToString()));
                    if (medicineUseForm != null)
                    {
                        cboMedicineUseForm.Properties.Buttons[1].Visible = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grdViewHisServicePaty_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ADO.VHisServicePatyADO data = (ADO.VHisServicePatyADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "FROM_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.FROM_TIME ?? 0);
                            data.STT = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "TO_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.TO_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemDateEditToTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                var data = (ADO.VHisServicePatyADO)grdViewHisServicePaty.GetFocusedRow();
                var dtToTime = sender as DateEdit;
                if (e.CloseMode == PopupCloseMode.Normal && dtToTime.DateTime != DateTime.MinValue)
                {
                    dtToTime.DateTime = dtToTime.DateTime.Date.AddHours(23).AddMinutes(59);
                }
                foreach (var item in lsVHisServicePaty)
                {
                    if (item.STT == data.STT)
                    {
                        item.DT_TO_TIME = dtToTime.DateTime;
                        item.TO_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(item.DT_TO_TIME);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemDateEditFromTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                var data = (ADO.VHisServicePatyADO)grdViewHisServicePaty.GetFocusedRow();
                var dtFromTime = sender as DateEdit;
                foreach (var item in lsVHisServicePaty)
                {
                    if (item.STT == data.STT)
                    {
                        item.DT_FROM_TIME = dtFromTime.DateTime;
                        item.FROM_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(item.DT_FROM_TIME);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemDateEditFromTime_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var dtFromTime = sender as DateEdit;
                    var data = (ADO.VHisServicePatyADO)grdViewHisServicePaty.GetFocusedRow();
                    foreach (var item in lsVHisServicePaty)
                    {
                        if (item.STT == data.STT)
                        {
                            item.DT_FROM_TIME = dtFromTime.DateTime;
                            item.FROM_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(item.DT_FROM_TIME);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemDateEditFromTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var dtFromTime = sender as DateEdit;
                    var data = (ADO.VHisServicePatyADO)grdViewHisServicePaty.GetFocusedRow();
                    foreach (var item in lsVHisServicePaty)
                    {
                        if (item.STT == data.STT)
                        {
                            item.DT_FROM_TIME = dtFromTime.DateTime;
                            item.FROM_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(item.DT_FROM_TIME);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemDateEditToTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var dtToTime = sender as DateEdit;
                    var data = (ADO.VHisServicePatyADO)grdViewHisServicePaty.GetFocusedRow();
                    foreach (var item in lsVHisServicePaty)
                    {
                        if (item.STT == data.STT)
                        {
                            item.DT_TO_TIME = dtToTime.DateTime;
                            item.TO_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(item.DT_TO_TIME);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemDateEditFromTime_Leave(object sender, EventArgs e)
        {
            try
            {
                var dtFromTime = sender as DateEdit;
                var data = (ADO.VHisServicePatyADO)grdViewHisServicePaty.GetFocusedRow();
                foreach (var item in lsVHisServicePaty)
                {
                    if (item.STT == data.STT)
                    {
                        item.DT_FROM_TIME = dtFromTime.DateTime;
                        item.FROM_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(item.DT_FROM_TIME);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemDateEditToTime_Leave(object sender, EventArgs e)
        {
            try
            {
                var data = (ADO.VHisServicePatyADO)grdViewHisServicePaty.GetFocusedRow();
                var dtToTime = sender as DateEdit;
                foreach (var item in lsVHisServicePaty)
                {
                    if (item.STT == data.STT)
                    {
                        item.DT_TO_TIME = dtToTime.DateTime;
                        item.TO_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(item.DT_TO_TIME);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPreprocessingName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    Popup.frmProcessingMethod frm = new Popup.frmProcessingMethod(GetPreprocessing, false);
                    frm.ShowDialog();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    txtProcessing.Focus();
                    txtProcessing.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetPreprocessing(List<HIS_PROCESSING_METHOD> obj)
        {
            try
            {
                if (obj != null && obj.Count > 0)
                {
                    txtPreprocessing.Text = String.Join(";", obj.Select(o => o.PROCESSING_METHOD_CODE).ToList());
                    txtPreprocessingName.Text = String.Join(";", obj.Select(o => o.PROCESSING_METHOD_NAME).ToList());
                }
                else
                {
                    txtPreprocessing.Text = txtPreprocessingName.Text = String.Empty;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtProcessingName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    Popup.frmProcessingMethod frm = new Popup.frmProcessingMethod(GetProcessing, true);
                    frm.ShowDialog();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    dtHSD.Focus();
                    dtHSD.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetProcessing(List<HIS_PROCESSING_METHOD> obj)
        {
            try
            {
                if (obj != null && obj.Count > 0)
                {
                    txtProcessing.Text = String.Join(";", obj.Select(o => o.PROCESSING_METHOD_CODE).ToList());
                    txtProcessingName.Text = String.Join(";", obj.Select(o => o.PROCESSING_METHOD_NAME).ToList());
                }
                else
                {
                    txtProcessing.Text = txtProcessingName.Text = String.Empty;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSttTT20_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinNumOrder.Focus();
                    spinNumOrder.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void spinUseInDay_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    rdoBlock1.Focus();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void rdoWarning1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    rdoBlock1.Focus();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void rdoBlock1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboBlockDepartment.Focus();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void spinUseInDay_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                e.Handled = true;
        }

        private void rdoWarning1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoWarning1.Checked)
                {
                    rdoBlock1.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void rdoBlock1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoBlock1.Checked)
                {
                    rdoWarning1.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBlockDepartment_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboOTHER_PAY_SOURCE.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDosageForm_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDosageForm.EditValue = null;
                }
                else if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add((DelegateSelectData)refreshDataToDosageFormCombo);
                    if (this.module == null)
                    {
                        CallModule callModule = new CallModule(CallModule.HisDosageForm, 0, 0, listArgs);
                    }
                    else
                    {
                        CallModule callModule = new CallModule(CallModule.HisDosageForm, this.module.RoomId, this.module.RoomTypeId, listArgs);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDosageForm_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDosageForm.EditValue != null)
                    {
                        var dosageform = dataDosageForm.SingleOrDefault(o => o.DOSAGE_FORM_NAME == (cboDosageForm.EditValue ?? "").ToString());
                        if (dosageform != null)
                        {
                            cboDosageForm.Properties.Buttons[1].Visible = true;
                            txtPreprocessing.Focus();
                            txtPreprocessing.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHowToUse_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboHowToUse.EditValue = null;
                }
                else if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    List<object> listArgs = new List<object>();
                    if (this.module == null)
                    {
                        CallModule callModule = new CallModule(CallModule.HisHowToUse, 0, 0, listArgs);
                    }
                    else
                    {
                        CallModule callModule = new CallModule(CallModule.HisHowToUse, this.module.RoomId, this.module.RoomTypeId, listArgs);
                    }

                    BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_HTU>();
                    InitHowToUse();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void cboHowToUse_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboHowToUse.EditValue != null)
                    {
                        var howtouse = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_HTU>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboHowToUse.EditValue ?? "").ToString()));
                        if (howtouse != null)
                        {
                            cboHowToUse.Properties.Buttons[1].Visible = true;
                            txtMedicineNationalCode.Focus();
                            txtMedicineNationalCode.SelectAll();
                        }
                        else
                        {
                            cboHowToUse.Focus();
                            cboHowToUse.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHowToUse_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(cboHowToUse.Text))
                    {
                        string key = cboHowToUse.Text.ToLower();
                        var listData = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_HTU>().Where(o => o.HTU_CODE.ToLower().Contains(key) || o.HTU_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboHowToUse.EditValue = listData.First().ID;
                            txtMedicineNationalCode.Focus();
                            txtMedicineNationalCode.SelectAll();
                        }
                    }
                    if (!valid)
                    {
                        cboHowToUse.Focus();
                        cboHowToUse.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnContentWarning_Click(object sender, EventArgs e)
        {
            try
            {
                SimpleButton editor = sender as SimpleButton;
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupControlContainerTextEdit.ShowPopup(new System.Drawing.Point(layoutControlItem400.Location.X + editor.Bounds.X , buttonPosition.Bottom + popupControlContainerTextEdit.Size.Height));
                this.currentContainerClick = ContainerClick.ContentWarning;
                memoContainer.Text = txtContentWarning.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDosageForm_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboDosageForm.EditValue != null)
                {
                    var medicineUseForm = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DOSAGE_FORM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDosageForm.EditValue ?? "").ToString()));
                    if (medicineUseForm != null)
                    {
                        cboDosageForm.Properties.Buttons[1].Visible = true;
                    }
                }
                else
                {
                    cboDosageForm.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHowToUse_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboHowToUse.EditValue != null)
                {
                    cboHowToUse.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboHowToUse.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDieuChinhLieu_Click(object sender, EventArgs e)
        {
            try
            {
                frmDieuChinhLieu frm = new frmDieuChinhLieu(module, this.currentMedicineTypeId);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNguonGoc_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboNguonGoc.Properties.Buttons[1].Visible = cboNguonGoc.EditValue != null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkGenneric_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void chkBiologic_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void chkOriginal_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtHeinLimitRatio.Focus();
                    txtHeinLimitRatio.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void spinVolume_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTcyNumOrder.Focus();
                    txtTcyNumOrder.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnEditInfo_Click(object sender, EventArgs e)
        {
            try
            {
                HIS.Desktop.ADO.ProductInfoADO ado = new ProductInfoADO();
                ado.MedicineTypeId = this.currentMedicineTypeId??0;
                ado.ProductInfoOpen = 1;
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisProductInfo").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisProductInfo");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(module);
                    listArgs.Add(ado);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.module.RoomId, this.module.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        public decimal? UseInTreat { get; set; }
        
        private void spinUseInTreat_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if(spinUseInTreat.EditValue == null || Convert.ToDecimal(spinUseInTreat.EditValue) == 0)
                {
                    this.UseInTreat = null;
                }
                if(Convert.ToDecimal(spinUseInTreat.EditValue) < 0)
                {
                    dxErrorProvider1.SetError(spinUseInTreat, "Số lượng không hợp lệ",DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning);
                }
                else
                {
                    if (Convert.ToDecimal(spinUseInTreat.EditValue) > 0) UseInTreat = Convert.ToDecimal(spinUseInTreat.EditValue);
                    dxErrorProvider1.SetError(spinUseInTreat, "", DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void chkWarningInTreat_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                
                chkBlockInTreat.Checked = !chkWarningInTreat.Checked;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void chkBlockInTreat_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                
                chkWarningInTreat.Checked = !chkBlockInTreat.Checked;
                
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void simpleButton24_Click(object sender, EventArgs e)
        {
            var serviceId = BackendDataWorker.Get<HIS_MEDICINE_TYPE>().Where(p => p.ID == this.currentMedicineTypeId).Select(p => p.SERVICE_ID).FirstOrDefault();

            frmDepartmentPatientType frm = serviceId != 0
                ? new frmDepartmentPatientType(serviceId, this.depaPatientTypes ?? new List<HIS_DEPA_PATIENT_TYPE>(), this.isCalledApi, this.isClickPick)
                : new frmDepartmentPatientType(this.depaPatientTypes ?? new List<HIS_DEPA_PATIENT_TYPE>());

            frm.OnDepaPatientTypeSaved += Frm_OnDepaPatientTypeSaved;
            frm.Show();
        }

        private void Frm_OnDepaPatientTypeSaved(List<HIS_DEPA_PATIENT_TYPE> depaPatientTypes, bool isCalledApi, bool isClickPick)
        {
            try
            {
                this.isCalledApi = isCalledApi;
                this.depaPatientTypes = depaPatientTypes != null ? depaPatientTypes : new List<HIS_DEPA_PATIENT_TYPE>();
                this.isClickPick = isClickPick;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboLoaiThuoc_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string roomName = "";
                if (this.lstMedicine != null && this.lstMedicine.Count > 0)
                {
                    foreach (var item in this.lstMedicine)
                    {
                        roomName += item.NAME + ",";

                    }
                }
                e.DisplayText = roomName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
        }

        private void InitComboLoaiThuocCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboLoaiThuoc.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(Event_Check);
                cboLoaiThuoc.Properties.Tag = gridCheck;
                cboLoaiThuoc.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboLoaiThuoc.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboLoaiThuoc.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboConfigCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboConfig.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(Event_Check_1);
                cboConfig.Properties.Tag = gridCheck;
                cboConfig.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboConfig.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboConfig.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void Event_Check_1(object sender, EventArgs e)
        {
            if (_isReloadingConfigCombo) return; // Prevent recursion

            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                bool hasKD = false;
                bool hasAllowOdd = false; // Track if ID=4 is checked

                if (gridCheckMark != null)
                {
                    // Lấy selection hiện tại
                    var currentSelection = gridCheckMark.Selection.OfType<AConfigADO>().ToList();

                    // Định nghĩa ràng buộc
                    Dictionary<string, string> dependencies = new Dictionary<string, string>
            {
                {"Thuốc quầy thuốc", "Thuốc kinh doanh"}
            };

                    // Validate và loại bỏ selection không hợp lệ
                    var validSelection = new List<AConfigADO>();
                    var itemsToRemove = new List<AConfigADO>();

                    foreach (var item in currentSelection)
                    {
                        bool isValid = true;

                        // Kiểm tra ràng buộc
                        if (dependencies.ContainsKey(item.NAME))
                        {
                            string parentName = dependencies[item.NAME];
                            bool parentExists = currentSelection.Any(x => x.NAME == parentName);
                            if (!parentExists)
                            {
                                isValid = false;
                                itemsToRemove.Add(item);
                            }
                        }

                        if (isValid)
                        {
                            validSelection.Add(item);
                        }
                    }

                    // Remove invalid items từ GridCheckMarksSelection
                    foreach (var item in itemsToRemove)
                    {
                        gridCheckMark.Selection.Remove(item);
                    }

                    // CẬP NHẬT lstConfig từ validSelection
                    this.lstConfig = new List<AConfigADO>();
                    foreach (var er in validSelection)
                    {
                        if (sb.Length > 0) 
                        {                             
                            sb.Append(", ");
                        }

                        if (er.ID == 15) hasKD = true;
                        if (er.ID == 4) hasAllowOdd = true; // Check for ID=4
                        sb.Append(er.NAME);

                        lstConfig.Add(er);
                    }
                }
                
                // UPDATE TEXT
                string displayText = sb.ToString();
                cboConfig.Text = displayText;
                
                // Force refresh để update UI
                if (cboConfig.Properties.View != null)
                {
                    cboConfig.Properties.View.RefreshData();
                }
                
                if (hasKD)
                {
                    chkIsSaleEqualImpPrice.Enabled = false;
                    chkIsSaleEqualImpPrice.Checked = false;
                    chkKD = true;
                    FillDataToGridConrolServicePaty();
                }
                else
                {
                    chkIsSaleEqualImpPrice.Enabled = true;
                    chkIsSaleEqualImpPrice.Checked = true;
                    chkKD = false;
                    FillDataToGridConrolServicePaty();
                }
                if (hasAllowOdd)
                {
                    txtContentWarning.Enabled = true;
                    btnContentWarning.Enabled = true;
                }
                else
                {
                    txtContentWarning.Enabled = false;
                    txtContentWarning.Text = "";
                    btnContentWarning.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Event_Check(object sender, EventArgs e)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                
                if (gridCheckMark != null)
                {
                    // Lấy selection hiện tại từ GridCheckMarksSelection
                    var currentSelection = gridCheckMark.Selection.OfType<AMedicineTypeADO>().ToList();
                    
                    // CẬP NHẬT lstMedicine từ currentSelection
                    this.lstMedicine = new List<AMedicineTypeADO>();
                    foreach (AMedicineTypeADO er in currentSelection)
                    {
                        if (er != null)
                        {
                            if (sb.Length > 0) { sb.Append(", "); }
                            sb.Append(er.NAME);
                            this.lstMedicine.Add(er);
                        }
                    }
                }
                
                // UPDATE TEXT
                this.cboLoaiThuoc.Text = sb.ToString();
                
                // Force refresh để update UI
                if (cboLoaiThuoc.Properties.View != null)
                {
                    cboLoaiThuoc.Properties.View.RefreshData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<AMedicineTypeADO> listHisSuimIndexDefault = new List<AMedicineTypeADO>();
        private void ProcessSelectBS(string p, GridCheckMarksSelection gridCheckMark)
        {
            try
            {
                List<AMedicineTypeADO> ds = cboLoaiThuoc.Properties.DataSource as List<AMedicineTypeADO>;
                if (ds == null || ds.Count == 0)
                {
                    ds = getListAMedicineType();

                    // Gán lại datasource để GridLookup có dữ liệu
                    cboLoaiThuoc.Properties.DataSource = ds;
                }
                string[] arrays = p.Split(',');
                if (arrays != null && arrays.Length > 0)
                {
                    List<AMedicineTypeADO> selects = new List<AMedicineTypeADO>();
                    foreach (var item in arrays)
                    {
                        string nameTrim = item.Trim();
                        var row = ds.FirstOrDefault(o => o.NAME == nameTrim);
                        if (row != null)
                        {
                            selects.Add(row);
                            listHisSuimIndexDefault.Add(row);
                        }
                    }
                    gridCheckMark.SelectAll(selects);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<AConfigADO> listConfigDefault = new List<AConfigADO>();
        private void ProcessSelectConfig(string p, GridCheckMarksSelection gridCheckMark)
        {
            try
            {
                List<AConfigADO> ds = cboLoaiThuoc.Properties.DataSource as List<AConfigADO>;
                if (ds == null || ds.Count == 0)
                {
                    ds = getConfigType();

                    // Gán lại datasource để GridLookup có dữ liệu
                    cboConfig.Properties.DataSource = ds;
                }
                string[] arrays = p.Split(',');
                if (arrays != null && arrays.Length > 0)
                {
                    List<AConfigADO> selects = new List<AConfigADO>();
                    foreach (var item in arrays)
                    {
                        string nameTrim = item.Trim();
                        var row = ds.FirstOrDefault(o => o.NAME == nameTrim);
                        if (row != null)
                        {
                            selects.Add(row);
                            listConfigDefault.Add(row);
                        }
                    }
                    gridCheckMark.SelectAll(selects);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboConfig_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string roomName = "";
                if (this.lstConfig != null && this.lstConfig.Count > 0)
                {
                    foreach (var item in this.lstConfig)
                    {
                        roomName += item.NAME + ",";

                    }
                }
                e.DisplayText = roomName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
        }

        private void cboConfig_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtActiveIngrBhytCode_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if(txtActiveIngrBhytCode.EditValue != null && txtActiveIngrBhytCode.EditValue != "")
                {
                    ValidatecboDosageForm(true);
                }
                else
                {
                    dxValidationMedicineType.SetValidationRule(cboDosageForm, null);
                    ValidatecboDosageForm(false);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

