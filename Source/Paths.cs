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
	}
}
