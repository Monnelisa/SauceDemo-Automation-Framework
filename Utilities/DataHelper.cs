using System;

namespace TakealotAutomation.Utilities
{
    /// <summary>
    /// Utility class for string operations and data handling
    /// </summary>
    public class DataHelper
    {
        /// <summary>
        /// Generates random email
        /// </summary>
        public static string GenerateRandomEmail()
        {
            string randomPart = Guid.NewGuid().ToString().Substring(0, 8);
            return $"user_{randomPart}@example.com";
        }

        /// <summary>
        /// Generates random string
        /// </summary>
        public static string GenerateRandomString(int length = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var result = new char[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = chars[random.Next(chars.Length)];
            }

            return new string(result);
        }

        /// <summary>
        /// Generates random phone number
        /// </summary>
        public static string GenerateRandomPhoneNumber()
        {
            var random = new Random();
            return $"072{random.Next(1000000, 9999999)}";
        }

        /// <summary>
        /// Extracts currency value from string
        /// </summary>
        public static decimal ExtractCurrencyValue(string text)
        {
            try
            {
                string numericValue = System.Text.RegularExpressions.Regex.Replace(text, @"[^\d.]", "");
                if (decimal.TryParse(numericValue, out decimal value))
                {
                    return value;
                }
                return 0m;
            }
            catch (Exception)
            {
                return 0m;
            }
        }

        /// <summary>
        /// Checks if string contains only digits
        /// </summary>
        public static bool IsNumeric(string text)
        {
            return !string.IsNullOrEmpty(text) && System.Text.RegularExpressions.Regex.IsMatch(text, @"^\d+$");
        }

        /// <summary>
        /// Truncates string to specified length
        /// </summary>
        public static string Truncate(string text, int maxLength)
        {
            return string.IsNullOrEmpty(text) || text.Length <= maxLength
                ? text
                : text.Substring(0, maxLength) + "...";
        }
    }
}
