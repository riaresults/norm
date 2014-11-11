using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace D10.Norm
{
    internal static class ValidationUtils
    {
        public const string EmailAddressRegex = @"^([a-zA-Z0-9_'+*$%\^&!\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9:]{2,4})+$";
        public const string CurrencyRegex = @"(^\$?(?!0,?\d)\d{1,3}(,?\d{3})*(\.\d\d)?)$";
        public const string DateRegex =
            @"^(((0?[1-9]|[12]\d|3[01])[\.\-\/](0?[13578]|1[02])[\.\-\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|((0?[1-9]|[12]\d|30)[\.\-\/](0?[13456789]|1[012])[\.\-\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|((0?[1-9]|1\d|2[0-8])[\.\-\/]0?2[\.\-\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|(29[\.\-\/]0?2[\.\-\/]((1[6-9]|[2-9]\d)?(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)|00|[048])))$";
        public const string NumericRegex = @"\d*";

        /// <summary>
        /// Validates an argument is not null or empty.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        public static void ArgumentNotNullOrEmpty(string value, string parameterName)
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);

            if (value.Length == 0)
                throw new ArgumentException(string.Format("'{0}' cannot be empty.", parameterName), parameterName);
        }

        /// <summary>
        /// Validates an argument is not null or empty or whitespace.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        public static void ArgumentNotNullOrEmptyOrWhitespace(string value, string parameterName)
        {
            ArgumentNotNullOrEmpty(value, parameterName);

            if (StringUtils.IsWhiteSpace(value))
                throw new ArgumentException(string.Format("'{0}' cannot only be whitespace.", parameterName), parameterName);
        }

        /// <summary>
        /// Validates a type argument is an enum.
        /// </summary>
        /// <param name="type">The type argument.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        public static void ArgumentTypeIsEnum(Type type, string parameterName)
        {
            ArgumentNotNull(type, "type");

            if (!type.IsEnum)
                throw new ArgumentException(string.Format("Type {0} is not an Enum.", type), parameterName);
        }

        /// <summary>
        /// Validates a collection argument is not null or empty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        public static void ArgumentNotNullOrEmpty<T>(ICollection<T> collection, string parameterName)
        {
            ArgumentNotNullOrEmpty<T>(collection, parameterName, string.Format("Collection '{0}' cannot be empty.", parameterName));
        }

        /// <summary>
        /// Validates a collection argument is not null or empty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="failureMessage">The failure message.</param>
        public static void ArgumentNotNullOrEmpty<T>(ICollection<T> collection, string parameterName, string failureMessage)
        {
            if (collection == null)
                throw new ArgumentNullException(parameterName);

            if (collection.Count == 0)
                throw new ArgumentException(failureMessage, parameterName);
        }

        /// <summary>
        /// Validates a collection argument is not null or empty.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        public static void ArgumentNotNullOrEmpty(ICollection collection, string parameterName)
        {
            ArgumentNotNullOrEmpty(collection, parameterName, string.Format("Collection '{0}' cannot be empty.", parameterName));
        }

        /// <summary>
        /// Validates a collection argument is not null or empty.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="failureMessage">The failure message.</param>
        public static void ArgumentNotNullOrEmpty(ICollection collection, string parameterName, string failureMessage)
        {
            if (collection == null)
                throw new ArgumentNullException(parameterName);

            if (collection.Count == 0)
                throw new ArgumentException(failureMessage, parameterName);
        }

        /// <summary>
        /// Validates an argument is not null.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        public static void ArgumentNotNull(object value, string parameterName)
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);
        }

        /// <summary>
        /// Validates an argument is not negative.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        public static void ArgumentNotNegative<T>(int value, string parameterName) where T : struct, IComparable<T>
        {
            ArgumentNotNegative<T>(value, parameterName, "Argument cannot be negative.");
        }

        /// <summary>
        /// Validates an argument is not negative.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="failureMessage">The failure message.</param>
        public static void ArgumentNotNegative<T>(int value, string parameterName, string failureMessage) where T : struct, IComparable<T>
        {
            if (value.CompareTo(default(T)) == -1)
                throw new ArgumentOutOfRangeException(parameterName, value, failureMessage);
        }

        /// <summary>
        /// Validates an argument is not zero.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        public static void ArgumentNotZero<T>(T value, string parameterName) where T : struct, IComparable<T>
        {
            ArgumentNotZero<T>(value, parameterName, "Argument cannot be zero.");
        }

        /// <summary>
        /// Validates an argument is not zero.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="failureMessage">The failure message.</param>
        public static void ArgumentNotZero<T>(T value, string parameterName, string failureMessage) where T : struct, IComparable<T>
        {
            if (value.CompareTo(default(T)) == 0)
                throw new ArgumentOutOfRangeException(parameterName, value, failureMessage);
        }

        /// <summary>
        /// Validates an argument is positive.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        public static void ArgumentIsPositive<T>(T value, string parameterName) where T : struct, IComparable<T>
        {
            ArgumentIsPositive<T>(value, parameterName, "Positive number required.");
        }

        /// <summary>
        /// Validates an argument is positive.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="failureMessage">The failure message.</param>
        public static void ArgumentIsPositive<T>(T value, string parameterName, string failureMessage) where T : struct, IComparable<T>
        {
            if (value.CompareTo(default(T)) != 1)
                throw new ArgumentOutOfRangeException(parameterName, value, failureMessage);
        }

        /// <summary>
        /// Validates an object is not disposed.
        /// </summary>
        /// <param name="disposed">A flag indicating whether the object is dispoed.</param>
        /// <param name="objectType">Type of the object.</param>
        public static void ObjectNotDisposed(bool disposed, Type objectType)
        {
            if (disposed)
                throw new ObjectDisposedException(objectType.Name);
        }

        /// <summary>
        /// Validates that an argument condition is true.
        /// </summary>
        /// <param name="condition">The argument condition.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="failureMessage">The failure message.</param>
        public static void ArgumentConditionTrue(bool condition, string parameterName, string failureMessage)
        {
            if (!condition)
                throw new ArgumentException(failureMessage, parameterName);
        }
    }
}
