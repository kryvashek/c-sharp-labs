using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace lab08 {
    class ToStr {
        public static string ConvertMatrix< T >( T[,] matrix ) {
            StringWriter sw = new StringWriter();
            int     i, j;

            for( i = 0; i < matrix.GetLength( 0 ); i++ ) {
                for( j = 0; j < matrix.GetLength( 1 ); j++ )
                    sw.Write( matrix[ i, j ] + " " );
                sw.WriteLine();
            }

            return sw.ToString();
        }
    }

    class ConvMatrix {
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
    }

    class MainClass {
        public static Bitmap LoadBitmap(string fileName) {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                return new Bitmap(fs);
        }

        public static byte[][,] CreateRgbBitmap( int height, int width ) {
            return new byte[ 3 ][,] { new byte[ height, width ], new byte[ height, width ], new byte[ height, width ] };
        }

        public unsafe static byte[][,] BitmapToBytes( Bitmap bmp ) {
            int width = bmp.Width, height = bmp.Height, h, w;
            byte[][,] res = CreateRgbBitmap( height, width );
            BitmapData bd = bmp.LockBits( new Rectangle( 0, 0, width, height ), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb );

            try {
                byte* curpos;
                for( h = 0; h < height; h++ ) {
                    curpos = ( ( byte * )bd.Scan0 ) + h * bd.Stride;
                    for( w = 0; w < width; w++ ) {
                        res[ 2 ][ h, w ] = *( curpos++ );
                        res[ 1 ][ h, w ] = *( curpos++ );
                        res[ 0 ][ h, w ] = *( curpos++ );
                    }
                }
            }
            finally {
                bmp.UnlockBits( bd );
            }
            return res;
        }

        public unsafe static Bitmap BytesToBitmap( byte[][,] rgb ) {
            if( 3 != rgb.GetLength( 0 ) )
                throw new ArrayTypeMismatchException( "Size of first dimension for passed array must be 3 (RGB components)." );
            
            if( 2 != rgb[ 0 ].Rank )
                throw new ArrayTypeMismatchException( "Every passed component array should have 2 dimensions." );

            int height = rgb[ 0 ].GetLength( 0 ), width = rgb[ 0 ].GetLength( 1 ), h, w;
            Bitmap res = new Bitmap( width, height, PixelFormat.Format24bppRgb );
            BitmapData bd = res.LockBits( new Rectangle( 0, 0, width, height ), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb );

            try {
                byte* curpos;
                for( h = 0; h < height; h++ ) {
                    curpos = ( ( byte * )bd.Scan0 ) + h * bd.Stride;
                    for( w = 0; w < width; w++ ) {
                        *( curpos++ ) = rgb[ 2 ][ h, w ];
                        *( curpos++ ) = rgb[ 1 ][ h, w ];
                        *( curpos++ ) = rgb[ 0 ][ h, w ];
                    }
                }
            }
            finally {
                res.UnlockBits( bd );
            }

            return res;
        }
            
        public static void Main( string[] args ) {
            ConvMatrix mtx = new ConvMatrix( new float [,] { 
                { 1, 0.1f, 0.1f, 0, 1 },
                { 0, 0, 0.3f, 0, 0.1f },
                { 0.1f, 0.3f, 0, 0.3f, 0.1f },
                { 0.1f, 0, 0.3f, 0, 0 },
                { 1, 0, 0.1f, 0.1f, 1 } } );

            byte[][,] bitmap = BitmapToBytes( LoadBitmap( "/home/kryvashek/avatar_fullsize.jpg" ) );
            byte[][,] new_bitmap = CreateRgbBitmap( bitmap[ 0 ].GetLength( 0 ), bitmap[ 0 ].GetLength( 1 ) );
            int c, m, n;

            Console.WriteLine( mtx );

            for( c = 0; c < 3; c++)
                for( m = 0; m < bitmap[ c ].GetLength( 0 ); m++ )
                    for( n = 0; n < bitmap[ c ].GetLength( 1 ); n++ )
                        new_bitmap[ c ][ m, n ] = ( byte )mtx.Apply( ref bitmap[ c ], m, n );

            Bitmap result = BytesToBitmap( new_bitmap );

            result.Save( "/home/kryvashek/avatar_fullsize_filtered.bmp", ImageFormat.Bmp );
        }
    }
}
