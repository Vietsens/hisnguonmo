using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Development.Design.Template1
{
    internal partial class Template1
    {
        internal void SetImageForPicLogo(string path)
        {
            try
            {
                string filePath = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath;
                string pathFileLogo = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filePath), path);
                picLogo.Image = Image.FromFile(pathFileLogo);
                if (picLogo.Image == null)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => path), path));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
