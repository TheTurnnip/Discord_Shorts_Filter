using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Shorts_Filter.tools
{
    internal class ValidatedChannelName
    {
        public string ValidName { get; private set; }
        public string UnValidatedName { get; private set; }

        public ValidatedChannelName(string unvalidatedName)
        {
            if (unvalidatedName == null)
            {
                throw new ArgumentNullException(unvalidatedName, "A null channel name can not be validated!");
            }

            UnValidatedName = unvalidatedName;
            ValidatedName(unvalidatedName);
        }

        private void ValidatedName(string originalName)
        {
            foreach (char letter in originalName)
            {
                if (letter == ' ')
                {
                    ValidName += "_";
                }
                else if (char.IsUpper(letter))
                {
                    ValidName += letter.ToString().ToLower();
                }
                else
                {
                    ValidName += letter;
                }
            } 
        }
    }
}
