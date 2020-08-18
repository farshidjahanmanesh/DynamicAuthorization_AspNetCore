using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityLearning.Models.ViewModels
{
    public class UserListViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public bool IsActive { get; set; }
        public bool EmailConfirmed { get; set; }

    }
}
