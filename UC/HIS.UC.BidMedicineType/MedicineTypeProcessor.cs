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
using HIS.UC.MedicineType.GetListCheck;
using HIS.UC.MedicineType.Reload;
using HIS.UC.MedicineType.Run;
using HIS.UC.MedicineType.Search;
using HIS.UC.MedicineType.ADO;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.MedicineType.Focus;
using HIS.UC.MedicineType.GetData;
using HIS.UC.MedicineType.ResetKeyWord;
using HIS.UC.MaterialType.EnableSave;

namespace HIS.UC.MedicineType
{
  public class MedicineTypeProcessor : BussinessBase
  {
    object uc;
    public MedicineTypeProcessor()
      : base()
    {
    }

    public MedicineTypeProcessor(CommonParam paramBusiness)
      : base(paramBusiness)
    {
    }

    public object Run(MedicineTypeInitADO arg)
    {
      uc = null;
      try
      {
        IRun behavior = RunFactory.MakeIMedicineType(param, arg);
        uc = behavior != null ? (behavior.Run()) : null;
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
        uc = null;
      }
      return uc;
    }

    public void Search(UserControl control)
    {
      try
      {
        ISearch behavior = SearchFactory.MakeISearch(param, (control == null ? (UserControl)uc : control));
        if (behavior != null) behavior.Run();
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    public void EnableSaveButton(UserControl control, bool _enable)
    {
        try
        {
            IEnableSave behavior = EnableSaveFactory.MakeIEnableSave((control == null ? (UserControl)uc : control), _enable);
            if (behavior != null) behavior.Run();
        }
        catch (Exception ex)
        {
            Inventec.Common.Logging.LogSystem.Error(ex);
        }
    }

    public void Reload(UserControl control, List<MedicineTypeADO> MedicineTypes)
    {
      try
      {
        IReload behavior = ReloadFactory.MakeIReload(param, (control == null ? (UserControl)uc : control), MedicineTypes);
        if (behavior != null) behavior.Run();
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    public List<MedicineTypeADO> GetListCheck(UserControl control)
    {
      List<MedicineTypeADO> result = null;
      try
      {
        IGetListCheck behavior = GetListCheckFactory.MakeIGetListCheck(control);
        result = (behavior != null) ? behavior.Run() : null;
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
        result = null;
      }
      return result;
    }

    public void FocusKeyword(UserControl uc)
    {
      try
      {
        IFocus behavior = FocusFactory.MakeIFocus(uc);
        if (behavior != null) behavior.Run();
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    public void ResetKeyword(UserControl uc)
    {
        try
        {
            IResetKeyWord behavior = ResetKeyWordFactory.MakeIResetKeyWord(uc);
            if (behavior != null) behavior.Run();
        }
        catch (Exception ex)
        {
            Inventec.Common.Logging.LogSystem.Error(ex);
        }
    }


    public object GetData(UserControl control)
    {
      object result = null;
      //object uc = null;
      try
      {
        IGetData behavior = GetDataFactory.MakeIGetData(param, (control == null ? (UserControl)uc : control));
        result = (behavior != null) ? behavior.Run() : null;
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
      return result;
    }
  }
}
