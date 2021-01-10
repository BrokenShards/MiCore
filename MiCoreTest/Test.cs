////////////////////////////////////////////////////////////////////////////////
// Test.cs 
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

namespace MiCore.Test
{
	static class Tests
	{
		static void Main()
		{
			Logger.LogToFile = true;
			Logger.Log( "Running MiCore Tests..." );

			bool result = true;

			if( !Testing.Test<IDTest>() )
				result = false;
			if( !Testing.Test<NameTest>() )
				result = false;
			if( !SerializableTest.Run() )
				result = false;
			if( !Testing.Test<XmlTest>() )
				result = false;
			if( !ECSTest.Run() )
				result = false;

			Logger.Log( result ? "All MiCore tests completed successfully!" : "One or more MiCore tests failed!" );

			Logger.Log( "Press enter to exit." );
			Console.ReadLine();
		}
	}
}
