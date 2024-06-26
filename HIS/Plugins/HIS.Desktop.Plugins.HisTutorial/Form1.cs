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
using ACS.EFMODEL.DataModels;
using ACS.Filter;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisTutorial
{
    public partial class Form1 : Form
    {
        const string tutorialFolderRoot = "Tut";
        const string extFile = ".pdf";
        string moduleLink;
        List<V_ACS_MODULE> moduleAccepts;
        V_ACS_MODULE currentModule;
        string fullPath;

        public Form1()
        {
            InitializeComponent();
        }

        public Form1(string modulelink)
        {
            InitializeComponent();
            try
            {
                this.moduleLink = modulelink;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.moduleLink), this.moduleLink));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                this.fullPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, tutorialFolderRoot));
                LoadModuleBar();


                //FillDataToPdfViewer("");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            //webBrowser1.Navigate("https://www.youtube.com/watch?v=UETG1YEdxvo");
        }

        private void LoadModuleBar()
        {
            try
            {
                gridModules.BeginUpdate();
                CommonParam param = new CommonParam();
                AcsModuleViewFilter moduleFilter = new AcsModuleViewFilter();
                moduleFilter.IS_ACTIVE = IMSys.DbConfig.ACS_RS.COMMON.IS_ACTIVE__TRUE;
                moduleFilter.APPLICATION_CODE = GlobalVariables.APPLICATION_CODE;
                //moduleFilter.IS_VISIBLE = GlobalVariables.CommonNumberTrue;
                moduleFilter.ORDER_DIRECTION = "ASC";
                moduleFilter.ORDER_FIELD = "MODULE_NAME";
                var modules = new BackendAdapter(param).Get<List<V_ACS_MODULE>>("api/AcsModule/GetView", ApiConsumers.AcsConsumer, moduleFilter, SessionManager.ActionLostToken, param);
                if (modules != null && modules.Count > 0)
                {
                    var data = modules.Where(o => o.MODULE_LINK == "HIS.Desktop.Plugins.TransactionBillSelect").ToList();
                    this.moduleAccepts = new List<V_ACS_MODULE>();

                    //Kiểm tra va gan danh sach cac file co huong dan su dung
                    if (Directory.Exists(fullPath))
                    {
                        string[] fileEntries = Directory.EnumerateFiles(fullPath, "*.*", SearchOption.AllDirectories)
                            .Where(s => s.EndsWith(extFile)).ToArray();
                        if (fileEntries == null || fileEntries.Count() == 0)
                        {
                            Inventec.Common.Logging.LogSystem.Debug("Khong ton tai file template trong thu muc. Path = " + fullPath + ". " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileEntries), fileEntries));
                        }
                        else
                        {
                            foreach (var item in fileEntries)
                            {
                                int indexg1_3 = item.LastIndexOf("\\");
                                string fName = item.Substring(indexg1_3 + 1, item.Length - indexg1_3 - 5);
                                var mdSearch = modules.FirstOrDefault(o => fName.Equals(o.MODULE_LINK));
                                if (mdSearch != null)
                                {
                                    this.moduleAccepts.Add(mdSearch);
                                }
                                else
                                {
                                    V_ACS_MODULE md = new V_ACS_MODULE();
                                    md.MODULE_LINK = fName;
                                    md.PARENT_NAME = item;
                                    this.moduleAccepts.Add(md);
                                }
                            }
                        }
                    }

                    //Xử lý danh sách các chức năng có video hướng dẫn sd
                    var moduleHasVideos = modules.Where(o => !String.IsNullOrWhiteSpace(o.VIDEO_URLS)).ToList();
                    if (moduleHasVideos != null && moduleHasVideos.Count > 0)
                    {
                        foreach (var item in moduleHasVideos)
                        {
                            var mdInAccept = this.moduleAccepts.Where(o => o.MODULE_LINK == item.MODULE_LINK).FirstOrDefault();
                            if (mdInAccept != null)
                            {
                                //Không làm gì cả
                            }
                            else
                            {
                                V_ACS_MODULE md1 = new V_ACS_MODULE();
                                md1.MODULE_LINK = item.MODULE_LINK;
                                md1.PARENT_NAME = item.MODULE_NAME;
                                md1.VIDEO_URLS = item.VIDEO_URLS;
                                this.moduleAccepts.Add(md1);
                            }
                        }
                    }

                    gridModules.GridControl.DataSource = this.moduleAccepts;
                    if (!String.IsNullOrEmpty(this.moduleLink))
                    {
                        this.searchMenuControl.Text = this.moduleLink;
                    }
                    else
                    {
                        gridModules_FocusedRowChanged();
                    }
                }

                gridModules.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
                
        private void LinkClicked(V_ACS_MODULE md)
        {
            try
            {
                if (md != null)
                {
                    this.moduleLink = md.MODULE_LINK;
                    FillDataToPdfViewer(md);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToPdfViewer(V_ACS_MODULE md)
        {
            FillDataToPdfViewerContent(md);
            FillDataToPdfViewerVideo(md);
        }

        private void FillDataToPdfViewerContent(V_ACS_MODULE md)
        {
            try
            {
                string filePathShow = "";
                string filePathShowName = "";
                if (!String.IsNullOrEmpty(md.MODULE_LINK) && !String.IsNullOrEmpty(md.MODULE_NAME))
                {
                    
                    filePathShow = System.IO.Path.Combine(fullPath, md.MODULE_LINK + extFile);
                    filePathShowName = md.MODULE_LINK;
                }
                else if (!String.IsNullOrEmpty(md.MODULE_LINK))
                {
                    filePathShow = System.IO.Path.Combine(fullPath, md.MODULE_LINK + extFile);
                    filePathShowName = md.MODULE_LINK;
                }

                if (File.Exists(filePathShow))
                {
                    pdfViewer1.LoadDocument(filePathShow);
                    xtraTabControl1.TabPages[0].PageVisible = true;
                }
                else
                {
                    xtraTabControl1.TabPages[0].PageVisible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToPdfViewerVideo(V_ACS_MODULE md)
        {
            try
            {
                foreach (Control item in pnlVideo.Controls)
                {
                    item.Dispose();
                }

                pnlVideo.Controls.Clear();

                List<string> videoByModules = new List<string>();
                if (!String.IsNullOrEmpty(md.VIDEO_URLS))
                {
                    xtraTabControl1.TabPages[1].PageVisible = true;
                    var vdArr = md.VIDEO_URLS.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
                    if (vdArr != null && vdArr.Count() > 0)
                    {
                        //foreach (var url in vdArr)
                        //{
                        //    WebBrowser webBrowserAdd = new System.Windows.Forms.WebBrowser();
                        //    webBrowserAdd.Navigate(url);
                        //    webBrowserAdd.Dock = DockStyle.Fill;
                        //    pnlVideo.Controls.Add(webBrowserAdd);
                        //}
                    }   
                    else
                    {
                        xtraTabControl1.TabPages[1].PageVisible = false;
                    }
                }
                else
                {
                    xtraTabControl1.TabPages[1].PageVisible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void searchControl1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(searchMenuControl.Text.Trim()))
                {
                    gridModules.ActiveFilter.Clear();
                }
                else
                {
                    gridModules.ActiveFilterString = "[MODULE_NAME] Like '%" + searchMenuControl.Text
                                + "%' OR [MODULE_GROUP_NAME] Like '%" + searchMenuControl.Text + "%'"
                            + " OR [MODULE_LINK] Like '%" + searchMenuControl.Text + "%'";
                    //+ " OR [CONCENTRA] Like '%" + txtMediMatyForPrescription.Text + "%'"
                    //+ " OR [MEDI_STOCK_NAME] Like '%" + txtMediMatyForPrescription.Text + "%'";
                    gridModules.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                    gridModules.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                    gridModules.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                    gridModules.FocusedRowHandle = 0;
                    gridModules.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
                    gridModules.OptionsFind.HighlightFindResults = true;
                }
                if (gridModules.RowCount > 0)
                {
                    gridModules_FocusedRowChanged();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridModules_FocusedRowChanged()
        {
            try
            {
                var md = (V_ACS_MODULE)this.gridModules.GetFocusedRow();
                if (md != null && md.MODULE_LINK != (this.currentModule ?? new V_ACS_MODULE()).MODULE_LINK)
                {
                    this.currentModule = md;
                    LinkClicked(this.currentModule);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void gridModules_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_ACS_MODULE vModule = (V_ACS_MODULE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (vModule != null)
                    {
                        if (e.Column.FieldName == "MODULE_NAME_DISPLAY")
                        {
                            if (!String.IsNullOrEmpty(vModule.MODULE_NAME) && !String.IsNullOrEmpty(vModule.MODULE_LINK))
                            {
                                e.Value = (vModule.MODULE_NAME + " - " + vModule.MODULE_LINK);
                            }
                            else if (!String.IsNullOrEmpty(vModule.MODULE_NAME))
                            {
                                e.Value = (vModule.MODULE_NAME);
                            }
                            else if (!String.IsNullOrEmpty(vModule.MODULE_LINK))
                            {
                                e.Value = (vModule.MODULE_LINK);
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

        private void gridModules_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void gridModules_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            gridModules_FocusedRowChanged();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (Control item in pnlVideo.Controls)
            {
                item.Dispose();
            }
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                if (xtraTabControl1.SelectedTabPage == xtraTabPageVideos)
                {
                    var vdArr = currentModule.VIDEO_URLS.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
                    if (vdArr != null && vdArr.Count() > 0)
                    {
                        foreach (var url in vdArr)
                        {
                            WebBrowser webBrowserAdd = new System.Windows.Forms.WebBrowser();
                            webBrowserAdd.Navigate(url);
                            webBrowserAdd.Dock = DockStyle.Fill;
                            pnlVideo.Controls.Add(webBrowserAdd);
                        }
                    }
                    else
                    {
                        xtraTabControl1.TabPages[1].PageVisible = false;
                    }
                }
                else
                {
                    foreach (Control item in pnlVideo.Controls)
                    {
                        item.Dispose();
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
