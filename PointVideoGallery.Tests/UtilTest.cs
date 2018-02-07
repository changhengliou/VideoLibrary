using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PointVideoGallery.Api;
using PointVideoGallery.Models;

namespace PointVideoGallery.Tests
{
    [TestClass]
    class UtilTest
    {
        [TestMethod]
        public void IsUserRoleValid()
        {
            // Arrange
            AccountApiController controller = new AccountApiController();

            // Act
//            var result = controller.IsUserRoleValid(new UserRole
//            {
//                Id = 1,
//                EnableAdmin = "x",
//                EnableEvent = "x",
//                EnableLocation = "x",
//                EnablePublish = "x",
//                EnableResource = "x",
//                EnableSo = "x"
//            });

            // Assert
//            Assert.IsTrue(result);
        }
    }
}