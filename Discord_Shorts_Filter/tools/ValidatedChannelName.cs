
namespace Discord_Shorts_Filter.Tools
{
    /// <summary>
    /// Represents a channel name that has been validated to
    /// work with discord.
    /// Illegal characters or dashes and replaced.
    /// </summary>
    public class ValidatedChannelName
    {
        /// <summary>
        /// The name that has been validated to work with discord.
        /// </summary>
        public string? ValidName {  get; private set; }
        
        /// <summary>
        /// The name before any validation and replacement has taken place.
        /// </summary>
        public string UnValidatedName { get; private set; }

        /// <summary>
        /// Creates an instance of the ValidatedChannelName class.
        /// </summary>
        /// <param name="unvalidatedName">The name to be validated.</param>
        /// <exception cref="ArgumentNullException">
        /// Raised when a null unValidatedName is passed into the constructor.
        /// </exception>
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
