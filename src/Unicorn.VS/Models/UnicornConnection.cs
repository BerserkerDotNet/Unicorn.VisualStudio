using System;

namespace Unicorn.VS.Models
{
    public class UnicornConnection : IEquatable<UnicornConnection>
    {
        public UnicornConnection()
        {
            Id = Guid.NewGuid().ToString();
            ServerUrl = "http://";
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string ServerUrl { get; set; }

        public string Token { get; set; }

        public bool Equals(UnicornConnection other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UnicornConnection)obj);
        }

        public override int GetHashCode()
        {
            return Id?.GetHashCode() ?? 0;
        }

    }
}