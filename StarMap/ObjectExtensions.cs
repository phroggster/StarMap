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
using System;
using System.Collections.Generic;

namespace StarMap
{
    public static class ObjectExtensions
    {
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
    }
}

