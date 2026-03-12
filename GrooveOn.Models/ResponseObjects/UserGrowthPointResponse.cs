namespace GrooveOn.Model.ResponseObjects
{
    public class UserGrowthPointResponse
    {
        public int Month {get; set;}
        public string Label { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}