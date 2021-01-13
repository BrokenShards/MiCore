////////////////////////////////////////////////////////////////////////////////
// MiComponent.cs 
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
using SFML.Window;

namespace MiCore
{
	/// <summary>
	///   Base class for components.
	/// </summary>
	public abstract class MiComponent : MiObject
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public MiComponent()
		:	base()
		{
			Stack                  = null;
			RequiredComponents     = null;
			IncompatibleComponents = null;
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="comp">
		///   The object to copy.
		/// </param>
		public MiComponent( MiComponent comp )
		:	base( comp )
		{
			Stack                  = null;
			RequiredComponents     = null;
			IncompatibleComponents = null;
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
		///   The component stack that owns the component.
		/// </summary>
		public ComponentStack Stack
		{
			get; set;
		}
		/// <summary>
		///   The entity that owns the component stack.
		/// </summary>
		public MiEntity Parent
		{
			get { return Stack.Parent; }
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
			if( typename != null && RequiredComponents != null )
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
			if( type == null || RequiredComponents == null )
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
		public bool Requires<T>() where T : MiComponent, new()
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
			if( typename != null && IncompatibleComponents != null )
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
			if( type == null || IncompatibleComponents == null )
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
		public bool Incompatible<T>() where T : MiComponent, new()
		{
			using( T t = new T() )
				return Incompatible( t.TypeName );
		}

		/// <summary>
		///   Called on TextEntered event.
		/// </summary>
		/// <param name="e">
		///   The event args.
		/// </param>
		public void TextEntered( TextEventArgs e )
		{
			if( e != null && Enabled )
				OnTextEntered( e );
		}

		/// <summary>
		///   Override this with your TextEntered logic.
		/// </summary>
		/// <param name="e">
		///   The event args.
		/// </param>
		protected virtual void OnTextEntered( TextEventArgs e )
		{ }
	}
}
