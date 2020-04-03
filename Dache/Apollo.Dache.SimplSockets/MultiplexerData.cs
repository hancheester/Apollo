using System.Threading;

namespace Apollo.Dache.SimplSockets
{
    /// <summary>
    /// Contains multiplexer data.
    /// </summary>
    internal class MultiplexerData
    {
        public byte[] Message { get; set; }
        public ManualResetEvent ManualResetEvent { get; set; }
    }
}
