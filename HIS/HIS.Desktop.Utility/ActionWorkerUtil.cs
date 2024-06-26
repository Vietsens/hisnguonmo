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
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Utility
{
    public class ActionWorkerUtil
    {
        public static void Execute(Action action)
        {
            Stopwatch overallStopWatch = new Stopwatch();
            System.ComponentModel.BackgroundWorker backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.ProgressChanged += delegate
            {
                double totalSeconds = overallStopWatch.Elapsed.TotalSeconds;
                if (totalSeconds >= 2)
                {
                    WaitingManager.Show();
                }
            };
            backgroundWorker1.DoWork += delegate
            {
                overallStopWatch.Start();
                if (action != null)
                    action();
            };
            backgroundWorker1.RunWorkerCompleted += delegate
            {
                WaitingManager.Hide();
                //MessageBox.Show("Completed");
            };
            backgroundWorker1.RunWorkerAsync();
        }

    }
}
