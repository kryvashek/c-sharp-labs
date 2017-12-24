using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;

namespace lab10 {
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

        public static IEnumerable< XmlReader > ReadEntries( object source, object checkData ) {
            using( XmlReader reader = CreateReader( ref source ) ) {
                CheckerMethod checker = CreateChecker( ref checkData );
                reader.MoveToContent();

                while( reader.Read() )
                    if( XmlNodeType.Element == reader.NodeType && checker( reader.Name ) )
                        yield return reader.ReadSubtree();
            }
        }

        public delegate void ProcessMethod( XmlReader entry );

        public static void ProcessEntries( object source, object checkData, ProcessMethod method ) {
            foreach( XmlReader reader in ReadEntries( source, checkData ) )
                method( reader );
        }
    }
}

