namespace FundaReport.Models
{
    public class FundaResponseBaseModel
    {
        public MakelaarResponseModel[] Objects { get; set; }
        public FundaResponsePagingModel Paging { get; set; }
        public int TotaalAantalObjecten { get; set; }
    }
}
