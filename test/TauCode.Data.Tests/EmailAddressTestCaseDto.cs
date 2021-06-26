﻿namespace TauCode.Data.Tests
{
    public class EmailAddressTestCaseDto
    {
        public EmailAddressTestCaseDto()
        {
        }

        public EmailAddressTestCaseDto(
            string name,
            string email,
            bool expectedResult,
            string comment)
        {
            this.Name = name;
            this.Email = email;
            this.ExpectedResult = expectedResult;
            this.Comment = comment;
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public bool ExpectedResult { get; set; }
        public string Comment { get; set; }

        public override string ToString()
        {
            return this.Email;
        }
    }
}
