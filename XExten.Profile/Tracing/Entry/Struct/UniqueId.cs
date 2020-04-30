using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.Profile.Tracing.Entry.Struct
{
    public struct UniqueId
    {
        public long Part1 { get; set; }
        public long Part2 { get; set; }
        public long Part3 { get; set; }
        public UniqueId(long part1, long part2, long part3)
        {
            Part1 = part1;
            Part2 = part2;
            Part3 = part3;
        }
        public override string ToString() => $"{Part1}.{Part2}.{Part3}";
        public bool Equals(UniqueId other) => Part1 == other.Part1 && Part2 == other.Part2 && Part3 == other.Part3;
        public override bool Equals(object obj) =>obj is UniqueId other && Equals(other);
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Part1.GetHashCode();
                hashCode = (hashCode * 397) ^ Part2.GetHashCode();
                hashCode = (hashCode * 397) ^ Part3.GetHashCode();
                return hashCode;
            }
        }
        public static bool operator ==(UniqueId left, UniqueId right) => left.Equals(right);
        public static bool operator !=(UniqueId left, UniqueId right) => !left.Equals(right);
    }
}
