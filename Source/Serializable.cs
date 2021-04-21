////////////////////////////////////////////////////////////////////////////////
// Serializable.cs 
////////////////////////////////////////////////////////////////////////////////
//
// MiCore - Core library for my programs and other libraries.
// Copyright (C) 2021 Michael Furlong <michaeljfurlong@outlook.com>
//
// This program is free software: you can redistribute it and/or modify it 
// under the terms of the GNU General Public License as published by the Free 
// Software Foundation, either version 3 of the License, or (at your option) 
// any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for 
// more details.
// 
// You should have received a copy of the GNU General Public License along with
// this program. If not, see <https://www.gnu.org/licenses/>.
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Text;
using System.Xml;

namespace MiCore
{
	/// <summary>
	///   Interface for objects that can be serialized/deserialized to/from a file.
	/// </summary>
	/// <typeparam name="ReadT">
	///   The stream reader type used to deserialize the object from a stream.
	/// </typeparam>
	/// <typeparam name="WriteT">
	///   The stream writer type used to serialize the object to a stream.
	/// </typeparam>
	public interface ISerializable<ReadT, WriteT>
	{
		/// <summary>
		///   Attempts to deserialize the object from the stream.
		/// </summary>
		/// <param name="sr">
		///   Stream reader.
		/// </param>
		/// <returns>
		///   True if deserialization succeeded and false otherwise.
		/// </returns>
		bool LoadFromStream( ReadT sr );
		/// <summary>
		///   Attempts to serialize the object to the stream.
		/// </summary>
		/// <param name="sw">
		///   Stream writer.
		/// </param>
		/// <returns>
		///   True if serialization succeeded and false otherwise.
		/// </returns>
		bool SaveToStream( WriteT sw );
	}

	/// <summary>
	///   Interface for objects that can be serialized/deserialized to/from text files.
	/// </summary>
	public interface ITextSerializable : ISerializable<StreamReader, StreamWriter>
	{ }
	/// <summary>
	///   Interface for objects that are be serialized/deserialized to/from binary files.
	/// </summary>
	public interface IBinarySerializable : ISerializable<BinaryReader, BinaryWriter>
	{ }
	/// <summary>
	///   Interface for objects that are be serialized/deserialized to/from xml files
	///   procedurally.
	/// </summary>
	public interface IXmlSerializable : ISerializable<XmlReader, XmlWriter>
	{ }

	/// <summary>
	///   Interface for objects that can be loaded from an xml element.
	/// </summary>
	public interface IXmlLoadable
	{
		/// <summary>
		///   Attempts to load the object from the xml element.
		/// </summary>
		/// <param name="element">
		///   The xml element.
		/// </param>
		/// <returns>
		///   True if the object was successfully loaded, otherwise false.
		/// </returns>
		bool LoadFromXml( XmlElement element );
	}


	/// <summary>
	///   Base class for objects that are be serialized/deserialized to/from text files.
	/// </summary>
	[Serializable]
	public abstract class TextSerializable : ITextSerializable
	{
		/// <summary>
		///   Attempts to deserialize the object from the stream.
		/// </summary>
		/// <param name="sr">
		///   Stream reader.
		/// </param>
		/// <returns>
		///   True if deserialization succeeded and false otherwise.
		/// </returns>
		public abstract bool LoadFromStream( StreamReader sr );
		/// <summary>
		///   Attempts to serialize the object to the stream.
		/// </summary>
		/// <remarks>
		///   Default implementation attempts to write <see cref="ToString()"/> to stream so either override this or
		///   <see cref="ToString()"/>.
		/// </remarks>
		/// <param name="sw">
		///   Stream writer.
		/// </param>
		/// <returns>
		///   True if serialization succeeded and false otherwise.
		/// </returns>
		public virtual bool SaveToStream( StreamWriter sw )
		{
			if( sw == null )
				return Logger.LogReturn( "Unable to save to a null stream.", false, LogType.Error );

			try
			{
				sw.Write( ToString() );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Unable to save to stream: " + e.Message + ".", false, LogType.Error );
			}

			return true;
		}

		/// <summary>
		///   Returns the object string as it would be written in file.
		/// </summary>
		/// <returns>
		///   The object string as it would be written in file.
		/// </returns>
		public abstract override string ToString();

