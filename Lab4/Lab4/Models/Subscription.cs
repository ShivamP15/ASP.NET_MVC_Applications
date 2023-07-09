namespace Lab4.Models
{
    public class Subscription
    {
        public int FanId { get; set; }

        public string SportClubId { get; set;}

        public Fan fan { get; set;}

        public SportClub sportClub { get; set;}
    }
}
