namespace Ngo_Project3_Api.Model
{
    public class Donations
    {
        public int Id { get; set; }
        public int ProgramId { get; set; }
        public int CauseId { get; set; }
        public int UserId { get; set; }
        public decimal DonationAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
