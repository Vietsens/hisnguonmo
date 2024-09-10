using Inventec.UC.ListReports.Init;
using Inventec.UC.ListReports.Set.Delegate;
using Inventec.UC.ListReports.Set.MeShow;
using Inventec.UC.ListReports.Set.ShortcutButton.Refresh;
using Inventec.UC.ListReports.Set.ShortcutButton.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ListReports
{
  public partial class MainListReports
  {
    public enum EnumTemplate
    {
      TEMPLATE1,
      TEMPLATE2,
      TEMPLATE3,
      //TEMPLATE4
    }

    public UserControl Init(EnumTemplate Template, Inventec.UC.ListReports.Data.InitData Data)
    {
      UserControl result = null;
      try
      {
        result = InitFactory.MakeIInit().InitUC(Template, Data);
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
      return result;
    }

    public bool SetDelegateProcessHasException(UserControl UC, ProcessHasException hasException)
    {
      bool result = false;
      try
      {
        result = SetDelegateHasExceptionFactory.MakeISetDelegateHasException().SetDelegate(UC, hasException);
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
        result = false;
      }
      return result;
    }

    public void MeShowUC(UserControl UC)
    {
      try
      {
        SetUCMeShowFactory.MakeISetUCMeShow().MeShow(UC);
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    public void ButtonSearchClick(UserControl UC)
    {
      try
      {
        SetShortcutButtonSearchFactory.MakeISetShortcutButtonSearch().SearchClick(UC);
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    public void ButtonRefreshClick(UserControl UC)
    {
      try
      {
        SetShortcutButtonRefreshFactory.MakeISetShortcutButtonRefresh().RefreshClick(UC);
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }
  }
}
