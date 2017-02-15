using System;

namespace Vefaqu.Host
{
    public class Message
    {
        public Message(byte[] content, DateTime? expires = null)
        {
            Content = content;
            Expires = expires;

        }

        public byte[] Content { get; }
        public DateTime? Expires { get; }
    }
}