		/// <summary>
		///   Constructs an object of type T and attempts to deserialize it from file.
		/// </summary>
		/// <typeparam name="T">
		///   The type of object to deserialize.
		/// </typeparam>
		/// <param name="path">
		///   The file path.
		/// </param>
		/// <returns>
		///   A new object of type T deserialzed from file on success, otherwise null.
		/// </returns>
		public static T FromFile<T>( string path ) where T : class, ITextSerializable, new()
		{
			T val = new T();

			try
			{
				using( FileStream str = File.OpenRead( path ) )
				using( StreamReader r = new StreamReader( str ) )
					if( !val.LoadFromStream( r ) )
						return Logger.LogReturn<T>( "Unable to load TextSerializable from stream.", null, LogType.Error );
			}
			catch( Exception e )
			{
				return Logger.LogReturn<T>( "Unable to load from file: " + e.Message + ".", null, LogType.Error );
			}

			return val;
		}
		/// <summary>
		///   Attempts to serialize the object to file.
		/// </summary>
		/// <param name="t">
		///   The object to save to file.
		/// </param>
		/// <param name="path">
		///   The file path.
		/// </param>
		/// <param name="replace">
		///   If a file already exists at <paramref name="path"/>, should it be replaced?
		/// </param>
		/// <returns>
		///   True if <paramref name="path"/> is valid and data is successfully written to file, otherwise false. Also
		///   returns false if a file already exists at <paramref name="path"/> and <paramref name="replace"/> is false.
		/// </returns>
		public static bool ToFile<T>( T t, string path, bool replace = false ) where T : class, ITextSerializable, new()
		{
			if( t == null )
				return Logger.LogReturn( "Unable to save to file: Object is null.", false, LogType.Error );
			if( string.IsNullOrWhiteSpace( path ) )
				return Logger.LogReturn( "Unable to save to file: Path is null, empty or contains only whitespace.", false, LogType.Error );

			try
			{
				if( File.Exists( path ) )
				{
					if( !replace )
						return Logger.LogReturn( "Unable to save to file: File already exists and replace is false.", false, LogType.Error );

					File.Delete( path );
				}

				using( FileStream str = File.OpenWrite( path ) )
				using( StreamWriter r = new StreamWriter( str ) )
					if( !t.SaveToStream( r ) )
						return Logger.LogReturn( "Unable to save to file: Saving to stream failed.", false, LogType.Error );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Unable to save to stream: " + e.Message + ".", false, LogType.Error );
			}

			return true;
		}
	}
	/// <summary>
	///   Base class for objects that are be serialized/deserialized to/from 
	///   binary files.
	/// </summary>
	[Serializable]
	public abstract class BinarySerializable : IBinarySerializable
	{
		/// <summary>
		///   Attempts to deserialize the object from the stream.
		/// </summary>
		/// <param name="sr">
		///   Stream reader.
		/// </param>
		/// <returns>
		///   True if deserialization succeeded and false otherwise.
		/// </returns>
		public abstract bool LoadFromStream( BinaryReader sr );
		/// <summary>
		///   Attempts to serialize the object to the stream.
		/// </summary>
		/// <param name="sw">
		///   Stream writer.
		/// </param>
		/// <returns>
		///   True if serialization succeeded and false otherwise.
		/// </returns>
		public abstract bool SaveToStream( BinaryWriter sw );

		/// <summary>
		///   Constructs an object of type T and attempts to deserialize from file.
		/// </summary>
		/// <typeparam name="T">
		///   The type of object to deserialize.
		/// </typeparam>
		/// <param name="path">
		///   The file path.
		/// </param>
		/// <returns>
		///   A new object of type T deserialzed from file on success, otherwise null.
		/// </returns>
		public static T FromFile<T>( string path ) where T : class, IBinarySerializable, new()
		{
			T val = new T();

			try
			{
				using( FileStream str = File.OpenRead( path ) )
				using( BinaryReader r = new BinaryReader( str ) )
					if( !val.LoadFromStream( r ) )
						return null;
			}
			catch
			{
				return null;
			}

			return val;
		}
		/// <summary>
		///   Attempts to serialize an object to file.
		/// </summary>
		/// <param name="t">
		///   The object to save to file.
		/// </param>
		/// <param name="path">
		///   The file path.
		/// </param>
		/// <param name="replace">
		///   If a file already exists at <paramref name="path"/>, should it be replaced?
		/// </param>
		/// <returns>
		///   True if <paramref name="path"/> is valid and data is successfully written to file, otherwise false. Also
		///   returns false if a file already exists at <paramref name="path"/> and <paramref name="replace"/> is false.
		/// </returns>
		public static bool ToFile<T>( T t, string path, bool replace = false ) where T : class, IBinarySerializable, new()
		{
			if( t == null )
				return Logger.LogReturn( "Unable to save a null object to file.", false, LogType.Error );
			if( path == null )
				return Logger.LogReturn( "Unable to save object to a null file path.", false, LogType.Error );

			try
			{
				if( File.Exists( path ) )
				{
					if( !replace )
						return Logger.LogReturn( "Unable to save to file: File already exists and replace is false.", false, LogType.Error );

					File.Delete( path );
				}

				using( FileStream str = File.OpenWrite( path ) )
				using( BinaryWriter r = new BinaryWriter( str ) )
					if( !t.SaveToStream( r ) )
						return Logger.LogReturn( "Unable to save object to file: Saving to stream failed.", false, LogType.Error );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Unable to save object to file: " + e.Message + ".", false, LogType.Error );
			}

			return true;
		}
	}

