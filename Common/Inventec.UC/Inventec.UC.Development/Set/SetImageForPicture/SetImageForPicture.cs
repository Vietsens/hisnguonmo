using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Development.Set.SetImageForPicture
{
    class SetImageForPicture : ISetImageForPicture
    {
        public void SetImage(System.Windows.Forms.UserControl UC, string pathfile)
        {
            try
            {
                if (UC.GetType() == typeof(Design.Template1.Template1))
                {
                    Design.Template1.Template1 UCDevelopment = (Design.Template1.Template1)UC;
                    UCDevelopment.SetImageForPicLogo(pathfile);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
