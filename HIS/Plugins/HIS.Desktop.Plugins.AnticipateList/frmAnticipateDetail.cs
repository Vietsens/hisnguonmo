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
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.LocalStorage.Location;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AnticipateList
{
    public partial class frmAnticipateDetail : HIS.Desktop.Utility.FormBase
    {
        private MOS.EFMODEL.DataModels.V_HIS_ANTICIPATE anticipate;
        private List<MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO> anticipateMetiePrints = new List<MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO>();
        public frmAnticipateDetail(MOS.EFMODEL.DataModels.V_HIS_ANTICIPATE Anticipate)
        {
            try
            {
                SetIcon();
                InitializeComponent();
                this.anticipate = Anticipate;


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.AnticipateList.Resources.Lang", typeof(HIS.Desktop.Plugins.AnticipateList.frmAnticipateDetail).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmAnticipateDetail.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("frmAnticipateDetail.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage1.Text = Inventec.Common.Resource.Get.Value("frmAnticipateDetail.xtraTabPage1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmAnticipateDetail.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmAnticipateDetail.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmAnticipateDetail.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmAnticipateDetail.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmAnticipateDetail.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmAnticipateDetail.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmAnticipateDetail.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmAnticipateDetail.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmAnticipateDetail.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmAnticipateDetail.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage2.Text = Inventec.Common.Resource.Get.Value("frmAnticipateDetail.xtraTabPage2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("frmAnticipateDetail.gridColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn18.Caption = Inventec.Common.Resource.Get.Value("frmAnticipateDetail.gridColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn19.Caption = Inventec.Common.Resource.Get.Value("frmAnticipateDetail.gridColumn19.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn20.Caption = Inventec.Common.Resource.Get.Value("frmAnticipateDetail.gridColumn20.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn21.Caption = Inventec.Common.Resource.Get.Value("frmAnticipateDetail.gridColumn21.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn23.Caption = Inventec.Common.Resource.Get.Value("frmAnticipateDetail.gridColumn23.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn24.Caption = Inventec.Common.Resource.Get.Value("frmAnticipateDetail.gridColumn24.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn25.Caption = Inventec.Common.Resource.Get.Value("frmAnticipateDetail.gridColumn25.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage3.Text = Inventec.Common.Resource.Get.Value("frmAnticipateDetail.xtraTabPage3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn30.Caption = Inventec.Common.Resource.Get.Value("frmAnticipateDetail.gridColumn30.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn31.Caption = Inventec.Common.Resource.Get.Value("frmAnticipateDetail.gridColumn31.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn32.Caption = Inventec.Common.Resource.Get.Value("frmAnticipateDetail.gridColumn32.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn33.Caption = Inventec.Common.Resource.Get.Value("frmAnticipateDetail.gridColumn33.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn34.Caption = Inventec.Common.Resource.Get.Value("frmAnticipateDetail.gridColumn34.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn36.Caption = Inventec.Common.Resource.Get.Value("frmAnticipateDetail.gridColumn36.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("frmAnticipateDetail.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmAnticipateDetail.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void frmAnticipateDetail_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                loadDataToGridControlMety();
                loadDataToGridControlMaty();
                loadDataToGridControlBlty();
                ShowTab();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        private void loadDataToGridControlMety()
        {
            try
            {
                if (this.anticipate != null)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisAnticipateMetyViewFilter filter = new MOS.Filter.HisAnticipateMetyViewFilter();
                    filter.ANTICIPATE_ID = this.anticipate.ID;
                    var anticipateMeties = new BackendAdapter(param).Get<List<V_HIS_ANTICIPATE_METY>>(HisRequestUriStore.HIS_ANTICIPATE_METY_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                    if (anticipateMeties != null && anticipateMeties.Count > 0)
                    {
                        foreach (var item in anticipateMeties)
                        {
                            MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO aAnticipateMety = new MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.V_HIS_ANTICIPATE_METY>(aAnticipateMety, item);
                            aAnticipateMety.TotalMoney = item.AMOUNT * (item.IMP_PRICE ?? 0);
                            anticipateMetiePrints.Add(aAnticipateMety);
                        }
                    }
                    gridControlAnticipateMety.DataSource = anticipateMeties;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void loadDataToGridControlMaty()
        {
            try
            {
                if (this.anticipate != null)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisAnticipateMatyViewFilter filter = new MOS.Filter.HisAnticipateMatyViewFilter();
                    filter.ANTICIPATE_ID = this.anticipate.ID;
                    var anticipateMeties = new BackendAdapter(param).Get<List<V_HIS_ANTICIPATE_MATY>>(HisRequestUriStore.HIS_ANTICIPATE_MATY_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                    foreach (var item in anticipateMeties)
                    {
                        MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO aAnticipateMety = new MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.V_HIS_ANTICIPATE_MATY>(aAnticipateMety, item);
                        aAnticipateMety.MEDICINE_TYPE_CODE = item.MATERIAL_TYPE_CODE;
                        aAnticipateMety.MEDICINE_TYPE_NAME = item.MATERIAL_TYPE_NAME;
                        aAnticipateMety.TotalMoney = (item.AMOUNT) * (item.IMP_PRICE ?? 0);
                        anticipateMetiePrints.Add(aAnticipateMety);
                    }
                    gridControlAnticipateMaty.DataSource = anticipateMeties;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ShowTab()
        {
            try
            {
                if (gridControlAnticipateMety.DataSource != null
                    && gridViewAnticipateMety.RowCount > 0) xtraTabControl1.SelectedTabPageIndex = 0;
                else if (gridControlAnticipateMaty.DataSource != null
                    && gridViewAnticipateMaty.RowCount > 0) xtraTabControl1.SelectedTabPageIndex = 1;
                else if (gridControlAnticipateBlty.DataSource != null && gridViewAnticipateBlty.RowCount > 0) xtraTabControl1.SelectedTabPageIndex = 2;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void loadDataToGridControlBlty()
        {
            try
            {
                if (this.anticipate != null)
                {
                    CommonParam param = new CommonParam();
                    MOS.EFMODEL.DataModels.V_HIS_ANTICIPATE_BLTY anticipateLogic = new MOS.EFMODEL.DataModels.V_HIS_ANTICIPATE_BLTY();
                    MOS.Filter.HisAnticipateBltyViewFilter filter = new MOS.Filter.HisAnticipateBltyViewFilter();
                    filter.ANTICIPATE_ID = this.anticipate.ID;
                    var anticipateMeties = new BackendAdapter(param).Get<List<V_HIS_ANTICIPATE_BLTY>>(HisRequestUriStore.HIS_ANTICIPATE_BLTY_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                    if (anticipateMeties != null && anticipateMeties.Count > 0)
                    {
                        foreach (var item in anticipateMeties)
                        {
                            MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO aAnticipateMety = new MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.V_HIS_ANTICIPATE_BLTY>(aAnticipateMety, item);
                            aAnticipateMety.TotalMoney = item.AMOUNT * (item.IMP_PRICE ?? 0);
                            anticipateMetiePrints.Add(aAnticipateMety);
                        }
                    }
                    gridControlAnticipateBlty.DataSource = anticipateMeties;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void gridViewAnticipateMety_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_ANTICIPATE_METY data = (MOS.EFMODEL.DataModels.V_HIS_ANTICIPATE_METY)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                        if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "IMP_PRICE_DISPLAY")
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString(data.IMP_PRICE ?? 0, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void gridViewAnticipateMaty_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_ANTICIPATE_MATY data = (MOS.EFMODEL.DataModels.V_HIS_ANTICIPATE_MATY)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                        if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "IMP_PRICE_DISPLAY")
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString(data.IMP_PRICE ?? 0, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void gridViewAnticipateBlty_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_ANTICIPATE_BLTY data = (MOS.EFMODEL.DataModels.V_HIS_ANTICIPATE_BLTY)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                        if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "IMP_PRICE_DISPLAY")
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString(data.IMP_PRICE ?? 0, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuDuTru__MPS000117, DeletegatePrintTemplate);
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
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
