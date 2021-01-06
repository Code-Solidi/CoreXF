using System;

namespace EventBus.Interfaces
{
    public interface IFireForgetMessage : IMessage
    {
        TimeSpan TimeToLive { get; set; }
    }
}