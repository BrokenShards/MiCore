////////////////////////////////////////////////////////////////////////////////
// Component.cs 
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

namespace MiCore
{
	/// <summary>
	///   Base class for components.
	/// </summary>
	public abstract class Component : MiObject
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public Component()
		:	base()
		{
			Parent = null;

			RequiredComponents     = new string[ 0 ];
			IncompatibleComponents = new string[ 0 ];
		}
		/// <summary>
		///   Constructor.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		///   If any strings in required or incompatible are not registered component type names.
		///   If required and incompatible contain any of the same type names.
		/// </exception>
		public Component( string[] required, string[] incompatible = null )
		:	base()
		{
			Parent = null;

			if( required != null && required.Length > 0 )
			{
				for( int i = 0; i < required.Length; i++ )
					if( !ComponentRegister.Manager.Registered( required[ i ] ) )
						throw new InvalidOperationException( "Cannot construct component with invalid required type." );

				RequiredComponents = new string[ required.Length ];

				for( int i = 0; i < required.Length; i++ )
					RequiredComponents[ i ] = new string( required[ i ].ToCharArray() );
			}
			else
				RequiredComponents = new string[ 0 ];

			if( incompatible != null && incompatible.Length > 0 )
			{
				for( int i = 0; i < incompatible.Length; i++ )
				{
					if( !ComponentRegister.Manager.Registered( incompatible[ i ] ) )
						throw new InvalidOperationException( "Cannot construct component with invalid incompatible type." );

					for( int j = 0; j < RequiredComponents.Length; j++ )
						if( incompatible[ i ].Equals( RequiredComponents[ j ] ) )
							throw new InvalidOperationException( "Component type cannot be required and incompatible." );
				}

				IncompatibleComponents = new string[ incompatible.Length ];

				for( int i = 0; i < incompatible.Length; i++ )
					IncompatibleComponents[ i ] = new string( incompatible[ i ].ToCharArray() );
			}
			else
				IncompatibleComponents = new string[ 0 ];
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="comp">
		///   The object to copy.
		/// </param>
		public Component( Component comp )
		:	base( comp )
		{
			Parent = null;

			RequiredComponents     = new string[ 0 ];
			IncompatibleComponents = new string[ 0 ];
		}

		/// <summary>
		///   Contains the types of components required by this component type.
		/// </summary>
		public string[] RequiredComponents
		{
			get; protected set;
		}
		/// <summary>
		///   Contains the types of components incompatible with this component type.
		/// </summary>
		public string[] IncompatibleComponents
		{
			get; protected set;
		}

		/// <summary>
		///   The component type name.
		/// </summary>
		public abstract string TypeName
		{
			get;
		}

		/// <summary>
		///   The entity that owns the component.
		/// </summary>
		public Entity Parent
		{
			get; set;
		}

		/// <summary>
		///   Checks if the component requires a component with the given component name.
		/// </summary>
		/// <param name="typename">
		///   The name of the component to check.
		/// </param>
		/// <returns>
		///   True if the component requires on the given component type name.
		/// </returns>
		public bool Requires( string typename )
		{
			if( typename != null )
				foreach( string s in RequiredComponents )
					if( s.Equals( typename ) )
						return true;

			return false;
		}
		/// <summary>
		///   Checks if the component requires a component of the given type.
		/// </summary>
		/// <param name="type">
		///   The component type.
		/// </param>
		/// <returns>
		///   True if the component requires on the given component type.
		/// </returns>
		public bool Requires( Type type )
		{
			if( type == null )
				return false;

			string name = null;

			foreach( var v in ComponentRegister.Manager )
				if( v.Value.Equals( type ) )
					name = new string( v.Key.ToCharArray() );

			return Requires( name );
		}
		/// <summary>
		///   Checks if the component requires a component of the given type.
		/// </summary>
		/// <typeparam name="T">
		///   The component type.
		/// </typeparam>
		/// <returns>
		///   True if the component requires on the given component type.
		/// </returns>
		public bool Requires<T>() where T : Component, new()
		{
			using( T t = new T() )
				return Requires( t.TypeName );
		}

		/// <summary>
		///   Checks if the component is incompatible with a component with the given component name.
		/// </summary>
		/// <param name="typename">
		///   The name of the component to check.
		/// </param>
		/// <returns>
		///   True if the component is incompatible with the given component type name.
		/// </returns>
		public bool Incompatible( string typename )
		{
			if( typename != null )
				foreach( string s in IncompatibleComponents )
					if( s.Equals( typename ) )
						return true;

			return false;
		}
		/// <summary>
		///   Checks if the component is incompatible with a component of the given type.
		/// </summary>
		/// <param name="type">
		///   The component type.
		/// </param>
		/// <returns>
		///   True if the component is incompatible with the given component type.
		/// </returns>
		public bool Incompatible( Type type )
		{
			if( type == null )
				return false;

			string name = null;

			foreach( var v in ComponentRegister.Manager )
				if( v.Value.Equals( type ) )
					name = new string( v.Key.ToCharArray() );

			return Incompatible( name );
		}
		/// <summary>
		///   Checks if the component is incompatible with a component of the given type.
		/// </summary>
		/// <typeparam name="T">
		///   The component type.
		/// </typeparam>
		/// <returns>
		///   True if the component is incompatible with the given component type.
		/// </returns>
		public bool Incompatible<T>() where T : Component, new()
		{
			using( T t = new T() )
				return Incompatible( t.TypeName );
		}
	}
}
