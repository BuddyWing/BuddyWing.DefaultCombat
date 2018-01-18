// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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