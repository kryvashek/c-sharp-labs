using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace lab08 {
    public class ToStr {
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

        public static bool ReadArguments( ref string[] args, out string matrixFile, out string inFileName, out string outFormatExt, out string outFileName ) {
            matrixFile = inFileName = outFormatExt = outFileName = "";

            if( 2 > args.Length || 4 < args.Length ) {
                Console.WriteLine( "Wrong parameters count. Usage:\n" +
                    "filter <matrix-file> <input-file> [output-format] [output-file]\n" +
                    "<matrix-file> specifies a file to read convolutional matrix from and has no default value\n" +
                    "<input-file> specifies a file to be filtered and has no default value\n" +
                    "[output-format] specifies a format by its extension and by default is 'bmp'\n" +
                    "[output-file] specifies output file and by default is '<input-file>.<output-format-extension>'" );
                return false;
            }

            matrixFile = args[ 0 ];
            inFileName = args[ 1 ];
            outFormatExt = "bmp";
            outFileName = inFileName + "-filtered." + outFormatExt;

            if( 2 < args.Length )
                outFormatExt = args[ 2 ];

            if( 3 < args.Length )
                outFormatExt = args[ 3 ];

            return true;
        }
            
        public static int Main( string[] args ) {
            string matrixFile, inFileName, outFormatExt, outFileName;

            if( !ReadArguments( ref args, out matrixFile, out inFileName, out outFormatExt, out outFileName ) )
                return -1;

            ConvMatrix mtx = ConvMatrix.ReadFromFile( matrixFile );

            if( null == mtx )
                return -2;

            Console.WriteLine( 
                "Convolutional matrix file: " + matrixFile + "\n" +
                "Input image file: " + inFileName + "\n" +
                "Output format: " + outFormatExt + "\n" +
                "Output image file: " + outFileName + "\n" +
                "Convolutional matrix to apply:\n" + mtx );

            byte[][,] bitmap = BitmapToBytes( LoadBitmap( inFileName ) );
            byte[][,] new_bitmap = CreateRgbBitmap( bitmap[ 0 ].GetLength( 0 ), bitmap[ 0 ].GetLength( 1 ) );
            int c, m, n;

            for( c = 0; c < 3; c++)
                for( m = 0; m < bitmap[ c ].GetLength( 0 ); m++ )
                    for( n = 0; n < bitmap[ c ].GetLength( 1 ); n++ )
                        new_bitmap[ c ][ m, n ] = ( byte )mtx.Apply( ref bitmap[ c ], m, n );

            BytesToBitmap( new_bitmap ).Save( outFileName, ImageFormat.Bmp );
            return 0;
        }
    }
}
