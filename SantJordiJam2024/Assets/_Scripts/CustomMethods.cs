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
}
