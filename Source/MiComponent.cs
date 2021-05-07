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

namespace MiCore
{
	/// <summary>
	///   Base class for components.
	/// </summary>
	public abstract class MiComponent : MiObject, IEquatable<MiComponent>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public MiComponent()
		:	base()
		{
			Parent                 = null;
			RequiredComponents     = GetRequiredComponents();
			IncompatibleComponents = GetIncompatibleComponents();
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
			Parent                 = null;
			RequiredComponents     = GetRequiredComponents();
			IncompatibleComponents = GetIncompatibleComponents();
		}

		/// <summary>
		///   Contains the type names of components required by this component type.
		/// </summary>
		public readonly string[] RequiredComponents;
		/// <summary>
		///   Contains the type names of components incompatible with this component type.
		/// </summary>
		public readonly string[] IncompatibleComponents;

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
		public MiEntity Parent
		{
			get; set;
		}

		/// <summary>
		///   Called when the component is added to an entity.
		/// </summary>
		public virtual void OnAdd()
		{ }
		/// <summary>
		///   Called when the component is removed from an entity.
		/// </summary>
		public virtual void OnRemove()
		{ }

		/// <summary>
		///   Refreshes the components visual elements.
		/// </summary>
		/// <remarks>
		///   This method is called before the component is updated and should only set up the
		///   component to be displayed. It should contain no logic and not respond to any input.
		/// </remarks>
		public virtual void Refresh()
		{ }

		/// <summary>
		///   Updates the object if enabled; called once per frame.
		/// </summary>
		/// <param name="dt">
		///   Delta time.
		/// </param>
		public override void Update( float dt )
		{
			Refresh();

			if( Enabled )
				OnUpdate( dt );
		}

		/// <summary>
		///   Subscribe to window events. 
		/// </summary>
		public virtual void SubscribeEvents()
		{ }
		/// <summary>
		///   Unsubscribe to window events. 
		/// </summary>
		public virtual void UnsubscribeEvents()
		{ }

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
			if( typename is not null && RequiredComponents is not null )
				for( int i = 0; i < RequiredComponents.Length; i++ )
					if( RequiredComponents[ i ].Equals( typename ) )
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
			if( type is null || RequiredComponents is null )
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
			using T t = new();
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
			if( typename is not null && IncompatibleComponents is not null )
				for( int i = 0; i < IncompatibleComponents.Length; i++ )
					if( IncompatibleComponents[ i ].Equals( typename ) )
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
			if( type is null || IncompatibleComponents is null )
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
			using T t = new();
			return Incompatible( t.TypeName );
		}

		/// <summary>
		///   Checks if the object is considered equal to this object.
		/// </summary>
		/// <param name="other">
		///   The object to compare to.
		/// </param>
		/// <returns>
		///   True if the object is considered equal to this object, otherwise false.
		/// </returns>
		public bool Equals( MiComponent other )
		{
			if( other is null || TypeName != other.TypeName )
				return false;

			return true;
		}
		/// <summary>
		///   Checks if the object is considered equal to this object.
		/// </summary>
		/// <param name="obj">
		///   The object to compare to.
		/// </param>
		/// <returns>
		///   True if the object is considered equal to this object, otherwise false.
		/// </returns>
		public override bool Equals( object obj )
		{
			return Equals( obj as MiComponent );
		}

		/// <summary>
		///   Gets the type names of components required by this component type. Used to assign
		///   <see cref="RequiredComponents"/>.
		/// </summary>
		/// <returns>
		///   The type names of components required by this component type.
		/// </returns>
		protected virtual string[] GetRequiredComponents()
		{
			return Array.Empty<string>();
		}
		/// <summary>
		///   Gets the type names of components incompatible with this component type. Used to
		///   assign <see cref="IncompatibleComponents"/>.
		/// </summary>
		/// <returns>
		///   The type names of components incompatible with this component type.
		/// </returns>
		protected virtual string[] GetIncompatibleComponents()
		{
			return Array.Empty<string>();
		}

		/// <summary>
		///   Serves as the default hash function,
		/// </summary>
		/// <returns>
		///   A hash code for the current object.
		/// </returns>
		public override int GetHashCode()
		{
			return HashCode.Combine( base.GetHashCode(), Parent, RequiredComponents, IncompatibleComponents );
		}
	}
}
