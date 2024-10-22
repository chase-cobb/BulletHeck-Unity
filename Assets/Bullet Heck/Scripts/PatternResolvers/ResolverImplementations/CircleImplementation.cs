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
 #if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

using UnityEngine;

namespace HeckYeahGames
{
    namespace BulletHeck
    {
        namespace Patterns
        {
            namespace Shape
            {
                public class CircleImplementation : IShapeImplementation
                {
                    public void Build(Vector3 worldPosition, ShapePattern shapePattern, ref ShapePoint[] shapePoints)
                    {
                        if(shapePattern == null)
                        {
                            return;
                        }

                        shapePoints = new ShapePoint[shapePattern.numberOfPoints];

                        MathHelper.GetPointsOnRadius(ref shapePoints, worldPosition, shapePattern.maxRadius, 360.0f, 0.0f, false, true);
                    }

                    public void Update(Vector3 worldPosition, ShapePattern shapePattern, ref ShapePoint[] shapePoints, float rotationOffset)
                    {
                        float rotationModifier = 1.0f;
                        if(shapePattern.reverseRotation)
                        {
                            rotationModifier = -1.0f;
                        }
                        //MathHelper.GetPointsOnRadius(ref shapePoints, worldPosition, shapePattern.maxRadius, 360.0f, rotationOffset * rotationModifier, false, true);
                        MathHelper.RotatePoints(ref shapePoints, worldPosition, rotationOffset * rotationModifier);
                    }

                    public void OnSelectedGizmoDraw(Vector3 origin, Vector3 forward, ShapePattern shapePattern, ref ShapePoint[] shapePoints)
                    {
                        for(int i = 0; i < shapePoints.Length; ++i)
                        {
                            Vector3 pointPosition = shapePoints[i].world;
                            Gizmos.DrawWireSphere(origin, shapePattern.maxRadius);
                            Gizmos.DrawSphere(pointPosition, 0.2f);

                            Color cachedGizmoColor = Gizmos.color;
                            Gizmos.color = Color.green;
                            Gizmos.DrawLine(shapePoints[i].world, shapePoints[i].world + shapePoints[i].normalWorld);
                            Gizmos.color = cachedGizmoColor;
                            
                            #if UNITY_EDITOR
                            Handles.Label(shapePoints[i].world + shapePoints[i].normalWorld, "[" + i + "]");
                            #endif // UNITY_EDITOR
                        }
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawRay(origin, forward * 0.8f);
                    }
                }// CircleImplementation
            }// Shape
        }// Patterns
    }// BulletHeck
}// HeckYeahGames
