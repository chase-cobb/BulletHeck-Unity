/*
Copyright 2024 Heck Yeah Games LLC

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/
namespace HeckYeahGames
{
    namespace BulletHeck
    {
        namespace Patterns
        {
            public delegate void OnPatternChange();

            /// <summary>
            /// Interface used by pattern types that will allow handler
            /// registration for when the pattern changes in any way
            /// that requires it to rebuilt by a resolver.
            /// 
            /// See BulletPatternResolver.cs or ShapePatternResolver.cs
            /// </summary>
            public interface IPattern
            {
                /// <summary>
                /// Registers an OnPatternChange event handler.
                /// </summary>
                /// <param name="handler">Handler to add to OnPatternChange event.</param>
                public void RegisterOnChangeEventHandler(OnPatternChange handler);

                /// <summary>
                /// Unregisters an OnPatternChange event handler.
                /// </summary>
                /// <param name="handler">Handler to remove from the OnPatternChange event.</param>
                public void UnregisterOnChangeEventHandler(OnPatternChange handler);
            }// IPattern
        }// Patterns
    }// BulletHeck
}// HeckYeahGames
