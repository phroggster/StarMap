/*
 * Copyright © 2017 phroggie <phroggster@gmail.com>, StarMap development team
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this
 * file except in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software distributed under
 * the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
 * ANY KIND, either express or implied. See the License for the specific language
 * governing permissions and limitations under the License.
 */
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace StarMap
{
    public static class ObjectExtensions
    {
        public static float Clamp(this float val, float min, float max)
        {
            if (max < min)
                throw new ArgumentOutOfRangeException();
            else if (max == min || val == float.PositiveInfinity)
                return max;
            else if (val == float.NegativeInfinity)
                return min;
            else if (float.IsNaN(val))
                return float.NaN;
            else if (val <= min)
                return min;
            else if (val >= max)
                return max;
            else
                return val;
        }

        public static float Lerp(float value1, float value2, float amount)
        {
            float dist = (value2 - value1) * amount;
            return value1 + dist;
        }

        public static Vector4 ToVec4(this Color colour)
        {
            return new Vector4(Color4.ToXyz(colour));
        }

        public static Vector4 ToVec4(this Color4 colour)
        {
            return new Vector4(Color4.ToXyz(colour));
        }

        public static string SplitCapsWord(this string capslower)
        {
            List<int> positions = new List<int>();
            List<string> words = new List<string>();

            int start = 0;

            if (capslower[0] == '-' || char.IsWhiteSpace(capslower[0]))  // Remove leading dash or whitespace
                start = 1;

            for (int i = 1; i <= capslower.Length; i++)
            {
                char c0 = capslower[i - 1];
                char c1 = i < capslower.Length ? capslower[i] : '\0';
                char c2 = i < capslower.Length - 1 ? capslower[i + 1] : '\0';

                if (i == capslower.Length || // End of string
                    (i < capslower.Length - 1 && c0 >= 'A' && c0 <= 'Z' && c1 >= 'A' && c1 <= 'Z' && c2 >= 'a' && c2 <= 'z') || // UpperUpperLower
                    (((c0 >= 'a' && c0 <= 'z') || (c0 >= '0' && c0 <= '9')) && c1 >= 'A' && c1 <= 'Z') || // LowerdigitUpper
                    (c1 == '-' || c1 == ' ' || c1 == '\t' || c1 == '\r')) // dash or whitespace
                {
                    if (i > start)
                        words.Add(capslower.Substring(start, i - start));

                    if (i < capslower.Length && (c1 == '-' || c1 == ' ' || c1 == '\t' || c1 == '\r'))
                        start = i + 1;
                    else
                        start = i;
                }
            }

            return String.Join(" ", words);
        }

        public static float HalfPI { get; } = (float)Math.PI * 0.5f;
    }
}

