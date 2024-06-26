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
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Library.CacheClient;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ContentSubclinical.ADO;
using HIS.Desktop.Plugins.ContentSubclinical.Config;
using HIS.Desktop.Plugins.ContentSubclinical.Print;
using HIS.Desktop.Plugins.ContentSubclinical.Resources;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ContentSubclinical
{
    public partial class frmContentSubclinical : HIS.Desktop.Utility.FormBase
    {
        internal Inventec.Desktop.Common.Modules.Module currentModule;

        long treatmentId = 0;
        long treatmentIdSearch = 0;

        HIS.Desktop.Common.DelegateSelectData _DelegateSelectData;

        ControlStateWorker controlStateWorker;
        List<ControlStateRDO> currentControlStateRDO;
        bool isInit = true;
        bool isReturnObject = false;

        List<HIS_SERVICE_REQ> listServiceReq;
        List<ImageADO> ImageADOs = null;
        bool isInitComboHisPatient = true;
        List<V_HIS_TREATMENT> listHisTreatment;
        string ShowResultWhenReqComplete;

        public frmContentSubclinical()
        {
            InitializeComponent();
        }

        public frmContentSubclinical(Inventec.Desktop.Common.Modules.Module currentModule, long _treatmentId, HIS.Desktop.Common.DelegateSelectData _delegateSelectData, bool returnObject)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this.treatmentId = _treatmentId;
                this._DelegateSelectData = _delegateSelectData;
                this.isReturnObject = returnObject;
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
                this.controlStateWorker = new ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(ControlStateConstant.MODULE_LINK);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == ControlStateConstant.CHECK_JUST_SELECT_INDEX_IMPORTANT)
                        {
                            chkJustSelectIndexImportant.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == ControlStateConstant.CHECK_NOT_SELECT_SURG)
                        {
                            chkNotSelectSurg.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == ControlStateConstant.chkShowMicrobiological)
                        {
                            chkShowMicrobiological.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == ControlStateConstant.chkShowParentServiceGroup)
                        {
                            chkShowParentServiceGroup.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkGetInfo.Name)
                        {
                            chkGetInfo.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmContentSubclinical
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ContentSubclinical.Resources.Lang", typeof(frmContentSubclinical).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmContentSubclinical.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrintKetQua.Text = Inventec.Common.Resource.Get.Value("frmContentSubclinical.btnPrintKetQua.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrintKetQua.ToolTip = Inventec.Common.Resource.Get.Value("frmContentSubclinical.btnPrintKetQua.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkNotSelectSurg.Properties.Caption = Inventec.Common.Resource.Get.Value("frmContentSubclinical.chkNotSelectSurg.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmContentSubclinical.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemSave.Caption = Inventec.Common.Resource.Get.Value("frmContentSubclinical.barButtonItemSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemSearch.Caption = Inventec.Common.Resource.Get.Value("frmContentSubclinical.barButtonItemSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkJustSelectIndexImportant.Properties.Caption = Inventec.Common.Resource.Get.Value("frmContentSubclinical.chkJustSelectIndexImportant.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmContentSubclinical.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutViewColumn1.Caption = Inventec.Common.Resource.Get.Value("frmContentSubclinical.layoutViewColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutViewColumn2.Caption = Inventec.Common.Resource.Get.Value("frmContentSubclinical.layoutViewColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutViewCard1.Text = Inventec.Common.Resource.Get.Value("frmContentSubclinical.layoutViewCard1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkBelow.Properties.Caption = Inventec.Common.Resource.Get.Value("frmContentSubclinical.chkBelow.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAbove.Properties.Caption = Inventec.Common.Resource.Get.Value("frmContentSubclinical.chkAbove.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.Caption = Inventec.Common.Resource.Get.Value("frmContentSubclinical.treeListColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn4.Caption = Inventec.Common.Resource.Get.Value("frmContentSubclinical.treeListColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn5.Caption = Inventec.Common.Resource.Get.Value("frmContentSubclinical.treeListColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmContentSubclinical.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTimeFrom.Text = Inventec.Common.Resource.Get.Value("frmContentSubclinical.lciTimeFrom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem26.Text = Inventec.Common.Resource.Get.Value("frmContentSubclinical.layoutControlItem26.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmContentSubclinical.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciJustSelectIndexImportant.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmContentSubclinical.lciJustSelectIndexImportant.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciJustSelectIndexImportant.Text = Inventec.Common.Resource.Get.Value("frmContentSubclinical.lciJustSelectIndexImportant.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNotSelectSurg.Text = Inventec.Common.Resource.Get.Value("frmContentSubclinical.lciNotSelectSurg.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmContentSubclinical.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmContentSubclinical_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                this.InitControlState();
                SetIconFrm();
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                chkThisTreatment.Checked = true;
                style = NumberStyles.Any;
                ShowResultWhenReqComplete = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ContentSubclinical.ShowResultWhenReqComplete");
                LoadDataSS();
                this.isInit = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataSS();
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (this._DelegateSelectData != null)
                {
                    if (this.isReturnObject)
                    {
                        var dataChecks = (List<TreeSereServADO>)this.GetListCheck();
                        if (dataChecks != null && dataChecks.Count > 0)
                        {
                            dataChecks = dataChecks.OrderBy(p => p.NUM_ORDER).ToList();
                            List<ContentSubclinicalADO> ados = new List<ContentSubclinicalADO>();
                            foreach (var item in dataChecks)
                            {
                                ContentSubclinicalADO a = new ContentSubclinicalADO();
                                a.DESCRIPTION = item.DESCRIPTION;
                                a.NUM_ORDER = item.NUM_ORDER;
                                a.SERVICE_ID = item.SERVICE_ID;
                                a.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                                a.TDL_SERVICE_CODE = item.TDL_SERVICE_CODE;
                                a.TDL_SERVICE_NAME = item.TDL_SERVICE_NAME;
                                a.TDL_SERVICE_TYPE_ID = item.TDL_SERVICE_TYPE_ID;
                                a.TEST_INDEX_NAME = item.TEST_INDEX_NAME;
                                if (item.VALUE.HasValue)
                                    a.VALUE = " = " + item.VALUE.Value.ToString() + " " + item.TEST_INDEX_UNIT_NAME;
                                else
                                    a.VALUE = ((!String.IsNullOrEmpty(item.VALUE_RANGE) && item.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN) ?
                                        String.Format(" = {0} {1}", item.VALUE_RANGE, item.TEST_INDEX_UNIT_NAME) : item.VALUE_RANGE);

                                ados.Add(a);
                            }
                            this._DelegateSelectData(ados);
                        }
                    }
                    else
                    {
                        if (!chkShowParentServiceGroup.Checked)
                        {
                            List<string> _str = new List<string>();
                            var dataChecks = (List<TreeSereServADO>)this.GetListCheck();
                            foreach (var item in dataChecks)
                            {
                                string dienBien = "";
                                string value = item.VALUE_RANGE;
                                if (item.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN && !string.IsNullOrEmpty(value))
                                {
                                    dienBien = item.SERVICE_REQ_CODE + ": " + value + " " + item.TEST_INDEX_UNIT_NAME;
                                }
                                else
                                {
                                    dienBien = item.SERVICE_REQ_CODE + (!string.IsNullOrEmpty(value) ? (": " + value) : "");
                                }
                                if(chkGetInfo.Checked && dicSereServExt != null && dicSereServExt.ContainsKey(item.ID))
                                {
                                    List<string> ext = new List<string>();
                                    if (!string.IsNullOrEmpty(dicSereServExt[item.ID].NOTE))
                                        ext.Add("Ghi chú: " + dicSereServExt[item.ID].NOTE);
                                    if (!string.IsNullOrEmpty(dicSereServExt[item.ID].DESCRIPTION))
                                        ext.Add("Nhận xét: " + dicSereServExt[item.ID].DESCRIPTION);
                                    if (!string.IsNullOrEmpty(dicSereServExt[item.ID].CONCLUDE))
                                        ext.Add("Kết luận: " + dicSereServExt[item.ID].CONCLUDE);
                                    dienBien += " " + string.Join(" - ", ext);
                                } 
                                _str.Add(dienBien);
                            }
                            string _strNews = "";
                            if (_str != null && _str.Count > 0)
                                _strNews = string.Join("; ", _str);

                            this._DelegateSelectData(_strNews);
                        }
                        else
                        {
                            var dataChecks = (List<TreeSereServADO>)this.GetListCheckAndIndeterminate();
                            if (dataChecks != null && dataChecks.Count > 0)
                            {
                                List<TreeSereServADO> listDate = new List<TreeSereServADO>();
                                List<TreeSereServADO> listServiceType = new List<TreeSereServADO>();
                                List<TreeSereServADO> listParentServiceType = new List<TreeSereServADO>();
                                List<TreeSereServADO> listService = new List<TreeSereServADO>();
                                List<TreeSereServADO> listSereServHasMIC = new List<TreeSereServADO>();
                                List<TreeSereServADO> listBacterium = new List<TreeSereServADO>();
                                List<TreeSereServADO> listAntibotic = new List<TreeSereServADO>();
                                List<TreeSereServADO> listTestIndex = new List<TreeSereServADO>();
                                foreach (var dt in dataChecks)
                                {
                                    if (String.IsNullOrEmpty(dt.PARENT_ID__IN_SETY))
                                    {
                                        listDate.Add(dt);
                                    }
                                    else if (dt.IS_SERE_SERV_HAS_MIC)
                                    {
                                        listSereServHasMIC.Add(dt);
                                    }
                                    else if (dt.IS_BACTERIUM)
                                    {
                                        listBacterium.Add(dt);
                                    }
                                    else if (dt.IS_ANTIBIOTIC)
                                    {
                                        listAntibotic.Add(dt);
                                    }
                                    else if (!dt.IS_SERE_SERV_DATA && !dt.IsLeaf && !dt.IsParentServiceType)
                                    {
                                        listServiceType.Add(dt);
                                    }
                                    else if (dt.IsParentServiceType)
                                    {
                                        listParentServiceType.Add(dt);
                                    }
                                    else if (dt.IS_SERE_SERV_DATA && !dt.IsLeaf)
                                    {
                                        listService.Add(dt);
                                    }
                                    else
                                    {
                                        listTestIndex.Add(dt);
                                    }
                                }
                                string content = "";
                                foreach (var date in listDate)
                                {
                                    content += date.SERVICE_REQ_CODE + ":\r\n";
                                    var lstServiceType = listServiceType.Where(o => o.PARENT_ID__IN_SETY == date.CONCRETE_ID__IN_SETY);
                                    foreach (var serviceType in lstServiceType)
                                    {
                                        if (chkShowParentServiceGroup.Checked && serviceType.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)
                                        {
                                            string str = ContentSubclinicalShowParentServiceType(serviceType.CONCRETE_ID__IN_SETY, listService, listTestIndex, listSereServHasMIC, listBacterium, listAntibotic);
                                            content += !string.IsNullOrEmpty(str) ? str + "\r\n" : "";
                                            var lstParentServiceType = listParentServiceType.Where(o => o.PARENT_ID__IN_SETY == serviceType.CONCRETE_ID__IN_SETY).ToList();
                                            if (lstParentServiceType != null && lstParentServiceType.Count > 0)
                                            {
                                                bool isFristLine = true;
                                                foreach (var parentServiceType in lstParentServiceType)
                                                {
                                                    content += !isFristLine ? "\r\n" : "";
                                                    content += parentServiceType.SERVICE_REQ_CODE + "\r\n";
                                                    content += ContentSubclinicalShowParentServiceType(parentServiceType.CONCRETE_ID__IN_SETY, listService, listTestIndex, listSereServHasMIC, listBacterium, listAntibotic);
                                                    isFristLine = false;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            content += serviceType.SERVICE_REQ_CODE + " - ";
                                            content += ContentSubclinicalShowParentServiceType(serviceType.CONCRETE_ID__IN_SETY, listService, listTestIndex, listSereServHasMIC, listBacterium, listAntibotic);
                                        }
                                        if (chkGetInfo.Checked && dicSereServExt != null && dicSereServExt.ContainsKey(date.ID))
                                        {
                                            List<string> ext = new List<string>();
                                            if (!string.IsNullOrEmpty(dicSereServExt[date.ID].NOTE))
                                                ext.Add("Ghi chú: " + dicSereServExt[date.ID].NOTE);
                                            if (!string.IsNullOrEmpty(dicSereServExt[date.ID].DESCRIPTION))
                                                ext.Add("Nhận xét: " + dicSereServExt[date.ID].DESCRIPTION);
                                            if (!string.IsNullOrEmpty(dicSereServExt[date.ID].CONCLUDE))
                                                ext.Add("Kết luận: " + dicSereServExt[date.ID].CONCLUDE);
                                            content += " " + string.Join(" - ", ext);
                                        }
                                        content += "\r\n";
                                    }
                                    content += "\r\n";
                                }
                                this._DelegateSelectData(content);
                            }
                        }
                    }
                    this.Close();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private string ContentSubclinicalShowParentServiceType(string parentIdInSety, List<TreeSereServADO> listService, List<TreeSereServADO> listTestIndex, List<TreeSereServADO> listSereServHasMIC, List<TreeSereServADO> listBacterium, List<TreeSereServADO> listAntibotic)
        {
            string content = "";
            try
            {
                var lstService = listService.Where(o => o.PARENT_ID__IN_SETY == parentIdInSety);
                List<string> _strService = new List<string>();
                foreach (var service in lstService)
                {
                    string serivceStr = "";
                    var lstTestIndex = listTestIndex.Where(o => o.PARENT_ID__IN_SETY == service.CONCRETE_ID__IN_SETY).ToList();
                    bool hasBracket = false;
                    bool kt = chkShowParentServiceGroup.Checked && service.NUM_ORDER == 1 && lstTestIndex != null && lstTestIndex.Count == 1; // Nếu dịch vụ xét nghiệm chỉ có 1 chỉ số, thì chỉ thể hiện tên chỉ số
                    if (!kt)
                    {
                        serivceStr += service.SERVICE_REQ_CODE + (!string.IsNullOrEmpty(service.VALUE_RANGE) ? (": " + service.VALUE_RANGE) : "");
                        if (service.NUM_ORDER == 1)
                        {
                            serivceStr += "(";
                            hasBracket = true;
                        }
                    }
                    List<string> _strTestIndex = new List<string>();
                    foreach (var testIndex in lstTestIndex)
                    {
                        string testIndexStr = "";
                        string value = testIndex.VALUE_RANGE;
                        if (testIndex.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN && !string.IsNullOrEmpty(value))
                        {
                            testIndexStr = testIndex.SERVICE_REQ_CODE + ": " + value + " " + testIndex.TEST_INDEX_UNIT_NAME;
                        }
                        else
                        {
                            testIndexStr = testIndex.SERVICE_REQ_CODE + (!string.IsNullOrEmpty(value) ? (": " + value) : "");
                        }
                        _strTestIndex.Add(testIndexStr);
                    }
                    if (_strTestIndex != null && _strTestIndex.Count > 0)
                        serivceStr += string.Join("; ", _strTestIndex);
                    if (hasBracket)
                    {
                        serivceStr += ")";
                    }
                    _strService.Add(serivceStr);
                }
                if (_strService != null && _strService.Count > 0)
                    content += string.Join("; ", _strService);

                if (!chkImportant.Checked && chkShowMicrobiological.Checked)
                {
                    var lstSereServHasMIC = listSereServHasMIC.Where(o => o.PARENT_ID__IN_SETY == parentIdInSety);
                    List<string> _strSereServHasMIC = new List<string>();
                    foreach (var sereServHasMIC in lstSereServHasMIC)
                    {
                        content += "\r\n" + String.Format("{0}: {1}", sereServHasMIC.SERVICE_REQ_CODE, !string.IsNullOrEmpty(sereServHasMIC.VALUE_RANGE) ? string.Format("({0})", sereServHasMIC.VALUE_RANGE) : "");
                        var lstBacterium = listBacterium.Where(o => o.PARENT_ID__IN_SETY == sereServHasMIC.CONCRETE_ID__IN_SETY).ToList();
                        foreach (var bacterium in lstBacterium)
                        {
                            content += "\r\n- " + bacterium.SERVICE_REQ_CODE + ": ";
                            var lstAntibiotic = listAntibotic.Where(o => o.PARENT_ID__IN_SETY == bacterium.CONCRETE_ID__IN_SETY).ToList();
                            List<string> _strAntibiotics = new List<string>();
                            foreach (var antibiotic in lstAntibiotic)
                            {
                                string strAnti = antibiotic.SERVICE_REQ_CODE;
                                strAnti += !string.IsNullOrEmpty(antibiotic.SRI_CODE) ? string.Format("({0})", antibiotic.SRI_CODE) : "";
                                _strAntibiotics.Add(strAnti);
                            }
                            if (_strAntibiotics != null && _strAntibiotics.Count > 0)
                                content += string.Join(", ", _strAntibiotics);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                content = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return content;
        }

        private void barButtonItemSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void chkJustSelectIndexImportant_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isInit)
                {
                    return;
                }
                WaitingManager.Show();
                ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.CHECK_JUST_SELECT_INDEX_IMPORTANT && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                LogSystem.Debug(LogUtil.TraceData("csAddOrUpdate", csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkJustSelectIndexImportant.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.CHECK_JUST_SELECT_INDEX_IMPORTANT;
                    csAddOrUpdate.VALUE = (chkJustSelectIndexImportant.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                treeSereServ_CheckAllNode(treeListServiceReq.Nodes, true, 2);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkNotSelectSurg_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isInit)
                {
                    return;
                }
                WaitingManager.Show();
                ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.CHECK_NOT_SELECT_SURG && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                LogSystem.Debug(LogUtil.TraceData("csAddOrUpdate", csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkNotSelectSurg.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.CHECK_NOT_SELECT_SURG;
                    csAddOrUpdate.VALUE = (chkNotSelectSurg.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                treeSereServ_CheckAllNode(treeListServiceReq.Nodes, true, 3);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrintKetQua_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessPrintKetQua();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessPrintKetQua()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ProcessPrintKetQua().BEGIN!");
                var dataChecks = (List<TreeSereServADO>)this.GetListCheck_SereServForPrint();
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("__dataChecks", dataChecks));
                List<HIS_SERE_SERV> listSereServ_ForPrint = new List<HIS_SERE_SERV>();
                if (dataChecks != null && dataChecks.Count > 0)
                {
                    dataChecks = dataChecks.OrderBy(p => p.NUM_ORDER).ToList();
                    foreach (var item in dataChecks)
                    {
                        HIS_SERE_SERV itemSereServ = new HIS_SERE_SERV();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV>(itemSereServ, item);
                        listSereServ_ForPrint.Add(itemSereServ);
                    }
                }
                List<long?> listServiceReqIds = dataChecks.Where(o => o.SERVICE_REQ_ID != null).Select(o => o.SERVICE_REQ_ID).Distinct().ToList() ?? new List<long?>();
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("__listServiceReqIds", listServiceReqIds));
                List<HIS_SERVICE_REQ> listServiceReq_ForPrint = new List<HIS_SERVICE_REQ>();
                if (this.listServiceReq != null)
                {
                    listServiceReq_ForPrint = this.listServiceReq.Where(o => listServiceReqIds.Contains(o.ID)).ToList();
                }
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("__listServiceReq_ForPrint", listServiceReq_ForPrint));
                PrintKetQuaProcessor printProcess = new PrintKetQuaProcessor(listServiceReq_ForPrint, listSereServ_ForPrint, this.treatmentId, currentModule.RoomId);
                printProcess.Print();
                Inventec.Common.Logging.LogSystem.Debug("ProcessPrintKetQua().END!");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private HIS_TREATMENT GetHisTreatment(long treatmentId)
        {
            HIS_TREATMENT result = null;
            try
            {
                if (treatmentId > 0)
                {
                    CommonParam param = new CommonParam();
                    HisTreatmentFilter filter = new HisTreatmentFilter();
                    filter.ID = treatmentId;
                    var apiResult = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, SessionManager.ActionLostToken, param);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        result = apiResult.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        private List<V_HIS_TREATMENT> GetListTreatment()
        {
            List<V_HIS_TREATMENT> result = null;
            try
            {
                HIS_TREATMENT hisTreatment = GetHisTreatment(this.treatmentId);
                if (hisTreatment != null)
                {
                    CommonParam param = new CommonParam();
                    HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                    filter.PATIENT_ID = hisTreatment.PATIENT_ID;
                    filter.ORDER_FIELD = "IN_TIME";
                    filter.ORDER_DIRECTION = "DESC";

                    var apiResult = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, SessionManager.ActionLostToken, param);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        var rs = apiResult.Where(o => o.ID != this.treatmentId);
                        if (rs != null)
                            result = rs.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        private void InitDataComboHisTreatment(List<V_HIS_TREATMENT> listHisTreatment)
        {
            try
            {
                if (listHisTreatment != null && listHisTreatment.Count > 0)
                {
                    List<TreatmentComboADO> listSource = new List<TreatmentComboADO>();
                    foreach (var item in listHisTreatment)
                    {
                        TreatmentComboADO ado = new TreatmentComboADO();
                        ado.TreatmentId = item.ID;
                        ado.TreatmentCode = item.TREATMENT_CODE;
                        ado.InTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(item.IN_TIME);
                        ado.EndDepartmentName = item.END_DEPARTMENT_NAME;
                        listSource.Add(ado);
                    }
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("TreatmentCode", "Mã điều trị", 150, 1));
                    columnInfos.Add(new ColumnInfo("InTime", "Ngày khám", 150, 2));
                    columnInfos.Add(new ColumnInfo("EndDepartmentName", "Khoa kết thúc", 200, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("TreatmentCode", "TreatmentId", columnInfos, true, 500);
                    this.cboHisTreatment.Properties.PopupFormSize = new Size(500, this.cboHisTreatment.Properties.PopupFormSize.Height);
                    ControlEditorLoader.Load(this.cboHisTreatment, listSource, controlEditorADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkOtherTreatment_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkOtherTreatment.Checked)
                {
                    cboHisTreatment.Enabled = true;
                    if (isInitComboHisPatient)
                    {
                        isInitComboHisPatient = false;
                        listHisTreatment = GetListTreatment();
                        InitDataComboHisTreatment(listHisTreatment);
                    }
                }
                else
                {
                    cboHisTreatment.EditValue = null;
                    cboHisTreatment.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkShowParentServiceGroup_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isInit)
                {
                    return;
                }
                WaitingManager.Show();
                ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.chkShowParentServiceGroup && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkShowParentServiceGroup.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.chkShowParentServiceGroup;
                    csAddOrUpdate.VALUE = (chkShowParentServiceGroup.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
                LoadDataSS();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkShowMicrobiological_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isInit)
                {
                    return;
                }
                WaitingManager.Show();
                ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.chkShowMicrobiological && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkShowMicrobiological.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.chkShowMicrobiological;
                    csAddOrUpdate.VALUE = (chkShowMicrobiological.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkGetInfo_CheckedChanged(object sender, EventArgs e)
        {

            try
            {
                if (isInit)
                {
                    return;
                }
                WaitingManager.Show();
                ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkGetInfo.Name && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkGetInfo.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkGetInfo.Name;
                    csAddOrUpdate.VALUE = (chkGetInfo.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
