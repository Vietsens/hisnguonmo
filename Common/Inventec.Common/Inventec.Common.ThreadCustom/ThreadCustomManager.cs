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
using System.Threading;
using System.Threading.Tasks;

namespace Inventec.Common.ThreadCustom
{
    public class ThreadCustomManager
    {
        public delegate void ResultCallback();
        public static void MultipleThreadWithJoin(List<Action> listFunction)
        {
            //Start All
            List<Thread> listThread = new List<Thread>();
            foreach (var item in listFunction)
            {
                Thread thread = new Thread(item.Invoke);
                thread.Start();
                listThread.Add(thread);
            }

            foreach (var item in listThread)
            {
                item.Join();
            }
        }

        public static void MultipleThreadWithJoin2(List<Action> listFunction)
        {
            List<Thread> listThread = new List<Thread>();
            List<Thread> listThreadStop = new List<Thread>();
            foreach (var item in listFunction)
            {
                Thread thread = new Thread(item.Invoke);
                thread.Start();
                listThread.Add(thread);
            }
            while (true)
            {
                foreach (var item in listThread)
                {
                    Thread thread = listThreadStop.FirstOrDefault(o => o.ManagedThreadId == item.ManagedThreadId);
                    if (item.ThreadState == ThreadState.Stopped && thread == null)
                        listThreadStop.Add(item);
                }

                if (listThreadStop.Count == listThread.Count)
                    return;
            }
        }


        ResultCallback ResultCallBack;
        public static void ThreadResultCallBack(Action function, ResultCallback resultCallBack)
        {
            Thread thread = new Thread(
                () =>
                {
                    try
                    {
                        function.Invoke();
                    }
                    finally
                    {
                        resultCallBack();
                    }
                });

            thread.Start();
        }
    }
}
