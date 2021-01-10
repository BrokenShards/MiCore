////////////////////////////////////////////////////////////////////////////////
// TestModule.cs 
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

using SFML.Graphics;

namespace MiCore
{
	/// <summary>
	///   Base class for test modules.
	/// </summary>
	public abstract class TestModule
	{
		/// <summary>
		///   Run module tests.
		/// </summary>
		/// <returns>
		///   True if tests succeeded, otherwise false.
		/// </returns>
		public virtual bool RunTest( RenderWindow window = null )
		{
			return OnTest();
		}

		/// <summary>
		///   Override this to run your module tests.
		/// </summary>
		/// <returns>
		///   True if tests succeeded, otherwise false.
		/// </returns>
		protected abstract bool OnTest();
	}

	/// <summary>
	///   Base class for test modules that also require a visual test.
	/// </summary>
	public abstract class VisualTestModule : TestModule
	{
		/// <summary>
		///   Run module tests.
		/// </summary>
		/// <returns>
		///   True if tests succeeded, otherwise false.
		/// </returns>
		public override bool RunTest( RenderWindow window )
		{
			return OnTest() && OnVisualTest( window );
		}

		/// <summary>
		///   Run visual module tests.
		/// </summary>
		/// <param name="window">
		///   The render window.
		/// </param>
		/// <returns>
		///   True if tests succeeded, otherwise false.
		/// </returns>
		protected abstract bool OnVisualTest( RenderWindow window );
	}

	/// <summary>
	///   Contains test related functionality.
	/// </summary>
	public static class Testing
	{
		/// <summary>
		///   Runs a test module by type.
		/// </summary>
		/// <typeparam name="T">
		///   The type of test module to run.
		/// </typeparam>
		/// <param name="window">
		///   The render window.
		/// </param>
		/// <returns>
		///   True if tests succeeded, otherwise false.
		/// </returns>
		public static bool Test<T>( RenderWindow window = null ) where T : TestModule, new()
		{
			return new T().RunTest( window );
		}
	}
}
