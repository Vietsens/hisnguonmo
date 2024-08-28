using Inventec.UC.Development.Init;
using Inventec.UC.Development.Set.SetImageForPicture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.Development
{
    public partial class MainDevelopment
    {
        public enum EmumTemp
        {
            TEMPLATE1
        }

        public UserControl Init(EmumTemp enumT, string contentDonVi, string contentPhatTrien)
        {
            UserControl result = null;
            try
            {
                result = InitFactory.MakeIInit().InitUC(enumT, contentDonVi, contentPhatTrien);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public void SetImage(UserControl UC, string pathfile)
        {
            try
            {
                SetImageForPictureFactory.MakeISetImageForPicture().SetImage(UC, pathfile);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
