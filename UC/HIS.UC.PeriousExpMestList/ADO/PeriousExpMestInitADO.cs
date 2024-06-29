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
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.PeriousExpMestList.ADO
{
    public class PeriousExpMestInitADO
    {
        public LanguageInputADO LanguageInputADO { get; set; }
        public List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_7> listExpMests { get; set; }
        public MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_7 ExpMest { get; set; }
        public HisTreatmentWithPatientTypeInfoSDO Treatment { get; set; }
        public bool? IsShowSearchPanel { get; set; }
        public bool? IsAutoWidth { get; set; }
        public bool? IsNotShowExpMestTypeDTT { get; set; }
        public Action<bool> ActNotShowExpMestTypeDTTCheckedChanged { get; set; }
        public long? ExpMestTypeId { get; set; }
        public gridviewHandler btnView_Click { get; set; }
        public gridviewHandlerList btnSelected_Click { get; set; }
        public gridviewHandler UpdateSingleRow { get; set; }
        public bool? IsPresPK { get; set; }
        public MOS.Filter.HisServiceReqView7Filter ServiceReqView7Filter { get; set; }      
    }
}
