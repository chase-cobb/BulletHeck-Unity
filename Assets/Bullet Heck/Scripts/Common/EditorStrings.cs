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
        /// <summary>
        /// Simple class to provide strings for use by the editor. This could
        /// be extended to support multiple languages.
        /// </summary>
        public class EditorStrings
        {
            // BULLET PATTERN

            public const string BULLET_TYPE = "Bullet Type";
            public const string BULLET_TYPE_TOOLTIP = "The type of bullet to fire from this bullet pattern";

            // trajectory
            public const string TRAJECTORY = "Trajectory";
            public const string TRAJECTORY_TOOLTIP = "Initial trajectory when bullet spawns";

            public const string FIRING_SEQUENCE = "Firing Sequence";
            public const string FIRING_SEQUENCE_TOOLTIP = "The order that the bullets are spawned from the shape pattern";

            public const string BULLET_DELAY = "Bullet Delay";
            public const string BULLET_DELAY_TOOLTIP = "The amount of time between individual bullets spawning in the firing sequence, in seconds";

            public const string BULLET_PATTERN_IS_LOOPING = "Is Looping";
            public const string BULLET_PATTERN_IS_LOOPING_TOOLTIP = "Enabling will set the bullet pattern to looping";

            public const string LOOP_DELAY = "Loop Delay";
            public const string LOOP_DELAY_TOOLTIP = "Delay between loops, in seconds";

            public const string SINGLE_FIRE_DELAY = "Single Fire Delay";
            public const string SINGLE_FIRE_DELAY_TOOLTIP = "The delay between each firing sequence that can be triggered by user input";

            public const string BURST_COUNT = "Burst Count";
            public const string BURST_COUNT_TOOLTIP = "The number of times the firing sequence will happen per shot";

            public const string BURST_DELAY = "Burst Delay";
            public const string BURST_DELAY_TOOLTIP = "Delay between bursts, in seconds";

            public const string OSCILLATION_ANGLE = "Angle Range";
            public const string OSCILLATION_ANGLE_TOOLTIP = "The angle range that the trajectory vector should oscillate, in degrees";

            public const string OSCILLATION_SPEED = "Degrees per Second";
            public const string OSCILLATION_SPEED_TOOLTIP = "Rotation in degrees per second";

            public const string BULLET_SPEED = "Bullet speed";
            public const string BULLET_SPEED_TOOLTIP = "Speed of the bullet in units per second";

            public const string BULLET_DAMAGE = "Bullet damage";
            public const string BULLET_DAMAGE_TOOLTIP = "The amount of base health lost by character when a bullet hits them";

            // BULLET PATTERN END
        }// EditorStrings
    }// BulletHeck
}// HeckYeahGames
