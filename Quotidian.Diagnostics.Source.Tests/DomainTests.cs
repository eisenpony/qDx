using System;
using Moq;
using NUnit.Framework;
using Quotidian.Diagnostics.Source;
using Quotidian.Diagnostics.Source.Domain;

namespace Tests
{
    public class DomainTests
    {
        [Test]
        public void TestWarningBasic()
        {
            var mock = new Mock<ITrace>(MockBehavior.Loose);
            mock.SetupGet(t => t.Name).Returns(GetType().FullName);

            mock.Object.Warning(TestCodes.Cheeseburger, "Hello World");

            mock.Verify(t => t.Log(It.Is<ILogEntry<string>>(e => e.Level == TraceLevel.Warning)), "Trace level was not correct");
            mock.Verify(t => t.Log(It.Is<ILogEntry<string>>(e => e.Data == "Hello World")), "Trace data was not correct");
            mock.Verify(t => t.Log(It.Is<ILogEntry<string>>(e => e.SourceName == GetType().FullName)), "Trace source name was not correct");
            mock.Verify(t => t.Log(It.Is<ILogEntry<string>>(e => e.TimeStamp <= DateTimeOffset.Now)), "Trace timestamp was not correct");
        }

        [Test]
        public void TestWarningOptional()
        {
            var mock = new Mock<ITrace>(MockBehavior.Loose);
            mock.SetupGet(t => t.Name).Returns(GetType().FullName);

            mock.Object.Warning(TestCodes.Cheeseburger, "Hello World", correlation: "1234");

            mock.Verify(t => t.Log(It.Is<ILogEntry<string>>(e => e.Level == TraceLevel.Warning)), "Trace level was not correct");
            mock.Verify(t => t.Log(It.Is<ILogEntry<string>>(e => e.Data == "Hello World")), "Trace data was not correct");
            mock.Verify(t => t.Log(It.Is<ILogEntry<string>>(e => e.SourceName == GetType().FullName)), "Trace source name was not correct");
            mock.Verify(t => t.Log(It.Is<ILogEntry<string>>(e => e.TimeStamp <= DateTimeOffset.Now)), "Trace timestamp was not correct");
            mock.Verify(t => t.Log(It.Is<ILogEntry<string>>(e => e.Code.ToString() == TestCodes.Cheeseburger.ToString())), "Trace code was not correct");
            mock.Verify(t => t.Log(It.Is<ILogEntry<string>>(e => e.CorrelationId == "1234")), "Trace correlation was not correct");
        }

        [Test]
        public void TestReport()
        {
            var mock = new Mock<ITrace>(MockBehavior.Loose);
            mock.SetupGet(t => t.Name).Returns(GetType().FullName);

            using (var report = mock.Object.Report(TestCodes.Cheeseburger, "1234"))
                report
                    .Warning("Missing quote")
                    .Warning(new InvalidOperationException("Could not find quote"));

            mock.Verify(t => t.Log(It.Is<ILogEntry<string>>(e => e.Level == TraceLevel.Warning)), "Trace level was not correct");
            mock.Verify(t => t.Log(It.Is<ILogEntry<string>>(e => e.Data == "Missing quote")), "Trace data was not correct");
            mock.Verify(t => t.Log(It.Is<ILogEntry<string>>(e => e.SourceName == GetType().FullName)), "Trace source name was not correct");
            mock.Verify(t => t.Log(It.Is<ILogEntry<string>>(e => e.TimeStamp <= DateTimeOffset.Now)), "Trace timestamp was not correct");
            mock.Verify(t => t.Log(It.Is<ILogEntry<string>>(e => e.Code.ToString() == TestCodes.Cheeseburger.ToString())), "Trace code was not correct");
            mock.Verify(t => t.Log(It.Is<ILogEntry<string>>(e => e.CorrelationId == "1234")), "Trace correlation was not correct");

            mock.Verify(t => t.Log(It.Is<ILogEntry<Exception>>(e => e.Level == TraceLevel.Warning)), "Trace level was not correct");
            mock.Verify(t => t.Log(It.Is<ILogEntry<Exception>>(e => e.Data.Message == "Could not find quote")), "Trace data was not correct");
            mock.Verify(t => t.Log(It.Is<ILogEntry<Exception>>(e => e.SourceName == GetType().FullName)), "Trace source name was not correct");
            mock.Verify(t => t.Log(It.Is<ILogEntry<Exception>>(e => e.TimeStamp <= DateTimeOffset.Now)), "Trace timestamp was not correct");
            mock.Verify(t => t.Log(It.Is<ILogEntry<Exception>>(e => e.Code.ToString() == TestCodes.Cheeseburger.ToString())), "Trace code was not correct");
            mock.Verify(t => t.Log(It.Is<ILogEntry<Exception>>(e => e.CorrelationId == "1234")), "Trace correlation was not correct");
        }

        private enum TestCodes
        {
            Cheeseburger,
            Hamburger,
        }
    }
}