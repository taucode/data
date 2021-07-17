using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TauCode.Extensions;

namespace TauCode.Data.Tests
{
    [TestFixture]
    public class HostTests
    {
        [Test]
        public void TodoWat()
        {
            var zoldJson = this.GetType().Assembly.GetResourceText("ZoldHostTestCases.json", true);
            var zoldCases = JsonConvert.DeserializeObject<IList<ZoldHostTestCaseDto>>(zoldJson);

            var cases = zoldCases
                .Select(x => new HostTestCaseDto
                {
                    TestName = x.TestName,
                    Host = x.Host,
                    ExpectedHost = x.ExpectedHost == null ? null : new HostDto
                    {
                        Kind = x.ExpectedHostKind,
                        Value = x.ExpectedHost,
                    },
                    ExpectedTextLocationChange = x.ExpectedTextLocationChange,
                    ExpectedError = x.ExpectedError,
                    Comment = x.Comment,
                })
                .ToList();

            var json = JsonConvert.SerializeObject(cases, Formatting.Indented);
            File.WriteAllText("c:/temp/HostTestCases.json", json, Encoding.UTF8);

        }

        [TestCaseSource(nameof(TestCases))]
        [Test]
        public void TryExtract_HostProvided_ProducesExpectedResult(HostTestCaseDto testCase)
        {
            // todo:
            // "2001:1db8:3333:4444:5555:6666:7777:8888"
            // "2001:1db8:3333:4444:5555:6666:7777:8888\n"
            // "2001:1db8:3333:4444:5555:6666:123.156.117.119"

            // Arrange

            // todo: ut 4 cases, actually: 
            // * non-contaminated
            // * contaminated on left
            // * contaminated on right
            // * contaminated on both left and right


            var leftContamination = "abc";
            var rightContamination = "\r\r\ndef";

            var hostStrings = new[]
            {
                testCase.Host,
                leftContamination + testCase.Host,
                testCase.Host + rightContamination,
                leftContamination + testCase.Host + rightContamination,
            };

            var starts = new[]
            {
                0,
                leftContamination.Length,
                0,
                leftContamination.Length,
            };

            var textLocationChanges = new TextLocationChange?[hostStrings.Length];
            var hosts = new Host?[hostStrings.Length];
            var errors = new ExtractionErrorDto[hostStrings.Length];


            // todo clean

            //var hostString1 = testCase.Host;
            //var start1 = 0;

            //var hostString2 = leftContamination + testCase.Host;
            //var start2 = leftContamination.Length;

            //var hostString3 = testCase.Host + rightContamination;
            //var start3 = 0;

            //var hostString4 = leftContamination + testCase.Host + rightContamination;
            //var start4 = leftContamination.Length;


            // Act
            for (var i = 0; i < hostStrings.Length; i++)
            {
                throw new NotImplementedException();
                //textLocationChanges[i] = Host.TryExtract(hostStrings[i], starts[i], out hosts[i]);
                //errors[i] = Host.LastHostExtractionErrorInfo?.ToDto();
            }


            //var textLocationChange1 = Host.TryExtract(hostString1, 0, out var host);


            //var textLocationChange = Host.TryExtract(testCase.Host, 0, out var host);

            //var contaminatedHost = "abc" + testCase.Host + " def";
            //var textLocation2 = Host.TryExtract(contaminatedHost, "abc".Length, out var host2);

            // Assert

            for (var i = 0; i < hostStrings.Length; i++)
            {
                var host = hosts[i];
                var textLocationChange = textLocationChanges[i];
                var error = errors[i];

                if (host == null)
                {
                    Assert.That(testCase.ExpectedHost, Is.Null);
                }
                else
                {
                    Assert.That(testCase.ExpectedHost, Is.Not.Null);

                    Assert.That(host.Value.Kind, Is.EqualTo(testCase.ExpectedHost.Kind));
                    Assert.That(host.Value.Value, Is.EqualTo(testCase.ExpectedHost.Value));
                }

                if (textLocationChange == null)
                {
                    Assert.That(testCase.ExpectedTextLocationChange, Is.Null);
                }
                else
                {
                    Assert.That(testCase.ExpectedTextLocationChange, Is.Not.Null);

                    Assert.That(
                        textLocationChange.Value.LineChange,
                        Is.EqualTo(testCase.ExpectedTextLocationChange.LineChange));

                    Assert.That(
                        textLocationChange.Value.ColumnChange,
                        Is.EqualTo(testCase.ExpectedTextLocationChange.ColumnChange));

                    Assert.That(
                        textLocationChange.Value.IndexChange,
                        Is.EqualTo(testCase.ExpectedTextLocationChange.IndexChange));

                }

                if (error == null)
                {
                    Assert.That(testCase.ExpectedError, Is.Null);
                }
                else
                {
                    Assert.That(testCase.ExpectedError, Is.Not.Null);

                    Assert.That(error.LineChange, Is.EqualTo(testCase.ExpectedError.LineChange));
                    Assert.That(error.ColumnChange, Is.EqualTo(testCase.ExpectedError.ColumnChange));
                    Assert.That(error.IndexChange, Is.EqualTo(testCase.ExpectedError.IndexChange));
                    Assert.That(error.Char, Is.EqualTo(testCase.ExpectedError.Char));
                    Assert.That(error.Message, Is.EqualTo(testCase.ExpectedError.Message));
                }
            }




            //Assert.That(textLocation2, Is.EqualTo(textLocationChange));
            //Assert.That(host, Is.EqualTo(host2));

            //Assert.That(host.Kind, Is.EqualTo(testCase.ExpectedHostKind));

            //throw new NotImplementedException();
            //if (testCase.ExpectedColumnShift == null)
            //{
            //    Assert.That(textLocation, Is.Null);
            //    Assert.That(host.Value, Is.Null);
            //}
            //else
            //{
            //    Assert.That(textLocation, Is.Not.Null);
            //    Assert.That(textLocation.Value.Line, Is.Zero);
            //    Assert.That(textLocation.Value.Column, Is.EqualTo(testCase.ExpectedColumnShift.Value));

            //    Assert.That(host.Kind, Is.EqualTo(testCase.ExpectedHostKind));
            //    Assert.That(host.Value, Is.EqualTo(testCase.ExpectedHost));
            //}
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
