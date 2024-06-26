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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using System;
using System.ComponentModel;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.MultiDate
{
    [UserRepositoryItem("RegisterDatePeriodEdit")]
    public class RepositoryItemDatePeriodEdit : RepositoryItemDateEdit
    {
       public const string DatePeriodEditName = "DatePeriodEdit";
       char _separatorChar = ',';

       static RepositoryItemDatePeriodEdit()
       {
           RegisterDatePeriodEdit();
       }

       public RepositoryItemDatePeriodEdit()
       {
           TextEditStyle = TextEditStyles.DisableTextEditor;
           ShowOk = DefaultBoolean.True;
       }

       public override string EditorTypeName {get{return DatePeriodEditName;}}

       [Description("Gets or sets the character separating periods"), Category(CategoryName.Format), DefaultValue(',')]
       public char SeparatorChar
       {
           get { return _separatorChar; }
           set
           {
               if (SeparatorChar == value) return;
               _separatorChar = value;
               OnPropertiesChanged();
           }
       }       

       public static void RegisterDatePeriodEdit()
       {
           EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(DatePeriodEditName,
               typeof(DatePeriodEdit), typeof(RepositoryItemDatePeriodEdit),
               typeof(DateEditViewInfo), new ButtonEditPainter(), true));
       }

       public override void Assign(RepositoryItem item)
       {
           base.Assign(item);
           var source = item as RepositoryItemDatePeriodEdit;
           if (source != null) _separatorChar = source.SeparatorChar;
       }

       protected override bool IsNullValue(object editValue)
       {
           var value = editValue as PeriodsSet;
           if (value != null)
               return value.Periods.Count == 0;
           var s = editValue as string;
           if (s == null) return false;

           return PeriodsSet.Parse(s).Periods.Count == 0;
       }

       public override string GetDisplayText(FormatInfo format, object editValue)
       {
           var displayText = string.Empty;
           var period = editValue as PeriodsSet;
           if (period != null)
               displayText = period.ToString(format.FormatString, SeparatorChar);
           else
           {
               var s = editValue as string;
               if (s != null)
                   displayText = PeriodsSet.Parse(s).ToString(format.FormatString, SeparatorChar);
           }
           var e = new CustomDisplayTextEventArgs(editValue, displayText);
           if (format != EditFormat)
               RaiseCustomDisplayText(e);
           return e.DisplayText;
       }
    }
    public class PopupDatePeriodEditForm : PopupDateEditForm
    {
        public PopupDatePeriodEditForm(DateEdit ownerEdit)
            : base(ownerEdit)
        {        
        }

        public override void ShowPopupForm()
        {
            base.ShowPopupForm();
            SetSelectedRange();
        }

        protected override void AssignCalendarProperties()
        {
            base.AssignCalendarProperties();
            Calendar.SelectionMode = CalendarSelectionMode.Multiple;
            Calendar.CaseWeekDayAbbreviations = TextCaseMode.SentenceCase;
            Calendar.ShowMonthHeaders = false;
            Calendar.SyncSelectionWithEditValue = false;
        }

        private void SetSelectedRange()
        {
            var value = OwnerEdit.EditValue as PeriodsSet;
            if (value != null && value.Periods.Count > 0)
            {
                Calendar.EditValue = ((Period)value.Periods[value.Periods.Count - 1]).End;               
                Calendar.SelectedRanges.BeginUpdate();
                Calendar.SelectedRanges.Clear();
                foreach (Period period in value.Periods)
                {
                    Calendar.SelectedRanges.Add(new DateRange(period.Begin, period.End));
                }
                Calendar.SelectedRanges.EndUpdate();
            }
            else
            {
                Calendar.EditValue = OwnerEdit.DateTime;
            }
        }

    }
    public class DatePeriodEdit : DateEdit
    {
         static DatePeriodEdit() { RepositoryItemDatePeriodEdit.RegisterDatePeriodEdit(); }

       public DatePeriodEdit()
       {
           EditValue = new PeriodsSet();
       }

       public override object EditValue
       {
           get
           {
               var value = base.EditValue as PeriodsSet;
               return value ?? new PeriodsSet();
           }
           set
           {
               if (value is PeriodsSet)
               {
                   base.EditValue = value;
                   return;
               }
               var s = value as string;
               if (s != null)
               {
                   base.EditValue = PeriodsSet.Parse(s);
               }
           }
       }

       public override string EditorTypeName
       {
           get { return RepositoryItemDatePeriodEdit.DatePeriodEditName; }          
       }

       [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
       public new RepositoryItemDatePeriodEdit Properties { get { return base.Properties as RepositoryItemDatePeriodEdit; } set { } }

       protected override void AcceptPopupValue(object val)
       {
           if (PopupForm != null)
           {
               var calendar = ((PopupDatePeriodEditForm)PopupForm).Calendar;
               var periods = new PeriodsSet();                
               foreach (var range in calendar.SelectedRanges)               
                       periods.Periods.Add(new Period(range.StartDate, range.EndDate.AddSeconds(-1)));
               if(val == null && calendar.SelectedRanges.Count == 0)
                    periods.Periods.Add(new Period(DateTime.MinValue));

               EditValue = periods;
               calendar.SelectedRanges.Clear();
           }
       }

       protected override PopupBaseForm CreatePopupForm() { return new PopupDatePeriodEditForm(this); }
       protected override object ExtractParsedValue(ConvertEditValueEventArgs e) { return e.Value; }
    }
    
}
