using System;

namespace BiF.Web.ViewModels.Exchanges
{
    public class ExchangeVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? OpenDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public DateTime? MatchDate { get; set; }
    }
}