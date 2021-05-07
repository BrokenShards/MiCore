////////////////////////////////////////////////////////////////////////////////
// IDatabase.cs 
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
}
