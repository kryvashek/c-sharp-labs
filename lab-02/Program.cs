using System;
using System.IO;
using System.Collections.Generic;

namespace lab02 {
	class MainClass {
        private static void Creation( List< AbstractShape > shapes ) {
            char letter;

            do {
                Console.Write( "Choose shape type:\n" +
                    "E - ellipse\n" +
                    "C - circle\n" +
                    "P - polygon\n" +
                    "Q - cancel\n" +
                    "S - stop it\n" );
                letter = Char.ToUpper( ( char )Console.Read() );
                Console.WriteLine();

                switch( letter ) {
                    case 'Q' :
                    case 'S' :
                        Console.WriteLine( "Cancelling creation." );
                        break;

                    case 'E' :
                        Console.WriteLine( "Enter first focus position:" );
                        Vertex focus1 = new Vertex( Convert.ToDouble( Console.ReadLine() ), Convert.ToDouble( Console.ReadLine() ) );
                        Console.WriteLine( "Enter second focus position:" );
                        Vertex focus2 = new Vertex( Convert.ToDouble( Console.ReadLine() ), Convert.ToDouble( Console.ReadLine() ) );
                        Console.WriteLine( "Enter major axis length:" );
                        double majorAxis = Convert.ToDouble( Console.ReadLine() );
                        shapes.Add( new Ellipse( focus1, focus2, majorAxis ) );
                        break;

                    case 'C' :
                        Console.WriteLine( "Enter center position:" );
                        Vertex center = new Vertex( Convert.ToDouble( Console.ReadLine() ), Convert.ToDouble( Console.ReadLine() ) );
                        Console.WriteLine( "Enter radius length:" );
                        double radius = Convert.ToDouble( Console.ReadLine() );
                        shapes.Add( new Circle( center, radius ) );
                        break;

                    case 'P' :
                        Console.WriteLine( "Enter points count:" );
                        int count = Convert.ToUInt16( Console.ReadLine() );
                        Vertex[]   points = new Vertex[count];

                        if( 0 >= count )
                            Console.WriteLine( "Impossible points count for polygon" );
                        else
                            while( count-- > 0 ) {
                                Console.WriteLine( "Enter point position:" );
                                points[ count ] = new Vertex( Convert.ToDouble( Console.ReadLine() ), Convert.ToDouble( Console.ReadLine() ) );
                        }

                        shapes.Add( new Polygon( points ) );
                        break;

                    default :
                        Console.WriteLine( "Wrong command entered." );
                        break;
                }
            } while( letter != 'Q' && letter != 'S' );
        }

		public static void Main (string[] args) {
            char letter;
            var shapes = new List< AbstractShape >();
            var parsGetter = new Pars.ParsGetter();
            string path;
            object parsResult;

            parsGetter.Add( "L", delegate ( Pars.ParsGetter.ArgumentsList argList ) {
                var resultList = new List<AbstractShape>( argList.Count );

                foreach( var argument in argList.Values )
                    resultList.Add( argument as AbstractShape );

                return resultList;
            } ).Add( "V", delegate ( Pars.ParsGetter.ArgumentsList argList ) {
                return new Vertex( ( double )argList[ "x" ], ( double )argList[ "y" ] );
            } ).Add( "C", delegate ( Pars.ParsGetter.ArgumentsList argList ) {
                return new Circle( ( Vertex )argList[ "c" ], ( double )argList[ "r" ] );
            } ).Add( "E", delegate ( Pars.ParsGetter.ArgumentsList argList ) {
                return new Ellipse( ( Vertex )argList[ "f1" ], ( Vertex )argList[ "f2" ], ( double )argList[ "ma" ] );
            } ).Add( "P", delegate ( Pars.ParsGetter.ArgumentsList argList ) {
                Vertex[] points = new Vertex[ argList.Count ];
                int index = 0;

                foreach( var point in argList.Values )
                    points[ index++ ] = ( Vertex )point;

                return new Polygon( points );
            } );

            do {
                Console.Write( "Choose action:\n" +
                              "C - create a new shape\n" +
                              "P - print shapes list\n" +
                              "S - save shapes list\n" +
                              "L - load shapes list\n" +
                              "H - print help\n" +
                              "Q - quit\n" +
                              "E - exit\n" +
                              "G - get off\n" );
                letter = Char.ToUpper( ( char ) Console.Read() );
                Console.WriteLine();

                switch( letter ) {
                    case 'Q' :
                    case 'E' :
                    case 'G' :
                        Console.WriteLine( "Escaping from the application." );
                        break;

                    case 'H' :
                        Console.WriteLine( "How can this application help you? Absolutely no chances." );
                        break;

                    case 'P' :
                        if( 0 == shapes.Count )
                            Console.WriteLine( "No shapes in the list." );
                        else
                            foreach( Shape shape in shapes )
                                Console.WriteLine( shape.Description );
                        break;

                    case 'S' :
                        Console.WriteLine( "Enter file name:" );
                        path = Console.ReadLine();

                        using( var parsAdder = new Pars.ParsAddder( "L" ) )
                            File.WriteAllText( path, parsAdder.Add( "s", shapes ).Finish(), System.Text.Encoding.UTF8 );
                        
                        break;

                    case 'L' :
                        Console.WriteLine( "Enter file name:" );
                        path = Console.ReadLine();

                        if( parsGetter.Parse( File.ReadAllText( path, System.Text.Encoding.UTF8 ), out parsResult ) )
                            shapes = parsResult as List< AbstractShape >;
                        else
                            Console.WriteLine( String.Format( "Failed reading {0}", path ) );
                        
                        break;

                    case 'C' :
                        Console.WriteLine( "Started creating new shapes." );
                        Creation( shapes );
                        break;

                    default :
                        Console.WriteLine( "Wrong command entered." );
                        break;
                        
                }
            } while( letter != 'Q' && letter != 'E' && letter != 'G' );
		}
	}
}
