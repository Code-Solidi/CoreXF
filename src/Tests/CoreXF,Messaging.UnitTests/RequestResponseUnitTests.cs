using CoreXF.Messaging;
using CoreXF.Messaging.Abstractions;
using CoreXF.Messaging.Abstractions.Messages;
using CoreXF.Messaging.Channels.InProcess;
using CoreXF.Messaging.Messages;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

namespace CoreXF_Messaging.UnitTests
{
    [TestClass]
    public class InProcessChannelFactoryUnitTests
    {
        private const string messageType = "New Message Type";

        [TestMethod, TestCategory("RequestResponse")]
        public void AddRecipientTest()
        {
            // Arrange
            var messageBroker = new MessageBroker(new InProcessChannelFactory(new LoggerFactory()));

            // Act
            messageBroker.AddRecipient(InProcessChannelFactoryUnitTests.messageType, new Recipient(Guid.NewGuid().ToString()));

            // Assert
            Assert.IsTrue(true);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException)), TestCategory("RequestResponse")]
        public void AddRecipientMiltipleTest()
        {
            // Arrange
            var messageBroker = new MessageBroker(new InProcessChannelFactory(new LoggerFactory()));

            // Act
            messageBroker.AddRecipient(InProcessChannelFactoryUnitTests.messageType, new Recipient(Guid.NewGuid().ToString()));
            messageBroker.AddRecipient(InProcessChannelFactoryUnitTests.messageType, new Recipient(Guid.NewGuid().ToString()));

            // Assert
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException)), TestCategory("RequestResponse")]
        public void AddRecipientSameIdentityMiltipleTest()
        {
            // Arrange
            var messageBroker = new MessageBroker(new InProcessChannelFactory(new LoggerFactory()));
            var identity = Guid.NewGuid().ToString();

            // Act
            messageBroker.AddRecipient(InProcessChannelFactoryUnitTests.messageType, new Recipient(Guid.NewGuid().ToString()));
            messageBroker.AddRecipient(InProcessChannelFactoryUnitTests.messageType, new Recipient(Guid.NewGuid().ToString()));

            // Assert
        }

        [TestMethod, TestCategory("RequestResponse")]
        public void RemoveRecipientTest()
        {
            // Arrange
            var messageBroker = new MessageBroker(new InProcessChannelFactory(new LoggerFactory()));
            var identity = Guid.NewGuid().ToString();
            messageBroker.AddRecipient(InProcessChannelFactoryUnitTests.messageType, new Recipient(identity));

            // Act
            messageBroker.RemoveRecipient(InProcessChannelFactoryUnitTests.messageType);

            // Assert
            Assert.IsFalse(messageBroker.FindRecipient(InProcessChannelFactoryUnitTests.messageType));
        }

        [TestMethod, TestCategory("RequestResponse"), ExpectedException(typeof(InvalidOperationException))]
        public void RemoveNotAddedRecipientTest()
        {
            // Arrange
            var messageBroker = new MessageBroker(new InProcessChannelFactory(new LoggerFactory()));
            var identity = Guid.NewGuid().ToString();

            // Act
            messageBroker.RemoveRecipient(identity);

            // Assert
        }

        [TestMethod, TestCategory("RequestResponse")]
        public void FindRecipientTest()
        {
            // Arrange
            var messageBroker = new MessageBroker(new InProcessChannelFactory(new LoggerFactory()));
            var identity = Guid.NewGuid().ToString();
            messageBroker.AddRecipient(InProcessChannelFactoryUnitTests.messageType, new Recipient(identity));

            // Act
            var found = messageBroker.FindRecipient(InProcessChannelFactoryUnitTests.messageType);

            // Assert
            Assert.IsTrue(found);
        }

        [TestMethod, TestCategory("RequestResponse")]
        public void RequestResponseTest()
        {
            // Arrange
            var payload = new Payload { x = 2, y = 3 };
            var identity = Guid.NewGuid().ToString();
            var messageBroker = new MessageBroker(new InProcessChannelFactory(new LoggerFactory()));
            messageBroker.AddRecipient(InProcessChannelFactoryUnitTests.messageType, new TestRecipient(identity));

            // Act
            var response = messageBroker.Request(new RequestResponseMessage(InProcessChannelFactoryUnitTests.messageType, payload));

            // Assert
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Content);
            var content = (Payload)response.Content;
            Assert.AreEqual(payload.x * 10, content.x);
            Assert.AreEqual(payload.y * 100, content.y);
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

        public class TestRecipient : Recipient
        {
            public TestRecipient(string identity) : base(identity)
            {
            }

            public override IMessageResponse Recieve(IRequestResponseMessage message)
            {
                var response = base.Recieve(message);
                var content = message.GetPayload<Payload>();
                content.x *= 10;
                content.y *= 100;
                response.SetContent(content);
                return response;
            }
        }
    }
}