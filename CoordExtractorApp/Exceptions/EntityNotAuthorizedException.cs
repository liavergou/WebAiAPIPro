
// Για περιπτώσεις όπου ένας χρήστης προσπαθεί να εκτελέσει μια ενέργεια για την οποία δεν έχει την απαιτούμενη εξουσιοδότηση.
// Κληρονομεί από την `AppException` για να ενσωματωθεί στο γενικό σύστημα διαχείρισης σφαλμάτων.
namespace CoordExtractorApp.Exceptions
{
    /// <summary>
    /// Exception thrown when a user attempts to perform an action they are not authorized for.
    /// </summary>
    public class EntityNotAuthorizedException : AppException
    {
      
        private static readonly string DEFAULT_CODE = "NotAuthorized"; //σταθερός κωδικός που προστίθεται στο τέλος του κωδικού σφάλματος

        //constructor
        public EntityNotAuthorizedException(string code, string message)
            
            : base(code + DEFAULT_CODE, message)
        {
        }
    }
}