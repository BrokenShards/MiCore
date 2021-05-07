////////////////////////////////////////////////////////////////////////////////
// Identifiable.cs 
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
	///   Interface for objects that are identified by an ID.
	/// </summary>
	public interface IIdentifiable<T>
	{
		/// <summary>
		///   The object ID.
		/// </summary>
		T ID { get; }
	}
	
	/// <summary>
	///   Contains ID and name related functionality.
	/// </summary>
	public static class Identifiable
	{
		/// <summary>
		///   If the given ID is valid.
		/// </summary>
		/// <param name="id">
		///   The ID to check.
		/// </param>
		/// <returns>
		///   True if the ID is valid and false otherwise.
		/// </returns>
		public static bool IsValid( string id )
		{
			if( string.IsNullOrWhiteSpace( id ) || ( !char.IsLetter( id[ 0 ] ) && id[ 0 ] != '_' ) )
				return false;

			for( int i = 0; i < id.Length; i++ )
				if( !char.IsLetterOrDigit( id[ i ] ) && id[ i ] != '_' )
					return false;

			return true;
		}
		/// <summary>
		///   If the given objects' ID is valid.
		/// </summary>
		/// <param name="i">
		///   The object to check.
		/// </param>
		/// <returns>
		///   True if the objects' ID is valid and false otherwise.
		/// </returns>
		public static bool IsValid( IIdentifiable<string> i )
		{
			return IsValid( i.ID );
		}

		/// <summary>
		///   Returns either the given string as a valid ID or a generated random ID if it is empty or null.
		/// </summary>
		/// <param name="id">
		///   The invalid ID.
		/// </param>
		/// <returns>
		///   The given string as a valid ID.
		/// </returns>
		public static string AsValid( string id )
		{
			if( IsValid( id ) )
				return id;
			if( string.IsNullOrWhiteSpace( id ) )
				return RandomStringID( 12 );
			
			string n = id.Trim();
			
			StringBuilder sb = new( n.Length );

			for( int i = 0; i < n.Length; i++ )
			{
				if( !char.IsLetterOrDigit( n[ i ] ) && n[ i ] != '_' )
					sb.Append( '_' );
				else
					sb.Append( n[ i ] );
			}

			return sb.ToString();
		}
		/// <summary>
		///   Returns the given objects' ID as a valid ID.
		/// </summary>
		/// <param name="i">
		///   The object.
		/// </param>
		/// <returns>
		///   The given object' invalid ID as a valid ID.
		/// </returns>
		public static string AsValid( IIdentifiable<string> i )
		{
			return AsValid( i.ID );
		}

		/// <summary>
		///   Creates a psuedo-new, valid ID from a given prefix.
		/// </summary>
		/// <param name="prefix">
		///   Prefixes the ID numbers.
		/// </param>
		/// <returns>
		///   A new ID.
		/// </returns>
		public static string NewStringID( string prefix = null )
		{
			string p = string.IsNullOrWhiteSpace( prefix ) ? "ID_" : prefix.Trim();
			string l = p.ToLower();

			if( !_counter.ContainsKey( l ) )
				_counter.Add( l, 0 );

			string str = $"{ p }{ _counter[ l ] }";
			_counter[ l ]++;
			return str;
		}
		
		/// <summary>
		///   Creates a psuedo-new ulong ID.
		/// </summary>
		/// <param name="prefix">
		///   The prefix linked to the ID type.
		/// </param>
		/// <returns>
		///   A new ID.
		/// </returns>
		public static ulong NewUIntID( string prefix = null )
		{
			string p = string.IsNullOrWhiteSpace( prefix ) ? "<'ulong'>" : prefix.Trim().ToLower();

			if( !_counter.ContainsKey( p ) )
				_counter.Add( p, 0 );

			ulong id = _counter[ p ];
			_counter[ p ]++;
			return id;
		}
		/// <summary>
		///   Generates a random ID string with the given length.
		/// </summary>
		/// <param name="length">
		///   The length of the ID.
		/// </param>
		/// <returns>
		///   A random ID with the given length or `string.Empty` if length is zero.
		/// </returns>
		public static string RandomStringID( uint length )
		{
			if( length == 0 )
				return string.Empty;

			const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ_";
			const string chars   = letters + "0123456789";

			char first = letters[ random.Next( letters.Length ) ];

			return first + new string( Enumerable.Repeat( chars, (int)length - 1 ).Select(
				s => s[ random.Next( s.Length ) ] ).ToArray() );
		}

		static readonly Dictionary<string, ulong> _counter = new();
		static readonly Random random = new();
	}
}
