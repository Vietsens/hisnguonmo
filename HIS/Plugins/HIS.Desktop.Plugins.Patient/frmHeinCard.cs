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
using Inventec.Common.Adapter;
using Inventec.Core;
using HIS.Desktop.Plugins.Patient.Base;
using MOS.EFMODEL.DataModels;
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
using MOS.Filter;
using Inventec.Desktop.Common.LocalStorage.Location;
using System.Configuration;
using System.Globalization;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.Patient
{
    public partial class frmHeinCard : HIS.Desktop.Utility.FormBase
    {
        private long patientId;
        List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER> patyAlterBhyts = new List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>();
        private static CultureInfo cultureInfo;

        public frmHeinCard(long PatientId)
        {
            try
            {
                patientId = PatientId;
                SetIcon();
                InitializeComponent();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void LoadKeysFromlanguage()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.Patient.Resources.Lang", typeof(frmHeinCard).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHeinCard.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHeinCard.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColHeinCardNumber.Caption = Inventec.Common.Resource.Get.Value("frmHeinCard.gridColHeinCardNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColFromDate.Caption = Inventec.Common.Resource.Get.Value("frmHeinCard.gridColFromDate.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColToDate.Caption = Inventec.Common.Resource.Get.Value("frmHeinCard.gridColToDate.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColPatientName.Caption = Inventec.Common.Resource.Get.Value("frmHeinCard.gridColPatientName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColHeinMediOrg.Caption = Inventec.Common.Resource.Get.Value("frmHeinCard.gridColHeinMediOrg.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHeinCard.gridColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHeinCard.gridColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHeinCard.gridColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColModifier.Caption = Inventec.Common.Resource.Get.Value("frmHeinCard.gridColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHeinCard.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmHeinCard_Load(object sender, EventArgs e)
        {
            try
            {
                Language_HeinCard();
                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientTypeAlterViewFilter filter = new MOS.Filter.HisPatientTypeAlterViewFilter();
                filter.TDL_PATIENT_ID = this.patientId;
                var heinCard = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_VIEW, ApiConsumers.MosConsumer, filter, param).Where(o => o.HEIN_CARD_NUMBER != null).ToList();
                LoadKeysFromlanguage();
                if (heinCard != null && heinCard.Count > 0)
                {
                    gridControlHeinCard.DataSource = heinCard;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void Language_HeinCard()
        {
            try
            {
                //cultureInfo = new CultureInfo("vi");
                gridColStt.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_HEIN_CARD_GRID_COL_STT", Base.ResourceLangManager.LanguageFrmHeinCard, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //this.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LANGUAGE_FRM_HEIN_CARD_FRM_HEIN_CARD", Base.ResourceLangManager.LanguageFrmHeinCard, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                gridColHeinCardNumber.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_HEIN_CARD_GRID_COL_HEIN_CARD_NUMBER", Base.ResourceLangManager.LanguageFrmHeinCard, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                gridColFromDate.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_HEIN_CARD_GRID_COL_FROM_DATE", Base.ResourceLangManager.LanguageFrmHeinCard, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                gridColToDate.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_HEIN_CARD_GRID_COL_TO_DATE", Base.ResourceLangManager.LanguageFrmHeinCard, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                gridColHeinCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_HEIN_CARD_GRID_COL_HEIN_CODE", Base.ResourceLangManager.LanguageFrmHeinCard, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                gridColHeinMediOrg.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_HEIN_CARD_GRID_COL_HEIN_MEDI_ORG", Base.ResourceLangManager.LanguageFrmHeinCard, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                gridColCreateTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_HEIN_CARD_GRID_COL_CREATE_TIME", Base.ResourceLangManager.LanguageFrmHeinCard, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                gridColCreator.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_HEIN_CARD_GRID_COL_CREATOR", Base.ResourceLangManager.LanguageFrmHeinCard, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                gridColModifyTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_HEIN_CARD_GRID_COL_MODIFY_TIME", Base.ResourceLangManager.LanguageFrmHeinCard, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                gridColModifier.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_HEIN_CARD_GRID_COL_MODIFIER", Base.ResourceLangManager.LanguageFrmHeinCard, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewHeinCard_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "FROM_DATE_DISPLAY")
                    {
                        try
                        {
                            string fromDate = (view.GetRowCellValue(e.ListSourceRowIndex, "HEIN_CARD_FROM_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(Inventec.Common.TypeConvert.Parse.ToInt64(fromDate));
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "TO_DATE_DISPLAY")
                    {
                        try
                        {
                            string toDate = (view.GetRowCellValue(e.ListSourceRowIndex, "HEIN_CARD_TO_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(Inventec.Common.TypeConvert.Parse.ToInt64(toDate));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                    {
                        try
                        {
                            string MODIFY_TIME = (view.GetRowCellValue(e.ListSourceRowIndex, "MODIFY_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(MODIFY_TIME));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao MODIFY_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                    {
                        try
                        {
                            string DOB = (view.GetRowCellValue(e.ListSourceRowIndex, "CREATE_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(DOB));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao DOB", ex);
                        }
                    }
                }
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
    }
}
