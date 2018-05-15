using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace EnvLogger.Tests
{
    [TestFixture]
    class LoggerTests
    {
        private StringWriter _console;

        private Logger _log;

        [SetUp]
        public void SetUp()
        {
            _log = new Logger();
            _console = new StringWriter();
            Console.SetOut(_console);
        }

        [Test]
        public void ShouldPrintMessage()
        {
            _log.Error("test {0}", 1);
            StringAssert.Contains("test 1", _console.ToString());
        }

        [Test]
        public void ShouldPrefixMessageWithLogLevel()
        {
            _log.Error("test");
            StringAssert.StartsWith("ERROR", _console.ToString());
        }

        [Test]
        public void ShouldPrefixMessageWithTime()
        {
            _log.Error("test");
            var re = @"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}Z.*test";
            StringAssert.IsMatch(re, _console.ToString());
        }

        [Test]
        public void ShouldPrefixMessageWithAssembly()
        {
            var asm = Assembly.GetExecutingAssembly().GetName().Name;
            _log.Error("test");
            StringAssert.IsMatch($"{asm}.*test", _console.ToString());
        }

        [Test]
        public void ShouldNotPrintNonErrorsByDefault()
        {
            _log.Warn("test");
            Assert.AreEqual("", _console.ToString());
        }

        [Test]
        public void ShouldPrintFromAssembly()
        {
            var asm = Assembly.GetExecutingAssembly().GetName().Name;
            Environment.SetEnvironmentVariable("DOTNET_LOG", $"{asm}=info");
            Logger.ConfigureLevels();
            _log.Info("test");
            StringAssert.Contains("test", _console.ToString());
        }

        [Test]
        public void ShouldPrintFromGlobalLevel()
        {
            var asm = Assembly.GetExecutingAssembly().GetName().Name;
            Environment.SetEnvironmentVariable("DOTNET_LOG", "info");
            Logger.ConfigureLevels();
            _log.Info("test");
            StringAssert.Contains("test", _console.ToString());
        }
    }
}
