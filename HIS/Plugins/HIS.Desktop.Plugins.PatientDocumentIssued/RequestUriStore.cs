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
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.PatientDocumentIssued
{
    class RequestUriStore
    {
        //internal const string EMR_DOCUMENT_TYPE_CREATE = "api/EmrDocumentType/Create";
        //internal const string EMR_DOCUMENT_TYPE_DELETE = "api/EmrDocumentType/Delete";
        //internal const string EMR_DOCUMENT_TYPE_UPDATE = "api/EmrDocumentType/Update";
        //internal const string EMR_DOCUMENT_TYPE_CHANGE_LOCK = "api/EmrDocumentType/ChangeLock";
        internal const string EMR_DOCUMENT_TYPE_GET = "api/EmrDocumentType/Get";
        internal const string V_EMR_DOCUMENT_GET = "api/EmrDocument/GetView";
        
        internal const string EMR_DOCUMENT_ISSUED_UPDATE = "api/EmrDocument/Issued";
        internal const string EMR_DOCUMENT_DISISSUED_UPDATE = "api/EmrDocument/Disissued";
    }
}
