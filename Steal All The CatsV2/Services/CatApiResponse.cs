namespace Steal_All_The_CatsV2.Services
{
    public class CatApiResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int Width { get; set; }
        public int Height { get; set; }

        
        public List<Breed> Breeds { get; set; } = new List<Breed>();
    }
     
    public class Breed
    {
        public string Temperament { get; set; } = string.Empty;
    }
}