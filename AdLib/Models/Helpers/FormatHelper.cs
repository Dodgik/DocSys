using System;
using System.Text;
using System.Text.RegularExpressions;

namespace AdLib.Models.Helpers
{
    public class FormatHelper
    {
        
        /// <summary>
        /// Форматирует Фамилию, Имя и Отчество в одну строку с полной фамилией та инициалами.
        /// </summary>
        /// <returns>
        /// Объект <see cref="T:System.String"/>, содержащий фамилию та инициалы.
        /// </returns>
        /// <param name="lastName">Фамилия.</param>
        /// <param name="firstName">Имя.</param>
        /// <param name="middleName">Отчество.</param>
        public static string FormatToLastNameAndInitials(string lastName, string firstName, string middleName)
        {
            string completeName = string.Empty;
            if (!String.IsNullOrWhiteSpace(lastName))
            {
                completeName = lastName;
                if (!String.IsNullOrWhiteSpace(firstName))
                {
                    completeName = String.Format("{0} {1}.", completeName, firstName.Substring(0, 1));
                    if (!String.IsNullOrWhiteSpace(middleName))
                        completeName = String.Format("{0}{1}.", completeName, middleName.Substring(0, 1));
                }
            }
            return completeName;
        }

        /// <summary>
        /// Форматирует Фамилию и Имя в одну строку с полной фамилией та инициалами.
        /// </summary>
        /// <returns>
        /// Объект <see cref="T:System.String"/>, содержащий фамилию та инициалы.
        /// </returns>
        /// <param name="lastName">Фамилия.</param>
        /// <param name="firstName">Имя.</param>
        public static string FormatToLastNameAndInitials(string lastName, string firstName)
        {
            return FormatToLastNameAndInitials(lastName, firstName, String.Empty);
        }

        public static string GetLastName(string fullString)
        {
            string result = String.Empty;

            if (!String.IsNullOrWhiteSpace(fullString))
            {
                fullString = fullString.Trim();

                int endLastName = fullString.IndexOf(" ");

                if (endLastName <= 0)
                {
                    for (int i = 0; i < fullString.Length; i++)
                    {
                        string c = fullString[i].ToString();
                        if (String.Equals(c, c.ToUpper()))
                        {
                            endLastName = i;
                            break;
                        }
                    }
                }

                if (endLastName <= 0)
                    result = fullString;
                else
                    result = fullString.Substring(0, endLastName).Trim();
            }

            return result;
        }
        
        public static string GetFirstName(string fullString)
        {
            string result = String.Empty;

            if (!String.IsNullOrWhiteSpace(fullString))
            {
                fullString = fullString.Trim();

                int endLastName = fullString.IndexOf(" ");

                if (endLastName <= 0)
                {
                    for (int i = 0; i < fullString.Length; i++)
                    {
                        string c = fullString[i].ToString();
                        if (String.Equals(c, c.ToUpper()))
                        {
                            endLastName = i;
                            break;
                        }
                    }
                }

                if (endLastName > 0)
                {
                    int endFirstName = fullString.IndexOf(" ", endLastName + 1);
                    if (endFirstName <= 0)
                    {
                        endFirstName = fullString.IndexOf(".", endLastName + 1);
                    }

                    if (endFirstName <= 0)
                        endFirstName = fullString.Length;

                    result = fullString.Substring(endLastName, endFirstName - endLastName).Trim(new[] {' ','.'});
                }
            }

            return result;
        }
        
        public static string GetMiddleName(string fullString)
        {
            string result = String.Empty;

            if (!String.IsNullOrWhiteSpace(fullString))
            {
                fullString = fullString.Trim();

                int endLastName = fullString.IndexOf(" ");

                if (endLastName <= 0)
                {
                    for (int i = 0; i < fullString.Length; i++)
                    {
                        string c = fullString[i].ToString();
                        if (String.Equals(c, c.ToUpper()))
                        {
                            endLastName = i;
                            break;
                        }
                    }
                }

                if (endLastName > 0)
                {
                    int endFirstName = fullString.IndexOf(" ", endLastName + 1);
                    if (endFirstName <= 0)
                    {
                        endFirstName = fullString.IndexOf(".", endLastName + 1);
                    }

                    if (endFirstName <= 0)
                        endFirstName = fullString.Length;

                    result = fullString.Substring(endFirstName).Trim(new[] {' ','.'});
                }
            }

            return result;
        }

