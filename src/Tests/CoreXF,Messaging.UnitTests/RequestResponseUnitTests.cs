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
        private IMessageBroker messageBroker;

        [TestInitialize]
        public void TestInit()
        {
            this.messageBroker = new MessageBroker(new InProcessChannelFactory(new LoggerFactory()));
        }

        [TestMethod, TestCategory("RequestResponse")]
        public void AddRecipientTest()
        {
            // Arrange

            // Act
            this.messageBroker.AddRecipient(InProcessChannelFactoryUnitTests.messageType, new Recipient(Guid.NewGuid().ToString()));

            // Assert
            Assert.IsTrue(true);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException)), TestCategory("RequestResponse")]
        public void AddInvalidRecipientTest()
        {
            // Arrange

            // Act
            this.messageBroker.AddRecipient(InProcessChannelFactoryUnitTests.messageType, null);

            // Assert
            Assert.IsTrue(true);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException)), TestCategory("RequestResponse")]
        public void AddInvalidMessageTypeTest()
        {
            // Arrange

            // Act
            this.messageBroker.AddRecipient(null, new Recipient(Guid.NewGuid().ToString()));

            // Assert
            Assert.IsTrue(true);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException)), TestCategory("RequestResponse")]
        public void AddRecipientMiltipleTest()
        {
            // Arrange

            // Act
            this.messageBroker.AddRecipient(InProcessChannelFactoryUnitTests.messageType, new Recipient(Guid.NewGuid().ToString()));
            this.messageBroker.AddRecipient(InProcessChannelFactoryUnitTests.messageType, new Recipient(Guid.NewGuid().ToString()));

            // Assert
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException)), TestCategory("RequestResponse")]
        public void AddRecipientSameIdentityMiltipleTest()
        {
            // Arrange
            var identity = Guid.NewGuid().ToString();

            // Act
            this.messageBroker.AddRecipient(InProcessChannelFactoryUnitTests.messageType, new Recipient(Guid.NewGuid().ToString()));
            this.messageBroker.AddRecipient(InProcessChannelFactoryUnitTests.messageType, new Recipient(Guid.NewGuid().ToString()));

            // Assert
        }

        [TestMethod, TestCategory("RequestResponse")]
        public void RemoveRecipientTest()
        {
            // Arrange
            var identity = Guid.NewGuid().ToString();
            this.messageBroker.AddRecipient(InProcessChannelFactoryUnitTests.messageType, new Recipient(identity));

            // Act
            this.messageBroker.RemoveRecipient(InProcessChannelFactoryUnitTests.messageType);

            // Assert
            Assert.IsFalse(this.messageBroker.FindRecipient(InProcessChannelFactoryUnitTests.messageType));
        }

        [TestMethod, ExpectedException(typeof(ArgumentException)), TestCategory("RequestResponse")]
        public void RemoveInvalidRecipientTest()
        {
            // Arrange

            // Act
            this.messageBroker.RemoveRecipient(null);

            // Assert
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException)), TestCategory("RequestResponse")]
        public void RemoveNotAddedRecipientTest()
        {
            // Arrange
            var identity = Guid.NewGuid().ToString();

            // Act
            this.messageBroker.RemoveRecipient(identity);

            // Assert
        }

        [TestMethod, TestCategory("RequestResponse")]
        public void FindRecipientTest()
        {
            // Arrange
            var identity = Guid.NewGuid().ToString();
            this.messageBroker.AddRecipient(InProcessChannelFactoryUnitTests.messageType, new Recipient(identity));

            // Act
            var found = this.messageBroker.FindRecipient(InProcessChannelFactoryUnitTests.messageType);

            // Assert
            Assert.IsTrue(found);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException)), TestCategory("RequestResponse")]
        public void FindNullRecipientTest()
        {
            // Arrange

            // Act
            var found = this.messageBroker.FindRecipient(null);

            // Assert
        }

        [TestMethod, TestCategory("RequestResponse")]
        public void RequestResponseTest()
        {
            // Arrange
            var payload = new Payload { x = 2, y = 3 };
            var identity = Guid.NewGuid().ToString();
            var recipient = new TestRecipient(identity);
            this.messageBroker.AddRecipient(InProcessChannelFactoryUnitTests.messageType, recipient);

            // Act
            var response = this.messageBroker.Request(new RequestMessage(InProcessChannelFactoryUnitTests.messageType, payload));

            // Assert
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Content);
            Assert.AreEqual(StatusCode.Success, response.StatusCode);
            Assert.AreEqual(recipient, response.Recipient);
            Assert.IsNull(response.ReasonPhrase);

            var content = (Payload)response.Content;
            Assert.AreEqual(payload.x * 10, content.x);
            Assert.AreEqual(payload.y * 100, content.y);
        }

        [TestMethod, TestCategory("RequestResponse")]
        public void RequestResponseOnResponseTest()
        {
            // Arrange
            var payload = new Payload { x = 2, y = 3 };
            var identity = Guid.NewGuid().ToString();
            this.messageBroker.AddRecipient(InProcessChannelFactoryUnitTests.messageType, new TestRecipient(identity));
            this.messageBroker.OnResponse += this.MessageBroker_OnResponse;

            // Act
            var response = this.messageBroker.Request(new RequestMessage(InProcessChannelFactoryUnitTests.messageType, payload));

            // Assert
            //Assert.IsNotNull(response);
            //Assert.IsNotNull(response.Content);
            //var content = (Payload)response.Content;
            //Assert.AreEqual(payload.x * 10, content.x);
            //Assert.AreEqual(payload.y * 100, content.y);
        }

        private void MessageBroker_OnResponse(IRequestMessage message, IMessageResponse response)
        {
            var payload = message.GetPayload<Payload>();
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Content);
            var content = (Payload)response.Content;
            Assert.AreEqual(payload.x * 10, content.x);
            Assert.AreEqual(payload.y * 100, content.y);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException)), TestCategory("RequestResponse")]
        public void RequestNonRegisteredMessageTypeResponseTest()
        {
            // Arrange
            var payload = new Payload { x = 2, y = 3 };

            // Act
            this.messageBroker.Request(new RequestMessage(InProcessChannelFactoryUnitTests.messageType, payload));

            // Assert
        }

        [TestMethod, ExpectedException(typeof(ArgumentException)), TestCategory("RequestResponse")]
        public void RequestInvalidMessageTypeResponseTest()
        {
            // Arrange
            var payload = new Payload { x = 2, y = 3 };
            var identity = Guid.NewGuid().ToString();

            // Act
            this.messageBroker.Request(new RequestMessage(null, payload));

            // Assert
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException)), TestCategory("RequestResponse")]
        public void RequestInvalidMessageResponseTest()
        {
            // Arrange

            // Act
            this.messageBroker.Request(null);

            // Assert
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

            public override IMessageResponse Recieve(IRequestMessage message)
            {
                var response = base.Recieve(message);
                var content = message.GetPayload<Payload>();
                content.x *= 10;
                content.y *= 100;
                (response as MessageResponse).SetContent(content);
                (response as MessageResponse).SetRecipient(this);
                return response;
            }
        }
    }
}