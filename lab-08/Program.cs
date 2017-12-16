using System;
using System.IO;
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
        public static bool ReadArguments( ref string[] args, out string matrixFile, out string inFileName, out string outFormatExt, out string outFileName, out int threadsToRun ) {
            matrixFile = inFileName = outFormatExt = outFileName = "";
            threadsToRun = 1;

            if( 2 > args.Length || 5 < args.Length ) {
                Console.WriteLine( "Wrong parameters count. Usage:\n" +
                    "filter <matrix-file> <input-file> [output-format] [output-file] [threads-to-run]\n" +
                    "<matrix-file> specifies a file to read convolutional matrix from and has no default value\n" +
                    "<input-file> specifies a file to be filtered and has no default value\n" +
                    "[output-format] specifies a format by its extension and by default is 'bmp'\n" +
                    "[output-file] specifies output file and by default is '<input-file>.<output-format-extension>'\n" +
                    "[threads-to-run] specifies count of threads to process image and by default is 1 (singlethreaded)" );
                return false;
            }

            matrixFile = args[ 0 ];
            inFileName = args[ 1 ];
            outFormatExt = "bmp";

            if( 2 < args.Length )
                outFormatExt = args[ 2 ];
            
            outFileName = inFileName + "-filtered." + outFormatExt;
            outFormatExt = outFormatExt.ToLower();

            if( 3 < args.Length )
                outFileName = args[ 3 ];

            if( 4 < args.Length )
                threadsToRun = int.Parse( args[ 4 ].Trim() );

            return true;
        }
            
        public static int Main( string[] args ) {
            string matrixFile, inFileName, outFormatExt, outFileName;
            int threadsToRun;

            if( !ReadArguments( ref args, out matrixFile, out inFileName, out outFormatExt, out outFileName, out threadsToRun ) )
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

            byte[][,] inBmp = BmpWork.LoadImageBytes( inFileName );

            if( null == inBmp )
                return -3;
            
            byte[][,] outBmp = BmpWork.CreateImageBytes( inBmp[ 0 ].GetLength( 0 ), inBmp[ 0 ].GetLength( 1 ) );

            if( !mtx.Apply( inBmp, outBmp, threadsToRun ) )
                return -4;

            if( !BmpWork.SaveImageBytes( ref outBmp, outFileName, outFormatExt ) )
                return -5;
            
            return 0;
        }
    }
}
