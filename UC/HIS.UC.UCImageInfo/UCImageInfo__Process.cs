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
using HIS.Desktop.Utility;
using HIS.Desktop.Common;

namespace HIS.UC.UCImageInfo
{
    public partial class UCImageInfo : UserControl
    {
        private void CallModuleCamera()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.Camera").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.Camera");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add((DelegateSelectData)this.FillImageFromModuleCamereToUC);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(PluginInstance.GetModuleWithWorkingRoom(moduleData, 0, 0), listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillImageFromModuleCamereToUC(object dataImage)
        {
            try
            {
                if (dataImage != null)
                {
                    Image data = (Image)dataImage;

                    switch (this.Type)
                    {
                        case HIS.UC.UCImageInfo.Base.ImageType.CHAN_DUNG:
                            pteAnhChanDung.Image = data;
                            pteAnhChanDung.Tag = data.Tag;
                            pteAnhChanDung.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
                            break;
                        case HIS.UC.UCImageInfo.Base.ImageType.THE_BHYT:
                            pteAnhTheBHYT.Image = data;
                            pteAnhTheBHYT.Tag = data.Tag;
                            pteAnhTheBHYT.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
                            break;
                        case HIS.UC.UCImageInfo.Base.ImageType.CMND_CCCD_TRUOC:
                            pteCmndBefore.Image = data;
                            pteCmndBefore.Tag = data.Tag;
                            pteCmndBefore.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
                            break;
                        case HIS.UC.UCImageInfo.Base.ImageType.CMND_CCCD_SAU:
                            pteCmndAfter.Image = data;
                            pteCmndAfter.Tag = data.Tag;
                            pteCmndAfter.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
                            break;
                        default:
                            break;
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
