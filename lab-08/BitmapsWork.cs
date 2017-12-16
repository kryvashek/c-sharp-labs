using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace lab08 {
    public class BitmapsWork {
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
    }
}

