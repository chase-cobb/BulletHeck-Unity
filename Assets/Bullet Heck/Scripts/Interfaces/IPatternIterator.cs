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
            namespace Shape
            {
                /// <summary>
                /// Interface for creating pattern iterators.
                /// </summary>
                public interface IPatternIterator
                {
                    /// <summary>
                    /// Returns an array of ShapePoint objects as defined by the iterator. The length
                    /// is defined by the iterator and it could be 1 or longer.
                    /// 
                    /// Calling this will also reset the iterator to the beginning.
                    /// </summary>
                    /// <returns>An array of ShapePoint objects. Null, if no valid points exits.</returns>
                    public ShapePoint[] First();

                    /// <summary>
                    /// Returns an array of ShapePoints of the current offset of the iterator, withouth
                    /// advancing the iterator.
                    /// </summary>
                    /// <returns>An array of ShapePoint objects. Null, if no valid points exits.</returns>
                    public ShapePoint[] Current();

                    /// <summary>
                    /// Advances the iterator and returns an array of ShapePoint objects. The length
                    /// is defined by the iterator and it could be 1 or longer.
                    /// </summary>
                    /// <returns></returns>
                    public ShapePoint[] Next();

                    /// <summary>
                    /// Returns true if the iterator has reached the end of valid objects.
                    /// </summary>
                    /// <returns>True if the end has been reached. Otherwise, false.</returns>
                    public bool Done();
                }//IPatternIterator
            }// Shape
        }// Patterns
    }// BulletHeck
}// HeckYeahGames
