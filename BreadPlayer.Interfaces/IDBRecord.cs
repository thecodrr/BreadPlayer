namespace BreadPlayer.Interfaces
{
    public interface IDbRecord
    {
        long Id { get; set; }
        string TextSearchKey { get; }

        string GetTextSearchKey();
    }
}