// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using System.Windows.Media;
using Buddy.Common;

namespace DefaultCombat
{
	public static class Logger
	{
		public static void Write(string message)
		{
			Write(Colors.Green, message);
		}

		public static void Write(string message, params object[] args)
		{
			Write(Colors.Green, message, args);
		}

		public static void Write(Color clr, string message, params object[] args)
		{
			Logging.Write(clr, "[DefaultCombat] " + message, args);
		}
	}
}