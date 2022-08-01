﻿using NUnit.Framework;
using TauCode.Data.Tests.Data;
using TauCode.Data.Tests.Dto;
using TauCode.Data.Tests.Dto.TestDto;

namespace TauCode.Data.Tests;

[TestFixture]
public class IntervalTestsStruct
{
    [Test]
    [TestCaseSource(nameof(GetCtorTestDtos))]
    public void Ctor_AnyArgument_ReturnsExpectedResult(CtorTestDto testDto)
    {
        // Arrange
        var intervalDto = new IntervalDto(testDto.TestInterval!);

        Interval<IntStruct> interval;

        // Act & Assert
        if (testDto.ExceptionException == null)
        {
            interval = new Interval<IntStruct>(
                intervalDto.Start ?? throw new Exception(),
                intervalDto.End ?? throw new Exception(),
                intervalDto.IsStartIncluded,
                intervalDto.IsEndIncluded);

            Assert.That(interval.Start, Is.EqualTo((IntStruct)testDto.ExpectedInterval!.Start!));
            Assert.That(interval.End, Is.EqualTo((IntStruct)testDto.ExpectedInterval.End!));
            Assert.That(interval.IsStartIncluded, Is.EqualTo(testDto.ExpectedInterval.IsStartIncluded));
            Assert.That(interval.IsEndIncluded, Is.EqualTo(testDto.ExpectedInterval.IsEndIncluded));

            Assert.That(interval.ToString(), Is.EqualTo(testDto.TestInterval));
        }
        else
        {
            Assert.That(testDto.ExpectedInterval, Is.Null);

            var ex = Assert.Throws<ArgumentException>(() =>
                interval = new Interval<IntStruct>(
                    intervalDto.Start ?? throw new Exception(),
                    intervalDto.End ?? throw new Exception(),
                    intervalDto.IsStartIncluded,
                    intervalDto.IsEndIncluded))!;

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Does.StartWith(testDto.ExceptionException.Message));
            Assert.That(ex.ParamName, Is.EqualTo(testDto.ExceptionException.ParamName));
        }
    }

    [Test]
    [TestCaseSource(nameof(GetIsSubsetOfTestDtos))]
    public void IsSubsetOf_ValidArgument_ReturnsExpectedResult(IsSubsetOfTestDto testDto)
    {
        // Arrange
        var testInterval1 = DtoToInterval(testDto.TestInterval1);
        var testInterval2 = DtoToInterval(testDto.TestInterval2);

        // Act
        var result = testInterval1.IsSubsetOf(testInterval2);

        // Assert
        Assert.That(result, Is.EqualTo(testDto.ExpectedResult));
    }

    [Test]
    [TestCaseSource(nameof(GetIntersectWithTestDtos))]
    public void IntersectWith_ValidArgument_ReturnsExpectedResult(IntersectWithTestDto testDto)
    {
        // Arrange
        var testInterval1 = DtoToInterval(testDto.TestInterval1);
        var testInterval2 = DtoToInterval(testDto.TestInterval2);
        var expectedResult = DtoToInterval(testDto.ExpectedResult);

        // Act
        var result1 = testInterval1.IntersectWith(testInterval2);
        var result2 = testInterval2.IntersectWith(testInterval1);

        // Assert
        Assert.That(result1, Is.EqualTo(expectedResult));
        Assert.That(result2, Is.EqualTo(expectedResult));
    }

    [Test]
    [TestCaseSource(nameof(GetContainsTestDtos))]
    public void Contains_ValidArgument_ReturnsExpectedResult(ContainsTestDto testDto)
    {
        // Arrange
        var testInterval = DtoToInterval(testDto.TestInterval!);

        // Act
        var result = testInterval.Contains(testDto.TestValue);

        // Assert
        Assert.That(result, Is.EqualTo(testDto.ExpectedResult));
    }

    [Test]
    [TestCaseSource(nameof(GetIsEmptyTestDtos))]
    public void IsEmpty_ValidArgument_ReturnsExpectedResult(IsEmptyTestDto testDto)
    {
        // Arrange
        var testInterval = DtoToInterval(testDto.TestInterval!);

        // Act
        var result = testInterval.IsEmpty();

        // Assert
        Assert.That(result, Is.EqualTo(testDto.ExpectedResult));
    }

    [Test]
    [TestCaseSource(nameof(GetIsUniversalTestDtos))]
    public void IsUniversal_ValidArgument_ReturnsExpectedResult(IsUniversalTestDto testDto)
    {
        // Arrange
        var testInterval = DtoToInterval(testDto.TestInterval!);

        // Act
        var result = testInterval.IsUniversal();

        // Assert
        Assert.That(result, Is.EqualTo(testDto.ExpectedResult));
    }

    [Test]
    [TestCaseSource(nameof(GetIsSingleValueTestDtos))]
    public void IsSingleValue_ValidArgument_ReturnsExpectedResult(IsSingleValueTestDto testDto)
    {
        // Arrange
        var testInterval = DtoToInterval(testDto.TestInterval!);

        // Act
        var result = testInterval.IsSingleValue(out var dummy);

        // Assert
        Assert.That(result, Is.EqualTo(testDto.ExpectedResult));
    }

    [Test]
    [TestCaseSource(nameof(GetIsInfiniteTestDtos))]
    public void IsInfinite_ValidArgument_ReturnsExpectedResult(IsInfiniteTestDto testDto)
    {
        // Arrange
        var testInterval = DtoToInterval(testDto.TestInterval!);

        // Act
        var result = testInterval.IsInfinite();

        // Assert
        Assert.That(result, Is.EqualTo(testDto.ExpectedResult));
    }

    [Test]
    [TestCaseSource(nameof(GetIsFiniteTestDtos))]
    public void IsFinite_ValidArgument_ReturnsExpectedResult(IsFiniteTestDto testDto)
    {
        // Arrange
        var testInterval = DtoToInterval(testDto.TestInterval!);

        // Act
        var result = testInterval.IsFinite();

        // Assert
        Assert.That(result, Is.EqualTo(testDto.ExpectedResult));
    }

    [Test]
    public void CreateEmpty_NoArguments_CreatesEmptyInterval()
    {
        // Arrange

        // Act
        var interval = Interval<IntStruct>.CreateEmpty();

        // Assert
        Assert.That(interval.Start, Is.EqualTo((IntStruct)0));
        Assert.That(interval.End, Is.EqualTo((IntStruct)0));
        Assert.That(interval.IsStartIncluded, Is.False);
        Assert.That(interval.IsEndIncluded, Is.False);
    }

    [Test]
    public void CreateSingleValue_ValidValue_CreatesSingleValueInterval()
    {
        // Arrange

        // Act
        var interval = Interval<IntStruct>.CreateSingleValue(17);

        // Assert
        Assert.That(interval.Start, Is.EqualTo((IntStruct)17));
        Assert.That(interval.End, Is.EqualTo((IntStruct)17));
        Assert.That(interval.IsStartIncluded, Is.True);
        Assert.That(interval.IsEndIncluded, Is.True);
    }

    #region Test Case Sources

    public static IList<CtorTestDto> GetCtorTestDtos() =>
        Helper.GetCtorTestDtos(false);

    public static IList<IsSubsetOfTestDto> GetIsSubsetOfTestDtos() =>
        Helper.GetIsSubsetOfTestDtos(false);

    public static IList<IntersectWithTestDto> GetIntersectWithTestDtos() =>
        Helper.GetIntersectWithTestDtos(false);

    public static IList<ContainsTestDto> GetContainsTestDtos() =>
        Helper.GetContainsTestDtos(false);

    public static IList<IsEmptyTestDto> GetIsEmptyTestDtos() =>
        Helper.GetIsTestDtos<IsEmptyTestDto>(".IsEmpty.txt", false);

    public static IList<IsUniversalTestDto> GetIsUniversalTestDtos() =>
        Helper.GetIsTestDtos<IsUniversalTestDto>(".IsUniversal.txt", false);

    public static IList<IsSingleValueTestDto> GetIsSingleValueTestDtos() =>
        Helper.GetIsTestDtos<IsSingleValueTestDto>(".IsSingleValue.txt", false);

    public static IList<IsInfiniteTestDto> GetIsInfiniteTestDtos() =>
        Helper.GetIsTestDtos<IsInfiniteTestDto>(".IsInfinite.txt", false);

    public static IList<IsFiniteTestDto> GetIsFiniteTestDtos() =>
        Helper.GetIsTestDtos<IsFiniteTestDto>(".IsFinite.txt", false);

    #endregion

    private static Interval<IntStruct> DtoToInterval(IntervalDto dto)
    {
        return new Interval<IntStruct>(
            dto.Start ?? throw new Exception(),
            dto.End ?? throw new Exception(),
            dto.IsStartIncluded,
            dto.IsEndIncluded);
    }
}