	/// <summary>
	///   Base class for objects that are be serialized/deserialized to/from 
	///   xml files procedurally.
	/// </summary>
	[Serializable]
	public abstract class XmlSerializable : IXmlSerializable
	{
		/// <summary>
		///   Attempts to deserialize the object from the stream.
		/// </summary>
		/// <param name="sr">
		///   Stream reader.
		/// </param>
		/// <returns>
		///   True if deserialization succeeded and false otherwise.
		/// </returns>
		public abstract bool LoadFromStream( XmlReader sr );
		/// <summary>
		///   Attempts to serialize the object to the stream.
		/// </summary>
		/// <param name="sw">
		///   Stream writer.
		/// </param>
		/// <returns>
		///   True if serialization succeeded and false otherwise.
		/// </returns>
		public abstract bool SaveToStream( XmlWriter sw );
	}

	/// <summary>
	///   Base class for objects that can be loaded from an xml element.
	/// </summary>
	[Serializable]
	public abstract class XmlLoadable : IXmlLoadable
	{
		/// <summary>
		///   Attempts to load the object from the xml element.
		/// </summary>
		/// <param name="element">
		///   The xml element.
		/// </param>
		/// <returns>
		///   True if the object was successfully loaded and false otherwise.
		/// </returns>
		public abstract bool LoadFromXml( XmlElement element );
		/// <summary>
		///   Attempts to load the object from an xml file.
		/// </summary>
		/// <param name="path">
		///   The path to the xml file.
		/// </param>
		/// <param name="xpath">
		///   Optional xpath expression to select a single node to load from.
		/// </param>
		/// <param name="nsm">
		///   Optional xml namespace manager.
		/// </param>
		/// <returns>
		///   True on success and false on failure.
		/// </returns>
		public bool LoadFromFile( string path, string xpath = null, XmlNamespaceManager nsm = null )
		{
			XmlDocument doc = new XmlDocument();

			try
			{
				doc.Load( path );

				if( string.IsNullOrWhiteSpace( xpath ) )
					xpath = null;

				if( xpath == null && !LoadFromXml( doc.DocumentElement ) )
					return Logger.LogReturn( "Loading XmlLoadable from root element failed.", false, LogType.Error );
				else if( xpath != null )
				{
					if( ( nsm == null && !LoadFromXml( (XmlElement)doc.SelectSingleNode( xpath ) ) ) ||
						( nsm != null && !LoadFromXml( (XmlElement)doc.SelectSingleNode( xpath, nsm ) ) ) )
						return Logger.LogReturn( "Loading XmlLoadable from xpath element failed.", false, LogType.Error );
				}
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Loading XmlLoadable from xml element failed: " + e.Message + ".", false, LogType.Error );
			}

			return true;
		}

		/// <summary>
		///   Converts the object to an xml string.
		/// </summary>
		/// <returns>
		///   Returns the object to an xml string.
		/// </returns>
		public abstract override string ToString();
		/// <summary>
		///   Converts the object to an xml string with the given indentation level.
		/// </summary>
		/// <param name="indent">
		///   Indentation level.
		/// </param>
		/// <returns>
		///   Returns the object to an xml string with the given indentation level.
		/// </returns>
		public string ToString( uint indent )
		{
			return ToString( this, indent );
		}

		/// <summary>
		///   Converts the object to an xml string with optional indentation.
		/// </summary>
		/// <param name="xl">
		///   The object.
		/// </param>
		/// <param name="indent">
		///   Indentation level.
		/// </param>
		/// <returns>
		///   Returns the object to an xml string with the given indentation level.
		/// </returns>
		public static string ToString( IXmlLoadable xl, uint indent = 0 )
		{
			if( xl == null )
				return null;

			string[] lines;
			{
				string data = xl.ToString().Replace( "\r\n", "\n" );
				lines = data.Split( '\n' );
			}

			StringBuilder sb = new StringBuilder();

			for( int i = 0; i < lines.Length; i++ )
			{
				if( i + i < lines.Length )
					sb.AppendLine( lines[ i ].TrimEnd() );
				else
					sb.Append( lines[ i ].TrimEnd() );
			}

			return Xml.Indent( sb.ToString(), indent );
		}

