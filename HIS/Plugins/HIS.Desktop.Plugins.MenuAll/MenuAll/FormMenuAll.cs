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
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.ModuleExt;
using HIS.Desktop.Utility;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Common.Token;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MenuAll.MenuAll
{
    public partial class FormMenuAll : HIS.Desktop.Utility.FormBase
    {
        #region Declare

        Inventec.Desktop.Common.Modules.Module Module;
        MenuAllADO _menuAllADO;
        TileItem tileNew;
        ImageCollection _imageCollection;
        List<Inventec.Desktop.Common.Modules.Module> _listmodules;
        List<Inventec.Desktop.Common.Modules.Module> searchModule;

        #endregion

        #region Construct

        public FormMenuAll()
        {
            InitializeComponent();
        }

        public FormMenuAll(Inventec.Desktop.Common.Modules.Module module)
		:base(module)
        {
            InitializeComponent();
            this.Module = module;
        }

        public FormMenuAll(Inventec.Desktop.Common.Modules.Module module, MenuAllADO menuAllADO)
		:base(module)
        {
            InitializeComponent();
            this.Module = module;
            this._menuAllADO = menuAllADO;
            this._imageCollection = _menuAllADO.ImageCollection;
            this._listmodules = _menuAllADO.ModuleAllInPages;
        }
        #endregion

        private void FormMenuAll_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                txtSearch.Focus();
                txtSearch.SelectAll();
                SetItemByKeyWord();
                SetItemToGroup(_imageCollection, searchModule);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.MenuAll.Resources.Lang", typeof(HIS.Desktop.Plugins.MenuAll.MenuAll.FormMenuAll).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormMenuAll.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tileControl1.Text = Inventec.Common.Resource.Get.Value("FormMenuAll.tileControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tileGroupMenu.Text = Inventec.Common.Resource.Get.Value("FormMenuAll.tileGroupMenu.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("FormMenuAll.txtSearch.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormMenuAll.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void SetItemByKeyWord()
        {
            try
            {
                string keyword = txtSearch.Text.ToLower().Trim();
                searchModule = new List<Module>();
                if (_listmodules != null)
                {
                    searchModule = _listmodules.Where(o => o.text.ToLower().Contains(keyword)).ToList();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        #region Method

        private void SetItemToGroup(ImageCollection imageCollection, List<Inventec.Desktop.Common.Modules.Module> modules)
        {
            try
            {
                if (modules != null && modules.Count > 0)
                {
                    foreach (var item in modules)
                    {
                        var imageIndex = item.ImageIndex;
                        tileNew = new TileItem();
                        tileNew.Name = "barItem__" + _menuAllADO.ModuleAllInPages[0].RoomId + "__" + _menuAllADO.ModuleAllInPages[0].RoomTypeId;
                        tileNew.Text = item.text;
                        tileNew.AppearanceItem.Normal.ForeColor = Color.Black;
                        tileNew.TextAlignment = TileItemContentAlignment.BottomCenter;
                        tileNew.ItemSize = TileItemSize.Medium;

                        if (!String.IsNullOrEmpty(item.Icon))
                        {
                            SetIconMenu(tileNew, imageCollection, item.Icon);
                        }
                        else if (item.ImageIndex > 0)
                        {
                            SetIconMenu(tileNew, imageCollection, imageIndex);
                        }
                        else
                        {
                            SetIconMenu(tileNew, imageCollection, 0);
                        }
                        tileNew.Tag = item;
                        tileNew.ImageAlignment = TileItemContentAlignment.MiddleCenter;
                        Thread.Sleep(10);
                        tileNew.AppearanceItem.Normal.BorderColor = Color.Gray;
                        tileNew.Checked = false;
                        tileNew.Visible = true;
                        tileNew.ItemClick += MenuItemClick;
                        tileNew.AppearanceItem.Normal.BackColor = Color.White;
                        tileGroupMenu.Items.Add(tileNew);
                    }
                }

            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        void MenuItemClick(object sender, TileItemEventArgs e)
        {
            try
            {
                if (e.Item.Tag != null)
                {
                    ButtonMenuProcessor.MenuItemClick(sender, e, tileNew.Name);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Desktop.Common.Message.MessageManager.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay));
                LogSystem.Error(ex);
            }
        }

        void SetIconMenu(TileItem itemTile, ImageCollection imageCollection, string icon)
        {
            try
            {
                if (!String.IsNullOrEmpty(icon))
                    itemTile.Image = imageCollection.Images[icon];
                else
                    itemTile.Image = imageCollection.Images[0];
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        void SetIconMenu(TileItem itemTile, ImageCollection imageCollection, int imageIndex)
        {
            try
            {
                if (imageIndex > -1)
                    itemTile.Image = imageCollection.Images[imageIndex];
                else
                    itemTile.Image = imageCollection.Images[0];
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }


        private void SetClose()
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        #endregion

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    tileGroupMenu.Items.Clear();
                    SetItemByKeyWord();
                    SetItemToGroup(_imageCollection, searchModule);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                
                //SetItemByKeyWord();
                //tileGroupMenu.Items.Clear();
                //SetItemToGroup(_imageCollection, searchModule);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        #region Event
        #endregion

        #region
        #endregion


    }
}
