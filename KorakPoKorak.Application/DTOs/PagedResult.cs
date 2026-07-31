namespace KorakPoKorak.Application.DTOs
{
    public class PagedResult<T>
    {
        public List<T> Content { get; set; } = new();
        public int TotalElements { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
    }
}
