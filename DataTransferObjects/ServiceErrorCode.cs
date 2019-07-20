namespace DTO
{
    public enum ServiceErrorCode
    {
        WrongUserOrPassword = 0,
        DuplicateUser = 1,
        ConfirmAccountFailed = 2,
        SetPasswordFailed = 3,
        GeneralError = 4,
        ErrorUpdatingUser = 5,
        ErrorUpdatingProfilePicture = 6,
        ErrorUpdatingRestaurantFeatured = 7
    }
}
