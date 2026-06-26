namespace CompanySystemAPI.Models
{
    public class Project
    {
        public int Id { get; set; }

        public string Title { get; set; } = "";

        public string Employee { get; set; } = "";

        public int Progress { get; set; }
    }
}