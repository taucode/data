using NUnit.Framework;

namespace TauCode.Data.Tests;

[TestFixture]
public class ValueDictionaryTests
{
    [Test]
    public void Constructor_ObjectProvided_CreatesDictionary()
    {
        // Arrange

        // Act
        var dictionary = new ValueDictionary(new { id = 1, name = "marina" });

        // Assert
        Assert.That(dictionary["id"], Is.EqualTo(1));
        Assert.That(dictionary["name"], Is.EqualTo("marina"));
    }
}