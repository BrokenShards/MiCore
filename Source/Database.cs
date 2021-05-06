////////////////////////////////////////////////////////////////////////////////
// Database.cs 
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MiCore
{
	/// <summary>
	///   Interface for databases.
	/// </summary>
	/// <typeparam name="T">
	///   Data type.
	/// </typeparam>
	/// <typeparam name="ReadT">
	///   The type used to deserialize from a stream.
	/// </typeparam>
	/// <typeparam name="WriteT">
	///   The type used to serialize to a stream.
	/// </typeparam>
	public interface IDatabase<T, ReadT, WriteT> :
		IEnumerable<KeyValuePair<string, T>>,
		ISerializable<ReadT, WriteT>
		where T : class, ISerializable<ReadT, WriteT>, new()
	{
		/// <summary>
		///   File path used for serialization.
		/// </summary>
		string FilePath { get; }

		/// <summary>
		///   Element accessor.
		/// </summary>
		/// <param name="key">
		///   The key of the element to access.
		/// </param>
		T this[ string key ] { get; set; }

		/// <summary>
		///   If the database is empty.
		/// </summary>
		bool Empty { get; }
		/// <summary>
		///   The amount of elements the database contains.
		/// </summary>
		int Count { get; }
		/// <summary>
		///   List of element keys.
		/// </summary>
		List<string> Keys { get; }

		/// <summary>
		///   If the database contains an element with the given key.
		/// </summary>
		/// <param name="key">
		///   The element key.
		/// </param>
		/// <returns>
		///   True if the database contains an element with the given key and
		///   false otherwise.
		/// </returns>
		bool Contains( string key );

		/// <summary>
		///   Gets the element with the given key.
		/// </summary>
		/// <param name="key">
		///   The element key.
		/// </param>
		/// <returns>
		///   The element with the given key or null if one does not exist.
		/// </returns>
		T Get( string key );
		/// <summary>
		///   Replaces the element with the given key.
		/// </summary>
		/// <param name="key">
		///   The element key.
		/// </param>
		/// <param name="val">
		///   The new element.
		/// </param>
		/// <returns>
		///   True if the database contains an element with the given key and 
		///   it was successfully replaced, otherwise false.
		/// </returns>
		bool Set( string key, T val );
		/// <summary>
		///   Adds an element to the database, optionally replacing an existing
		///   element with the same key.
		/// </summary>
		/// <param name="key">
		///   The element key.
		/// </param>
		/// <param name="val">
		///   The element to add.
		/// </param>
		/// <param name="replace">
		///   If an element already exists in the database with the given key,
		///   should it be replaced?
		/// </param>
		/// <returns>
		///   True if the element was successfully added to the database,
		///   otherwise false.
		/// </returns>
		bool Add( string key, T val, bool replace = false );
		/// <summary>
		///   Removes the element with the given key from the database.
		/// </summary>
		/// <param name="key">
		///   The element key.
		/// </param>
		/// <returns>
		///   True if an element existed in the database with the given key and
		///   it was removed successfully, otherwise false.
		/// </returns>
		bool Remove( string key );
		/// <summary>
		///   Removes all elements from the database.
		/// </summary>
		void Clear();

		/// <summary>
		///   Loads the database from <see cref="FilePath"/>.
		/// </summary>
		/// <returns>
		///   True if the database was loaded successfully from file and false
		///   otherwise.
		/// </returns>
		bool LoadFromFile();
		/// <summary>
		///   Saves the database to <see cref="FilePath"/>.
		/// </summary>
		/// <param name="overwrite">
		///   If an already existing file should be overwritten.
		/// </param>
		/// <returns>
		///   True if the database was saves successfully to file and false
		///   otherwise.
		/// </returns>
		bool SaveToFile( bool overwrite = true );
	}
	/// <summary>
	///   Interface for disposable databases.
	/// </summary>
	/// <typeparam name="T">
	///   Disposable data type.
	/// </typeparam>
	/// <typeparam name="ReadT">
	///   The type used to deserialize from a stream.
	/// </typeparam>
	/// <typeparam name="WriteT">
	///   The type used to serialize to a stream.
	/// </typeparam>
	public interface IDisposableDatabase<T, ReadT, WriteT> :
		IDatabase<T, ReadT, WriteT>, IDisposable
		where T : class, IDisposable, ISerializable<ReadT, WriteT>, new()
	{ }

	/// <summary>
	///   Database for text serializable types.
	/// </summary>
	/// <typeparam name="T">
	///   Data type.
	/// </typeparam>
	public interface ITextDatabase<T> :
		IDatabase<T, StreamReader, StreamWriter>
		where T : class, ITextSerializable, new()
	{ }
	/// <summary>
	///   Database for disposable text serializable types.
	/// </summary>
	/// <typeparam name="T">
	///   Disposable data type.
	/// </typeparam>
	public interface IDisposableTextDatabase<T> : ITextDatabase<T>, IDisposable
		where T : class, IDisposable, ITextSerializable, new()
	{ }

	/// <summary>
	///   Database for binary serializable types.
	/// </summary>
	/// <typeparam name="T">
	///   Data type.
	/// </typeparam>
	public interface IBinaryDatabase<T> :
		IDatabase<T, BinaryReader, BinaryWriter>
		where T : class, IBinarySerializable, new()
	{ }
	/// <summary>
	///   Database for disposable binary serializable types.
	/// </summary>
	/// <typeparam name="T">
	///   Disposable data type.
	/// </typeparam>
	public interface IDisposableBinaryDatabase<T> : IBinaryDatabase<T>, IDisposable
		where T : class, IDisposable, IBinarySerializable, new()
	{ }

	/// <summary>
	///   Base class for databases.
	/// </summary>
	/// <typeparam name="T">
	///   Data type.
	/// </typeparam>
	/// <typeparam name="ReadT">
	///   The type used to deserialize from a stream.
	/// </typeparam>
	/// <typeparam name="WriteT">
	///   The type used to serialize to a stream.
	/// </typeparam>
	[Serializable]
	public abstract class Database<T, ReadT, WriteT> :
		IDatabase<T, ReadT, WriteT>
		where T : class, ISerializable<ReadT, WriteT>, new()
	{
		/// <summary>
		///   Constructor
		/// </summary>
		public Database()
		{
			m_db = new Dictionary<string, T>();
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="sd">
		///   The database to copy from.
		/// </param>
		public Database( Database<T, ReadT, WriteT> sd )
		{
			if( sd == null )
				throw new ArgumentNullException();

			m_db = new Dictionary<string, T>( sd.m_db.Count );

			foreach( var v in m_db )
				Add( v.Key, v.Value );
		}

		/// <summary>
		///   File path used for serialization.
		/// </summary>
		public abstract string FilePath { get; }

		/// <summary>
		///   Element accessor.
		/// </summary>
		/// <param name="key">
		///   The key of the element to access.
		/// </param>
		public T this[ string key ]
		{
			get { return Get( key ); }
			set
			{
				if( !Set( key, value ) )
					throw new ArgumentException();
			}
		}
		
		/// <summary>
		///   If the database is empty.
		/// </summary>
		public bool Empty
		{
			get { return Count == 0; }
		}
		/// <summary>
		///   The amount of elements the database contains.
		/// </summary>
		public int Count
		{
			get { return m_db.Count; }
		}

		/// <summary>
		///   List of element keys.
		/// </summary>
		public List<string> Keys
		{
			get { return m_db.Keys.ToList(); }
		}

		/// <summary>
		///   If the database contains an element with the given key.
		/// </summary>
		/// <param name="key">
		///   The element key.
		/// </param>
		/// <returns>
		///   True if the database contains an element with the given key and
		///   false otherwise.
		/// </returns>
		public bool Contains( string key )
		{
			if( string.IsNullOrWhiteSpace( key ) )
				return false;

			return m_db.ContainsKey( key );
		}

		/// <summary>
		///   Gets the element with the given key.
		/// </summary>
		/// <param name="key">
		///   The element key.
		/// </param>
		/// <returns>
		///   The element with the given key or null if one does not exist.
		/// </returns>
		public T Get( string key )
		{
			if( !Contains( key ) )
				return null;

			return m_db[ key ];
		}
		/// <summary>
		///   Replaces the element with the given key.
		/// </summary>
		/// <param name="key">
		///   The element key.
		/// </param>
		/// <param name="val">
		///   The new element.
		/// </param>
		/// <returns>
		///   True if the database contains an element with the given key and 
		///   it was successfully replaced, otherwise false.
		/// </returns>
		public bool Set( string key, T val )
		{
			if( !Contains( key ) )
				return false;

			m_db[ key ] = val;
			return true;
		}
		/// <summary>
		///   Adds an element to the database, optionally replacing an existing
		///   element with the same key.
		/// </summary>
		/// <param name="key">
		///   The element key.
		/// </param>
		/// <param name="val">
		///   The element to add.
		/// </param>
		/// <param name="replace">
		///   If an element already exists in the database with the given key,
		///   should it be replaced?
		/// </param>
		/// <returns>
		///   True if the element was successfully added to the database,
		///   otherwise false.
		/// </returns>
		public bool Add( string key, T val, bool replace = false )
		{
			if( val == null )
				return false;

			if( Contains( key ) )
			{
				if( !replace )
					return false;

				return Set( key, val );
			}

			try
			{
				m_db.Add( key, val );
			}
			catch
			{
				return false;
			}

			return true;
		}
		/// <summary>
		///   Removes the element with the given key from the database.
		/// </summary>
		/// <param name="key">
		///   The element key.
		/// </param>
		/// <returns>
		///   True if an element existed in the database with the given key and
		///   it was removed successfully, otherwise false.
		/// </returns>
		public virtual bool Remove( string key )
		{
			if( string.IsNullOrWhiteSpace( key ) )
				return false;

			return m_db.Remove( key );
		}
		/// <summary>
		///   Removes all elements from the database.
		/// </summary>
		public virtual void Clear()
		{
			m_db.Clear();
		}

		/// <summary>
		///   Attempts to deserialize the object from the stream.
		/// </summary>
		/// <param name="sr">
		///   Stream reader.
		/// </param>
		/// <returns>
		///   True if deserialization succeeded and false otherwise.
		/// </returns>
		public abstract bool LoadFromStream( ReadT sr );
		/// <summary>
		///   Attempts to serialize the object to the stream.
		/// </summary>
		/// <param name="sw">
		///   Stream writer.
		/// </param>
		/// <returns>
		///   True if serialization succeeded and false otherwise.
		/// </returns>
		public abstract bool SaveToStream( WriteT sw );
		/// <summary>
		///   Loads the database from <see cref="FilePath"/>.
		/// </summary>
		/// <returns>
		///   True if the database was loaded successfully from file and false
		///   otherwise.
		/// </returns>
		public abstract bool LoadFromFile();
		/// <summary>
		///   Saves the database to <see cref="FilePath"/>.
		/// </summary>
		/// <param name="overwrite">
		///   If an already existing file should be overwritten.
		/// </param>
		/// <returns>
		///   True if the database was saves successfully to file and false
		///   otherwise.
		/// </returns>
		public abstract bool SaveToFile( bool overwrite = true );

		/// <summary>
		///   Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		///   An enumerator that iterates through the collection.
		/// </returns>
		public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
		{
			return ( (IEnumerable<KeyValuePair<string, T>>)m_db ).GetEnumerator();
		}
		/// <summary>
		///   Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		///   An enumerator that iterates through the collection.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ( (IEnumerable<KeyValuePair<string, T>>)m_db ).GetEnumerator();
		}

		/// <summary>
		///   Collection of database elements indexed by their string key.
		/// </summary>
		protected Dictionary<string, T> m_db;
	}
	/// <summary>
	///   Base class for disposable databases.
	/// </summary>
	/// <typeparam name="T">
	///   Disposable data type.
	/// </typeparam>
	/// <typeparam name="ReadT">
	///   The type used to deserialize from a stream.
	/// </typeparam>
	/// <typeparam name="WriteT">
	///   The type used to serialize to a stream.
	/// </typeparam>
	[Serializable]
	public abstract class DisposableDatabase<T, ReadT, WriteT> :
		Database<T, ReadT, WriteT>, IDisposableDatabase<T, ReadT, WriteT>
		where T : class, IDisposable, ISerializable<ReadT, WriteT>, new()
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public DisposableDatabase()
		:	base()
		{ }
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="dd">
		///   The database to copy from.
		/// </param>
		public DisposableDatabase( DisposableDatabase<T, ReadT, WriteT> dd )
		:	base( dd )
		{ }

		/// <summary>
		///   Disposes and removes the element with the given key from the
		///   database.
		/// </summary>
		/// <param name="key">
		///   The element key.
		/// </param>
		/// <returns>
		///   True if an element existed in the database with the given key and
		///   it was removed successfully, otherwise false.
		/// </returns>
		public override bool Remove( string key )
		{
			if( Contains( key ) )
				m_db[ key ].Dispose();
			
			return base.Remove( key );
		}
		/// <summary>
		///   Removes all elements from the database.
		/// </summary>
		public override void Clear()
		{
			foreach( var v in m_db )
				v.Value?.Dispose();

			m_db.Clear();
		}

		/// <summary>
		///   Performs application-defined tasks associated with freeing, 
		///   releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Clear();
		}
	}

	/// <summary>
	///   Base class for databases that contain text serializable objects.
	/// </summary>
	/// <typeparam name="T">
	///   The text serializable type the database will contain.
	/// </typeparam>
	[Serializable]
	public abstract class TextDatabase<T> : 
		Database<T, StreamReader, StreamWriter>, ITextDatabase<T>
		where T : class, ITextSerializable, new()
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public TextDatabase()
		:	base()
		{ }
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="sd">
		///   The database to copy from.
		/// </param>
		public TextDatabase( TextDatabase<T> sd )
		:	base( sd )
		{ }

		/// <summary>
		///   Attempts to deserialize the object from the stream.
		/// </summary>
		/// <param name="sr">
		///   Stream reader.
		/// </param>
		/// <returns>
		///   True if deserialization succeeded and false otherwise.
		/// </returns>
		public override bool LoadFromStream( StreamReader sr )
		{
			if( sr == null || sr.EndOfStream )
				return false;

			try
			{
				string line = sr.ReadLine();

				while( string.IsNullOrWhiteSpace( line ) && !sr.EndOfStream )
					line = sr.ReadLine();

				if( sr.EndOfStream )
					return false;

				uint count = uint.Parse( line );

				for( uint i = 0; i < count; i++ )
				{
					line = sr.ReadLine();

					while( string.IsNullOrWhiteSpace( line ) && !sr.EndOfStream )
						line = sr.ReadLine();

					if( sr.EndOfStream )
						return false;

					string id = line;
					T t = new T();
					if( !t.LoadFromStream( sr ) )
						return false;

					if( !Add( id, t, true ) )
						return false;
				}
			}
			catch
			{
				return false;
			}

			return true;
		}
		/// <summary>
		///   Attempts to serialize the object to the stream.
		/// </summary>
		/// <param name="sw">
		///   Stream writer.
		/// </param>
		/// <returns>
		///   True if serialization succeeded and false otherwise.
		/// </returns>
		public override bool SaveToStream( StreamWriter sw )
		{
			if( sw == null )
				return false;

			try
			{
				sw.WriteLine( (uint)Count );

				foreach( var v in m_db )
				{
					sw.WriteLine( v.Key );
					if( !v.Value.SaveToStream( sw ) )
						return false;
					sw.WriteLine();
				}
			}
			catch
			{
				return false;
			}

			return true;
		}

		/// <summary>
		///   Loads the database from the file path.
		/// </summary>
		/// <returns>
		///   True if the database was loaded successfully from file and false
		///   otherwise.
		/// </returns>
		public override bool LoadFromFile()
		{
			if( !File.Exists( FilePath ) )
				return false;

			try
			{
				using( FileStream stream = File.OpenRead( FilePath ) )
				using( StreamReader br = new StreamReader( stream ) )
					return LoadFromStream( br );
			}
			catch
			{
				return false;
			}
		}
		/// <summary>
		///   Saves the database to the file path.
		/// </summary>
		/// <param name="overwrite">
		///   If an already existing file should be overwritten.
		/// </param>
		/// <returns>
		///   True if the database was saves successfully to file and false
		///   otherwise.
		/// </returns>
		public override bool SaveToFile( bool overwrite = true )
		{
			if( File.Exists( FilePath ) && !overwrite )
				return false;

			try
			{
				using( FileStream stream = File.Open( FilePath, FileMode.Create ) )
				using( StreamWriter bw = new StreamWriter( stream ) )
					return SaveToStream( bw );
			}
			catch
			{
				return false;
			}
		}
	}

	/// <summary>
	///   Base class for databases that contain disposable, text serializable objects.
	/// </summary>
	/// <typeparam name="T">
	///   The disposable, text serializable type the database will contain.
	/// </typeparam>
	[Serializable]
	public abstract class DisposableTextDatabase<T> : TextDatabase<T>, IDisposableTextDatabase<T>
		where T : class, IDisposable, ITextSerializable, new()
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public DisposableTextDatabase()
		:	base()
		{ }
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="dtd">
		///   The database to copy from.
		/// </param>
		public DisposableTextDatabase( DisposableTextDatabase<T> dtd )
		:	base( dtd )
		{ }

		/// <summary>
		///   Disposes and removes the element with the given key from the
		///   database.
		/// </summary>
		/// <param name="key">
		///   The element key.
		/// </param>
		/// <returns>
		///   True if an element existed in the database with the given key and
		///   it was removed successfully, otherwise false.
		/// </returns>
		public override bool Remove( string key )
		{
			if( Contains( key ) )
				m_db[ key ].Dispose();

			return base.Remove( key );
		}
		/// <summary>
		///   Removes all elements from the database.
		/// </summary>
		public override void Clear()
		{
			foreach( var v in m_db )
				v.Value?.Dispose();

			m_db.Clear();
		}

		/// <summary>
		///   Performs application-defined tasks associated with freeing, 
		///   releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Clear();
		}
	}

	/// <summary>
	///   Base class for databases that contain binary serializable objects.
	/// </summary>
	/// <typeparam name="T">
	///   The binary serializable type the database will contain.
	/// </typeparam>
	[Serializable]
	public abstract class BinaryDatabase<T> :
		Database<T, BinaryReader, BinaryWriter>, IBinaryDatabase<T>
		where T : class, IBinarySerializable, new()
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public BinaryDatabase()
		:	base()
		{ }
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="sd">
		///   The database to copy from.
		/// </param>
		public BinaryDatabase( BinaryDatabase<T> sd )
		:	base( sd )
		{ }

		/// <summary>
		///   Attempts to deserialize the object from the stream.
		/// </summary>
		/// <param name="sr">
		///   Stream reader.
		/// </param>
		/// <returns>
		///   True if deserialization succeeded and false otherwise.
		/// </returns>
		public override bool LoadFromStream( BinaryReader sr )
		{
			if( sr == null )
				return false;

			try
			{
				uint count = sr.ReadUInt32();
				m_db = new Dictionary<string, T>( (int)count );

				for( uint i = 0; i < count; i++ )
				{
					string id = sr.ReadString();
					T t = new T();
					if( !t.LoadFromStream( sr ) )
						return false;

					if( !Add( id, t, true ) )
						return false;
				}
			}
			catch
			{
				return false;
			}

			return true;
		}
		/// <summary>
		///   Attempts to serialize the object to the stream.
		/// </summary>
		/// <param name="sw">
		///   Stream writer.
		/// </param>
		/// <returns>
		///   True if serialization succeeded and false otherwise.
		/// </returns>
		public override bool SaveToStream( BinaryWriter sw )
		{
			if( sw == null )
				return false;

			try
			{
				sw.Write( (uint)Count );

				foreach( var v in m_db )
				{
					sw.Write( v.Key );
					if( !v.Value.SaveToStream( sw ) )
						return false;
				}
			}
			catch
			{
				return false;
			}

			return true;
		}

		/// <summary>
		///   Loads the database from the file path.
		/// </summary>
		/// <returns>
		///   True if the database was loaded successfully from file and false
		///   otherwise.
		/// </returns>
		public override bool LoadFromFile()
		{
			if( !File.Exists( FilePath ) )
				return false;

			try
			{
				using( FileStream stream = File.OpenRead( FilePath ) )
				using( BinaryReader br = new BinaryReader( stream ) )
					return LoadFromStream( br );
			}
			catch
			{
				return false;
			}
		}
		/// <summary>
		///   Saves the database to the file path.
		/// </summary>
		/// <param name="overwrite">
		///   If an already existing file should be overwritten.
		/// </param>
		/// <returns>
		///   True if the database was saves successfully to file and false
		///   otherwise.
		/// </returns>
		public override bool SaveToFile( bool overwrite = true )
		{
			if( File.Exists( FilePath ) && !overwrite )
				return false;

			try
			{
				using( FileStream stream = File.Open( FilePath, FileMode.Create ) )
				using( BinaryWriter bw = new BinaryWriter( stream ) )
					return SaveToStream( bw );
			}
			catch( Exception e )
			{
				Console.WriteLine( e.Message );
				return false;
			}
		}
	}
	/// <summary>
	///   Base class for databases that contain disposable, binary serializable objects.
	/// </summary>
	/// <typeparam name="T">
	///   The disposable, binary serializable type the database will contain.
	/// </typeparam>
	[Serializable]
	public abstract class DisposableBinaryDatabase<T> : BinaryDatabase<T>, IDisposableBinaryDatabase<T>
		where T : class, IDisposable, IBinarySerializable, new()
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public DisposableBinaryDatabase()
		:	base()
		{ }
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="dbd">
		///   The database to copy from.
		/// </param>
		public DisposableBinaryDatabase( DisposableBinaryDatabase<T> dbd )
		:	base( dbd )
		{ }

		/// <summary>
		///   Disposes and removes the element with the given key from the
		///   database.
		/// </summary>
		/// <param name="key">
		///   The element key.
		/// </param>
		/// <returns>
		///   True if an element existed in the database with the given key and
		///   it was removed successfully, otherwise false.
		/// </returns>
		public override bool Remove( string key )
		{
			if( Contains( key ) )
				m_db[ key ].Dispose();

			return base.Remove( key );
		}
		/// <summary>
		///   Removes all elements from the database.
		/// </summary>
		public override void Clear()
		{
			foreach( var v in m_db )
				v.Value?.Dispose();

			m_db.Clear();
		}
		/// <summary>
		///   Performs application-defined tasks associated with freeing, 
		///   releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Clear();
		}
	}
}
