////////////////////////////////////////////////////////////////////////////////
// Paths.cs 
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

using System.IO;
using System.Reflection;

namespace MiCore
{
	/// <summary>
	///   Contains path related functionality.
	/// </summary>
	public static class Paths
	{
		/// <summary>
		///   Swaps path seperator character '/' with '\\' like in windows paths.
		/// </summary>
		/// <param name="path">
		///   The path.
		/// </param>
		/// <returns>
		///   The path with the seperator character '/' replaced with '\\'.
		/// </returns>
		public static string ToWindows( string path )
		{
			if( string.IsNullOrWhiteSpace( path ) )
				return path;

			return path.Replace( '/', '\\' );
		}
		/// <summary>
		///   Swaps path seperator character '\\' with '/' like in non windows paths.
		/// </summary>
		/// <param name="path">
		///   The path.
		/// </param>
		/// <returns>
		///   The path with the seperator character '\\' replaced with '/'.
		/// </returns>
		public static string FromWindows( string path )
		{
			if( string.IsNullOrWhiteSpace( path ) )
				return path;

			return path.Replace( '\\', '/' );
		}

		/// <summary>
		///   The path to the binary executable.
		/// </summary>
		public static string Executable
		{
			get
			{
				string path = Assembly.GetExecutingAssembly().CodeBase;

				int len = path.Length;

				string s5 = len > 5 ? path.Substring( 0, 5 ).ToLower() : null;
				string s6 = len > 6 ? path.Substring( 0, 6 ).ToLower() : null;
				string s8 = len > 8 ? path.Substring( 0, 8 ).ToLower() : null;
				string s10 = len > 10 ? path.Substring( 0, 10 ).ToLower() : null;

				if( s5 != null && ( s5 == "dir:/" || s5 == "dir:\\" ) )
					path = path.Substring( 5 );
				else if( s6 != null && ( s6 == "file:/" || s6 == "file:\\" || s6 == "path:/" || s6 == "path:\\" ) )
					path = path.Substring( 6 );
				else if( s8 != null && ( s8 == "folder:/" || s8 == "folder:\\" ) )
					path = path.Substring( 8 );
				else if( s10 != null && ( s10 == "directory:/" || s10 == "directory:\\" ) )
					path = path.Substring( 10 );

				return ToWindows( path );
			}
		}
		/// <summary>
		///   The folder path containing the binary executable.
		/// </summary>
		public static string ExecutableFolder
		{
			get
			{
				return Path.GetDirectoryName( Executable );
			}
		}
	}
}
