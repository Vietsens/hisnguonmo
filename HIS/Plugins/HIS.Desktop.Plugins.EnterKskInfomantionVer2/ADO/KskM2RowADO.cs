/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * vCong61141 - grid rows of the Mau 02 input tabs (vaccination, B1, B2).
 */
using System;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.ADO
{
    /// <summary>One vaccination row. Exactly one of Done / NotDone / Unknown may be checked.</summary>
    public class KskM2VaccineRowADO
    {
        public string Field { get; set; }
        public string Disease { get; set; }
        public string Schedule { get; set; }
        public string Dose { get; set; }
        public DateTime? InjectDate { get; set; }
        public bool Done { get; set; }
        public bool NotDone { get; set; }
        public bool Unknown { get; set; }
        /// <summary>Row "Cac loai khac" — disease / schedule / dose are editable.</summary>
        public bool IsOther { get; set; }

        public int Status
        {
            get
            {
                if (Done) return (int)KskM2VaccineStatus.Done;
                if (NotDone) return (int)KskM2VaccineStatus.NotDone;
                if (Unknown) return (int)KskM2VaccineStatus.Unknown;
                return (int)KskM2VaccineStatus.None;
            }
            set
            {
                Done = value == (int)KskM2VaccineStatus.Done;
                NotDone = value == (int)KskM2VaccineStatus.NotDone;
                Unknown = value == (int)KskM2VaccineStatus.Unknown;
            }
        }
    }

    /// <summary>One screening question row. C1..C4 are the answer columns, at most one checked.</summary>
    public class KskM2QuestionRowADO
    {
        public int No { get; set; }
        public string Field { get; set; }
        public string Text { get; set; }
        public bool C1 { get; set; }
        public bool C2 { get; set; }
        public bool C3 { get; set; }
        public bool C4 { get; set; }

        public int Level
        {
            get
            {
                if (C1) return 1;
                if (C2) return 2;
                if (C3) return 3;
                if (C4) return 4;
                return 0;
            }
            set
            {
                C1 = value == 1;
                C2 = value == 2;
                C3 = value == 3;
                C4 = value == 4;
            }
        }
    }

    /// <summary>One status of the portal tooth catalog shown in a tooth cell of Mau 02:
    /// the cell shows SYMBOL (legend of the form), the drop-down shows SYMBOL + NAME.</summary>
    public class KskM2ToothItemADO
    {
        public int ID { get; set; }
        public string NAME { get; set; }
        public string SYMBOL { get; set; }
    }
}
