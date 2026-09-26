/*namespace Enterprise.DTOs
{
    public class QuickUserAdd
    {
        // --- Required core user fields ---
        public required string Username { get; set; }
        public required string Fullname { get; set; }
        public required string Email { get; set; }
        public required string Activepictureurl { get; set; }
        public required string Role { get; set; }
        public required string Plainpassword { get; set; }

        // --- Address fields ---
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? StateRegion { get; set; }
        public string? Country { get; set; }
        public string? PostalZip { get; set; }

        // --- Contact fields ---
        public string? Phone { get; set; }
        public string? Cellphone { get; set; }

        // --- Mapping helpers ---

        public Models.User ToUser()
        {
            return new Models.User
            {
                Username = this.Username,
                Fullname = this.Fullname,
                Email = this.Email,
                Activepictureurl = this.Activepictureurl,
                Role = this.Role,
                Plainpassword = this.Plainpassword
            };
        }

        public Models.Userprofile ToUserProfile(int userId)
        {
            // Split fullname into first/last
            var names = (Fullname ?? "").Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            string first = names.Length > 0 ? names[0] : "";
            string last = names.Length > 1 ? names[1] : "";

            return new Models.Userprofile
            {
                Userid = userId,
                Useridstring = userId.ToString(),

                Fullname = this.Fullname,
                Firstname = first,
                Lastname = last,

                Address1 = this.Address1,
                Address2 = this.Address2,
                City = this.City,
                Stateregion = this.StateRegion,
                Country = this.Country,
                Postalzip = this.PostalZip,

                Phone = this.Phone,
                Cellphone = this.Cellphone,

                Email = this.Email,
                Activepictureurl = this.Activepictureurl
            };
        }

        public Models.UserPicture ToUserPicture(int userId)
        {
            return new Models.UserPicture
            {
                Userid = userId,
                Useridstring = userId.ToString(),
                Activepictureurl = this.Activepictureurl,
                Somepicture = null
            };
        }

        public Models.CartMaster ToCartMaster(int userId)
        {
            return new Models.CartMaster
            {
                UserId = userId,
                Useridstring = userId.ToString()
            };
        }
    }
}


*/