namespace FundaReport.Models
{
    public class MakelaarTableModel
    {
        public string Query { get; set; }
        public int NumberOfApiRequests { get; set; }
        public int TotalTimePreparingTable { get; set; }
        public int TotalTimeWaitingOnRateLimit { get; set; }
        public List<MakelaarRowModel> Rows { get; set; }
    }
}
