////////////////////////////////////////////////////////////////////////////////
// NodeTree.cs 
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

namespace MiCore
{
	/// <summary>
	///   Base class for tree-based node objects.
	/// </summary>
	public abstract class MiNode<T> : MiObject, INamable, IIdentifiable<string>, IEnumerable<T>, IEquatable<T>
		where T : MiNode<T>, new()
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public MiNode()
		:	base()
		{
			ID         = Identifiable.NewStringID( nameof( MiNode<T> ) );
			Name       = Naming.NewName( nameof( MiNode<T> ) );
			Parent     = null;
			m_children = new List<T>();
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <remarks>
		///   The coppied ID will have "_copy" appended.
		/// </remarks>
		/// <param name="n">
		///   The object to copy.
		/// </param>
		public MiNode( MiNode<T> n )
		:	base( n )
		{
			ID     = new string( n.ID.ToCharArray() ) + "_copy";
			Name   = new string( n.Name.ToCharArray() );
			Parent = n.Parent;

			m_children = n.HasChildren ? new List<T>( n.ChildCount ) : new List<T>();

			foreach( T e in n )
				if( !AddChild( (T)e.Clone() ) )
					throw new InvalidOperationException( "Failed adding cloned node as child." );

			if( n.Parent != null )
				if( !n.Parent.AddChild( (T)this ) )
					throw new InvalidOperationException( "Failed adding coppied node to parent." );
		}
		/// <summary>
		///   Constructor setting the ID and optionally the name and parent.
		/// </summary>
		/// <param name="id">
		///   Node ID.
		/// </param>
		/// <param name="name">
		///   Node name.
		/// </param>
		/// <param name="parent">
		///   The parent of the node.
		/// </param>
		public MiNode( string id, string name = null, T parent = null )
		:	base()
		{
			ID         = id;
			Name       = name;
			Parent     = parent;
			m_children = new List<T>();
		}

		/// <summary>
		///   The object ID.
		/// </summary>
		public string ID
		{
			get { return m_id; }
			set { m_id = string.IsNullOrWhiteSpace( value ) ? Identifiable.NewStringID( nameof( MiNode<T> ) ) : Identifiable.AsValid( value ); }
		}
		/// <summary>
		///   The object name.
		/// </summary>
		public string Name
		{
			get { return m_name; }
			set { m_name = string.IsNullOrWhiteSpace( value ) ? Naming.NewName( nameof( MiNode<T> ) ) : Naming.AsValid( value, ' ' ); }
		}

		/// <summary>
		///   The owning parent.
		/// </summary>
		/// <exception cref="ArgumentException">
		///   If trying to set parent to `this` or to a child node.
		/// </exception>
		public T Parent
		{
			get { return m_parent; }
			private set
			{
				if( value == this )
					throw new ArgumentException();

				if( HasChildren && value != null )
					foreach( T c in AllChildren )
						if( c == value )
							throw new ArgumentException();

				m_parent = value;
			}
		}
		/// <summary>
		///   An array containing a bottom-up list of each parent node.
		/// </summary>
		public T[] Parents
		{
			get
			{
				List<T> parents = new List<T>();

				for( T t = Parent; t != null; t = t.Parent )
					parents.Add( t );

				return parents.ToArray();
			}
		}

		/// <summary>
		///   Gets an array containing child nodes.
		/// </summary>
		public T[] Children
		{
			get { return m_children.ToArray(); }
		}
		/// <summary>
		///   Gets an array containing all child nodes and their children.
		/// </summary>
		public T[] AllChildren
		{
			get			
			{
				if( !HasChildren )
					return new T[ 0 ];

				List<T> children = new List<T>( Children );

				foreach( T child in this )
				{
					if( !child.HasChildren )
						continue;

					List<T> c = new List<T>( child.AllChildren );
					children.AddRange( c );
				}

				return children.ToArray();
			}
		}

		/// <summary>
		///   If the node has a parent.
		/// </summary>
		public bool HasParent
		{
			get { return Parent != null; }
		}

		/// <summary>
		///   If the node has any children.
		/// </summary>
		public bool HasChildren
		{
			get { return ChildCount > 0; }
		}
		/// <summary>
		///   The amount of immediate children the node has.
		/// </summary>
		public int ChildCount
		{
			get { return m_children?.Count ?? 0; }
		}
		/// <summary>
		///   The total amount of child entities owned by the node and its children.
		/// </summary>
		public int TotalChildCount
		{
			get
			{
				int counter = ChildCount;

				foreach( T c in m_children )
					counter += c.ChildCount;

				return counter;
			}
		}

		/// <summary>
		///   Checks up the tree if the node is a parent or a parent of a parent.
		/// </summary>
		/// <param name="parent">
		///   The node to check.
		/// </param>
		/// <returns>
		///   True if the node is a parent or a parent of a parent, otherwise false.
		/// </returns>
		public bool IsParent( T parent )
		{
			if( parent != null && parent != this )
				foreach( T p in Parents )
					if( p == parent )
						return true;

			return false;
		}

		/// <summary>
		///   If the node contains the given child.
		/// </summary>
		/// <param name="e">
		///   The child node.
		/// </param>
		/// <param name="recursive">
		///   If children of children should be checked.
		/// </param>
		/// <returns>
		///   True if the node contains the child, otherwise false.
		/// </returns>
		public bool HasChild( T e, bool recursive = false )
		{
			if( !HasChildren || e == null )
				return false;

			foreach( T e1 in m_children )
			{
				if( e1.Equals( e ) )
					return true;

				if( recursive )
					foreach( T e2 in e1 )
						if( e2.HasChild( e, true ) )
							return true;
			}

			return false;
		}
		/// <summary>
		///   If the node contains a child with the given ID.
		/// </summary>
		/// <param name="id">
		///   The node ID.
		/// </param>
		/// <param name="recursive">
		///   If children of children should be checked.
		/// </param>
		/// <returns>
		///   True if the node contains a child with the given ID, otherwise false.
		/// </returns>
		public bool HasChild( string id, bool recursive = false )
		{
			if( !HasChildren || !Identifiable.IsValid( id ) )
				return false;

			foreach( T e1 in m_children )
			{
				if( e1.ID.Equals( id ) )
					return true;

				if( recursive )
					foreach( T e2 in e1 )
						if( e2.HasChild( id, true ) )
							return true;
			}

			return false;
		}
		/// <summary>
		///   If the node contains a child with the given name.
		/// </summary>
		/// <param name="name">
		///   The node name.
		/// </param>
		/// <param name="recursive">
		///   If children of children should be checked.
		/// </param>
		/// <returns>
		///   True if the node contains a child with the given name, otherwise false.
		/// </returns>
		public bool HasChildNamed( string name, bool recursive = false )
		{
			if( !HasChildren || !Naming.IsValid( name ) )
				return false;

			foreach( T e1 in m_children )
			{
				if( e1.Name.Equals( name ) )
					return true;

				if( recursive )
					foreach( T e2 in e1 )
						if( e2.HasChildNamed( name, true ) )
							return true;
			}

			return false;
		}
		
		/// <summary>
		///   Gets the child node at the given index.
		/// </summary>
		/// <param name="index">
		///   node index.
		/// </param>
		/// <returns>
		///   The node at the given index if it exists, otherwise null.
		/// </returns>
		public T GetChild( int index )
		{
			if( index < 0 || index >= ChildCount )
				return null;

			return m_children[ index ];
		}
		/// <summary>
		///   Gets the child node with the given ID.
		/// </summary>
		/// <param name="id">
		///   node ID.
		/// </param>
		/// <param name="recursive">
		///   Should child entities be searched recursively.
		/// </param>
		/// <returns>
		///   The node with the given ID if it exists, otherwise null.
		/// </returns>
		public T GetChild( string id, bool recursive = false )
		{
			if( !Identifiable.IsValid( id ) || !HasChildren )
				return null;

			foreach( T e1 in m_children )
			{
				if( e1.ID.Equals( id ) )
					return e1;

				if( recursive )
				{
					foreach( T e2 in e1 )
					{
						T c = e2.GetChild( id, true );

						if( c != null )
							return c;
					}
				}
			}

			return null;
		}
		/// <summary>
		///   Gets the first child node with the given name.
		/// </summary>
		/// <param name="name">
		///   node name.
		/// </param>
		/// <param name="recursive">
		///   Should child entities be searched recursively.
		/// </param>
		/// <returns>
		///   The node with the given name if it exists, otherwise null.
		/// </returns>
		public T GetFirstChild( string name, bool recursive = false )
		{
			if( !Naming.IsValid( name ) || !HasChildren )
				return null;

			foreach( T e1 in m_children )
			{
				if( e1.Name.Equals( name ) )
					return e1;

				if( recursive )
				{
					foreach( T e2 in e1 )
					{
						T c = e2.GetFirstChild( name, true );

						if( c != null )
							return c;
					}
				}
			}

			return null;
		}
		/// <summary>
		///   Gets the last child node with the given name.
		/// </summary>
		/// <param name="name">
		///   node name.
		/// </param>
		/// <param name="recursive">
		///   Should child entities be searched recursively.
		/// </param>
		/// <returns>
		///   The node with the given name if it exists, otherwise null.
		/// </returns>
		public T GetLastChild( string name, bool recursive = false )
		{
			if( !Naming.IsValid( name ) || !HasChildren )
				return null;

			T ent = null;

			foreach( T e1 in m_children )
			{
				if( e1.Name.Equals( name ) )
					ent = e1;

				if( recursive )
				{
					foreach( T e2 in e1 )
					{
						T c = e2.GetFirstChild( name, true );

						if( c != null )
							ent = c;
					}
				}
			}

			return ent;
		}

		/// <summary>
		///   Gets all child entities with the given name.
		/// </summary>
		/// <param name="name">
		///   node name.
		/// </param>
		/// <param name="recursive">
		///   Should child entities be searched recursively.
		/// </param>
		/// <returns>
		///   All child entities with the given name or an empty array if there are no matches.
		/// </returns>
		public T[] GetChildren( string name, bool recursive = false )
		{
			List<T> children = new List<T>();

			if( !Naming.IsValid( name ) || !HasChildren )
				return children.ToArray();

			foreach( T e1 in m_children )
			{
				if( e1.Name.Equals( name ) )
					children.Add( e1 );

				if( !recursive )
					continue;
				
				foreach( T e2 in e1 )
				{
					T[] c = e2.GetChildren( name, true );
					
					if( c != null )
						foreach( T ce in c )
							children.Add( ce );
				}
			}

			return children.ToArray();
		}

		/// <summary>
		///   Sets an node as a child of this object.
		/// </summary>
		/// <param name="e">
		///   The node to add as child.
		/// </param>
		/// <param name="replace">
		///   If an node with the same ID should be replaced?
		/// </param>
		/// <returns>
		///   True if e is already a child or if it was added successfully, false if e is null, 
		///   this, or  e is parent or .
		/// </returns>
		public bool AddChild( T e, bool replace = false )
		{
			if( e == null || e == this || IsParent( e ) )
				return false;
			if( HasChild( e ) )
				return AddChild( ReleaseChild( e ) );

			if( HasChild( e.ID ) )
			{
				if( !replace )
					return false;

				RemoveChild( e.ID );
			}

			if( e.Parent != null )
				e.Parent.ReleaseChild( e.ID );

			e.Parent = (T)this;
			m_children.Add( e );
			return true;
		}
		/// <summary>
		///   Adds multiple child nodes.
		/// </summary>
		/// <param name="nodes">
		///   Nodes to add as children.
		/// </param>
		/// <returns>
		///   True if all nodes were added successfully, otherwise false. Will also return false
		///   if <paramref name="nodes"/> is null.
		/// </returns>
		public bool AddChildren( params T[] nodes )
		{
			if( nodes == null )
				return false;

			foreach( T n in nodes )
				if( !AddChild( n ) )
					return false;

			return true;
		}

		/// <summary>
		///   Removes and disposes of the node if it is a child.
		/// </summary>
		/// <param name="e">
		///   The child node.
		/// </param>
		/// <param name="recursive">
		///   Should child entities be searched recursively.
		/// </param>
		/// <returns>
		///   True if e was a child and was removed successfully, otherwise false.
		/// </returns>
		public bool RemoveChild( T e, bool recursive = false )
		{
			if( !HasChildren || e == null )
				return false;

			for( int i = 0; i < ChildCount; i++ )
			{
				if( m_children[ i ].Equals( e ) )
					return RemoveChild( i );

				if( recursive )
					for( int j = 0; j < m_children[ i ].ChildCount; i++ )
						if( m_children[ i ].RemoveChild( e, true ) )
							return true;
			}

			return false;
		}
		/// <summary>
		///   Removes and disposes of the child node at the given index.
		/// </summary>
		/// <param name="index">
		///   The child index.
		/// </param>
		/// <returns>
		///   True if a child existed at the given index and was removed successfully, otherwise
		///   false.
		/// </returns>
		public bool RemoveChild( int index )
		{
			if( index < 0 || index >= ChildCount )
				return false;

			m_children[ index ].Dispose();
			m_children.RemoveAt( index );
			return true;
		}
		/// <summary>
		///   Removes the child with the given ID.
		/// </summary>
		/// <param name="id">
		///   The child ID.
		/// </param>
		/// <param name="recursive">
		///   Should child entities be searched recursively.
		/// </param>
		/// <returns>
		///   True if a child existed and was removed successfully, otherwise false.
		/// </returns>
		public bool RemoveChild( string id, bool recursive = false )
		{
			if( !Identifiable.IsValid( id ) || !HasChildren )
				return false;

			for( int i = 0; i < ChildCount; i++ )
			{
				if( m_children[ i ].ID.Equals( id ) )
				{
					m_children[ i ].Dispose();
					m_children.RemoveAt( i );
					return true;
				}

				if( recursive )
					if( m_children[ i ].RemoveChild( id, true ) )
						return true;
			}

			return false;
		}
		/// <summary>
		///   Removes and disposes of all child entities.
		/// </summary>
		public void RemoveAllChildren()
		{
			foreach( T c in m_children )
				c.Dispose();

			m_children.Clear();
		}

		/// <summary>
		///   Removes and returns the node if it is a child.
		/// </summary>
		/// <param name="ent">
		///   The child node.
		/// </param>
		/// <param name="recursive">
		///   Should child entities be searched recursively.
		/// </param>
		/// <returns>
		///   The removed node if it exists, otherwise null.
		/// </returns>
		public T ReleaseChild( T ent, bool recursive = false )
		{
			if( !HasChildren || ent == null )
				return null;

			for( int i = 0; i < ChildCount; i++ )
			{
				if( m_children[ i ].Equals( ent ) )
					return ReleaseChild( i );

				if( recursive )
				{
					for( int j = 0; j < m_children[ i ].ChildCount; i++ )
					{
						T e = m_children[ i ].ReleaseChild( ent, true );

						if( e != null )
							return e;
					}
				}
			}

			return null;
		}
		/// <summary>
		///   Removes and returns the child node at the given index.
		/// </summary>
		/// <param name="index">
		///   The child index.
		/// </param>
		/// <returns>
		///   The removed node if it exists, otherwise null.
		/// </returns>
		public T ReleaseChild( int index )
		{
			if( index < 0 || index >= ChildCount )
				return null;

			T e = m_children[ index ];
			m_children.RemoveAt( index );
			e.Parent = null;
			return e;
		}
		/// <summary>
		///   Removes and returns the child node with the given ID.
		/// </summary>
		/// <param name="id">
		///   The child ID.
		/// </param>
		/// <param name="recursive">
		///   Should child entities be searched recursively.
		/// </param>
		/// <returns>
		///   The removed node if it exists, otherwise null.
		/// </returns>
		public T ReleaseChild( string id, bool recursive = false )
		{
			if( !Identifiable.IsValid( id ) || !HasChildren )
				return null;

			for( int i = 0; i < ChildCount; i++ )
			{
				if( m_children[ i ].ID.Equals( id ) )
				{
					T e = m_children[ i ];
					m_children.RemoveAt( i );
					e.Parent = null;
					return e;
				}

				if( recursive )
				{
					T e = m_children[ i ].ReleaseChild( id, true );

					if( e != null )
						return e;
				}
			}

			return null;
		}
		/// <summary>
		///   Removes and returns all child entities.
		/// </summary>
		/// <returns>
		///   An array containing the removed child entities.
		/// </returns>
		public T[] ReleaseAllChildren()
		{
			T[] ents = new T[ ChildCount ];

			for( int i = 0; i < ChildCount; i++ )
			{
				m_children[ i ].Parent = null;
				ents[ i ] = m_children[ i ];
			}

			m_children.Clear();
			return ents;
		}

		/// <summary>
		///   Returns the index of the given child node.
		/// </summary>
		/// <param name="e">
		///   The child node.
		/// </param>
		/// <returns>
		///   The index of the child node if it exists, otherwise -1.
		/// </returns>
		public int ChildIndex( T e )
		{
			if( e != null )
				for( int i = 0; i < ChildCount; i++ )
					if( m_children[ i ].Equals( e ) )
						return i;

			return -1;
		}
		/// <summary>
		///   Returns the index of the given child node.
		/// </summary>
		/// <param name="id">
		///   The child node id.
		/// </param>
		/// <returns>
		///   The index of the child node if it exists, otherwise -1.
		/// </returns>
		public int ChildIndex( string id )
		{
			if( Identifiable.IsValid( id ) )
				for( int i = 0; i < ChildCount; i++ )
					if( m_children[ i ].ID.Equals( id ) )
						return i;

			return -1;
		}
		/// <summary>
		///   Returns the first index of the child with the given name.
		/// </summary>
		/// <param name="name">
		///   The child node name.
		/// </param>
		/// <returns>
		///   The index of the child node if it exists, otherwise -1.
		/// </returns>
		public int FirstChildIndex( string name )
		{
			if( Naming.IsValid( name ) )
				for( int i = 0; i < ChildCount; i++ )
					if( m_children[ i ].Name.Equals( name ) )
						return i;

			return -1;
		}
		/// <summary>
		///   Returns the last index of the child with the given name.
		/// </summary>
		/// <param name="name">
		///   The child node name.
		/// </param>
		/// <returns>
		///   The index of the child node if it exists, otherwise -1.
		/// </returns>
		public int LastChildIndex( string name )
		{
			int index = -1;

			if( Naming.IsValid( name ) )
				for( int i = 0; i < ChildCount; i++ )
					if( m_children[ i ].Name.Equals( name ) )
						index = i;

			return index;
		}

		/// <summary>
		///   Attempts to deserialize the node and its children from the stream.
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
				m_children = new List<T>( count );

				for( int i = 0; i < count; i++ )
				{
					T n = new T();

					if( !n.LoadFromStream( sr ) )
						return Logger.LogReturn( "Failed loading node: Unable to load child from stream.", false, LogType.Error );
					if( !AddChild( n ) )
						return Logger.LogReturn( "Failed loading node: Unable to add loaded child.", false, LogType.Error );
				}
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed loading node from stream: " + e.Message, false, LogType.Error );
			}

