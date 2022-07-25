using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using TauCode.Data.Tests.Dto;
using TauCode.Extensions;

namespace TauCode.Data.Tests;

[TestFixture]
public class IntervalTests
{
    #region ctor - Nullable type

    [Test]
    [TestCaseSource(nameof(GetCtorNullableTypeTestDtos))]
    public void Ctor_NullableType_ReturnsExpectedResult(IntervalCtorTestDto testDto)
    {
        // Arrange
        ExtractInterval(testDto.TestInterval, out var start, out var end, out var isStartIncluded, out var isEndIncluded);

        Interval<int?> interval;

        // Act & Assert
        if (testDto.ExceptionException == null)
        {
            interval = new Interval<int?>(start, end, isStartIncluded, isEndIncluded);
            Assert.That(interval.Start, Is.EqualTo(testDto.ExpectedInterval.Start));
            Assert.That(interval.End, Is.EqualTo(testDto.ExpectedInterval.End));
            Assert.That(interval.IsStartIncluded, Is.EqualTo(testDto.ExpectedInterval.IsStartIncluded));
            Assert.That(interval.IsEndIncluded, Is.EqualTo(testDto.ExpectedInterval.IsEndIncluded));

            Assert.That(interval.ToString(), Is.EqualTo(testDto.TestInterval));
        }
        else
        {
            Assert.That(testDto.ExpectedInterval, Is.Null);

            var ex = Assert.Throws<ArgumentException>(() =>
                interval = new Interval<int?>(start, end, isStartIncluded, isEndIncluded));

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Does.StartWith(testDto.ExceptionException.Message));
            Assert.That(ex.ParamName, Is.EqualTo(testDto.ExceptionException.ParamName));
        }
    }

    public static IList<IntervalCtorTestDto> GetCtorNullableTypeTestDtos()
    {
        var json = typeof(IntervalTests).Assembly.GetResourceText("IntervalCtorTests.Nullable.json", true);
        var list = JsonConvert.DeserializeObject<IList<IntervalCtorTestDto>>(json);

        for (var i = 0; i < list.Count; i++)
        {
            list[i].Index = i;
        }

        return list;
    }

    #endregion

    #region ctor - Non-nullable type

    [Test]
    [TestCaseSource(nameof(GetCtorNonNullableTypeTestDtos))]
    public void Ctor_NonNullableType_ReturnsExpectedResult(IntervalCtorTestDto testDto)
    {
        // Arrange
        ExtractInterval(testDto.TestInterval, out var start, out var end, out var isStartIncluded, out var isEndIncluded);

        Interval<int> interval;

        // Act & Assert
        if (testDto.ExceptionException == null)
        {
            interval = new Interval<int>(start.Value, end.Value, isStartIncluded, isEndIncluded);
            Assert.That(interval.Start, Is.EqualTo(testDto.ExpectedInterval.Start));
            Assert.That(interval.End, Is.EqualTo(testDto.ExpectedInterval.End));
            Assert.That(interval.IsStartIncluded, Is.EqualTo(testDto.ExpectedInterval.IsStartIncluded));
            Assert.That(interval.IsEndIncluded, Is.EqualTo(testDto.ExpectedInterval.IsEndIncluded));

            Assert.That(interval.ToString(), Is.EqualTo(testDto.TestInterval));
        }
        else
        {
            Assert.That(testDto.ExpectedInterval, Is.Null);

            var ex = Assert.Throws<ArgumentException>(() =>
                interval = new Interval<int>(start.Value, end.Value, isStartIncluded, isEndIncluded));

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Does.StartWith(testDto.ExceptionException.Message));
            Assert.That(ex.ParamName, Is.EqualTo(testDto.ExceptionException.ParamName));
        }
    }

    public static IList<IntervalCtorTestDto> GetCtorNonNullableTypeTestDtos()
    {
        var json = typeof(IntervalTests).Assembly.GetResourceText("IntervalCtorTests.NonNullable.json", true);
        var list = JsonConvert.DeserializeObject<IList<IntervalCtorTestDto>>(json);

        for (var i = 0; i < list.Count; i++)
        {
            list[i].Index = i;
        }

        return list;
    }


    #endregion

    #region Contains - Nullable type

    [Test]
    [TestCase("(-∞, +∞)", 7, true)]
    [TestCase("(-∞, 10]", 7, true)]
    [TestCase("(-∞, 10)", 7, true)]
    [TestCase("(-∞, 7)", 7, false)]
    [TestCase("(-∞, 7]", 7, true)]
    [TestCase("(-∞, 5)", 7, false)]
    [TestCase("(-∞, 5]", 7, false)]
    [TestCase("[5, +∞)", 7, true)]
    [TestCase("(5, +∞)", 7, true)]
    [TestCase("[7, +∞)", 7, true)]
    [TestCase("(7, +∞)", 7, false)]
    [TestCase("(7, 11)", 7, false)]
    [TestCase("[7, 11)", 7, true)]
    [TestCase("[-1, 5]", 7, false)]
    [TestCase("[-1, 5)", 7, false)]
    [TestCase("[-1, 7)", 7, false)]
    [TestCase("[-1, 7]", 7, true)]
    [TestCase("(8, +∞)", 7, false)]
    [TestCase("[8, +∞)", 7, false)]
    [TestCase("[8, 11]", 7, false)]
    [TestCase("(8, 11)", 7, false)]
    public void ContainsValue_NullableType_ReturnsExpectedResult(string intervalString, int value, bool expectedResult)
    {
        // Arrange
        var interval = ParseNullableInterval(intervalString);

        // Act
        var contains = interval.Contains(value);

        // Assert
        Assert.That(contains, Is.EqualTo(expectedResult));
    }

    #endregion

    #region Contains - Non-nullable type

    [Test]
    [TestCase("[-1, 5]", 7, false)]
    [TestCase("[-1, 5)", 7, false)]
    [TestCase("[-1, 7)", 7, false)]
    [TestCase("[-1, 7]", 7, true)]
    [TestCase("(7, 11)", 7, false)]
    [TestCase("[7, 11)", 7, true)]
    [TestCase("[8, 11]", 7, false)]
    [TestCase("(8, 11)", 7, false)]
    public void ContainsValue_NonNullableType_ReturnsExpectedResult(string intervalString, int value, bool expectedResult)
    {
        // Arrange
        var interval = ParseNonNullableInterval(intervalString);

        // Act
        var contains = interval.Contains(value);

        // Assert
        Assert.That(contains, Is.EqualTo(expectedResult));
    }

    #endregion

    #region IsSubsetOf - Nullable type

    [Test]
    [TestCaseSource(nameof(GetNullableTestCasesForIsSubsetOf))]
    public void IsSubsetOf_IntervalWithNullableType_ReturnsExpectedResult(string testCase)
    {
        // Arrange
        bool expectedIsSubset;
        string[] parts;

        if (testCase.Contains('⊆'))
        {
            expectedIsSubset = true;
            parts = testCase
                .Split('⊆')
                .Select(x => x.Trim())
                .ToArray();
        }
        else if (testCase.Contains('⊄'))
        {
            expectedIsSubset = false;
            parts = testCase
                .Split('⊄')
                .Select(x => x.Trim())
                .ToArray();
        }
        else
        {
            throw new Exception();
        }

        var thisInterval = ParseNullableInterval(parts.First());
        var argumentInterval = ParseNullableInterval(parts.Skip(1).Single());

        // Act
        var isSubset = thisInterval.IsSubsetOf(argumentInterval);

        // Assert
        Assert.That(isSubset, Is.EqualTo(expectedIsSubset));
    }

    #endregion

    #region IsSubsetOf - Non-nullable type

    [Test]
    [TestCaseSource(nameof(GetNonNullableTestCasesForIsSubsetOf))]
    public void IsSubsetOf_IntervalWithNonNullableType_ReturnsExpectedResult(string testCase)
    {
        // Arrange
        bool expectedIsSubset;
        string[] parts;

        if (testCase.Contains('⊆'))
        {
            expectedIsSubset = true;
            parts = testCase
                .Split('⊆')
                .Select(x => x.Trim())
                .ToArray();
        }
        else if (testCase.Contains('⊄'))
        {
            expectedIsSubset = false;
            parts = testCase
                .Split('⊄')
                .Select(x => x.Trim())
                .ToArray();
        }
        else
        {
            throw new Exception();
        }

        var thisInterval = ParseNonNullableInterval(parts.First());
        var argumentInterval = ParseNonNullableInterval(parts.Skip(1).Single());

        // Act
        var isSubset = thisInterval.IsSubsetOf(argumentInterval);

        // Assert
        Assert.That(isSubset, Is.EqualTo(expectedIsSubset));
    }

    #endregion

    private static void ExtractInterval(
        string intervalString,
        out int? start,
        out int? end,
        out bool isStartIncluded,
        out bool isEndIncluded)
    {
        var c = intervalString[0];
        if (c == '[')
        {
            isStartIncluded = true;
        }
        else if (c == '(')
        {
            isStartIncluded = false;
        }
        else
        {
            throw new Exception();
        }

        c = intervalString[^1];
        if (c == ']')
        {
            isEndIncluded = true;
        }
        else if (c == ')')
        {
            isEndIncluded = false;
        }
        else
        {
            throw new Exception();
        }

        var content = intervalString.Substring(1, intervalString.Length - 2);
        var parts = content.Split(',').Select(x => x.Trim()).ToList();

        var first = parts.First();
        if (first == "-∞")
        {
            start = null;
        }
        else
        {
            start = int.Parse(first);
        }

        var second = parts.Skip(1).Single();
        if (second == "+∞")
        {
            end = null;
        }
        else
        {
            end = int.Parse(second);
        }
    }

    private static Interval<int?> ParseNullableInterval(
        string intervalString)
    {
        ExtractInterval(
            intervalString,
            out var start,
            out var end,
            out var isStartIncluded,
            out var isEndIncluded);

        return new Interval<int?>(start, end, isStartIncluded, isEndIncluded);
    }

    private static Interval<int> ParseNonNullableInterval(
        string intervalString)
    {
        ExtractInterval(
            intervalString,
            out var start,
            out var end,
            out var isStartIncluded,
            out var isEndIncluded);

        return new Interval<int>(start.Value, end.Value, isStartIncluded, isEndIncluded);
    }

    public static IList<string> GetNullableTestCasesForIsSubsetOf()
    {
        return new List<string>
        {
            #region universum contains anything

            "(-∞, +∞) ⊆ (-∞, +∞)",

            "(12, 12) ⊆ (-∞, +∞)",
            "[12, 12] ⊆ (-∞, +∞)",

            "(-∞, 12] ⊆ (-∞, +∞)",
            "(-∞, 12) ⊆ (-∞, +∞)",

            "[12, +∞) ⊆ (-∞, +∞)",
            "(12, +∞) ⊆ (-∞, +∞)",

            "[12, 17] ⊆ (-∞, +∞)",
            "(12, 17] ⊆ (-∞, +∞)",
            "[12, 17) ⊆ (-∞, +∞)",
            "(12, 17) ⊆ (-∞, +∞)",

            #endregion

            #region belonging to (-∞, x]

            "(-∞, +∞) ⊄ (-∞, 12]",

            "(12, 12) ⊆ (-∞, 12]",
            "(17, 17) ⊆ (-∞, 12]",
            "(10, 10) ⊆ (-∞, 12]",

            "[12, 12] ⊆ (-∞, 12]",
            "[17, 17] ⊄ (-∞, 12]",
            "[10, 10] ⊆ (-∞, 12]",

            "(-∞, 12] ⊆ (-∞, 12]",
            "(-∞, 12) ⊆ (-∞, 12]",

            "(-∞, 10] ⊆ (-∞, 12]",
            "(-∞, 10) ⊆ (-∞, 12]",

            "[10, 11] ⊆ (-∞, 12]",
            "(10, 11] ⊆ (-∞, 12]",
            "[10, 11) ⊆ (-∞, 12]",
            "(10, 11) ⊆ (-∞, 12]",

            "[10, 12] ⊆ (-∞, 12]",
            "(10, 12] ⊆ (-∞, 12]",
            "[10, 12) ⊆ (-∞, 12]",
            "(10, 12) ⊆ (-∞, 12]",

            "(-∞, 13] ⊄ (-∞, 12]" ,
            "(-∞, 13) ⊄ (-∞, 12]" ,

            "[10, 13] ⊄ (-∞, 12]" ,
            "(10, 13] ⊄ (-∞, 12]" ,
            "[10, 13) ⊄ (-∞, 12]" ,
            "(10, 13) ⊄ (-∞, 12]" ,

            "[12, +∞) ⊄ (-∞, 12]" ,
            "(12, +∞) ⊄ (-∞, 12]" ,

            "[13, +∞) ⊄ (-∞, 12]" ,
            "(13, +∞) ⊄ (-∞, 12]" ,

            "[13, 17] ⊄ (-∞, 12]" ,
            "(13, 17] ⊄ (-∞, 12]" ,
            "[13, 17) ⊄ (-∞, 12]" ,
            "(13, 17) ⊄ (-∞, 12]" ,

            #endregion

            #region belonging to (-∞, x)

            "(-∞, +∞) ⊄ (-∞, 12)",

            "(12, 12) ⊆ (-∞, 12)",
            "(17, 17) ⊆ (-∞, 12)",
            "(10, 10) ⊆ (-∞, 12)",

            "[12, 12] ⊄ (-∞, 12)",
            "[17, 17] ⊄ (-∞, 12)",
            "[10, 10] ⊆ (-∞, 12]",

            "(-∞, 12] ⊄ (-∞, 12)",
            "(-∞, 12) ⊆ (-∞, 12)",

            "(-∞, 10] ⊆ (-∞, 12)",
            "(-∞, 10) ⊆ (-∞, 12)",

            "[10, 11] ⊆ (-∞, 12)",
            "(10, 11] ⊆ (-∞, 12)",
            "[10, 11) ⊆ (-∞, 12)",
            "(10, 11) ⊆ (-∞, 12)",

            "[10, 12] ⊄ (-∞, 12)",
            "(10, 12] ⊄ (-∞, 12)",
            "[10, 12) ⊆ (-∞, 12)",
            "(10, 12) ⊆ (-∞, 12)",

            "(-∞, 13] ⊄ (-∞, 12)",
            "(-∞, 13) ⊄ (-∞, 12)",

            "[10, 13] ⊄ (-∞, 12)",
            "(10, 13] ⊄ (-∞, 12)",
            "[10, 13) ⊄ (-∞, 12)",
            "(10, 13) ⊄ (-∞, 12)",

            "[12, +∞) ⊄ (-∞, 12)",
            "(12, +∞) ⊄ (-∞, 12)",

            "[13, +∞) ⊄ (-∞, 12)",
            "(13, +∞) ⊄ (-∞, 12)",

            "[13, 17] ⊄ (-∞, 12)",
            "(13, 17] ⊄ (-∞, 12)",
            "[13, 17) ⊄ (-∞, 12)",
            "(13, 17) ⊄ (-∞, 12)",

            #endregion

            #region belonging to [x, +∞)

            "(-∞, +∞) ⊄ [12, +∞)",

            "(12, 12) ⊆ [12, +∞)",
            "(17, 17) ⊆ [12, +∞)",
            "(10, 10) ⊆ (12, +∞)",

            "[12, 12] ⊆ [12, +∞)",
            "[17, 17] ⊆ [12, +∞)",
            "[10, 10] ⊄ [12, +∞)",

            "(-∞, 12] ⊄ [12, +∞)",
            "(-∞, 12) ⊄ [12, +∞)",

            "(-∞, 10] ⊄ [12, +∞)",
            "(-∞, 10) ⊄ [12, +∞)",

            "[10, 11] ⊄ [12, +∞)",
            "(10, 11] ⊄ [12, +∞)",
            "[10, 11) ⊄ [12, +∞)",
            "(10, 11) ⊄ [12, +∞)",

            "[10, 12] ⊄ [12, +∞)",
            "(10, 12] ⊄ [12, +∞)",
            "[10, 12) ⊄ [12, +∞)",
            "(10, 12) ⊄ [12, +∞)",

            "(-∞, 13] ⊄ [12, +∞)",
            "(-∞, 13) ⊄ [12, +∞)",

            "[10, 13] ⊄ [12, +∞)",
            "(10, 13] ⊄ [12, +∞)",
            "[10, 13) ⊄ [12, +∞)",
            "(10, 13) ⊄ [12, +∞)",

            "[12, +∞) ⊆ [12, +∞)",
            "(12, +∞) ⊆ [12, +∞)",

            "[13, +∞) ⊆ [12, +∞)",
            "(13, +∞) ⊆ [12, +∞)",

            "[13, 17] ⊆ [12, +∞)",
            "(13, 17] ⊆ [12, +∞)",
            "[13, 17) ⊆ [12, +∞)",
            "(13, 17) ⊆ [12, +∞)",

            #endregion

            #region belonging to (x, +∞)

            "(-∞, +∞) ⊄ (12, +∞)",

            "(12, 12) ⊆ (12, +∞)",
            "(17, 17) ⊆ (12, +∞)",
            "(10, 10) ⊆ (12, +∞)",

            "[12, 12] ⊄ (12, +∞)",
            "[17, 17] ⊆ (12, +∞)",
            "[10, 10] ⊄ (12, +∞)",

            "(-∞, 12] ⊄ (12, +∞)",
            "(-∞, 12) ⊄ (12, +∞)",

            "(-∞, 10] ⊄ (12, +∞)",
            "(-∞, 10) ⊄ (12, +∞)",

            "[10, 11] ⊄ (12, +∞)",
            "(10, 11] ⊄ (12, +∞)",
            "[10, 11) ⊄ (12, +∞)",
            "(10, 11) ⊄ (12, +∞)",

            "[10, 12] ⊄ (12, +∞)",
            "(10, 12] ⊄ (12, +∞)",
            "[10, 12) ⊄ (12, +∞)",
            "(10, 12) ⊄ (12, +∞)",

            "(-∞, 13] ⊄ (12, +∞)",
            "(-∞, 13) ⊄ (12, +∞)",

            "[10, 13] ⊄ (12, +∞)",
            "(10, 13] ⊄ (12, +∞)",
            "[10, 13) ⊄ (12, +∞)",
            "(10, 13) ⊄ (12, +∞)",

            "[12, +∞) ⊄ (12, +∞)",
            "(12, +∞) ⊆ (12, +∞)",

            "[13, +∞) ⊆ (12, +∞)",
            "(13, +∞) ⊆ (12, +∞)",

            "[13, 17] ⊆ (12, +∞)",
            "(13, 17] ⊆ (12, +∞)",
            "[13, 17) ⊆ (12, +∞)",
            "(13, 17) ⊆ (12, +∞)",

            #endregion

            #region belonging to [x, x]

            "(-∞, +∞) ⊄ [12, 12]",

            "(12, 12) ⊆ [12, 12]",
            "(17, 17) ⊆ [12, 12]",
            "(10, 10) ⊆ [12, 12]",

            "[12, 12] ⊆ [12, 12]",
            "[17, 17] ⊄ [12, 12]",
            "[10, 10] ⊄ [12, 12]",

            "(-∞, 12] ⊄ [12, 12]",
            "(-∞, 12) ⊄ [12, 12]",

            "(-∞, 10] ⊄ [12, 12]",
            "(-∞, 10) ⊄ [12, 12]",

            "[10, 11] ⊄ [12, 12]",
            "(10, 11] ⊄ [12, 12]",
            "[10, 11) ⊄ [12, 12]",
            "(10, 11) ⊄ [12, 12]",

            "[10, 12] ⊄ [12, 12]",
            "(10, 12] ⊄ [12, 12]",
            "[10, 12) ⊄ [12, 12]",
            "(10, 12) ⊄ [12, 12]",

            "(-∞, 13] ⊄ [12, 12]",
            "(-∞, 13) ⊄ [12, 12]",

            "[10, 13] ⊄ [12, 12]",
            "(10, 13] ⊄ [12, 12]",
            "[10, 13) ⊄ [12, 12]",
            "(10, 13) ⊄ [12, 12]",

            "[12, +∞) ⊄ [12, 12]",
            "(12, +∞) ⊄ [12, 12]",

            "[13, +∞) ⊄ [12, 12]",
            "(13, +∞) ⊄ [12, 12]",

            "[13, 17] ⊄ [12, 12]",
            "(13, 17] ⊄ [12, 12]",
            "[13, 17) ⊄ [12, 12]",
            "(13, 17) ⊄ [12, 12]",

            #endregion

            #region belonging to (x, x)

            "(-∞, +∞) ⊄ (12, 12)",

            "(12, 12) ⊆ (12, 12)",
            "(17, 17) ⊆ (12, 12)",
            "(10, 10) ⊆ (12, 12)",

            "[12, 12] ⊄ (12, 12)",
            "[17, 17] ⊄ (12, 12)",
            "[10, 10] ⊄ (12, 12)",

            "(-∞, 12] ⊄ (12, 12)",
            "(-∞, 12) ⊄ (12, 12)",

            "(-∞, 10] ⊄ (12, 12)",
            "(-∞, 10) ⊄ (12, 12)",

            "[10, 11] ⊄ (12, 12)",
            "(10, 11] ⊄ (12, 12)",
            "[10, 11) ⊄ (12, 12)",
            "(10, 11) ⊄ (12, 12)",

            "[10, 12] ⊄ (12, 12)",
            "(10, 12] ⊄ (12, 12)",
            "[10, 12) ⊄ (12, 12)",
            "(10, 12) ⊄ (12, 12)",

            "(-∞, 13] ⊄ (12, 12)",
            "(-∞, 13) ⊄ (12, 12)",

            "[10, 13] ⊄ (12, 12)",
            "(10, 13] ⊄ (12, 12)",
            "[10, 13) ⊄ (12, 12)",
            "(10, 13) ⊄ (12, 12)",

            "[12, +∞) ⊄ (12, 12)",
            "(12, +∞) ⊄ (12, 12)",

            "[13, +∞) ⊄ (12, 12)",
            "(13, +∞) ⊄ (12, 12)",

            "[13, 17] ⊄ (12, 12)",
            "(13, 17] ⊄ (12, 12)",
            "[13, 17) ⊄ (12, 12)",
            "(13, 17) ⊄ (12, 12)",

            #endregion

            #region belonging to [x, y]

            "(-∞, +∞) ⊄ [12, 17]",

            "(10, 10) ⊆ [12, 17]",
            "(12, 12) ⊆ [12, 17]",
            "(13, 13) ⊆ [12, 17]",
            "(17, 17) ⊆ [12, 17]",
            "(19, 19) ⊆ [12, 17]",

            "[10, 10] ⊄ [12, 17]",
            "[12, 12] ⊆ [12, 17]",
            "[13, 13] ⊆ [12, 17]",
            "[17, 17] ⊆ [12, 17]",
            "[19, 19] ⊄ [12, 17]",

            "(-∞, 10] ⊄ [12, 17]",
            "(-∞, 10) ⊄ [12, 17]",

            "(-∞, 12] ⊄ [12, 17]",
            "(-∞, 12) ⊄ [12, 17]",

            "(-∞, 13] ⊄ [12, 17]",
            "(-∞, 13) ⊄ [12, 17]",

            "(-∞, 17] ⊄ [12, 17]",
            "(-∞, 17) ⊄ [12, 17]",

            "(-∞, 19] ⊄ [12, 17]",
            "(-∞, 19) ⊄ [12, 17]",

            "[10, +∞) ⊄ [12, 17]",
            "(10, +∞) ⊄ [12, 17]",

            "[12, +∞) ⊄ [12, 17]",
            "(12, +∞) ⊄ [12, 17]",

            "[13, +∞) ⊄ [12, 17]",
            "(13, +∞) ⊄ [12, 17]",

            "[17, +∞) ⊄ [12, 17]",
            "(17, +∞) ⊄ [12, 17]",

            "[19, +∞) ⊄ [12, 17]",
            "(19, +∞) ⊄ [12, 17]",

            "[10, 11] ⊄ [12, 17]",
            "(10, 11] ⊄ [12, 17]",
            "[10, 11) ⊄ [12, 17]",
            "(10, 11) ⊄ [12, 17]",

            "[10, 12] ⊄ [12, 17]",
            "(10, 12] ⊄ [12, 17]",
            "[10, 12) ⊄ [12, 17]",
            "(10, 12) ⊄ [12, 17]",

            "[10, 13] ⊄ [12, 17]",
            "(10, 13] ⊄ [12, 17]",
            "[10, 13) ⊄ [12, 17]",
            "(10, 13) ⊄ [12, 17]",

            "[10, 17] ⊄ [12, 17]",
            "(10, 17] ⊄ [12, 17]",
            "[10, 17) ⊄ [12, 17]",
            "(10, 17) ⊄ [12, 17]",

            "[10, 19] ⊄ [12, 17]",
            "(10, 19] ⊄ [12, 17]",
            "[10, 19) ⊄ [12, 17]",
            "(10, 19) ⊄ [12, 17]",

            "[12, 13] ⊆ [12, 17]",
            "(12, 13] ⊆ [12, 17]",
            "[12, 13) ⊆ [12, 17]",
            "(12, 13) ⊆ [12, 17]",

            "[12, 17] ⊆ [12, 17]",
            "(12, 17] ⊆ [12, 17]",
            "[12, 17) ⊆ [12, 17]",
            "(12, 17) ⊆ [12, 17]",

            "[12, 19] ⊄ [12, 17]",
            "(12, 19] ⊄ [12, 17]",
            "[12, 19) ⊄ [12, 17]",
            "(12, 19) ⊄ [12, 17]",

            "[13, 15] ⊆ [12, 17]",
            "(13, 15] ⊆ [12, 17]",
            "[13, 15) ⊆ [12, 17]",
            "(13, 15) ⊆ [12, 17]",
                    
            "[13, 17] ⊆ [12, 17]",
            "(13, 17] ⊆ [12, 17]",
            "[13, 17) ⊆ [12, 17]",
            "(13, 17) ⊆ [12, 17]",

            "[13, 19] ⊄ [12, 17]",
            "(13, 19] ⊄ [12, 17]",
            "[13, 19) ⊄ [12, 17]",
            "(13, 19) ⊄ [12, 17]",
                   
            "[17, 19] ⊄ [12, 17]",
            "(17, 19] ⊄ [12, 17]",
            "[17, 19) ⊄ [12, 17]",
            "(17, 19) ⊄ [12, 17]",
                   
            "[19, 21] ⊄ [12, 17]",
            "(19, 21] ⊄ [12, 17]",
            "[19, 21) ⊄ [12, 17]",
            "(19, 21) ⊄ [12, 17]",

            #endregion

            #region belonging to (x, y]

            "(-∞, +∞) ⊄ (12, 17]",

            "(10, 10) ⊆ (12, 17]",
            "(12, 12) ⊆ (12, 17]",
            "(13, 13) ⊆ (12, 17]",
            "(17, 17) ⊆ (12, 17]",
            "(19, 19) ⊆ (12, 17]",

            "[10, 10] ⊄ (12, 17]",
            "[12, 12] ⊄ (12, 17]",
            "[13, 13] ⊆ (12, 17]",
            "[17, 17] ⊆ (12, 17]",
            "[19, 19] ⊄ (12, 17]",

            "(-∞, 10] ⊄ (12, 17]",
            "(-∞, 10) ⊄ (12, 17]",

            "(-∞, 12] ⊄ (12, 17]",
            "(-∞, 12) ⊄ (12, 17]",

            "(-∞, 13] ⊄ (12, 17]",
            "(-∞, 13) ⊄ (12, 17]",

            "(-∞, 17] ⊄ (12, 17]",
            "(-∞, 17) ⊄ (12, 17]",

            "(-∞, 19] ⊄ (12, 17]",
            "(-∞, 19) ⊄ (12, 17]",

            "[10, +∞) ⊄ (12, 17]",
            "(10, +∞) ⊄ (12, 17]",

            "[12, +∞) ⊄ (12, 17]",
            "(12, +∞) ⊄ (12, 17]",

            "[13, +∞) ⊄ (12, 17]",
            "(13, +∞) ⊄ (12, 17]",

            "[17, +∞) ⊄ (12, 17]",
            "(17, +∞) ⊄ (12, 17]",

            "[19, +∞) ⊄ (12, 17]",
            "(19, +∞) ⊄ (12, 17]",

            "[10, 11] ⊄ (12, 17]",
            "(10, 11] ⊄ (12, 17]",
            "[10, 11) ⊄ (12, 17]",
            "(10, 11) ⊄ (12, 17]",

            "[10, 12] ⊄ (12, 17]",
            "(10, 12] ⊄ (12, 17]",
            "[10, 12) ⊄ (12, 17]",
            "(10, 12) ⊄ (12, 17]",

            "[10, 13] ⊄ (12, 17]",
            "(10, 13] ⊄ (12, 17]",
            "[10, 13) ⊄ (12, 17]",
            "(10, 13) ⊄ (12, 17]",

            "[10, 17] ⊄ (12, 17]",
            "(10, 17] ⊄ (12, 17]",
            "[10, 17) ⊄ (12, 17]",
            "(10, 17) ⊄ (12, 17]",

            "[10, 19] ⊄ (12, 17]",
            "(10, 19] ⊄ (12, 17]",
            "[10, 19) ⊄ (12, 17]",
            "(10, 19) ⊄ (12, 17]",

            "[12, 13] ⊄ (12, 17]",
            "(12, 13] ⊆ (12, 17]",
            "[12, 13) ⊄ (12, 17]",
            "(12, 13) ⊆ (12, 17]",

            "[12, 17] ⊄ (12, 17]",
            "(12, 17] ⊆ (12, 17]",
            "[12, 17) ⊄ (12, 17]",
            "(12, 17) ⊆ (12, 17]",

            "[12, 19] ⊄ (12, 17]",
            "(12, 19] ⊄ (12, 17]",
            "[12, 19) ⊄ (12, 17]",
            "(12, 19) ⊄ (12, 17]",

            "[13, 15] ⊆ (12, 17]",
            "(13, 15] ⊆ (12, 17]",
            "[13, 15) ⊆ (12, 17]",
            "(13, 15) ⊆ (12, 17]",

            "[13, 17] ⊆ (12, 17]",
            "(13, 17] ⊆ (12, 17]",
            "[13, 17) ⊆ (12, 17]",
            "(13, 17) ⊆ (12, 17]",

            "[13, 19] ⊄ (12, 17]",
            "(13, 19] ⊄ (12, 17]",
            "[13, 19) ⊄ (12, 17]",
            "(13, 19) ⊄ (12, 17]",

            "[17, 19] ⊄ (12, 17]",
            "(17, 19] ⊄ (12, 17]",
            "[17, 19) ⊄ (12, 17]",
            "(17, 19) ⊄ (12, 17]",

            "[19, 21] ⊄ (12, 17]",
            "(19, 21] ⊄ (12, 17]",
            "[19, 21) ⊄ (12, 17]",
            "(19, 21) ⊄ (12, 17]",

            #endregion

            #region belonging to [x, y)

            "(-∞, +∞) ⊄ [12, 17)",

            "(10, 10) ⊆ [12, 17)",
            "(12, 12) ⊆ [12, 17)",
            "(13, 13) ⊆ [12, 17)",
            "(17, 17) ⊆ [12, 17)",
            "(19, 19) ⊆ [12, 17)",

            "[10, 10] ⊄ [12, 17)",
            "[12, 12] ⊆ [12, 17)",
            "[13, 13] ⊆ [12, 17)",
            "[17, 17] ⊄ [12, 17)",
            "[19, 19] ⊄ [12, 17)",

            "(-∞, 10] ⊄ [12, 17)",
            "(-∞, 10) ⊄ [12, 17)",

            "(-∞, 12] ⊄ [12, 17)",
            "(-∞, 12) ⊄ [12, 17)",

            "(-∞, 13] ⊄ [12, 17)",
            "(-∞, 13) ⊄ [12, 17)",

            "(-∞, 17] ⊄ [12, 17)",
            "(-∞, 17) ⊄ [12, 17)",

            "(-∞, 19] ⊄ [12, 17)",
            "(-∞, 19) ⊄ [12, 17)",

            "[10, +∞) ⊄ [12, 17)",
            "(10, +∞) ⊄ [12, 17)",

            "[12, +∞) ⊄ [12, 17)",
            "(12, +∞) ⊄ [12, 17)",

            "[13, +∞) ⊄ [12, 17)",
            "(13, +∞) ⊄ [12, 17)",

            "[17, +∞) ⊄ [12, 17)",
            "(17, +∞) ⊄ [12, 17)",

            "[19, +∞) ⊄ [12, 17)",
            "(19, +∞) ⊄ [12, 17)",

            "[10, 11] ⊄ [12, 17)",
            "(10, 11] ⊄ [12, 17)",
            "[10, 11) ⊄ [12, 17)",
            "(10, 11) ⊄ [12, 17)",

            "[10, 12] ⊄ [12, 17)",
            "(10, 12] ⊄ [12, 17)",
            "[10, 12) ⊄ [12, 17)",
            "(10, 12) ⊄ [12, 17)",

            "[10, 13] ⊄ [12, 17)",
            "(10, 13] ⊄ [12, 17)",
            "[10, 13) ⊄ [12, 17)",
            "(10, 13) ⊄ [12, 17)",

            "[10, 17] ⊄ [12, 17)",
            "(10, 17] ⊄ [12, 17)",
            "[10, 17) ⊄ [12, 17)",
            "(10, 17) ⊄ [12, 17)",

            "[10, 19] ⊄ [12, 17)",
            "(10, 19] ⊄ [12, 17)",
            "[10, 19) ⊄ [12, 17)",
            "(10, 19) ⊄ [12, 17)",

            "[12, 13] ⊆ [12, 17)",
            "(12, 13] ⊆ [12, 17)",
            "[12, 13) ⊆ [12, 17)",
            "(12, 13) ⊆ [12, 17)",

            "[12, 17] ⊄ [12, 17)",
            "(12, 17] ⊄ [12, 17)",
            "[12, 17) ⊆ [12, 17)",
            "(12, 17) ⊆ [12, 17)",

            "[12, 19] ⊄ [12, 17)",
            "(12, 19] ⊄ [12, 17)",
            "[12, 19) ⊄ [12, 17)",
            "(12, 19) ⊄ [12, 17)",

            "[13, 15] ⊆ [12, 17)",
            "(13, 15] ⊆ [12, 17)",
            "[13, 15) ⊆ [12, 17)",
            "(13, 15) ⊆ [12, 17)",

            "[13, 17] ⊄ [12, 17)",
            "(13, 17] ⊄ [12, 17)",
            "[13, 17) ⊆ [12, 17)",
            "(13, 17) ⊆ [12, 17)",

            "[13, 19] ⊄ [12, 17)",
            "(13, 19] ⊄ [12, 17)",
            "[13, 19) ⊄ [12, 17)",
            "(13, 19) ⊄ [12, 17)",
              
            "[17, 19] ⊄ [12, 17)",
            "(17, 19] ⊄ [12, 17)",
            "[17, 19) ⊄ [12, 17)",
            "(17, 19) ⊄ [12, 17)",
              
            "[19, 21] ⊄ [12, 17)",
            "(19, 21] ⊄ [12, 17)",
            "[19, 21) ⊄ [12, 17)",
            "(19, 21) ⊄ [12, 17)",

            #endregion

            #region belonging to (x, y)

            "(-∞, +∞) ⊄ (12, 17)",

            "(10, 10) ⊆ (12, 17)",
            "(12, 12) ⊆ (12, 17)",
            "(13, 13) ⊆ (12, 17)",
            "(17, 17) ⊆ (12, 17)",
            "(19, 19) ⊆ (12, 17)",

            "[10, 10] ⊄ (12, 17)",
            "[12, 12] ⊄ (12, 17)",
            "[13, 13] ⊆ (12, 17)",
            "[17, 17] ⊄ (12, 17)",
            "[19, 19] ⊄ (12, 17)",

            "(-∞, 10] ⊄ (12, 17)",
            "(-∞, 10) ⊄ (12, 17)",

            "(-∞, 12] ⊄ (12, 17)",
            "(-∞, 12) ⊄ (12, 17)",

            "(-∞, 13] ⊄ (12, 17)",
            "(-∞, 13) ⊄ (12, 17)",

            "(-∞, 17] ⊄ (12, 17)",
            "(-∞, 17) ⊄ (12, 17)",

            "(-∞, 19] ⊄ (12, 17)",
            "(-∞, 19) ⊄ (12, 17)",

            "[10, +∞) ⊄ (12, 17)",
            "(10, +∞) ⊄ (12, 17)",

            "[12, +∞) ⊄ (12, 17)",
            "(12, +∞) ⊄ (12, 17)",

            "[13, +∞) ⊄ (12, 17)",
            "(13, +∞) ⊄ (12, 17)",

            "[17, +∞) ⊄ (12, 17)",
            "(17, +∞) ⊄ (12, 17)",

            "[19, +∞) ⊄ (12, 17)",
            "(19, +∞) ⊄ (12, 17)",

            "[10, 11] ⊄ (12, 17)",
            "(10, 11] ⊄ (12, 17)",
            "[10, 11) ⊄ (12, 17)",
            "(10, 11) ⊄ (12, 17)",

            "[10, 12] ⊄ (12, 17)",
            "(10, 12] ⊄ (12, 17)",
            "[10, 12) ⊄ (12, 17)",
            "(10, 12) ⊄ (12, 17)",

            "[10, 13] ⊄ (12, 17)",
            "(10, 13] ⊄ (12, 17)",
            "[10, 13) ⊄ (12, 17)",
            "(10, 13) ⊄ (12, 17)",

            "[10, 17] ⊄ (12, 17)",
            "(10, 17] ⊄ (12, 17)",
            "[10, 17) ⊄ (12, 17)",
            "(10, 17) ⊄ (12, 17)",

            "[10, 19] ⊄ (12, 17)",
            "(10, 19] ⊄ (12, 17)",
            "[10, 19) ⊄ (12, 17)",
            "(10, 19) ⊄ (12, 17)",

            "[12, 13] ⊄ (12, 17)",
            "(12, 13] ⊆ (12, 17)",
            "[12, 13) ⊄ (12, 17)",
            "(12, 13) ⊆ (12, 17)",

            "[12, 17] ⊄ (12, 17)",
            "(12, 17] ⊄ (12, 17)",
            "[12, 17) ⊄ (12, 17)",
            "(12, 17) ⊆ (12, 17)",

            "[12, 19] ⊄ (12, 17)",
            "(12, 19] ⊄ (12, 17)",
            "[12, 19) ⊄ (12, 17)",
            "(12, 19) ⊄ (12, 17)",

            "[13, 15] ⊆ (12, 17)",
            "(13, 15] ⊆ (12, 17)",
            "[13, 15) ⊆ (12, 17)",
            "(13, 15) ⊆ (12, 17)",

            "[13, 17] ⊄ (12, 17)",
            "(13, 17] ⊄ (12, 17)",
            "[13, 17) ⊆ (12, 17)",
            "(13, 17) ⊆ (12, 17)",

            "[13, 19] ⊄ (12, 17)",
            "(13, 19] ⊄ (12, 17)",
            "[13, 19) ⊄ (12, 17)",
            "(13, 19) ⊄ (12, 17)",

            "[17, 19] ⊄ (12, 17)",
            "(17, 19] ⊄ (12, 17)",
            "[17, 19) ⊄ (12, 17)",
            "(17, 19) ⊄ (12, 17)",

            "[19, 21] ⊄ (12, 17)",
            "(19, 21] ⊄ (12, 17)",
            "[19, 21) ⊄ (12, 17)",
            "(19, 21) ⊄ (12, 17)",

            #endregion
        };
    }

    public static IList<string> GetNonNullableTestCasesForIsSubsetOf() =>
        GetNullableTestCasesForIsSubsetOf()
            .Where(x => !x.Contains('∞'))
            .ToList();
}
