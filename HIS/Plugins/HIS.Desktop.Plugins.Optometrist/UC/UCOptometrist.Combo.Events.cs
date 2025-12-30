using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Utility;
using System;

namespace HIS.Desktop.Plugins.Optometrist.UC
{
    public partial class UCOptometrist : UserControlBase
    {
        private void VISION_TEST_TIME_Closed(object sender, ClosedEventArgs e)
        {
            ApplyCurrentTimeIfNeeded(sender, e);
        }

        private void GLASS_USE_TIME_Closed(object sender, ClosedEventArgs e)
        {
            ApplyCurrentTimeIfNeeded(sender, e);
        }

        private void MEDI_USE_TIME_Closed(object sender, ClosedEventArgs e)
        {
            ApplyCurrentTimeIfNeeded(sender, e);
        }

        private void ApplyCurrentTimeIfNeeded(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode != PopupCloseMode.Normal)
                {
                    return;
                }
                var editor = sender as DateEdit;
                if (editor == null)
                {
                    return;
                }
                var selectedSereServ = GetSelectedSereServ();
                if (selectedSereServ != null && selectedSereServ.VISION_TEST_TIME.HasValue)
                {
                    return;
                }
                if (editor?.EditValue == null)
                {
                    return;
                }
                var selectedDateTime = editor.DateTime;
                if (selectedDateTime.TimeOfDay == TimeSpan.Zero)
                {
                    editor.DateTime = selectedDateTime.Date.Add(DateTime.Now.TimeOfDay);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}