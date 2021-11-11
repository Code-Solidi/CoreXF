/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the GNU GENERAL PUBLIC LICENSE Version 2. See GNU-GPL.txt in the project root for license information.
 */

using CoreXF.Messaging;
using CoreXF.Messaging.Abstractions;
using CoreXF.Messaging.Abstractions.Messages;
using CoreXF.Messaging.Channels.InProcess;
using CoreXF.Messaging.Messages;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CoreXF_Messaging.UnitTests
{
    [TestClass, TestCategory("PublishSubscribe")]
    public class PublishSubscribeUnitTests
    {
        private const string messageType = "New Message Type";
        private IMessageBroker messageBroker;

        [TestInitialize]
        public void TestInit()
        {
            this.messageBroker = new MessageBroker(new InProcessChannelFactory(new LoggerFactory()));
        }

        [TestMethod]
        public void SubscribeToMessageTypeTest()
        {
            // Arrange
            var subscriber = new Subscriber(Guid.NewGuid().ToString());
            this.messageBroker.Register(PublishSubscribeUnitTests.messageType);

            // Act
            this.messageBroker.Subscribe(subscriber, PublishSubscribeUnitTests.messageType);

            // Assert
            var isSubscribed = this.messageBroker.IsSubscribed(subscriber, PublishSubscribeUnitTests.messageType);
            Assert.IsTrue(isSubscribed);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void SubscribeNullSubscriberToMessageTypeTest()
        {
            // Arrange

            // Act
            this.messageBroker.Subscribe(null, PublishSubscribeUnitTests.messageType);

            // Assert
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void SubscribeInvalidMessageToMessageTypeTest()
        {
            // Arrange

            // Act
            this.messageBroker.Subscribe(new Subscriber(Guid.NewGuid().ToString()), string.Empty);

            // Assert
        }

        [TestMethod]
        public void SubscribeMultipleToMessageTypeTest()
        {
            // Arrange
            var subscriber = new Subscriber(Guid.NewGuid().ToString());
            var subscriber2 = new Subscriber(Guid.NewGuid().ToString());
            this.messageBroker.Register(PublishSubscribeUnitTests.messageType);

            // Act
            this.messageBroker.Subscribe(subscriber, PublishSubscribeUnitTests.messageType);
            this.messageBroker.Subscribe(subscriber2, PublishSubscribeUnitTests.messageType);

            // Assert
            Assert.IsTrue(this.messageBroker.IsSubscribed(subscriber, PublishSubscribeUnitTests.messageType));
            Assert.IsTrue(this.messageBroker.IsSubscribed(subscriber2, PublishSubscribeUnitTests.messageType));
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void SubscribeSameSubscriberMultipleToMessageTypeTest()
        {
            // Arrange
            var identity = Guid.NewGuid().ToString();
            this.messageBroker.Register(PublishSubscribeUnitTests.messageType);
            var subscriber = new Subscriber(identity);

            // Act
            this.messageBroker.Subscribe(subscriber, PublishSubscribeUnitTests.messageType);
            this.messageBroker.Subscribe(subscriber, PublishSubscribeUnitTests.messageType);

            // Assert
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void SubscribeSameSubscriberIdentityMultipleToMessageTypeTest()
        {
            // Arrange
            var identity = Guid.NewGuid().ToString();
            this.messageBroker.Register(PublishSubscribeUnitTests.messageType);

            // Act
            this.messageBroker.Subscribe(new Subscriber(identity), PublishSubscribeUnitTests.messageType);
            this.messageBroker.Subscribe(new Subscriber(identity), PublishSubscribeUnitTests.messageType);

            // Assert
        }

        [TestMethod]
        public void IsSubscribedToMessageTypeTest()
        {
            // Arrange
            var subscriber = new Subscriber(Guid.NewGuid().ToString());
            this.messageBroker.Register(PublishSubscribeUnitTests.messageType);
            this.messageBroker.Subscribe(subscriber, PublishSubscribeUnitTests.messageType);

            // Act
            var isSubscribed = this.messageBroker.IsSubscribed(subscriber, PublishSubscribeUnitTests.messageType);

            // Assert
            Assert.IsTrue(isSubscribed);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void IsSubscribedToNonRegisteredMessageTypeTest()
        {
            // Arrange
            var subscriber = new Subscriber(Guid.NewGuid().ToString());

            // Act
            this.messageBroker.IsSubscribed(subscriber, PublishSubscribeUnitTests.messageType);

            // Assert
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void IsSubscribedToInvalidMessageTypeTest()
        {
            // Arrange
            var subscriber = new Subscriber(Guid.NewGuid().ToString());

            // Act
            this.messageBroker.IsSubscribed(subscriber, string.Empty);

            // Assert
        }

        [TestMethod]
        public void UnsubscribeFromMessageTypeTest()
        {
            // Arrange
            var subscriber = new Subscriber(Guid.NewGuid().ToString());
            this.messageBroker.Register(PublishSubscribeUnitTests.messageType);
            messageBroker.Subscribe(subscriber, PublishSubscribeUnitTests.messageType);

            // Act
            this.messageBroker.Unsubscribe(subscriber.Identity, PublishSubscribeUnitTests.messageType);

            // Assert
            var isSubscribed = this.messageBroker.IsSubscribed(subscriber, PublishSubscribeUnitTests.messageType);
            Assert.IsFalse(isSubscribed);
        }

        [TestMethod]
        public void RegisterMessageTypeTest()
        {
            // Arrange

            // Act
            this.messageBroker.Register(PublishSubscribeUnitTests.messageType);

            // Assert
            var isRegistered = this.messageBroker.IsRegistered(PublishSubscribeUnitTests.messageType);
            Assert.IsTrue(isRegistered);
        }

        [TestMethod]
        public void UnRegisterMessageTypeTest()
        {
            // Arrange
            this.messageBroker.Register(PublishSubscribeUnitTests.messageType);

            // Act
            this.messageBroker.Unregister(PublishSubscribeUnitTests.messageType);

            // Assert
            var isRegistered = this.messageBroker.IsRegistered(PublishSubscribeUnitTests.messageType);
            Assert.IsFalse(isRegistered);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void UnRegisterNonRegisteredMessageTypeTest()
        {
            // Arrange

            // Act
            this.messageBroker.Unregister(PublishSubscribeUnitTests.messageType);

            // Assert
            var isRegistered = this.messageBroker.IsRegistered(PublishSubscribeUnitTests.messageType);
            Assert.IsFalse(isRegistered);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void UnRegisterMessageWithSubscriptionsTest()
        {
            // Arrange
            this.messageBroker.Register(PublishSubscribeUnitTests.messageType);
            this.messageBroker.Subscribe(new Subscriber(Guid.NewGuid().ToString()), PublishSubscribeUnitTests.messageType);

            // Act
            this.messageBroker.Unregister(PublishSubscribeUnitTests.messageType);

            // Assert
            var isRegistered = this.messageBroker.IsRegistered(PublishSubscribeUnitTests.messageType);
            Assert.IsFalse(isRegistered);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void SubscribeToUnregisteredMessageTypeTest()
        {
            // Arrange
            var subscriber = new Subscriber(Guid.NewGuid().ToString());
            this.messageBroker.Register(PublishSubscribeUnitTests.messageType);

            // Act
            this.messageBroker.Unregister(PublishSubscribeUnitTests.messageType);
            this.messageBroker.Subscribe(subscriber, PublishSubscribeUnitTests.messageType);

            // Assert
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void SubscribeToNonRegisteredMessageTypeTest()
        {
            // Arrange
            var subscriber = new Subscriber(Guid.NewGuid().ToString());

            // Act
            this.messageBroker.Subscribe(subscriber, PublishSubscribeUnitTests.messageType);

            // Assert
        }

        [TestMethod]
        public void PublishTest()
        {
            // Arrange
            this.messageBroker.Register(PublishSubscribeUnitTests.messageType);
            this.messageBroker.Subscribe(new TestSubscriber(Guid.NewGuid().ToString()), PublishSubscribeUnitTests.messageType);

            // Act
            this.messageBroker.Publish(new PublishedMessage(PublishSubscribeUnitTests.messageType, new Payload { x = 123, y = 321 }));

            // Assert
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void PublishToManySubscribersTest()
        {
            // Arrange
            this.messageBroker.Register(PublishSubscribeUnitTests.messageType);
            this.messageBroker.Subscribe(new TestSubscriber(Guid.NewGuid().ToString()), PublishSubscribeUnitTests.messageType);
            this.messageBroker.Subscribe(new TestSubscriber(Guid.NewGuid().ToString()), PublishSubscribeUnitTests.messageType);
            this.messageBroker.Subscribe(new TestSubscriber(Guid.NewGuid().ToString()), PublishSubscribeUnitTests.messageType);

            // Act
            this.messageBroker.Publish(new PublishedMessage(PublishSubscribeUnitTests.messageType, new Payload { x = 123, y = 321 }));

            // Assert
            Assert.IsTrue(true);
        }

        private class TestSubscriber : Subscriber
        {
            public TestSubscriber(string identity) : base(identity)
            {
            }

            public override void Recieve(IPublishedMessage message)
            {
                var payload = message.GetPayload<Payload>();
                Assert.AreEqual(123, payload.x);
                Assert.AreEqual(321, payload.y);
            }
        }

        private void MessageBroker_OnFire(IFireAndForgetMessage message)
        {
            var payload = message.GetPayload<Payload>();
            Assert.AreEqual(2, payload.x);
            Assert.AreEqual(3, payload.y);
        }

        private struct Payload
        {
            internal int x;
            internal int y;

            public static bool operator ==(Payload p1, Payload p2)
            {
                return p1.Equals(p2);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public static bool operator !=(Payload p1, Payload p2)
            {
                return !p1.Equals(p2);
            }

            public override bool Equals(object obj)
            {
                return this.x == ((Payload)obj).x && this.y == ((Payload)obj).y;
            }
        }
    }
}