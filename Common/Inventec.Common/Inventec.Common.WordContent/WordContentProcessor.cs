using System;
using System.Collections.Generic;
using System.Text;

namespace Inventec.Common.WordContent
{
    public class WordContentProcessor
    {
        public WordContentProcessor() { }

        public void ShowForm(WordContentADO wordContentADO)
        {
            try
            {
                frmMain frmMain = new frmMain(wordContentADO);
                frmMain.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }            
        }
    }
}
