﻿using System;

namespace Apollo.Dache.SimplSockets
{
    /// <summary>
    /// Socket error args.
    /// </summary>
    public class SocketErrorArgs : EventArgs
    {
        /// <summary>
        /// Internal constructor.
        /// </summary>
        internal SocketErrorArgs() { }

        /// <summary>
        /// The exception.
        /// </summary>
        public Exception Exception { get; internal set; }
    }
}
