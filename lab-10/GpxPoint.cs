using System;
using System.Xml;
using System.Xml.Linq;

namespace lab10 {
    public class GpxPoint {
        readonly XElement _entry;
        readonly DateTime _moment;
        readonly double _latitude, _longitude, _altitude, _hdop;
        readonly XNamespace _xmlns;
        double _velocity, _distance;

        public double Latitude { get { return _latitude; } }

        public double Longitude { get { return _longitude; } }

        public double Altitude { get { return _altitude; } }

        public double Hdop { get { return _hdop; } }

        public double Velocity {
            get { return _velocity; }
            set {
                _velocity = value;

                if( null == _entry.Element( _xmlns + "extensions" ) )
                    _entry.Add( new XElement( _xmlns + "extensions" ) );

                _entry.Element( _xmlns + "extensiosn" ).SetElementValue( _xmlns + "velocity", value );
            }
        }

        public double Distance {
            get { return _distance; }
            set {
                _distance = value;

                if( null == _entry.Element( _xmlns + "extensions" ) )
                    _entry.Add( new XElement( _xmlns + "extensions" ) );

                _entry.Element( _xmlns + "extensiosn" ).SetElementValue( _xmlns + "distance", value );
            }
        }

        public GpxPoint( double latitude, double longitude, DateTime moment, double altitude = .0, double hdop = .0 ) {
            _latitude = latitude;
            _longitude = longitude;
            _moment = moment;
            _altitude = altitude;
            _hdop = hdop;
            _xmlns = XNamespace.Get( @"http://www.topografix.com/GPX/1/1" );
            _velocity = .0;
            _distance = .0;
            _entry = new XElement( _xmlns + "trkpt" );
            _entry.SetAttributeValue( _xmlns + "lat", _latitude );
            _entry.SetAttributeValue( _xmlns + "lon", _longitude );
            _entry.SetElementValue( _xmlns + "ele", _altitude );
            _entry.SetElementValue( _xmlns + "hdop", _hdop );
            _entry.SetElementValue( _xmlns + "time", _moment.ToUniversalTime() );
        }

        private bool ParseDouble( string text, out double value ) {
            return double.TryParse( text.Replace( '.', ',' ), out value );
        }

        private double ReadDoubleAttr( XElement entry, XName name ) {
            XAttribute attr = entry.Attribute( name );
            double temp;

            if( null == attr )
                throw new ArgumentOutOfRangeException( "No '" + name + "' attribute passed to GpxPoint in XML entry:\n" + entry );

            if( !ParseDouble( attr.Value, out temp ) )
                throw new ArgumentOutOfRangeException( "Wrong value of '" + name + "' attribute ('" + attr.Value + "') passed to GpxPoint via XML." );
               
            return temp;
        }

        private double ReadDoubleField( XElement entry, XName name ) {
            XElement field = entry.Element( name );
            double temp;

            if( null != field && ParseDouble( field.Value, out temp ) )
                return temp;

            return .0;
        }

        public GpxPoint( XElement entry, XNamespace xmlns ) {
            _entry = entry;
            _xmlns = xmlns;
            _latitude = ReadDoubleAttr( _entry, "lat" );
            _longitude = ReadDoubleAttr( _entry, "lon" );
            _altitude = ReadDoubleField( _entry, _xmlns + "ele" );
            _hdop = ReadDoubleField( _entry, _xmlns + "hdop" );

            XElement timeEntry = _entry.Element( _xmlns + "time" );

            if( null == timeEntry )
                throw new ArgumentOutOfRangeException( "No 'time' element passed to GpxPoint in XML entry:\n" + entry );

            if( !DateTime.TryParse( timeEntry.Value, out _moment ) )
                throw new ArgumentOutOfRangeException( "Wrong value of 'time' element ('" + timeEntry.Value + "') passed to GpxPoint via XML." );
        }

        override public string ToString() {
            return _latitude + ":" + _longitude + ":" + _altitude + " on " + _moment;
        }
    }
}

