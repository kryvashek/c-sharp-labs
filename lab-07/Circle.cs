using System;

namespace lab02 {
	public class Circle : Shape {
        double	_radius;

		public Circle ( Vertex center, double radius ) :
		    base( center ) {
			_radius = radius;
			_area = Math.PI * _radius * _radius;
			_perimeter = 2 * Math.PI * _radius;
            _kind = "Circle";
		}

		public double Radius {
			get {
				return _radius;
			}
        }

        public override Vertex Min {
            get {
                return _position - new Vertex( _radius, _radius );
            }
        }

        public override Vertex Max {
            get {
                return _position + new Vertex( _radius, _radius );
            }
        }

        public override string ParsValue() {
            using( var parsAdder = new Pars.ParsAddder( "C" ) ) {
                return parsAdder.Add( "c", _position ).Add( "r", _radius ).Finish();
            }
        }

        public override void Draw( Cairo.Context context ) {
            context.MoveTo( _position.X + _radius, _position.Y );
            context.Arc( _position.X, _position.Y, _radius, 0.0, 2 * Math.PI );
            context.ClosePath();
        }
    }
}

