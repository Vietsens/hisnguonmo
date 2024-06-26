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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.Filter;
using MOS.EFMODEL.DataModels;
using IMSys.DbConfig.HIS_RS;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.IsAdmin;

namespace HIS.Desktop.Plugins.TextLibrary
{
    public partial class FormTextLibrary : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        System.Globalization.CultureInfo cultureLang;
        internal int ActionType = 0;
        bool ChooseTextLib = false;
        int positionHandle = -1;
        bool _isUsingTextLibraryInfoADO = false;
        bool _isNotSaveTemplate = false;
        bool _isFindTemplate = false;
        bool _isFillHashtag = false;
        bool _isFillContent = false;
        string hashtag = "";
        string content = "";
        MOS.EFMODEL.DataModels.HIS_TEXT_LIB textLib;
        HIS.Desktop.Common.DelegateDataTextLib dataTextLib;
        string creator = "";
        short IS_TRUE = 1;

        HIS.Desktop.Plugins.TextLibrary.UC.UCPicture UcPicture = new UC.UCPicture();
        HIS.Desktop.Plugins.TextLibrary.UC.UCDocument UcDocument = new UC.UCDocument();


        #endregion

        #region Construct
        public FormTextLibrary()
        {
            InitializeComponent();
            try
            {
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                this.textLib = new MOS.EFMODEL.DataModels.HIS_TEXT_LIB();
                creator = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                SetIcon();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public FormTextLibrary(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                this.textLib = new MOS.EFMODEL.DataModels.HIS_TEXT_LIB();
                creator = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.Text = module.text;
                SetIcon();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public FormTextLibrary(string txt, HIS.Desktop.Common.DelegateDataTextLib tag, Inventec.Desktop.Common.Modules.Module module, HIS.Desktop.ADO.TextLibraryInfoADO textLibraryInfoADO)
            : this(module)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => textLibraryInfoADO), textLibraryInfoADO));
                if (textLibraryInfoADO != null)
                {
                    this.hashtag = textLibraryInfoADO.Hashtag;
                    this.content = textLibraryInfoADO.Content;
                    this._isNotSaveTemplate = textLibraryInfoADO.IsNotSaveTemplate;
                    this._isFindTemplate = textLibraryInfoADO.IsFindTemplate;
                    this._isFillHashtag = textLibraryInfoADO.IsFillHashtag;
                    this._isFillContent = textLibraryInfoADO.IsFillContent;
                    if (this._isFindTemplate)
                    {
                        ChooseTextLib = true;
                    }
                    if (textLibraryInfoADO.IsNotSaveTemplate || textLibraryInfoADO.IsFindTemplate
                        || textLibraryInfoADO.IsFillHashtag || textLibraryInfoADO.IsFillContent)
                    {
                        this._isUsingTextLibraryInfoADO = true;
                    }
                }
                if (!String.IsNullOrEmpty(txt))
                {
                    ChooseTextLib = true;
                    this.hashtag = txt;
                }
                if (tag != null)
                {
                    this.dataTextLib = tag;
                    ChooseTextLib = true;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormTextLibrary_Load(object sender, EventArgs e)
        {
            try
            {
                ShowButton();
                SetCaptionByLanguageKeyNew();
                //LoadKeysFromlanguage();

                SetDefaultValueControl();

                FillDataToGrid();

                ValidControls();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method
        private void ShowButton()
        {
            try
            {
                int widthButton = 110;
                if (ChooseTextLib == false)
                {
                    lciBtnChoose.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    emptySpaceItem2.Size = new Size(Root.Size.Width - widthButton * 2, 26);
                    lciBtnSave.Size = new Size(widthButton, 26);
                    lciBtnNew.Size = new Size(widthButton, 26);
                    barButtonItemChoose.Enabled = false;
                }


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
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeysFromlanguage()
        {
            try
            {
                //layout
                this.btnNew.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TEXT_LIBRARY__BTN_NEW",
                    Resources.ResourceLanguageManager.LanguageFormTextLibrary,
                    cultureLang);
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TEXT_LIBRARY__BTN_SEARCH",
                    Resources.ResourceLanguageManager.LanguageFormTextLibrary,
                    cultureLang);
                this.btnSave.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TEXT_LIBRARY__BTN_UPDATE",
                    Resources.ResourceLanguageManager.LanguageFormTextLibrary,
                    cultureLang);
                this.lciHashTag.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TEXT_LIBRARY__LCI_HASHTAG",
                    Resources.ResourceLanguageManager.LanguageFormTextLibrary,
                    cultureLang);
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TEXT_LIBRARY__LCI_HOTKEY",
                    Resources.ResourceLanguageManager.LanguageFormTextLibrary,
                    cultureLang);
                this.lciTitle.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TEXT_LIBRARY__LCI_TITLE",
                    Resources.ResourceLanguageManager.LanguageFormTextLibrary,
                    cultureLang);
                this.txtFindHashtag.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TEXT_LIBRARY__TXT_FIND_HASHTAG__NULL_VALUE",
                    Resources.ResourceLanguageManager.LanguageFormTextLibrary,
                    cultureLang);
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TEXT_LIBRARY__TXT_KEYWORD__NULL_VALUE",
                    Resources.ResourceLanguageManager.LanguageFormTextLibrary,
                    cultureLang);
                this.lciChkIsPublic.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TEXT_LIBRARY__LCI_CHK_IS_PUBLIC",
                    Resources.ResourceLanguageManager.LanguageFormTextLibrary,
                    cultureLang);

                //gridView
                Gc_CreateTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TEXT_LIBRARY__GC_CREATE_TIME",
                    Resources.ResourceLanguageManager.LanguageFormTextLibrary,
                    cultureLang);
                this.Gc_Creator.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TEXT_LIBRARY__GC_CREATOR",
                    Resources.ResourceLanguageManager.LanguageFormTextLibrary,
                    cultureLang);
                this.Gc_Modifier.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TEXT_LIBRARY__GC_MODIFIER",
                    Resources.ResourceLanguageManager.LanguageFormTextLibrary,
                    cultureLang);
                this.Gc_ModifyTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TEXT_LIBRARY__GC_MODIFY_TIME",
                    Resources.ResourceLanguageManager.LanguageFormTextLibrary,
                    cultureLang);
                this.Gc_Hashtag.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TEXT_LIBRARY__GC_HASHTAG",
                    Resources.ResourceLanguageManager.LanguageFormTextLibrary,
                    cultureLang);
                this.Gc_Title.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TEXT_LIBRARY__GC_TITLE",
                    Resources.ResourceLanguageManager.LanguageFormTextLibrary,
                    cultureLang);
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TEXT_LIBRARY__GC_HOTKEY",
                    Resources.ResourceLanguageManager.LanguageFormTextLibrary,
                    cultureLang);
                this.Gc_Stt.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TEXT_LIBRARY__GC_STT",
                    Resources.ResourceLanguageManager.LanguageFormTextLibrary,
                    cultureLang);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện FormTextLibrary
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.TextLibrary.Resources.Lang", typeof(FormTextLibrary).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormTextLibrary.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("FormTextLibrary.btnNew.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("FormTextLibrary.txtKeyWord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("FormTextLibrary.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("FormTextLibrary.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("FormTextLibrary.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPicture.Properties.Caption = Inventec.Common.Resource.Get.Value("FormTextLibrary.chkPicture.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkDocument.Properties.Caption = Inventec.Common.Resource.Get.Value("FormTextLibrary.chkDocument.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("FormTextLibrary.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("FormTextLibrary.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsDepartment.Properties.Caption = Inventec.Common.Resource.Get.Value("FormTextLibrary.chkIsDepartment.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsPublic.Properties.Caption = Inventec.Common.Resource.Get.Value("FormTextLibrary.chkIsPublic.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTitle.Text = Inventec.Common.Resource.Get.Value("FormTextLibrary.lciTitle.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHashTag.Text = Inventec.Common.Resource.Get.Value("FormTextLibrary.lciHashTag.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("FormTextLibrary.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnChoose.Text = Inventec.Common.Resource.Get.Value("FormTextLibrary.btnChoose.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("FormTextLibrary.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtFindHashtag.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("FormTextLibrary.txtFindHashtag.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_Stt.Caption = Inventec.Common.Resource.Get.Value("FormTextLibrary.Gc_Stt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("FormTextLibrary.gridColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn18.Caption = Inventec.Common.Resource.Get.Value("FormTextLibrary.gridColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_Title.Caption = Inventec.Common.Resource.Get.Value("FormTextLibrary.Gc_Title.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_Hashtag.Caption = Inventec.Common.Resource.Get.Value("FormTextLibrary.Gc_Hashtag.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("FormTextLibrary.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("FormTextLibrary.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_IsPublic.Caption = Inventec.Common.Resource.Get.Value("FormTextLibrary.Gc_IsPublic.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("FormTextLibrary.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_CreateTime.Caption = Inventec.Common.Resource.Get.Value("FormTextLibrary.Gc_CreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_Creator.Caption = Inventec.Common.Resource.Get.Value("FormTextLibrary.Gc_Creator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_ModifyTime.Caption = Inventec.Common.Resource.Get.Value("FormTextLibrary.Gc_ModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_Modifier.Caption = Inventec.Common.Resource.Get.Value("FormTextLibrary.Gc_Modifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormTextLibrary.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemFocus.Caption = Inventec.Common.Resource.Get.Value("FormTextLibrary.barButtonItemFocus.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemSearch.Caption = Inventec.Common.Resource.Get.Value("FormTextLibrary.barButtonItemSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemChoose.Caption = Inventec.Common.Resource.Get.Value("FormTextLibrary.barButtonItemChoose.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemSave.Caption = Inventec.Common.Resource.Get.Value("FormTextLibrary.barButtonItemSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemNew.Caption = Inventec.Common.Resource.Get.Value("FormTextLibrary.barButtonItemNew.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormTextLibrary.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetDefaultValueControl()
        {
            try
            {
                txtKeyWord.Text = "";
                txtFindHashtag.Text = "";

                txtFindHashtag.Focus();
                txtFindHashtag.SelectAll();

                this.UcDocument.SetRtfText("");
                this.UcPicture.clearImage();

                chkDocument.Checked = true;
                SetDefaultValueControlRight();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControlRight()
        {
            try
            {

                txtTitle.Text = "";
                txtHashtag.Text = "";
                txtHotkey.Text = "";
                this.UcDocument.SetRtfText("");
                this.UcPicture.clearImage();

                if (this._isUsingTextLibraryInfoADO == false)
                {
                    if (String.IsNullOrEmpty(hashtag))
                    {
                        txtFindHashtag.Text = "";
                        txtHashtag.Text = "";
                    }
                    else if (hashtag.ToLower().Contains("danhsachmau"))
                    {

                        txtFindHashtag.Text = this.hashtag.Substring(0, hashtag.Length - 11);
                        barButtonItemSave.Enabled = false;
                        barButtonItemNew.Enabled = false;
                    }
                    else if (hashtag.ToLower().Contains("luumau"))
                    {

                        txtHashtag.Text = this.hashtag.Substring(0, hashtag.Length - 6);
                    }
                    else
                    {
                        txtFindHashtag.Text = hashtag;
                        txtHashtag.Text = hashtag;
                    }
                    if (!string.IsNullOrEmpty(content))
                    {
                        this.UcDocument.SetText(content);
                    }
                }
                else
                {
                    if (this._isNotSaveTemplate)
                    {
                        barButtonItemSave.Enabled = false;
                        barButtonItemNew.Enabled = false;
                        lciBtnSave.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        lciBtnNew.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                    if (this._isFindTemplate)
                    {
                        txtFindHashtag.Text = this.hashtag;
                    }
                    if (this._isFillHashtag)
                    {
                        txtHashtag.Text = this.hashtag;
                    }
                    if (this._isFillContent)
                    {
                        if (!string.IsNullOrEmpty(content))
                        {
                            this.UcDocument.SetText(content);
                        }
                    }
                }

                chkIsPublic.Checked = false;
                chkIsDepartment.Checked = false;
                this.ActionType = GlobalVariables.ActionAdd;
                //txtContent.Appearance.Text.Font = new Font("Times New Roman", 14);
                this.UcDocument.SetFont(new Font("Times New Roman", 14));
                dxValidationProvider.RemoveControlError(txtTitle);
                //               ribbonControlContent.SelectedPage = homeRibbonPage1;
                zoomFactor();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                WaitingManager.Show();
                int pagingSize = ucPaging.pagingGrid != null ? ucPaging.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                GridPaging(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(GridPaging, param, pagingSize);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void GridPaging(object param)
        {
            try
            {
                long? departmentId = null;
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_TEXT_LIB>> apiResult = null;
                MOS.Filter.HisTextLibFilter filter = new MOS.Filter.HisTextLibFilter();
                SetFilter(ref filter);
                gridView.BeginUpdate();
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_TEXT_LIB>>
                    (Base.GlobalStore.HIS_TEXT_LIB_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                //CommonParam paramCo = new CommonParam();
                //HisEmployeeFilter hisFilter = new HisEmployeeFilter();
                //hisFilter.LOGINNAME__EXACT = creator;
                //var employees = new Inventec.Common.Adapter.BackendAdapter
                //    (paramCo).Get<List<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>>
                //    (Base.GlobalStore.HIS_EMPLOYEE, ApiConsumer.ApiConsumers.MosConsumer, hisFilter, paramCo);
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => employees), employees));
                //if (employees != null && employees.Count > 0)
                //{
                //    departmentId = employees.FirstOrDefault().DEPARTMENT_ID;
                //}
                //else
                //{
                //    departmentId = null;
                //}
                //Inventec.Common.Logging.LogSystem.Debug("creator" + creator);
                //Inventec.Common.Logging.LogSystem.Debug("departmentId: " + departmentId);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    Inventec.Common.Logging.LogSystem.Debug("data.Count: " + data.Count);
                    //if (departmentId == null)
                    //{
                    //    data = data.Where(o => o.IS_PUBLIC == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                    //}
                    //else
                    //{
                    //    data = data.Where(o => o.IS_PUBLIC == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE || o.DEPARTMENT_ID == departmentId).ToList();
                    //}
                    if (data != null && data.Count > 0)
                    {
                        gridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControl.DataSource = null;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                        //txtTitle.Text = "";
                        //txtHashtag.Text = "";
                        //txtContent.RtfText = "";
                        //this.UcDocument.SetRtfText("");
                    }
                }
                gridView.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                gridView.EndUpdate();
            }
        }

        private void SetFilter(ref MOS.Filter.HisTextLibFilter filter)
        {
            try
            {
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.KEY_WORD = txtKeyWord.Text.Trim();
                if (!String.IsNullOrEmpty(txtFindHashtag.Text))
                {
                    string code = txtFindHashtag.Text.Trim();
                    code = CheckHashtag(code);
                    txtFindHashtag.Text = code;
                    List<string> arrListStr = txtFindHashtag.Text.Trim().Split(',').ToList();
                    filter.HASHTAGs = arrListStr;
                }
                else filter.HASHTAGs = null;

                filter.CAN_VIEW = true;

                //if (ChooseTextLib)
                //{
                //    filter.CAN_VIEW = true;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    var data = (MOS.EFMODEL.DataModels.HIS_TEXT_LIB)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "IS_PUBLIC_DISPLAY")
                        {
                            if (data.IS_PUBLIC == IS_TRUE)
                            {
                                e.Value = true;
                            }
                            else
                                e.Value = false;

                        }
                        else if (e.Column.FieldName == "DEPARTMENT_ID_STR")
                        {
                            if (data.IS_PUBLIC_IN_DEPARTMENT == IS_TRUE)
                            {
                                e.Value = true;
                            }
                            else
                                e.Value = false;

                        }
                        else if (e.Column.FieldName == "CONTENT_DISPLAY")
                        {

                            if (data.CONTENT != null)
                            {
                                e.Value = HIS.Desktop.Utility.TextLibHelper.BytesToString(data.CONTENT);
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

        private void dxValidationProvider_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.BaseEdit edit = e.InvalidControl as DevExpress.XtraEditors.BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
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

        private void gridView_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTitle.Focus();
                    txtTitle.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void zoomFactor()
        {
            try
            {
                float zoom = 0;
                //if (txtContent.Document.Sections[0].Page.Landscape)
                //    zoom = (float)(txtContent.Width - 50) / (txtContent.Document.Sections[0].Page.Height / 3);
                //else
                //    zoom = (float)(txtContent.Width - 50) / (txtContent.Document.Sections[0].Page.Width / 3);
                //txtContent.ActiveView.ZoomFactor = zoom;

                if (this.UcDocument.getLandscape(0))
                    zoom = (float)(this.UcDocument.getWidth() - 50) / (this.UcDocument.getPageHeight(0) / 3);
                else
                    zoom = (float)(this.UcDocument.getWidth() - 50) / (this.UcDocument.getPageWidth(0) / 3);
                this.UcDocument.SetZoomFactor(zoom);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Click
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValueControlRight();
                txtTitle.Focus();
                txtTitle.SelectAll();
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
                positionHandle = -1;
                if (!btnSave.Enabled) return;
                if (!dxValidationProvider.Validate()) return;
                WaitingManager.Show();
                if (SaveProcess(ref param, ref this.textLib))
                {
                    success = true;
                    SetDefaultValueControl();
                    this.UcDocument.SetRtfText("");
                    this.txtHashtag.Text = "";
                    this.txtFindHashtag.Text = "";
                    FillDataToGrid();
                    if (dataTextLib != null)
                    {
                        this.dataTextLib(this.textLib);
                    }
                }
                WaitingManager.Hide();

                #region Show message
                MessageManager.Show(this, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool SaveProcess(ref CommonParam param, ref MOS.EFMODEL.DataModels.HIS_TEXT_LIB result)
        {
            bool success = false;
            try
            {
                if (this.ActionType == GlobalVariables.ActionAdd)
                    this.textLib = new MOS.EFMODEL.DataModels.HIS_TEXT_LIB();
                SetData(textLib);
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post
                    <MOS.EFMODEL.DataModels.HIS_TEXT_LIB>
                    (this.ActionType == GlobalVariables.ActionAdd ?
                    Base.GlobalStore.HIS_TEXT_LIB_CREATE :
                    Base.GlobalStore.HIS_TEXT_LIB_UPDATE,
                    ApiConsumer.ApiConsumers.MosConsumer, textLib, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (rs != null)
                {
                    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.HIS_TEXT_LIB, MOS.EFMODEL.DataModels.HIS_TEXT_LIB>();
                    result = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.HIS_TEXT_LIB, MOS.EFMODEL.DataModels.HIS_TEXT_LIB>(rs);
                    BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_TEXT_LIB>();
                    success = true;
                }
                return success;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return success;
            }
        }

        private void SetData(MOS.EFMODEL.DataModels.HIS_TEXT_LIB textLib)
        {
            try
            {
                textLib.TITLE = txtTitle.Text;
                textLib.HASHTAG = CheckHashtag(txtHashtag.Text);
                if (chkDocument.Checked)
                {
                    textLib.LIB_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_LIB_TYPE.ID__TEXT;

                    if (!String.IsNullOrEmpty(this.UcDocument.getRtfText()))
                        textLib.CONTENT = Encoding.UTF8.GetBytes(this.UcDocument.getRtfText());
                    else
                        textLib.CONTENT = null;
                }
                if (chkPicture.Checked)
                {

                    textLib.LIB_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_LIB_TYPE.ID__IMAGE;
                    if (!String.IsNullOrEmpty(this.UcPicture.ImageToBase64()))
                    {
                        textLib.CONTENT = Encoding.UTF8.GetBytes(this.UcPicture.ImageToBase64());
                    }
                    else
                    {
                        textLib.CONTENT = null;
                    }
                }

                textLib.IS_PUBLIC = chkIsPublic.Checked ? IS_TRUE : (short)0;
                textLib.IS_PUBLIC_IN_DEPARTMENT = chkIsDepartment.Checked ? IS_TRUE : (short)0;
                textLib.HOT_KEY = txtHotkey.Text;
                Inventec.Common.Logging.LogSystem.Warn(chkIsDepartment.Checked + "");
                if (this.ActionType == GlobalVariables.ActionAdd)
                {
                    CommonParam param = new CommonParam();
                    HisEmployeeFilter filter = new HisEmployeeFilter();
                    filter.LOGINNAME__EXACT = creator;
                    var employees = new Inventec.Common.Adapter.BackendAdapter
                        (param).Get<List<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>>
                        (Base.GlobalStore.HIS_EMPLOYEE, ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                    if (employees != null && employees.Count > 0)
                    {
                        var departmentId = employees.FirstOrDefault().DEPARTMENT_ID;
                        textLib.DEPARTMENT_ID = departmentId;
                    }
                    else
                    {
                        textLib.DEPARTMENT_ID = null;
                    }
                }
                else if (this.ActionType == GlobalVariables.ActionEdit)
                {
                    if (textLib.CREATOR == this.creator)
                    {
                        CommonParam param = new CommonParam();
                        HisEmployeeFilter filter = new HisEmployeeFilter();
                        filter.LOGINNAME__EXACT = creator;
                        var employees = new Inventec.Common.Adapter.BackendAdapter
                            (param).Get<List<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>>
                            (Base.GlobalStore.HIS_EMPLOYEE, ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                        if (employees != null && employees.Count > 0)
                        {
                            var departmentId = employees.FirstOrDefault().DEPARTMENT_ID;
                            textLib.DEPARTMENT_ID = departmentId;
                        }
                        else
                        {
                            textLib.DEPARTMENT_ID = null;
                        }
                    }
                }


            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string CheckHashtag(string s)
        {
            string result = s;
            try
            {
                if (result[0].ToString() != ",") result = string.Format(",{0}", result);
                if (result[result.Length - 1].ToString() != ",") result = string.Format("{0},", result);
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                if (lciBtnChoose.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never) return;
                if (!dxValidationProvider.Validate()) return;
                if (SaveProcess(ref param, ref textLib))
                {
                    if (dataTextLib != null)
                        this.dataTextLib(textLib);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                if (DevExpress.XtraEditors.XtraMessageBox.Show(
                    Resources.ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong,
                    Resources.ResourceMessage.ThongBao,
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    var row = (MOS.EFMODEL.DataModels.HIS_TEXT_LIB)gridView.GetFocusedRow();
                    if (row != null)
                    {
                        WaitingManager.Show();

                        var apiresult = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<bool>
                            (Base.GlobalStore.HIS_TEXT_LIB_DELETE, ApiConsumer.ApiConsumers.MosConsumer, row, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (apiresult)
                        {
                            success = true;
                            FillDataToGrid();
                            BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_TEXT_LIB>();
                        }
                        WaitingManager.Hide();
                        #region Show message
                        MessageManager.Show(this, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }
        #endregion

        #region enter
        private void txtFindHashtag_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtFindHashtag.Text)) FillDataToGrid();
                    else
                    {
                        txtKeyWord.Focus();
                        txtKeyWord.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter) FillDataToGrid();
                if (e.KeyCode == Keys.Down) gridView.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtHotkey_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtHashtag.Focus();
                    txtHashtag.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTitle_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtHotkey.Focus();
                    txtHotkey.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtHashtag_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (chkDocument.Checked)
                    {
                        this.UcDocument.Focus();
                    }
                    else
                    {
                        this.UcPicture.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtHashtag_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (Char.IsLetterOrDigit(e.KeyChar) || Char.IsControl(e.KeyChar) || e.KeyChar == 44 || e.KeyChar == 95)
                {
                    e.Handled = false;
                }
                else
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Validation
        private void ValidControls()
        {
            try
            {
                ValidSereServTempCode();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidSereServTempCode()
        {
            try
            {
                Validation.TitleValidationRule TitleValidationRule = new Validation.TitleValidationRule();
                TitleValidationRule.txtTitle = txtTitle;
                TitleValidationRule.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                TitleValidationRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(txtTitle, TitleValidationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtHashtag_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                string tag = this.txtHashtag.Text;
                char[] text = tag.ToCharArray();
                for (int i = 0; i < text.Length; i++)
                {
                    if (!char.IsLetterOrDigit(text[i]) && text[i] != char.Parse(",") && text[i] != char.Parse("_"))
                        text[i] = char.Parse("_");
                }
                tag = "";
                foreach (var item in text)
                {
                    tag += item;
                }
                this.txtHashtag.Text = tag;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        #endregion

        #region Shortcut
        private void barButtonItemNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barButtonItemChoose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnChoose_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemFocus_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtFindHashtag.Focus();
                txtFindHashtag.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void gridView_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                var row = (MOS.EFMODEL.DataModels.HIS_TEXT_LIB)gridView.GetFocusedRow();
                if (row != null)
                {
                    this.textLib = row;
                    if (row.CONTENT != null)
                    {

                        if (row.LIB_TYPE_ID == 1)
                        {
                            chkDocument.Checked = true;
                            Inventec.Common.Logging.LogSystem.Error(Encoding.UTF8.GetString(row.CONTENT));

                            this.UcDocument.SetRtfText(Encoding.UTF8.GetString(row.CONTENT));

                            this.UcPicture.clearImage();
                        }
                        else
                        {
                            chkPicture.Checked = true;
                            this.UcDocument.SetRtfText("");

                            this.UcPicture.Base64ToImage(Encoding.UTF8.GetString(row.CONTENT));
                        }
                    }
                    else
                    {
                        if (row.LIB_TYPE_ID == 1)
                        {
                            chkDocument.Checked = true;
                            this.UcDocument.SetRtfText("");
                        }
                        else
                        {
                            chkPicture.Checked = true;
                        }
                    }
                    txtTitle.Text = row.TITLE;
                    txtHashtag.Text = row.HASHTAG;
                    txtHotkey.Text = row.HOT_KEY;
                    if (row.IS_PUBLIC == IS_TRUE)
                    {
                        chkIsPublic.Checked = true;
                    }
                    else
                        chkIsPublic.Checked = false;
                    if (row.IS_PUBLIC_IN_DEPARTMENT == IS_TRUE)
                    {
                        chkIsDepartment.Checked = true;
                    }
                    else
                        chkIsDepartment.Checked = false;
                    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    if (row.CREATOR == loginName || CheckLoginAdmin.IsAdmin(loginName))
                    {
                        btnSave.Enabled = true;
                    }
                    else
                    {
                        btnSave.Enabled = false;
                    }
                    zoomFactor();
                    this.ActionType = GlobalVariables.ActionEdit;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Error(row.ID + " ##ERRR");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Khoi Tao uc Document
        /// </summary>
        private void InitDocument()
        {
            try
            {
                this.panelControl1.Controls.Clear();

                if (this.UcDocument != null)
                {
                    this.UcDocument.Dock = DockStyle.Fill;
                    this.panelControl1.Controls.Add(this.UcDocument);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        /// <summary>
        /// Khoi Tao uc Picture
        /// </summary>
        private void InitPicture()
        {
            try
            {
                this.panelControl1.Controls.Clear();

                if (this.UcPicture != null)
                {
                    this.UcPicture.Dock = DockStyle.Fill;
                    this.panelControl1.Controls.Add(this.UcPicture);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkDocument_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkDocument.Checked)
                {
                    //layoutControlItem8
                    //    layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    chkPicture.Checked = false;
                    InitDocument();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPicture_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkPicture.Checked)
                {
                    chkDocument.Checked = false;
                    //       layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    //        ribbonControlContent.Visible = false;
                    InitPicture();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                DevExpress.Utils.DXMouseEventArgs ea = e as DevExpress.Utils.DXMouseEventArgs;
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo info = view.CalcHitInfo(ea.Location);
                if (info.InRow || info.InRowCell)
                {
                    var row = (MOS.EFMODEL.DataModels.HIS_TEXT_LIB)gridView.GetFocusedRow();
                    if (row != null)
                    {
                        this.textLib = row;
                    }
                    if (dataTextLib != null)
                    {
                        this.dataTextLib(this.textLib);
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    HIS_TEXT_LIB data = (HIS_TEXT_LIB)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "DELELTE")
                    {
                        try
                        {
                            string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                            if (data.CREATOR == loginName || CheckLoginAdmin.IsAdmin(loginName))
                                e.RepositoryItem = ButtonDelete;
                            else
                                e.RepositoryItem = ButtonDisnableDelete;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
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
