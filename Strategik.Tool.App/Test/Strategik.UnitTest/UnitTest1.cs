using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Strategik.Tool.Authentication;
using Strategik.Tool.Services.Client;
using Strategik.Tool.Services.Drive;
using System;
using System.Net.Http;

namespace Strategik.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        private IGraphClientService _graphClientService;
        private  IAuthHandler  _authHandler;
        private HttpClient _httpClient;
        private IDriveService _driveService;
        private ILogger<AuthHandler> _authlogger;
        private  ILogger<GraphClientService> _grapgClientLogger;

        [TestInitialize]
        public void Setup()
        {
            var auth_logger_mock = new Mock<ILogger<AuthHandler>>();
            this._authlogger= auth_logger_mock.Object;

            var graphClient_logger_mock = new Mock<ILogger<GraphClientService>>();
            this._grapgClientLogger = graphClient_logger_mock.Object;


            this._httpClient=new HttpClient();
            _authHandler = new AuthHandler(_httpClient,this._authlogger);

            _graphClientService = new GraphClientService(_authHandler,_grapgClientLogger);

            _driveService = new DriveService(_graphClientService);
           
        }

        [TestMethod]
        public void Get_Authentication_Token()
        {
          var token=  _authHandler.GetTokenAsync().GetAwaiter().GetResult();
            Assert.IsNotNull(token);
        }

        [TestMethod]
        public void Get_GraphClient()
        {
            var client = _graphClientService.GetClientAsync().GetAwaiter().GetResult();
            Assert.IsNotNull(client);
        }

        [TestMethod]
        public void UploadFile_SharePoint_Drive()
        {
            string sourceDirectory = System.Configuration.ConfigurationManager.AppSettings["SourceDirectory"];
            string sourceFullPath = sourceDirectory + "\\UnitTest-File-1.2GB.txt"; // change the filename as per file in source directory
            var result = _driveService.UploadFilesAsync(sourceFullPath).GetAwaiter().GetResult();
            Assert.AreEqual(result,1);
        }
    }
}
