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
    [TestClass]
    public class FireAndForgetUnitTests
    {
        private const string messageType = "New Message Type";
        private IMessageBroker messageBroker;

        [TestInitialize]
        public void TestInit()
        {
            this.messageBroker = new MessageBroker(new InProcessChannelFactory(new LoggerFactory()));
        }

        [TestMethod, TestCategory("FireAndForget")]
        public void FireThenPeekTest()
        {
            // Arrange
            var payload = new Payload { x = 2, y = 3 };

            // Act
            this.messageBroker.Fire(new FireAndForgetMessage(FireAndForgetUnitTests.messageType, payload));

            // Assert
            var messages = this.messageBroker.Peek(FireAndForgetUnitTests.messageType);
            Assert.IsTrue(messages.Count() == 1);
            var message = messages.First();
            Assert.IsTrue(message.GetPayload<Payload>() == payload);
        }

        [TestMethod, TestCategory("FireAndForget")]
        public void FireThenPeekOnFireTest()
        {
            // Arrange
            var payload = new Payload { x = 2, y = 3 };
            this.messageBroker.OnFire += this.MessageBroker_OnFire;

            // Act
            this.messageBroker.Fire(new FireAndForgetMessage(FireAndForgetUnitTests.messageType, payload));

            // Assert
            var messages = this.messageBroker.Peek(FireAndForgetUnitTests.messageType);
            Assert.IsTrue(messages.Count() == 1);
            var message = messages.First();
            var messagePayload = message.GetPayload<object>();
            Assert.AreEqual(payload, messagePayload);
        }

        [TestMethod, TestCategory("FireAndForget")]
        public void FireShortLivingMessageTest()
        {
            // Arrange

            // Act
            this.messageBroker.Fire(new FireAndForgetMessage(FireAndForgetUnitTests.messageType) { TimeToLive = new TimeSpan(0, 0, 1) });
            Thread.Sleep(2000);

            // Assert
            var messages = messageBroker.Peek(FireAndForgetUnitTests.messageType);
            Assert.AreEqual(0, messages.Count());
        }

        [TestMethod, TestCategory("FireAndForget")]
        public void FireThenPeekThenKillMessageTest()
        {
            // Arrange

            // Act
            this.messageBroker.Fire(new FireAndForgetMessage(FireAndForgetUnitTests.messageType));
            var message = this.messageBroker.Peek(FireAndForgetUnitTests.messageType).First();
            message.TimeToLive = new TimeSpan(0, 0, 1);
            Thread.Sleep(2000);

            // Assert
            var messages = this.messageBroker.Peek(FireAndForgetUnitTests.messageType);
            Assert.AreEqual(0, messages.Count());
        }

        [TestMethod, ExpectedException(typeof(KeyNotFoundException)), TestCategory("FireAndForget")]
        public void FireThenPeekAnotherTypeMessageTest()
        {
            // Arrange
            const string nonExistingMessageType = "Non Existing Message Type";
            var payload = new Payload { x = 2, y = 3 };

            // Act
            this.messageBroker.Fire(new FireAndForgetMessage(FireAndForgetUnitTests.messageType, payload));

            // Assert
            var messages = this.messageBroker.Peek(nonExistingMessageType);
        }

        [TestMethod, ExpectedException(typeof(KeyNotFoundException)), TestCategory("FireAndForget")]
        public void PeekNonFiredMessageTest()
        {
            // Arrange

            // Act
            var messages = this.messageBroker.Peek(FireAndForgetUnitTests.messageType);

            // Assert
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