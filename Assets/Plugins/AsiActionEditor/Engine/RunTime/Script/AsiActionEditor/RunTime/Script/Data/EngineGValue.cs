using System;
using UnityEngine;

namespace AsiActionEngine.RunTime
{
    [System.Serializable]
    public class EngineGValue
    {
        public bool[] mEngineBool;
        public int[] mEngineInt;
        public float[] mEngineFloat;
        public string[] mEngineString;
        public byte[] mEngineEnum;
        public byte[] mEnginePointData;
        public byte[] mEngineUnit;
        public byte[] mEngineTransform;
        public Color[] mEngineColor;
        public Vector2[] mEngineVector2;
        public Vector3[] mEngineVector3;
        // public Quaternion[] mEngineQuaternion;

        public GValueEquation.GValueEquation_FloatPart[] mGValueEquation;
        public EngineGValue Clone()
        {
            EngineGValue _gValue = new EngineGValue();
            _gValue.mEngineBool = (bool[])mEngineBool.Clone();
            _gValue.mEngineInt = (int[])mEngineInt.Clone();
            _gValue.mEngineFloat = (float[])mEngineFloat.Clone();
            _gValue.mEngineString = (string[])mEngineString.Clone();
            _gValue.mEngineEnum = (byte[])mEngineEnum.Clone();
            _gValue.mEnginePointData = (byte[])mEnginePointData.Clone();
            _gValue.mEngineUnit = (byte[])mEngineUnit.Clone();
            _gValue.mEngineTransform = (byte[])mEngineTransform.Clone();
            _gValue.mGValueEquation = mGValueEquation;
            // _gValue.mGValueEquation = (GValueEquation.GValueEquation_Value[])mGValueEquation.Clone();

            // _gValue.mEngineBool = new bool[mEngineBool.Length];
            // for (int i = 0; i < _gValue.mEngineBool.Length; i++) _gValue.mEngineBool[i] = mEngineBool[i];
            // _gValue.mEngineInt = new int [mEngineInt.Length];
            // for (int i = 0; i < mEngineInt.Length; i++) _gValue.mEngineInt[i] = mEngineInt[i];
            // _gValue.mEngineFloat = new float[mEngineFloat.Length];
            // for (int i = 0; i < mEngineFloat.Length; i++) _gValue.mEngineFloat[i] = mEngineFloat[i];
            // _gValue.mEngineString = new string[mEngineString.Length];
            // for (int i = 0; i < mEngineString.Length; i++) _gValue.mEngineString[i] = mEngineString[i];
            // _gValue.mEngineEnum = new byte[mEngineEnum.Length];
            // for (int i = 0; i < mEngineEnum.Length; i++) _gValue.mEngineEnum[i] = mEngineEnum[i];
            // _gValue.mEnginePointData = new byte[mEnginePointData.Length];
            // for (int i = 0; i < mEnginePointData.Length; i++) _gValue.mEnginePointData[i] = mEnginePointData[i];
            // _gValue.mEngineUnit = new byte[mEngineUnit.Length];
            // for (int i = 0; i < mEngineUnit.Length; i++) _gValue.mEngineUnit[i] = mEngineUnit[i];
            // _gValue.mEngineTransform = new byte[mEngineTransform.Length];
            // for (int i = 0; i < mEngineTransform.Length; i++) _gValue.mEngineTransform[i] = mEngineTransform[i];
            // _gValue.mGValueEquation = new GValueEquation.GValueEquation[mGValueEquation.Length];
            // for (int i = 0; i < mGValueEquation.Length; i++) _gValue.mGValueEquation[i] = mGValueEquation[i];
            
            // _gValue.mEngineInt = new int [mEngineInt.Length];
            // for (int i = 0; i < mEngineInt.Length; i++) _gValue.mEngineInt[i] = mEngineInt[i];
            // _gValue.mEngineFloat = new float[mEngineFloat.Length];
            // for (int i = 0; i < mEngineFloat.Length; i++) _gValue.mEngineFloat[i] = mEngineFloat[i];
            // _gValue.mEngineString = new string[mEngineString.Length];
            // for (int i = 0; i < mEngineString.Length; i++) _gValue.mEngineString[i] = mEngineString[i];
            // _gValue.mEngineColor = new Color[mEngineColor.Length];
            // for (int i = 0; i < mEngineColor.Length; i++) _gValue.mEngineColor[i] = mEngineColor[i];
            // _gValue.mEngineVector2 = new Vector2[mEngineVector2.Length];
            // for (int i = 0; i < mEngineVector2.Length; i++) _gValue.mEngineVector2[i] = mEngineVector2[i];
            // _gValue.mEngineVector3 = new Vector3[mEngineVector3.Length];
            // for (int i = 0; i < mEngineVector3.Length; i++) _gValue.mEngineVector3[i] = mEngineVector3[i];
            // _gValue.mEngineQuaternion = new Quaternion[mEngineQuaternion.Length];
            // for (int i = 0; i < mEngineQuaternion.Length; i++) _gValue.mEngineQuaternion[i] = mEngineQuaternion[i];
            // _gValue.mEngineUnit = new Unit[mEngineUnit.Length];
            // for (int i = 0; i < mEngineUnit.Length; i++) _gValue.mEngineUnit[i] = mEngineUnit[i];
            // _gValue.mEngineTransform = new Transform[mEngineTransform.Length];
            // for (int i = 0; i < mEngineTransform.Length; i++) _gValue.mEngineTransform[i] = mEngineTransform[i];
            return _gValue;
        }
    }
}