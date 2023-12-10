﻿namespace JJ_API.Service.Buisneess
{
   public enum Results
    {
        OK = 0,
        GeneralError,
        ErrorDuringAddingNewPhotos,
        ErrorDuringRemovingPhotos,
        NotFoundAnyTouristSpots,
        BadPassword,
        NoSuchAccount,
        LoginAlreadyInUse,
        EmailAlreadyInUse,
        BadEmail,
        NotInsertedTouristSpot,
        ErrorDuringAddingNewTouristSpots,
        ErrorDuringRemovingTouristSpots,
        ErrorDuringAddingNewComment,
        ErrorDuringRemovingComments,
        ErrorDuringAddingCitys,
        ErrorDuringRemovingCitys,
        NotFoundAnyCitys,
        MoreThanOneRecordHasBeenDeleted,
        NothingWasDeleted,
        VisitedSpotAlreadyAdded,
        UnexpectedError,
        AvatarNotInserted,
        RoouteNameAlreadyExist,
        RouteAlreadyAdded,
        CurseFoundInContent,
        IncorrectPasswordForUser,
        NotInsertedNotification,
        ErrorDuringSendingNotification,
        TooManyCommentsPast2Min,
        CommentNotValid,
        UserNotFound,
        CommentNoFound,
        CommentNofound,
        InputIsNull,
        ErrorDuringRemovingUser,
    }
}
