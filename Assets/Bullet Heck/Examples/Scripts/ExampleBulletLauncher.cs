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
using HeckYeahGames.BulletHeck;
using UnityEngine;

/// <summary>
/// This is an example bullet launcher. It's only job is to instantiate and/or
/// initialize bullets, and fire them. The important functionality from this
/// example is the IFireBullets.Fire method. This is called by the BulletPatternResolver
/// and responsible for making sure bullets are fired based on the pattern.
/// 
/// The provided example shows how to create a bullet object at runtime. However, 
/// you could also get a bullet object from your own object pool implementation here
/// as well.
/// </summary>
public class ExampleBulletLauncher : MonoBehaviour, IFireBullets
{
    [SerializeField]
    private GameObject m_bulletTemplate;

    public void Fire(Vector2 worldPosition, Vector2 launchVector, float speed, float damage)
    {
        Debug.Log("ExampleBulletLauncher.Fire called from BulletPatternResolver");
        if(m_bulletTemplate == null)
        {
            Debug.LogWarning(gameObject.name + " is missing it's bullet template object!");
        }

        // Instantiate and initialize the bullet object
        GameObject bullet = Instantiate(m_bulletTemplate);
        bullet.transform.position = (Vector3)worldPosition;

        ExampleBullet exampleBullet = bullet.GetComponent<ExampleBullet>();
        if(exampleBullet != null)
        {
            exampleBullet.SetTrajectory(launchVector);
            exampleBullet.SetSpeed(speed);
            exampleBullet.SetDamage(damage);
        }
    }
}
