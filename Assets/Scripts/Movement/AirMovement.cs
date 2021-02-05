using System;
using UnityEngine;

namespace Assets.Scripts.Movement
{
    class AirMovement : PlayerMovement
    {
        GameObject playerGO;

        public AirMovement(GameObject playerGO, float accelerationSpeed) : base(accelerationSpeed)
        { 
            this.playerGO = playerGO;
        }

        public override float GetGravityValue(bool isGrounded, float ySpeed)
        {
            return isGrounded ? ySpeed + Physics.gravity.y * Time.deltaTime : 0;
        }

        public override Vector3 GetMoveVector(Vector2 moveDirection, float currentSpeed, bool accelerated)
        {
            var moveVector = playerGO.transform.forward * moveDirection.y * currentSpeed +
                playerGO.transform.right * moveDirection.x * currentSpeed / 2;

            return moveVector;
        }
    }
}
