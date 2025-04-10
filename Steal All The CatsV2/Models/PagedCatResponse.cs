namespace Steal_All_The_CatsV2.Models
{
    public class PagedCatResponse
    {
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public required List<CatEntity> Items { get; set; }
    }
}
