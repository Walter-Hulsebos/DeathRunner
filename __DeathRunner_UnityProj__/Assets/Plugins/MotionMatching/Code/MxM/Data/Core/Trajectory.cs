using UnityEngine;
using System;

namespace MxM
{
    [System.Serializable]
    public struct Trajectory
    {
        private TrajectoryPoint PointA;
        private TrajectoryPoint PointB;
        private TrajectoryPoint PointC;
        private TrajectoryPoint PointD;

    }//End of struct: Trajectory

    public struct TrajectoryFlat
    {
        private float PointAPosX;
        private float PointAPosY;
        private float PointAPosZ;
        private float PointAFacing;

        private float PointBPosX;
        private float PointBPosY;
        private float PointBPosZ;
        private float PointBFacing;

        private float PointCPosX;
        private float PointCPosY;
        private float PointCPosZ;
        private float PointCFacing;

        private float PointDPosX;
        private float PointDPosY;
        private float PointDPosZ;
        private float PointDFacing;

        private float PointEPosX;
        private float PointEPosY;
        private float PointEPosZ;
        private float PointEFacing;

    }//End of struct: TrajectoryFlat

    public struct TrajectorySemi
    {
        private Vector3 PointAPos;
        private float PointAFacing;

        private Vector3 PointBPos;
        private float PointBFacing;

        private Vector3 PointCPos;
        private float PointCFacing;

        private Vector3 PointDPos;
        private float PointDFacing;

        private Vector3 PointEPos;
        private float PointEFacing;
    }//End of struct: TrajectorySemi


}//End of namespace: MxM

