using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Services.POS
{
    public class POSUserRegistrationService: IPOSUserRegistrationService
    {
        private readonly ILocalizationService _localizationService = EngineContext.Current.Resolve<ILocalizationService>();
        private readonly CustomerSettings _customerSettings = EngineContext.Current.Resolve<CustomerSettings>();
        private readonly ICustomerService _customerService = EngineContext.Current.Resolve<ICustomerService>();
        private readonly IEncryptionService _encryptionService = EngineContext.Current.Resolve<IEncryptionService>();
        private readonly RewardPointsSettings _rewardPointsSettings = EngineContext.Current.Resolve<RewardPointsSettings>();
        private readonly IRewardPointService _rewardPointService = EngineContext.Current.Resolve<IRewardPointService>();

        public POSUserRegistrationService()
        {

        }

        /// <summary>
        /// Register customer
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        public virtual CustomerRegistrationResult RegisterPOSCustomer(CustomerRegistrationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Customer == null)
                throw new ArgumentException("Can't load current customer");

            var result = new CustomerRegistrationResult();
            if (request.Customer.IsSearchEngineAccount())
            {
                result.AddError("Search engine can't be registered");
                return result;
            }

            if (request.Customer.IsBackgroundTaskAccount())
            {
                result.AddError("Background task account can't be registered");
                return result;
            }

            if (request.Customer.IsRegistered())
            {
                result.AddError("Current customer is already registered");
                return result;
            }

            if (string.IsNullOrEmpty(request.Email))
            {
                result.AddError(_localizationService.GetResource("Account.Register.Errors.EmailIsNotProvided"));
                return result;
            }

            if (!CommonHelper.IsValidEmail(request.Email))
            {
                result.AddError(_localizationService.GetResource("Common.WrongEmail"));
                return result;
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                result.AddError(_localizationService.GetResource("Account.Register.Errors.PasswordIsNotProvided"));
                return result;
            }

            if (_customerSettings.UsernamesEnabled && string.IsNullOrEmpty(request.Username))
            {
                result.AddError(_localizationService.GetResource("Account.Register.Errors.UsernameIsNotProvided"));
                return result;
            }

            //validate unique user
            if (_customerService.GetCustomerByEmail(request.Email) != null)
            {
                result.AddError(_localizationService.GetResource("Account.Register.Errors.EmailAlreadyExists"));
                return result;
            }

            if (_customerService.GetCustomerByMobileNumber(request.MobileNumber) != null)
            {
                result.AddError("Mobile already exists");
                return result;
            }

            if (_customerSettings.UsernamesEnabled && _customerService.GetCustomerByUsername(request.Username) != null)
            {
                result.AddError(_localizationService.GetResource("Account.Register.Errors.UsernameAlreadyExists"));
                return result;
            }

            //at this point request is valid
            request.Customer.Username = request.Username;
            request.Customer.Email = request.Email;
            /*mobile number*/
            request.Customer.MobileNumber = request.MobileNumber;
            /*mobile number*/

            var customerPassword = new CustomerPassword
            {
                Customer = request.Customer,
                PasswordFormat = request.PasswordFormat,
                CreatedOnUtc = DateTime.UtcNow
            };
            switch (request.PasswordFormat)
            {
                case PasswordFormat.Clear:
                    customerPassword.Password = request.Password;
                    break;
                case PasswordFormat.Encrypted:
                    customerPassword.Password = _encryptionService.EncryptText(request.Password);
                    break;
                case PasswordFormat.Hashed:
                    var saltKey = _encryptionService.CreateSaltKey(NopCustomerServiceDefaults.PasswordSaltKeySize);
                    customerPassword.PasswordSalt = saltKey;
                    customerPassword.Password = _encryptionService.CreatePasswordHash(request.Password, saltKey, _customerSettings.HashedPasswordFormat);
                    break;
            }

            _customerService.InsertCustomerPassword(customerPassword);

            request.Customer.Active = request.IsApproved;

            //add to 'Registered' role
            var registeredRole = _customerService.GetCustomerRoleBySystemName(NopCustomerDefaults.RegisteredRoleName);
            if (registeredRole == null)
                throw new NopException("'Registered' role could not be loaded");
            var posUserRole = _customerService.GetCustomerRoleBySystemName("POSUser");
            request.Customer.CustomerRoles.Add(registeredRole);
            request.Customer.CustomerCustomerRoleMappings.Add(new CustomerCustomerRoleMapping { CustomerRole = registeredRole });
            request.Customer.CustomerCustomerRoleMappings.Add(new CustomerCustomerRoleMapping { CustomerRole = posUserRole });
            //remove from 'Guests' role
            var guestRole = request.Customer.CustomerRoles.FirstOrDefault(cr => cr.SystemName == NopCustomerDefaults.GuestsRoleName);
            if (guestRole != null)
            {
                //request.Customer.CustomerRoles.Remove(guestRole);
                request.Customer.CustomerCustomerRoleMappings
                    .Remove(request.Customer.CustomerCustomerRoleMappings.FirstOrDefault(mapping => mapping.CustomerRoleId == guestRole.Id));
            }

            //add reward points for customer registration (if enabled)
            if (_rewardPointsSettings.Enabled && _rewardPointsSettings.PointsForRegistration > 0)
            {
                var endDate = _rewardPointsSettings.RegistrationPointsValidity > 0
                    ? (DateTime?)DateTime.UtcNow.AddDays(_rewardPointsSettings.RegistrationPointsValidity.Value) : null;
                _rewardPointService.AddRewardPointsHistoryEntry(request.Customer, _rewardPointsSettings.PointsForRegistration,
                    request.StoreId, _localizationService.GetResource("RewardPoints.Message.EarnedForRegistration"), endDate: endDate);
            }

            _customerService.UpdateCustomer(request.Customer);

            return result;
        }
    }
}
