using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;


namespace MyMusicHeaven.Models
{
    public class AnnouncementEntity : TableEntity
    {
        public AnnouncementEntity(string Artist, string Title)
        {
            this.PartitionKey = Artist;
            this.RowKey = Title;
        }

        public AnnouncementEntity() { }

        [Display(Name = "Announcement Date")]
        [DataType(DataType.DateTime)]

        public DateTime Announcement_Date { get; set; }
        public string Announcement { get; set; }


    }
}
