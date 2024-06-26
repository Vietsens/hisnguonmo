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
using Inventec.Core;

namespace HIS.UC.WorkPlace.Async
{
    class WorkPlaceGenerateBehavior : IWorkPlaceGenerate
    {
        List<MOS.EFMODEL.DataModels.HIS_WORK_PLACE> worlPlaces;
        WorkPlaceProcessor.Template template;
        DelegateFocusMoveout focusMoveout;
        HIS.UC.WorkPlace.DelegatePlusClick plusClick;
        bool CheckValidate;

        internal WorkPlaceGenerateBehavior(CommonParam param, WorkPlaceInitADO workPlaceInitADO)
        {
            this.worlPlaces = workPlaceInitADO.WorlPlaces;
            this.template = workPlaceInitADO.Template;
            this.focusMoveout = workPlaceInitADO.FocusMoveout;
            this.plusClick = workPlaceInitADO.PlusClick;
            this.CheckValidate = workPlaceInitADO.SetValidateControl;
        }

        async Task<System.Windows.Forms.UserControl> IWorkPlaceGenerate.Run()
        {
            System.Windows.Forms.UserControl uc = null;
            try
            {
                switch (template)
                {
                    case WorkPlaceProcessor.Template.Combo:
                        if (this.CheckValidate)
                        {
                            uc = new UCWorkPlaceCombo(focusMoveout, this.plusClick, this.worlPlaces, this.CheckValidate);
                        }
                        else
                        {
                            uc = new UCWorkPlaceCombo(focusMoveout, this.plusClick, this.worlPlaces);
                        }
                        break;
                    case WorkPlaceProcessor.Template.Textbox:
                        uc = new UCWorkPlaceTextbox(focusMoveout);
                        break;
                    case WorkPlaceProcessor.Template.Combo1:
                        if (this.CheckValidate)
                        {
                            uc = new UCWorkPlaceCombo1(focusMoveout, this.plusClick, this.worlPlaces,this.CheckValidate);
                        }
                        else
                        {
                            uc = new UCWorkPlaceCombo1(focusMoveout, this.plusClick, this.worlPlaces);
                        }
                        
                        break;
                    case WorkPlaceProcessor.Template.Textbox1:
                        uc = new UCWorkPlaceTextbox1(focusMoveout);
                        break;
                    default:
                        uc = null;
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return uc;
        }
    }
}
