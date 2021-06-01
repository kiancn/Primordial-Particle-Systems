using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* Unused, for deletion, possibly */
[Serializable]
public struct SPPCellStruct 
{
        public SPPCellStruct(float velocity, float neighborhoodRadius, float fixedRotation, float neighborhoodProportialRotationConstant)
        {
                this.velocity = velocity;
                this.neighborhoodRadius = neighborhoodRadius;
                this.fixedRotation = fixedRotation;
                this.neighborhoodProportialRotationConstant = neighborhoodProportialRotationConstant;
        }

        public float Velocity
        {
                get => velocity;
                set => velocity = value;
        }

        public float NeighborhoodRadius
        {
                get => neighborhoodRadius;
                set => neighborhoodRadius = value;
        }

        public float FixedRotation
        {
                get => fixedRotation;
                set => fixedRotation = value;
        }

        public float NeighborhoodProportialRotationConstant
        {
                get => neighborhoodProportialRotationConstant;
                set => neighborhoodProportialRotationConstant = value;
        }

        private float velocity;
        private float neighborhoodRadius;
        private float fixedRotation;
        private float neighborhoodProportialRotationConstant;
}
