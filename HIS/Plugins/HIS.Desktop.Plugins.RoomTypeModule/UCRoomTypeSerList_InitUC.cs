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
using HIS.UC.RoomTypeList;
using HIS.UC.Module;
using HIS.UC.RoomTypeList.ADO;
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.UC.Module.ADO;
using HIS.Desktop.ADO;
using Inventec.Desktop.Common.Message;
using MOS.Filter;
using DevExpress.XtraBars;
using MOS.SDO;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using DevExpress.XtraGrid.Columns;

namespace HIS.Desktop.Plugins.RoomTypeModule
{
    public partial class UCRoomTypeSerList : HIS.Desktop.Utility.UserControlBase
    {
        private void SetCaptionByLanguageKey()
        {
            try
            {
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = moduleData.text;
                }
                //Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.RoomTypeModule.Resources.Lang", typeof(HIS.Desktop.Plugins.RoomTypeModule.UCRoomTypeSerList).Assembly);

                this.btnSearch1.Text = Inventec.Common.Resource.Get.Value("UCRoomTypeSerList.btnSearch1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch2.Text = Inventec.Common.Resource.Get.Value("UCRoomTypeSerList.btnSearch2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UCRoomTypeSerList.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("UCRoomTypeSerList.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword1.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCRoomTypeSerList.txtKeyword1.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword2.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCRoomTypeSerList.txtKeyword2.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitRoomType()
        {
            try
            {
                roomtypeProcessor = new UCRoomTypeListProcessor();
                RoomTypeListInitADO ado = new RoomTypeListInitADO();
                ado.ListRoomTypeListColumn = new List<UC.RoomTypeList.RoomTypeListColumn>();
                ado.gridViewRoomTypeList_MouseDownRoomTypeList = gridViewRoomType_MouseDownRoomType;
                ado.btn_Radio_Enable_Click = btn_Radio_Enable_Click;
                ado.gridView_MouseRightClick = RoomTypeGridView_MouseRightClick;
                
                RoomTypeListColumn colRadio1 = new RoomTypeListColumn("   ", "radio1", 30, true);
                colRadio1.VisibleIndex = 0;
                colRadio1.Visible = false;
                colRadio1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListRoomTypeListColumn.Add(colRadio1);

                RoomTypeListColumn colCheck1 = new RoomTypeListColumn("   ", "check1", 30, true);
                colCheck1.VisibleIndex = 1;
                colCheck1.image = imgRoomType.Images[0];
                colCheck1.Caption = "Chọn tất cả";
                colCheck1.Visible = false;
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListRoomTypeListColumn.Add(colCheck1);

                RoomTypeListColumn colMaPhong = new RoomTypeListColumn(Inventec.Common.Resource.Get.Value("UCRoomTypeList.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), "ROOM_TYPE_CODE", 60, false);
                colMaPhong.VisibleIndex = 2;
                ado.ListRoomTypeListColumn.Add(colMaPhong);

                RoomTypeListColumn colTenPhong = new RoomTypeListColumn(Inventec.Common.Resource.Get.Value("UCRoomTypeList.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), "ROOM_TYPE_NAME", 100, false);
                colTenPhong.VisibleIndex = 3;
                ado.ListRoomTypeListColumn.Add(colTenPhong);

                this.ucRoomType = (UserControl)roomtypeProcessor.Run(ado);
                if (ucRoomType != null)
                {
                    this.pnlRoomType.Controls.Add(this.ucRoomType);
                    this.ucRoomType.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitModule()
        {
            try
            {
                moduleProcessor = new UCModuleProcessor();
                ModuleInitADO ado = new ModuleInitADO();
                ado.ListModuleColumn = new List<UC.Module.ModuleColumn>();
                ado.gridViewModule_MouseDownModule = gridViewModule_MouseDownModule;
                ado.btn_Radio_Enable_Click1 = btn_Radio_Enable_Click1;
                ado.gridView_MouseRightClick = ModuleGridView_MouseRightClick;

                ModuleColumn colRadio2 = new ModuleColumn("   ", "radio2", 30, true);
                colRadio2.VisibleIndex = 0;
                colRadio2.Visible = false;
                colRadio2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListModuleColumn.Add(colRadio2);

                ModuleColumn colCheck2 = new ModuleColumn("   ", "check2", 30, true);
                colCheck2.VisibleIndex = 1;
                colCheck2.image = imgModule.Images[0];
                colCheck2.Caption = "Chọn tất cả";
                colCheck2.Visible = false;
                colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListModuleColumn.Add(colCheck2);

                ModuleColumn colMaPhong = new ModuleColumn(Inventec.Common.Resource.Get.Value("UCModule.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), "MODULE_NAME", 100, false);
                colMaPhong.VisibleIndex = 2;
                ado.ListModuleColumn.Add(colMaPhong);

                ModuleColumn colTenPhong = new ModuleColumn(Inventec.Common.Resource.Get.Value("UCModule.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), "MODULE_LINK", 200, false);
                colTenPhong.VisibleIndex = 3;
                ado.ListModuleColumn.Add(colTenPhong);

                this.ucModule = (UserControl)moduleProcessor.Run(ado);
                if (ucModule != null)
                {
                    this.pnlRoom.Controls.Add(this.ucModule);
                    this.ucModule.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ModuleGridView_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem) && sender != null && sender is HIS.UC.Module.ModuleADO)
                {
                    var type = (HIS.UC.Module.Popup.PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case HIS.UC.Module.Popup.PopupMenuProcessor.ItemType.Copy:
                            {
                                if (isChoseModule != 2)
                                {
                                    MessageManager.Show(Resources.ResourceMessage.Plugin_VuiLongChonMenu);
                                    break;
                                }
                                this.currentCopyModuleAdo = (HIS.UC.Module.ModuleADO)sender;
                                break;
                            }
                        case HIS.UC.Module.Popup.PopupMenuProcessor.ItemType.Paste:
                            {
                                var currentPaste = (HIS.UC.Module.ModuleADO)sender;
                                bool success = false;
                                CommonParam param = new CommonParam();
                                if (this.currentCopyModuleAdo == null && isChoseModule != 2)
                                {
                                    MessageManager.Show(Resources.ResourceMessage.Plugin_VuiLongCopy);
                                    break;
                                }
                                if (this.currentCopyModuleAdo != null && currentPaste != null && isChoseModule == 2)
                                {
                                    if (this.currentCopyModuleAdo.ID == currentPaste.ID)
                                    {
                                        MessageManager.Show(Resources.ResourceMessage.Plugin_TrungDuLieuCopyVaPaste);
                                        break;
                                    }
                                    HisRotyModuleCopyByModuleSDO hisMestMatyCopyByMatySDO = new HisRotyModuleCopyByModuleSDO();
                                    hisMestMatyCopyByMatySDO.CopyModuleLink = this.currentCopyModuleAdo.MODULE_LINK;
                                    hisMestMatyCopyByMatySDO.PasteModuleLink = currentPaste.MODULE_LINK;
                                    var result = new BackendAdapter(param).Post<List<HIS_ROOM_TYPE_MODULE>>("api/HisRoomTypeModule/CopyByModule", ApiConsumer.ApiConsumers.MosConsumer, hisMestMatyCopyByMatySDO, param);
                                    if (result != null)
                                    {
                                        success = true;
                                        List<HIS.UC.RoomTypeList.RoomTypeListADO> dataNew = new List<HIS.UC.RoomTypeList.RoomTypeListADO>();
                                        dataNew = (from r in listRoomtype select new RoomTypeListADO(r)).ToList();
                                        if (result != null && result.Count > 0)
                                        {

                                            foreach (var itemRoomtypeModule in result)
                                            {
                                                var check = dataNew.FirstOrDefault(o => o.ID == itemRoomtypeModule.ROOM_TYPE_ID);
                                                if (check != null)
                                                {
                                                    check.check1 = true;
                                                }
                                            }

                                            dataNew = dataNew.OrderByDescending(p => p.check1).ToList();
                                            if (ucRoomType != null)
                                            {
                                                roomtypeProcessor.Reload(ucRoomType, dataNew);
                                            }
                                        }
                                        else
                                        {
                                            FillDataToGrid1(this);
                                        }
                                    }
                                }
                                MessageManager.Show(this.ParentForm, param, success);
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RoomTypeGridView_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem) && sender != null && sender is HIS.UC.RoomTypeList.RoomTypeListADO)
                {
                    var type = (HIS.UC.RoomTypeList.Popup.PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case HIS.UC.RoomTypeList.Popup.PopupMenuProcessor.ItemType.Copy:
                            {
                                if (isChoseRoomType != 1)
                                {
                                    MessageManager.Show(Resources.ResourceMessage.Plugin_VuiLongChonPhong);
                                    break;
                                }
                                this.currentCopyRoomTypeAdo = (HIS.UC.RoomTypeList.RoomTypeListADO)sender;
                                break;
                            }
                        case HIS.UC.RoomTypeList.Popup.PopupMenuProcessor.ItemType.Paste:
                            {
                                var currentPaste = (HIS.UC.RoomTypeList.RoomTypeListADO)sender;
                                bool success = false;
                                CommonParam param = new CommonParam();
                                if (this.currentCopyRoomTypeAdo == null && isChoseRoomType != 1)
                                {
                                    MessageManager.Show(Resources.ResourceMessage.Plugin_VuiLongCopy);
                                    break;
                                }
                                if (this.currentCopyRoomTypeAdo != null && currentPaste != null && isChoseRoomType == 1)
                                {
                                    if (this.currentCopyRoomTypeAdo.ID == currentPaste.ID)
                                    {
                                        MessageManager.Show(Resources.ResourceMessage.Plugin_TrungDuLieuCopyVaPaste);
                                        break;
                                    }
                                    HisRotyModuleCopyByRoomTypeSDO hisMestMatyCopyByMatySDO = new HisRotyModuleCopyByRoomTypeSDO();
                                    hisMestMatyCopyByMatySDO.CopyRoomTypeId = this.currentCopyRoomTypeAdo.ID;
                                    hisMestMatyCopyByMatySDO.PasteRoomTypeId = currentPaste.ID;
                                    var result = new BackendAdapter(param).Post<List<HIS_ROOM_TYPE_MODULE>>("api/HisRoomTypeModule/CopyByRoomType", ApiConsumer.ApiConsumers.MosConsumer, hisMestMatyCopyByMatySDO, param);
                                    if (result != null)
                                    {
                                        success = true;
                                        listRoomTypeModule = result;
                                        List<HIS.UC.Module.ModuleADO> dataNew = new List<HIS.UC.Module.ModuleADO>();
                                        dataNew = (from r in listModule select new HIS.UC.Module.ModuleADO(r)).ToList();
                                        if (listRoomTypeModule != null && listRoomTypeModule.Count > 0)
                                        {
                                            foreach (var itemRoom in listRoomTypeModule)
                                            {
                                                var check = dataNew.FirstOrDefault(o => o.MODULE_LINK == itemRoom.MODULE_LINK);
                                                if (check != null)
                                                {
                                                    check.check2 = true;
                                                }
                                            }
                                        }

                                        dataNew = dataNew.OrderByDescending(p => p.check2).ToList();
                                        if (ucModule != null)
                                        {
                                            moduleProcessor.Reload(ucModule, dataNew);
                                        }
                                    }
                                }
                                MessageManager.Show(this.ParentForm, param, success);
                                break;
                            }
                        default:
                            break;
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
