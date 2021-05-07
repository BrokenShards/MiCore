////////////////////////////////////////////////////////////////////////////////
// ComponentRegister.cs 
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
	///   Used for registering and creating components.
	/// </summary>
	public sealed class ComponentRegister : TypeRegister<MiComponent>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		private ComponentRegister()
		:	base()
		{ }

		/// <summary>
		///   The register object.
		/// </summary>
		public static ComponentRegister Manager
		{
			get
			{
				if( _instance == null )
				{
					lock( _syncRoot )
					{
						if( _instance == null )
						{
							_instance = new ComponentRegister();
						}
					}
				}

				return _instance;
			}
		}

		/// <summary>
		///   Registers a component type.
		/// </summary>
		/// <typeparam name="T">
		///   The component type.
		/// </typeparam>
		/// <param name="replace">
		///   If an already registered type should be replaced.
		/// </param>
		/// <returns>
		///   True if the component type was registered successfully; false if `T.TypeName` is not a
		///   valid ID or a type is already registered to the ID and replace is false.
		/// </returns>
		public bool Register<T>( bool replace = true ) where T : MiComponent, new()
		{
			string id;

			using( T t = new() )
				id = t.TypeName;

			return Register<T>( id, replace );
		}
		
		private static volatile ComponentRegister _instance;
		private static readonly object _syncRoot = new object();
	}
}
