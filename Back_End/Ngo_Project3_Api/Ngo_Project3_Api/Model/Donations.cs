namespace Ngo_Project3_Api.Model
{
    public class Donations
    {
        public int Id { get; set; }
        public int ProgramId { get; set; }
        public int CauseId { get; set; }
        public int UserId { get; set; }

        public string ProgramName { get; set; }
        public string AccNumber { get; set; }
        public string CauseName { get; set; }
        public string UserName { get; set; }
        public decimal DonationAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
