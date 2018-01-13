using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;

namespace lab10 {
    public delegate void ProcessMethod( XmlReader entry );
    public delegate void ProcessMethodExtended( XmlReader entry , object extraData );

    public class ProcessXml {
        private static XmlReader CreateReader( ref object source ) {
            if( source is string )
                return XmlReader.Create( source as string );
            else if( source is TextReader )
                return XmlReader.Create( source as TextReader );
            else if( source is Stream )
                return XmlReader.Create( source as Stream );
            else if( source is XElement )
                return ( source as XElement ).CreateReader();
            else if( source is XmlReader )
                return ( source as XmlReader );
            else
                return null;
        }

        private delegate bool CheckerMethod( string name );

        private static CheckerMethod CreateChecker( ref object checkData ) {
            if( checkData is string )
                return ( checkData as string ).Equals;
            else if( checkData is ICollection< string > )
                return ( checkData as ICollection< string > ).Contains;
            else
                return null;
        }

        public static XElement ReadElement( XmlReader reader ) {
            reader.MoveToContent();
            return ( XElement.ReadFrom( reader ) as XElement );
        }

        public static IEnumerable< XmlReader > ReadEntries( XmlReader source, object checkData ) {
            CheckerMethod checker = CreateChecker( ref checkData );
            source.MoveToContent();

            while( source.Read() )
                if( XmlNodeType.Element == source.NodeType && checker( source.LocalName ) )
                    yield return source.ReadSubtree();
        }

        public static IEnumerable< XmlReader > ReadEntries( object source, object checkData ) {
            using( XmlReader reader = CreateReader( ref source ) ) {
                foreach( XmlReader entry in ReadEntries( reader, checkData ) )
                    yield return entry;
            }
        }

        public static void ProcessEntries( object source, object checkData, ProcessMethod method ) {
            foreach( XmlReader entry in ReadEntries( source, checkData ) )
                method( entry );
        }

        public static void ProcessEntries( object source, object checkData, ProcessMethodExtended method, object extraData ) {
            foreach( XmlReader entry in ReadEntries( source, checkData ) )
                method( entry, extraData );
        }
    }
}

