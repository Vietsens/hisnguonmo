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
using HIS.UC.MaterialType.ADO;
using HIS.UC.MaterialType.Focus;
using HIS.UC.MaterialType.GetData;
using HIS.UC.MaterialType.GetListCheck;
using HIS.UC.MaterialType.New;
using HIS.UC.MaterialType.Reload;
using HIS.UC.MaterialType.ResetKeyWord;
using HIS.UC.MaterialType.Run;
using HIS.UC.MaterialType.EnableSave;
using HIS.UC.MaterialType.Search;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.MaterialType
{
    public class MaterialTypeTreeProcessor : BussinessBase
    {
        object uc;
        public MaterialTypeTreeProcessor()
            : base()
        {
        }

        public MaterialTypeTreeProcessor(CommonParam paramBusiness)
            : base(paramBusiness)
        {
        }

        public object Run(MaterialTypeInitADO arg)
        {
            uc = null;
            try
            {
                IRun behavior = RunFactory.MakeIMaterialType(param, arg);
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

        public void Reload(UserControl control, List<MaterialTypeADO> MaterialTypes)
        {
            try
            {
                IReload behavior = ReloadFactory.MakeIReload(param, (control == null ? (UserControl)uc : control), MaterialTypes);
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

        public void New(UserControl control)
        {
            try
            {
                INew behavior = NewFactory.MakeINew(param, (control == null ? (UserControl)uc : control));
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<MaterialTypeADO> GetListCheck(UserControl control)
        {
            List<MaterialTypeADO> result = null;
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
    }
}
