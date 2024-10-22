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
using UnityEngine;

namespace HeckYeahGames
{
    namespace BulletHeck
    {
        namespace Patterns
        {
            namespace Shape
            {
                /// <summary>
                /// Interface implemented by objects that intend to implement new
                /// shape implementations for ShapePatternResolver.cs
                /// </summary>
                public interface IShapeImplementation
                {
                    /// <summary>
                    /// Constructs the new shape pattern.
                    /// </summary>
                    /// <param name="worldPosition">Origin of the shape in world space.</param>
                    /// <param name="shapePattern">The shape pattern data container that contains parameters for the shape.</param>
                    /// <param name="shapePoints">The points of the shape filled out by reference.</param>
                    void Build(Vector3 worldPosition, ShapePattern shapePattern, ref ShapePoint[] shapePoints);

                    /// <summary>
                    /// Updates the 
                    /// </summary>
                    /// <param name="worldPosition">Origin of the shape in world space.</param>
                    /// <param name="shapePattern">The shape pattern data container that contains parameters for the shape.</param>
                    /// <param name="shapePoints">The points of the shape filled out by reference.</param>
                    /// <param name="rotationOffset">Rotation to be applied to the shape.</param>
                    void Update(Vector3 worldPosition, ShapePattern shapePattern, ref ShapePoint[] shapePoints, float rotationOffset);

                    /// <summary>
                    /// Draws the shape in the editor when the game object is selected.
                    /// </summary>
                    /// <param name="origin">Origin of the shanpe in world space.</param>
                    /// <param name="forward">The current forward vector of the ShapePattern</param>
                    /// <param name="shapePattern">The shape pattern data container that contains parameters for the shape.</param>
                    /// <param name="shapePoints"></param>
                    void OnSelectedGizmoDraw(Vector3 origin, Vector3 forward, ShapePattern shapePattern, ref ShapePoint[] shapePoints);
                }// IShapeImplementation
            }// Shape
        }// Patterns
    }// BulletHeck
}// HeckYeahGames
