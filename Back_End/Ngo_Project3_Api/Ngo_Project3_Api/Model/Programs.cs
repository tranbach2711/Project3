namespace Ngo_Project3_Api.Model
{
    public class Programs
    {
        public int Id { get; set; }
        public string ProgramName { get; set; }
        public string Depcription { get; set; }
        public int NgoId { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
