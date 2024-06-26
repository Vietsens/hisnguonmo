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
using HIS.Desktop.LocalStorage.BackendData.ADO;

namespace HIS.Desktop.LocalStorage.BackendData
{
    class VaccinationExam1ADOWorker
    {
        private static VaccinationExamADO vaccinationExam1ADO;

        public static VaccinationExamADO VaccinationExam1ADO
        {
            get
            {
                if (vaccinationExam1ADO == null)
                {
                    vaccinationExam1ADO = new VaccinationExamADO();
                }
                lock (vaccinationExam1ADO) ;
                return vaccinationExam1ADO;
            }
            set
            {
                lock (vaccinationExam1ADO) ;
                vaccinationExam1ADO = value;
            }
        }
    }
}
