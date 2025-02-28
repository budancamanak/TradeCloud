using Ardalis.GuardClauses;

namespace Common.Core.Extensions;

public static class GuardClausesExtensions
{
    public static IGuardClause NonNull(this IGuardClause guardClause, object? input, string message = "",
        Func<Exception>? exceptionCreator = null)
    {
        if (input != null)
        {
            Exception? exception = exceptionCreator?.Invoke();
            throw exception ?? new ArgumentException(message);
        }

        return guardClause;
    }

    public static IGuardClause NullOrZeroLengthArray<T>(this IGuardClause guardClause, T[]? input, string message = "")
    {
        if (input == null)
            throw new ArgumentNullException(message);

        if (input.Length == 0)
            throw new ArgumentOutOfRangeException(message);
        return guardClause;
    }

    public static IGuardClause NullOrZeroLengthArray<T>(this IGuardClause guardClause, IList<T>? input, string message = "")
    {
        if (input == null)
            throw new ArgumentNullException(message);

        if (input.Count == 0)
            throw new ArgumentOutOfRangeException(message);
        return guardClause;
    }

    public static IGuardClause NullDate(this IGuardClause clause, DateTime date)
    {
        if (date == default)
            throw new ArgumentNullException("date", "Datetime can't be null or default");
        return clause;
    }
}