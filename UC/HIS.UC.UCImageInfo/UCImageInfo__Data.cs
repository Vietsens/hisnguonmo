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
using HIS.UC.UCImageInfo.ADO;
using System.IO;
using System.Drawing.Imaging;

namespace HIS.UC.UCImageInfo
{
    public partial class UCImageInfo : UserControl
    {
        #region Get - Set Data

        public UCImageInfoADO GetValue()
        {
            UCImageInfoADO data = new UCImageInfoADO();
            try
            {
                ProcessImageDataByControl(pteAnhChanDung, ref data);
                ProcessImageDataByControl(pteAnhTheBHYT, ref data);
                ProcessImageDataByControl(pteCmndAfter, ref data);
                ProcessImageDataByControl(pteCmndBefore, ref data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return data;
        }

        private void ProcessImageDataByControl(DevExpress.XtraEditors.PictureEdit pte, ref UCImageInfoADO data)
        {
            try
            {
                if (pte != null && pte.Image != null && ConvertBase64Image(pte.Image) != ConvertBase64Image(Image.FromFile(GetPathDefault())))
                {
                    MemoryStream memory = new MemoryStream();
                    var bitMap = new System.Drawing.Bitmap(pte.Image);
                    bitMap.Save(memory, System.Drawing.Imaging.ImageFormat.Jpeg);

                    if (data.ListImageData == null)
                        data.ListImageData = new List<ImageInfoADO>();

                    ImageInfoADO ado = new ImageInfoADO();

                    if (pte == this.pteAnhChanDung)
                    {
                        ado.Type = Base.ImageType.CHAN_DUNG;
                    }
                    else if (pte == this.pteAnhTheBHYT)
                    {
                        ado.Type = Base.ImageType.THE_BHYT;
                    }
                    else if (pte == this.pteCmndAfter)
                    {
                        ado.Type = Base.ImageType.CMND_CCCD_SAU;
                    }
                    else if (pte == this.pteCmndBefore)
                    {
                        ado.Type = Base.ImageType.CMND_CCCD_TRUOC;
                    }

                    ado.FileImage = memory.ToArray();
                    data.ListImageData.Add(ado);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private string ConvertBase64Image(Image data)
        {
            string result = "";
            Bitmap bmp = new Bitmap(data);
            using (MemoryStream m = new MemoryStream())
            {
                bmp.Save(m, ImageFormat.Jpeg);
                byte[] imageBytes = m.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);

                if (base64String.Length % 4 == 1)
                {
                    result = base64String + "===";
                }
                else if (base64String.Length % 4 == 2)
                {
                    result = base64String + "==";
                }
                else if (base64String.Length % 4 == 3)
                {
                    result = base64String + "=";
                }
                else
                {
                    result = base64String;
                }
            }
            return result;
        }

        public void SetValue(UCImageInfoADO dataImage)
        {
            try
            {
                RefreshUserControl();
                if (dataImage.ListImageData != null && dataImage.ListImageData.Count > 0)
                {
                    foreach (var item in dataImage.ListImageData)
                    {
                        if (!String.IsNullOrEmpty(item.Url))
                        {
                            switch (item.Type)
                            {
                                case HIS.UC.UCImageInfo.Base.ImageType.CHAN_DUNG:
                                    MemoryStream streamChanDung = Inventec.Fss.Client.FileDownload.GetFile(item.Url);
                                    pteAnhChanDung.Image = Image.FromStream(streamChanDung);
                                    pteAnhChanDung.Image.Tag = item.Url;
                                    break;
                                case HIS.UC.UCImageInfo.Base.ImageType.THE_BHYT:
                                    MemoryStream streamTheBHYT = Inventec.Fss.Client.FileDownload.GetFile(item.Url);
                                    pteAnhTheBHYT.Image = Image.FromStream(streamTheBHYT);
                                    pteAnhTheBHYT.Image.Tag = item.Url;
                                    break;
                                case HIS.UC.UCImageInfo.Base.ImageType.CMND_CCCD_TRUOC:
                                    MemoryStream streamCmndBefore = Inventec.Fss.Client.FileDownload.GetFile(item.Url);
                                    pteCmndBefore.Image = Image.FromStream(streamCmndBefore);
                                    pteCmndBefore.Image.Tag = item.Url;
                                    break;
                                case HIS.UC.UCImageInfo.Base.ImageType.CMND_CCCD_SAU:
                                    MemoryStream streamCmndAfter = Inventec.Fss.Client.FileDownload.GetFile(item.Url);
                                    pteCmndAfter.Image = Image.FromStream(streamCmndAfter);
                                    pteCmndAfter.Image.Tag = item.Url;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }

                if (dataImage._FocusNextUserControl != null)
                {
                    this.dlgFocusNextUserControl = dataImage._FocusNextUserControl;
                }

                if (dataImage._ReloadDataByCmndAfter != null)
                {
                    this.ReloadDataByCmndAfter = dataImage._ReloadDataByCmndAfter;
                }

                if (dataImage._ReloadDataByCmndBefore != null)
                {
                    this.ReloadDataByCmndBefore = dataImage._ReloadDataByCmndBefore;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Refresh UserControl
        public void RefreshUserControl()
        {
            try
            {
                ImageDefault(pteAnhChanDung);
                ImageDefault(pteAnhTheBHYT);
                ImageDefault(pteCmndAfter);
                ImageDefault(pteCmndBefore);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ImageDefault(DevExpress.XtraEditors.PictureEdit pte)
        {
            try
            {
                string pathLocal = GetPathDefault();
                pte.Tag = "noImage";
                pte.Image = Image.FromFile(pathLocal);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetPathDefault()
        {
            string imageDefaultPath = string.Empty;
            try
            {
                string localPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                imageDefaultPath = localPath + "\\Img\\ImageStorage\\notImage.jpg";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return imageDefaultPath;
        }
        #endregion
    }
}
