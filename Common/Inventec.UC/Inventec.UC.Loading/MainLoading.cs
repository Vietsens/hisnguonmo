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
using Inventec.UC.Loading.Init;
using Inventec.UC.Loading.Set.Delegate.SetDelegateBWDoWorker;
using Inventec.UC.Loading.Set.Delegate.SetDelegateBWRunWorkerCompleted;
using Inventec.UC.Loading.Set.SetReportProgressChanged;
using Inventec.UC.Loading.Start.StartBackgroundWorker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.Loading
{
    public partial class MainLoginLoading
    {
        public static string TEMPLATE1 = "Template1";


        public UserControl Init(string Template)
        {
            UserControl result = null;
            try
            {
                result = InitFactory.MakeIInit().InitUC(Template);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public bool SetDelegateDoWorker(UserControl UC, BWDoWorker DoWorker)
        {
            bool result = false;
            try
            {
                result = SetDelegateBWDoWorkerFactory.MakeISetDelegateBWDoWorker().SetDelegateDoWorker(UC, DoWorker);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public bool SetDelegateRunWorkerCompleted(UserControl UC, BWRunWorkerCompleted RunCompleted)
        {
            bool result = false;
            try
            {
                result = SetDelegateBWRunWorkerCompletedFactory.MakeISetDelegateBWRunWorkerCompleted().SetDelegateRunCompleted(UC, RunCompleted);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public void StartRunWorker(UserControl UC)
        {
            try
            {
                StartBackgroundWorkerFactory.MakeIStartBackgroundWorker().StartRunWorkerAsync(UC);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetReportProgress(UserControl UC, int i)
        {
            try
            {
                SetReportProgressChangedFactory.MakeISetReportProgressChanged().SetReportProgress(UC, i);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
