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
using HeckYeahGames.BulletHeck.Patterns.Bullet;

using UnityEngine;

/// <summary>
/// Simple example of an object that uses a BulletPatternResolver to 
/// fire bullets. For example, player and enemy components. :)
/// </summary>
public class ExampleBulletPatternConsumer : MonoBehaviour
{
    private BulletPatternResolver m_bulletPattern;

    // Start is called before the first frame update
    void Start()
    {
        m_bulletPattern = transform.Find("BulletPattern").gameObject.GetComponent<BulletPatternResolver>();

        if(m_bulletPattern == null)
        {
            Debug.Log("ExampleBulletPatternConsumer : bullet pattern not found!!");
        }
    }

    /// <summary>
    /// For the purposes of the sample scene, this is the function called by the "Fire"
    /// button in the UI.
    /// </summary>
    public void FireBulletPattern()
    {
        if(m_bulletPattern.TryToFire())
        {
            Debug.Log("Bullet pattern fired successfully!");
        }
        else
        {
            if(m_bulletPattern.PatternIsRunning())
            {
                Debug.Log("Bullet pattern is currently firing!");
            }
            else
            {
                Debug.Log("Bullet pattern is still in cooldown...");
            }
        }
    }
}
