////////////////////////////////////////////////////////////////////////////////
// MiObject.cs 
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
using System.Xml;

using SFML.Graphics;

namespace MiCore
{
	/// <summary>
	///   Base interface for ECS objects.
	/// </summary>
	public interface IMiObject : IBinarySerializable, IXmlLoadable, Drawable, IDisposable
	{
		/// <summary>
		///   If the object is enabled and should be updated.
		/// </summary>
		bool Enabled { get; set; }
		/// <summary>
		///   If the object is visible and should be drawn.
		/// </summary>
		bool Visible { get; set; }

		/// <summary>
		///   Updates the object; called once per frame.
		/// </summary>
		/// <param name="dt">
		///   Delta time.
		/// </param>
		void Update( float dt );
	}

	/// <summary>
	///   Base class for all ECS objects.
	/// </summary>
	public abstract class MiObject : BinarySerializable, IMiObject
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public MiObject()
		{
			Enabled = true;
			Visible = true;
		}
		/// <summary>
		///   Copy constructor; "_Copy" will be appended to the end of the ID.
		/// </summary>
		/// <param name="obj">
		///   The object to copy.
		/// </param>
		public MiObject( MiObject obj )
		{
			Enabled = obj.Enabled;
			Visible = obj.Visible;
		}

		/// <summary>
		///   If the object is enabled and should be updated.
		/// </summary>
		public bool Enabled { get; set; }
		/// <summary>
		///   If the object is visible and should be drawn.
		/// </summary>
		public bool Visible { get; set; }

		/// <summary>
		///   Updates the object if enabled; called once per frame.
		/// </summary>
		/// <param name="dt">
		///   Delta time.
		/// </param>
		public void Update( float dt )
		{
			if( Enabled )
				OnUpdate( dt );
		}
		/// <summary>
		///   Draws the object to the render target if visible.
		/// </summary>
		/// <param name="target">
		///   Render target.
		/// </param>
		/// <param name="states">
		///   Render states.
		/// </param>
		public void Draw( RenderTarget target, RenderStates states )
		{
			if( Visible )
				OnDraw( target, states );
		}

		/// <summary>
		///   Override this with the object logic.
		/// </summary>
		/// <param name="dt">
		///   Delta time.
		/// </param>
		protected virtual void OnUpdate( float dt )
		{ }
		/// <summary>
		///   Override this to draw the object to the render target.
		/// </summary>
		/// <param name="target">
		///   Render target.
		/// </param>
		/// <param name="states">
		///   Render states.
		/// </param>
		protected virtual void OnDraw( RenderTarget target, RenderStates states )
		{ }

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
			if( sr == null )
				return Logger.LogReturn( "Unable to load object from null stream.", false, LogType.Error );

			try
			{
				Enabled = sr.ReadBoolean();
				Visible = sr.ReadBoolean();
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Unable to load object from stream: " + e.Message, false, LogType.Error );
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
			if( sw == null )
				return Logger.LogReturn( "Unable to save object to null stream.", false, LogType.Error );

			try
			{
				sw.Write( Enabled );
				sw.Write( Visible );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Unable to save object to stream: " + e.Message, false, LogType.Error );
			}

			return true;
		}
		/// <summary>
		///   Loads the object from xml.
		/// </summary>
		/// <param name="element">
		///   The xml element.
		/// </param>
		/// <returns>
		///   True on success or false on failure.
		/// </returns>
		public virtual bool LoadFromXml( XmlElement element )
		{
			if( element == null )
				return Logger.LogReturn( "Unable to load object from null xml element.", false, LogType.Error );

			Enabled = true;
			Visible = true;

			if( element.HasAttribute( nameof( Enabled ) ) )
			{
				if( !bool.TryParse( element.GetAttribute( nameof( Enabled ) ), out bool e ) )
					return Logger.LogReturn( "Unable to load object: Failed parsing Enabled attribute.", false, LogType.Error );

				Enabled = e;
			}
			if( element.HasAttribute( nameof( Visible ) ) )
			{
				if( !bool.TryParse( element.GetAttribute( nameof( Visible ) ), out bool v ) )
					return Logger.LogReturn( "Unable to load object: Failed parsing Visible attribute.", false, LogType.Error );

				Visible = v;
			}

			return true;
		}

		/// <summary>
		///   Disposes of the object.
		/// </summary>
		public abstract void Dispose();
	}
}
