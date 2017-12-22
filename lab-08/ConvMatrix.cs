using System;
using System.IO;
using System.Threading;

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

        public bool Apply( byte[][,] input, byte [][,] output, int threadsToRun = 0 ) {
            if( 1 != input.Rank || 1 != output.Rank || 3 != input.Length || 3 != output.Length || 2 != input[ 0 ].Rank || 2 != output[ 0 ].Rank ) {
                Console.WriteLine( "Wrong bitmap arrays passed to convolutional matrix." );
                return false;
            }

            int limY = input[ 0 ].GetLength( 0 ), limX = input[ 0 ].GetLength( 1 );

            if( limY != output[ 0 ].GetLength( 0 ) || limX != output[ 0 ].GetLength( 1 ) ) {
                Console.WriteLine( "Bitmap arrays passed to convolutional matrix are not equal." );
                return false;
            }

            if( 2 > threadsToRun ) {
                int c, m, n;

                for( c = 0; c < 3; c++ )
                    for( m = 0; m < limY; m++ )
                        for( n = 0; n < limX; n++ )
                            output[ c ][ m, n ] = ( byte )Apply( ref input[ c ], m, n );
            } else {
                int step = limY / threadsToRun, rest = limY % threadsToRun;

                Console.WriteLine( "Using " + threadsToRun + " threads to process image" );

                using( CountdownEvent completed = new CountdownEvent( threadsToRun ) ) {
                    for( int num = 0; num < threadsToRun; num++ )
                        ThreadPool.QueueUserWorkItem( new WaitCallback( idx => {
                            int i = ( int )idx;
                            int start = i * step, finish = start + step, c, m, n;

                            if( i < rest ) {
                                start += i;
                                finish += i + 1;
                            } else {
                                start += rest;
                                finish += rest;
                            }

                            Console.WriteLine( "Thread " + i + " processing lines from " + start + " till " + finish );

                            for( c = 0; c < 3; c++ )
                                for( m = start; m < finish; m++ )
                                    for( n = 0; n < limX; n++ )
                                        output[ c ][ m, n ] = ( byte )Apply( ref input[ c ], m, n );

                            completed.Signal();
                        }), num );

                    completed.Wait();
                }
            }

            return true;
        }

        public override string ToString() {
            return ToStr.ConvertMatrix( _val );
        }

        public static ConvMatrix ReadFromFile( string fileName ) {
            try {
                string[] matrixText = File.ReadAllText( fileName ).Split( new char [] { ' ', '\r', '\n', '\t', '\v' } );
                int m = int.Parse( matrixText[ 0 ] ), n = int.Parse( matrixText[ 1 ] ), c = 2;
                float[,] temp_mtx = new float [ m, n ];

                for( m = 0; m < temp_mtx.GetLength( 0 ); m++ )
                    for( n = 0; n < temp_mtx.GetLength( 1 ); n++ ) {
                        while( 0 == matrixText[ c ].Length )
                            c++;
                        
                        temp_mtx[ m, n ] = float.Parse( matrixText[ c++ ].Trim() );
                    }

                return new ConvMatrix( temp_mtx );
            } catch( Exception e ) {
                Console.WriteLine( "Error reading convolutional matrix from file " + fileName + ": \n" + e );
                return null;
            }
        }
    }
}

