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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TrackingInMediRecord.ADO
{
    public class TrackingADO : HIS_TRACKING
    {
        public bool IsCheck { get; set; }
        public string TREATMENT_CODE { get; set; }
        public long IN_TIME { get; set; }
        public long? OUT_TIME { get; set; }

        public TrackingADO(HIS_TRACKING tracking)
        {
            if (tracking != null)
            {
                this.AWARENESS_BEHAVIOR = tracking.AWARENESS_BEHAVIOR;
                this.CARDIOVASCULAR = tracking.CARDIOVASCULAR;
                this.CARE_INSTRUCTION = tracking.CARE_INSTRUCTION;
                this.CONCENTRATION = tracking.CONCENTRATION;
                this.CONTENT = tracking.CONTENT;
                this.CONTENT_OF_THINKING = tracking.CONTENT_OF_THINKING;
                this.CREATE_TIME = tracking.CREATE_TIME;
                this.CREATOR = tracking.CREATOR;
                this.DEPARTMENT_ID = tracking.DEPARTMENT_ID;
                this.EMOTION = tracking.EMOTION;
                this.EYE_TENSION_LEFT = tracking.EYE_TENSION_LEFT;
                this.EYE_TENSION_RIGHT = tracking.EYE_TENSION_RIGHT;
                this.EYESIGHT_GLASS_LEFT = tracking.EYESIGHT_GLASS_LEFT;
                this.EYESIGHT_GLASS_RIGHT = tracking.EYESIGHT_GLASS_RIGHT;
                this.EYESIGHT_LEFT = tracking.EYESIGHT_LEFT;
                this.EYESIGHT_RIGHT = tracking.EYESIGHT_RIGHT;
                this.FORM_OF_THINKING = tracking.FORM_OF_THINKING;
                this.GENERAL_EXPRESSION = tracking.GENERAL_EXPRESSION;
                this.ICD_CODE = tracking.ICD_CODE;
                this.ICD_ID__DELETE = tracking.ICD_ID__DELETE;
                this.ICD_NAME = tracking.ICD_NAME;
                this.ICD_SUB_CODE = tracking.ICD_SUB_CODE;
                this.ICD_TEXT = tracking.ICD_TEXT;
                this.ID = tracking.ID;
                this.INSTINCTIVELY_BEHAVIOR = tracking.INSTINCTIVELY_BEHAVIOR;
                this.INTELLECTUAL = tracking.INTELLECTUAL;
                this.IS_ACTIVE = tracking.IS_ACTIVE;
                this.IS_DELETE = tracking.IS_DELETE;
                this.MEDICAL_INSTRUCTION = tracking.MEDICAL_INSTRUCTION;
                this.MEMORY = tracking.MEMORY;
                this.MODIFIER = tracking.MODIFIER;
                this.MODIFY_TIME = tracking.MODIFY_TIME;
                this.ORIENTATION_CAPACITY = tracking.ORIENTATION_CAPACITY;
                this.PERCEPTION = tracking.PERCEPTION;
                this.RESPIRATORY = tracking.RESPIRATORY;
                this.ROOM_ID = tracking.ROOM_ID;
                this.SHEET_ORDER = tracking.SHEET_ORDER;
                this.SUBCLINICAL_PROCESSES = tracking.SUBCLINICAL_PROCESSES;
                this.TRACKING_TIME = tracking.TRACKING_TIME;
                this.TRADITIONAL_ICD_CODE = tracking.TRADITIONAL_ICD_CODE;
                this.TRADITIONAL_ICD_NAME = tracking.TRADITIONAL_ICD_NAME;
                this.TRADITIONAL_ICD_SUB_CODE = tracking.TRADITIONAL_ICD_SUB_CODE;
                this.TRADITIONAL_ICD_TEXT = tracking.TRADITIONAL_ICD_TEXT;
                this.TREATMENT_ID = tracking.TREATMENT_ID;
            }
        }
    }
}
