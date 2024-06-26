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

using MPS;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ProcessorBase.Core;
using MPS.ProcessorBase;

namespace MPS.Processor.Mps000020.PDO
{
    public partial class Mps000020PDO : RDOBase
    {
        public string departmentName;
        public string bebRoomName;
        public string genderCode__Male;
        public string genderCode__FeMale;
        public string currentDateSeparateFullTime;

        public V_HIS_TREATMENT _Treatment { get; set; }

        public Mps000020PDO() { }

        public Mps000020PDO(
            V_HIS_PATIENT patient,
            string bebRoomName,
            string departmentName,
            HIS_DEBATE currentHisDebate,
            V_HIS_DEPARTMENT_TRAN departmentTran,
            string currentDateSeparateFullTime,
            V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt,
            string genderCode__Male,
            string genderCode__FeMale
            )
        {
            try
            {
                this.Patient = patient;
                this.bebRoomName = bebRoomName;
                this.departmentName = departmentName;
                this.currentHisDebate = currentHisDebate;
                this.departmentTran = departmentTran;
                this.currentDateSeparateFullTime = currentDateSeparateFullTime;
                this.patyAlterBhyt = patyAlterBhyt;
                this.genderCode__Male = genderCode__Male;
                this.genderCode__FeMale = genderCode__FeMale;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public Mps000020PDO(
            V_HIS_PATIENT patient,
            string bebRoomName,
            string departmentName,
            HIS_DEBATE currentHisDebate,
            V_HIS_DEPARTMENT_TRAN departmentTran,
            string currentDateSeparateFullTime,
            V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt
            )
        {
            try
            {
                this.Patient = patient;
                this.bebRoomName = bebRoomName;
                this.departmentName = departmentName;
                this.currentHisDebate = currentHisDebate;
                this.departmentTran = departmentTran;
                this.currentDateSeparateFullTime = currentDateSeparateFullTime;
                this.patyAlterBhyt = patyAlterBhyt;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000020PDO(
            V_HIS_PATIENT patient,
            string bebRoomName,
            string departmentName,
            HIS_DEBATE currentHisDebate,
            V_HIS_DEPARTMENT_TRAN departmentTran,
            string currentDateSeparateFullTime,
            V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt,
            V_HIS_TREATMENT treatment
            )
        {
            try
            {
                this.Patient = patient;
                this.bebRoomName = bebRoomName;
                this.departmentName = departmentName;
                this.currentHisDebate = currentHisDebate;
                this.departmentTran = departmentTran;
                this.currentDateSeparateFullTime = currentDateSeparateFullTime;
                this.patyAlterBhyt = patyAlterBhyt;
                this._Treatment = treatment;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000020PDO(
            V_HIS_PATIENT patient,
            string bebRoomName,
            string departmentName,
            HIS_DEBATE currentHisDebate,
            V_HIS_DEPARTMENT_TRAN departmentTran,
            string currentDateSeparateFullTime,
            V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt,
            V_HIS_TREATMENT treatment,
            V_HIS_DEBATE currentHisDebateView
            )
        {
            try
            {
                this.Patient = patient;
                this.bebRoomName = bebRoomName;
                this.departmentName = departmentName;
                this.currentHisDebateView = currentHisDebateView;
                this.departmentTran = departmentTran;
                this.currentDateSeparateFullTime = currentDateSeparateFullTime;
                this.patyAlterBhyt = patyAlterBhyt;
                this._Treatment = treatment;
                this.currentHisDebate = currentHisDebate;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
