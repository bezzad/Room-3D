using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace GraphicTools
{
    /// <summary>
    /// Stores a set of four integers that represent the location and size of a rectangle in 3D-Space.
    /// For more advanced region functions, use a System.Drawing.Region object.
    /// </summary>
    public class threeAxisRectangle
    {
        /// <summary>
        /// Initializes a new instance of the threeAxisRectangle class with the
        /// specified location and size.
        /// </summary>
        /// <param name="rightLowerPos">The x & y & z coordinate of the lower-right corner of the rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <param name="depth">The depth of the rectangle.</param>
        public threeAxisRectangle(Microsoft.DirectX.Vector3 rightLowerPos, int width, int height, int depth)
        {
            this.x = rightLowerPos.X;
            this.y = rightLowerPos.Y;
            this.z = rightLowerPos.Z;
            this.Width = width;
            this.Height = height;
            this.Depth = depth;
            _rightLowerPos = rightLowerPos;
        }

        /// <summary>
        /// Gets or sets the width of this threeAxisRectangle structure.
        /// </summary>
        public int Width;
        /// <summary>
        /// Gets or sets the height of this threeAxisRectangle structure.
        /// </summary>
        public int Height;
        /// <summary>
        /// Gets or sets the depth of this threeAxisRectangle structure.
        /// </summary>
        public int Depth;
        
        private float x;
        /// <summary>
        /// Gets or sets the x-coordinate of the lower-right corner of this 
        /// threeAxisRectangle structure.
        /// </summary>
        public float X
        {
            get { return x; }
            set
            {
                x = value;
                _rightLowerPos.X = x;
            }
        }

        private float y;
        /// <summary>
        /// Gets or sets the y-coordinate of the lower-right corner of this 
        /// threeAxisRectangle structure.
        /// </summary>
        public float Y
        {
            get { return y; }
            set
            {
                y = value;
                _rightLowerPos.Y = y;
            }
        }

        private float z;
        /// <summary>
        /// Gets or sets the z-coordinate of the lower-right corner of this 
        /// threeAxisRectangle structure.
        /// </summary>
        public float Z
        {
            get { return z; }
            set
            {
                z = value;
                _rightLowerPos.Z = z;
            }
        }

        /// <summary>
        /// The x & y & z coordinate of the lower-right corner of the rectangle.
        /// </summary>
        private Microsoft.DirectX.Vector3 _rightLowerPos;
        /// <summary>
        /// The x & y & z coordinate of the lower-right corner of the rectangle.
        /// </summary>
        public Microsoft.DirectX.Vector3 RightLowerPos { get { return _rightLowerPos; } } // ReadOnly

        /// <summary>
        /// Tests whether two threeAxisRectangle structures differ in location.
        /// </summary>
        /// <param name="left">The threeAxisRectangle structure that is to the left of the inequality operator.</param>
        /// <param name="right">The threeAxisRectangle structure that is to the right of the inequality operator.</param>
        /// <returns>This operator returns true if any of the threeAxisRectangle.X, threeAxisRectangle.Y,
        /// threeAxisRectangle.Z, threeAxisRectangle.Width, threeAxisRectangle.Height or threeAxisRectangle.Depth
        /// properties of the two threeAxisRectangle structures are unequal; otherwise false.</returns>
        public static bool operator !=(threeAxisRectangle left, threeAxisRectangle right)
        {
            return ((left.X != right.X) || (left.Y != right.Y) || (left.Z != right.Z) ||
                    (left.Width != right.Width) || (left.Height != right.Height) || (left.Depth != right.Depth));
        }
        /// <summary>
        /// Tests whether two threeAxisRectangle structures have equal location.
        /// </summary>
        /// <param name="left">The threeAxisRectangle structure that is to the left of the equality operator.</param>
        /// <param name="right">The threeAxisRectangle structure that is to the right of the equality operator.</param>
        /// <returns>This operator returns true if the two threeAxisRectangle structures
        /// have equal threeAxisRectangle.X, threeAxisRectangle.Y, threeAxisRectangle.Z, 
        /// threeAxisRectangle.Width, threeAxisRectangle.Height and threeAxisRectangle.Depth properties.</returns>
        public static bool operator ==(threeAxisRectangle left, threeAxisRectangle right)
        {
            return ((left.X == right.X) && (left.Y == right.Y) && (left.Z == right.Z) &&
                    (left.Width == right.Width) && (left.Height == right.Height) && (left.Depth == right.Depth));
        }
        /// <summary>
        /// Tests whether obj is a threeAxisRectangle structure with the same location
        /// of this threeAxisRectangle structure.
        /// </summary>
        /// <param name="obj">The System.Object to test.</param>
        /// <returns>This method returns true if obj is a threeAxisRectangle structure and
        /// its threeAxisRectangle.X, threeAxisRectangle.Y, threeAxisRectangle.Width,
        /// and threeAxisRectangle.Height properties are equal to the corresponding
        /// properties of this threeAxisRectangle structure; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is threeAxisRectangle)
            {
                return ((this.X == ((threeAxisRectangle)obj).X) &&
                        (this.Y == ((threeAxisRectangle)obj).Y) &&
                        (this.Z == ((threeAxisRectangle)obj).Z) &&
                        (this.Width == ((threeAxisRectangle)obj).Width) &&
                        (this.Height == ((threeAxisRectangle)obj).Height) &&
                        (this.Depth == ((threeAxisRectangle)obj).Depth));
            }
            else return false;
        }
        /// <summary>
        /// Converts the attributes of this threeAxisRectangle to a human-readable
        /// string.
        /// </summary>
        /// <returns>A string that contains the position, width, height and depth of this threeAxisRectangle
        /// structure ¾ for example, {X=20, Y=20, Z=30, Width=100, Height=50, Depth=90}</returns>
        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.CurrentCulture, 
                                 "{X={0}, Y={1}, Z={2}, Width={3}, Height={4}, Depth={5}}",
                                 this.X, this.Y, this.Z, this.Width, this.Height, this.Depth);
        }
        /// <summary>
        ///  Returns the hash code for this System.Drawing.Rectangle structure. For information
        ///  about the use of hash codes, see System.Object.GetHashCode() .
        /// </summary>
        /// <returns>An integer that represents the hash code for this rectangle.</returns>
    }
}
