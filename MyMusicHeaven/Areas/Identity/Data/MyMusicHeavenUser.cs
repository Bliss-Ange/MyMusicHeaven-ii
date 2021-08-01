using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MyMusicHeaven.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the MyMusicHeavenUser class
    public class MyMusicHeavenUser : IdentityUser //modify the table column
    {
        //add extra information such as: name, age, dob, address
        [PersonalData]
        [Required] //check no empty
        public string User_Full_Name { get; set; }

        [PersonalData]
        public int User_Age { get; set; }

        [PersonalData]
        public DateTime User_DOB { get; set; }

        [PersonalData]
        public string User_Address { get; set; }

        public byte[] ProfilePicture { get; set; }
    }
}
