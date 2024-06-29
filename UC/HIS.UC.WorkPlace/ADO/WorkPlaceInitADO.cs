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
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.WorkPlace
{
    public class WorkPlaceInitADO
    {
        public WorkPlaceInitADO()
        {

        }
        public WorkPlaceInitADO(List<MOS.EFMODEL.DataModels.HIS_WORK_PLACE> WorlPlaces, WorkPlaceProcessor.Template Template, DelegateFocusMoveout FocusMoveout, HIS.UC.WorkPlace.DelegatePlusClick plusClick, HIS.UC.WorkPlace.UCWorkPlaceCombo place)
        {
            this.WorlPlaces = WorlPlaces;
            this.Template = Template;
            this.FocusMoveout = FocusMoveout;
            this.PlusClick = plusClick;
            this.place = place;
        }

        public WorkPlaceInitADO(List<MOS.EFMODEL.DataModels.HIS_WORK_PLACE> WorlPlaces, WorkPlaceProcessor.Template Template, DelegateFocusMoveout FocusMoveout, HIS.UC.WorkPlace.DelegatePlusClick plusClick, HIS.UC.WorkPlace.UCWorkPlaceCombo place, bool validate)
        {
            this.WorlPlaces = WorlPlaces;
            this.Template = Template;
            this.FocusMoveout = FocusMoveout;
            this.PlusClick = plusClick;
            this.place = place;
            this.SetValidateControl = validate;
        }

        public HIS.UC.WorkPlace.UCWorkPlaceCombo place { get; set; }
        public List<MOS.EFMODEL.DataModels.HIS_WORK_PLACE> WorlPlaces { get; set; }
        public MOS.EFMODEL.DataModels.HIS_WORK_PLACE thisWorkPlace { get; set; }
        public WorkPlaceProcessor.Template Template { get; set; }
        public DelegateFocusMoveout FocusMoveout { get; set; }
        public HIS.UC.WorkPlace.DelegatePlusClick PlusClick { get; set; }
        public bool SetValidateControl { get; set; }
    }
}
