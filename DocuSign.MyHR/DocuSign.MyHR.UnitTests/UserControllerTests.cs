using System.Collections.Generic;
using System.Security.Claims;
using Moq;
using Xunit;
using AutoFixture.Xunit2;
using DocuSign.MyHR.Controllers;
using DocuSign.MyHR.Domain;
using DocuSign.MyHR.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DocuSign.MyHR.UnitTests
{
    public class UserControllerTests
    {
        private Mock<IUserService> _userService;

        public UserControllerTests()
        {
            _userService = new Mock<IUserService>();
        }

        [Theory, AutoData]
        public void Index_WhenGetWithCorrectParameters_ReturnsCorrectResult(
            Mock<IUserService> userService,
            UserDetails userDetails,
            Account account,
            User user)
        {
            //Arrange
            InitContext(account, user);
            userService.Setup(c => c.GetUserDetails(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<LoginType>()))
                .Returns(userDetails);

            var sut = new UserController(userService.Object);

            //Act
            var result = sut.Index();

            //Assert 
            result.Should().BeEquivalentTo(new OkObjectResult(userDetails)); 
        }

        [Fact]
        public void Index_WhenPutWithModelStateInvalid_ReturnsBadRequestResult()
        {
            // Arrange        
            var sut = new UserController(_userService.Object);
    
            // Act
            var result = sut.Index(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory, AutoData]
        public void Index_WhenPutWithCorrectParameters_ReturnsCorrectResult(
            Mock<IUserService> userService,
            UserDetails userDetails,
            Account account,
            User user)
        {
            //Arrange
            InitContext(account, user);

            var sut = new UserController(userService.Object);

            //Act
            var result = sut.Index(userDetails);

            //Assert
            Assert.True(result is OkResult);
        } 

        private void InitContext(Account account, User user)
        {
            var context = new Context();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim("authType", LoginType.CodeGrant.ToString())
            };
            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            claimsIdentity.AddClaim(new Claim("accounts", JsonConvert.SerializeObject(account)));
            claimsIdentity.AddClaim(new Claim("account_id", account.Id));

            context.Init(new ClaimsPrincipal(claimsIdentity));
        }
    }
}
