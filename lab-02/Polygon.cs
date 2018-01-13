using System;
using System.Collections.Generic;

namespace lab02 {
    public class Polygon : Shape {
        List< Point >   _points;

        private void processPointPair( Point point1, Point point2 ) {
            Point   center = ( point2 + point1 ) * 0.5;
            double  width = point2.X - point1.X,
                    area = width * center.Y;

            center.Y /= 2;
            _perimeter += ~( point2 - point1 );
            _area += area;
            _massCenter += area * center;
        }

        public Polygon( params Point[] points ) :
            base( new Point( .0, .0 ) ) {
            _points = new List< Point >();

            foreach( Point point in points ) {
                _position += point;
                _points.Add( point );
            }

            _position *= ( 1.0 / ( double )points.Length );

            for( int i = 1; i < points.Length; i++ )
                processPointPair( points[ i - 1 ], points[ i ] );
            
            processPointPair( points[ points.Length - 1 ], points[ 0 ] );
            _massCenter *= ( 1.0 / _area );
            _area = Math.Abs( _area );
            _kind = "Polygon";
        }
    }
}

