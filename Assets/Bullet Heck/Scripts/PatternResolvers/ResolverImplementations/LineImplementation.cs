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
                public class LineImplementation : IShapeImplementation
                {
                    public void Build(Vector3 worldPosition, ShapePattern shapePattern, ref ShapePoint[] shapePoints)
                    {
                        if(shapePattern == null)
                        {
                            return;
                        }

                        shapePoints = new ShapePoint[shapePattern.numberOfPoints * (shapePattern.pointsOnEdges + 1)];
                        
                        ShapePoint[] tempMajorPoints = new ShapePoint[shapePattern.numberOfPoints];

                        MathHelper.GetPointsOnRadius(ref tempMajorPoints, worldPosition, shapePattern.maxRadius, shapePattern.spreadInDegrees, 0.0f, true, true);

                        // Go through points and adde subdivisions
                        for(int i = 0; i < tempMajorPoints.Length; ++i)
                        {
                            // we need to offset to the major shape points and step over
                            // the points on the edges, so that we can iterate them in the
                            // for loop below
                            int majorPointOffset = i * (shapePattern.pointsOnEdges + 1);
                            shapePoints[majorPointOffset] = tempMajorPoints[i];

                            // create a line segment that can be used to lerp across for the
                            // positions of the points on the edges (subdivisions)
                            Vector2 lineSegment = Vector2.zero;
                            Vector2 world = worldPosition; // just for implicit conversion from Vec3 to Vec2

                            if(i < (tempMajorPoints.Length - 1))
                            {
                                lineSegment = tempMajorPoints[i + 1].local - tempMajorPoints[i].local;
                            }
                            else
                            {
                                lineSegment = tempMajorPoints[0].local - tempMajorPoints[i].local;
                            }

                            // Create the points on the edges (subdivisions)
                            for(int j = 1; j <= shapePattern.pointsOnEdges; ++j) // j is an offset, not index
                            {
                                // interpolate across the length of the segment
                                float segmentVectorScale = ((float)j / (((float)shapePattern.pointsOnEdges) + 1.0f));

                                // scale this point across the segment
                                Vector2 pointOnLineSegment = lineSegment * segmentVectorScale;
                                
                                Vector2 pointLocation = tempMajorPoints[i].local + pointOnLineSegment;

                                shapePoints[majorPointOffset + j].local = new Vector2(pointLocation.x, pointLocation.y);
                                shapePoints[majorPointOffset + j].world = (Vector2)worldPosition + shapePoints[majorPointOffset + j].local;

                                Vector2 normal = shapePoints[majorPointOffset + j].local.normalized;
                                shapePoints[majorPointOffset + j].normal = new Vector2(normal.x, normal.y);
                                shapePoints[majorPointOffset + j].normalWorld = new Vector2(normal.x, normal.y);
                            }
                        }
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
                            Gizmos.DrawSphere(pointPosition, 0.2f);

                            Color cachedGizmoColor = Gizmos.color;
                            Gizmos.color = Color.green;
                            Gizmos.DrawLine(shapePoints[i].world, shapePoints[i].world + shapePoints[i].normalWorld);
                            Gizmos.color = cachedGizmoColor;

                            if(i % (shapePattern.pointsOnEdges + 1) == 0)
                            {
                                if(i < shapePoints.Length - shapePattern.pointsOnEdges - 1)
                                {
                                    Gizmos.DrawLine(shapePoints[i].world, shapePoints[i + shapePattern.pointsOnEdges + 1].world);
                                }
                                else
                                {
                                    Gizmos.DrawLine(shapePoints[i].world, shapePoints[0].world);
                                }
                            }
                        }
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawRay(origin, forward * 0.8f);
                    }
                }// LineImplementation
            }// Shape
        }// Patterns
    }// BulletHeck
}// HeckYeahGames