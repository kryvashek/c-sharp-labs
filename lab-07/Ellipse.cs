using System;

namespace lab02 {
	public class Ellipse : Shape {
        double	_majorAxis;
        Vertex	_focus;

		public Ellipse ( Vertex focus1, Vertex focus2, double majorAxis ) :
            base( ( focus1 + focus2 ) * 0.5 ) {
            _focus = focus2;
            _majorAxis = majorAxis;

            double  minorAxis = MinorAxis,
                    ratio = ( majorAxis - minorAxis ) / ( majorAxis + minorAxis );

            _area = Math.PI * majorAxis * minorAxis;
            ratio *= 3 * ratio;
            // вторая формула Рамануджана для вычисления периметра эллипса
            _perimeter = Math.PI * ( majorAxis + minorAxis ) * ( 1 + ratio / ( 10 + Math.Sqrt( 4 - ratio ) ) );
            _kind = "Ellipse";
		}

        public double MajorAxis {
            get {
                return _majorAxis;
            }
        }

        public double MinorAxis {
            get {
                double focalDistance = FocalDistance;
                return Math.Sqrt( _majorAxis * _majorAxis - focalDistance * focalDistance );
            }
        }

        public double FocalDistance {
            get {
                return ~( _focus - _position );
            }
        }

        public override Vertex Min {
            get {
                return _position - new Vertex( _majorAxis, MinorAxis );
                //todo change in order to process rotated ellipses (i.e. MajorAxis is collinear with OY)
            }
        }

        public override Vertex Max {
            get {
                return _position + new Vertex( _majorAxis, MinorAxis );
                //todo change in order to process rotated ellipses (i.e. MajorAxis is collinear with OY)
            }
        }

        public override string ParsValue() {
            using( var parsAdder = new Pars.ParsAddder( "E" ) ) {
                return parsAdder.Add( "f1", 2 * _position - _focus )
                                .Add( "f2", _focus )
                                .Add( "ma", _majorAxis ).Finish();
            }
        }

        public override void Draw( Cairo.Context context ) {
            Cairo.Matrix oldMatrix = context.Matrix;
            double  a = MinorAxis / _majorAxis;

            context.Translate( 0.0, _position.Y * ( 1 - a ) );
            context.Scale( 1.0, a );
            context.MoveTo( _position.X + _majorAxis, _position.Y );
            context.Arc( _position.X, _position.Y, _majorAxis, 0.0, 2 * Math.PI );
            context.ClosePath();
            //todo change in order to process rotated ellipses (i.e. MajorAxis is collinear with OY)
            context.Matrix = oldMatrix;
        }
    }
}

