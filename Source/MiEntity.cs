﻿////////////////////////////////////////////////////////////////////////////////
// MiEntity.cs 
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
using System.Text;
using System.Xml;

using SFML.Graphics;
using SFML.Window;

namespace MiCore
{
	/// <summary>
	///   Base class for all game objects.
	/// </summary>
	public class MiEntity : MiNode<MiEntity>, IEquatable<MiEntity>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public MiEntity()
		:	base()
		{
			m_window     = null;
			m_components = new List<MiComponent>();
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="ent">
		///   Entity to copy.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///   Inherited from <see cref="MiObject(MiObject)"/>.
		/// </exception>
		public MiEntity( MiEntity ent )
		:	base( ent )
		{
			m_window = ent.Window;

			if( ent.ComponentCount == 0 )
				m_components = new List<MiComponent>();
			else
			{
				m_components = new List<MiComponent>( ent.ComponentCount );

				foreach( MiComponent c in ent.m_components )
					if( !AddComponent( (MiComponent)c.Clone(), true ) )
						throw new InvalidOperationException( "Unable to add coppied component to ComponentStack." );
			}
		}
		/// <summary>
		///   Constructor setting the target render window.
		/// </summary>
		/// <param name="window">
		///   The target window.
		/// </param>
		public MiEntity( RenderWindow window )
		:	base()
		{
			m_window     = window;
			m_components = new List<MiComponent>();
		}
		/// <summary>
		///   Constructor setting the object ID and optionally the target render window.
		/// </summary>
		/// <param name="id">
		///   The object ID.
		/// </param>
		/// <param name="window">
		///   The target window.
		/// </param>
		public MiEntity( string id, RenderWindow window = null )
		:	base( id )
		{
			m_window     = window;
			m_components = new List<MiComponent>();
		}

		/// <summary>
		///   A reference to the target render window.
		/// </summary>
		public RenderWindow Window
		{
			get { return m_window; }
			set
			{
				UnsubscribeEvents();
				m_window = value;
				SubscribeEvents();

				if( HasChildren )
					foreach( MiEntity e in Children )
						if( e != null )
							e.Window = value;
			}
		}

		/// <summary>
		///   Unsubscribes window events, switches the target window and subscribes window events.
		/// </summary>
		/// <param name="window">
		///   The new target window.
		/// </param>
		public void ChangeWindow( RenderWindow window )
		{
			if( Window != null )
				foreach( MiComponent c in m_components )
					c?.UnsubscribeEvents();

			Window = window;

			if( Window != null )
				foreach( MiComponent c in m_components )
					c?.SubscribeEvents();

			if( HasChildren )
				foreach( MiEntity e in Children )
					e?.ChangeWindow( window );
		}

		/// <summary>
		///   The amount of components the stack contains.
		/// </summary>
		public int ComponentCount
		{
			get { return m_components.Count; }
		}

		/// <summary>
		///   Gets an array containing all components owned by the entity.
		/// </summary>
		public MiComponent[] Components
		{
			get { return m_components.ToArray(); }
		}

		/// <summary>
		///   Creates an array containing the names of all components incompatible with the existing
		///   components.
		/// </summary>
		public string[] GetIncompatibleComponents()
		{
			List<string> list = new List<string>();

			bool ContainsString( string s )
			{
				if( !Identifiable.IsValid( s ) )
					return false;

				foreach( string l in list )
					if( l.Equals( s ) )
						return true;

				return false;
			}

			foreach( MiComponent c in m_components )
				foreach( string i in c.IncompatibleComponents )
					if( !ContainsString( i ) )
						list.Add( i );

			return list.ToArray();
		}

		/// <summary>
		///   Checks if a given component type is compatible with the current components.
		/// </summary>
		/// <typeparam name="T">
		///   The component type.
		/// </typeparam>
		/// <returns>
		///   True if the component type is registered, compatible and can be added, otherwise false.
		/// </returns>
		public bool IsCompatible<T>() where T : MiComponent, new()
		{
			using( T t = new T() )
				return IsCompatible( t );
		}
		/// <summary>
		///   Checks if a given component type name is compatible with the current components.
		/// </summary>
		/// <param name="typename">
		///   The component type name.
		/// </param>
		/// <returns>
		///   True if the component type is registered, compatible and can be added, otherwise false.
		/// </returns>
		public bool IsCompatible( string typename )
		{
			if( typename == null || !ComponentRegister.Manager.Registered( typename ) )
				return false;

			string[] incomp = GetIncompatibleComponents();

			foreach( string s in incomp )
				if( s.Equals( typename ) )
					return false;

			return true;
		}
		/// <summary>
		///   Checks if a given component and its required components are compatible with the 
		///   current components.
		/// </summary>
		/// <param name="comp">
		///   The component.
		/// </param>
		/// <returns>
		///   True if the component and its requirements are registered, compatible and can be 
		///   added, otherwise false.
		/// </returns>
		public bool IsCompatible( MiComponent comp )
		{
			if( comp == null || !IsCompatible( comp.TypeName ) )
				return false;

			foreach( string s in comp.RequiredComponents )
				if( !IsCompatible( s ) )
					return false;

			return true;
		}

		/// <summary>
		///   Checks if the stack contains a component of the given type.
		/// </summary>
		/// <typeparam name="T">
		///   The component type.
		/// </typeparam>
		/// <returns>
		///   True if the stack contains a component with the given type, otherwise false.
		/// </returns>
		public bool HasComponent<T>() where T : MiComponent, new()
		{
			string typename;

			using( T t = new T() )
				typename = new string( t.TypeName.ToCharArray() );

			return HasComponent( typename );
		}
		/// <summary>
		///   Checks if the stack contains a component with the given type name.
		/// </summary>
		/// <param name="type">
		///   The component type name.
		/// </param>
		/// <returns>
		///   True if the stack contains a component with the given type name, otherwise false.
		/// </returns>
		public bool HasComponent( string type )
		{
			if( string.IsNullOrWhiteSpace( type ) )
				return false;

			foreach( MiComponent c in m_components )
			{
				if( c == null )
					continue;

				if( c.TypeName.Equals( type ) )
					return true;
			}

			return false;
		}

		/// <summary>
		///   Gets the index of the component with the given type.
		/// </summary>
		/// <typeparam name="T">
		///   The component type.
		/// </typeparam>
		/// <returns>
		///   A non-negative index if the stack contains a component with the given type,
		///   otherwise -1.
		/// </returns>
		public int ComponentIndex<T>() where T : MiComponent, new()
		{
			string typename;

			using( T t = new T() )
				typename = t.TypeName;

			return ComponentIndex( typename );
		}
		/// <summary>
		///   Gets the index of the component with the given type name.
		/// </summary>
		/// <param name="type">
		///   The component type name.
		/// </param>
		/// <returns>
		///   A non-negative index if the stack contains a component with the given type name,
		///   otherwise -1.
		/// </returns>
		public int ComponentIndex( string type )
		{
			if( !string.IsNullOrWhiteSpace( type ) )
			{ 
				for( int i = 0; i < ComponentCount; i++ )
				{
					if( m_components[ i ] == null )
						continue;
					if( m_components[ i ].TypeName.Equals( type ) )
						return i;
				}
			}

			return -1;
		}
		
		/// <summary>
		///   Gets the component at the given index.
		/// </summary>
		/// <param name="index">
		///   The component index.
		/// </param>
		/// <returns>
		///   The component at the given index or null if the index is out of range.
		/// </returns>
		public MiComponent GetComponent( int index )
		{
			if( index < 0 || index >= ComponentCount )
				return null;

			return m_components[ index ];
		}
		/// <summary>
		///   Gets the component with the given type name.
		/// </summary>
		/// <param name="typename">
		///   The component type name.
		/// </param>
		/// <returns>
		///   The component with the given type name if it exists, otherwise null.
		/// </returns>
		public MiComponent GetComponent( string typename )
		{
			return GetComponent( ComponentIndex( typename ) );
		}
		/// <summary>
		///   Gets the component with the given type.
		/// </summary>
		/// <typeparam name="T">
		///   The component type.
		/// </typeparam>
		/// <returns>
		///   The component with the given type or null if it does not exist.
		/// </returns>
		public T GetComponent<T>() where T : MiComponent, new()
		{
			return GetComponent( ComponentIndex<T>() ) as T;
		}

		/// <summary>
		///   Gets all immediate children that contain the given component.
		/// </summary>
		/// <typeparam name="T">
		///   The component type.
		/// </typeparam>
		/// <returns>
		///   All immediate children that contain the given component.
		/// </returns>
		public MiEntity[] GetChildrenWithComponent<T>() where T : MiComponent, new()
		{
			List<MiEntity> ents = new List<MiEntity>();

			foreach( MiEntity e in Children )
				if( e.HasComponent<T>() )
					ents.Add( e );

			return ents.ToArray();
		}
		/// <summary>
		///   Gets all immediate children that contain the given component.
		/// </summary>
		/// <param name="typename">
		///   The type name of the component.
		/// </param>
		/// <returns>
		///   All immediate children that contain the given component.
		/// </returns>
		public MiEntity[] GetChildrenWithComponent( string typename )
		{
			List<MiEntity> ents = new List<MiEntity>();

			foreach( MiEntity e in Children )
				if( e.HasComponent( typename ) )
					ents.Add( e );

			return ents.ToArray();
		}

		/// <summary>
		///   Recursively gets all children that contain the given component.
		/// </summary>
		/// <typeparam name="T">
		///   The component type.
		/// </typeparam>
		/// <returns>
		///   All immediate children that contain the given component.
		/// </returns>
		public MiEntity[] GetAllChildrenWithComponent<T>() where T : MiComponent, new()
		{
			List<MiEntity> ents = new List<MiEntity>();

			ents.AddRange( GetChildrenWithComponent<T>() );

			foreach( MiEntity e in Children )
				ents.AddRange( e.GetAllChildrenWithComponent<T>() );

			return ents.ToArray();
		}
		/// <summary>
		///   Recursively gets all immediate children that contain the given component.
		/// </summary>
		/// <param name="typename">
		///   The type name of the component.
		/// </param>
		/// <returns>
		///   All immediate children that contain the given component.
		/// </returns>
		public MiEntity[] GetAllChildrenWithComponent( string typename )
		{
			List<MiEntity> ents = new List<MiEntity>();

			ents.AddRange( GetChildrenWithComponent( typename ) );

			foreach( MiEntity e in Children )
				ents.AddRange( e.GetAllChildrenWithComponent( typename ) );

			return ents.ToArray();
		}

		/// <summary>
		///   Gets all immediate children that contain any of the given components.
		/// </summary>
		/// <param name="types">
		///   The type names of the components.
		/// </param>
		/// <returns>
		///   All immediate children that contain any of the given components.
		/// </returns>
		public MiEntity[] GetChildrenWithAny( params string[] types )
		{
			List<MiEntity> ents = new List<MiEntity>();

			foreach( MiEntity e in Children )
			{
				foreach( string typename in types )
				{
					if( e.HasComponent( typename ) )
					{
						ents.Add( e );
						break;
					}
				}
			}

			return ents.ToArray();
		}
		/// <summary>
		///   Gets all immediate children that contain all of the given components.
		/// </summary>
		/// <param name="types">
		///   The type names of the components.
		/// </param>
		/// <returns>
		///   All immediate children that contain all of the given components.
		/// </returns>
		public MiEntity[] GetChildrenWithAll( params string[] types )
		{
			List<MiEntity> ents = new List<MiEntity>();

			foreach( MiEntity e in Children )
			{
				bool contains = true;

				foreach( string typename in types )
				{
					if( !e.HasComponent( typename ) )
					{
						contains = false;
						break;
					}
				}

				if( contains )
					ents.Add( e );
			}

			return ents.ToArray();
		}

		/// <summary>
		///   Recursively gets all children that contain any of the given components.
		/// </summary>
		/// <param name="types">
		///   The type names of the components.
		/// </param>
		/// <returns>
		///   All children that contain any of the given components.
		/// </returns>
		public MiEntity[] GetAllChildrenWithAny( params string[] types )
		{
			List<MiEntity> ents = new List<MiEntity>();

			ents.AddRange( GetChildrenWithAny( types ) );

			foreach( MiEntity e in Children )
				ents.AddRange( e.GetAllChildrenWithAny( types ) );

			return ents.ToArray();
		}
		/// <summary>
		///   Recursively gets all children that contain all of the given components.
		/// </summary>
		/// <param name="types">
		///   The type names of the components.
		/// </param>
		/// <returns>
		///   All children that contain all of the given components.
		/// </returns>
		public MiEntity[] GetAllChildrenWithAll( params string[] types )
		{
			List<MiEntity> ents = new List<MiEntity>();

			ents.AddRange( GetChildrenWithAll( types ) );

			foreach( MiEntity e in Children )
				ents.AddRange( e.GetAllChildrenWithAll( types ) );

			return ents.ToArray();
		}

		/// <summary>
		///   Adds a new component to the stack.
		/// </summary>
		/// <typeparam name="T">
		///   The component type to add.
		/// </typeparam>
		/// <param name="replace">
		///   Should an already existing component of the same type be replaced?
		/// </param>
		/// <returns>
		///   True if the component was added successfully, otherwise false.
		/// </returns>
		public bool AddNewComponent<T>( bool replace = false ) where T : MiComponent, new()
		{
			if( !ComponentRegister.Manager.Registered<T>() )
				return Logger.LogReturn( "Unable to add component: Component is not registered.", false, LogType.Error );

			return AddComponent( ComponentRegister.Manager.Create<T>(), replace );
		}
		/// <summary>
		///   Adds a new existing component to the stack.
		/// </summary>
		/// <typeparam name="T">
		///   The component type to add.
		/// </typeparam>
		/// <param name="comp">
		///   The component to add.
		/// </param>
		/// <param name="replace">
		///   Should an already existing component of the same type be replaced?
		/// </param>
		/// <returns>
		///   True if the component was added successfully, otherwise false.
		/// </returns>
		public bool AddComponent<T>( T comp, bool replace = false ) where T : MiComponent, new()
		{
			if( !ComponentRegister.Manager.Registered<T>() )
				return Logger.LogReturn( "Unable to add component: Component is not registered.", false, LogType.Error );

			return AddComponent( (MiComponent)comp, replace );
		}
		/// <summary>
		///   Adds a component to the stack.
		/// </summary>
		/// <param name="comp">
		///   The component to add.
		/// </param>
		/// <param name="replace">
		///   If an already existing component should be replaced.
		/// </param>
		/// <returns>
		///   True if the component was added successfully, otherwise false.
		/// </returns>
		public bool AddComponent( MiComponent comp, bool replace = false )
		{
			if( m_components.Contains( comp ) )
				return true;

			if( !IsCompatible( comp ) )
				return false;			

			if( HasComponent( comp.TypeName ) )
			{
				if( !replace )
					return false;

				RemoveComponent( comp.TypeName );
			}

			List<string> added = new List<string>();

			comp.Parent = this;

			if( Window != null )
				comp.SubscribeEvents();

			m_components.Add( comp );
			added.Add( comp.TypeName );

			bool result = true;			

			foreach( string r in comp.RequiredComponents )
			{
				if( !HasComponent( r ) )
				{
					if( !AddComponent( ComponentRegister.Manager.Create( r ) ) )
					{
						result = false;
						break;
					}

					added.Add( r );
				}
				else
				{
					MoveComponent( ComponentIndex( r ), ComponentCount );
				}
			}

			if( !result )
			{
				foreach( string r in added )
					RemoveComponent( r );
			}
			else
				comp.OnAdd();

			return result;
		}

		/// <summary>
		///   Adds a range of components to the stack.
		/// </summary>
		/// <param name="comps">
		///   The component range to add.
		/// </param>
		/// <param name="replace">
		///   If an already existing component should be replaced.
		/// </param>
		/// <returns>
		///   True if comps is not null and all componenta were added successfully, otherwise false.
		/// </returns>
		public bool AddComponentRange( IEnumerable<MiComponent> comps, bool replace = false )
		{
			if( comps == null )
				return false;

			foreach( MiComponent c in comps )
				if( !AddComponent( c, replace ) )
					return false;

			return true;
		}

		/// <summary>
		///   Moves a component from one position to another in the list.
		/// </summary>
		/// <param name="from">
		///   The index of the component to move.
		/// </param>
		/// <param name="to">
		///   The destination to move the component. If >= <see cref="ComponentCount"/>, it will be
		///   added onto the end.
		/// </param>
		/// <returns>
		///   True if from and to are within range and the move was successful, otherwise false.
		/// </returns>
		public bool MoveComponent( int from, int to )
		{
			if( from < 0 || from >= ComponentCount ||
				to   < 0 )
				return false;

			if( from == to )
				return true;

			MiComponent c = GetComponent( from );

			if( to >= ComponentCount )
				m_components.Add( c );
			else
				m_components.Insert( to, c );

			m_components.RemoveAt( from );
			return true;
		}

		/// <summary>
		///   Removes the component with the given type.
		/// </summary>
		/// <typeparam name="T">
		///   The component type.
		/// </typeparam>
		/// <returns>
		///   True if there was a component with the given type and it was removed,
		///   otherwise false.
		/// </returns>
		public bool RemoveComponent<T>() where T : MiComponent, new()
		{
			return RemoveComponent( ComponentIndex<T>() );
		}
		/// <summary>
		///   Removes the component at the given index.
		/// </summary>
		/// <param name="index">
		///   The component index.
		/// </param>
		/// <returns>
		///   True if index was in range and the component was removed, otherwise false.
		/// </returns>
		public bool RemoveComponent( int index )
		{
			if( index < 0 || index >= ComponentCount )
				return false;

			List<string> rem = new List<string>();

			for( int i = 0; i < ComponentCount; i++ )
				if( i != index && m_components[ i ].Requires( m_components[ index ].TypeName ) )
					rem.Add( m_components[ i ].TypeName );

			m_components[ index ]?.UnsubscribeEvents();
			m_components[ index ]?.OnRemove();
			m_components[ index ]?.Dispose();
			m_components.RemoveAt( index );

			foreach( string s in rem )
				RemoveComponent( s );

			return true;
		}
		/// <summary>
		///   Removes the component with the given type name.
		/// </summary>
		/// <param name="typename">
		///   The component type name.
		/// </param>
		/// <returns>
		///   True if the component existed and was removed successfully.
		/// </returns>
		public bool RemoveComponent( string typename )
		{
			return RemoveComponent( ComponentIndex( typename ) );
		}

		/// <summary>
		///   Releases the component with the given type without disposing it.
		/// </summary>
		/// <typeparam name="T">
		///   The component type.
		/// </typeparam>
		/// <returns>
		///   The released component or null if nothing was removed.
		/// </returns>
		public T ReleaseComponent<T>() where T : MiComponent, new()
		{
			return ReleaseComponent( ComponentIndex<T>() ) as T;
		}
		/// <summary>
		///   Releases the component at the given index.
		/// </summary>
		/// <param name="index">
		///   The component index.
		/// </param>
		/// <returns>
		///   The released component or null if index is out of range.
		/// </returns>
		public MiComponent ReleaseComponent( int index )
		{
			if( index < 0 || index >= ComponentCount )
				return null;

			List<string> rem = new List<string>();

			for( int i = 0; i < ComponentCount; i++ )
				if( i != index && m_components[ i ].Requires( m_components[ index ].TypeName ) )
					rem.Add( m_components[ i ].TypeName );

			MiComponent result = m_components[ index ];
			result.UnsubscribeEvents();
			result.OnRemove();
			m_components.RemoveAt( index );

			foreach( string s in rem )
				RemoveComponent( s );

			result.Parent = null;
			return result;
		}
		/// <summary>
		///   Releases the component with the given type name.
		/// </summary>
		/// <param name="typename">
		///   The component type name.
		/// </param>
		/// <returns>
		///   The released component or null if it does not exist.
		/// </returns>
		public MiComponent ReleaseComponent( string typename )
		{
			return ReleaseComponent( ComponentIndex( typename ) );
		}
		/// <summary>
		///   Releases all components into an array and returns it.
		/// </summary>
		/// <returns>
		///   An array containing all released components.
		/// </returns>
		public MiComponent[] ReleaseAllComponents()
		{
			List<MiComponent> list = new List<MiComponent>();

			while( ComponentCount > 0 )
				list.Add( ReleaseComponent( 0 ) );

			return list.ToArray();
		}

		/// <summary>
		///   Disposes of and clears all components from the stack.
		/// </summary>
		public void ClearComponents()
		{
			foreach( MiComponent c in m_components )
			{
				c?.UnsubscribeEvents();
				c?.OnRemove();
				c?.Dispose();
			}

			m_components.Clear();
		}

		/// <summary>
		///   Refreshes components and child entities.
		/// </summary>
		public void Refresh()
		{
			for( int i = 0; i < ComponentCount; i++ )
			{
				m_components[ i ].Parent = this;
				m_components[ i ].Refresh();
			}
			foreach( MiEntity e in this )
			{
				e.Window = Window;
				e.Refresh();
			}
		}

		/// <summary>
		///   Updates the object if enabled; called once per frame.
		/// </summary>
		/// <param name="dt">
		///   Delta time.
		/// </param>
		public override void Update( float dt )
		{
			if( Enabled )
				OnUpdate( dt );
			else
				Refresh();
		}

		/// <summary>
		///   Subscribe to window events. 
		/// </summary>
		/// <param name="rec">
		///   Recursively subscribe child entity events?
		/// </param>
		/// <returns>
		///   True on success or false on failure.
		/// </returns>
		public bool SubscribeEvents( bool rec = true )
		{
			if( Window == null )
				return false;

			foreach( MiComponent c in m_components )
				c?.SubscribeEvents();

			if( rec )
				foreach( MiEntity e in this )
					e?.SubscribeEvents( rec );

			return true;
		}
		/// <summary>
		///   Unsubscribe to window events. 
		/// </summary>
		/// <param name="rec">
		///   Recursively unsubscribe child entity events?
		/// </param>
		/// <returns>
		///   True on success or false on failure.
		/// </returns>
		public bool UnsubscribeEvents( bool rec = true )
		{
			if( Window == null )
				return false;

			foreach( MiComponent c in m_components )
				c?.UnsubscribeEvents();

			if( rec )
				foreach( MiEntity e in this )
					e?.UnsubscribeEvents( rec );

			return true;
		}

		/// <summary>
		///   Updates components and child entities.
		/// </summary>
		/// <param name="dt">
		///   Delta time.
		/// </param>
		protected override void OnUpdate( float dt )
		{
			for( int i = 0; i < ComponentCount; i++ )
			{
				m_components[ i ].Parent = this;
				m_components[ i ]?.Update( dt );
			}
			foreach( MiEntity e in this )
			{
				e.Window = Window;
				e.Update( dt );
			}
		}
		/// <summary>
		///   Draws the component stack and children.
		/// </summary>
		/// <param name="target">
		///   The render target.
		/// </param>
		/// <param name="states">
		///   Render states.
		/// </param>
		protected override void OnDraw( RenderTarget target, RenderStates states )
		{
			for( int i = 0; i < ComponentCount; i++ )
			{
				m_components[ i ].Parent = this;
				m_components[ i ]?.Draw( target, states );
			}

			foreach( MiEntity e in this )
				e.Draw( target, states );
		}
		/// <summary>
		///   Disposes of the object and children.
		/// </summary>
		protected override void OnDispose()
		{
			ClearComponents();
			base.OnDispose();
			Window = null;
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
		public override bool LoadFromStream( BinaryReader sr )
		{
			if( !base.LoadFromStream( sr ) )
				return false;

			try
			{
				int count = sr.ReadInt32();
				m_components = new List<MiComponent>( count );

				for( int i = 0; i < count; i++ )
				{
					string type = sr.ReadString();

					if( !ComponentRegister.Manager.Registered( type ) )
						return Logger.LogReturn( "Failed loading MiEntity: Saved object contains unregistered component name" + type + ".", false, LogType.Error );

					if( HasComponent( type ) )
					{
						if( !GetComponent( type ).LoadFromStream( sr ) )
							return Logger.LogReturn( "Failed loading MiEntity: Unable to load component " + type + " from stream.", false, LogType.Error );
					}
					else
					{
						MiComponent c = ComponentRegister.Manager.Create( type );

						if( !c.LoadFromStream( sr ) )
							return Logger.LogReturn( "Failed loading MiEntity: Unable to load component " + type + " from stream.", false, LogType.Error );
						if( !AddComponent( c ) )
							return Logger.LogReturn( "Failed loading MiEntity: Unable to add component " + type + " loaded from stream.", false, LogType.Error );
					}
				}
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed loading MiEntity from stream: " + e.Message, false, LogType.Error );
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
			if( !base.SaveToStream( sw ) )
				return false;

			try
			{
				sw.Write( ComponentCount );

				for( int i = 0; i < ComponentCount; i++ )
				{
					sw.Write( m_components[ i ].TypeName );

					if( !m_components[ i ].SaveToStream( sw ) )
						return Logger.LogReturn( "Failed saving MiEntity: Unable to save component " + m_components[ i ].TypeName + " to stream.", false, LogType.Error );
				}
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Unable to save MiEntity to stream: " + e.Message, false, LogType.Error );
			}

			return true;
		}
		/// <summary>
		///   Attempts to load the object from the xml element.
		/// </summary>
		/// <param name="element">
		///   The xml element.
		/// </param>
		/// <returns>
		///   True if the object loaded successfully or false on failure.
		/// </returns>
		public override bool LoadFromXml( XmlElement element )
		{
			if( !base.LoadFromXml( element ) )
				return false;

			ClearComponents();

			XmlNodeList comps = element.ChildNodes;

			foreach( XmlNode node in comps )
			{
				if( node.NodeType != XmlNodeType.Element )
					continue;

				XmlElement e = (XmlElement)node;

				if( !ComponentRegister.Manager.Registered( e.Name ) )
					continue;

				MiComponent c = ComponentRegister.Manager.Create( e.Name );

				if( c == null )
					return Logger.LogReturn( "Unable to load MiEntity: Failed creating component " + e.Name + ".", false, LogType.Error );
				if( !c.LoadFromXml( e ) )
					return Logger.LogReturn( "Unable to load MiEntity: Failed parsing component " + e.Name + ".", false, LogType.Error );
				if( !AddComponent( c ) )
					return Logger.LogReturn( "Unable to load MiEntity: Failed adding component " + e.Name + ".", false, LogType.Error );
			}

			return true;
		}
		/// <summary>
		///   Gets the object xml string.
		/// </summary>
		/// <returns>
		///   The xml string of the object.
		/// </returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append( "<" );
			sb.Append( nameof( MiEntity ) );

			sb.Append( " " );
			sb.Append( nameof( ID ) );
			sb.Append( "=\"" );
			sb.Append( ID );
			sb.AppendLine( "\"" );

			sb.Append( "        " );
			sb.Append( nameof( Enabled ) );
			sb.Append( "=\"" );
			sb.Append( Enabled );
			sb.AppendLine( "\"" );

			sb.Append( "        " );
			sb.Append( nameof( Visible ) );
			sb.Append( "=\"" );
			sb.Append( Visible );			
			sb.AppendLine( "\">" );

			for( int i = 0; i < ComponentCount; i++ )
				sb.AppendLine( XmlLoadable.ToString( m_components[ i ], 1 ) );

			if( HasChildren )
			{
				sb.Append( "\t<" );
				sb.Append( nameof( Children ) );
				sb.AppendLine( ">" );

				foreach( MiEntity e in Children )
					sb.AppendLine( XmlLoadable.ToString( e, 2 ) );

				sb.Append( "\t</" );
				sb.Append( nameof( Children ) );
				sb.AppendLine( ">" );
			}

			sb.Append( "</" );
			sb.Append( nameof( MiEntity ) );
			sb.Append( ">" );

			return sb.ToString();
		}

		/// <summary>
		///   Checks if this object is equal to another.
		/// </summary>
		/// <param name="other">
		///   The object to check against.
		/// </param>
		/// <returns>
		///   True if the given object is concidered equal to this object, otherwise false.
		/// </returns>
		public new bool Equals( MiEntity other )
		{
			if( !base.Equals( other ) || ComponentCount != other.ComponentCount )
				return false;

			for( int i = 0; i < ComponentCount; i++ )
				if( !m_components[ i ].Equals( other.m_components[ i ] ) )
					return false;

			return true;
		}
		/// <summary>
		///   Returns a copy of this object.
		/// </summary>
		/// <returns>
		///   A copy of this object.
		/// </returns>
		public override object Clone()
		{
			return new MiEntity( this );
		}

		RenderWindow m_window;
		List<MiComponent> m_components;
	}
}
