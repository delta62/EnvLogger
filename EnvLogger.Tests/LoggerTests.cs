using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace EnvLogger.Tests
{
    [TestFixture]
    class LoggerTests
    {
        private StringWriter _stderr;

        private Logger _log;

        private string Logs { get { return _stderr.ToString(); } }

        [SetUp]
        public void SetUp()
        {
            _log = new Logger();
            _stderr = new StringWriter();
            Console.SetError(_stderr);
        }

        [Test]
        public void ShouldPrintMessage()
        {
            _log.Error("test {0}", 1);
            StringAssert.Contains("test 1", Logs);
        }

        [Test]
        public void ShouldPrefixMessageWithLogLevel()
        {
            _log.Error("test");
            StringAssert.StartsWith("ERROR", Logs);
        }

        [Test]
        public void ShouldPrefixMessageWithTime()
        {
            _log.Error("test");
            var re = @"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}Z.*test";
            StringAssert.IsMatch(re, Logs);
        }

        [Test]
        public void ShouldPrefixMessageWithAssembly()
        {
            var asm = Assembly.GetExecutingAssembly().GetName().Name;
            _log.Error("test");
            StringAssert.IsMatch($"{asm}.*test", Logs);
        }

        [Test]
        public void ShouldNotPrintNonErrorsByDefault()
        {
            _log.Warn("test");
            Assert.AreEqual("", Logs);
        }

        [Test]
        public void ShouldPrintFromAssembly()
        {
            var asm = Assembly.GetExecutingAssembly().GetName().Name;
            Environment.SetEnvironmentVariable("DOTNET_LOG", $"{asm}=info");
            Logger.ConfigureLevels();
            _log.Info("test");
            StringAssert.Contains("test", Logs);
        }

        [Test]
        public void ShouldPrintFromGlobalLevel()
        {
            Environment.SetEnvironmentVariable("DOTNET_LOG", "info");
            Logger.ConfigureLevels();
            _log.Info("test");
            StringAssert.Contains("test", Logs);
        }
    }
}
