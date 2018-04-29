using System;

namespace lab02 {
    public struct Vertex : Pars.IParsable {
        double	_x, _y;

		public Vertex( double x, double y ) {
			_x = x;
			_y = y;
		}

		public Vertex Diff( Vertex other ) {
			Vertex difference;

			difference._x = _x - other._x;
			difference._y = _y - other._y;
			return difference;
		}

		public Vertex Summ( Vertex other ) {
			Vertex summ;

			summ._x = _x + other._x;
			summ._y = _y + other._y;
			return summ;
		}

		public Vertex Product( double factor ) {
			Vertex product;

			product._x = _x * factor;
			product._y = _y * factor;
			return product;
		}

        public Vertex Add( Vertex other ) {
            _x += other._x;
            _y += other._y;
            return this;
        }

        public Vertex Sub( Vertex other ) {
            _x -= other._x;
            _y -= other._y;
            return this;
        }

        public Vertex Minimize( Vertex other ) {
            if( _x > other._x )
                _x = other._x;

            if( _y > other._y )
                _y = other._y;
            
            return this;
        }

        public Vertex Maximize( Vertex other ) {
            if( _x < other._x )
                _x = other._x;

            if( _y < other._y )
                _y = other._y;

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

        public string ParsValue() {
            using( var parsAdder = new Pars.ParsAddder( "V" ) ) {
                return parsAdder.Add( "x", _x ).Add( "y", _y ).Finish();
            }
        }

		public static Vertex operator -( Vertex point1, Vertex point2 ) {
			return point1.Diff( point2 );
		}

		public static Vertex operator +( Vertex point1, Vertex point2 ) {
			return point1.Summ( point2 );
		}

		public static Vertex operator *( double factor, Vertex point ) {
			return point.Product( factor );
		}

		public static Vertex operator *( Vertex point, double factor ) {
			return point.Product( factor );
		}

        public static string operator +( Vertex point, string line ) {
            return point.Literal + line;
        }

        public static string operator +( string line, Vertex point ) {
            return line + point.Literal;
        }

        public static double operator ~( Vertex point ) {
            return point.Norma;
        }

        public static Vertex ChooseMin( Vertex one, Vertex two ) {
            var result = one;

            return result.Minimize( two );
        }

        public static Vertex ChooseMax( Vertex one, Vertex two ) {
            var result = one;

            return result.Maximize( two );
        }

        public static Vertex CreateMin() {
            return new Vertex( Double.MinValue, Double.MinValue );
        }

        public static Vertex CreateMax() {
            return new Vertex( Double.MaxValue, Double.MaxValue );
        }

        public static Vertex Create( double x, double y ) {
            return new Vertex( x, y );
        }
	}
}

