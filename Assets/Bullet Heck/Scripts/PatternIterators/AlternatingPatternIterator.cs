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
using System.Collections.Generic;

namespace HeckYeahGames
{
    namespace BulletHeck
    {
        namespace Patterns
        {
            namespace Shape
            {
                public class AlternatingPatternIterator : BasePatternIterator, IPatternIterator
                {
                    Stack<ShapePoint[]> m_sortedPoints;
                    ShapePoint[] m_evenPoints;
                    ShapePoint[] m_oddPoints;
                    bool m_pointsHaveBeenSorted;
                    public ShapePoint[] Current()
                    {
                        return m_currentShapePoints;
                    }

                    public bool Done()
                    {
                        return m_sortedPoints.Count == 0;
                    }

                    public ShapePoint[] First()
                    {
                        Init();

                        m_currentShapePoints = m_sortedPoints.Pop();

                        return m_currentShapePoints;
                    }

                    public ShapePoint[] Next()
                    {
                        if(m_patternShapePoints == null || Done())
                        {
                            return null;
                        }

                        m_currentShapePoints = m_sortedPoints.Pop();

                        return m_currentShapePoints;
                    }

                    public AlternatingPatternIterator(bool isReversed, ShapePoint[] patternSnapePoints) : base(isReversed, patternSnapePoints)
                    {
                        m_sortedPoints = new Stack<ShapePoint[]>();
                        m_pointsHaveBeenSorted = false;
                    }

                    protected override void Init()
                    {
                        if(m_patternShapePoints == null || m_patternShapePoints.Length <= 0)
                        {
                            m_currentShapePoints = null;
                            return;
                        }

                        if(m_pointsHaveBeenSorted == false)
                        {
                            // Sort even and odd points
                            SortEvenAndOdd();
                        }

                        m_currentShapePoints = null;

                        // clear the stack
                        m_sortedPoints.Clear();

                        // Add them to the stack
                        if(IsReversed)
                        {
                            // even then odd
                            if(m_evenPoints != null)
                            {
                                m_sortedPoints.Push(m_evenPoints);
                            }
                            m_sortedPoints.Push(m_oddPoints);
                        }
                        else
                        {
                            // odd then even
                            m_sortedPoints.Push(m_oddPoints);
                            if(m_evenPoints != null)
                            {
                                m_sortedPoints.Push(m_evenPoints);
                            }
                        }
                    }

                    private void SortEvenAndOdd()
                    {
                        List<ShapePoint> even = new List<ShapePoint>();
                        List<ShapePoint> odd = new List<ShapePoint>();
                        for(int i = 0; i < m_patternShapePoints.Length; ++i)
                        {
                            if(i % 2 == 0)
                            {
                                even.Add(m_patternShapePoints[i]);
                            }
                            else
                            {
                                odd.Add(m_patternShapePoints[i]);
                            }
                        }

                        m_oddPoints = odd.ToArray();
                        if(even.Count > 0)
                        {
                            m_evenPoints = even.ToArray();
                        }
                        m_pointsHaveBeenSorted = true;
                    }
                }// AlternatingPatternIterator
            }// Shape
        }// Patterns
    }// BulletHeck
}// HeckYeahGames
