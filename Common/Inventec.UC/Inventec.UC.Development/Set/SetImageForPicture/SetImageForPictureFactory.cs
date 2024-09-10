using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Development.Set.SetImageForPicture
{
    class SetImageForPictureFactory
    {
        internal static ISetImageForPicture MakeISetImageForPicture()
        {
            ISetImageForPicture result = null;
            try
            {
                result = new SetImageForPicture();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
