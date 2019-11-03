using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace BiF.DAL.Models
{
    public class Exchange
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Theme { get; set; }
        public string Description { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreatorId { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdaterId { get; set; }
        public DateTime? OpenDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public DateTime? MatchDate { get; set; }
        public DateTime? ShipDate { get; set; }
        public string Carrier { get; set; }
        public string TrackingNo { get; set; }

    }
}
