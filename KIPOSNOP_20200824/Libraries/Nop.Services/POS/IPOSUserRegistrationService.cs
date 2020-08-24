using Nop.Services.Customers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.POS
{
    public interface IPOSUserRegistrationService
    {
        CustomerRegistrationResult RegisterPOSCustomer(CustomerRegistrationRequest request);
    }
}
