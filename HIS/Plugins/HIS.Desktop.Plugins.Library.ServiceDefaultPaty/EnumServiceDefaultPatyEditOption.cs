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

namespace HIS.Desktop.Plugins.Library.ServiceDefaultPaty
{
    /// <summary>
    /// PT-44730. Who is allowed to edit the patient type of a service request whose service
    /// is declared in HIS_SERVICE_DEFAULT_PATY.
    /// Mapped from config key HIS.Desktop.Plugins.Assign.ServiceDefaultPatyEditOption.
    /// </summary>
    public enum EnumServiceDefaultPatyEditOption
    {
        /// <summary>Only accounts flagged as administrator. Default value of the config.</summary>
        OnlyAdmin = 1,

        /// <summary>The account written as requester on the service request, or an administrator.</summary>
        RequestUserOrAdmin = 2
    }
}
