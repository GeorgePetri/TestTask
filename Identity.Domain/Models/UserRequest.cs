using System.ComponentModel.DataAnnotations;

namespace Identity.Domain.Models
{
    public class UserRequest
    {
        [Required] public string FirstName { get; set; }

        [Required] public string LastName { get; set; }

        [Required] public string Password { get; set; }

        [Required] public string Login { get; set; }

        [Required] public string Country { get; set; }

        [Required] public string City { get; set; }

        public string State { get; set; }

        public string Street { get; set; }

        public string StreetNumber { get; set; }

        public string PostalCode { get; set; }

        public string FlatNumber { get; set; }
    }
}