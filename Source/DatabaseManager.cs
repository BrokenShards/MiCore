////////////////////////////////////////////////////////////////////////////////
// DatabaseManager.cs 
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
using System.Collections.Generic;

namespace MiCore
{
	/// <summary>
	///   Singleton class that manages binary databases.
	/// </summary>
	public sealed class DatabaseManager
	{
		private DatabaseManager()
		{
			m_dbs = new Dictionary<Type, IBinarySerializable>();
		}

		/// <summary>
		///   If the manager contains no databases.
		/// </summary>
		public bool Empty
		{
			get { return Count == 0; }
		}
		/// <summary>
		///   The amount of databases in the manager.
		/// </summary>
		public int Count
		{
			get { return m_dbs.Count; }
		}

		/// <summary>
		///   If the manager contains a database of the given type.
		/// </summary>
		/// <param name="t">
		///   The database type to check.
		/// </param>
		/// <returns>
		///   True if the manager contains a database of the given type,
		///   otherwise false.
		/// </returns>
		public bool Contains( Type t )
		{
			if( t == null )
				return false;

			return m_dbs.ContainsKey( t );
		}
		/// <summary>
		///   Attempts to get the database of type T. Loading it from file or
		///   creating a new one if needed.
		/// </summary>
		/// <typeparam name="T">
		///   The binary database type.
		/// </typeparam>
		/// <typeparam name="D">
		///   Data type managed by T.
		/// </typeparam>
		/// <returns>
		///   The database of the given type if it exists or can be loaded,
		///   otherwise null.
		/// </returns>
		public T Get<T, D>() where T : class, IBinarySerializable, IBinaryDatabase<D>, new() where D : class, IBinarySerializable, new()
		{
			T db;

			try
			{
				if( !Contains( typeof( T ) ) && !Load<T, D>() && !Create<T, D>() )
					return null;

				db = m_dbs[ typeof( T ) ] as T;
			}
			catch( Exception e )
			{
				Logger.Log( "Unable to get database: " + e.Message );
				return null;
			}

			return db;
		}

		/// <summary>
		///   Attempts to load the database of type T from file.
		/// </summary>
		/// <typeparam name="T">
		///   The binary database type.
		/// </typeparam>
		/// <typeparam name="D">
		///   Data type managed by T
		/// </typeparam>
		/// <param name="reload">
		///   If an already loaded database should be loaded again.
		/// </param>
		/// <returns>
		///   True if the database was loaded successfully or loaded already,
		///   otherwise false.
		/// </returns>
		public bool Load<T, D>( bool reload = false ) where T : class, IBinarySerializable, IBinaryDatabase<D>, new() where D : class, IBinarySerializable, new()
		{
			if( Contains( typeof( T ) ) )
			{
				if( !reload )
					return true;

				T db;

				try
				{
					db = m_dbs[ typeof( T ) ] as T;
				}
				catch( Exception e )
				{
					Logger.Log( "Unable to load DB to DBManager: " + e.Message, LogType.Error );
					throw;
				}

				return db?.LoadFromFile() ?? false;
			}
			else
			{
				T db = new T();

				if( !db.LoadFromFile() )
					return false;

				m_dbs.Add( typeof( T ), db );
				return true;
			}
		}
		/// <summary>
		///   Attempts to save the database of type T to file.
		/// </summary>
		/// <typeparam name="T">
		///   The binary database type.
		/// </typeparam>
		/// <typeparam name="D">
		///   Data type managed by T
		/// </typeparam>
		/// <param name="overwrite">
		///   If an already existing file should be overwritten.
		/// </param>
		/// <returns>
		///   True if the database was already loaded and was saved successfully
		///   to file, otherwise false.
		/// </returns>
		public bool Save<T, D>( bool overwrite = true ) where T : class, IBinarySerializable, IBinaryDatabase<D>, new() where D : class, IBinarySerializable, new()
		{
			if( !Contains( typeof( T ) ) )
				return false;

			T db;

			try
			{
				db = m_dbs[ typeof( T ) ] as T;
			}
			catch( Exception e )
			{
				Logger.Log( "Unable to save DB from DBManager: " + e.Message, LogType.Error );
				throw;
			}

			return db.SaveToFile( overwrite );
		}
		/// <summary>
		///   Attempts to create a new database of type T.
		/// </summary>
		/// <typeparam name="T">
		///   The binary database type.
		/// </typeparam>
		/// <typeparam name="D">
		///   Data type managed by T
		/// </typeparam>
		/// <param name="delete">
		///   If an already existing database should be deleted.
		/// </param>
		/// <returns>
		///   True if the database was created successfully, otherwise false.
		/// </returns>
		public bool Create<T, D>( bool delete = false ) where T : class, IBinarySerializable, IBinaryDatabase<D>, new() where D : class, IBinarySerializable, new()
		{
			if( Contains( typeof( T ) ) )
			{
				if( !delete )
					return true;

				T db;

				try
				{
					db = m_dbs[ typeof( T ) ] as T;
				}
				catch( Exception e )
				{
					throw new InvalidOperationException( "Trying to cast an existing DB to a different DB type (Should be impossible).", e );
				}

				try
				{
					if( File.Exists( db.FilePath ) )
						File.Delete( db.FilePath );
				}
				catch( IOException )
				{
					return Logger.LogReturn( "Unable to delete old DB file as it is in use.", false, LogType.Warning );
				}
				catch( UnauthorizedAccessException )
				{
					return Logger.LogReturn( "Do not have permission to delete old DB file (try running as admin?).", false, LogType.Error );
				}
				catch( ArgumentNullException )
				{
					Logger.Log( "The specified DB FilePath is reading as null! Fix your code!", LogType.Error );
					throw;
				}
				catch( NotSupportedException )
				{
					Logger.Log( "The specified DB FilePath is not supported! Fix your code!", LogType.Error );
					throw;
				}
				catch( ArgumentException )
				{
					Logger.Log( "The specified DB FilePath must be invalid! Fix your code!", LogType.Error );
					throw;
				}
				catch
				{
					throw;
				}

				m_dbs.Remove( typeof( T ) );				
			}

			m_dbs.Add( typeof( T ), new T() );
			return true;
		}

		private Dictionary<Type, IBinarySerializable> m_dbs;

		/// <summary>
		///   The singleton instance.
		/// </summary>
		public static DatabaseManager Instance
		{
			get
			{
				if( _instance == null )
				{
					lock( _syncRoot )
					{
						if( _instance == null )
						{
							_instance = new DatabaseManager();
						}
					}
				}

				return _instance;
			}
		}

		private static volatile DatabaseManager _instance;
		private static readonly object _syncRoot = new object();
	}
}
