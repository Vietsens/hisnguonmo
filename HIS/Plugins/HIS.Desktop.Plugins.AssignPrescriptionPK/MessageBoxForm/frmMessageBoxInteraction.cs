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
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraLayout.Utils;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.LibraryMessage;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.MessageBoxForm
{
    public partial class frmMessageBoxInteraction : DevExpress.XtraEditors.XtraForm
    {
        LyDoKeThuocTuongTac LyDoKeThuocTuongTac;
        TuongTacKhongBoSung TuongTacKhongBoSung;
        string InteractionMessage = "";
        string Question ="";
        internal int positionHandleControl = -1;
        string MucDo = "", CoChe = "", HauQua = "", MoTa = "", XuLy = "";

        public frmMessageBoxInteraction(string _InteractionMessage, string _MucDo, string _CoChe, string _HauQua, string _MoTa, string _XuLy, string _Question, LyDoKeThuocTuongTac _LyDoKeThuocTuongTac, TuongTacKhongBoSung _TuongTacKhongBoSung)
        {
            InitializeComponent();
            this.LyDoKeThuocTuongTac = _LyDoKeThuocTuongTac;
            this.TuongTacKhongBoSung = _TuongTacKhongBoSung;
            this.InteractionMessage = _InteractionMessage;
            this.Question = _Question;
            this.MucDo = _MucDo;
            this.CoChe = _CoChe;
            this.HauQua = _HauQua;
            this.MoTa = _MoTa;
            this.XuLy = _XuLy;

            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmMessageBoxInteraction_Load(object sender, EventArgs e)
        {

            try
            {
                 
                 Resources.ResourceLanguageManager.LanguagefrmMessageBoxInteraction = new ResourceManager("HIS.Desktop.Plugins.AssignPrescriptionPK.Resources.Lang", typeof(HIS.Desktop.Plugins.AssignPrescriptionPK.MessageBoxForm.frmMessageBoxInteraction).Assembly);

                 this.Text = Inventec.Common.Resource.Get.Value("frmMessageBoxInteraction.Text", Resources.ResourceLanguageManager.LanguagefrmMessageBoxInteraction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                 this.txtInteractionMessage.Text = this.InteractionMessage;
                 this.txtMucDo.Text = this.MucDo;
                 this.txtCoChe.Text = this.CoChe;
                 this.txtHauQua.Text = this.HauQua;
                 this.txtMoTa.Text = this.MoTa;
                 this.txtXuLy.Text = this.XuLy;

                 if (!String.IsNullOrEmpty(this.Question))
                 {
                     this.lblQuestion.Text = this.Question;
                     this.lciReason.Visibility = LayoutVisibility.Always;
                    this.layoutControlItem1.Visibility = LayoutVisibility.Always;
                    this.btnAdd.Visible = true;
                     layoutControlItem4.Visibility = LayoutVisibility.Always;
                     this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmMessageBoxInteraction.btnAdd.Text", Resources.ResourceLanguageManager.LanguagefrmMessageBoxInteraction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                     this.btnNoAdd.Text = Inventec.Common.Resource.Get.Value("frmMessageBoxInteraction.btnNoAdd.Text", Resources.ResourceLanguageManager.LanguagefrmMessageBoxInteraction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                     this.Size = new Size(627, 420);
                 }
                 else
                 {
                     this.lciQuestion.Visibility = LayoutVisibility.Never;
                     this.lciReason.Visibility = LayoutVisibility.Never;
                    this.layoutControlItem1.Visibility = LayoutVisibility.Never;
                    this.btnAdd.Visible = false;
                     layoutControlItem4.Visibility = LayoutVisibility.Never;
                     this.btnNoAdd.Text = Inventec.Common.Resource.Get.Value("frmMessageBoxInteraction.btnAgree.Text", Resources.ResourceLanguageManager.LanguagefrmMessageBoxInteraction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                     this.Size = new Size(627, 360);
                     emptySpaceItem1.Size = new Size(530, 20);
                 }

                 ValidateForm();
                gridControl1.DataSource = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_INTERACTION_REASON>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                 this.timer1.Interval = 100;
                 this.timer1.Enabled = true;
                 this.timer1.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }



        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmMessageBoxInteraction
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResourcefrmMessageBoxInteraction = new ResourceManager("HIS.Desktop.Plugins.AssignPrescriptionPK.Resources.Lang", typeof(frmMessageBoxInteraction).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmMessageBoxInteraction.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMessageBoxInteraction, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmMessageBoxInteraction.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMessageBoxInteraction, LanguageManager.GetCulture());
                this.btnNoAdd.Text = Inventec.Common.Resource.Get.Value("frmMessageBoxInteraction.btnNoAdd.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMessageBoxInteraction, LanguageManager.GetCulture());
                this.lciReason.Text = Inventec.Common.Resource.Get.Value("frmMessageBoxInteraction.lciReason.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMessageBoxInteraction, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmMessageBoxInteraction.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMessageBoxInteraction, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmMessageBoxInteraction.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMessageBoxInteraction, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmMessageBoxInteraction.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMessageBoxInteraction, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmMessageBoxInteraction.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMessageBoxInteraction, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmMessageBoxInteraction.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMessageBoxInteraction, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmMessageBoxInteraction.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMessageBoxInteraction, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void ValidateForm()
        {
            try
            {
                this.dxValidationProvider1.SetValidationRule(txtReason, null);

                if (txtReason.Enabled)
                {
                    this.ValidationSingleControl(this.txtReason, this.dxValidationProvider1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void ValidationSingleControl(BaseEdit control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void btnNoAdd_Click(object sender, EventArgs e)
        {
            try
            {
                this.TuongTacKhongBoSung();
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnAdd.Visible)
                    return;

                this.positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                this.LyDoKeThuocTuongTac(this.txtReason.Text);
                this.Close();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                this.btnNoAdd.Focus();
                this.timer1.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnMore_Click(object sender, EventArgs e)
        {
            ProcessShowpopupControlContainer();
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {

            try
            {
                var rowdataAssignOld = (MOS.EFMODEL.DataModels.HIS_INTERACTION_REASON)gridView1.GetFocusedRow();

                if (rowdataAssignOld != null)
                {
                    txtReason.Text = rowdataAssignOld.INTERACTION_REASON_NAME;
                }
                popupControlContainer1.HidePopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        int popupHeight = 600;
        void ProcessShowpopupControlContainer()
        {
            int heightPlus = 0;
            Rectangle bounds = GetClientRectangle(this.txtReason, ref heightPlus);
            Rectangle bounds1 = GetAllClientRectangle(this.Parent, ref heightPlus);
            if (bounds == null)
            {
                bounds = txtReason.Bounds;
            }

            //xử lý tính toán lại vị trí hiển thị popup tương đối phụ thuộc theo chiều cao của popup, kích thước màn hình, đối tượng bệnh nhân(bhyt/...)
            if (bounds1.Height <= 768)
            {
                if (popupHeight == 600)
                {
                    heightPlus = bounds.Y >= 650 ? -125 : (bounds.Y > 410 ? (-262) : (bounds.Y < 230 ? (-bounds.Y - 227) : -276));
                }
                else
                    heightPlus = bounds.Y >= 650 ? -60 : (bounds.Y > 410 ? -60 : ((bounds.Y < 230 ? -bounds.Y - 27 : -78)));
            }
            else
            {
                if (popupHeight == 600)
                {
                    heightPlus = bounds.Y >= 650 ? -327 : (bounds.Y > 410 ? -260 : (bounds.Y < 230 ? (-bounds.Y - 225) : -608));
                }
                else
                    heightPlus = bounds.Y >= 650 ? (-122) : (bounds.Y > 410 ? -60 : ((bounds.Y < 230 ? -bounds.Y - 25 : -180)));
            }

            Rectangle buttonBounds = new Rectangle(txtReason.Bounds.X + 100, bounds.Y, bounds.Width, bounds.Height);
            popupControlContainer1.ShowPopup(new Point(txtReason.Bounds.X + 650, txtReason.Bounds.Y + txtReason.Bounds.Height + 320));
            gridView1.Focus();
            gridView1.FocusedRowHandle = 0;
        }

        private Rectangle GetClientRectangle(Control control, ref int heightPlus)
        {
            Rectangle bounds = default(Rectangle);
            if (control != null)
            {
                bounds = control.Bounds;
                if (control.Parent != null && !(control is UserControl))
                {
                    heightPlus += bounds.Y;
                    return GetClientRectangle(control.Parent, ref heightPlus);
                }
            }
            return bounds;
        }

        private Rectangle GetAllClientRectangle(Control control, ref int heightPlus)
        {
            Rectangle bounds = default(Rectangle);
            if (control != null)
            {
                bounds = control.Bounds;
                if (control.Parent != null)
                {
                    heightPlus += bounds.Y;
                    return GetAllClientRectangle(control.Parent, ref heightPlus);
                }
            }
            return bounds;
        }


    }
}
