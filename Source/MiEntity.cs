////////////////////////////////////////////////////////////////////////////////
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
			Components = new ComponentStack( this );
			Window     = null;
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
			Components = new ComponentStack( ent.Components );
			Components.Parent = this;
			Window = ent.Window;
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
			Components = new ComponentStack( this );
			Window     = window;
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
			Components = new ComponentStack( this );
			Window     = window;
		}
		/// <summary>
		///   Constructor setting the object ID, name and optionally the target render window.
		/// </summary>
		/// <param name="id">
		///   The object ID.
		/// </param>
		/// <param name="name">
		///   The object name.
		/// </param>
		/// <param name="window">
		///   The target window.
		/// </param>
		public MiEntity( string id, string name, RenderWindow window = null )
		:	base( id, name )
		{
			Components = new ComponentStack( this );
			Window     = window;
		}

		/// <summary>
		///   Entity component stack.
		/// </summary>
		public ComponentStack Components
		{
			get; private set;
		}
		/// <summary>
		///   A reference to the target render window.
		/// </summary>
		public RenderWindow Window
		{
			get { return m_window; }
			set
			{
				m_window = value;

				if( HasChildren )
					foreach( MiEntity e in Children )
						if( e != null )
							e.Window = value;
			}
		}

		/// <summary>
		///   To be called on TextEntered event. Calls <see cref="MiComponent.OnTextEntered(TextEventArgs)"/>
		///   for all enabled components for this entity and all child entities.
		/// </summary>
		/// <param name="e">
		///   The event args.
		/// </param>
		public void TextEntered( TextEventArgs e )
		{
			if( e != null && Enabled )
			{
				foreach( MiComponent c in Components )
					c.TextEntered( e );

				if( HasChildren )
					foreach( MiEntity en in AllChildren )
						foreach( MiComponent c in en.Components )
							c.TextEntered( e );
			}
		}

		/// <summary>
		///   Updates the component stack and children.
		/// </summary>
		/// <param name="dt">
		///   Delta time.
		/// </param>
		protected override void OnUpdate( float dt )
		{
			Components.Parent = this;
			Components.Update( dt );

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
			Components.Parent = this;
			Components.Draw( target, states );

			foreach( MiEntity e in this )
				e.Draw( target, states );
		}

		/// <summary>
		///   Disposes of the object and children.
		/// </summary>
		protected override void OnDispose()
		{
			Components.Dispose();
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
			if( !Components.LoadFromStream( sr ) )
				return Logger.LogReturn( "Failed loading entity: Unable to load components from stream.", false, LogType.Error );

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
			if( !Components.SaveToStream( sw ) )
				return Logger.LogReturn( "Failed saving entity: Unable to save components.", false, LogType.Error );

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

			XmlElement comp = element[ nameof( ComponentStack ) ];

			if( comp == null )
				return Logger.LogReturn( "Failed loading entity: No MiComponentStack element.", false, LogType.Error );
			if( !Components.LoadFromXml( comp ) )
				return Logger.LogReturn( "Failed loading entity: Unable to load MiComponentStack from xml.", false, LogType.Error );

			return true;
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
			return base.Equals( other ) && Components.Equals( other.Components );
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

			sb.AppendLine( XmlLoadable.ToString( Components, 1 ) );

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
	}
}
