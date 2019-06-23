using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakingStorm.Common
{
    public struct Coordinate2D
    {
        public Coordinate2D(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public int X;
        public int Y;

        public IList<Coordinate2D> GetAround()
        {
            IList<Coordinate2D> list = new List<Coordinate2D>();
            list.Add(new Coordinate2D(this.X-1, this.Y-1));
            list.Add(new Coordinate2D(this.X, this.Y-1));
            list.Add(new Coordinate2D(this.X+1, this.Y-1));
            list.Add(new Coordinate2D(this.X-1, this.Y));
            list.Add(new Coordinate2D(this.X+1, this.Y));
            list.Add(new Coordinate2D(this.X-1, this.Y+1));
            list.Add(new Coordinate2D(this.X, this.Y+1));
            list.Add(new Coordinate2D(this.X+1, this.Y+1));
            return list;
        }
    }
}
