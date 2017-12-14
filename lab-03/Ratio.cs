using System;

namespace lab03 {
    public class Ratio {
        int     _numerator;
        uint    _denominator;

        public Ratio( int numerator, uint denominator ) {
            _numerator = numerator;

            if( 0 == denominator )
                throw new ArgumentOutOfRangeException( "Denominator can`t be zero." );
            
            _denominator = denominator;
            simplify();
        }

        public Ratio( int numerator ) {
            _numerator = numerator;
            _denominator = 1;
        }

        public Ratio() {
            _numerator = 1;
            _denominator = 1;
        }

        private Ratio simplify() {
            uint t, a = _denominator, b = ( uint )Math.Abs( _numerator );

            while( b > 0 ) {
                t = a;
                a = b;
                b = t % b;
            }

            _denominator /= a;
            _numerator /= ( int )a;
            return this;
        }

        override public string ToString() {
            return toString;
        }

        public double toDouble {
            get {
                return ( double )_numerator / ( double )_denominator;
            }
        }

        public string toString {
            get {
                return _numerator + "/" + _denominator;
            }
        }

        public static Ratio operator +( Ratio A, Ratio B ) {
            Ratio R = new Ratio( A._numerator * ( int )B._denominator + ( int )A._denominator * B._numerator, A._denominator * B._denominator );
            return R.simplify();
        }

        public static Ratio operator -( Ratio A, Ratio B ) {
            Ratio R = new Ratio( A._numerator * ( int )B._denominator - ( int )A._denominator * B._numerator, A._denominator * B._denominator );
            return R.simplify();
        }

        public static Ratio operator *( Ratio A, Ratio B ) {
            Ratio R = new Ratio( A._numerator * B._numerator, A._denominator * B._denominator );
            return R.simplify();
        }

        public static Ratio operator /( Ratio A, Ratio B ) {
            if( 0 == B._numerator )
                throw new ArgumentOutOfRangeException( "Denominator can`t be zero." );
            
            Ratio R = new Ratio( A._numerator * ( int )B._denominator, A._denominator * ( uint )Math.Abs( B._numerator ) );

            if( 0 > B._numerator )
                R._numerator *= -1;
            
            return R.simplify();
        }

        public static Ratio operator +( Ratio A ) {
            return A;
        }

        public static Ratio operator -( Ratio A ) {
            return new Ratio( -A._numerator, A._denominator );
        }
    }
}

