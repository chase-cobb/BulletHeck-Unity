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
using System;

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
                /// <summary>
                /// Data container that provides the constraints required
                /// to build a shape pattern.
                /// 
                /// See ShapePatternResolver.cs
                /// </summary>
                [CreateAssetMenu(fileName = "Shape Pattern Object", menuName = "Heck Yeah Games/Patterns/Shape"), Serializable]
                public class ShapePattern : ScriptableObject, IPattern
                {
                    public enum Geometry
                    {
                        CIRCLE,
                        LINE,
                        ARC,
                        N_SIDED,
                        N_SIDED_COMPLEX
                    }

                    public Geometry currentGeometry = Geometry.CIRCLE;

                    [SerializeField]
                    internal float maxRadius = 1.0f;
                    [SerializeField]
                    internal float minRadius = 0.2f;
                    [SerializeField]
                    internal int numberOfPoints = 3;
                    [SerializeField]
                    internal int pointsOnEdges = 0;
                    [SerializeField]
                    internal float forwardRotationInDegrees = 0.0f;
                    [SerializeField]
                    internal float rotationDegreesPerSecond = 0.0f;
                    [SerializeField]
                    internal bool reverseRotation = false;
                    [SerializeField]
                    internal float spreadInDegrees = 35.0f;

                    // Should be fired when any property changes that requires a resolver recalculation.
                    private event OnPatternChange m_onChange;

                    public void RegisterOnChangeEventHandler(OnPatternChange handler)
                    {
                        if(handler != null)
                        {
                            m_onChange += handler;
                        }
                    }
                    public void UnregisterOnChangeEventHandler(OnPatternChange handler)
                    {
                        if(handler != null)
                        {
                            m_onChange -= handler;
                        }
                    }

                    // Allows Inspector events in this namespace to trigger this event
                    internal void FireOnChangeEvent()
                    {
                        m_onChange?.Invoke();
                    }

                }// ShapePattern

                #if UNITY_EDITOR
                [CustomEditor(typeof(ShapePattern))]
                class ShapePatternEditor : Editor
                {
                    // This custom editor is defined in this file to make sure Unity
                    // puts both classes in the same assembly, to keep access to members
                    // and methods with the "internal" keyword. Placing this file in
                    // an "Editor" folder compiles it into the editor assemblies. :(
                    //https://docs.unity3d.com/Manual/ScriptCompilationAssemblyDefinitionFiles.html#defining-assemblies

                    [Flags]
                    public enum DrawProperties
                    {
                        NONE = 0,
                        GEOMETRY = 1,
                        MAX_RADIUS = 2,
                        MIN_RADIUS = 4,
                        NUMBER_OF_POINTS = 8,
                        POINTS_ON_EDGES = 16,
                        FORWARD_ROTATION = 32,
                        ROTATION_DEG_PER_SECOND = 64,
                        REVERSE_ROTATION = 128,
                        SPREAD_IN_DEGREES = 256,
                    }

                    protected struct Constraints
                    {
                        // radius
                        public float minRadius;
                        public float maxRadius;

                        // number of points
                        public int minPoints;
                        public int maxPoints;
                        public bool pointsDivisibleByTwo;

                        // points on edges
                        public int minPointsOnEdges;
                        public int maxPointsOnEdges;

                        // rotation degrees per second
                        public float maxRotationDegreesPerSecond;

                        // spread in degrees
                        public float maxSpreadInDegrees;

                        public Constraints(float minRadius, float maxRadius, int minPoints, int maxPoints, bool pointsDivisibleByTwo, int minPointsOnEdges, int maxPointsOnEdges, float maxRotationDegreesPerSecond, float maxSpreadInDegrees)
                        {
                            this.minRadius = minRadius;
                            this.maxRadius = maxRadius;
                            this.minPoints = minPoints;
                            this.maxPoints = maxPoints;
                            this.pointsDivisibleByTwo = pointsDivisibleByTwo;
                            this.minPointsOnEdges = minPointsOnEdges;
                            this.maxPointsOnEdges = maxPointsOnEdges;
                            this.maxRotationDegreesPerSecond = maxRotationDegreesPerSecond;
                            this.maxSpreadInDegrees = maxSpreadInDegrees;

                        }
                    }

                    SerializedProperty m_currentGeometry;
                    SerializedProperty m_maxRadius;
                    SerializedProperty m_minRadius;
                    SerializedProperty m_numberOfPoints;
                    SerializedProperty m_pointsOnEdges;
                    SerializedProperty m_forwardRotationInDegrees;
                    SerializedProperty m_rotationDegreesPerSecond;
                    SerializedProperty m_reverseRotation;
                    SerializedProperty m_spreadInDegrees;

                    DrawProperties m_drawProperties;

                    Constraints m_constraints;

                    void OnEnable()
                    {
                        m_currentGeometry = serializedObject.FindProperty("currentGeometry");
                        m_maxRadius = serializedObject.FindProperty("maxRadius");
                        m_minRadius = serializedObject.FindProperty("minRadius");
                        m_numberOfPoints = serializedObject.FindProperty("numberOfPoints");
                        m_pointsOnEdges = serializedObject.FindProperty("pointsOnEdges");
                        m_forwardRotationInDegrees = serializedObject.FindProperty("forwardRotationInDegrees");
                        m_rotationDegreesPerSecond = serializedObject.FindProperty("rotationDegreesPerSecond");
                        m_reverseRotation = serializedObject.FindProperty("reverseRotation");
                        m_spreadInDegrees = serializedObject.FindProperty("spreadInDegrees");

                        SetInspectorPropertyFlags(DrawProperties.GEOMETRY);
                    }

                    //https://youtu.be/J_Q6Bpk4XDc?si=xiSDK_CJ6zCeHdx1
                    public override void OnInspectorGUI()
                    {
                        var shapePattern = (ShapePattern)target;
                        if(shapePattern == null)
                             return;

                        serializedObject.Update();

                        ShapePattern.Geometry serializedGeometry = (ShapePattern.Geometry)m_currentGeometry.enumValueFlag;
                        UpdateShapeEditorGUI(serializedGeometry);

                        if(IsPropertyEnabled(DrawProperties.GEOMETRY))
                        {
                            EditorGUI.BeginChangeCheck();

                            ShapePattern.Geometry geometry = (ShapePattern.Geometry)EditorGUILayout.EnumPopup("Geometry", (ShapePattern.Geometry)m_currentGeometry.enumValueFlag);

                            if(EditorGUI.EndChangeCheck())
                            {
                                SetShapeEditorGUI(geometry);
                                serializedObject.ApplyModifiedProperties();
                                shapePattern.FireOnChangeEvent();
                            }
                        }

                        EditorGUI.indentLevel++;

                        // number of points
                        if(IsPropertyEnabled(DrawProperties.NUMBER_OF_POINTS))
                        {
                            EditorGUI.BeginChangeCheck();

                            float preEditValue = m_numberOfPoints.intValue;

                            m_numberOfPoints.intValue = EditorGUILayout.IntSlider("Points", m_numberOfPoints.intValue, m_constraints.minPoints, m_constraints.maxPoints);

                            if(m_constraints.pointsDivisibleByTwo)
                            {
                                if(m_numberOfPoints.intValue % 2 != 0)
                                {
                                    if(m_numberOfPoints.intValue < preEditValue) // user is sliding value down, so round down
                                    {
                                        m_numberOfPoints.intValue--;
                                    }
                                    else
                                    {
                                        m_numberOfPoints.intValue++;
                                    }
                                }
                            }

                            if(EditorGUI.EndChangeCheck())
                            {
                                shapePattern.FireOnChangeEvent();
                            }
                        }

                        // points on edges
                        if(IsPropertyEnabled(DrawProperties.POINTS_ON_EDGES))
                        {
                            EditorGUI.BeginChangeCheck();

                            m_pointsOnEdges.intValue = EditorGUILayout.IntSlider("Points on Edges", m_pointsOnEdges.intValue, m_constraints.minPointsOnEdges, m_constraints.maxPointsOnEdges);

                            if(EditorGUI.EndChangeCheck())
                            {
                                shapePattern.FireOnChangeEvent();
                            }
                        }

                        // min radius
                        if(IsPropertyEnabled(DrawProperties.MIN_RADIUS))
                        {
                            EditorGUI.BeginChangeCheck();

                            m_minRadius.floatValue = EditorGUILayout.Slider("Min Radius", m_minRadius.floatValue, m_constraints.minRadius, m_constraints.maxRadius);

                            if(EditorGUI.EndChangeCheck())
                            {
                                shapePattern.FireOnChangeEvent();
                            }
                        }

                        // max radius
                        if(IsPropertyEnabled(DrawProperties.MAX_RADIUS))
                        {
                            EditorGUI.BeginChangeCheck();

                            m_maxRadius.floatValue = EditorGUILayout.Slider("Max Radius", m_maxRadius.floatValue, m_constraints.minRadius, m_constraints.maxRadius);

                            if(EditorGUI.EndChangeCheck())
                            {
                                shapePattern.FireOnChangeEvent();
                            }
                        }

                        // forward rotation in degrees
                        if(IsPropertyEnabled(DrawProperties.FORWARD_ROTATION))
                        {
                            m_forwardRotationInDegrees.floatValue = EditorGUILayout.Slider("Forward Rotation", m_forwardRotationInDegrees.floatValue, 0.0f, 360.0f);
                        }

                        if(IsPropertyEnabled(DrawProperties.ROTATION_DEG_PER_SECOND))
                        {
                            m_rotationDegreesPerSecond.floatValue = EditorGUILayout.Slider("Rotation Degrees Per Second", m_rotationDegreesPerSecond.floatValue, 0.0f, m_constraints.maxRotationDegreesPerSecond);
                        }

                        // reverse rotation
                        if(IsPropertyEnabled(DrawProperties.REVERSE_ROTATION))
                        {
                            EditorGUI.indentLevel++;
                            m_reverseRotation.boolValue = EditorGUILayout.Toggle("Reverse", m_reverseRotation.boolValue);
                            EditorGUI.indentLevel--;
                        }

                        // spread in degrees
                        if(IsPropertyEnabled(DrawProperties.SPREAD_IN_DEGREES))
                        {
                            m_spreadInDegrees.floatValue = EditorGUILayout.Slider("Angle Spread", m_spreadInDegrees.floatValue, 0.0f, m_constraints.maxSpreadInDegrees);
                        }

                        EditorGUI.indentLevel--;

                        serializedObject.ApplyModifiedProperties();
                    }

                    private void SetInspectorPropertyFlags(DrawProperties properties)
                    {
                        m_drawProperties = properties;
                    }

                    private bool IsPropertyEnabled(DrawProperties property)
                    {
                        return ((m_drawProperties & property) != 0);
                    }

                    private void SetShapeEditorGUI(ShapePattern.Geometry geometry)
                    {
                        m_currentGeometry.enumValueFlag = (int)geometry;
                        UpdateShapeEditorGUI(geometry);
                    }

                    private void UpdateShapeEditorGUI(ShapePattern.Geometry geometry)
                    {
                        switch(geometry)
                        {
                            case ShapePattern.Geometry.CIRCLE:
                            {
                                SetInspectorPropertyFlags(DrawProperties.GEOMETRY |
                                                          DrawProperties.MAX_RADIUS |
                                                          DrawProperties.NUMBER_OF_POINTS |
                                                          DrawProperties.ROTATION_DEG_PER_SECOND|
                                                          DrawProperties.REVERSE_ROTATION);

                                m_constraints = new Constraints(0.0f, 10.0f, 1, 20, false, 0, 0, 300.0f, 360.0f);
                                break;
                            }
                            case ShapePattern.Geometry.LINE:
                            {
                                SetInspectorPropertyFlags(DrawProperties.GEOMETRY |
                                                          DrawProperties.MAX_RADIUS |
                                                          DrawProperties.SPREAD_IN_DEGREES |
                                                          DrawProperties.ROTATION_DEG_PER_SECOND|
                                                          DrawProperties.REVERSE_ROTATION |
                                                          DrawProperties.POINTS_ON_EDGES);

                                m_constraints = new Constraints(0.0f, 10.0f, 2, 2, false, 0, 10, 300.0f, 180.0f);
                                break;
                            }
                            case ShapePattern.Geometry.ARC:
                            {
                                SetInspectorPropertyFlags(DrawProperties.GEOMETRY |
                                                          DrawProperties.MAX_RADIUS |
                                                          DrawProperties.SPREAD_IN_DEGREES |
                                                          DrawProperties.ROTATION_DEG_PER_SECOND|
                                                          DrawProperties.REVERSE_ROTATION |
                                                          DrawProperties.NUMBER_OF_POINTS);

                                m_constraints = new Constraints(0.0f, 10.0f, 2, 10, false, 0, 10, 300.0f, 180.0f);
                                break;
                            }
                            case ShapePattern.Geometry.N_SIDED:
                            {
                                SetInspectorPropertyFlags(DrawProperties.GEOMETRY |
                                                          DrawProperties.MAX_RADIUS |
                                                          DrawProperties.POINTS_ON_EDGES |
                                                          DrawProperties.ROTATION_DEG_PER_SECOND|
                                                          DrawProperties.REVERSE_ROTATION |
                                                          DrawProperties.NUMBER_OF_POINTS);

                                m_constraints = new Constraints(0.0f, 10.0f, 3, 20, false, 0, 12, 300.0f, 360.0f);
                                break;
                            }
                            case ShapePattern.Geometry.N_SIDED_COMPLEX:
                            {
                                SetInspectorPropertyFlags(DrawProperties.GEOMETRY |
                                                          DrawProperties.MAX_RADIUS |
                                                          DrawProperties.MIN_RADIUS |
                                                          DrawProperties.ROTATION_DEG_PER_SECOND|
                                                          DrawProperties.REVERSE_ROTATION |
                                                          DrawProperties.NUMBER_OF_POINTS);

                                m_constraints = new Constraints(0.0f, 10.0f, 6, 20, true, 0, 0, 300.0f, 360.0f);
                                break;
                            }
                        }
                    }
                }// ShapePatternEditor
                #endif
            }// Shape
        }// Patterns
    }// BulletHeck
}// HeckYeahGames
