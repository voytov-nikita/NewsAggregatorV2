using System;
using System.Linq.Expressions;

using FluentAssertions;
using FluentAssertions.Equivalency;

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

	public static Func<EquivalencyAssertionOptions<T>, EquivalencyAssertionOptions<T>> ConfigureDateTimeComparison<T>()
	{
		return options => options
			//Specified rule for comparison DateTimes, because database has another accuracy for datetime type
			.ConfigureDateTimeComparison();
	}
	public static EquivalencyAssertionOptions<T> ConfigureDateTimeComparison<T>(this EquivalencyAssertionOptions<T> options)
	{
		return options

			.Using<DateTime>(context => context
				.Subject
				.Should()
				.BeCloseTo(context.Expectation, TimeSpan.FromMilliseconds(50)
				)
			)
			.WhenTypeIs<DateTime>();
	}
}
