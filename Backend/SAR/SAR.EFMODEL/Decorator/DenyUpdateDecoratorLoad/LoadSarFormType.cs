using SAR.EFMODEL.DataModels;
using System.Collections.Generic;
using System.Reflection;

namespace SAR.EFMODEL.Decorator
{
    public partial class DenyUpdateDecorator
    {
        private static void LoadSarFormType()
        {
            List<string> pies = new List<string>();
            pies.Add("CREATOR");
            pies.Add("APP_CREATOR");
            pies.Add("CREATE_TIME");

            properties[typeof(SAR_FORM_TYPE)] = pies;
        }
    }
}
