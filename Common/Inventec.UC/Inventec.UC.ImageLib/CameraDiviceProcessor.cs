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
