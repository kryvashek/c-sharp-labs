using System;
using System.IO;

namespace lab08 {
    public class ConvMatrix {
        float[,]    _val = new float[,] { { 1 } };
        float       _div = 1;
        int         _halfY = 0, _halfX = 0;

        public float this[ int i, int j ] {
            get { return _val[ i, j ]; }
            set { _val[ i, j ] = value; }
        }

        public float[,] Val {
            get { return _val; }
            set {
                if( 2 != value.Rank || 0 == value.GetLength( 0 ) % 2 || 0 == value.GetLength( 1 ) % 2 )
                    throw new ArrayTypeMismatchException( "Convolutional matrix should have 2 odd dimensions." );

                _val = value;

                _div = 0;
                for( int i = 0; i < _val.GetLength( 0 ); i++ )
                    for( int j = 0; j < _val.GetLength( 1 ); j++ )
                        _div += _val[ i, j ];

                _halfY = value.GetLength( 0 ) / 2;
                _halfX = value.GetLength( 1 ) / 2;
            }
        }

        public ConvMatrix( float[,] source ) {
            Val = source;
        }

        public float Apply( ref byte[,] bitmap, int m, int n ) {
            float   temp = 0;
            float   div_n = _div;
            int     i, j;

            for( i = -_halfY; i <= _halfY; i++ )
                for( j = -_halfX; j <= _halfX; j++ )
                    if( m + i >= 0 && m + i < bitmap.GetLength( 0 ) && n + j >= 0 && n + j < bitmap.GetLength( 1 ) )
                        temp += _val[ _halfY + i, _halfX + j ] * ( float )bitmap[ m + i, n + j ];
                    else
                        div_n -= _val[ _halfY + i, _halfX + j ];

            return ( temp / div_n );
        }

        public override string ToString() {
            return ToStr.ConvertMatrix( _val );
        }

        public static ConvMatrix ReadFromFile( string fileName ) {
            try {
                string[] matrixText = File.ReadAllText( fileName ).Split( new char [] { ' ', '\r', '\n' } );
                int m = int.Parse( matrixText[ 0 ] ), n = int.Parse( matrixText[ 1 ] ), c = 2;
                float[,] temp_mtx = new float [ m, n ];

                for( m = 0; m < temp_mtx.GetLength( 0 ); m++ )
                    for( n = 0; n < temp_mtx.GetLength( 1 ); n++ )
                        temp_mtx[ m, n ] = float.Parse( matrixText[ c++ ] );

                return new ConvMatrix( temp_mtx );
            } catch( Exception e ) {
                Console.WriteLine( "Error reading convolutional matrix from file " + fileName + ": \n" + e );
                return null;
            }
        }
    }
}

