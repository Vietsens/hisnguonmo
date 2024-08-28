#region License

// Created by phuongdt

#endregion

using System;

namespace Inventec.Desktop.Core
{
	internal class LocalTimeProvider : ITimeProvider
	{
		#region ITimeProvider Members

		public DateTime GetCurrentTime(DateTimeKind kind)
		{
			switch (kind)
			{
				case DateTimeKind.Utc:
					return DateTime.UtcNow;
				case DateTimeKind.Local:
					return DateTime.Now;
				default:
					throw new ArgumentOutOfRangeException("kind");
			}
		}

		#endregion
	}
}
