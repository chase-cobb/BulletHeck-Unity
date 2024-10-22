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
                public class LinearPatternIterator : BasePatternIterator, IPatternIterator
                {
                    public ShapePoint[] Current()
                    {
                        return m_currentShapePoints;
                    }

                    public bool Done()
                    {
                        return (m_offset < 0 || m_offset >= m_patternShapePoints.Length);
                    }

                    public ShapePoint[] First()
                    {
                        Init();

                        if(m_patternShapePoints == null || Done())
                        {
                            return null;
                        }

                        m_currentShapePoints[0] = m_patternShapePoints[m_offset];

                        //AdvanceIterator();

                        return m_currentShapePoints;
                    }

                    public ShapePoint[] Next()
                    {
                        AdvanceIterator();
                        if(m_patternShapePoints != null && !Done())
                        {
                            m_currentShapePoints[0] = m_patternShapePoints[m_offset];

                            return m_currentShapePoints;
                        }

                        return null;
                    }

                    public LinearPatternIterator(bool isReversed, ShapePoint[] patternSnapePoints) : base(isReversed, patternSnapePoints)
                    {
                        Init();
                    }

                    protected override void Init()
                    {
                        m_currentShapePoints = new ShapePoint[1];
                        if(IsReversed)
                        {
                            m_offset = m_patternShapePoints.Length - 1;
                        }
                        else
                        {
                            m_offset = 0;
                        }
                    }
                }// LinearPatternIterator
            }// Shape
        }// Patterns
    }// BulletHeck
}// HeckYeahGames