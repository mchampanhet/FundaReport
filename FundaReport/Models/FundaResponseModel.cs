namespace FundaReport.Models
{
    public class FundaResponseModel
    {
        public bool IsFailed { get; set; }
        public string Error { get; set; }
        public FundaResponseBaseModel Content { get; set; }
    }
}
