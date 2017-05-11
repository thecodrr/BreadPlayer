namespace BreadPlayer.Core.Common
{
    public interface IDbRecord
    {
        long Id { get; set; }
        string GetTextSearchKey();
    }
}