		/// <summary>
		///   Attempts to create a new object from the xml element.
		/// </summary>
		/// <typeparam name="T">
		///   The type of object to load.
		/// </typeparam>
		/// <param name="element">
		///   The xml element.
		/// </param>
		/// <returns>
		///   A valid object of type T loaded from the xml element on success and null on failure.
		/// </returns>
		public static T FromElement<T>( XmlElement element ) where T : class, IXmlLoadable, new()
		{
			if( element == null )
				return Logger.LogReturn<T>( "Unable to load XmlLoadable from a null XmlElement.", null, LogType.Error );

			T val = new T();

			if( !val.LoadFromXml( element ) )
				return Logger.LogReturn<T>( "Failed loading XmlLoadable from XmlElement.", null, LogType.Error );
			
			return val;
		}
		/// <summary>
		///   Attempts to create a new object loaded from xml at the given path.
		/// </summary>
		/// <typeparam name="T">
		///   The type to load.
		/// </typeparam>
		/// <param name="path">
		///   The path of the xml document.
		/// </param>
		/// <param name="xpath">
		///   Optional xpath expression to select a single node to load from.
		/// </param>
		/// <param name="nsm">
		///   Optional xml namespace manager.
		/// </param>
		/// <returns>
		///   A valid object of type T on success and null on failure.
		/// </returns>
		public static T FromFile<T>( string path, string xpath = null, XmlNamespaceManager nsm = null ) where T: class, IXmlLoadable, new()
		{
			T val = new T();

			XmlDocument doc = new XmlDocument();

			try
			{
				doc.Load( path );

				if( string.IsNullOrWhiteSpace( xpath ) )
					xpath = null;

				if( xpath == null && !val.LoadFromXml( doc.DocumentElement ) )
					return Logger.LogReturn<T>( "Loading XmlLoadable from root element failed.", null, LogType.Error );
				else if( xpath != null )
				{
					if( ( nsm == null && !val.LoadFromXml( (XmlElement)doc.SelectSingleNode( xpath ) ) ) ||
						( nsm != null && !val.LoadFromXml( (XmlElement)doc.SelectSingleNode( xpath, nsm ) ) ) )
						return Logger.LogReturn<T>( "Loading XmlLoadable from xpath element failed.", null, LogType.Error );
				}
			}
			catch( Exception e )
			{
				return Logger.LogReturn<T>( "Loading XmlLoadable from xml element failed: " + e.Message + ".", null, LogType.Error );
			}

			return val;
		}
		/// <summary>
		///   Attempts to create a new object from a string.
		/// </summary>
		/// <typeparam name="T">
		///   The type to load.
		/// </typeparam>
		/// <param name="xml">
		///   The xml string.
		/// </param>
		/// <param name="xpath">
		///   Optional xpath expression to select a single node to load from.
		/// </param>
		/// <param name="nsm">
		///   Optional xml namespace manager.
		/// </param>
		/// <returns>
		///   A valid object of type T on success and null on failure.
		/// </returns>
		public static T FromXml<T>( string xml, string xpath = null, XmlNamespaceManager nsm = null ) where T : class, IXmlLoadable, new()
		{
			T val = new T();

			XmlDocument doc = new XmlDocument();

			try
			{
				doc.LoadXml( xml );

				if( string.IsNullOrWhiteSpace( xpath ) )
					xpath = null;

				if( xpath == null && !val.LoadFromXml( doc.DocumentElement ) )
					return Logger.LogReturn<T>( "Loading XmlLoadable from root element failed.", null, LogType.Error );
				else if( xpath != null )
				{
					if( ( nsm == null && !val.LoadFromXml( (XmlElement)doc.SelectSingleNode( xpath ) ) ) ||
						( nsm != null && !val.LoadFromXml( (XmlElement)doc.SelectSingleNode( xpath, nsm ) ) ) )
						return Logger.LogReturn<T>( "Loading XmlLoadable from xpath element failed.", null, LogType.Error );
				}
			}
			catch( Exception e )
			{
				return Logger.LogReturn<T>( "Loading XmlLoadable from xml element failed: " + e.Message + ".", null, LogType.Error );
			}

			return val;
		}

		/// <summary>
		///   Attempts to save XmlLoadable object to file.
		/// </summary>
		/// <remarks>
		///   Please note <see cref="Xml.Header"/> will be written at the beginning of the file
		///   before the file data.
		/// </remarks>
		/// <param name="x">
		///   The object to save.
		/// </param>
		/// <param name="path">
		///   The path to save the object to.
		/// </param>
		/// <param name="overwrite">
		///   If an already existing file should be overwritten.
		/// </param>
		/// <returns>
		///   True if the object was written to file successfully, otherwise false.
		/// </returns>
		public static bool ToFile( IXmlLoadable x, string path, bool overwrite = false )
		{
			if( x == null )
				return Logger.LogReturn( "Unable to save null XmlLoadable to file.", false, LogType.Error );

			if( path != null && File.Exists( path ) && !overwrite )
				return false;

			try
			{
				File.WriteAllText( path, Xml.Header + "\r\n" + x.ToString() );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Unable to save XmlLoadable to file: " + e.Message + ".", false, LogType.Error );
			}

			return true;
		}
	}
}
