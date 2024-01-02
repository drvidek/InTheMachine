using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QKit
{
    public enum VectorValue { x, y, z };

    public class QMath
    {
        /// <summary>
        /// Return a Vector3 from a string-formatted Vector 3, i.e. "(0,0,0)"
        /// </summary>
        /// <param name="sVector"></param>
        /// <returns></returns>
        public static Vector3 StringToVector3(string sVector)
        {
            // Remove the parentheses
            if (sVector.StartsWith("(") && sVector.EndsWith(")"))
            {
                sVector = sVector.Substring(1, sVector.Length - 2);
            }

            // split the items
            string[] sArray = sVector.Split(',');

            // store as a Vector3
            Vector3 result = new Vector3(
                float.Parse(sArray[0]),
                float.Parse(sArray[1]),
                float.Parse(sArray[2]));

            return result;
        }

        /// <summary>
        /// Return true 1/n times.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static bool Roll(int n)
        {
            int i = Random.Range(0, n);
            return (i == n - 1);
        }

        /// <summary>
        /// Roll a n-sided die, return true 1/n times, and output the number rolled
        /// </summary>
        /// <param name="n"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool Roll(int n, out int result)
        {
            result = Random.Range(0, n) + 1;
            return (result == n);
        }

        /// <summary>
        /// Returns true if a is between b and c
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool Between(float a, float b, float c)
        {
            return a > b ? (a > b && a < c) : (a > c && a < b);
        }


        /// <summary>
        /// Gets the direction from one position to another, optional parameter to turn off normalising
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="normalised"></param>
        /// <returns></returns>
        public static Vector3 Direction(Vector3 from, Vector3 to, bool normalised = true)
        {
            Vector3 dir = to - from;
            if (normalised)
                dir = dir.normalized;
            return dir;
        }

        /// <summary>
        /// Return the difference between two numbers, always returns a positive value.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float Difference(float a, float b)
        {
            return Mathf.Abs(a - b);
        }

        /// <summary>
        /// Return the difference between two numbers, always returns a positive value.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int Difference(int a, int b)
        {
            return Mathf.Abs(a - b);
        }

        /// <summary>
        /// If n falls below min or above max, it will wrap to the other value
        /// </summary>
        /// <param name="n"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int WrapIndex(int n, int min, int max)
        {
            if (n < min)
            {
                n = max;
                return n;
            }
            if (n > max)
            {
                n = min;
                return n;
            }
            return n;
        }

        /// <summary>
        /// Clip float f to d decimal places
        /// </summary>
        /// <param name="f"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static float ClipToDecimalPlace(float f, float d)
        {
            float precision = Mathf.Max(1, Mathf.Pow(10, d));
            return Mathf.Ceil(f * precision) / precision;
        }

        /// <summary>
        /// Round float f to the nearest multiple of a provided fraction
        /// </summary>
        /// <param name="f"></param>
        /// <param name="fraction"></param>
        /// <returns></returns>
        public static float RoundToNearestFraction(float f, float fraction)
        {
            return Mathf.Round(f * (1 / fraction)) / (1 / fraction);
        }

        /// <summary>
        /// Returns a vector based on a source vector, replacing a single value using enum VectorValue.x, .y, or .z
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <param name="vectorValue"></param>
        /// <returns></returns>
        public static Vector3 ReplaceVectorValue(Vector3 source, VectorValue vectorValue, float value)
        {
            return new Vector3(
                vectorValue == VectorValue.x ? value : source.x,
                vectorValue == VectorValue.y ? value : source.y,
                vectorValue == VectorValue.z ? value : source.z
                );
        }

        /// <summary>
        /// Returns true if the provided layer is found within the mask
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public static bool DoesLayerMaskContain(LayerMask mask, int layer)
        {
            return mask == (mask | (1 << layer));
        }

        /// <summary>
        /// Returns a random entry from a provided array of variables defined by the user
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T Choose<T>(params T[] array)
        {
            return array[Random.Range(0, array.Length)];
        }

        /// <summary>
        /// Try to get type T out of a transform, its parent (if it exists), and its root.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transform"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGet<T>(Transform transform, out T result)
        {
            if (transform.TryGetComponent<T>(out result))
                return true;

            if (transform.parent != null && transform.parent.TryGetComponent<T>(out result))
                return true;

            return (transform.root.TryGetComponent<T>(out result));
        }

        /// <summary>
        /// Moves the given transform towards the target position at speed, ignoring collision. If the distance remaining is less than snapping distance, snaps to that position and returns true.
        /// </summary>
        /// <param name="transformToMove"></param>
        /// <param name="targetPosition"></param>
        /// <param name="speed"></param>
        /// <param name="snappingDistance"></param>
        /// <returns></returns>
        public static bool MoveTowardsAndSnap(Transform transformToMove, Vector3 targetPosition, float speed, float snappingDistance)
        {
            if (Vector3.Distance(transformToMove.position, targetPosition) < snappingDistance)
            {
                transformToMove.position = targetPosition;
                return true;
            }
            Vector3 direction = targetPosition - transformToMove.position;
            direction.Normalize();
            transformToMove.position += direction * speed;
            return false;
        }
    }
}