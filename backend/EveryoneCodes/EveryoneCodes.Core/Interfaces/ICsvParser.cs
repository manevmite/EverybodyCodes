namespace EveryoneCodes.Core.Interfaces
{
    public interface ICsvParser<T>
    {
        Task<IEnumerable<T>> ParseAsync(Stream stream);
        IEnumerable<T> Parse(TextReader reader);
    }
}
