using System;

namespace lab04 {
    class MainClass {
        public static void Main( string[] args ) {
            Console.WriteLine( "Hello World!" );

            Vector< int >   a = new Vector<int>( 1, 2, 3, 4, 5 ),
                            b = new Vector<int>( 10, 8, 6, 4, 2 );

            foreach( int value in ( a + b ) )
                Console.WriteLine( value );
        }
    }
}
