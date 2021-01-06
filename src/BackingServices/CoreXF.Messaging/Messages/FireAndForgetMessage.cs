using CoreXF.Messaging.Abstractions.Messages;

using System;

namespace CoreXF.Messaging.Messages
{
    public class FireAndForgetMessage : AbstractMessage, IFireAndForgetMessage
    {
        public static TimeSpan DefaultTimeToLive = new TimeSpan(7, 0, 0, 0);    // live for seven days

        public TimeSpan TimeToLive { get; set; }

        public FireAndForgetMessage(string messageType) : base(messageType)
        {
        }

        public FireAndForgetMessage(string messageType, object payload) : base(messageType, payload)
        {
        }
    }
}