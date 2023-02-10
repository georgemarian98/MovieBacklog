using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBacklog.Models.ViewModels
{
    public class UsersViewModel
    {
        private static readonly List<string> users = new()
        {
            "George",
            "Rebeka"
        };

        public string CurrentUser { get; set; } = "Rebeka";

        public List<SelectListItem> AllUsers;

        public UsersViewModel()
        {
            AllUsers = new List<SelectListItem>();
            foreach (var user in users) AllUsers.Add(new SelectListItem { Text = user, Value = user });
        }
    }
}
