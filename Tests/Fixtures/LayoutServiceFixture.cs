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
        public IConfiguration? WrongConfiguration { get; }
        public IConfiguration? IntranetConfiguration { get; }
        public IConfiguration? PublicConfiguration { get; }

        public IHttpContextAccessor ContextAccessor { get; }

        public LayoutServiceFixture()
        {
            const string configDummiesDirName = "ConfigDummies";
            const string wrongAppSettingsDummyName = "WrongAppSettingsDummy.json";
            const string intranetAppSettingsDummyName = "IntranetAppSettingsDummy.json";
            const string publicAppSettingsDummyName = "PublicAppSettingsDummy.json";

            try
            {
                string baseDirPath = AppDomain.CurrentDomain.BaseDirectory;
                string testProjDirName = "/Tests";
                if (baseDirPath.Contains("bin"))
                    testProjDirName = "../" + testProjDirName;

                if (baseDirPath.Contains("Debug"))
                    testProjDirName = "../" + testProjDirName;

                if (baseDirPath.Contains("Release"))
                    testProjDirName = "../" + testProjDirName;

                if (baseDirPath.Contains("net8.0"))
                    testProjDirName = "../" + testProjDirName;

                if (baseDirPath.Contains("Tests"))
                    testProjDirName = "../" + testProjDirName;

                
                string wrongDummyConfigFilePath = Path.Join(baseDirPath, testProjDirName, configDummiesDirName, wrongAppSettingsDummyName);
                var wrongDummyCfgBuilder = new ConfigurationBuilder();
                wrongDummyCfgBuilder.AddJsonFile(wrongDummyConfigFilePath);
                WrongConfiguration = wrongDummyCfgBuilder.Build();

                string intranetDummyConfigFilePath = Path.Join(baseDirPath, testProjDirName, configDummiesDirName, intranetAppSettingsDummyName);
                var intranetDummyCfgBuilder = new ConfigurationBuilder();
                intranetDummyCfgBuilder.AddJsonFile(intranetDummyConfigFilePath);
                IntranetConfiguration = intranetDummyCfgBuilder.Build();

                string publicDummyConfigFilePath = Path.Join(baseDirPath, testProjDirName, configDummiesDirName, publicAppSettingsDummyName);
                var publicDummyCfgBuilder = new ConfigurationBuilder();
                publicDummyCfgBuilder.AddJsonFile(publicDummyConfigFilePath);
                PublicConfiguration = publicDummyCfgBuilder.Build();

                var contextAccesorMoq = new Mock<IHttpContextAccessor>();
                ContextAccessor = contextAccesorMoq.Object;
            }
            catch (Exception ex)
            {
                var wrongDummyCfgBuilder = new Mock<ConfigurationBuilder>();
                WrongConfiguration = wrongDummyCfgBuilder.Object.Build();

                var intranetDummyCfgBuilder = new Mock<ConfigurationBuilder>();
                IntranetConfiguration = intranetDummyCfgBuilder.Object.Build();

                var publicDummyCfgBuilder = new Mock<ConfigurationBuilder>();
                PublicConfiguration = publicDummyCfgBuilder.Object.Build();

                var contextAccesorMoq = new Mock<IHttpContextAccessor>();
                ContextAccessor = contextAccesorMoq.Object;
            }
        }

        public void Dispose()
        {
        }
    }
}
