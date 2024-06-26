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
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Plugins.ExecuteRoom.Delegate;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
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

namespace HIS.Desktop.Plugins.ExecuteRoom.Design
{
    public partial class frmNumberFilmInput : HIS.Desktop.Utility.FormBase
    {
        RefeshDataExt _dlg { get; set; }

        V_HIS_SERE_SERV_6 _SereServ { get; set; }

        HIS_SERE_SERV_EXT _SSExt { get; set; }
        public frmNumberFilmInput()
            : base(null)
        {
            InitializeComponent();
        }

        public frmNumberFilmInput(V_HIS_SERE_SERV_6 _sS, RefeshDataExt dlg)
            : base(null)
        {
            InitializeComponent();
            try
            {
                this._SereServ = _sS;

                this._dlg = dlg;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmNumberFilmInput_Load(object sender, EventArgs e)
        {
            try
            {
                SetIconFrm();

                ComboSizeFilm();

                ValidNumberOfFilm();

                LoadSereServExt();
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmNumberFilmInput
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ExecuteRoom.Resources.Lang", typeof(frmNumberFilmInput).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmNumberFilmInput.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmNumberFilmInput.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem_Save.Caption = Inventec.Common.Resource.Get.Value("frmNumberFilmInput.barButtonItem_Save.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmNumberFilmInput.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboSizeOfFilm.Properties.NullText = Inventec.Common.Resource.Get.Value("frmNumberFilmInput.cboSizeOfFilm.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMunberOfFilm.Text = Inventec.Common.Resource.Get.Value("frmNumberFilmInput.lciMunberOfFilm.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmNumberFilmInput.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciNumberFailFilm.Text = Inventec.Common.Resource.Get.Value("frmNumberFilmInput.LciNumberFailFilm.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmNumberFilmInput.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadSereServExt()
        {
            try
            {
                if (this._SereServ != null && this._SereServ.ID > 0)
                {
                    MOS.Filter.HisSereServExtFilter filter = new MOS.Filter.HisSereServExtFilter();
                    filter.SERE_SERV_ID = this._SereServ.ID;
                    filter.IS_ACTIVE = 1;

                    _SSExt = new BackendAdapter(new Inventec.Core.CommonParam()).Get<List<HIS_SERE_SERV_EXT>>("/api/HisSereServExt/Get", ApiConsumers.MosConsumer, filter, null).FirstOrDefault();
                    if (_SSExt != null)
                    {
                        this.txtNumberOfFilm.Text = _SSExt.NUMBER_OF_FILM != null ? _SSExt.NUMBER_OF_FILM.ToString() : "";
                        this.cboSizeOfFilm.EditValue = _SSExt.FILM_SIZE_ID;
                        this.TxtNumberFailFilm.EditValue = _SSExt.NUMBER_OF_FAIL_FILM;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ComboSizeFilm()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisFilmSizeFilter filter = new HisFilmSizeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_FILM_SIZE>>("api/HisFilmSize/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("FILM_SIZE_CODE", "Mã kích cỡ phim", 150, 1));
                columnInfos.Add(new ColumnInfo("FILM_SIZE_NAME", "Tên kích cỡ phim", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("FILM_SIZE_NAME", "ID", columnInfos, true, 350);
                ControlEditorLoader.Load(cboSizeOfFilm, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidNumberOfFilm()
        {
            try
            {
                string key = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ServiceExecute.CĐHA.ValidNumberOfFilm");//Giá trị = 1 bắt buộc nhập số phim

                bool isCheck = false;
                if (this._SereServ.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA && key.Trim() == "1")
                {
                    isCheck = true;
                    lciMunberOfFilm.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
                FilmValidationRule FilmRule = new FilmValidationRule();
                FilmRule.txtNumberOfFilm = txtNumberOfFilm;
                FilmRule.isCheck = isCheck;
                FilmRule.ErrorText = "Trường dữ liệu bắt buộc";
                FilmRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(txtNumberOfFilm, FilmRule);
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
                if (!dxValidationProvider1.Validate()) return;

                //WaitingManager.Show();
                HIS_SERE_SERV_EXT ado = new HIS_SERE_SERV_EXT();
                if (!String.IsNullOrEmpty(txtNumberOfFilm.Text))
                {
                    ado.NUMBER_OF_FILM = long.Parse(txtNumberOfFilm.Text);
                }
                else
                {
                    ado.NUMBER_OF_FILM = null;
                }
                ado.FILM_SIZE_ID = cboSizeOfFilm.EditValue != null ? (long?)cboSizeOfFilm.EditValue : null;
                ado.SERE_SERV_ID = _SereServ.ID;
                if (_SSExt != null && _SSExt.ID > 0)
                {
                    ado.ID = _SSExt.ID;
                }

                if (!String.IsNullOrEmpty(TxtNumberFailFilm.Text))
                {
                    ado.NUMBER_OF_FAIL_FILM = long.Parse(TxtNumberFailFilm.Text);
                }
                else
                {
                    ado.NUMBER_OF_FAIL_FILM = null;
                }


                this.Close();
                if (this._dlg != null)
                    this._dlg(ado);

                //string uri = _SSExt != null ? "/api/HisSereServExt/Update" : "/api/HisSereServExt/Create";
                //CommonParam param = new CommonParam();
                //bool success = false;
                //var result = new BackendAdapter(param).Post<HIS_SERE_SERV_EXT>(uri, ApiConsumers.MosConsumer, ado, param);
                //if (result != null)
                //{
                //    success = true;
                //    if (this._dlg != null)
                //        this._dlg(ado);
                //    this.Close();
                //}
                //WaitingManager.Hide();
                //MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem_Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNumberOfFilm_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '/'))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNumberOfFilm_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    TxtNumberFailFilm.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSizeOfFilm_KeyUp(object sender, KeyEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSizeOfFilm_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    btnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TxtNumberFailFilm_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboSizeOfFilm.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TxtNumberFailFilm_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '/'))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
