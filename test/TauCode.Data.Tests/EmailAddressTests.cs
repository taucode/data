using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TauCode.Extensions;

namespace TauCode.Data.Tests
{
    [TestFixture]
    public class EmailAddressTests
    {
        [TestCaseSource(nameof(TestCases))]
        public void TryExtract_InputProvided_ExpectedResult(EmailAddressTestCaseDto testCase)
        {
            // Arrange
            var email = testCase.Email.Replace('␀', '\0');

            // todo: ut 4 cases, actually: 
            // * non-contaminated
            // * contaminated on left
            // * contaminated on right
            // * contaminated on both left and right

            var contaminatedEmail = "abc" + email + " aa";

            // Act
            var location = EmailAddress.TryExtract(contaminatedEmail, "abc".Length, out var emailAddress);

            // Assert
            throw new NotImplementedException("go on!");

            //Assert.That(isEmail, Is.EqualTo(testCase.ExpectedResult));
        }

        [TestCaseSource(nameof(ExtraTestCases))]
        public void TryExtract_ExtraCases_ExpectedResult(EmailAddressTestCaseDto testCase)
        {
            // Arrange
            var email = testCase.Email.Replace('␀', '\0');

            var contaminatedEmail = "abc" + email + " aa";

            // Act
            var location = EmailAddress.TryExtract(contaminatedEmail, "abc".Length, out var emailAddress);

            // Assert
            throw new NotImplementedException("go on!");

            //Assert.That(isEmail, Is.EqualTo(testCase.ExpectedResult));
        }

        public static IList<EmailAddressTestCaseDto> TestCases
        {
            get
            {
                var json = typeof(EmailAddressTests).Assembly.GetResourceText("EmailAddressTestCases.json", true);
                var list = JsonConvert.DeserializeObject<IList<EmailAddressTestCaseDto>>(json);
                return list;
            }
        }

        public static IList<EmailAddressTestCaseDto> ExtraTestCases
        {
            get
            {
                var json = typeof(EmailAddressTests).Assembly.GetResourceText("EmailAddressTestCases.Extra.json", true);
                var list = JsonConvert.DeserializeObject<IList<EmailAddressTestCaseDto>>(json);
                return list;
            }
        }
    }
}