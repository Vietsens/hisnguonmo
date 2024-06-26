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
using HIS.Desktop.ADO;
using HIS.Desktop.Common;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LocalStorage.ConfigSystem;
using Inventec.Common.RichEditor.Base;
using Inventec.Common.RichEditor.DAL;
using Inventec.Desktop.Common.LanguageManager;
using SAR.Desktop.Plugins.SarPrintList.Resources;
using SAR.EFMODEL.DataModels;
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

namespace SAR.Desktop.Plugins.SarPrintList
{
    public partial class frmSarPrintList : HIS.Desktop.Utility.FormBase
    {
        internal string jsonPrintId;
        internal List<SAR.EFMODEL.DataModels.SAR_PRINT> prints { get; set; }
        internal Inventec.Desktop.Common.Modules.Module module { get; set; }
        internal Dictionary<string, object> dicParam;
        internal Dictionary<string, System.Drawing.Image> dicImage;
        internal Inventec.Common.RichEditor.RichEditorStore richEditorMain;
        internal SarPrintADO sarPrintADO;
        DelegateSelectData delegateSarPrintResult;

        public frmSarPrintList()
        {
            InitializeComponent();
        }

        public frmSarPrintList(Inventec.Desktop.Common.Modules.Module _module, SarPrintADO sarPrintADO)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                this.sarPrintADO = sarPrintADO;
                this.module = _module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmSarPrintList_Load(object sender, EventArgs e)
        {
            try
            {
                InitLanguage();
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(Inventec.Desktop.Common.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
                if (sarPrintADO != null)
                {
                    delegateSarPrintResult = sarPrintADO.JsonPrintResult;
                    jsonPrintId = sarPrintADO.JSON_PRINT_ID;
                }
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSarPrint_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    SAR.EFMODEL.DataModels.SAR_PRINT data = (SAR.EFMODEL.DataModels.SAR_PRINT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButton_Delete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                DialogResult myResult = MessageBox.Show(ResourceMessage.BanCoMuonXoaKhongDuLieu, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (myResult == DialogResult.OK)
                {
                    var sarDelete = (SAR.EFMODEL.DataModels.SAR_PRINT)gridViewSarPrint.GetFocusedRow();
                    DeleteJsonPrint(sarDelete);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public bool updateDataSuccess(SAR.EFMODEL.DataModels.SAR_PRINT sarPrint)
        {
            FillDataToGridControl();
            return true;
        }

        private void repositoryItemButton_Edit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var sarPrint = (SAR.EFMODEL.DataModels.SAR_PRINT)gridViewSarPrint.GetFocusedRow();
                richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), Inventec.Desktop.Common.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                richEditorMain.RunPrintEditor(sarPrint, updateDataSuccess);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonEditPrint_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), Inventec.Desktop.Common.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                if (richEditorMain == null) throw new ArgumentNullException("richEditorMain is null");

                var print = (SAR.EFMODEL.DataModels.SAR_PRINT)gridViewSarPrint.GetFocusedRow();
                if (print != null)
                {
                    List<long> currentPrintIds = new List<long>();
                    currentPrintIds.Add(print.ID);
                    richEditorMain.RunPrint(currentPrintIds, dicParam, dicImage, null, ShowPrintPreview);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ShowPrintPreview(byte[] CONTENT_B)
        {
            try
            {
                richEditorMain.ShowPrintPreview(CONTENT_B, null, dicParam, dicImage);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSarPrint_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                SAR_PRINT data = null;
                if (e.RowHandle > -1)
                {
                    var index = gridViewSarPrint.GetDataSourceRowIndex(e.RowHandle);
                    data = (SAR_PRINT)((IList)((BaseView)sender).DataSource)[index];
                }
                if (e.RowHandle >= 0)
                {
                    if (data != null)
                    {
                        string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        if (e.Column.FieldName == "ACTION_EDIT")
                        {
                            if (data.CREATOR == loginName || CheckLoginAdmin.IsAdmin(loginName))
                            {
                                e.RepositoryItem = repositoryItemButton_Edit;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonEdit__Disable;
                            }
                        }
                        else if (e.Column.FieldName == "ACTION_DELETE")
                        {
                            if (this.sarPrintADO.IsFinished == false
                                && (data.CREATOR == loginName || CheckLoginAdmin.IsAdmin(loginName)))
                            {
                                e.RepositoryItem = repositoryItemButton_Delete;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButton_Delete__Disable;
                            }
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
