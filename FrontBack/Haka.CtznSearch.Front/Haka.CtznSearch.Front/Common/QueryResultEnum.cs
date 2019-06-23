namespace Haka.CtznSearch.Front
{
    public enum QueryResultEnum : byte
    {
        Executed = 0, //!
        Exist = 2, //!
        NotFound = 3, //!
        Already = 4, //!
        NotConfirmed = 5, //!
        Saved = 6, //!

        //Unknown        
        Unknown = 20,
        UnknownParent = 21,        

        //NotFound                            
        NotFoundGeocode = 40,//!
        NotFoundCategory = 41,//!
        NotFoundKeyWord = 42,//!
        NotFoundPoint = 43,//!
        NotFoundActionCode = 44,//!
        NotFoundCrossServer = 45,
        NotFoundUser = 46,//!
        NotFoundSession = 47,
        NotFoundQuery = 48,

        //Wrong        
        WrongGeocode = 61,
        WrongAttributes = 62, //!
        WrongPassword = 63, //!
        WrongCaptcha = 64, //!
        WrongPhone = 65, //!
        WrongCode = 66, //!
        WrongControlList = 67, //!
        WrongLanguageSymbol = 68, //!

        //Lang
        WordUnknown = 81,
        WordCorected = 82,
        WordDublicated = 83, //имеются омонимы или синонимы, нет однозначности

        //Other
        NoCoordinates = 102,
        KeyWordNotInCategory = 103,
        AddedParent = 104, //!

        TooMuchTry=120, //!
        YouCantDoIt = 121, //!
        Blocked = 122, //!
        TooOffen=123, //!
        InWork = 124,
        MoreThanLimit = 125, //!
        AuthNeed = 126, //!

        
        /*
        //Other
        CorectedWord = 63,
        DublicatedWord = 64, //имеются омонимы или синонимы, нет однозначности
        NoCoordinates = 65,
        KeyWordNotInCategory = 66,
        AddedParent = 67,
        */

        //Warning
        WarningControlListContainsUnknownWord = 150, //!


        MoneyNotEnough = 170, //!
        MoneyUnknownTransferRequest = 171, //!
        MoneyTransferRequestInWrongStatus = 172, 

        //System
        Error_WrongFormat = 200, //!
        Error_WrongSigniture = 201, //!
        Error_SecurityError = 202, //!
        Error_SessionError = 203, //!
        Error_CrossAnswer = 248, //!
        Error_CrossConnection = 249, //!
        Error_Unknown = 250, //!
        Error_DBError = 251, //!
        Error_JSONFormatError = 254, //!
        Error_LostConnection = 255, //!
    }
}
