using NUnit.Framework;

namespace TauCode.Data.Tests
{
    [TestFixture]
    public class SemanticVersionTests
    {
        #region ctor (5 args)

        [Test]
        [TestCase(
            0,
            0,
            0,
            null,
            null,
            "0.0.0",
            "0.0.0")]
        [TestCase(
            1,
            0,
            0,
            null,
            "-",
            "1.0.0+-",
            "1.0.0")]
        [TestCase(
            2,
            3,
            1,
            "-rc.alpha.beta.123",
            "meta.3491",
            "2.3.1--rc.alpha.beta.123+meta.3491",
            "2.3.1--rc.alpha.beta.123")]
        public void Ctor5Args_ValidArguments_RunsOk(
            int major,
            int minor,
            int patch,
            string suffix,
            string metadata,
            string expectedFullText,
            string expectedTextWithoutMetadata)
        {
            // Arrange

            // Act
            var version = new SemanticVersion(major, minor, patch, suffix, metadata);

            // Assert
            Assert.That(version.Major, Is.EqualTo(major));
            Assert.That(version.Minor, Is.EqualTo(minor));
            Assert.That(version.Patch, Is.EqualTo(patch));
            Assert.That(version.Suffix, Is.EqualTo(suffix));
            Assert.That(version.Metadata, Is.EqualTo(metadata));

            Assert.That(version.ToString(true), Is.EqualTo(expectedFullText));
            Assert.That(version.ToString(false), Is.EqualTo(expectedTextWithoutMetadata));
            Assert.That(version.ToString(), Is.EqualTo(expectedFullText));
        }

        #endregion

        [Test]
        [TestCase(0, 0, 1, null, "0.0.1")]
        [TestCase(0, 2, 7, null, "0.2.7")]
        [TestCase(3, 11, 16, null, "3.11.16")]
        [TestCase(7, 4, 8, "rc2", "7.4.8-rc2")]
        public void Ctor_ValidArguments_ProducesValidVersion(
            int major,
            int minor,
            int patch,
            string suffix,
            string expectedVersionString)
        {
            // Arrange

            // Act
            var version = new SemanticVersion(major, minor, patch, suffix, null);

            // Assert
            Assert.That(version.Major, Is.EqualTo(major));
            Assert.That(version.Minor, Is.EqualTo(minor));
            Assert.That(version.Patch, Is.EqualTo(patch));
            Assert.That(version.Suffix, Is.EqualTo(suffix));

            Assert.That(version.ToString(), Is.EqualTo(expectedVersionString));
        }

        [Test]
        [TestCase("1-rc", "1.0-rc")]
        [TestCase("37.77", "37.77.0")]
        public void OperatorEquals_VersionsAreEqual_ReturnsTrue(string s1, string s2)
        {
            // Arrange
            var v1 = new SemanticVersion(s1);
            var v2 = new SemanticVersion(s2);

            // Act
            var eq = v1 == v2;

            // Assert
            Assert.That(eq, Is.True);
        }

        [Test]
        public void OperatorLess_VersionsAreSequential_ReturnsTrue()
        {
            // Arrange
            var v1 = new SemanticVersion("1.0.0-alpha");
            var v2 = new SemanticVersion("1.0.0-alpha.1");
            var v3 = new SemanticVersion("1.0.0-alpha.beta");
            var v4 = new SemanticVersion("1.0.0-beta");
            var v5 = new SemanticVersion("1.0.0-beta.2");
            var v6 = new SemanticVersion("1.0.0-beta.11");
            var v7 = new SemanticVersion("1.0.0-rc.1");
            var v8 = new SemanticVersion("1.0.0");

            // Act
            var statement =
                v1 < v2 &&
                v2 < v3 &&
                v3 < v4 &&
                v4 < v5 &&
                v5 < v6 &&
                v6 < v7 &&
                v7 < v8 &&
                true;

            // Assert
            Assert.That(statement, Is.True);
        }

    }
}
