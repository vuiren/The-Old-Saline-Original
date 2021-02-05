using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Movement
{
    class WaterMovement : PlayerMovement
    {
        Water waterType;

        public WaterMovement(Water waterType):base(0)
        {
            this.waterType = waterType;
        }

        public void SetWaterType(Water water) => waterType = water;

        public override float GetGravityValue(bool isGrounded, float ySpeed)
        {
           return ySpeed < 0 ? ySpeed + waterType.GetViscosity * Time.deltaTime : 0;
        }

        public override Vector3 GetMoveVector(Vector2 moveDirection, float currentSpeed, bool accelerated)
        {
            throw new NotImplementedException();
        }
    }
}
