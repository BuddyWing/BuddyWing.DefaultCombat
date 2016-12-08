// Copyright (C) 2011-2016 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using System.Windows.Media;
using Buddy.Common;
using log4net;

namespace DefaultCombat
{
	public static class Logger
	{
	    private static readonly ILog _log = Log.Get();

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
            _log.Info($"[DefaultCombat] {string.Format(message, args)}");
		}
	}
}