////////////////////////////////////////////////////////////////////////////////
// XmlTest.cs 
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
using System.Xml;

using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace MiCore.Test
{
	public class XmlTest : TestModule
	{
		protected override bool OnTest()
		{
			Logger.Log( "Running Xml Test..." );

			bool result = true;

			if( !Vec2fTest() )
				result = false;
			if( !Vec2iTest() )
				result = false;
			if( !Vec2uTest() )
				result = false;
			if( !IRectTest() )
				result = false;
			if( !FRectTest() )
				result = false;
			if( !ColorTest() )
				result = false;
			if( !VideoModeTest() )
				result = false;

			return Logger.LogReturn( result ? "Xml test succeeded!." : "Xml test failed!.", result );
		}

		bool Vec2fTest()
		{
			Logger.Log( "Running Vector2f Test..." );

			Vector2f vec = new Vector2f( 13, 25 );
			XmlDocument doc = new XmlDocument();

			try
			{
				// Load xml document from `Xml.ToString`.
				doc.LoadXml( Xml.Header + "\r\n" + Xml.ToString( vec ) );

				// Try parsing the document element back to a vector and check for equality.
				if( !vec.Equals( Xml.ToVec2f( doc.DocumentElement ) ) )
					return Logger.LogReturn( "Failed! Loaded Vector2f has incorrect values.", false );
			}
			catch( Exception e )
			{
				// If we get here, `doc.LoadXml` or `Xml.ToVec2f` failed.
				return Logger.LogReturn( "Failed! " + e.Message, false );
			}

			return Logger.LogReturn( "Success!", true );
		}
		bool Vec2iTest()
		{
			Logger.Log( "Running Vector2i Test..." );

			Vector2i vec = new Vector2i( 13, 25 );
			XmlDocument doc = new XmlDocument();

			try
			{
				// Load xml document from `Xml.ToString`.
				doc.LoadXml( Xml.Header + "\r\n" + Xml.ToString( vec ) );

				// Try parsing the document element back to a vector and check for equality.
				if( !vec.Equals( Xml.ToVec2i( doc.DocumentElement ) ) )
					return Logger.LogReturn( "Failed! Loaded Vector2i has incorrect values.", false );
			}
			catch( Exception e )
			{
				// If we get here, `doc.LoadXml` or `Xml.ToVec2i` failed.
				return Logger.LogReturn( "Failed! " + e.Message, false );
			}

			return Logger.LogReturn( "Success!", true );
		}
		bool Vec2uTest()
		{
			Logger.Log( "Running Vector2u Test..." );

			Vector2u vec = new Vector2u( 13, 25 );
			XmlDocument doc = new XmlDocument();

			try
			{
				// Load xml document from `Xml.ToString`.
				doc.LoadXml( Xml.Header + "\r\n" + Xml.ToString( vec ) );

				// Try parsing the document element back to a vector and check for equality.
				if( !vec.Equals( Xml.ToVec2u( doc.DocumentElement ) ) )
					return Logger.LogReturn( "Failed! Loaded Vector2u has incorrect values.", false );
			}
			catch( Exception e )
			{
				// If we get here, `doc.LoadXml` or `Xml.ToVec2u` failed.
				return Logger.LogReturn( "Failed! " + e.Message, false );
			}

			return Logger.LogReturn( "Success!", true );
		}

		bool IRectTest()
		{
			Logger.Log( "Running IntRect Test..." );

			IntRect rect = new IntRect( 13, 25, 45, 56 );
			XmlDocument doc = new XmlDocument();

			try
			{
				// Load xml document from `Xml.ToString`.
				doc.LoadXml( Xml.Header + "\r\n" + Xml.ToString( rect ) );

				// Try parsing the document element back to a rect and check for equality.
				if( !rect.Equals( Xml.ToIRect( doc.DocumentElement ) ) )
					return Logger.LogReturn( "Failed! Loaded IntRect has incorrect values.", false );
			}
			catch( Exception e )
			{
				// If we get here, `doc.LoadXml` or `Xml.ToIRect` failed.
				return Logger.LogReturn( "Failed! " + e.Message, false );
			}

			return Logger.LogReturn( "Success!", true );
		}
		bool FRectTest()
		{
			Logger.Log( "Running FloatRect Test..." );

			FloatRect rect = new FloatRect( 13, 25, 45, 56 );
			XmlDocument doc = new XmlDocument();

			try
			{
				// Load xml document from `Xml.ToString`.
				doc.LoadXml( Xml.Header + "\r\n" + Xml.ToString( rect ) );

				// Try parsing the document element back to a rect and check for equality.
				if( !rect.Equals( Xml.ToFRect( doc.DocumentElement ) ) )
					return Logger.LogReturn( "Failed! Loaded FloatRect has incorrect values.", false );
			}
			catch( Exception e )
			{
				// If we get here, `doc.LoadXml` or `Xml.ToFRect` failed.
				return Logger.LogReturn( "Failed! " + e.Message, false );
			}

			return Logger.LogReturn( "Success!", true );
		}

		bool ColorTest()
		{
			Logger.Log( "Running Color Test..." );

			Color col = new Color( 13, 25, 45, 56 );
			XmlDocument doc = new XmlDocument();

			try
			{
				// Load xml document from `Xml.ToString`.
				doc.LoadXml( Xml.Header + "\r\n" + Xml.ToString( col ) );

				// Try parsing the document element back to a color and check for equality.
				if( !col.Equals( Xml.ToColor( doc.DocumentElement ) ) )
					return Logger.LogReturn( "Failed! Loaded Color has incorrect values.", false );
			}
			catch( Exception e )
			{
				// If we get here, `doc.LoadXml` or `Xml.ToColor` failed.
				return Logger.LogReturn( "Failed! " + e.Message, false );
			}

			return Logger.LogReturn( "Success!", true );
		}
		bool VideoModeTest()
		{
			Logger.Log( "Running VideoMode Test..." );

			VideoMode vm = new VideoMode( 800, 600, 32 );
			XmlDocument doc = new XmlDocument();

			try
			{
				// Load xml document from `Xml.ToString`.
				doc.LoadXml( Xml.Header + "\r\n" + Xml.ToString( vm ) );

				// Try parsing the document element back to a video mode and check for equality.
				if( !vm.Equals( Xml.ToVideoMode( doc.DocumentElement ) ) )
					return Logger.LogReturn( "Failed! Loaded VideoMode has incorrect values.", false );
			}
			catch( Exception e )
			{
				// If we get here, `doc.LoadXml` or `Xml.ToVideoMode` failed.
				return Logger.LogReturn( "Failed! " + e.Message, false );
			}

			return Logger.LogReturn( "Success!", true );
		}
	}
}
