using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using App.Controllers;
using System.Web.Http;
using Models;
using System.Web.Http.Results;
using System.Linq;

namespace Test.Controllers
{
    /// <summary>
    /// Summary description for CustomerTest
    /// </summary>
    [TestClass]
    public class CustomerTest
    {
        public CustomerTest()
        {
            //
            // TODO: Add constructor logic here
            //
            CustomerControllerInstance = new ApiCustomerController();
        }

        private ApiCustomerController CustomerControllerInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public ApiCustomerController Controller
        {
            get
            {
                return CustomerControllerInstance;
            }
            set
            {
                CustomerControllerInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestGet()
        {
            //
            // TODO: Add test logic here
            //

            IHttpActionResult actionResult = Controller.Get(1).Result;
            var contentResult = actionResult as OkNegotiatedContentResult<Customer>;

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(1, contentResult.Content.Id);
        }

        [TestMethod]
        public void TestGetOrders()
        {
            //
            // TODO: Add test logic here
            //

            IHttpActionResult actionResult = Controller.GetOrders(1).Result;
            var contentResult = actionResult as OkNegotiatedContentResult<List<Order>>;

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.IsTrue(contentResult.Content.Any());
        }
    }
}
