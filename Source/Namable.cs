////////////////////////////////////////////////////////////////////////////////
// Namable.cs 
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
using System.Linq;
using System.Text;

namespace MiCore
{
	/// <summary>
	///   Interface for objects that have a name.
	/// </summary>
	public interface INamable
	{
		/// <summary>
		///   The name of the object.
		/// </summary>
		string Name { get; set; }
	}

	/// <summary>
	///   Contains Name related functionality.
	/// </summary>
	public static class Naming
	{
		/// <summary>
		///   If the given string is a valid name.
		/// </summary>
		/// <param name="name">
		///   The name string.
		/// </param>
		/// <returns>
		///   True if the name is valid and false otherwise.
		/// </returns>
		public static bool IsValid( string name )
		{
			if( string.IsNullOrWhiteSpace( name ) || char.IsWhiteSpace( name[ 0 ] ) )
				return false;

			for( int i = 0; i < name.Length; i++ )
				if( !char.IsLetterOrDigit( name[ i ] ) && !char.IsPunctuation( name[ i ] ) &&
					!char.IsSymbol( name[ i ] ) && name[ i ] is not ' ' )
					return false;

			return true;
		}
		/// <summary>
		///   If the name of the given object is valid.
		/// </summary>
		/// <param name="i">
		///   The object to check.
		/// </param>
		/// <returns>
		///   True if the name of the object is valid and false otherwise.
		/// </returns>
		public static bool IsValid( IIdentifiable<string> i )
		{
			return IsValid( i.ID );
		}

		/// <summary>
		///   Returns the given string as a valid name.
		/// </summary>
		/// <param name="name">
		///   The possibly invalid name.
		/// </param>
		/// <param name="repl">
		///   The character used to replace invalid characters.
		/// </param>
		/// <returns>
		///   The given string as a valid name.
		/// </returns>
		public static string AsValid( string name, char repl = '_' )
		{
			if( string.IsNullOrWhiteSpace( name ) )
				return NewName();
			if( IsValid( name ) )
				return name;

			static bool valid_char( char c ) => char.IsLetterOrDigit( c ) || char.IsPunctuation( c ) || char.IsSymbol( c );

			if( !valid_char( repl ) )
				repl = '_';

			string n = name.Trim();
			StringBuilder res = new( n.Length );

			for( int i = 0; i < n.Length; i++ )
			{
				if( !valid_char( n[ i ] ) && n[ i ] is not ' ' )
					res.Append( repl );
				else
					res.Append( n[ i ] );
			}

			return res.ToString();
		}
		/// <summary>
		///   Returns the given objects' name as a valid name.
		/// </summary>
		/// <param name="i">
		///   The identifiable object.
		/// </param>
		/// <returns>
		///   The given object' invalid name as a valid ID.
		/// </returns>
		public static string AsValid( IIdentifiable<string> i ) => AsValid( i?.ID );

		/// <summary>
		///   Creates a psuedo-new, valid name with a given prefix.
		/// </summary>
		/// <param name="prefix">
		///   Prefixes the name numbers.
		/// </param>
		/// <returns>
		///   A new name.
		/// </returns>
		public static string NewName( string prefix = null )
		{
			if( string.IsNullOrWhiteSpace( prefix ) )
				prefix = "New Name";

			if( !_counter.ContainsKey( prefix ) )
				_counter.Add( prefix, 0 );

			string str = prefix + _counter[ prefix ].ToString();
			_counter[ prefix ]++;
			return str.Trim();
		}

		/// <summary>
		///   Generates a random name string with the given length.
		/// </summary>
		/// <param name="length">
		///   The length of the name.
		/// </param>
		/// <returns>
		///   A random name with the given length or `string.Empty` if length is zero.
		/// </returns>
		public static string RandomName( uint length )
		{
			if( length == 0 )
				return string.Empty;

			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

			string result = new( Enumerable.Repeat( chars, (int)length )
			  .Select( s => s[ random.Next( s.Length ) ] ).ToArray() );

			result = result[ 0 ] + result[ 1.. ].ToLower();
			return result;
		}

		private static readonly Dictionary<string, ulong> _counter = new();
		private static readonly Random random = new();
	}
}
