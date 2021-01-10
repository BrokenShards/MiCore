////////////////////////////////////////////////////////////////////////////////
// IDTest.cs 
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
	public class IDTest : TestModule
	{
		// Valid string ID.
		const string ValidID = "valid_id_23";
		// Invalid string ID.
		const string InvalidID = " in-valid-ID";

		protected override bool OnTest()
		{
			Logger.Log( "Running Identifiable Test..." );

			bool result = true;

			// Ensure valid ID is not reported as invalid.
			if( !Identifiable.IsValid( ValidID ) )
				result = Logger.LogReturn( "Failed! Valid ID reported as invalid.", false );

			// Ensure invalid ID is not reported as valid.
			if( Identifiable.IsValid( InvalidID ) )
				result = Logger.LogReturn( "Failed! Invalid ID reported as valid.", false );

			// Ensure validated ID is not reported as invalid.
			if( !Identifiable.IsValid( Identifiable.AsValid( InvalidID ) ) )
				result = Logger.LogReturn( "Failed! Validated ID reported as invalid.", false );

			// Ensure random ID is not reported as invalid.
			if( !Identifiable.IsValid( Identifiable.RandomStringID( 8 ) ) )
				result = Logger.LogReturn( "Failed! Random ID reported as invalid.", false );

			// Ensure new ID is not reported as invalid.
			if( !Identifiable.IsValid( Identifiable.NewStringID() ) )
				result = Logger.LogReturn( "Failed! New ID reported as invalid.", false );

			return Logger.LogReturn( result ? "Identifiable test succeeded!." : "Identifiable test failed!.", result );
		}
	}
}
