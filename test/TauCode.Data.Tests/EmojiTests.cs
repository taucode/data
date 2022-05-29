using System.Linq;
using System.Text;
using NUnit.Framework;

namespace TauCode.Data.Tests
{
    [TestFixture]
    public class EmojiTests
    {
        /// <summary>
        /// 'Master' means an emoji that contains other emoji as starting substring
        /// </summary>
        [Test]
        public void TryExtract_IncompleteMasterEmoji_ExtractsPartially()
        {
            // Arrange
            var walesFlagEmoji = Emoji.EnumerateAll().Single(x => x.Name == "flag: Wales");
            var sb = new StringBuilder(walesFlagEmoji.Value);
            sb[^1] = 'X';
            var input = sb.ToString();

            var blackFlagEmoji = Emoji.EnumerateAll().Single(x => x.Name == "black flag");

            // Act
            var consumed = Emoji.TryExtract(input, out var emoji, out var error);

            // Assert
            Assert.That(consumed, Is.Not.Null);
            Assert.That(consumed, Is.EqualTo(blackFlagEmoji.Value.Length));

            Assert.That(emoji, Is.EqualTo(blackFlagEmoji));
            Assert.That(error, Is.Null);
        }

        [Test]
        public void TryExtract_NonEmojiInput_ReturnsNull()
        {
            // Arrange
            var input = "abc";

            // Act
            var consumed = Emoji.TryExtract(input, out var emoji, out var error);

            // Assert
            Assert.That(consumed, Is.Null);
            Assert.That(emoji, Is.Null);
            Assert.That(error, Is.Not.Null);
            Assert.That(error.ErrorIndex, Is.EqualTo(0));
            Assert.That(error.Message, Is.EqualTo("Non-emoji character."));
        }
    }
}
