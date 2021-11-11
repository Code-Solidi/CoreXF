/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the GNU GENERAL PUBLIC LICENSE Version 2. See GNU-GPL.txt in the project root for license information.
 */

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