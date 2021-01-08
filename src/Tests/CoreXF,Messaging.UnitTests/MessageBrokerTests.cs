using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

namespace CoreXF.Messaging.Tests
{
    [TestClass, TestCategory("MessgeBroker")]
    public class MessageBrokerTests
    {
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void MessageBrokerNullChannelFactoryTest()
        {
            // Arrange

            // Act
            new MessageBroker(null);

            // Assert
        }
    }
}