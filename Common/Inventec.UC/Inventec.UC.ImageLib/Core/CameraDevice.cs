using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ImageLib.Core
{
    public class CameraDevice
    {
        public CameraDevice() { }
        public CameraDevice(string cameraName, string monikerString)
        {
            this.CameraName = cameraName;
            this.MonikerString = monikerString;
        }
        public string CameraName { get; set; }
        public string MonikerString { get; set; }
    }
}
