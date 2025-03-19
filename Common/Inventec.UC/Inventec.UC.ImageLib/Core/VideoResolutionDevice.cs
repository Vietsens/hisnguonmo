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

namespace Inventec.UC.ImageLib.Core
{
    public class VideoResolutionDevice
    {
        public VideoResolutionDevice() { }
        public int AverageFrameRate { get; set; }
        public int BitCount { get; set; }
        public AForge.Video.DirectShow.VideoCapabilities VideoCapabilities;
        public string FrameSizeString { get; set; }
        public string FrameSizeDisplay { get; set; }
        public int MaximumFrameRate { get; set; }
        public int FrameRate { get; set; }
    }

    internal class VideoResolutionDeviceLoader
    {
        internal static List<VideoResolutionDevice> GetVideoResolutionDevices(string monikerString)
        {
            List<VideoResolutionDevice> videoResolutionDevices = new List<VideoResolutionDevice>();
            AForge.Video.DirectShow.VideoCaptureDevice videoSource1 = new AForge.Video.DirectShow.VideoCaptureDevice(monikerString);
            var rawVideoCapabilities = videoSource1.VideoCapabilities.ToList();
            videoResolutionDevices = new List<VideoResolutionDevice>();
            foreach (var vCapa in rawVideoCapabilities)
            {
                videoResolutionDevices.Add(new VideoResolutionDevice()
                {
                    AverageFrameRate = vCapa.AverageFrameRate,
                    BitCount = vCapa.BitCount,
                    VideoCapabilities = vCapa,
                    MaximumFrameRate = vCapa.MaximumFrameRate,
                    FrameSizeString = vCapa.FrameSize.ToString(),
                    FrameSizeDisplay = vCapa.FrameSize.Width + " x " + vCapa.FrameSize.Height,
                });
            }

            return videoResolutionDevices;
        }
    }
}
