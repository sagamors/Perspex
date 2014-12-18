// -----------------------------------------------------------------------
// <copyright file="GridLength.cs" company="Steven Kirk">
// Copyright 2013 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Perspex.Controls
{
    using System;

    public enum GridUnitType
    {
        Auto = 0,
        Pixel = 1,
        Star = 2,
    }

    public struct GridLength : IEquatable<GridLength>
    {
        private static readonly GridLength auto = new GridLength(0, GridUnitType.Auto);

        private static readonly GridLength star = new GridLength(1, GridUnitType.Star);

        private GridUnitType type;

        private double value;

        public GridLength(double value)
            : this(value, GridUnitType.Pixel)
        {
        }

        public GridLength(double value, GridUnitType type)
        {
            if (value < 0 || double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new ArgumentException("Invalid value", "value");
            }

            if (type < GridUnitType.Auto || type > GridUnitType.Star)
            {
                throw new ArgumentException("Invalid value", "type");
            }

            this.type = type;
            this.value = value;
        }

        public static GridLength Auto 
        {
            get { return auto; }
        }

        public static GridLength Star
        {
            get { return star; }
        }

        public GridUnitType GridUnitType
        {
            get { return this.type; }
        }

        public bool IsAbsolute
        {
            get { return this.type == GridUnitType.Pixel; }
        }

        public bool IsAuto
        {
            get { return this.type == GridUnitType.Auto; }
        }

        public bool IsStar 
        {
            get { return this.type == GridUnitType.Star; }
        }

        public double Value 
        {
            get { return this.value; }
        }

        public static bool operator ==(GridLength a, GridLength b)
        {
            return (a.IsAuto && b.IsAuto) || (a.value == b.value && a.type == b.type);
        }

        public static bool operator !=(GridLength gl1, GridLength gl2)
        {
            return !(gl1 == gl2);
        }

        public override bool Equals(object o)
        {
            if (o == null)
            {
                return false;
            }

            if (!(o is GridLength))
            {
                return false;
            }

            return this == (GridLength)o;
        }

        public bool Equals(GridLength gridLength)
        {
            return this == gridLength;
        }

        public override int GetHashCode()
        {
            return this.value.GetHashCode() ^ this.type.GetHashCode();
        }

        public override string ToString()
        {
            if (this.IsAuto)
            {
                return "Auto";
            }

            string s = this.value.ToString();
            return this.IsStar ? s + "*" : s;
        }
    }
}
