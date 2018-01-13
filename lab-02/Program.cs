using System;
using System.Collections.Generic;

namespace lab02 {
	class MainClass {
        private static void Creation( List< Shape > shapes ) {
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
                        Point focus1 = new Point( Convert.ToDouble( Console.ReadLine() ), Convert.ToDouble( Console.ReadLine() ) );
                        Console.WriteLine( "Enter second focus position:" );
                        Point focus2 = new Point( Convert.ToDouble( Console.ReadLine() ), Convert.ToDouble( Console.ReadLine() ) );
                        Console.WriteLine( "Enter major axis length:" );
                        double majorAxis = Convert.ToDouble( Console.ReadLine() );
                        shapes.Add( new Ellipse( focus1, focus2, majorAxis ) );
                        break;

                    case 'C' :
                        Console.WriteLine( "Enter center position:" );
                        Point center = new Point( Convert.ToDouble( Console.ReadLine() ), Convert.ToDouble( Console.ReadLine() ) );
                        Console.WriteLine( "Enter radius length:" );
                        double radius = Convert.ToDouble( Console.ReadLine() );
                        shapes.Add( new Circle( center, radius ) );
                        break;

                    case 'P' :
                        Console.WriteLine( "Enter points count:" );
                        int count = Convert.ToUInt16( Console.ReadLine() );
                        Point[]   points = new Point[count];

                        while( count-- > 0 ) {
                            Console.WriteLine( "Enter point position:" );
                            points[ count ] = new Point( Convert.ToDouble( Console.ReadLine() ), Convert.ToDouble( Console.ReadLine() ) );
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
            List< Shape > shapes = new List< Shape >();

            do {
                Console.Write( "Choose action:\n" +
                    "C - create a new shape\n" +
                    "L - list all created shapes\n" +
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

                    case 'L' :
                        if( 0 == shapes.Count )
                            Console.WriteLine( "No shapes in the list." );
                        else
                            foreach( Shape shape in shapes )
                                Console.WriteLine( shape.Description );
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
