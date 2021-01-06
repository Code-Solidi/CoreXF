
using System;

namespace CoreXF.Messaging.Abstractions.Messages
{
    public interface IFireAndForgetMessage : IMessage
    {
        TimeSpan TimeToLive { get; set; }
    }
}