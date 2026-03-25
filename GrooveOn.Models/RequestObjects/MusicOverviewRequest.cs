namespace GrooveOn.Model.RequestObjects
{
    public class MusicOverviewRequest
    {

        public string Mode { get; set; } = "year";

        public int UserId { get; set; }

        public int Year { get; set; }

        public int? Month { get; set; }

        public int Take { get; set; } = 4;
    }
}