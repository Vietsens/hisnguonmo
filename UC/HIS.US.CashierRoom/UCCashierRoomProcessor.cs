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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.CashierRoom
{
  public class UCCashierRoomProcessor : BussinessBase
  {
    object uc;
    public UCCashierRoomProcessor() : base() { }

    public UCCashierRoomProcessor(Inventec.Core.CommonParam paramBusiness) : base(paramBusiness) { }

    public object run(HIS.UC.CashierRoom.ADO.CashierRoomInitADO arg)
    {
      uc = null;
      try
      {
        HIS.UC.CashierRoom.Run.IRun behavior = HIS.UC.CashierRoom.Run.RunFactory.MakeIExpMestMedicineGrid(param, arg);
        uc = behavior != null ? (behavior.Run()) : null;
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
        uc = null;
      }
      return uc;
    }

    public void Reload(UserControl control, List<CashierRoomADO> data)
    {
      try
      {
        HIS.UC.CashierRoom.Reload.IReload behavior = HIS.UC.CashierRoom.Reload.ReloadFactory.MakeIReload(param, (control == null ? (UserControl)uc : control), data);
        if (behavior != null) behavior.Run();
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    public object GetDataGridView(UserControl control)
    {
      object result = null;
      try
      {
        HIS.UC.CashierRoom.GetDataGridView.IGetDataGridView behavior = HIS.UC.CashierRoom.GetDataGridView.GetDataGridViewFactory.MakeIGetDataGridView
          (param, (control == null ? (UserControl)uc : control));
        result = behavior != null ? behavior.Run() : null;
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
      return result;
    }
  }
}
