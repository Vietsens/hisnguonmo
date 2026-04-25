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

namespace EMR.Desktop.Plugins.EmrExamCategory
{
    class EmrRequestUriStore
    {
        // EMR_EXAM_CATEGORY
        internal const string EMR_EXAM_CATEGORY_GET = "api/EmrExamCategory/Get";
        internal const string EMR_EXAM_CATEGORY_CREATE = "api/EmrExamCategory/Create";
        internal const string EMR_EXAM_CATEGORY_UPDATE = "api/EmrExamCategory/Update";
        internal const string EMR_EXAM_CATEGORY_DELETE = "api/EmrExamCategory/Delete";
        internal const string EMR_EXAM_CATEGORY_SAVE_ALL = "api/EmrExamCategory/SaveAll";

        // EMR_DOCUMENT_PAIR_RULE
        internal const string EMR_DOCUMENT_PAIR_RULE_GET = "api/EmrDocumentPairRule/Get";
        internal const string EMR_DOCUMENT_PAIR_RULE_CREATE = "api/EmrDocumentPairRule/Create";
        internal const string EMR_DOCUMENT_PAIR_RULE_UPDATE = "api/EmrDocumentPairRule/Update";
        internal const string EMR_DOCUMENT_PAIR_RULE_DELETE = "api/EmrDocumentPairRule/Delete";
        internal const string EMR_DOCUMENT_PAIR_RULE_TEST = "api/EmrDocumentPairRule/TestRule";
    }
}
