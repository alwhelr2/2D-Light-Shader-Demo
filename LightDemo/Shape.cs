using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LightDemo
{

    public abstract class Shape
    {
        protected VertexPositionColor[] vPColors;

        public Shape(VertexPositionColor[] vPColors)
        {
            this.vPColors = vPColors;
        }

        public abstract void Draw(GraphicsDevice gDevice);
        public abstract Vector4[] Walls { get; }
    }

    public class Square : Shape
    {
        public int[] indices;

        public Square(VertexPositionColor[] vPColors) : base(vPColors)
        {
            indices = new int[] { 0, 1, 2, 0, 2, 3 };    
        }

        public override void Draw(GraphicsDevice gDevice)
        {
            gDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vPColors, 0, 4, indices, 0, 2);
        }

        public override Vector4[] Walls
        {
            get
            {
                return new Vector4[] { new Vector4(vPColors[0].Position.X, vPColors[0].Position.Y, vPColors[1].Position.X, vPColors[1].Position.Y), new Vector4(vPColors[1].Position.X, vPColors[1].Position.Y, vPColors[2].Position.X, vPColors[2].Position.Y), new Vector4(vPColors[2].Position.X, vPColors[2].Position.Y, vPColors[3].Position.X, vPColors[3].Position.Y), new Vector4(vPColors[3].Position.X, vPColors[3].Position.Y, vPColors[0].Position.X, vPColors[0].Position.Y) };
            }
        }
    }

    public class Triangle : Shape
    {
        public Triangle(VertexPositionColor[] vPColors) : base(vPColors) { }

        public override void Draw(GraphicsDevice gDevice)
        {
            gDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vPColors, 0, 1, VertexPositionColor.VertexDeclaration);
        }

        public override Vector4[] Walls
        {
            get
            {
                return new Vector4[] { new Vector4(vPColors[0].Position.X, vPColors[0].Position.Y, vPColors[1].Position.X, vPColors[1].Position.Y), new Vector4(vPColors[1].Position.X, vPColors[1].Position.Y, vPColors[2].Position.X, vPColors[2].Position.Y), new Vector4(vPColors[2].Position.X, vPColors[2].Position.Y, vPColors[0].Position.X, vPColors[0].Position.Y) };
            }
        }
    }
}
