namespace Lab4.Models.ViewModels
{
    public class NewsViewModel
    {
        public SportClub SportClub { get; set; }
        public IEnumerable<News> News { get; set; }

        /*ublic IEnumerable<Fan> Fans { get; set; }
        public IEnumerable<SportClub> SportClubs { get; set; }
        public IEnumerable<Subscription> Subscriptions { get; set; }*/
    }
}
