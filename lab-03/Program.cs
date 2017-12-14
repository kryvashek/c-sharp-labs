using System;

namespace lab03 {
    class MainClass {
        public static void Main( string[] args ) {
            Ratio   a = new Ratio( 3, 5 ),
                    b = new Ratio( 7, 21 ),
                    c = new Ratio( 15, 196 );

            Console.WriteLine( "Has fractions " +
                a + " (" + a.toDouble + "), " +
                b + " (" + b.toDouble + ") and " +
                c + " (" + c.toDouble + ")" );

            c += a;
            b -= a;
            a *= a;

            Console.WriteLine( "Has fractions " +
                a + " (" + a.toDouble + "), " +
                b + " (" + b.toDouble + ") and " +
                c + " (" + c.toDouble + ")" );

            c /= b;
            b = -b * a;
            a = c * b;

            Console.WriteLine( "Has fractions " +
                a + " (" + a.toDouble + "), " +
                b + " (" + b.toDouble + ") and " +
                c + " (" + c.toDouble + ")" );
        }
    }
}
