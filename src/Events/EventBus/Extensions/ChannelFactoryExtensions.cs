using EventBus.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventBus.Extensions
{
    public static class ChannelFactoryExtensions
    {
        public static IMessage Poll(this IChannelFactory factory, Guid messageId)
        {
            var messge = factory.GetAllMessages().SingleOrDefault(m => m.Id == messageId);
            return messge;
        }

        public static IEnumerable<IMessage> PollAll(this IChannelFactory factory, string messageType)
        {
            var messges = factory.GetAllMessages().Where(m => m.Type == messageType);
            return messges;
        }
    }
}