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

namespace HeckYeahGames.BulletHeck
{
    /// <summary>
    /// Interface used for the component responsible for launching bullets from
    /// BulletPatternResolver.cs
    /// 
    /// BulletPatternResolver.cs calls the Fire function on this interface when it
    /// is ready to fire a bullet.
    /// </summary>
    public interface IFireBullets
    {
        /// <summary>
        /// Method called by BulletPatternResolver when it is ready
        /// to fire a bullet.
        /// </summary>
        /// <param name="worldPosition">Position on the bullet in world space.</param>
        /// <param name="launchVector">Normalized trajectory of the bullet.</param>
        /// <param name="speed">Initial speed of the bullet upon launch.</param>
        /// <param name="damage">Damage the bullet will do upon collisions.</param>
        void Fire(Vector2 worldPosition, Vector2 launchVector, float speed, float damage);
    }// IFireBullets
}// HeckYeahGames.BulletHeck
