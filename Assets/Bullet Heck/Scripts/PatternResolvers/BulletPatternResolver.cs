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
using System.Collections;
using HeckYeahGames.BulletHeck.Patterns.Shape;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

namespace HeckYeahGames
{
    namespace BulletHeck
    {
        namespace Patterns
        {
            namespace Bullet
            {
                public delegate void FiringShot();
                public delegate void IteratingBegin();
                public delegate void IteratingEnd();
                public delegate void BurstBegin();
                public delegate void BurstEnd();
                public delegate void LoopFinished();
                [RequireComponent(typeof(ShapePatternResolver), typeof(IFireBullets)), ExecuteInEditMode]
                public class BulletPatternResolver : MonoBehaviour, IPatternListener
                {
                    [SerializeField]
                    internal BulletPattern m_bulletPattern;

                    private IPatternIterator m_firingSequenceIterator;

                    private bool m_patternIsRunning = false;
                    private Coroutine m_shootingCoroutine = null;
                    private IFireBullets m_bulletLauncher;

                    public event FiringShot m_onFiringShot;

                    // Start is called before the first frame update
                    void Start()
                    {
                        m_bulletLauncher = GetComponent<IFireBullets>();
                        if (m_bulletLauncher == null)
                        {
                            Debug.Log("BulletPatternResolver cannot find the IFireBullet component attached to " + gameObject.name);
                        }
                    }

                    void OnEnable()
                    {
                        Initialize();
                    }

                    void OnDisable()
                    {
                        Shutdown();
                    }

                    private void Initialize()
                    {
                        if (m_bulletPattern == null)
                        {
                            return;
                        }

                        m_bulletPattern.UnregisterOnChangeEventHandler(OnPatternChange);
                        m_bulletPattern.RegisterOnChangeEventHandler(OnPatternChange);


                        GetComponent<ShapePatternResolver>().m_onPatternRebuild -= OnShapePatternRebuild;
                        GetComponent<ShapePatternResolver>().m_onPatternRebuild += OnShapePatternRebuild;
                        GetComponent<ShapePatternResolver>().OnPatternChange();
                    }

                    private void Shutdown()
                    {
                        if(m_bulletPattern == null)
                        {
                            return;
                        }

                        // We need to unregister from the bullet pattern, as it is possible
                        // for it to outlive this game object
                        m_bulletPattern.UnregisterOnChangeEventHandler(OnPatternChange);

                        // We don't have to unregister from ShapePatternResolver because it
                        // will be destroyed at the same time as this component.
                    }

                    public bool CanFire()
                    {
                        if (m_bulletPattern != null && m_firingSequenceIterator != null)
                        {
                            if (m_patternIsRunning == false)
                            {
                                return true;
                            }
                        }
                        return false;
                    }

                    public bool TryToFire()
                    {
                        if (m_bulletPattern != null && m_firingSequenceIterator != null)
                        {
                            if (m_patternIsRunning == false)
                            {
                                m_shootingCoroutine = StartCoroutine(FireBulletPattern());
                                return true;
                            }
                        }
                        return false;
                    }

                    public void StopFiring()
                    {
                        m_patternIsRunning = false;
                        if (m_shootingCoroutine != null)
                        {
                            StopCoroutine(m_shootingCoroutine);
                        }
                    }

                    public bool PatternIsLooping()
                    {
                        return m_patternIsRunning && m_bulletPattern.isLooping;
                    }

                    public bool PatternIsRunning()
                    {
                        return m_patternIsRunning;
                    }

                    IEnumerator FireBulletPattern()
                    {
                        m_patternIsRunning = true;

                        do // looping
                        {
                            int numberOfBurstsPerformed = 0;
                            bool stillBursting = false;

                            do // burst
                            {
                                // perform shot
                                ShapePoint[] initialBullets = m_firingSequenceIterator.First();
                                FireBullets(initialBullets);
                                
                                while (m_firingSequenceIterator.Done() == false)
                                {

                                    if (m_bulletPattern.firingSequence != BulletPattern.FiringSequence.IMMEDIATE)
                                    {
                                        yield return new WaitForSeconds(m_bulletPattern.bulletDelay);
                                    }

                                    ShapePoint[] nextBullets = m_firingSequenceIterator.Next();
                                    if (FireBullets(nextBullets) == false)
                                    {
                                        // we have reached the end of the loop
                                        break;
                                    }
                                }
                                numberOfBurstsPerformed++;
                                stillBursting = (int)(m_bulletPattern.burstPerShot) > numberOfBurstsPerformed;

                                // only apply burst delay between bursts and not after
                                // they have been completed
                                if (stillBursting)
                                {
                                    yield return new WaitForSeconds(m_bulletPattern.burstDelay);
                                }

                            } while (stillBursting);

                            // delay if looping
                            if (m_bulletPattern.isLooping)
                            {
                                yield return new WaitForSeconds(m_bulletPattern.loopDelay);
                            }
                        } while (m_bulletPattern.isLooping);

                        // delay before another shot can be initiated
                        yield return new WaitForSeconds(m_bulletPattern.singleFireDelay);

                        m_patternIsRunning = false;
                        yield return 0;
                    }

