using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Xunit;
using AutoFixture.Xunit2;
using DocuSign.eSign.Api;
using DocuSign.eSign.Model;
using DocuSign.MyHR.Domain;
using DocuSign.MyHR.Exceptions;
using DocuSign.MyHR.Services;
using DocuSign.MyHR.Services.TemplateHandlers;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace DocuSign.MyHR.UnitTests
{
    public class EnvelopeServiceTests
    {
        private static string _accountId = "1";
        private static string _userId = "2";
        private static LoginType _loginType = LoginType.CodeGrant;
        private Mock<IDocuSignApiProvider> _docuSignApiProvider;
        private Mock<IUserService> _userService;
        private Mock<IEnvelopesApi> _envelopeApi;
        private Mock<ITemplatesApi> _templateApi;
        private Mock<IAccountsApi> _accountsApi;

        public EnvelopeServiceTests()
        {
           _docuSignApiProvider = new Mock<IDocuSignApiProvider>();
           _userService = new Mock<IUserService>();
           _envelopeApi = new Mock<IEnvelopesApi>();
           _templateApi = new Mock<ITemplatesApi>();
           _accountsApi = new Mock<IAccountsApi>();

            _envelopeApi.Setup(x => x.CreateEnvelope(_accountId, It.IsAny<EnvelopeDefinition>(), It.IsAny<EnvelopesApi.CreateEnvelopeOptions>()))
                .Returns(() => new EnvelopeSummary(EnvelopeId: "1"));

           _envelopeApi.Setup(x => x.CreateRecipientView(_accountId, "1", It.IsAny<RecipientViewRequest>()))
                .Returns((string a, string b, RecipientViewRequest c) =>
                    new ViewUrl($"accountId={a}&templateId={b}&userEmail={c.Email}&userName={c.UserName}"));

           _docuSignApiProvider.SetupGet(c => c.EnvelopApi).Returns(_envelopeApi.Object);
           

           _templateApi.Setup(x => x.CreateTemplate(_accountId, It.IsAny<EnvelopeTemplate>()))
                .Returns((string a, EnvelopeTemplate b) =>
                    new TemplateSummary(DocumentName: b.Name, TemplateId: "1"));

           _docuSignApiProvider.SetupGet(c => c.TemplatesApi).Returns(_templateApi.Object);
        }

        [Theory]
        [InlineAutoData(DocumentType.W4)]
        [InlineAutoData(DocumentType.Offer)]
        [InlineAutoData(DocumentType.DirectDeposit)]
        [InlineAutoData(DocumentType.TuitionRbt)]
        public void CreateEnvelope_WhenCorrectParameters_ReturnsCorrectResult(
            DocumentType type,
            UserDetails userInformation,
            UserDetails additionalUser)
        {
            //Arrange
            _userService.Setup(x => x.GetUserDetails(_accountId, _userId, _loginType)).Returns(userInformation);

            var sut = new EnvelopeService(_docuSignApiProvider.Object, _userService.Object, SetupConfiguration());

            //Act
            CreateEnvelopeResponse res = sut.CreateEnvelope(type, _accountId, _userId, LoginType.CodeGrant, additionalUser, "", "");

            //Assert
            res.Should().BeEquivalentTo(new CreateEnvelopeResponse(
                $"accountId={_accountId}&templateId=1&userEmail={userInformation.Email}&userName={userInformation.Name}",
                "1"));
        }

        [Theory]
        [InlineAutoData(DocumentType.I9)]
        public void CreateEnvelope_WhenCorrectParametersAndWhenDocumentTypeI9_ReturnsCorrectResultWithRedirectUrlEmply(
            DocumentType type,
            UserDetails userInformation,
            UserDetails additionalUser)
        {
            //Arrange                      
            _userService.Setup(x => x.GetUserDetails(_accountId, _userId, _loginType)).Returns(userInformation);

            var sut = new EnvelopeService(_docuSignApiProvider.Object, _userService.Object, SetupConfiguration());

            //Act
            CreateEnvelopeResponse res = sut.CreateEnvelope(type, _accountId, _userId, LoginType.CodeGrant, additionalUser, "", "");

            //Assert
            res.Should().BeEquivalentTo(new CreateEnvelopeResponse(string.Empty, "1"));
        }

        [Theory]
        [InlineAutoData(DocumentType.None)]
        public void CreateEnvelope_WhenDocumentTypeNone_ThrowsInvalidOperationException(
            DocumentType type,
            UserDetails userInformation,
            UserDetails additionalUser)
        {
            //Arrange                      
            _userService.Setup(x => x.GetUserDetails(_accountId, _userId, _loginType)).Returns(userInformation);

            var sut = new EnvelopeService(_docuSignApiProvider.Object, _userService.Object, SetupConfiguration());

            //Act
            //Assert
            Assert.Throws<InvalidOperationException>(() =>
                sut.CreateEnvelope(type, _accountId, _userId, LoginType.CodeGrant, additionalUser, "", ""));
        }

        [Theory]
        [AutoData]
        public void CreateEnvelope_WhenDocumentTypeI9AndLoginTypeJWT_RequestedEnvelopeTemplateWithIdvEnabled(
            UserDetails userInformation,
            UserDetails additionalUser)
        {
            //Arrange           
            _userService.Setup(x => x.GetUserDetails(_accountId, _userId, _loginType)).Returns(userInformation);
            _accountsApi.Setup(x => x.GetAccountIdentityVerification(_accountId, new AccountsApi.GetAccountIdentityVerificationOptions())).Returns(() =>
                    new AccountIdentityVerificationResponse
                    {
                        IdentityVerification = new List<AccountIdentityVerificationWorkflow>
                        {
                            new AccountIdentityVerificationWorkflow(WorkflowId: "100")
                        }
                    });
            _docuSignApiProvider.SetupGet(c => c.AccountsApi).Returns(_accountsApi.Object);

            var sut = new EnvelopeService(_docuSignApiProvider.Object, _userService.Object, SetupConfiguration());

            //Act
            sut.CreateEnvelope(DocumentType.I9, _accountId, _userId, LoginType.JWT, additionalUser, "", "");

            //Assert
            EnvelopeTemplate templateToExpect = new I9TemplateHandler().BuildTemplate("../../../../DocuSign.MyHR/");
            templateToExpect.Recipients.Signers.First().IdentityVerification = new RecipientIdentityVerification(WorkflowId: "100");

            _docuSignApiProvider.Verify(mock => mock.TemplatesApi.CreateTemplate(_accountId, templateToExpect), Times.Once());
        }

        [Theory]
        [AutoData]
        public void CreateEnvelope_WhenDocumentTypeI9AndLoginTypeCodeGrant_RequestedEnvelopeTemplateWithIdvNotEnabled(
            UserDetails userInformation,
            UserDetails additionalUser)
        {
            //Arrange  
            _userService.Setup(x => x.GetUserDetails(_accountId, _userId, _loginType)).Returns(userInformation);
            _accountsApi.Setup(x => x.GetAccountIdentityVerification(_accountId, new AccountsApi.GetAccountIdentityVerificationOptions())).Returns(() =>
                    new AccountIdentityVerificationResponse
                    {
                        IdentityVerification = new List<AccountIdentityVerificationWorkflow>
                        {
                            new AccountIdentityVerificationWorkflow(WorkflowId: "100")
                        }
                    });
            _docuSignApiProvider.SetupGet(c => c.AccountsApi).Returns(_accountsApi.Object);

            var sut = new EnvelopeService(_docuSignApiProvider.Object, _userService.Object, SetupConfiguration());

            //Act
            sut.CreateEnvelope(DocumentType.I9, _accountId, _userId, LoginType.CodeGrant, additionalUser, "", "");

            //Assert
            EnvelopeTemplate templateToExpect = new I9TemplateHandler().BuildTemplate("../../../../DocuSign.MyHR/");
            templateToExpect.Recipients.Signers.First().IdentityVerification = null;

            _docuSignApiProvider.Verify(mock => mock.TemplatesApi.CreateTemplate(_accountId, templateToExpect), Times.Once());
        }

        [Theory]
        [AutoData]
        public void CreateEnvelope_WhenDocumentTypeI9AndLoginTypeJWTButIdvForAccountNotEnabled_ThrowsIDVException(
            UserDetails userInformation,
            UserDetails additionalUser)
        {
            //Arrange
            _userService.Setup(x => x.GetUserDetails(_accountId, _userId, _loginType)).Returns(userInformation);
            _accountsApi.Setup(x => x.GetAccountIdentityVerification(_accountId, new AccountsApi.GetAccountIdentityVerificationOptions()))
                .Returns(() =>
                    new AccountIdentityVerificationResponse
                    {
                        IdentityVerification = new List<AccountIdentityVerificationWorkflow>()
                    });
            _docuSignApiProvider.SetupGet(c => c.AccountsApi).Returns(_accountsApi.Object);

            var sut = new EnvelopeService(_docuSignApiProvider.Object, _userService.Object, SetupConfiguration());

            //Act
            //Assert
            Assert.Throws<IDVException>(() => sut.CreateEnvelope(DocumentType.I9, _accountId, _userId, LoginType.JWT, additionalUser, "", ""));
        }

        [Theory]
        [InlineAutoData(DocumentType.W4)]
        [InlineAutoData(DocumentType.Offer)]
        [InlineAutoData(DocumentType.DirectDeposit)]
        [InlineAutoData(DocumentType.I9)]
        [InlineAutoData(DocumentType.TuitionRbt)]
        public void CreateEnvelope_WhenAccountIdNull_ThrowsArgumentException(
            DocumentType type,
            UserDetails additionalUser)
        {
            //Arrange
            var sut = new EnvelopeService(_docuSignApiProvider.Object, _userService.Object, SetupConfiguration());

            //Act - Assert
            Assert.Throws<ArgumentNullException>(() => sut.CreateEnvelope(type, null, _userId, LoginType.CodeGrant, additionalUser, "", ""));
        }

        [Theory]
        [InlineAutoData(DocumentType.W4)]
        [InlineAutoData(DocumentType.Offer)]
        [InlineAutoData(DocumentType.DirectDeposit)]
        [InlineAutoData(DocumentType.I9)]
        [InlineAutoData(DocumentType.TuitionRbt)]
        public void CreateEnvelope_WhenUserIdNull_ThrowsArgumentException(
            DocumentType type,
            UserDetails additionalUser)
        {
            //Arrange
            var sut = new EnvelopeService(_docuSignApiProvider.Object, _userService.Object, SetupConfiguration());

            //Act - Assert
            Assert.Throws<ArgumentNullException>(() => sut.CreateEnvelope(type, _accountId, null, LoginType.CodeGrant, additionalUser, "", ""));
        }

        [Theory]
        [InlineAutoData(DocumentType.Offer)]
        [InlineAutoData(DocumentType.I9)]
        public void CreateEnvelope_WhenAdditionalUserNullAndDocumentTypeI9OrOffer_ThrowsArgumentException(
            DocumentType type,
            UserDetails userInformation)
        {
            //Arrange
            _userService.Setup(x => x.GetUserDetails(_accountId, _userId, _loginType)).Returns(userInformation);
            var sut = new EnvelopeService(_docuSignApiProvider.Object, _userService.Object, SetupConfiguration());

            //Act - Assert
            Assert.Throws<ArgumentNullException>(() => sut.CreateEnvelope(type, _accountId, _userId, LoginType.CodeGrant, null, "", ""));
        }

        [Theory]
        [InlineAutoData(DocumentType.W4)]
        [InlineAutoData(DocumentType.DirectDeposit)]
        [InlineAutoData(DocumentType.TuitionRbt)]
        public void CreateEnvelope_WhenAdditionalUserNullAndDocumentTypeDirectDepositOrW4_NotThrowsArgumentException(
            DocumentType type,
            UserDetails userInformation)
        {
            //Arrange
            _userService.Setup(x => x.GetUserDetails(_accountId, _userId, _loginType)).Returns(userInformation);
            var sut = new EnvelopeService(_docuSignApiProvider.Object, _userService.Object, SetupConfiguration());

            //Act - Assert
            sut.CreateEnvelope(type, _accountId, _userId, LoginType.CodeGrant, null, "", "");
        }

        [Fact]
        public void GetEnvelope_WhenCorrectRequestParameters_ReturnsCorrectFormData()
        {
            //Arrange
            var envelopeId = "1";

            _envelopeApi.Setup(x => x.GetFormData(_accountId, envelopeId)).Returns(() => new EnvelopeFormData(string.Empty, "1",
                    new List<FormDataItem>
                    {
                        new FormDataItem { Name = "Field1", Value = "Value1" }
                    }));
            _docuSignApiProvider.SetupGet(c => c.EnvelopApi).Returns(_envelopeApi.Object);
            var sut = new EnvelopeService(_docuSignApiProvider.Object, _userService.Object, SetupConfiguration());

            //Act - Assert
            Dictionary<string, string> result = sut.GetEnvelopData(_accountId, envelopeId);

            //Assert
            result.Should().BeEquivalentTo(new Dictionary<string, string>()
            {
                {  "Field1", "Value1"}
            });
        }

        [Fact]
        public void GetEnvelope_WhenAccountIdIsNull_ThrowsArgumentException()
        {
            //Arrange
            var envelopeId = "1";
            var sut = new EnvelopeService(_docuSignApiProvider.Object, _userService.Object, SetupConfiguration());

            //Act - Assert
            Assert.Throws<ArgumentNullException>(() => sut.GetEnvelopData(null, envelopeId));
        }

        [Fact]
        public void GetEnvelope_WhenEnvelopeIdIsNull_ThrowsArgumentException()
        {
            //Arrange 
            var sut = new EnvelopeService(_docuSignApiProvider.Object, _userService.Object, SetupConfiguration());

            //Act - Assert
            Assert.Throws<ArgumentNullException>(() => sut.GetEnvelopData(_accountId, null));
        }

        private static IConfiguration SetupConfiguration()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                {"contentRoot", "../../../../DocuSign.MyHR/"},
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            return configuration;
        }
    }
}
