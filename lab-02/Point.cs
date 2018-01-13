using System;

namespace lab02 {
	public struct Point {
        double	_x, _y;

		public Point( double x, double y ) {
			_x = x;
			_y = y;
		}

		public Point Diff( Point other ) {
			Point difference;

			difference._x = _x - other._x;
			difference._y = _y - other._y;
			return difference;
		}

		public Point Summ( Point other ) {
			Point summ;

			summ._x = _x + other._x;
			summ._y = _y + other._y;
			return summ;
		}

		public Point Product( double factor ) {
			Point product;

			product._x = _x * factor;
			product._y = _y * factor;
			return product;
		}

        public Point Add( Point other ) {
            _x += other._x;
            _y += other._y;
            return this;
        }

        public Point Sub( Point other ) {
            _x -= other._x;
            _y -= other._y;
            return this;
        }

        public double Norma {
            get {
                return Math.Sqrt( _x * _x + _y * _y );
            }
        }

        public string Literal {
            get {
                return "(" + _x + "; " + _y + ")";
            }
        }

        public double X {
            get {
                return _x;
            }

            set {
                _x = value;
            }
        }

        public double Y {
            get {
                return _y;
            }

            set {
                _y = value;
            }
        }

		public static Point operator -( Point point1, Point point2 ) {
			return point1.Diff( point2 );
		}

		public static Point operator +( Point point1, Point point2 ) {
			return point1.Summ( point2 );
		}

		public static Point operator *( double factor, Point point ) {
			return point.Product( factor );
		}

		public static Point operator *( Point point, double factor ) {
			return point.Product( factor );
		}

        public static string operator +( Point point, string line ) {
            return point.Literal + line;
        }

        public static string operator +( string line, Point point ) {
            return line + point.Literal;
        }

        public static double operator ~( Point point ) {
            return point.Norma;
        }
	}
}

