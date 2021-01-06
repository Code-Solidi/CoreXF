using EventBus.Interfaces;
using System;
using System.Text;

namespace EventBus.Foundation.Messages
{
    public class FireForgetMessage : AbstractMessage, IFireForgetMessage
    {
        public static TimeSpan DefaultTimeToLive = new TimeSpan(7, 0, 0, 0);    // live for seven days

        public TimeSpan TimeToLive { get; set; }
    }
}