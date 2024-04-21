using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace CustomMethods
{
    [Serializable]
    public class Pair<T1,T2>
    {
        public T1 Alpha;
        public T2 Beta;
    }

    public static class ExtendedMathUtility
    {
        #region Linear Methods

        /// <summary>
        /// Returns closest smallest multiple of given value
        /// </summary>
        /// 
        public static float FloorToMultipleOf(float value, float multiple)
        {
            return Mathf.Round(value / multiple) * multiple; // value - (value % multiple)
        }

        #endregion

        #region Vector Methods

        /// <summary>
        /// Returns the Distance between 2 vectors, ignoring their Y component
        /// </summary>
        public static float VectorXZDistance(Vector3 v1, Vector3 v2)
        {
            return Mathf.Sqrt((v1.x - v2.x) * (v1.x - v2.x) + (v1.z - v2.z) * (v1.z - v2.z));
        }

        /// <summary>
        /// Returns the Squared Distance between 2 vectors, ignoring their Y component
        /// </summary>
        public static float VectorXZDistanceSquared(Vector3 v1, Vector3 v2)
        {
            return (v1.x - v2.x) * (v1.x - v2.x) + (v1.z - v2.z) * (v1.z - v2.z);
        }

        /// <summary>
        /// Returns the Angle between 2 vectors, ignoring their Y component
        /// </summary>
        public static float VectorXZAngle(Vector3 v1, Vector3 v2)
        {
            v1.y = v2.y;
            return Vector3.Angle(v1, v2);
        }

        /// <summary>
        /// Returns the magnitude of a vector only considering its XZ components
        /// </summary>
        public static float VectorXZMagnitude(Vector3 v)
        {
            return Mathf.Sqrt(v.x * v.x + v.z * v.z);
        }

        /// <summary>
        /// Returns the squared magnitude of a vector only considering its XZ components
        /// </summary>
        public static float VectorXZSquaredMagnitude(Vector3 v)
        {
            return v.x * v.x + v.z * v.z;
        }

        /// <summary>
        /// Returns a direction from origin to extreme, ignoring their Y difference
        /// </summary>
        public static Vector3 HorizontalDirection(Vector3 origin, Vector3 extreme)
        {
            return Vector3.ClampMagnitude(new Vector3(extreme.x - origin.x, 0f, extreme.z - origin.z), 1f);
        }

        /// <summary>
        /// Returns the projection of a vector onto a plane defined by its normal and a point it passes through
        /// </summary>
        public static Vector3 ProjectOnPlane(Vector3 vec, Vector3 planeNormal, Vector3 planePoint)
        {
            return Vector3.ProjectOnPlane(vec, planeNormal) + Vector3.Dot(planePoint, planeNormal) * planeNormal;
        }

        /// <summary>
        /// Returns the local direction in world direction, knowing the rotation of the parent
        /// </summary>
        public static Vector3 LocalToWordDirection(Vector3 direction, Transform parent)
        {
            Vector3 worldDirection = parent.right * direction.x 
                                   + parent.up * direction.y 
                                   + parent.forward * direction.z;
            return worldDirection;
        }
        public static Vector3 LocalToWordDirection(Vector2 direction, Transform parent)
        {
            Vector3 worldDirection = parent.right * direction.x
                                   + parent.forward * direction.y;
            return worldDirection;
        }

        /// <summary>
        /// Snaps a vector direction to the nearest angle increment
        /// </summary>
        /// <param name="v3"> The Direction to rotate </param>
        /// <param name="snapAngle"> The agle increment </param>
        /// <param name="customUpAxis"> The up axis to rotate around </param>
        /// <returns></returns>
        public static Vector3 SnapTo(Vector3 v3, float snapAngle, Vector3 customUpAxis)
        {
            float angle = Vector3.Angle(v3, customUpAxis);
            if (angle < snapAngle / 2.0f)          // Cannot do cross product
                return customUpAxis * v3.magnitude;  //   with angles 0 & 180
            if (angle > 180.0f - snapAngle / 2.0f)
                return -customUpAxis * v3.magnitude;

            float t = Mathf.Round(angle / snapAngle);
            float deltaAngle = (t * snapAngle) - angle;

            Vector3 axis = Vector3.Cross(customUpAxis, v3);
            Quaternion q = Quaternion.AngleAxis(deltaAngle, axis);
            return q * v3;
        }

        public static Vector3 SnapTo(Vector3 v3, float snapAngle)
        {
            float angle = Vector3.Angle(v3, Vector3.up);
            if (angle < snapAngle / 2.0f)          // Cannot do cross product 
                return Vector3.up * v3.magnitude;  //   with angles 0 & 180
            if (angle > 180.0f - snapAngle / 2.0f)
                return Vector3.down * v3.magnitude;

            float t = Mathf.Round(angle / snapAngle);

            float deltaAngle = (t * snapAngle) - angle;

            Vector3 axis = Vector3.Cross(Vector3.up, v3);
            Quaternion q = Quaternion.AngleAxis(deltaAngle, axis);
            return q * v3;
        }

        public static T1 GetRandomWeightedValue<T1>(Dictionary<T1, int> values)
        {
            int totalWeight = 0;

            //get a random weight Value
            foreach (T1 value in values.Keys) totalWeight += values.GetValueOrDefault(value);
            int rndWeightValue = UnityEngine.Random.Range(1, totalWeight + 1);

            //check where the random weight value falls
            int processedWeight = 0;
            foreach (T1 value in values.Keys)
            {
                processedWeight += values.GetValueOrDefault(value);
                if (rndWeightValue <= processedWeight)
                {
                    return value;
                }
            }

            //fallback to uniform distribution if weighted one fails
            return values.ElementAtOrDefault(UnityEngine.Random.Range(0, values.Count)).Key;
        }

        #endregion
    }
    public static class ExtendedDataUtility
    {
        /// <summary>
        /// Shorthand Ternary Operation
        /// </summary>
        /// 
        public static T Select<T>(bool condition, T consequent, T alternatine)
        {
            return (condition) ? consequent : alternatine;
        }

        /// <summary>
        /// Returns all fields of the specified type in the given class instance
        /// </summary>
        /// 
        public static List<T> GetAllFieldsFromTypeInObject<T>(object instance)
        {
            List<T> foundVars = new();

            foreach (System.Reflection.FieldInfo field in instance.GetType().GetFields())
            {
                if (field.FieldType.Equals(typeof(T)))
                {
                    foundVars.Add((T)field.GetValue(instance));
                }
            }
            return foundVars;
        }

        /// <summary>
        /// Returns true if object is within the given camera's frustrum
        /// </summary>
        public static bool IsPointOnCamera(Vector3 objectWorldPosition, Camera camera)
        {
            Vector3 screenPoint = camera.WorldToViewportPoint(objectWorldPosition);

            return screenPoint.x>0 && screenPoint.x<1 && screenPoint.y>0 && screenPoint.y<1 && screenPoint.z>0;
        }

        /// <summary>
        /// Returns true if the object is within the designed area
        /// </summary>
        public static bool IsPointInArea(Vector3 objectWorldPosition, Vector3 areaWorldPosition, Vector3 areaSize)
        {
            return
                objectWorldPosition.x < areaWorldPosition.x + areaSize.x * 0.5f &&
                objectWorldPosition.x > areaWorldPosition.x - areaSize.x * 0.5f &&
                objectWorldPosition.y < areaWorldPosition.y + areaSize.y * 0.5f &&
                objectWorldPosition.y > areaWorldPosition.y - areaSize.y * 0.5f &&
                objectWorldPosition.z < areaWorldPosition.z + areaSize.z * 0.5f &&
                objectWorldPosition.z > areaWorldPosition.z - areaSize.z * 0.5f;
        }

        /// <summary>
        ///  Returns and array that chains array A and array B together
        /// </summary>
        public static T[] CombineArrays<T>(T[] a, T[] b)
        {
            T[] array = new T[a.Length + b.Length];

            for (int i = 0; i < a.Length; i++)
            {
                array[i] = a[i];
            }
            for (int i = 0; i < b.Length; i++)
            {
                array[a.Length + i] = b[i];
            }

            return array;
        }

        /// <summary>
        /// Returns the SubmeshIndex of this mesh, that contains the given triangle index
        /// </summary>
        public static int GetSubmeshFromTriangle(int triangleIndex, Mesh mesh)
        {
            int lim = triangleIndex * 3;

            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                int currentIndex = mesh.GetSubMesh(i).indexCount;

                if (currentIndex > lim) return i;
                lim -= currentIndex;
            }
            return 0;
        }

        /// <summary>
        /// Returns true if the index is comtained within an array of this lenght
        /// </summary>
        public static bool IsIndexInRange(int value, int range)
        {
            return value >= 0 && value < range;
        }
    }

    /// <summary>
    /// Provides a numerically integrated version of a function.
    /// </summary>
    public class IntegrateFunc
    {
        private System.Func<float, float> _func;
        private  float[] _values;
        private float _from, _to;

        /// <summary>
        /// Integrates a function on an interval. Use the steps parameter to control
        /// the precision of the numerical integration. Larger step values lead to
        /// better precision.
        /// </summary>
        public IntegrateFunc(System.Func<float, float> func,
                             float from, float to, int steps)
        {
            _values = new float[steps + 1];
            _func = func;
            _from = from;
            _to = to;
            ComputeValues();
        }

        private void ComputeValues()
        {
            int n = _values.Length;
            float segment = (_to - _from) / (n - 1);
            float lastY = _func(_from);
            float sum = 0;
            _values[0] = 0;
            for (int i = 1; i < n; i++)
            {
                float x = _from + i * segment;
                float nextY = _func(x);
                sum += segment * (nextY + lastY) / 2;
                lastY = nextY;
                _values[i] = sum;
            }
        }

        /// <summary>
        /// Evaluates the integrated function at any point in the interval.
        /// </summary>
        public float Evaluate(float x)
        {
            Debug.Assert(_from <= x && x <= _to);
            float t = Mathf.InverseLerp(_from, _to, x);
            int lower = (int)(t * _values.Length);
            int upper = (int)(t * _values.Length + .5f);
            if (lower == upper || upper >= _values.Length)
                return _values[lower];
            float innerT = Mathf.InverseLerp(lower, upper, t * _values.Length);
            return (1 - innerT) * _values[lower] + innerT * _values[upper];
        }

        /// <summary>
        /// Returns the total value integrated over the whole interval.
        /// </summary>
        public float Total
        {
            get
            {
                return _values[_values.Length - 1];
            }
        }
    }

    /// <summary>
    /// Samples according to a density given by an animation curve.
    /// This assumes that the animation curve is non-negative everywhere.
    /// </summary>
    public class AnimationCurveSampler
    {
        private readonly AnimationCurve _densityCurve;
        private readonly IntegrateFunc _integratedDensity;

        public AnimationCurveSampler(AnimationCurve curve, int integrationSteps = 100)
        {
            _densityCurve = curve;
            _integratedDensity = new IntegrateFunc(curve.Evaluate,
                                                   curve.keys[0].time,
                                                   curve.keys[curve.length - 1].time,
                                                   integrationSteps);
        }

        /// <summary>
        /// Takes a value s in [0, 1], scales it up to the interval
        /// [0, totalIntegratedValue] and computes its inverse.
        /// </summary>
        private float Invert(float s)
        {
            s *= _integratedDensity.Total;
            float lower = MinT;
            float upper = MaxT;
            const float precision = 0.00001f;
            while (upper - lower > precision)
            {
                float mid = (lower + upper) / 2f;
                float d = _integratedDensity.Evaluate(mid);
                if (d > s)
                {
                    upper = mid;
                }
                else if (d < s)
                {
                    lower = mid;
                }
                else
                {
                    // unlikely :)
                    return mid;
                }
            }

            return (lower + upper) / 2f;
        }

        public float TransformUnit(float unitValue)
        {
            return Invert(unitValue);
        }

        public float Sample()
        {
            return Invert(UnityEngine.Random.value);
        }

        private float MinT
        {
            get { return _densityCurve.keys[0].time; }
        }

        private float MaxT
        {
            get { return _densityCurve.keys[_densityCurve.length - 1].time; }
        }
    }
}
