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
            namespace Bullet
            {
                /// <summary>
                /// Data container that provides the constraints required
                /// to build a bullet pattern.
                /// 
                /// See BulletPatternResolver.cs
                /// </summary>
                [CreateAssetMenu(fileName = "Bullet Pattern Object", menuName = "Heck Yeah Games/Patterns/Bullet")]
                public class BulletPattern : ScriptableObject, IPattern
                {
                    public enum Trajectory
                    {
                        SHAPE_PATTERN_NORMALS,
                        OSCILLATE_SHAPE_PATTERN_NORMALS,
                        TARGET_PLAYER,
                        HORIZONTAL,
                        CUSTOM
                    }

                    public enum FiringSequence
                    {
                        IMMEDIATE,
                        IMMEDIATE_ALT_EVEN_AND_ODD,
                        BACK_TO_FRONT,
                        FRONT_TO_BACK,
                        PING_PONG,
                        CENTER_TO_OUTSIDE,
                        OUTSIDE_TO_CENTER,
                        RANDOM_BULLET_SPAWN,
                    }

                    [SerializeField]
                    internal Trajectory trajectory = Trajectory.SHAPE_PATTERN_NORMALS;

                    [SerializeField]
                    internal FiringSequence firingSequence = FiringSequence.IMMEDIATE;
                    // Delay between shots when iterating the firing sequence
                    [SerializeField]
                    internal float bulletDelay = 0.05f;

                    // looping
                    [SerializeField]
                    internal bool isLooping = false;
                    [SerializeField]
                    internal float singleFireDelay = 0.2f;
                    [SerializeField]
                    internal float loopDelay = 0.1f;

                    // burst
                    [SerializeField]
                    internal uint burstPerShot = 0;
                    [SerializeField]
                    internal float burstDelay = 0.1f;


                    // launch angle modifiers
                    [SerializeField]
                    internal bool trajectoryVectorOscillating = false;
                    [SerializeField]
                    internal float oscillationAngle = 30.0f;
                    [SerializeField]
                    internal float oscillateDegreesPerSecond = 5.0f;

                    [SerializeField]
                    internal float bulletSpeed = 20.0f;

                    [SerializeField]
                    internal int bulletDamage = 1;

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
                }// BulletPattern

                #if UNITY_EDITOR
                [CustomEditor(typeof(BulletPattern))]
                class BulletPatternEditor : Editor
                {
                    SerializedProperty m_trajectory;
                    SerializedProperty m_firingSequence;
                    SerializedProperty m_bulletDelay;
                    SerializedProperty m_isLooping;
                    SerializedProperty m_singleFireDelay;
                    SerializedProperty m_loopDelay;
                    SerializedProperty m_burstPerShot;
                    SerializedProperty m_burstDelay;
                    SerializedProperty m_trajectoryVectorOscillating;
                    SerializedProperty m_oscillationAngle;
                    SerializedProperty m_oscillatingDegreesPerSecond;
                    SerializedProperty m_bulletSpeed;
                    SerializedProperty m_bulletDamage;

                    void OnEnable()
                    {
                        m_trajectory = serializedObject.FindProperty(nameof(BulletPattern.trajectory));
                        m_firingSequence = serializedObject.FindProperty(nameof(BulletPattern.firingSequence));
                        m_bulletDelay = serializedObject.FindProperty(nameof(BulletPattern.bulletDelay));
                        m_isLooping = serializedObject.FindProperty(nameof(BulletPattern.isLooping));
                        m_singleFireDelay = serializedObject.FindProperty(nameof(BulletPattern.singleFireDelay));
                        m_loopDelay = serializedObject.FindProperty(nameof(BulletPattern.loopDelay));
                        m_burstPerShot = serializedObject.FindProperty(nameof(BulletPattern.burstPerShot));
                        m_burstDelay = serializedObject.FindProperty(nameof(BulletPattern.burstDelay));
                        m_trajectoryVectorOscillating = serializedObject.FindProperty(nameof(BulletPattern.trajectoryVectorOscillating));
                        m_oscillationAngle = serializedObject.FindProperty(nameof(BulletPattern.oscillationAngle));
                        m_oscillatingDegreesPerSecond = serializedObject.FindProperty(nameof(BulletPattern.oscillateDegreesPerSecond));
                        m_bulletSpeed = serializedObject.FindProperty(nameof(BulletPattern.bulletSpeed));
                        m_bulletDamage = serializedObject.FindProperty(nameof(BulletPattern.bulletDamage));
                    }

                    public override void OnInspectorGUI()
                    {
                        var bulletPattern = (BulletPattern)target;
                        if(bulletPattern == null)
                            return;

                        //Undo.RecordObject(bulletPattern, "Undo changes");

                        bool fireChangeEvent = false;

                        serializedObject.Update();

                        GUIContent guiContent = new GUIContent(EditorStrings.TRAJECTORY, EditorStrings.TRAJECTORY_TOOLTIP);
                        m_trajectory.enumValueFlag = (int)((BulletPattern.Trajectory)EditorGUILayout.EnumPopup(guiContent, (BulletPattern.Trajectory)m_trajectory.enumValueFlag));

                        if(m_trajectory.enumValueFlag == (int)BulletPattern.Trajectory.OSCILLATE_SHAPE_PATTERN_NORMALS)
                        {
                            guiContent.text = EditorStrings.OSCILLATION_ANGLE;
                            guiContent.tooltip = EditorStrings.OSCILLATION_ANGLE_TOOLTIP;
                            m_oscillationAngle.floatValue = EditorGUILayout.Slider(guiContent, m_oscillationAngle.floatValue, 0.1f, 180.0f);

                            guiContent.text = EditorStrings.OSCILLATION_SPEED;
                            guiContent.tooltip = EditorStrings.OSCILLATION_SPEED_TOOLTIP;
                            m_oscillatingDegreesPerSecond.floatValue = EditorGUILayout.Slider(guiContent, m_oscillatingDegreesPerSecond.floatValue, 0.1f, 180.0f);

                            EditorGUI.indentLevel--;
                        }

                        guiContent.text = EditorStrings.FIRING_SEQUENCE;
                        guiContent.tooltip = EditorStrings.FIRING_SEQUENCE_TOOLTIP;

                        EditorGUI.BeginChangeCheck();
                        m_firingSequence.enumValueFlag = (int)((BulletPattern.FiringSequence)EditorGUILayout.EnumPopup(guiContent, (BulletPattern.FiringSequence)m_firingSequence.enumValueFlag));
                        if(EditorGUI.EndChangeCheck())
                        {
                            Debug.Log("Firing sequence changed  to " + ((BulletPattern.FiringSequence)m_firingSequence.enumValueFlag));
                            fireChangeEvent = true;
                        }

                        if(m_firingSequence.enumValueFlag != (int)BulletPattern.FiringSequence.IMMEDIATE)
                        {
                            EditorGUI.indentLevel++;
                            guiContent.text = EditorStrings.BULLET_DELAY;
                            guiContent.tooltip = EditorStrings.BULLET_DELAY_TOOLTIP;
                            EditorGUI.BeginChangeCheck();
                            //bulletPattern.bulletDelay = EditorGUILayout.Slider(guiContent, bulletPattern.bulletDelay, 0.01f, 5.0f);
                            m_bulletDelay.floatValue = EditorGUILayout.Slider(guiContent, m_bulletDelay.floatValue, 0.01f, 5.0f);
                            if(EditorGUI.EndChangeCheck())
                            {
                                fireChangeEvent = true;
                            }
                            EditorGUI.indentLevel--;
                        }

                        m_isLooping.boolValue = EditorGUILayout.Toggle("Is Looping", m_isLooping.boolValue);
                        if(bulletPattern.isLooping)
                        {
                            EditorGUI.indentLevel++;
                            guiContent.text = EditorStrings.LOOP_DELAY;
                            guiContent.tooltip = EditorStrings.LOOP_DELAY_TOOLTIP;
                            EditorGUI.BeginChangeCheck();
                            m_loopDelay.floatValue = EditorGUILayout.Slider(guiContent, m_loopDelay.floatValue, 0.01f, 5.0f);
                            if(EditorGUI.EndChangeCheck())
                            {
                                fireChangeEvent = true;
                            }
                            EditorGUI.indentLevel--;
                        }
                        else
                        {
                            EditorGUI.indentLevel++;
                            guiContent.text = EditorStrings.SINGLE_FIRE_DELAY;
                            guiContent.tooltip = EditorStrings.SINGLE_FIRE_DELAY_TOOLTIP;
                            m_singleFireDelay.floatValue = EditorGUILayout.Slider(guiContent, m_singleFireDelay.floatValue, 0.01f, 5.0f);
                            EditorGUI.indentLevel--;
                        }

                        guiContent.text = EditorStrings.BURST_COUNT;
                        guiContent.tooltip = EditorStrings.BURST_COUNT_TOOLTIP;
                        m_burstPerShot.uintValue = (uint)EditorGUILayout.IntSlider(guiContent, (int)m_burstPerShot.uintValue, 0, 10);

                        if(m_burstPerShot.uintValue > 0)
                        {
                            EditorGUI.indentLevel++;
                            guiContent.text = EditorStrings.BURST_DELAY;
                            guiContent.tooltip = EditorStrings.BURST_DELAY_TOOLTIP;
                            m_burstDelay.floatValue = EditorGUILayout.Slider(guiContent, m_burstDelay.floatValue, 0.01f, 5.0f);
                            EditorGUI.indentLevel--;
                        }

                        guiContent.text = EditorStrings.BULLET_SPEED;
                        guiContent.tooltip = EditorStrings.BULLET_SPEED_TOOLTIP;
                        m_bulletSpeed.floatValue = EditorGUILayout.Slider(guiContent, m_bulletSpeed.floatValue, 1.0f, 150.0f);

                        guiContent.text = EditorStrings.BULLET_DAMAGE;
                        guiContent.tooltip = EditorStrings.BULLET_DAMAGE_TOOLTIP;
                        m_bulletDamage.intValue = EditorGUILayout.IntSlider(guiContent, m_bulletDamage.intValue, 1, 40);

                        serializedObject.ApplyModifiedProperties();

                        if(fireChangeEvent)
                        {
                            bulletPattern.FireOnChangeEvent();
                        }
                    }
                }// BulletPatternEditor
                #endif
            }// Bullet
        }// Patterns
    }// BulletHeck
}// HeckYeahGames
