using System;

namespace Vefaqu.Host
{
    public class MessageContainer
    {
        public MessageContainer(Message message)
        {
            Content = message.Content;
            Created = BitConverter.GetBytes(DateTime.UtcNow.Ticks);

            var expireDate = message.Expires.GetValueOrDefault(DateTime.MaxValue);
            if (expireDate < DateTime.UtcNow)
                expireDate = DateTime.UtcNow;

            Expires = BitConverter.GetBytes(expireDate.Ticks);
            Length = BitConverter.GetBytes(Content.Length);
        }

        public byte[] Content { get; }
        public byte[] Created { get; }
        public byte[] Expires { get; }
        public byte[] Length { get; }
    }
}
