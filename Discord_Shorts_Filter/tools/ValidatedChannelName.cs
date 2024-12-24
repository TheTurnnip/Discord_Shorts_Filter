
namespace Discord_Shorts_Filter.Tools
{
    public class ValidatedChannelName
    {
        public string? ValidName {  get; private set; }
        public string UnValidatedName { get; private set; }

        public ValidatedChannelName(string unvalidatedName)
        {
            if (unvalidatedName == null)
            {
                throw new ArgumentNullException(unvalidatedName, "A null channel name can not be validated!");
            }
            
            UnValidatedName = unvalidatedName.Trim();
            
            foreach (char letter in UnValidatedName)
            {
                if (letter == ' ')
                {
                    ValidName += "-";
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
