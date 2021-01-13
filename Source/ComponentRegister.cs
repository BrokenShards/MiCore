////////////////////////////////////////////////////////////////////////////////
// ComponentRegister.cs 
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
	///   Used for registering and creating components.
	/// </summary>
	public class ComponentRegister : IEnumerable<KeyValuePair<string, Type>>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		private ComponentRegister()
		{
			m_typemap = new Dictionary<string, Type>();
		}

		/// <summary>
		///   The register object.
		/// </summary>
		public static ComponentRegister Manager
		{
			get
			{
				if( _instance == null )
				{
					lock( _syncRoot )
					{
						if( _instance == null )
						{
							_instance = new ComponentRegister();
						}
					}
				}

				return _instance;
			}
		}

		/// <summary>
		///   If no components are registered.
		/// </summary>
		public bool Empty
		{
			get { return Count == 0; }
		}
		/// <summary>
		///   The amount of components registered.
		/// </summary>
		public int Count
		{
			get { return m_typemap?.Count ?? 0; }
		}

		/// <summary>
		///   Checks if the given component type is registered.
		/// </summary>
		/// <typeparam name="T">
		///   The component type.
		/// </typeparam>
		/// <returns>
		///   True if the given component type is registered, otherwise false.
		/// </returns>
		public bool Registered<T>() where T : MiComponent, new()
		{
			return Registered( typeof( T ) );
		}
		/// <summary>
		///   Checks if the given component type name is registered.
		/// </summary>
		/// <param name="typename">
		///   The component type name.
		/// </param>
		/// <returns>
		///   True if the given component type name is registered, otherwise false.
		/// </returns>
		public bool Registered( string typename )
		{
			if( typename == null )
				return false;

			return m_typemap.ContainsKey( typename );
		}
		/// <summary>
		///   Checks if the given component type is registered.
		/// </summary>
		/// <param name="type">
		///   The component type.
		/// </param>
		/// <returns>
		///   True if the given component type is registered, otherwise false.
		/// </returns>
		public bool Registered( Type type )
		{
			if( type == null )
				return false;

			return m_typemap.ContainsValue( type );
		}

		/// <summary>
		///   Registers a component type.
		/// </summary>
		/// <typeparam name="T">
		///   The component type.
		/// </typeparam>
		/// <returns>
		///   True if the component type was registered successfully and false otherwise. This will
		///   only fail if `T.TypeName` is not a valid ID.
		/// </returns>
		public bool Register<T>( bool replace = true ) where T : MiComponent, new()
		{
			string name;

			using( T t = new T() )
				name = t.TypeName;

			if( !Identifiable.IsValid( name ) )
				return false;

			if( Registered<T>() )
			{
				if( !replace )
					return true;

				m_typemap.Remove( name );
			}

			m_typemap.Add( name, typeof( T ) );
			return true;
		}
		
		/// <summary>
		///   Creates a new component of the given type.
		/// </summary>
		/// <typeparam name="T">
		///   The component type.
		/// </typeparam>
		/// <returns>
		///   A new component of the given type or null if unregistered or unable to create.
		/// </returns>
		public T Create<T>() where T : MiComponent, new()
		{
			string name;

			using( T t = new T() )
				name = t.TypeName;

			if( !m_typemap.ContainsKey( name ) )
				return null;

			T c;

			try
			{
				c = (T)Activator.CreateInstance( m_typemap[ name ] );
			}
			catch( Exception e )
			{
				return Logger.LogReturn<T>( "Unable to create component: " + e.Message, null, LogType.Error );
			}

			return c;
		}
		/// <summary>
		///   Creates a component from a given type name.
		/// </summary>
		/// <param name="typename">
		///   The component type name.
		/// </param>
		/// <returns>
		///   A new component if the type name was registered, otherwise false.
		/// </returns>
		public MiComponent Create( string typename )
		{
			if( !Registered( typename ) )
				return null;

			MiComponent c;

			try
			{
				c = (MiComponent)Activator.CreateInstance( m_typemap[ typename ] );
			}
			catch( Exception e )
			{
				return Logger.LogReturn<MiComponent>( "Unable to create component: " + e.Message, null, LogType.Error );
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

		Dictionary<string, Type> m_typemap;

		private static volatile ComponentRegister _instance;
		private static readonly object _syncRoot = new object();
	}
}
