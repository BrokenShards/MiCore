////////////////////////////////////////////////////////////////////////////////
// NameTest.cs 
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

namespace MiCore.Test
{
	public class NameTest : TestModule
	{
		// Valid string name.
		const string ValidName = "Mr. Valid Name";
		// Invalid string name.
		const string InvalidName = "\t Mr. Invalid Name\n";

		protected override bool OnTest()
		{
			Logger.Log( "Running Naming Test..." );

			bool result = true;

			// Ensure valid name is not reported as invalid.
			if( !Naming.IsValid( ValidName ) )
				result = Logger.LogReturn( "Failed! Valid name reported as invalid.", false );

			// Ensure invalid name is not reported as valid.
			if( Naming.IsValid( InvalidName ) )
				result = Logger.LogReturn( "Failed! Invalid name reported as valid.", false );

			// Ensure validated name is not reported as invalid.
			if( !Naming.IsValid( Naming.AsValid( InvalidName ) ) )
				result = Logger.LogReturn( "Failed! Validated name reported as invalid.", false );

			// Ensure random name is not reported as invalid.
			if( !Naming.IsValid( Naming.RandomName( 8 ) ) )
				result = Logger.LogReturn( "Failed! Random name reported as invalid.", false );

			// Ensure new name is not reported as invalid.
			if( !Naming.IsValid( Naming.NewName() ) )
				result = Logger.LogReturn( "Failed! Random name reported as invalid.", false );

			return Logger.LogReturn( result ? "Naming test succeeded!." : "Naming test failed!.", result );
		}
	}
}
