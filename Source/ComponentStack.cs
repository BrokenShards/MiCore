////////////////////////////////////////////////////////////////////////////////
// ComponentStack.cs 
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
using System.IO;
using System.Text;
using System.Xml;

using SFML.Graphics;

namespace MiCore
{
	/// <summary>
	///   Container for storing related components.
	/// </summary>
	public class ComponentStack : MiObject, IEnumerable<MiComponent>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public ComponentStack()
		:	base()
		{
			Parent       = null;
			m_components = new List<MiComponent>();
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="cs">
		///   The object to copy.
		/// </param>
		/// <exception cref="InvalidOperationException">
		///   If unable to add cloned components.
		/// </exception>
		public ComponentStack( ComponentStack cs )
		:	base( cs )
		{
			Parent = cs.Parent;

			if( cs.Empty )
				m_components = new List<MiComponent>();
			else
			{
				m_components = new List<MiComponent>( cs.Count );

				foreach( MiComponent c in cs )
					if( !Add( (MiComponent)c.Clone(), true ) )
						throw new InvalidOperationException( "Unable to add coppied component to ComponentStack." );
			}
		}
		/// <summary>
		///   Constructor setting parent entity.
		/// </summary>
		/// <param name="parent">
		///   The owning parent entity
		/// </param>
		public ComponentStack( MiEntity parent )
		:	base()
		{
			Parent       = parent;
			m_components = new List<MiComponent>();
		}

		/// <summary>
		///   The owning parent entity.
		/// </summary>
		public MiEntity Parent
		{
			get; set;
		}

		/// <summary>
		///   If the stack contains any components.
		/// </summary>
		public bool Empty
		{
			get { return Count == 0; }
		}
		/// <summary>
		///   The amount of components the stack contains.
		/// </summary>
		public int Count
		{
			get { return m_components.Count; }
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
			{
				if( c.IncompatibleComponents == null )
					continue;

				foreach( string i in c.IncompatibleComponents )
					if( !ContainsString( i ) )
						list.Add( i );
			}

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

			if( comp.RequiredComponents != null )
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
		public bool Contains<T>() where T : MiComponent, new()
		{
			string typename;

			using( T t = new T() )
				typename = new string( t.TypeName.ToCharArray() );

			return Contains( typename );
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
		public bool Contains( string type )
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
		public int IndexOf<T>() where T : MiComponent, new()
		{
			string typename;

			using( T t = new T() )
				typename = t.TypeName;

			return IndexOf( typename );
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
		public int IndexOf( string type )
		{
			if( !string.IsNullOrWhiteSpace( type ) )
			{ 
				for( int i = 0; i < Count; i++ )
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
		public MiComponent Get( int index )
		{
			if( index < 0 || index >= Count )
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
		public MiComponent Get( string typename )
		{
			return Get( IndexOf( typename ) );
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
		public T Get<T>() where T : MiComponent, new()
		{
			return Get( IndexOf<T>() ) as T;
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
		public bool AddNew<T>( bool replace = false ) where T : MiComponent, new()
		{
			if( !ComponentRegister.Manager.Registered<T>() )
				return Logger.LogReturn( "Unable to add component: Component is not registered.", false, LogType.Error );

			return Add( ComponentRegister.Manager.Create<T>(), replace );
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
		public bool Add<T>( T comp, bool replace = false ) where T : MiComponent, new()
		{
			if( !ComponentRegister.Manager.Registered<T>() )
				return Logger.LogReturn( "Unable to add component: Component is not registered.", false, LogType.Error );

			return Add( (MiComponent)comp, replace );
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
		public bool Add( MiComponent comp, bool replace = false )
		{
			if( !IsCompatible( comp ) )
				return false;
			if( m_components.Contains( comp ) )
				return true;

			if( Contains( comp.TypeName ) )
			{
				if( !replace )
					return false;

				Remove( comp.TypeName );
			}

			if( comp.RequiredComponents != null )
				foreach( string r in comp.RequiredComponents )
					if( !Contains( r ) )
						if( !Add( ComponentRegister.Manager.Create( r ) ) )
							return false;

			comp.Stack = this;
			m_components.Add( comp );
			return true;
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
		public bool AddRange( IEnumerable<MiComponent> comps, bool replace = false )
		{
			if( comps == null )
				return false;

			foreach( MiComponent c in comps )
				if( !Add( c, replace ) )
					return false;

			return true;
		}
		/// <summary>
		///   Clones and adds a range of components from another stack.
		/// </summary>
		/// <param name="stack">
		///   The component stack to add.
		/// </param>
		/// <param name="replace">
		///   If an already existing component should be replaced.
		/// </param>
		/// <returns>
		///   True if stack is not null and all componenta were added successfully, otherwise false.
		/// </returns>
		public bool AddRange( ComponentStack stack, bool replace = false )
		{
			if( stack == null )
				return false;

			foreach( MiComponent c in stack )
				if( !Add( (MiComponent)c.Clone(), replace ) )
					return false;

			return true;
		}

		/// <summary>
		///   Inserts a component at the given index in the stack.
		/// </summary>
		/// <param name="index">
		///   The index to insert the component. <see cref="Add(MiComponent, bool)"/> will be called
		///   instead if equals to <see cref="Count"/>.
		/// </param>
		/// <param name="comp">
		///   The component to add.
		/// </param>
		/// <param name="replace">
		///   Should an alreadu existing component of the same type be replaced?
		/// </param>
		/// <returns>
		///   True if the component was inserted successfully, otherwise false.
		/// </returns>
		public bool Insert( int index, MiComponent comp, bool replace = false )
		{
			if( comp == null || index < 0 || index > Count || !IsCompatible( comp ) )
				return false;
			if( index == Count )
				return Add( comp, replace );
			if( m_components.Contains( comp ) )
				return Insert( index, Release( comp.TypeName ), replace );

			if( Contains( comp.TypeName ) )
			{
				if( !replace )
					return false;

				int i = IndexOf( comp.TypeName );

				if( i < index )
					index--;

				Remove( comp.TypeName );
			}

			if( comp.RequiredComponents != null )
				foreach( string r in comp.RequiredComponents )
					if( !Contains( r ) )
						if( !Add( ComponentRegister.Manager.Create( r ) ) )
							return Logger.LogReturn( "Unable to add component to Entity.", false, LogType.Error );

			comp.Stack = this;
			m_components.Insert( index, comp );
			return true;
		}
		/// <summary>
		///   Inserts a component at the given index in the stack.
		/// </summary>
		/// <typeparam name="T">
		///   The component type to add.
		/// </typeparam>
		/// <param name="index">
		///   The index to insert the component.
		/// </param>
		/// <param name="comp">
		///   The component to add.
		/// </param>
		/// <param name="replace">
		///   Should an already existing component of the same type be replaced?
		/// </param>
		/// <returns>
		///   True if the component was added successfully, otherwise false.
		/// </returns>
		public bool Insert<T>( int index, T comp, bool replace = false ) where T : MiComponent, new()
		{
			if( !ComponentRegister.Manager.Registered<T>() )
				return Logger.LogReturn( "Unable to add component: Component is not registered.", false, LogType.Error );

			return Insert( index, (MiComponent)comp, replace );
		}
		/// <summary>
		///   Inserts a component at the given index in the stack.
		/// </summary>
		/// <typeparam name="T">
		///   The component type to add.
		/// </typeparam>
		/// <param name="index">
		///   The index to insert the component.
		/// </param>
		/// <param name="replace">
		///   Should an already existing component of the same type be replaced?
		/// </param>
		/// <returns>
		///   True if the component was added successfully, otherwise false.
		/// </returns>
		public bool InsertNew<T>( int index, bool replace = false ) where T : MiComponent, new()
		{
			if( !ComponentRegister.Manager.Registered<T>() )
				return Logger.LogReturn( "Unable to add component: Component is not registered.", false, LogType.Error );

			return Insert( index, ComponentRegister.Manager.Create<T>(), replace );
		}

		/// <summary>
		///   Inserts a range of components to the stack at the given index.
		/// </summary>
		/// <param name="index">
		///   The index to insert the components.
		/// </param>
		/// <param name="comps">
		///   The component range to add.
		/// </param>
		/// <param name="replace">
		///   If an already existing component should be replaced.
		/// </param>
		/// <returns>
		///   True if comps is not null and all componenta were added successfully, otherwise false.
		/// </returns>
		public bool InsertRange( int index, IEnumerable<MiComponent> comps, bool replace = false )
		{
			if( comps == null || index < 0 || index > Count )
				return false;
			if( index == Count )
				return AddRange( comps, replace );

			int i = index;

			foreach( MiComponent c in comps )
			{
				if( !Insert( i, c, replace ) )
					return false;

				i++;
			}

			return true;
		}
		/// <summary>
		///   Inserts a range of components from another stack at the given index.
		/// </summary>
		/// <param name="index">
		///   The index to insert the components.
		/// </param>
		/// <param name="stack">
		///   The component stack to add.
		/// </param>
		/// <param name="replace">
		///   If an already existing component should be replaced.
		/// </param>
		/// <returns>
		///   True if stack is not null and all componenta were added successfully, otherwise false.
		/// </returns>
		public bool InsertRange( int index, ComponentStack stack, bool replace = false )
		{
			if( stack == null || index < 0 || index > Count )
				return false;
			if( index == Count )
				return AddRange( stack, replace );

			int i = index;

			foreach( MiComponent c in stack )
			{
				if( !Insert( i, (MiComponent)c.Clone(), replace ) )
					return false;

				i++;
			}

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
		public bool Remove<T>() where T : MiComponent, new()
		{
			return Remove( IndexOf<T>() );
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
		public bool Remove( int index )
		{
			if( index < 0 || index >= Count )
				return false;

			List<string> rem = new List<string>();

			for( int i = 0; i < Count; i++ )
				if( i != index && m_components[ i ].Requires( m_components[ index ].TypeName ) )
					rem.Add( m_components[ i ].TypeName );

			m_components[ index ].Dispose();
			m_components[ index ] = null;
			m_components.RemoveAt( index );

			foreach( string s in rem )
				Remove( s );

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
		public bool Remove( string typename )
		{
			return Remove( IndexOf( typename ) );
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
		public T Release<T>() where T : MiComponent, new()
		{
			return Release( IndexOf<T>() ) as T;
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
		public MiComponent Release( int index )
		{
			if( index < 0 || index >= Count )
				return null;

			List<string> rem = new List<string>();

			for( int i = 0; i < Count; i++ )
				if( i != index && m_components[ i ].Requires( m_components[ index ].TypeName ) )
					rem.Add( m_components[ i ].TypeName );

			MiComponent result = m_components[ index ];
			m_components.RemoveAt( index );

			foreach( string s in rem )
				Remove( s );

			result.Stack = null;
			return result;
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
		public MiComponent Release( string typename )
		{
			return Release( IndexOf( typename ) );
		}

		/// <summary>
		///   Clears all components from the stack.
		/// </summary>
		public void Clear()
		{
			foreach( MiComponent c in m_components )
				c?.Dispose();

			m_components.Clear();
		}

		/// <summary>
		///   Updates the stack and components.
		/// </summary>
		/// <param name="dt">
		///   Delta time.
		/// </param>
		protected override void OnUpdate( float dt )
		{
			for( int i = 0; i < Count; i++ )
				m_components[ i ]?.Update( dt );
		}
		/// <summary>
		///   Draws the stack and components.
		/// </summary>
		/// <param name="target">
		///   The render target.
		/// </param>
		/// <param name="states">
		///   Render states.
		/// </param>
		protected override void OnDraw( RenderTarget target, RenderStates states )
		{
			for( int i = 0; i < Count; i++ )
				m_components[ i ]?.Draw( target, states );
		}

		/// <summary>
		///   Disposes of the object.
		/// </summary>
		protected override void OnDispose()
		{
			for( int i = 0; i < Count; i++ )
				m_components[ i ]?.Dispose();

			m_components.Clear();
			m_components = null;
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
						return Logger.LogReturn( "Failed loading ComponentStack: Saved object contains unregistered component name.", false, LogType.Error );

					if( Contains( type ) )
					{
						if( !Get( type ).LoadFromStream( sr ) )
							return Logger.LogReturn( "Failed loading ComponentStack: Unable to load component from stream.", false, LogType.Error );
					}
					else
					{
						MiComponent c = ComponentRegister.Manager.Create( type );

						if( !c.LoadFromStream( sr ) )
							return Logger.LogReturn( "Failed loading ComponentStack: Unable to load component from stream.", false, LogType.Error );
						if( Add( c ) )
							return Logger.LogReturn( "Failed loading ComponentStack: Unable to add component loaded from stream.", false, LogType.Error );
					}
				}
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed loading ComponentStack from stream: " + e.Message, false, LogType.Error );
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
				sw.Write( Count );

				for( int i = 0; i < Count; i++ )
				{
					sw.Write( m_components[ i ].TypeName );
					
					if( !m_components[ i ].SaveToStream( sw ) )
						return Logger.LogReturn( "Failed saving ComponentStack: Unable to save component to stream.", false, LogType.Error );
				}
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Unable to save ComponentStack to stream: " + e.Message, false, LogType.Error );
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

			Clear();

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
					return Logger.LogReturn( "Unable to load ComponentStack: Failed creating component.", false, LogType.Error );
				if( !c.LoadFromXml( e ) )
					return Logger.LogReturn( "Unable to load ComponentStack: Failed parsing component.", false, LogType.Error );
				if( !Add( c ) )
					return Logger.LogReturn( "Unable to load ComponentStack: Failed adding component.", false, LogType.Error );
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
			sb.Append( nameof( ComponentStack ) );

			sb.Append( " " );
			sb.Append( nameof( Enabled ) );
			sb.Append( "=\"" );
			sb.Append( Enabled );
			sb.AppendLine( "\"" );

			sb.Append( "        " );
			sb.Append( nameof( Visible ) );
			sb.Append( "=\"" );
			sb.Append( Visible );
			sb.AppendLine( "\">" );

			for( int i = 0; i < Count; i++ )
				sb.AppendLine( XmlLoadable.ToString( m_components[ i ], 1 ) );

			sb.Append( "</" );
			sb.Append( nameof( ComponentStack ) );
			sb.Append( ">" );

			return sb.ToString();
		}

		/// <summary>
		///   Returns a copy of this object.
		/// </summary>
		/// <returns>
		///   A copy of this object.
		/// </returns>
		public override object Clone()
		{
			return new ComponentStack( this );
		}

		/// <summary>
		///   Gets an enumerator that can be used to iterate through the components.
		/// </summary>
		/// <returns>
		///   An enumerator that can be used to iterate through the component collection.
		/// </returns>
		public IEnumerator<MiComponent> GetEnumerator()
		{
			return ( (IEnumerable<MiComponent>)m_components ).GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ( (IEnumerable)m_components ).GetEnumerator();
		}

		List<MiComponent> m_components;
	}
}
