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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.MultiDate
{
    public class MultiDateExt
    {
        string periodsString;     
        PeriodsSet periodsSet;
        public MultiDateExt()
        {         
            periodsSet = new PeriodsSet();
            periodsSet.MergeWith(DateTime.Today, DateTime.Today);          
            periodsString = periodsSet.ToString();
        }     
        public string PeriodsString
        {
            set { periodsString = value; }
            get { return periodsString; }
        }
        public PeriodsSet PeriodsSet
        {
            set { periodsSet = value; }
            get { return periodsSet; }
        }
    }
    public class MultiDateExts : ArrayList
    {
        public new virtual MultiDateExt this[int index] { get { return base[index] as MultiDateExt; } set { base[index] = value; } }
    }
}
