using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using DocumentFormat.OpenXml.Packaging;
using DocuSign.MyHR.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq.Protected;
using Newtonsoft.Json;

namespace DocuSign.MyHR.UnitTests
{
    public class ClickWrapServiceTests
    {
        private static string _accountId = "1";
        private static string _userId = "2";
        private Mock<IDocuSignApiProvider> _docuSignApiProvider;

        public ClickWrapServiceTests()
        {
            _docuSignApiProvider = new Mock<IDocuSignApiProvider>();
        }

        [Fact]
        public void CreateTimeTrackClickWrap_WithCorrectInput_ReturnsCorrectResponse()
        {
            //Arrange
            var sut = CreateClickWrapService(HttpStatusCode.Created);

            //Act
            var response = sut.CreateTimeTrackClickWrap(_accountId, _userId, new[] { 1, 2, 4, 6, 6 });

            //Assert 
            response.Should().BeEquivalentTo(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { clickwrapId = "1" }))
            });
        }

        [Fact]
        public void CreateTimeTrackClickWrap_WithCorrectInput_CallsClickWrapApiWithCorrectDocument()
        {
            //Arrange
            dynamic createRequestObj = null;
            var sut = CreateClickWrapService(HttpStatusCode.Created,  (request) =>
            {
                createRequestObj = request;
            });
          
            //Act
            sut.CreateTimeTrackClickWrap(_accountId, _userId, new[] { 1, 2, 4, 6, 6 });

            //Assert - verify document content
            byte[] data = Convert.FromBase64String((string)createRequestObj.documents[0].documentBase64);
            using Stream ms = new MemoryStream(data);
            WordprocessingDocument wordDoc = WordprocessingDocument.Open(ms, false);
            Assert.Equal("I affirm I worked 19 hours this week.", wordDoc.MainDocumentPart.Document.Body.InnerText);
        }

        [Fact]
        public void CreateTimeTrackClickWrap_WhenClickWrapIsNotCreatedByApi_ThrowsInvalidOperationException()
        {
            //Arrange
            var sut = CreateClickWrapService(HttpStatusCode.BadRequest);

            //Act
            //Assert
            Assert.Throws<InvalidOperationException>(() => sut.CreateTimeTrackClickWrap(_accountId, _userId, new[] { 5, 5, 6, 8, 7 }));
        }

        [Fact]
        public void CreateTimeTrackClickWrap_WhenAccountIdIsNull_ThrowsArgumentNullException()
        {
            //Arrange
            var sut = CreateClickWrapService(HttpStatusCode.Created);

            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => sut.CreateTimeTrackClickWrap(null, _userId, new[] { 5, 5, 6, 8, 7 }));
        }

        [Fact]
        public void CreateTimeTrackClickWrap_WhenUserIdIsNull_ThrowsArgumentNullException()
        {
            //Arrange
            var sut = CreateClickWrapService(HttpStatusCode.Created);

            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => sut.CreateTimeTrackClickWrap(_accountId, null, new[] { 5, 5, 6, 8, 7 }));
        }

        [Fact]
        public void CreateTimeTrackClickWrap_WhenWorkLogIsNull_ThrowsArgumentNullException()
        {
            //Arrange
            var sut = CreateClickWrapService(HttpStatusCode.Created);

            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => sut.CreateTimeTrackClickWrap(_accountId, _userId, null));
        }

        [Fact]
        public void CreateTimeTrackClickWrap_WhenWorkLogDoesNotContainAllWorkingDays_ThrowsInvalidOperationException()
        {
            //Arrange
            var sut = CreateClickWrapService(HttpStatusCode.Created);

            //Act
            //Assert
            Assert.Throws<InvalidOperationException>(() => sut.CreateTimeTrackClickWrap(_accountId, _userId, new[] { 5, 5, 6 }));
        }

        private ClickWrapService CreateClickWrapService(HttpStatusCode clickwrapCreatedStatusCode, Action<dynamic> setRequest = null)
        {
            var docuSignApiProvider = new Mock<IDocuSignApiProvider>();
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync((HttpRequestMessage a, CancellationToken b) =>
                {
                    if (a.Method == HttpMethod.Post)
                    {
                        setRequest?.Invoke(JsonConvert.DeserializeObject<dynamic>(a.Content.ReadAsStringAsync().Result));
                    }

                    return new HttpResponseMessage
                    {
                        StatusCode = a.Method == HttpMethod.Post ? clickwrapCreatedStatusCode : HttpStatusCode.OK,
                        Content = new StringContent(JsonConvert.SerializeObject(new { clickwrapId = "1" }))
                    };
                });

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"contentRoot", "../../../../DocuSign.MyHR/"},
                })
                .Build();

            var httpClient = new HttpClient(mockMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
            docuSignApiProvider.SetupGet(c => c.DocuSignHttpClient).Returns(httpClient);

            return new ClickWrapService(docuSignApiProvider.Object, configuration);
        }
    }
}
