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
using System.Globalization;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.MultiDate
{
    public class Period : IComparable
    {
        DateTime begin, end;
        public Period(DateTime begin, DateTime end)
        {
            this.begin = begin.Date;
            this.end = EndOfDay(end);
        }
        public Period(DateTime date) : this(date, date) { }
        public static DateTime EndOfDay(DateTime date) { return date.Date.AddDays(1).AddSeconds(-1); }
        public static DateTime BeginOfDay(DateTime date) { return date.Date; }
        public DateTime Begin { get { return begin; } set { if (Begin != value) begin = value.Date ; } }
        public DateTime End { get { return end; } set { if (End != value) end = EndOfDay(value); } }
        public int CompareTo(object obj)
        {
            Period dp = obj as Period;
            if (dp != null)
                return this.Begin.CompareTo(dp.Begin);
            else
                throw new ArgumentException("Object is not a DatePeriod");
        }
        public override string ToString()
        {
            if (Begin.Date == End.Date)
                return Begin.ToString("d");
            return Begin.ToString("d") + " - " + End.ToString("d");
        }
        public virtual string ToString(string formatString)
        {
            if (formatString == string.Empty) return ToString();
            if (Begin.Date == End.Date)
                return Begin.ToString(formatString);
            return Begin.ToString(formatString) + " - " + End.ToString(formatString);
        }
        public virtual string ToString(IFormatProvider format)
        {
            if (format == null) return ToString();
            if (Begin.Date == End.Date)
                return Begin.ToString(format);
            return Begin.ToString(format) + " - " + End.ToString(format);
        }
        public static Period Parse(String str, IFormatProvider format)
        {
            str = str.Trim();
            if (str.Contains(" - "))
            {
                bool success = true;
                string[] periodSeparators = new string[1] ;
                periodSeparators[0] = " - ";
                string[] sides = string.Format("{0}", str).Split(periodSeparators, StringSplitOptions.RemoveEmptyEntries);
                DateTime[] dates = new DateTime[2];
                int i = 0;
                foreach (string dateStr in sides)
                {
                    if (i > 1) continue;
                    string stringDate = dateStr.Trim();
                    success = success && DateTime.TryParse(stringDate, format, DateTimeStyles.None, out dates[i]);
                    i++;
                }
                if (success)
                    if (dates[0] <= dates[1])
                        return new Period(dates[0], dates[1]);
            }
            else
            {
                DateTime dt;
                if (DateTime.TryParse(str, format, DateTimeStyles.None, out dt))
                    return new Period(dt);
            }
            return null;
        }
    }
    public class PeriodsSet : IConvertible
    {
        ArrayList periods;
        protected static char DefaultSeparator { get { return ','; } }
        protected static CultureInfo InvariantCulture { get { return new CultureInfo(String.Empty); } }
        public PeriodsSet() { periods = new ArrayList(); }
        public Period this[int index]
        {
            get { return periods[index] as Period; }
            set { periods[index] = value; }
        }
        protected virtual int Add(Period value)
        {
            foreach (Period dp in periods)
            {
                if (dp.Begin.Date == value.End.Date.AddDays(1))
                {
                    dp.Begin = value.Begin;
                    return periods.IndexOf(dp);
                }
                if (dp.End.Date == value.Begin.Date.AddDays(-1))
                {
                    dp.End = value.End;
                    return periods.IndexOf(dp);
                }
            }
            periods.Add(value);
            periods.Sort();
            return periods.IndexOf(value);
        }
        public ArrayList Periods { get { return periods; } }
        public void IntersectWith(DateTime begin, DateTime end)
        {
            if (begin.Date  > end.Date) return;
            begin = Period.BeginOfDay(begin);
            end = Period.EndOfDay(end);
            for (int i = 0; i < Periods.Count; i++)
            {
                if (begin <= this[i].Begin && end >= this[i].End)
                {
                    DateTime oldBegin = this[i].Begin, oldEnd = this[i].End;
                    periods.RemoveAt(i);
                    IntersectWith(begin, oldBegin.AddSeconds(-1));
                    IntersectWith(oldEnd.AddSeconds(1), end);
                    return;
                }
            }
            foreach (Period dp in periods)
                if (begin > dp.Begin && end < dp.End)
                {
                    DateTime periodEnd = dp.End;
                    dp.End = begin.AddSeconds(-1);
                    IntersectWith(end.AddSeconds(1), periodEnd);
                    return;
                }
            for (int i_1 = 0; i_1 < Periods.Count; i_1++)
            {
                if (begin == this[i_1].Begin)
                {
                    this[i_1].Begin = end.AddSeconds(1);
                    return;
                }
                if (end == this[i_1].End)
                {
                    this[i_1].End = begin.AddSeconds(-1);
                    return;
                }
            }
            for (int i_2 = 0; i_2 < Periods.Count; i_2++)
            {
                if (begin >= this[i_2].Begin && begin <= this[i_2].End)
                {
                    DateTime oldEnd = this[i_2].End;
                    this[i_2].End = begin.AddSeconds(-1);
                    begin = oldEnd.AddSeconds(1);
                }
                if (end >= this[i_2].Begin && end <= this[i_2].End)
                {
                    DateTime oldBegin = this[i_2].Begin;
                    this[i_2].Begin = end.AddSeconds(1);
                    end = oldBegin.AddSeconds(-1);
                }
            }
            Add(new Period(begin, end));
        }
        public void MergeWith(DateTime begin, DateTime end)
        {
            if (begin.Date > end.Date) return;
            begin = Period.BeginOfDay(begin);
            end = Period.EndOfDay(end);
            if (ContainPeriod(begin, end)) return;
            for (int i = 0; i < Periods.Count; i++)
                if (begin <= this[i].Begin && end >= this[i].End)
                {
                    periods.RemoveAt(i);
                    MergeWith(begin, end);
                    return;
                }
            Period beginPeriod = null, endPeriod = null;
            for (int i_1 = 0; i_1 < Periods.Count; i_1++)
            {
                if (begin >= this[i_1].Begin && begin <= this[i_1].End) beginPeriod = this[i_1];
                if (end >= this[i_1].Begin && end <= this[i_1].End) endPeriod = this[i_1];
            }
            if (beginPeriod != null && endPeriod != null)
            {
                beginPeriod.End = endPeriod.End;
                periods.Remove(endPeriod);
                return;
            }
            if (beginPeriod != null)
            {
                beginPeriod.End = end;
                return;
            }
            if (endPeriod != null)
            {
                endPeriod.Begin = begin;
                return;
            }
            Add(new Period(begin, end));
        }
        public bool ContainPeriod(object item)
        {
            Period dp = item as Period;
            if (dp != null)
                return ContainPeriod(dp.Begin, dp.End);
            return false;
        }
        public virtual bool ContainPeriod(DateTime begin, DateTime end)
        {
            for (int i = 0; i < Periods.Count; i++)
                if (begin >= this[i].Begin && end <= this[i].End) return true;
            return false;
        }
        public virtual bool ContainPartOfPeriod(DateTime begin, DateTime end)
        {
            if (ContainPeriod(begin, end)) return true;
            for (int i = 0; i < Periods.Count; i++)
                if ((begin <= this[i].Begin && end >= this[i].Begin) || (begin <= this[i].End && end >= this[i].End)) return true;
            return false;
        }
        public virtual PeriodsSet GetCopy()
        {
            PeriodsSet result = new PeriodsSet();
            foreach (Period period in periods)
                result.Add(period);
            return result;
        }
        public virtual string ToString(IFormatProvider format, char separator)
        {
            string str = string.Empty;
            foreach (Period dp in periods)
                str = str + dp.ToString(format) + separator.ToString() + " ";
            if (str.Length > 2) str = str.Remove(str.Length - 2);
            return str;
        }
        public virtual string ToString(string formatString, char separator)
        {
            string str = string.Empty;
            foreach (Period dp in periods)
                str = str + dp.ToString(formatString) + separator.ToString() + " ";
            if (str.Length > 2) str = str.Remove(str.Length - 2);
            return str;
        }
        public override string ToString() { return ToString(InvariantCulture, DefaultSeparator); }
        public static PeriodsSet Parse(string str) { return Parse(str, InvariantCulture, DefaultSeparator); }
        public static PeriodsSet Parse(string str, IFormatProvider format, char separatorChar)
        {
            PeriodsSet result = new PeriodsSet();
            string[] periodSet = string.Format("{0}", str).Split(separatorChar);
            foreach (string periodStr in periodSet)
            {
                Period dp = Period.Parse(periodStr, format);
                if (dp != null) result.Add(dp);
            }
            return result;
        }

        #region IConvertible        
        
        public TypeCode GetTypeCode() {
            throw new NotImplementedException();
        }

        public bool ToBoolean(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public char ToChar(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public sbyte ToSByte(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public byte ToByte(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public short ToInt16(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public ushort ToUInt16(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public int ToInt32(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public uint ToUInt32(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public long ToInt64(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public ulong ToUInt64(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public float ToSingle(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public double ToDouble(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public decimal ToDecimal(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public string ToString(IFormatProvider provider) {
            return this.ToString();
        }

        public object ToType(Type conversionType, IFormatProvider provider) {
            throw new NotImplementedException();
        }
        #endregion
    }
}
