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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisCareLevel
{
    public partial class frmCareLevel : HIS.Desktop.Utility.FormBase
    {
        #region Declare

        /// <summary>So ban ghi tra ve o trang hien tai</summary>
        int rowCount = 0;

        /// <summary>Tong so ban ghi khop dieu kien tim kiem (do backend tra ve)</summary>
        int dataTotal = 0;

        /// <summary>Chi so ban ghi dau tien cua trang hien tai - dung de danh so cot STT</summary>
        int startPage = 0;

        /// <summary>GlobalVariables.ActionAdd / GlobalVariables.ActionEdit</summary>
        int ActionType = -1;

        /// <summary>TabIndex cua control dang giu loi validate dau tien</summary>
        int positionHandle = -1;

        /// <summary>Ban ghi dang duoc chon tren luoi</summary>
        HIS_CARE_LEVEL currentData;

        /// <summary>
        /// Bat khi dang gan DataSource cho luoi. Gan DataSource lam GridView ban su kien
        /// FocusedRowChanged, neu khong chan thi form editor bi do lai du lieu dong dau tien
        /// va ActionType bi keo ve che do Sua ngay sau khi vua Them moi xong.
        /// </summary>
        bool isLoadingData = false;

        /// <summary>
        /// Bat trong luc goi ucPaging.Init. UcPaging.Init gan txtPageSize.EditValue TRUOC khi
        /// tao PagingGrid moi, lam ComboBoxEdit ban SelectedIndexChanged trong khi PagingGrid cu
        /// van con song -> thu vien goi lai LoadPaging voi Start cua trang CU. Hau qua: sau khi
        /// bam Tim kiem tu trang >= 2, luoi hien nham trang cua ket qua moi va API bi goi 2 lan.
        /// </summary>
        bool isInitPaging = false;

        Inventec.Desktop.Common.Modules.Module moduleData;

        #endregion

        #region Construct

        public frmCareLevel(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                this.moduleData = moduleData;
                gridControlCareLevel.ToolTipController = toolTipControllerGrid;

                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Public method

        public void MeShow()
        {
            try
            {
                //Gan gia tri mac dinh
                SetDefaultValue();

                //Set enable control default
                EnableControlChanged(this.ActionType);

                //Load du lieu
                FillDataToGridControl();

                //Load ngon ngu label control
                SetCaptionByLanguageKey();

                //Set validate rule
                ValidateForm();

                //Focus default
                SetDefaultFocus();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Private method

        private void frmCareLevel_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                HIS.Desktop.Plugins.HisCareLevel.Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisCareLevel.Resources.Lang", typeof(frmCareLevel).Assembly);
                ResourceManager res = HIS.Desktop.Plugins.HisCareLevel.Resources.ResourceLanguageManager.LanguageResource;

                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmCareLevel.btnSearch.Text", res, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmCareLevel.btnAdd.Text", res, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmCareLevel.btnEdit.Text", res, LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmCareLevel.btnCancel.Text", res, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmCareLevel.txtKeyword.Properties.NullValuePrompt", res, LanguageManager.GetCulture());

                this.lciCareLevelCode.Text = Inventec.Common.Resource.Get.Value("frmCareLevel.lciCareLevelCode.Text", res, LanguageManager.GetCulture());
                this.lciCareLevelName.Text = Inventec.Common.Resource.Get.Value("frmCareLevel.lciCareLevelName.Text", res, LanguageManager.GetCulture());
                this.lciDisplayColor.Text = Inventec.Common.Resource.Get.Value("frmCareLevel.lciDisplayColor.Text", res, LanguageManager.GetCulture());

                this.grdColSTT.Caption = Inventec.Common.Resource.Get.Value("frmCareLevel.grdColSTT.Caption", res, LanguageManager.GetCulture());
                this.grdColDisplayColor.Caption = Inventec.Common.Resource.Get.Value("frmCareLevel.grdColDisplayColor.Caption", res, LanguageManager.GetCulture());
                this.grdColCareLevelCode.Caption = Inventec.Common.Resource.Get.Value("frmCareLevel.grdColCareLevelCode.Caption", res, LanguageManager.GetCulture());
                this.grdColCareLevelName.Caption = Inventec.Common.Resource.Get.Value("frmCareLevel.grdColCareLevelName.Caption", res, LanguageManager.GetCulture());
                this.grdColIsActive.Caption = Inventec.Common.Resource.Get.Value("frmCareLevel.grdColIsActive.Caption", res, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmCareLevel.grdColCreateTime.Caption", res, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmCareLevel.grdColCreator.Caption", res, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmCareLevel.grdColModifyTime.Caption", res, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmCareLevel.grdColModifier.Caption", res, LanguageManager.GetCulture());

                this.bbtnSearch.Caption = this.btnSearch.Text;
                this.bbtnAdd.Caption = this.btnAdd.Text;
                this.bbtnEdit.Caption = this.btnEdit.Text;
                this.bbtnReset.Caption = this.btnCancel.Text;

                this.Text = Inventec.Common.Resource.Get.Value("frmCareLevel.Text", res, LanguageManager.GetCulture());
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                this.currentData = null;
                this.ActionType = GlobalVariables.ActionAdd;
                txtKeyword.Text = "";
                ResetFormData();
                EnableControlChanged(this.ActionType);
                // Che do Them moi -> cho phep nhap Ma
                txtCareLevelCode.Properties.ReadOnly = false;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
        private void SetDefaultFocus()
        {
            try
            {
                txtKeyword.Focus();
                txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }
        }

        /// <summary>
        /// Gan focus vao control editor dau tien
        /// </summary>
        private void SetFocusEditor()
        {
            try
            {
                if (txtCareLevelCode.Properties.ReadOnly)
                {
                    txtCareLevelName.Focus();
                    txtCareLevelName.SelectAll();
                }
                else
                {
                    txtCareLevelCode.Focus();
                    txtCareLevelCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }
        }

        private void EnableControlChanged(int action)
        {
            try
            {
                btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
                btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Load data

        /// <summary>
        /// Ham lay du lieu theo dieu kien tim kiem va gan du lieu vao danh sach
        /// </summary>
        public void FillDataToGridControl()
        {
            try
            {
                WaitingManager.Show();

                int numPageSize = 0;
                if (ucPaging.pagingGrid != null && ucPaging.pagingGrid.PageSize > 0)
                {
                    numPageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                if (numPageSize <= 0) numPageSize = 20;

                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;

                //Chan UcPaging.Init tai nhap LoadPaging voi Start cua trang cu (xem chu thich cua isInitPaging).
                //Gan pagingGrid = null de guard "pagingGrid != null" ben trong thu vien tu ngat,
                //kem co isInitPaging de van an toan neu thu vien doi thu tu khoi tao.
                //Bat buoc dat SAU khi da doc numPageSize o tren, neu khong se mat so ban ghi/trang nguoi dung chon.
                isInitPaging = true;
                try
                {
                    ucPaging.pagingGrid = null;
                    ucPaging.Init(LoadPaging, param, numPageSize);
                }
                finally
                {
                    isInitPaging = false;
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        /// <summary>
        /// Ham goi api lay du lieu phan trang. Duoc UcPaging goi lai moi khi doi trang/doi so ban ghi mot trang.
        /// </summary>
        /// <param name="param">CommonParam chua Start/Limit cua trang can lay</param>
        private void LoadPaging(object param)
        {
            try
            {
                //Bo qua lan goi lai do ucPaging.Init sinh ra (Start con la cua trang cu)
                if (isInitPaging) return;

                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);

                HisCareLevelFilter filter = new HisCareLevelFilter();
                SetFilter(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";

                List<HIS_CARE_LEVEL> data = null;

                isLoadingData = true;
                gridViewCareLevel.BeginUpdate();
                try
                {
                    Inventec.Core.ApiResultObject<List<HIS_CARE_LEVEL>> apiResult =
                        new BackendAdapter(paramCommon).GetRO<List<HIS_CARE_LEVEL>>(HisRequestUriStore.MOSHIS_CARE_LEVEL_GET, ApiConsumers.MosConsumer, filter, paramCommon);

                    if (apiResult != null)
                    {
                        data = (List<HIS_CARE_LEVEL>)apiResult.Data;
                    }

                    gridControlCareLevel.DataSource = data;
                    rowCount = (data == null ? 0 : data.Count);
                    dataTotal = (apiResult == null || apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                }
                finally
                {
                    gridViewCareLevel.EndUpdate();
                    isLoadingData = false;
                }

                SyncEditorWithGrid(data);

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Sau moi lan nap lai luoi, keo con tro luoi va form editor ve cung mot ban ghi.
        /// Neu bo qua buoc nay: luoi tu dua con tro ve dong 0 trong khi editor van giu ban ghi cu,
        /// nguoi dung bam vao dong 0 thi FocusedRowChanged khong ban (con tro von da o do)
        /// -> bam "Sua" se ghi de nham ban ghi cu.
        /// Buoc nay dong thoi lam moi trang thai nut Sua sau khi Khoa/Bo khoa.
        /// </summary>
        private void SyncEditorWithGrid(List<HIS_CARE_LEVEL> data)
        {
            try
            {
                if (this.currentData == null) return;

                HIS_CARE_LEVEL stillExist = (data == null ? null : data.FirstOrDefault(o => o.ID == this.currentData.ID));

                if (stillExist != null)
                {
                    //Ban ghi dang mo tren form van con trong ket qua -> keo con tro luoi ve dung dong do
                    gridViewCareLevel.FocusedRowHandle = gridViewCareLevel.GetRowHandle(data.IndexOf(stillExist));
                    ChangedDataRow(stillExist);
                }
                else
                {
                    //Khong con trong ket qua (bi xoa, hoac lot khoi dieu kien tim kiem) -> ve che do Them moi
                    this.currentData = null;
                    this.ActionType = GlobalVariables.ActionAdd;
                    EnableControlChanged(this.ActionType);
                    ResetFormData();
                    txtCareLevelCode.Properties.ReadOnly = false;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SetFilter(ref HisCareLevelFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtKeyword.Text.Trim();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Lay lai ban ghi moi nhat tu backend truoc khi update, tranh ghi de cac truong khong hien tren form
        /// </summary>
        private void LoadCurrent(long currentId, ref HIS_CARE_LEVEL currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisCareLevelFilter filter = new HisCareLevelFilter();
                filter.ID = currentId;
                HIS_CARE_LEVEL data = new BackendAdapter(param).Get<List<HIS_CARE_LEVEL>>(HisRequestUriStore.MOSHIS_CARE_LEVEL_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                if (data != null)
                {
                    currentDTO = data;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Editor <-> data

        private void ChangedDataRow(HIS_CARE_LEVEL data)
        {
            try
            {
                if (data == null) return;

                this.currentData = data;
                FillDataToEditorControl(data);
                this.ActionType = GlobalVariables.ActionEdit;
                EnableControlChanged(this.ActionType);

                // Che do Sua -> khong cho doi Ma
                txtCareLevelCode.Properties.ReadOnly = true;

                //Disable nut Sua neu du lieu da bi khoa
                btnEdit.Enabled = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(HIS_CARE_LEVEL data)
        {
            try
            {
                if (data == null) return;

                txtCareLevelCode.Text = data.CARE_LEVEL_CODE;
                txtCareLevelName.Text = data.CARE_LEVEL_NAME;

                Color? color = ParseDisplayColor(data.DISPLAY_COLOR);
                if (color.HasValue)
                {
                    colorDisplayColor.Color = color.Value;
                }
                else
                {
                    colorDisplayColor.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref HIS_CARE_LEVEL currentDTO)
        {
            try
            {
                currentDTO.CARE_LEVEL_CODE = txtCareLevelCode.Text.Trim();
                currentDTO.CARE_LEVEL_NAME = txtCareLevelName.Text.Trim();
                currentDTO.DISPLAY_COLOR = FormatDisplayColor(colorDisplayColor);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ResetFormData()
        {
            try
            {
                if (!layoutControl2.IsInitialized) return;
                layoutControl2.BeginUpdate();
                try
                {
                    txtCareLevelCode.Text = "";
                    txtCareLevelName.Text = "";
                    colorDisplayColor.EditValue = null;
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
                finally
                {
                    layoutControl2.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Doc chuoi mau dang "R,G,B" thanh Color. Tra ve null neu chuoi rong/khong hop le.
        /// </summary>
        private static Color? ParseDisplayColor(string displayColor)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(displayColor)) return null;

                string[] parts = displayColor.Split(',');
                if (parts.Length < 3) return null;

                int red = Inventec.Common.TypeConvert.Parse.ToInt32(parts[0].Trim());
                int green = Inventec.Common.TypeConvert.Parse.ToInt32(parts[1].Trim());
                int blue = Inventec.Common.TypeConvert.Parse.ToInt32(parts[2].Trim());

                if (red < 0 || red > 255 || green < 0 || green > 255 || blue < 0 || blue > 255) return null;

                return Color.FromArgb(red, green, blue);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return null;
        }

        /// <summary>
        /// Ghi mau dang "R,G,B" - cung quy uoc voi HIS_PATIENT_CLASSIFY.DISPLAY_COLOR.
        /// Tra ve null khi nguoi dung khong chon mau.
        /// </summary>
        private static string FormatDisplayColor(ColorPickEdit editor)
        {
            try
            {
                if (editor == null || editor.EditValue == null) return null;
                if (!(editor.EditValue is Color)) return null;

                Color color = editor.Color;
                if (color.IsEmpty) return null;

                return String.Format("{0},{1},{2}", color.R, color.G, color.B);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return null;
        }

        #endregion

        #region Grid handler

        private void gridViewCareLevel_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (!e.IsGetData || e.Column.UnboundType == DevExpress.Data.UnboundColumnType.Bound) return;

                IList source = (IList)((BaseView)sender).DataSource;
                if (source == null || e.ListSourceRowIndex < 0 || e.ListSourceRowIndex >= source.Count) return;

                HIS_CARE_LEVEL data = (HIS_CARE_LEVEL)source[e.ListSourceRowIndex];
                if (data == null) return;

                if (e.Column.FieldName == "STT")
                {
                    e.Value = e.ListSourceRowIndex + 1 + startPage;
                }
                else if (e.Column.FieldName == "CREATE_TIME_STR")
                {
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                }
                else if (e.Column.FieldName == "MODIFY_TIME_STR")
                {
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                }
                else if (e.Column.FieldName == "IS_ACTIVE_STR")
                {
                    e.Value = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        ? HIS.Desktop.Plugins.HisCareLevel.Resources.ResourceMessage.TrangThaiHoatDong
                        : HIS.Desktop.Plugins.HisCareLevel.Resources.ResourceMessage.TrangThaiTamKhoa;
                }
                else if (e.Column.FieldName == "DISPLAY_COLOR_STR")
                {
                    e.Value = "";
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Doi editor cua cot Khoa/Xoa theo trang thai ban ghi:
        /// - Dang hoat dong  -> nut "Khóa" + nut "Xóa" (bam duoc)
        /// - Dang bi khoa    -> nut "Bỏ khóa" + nut "Xóa" mo (khong bam duoc)
        /// </summary>
        private void gridViewCareLevel_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                //Dung GetRow (khong index thang vao DataSource) vi e.RowHandle la chi so theo
                //thu tu HIEN THI - nguoi dung sap xep cot thi no lech voi chi so cua list nguon
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (view == null || e.RowHandle < 0) return;

                HIS_CARE_LEVEL data = view.GetRow(e.RowHandle) as HIS_CARE_LEVEL;
                if (data == null) return;

                bool isActive = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                if (e.Column.FieldName == "IS_LOCK")
                {
                    e.RepositoryItem = (isActive ? repositoryItemLock : repositoryItemUnLock);
                }
                else if (e.Column.FieldName == "IS_DELETE_ROW")
                {
                    e.RepositoryItem = (isActive ? repositoryItemDelete : repositoryItemDisDelete);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewCareLevel_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                //Xem chu thich o gridViewCareLevel_CustomRowCellEdit ve ly do dung GetRow
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (view == null || e.RowHandle < 0) return;

                HIS_CARE_LEVEL data = view.GetRow(e.RowHandle) as HIS_CARE_LEVEL;
                if (data == null) return;

                if (e.Column.FieldName == "IS_ACTIVE_STR")
                {
                    e.Appearance.ForeColor = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE) ? Color.Red : Color.Green;
                }
                else if (e.Column.FieldName == "DISPLAY_COLOR_STR")
                {
                    Color? color = ParseDisplayColor(data.DISPLAY_COLOR);
                    if (color.HasValue)
                    {
                        e.Appearance.BackColor = color.Value;
                        e.Appearance.BackColor2 = color.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridViewCareLevel_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            try
            {
                //Dang nap lai luoi -> khong do du lieu sang form editor
                if (isLoadingData) return;

                HIS_CARE_LEVEL rowData = gridViewCareLevel.GetFocusedRow() as HIS_CARE_LEVEL;
                if (rowData != null)
                {
                    ChangedDataRow(rowData);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Bam mot lan tren luoi luon dong bo ban ghi dang chon sang form editor.
        /// Can thiet vi khi con tro da nam san o dong do (vd vua nap lai luoi) thi
        /// FocusedRowChanged khong ban. Khong goi SetFocusEditor de giu focus tren luoi,
        /// cho phep tiep tuc di chuyen bang phim mui ten.
        /// </summary>
        private void gridViewCareLevel_Click(object sender, EventArgs e)
        {
            try
            {
                if (isLoadingData) return;

                HIS_CARE_LEVEL rowData = gridViewCareLevel.GetFocusedRow() as HIS_CARE_LEVEL;
                if (rowData != null && (this.currentData == null || this.currentData.ID != rowData.ID))
                {
                    ChangedDataRow(rowData);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridControlCareLevel_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                HIS_CARE_LEVEL rowData = gridViewCareLevel.GetFocusedRow() as HIS_CARE_LEVEL;
                if (rowData != null)
                {
                    ChangedDataRow(rowData);
                    SetFocusEditor();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridViewCareLevel_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    HIS_CARE_LEVEL rowData = gridViewCareLevel.GetFocusedRow() as HIS_CARE_LEVEL;
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);
                        SetFocusEditor();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Button handler

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Nut "Làm lại": xoa trang du lieu dang nhap, tro ve che do Them moi
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.currentData = null;
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                ResetFormData();
                // Lam lai -> ve che do Them moi, cho phep nhap Ma
                txtCareLevelCode.Properties.ReadOnly = false;
                SetFocusEditor();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SaveProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!btnEdit.Enabled && !btnAdd.Enabled)
                    return;

                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;

                WaitingManager.Show();

                HIS_CARE_LEVEL updateDTO = new HIS_CARE_LEVEL();

                if (this.ActionType == GlobalVariables.ActionEdit && this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }

                UpdateDTOFromDataForm(ref updateDTO);

                if (this.ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    HIS_CARE_LEVEL resultData = new BackendAdapter(param).Post<HIS_CARE_LEVEL>(HisRequestUriStore.MOSHIS_CARE_LEVEL_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        BackendDataWorker.Reset<HIS_CARE_LEVEL>();

                        //Them moi xong -> tra form ve trang thai san sang them ban ghi tiep theo.
                        //Dat truoc khi nap lai luoi de SyncEditorWithGrid khong keo ban ghi cu len form.
                        this.currentData = null;
                        this.ActionType = GlobalVariables.ActionAdd;
                        EnableControlChanged(this.ActionType);
                        ResetFormData();
                        txtCareLevelCode.Properties.ReadOnly = false;

                        FillDataToGridControl();
                    }
                }
                else
                {
                    HIS_CARE_LEVEL resultData = new BackendAdapter(param).Post<HIS_CARE_LEVEL>(HisRequestUriStore.MOSHIS_CARE_LEVEL_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        BackendDataWorker.Reset<HIS_CARE_LEVEL>();

                        //Sua xong -> giu nguyen ban ghi vua sua tren form editor.
                        //SyncEditorWithGrid trong LoadPaging se do lai du lieu moi nhat va
                        //cap nhat trang thai nut Sua theo IS_ACTIVE.
                        this.currentData = resultData;
                        FillDataToGridControl();
                    }
                }

                WaitingManager.Hide();

                if (success)
                {
                    SetFocusEditor();
                }

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Grid button: Lock / Unlock / Delete

        /// <summary>
        /// Ban ghi dang hoat dong -> khoa lai (IS_ACTIVE = 0)
        /// </summary>
        private void repositoryItemLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                HIS_CARE_LEVEL data = gridViewCareLevel.GetFocusedRow() as HIS_CARE_LEVEL;
                if (data == null) return;

                if (MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                WaitingManager.Show();
                HIS_CARE_LEVEL result = new BackendAdapter(param).Post<HIS_CARE_LEVEL>(HisRequestUriStore.MOSHIS_CARE_LEVEL_LOCK, ApiConsumers.MosConsumer, data.ID, param);
                WaitingManager.Hide();

                if (result != null)
                {
                    BackendDataWorker.Reset<HIS_CARE_LEVEL>();
                    //SyncEditorWithGrid cap nhat lai trang thai nut Sua theo IS_ACTIVE moi
                    FillDataToGridControl();
                }

                MessageManager.Show(this, param, result != null);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Ban ghi dang bi khoa -> bo khoa (IS_ACTIVE = 1)
        /// </summary>
        private void repositoryItemUnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                HIS_CARE_LEVEL data = gridViewCareLevel.GetFocusedRow() as HIS_CARE_LEVEL;
                if (data == null) return;

                if (MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                WaitingManager.Show();
                HIS_CARE_LEVEL result = new BackendAdapter(param).Post<HIS_CARE_LEVEL>(HisRequestUriStore.MOSHIS_CARE_LEVEL_UNLOCK, ApiConsumers.MosConsumer, data.ID, param);
                WaitingManager.Hide();

                if (result != null)
                {
                    BackendDataWorker.Reset<HIS_CARE_LEVEL>();
                    //SyncEditorWithGrid cap nhat lai trang thai nut Sua theo IS_ACTIVE moi
                    FillDataToGridControl();
                }

                MessageManager.Show(this, param, result != null);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void repositoryItemDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                HIS_CARE_LEVEL data = gridViewCareLevel.GetFocusedRow() as HIS_CARE_LEVEL;
                if (data == null) return;

                if (MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                WaitingManager.Show();
                bool success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSHIS_CARE_LEVEL_DELETE, ApiConsumers.MosConsumer, data.ID, param);
                WaitingManager.Hide();

                if (success)
                {
                    BackendDataWorker.Reset<HIS_CARE_LEVEL>();
                    //SyncEditorWithGrid tu tra form ve che do Them moi neu ban ghi vua xoa dang mo tren form
                    FillDataToGridControl();
                }

                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        #endregion

        #region Validate

        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(txtCareLevelCode);
                ValidationSingleControl(txtCareLevelName);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1 || positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Keyboard

        private void txtKeyword_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridViewCareLevel.Focus();
                    gridViewCareLevel.FocusedRowHandle = 0;
                    HIS_CARE_LEVEL rowData = gridViewCareLevel.GetFocusedRow() as HIS_CARE_LEVEL;
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtCareLevelCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtCareLevelName.Focus();
                    txtCareLevelName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtCareLevelName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    colorDisplayColor.Focus();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void colorDisplayColor_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                    }
                    else if (btnEdit.Enabled)
                    {
                        btnEdit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Shortcut

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                {
                    btnAdd_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType != GlobalVariables.ActionEdit) return;

                if (!btnEdit.Enabled)
                {
                    //Ban ghi dang bi khoa -> khong cho sua
                    MessageBox.Show(HIS.Desktop.Plugins.HisCareLevel.Resources.ResourceMessage.BanGhiDangBiKhoaKhongTheSua, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnCancel_Click(null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void bbtnFocusDefault_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SetDefaultFocus();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Tooltip

        private void toolTipControllerGrid_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                //TODO
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion
    }
}
