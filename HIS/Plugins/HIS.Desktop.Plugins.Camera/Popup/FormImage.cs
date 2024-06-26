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
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using DevExpress.Utils;

namespace HIS.Desktop.Plugins.Camera.Popup
{
    public partial class FormImage : Form
    {
        public FormImage()
        {
            InitializeComponent();
        }

        public void CaptureImage(Stream stream)
        {
            try
            {
                    TileItem tileNew = new TileItem();
                    tileNew.Name = "IMG_" + DateTime.Now.ToString("HHmmss");
                    tileNew.Text = "Image_" + DateTime.Now.ToString("HHmmss") + ".jpg";
                    tileNew.AppearanceItem.Normal.ForeColor = Color.Black;
                    tileNew.TextAlignment = TileItemContentAlignment.BottomCenter;
                    tileNew.ItemSize = TileItemSize.Medium;
                    tileNew.Image = Image.FromStream(stream);
                    tileNew.Tag = new Bitmap(tileNew.Image);
                    tileNew.ImageAlignment = TileItemContentAlignment.MiddleCenter;
                    Thread.Sleep(10);
                    tileNew.AppearanceItem.Normal.BorderColor = Color.Green;
                    tileNew.Checked = false;
                    tileNew.Visible = true;
                    tileNew.ItemClick += Image_ItemClick;
                    tileGroup2.Items.Add(tileNew);

      
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void Image_ItemClick(object sender, TileItemEventArgs e)
        {
            try
            {
                e.Item.Checked = !e.Item.Checked;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < tileGroup2.Items.Count; i++)
                {
                    tileGroup2.Items[i].Checked = true;
                }
                List<TileItem> tileItems = tileControl1.GetCheckedItems();
                for (int i = 0; i < tileItems.Count; i++)
                {
                    tileGroup2.Items.Remove(tileItems[i]);
                }

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                List<TileItem> tileItems = tileControl1.GetCheckedItems();
                if (tileItems != null)
                {
                    for (int i = 0; i < tileItems.Count; i++)
                    {
                        tileGroup2.Items.Remove(tileItems[i]);
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                List<TileItem> tileItems = tileControl1.GetCheckedItems();
                if (tileItems != null && tileItems.Count > 0)
                {
                    foreach (var item in tileItems)
                    {
                        Inventec.Common.Logging.LogSystem.Info("item=" + item.Text + " item.Image=" + item.Image.Width + "x" + item.Image.Height);
                        Bitmap file = new Bitmap(item.Image);
                        Inventec.Common.Logging.LogSystem.Info(" file=" + file.Width + "x" + file.Height);
                        (item.Tag as Bitmap).Dispose();
                        file.Save(System.IO.Path.Combine(Inventec.UC.ImageLib.Base.LocalStore.LOCAL_STORAGE_PATH, item.Text), ImageFormat.Png);
                        Inventec.Common.Logging.LogSystem.Info("save file finish");
                        //item.Checked = false;
                        item.Enabled = false;
                    }

                    MessageBox.Show("Lưu thành công");
                }
                else
                {
                    MessageBox.Show("Không có file ảnh nào được chọn");
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void FormImage_Load(object sender, EventArgs e)
        {

        }


    }
}