                    private bool FireBullets(ShapePoint[] shapePoints)
                    {
                        if (shapePoints != null)
                        {
                            // Fire bullets. Ignore shape points that are just zero
                            for (int i = 0; i < shapePoints.Length; ++i)
                            {
                                if (shapePoints[i].normalWorld != Vector2.zero)
                                {
                                    m_onFiringShot?.Invoke();
                                    m_bulletLauncher.Fire(shapePoints[i].world,
                                                          shapePoints[i].normalWorld,
                                                          m_bulletPattern.bulletSpeed,
                                                          m_bulletPattern.bulletDamage);
                                }
                            }
                            return true;
                        }

                        return false;
                    }

                    void OnDestroy()
                    {
                        // TODO : unregister from pattern scriptable object to avoid weird
                        // nullref issues in event handlers
                    }

                    public void OnPatternChange()
                    {
                        GetFiringSequenceIterator();
                    }

                    public void OnShapePatternRebuild(ShapePoint[] shapePoints) // HACK : Don't need these shape points
                    {
                        GetFiringSequenceIterator();
                    }

                    private void GetFiringSequenceIterator()
                    {
                        m_firingSequenceIterator = null;

                        switch (m_bulletPattern.firingSequence)
                        {
                            case BulletPattern.FiringSequence.IMMEDIATE:
                                {
                                    m_firingSequenceIterator = GetComponent<ShapePatternResolver>().GetPatternIterator<LinearPatternIterator>(false);
                                    break;
                                }
                            case BulletPattern.FiringSequence.IMMEDIATE_ALT_EVEN_AND_ODD:
                                {
                                    m_firingSequenceIterator = GetComponent<ShapePatternResolver>().GetPatternIterator<AlternatingPatternIterator>(false);
                                    break;
                                }
                            case BulletPattern.FiringSequence.BACK_TO_FRONT:
                                {
                                    m_firingSequenceIterator = GetComponent<ShapePatternResolver>().GetPatternIterator<LinearPatternIterator>(true);
                                    break;
                                }
                            case BulletPattern.FiringSequence.FRONT_TO_BACK:
                                {
                                    m_firingSequenceIterator = GetComponent<ShapePatternResolver>().GetPatternIterator<LinearPatternIterator>(false);
                                    break;
                                }
                            case BulletPattern.FiringSequence.PING_PONG:
                                {
                                    // TODO
                                    break;
                                }
                            case BulletPattern.FiringSequence.CENTER_TO_OUTSIDE:
                                {
                                    m_firingSequenceIterator = GetComponent<ShapePatternResolver>().GetPatternIterator<BilinearPatternIterator>(false);
                                    break;
                                }
                            case BulletPattern.FiringSequence.OUTSIDE_TO_CENTER:
                                {
                                    m_firingSequenceIterator = GetComponent<ShapePatternResolver>().GetPatternIterator<BilinearPatternIterator>(true);
                                    break;
                                }
                        }
                    }

                    internal void OnBeforeShapePatternReplace()
                    {
                        if (m_bulletPattern != null)
                        {
                            Debug.Log("OnBeforeBulletPatternReplace");
                            m_bulletPattern.UnregisterOnChangeEventHandler(OnPatternChange);
                        }
                    }

                    internal void OnAfterShapePatternReplace()
                    {
                        if (m_bulletPattern != null)
                        {
                            Debug.Log("OnAfterBulletPatternReplace");
                            m_bulletPattern.RegisterOnChangeEventHandler(OnPatternChange);
                        }
                    }

                }// BulletPatternResolver
#if UNITY_EDITOR
                [CustomEditor(typeof(BulletPatternResolver))]
                class BulletPatternResolverEditor : Editor
                {
                    public override void OnInspectorGUI()
                    {
                        BulletPatternResolver bulletPatternResolver = (BulletPatternResolver)target;
                        if (bulletPatternResolver == null)
                            return;

                        Undo.RecordObject(bulletPatternResolver, "Undo changes");

                        BulletPattern bulletPattern = (BulletPattern)EditorGUILayout.ObjectField("Primary Pattern : ",
                                                                                                bulletPatternResolver.m_bulletPattern,
                                                                                                typeof(BulletPattern),
                                                                                                true);

                        if (bulletPattern != bulletPatternResolver.m_bulletPattern)
                        {
                            bulletPatternResolver.OnBeforeShapePatternReplace();
                            bulletPatternResolver.m_bulletPattern = bulletPattern;
                            bulletPatternResolver.OnAfterShapePatternReplace();
                        }

                        if (bulletPatternResolver.m_bulletPattern != null)
                        {
                            BulletPatternEditor bulletPatternEditor = (BulletPatternEditor)BulletPatternEditor.CreateEditor(bulletPatternResolver.m_bulletPattern);
                            bulletPatternEditor.OnInspectorGUI();
                        }
                    }
                }
#endif
            }// Bullet
        }// Patterns
    }// BulletHeck
}// HeckYeahGames
