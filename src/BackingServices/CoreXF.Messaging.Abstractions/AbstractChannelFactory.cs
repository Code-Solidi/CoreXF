/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Messaging.Abstractions.Channels;

using Microsoft.Extensions.Logging;

namespace CoreXF.Messaging.Abstractions
{
    public abstract class AbstractChannelFactory
    {
        private readonly ILogger logger;

        //private readonly object locker = new object();

        /// <summary>
        /// This is the queue of fire'n'forget messages.
        /// </summary>
        //public IDictionary<string, ICollection<AbstractMessage>> MessageQueue { get; private set; }

        public ILogger Logger => this.logger;

        //public IEnumerable<IMessage> GetAllMessages()
        //{
        //    var list = new List<IMessage>();
        //    foreach (var item in this.MessageQueue.Values)
        //    {
        //        list.AddRange(item);
        //    }

        //    return list;
        //}

        protected AbstractChannelFactory(ILogger logger)
        {
            this.logger = logger;
            //    this.MessageQueue = new Dictionary<string, ICollection<AbstractMessage>>();
        }

        public abstract IFireAndForgetChannel CreateFireAndForgetChannel(IMessageBroker broker);

        public abstract IPublishSubscribeChannel CreatePublishSubscribeChannel(IMessageBroker broker);

        public abstract IRequestResponseChannel CreateRequestResponseChannel(IMessageBroker broker);

        ///*protected */internal /*virtual */void AddMessage(AbstractMessage message)
        //{
        //    lock (this.locker)
        //    {
        //        // NB: this is registration in fact, done silently
        //        if (this.MessageQueue.ContainsKey(message.Type) == false)
        //        {
        //            this.MessageQueue[message.Type] = new List<AbstractMessage>();
        //        }

        //        this.MessageQueue[message.Type].Add(message);
        //    }
        //}

        //protected virtual void RemoveDeadMessages(object state)
        //{
        //    var lists = new Dictionary<string, List<AbstractMessage>>();
        //    lock (this.locker)
        //    {
        //        foreach (var item in this.MessageQueue)
        //        {
        //            var expired = item.Value.Where(m => DateTime.UtcNow - m.DateTime > ((IFireAndForgetMessage)m).TimeToLive);
        //            if (expired.Any())
        //            {
        //                lists[item.Key] = new List<AbstractMessage>(expired.Cast<AbstractMessage>());
        //            }
        //        }

        //        foreach (var list in lists)
        //        {
        //            var messageList = this.MessageQueue[list.Key];
        //            foreach (var expired in list.Value)
        //            {
        //                messageList.Remove(expired);
        //                this.Logger.LogInformation($"Message '{expired.Id}' expired ({expired.DateTime}, {((IFireAndForgetMessage)expired).TimeToLive})");
        //            }
        //        }
        //    }
        //}
    }
}