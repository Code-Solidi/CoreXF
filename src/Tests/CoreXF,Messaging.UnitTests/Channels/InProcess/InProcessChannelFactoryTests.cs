/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the GNU GENERAL PUBLIC LICENSE Version 2. See GNU-GPL.txt in the project root for license information.
 */

using CoreXF.Messaging.Messages;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;

namespace CoreXF.Messaging.Channels.InProcess.Tests
{
    [TestClass, TestCategory("InProcessChannelFactory")]
    public class InProcessChannelFactoryTests
    {
        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void CreateWithNegativePeriodTest()
        {
            // Arrange

            // Act
            var factory = new InProcessChannelFactory(new LoggerFactory(), -1);

            // Assert
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void CreateWithZeroPeriodTest()
        {
            // Arrange

            // Act
            var factory = new InProcessChannelFactory(new LoggerFactory(), 0);

            // Assert
        }

        //[TestMethod]
        //public void GetAllMessagesTest()
        //{
        //    // Arrange
        //    var factory = new InProcessChannelFactory(new LoggerFactory());
        //    var messageBroker = new MessageBroker(factory);

        //    // Act
        //    messageBroker.Fire(new FireAndForgetMessage("Test"));
        //    var count = factory.GetAllMessages().Count();

        //    // Assert
        //    Assert.AreEqual(1, count);
        //}
    }
}