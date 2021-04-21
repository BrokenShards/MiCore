////////////////////////////////////////////////////////////////////////////////
// Xml.cs 
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
using System.Text;
using System.Xml;

using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace MiCore
{
	/// <summary>
	///   Contains xml helper functionality.
	/// </summary>
	public static class Xml
	{
		/// <summary>
		///   Standard xml header.
		/// </summary>
		public const string Header = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";

		/// <summary>
		///   Gets the vector xml string.
		/// </summary>
		/// <param name="vec">
		///   The vector.
		/// </param>
		/// <param name="name">
		///   Xml element name.
		/// </param>
		/// <param name="indent">
		///   Indentation level.
		/// </param>
		/// <returns>
		///   The given vector as an xml string.
		/// </returns>
		public static string ToString( Vector2f vec, string name = null, uint indent = 0 )
		{
			if( !Identifiable.IsValid( name ) )
				name = nameof( Vector2f );

			string spaces = " ";

			for( int i = 0; i < name.Length; i++ )
				spaces += ' ';

			StringBuilder sb = new StringBuilder();

			sb.Append( "<" );
			sb.Append( name );
			sb.Append( " " + nameof( Vector2f.X ) + "=\"" );
			sb.Append( vec.X );
			sb.AppendLine( "\"" );
			sb.Append( spaces );
			sb.Append( " " + nameof( Vector2f.Y ) + "=\"" );
			sb.Append( vec.Y );
			sb.AppendLine( "\"/>" );

			return Indent( sb.ToString(), indent );
		}
		/// <summary>
		///   Gets the vector xml string.
		/// </summary>
		/// <param name="vec">
		///   The vector.
		/// </param>
		/// <param name="name">
		///   Xml element name.
		/// </param>
		/// <param name="indent">
		///   Indentation level.
		/// </param>
		/// <returns>
		///   The given vector as an xml string.
		/// </returns>
		public static string ToString( Vector2i vec, string name = null, uint indent = 0 )
		{
			if( !Identifiable.IsValid( name ) )
				name = nameof( Vector2i );

			string spaces = " ";

			for( int i = 0; i < name.Length; i++ )
				spaces += ' ';

			StringBuilder sb = new StringBuilder();

			sb.Append( "<" );
			sb.Append( name );
			sb.Append( " " + nameof( Vector2i.X ) + "=\"" );
			sb.Append( vec.X );
			sb.AppendLine( "\"" );
			sb.Append( spaces );
			sb.Append( " " + nameof( Vector2i.Y ) + "=\"" );
			sb.Append( vec.Y );
			sb.AppendLine( "\"/>" );

			return Indent( sb.ToString(), indent );
		}
		/// <summary>
		///   Gets the vector xml string.
		/// </summary>
		/// <param name="vec">
		///   The vector.
		/// </param>
		/// <param name="name">
		///   Xml element name.
		/// </param>
		/// <param name="indent">
		///   Indentation level.
		/// </param>
		/// <returns>
		///   The given vector as an xml string.
		/// </returns>
		public static string ToString( Vector2u vec, string name = null, uint indent = 0 )
		{
			if( !Identifiable.IsValid( name ) )
				name = nameof( Vector2u );

			string spaces = " ";

			for( int i = 0; i < name.Length; i++ )
				spaces += ' ';

			StringBuilder sb = new StringBuilder();

			sb.Append( "<" );
			sb.Append( name );
			sb.Append( " " + nameof( Vector2u.X ) + "=\"" );
			sb.Append( vec.X );
			sb.AppendLine( "\"" );
			sb.Append( spaces );
			sb.Append( " " + nameof( Vector2u.Y ) + "=\"" );
			sb.Append( vec.Y );
			sb.AppendLine( "\"/>" );

			return Indent( sb.ToString(), indent );
		}

		/// <summary>
		///   Gets the rect xml string.
		/// </summary>
		/// <param name="rect">
		///   The rect.
		/// </param>
		/// <param name="name">
		///   Xml element name.
		/// </param>
		/// <param name="indent">
		///   Indentation level.
		/// </param>
		/// <returns>
		///   The given rect as an xml string.
		/// </returns>
		public static string ToString( FloatRect rect, string name = null, uint indent = 0 )
		{
			if( !Identifiable.IsValid( name ) )
				name = nameof( FloatRect );

			string spaces = " ";

			for( int i = 0; i < name.Length; i++ )
				spaces += ' ';

			StringBuilder sb = new StringBuilder();

			sb.Append( "<" );
			sb.Append( name );
			sb.Append( " " + nameof( FloatRect.Left ) + "=\"" );
			sb.Append( rect.Left );
			sb.AppendLine( "\"" );

			sb.Append( spaces );
			sb.Append( " " + nameof( FloatRect.Top ) + "=\"" );
			sb.Append( rect.Top );
			sb.AppendLine( "\"" );

			sb.Append( spaces );
			sb.Append( " " + nameof( FloatRect.Width ) + "=\"" );
			sb.Append( rect.Width );
			sb.AppendLine( "\"" );

			sb.Append( spaces );
			sb.Append( " " + nameof( FloatRect.Height ) + "=\"" );
			sb.Append( rect.Height );
			sb.AppendLine( "\"/>" );

			return Indent( sb.ToString(), indent );
		}
		/// <summary>
		///   Gets the rect xml string.
		/// </summary>
		/// <param name="rect">
		///   The rect.
		/// </param>
		/// <param name="name">
		///   Xml element name.
		/// </param>
		/// <param name="indent">
		///   Indentation level.
		/// </param>
		/// <returns>
		///   The given rect as an xml string.
		/// </returns>
		public static string ToString( IntRect rect, string name = null, uint indent = 0 )
		{
			if( !Identifiable.IsValid( name ) )
				name = nameof( IntRect );

			string spaces = " ";

			for( int i = 0; i < name.Length; i++ )
				spaces += ' ';

			StringBuilder sb = new StringBuilder();

			sb.Append( "<" );
			sb.Append( name );
			sb.Append( " " + nameof( IntRect.Left ) + "=\"" );
			sb.Append( rect.Left );
			sb.AppendLine( "\"" );

			sb.Append( spaces );
			sb.Append( " " + nameof( IntRect.Top ) + "=\"" );
			sb.Append( rect.Top );
			sb.AppendLine( "\"" );

			sb.Append( spaces );
			sb.Append( " " + nameof( IntRect.Width ) + "=\"" );
			sb.Append( rect.Width );
			sb.AppendLine( "\"" );

			sb.Append( spaces );
			sb.Append( " " + nameof( IntRect.Height ) + "=\"" );
			sb.Append( rect.Height );
			sb.AppendLine( "\"/>" );

			return Indent( sb.ToString(), indent );
		}

		/// <summary>
		///   Gets the rect xml string.
		/// </summary>
		/// <param name="col">
		///   The rect.
		/// </param>
		/// <param name="name">
		///   Xml element name.
		/// </param>
		/// <param name="indent">
		///   Indentation level.
		/// </param>
		/// <returns>
		///   The given rect as an xml string.
		/// </returns>
		public static string ToString( Color col, string name = null, uint indent = 0 )
		{
			if( !Identifiable.IsValid( name ) )
				name = nameof( Color );

			string spaces = " ";

			for( int i = 0; i < name.Length; i++ )
				spaces += ' ';

			StringBuilder sb = new StringBuilder();

			sb.Append( "<" );
			sb.Append( name );
			sb.Append( " " );
			sb.Append( nameof( Color.R ) );
			sb.Append( "=\"" );
			sb.Append( col.R );
			sb.AppendLine( "\"" );

			sb.Append( spaces );
			sb.Append( " " );
			sb.Append( nameof( Color.G ) );
			sb.Append( "=\"" );
			sb.Append( col.G );
			sb.AppendLine( "\"" );

			sb.Append( spaces );
			sb.Append( " " );
			sb.Append( nameof( Color.B ) );
			sb.Append( "=\"" );
			sb.Append( col.B );
			sb.AppendLine( "\"" );

			sb.Append( spaces );
			sb.Append( " " );
			sb.Append( nameof( Color.A ) );
			sb.Append( "=\"" );
			sb.Append( col.A );
			sb.AppendLine( "\"/>" );

			return Indent( sb.ToString(), indent );
		}

		/// <summary>
		///   Gets the video mode xml string.
		/// </summary>
		/// <param name="vm">
		///   The video mode.
		/// </param>
		/// <param name="name">
		///   Xml element name.
		/// </param>
		/// <param name="indent">
		///   Indentation level.
		/// </param>
		/// <returns>
		///   The given video mode as an xml string.
		/// </returns>
		public static string ToString( VideoMode vm, string name = null, uint indent = 0 )
		{
			if( !Identifiable.IsValid( name ) )
				name = nameof( VideoMode );

			string spaces = " ";

			for( int i = 0; i < name.Length; i++ )
				spaces += ' ';

			StringBuilder sb = new StringBuilder();

			sb.Append( "<" );
			sb.Append( name );
			sb.Append( " " + nameof( VideoMode.Width ) + "=\"" );
			sb.Append( vm.Width );
			sb.AppendLine( "\"" );
			sb.Append( spaces );
			sb.Append( " " + nameof( VideoMode.Height ) + "=\"" );
			sb.Append( vm.Height );
			sb.AppendLine( "\"" );
			sb.Append( spaces );
			sb.Append( " " + nameof( VideoMode.BitsPerPixel ) + "=\"" );
			sb.Append( vm.BitsPerPixel );
			sb.AppendLine( "\"/>" );

			return Indent( sb.ToString(), indent );
		}

		/// <summary>
		///   Attempts to load a vector from an xml element.
		/// </summary>
		/// <param name="ele">
		///   The element to load from.
		/// </param>
		/// <returns>
		///   A valid vector on success or null on failure.
		/// </returns>
		public static Vector2f? ToVec2f( XmlElement ele )
		{
			if( ele == null )
				return Logger.LogReturn<Vector2f?>( "Unable to load Vector2f from null xml element.", null, LogType.Error );

			if( !ele.HasAttribute( nameof( Vector2f.X ) ) )
				return Logger.LogReturn<Vector2f?>( "Unable to load Vector2f: No X attribute.", null, LogType.Error );
			if( !ele.HasAttribute( nameof( Vector2f.Y ) ) )
				return Logger.LogReturn<Vector2f?>( "Unable to load Vector2f: No Y attribute.", null, LogType.Error );

			Vector2f vec;

			try
			{
				vec = new Vector2f( float.Parse( ele.GetAttribute( nameof( Vector2f.X ) ) ), 
				                    float.Parse( ele.GetAttribute( nameof( Vector2f.Y ) ) ) );
			}
			catch( Exception e )
			{
				return Logger.LogReturn<Vector2f?>( "Unable to load Vector2f: " + e.Message, null, LogType.Error );
			}

			return vec;
		}
		/// <summary>
		///   Attempts to load a vector from an xml element.
		/// </summary>
		/// <param name="ele">
		///   The element to load from.
		/// </param>
		/// <returns>
		///   A valid vector on success or null on failure.
		/// </returns>
		public static Vector2i? ToVec2i( XmlElement ele )
		{
			if( ele == null )
				return Logger.LogReturn<Vector2i?>( "Unable to load Vector2i from null xml element.", null, LogType.Error );

			if( !ele.HasAttribute( nameof( Vector2i.X ) ) )
				return Logger.LogReturn<Vector2i?>( "Unable to load Vector2i: No X attribute.", null, LogType.Error );
			if( !ele.HasAttribute( nameof( Vector2i.Y ) ) )
				return Logger.LogReturn<Vector2i?>( "Unable to load Vector2i: No Y attribute.", null, LogType.Error );

			Vector2i vec;

			try
			{
				vec = new Vector2i( int.Parse( ele.GetAttribute( nameof( Vector2i.X ) ) ),
									int.Parse( ele.GetAttribute( nameof( Vector2i.Y ) ) ) );
			}
			catch( Exception e )
			{
				return Logger.LogReturn<Vector2i?>( "Unable to load Vector2i: " + e.Message, null, LogType.Error );
			}

			return vec;
		}
		/// <summary>
		///   Attempts to load a vector from an xml element.
		/// </summary>
		/// <param name="ele">
		///   The element to load from.
		/// </param>
		/// <returns>
		///   A valid vector on success or null on failure.
		/// </returns>
		public static Vector2u? ToVec2u( XmlElement ele )
		{
			if( ele == null )
				return Logger.LogReturn<Vector2u?>( "Unable to load Vector2u from null xml element.", null, LogType.Error );

			if( !ele.HasAttribute( nameof( Vector2u.X ) ) )
				return Logger.LogReturn<Vector2u?>( "Unable to load Vector2u: No X attribute.", null, LogType.Error );
			if( !ele.HasAttribute( nameof( Vector2i.Y ) ) )
				return Logger.LogReturn<Vector2u?>( "Unable to load Vector2u: No Y attribute.", null, LogType.Error );

			Vector2u vec;

			try
			{
				vec = new Vector2u( uint.Parse( ele.GetAttribute( nameof( Vector2u.X ) ) ),
									uint.Parse( ele.GetAttribute( nameof( Vector2u.Y ) ) ) );
			}
			catch( Exception e )
			{
				return Logger.LogReturn<Vector2u?>( "Unable to load Vector2u: " + e.Message, null, LogType.Error );
			}

			return vec;
		}

		/// <summary>
		///   Attempts to load a rect from an xml element.
		/// </summary>
		/// <param name="ele">
		///   The element to load from.
		/// </param>
		/// <returns>
		///   A valid rect on success or null on failure.
		/// </returns>
		public static FloatRect? ToFRect( XmlElement ele )
		{
			if( ele == null )
				return Logger.LogReturn<FloatRect?>( "Unable to load FloatRect from null xml element.", null, LogType.Error );

			if( !ele.HasAttribute( nameof( FloatRect.Left ) ) )
				return Logger.LogReturn<FloatRect?>( "Unable to load FloatRect: No Left attribute.", null, LogType.Error );
			if( !ele.HasAttribute( nameof( FloatRect.Top ) ) )
				return Logger.LogReturn<FloatRect?>( "Unable to load FloatRect: No Top attribute.", null, LogType.Error );
			if( !ele.HasAttribute( nameof( FloatRect.Width ) ) )
				return Logger.LogReturn<FloatRect?>( "Unable to load FloatRect: No Width attribute.", null, LogType.Error );
			if( !ele.HasAttribute( nameof( FloatRect.Height ) ) )
				return Logger.LogReturn<FloatRect?>( "Unable to load FloatRect: No Height attribute.", null, LogType.Error );

			FloatRect rect;

			try
			{
				rect = new FloatRect( float.Parse( ele.GetAttribute( nameof( FloatRect.Left ) ) ),
				                      float.Parse( ele.GetAttribute( nameof( FloatRect.Top ) ) ),
				                      float.Parse( ele.GetAttribute( nameof( FloatRect.Width ) ) ),
									  float.Parse( ele.GetAttribute( nameof( FloatRect.Height ) ) ) );
			}
			catch( Exception e )
			{
				return Logger.LogReturn<FloatRect?>( "Unable to load FloatRect: " + e.Message, null, LogType.Error );
			}

			return rect;
		}
		/// <summary>
		///   Attempts to load a rect from an xml element.
		/// </summary>
		/// <param name="ele">
		///   The element to load from.
		/// </param>
		/// <returns>
		///   A valid rect on success or null on failure.
		/// </returns>
		public static IntRect? ToIRect( XmlElement ele )
		{
			if( ele == null )
				return Logger.LogReturn<IntRect?>( "Unable to load IntRect from null xml element.", null, LogType.Error );

			if( !ele.HasAttribute( nameof( IntRect.Left ) ) )
				return Logger.LogReturn<IntRect?>( "Unable to load IntRect: No Left attribute.", null, LogType.Error );
			if( !ele.HasAttribute( nameof( IntRect.Top ) ) )
				return Logger.LogReturn<IntRect?>( "Unable to load IntRect: No Top attribute.", null, LogType.Error );
			if( !ele.HasAttribute( nameof( IntRect.Width ) ) )
				return Logger.LogReturn<IntRect?>( "Unable to load IntRect: No Width attribute.", null, LogType.Error );
			if( !ele.HasAttribute( nameof( IntRect.Height ) ) )
				return Logger.LogReturn<IntRect?>( "Unable to load IntRect: No Height attribute.", null, LogType.Error );

			IntRect rect;

			try
			{
				rect = new IntRect( int.Parse( ele.GetAttribute( nameof( IntRect.Left ) ) ),
									int.Parse( ele.GetAttribute( nameof( IntRect.Top ) ) ),
									int.Parse( ele.GetAttribute( nameof( IntRect.Width ) ) ),
									int.Parse( ele.GetAttribute( nameof( IntRect.Height ) ) ) );
			}
			catch( Exception e )
			{
				return Logger.LogReturn<IntRect?>( "Unable to load IntRect: " + e.Message, null, LogType.Error );
			}

			return rect;
		}

		/// <summary>
		///   Attempts to load a color from an xml element.
		/// </summary>
		/// <param name="ele">
		///   The element to load from.
		/// </param>
		/// <returns>
		///   A valid color on success or null on failure.
		/// </returns>
		public static Color? ToColor( XmlElement ele )
		{
			if( ele == null )
				return Logger.LogReturn<Color?>( "Unable to load Color from null xml element.", null, LogType.Error );

			if( !ele.HasAttribute( nameof( Color.R ) ) )
				return Logger.LogReturn<Color?>( "Unable to load Color: No R attribute.", null, LogType.Error );
			if( !ele.HasAttribute( nameof( Color.G ) ) )
				return Logger.LogReturn<Color?>( "Unable to load Color: No G attribute.", null, LogType.Error );
			if( !ele.HasAttribute( nameof( Color.B ) ) )
				return Logger.LogReturn<Color?>( "Unable to load Color: No B attribute.", null, LogType.Error );
			if( !ele.HasAttribute( nameof( Color.A ) ) )
				return Logger.LogReturn<Color?>( "Unable to load Color: No A attribute.", null, LogType.Error );

			Color col;

			try
			{
				col = new Color( byte.Parse( ele.GetAttribute( nameof( Color.R ) ) ),
								 byte.Parse( ele.GetAttribute( nameof( Color.G ) ) ),
								 byte.Parse( ele.GetAttribute( nameof( Color.B ) ) ),
								 byte.Parse( ele.GetAttribute( nameof( Color.A ) ) ) );
			}
			catch( Exception e )
			{
				return Logger.LogReturn<Color?>( "Unable to load Color: " + e.Message, null, LogType.Error );
			}

			return col;
		}

		/// <summary>
		///   Attempts to load a video mode from an xml element.
		/// </summary>
		/// <param name="ele">
		///   The element to load from.
		/// </param>
		/// <returns>
		///   A valid video mode on success or null on failure.
		/// </returns>
		public static VideoMode? ToVideoMode( XmlElement ele )
		{
			if( ele == null )
				return Logger.LogReturn<VideoMode?>( "Unable to load VideoMode from null xml element.", null, LogType.Error );

			if( !ele.HasAttribute( nameof( VideoMode.Width ) ) )
				return Logger.LogReturn<VideoMode?>( "Unable to load VideoMode: No Width attribute.", null, LogType.Error );
			if( !ele.HasAttribute( nameof( VideoMode.Height ) ) )
				return Logger.LogReturn<VideoMode?>( "Unable to load VideoMode: No Height attribute.", null, LogType.Error );
			if( !ele.HasAttribute( nameof( VideoMode.BitsPerPixel ) ) )
				return Logger.LogReturn<VideoMode?>( "Unable to load VideoMode: No BitsPerPixel attribute.", null, LogType.Error );

			VideoMode vm;

			try
			{
				vm = new VideoMode( uint.Parse( ele.GetAttribute( nameof( VideoMode.Width ) ) ),
									uint.Parse( ele.GetAttribute( nameof( VideoMode.Height ) ) ),
									uint.Parse( ele.GetAttribute( nameof( VideoMode.BitsPerPixel ) ) ) );
			}
			catch( Exception e )
			{
				return Logger.LogReturn<VideoMode?>( "Unable to load VideoMode: " + e.Message, null, LogType.Error );
			}

			return vm;
		}

		/// <summary>
		///   Indents each line of the string a set amount of times.
		/// </summary>
		/// <param name="lines">
		///   The string to indent.
		/// </param>
		/// <param name="indent">
		///   The amount of tabs to use for indentation.
		/// </param>
		/// <returns>
		///   The given string indented with the given amount of tabs, or just tabs if the string is
		///   null or empty.
		/// </returns>
		public static string Indent( string lines, uint indent = 1 )
		{
			string tabs = string.Empty;

			for( uint i = 0; i < indent; i++ )
				tabs += '\t';

			if( string.IsNullOrEmpty( lines ) )
				return tabs;

			return tabs + lines.Replace( "\r\n", "\n" ).Replace( "\n", "\n" + tabs ).Replace( "\n", "\r\n" );
		}
	}
}
