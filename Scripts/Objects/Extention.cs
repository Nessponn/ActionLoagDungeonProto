using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extentions.UnityEngine.Vector
{
    public static class Extention
    {
        // ���K���i�y�ʔŁj
        public static Vector3 NormalizedEx(this Vector3 vec)
        {
            float len = Mathf.Sqrt(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z);
            return new Vector3(vec.x / len, vec.y / len, vec.z / len);
        }
    }

}