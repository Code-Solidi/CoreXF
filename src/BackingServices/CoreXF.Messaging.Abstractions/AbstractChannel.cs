using CoreXF.Messaging.Abstractions.Channels;
using CoreXF.Messaging.Abstractions.Messages;

using Microsoft.Extensions.Logging;

using System.Collections.Generic;

namespace CoreXF.Messaging.Abstractions
{
    public abstract class AbstractChannel //: IChannel
    {
        private readonly object locker = new object();

        //protected internal IDictionary<string, ICollection<AbstractMessage>> MessageQueue { get; set; }

        protected internal ILogger Logger { get; set; }

        protected AbstractChannel(AbstractChannelFactory factory, ILogger logger)
        {
            this.Logger = logger;
            //this.MessageQueue = factory.MessageQueue;
        }

        ///// <summary>
        ///// Peeks the specified message type.
        ///// </summary>
        ///// <param name="messageType">Type of the message.</param>
        ///// <returns></returns>
        //public virtual IEnumerable<AbstractMessage> Peek(string messageType)
        //{
        //    lock (this.locker)
        //    {
        //        return this.MessageQueue[messageType];
        //    }
        //}

        ///// <summary>
        ///// Gets the message types.
        ///// </summary>
        ///// <returns></returns>
        //public virtual IEnumerable<string> MessageTypes => this.MessageQueue.Keys;
    }
}