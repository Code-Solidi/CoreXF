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

        [TestMethod]
        public void SubscribeToMessageTypeTest()
        {
            // Arrange
            var subscriber = new Subscriber(Guid.NewGuid().ToString());
            var messageBroker = new MessageBroker(new InProcessChannelFactory(new LoggerFactory()));
            messageBroker.Register(PublishSubscribeUnitTests.messageType);

            // Act
            messageBroker.Subscribe(subscriber, PublishSubscribeUnitTests.messageType);

            // Assert
            var isSubscribed = messageBroker.IsSubscribed(subscriber, PublishSubscribeUnitTests.messageType);
            Assert.IsTrue(isSubscribed);
        }

        [TestMethod]
        public void SubscribeMultipleToMessageTypeTest()
        {
            // Arrange
            var subscriber = new Subscriber(Guid.NewGuid().ToString());
            var subscriber2 = new Subscriber(Guid.NewGuid().ToString());
            var messageBroker = new MessageBroker(new InProcessChannelFactory(new LoggerFactory()));
            messageBroker.Register(PublishSubscribeUnitTests.messageType);

            // Act
            messageBroker.Subscribe(subscriber, PublishSubscribeUnitTests.messageType);
            messageBroker.Subscribe(subscriber2, PublishSubscribeUnitTests.messageType);

            // Assert
            Assert.IsTrue(messageBroker.IsSubscribed(subscriber, PublishSubscribeUnitTests.messageType));
            Assert.IsTrue(messageBroker.IsSubscribed(subscriber2, PublishSubscribeUnitTests.messageType));
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void SubscribeSameSubscriberMultipleToMessageTypeTest()
        {
            // Arrange
            var identity = Guid.NewGuid().ToString();
            var messageBroker = new MessageBroker(new InProcessChannelFactory(new LoggerFactory()));
            messageBroker.Register(PublishSubscribeUnitTests.messageType);
            var subscriber = new Subscriber(identity);

            // Act
            messageBroker.Subscribe(subscriber, PublishSubscribeUnitTests.messageType);
            messageBroker.Subscribe(subscriber, PublishSubscribeUnitTests.messageType);

            // Assert
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void SubscribeSameSubscriberIdentityMultipleToMessageTypeTest()
        {
            // Arrange
            var identity = Guid.NewGuid().ToString();
            var messageBroker = new MessageBroker(new InProcessChannelFactory(new LoggerFactory()));
            messageBroker.Register(PublishSubscribeUnitTests.messageType);

            // Act
            messageBroker.Subscribe(new Subscriber(identity), PublishSubscribeUnitTests.messageType);
            messageBroker.Subscribe(new Subscriber(identity), PublishSubscribeUnitTests.messageType);

            // Assert
        }


        [TestMethod]
        public void UnsubscribeFromMessageTypeTest()
        {
            // Arrange
            var subscriber = new Subscriber(Guid.NewGuid().ToString());
            var messageBroker = new MessageBroker(new InProcessChannelFactory(new LoggerFactory()));
            messageBroker.Register(PublishSubscribeUnitTests.messageType);
            messageBroker.Subscribe(subscriber, PublishSubscribeUnitTests.messageType);

            // Act
            messageBroker.Unsubscribe(subscriber.Identity, PublishSubscribeUnitTests.messageType);

            // Assert
            var isSubscribed = messageBroker.IsSubscribed(subscriber, PublishSubscribeUnitTests.messageType);
            Assert.IsFalse(isSubscribed);
        }

        [TestMethod]
        public void RegisterMessageTypeTest()
        {
            // Arrange
            var messageBroker = new MessageBroker(new InProcessChannelFactory(new LoggerFactory()));

            // Act
            messageBroker.Register(PublishSubscribeUnitTests.messageType);

            // Assert
            var isRegistered = messageBroker.IsRegistered(PublishSubscribeUnitTests.messageType);
            Assert.IsTrue(isRegistered);
        }

        [TestMethod]
        public void UnRegisterMessageTypeTest()
        {
            // Arrange
            var messageBroker = new MessageBroker(new InProcessChannelFactory(new LoggerFactory()));
            messageBroker.Register(PublishSubscribeUnitTests.messageType);

            // Act
            messageBroker.Unregister(PublishSubscribeUnitTests.messageType);

            // Assert
            var isRegistered = messageBroker.IsRegistered(PublishSubscribeUnitTests.messageType);
            Assert.IsFalse(isRegistered);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void UnRegisterNonRegisteredMessageTypeTest()
        {
            // Arrange
            var messageBroker = new MessageBroker(new InProcessChannelFactory(new LoggerFactory()));

            // Act
            messageBroker.Unregister(PublishSubscribeUnitTests.messageType);

            // Assert
            var isRegistered = messageBroker.IsRegistered(PublishSubscribeUnitTests.messageType);
            Assert.IsFalse(isRegistered);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void UnRegisterMessageWithSubscriptionsTest()
        {
            // Arrange
            var messageBroker = new MessageBroker(new InProcessChannelFactory(new LoggerFactory()));
            messageBroker.Register(PublishSubscribeUnitTests.messageType);
            messageBroker.Subscribe(new Subscriber(Guid.NewGuid().ToString()), PublishSubscribeUnitTests.messageType);

            // Act
            messageBroker.Unregister(PublishSubscribeUnitTests.messageType);

            // Assert
            var isRegistered = messageBroker.IsRegistered(PublishSubscribeUnitTests.messageType);
            Assert.IsFalse(isRegistered);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void SubscribeToUnregisteredMessageTypeTest()
        {
            // Arrange
            var subscriber = new Subscriber(Guid.NewGuid().ToString());
            var messageBroker = new MessageBroker(new InProcessChannelFactory(new LoggerFactory()));
            messageBroker.Register(PublishSubscribeUnitTests.messageType);

            // Act
            messageBroker.Unregister(PublishSubscribeUnitTests.messageType);
            messageBroker.Subscribe(subscriber, PublishSubscribeUnitTests.messageType);

            // Assert
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void SubscribeToNonRegisteredMessageTypeTest()
        {
            // Arrange
            var subscriber = new Subscriber(Guid.NewGuid().ToString());
            var messageBroker = new MessageBroker(new InProcessChannelFactory(new LoggerFactory()));

            // Act
            messageBroker.Subscribe(subscriber, PublishSubscribeUnitTests.messageType);

            // Assert
        }

        [TestMethod]
        public void PublishTest()
        {
            // Arrange
            var messageBroker = new MessageBroker(new InProcessChannelFactory(new LoggerFactory()));
            messageBroker.Register(PublishSubscribeUnitTests.messageType);

            messageBroker.Subscribe(new TestSubscriber(Guid.NewGuid().ToString()), PublishSubscribeUnitTests.messageType);

            // Act
            messageBroker.Publish(new PublishSubscribeMessage(PublishSubscribeUnitTests.messageType, new Payload { x = 123, y = 321 }));

            // Assert
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void PublishToManySubscribersTest()
        {
            // Arrange
            var messageBroker = new MessageBroker(new InProcessChannelFactory(new LoggerFactory()));
            messageBroker.Register(PublishSubscribeUnitTests.messageType);

            messageBroker.Subscribe(new TestSubscriber(Guid.NewGuid().ToString()), PublishSubscribeUnitTests.messageType);
            messageBroker.Subscribe(new TestSubscriber(Guid.NewGuid().ToString()), PublishSubscribeUnitTests.messageType);
            messageBroker.Subscribe(new TestSubscriber(Guid.NewGuid().ToString()), PublishSubscribeUnitTests.messageType);

            // Act
            messageBroker.Publish(new PublishSubscribeMessage(PublishSubscribeUnitTests.messageType, new Payload { x = 123, y = 321 }));

            // Assert
            Assert.IsTrue(true);
        }

        private class TestSubscriber : Subscriber
        {
            public TestSubscriber(string identity) : base(identity)
            {
            }

            public override IMessageResponse Recieve(IPublishSubscribeMessage message)
            {
                var payload = message.GetPayload<Payload>();
                Assert.AreEqual(123, payload.x);
                Assert.AreEqual(321, payload.y);
                return base.Recieve(message);
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