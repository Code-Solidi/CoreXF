/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the GNU GENERAL PUBLIC LICENSE Version 2. See GNU-GPL.txt in the project root for license information.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CoreXF.Messaging.Messages;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreXF.Messaging.Messages.Tests
{
    [TestClass, TestCategory("Messages")]
    public class AbstractMessageTests
    {
        private static readonly string messageType = "Test";

        [TestMethod]
        public void GetPayloadTest()
        {
            // Arrange
            var message = new FireAndForgetMessage(AbstractMessageTests.messageType, payload: "123");

            // Act
            var converted = message.GetPayload<int>();

            // Assert
            Assert.AreEqual(123, converted);
        }

        [TestMethod]
        public void GetPayloadAsyncTest()
        {
            // Arrange
            var message = new FireAndForgetMessage(AbstractMessageTests.messageType, payload: "123");

            // Act
            var converted = message.GetPayloadAsync<int>().Result;

            // Assert
            Assert.AreEqual(123, converted);
        }

        [TestMethod]
        public void GetDateTimeTest()
        {
            // Arrange
            var message = new FireAndForgetMessage(AbstractMessageTests.messageType);
            var current = DateTime.UtcNow;

            // Act
            var fired = message.DateTime;

            // Assert
            Assert.IsTrue(fired <= current);
        }
    }
}