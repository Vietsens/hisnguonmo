﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReports.Set.MeShow
{
  class SetUCMeShow : ISetUCMeShow
  {
    public void MeShow(System.Windows.Forms.UserControl UC)
    {
      try
      {
        if (UC.GetType() == typeof(Design.Template1.Template1))
        {
          Design.Template1.Template1 UCReports = (Design.Template1.Template1)UC;
          UCReports.MeShow();
        }
        else if (UC.GetType() == typeof(Design.Template2.Template2))
        {
          Design.Template2.Template2 UCReports = (Design.Template2.Template2)UC;
          UCReports.MeShow();
        }
        else if (UC.GetType() == typeof(Design.Template3.Template3))
        {
          Design.Template3.Template3 UCReports = (Design.Template3.Template3)UC;
          UCReports.MeShow();
        }
        else
        {
          Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => UC), UC));
        }
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }
  }
}
