////////////////////////////////////////////////////////////////////////////////
// TypeRegister.cs 
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
using System.Collections;
using System.Collections.Generic;

namespace MiCore
{
	/// <summary>
	///   Used for registering types to IDs.
	/// </summary>
	public class TypeRegister<RT> : IEnumerable<KeyValuePair<string, Type>> where RT : class
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public TypeRegister()
		{
			m_typemap = new Dictionary<string, Type>();
			m_allownull = false;
		}
		/// <summary>
		///   Constructor setting if null types can be registered.
		/// </summary>
		/// <param name="allownull">
		///   If null types can be registered.
		/// </param>
		public TypeRegister( bool allownull )
		{
			m_typemap = new Dictionary<string, Type>();
			m_allownull = allownull;
		}

		/// <summary>
		///   If no types are registered.
		/// </summary>
		public bool Empty
		{
			get { return Count == 0; }
		}
		/// <summary>
		///   The amount of types registered.
		/// </summary>
		public int Count
		{
			get { return m_typemap?.Count ?? 0; }
		}

		/// <summary>
		///   Gets the type mapped to the given type ID.
		/// </summary>
		/// <param name="typeid">
		///   The type ID.
		/// </param>
		/// <returns>
		///   The type mapped to the given type ID.
		/// </returns>
		public Type Get( string typeid )
		{
			if( !Registered( typeid ) )
				return null;

			return m_typemap[ typeid ];
		}

		/// <summary>
		///   Checks if the given type is registered.
		/// </summary>
		/// <typeparam name="T">
		///   The type.
		/// </typeparam>
		/// <returns>
		///   True if the given type is registered, otherwise false.
		/// </returns>
		public bool Registered<T>() where T : class, RT, new()
		{
			return Registered( typeof( T ) );
		}
		/// <summary>
		///   Checks if the given type ID is registered.
		/// </summary>
		/// <param name="typeid">
		///   The type ID.
		/// </param>
		/// <returns>
		///   True if the given type ID is registered, otherwise false.
		/// </returns>
		public bool Registered( string typeid )
		{
			if( typeid is null )
				return false;

			return m_typemap.ContainsKey( typeid );
		}
		/// <summary>
		///   Checks if the given type is registered.
		/// </summary>
		/// <remarks>
		///   Always returns false if type is null.
		/// </remarks>
		/// <param name="type">
		///   The type.
		/// </param>
		/// <returns>
		///   True if the given type is registered, otherwise false.
		/// </returns>
		public bool Registered( Type type )
		{
			if( type is null )
				return false;

			return m_typemap.ContainsValue( type );
		}

		/// <summary>
		///   Registers a type.
		/// </summary>
		/// <typeparam name="T">
		///   The type.
		/// </typeparam>
		/// <param name="typeid">
		///   The type ID.
		/// </param>
		/// <param name="replace">
		///   If an already registered type should be replaced.
		/// </param>
		/// <returns>
		///   True if the type was registered successfully; false if typeid is not a valid ID or a
		///   type is already registered to the ID and replace is false.
		/// </returns>
		public bool Register<T>( string typeid, bool replace = true ) where T : class, RT, new()
		{
			if( !Identifiable.IsValid( typeid ) )
				return false;

			if( Registered<T>() )
			{
				if( !replace )
					return true;

				m_typemap.Remove( typeid );
			}

			m_typemap.Add( typeid, typeof( T ) );
			return true;
		}
		/// <summary>
		///   Registers a type to a type ID.
		/// </summary>
		/// <param name="typeid">
		///   Type ID.
		/// </param>
		/// <param name="type">
		///   Object type.
		/// </param>
		/// <param name="replace">
		///   If a type is already registered to the type ID, should it be replaced? 
		/// </param>
		/// <returns>
		///   True if the type was registered successfully; false if typeid is not a valid ID, type
		///   is null, or a type is already registered to the ID and replace is false.
		/// </returns>
		public bool Register( string typeid, Type type, bool replace = true )
		{
			if( !Identifiable.IsValid( typeid ) )
				return false;
			if( type is null && !m_allownull )
				return false;
			if( type is not null && !type.IsSubclassOf( typeof( RT ) ) )
				return false;

			if( Registered( typeid ) )
			{
				if( !replace )
					return true;

				m_typemap.Remove( typeid );
			}

			m_typemap.Add( typeid, type );
			return true;
		}

		/// <summary>
		///   Creates a new object of the given registered type.
		/// </summary>
		/// <typeparam name="T">
		///   The type.
		/// </typeparam>
		/// <returns>
		///   A new object of the given type or null if unregistered or unable to create.
		/// </returns>
		public T Create<T>() where T : class, RT, new()
		{
			bool exists = false;

			foreach( var v in this )
			{
				if( v.Value.Equals( typeof( T ) ) )
				{
					exists = true;
					break;
				}
			}

			if( !exists )
				return null;

			T c;

			try
			{
				c = (T)Activator.CreateInstance( typeof( T ) );
			}
			catch( Exception e )
			{
				return Logger.LogReturn<T>( $"Unable to create object: { e.Message }", null, LogType.Error );
			}

			return c;
		}
		/// <summary>
		///   Creates an object of the type registered to the type ID.
		/// </summary>
		/// <param name="typeid">
		///   The type ID.
		/// </param>
		/// <returns>
		///   A new object if the type ID was registered, otherwise false.
		/// </returns>
		public RT Create( string typeid )
		{
			if( !Registered( typeid ) || Get( typeid ) is null )
				return null;

			RT c;

			try
			{
				c = (RT)Activator.CreateInstance( m_typemap[ typeid ] );
			}
			catch( Exception e )
			{
				return Logger.LogReturn<RT>( $"Unable to create object: { e.Message }", null, LogType.Error );
			}

			return c;
		}

		/// <summary>
		///   Gets an enumerator to iterate through the collection.
		/// </summary>
		/// <returns>
		///   An enumerator to iterate through the collection.
		/// </returns>
		public IEnumerator<KeyValuePair<string, Type>> GetEnumerator()
		{
			return ( (IEnumerable<KeyValuePair<string, Type>>)m_typemap ).GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ( (IEnumerable)m_typemap ).GetEnumerator();
		}

		/// <summary>If null types can be registered.</summary>
		protected readonly bool m_allownull;

		readonly Dictionary<string, Type> m_typemap;
	}
}
