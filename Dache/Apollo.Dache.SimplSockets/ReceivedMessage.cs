using System.Net.Sockets;

namespace Apollo.Dache.SimplSockets
{
    /// <summary>
    /// A received message.
    /// </summary>
    public class ReceivedMessage
    {
        internal Socket Socket;
        internal int ThreadId;

        /// <summary>
        /// The message bytes.
        /// </summary>
        public byte[] Message;
    }
}
