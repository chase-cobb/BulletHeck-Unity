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
using UnityEngine;

/// <summary>
/// Simple bullet used for the purposes of the example.
/// </summary>
public class ExampleBullet : MonoBehaviour
{
    [SerializeField]
    private float m_movementSpeed = 5.0f;

    [SerializeField]
    private float m_damageAmount = 3.0f;

    private Vector2 m_trajectory = Vector2.zero;
    private float m_speedModifier = 1.0f;
    private float m_damageModifier = 1.0f;

    void Start()
    {
        StartCoroutine(TimedSelfDestruct());
    }

    // Update is called once per frame
    void Update()
    {
        float movementSpeed = m_movementSpeed * m_speedModifier;

        transform.position += movementSpeed * (Vector3)m_trajectory * Time.deltaTime;
    }

    public float GetDamage()
    {
        return m_damageAmount * m_damageModifier;
    }

    public void SetSpeed(float speed)
    {
        m_movementSpeed = speed;
    }

    public void SetSpeedModifier(float modifier)
    {
        m_speedModifier = modifier;
    }

    public void SetDamage(float damage)
    {
        m_damageAmount = damage;
    }

    public void SetDamageModifier(float modifier)
    {
        m_damageModifier = modifier;
    }

    public void SetTrajectory(Vector2 trajectory)
    {
        m_trajectory = trajectory;
    }

    IEnumerator TimedSelfDestruct()
    {
        yield return new WaitForSeconds(4.0f);

        Destroy(gameObject);
    }
}
