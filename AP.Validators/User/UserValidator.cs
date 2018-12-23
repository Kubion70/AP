using System.Collections.Generic;
using System.Text.RegularExpressions;
using AP.Entities.Enums;
using Models = AP.Entities.Models;

namespace AP.Validators.User
{
    public static class UserValidator
    {
        public static IEnumerable<string> OnUserCreateValidation(Models.User user)
        {
            var validationErrors = new List<string>();

            // Username
            if(string.IsNullOrWhiteSpace(user.Username))
                validationErrors.Add(CommonResponseMessages.EmptyUsername);

            // Password
            string passwordPattern = @"^(?=.*[A-Z].*[A-Z])(?=.*[!@#$&*])(?=.*[0-9].*[0-9])(?=.*[a-z].*[a-z].*[a-z]).{8,}$";
            if(string.IsNullOrWhiteSpace(user.Password))
                validationErrors.Add(CommonResponseMessages.EmptyPassword);
            else if(!Regex.IsMatch(user.Password, passwordPattern))
                validationErrors.Add(CommonResponseMessages.PasswordTooWeak);

            // Email
            string emailPattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
            if(string.IsNullOrWhiteSpace(user.Email) || !Regex.IsMatch(user.Email, emailPattern))
                validationErrors.Add(CommonResponseMessages.InvalidEmail);

            return validationErrors;
        }
    }
}