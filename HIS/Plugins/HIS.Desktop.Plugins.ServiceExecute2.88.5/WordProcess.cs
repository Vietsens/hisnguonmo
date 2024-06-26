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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ServiceExecute
{
    class WordProcess
    {
        internal static void zoomFactor(DevExpress.XtraRichEdit.RichEditControl txtDescription)
        {
            try
            {
                if (txtDescription != null)
                {
                    float zoom = 0;
                    if (txtDescription.Document.Sections[0].Page.Landscape)
                        //zoom = (float)(txtDescription.Width - 50) / (txtDescription.Document.Sections[0].Page.Height / 3);
                        zoom = (float)(txtDescription.Width) / (txtDescription.Document.Sections[0].Page.Height / 3);
                    else
                        //zoom = (float)(txtDescription.Width - 50) / (txtDescription.Document.Sections[0].Page.Width / 3);
                        zoom = (float)(txtDescription.Width) / (txtDescription.Document.Sections[0].Page.Width / 3);
                    txtDescription.ActiveView.ZoomFactor = zoom;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
