using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace NewNewTry.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the User class
    public class User : IdentityUser
    {
        [PersonalData]
        public string CustomerName { get; set; }

        [PersonalData]
        public int CustomerAge { get; set; }

        [PersonalData]
        public DateTime CustomerDOB { get; set; }

        [PersonalData]
        public string CustomerLivingState { get; set; }
        
        [PersonalData]
        public string UserRole { get; set; }
    }
}