			return true;
		}
		/// <summary>
		///   Attempts to serialize the node and its chidren to the stream.
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
				sw.Write( ChildCount );

				for( int i = 0; i < ChildCount; i++ )
					if( !m_children[ i ].SaveToStream( sw ) )
						return Logger.LogReturn( "Failed saving node: Unable to save child to stream.", false, LogType.Error );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Unable to save node to stream: " + e.Message, false, LogType.Error );
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

			RemoveAllChildren();
			XmlElement child = element[ nameof( Children ) ];

			if( child != null )
			{
				foreach( XmlNode node in child.ChildNodes )
				{
					if( node.NodeType != XmlNodeType.Element )
						continue;

					XmlElement xe = (XmlElement)node;
					T e = new T();

					if( !e.LoadFromXml( xe ) )
						return Logger.LogReturn( "Failed loading node: Unable to load child from xml.", false, LogType.Error );
					if( !AddChild( e ) )
						return Logger.LogReturn( "Failed loading node: Unable to add loaded child.", false, LogType.Error );
				}
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

			sb.Append( "<MiNode " );

			sb.Append( nameof( Enabled ) );
			sb.Append( "=\"" );
			sb.Append( Enabled );
			sb.AppendLine( "\"" );

			sb.Append( "        " );
			sb.Append( nameof( Visible ) );
			sb.Append( "=\"" );
			sb.Append( Enabled );
			sb.AppendLine( "\"" );

			sb.Append( "        " );
			sb.Append( nameof( ID ) );
			sb.Append( "=\"" );
			sb.Append( ID );
			sb.AppendLine( "\"" );

			sb.Append( "        " );
			sb.Append( nameof( Name ) );
			sb.Append( "=\"" );
			sb.Append( Name );
			sb.AppendLine( "\">" );

			if( HasChildren )
			{
				sb.Append( "\t<" );
				sb.Append( nameof( Children ) );
				sb.AppendLine( ">" );

				foreach( T t in m_children )
					sb.AppendLine( XmlLoadable.ToString( t, 2 ) );

				sb.Append( "\t</" );
				sb.Append( nameof( Children ) );
				sb.AppendLine( ">" );
			}

			sb.Append( "</MiNode>" );
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
		public virtual bool Equals( T other )
		{
			if( !base.Equals( other ) || !Name.Equals( other.Name ) || Parent != other.Parent ||
				ChildCount != other.ChildCount )
				return false;

			for( int i = 0; i < ChildCount; i++ )
				if( !m_children[ i ].Equals( other.m_children[ i ] ) )
					return false;

			return true;
		}

		/// <summary>
		///   Disposes of the object.
		/// </summary>
		protected override void OnDispose()
		{
			RemoveAllChildren();
		}

		/// <summary>
		///   Gets an enumerator that can be used to iterate through the child nodes.
		/// </summary>
		/// <returns>
		///   An enumerator that can be used to iterate through the child collection.
		/// </returns>
		public IEnumerator<T> GetEnumerator()
		{
			return ( (IEnumerable<T>)m_children ).GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ( (IEnumerable)m_children ).GetEnumerator();
		}

		string  m_id, 
		        m_name;
		T       m_parent;
		List<T> m_children;
	}
}