        /// <summary>
        /// Форматирует адрес.
        /// </summary>
        /// <returns>
        /// Объект <see cref="T:System.String"/>, содержащий адрес.
        /// </returns>
        /// <param name="address">Адрес</param>
        /// <param name="cityObjectTypeShortName">Короткое название типа городского обьекта</param>
        /// <param name="cityObjectName">Улица</param>
        /// <param name="houseNumber">Номер дома</param>
        /// <param name="corps">Корпус</param>
        /// <param name="apartmentNumber">Номер квартиры</param>
        public static string FormatAddress(string address, string cityObjectTypeShortName, string cityObjectName, string houseNumber, string corps, string apartmentNumber)
        {
            string fullAddress = String.Empty;
                if (!String.IsNullOrWhiteSpace(address))
                    fullAddress = address + " ";
                fullAddress = fullAddress + String.Format("{0}{1} {2}", cityObjectTypeShortName, cityObjectName, houseNumber);
                if (!String.IsNullOrWhiteSpace(corps))
                    fullAddress = fullAddress + String.Format(" корп.{0}", corps);
                if (!String.IsNullOrWhiteSpace(apartmentNumber))
                    fullAddress = fullAddress + String.Format(" кв.{0}", apartmentNumber);

            return fullAddress.Trim();
        }

        public static void ParseAddress(string address, out string cityObjectName, out string houseNumber, out string corps, out string apartmentNumber)
        {
            houseNumber = String.Empty;
            corps = String.Empty;
            apartmentNumber = String.Empty;

            string adrs = address.ToLower().Trim();
            int hnIndex = adrs.IndexOf("буд.");
            int corpsIndex = adrs.IndexOf("корп.");
            int anIndex = adrs.IndexOf("кв.");

            if (hnIndex > 0)
            {
                cityObjectName = adrs.Substring(0, hnIndex).Trim();
            }
            else if (corpsIndex > 0)
            {
                cityObjectName = adrs.Substring(0, corpsIndex).Trim();
            }
            else if (anIndex > 0)
            {
                cityObjectName = adrs.Substring(0, anIndex).Trim();
            }
            else
            {
                cityObjectName = adrs;
            }

            if (hnIndex > 0)
            {
                houseNumber = adrs.Substring(hnIndex + 4).Trim();
                if (houseNumber.IndexOf("корп.") > 0)
                {
                    houseNumber = houseNumber.Substring(0, houseNumber.IndexOf("корп.")).Trim();
                }
                if (houseNumber.IndexOf("кв.") > 0)
                {
                    houseNumber = houseNumber.Substring(0, houseNumber.IndexOf("кв.")).Trim();
                }
            }

            if (corpsIndex > 0)
            {
                corps = adrs.Substring(corpsIndex + 5).Trim();
                if (corps.IndexOf("буд.") > 0)
                {
                    corps = corps.Substring(0, corps.IndexOf("буд.")).Trim();
                }
                if (corps.IndexOf("кв.") > 0)
                {
                    corps = corps.Substring(0, corps.IndexOf("кв.")).Trim();
                }
            }

            if (anIndex > 0)
            {
                apartmentNumber = adrs.Substring(anIndex + 3).Trim();
                if (apartmentNumber.IndexOf("буд.") > 0)
                {
                    apartmentNumber = apartmentNumber.Substring(0, apartmentNumber.IndexOf("буд.")).Trim();
                }
                if (apartmentNumber.IndexOf("корп.") > 0)
                {
                    apartmentNumber = apartmentNumber.Substring(0, apartmentNumber.IndexOf("корп.")).Trim();
                }
            }
        }
        
        public static string Cp866ToUnicode(string str)
        {
            return Regex.Replace(str.Replace('I', 'І').Replace('i', 'і').Replace("Н\n", "").Replace(" n", ""), @"\s+", " ");
        }

        public static string UnicodeToCp866(string str)
        {
            return str.Replace('І', 'I').Replace('і', 'i');
        }

        public string RemoveDuplicateWhiteSpace(string input)
        {
            return Regex.Replace(input, @"[\s]+", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        }

        public static string RemoveWhitespaceWithSplit(string inputString)
        {
            StringBuilder sb = new StringBuilder();

            string[] parts = inputString.Split(new char[] {' ', '\n', '\t', '\r', '\f', '\v'}, StringSplitOptions.RemoveEmptyEntries);

            int size = parts.Length;
            for (int i = 0; i < size; i++)
                sb.AppendFormat("{0} ", parts[i]);

            return sb.ToString();
        }
    }
}
