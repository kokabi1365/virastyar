// Author: Omid Kashefi, Mitra Nasri 
// Created on: 2010-March-08
// Last Modified: Omid Kashefi, Mitra Nasri at 2010-March-08
//

using System;
using System.Collections.Generic;

namespace SCICT.NLP.Utility.StringDistance
{    
    ///<summary>
    /// Calculate Euclidean and Cosine Distance between two letters on given keyboard layout
    ///</summary>
    public class KeyboardKeyDistance
    {
        # region Variables and properties
        private KeyboardConfig m_keyboardConfig;
        private readonly List<KeyboardKey> m_keys;

        ///<summary>
        /// Keyboard layout
        ///</summary>
        public KeyboardConfig CurrentConfig
        {
            get { return m_keyboardConfig; }
        }            

        double m_maximumDistance;
        double m_minimumDistance;

        #endregion

        #region Constractor
        /// <summary>
        /// default consturcor which also sets the language to Persian.
        /// </summary>
        public KeyboardKeyDistance()
        {
            this.m_keyboardConfig = new KeyboardConfig();
            this.m_keys = new List<KeyboardKey>();

            Reconfig(this.m_keyboardConfig);

        }
        #endregion

        /// <summary>
        /// this function adds a row of m_keys to m_keys list. a row contains real characters on keyboard and can start from "Q", "A" and "Z".
        /// </summary>
        /// <param name="startX">for "Q" row it is 1.0, for "A" row it is "1.5" and for "Z" row it is 2.</param>
        /// <param name="y">for "Q" row it is 3.0, for "A" row it is "2" and for "Z" row it is 1.</param>
        /// <param name="values">list of letters in each row, e.g. for a English standard qwerty keyboard layout it could be "asdfghjkl;'" for second row</param>
        private void AddARow(float startX, float y, string values)
        {
            for (int i = 0; i < values.Length; i++)
                this.m_keys.Add(new KeyboardKey(i + startX, y, values[i]));
        }

        /// <summary>
        /// this function computes maximum possible distance between m_keys in the defined keyboard.
        /// </summary>
        private void SetMaximumDistance()
        {
            double max = 0;
            for (int i = 0; i < this.m_keys.Count; i++)
            {
                for (int j = i + 1; j < this.m_keys.Count; j++)
                    max = Math.Max(max, EuclideanDistance(this.m_keys[i].Value, this.m_keys[j].Value));
            }
            this.m_maximumDistance = max;
        }

        /// <summary>
        /// this function computes minimum possible distance between m_keys in the defined keyboard.
        /// </summary>
        private void SetMinimumDistance()
        {
            this.m_minimumDistance = EuclideanDistance('ب', 'ل');
        }

        /// <summary>
        /// this function sets keyboadr language to Farsi and sets up Keys list.
        /// </summary>
        public void Reconfig(KeyboardConfig aConfig)
        {
            // there is a cartesian environment for key layout which is centered bottom left position of keyboard plate.
            // distance between two adjacent key in a row is 1.0 and height between two adjacent column is 1.0. 
            // but the second row of m_keys (<A> to <'> are start from position 1.5.

            this.m_keys.Clear();

            //  there is 30 key on keyboard in 3 rows.
            AddARow(1.0f, 2.0f, aConfig.FirstRowCharacters); // from <Q> to <]>
            AddARow(1.5f, 1.0f, aConfig.SecondRowCharacters); // from <A> to <'>
            AddARow(2.0f, 0.0f, aConfig.ThirdRowCharacters); // from <Z> to <,>            

            // after normal m_keys, there is still some other m_keys which should be set according to keyboard layout in farsi for some keyboards.
            foreach (KeyboardKey key in aConfig.OtherCharacters)
            {
                this.m_keys.Add(key);
            }

            this.m_keys.Sort();
            
            this.m_keyboardConfig = aConfig;
            
            SetMaximumDistance();
            SetMinimumDistance();
        }

