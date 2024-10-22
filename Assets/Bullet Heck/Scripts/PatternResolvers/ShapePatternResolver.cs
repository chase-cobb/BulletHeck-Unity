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

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HeckYeahGames
{
    namespace BulletHeck
    {
        namespace Patterns
        {
            namespace Shape
            {
                public delegate void OnPatternRebuild(ShapePoint[] shapePoints);
                public struct ShapePoint
                {
                    internal Vector2 local;
                    public Vector2 world; // gets recalculated
                    internal Vector2 normal;
                    public Vector2 normalWorld; // gets recalculated
                }

                [ExecuteInEditMode]
                public class ShapePatternResolver : MonoBehaviour, IPatternListener
                {
                    [SerializeField]
                    internal ShapePattern m_shapePattern;

                    private IShapeImplementation m_shapeImplementation;

                    private ShapePoint[] m_shapePoints;

                    [SerializeField]
                    internal float m_rotationOffset;
                    private Vector2 m_forwardVector = Vector2.right;

                    public event OnPatternRebuild m_onPatternRebuild;

                    private bool m_patternHasBeenBuilt;

                    void OnEnable()
                    {
                        m_patternHasBeenBuilt = false;
                        Initialize();
                    }

                    void OnDisable()
                    {
                        if (m_shapePattern != null)
                        {
                            m_shapePattern.UnregisterOnChangeEventHandler(OnPatternChange);
                        }
                    }

                    // Update is called once per frame
                    void Update()
                    {
                        if (m_shapePattern != null)
                        {
                            if (m_shapeImplementation == null)
                            {
#if UNITY_EDITOR
                                //This helps initalize everything in the editor in instances where
                                //changes are compiled and Start is missed
                                Initialize();
#endif
                            }
                            // calculate offset
                            m_rotationOffset += m_shapePattern.rotationDegreesPerSecond * Time.deltaTime;

                            if (m_rotationOffset > 360.0f)
                            {
                                m_rotationOffset -= 360.0f;
                            }
                            else if (m_rotationOffset < 0.0f)
                            {
                                m_rotationOffset += 360.0f;
                            }

                            // perform rotation
                            m_shapeImplementation.Update(transform.position, m_shapePattern, ref m_shapePoints, m_rotationOffset);
                        }
                    }

                    public IPatternIterator GetPatternIterator<T>(bool isReversed = false) where T : IPatternIterator
                    {
                        IPatternIterator newIteratorInterface = null;
                        BasePatternIterator newIteratorObject = null;

                        if (typeof(T) == typeof(LinearPatternIterator))
                        {
                            newIteratorObject = new LinearPatternIterator(isReversed, m_shapePoints);
                        }
                        else if (typeof(T) == typeof(BilinearPatternIterator))
                        {
                            newIteratorObject = new BilinearPatternIterator(isReversed, m_shapePoints);
                        }
                        else if (typeof(T) == typeof(AlternatingPatternIterator))
                        {
                            newIteratorObject = new AlternatingPatternIterator(isReversed, m_shapePoints);
                        }
                        else
                        {
                            Debug.LogError("ShapePatternResolver.GetPatternIterator : Incorrect iterator type requested");
                            return null;
                        }

                        newIteratorInterface = newIteratorObject as IPatternIterator;

                        return newIteratorInterface;
                    }

                    public bool PatternHasBeenBuilt()
                    {
                        return m_patternHasBeenBuilt;
                    }

                    public int GetNumberOfShapePoints()
                    {
                        return m_shapePoints.Length;
                    }

                    public void GetShapePointDataAtIndex(int index, ref Vector2 position, ref Vector2 normal)
                    {
                        if (m_shapePoints != null && m_shapePoints.Length > index && index >= 0)
                        {
                            position = m_shapePoints[index].world;
                            normal = m_shapePoints[index].normalWorld;
                        }
                    }

                    void OnDrawGizmosSelected()
                    {
#if UNITY_EDITOR
                        // Ensure continuous Update calls.
                        if (!Application.isPlaying)
                        {
                            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
                            UnityEditor.SceneView.RepaintAll();
                        }
#endif

                        if (m_shapePattern != null)
                        {
                            Gizmos.color = Color.cyan;

                            if (m_shapeImplementation != null)
                            {
                                m_forwardVector = MathHelper.GetNormalizedVectorFromRotationInDegrees(m_rotationOffset);
                                m_shapeImplementation.OnSelectedGizmoDraw(transform.position, m_forwardVector, m_shapePattern, ref m_shapePoints);
                            }
                        }
                    }

                    private void BuildShape()
                    {
                        m_shapeImplementation = null;

                        switch (m_shapePattern.currentGeometry)
                        {
                            case ShapePattern.Geometry.CIRCLE:
                                {
                                    m_shapeImplementation = new CircleImplementation();
                                    break;
                                }
                            case ShapePattern.Geometry.LINE:
                                {
                                    m_shapeImplementation = new LineImplementation();
                                    m_shapePattern.numberOfPoints = 2;
                                    break;
                                }
                            case ShapePattern.Geometry.ARC:
                                {
                                    m_shapeImplementation = new ArcImplementation();
                                    break;
                                }
                            case ShapePattern.Geometry.N_SIDED:
                                {
                                    m_shapeImplementation = new NSidedImplementation();
                                    if (m_shapePattern.numberOfPoints < 3)
                                    {
                                        m_shapePattern.numberOfPoints = 3;
                                    }
                                    break;
                                }
                            case ShapePattern.Geometry.N_SIDED_COMPLEX:
                                {
                                    m_shapeImplementation = new ComplexNSidedImplementation();
                                    if (m_shapePattern.numberOfPoints < 6)
                                    {
                                        m_shapePattern.numberOfPoints = 6;
                                    }
                                    m_shapePattern.spreadInDegrees = 360;
                                    break;
                                }
                        }

                        if (m_shapeImplementation != null)
                        {
                            m_shapeImplementation.Build(transform.position, m_shapePattern, ref m_shapePoints);
                            Vector2 position = transform.position;
                            m_forwardVector = MathHelper.GetNormalizedVectorFromRotationInDegrees(m_shapePattern.forwardRotationInDegrees) * (m_shapePattern.maxRadius);
                            int numberOfPoints = m_shapePoints.Length;
                            m_onPatternRebuild?.Invoke(m_shapePoints);
                            m_patternHasBeenBuilt = true;
                        }
                    }

                    public void OnPatternChange()
                    {
                        BuildShape();
                    }

                    internal void Initialize()
                    {
                        if (m_shapePattern == null)
                        {
                            return;
                        }

                        m_shapePattern.UnregisterOnChangeEventHandler(OnPatternChange);
                        m_shapePattern.RegisterOnChangeEventHandler(OnPatternChange);
                        BuildShape();
                    }

                    internal void OnBeforeShapePatternReplace()
                    {
                        if (m_shapePattern != null)
                        {
                            Debug.Log("OnBeforeShapePatternReplace");
                            m_shapePattern.UnregisterOnChangeEventHandler(OnPatternChange);
                        }
                    }

                    internal void OnAfterShapePatternReplace()
                    {
                        if (m_shapePattern != null)
                        {
                            Debug.Log("OnAfterShapePatternReplace");
                            m_shapePattern.RegisterOnChangeEventHandler(OnPatternChange);
                        }
                    }
                }// ShapePatternResolver

#if UNITY_EDITOR
                [CustomEditor(typeof(ShapePatternResolver))]
                class ShapePatternResolverEditor : Editor
                {
                    // This custom editor is defined in this file to make sure Unity
                    // puts both classes in the same assembly, to keep access to members
                    // and methods with the "internal" keyword. Placing this file in
                    // an "Editor" folder compiles it into the editor assemblies. :(
                    //https://docs.unity3d.com/Manual/ScriptCompilationAssemblyDefinitionFiles.html#defining-assemblies
                    public override void OnInspectorGUI()
                    {
                        var shapePatternResolver = (ShapePatternResolver)target;
                        if (shapePatternResolver == null)
                            return;

                        Undo.RecordObject(shapePatternResolver, "Undo changes");

                        ShapePattern shapePattern = (ShapePattern)EditorGUILayout.ObjectField("Primary Pattern : ",
                                                                                            shapePatternResolver.m_shapePattern,
                                                                                            typeof(ShapePattern),
                                                                                            true);

                        if (shapePattern != shapePatternResolver.m_shapePattern)
                        {
                            shapePatternResolver.OnBeforeShapePatternReplace();
                            shapePatternResolver.m_shapePattern = shapePattern;
                            shapePatternResolver.OnPatternChange(); // Force rebuild of pattern
                            shapePatternResolver.OnAfterShapePatternReplace();
                        }

                        // If there is a primary shape pattern, draw it's inspector options
                        if (shapePatternResolver.m_shapePattern != null)
                        {
                            ShapePatternEditor shapePatternEditor = (ShapePatternEditor)ShapePatternEditor.CreateEditor(shapePatternResolver.m_shapePattern);
                            shapePatternEditor.OnInspectorGUI();
                        }

                        shapePatternResolver.m_rotationOffset = EditorGUILayout.Slider("Rotation",
                                                                                       shapePatternResolver.m_rotationOffset,
                                                                                       0.0f,
                                                                                       360.0f);
                    }
                }// ShapePatternEditor
#endif
            }// Shape
        }// Patterns
    }// BulletHeck
}// HeckYeahGames
