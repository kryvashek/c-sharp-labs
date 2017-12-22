using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

namespace lab08 {
    public class BmpWork {
        static Bitmap LoadBitmap( string fileName ) {
            try {
                using( FileStream fs = new FileStream( fileName, FileMode.Open, FileAccess.Read, FileShare.Read ) )
                    return new Bitmap( fs );
            } catch( Exception e ) {
                Console.WriteLine( "Failed loading image from file " + fileName + ":\n" + e );
                return null;
            }
        }

        static bool SaveBitmap( ref Bitmap bmp, string fileName, ImageFormat format ) {
            try {
                bmp.Save( fileName, format );
                return true;
            } catch( Exception e ) {
                Console.WriteLine( "Failed saving new image file " + fileName + ":\n" + e );
                return false;
            }
        }

        public static byte[][,] CreateImageBytes( int height, int width ) {
            return new byte[ 3 ][,] { new byte[ height, width ], new byte[ height, width ], new byte[ height, width ] };
        }

        public unsafe static byte[][,] LoadImageBytes( string fileName, int threadsToRun = 0 ) {
            Bitmap bmp = LoadBitmap( fileName );

            if( null == bmp )
                return null;

            int width = bmp.Width, height = bmp.Height;
            byte[][,] res = CreateImageBytes( height, width );
            BitmapData bd = bmp.LockBits( new Rectangle( 0, 0, width, height ), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb );

            try {
                if( 2 > threadsToRun ) {
                    int h, w;
                    byte* curpos;

                    Console.WriteLine( "Using 1 thread to load image" );

                    for( h = 0; h < height; h++ ) {
                        curpos = ( ( byte * )bd.Scan0 ) + h * bd.Stride;
                        for( w = 0; w < width; w++ ) {
                            res[ 2 ][ h, w ] = *( curpos++ );
                            res[ 1 ][ h, w ] = *( curpos++ );
                            res[ 0 ][ h, w ] = *( curpos++ );
                        }
                    }
                } else {
                    int step = height / threadsToRun, rest = height % threadsToRun;

                    Console.WriteLine( "Using " + threadsToRun + " threads to load image" );

                    using( CountdownEvent completed = new CountdownEvent( threadsToRun ) ) {
                        for( int num = 0; num < threadsToRun; num++ )
                            ThreadPool.QueueUserWorkItem( new WaitCallback( idx => {
                                int i = ( int )idx;
                                int start = i * step, finish = start + step, h, w;
                                byte* curpos;

                                if( i < rest ) {
                                    start += i;
                                    finish += i + 1;
                                } else {
                                    start += rest;
                                    finish += rest;
                                }

                                Console.WriteLine( "Thread " + i + " loading lines from " + start + " till " + finish );

                                for( h = start; h < finish; h++ ) {
                                    curpos = ( ( byte * )bd.Scan0 ) + h * bd.Stride;
                                    for( w = 0; w < width; w++ ) {
                                        res[ 2 ][ h, w ] = *( curpos++ );
                                        res[ 1 ][ h, w ] = *( curpos++ );
                                        res[ 0 ][ h, w ] = *( curpos++ );
                                    }
                                }

                                completed.Signal();
                            }), num );

                        completed.Wait();
                    }
                }
            } finally {
                bmp.UnlockBits( bd );
            }

            return res;
        }

        public unsafe static bool SaveImageBytes( byte[][,] rgb, string fileName, string format, int threadsToRun = 0 ) {
            if( 3 != rgb.GetLength( 0 ) )
                throw new ArrayTypeMismatchException( "Size of first dimension for passed array must be 3 (RGB components)." );

            if( 2 != rgb[ 0 ].Rank )
                throw new ArrayTypeMismatchException( "Every passed component array should have 2 dimensions." );

            int height = rgb[ 0 ].GetLength( 0 ), width = rgb[ 0 ].GetLength( 1 );
            Bitmap res = new Bitmap( width, height, PixelFormat.Format24bppRgb );
            BitmapData bd = res.LockBits( new Rectangle( 0, 0, width, height ), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb );

            try {
                if( 2 > threadsToRun ) {
                    int h, w;
                    byte* curpos;

                    Console.WriteLine( "Using 1 thread to save image" );

                    for( h = 0; h < height; h++ ) {
                        curpos = ( ( byte * )bd.Scan0 ) + h * bd.Stride;
                        for( w = 0; w < width; w++ ) {
                            *( curpos++ ) = rgb[ 2 ][ h, w ];
                            *( curpos++ ) = rgb[ 1 ][ h, w ];
                            *( curpos++ ) = rgb[ 0 ][ h, w ];
                        }
                    }
                } else {
                    int step = height / threadsToRun, rest = height % threadsToRun;

                    Console.WriteLine( "Using " + threadsToRun + " threads to save image" );

                    using( CountdownEvent completed = new CountdownEvent( threadsToRun ) ) {
                        for( int num = 0; num < threadsToRun; num++ )
                            ThreadPool.QueueUserWorkItem( new WaitCallback( idx => {
                                int i = ( int )idx;
                                int start = i * step, finish = start + step, h, w;
                                byte* curpos;

                                if( i < rest ) {
                                    start += i;
                                    finish += i + 1;
                                } else {
                                    start += rest;
                                    finish += rest;
                                }

                                Console.WriteLine( "Thread " + i + " saving lines from " + start + " till " + finish );

                                for( h = start; h < finish; h++ ) {
                                    curpos = ( ( byte * )bd.Scan0 ) + h * bd.Stride;
                                    for( w = 0; w < width; w++ ) {
                                        *( curpos++ ) = rgb[ 2 ][ h, w ];
                                        *( curpos++ ) = rgb[ 1 ][ h, w ];
                                        *( curpos++ ) = rgb[ 0 ][ h, w ];
                                    }
                                }

                                completed.Signal();
                            }), num );

                        completed.Wait();
                    }
                }
            } finally {
                res.UnlockBits( bd );
            }

            return SaveBitmap( ref res, fileName, Ext2Fmt( format ) );
        }

        public static string Fmt2Ext( ImageFormat format ) {
            if( format.Equals( ImageFormat.Bmp ) ) return "bmp";
            if( format.Equals( ImageFormat.Jpeg ) ) return "jpg";
            if( format.Equals( ImageFormat.Png ) ) return "png";
            return "tiff";
        }

        public static ImageFormat Ext2Fmt( string extension ) {
            switch( extension ) {
                case "bmp": return ImageFormat.Bmp;
                case "jpg": return ImageFormat.Jpeg;
                case "png": return ImageFormat.Png;
                default: return ImageFormat.Tiff;
            }
        }
    }
}

