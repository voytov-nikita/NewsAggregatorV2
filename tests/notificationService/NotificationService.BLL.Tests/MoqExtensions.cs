using System.Linq.Expressions;
using FluentAssertions;

// ReSharper disable once CheckNamespace
namespace Moq;

public static class MoqExtensions
{
    public static Expression<Func<T, bool>> IsEqualTo<T>(this T source)
    {
        return destination => IsEqualTo(source, destination);
    }

    private static bool IsEqualTo<T>(T source, T destination)
    {
        try
        {
            source.Should().BeEquivalentTo(destination);
        }
        catch
        {
            return false;
        }

        return true;
    }
}