        /// <summary>
        /// this function calculates simple Cosine distance between two characters on keyboard.
        /// Note that this distance is not normalized!
        /// If any of these characters doesn't exist in current keyboard setting, then this function returns Maximum Distance value specified in current object.
        /// </summary>
        /// <param name="ch1"></param>
        /// <param name="ch2"></param>
        /// <returns>returns distance between two characters in keyboard.</returns>
        public double CosineDistance(char ch1, char ch2)
        {
            KeyboardKey aKey1 = new KeyboardKey();
            KeyboardKey aKey2 = new KeyboardKey();

            aKey1.Value = ch1;
            int index1 = this.m_keys.BinarySearch(aKey1);

            if (index1 >= 0)
                aKey1 = this.m_keys[index1];
            else
                return this.m_maximumDistance;

            aKey2.Value = ch2;
            int index2 = this.m_keys.BinarySearch(aKey2);

            if (index2 >= 0)
                aKey2 = m_keys[index2];
            else
                return this.m_maximumDistance;

            double result = ((aKey1.X * aKey2.X) + (aKey1.Y * aKey2.Y)) / (Math.Sqrt(aKey1.X * aKey1.X + aKey1.Y * aKey1.Y) * Math.Sqrt(aKey2.X * aKey2.X + aKey2.Y * aKey2.Y));
            
            //if both m_keys use shift, so we don't include shift in their distance, else shift will add 1.0 ro their real distance
            if (aKey1.UseShift != aKey2.UseShift)
                result += 1;

            return result;
        }

        /// <summary>
        /// this function calculates simple Cosine distance between two characters on keyboard. 
        /// </summary>
        /// <param name="ch1"></param>
        /// <param name="ch2"></param>
        /// <returns>result is in range [0..1]. 0 means that two characters are near each other.</returns>
        public double NormalizedCosineDistance(char ch1, char ch2)
        {
            double result = EuclideanDistance(ch1, ch2);
            return result / this.m_maximumDistance;
        }
        
        /// <summary>
        /// this function calculates simple euclidean distance between two characters on keyboard.
        /// Note that this distance is not normalized!
        /// If any of these characters doesn't exist in current keyboard setting, then this function returns Maximum Distance value specified in current object.
        /// </summary>
        /// <param name="ch1"></param>
        /// <param name="ch2"></param>
        /// <returns>returns distance between two characters in keyboard.</returns>
        public double EuclideanDistance(char ch1, char ch2)
        {
            KeyboardKey aKey1 = new KeyboardKey();
            KeyboardKey aKey2 = new KeyboardKey();

            aKey1.Value = ch1;
            int index1 = this.m_keys.BinarySearch(aKey1);

            if (index1 >= 0)
                aKey1 = this.m_keys[index1];
            else
                return this.m_maximumDistance;

            aKey2.Value = ch2;
            int index2 = this.m_keys.BinarySearch(aKey2);

            if (index2 >= 0)
                aKey2 = m_keys[index2];
            else
                return this.m_maximumDistance;
            
            double result = Math.Sqrt((aKey1.X - aKey2.X) * (aKey1.X - aKey2.X) + (aKey1.Y - aKey2.Y) * (aKey1.Y - aKey2.Y));

            #region Shift Key
            if (aKey1.UseShift != aKey2.UseShift)
            {
                // There is difference beetween using Shift key mistakenly and not using it mistakenly.
                // Shift key rarely mistakenly used, but it's more possible the not to use shift key.

                if (aKey1.UseShift)
                {
                    //AVG distance
                    result += (m_maximumDistance + m_minimumDistance) / 2;
                }
                else if (aKey2.UseShift)
                {
                    //nothing more
                    result += 0;
                }
            }
            #endregion

            return result;
        }

        ///<summary>
        /// Minimum possible distance
        ///</summary>
        public double MinimumNormalizedDistance
        {
            get
            {
                return m_minimumDistance / m_maximumDistance;
            }
        }

        ///<summary>
        /// Maximum possible distance
        ///</summary>
        public double MaximumNormalizedDistance
        {
            get
            {
                return m_maximumDistance / m_maximumDistance;
            }
        }

        /// <summary>
        /// this function calculates simple euclidean distance between two characters on keyboard. 
        /// </summary>
        /// <param name="ch1"></param>
        /// <param name="ch2"></param>
        /// <returns>result is in range [0..1]. 0 means that two characters are near each other.</returns>
        public double NormalizedEuclideanDistance(char ch1, char ch2)
        {
            double result = EuclideanDistance(ch1, ch2);
            return result / this.m_maximumDistance;           
        }
    }
}
