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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.UC.ExamTreatmentFinish.ADO;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.UC.ExamTreatmentFinish
{
    public partial class UcAdvise : UserControl
    {
        List<RoomExamADO> _RoomExamADOs { get; set; }
        private int positionHandle = -1;
        long roomId = 0;
        bool isCheckAll = true;

        public UcAdvise()
        {
            InitializeComponent();
            try
            {
                SetMaxlength(this.txtAdvise, 500);
                LoadRoomExam();
                //this.AutoScrollMinSize = new System.Drawing.Size(450, 295);
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UcAdvise(long _roomId)
        {
            InitializeComponent();
            try
            {
                this.roomId = _roomId;
                SetMaxlength(this.txtAdvise, 500);
                LoadRoomExam();
                //this.AutoScrollMinSize = new System.Drawing.Size(450, 295);
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UcAdvise
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.ExamTreatmentFinish.Resources.Lang", typeof(UcAdvise).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UcAdvise.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UcAdvise.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UcAdvise.txtSearch.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtAdvise.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UcAdvise.txtAdvise.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UcAdvise.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UcAdvise.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("UcAdvise.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("UcAdvise.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("UcAdvise.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadRoomExam()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("UcAdvise(long _roomId) roomId " + this.roomId);
                gridControlRoomExam.DataSource = null;

                var _vHisExecuteRooms = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().Where(p =>
                     p.IS_ACTIVE == 1
                     && p.IS_EXAM == 1).ToList();

                _RoomExamADOs = new List<RoomExamADO>();
                foreach (var item in _vHisExecuteRooms)
                {
                    RoomExamADO ado = new RoomExamADO()
                    {
                        ID = item.ROOM_ID,
                        ROOM_CODE = item.EXECUTE_ROOM_CODE,
                        ROOM_NAME = item.EXECUTE_ROOM_NAME
                    };
                    if (item.ROOM_ID == this.roomId)
                        ado.IsCheck = true;

                    _RoomExamADOs.Add(ado);
                }
                _RoomExamADOs = _RoomExamADOs != null && _RoomExamADOs.Count > 0 
                    ? _RoomExamADOs.OrderByDescending(o => o.IsCheck).ThenBy(p => p.ROOM_NAME).ToList() 
                    : _RoomExamADOs;

                gridControlRoomExam.DataSource = _RoomExamADOs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public AdviseADO GetValue()
        {
            AdviseADO result = new AdviseADO();
            try
            {
                result.Advise = this.txtAdvise.Text;
                var datas = (List<RoomExamADO>)gridControlRoomExam.DataSource;
                if (datas != null && datas.Count > 0)
                {
                    List<long> _ids = datas.Where(p => p.IsCheck).Select(p => p.ID).Distinct().ToList();
                    if (_ids != null && _ids.Count > 0)
                    {
                        result.AppointmentExamRoomIds = string.Join(",", _ids);
                    }
                    result.ExamRoomIds = _ids;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public void SetValue(AdviseADO value)
        {
            try
            {
                this.txtAdvise.Text = value.Advise;
                if (!string.IsNullOrEmpty(value.AppointmentExamRoomIds) && _RoomExamADOs != null && _RoomExamADOs.Count > 0)
                {
                    string[] ids = value.AppointmentExamRoomIds.Split(',');
                    foreach (var item in _RoomExamADOs)
                    {
                        var dataCheck = ids.FirstOrDefault(p => p.Trim() == item.ID.ToString().Trim());
                        if (!string.IsNullOrEmpty(dataCheck))
                            item.IsCheck = true;
                    }
                    _RoomExamADOs = _RoomExamADOs.OrderByDescending(p => p.IsCheck).ToList();
                }

                gridControlRoomExam.BeginUpdate();
                gridControlRoomExam.DataSource = _RoomExamADOs;
                gridControlRoomExam.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetMaxlength(BaseEdit control, int maxlenght)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxlenght;
                validate.IsRequired = false;
                validate.ErrorText = string.Format("Nhập quá kí tự cho phép", maxlenght);
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public bool GetValidate()
        {
            bool result = true;
            try
            {
                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void gridViewRoomExam_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            view.ShowEditor();
                            DevExpress.XtraEditors.CheckEdit checkEdit = view.ActiveEditor as DevExpress.XtraEditors.CheckEdit;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {

                                checkEdit.Checked = !checkEdit.Checked;
                                view.CloseEditor();
                                if (this._RoomExamADOs != null && this._RoomExamADOs.Count > 0)
                                {
                                    var dataChecks = this._RoomExamADOs.Where(p => p.IsCheck).ToList();
                                    if (dataChecks != null && dataChecks.Count > 0)
                                    {
                                        gridColumnCheck.Image = imageListIcon.Images[5];
                                    }
                                    else
                                    {
                                        gridColumnCheck.Image = imageListIcon.Images[6];
                                    }
                                }
                            }
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                    }
                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "IsCheck")
                        {
                            gridColumnCheck.Image = imageListIcon.Images[5];
                            gridViewRoomExam.BeginUpdate();
                            if (this._RoomExamADOs == null)
                                this._RoomExamADOs = new List<RoomExamADO>();
                            if (isCheckAll == true)
                            {
                                foreach (var item in this._RoomExamADOs)
                                {
                                    item.IsCheck = true;
                                }
                                isCheckAll = false;
                            }
                            else
                            {
                                gridColumnCheck.Image = imageListIcon.Images[6];
                                foreach (var item in this._RoomExamADOs)
                                {
                                    item.IsCheck = false;
                                }
                                isCheckAll = true;
                            }
                            gridViewRoomExam.EndUpdate();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearch_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                string str = txtSearch.Text.Trim();
                List<RoomExamADO> _RoomExamADONews = new List<RoomExamADO>();
                if (!string.IsNullOrEmpty(str))
                {
                    _RoomExamADONews = _RoomExamADOs.Where(p => p.ROOM_CODE.ToUpper().Contains(str.ToUpper())
                        || p.ROOM_NAME.ToUpper().Contains(str.ToUpper())).ToList();
                }
                else
                {
                    _RoomExamADONews = _RoomExamADOs;
                }
                _RoomExamADONews = _RoomExamADONews.OrderByDescending(p => p.IsCheck).ThenBy(p => p.ROOM_CODE).ToList();

                gridControlRoomExam.BeginUpdate();
                gridControlRoomExam.DataSource = _RoomExamADONews;
                gridControlRoomExam.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
