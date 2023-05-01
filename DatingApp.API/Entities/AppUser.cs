using System.ComponentModel.DataAnnotations;
using Microsoft.JSInterop.Infrastructure;

namespace DatingApp.API.Entities
{
    public class AppUser
    {
        public int Id { get; set; } 
        public string UserName { get; set; }
    }
}