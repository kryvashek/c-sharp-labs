using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Microsoft.CSharp;

namespace lab04 {
    public class Vector< T > : IEnumerable< T >, IEquatable< Vector< T > >
        where T : IEquatable< T >, new() {

        T[] _values;

#region Properties

        public int Length {
            get {
                return _values.GetLength( 0 );
            }
        }

        public T this[ int i ] {
            get {
                return _values[ i ];
            }

            set {
                _values[ i ] = value;
            }
        }

#endregion

#region Methods

        public Vector( int length ) {
            _values = new T[ length ];
        }

        public Vector( params T[] values ) {
            _values = values;
        }

        public IEnumerator< T > GetEnumerator() {
            return _values.Cast< T >().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ( IEnumerator )GetEnumerator();
        }

        public int GetLength() {
            return Length;
        }

        public bool Equals( Vector< T > ov ) {
            if( null == ov )
                return false;

            if( Length != ov.Length )
                return false;

            for( int i = 0; i < Length; i++ )
                if( ( dynamic )_values[ i ] != ( dynamic )ov._values[ i ] )
                    return false;

            return true;

        }

        public override bool Equals( Object o ) {
            return Equals( o as Vector< T > );
        }

        public override int GetHashCode() {
            return _values.GetHashCode();
        }

        public static Vector< T > operator +( Vector< T > x, Vector< T > y ) { // summ
            if( x.Length != y.Length )
                throw new ArrayTypeMismatchException( "Unequal lengths of summed vectors." );

            Vector< T > z = new Vector<T>( x.Length );

            for( int i = 0; i < x.Length; i++ )
                z._values[ i ] = ( dynamic )x._values[ i ] + ( dynamic )y._values[ i ];

            return z;
        }

        public static Vector< T > operator -( Vector< T > x, Vector< T > y ) { // substract
            if( x.Length != y.Length )
                throw new ArrayTypeMismatchException( "Unequal lengths of subtracted vectors." );

            Vector< T > z = new Vector<T>( x.Length );

            for( int i = 0; i < x.Length; i++ )
                z._values[ i ] = ( dynamic )x._values[ i ] - ( dynamic )y._values[ i ];

            return z;
        }

        public static T operator *( Vector< T > x, Vector< T > y ) { // scalar product
            if( x.Length != y.Length )
                throw new ArrayTypeMismatchException( "Unequal lengths of subtracted vectors." );

            T r = new T();

            for( int i = 0; i < x.Length; i++ )
                r += ( dynamic )x._values[ i ] * ( dynamic )y._values[ i ];

            return r;
        }

        public static Vector< T > operator *( Vector< T > x, T a ) { // post-coeff
            Vector< T > z = new Vector<T>( x.Length );

            for( int i = 0; i < x.Length; i++ )
                z._values[ i ] = ( dynamic )x._values[ i ] * ( dynamic )a;

            return z;
        }

        public static Vector< T > operator *( T a, Vector< T > x ) { // pre-coeff
            Vector< T > z = new Vector<T>( x.Length );

            for( int i = 0; i < x.Length; i++ )
                z._values[ i ] = ( dynamic )a * ( dynamic )x._values[ i ];

            return z;
        }

        public static Vector< T > operator |( Vector< T > x, Vector< T > y ) { // vector product
            if( 3 != x.Length || 3 != y.Length )
                throw new ArrayTypeMismatchException( "Cross product is available only for 3D." );

            Vector< T > z = new Vector<T>( 3 );

            z._values[ 0 ] = ( dynamic )x._values[ 1 ] * ( dynamic )y._values[ 2 ] - ( dynamic )x._values[ 2 ] * ( dynamic )y._values[ 1 ];
            z._values[ 1 ] = ( dynamic )x._values[ 2 ] * ( dynamic )y._values[ 0 ] - ( dynamic )x._values[ 0 ] * ( dynamic )y._values[ 2 ];
            z._values[ 2 ] = ( dynamic )x._values[ 0 ] * ( dynamic )y._values[ 1 ] - ( dynamic )x._values[ 1 ] * ( dynamic )y._values[ 0 ];

            return z;
        }

        public static bool operator ==( Vector< T > x, Vector< T > y ) { // comparison
            return x.Equals( y );
        }

        public static bool operator !=( Vector< T > x, Vector< T > y ) { // miscomparison
            return !x.Equals( y );
        }

#endregion
    }
}

