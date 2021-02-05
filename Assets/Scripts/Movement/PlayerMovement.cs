using UnityEngine;

namespace Assets.Scripts.Movement
{
    public abstract class PlayerMovement
    {
        private readonly float accelerationSpeed;

        protected PlayerMovement(float accelerationSpeed)
        {
            this.accelerationSpeed = accelerationSpeed;
        }

        protected float AccelerationSpeed { get => accelerationSpeed; }

        public abstract Vector3 GetMoveVector(Vector2 moveDirection, float currentSpeed, bool accelerated);
        public abstract float GetGravityValue(bool isGrounded, float ySpeed);
    }
}