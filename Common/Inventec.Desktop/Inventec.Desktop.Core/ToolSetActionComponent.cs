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
#region License

// Author: Phuongdt

#endregion

using Inventec.Desktop.Core.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Desktop.Core
{
    public partial class ToolSetActionComponent
    {
        public ToolSetActionComponent(ExtensionInfo extensionInfo)
            : this(new ViewerSetupHelper(), extensionInfo) { }

        public ToolSetActionComponent(IViewerSetupHelper setupHelper, ExtensionInfo extensionInfo)
        {
            Platform.CheckForNullReference(setupHelper, "setupHelper");
            Platform.CheckForNullReference(extensionInfo, "extensionInfo");
            _setupHelper = setupHelper;
            _extensionInfo = extensionInfo;
            _toolSet = new ToolSet(CreateTools(extensionInfo), CreateToolContext());
        }

        private ToolSet _toolSet;
        private readonly IViewerSetupHelper _setupHelper;
        private ExtensionInfo _extensionInfo;

        /// <summary>
        /// Gets the <see cref="ToolSet"/>.
        /// </summary>
        public ToolSet ToolSet
        {
            get { return _toolSet; }
        }

        /// <summary>
        /// Creates a set of tools for this image viewer to load into its tool set.  Subclasses can override
        /// this to provide their own tools or cull the set of tools this creates.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable CreateTools(ExtensionInfo extensionInfo)
        {
            return _setupHelper.GetTools(extensionInfo);
        }

        /// <summary>
        /// Creates an <see cref="IDesktopToolContext"/> to provide to all the tools owned by this image viewer.
        /// </summary>
        /// <remarks>
        /// Subclasses can override this to provide their own custom implementation of an <see cref="IDesktopToolContext"/>.
        /// </remarks>
        protected virtual DesktopToolContext CreateToolContext()
        {
            return new DesktopToolContext();
        }
    }
}
