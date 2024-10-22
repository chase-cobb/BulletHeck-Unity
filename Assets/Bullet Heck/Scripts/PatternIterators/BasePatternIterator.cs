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
                public abstract class BasePatternIterator
                {
                    protected bool IsReversed { get; set; } = false;
                    protected int m_offset = 0;
                    protected ShapePoint[] m_patternShapePoints;
                    protected ShapePoint[] m_currentShapePoints;

                    protected BasePatternIterator(bool isReversed, ShapePoint[] patternSnapePoints)
                    {
                        IsReversed = isReversed;
                        m_offset = 0;
                        m_patternShapePoints = patternSnapePoints;
                        m_currentShapePoints = null;
                    }

                    protected abstract void Init();

                    protected void AdvanceIterator()
                    {
                        if(IsReversed)
                        {
                            m_offset--;
                        }
                        else
                        {
                            m_offset++;
                        }
                    }
                    // public void RegisterHandlerForEvent(ref OnPatternRebuild patternRebuildEvent)
                    // {
                    //     patternRebuildEvent += OnPatternRebuildHander;
                    // }

                    // protected void OnPatternRebuildHander(ShapePoint[] shapePoints)
                    // {
                    //     Debug.Log("~~~~~~Pattern rebuild handled by pattern iterator!!");
                    //     m_patternShapePoints = shapePoints;
                    //     m_offset = 0;
                    //     m_currentShapePoints = null;
                    //     Init();
                    // }
                }// BasePatternIterator
            }// Shape
        }// Patterns
    }// BulletHeck
}// HeckYeahGames
