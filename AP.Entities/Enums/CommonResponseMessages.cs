namespace AP.Entities.Enums
{
    public static class CommonResponseMessages
    {
        public static string NoId
        {
            get {
                return "NO_ID";
            }
        }

        public static string EmptyName
        {
            get {
                return "EMPTY_NAME";
            }
        }

        public static string InvalidSlug
        {
            get {
                return "INVALID_SLUG";
            }
        }

        public static string NoPostFound
        {
            get {
                return "NO_POST_FOUND";
            }
        }

        public static string AuthorDoesNotExists
        {
            get {
                return "AUTHOR_DOES_NOT_EXISTS";
            }
        }

        public static string OneOfCategoriesDoesNotExists
        {
            get {
                return "ONE_OF_CATEGORIES_DOES_NOT_EXISTS";
            }
        }
    }
}