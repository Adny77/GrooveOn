class ApiConfig {
  static const String apiBase = String.fromEnvironment(
    'API_BASE_URL_MOBILE',
    defaultValue: 'http://10.0.2.2:5277',
  );

  static String get imagesUsers => "$apiBase/images/users";
  static String get imagesProperties => "$apiBase/images/playlists";

  static Map<String, String> imageFolders = {
    'users': imagesUsers,
    'playlists': imagesProperties,
  };
}
