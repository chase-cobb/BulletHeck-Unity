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
                public class BilinearPatternIterator : BasePatternIterator, IPatternIterator
                {
                    private int m_offsetUpperlimit = 0;
                    private int m_halfLength = 0;
                    private bool m_isEvenLength = false;

                    public ShapePoint[] Current()
                    {
                        return m_currentShapePoints;
                    }

                    public bool Done()
                    {
                        return (m_offset < 0 || m_offset >= m_offsetUpperlimit);
                    }

                    public ShapePoint[] First()
                    {
                        Init();

                        if(m_patternShapePoints == null || Done())
                        {
                            return null;
                        }

                        GetPointsAtCurrentOffset();

                        //AdvanceIterator();
                        return m_currentShapePoints;
                    }

                    public ShapePoint[] Next()
                    {
                        AdvanceIterator();
                        if(m_patternShapePoints == null || Done())
                        {
                            return null;
                        }

                        GetPointsAtCurrentOffset();

                        return m_currentShapePoints;
                    }

                    private void GetPointsAtCurrentOffset()
                    {
                        if(m_isEvenLength)
                        {
                            m_currentShapePoints[0] = m_patternShapePoints[m_halfLength + m_offset];
                            m_currentShapePoints[1] = m_patternShapePoints[m_halfLength - 1 - m_offset];
                        }
                        else
                        {
                            int indexOne = m_halfLength - m_offset;
                            int indexTwo = m_halfLength + 1 + m_offset;
                            
                            m_currentShapePoints[0] = m_patternShapePoints[indexOne];
                            if(m_offset == m_halfLength)
                            {
                                m_currentShapePoints[1] = new ShapePoint();
                            }
                            else
                            {
                                m_currentShapePoints[1] = m_patternShapePoints[indexTwo];
                            }
                        }
                    }

                    public BilinearPatternIterator(bool isReversed, ShapePoint[] patternSnapePoints) : base(isReversed, patternSnapePoints)
                    {
                        Init();
                    }

                    protected override void Init()
                    {
                        m_currentShapePoints = new ShapePoint[2];
                        m_isEvenLength = m_patternShapePoints.Length % 2 == 0;
                        m_halfLength = m_patternShapePoints.Length / 2;
                        if(IsReversed)
                        {
                            if(m_isEvenLength)
                            {
                                m_offset = m_halfLength - 1;
                            }
                            else
                            {
                                m_offset = m_halfLength;
                            }
                        }
                        else
                        {
                            m_offset = 0;
                        }

                        if(m_isEvenLength)
                        {
                            m_offsetUpperlimit = m_halfLength;
                        }
                        else
                        {
                            m_offsetUpperlimit = m_halfLength + 1;
                        }
                    }
                }// BilinearPatternIterator
            }// Shape
        }// Patterns
    }// BulletHeck
}// HeckYeahGames
