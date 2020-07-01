
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace cassie.git.module
{
    public class Signature : IEquatable<Signature>
    {
        // The name of the person.
        public string Name { get; set; }
        // The email address.
        public string Email { get; set; }
        // The time of the signurate.
        public DateTime When { get; set; }

        

        public bool Equals(Signature other)
        {
            if(other == null) return false;
            if(other.Name == this.Name && other.Email == this.Email && other.When == this.When) return true;
            return false;
        }
        public static Signature ParseSignature(string line)
        {
            var emailStart = line.IndexOf("<");
            var emailEnd = line.IndexOf(">");
            var signature = new Signature();
            signature.Name = line.Substring(0,emailStart-1);
            signature.Email = line.Substring(emailStart+1,emailEnd-emailStart-1);
            // Check the date format
            var firstChar = line[emailEnd + 2];

            if(firstChar >= 48 && firstChar <=57) // ASCII code for 0-9
            {
                var emailPath = line.Substring(emailEnd + 2);
                var timestop = emailPath.IndexOf(" ");
                var timestamp = emailPath.Substring(0,timestop);
                var seconds = Convert.ToInt64(timestamp);
                try
                {
                    signature.When = Utils.UnixTimeStampToDateTime(seconds);
                }
                catch (System.Exception)
                {
                    throw;
                }
                return signature;
            }
            try
            {
                var time = line.Substring(emailEnd + 2);
                signature.When = Utils.LongDateToDateTime(time);
            }
            catch (System.Exception)
            {
                throw;
            }
            return signature;
        }
    }

}