using System;
using System.Text;
using System.Collections.Generic;

namespace Pars {
    public class Pars {
        public static bool StrIsValid( string text ) {
            char[] specialSymbols = { '[', ']', ';', '=' };
            return -1 == text.IndexOfAny( specialSymbols );
        }
    }

    public interface IParsable {
        string ParsValue();
    }

    public class ParsAddder : IDisposable {
        StringBuilder _builder;

        public ParsAddder( string typeId ) {
            _builder = new StringBuilder( "[" );

            _builder.Append( typeId );
        }

        protected string getParsValue<T>( T fieldValue ) {
            return ( fieldValue is IParsable ? ( fieldValue as IParsable ).ParsValue() : fieldValue.ToString() );
        }

        public ParsAddder Add( string fieldName, object fieldValue ) {
            _builder.AppendFormat( ";{0}={1}", fieldName, getParsValue( fieldValue ) );
            return this;
        }

        public ParsAddder Add<T>( string fieldName, IEnumerable<T> valuesCollection ) {
            int index = 0;

            if( null != valuesCollection )
                foreach( T entry in valuesCollection )
                    _builder.AppendFormat( ";{0}{1}={2}", fieldName, index++, getParsValue( entry ) );

            return this;
        }

        public string Finish() {
            _builder.Append( "]" );
            string result = _builder.ToString();
            _builder.Remove( _builder.Length - 1, 1 );
            return result;
        }

        public void Dispose() {
            _builder.Clear();
        }
    }

    public class ParsGetter : IDisposable {
        public class ArgumentsList : Dictionary<string, object> { }

        public delegate object Creator( ArgumentsList argsList );

        Dictionary<string, Creator> _creators;
        int _pos;
        string _text;
        readonly char[] _valueEnd = { ';', ']' };

        public ParsGetter() {
            _creators = new Dictionary<string, Creator>();
        }

        // routine to read any string
        protected bool getParsStr( bool isFieldName, out string str ) {
            int end;

            if( isFieldName )
                end = _text.IndexOf( '=', _pos );
            else
                end = _text.IndexOfAny( _valueEnd, _pos );

            if( -1 != end ) {
                str = _text.Substring( _pos, end - _pos );

                if( Pars.StrIsValid( str ) ) {
                    _pos = end;
                    return true;
                }
            }

            str = null;
            return false;
        }

        // routine to read 'value' part of 'name=value' pair if it is object ([type,name=value,...])
        protected bool getParsObject( out object val ) {
            string typeName;

            val = null;

            if( '[' == _text[ _pos ] )
                _pos++; // pass '['

            if( !getParsStr( false, out typeName ) )
                return false;

            if( !_creators.ContainsKey( typeName ) )
                return false;

            var argsList = new ArgumentsList();
            string fieldName;
            object fieldValue;

            while( getParsField( out fieldName, out fieldValue ) )
                argsList.Add( fieldName, fieldValue );

            if( ']' == _text[ _pos ] )
                _pos++; // pass ']'

            try {
                val = _creators[ typeName ]( argsList );
            } catch {
                val = null;
                return false;
            }

            return true;
        }

        // routine to read 'value' part of each 'name=value' pair
        protected bool getParsValue( out object val ) {
            double dblValue;

            val = null;

            if( '[' == _text[ _pos ] )
                return getParsObject( out val );

            string value;

            if( !getParsStr( false, out value ) )
                return false;

            if( Double.TryParse( value, out dblValue ) )
                val = dblValue;
            else
                val = value;

            return true;
        }

        // routine to pars 'name=value' pairs
        protected bool getParsField( out string name, out object value ) {
            name = "";
            value = null;

            if( ']' == _text[ _pos ] )
                return false; 

            if( ';' == _text[ _pos ] )
                _pos++; // pass ';'

            if( !getParsStr( true, out name ) )
                return false;

            _pos++; // pass '='

            return getParsValue( out value );
        }

        public ParsGetter Add( string typeId, Creator creator ) {
            _creators.Add( typeId, creator );
            return this;
        }

        public bool Parse( string text, out object result ) {
            _pos = 0;
            _text = text;

            return getParsValue( out result );
        }

        public void Dispose() {
            _creators.Clear();
            _text = "";
        }
    }
}
