using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Microsoft.CSharp;

namespace lab04 {

    public class Vector< T > : IEnumerable< T >
        where T : struct, IEquatable< T >, IComparable< T >, IFormattable {

        T[] _values;

        public Vector( int length ) {
            _values = new T[ length ];
        }

        public Vector( params T[] values ) {
            _values = values;
        }

        public int Length {
            get {
                return _values.GetLength( 0 );
            }
        }

        public IEnumerator< T > GetEnumerator() {
            return _values.Cast<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ( IEnumerator )GetEnumerator();
        }

        public int GetLength() {
            return Length;
        }

        public static Vector< T > operator +( Vector< T > x, Vector< T > y ) {
            if( x.Length != y.Length )
                throw new ArrayTypeMismatchException( "Unequal lengths of summed vectors." );

            Vector< T > z = new Vector<T>( x.Length );

            for( int i = 0; i < x.Length; i++ )
                z._values[ i ] = ( dynamic )x._values[ i ] + ( dynamic )y._values[ i ];

            return z;
        }

        public static Vector< T > operator -( Vector< T > x, Vector< T > y ) {
            if( x.Length != y.Length )
                throw new ArrayTypeMismatchException( "Unequal lengths of subtracted vectors." );

            Vector< T > z = new Vector<T>( x.Length );

            for( int i = 0; i < x.Length; i++ )
                z._values[ i ] = ( dynamic )x._values[ i ] - ( dynamic )y._values[ i ];

            return z;
        }            
    }
}

