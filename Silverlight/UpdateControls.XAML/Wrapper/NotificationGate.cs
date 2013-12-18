/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2010 Michael L Perry
 * MIT License
 * 
 * http://updatecontrols.net
 * http://updatecontrols.codeplex.com/
 * 
 **********************************************************************/

using System;

namespace UpdateControls.XAML.Wrapper
{
    class NotificationGate : IDisposable
    {
        [ThreadStatic]
        private static int _outboundCount;

        private NotificationGate()
        {
        }

        public static bool IsInbound
        {
            get { return _outboundCount == 0; }
        }

        public static IDisposable BeginOutbound()
        {
            ++_outboundCount;
            return new NotificationGate();
        }

        public void Dispose()
        {
            --_outboundCount;
        }
    }
}
