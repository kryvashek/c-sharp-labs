using System;

namespace lab04 {
    class MainClass {
        public delegate void TAction< T >( T value );

        public static void Foreach< T >( Vector< T > v, TAction< T > action ) where T : IEquatable< T >, IFormattable, new() {
            foreach( T value in v )
                action( value );
        }

        public static void PrintOne< T >( T value ) {
            Console.Write( value + " " );
        }

        public static void PrintEach< T >( Vector< T > v ) where T : IEquatable< T >, IFormattable, new() {
            Foreach( v, PrintOne< T > );
            Console.WriteLine();
        }

        public static void Main( string[] args ) {
            Vector< double >   a = new Vector<double>( 1, 2, 3 ),
                               b = new Vector<double>( 10, 8, 6 );

            Console.WriteLine( "Обход по индексам a" );
            for( int i = 0; i < a.Length; i++ )
                Console.Write( a[ i ] + " " );
            Console.WriteLine();
            
            Console.WriteLine( "Обход по индексам b" );
            for( int i = 0; i < b.Length; i++ )
                Console.Write( b[ i ] + " " );
            Console.WriteLine();

            Console.WriteLine( "Сумма" );
            PrintEach( a + b );

            Console.WriteLine( "Разность" );
            PrintEach( b - a );

            Console.WriteLine( "Умножение на скаляр" );
            PrintEach( b * 0.5 );

            Console.WriteLine( "Скалярное произведение" );
            Console.WriteLine( a * b );

            Console.WriteLine( "Векторное произведение a | b" );
            PrintEach( a | b );

            Console.WriteLine( "Смешанное произведение b * (a | b)" );
            Console.WriteLine( b * ( a | b ) );

            Console.WriteLine( "Смешанное произведение (a | b) * a" );
            Console.WriteLine( ( a | b ) * a );

        }
    }
}
