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
using System;
using HeckYeahGames.BulletHeck.Patterns.Shape;
using UnityEngine;

namespace HeckYeahGames
{
    namespace BulletHeck
    {
        public static class MathHelper
        {
            /// <summary>
            /// Given an array of Vector2, this function will populate the array with a set of points on a radius.
            /// </summary>
            /// <param name="points">An allocated array to be filled out by this function.</param>
            /// <param name="relativePosition">Pass Vector2.Zero for local space, otherwise pass a global position for world space.</param>
            /// <param name="radius">The distance from the relative position that the points should be located</param>
            /// <param name="spreadAngle">The angle the points will be interpolated across.</param>
            /// <param name="offsetAngle">A rotational offset to change overall spread rotation. </param>
            /// <param name="fullSpread">Should the points evenly cover the entire spread (0-n inclusive)</param>
            /// <param name="centerSpread">Should the spread be split across the center</param>
            public static void GetPointsOnRadius(ref ShapePoint[] points, Vector2 relativePosition, float radius, float spreadAngle, float offsetAngle, bool fullSpread = false, bool centerSpread = false)
            {
                if (points == null || points.Length < 1)
                {
                    return;
                }

                float angleSlice = 0.0f;
                float centerOffset = 0.0f;

                // divide the angles of each point into slices
                if(!fullSpread)
                {
                    // This prevents overlap of the 0 and nth points
                    angleSlice = spreadAngle / points.Length;
                }
                else
                {
                    // This makes sure the spread is fully inclusive of the spread angle
                    angleSlice = spreadAngle / (points.Length - 1);
                }

                if(centerSpread)
                {
                    centerOffset = spreadAngle / 2.0f;
                }

                for (int i = 0; i < points.Length; i++)
                {
                    float angle = UnityEngine.Mathf.Deg2Rad * ((angleSlice * i) + offsetAngle - centerOffset);
                    Vector2 normalizedDirection = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)).normalized;
                    points[i].local = (normalizedDirection * radius);
                    points[i].world = relativePosition + points[i].local;
                    points[i].normal = points[i].normalWorld = normalizedDirection;
                }
            }

            public static void GetComplexPointsOnMultipleRadius(ref ShapePoint[] points, Vector2 relativePosition, ShapePattern shapePattern, float offsetAngle, bool fullSpread = false, bool centerSpread = false)
            {
                if (points == null || points.Length < 1)
                {
                    return;
                }

                float angleSlice = 0.0f;
                float centerOffset = 0.0f;

                // divide the angles of each point into slices
                if(!fullSpread)
                {
                    // This prevents overlap of the 0 and nth points
                    angleSlice = shapePattern.spreadInDegrees / points.Length;
                }
                else
                {
                    // This makes sure the spread is fully inclusive of the spread angle
                    angleSlice = shapePattern.spreadInDegrees / (points.Length - 1);
                }

                if(centerSpread)
                {
                    centerOffset = shapePattern.spreadInDegrees / 2.0f;
                }

                for (int i = 0; i < points.Length; i++)
                {
                    float angle = UnityEngine.Mathf.Deg2Rad * ((angleSlice * i) + offsetAngle - centerOffset);
                    Vector2 normalizedDirection = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)).normalized;
                    if(i % 2 == 0)
                    {
                        points[i].local = (normalizedDirection * shapePattern.maxRadius);
                    }
                    else
                    {
                        points[i].local = (normalizedDirection * shapePattern.minRadius);
                    }
                    points[i].world = relativePosition + points[i].local;
                    points[i].normal = points[i].normalWorld = normalizedDirection;
                }
            }

            public static void RotatePoints(ref ShapePoint[] points, Vector2 origin, float rotationOffset)
            {
                float rotationAmount = Mathf.Deg2Rad * rotationOffset;
                for(int i = 0; i < points.Length; ++i)
                {
                    // calculate rotation based on localPoints
                    points[i].world.x = points[i].local.x * UnityEngine.Mathf.Cos(rotationAmount) - points[i].local.y * UnityEngine.Mathf.Sin(rotationAmount);
                    points[i].world.y = points[i].local.x * UnityEngine.Mathf.Sin(rotationAmount) + points[i].local.y * UnityEngine.Mathf.Cos(rotationAmount);

                    // calculate normals in world space
                    points[i].normalWorld.x = points[i].normal.x * UnityEngine.Mathf.Cos(rotationAmount) - points[i].normal.y * UnityEngine.Mathf.Sin(rotationAmount);
                    points[i].normalWorld.y = points[i].normal.x * UnityEngine.Mathf.Sin(rotationAmount) + points[i].normal.y * UnityEngine.Mathf.Cos(rotationAmount);

                    // move into world space
                    points[i].world += origin;
                }
            }

            public static Vector2 GetNormalizedVectorFromRotationInDegrees(float rotation)
            {
                float rotationInRads = Mathf.Deg2Rad * rotation;
                Vector2 rotatedVector = new Vector2((float)Math.Cos(rotationInRads), (float)Math.Sin(rotationInRads)).normalized;
                return rotatedVector;
            }

            public static Vector2 Rotate(Vector2 vector2, float angleInRadians)
            {
                return new Vector2(
                    vector2.x * Mathf.Cos(angleInRadians) - vector2.y * Mathf.Sin(angleInRadians),
                    vector2.x * Mathf.Sin(angleInRadians) + vector2.y * Mathf.Cos(angleInRadians));
            }
        }// MathHelper
    }// BulletHeck
}// HeckYeahGames
