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
using AForge.Video.DirectShow;
using DirectX.Capture;
using Inventec.UC.ImageLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ImageLib
{
    public class CameraDiviceProcessor
    {
        public static List<CameraDevice> GetSvideoCameraDevices()
        {
            List<CameraDevice> CameraDevices = new List<CameraDevice>();
            try
            {
                var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices.Count == 0)
                {
                    throw new ArgumentNullException("videoDevices.Count == 0");
                }

                CameraDevices = new List<CameraDevice>();
                for (int i = 1, n = videoDevices.Count; i <= n; i++)
                {
                    CameraDevice cam = new CameraDevice(i + " : " + videoDevices[i - 1].Name, videoDevices[i - 1].MonikerString);
                    CameraDevices.Add(cam);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
           
            return CameraDevices;
        }

        public static List<CameraDevice> GetUsbCameraDevices()
        {
            List<CameraDevice> CameraDevices = new List<CameraDevice>();
            try
            {
                Filters filters = new Filters();
                Filter f;
                for (int c = 0; c < filters.VideoInputDevices.Count; c++)
                {
                    f = filters.VideoInputDevices[c];
                    CameraDevice cam = new CameraDevice(c + " : " + f.Name, f.MonikerString);
                    CameraDevices.Add(cam);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return CameraDevices;
        }
    }
}
