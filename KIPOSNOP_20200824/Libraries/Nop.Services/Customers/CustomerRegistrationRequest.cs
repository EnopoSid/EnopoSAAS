using Nop.Core.Domain.Customers;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Customer registration request
    /// </summary>
    public class CustomerRegistrationRequest
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="email">Email</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="passwordFormat">Password format</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="isApproved">Is approved</param>
         /*Added by surakshith for IsGuestUser Start*/
        /// <param name="isGuestUser">Is approved</param>
         /*Added by surakshith for IsGuestUser End*/
        public CustomerRegistrationRequest(Customer customer, string email, string username,
            string password,
            PasswordFormat passwordFormat,
            int storeId,
            string mobileNumber,
            /*Added by surakshith for IsGuestUser Start*/
            bool? isGuestUser,
            /*Added by surakshith for IsGuestUser End*/
            bool isApproved = true)
        {
            this.Customer = customer;
            this.Email = email;
            this.Username = username;
            /*Added by sree for mobile Number 08_01_2019 Start*/
            this.MobileNumber = mobileNumber;
            /*Added by sree for mobile Number 08_01_2019 End*/
            this.Password = password;
            this.PasswordFormat = passwordFormat;
            this.StoreId = storeId;
            this.IsApproved = isApproved;
            this.IsGuestUser = isGuestUser;
        }

        /// <summary>
        /// Customer
        /// </summary>
        public Customer Customer { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }


        /*Added by sree for mobile Number 08_01_2019 Start*/
        /// <summary>
        /// Mobile Number
        /// </summary>
        public string MobileNumber { get; set; }
        /*Added by sree for mobile Number 08_01_2019 End*/

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Password format
        /// </summary>
        public PasswordFormat PasswordFormat { get; set; }

        /// <summary>
        /// Store identifier
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// Is approved
        /// </summary>
        public bool IsApproved { get; set; }

        /*Added by surakshith for IsGuestUser Start*/
        /// <summary>
        /// IsGuestUser
        /// </summary>
        public bool? IsGuestUser { get; set; }
        /*Added by surakshith for IsGuestUser End*/
    }
}
