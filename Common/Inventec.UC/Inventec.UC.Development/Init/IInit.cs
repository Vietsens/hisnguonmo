﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.Development.Init
{
    interface IInit
    {
        UserControl InitUC(MainDevelopment.EmumTemp enumT, string contentDonVi, string contentPhatTrien);
    }
}