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
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentLockList
{
    public partial class frmTreatmentLockList : HIS.Desktop.Utility.FormBase
    {
        long treatmentId;

        Inventec.Desktop.Common.Modules.Module currentModule = null;

        public frmTreatmentLockList(Inventec.Desktop.Common.Modules.Module module, long data)
            : base(module)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                SetIcon();
                this.treatmentId = data;
                this.currentModule = module;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
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
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmTreatmentLockList_Load(object sender, EventArgs e)
        {
            try
            {
                LoadKeyFrmLanguage();
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridControl()
        {
            try
            {
                if (this.treatmentId > 0)
                {
                    CommonParam param = new CommonParam();
                    HisTreatmentLoggingFilter filter = new HisTreatmentLoggingFilter();
                    filter.TREATMENT_ID = this.treatmentId;
                    var listTreatmentLogging = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT_LOGGING>>("api/HisTreatmentLogging/GetView", ApiConsumers.MosConsumer, filter, param);

                    if (listTreatmentLogging != null && listTreatmentLogging.Count > 0)
                    {
                        listTreatmentLogging = listTreatmentLogging.OrderBy(o => o.CREATE_TIME).ThenBy(o => o.ID).ToList();
                    }
                    gridControlTreatmentLock.BeginUpdate();
                    gridControlTreatmentLock.DataSource = listTreatmentLogging;
                    gridControlTreatmentLock.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatmentLock_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HIS_TREATMENT_LOGGING)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        //else if (e.Column.FieldName == "IS_LOCK_STR")
                        //{
                        //    try
                        //    {
                        //        if (data. == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_LOCK.IS_LOCK__TRUE)
                        //        {
                        //            e.Value = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TREATMENT_LOCK_LIST__ACTION__LOCK", Base.ResourceLangManager.LanguageFrmTreatmentLockList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                        //        }
                        //        else
                        //        {
                        //            e.Value = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TREATMENT_LOCK_LIST__ACTION__UNLOCK", Base.ResourceLangManager.LanguageFrmTreatmentLockList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                        //        }
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        Inventec.Common.Logging.LogSystem.Error(ex);
                        //    }
                        //}
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "USER_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.USER_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void LoadKeyFrmLanguage()
        {
            try
            {
                //GridControl TreatmentLock
                this.gridColumn_TreatmentLock_CreateTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TREATMENT_LOCK_LIST__GRID_TREATMENT_LOCK__COLUMN_CREATE_TIME", Base.ResourceLangManager.LanguageFrmTreatmentLockList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_TreatmentLock_IsLock.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TREATMENT_LOCK_LIST__GRID_TREATMENT_LOCK__COLUMN_IS_LOCK", Base.ResourceLangManager.LanguageFrmTreatmentLockList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_TreatmentLock_LockerLoginname.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TREATMENT_LOCK_LIST__GRID_TREATMENT_LOCK__COLUMN_LOCKER_LOGINNAME", Base.ResourceLangManager.LanguageFrmTreatmentLockList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_TreatmentLock_Stt.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TREATMENT_LOCK_LIST__GRID_TREATMENT_LOCK__COLUMN_STT", Base.ResourceLangManager.LanguageFrmTreatmentLockList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_TreatmentLock_FeeLockTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TREATMENT_LOCK_LIST__GRID_TREATMENT_LOCK__COLUMN_FEE_LOCK_TIME", Base.ResourceLangManager.LanguageFrmTreatmentLockList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_TreatmentLock_FeeLockTime.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TREATMENT_LOCK_LIST__GRID_TREATMENT_LOCK__COLUMN_FEE_LOCK_TIME_TOOLTIP", Base.ResourceLangManager.LanguageFrmTreatmentLockList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //Caption Frm
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
