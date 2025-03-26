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

// Created by phuongdt

#endregion

#pragma warning disable 1591

using System.Collections;

namespace Inventec.Desktop.Core.Specifications
{
	public abstract class EnumerableSpecification : Specification
	{
		private readonly ISpecification _elementSpecification;

		protected EnumerableSpecification(ISpecification elementSpecification)
		{
			Platform.CheckForNullReference(elementSpecification, "elementSpecification");
			_elementSpecification = elementSpecification;
		}

		protected internal ISpecification ElementSpec
		{
			get { return _elementSpecification; }
		}

		protected static IEnumerable AsEnumerable(object obj)
		{
			var enumerable = obj as IEnumerable;
			if (enumerable == null)
				throw new SpecificationException(SR.ExceptionCastExpressionEnumerable);

			return enumerable;
		}
	}
}
