////////////////////////////////////////////////////////////////////////////////
// TextDatabase.cs 
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
using System.Collections.Generic;
using System.IO;

namespace MiCore
{
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
			if( sr is null || sr.EndOfStream )
				return false;

			m_db = new Dictionary<string, T>();

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
					T t = new();

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
			if( sw is null )
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
				using FileStream stream = File.OpenRead( FilePath );
				using StreamReader br = new( stream );
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
				using FileStream stream = File.Open( FilePath, FileMode.Create );
				using StreamWriter bw = new( stream );
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
			GC.SuppressFinalize( this );
		}
	}
}
