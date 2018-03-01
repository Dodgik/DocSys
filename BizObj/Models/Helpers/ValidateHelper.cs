using System.Text.RegularExpressions;

namespace BizObj
{
    public class ValidateHelper
    {
        public const string RegularEmailHtml = "^([a-z0-9!#$%&'*+/=?^_`(|).~-]+)@((\\\\[[0-9]{1,3}\\\\.[0-9]{1,3}\\\\.[0-9]{1,3}\\\\.)|(([\\\\w-]+\\\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\\\]?)$";
        public const string RegularEmailSc = "^([a-z0-9!#$%&'*+/=?^_`(|).~-]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([\\w-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$";

        private static readonly Regex SRgxEmail = new Regex(RegularEmailSc, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        public static bool ValidateEmail(string email)
        {
            if (email == null) return false;
            if (email.Length == 0) return false;
            if (email.Length > 64) return false;

            else
            {
                Match m = SRgxEmail.Match(email.Trim());
                return m.Success;
            }
        }
    }
}
