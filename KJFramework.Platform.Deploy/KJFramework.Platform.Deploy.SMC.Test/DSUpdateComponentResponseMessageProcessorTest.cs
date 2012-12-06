﻿using KJFramework.Platform.Deploy.SMC.CP.Connector.Processors;
using KJFramework.Platform.Deploy.SMC.CP.Connector.Processors.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using KJFramework.Platform.Deploy.SMC.ProtocolStack.Services;

namespace KJFramework.Platform.Deploy.SMC.Test
{
    /// <summary>
    ///This is a test class for DSUpdateComponentResponseMessageProcessorTest and is intended
    ///to contain all DSUpdateComponentResponseMessageProcessorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DSUpdateComponentResponseMessageProcessorTest
    {
        #region Members

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #endregion

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///A test for Process
        ///</summary>
        [TestMethod()]
        public void ProcessTest()
        {
            Regist();
            DSUpdateComponentResponseMessageProcessor target = new DSUpdateComponentResponseMessageProcessor(); // TODO: Initialize to an appropriate value
            Guid id = new Guid(); // TODO: Initialize to an appropriate value
            DynamicServiceUpdateComponentResponseMessage message = new DynamicServiceUpdateComponentResponseMessage();
            message.ErrorTrace = "这是一个错误信息";
            message.Result = false;
            message.ServiceName = "ServiceName";
            message.Bind();
            Assert.IsTrue(message.IsBind);
            DynamicServiceMessage actual;
            actual = target.Process(id, message);
            Assert.IsNull(actual);
        }

        /// <summary>
        ///     Regist a service for test.
        ///</summary>
        private void Regist()
        {
            DSRegistRequestMessageProcessor target = new DSRegistRequestMessageProcessor(); // TODO: Initialize to an appropriate value
            Guid id = new Guid(); // TODO: Initialize to an appropriate value
            DynamicServiceRegistRequestMessage message = new DynamicServiceRegistRequestMessage();
            message.ServiceName = "ServiceName";
            message.Name = "服务名称";
            message.SupportDomainPerformance = false;
            message.Version = "0.0.0.1";
            message.Description = "服务描述";
            DynamicServiceRegistResponseMessage actual;
            actual = (DynamicServiceRegistResponseMessage)target.Process(id, message);
            actual.Bind();
            Assert.IsTrue(actual.IsBind);
            Assert.IsTrue(actual.Result);
        }
    }
}
