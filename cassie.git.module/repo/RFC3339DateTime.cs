using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cassie.git.module.repo
{
    public struct RFC3339DateTime : IEquatable<RFC3339DateTime>, IComparable<RFC3339DateTime>
    {
        private readonly DateTimeOffset _value;

        private static readonly string[] _formats = new string[] { "yyyy-MM-ddTHH:mm:ssK", "yyyy-MM-ddTHH:mm:ss.ffK", "yyyy-MM-ddTHH:mm:ssZ", "yyyy-MM-ddTHH:mm:ss.ffZ" };

        public RFC3339DateTime(string rfc3339FormattedDateTime)
        {
            DateTimeOffset tmp;
            if (!DateTimeOffset.TryParseExact(rfc3339FormattedDateTime, _formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out tmp))
            {
                throw new ArgumentException("Value is not in proper RFC3339 format", "rfc3339FormattedDateTime");
            }
            _value = tmp;
        }

        public static explicit operator RFC3339DateTime(string rfc3339FormattedDateTime)
        {
            return new RFC3339DateTime(rfc3339FormattedDateTime);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is RFC3339DateTime)) return false;
            return this._value.Equals(((RFC3339DateTime)obj)._value);
        }

        public bool Equals(RFC3339DateTime other)
        {
            return this._value.Equals(other._value);
        }

        public static bool operator ==(RFC3339DateTime a, RFC3339DateTime b)
        {
            return a._value == b._value;
        }

        public static bool operator !=(RFC3339DateTime a, RFC3339DateTime b)
        {
            return a._value != b._value;
        }

        public int CompareTo(RFC3339DateTime other)
        {
            return this._value.CompareTo(other._value);
        }
    }
}