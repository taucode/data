using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using TauCode.Extensions;

namespace TauCode.Data.Tests
{
    [TestFixture]
    public class HostTests
    {
        [TestCaseSource(nameof(TestCases))]
        [Test]
        public void TryExtract_HostProvided_ProducesExpectedResult(HostTestCaseDto testCase)
        {
            // todo:
            // "2001:1db8:3333:4444:5555:6666:7777:8888"
            // "2001:1db8:3333:4444:5555:6666:7777:8888\n"
            // "2001:1db8:3333:4444:5555:6666:123.156.117.119"

            // Arrange
            //var span = testCase.Host.AsSpan();

            // Act
            var textLocation = Host.TryExtract(testCase.Host, 0, out var host);

            // todo uncomment
            //var contaminatedHost = "abc" + testCase.Host + " def";
            //var textLocation2 = Host.TryExtract(contaminatedHost, "abc".Length, out var host2);

            // Assert
            // todo uncomment
            //Assert.That(textLocation2, Is.EqualTo(textLocation));
            //Assert.That(host, Is.EqualTo(host2));

            Assert.That(host.Kind, Is.EqualTo(testCase.ExpectedHostKind));

            if (testCase.ExpectedColumnShift == null)
            {
                Assert.That(textLocation, Is.Null);
                Assert.That(host.Value, Is.Null);
            }
            else
            {
                Assert.That(textLocation, Is.Not.Null);
                Assert.That(textLocation.Value.Line, Is.Zero);
                Assert.That(textLocation.Value.Column, Is.EqualTo(testCase.ExpectedColumnShift.Value));

                Assert.That(host.Kind, Is.EqualTo(testCase.ExpectedHostKind));
                Assert.That(host.Value, Is.EqualTo(testCase.ExpectedHost));
            }
        }

        public static IList<HostTestCaseDto> TestCases
        {
            get
            {
                var json = typeof(HostTests).Assembly.GetResourceText("HostTestCases.json", true);
                var result = JsonConvert.DeserializeObject<IList<HostTestCaseDto>>(json);
                return result;
            }
        }
    }
}
