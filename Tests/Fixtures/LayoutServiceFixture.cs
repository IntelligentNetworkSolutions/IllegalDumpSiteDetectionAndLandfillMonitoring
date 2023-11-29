using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Tests.Fixtures
{
    public class LayoutServiceFixture : IDisposable
    {
        public IConfiguration WrongConfiguration { get; }
        public IConfiguration IntranetConfiguration { get; }
        public IConfiguration PublicConfiguration { get; }

        public IHttpContextAccessor ContextAccessor { get; }

        public LayoutServiceFixture()
        {
            string wrongDummyConfigFilePath = "C:\\Visual Studio Projects\\IllegalDumpSiteDetectionAndLandfillMonitoring\\Tests\\ConfigDummies\\WrongAppSettingsDummy.json";
            var wrongDummyCfgBuilder = new ConfigurationBuilder();
            wrongDummyCfgBuilder.AddJsonFile(wrongDummyConfigFilePath);
            WrongConfiguration = wrongDummyCfgBuilder.Build();

            string intranetDummyConfigFilePath = "C:\\Visual Studio Projects\\IllegalDumpSiteDetectionAndLandfillMonitoring\\Tests\\ConfigDummies\\IntranetAppSettingsDummy.json";
            var intranetDummyCfgBuilder = new ConfigurationBuilder();
            intranetDummyCfgBuilder.AddJsonFile(intranetDummyConfigFilePath);
            IntranetConfiguration = intranetDummyCfgBuilder.Build();

            string publicDummyConfigFilePath = "C:\\Visual Studio Projects\\IllegalDumpSiteDetectionAndLandfillMonitoring\\Tests\\ConfigDummies\\PublicAppSettingsDummy.json";
            var publicDummyCfgBuilder = new ConfigurationBuilder();
            publicDummyCfgBuilder.AddJsonFile(publicDummyConfigFilePath);
            PublicConfiguration = publicDummyCfgBuilder.Build();

            var contextAccesorMoq = new Mock<IHttpContextAccessor>();
            ContextAccessor = contextAccesorMoq.Object;
        }

        public void Dispose()
        {
        }
    }
}
