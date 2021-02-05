using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace VirtualizingWrapPanel.Sample.Models
{
    public struct Position : IComparable<Position>, IComparer<Position>, IEquatable<Position>, IEqualityComparer<Position>
    {
        public int ChapterIndex { get; set; }
        public long ParagraphId { get; set; }
        public long Offset { get; set; }
        public bool IsPagePosition { get; set; }

        public Position(int chapterIndex, long paragraphId, long startOffset)
        {
            ChapterIndex = chapterIndex;
            ParagraphId = paragraphId;
            Offset = startOffset;
            IsPagePosition = false;
        }

        public Position(int page)
        {
            ChapterIndex = 0;
            ParagraphId = 0;
            Offset = page;
            IsPagePosition = true;
        }

        public Position(string pos)
        {
            var data = pos?.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            IsPagePosition = false;

            if (data?.Length != 3)
            {
                data = new[] { "0", "0", int.TryParse(pos, out var page) ? page.ToString() : "0" };
                IsPagePosition = true;
            }

            ChapterIndex = int.TryParse(data[0], out var chapterId) ? chapterId : 0;
            ParagraphId = int.TryParse(data[1], out var paraId) ? paraId : 0;
            Offset = int.TryParse(data[2], out var offset) ? offset : 0;
        }

        #region Implement IComparable<Position>

        public int CompareTo(int chapterIndex, long paragraphId, long offSet)
        {
            if (ChapterIndex > chapterIndex) return 1;
            if (ChapterIndex < chapterIndex) return -1;
            if (ParagraphId > paragraphId) return 1;
            if (ParagraphId < paragraphId) return -1;
            if (Offset > offSet) return 1;
            if (Offset < offSet) return -1;
            return 0;
        }

        public int CompareTo(Position position)
        {
            return CompareTo(position.ChapterIndex, position.ParagraphId, position.Offset);
        }

        #endregion

        #region Implement IComparer<Position>

        public int Compare(Position x, Position y)
        {
            return x.CompareTo(y);
        }

        #endregion

        #region Implement IEquatable<Position>

        public bool Equals(Position other)
        {
            return ChapterIndex == other.ChapterIndex && ParagraphId == other.ParagraphId && Offset == other.Offset;
        }

        public override bool Equals(object obj)
        {
            if (obj?.GetType() != GetType()) return false;
            return Equals((Position)obj);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ChapterIndex;
                hashCode = (hashCode * 397) ^ ParagraphId.GetHashCode();
                hashCode = (hashCode * 397) ^ Offset.GetHashCode();
                return hashCode;
            }
        }

        #endregion

        #region Implement IEqualityComparer<Position>

        public bool Equals(Position x, Position y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(Position obj)
        {
            return obj.GetHashCode();
        }

        #endregion

        public static bool operator <(Position left, Position right)
        {
            return left.CompareTo(right) < 0;
        }
        public static bool operator >(Position left, Position right)
        {
            return left.CompareTo(right) > 0;
        }
        public static bool operator <=(Position left, Position right)
        {
            return left.CompareTo(right) <= 0;
        }
        public static bool operator >=(Position left, Position right)
        {
            return left.CompareTo(right) >= 0;
        }
        public static bool operator ==(Position left, Position right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Position left, Position right)
        {
            return !left.Equals(right);
        }
        public static implicit operator string(Position pos) => pos.ToString();
        public static explicit operator Position(string pos) => new Position(pos);

        public bool IsEmpty() => ChapterIndex == 0 && ParagraphId == 0 && Offset == 0;

        public override string ToString()
        {
            return IsPagePosition ? Offset.ToString() : $"{ChapterIndex}:{ParagraphId}:{Offset}";
        }
    }
}
