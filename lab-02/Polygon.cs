using System;
using System.Collections.Generic;

namespace lab02 {
    public class Polygon : Shape {
        List< Vertex >   _points;
        Vertex           _min,
                        _max;

        void processPointPair( Vertex point1, Vertex point2 ) {
            Vertex   center = ( point2 + point1 ) * 0.5;
            double  width = point2.X - point1.X,
                    area = width * center.Y;

            center.Y /= 2;
            _perimeter += ~( point2 - point1 );
            _area += area;
            _massCenter += area * center;
        }

        void initPolygon() {
            _min = Vertex.CreateMax();
            _max = Vertex.CreateMin();

            foreach( Vertex point in _points ) {
                _position += point;
                _min.Minimize( point );
                _max.Maximize( point );
            }

            _position *= ( 1.0 / ( double )_points.Count );

            for( int i = 1; i < _points.Count; i++ )
                processPointPair( _points[ i - 1 ], _points[ i ] );

            processPointPair( _points[ _points.Count - 1 ], _points[ 0 ] );
            _massCenter *= ( 1.0 / _area );
            _area = Math.Abs( _area );
            _kind = "Polygon";
        }

        public Polygon( params Vertex[] points ) :
        base( new Vertex( .0, .0 ) ) {
            _points = new List<Vertex>( points );
            initPolygon();
        }

        public Polygon( IEnumerable<Vertex> points ) :
        base( new Vertex( .0, .0 ) ) {
            _points = new List<Vertex>( points );
            initPolygon();
        }

        public override Vertex Min {
            get {
                return _min;
            }
        }

        public override Vertex Max {
            get {
                return _max;
            }
        }

        public List<Vertex> Points {
            get {
                return _points;
            }
        }

        public override string ParsValue() {
            using( var parsAdder = new Pars.ParsAddder( "P" ) )
                return parsAdder.Add( "v", _points ).Finish();
        }
    }
}

