using Newtonsoft.Json;
using TauCode.Data.Tests.Dto;
using TauCode.Data.Tests.Dto.TestDto;
using TauCode.Extensions;

namespace TauCode.Data.Tests;

internal static class Helper
{
    internal static IList<CtorTestDto> GetCtorTestDtos(bool includeInfinite)
    {
        var json = typeof(Helper).Assembly.GetResourceText(".Ctor.json", true);
        var list = JsonConvert.DeserializeObject<IList<CtorTestDto>>(json);

        if (!includeInfinite)
        {
            list = list
                .Where(x => !x.TestInterval!.Contains("∞"))
                .ToList();
        }

        return list;
    }

    internal static IList<IsSubsetOfTestDto> GetIsSubsetOfTestDtos(bool includeInfinite)
    {
        var lines = typeof(Helper).Assembly.GetResourceLines(".IsSubsetOf.txt", true);

        var list = new List<IsSubsetOfTestDto>();

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i].CutComment().Trim();

            if (line == string.Empty)
            {
                continue;
            }

            if (line.Contains("∞") && !includeInfinite)
            {
                continue;
            }

            string[] parts;
            bool expectedResult;

            if (line.Contains('⊆'))
            {
                parts = line.Split('⊆');
                expectedResult = true;
            }
            else if (line.Contains('⊄'))
            {
                parts = line.Split('⊄');
                expectedResult = false;
            }
            else
            {
                throw new Exception();
            }

            var argument1String = parts.First().Trim();
            var argument2String = parts.Skip(1).Single().Trim();

            var dto = new IsSubsetOfTestDto
            {
                TextLineNumber = i + 1,

                TestInterval1 = new IntervalDto(argument1String),
                TestInterval2 = new IntervalDto(argument2String),
                ExpectedResult = expectedResult,
            };

            list.Add(dto);
        }

        return list;
    }

    internal static IList<IntersectWithTestDto> GetIntersectWithTestDtos(bool includeInfinite)
    {
        var lines = typeof(Helper).Assembly.GetResourceLines(".IntersectWith.txt", true);

        var list = new List<IntersectWithTestDto>();

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i].CutComment().Trim();
            if (line == string.Empty)
            {
                continue;
            }

            if (line.Contains("∞") && !includeInfinite)
            {
                continue;
            }

            var partsOfEquation = line
                .Split('=')
                .Select(x => x.Trim())
                .ToList();

            var arguments = partsOfEquation
                .First()
                .Split('∩');

            var argument1String = arguments.First();
            var argument2String = arguments.Skip(1).Single();

            var resultString = partsOfEquation.Skip(1).Single();

            var intervalDto1 = new IntervalDto(argument1String);
            var intervalDto2 = new IntervalDto(argument2String);
            var resultDto = new IntervalDto(resultString);

            var dto = new IntersectWithTestDto
            {
                TextLineNumber = i + 1,

                TestInterval1 = intervalDto1,
                TestInterval2 = intervalDto2,
                ExpectedResult = resultDto,
            };


            list.Add(dto);
        }

        return list;
    }

    internal static IList<ContainsTestDto> GetContainsTestDtos(bool includeInfinite)
    {
        var lines = typeof(Helper).Assembly.GetResourceLines(".Contains.txt", true);
        var list = new List<ContainsTestDto>();

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i].CutComment().Trim();
            if (line == string.Empty)
            {
                continue;
            }

            if (line.Contains("∞") && !includeInfinite)
            {
                continue;
            }

            string[] parts;
            bool expectedResult;

            if (line.Contains('⊆'))
            {
                parts = line.Split('⊆');
                expectedResult = true;
            }
            else if (line.Contains('⊄'))
            {
                parts = line.Split('⊄');
                expectedResult = false;
            }
            else
            {
                throw new Exception();
            }

            var valueString = parts.First().Trim();
            var intervalString = parts.Skip(1).Single().Trim();

            var value = int.Parse(valueString);
            var intervalDto = new IntervalDto(intervalString);

            var dto = new ContainsTestDto
            {
                TextLineNumber = i + 1,

                TestValue = value,
                TestInterval = intervalDto,
                ExpectedResult = expectedResult,
            };

            list.Add(dto);
        }

        return list;
    }

    internal static IList<T> GetIsTestDtos<T>(string resourceName, bool includeInfinite)
        where T : IsTestDtoBase, new()
    {
        var lines = typeof(Helper).Assembly.GetResourceLines(resourceName, true);
        var list = new List<T>();

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i].CutComment().Trim();
            if (line == string.Empty)
            {
                continue;
            }

            if (line.Contains("∞") && !includeInfinite)
            {
                continue;
            }

            var parts = line
                .Split(':')
                .Select(x => x.Trim())
                .ToArray();

            var expectedResult = bool.Parse(parts.First());
            var intervalDto = new IntervalDto(parts.Skip(1).Single());

            var dto = new T
            {
                TextLineNumber = i + 1,

                TestInterval = intervalDto,
                ExpectedResult = expectedResult,
            };

            list.Add(dto);
        }

        return list;
    }

    internal static string CutComment(this string txt)
    {
        var idx = txt.IndexOf('#');
        if (idx >= 0)
        {
            return txt[..idx];
        }

        return txt;
    }
}
